//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：BaseLogic2CS
//* クラス日本語名  ：業務コード親クラス１（2層C/S用）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/01/28  西野  大介        新規作成
//*  2009/03/13  西野  大介        トランザクション制御部品による制御。
//*  2010/03/11  西野  大介        自動振り分け対応でpublicメソッド直呼びをエラーにする。
//*  2010/11/16  西野  大介        DoBusinessLogicのIsolationLevelEnum無しオーバーロードを追加
//*  2012/06/18  西野  大介        OriginalStackTrace（ログ出力）の品質向上
//*  2012/09/03  西野  大介        Connection、Transaction等が不正な状態に陥ったらDamを解放
//**********************************************************************************

// System
using System;
using System.Data;
using System.Collections;

// 業務フレームワーク（循環参照になるため、参照しない）

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.RichClient.Business
{
    /// <summary>業務コード親クラス１（2層C/S用）</summary>
    /// <remarks>業務コード親クラス２、業務コード クラス、画面コード クラスから利用する。</remarks>
    public abstract class BaseLogic2CS
    {
        #region グローバル変数

        /// <summary>トランザクション制御シングルトン クラス</summary>
        /// <remarks>
        /// 初期化は起動時の１回のみであり、
        /// 読み取り専用のデータを保持する場合
        /// のみに適用するデザインパターンとする。
        /// </remarks>
        private static TransactionControl TC = new TransactionControl();

        #endregion

        #region インスタンス変数

        /// <summary>
        /// データアクセス制御クラス（ＤＡＭ）
        /// </summary>
        private static BaseDam _dam;

        /// <summary>
        /// 排他のためのクラス変数
        /// </summary>
        private static readonly object _lock = new object();

        #region 自動振り分け対応

        /// <summary>オリジナルのスタックトレース値</summary>
        protected string OriginalStackTrace = "";

        /// <summary>DoBusinessLogicから呼ばれたかフラグ</summary>
        private bool WasCalledFromDoBusinessLogic = false;

        /// <summary>戻り値の設定用メンバ変数</summary>
        /// <remarks>
        /// レイトバインドにおいて、例外発生時、戻り値（ref）が戻らないので、メンバ変数を使用
        /// </remarks>
        private BaseReturnValue _returnValue = null;

        /// <summary>戻り値の設定用メンバ変数のプロパティ</summary>
        /// <remarks>
        /// レイトバインドにおいて、例外発生時、戻り値（ref）が戻らないので、メンバ変数を使用
        /// </remarks>
        protected BaseReturnValue ReturnValue
        {
            set
            {
                // 引数は、ほぼ必ずセットするので、
                // ここで不正な呼び出しをチェック。
                if (this.WasCalledFromDoBusinessLogic)
                {
                    this._returnValue = value;
                }
                else
                {
                    // 不正な呼び出し
                    throw new FrameworkException(
                        FrameworkExceptionMessage.LB_ILLEGAL_CALL_CHECK_ERROR[0],
                        FrameworkExceptionMessage.LB_ILLEGAL_CALL_CHECK_ERROR[1]);
                }
            }
            get
            {
                return this._returnValue;
            }
        }

        #endregion

        #endregion

        #region メソッド

        #region 業務コード呼び出しメソッド（業務ロジックの入り口）

        /// <summary>
        /// 業務コード呼び出しメソッド（業務ロジックの入り口）
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <returns>戻り値クラス</returns>
        /// <remarks>
        /// クライアント側（2層C/S）用
        /// 画面コード クラスから利用する。
        /// </remarks>
        public BaseReturnValue DoBusinessLogic(BaseParameterValue parameterValue)
        {
            // IsolationLevelEnum.Userで呼び出す
            return this.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.User);
        }

        /// <summary>
        /// 業務コード呼び出しメソッド（業務ロジックの入り口）
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="iso">分離レベル（ＤＢＭＳ毎の分離レベルの違いを理解して設定すること）</param>
        /// <returns>戻り値クラス</returns>
        /// <remarks>
        /// クライアント側（2層C/S）用
        /// 画面コード クラスから利用する。
        /// </remarks>
        public BaseReturnValue DoBusinessLogic(
            BaseParameterValue parameterValue, DbEnum.IsolationLevelEnum iso)
        {
            // 戻り値クラス
            BaseReturnValue returnValue = null;

            // オリジナルのスタックトレース値のクリア
            this.OriginalStackTrace = "";

            // データアクセス制御クラス（ＤＡＭ）がグローバルなので、全てロックする。
            lock (BaseLogic2CS._lock)
            {

                if (BaseLogic2CS._dam == null)
                {
                    // データアクセス制御クラス（ＤＡＭ）が無い場合

                    // ★データアクセス制御クラス（ＤＡＭ）の生成し、コネクションを確立、
                    // トランザクションを開始する処理（業務フレームワークに、UOCで実装する）
                    this.UOC_ConnectionOpen(parameterValue, iso);
                }
                else
                {
                    // データアクセス制御クラス（ＤＡＭ）が有る場合
                }

                try
                {
                    // 自動振り分け対応
                    this.WasCalledFromDoBusinessLogic = true;

                    // ★前処理（業務フレームワークに、UOCで実装する）
                    this.UOC_PreAction(parameterValue);

                    // ★業務ロジックの実行（業務処理の派生クラスに、UOCで実装する）
                    this.UOC_DoAction(parameterValue, ref returnValue);

                    // ★後処理（業務フレームワークに、UOCで実装する）
                    this.UOC_AfterAction(parameterValue, returnValue);

                    //// トランザクション終了
                    //BaseLogic2CS._dam.CommitTransaction();
                    
                    //// ★トランザクション完了後の後処理（業務フレームワークに、UOCで実装する）
                    //this.UOC_AfterTransaction(parameterValue, returnValue);
                }
                catch (BusinessApplicationException baEx)// 業務例外
                {
                    // ★★業務例外時のロールバックは自動にしない。

                    // 業務例外の場合、エラーフラグをセットする。

                    // 戻り値がnullの場合は、生成する。
                    if (returnValue == null)
                    {
                        returnValue = new BaseReturnValue();
                    }

                    returnValue.ErrorFlag = true;

                    // メッセージを戻す（戻り値クラスに設定）。
                    returnValue.ErrorMessageID = baEx.messageID;
                    returnValue.ErrorMessage = baEx.Message;
                    returnValue.ErrorInfo = baEx.Information;

                    // ★異常系の後処理（業務フレームワークに、UOCで実装する）
                    this.UOC_ABEND(parameterValue, returnValue, baEx);

                    // 正常系の戻り値にして戻すため、リスローしない。
                }
                catch (BusinessSystemException bsEx)// システム例外
                {
                    // ★システム例外時は、自動的にロールバック。

                    // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここから）
                    // トランザクションをロールバック
                    if (BaseLogic2CS._dam == null)
                    {
                        // nullの場合はロールバックしない（何もしない）。
                    }
                    else
                    {
                        // 例外対策（例外は潰さない）
                        try
                        {
                            // nullでない場合はロールバックする。
                            BaseLogic2CS._dam.RollbackTransaction();
                            // コネクション クローズ
                            BaseLogic2CS._dam.ConnectionClose();
                        }
                        finally
                        {
                            // nullクリア（次回、再接続される。）
                            BaseLogic2CS._dam = null;
                        }
                    }
                    // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここまで）

                    // 戻り値がnullの場合は、生成する。
                    if (returnValue == null)
                    {
                        returnValue = new BaseReturnValue();
                    }

                    // ★異常系の後処理（業務フレームワークに、UOCで実装する）
                    this.UOC_ABEND(parameterValue, returnValue, bsEx);

                    // リスロー
                    throw;
                }
                catch (Exception Ex)// その他、一般的な例外
                {
                    // ★その他、一般的な例外は、自動的にロールバック。

                    // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここから）
                    // トランザクションをロールバック
                    if (BaseLogic2CS._dam == null)
                    {
                        // nullの場合はロールバックしない（何もしない）。
                    }
                    else
                    {
                        // 例外対策（例外は潰さない）
                        try
                        {
                            // nullでない場合はロールバックする。
                            BaseLogic2CS._dam.RollbackTransaction();
                            // コネクション クローズ
                            BaseLogic2CS._dam.ConnectionClose();
                        }
                        finally
                        {
                            // nullクリア（次回、再接続される。）
                            BaseLogic2CS._dam = null;
                        }
                    }
                    // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここまで）

                    // 戻り値がnullの場合は、生成する。
                    if (returnValue == null)
                    {
                        returnValue = new BaseReturnValue();
                    }

                    // ★異常系の後処理（業務フレームワークに、UOCで実装する）
                    this.UOC_ABEND(parameterValue, ref returnValue, Ex);

                    // リスローしない（上記のUOC_ABENDで必要に応じてリスロー）
                    // throw;
                }
                finally
                {
                    // 自動振り分け対応
                    this.WasCalledFromDoBusinessLogic = false;

                    // クライアント側（2層C/S）用では、マニュアル操作だが、
                    // ノートランザクションの時は、都度コネクションを閉じる。

                    // Damオブジェクトの存在チェック
                    if (BaseLogic2CS._dam == null)
                    {
                        // nullのためなにもしない。
                    }
                    else
                    {
                        // ノートランザクションの時は、都度コネクションを閉じる。
                        if (iso == DbEnum.IsolationLevelEnum.NoTransaction)
                        {
                            // 例外対策（例外は潰さない）
                            try
                            {
                                // コネクション クローズ
                                BaseLogic2CS._dam.ConnectionClose();
                            }
                            finally
                            {
                                // nullクリア（次回の「DoBusinessLogic_2CS」で再接続される。）
                                BaseLogic2CS._dam = null;
                            }
                        }
                    }
                }
            }

            // 戻り値を戻す。
            return returnValue;
        }

        #endregion

        #region データアクセス制御クラス（ＤＡＭ）取得・操作用

        /// <summary>データアクセス制御クラス（ＤＡＭ）を返す。</summary>
        /// <returns>データアクセス制御クラス（ＤＡＭ）</returns>
        /// <remarks>派生の業務コード親クラス２、業務コード クラスから利用する。</remarks>
        protected BaseDam GetDam()
        {
            // ＤＡＭは、ほぼ必ず取得するので、
            // ここで不正な呼び出しをチェック。
            if (this.WasCalledFromDoBusinessLogic)
            {
                // このメソッドはロックしない。
                // ※ メンバ アクセス修飾子（protected）でガードしている。
                return BaseLogic2CS._dam;
            }
            else
            {
                // 不正な呼び出し
                throw new FrameworkException(
                    FrameworkExceptionMessage.LB_ILLEGAL_CALL_CHECK_ERROR[0],
                    FrameworkExceptionMessage.LB_ILLEGAL_CALL_CHECK_ERROR[1]);
            }
        }

        // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここから）
        /// <summary>データアクセス制御クラス（ＤＡＭ）を設定する。</summary>
        /// <param name="dam">データアクセス制御クラス（ＤＡＭ）</param>
        /// <remarks>派生の業務コード親クラス２、業務コード クラスから利用する。</remarks>
        protected void SetDam(BaseDam dam)
        {
            // このメソッドはロックしない。
            // ※ メンバ アクセス修飾子（protected）でガードしている。
            BaseLogic2CS._dam = dam;
        }
        // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここまで）

        #endregion

        #region 業務コード親クラス２でオーバーライドするメソッド

        /// <summary>
        /// データアクセス制御クラス（ＤＡＭ）の生成し、
        /// コネクションを確立、トランザクションを開始する処理を実装
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="iso">分離レベル（ＤＢＭＳ毎の分離レベルの違いを理解して設定すること）</param>
        /// <remarks>派生の業務コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_ConnectionOpen(
            BaseParameterValue parameterValue,
            DbEnum.IsolationLevelEnum iso) { return; }

        /// <summary>
        /// Ｂ層の開始処理を実装
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <remarks>派生の業務コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_PreAction(BaseParameterValue parameterValue) { }

        /// <summary>
        /// 業務処理を実装
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        /// <remarks>派生の業務コード クラスでオーバーライドする。</remarks>
        protected abstract void UOC_DoAction(BaseParameterValue parameterValue, ref BaseReturnValue returnValue);

        /// <summary>
        /// Ｂ層の終了処理を実装
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        /// <remarks>派生の業務コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_AfterAction(BaseParameterValue parameterValue, BaseReturnValue returnValue) { }

        /// <summary>
        /// Ｂ層のトランザクションのコミット後の終了処理を実装
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        /// <remarks>派生の業務コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_AfterTransaction(BaseParameterValue parameterValue, BaseReturnValue returnValue) { }


        /// <summary>
        /// Ｂ層の業務例外による異常終了の後処理を実装するUOCメソッド。
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        /// <param name="baEx">BusinessApplicationException</param>
        /// <remarks>派生の業務コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_ABEND(BaseParameterValue parameterValue, BaseReturnValue returnValue, BusinessApplicationException baEx) { }

        /// <summary>
        /// Ｂ層のシステム例外による異常終了の後処理を実装するUOCメソッド。
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        /// <param name="bsEx">BusinessSystemException</param>
        /// <remarks>派生の業務コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_ABEND(BaseParameterValue parameterValue, BaseReturnValue returnValue, BusinessSystemException bsEx) { }

        /// <summary>
        /// Ｂ層の一般的な例外による異常終了の後処理を実装するUOCメソッド。
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        /// <param name="ex">Exception</param>
        /// <remarks>派生の業務コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_ABEND(BaseParameterValue parameterValue, ref BaseReturnValue returnValue, Exception ex) { }

        #endregion

        #region トランザクション制御部品
        // 2009/03/13---トランザクション制御部品による制御（ここから）

        /// <summary>トランザクション グループIDからトランザクション パターンIDを取得する。</summary>
        /// <param name="TransactionGroupID">トランザクション グループID</param>
        /// <param name="TransactionPatternID">トランザクション パターンID（配列）</param>
        /// <remarks>派生の業務コード親クラス２、業務コード クラスから利用する。</remarks>
        protected static void GetTransactionPatterns(string TransactionGroupID, out string[] TransactionPatternID)
        {
            BaseLogic2CS.TC.GetTransactionPatterns(TransactionGroupID, out TransactionPatternID);
        }

        /// <summary>データアクセス制御クラス（DAM）を初期化する</summary>
        /// <param name="businessID">業務ID</param>
        /// <param name="dam">データアクセス制御クラス（DAM）インスタンス</param>
        /// <remarks>派生の業務コード親クラス２、業務コード クラスから利用する。</remarks>
        protected static void InitDam(string businessID, BaseDam dam)
        {
            BaseLogic2CS.TC.InitDam(businessID, dam);
        }

        // 2009/03/13---トランザクション制御部品による制御（ここまで）
        #endregion

        #region トランザクション手動制御用

        /// <summary>コミット ＋ コネクション クローズ処理</summary>
        /// <remarks>
        /// 接続・切断オーバーヘッドは、コネクションプールにより回避する。
        /// 画面コード クラスから利用する。
        /// </remarks>
        public static void CommitAndClose()
        {
            // データアクセス制御クラス（ＤＡＭ）がグローバルなので、全てロックする。
            lock (BaseLogic2CS._lock)
            {
                if (BaseLogic2CS._dam == null) { }
                else
                {
                    // 例外対策（例外は潰さない）
                    try
                    {
                        // コミット
                        BaseLogic2CS._dam.CommitTransaction();
                        // コネクション クローズ
                        BaseLogic2CS._dam.ConnectionClose();
                    }
                    finally
                    {
                        // nullクリア（次回の「DoBusinessLogic_2CS」で再接続される。）
                        BaseLogic2CS._dam = null;
                    }
                }
            }
        }

        /// <summary>ロールバック ＋ コネクション クローズ処理</summary>
        /// <remarks>
        /// 接続・切断オーバーヘッドは、コネクションプールにより回避する。
        /// 画面コード クラスから利用する。
        /// </remarks>
        public static void RollbackAndClose()
        {
            // データアクセス制御クラス（ＤＡＭ）がグローバルなので、全てロックする。
            lock (BaseLogic2CS._lock)
            {
                if (BaseLogic2CS._dam == null) { }
                else
                {
                    // 例外対策（例外は潰さない）
                    try
                    {
                        // ロールバック
                        BaseLogic2CS._dam.RollbackTransaction();
                        // コネクション クローズ
                        BaseLogic2CS._dam.ConnectionClose();
                    }
                    finally
                    {
                        // nullクリア（次回の「DoBusinessLogic_2CS」で再接続される。）
                        BaseLogic2CS._dam = null;
                    }
                }
            }
        }

        /// <summary>コネクション クローズ処理</summary>
        /// <remarks>
        /// トランザクションを開始していない場合のコネクションクローズの方法
        /// 画面コード クラスから利用する。
        /// </remarks>
        public static void ConnectionClose()
        {
            // データアクセス制御クラス（ＤＡＭ）がグローバルなので、全てロックする。
            lock (BaseLogic2CS._lock)
            {
                if (BaseLogic2CS._dam == null) { }
                else
                {
                    // 例外対策（例外は潰さない）
                    try
                    {
                        // コネクション クローズ
                        BaseLogic2CS._dam.ConnectionClose();
                    }
                    finally
                    {
                        // nullクリア（次回の「DoBusinessLogic_2CS」で再接続される。）
                        BaseLogic2CS._dam = null;
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}

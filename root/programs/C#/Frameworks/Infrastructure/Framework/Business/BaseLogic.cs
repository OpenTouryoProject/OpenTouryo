//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
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
//* クラス名        ：BaseLogic
//* クラス日本語名  ：業務コード親クラス１（サーバ側、DBMSトランザクション用）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野 大介         新規作成
//*  2009/01/28  西野 大介         モジュール追加により、コメントなどを修正。
//*  2009/01/28  西野 大介         GetDam()のメンバ アクセス修飾子をprotectedに変更。
//*  2009/04/17  西野 大介         トランザクション手動制御を可能に。
//*  2009/04/17  西野 大介         UOC_ConnectionOpenの戻りをvoidに（SetDam()利用に統一）。
//*  2009/04/17  西野 大介         データアクセス制御クラスを配列化。
//*  2009/04/17  西野 大介         トランザクション制御部品による制御。
//*  2009/04/17  西野 大介         トランザクションパターンとグループの概念を追加。
//*  2010/03/11  西野 大介         自動振り分け対応でpublicメソッド直呼びをエラーにする。
//*  2010/09/24  西野 大介         ジェネリック対応（Dictionary、List、Queue、Stack<T>）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2010/11/16  西野 大介         DoBusinessLogicのIsolationLevelEnum無しオーバーロード
//*  2012/06/18  西野 大介         OriginalStackTrace（ログ出力）の品質向上
//*  2017/02/14  西野 大介         DoBusinessLogicの非同期バージョン（DoBusinessLogicAsync）を追加
//*  2017/02/28  西野 大介         ExceptionDispatchInfoを取り入れ、OriginalStackTraceを削除
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Public.Db;

namespace Touryo.Infrastructure.Framework.Business
{
    /// <summary>業務コード親クラス１（サーバ側、DBMSトランザクション用）</summary>
    /// <remarks>業務コード親クラス２、業務コード クラス、画面コード クラスから利用する。</remarks>
    public abstract class BaseLogic
    {
        #region グローバル変数
        // 2009/03/13---トランザクション制御部品による制御（ここから）

        /// <summary>トランザクション制御シングルトン クラス</summary>
        /// <remarks>
        /// 初期化は起動時の１回のみであり、
        /// 読み取り専用のデータを保持する場合
        /// のみに適用するデザインパターンとする。
        /// </remarks>
        private static TransactionControl TC = new TransactionControl();

        // 2009/03/13---トランザクション制御部品による制御（ここまで）
        #endregion

        #region インスタンス変数

        /// <summary>
        /// データアクセス制御クラス（ＤＡＭ）
        /// </summary>
        private BaseDam _dam;

        // 2009/03/28---データアクセス制御クラスを配列化（ここから）

        /// <summary>
        /// データアクセス制御クラス（ＤＡＭ）を保持するディクショナリ
        /// </summary>
        private Dictionary<string, BaseDam> _dams = new Dictionary<string, BaseDam>();

        // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここまで）

        #region 自動振り分け対応

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

        #region 非同期バージョン

        /// <summary>DoBusinessLogicメソッドの非同期バージョン</summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <returns>戻り値クラス</returns>
        /// <remarks>画面コード クラスから利用する。</remarks>
        public Task<BaseReturnValue> DoBusinessLogicAsync(BaseParameterValue parameterValue)
        {
            return Task<BaseReturnValue>.Factory.StartNew(() => {
                return this.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.User);
            });
            // IsolationLevelEnum.Userで呼び出す
        }

        /// <summary>DoBusinessLogicメソッドの非同期バージョン</summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="iso">分離レベル（ＤＢＭＳ毎の分離レベルの違いを理解して設定すること）</param>
        /// <returns>戻り値クラス</returns>
        /// <remarks>画面コード クラスから利用する。</remarks>
        public Task<BaseReturnValue> DoBusinessLogicAsync(
            BaseParameterValue parameterValue, DbEnum.IsolationLevelEnum iso)
        {
            return Task<BaseReturnValue>.Factory.StartNew(() => {
                return this.DoBusinessLogic(parameterValue, iso);
            });
        }

        #endregion

            /// <summary>
            /// 業務コード呼び出しメソッド（業務ロジックの入り口）
            /// </summary>
            /// <param name="parameterValue">引数クラス</param>
            /// <returns>戻り値クラス</returns>
            /// <remarks>画面コード クラスから利用する。</remarks>
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
        /// <remarks>画面コード クラスから利用する。</remarks>
        public BaseReturnValue DoBusinessLogic(
            BaseParameterValue parameterValue, DbEnum.IsolationLevelEnum iso)
        {
            // 戻り値クラス
            BaseReturnValue returnValue = null;
            
            // ★データアクセス制御クラス（ＤＡＭ）の生成し、コネクションを確立、
            // トランザクションを開始する処理（業務フレームワークに、UOCで実装する）
            // this._dam = this.UOC_ConnectionOpen(parameterValue, iso);
            this.UOC_ConnectionOpen(parameterValue, iso);

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

                #region トランザクションをコミット

                // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここから）
                
                if (this._dam == null)
                {
                    // nullの場合はコミットしない（何もしない）。
                }
                else
                {
                    // nullでない場合はコミットする。
                    this._dam.CommitTransaction();
                }    
            
                // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここまで）

                // 2009/03/28---データアクセス制御クラスを配列化（ここから）
                
                foreach (string key in this._dams.Keys)
                {
                    // ここはforeachで取るので「キーなし」にならない
                    if (this._dams[key] == null)
                    {
                        // nullの場合はコミットしない（何もしない）。
                    }
                    else
                    {
                        // nullでない場合はコミットする。
                        ((BaseDam)this._dams[key]).CommitTransaction();
                    }
                }
                
                // 2009/03/28---データアクセス制御クラスを配列化（ここまで）

                #endregion

                // ★トランザクション完了後の後処理（業務フレームワークに、UOCで実装する）
                this.UOC_AfterTransaction(parameterValue, returnValue);
            }
            catch (BusinessApplicationException baEx)// 業務例外
            {
                #region トランザクションをロールバック

                // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここから）

                if (this._dam == null)
                {
                    // nullの場合はロールバックしない（何もしない）。
                }
                else
                {
                    // nullでない場合はロールバックする。
                    this._dam.RollbackTransaction();
                }
                
                // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここまで）

                // 2009/03/28---データアクセス制御クラスを配列化（ここから）

                foreach (string key in this._dams.Keys)
                {
                    // ここはforeachで取るので「キーなし」にならない
                    if (this._dams[key] == null)
                    {
                        // nullの場合はロールバックしない（何もしない）。
                    }
                    else
                    {
                        // nullでない場合はロールバックする。
                        ((BaseDam)this._dams[key]).RollbackTransaction();
                    }
                }
                
                // 2009/03/28---データアクセス制御クラスを配列化（ここまで）

                #endregion

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
                #region トランザクションをロールバック

                // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここから）

                if (this._dam == null)
                {
                    // nullの場合はロールバックしない（何もしない）。
                }
                else
                {
                    // nullでない場合はロールバックする。
                    this._dam.RollbackTransaction();
                }

                // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここまで）

                // 2009/03/28---データアクセス制御クラスを配列化（ここから）

                foreach (string key in this._dams.Keys)
                {
                    // ここはforeachで取るので「キーなし」にならない
                    if (this._dams[key] == null)
                    {
                        // nullの場合はロールバックしない（何もしない）。
                    }
                    else
                    {
                        // nullでない場合はロールバックする。
                        ((BaseDam)this._dams[key]).RollbackTransaction();
                    }
                }

                // 2009/03/28---データアクセス制御クラスを配列化（ここまで）

                #endregion

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
                #region トランザクションをロールバック

                // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここから）

                if (this._dam == null)
                {
                    // nullの場合はロールバックしない（何もしない）。
                }
                else
                {
                    // nullでない場合はロールバックする。
                    this._dam.RollbackTransaction();
                }

                // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここまで）

                // 2009/03/28---データアクセス制御クラスを配列化（ここから）

                foreach (string key in this._dams.Keys)
                {
                    // ここはforeachで取るので「キーなし」にならない
                    if (this._dams[key] == null)
                    {
                        // nullの場合はロールバックしない（何もしない）。
                    }
                    else
                    {
                        // nullでない場合はロールバックする。
                        ((BaseDam)this._dams[key]).RollbackTransaction();
                    }
                }

                // 2009/03/28---データアクセス制御クラスを配列化（ここまで）

                #endregion

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

                #region コネクションを閉じる

                // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここから）
                
                if (this._dam == null)
                {
                    // nullのためなにもしない。
                }
                else
                {
                    // コネクションを閉じる。
                    this._dam.ConnectionClose();
                }
                
                // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここまで）

                // 2009/03/28---データアクセス制御クラスを配列化（ここから）

                foreach (string key in this._dams.Keys)
                {
                    // ここはforeachで取るので「キーなし」にならない
                    if (this._dams[key] == null)
                    {
                        // nullのためなにもしない。
                    }
                    else
                    {
                        // コネクションを閉じる。
                        ((BaseDam)this._dams[key]).ConnectionClose();
                    }
                }

                // 2009/03/28---データアクセス制御クラスを配列化（ここまで）

                #endregion
            }

            // 戻り値を戻す。
            return returnValue;
        }

        #endregion

        #region データアクセス制御クラス（ＤＡＭ）の取得・設定用メソッド

        /// <summary>データアクセス制御クラス（ＤＡＭ）を返す。</summary>
        /// <returns>データアクセス制御クラス（ＤＡＭ）</returns>
        /// <remarks>派生の業務コード親クラス２、業務コード クラスから利用する。</remarks>
        protected BaseDam GetDam()
        {
            // ＤＡＭは、ほぼ必ず取得するので、
            // ここで不正な呼び出しをチェック。
            if (this.WasCalledFromDoBusinessLogic)
            {
                return this._dam;
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
            this._dam = dam;
        }
        // 2009/03/13---Ｂ層内：トランザクション手動制御を可能に（ここまで）

        // 2009/03/28---データアクセス制御クラスを配列化（ここから）

        /// <summary>データアクセス制御クラス（ＤＡＭ）を返す。</summary>
        /// <param name="key">データアクセス制御クラス（ＤＡＭ）のキー</param>
        /// <returns>データアクセス制御クラス（ＤＡＭ）</returns>
        /// <remarks>派生の業務コード親クラス２、業務コード クラスから利用する。</remarks>
        protected BaseDam GetDam(string key)
        {
            // ＤＡＭは、ほぼ必ず取得するので、
            // ここで不正な呼び出しをチェック。
            if (this.WasCalledFromDoBusinessLogic)
            {
                // 2010/10/13 - ContainsKeyによるチェック処理を追加した。
                if (this._dams.ContainsKey(key))
                {
                    return (BaseDam)this._dams[key];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                // 不正な呼び出し
                throw new FrameworkException(
                    FrameworkExceptionMessage.LB_ILLEGAL_CALL_CHECK_ERROR[0],
                    FrameworkExceptionMessage.LB_ILLEGAL_CALL_CHECK_ERROR[1]);
            }
        }

        /// <summary>データアクセス制御クラス（ＤＡＭ）を設定する。</summary>
        /// <param name="key">データアクセス制御クラス（ＤＡＭ）のキー</param>
        /// <param name="dam">データアクセス制御クラス（ＤＡＭ）</param>
        /// <remarks>派生の業務コード親クラス２、業務コード クラスから利用する。</remarks>
        protected void SetDam(string key, BaseDam dam)
        {
            this._dams[key] = dam;
        }
        
        // 2009/03/28---データアクセス制御クラスを配列化（ここまで）

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
            BaseLogic.TC.GetTransactionPatterns(TransactionGroupID, out TransactionPatternID);
        }

        /// <summary>データアクセス制御クラス（DAM）を初期化する</summary>
        /// <param name="TransactionPatternID">トランザクション パターンID</param>
        /// <param name="dam">データアクセス制御クラス（DAM）インスタンス</param>
        /// <remarks>派生の業務コード親クラス２、業務コード クラスから利用する。</remarks>
        protected static void InitDam(string TransactionPatternID, BaseDam dam)
        {
            BaseLogic.TC.InitDam(TransactionPatternID, dam);
        }

        // 2009/03/13---トランザクション制御部品による制御（ここまで）
        #endregion

        #endregion
    }
}

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
//* クラス名        ：MyFcBaseLogic
//* クラス日本語名  ：自動振り分け機能付き業務コード親クラス２（サーバ用）（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//*  2010/03/30  西野 大介         ログ フォーマットにメソッド名を追加
//*  2010/09/24  西野 大介         共通引数クラス内にユーザ情報を格納したので
//*  2010/09/24  西野 大介         Damクラス内にユーザ情報を格納したので
//*  2011/01/24  西野 大介         上記コードが通るカバレージを修正
//*  2011/01/31  西野 大介         Damがnullの場合は処理しないように修正
//*  2012/02/09  西野 大介         OLEDB、ODBCのデータプロバイダ対応
//*  2012/04/05  西野 大介         \n → \r\n 化
//*  2012/06/18  西野  大介        OriginalStackTrace（ログ出力）の品質向上
//**********************************************************************************

// デバッグ用
using System.Diagnostics;

// メソッドの属性を取得
using System.Reflection;

// System
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;

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

namespace Touryo.Infrastructure.Business.Business
{
    /// <summary>自動振り分け機能付き業務コード親クラス２（サーバ用）（テンプレート）</summary>
    /// <remarks>（オーバーライドして）自由に利用できる。</remarks>
    public abstract class MyFcBaseLogic : BaseLogic
    {
        /// <summary>性能測定</summary>
        private PerformanceRecorder perfRec;

        #region メソッド

        #region 処理の自動振り分け

        /// <summary>自動振り分け処理</summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        protected override void UOC_DoAction(BaseParameterValue parameterValue, ref BaseReturnValue returnValue)
        {
            // メソッド名を生成
            string methodName = "UOC_" + parameterValue.MethodName;

            #region レイトバインドする

            object[] paramSet = new object[] { parameterValue };

            try
            {
                // Latebind
                Latebind.InvokeMethod(this, methodName, paramSet);
            }
            catch (System.Reflection.TargetInvocationException rtEx)
            {
                // InnerExceptionのスタックトレースを保存しておく（以下のリスローで消去されるため）。
                this.OriginalStackTrace = rtEx.InnerException.StackTrace;

                // InnerExceptionを投げなおす。
                throw rtEx.InnerException;
            }
            finally
            {
                // レイトバインドにおいて、
                // ・ 戻り値（in）の場合、下位で生成した戻り値インスタンスは戻らない。
                // ・ 戻り値（ref, out）の場合、例外発生時は戻り値インスタンスは戻らない。
                // という問題がある。

                // ∴ （特に後者の対応のため、）
                // メンバ変数を使用して戻り値インスタンスを戻す。
                returnValue = this.ReturnValue;
            }

            #endregion
        }

        #endregion

        #region ＤＢ接続

        /// <summary>データアクセス制御クラス（ＤＡＭ）の生成し、コネクションを確立、トランザクションを開始する処理を実装</summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="iso">分離レベル（ＤＢＭＳ毎の分離レベルの違いを理解して設定すること）</param>
        /// <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_ConnectionOpen(
            BaseParameterValue parameterValue,
            DbEnum.IsolationLevelEnum iso)
        {
            #region トランザクション属性取得例

            //// クラスの属性、メソッドの属性から調査
            //MyAttribute[] aryMCA;
            //MyAttribute[] aryMMA;

            //// クラスの属性を取得
            //MyAttribute.GetAttr(this, out aryMCA);

            //foreach (MyAttribute mca in aryMCA)
            //{
            //    Debug.WriteLine(this.GetType().ToString() + ".MyAttributeA = " + mca.MyAttributeA);
            //    Debug.WriteLine(this.GetType().ToString() + ".MyAttributeB = " + mca.MyAttributeB);
            //    Debug.WriteLine(this.GetType().ToString() + ".MyAttributeC = " + mca.MyAttributeC);
            //    Debug.WriteLine("+------------------+");
            //}

            //// メソッドの属性を取得
            //MethodInfo[] aryMtdInfo = this.GetType().GetMethods();

            //foreach (MethodInfo mtdInfo in aryMtdInfo)
            //{
            //    MyAttribute.GetAttr(mtdInfo, out aryMMA);

            //    foreach (MyAttribute mma in aryMMA)
            //    {
            //        Debug.WriteLine(mtdInfo.Name + ".MyAttributeA = " + mma.MyAttributeA);
            //        Debug.WriteLine(mtdInfo.Name + ".MyAttributeB = " + mma.MyAttributeB);
            //        Debug.WriteLine(mtdInfo.Name + ".MyAttributeC = " + mma.MyAttributeC);
            //        Debug.WriteLine("+------------------+");
            //    }
            //}

            #endregion

            // データアクセス制御クラス（ＤＡＭ）
            BaseDam dam = null;

            #region 接続

            if (iso == DbEnum.IsolationLevelEnum.NotConnect)
            {
                // 接続しない
            }
            else
            {
                // 接続する

                string connstring = "";

                #region データ プロバイダ選択

                // SQL Server / SQL Client用のDamを生成
                dam = new DamSqlSvr();

                // 接続文字列をロード
                connstring = GetConfigParameter.GetConnectionString("ConnectionString_SQL");

                //if (parameterValue.ActionType.Split('%')[0] == "SQL")
                //{
                //    // SQL Server / SQL Client用のDamを生成
                //    dam = new DamSqlSvr();

                //    // 接続文字列をロード
                //    connstring = GetConfigParameter.GetConnectionString("ConnectionString_SQL");
                //}
                //else if (parameterValue.ActionType.Split('%')[0] == "OLE")
                //{
                //    // OLEDB.NET用のDamを生成
                //    dam = new DamOLEDB();

                //    // 接続文字列をロード
                //    connstring = GetConfigParameter.GetConnectionString("ConnectionString_OLE");
                //}
                //else if (parameterValue.ActionType.Split('%')[0] == "ODB")
                //{
                //    // ODBC.NET用のDamを生成
                //    dam = new DamODBC();

                //    // 接続文字列をロード
                //    connstring = GetConfigParameter.GetConnectionString("ConnectionString_ODBC");
                //}
                //else if (parameterValue.ActionType.Split('%')[0] == "ORA")
                //{
                //    // Oracle / Oracle Client用のDamを生成
                //    dam = new DamOraClient();

                //    // 接続文字列をロード
                //    connstring = GetConfigParameter.GetConnectionString("ConnectionString_ORA");
                //}
                //else if (parameterValue.ActionType.Split('%')[0] == "ODP")
                //{
                //    // Oracle / ODP.NET用のDamを生成
                //    dam = new DamOraOdp();

                //    // 接続文字列をロード（ODP2：Instant Client）
                //    connstring = GetConfigParameter.GetConnectionString("ConnectionString_ODP2");
                //}
                //else if (parameterValue.ActionType.Split('%')[0] == "DB2")
                //{
                //    // DB2.NET用のDamを生成
                //    dam = new DamDB2();

                //    // 接続文字列をロード
                //    connstring = GetConfigParameter.GetConnectionString("ConnectionString_DB2");
                //}
                //else if (parameterValue.ActionType.Split('%')[0] == "HIR")
                //{
                //    // HiRDBデータプロバイダ用のDamを生成
                //    dam = new DamHiRDB();

                //    // 接続文字列をロード
                //    connstring = GetConfigParameter.GetConnectionString("ConnectionString_HIR");
                //}
                //else if (parameterValue.ActionType.Split('%')[0] == "MCN")
                //{
                //    // MySQL Cnn/NET用のDamを生成
                //    dam = new DamMySQL();

                //    // 接続文字列をロード
                //    connstring = GetConfigParameter.GetConnectionString("ConnectionString_MCN");
                //}
                //else if (parameterValue.ActionType.Split('%')[0] == "NPS")
                //{
                //    // PostgreSQL / Npgsql用のDamを生成
                //    dam = new DamPstGrS();

                //    // 接続文字列をロード
                //    connstring = GetConfigParameter.GetConnectionString("ConnectionString_NPS");
                //}
                //else
                //{
                //    // ここは通らない
                //}

                #endregion

                if (dam != null)
                {
                    // コネクションをオープンする。
                    dam.ConnectionOpen(connstring);

                    #region トランザクションを開始する。

                    if (iso == DbEnum.IsolationLevelEnum.User)
                    {
                        // 自動トランザクション（規定の分離レベル）
                        dam.BeginTransaction(DbEnum.IsolationLevelEnum.ReadCommitted);
                    }
                    else
                    {
                        // 自動トランザクション（指定の分離レベル）
                        dam.BeginTransaction(iso);
                    }

                    #endregion

                    // ユーザ情報を格納する（ログ出力で利用）。
                    dam.Obj = ((MyParameterValue)parameterValue).User;

                    // damを設定する。
                    this.SetDam(dam);
                }
            }

            #endregion
        }

        #endregion

        #region 開始・終了処理

        /// <summary>
        /// Ｂ層の開始処理を実装
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_PreAction(BaseParameterValue parameterValue)
        {
            // ACCESSログ出力-----------------------------------------------

            MyParameterValue myPV = (MyParameterValue)parameterValue;

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス, レイヤ, 
            // 画面名, コントロール名, メソッド名, 処理名
            // ------------
            string strLogMessage =
                "," + myPV.User.UserName +
                "," + myPV.User.IPAddress +
                "," + "----->>" +
                "," + myPV.ScreenId +
                "," + myPV.ControlId +
                "," + myPV.MethodName +
                "," + myPV.ActionType;

            // Log4Netへログ出力
            LogIF.InfoLog("ACCESS", strLogMessage);

            // -------------------------------------------------------------

            // 性能測定開始
            this.perfRec = new PerformanceRecorder();
            this.perfRec.StartsPerformanceRecord();

        }

        /// <summary>
        /// Ｂ層の終了処理を実装
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        /// <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_AfterAction(BaseParameterValue parameterValue, BaseReturnValue returnValue)
        {
            // 性能測定終了
            this.perfRec.EndsPerformanceRecord();

            // ACCESSログ出力-----------------------------------------------

            MyParameterValue myPV = (MyParameterValue)parameterValue;

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス, レイヤ, 
            // 画面名, コントロール名, メソッド名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // ------------
            string strLogMessage =
                "," + myPV.User.UserName +
                "," + myPV.User.IPAddress +
                "," + "<<-----" +
                "," + myPV.ScreenId +
                "," + myPV.ControlId +
                "," + myPV.MethodName +
                "," + myPV.ActionType +
                "," + this.perfRec.ExecTime +
                "," + this.perfRec.CpuTime;

            // Log4Netへログ出力
            LogIF.InfoLog("ACCESS", strLogMessage);

            // -------------------------------------------------------------            
        }

        /// <summary>
        /// Ｂ層のトランザクションのコミット後の終了処理を実装
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        /// <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_AfterTransaction(BaseParameterValue parameterValue, BaseReturnValue returnValue)
        {
            // TODO:
        }

        #endregion

        #region 例外処理

        /// <summary>
        /// Ｂ層の業務例外による異常終了の後処理を実装するUOCメソッド。
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        /// <param name="baEx">BusinessApplicationException</param>
        /// <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_ABEND(BaseParameterValue parameterValue, BaseReturnValue returnValue, BusinessApplicationException baEx)
        {
            // 業務例外発生時の処理を実装
            // TODO:

            // nullチェック
            if (this.perfRec == null)
            {
                // なにもしない
            }
            else
            {
                // 性能測定終了
                this.perfRec.EndsPerformanceRecord();

                // ACCESSログ出力-----------------------------------------------

                MyParameterValue myPV = (MyParameterValue)parameterValue;

                // ------------
                // メッセージ部
                // ------------
                // ユーザ名, IPアドレス, レイヤ, 
                // 画面名, コントロール名, メソッド名, 処理名
                // 処理時間（実行時間）, 処理時間（CPU時間）
                // エラーメッセージID, エラーメッセージ等
                // ------------
                string strLogMessage =
                    "," + myPV.User.UserName +
                    "," + myPV.User.IPAddress +
                    "," + "<<-----" +
                    "," + myPV.ScreenId +
                    "," + myPV.ControlId +
                    "," + myPV.MethodName +
                    "," + myPV.ActionType +
                    "," + this.perfRec.ExecTime +
                    "," + this.perfRec.CpuTime +
                    "," + baEx.messageID +
                    "," + baEx.Message;

                // Log4Netへログ出力
                LogIF.WarnLog("ACCESS", strLogMessage);
            }

            // -------------------------------------------------------------   
        }

        /// <summary>
        /// Ｂ層のシステム例外による異常終了の後処理を実装するUOCメソッド。
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        /// <param name="bsEx">BusinessSystemException</param>
        /// <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_ABEND(BaseParameterValue parameterValue, BaseReturnValue returnValue, BusinessSystemException bsEx)
        {
            // システム例外発生時の処理を実装
            // TODO:

            // nullチェック
            if (this.perfRec == null)
            {
                // なにもしない
            }
            else
            {
                // 性能測定終了
                this.perfRec.EndsPerformanceRecord();

                // ACCESSログ出力-----------------------------------------------

                MyParameterValue myPV = (MyParameterValue)parameterValue;

                // ------------
                // メッセージ部
                // ------------
                // ユーザ名, IPアドレス, レイヤ, 
                // 画面名, コントロール名, メソッド名, 処理名
                // 処理時間（実行時間）, 処理時間（CPU時間）
                // エラーメッセージID, エラーメッセージ等
                // ------------
                string strLogMessage =
                        "," + myPV.User.UserName +
                        "," + myPV.User.IPAddress +
                        "," + "<<-----" +
                        "," + myPV.ScreenId +
                        "," + myPV.ControlId +
                        "," + myPV.MethodName +
                        "," + myPV.ActionType +
                        "," + this.perfRec.ExecTime +
                        "," + this.perfRec.CpuTime +
                        "," + bsEx.messageID +
                        "," + bsEx.Message + "\r\n";
                // OriginalStackTrace（ログ出力）の品質向上
                if (this.OriginalStackTrace == "")
                {
                    strLogMessage += bsEx.StackTrace;
                }
                else
                {
                    strLogMessage += this.OriginalStackTrace;
                }
                
                // Log4Netへログ出力
                LogIF.ErrorLog("ACCESS", strLogMessage);
            }

            // -------------------------------------------------------------   
        }

        /// <summary>
        /// Ｂ層の一般的な例外による異常終了の後処理を実装するUOCメソッド。
        /// </summary>
        /// <param name="parameterValue">引数クラス</param>
        /// <param name="returnValue">戻り値クラス</param>
        /// <param name="ex">Exception</param>
        /// <remarks>業務コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_ABEND(BaseParameterValue parameterValue, ref BaseReturnValue returnValue, Exception ex)
        {
            // 一般的な例外発生時の処理を実装
            // TODO:

            // nullチェック
            if (this.perfRec == null)
            {
                // なにもしない

                // リスロー
                throw ex;
            }
            else
            {
                // 性能測定終了
                this.perfRec.EndsPerformanceRecord();

                // キャスト
                MyParameterValue myPV = (MyParameterValue)parameterValue;

                // システム例外に振り替える用のワーク
                bool sysErrorFlag = false;
                string sysErrorMessageID = "";
                string sysErrorMessage = "";

                #region 例外の振替処理のIF文

                if (ex.Message == "Other-Business")
                {
                    // 業務例外へ変換
                    returnValue.ErrorFlag = true;
                    returnValue.ErrorMessageID = "振替後";
                    returnValue.ErrorMessage = "振替後";
                    returnValue.ErrorInfo = "振り替える場合は、基本的にここを利用。";
                }
                else if (ex.Message == "Other-System")
                {
                    // システム例外へ振替
                    sysErrorFlag = true;
                    sysErrorMessageID = "振替後";
                    sysErrorMessage = "振替後";
                }

                #endregion

                #region ACCESSログ出力、リスローする・しない

                if (returnValue.ErrorFlag)
                {
                    // 業務例外へ変換

                    // ------------
                    // メッセージ部
                    // ------------
                    // ユーザ名, IPアドレス, レイヤ, 
                    // 画面名, コントロール名, メソッド名, 処理名
                    // 処理時間（実行時間）, 処理時間（CPU時間）
                    // エラーメッセージID, エラーメッセージ等
                    // ------------
                    string strLogMessage =
                        "," + myPV.User.UserName +
                        "," + myPV.User.IPAddress +
                        "," + "<<-----" +
                        "," + myPV.ScreenId +
                        "," + myPV.ControlId +
                        "," + myPV.MethodName +
                        "," + myPV.ActionType +
                        "," + this.perfRec.ExecTime +
                        "," + this.perfRec.CpuTime +
                        "," + returnValue.ErrorMessageID +
                        "," + returnValue.ErrorMessage;

                    // Log4Netへログ出力
                    LogIF.WarnLog("ACCESS", strLogMessage);
                }
                else if (sysErrorFlag)
                {
                    // システム例外へ振替

                    // ------------
                    // メッセージ部
                    // ------------
                    // ユーザ名, IPアドレス, レイヤ, 
                    // 画面名, コントロール名, メソッド名, 処理名
                    // 処理時間（実行時間）, 処理時間（CPU時間）
                    // エラーメッセージID, エラーメッセージ等
                    // ------------
                    string strLogMessage =
                        "," + myPV.User.UserName +
                        "," + myPV.User.IPAddress +
                        "," + "<<-----" +
                        "," + myPV.ScreenId +
                        "," + myPV.ControlId +
                        "," + myPV.MethodName +
                        "," + myPV.ActionType +
                        "," + this.perfRec.ExecTime +
                        "," + this.perfRec.CpuTime +
                        "," + sysErrorMessageID +
                        "," + sysErrorMessage + "\r\n";
                    // OriginalStackTrace（ログ出力）の品質向上
                    if (this.OriginalStackTrace == "")
                    {
                        strLogMessage += ex.StackTrace;
                    }
                    else
                    {
                        strLogMessage += this.OriginalStackTrace;
                    }

                    // Log4Netへログ出力
                    LogIF.ErrorLog("ACCESS", strLogMessage);

                    // 振替てスロー
                    throw new BusinessSystemException(sysErrorMessageID, sysErrorMessage);
                }
                else
                {
                    // そのまま

                    // ------------
                    // メッセージ部
                    // ------------
                    // ユーザ名, IPアドレス, レイヤ, 
                    // 画面名, コントロール名, メソッド名, 処理名
                    // 処理時間（実行時間）, 処理時間（CPU時間）
                    // エラーメッセージID, エラーメッセージ等
                    // ------------
                    string strLogMessage =
                        "," + myPV.User.UserName +
                        "," + myPV.User.IPAddress +
                        "," + "<<-----" +
                        "," + myPV.ScreenId +
                        "," + myPV.ControlId +
                        "," + myPV.MethodName +
                        "," + myPV.ActionType +
                        "," + this.perfRec.ExecTime +
                        "," + this.perfRec.CpuTime +
                        "," + "other Exception" +
                        "," + ex.Message + "\r\n";
                    // OriginalStackTrace（ログ出力）の品質向上
                    if (this.OriginalStackTrace == "")
                    {
                        strLogMessage += ex.StackTrace;
                    }
                    else
                    {
                        strLogMessage += this.OriginalStackTrace;
                    }

                    // Log4Netへログ出力
                    LogIF.ErrorLog("ACCESS", strLogMessage);

                    // リスロー
                    throw ex;
                }

                #endregion
            }
        }

        #endregion

        #endregion
    }
}

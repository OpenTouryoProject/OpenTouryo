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
//* クラス名        ：MyBaseControllerWin
//* クラス日本語名  ：画面コード親クラス２（Windowアプリケーション）（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//*  2012/06/14  西野 大介         コントロール検索の再帰処理性能の集約＆効率化。
//*  2012/06/18  西野 大介         OriginalStackTrace（ログ出力）の品質向上
//*  2012/09/19  西野 大介         UOC_CMNAfterFormInitの追加
//*  2013/03/05  西野 大介         UOC_CMNAfterFormInit、UOC_CMNAfterFormEndの呼出処理を追加
//*  2014/03/03  西野 大介         ユーザ コントロールのインスタンスの区別。
//*  2017/02/28  西野 大介         ExceptionDispatchInfoを取り入れ、OriginalStackTraceを削除
//*  2017/02/28  西野 大介         エラーログの見直し（その他の例外の場合、ex.ToString()を出力）
//*  2017/09/12  西野 大介         UserControlの動的配置対応のため、MyCreatePrefixAndEvtHndHtを新設。
//*  2019/05/07  西野 大介         ShowDialogによるEventHandler二重登録問題への対応
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Touryo.Infrastructure.Business.RichClient.Util;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.RichClient.Presentation;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Reflection;

namespace Touryo.Infrastructure.Business.RichClient.Presentation
{
    /// <summary>画面コード親クラス２（Windowアプリケーション）</summary>
    /// <remarks>（オーバーライドして）自由に利用できる。</remarks>
    public class MyBaseControllerWin : BaseControllerWin
    {
        // ↑abstractだとVSデザイナが機能しないので外した。

        /// <summary>ユーザ情報</summary>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected static MyUserInfo UserInfo = new MyUserInfo("－", Environment.MachineName);

        /// <summary>ログ出力の可否フラグ</summary>
        /// <remarks>
        /// 自動デプロイ環境ではログ出力が出来ない
        /// ことがあるので、その場合に利用すること。
        /// </remarks>
        protected static bool CanOutPutLog = true;

        /// <summary>性能測定</summary>
        private PerformanceRecorder perfRec;

        #region 全画面共通の処理

        #region Ｐ層イベント追加
        //（不要であれば削除すること）

        // Ｐ層フレームワークのイベント処理機能へ
        // コントロール イベントを追加するコード

        #region コントロールの検索、取得、イベントハンドラの設定処理

        /// <summary>イベント追加処理</summary>
        private void addControlEvent()
        {
            if (!this.IsInitializedEvent)
            {
                #region Formイベント

                // Formイベント
                this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_CMNEventHandler);

                // FormのKeyイベント
                this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDownEventHandler);
                this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_CMNEventHandler);

                this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form_KeyPressEventHandler);
                this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form_CMNEventHandler);

                this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form_KeyUpEventHandler);
                this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form_CMNEventHandler);

                #endregion

                #region Controlイベント

                // コントロール検索＆イベントハンドラ設定
                RcMyCmnFunction.GetCtrlAndSetClickEventHandler2(this, this.MyCreatePrefixAndEvtHndHt(), this.ControlHt);

                #endregion
            }
        }

        /// <summary>
        /// コントロールのプレフィックスと
        /// イベント ハンドラのディクショナリを生成
        /// </summary>
        /// <returns>
        /// プレフィックスと
        /// イベント ハンドラのディクショナリ
        /// </returns>
        protected Dictionary<string, object> MyCreatePrefixAndEvtHndHt()
        {
            // プレフィックス
            string prefix = "";
            // プレフィックスとイベント ハンドラのディクショナリを生成
            Dictionary<string, object> prefixAndEvtHndHt = new Dictionary<string, object>();

            // CHECK BOX
            prefix = GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX);
            if (!string.IsNullOrEmpty(prefix))
            {
                prefixAndEvtHndHt.Add(prefix, new System.EventHandler(this.Check_CheckedChanged));
            }

            return prefixAndEvtHndHt;
        }

        #endregion

        #region 集約イベント ハンドラ

        ///// <summary>
        ///// CheckBoxのCheckedChangedイベントに対応した集約イベント ハンドラ
        ///// </summary>
        //protected void Check_CheckedChanged(object sender, System.EventArgs e)
        //{
        //    // イベント ハンドラの共通引数の作成
        //    RcFxEventArgs rcFxEventArgs = new RcFxEventArgs(
        //        ((System.Web.UI.Control)(sender)).ID,
        //        0, 0, "",
        //        this.GetMethodName(((System.Web.UI.Control)(sender)).ID, 
        //            MyLiteral.UOC_METHOD_FOOTER_CHECKED_CHANGED));

        //    // クリック イベント処理の共通メソッド
        //    this.CMN_Event_Handler(rcFxEventArgs);
        //}

        /// <summary>
        /// Item系のClickイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void Item_Click(object sender, EventArgs e)
        {
            string name = sender.ToString();

            // イベント ハンドラの共通引数の作成
            RcFxEventArgs rcFxEventArgs
                = new RcFxEventArgs(name,
                    this.GetMethodName(((Control)(sender)),
                    FxLiteral.UOC_METHOD_FOOTER_CLICK), sender, e);

            // イベント処理の共通メソッド
            this.CMN_Event_Handler(rcFxEventArgs);
        }

        #endregion

        #region Form系追加イベント

        #region イベントの識別

        /// <summary>イベントを識別するイベントID</summary>
        private string _eventID = "";

        /// <summary>イベントを識別する（KeyDown）</summary>
        private void Form_KeyDownEventHandler(object sender, EventArgs e)
        {
            this._eventID = "KeyDown";
        }

        /// <summary>イベントを識別する（KeyPress）</summary>
        private void Form_KeyPressEventHandler(object sender, EventArgs e)
        {
            this._eventID = "KeyPress";
        }

        /// <summary>イベントを識別する（KeyUp）</summary>
        private void Form_KeyUpEventHandler(object sender, EventArgs e)
        {
            this._eventID = "KeyUp";
        }

        #endregion

        #region 共通イベント ハンドラ

        /// <summary>キーイベント</summary>
        private void Form_CMNEventHandler(object sender, EventArgs e)
        {
            // メソッド名
            string methodName = "UOC_Form_";
            // イベント名
            string eventName = ""; // ((Control)sender).Name + "_";

            // イベントを識別

            if (e is KeyEventArgs)
            {
                string temp = "";
                KeyEventArgs kea = (KeyEventArgs)e;

                if (this._eventID == "KeyDown")
                {
                    // KeyDownイベント
                    if (kea.KeyCode == Keys.Enter)
                    {
                        temp = "Enter";
                    }
                    else if (kea.KeyCode == Keys.F1)
                    {
                        temp = "F1";
                    }
                    else if (kea.KeyCode == Keys.F2)
                    {
                        temp = "F2";
                    }
                    else if (kea.KeyCode == Keys.F3)
                    {
                        temp = "F3";
                    }
                    else if (kea.KeyCode == Keys.F4)
                    {
                        if (kea.Alt)
                        {
                            temp = "AltAndF4";
                        }
                        else
                        {
                            temp = "F4";
                        }
                    }
                    else if (kea.KeyCode == Keys.F5)
                    {
                        temp = "F5";
                    }
                    else if (kea.KeyCode == Keys.F6)
                    {
                        temp = "F6";
                    }
                    else if (kea.KeyCode == Keys.F7)
                    {
                        temp = "F7";
                    }
                    else if (kea.KeyCode == Keys.F8)
                    {
                        temp = "F8";
                    }
                    else if (kea.KeyCode == Keys.F9)
                    {
                        temp = "F9";
                    }
                    else if (kea.KeyCode == Keys.F10)
                    {
                        temp = "F10";
                    }
                    else if (kea.KeyCode == Keys.F11)
                    {
                        temp = "F11";
                    }
                    else if (kea.KeyCode == Keys.F12)
                    {
                        temp = "F12";
                    }
                }
                else if (this._eventID == "KeyPress")
                { }
                else if (this._eventID == "KeyUp")
                { }

                // イベント名を指定
                eventName += temp + "_" + this._eventID;
                // メソッド名を指定
                methodName += temp + "_" + this._eventID;
            }
            else if (e is FormClosingEventArgs)
            {
                // FormClosingイベント

                // イベント名を指定
                eventName += "Closing";
                // メソッド名を指定
                methodName += "Closing";
            }
            //else if (e is xxx) { }

            // イベント実行
            if (Latebind.CheckTypeOfMethodByName(this, methodName))
            {
                // イベント引数の作成
                RcFxEventArgs rcFxEventArgs =
                    new RcFxEventArgs(eventName, methodName, sender, e);

                try
                {
                    // 開始処理を実行する。
                    this.UOC_PreAction(rcFxEventArgs);

                    // イベント処理を実行する。
                    Latebind.InvokeMethod_NoErr(this, methodName, new object[] { rcFxEventArgs });

                    // 終了処理を実行する。
                    this.UOC_AfterAction(rcFxEventArgs);
                }
                catch (BusinessApplicationException baEx)
                {
                    // アプリケーション例外発生時の処理を実行する。
                    this.UOC_ABEND(baEx, rcFxEventArgs);

                    // アプリケーション例外はリスローしない。
                }
                catch (BusinessSystemException bsEx)
                {
                    // システム例外発生時の処理を実行する。
                    this.UOC_ABEND(bsEx, rcFxEventArgs);

                    // システム例外はリスローする。
                    throw;
                }
                catch (Exception ex)
                {
                    // 一般的な例外発生時の処理を実行する。
                    this.UOC_ABEND(ex, rcFxEventArgs);

                    // 一般的な例外はリスローする。
                    throw;
                }
                finally
                {
                    // Finally節のUOCメソッド 
                    this.UOC_Finally(rcFxEventArgs);
                }
            }
        }

        #endregion

        #endregion

        #endregion

        #region 共通処理

        #region フォーム ロードの共通処理

        /// <summary>フォーム ロードのUOCメソッド（共通）</summary>
        /// <remarks>
        /// 実装必須
        /// 画面コード親クラス１から利用される派生の末端
        /// </remarks>
        protected override void UOC_CMNFormInit()
        {
            // フォーム ロード時に実行する処理を実装する
            // TODO:

            // イベント追加処理を呼び出す
            this.addControlEvent();

            string eventName = FxLiteral.EVENT_FORM_LOAD;

            // フォーム初期化共通処理

            // 権限チェック ------------------------------------------------
            // -------------------------------------------------------------

            // 閉塞チェック ------------------------------------------------
            // -------------------------------------------------------------

            // タイトル設定 ------------------------------------------------
            // this.ContentPageFileNoExプロパティとタイトルを関係付けると良い
            this.Text =
                GetConfigParameter.GetConfigValue("Title") + "（" + this.Text + "）";
            // -------------------------------------------------------------

            // ACCESSログ出力 ----------------------------------------------

            if (MyBaseControllerWin.CanOutPutLog)
            {
                // ------------
                // メッセージ部
                // ------------
                // ユーザ名, レイヤ, 画面名, コントロール名
                // ------------
                string strLogMessage =
                    "," + UserInfo.UserName +
                    "," + "－" +
                    "," + this.Name +
                    "," + eventName;

                // Log4Netへログ出力
                LogIF.InfoLog("ACCESS", strLogMessage);
            }

            // -------------------------------------------------------------
        }

        /// <summary>
        /// フォーム ロードのUOCメソッド（個別）
        /// </summary>
        /// <remarks>
        /// サブクラスで利用するのでここでは処理を実装しない。
        /// </remarks>
        protected override void UOC_FormInit() { }

        /// <summary>フォーム ロードのUOCメソッド（共通）</summary>
        /// <remarks>
        /// 実装必須
        /// 画面コード親クラス１から利用される派生の末端
        /// </remarks>
        protected override void UOC_CMNAfterFormInit()
        {
            // フォーム ロード時に実行する処理を実装する
            // TODO:
        }

        #endregion

        #region フォーム クローズドの共通処理

        /// <summary>
        /// フォーム クローズドのUOCメソッド（個別）
        /// </summary>
        /// <remarks>
        /// サブクラスで利用するのでここでは処理を実装しない。
        /// </remarks>
        protected override void UOC_FormEnd() { }

        /// <summary>フォーム クローズドのUOCメソッド（共通）</summary>
        /// <remarks>
        /// 実装必須
        /// 画面コード親クラス１から利用される派生の末端
        /// </remarks>
        protected override void UOC_CMNFormEnd()
        {
            // フォーム クローズドに実行する処理を実装する
            // TODO:

            string eventName = FxLiteral.EVENT_FORM_CLOSED;

            // ACCESSログ出力 ----------------------------------------------

            if (MyBaseControllerWin.CanOutPutLog)
            {
                // ------------
                // メッセージ部
                // ------------
                // ユーザ名, レイヤ, 画面名, コントロール名
                // ------------
                string strLogMessage =
                    "," + UserInfo.UserName +
                    "," + "－" +
                    "," + this.Name +
                    "," + eventName;

                // Log4Netへログ出力
                LogIF.InfoLog("ACCESS", strLogMessage);
            }

            // -------------------------------------------------------------
        }

        /// <summary>フォーム クローズドのUOCメソッド（共通）</summary>
        /// <remarks>
        /// 実装必須
        /// 画面コード親クラス１から利用される派生の末端
        /// </remarks>
        protected override void UOC_CMNAfterFormEnd()
        {
            // フォーム ロード時に実行する処理を実装する
            // TODO:
        }

        #endregion

        #region Ｐ層フレームワークの共通処理

        #region 開始 終了処理のUOCメソッド

        /// <summary>フレームワークの対象コントロールイベントの開始処理を実装</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_PreAction(RcFxEventArgs rcFxEventArgs)
        {
            // フレームワークの対象コントロールイベントの開始処理を実装する
            // TODO:

            // 権限チェック ------------------------------------------------
            // -------------------------------------------------------------

            // 閉塞チェック ------------------------------------------------
            // -------------------------------------------------------------

            // ACCESSログ出力 ----------------------------------------------

            if (MyBaseControllerWin.CanOutPutLog)
            {
                // ------------
                // メッセージ部
                // ------------
                // ユーザ名, レイヤ, 画面名, コントロール名,
                // 処理時間（実行時間）, 処理時間（CPU時間）
                // エラーメッセージID, エラーメッセージ等
                // ------------
                string strLogMessage =
                    "," + UserInfo.UserName +
                    "," + "----->" +
                    "," + this.Name +
                    "," + rcFxEventArgs.ControlName;

                // Log4Netへログ出力
                LogIF.InfoLog("ACCESS", strLogMessage);
            }

            // -------------------------------------------------------------

            // 性能測定開始
            this.perfRec = new PerformanceRecorder();
            this.perfRec.StartsPerformanceRecord();
        }

        /// <summary>フレームワークの対象コントロールイベントの終了処理を実装</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_AfterAction(RcFxEventArgs rcFxEventArgs)
        {
            // フレームワークの対象コントロールイベントの終了処理を実装する
            // TODO:

            // 性能測定終了
            this.perfRec.EndsPerformanceRecord();

            // ACCESSログ出力 ----------------------------------------------

            if (MyBaseControllerWin.CanOutPutLog)
            {
                // ------------
                // メッセージ部
                // ------------
                // ユーザ名, レイヤ, 画面名, コントロール名,
                // 処理時間（実行時間）, 処理時間（CPU時間）
                // エラーメッセージID, エラーメッセージ等
                // ------------
                string strLogMessage =
                    "," + UserInfo.UserName +
                    "," + "<-----" +
                    "," + this.Name +
                    "," + rcFxEventArgs.ControlName +
                    "," + perfRec.ExecTime +
                    "," + perfRec.CpuTime;

                // Log4Netへログ出力
                LogIF.InfoLog("ACCESS", strLogMessage);
            }

            // -------------------------------------------------------------
        }

        /// <summary>Finally節の処理を実装</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_Finally(RcFxEventArgs rcFxEventArgs)
        {
            //// Log4Netへログ出力
            //LogIF.InfoLog("ACCESS", "UOC_Finally:" + rcFxEventArgs.ButtonID);

            //// 非同期呼び出しと併用不可能（Invokeも破棄するため）。
            //// 以下のメッセージ（一定？？確認が必要）を追加でDispatchするとControl.Invoke可能。
            //int[] fm, dm;
            //PeekMessage.RemoveMessage(new int[] { 0xC25D, 0xC27D }, out fm, out dm);

            //// フィルタされたメッセージ
            //System.Diagnostics.Debug.WriteLine("fm:");
            //foreach (int i in fm)
            //{
            //    System.Diagnostics.Debug.WriteLine(i);
            //}

            //// ディスパッチされたメッセージ
            //System.Diagnostics.Debug.WriteLine("dm:");
            //foreach (int i in dm)
            //{
            //    System.Diagnostics.Debug.WriteLine(i);
            //}
        }

        #endregion

        #region 例外処理のUOCメソッド

        /// <summary>業務例外発生時の処理を実装</summary>
        /// <param name="baEx">BusinessApplicationException</param>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_ABEND(BusinessApplicationException baEx, RcFxEventArgs rcFxEventArgs)
        {
            // 業務例外発生時の処理を実装
            // TODO:

            // ここに、メッセージの組み立てロジックを実装する。

            // メッセージ編集処理 ------------------------------------------

            // -------------------------------------------------------------

            // メッセージ表示処理 ------------------------------------------

            // -------------------------------------------------------------

            // 性能測定終了

            // イベント処理開始前にエラーが発生した場合は、
            // this.perfRecがnullの場合があるので、null対策コードを挿入する。
            if (this.perfRec == null)
            {
                // nullの場合、新しいインスタンスを生成し、性能測定開始。
                this.perfRec = new PerformanceRecorder();
                perfRec.StartsPerformanceRecord();
            }

            this.perfRec.EndsPerformanceRecord();

            // ACCESSログ出力-----------------------------------------------

            if (MyBaseControllerWin.CanOutPutLog)
            {
                // ------------
                // メッセージ部
                // ------------
                // ユーザ名, レイヤ, 画面名, コントロール名,
                // 処理時間（実行時間）, 処理時間（CPU時間）
                // エラーメッセージID, エラーメッセージ等
                // ------------
                string strLogMessage =
                    "," + UserInfo.UserName +
                    "," + "<-----" +
                    "," + this.Name +
                    "," + rcFxEventArgs.ControlName +
                    "," + this.perfRec.ExecTime +
                    "," + this.perfRec.CpuTime +
                    "," + baEx.messageID +
                    "," + baEx.Message; // baEx

                // Log4Netへログ出力
                LogIF.WarnLog("ACCESS", strLogMessage);
            }

            // -------------------------------------------------------------            
        }

        /// <summary>システム例外発生時の処理を実装</summary>
        /// <param name="bsEx">BusinessSystemException</param>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_ABEND(BusinessSystemException bsEx, RcFxEventArgs rcFxEventArgs)
        {
            // システム例外発生時の処理を実装
            // TODO:

            // 性能測定終了

            // イベント処理開始前にエラーが発生した場合は、
            // this.perfRecがnullの場合があるので、null対策コードを挿入する。
            if (this.perfRec == null)
            {
                // nullの場合、新しいインスタンスを生成し、性能測定開始。
                this.perfRec = new PerformanceRecorder();
                perfRec.StartsPerformanceRecord();
            }

            this.perfRec.EndsPerformanceRecord();

            // ACCESSログ出力-----------------------------------------------

            if (MyBaseControllerWin.CanOutPutLog)
            {
                // ------------
                // メッセージ部
                // ------------
                // ユーザ名, レイヤ, 画面名, コントロール名,
                // 処理時間（実行時間）, 処理時間（CPU時間）
                // エラーメッセージID, エラーメッセージ等
                // ------------
                string strLogMessage =
                    "," + UserInfo.UserName +
                    "," + "<-----" +
                    "," + this.Name +
                    "," + rcFxEventArgs.ControlName +
                    "," + this.perfRec.ExecTime +
                    "," + this.perfRec.CpuTime +
                    "," + bsEx.messageID +
                    "," + bsEx.Message +
                    "\r\n" + bsEx.StackTrace; // bsEx

                // Log4Netへログ出力
                LogIF.ErrorLog("ACCESS", strLogMessage);
            }

            // -------------------------------------------------------------
        }

        /// <summary>一般的な例外発生時の処理を実装</summary>
        /// <param name="ex">例外オブジェクト</param>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_ABEND(Exception ex, RcFxEventArgs rcFxEventArgs)
        {
            // 一般的な例外発生時の処理を実装
            // TODO:

            // 性能測定終了

            // イベント処理開始前にエラーが発生した場合は、
            // this.perfRecがnullの場合があるので、null対策コードを挿入する。
            if (this.perfRec == null)
            {
                // nullの場合、新しいインスタンスを生成し、性能測定開始。
                this.perfRec = new PerformanceRecorder();
                perfRec.StartsPerformanceRecord();
            }

            this.perfRec.EndsPerformanceRecord();

            // ACCESSログ出力-----------------------------------------------

            if (MyBaseControllerWin.CanOutPutLog)
            {
                // ------------
                // メッセージ部
                // ------------
                // ユーザ名, レイヤ, 画面名, コントロール名,
                // 処理時間（実行時間）, 処理時間（CPU時間）
                // エラーメッセージID, エラーメッセージ等
                // ------------
                string strLogMessage =
                    "," + UserInfo.UserName +
                    "," + "<-----" +
                    "," + this.Name +
                    "," + rcFxEventArgs.ControlName +
                    "," + this.perfRec.ExecTime +
                    "," + this.perfRec.CpuTime +
                    "," + "other Exception" +
                    "," + ex.Message +
                    "\r\n" + ex.ToString(); // ex

                // Log4Netへログ出力
                LogIF.ErrorLog("ACCESS", strLogMessage);
            }
            // -------------------------------------------------------------
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}

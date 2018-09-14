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
//* クラス名        ：MyBaseController
//* クラス日本語名  ：画面コード親クラス２（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//*  2009/04/21  西野 大介         FrameworkExceptionの追加に伴い、実装変更
//*  2009/06/02  西野 大介         sln - IR版からの修正
//*                                ・#5  ： コントロール数取得処理（デフォルト値不正）
//*                                ・#19 ： InnerException対策
//*                                ・#21 ： 不要なコードブロックの削除
//*                                ・#z  ： Finally節のUOCメソッドを追加した。
//*  2009/07/21  西野 大介         コントロール取得処理の仕様変更
//*  2009/07/31  西野 大介         エラー時に、セッションを開放しないで、
//*                                業務を続行可能にする処理を追加（不正操作エラー）
//*  2009/08/10  西野 大介         他の修正により、urlの引数がnullとなり得るので、修正。
//*  2009/09/01  西野 大介         サブシステム情報クラスの実装を追加した。
//*  2009/09/25  西野 大介         セッション ステートレス対応。
//*  2010/09/24  西野 大介         共通引数クラス内にユーザ情報を格納したので
//*  2010/10/21  西野 大介         幾つかのイベント処理の正式対応（ベースクラス２→１へ）
//*  2010/11/21  西野 大介         集約イベント ハンドラをprotectedに変更（動的追加を考慮）
//*  2011/01/14  西野 大介         エラー時に、セッションを開放しないで、
//*                                業務を続行可能にする処理を追加（画面遷移制御チェック エラー）
//*  2012/04/05  西野 大介         \n → \r\n 化
//*  2012/06/14  西野 大介         コントロール検索の再帰処理性能の集約＆効率化。
//*  2012/06/18  西野 大介         OriginalStackTrace（ログ出力）の品質向上
//*  2013/01/18  西野 大介         public static TransferErrorScreen2追加（他から呼出可能に）
//*  2013/01/18  西野 大介         public static GetUserInfo2追加（他から呼出可能に）
//*  2016/01/13  Sandeep           Resolved the URL issue of error screen transition path
//*  2016/01/18  Sandeep           Modified default relative path of the sample application screens
//*  2017/02/14  西野 大介         キャッシュ無効化処理にスイッチを追加した。
//*  2017/02/28  西野 大介         ExceptionDispatchInfoを取り入れ、OriginalStackTraceを削除
//*  2017/02/28  西野 大介         TransferErrorScreen2のErrorMessage生成処理の見直し。
//*  2017/02/28  西野 大介         エラーログの見直し（その他の例外の場合、ex.ToString()を出力）
//*  2018/07/19  西野 大介         復元後のユーザー情報をSessionに設定するコードを追加
//**********************************************************************************

using System;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Business.Presentation
{
    /// <summary>画面コード親クラス２</summary>
    /// <remarks>（オーバーライドして）自由に利用できる。</remarks>
    public abstract class MyBaseController : BaseController
    {
        /// <summary>ユーザ情報</summary>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected MyUserInfo UserInfo;

        // 2009/09/01-start
        /// <summary>サブシステム情報</summary>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected MySubsysInfo SubsysInfo;
        // 2009/09/01-end

        /// <summary>性能測定</summary>
        private PerformanceRecorder perfRec;

        #region 全画面共通の処理

        #region Ｐ層イベント追加（不要であれば削除すること）
        // Ｐ層フレームワークのイベント処理機能へ
        // コントロール イベントを追加するコード

        #region コントロールの検索、取得、イベントハンドラの設定処理

        /// <summary>イベント追加処理</summary>
        private void addControlEvent()
        {
            // 2009/07/21-start

            #region コントロール取得処理

            #region 旧処理
            //// CHECK BOX
            //MyCmnFunction.GetCtrlAndSetClickEventHandler(
            //    this, GetConfigParameter.GetConfigValue(MyLiteral.PREFIX_OF_CHECK_BOX),
            //    new System.EventHandler(this.Check_CheckedChanged), this.ControlHt);
            #endregion

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

            // コントロール検索＆イベントハンドラ設定
            MyCmnFunction.GetCtrlAndSetClickEventHandler2(this, prefixAndEvtHndHt, this.ControlHt);

            #endregion

            // 2009/07/21-end
        }

        #endregion

        #region 集約イベント ハンドラ

        ///// <summary>
        ///// CheckBoxのCheckedChangedイベントに対応した集約イベント ハンドラ
        ///// </summary>
        //protected void Check_CheckedChanged(object sender, System.EventArgs e)
        //{
        //    // イベント ハンドラの共通引数の作成
        //    FxEventArgs fxEventArgs = new FxEventArgs(
        //        ((System.Web.UI.Control)(sender)).ID,
        //        0, 0, "",
        //        this.GetMethodName(((System.Web.UI.Control)(sender)).ID, 
        //            MyLiteral.UOC_METHOD_FOOTER_CHECKED_CHANGED));

        //    // クリック イベント処理の共通メソッド
        //    this.CMN_Event_Handler(fxEventArgs);
        //}

        #endregion

        #endregion

        #region ページロードの共通処理

        /// <summary>ページロードのUOCメソッド（共通：初回ロード）</summary>
        /// <remarks>
        /// 実装必須
        /// 画面コード親クラス１から利用される派生の末端
        /// </remarks>
        protected override void UOC_CMNFormInit()
        {
            // フォーム初期化（初回ロード）時に実行する処理を実装する
            // TODO:

            // フォーム初期化共通処理
            this.CMN_FormInit("init");
        }

        /// <summary>ページロードのUOCメソッド（共通：ポストバック）</summary>
        /// <remarks>
        /// 実装必須
        /// 画面コード親クラス１から利用される派生の末端
        /// </remarks>
        protected override void UOC_CMNFormInit_PostBack()
        {
            // フォーム初期化（ポストバック）時に実行する処理を実装する
            // TODO:

            // イベントの判別
            if (this.IsClientCallback)
            {
                // フォーム初期化共通処理
                this.CMN_FormInit("postback(ClientCallback)");
            }
            else if (this.AjaxExtensionStatus == FxEnum.AjaxExtStat.IsAjaxExtension)
            {
                // フォーム初期化共通処理
                this.CMN_FormInit("postback(AjaxExtension)");
            }
            else
            {
                // フォーム初期化共通処理
                this.CMN_FormInit("postback");
            }
        }

        /// <summary>フォーム初期化共通処理</summary>
        /// <param name="eventName">イベント名（init or postback）</param>
        private void CMN_FormInit(string eventName)
        {
            // イベント追加処理を呼び出す
            this.addControlEvent();

            // カスタム認証処理 --------------------------------------------
            // ・・・
            // -------------------------------------------------------------

            // 2009/09/01-start
            // 認証ユーザ情報をメンバにロードする --------------------------
            this.GetUserInfo();
            // -------------------------------------------------------------

            // サブシステム情報をメンバにロードする ------------------------
            this.GetSubsysInfo();
            // -------------------------------------------------------------
            // 2009/09/01-end

            // 権限チェック ------------------------------------------------
            // ・・・
            // -------------------------------------------------------------

            // 閉塞チェック ------------------------------------------------
            // ・・・
            // -------------------------------------------------------------

            // キャッシュ制御処理 ------------------------------------------
            this.CacheControlWithSwitch();
            // -------------------------------------------------------------

            // ポストバック後の位置復元 ------------------------------------
            Page.MaintainScrollPositionOnPostBack = true;
            // -------------------------------------------------------------

            // タイトル設定 ------------------------------------------------
            // this.ContentPageFileNoExプロパティとタイトルを関係付けると良い

            this.Page.Title =
                GetConfigParameter.GetConfigValue("BrowserTitle")
                + "（" + this.ContentPageFileNoEx + "）";
            // -------------------------------------------------------------

            // ACCESSログ出力 ----------------------------------------------

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                "," + UserInfo.UserName +
                "," + Request.UserHostAddress +
                "," + eventName +
                "," + this.ContentPageFileNoEx;

            // Log4Netへログ出力
            LogIF.InfoLog("ACCESS", strLogMessage);

            // -------------------------------------------------------------
        }

        // 2009/09/01 & 2009/09/25-start

        /// <summary>ユーザ情報を取得する</summary>
        private void GetUserInfo()
        {
            // メンバへセット
            this.UserInfo = MyBaseController.GetUserInfo2();
        }

        /// <summary>ユーザ情報を取得する</summary>
        /// <remarks>他から呼び出し可能に変更（static）</remarks>
        public static MyUserInfo GetUserInfo2()
        {
            MyUserInfo userInfo = null;

            // セッションステートレス対応
            if (HttpContext.Current.Session == null)
            {
                // SessionがOFFの場合
            }
            else
            {
                // 取得を試みる。
                userInfo = (MyUserInfo)UserInfoHandle.GetUserInformation();

                // nullチェック
                if (userInfo == null)
                {
                    // nullの場合、仮の値を生成 / 設定する。
                    string userName = System.Threading.Thread.CurrentPrincipal.Identity.Name;

                    if (userName == null || userName == "")
                    {
                        // 未認証状態
                        userInfo = new MyUserInfo("未認証", HttpContext.Current.Request.UserHostAddress);
                    }
                    else
                    {
                        // 認証状態
                        userInfo = new MyUserInfo(userName, HttpContext.Current.Request.UserHostAddress);

                        // 必要に応じて認証チケットのユーザ名からユーザ情報を復元する。
                        // ★ 必要であれば、他の業務共通引継ぎ情報などをロードする。
                        // ・・・

                        // 復元したユーザ情報をセット
                        UserInfoHandle.SetUserInformation(userInfo);
                    }
                }
            }

            // 値を戻す。
            return userInfo;
        }

        /// <summary>サブシステム情報を取得する</summary>
        /// <remarks>nullの場合は、新規生成してSetする。</remarks>
        private void GetSubsysInfo()
        {
            SubsysInfo subsysInfo;

            // セッションステートレス対応
            if (HttpContext.Current.Session == null)
            {
                // SessionがOFFの場合
            }
            else
            {
                // 取得を試みる。
                subsysInfo = SubsysInfoHandle.GetSubsysInformation();

                // nullチェック
                if (subsysInfo == null)
                {
                    // nullの場合、新規生成してSetする。
                    this.SubsysInfo = new MySubsysInfo();
                    SubsysInfoHandle.SetSubsysInformation(this.SubsysInfo);
                }
                else
                {
                    // nullで無い場合、取得した値を設定する。
                    this.SubsysInfo = (MySubsysInfo)subsysInfo;
                }
            }
        }

        // 2009/09/01 & 2009/09/25-end

        /// <summary>キャッシュ制御処理（スイッチ付き）</summary>
        private void CacheControlWithSwitch()
        {
            // システムで固定に出来る場合は、ここでキャッシュ無効化する。
            // また、ユーザープログラムのファイル・ダウンロード処理などで
            // フレームワークの設定したキャッシュ制御を変更したい場合は、Response.Clearを実行して再設定する。

            // 画面遷移方法の定義を取得
            string noCache = GetConfigParameter.GetConfigValue(MyLiteral.CACHE_CONTROL);

            // デフォルト値対策：設定なし（null）の場合の扱いを決定
            if (noCache == null)
            {
                // OFF扱い
                noCache = FxLiteral.OFF;
            }

            if (noCache.ToUpper() == FxLiteral.ON)
            {
                // ON

                // http - How to control web page caching, across all browsers? - Stack Overflow
                // http://stackoverflow.com/questions/49547/how-to-control-web-page-caching-across-all-browsers

                // Using ASP.NET:
                Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
                Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
                Response.AppendHeader("Expires", "0"); // Proxies.

            }
            else if (noCache.ToUpper() == FxLiteral.OFF)
            {
                // OFF
            }
            else
            {
                // パラメータ・エラー（書式不正）
                throw new FrameworkException(
                    FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[0],
                    String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[1],
                        MyLiteral.CACHE_CONTROL));
            }
        }

        #endregion

        #region Ｐ層フレームワークの共通処理

        #region 開始 終了処理のUOCメソッド

        /// <summary>フレームワークの対象コントロールイベントの開始処理を実装</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_PreAction(FxEventArgs fxEventArgs)
        {
            // フレームワークの対象コントロールイベントの開始処理を実装する
            // TODO:

            // 認証ユーザ情報を取得する ------------------------------------
            this.GetUserInfo();

            // -------------------------------------------------------------

            // 権限チェック ------------------------------------------------
            // -------------------------------------------------------------

            // 閉塞チェック ------------------------------------------------
            // -------------------------------------------------------------

            // ACCESSログ出力 ----------------------------------------------

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                "," + UserInfo.UserName +
                "," + Request.UserHostAddress +
                "," + "----->" +
                "," + this.ContentPageFileNoEx +
                "," + fxEventArgs.ButtonID;

            // Log4Netへログ出力
            LogIF.InfoLog("ACCESS", strLogMessage);

            // -------------------------------------------------------------

            // 性能測定開始
            this.perfRec = new PerformanceRecorder();
            this.perfRec.StartsPerformanceRecord();
        }

        /// <summary>フレームワークの対象コントロールイベントの終了処理を実装</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_AfterAction(FxEventArgs fxEventArgs)
        {
            // フレームワークの対象コントロールイベントの終了処理を実装する
            // TODO:

            // 性能測定終了
            this.perfRec.EndsPerformanceRecord();

            // 認証ユーザ情報を取得する ------------------------------------
            this.GetUserInfo();

            // ACCESSログ出力 ----------------------------------------------

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                "," + UserInfo.UserName +
                "," + Request.UserHostAddress +
                "," + "<-----" +
                "," + this.ContentPageFileNoEx +
                "," + fxEventArgs.ButtonID +
                "," + "" +
                "," + perfRec.ExecTime +
                "," + perfRec.CpuTime;

            // Log4Netへログ出力
            LogIF.InfoLog("ACCESS", strLogMessage);

            // -------------------------------------------------------------
        }

        // #z-start

        /// <summary>Finally節の処理を実装</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_Finally(FxEventArgs fxEventArgs)
        {
            //// Log4Netへログ出力
            //LogIF.InfoLog("ACCESS", "UOC_Finally:" + fxEventArgs.ButtonID);
        }

        // #z-end

        #endregion

        #region 画面遷移のUOCメソッド

        /// <summary>ボタンのクリック・イベントの終了後の画面遷移処理を実装</summary>
        /// <param name="url">画面遷移する場合のURL</param>
        /// <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_Screen_Transition(string url)
        {

            if (url == null || url == "") // 2009/08/10-この行
            {
                // urlが空の場合、どこにも遷移せず、ポストバックとなる。
            }
            else
            {
                // urlが空でない場合、画面遷移する。

                if (MyBaseController.TransitionMethod == FxLiteral.OFF)
                {
                    // 画面遷移方法を取得（テストプログラム用パラメータ）
                    string screenTransitionMethod =
                        GetConfigParameter.GetConfigValue("ScreenTransitionMethod");

                    if (screenTransitionMethod == "1")
                    {
                        // フレームワーク管理下の画面遷移（Transfer）
                        this.FxTransfer(url);
                    }
                    else if (screenTransitionMethod == "2")
                    {
                        // フレームワーク管理下の画面遷移（Redirect）
                        this.FxRedirect(url);
                    }
                    else
                    {
                        // パラメータ指定ミス
                    }
                }
                else
                {
                    // 画面遷移制御部品を使用して画面遷移
                    this.ScreenTransition(url);
                }
            }
        }

        #endregion

        #region 例外処理のUOCメソッド

        /// <summary>業務例外発生時の処理を実装</summary>
        /// <param name="baEx">BusinessApplicationException</param>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_ABEND(BusinessApplicationException baEx, FxEventArgs fxEventArgs)
        {
            // 業務例外発生時の処理を実装
            // TODO:

            // ここに、メッセージの組み立てロジックを実装する。

            // メッセージ編集処理 ------------------------------------------

            string messageID = baEx.messageID;
            string messageDescription = "";

            // メッセージIDから、対応するメッセージを取得する。
            messageDescription = GetMessage.GetMessageDescription(messageID);
            if (messageDescription == "")
            {
                // メッセージが取得できなかった場合
                messageDescription = baEx.Message;
            }
            else
            {
                // メッセージが取得できた場合、
                // 必要なら、メッセージに、可変文字列を組み込む。

                // 方式は、プロジェクト毎に検討のこと。
                messageDescription = messageDescription.Replace("%1", baEx.Message);
                messageDescription = messageDescription.Replace("%2", baEx.Information);
            }

            // -------------------------------------------------------------

            // メッセージ表示処理 ------------------------------------------

            #region メッセージボックスを使用して表示する場合。

            // 「OK」メッセージダイアログの表示処理
            this.ShowOKMessageDialog(
                messageID, messageDescription,
                FxEnum.IconType.Exclamation,
                "BusinessApplicationExceptionを使用したダイアログ表示");

            #endregion

            #region マスタ ページ上のラベルに表示する場合。

            //// 結果表示するメッセージ エリア
            //Label label = (Label)this.GetMasterWebControl("Label1");
            //label.Text = "";

            //// 結果（業務続行可能なエラー）
            //label.Text = "ErrorMessageID:" + baEx.messageID + "\r\n";
            //label.Text += "ErrorMessage:" + baEx.Message + "\r\n";
            //label.Text += "ErrorInfo:" + baEx.Information + "\r\n";

            #endregion

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

            // 認証ユーザ情報を取得する ------------------------------------
            this.GetUserInfo();

            // ACCESSログ出力-----------------------------------------------

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                "," + UserInfo.UserName +
                "," + Request.UserHostAddress +
                "," + "<-----" +
                "," + this.ContentPageFileNoEx +
                "," + fxEventArgs.ButtonID +
                "," + "" +
                "," + this.perfRec.ExecTime +
                "," + this.perfRec.CpuTime +
                "," + baEx.messageID +
                "," + baEx.Message; // baEx

            // Log4Netへログ出力
            LogIF.WarnLog("ACCESS", strLogMessage);

            // -------------------------------------------------------------
        }

        /// <summary>システム例外発生時の処理を実装</summary>
        /// <param name="bsEx">BusinessSystemException</param>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_ABEND(BusinessSystemException bsEx, FxEventArgs fxEventArgs)
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

            // 認証ユーザ情報を取得する ------------------------------------
            this.GetUserInfo();

            // ACCESSログ出力-----------------------------------------------

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                "," + UserInfo.UserName +
                "," + Request.UserHostAddress +
                "," + "<-----" +
                "," + this.ContentPageFileNoEx +
                "," + fxEventArgs.ButtonID +
                "," + "" +
                "," + this.perfRec.ExecTime +
                "," + this.perfRec.CpuTime +
                "," + bsEx.messageID +
                "," + bsEx.Message + "\r\n" +
                "," + bsEx.StackTrace; // bsEX

            // Log4Netへログ出力
            LogIF.ErrorLog("ACCESS", strLogMessage);

            // -------------------------------------------------------------

            // エラー画面に画面遷移する ------------------------------------
            this.TransferErrorScreen(bsEx);
            // -------------------------------------------------------------
        }

        /// <summary>一般的な例外発生時の処理を実装</summary>
        /// <param name="ex">例外オブジェクト</param>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>画面コード親クラス１から利用される派生の末端</remarks>
        protected override void UOC_ABEND(Exception ex, FxEventArgs fxEventArgs)
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

            // 認証ユーザ情報を取得する ------------------------------------
            this.GetUserInfo();

            // ACCESSログ出力-----------------------------------------------

            // ------------
            // メッセージ部
            // ------------
            // ユーザ名, IPアドレス,
            // レイヤ, 画面名, コントロール名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // エラーメッセージID, エラーメッセージ等
            // ------------
            string strLogMessage =
                "," + UserInfo.UserName +
                "," + Request.UserHostAddress +
                "," + "<-----" +
                "," + this.ContentPageFileNoEx +
                "," + fxEventArgs.ButtonID +
                "," + "" +
                "," + this.perfRec.ExecTime +
                "," + this.perfRec.CpuTime +
                "," + "other Exception" +
                "," + ex.Message + "\r\n" + 
                ex.ToString(); // ex

            // Log4Netへログ出力
            LogIF.ErrorLog("ACCESS", strLogMessage);

            // -------------------------------------------------------------

            // エラー画面に画面遷移する ------------------------------------
            this.TransferErrorScreen(ex);
            // -------------------------------------------------------------
        }

        #endregion

        #region 例外発生時に、エラー画面に画面遷移

        /// <summary>例外発生時に、エラー画面に画面遷移</summary>
        /// <param name="ex">例外オブジェクト</param>
        /// <remarks>Global_asaxから移植</remarks>
        private void TransferErrorScreen(Exception ex)
        {
            if (this.AjaxExtensionStatus == FxEnum.AjaxExtStat.IsAjaxExtension)
            {
                // Ajax Extensionの場合は、エラー画面を戻さないこと！
                throw ex;
            }
            else if (this.IsClientCallback)
            {
                // Client Callbackの場合は、エラー画面を戻さないこと！
                throw ex;
            }
            else
            {
                MyBaseController.TransferErrorScreen2(ex);
            }
        }

        /// <summary>例外発生時に、エラー画面に遷移</summary>
        /// <param name="ex">例外オブジェクト</param>
        /// <remarks>他から呼び出し可能に変更（static）</remarks>
        public static void TransferErrorScreen2(Exception ex)
        {
            #region 例外型を判別しエラーメッセージIDを取得

            // エラーメッセージ
            string err_msg;

            // エラー情報をセッションから取得
            string err_info;

            // エラーのタイプ
            string[] arrErrType = ex.GetType().ToString().Split('.');
            string errType = arrErrType[arrErrType.Length - 1];

            // エラーメッセージＩＤ
            string errMsgId = "";

            // #21-start
            if (errType == "BusinessSystemException")
            {
                // システム例外
                BusinessSystemException bsEx = (BusinessSystemException)ex;
                errMsgId = bsEx.messageID;
            }
            else if (errType == "FrameworkException")
            {
                // フレームワーク例外
                FrameworkException fxEx = (FrameworkException)ex;
                errMsgId = fxEx.messageID;
            }
            else
            {
                // それ以外の例外
                errMsgId = "－";
            }
            // #21-end

            #endregion

            // 2009/07/31-start

            #region エラー時に、セッションを解放しないで、業務を続行可能にする処理を追加。

            // 不正操作エラー or 画面遷移制御チェック エラー
            if (errMsgId == "IllegalOperationCheckError"
                || errMsgId == "ScreenControlCheckError")
            {
                // セッションをクリアしない
                HttpContext.Current.Items.Add(FxHttpContextIndex.SESSION_ABANDON_FLAG, false);
            }
            else
            {
                // セッションをクリアする
                HttpContext.Current.Items.Add(FxHttpContextIndex.SESSION_ABANDON_FLAG, true);
            }

            #endregion

            // 2009/07/31-end

            #region エラー画面に表示するエラー情報を作成

            err_msg = Environment.NewLine +
                "Error Message ID : " + errMsgId + Environment.NewLine +
                "Error Message : " + ex.Message.ToString();

            err_info = Environment.NewLine +
                "Current Request Url : " + HttpContext.Current.Request.Url.ToString() + Environment.NewLine +
                "Exception.StackTrace : " + ex.StackTrace + Environment.NewLine +
                "Exception.ToString() : " + ex.ToString();

            // Form情報を出力するために、
            // 遷移方法をServer.Transferに変更。
            // また、情報受け渡しを、HttpContextに変更。
            HttpContext.Current.Items.Add(FxHttpContextIndex.SYSTEM_EXCEPTION_MESSAGE, err_msg);
            HttpContext.Current.Items.Add(FxHttpContextIndex.SYSTEM_EXCEPTION_INFORMATION, err_info);

            #endregion

            // エラー画面へのパスを取得 --- チェック不要（ベースクラスでチェック済み）
            string errorScreenPath = GetConfigParameter.GetConfigValue(FxLiteral.ERROR_SCREEN_PATH);

            // Resolves the path of the url with respect to the server
            BaseController.ResolveServerUrl(ref errorScreenPath);

            // エラー画面へ画面遷移
            HttpContext.Current.Server.Transfer(errorScreenPath);
        }

        #endregion

        #endregion

        #endregion

        #region マスタ ページ上のフレームワーク対象コントロール（不要な場合は削除してく下さい）

        #region sampleScreen.masterマスタ ページ上のフレームワーク対象コントロールの、共通イベントのUOCメソッド

        /// <summary>
        /// btnMButton101のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_sampleScreen_btnMButton101_Click(FxEventArgs fxEventArgs)
        {
            // ログオフ（認証チケットを削除する）
            FormsAuthentication.SignOut();

            // メッセージ表示
            Label label = (Label)this.GetMasterWebControl("Label1");
            label.Text = "ログアウトしました。";

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "~/Aspx/start/menu.aspx";
        }

        /// <summary>
        /// btnMButton102のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_sampleScreen_btnMButton102_Click(FxEventArgs fxEventArgs)
        {
            // ウィンドウの表示
            this.ShowNormalScreen("~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx");

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #region TestScreen1.masterマスタ ページ上のフレームワーク対象コントロールの、共通イベントのUOCメソッド

        #region 基本処理

        /// <summary>
        /// btnMButton1のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen1_btnMButton1_Click(FxEventArgs fxEventArgs)
        {
            // メッセージ表示
            this.ShowOKMessageDialog(
                fxEventArgs.ButtonID + "クリック イベント",
                fxEventArgs.MethodName + "の実行",
                FxEnum.IconType.Information, "テスト結果");

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// lbnMLinkButton1のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen1_lbnMLinkButton1_Click(FxEventArgs fxEventArgs)
        {
            // メッセージ表示
            this.ShowOKMessageDialog(
                fxEventArgs.ButtonID + "クリック イベント",
                fxEventArgs.MethodName + "の実行",
                FxEnum.IconType.Information, "テスト結果");

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// ibnMImageButton1のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen1_ibnMImageButton1_Click(FxEventArgs fxEventArgs)
        {
            // メッセージ表示
            this.ShowOKMessageDialog(
                fxEventArgs.ButtonID + "クリック イベント",
                fxEventArgs.MethodName + "の実行 - " +
                "x:" + fxEventArgs.X.ToString() +
                ",y:" + fxEventArgs.Y.ToString(),
                FxEnum.IconType.Information, "テスト結果");

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        /// <summary>
        /// impMImageMap1のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen1_impMImageMap1_Click(FxEventArgs fxEventArgs)
        {
            // メッセージ表示
            this.ShowOKMessageDialog(
                fxEventArgs.ButtonID + "クリック イベント",
                fxEventArgs.MethodName + "の実行 - " +
                "pbv:" + fxEventArgs.PostBackValue,
                FxEnum.IconType.Information, "テスト結果");

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #region 画面遷移処理

        /// <summary>
        /// btnMButton2のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen1_btnMButton2_Click(FxEventArgs fxEventArgs)
        {
            return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx";
        }

        /// <summary>
        /// lbnMLinkButton2のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen1_lbnMLinkButton2_Click(FxEventArgs fxEventArgs)
        {
            return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx";
        }

        /// <summary>
        /// ibnMImageButton2のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen1_ibnMImageButton2_Click(FxEventArgs fxEventArgs)
        {
            return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx";
        }

        /// <summary>
        /// impMImageMap2のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen1_impMImageMap2_Click(FxEventArgs fxEventArgs)
        {
            return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx";
        }

        #endregion

        #region コントロール取得

        /// <summary>
        /// btnMButton3のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen1_btnMButton3_Click(FxEventArgs fxEventArgs)
        {
            // コントロールを取得し
            Control temp = (Control)this.GetFxWebControl(((TextBox)this.GetMasterWebControl("TextBox1")).Text);

            if (temp == null)
            {
                // 取得できなかった

                // メッセージ表示
                this.ShowOKMessageDialog(
                    "GetFxWebControl",
                    "コントロールを取得できませんでした。",
                    FxEnum.IconType.Information, "テスト結果");
            }
            else
            {
                // 取得できた

                // 消したり出したり
                if (temp.Visible == true)
                {
                    temp.Visible = false;
                }
                else
                {
                    temp.Visible = true;
                }
            }

            return "";
        }

        /// <summary>
        /// lbnMLinkButton3のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen1_lbnMLinkButton3_Click(FxEventArgs fxEventArgs)
        {
            // コントロールを取得し
            Control temp = (Control)this.GetMasterWebControl(((TextBox)this.GetMasterWebControl("TextBox1")).Text);

            if (temp == null)
            {
                // 取得できなかった

                // メッセージ表示
                this.ShowOKMessageDialog(
                    "GetMasterWebControl",
                    "コントロールを取得できませんでした。",
                    FxEnum.IconType.Information, "テスト結果");
            }
            else
            {
                // 取得できた

                // 消したり出したり
                if (temp.Visible == true)
                {
                    temp.Visible = false;
                }
                else
                {
                    temp.Visible = true;
                }
            }

            return "";
        }

        /// <summary>
        /// ibnMImageButton3のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen1_ibnMImageButton3_Click(FxEventArgs fxEventArgs)
        {
            // コントロールを取得し
            Control temp = (Control)this.GetContentWebControl(((TextBox)this.GetMasterWebControl("TextBox1")).Text);

            if (temp == null)
            {
                // 取得できなかった

                // メッセージ表示
                this.ShowOKMessageDialog(
                    "GetContentWebControl",
                    "コントロールを取得できませんでした。",
                    FxEnum.IconType.Information, "テスト結果");
            }
            else
            {
                // 取得できた

                // 消したり出したり
                if (temp.Visible == true)
                {
                    temp.Visible = false;
                }
                else
                {
                    temp.Visible = true;
                }
            }

            return "";
        }

        #endregion

        #region ダイアログ表示

        /// <summary>
        /// btnMButton4のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen1_btnMButton4_Click(FxEventArgs fxEventArgs)
        {
            // スタイルを取得
            string style = ((TextBox)this.GetMasterWebControl("TextBox2")).Text;

            // 受け渡しデータの設定
            string msg = ((TextBox)this.GetMasterWebControl("TextBox3")).Text;

            if (((CheckBox)this.GetMasterWebControl("CheckBox1")).Checked == true)
            {
                // スタイル指定あり
                this.ShowOKMessageDialog(
                    "メッセージＩＤ", "メッセージ：" + msg,
                    FxEnum.IconType.Information, "テスト", style);
            }
            else
            {
                // スタイル指定なし
                this.ShowOKMessageDialog(
                    "メッセージＩＤ", "メッセージ：" + msg,
                    FxEnum.IconType.Information, "テスト");
            }

            return "";
        }

        /// <summary>
        /// lbnMLinkButton4のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen1_lbnMLinkButton4_Click(FxEventArgs fxEventArgs)
        {
            // スタイルを取得
            string style = ((TextBox)this.GetMasterWebControl("TextBox2")).Text;

            if (((CheckBox)this.GetMasterWebControl("CheckBox1")).Checked == true)
            {
                // スタイル指定あり
                this.ShowNormalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx", style);
            }
            else
            {
                // スタイル指定なし
                this.ShowNormalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx");
            }

            return "";
        }

        #endregion

        #endregion

        #region TestScreen2.masterマスタ ページ上のフレームワーク対象コントロールの、共通イベントのUOCメソッド

        /// <summary>
        /// btnMButton1のクリックイベント
        /// </summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        public string UOC_TestScreen2_btnMButton1_Click(FxEventArgs fxEventArgs)
        {
            // 動作確認のため、３秒待つ。
            System.Threading.Thread.Sleep(3000);

            // メッセージ表示
            this.ShowOKMessageDialog(
                fxEventArgs.ButtonID + "クリック イベント",
                fxEventArgs.MethodName + "の実行",
                FxEnum.IconType.Information, "テスト結果");

            // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
            return "";
        }

        #endregion

        #endregion
    }
}

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
//* クラス名        ：BaseController
//* クラス日本語名  ：画面コード親クラス１
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2008/10/16  西野  大介        問題点の修正
//*  2008/10/22  西野  大介        問題点の修正
//*  2008/11/28  西野  大介        問題点の修正
//*  2009/03/13  西野  大介        機能追加（画面遷移制御機能など）、修正など。
//*  2009/03/27  西野  大介        Ajax対応（不要なカバレージを実行しないように修正）。
//*  2009/04/21  西野  大介        FrameworkExceptionの追加に伴い、実装変更
//*  2009/04/28  西野  大介        デフォルト値を設けた
//*  2009/05/11  西野  大介        画面遷移制御処理の機能改修（Cmnタグの追加）
//*  2009/06/02  西野  大介        sln - IR版からの修正
//*                                ・#5  ： コントロール数取得処理（デフォルト値不正）
//*                                ・#22 ： 通過しないカバレッジを削除
//*                                ・#x  ： QueryString付きのダイアログ起動を可能にした。
//*                                ・#y  ： クライアント側からのダイアログ起動を可能にした。
//*                                ・#z  ： Finally節のUOCメソッドを追加した。
//*  2009/07/21  西野  大介        コントロール取得処理の仕様変更
//*  2009/07/21  西野  大介        マスタ ページのネストに対応
//*  2009/07/31  西野  大介        セッション情報の自動削除機能を追加
//*  2009/07/31  西野  大介        不正操作の検出機能を追加
//*  2009/08/10  西野  大介        メソッドが無くても例外をスローしないように変更
//*  2009/08/12  西野  大介        比較演算子の向きを「<」に統一した。
//*  2009/09/01  西野  大介        コントロール取得処理（イベント設定用）の性能チェック
//*                                問題なかったので、コメントアウト
//*  2009/09/07  西野  大介        コントロール取得処理（参照取得用）のバグ修正
//*  2009/09/18  西野  大介        セッション タイム アウト検出用cookieのパス属性を明示
//*  2009/09/25  西野  大介        セッション ステートレス対応。
//*  2009/10/15  西野  大介        画面遷移制御機能（遷移＆チェック）の性能チェック
//*                                問題なかったので、コメントアウト（但し、改善の余地有り）
//*  2009/11/06  西野  大介        画面遷移の性能対策をしてみたが、
//*                                （GetElementById → GetElementsByTagName ＋ ループ)
//*                                逆に遅くなるので、元に戻した。
//*                                GetElementByIdには、データ量によらず、
//*                                大体20msec程度のオーバヘッドがある模様。
//*  2010/09/20  西野  大介        業務モードレス画面表示時のターゲット指定対応
//*  2010/09/24  西野  大介        ジェネリック対応（Dictionary、List、Queue、Stack<T>）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2010/10/04  西野  大介        ボタン履歴情報記録機能のON / OFFスイッチ変更
//*  2010/10/13  西野  大介        ダイアログ表示で消費ウィンドウGUIDを消費しない仕様に変更
//*  2010/10/21  西野  大介        幾つかのイベント処理の正式対応（ベースクラス２→１へ）
//*  2010/10/21  西野  大介        RepeaterコントロールのItemCommandイベントを追加する。
//*  2010/11/21  西野  大介        集約イベント ハンドラをprotectedに変更（動的追加を考慮）
//*  2010/12/09  西野  大介        IsNoSession隠しフラグを追加（当該画面でSessionIDを返さない）
//*  2010/12/10  西野  大介        セッション消去メソッドを追加（IsNoSessionでは解放しない）
//*  2011/01/14  西野  大介        CanCheckIllegalOperationフラグを追加（当該画面の不正操作動作を変更）
//*  2011/01/18  西野  大介        GridViewコントロールのRowCommand、SelectedIndexChanged、
//*                                RowUpdating、RowDeleting、PageIndexChanging、Sortingイベントを追加する。
//*  2011/01/19  西野  大介        環境変数の組み込み処理に対応
//*  2011/02/08  西野  大介        マスタ ページ上にイベントハンドラを実装可能にする。
//*  2011/02/08  西野  大介        ユーザ コントロール上にイベントハンドラを実装可能にする。
//*  2011/02/10  西野  大介        Clickイベント以外、Repeater、GridViewの
//*                                行数をPostBackValueに含める処理を追加した。
//*  2011/02/10  西野  大介        GetFileNameNoExメソッドを追加し、処理を共通化。
//*  2011/05/18  西野  大介        画面遷移制御機能の埋め込まれたリソース対応（Azure対応）
//*  2012/04/05  西野  大介         \n → \r\n 化
//*  2012/06/14  西野  大介        コントロール検索の再帰処理性能の集約＆効率化。
//*  2012/06/18  西野  大介        OriginalStackTrace（ログ出力）の品質向上
//*  2012/06/18  西野  大介        Screenタグの中にコメントを記述可能にした。
//*  2014/03/03  西野  大介        ユーザ コントロールのインスタンスの区別。
//*  2014/08/18  Sai-San           Added Postback Value, events and event handlers for ListView events.   
//*  2014/10/03  Rituparna         Added code for Supporting ItemCommand event to ListViewControl. 
//*  2014/10/03  Rituparna         Added code SelectedIndexChanged for RadiobuttonList and CheckBoxList. 
//**********************************************************************************

// 処理に必要
using System.Diagnostics;
using System.ComponentModel;

// System
using System;
using System.Xml;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// System.Web
using System.Web;
using System.Web.Security;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

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

namespace Touryo.Infrastructure.Framework.Presentation
{
    /// <summary>画面コード親クラス１</summary>
    /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
    public abstract class BaseController : System.Web.UI.Page
    {
        #region ロック用変数

        /// <summary>Sessionレベルでロックするロック用オブジェクト</summary>
        /// <remarks>インスタンス変数</remarks>
        private object SessionLock;

        #endregion

        #region グローバル変数

        #region 画面遷移制御

        /// <summary>画面遷移制御シングルトン クラス</summary>
        /// <remarks>
        /// 初期化は起動時の１回のみであり、
        /// 読み取り専用のデータを保持する場合
        /// のみに適用するデザインパターンとする。
        /// </remarks>
        private static ScreenControl SC = new ScreenControl();

        /// <summary>画面遷移方法</summary>
        /// <remarks>画面コード親クラス２から利用する。</remarks>
        protected static string TransitionMethod
        {
            get { return BaseController.SC.TransitionMethod; }
        }

        #endregion

        #endregion

        #region インスタンス変数

        #region マスタ ページ、コンテンツ ページ、ユーザ コントロール

        /// <summary>ルートのマスタ ページを保存するワーク領域</summary>
        private MasterPage RootMasterPage = null;

        /// <summary>全てのマスタ ページを保存するワーク領域</summary>
        private Stack<MasterPage> StcMasterPage = null;

        /// <summary>全てのマスタ ページ名を保存するワーク領域</summary>
        private Stack<string> StcMasterPageFileNoEx = null;

        /// <summary>全てのユーザ コントロールを保存するワーク領域</summary>
        private List<UserControl> LstUserControl = null;

        /// <summary>ルートのマスタ ページ名を保存するワーク領域</summary>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected string RootMasterPageFileNoEx = null;

        /// <summary>コンテンツ ページ名を保存するワーク領域</summary>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected string ContentPageFileNoEx = null;

        #endregion

        /// <summary>フレームワークのイベント処理対応コントロールを保持する</summary>
        /// <remarks>画面コード親クラス２から利用する。</remarks>
        protected Dictionary<string, Control> ControlHt = new Dictionary<string, Control>();

        /// <summary>オリジナルのスタックトレース値</summary>
        /// <remarks>画面コード親クラス２から利用する。</remarks>
        protected string OriginalStackTrace = "";

        #region 画面遷移制御機能

        /// <summary>現在の画面（.aspx）の仮想パス</summary>
        private string FormerAspxVirtualPath;

        /// <summary>複数ブラウザ ウィンドウ対応（ウィンドウGUID）</summary>
        private HiddenField WindowGuid;

        // 2009/07/31-start

        /// <summary>不正操作の検出機能対応（リクエスト チケットGUID）</summary>
        private HiddenField RequestTicketGuid;

        /// <summary>リクエスト チケットGUIDを保持するキュー</summary>
        private Queue<string> RequestTicketGuid_Queue = null;

        // 2009/07/31-end

        #endregion

        #region ダイアログ表示機能

        /// <summary>開く子画面のタイプ</summary>
        private HiddenField ChildScreenType;

        /// <summary>開く子画面のURL</summary>
        private HiddenField ChildScreenUrl;

        /// <summary>子画面を閉じるフラグ</summary>
        private HiddenField CloseFlag;

        /// <summary>子画面を閉じた後の後処理を判別するフラグ</summary>
        private HiddenField SubmitFlag;

        /// <summary>ダイアログのスタイルを指定する。</summary>
        private HiddenField FxDialogStyle;

        /// <summary>業務モーダル画面のスタイルを指定する。</summary>
        private HiddenField BusinessDialogStyle;

        /// <summary>業務モードレス画面のスタイルを指定する。</summary>
        private HiddenField NormalScreenStyle;

        /// <summary>業務モードレス画面のターゲットを指定する。</summary>
        private HiddenField NormalScreenTarget;

        /// <summary>業務モーダル画面ロード用HTMLのURLを指定する。</summary>
        private HiddenField DialogFrameUrl;

        /// <summary>複数ダイアログ親画面対応（画面GUID）</summary>
        private HiddenField ScreenGuid;

        /// <summary>ボタン履歴情報記録機能のon・off</summary>
        private bool ButtonHistoryRecorder;

        /// <summary>ボタン履歴情報を保持するキュー</summary>
        private Queue<ArrayList> Buttonhistory_Queue;

        /// <summary>ボタン履歴情報を保持するキューのサイズ</summary>
        private int ButtonhistoryMaxQueueLength;

        /// <summary>ボタン履歴情報を保持するスタック（１画面に１つ）</summary>
        private Stack<FxEventArgs> Buttonhistory_Stack;

        #endregion

        #region セッション関連

        /// <summary>画面GUIDを保持するキュー</summary>
        private Queue<string> ScreenGuid_Queue = null;

        /// <summary>ウィンドウGUIDを保持するキュー</summary>
        private Queue<string> WindowGuid_Queue = null;

        #endregion

        #region Ajax関連

        /// <summary>スクリプト マネージャ</summary>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected ScriptManager CurrentScriptManager;

        /// <summary>AjaxExtensionの状態</summary>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected FxEnum.AjaxExtStat AjaxExtensionStatus;

        /// <summary>ClientCallbackの状態</summary>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected bool IsClientCallback = false;

        #endregion

        #endregion

        #region コンストラクタ

        /// <summary>BaseControllerクラスのコンストラクタ</summary>
        /// <remarks>Page_Loadイベントハンドラを登録する。</remarks>
        public BaseController()
        {
            // Page_Loadイベントハンドラの登録処理
            this.Load += new System.EventHandler(this.Page_Load);
        }

        #endregion

        #region イベント ハンドラ

        #region ページロードのイベントハンドラ

        /// <summary>
        /// 不正操作防止機能の動作変更フラグ
        /// </summary>
        /// <remarks>
        /// null以外の値が設定されている場合
        /// app.configの設定を優先する。
        /// </remarks>
        protected bool? CanCheckIllegalOperation = null;

        /// <summary>Sessionを使用しない隠しフラグ</summary>
        /// <remarks>隠しフラグなのでインテリセンスから参照不可</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected bool IsNoSession = false;

        /// <summary>Page_Loadのイベントハンドラ</summary>
        private void Page_Load(object sender, EventArgs e)
        {
            // isNoSessionフラグをtrueに設定すると、当該画面でのみ以下の機能をOFFにして、
            // ・ボタン履歴情報記録機能
            // ・不正操作防止機能

            // 最後に

            // ・Session.Abandon();
            //　 ＋ Sessionタイムアウト検出用Cookieの消去

            // を実行する。

            // これにより、ログイン画面相当（先頭画面であること）で、
            // 基盤側の処理でSessionIDを返さないように都度クリア。

            // なお、次画面への遷移には、リダイレクトのGETを使用する。
            // （ブラウザウィンドウ スコープが自動消去されなくなるので必要であれば念の為）

            // 中間画面でこのフラグをTrueに設定した場合の動作保証はしない。
            // （IDEのインテリセンスに出ない（隠しコマンド的）扱いにする）

            bool isNoSession = this.IsNoSession;
            bool? canCheckIllegalOperation = this.CanCheckIllegalOperation;

            try
            {
                #region セッション スコープのロック

                // 2009/09/25-start

                // セッションステートレス対応
                if (HttpContext.Current.Session == null)
                {
                    // SessionがOFFの場合
                    this.SessionLock = new object();
                }
                else
                {
                    // チェック
                    this.SessionLock = Session[FxHttpSessionIndex.SESSION_LOCK];

                    if (this.SessionLock == null)
                    {
                        // nullの場合、新規生成
                        Session[FxHttpSessionIndex.SESSION_LOCK] = new object();
                        this.SessionLock = Session[FxHttpSessionIndex.SESSION_LOCK];
                    }
                    else
                    {
                        // 存在する。
                    }
                }

                // 2009/09/25-end

                #endregion

                // 念のためセッション単位でロック。
                lock (this.SessionLock)
                {
                    #region フレームワークの初期処理

                    #region マスタ ページの初期化

                    // コンテンツ ページのファイル名（拡張子抜き）を取り出す
                    //string[] aryContentPageFile = this.AppRelativeVirtualPath.Split('/');
                    //this.ContentPageFileNoEx = aryContentPageFile[aryContentPageFile.Length - 1].Split('.')[0];
                    this.ContentPageFileNoEx = PubCmnFunction.GetFileNameNoEx(this.AppRelativeVirtualPath, '/');

                    // マスタ ページを取得し、
                    // ルートのマスタ ページ名も初期化する。
                    this.GetMasterPages();

                    #endregion

                    #region ユーザ コントロールの初期化

                    // ユーザ コントロールの初期化
                    this.GetUserControl(this);

                    #endregion

                    #region Ajaxの状態を確認

                    #region ASP.NET Ajax

                    // スクリプト マネージャを取得
                    this.CurrentScriptManager = ScriptManager.GetCurrent(this);

                    if (this.CurrentScriptManager == null)
                    {
                        // AjaxExtensionをサポートしない画面
                        this.AjaxExtensionStatus = FxEnum.AjaxExtStat.NoAjaxExtension;
                    }
                    else
                    {
                        if (this.CurrentScriptManager.IsInAsyncPostBack)
                        {
                            // AjaxExtensionをサポートする画面であり、当該処理はAjaxExtensionである。
                            this.AjaxExtensionStatus = FxEnum.AjaxExtStat.IsAjaxExtension;
                        }
                        else
                        {
                            // AjaxExtensionをサポートする画面だが、当該処理はAjaxExtensionでない。
                            this.AjaxExtensionStatus = FxEnum.AjaxExtStat.IsNotAjaxExtension;
                        }
                    }

                    #endregion

                    #region ClientCallback

                    if (Request.Form[FxLiteral.CALLBACK_ID] == null)
                    {
                        // ClientCallbackのポストバック以外のリクエスト
                        this.IsClientCallback = false;
                    }
                    else
                    {
                        // #22-start
                        //if (this.AjaxExtensionStatus == FxEnum.AjaxExtStat.IsAjaxExtension)
                        //{
                        //    // ClientCallbackを使用したAjaxExtension
                        //    this.IsClientCallback = false;
                        //}
                        //else
                        //{
                        // ClientCallbackのポストバック
                        this.IsClientCallback = true;
                        //}
                        // #22-end
                    }

                    #endregion

                    #endregion

                    #region エラー画面へのパスチェック

                    string errorScreenPath =
                        GetConfigParameter.GetConfigValue(FxLiteral.ERROR_SCREEN_PATH);

                    // エラー処理
                    if (errorScreenPath == null || errorScreenPath == "")
                    {
                        throw new FrameworkException(
                            FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1[0],
                            String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1[1], FxLiteral.ERROR_SCREEN_PATH));
                    }

                    #endregion

                    #region セッションタイムアウト検出処理

                    // セッションタイムアウト検出処理の定義を取得
                    string sessionTimeOutCheck =
                        GetConfigParameter.GetConfigValue(FxLiteral.SESSION_TIMEOUT_CHECK);

                    // デフォルト値対策：設定なし（null）の場合の扱いを決定
                    if (sessionTimeOutCheck == null)
                    {
                        // OFF扱い
                        sessionTimeOutCheck = FxLiteral.OFF;
                    }

                    // ON / OFF
                    if (sessionTimeOutCheck.ToUpper() == FxLiteral.ON)
                    {
                        // セッションタイムアウト検出処理（ON）

                        // セッション状態の確認
                        if (Session.IsNewSession)
                        {
                            // 新しいセッションが開始された

                            // セッションタイムアウト検出用Cookieをチェック
                            HttpCookie cookie = Request.Cookies.Get(FxHttpCookieIndex.SESSION_TIMEOUT);

                            if (cookie == null)
                            {
                                // セッションタイムアウト検出用Cookie無し → 新規のアクセス

                                // 2009/09/18-start

                                // セッションタイムアウト検出用Cookieを新規作成（値は空文字以外、何でも良い）

                                // Set-Cookie HTTPヘッダをレスポンス
                                Response.Cookies.Set(FxCmnFunction.CreateCookieForSessionTimeoutDetection());
                                // 2009/09/18-end
                            }
                            else
                            {
                                // セッションタイムアウト検出用Cookie有り

                                if (cookie.Value == "")
                                {
                                    // セッションタイムアウト発生後の新規アクセス

                                    // だが、値が消去されている（空文字に設定されている）場合は、
                                    // 一度エラー or セッションタイムアウトになった後の新規のアクセスである。

                                    // 2009/09/18-start

                                    // セッションタイムアウト検出用Cookieを再作成（値は空文字以外、何でも良い）

                                    // Set-Cookie HTTPヘッダをレスポンス
                                    Response.Cookies.Set(FxCmnFunction.CreateCookieForSessionTimeoutDetection());
                                    // 2009/09/18-end
                                }
                                else
                                {
                                    // セッションタイムアウト発生

                                    // エラー画面で以下の処理を実行する。
                                    // ・セッションタイムアウト検出用Cookieを消去
                                    // ・セッションを消去

                                    // ※ エラー画面への遷移方法がTransferになっているため、
                                    //    ここでセッションタイムアウト検出用Cookieを消去できないため。

                                    // セッションタイムアウト例外を発生させる
                                    throw new FrameworkException(
                                        FrameworkExceptionMessage.SESSION_TIMEOUT[0],
                                        FrameworkExceptionMessage.SESSION_TIMEOUT[1]);
                                }
                            }
                        }
                        else
                        {
                            // セッション継続中
                        }
                    }
                    else if (sessionTimeOutCheck.ToUpper() == FxLiteral.OFF)
                    {
                        // セッションタイムアウト検出処理（OFF）
                    }
                    else
                    {
                        // パラメータ・エラー（書式不正）
                        throw new FrameworkException(
                            FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[0],
                            String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[1],
                                FxLiteral.SESSION_TIMEOUT_CHECK));
                    }

                    // 2008/10/16---チェック処理の追加（ここまで）

                    #endregion

                    #region HIDDENコントロールの検索・取得

                    // 2009/07/21-start-end

                    #region 画面遷移制御関連

                    // ブラウザ ウィンドウGUID
                    this.WindowGuid = (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_WINDOW_GUID); // 2009/07/21-この行

                    // エラー処理
                    if (this.WindowGuid == null)
                    {
                        throw new FrameworkException(
                            FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                            String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1], FxLiteral.HIDDEN_WINDOW_GUID));
                    }

                    // リクエストチケットGUID
                    this.RequestTicketGuid = (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_REQUEST_TICKET_GUID); // 2009/07/21-この行

                    // エラー処理
                    if (this.RequestTicketGuid == null)
                    {
                        throw new FrameworkException(
                            FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                            String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1], FxLiteral.HIDDEN_REQUEST_TICKET_GUID));
                    }

                    #endregion

                    #region ダイアログ表示関連

                    // 開く子画面のタイプ
                    this.ChildScreenType =
                        (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_CHILD_SCREEN_TYPE); // 2009/07/21-この行

                    // エラー処理
                    if (this.ChildScreenType == null)
                    {
                        throw new FrameworkException(
                            FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                            String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1], FxLiteral.HIDDEN_CHILD_SCREEN_TYPE));
                    }
                    else
                    {
                        // 逐次初期化
                        this.ChildScreenType.Value = "0";
                    }

                    // 開く子画面のURL
                    this.ChildScreenUrl =
                        (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_CHILD_SCREEN_URL); // 2009/07/21-この行

                    // エラー処理
                    if (this.ChildScreenUrl == null)
                    {
                        throw new FrameworkException(
                            FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                            String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1], FxLiteral.HIDDEN_CHILD_SCREEN_URL));
                    }

                    // 子画面を閉じるフラグ
                    this.CloseFlag = (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_CLOSE_FLAG); // 2009/07/21-この行

                    // エラー処理
                    if (this.CloseFlag == null)
                    {
                        throw new FrameworkException(
                            FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                            String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1], FxLiteral.HIDDEN_CLOSE_FLAG));

                    }

                    // 子画面を閉じた後の後処理を判別するフラグ
                    this.SubmitFlag = (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_SUBMIT_FLAG); // 2009/07/21-この行

                    // エラー処理
                    if (this.SubmitFlag == null)
                    {
                        throw new FrameworkException(
                            FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                            String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1], FxLiteral.HIDDEN_SUBMIT_FLAG));

                    }

                    // 画面GUIDを保存するHiddenコントロールを取得
                    this.ScreenGuid = (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_SCREEN_GUID); // 2009/07/21-この行

                    // エラー処理
                    if (this.ScreenGuid == null)
                    {
                        throw new FrameworkException(
                            FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                            String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1], FxLiteral.HIDDEN_SCREEN_GUID));
                    }

                    #endregion

                    #endregion

                    #region 二重送信の検出処理

                    // 二重送信の検出処理の定義を取得
                    string doubleTransmissionCheck =
                        GetConfigParameter.GetConfigValue(FxLiteral.DOUBLE_TRANSMISSION_CHECK);

                    // デフォルト値対策：設定なし（null）の場合の扱いを決定
                    if (doubleTransmissionCheck == null)
                    {
                        // OFF扱い
                        doubleTransmissionCheck = FxLiteral.OFF;
                    }

                    // ON / OFF
                    if (doubleTransmissionCheck.ToUpper() == FxLiteral.ON)
                    {
                        // 二重送信の検出処理（ON）
                        // onSubmitイベントに、JavaScriptを仕掛ける。
                        this.Form.Attributes.Add("onSubmit", "return Fx_OnSubmit();");
                    }
                    else if (doubleTransmissionCheck.ToUpper() == FxLiteral.OFF)
                    {
                        // 二重送信の検出処理（OFF）
                    }
                    else
                    {
                        // パラメータ・エラー（書式不正）
                        throw new FrameworkException(
                            FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[0],
                            String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[1],
                                FxLiteral.DOUBLE_TRANSMISSION_CHECK));
                    }

                    #endregion

                    #region 不正操作の検出機能

                    int requestTicketGuidMaxQueueLength = 0;

                    // ajaxの場合は、実行しない。
                    if (this.IsClientCallback)
                    {
                        // ClientCallbackなので実行しない。
                        // requestTicketGuidMaxQueueLength = 0 のままに
                    }
                    else
                    {
                        if (this.AjaxExtensionStatus == FxEnum.AjaxExtStat.IsAjaxExtension)
                        {
                            // Ajax Extensionなので実行しない。
                            // requestTicketGuidMaxQueueLength = 0 のままに
                        }
                        else
                        {
                            // 不正操作の検出機能の定義を取得
                            // ※ 記録する操作の最大数
                            requestTicketGuidMaxQueueLength =
                                FxCmnFunction.GetNumFromConfig(
                                    FxLiteral.REQUEST_TICKET_GUID_MAX_QUEUE_LENGTH, 0);
                        }
                    }

                    // isNoSessionフラグ
                    if (isNoSession)
                    {
                        // 不正操作の検出機能（強制OFF）
                        requestTicketGuidMaxQueueLength = 0;
                    }
                    else
                    {
                        if (canCheckIllegalOperation == null)
                        {
                            // 定義値による
                        }
                        else if (canCheckIllegalOperation == true)
                        {
                            // 不正操作の検出機能（強制ON）
                            if (requestTicketGuidMaxQueueLength <= 0)
                            {
                                // OFF→ON // 保持する履歴の数は固定。
                                requestTicketGuidMaxQueueLength = 100;
                            }
                            else //(canCheckIllegalOperation == false)
                            {
                                // ON→ON // 定義値による。
                            }
                        }
                        else
                        {
                            // 不正操作の検出機能（強制OFF）
                            requestTicketGuidMaxQueueLength = 0;
                        }
                    }

                    // 定義のチェック
                    if (0 < requestTicketGuidMaxQueueLength)
                    {
                        // キューの取得
                        this.GetGuidQueue(
                            out this.RequestTicketGuid_Queue,
                            FxHttpSessionIndex.REQUEST_TICKET_GUID_QUEUE,
                            requestTicketGuidMaxQueueLength);

                        // チェック対象はポストバックのみ
                        if (this.IsPostBack)
                        {
                            // ポストバック

                            // HIDDENとSESSIONのリクエスト チケットGUIDをチェック
                            if (this.RequestTicketGuid_Queue.Contains(this.RequestTicketGuid.Value))
                            {
                                // ＝ → 正常

                                // 旧リクエスト チケットGUIDの作成
                                string oldGuid = this.RequestTicketGuid.Value;
                                // 新規リクエスト チケットGUIDの作成
                                string newGuid = Guid.NewGuid().ToString();

                                // HIDDENとSESSIONに保存

                                // HIDDEN
                                this.RequestTicketGuid.Value = newGuid;

                                // SESSION（リクエスト チケットGUIDキューの再構築）
                                this.RestructureGuidQueue(
                                    oldGuid, newGuid, out this.RequestTicketGuid_Queue,
                                    FxHttpSessionIndex.REQUEST_TICKET_GUID_QUEUE,
                                    requestTicketGuidMaxQueueLength);
                            }
                            else
                            {
                                // ≠ → 異常

                                //  不正操作を検出
                                throw new FrameworkException(
                                    FrameworkExceptionMessage.ILLEGAL_OPERATION_CHECK_ERROR[0],
                                    FrameworkExceptionMessage.ILLEGAL_OPERATION_CHECK_ERROR[1]);
                            }
                        }
                        else
                        {
                            // 初回ロード

                            // 新規リクエスト チケットGUIDの作成
                            string guid = Guid.NewGuid().ToString();

                            // HIDDENとSESSIONに保存

                            // HIDDEN
                            this.RequestTicketGuid.Value = guid;

                            // SESSION

                            // キューの最大長を超える場合は、デキューする
                            if (requestTicketGuidMaxQueueLength <= this.RequestTicketGuid_Queue.Count)
                            {
                                // 最も古いリクエスト チケットGUIDをデキュー
                                RequestTicketGuid_Queue.Dequeue();

                            }

                            // 新規のリクエスト チケットGUIDをエンキュー
                            RequestTicketGuid_Queue.Enqueue(guid);
                        }
                    }

                    #endregion

                    #region 画面遷移制御機能

                    // 2009/07/31-start

                    #region ウィンドウGUID設定

                    // ウィンドウGUID
                    string browserWindowGuid = "";

                    // ウィンドウ別セッション領域の自動削除機能の定義を取得
                    // ※ ウィンドウ別セッション領域のスコープの最大数
                    int windowGuidMaxQueueLength =
                        FxCmnFunction.GetNumFromConfig(
                            FxLiteral.WINDOW_GUID_MAX_QUEUE_LENGTH, 0);

                    if (this.WindowGuid.Value.Length == 36)
                    {
                        // ウィンドウGUIDがある場合

                        #region ウィンドウGUIDキューの再構築

                        // 定義のチェック
                        if (0 < windowGuidMaxQueueLength)
                        {
                            // ウィンドウGUIDキューの再構築
                            this.RestructureGuidQueue(this.WindowGuid.Value, out this.WindowGuid_Queue,
                                FxHttpSessionIndex.WINDOW_GUID_QUEUE, windowGuidMaxQueueLength);
                        }

                        #endregion
                    }
                    else
                    {
                        // ウィンドウGUIDがない場合

                        #region ウィンドウGUIDの初期化

                        // HTTP ContextにウィンドウGUIDがあるか？
                        if (HttpContext.Current.Items[
                            FxHttpContextIndex.BROWSER_WINDOW_GUID] == null)
                        { }
                        else
                        {
                            browserWindowGuid
                                = HttpContext.Current.Items[
                                    FxHttpContextIndex.BROWSER_WINDOW_GUID].ToString();
                        }

                        // 無かった場合、
                        if (browserWindowGuid == "")
                        {
                            // HTTP Query StringにウィンドウGUIDがあるか？
                            if (HttpContext.Current.Request.QueryString[
                                    FxHttpQueryStringIndex.BROWSER_WINDOW_GUID] == null)
                            { }
                            else
                            {
                                browserWindowGuid
                                    = HttpContext.Current.Request.QueryString[
                                        FxHttpQueryStringIndex.BROWSER_WINDOW_GUID].ToString();
                            }
                        }

                        // ウィンドウGUIDが新規生成されたか判別するフラグ
                        bool isNewWindowGuid = false;

                        // 無かった場合、
                        if (browserWindowGuid == "")
                        {
                            // ウィンドウGUIDを新規生成し、ウィンドウGUIDをHIDDEに設定
                            this.WindowGuid.Value = Guid.NewGuid().ToString();
                            isNewWindowGuid = true;
                        }
                        else
                        {
                            // ウィンドウGUIDをHIDDEに設定
                            this.WindowGuid.Value = browserWindowGuid;
                            isNewWindowGuid = false;
                        }

                        #endregion

                        #region ウィンドウ別セッション領域の自動削除とウィンドウGUIDキューの再構築

                        // 定義のチェック
                        if (0 < windowGuidMaxQueueLength)
                        {
                            if (isNewWindowGuid)
                            {
                                // 新規ウィンドウGUIDが生成された場合

                                // ウィンドウGUIDキューからセッション自動削除
                                this.DeleteSessionWithGuidQueueAutomatically(
                                    this.WindowGuid.Value, this.WindowGuid_Queue,
                                    FxHttpSessionIndex.WINDOW_GUID_QUEUE, windowGuidMaxQueueLength);
                            }
                            else
                            {
                                // 新規ウィンドウGUIDが生成されなかった場合

                                // ウィンドウGUIDキューの再構築
                                this.RestructureGuidQueue(this.WindowGuid.Value, out this.WindowGuid_Queue,
                                    FxHttpSessionIndex.WINDOW_GUID_QUEUE, windowGuidMaxQueueLength);
                            }
                        }

                        #endregion
                    }

                    #endregion

                    // 2009/07/31-end

                    #region 画面遷移チェック処理

                    if (SC.TransitionCheck)
                    {
                        // ポストバック
                        if (this.IsPostBack)
                        {
                            // ポストバック時はチェックしない。
                        }
                        else
                        {
                            // 初回ロード時にチェックする。

                            // ウィンドウ別セッション領域から前画面情報を取得する。
                            if (this.GetDataFromBrowserWindow(
                                    FxHttpSessionIndex.SCREEN_TRANSITION_INFO) == null)
                            {
                                this.FormerAspxVirtualPath = "";
                            }
                            else
                            {
                                this.FormerAspxVirtualPath
                                = this.GetDataFromBrowserWindow(
                                    FxHttpSessionIndex.SCREEN_TRANSITION_INFO).ToString();
                            }

                            // 取得後、直ちに消去
                            this.DeleteDataFromBrowserWindow(
                                    FxHttpSessionIndex.SCREEN_TRANSITION_INFO);

                            if (this.FormerAspxVirtualPath == "")
                            {
                                // 全画面情報無しなので、部品で遷移していない場合の、画面遷移チェック
                                if (HttpContext.Current.Request.HttpMethod.ToUpper() == FxLiteral.GET)
                                {
                                    // GET → GETチェック
                                    BaseController.SC.
                                        CheckScreenTransitionGet(this.AppRelativeVirtualPath);
                                }
                                else
                                {
                                    // POST → 強制的にエラー
                                    BaseController.SC.
                                        CheckScreenTransitionPost(this.AppRelativeVirtualPath);
                                }
                            }
                            else
                            {
                                // 前画面情報有りなので、部品で遷移している場合の、画面遷移チェック
                                BaseController.SC.CheckScreenTransition(
                                    this.FormerAspxVirtualPath,
                                    this.AppRelativeVirtualPath);
                            }
                        }
                    }
                    else
                    {
                        // 画面遷移チェックしない。
                    }

                    #endregion

                    #endregion

                    #region 画面GUID関連の処理

                    // ClientCallback時は実行しない。
                    if (!this.IsClientCallback)
                    {
                        // 2010/10/04-start

                        # region ボタン履歴情報記録機能のon・off

                        // ボタン履歴情報記録機能のon・offの定義を取得
                        int tempButtonhistoryMaxQueueLength =
                            FxCmnFunction.GetNumFromConfig(
                                    FxLiteral.BUTTON_HISTORY_MAX_QUEUE_LENGTH,
                                    FxLiteral.BUTTON_HISTORY_DEFAULT_QUEUE_LENGTH);

                        // ON( > 0) / OFF( <= 0)
                        if (tempButtonhistoryMaxQueueLength > 0)
                        {
                            // ボタン履歴情報記録機能（ON）
                            this.ButtonHistoryRecorder = true;
                        }
                        else
                        {
                            // ボタン履歴情報記録機能（OFF）
                            this.ButtonHistoryRecorder = false;
                        }

                        // isNoSessionフラグ
                        if (isNoSession)
                        {
                            // ボタン履歴情報記録機能（OFF）
                            this.ButtonHistoryRecorder = false;
                        }

                        #endregion

                        #region ボタン履歴情報（キュー）の初期化

                        if (this.ButtonHistoryRecorder) // 2008/03/29---ボタン履歴記録機能が有効な場合のみ実行する（追加）。
                        {
                            // ボタン履歴記録機能が無効な場合、キューの最大長を指定
                            this.ButtonhistoryMaxQueueLength = tempButtonhistoryMaxQueueLength;
                        }
                        else
                        {
                            // ボタン履歴記録機能が無効な場合は「0」で初期化
                            this.ButtonhistoryMaxQueueLength = 0; // = tempButtonhistoryMaxQueueLength;
                        }

                        if (this.ButtonHistoryRecorder) // 2008/11/28---ボタン履歴記録機能が有効な場合のみ実行する（追加）。
                        {
                            if (Session[FxHttpSessionIndex.BUTTON_HISTORY] == null)
                            {
                                // 新規生成（キューの最大長を指定する）
                                this.Buttonhistory_Queue = new Queue<ArrayList>(this.ButtonhistoryMaxQueueLength);

                                // Sessionに設定
                                Session[FxHttpSessionIndex.BUTTON_HISTORY] = this.Buttonhistory_Queue;
                            }
                            else
                            {
                                // Session領域から取得
                                this.Buttonhistory_Queue = (Queue<ArrayList>)Session[FxHttpSessionIndex.BUTTON_HISTORY];
                            }
                        }

                        #endregion

                        // 2010/10/04-end

                        // 2009/07/31-start

                        // 画面GUIDが新規生成されたか判別するフラグ
                        bool isNewScreenGuid = false;

                        // 親画面別セッション領域の自動削除機能の定義を取得
                        // ※ 親画面別セッション領域のスコープの最大数
                        int screenGuidMaxQueueLength =
                            FxCmnFunction.GetNumFromConfig(
                                FxLiteral.SCREEEN_GUID_MAX_QUEUE_LENGTH, 0);

                        if (this.ScreenGuid.Value.Length == 36)
                        {
                            // 画面GUIDがある場合 → 親画面・子画面のポストバック時

                            #region ボタン履歴（キュー）の再構築

                            if (this.ButtonHistoryRecorder) // 2008/11/28---ボタン履歴記録機能が有効な場合のみ実行する（追加）。
                            {
                                // ボタン履歴情報（キュー）から、
                                // 画面GUID付きボタン履歴情報（スタック）を取得
                                foreach (ArrayList tempObj in this.Buttonhistory_Queue) // ここはArrayListでないとNG（Buttonhistory_Queue）
                                {
                                    // 画面GUIDのインデックスを確認
                                    if (tempObj[0].ToString() == this.ScreenGuid.Value)
                                    {
                                        // ボタン履歴情報（スタック）を取得
                                        this.Buttonhistory_Stack = (Stack<FxEventArgs>)tempObj[1];
                                    }
                                }

                                // ※ 上記の処理で、Buttonhistory_Stackが取得できない場合に考えられる理由。
                                // 　 ・ セッションタイムアウトした場合（検出機能OFFの場合）
                                // 　 ・ 古いキャッシュを参照した場合
                                // 　 ・ スタックがキューからデキューされた場合

                                if (this.Buttonhistory_Stack == null)
                                {
                                    // キャッシュを参照した可能性
                                    throw new FrameworkException(
                                        FrameworkExceptionMessage.FX_PROCESSING_STATUS_ERROR[0],
                                        String.Format(FrameworkExceptionMessage.FX_PROCESSING_STATUS_ERROR[1],
                                            FrameworkExceptionMessage.FX_PROCESSING_STATUS_ERROR_NO_BH_QUEUE));
                                }

                                // ボタン履歴情報（キュー）の再構築

                                // ☆ Sessionに再設定する必要がある
                                //    （作り直しでインスタンスが変わったため）
                                Session[FxHttpSessionIndex.BUTTON_HISTORY]
                                    = FxCmnFunction.RestructuringLRUQueue2(
                                        this.Buttonhistory_Queue,
                                        this.ScreenGuid.Value,
                                        this.ButtonhistoryMaxQueueLength);

                                // ※ ボタン履歴情報（スタック）を操作するのは、イベント ハンドラ
                            }

                            #endregion

                            #region 画面GUIDキューの再構築

                            // 定義のチェック
                            if (0 < screenGuidMaxQueueLength)
                            {
                                // 画面GUIDキューの再構築
                                this.RestructureGuidQueue(this.ScreenGuid.Value, out this.ScreenGuid_Queue,
                                    FxHttpSessionIndex.SCREEN_GUID_QUEUE, screenGuidMaxQueueLength);
                            }

                            #endregion
                        }
                        else
                        {
                            // 画面GUIDがない場合 → 親画面・子画面の初回ロード時

                            #region 画面GUIDの初期化

                            string QueryStringScreenGuid
                                = (string)Request.QueryString[FxHttpQueryStringIndex.PARENT_SCREEN_GUID];

                            if (QueryStringScreenGuid == null)
                            {
                                QueryStringScreenGuid = "";
                            }

                            #endregion

                            #region ボタン履歴（スタック）の初期化

                            if (QueryStringScreenGuid.Length == 36)
                            {
                                // クエリーストリングがある場合 → 自画面は業務モーダル画面で、且つ初回ロード時

                                // 画面GUIDを設定
                                this.ScreenGuid.Value = QueryStringScreenGuid;

                                #region ボタン履歴（スタックをキューから取得）

                                if (this.ButtonHistoryRecorder) // 2008/11/28---ボタン履歴記録機能が有効な場合のみ実行する（追加）。
                                {
                                    // ボタン履歴情報（キュー）から、
                                    // 画面GUID付きボタン履歴情報（スタック）を取得
                                    foreach (ArrayList tempObj in this.Buttonhistory_Queue) // ここはArrayListでないとNG（Buttonhistory_Queue）
                                    {
                                        // 画面GUIDのインデックスを確認
                                        if (tempObj[0].ToString() == this.ScreenGuid.Value)
                                        {
                                            // ボタン履歴情報（スタック）を取得
                                            this.Buttonhistory_Stack = (Stack<FxEventArgs>)tempObj[1];
                                        }
                                    }

                                    // 初期値をプッシュ
                                    this.Buttonhistory_Stack.Push(
                                        new FxEventArgs(FxLiteral.VALUE_STR_DUMMY_STRING, 0, 0, "", ""));
                                }

                                #endregion
                            }
                            else
                            {
                                // クエリーストリングがない場合 → 自画面は親画面で、且つ初回ロード時

                                // 画面GUIDを新規生成
                                this.ScreenGuid.Value = Guid.NewGuid().ToString();
                                isNewScreenGuid = true;

                                #region ボタン履歴（スタックを新規生成）

                                if (this.ButtonHistoryRecorder) // 2008/11/28---ボタン履歴記録機能が有効な場合のみ実行する（追加）。
                                {
                                    // ボタン履歴情報（スタック）を新規生成
                                    this.Buttonhistory_Stack = new Stack<FxEventArgs>();

                                    // 初期値をプッシュ
                                    this.Buttonhistory_Stack.Push(
                                        new FxEventArgs(FxLiteral.VALUE_STR_DUMMY_STRING, 0, 0, "", ""));

                                    // ボタン履歴情報（スタック）をボタン履歴情報（キュー）に格納

                                    // ボタン履歴情報（スタック）に格納するデータ型は配列
                                    ArrayList tempObj = new ArrayList(2); // ここはArrayListでないとNG（Buttonhistory_Queue）

                                    // 画面GUIDのインデックス
                                    tempObj.Add(this.ScreenGuid.Value);

                                    // ボタン履歴情報（スタック）
                                    tempObj.Add(this.Buttonhistory_Stack);

                                    // キューの最大長を超える場合は、デキューする
                                    if (this.ButtonhistoryMaxQueueLength <= this.Buttonhistory_Queue.Count) // 2009/08/12-この行
                                    {
                                        // デキュー
                                        this.Buttonhistory_Queue.Dequeue();
                                    }

                                    // エンキュー
                                    this.Buttonhistory_Queue.Enqueue(tempObj);
                                }

                                #endregion
                            }

                            #endregion

                            #region 親画面別セッション領域の自動削除と画面GUIDキューの再構築

                            // 定義のチェック
                            if (0 < screenGuidMaxQueueLength)
                            {
                                if (isNewScreenGuid)
                                {
                                    // 新規画面GUIDが生成された場合

                                    // 画面GUIDキューからセッション自動削除
                                    this.DeleteSessionWithGuidQueueAutomatically(
                                        this.ScreenGuid.Value, this.ScreenGuid_Queue,
                                        FxHttpSessionIndex.SCREEN_GUID_QUEUE, screenGuidMaxQueueLength);
                                }
                                else
                                {
                                    // 新規画面GUIDが生成されなかった場合

                                    // 画面GUIDキューの再構築
                                    this.RestructureGuidQueue(this.ScreenGuid.Value, out this.ScreenGuid_Queue,
                                        FxHttpSessionIndex.SCREEN_GUID_QUEUE, screenGuidMaxQueueLength);
                                }
                            }

                            #endregion
                        }

                        // 2009/07/31-end
                    }

                    #endregion

                    #region コントロール取得処理

                    #region 旧処理
                    ////#if DEBUG // 2009/09/01-このプリプロセッサ
                    ////                    PerformanceRecorder perfRec = new PerformanceRecorder();
                    ////                    perfRec.StartsPerformanceRecord();
                    ////#endif

                    //// BUTTON
                    //FxCmnFunction.GetCtrlAndSetClickEventHandler(
                    //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_BUTTON),
                    //    new System.EventHandler(this.Button_Click), this.ControlHt);

                    ////#if DEBUG // 2009/09/01-このプリプロセッサ
                    ////                    Debug.WriteLine("BUTTONコントロール検索のパフォーマンス情報：\r\n" + perfRec.EndsPerformanceRecord());
                    ////#endif

                    //// LINK BUTTON
                    //FxCmnFunction.GetCtrlAndSetClickEventHandler(
                    //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LINK_BUTTON),
                    //    new System.EventHandler(this.Button_Click), this.ControlHt);

                    //// IMAGE BUTTON
                    //FxCmnFunction.GetCtrlAndSetClickEventHandler(
                    //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_IMAGE_BUTTON),
                    //    new System.Web.UI.ImageClickEventHandler(this.ImageButton_Click), this.ControlHt);

                    //// IMAGE MAP
                    //FxCmnFunction.GetCtrlAndSetClickEventHandler(
                    //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_IMAGE_MAP),
                    //    new System.Web.UI.WebControls.ImageMapEventHandler(this.ImageMap_Click), this.ControlHt);

                    //// DROP DOWN LIST
                    //FxCmnFunction.GetCtrlAndSetClickEventHandler(
                    //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_DROP_DOWN_LIST),
                    //    new System.EventHandler(this.List_SelectedIndexChanged), this.ControlHt);

                    //// LIST BOX
                    //FxCmnFunction.GetCtrlAndSetClickEventHandler(
                    //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LIST_BOX),
                    //    new System.EventHandler(this.List_SelectedIndexChanged), this.ControlHt);

                    //// RADIO BUTTON
                    //FxCmnFunction.GetCtrlAndSetClickEventHandler(
                    //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_RADIO_BUTTON),
                    //    new System.EventHandler(this.Check_CheckedChanged), this.ControlHt);

                    //// REPEATER
                    //FxCmnFunction.GetCtrlAndSetClickEventHandler(
                    //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_REPEATER),
                    //    new RepeaterCommandEventHandler(this.Repeater_ItemCommand), this.ControlHt);

                    //// GRIDVIEW
                    //object[] gridViewEventHandlers = new object[]{
                    //    new GridViewCommandEventHandler(this.GridView_RowCommand),
                    //    new EventHandler(this.List_SelectedIndexChanged),
                    //    new GridViewUpdateEventHandler(this.GridView_RowUpdating),
                    //    new GridViewDeleteEventHandler(this.GridView_RowDeleting),
                    //    new GridViewPageEventHandler(this.GridView_PageIndexChanging),
                    //    new GridViewSortEventHandler(this.GridView_Sorting)
                    //};

                    //FxCmnFunction.GetCtrlAndSetClickEventHandler(
                    //    this, GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_GRIDVIEW),
                    //    gridViewEventHandlers, this.ControlHt);
                    #endregion

                    // プレフィックス
                    string prefix = "";
                    // プレフィックスとイベント ハンドラのディクショナリを生成
                    Dictionary<string, object> prefixAndEvtHndHt = new Dictionary<string, object>();

                    // BUTTON
                    prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_BUTTON);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        prefixAndEvtHndHt.Add(prefix, new System.EventHandler(this.Button_Click));
                    }

                    // LINK BUTTON
                    prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LINK_BUTTON);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        prefixAndEvtHndHt.Add(prefix, new System.EventHandler(this.Button_Click));
                    }

                    // IMAGE BUTTON
                    prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_IMAGE_BUTTON);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        prefixAndEvtHndHt.Add(prefix, new System.Web.UI.ImageClickEventHandler(this.ImageButton_Click));
                    }

                    // IMAGE MAP
                    prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_IMAGE_MAP);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        prefixAndEvtHndHt.Add(prefix, new System.Web.UI.WebControls.ImageMapEventHandler(this.ImageMap_Click));
                    }

                    // DROP DOWN LIST
                    prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_DROP_DOWN_LIST);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        prefixAndEvtHndHt.Add(prefix, new System.EventHandler(this.List_SelectedIndexChanged));
                    }

                    // LIST BOX
                    prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LIST_BOX);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        prefixAndEvtHndHt.Add(prefix, new System.EventHandler(this.List_SelectedIndexChanged));
                    }

                    // RADIO BUTTON
                    prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_RADIO_BUTTON);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        prefixAndEvtHndHt.Add(prefix, new System.EventHandler(this.Check_CheckedChanged));
                    }

                    // REPEATER
                    prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_REPEATER);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        prefixAndEvtHndHt.Add(prefix, new RepeaterCommandEventHandler(this.Repeater_ItemCommand));
                    }

                    // GRIDVIEW
                    prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_GRIDVIEW);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        object[] gridViewEventHandlers = new object[]{
                        new GridViewCommandEventHandler(this.GridView_RowCommand),
                        new EventHandler(this.List_SelectedIndexChanged),
                        new GridViewUpdateEventHandler(this.GridView_RowUpdating),
                        new GridViewDeleteEventHandler(this.GridView_RowDeleting),
                        new GridViewPageEventHandler(this.GridView_PageIndexChanging),
                        new GridViewSortEventHandler(this.GridView_Sorting)};

                        prefixAndEvtHndHt.Add(prefix, gridViewEventHandlers);
                    }

                    // LISTVIEW
                    prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_LISTVIEW);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        object[] listViewEventHandlers = new object[]{
                        new EventHandler<ListViewDeleteEventArgs>(this.ListView_ItemDeleting),
                        new EventHandler<ListViewUpdateEventArgs>(this.ListView_ItemUpdating),                       
                        new EventHandler(this.ListView_PagePropertiesChanged),
                        new EventHandler<ListViewSortEventArgs>(this.ListView_Sorting),                         
                        new EventHandler(this.List_SelectedIndexChanged),
                         new EventHandler<ListViewCommandEventArgs>(this.ListView_OnItemCommand)
                        };

                        prefixAndEvtHndHt.Add(prefix, listViewEventHandlers);
                    }

                    // RADIO BUTTON LIST
                    prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_RADIOBUTTONLIST);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        prefixAndEvtHndHt.Add(prefix, new System.EventHandler(this.List_SelectedIndexChanged));
                    }

                    //CHECKBOX LIST
                    prefix = GetConfigParameter.GetConfigValue(FxLiteral.PREFIX_OF_CHECKBOXLIST);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        prefixAndEvtHndHt.Add(prefix, new System.EventHandler(this.List_SelectedIndexChanged));
                    }

                    // コントロール検索＆イベントハンドラ設定
                    FxCmnFunction.GetCtrlAndSetClickEventHandler2(this, prefixAndEvtHndHt, this.ControlHt);

                    #endregion

                    #endregion

                    #region 画面の初期処理（初回ロード、ポストバック）

                    #region 共通UOC処理（UOC_CMNFormInit、UOC_CMNFormInit_PostBack）。

                    if (this.IsPostBack)
                    {
                        // ポストバック時

                        // ポストバック時のUOCメソッド
                        this.UOC_CMNFormInit_PostBack();
                    }
                    else
                    {
                        // 初回起動時

                        // 初回起動時のUOCメソッド
                        this.UOC_CMNFormInit();
                    }

                    #endregion

                    #region 個別UOC処理（UOC_FormInit、UOC_FormInit_PostBack）。

                    if (this.IsPostBack)
                    {
                        // ポストバック時

                        // ポストバック時のUOCメソッド
                        this.UOC_FormInit_PostBack();
                    }
                    else
                    {
                        // 初回起動時

                        // 初回起動時のUOCメソッド
                        this.UOC_FormInit();
                    }

                    #endregion

                    #endregion

                    // ・ ポストバック時のみ実行
                    // ・ ClientCallback時は実行しない。
                    // ・ AjaxExtension時は実行しない。
                    if (this.IsPostBack
                        && !this.IsClientCallback
                        && !(this.AjaxExtensionStatus == FxEnum.AjaxExtStat.IsAjaxExtension))
                    {
                        #region ダイアログ（から戻った場合）の後処理（イベント）

                        // 子画面を閉じた後の後処理を判別するフラグのnullチェック
                        if (this.SubmitFlag == null)
                        {
                            // nullの場合
                        }
                        else
                        {
                            // 値が1・2・3・4に該当する場合、後処理をする。

                            // ★１：「YES」・「NO」メッセージ・ダイアログの「×」が押され閉じられた場合。
                            // ★２：「YES」・「NO」メッセージ・ダイアログの「YES」が押された場合。
                            // ★３：「YES」・「NO」メッセージ・ダイアログの「NO」が押された場合。
                            // ★４：業務モーダル・ダイアログから戻った場合。

                            if (((int)FxEnum.SubmitMode.YesNo_X).ToString() == this.SubmitFlag.Value)
                            {
                                // ★ イベントの開始前のUOC処理
                                this.UOC_PreAction(new FxEventArgs(FxLiteral.EVENT_AFTER_YES_NO_X, 0, 0, "", ""));

                                // ★：「YES」・「NO」メッセージ・ダイアログの「×」が押され閉じられた場合。

                                if (this.ButtonHistoryRecorder) // 2008/11/28---ボタン履歴記録機能が有効な場合のみ実行する（追加）。
                                {
                                    // 引数：親画面で押したボタン情報
                                    this.UOC_YesNoDialog_X_Click((FxEventArgs)this.Buttonhistory_Stack.Peek());
                                }
                                else
                                {
                                    // 引数：ダミーのボタン情報
                                    this.UOC_YesNoDialog_X_Click(
                                        new FxEventArgs(FxLiteral.VALUE_STR_DUMMY_STRING, 0, 0, "", ""));
                                }

                                // ★ イベントの終了後のUOC処理
                                this.UOC_AfterAction(new FxEventArgs(FxLiteral.EVENT_AFTER_YES_NO_X, 0, 0, "", ""));
                            }
                            else if (((int)FxEnum.SubmitMode.YesNo_Yes).ToString() == this.SubmitFlag.Value)
                            {
                                // ★ イベントの開始前のUOC処理
                                this.UOC_PreAction(new FxEventArgs(FxLiteral.EVENT_AFTER_YES_NO_YES, 0, 0, "", ""));

                                // ★：「YES」・「NO」メッセージ・ダイアログの「YES」が押された場合。

                                if (this.ButtonHistoryRecorder) // 2008/11/28---ボタン履歴記録機能が有効な場合のみ実行する（追加）。
                                {
                                    // 引数：親画面で押したボタン情報
                                    this.UOC_YesNoDialog_Yes_Click((FxEventArgs)this.Buttonhistory_Stack.Peek());
                                }
                                else
                                {
                                    // 引数：ダミーのボタン情報
                                    this.UOC_YesNoDialog_Yes_Click(
                                        new FxEventArgs(FxLiteral.VALUE_STR_DUMMY_STRING, 0, 0, "", ""));
                                }

                                // ★ イベントの終了後のUOC処理
                                this.UOC_AfterAction(new FxEventArgs(FxLiteral.EVENT_AFTER_YES_NO_YES, 0, 0, "", ""));
                            }
                            else if (((int)FxEnum.SubmitMode.YesNo_No).ToString() == this.SubmitFlag.Value)
                            {
                                // ★ イベントの開始前のUOC処理
                                this.UOC_PreAction(new FxEventArgs(FxLiteral.EVENT_AFTER_YES_NO_NO, 0, 0, "", ""));

                                // ★：「YES」・「NO」メッセージ・ダイアログの「NO」が押された場合。

                                if (this.ButtonHistoryRecorder) // 2008/11/28---ボタン履歴記録機能が有効な場合のみ実行する（追加）。
                                {
                                    // 引数：親画面で押したボタンの情報
                                    this.UOC_YesNoDialog_No_Click((FxEventArgs)this.Buttonhistory_Stack.Peek());
                                }
                                else
                                {
                                    // 引数：ダミーのボタン情報
                                    this.UOC_YesNoDialog_No_Click(
                                        new FxEventArgs(FxLiteral.VALUE_STR_DUMMY_STRING, 0, 0, "", ""));
                                }

                                // ★ イベントの終了後のUOC処理
                                this.UOC_AfterAction(new FxEventArgs(FxLiteral.EVENT_AFTER_YES_NO_NO, 0, 0, "", ""));
                            }
                            else if (((int)FxEnum.SubmitMode.Modal).ToString() == this.SubmitFlag.Value)
                            {
                                // ★ イベントの開始前のUOC処理
                                this.UOC_PreAction(new FxEventArgs(FxLiteral.EVENT_AFTER_MODAL_DIALOG, 0, 0, "", ""));

                                // 親画面で押したボタンの情報
                                FxEventArgs parentFxEventArgs;

                                //子画面で押されたボタンの情報
                                FxEventArgs childFxEventArgs;

                                try
                                {
                                    if (this.ButtonHistoryRecorder) // 2008/11/28---ボタン履歴記録機能が有効な場合のみ実行する（追加）。
                                    {
                                        // POP（Nullチェックしない）
                                        childFxEventArgs = (FxEventArgs)this.Buttonhistory_Stack.Pop();
                                    }
                                    else
                                    {
                                        // ダミーのボタン情報
                                        childFxEventArgs = new FxEventArgs(FxLiteral.VALUE_STR_DUMMY_STRING, 0, 0, "", "");
                                    }

                                    if (this.ButtonHistoryRecorder) // 2008/11/28---ボタン履歴記録機能が有効な場合のみ実行する（追加）。
                                    {
                                        // 親画面で押したボタンの情報
                                        parentFxEventArgs = (FxEventArgs)this.Buttonhistory_Stack.Peek();
                                    }
                                    else
                                    {
                                        // ダミーのボタン情報
                                        parentFxEventArgs = new FxEventArgs(FxLiteral.VALUE_STR_DUMMY_STRING, 0, 0, "", "");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // 子画面でかならずプッシュされるが、異常な状態に陥った場合NullReferenceが発生する。
                                    // エラー発生時や、親画面の後処理でファイルダウンロードのHTTPレスポンスを返した場合など。
                                    throw new FrameworkException(
                                        FrameworkExceptionMessage.DIALOG_AFTER_PROCESSING_STATUS_ERROR[0],
                                        String.Format(FrameworkExceptionMessage.DIALOG_AFTER_PROCESSING_STATUS_ERROR[1], ex.Message));
                                }

                                // ★ 業務モーダル・ダイアログの後処理を実行する。
                                this.UOC_ModalDialog_End(parentFxEventArgs, childFxEventArgs);

                                // ★ イベントの終了後のUOC処理
                                this.UOC_AfterAction(new FxEventArgs(FxLiteral.EVENT_AFTER_MODAL_DIALOG, 0, 0, "", ""));
                            }
                        }

                        #endregion
                    }
                }
            }
            catch (BusinessApplicationException baEx)
            {
                // アプリケーション例外発生時の処理は派生クラスに記述する。
                this.UOC_ABEND(baEx, new FxEventArgs(FxLiteral.EVENT_PAGE_LOAD, 0, 0, "", ""));

                // アプリケーション例外はリスローしない。
            }
            catch (BusinessSystemException bsEx)
            {
                // システム例外発生時の処理は派生クラスに記述する。
                this.UOC_ABEND(bsEx, new FxEventArgs(FxLiteral.EVENT_PAGE_LOAD, 0, 0, "", ""));

                // システム例外はリスローする。
                throw;
            }
            catch (System.Threading.ThreadAbortException taEx)
            {
                // スレッド中断エラーの場合は何もしない
                Exception ex = taEx; // ← 警告を出さないため
            }
            catch (Exception ex)
            {
                // 一般的な例外発生時の処理は派生クラスに記述する
                this.UOC_ABEND(ex, new FxEventArgs(FxLiteral.EVENT_PAGE_LOAD, 0, 0, "", ""));

                // 一般的な例外はリスローする。
                throw;
            }
            finally
            {
                // Hidden（submitFlag）のnullチェック
                if (this.SubmitFlag == null)
                {
                    // nullの場合
                }
                else
                {
                    // submitフラグを初期化
                    SubmitFlag.Value = "0";
                }

                // Finally節のUOCメソッド #z-この行
                this.UOC_Finally(new FxEventArgs(FxLiteral.EVENT_PAGE_LOAD, 0, 0, "", ""));

                //// isNoSessionフラグ
                //if (isNoSession) // 認証情報をSessionに格納するパターンでうまく行かないため、コメントアウト
                //{
                //    // Set-Cookie HTTPヘッダをレスポンス
                //    Response.Cookies.Set(FxCmnFunction.DeleteCookieForSessionTimeoutDetection());
                //    // セッションを消去
                //    Session.Abandon();
                //}
            }
        }

        /// <summary>セッションを消去</summary>
        /// <remarks>併せてSessionタイムアウト検出用Cookieを消去</remarks>
        protected void FxSessionAbandon()
        {
            // Set-Cookie HTTPヘッダをレスポンス
            Response.Cookies.Set(FxCmnFunction.DeleteCookieForSessionTimeoutDetection());
            // セッションを消去
            Session.Abandon();
        }

        #region GUIDキューを取得 ～ 再構築

        /// <summary>GUIDキューを取得</summary>
        /// <param name="guidQueue">GUIDキュー</param>
        /// <param name="sessionIndexOfGuidQueue">セッションのインデックス</param>
        /// <param name="maxQueueLength">GUIDキューの最大長</param>
        private void GetGuidQueue(out Queue<string> guidQueue, string sessionIndexOfGuidQueue, int maxQueueLength)
        {
            // GUIDキューの初期化
            if (Session[sessionIndexOfGuidQueue] == null)
            {
                // 新規生成（キューの最大長を指定する）
                guidQueue = new Queue<string>(maxQueueLength);

                // Sessionに設定
                Session[sessionIndexOfGuidQueue] = guidQueue;
            }
            else
            {
                // Session領域から取得
                guidQueue = (Queue<string>)Session[sessionIndexOfGuidQueue];
            }
        }

        /// <summary>GUIDキューを再構築</summary>
        /// <param name="guid">GUID（検索・更新兼用）</param>
        /// <param name="guidQueue">GUIDキュー</param>
        /// <param name="sessionIndexOfGuidQueue">セッションのインデックス</param>
        /// <param name="maxQueueLength">GUIDキューの最大長</param>
        private void RestructureGuidQueue(string guid,
            out Queue<string> guidQueue, string sessionIndexOfGuidQueue, int maxQueueLength)
        {
            // キューを初期化する。
            this.GetGuidQueue(out guidQueue, sessionIndexOfGuidQueue, maxQueueLength);

            // ☆ Sessionに再設定する必要がある
            //    （作り直しでインスタンスが変わったため）
            Session[sessionIndexOfGuidQueue]
                = FxCmnFunction.RestructuringLRUQueue1(guidQueue, guid, guid, maxQueueLength);
        }

        /// <summary>GUIDキューを再構築（更新機能付き）</summary>
        /// <param name="oldGuid">GUID（検索用）</param>
        /// <param name="newGuid">GUID（更新用）</param>
        /// <param name="guidQueue">GUIDキュー</param>
        /// <param name="sessionIndexOfGuidQueue">セッションのインデックス</param>
        /// <param name="maxQueueLength">GUIDキューの最大長</param>
        private void RestructureGuidQueue(string oldGuid, string newGuid,
            out Queue<string> guidQueue, string sessionIndexOfGuidQueue, int maxQueueLength)
        {
            // キューを初期化する。
            this.GetGuidQueue(out guidQueue, sessionIndexOfGuidQueue, maxQueueLength);

            // ☆ Sessionに再設定する必要がある
            //    （作り直しでインスタンスが変わったため）
            Session[sessionIndexOfGuidQueue]
                = FxCmnFunction.RestructuringLRUQueue1(guidQueue, oldGuid, newGuid, maxQueueLength);
        }

        #endregion

        #region GUIDキューを使用して、セッションを自動削除

        /// <summary>セッションの自動削除</summary>
        /// <param name="guid">GUID</param>
        /// <param name="guidQueue">GUIDキュー</param>
        /// <param name="sessionIndexOfGuidQueue">セッションのインデックス</param>
        /// <param name="maxQueueLength">GUIDキューの最大長</param>
        private void DeleteSessionWithGuidQueueAutomatically(string guid, Queue<string> guidQueue, string sessionIndexOfGuidQueue, int maxQueueLength)
        {
            // 初期化
            if (Session[sessionIndexOfGuidQueue] == null)
            {
                // 新規生成（キューの最大長を指定する）
                guidQueue = new Queue<string>(maxQueueLength);

                // Sessionに設定
                Session[sessionIndexOfGuidQueue] = guidQueue;
            }
            else
            {
                // Session領域から取得
                guidQueue = (Queue<string>)Session[sessionIndexOfGuidQueue];
            }

            // キューの最大長を超える場合は、デキューする
            if (maxQueueLength <= guidQueue.Count)
            {
                // ウィンドウGUIDをデキュー
                string tempGuid = (string)guidQueue.Dequeue();

                List<string> aryTemp = new List<string>();

                // 削除対象のキーを収集する。
                foreach (string key in Session.Keys)
                {
                    if (key.IndexOf(tempGuid) == -1)
                    {
                        // 削除対象でない。
                    }
                    else
                    {
                        // 削除対象である。
                        aryTemp.Add(key);
                    }
                }

                // 指定のキーを削除する。
                foreach (string key in aryTemp)
                {
                    Session.Remove(key);
                }
            }

            // 新規ウィンドウGUIDをエンキューする
            guidQueue.Enqueue(guid);
        }

        #endregion

        // 2009/07/31-end

        #endregion

        #region   集約イベント ハンドラ

        #region クリック イベントに対応した集約イベント ハンドラ

        /// <summary>
        /// Button、LinkButtonのクリック・イベントに対応した集約イベント ハンドラ
        /// ※ イベント・ハンドラ自体は、Button、LinkButtonを区別しない。
        /// </summary>
        protected void Button_Click(object sender, EventArgs e)
        {
            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID,
                0, 0, "",
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                FxLiteral.UOC_METHOD_FOOTER_CLICK));

            // イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs);
        }

        /// <summary>
        /// ImageButtonのクリック・イベントに対応した集約イベント・ハンドラ
        /// </summary>
        protected void ImageButton_Click(object sender, ImageClickEventArgs e)
        {
            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID,
                e.X, e.Y, "",
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                FxLiteral.UOC_METHOD_FOOTER_CLICK));

            // イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs);
        }

        /// <summary>
        /// ImageMapのクリック・イベントに対応した集約イベント・ハンドラ
        /// </summary>
        protected void ImageMap_Click(object sender, ImageMapEventArgs e)
        {
            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID + "_" + e.PostBackValue,
                0, 0, e.PostBackValue,
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                FxLiteral.UOC_METHOD_FOOTER_CLICK));

            // イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs);
        }

        #endregion

        #region その他イベントに対応した集約イベント ハンドラ

        /// <summary>
        /// DropDownList、ListBox、GridView,RadioButtonList,CheckBoxList（など）の
        /// SelectedIndexChangedイベントに対応した集約イベント ハンドラ
        /// ※ イベント・ハンドラ自体は、コントロールを区別しない。
        /// </summary>
        protected void List_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region PostBackValueに一覧のインデックスを格納。

            string postBackValue = "";

            // ワーク
            DropDownList ddl = null;
            ListBox lbx = null;
            RadioButtonList rbl = null;
            CheckBoxList cbl = null;
            object namingContainer = null;

            // 型を識別し、NamingContainerを取得
            if (sender is DropDownList)
            {
                ddl = (DropDownList)sender;
                namingContainer = ddl.NamingContainer;
            }
            else if (sender is ListBox)
            {
                lbx = (ListBox)sender;
                namingContainer = lbx.NamingContainer;
            }
            else if (sender is RadioButtonList)
            {
                rbl = (RadioButtonList)sender;
                namingContainer = rbl.NamingContainer;
            }
            else if (sender is CheckBoxList)
            {
                cbl = (CheckBoxList)sender;
                namingContainer = cbl.NamingContainer;
            }

            // NamingContainerを識別し、PostBackValueを取得
            if (namingContainer != null)
            {
                if (namingContainer is RepeaterItem)
                {
                    RepeaterItem ri = (RepeaterItem)namingContainer;
                    postBackValue = ri.ItemIndex.ToString();
                }
                else if (namingContainer is GridViewRow)
                {
                    GridViewRow gvr = (GridViewRow)namingContainer;
                    postBackValue = gvr.RowIndex.ToString();
                }
                else if (namingContainer is ListViewDataItem)
                {
                    ListViewDataItem lvDataItem = (ListViewDataItem)namingContainer;
                    postBackValue = lvDataItem.DataItemIndex.ToString();
                }
            }

            #endregion

            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID,
                0, 0, postBackValue,
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                    FxLiteral.UOC_METHOD_FOOTER_SELECTED_INDEX_CHANGED));

            // クリック イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs);
        }

        /// <summary>
        /// RadioButton（など）のCheckedChangedイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void Check_CheckedChanged(object sender, System.EventArgs e)
        {
            #region PostBackValueに一覧のインデックスを格納。

            string postBackValue = "";

            // ワーク
            CheckBox cbx = null;
            RadioButton rbn = null;
            object namingContainer = null;

            // 型を識別し、NamingContainerを取得
            if (sender is CheckBox)
            {
                cbx = (CheckBox)sender;
                namingContainer = cbx.NamingContainer;
            }
            else if (sender is RadioButton)
            {
                rbn = (RadioButton)sender;
                namingContainer = rbn.NamingContainer;
            }

            // NamingContainerを識別し、PostBackValueを取得
            if (namingContainer != null)
            {
                if (namingContainer is RepeaterItem)
                {
                    RepeaterItem ri = (RepeaterItem)namingContainer;
                    postBackValue = ri.ItemIndex.ToString();
                }
                else if (namingContainer is GridViewRow)
                {
                    GridViewRow gvr = (GridViewRow)namingContainer;
                    postBackValue = gvr.RowIndex.ToString();
                }
            }

            #endregion

            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID,
                0, 0, postBackValue,
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                    FxLiteral.UOC_METHOD_FOOTER_CHECKED_CHANGED));

            // クリック イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs);
        }

        #region 一覧系

        /// <summary>
        /// RepeaterのItemCommandイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void Repeater_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID,
                ((System.Web.UI.Control)(e.CommandSource)).ID,
                0, 0, e.CommandName, // ★ CommandNameがPostBackValue
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                    FxLiteral.UOC_METHOD_FOOTER_ITEM_COMMAND));

            // クリック イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs);
        }

        #region GRIDVIEW

        /// <summary>
        /// GridViewのRowCommandイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void GridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // ※ sender = e.CommandSource

            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID, e.CommandName,
                0, 0, e.CommandArgument.ToString(), // ★ CommandArgumentがPostBackValue
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                    FxLiteral.UOC_METHOD_FOOTER_ROW_COMMAND));

            // クリック イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs);
        }

        /// <summary>
        /// GridViewのRowUpdatingイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void GridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID, "RowUpdating",
                0, 0, e.RowIndex.ToString(), // ★ RowIndexがPostBackValue
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                    FxLiteral.UOC_METHOD_FOOTER_ROW_UPDATING));

            // クリック イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs, e);
        }

        /// <summary>
        /// GridViewのRowDeletingイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void GridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID, "RowDeleting",
                0, 0, e.RowIndex.ToString(), // ★ RowIndexがPostBackValue
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                    FxLiteral.UOC_METHOD_FOOTER_ROW_DELETING));

            // クリック イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs, e);
        }

        /// <summary>
        /// GridViewのPageIndexChangingイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void GridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID, "PageIndexChanging",
                0, 0, e.NewPageIndex.ToString(), // ★ NewPageIndexがPostBackValue
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                    FxLiteral.UOC_METHOD_FOOTER_PAGE_INDEX_CHANGING));

            // クリック イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs, e);
        }

        /// <summary>
        /// GridViewのSortingイベントに対応した集約イベント ハンドラ
        /// </summary>
        protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID,
                ((SortDirection)(e.SortDirection)).ToString(),
                0, 0, e.SortExpression, // ★ SortExpressionがPostBackValue
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                    FxLiteral.UOC_METHOD_FOOTER_SORTING));

            // クリック イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs, e);
        }

        #endregion

        #region ListView
        /// <summary>
        /// ListViewのItemEditing event handler method
        /// </summary>
        protected void ListView_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID, "ItemDeleting",
                0, 0, e.ItemIndex.ToString(), // ★ RowIndexがPostBackValue
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                    FxLiteral.UOC_METHOD_FOOTER_LISTVIEW_ROW_DELETING));

            // クリック イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs, e);
        }

        #region ListView
        /// <summary>
        /// ListViewのItemCommand event handler method
        /// </summary>
        protected void ListView_OnItemCommand(object sender, ListViewCommandEventArgs e)
        {
            // ItemCommand method
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID, "OnItemCommand",
                0, 0, e.CommandArgument.ToString(), // ★ RowIndexがPostBackValue
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                    FxLiteral.UOC_METHOD_FOOTER_LISTVIEW_ROW_ITEMCOMMAND));

            // クリック イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs, e);
        }
        /// <summary>
        /// Listview Paging  event handler method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ListView_PagePropertiesChanged(object sender, EventArgs e)
        {
            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID, "PagePropertiesChanged",
                0, 0, e.ToString(), // ★ RowIndexがPostBackValue
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                    FxLiteral.UOC_METHOD_FOOTER_LISTVIEW_PAGE_PROPERTIES_CHANGED));

            // クリック イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs, e);
        }

        /// <summary>
        /// Listview Item Updating  event handler method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ListView_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID, "ItemUpdating",
                0, 0, e.ToString(), // ★ RowIndexがPostBackValue
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                    FxLiteral.UOC_METHOD_FOOTER_LISTVIEW_ROW_UPDATING));

            // クリック イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs, e);
        }

        /// <summary>
        /// ListView Sorting event handler method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ListView_Sorting(object sender, ListViewSortEventArgs e)
        {
            // イベント ハンドラの共通引数の作成
            FxEventArgs fxEventArgs = new FxEventArgs(
                ((System.Web.UI.Control)(sender)).ID, "Sorting",
                0, 0, e.ToString(), // ★ RowIndexがPostBackValue
                this.GetMethodName(((System.Web.UI.Control)(sender)).ID,
                    FxLiteral.UOC_METHOD_FOOTER_LISTVIEW_ROW_SORTING));

            // クリック イベント処理の共通メソッド
            this.CMN_Event_Handler(fxEventArgs, e);

        }

        #endregion

        #endregion

        #endregion

        #region メソッド名生成

        /// <summary>マスタ ページ名を記憶しておくワーク</summary>
        private string MasterPageFileNoExImplementingMethod = "";

        /// <summary>ユーザ コントロール名を記憶しておくワーク</summary>
        private string UserControlNameImplementingMethod = "";

        /// <summary>
        /// レイトバインドする際に使用するメソッド名を生成する
        /// </summary>
        /// <param name="ControlID">コントロール名</param>
        /// <param name="EventName">イベント名</param>
        /// <returns>レイトバインドする際に使用するメソッド名</returns>
        /// <remarks>派生の画面コード親クラス２から利用する。</remarks>
        protected string GetMethodName(string ControlID, string EventName)
        {
            if (this.GetMasterWebControl(ControlID, out this.MasterPageFileNoExImplementingMethod) == null)
            {
                // マスタ ページ上でない場合

                if (this.GetUCWebControl(ControlID, out this.UserControlNameImplementingMethod) == null)
                {
                    // ユーザ コントロール上でない場合

                    // => コンテンツ ページ上の場合

                    // 以下のメソッド名でレイトバインド
                    return FxLiteral.UOC_METHOD_HEADER + ControlID + "_" + EventName;
                }
                else
                {
                    // ユーザ コントロール上の場合

                    // 以下のメソッド名でレイトバインド
                    // ユーザ コントロールのヘッダ有り
                    return FxLiteral.UOC_METHOD_HEADER
                        + this.UserControlNameImplementingMethod
                        + "_" + ControlID + "_" + EventName;
                }
            }
            else
            {
                // マスタ ページ上の場合

                // 以下のメソッド名でレイトバインド
                // マスタ ページのヘッダ有り
                return FxLiteral.UOC_METHOD_HEADER
                    + this.MasterPageFileNoExImplementingMethod
                    + "_" + ControlID + "_" + EventName;
            }
        }

        #endregion

        #region イベント処理の共通メソッド

        /// <summary>イベント処理の共通メソッド</summary>
        /// <param name="fxEventArgs">イベント ハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２から利用する。</remarks>
        protected void CMN_Event_Handler(FxEventArgs fxEventArgs)
        {
            this.CMN_Event_Handler(fxEventArgs, null);
        }

        /// <summary>イベント処理の共通メソッド</summary>
        /// <param name="fxEventArgs">イベント ハンドラの共通引数</param>
        /// <param name="eventArgs">オリジナルのイベント引数</param>
        /// <remarks>派生の画面コード親クラス２から利用する。</remarks>
        protected void CMN_Event_Handler(FxEventArgs fxEventArgs, EventArgs eventArgs)
        {
            // UOCメソッドの戻り値、urlを受ける。
            string url = "";

            // オリジナルのスタックトレース値のクリア
            this.OriginalStackTrace = "";

            try
            {
                /////////////////////////////////////////////////
                // ボタン履歴情報を記録
                /////////////////////////////////////////////////

                if (this.ButtonHistoryRecorder) // 2008/11/28---ボタン履歴記録機能が有効な場合のみ実行する（追加）。
                {
                    lock (this.SessionLock) // 2009/03/13---念のためセッション単位でロック。
                    {
                        // １つポップしてから
                        this.Buttonhistory_Stack.Pop();

                        // ボタンIDをプッシュ
                        this.Buttonhistory_Stack.Push(fxEventArgs);
                    }
                }

                ///////////////////////////////////////////////////////

                // ★ イベントの開始前のUOC処理
                this.UOC_PreAction(fxEventArgs);

                // ★ イベントのUOC処理
                url = this.LateBind(fxEventArgs, eventArgs);

                // ★ イベントの終了後のUOC処理
                this.UOC_AfterAction(fxEventArgs);

                // ★ イベントの終了後の画面遷移 UOC処理
                this.UOC_Screen_Transition(url);

            }
            catch (BusinessApplicationException baEx)
            {
                // アプリケーション例外発生時の処理は派生クラスに記述する。
                this.UOC_ABEND(baEx, fxEventArgs);

                // アプリケーション例外はリスローしない。
            }
            catch (BusinessSystemException bsEx)
            {
                // システム例外発生時の処理は派生クラスに記述する。
                this.UOC_ABEND(bsEx, fxEventArgs);

                // システム例外はリスローする。
                throw;
            }
            catch (System.Threading.ThreadAbortException taEx)
            {
                // スレッド中断エラーの場合は何もしない
                Exception ex = taEx; // ← 警告を出さないため
            }
            catch (Exception ex)
            {
                // 一般的な例外発生時の処理は派生クラスに記述する
                this.UOC_ABEND(ex, fxEventArgs);

                // 一般的な例外はリスローする。
                throw;
            }
            finally
            {
                // Hidden（submitFlag）のnullチェック
                if (this.SubmitFlag == null)
                {
                    // nullの場合
                }
                else
                {
                    // submitフラグを初期化
                    SubmitFlag.Value = "0";
                }

                // Finally節のUOCメソッド #z-この行
                this.UOC_Finally(fxEventArgs);
            }
        }

        #endregion

        #region レイトバインドするメソッド

        /// <summary>レイトバインドするメソッド</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <param name="eventArgs">オリジナルのイベント引数</param>
        /// <returns>画面遷移する場合のURL</returns>
        private string LateBind(FxEventArgs fxEventArgs, EventArgs eventArgs)
        {
            // ワーク変数
            string strTemp;
            string newMethodName;

            // 引数を格納するオブジェクト配列
            object[] parameter = null;

            if (eventArgs == null)
            {
                // オリジナルのイベント引数を含めない。
                parameter = new object[] { fxEventArgs };
            }
            else
            {
                // オリジナルのイベント引数を含める。
                parameter = new object[] { fxEventArgs, eventArgs };
            }

            string url = "";

            try
            {
                // Latebind（2009/08/08、2011/02/08）
                if (Latebind.CheckTypeOfMethodByName(this, fxEventArgs.MethodName))
                {
                    // 本ページ中にメソッドがある。

                    // 本ページに対してレイトバインド。
                    url = (string)Latebind.InvokeMethod_NoErr(this, fxEventArgs.MethodName, parameter);
                }
                else
                {
                    // 本ページ中にメソッドがない。

                    // どこに実装されているか？
                    if (this.MasterPageFileNoExImplementingMethod != "")
                    {
                        // マスタ ページの可能性
                        foreach (MasterPage mp in this.StcMasterPage)
                        {
                            // マスタ ページ名を取得

                            // ここは、他と同じロジック（this.StcMasterPageFileNoExの作成時）
                            //aryTemp = mp.AppRelativeVirtualPath.Split('/');
                            //strTemp = aryTemp[aryTemp.Length - 1].Split('.')[0];
                            strTemp = PubCmnFunction.GetFileNameNoEx(mp.AppRelativeVirtualPath, '/');

                            // 比較してイコールであること。
                            if (strTemp == this.MasterPageFileNoExImplementingMethod)
                            {
                                // メソッドを実装するマスタ ページの参照を取得できた場合、

                                // メソッド名からマスタ ページ名のプレフィックスを削除し、
                                newMethodName = fxEventArgs.MethodName.
                                    Replace(this.MasterPageFileNoExImplementingMethod + "_", "");

                                // マスタ ページに対してレイトバインド。
                                url = (string)Latebind.InvokeMethod_NoErr(mp, newMethodName, parameter);

                                // ・・・fxEventArgs.MethodNameと一致しないが、こういう仕様ということで。
                            }
                        }
                    }
                    else if (this.UserControlNameImplementingMethod != "")
                    {
                        // ユーザ コントロールの可能性
                        foreach (UserControl uc in this.LstUserControl)
                        {
                            // ユーザ コントロール名を取得

                            // ここは、他と同じロジック（this.StcMasterPageFileNoExの作成時）
                            //aryTemp = uc.AppRelativeVirtualPath.Split('/');
                            //strTemp = aryTemp[aryTemp.Length - 1].Split('.')[0];
                            //strTemp = PubCmnFunction.GetFileNameNoEx(uc.AppRelativeVirtualPath, '/');

                            // 比較してイコールであること。
                            if (uc.ID == this.UserControlNameImplementingMethod)
                            {
                                // メソッドを実装するユーザ コントロールの参照を取得できた場合、

                                // メソッド名からユーザ コントロール名のプレフィックスを削除し、
                                newMethodName = fxEventArgs.MethodName.
                                    Replace(this.UserControlNameImplementingMethod + "_", "");

                                // マスタ ページに対してレイトバインド。
                                url = (string)Latebind.InvokeMethod_NoErr(uc, newMethodName, parameter);

                                // ・・・fxEventArgs.MethodNameと一致しないが、こういう仕様ということで。
                            }
                        }
                    }
                }
            }
            catch (System.Reflection.TargetInvocationException rtEx)
            {
                // InnerExceptionのスタックトレースを保存しておく（以下のリスローで消去されるため）。
                this.OriginalStackTrace = rtEx.InnerException.StackTrace;

                // InnerExceptionを投げなおす。
                throw rtEx.InnerException;
            }

            return url;
        }

        #endregion

        #endregion

        #endregion

        #region ユーティリティ メソッド

        #region 画面遷移処理メソッド

        /// <summary>画面遷移制御部品を使用して、画面遷移する。</summary>
        /// <param name="label">画面遷移に使用するラベル</param>
        /// <remarks>派生の画面コード親クラス２から利用する。</remarks>
        protected void ScreenTransition(string label)
        {
            // ウィンドウ別セッション領域に前画面情報を設定する。
            this.SetDataToBrowserWindow(
                                FxHttpSessionIndex.SCREEN_TRANSITION_INFO,
                                this.AppRelativeVirtualPath);

            // 画面遷移
            BaseController.SC.ScreenTransition(
                this.AppRelativeVirtualPath,
                label, this.WindowGuid.Value);
        }

        /// <summary>画面遷移制御部品を使用しないで、Transferで画面遷移する。</summary>
        /// <param name="url">URL</param>
        /// <remarks>派生の画面コード親クラス２から利用する。</remarks>
        protected void FxTransfer(string url)
        {
            // 部品：Transfer
            BaseController.SC.FxTransfer(
                url, "", this.WindowGuid.Value);
        }

        /// <summary>画面遷移制御部品を使用しないで、Redirectで画面遷移する。</summary>
        /// <param name="url">URL</param>
        /// <remarks>派生の画面コード親クラス２から利用する。</remarks>
        protected void FxRedirect(string url)
        {
            // 部品：Redirect
            BaseController.SC.FxRedirect(
                url, "", this.WindowGuid.Value);
        }

        #endregion

        #region ウィンドウ表示メソッド

        #region モーダル画面表示メソッド

        /// <summary>「OK」メッセージ ダイアログを表示する。</summary>
        /// <param name="messegeID">メッセージID</param>
        /// <param name="message">メッセージ</param>
        /// <param name="iconType">アイコンタイプ</param>
        /// <param name="dialogName">ウィンドウ名</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void ShowOKMessageDialog(string messegeID, string message, FxEnum.IconType iconType, string dialogName)
        {
            // オーバーロードしたメソッドを呼び出す。
            this.ShowOKMessageDialog(messegeID, message, iconType, dialogName,
                GetConfigParameter.GetConfigValue(FxLiteral.DEFAULT_FX_DIALOG_STYLE));
        }

        /// <summary>「OK」メッセージ ダイアログを表示する。</summary>
        /// <param name="messegeID">メッセージID</param>
        /// <param name="message">メッセージ</param>
        /// <param name="iconType">アイコンタイプ</param>
        /// <param name="dialogName">ウィンドウ名</param>
        /// <param name="dialogStyle">スタイル</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void ShowOKMessageDialog(string messegeID, string message, FxEnum.IconType iconType, string dialogName, string dialogStyle)
        {
            // メッセージボックスフラグを設定
            this.ChildScreenType.Value = ((int)FxEnum.ChildScreenType.OKMessageDialog).ToString();

            // Hiddenにスタイルを設定する。
            this.FxDialogStyle = (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_FX_DIALOG_STYLE); // 2009/07/21-この行

            // エラー処理
            if (this.FxDialogStyle == null)
            {
                throw new FrameworkException(
                    FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                    String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1],
                        FxLiteral.HIDDEN_FX_DIALOG_STYLE));
            }

            this.FxDialogStyle.Value = dialogStyle;

            // Cookieフラグを設定（バックボタン押下時の不具合対応）
            HttpCookie cookieBackButtonControl =
                new HttpCookie(FxHttpCookieIndex.BACK_BUTTON_CONTROL, FxLiteral.VALUE_STR_TRUE);
            // cookieBackButtonControl.Path = "/"; // パス属性はデフォルトで設定される。
            Response.Cookies.Add(cookieBackButtonControl);

            // URL（QueryStringで画面GUIDを渡す）

            // 「OK」メッセージ ダイアログへのパスを取得
            string okMessageDialogPath =
                GetConfigParameter.GetConfigValue(FxLiteral.OK_MESSAGE_DIALOG_PATH);

            // エラー処理
            if (okMessageDialogPath == null || okMessageDialogPath == "")
            {
                throw new FrameworkException(
                    FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1[0],
                    String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1[1],
                        FxLiteral.OK_MESSAGE_DIALOG_PATH));
            }

            this.ChildScreenUrl.Value = okMessageDialogPath + "?" +
                FxHttpQueryStringIndex.PARENT_SCREEN_GUID + "=" + this.ScreenGuid.Value;

            // セッションにメッセージを設定

            // 「OK」メッセージ ダイアログは、Baseクラスを継承しないため、SetModalModalInterfaceDataは使用しない。
            // 素のSessionを使用する（ただし、ScreenGuidが付くため競合はしない）。
            Session.Add(this.ScreenGuid.Value + FxHttpSessionIndex.MODAL_DIALOG_MESSAGEID, messegeID);
            Session.Add(this.ScreenGuid.Value + FxHttpSessionIndex.MODAL_DIALOG_MESSAGE, message);
            Session.Add(this.ScreenGuid.Value + FxHttpSessionIndex.MODAL_DIALOG_ICONTYPE, iconType);
            Session.Add(this.ScreenGuid.Value + FxHttpSessionIndex.MODAL_DIALOG_NAME, dialogName);
        }

        /// <summary>「Yes」「No」メッセージ ダイアログを表示する。</summary>
        /// <param name="messegeID">メッセージID</param>
        /// <param name="message">メッセージ</param>
        /// <param name="dialogName">ウィンドウ名</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void ShowYesNoMessageDialog(string messegeID, string message, string dialogName)
        {
            // オーバーロードしたメソッドを呼び出す。
            this.ShowYesNoMessageDialog(messegeID, message, dialogName,
                GetConfigParameter.GetConfigValue(FxLiteral.DEFAULT_FX_DIALOG_STYLE));
        }

        /// <summary>「Yes」「No」メッセージ ダイアログを表示する。</summary>
        /// <param name="messegeID">メッセージID</param>
        /// <param name="message">メッセージ</param>
        /// <param name="dialogName">ウィンドウ名</param>
        /// <param name="dialogStyle">スタイル</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void ShowYesNoMessageDialog(string messegeID, string message, string dialogName, string dialogStyle)
        {
            // 子画面タイプを設定
            this.ChildScreenType.Value = ((int)FxEnum.ChildScreenType.YesNoMessageDialog).ToString();

            // Hiddenにスタイルを設定する。
            this.FxDialogStyle = (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_FX_DIALOG_STYLE); // 2009/07/21-この行

            // エラー処理
            if (this.FxDialogStyle == null)
            {
                throw new FrameworkException(
                    FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                    String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1],
                        FxLiteral.HIDDEN_FX_DIALOG_STYLE));
            }

            this.FxDialogStyle.Value = dialogStyle;

            // Cookieフラグを設定（バックボタン押下時の不具合対応）
            HttpCookie cookieBackButtonControl =
                new HttpCookie(FxHttpCookieIndex.BACK_BUTTON_CONTROL, FxLiteral.VALUE_STR_TRUE);
            // cookieBackButtonControl.Path = "/"; // パス属性はデフォルトで設定される。
            Response.Cookies.Add(cookieBackButtonControl);

            // URL（QueryStringで画面GUIDを渡す）

            // 「Yes」「No」メッセージ ダイアログへのパスを取得
            string yesNoMessageDialogPath =
                GetConfigParameter.GetConfigValue(FxLiteral.YES_NO_MESSAGE_DIALOG_PATH);

            // エラー処理
            if (yesNoMessageDialogPath == null || yesNoMessageDialogPath == "")
            {
                throw new FrameworkException(
                    FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1[0],
                    String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1[1],
                        FxLiteral.YES_NO_MESSAGE_DIALOG_PATH));
            }

            this.ChildScreenUrl.Value = yesNoMessageDialogPath + "?" +
                FxHttpQueryStringIndex.PARENT_SCREEN_GUID + "=" + this.ScreenGuid.Value;

            // セッションにメッセージを設定

            // 「Yes」「No」メッセージ ダイアログは、Baseクラスを継承しないため、SetModalModalInterfaceDataは使用しない。
            // 素のSessionを使用する（ただし、ScreenGuidが付くため競合はしない）。
            Session.Add(this.ScreenGuid.Value + FxHttpSessionIndex.MODAL_DIALOG_MESSAGEID, messegeID);
            Session.Add(this.ScreenGuid.Value + FxHttpSessionIndex.MODAL_DIALOG_MESSAGE, message);
            Session.Add(this.ScreenGuid.Value + FxHttpSessionIndex.MODAL_DIALOG_NAME, dialogName);
        }

        /// <summary>業務モーダル画面を表示する。</summary>
        /// <param name="screenURL">画面のURL</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void ShowModalScreen(string screenURL)
        {
            // オーバーロードしたメソッドを呼び出す。
            this.ShowModalScreen(screenURL,
                GetConfigParameter.GetConfigValue(FxLiteral.DEFAULT_BUSINESS_DIALOG_STYLE));
        }

        /// <summary>業務モーダル画面を表示する。</summary>
        /// <param name="screenURL">画面のURL</param>
        /// <param name="dialogStyle">スタイル</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void ShowModalScreen(string screenURL, string dialogStyle)
        {
            // 子画面タイプを設定
            this.ChildScreenType.Value = ((int)FxEnum.ChildScreenType.ModalScreen).ToString();

            // Hiddenにスタイルを設定する。
            this.BusinessDialogStyle =
                (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_BUSINESS_DIALOG_STYLE); // 2009/07/21-この行

            // エラー処理
            if (this.BusinessDialogStyle == null)
            {
                throw new FrameworkException(
                    FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                    String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1],
                        FxLiteral.HIDDEN_BUSINESS_DIALOG_STYLE));
            }

            this.BusinessDialogStyle.Value = dialogStyle;

            // HiddenにDialogFrameのパスを設定する。
            this.DialogFrameUrl =
                (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_DIALOG_FRAME_URL); // 2009/07/21-この行

            // エラー処理
            if (DialogFrameUrl == null)
            {
                throw new FrameworkException(
                    FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                    String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1],
                        FxLiteral.HIDDEN_DIALOG_FRAME_URL));
            }

            // DialogFrameへのパスを取得
            string dialogFramePath = GetConfigParameter.GetConfigValue(FxLiteral.DIALOG_FRAME_PATH);

            // エラー処理
            if (dialogFramePath == null || dialogFramePath == "")
            {
                throw new FrameworkException(
                    FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1[0],
                    String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1[1],
                        FxLiteral.DIALOG_FRAME_PATH));
            }

            this.DialogFrameUrl.Value = dialogFramePath;

            // ---

            // Cookieフラグを設定（バックボタン押下時の不具合対応）
            HttpCookie cookieBackButtonControl =
                new HttpCookie(FxHttpCookieIndex.BACK_BUTTON_CONTROL, FxLiteral.VALUE_STR_TRUE);
            // cookieBackButtonControl.Path = "/"; // パス属性はデフォルトで設定される。
            Response.Cookies.Add(cookieBackButtonControl);

            // 2010/10/13 以下のif-else

            // URL（QueryStringで画面GUID、ウィンドウGUIDを渡す）

            string queryString =
                FxHttpQueryStringIndex.PARENT_SCREEN_GUID + "=" + this.ScreenGuid.Value + "&"
                + FxHttpQueryStringIndex.BROWSER_WINDOW_GUID + "=" + this.WindowGuid.Value;

            if (screenURL.IndexOf('?') == -1)
            {
                // QueryString指定なし
                this.ChildScreenUrl.Value = screenURL + "?" + queryString;
            }
            else
            {
                // QueryString指定あり
                this.ChildScreenUrl.Value = screenURL + "&" + queryString;
            }
        }

        // #y-start
        /// <summary>
        /// クライアント側のJSから業務モーダル画面を
        /// 起動するためのスクリプトを取得する。
        /// </summary>
        /// <param name="screenURL">画面のURL</param>
        /// <returns>業務モーダル画面を起動するスクリプト</returns>
        protected string GetScriptToShowModalScreen(string screenURL)
        {
            // オーバーロードしたメソッドを呼び出す。
            return this.GetScriptToShowModalScreen(screenURL,
                GetConfigParameter.GetConfigValue(FxLiteral.DEFAULT_BUSINESS_DIALOG_STYLE));
        }

        /// <summary>
        /// クライアント側のJSから業務モーダル画面を
        /// 起動するためのスクリプトを取得する。
        /// </summary>
        /// <param name="screenURL">画面のURL</param>
        /// <param name="dialogStyle">スタイル</param>
        /// <returns>業務モーダル画面を起動するスクリプト</returns>
        protected string GetScriptToShowModalScreen(string screenURL, string dialogStyle)
        {
            // HiddenにDialogFrameのパスを設定する。
            this.DialogFrameUrl =
                (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_DIALOG_FRAME_URL); // 2009/07/21-この行

            // エラー処理
            if (DialogFrameUrl == null)
            {
                throw new FrameworkException(
                    FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                    String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1],
                        FxLiteral.HIDDEN_DIALOG_FRAME_URL));
            }

            // DialogFrameへのパスを取得
            string dialogFramePath = GetConfigParameter.GetConfigValue(FxLiteral.DIALOG_FRAME_PATH);

            // エラー処理
            if (dialogFramePath == null || dialogFramePath == "")
            {
                throw new FrameworkException(
                    FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1[0],
                    String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1[1],
                        FxLiteral.DIALOG_FRAME_PATH));
            }

            this.DialogFrameUrl.Value = dialogFramePath;

            // ---

            // 2010/10/13 以下のif-else
            // URL（QueryStringで画面GUID、ウィンドウGUIDを渡す）

            string queryString =
                            FxHttpQueryStringIndex.PARENT_SCREEN_GUID + "=" + this.ScreenGuid.Value + "&"
                            + FxHttpQueryStringIndex.BROWSER_WINDOW_GUID + "=" + this.WindowGuid.Value;

            // 2009/08/04 以下のif-else
            // 諸事情により、JavaScript関数の引数の順序を変更した（url→styleの順）。

            // スクリプト（注意：リテラル化不可能）
            string script = "Fx_ShowModalScreen('{0}', '{1}')";
            // ※ この部分は、シングル クォート

            if (screenURL.IndexOf('?') == -1)
            {
                // QueryString指定なし
                return string.Format(
                    script,
                    screenURL + "?" + queryString,
                    dialogStyle);
            }
            else
            {
                // QueryString指定あり
                return string.Format(
                    script,
                    screenURL + "&" + queryString,
                    dialogStyle);
            }
        }
        // #y-end

        /// <summary>
        /// 現在の画面（ダイアログ）を閉じる。
        /// ダイアログから親画面に戻った際に、
        /// ポストバックを実行し後処理を実行する。
        /// </summary>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void CloseModalScreen()
        {
            // 閉じるフラグを設定
            this.CloseFlag.Value = ((int)FxEnum.CloseMode.Normal).ToString();
        }

        /// <summary>
        /// 現在の画面（ダイアログ）を閉じる。
        /// ダイアログから親画面に戻った際に、
        /// ポストバックをせず後処理を実行しない。
        /// </summary>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void CloseModalScreen_NoPostback()
        {
            // 閉じるフラグを設定
            this.CloseFlag.Value = ((int)FxEnum.CloseMode.NoPostback).ToString();

            if (this.ButtonHistoryRecorder) // 2008/11/28---ボタン履歴記録機能が有効な場合のみ実行する（追加）。
            {
                lock (this.SessionLock) // 2009/03/13---念のためセッション単位でロック。
                {
                    if (2 <= this.Buttonhistory_Stack.Count) // 2009/08/12-この行
                    {
                        // ボタン履歴が、２画面分以上存在する。
                        // ボタン履歴をPOP
                        this.Buttonhistory_Stack.Pop();
                    }
                    else
                    {
                        // ボタン履歴が、２画面分以上存在しない。
                        throw new FrameworkException(
                            FrameworkExceptionMessage.DIALOG_CLOSING_STATUS_ERROR[0],
                            String.Format(FrameworkExceptionMessage.DIALOG_CLOSING_STATUS_ERROR[1],
                                FrameworkExceptionMessage.DIALOG_CLOSING_STATUS_ERROR1));
                    }
                }
            }
        }

        /// <summary>
        /// 現在の画面（ダイアログ）を閉じる。
        /// ダイアログの場合、ルートの親画面まで、全てのダイアログ画面を閉じる。
        /// ルートの親画面に戻った際に、ポストバックを実行し後処理を実行する。
        /// </summary>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void CloseModalScreen_WithAllParent()
        {
            // 閉じるフラグを設定
            this.CloseFlag.Value = ((int)FxEnum.CloseMode.WithAllParent).ToString();

            FxEventArgs childFxEventArgs;

            if (this.ButtonHistoryRecorder) // 2008/11/28---ボタン履歴記録機能が有効な場合のみ実行する（追加）。
            {
                lock (this.SessionLock) // 2009/03/13---念のためセッション単位でロック。
                {
                    if (2 <= this.Buttonhistory_Stack.Count) // 2009/08/12-この行
                    {
                        // ボタン履歴が、２画面分以上存在する。
                        // ボタン履歴をPOP
                        childFxEventArgs = (FxEventArgs)this.Buttonhistory_Stack.Pop();
                    }
                    else
                    {
                        // ボタン履歴が、２画面分以上存在しない。
                        throw new FrameworkException(
                            FrameworkExceptionMessage.DIALOG_CLOSING_STATUS_ERROR[0],
                            String.Format(FrameworkExceptionMessage.DIALOG_CLOSING_STATUS_ERROR[1],
                                FrameworkExceptionMessage.DIALOG_CLOSING_STATUS_ERROR2));
                    }

                    // ルート画面のボタン履歴が現れるまで、
                    while (1 < this.Buttonhistory_Stack.Count) // 2009/08/12-この行
                    {
                        // ボタン履歴をPOP
                        this.Buttonhistory_Stack.Pop();
                    }

                    // ボタン履歴を、ルート画面→現画面と詰める。
                    this.Buttonhistory_Stack.Push(childFxEventArgs);
                }
            }
        }

        #endregion

        #region モードレス画面表示メソッド

        /// <summary>
        /// 業務モードレス画面を表示する。
        /// </summary>
        /// <param name="screenURL">画面のURL</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void ShowNormalScreen(string screenURL)
        {
            // オーバーロードしたメソッドを呼び出す。
            this.ShowNormalScreen(screenURL,
                GetConfigParameter.GetConfigValue(FxLiteral.DEFAULT_NORMAL_SCREEN_STYLE), "");
        }

        /// <summary>
        /// 業務モードレス画面を表示する。
        /// </summary>
        /// <param name="screenURL">画面のURL</param>
        /// <param name="screenStyle">スタイル</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void ShowNormalScreen(string screenURL, string screenStyle)
        {
            // オーバーロードしたメソッドを呼び出す。
            this.ShowNormalScreen(screenURL, screenStyle, "");
        }

        ///// <summary>
        ///// 業務モードレス画面を表示する。
        ///// </summary>
        ///// <param name="screenURL">画面のURL</param>
        ///// <param name="screenTarget">ターゲット</param>
        ///// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        //protected void ShowNormalScreen(string screenURL, string screenTarget)
        //{
        //    // オーバーロードしたメソッドを呼び出す。
        //    this.ShowNormalScreen(screenURL, "", screenTarget);
        //}

        /// <summary>
        /// 業務モードレス画面を表示する。
        /// </summary>
        /// <param name="screenURL">画面のURL</param>
        /// <param name="screenStyle">スタイル</param>
        /// <param name="screenTarget">ターゲット</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void ShowNormalScreen(string screenURL, string screenStyle, string screenTarget)
        {
            // 開く子画面のURLを設定
            this.ChildScreenUrl.Value = screenURL;

            // 開く子画面のタイプを設定
            this.ChildScreenType.Value = ((int)FxEnum.ChildScreenType.NormalScreen).ToString();

            // 業務モードレス画面のスタイルを設定する。
            this.NormalScreenStyle = (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_NORMAL_SCREEN_STYLE); // 2009/07/21-この行

            // エラー処理
            if (this.NormalScreenStyle == null)
            {
                throw new FrameworkException(
                    FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                    String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1],
                        FxLiteral.HIDDEN_NORMAL_SCREEN_STYLE));
            }

            this.NormalScreenStyle.Value = screenStyle;

            // 業務モードレス画面のターゲットを設定する。
            this.NormalScreenTarget =
                (HiddenField)this.RootMasterPage.FindControl(FxLiteral.HIDDEN_NORMAL_SCREEN_TARGET);

            // エラー処理
            if (this.NormalScreenTarget == null)
            {
                throw new FrameworkException(
                    FrameworkExceptionMessage.NO_FX_HIDDEN[0],
                    String.Format(FrameworkExceptionMessage.NO_FX_HIDDEN[1],
                        FxLiteral.HIDDEN_NORMAL_SCREEN_TARGET));
            }

            this.NormalScreenTarget.Value = screenTarget;

            // Cookieフラグを設定（バックボタン押下時の不具合対応）
            HttpCookie cookieBackButtonControl =
                new HttpCookie(FxHttpCookieIndex.BACK_BUTTON_CONTROL, FxLiteral.VALUE_STR_TRUE);
            // cookieBackButtonControl.Path = "/"; // パス属性はデフォルトで設定される。
            Response.Cookies.Add(cookieBackButtonControl);
        }

        /// <summary>
        /// クライアント側のJSから業務モードレス画面を
        /// 起動するためのスクリプトを取得する。
        /// </summary>
        /// <param name="screenURL">画面のURL</param>
        /// <returns>業務モードレス画面を起動するスクリプト</returns>
        protected string GetScriptToShowNormalScreen(string screenURL)
        {
            // オーバーロードしたメソッドを呼び出す。
            return this.GetScriptToShowNormalScreen(screenURL,
                GetConfigParameter.GetConfigValue(FxLiteral.DEFAULT_NORMAL_SCREEN_STYLE), "");
        }

        /// <summary>
        /// クライアント側のJSから業務モードレス画面を
        /// 起動するためのスクリプトを取得する。
        /// </summary>
        /// <param name="screenURL">画面のURL</param>
        /// <param name="screenStyle">スタイル</param>
        /// <returns>業務モードレス画面を起動するスクリプト</returns>
        protected string GetScriptToShowNormalScreen(string screenURL, string screenStyle)
        {
            // オーバーロードしたメソッドを呼び出す。
            return this.GetScriptToShowNormalScreen(screenURL, screenStyle, "");
        }

        /// <summary>
        /// クライアント側のJSから業務モードレス画面を
        /// 起動するためのスクリプトを取得する。
        /// </summary>
        /// <param name="screenURL">画面のURL</param>
        /// <param name="screenStyle">スタイル</param>
        /// <param name="screenTarget">ターゲット</param>
        /// <returns>業務モードレス画面を起動するスクリプト</returns>
        protected string GetScriptToShowNormalScreen(string screenURL, string screenStyle, string screenTarget)
        {
            // スクリプト（注意：リテラル化不可能）
            return "window.open("
                + "'" + screenURL + "', "
                + "'" + screenTarget + "', "
                + "'" + screenStyle + "')";
            // ※ この部分は、シングル クォート
        }

        #endregion

        #endregion

        #region ｘｘ別セッション利用のメソッド

        #region ダイアログ間の情報受け渡し

        /// <summary>ダイアログ間の受け渡しデータの設定</summary>
        /// <param name="name">キー名</param>
        /// <param name="value">値</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void SetDataToModalInterface(string name, object value)
        {
            FxSessionUtil.Add(
                FxHttpSessionIndex.SESSION_SCOPE_OF_PARENT_SCREEN_BY_GUID,
                this.ScreenGuid.Value, name, value);
        }

        /// <summary>ダイアログ間の受け渡しデータの取得</summary>
        /// <param name="name">キー名</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected object GetDataFromModalInterface(string name)
        {
            return FxSessionUtil.Item(
                FxHttpSessionIndex.SESSION_SCOPE_OF_PARENT_SCREEN_BY_GUID,
                this.ScreenGuid.Value, name);
        }

        /// <summary>ダイアログ間の受け渡しデータの削除（キー毎）</summary>
        /// <param name="name">キー名</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void DeleteDataFromModalInterface(string name)
        {
            FxSessionUtil.Remove(
                FxHttpSessionIndex.SESSION_SCOPE_OF_PARENT_SCREEN_BY_GUID,
                this.ScreenGuid.Value, name);
        }

        /// <summary>ダイアログ間の受け渡しデータの削除（全て）</summary>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void DeleteDataFromModalInterface()
        {
            FxSessionUtil.Remove(
                FxHttpSessionIndex.SESSION_SCOPE_OF_PARENT_SCREEN_BY_GUID,
                this.ScreenGuid.Value);
        }

        #endregion

        #region 複数ブラウザ ウィンドウ時の画面間の情報受け渡し

        // 2009/03/13---機能追加（画面遷移制御機能など）（ここから）

        /// <summary>複数ブラウザ ウィンドウ時の画面間の受け渡しデータの設定</summary>
        /// <param name="name">キー名</param>
        /// <param name="value">値</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void SetDataToBrowserWindow(string name, object value)
        {
            FxSessionUtil.Add(
                FxHttpSessionIndex.SESSION_SCOPE_OF_BROWSER_WINDOW_BY_GUID,
                this.WindowGuid.Value, name, value);
        }

        /// <summary>複数ブラウザ ウィンドウ時の画面間の受け渡しデータの取得</summary>
        /// <param name="name">キー名</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected object GetDataFromBrowserWindow(string name)
        {
            return FxSessionUtil.Item(
                FxHttpSessionIndex.SESSION_SCOPE_OF_BROWSER_WINDOW_BY_GUID,
                this.WindowGuid.Value, name);
        }

        /// <summary>複数ブラウザ ウィンドウ時の画面間の受け渡しデータの削除（キー毎）</summary>
        /// <param name="name">キー名</param>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void DeleteDataFromBrowserWindow(string name)
        {
            FxSessionUtil.Remove(
                FxHttpSessionIndex.SESSION_SCOPE_OF_BROWSER_WINDOW_BY_GUID,
                this.WindowGuid.Value, name);
        }

        /// <summary>複数ブラウザ ウィンドウ時の画面間の受け渡しデータの削除（全て）</summary>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected void DeleteDataFromBrowserWindow()
        {
            FxSessionUtil.Remove(
                FxHttpSessionIndex.SESSION_SCOPE_OF_BROWSER_WINDOW_BY_GUID,
                this.WindowGuid.Value);
        }

        // 2009/03/13---機能追加（画面遷移制御機能など）（ここまで）

        #endregion

        #endregion

        #region コントロール取得メソッド

        /// <summary>Fxでハンドルしているコントロールを取得するメソッド</summary>
        /// <param name="controlName">取得したいコントロールのコントロール名</param>
        /// <returns>コントロールのオブジェクト参照</returns>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected Control GetFxWebControl(string controlName)
        {
            // Fxでハンドルしているコントロールの参照を取得する。

            // 2010/10/13 - ContainsKeyによるチェック処理を追加した。
            if (this.ControlHt.ContainsKey(controlName))
            {
                return (Control)this.ControlHt[controlName];
            }
            else
            {
                return null;
            }
        }

        #region マスタ ページの情報を初期化する

        /// <summary>マスタ ページの情報を初期化する</summary>
        private void GetMasterPages()
        {
            // ワーク変数
            string strTemp;

            // マスタ ページを遡る。
            MasterPage childMasterPage = null;
            MasterPage masterPage = this.Master;

            // 初期化
            this.RootMasterPage = null;
            this.StcMasterPage = new Stack<MasterPage>();
            this.RootMasterPageFileNoEx = null;
            this.StcMasterPageFileNoEx = new Stack<string>();

            // チェック
            if (masterPage == null)
            {
                // マスタ ページが無い場合
                throw new FrameworkException(
                    FrameworkExceptionMessage.NO_MASTER_PAGE[0],
                    String.Format(FrameworkExceptionMessage.NO_MASTER_PAGE[1], this.ContentPageFileNoEx));
            }
            else
            {
                // マスタ ページが有る場合

                // 遡る
                while (masterPage != null)
                {
                    // マスタ ページのファイル名を取得
                    if (childMasterPage == null)
                    {
                        // この場合、コンテンツ ページの１つ上

                        // マスタ ページのファイル名（拡張子抜き）をthis.MasterPageFileから取得
                        //aryTemp = this.MasterPageFile.Split('/');
                        //strTemp = aryTemp[aryTemp.Length - 1].Split('.')[0];
                        strTemp = PubCmnFunction.GetFileNameNoEx(this.MasterPageFile, '/');

                        // スタックに追加
                        this.StcMasterPage.Push(masterPage);
                        this.StcMasterPageFileNoEx.Push(strTemp);
                    }
                    else
                    {
                        // この場合、コンテンツ ページの１つ以上上（ネスト確定）

                        // マスタ ページのファイル名（拡張子抜き）をchildMasterPage.MasterPageFileから取得
                        //aryTemp = childMasterPage.MasterPageFile.Split('/');
                        //strTemp = aryTemp[aryTemp.Length - 1].Split('.')[0];
                        strTemp = PubCmnFunction.GetFileNameNoEx(childMasterPage.MasterPageFile, '/');

                        // スタックに追加
                        this.StcMasterPage.Push(masterPage);
                        this.StcMasterPageFileNoEx.Push(strTemp);
                    }

                    // ルートのmasterPageまで遡る。
                    childMasterPage = masterPage;
                    masterPage = masterPage.Master;
                }

                // スタックのPeekで、ルートのマスタ ページを設定する。
                this.RootMasterPage = (MasterPage)StcMasterPage.Peek();
                this.RootMasterPageFileNoEx = (string)StcMasterPageFileNoEx.Peek();
            }
        }

        #endregion

        #region ユーザ コントロールの情報を初期化する

        /// <summary>ユーザ コントロールの情報を初期化する</summary>
        /// <param name="ctrl">コントロール</param>
        private void GetUserControl(Control ctrl)
        {
            // 必要なら初期化する。
            if (this.LstUserControl == null)
            {
                this.LstUserControl = new List<UserControl>();
            }

            // ユーザ コントロールならば、
            if (ctrl is UserControl)
            {
                // ユーザ コントロールのリストに追加
                this.LstUserControl.Add((UserControl)ctrl);
            }

            // 子があれば
            if (ctrl.HasControls())
            {
                // 再帰検索する。
                foreach (Control child in ctrl.Controls)
                {
                    this.GetUserControl(child);
                }
            }
            else
            {
                // 何もない場合は、リターンする。
                return;
            }
        }

        #endregion

        #region マスタ ページ上のコントロールを取得するメソッド

        /// <summary>マスタ ページ上のコントロールを取得するメソッド</summary>
        /// <param name="controlName">取得したいコントロールのコントロール名</param>
        /// <returns>コントロールのオブジェクト参照</returns>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected Control GetMasterWebControl(string controlName)
        {
            // インターフェイス互換のため、
            // マスタ ページ名は返さないで捨てる。
            string masterPageFileNoEx = "";
            return this.GetMasterWebControl(controlName, out masterPageFileNoEx);
        }

        /// <summary>マスタ ページ上のコントロールを取得するメソッド</summary>
        /// <param name="controlName">取得したいコントロールのコントロール名</param>
        /// <param name="masterPageFileNoEx">マスタ ページのファイル名（拡張子無し）</param>
        /// <returns>コントロールのオブジェクト参照</returns>
        /// <remarks>マスタ ページ名は、GetMethodNameで使用するだけなのでprivate。</remarks>
        private Control GetMasterWebControl(string controlName, out string masterPageFileNoEx)
        {
            masterPageFileNoEx = "";
            Control ctrl = null;

            // ルート マスタ ページを検索する。
            ctrl = this.GetRootMasterWebControl(controlName, out masterPageFileNoEx);

            // 取得できていれば返す。
            if (ctrl != null)
            {
                return ctrl;
            }

            // ルート マスタ ページ以下を検索する。
            // 検索対象：マスタ ページ
            ctrl = this.GetCPFWebControl(controlName, 0, null,
                out masterPageFileNoEx, FxEnum.ControlSearchType.MasterPage);

            // 取得できていれば返す。
            if (ctrl != null)
            {
                return ctrl;
            }

            // 取得できていない場合
            masterPageFileNoEx = "";
            return null;
        }

        /// <summary>マスタ ページ上のコントロールを取得するメソッド</summary>
        /// <param name="controlName">取得したいコントロールのコントロール名</param>
        /// <param name="masterPageFileNoEx">マスタ ページのファイル名（拡張子無し）</param>
        /// <returns>コントロールのオブジェクト参照</returns>
        /// <remarks>ルートのマスタ ページのみ対象</remarks>
        private Control GetRootMasterWebControl(string controlName, out string masterPageFileNoEx)
        {
            // ルートのマスタ ページ名
            masterPageFileNoEx = this.RootMasterPageFileNoEx;
            // ルートのマスタ ページをFindControl
            return this.RootMasterPage.FindControl(controlName);
        }

        #endregion

        #region ユーザ コントロール上のコントロールを取得するメソッド

        /// <summary>ユーザ コントロール上のコントロールを取得するメソッド</summary>
        /// <param name="controlName">取得したいコントロールのコントロール名</param>
        /// <param name="userControlName">ユーザ コントロール名</param>
        /// <returns>コントロールのオブジェクト参照</returns>
        private Control GetUCWebControl(string controlName, out string userControlName)
        {
            // 検索
            foreach (UserControl uc in this.LstUserControl)
            {
                Control ctrl = uc.FindControl(controlName);

                if (ctrl == null)
                {
                    // この[UserControl]には無かった。
                }
                else
                {
                    // この[UserControl]に在った。

                    // ここは、他と同じロジック（this.StcMasterPageFileNoExの作成時）
                    // aryTemp = uc.AppRelativeVirtualPath.Split('/');
                    // userControlFileNoEx = aryTemp[aryTemp.Length - 1].Split('.')[0];
                    // userControlFileNoEx = PubCmnFunction.GetFileNameNoEx(uc.AppRelativeVirtualPath, '/');

                    // ユーザ コントロール名
                    userControlName = uc.ID;

                    // イベントの発生源を特定。
                    if (string.IsNullOrEmpty(Request.Form["__EVENTTARGET"]))
                    {
                        // __EVENTTARGETを使用しない。
                        foreach (string s in Request.Form.Keys)
                        {
                            if (s.IndexOf(userControlName + '$') != -1
                                && s.IndexOf('$' + controlName) != -1)
                            {
                                // この[UserControl]である。
                                return ctrl;
                            }
                            else
                            {
                                // この[UserControl]ではない。
                            }
                        }
                    }
                    else
                    {
                        // __EVENTTARGETを使用する。
                        if (Request.Form["__EVENTTARGET"].IndexOf(userControlName + '$') != -1
                            && Request.Form["__EVENTTARGET"].IndexOf('$' + controlName) != -1)
                        {
                            // この[UserControl]である。
                            return ctrl;
                        }
                        else
                        {
                            // この[UserControl]ではない。
                        }
                    }
                }
            }

            // すべての[UserControl]に無かった。
            userControlName = "";
            return null;
        }

        #endregion

        #region コンテンツ ページ上のコントロールを取得するメソッド

        /// <summary>コンテンツ ページ上のコントロールを取得するメソッド</summary>
        /// <param name="controlName">取得したいコントロールのコントロール名</param>
        /// <returns>コントロールのオブジェクト参照</returns>
        /// <remarks>画面コード親クラス２、画面コード クラスから利用する。</remarks>
        protected Control GetContentWebControl(string controlName)
        {
            // マスタ ページ名は返さないで捨てる。
            string masterPageFileNoEx = "";

            // ルート マスタ ページ以下を検索する。
            // 検索対象：コンテンツ ページ
            return this.GetCPFWebControl(controlName, 0, null,
                out masterPageFileNoEx, FxEnum.ControlSearchType.ContentsPage);
        }

        #endregion

        // ↓ マスタ ページ、コンテンツ ページ共用のメソッド
        #region コンテンツ プレイス ホルダー上のコントロールを取得するメソッド

        /// <summary>コンテンツ プレイス ホルダー上のコントロールを取得するメソッド</summary>
        /// <param name="controlName">取得したいコントロールのコントロール名</param>
        /// <param name="depth">深さ（再帰の際に使用）</param>
        /// <param name="cph1">コンテンツ プレイス ホルダ（再帰の際に使用）</param>
        /// <param name="masterPageFileNoEx">マスタ ページのファイル名（拡張子無し）</param>
        /// <param name="controlSearchType">コントロールの検索タイプ（MasterPage？ContentsPage？）</param>
        /// <returns>コントロールのオブジェクト参照</returns>
        /// <remarks>ブランチのマスタ ページとコンテンツ ページ上のコントロールのみ対象</remarks>
        private Control GetCPFWebControl(string controlName, int depth, ContentPlaceHolder cph1,
            out string masterPageFileNoEx, FxEnum.ControlSearchType controlSearchType)
        {
            // ワーク変数
            object[] aryObj = null;
            ContentPlaceHolder cph2 = null;
            BaseMasterController baseMasterController = null;

            // 戻り
            masterPageFileNoEx = "";
            Control ctrl = null;

            #region 処理を停止するかどうか

            // 検索対象を判定し、
            if (controlSearchType == FxEnum.ControlSearchType.MasterPage)
            {
                // マスタ ページを検索する場合は、

                // コンテンツ ページに至る場合は、処理しない。
                // ※ こちらは、GetContentWebControlで検索する。
                if (this.StcMasterPage.Count - 1 <= depth)
                {
                    // 再帰も終了
                    return ctrl;
                }
            }

            // 検索深度を超えた場合
            if (this.StcMasterPage.Count - 1 < depth)
            {
                // 再帰も終了
                return ctrl;
            }

            #endregion

            #region コンテンツ プレイス ホルダー情報を取得する。

            // コンテンツ プレイス ホルダー情報を取得するために、
            // 指定の深さのマスタ ページのキャストを試みる。
            try
            {
                // キャストできる
                aryObj = this.StcMasterPage.ToArray();
                baseMasterController = (BaseMasterController)aryObj[depth];
            }
            catch (Exception ex)
            {
                // キャストできない
                throw new FrameworkException(
                    FrameworkExceptionMessage.MASTER_PAGE_TYPE_ERROR[0],
                    String.Format(FrameworkExceptionMessage.MASTER_PAGE_TYPE_ERROR[1], aryObj[depth].GetType().ToString()), ex);
            }

            #endregion

            #region コントロールの参照を取得する。

            // コンテンツ プレイス ホルダーのコレクションでループする。
            foreach (string cphName in baseMasterController.ContentPlaceHolders2)
            {
                #region ページのコンテンツ プレイス ホルダーをFindControl

                if (depth == 0)
                {
                    // ルートの場合、RootMasterPageから。
                    cph2 = (ContentPlaceHolder)this.RootMasterPage.FindControl(cphName);
                }
                else
                {
                    // ブランチの場合、指定のcphから。
                    cph2 = (ContentPlaceHolder)cph1.FindControl(cphName);
                }

                #endregion

                #region コンテンツ プレイス ホルダーのnullチェック

                // 違うブランチを検索する場合があるので、nullがあり得るため。
                if (cph2 == null)
                {
                    // nullの場合
                }
                else
                {
                    // nullでない場合

                    #region コンテンツ プレイス ホルダーのコントロールをFindControl

                    // 検索対象を判定
                    if (controlSearchType == FxEnum.ControlSearchType.MasterPage)
                    {
                        // マスタ ページを検索する場合

                        // コンテンツ プレイス ホルダーのコントロールをFindControl
                        ctrl = cph2.FindControl(controlName);
                    }
                    else
                    {
                        // コンテンツ ページを検索する場合

                        if (this.StcMasterPage.Count - 1 <= depth)
                        {
                            // コンテンツ ページに至る場合のみ検索する。

                            // コンテンツ プレイス ホルダーのコントロールをFindControl
                            ctrl = cph2.FindControl(controlName);
                        }
                    }

                    #endregion

                    #region コントロールのnullチェック

                    if (ctrl == null)
                    {
                        // コントロールを発見できなかった場合

                        // 再帰呼出で検索を続行する。
                        ctrl = this.GetCPFWebControl(
                            controlName, depth + 1, cph2, out masterPageFileNoEx, controlSearchType);
                    }
                    else
                    {
                        // コントロールを発見できた場合

                        // 再帰を停止し、結果を返す。

                        // 検索対象を判定し、
                        if (controlSearchType == FxEnum.ControlSearchType.MasterPage)
                        {
                            // マスタ ページを検索する場合は、マスタ ページ名を返す。
                            aryObj = this.StcMasterPageFileNoEx.ToArray();
                            masterPageFileNoEx = (string)aryObj[depth + 1];
                        }

                        //// コントロール
                        //return ctrl; // 2009/09/07-この行
                    }

                    #endregion

                    // 2009/09/07-start

                    // コントロールが取得できていれば、
                    if (ctrl != null)
                    {
                        // コントロールを返す（再帰を上に辿る）
                        return ctrl;
                    }

                    // 2009/09/07-end
                }

                #endregion
            }

            #endregion

            // コントロールを返す（再帰から抜ける場合）
            return ctrl;
        }

        #endregion

        #endregion

        #endregion

        #region ＵＯＣメソッド

        #region ページ ロード イベント内のＵＯＣメソッド

        /// <summary>初回起動に対応した共通UOCメソッド</summary>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected abstract void UOC_CMNFormInit();

        /// <summary>ポストバックに対応した共通UOCメソッド</summary>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected abstract void UOC_CMNFormInit_PostBack();

        /// <summary>初回起動時に対応した個別UOCメソッド</summary>
        /// <remarks>派生の画面コード クラスでオーバーライドする。</remarks>
        protected abstract void UOC_FormInit();

        /// <summary>ポストバックに対応した個別UOCメソッド</summary>
        /// <remarks>派生の画面コード クラスでオーバーライドする。</remarks>
        protected abstract void UOC_FormInit_PostBack();

        #endregion

        #region イベント処理の開始前、終了後処理のＵＯＣメソッド

        /// <summary>イベントの開始前の処理を実装</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_PreAction(FxEventArgs fxEventArgs) { }

        /// <summary>イベントの終了後の処理を実装</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_AfterAction(FxEventArgs fxEventArgs) { }

        /// <summary>イベントの終了後の画面遷移処理を実装</summary>
        /// <param name="url">画面遷移する場合のURL</param>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_Screen_Transition(string url) { }

        /// <summary>Finally節の処理を実装</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_Finally(FxEventArgs fxEventArgs) { }

        #endregion

        #region エラー処理のＵＯＣメソッド

        /// <summary>BusinessApplicationExceptionの例外処理用のUOCメソッド</summary>
        /// <param name="baEx">BusinessApplicationException</param>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_ABEND(BusinessApplicationException baEx, FxEventArgs fxEventArgs) { }

        /// <summary>BusinessSystemExceptionの例外処理用のUOCメソッド</summary>
        /// <param name="bsEx">BusinessSystemException</param>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_ABEND(BusinessSystemException bsEx, FxEventArgs fxEventArgs) { }

        /// <summary>Exceptionの例外処理用のUOCメソッド</summary>
        /// <param name="ex">Exception</param>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <remarks>派生の画面コード親クラス２でオーバーライドする。</remarks>
        protected virtual void UOC_ABEND(Exception ex, FxEventArgs fxEventArgs) { }

        #endregion

        #region ダイアログ関連のＵＯＣメソッド

        /// <summary>
        /// 「YES」・「NO」メッセージ・ダイアログの「×」が押され閉じられた場合の処理を実装する。
        /// </summary>
        /// <param name="parentFxEventArgs">
        /// 「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴
        /// </param>
        /// <remarks>派生の画面コード クラスでオーバーライドする。</remarks>
        protected virtual void UOC_YesNoDialog_X_Click(FxEventArgs parentFxEventArgs) { }

        /// <summary>
        /// 「YES」・「NO」メッセージ・ダイアログの「YES」が押され閉じられた場合の処理を実装する。
        /// </summary>
        /// <param name="parentFxEventArgs">
        /// 「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴
        /// </param>
        /// <remarks>派生の画面コード クラスでオーバーライドする。</remarks>
        protected virtual void UOC_YesNoDialog_Yes_Click(FxEventArgs parentFxEventArgs) { }

        /// <summary>
        /// 「YES」・「NO」メッセージ・ダイアログの「NO」が押され閉じられた場合の処理を実装する。
        /// </summary>
        /// <param name="parentFxEventArgs">
        /// 「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴
        /// </param>
        /// <remarks>派生の画面コード クラスでオーバーライドする。</remarks>
        protected virtual void UOC_YesNoDialog_No_Click(FxEventArgs parentFxEventArgs) { }

        /// <summary>
        /// 業務モーダル画面の後処理を実装する。
        /// </summary>
        /// <param name="parentFxEventArgs">
        /// 業務モーダル画面を開いた（親画面側の）ボタンのボタン履歴
        /// </param>
        /// <param name="childFxEventArgs">
        /// 業務モーダル画面を閉じた（若しくは一番最後に押された子画面側の）ボタンのボタン履歴
        /// </param>
        /// <remarks>派生の画面コード クラスでオーバーライドする。</remarks>
        protected virtual void UOC_ModalDialog_End(FxEventArgs parentFxEventArgs, FxEventArgs childFxEventArgs) { }

        #endregion

        #endregion

        #region 画面遷移制御クラス（インナークラス）

        //**********************************************************************************
        //* クラス名        ：ScreenControl
        //* クラス日本語名  ：画面遷移制御クラス
        //*
        //* 作成日時        ：－
        //* 作成者          ：sas 生技
        //* 更新履歴        ：
        //*
        //*  日時        更新者            内容
        //*  ----------  ----------------  -------------------------------------------------
        //*  2009/03/13  西野  大介        新規作成
        //*  2009/04/21  西野  大介        FrameworkExceptionの追加に伴い、実装変更
        //*  2009/06/02  西野  大介        sln - IR版からの修正
        //*                                ・#1  ： 画面遷移制御のチェック処理不正（比較対象不正）
        //*                                ・#10 ： 画面遷移制御のmode属性不正時はラベルを組込む
        //*                                ・#14 ： XMLチェック処理追加
        //*                                ・#15 ： XML要素のリテラル化
        //**********************************************************************************

        /// <summary>画面遷移制御クラス</summary>
        /// <remarks>
        /// シングルトンとして利用されるデザイン パターンを採用
        /// インナークラス
        /// </remarks>
        private class ScreenControl
        {
            #region インスタンス変数

            /// <summary>画面遷移方法</summary>
            private string _transitionMethod;

            /// <summary>画面遷移方法</summary>
            public string TransitionMethod
            {
                get { return this._transitionMethod; }
            }

            /// <summary>画面遷移チェック</summary>
            private bool _transitionCheck;

            /// <summary>画面遷移チェック</summary>
            public bool TransitionCheck
            {
                get { return this._transitionCheck; }
            }

            /// <summary>画面遷移定義</summary>
            private XmlDocument XMLSCD = new XmlDocument();

            #endregion

            #region コンストラクタ

            /// <summary>コンストラクタ</summary>
            /// <remarks>
            /// シングルトンなので、初期化は起動時の１回のみ。
            /// インナークラス
            /// </remarks>
            public ScreenControl()
            {
                #region 画面遷移方法

                // 画面遷移方法の定義を取得
                string screenTransition =
                    GetConfigParameter.GetConfigValue(FxLiteral.SCREEN_TRANSITION_MODE);

                // デフォルト値対策：設定なし（null）の場合の扱いを決定
                if (screenTransition == null)
                {
                    // OFF扱い
                    screenTransition = FxLiteral.OFF;
                }

                // T / R / OFF
                if (screenTransition.ToUpper() == FxLiteral.TRANSFER)
                {
                    // Server.Transfer
                    this._transitionMethod = FxLiteral.TRANSFER;
                }
                else if (screenTransition.ToUpper() == FxLiteral.REDIRECT)
                {
                    // Response.Redirect
                    this._transitionMethod = FxLiteral.REDIRECT;
                }
                else if (screenTransition.ToUpper() == FxLiteral.OFF)
                {
                    // 画面遷移制御機能を無効にする
                    this._transitionMethod = FxLiteral.OFF;
                }
                else
                {
                    // パラメータ・エラー（書式不正）
                    throw new FrameworkException(
                        FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[0],
                        String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[1],
                            FxLiteral.SCREEN_TRANSITION_MODE));
                }

                #endregion

                #region 画面遷移定義

                if (this._transitionMethod == FxLiteral.OFF)
                {
                    // 画面遷移制御機能が無効な場合、画面遷移定義（XmlDocument）を空で初期化
                    this.XMLSCD.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><SCD></SCD>");
                }
                else
                {
                    // 画面遷移制御機能が有効な場合、画面遷移定義をロードする。

                    // リソース ローダでチェック（ここで落とすとハンドルされないので落とさない。）
                    if (EmbeddedResourceLoader.Exists(
                        GetConfigParameter.GetConfigValue(FxLiteral.XML_SC_DEFINITION), false))
                    {
                        // 画面遷移定義（XmlDocument）のロード
                        this.XMLSCD.LoadXml(
                            EmbeddedResourceLoader.LoadXMLAsString(
                                GetConfigParameter.GetConfigValue(FxLiteral.XML_SC_DEFINITION)));
                    }
                    else if (ResourceLoader.Exists(
                        GetConfigParameter.GetConfigValue(FxLiteral.XML_SC_DEFINITION), false))
                    {
                        // 画面遷移定義（XmlDocument）のロード
                        this.XMLSCD.Load(
                            PubCmnFunction.BuiltStringIntoEnvironmentVariable(
                                GetConfigParameter.GetConfigValue(FxLiteral.XML_SC_DEFINITION)));
                    }
                    else
                    {
                        // チェック
                        if (GetConfigParameter.GetConfigValue(FxLiteral.XML_SC_DEFINITION) == null
                            || GetConfigParameter.GetConfigValue(FxLiteral.XML_SC_DEFINITION) == "")
                        {
                            // 定義が無い（offの扱い）。

                            // 画面遷移定義（XmlDocument）を空で初期化
                            this.XMLSCD.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><SCD></SCD>");
                        }
                        else
                        {
                            // 定義が間違っている（エラー）。

                            // エラーをスロー
                            throw new FrameworkException(
                                FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH2[0],
                                String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH2[1],
                                    FxLiteral.XML_SC_DEFINITION));
                        }
                    }
                }

                #endregion

                #region 画面遷移チェック

                if (this._transitionMethod == FxLiteral.OFF)
                {
                    // 画面遷移制御機能が無効な場合、
                    // 強制的に画面遷移チェック処理（OFF）
                    this._transitionCheck = false;
                }
                else
                {
                    // 画面遷移制御機能が有効な場合、

                    // 画面遷移チェック処理の定義を取得
                    string screenTransitionCheck =
                        GetConfigParameter.GetConfigValue(FxLiteral.SCREEN_TRANSITION_CHECK);

                    // デフォルト値対策：設定なし（null）の場合の扱いを決定
                    if (screenTransitionCheck == null)
                    {
                        // OFF扱い
                        screenTransitionCheck = FxLiteral.OFF;
                    }

                    if (screenTransitionCheck.ToUpper() == FxLiteral.ON)
                    {
                        // 画面遷移チェック処理（ON）
                        this._transitionCheck = true;

                    }
                    else if (screenTransitionCheck.ToUpper() == FxLiteral.OFF)
                    {
                        // 画面遷移チェック処理（OFF）
                        this._transitionCheck = false;
                    }
                    else
                    {
                        // パラメータ・エラー（書式不正）
                        throw new FrameworkException(
                            FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[0],
                            String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1[1],
                                FxLiteral.SCREEN_TRANSITION_CHECK));
                    }
                }

                #endregion
            }

            #endregion

            // #1,10,14,15-start

            #region 画面遷移

            #region 部品使用時

            /// <summary>画面遷移する（画面遷移制御部品を使用する場合）</summary>
            /// <param name="currentAspxVirtualPath">現画面の仮想パス</param>
            /// <param name="label">遷移ラベル</param>
            /// <param name="windowGuid">ウィンドウGUID</param>
            /// <remarks>インナークラス</remarks>
            public void ScreenTransition(string currentAspxVirtualPath, string label, string windowGuid)
            {

                //#if DEBUG // 2009/10/15-このプリプロセッサ
                //                PerformanceRecorder perfRec = new PerformanceRecorder();
                //                perfRec.StartsPerformanceRecord();
                //#endif

                //                try // 2009/10/15-このtry-finallyブロック
                //                {

                #region QueryStringを退避

                string queryString = "";

                if (label.IndexOf('?') == -1)
                {
                    // QueryString無し
                }
                else
                {
                    // QueryString有り
                    string[] aryLabel = label.Split('?');

                    // URL
                    label = aryLabel[0];
                    // QueryString
                    queryString = aryLabel[1];
                }

                #endregion

                // 属性チェック用
                XmlNode xmlNode = null;

                // エラー処理で使う
                string value = "";

                #region CmnTransitionタグ

                // CmnTransitionタグを取得

                // 2009/11/6-start

                XmlElement xmlEmt_CmnTransition
                   = this.XMLSCD.GetElementById(label);
                // = null;

                #region 性能対策（GetElementById → GetElementsByTagName ＋ ループ）

                //// CmnTransitionタグを取得する。
                //XmlNodeList xmlNodeList_CmnTransition
                //    = this.XMLSCD.GetElementsByTagName(FxLiteral.XML_SC_TAG_CMN_TRANSITION);

                //if (xmlNodeList_CmnTransition == null)
                //{
                //    // CmnTransitionタグなし
                //}
                //else
                //{
                //    // CmnTransitionタグあり

                //    foreach (XmlElement xmlEmt_CmnTransition_fer in xmlNodeList_CmnTransition)
                //    {
                //        // 遷移に合致するCmnTransitionタグ

                //        // value属性
                //        xmlNode = xmlEmt_CmnTransition_fer.Attributes[FxLiteral.XML_CMN_ATTR_VALUE];

                //        if (xmlNode == null)
                //        {
                //            // value属性なしの場合

                //            // 例外を発生させる。
                //            throw new FrameworkException(
                //                FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                //                String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                //                    String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_value,
                //                        FxLiteral.XML_SC_TAG_CMN_TRANSITION, "－")));
                //        }
                //        else
                //        {
                //            // value属性ありの場合

                //            // 遷移に合致するCmnTransitionタグ
                //            if (xmlNode.Value.ToUpper().IndexOf(
                //                currentAspxVirtualPath.ToUpper()) != -1) // #1-左のステップ
                //            {
                //                // 正常（定義あり）
                //                xmlEmt_CmnTransition = xmlEmt_CmnTransition_fer;
                //                break; // foreachを抜ける。
                //            }
                //        }
                //    }
                //}

                #endregion

                // 2009/11/6-end

                if (xmlEmt_CmnTransition == null)
                {
                    // CmnTransitionタグがない。
                }
                else
                {
                    // CmnTransitionタグがある。

                    // 次画面の仮想パスを取得

                    // value属性
                    xmlNode = xmlEmt_CmnTransition.Attributes[FxLiteral.XML_CMN_ATTR_VALUE];

                    if (xmlNode == null)
                    {
                        // value属性なしの場合

                        // 例外を発生させる。
                        throw new FrameworkException(
                            FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                            String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                                String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_value,
                                    FxLiteral.XML_SC_TAG_CMN_TRANSITION, label)));
                    }
                    else
                    {
                        // value属性ありの場合

                        string tempUrl = xmlNode.Value;

                        // mode属性
                        xmlNode = xmlEmt_CmnTransition.Attributes[FxLiteral.XML_SC_ATTR_MODE];

                        // 画面遷移方法を取得
                        if (xmlNode == null)
                        {
                            // mode属性なしの場合

                            // 既定の画面遷移方法で画面遷移
                            if (this._transitionMethod == FxLiteral.TRANSFER)
                            {
                                // Transferで画面遷移
                                this.FxTransfer(tempUrl, queryString, windowGuid);
                            }
                            else if (this._transitionMethod == FxLiteral.REDIRECT)
                            {
                                // Redirectで画面遷移
                                this.FxRedirect(tempUrl, queryString, windowGuid);
                            }
                            else
                            {
                                // ありえない（初期処理でチェック済み）。
                            }
                        }
                        else
                        {
                            // mode属性ありの場合

                            // 指定の画面遷移方法で画面遷移
                            string transitionMode = xmlNode.Value;

                            if (transitionMode.ToUpper() == FxLiteral.TRANSFER)
                            {
                                // Transferで画面遷移
                                this.FxTransfer(tempUrl, queryString, windowGuid);
                            }
                            else if (transitionMode.ToUpper() == FxLiteral.REDIRECT)
                            {
                                // Redirectで画面遷移
                                this.FxRedirect(tempUrl, queryString, windowGuid);
                            }
                            else
                            {
                                // 画面遷移定義（XML）書式エラー

                                // 例外を発生させる。
                                // #10-start
                                throw new FrameworkException(
                                    FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR[0],
                                    String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR[1],
                                        String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_mode,
                                            transitionMode, FxLiteral.XML_SC_TAG_CMN_TRANSITION, label)));
                                // #10-end
                            }
                        }
                    }
                }

                #endregion

                // 先頭の～を消去、～は、アプリケーション名相当か。
                currentAspxVirtualPath = currentAspxVirtualPath.Substring(1);

                #region Screen-Transitionタグ

                // Screenタグを取得
                XmlNodeList xmlNodeList_Screen
                    = this.XMLSCD.GetElementsByTagName(FxLiteral.XML_SC_TAG_SCREEN);

                if (xmlNodeList_Screen == null)
                {
                    // Screenタグなし
                }
                else
                {
                    // Screenタグあり

                    foreach (XmlElement xmlEmt_Screen in xmlNodeList_Screen)
                    {
                        // value属性
                        xmlNode = xmlEmt_Screen.Attributes[FxLiteral.XML_CMN_ATTR_VALUE];

                        if (xmlNode == null)
                        {
                            // value属性なしの場合

                            // 例外を発生させる。
                            throw new FrameworkException(
                                FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                                String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                                    String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_value,
                                        FxLiteral.XML_SC_TAG_SCREEN, "－")));
                        }
                        else
                        {
                            // value属性ありの場合
                            value = xmlNode.Value;

                            // 現画面の仮想パスのScreenタグ
                            if (value.ToUpper().IndexOf(currentAspxVirtualPath.ToUpper()) != -1)
                            {
                                // 子のTransitionタグ
                                XmlNodeList xmlNodeList_Transition = xmlEmt_Screen.ChildNodes;

                                if (xmlNodeList_Screen == null)
                                {
                                    // Transitionタグなし
                                }
                                else
                                {
                                    // Transitionタグあり

                                    foreach (object temp in xmlNodeList_Transition)
                                    {
                                        // コメントがあった場合、無視する。
                                        if (!(temp is XmlElement)) continue;
                                        XmlElement xmlEmt_Transition = (XmlElement)temp;

                                        // value属性
                                        xmlNode = xmlEmt_Transition.Attributes[FxLiteral.XML_CMN_ATTR_VALUE];

                                        if (xmlNode == null)
                                        {
                                            // value属性なしの場合

                                            // 例外を発生させる。
                                            throw new FrameworkException(
                                                FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                                                String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                                                    String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_value,
                                                        FxLiteral.XML_SC_TAG_TRANSITION, "－")));
                                        }
                                        else
                                        {
                                            // value属性ありの場合
                                            value = xmlNode.Value;

                                            // label属性
                                            xmlNode = xmlEmt_Transition.Attributes[FxLiteral.XML_SC_ATTR_LABEL];

                                            if (xmlNode == null)
                                            {
                                                // label属性なしの場合

                                                // 例外を発生させる。
                                                throw new FrameworkException(
                                                    FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                                                    String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                                                        String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_label,
                                                            FxLiteral.XML_SC_TAG_TRANSITION, value)));
                                            }
                                            else
                                            {
                                                // label属性ありの場合

                                                // 遷移ラベルに合致するTransitionタグ
                                                if (xmlNode.Value.ToUpper() == label.ToUpper())
                                                {
                                                    // 次画面の仮想パスを取得
                                                    string tempUrl = value;

                                                    // mode属性
                                                    xmlNode = xmlEmt_Transition.Attributes[FxLiteral.XML_SC_ATTR_MODE];

                                                    // 画面遷移方法を取得
                                                    if (xmlNode == null)
                                                    {
                                                        // mode属性なしの場合

                                                        // 既定の画面遷移方法で画面遷移
                                                        if (this._transitionMethod == FxLiteral.TRANSFER)
                                                        {
                                                            // Transferで画面遷移
                                                            this.FxTransfer(tempUrl, queryString, windowGuid);
                                                        }
                                                        else if (this._transitionMethod == FxLiteral.REDIRECT)
                                                        {
                                                            // Redirectで画面遷移
                                                            this.FxRedirect(tempUrl, queryString, windowGuid);
                                                        }
                                                        else
                                                        {
                                                            // ありえない（初期処理でチェック済み）。
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // mode属性ありの場合

                                                        // 指定の画面遷移方法で画面遷移
                                                        string transitionMode = xmlNode.Value;

                                                        if (transitionMode.ToUpper() == FxLiteral.TRANSFER)
                                                        {
                                                            // Transferで画面遷移
                                                            this.FxTransfer(tempUrl, queryString, windowGuid);
                                                        }
                                                        else if (transitionMode.ToUpper() == FxLiteral.REDIRECT)
                                                        {
                                                            // Redirectで画面遷移
                                                            this.FxRedirect(tempUrl, queryString, windowGuid);
                                                        }
                                                        else
                                                        {
                                                            // 画面遷移定義（XML）書式エラー

                                                            // 例外を発生させる。
                                                            // #10-start
                                                            throw new FrameworkException(
                                                                FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR[0],
                                                                String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR[1],
                                                                    String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_mode,
                                                                        transitionMode, FxLiteral.XML_SC_TAG_TRANSITION, label)));
                                                            // #10-end
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion

                //                }
                //                finally
                //                {

                //#if DEBUG // 2009/10/15-このプリプロセッサ
                //                    Debug.WriteLine("画面遷移処理のパフォーマンス情報：\r\n" + perfRec.EndsPerformanceRecord());
                //#endif

                //                }

            }

            #endregion

            #region 部品不使用時

            /// <summary>Transferで画面遷移する（画面遷移制御部品を使用しないで画面遷移する場合）</summary>
            /// <param name="url">遷移先画面のURL</param>
            /// <param name="queryString">クエリー ストリング</param>
            /// <param name="windowGuid">ウィンドウGUID</param>
            /// <remarks>インナークラス</remarks>
            public void FxTransfer(string url, string queryString, string windowGuid)
            {
                // ウィンドウGUIDの送信（HttpContext）
                HttpContext.Current.Items[FxHttpContextIndex.BROWSER_WINDOW_GUID] = windowGuid;

                // QueryStringの追加
                if (queryString == "")
                {
                    // QueryStringの追加、無し
                }
                else
                {
                    // QueryStringの追加、有り

                    // 既存のQueryStringチェック
                    if (url.IndexOf('?') == -1)
                    {
                        // 既存のQueryString、無し
                        url += "?" + queryString;
                    }
                    else
                    {
                        // 既存のQueryString、有り
                        url += "&" + queryString;
                    }
                }

                // Transfer
                HttpContext.Current.Server.Transfer(url, false);

                // ★ 次画面への情報引継ぎには、HTTPコンテキストを明示的に使用するので。
                // 第二引数を[false]とし、Request.Form、Request.QueryStringコレクションは破棄する。
            }

            /// <summary>Redirectで画面遷移する（画面遷移制御部品を使用しないで画面遷移する場合）</summary>
            /// <param name="url">遷移先画面のURL</param>
            /// <param name="queryString">クエリー ストリング</param>
            /// <param name="windowGuid">ウィンドウGUID</param>
            /// <remarks>インナークラス</remarks>
            public void FxRedirect(string url, string queryString, string windowGuid)
            {
                // ウィンドウGUIDの送信（QueryString）

                // 既存のQueryStringチェック
                if (url.IndexOf('?') == -1)
                {
                    // 既存のQueryString、無し
                    url += "?" + FxHttpQueryStringIndex.BROWSER_WINDOW_GUID + "=" + windowGuid;
                }
                else
                {
                    // 既存のQueryString、有り
                    url += "&" + FxHttpQueryStringIndex.BROWSER_WINDOW_GUID + "=" + windowGuid;
                }

                // QueryStringの追加
                if (queryString == "")
                {
                    // QueryStringの追加、無し
                }
                else
                {
                    // QueryStringの追加、有り
                    url += "&" + queryString;
                    // ★ 既存のQueryStringは有るので場合分け不要
                }

                // Redirect
                HttpContext.Current.Response.Redirect(url);
            }

            #endregion

            #endregion

            #region 画面遷移チェック

            #region Get

            /// <summary>画面遷移チェック（Getの場合）</summary>
            /// <param name="currentAspxVirtualPath">現画面の仮想パス</param>
            /// <remarks>インナークラス</remarks>
            public void CheckScreenTransitionGet(string currentAspxVirtualPath)
            {
                // 先頭の～を消去、～は、アプリケーション名相当か。
                currentAspxVirtualPath = currentAspxVirtualPath.Substring(1);

                // Screenタグを取得する。
                XmlNodeList xmlNodeList_Screen
                    = this.XMLSCD.GetElementsByTagName(FxLiteral.XML_SC_TAG_SCREEN);

                // 属性チェック用
                XmlNode xmlNode = null;

                // エラー処理で使う
                string value = "";

                if (xmlNodeList_Screen == null)
                {
                    // Screenタグなし
                }
                else
                {
                    // Screenタグあり

                    foreach (XmlElement xmlEmt_Screen in xmlNodeList_Screen)
                    {
                        // 現画面の仮想パスのScreenタグ

                        // value属性
                        xmlNode = xmlEmt_Screen.Attributes[FxLiteral.XML_CMN_ATTR_VALUE];

                        if (xmlNode == null)
                        {
                            // value属性なしの場合

                            // 例外を発生させる。
                            throw new FrameworkException(
                                FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                                String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                                    String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_value,
                                        FxLiteral.XML_SC_TAG_SCREEN, "－")));
                        }
                        else
                        {
                            // value属性ありの場合
                            value = xmlNode.Value;

                            if (value.ToUpper().IndexOf(currentAspxVirtualPath.ToUpper()) != -1)
                            {
                                // directLink属性
                                xmlNode = xmlEmt_Screen.Attributes[FxLiteral.XML_SC_ATTR_DIRECTLINK];

                                if (xmlNode == null)
                                {
                                    // directLink属性なしの場合

                                    // 例外を発生させる。
                                    throw new FrameworkException(
                                        FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                                        String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                                            String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_dl1, value)));
                                }
                                else
                                {
                                    // directLink属性ありの場合

                                    if (xmlNode.Value.ToUpper() == FxLiteral.DENY)
                                    {
                                        // 異常（Get拒否）

                                        // 例外を発生させる。
                                        throw new FrameworkException(
                                            FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                                            String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                                                String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR_get, currentAspxVirtualPath)));
                                    }
                                    else if (xmlNode.Value.ToUpper() == FxLiteral.ALLOW)
                                    {
                                        // 正常（Get許可）
                                        return;
                                    }
                                    else
                                    {
                                        // 画面遷移定義（XML）書式エラー

                                        // 例外を発生させる。
                                        throw new FrameworkException(
                                            FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR[0],
                                            String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR[1],
                                                String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_dl2, xmlNode.Value, value)));
                                    }
                                }
                            }
                        }
                    }
                }

                // ここまで抜けた場合、画面遷移定義が無い。

                // 異常（Getエラー）

                // 例外を発生させる。
                throw new FrameworkException(
                    FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                    String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                        String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR_get, currentAspxVirtualPath)));
            }

            #endregion

            #region Post

            /// <summary>画面遷移チェック（画面遷移部品を使用しないPostの場合）</summary>
            /// <param name="currentAspxVirtualPath">現画面の仮想パス</param>
            /// <remarks>インナークラス</remarks>
            public void CheckScreenTransitionPost(string currentAspxVirtualPath)
            {
                // 先頭の～を消去、～は、アプリケーション名に相当か。
                currentAspxVirtualPath = currentAspxVirtualPath.Substring(1);

                // 例外を発生させる。
                throw new FrameworkException(
                    FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                    String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                        String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR_naked, currentAspxVirtualPath)));
            }

            #endregion

            #region ScreenTransition

            /// <summary>画面遷移チェック</summary>
            /// <param name="formerAspxVirtualPath">前画面の仮想パス</param>
            /// <param name="currentAspxVirtualPath">現画面の仮想パス</param>
            /// <remarks>インナークラス</remarks>
            public void CheckScreenTransition(
                string formerAspxVirtualPath, string currentAspxVirtualPath)
            {

                //#if DEBUG // 2009/10/15-このプリプロセッサ
                //                PerformanceRecorder perfRec = new PerformanceRecorder();
                //                perfRec.StartsPerformanceRecord();
                //#endif

                //                try // 2009/10/15-このtry-finallyブロック
                //                {
                // 先頭の～を消去、～は、アプリケーション名相当か。
                formerAspxVirtualPath = formerAspxVirtualPath.Substring(1);
                currentAspxVirtualPath = currentAspxVirtualPath.Substring(1);

                // 属性チェック用
                XmlNode xmlNode = null;

                #region CmnTransitionタグのチェック

                // CmnTransitionタグを取得する。
                XmlNodeList xmlNodeList_CmnTransition
                    = this.XMLSCD.GetElementsByTagName(FxLiteral.XML_SC_TAG_CMN_TRANSITION);

                if (xmlNodeList_CmnTransition == null)
                {
                    // CmnTransitionタグなし
                }
                else
                {
                    // CmnTransitionタグあり

                    foreach (XmlElement xmlEmt_CmnTransition in xmlNodeList_CmnTransition)
                    {
                        // 遷移に合致するCmnTransitionタグ

                        // value属性
                        xmlNode = xmlEmt_CmnTransition.Attributes[FxLiteral.XML_CMN_ATTR_VALUE];

                        if (xmlNode == null)
                        {
                            // value属性なしの場合

                            // 例外を発生させる。
                            throw new FrameworkException(
                                FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                                String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                                    String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_value,
                                        FxLiteral.XML_SC_TAG_CMN_TRANSITION, "－")));
                        }
                        else
                        {
                            // value属性ありの場合

                            // 遷移に合致するCmnTransitionタグ
                            if (xmlNode.Value.ToUpper().IndexOf(
                                currentAspxVirtualPath.ToUpper()) != -1) // #1-左のステップ
                            {
                                // 正常（定義あり）
                                return;
                            }
                        }
                    }
                }

                #endregion

                #region Screen-Transitionタグのチェック

                XmlNodeList xmlNodeList_Screen
                    = this.XMLSCD.GetElementsByTagName(FxLiteral.XML_SC_TAG_SCREEN);

                if (xmlNodeList_Screen == null)
                {
                    // Screenタグなし
                }
                else
                {
                    // Screenタグあり

                    foreach (XmlElement xmlEmt_Screen in xmlNodeList_Screen)
                    {
                        // value属性
                        xmlNode = xmlEmt_Screen.Attributes[FxLiteral.XML_CMN_ATTR_VALUE];

                        if (xmlNode == null)
                        {
                            // value属性なしの場合

                            // 例外を発生させる。
                            throw new FrameworkException(
                                FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                                String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                                    String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_value,
                                        FxLiteral.XML_SC_TAG_SCREEN, "－")));
                        }
                        else
                        {
                            // value属性ありの場合

                            // 前画面の仮想パスのScreenタグ
                            if (xmlNode.Value.ToUpper().IndexOf(formerAspxVirtualPath.ToUpper()) != -1)
                            {
                                // 子のTransitionタグ
                                XmlNodeList xmlNodeList_Transition = xmlEmt_Screen.ChildNodes;

                                if (xmlNodeList_Transition == null)
                                {
                                    // Transitionタグなし
                                }
                                else
                                {
                                    // Transitionタグあり

                                    foreach (object temp in xmlNodeList_Transition)
                                    {
                                        // コメントがあった場合、無視する。
                                        if (!(temp is XmlElement)) continue;
                                        XmlElement xmlEmt_Transition = (XmlElement)temp;

                                        // value属性
                                        xmlNode = xmlEmt_Transition.Attributes[FxLiteral.XML_CMN_ATTR_VALUE];

                                        if (xmlNode == null)
                                        {
                                            // value属性なしの場合

                                            // 例外を発生させる。
                                            throw new FrameworkException(
                                                FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                                                String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                                                    String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_XML_FORMAT_ERROR_value,
                                                        FxLiteral.XML_SC_TAG_TRANSITION, "－")));
                                        }
                                        else
                                        {
                                            // value属性ありの場合

                                            // 遷移に合致するTransitionタグ
                                            if (xmlNode.Value.ToUpper().IndexOf(currentAspxVirtualPath.ToUpper()) != -1)
                                            {
                                                // 正常（定義あり）
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion

                // ここまで抜けた場合、画面遷移定義が無い。

                // 異常（ScreenTransitionエラー）

                // 例外を発生させる。
                throw new FrameworkException(
                    FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[0],
                    String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR[1],
                        String.Format(FrameworkExceptionMessage.SCREEN_CONTROL_CHECK_ERROR_nolbl,
                            formerAspxVirtualPath, currentAspxVirtualPath)));
                //                }
                //                finally
                //                {

                //#if DEBUG // 2009/10/15-このプリプロセッサ
                //                    Debug.WriteLine("画面遷移チェック処理のパフォーマンス情報：\r\n" + perfRec.EndsPerformanceRecord());
                //#endif

                //                }
            }

            #endregion

            #endregion

            // #1,10,14,15-end
        }

        #endregion
    }
}
        #endregion
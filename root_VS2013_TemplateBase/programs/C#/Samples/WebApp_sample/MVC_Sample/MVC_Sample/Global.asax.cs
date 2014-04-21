//**********************************************************************************
//* Global.asax
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：MvcApplication
//* クラス日本語名  ：Global.asaxのコード ビハインド
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MVC_Sample
{
    // メモ: IIS6 または IIS7 のクラシック モードの詳細については、
    // http://go.microsoft.com/?LinkId=9394801 を参照してください

    public class MvcApplication : System.Web.HttpApplication
    {

        /////////////////////////////////////////////////////////////////////////////////
        // イベント ハンドラ
        /////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////
        // アプリケーションの開始、終了に関するイベント
        ///////////////////////////////////////////////////

        /// <summary>
        /// アプリケーションの開始に関するイベント
        /// </summary>
        protected void Application_Start(object sender, EventArgs e)
        {
            // アプリケーションのスタートアップで実行するコード

            // 
            AreaRegistration.RegisterAllAreas();

            // 
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            // グローバルフィルタの登録
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // ルート定義の登録
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // バンドル＆ミニフィケーションの登録
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// アプリケーションの終了に関するイベント
        /// </summary>
        void Application_End(object sender, EventArgs e)
        {
            // アプリケーションのシャットダウンで実行するコード
        }

        ///////////////////////////////////////////////////
        // アプリケーションのエラーに関するイベント
        ///////////////////////////////////////////////////

        /// <summary>
        /// アプリケーションのエラーに関するイベント
        /// </summary>
        protected void Application_Error(object sender, EventArgs e)
        {
            // ハンドルされていないエラーが発生したときに実行するコード
        }

        ///////////////////////////////////////////////////
        // セッションの開始、終了に関するイベント
        ///////////////////////////////////////////////////

        /// <summary>
        /// セッションの開始に関するイベント
        /// </summary>
        void Session_Start(object sender, EventArgs e)
        {
            // 新規セッションを開始したときに実行するコード
        }

        /// <summary>
        /// セッションの終了に関するイベント
        /// </summary>
        void Session_End(object sender, EventArgs e)
        {
            // セッションが終了したときに実行するコード

            // Web.configファイル内でsessionstateモードが[InProc]に設定されているときのみ、Session_Endイベントが発生する。
            // sessionstateモードが[StateServer]か、または[SQLServer]に設定されている場合、イベントは発生しない。

        }

        /////////////////////////////////////////////////////////////////////////////////
        // ASP.NETパイプライン処理のイベント ハンドラ
        /////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////

        // Global.asaxが対応しているASP.NETパイプライン処理のイベント ハンドラの一覧
        // -----------------------------------------------------------------------------------
        // ① Application_OnBeginRequest                :リクエスト処理を開始する前に発生 
        // ② Application_OnAuthenticateRequest         :認証の直前に発生 
        // ③ Application_OnAuthorizeRequest            :認証が完了したタイミングで発生 
        // ④ Application_OnResolveRequestCache         :リクエストをキャッシングするタイミングで発生 
        // ⑤ Application_OnAcquireRequestState         :セッション状態などを取得するタイミングで発生 
        // ⑥ Application_OnPreRequestHandlerExecute    :ページの実行を開始する直前に発生 
        // ⑦ Application_OnPostRequestHandlerExecute   :ページの実行を完了した直後に発生 
        // ⑧ Application_OnReleaseRequestState         :すべての処理を完了したタイミングで発生 
        // ⑨ Application_OnUpdateRequestCache          :出力キャッシュを更新したタイミングで発生 
        // ⑩ Application_OnEndRequest                  :すべてのリクエスト処理が完了したタイミングで発生 
        // ⑪ Application_OnPreSendRequestHeaders       :ヘッダをクライアントに送信する直前に発生 
        // ⑫ Application_OnPreSendRequestContent       :コンテンツをクライアントに送信する直前に発生 

        // イベント・ハンドラはこの表の順番で呼び出される。

        // ただし、Application_OnPreSendRequestHeadersメソッドや
        // Application_OnPreSendRequestContentメソッドは
        // バッファ処理（HTTP応答バッファリング）が有効かどうかによって
        // 呼び出されるタイミングが異なるので注意すること。

        // バッファ処理が有効である場合には、上記表の順番で発生するが、
        // バッファ処理が無効である場合には最初のページ出力が開始される
        // 任意のタイミングで呼び出される。

        // なお、それぞれのイベント・ハンドラの名前から「Application_On」を
        // 取り除いた部分がGlobal.asaxで発生するイベントの名前である。
        // Global.asaxではイベント名に「Application_On」あるいは「Application_」を付けた
        // イベント・ハンドラが事前に定義されており、イベントの発生時に呼び出される。     

        /// <summary>
        /// ① リクエスト処理を開始する前に発生
        /// </summary>
        void Application_OnBeginRequest(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application_OnBeginRequest");
        }

        /// <summary>
        /// ② 認証の直前に発生
        /// </summary>
        void Application_OnAuthenticateRequest(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application_OnAuthenticateRequest");
        }

        /// <summary>
        /// ③ 認証が完了したタイミングで発生
        /// </summary>
        void Application_OnAuthorizeRequest(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application_OnAuthorizeRequest");
        }

        /// <summary>
        /// ④ リクエストをキャッシングするタイミングで発生
        /// </summary>
        void Application_OnResolveRequestCache(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application_OnResolveRequestCache");
        }

        /// <summary>
        /// ⑤ セッション状態などを取得するタイミングで発生
        /// </summary>
        void Application_OnAcquireRequestState(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application_OnAcquireRequestState");
        }

        /// <summary>
        /// ⑥ ページの実行を開始する直前に発生
        /// </summary>
        void Application_OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application_OnPreRequestHandlerExecute");
        }

        ///////////////////////////////////////////////////////////////////
        // ページの実行が⑥～⑦の間に入る。
        ///////////////////////////////////////////////////////////////////

        /// <summary>
        /// ⑦ ページの実行を完了した直後に発生
        /// </summary>
        void Application_OnPostRequestHandlerExecute(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application_OnPostRequestHandlerExecute");
        }

        /// <summary>
        /// ⑧ すべての処理を完了したタイミングで発生
        /// </summary>
        void Application_OnReleaseRequestState(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application_OnReleaseRequestState");
        }

        /// <summary>
        /// ⑨ 出力キャッシュを更新したタイミングで発生
        /// </summary>
        void Application_OnUpdateRequestCache(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application_OnUpdateRequestCache");
        }

        /// <summary>
        /// ⑩ すべてのリクエスト処理が完了したタイミングで発生
        /// </summary>
        void Application_OnEndRequest(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application_OnEndRequest");
        }

        /// <summary>
        /// ⑪ ヘッダをクライアントに送信する直前に発生
        /// </summary>
        void Application_OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application_OnPreSendRequestHeaders");
        }

        /// <summary>
        /// ⑫ コンテンツをクライアントに送信する直前に発生
        /// </summary>
        void Application_OnPreSendRequestContent(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application_OnPreSendRequestContent");
        }

    }
}
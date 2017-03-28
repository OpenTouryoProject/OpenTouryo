//**********************************************************************************
//* テンプレート
//**********************************************************************************

// サンプル中のテンプレートなので、必要に応じて流用して下さい。

//**********************************************************************************
//* クラス名        ：Global
//* クラス日本語名  ：Global.asaxのコード ビハインド
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*  2011/12/07  西野 大介         Application_ErrorにACCESSログを追加
//*  2012/04/05  西野 大介         Application_OnPreRequestHandlerExecute
//*                                OnPostRequestHandlerExecuteにACCESSログを追加
//**********************************************************************************

using System;

using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;

namespace ProjectX_sample
{
    /// <summary>Global.asax class </summary>
    public class Global : System.Web.HttpApplication
    {
        /////////////////////////////////////////////////////////////////////////////////
        // Global_asaxのメンバ変数(インスタンス変数）はスレッドセーフ
        /////////////////////////////////////////////////////////////////////////////////

        // ここにインスタンス変数を定義した場合、これは、各スレッドに割り当てられる。
        // 故に、マルチスレッド（ユーザ）のASP.NETアプリケーションでも競合しない。
        // http:// support.microsoft.com/kb/312607/ja

        // ---

        // 静的変数の場合は競合する。

        // ASP.NET1.0、1.1では、Applicationオブジェクトではなく、静的変数の使用が推奨されていたが、
        // ASP.NET2.0では、静的変数が使用できないので、静的変数ではなく、Applicationオブジェクトを
        // 使用する（ただし、Applicationオブジェクトも競合するので注意する）。

        /// <summary>性能測定</summary>                                                       
        private PerformanceRecorder perfRec;

        /////////////////////////////////////////////////////////////////////////////////
        // イベント ハンドラ
        /////////////////////////////////////////////////////////////////////////////////

        ///////////////////////////////////////////////////
        // アプリケーションの開始、終了に関するイベント
        ///////////////////////////////////////////////////

        /// <summary>
        /// アプリケーションの開始に関するイベント
        /// </summary>
        void Application_Start(object sender, EventArgs e)
        {
            // アプリケーションのスタートアップで実行するコード
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
        void Application_Error(object sender, EventArgs e)
        {
            // ハンドルされていないエラーが発生したときに実行するコード

            Exception ex = Server.GetLastError().GetBaseException();
            //Server.ClearError(); // Server.GetLastError()をクリア

            // ACCESSログ出力 ----------------------------------------------

            // ------------
            // Message部
            // ------------
            // ユーザ名, IPアドレス,レイヤ, 
            // 画面名, Control名, メソッド名, 処理名
            // 処理時間（実行時間）, 処理時間（CPU時間）
            // Error MessageID, Error Message等
            // ------------
            string strLogMessage =
                "," + "－" +
                "," + Request.UserHostAddress +
                "," + "－" +
                "," + "Global.asax" +
                "," + "Application_Error" +
                ",,,,," + ex.ToString();

            // Log4Netへログ出力
            LogIF.FatalLog("ACCESS", strLogMessage);

            // -------------------------------------------------------------
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
        }

        /// <summary>
        /// ② 認証の直前に発生
        /// </summary>
        void Application_OnAuthenticateRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ③ 認証が完了したタイミングで発生
        /// </summary>
        void Application_OnAuthorizeRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ④ リクエストをキャッシングするタイミングで発生
        /// </summary>
        void Application_OnResolveRequestCache(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ⑤ セッション状態などを取得するタイミングで発生
        /// </summary>
        void Application_OnAcquireRequestState(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ⑥ ページの実行を開始する直前に発生
        /// </summary>
        void Application_OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            // ------------
            // Message部
            // ------------
            // ユーザ名, IPアドレス, レイヤ, 
            // 画面名, Control名, メソッド名, 処理名
            // ------------
            string strLogMessage =
                "," + "－" +
                "," + Request.UserHostAddress +
                "," + "-----↓" +
                "," + "Global.asax" +
                "," + "Application_OnPreRequest";

            // Log4Netへログ出力
            LogIF.DebugLog("ACCESS", strLogMessage);

            // -------------------------------------------------------------

            // 性能測定開始
            this.perfRec = new PerformanceRecorder();
            this.perfRec.StartsPerformanceRecord();
        }

        ///////////////////////////////////////////////////////////////////
        // ページの実行が⑥～⑦の間に入る。
        ///////////////////////////////////////////////////////////////////

        /// <summary>
        /// ⑦ ページの実行を完了した直後に発生
        /// </summary>
        void Application_OnPostRequestHandlerExecute(object sender, EventArgs e)
        {
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

                // ------------
                // Message部
                // ------------
                // ユーザ名, IPアドレス, レイヤ, 
                // 画面名, Control名, メソッド名, 処理名
                // 処理時間（実行時間）, 処理時間（CPU時間）
                // ------------
                string strLogMessage =
                    "," + "－" +
                    "," + Request.UserHostAddress +
                    "," + "-----↑" +
                    "," + "Global.asax" +
                    "," + "Application_OnPostRequest" +
                    "," + "－" +
                    "," + "－" +
                    "," + this.perfRec.ExecTime +
                    "," + this.perfRec.CpuTime;

                // Log4Netへログ出力
                LogIF.DebugLog("ACCESS", strLogMessage);
            }
        }

        /// <summary>
        /// ⑧ すべての処理を完了したタイミングで発生
        /// </summary>
        void Application_OnReleaseRequestState(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ⑨ 出力キャッシュを更新したタイミングで発生
        /// </summary>
        void Application_OnUpdateRequestCache(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ⑩ すべてのリクエスト処理が完了したタイミングで発生
        /// </summary>
        void Application_OnEndRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ⑪ ヘッダをクライアントに送信する直前に発生
        /// </summary>
        void Application_OnPreSendRequestHeaders(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// ⑫ コンテンツをクライアントに送信する直前に発生
        /// </summary>
        void Application_OnPreSendRequestContent(object sender, EventArgs e)
        {
        }

    }
}

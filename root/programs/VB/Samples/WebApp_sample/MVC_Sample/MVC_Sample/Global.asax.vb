'**********************************************************************************
'* Global.asax
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：MvcApplication
'* クラス日本語名  ：Global.asaxのコード ビハインド
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Http
Imports System.Web.Mvc
Imports System.Web.Optimization
Imports System.Web.Routing

' メモ: IIS6 または IIS7 のクラシック モードの詳細については、
' http://go.microsoft.com/?LinkId=9394801 を参照してください

Public Class MvcApplication
    Inherits System.Web.HttpApplication

    '''''''''''''''''''''''''''''''''''''''''''
    ' イベント ハンドラ
    '''''''''''''''''''''''''''''''''''''''''''

    '''''''''''''''''''''''''''''''''''''''''''
    ' アプリケーションの開始、終了に関するイベント
    '''''''''''''''''''''''''''''''''''''''''''

    ''' <summary>
    ''' アプリケーションの開始に関するイベント
    ''' </summary>
    Protected Sub Application_Start(sender As Object, e As EventArgs)
        ' アプリケーションのスタートアップで実行するコード

        ' 
        AreaRegistration.RegisterAllAreas()

        ' 
        WebApiConfig.Register(GlobalConfiguration.Configuration)

        ' グローバルフィルタの登録
        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters)

        ' ルート定義の登録
        RouteConfig.RegisterRoutes(RouteTable.Routes)

        ' バンドル＆ミニフィケーションの登録
        BundleConfig.RegisterBundles(BundleTable.Bundles)
    End Sub

    ''' <summary>
    ''' アプリケーションの終了に関するイベント
    ''' </summary>
    Private Sub Application_End(sender As Object, e As EventArgs)
        ' アプリケーションのシャットダウンで実行するコード
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''
    ' アプリケーションのエラーに関するイベント
    '''''''''''''''''''''''''''''''''''''''''''

    ''' <summary>
    ''' アプリケーションのエラーに関するイベント
    ''' </summary>
    Protected Sub Application_Error(sender As Object, e As EventArgs)
        ' ハンドルされていないエラーが発生したときに実行するコード
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''
    ' セッションの開始、終了に関するイベント
    '''''''''''''''''''''''''''''''''''''''''''

    ''' <summary>
    ''' セッションの開始に関するイベント
    ''' </summary>
    Private Sub Session_Start(sender As Object, e As EventArgs)
        ' 新規セッションを開始したときに実行するコード
    End Sub

    ''' <summary>
    ''' セッションの終了に関するイベント
    ''' </summary>
    Private Sub Session_End(sender As Object, e As EventArgs)
        ' セッションが終了したときに実行するコード

        ' Web.configファイル内でsessionstateモードが[InProc]に設定されているときのみ、Session_Endイベントが発生する。
        ' sessionstateモードが[StateServer]か、または[SQLServer]に設定されている場合、イベントは発生しない。

    End Sub

    '''''''''''''''''''''''''''''''''''''''''''
    ' ASP.NETパイプライン処理のイベント ハンドラ
    '''''''''''''''''''''''''''''''''''''''''''

    '''''''''''''''''''''''''''''''''''''''''''

    ' Global.asaxが対応しているASP.NETパイプライン処理のイベント ハンドラの一覧
    ' -----------------------------------------------------------------------------------
    ' ① Application_OnBeginRequest                :リクエスト処理を開始する前に発生 
    ' ② Application_OnAuthenticateRequest         :認証の直前に発生 
    ' ③ Application_OnAuthorizeRequest            :認証が完了したタイミングで発生 
    ' ④ Application_OnResolveRequestCache         :リクエストをキャッシングするタイミングで発生 
    ' ⑤ Application_OnAcquireRequestState         :セッション状態などを取得するタイミングで発生 
    ' ⑥ Application_OnPreRequestHandlerExecute    :ページの実行を開始する直前に発生 
    ' ⑦ Application_OnPostRequestHandlerExecute   :ページの実行を完了した直後に発生 
    ' ⑧ Application_OnReleaseRequestState         :すべての処理を完了したタイミングで発生 
    ' ⑨ Application_OnUpdateRequestCache          :出力キャッシュを更新したタイミングで発生 
    ' ⑩ Application_OnEndRequest                  :すべてのリクエスト処理が完了したタイミングで発生 
    ' ⑪ Application_OnPreSendRequestHeaders       :ヘッダをクライアントに送信する直前に発生 
    ' ⑫ Application_OnPreSendRequestContent       :コンテンツをクライアントに送信する直前に発生 

    ' イベント・ハンドラはこの表の順番で呼び出される。

    ' ただし、Application_OnPreSendRequestHeadersメソッドや
    ' Application_OnPreSendRequestContentメソッドは
    ' バッファ処理（HTTP応答バッファリング）が有効かどうかによって
    ' 呼び出されるタイミングが異なるので注意すること。

    ' バッファ処理が有効である場合には、上記表の順番で発生するが、
    ' バッファ処理が無効である場合には最初のページ出力が開始される
    ' 任意のタイミングで呼び出される。

    ' なお、それぞれのイベント・ハンドラの名前から「Application_On」を
    ' 取り除いた部分がGlobal.asaxで発生するイベントの名前である。
    ' Global.asaxではイベント名に「Application_On」あるいは「Application_」を付けた
    ' イベント・ハンドラが事前に定義されており、イベントの発生時に呼び出される。     

    ''' <summary>
    ''' ① リクエスト処理を開始する前に発生
    ''' </summary>
    Private Sub Application_OnBeginRequest(sender As Object, e As EventArgs)
        System.Diagnostics.Debug.WriteLine("Application_OnBeginRequest")
    End Sub

    ''' <summary>
    ''' ② 認証の直前に発生
    ''' </summary>
    Private Sub Application_OnAuthenticateRequest(sender As Object, e As EventArgs)
        System.Diagnostics.Debug.WriteLine("Application_OnAuthenticateRequest")
    End Sub

    ''' <summary>
    ''' ③ 認証が完了したタイミングで発生
    ''' </summary>
    Private Sub Application_OnAuthorizeRequest(sender As Object, e As EventArgs)
        System.Diagnostics.Debug.WriteLine("Application_OnAuthorizeRequest")
    End Sub

    ''' <summary>
    ''' ④ リクエストをキャッシングするタイミングで発生
    ''' </summary>
    Private Sub Application_OnResolveRequestCache(sender As Object, e As EventArgs)
        System.Diagnostics.Debug.WriteLine("Application_OnResolveRequestCache")
    End Sub

    ''' <summary>
    ''' ⑤ セッション状態などを取得するタイミングで発生
    ''' </summary>
    Private Sub Application_OnAcquireRequestState(sender As Object, e As EventArgs)
        System.Diagnostics.Debug.WriteLine("Application_OnAcquireRequestState")
    End Sub

    ''' <summary>
    ''' ⑥ ページの実行を開始する直前に発生
    ''' </summary>
    Private Sub Application_OnPreRequestHandlerExecute(sender As Object, e As EventArgs)
        System.Diagnostics.Debug.WriteLine("Application_OnPreRequestHandlerExecute")
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''
    ' ページの実行が⑥～⑦の間に入る。
    '''''''''''''''''''''''''''''''''''''''''''

    ''' <summary>
    ''' ⑦ ページの実行を完了した直後に発生
    ''' </summary>
    Private Sub Application_OnPostRequestHandlerExecute(sender As Object, e As EventArgs)
        System.Diagnostics.Debug.WriteLine("Application_OnPostRequestHandlerExecute")
    End Sub

    ''' <summary>
    ''' ⑧ すべての処理を完了したタイミングで発生
    ''' </summary>
    Private Sub Application_OnReleaseRequestState(sender As Object, e As EventArgs)
        System.Diagnostics.Debug.WriteLine("Application_OnReleaseRequestState")
    End Sub

    ''' <summary>
    ''' ⑨ 出力キャッシュを更新したタイミングで発生
    ''' </summary>
    Private Sub Application_OnUpdateRequestCache(sender As Object, e As EventArgs)
        System.Diagnostics.Debug.WriteLine("Application_OnUpdateRequestCache")
    End Sub

    ''' <summary>
    ''' ⑩ すべてのリクエスト処理が完了したタイミングで発生
    ''' </summary>
    Private Sub Application_OnEndRequest(sender As Object, e As EventArgs)
        System.Diagnostics.Debug.WriteLine("Application_OnEndRequest")
    End Sub

    ''' <summary>
    ''' ⑪ ヘッダをクライアントに送信する直前に発生
    ''' </summary>
    Private Sub Application_OnPreSendRequestHeaders(sender As Object, e As EventArgs)
        System.Diagnostics.Debug.WriteLine("Application_OnPreSendRequestHeaders")
    End Sub

    ''' <summary>
    ''' ⑫ コンテンツをクライアントに送信する直前に発生
    ''' </summary>
    Private Sub Application_OnPreSendRequestContent(sender As Object, e As EventArgs)
        System.Diagnostics.Debug.WriteLine("Application_OnPreSendRequestContent")
    End Sub
End Class

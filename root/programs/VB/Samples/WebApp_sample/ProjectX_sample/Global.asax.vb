'**********************************************************************************
'* Global.asax
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Global_asax
'* クラス日本語名  ：Global.asaxのコード ビハインド
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*  2011/12/07  西野 大介         Application_ErrorにACCESSログを追加
'*  2012/04/05  西野 大介         Application_OnPreRequestHandlerExecute
'*                                OnPostRequestHandlerExecuteにACCESSログを追加
'**********************************************************************************
' System
Imports System
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic

Imports System.Web
Imports System.Web.Security

' Touryo
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Util

Public Class Global_asax
    Inherits System.Web.HttpApplication

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' Global_asaxのメンバ変数(インスタンス変数）はスレッドセーフ
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    ' ここにインスタンス変数を定義した場合、これは、各スレッドに割り当てられる。
    ' 故に、マルチスレッド（ユーザ）のASP.NETアプリケーションでも競合しない。
    ' http:' support.microsoft.com/kb/312607/ja

    ' ---

    ' 静的変数の場合は競合する。

    ' ASP.NET1.0、1.1では、Applicationオブジェクトではなく、静的変数の使用が推奨されていたが、
    ' ASP.NET2.0では、静的変数が使用できないので、静的変数ではなく、Applicationオブジェクトを
    ' 使用する（ただし、Applicationオブジェクトも競合するので注意する）。

    ''' <summary>性能測定</summary>                                                       
    Private perfRec As PerformanceRecorder

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' イベント ハンドラ
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    '''''''''''''''''''''''''''''''''''''''''''''''''
    ' アプリケーションの開始、終了に関するイベント
    '''''''''''''''''''''''''''''''''''''''''''''''''

    ''' <summary>
    ''' アプリケーションの開始に関するイベント
    ''' </summary>
    Private Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' アプリケーションのスタートアップで実行するコード
    End Sub

    ''' <summary>
    ''' アプリケーションの終了に関するイベント
    ''' </summary>
    Private Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' アプリケーションのシャットダウンで実行するコード
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''
    ' アプリケーションのエラーに関するイベント
    '''''''''''''''''''''''''''''''''''''''''''''''''

    ''' <summary>
    ''' アプリケーションのエラーに関するイベント
    ''' </summary>
    Private Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' ハンドルされていないエラーが発生したときに実行するコード

        Dim ex As Exception = Server.GetLastError().GetBaseException()
        'Server.ClearError(); ' Server.GetLastError()をクリア

        ' ACCESSログ出力 ----------------------------------------------

        ' ------------
        ' メッセージ部
        ' ------------
        ' ユーザ名, IPアドレス,レイヤ, 
        ' 画面名, コントロール名, メソッド名, 処理名
        ' 処理時間（実行時間）, 処理時間（CPU時間）
        ' エラーメッセージID, エラーメッセージ等
        ' ------------
        Dim strLogMessage As String = ("," & "－" & ",") + Request.UserHostAddress & "," & "－" & "," & "Global.asax" & "," & "Application_Error" & ",,,,," & ex.ToString()

        ' Log4Netへログ出力
        LogIF.FatalLog("ACCESS", strLogMessage)

        ' -------------------------------------------------------------
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''
    ' セッションの開始、終了に関するイベント
    '''''''''''''''''''''''''''''''''''''''''''''''''

    ''' <summary>
    ''' セッションの開始に関するイベント
    ''' </summary>
    Private Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' 新規セッションを開始したときに実行するコード
    End Sub

    ''' <summary>
    ''' セッションの終了に関するイベント
    ''' </summary>
    Private Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' セッションが終了したときに実行するコード

        ' Web.configファイル内でsessionstateモードが[InProc]に設定されているときのみ、Session_Endイベントが発生する。
        ' sessionstateモードが[StateServer]か、または[SQLServer]に設定されている場合、イベントは発生しない。

    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' ASP.NETパイプライン処理のイベント ハンドラ
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

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
    Private Sub Application_OnBeginRequest(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    ''' <summary>
    ''' ② 認証の直前に発生
    ''' </summary>
    Private Sub Application_OnAuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    ''' <summary>
    ''' ③ 認証が完了したタイミングで発生
    ''' </summary>
    Private Sub Application_OnAuthorizeRequest(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    ''' <summary>
    ''' ④ リクエストをキャッシングするタイミングで発生
    ''' </summary>
    Private Sub Application_OnResolveRequestCache(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    ''' <summary>
    ''' ⑤ セッション状態などを取得するタイミングで発生
    ''' </summary>
    Private Sub Application_OnAcquireRequestState(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    ''' <summary>
    ''' ⑥ ページの実行を開始する直前に発生
    ''' </summary>
    Private Sub Application_OnPreRequestHandlerExecute(ByVal sender As Object, ByVal e As EventArgs)
        ' ------------
        ' メッセージ部
        ' ------------
        ' ユーザ名, IPアドレス, レイヤ, 
        ' 画面名, コントロール名, メソッド名, 処理名
        ' ------------
        Dim strLogMessage As String = ("," & "－" & ",") + Request.UserHostAddress & "," & "-----↓" & "," & "Global.asax" & "," & "Application_OnPreRequest"

        ' Log4Netへログ出力
        LogIF.DebugLog("ACCESS", strLogMessage)

        ' -------------------------------------------------------------

        ' 性能測定開始
        Me.perfRec = New PerformanceRecorder()
        Me.perfRec.StartsPerformanceRecord()
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' ページの実行が⑥～⑦の間に入る。
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    ''' <summary>
    ''' ⑦ ページの実行を完了した直後に発生
    ''' </summary>
    Private Sub Application_OnPostRequestHandlerExecute(ByVal sender As Object, ByVal e As EventArgs)
        ' nullチェック
        ' なにもしない
        If Me.perfRec Is Nothing Then
        Else
            ' 性能測定終了
            Me.perfRec.EndsPerformanceRecord()

            ' ACCESSログ出力-----------------------------------------------

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス, レイヤ, 
            ' 画面名, コントロール名, メソッド名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' ------------
            Dim strLogMessage As String = ("," & "－" & ",") + Request.UserHostAddress & "," & "-----↑" & "," & "Global.asax" & "," & "Application_OnPostRequest" & "," & "－" & "," & "－" & "," & Convert.ToString(Me.perfRec.ExecTime) & "," & Convert.ToString(Me.perfRec.CpuTime)

            ' Log4Netへログ出力
            LogIF.DebugLog("ACCESS", strLogMessage)
        End If
    End Sub

    ''' <summary>
    ''' ⑧ すべての処理を完了したタイミングで発生
    ''' </summary>
    Private Sub Application_OnReleaseRequestState(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    ''' <summary>
    ''' ⑨ 出力キャッシュを更新したタイミングで発生
    ''' </summary>
    Private Sub Application_OnUpdateRequestCache(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    ''' <summary>
    ''' ⑩ すべてのリクエスト処理が完了したタイミングで発生
    ''' </summary>
    Private Sub Application_OnEndRequest(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    ''' <summary>
    ''' ⑪ ヘッダをクライアントに送信する直前に発生
    ''' </summary>
    Private Sub Application_OnPreSendRequestHeaders(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    ''' <summary>
    ''' ⑫ コンテンツをクライアントに送信する直前に発生
    ''' </summary>
    Private Sub Application_OnPreSendRequestContent(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

End Class

'**********************************************************************************
'* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
'**********************************************************************************

#Region "Apache License"
'  
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License. 
' You may obtain a copy of the License at
'
' http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
'
#End Region

'**********************************************************************************
'* クラス名        ：MyBaseMVController
'* クラス日本語名  ：ASP.NET MVC用 画面コード親クラス２（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2015/08/04  Supragyan         Added code for SessionTimeout to OnActionExecuting method.
'*  2015/08/31  Supragyan         Modified OnException method to display error message on Error screen
'*  2015/09/03  Supragyan         Modified ExceptionType,Session,RedireResult on OnException method 
'*  2015/10/27  Sai               Moved the code of SessionTimeout from OnActionExecuting method to BaseMVController class.  
'*  2015/10/30  Sai               Added else part to the filterContext If statement in OnException method to resolve
'*                                the exception occurs in the redirection method in the child action as per the comments in Github.  
'*  2015/11/03  Sai               Implemeted performance measurement in the methods
'*                                OnActionExecuting, OnActionExecuted, OnResultExecuting and OnResultExecuted
'*  2017/01/23  西野 大介         UserInfoプロパティとGetUserInfoメソッドを追加した。
'*  2017/01/24  西野 大介         ControllerName, ActionNameプロパティとGetRouteDataメソッドを追加した。
'*  2017/01/24  西野 大介         ログ出力の見直し（OnResultメソッドではDebugを使用、ViewではViewNameを表示。）
'*  2017/01/24  西野 大介         ログ出力の見直し（ログ出力フォーマットの全面的な見直し）
'*  2017/02/14  西野 大介         OnException内での null reference 対策を行った。
'*  2017/02/14  西野 大介         スイッチ付きのキャッシュ無効化処理を追加した。
'*  2017/02/28  西野 大介         OnExceptionのErrorMessage生成処理の見直し。
'*  2017/02/28  西野 大介         TransferErrorScreenメソッドを追加した。
'*  2017/02/28  西野 大介         エラーログの見直し（その他の例外の場合、ex.ToString()を出力）
'*  2018/07/19  西野 大介         復元後のユーザー情報をSessionに設定するコードを追加
'**********************************************************************************

Imports System.Web
Imports System.Web.Routing
Imports System.Web.Mvc

Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Util

#Region "イベント実行順"
' お楽しみはこれからだ！: イベントの実行順が面白くて
' http://takepara.blogspot.jp/2008/08/blog-post.html
'
' before Execute
'
' - OnAuthorization
'
' - OnActionExecuting
' -- Index action execute ← ここでアクション実行
' -- View
' - OnActionExecuted
'
' - OnResultExecuting
' -- page rendering ← ここでレンダリング
' - OnResultExecuted
'
' after Execute
#End Region

Namespace Touryo.Infrastructure.Business.Presentation
    ''' <summary>画面コード親クラス２</summary>
    ''' <remarks>（オーバーライドして）自由に利用できる。</remarks>
    Public Class MyBaseMVController
        Inherits BaseMVController
        ''' <summary>性能測定</summary>
        Private perfRec As PerformanceRecorder

        ''' <summary>UserInfo</summary>
        Protected UserInfo As MyUserInfo

        ''' <summary>ControllerName</summary>
        Protected ControllerName As String = ""

        ''' <summary>ActionName</summary>
        Protected ActionName As String = ""

#Region "OnAction"

        ''' <summary>
        ''' アクション メソッドの呼び出し前に呼び出されます。  
        ''' Controller.OnActionExecuting メソッド (System.Web.Mvc)
        ''' http://msdn.microsoft.com/ja-jp/library/system.web.mvc.controller.onactionexecuting.aspx
        ''' </summary>
        ''' <param name="filterContext">
        ''' 型: System.Web.Mvc.ActionExecutingContext
        ''' 現在の要求およびアクションに関する情報。
        ''' </param>
        Protected Overrides Sub OnActionExecuting(filterContext As ActionExecutingContext)
            Me.GetRouteData()

            ' カスタム認証処理 --------------------------------------------
            ' ・・・
            ' -------------------------------------------------------------

            ' 認証ユーザ情報をメンバにロードする --------------------------
            Me.GetUserInfo()
            ' -------------------------------------------------------------

            ' 権限チェック ------------------------------------------------
            ' ・・・
            ' -------------------------------------------------------------

            ' 閉塞チェック ------------------------------------------------
            ' ・・・
            ' -------------------------------------------------------------

            ' キャッシュ制御処理 ------------------------------------------
            Me.CacheControlWithSwitch()
            ' -------------------------------------------------------------

            ' 性能測定開始
            Me.perfRec = New PerformanceRecorder()
            Me.perfRec.StartsPerformanceRecord()

            ' Calling base class method.
            MyBase.OnActionExecuting(filterContext)

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim strLogMessage As String =
                "," & Convert.ToString(Me.UserInfo.UserName) &
                "," & Convert.ToString(Me.UserInfo.IPAddress) &
                "," & "----->" &
                "," & Me.ControllerName &
                "," & Me.ActionName & "(OnActionExecuting)"

            LogIF.InfoLog("ACCESS", strLogMessage)

        End Sub

        ''' <summary>
        ''' アクション メソッドの呼び出し後に呼び出されます。
        ''' Controller.OnActionExecuted メソッド (System.Web.Mvc)
        ''' http://msdn.microsoft.com/ja-jp/library/system.web.mvc.controller.onactionexecuted.aspx
        ''' </summary>
        ''' <param name="filterContext">
        ''' 型: System.Web.Mvc.ActionExecutedContext
        ''' 現在の要求およびアクションに関する情報。
        ''' </param>
        Protected Overrides Sub OnActionExecuted(filterContext As ActionExecutedContext)
            ' Calling base class method.
            MyBase.OnActionExecuted(filterContext)

            ' 性能測定終了
            Me.perfRec.EndsPerformanceRecord()

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim strLogMessage As String =
                "," & Convert.ToString(Me.UserInfo.UserName) &
                "," & Convert.ToString(Me.UserInfo.IPAddress) &
                "," & "<-----" &
                "," & Me.ControllerName &
                "," & Me.ActionName & "(OnActionExecuted)" &
                "," & perfRec.ExecTime &
                "," & perfRec.CpuTime

            LogIF.InfoLog("ACCESS", strLogMessage)
        End Sub

#Region "View"

        ''' <summary>
        ''' 応答にビューを表示する ViewResult オブジェクトを作成します。
        ''' Controller.View メソッド (System.Web.Mvc)
        ''' http://msdn.microsoft.com/ja-jp/library/system.web.mvc.controller.view.aspx
        ''' </summary>
        ''' <param name="view__1">ビュー</param>
        ''' <param name="model">モデル</param>
        ''' <returns>ViewResult オブジェクト</returns>
        Protected Overrides Function View(view__1 As IView, model As Object) As ViewResult
            Dim vr As ViewResult = MyBase.View(view__1, model)
            Dim temp As String() = vr.ViewName.Split("."c)

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim strLogMessage As String =
                "," & Convert.ToString(Me.UserInfo.UserName) &
                "," & Convert.ToString(Me.UserInfo.IPAddress) &
                "," & "----->>" &
                "," & Me.ControllerName &
                "," & Me.ActionName & " -> " & temp(temp.Length - 1)

            LogIF.InfoLog("ACCESS", strLogMessage)

            Return vr
        End Function

        ''' <summary>
        ''' 応答にビューを表示する ViewResult オブジェクトを作成します。
        ''' Controller.View メソッド (System.Web.Mvc)
        ''' http://msdn.microsoft.com/ja-jp/library/system.web.mvc.controller.view.aspx
        ''' </summary>
        ''' <param name="viewName">ビュー名</param>
        ''' <param name="masterName">マスター ページ名</param>
        ''' <param name="model">モデル</param>
        ''' <returns>ViewResult オブジェクト</returns>
        Protected Overrides Function View(viewName As String, masterName As String, model As Object) As ViewResult
            Dim vr As ViewResult = MyBase.View(viewName, masterName, model)
            Dim temp As String() = vr.ViewName.Split("."c)

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim strLogMessage As String =
                "," & Convert.ToString(Me.UserInfo.UserName) &
                "," & Convert.ToString(Me.UserInfo.IPAddress) &
                "," & "----->>" &
                "," & Me.ControllerName &
                "," & Me.ActionName & " -> " & temp(temp.Length - 1)

            LogIF.InfoLog("ACCESS", strLogMessage)

            Return vr
        End Function

#End Region

#End Region

#Region "OnResult"

        ''' <summary>
        ''' アクション メソッドによって返されたアクション結果が実行される前に呼び出されます。  
        ''' Controller.OnResultExecuting メソッド (System.Web.Mvc)
        ''' http://msdn.microsoft.com/ja-jp/library/system.web.mvc.controller.onresultexecuting.aspx
        ''' </summary>
        ''' <param name="filterContext">
        ''' 型: System.Web.Mvc.ResultExecutingContext
        ''' 現在の要求およびアクション結果に関する情報。
        ''' </param>
        Protected Overrides Sub OnResultExecuting(filterContext As ResultExecutingContext)
            ' イベント処理開始前にエラーが発生した場合は、
            ' this.perfRecがnullの場合があるので、null対策コードを挿入する。
            If Me.perfRec Is Nothing Then
                ' nullの場合、新しいインスタンスを生成し、性能測定開始。
                Me.perfRec = New PerformanceRecorder()
                perfRec.StartsPerformanceRecord()
            End If

            ' Calling base class method.
            MyBase.OnResultExecuting(filterContext)

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim strLogMessage As String =
                "," & Convert.ToString(Me.UserInfo.UserName) &
                "," & Convert.ToString(Me.UserInfo.IPAddress) &
                "," & "----->" &
                "," & Me.ControllerName &
                "," & Me.ActionName & "(OnResultExecuting)"

            LogIF.DebugLog("ACCESS", strLogMessage)
        End Sub

        ''' <summary>
        ''' アクション メソッドによって返されたアクション結果が実行された後に呼び出されます。 
        ''' Controller.OnResultExecuted メソッド (System.Web.Mvc)
        ''' http://msdn.microsoft.com/ja-jp/library/system.web.mvc.controller.onresultexecuted.aspx
        ''' </summary>
        ''' <param name="filterContext">
        ''' 型: System.Web.Mvc.ResultExecutingContext
        ''' 現在の要求およびアクション結果に関する情報。
        ''' </param>
        Protected Overrides Sub OnResultExecuted(filterContext As ResultExecutedContext)
            ' Calling base class method.
            MyBase.OnResultExecuted(filterContext)

            ' イベント処理開始前にエラーが発生した場合は、
            ' this.perfRecがnullの場合があるので、null対策コードを挿入する。
            If Me.perfRec Is Nothing Then
                ' nullの場合、新しいインスタンスを生成し、性能測定開始。
                Me.perfRec = New PerformanceRecorder()
                perfRec.StartsPerformanceRecord()
            End If

            Me.perfRec.EndsPerformanceRecord()

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim strLogMessage As String =
                "," & Convert.ToString(Me.UserInfo.UserName) &
                "," & Convert.ToString(Me.UserInfo.IPAddress) &
                "," & "<-----" &
                "," & Me.ControllerName &
                "," & Me.ActionName & "(OnResultExecuted)" &
                "," & perfRec.ExecTime &
                "," & perfRec.CpuTime

            LogIF.DebugLog("ACCESS", strLogMessage)
        End Sub

#End Region

#Region "OnException"

        ''' <summary>アクションでハンドルされない例外が発生したときに呼び出されます。</summary>
        ''' <param name="exceptionContext">
        ''' 型: System.Web.Mvc.ResultExecutingContext
        ''' 現在の要求およびアクション結果に関する情報。
        ''' </param>
        ''' <remarks>
        ''' web.config に customErrors mode="on" を追記（無い場合は、OnExceptionメソッドが動かない 
        ''' </remarks>
        Protected Overrides Sub OnException(exceptionContext As ExceptionContext)
            ' Calling base class method.
            MyBase.OnException(exceptionContext)
            ' エラーログの出力
            Me.OutputErrorLog(exceptionContext)
            ' エラー画面に画面遷移する
            Me.TransferErrorScreen(exceptionContext)
        End Sub

        ''' <summary>エラーログの出力</summary>
        ''' <param name="exceptionContext">ExceptionContext</param>
        Private Sub OutputErrorLog(exceptionContext As ExceptionContext)
            ' 非同期ControllerのInnerException対策（底のExceptionを取得する）。
            Dim ex As Exception = exceptionContext.Exception
            Dim bottomException As Exception = ex
            While bottomException.InnerException IsNot Nothing
                bottomException = bottomException.InnerException
            End While

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim userName As String = ""
            Dim ipAddress As String = ""
            If Me.UserInfo IsNot Nothing Then
                userName = Me.UserInfo.UserName
                ipAddress = Me.UserInfo.IPAddress
            End If

            Dim strLogMessage As String =
                "," & If(Me.UserInfo IsNot Nothing, Me.UserInfo.UserName, "null") &
                "," & If(Me.UserInfo IsNot Nothing, Me.UserInfo.IPAddress, "null") &
                "," & "<-----" & "," & Me.ControllerName & "," & Me.ActionName & "(OnException)" &
                "," &
                "," &
                "," & GetExceptionMessageID(bottomException) &
                "," & bottomException.Message & vbCr & vbLf &
                "," & bottomException.StackTrace & vbCr & vbLf &
                "," & ex.ToString()

            ' Exception.ToString()はRootのExceptionに対して行なう。
            LogIF.ErrorLog("ACCESS", strLogMessage)
        End Sub

        ''' <summary>例外発生時に、エラー画面に画面遷移</summary>
        ''' <param name="exceptionContext">ExceptionContext</param>
        Private Sub TransferErrorScreen(exceptionContext As ExceptionContext)
            ' 非同期ControllerのInnerException対策（底のExceptionを取得する）。
            Dim ex As Exception = exceptionContext.Exception
            Dim bottomException As Exception = ex
            While bottomException.InnerException IsNot Nothing
                bottomException = bottomException.InnerException
            End While

            '#Region "例外型を判別しエラーメッセージIDを取得"

            ' エラーメッセージ
            Dim err_msg As String

            ' エラー情報をセッションから取得
            Dim err_info As String

            ' エラー画面へのパスを取得 --- チェック不要（ベースクラスでチェック済み）
            Dim errorScreenPath As String = GetConfigParameter.GetConfigValue(FxLiteral.ERROR_SCREEN_PATH)

            ' エラーメッセージＩＤ
            Dim errMsgId As String = Me.GetExceptionMessageID(ex)

            '#End Region

            '#Region "エラー時に、セッションを開放しないで、業務を続行可能にする処理を追加。"

            ' 不正操作エラー or 画面遷移制御チェック エラー
            If errMsgId = "IllegalOperationCheckError" OrElse errMsgId = "ScreenControlCheckError" Then
                ' セッションをクリアしない
                Session(FxHttpContextIndex.SESSION_ABANDON_FLAG) = False
            Else
                ' セッションをクリアする
                Session(FxHttpContextIndex.SESSION_ABANDON_FLAG) = True
            End If

            '#End Region

            '#Region "エラー画面に表示するエラー情報を作成"

            err_msg = Environment.NewLine &
                "Error Message ID : " & errMsgId & Environment.NewLine &
                "Error Message : " & bottomException.Message.ToString()

            err_info = System.Environment.NewLine &
                "Current Request Url : " & Request.Url.ToString() & Environment.NewLine &
                "Exception.StackTrace : " & bottomException.StackTrace & Environment.NewLine &
                "Exception.ToString() : " & ex.ToString()

            ' Exception.ToString()はRootのExceptionに対して行なう。
            ' Add exception information to Session
            Session(FxHttpContextIndex.SYSTEM_EXCEPTION_MESSAGE) = err_msg
            Session(FxHttpContextIndex.SYSTEM_EXCEPTION_INFORMATION) = err_info

            ' Add Form information to Session
            Session(FxHttpContextIndex.FORMS_INFORMATION) = Request.Form

            '#End Region

            '#Region "エラー画面へ画面遷移"

            exceptionContext.ExceptionHandled = True
            exceptionContext.HttpContext.Response.Clear()

            If exceptionContext.HttpContext.Request.IsAjaxRequest() Then
                exceptionContext.Result = New JavaScriptResult() With {
                    .Script = "location.href = '" & errorScreenPath & "'"
                }
            ElseIf exceptionContext.IsChildAction Then
                exceptionContext.Result = New ContentResult() With {
                    .Content = "<script>location.href = '" & errorScreenPath & "'</script>"
                }
            Else
                exceptionContext.Result = New RedirectResult(errorScreenPath)
            End If

            '#End Region
        End Sub

        ''' <summary>ExceptionのMessageIDを返す。</summary>
        ''' <param name="ex">Exception</param>
        ''' <returns>ExceptionのMessageID</returns>
        Private Function GetExceptionMessageID(ex As Exception) As String
            ' Check exception type 
            If TypeOf ex Is BusinessSystemException Then
                ' システム例外
                Dim bsEx As BusinessSystemException = DirectCast(ex, BusinessSystemException)
                Return bsEx.messageID
            ElseIf TypeOf ex Is FrameworkException Then
                ' フレームワーク例外
                Dim fxEx As FrameworkException = DirectCast(ex, FrameworkException)
                Return fxEx.messageID
            Else
                ' それ以外の例外
                Return "other Exception"
            End If
        End Function

#End Region

#Region "情報取得用"

        ''' <summary>ユーザ情報を取得する</summary>
        Private Sub GetUserInfo()
            ' セッションステートレス対応
            ' SessionがOFFの場合
            If Me.HttpContext.Session Is Nothing Then
            Else
                ' 取得を試みる。
                Me.UserInfo = DirectCast(UserInfoHandle.GetUserInformation(), MyUserInfo)

                ' nullチェック
                If Me.UserInfo Is Nothing Then
                    ' nullの場合、仮の値を生成 / 設定する。
                    Dim userName As String = System.Threading.Thread.CurrentPrincipal.Identity.Name

                    If userName Is Nothing OrElse userName = "" Then
                        ' 未認証状態
                        Me.UserInfo = New MyUserInfo("未認証", Me.HttpContext.Request.UserHostAddress)
                    Else
                        ' 認証状態
                        Me.UserInfo = New MyUserInfo(userName, Me.HttpContext.Request.UserHostAddress)

                        ' 必要に応じて認証チケットのユーザ名からユーザ情報を復元する。
                        ' ★ 必要であれば、他の業務共通引継ぎ情報などをロードする。
                        ' ・・・

                        ' 復元したユーザ情報をセット
                        UserInfoHandle.SetUserInformation(Me.UserInfo)
                    End If
                End If
            End If

        End Sub

        ''' <summary>ルーティング情報を取得する</summary>
        Private Sub GetRouteData()
            Dim routeData As RouteData = RouteTable.Routes.GetRouteData(Me.HttpContext)

            Dim temp As String() = Nothing
            temp = routeData.Values("controller").ToString().Split("."c)
            Me.ControllerName = routeData.Values("controller").ToString()
            Me.ActionName = routeData.Values("action").ToString()
        End Sub

        ''' <summary>キャッシュ制御処理（スイッチ付き）</summary>
        Private Sub CacheControlWithSwitch()
            ' システムで固定に出来る場合は、ここでキャッシュ無効化する。
            ' また、ユーザープログラムのファイル・ダウンロード処理などで
            ' フレームワークの設定したキャッシュ制御を変更したい場合は、Response.Clearを実行して再設定する。

            ' 画面遷移方法の定義を取得
            Dim noCache As String = GetConfigParameter.GetConfigValue(MyLiteral.CACHE_CONTROL)

            ' デフォルト値対策：設定なし（null）の場合の扱いを決定
            If noCache Is Nothing Then
                ' OFF扱い
                noCache = FxLiteral.OFF
            End If

            If noCache.ToUpper() = FxLiteral.[ON] Then
                ' ON

                ' http - How to control web page caching, across all browsers? - Stack Overflow
                ' http://stackoverflow.com/questions/49547/how-to-control-web-page-caching-across-all-browsers

                ' Using ASP.NET-MVC:
                Me.Response.Cache.SetCacheability(HttpCacheability.NoCache)
                ' HTTP 1.1.
                Me.Response.Cache.AppendCacheExtension("no-store, must-revalidate")
                Me.Response.AppendHeader("Pragma", "no-cache")
                ' HTTP 1.0.
                ' Proxies.
                Me.Response.AppendHeader("Expires", "0")
                ' OFF
            ElseIf noCache.ToUpper() = FxLiteral.OFF Then
            Else
                ' パラメータ・エラー（書式不正）
                Throw New FrameworkException(
                    FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1(0),
                    [String].Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_SWITCH1(1), MyLiteral.CACHE_CONTROL))
            End If
        End Sub

#End Region
    End Class
End Namespace

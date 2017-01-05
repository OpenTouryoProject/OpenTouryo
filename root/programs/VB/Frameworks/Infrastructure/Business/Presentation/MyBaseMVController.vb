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
'*  2015/08/04  Supragyan        Added code for SessionTimeout to OnActionExecuting method.
'*  2015/08/31  Supragyan        Modified OnException method to display error message on Error screen
'*  2015/09/03  Supragyan        Modified ExceptionType,Session,RedireResult on OnException method 
'*  2015/10/27  Sai              Moved the code of SessionTimeout from OnActionExecuting method to BaseMVController class.  
'*  2015/10/30  Sai              Added else part to the filterContext If statement in OnException method to resolve the exception occurs
'*                               in the redirection method in the child action as per the comments in Github.  
'*  2015/11/03  Sai              Implemeted performance measurement in the methods OnActionExecuting, OnActionExecuted, OnResultExecuting
'*                               and OnResultExecuted     
'**********************************************************************************

' System
' System.Web
Imports System.Web.Mvc
' フレームワーク
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
' 部品
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Util

Namespace Touryo.Infrastructure.Business.Presentation
    ''' <summary>画面コード親クラス２</summary>
    ''' <remarks>（オーバーライドして）自由に利用できる。</remarks>
    Public Class MyBaseMVController
        Inherits BaseMVController
        ''' <summary>性能測定</summary>
        Private perfRec As PerformanceRecorder

        ''' <summary>
        ''' 応答にビューを表示する ViewResult オブジェクトを作成します。
        ''' Controller.View メソッド (System.Web.Mvc)
        ''' http://msdn.microsoft.com/ja-jp/library/system.web.mvc.controller.view.aspx
        ''' </summary>
        ''' <param name="view1">ビュー</param>
        ''' <param name="model">モデル</param>
        ''' <returns>ViewResult オブジェクト</returns>
        Protected Overrides Function View(view1 As IView, model As Object) As ViewResult
            Dim vr As ViewResult = MyBase.View(view1, model)

            ' Logging.
            LogIF.InfoLog("ACCESS", "View")

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
            LogIF.InfoLog("ACCESS", "View")
            Return MyBase.View(viewName, masterName, model)
        End Function

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
            ' 性能測定開始
            Me.perfRec = New PerformanceRecorder()
            Me.perfRec.StartsPerformanceRecord()

            ' Calling base class method.
            MyBase.OnActionExecuting(filterContext)

            ' Logging.

            Dim strLogMessage As String = ", -" & "," & Request.UserHostAddress & "," & "<-----" & "," & filterContext.Controller.ToString() & _
                "," & filterContext.ActionDescriptor.ActionName & "," & "OnActionExecuting" & "," & perfRec.ExecTime & _
                "," & perfRec.CpuTime

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

            Dim strLogMessage As String = ", -" & "," & Request.UserHostAddress & "," & "<-----" & "," & filterContext.Controller.ToString() & _
                "," & Convert.ToString(filterContext.ActionDescriptor.ActionName) & "," & "OnActionExecuted" & "," & perfRec.ExecTime & _
                "," & perfRec.CpuTime

            LogIF.InfoLog("ACCESS", strLogMessage)
        End Sub

        ''' <summary>アクションでハンドルされない例外が発生したときに呼び出されます。</summary>
        ''' <param name="filterContext">
        ''' 型: System.Web.Mvc.ResultExecutingContext
        ''' 現在の要求およびアクション結果に関する情報。
        ''' </param>
        ''' <remarks>
        ''' web.config に customErrors mode="on" を追記（無い場合は、OnExceptionメソッドが動かない 
        ''' </remarks>
        Protected Overrides Sub OnException(filterContext As ExceptionContext)
            ' Calling base class method.
            MyBase.OnException(filterContext)

            '#Region "例外型を判別しエラーメッセージIDを取得"

            ' エラーメッセージ
            Dim err_msg As String

            ' エラー情報をセッションから取得
            Dim err_info As String

            ' エラー画面へのパスを取得 --- チェック不要（ベースクラスでチェック済み）
            Dim errorScreenPath As String = GetConfigParameter.GetConfigValue(FxLiteral.ERROR_SCREEN_PATH)

            ' Store the exception information for a Session.
            Session("ExceptionInformation") = filterContext.Exception.ToString()

            ' エラーのタイプ

            ' エラーメッセージＩＤ
            Dim errMsgId As String = ""

            ' Check exception type 
            If TypeOf filterContext.Exception Is BusinessSystemException Then
                ' システム例外
                Dim bsEx As BusinessSystemException = DirectCast(filterContext.Exception, BusinessSystemException)
                errMsgId = bsEx.messageID
            ElseIf TypeOf filterContext.Exception Is FrameworkException Then
                ' フレームワーク例外
                Dim fxEx As FrameworkException = DirectCast(filterContext.Exception, FrameworkException)
                errMsgId = fxEx.messageID
            Else
                ' それ以外の例外
                errMsgId = "－"
            End If

            '#End Region

            '#Region "エラー時に、セッションを開放しないで、業務を続行可能にする処理を追加。"

            ' 不正操作エラー or 画面遷移制御チェック エラー
            If errMsgId = "IllegalOperationCheckError" OrElse _
                errMsgId = "ScreenControlCheckError" Then
                ' セッションをクリアしない
                Session(FxHttpContextIndex.SESSION_ABANDON_FLAG) = False
            Else
                ' セッションをクリアする
                Session(FxHttpContextIndex.SESSION_ABANDON_FLAG) = True
            End If

            '#End Region

            '#Region "エラー画面に表示するエラー情報を作成"

            err_msg = System.Environment.NewLine & _
                "エラーメッセージＩＤ: " & errMsgId & System.Environment.NewLine & _
                "エラーメッセージ: " & filterContext.Exception.Message.ToString()

            err_info = System.Environment.NewLine & _
                "対象URL: " & Request.Url.ToString() & System.Environment.NewLine & _
                "スタックトレース:" & filterContext.Exception.StackTrace.ToString() & System.Environment.NewLine & _
                "Exception.ToString():" & filterContext.ToString()

            ' Add exception information to Session。
            Session(FxHttpContextIndex.SYSTEM_EXCEPTION_MESSAGE) = err_msg
            Session(FxHttpContextIndex.SYSTEM_EXCEPTION_INFORMATION) = err_info

            '#End Region

            filterContext.ExceptionHandled = True
            filterContext.HttpContext.Response.Clear()

            ' エラー画面へ画面遷移

            If filterContext.HttpContext.Request.IsAjaxRequest() Then
                filterContext.Result = New JavaScriptResult() With {.Script = "location.href = '" & errorScreenPath & "'"}
            ElseIf filterContext.IsChildAction Then
                filterContext.Result = New ContentResult() With {.Content = "<script>location.href = '" & errorScreenPath & "'</script>"}
            Else
                filterContext.Result = New RedirectResult(errorScreenPath)
            End If

            ' Logging.
            Dim strLogMessage As String = ", -" & "," & Request.UserHostAddress & "," & "<-----" & "," & filterContext.Controller.ToString() & " - " _
                                          & "OnException" & " - " & filterContext.Exception.Message

            LogIF.ErrorLog("ACCESS", strLogMessage)
        End Sub

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

            ' Logging.
            Dim strLogMessage As String = ", -" & "," & Request.UserHostAddress & "," & "<-----" & "," & filterContext.Controller.ToString() & _
                "," & Convert.ToString(filterContext.Result) & "," & "OnResultExecuting" & "," & perfRec.ExecTime & _
                "," & perfRec.CpuTime

            LogIF.InfoLog("ACCESS", strLogMessage)
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

            ' Logging.
            Dim strLogMessage As String = ", -" & "," & Request.UserHostAddress & "," & "<-----" & "," & filterContext.Controller.ToString() & _
                "," & Convert.ToString(filterContext.Result) & "," & "OnResultExecuted" & "," & perfRec.ExecTime & _
                "," & perfRec.CpuTime

            LogIF.InfoLog("ACCESS", strLogMessage)
        End Sub
    End Class
End Namespace

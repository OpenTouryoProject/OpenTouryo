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
'* クラス名        ：MyBaseApiControllerAsync (ActionFilterAttribute)
'* クラス日本語名  ：非同期 ASP.NET WebAPI用 ベーククラス２（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2017/08/11  西野 大介         新規作成
'**********************************************************************************

Imports System
Imports System.Threading
Imports System.Threading.Tasks

Imports System.Web
'using System.Web.Mvc;
Imports System.Web.Http.Controllers
Imports System.Web.Http.Filters

Imports System.Net
Imports System.Net.Http

Imports Microsoft.Owin

Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Util

Namespace Touryo.Infrastructure.Business.Presentation
    ''' <summary>非同期 ASP.NET WebAPI用 ベーククラス２</summary>
    ''' <remarks>（ActionFilterAttributeとして）自由に利用できる。</remarks>
    <AttributeUsage(AttributeTargets.[Class] Or AttributeTargets.Method, Inherited:=True, AllowMultiple:=True)>
    Public Class MyBaseApiControllerAsync
        Inherits ActionFilterAttribute
        Implements IAuthenticationFilter
        Implements IActionFilter
        Implements IExceptionFilter
        ''' <summary>性能測定</summary>
        Private perfRec As PerformanceRecorder

        ''' <summary>UserInfo</summary>
        Protected UserInfo As MyUserInfo

        ''' <summary>ControllerName</summary>
        Protected ControllerName As String = ""

        ''' <summary>ActionName</summary>
        Protected ActionName As String = ""

        ''' <summary>
        ''' プロセスが承認を要求したときに呼び出します。
        ''' https://msdn.microsoft.com/ja-jp/library/dn314618.aspx
        ''' </summary>
        ''' <param name="authenticationContext">HttpAuthenticationContext</param>
        ''' <param name="cancellationToken">CancellationToken</param>
        Public Async Function AuthenticateAsync(authenticationContext As HttpAuthenticationContext, cancellationToken As CancellationToken) As Task Implements IAuthenticationFilter.AuthenticateAsync
            ' 認証ユーザ情報をメンバにロードする --------------------------
            Await Me.GetUserInfo(authenticationContext)
            ' -------------------------------------------------------------
        End Function

        ''' <summary>
        ''' ・・・
        ''' https://msdn.microsoft.com/ja-jp/library/dn573281.aspx
        ''' </summary>
        ''' <param name="authenticationChallengeContext">HttpAuthenticationChallengeContext</param>
        ''' <param name="cancellationToken">CancellationToken</param>
        Public Async Function ChallengeAsync(authenticationChallengeContext As HttpAuthenticationChallengeContext, cancellationToken As CancellationToken) As Task Implements IAuthenticationFilter.ChallengeAsync
        End Function

#Region "OnAction"

        ''' <summary>
        ''' アクション メソッドの呼び出し前に発生します。
        ''' https://msdn.microsoft.com/ja-jp/library/dn573278.aspx
        ''' </summary>
        ''' <param name="actionContext">HttpActionContext</param>
        ''' <param name="cancellationToken">CancellationToken</param>
        Public Overrides Async Function OnActionExecutingAsync(actionContext As HttpActionContext, cancellationToken As CancellationToken) As Task
            ' 権限チェック ------------------------------------------------
            ' ・・・
            ' -------------------------------------------------------------

            ' 閉塞チェック ------------------------------------------------
            ' ・・・
            ' -------------------------------------------------------------

            ' 性能測定開始
            Me.perfRec = New PerformanceRecorder()
            Me.perfRec.StartsPerformanceRecord()

            ' Calling base class method.
            Await MyBase.OnActionExecutingAsync(actionContext, cancellationToken)

            ' ------------
            ' メッセージ部
            ' ------------
            ' ユーザ名, IPアドレス,
            ' レイヤ, 画面名, コントロール名, 処理名
            ' 処理時間（実行時間）, 処理時間（CPU時間）
            ' エラーメッセージID, エラーメッセージ等
            ' ------------
            Dim strLogMessage As String = "," & Convert.ToString(Me.UserInfo.UserName) & "," & Convert.ToString(Me.UserInfo.IPAddress) & "," & "----->" & "," & Me.ControllerName & "," & Me.ActionName & "(OnActionExecuting)"

            LogIF.InfoLog("ACCESS", strLogMessage)

        End Function

        ''' <summary>
        ''' アクション メソッドの呼び出し後に発生します。
        ''' https://msdn.microsoft.com/ja-jp/library/dn573277.aspx
        ''' </summary>
        ''' <param name="actionExecutedContext">HttpActionExecutedContext</param>
        ''' <param name="cancellationToken">CancellationToken</param>
        Public Overrides Async Function OnActionExecutedAsync(actionExecutedContext As HttpActionExecutedContext, cancellationToken As CancellationToken) As Task
            ' Calling base class method.
            Await MyBase.OnActionExecutedAsync(actionExecutedContext, cancellationToken)

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
            Dim strLogMessage As String = "," & Convert.ToString(Me.UserInfo.UserName) & "," & Convert.ToString(Me.UserInfo.IPAddress) & "," & "<-----" & "," & Me.ControllerName & "," & Me.ActionName & "(OnActionExecuted)" & "," & Convert.ToString(perfRec.ExecTime) & "," & Convert.ToString(perfRec.CpuTime)

            LogIF.InfoLog("ACCESS", strLogMessage)
        End Function

#End Region

#Region "OnException"

        ''' <summary>
        ''' 非同期の例外フィルターを実行します。
        ''' https://msdn.microsoft.com/ja-jp/library/hh835353.aspx
        ''' </summary>
        ''' <param name="exceptionContext">HttpActionExecutedContext</param>
        ''' <param name="cancellationToken">CancellationToken</param>
        Public Async Function ExecuteExceptionFilterAsync(exceptionContext As HttpActionExecutedContext, cancellationToken As CancellationToken) As Task Implements IExceptionFilter.ExecuteExceptionFilterAsync
            ' エラーログの出力
            Await Me.OutputErrorLog(exceptionContext)
        End Function

        ''' <summary>エラーログの出力</summary>
        ''' <param name="exceptionContext">HttpActionExecutedContext</param>
        Private Async Function OutputErrorLog(exceptionContext As HttpActionExecutedContext) As Task
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

            'this.perfRec.ExecTime +
            'this.perfRec.CpuTime + 
            Dim strLogMessage As String = "," & Convert.ToString((If(Me.UserInfo IsNot Nothing, Me.UserInfo.UserName, "null"))) & "," & Convert.ToString((If(Me.UserInfo IsNot Nothing, Me.UserInfo.IPAddress, "null"))) & "," & "----->>" & "," & Me.ControllerName & "," & Me.ActionName & "(OnException)" & "," & "," & "," & GetExceptionMessageID(bottomException) & "," & bottomException.Message & vbCr & vbLf & "," & bottomException.StackTrace & vbCr & vbLf & "," & ex.ToString()
            ' Exception.ToString()はRootのExceptionに対して行なう。
            LogIF.ErrorLog("ACCESS", strLogMessage)
        End Function

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
        ''' <param name="authenticationContext">HttpAuthenticationContext</param>
        Private Async Function GetUserInfo(authenticationContext As HttpAuthenticationContext) As Task
            ' カスタム認証処理 --------------------------------------------
            ' Authorization: Bearer ヘッダから
            ' JWTアサーションを処理し、認証、UserInfoを生成するなど。
            ' ・・・
            System.Diagnostics.Debug.WriteLine(authenticationContext.Request.Headers.ToString())
            ' -------------------------------------------------------------

            ' nullチェック
            If Me.UserInfo Is Nothing Then
                ' nullの場合、仮の値を生成 / 設定する。
                Dim userName As String = System.Threading.Thread.CurrentPrincipal.Identity.Name

                If userName Is Nothing OrElse userName = "" Then
                    ' 未認証状態
                    Me.UserInfo = New MyUserInfo("未認証", Me.GetClientIpAddress(authenticationContext.Request))
                Else
                    ' 認証状態
                    ' 必要に応じて認証チケットのユーザ名からユーザ情報を復元する。
                    Me.UserInfo = New MyUserInfo(userName, Me.GetClientIpAddress(authenticationContext.Request))
                End If
                ' nullで無い場合、取得した値を設定する。
            Else
            End If

            ' ★ 必要であれば、他の業務共通引継ぎ情報などをロードする。
        End Function

        ''' <summary>GetClientIpAddress</summary>
        ''' <param name="request">HttpRequestMessage</param>
        ''' <returns>IPAddress(文字列)</returns>
        Private Function GetClientIpAddress(request As HttpRequestMessage) As String
            If request.Properties.ContainsKey("MS_HttpContext") Then
                Return IPAddress.Parse(DirectCast(request.Properties("MS_HttpContext"), HttpContextBase).Request.UserHostAddress).ToString()
            End If
            If request.Properties.ContainsKey("MS_OwinContext") Then
                Return IPAddress.Parse(DirectCast(request.Properties("MS_OwinContext"), OwinContext).Request.RemoteIpAddress).ToString()
            End If
            Return [String].Empty
        End Function

#End Region
    End Class
End Namespace

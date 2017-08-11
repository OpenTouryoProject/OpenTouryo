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
'* クラス名        ：MyBaseApiController (FilterAttribute)
'* クラス日本語名  ：ASP.NET WebAPI用 ベーククラス２（テンプレート）
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2017/08/11  西野 大介         新規作成
'**********************************************************************************

Imports System

Imports System.Web.Mvc

Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Util

Namespace Touryo.Infrastructure.Business.Presentation
    ''' <summary>ASP.NET WebAPI用 ベーククラス２</summary>
    ''' <remarks>（FilterAttributeとして）自由に利用できる。</remarks>
    Public Class MyBaseApiController
        Inherits ActionFilterAttribute
        Implements IExceptionFilter
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
        ''' ActionFilterAttribute.OnActionExecuting Method (ActionExecutingContext) (System.Web.Mvc)
        ''' https://msdn.microsoft.com/en-us/library/system.web.mvc.actionfilterattribute.onactionexecuting.aspx
        ''' </summary>
        ''' <param name="filterContext">
        ''' 型: System.Web.Mvc.ActionExecutingContext
        ''' 現在の要求およびアクションに関する情報。
        ''' </param>
        Public Overrides Sub OnActionExecuting(filterContext As ActionExecutingContext)
            ' カスタム認証処理 --------------------------------------------
            ' Authorization: Bearer ヘッダから
            ' JWTアサーションを処理し、認証、UserInfoを生成するなど。
            ' ・・・
            System.Diagnostics.Debug.WriteLine(filterContext.HttpContext.Request.Headers.ToString())
            ' -------------------------------------------------------------

            ' 認証ユーザ情報をメンバにロードする --------------------------
            Me.GetUserInfo(filterContext)
            ' -------------------------------------------------------------

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
        ''' ActionFilterAttribute.OnActionExecuted Method (ActionExecutedContext) (System.Web.Mvc)
        ''' https://msdn.microsoft.com/en-us/library/system.web.mvc.actionfilterattribute.onactionexecuted.aspx
        ''' </summary>
        ''' <param name="filterContext">
        ''' 型: System.Web.Mvc.ActionExecutedContext
        ''' 現在の要求およびアクションに関する情報。
        ''' </param>
        Public Overrides Sub OnActionExecuted(filterContext As ActionExecutedContext)
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
                "," & Convert.ToString(perfRec.ExecTime) &
                "," & Convert.ToString(perfRec.CpuTime)

            LogIF.InfoLog("ACCESS", strLogMessage)
        End Sub

#End Region

#Region "OnResult"

        ''' <summary>
        ''' アクション メソッドによって返されたアクション結果が実行される前に呼び出されます。  
        ''' ActionFilterAttribute.OnResultExecuting Method (ResultExecutingContext) (System.Web.Mvc)
        ''' https://msdn.microsoft.com/en-us/library/system.web.mvc.actionfilterattribute.onresultexecuting.aspx
        ''' </summary>
        ''' <param name="filterContext">
        ''' 型: System.Web.Mvc.ResultExecutingContext
        ''' 現在の要求およびアクション結果に関する情報。
        ''' </param>
        Public Overrides Sub OnResultExecuting(filterContext As ResultExecutingContext)
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
        ''' ActionFilterAttribute.OnResultExecuted Method (ResultExecutedContext) (System.Web.Mvc)
        ''' https://msdn.microsoft.com/en-us/library/system.web.mvc.actionfilterattribute.onresultexecuted.aspx
        ''' </summary>
        ''' <param name="filterContext">
        ''' 型: System.Web.Mvc.ResultExecutingContext
        ''' 現在の要求およびアクション結果に関する情報。
        ''' </param>
        Public Overrides Sub OnResultExecuted(filterContext As ResultExecutedContext)
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
                "," & Convert.ToString(perfRec.ExecTime) &
                "," & Convert.ToString(perfRec.CpuTime)

            LogIF.DebugLog("ACCESS", strLogMessage)
        End Sub

#End Region

#Region "OnException"

        ''' <summary>アクションでハンドルされない例外が発生したときに呼び出されます。</summary>
        ''' <param name="filterContext">
        ''' 型: System.Web.Mvc.ResultExecutingContext
        ''' 現在の要求およびアクション結果に関する情報。
        ''' </param>
        ''' <remarks>
        ''' web.config に customErrors mode="on" を追記（無い場合は、OnExceptionメソッドが動かない 
        ''' </remarks>
        Private Sub IExceptionFilter_OnException(filterContext As ExceptionContext) Implements IExceptionFilter.OnException
            ' エラーログの出力
            Me.OutputErrorLog(filterContext)
        End Sub

        ''' <summary>エラーログの出力</summary>
        ''' <param name="filterContext">ExceptionContext</param>
        Private Sub OutputErrorLog(filterContext As ExceptionContext)
            ' 非同期ControllerのInnerException対策（底のExceptionを取得する）。
            Dim ex As Exception = filterContext.Exception
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
            Dim strLogMessage As String =
                "," & Convert.ToString((If(Me.UserInfo IsNot Nothing, Me.UserInfo.UserName, "null"))) &
                "," & Convert.ToString((If(Me.UserInfo IsNot Nothing, Me.UserInfo.IPAddress, "null"))) &
                "," & "----->>" & "," & Me.ControllerName &
                "," & Me.ActionName & "(OnException)" &
                "," &
                "," &
                "," & GetExceptionMessageID(bottomException) &
                "," & bottomException.Message & vbCr & vbLf &
                "," & bottomException.StackTrace & vbCr & vbLf &
                "," & ex.ToString()
            ' Exception.ToString()はRootのExceptionに対して行なう。
            LogIF.ErrorLog("ACCESS", strLogMessage)
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
        Private Sub GetUserInfo(filterContext As ActionExecutingContext)
            ' nullチェック
            If Me.UserInfo Is Nothing Then
                ' nullの場合、仮の値を生成 / 設定する。
                Dim userName As String = System.Threading.Thread.CurrentPrincipal.Identity.Name

                If userName Is Nothing OrElse userName = "" Then
                    ' 未認証状態
                    Me.UserInfo = New MyUserInfo("未認証", filterContext.RequestContext.HttpContext.Request.UserHostAddress)
                Else
                    ' 認証状態
                    ' 必要に応じて認証チケットのユーザ名からユーザ情報を復元する。
                    Me.UserInfo = New MyUserInfo(userName, filterContext.RequestContext.HttpContext.Request.UserHostAddress)
                End If
                ' nullで無い場合、取得した値を設定する。
            Else
            End If

            ' ★ 必要であれば、他の業務共通引継ぎ情報などをロードする。
        End Sub

#End Region
    End Class
End Namespace

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
Imports System.Text
Imports System.Linq
Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Security.Claims
Imports System.Security.Principal
Imports System.Security.Authentication

Imports System.Web
Imports System.Web.Http.Controllers
Imports System.Web.Http.Filters

Imports System.Net
Imports System.Net.Http

Imports Microsoft.Owin
Imports Microsoft.Owin.Security.DataHandler.Encoder

Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Util
Imports Touryo.Infrastructure.Public.Util.JWT

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
        ''' https://msdn.microsoft.com/ja-jp/magazine/dn781361.aspx
        ''' </summary>
        ''' <param name="authenticationContext">HttpAuthenticationContext</param>
        ''' <param name="cancellationToken">CancellationToken</param>
        Public Async Function AuthenticateAsync(authenticationContext As HttpAuthenticationContext, cancellationToken As CancellationToken) As Task Implements IAuthenticationFilter.AuthenticateAsync
            ' 認証ユーザ情報をメンバにロードする
            Await Me.GetUserInfo(authenticationContext)
        End Function

        ''' <summary>
        ''' ・・・ChallengeAsync・・・
        ''' https://msdn.microsoft.com/ja-jp/library/dn573281.aspx
        ''' https://msdn.microsoft.com/ja-jp/magazine/dn781361.aspx
        ''' </summary>
        ''' <param name="authenticationChallengeContext">HttpAuthenticationChallengeContext</param>
        ''' <param name="cancellationToken">CancellationToken</param>
        Public Async Function ChallengeAsync(authenticationChallengeContext As HttpAuthenticationChallengeContext, cancellationToken As CancellationToken) As Task Implements IAuthenticationFilter.ChallengeAsync
            authenticationChallengeContext.Result = New ResultWithChallenge(authenticationChallengeContext.Result)
            Return
        End Function

#Region "OnAction"

        ''' <summary>
        ''' アクション メソッドの呼び出し前に発生します。
        ''' https://msdn.microsoft.com/ja-jp/library/dn573278.aspx
        ''' </summary>
        ''' <param name="actionContext">HttpActionContext</param>
        ''' <param name="cancellationToken">CancellationToken</param>
        Public Overrides Async Function OnActionExecutingAsync(actionContext As HttpActionContext, cancellationToken As CancellationToken) As Task
            ' Claimを取得する。
            Dim userName As String = "", roles As String = "", ipAddress As String = ""
            Me.GetClaims(userName, roles, ipAddress)

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

            'this.UserInfo.UserName +
            'this.UserInfo.IPAddress +
            Dim strLogMessage As String =
                "," & userName &
                "," & ipAddress &
                "," & "----->" &
                "," & Me.ControllerName &
                "," & Me.ActionName & "(OnActionExecuting)"

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

            ' Claimを取得する。
            Dim userName As String = "", roles As String = "", ipAddress As String = ""
            Me.GetClaims(userName, roles, ipAddress)

            'this.UserInfo.UserName +
            'this.UserInfo.IPAddress +
            Dim strLogMessage As String =
                "," & userName &
                "," & ipAddress &
                "," & "<-----" &
                "," & Me.ControllerName &
                "," & Me.ActionName & "(OnActionExecuted)" &
                "," & Convert.ToString(perfRec.ExecTime) &
                "," & Convert.ToString(perfRec.CpuTime)

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

            ' Claimを取得する。
            Dim userName As String = "", roles As String = "", ipAddress As String = ""
            Me.GetClaims(userName, roles, ipAddress)

            ' (this.UserInfo != null ? this.UserInfo.UserName : "null") +
            '(this.UserInfo != null ? this.UserInfo.IPAddress : "null") +
            'this.perfRec.ExecTime +
            'this.perfRec.CpuTime + 
            Dim strLogMessage As String =
                "," & userName &
                "," & ipAddress &
                "," & "----->>" &
                "," & Me.ControllerName &
                "," & Me.ActionName & "(ExecuteExceptionFilterAsync)" &
                "," & "," &
                "," & GetExceptionMessageID(bottomException) &
                "," & bottomException.Message & vbCr & vbLf &
                "," & bottomException.StackTrace & vbCr & vbLf & "," & ex.ToString()

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
            'System.Diagnostics.Debug.WriteLine(authenticationContext.Request.Headers.Authorization.ToString());
            ' -------------------------------------------------------------

            Dim claims As List(Of Claim) = Nothing

            If authenticationContext.Request.Headers.Authorization.Scheme.ToLower() = "bearer" Then
                Dim access_token As String = authenticationContext.Request.Headers.Authorization.Parameter
                Dim jwtRS256 As New JWT_RS256("C:\Git1\MultiPurposeAuthSite\root\programs\MultiPurposeAuthSite\CreateClientsIdentity\CreateClientsIdentity_RS256.cer", "test")

                If jwtRS256.Verify(access_token) Then
                    Dim base64UrlEncoder As New Base64UrlTextEncoder()
                    Dim jwtPayload As String = Encoding.UTF8.GetString(base64UrlEncoder.Decode(access_token.Split("."c)(1)))

                    ' id_tokenライクなJWTなので、中からsubなどを取り出すことができる。
                    Dim jobj As JObject = DirectCast(JsonConvert.DeserializeObject(jwtPayload), JObject)

                    'string nonce = (string)jobj["nonce"];
                    Dim iss As String = jobj("iss").ToString()
                    Dim aud As String = jobj("aud").ToString()
                    Dim iat As String = jobj("iat").ToString()
                    Dim exp As String = jobj("exp").ToString()

                    Dim [sub] As String = jobj("sub").ToString()
                    Dim roles As List(Of String) = JsonConvert.DeserializeObject(Of List(Of String))(jobj("roles").ToString())

                    If iss = "http://jwtssoauth.opentouryo.com" _
                        AndAlso aud = "f374a155909d486a9234693c34e94479" _
                        AndAlso Long.Parse(exp) >= DateTimeOffset.Now.ToUnixTimeSeconds() Then
                        ' ログインに成功

                        ' ActionFilterAttributeとApiController間の情報共有はcontext.Principalを使用する。
                        ' ★ 必要であれば、他の業務共通引継ぎ情報などをロードする。
                        claims = New List(Of Claim)() From {
                            New Claim(ClaimTypes.Name, [sub]),
                            New Claim(ClaimTypes.Role, String.Join(",", roles)),
                            New Claim("IpAddress", Me.GetClientIpAddress(authenticationContext.Request))
                        }

                        ' The request message contains valid credential
                        authenticationContext.Principal = New ClaimsPrincipal(New List(Of ClaimsIdentity)() From {
                            New ClaimsIdentity(claims, "Token")
                        })
                        Return
                    Else
                    End If
                Else
                End If
            Else
            End If

            ' 未認証状態
            ' The request message contains invalid credential
            'context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);

            claims = New List(Of Claim)() From {
                New Claim(ClaimTypes.Name, "未認証"),
                New Claim(ClaimTypes.Role, ""),
                New Claim("IpAddress", Me.GetClientIpAddress(authenticationContext.Request))
            }

            authenticationContext.Principal = New ClaimsPrincipal(New List(Of ClaimsIdentity)() From {
                New ClaimsIdentity(claims, "Token")
            })
            Return
        End Function

        ''' <summary>GetClientIpAddress</summary>
        ''' <param name="request">HttpRequestMessage</param>
        ''' <returns>IPAddress(文字列)</returns>
        Private Function GetClientIpAddress(request As HttpRequestMessage) As String
            Dim headerValues As IEnumerable(Of String) = Nothing
            Dim clientIp As String = ""

            If request.Headers.TryGetValues("X-Forwarded-For", headerValues) = True Then
                Dim xForwardedFor As String = headerValues.FirstOrDefault()
                clientIp = xForwardedFor.Split(","c).GetValue(0).ToString().Trim()
            Else
                If request.Properties.ContainsKey("MS_HttpContext") Then
                    clientIp = DirectCast(request.Properties("MS_HttpContext"), HttpContextWrapper).Request.UserHostAddress
                End If
            End If

            If clientIp = "::1" Then
                'localhost
                clientIp = "localhost"
            Else
                clientIp = clientIp.Split(":"c).GetValue(0).ToString().Trim()
            End If

            Return clientIp
        End Function

        ''' <summary>GetClaims</summary>
        ''' <param name="userName">string</param>
        ''' <param name="roles">string</param>
        ''' <param name="ipAddress">string</param>
        Private Sub GetClaims(ByRef userName As String, ByRef roles As String, ByRef ipAddress As String)
            Dim claims As IEnumerable(Of Claim) = DirectCast(HttpContext.Current.User.Identity, ClaimsIdentity).Claims
            userName = claims.FirstOrDefault(Function(c) c.Type = ClaimTypes.Name).Value
            roles = claims.FirstOrDefault(Function(c) c.Type = ClaimTypes.Role).Value
            ipAddress = claims.FirstOrDefault(Function(c) c.Type = "IpAddress").Value
        End Sub

#End Region
    End Class
End Namespace

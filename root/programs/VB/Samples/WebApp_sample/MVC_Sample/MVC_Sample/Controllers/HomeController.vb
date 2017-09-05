'**********************************************************************************
'* サンプル アプリ・コントローラ
'**********************************************************************************

' テスト用クラスなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：HomeController
'* クラス日本語名  ：認証用サンプル アプリ・コントローラ
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports MVC_Sample.Models.ViewModels

Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks

Imports Microsoft.Owin.Security.DataHandler.Encoder

Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Util
Imports Touryo.Infrastructure.Public.Util.JWT

Namespace Controllers
    ''' <summary>HomeController</summary>
    <Authorize>
    Public Class HomeController
        Inherits MyBaseMVController
        ''' <summary>Nonce</summary>
        Public ReadOnly Property Nonce() As String
            Get
                If Session("nonce") Is Nothing Then
                    Session("nonce") = GetPassword.Base64UrlSecret(10)
                End If
                Return DirectCast(Session("nonce"), String)
            End Get
        End Property

        ''' <summary>State</summary>
        Public ReadOnly Property State() As String
            Get
                If Session("state") Is Nothing Then
                    Session("state") = GetPassword.Base64UrlSecret(10)
                End If
                Return DirectCast(Session("state"), String)
            End Get
        End Property

        ''' <summary>
        ''' GET: Home
        ''' </summary>
        ''' <returns>ActionResult</returns>
        <HttpGet>
        <AllowAnonymous>
        Public Function Index() As ActionResult
            Return View()
        End Function

        ''' <summary>
        ''' GET: /Home/Login
        ''' </summary>
        ''' <returns>ActionResult</returns>
        <HttpGet>
        <AllowAnonymous>
        Public Function Login() As ActionResult
            ' Session消去
            Me.FxSessionAbandon()

            Return Me.View()
        End Function

        ''' <summary>
        ''' POST: /Home/Login
        ''' </summary>
        ''' <param name="model">LoginViewModel</param>
        ''' <returns>ActionResult</returns>
        <HttpPost>
        <AllowAnonymous>
        <ValidateAntiForgeryToken>
        Public Function Login(model As LoginViewModel) As ActionResult
            If Not Request.Form.AllKeys.Any(Function(x) x = "external") Then
                ' 通常ログイン
                If ModelState.IsValid Then
                    If Not String.IsNullOrEmpty(model.UserName) Then
                        ' 認証か完了した場合、認証チケットを生成し、元のページにRedirectする。
                        ' 第２引数は、「クライアントがCookieを永続化（ファイルとして保存）するかどうか。」
                        ' を設定する引数であるが、セキュリティを考慮して、falseの設定を勧める。
                        FormsAuthentication.RedirectFromLoginPage(model.UserName, False)

                        ' 認証情報を保存する。
                        Dim ui As New MyUserInfo(model.UserName, Request.UserHostAddress)
                        UserInfoHandle.SetUserInformation(ui)

                        '基盤に任せるのでリダイレクトしない。
                        'return this.Redirect(ReturnUrl);
                        Return New EmptyResult()
                    Else
                        ' ユーザー認証 失敗
                        Me.ModelState.AddModelError(String.Empty, "指定されたユーザー名またはパスワードが正しくありません。")
                    End If
                    ' LoginViewModelの検証に失敗
                Else
                End If

                ' Session消去
                Me.FxSessionAbandon()

                ' ポストバック的な
                Return Me.View(model)
            Else
                ' 外部ログイン
                Return Redirect(String.Format(
                                "http://localhost:63359/MultiPurposeAuthSite/Account/OAuthAuthorize?client_id=f53469c17c5a432f86ce563b7805ab89&response_type=code&scope=profile%20email%20phone%20address%20userid%20auth%20openid&state={0}&nonce={1}",
                                Me.State, Me.Nonce))
            End If
        End Function

        ''' <summary>
        ''' Get: /Home/Scroll
        ''' </summary>
        ''' <returns></returns>
        <HttpGet>
        Public Function Scroll() As ActionResult
            Return Me.View()
        End Function

        ''' <summary>
        ''' Get: /Home/Logout
        ''' </summary>
        ''' <returns></returns>
        <HttpGet>
        Public Function Logout() As ActionResult
            FormsAuthentication.SignOut()
            Return Me.Redirect(Url.Action("Index", "Home"))
        End Function

        ''' <summary>OAuthAuthorizationCodeGrantClient</summary>
        ''' <param name="code">string</param>
        ''' <param name="state">string</param>
        ''' <returns>ActionResultを非同期的に返す</returns>
        <HttpGet>
        <AllowAnonymous>
        Public Async Function OAuthAuthorizationCodeGrantClient(code As String, state As String) As Task(Of ActionResult)
            Try
                If state = Me.State Then
                    ' CSRF(XSRF)対策のstateの検証は重要
                    Dim httpClient As New HttpClient()

                    Dim httpRequestMessage As HttpRequestMessage = Nothing
                    Dim httpResponseMessage As HttpResponseMessage = Nothing

                    ' HttpRequestMessage (Method & RequestUri)
                    httpRequestMessage = New HttpRequestMessage() With {
                        .Method = HttpMethod.Post,
                        .RequestUri = New Uri("http://localhost:63359/MultiPurposeAuthSite/OAuthBearerToken")
                    }

                    ' HttpRequestMessage (Headers & Content)
                    httpRequestMessage.Headers.Authorization =
                        New AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(
                            String.Format("{0}:{1}", "f53469c17c5a432f86ce563b7805ab89", "cKdwJb6mRKVIJpGxEWjIC94zquQltw_ECfO-55p21YM"))))

                    httpRequestMessage.Content = New FormUrlEncodedContent(New Dictionary(Of String, String)() From {
                        {"grant_type", "authorization_code"},
                        {"code", code},
                        {"redirect_uri", System.Web.HttpUtility.HtmlEncode("http://localhost:58496/MVC_Sample/Home/OAuthAuthorizationCodeGrantClient")}
                    })

                    ' HttpResponseMessage
                    httpResponseMessage = Await httpClient.SendAsync(httpRequestMessage)
                    Dim response As String = Await httpResponseMessage.Content.ReadAsStringAsync()

                    ' 汎用認証サイトはOIDCをサポートしたのでid_tokenを取得し、検証可能。
                    Dim base64UrlEncoder As New Base64UrlTextEncoder()
                    Dim dic As Dictionary(Of String, String) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(response)

                    ' id_tokenの検証コード
                    Dim id_token As String = dic("id_token")

                    Dim jwtRS256 As New JWT_RS256("C:\Git1\MultiPurposeAuthSite\root\programs\MultiPurposeAuthSite\CreateClientsIdentity\CreateClientsIdentity_RS256.cer", "test")

                    If jwtRS256.Verify(id_token) Then
                        Dim jwtPayload As String = Encoding.UTF8.GetString(base64UrlEncoder.Decode(dic("id_token").Split("."c)(1)))

                        ' id_tokenライクなJWTなので、中からsubなどを取り出すことができる。
                        Dim jobj As JObject = DirectCast(JsonConvert.DeserializeObject(jwtPayload), JObject)

                        Dim nonce As String = jobj("nonce").ToString()
                        Dim iss As String = jobj("iss").ToString()
                        Dim aud As String = jobj("aud").ToString()
                        Dim iat As String = jobj("iat").ToString()
                        Dim exp As String = jobj("exp").ToString()

                        Dim [sub] As String = jobj("sub").ToString()

                        If nonce = Me.Nonce _
                            AndAlso iss = "http://jwtssoauth.opentouryo.com" _
                            AndAlso aud = "f53469c17c5a432f86ce563b7805ab89" _
                            AndAlso Long.Parse(exp) >= DateTimeOffset.Now.ToUnixTimeSeconds() Then
                            ' ログインに成功
                            FormsAuthentication.RedirectFromLoginPage([sub], False)
                            Dim ui As New MyUserInfo([sub], Request.UserHostAddress)
                            UserInfoHandle.SetUserInformation(ui)

                            Return New EmptyResult()
                        Else
                        End If
                    Else
                    End If
                End If

                ' ログインに失敗
                Return RedirectToAction("Login")
            Finally
                Me.ClearExLoginsParams()
            End Try
        End Function

        ''' <summary>ClearExLoginsParam</summary>
        Private Sub ClearExLoginsParams()
            Session("nonce") = Nothing
            Session("state") = Nothing
        End Sub
    End Class
End Namespace

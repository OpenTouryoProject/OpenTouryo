Imports System
Imports System.Text
Imports System.Collections.Generic
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Web.Security

Imports Microsoft.Owin.Security.DataHandler.Encoder

Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Util
Imports Touryo.Infrastructure.Public.Util.JWT

Namespace Aspx.Auth
    ''' <summary>認証画面</summary>
    Public Class OAuthAuthorizationCodeGrantClient
        Inherits System.Web.UI.Page

        ''' <summary>Nonce</summary>
        Public ReadOnly Property Nonce() As String
            Get
                If Session("nonce") Is Nothing Then
                    Session("nonce") = GetPassword.Base64UrlSecret(10)
                    Return DirectCast(Session("nonce"), String)
                Else
                    Return DirectCast(Session("nonce"), String)
                End If
            End Get
        End Property

        ''' <summary>State</summary>
        Public ReadOnly Property State() As String
            Get
                If Session("state") Is Nothing Then
                    Session("state") = GetPassword.Base64UrlSecret(10)
                    Return DirectCast(Session("state"), String)
                Else
                    Return DirectCast(Session("state"), String)
                End If
            End Get
        End Property

        ''' <summary></summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Protected Async Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim code As String = Request.QueryString("code")
            Dim state As String = Request.QueryString("state")

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
                httpRequestMessage.Headers.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(String.Format("{0}:{1}", "b6b393fe861b430eb4ee061006826b03", "p2RgAFKF-JaF0A9F1tyDXp4wMq-uQZYyvTBM8wr_v8g"))))

                httpRequestMessage.Content = New FormUrlEncodedContent(New Dictionary(Of String, String)() From {
                    {"grant_type", "authorization_code"},
                    {"code", code},
                    {"redirect_uri", System.Web.HttpUtility.HtmlEncode("http://localhost:9999/WebForms_Sample/Aspx/Auth/OAuthAuthorizationCodeGrantClient.aspx")}
                })

                ' HttpResponseMessage
                httpResponseMessage = Await httpClient.SendAsync(httpRequestMessage)
                Dim response__1 As String = Await httpResponseMessage.Content.ReadAsStringAsync()

                ' 汎用認証サイトはOIDCをサポートしたのでid_tokenを取得し、検証可能。
                Dim base64UrlEncoder As New Base64UrlTextEncoder()
                Dim dic As Dictionary(Of String, String) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(response__1)

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

                    If nonce = Me.Nonce AndAlso
                        iss = "http://jwtssoauth.opentouryo.com" AndAlso
                        aud = "b6b393fe861b430eb4ee061006826b03" AndAlso
                        Long.Parse(exp) >= DateTimeOffset.Now.ToUnixTimeSeconds() Then

                        ' ログインに成功
                        FormsAuthentication.RedirectFromLoginPage([sub], False)
                        Dim ui As New MyUserInfo([sub], Request.UserHostAddress)
                        UserInfoHandle.SetUserInformation(ui)

                        Return
                    Else
                    End If
                Else
                End If

                ' ログインに失敗
                Response.Redirect("../Start/login.aspx")
            End If

            ' スターター
        End Sub
    End Class
End Namespace
'**********************************************************************************
'* サンプル画面（認証）
'**********************************************************************************

' サンプル画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：OAuth2AuthorizationCodeGrantClient
'* クラス日本語名  ：OAuth2, OIDC認証画面
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System.Net.Http
Imports System.Net.Http.Headers

Imports Microsoft.Owin.Security.DataHandler.Encoder

Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Security

Namespace Aspx.OAuth2
    ''' <summary>認証画面</summary>
    Public Class OAuth2AuthorizationCodeGrantClient
        Inherits System.Web.UI.Page

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

        ''' <summary></summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Protected Async Sub Page_Load(sender As Object, e As EventArgs)
            Dim code As String = Request.QueryString("code")
            Dim state As String = Request.QueryString("state")

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
                    httpRequestMessage.Headers.Authorization = New AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(
                        String.Format("{0}:{1}", New String() {OAuth2AndOIDCParams.ClientID, OAuth2AndOIDCParams.ClientSecret}))))

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
                    If dic.ContainsKey("id_token") Then
                        Dim id_token As String = dic("id_token")

                        Dim [sub] As String = ""
                        Dim roles As List(Of String) = Nothing
                        Dim scopes As List(Of String) = Nothing
                        Dim jobj As JObject = Nothing

                        If JwtToken.Verify(id_token, [sub], roles, scopes, jobj) AndAlso jobj("nonce").ToString() = Me.Nonce Then
                            ' ログインに成功
                            FormsAuthentication.RedirectFromLoginPage([sub], False)
                            Dim ui As New MyUserInfo([sub], Request.UserHostAddress)
                            UserInfoHandle.SetUserInformation(ui)

                            Return

                        End If
                    Else
                    End If
                Else
                End If

                ' ログインに失敗
                Response.Redirect("../Start/login.aspx")
            Finally
                Me.ClearExLoginsParams()
            End Try
        End Sub

        ''' <summary>ClearExLoginsParam</summary>
        Private Sub ClearExLoginsParams()
            Session("nonce") = Nothing
            Session("state") = Nothing
        End Sub

    End Class
End Namespace
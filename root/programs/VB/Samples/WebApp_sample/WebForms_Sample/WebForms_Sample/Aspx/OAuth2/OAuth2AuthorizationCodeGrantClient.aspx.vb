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

Imports Microsoft.Owin.Security.DataHandler.Encoder

Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports Touryo.Infrastructure.Framework.Authentication
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Security.Pwd

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
                Dim response__1 As String = ""

                If state = Me.State Then
                    ' CSRF(XSRF)対策のstateの検証は重要
                    response__1 = Await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                        New Uri("https://localhost:44300/MultiPurposeAuthSite/token"),
                        OAuth2AndOIDCParams.ClientID, OAuth2AndOIDCParams.ClientSecret, "", code)

                    ' 汎用認証サイトはOIDCをサポートしたのでid_tokenを取得し、検証可能。
                    Dim base64UrlEncoder As New Base64UrlTextEncoder()
                    Dim dic As Dictionary(Of String, String) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(response__1)

                    Dim [sub] As String = ""
                    Dim nonce As String = ""
                    Dim roles As List(Of String) = Nothing
                    Dim scopes As List(Of String) = Nothing
                    Dim jobj As JObject = Nothing

                    ' id_tokenの検証
                    If IdToken.Verify(dic("id_token"), dic("access_token"),
                                      code, state, [sub], nonce, jobj) AndAlso nonce = Me.Nonce Then
                        ' ログインに成功
                        ' /userinfoエンドポイントにアクセスする場合
                        response__1 = Await OAuth2AndOIDCClient.GetUserInfoAsync(
                            New Uri("https://localhost:44300/MultiPurposeAuthSite/userinfo"), dic("access_token"))

                        FormsAuthentication.RedirectFromLoginPage([sub], False)
                        Dim ui As New MyUserInfo([sub], Request.UserHostAddress)
                        UserInfoHandle.SetUserInformation(ui)

                        Return
                    Else
                    End If
                Else
                End If
                
                'ResolveClientUrlがInvalidOperationExceptionを吐くので...
                '' ログインに失敗
                'Response.Redirect("../Start/login.aspx")
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
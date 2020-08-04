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
Imports System.Threading.Tasks

Imports Microsoft.Owin.Security.DataHandler.Encoder

Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Authentication
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Security.Pwd

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
                                CmnClientParams.SpRp_AuthRequestUri _
                                & "?client_id=" & OAuth2AndOIDCParams.ClientID _
                                & "&response_type=code" _
                                & "&scope=profile%20email%20phone%20address%20openid" _
                                & "&state={0}" _
                                & "&nonce={1}" _
                                & "&redirect_uri={2}" _
                                & "&prompt=none",
                                Me.State, Me.Nonce,
                                CustomEncode.UrlEncode(CmnClientParams.SpRp_RedirectUri)))
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

        ''' <summary>OAuth2AuthorizationCodeGrantClient</summary>
        ''' <param name="code">string</param>
        ''' <param name="state">string</param>
        ''' <returns>ActionResultを非同期的に返す</returns>
        <HttpGet>
        <AllowAnonymous>
        Public Async Function OAuth2AuthorizationCodeGrantClient(code As String, state As String) As Task(Of ActionResult)
            Try
                Dim response As String = ""

                If state = Me.State Then
                    ' CSRF(XSRF)対策のstateの検証は重要
                    response = Await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                        New Uri(CmnClientParams.SpRp_TokenRequestUri),
                        OAuth2AndOIDCParams.ClientID, OAuth2AndOIDCParams.ClientSecret, "", code)

                    ' 汎用認証サイトはOIDCをサポートしたのでid_tokenを取得し、検証可能。
                    Dim base64UrlEncoder As New Base64UrlTextEncoder()
                    Dim dic As Dictionary(Of String, String) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(response)

                    ' id_tokenの検証コード
                    If dic.ContainsKey("id_token") Then
                        Dim [sub] As String = ""
                        Dim nonce As String = ""
                        Dim jobj As JObject = Nothing

                        If IdToken.Verify(dic("id_token"), dic("access_token"),
                                          code, state, [sub], nonce, jobj) AndAlso nonce = Me.Nonce Then

                            ' ログインに成功

                            ' /userinfoエンドポイントにアクセスする場合
                            response = Await OAuth2AndOIDCClient.GetUserInfoAsync(
                                New Uri(CmnClientParams.SpRp_UserInfoUri), dic("access_token"))

                            FormsAuthentication.RedirectFromLoginPage([sub], False)
                            Dim ui As New MyUserInfo([sub], Request.UserHostAddress)
                            UserInfoHandle.SetUserInformation(ui)

                            Return New EmptyResult()

                        End If
                    Else
                    End If
                Else
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

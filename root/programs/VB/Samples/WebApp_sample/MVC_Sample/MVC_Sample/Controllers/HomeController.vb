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
'*  2026/11/04  玄人 幸道         C# 版に合わせて、外部ログインを今のライブラリと MPAS のスタブに合わせた（#649）
'*                                （prompt=none を外す・トークン要求に redirect_uri・id_token を jwks_uri の鍵と iss / aud で検証・
'*                                  表示名を UserInfo の email に・SAML2 の外部ログインを追加・並びを Region でまとめた）
'**********************************************************************************

Imports MVC_Sample.Models.ViewModels

Imports System.Security.Cryptography
Imports System.Threading.Tasks

Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Authentication
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Security.Jwt
Imports Touryo.Infrastructure.Public.Security.Pwd

Namespace Controllers
    ''' <summary>HomeController</summary>
    ''' <remarks>
    ''' 並び順（C# 版の HomeController と同じ）
    ''' | Region | 内容 |
    ''' |---|---|
    ''' | 定数 | Session のキー |
    ''' | プロパティ | OIDC の state・nonce |
    ''' | 画面 | Index・Login（GET / POST）・Scroll・Logout |
    ''' | 外部ログイン（OIDC） | 要求 → 応答（リダイレクト先）→ 検証 |
    ''' | 外部ログイン（SAML2） | 要求 → 応答（ACS） |
    ''' | 外部ログイン（共通） | サイン イン |
    ''' </remarks>
    <Authorize>
    Public Class HomeController
        Inherits MyBaseMVController

#Region "定数（Session のキー）"

        ''' <summary>OIDC の nonce</summary>
        Private Const SessionKeyNonce As String = "nonce"

        ''' <summary>OIDC の state</summary>
        Private Const SessionKeyState As String = "state"

        ''' <summary>SAML2 の RelayState</summary>
        Private Const SessionKeySaml2RelayState As String = "saml2_relay_state"

        ''' <summary>SAML2 の AuthnRequest の ID（応答の InResponseTo と照合する）</summary>
        Private Const SessionKeySaml2RequestId As String = "saml2_request_id"

#End Region

#Region "プロパティ（OIDC の state・nonce）"

        ''' <summary>Nonce</summary>
        Public ReadOnly Property Nonce() As String
            Get
                If Session(HomeController.SessionKeyNonce) Is Nothing Then
                    Session(HomeController.SessionKeyNonce) = GetPassword.Base64UrlSecret(32)
                End If
                Return DirectCast(Session(HomeController.SessionKeyNonce), String)
            End Get
        End Property

        ''' <summary>State</summary>
        Public ReadOnly Property State() As String
            Get
                If Session(HomeController.SessionKeyState) Is Nothing Then
                    Session(HomeController.SessionKeyState) = GetPassword.Base64UrlSecret(32)
                End If
                Return DirectCast(Session(HomeController.SessionKeyState), String)
            End Get
        End Property

#End Region

#Region "画面（Index・Login・Scroll・Logout）"

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
        ''' <returns>ActionResultを非同期的に返す</returns>
        ''' <remarks>
        ''' 押されたボタンで分ける（saml2 → SAML2、external → OIDC、それ以外 → 通常ログイン）。
        ''' </remarks>
        <HttpPost>
        <AllowAnonymous>
        <ValidateAntiForgeryToken>
        Public Async Function Login(model As LoginViewModel) As Task(Of ActionResult)
            If Request.Form.AllKeys.Any(Function(x) x = "saml2") Then
                ' 外部ログイン（SAML2）
                Return Await Me.StartSaml2LoginAsync()
            End If

            If Request.Form.AllKeys.Any(Function(x) x = "external") Then
                ' 外部ログイン（OIDC）
                Return Me.StartOidcLogin()
            End If

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
            Else
                ' LoginViewModelの検証に失敗
            End If

            ' Session消去
            Me.FxSessionAbandon()

            ' ポストバック的な
            Return Me.View(model)
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

#End Region

#Region "外部ログイン（OIDC）"

#Region "要求"

        ''' <summary>OIDC の外部ログインを始める（認可要求へリダイレクトする）</summary>
        ''' <returns>ActionResult</returns>
        ''' <remarks>
        ''' 認可コード フロー。応答は OAuth2AuthorizationCodeGrantClient が受ける。
        ''' **prompt=none は付けない。** IdP にまだセッションが無いと、
        ''' ログイン画面を出さずに login_required で戻ってしまう（OpenID Connect Core 1.0 3.1.2.1）。
        ''' </remarks>
        Private Function StartOidcLogin() As ActionResult
            Return Redirect(CmnClientParams.SpRp_AuthRequestUri _
                & "?client_id=" & Uri.EscapeDataString(OAuth2AndOIDCParams.ClientID) _
                & "&response_type=code" _
                & "&scope=" & Uri.EscapeDataString("openid profile email phone address") _
                & "&state=" & Uri.EscapeDataString(Me.State) _
                & "&nonce=" & Uri.EscapeDataString(Me.Nonce) _
                & "&redirect_uri=" & Uri.EscapeDataString(CmnClientParams.SpRp_RedirectUri))
        End Function

#End Region

#Region "応答（リダイレクト先）"

        ''' <summary>OAuth2AuthorizationCodeGrantClient（OIDC の応答を受ける口。SpRp_RedirectUri）</summary>
        ''' <param name="code">string</param>
        ''' <param name="state">string</param>
        ''' <param name="iss">string（認可応答の iss。RFC 9207）</param>
        ''' <returns>ActionResultを非同期的に返す</returns>
        <HttpGet>
        <AllowAnonymous>
        Public Async Function OAuth2AuthorizationCodeGrantClient(code As String, state As String, iss As String) As Task(Of ActionResult)
            Try
                Dim response As String = ""

                ' CSRF(XSRF)対策のstateの検証は重要。iss は Mix-Up 対策（RFC 9207）
                If Not String.IsNullOrEmpty(code) _
                    AndAlso state = Me.State _
                    AndAlso (String.IsNullOrEmpty(iss) OrElse iss = CmnClientParams.Isser) Then

                    ' **redirect_uri は、認可要求と同じ値を送る**（RFC 6749 4.1.3）
                    response = Await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                        New Uri(CmnClientParams.SpRp_TokenRequestUri),
                        OAuth2AndOIDCParams.ClientID, OAuth2AndOIDCParams.ClientSecret,
                        CmnClientParams.SpRp_RedirectUri, code)

                    Dim token As JObject = JObject.Parse(response)
                    Dim id_token As String = CStr(token(OAuth2AndOIDCConst.IDToken))
                    Dim access_token As String = CStr(token(OAuth2AndOIDCConst.AccessToken))

                    ' id_tokenの検証コード
                    If Not String.IsNullOrEmpty(id_token) Then
                        Dim [sub] As String = Await Me.VerifyIdTokenAsync(id_token, access_token, code)

                        If Not String.IsNullOrEmpty([sub]) Then
                            ' /userinfoエンドポイントにアクセスし、表示名（email）を得る
                            Dim displayName As String = HomeController.GetDisplayName(
                                Await OAuth2AndOIDCClient.GetUserInfoAsync(
                                    New Uri(CmnClientParams.SpRp_UserInfoUri), access_token), [sub])

                            If displayName IsNot Nothing Then ' UserInfo の sub が id_token と同じ（OpenID Connect Core 1.0 5.3.2）
                                ' ログインに成功
                                '   **利用者の識別子は sub**（iss と組で一意・不変。OpenID Connect Core 1.0 5.7）。
                                Me.SignInExternal([sub], displayName)

                                Return New EmptyResult()
                            End If
                        End If
                    End If
                End If

                ' ログインに失敗
                Return RedirectToAction("Login")
            Finally
                Me.ClearExLoginsParams()
            End Try
        End Function

#End Region

#Region "検証"

        ''' <summary>相手（OP）の id_token を検証する</summary>
        ''' <param name="id_token">id_token</param>
        ''' <param name="access_token">access_token（at_hash の検証）</param>
        ''' <param name="code">code（c_hash の検証）</param>
        ''' <returns>sub（検証に失敗したら Nothing）</returns>
        ''' <remarks>
        ''' **鍵は、ヘッダの kid で jwks_uri から引く**（SpRp_DiscoveryUri の Discovery。issuer は SpRp_Isser と照合する）。
        ''' **alg は非対称のもの（JwsFactory.SupportedAlgs）に限る**（none・HS* を受けない）。
        ''' **iss・aud を指定する版の IdToken.Verify を使う**（#601。相手の id_token を、設定の自分の鍵で検証しない）。
        ''' </remarks>
        Private Async Function VerifyIdTokenAsync(id_token As String, access_token As String, code As String) As Task(Of String)
            Dim alg As String = JwtReader.TryReadAlg(id_token)
            If Not JwsFactory.IsSupported(alg) Then Return Nothing

            Dim jwk As JObject = Await RemoteJwkSetStore.GetJwkByDiscoveryAsync(JwtReader.TryReadKid(id_token))
            If jwk Is Nothing Then Return Nothing

            Dim jws As JWS = JwsFactory.CreateFromJwk(alg, jwk)
            If jws Is Nothing Then Return Nothing

            Dim [sub] As String = ""
            Dim nonce As String = ""
            Dim claims As JObject = Nothing
            If IdToken.Verify(id_token, access_token, code, "",
                jws, CmnClientParams.Isser, OAuth2AndOIDCParams.ClientID,
                [sub], nonce, claims) _
                AndAlso nonce = Me.Nonce Then
                Return [sub]
            End If

            Return Nothing
        End Function

        ''' <summary>UserInfo の応答から、表示名を得る</summary>
        ''' <param name="userInfoJson">UserInfo の応答</param>
        ''' <param name="sub">id_token の sub</param>
        ''' <returns>email（無ければ sub）。UserInfo の sub が id_token と違えば Nothing</returns>
        ''' <remarks>
        ''' **UserInfo の sub は、id_token の sub と一致しなければならない**（OpenID Connect Core 1.0 5.3.2）。
        ''' 一致しない応答は使わない（トークンの差し替え）。
        ''' **email は表示にだけ使う。** 利用者の識別子（UserStore のキー）は sub である。
        ''' </remarks>
        Private Shared Function GetDisplayName(userInfoJson As String, [sub] As String) As String
            Dim userInfo As JObject
            Try
                userInfo = JObject.Parse(userInfoJson)
            Catch ex As JsonReaderException
                Return Nothing
            End Try

            If CStr(userInfo(OAuth2AndOIDCConst.[sub])) <> [sub] Then Return Nothing

            Dim email As String = CStr(userInfo("email")) ' email クレーム（OpenID Connect Core 1.0 5.1）
            Return If(String.IsNullOrEmpty(email), [sub], email)
        End Function

        ''' <summary>OIDC の state・nonce を消す（1 度しか使わない）</summary>
        Private Sub ClearExLoginsParams()
            Session(HomeController.SessionKeyNonce) = Nothing
            Session(HomeController.SessionKeyState) = Nothing
        End Sub

#End Region

#End Region

#Region "外部ログイン（SAML2）"

#Region "要求"

        ''' <summary>SAML2 の外部ログインを始める（AuthnRequest を IdP へ送る）</summary>
        ''' <returns>ActionResultを非同期的に返す</returns>
        ''' <remarks>
        ''' 応答は Saml2AssertionConsumerService が受ける。
        ''' **IdP の SSO の口と署名鍵は、メタデータ（SpRp_Saml2MetadataUri）から引く**（entityID は SpRp_Saml2EntityId と照合する）。
        ''' **要求は HTTP-Redirect で送り、応答も HTTP-Redirect で受ける**（ProtocolBinding）。
        ''' **NameID は Persistent を求める**（SP ごとに固定の値。識別子にする。SAML Core 8.3.7）。
        ''' 要求には SpRp_RsaPfxFilePath の鍵で署名する（IdP が WantAuthnRequestsSigned）。
        ''' **Issuer は "http://" ＋ client_id**（汎用認証サイトは、Issuer から client_id を引く）。
        ''' </remarks>
        Private Async Function StartSaml2LoginAsync() As Task(Of ActionResult)
            Dim metadata As SAML2Metadata = Await SAML2Metadata.GetAsync()
            If metadata Is Nothing OrElse String.IsNullOrEmpty(metadata.SsoRedirectLocation) Then
                ' メタデータが引けない（IdP が動いていない、entityID が違う）
                Return RedirectToAction("Login")
            End If

            ' **RelayState と要求の ID を控え、応答と照合する**（CSRF・応答の差し替え）
            Dim relayState As String = GetPassword.Base64UrlSecret(32)
            Dim id As String = ""
            Dim queryString As String = SAML2Client.CreateRedirectRequest(
                SAML2Enum.RequestOrResponse.Request,
                SAML2Enum.ProtocolBinding.HttpRedirect,
                SAML2Enum.NameIDFormat.Persistent,
                "http://" & OAuth2AndOIDCParams.ClientID,
                CmnClientParams.SpRp_Saml2AcsUri, relayState, id)

            Session(HomeController.SessionKeySaml2RelayState) = relayState
            Session(HomeController.SessionKeySaml2RequestId) = id

            Return Redirect(metadata.SsoRedirectLocation & "?" & queryString)
        End Function

#End Region

#Region "応答（ACS）"

        ''' <summary>Saml2AssertionConsumerService（SAML2 の応答を受ける口。SpRp_Saml2AcsUri。HTTP-Redirect）</summary>
        ''' <returns>ActionResultを非同期的に返す</returns>
        ''' <remarks>
        ''' **検証は SAML2Client.VerifyResponseWithReason**（署名・Issuer・Audience・Recipient・InResponseTo・RelayState・期限）。
        ''' **使ってよいかの判断は IsValid だけで行う。**
        ''' </remarks>
        <HttpGet>
        <AllowAnonymous>
        Public Async Function Saml2AssertionConsumerService() As Task(Of ActionResult)
            ' **Redirect Binding は、クエリ文字列そのものが署名の対象**。受け取ったままの値（RawUrl）を使う。
            Dim rawUrl As String = If(Request.RawUrl, "")
            Dim q As Integer = rawUrl.IndexOf("?"c)
            Dim queryString As String = If(q < 0, "", rawUrl.Substring(q + 1))

            ' 1 度しか使わない
            Dim expectedRelayState As String = DirectCast(Session(HomeController.SessionKeySaml2RelayState), String)
            Dim expectedRequestId As String = DirectCast(Session(HomeController.SessionKeySaml2RequestId), String)
            Session(HomeController.SessionKeySaml2RelayState) = Nothing
            Session(HomeController.SessionKeySaml2RequestId) = Nothing

            If String.IsNullOrEmpty(expectedRelayState) OrElse String.IsNullOrEmpty(expectedRequestId) Then
                ' 自分が送った要求への応答ではない
                Return RedirectToAction("Login")
            End If

            ' **署名を検証する鍵は、メタデータの署名鍵**（ロールオーバー中は複数。順に試す）
            Dim metadata As SAML2Metadata = Await SAML2Metadata.GetAsync()
            Dim keys As List(Of RSA) = If(metadata Is Nothing, New List(Of RSA)(), metadata.GetSigningKeys())

            Dim result As SAML2VerifyResult = SAML2Client.VerifyResponseWithReason(
                queryString, Nothing, keys,
                CmnClientParams.SpRp_Saml2EntityId, CmnClientParams.SpRp_Saml2AcsUri,
                expectedRequestId, Request.QueryString("RelayState"), expectedRelayState)

            '  **識別子は NameID**（IdP の entityID と組で使う）。**Transient は受けない**（毎回変わる。SAML Core 8.3.8）。
            If Not result.IsValid _
                OrElse String.IsNullOrEmpty(result.NameId) _
                OrElse result.NameIdFormat = SAML2Enum.NameIDFormat.Transient Then
                ' ログインに失敗
                Return RedirectToAction("Login")
            End If

            ' ログインに成功
            '   **表示名は属性の mail**（無ければ NameID）。
            Dim email As String = SAML2Attributes.GetEmail(result.Attributes)
            Me.SignInExternal(result.NameId, If(String.IsNullOrEmpty(email), result.NameId, email))

            Return New EmptyResult()
        End Function

#End Region

#End Region

#Region "外部ログイン（共通）"

        ''' <summary>外部ログイン（OIDC・SAML2）の結果で、サイン インする</summary>
        ''' <param name="id">利用者の識別子（OIDC は sub、SAML2 は NameID）</param>
        ''' <param name="displayName">表示名（email）</param>
        ''' <remarks>
        ''' **識別子は Forms 認証の名前**（UserStore のキーにするなら、これ）。
        ''' **表示名は MyUserInfo**（変更・再割り当てがありうるので、識別子にしない）。
        ''' Forms 認証の既定の画面（ReturnUrl / defaultUrl）へリダイレクトする（呼び出し側は EmptyResult を返す）。
        ''' </remarks>
        Private Sub SignInExternal(id As String, displayName As String)
            FormsAuthentication.RedirectFromLoginPage(id, False)

            ' 認証情報を保存する（画面・ログに出すユーザ名は、表示名）。
            Dim ui As New MyUserInfo(displayName, Request.UserHostAddress)
            UserInfoHandle.SetUserInformation(ui)
        End Sub

#End Region

    End Class
End Namespace

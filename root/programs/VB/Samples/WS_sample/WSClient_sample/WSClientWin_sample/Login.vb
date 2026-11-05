'**********************************************************************************
'* ３層型 サンプル アプリ画面
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：login
'* クラス日本語名  ：ログイン画面
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*  2026/11/05  玄人 幸道         C# 版に合わせて、外部ログインを ROPC から LIR（認可コード ＋ PKCE、ループバック）に替えた（#649）
'*                                （MPAS のスタブの Device_Client。id_token を検証し、表示名は UserInfo の email）
'**********************************************************************************

Imports System.Collections.Specialized
Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks

Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports Touryo.Infrastructure.Framework.Authentication
Imports Touryo.Infrastructure.Business.RichClient.Presentation
Imports Touryo.Infrastructure.Framework.RichClient.Presentation
Imports Touryo.Infrastructure.Public.Security.Jwt
Imports Touryo.Infrastructure.Public.Security.Pwd
Imports Touryo.Infrastructure.Public.Util


''' <summary>login</summary>
Partial Public Class Login
    Inherits MyBaseControllerWin

    ''' <summary>ブラウザからの応答を待つ上限</summary>
    Private Shared ReadOnly ResponseTimeout As TimeSpan = TimeSpan.FromMinutes(5)

    ''' <summary>コンストラクタ</summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>フォームロードのUOCメソッド</summary>
    Protected Overrides Sub UOC_FormInit()
    End Sub

    ''' <summary>ログイン</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_btnButton1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        MyBaseControllerWin.UserInfo.UserName = Me.textBox1.Text
        MyBaseControllerWin.UserInfo.IPAddress = Environment.MachineName

        Program.FlagEnd = False ' フラグ完了
        Me.Close()
    End Sub

#Region "外部ログイン"

    ''' <summary>外部ログイン</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    ''' <remarks>
    ''' **LIR（OAuth 2.0 for Native Apps のループバック。RFC 8252 §7.3）で、MPAS にログインする。**
    ''' 既定のブラウザで認可し、http://localhost:12345/ で応答を受け、認可コードを PKCE（RFC 7636）で引き換える。
    ''' **ROPC（パスワードを、この画面で受けて IdP に送る）は使わない**（RFC 9700 2.4。MPAS の雛形も無効にしている）。
    ''' そのため、この画面のユーザ ID・パスワードの欄は、外部ログインでは使わない。
    '''
    ''' **ブラウザでの操作を待つので、Async で待つ**（UI スレッドを止めない）。
    ''' 基盤は UOC メソッドを、戻り値を見ずに呼ぶ（Latebind）ので、Async Sub でよい。
    ''' **例外は、ここで受ける**（Async Sub の例外は、基盤の UOC_ABEND に届かない）。
    ''' </remarks>
    Protected Async Sub UOC_btnButton2_Click(rcFxEventArgs As RcFxEventArgs)
        Me.btnButton1.Enabled = False
        Me.btnButton2.Enabled = False

        Try
            Dim result As Tuple(Of String, String) = Await Me.ExLoginAsync()
            If result IsNot Nothing Then
                ' ログインに成功
                '   **表示名は UserInfo の email**（識別子は id_token の sub。WebApp のサンプルと同じ）
                MyBaseControllerWin.UserInfo.UserName = result.Item2
                MyBaseControllerWin.UserInfo.IPAddress = Environment.MachineName

                Program.FlagEnd = False ' フラグ完了
                Program.AccessToken = result.Item1 ' AccessToken（リソース サーバへの Bearer）
                Me.Close()
            End If
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "外部ログイン", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            If Not Me.IsDisposed Then
                Me.btnButton1.Enabled = True
                Me.btnButton2.Enabled = True
            End If
        End Try
    End Sub

    ''' <summary>外部ログイン（LIR）</summary>
    ''' <returns>（access_token, 表示名）。失敗なら Nothing（理由は表示済み）</returns>
    ''' <remarks>
    ''' 接続先とクライアントは app.config にある（MPAS のスタブと、その雛形の Device_Client）。
    ''' **エンドポイントは Discovery から引く**（issuer を SpRp_Isser と照合する）。
    '''
    ''' **確かめること**
    ''' | 値 | 何のため |
    ''' |---|---|
    ''' | `state` | 自分が出した要求への応答か（CSRF。RFC 6749 §10.12） |
    ''' | `iss`（応答に在れば） | どの IdP の応答か（Mix-Up。RFC 9207） |
    ''' | `code_verifier` | コードを横取りされても引き換えられない（RFC 7636） |
    ''' | id_token の署名・`iss`・`aud`・`nonce` | OpenID Connect Core 1.0 §3.1.3.7 |
    ''' | UserInfo の `sub` | id_token の `sub` と同じか（OpenID Connect Core 1.0 §5.3.2） |
    '''
    ''' **アクセス トークンは、ここでは検証しない。** 受けるのはリソース サーバで、クライアントには中身の約束が無い。
    ''' </remarks>
    Private Async Function ExLoginAsync() As Task(Of Tuple(Of String, String))
        OAuth2AndOIDCClient.HttpClient = New HttpClient()

        ' Discovery（**issuer を照合する**。OpenID Connect Discovery 1.0 §4.3）
        Dim json As String = Await OAuth2AndOIDCClient.GetDiscoveryDocumentAsync(New Uri(CmnClientParams.SpRp_DiscoveryUri))
        If String.IsNullOrEmpty(json) Then
            Return Me.Fail("Discovery（" & CmnClientParams.SpRp_DiscoveryUri & "）を読めません。IdP（スタブ）は起動していますか？")
        End If

        Dim discovery As JObject = JObject.Parse(json)
        Dim issuer As String = CStr(discovery(OAuth2AndOIDCConst.issuer))
        If issuer <> CmnClientParams.Isser Then
            Return Me.Fail("Discovery の issuer（" & issuer & "）が、期待する値（" & CmnClientParams.Isser & "）と違います。")
        End If

        ' パラメタ
        '   **state・nonce・code_verifier は、毎回作り直す**（使い回すと、確かめる意味が無くなる）
        Dim client_id As String = OAuth2AndOIDCParams.ClientID
        Dim redirect_uri As String = CmnClientParams.SpRp_RedirectUri
        Dim scope As String = GetConfigParameter.GetConfigValue("SpRp_Scope")
        Dim state As String = GetPassword.Base64UrlSecret(32)
        Dim nonce As String = GetPassword.Base64UrlSecret(32)
        Dim code_verifier As String = GetPassword.Base64UrlSecret(32) ' 43 文字（RFC 7636 §4.1 の下限）
        Dim code_challenge As String = OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod(code_verifier)

        ' **値はエンコードする**（scope の空白など）。**redirect_uri は、トークン要求と同じ値を送る**（RFC 6749 §4.1.3）
        Dim target As String = CStr(discovery("authorization_endpoint")) _
            & "?" & OAuth2AndOIDCConst.client_id & "=" & Uri.EscapeDataString(client_id) _
            & "&" & OAuth2AndOIDCConst.response_type & "=" & OAuth2AndOIDCConst.AuthorizationCodeResponseType _
            & "&" & OAuth2AndOIDCConst.scope & "=" & Uri.EscapeDataString(scope) _
            & "&" & OAuth2AndOIDCConst.redirect_uri & "=" & Uri.EscapeDataString(redirect_uri) _
            & "&" & OAuth2AndOIDCConst.state & "=" & Uri.EscapeDataString(state) _
            & "&" & OAuth2AndOIDCConst.nonce & "=" & Uri.EscapeDataString(nonce) _
            & "&" & OAuth2AndOIDCConst.code_challenge & "=" & Uri.EscapeDataString(code_challenge) _
            & "&" & OAuth2AndOIDCConst.code_challenge_method & "=" & OAuth2AndOIDCConst.PKCE_S256

        ' 認可リクエスト・レスポンス
        Dim query As NameValueCollection = Await Login.AuthorizeInBrowserAsync(target, redirect_uri)
        If query Is Nothing Then
            Return Me.Fail("ブラウザからの応答を受けられませんでした（" & Login.ResponseTimeout.TotalMinutes & " 分）。")
        End If

        If Not String.IsNullOrEmpty(query(OAuth2AndOIDCConst.error)) Then
            Return Me.Fail("認可が断られました : " & query(OAuth2AndOIDCConst.error))
        End If

        If query(OAuth2AndOIDCConst.state) <> state Then
            Return Me.Fail("state が一致しません（自分が出した要求への応答ではない）。")
        End If

        Dim iss As String = query(OAuth2AndOIDCConst.iss)
        If Not String.IsNullOrEmpty(iss) AndAlso iss <> issuer Then
            Return Me.Fail("応答の iss（" & iss & "）が、期待する IdP と違います。")
        End If

        Dim code As String = query(OAuth2AndOIDCConst.code)

        ' Tokenリクエスト（**パブリック クライアントなので、秘密は送らない**）
        Dim token As JObject = JObject.Parse(Await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
            New Uri(CStr(discovery("token_endpoint"))), client_id, "", redirect_uri, code, code_verifier))

        If token.ContainsKey(OAuth2AndOIDCConst.error) Then
            Return Me.Fail("トークン要求が断られました : " & CStr(token(OAuth2AndOIDCConst.error)))
        End If

        Dim access_token As String = CStr(token(OAuth2AndOIDCConst.AccessToken))
        Dim id_token As String = CStr(token(OAuth2AndOIDCConst.IDToken))

        ' id_tokenの検証
        Dim [sub] As String = Nothing
        If Not String.IsNullOrEmpty(id_token) Then
            [sub] = Await Login.VerifyIdTokenAsync(id_token, access_token, code, issuer, client_id, nonce)
        End If
        If [sub] Is Nothing Then
            Return Me.Fail("id_token の検証に失敗しました（無い、署名・iss・aud・exp・ハッシュ・nonce のいずれか）。")
        End If

        ' UserInfoリクエスト（表示名を得る）
        Dim displayName As String = Login.GetDisplayName(Await OAuth2AndOIDCClient.GetUserInfoAsync(
            New Uri(CStr(discovery("userinfo_endpoint"))), access_token), [sub])
        If displayName Is Nothing Then
            Return Me.Fail("UserInfo を得られません（エラーの応答、または sub が id_token と違う）。")
        End If

        Return Tuple.Create(access_token, displayName)
    End Function

    ''' <summary>失敗を表示する</summary>
    ''' <param name="message">理由</param>
    ''' <returns>Nothing</returns>
    Private Function Fail(message As String) As Tuple(Of String, String)
        MessageBox.Show(Me, message, "外部ログイン", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Return Nothing
    End Function

    ''' <summary>ブラウザで認可要求を出し、ループバックで応答を受ける</summary>
    ''' <param name="target">認可要求の URL</param>
    ''' <param name="redirect_uri">戻り先（ループバック）</param>
    ''' <returns>応答のクエリ（待ちきれなければ Nothing）</returns>
    ''' <remarks>
    ''' **戻り先は IdP に登録した値と、1 文字も違ってはならない**（末尾の / も含む）。
    ''' **ブラウザを開く前に待ち受ける**（先に開くと、応答を取りこぼすことがある）。
    ''' 待ち受けられない（ポートが使用中など）ときは、HttpListenerException が上がる。
    ''' </remarks>
    Private Shared Async Function AuthorizeInBrowserAsync(target As String, redirect_uri As String) As Task(Of NameValueCollection)
        Dim listener As New HttpListener()
        listener.Prefixes.Add(redirect_uri)

        Try
            listener.Start()

            ' **UseShellExecute で既定のブラウザに渡す**（cmd /c start は & を解釈してしまう）
            Process.Start(New ProcessStartInfo(target) With {.UseShellExecute = True})

            Dim getContext As Task(Of HttpListenerContext) = listener.GetContextAsync()
            If Await Task.WhenAny(getContext, Task.Delay(Login.ResponseTimeout)) IsNot getContext Then
                Return Nothing
            End If

            Dim context As HttpListenerContext = getContext.Result
            Dim query As NameValueCollection = context.Request.QueryString

            ' 返信（**結果だけを返す。トークンは画面に出さない**）
            Dim buffer As Byte() = Encoding.UTF8.GetBytes(
                "<HTML><BODY>Accepted the authorization response. You can close this tab.</BODY></HTML>")
            context.Response.ContentLength64 = buffer.Length
            context.Response.OutputStream.Write(buffer, 0, buffer.Length)
            context.Response.OutputStream.Close()

            Return query
        Finally
            listener.Close()
        End Try
    End Function

    ''' <summary>相手（OP）の id_token を、jwks_uri の鍵で検証する</summary>
    ''' <param name="id_token">id_token</param>
    ''' <param name="access_token">access_token（at_hash の検証）</param>
    ''' <param name="code">code（c_hash の検証）</param>
    ''' <param name="issuer">期待する iss</param>
    ''' <param name="client_id">期待する aud</param>
    ''' <param name="expectedNonce">認可要求で送った nonce</param>
    ''' <returns>sub（検証に失敗したら Nothing）</returns>
    ''' <remarks>
    ''' **鍵は、ヘッダの kid で jwks_uri から引く**（RemoteJwkSetStore。SpRp_DiscoveryUri の Discovery）。
    ''' **alg は非対称のもの（JwsFactory.SupportedAlgs）に限る**（none・HS* を受けない）。
    ''' </remarks>
    Private Shared Async Function VerifyIdTokenAsync(
        id_token As String, access_token As String, code As String,
        issuer As String, client_id As String, expectedNonce As String) As Task(Of String)

        Dim alg As String = JwtReader.TryReadAlg(id_token)
        If Not JwsFactory.IsSupported(alg) Then Return Nothing

        Dim jwk As JObject = Await RemoteJwkSetStore.GetJwkByDiscoveryAsync(JwtReader.TryReadKid(id_token))
        If jwk Is Nothing Then Return Nothing

        Dim jws As JWS = JwsFactory.CreateFromJwk(alg, jwk)
        If jws Is Nothing Then Return Nothing

        ' **署名・iss・aud・exp と、at_hash / c_hash を確かめる**（IdToken.Verify。#601）
        Dim [sub] As String = ""
        Dim nonce As String = ""
        Dim claims As JObject = Nothing
        If Not IdToken.Verify(id_token, access_token, code, "",
            jws, issuer, client_id, [sub], nonce, claims) Then
            Return Nothing
        End If

        ' **nonce は、呼ぶ側が確かめる**（IdToken.Verify は取り出すだけ。OpenID Connect Core 1.0 §3.1.3.7 の 11）
        Return If(nonce = expectedNonce, [sub], Nothing)
    End Function

    ''' <summary>UserInfo の応答から、表示名を得る</summary>
    ''' <param name="userInfoJson">UserInfo の応答</param>
    ''' <param name="sub">id_token の sub</param>
    ''' <returns>email（無ければ sub）。エラーの応答や、sub が違えば Nothing</returns>
    ''' <remarks>
    ''' **エラーの応答も JSON で返る**（insufficient_scope など。RFC 6750 §3.1）。
    ''' **UserInfo の sub は、id_token の sub と一致しなければならない**（OpenID Connect Core 1.0 §5.3.2）。
    ''' </remarks>
    Private Shared Function GetDisplayName(userInfoJson As String, [sub] As String) As String
        Dim userInfo As JObject
        Try
            userInfo = JObject.Parse(userInfoJson)
        Catch ex As JsonReaderException
            Return Nothing
        End Try

        If userInfo.ContainsKey(OAuth2AndOIDCConst.error) Then Return Nothing
        If CStr(userInfo(OAuth2AndOIDCConst.[sub])) <> [sub] Then Return Nothing

        Dim email As String = CStr(userInfo("email")) ' email クレーム（OpenID Connect Core 1.0 §5.1）
        Return If(String.IsNullOrEmpty(email), [sub], email)
    End Function

#End Region

End Class

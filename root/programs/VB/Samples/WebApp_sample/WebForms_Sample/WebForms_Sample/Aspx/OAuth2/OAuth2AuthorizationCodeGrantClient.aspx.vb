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
'*  2026/11/04  玄人 幸道         C# 版に合わせて、今のライブラリと MPAS のスタブに合わせた（#649）
'*                                （トークン要求に redirect_uri・id_token を jwks_uri の鍵と iss / aud で検証・
'*                                  エラー応答で落ちない・表示名を UserInfo の email に）
'**********************************************************************************

Imports System.Threading.Tasks

Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports Touryo.Infrastructure.Framework.Authentication
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Security.Jwt
Imports Touryo.Infrastructure.Public.Security.Pwd

Namespace Aspx.OAuth2
    ''' <summary>認証画面</summary>
    Public Class OAuth2AuthorizationCodeGrantClient
        Inherits System.Web.UI.Page

#Region "プロパティ（OIDC の state・nonce）"

        ''' <summary>Nonce</summary>
        Public ReadOnly Property Nonce() As String
            Get
                If Session("nonce") Is Nothing Then
                    Session("nonce") = GetPassword.Base64UrlSecret(32)
                End If
                Return DirectCast(Session("nonce"), String)
            End Get
        End Property

        ''' <summary>State</summary>
        Public ReadOnly Property State() As String
            Get
                If Session("state") Is Nothing Then
                    Session("state") = GetPassword.Base64UrlSecret(32)
                End If
                Return DirectCast(Session("state"), String)
            End Get
        End Property

#End Region

#Region "応答（リダイレクト先）"

        ''' <summary>Page_Load</summary>
        ''' <param name="sender">object</param>
        ''' <param name="e">EventArgs</param>
        Protected Async Sub Page_Load(sender As Object, e As EventArgs)
            Dim code As String = Request.QueryString("code")
            Dim state As String = Request.QueryString("state")
            Dim iss As String = Request.QueryString("iss") ' 認可応答の iss（RFC 9207）

            Try
                Dim response__1 As String = ""

                ' CSRF(XSRF)対策のstateの検証は重要。iss は Mix-Up 対策（RFC 9207）
                If Not String.IsNullOrEmpty(code) _
                    AndAlso state = Me.State _
                    AndAlso (String.IsNullOrEmpty(iss) OrElse iss = CmnClientParams.Isser) Then

                    ' **redirect_uri は、認可要求と同じ値を送る**（RFC 6749 4.1.3）
                    response__1 = Await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                        New Uri(CmnClientParams.SpRp_TokenRequestUri),
                        OAuth2AndOIDCParams.ClientID, OAuth2AndOIDCParams.ClientSecret,
                        CmnClientParams.SpRp_RedirectUri, code)

                    ' **エラーの応答には id_token が無い**（以前は、ここで KeyNotFoundException になっていた）
                    Dim token As JObject = JObject.Parse(response__1)
                    Dim id_token As String = CStr(token(OAuth2AndOIDCConst.IDToken))
                    Dim access_token As String = CStr(token(OAuth2AndOIDCConst.AccessToken))

                    ' id_tokenの検証
                    Dim [sub] As String = Nothing
                    If Not String.IsNullOrEmpty(id_token) Then
                        [sub] = Await Me.VerifyIdTokenAsync(id_token, access_token, code)
                    End If

                    ' /userinfoエンドポイントにアクセスし、表示名（email）を得る
                    '   UserInfo の sub が id_token と違えば Nothing（OpenID Connect Core 1.0 5.3.2）
                    Dim displayName As String = Nothing
                    If Not String.IsNullOrEmpty([sub]) Then
                        displayName = OAuth2AuthorizationCodeGrantClient.GetDisplayName(
                            Await OAuth2AndOIDCClient.GetUserInfoAsync(
                                New Uri(CmnClientParams.SpRp_UserInfoUri), access_token), [sub])
                    End If

                    If displayName IsNot Nothing Then
                        ' ログインに成功

                        '   **利用者の識別子は sub**（Forms 認証の名前。iss と組で一意・不変。OpenID Connect Core 1.0 5.7）。
                        '   **email は表示名**（MyUserInfo。変更・再割り当てがありうるので、識別子にしない）。
                        FormsAuthentication.RedirectFromLoginPage([sub], False)
                        Dim ui As New MyUserInfo(displayName, Request.UserHostAddress)
                        UserInfoHandle.SetUserInformation(ui)

                        Return
                    End If
                End If

                'ResolveClientUrlがInvalidOperationExceptionを吐くので...
                '' ログインに失敗
                'Response.Redirect("../Start/login.aspx")
            Finally
                Me.ClearExLoginsParams()
            End Try
        End Sub

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
            Session("nonce") = Nothing
            Session("state") = Nothing
        End Sub

#End Region

    End Class
End Namespace

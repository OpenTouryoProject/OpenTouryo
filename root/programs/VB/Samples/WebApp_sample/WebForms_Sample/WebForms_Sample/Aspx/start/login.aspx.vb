'**********************************************************************************
'* サンプル画面（Ｐ層）
'**********************************************************************************

' サンプル画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：login
'* クラス日本語名  ：ログイン画面（Forms認証対応）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*  2026/11/04  玄人 幸道         C# 版に合わせて、外部ログインの要求を MPAS のスタブに合わせた（#649）
'*                                （prompt=none を外す・値をエンコード・SAML2 の外部ログイン（btnButton3）を追加）
'*  2026/11/05  玄人 幸道         外部ログインの IdP へは、基盤の画面遷移を通さずに送る（ウィンドウ GUID を付け足さない。#649）
'**********************************************************************************

Imports System.Threading.Tasks

Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Authentication
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Security.Pwd

Namespace Aspx.Start
    ''' <summary>ログイン画面（Forms認証対応）</summary>
    Partial Public Class login
        Inherits MyBaseController

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

#Region "コンストラクタ"

        ''' <summary>constructor</summary>
        Public Sub New()
            Me.IsNoSession = True
        End Sub

#End Region

#Region "Page LoadのUOCメソッド"

        ''' <summary>
        ''' Page LoadのUOCメソッド（個別：初回Load）
        ''' </summary>
        ''' <remarks>
        ''' 実装必須
        ''' </remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する

            ' TODO:
            ' ここでは何もしない

            ' Session消去
            Me.FxSessionAbandon()
        End Sub

        ''' <summary>
        ''' Page LoadのUOCメソッド（個別：Post Back）
        ''' </summary>
        ''' <remarks>
        ''' 実装必須
        ''' </remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する

            ' TODO:
            ' ここでは何もしない

            ' btnButton1, 2, 3のイベントであれば、Session消去しない
            If Request.Form("ctl00$ContentPlaceHolder_A$btnButton1") Is Nothing _
                AndAlso Request.Form("ctl00$ContentPlaceHolder_A$btnButton2") Is Nothing _
                AndAlso Request.Form("ctl00$ContentPlaceHolder_A$btnButton3") Is Nothing Then
                ' Session消去
                Me.FxSessionAbandon()
            End If
        End Sub

#End Region

#Region "Event Handler"

        ''' <summary>ログイン</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
            ' ここで、入力されたユーザIDと、パスワードをチェックし、ユーザ認証する。

            If Not String.IsNullOrEmpty(Me.txtUserID.Text) Then
                ' 現時点では、全て(空文字以外)認証する。
                ' 認証か完了した場合、認証チケットを生成し、元のページにRedirectする。
                ' 第２引数は、「クライアントがCookieを永続化（ファイルとして保存）するかどうか。」
                ' を設定する引数であるが、セキュリティを考慮して、falseの設定を勧める。
                FormsAuthentication.RedirectFromLoginPage(Me.txtUserID.Text, False)

                ' 認証情報を保存する。
                Dim ui As New MyUserInfo(Me.txtUserID.Text, Request.UserHostAddress)
                UserInfoHandle.SetUserInformation(ui)
            Else
                ' 認証に失敗した場合は、Messageを表示する
                Me.lblMessage.Text = "認証に失敗しました。ユーザIDか、パスワードが間違っています。"

                ' Session消去
                Me.FxSessionAbandon()
            End If

            ' 画面遷移はしない（基盤に任せるため）。
            Return String.Empty
        End Function

        ''' <summary>外部ログイン</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>
        ''' OIDC の認可コード フロー。応答は OAuth2/OAuth2AuthorizationCodeGrantClient.aspx が受ける。
        ''' **prompt=none は付けない。** IdP にまだセッションが無いと、
        ''' ログイン画面を出さずに login_required で戻ってしまう（OpenID Connect Core 1.0 3.1.2.1）。
        ''' **IdP へは Response.Redirect で直接送る**（RedirectToIdP。基盤の画面遷移はウィンドウ GUID を付け足すため）。
        ''' </remarks>
        Protected Function UOC_btnButton2_Click(fxEventArgs As FxEventArgs) As String
            Return Me.RedirectToIdP(CmnClientParams.SpRp_AuthRequestUri _
                & "?client_id=" & Uri.EscapeDataString(OAuth2AndOIDCParams.ClientID) _
                & "&response_type=code" _
                & "&scope=" & Uri.EscapeDataString("openid profile email phone address roles") _
                & "&state=" & Uri.EscapeDataString(Me.State) _
                & "&nonce=" & Uri.EscapeDataString(Me.Nonce) _
                & "&redirect_uri=" & Uri.EscapeDataString(CmnClientParams.SpRp_RedirectUri))
        End Function

        ''' <summary>外部ログイン（SAML2）</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        ''' <remarks>
        ''' 応答は Saml2/Saml2AssertionConsumerService.aspx が受ける。
        ''' **IdP の SSO の口は、メタデータ（SpRp_Saml2MetadataUri）から引く**（entityID は SpRp_Saml2EntityId と照合する）。
        ''' **要求は HTTP-Redirect で送り、応答も HTTP-Redirect で受ける**（ProtocolBinding）。
        ''' **NameID は Persistent を求める**（SP ごとに固定の値。識別子にする。SAML Core 8.3.7）。
        ''' 要求には SpRp_RsaPfxFilePath の鍵で署名する（IdP が WantAuthnRequestsSigned）。
        ''' **Issuer は "http://" ＋ client_id**（汎用認証サイトは、Issuer から client_id を引く）。
        ''' **IdP へは Response.Redirect で直接送る**（RedirectToIdP）。基盤の画面遷移は、署名した URL の後ろに
        ''' ウィンドウ GUID（BrowserWindowGUID）を付け足し、IdP が全体を署名の対象として扱うと検証に失敗するため（#649）。
        ''' </remarks>
        Protected Function UOC_btnButton3_Click(fxEventArgs As FxEventArgs) As String
            ' **イベント ハンドラは同期なので、別スレッドで待つ**（同期コンテキストでの .Result はデッドロックしうる）
            Dim metadata As SAML2Metadata = Task.Run(Function() SAML2Metadata.GetAsync()).Result
            If metadata Is Nothing OrElse String.IsNullOrEmpty(metadata.SsoRedirectLocation) Then
                ' メタデータが引けない（IdP が動いていない、entityID が違う）
                Me.lblMessage.Text = "SAML2 の IdP のメタデータを取得できませんでした。"
                Return String.Empty
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

            Session("saml2_relay_state") = relayState
            Session("saml2_request_id") = id

            Return Me.RedirectToIdP(metadata.SsoRedirectLocation & "?" & queryString)
        End Function

        ''' <summary>外部の IdP へ、URL をそのまま送る（基盤の画面遷移を通さない）</summary>
        ''' <param name="url">IdP の URL（クエリ文字列を含む）</param>
        ''' <returns>空文字（基盤の画面遷移はしない）</returns>
        ''' <remarks>
        ''' 基盤の画面遷移（FxRedirect）は、遷移先の URL にウィンドウ GUID（BrowserWindowGUID）を付け足す。
        ''' 外部の IdP には要らない値で、**SAML2 の Redirect Binding では、署名した URL を変えてしまう**（#649）。
        ''' **例外を使わない形で送る**（Redirect(url, False) と CompleteRequest）。イベント ハンドラはレイトバインドで呼ばれるので、
        ''' Response.Redirect(url) の ThreadAbortException が別の例外に包まれないようにする。
        ''' </remarks>
        Private Function RedirectToIdP(url As String) As String
            Me.Response.Redirect(url, False)
            Me.Context.ApplicationInstance.CompleteRequest()
            Return String.Empty
        End Function

#End Region
    End Class
End Namespace

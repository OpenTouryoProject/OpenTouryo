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
'* クラス名        ：Saml2AssertionConsumerService
'* クラス日本語名  ：SAML2 の応答を受ける画面（ACS。HTTP-Redirect）
'*
'* 作成者          ：玄人 幸道
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2026/11/04  玄人 幸道         新規作成（C# 版から移植。#649）
'*  2026/11/14  玄人 幸道         Audience を SP の entityID（SpRp_Saml2SpEntityId）と照合する版に替えた（空なら従来どおり ACS の URL。#693）
'**********************************************************************************

Imports System.Security.Cryptography

Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Authentication
Imports Touryo.Infrastructure.Framework.Util

Namespace Aspx.Saml2
    ''' <summary>SAML2 の応答を受ける画面（ACS。HTTP-Redirect）</summary>
    ''' <remarks>
    ''' 要求は Start/login.aspx（btnButton3）が出す。
    ''' **検証は SAML2Client.VerifyResponseWithReason**（署名・Issuer・Audience・Recipient・InResponseTo・RelayState・期限）。
    ''' **使ってよいかの判断は IsValid だけで行う。**
    ''' **Forms 認証の対象外にする**（Web.config の location。未認証で受けるため）。
    ''' </remarks>
    Public Class Saml2AssertionConsumerService
        Inherits System.Web.UI.Page

#Region "応答（ACS）"

        ''' <summary>Page_Load</summary>
        ''' <param name="sender">object</param>
        ''' <param name="e">EventArgs</param>
        Protected Async Sub Page_Load(sender As Object, e As EventArgs)
            ' **Redirect Binding は、クエリ文字列そのものが署名の対象**。受け取ったままの値（RawUrl）を使う。
            Dim rawUrl As String = If(Request.RawUrl, "")
            Dim q As Integer = rawUrl.IndexOf("?"c)
            Dim queryString As String = If(q < 0, "", rawUrl.Substring(q + 1))

            ' 1 度しか使わない
            Dim expectedRelayState As String = DirectCast(Session("saml2_relay_state"), String)
            Dim expectedRequestId As String = DirectCast(Session("saml2_request_id"), String)
            Session("saml2_relay_state") = Nothing
            Session("saml2_request_id") = Nothing

            If String.IsNullOrEmpty(expectedRelayState) OrElse String.IsNullOrEmpty(expectedRequestId) Then
                ' 自分が送った要求への応答ではない
                Return
            End If

            ' **署名を検証する鍵は、メタデータの署名鍵**（ロールオーバー中は複数。順に試す）
            Dim metadata As SAML2Metadata = Await SAML2Metadata.GetAsync()
            Dim keys As List(Of RSA) = If(metadata Is Nothing, New List(Of RSA)(), metadata.GetSigningKeys())

            ' **Audience は、自分（SP）の entityID（SpRp_Saml2SpEntityId）と照合する**（SAML Profiles 4.1.4.2。空なら、従来どおり ACS の URL と。#693）
            Dim result As SAML2VerifyResult = SAML2Client.VerifyResponseWithReason(
                queryString, Nothing, keys,
                CmnClientParams.SpRp_Saml2EntityId, CmnClientParams.SpRp_Saml2SpEntityId, CmnClientParams.SpRp_Saml2AcsUri,
                expectedRequestId, Request.QueryString("RelayState"), expectedRelayState, Nothing)

            '  **識別子は NameID**（IdP の entityID と組で使う）。**Transient は受けない**（毎回変わる。SAML Core 8.3.8）。
            If Not result.IsValid _
                OrElse String.IsNullOrEmpty(result.NameId) _
                OrElse result.NameIdFormat = SAML2Enum.NameIDFormat.Transient Then
                ' ログインに失敗（OAuth2/OAuth2AuthorizationCodeGrantClient.aspx と同じく、何も出さない）
                Return
            End If

            ' ログインに成功
            '   **識別子は NameID**（Forms 認証の名前）。**表示名は属性の mail**（MyUserInfo。無ければ NameID）。
            Dim email As String = SAML2Attributes.GetEmail(result.Attributes)
            FormsAuthentication.RedirectFromLoginPage(result.NameId, False)
            Dim ui As New MyUserInfo(If(String.IsNullOrEmpty(email), result.NameId, email), Request.UserHostAddress)
            UserInfoHandle.SetUserInformation(ui)
        End Sub

#End Region

    End Class
End Namespace

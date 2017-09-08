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
'**********************************************************************************

Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks

Imports Microsoft.Owin.Security.DataHandler.Encoder

Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Business.RichClient.Presentation
Imports Touryo.Infrastructure.Framework.RichClient.Presentation


''' <summary>login</summary>
Partial Public Class Login
    Inherits MyBaseControllerWin

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

    ''' <summary>外部ログイン</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_btnButton2_Click(rcFxEventArgs As RcFxEventArgs)
        Dim access_token As String = Me.ExLogin(Me.textBox1.Text, Me.textBox2.Text).Result
        If Not String.IsNullOrEmpty(access_token) Then
            MyBaseControllerWin.UserInfo.UserName = Me.textBox1.Text
            MyBaseControllerWin.UserInfo.IPAddress = Environment.MachineName

            Program.FlagEnd = False
            ' フラグ完了
            Program.AccessToken = access_token
            ' AccessToken
            Me.Close()
        End If
    End Sub

    ''' <summary>外部ログイン</summary>
    ''' <param name="userId">string</param>
    ''' <param name="password">string</param>
    ''' <returns>access_token</returns>
    Private Async Function ExLogin(userId As String, password As String) As Task(Of String)
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
            {"grant_type", "password"},
            {"username", userId},
            {"password", password},
            {"scope", "profile email phone address roles"}
        })

        ' HttpResponseMessage
        httpResponseMessage = Await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(False)
        Dim response As String = Await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(False)

        ' 汎用認証サイトはOIDCをサポートしたのでid_tokenを取得し、検証可能。
        Dim base64UrlEncoder As New Base64UrlTextEncoder()
        Dim dic As Dictionary(Of String, String) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(response)

        ' access_tokenの検証コード
        If dic.ContainsKey("access_token") Then
            Dim access_token As String = dic("access_token")

            Dim [sub] As String = ""
            Dim roles As List(Of String) = Nothing
            Dim scopes As List(Of String) = Nothing
            Dim jobj As JObject = Nothing

            If JwtToken.Verify(access_token, [sub], roles, scopes, jobj) Then
                ' ログインに成功
                Return access_token
            Else
                ' ログインに失敗
                Return ""
            End If
        Else
            ' ログインに失敗
            Return ""
        End If
    End Function

End Class

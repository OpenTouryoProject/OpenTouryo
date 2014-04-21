'**********************************************************************************
'* サンプル
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Aspx_Start_login
'* クラス日本語名  ：ログイン画面（Forms認証対応）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

' System～
Imports System
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic

' System.Web
Imports System.Web
Imports System.Web.Security

Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

''' <summary>ログイン画面（Forms認証対応）</summary>
Public Partial Class Aspx_Start_login
	Inherits MyBaseController
	Public Sub New()
		Me.IsNoSession = True
	End Sub

	#Region "ページロードのUOCメソッド"

	''' <summary>
	''' ページロードのUOCメソッド（個別：初回ロード）
	''' </summary>
	''' <remarks>
	''' 実装必須
	''' </remarks>
	Protected Overrides Sub UOC_FormInit()
		' フォーム初期化（初回ロード）時に実行する処理を実装する

		' TODO:
		' ここでは何もしない

		' Session消去
		Me.FxSessionAbandon()

	End Sub

	''' <summary>
	''' ページロードのUOCメソッド（個別：ポストバック）
	''' </summary>
	''' <remarks>
	''' 実装必須
	''' </remarks>
	Protected Overrides Sub UOC_FormInit_PostBack()
		' フォーム初期化（ポストバック）時に実行する処理を実装する

		' TODO:
		' ここでは何もしない

		' btnButton1のイベントであれば、Session消去しない
		If Request.Form("ctl00$ContentPlaceHolder_A$btnButton1") Is Nothing Then
			' Session消去
			Me.FxSessionAbandon()
		End If
	End Sub

	#End Region

	#Region "イベントハンドラ"

	''' <summary>ログイン</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
		' ここで、入力されたユーザIDと、パスワードをチェックし、ユーザ認証する。

		If Me.txtUserID.Text <> "" Then
			' 現時点では、全て(空文字以外)認証する。
			' 認証か完了した場合、認証チケットを生成し、元のページにRedirectする。
			' 第２引数は、「クライアントがCookieを永続化（ファイルとして保存）するかどうか。」
			' を設定する引数であるが、セキュリティを考慮して、falseの設定を勧める。
			FormsAuthentication.RedirectFromLoginPage(Me.txtUserID.Text, False)

			' 認証情報を保存する。
			Dim ui As New MyUserInfo(Me.txtUserID.Text, Request.UserHostAddress)
			UserInfoHandle.SetUserInformation(ui)

			' 認証Sessionの場合のテスト
			Session("test") = "test"
		Else
			' 認証に失敗した場合は、メッセージを表示する
			Me.lblMessage.Text = "認証に失敗しました。ユーザIDか、パスワードが間違っています。"

			' Session消去
			Me.FxSessionAbandon()
		End If

		' 画面遷移はしない（基盤に任せるため）。
		Return String.Empty
	End Function

	'
'    /// <summary>ログイン</summary>
'    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
'    /// <returns>URL</returns>
'    protected string UOC_btnButton2_Click(FxEventArgs fxEventArgs)
'    {
'        // ここで、入力されたユーザIDと、パスワードをチェックし、ユーザ認証する。
'
'        if (this.txtUserID.Text != "")  // 現時点では、全て(空文字以外)認証する。
'        {
'            // 認証か完了した場合、認証チケットを生成し、元のページにRedirectする。
'            // 第２引数は、「クライアントがCookieを永続化（ファイルとして保存）するかどうか。」
'            // を設定する引数であるが、セキュリティを考慮して、falseの設定を勧める。
'            FormsAuthentication.RedirectFromLoginPage(this.txtUserID.Text, false);
'
'            // 認証情報を保存する。
'            MyUserInfo ui = new MyUserInfo(this.txtUserID.Text, Request.UserHostAddress);
'            UserInfoHandle.SetUserInformation(ui);
'
'            // 認証Sessionの場合のテスト
'            Session["test"] = "test";
'
'            // 画面遷移制御機能を使用する。
'            // （mode：Tで遷移するとエラー）
'            return "menu";
'        }
'        else
'        {
'            // 認証に失敗した場合は、メッセージを表示する
'            this.lblMessage.Text = "認証に失敗しました。ユーザIDか、パスワードが間違っています。";
'
'            // Session消去
'            this.FxSessionAbandon();
'        }
'
'        // 画面遷移はしない
'        return string.Empty;
'    }
'    


	#End Region
End Class

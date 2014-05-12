'**********************************************************************************
'* フレームワーク・テスト画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Aspx_testFxLayerP_withAjax_testExtension_Single
'* クラス日本語名  ：ASP.NET AJAX Extensionのテスト画面（Ｐ層）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

' System
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

''' <summary>ASP.NET AJAX Extensionのテスト画面（Ｐ層）</summary>
Public Partial Class Aspx_testFxLayerP_withAjax_testExtension_Single
	Inherits MyBaseController
	#Region "ページロードのUOCメソッド"

	''' <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
	''' <remarks>実装必須</remarks>
	Protected Overrides Sub UOC_FormInit()
		' フォーム初期化（初回ロード）時に実行する処理を実装する
		' TODO:

		' ScriptManagerにコントロールの動作を指定する。
		' Init、PostBackの双方で都度実行する必要がある。
		Me.InitScriptManagerRegister()
	End Sub

	''' <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
	''' <remarks>実装必須</remarks>
	Protected Overrides Sub UOC_FormInit_PostBack()
		' フォーム初期化（ポストバック）時に実行する処理を実装する
		' TODO:

		' ScriptManagerにコントロールの動作を指定する。
		' Init、PostBackの双方で都度実行する必要がある。
		Me.InitScriptManagerRegister()
	End Sub

	''' <summary>
	''' ScriptManagerにコントロールの動作を指定する。
	''' </summary>
	Private Sub InitScriptManagerRegister()
		' RegisterPostBackControlメソッドで、
		' ・btnButton2
		' ・ddlDropDownList2
		' を非Ajax化する。

		' ※ 逆の動作は、RegisterAsyncPostBackControlになる。

		Me.CurrentScriptManager.RegisterPostBackControl(Me.GetContentWebControl("btnButton2"))
		Me.CurrentScriptManager.RegisterPostBackControl(Me.GetContentWebControl("ddlDropDownList2"))
	End Sub

	#End Region

	#Region "マスタ ページ上のフレームワーク対象コントロール"

	''' <summary>btnMButton4のクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_testAspNetAjaxExtension_Single_btnMButton4_Click(fxEventArgs As FxEventArgs) As String
		' 待機する（UpdateProgress、二重送信確認用）
		System.Threading.Thread.Sleep(3000)

		' テキストボックスの値を変更
		Dim textBox As TextBox = DirectCast(Me.GetMasterWebControl("TextBox5"), TextBox)
		textBox.Text = "ajaxのポストバック（ボタンクリック）"

		' ajaxのイベントハンドラでは画面遷移しないこと。
		Return ""
	End Function

	''' <summary>btnMButton5のクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_testAspNetAjaxExtension_Single_btnMButton5_Click(fxEventArgs As FxEventArgs) As String
		' 待機する（二重送信確認用）
		System.Threading.Thread.Sleep(3000)

		' テキストボックスの値を変更
		Dim textBox As TextBox = DirectCast(Me.GetMasterWebControl("TextBox6"), TextBox)
		textBox.Text = "通常のポストバック（ボタンクリック）"

		Return ""
	End Function

	''' <summary>
	''' ddlMDropDownList3のSelectedIndexChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_testAspNetAjaxExtension_Single_ddlMDropDownList3_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
		' 待機する（UpdateProgress、二重送信確認用）
		System.Threading.Thread.Sleep(3000)

		' テキストボックスの値を変更
		Dim textBox As TextBox = DirectCast(Me.GetMasterWebControl("TextBox7"), TextBox)
		textBox.Text = "ajaxのポストバック（ＤＤＬのセレクト インデックス チェンジ）"

		' ajaxのイベントハンドラでは画面遷移しないこと。
		Return ""
	End Function

	''' <summary>
	''' ddlMDropDownList4のSelectedIndexChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_testAspNetAjaxExtension_Single_ddlMDropDownList4_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
		' 待機する（二重送信確認用）
		System.Threading.Thread.Sleep(3000)

		' テキストボックスの値を変更
		Dim textBox As TextBox = DirectCast(Me.GetMasterWebControl("TextBox8"), TextBox)
		textBox.Text = "通常のポストバック（ＤＤＬのセレクト インデックス チェンジ）"

		Return ""
	End Function

	''' <summary>btnMButton6のクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_testAspNetAjaxExtension_Single_btnMButton6_Click(fxEventArgs As FxEventArgs) As String
		' 待機する（二重送信確認用）
		System.Threading.Thread.Sleep(3000)

		Throw New Exception("Ajaxでエラー")

		'return "";
	End Function

	#End Region

	#Region "コンテンツ ページ上のフレームワーク対象コントロール"

	''' <summary>btnButton1のクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
		' Ajaxを制御する場合は、ScriptManagerを使用する。
		' このクラスを使用すると、Ajax中であるかどうかを判別できる。
		Dim isInAsyncPostBack As Boolean = Me.CurrentScriptManager.IsInAsyncPostBack
		Dim ajaxES As FxEnum.AjaxExtStat = Me.AjaxExtensionStatus

		' 待機する（UpdateProgress、二重送信確認用）
		System.Threading.Thread.Sleep(3000)

		' テキストボックスの値を変更
		Dim textBox As TextBox = DirectCast(Me.GetContentWebControl("TextBox1"), TextBox)
		textBox.Text = "ajaxのポストバック（ボタンクリック）"

		' ajaxのイベントハンドラでは画面遷移しないこと。
		Return ""
	End Function

	''' <summary>btnButton2のクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton2_Click(fxEventArgs As FxEventArgs) As String
		' Ajaxを制御する場合は、ScriptManagerを使用する。
		' このクラスを使用すると、Ajax中であるかどうかを判別できる。
		Dim isInAsyncPostBack As Boolean = Me.CurrentScriptManager.IsInAsyncPostBack
		Dim ajaxES As FxEnum.AjaxExtStat = Me.AjaxExtensionStatus

		' 待機する（二重送信確認用）
		System.Threading.Thread.Sleep(3000)

		' テキストボックスの値を変更
		Dim textBox As TextBox = DirectCast(Me.GetContentWebControl("TextBox2"), TextBox)
		textBox.Text = "通常のポストバック（ボタンクリック）"

		Return ""
	End Function

	''' <summary>
	''' ddlDropDownList1のSelectedIndexChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_ddlDropDownList1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
		' Ajaxを制御する場合は、ScriptManagerを使用する。
		' このクラスを使用すると、Ajax中であるかどうかを判別できる。
		Dim isInAsyncPostBack As Boolean = Me.CurrentScriptManager.IsInAsyncPostBack
		Dim ajaxES As FxEnum.AjaxExtStat = Me.AjaxExtensionStatus

		' 待機する（UpdateProgress、二重送信確認用）
		System.Threading.Thread.Sleep(3000)

		' テキストボックスの値を変更
		Dim textBox As TextBox = DirectCast(Me.GetContentWebControl("TextBox3"), TextBox)
		textBox.Text = "ajaxのポストバック（ＤＤＬのセレクト インデックス チェンジ）"

		' ajaxのイベントハンドラでは画面遷移しないこと。
		Return ""
	End Function

	''' <summary>
	''' ddlDropDownList2のSelectedIndexChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_ddlDropDownList2_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
		' Ajaxを制御する場合は、ScriptManagerを使用する。
		' このクラスを使用すると、Ajax中であるかどうかを判別できる。
		Dim isInAsyncPostBack As Boolean = Me.CurrentScriptManager.IsInAsyncPostBack
		Dim ajaxES As FxEnum.AjaxExtStat = Me.AjaxExtensionStatus

		' 待機する（二重送信確認用）
		System.Threading.Thread.Sleep(3000)

		' テキストボックスの値を変更
		Dim textBox As TextBox = DirectCast(Me.GetContentWebControl("TextBox4"), TextBox)
		textBox.Text = "通常のポストバック（ＤＤＬのセレクト インデックス チェンジ）"

		Return ""
	End Function

	''' <summary>btnButton3のクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton3_Click(fxEventArgs As FxEventArgs) As String
		' Ajaxを制御する場合は、ScriptManagerを使用する。
		' このクラスを使用すると、Ajax中であるかどうかを判別できる。
		Dim isInAsyncPostBack As Boolean = Me.CurrentScriptManager.IsInAsyncPostBack
		Dim ajaxES As FxEnum.AjaxExtStat = Me.AjaxExtensionStatus

		' 待機する（UpdateProgress、二重送信確認用）
		System.Threading.Thread.Sleep(3000)

		Throw New Exception("Ajaxでエラー")

		'return "";
	End Function

	#End Region
End Class

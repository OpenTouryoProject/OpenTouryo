'**********************************************************************************
'* フレームワーク・テスト画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Aspx_testFxLayerP_normal_testScreen2
'* クラス日本語名  ：テスト画面２（Ｐ層）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*  2014/10/14  Rituparna         Changes made to support RsiodButtonList and CheckBoxList event
'*  2015/07/21  Supragyan         Created Textbox Textchanged event
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

''' <summary>テスト画面２（Ｐ層）</summary>
Public Partial Class Aspx_testFxLayerP_normal_testScreen2
	Inherits MyBaseController
	#Region "ページロードのUOCメソッド"

	''' <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
	''' <remarks>実装必須</remarks>
	Protected Overrides Sub UOC_FormInit()
		' フォーム初期化（初回ロード）時に実行する処理を実装する
		' TODO:
		Response.Write(Convert.ToString(Me.ContentPageFileNoEx) & "<br/>")

		' クライアントからの業務モーダル画面起動
		' スタイル指定なし
		Me.btnButton2.OnClientClick = "return " & Me.GetScriptToShowModalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx") & ";"
		Me.btnButton3.OnClientClick = "return " & Me.GetScriptToShowModalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx?test=test") & ";"

		' スタイル指定あり（空）
		Me.btnButton4.OnClientClick = "return " & Me.GetScriptToShowModalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx", "") & ";"
		Me.btnButton5.OnClientClick = "return " & Me.GetScriptToShowModalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx?test=test", "") & ";"

		' ---

		' クライアントからの業務モードレス画面起動
		Me.btnButton9.OnClientClick = Me.GetScriptToShowNormalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx") & "; return false;"

		Me.btnButton10.OnClientClick = Me.GetScriptToShowNormalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx?test=test", "") & "; return false;"

		Me.btnButton11.OnClientClick = Me.GetScriptToShowNormalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx?test=test", "", "t") & "; return false;"
	End Sub

	''' <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
	''' <remarks>実装必須</remarks>
	Protected Overrides Sub UOC_FormInit_PostBack()
		' フォーム初期化（ポストバック）時に実行する処理を実装する
		' TODO:
	End Sub

	#End Region

	#Region "外部パラメータ（アイコン）"

	#Region "コンテンツ ページ上のフレームワーク対象コントロール"

	''' <summary>
	''' btnButton1のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowYesNoMessageDialog("メッセージＩＤ", "メッセージ", "テスト結果")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' lbnLinkButton1のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_lbnLinkButton1_Click(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("メッセージＩＤ", "メッセージ", FxEnum.IconType.Information, "テスト結果（情報）")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' ibnImageButton1のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_ibnImageButton1_Click(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("メッセージＩＤ", "メッセージ", FxEnum.IconType.Exclamation, "テスト結果（警告）")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' impImageMap1のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_impImageMap1_Click(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("メッセージＩＤ", "メッセージ", FxEnum.IconType.StopMark, "テスト結果（エラー）")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	#End Region

	#End Region

	#Region "イベント追加"

	#Region "DropDownList"

	#Region "マスタ ページ上のフレームワーク対象コントロール"

	''' <summary>
	''' ddlMDropDownList1のSelectedIndexChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_TestScreen2_ddlMDropDownList1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("ddlMDropDownList1のSelectedIndexChangedイベント", DirectCast(Me.GetFxWebControl("ddlMDropDownList1"), DropDownList).SelectedValue, FxEnum.IconType.Information, "ＧＪ！")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	#End Region

	#Region "コンテンツ ページ上のフレームワーク対象コントロール"

	''' <summary>
	''' ddlDropDownList1のSelectedIndexChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_ddlDropDownList1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("ddlDropDownList1のSelectedIndexChangedイベント", DirectCast(Me.GetFxWebControl("ddlDropDownList1"), DropDownList).SelectedValue, FxEnum.IconType.Information, "ＧＪ！")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	#End Region

	#End Region

	#Region "ListBox"

	#Region "マスタ ページ上のフレームワーク対象コントロール"

	''' <summary>
	''' lbxMListBox1のSelectedIndexChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_TestScreen2_lbxMListBox1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("lbxMListBox1のSelectedIndexChangedイベント", DirectCast(Me.GetFxWebControl("lbxMListBox1"), ListBox).SelectedValue, FxEnum.IconType.Information, "ＧＪ！")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	#End Region

	#Region "コンテンツ ページ上のフレームワーク対象コントロール"

	''' <summary>
	''' lbxListBox1のSelectedIndexChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_lbxListBox1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("lbxListBox1のSelectedIndexChangedイベント", DirectCast(Me.GetFxWebControl("lbxListBox1"), ListBox).SelectedValue, FxEnum.IconType.Information, "ＧＪ！")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	#End Region

	#End Region

	#Region "RadioButton"

	#Region "マスタ ページ上のフレームワーク対象コントロール"

	''' <summary>
	''' rbnMRadioButton1のCheckedChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_TestScreen2_rbnMRadioButton1_CheckedChanged(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("rbnMRadioButton1のCheckedChangedイベント", DirectCast(Me.GetFxWebControl("rbnMRadioButton1"), RadioButton).Checked.ToString(), FxEnum.IconType.Information, "ＧＪ！")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' rbnMRadioButton2のCheckedChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_TestScreen2_rbnMRadioButton2_CheckedChanged(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("rbnMRadioButton2のCheckedChangedイベント", DirectCast(Me.GetFxWebControl("rbnMRadioButton2"), RadioButton).Checked.ToString(), FxEnum.IconType.Information, "ＧＪ！")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	#End Region

	#Region "コンテンツ ページ上のフレームワーク対象コントロール"

	''' <summary>
	''' rbnRadioButton1のCheckedChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_rbnRadioButton1_CheckedChanged(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("rbnRadioButton1のCheckedChangedイベント", DirectCast(Me.GetFxWebControl("rbnRadioButton1"), RadioButton).Checked.ToString(), FxEnum.IconType.Information, "ＧＪ！")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' rbnRadioButton2のCheckedChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_rbnRadioButton2_CheckedChanged(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("rbnRadioButton2のCheckedChangedイベント", DirectCast(Me.GetFxWebControl("rbnRadioButton2"), RadioButton).Checked.ToString(), FxEnum.IconType.Information, "ＧＪ！")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	#End Region

	#End Region

	#Region "CheckBox"

	#Region "マスタ ページ上のフレームワーク対象コントロール"

	''' <summary>
	''' cbxMCheckBox1のCheckedChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_TestScreen2_cbxMCheckBox1_CheckedChanged(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("cbxMCheckBox1のCheckedChangedイベント", DirectCast(Me.GetFxWebControl("cbxMCheckBox1"), CheckBox).Checked.ToString(), FxEnum.IconType.Information, "ＧＪ！")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' cbxMCheckBox2のCheckedChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_TestScreen2_cbxMCheckBox2_CheckedChanged(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("cbxMCheckBox2のCheckedChangedイベント", DirectCast(Me.GetFxWebControl("cbxMCheckBox2"), CheckBox).Checked.ToString(), FxEnum.IconType.Information, "ＧＪ！")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	#End Region

	#Region "コンテンツ ページ上のフレームワーク対象コントロール"

	''' <summary>
	''' cbxCheckBox1のCheckedChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_cbxCheckBox1_CheckedChanged(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("cbxCheckBox1のCheckedChangedイベント", DirectCast(Me.GetFxWebControl("cbxCheckBox1"), CheckBox).Checked.ToString(), FxEnum.IconType.Information, "ＧＪ！")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' cbxCheckBox2のCheckedChangedイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_cbxCheckBox2_CheckedChanged(fxEventArgs As FxEventArgs) As String
		' メッセージ表示
		Me.ShowOKMessageDialog("cbxCheckBox2のCheckedChangedイベント", DirectCast(Me.GetFxWebControl("cbxCheckBox2"), CheckBox).Checked.ToString(), FxEnum.IconType.Information, "ＧＪ！")

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	#End Region

	#End Region

	#Region "モーダル ダイアログのＩ / Ｆ"

	''' <summary>
	''' UOC_btnButton6のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton6_Click(fxEventArgs As FxEventArgs) As String
		' 親画面別セッション領域 - 設定
		Me.SetDataToModalInterface("msg", Me.TextBox1.Text)
		Return ""
	End Function

	''' <summary>
	''' UOC_btnButton7のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton7_Click(fxEventArgs As FxEventArgs) As String
		' 親画面別セッション領域 - 取得

		' メッセージ表示
		Me.ShowOKMessageDialog("親画面別セッション（キー：msg）は、", DirectCast(Me.GetDataFromModalInterface("msg"), String), FxEnum.IconType.Information, "テスト結果")

		Return ""
	End Function

	''' <summary>
	''' UOC_btnButton8のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton8_Click(fxEventArgs As FxEventArgs) As String
		' 親画面別セッション領域 - キー：msgのみ削除
		Me.DeleteDataFromModalInterface("msg")
		Return ""
    End Function

    ''' <summary>
    ''' UOC_txtTextBox2のテキスト変更イベント
    ''' </summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_txtTextBox2_TextChanged(ByVal fxEventArgs As FxEventArgs)
        Me.ShowOKMessageDialog("親画面別セッション（キー：msg）は、", "You changed text to" + " " + txtTextBox2.Text, FxEnum.IconType.Information, "テスト結果")
    End Sub

#End Region

#End Region

#Region "後処理のUOCメソッド"

    ''' <summary>「YES」・「NO」メッセージ・ダイアログの「×」が押され閉じられた場合の処理を実装する。</summary>
    ''' <param name="parentFxEventArgs">「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴</param>
    Protected Overrides Sub UOC_YesNoDialog_X_Click(ByVal parentFxEventArgs As FxEventArgs)
        ' 「YES」・「NO」メッセージ・ダイアログの「×」が押され閉じられた場合の処理を実装
        ' TODO:

        ' switch文

        ' メッセージ表示
        Me.ShowOKMessageDialog(Convert.ToString(parentFxEventArgs.ButtonID) & "で開いた「YES」・「NO」メッセージ・ダイアログ", "[×]ボタンを押した時の後処理", FxEnum.IconType.Information, "テスト結果")
    End Sub

    ''' <summary>「YES」・「NO」メッセージ・ダイアログの「YES」が押され閉じられた場合の処理を実装する。</summary>
    ''' <param name="parentFxEventArgs">「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴</param>
    Protected Overrides Sub UOC_YesNoDialog_Yes_Click(ByVal parentFxEventArgs As FxEventArgs)
        ' 「YES」・「NO」メッセージ・ダイアログの「YES」が押され閉じられた場合の処理を実装
        ' TODO:

        ' switch文

        ' メッセージ表示
        Me.ShowOKMessageDialog(Convert.ToString(parentFxEventArgs.ButtonID) & "で開いた「YES」・「NO」メッセージ・ダイアログ", "[Yes]ボタンを押した時の後処理", FxEnum.IconType.Information, "テスト結果")
    End Sub

    ''' <summary>「YES」・「NO」メッセージ・ダイアログの「NO」が押され閉じられた場合の処理を実装する。</summary>
    ''' <param name="parentFxEventArgs">「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴</param>
    Protected Overrides Sub UOC_YesNoDialog_No_Click(ByVal parentFxEventArgs As FxEventArgs)
        ' 「YES」・「NO」メッセージ・ダイアログの「NO」が押され閉じられた場合の処理を実装
        ' TODO:

        ' switch文

        ' メッセージ表示
        Me.ShowOKMessageDialog(Convert.ToString(parentFxEventArgs.ButtonID) & "で開いた「YES」・「NO」メッセージ・ダイアログ", "[No]ボタンを押した時の後処理", FxEnum.IconType.Information, "テスト結果")
    End Sub

    ''' <summary>業務モーダル画面の後処理を実装する。</summary>
    ''' <param name="parentFxEventArgs">業務モーダル画面を開いた（親画面側の）ボタンのボタン履歴</param>
    ''' <param name="childFxEventArgs">業務モーダル画面を閉じた（若しくは一番最後に押された子画面側の）ボタンのボタン履歴</param>
    Protected Overrides Sub UOC_ModalDialog_End(ByVal parentFxEventArgs As FxEventArgs, ByVal childFxEventArgs As FxEventArgs)
        ' 業務モーダル画面の後処理を実装
        ' TODO:

        ' switch文

        ' メッセージ表示
        Me.ShowOKMessageDialog(Convert.ToString(parentFxEventArgs.ButtonID) & "で開いた業務モーダル・ダイアログの", Convert.ToString(childFxEventArgs.ButtonID) & "ボタンを押して閉じた時の後処理", FxEnum.IconType.Information, "テスト結果")
    End Sub

#Region "RadioButtonList"
    ''' <summary>
    ''' rblRadioButtonList1のSelectedIndexChangedイベント
    ''' </summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_rblRadioButtonList1_SelectedIndexChanged(ByVal fxEventArgs As FxEventArgs) As String
        ' メッセージ表示
        Me.ShowOKMessageDialog("rblRadioButtonList1のSelectedIndexChangedイベント", DirectCast(Me.GetFxWebControl("rblRadioButtonList1"), RadioButtonList).SelectedValue, FxEnum.IconType.Information, "ＧＪ！")

        ' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        Return ""
    End Function

#End Region

#Region "CheckBoxList"
    ''' <summary>
    ''' cblCheckBoxList1のSelectedIndexChangedイベント
    ''' </summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_cblCheckBoxList1_SelectedIndexChanged(ByVal fxEventArgs As FxEventArgs) As String
        ' メッセージ表示
        Me.ShowOKMessageDialog("cblCheckBoxList1のSelectedIndexChangedイベント", DirectCast(Me.GetFxWebControl("cblCheckBoxList1"), CheckBoxList).SelectedValue, FxEnum.IconType.Information, "ＧＪ！")

        ' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        Return ""
    End Function

#End Region
#End Region

End Class

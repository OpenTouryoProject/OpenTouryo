'**********************************************************************************
'* フレームワーク・テスト画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Aspx_testFxLayerP_testDialogAtOnLoad
'* クラス日本語名  ：オンロードでのダイアログ表示のテスト画面（Ｐ層）
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

''' <summary>オンロードでのダイアログ表示のテスト画面（Ｐ層）</summary>
Public Partial Class Aspx_testFxLayerP_testDialogAtOnLoad
	Inherits MyBaseController
	#Region "ページロードのUOCメソッド"

	''' <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
	''' <remarks>実装必須</remarks>
	Protected Overrides Sub UOC_FormInit()
		' フォーム初期化（初回ロード）時に実行する処理を実装する
		' TODO:

		' 子画面表示メソッドをフォーム初期化（初回ロード）時に実行するテスト
		' （過去、サポートしていない時期があった）。

		' 取得
		Dim dialogAtOnLoad As String = DirectCast(Session("DialogAtOnLoad"), String)

		' Sessionをクリア
		Session("DialogAtOnLoad") = ""

		' ViewStateへ移動
		ViewState("DialogAtOnLoad") = dialogAtOnLoad

		' ---

		If dialogAtOnLoad = "ok" Then
			' ＯＫメッセージ ダイアログ表示
			Me.ShowOKMessageDialog("オンロード（初回ロード）で", "ＯＫメッセージ ダイアログ表示", FxEnum.IconType.Information, "オンロードでのテスト")
		ElseIf dialogAtOnLoad = "yesno" Then
			' Ｙｅｓ Ｎｏメッセージ ダイアログ表示
			Me.ShowYesNoMessageDialog("オンロード（初回ロード）で", "Ｙｅｓ Ｎｏメッセージ ダイアログ表示", "オンロードでのテスト")
		ElseIf dialogAtOnLoad = "modal" Then
			' 業務モーダル ダイアログ表示
			Me.ShowModalScreen("/ProjectX_sample/Aspx/testFxLayerP/testDialogAtOnLoad.aspx")
		ElseIf dialogAtOnLoad = "modaless" Then
			' 業務モーダレス画面表示
			Me.ShowNormalScreen("/ProjectX_sample/Aspx/testFxLayerP/testDialogAtOnLoad.aspx")
		End If
	End Sub

	''' <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
	''' <remarks>実装必須</remarks>
	Protected Overrides Sub UOC_FormInit_PostBack()
		' フォーム初期化（ポストバック）時に実行する処理を実装する
		' TODO:

		' こちらは、ページロードのUOCメソッド（個別：ポストバック）の動作確認用。

		If DirectCast(Me.GetContentWebControl("CheckBox1"), CheckBox).Checked Then
			Dim dialogAtOnLoad As String = DirectCast(ViewState("DialogAtOnLoad"), String)

			If dialogAtOnLoad = "ok" Then
				' ＯＫメッセージ・ダイアログ表示
				Me.ShowOKMessageDialog("オンロード（ポストバック）で", "ＯＫメッセージ・ダイアログ表示", FxEnum.IconType.Information, "オンロードでのテスト")
			ElseIf dialogAtOnLoad = "yesno" Then
				' Ｙｅｓ・Ｎｏメッセージ・ダイアログ表示

					' →　ポストバックのオンロードでShowYesNoMessageDialogは実行できない。
					'     ShowYesNoMessageDialogの後処理のポストバックで無限ループになる。

					'     1. ポストバックのオンロードでShowYesNoMessageDialog
					'     2. Ｙｅｓ・Ｎｏメッセージ・ダイアログ表示
					'     3. Ｙｅｓ・Ｎｏメッセージ・ダイアログの後処理のポストバック
					'     4. 1.に戻る（無限ループ）。
				Me.ShowYesNoMessageDialog("オンロード（ポストバック）で", "Ｙｅｓ・Ｎｏメッセージ・ダイアログ表示", "オンロードでのテスト")
			ElseIf dialogAtOnLoad = "modal" Then
				' 業務モーダル ダイアログ表示

					'→　ポストバックのオンロードでShowModalScreenは実行できない。
					'    ShowModalScreenの後処理のポストバックで無限ループになる。

					'     1. ポストバックのオンロードでShowModalScreen
					'     2. 業務モーダル ダイアログ表示
					'     3. 業務モーダル ダイアログの後処理のポストバック
					'     4. 1.に戻る（無限ループ）。

					'    ※ 閉じる処理が、NoPostback（WithAllParent中間を飛ばす場合）なら可能

					'    ※ サンプルでは、後処理で、ShowOKMessageDialogも実行され、
					'       コチラが優先されるので、無限ループにはならない。
				Me.ShowModalScreen("/ProjectX_sample/Aspx/testFxLayerP/testDialogAtOnLoad.aspx")
			ElseIf dialogAtOnLoad = "modaless" Then
				' 業務モーダレス画面表示
				Me.ShowNormalScreen("/ProjectX_sample/Aspx/testFxLayerP/testDialogAtOnLoad.aspx")
			End If
		End If
	End Sub

	#End Region

	#Region "コンテンツ ページ上のフレームワーク対象コントロール"

	''' <summary>
	''' btnButton1のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
		' 画面を閉じる
		Me.CloseModalScreen()

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' btnButton2のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton2_Click(fxEventArgs As FxEventArgs) As String
		' 画面を閉じる
		Me.CloseModalScreen_NoPostback()

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	''' <summary>
	''' btnButton2のクリックイベント
	''' </summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton3_Click(fxEventArgs As FxEventArgs) As String
		' 画面を閉じる
		Me.CloseModalScreen_WithAllParent()

		' 画面遷移しないポストバックの場合は、urlを空文字列に設定する
		Return ""
	End Function

	#End Region

	#Region "後処理のUOCメソッド"

	''' <summary>「YES」・「NO」メッセージ・ダイアログの「×」が押され閉じられた場合の処理を実装する。</summary>
	''' <param name="parentFxEventArgs">「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴</param>
	Protected Overrides Sub UOC_YesNoDialog_X_Click(parentFxEventArgs As FxEventArgs)
		' 「YES」・「NO」メッセージ・ダイアログの「×」が押され閉じられた場合の処理を実装
		' TODO:

		' switch文

		' メッセージ表示
		Me.ShowOKMessageDialog(Convert.ToString(parentFxEventArgs.ButtonID) & "で開いた「YES」・「NO」メッセージ・ダイアログ", "[×]ボタンを押した時の後処理", FxEnum.IconType.Information, "テスト結果")
	End Sub

	''' <summary>「YES」・「NO」メッセージ・ダイアログの「YES」が押され閉じられた場合の処理を実装する。</summary>
	''' <param name="parentFxEventArgs">「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴</param>
	Protected Overrides Sub UOC_YesNoDialog_Yes_Click(parentFxEventArgs As FxEventArgs)
		' 「YES」・「NO」メッセージ・ダイアログの「YES」が押され閉じられた場合の処理を実装
		' TODO:

		' switch文

		' メッセージ表示
		Me.ShowOKMessageDialog(Convert.ToString(parentFxEventArgs.ButtonID) & "で開いた「YES」・「NO」メッセージ・ダイアログ", "[Yes]ボタンを押した時の後処理", FxEnum.IconType.Information, "テスト結果")
	End Sub

	''' <summary>「YES」・「NO」メッセージ・ダイアログの「NO」が押され閉じられた場合の処理を実装する。</summary>
	''' <param name="parentFxEventArgs">「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴</param>
	Protected Overrides Sub UOC_YesNoDialog_No_Click(parentFxEventArgs As FxEventArgs)
		' 「YES」・「NO」メッセージ・ダイアログの「NO」が押され閉じられた場合の処理を実装
		' TODO:

		' switch文

		' メッセージ表示
		Me.ShowOKMessageDialog(Convert.ToString(parentFxEventArgs.ButtonID) & "で開いた「YES」・「NO」メッセージ・ダイアログ", "[No]ボタンを押した時の後処理", FxEnum.IconType.Information, "テスト結果")
	End Sub

	''' <summary>業務モーダル画面の後処理を実装する。</summary>
	''' <param name="parentFxEventArgs">業務モーダル画面を開いた（親画面側の）ボタンのボタン履歴</param>
	''' <param name="childFxEventArgs">業務モーダル画面を閉じた（若しくは一番最後に押された子画面側の）ボタンのボタン履歴</param>
	Protected Overrides Sub UOC_ModalDialog_End(parentFxEventArgs As FxEventArgs, childFxEventArgs As FxEventArgs)
		' 業務モーダル画面の後処理を実装
		' TODO:

		' switch文

		' メッセージ表示
		Me.ShowOKMessageDialog(Convert.ToString(parentFxEventArgs.ButtonID) & "で開いた業務モーダル・ダイアログの", Convert.ToString(childFxEventArgs.ButtonID) & "ボタンを押して閉じた時の後処理", FxEnum.IconType.Information, "テスト結果")
	End Sub

	#End Region
End Class

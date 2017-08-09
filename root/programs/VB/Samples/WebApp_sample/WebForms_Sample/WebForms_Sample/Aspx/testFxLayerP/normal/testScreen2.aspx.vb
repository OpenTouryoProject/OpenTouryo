'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testScreen2
'* クラス日本語名  ：テスト画面２（Ｐ層）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*  2015/04/17  Supragyan         Created Textbox Textchanged event
'**********************************************************************************

Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util

Namespace Aspx.TestFxLayerP.Normal
    ''' <summary>テスト画面２（Ｐ層）</summary>
    Partial Public Class testScreen2
        Inherits MyBaseController
#Region "Page LoadのUOCメソッド"

        ''' <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する
            ' TODO:
            Me.lblResult.Text = Me.ContentPageFileNoEx & "<br/>"

            ' クライアントからの業務Modal画面起動
            ' スタイル指定なし
            Me.btnButton2.OnClientClick = "return " & Me.GetScriptToShowModalScreen("~/Aspx/TestFxLayerP/Normal/testScreen1.aspx") & ";"
            Me.btnButton3.OnClientClick = "return " & Me.GetScriptToShowModalScreen("~/Aspx/TestFxLayerP/Normal/testScreen1.aspx?test=test") & ";"

            ' スタイル指定あり（空）
            Me.btnButton4.OnClientClick = "return " & Me.GetScriptToShowModalScreen("~/Aspx/TestFxLayerP/Normal/testScreen1.aspx", "") & ";"
            Me.btnButton5.OnClientClick = "return " & Me.GetScriptToShowModalScreen("~/Aspx/TestFxLayerP/Normal/testScreen1.aspx?test=test", "") & ";"

            ' ---

            ' クライアントからの業務Modeless画面起動
            Me.btnButton9.OnClientClick = Me.GetScriptToShowNormalScreen("~/Aspx/TestFxLayerP/Normal/testScreen1.aspx") & "; return false;"
            Me.btnButton10.OnClientClick = Me.GetScriptToShowNormalScreen("~/Aspx/TestFxLayerP/Normal/testScreen1.aspx?test=test", "") & "; return false;"
            Me.btnButton11.OnClientClick = Me.GetScriptToShowNormalScreen("~/Aspx/TestFxLayerP/Normal/testScreen1.aspx?test=test", "", "t") & "; return false;"
        End Sub

        ''' <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する
            ' TODO:
        End Sub

#End Region

#Region "外部パラメータ（アイコン）"

#Region "Content Page上のフレームワーク対象Control"

        ''' <summary>
        ''' btnButton1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowYesNoMessageDialog("MessageID", "Message", "テスト結果")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' lbnLinkButton1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbnLinkButton1_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog("MessageID", "Message", FxEnum.IconType.Information, "テスト結果（情報）")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' ibnImageButton1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ibnImageButton1_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog("MessageID", "Message", FxEnum.IconType.Exclamation, "テスト結果（警告）")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' impImageMap1のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_impImageMap1_Click(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog("MessageID", "Message", FxEnum.IconType.StopMark, "テスト結果（エラー）")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#End Region

#Region "イベント追加"

#Region "DropDownList"

#Region "Master Page上のフレームワーク対象Control"

        ''' <summary>
        ''' ddlMDropDownList1のSelectedIndexChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen2_ddlMDropDownList1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                "ddlMDropDownList1のSelectedIndexChangedイベント",
                DirectCast(Me.GetFxWebControl("ddlMDropDownList1"), DropDownList).SelectedValue,
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#Region "Content Page上のフレームワーク対象Control"

        ''' <summary>
        ''' ddlDropDownList1のSelectedIndexChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ddlDropDownList1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                "ddlDropDownList1のSelectedIndexChangedイベント",
                DirectCast(Me.GetFxWebControl("ddlDropDownList1"), DropDownList).SelectedValue,
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#End Region

#Region "ListBox"

#Region "Master Page上のフレームワーク対象Control"

        ''' <summary>
        ''' lbxMListBox1のSelectedIndexChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen2_lbxMListBox1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                "lbxMListBox1のSelectedIndexChangedイベント",
                DirectCast(Me.GetFxWebControl("lbxMListBox1"), ListBox).SelectedValue,
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#Region "Content Page上のフレームワーク対象Control"

        ''' <summary>
        ''' lbxListBox1のSelectedIndexChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbxListBox1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                "lbxListBox1のSelectedIndexChangedイベント",
                DirectCast(Me.GetFxWebControl("lbxListBox1"), ListBox).SelectedValue,
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#End Region

#Region "RadioButton"

#Region "Master Page上のフレームワーク対象Control"

        ''' <summary>
        ''' rbnMRadioButton1のCheckedChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen2_rbnMRadioButton1_CheckedChanged(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                "rbnMRadioButton1のCheckedChangedイベント",
                DirectCast(Me.GetFxWebControl("rbnMRadioButton1"), RadioButton).Checked.ToString(),
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' rbnMRadioButton2のCheckedChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen2_rbnMRadioButton2_CheckedChanged(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                "rbnMRadioButton2のCheckedChangedイベント",
                DirectCast(Me.GetFxWebControl("rbnMRadioButton2"), RadioButton).Checked.ToString(),
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#Region "Content Page上のフレームワーク対象Control"

        ''' <summary>
        ''' rbnRadioButton1のCheckedChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_rbnRadioButton1_CheckedChanged(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                "rbnRadioButton1のCheckedChangedイベント",
                DirectCast(Me.GetFxWebControl("rbnRadioButton1"), RadioButton).Checked.ToString(),
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' rbnRadioButton2のCheckedChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_rbnRadioButton2_CheckedChanged(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                "rbnRadioButton2のCheckedChangedイベント",
                DirectCast(Me.GetFxWebControl("rbnRadioButton2"), RadioButton).Checked.ToString(),
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#End Region

#Region "RadioButtonList"
        ''' <summary>
        ''' rblRadioButtonList1_SelectedIndexChanged
        ''' </summary>
        ''' <param name="fxEventArgs"></param>
        ''' <returns>URL</returns>
        Protected Function UOC_rblRadioButtonList1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            ' ShowOKMessageDialog for radiobuttonlist
            Me.ShowOKMessageDialog(
                "rblRadioButtonList1のSelectedIndexChangedイベント",
                DirectCast(Me.GetFxWebControl("rblRadioButtonList1"), RadioButtonList).SelectedValue,
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function
#End Region

#Region "CheckBox"

#Region "Master Page上のフレームワーク対象Control"

        ''' <summary>
        ''' cbxMCheckBox1のCheckedChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen2_cbxMCheckBox1_CheckedChanged(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                "cbxMCheckBox1のCheckedChangedイベント",
                DirectCast(Me.GetFxWebControl("cbxMCheckBox1"), CheckBox).Checked.ToString(),
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' cbxMCheckBox2のCheckedChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_TestScreen2_cbxMCheckBox2_CheckedChanged(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                "cbxMCheckBox2のCheckedChangedイベント",
                DirectCast(Me.GetFxWebControl("cbxMCheckBox2"), CheckBox).Checked.ToString(),
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#Region "Content Page上のフレームワーク対象Control"

        ''' <summary>
        ''' cbxCheckBox1のCheckedChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_cbxCheckBox1_CheckedChanged(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                "cbxCheckBox1のCheckedChangedイベント",
                DirectCast(Me.GetFxWebControl("cbxCheckBox1"), CheckBox).Checked.ToString(),
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

        ''' <summary>
        ''' cbxCheckBox2のCheckedChangedイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_cbxCheckBox2_CheckedChanged(fxEventArgs As FxEventArgs) As String
            ' Message表示
            Me.ShowOKMessageDialog(
                "cbxCheckBox2のCheckedChangedイベント",
                DirectCast(Me.GetFxWebControl("cbxCheckBox2"), CheckBox).Checked.ToString(),
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function

#End Region

#End Region

#Region "CheckboxList"
        ''' <summary>
        ''' UOC_cblCheckBoxList1_SelectedIndexChanged
        ''' </summary>
        ''' <param name="fxEventArgs"></param>
        ''' <returns>URL</returns>
        Protected Function UOC_cblCheckBoxList1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            '  ShowOKMessageDialog for checkboxlist
            Me.ShowOKMessageDialog(
                "cblCheckBoxList1のSelectedIndexChangedイベント",
                DirectCast(Me.GetFxWebControl("cblCheckBoxList1"), CheckBoxList).SelectedValue,
                FxEnum.IconType.Information, "GJ!!")

            ' 画面遷移しないPost Backの場合は、urlを空文字列に設定する
            Return ""
        End Function
#End Region

#Region "Modal DialogのI/F"

        ''' <summary>
        ''' UOC_btnButton6のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton6_Click(fxEventArgs As FxEventArgs) As String
            ' 親画面別セッション領域 - 設定
            Me.SetDataToModalInterface("msg", Me.TextBox1.Text)
            Return ""
        End Function

        ''' <summary>
        ''' UOC_btnButton7のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton7_Click(fxEventArgs As FxEventArgs) As String
            ' 親画面別セッション領域 - 取得

            ' Message表示
            Me.ShowOKMessageDialog(
                "親画面別セッション（キー：msg）は、",
                DirectCast(Me.GetDataFromModalInterface("msg"), String),
                FxEnum.IconType.Information, "テスト結果")

            Return ""
        End Function

        ''' <summary>
        ''' UOC_btnButton8のClickイベント
        ''' </summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton8_Click(fxEventArgs As FxEventArgs) As String
            ' 親画面別セッション領域 - キー：msgのみ削除
            Me.DeleteDataFromModalInterface("msg")
            Return ""
        End Function

        ''' <summary>
        ''' UOC_txtTextBox2のテキスト変更イベント
        ''' </summary>
        ''' <param name="fxEventArgs"></param>
        Protected Sub UOC_txtTextBox2_TextChanged(fxEventArgs As FxEventArgs)
            Me.ShowOKMessageDialog(
                "親画面別セッション（キー：msg）は、",
                ("You changed text to" & " ") + txtTextBox2.Text, FxEnum.IconType.Information,
                "テスト結果")
        End Sub

#End Region

#End Region

#Region "後処理のUOCメソッド"

        ''' <summary>「YES」・「NO」Message Dialogの「×」が押され閉じられた場合の処理を実装する。</summary>
        ''' <param name="parentFxEventArgs">「YES」・「NO」Message Dialogを開いた（親画面側の）ButtonのButton履歴</param>
        Protected Overrides Sub UOC_YesNoDialog_X_Click(parentFxEventArgs As FxEventArgs)
            ' 「YES」・「NO」Message Dialogの「×」が押され閉じられた場合の処理を実装
            ' TODO:

            ' switch文

            ' Message表示
            Me.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID & "で開いた「YES」・「NO」Message Dialog",
                "[×]Buttonを押した時の後処理", FxEnum.IconType.Information, "テスト結果")
        End Sub

        ''' <summary>「YES」・「NO」Message Dialogの「YES」が押され閉じられた場合の処理を実装する。</summary>
        ''' <param name="parentFxEventArgs">「YES」・「NO」Message Dialogを開いた（親画面側の）ButtonのButton履歴</param>
        Protected Overrides Sub UOC_YesNoDialog_Yes_Click(parentFxEventArgs As FxEventArgs)
            ' 「YES」・「NO」Message Dialogの「YES」が押され閉じられた場合の処理を実装
            ' TODO:

            ' switch文

            ' Message表示
            Me.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID & "で開いた「YES」・「NO」Message Dialog",
                "[Yes]Buttonを押した時の後処理", FxEnum.IconType.Information, "テスト結果")
        End Sub

        ''' <summary>「YES」・「NO」Message Dialogの「NO」が押され閉じられた場合の処理を実装する。</summary>
        ''' <param name="parentFxEventArgs">「YES」・「NO」Message Dialogを開いた（親画面側の）ButtonのButton履歴</param>
        Protected Overrides Sub UOC_YesNoDialog_No_Click(parentFxEventArgs As FxEventArgs)
            ' 「YES」・「NO」Message Dialogの「NO」が押され閉じられた場合の処理を実装
            ' TODO:

            ' switch文

            ' Message表示
            Me.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID & "で開いた「YES」・「NO」Message Dialog",
                "[No]Buttonを押した時の後処理", FxEnum.IconType.Information, "テスト結果")
        End Sub

        ''' <summary>業務Modal画面の後処理を実装する。</summary>
        ''' <param name="parentFxEventArgs">業務Modal画面を開いた（親画面側の）ButtonのButton履歴</param>
        ''' <param name="childFxEventArgs">業務Modal画面を閉じた（若しくは一番最後に押された子画面側の）ButtonのButton履歴</param>
        Protected Overrides Sub UOC_ModalDialog_End(parentFxEventArgs As FxEventArgs, childFxEventArgs As FxEventArgs)
            ' 業務Modal画面の後処理を実装
            ' TODO:

            ' switch文

            ' Message表示
            Me.ShowOKMessageDialog(
                parentFxEventArgs.ButtonID & "で開いた業務Modal Dialogの",
                childFxEventArgs.ButtonID & "Buttonを押して閉じた時の後処理",
                FxEnum.IconType.Information, "テスト結果")
        End Sub

#End Region

    End Class

End Namespace

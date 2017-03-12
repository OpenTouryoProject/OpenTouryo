'**********************************************************************************
'* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：UserControl3
'* クラス日本語名  ：ユーザコントロール
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*
'**********************************************************************************

Imports Touryo.Infrastructure.Framework.RichClient.Presentation

Partial Public Class UserControl3
    Inherits UserControl
    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Sub UOC_btnUCButton1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_btnUCButton1_Click")
    End Sub

    Protected Sub UOC_pbxUCPictureBox1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_pbxUCPictureBox1_Click")
    End Sub

    Protected Sub UOC_rbnUCRadioButton1_CheckedChanged(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_rbnUCRadioButton1_CheckedChanged")
    End Sub

    Protected Sub UOC_cbxUCCheckBox1_CheckedChanged(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_cbxUCCheckBox1_CheckedChanged")
    End Sub

    Protected Sub UOC_cbbUCComboBox1_SelectedIndexChanged(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_cbbUCComboBox1_SelectedIndexChanged")
    End Sub

    Protected Sub UOC_lbxUCListBox1_SelectedIndexChanged(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_lbxUCListBox1_SelectedIndexChanged")
    End Sub
End Class
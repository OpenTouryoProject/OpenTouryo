'**********************************************************************************
'* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：UserControl3
'* クラス日本語名  ：UserControl3
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

''' <summary>UserControl3</summary>
Partial Public Class UserControl3
    Inherits UserControl

    ''' <summary>constructor</summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>UOC_btnUCButton1_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_btnUCButton1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_btnUCButton1_Click")
    End Sub

    ''' <summary>UOC_pbxUCPictureBox1_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_pbxUCPictureBox1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_pbxUCPictureBox1_Click")
    End Sub

    ''' <summary>UOC_rbnUCRadioButton1_CheckedChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_rbnUCRadioButton1_CheckedChanged(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_rbnUCRadioButton1_CheckedChanged")
    End Sub

    ''' <summary>UOC_cbxUCCheckBox1_CheckedChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_cbxUCCheckBox1_CheckedChanged(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_cbxUCCheckBox1_CheckedChanged")
    End Sub

    ''' <summary>UOC_cbbUCComboBox1_SelectedIndexChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_cbbUCComboBox1_SelectedIndexChanged(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_cbbUCComboBox1_SelectedIndexChanged")
    End Sub

    ''' <summary>UOC_lbxUCListBox1_SelectedIndexChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_lbxUCListBox1_SelectedIndexChanged(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_lbxUCListBox1_SelectedIndexChanged")
    End Sub
End Class
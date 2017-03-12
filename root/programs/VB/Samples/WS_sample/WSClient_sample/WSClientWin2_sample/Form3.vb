'**********************************************************************************
'* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：Form3
'* クラス日本語名  ：サンプル アプリ画面
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System
Imports System.Threading
Imports System.Diagnostics
Imports System.Windows.Forms

Imports Touryo.Infrastructure.Business.RichClient.Presentation
Imports Touryo.Infrastructure.Framework.RichClient.Presentation

Partial Public Class Form3
    Inherits MyBaseControllerWin
    ''' <summary>コンストラクタ</summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>フォームロードのUOCメソッド</summary>
    Protected Overrides Sub UOC_FormInit()
        ' 表示する。
        Me.ContextMenuStrip = Me.contextMenuStrip1

        ' ここで設定する。
        AddHandler Me.contextMenuStrip1.Items(0).Click, AddressOf Me.Item_Click
        AddHandler Me.contextMenuStrip1.Items(1).Click, AddressOf Me.Item_Click
        AddHandler Me.contextMenuStrip1.Items(2).Click, AddressOf Me.Item_Click

        AddHandler Me.tsmiItem21ToolStripMenuItem.Click, AddressOf Me.Item_Click
        AddHandler Me.tsmiItem22ToolStripMenuItem.Click, AddressOf Me.Item_Click
        AddHandler Me.tsmiItem221ToolStripMenuItem.Click, AddressOf Me.Item_Click
        AddHandler Me.tsmiItem222ToolStripMenuItem.Click, AddressOf Me.Item_Click
    End Sub

#Region "Ctrlイベント"

    Protected Sub UOC_btnButton1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_btnButton1_Click")
    End Sub

    Protected Sub UOC_pbxPictureBox1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_pbxPictureBox1_Click")
    End Sub

    Protected Sub UOC_rbnRadioButton1_CheckedChanged(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_rbnRadioButton1_CheckedChanged")
    End Sub

    Protected Sub UOC_cbxCheckBox1_CheckedChanged(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_cbxCheckBox1_CheckedChanged")
    End Sub

    Protected Sub UOC_cbbComboBox1_SelectedIndexChanged(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_cbbComboBox1_SelectedIndexChanged")
    End Sub

    Protected Sub UOC_lbxListBox1_SelectedIndexChanged(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_lbxListBox1_SelectedIndexChanged")
    End Sub

    Protected Sub UOC_tsmiItem1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_tsmiItem1_Click")
    End Sub

    Protected Sub UOC_tsmiItem2_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_tsmiItem2_Click")
    End Sub

    Protected Sub UOC_tsmiItem21_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_tsmiItem21_Click")
    End Sub

    Protected Sub UOC_tsmiItem22_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_tsmiItem22_Click")
    End Sub

    Protected Sub UOC_tsmiItem221_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_tsmiItem221_Click")
    End Sub

    Protected Sub UOC_tsmiItem222_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_tsmiItem222_Click")
    End Sub

    Protected Sub UOC_tsmiItem3_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_tsmiItem3_Click")
    End Sub

    '---

    ' UserControlよりFormに実装されたメソッドが優先される。
    ' ※ ボタン名は一意である必要がある（イベントを識別できなくなる）。

    'protected void UOC_userControl3_btnUCButton1_Click(RcFxEventArgs rcFxEventArgs)
    '{
    '    Debug.WriteLine("UOC_userControl3_btnUCButton1_Click");
    '}

    'protected void UOC_userControl3_pbxUCPictureBox1_Click(RcFxEventArgs rcFxEventArgs)
    '{
    '    Debug.WriteLine("UOC_userControl3_pbxUCPictureBox1_Click");
    '}

    'protected void UOC_userControl3_rbnUCRadioButton1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
    '{
    '    Debug.WriteLine("UOC_userControl3_rbnUCRadioButton1_CheckedChanged");
    '}

    'protected void UOC_userControl3_cbxUCCheckBox1_CheckedChanged(RcFxEventArgs rcFxEventArgs)
    '{
    '    Debug.WriteLine("UOC_userControl3_cbxUCCheckBox1_CheckedChanged");
    '}

    'protected void UOC_userControl3_cbbUCComboBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
    '{
    '    Debug.WriteLine("UOC_userControl3_cbbUCComboBox1_SelectedIndexChanged");
    '}

    'protected void UOC_userControl3_lbxUCListBox1_SelectedIndexChanged(RcFxEventArgs rcFxEventArgs)
    '{
    '    Debug.WriteLine("UOC_userControl3_lbxUCListBox1_SelectedIndexChanged");
    '}

    ''' <summary>テスト１</summary>
    Protected Sub UOC_btnElse1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' newだけした場合・・・ 
        Dim f As Form = New Form2()

        MessageBox.Show("画面総数:" & BaseControllerWin.GetWindowsCount().ToString() & ", Form2総数:" & BaseControllerWin.GetWindowsCount(GetType(Form2)).ToString())
    End Sub

    ''' <summary>テスト２</summary>
    Protected Sub UOC_btnElse2_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        'throw new Exception("てすと");

        Dim th As New Thread(New ThreadStart(AddressOf Me.ThMe))
        th.Start()
    End Sub

    Private Sub ThMe()
        Throw New Exception("てすと")
    End Sub

#End Region

#Region "Formイベント"
    ' プロジェクト独自

    Protected Sub UOC_Form_Enter_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_Enter_KeyDown")
    End Sub
    Protected Sub UOC_Form_F1_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F1_KeyDown")
    End Sub
    Protected Sub UOC_Form_F2_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F2_KeyDown")
    End Sub
    Protected Sub UOC_Form_F3_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F3_KeyDown")
    End Sub
    Protected Sub UOC_Form_F4_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F4_KeyDown")
    End Sub
    Protected Sub UOC_Form_F5_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F5_KeyDown")
    End Sub
    Protected Sub UOC_Form_F6_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F6_KeyDown")
    End Sub
    Protected Sub UOC_Form_F7_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F7_KeyDown")
    End Sub
    Protected Sub UOC_Form_F8_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F8_KeyDown")
    End Sub
    Protected Sub UOC_Form_F9_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F9_KeyDown")
    End Sub
    Protected Sub UOC_Form_F10_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F10_KeyDown")
    End Sub
    Protected Sub UOC_Form_F11_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F11_KeyDown")
    End Sub
    Protected Sub UOC_Form_F12_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F12_KeyDown")
    End Sub
    Protected Sub UOC_Form_AltAndF4_KeyDown(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_AltAndF4_KeyDown")
    End Sub
    Protected Sub UOC_Form_Closing(ByVal rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_Closing")
    End Sub

#End Region

#Region "未解放イベント"
    ' ログが出過ぎるので

    'protected void UOC_Form_KeyDown(RcFxEventArgs rcFxEventArgs)
    '{
    '    Debug.WriteLine("UOC_Form_KeyDown");
    '}
    'protected void UOC_Form_KeyPress(RcFxEventArgs rcFxEventArgs)
    '{
    '    Debug.WriteLine("UOC_Form_KeyPress");
    '}
    'protected void UOC_Form_KeyUp(RcFxEventArgs rcFxEventArgs)
    '{
    '    Debug.WriteLine("UOC_Form_KeyUp");
    '}

#End Region

#Region "UserControlイベント"

    Protected Sub UOC_userControl31_btnUCButton1_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl31_btnUCButton1_Click")
    End Sub

    Protected Sub UOC_userControl31_pbxUCPictureBox1_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl31_pbxUCPictureBox1_Click")
    End Sub

    Protected Sub UOC_userControl31_rbnUCRadioButton1_CheckedChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl31_rbnUCRadioButton1_CheckedChanged")
    End Sub

    Protected Sub UOC_userControl31_cbxUCCheckBox1_CheckedChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl31_cbxUCCheckBox1_CheckedChanged")
    End Sub

    Protected Sub UOC_userControl31_cbbUCComboBox1_SelectedIndexChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl31_cbbUCComboBox1_SelectedIndexChanged")
    End Sub

    Protected Sub UOC_userControl31_lbxUCListBox1_SelectedIndexChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl31_lbxUCListBox1_SelectedIndexChanged")
    End Sub

    ' ---

    Protected Sub UOC_userControl32_btnUCButton1_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl32_btnUCButton1_Click")
    End Sub

    Protected Sub UOC_userControl32_pbxUCPictureBox1_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl32_pbxUCPictureBox1_Click")
    End Sub

    Protected Sub UOC_userControl32_rbnUCRadioButton1_CheckedChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl32_rbnUCRadioButton1_CheckedChanged")
    End Sub

    Protected Sub UOC_userControl32_cbxUCCheckBox1_CheckedChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl32_cbxUCCheckBox1_CheckedChanged")
    End Sub

    Protected Sub UOC_userControl32_cbbUCComboBox1_SelectedIndexChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl32_cbbUCComboBox1_SelectedIndexChanged")
    End Sub

    Protected Sub UOC_userControl32_lbxUCListBox1_SelectedIndexChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl32_lbxUCListBox1_SelectedIndexChanged")
    End Sub

#End Region

End Class


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

Imports System.Threading

Imports Touryo.Infrastructure.Business.RichClient.Presentation
Imports Touryo.Infrastructure.Business.RichClient.Util
Imports Touryo.Infrastructure.Framework.RichClient.Presentation
Imports Touryo.Infrastructure.Framework.RichClient.Util

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

        ' userControl32の動的ロード
        Dim userControl32 As UserControl3 = New UserControl3()
        userControl32.Location = New Point(8, 23)
        userControl32.Margin = New Padding(5)
        userControl32.Name = "userControl32"
        userControl32.Size = New Size(283, 330)
        userControl32.TabIndex = 0
        Me.groupBox3.Controls.Add(userControl32)
    End Sub

#Region "Ctrlイベント"

    ''' <summary>UOC_btnButton1_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_btnButton1_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_btnButton1_Click")
    End Sub

    ''' <summary>UOC_pbxPictureBox1_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_pbxPictureBox1_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_pbxPictureBox1_Click")
    End Sub

    ''' <summary>UOC_rbnRadioButton1_CheckedChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_rbnRadioButton1_CheckedChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_rbnRadioButton1_CheckedChanged")
    End Sub

    ''' <summary>UOC_cbxCheckBox1_CheckedChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_cbxCheckBox1_CheckedChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_cbxCheckBox1_CheckedChanged")
    End Sub

    ''' <summary>UOC_cbbComboBox1_SelectedIndexChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_cbbComboBox1_SelectedIndexChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_cbbComboBox1_SelectedIndexChanged")
    End Sub

    ''' <summary>UOC_lbxListBox1_SelectedIndexChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_lbxListBox1_SelectedIndexChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_lbxListBox1_SelectedIndexChanged")
    End Sub

    ''' <summary>UOC_tsmiItem1_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_tsmiItem1_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_tsmiItem1_Click")
    End Sub

    ''' <summary>UOC_tsmiItem2_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_tsmiItem2_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_tsmiItem2_Click")
    End Sub

    ''' <summary>UOC_tsmiItem21_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_tsmiItem21_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_tsmiItem21_Click")
    End Sub

    ''' <summary>UOC_tsmiItem22_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_tsmiItem22_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_tsmiItem22_Click")
    End Sub

    ''' <summary>UOC_tsmiItem221_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_tsmiItem221_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_tsmiItem221_Click")
    End Sub

    ''' <summary>UOC_tsmiItem222_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_tsmiItem222_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_tsmiItem222_Click")
    End Sub

    ''' <summary>UOC_tsmiItem3_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_tsmiItem3_Click(rcFxEventArgs As RcFxEventArgs)
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
    Protected Sub UOC_btnElse1_Click(rcFxEventArgs As RcFxEventArgs)
        ' newだけした場合・・・ 
        Dim f As Form = New Form2()

        MessageBox.Show("画面総数:" & BaseControllerWin.GetWindowsCount().ToString() & ", Form2総数:" & BaseControllerWin.GetWindowsCount(GetType(Form2)).ToString())
    End Sub

    ''' <summary>テスト２</summary>
    Protected Sub UOC_btnElse2_Click(rcFxEventArgs As RcFxEventArgs)
        'throw new Exception("てすと");

        Dim th As New Thread(New ThreadStart(AddressOf Me.ThMe))
        th.Start()
    End Sub

    ''' <summary>バックグラウンド・スレッド</summary>
    Private Sub ThMe()
        Throw New Exception("てすと")
    End Sub

#End Region

#Region "Formイベント"
    ' プロジェクト独自

    ''' <summary>UOC_Form_Enter_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_Enter_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_Enter_KeyDown")
    End Sub

    ''' <summary>UOC_Form_F1_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_F1_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F1_KeyDown")
    End Sub

    ''' <summary>UOC_Form_F2_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_F2_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F2_KeyDown")
    End Sub

    ''' <summary>UOC_Form_F3_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_F3_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F3_KeyDown")
    End Sub

    ''' <summary>UOC_Form_F4_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_F4_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F4_KeyDown")
    End Sub

    ''' <summary>UOC_Form_F5_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_F5_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F5_KeyDown")
    End Sub

    ''' <summary>UOC_Form_F6_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_F6_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F6_KeyDown")
    End Sub

    ''' <summary>UOC_Form_F7_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_F7_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F7_KeyDown")
    End Sub

    ''' <summary>UOC_Form_F8_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_F8_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F8_KeyDown")
    End Sub

    ''' <summary>UOC_Form_F9_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_F9_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F9_KeyDown")
    End Sub

    ''' <summary>UOC_Form_F10_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_F10_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F10_KeyDown")
    End Sub

    ''' <summary>UOC_Form_F11_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_F11_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F11_KeyDown")
    End Sub

    ''' <summary>UOC_Form_F12_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_F12_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_F12_KeyDown")
    End Sub

    ''' <summary>UOC_Form_AltAndF4_KeyDown</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_AltAndF4_KeyDown(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_Form_AltAndF4_KeyDown")
    End Sub

    ''' <summary>UOC_Form_Closing</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_Form_Closing(rcFxEventArgs As RcFxEventArgs)
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

#Region "userControl31"

    ''' <summary>UOC_userControl31_btnUCButton1_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_userControl31_btnUCButton1_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl31_btnUCButton1_Click")
    End Sub

    ''' <summary>UOC_userControl31_pbxUCPictureBox1_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_userControl31_pbxUCPictureBox1_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl31_pbxUCPictureBox1_Click")
    End Sub

    ''' <summary>UOC_userControl31_rbnUCRadioButton1_CheckedChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_userControl31_rbnUCRadioButton1_CheckedChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl31_rbnUCRadioButton1_CheckedChanged")
    End Sub

    ''' <summary>UOC_userControl31_cbxUCCheckBox1_CheckedChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_userControl31_cbxUCCheckBox1_CheckedChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl31_cbxUCCheckBox1_CheckedChanged")
    End Sub

    ''' <summary>UOC_userControl31_cbbUCComboBox1_SelectedIndexChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_userControl31_cbbUCComboBox1_SelectedIndexChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl31_cbbUCComboBox1_SelectedIndexChanged")
    End Sub

    ''' <summary>UOC_userControl31_lbxUCListBox1_SelectedIndexChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_userControl31_lbxUCListBox1_SelectedIndexChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl31_lbxUCListBox1_SelectedIndexChanged")
    End Sub

#End Region

#Region "userControl32"

    ''' <summary>UOC_userControl32_btnUCButton1_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_userControl32_btnUCButton1_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl32_btnUCButton1_Click")
    End Sub

    ''' <summary>UOC_userControl32_pbxUCPictureBox1_Click</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_userControl32_pbxUCPictureBox1_Click(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl32_pbxUCPictureBox1_Click")
    End Sub

    ''' <summary>UOC_userControl32_rbnUCRadioButton1_CheckedChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_userControl32_rbnUCRadioButton1_CheckedChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl32_rbnUCRadioButton1_CheckedChanged")
    End Sub

    ''' <summary>UOC_userControl32_cbxUCCheckBox1_CheckedChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_userControl32_cbxUCCheckBox1_CheckedChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl32_cbxUCCheckBox1_CheckedChanged")
    End Sub

    ''' <summary>UOC_userControl32_cbbUCComboBox1_SelectedIndexChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_userControl32_cbbUCComboBox1_SelectedIndexChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl32_cbbUCComboBox1_SelectedIndexChanged")
    End Sub

    ''' <summary>UOC_userControl32_lbxUCListBox1_SelectedIndexChanged</summary>
    ''' <param name="rcFxEventArgs">RcFxEventArgs</param>
    Protected Sub UOC_userControl32_lbxUCListBox1_SelectedIndexChanged(rcFxEventArgs As RcFxEventArgs)
        Debug.WriteLine("UOC_userControl32_lbxUCListBox1_SelectedIndexChanged")
        Me.groupBox3.Controls.RemoveByKey("userControl32") ' ★★
    End Sub

#End Region

#End Region

    ''' <summary>動的に追加したコントロールをLstUserControlに追加する</summary>
    ''' <param name="sender">object</param>
    ''' <param name="e">ControlEventArgs</param>
    Private Sub groupBox3_ControlAdded(sender As Object, e As ControlEventArgs) Handles groupBox3.ControlAdded
        ' UOC_イベントハンドラ内で追加すると例外が発生するのでココに書く。
        If TypeOf e.Control Is UserControl Then
            ' 動的ロード後のコントロール検索＆イベントハンドラ設定
            Me.LstUserControl.Add(DirectCast(e.Control, UserControl))
            RcFxCmnFunction.GetCtrlAndSetClickEventHandler2(
                DirectCast(e.Control, UserControl),
                Me.CreatePrefixAndEvtHndHt(), Me.ControlHt)   ' Base
            RcMyCmnFunction.GetCtrlAndSetClickEventHandler2(
                DirectCast(e.Control, UserControl),
                Me.MyCreatePrefixAndEvtHndHt(), Me.ControlHt) ' MyBase
        End If
    End Sub

    ''' <summary>動的に追加したコントロールをLstUserControlから削除する</summary>
    ''' <param name="sender">object</param>
    ''' <param name="e">ControlEventArgs</param>
    Private Sub groupBox3_ControlRemoved(sender As Object, e As ControlEventArgs) Handles groupBox3.ControlRemoved
        ' UOC_イベントハンドラ内で削除すると例外が発生するのでココに書く。
        If TypeOf e.Control Is UserControl Then
            Me.LstUserControl.Remove(DirectCast(e.Control, UserControl))
        End If
    End Sub

End Class


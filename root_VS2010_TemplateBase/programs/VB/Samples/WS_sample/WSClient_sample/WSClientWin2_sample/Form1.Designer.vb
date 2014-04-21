Imports Touryo.Infrastructure.Business.RichClient.Presentation

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits MyBaseControllerWin

    ''' <summary>
    ''' 必要なデザイナ変数です。
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' 使用中のリソースをすべてクリーンアップします。
    ''' </summary>
    ''' <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows フォーム デザイナで生成されたコード"

    ''' <summary>
    ''' デザイナ サポートに必要なメソッドです。このメソッドの内容を
    ''' コード エディタで変更しないでください。
    ''' </summary>
    Private Sub InitializeComponent()
        Me.btnSync = New System.Windows.Forms.Button()
        Me.txtStatus = New System.Windows.Forms.TextBox()
        Me.btnOpenForm2 = New System.Windows.Forms.Button()
        Me.btnButton1 = New System.Windows.Forms.Button()
        Me.comboBox1 = New System.Windows.Forms.ComboBox()
        Me.btnASync = New System.Windows.Forms.Button()
        Me.numericUpDown1 = New System.Windows.Forms.NumericUpDown()
        Me.label1 = New System.Windows.Forms.Label()
        Me.cbxDialog = New System.Windows.Forms.CheckBox()
        Me.cbxWindow = New System.Windows.Forms.CheckBox()
        Me.cbxClick = New System.Windows.Forms.CheckBox()
        Me.cbxDoClick = New System.Windows.Forms.CheckBox()
        Me.btnButton2 = New System.Windows.Forms.Button()
        Me.btnHdnBtn1 = New Touryo.Infrastructure.Framework.RichClient.Presentation.HiddenButton()
        Me.cbxDoClick2 = New System.Windows.Forms.CheckBox()
        DirectCast(Me.numericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        ' 
        ' btnSync
        ' 
        Me.btnSync.Location = New System.Drawing.Point(197, 4)
        Me.btnSync.Name = "btnSync"
        Me.btnSync.Size = New System.Drawing.Size(75, 23)
        Me.btnSync.TabIndex = 5
        Me.btnSync.Text = "同期実行"
        Me.btnSync.UseVisualStyleBackColor = True
        ' 
        ' txtStatus
        ' 
        Me.txtStatus.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtStatus.Location = New System.Drawing.Point(12, 168)
        Me.txtStatus.MaxLength = 1000000000
        Me.txtStatus.Multiline = True
        Me.txtStatus.Name = "txtStatus"
        Me.txtStatus.[ReadOnly] = True
        Me.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtStatus.Size = New System.Drawing.Size(468, 181)
        Me.txtStatus.TabIndex = 6
        AddHandler Me.txtStatus.TextChanged, New System.EventHandler(AddressOf Me.txtStatus_TextChanged)
        ' 
        ' btnOpenForm2
        ' 
        Me.btnOpenForm2.Location = New System.Drawing.Point(359, 4)
        Me.btnOpenForm2.Name = "btnOpenForm2"
        Me.btnOpenForm2.Size = New System.Drawing.Size(119, 23)
        Me.btnOpenForm2.TabIndex = 10
        Me.btnOpenForm2.Text = "Form2を開く"
        Me.btnOpenForm2.UseVisualStyleBackColor = True
        ' 
        ' btnButton1
        ' 
        Me.btnButton1.Location = New System.Drawing.Point(15, 33)
        Me.btnButton1.Name = "btnButton1"
        Me.btnButton1.Size = New System.Drawing.Size(256, 23)
        Me.btnButton1.TabIndex = 11
        Me.btnButton1.Text = "メソッドの実装されているボタン"
        Me.btnButton1.UseVisualStyleBackColor = True
        ' 
        ' comboBox1
        ' 
        Me.comboBox1.FormattingEnabled = True
        Me.comboBox1.Items.AddRange(New Object() {"テスト（処理中も操作可能１）", "テスト（処理中も操作可能２）", "テスト（処理中も操作可能３）", "テスト（処理中も操作可能４）", "テスト（処理中も操作可能５）", "テスト（処理中も操作可能６）", _
         "テスト（処理中も操作可能７）", "テスト（処理中も操作可能８）", "テスト（処理中も操作可能９）"})
        Me.comboBox1.Location = New System.Drawing.Point(277, 33)
        Me.comboBox1.Name = "comboBox1"
        Me.comboBox1.Size = New System.Drawing.Size(201, 20)
        Me.comboBox1.TabIndex = 12
        ' 
        ' btnASync
        ' 
        Me.btnASync.Location = New System.Drawing.Point(278, 4)
        Me.btnASync.Name = "btnASync"
        Me.btnASync.Size = New System.Drawing.Size(75, 23)
        Me.btnASync.TabIndex = 13
        Me.btnASync.Text = "非同期実行"
        Me.btnASync.UseVisualStyleBackColor = True
        ' 
        ' numericUpDown1
        ' 
        Me.numericUpDown1.Location = New System.Drawing.Point(71, 7)
        Me.numericUpDown1.Name = "numericUpDown1"
        Me.numericUpDown1.Size = New System.Drawing.Size(120, 19)
        Me.numericUpDown1.TabIndex = 14
        ' 
        ' label1
        ' 
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(12, 9)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(53, 12)
        Me.label1.TabIndex = 15
        Me.label1.Text = "実行時間"
        ' 
        ' cbxDialog
        ' 
        Me.cbxDialog.AutoSize = True
        Me.cbxDialog.Location = New System.Drawing.Point(14, 118)
        Me.cbxDialog.Name = "cbxDialog"
        Me.cbxDialog.Size = New System.Drawing.Size(205, 16)
        Me.cbxDialog.TabIndex = 16
        Me.cbxDialog.Text = "(2) 結果表示でダイアログを表示する。"
        Me.cbxDialog.UseVisualStyleBackColor = True
        ' 
        ' cbxWindow
        ' 
        Me.cbxWindow.AutoSize = True
        Me.cbxWindow.Location = New System.Drawing.Point(14, 96)
        Me.cbxWindow.Name = "cbxWindow"
        Me.cbxWindow.Size = New System.Drawing.Size(203, 16)
        Me.cbxWindow.TabIndex = 17
        Me.cbxWindow.Text = "(1) 結果表示でウィンドウを表示する。"
        Me.cbxWindow.UseVisualStyleBackColor = True
        ' 
        ' cbxClick
        ' 
        Me.cbxClick.AutoSize = True
        Me.cbxClick.Location = New System.Drawing.Point(229, 96)
        Me.cbxClick.Name = "cbxClick"
        Me.cbxClick.Size = New System.Drawing.Size(230, 16)
        Me.cbxClick.TabIndex = 18
        Me.cbxClick.Text = "(3) 結果表示でClickイベントを発生させる。"
        Me.cbxClick.UseVisualStyleBackColor = True
        ' 
        ' cbxDoClick
        ' 
        Me.cbxDoClick.AutoSize = True
        Me.cbxDoClick.Location = New System.Drawing.Point(229, 118)
        Me.cbxDoClick.Name = "cbxDoClick"
        Me.cbxDoClick.Size = New System.Drawing.Size(254, 16)
        Me.cbxDoClick.TabIndex = 19
        Me.cbxDoClick.Text = "(4) 結果表示でDoClickでイベントを発生させる。"
        Me.cbxDoClick.UseVisualStyleBackColor = True
        ' 
        ' btnButton2
        ' 
        Me.btnButton2.Location = New System.Drawing.Point(15, 62)
        Me.btnButton2.Name = "btnButton2"
        Me.btnButton2.Size = New System.Drawing.Size(256, 23)
        Me.btnButton2.TabIndex = 20
        Me.btnButton2.Text = "メソッドの実装されていないボタン"
        Me.btnButton2.UseVisualStyleBackColor = True
        ' 
        ' btnHdnBtn1
        ' 
        Me.btnHdnBtn1.Location = New System.Drawing.Point(278, 62)
        Me.btnHdnBtn1.Name = "btnHdnBtn1"
        Me.btnHdnBtn1.Size = New System.Drawing.Size(200, 23)
        Me.btnHdnBtn1.TabIndex = 21
        Me.btnHdnBtn1.Text = "hiddenButton1"
        Me.btnHdnBtn1.UseVisualStyleBackColor = True
        Me.btnHdnBtn1.Visible = False
        ' 
        ' cbxDoClick2
        ' 
        Me.cbxDoClick2.AutoSize = True
        Me.cbxDoClick2.Location = New System.Drawing.Point(14, 140)
        Me.cbxDoClick2.Name = "cbxDoClick2"
        Me.cbxDoClick2.Size = New System.Drawing.Size(458, 16)
        Me.cbxDoClick2.TabIndex = 22
        Me.cbxDoClick2.Text = "(5) 結果表示でDoClickでイベントを発生させ、そのイベント内で更に非同期呼び出しを行う。"
        Me.cbxDoClick2.UseVisualStyleBackColor = True
        ' 
        ' Form1
        ' 
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 12.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(492, 363)
        Me.Controls.Add(Me.cbxDoClick2)
        Me.Controls.Add(Me.btnHdnBtn1)
        Me.Controls.Add(Me.btnButton2)
        Me.Controls.Add(Me.cbxDoClick)
        Me.Controls.Add(Me.cbxClick)
        Me.Controls.Add(Me.cbxWindow)
        Me.Controls.Add(Me.cbxDialog)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.numericUpDown1)
        Me.Controls.Add(Me.btnASync)
        Me.Controls.Add(Me.comboBox1)
        Me.Controls.Add(Me.btnButton1)
        Me.Controls.Add(Me.btnOpenForm2)
        Me.Controls.Add(Me.txtStatus)
        Me.Controls.Add(Me.btnSync)
        Me.Name = "Form1"
        Me.Text = "Form1"
        DirectCast(Me.numericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private btnSync As System.Windows.Forms.Button
    Private txtStatus As System.Windows.Forms.TextBox
    Private btnOpenForm2 As System.Windows.Forms.Button
    Private btnButton1 As System.Windows.Forms.Button
    Private comboBox1 As System.Windows.Forms.ComboBox
    Private btnASync As System.Windows.Forms.Button
    Private numericUpDown1 As System.Windows.Forms.NumericUpDown
    Private label1 As System.Windows.Forms.Label
    Private cbxDialog As System.Windows.Forms.CheckBox
    Private cbxWindow As System.Windows.Forms.CheckBox
    Private cbxClick As System.Windows.Forms.CheckBox
    Private cbxDoClick As System.Windows.Forms.CheckBox
    Private btnButton2 As System.Windows.Forms.Button
    Private btnHdnBtn1 As Touryo.Infrastructure.Framework.RichClient.Presentation.HiddenButton
    Private cbxDoClick2 As System.Windows.Forms.CheckBox

End Class

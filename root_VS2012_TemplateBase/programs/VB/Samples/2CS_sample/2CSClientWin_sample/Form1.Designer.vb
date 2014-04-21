Imports Touryo.Infrastructure.Business.RichClient.Presentation

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits MyBaseControllerWin

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.label11 = New System.Windows.Forms.Label
        Me.labelMessage = New System.Windows.Forms.Label
        Me.btnButton10 = New System.Windows.Forms.Button
        Me.btnButton9 = New System.Windows.Forms.Button
        Me.btnButton8 = New System.Windows.Forms.Button
        Me.btnButton7 = New System.Windows.Forms.Button
        Me.btnButton6 = New System.Windows.Forms.Button
        Me.btnButton5 = New System.Windows.Forms.Button
        Me.btnButton4 = New System.Windows.Forms.Button
        Me.btnButton3 = New System.Windows.Forms.Button
        Me.btnButton2 = New System.Windows.Forms.Button
        Me.btnButton1 = New System.Windows.Forms.Button
        Me.dataGridView1 = New System.Windows.Forms.DataGridView
        Me.label10 = New System.Windows.Forms.Label
        Me.ddlOrderSequence = New System.Windows.Forms.ComboBox
        Me.label9 = New System.Windows.Forms.Label
        Me.ddlOrderColumn = New System.Windows.Forms.ComboBox
        Me.textBox3 = New System.Windows.Forms.TextBox
        Me.textBox2 = New System.Windows.Forms.TextBox
        Me.label8 = New System.Windows.Forms.Label
        Me.label7 = New System.Windows.Forms.Label
        Me.label6 = New System.Windows.Forms.Label
        Me.textBox1 = New System.Windows.Forms.TextBox
        Me.label5 = New System.Windows.Forms.Label
        Me.ddlExRollback = New System.Windows.Forms.ComboBox
        Me.label4 = New System.Windows.Forms.Label
        Me.ddlIso = New System.Windows.Forms.ComboBox
        Me.label3 = New System.Windows.Forms.Label
        Me.ddlMode2 = New System.Windows.Forms.ComboBox
        Me.label2 = New System.Windows.Forms.Label
        Me.label1 = New System.Windows.Forms.Label
        Me.ddlMode1 = New System.Windows.Forms.ComboBox
        Me.ddlDap = New System.Windows.Forms.ComboBox
        Me.label16 = New System.Windows.Forms.Label
        Me.textBox7 = New System.Windows.Forms.TextBox
        Me.btnButton12 = New System.Windows.Forms.Button
        Me.label15 = New System.Windows.Forms.Label
        Me.textBox6 = New System.Windows.Forms.TextBox
        Me.label14 = New System.Windows.Forms.Label
        Me.textBox5 = New System.Windows.Forms.TextBox
        Me.btnButton11 = New System.Windows.Forms.Button
        Me.label13 = New System.Windows.Forms.Label
        Me.textBox4 = New System.Windows.Forms.TextBox
        CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'label11
        '
        Me.label11.AutoSize = True
        Me.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label11.Location = New System.Drawing.Point(10, 406)
        Me.label11.Name = "label11"
        Me.label11.Size = New System.Drawing.Size(59, 12)
        Me.label11.TabIndex = 69
        Me.label11.Text = "処理結果："
        '
        'labelMessage
        '
        Me.labelMessage.AutoSize = True
        Me.labelMessage.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.labelMessage.Location = New System.Drawing.Point(108, 406)
        Me.labelMessage.Name = "labelMessage"
        Me.labelMessage.Size = New System.Drawing.Size(23, 12)
        Me.labelMessage.TabIndex = 68
        Me.labelMessage.Text = "***"
        '
        'btnButton10
        '
        Me.btnButton10.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnButton10.Location = New System.Drawing.Point(274, 373)
        Me.btnButton10.Name = "btnButton10"
        Me.btnButton10.Size = New System.Drawing.Size(464, 23)
        Me.btnButton10.TabIndex = 67
        Me.btnButton10.Text = "クリア"
        Me.btnButton10.UseVisualStyleBackColor = True
        '
        'btnButton9
        '
        Me.btnButton9.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnButton9.Location = New System.Drawing.Point(450, 470)
        Me.btnButton9.Name = "btnButton9"
        Me.btnButton9.Size = New System.Drawing.Size(140, 23)
        Me.btnButton9.TabIndex = 66
        Me.btnButton9.Text = "削除"
        Me.btnButton9.UseVisualStyleBackColor = True
        '
        'btnButton8
        '
        Me.btnButton8.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnButton8.Location = New System.Drawing.Point(304, 470)
        Me.btnButton8.Name = "btnButton8"
        Me.btnButton8.Size = New System.Drawing.Size(140, 23)
        Me.btnButton8.TabIndex = 65
        Me.btnButton8.Text = "更新"
        Me.btnButton8.UseVisualStyleBackColor = True
        '
        'btnButton7
        '
        Me.btnButton7.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnButton7.Location = New System.Drawing.Point(158, 470)
        Me.btnButton7.Name = "btnButton7"
        Me.btnButton7.Size = New System.Drawing.Size(140, 23)
        Me.btnButton7.TabIndex = 64
        Me.btnButton7.Text = "追加"
        Me.btnButton7.UseVisualStyleBackColor = True
        '
        'btnButton6
        '
        Me.btnButton6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnButton6.Location = New System.Drawing.Point(12, 470)
        Me.btnButton6.Name = "btnButton6"
        Me.btnButton6.Size = New System.Drawing.Size(140, 23)
        Me.btnButton6.TabIndex = 63
        Me.btnButton6.Text = "一件参照"
        Me.btnButton6.UseVisualStyleBackColor = True
        '
        'btnButton5
        '
        Me.btnButton5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnButton5.Location = New System.Drawing.Point(596, 441)
        Me.btnButton5.Name = "btnButton5"
        Me.btnButton5.Size = New System.Drawing.Size(140, 23)
        Me.btnButton5.TabIndex = 62
        Me.btnButton5.Text = "一覧取得（動的SQL）"
        Me.btnButton5.UseVisualStyleBackColor = True
        '
        'btnButton4
        '
        Me.btnButton4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnButton4.Location = New System.Drawing.Point(450, 441)
        Me.btnButton4.Name = "btnButton4"
        Me.btnButton4.Size = New System.Drawing.Size(140, 23)
        Me.btnButton4.TabIndex = 61
        Me.btnButton4.Text = "一覧取得（dr）"
        Me.btnButton4.UseVisualStyleBackColor = True
        '
        'btnButton3
        '
        Me.btnButton3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnButton3.Location = New System.Drawing.Point(304, 441)
        Me.btnButton3.Name = "btnButton3"
        Me.btnButton3.Size = New System.Drawing.Size(140, 23)
        Me.btnButton3.TabIndex = 60
        Me.btnButton3.Text = "一覧取得（ds）"
        Me.btnButton3.UseVisualStyleBackColor = True
        '
        'btnButton2
        '
        Me.btnButton2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnButton2.Location = New System.Drawing.Point(158, 441)
        Me.btnButton2.Name = "btnButton2"
        Me.btnButton2.Size = New System.Drawing.Size(140, 23)
        Me.btnButton2.TabIndex = 59
        Me.btnButton2.Text = "一覧取得（dt）"
        Me.btnButton2.UseVisualStyleBackColor = True
        '
        'btnButton1
        '
        Me.btnButton1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnButton1.Location = New System.Drawing.Point(12, 441)
        Me.btnButton1.Name = "btnButton1"
        Me.btnButton1.Size = New System.Drawing.Size(140, 23)
        Me.btnButton1.TabIndex = 58
        Me.btnButton1.Text = "件数取得"
        Me.btnButton1.UseVisualStyleBackColor = True
        '
        'dataGridView1
        '
        Me.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dataGridView1.Location = New System.Drawing.Point(274, 9)
        Me.dataGridView1.Name = "dataGridView1"
        Me.dataGridView1.RowTemplate.Height = 21
        Me.dataGridView1.Size = New System.Drawing.Size(464, 358)
        Me.dataGridView1.TabIndex = 57
        '
        'label10
        '
        Me.label10.AutoSize = True
        Me.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label10.Location = New System.Drawing.Point(10, 355)
        Me.label10.Name = "label10"
        Me.label10.Size = New System.Drawing.Size(59, 12)
        Me.label10.TabIndex = 56
        Me.label10.Text = "昇順・降順"
        '
        'ddlOrderSequence
        '
        Me.ddlOrderSequence.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlOrderSequence.FormattingEnabled = True
        Me.ddlOrderSequence.Location = New System.Drawing.Point(12, 370)
        Me.ddlOrderSequence.Name = "ddlOrderSequence"
        Me.ddlOrderSequence.Size = New System.Drawing.Size(250, 20)
        Me.ddlOrderSequence.TabIndex = 55
        '
        'label9
        '
        Me.label9.AutoSize = True
        Me.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label9.Location = New System.Drawing.Point(10, 313)
        Me.label9.Name = "label9"
        Me.label9.Size = New System.Drawing.Size(84, 12)
        Me.label9.TabIndex = 54
        Me.label9.Text = "並び替え対象列"
        '
        'ddlOrderColumn
        '
        Me.ddlOrderColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlOrderColumn.FormattingEnabled = True
        Me.ddlOrderColumn.Location = New System.Drawing.Point(12, 328)
        Me.ddlOrderColumn.Name = "ddlOrderColumn"
        Me.ddlOrderColumn.Size = New System.Drawing.Size(250, 20)
        Me.ddlOrderColumn.TabIndex = 53
        '
        'textBox3
        '
        Me.textBox3.Location = New System.Drawing.Point(110, 267)
        Me.textBox3.Name = "textBox3"
        Me.textBox3.Size = New System.Drawing.Size(152, 19)
        Me.textBox3.TabIndex = 52
        '
        'textBox2
        '
        Me.textBox2.Location = New System.Drawing.Point(110, 246)
        Me.textBox2.Name = "textBox2"
        Me.textBox2.Size = New System.Drawing.Size(152, 19)
        Me.textBox2.TabIndex = 51
        '
        'label8
        '
        Me.label8.AutoSize = True
        Me.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label8.Location = New System.Drawing.Point(10, 270)
        Me.label8.Name = "label8"
        Me.label8.Size = New System.Drawing.Size(42, 12)
        Me.label8.TabIndex = 50
        Me.label8.Text = "Phone："
        '
        'label7
        '
        Me.label7.AutoSize = True
        Me.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label7.Location = New System.Drawing.Point(10, 249)
        Me.label7.Name = "label7"
        Me.label7.Size = New System.Drawing.Size(87, 12)
        Me.label7.TabIndex = 49
        Me.label7.Text = "CompanyName："
        '
        'label6
        '
        Me.label6.AutoSize = True
        Me.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label6.Location = New System.Drawing.Point(10, 229)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(60, 12)
        Me.label6.TabIndex = 48
        Me.label6.Text = "ShipperID："
        '
        'textBox1
        '
        Me.textBox1.Location = New System.Drawing.Point(110, 226)
        Me.textBox1.Name = "textBox1"
        Me.textBox1.Size = New System.Drawing.Size(152, 19)
        Me.textBox1.TabIndex = 47
        '
        'label5
        '
        Me.label5.AutoSize = True
        Me.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label5.Location = New System.Drawing.Point(10, 161)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(258, 12)
        Me.label5.TabIndex = 46
        Me.label5.Text = "コミット、ロールバックを設定（例外発生時、ロールバック"
        '
        'ddlExRollback
        '
        Me.ddlExRollback.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlExRollback.FormattingEnabled = True
        Me.ddlExRollback.Location = New System.Drawing.Point(12, 176)
        Me.ddlExRollback.Name = "ddlExRollback"
        Me.ddlExRollback.Size = New System.Drawing.Size(250, 20)
        Me.ddlExRollback.TabIndex = 45
        '
        'label4
        '
        Me.label4.AutoSize = True
        Me.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label4.Location = New System.Drawing.Point(10, 123)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(91, 12)
        Me.label4.TabIndex = 44
        Me.label4.Text = "分離レベルを選択"
        '
        'ddlIso
        '
        Me.ddlIso.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlIso.FormattingEnabled = True
        Me.ddlIso.Location = New System.Drawing.Point(12, 138)
        Me.ddlIso.Name = "ddlIso"
        Me.ddlIso.Size = New System.Drawing.Size(250, 20)
        Me.ddlIso.TabIndex = 43
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label3.Location = New System.Drawing.Point(10, 85)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(252, 12)
        Me.label3.TabIndex = 42
        Me.label3.Text = "静的、動的のクエリ モードを選択（共通Dao選択時）"
        '
        'ddlMode2
        '
        Me.ddlMode2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlMode2.FormattingEnabled = True
        Me.ddlMode2.Location = New System.Drawing.Point(12, 100)
        Me.ddlMode2.Name = "ddlMode2"
        Me.ddlMode2.Size = New System.Drawing.Size(250, 20)
        Me.ddlMode2.TabIndex = 41
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label2.Location = New System.Drawing.Point(10, 47)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(151, 12)
        Me.label2.TabIndex = 40
        Me.label2.Text = "個別、共通のＤａｏ種別を選択"
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label1.Location = New System.Drawing.Point(12, 9)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(237, 12)
        Me.label1.TabIndex = 39
        Me.label1.Text = "データアクセス制御クラス（データプロバイダ）を選択"
        '
        'ddlMode1
        '
        Me.ddlMode1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlMode1.FormattingEnabled = True
        Me.ddlMode1.Location = New System.Drawing.Point(12, 62)
        Me.ddlMode1.Name = "ddlMode1"
        Me.ddlMode1.Size = New System.Drawing.Size(250, 20)
        Me.ddlMode1.TabIndex = 38
        '
        'ddlDap
        '
        Me.ddlDap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlDap.FormattingEnabled = True
        Me.ddlDap.Location = New System.Drawing.Point(12, 24)
        Me.ddlDap.Name = "ddlDap"
        Me.ddlDap.Size = New System.Drawing.Size(250, 20)
        Me.ddlDap.TabIndex = 37
        '
        'label16
        '
        Me.label16.AutoSize = True
        Me.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label16.Location = New System.Drawing.Point(235, 536)
        Me.label16.Name = "label16"
        Me.label16.Size = New System.Drawing.Size(23, 12)
        Me.label16.TabIndex = 101
        Me.label16.Text = "値："
        '
        'textBox7
        '
        Me.textBox7.Location = New System.Drawing.Point(297, 533)
        Me.textBox7.Name = "textBox7"
        Me.textBox7.Size = New System.Drawing.Size(439, 19)
        Me.textBox7.TabIndex = 99
        '
        'btnButton12
        '
        Me.btnButton12.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnButton12.Location = New System.Drawing.Point(187, 531)
        Me.btnButton12.Name = "btnButton12"
        Me.btnButton12.Size = New System.Drawing.Size(42, 23)
        Me.btnButton12.TabIndex = 98
        Me.btnButton12.Text = "→"
        Me.btnButton12.UseVisualStyleBackColor = True
        '
        'label15
        '
        Me.label15.AutoSize = True
        Me.label15.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label15.Location = New System.Drawing.Point(8, 536)
        Me.label15.Name = "label15"
        Me.label15.Size = New System.Drawing.Size(31, 12)
        Me.label15.TabIndex = 100
        Me.label15.Text = "キー："
        '
        'textBox6
        '
        Me.textBox6.Location = New System.Drawing.Point(81, 533)
        Me.textBox6.Name = "textBox6"
        Me.textBox6.Size = New System.Drawing.Size(100, 19)
        Me.textBox6.TabIndex = 97
        '
        'label14
        '
        Me.label14.AutoSize = True
        Me.label14.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label14.Location = New System.Drawing.Point(235, 511)
        Me.label14.Name = "label14"
        Me.label14.Size = New System.Drawing.Size(56, 12)
        Me.label14.TabIndex = 96
        Me.label14.Text = "メッセージ："
        '
        'textBox5
        '
        Me.textBox5.Location = New System.Drawing.Point(297, 508)
        Me.textBox5.Name = "textBox5"
        Me.textBox5.Size = New System.Drawing.Size(439, 19)
        Me.textBox5.TabIndex = 94
        '
        'btnButton11
        '
        Me.btnButton11.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnButton11.Location = New System.Drawing.Point(187, 506)
        Me.btnButton11.Name = "btnButton11"
        Me.btnButton11.Size = New System.Drawing.Size(42, 23)
        Me.btnButton11.TabIndex = 93
        Me.btnButton11.Text = "→"
        Me.btnButton11.UseVisualStyleBackColor = True
        '
        'label13
        '
        Me.label13.AutoSize = True
        Me.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label13.Location = New System.Drawing.Point(8, 511)
        Me.label13.Name = "label13"
        Me.label13.Size = New System.Drawing.Size(67, 12)
        Me.label13.TabIndex = 95
        Me.label13.Text = "メッセージID："
        '
        'textBox4
        '
        Me.textBox4.Location = New System.Drawing.Point(81, 508)
        Me.textBox4.Name = "textBox4"
        Me.textBox4.Size = New System.Drawing.Size(100, 19)
        Me.textBox4.TabIndex = 92
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(748, 563)
        Me.Controls.Add(Me.label16)
        Me.Controls.Add(Me.textBox7)
        Me.Controls.Add(Me.btnButton12)
        Me.Controls.Add(Me.label15)
        Me.Controls.Add(Me.textBox6)
        Me.Controls.Add(Me.label14)
        Me.Controls.Add(Me.textBox5)
        Me.Controls.Add(Me.btnButton11)
        Me.Controls.Add(Me.label13)
        Me.Controls.Add(Me.textBox4)
        Me.Controls.Add(Me.label11)
        Me.Controls.Add(Me.labelMessage)
        Me.Controls.Add(Me.btnButton10)
        Me.Controls.Add(Me.btnButton9)
        Me.Controls.Add(Me.btnButton8)
        Me.Controls.Add(Me.btnButton7)
        Me.Controls.Add(Me.btnButton6)
        Me.Controls.Add(Me.btnButton5)
        Me.Controls.Add(Me.btnButton4)
        Me.Controls.Add(Me.btnButton3)
        Me.Controls.Add(Me.btnButton2)
        Me.Controls.Add(Me.btnButton1)
        Me.Controls.Add(Me.dataGridView1)
        Me.Controls.Add(Me.label10)
        Me.Controls.Add(Me.ddlOrderSequence)
        Me.Controls.Add(Me.label9)
        Me.Controls.Add(Me.ddlOrderColumn)
        Me.Controls.Add(Me.textBox3)
        Me.Controls.Add(Me.textBox2)
        Me.Controls.Add(Me.label8)
        Me.Controls.Add(Me.label7)
        Me.Controls.Add(Me.label6)
        Me.Controls.Add(Me.textBox1)
        Me.Controls.Add(Me.label5)
        Me.Controls.Add(Me.ddlExRollback)
        Me.Controls.Add(Me.label4)
        Me.Controls.Add(Me.ddlIso)
        Me.Controls.Add(Me.label3)
        Me.Controls.Add(Me.ddlMode2)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.ddlMode1)
        Me.Controls.Add(Me.ddlDap)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents label11 As System.Windows.Forms.Label
    Private WithEvents labelMessage As System.Windows.Forms.Label
    Private WithEvents btnButton10 As System.Windows.Forms.Button
    Private WithEvents btnButton9 As System.Windows.Forms.Button
    Private WithEvents btnButton8 As System.Windows.Forms.Button
    Private WithEvents btnButton7 As System.Windows.Forms.Button
    Private WithEvents btnButton6 As System.Windows.Forms.Button
    Private WithEvents btnButton5 As System.Windows.Forms.Button
    Private WithEvents btnButton4 As System.Windows.Forms.Button
    Private WithEvents btnButton3 As System.Windows.Forms.Button
    Private WithEvents btnButton2 As System.Windows.Forms.Button
    Private WithEvents btnButton1 As System.Windows.Forms.Button
    Private WithEvents dataGridView1 As System.Windows.Forms.DataGridView
    Private WithEvents label10 As System.Windows.Forms.Label
    Private WithEvents ddlOrderSequence As System.Windows.Forms.ComboBox
    Private WithEvents label9 As System.Windows.Forms.Label
    Private WithEvents ddlOrderColumn As System.Windows.Forms.ComboBox
    Private WithEvents textBox3 As System.Windows.Forms.TextBox
    Private WithEvents textBox2 As System.Windows.Forms.TextBox
    Private WithEvents label8 As System.Windows.Forms.Label
    Private WithEvents label7 As System.Windows.Forms.Label
    Private WithEvents label6 As System.Windows.Forms.Label
    Private WithEvents textBox1 As System.Windows.Forms.TextBox
    Private WithEvents label5 As System.Windows.Forms.Label
    Private WithEvents ddlExRollback As System.Windows.Forms.ComboBox
    Private WithEvents label4 As System.Windows.Forms.Label
    Private WithEvents ddlIso As System.Windows.Forms.ComboBox
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents ddlMode2 As System.Windows.Forms.ComboBox
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents ddlMode1 As System.Windows.Forms.ComboBox
    Private WithEvents ddlDap As System.Windows.Forms.ComboBox
    Private WithEvents label16 As System.Windows.Forms.Label
    Private WithEvents textBox7 As System.Windows.Forms.TextBox
    Private WithEvents btnButton12 As System.Windows.Forms.Button
    Private WithEvents label15 As System.Windows.Forms.Label
    Private WithEvents textBox6 As System.Windows.Forms.TextBox
    Private WithEvents label14 As System.Windows.Forms.Label
    Private WithEvents textBox5 As System.Windows.Forms.TextBox
    Private WithEvents btnButton11 As System.Windows.Forms.Button
    Private WithEvents label13 As System.Windows.Forms.Label
    Private WithEvents textBox4 As System.Windows.Forms.TextBox
End Class

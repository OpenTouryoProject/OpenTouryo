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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.ddlTransmission = New System.Windows.Forms.ComboBox
        Me.label6 = New System.Windows.Forms.Label
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
        Me.label1 = New System.Windows.Forms.Label
        Me.textBox1 = New System.Windows.Forms.TextBox
        Me.label5 = New System.Windows.Forms.Label
        Me.ddlExRollback = New System.Windows.Forms.ComboBox
        Me.label4 = New System.Windows.Forms.Label
        Me.ddlIso = New System.Windows.Forms.ComboBox
        Me.label3 = New System.Windows.Forms.Label
        Me.ddlMode2 = New System.Windows.Forms.ComboBox
        Me.label2 = New System.Windows.Forms.Label
        Me.label12 = New System.Windows.Forms.Label
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
        'ddlTransmission
        '
        Me.ddlTransmission.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlTransmission.FormattingEnabled = True
        resources.ApplyResources(Me.ddlTransmission, "ddlTransmission")
        Me.ddlTransmission.Name = "ddlTransmission"
        '
        'label6
        '
        resources.ApplyResources(Me.label6, "label6")
        Me.label6.Name = "label6"
        '
        'label11
        '
        resources.ApplyResources(Me.label11, "label11")
        Me.label11.Name = "label11"
        '
        'labelMessage
        '
        resources.ApplyResources(Me.labelMessage, "labelMessage")
        Me.labelMessage.Name = "labelMessage"
        '
        'btnButton10
        '
        resources.ApplyResources(Me.btnButton10, "btnButton10")
        Me.btnButton10.Name = "btnButton10"
        Me.btnButton10.UseVisualStyleBackColor = True
        '
        'btnButton9
        '
        resources.ApplyResources(Me.btnButton9, "btnButton9")
        Me.btnButton9.Name = "btnButton9"
        Me.btnButton9.UseVisualStyleBackColor = True
        '
        'btnButton8
        '
        resources.ApplyResources(Me.btnButton8, "btnButton8")
        Me.btnButton8.Name = "btnButton8"
        Me.btnButton8.UseVisualStyleBackColor = True
        '
        'btnButton7
        '
        resources.ApplyResources(Me.btnButton7, "btnButton7")
        Me.btnButton7.Name = "btnButton7"
        Me.btnButton7.UseVisualStyleBackColor = True
        '
        'btnButton6
        '
        resources.ApplyResources(Me.btnButton6, "btnButton6")
        Me.btnButton6.Name = "btnButton6"
        Me.btnButton6.UseVisualStyleBackColor = True
        '
        'btnButton5
        '
        resources.ApplyResources(Me.btnButton5, "btnButton5")
        Me.btnButton5.Name = "btnButton5"
        Me.btnButton5.UseVisualStyleBackColor = True
        '
        'btnButton4
        '
        resources.ApplyResources(Me.btnButton4, "btnButton4")
        Me.btnButton4.Name = "btnButton4"
        Me.btnButton4.UseVisualStyleBackColor = True
        '
        'btnButton3
        '
        resources.ApplyResources(Me.btnButton3, "btnButton3")
        Me.btnButton3.Name = "btnButton3"
        Me.btnButton3.UseVisualStyleBackColor = True
        '
        'btnButton2
        '
        resources.ApplyResources(Me.btnButton2, "btnButton2")
        Me.btnButton2.Name = "btnButton2"
        Me.btnButton2.UseVisualStyleBackColor = True
        '
        'btnButton1
        '
        resources.ApplyResources(Me.btnButton1, "btnButton1")
        Me.btnButton1.Name = "btnButton1"
        Me.btnButton1.UseVisualStyleBackColor = True
        '
        'dataGridView1
        '
        Me.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dataGridView1, "dataGridView1")
        Me.dataGridView1.Name = "dataGridView1"
        Me.dataGridView1.RowTemplate.Height = 21
        '
        'label10
        '
        resources.ApplyResources(Me.label10, "label10")
        Me.label10.Name = "label10"
        '
        'ddlOrderSequence
        '
        Me.ddlOrderSequence.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlOrderSequence.FormattingEnabled = True
        resources.ApplyResources(Me.ddlOrderSequence, "ddlOrderSequence")
        Me.ddlOrderSequence.Name = "ddlOrderSequence"
        '
        'label9
        '
        resources.ApplyResources(Me.label9, "label9")
        Me.label9.Name = "label9"
        '
        'ddlOrderColumn
        '
        Me.ddlOrderColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlOrderColumn.FormattingEnabled = True
        resources.ApplyResources(Me.ddlOrderColumn, "ddlOrderColumn")
        Me.ddlOrderColumn.Name = "ddlOrderColumn"
        '
        'textBox3
        '
        resources.ApplyResources(Me.textBox3, "textBox3")
        Me.textBox3.Name = "textBox3"
        '
        'textBox2
        '
        resources.ApplyResources(Me.textBox2, "textBox2")
        Me.textBox2.Name = "textBox2"
        '
        'label8
        '
        resources.ApplyResources(Me.label8, "label8")
        Me.label8.Name = "label8"
        '
        'label7
        '
        resources.ApplyResources(Me.label7, "label7")
        Me.label7.Name = "label7"
        '
        'label1
        '
        resources.ApplyResources(Me.label1, "label1")
        Me.label1.Name = "label1"
        '
        'textBox1
        '
        resources.ApplyResources(Me.textBox1, "textBox1")
        Me.textBox1.Name = "textBox1"
        '
        'label5
        '
        resources.ApplyResources(Me.label5, "label5")
        Me.label5.Name = "label5"
        '
        'ddlExRollback
        '
        Me.ddlExRollback.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlExRollback.FormattingEnabled = True
        resources.ApplyResources(Me.ddlExRollback, "ddlExRollback")
        Me.ddlExRollback.Name = "ddlExRollback"
        '
        'label4
        '
        resources.ApplyResources(Me.label4, "label4")
        Me.label4.Name = "label4"
        '
        'ddlIso
        '
        Me.ddlIso.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlIso.FormattingEnabled = True
        resources.ApplyResources(Me.ddlIso, "ddlIso")
        Me.ddlIso.Name = "ddlIso"
        '
        'label3
        '
        resources.ApplyResources(Me.label3, "label3")
        Me.label3.Name = "label3"
        '
        'ddlMode2
        '
        Me.ddlMode2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlMode2.FormattingEnabled = True
        resources.ApplyResources(Me.ddlMode2, "ddlMode2")
        Me.ddlMode2.Name = "ddlMode2"
        '
        'label2
        '
        resources.ApplyResources(Me.label2, "label2")
        Me.label2.Name = "label2"
        '
        'label12
        '
        resources.ApplyResources(Me.label12, "label12")
        Me.label12.Name = "label12"
        '
        'ddlMode1
        '
        Me.ddlMode1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlMode1.FormattingEnabled = True
        resources.ApplyResources(Me.ddlMode1, "ddlMode1")
        Me.ddlMode1.Name = "ddlMode1"
        '
        'ddlDap
        '
        Me.ddlDap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ddlDap.FormattingEnabled = True
        resources.ApplyResources(Me.ddlDap, "ddlDap")
        Me.ddlDap.Name = "ddlDap"
        '
        'label16
        '
        resources.ApplyResources(Me.label16, "label16")
        Me.label16.Name = "label16"
        '
        'textBox7
        '
        resources.ApplyResources(Me.textBox7, "textBox7")
        Me.textBox7.Name = "textBox7"
        '
        'btnButton12
        '
        resources.ApplyResources(Me.btnButton12, "btnButton12")
        Me.btnButton12.Name = "btnButton12"
        Me.btnButton12.UseVisualStyleBackColor = True
        '
        'label15
        '
        resources.ApplyResources(Me.label15, "label15")
        Me.label15.Name = "label15"
        '
        'textBox6
        '
        resources.ApplyResources(Me.textBox6, "textBox6")
        Me.textBox6.Name = "textBox6"
        '
        'label14
        '
        resources.ApplyResources(Me.label14, "label14")
        Me.label14.Name = "label14"
        '
        'textBox5
        '
        resources.ApplyResources(Me.textBox5, "textBox5")
        Me.textBox5.Name = "textBox5"
        '
        'btnButton11
        '
        resources.ApplyResources(Me.btnButton11, "btnButton11")
        Me.btnButton11.Name = "btnButton11"
        Me.btnButton11.UseVisualStyleBackColor = True
        '
        'label13
        '
        resources.ApplyResources(Me.label13, "label13")
        Me.label13.Name = "label13"
        '
        'textBox4
        '
        resources.ApplyResources(Me.textBox4, "textBox4")
        Me.textBox4.Name = "textBox4"
        '
        'Form1
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
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
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.textBox1)
        Me.Controls.Add(Me.label5)
        Me.Controls.Add(Me.ddlExRollback)
        Me.Controls.Add(Me.label4)
        Me.Controls.Add(Me.ddlIso)
        Me.Controls.Add(Me.label3)
        Me.Controls.Add(Me.ddlMode2)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.label12)
        Me.Controls.Add(Me.ddlMode1)
        Me.Controls.Add(Me.ddlDap)
        Me.Controls.Add(Me.label6)
        Me.Controls.Add(Me.ddlTransmission)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form1"
        CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private ddlTransmission As System.Windows.Forms.ComboBox
    Private label6 As System.Windows.Forms.Label
    Private label11 As System.Windows.Forms.Label
    Private labelMessage As System.Windows.Forms.Label
    Private btnButton10 As System.Windows.Forms.Button
    Private btnButton9 As System.Windows.Forms.Button
    Private btnButton8 As System.Windows.Forms.Button
    Private btnButton7 As System.Windows.Forms.Button
    Private btnButton6 As System.Windows.Forms.Button
    Private btnButton5 As System.Windows.Forms.Button
    Private btnButton4 As System.Windows.Forms.Button
    Private btnButton3 As System.Windows.Forms.Button
    Private btnButton2 As System.Windows.Forms.Button
    Private btnButton1 As System.Windows.Forms.Button
    Private dataGridView1 As System.Windows.Forms.DataGridView
    Private label10 As System.Windows.Forms.Label
    Private ddlOrderSequence As System.Windows.Forms.ComboBox
    Private label9 As System.Windows.Forms.Label
    Private ddlOrderColumn As System.Windows.Forms.ComboBox
    Private textBox3 As System.Windows.Forms.TextBox
    Private textBox2 As System.Windows.Forms.TextBox
    Private label8 As System.Windows.Forms.Label
    Private label7 As System.Windows.Forms.Label
    Private label1 As System.Windows.Forms.Label
    Private textBox1 As System.Windows.Forms.TextBox
    Private label5 As System.Windows.Forms.Label
    Private ddlExRollback As System.Windows.Forms.ComboBox
    Private label4 As System.Windows.Forms.Label
    Private ddlIso As System.Windows.Forms.ComboBox
    Private label3 As System.Windows.Forms.Label
    Private ddlMode2 As System.Windows.Forms.ComboBox
    Private label2 As System.Windows.Forms.Label
    Private label12 As System.Windows.Forms.Label
    Private ddlMode1 As System.Windows.Forms.ComboBox
    Private ddlDap As System.Windows.Forms.ComboBox
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

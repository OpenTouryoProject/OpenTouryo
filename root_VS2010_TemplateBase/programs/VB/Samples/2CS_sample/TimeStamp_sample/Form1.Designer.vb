<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

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
        Me.btnClearTS = New System.Windows.Forms.Button
        Me.groupBox2 = New System.Windows.Forms.GroupBox
        Me.btnDynIns = New System.Windows.Forms.Button
        Me.btnDynSel = New System.Windows.Forms.Button
        Me.btnDynDel = New System.Windows.Forms.Button
        Me.btnDynUpd = New System.Windows.Forms.Button
        Me.groupBox1 = New System.Windows.Forms.GroupBox
        Me.btnInsert = New System.Windows.Forms.Button
        Me.btnSelect = New System.Windows.Forms.Button
        Me.btnUpdate = New System.Windows.Forms.Button
        Me.btnDelete = New System.Windows.Forms.Button
        Me.label5 = New System.Windows.Forms.Label
        Me.txtTS = New System.Windows.Forms.TextBox
        Me.label4 = New System.Windows.Forms.Label
        Me.txtVAL = New System.Windows.Forms.TextBox
        Me.label3 = New System.Windows.Forms.Label
        Me.cmbTableType = New System.Windows.Forms.ComboBox
        Me.label2 = New System.Windows.Forms.Label
        Me.cmbTSColType = New System.Windows.Forms.ComboBox
        Me.label1 = New System.Windows.Forms.Label
        Me.txtID = New System.Windows.Forms.TextBox
        Me.btnClear = New System.Windows.Forms.Button
        Me.btnGetAll = New System.Windows.Forms.Button
        Me.dataGridView1 = New System.Windows.Forms.DataGridView
        Me.groupBox2.SuspendLayout()
        Me.groupBox1.SuspendLayout()
        CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnClearTS
        '
        Me.btnClearTS.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnClearTS.Location = New System.Drawing.Point(17, 128)
        Me.btnClearTS.Name = "btnClearTS"
        Me.btnClearTS.Size = New System.Drawing.Size(168, 23)
        Me.btnClearTS.TabIndex = 40
        Me.btnClearTS.Text = "TSをクリア"
        Me.btnClearTS.UseVisualStyleBackColor = True
        '
        'groupBox2
        '
        Me.groupBox2.Controls.Add(Me.btnDynIns)
        Me.groupBox2.Controls.Add(Me.btnDynSel)
        Me.groupBox2.Controls.Add(Me.btnDynDel)
        Me.groupBox2.Controls.Add(Me.btnDynUpd)
        Me.groupBox2.Location = New System.Drawing.Point(19, 313)
        Me.groupBox2.Name = "groupBox2"
        Me.groupBox2.Size = New System.Drawing.Size(166, 138)
        Me.groupBox2.TabIndex = 39
        Me.groupBox2.TabStop = False
        Me.groupBox2.Text = "動的"
        '
        'btnDynIns
        '
        Me.btnDynIns.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnDynIns.Location = New System.Drawing.Point(15, 18)
        Me.btnDynIns.Name = "btnDynIns"
        Me.btnDynIns.Size = New System.Drawing.Size(136, 23)
        Me.btnDynIns.TabIndex = 17
        Me.btnDynIns.Text = "DynIns"
        Me.btnDynIns.UseVisualStyleBackColor = True
        '
        'btnDynSel
        '
        Me.btnDynSel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnDynSel.Location = New System.Drawing.Point(15, 47)
        Me.btnDynSel.Name = "btnDynSel"
        Me.btnDynSel.Size = New System.Drawing.Size(136, 23)
        Me.btnDynSel.TabIndex = 18
        Me.btnDynSel.Text = "DynSel →"
        Me.btnDynSel.UseVisualStyleBackColor = True
        '
        'btnDynDel
        '
        Me.btnDynDel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnDynDel.Location = New System.Drawing.Point(15, 105)
        Me.btnDynDel.Name = "btnDynDel"
        Me.btnDynDel.Size = New System.Drawing.Size(136, 23)
        Me.btnDynDel.TabIndex = 20
        Me.btnDynDel.Text = "DynDel"
        Me.btnDynDel.UseVisualStyleBackColor = True
        '
        'btnDynUpd
        '
        Me.btnDynUpd.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnDynUpd.Location = New System.Drawing.Point(15, 76)
        Me.btnDynUpd.Name = "btnDynUpd"
        Me.btnDynUpd.Size = New System.Drawing.Size(136, 23)
        Me.btnDynUpd.TabIndex = 19
        Me.btnDynUpd.Text = "DynUpd"
        Me.btnDynUpd.UseVisualStyleBackColor = True
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.btnInsert)
        Me.groupBox1.Controls.Add(Me.btnSelect)
        Me.groupBox1.Controls.Add(Me.btnUpdate)
        Me.groupBox1.Controls.Add(Me.btnDelete)
        Me.groupBox1.Location = New System.Drawing.Point(19, 169)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(166, 138)
        Me.groupBox1.TabIndex = 38
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "静的（主キー必須）"
        '
        'btnInsert
        '
        Me.btnInsert.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnInsert.Location = New System.Drawing.Point(15, 18)
        Me.btnInsert.Name = "btnInsert"
        Me.btnInsert.Size = New System.Drawing.Size(136, 23)
        Me.btnInsert.TabIndex = 13
        Me.btnInsert.Text = "Insert"
        Me.btnInsert.UseVisualStyleBackColor = True
        '
        'btnSelect
        '
        Me.btnSelect.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnSelect.Location = New System.Drawing.Point(15, 47)
        Me.btnSelect.Name = "btnSelect"
        Me.btnSelect.Size = New System.Drawing.Size(136, 23)
        Me.btnSelect.TabIndex = 14
        Me.btnSelect.Text = "Select"
        Me.btnSelect.UseVisualStyleBackColor = True
        '
        'btnUpdate
        '
        Me.btnUpdate.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnUpdate.Location = New System.Drawing.Point(15, 76)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(136, 23)
        Me.btnUpdate.TabIndex = 15
        Me.btnUpdate.Text = "Update"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnDelete.Location = New System.Drawing.Point(15, 105)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(136, 23)
        Me.btnDelete.TabIndex = 16
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'label5
        '
        Me.label5.AutoSize = True
        Me.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label5.Location = New System.Drawing.Point(17, 106)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(25, 12)
        Me.label5.TabIndex = 37
        Me.label5.Text = "TS："
        '
        'txtTS
        '
        Me.txtTS.Location = New System.Drawing.Point(56, 103)
        Me.txtTS.Name = "txtTS"
        Me.txtTS.ReadOnly = True
        Me.txtTS.Size = New System.Drawing.Size(129, 19)
        Me.txtTS.TabIndex = 36
        '
        'label4
        '
        Me.label4.AutoSize = True
        Me.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label4.Location = New System.Drawing.Point(17, 81)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(33, 12)
        Me.label4.TabIndex = 35
        Me.label4.Text = "VAL："
        '
        'txtVAL
        '
        Me.txtVAL.Location = New System.Drawing.Point(56, 78)
        Me.txtVAL.Name = "txtVAL"
        Me.txtVAL.Size = New System.Drawing.Size(129, 19)
        Me.txtVAL.TabIndex = 34
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label3.Location = New System.Drawing.Point(17, 56)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(22, 12)
        Me.label3.TabIndex = 33
        Me.label3.Text = "ID："
        '
        'cmbTableType
        '
        Me.cmbTableType.FormattingEnabled = True
        Me.cmbTableType.Items.AddRange(New Object() {"TS列末端", "TS列中間", "TS列先頭"})
        Me.cmbTableType.Location = New System.Drawing.Point(295, 15)
        Me.cmbTableType.Name = "cmbTableType"
        Me.cmbTableType.Size = New System.Drawing.Size(121, 20)
        Me.cmbTableType.TabIndex = 32
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label2.Location = New System.Drawing.Point(228, 18)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(61, 12)
        Me.label2.TabIndex = 31
        Me.label2.Text = "テーブル種："
        '
        'cmbTSColType
        '
        Me.cmbTSColType.FormattingEnabled = True
        Me.cmbTSColType.Items.AddRange(New Object() {"RAND（float）列", "timestamp列"})
        Me.cmbTSColType.Location = New System.Drawing.Point(72, 15)
        Me.cmbTSColType.Name = "cmbTSColType"
        Me.cmbTSColType.Size = New System.Drawing.Size(121, 20)
        Me.cmbTSColType.TabIndex = 30
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.label1.Location = New System.Drawing.Point(17, 18)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(49, 12)
        Me.label1.TabIndex = 29
        Me.label1.Text = "TS列種："
        '
        'txtID
        '
        Me.txtID.Location = New System.Drawing.Point(56, 53)
        Me.txtID.Name = "txtID"
        Me.txtID.Size = New System.Drawing.Size(129, 19)
        Me.txtID.TabIndex = 28
        '
        'btnClear
        '
        Me.btnClear.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnClear.Location = New System.Drawing.Point(196, 418)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(463, 23)
        Me.btnClear.TabIndex = 27
        Me.btnClear.Text = "クリア"
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'btnGetAll
        '
        Me.btnGetAll.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnGetAll.Location = New System.Drawing.Point(196, 389)
        Me.btnGetAll.Name = "btnGetAll"
        Me.btnGetAll.Size = New System.Drawing.Size(463, 23)
        Me.btnGetAll.TabIndex = 26
        Me.btnGetAll.Text = "全件取得（DnySel）"
        Me.btnGetAll.UseVisualStyleBackColor = True
        '
        'dataGridView1
        '
        Me.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dataGridView1.Location = New System.Drawing.Point(196, 54)
        Me.dataGridView1.Name = "dataGridView1"
        Me.dataGridView1.RowTemplate.Height = 21
        Me.dataGridView1.Size = New System.Drawing.Size(463, 329)
        Me.dataGridView1.TabIndex = 25
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(676, 467)
        Me.Controls.Add(Me.btnClearTS)
        Me.Controls.Add(Me.groupBox2)
        Me.Controls.Add(Me.groupBox1)
        Me.Controls.Add(Me.label5)
        Me.Controls.Add(Me.txtTS)
        Me.Controls.Add(Me.label4)
        Me.Controls.Add(Me.txtVAL)
        Me.Controls.Add(Me.label3)
        Me.Controls.Add(Me.cmbTableType)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.cmbTSColType)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.txtID)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.btnGetAll)
        Me.Controls.Add(Me.dataGridView1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.groupBox2.ResumeLayout(False)
        Me.groupBox1.ResumeLayout(False)
        CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents btnClearTS As System.Windows.Forms.Button
    Private WithEvents groupBox2 As System.Windows.Forms.GroupBox
    Private WithEvents btnDynIns As System.Windows.Forms.Button
    Private WithEvents btnDynSel As System.Windows.Forms.Button
    Private WithEvents btnDynDel As System.Windows.Forms.Button
    Private WithEvents btnDynUpd As System.Windows.Forms.Button
    Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
    Private WithEvents btnInsert As System.Windows.Forms.Button
    Private WithEvents btnSelect As System.Windows.Forms.Button
    Private WithEvents btnUpdate As System.Windows.Forms.Button
    Private WithEvents btnDelete As System.Windows.Forms.Button
    Private WithEvents label5 As System.Windows.Forms.Label
    Private WithEvents txtTS As System.Windows.Forms.TextBox
    Private WithEvents label4 As System.Windows.Forms.Label
    Private WithEvents txtVAL As System.Windows.Forms.TextBox
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents cmbTableType As System.Windows.Forms.ComboBox
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents cmbTSColType As System.Windows.Forms.ComboBox
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents txtID As System.Windows.Forms.TextBox
    Private WithEvents btnClear As System.Windows.Forms.Button
    Private WithEvents btnGetAll As System.Windows.Forms.Button
    Private WithEvents dataGridView1 As System.Windows.Forms.DataGridView

End Class

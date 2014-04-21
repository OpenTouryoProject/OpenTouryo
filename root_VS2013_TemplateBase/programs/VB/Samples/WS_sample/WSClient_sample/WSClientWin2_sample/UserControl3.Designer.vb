Partial Class UserControl3
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

#Region "コンポーネント デザイナで生成されたコード"

    ''' <summary> 
    ''' デザイナ サポートに必要なメソッドです。このメソッドの内容を 
    ''' コード エディタで変更しないでください。
    ''' </summary>
    Private Sub InitializeComponent()
        Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(UserControl3))
        Me.label2 = New System.Windows.Forms.Label()
        Me.label1 = New System.Windows.Forms.Label()
        Me.rbnUCRadioButton2 = New System.Windows.Forms.RadioButton()
        Me.pbxUCPictureBox2 = New System.Windows.Forms.PictureBox()
        Me.lbxUCListBox2 = New System.Windows.Forms.ListBox()
        Me.cbbUCComboBox2 = New System.Windows.Forms.ComboBox()
        Me.cbxUCCheckBox2 = New System.Windows.Forms.CheckBox()
        Me.btnUCButton2 = New System.Windows.Forms.Button()
        Me.rbnUCRadioButton1 = New System.Windows.Forms.RadioButton()
        Me.pbxUCPictureBox1 = New System.Windows.Forms.PictureBox()
        Me.lbxUCListBox1 = New System.Windows.Forms.ListBox()
        Me.cbbUCComboBox1 = New System.Windows.Forms.ComboBox()
        Me.cbxUCCheckBox1 = New System.Windows.Forms.CheckBox()
        Me.btnUCButton1 = New System.Windows.Forms.Button()
        DirectCast(Me.pbxUCPictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        DirectCast(Me.pbxUCPictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        ' 
        ' label2
        ' 
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(115, 17)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(57, 12)
        Me.label2.TabIndex = 28
        Me.label2.Text = "メソッドなし"
        ' 
        ' label1
        ' 
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(17, 17)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(56, 12)
        Me.label1.TabIndex = 27
        Me.label1.Text = "メソッドあり"
        ' 
        ' rbnUCRadioButton2
        ' 
        Me.rbnUCRadioButton2.AutoSize = True
        Me.rbnUCRadioButton2.Location = New System.Drawing.Point(117, 135)
        Me.rbnUCRadioButton2.Name = "rbnUCRadioButton2"
        Me.rbnUCRadioButton2.Size = New System.Drawing.Size(88, 16)
        Me.rbnUCRadioButton2.TabIndex = 26
        Me.rbnUCRadioButton2.TabStop = True
        Me.rbnUCRadioButton2.Text = "radioButton2"
        Me.rbnUCRadioButton2.UseVisualStyleBackColor = True
        ' 
        ' pbxUCPictureBox2
        ' 
        Me.pbxUCPictureBox2.ErrorImage = Nothing
        Me.pbxUCPictureBox2.Image = DirectCast(resources.GetObject("pbxUCPictureBox2.Image"), System.Drawing.Image)
        Me.pbxUCPictureBox2.Location = New System.Drawing.Point(117, 84)
        Me.pbxUCPictureBox2.Name = "pbxUCPictureBox2"
        Me.pbxUCPictureBox2.Size = New System.Drawing.Size(75, 45)
        Me.pbxUCPictureBox2.TabIndex = 25
        Me.pbxUCPictureBox2.TabStop = False
        ' 
        ' lbxUCListBox2
        ' 
        Me.lbxUCListBox2.FormattingEnabled = True
        Me.lbxUCListBox2.ItemHeight = 12
        Me.lbxUCListBox2.Items.AddRange(New Object() {"あああ", "いいい", "ううう", "えええ"})
        Me.lbxUCListBox2.Location = New System.Drawing.Point(117, 205)
        Me.lbxUCListBox2.Name = "lbxUCListBox2"
        Me.lbxUCListBox2.Size = New System.Drawing.Size(80, 40)
        Me.lbxUCListBox2.TabIndex = 24
        ' 
        ' cbbUCComboBox2
        ' 
        Me.cbbUCComboBox2.FormattingEnabled = True
        Me.cbbUCComboBox2.Items.AddRange(New Object() {"あああ", "いいい", "ううう", "えええ"})
        Me.cbbUCComboBox2.Location = New System.Drawing.Point(117, 179)
        Me.cbbUCComboBox2.Name = "cbbUCComboBox2"
        Me.cbbUCComboBox2.Size = New System.Drawing.Size(80, 20)
        Me.cbbUCComboBox2.TabIndex = 23
        ' 
        ' cbxUCCheckBox2
        ' 
        Me.cbxUCCheckBox2.AutoSize = True
        Me.cbxUCCheckBox2.Location = New System.Drawing.Point(117, 157)
        Me.cbxUCCheckBox2.Name = "cbxUCCheckBox2"
        Me.cbxUCCheckBox2.Size = New System.Drawing.Size(80, 16)
        Me.cbxUCCheckBox2.TabIndex = 22
        Me.cbxUCCheckBox2.Text = "checkBox2"
        Me.cbxUCCheckBox2.UseVisualStyleBackColor = True
        ' 
        ' btnUCButton2
        ' 
        Me.btnUCButton2.Location = New System.Drawing.Point(117, 48)
        Me.btnUCButton2.Name = "btnUCButton2"
        Me.btnUCButton2.Size = New System.Drawing.Size(75, 23)
        Me.btnUCButton2.TabIndex = 21
        Me.btnUCButton2.Text = "button2"
        Me.btnUCButton2.UseVisualStyleBackColor = True
        ' 
        ' rbnUCRadioButton1
        ' 
        Me.rbnUCRadioButton1.AutoSize = True
        Me.rbnUCRadioButton1.Location = New System.Drawing.Point(19, 135)
        Me.rbnUCRadioButton1.Name = "rbnUCRadioButton1"
        Me.rbnUCRadioButton1.Size = New System.Drawing.Size(88, 16)
        Me.rbnUCRadioButton1.TabIndex = 20
        Me.rbnUCRadioButton1.TabStop = True
        Me.rbnUCRadioButton1.Text = "radioButton1"
        Me.rbnUCRadioButton1.UseVisualStyleBackColor = True
        ' 
        ' pbxUCPictureBox1
        ' 
        Me.pbxUCPictureBox1.ErrorImage = Nothing
        Me.pbxUCPictureBox1.Image = DirectCast(resources.GetObject("pbxUCPictureBox1.Image"), System.Drawing.Image)
        Me.pbxUCPictureBox1.Location = New System.Drawing.Point(19, 84)
        Me.pbxUCPictureBox1.Name = "pbxUCPictureBox1"
        Me.pbxUCPictureBox1.Size = New System.Drawing.Size(75, 45)
        Me.pbxUCPictureBox1.TabIndex = 19
        Me.pbxUCPictureBox1.TabStop = False
        ' 
        ' lbxUCListBox1
        ' 
        Me.lbxUCListBox1.FormattingEnabled = True
        Me.lbxUCListBox1.ItemHeight = 12
        Me.lbxUCListBox1.Items.AddRange(New Object() {"あああ", "いいい", "ううう", "えええ"})
        Me.lbxUCListBox1.Location = New System.Drawing.Point(19, 205)
        Me.lbxUCListBox1.Name = "lbxUCListBox1"
        Me.lbxUCListBox1.Size = New System.Drawing.Size(80, 40)
        Me.lbxUCListBox1.TabIndex = 18
        ' 
        ' cbbUCComboBox1
        ' 
        Me.cbbUCComboBox1.FormattingEnabled = True
        Me.cbbUCComboBox1.Items.AddRange(New Object() {"あああ", "いいい", "ううう", "えええ"})
        Me.cbbUCComboBox1.Location = New System.Drawing.Point(19, 179)
        Me.cbbUCComboBox1.Name = "cbbUCComboBox1"
        Me.cbbUCComboBox1.Size = New System.Drawing.Size(80, 20)
        Me.cbbUCComboBox1.TabIndex = 17
        ' 
        ' cbxUCCheckBox1
        ' 
        Me.cbxUCCheckBox1.AutoSize = True
        Me.cbxUCCheckBox1.Location = New System.Drawing.Point(19, 157)
        Me.cbxUCCheckBox1.Name = "cbxUCCheckBox1"
        Me.cbxUCCheckBox1.Size = New System.Drawing.Size(80, 16)
        Me.cbxUCCheckBox1.TabIndex = 16
        Me.cbxUCCheckBox1.Text = "checkBox1"
        Me.cbxUCCheckBox1.UseVisualStyleBackColor = True
        ' 
        ' btnUCButton1
        ' 
        Me.btnUCButton1.Location = New System.Drawing.Point(19, 48)
        Me.btnUCButton1.Name = "btnUCButton1"
        Me.btnUCButton1.Size = New System.Drawing.Size(75, 23)
        Me.btnUCButton1.TabIndex = 15
        Me.btnUCButton1.Text = "button1"
        Me.btnUCButton1.UseVisualStyleBackColor = True
        ' 
        ' UserControl3
        ' 
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 12.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.rbnUCRadioButton2)
        Me.Controls.Add(Me.pbxUCPictureBox2)
        Me.Controls.Add(Me.lbxUCListBox2)
        Me.Controls.Add(Me.cbbUCComboBox2)
        Me.Controls.Add(Me.cbxUCCheckBox2)
        Me.Controls.Add(Me.btnUCButton2)
        Me.Controls.Add(Me.rbnUCRadioButton1)
        Me.Controls.Add(Me.pbxUCPictureBox1)
        Me.Controls.Add(Me.lbxUCListBox1)
        Me.Controls.Add(Me.cbbUCComboBox1)
        Me.Controls.Add(Me.cbxUCCheckBox1)
        Me.Controls.Add(Me.btnUCButton1)
        Me.Name = "UserControl3"
        Me.Size = New System.Drawing.Size(212, 264)
        DirectCast(Me.pbxUCPictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        DirectCast(Me.pbxUCPictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private label2 As System.Windows.Forms.Label
    Private label1 As System.Windows.Forms.Label
    Private rbnUCRadioButton2 As System.Windows.Forms.RadioButton
    Private pbxUCPictureBox2 As System.Windows.Forms.PictureBox
    Private lbxUCListBox2 As System.Windows.Forms.ListBox
    Private cbbUCComboBox2 As System.Windows.Forms.ComboBox
    Private cbxUCCheckBox2 As System.Windows.Forms.CheckBox
    Private btnUCButton2 As System.Windows.Forms.Button
    Private rbnUCRadioButton1 As System.Windows.Forms.RadioButton
    Private pbxUCPictureBox1 As System.Windows.Forms.PictureBox
    Private lbxUCListBox1 As System.Windows.Forms.ListBox
    Private cbbUCComboBox1 As System.Windows.Forms.ComboBox
    Private cbxUCCheckBox1 As System.Windows.Forms.CheckBox
    Private btnUCButton1 As System.Windows.Forms.Button

End Class

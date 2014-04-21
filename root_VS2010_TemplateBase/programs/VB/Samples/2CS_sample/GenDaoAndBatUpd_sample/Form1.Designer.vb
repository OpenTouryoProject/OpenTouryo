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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.tabControl1 = New System.Windows.Forms.TabControl
        Me.tabPage1 = New System.Windows.Forms.TabPage
        Me.lblHomePage = New System.Windows.Forms.Label
        Me.txtHomePage = New System.Windows.Forms.TextBox
        Me.txtFax = New System.Windows.Forms.TextBox
        Me.txtPhone = New System.Windows.Forms.TextBox
        Me.txtCountry = New System.Windows.Forms.TextBox
        Me.txtPostalCode = New System.Windows.Forms.TextBox
        Me.txtRegion = New System.Windows.Forms.TextBox
        Me.txtCity = New System.Windows.Forms.TextBox
        Me.txtAddress = New System.Windows.Forms.TextBox
        Me.txtContactTitle = New System.Windows.Forms.TextBox
        Me.txtCompanyName = New System.Windows.Forms.TextBox
        Me.txtContactName = New System.Windows.Forms.TextBox
        Me.txtSupplierID = New System.Windows.Forms.TextBox
        Me.lblFax = New System.Windows.Forms.Label
        Me.lblPhone = New System.Windows.Forms.Label
        Me.lblCountry = New System.Windows.Forms.Label
        Me.lblPostalCode = New System.Windows.Forms.Label
        Me.lblRegion = New System.Windows.Forms.Label
        Me.lblCity = New System.Windows.Forms.Label
        Me.lblAddress = New System.Windows.Forms.Label
        Me.lblContactTitle = New System.Windows.Forms.Label
        Me.lblContactName = New System.Windows.Forms.Label
        Me.lblCompanyName = New System.Windows.Forms.Label
        Me.lblSupplierID = New System.Windows.Forms.Label
        Me.btnDelete1 = New System.Windows.Forms.Button
        Me.btnUpdate1 = New System.Windows.Forms.Button
        Me.btnInsert1 = New System.Windows.Forms.Button
        Me.btnSelect1 = New System.Windows.Forms.Button
        Me.btnClear1 = New System.Windows.Forms.Button
        Me.btnSelectAll1 = New System.Windows.Forms.Button
        Me.label1 = New System.Windows.Forms.Label
        Me.dataGridView1 = New System.Windows.Forms.DataGridView
        Me.tabPage2 = New System.Windows.Forms.TabPage
        Me.label4 = New System.Windows.Forms.Label
        Me.label3 = New System.Windows.Forms.Label
        Me.txtPicture_where = New System.Windows.Forms.TextBox
        Me.txtDescription_where = New System.Windows.Forms.TextBox
        Me.txtCategoryName_where = New System.Windows.Forms.TextBox
        Me.txtCategoryID_where = New System.Windows.Forms.TextBox
        Me.txtPicture = New System.Windows.Forms.TextBox
        Me.txtCategoryName = New System.Windows.Forms.TextBox
        Me.txtDescription = New System.Windows.Forms.TextBox
        Me.txtCategoryID = New System.Windows.Forms.TextBox
        Me.lblPicture_where = New System.Windows.Forms.Label
        Me.lblDescription_where = New System.Windows.Forms.Label
        Me.lblCategoryName_where = New System.Windows.Forms.Label
        Me.lblCategoryID_where = New System.Windows.Forms.Label
        Me.lblPicture = New System.Windows.Forms.Label
        Me.lblDescription = New System.Windows.Forms.Label
        Me.lblCategoryName = New System.Windows.Forms.Label
        Me.lblCategoryID = New System.Windows.Forms.Label
        Me.btnDelete2 = New System.Windows.Forms.Button
        Me.btnUpdate2 = New System.Windows.Forms.Button
        Me.btnInsert2 = New System.Windows.Forms.Button
        Me.btnSelect2 = New System.Windows.Forms.Button
        Me.btnClear2 = New System.Windows.Forms.Button
        Me.btnSelectAll2 = New System.Windows.Forms.Button
        Me.label2 = New System.Windows.Forms.Label
        Me.dataGridView2 = New System.Windows.Forms.DataGridView
        Me.tabPage3 = New System.Windows.Forms.TabPage
        Me.btnBatUpd = New System.Windows.Forms.Button
        Me.btnClear3 = New System.Windows.Forms.Button
        Me.label5 = New System.Windows.Forms.Label
        Me.btnSelectAll3 = New System.Windows.Forms.Button
        Me.dataGridView3 = New System.Windows.Forms.DataGridView
        Me.tabPage4 = New System.Windows.Forms.TabPage
        Me.pictureBox1 = New System.Windows.Forms.PictureBox
        Me.tabControl1.SuspendLayout()
        Me.tabPage1.SuspendLayout()
        CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabPage2.SuspendLayout()
        CType(Me.dataGridView2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabPage3.SuspendLayout()
        CType(Me.dataGridView3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabPage4.SuspendLayout()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tabControl1
        '
        Me.tabControl1.Controls.Add(Me.tabPage1)
        Me.tabControl1.Controls.Add(Me.tabPage2)
        Me.tabControl1.Controls.Add(Me.tabPage3)
        Me.tabControl1.Controls.Add(Me.tabPage4)
        Me.tabControl1.Location = New System.Drawing.Point(12, 9)
        Me.tabControl1.Name = "tabControl1"
        Me.tabControl1.SelectedIndex = 0
        Me.tabControl1.Size = New System.Drawing.Size(701, 583)
        Me.tabControl1.TabIndex = 1
        '
        'tabPage1
        '
        Me.tabPage1.Controls.Add(Me.lblHomePage)
        Me.tabPage1.Controls.Add(Me.txtHomePage)
        Me.tabPage1.Controls.Add(Me.txtFax)
        Me.tabPage1.Controls.Add(Me.txtPhone)
        Me.tabPage1.Controls.Add(Me.txtCountry)
        Me.tabPage1.Controls.Add(Me.txtPostalCode)
        Me.tabPage1.Controls.Add(Me.txtRegion)
        Me.tabPage1.Controls.Add(Me.txtCity)
        Me.tabPage1.Controls.Add(Me.txtAddress)
        Me.tabPage1.Controls.Add(Me.txtContactTitle)
        Me.tabPage1.Controls.Add(Me.txtCompanyName)
        Me.tabPage1.Controls.Add(Me.txtContactName)
        Me.tabPage1.Controls.Add(Me.txtSupplierID)
        Me.tabPage1.Controls.Add(Me.lblFax)
        Me.tabPage1.Controls.Add(Me.lblPhone)
        Me.tabPage1.Controls.Add(Me.lblCountry)
        Me.tabPage1.Controls.Add(Me.lblPostalCode)
        Me.tabPage1.Controls.Add(Me.lblRegion)
        Me.tabPage1.Controls.Add(Me.lblCity)
        Me.tabPage1.Controls.Add(Me.lblAddress)
        Me.tabPage1.Controls.Add(Me.lblContactTitle)
        Me.tabPage1.Controls.Add(Me.lblContactName)
        Me.tabPage1.Controls.Add(Me.lblCompanyName)
        Me.tabPage1.Controls.Add(Me.lblSupplierID)
        Me.tabPage1.Controls.Add(Me.btnDelete1)
        Me.tabPage1.Controls.Add(Me.btnUpdate1)
        Me.tabPage1.Controls.Add(Me.btnInsert1)
        Me.tabPage1.Controls.Add(Me.btnSelect1)
        Me.tabPage1.Controls.Add(Me.btnClear1)
        Me.tabPage1.Controls.Add(Me.btnSelectAll1)
        Me.tabPage1.Controls.Add(Me.label1)
        Me.tabPage1.Controls.Add(Me.dataGridView1)
        Me.tabPage1.Location = New System.Drawing.Point(4, 21)
        Me.tabPage1.Name = "tabPage1"
        Me.tabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.tabPage1.Size = New System.Drawing.Size(693, 558)
        Me.tabPage1.TabIndex = 0
        Me.tabPage1.Text = "静的SQLのCRUD"
        Me.tabPage1.UseVisualStyleBackColor = True
        '
        'lblHomePage
        '
        Me.lblHomePage.AutoSize = True
        Me.lblHomePage.Location = New System.Drawing.Point(36, 310)
        Me.lblHomePage.Name = "lblHomePage"
        Me.lblHomePage.Size = New System.Drawing.Size(59, 12)
        Me.lblHomePage.TabIndex = 33
        Me.lblHomePage.Text = "HomePage"
        '
        'txtHomePage
        '
        Me.txtHomePage.Location = New System.Drawing.Point(101, 307)
        Me.txtHomePage.Name = "txtHomePage"
        Me.txtHomePage.Size = New System.Drawing.Size(122, 19)
        Me.txtHomePage.TabIndex = 32
        '
        'txtFax
        '
        Me.txtFax.Location = New System.Drawing.Point(101, 282)
        Me.txtFax.Name = "txtFax"
        Me.txtFax.Size = New System.Drawing.Size(122, 19)
        Me.txtFax.TabIndex = 30
        '
        'txtPhone
        '
        Me.txtPhone.Location = New System.Drawing.Point(101, 257)
        Me.txtPhone.Name = "txtPhone"
        Me.txtPhone.Size = New System.Drawing.Size(122, 19)
        Me.txtPhone.TabIndex = 28
        '
        'txtCountry
        '
        Me.txtCountry.Location = New System.Drawing.Point(101, 232)
        Me.txtCountry.Name = "txtCountry"
        Me.txtCountry.Size = New System.Drawing.Size(122, 19)
        Me.txtCountry.TabIndex = 26
        '
        'txtPostalCode
        '
        Me.txtPostalCode.Location = New System.Drawing.Point(101, 207)
        Me.txtPostalCode.Name = "txtPostalCode"
        Me.txtPostalCode.Size = New System.Drawing.Size(122, 19)
        Me.txtPostalCode.TabIndex = 24
        '
        'txtRegion
        '
        Me.txtRegion.Location = New System.Drawing.Point(101, 182)
        Me.txtRegion.Name = "txtRegion"
        Me.txtRegion.Size = New System.Drawing.Size(122, 19)
        Me.txtRegion.TabIndex = 22
        '
        'txtCity
        '
        Me.txtCity.Location = New System.Drawing.Point(101, 157)
        Me.txtCity.Name = "txtCity"
        Me.txtCity.Size = New System.Drawing.Size(122, 19)
        Me.txtCity.TabIndex = 20
        '
        'txtAddress
        '
        Me.txtAddress.Location = New System.Drawing.Point(101, 132)
        Me.txtAddress.Name = "txtAddress"
        Me.txtAddress.Size = New System.Drawing.Size(122, 19)
        Me.txtAddress.TabIndex = 18
        '
        'txtContactTitle
        '
        Me.txtContactTitle.Location = New System.Drawing.Point(101, 107)
        Me.txtContactTitle.Name = "txtContactTitle"
        Me.txtContactTitle.Size = New System.Drawing.Size(122, 19)
        Me.txtContactTitle.TabIndex = 16
        '
        'txtCompanyName
        '
        Me.txtCompanyName.Location = New System.Drawing.Point(101, 57)
        Me.txtCompanyName.Name = "txtCompanyName"
        Me.txtCompanyName.Size = New System.Drawing.Size(122, 19)
        Me.txtCompanyName.TabIndex = 14
        '
        'txtContactName
        '
        Me.txtContactName.Location = New System.Drawing.Point(101, 82)
        Me.txtContactName.Name = "txtContactName"
        Me.txtContactName.Size = New System.Drawing.Size(122, 19)
        Me.txtContactName.TabIndex = 9
        '
        'txtSupplierID
        '
        Me.txtSupplierID.Location = New System.Drawing.Point(101, 32)
        Me.txtSupplierID.Name = "txtSupplierID"
        Me.txtSupplierID.Size = New System.Drawing.Size(122, 19)
        Me.txtSupplierID.TabIndex = 8
        '
        'lblFax
        '
        Me.lblFax.AutoSize = True
        Me.lblFax.Location = New System.Drawing.Point(71, 285)
        Me.lblFax.Name = "lblFax"
        Me.lblFax.Size = New System.Drawing.Size(24, 12)
        Me.lblFax.TabIndex = 31
        Me.lblFax.Text = "Fax"
        '
        'lblPhone
        '
        Me.lblPhone.AutoSize = True
        Me.lblPhone.Location = New System.Drawing.Point(59, 260)
        Me.lblPhone.Name = "lblPhone"
        Me.lblPhone.Size = New System.Drawing.Size(36, 12)
        Me.lblPhone.TabIndex = 29
        Me.lblPhone.Text = "Phone"
        '
        'lblCountry
        '
        Me.lblCountry.AutoSize = True
        Me.lblCountry.Location = New System.Drawing.Point(50, 235)
        Me.lblCountry.Name = "lblCountry"
        Me.lblCountry.Size = New System.Drawing.Size(45, 12)
        Me.lblCountry.TabIndex = 27
        Me.lblCountry.Text = "Country"
        '
        'lblPostalCode
        '
        Me.lblPostalCode.AutoSize = True
        Me.lblPostalCode.Location = New System.Drawing.Point(32, 210)
        Me.lblPostalCode.Name = "lblPostalCode"
        Me.lblPostalCode.Size = New System.Drawing.Size(63, 12)
        Me.lblPostalCode.TabIndex = 25
        Me.lblPostalCode.Text = "PostalCode"
        '
        'lblRegion
        '
        Me.lblRegion.AutoSize = True
        Me.lblRegion.Location = New System.Drawing.Point(55, 185)
        Me.lblRegion.Name = "lblRegion"
        Me.lblRegion.Size = New System.Drawing.Size(40, 12)
        Me.lblRegion.TabIndex = 23
        Me.lblRegion.Text = "Region"
        '
        'lblCity
        '
        Me.lblCity.AutoSize = True
        Me.lblCity.Location = New System.Drawing.Point(69, 160)
        Me.lblCity.Name = "lblCity"
        Me.lblCity.Size = New System.Drawing.Size(26, 12)
        Me.lblCity.TabIndex = 21
        Me.lblCity.Text = "City"
        '
        'lblAddress
        '
        Me.lblAddress.AutoSize = True
        Me.lblAddress.Location = New System.Drawing.Point(48, 135)
        Me.lblAddress.Name = "lblAddress"
        Me.lblAddress.Size = New System.Drawing.Size(47, 12)
        Me.lblAddress.TabIndex = 19
        Me.lblAddress.Text = "Address"
        '
        'lblContactTitle
        '
        Me.lblContactTitle.AutoSize = True
        Me.lblContactTitle.Location = New System.Drawing.Point(27, 110)
        Me.lblContactTitle.Name = "lblContactTitle"
        Me.lblContactTitle.Size = New System.Drawing.Size(68, 12)
        Me.lblContactTitle.TabIndex = 17
        Me.lblContactTitle.Text = "ContactTitle"
        '
        'lblContactName
        '
        Me.lblContactName.AutoSize = True
        Me.lblContactName.Location = New System.Drawing.Point(21, 85)
        Me.lblContactName.Name = "lblContactName"
        Me.lblContactName.Size = New System.Drawing.Size(74, 12)
        Me.lblContactName.TabIndex = 15
        Me.lblContactName.Text = "ContactName"
        '
        'lblCompanyName
        '
        Me.lblCompanyName.AutoSize = True
        Me.lblCompanyName.Location = New System.Drawing.Point(14, 60)
        Me.lblCompanyName.Name = "lblCompanyName"
        Me.lblCompanyName.Size = New System.Drawing.Size(81, 12)
        Me.lblCompanyName.TabIndex = 13
        Me.lblCompanyName.Text = "CompanyName"
        '
        'lblSupplierID
        '
        Me.lblSupplierID.AutoSize = True
        Me.lblSupplierID.Location = New System.Drawing.Point(38, 35)
        Me.lblSupplierID.Name = "lblSupplierID"
        Me.lblSupplierID.Size = New System.Drawing.Size(57, 12)
        Me.lblSupplierID.TabIndex = 12
        Me.lblSupplierID.Text = "SupplierID"
        '
        'btnDelete1
        '
        Me.btnDelete1.Location = New System.Drawing.Point(6, 530)
        Me.btnDelete1.Name = "btnDelete1"
        Me.btnDelete1.Size = New System.Drawing.Size(217, 23)
        Me.btnDelete1.TabIndex = 7
        Me.btnDelete1.Text = "デリート"
        Me.btnDelete1.UseVisualStyleBackColor = True
        '
        'btnUpdate1
        '
        Me.btnUpdate1.Location = New System.Drawing.Point(6, 501)
        Me.btnUpdate1.Name = "btnUpdate1"
        Me.btnUpdate1.Size = New System.Drawing.Size(217, 23)
        Me.btnUpdate1.TabIndex = 6
        Me.btnUpdate1.Text = "アップデート"
        Me.btnUpdate1.UseVisualStyleBackColor = True
        '
        'btnInsert1
        '
        Me.btnInsert1.Location = New System.Drawing.Point(6, 443)
        Me.btnInsert1.Name = "btnInsert1"
        Me.btnInsert1.Size = New System.Drawing.Size(217, 23)
        Me.btnInsert1.TabIndex = 5
        Me.btnInsert1.Text = "インサート"
        Me.btnInsert1.UseVisualStyleBackColor = True
        '
        'btnSelect1
        '
        Me.btnSelect1.Location = New System.Drawing.Point(6, 472)
        Me.btnSelect1.Name = "btnSelect1"
        Me.btnSelect1.Size = New System.Drawing.Size(217, 23)
        Me.btnSelect1.TabIndex = 4
        Me.btnSelect1.Text = "セレクト"
        Me.btnSelect1.UseVisualStyleBackColor = True
        '
        'btnClear1
        '
        Me.btnClear1.Location = New System.Drawing.Point(229, 530)
        Me.btnClear1.Name = "btnClear1"
        Me.btnClear1.Size = New System.Drawing.Size(458, 23)
        Me.btnClear1.TabIndex = 3
        Me.btnClear1.Text = "クリア"
        Me.btnClear1.UseVisualStyleBackColor = True
        '
        'btnSelectAll1
        '
        Me.btnSelectAll1.Location = New System.Drawing.Point(229, 501)
        Me.btnSelectAll1.Name = "btnSelectAll1"
        Me.btnSelectAll1.Size = New System.Drawing.Size(458, 23)
        Me.btnSelectAll1.TabIndex = 2
        Me.btnSelectAll1.Text = "全件取得"
        Me.btnSelectAll1.UseVisualStyleBackColor = True
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(229, 19)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(90, 12)
        Me.label1.TabIndex = 1
        Me.label1.Text = "Suppliersテーブル"
        '
        'dataGridView1
        '
        Me.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dataGridView1.Location = New System.Drawing.Point(229, 34)
        Me.dataGridView1.Name = "dataGridView1"
        Me.dataGridView1.RowTemplate.Height = 21
        Me.dataGridView1.Size = New System.Drawing.Size(458, 461)
        Me.dataGridView1.TabIndex = 0
        '
        'tabPage2
        '
        Me.tabPage2.Controls.Add(Me.label4)
        Me.tabPage2.Controls.Add(Me.label3)
        Me.tabPage2.Controls.Add(Me.txtPicture_where)
        Me.tabPage2.Controls.Add(Me.txtDescription_where)
        Me.tabPage2.Controls.Add(Me.txtCategoryName_where)
        Me.tabPage2.Controls.Add(Me.txtCategoryID_where)
        Me.tabPage2.Controls.Add(Me.txtPicture)
        Me.tabPage2.Controls.Add(Me.txtCategoryName)
        Me.tabPage2.Controls.Add(Me.txtDescription)
        Me.tabPage2.Controls.Add(Me.txtCategoryID)
        Me.tabPage2.Controls.Add(Me.lblPicture_where)
        Me.tabPage2.Controls.Add(Me.lblDescription_where)
        Me.tabPage2.Controls.Add(Me.lblCategoryName_where)
        Me.tabPage2.Controls.Add(Me.lblCategoryID_where)
        Me.tabPage2.Controls.Add(Me.lblPicture)
        Me.tabPage2.Controls.Add(Me.lblDescription)
        Me.tabPage2.Controls.Add(Me.lblCategoryName)
        Me.tabPage2.Controls.Add(Me.lblCategoryID)
        Me.tabPage2.Controls.Add(Me.btnDelete2)
        Me.tabPage2.Controls.Add(Me.btnUpdate2)
        Me.tabPage2.Controls.Add(Me.btnInsert2)
        Me.tabPage2.Controls.Add(Me.btnSelect2)
        Me.tabPage2.Controls.Add(Me.btnClear2)
        Me.tabPage2.Controls.Add(Me.btnSelectAll2)
        Me.tabPage2.Controls.Add(Me.label2)
        Me.tabPage2.Controls.Add(Me.dataGridView2)
        Me.tabPage2.Location = New System.Drawing.Point(4, 21)
        Me.tabPage2.Name = "tabPage2"
        Me.tabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.tabPage2.Size = New System.Drawing.Size(693, 558)
        Me.tabPage2.TabIndex = 1
        Me.tabPage2.Text = "動的SQLのCRUD"
        Me.tabPage2.UseVisualStyleBackColor = True
        '
        'label4
        '
        Me.label4.AutoSize = True
        Me.label4.Location = New System.Drawing.Point(144, 192)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(53, 12)
        Me.label4.TabIndex = 65
        Me.label4.Text = "検索条件"
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(144, 42)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(29, 12)
        Me.label3.TabIndex = 64
        Me.label3.Text = "実値"
        '
        'txtPicture_where
        '
        Me.txtPicture_where.Enabled = False
        Me.txtPicture_where.Location = New System.Drawing.Point(146, 282)
        Me.txtPicture_where.Name = "txtPicture_where"
        Me.txtPicture_where.Size = New System.Drawing.Size(122, 19)
        Me.txtPicture_where.TabIndex = 62
        '
        'txtDescription_where
        '
        Me.txtDescription_where.Enabled = False
        Me.txtDescription_where.Location = New System.Drawing.Point(146, 257)
        Me.txtDescription_where.Name = "txtDescription_where"
        Me.txtDescription_where.Size = New System.Drawing.Size(122, 19)
        Me.txtDescription_where.TabIndex = 60
        '
        'txtCategoryName_where
        '
        Me.txtCategoryName_where.Location = New System.Drawing.Point(146, 232)
        Me.txtCategoryName_where.Name = "txtCategoryName_where"
        Me.txtCategoryName_where.Size = New System.Drawing.Size(122, 19)
        Me.txtCategoryName_where.TabIndex = 58
        '
        'txtCategoryID_where
        '
        Me.txtCategoryID_where.Location = New System.Drawing.Point(146, 207)
        Me.txtCategoryID_where.Name = "txtCategoryID_where"
        Me.txtCategoryID_where.Size = New System.Drawing.Size(122, 19)
        Me.txtCategoryID_where.TabIndex = 56
        '
        'txtPicture
        '
        Me.txtPicture.Enabled = False
        Me.txtPicture.Location = New System.Drawing.Point(146, 132)
        Me.txtPicture.Name = "txtPicture"
        Me.txtPicture.Size = New System.Drawing.Size(122, 19)
        Me.txtPicture.TabIndex = 48
        '
        'txtCategoryName
        '
        Me.txtCategoryName.Location = New System.Drawing.Point(146, 82)
        Me.txtCategoryName.Name = "txtCategoryName"
        Me.txtCategoryName.Size = New System.Drawing.Size(122, 19)
        Me.txtCategoryName.TabIndex = 46
        '
        'txtDescription
        '
        Me.txtDescription.Location = New System.Drawing.Point(146, 107)
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.Size = New System.Drawing.Size(122, 19)
        Me.txtDescription.TabIndex = 43
        '
        'txtCategoryID
        '
        Me.txtCategoryID.Enabled = False
        Me.txtCategoryID.Location = New System.Drawing.Point(146, 57)
        Me.txtCategoryID.Name = "txtCategoryID"
        Me.txtCategoryID.Size = New System.Drawing.Size(122, 19)
        Me.txtCategoryID.TabIndex = 42
        '
        'lblPicture_where
        '
        Me.lblPicture_where.AutoSize = True
        Me.lblPicture_where.Location = New System.Drawing.Point(65, 285)
        Me.lblPicture_where.Name = "lblPicture_where"
        Me.lblPicture_where.Size = New System.Drawing.Size(75, 12)
        Me.lblPicture_where.TabIndex = 63
        Me.lblPicture_where.Text = "Picture_where"
        '
        'lblDescription_where
        '
        Me.lblDescription_where.AutoSize = True
        Me.lblDescription_where.Location = New System.Drawing.Point(43, 260)
        Me.lblDescription_where.Name = "lblDescription_where"
        Me.lblDescription_where.Size = New System.Drawing.Size(97, 12)
        Me.lblDescription_where.TabIndex = 61
        Me.lblDescription_where.Text = "Description_where"
        '
        'lblCategoryName_where
        '
        Me.lblCategoryName_where.AutoSize = True
        Me.lblCategoryName_where.Location = New System.Drawing.Point(26, 235)
        Me.lblCategoryName_where.Name = "lblCategoryName_where"
        Me.lblCategoryName_where.Size = New System.Drawing.Size(114, 12)
        Me.lblCategoryName_where.TabIndex = 59
        Me.lblCategoryName_where.Text = "CategoryName_where"
        '
        'lblCategoryID_where
        '
        Me.lblCategoryID_where.AutoSize = True
        Me.lblCategoryID_where.Location = New System.Drawing.Point(44, 210)
        Me.lblCategoryID_where.Name = "lblCategoryID_where"
        Me.lblCategoryID_where.Size = New System.Drawing.Size(96, 12)
        Me.lblCategoryID_where.TabIndex = 57
        Me.lblCategoryID_where.Text = "CategoryID_where"
        '
        'lblPicture
        '
        Me.lblPicture.AutoSize = True
        Me.lblPicture.Location = New System.Drawing.Point(99, 135)
        Me.lblPicture.Name = "lblPicture"
        Me.lblPicture.Size = New System.Drawing.Size(41, 12)
        Me.lblPicture.TabIndex = 49
        Me.lblPicture.Text = "Picture"
        '
        'lblDescription
        '
        Me.lblDescription.AutoSize = True
        Me.lblDescription.Location = New System.Drawing.Point(77, 110)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(63, 12)
        Me.lblDescription.TabIndex = 47
        Me.lblDescription.Text = "Description"
        '
        'lblCategoryName
        '
        Me.lblCategoryName.AutoSize = True
        Me.lblCategoryName.Location = New System.Drawing.Point(60, 85)
        Me.lblCategoryName.Name = "lblCategoryName"
        Me.lblCategoryName.Size = New System.Drawing.Size(80, 12)
        Me.lblCategoryName.TabIndex = 45
        Me.lblCategoryName.Text = "CategoryName"
        '
        'lblCategoryID
        '
        Me.lblCategoryID.AutoSize = True
        Me.lblCategoryID.Location = New System.Drawing.Point(78, 60)
        Me.lblCategoryID.Name = "lblCategoryID"
        Me.lblCategoryID.Size = New System.Drawing.Size(62, 12)
        Me.lblCategoryID.TabIndex = 44
        Me.lblCategoryID.Text = "CategoryID"
        '
        'btnDelete2
        '
        Me.btnDelete2.Location = New System.Drawing.Point(6, 530)
        Me.btnDelete2.Name = "btnDelete2"
        Me.btnDelete2.Size = New System.Drawing.Size(262, 23)
        Me.btnDelete2.TabIndex = 41
        Me.btnDelete2.Text = "デリート"
        Me.btnDelete2.UseVisualStyleBackColor = True
        '
        'btnUpdate2
        '
        Me.btnUpdate2.Location = New System.Drawing.Point(6, 501)
        Me.btnUpdate2.Name = "btnUpdate2"
        Me.btnUpdate2.Size = New System.Drawing.Size(262, 23)
        Me.btnUpdate2.TabIndex = 40
        Me.btnUpdate2.Text = "アップデート"
        Me.btnUpdate2.UseVisualStyleBackColor = True
        '
        'btnInsert2
        '
        Me.btnInsert2.Location = New System.Drawing.Point(6, 443)
        Me.btnInsert2.Name = "btnInsert2"
        Me.btnInsert2.Size = New System.Drawing.Size(262, 23)
        Me.btnInsert2.TabIndex = 39
        Me.btnInsert2.Text = "インサート"
        Me.btnInsert2.UseVisualStyleBackColor = True
        '
        'btnSelect2
        '
        Me.btnSelect2.Location = New System.Drawing.Point(6, 472)
        Me.btnSelect2.Name = "btnSelect2"
        Me.btnSelect2.Size = New System.Drawing.Size(262, 23)
        Me.btnSelect2.TabIndex = 38
        Me.btnSelect2.Text = "セレクト"
        Me.btnSelect2.UseVisualStyleBackColor = True
        '
        'btnClear2
        '
        Me.btnClear2.Location = New System.Drawing.Point(274, 530)
        Me.btnClear2.Name = "btnClear2"
        Me.btnClear2.Size = New System.Drawing.Size(413, 23)
        Me.btnClear2.TabIndex = 37
        Me.btnClear2.Text = "クリア"
        Me.btnClear2.UseVisualStyleBackColor = True
        '
        'btnSelectAll2
        '
        Me.btnSelectAll2.Location = New System.Drawing.Point(274, 501)
        Me.btnSelectAll2.Name = "btnSelectAll2"
        Me.btnSelectAll2.Size = New System.Drawing.Size(413, 23)
        Me.btnSelectAll2.TabIndex = 36
        Me.btnSelectAll2.Text = "全件取得"
        Me.btnSelectAll2.UseVisualStyleBackColor = True
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(272, 19)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(89, 12)
        Me.label2.TabIndex = 35
        Me.label2.Text = "Categoryテーブル"
        '
        'dataGridView2
        '
        Me.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dataGridView2.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dataGridView2.Location = New System.Drawing.Point(274, 34)
        Me.dataGridView2.Name = "dataGridView2"
        Me.dataGridView2.RowTemplate.Height = 21
        Me.dataGridView2.Size = New System.Drawing.Size(413, 461)
        Me.dataGridView2.TabIndex = 34
        '
        'tabPage3
        '
        Me.tabPage3.Controls.Add(Me.btnBatUpd)
        Me.tabPage3.Controls.Add(Me.btnClear3)
        Me.tabPage3.Controls.Add(Me.label5)
        Me.tabPage3.Controls.Add(Me.btnSelectAll3)
        Me.tabPage3.Controls.Add(Me.dataGridView3)
        Me.tabPage3.Location = New System.Drawing.Point(4, 21)
        Me.tabPage3.Name = "tabPage3"
        Me.tabPage3.Size = New System.Drawing.Size(693, 558)
        Me.tabPage3.TabIndex = 2
        Me.tabPage3.Text = "バッチ更新"
        Me.tabPage3.UseVisualStyleBackColor = True
        '
        'btnBatUpd
        '
        Me.btnBatUpd.Location = New System.Drawing.Point(12, 501)
        Me.btnBatUpd.Name = "btnBatUpd"
        Me.btnBatUpd.Size = New System.Drawing.Size(666, 23)
        Me.btnBatUpd.TabIndex = 38
        Me.btnBatUpd.Text = "バッチ更新"
        Me.btnBatUpd.UseVisualStyleBackColor = True
        '
        'btnClear3
        '
        Me.btnClear3.Location = New System.Drawing.Point(12, 530)
        Me.btnClear3.Name = "btnClear3"
        Me.btnClear3.Size = New System.Drawing.Size(666, 23)
        Me.btnClear3.TabIndex = 37
        Me.btnClear3.Text = "クリア"
        Me.btnClear3.UseVisualStyleBackColor = True
        '
        'label5
        '
        Me.label5.AutoSize = True
        Me.label5.Location = New System.Drawing.Point(10, 19)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(88, 12)
        Me.label5.TabIndex = 36
        Me.label5.Text = "Productsテーブル"
        '
        'btnSelectAll3
        '
        Me.btnSelectAll3.Location = New System.Drawing.Point(12, 472)
        Me.btnSelectAll3.Name = "btnSelectAll3"
        Me.btnSelectAll3.Size = New System.Drawing.Size(666, 23)
        Me.btnSelectAll3.TabIndex = 1
        Me.btnSelectAll3.Text = "全件取得"
        Me.btnSelectAll3.UseVisualStyleBackColor = True
        '
        'dataGridView3
        '
        Me.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dataGridView3.Location = New System.Drawing.Point(12, 34)
        Me.dataGridView3.Name = "dataGridView3"
        Me.dataGridView3.RowTemplate.Height = 21
        Me.dataGridView3.Size = New System.Drawing.Size(666, 432)
        Me.dataGridView3.TabIndex = 0
        '
        'tabPage4
        '
        Me.tabPage4.Controls.Add(Me.pictureBox1)
        Me.tabPage4.Location = New System.Drawing.Point(4, 21)
        Me.tabPage4.Name = "tabPage4"
        Me.tabPage4.Size = New System.Drawing.Size(693, 558)
        Me.tabPage4.TabIndex = 3
        Me.tabPage4.Text = "ダイアグラム"
        Me.tabPage4.UseVisualStyleBackColor = True
        '
        'pictureBox1
        '
        Me.pictureBox1.Image = CType(resources.GetObject("pictureBox1.Image"), System.Drawing.Image)
        Me.pictureBox1.Location = New System.Drawing.Point(3, 3)
        Me.pictureBox1.Name = "pictureBox1"
        Me.pictureBox1.Size = New System.Drawing.Size(708, 552)
        Me.pictureBox1.TabIndex = 0
        Me.pictureBox1.TabStop = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(725, 600)
        Me.Controls.Add(Me.tabControl1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.tabControl1.ResumeLayout(False)
        Me.tabPage1.ResumeLayout(False)
        Me.tabPage1.PerformLayout()
        CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabPage2.ResumeLayout(False)
        Me.tabPage2.PerformLayout()
        CType(Me.dataGridView2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabPage3.ResumeLayout(False)
        Me.tabPage3.PerformLayout()
        CType(Me.dataGridView3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabPage4.ResumeLayout(False)
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents tabControl1 As System.Windows.Forms.TabControl
    Private WithEvents tabPage1 As System.Windows.Forms.TabPage
    Private WithEvents lblHomePage As System.Windows.Forms.Label
    Private WithEvents txtHomePage As System.Windows.Forms.TextBox
    Private WithEvents txtFax As System.Windows.Forms.TextBox
    Private WithEvents txtPhone As System.Windows.Forms.TextBox
    Private WithEvents txtCountry As System.Windows.Forms.TextBox
    Private WithEvents txtPostalCode As System.Windows.Forms.TextBox
    Private WithEvents txtRegion As System.Windows.Forms.TextBox
    Private WithEvents txtCity As System.Windows.Forms.TextBox
    Private WithEvents txtAddress As System.Windows.Forms.TextBox
    Private WithEvents txtContactTitle As System.Windows.Forms.TextBox
    Private WithEvents txtCompanyName As System.Windows.Forms.TextBox
    Private WithEvents txtContactName As System.Windows.Forms.TextBox
    Private WithEvents txtSupplierID As System.Windows.Forms.TextBox
    Private WithEvents lblFax As System.Windows.Forms.Label
    Private WithEvents lblPhone As System.Windows.Forms.Label
    Private WithEvents lblCountry As System.Windows.Forms.Label
    Private WithEvents lblPostalCode As System.Windows.Forms.Label
    Private WithEvents lblRegion As System.Windows.Forms.Label
    Private WithEvents lblCity As System.Windows.Forms.Label
    Private WithEvents lblAddress As System.Windows.Forms.Label
    Private WithEvents lblContactTitle As System.Windows.Forms.Label
    Private WithEvents lblContactName As System.Windows.Forms.Label
    Private WithEvents lblCompanyName As System.Windows.Forms.Label
    Private WithEvents lblSupplierID As System.Windows.Forms.Label
    Private WithEvents btnDelete1 As System.Windows.Forms.Button
    Private WithEvents btnUpdate1 As System.Windows.Forms.Button
    Private WithEvents btnInsert1 As System.Windows.Forms.Button
    Private WithEvents btnSelect1 As System.Windows.Forms.Button
    Private WithEvents btnClear1 As System.Windows.Forms.Button
    Private WithEvents btnSelectAll1 As System.Windows.Forms.Button
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents dataGridView1 As System.Windows.Forms.DataGridView
    Private WithEvents tabPage2 As System.Windows.Forms.TabPage
    Private WithEvents label4 As System.Windows.Forms.Label
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents txtPicture_where As System.Windows.Forms.TextBox
    Private WithEvents txtDescription_where As System.Windows.Forms.TextBox
    Private WithEvents txtCategoryName_where As System.Windows.Forms.TextBox
    Private WithEvents txtCategoryID_where As System.Windows.Forms.TextBox
    Private WithEvents txtPicture As System.Windows.Forms.TextBox
    Private WithEvents txtCategoryName As System.Windows.Forms.TextBox
    Private WithEvents txtDescription As System.Windows.Forms.TextBox
    Private WithEvents txtCategoryID As System.Windows.Forms.TextBox
    Private WithEvents lblPicture_where As System.Windows.Forms.Label
    Private WithEvents lblDescription_where As System.Windows.Forms.Label
    Private WithEvents lblCategoryName_where As System.Windows.Forms.Label
    Private WithEvents lblCategoryID_where As System.Windows.Forms.Label
    Private WithEvents lblPicture As System.Windows.Forms.Label
    Private WithEvents lblDescription As System.Windows.Forms.Label
    Private WithEvents lblCategoryName As System.Windows.Forms.Label
    Private WithEvents lblCategoryID As System.Windows.Forms.Label
    Private WithEvents btnDelete2 As System.Windows.Forms.Button
    Private WithEvents btnUpdate2 As System.Windows.Forms.Button
    Private WithEvents btnInsert2 As System.Windows.Forms.Button
    Private WithEvents btnSelect2 As System.Windows.Forms.Button
    Private WithEvents btnClear2 As System.Windows.Forms.Button
    Private WithEvents btnSelectAll2 As System.Windows.Forms.Button
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents dataGridView2 As System.Windows.Forms.DataGridView
    Private WithEvents tabPage3 As System.Windows.Forms.TabPage
    Private WithEvents btnBatUpd As System.Windows.Forms.Button
    Private WithEvents btnClear3 As System.Windows.Forms.Button
    Private WithEvents label5 As System.Windows.Forms.Label
    Private WithEvents btnSelectAll3 As System.Windows.Forms.Button
    Private WithEvents dataGridView3 As System.Windows.Forms.DataGridView
    Private WithEvents tabPage4 As System.Windows.Forms.TabPage
    Private WithEvents pictureBox1 As System.Windows.Forms.PictureBox

End Class

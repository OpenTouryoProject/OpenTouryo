'**********************************************************************************
'* �T���v�� �A�v�����
'**********************************************************************************

'**********************************************************************************
'* �N���X��        �FForm1
'* �N���X���{�ꖼ  �F������������Dao�̗��p�T���v��
'*                   �{ �f�[�^�e�[�u�����g�p�����o�b�`�X�V�T���v��
'*
'* �쐬����        �F�|
'* �쐬��          �Fsas ���Z
'* �X�V����        �F
'*
'*  ����        �X�V��            ���e
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  �w�w �w�w         �w�w�w�w
'*
'**********************************************************************************

' Window�A�v���P�[�V����
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel

' �^���
Imports GenDaoAndBatUpd_sample.Common
Imports GenDaoAndBatUpd_sample.Business

' System
Imports System
Imports System.IO
Imports System.Xml
Imports System.Text
Imports System.Data
Imports System.Collections

' �Ɩ��t���[�����[�N
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util

Imports Touryo.Infrastructure.Business.RichClient.Business

' �t���[�����[�N
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

Imports Touryo.Infrastructure.Framework.RichClient.Business

' ���i
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

''' <summary>������������Dao�̗��p�T���v���{�f�[�^�e�[�u�����g�p�����o�b�`�X�V�T���v��</summary>
Partial Public Class Form1
    Inherits Form
    ''' <summary>���[�U���</summary>
    Private myUserInfo As MyUserInfo

#Region "��������"

    ''' <summary>�R���X�g���N�^</summary>
    Public Sub New()
        InitializeComponent()

        ' ���ߍ��܂ꂽ���\�[�X���[�h
        MyBaseDao.UseEmbeddedResource = True
    End Sub

    ''' <summary>���[�h �C�x���g</summary>
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        '#Region "�t���[���C�A�E�g���ɂ���B"

        ' �^�u
        Me.tabControl1.Anchor = (AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)

        ' �O���b�h
        Me.dataGridView1.Anchor = (AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)
        Me.dataGridView2.Anchor = (AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)
        Me.dataGridView3.Anchor = (AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)

        ' �s�N�`��
        Me.pictureBox1.Anchor = (AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)

        ' �{�^���`
        Me.btnInsert1.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)
        Me.btnInsert2.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)

        Me.btnSelect1.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)
        Me.btnSelect2.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)

        Me.btnUpdate1.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)
        Me.btnUpdate2.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)

        Me.btnDelete1.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)
        Me.btnDelete2.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left)

        ' �{�^���a
        Me.btnSelectAll1.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)
        Me.btnSelectAll2.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)
        Me.btnSelectAll3.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)

        Me.btnClear1.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)
        Me.btnClear2.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)
        Me.btnClear3.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)

        Me.btnBatUpd.Anchor = (AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right)

        '#End Region

        ' ���[�U���
        Me.myUserInfo = New MyUserInfo("userName", Environment.MachineName)
    End Sub

#End Region

#Region "�f�[�^�̃��[�h"

    ''' <summary>Suppliers�e�[�u���̎擾</summary>
    Private Function GetSuppliers(ByVal controlId As String) As DataTable
        ' ����
        Dim testParameterValue As New TestParameterValue(Me.Name, controlId, "SelectAll", "SQL", Me.myUserInfo)

        ' �a�w�Ăяo��
        Dim lb As New LayerB_Static()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' �R�~�b�g
        BaseLogic2CS.CommitAndClose()

        ' �߂�l
        Return DirectCast(testReturnValue.dt, DataTable)
    End Function

    ''' <summary>Category�e�[�u���̎擾</summary>
    Private Function GetCategory(ByVal controlId As String) As DataTable
        ' ����
        Dim testParameterValue As New TestParameterValue(Me.Name, controlId, "SelectAll", "SQL", Me.myUserInfo)

        ' �a�w�Ăяo��
        Dim lb As New LayerB_Dynamic()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' �R�~�b�g
        BaseLogic2CS.CommitAndClose()

        ' �߂�l
        Return DirectCast(testReturnValue.dt, DataTable)
    End Function

#End Region

#Region "�^�u�P"

#Region "�F���N���A"

    ''' <summary>�F���N���A</summary>
    Private Sub ClearColor1()
        txtSupplierID.BackColor = Color.White
        txtCompanyName.BackColor = Color.White
        txtContactName.BackColor = Color.White
        txtContactTitle.BackColor = Color.White
        txtAddress.BackColor = Color.White
        txtCity.BackColor = Color.White
        txtRegion.BackColor = Color.White
        txtPostalCode.BackColor = Color.White
        txtCountry.BackColor = Color.White
        txtPhone.BackColor = Color.White
        txtFax.BackColor = Color.White
        txtHomePage.BackColor = Color.White
    End Sub

#End Region

#Region "�f�[�^�O���b�h�P"

    ''' <summary>�O���b�h�P�Ƀf�[�^�����[�h</summary>
    Private Sub btnSelectAll1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelectAll1.Click
        ' �f�[�^�����[�h
        Me.dataGridView1.DataSource = Me.GetSuppliers(DirectCast(sender, Button).Name)
    End Sub

    ''' <summary>�O���b�h�P���N���A</summary>
    Private Sub btnClear1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClear1.Click
        ' �F�̃N���A
        Me.ClearColor1()

        ' �N���A
        Me.dataGridView1.DataSource = Nothing
    End Sub

#End Region

#Region "�ÓISQL��CRUD"

    ''' <summary>�C���T�[�g</summary>
    Private Sub btnInsert1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInsert1.Click
        ' �F�̃N���A
        Me.ClearColor1()

        ' ����
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Insert", "SQL", Me.myUserInfo)

        testParameterValue.field1 = ""
        ' SupplierID
        testParameterValue.field2 = txtCompanyName.Text
        ' CompanyName
        txtCompanyName.BackColor = Color.LightYellow

        testParameterValue.field3 = txtContactName.Text
        ' ContactName
        txtContactName.BackColor = Color.LightYellow

        testParameterValue.field4 = txtContactTitle.Text
        ' ContactTitle
        txtContactTitle.BackColor = Color.LightYellow

        testParameterValue.field5 = txtAddress.Text
        ' Address
        txtAddress.BackColor = Color.LightYellow

        testParameterValue.field6 = txtCity.Text
        ' City
        txtCity.BackColor = Color.LightYellow

        testParameterValue.field7 = txtRegion.Text
        ' Region
        txtRegion.BackColor = Color.LightYellow

        testParameterValue.field8 = txtPostalCode.Text
        ' PostalCode
        txtPostalCode.BackColor = Color.LightYellow

        testParameterValue.field9 = txtCountry.Text
        ' Country
        txtCountry.BackColor = Color.LightYellow

        testParameterValue.field10 = txtPhone.Text
        ' Phone
        txtPhone.BackColor = Color.LightYellow

        testParameterValue.field11 = txtFax.Text
        ' Fax
        txtFax.BackColor = Color.LightYellow

        testParameterValue.field12 = txtHomePage.Text
        ' HomePage
        txtHomePage.BackColor = Color.LightYellow

        ' �a�w�Ăяo��
        Dim lb As New LayerB_Static()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' �R�~�b�g
        BaseLogic2CS.CommitAndClose()

        ' �f�[�^�O���b�h���X�V
        Me.btnSelectAll1_Click(sender, e)
    End Sub

    ''' <summary>�Z���N�g</summary>
    Private Sub btnSelect1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelect1.Click
        ' �F�̃N���A
        Me.ClearColor1()

        ' ��L�[��������΁A�������Ȃ��B
        If txtSupplierID.Text = "" Then
            MessageBox.Show("��L�[�iSupplierID�j����͂��Ă��������B")
            Return
        End If

        ' ����
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Select", "SQL", Me.myUserInfo)

        testParameterValue.field1 = txtSupplierID.Text
        ' SupplierID
        txtSupplierID.BackColor = Color.LightYellow

        ' �a�w�Ăяo��
        Dim lb As New LayerB_Static()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' �R�~�b�g
        BaseLogic2CS.CommitAndClose()

        ' �߂�l
        txtCompanyName.Text = testReturnValue.field2.ToString()
        ' CompanyName
        txtContactName.Text = testReturnValue.field3.ToString()
        ' ContactName
        txtContactTitle.Text = testReturnValue.field4.ToString()
        ' ContactTitle
        txtAddress.Text = testReturnValue.field5.ToString()
        ' Address
        txtCity.Text = testReturnValue.field6.ToString()
        ' City
        txtRegion.Text = testReturnValue.field7.ToString()
        ' Region
        txtPostalCode.Text = testReturnValue.field8.ToString()
        ' PostalCode
        txtCountry.Text = testReturnValue.field9.ToString()
        ' Country
        txtPhone.Text = testReturnValue.field10.ToString()
        ' Phone
        txtFax.Text = testReturnValue.field11.ToString()
        ' Fax
        txtHomePage.Text = testReturnValue.field12.ToString()
        ' HomePage
    End Sub

    ''' <summary>�A�b�v�f�[�g</summary>
    Private Sub btnUpdate1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdate1.Click
        ' �F�̃N���A
        Me.ClearColor1()

        ' ��L�[��������΁A�������Ȃ��B
        If txtSupplierID.Text = "" Then
            MessageBox.Show("��L�[�iSupplierID�j����͂��Ă��������B")
            Return
        End If

        ' ����
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Update", "SQL", Me.myUserInfo)

        testParameterValue.field1 = txtSupplierID.Text
        ' SupplierID
        txtSupplierID.BackColor = Color.LightYellow

        testParameterValue.field2_ForUpd = txtCompanyName.Text
        ' CompanyName
        txtCompanyName.BackColor = Color.LightYellow

        testParameterValue.field3_ForUpd = txtContactName.Text
        ' ContactName
        txtContactName.BackColor = Color.LightYellow

        testParameterValue.field4_ForUpd = txtContactTitle.Text
        ' ContactTitle
        txtContactTitle.BackColor = Color.LightYellow

        testParameterValue.field5_ForUpd = txtAddress.Text
        ' Address
        txtAddress.BackColor = Color.LightYellow

        testParameterValue.field6_ForUpd = txtCity.Text
        ' City
        txtCity.BackColor = Color.LightYellow

        testParameterValue.field7_ForUpd = txtRegion.Text
        ' Region
        txtRegion.BackColor = Color.LightYellow

        testParameterValue.field8_ForUpd = txtPostalCode.Text
        ' PostalCode
        txtPostalCode.BackColor = Color.LightYellow

        testParameterValue.field9_ForUpd = txtCountry.Text
        ' Country
        txtCountry.BackColor = Color.LightYellow

        testParameterValue.field10_ForUpd = txtPhone.Text
        ' Phone
        txtPhone.BackColor = Color.LightYellow

        testParameterValue.field11_ForUpd = txtFax.Text
        ' Fax
        txtFax.BackColor = Color.LightYellow

        testParameterValue.field12_ForUpd = txtHomePage.Text
        ' HomePage
        txtHomePage.BackColor = Color.LightYellow

        ' �a�w�Ăяo��
        Dim lb As New LayerB_Static()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' �R�~�b�g
        BaseLogic2CS.CommitAndClose()

        ' �f�[�^�O���b�h���X�V
        Me.btnSelectAll1_Click(sender, e)
    End Sub

    ''' <summary>�f���[�g</summary>
    Private Sub btnDelete1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete1.Click
        ' �F�̃N���A
        Me.ClearColor1()

        ' ��L�[��������΁A�������Ȃ��B
        If txtSupplierID.Text = "" Then
            MessageBox.Show("��L�[�iSupplierID�j����͂��Ă��������B")
            Return
        End If

        ' ����
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Delete", "SQL", Me.myUserInfo)

        testParameterValue.field1 = txtSupplierID.Text
        ' SupplierID
        txtSupplierID.BackColor = Color.LightYellow

        ' �a�w�Ăяo��
        Dim lb As New LayerB_Static()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' �R�~�b�g
        BaseLogic2CS.CommitAndClose()

        ' �f�[�^�O���b�h���X�V
        Me.btnSelectAll1_Click(sender, e)
    End Sub

#End Region

#End Region

#Region "�^�u�Q"

#Region "�F���N���A"

    ''' <summary>�F���N���A</summary>
    Private Sub ClearColor2()
        txtCategoryID.BackColor = Color.White
        txtCategoryName.BackColor = Color.White
        txtDescription.BackColor = Color.White
        'txtPicture.BackColor = Color.White;

        txtCategoryID_where.BackColor = Color.White
        txtCategoryName_where.BackColor = Color.White
        'txtDescription_where.BackColor = Color.White;
        'txtPicture_where.BackColor = Color.White;
    End Sub

#End Region

#Region "�f�[�^�O���b�h�Q"

    ''' <summary>�O���b�h�Q�Ƀf�[�^�����[�h</summary>
    Private Sub btnSelectAll2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelectAll2.Click
        ' �f�[�^�����[�h
        Me.dataGridView2.DataSource = Me.GetCategory(DirectCast(sender, Button).Name)
    End Sub

    ''' <summary>�O���b�h�Q���N���A</summary>
    Private Sub btnClear2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClear2.Click
        ' �F�̃N���A
        Me.ClearColor2()

        ' �N���A
        Me.dataGridView2.DataSource = Nothing
    End Sub

#End Region

#Region "���ISQL��CRUD"

    ''' <summary>�C���T�[�g</summary>
    Private Sub btnInsert2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInsert2.Click
        ' �F�̃N���A
        Me.ClearColor2()

        ' ����
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Insert", "SQL", Me.myUserInfo)

        ' �f�[�^����͂ł��Ȃ��̂Ńp�X
        'testParameterValue.field1 = this.txtCategoryID;          // CategoryID
        'this.txtCategoryID.BackColor = Color.LightYellow;

        testParameterValue.field2 = Me.txtCategoryName.Text
        ' CategoryName
        Me.txtCategoryName.BackColor = Color.LightYellow

        testParameterValue.field3 = Me.txtDescription.Text
        ' Description
        Me.txtDescription.BackColor = Color.LightYellow

        ' �f�[�^����͂ł��Ȃ��̂Ńp�X
        'testParameterValue.field4 = this.txtPicture.Text;      // Picture
        'this.txtPicture.BackColor = Color.LightYellow;

        ' �a�w�Ăяo��
        Dim lb As New LayerB_Dynamic()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' �R�~�b�g
        BaseLogic2CS.CommitAndClose()

        ' �f�[�^�O���b�h���X�V
        Me.btnSelectAll2_Click(sender, e)
    End Sub

    ''' <summary>�Z���N�g</summary>
    Private Sub btnSelect2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelect2.Click
        ' �F�̃N���A
        Me.ClearColor2()

        ' ����
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Select", "SQL", Me.myUserInfo)

        testParameterValue.field1_ForSearch = txtCategoryID_where.Text
        ' CategoryID_where
        txtCategoryID_where.BackColor = Color.LightYellow

        testParameterValue.field2_ForSearch = txtCategoryName_where.Text
        ' CategoryName_where
        txtCategoryName_where.BackColor = Color.LightYellow

        ' ���������Ɏg���Ȃ���

        'testParameterValue.field3_ForSearch = txtDescription_where.Text;  // Description_where
        'txtDescription_where.BackColor = Color.LightYellow;

        'testParameterValue.field4_ForSearch = txtPicture_where.Text;      // Picture
        'txtPicture_where.BackColor = Color.LightYellow;

        ' �a�w�Ăяo��
        Dim lb As New LayerB_Dynamic()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' �R�~�b�g
        BaseLogic2CS.CommitAndClose()

        ' �߂�l��ݒ�
        Me.dataGridView2.DataSource = testReturnValue.dt
    End Sub

    ''' <summary>�A�b�v�f�[�g</summary>
    Private Sub btnUpdate2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdate2.Click
        ' �F�̃N���A
        Me.ClearColor2()

        ' ����
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Update", "SQL", Me.myUserInfo)

        ' �X�V�l
        'testParameterValue.field1_ForUpd = txtCategoryID.Text;     // CategoryID
        'txtCategoryID.BackColor = Color.LightYellow;

        testParameterValue.field2_ForUpd = txtCategoryName.Text
        ' CategoryName
        txtCategoryName.BackColor = Color.LightYellow

        testParameterValue.field3_ForUpd = txtDescription.Text
        ' Description
        txtDescription.BackColor = Color.LightYellow

        ' ��������
        testParameterValue.field1_ForSearch = txtCategoryID_where.Text
        ' CategoryID_where
        txtCategoryID_where.BackColor = Color.LightYellow

        testParameterValue.field2_ForSearch = txtCategoryName_where.Text
        ' CategoryName_where
        txtCategoryName_where.BackColor = Color.LightYellow

        ' ���������Ɏg���Ȃ���

        'testParameterValue.field3_ForSearch = txtDescription_where.Text;  // Description_where
        'txtDescription_where.BackColor = Color.LightYellow;

        'testParameterValue.field4_ForSearch = txtPicture_where.Text;      // Picture
        'txtPicture_where.BackColor = Color.LightYellow;

        ' �a�w�Ăяo��
        Dim lb As New LayerB_Dynamic()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' �R�~�b�g
        BaseLogic2CS.CommitAndClose()

        ' �f�[�^�O���b�h���X�V
        Me.btnSelectAll2_Click(sender, e)
    End Sub

    ''' <summary>�f���[�g</summary>
    Private Sub btnDelete2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete2.Click
        ' �F�̃N���A
        Me.ClearColor2()

        ' ����
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Delete", "SQL", Me.myUserInfo)

        ' ��������
        testParameterValue.field1_ForSearch = txtCategoryID_where.Text
        ' CategoryID_where
        txtCategoryID_where.BackColor = Color.LightYellow

        testParameterValue.field2_ForSearch = txtCategoryName_where.Text
        ' CategoryName_where
        txtCategoryName_where.BackColor = Color.LightYellow

        ' ���������Ɏg���Ȃ���

        'testParameterValue.field3_ForSearch = txtDescription_where.Text;  // Description_where
        'txtDescription_where.BackColor = Color.LightYellow;

        'testParameterValue.field4_ForSearch = txtPicture_where.Text;      // Picture
        'txtPicture_where.BackColor = Color.LightYellow;

        ' �a�w�Ăяo��
        Dim lb As New LayerB_Dynamic()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' �R�~�b�g
        BaseLogic2CS.CommitAndClose()

        ' �f�[�^�O���b�h���X�V
        Me.btnSelectAll2_Click(sender, e)
    End Sub

#End Region

#End Region

#Region "�^�u�R"

    ''' <summary>�O���b�h�R�Ƀf�[�^�����[�h</summary>
    Private Sub btnSelectAll3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelectAll3.Click
        ' ����
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "SelectAll", "SQL", Me.myUserInfo)

        ' �a�w�Ăяo��
        Dim lb As New LayerB_BatUpd()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' �R�~�b�g
        BaseLogic2CS.CommitAndClose()

        ' �߂�l��ݒ�i�񂪎����I�ɍ쐬����Ȃ��悤�ɂ���j
        Me.dataGridView3.Columns.Clear()
        Me.dataGridView3.AutoGenerateColumns = False
        Me.dataGridView3.DataSource = testReturnValue.dt

        '#Region "�}�X�^�̃R���{����"

        '#Region "SupplierID - ComboBox"

        Dim dtSuppliers As DataTable = Me.GetSuppliers(DirectCast(sender, Button).Name)

        ' DataGridViewComboBoxColumn���쐬
        Dim cmbColSuppliers As New DataGridViewComboBoxColumn()
        Me.InitDataGridViewComboBoxColumn(cmbColSuppliers)

        ' "SupplierID"��Ƀo�C���h����Ă���f�[�^�Ɗ֘A�t���A
        cmbColSuppliers.DataPropertyName = "SupplierID"
        ' �w�b�_�[�̃e�L�X�g��ύX
        cmbColSuppliers.HeaderText = "Supplier"

        'DataGridViewComboBoxColumn��DataSource��ݒ�
        cmbColSuppliers.DataSource = dtSuppliers

        ' ���ۂ̒l��"SupplierID"��
        ' �\������e�L�X�g��"CompanyName"��
        cmbColSuppliers.ValueMember = "SupplierID"
        cmbColSuppliers.DisplayMember = "CompanyName"

        '#End Region

        '#Region "CategoryID - ComboBox"

        Dim dtCategory As DataTable = Me.GetCategory("btnSelectAll3")

        ' DataGridViewComboBoxColumn���쐬
        Dim cmbColCategory As New DataGridViewComboBoxColumn()
        Me.InitDataGridViewComboBoxColumn(cmbColCategory)

        ' "SupplierID"��Ƀo�C���h����Ă���f�[�^�Ɗ֘A�t���A
        cmbColCategory.DataPropertyName = "CategoryID"
        ' �w�b�_�[�̃e�L�X�g��ύX
        cmbColCategory.HeaderText = "Category"

        ' DataGridViewComboBoxColumn��DataSource��ݒ�
        cmbColCategory.DataSource = dtCategory

        ' ���ۂ̒l��"CategoryID"��
        ' �\������e�L�X�g��"CategoryName"��
        cmbColCategory.ValueMember = "CategoryID"
        cmbColCategory.DisplayMember = "CategoryName"

        '#End Region

        '#End Region

        '#Region "�蓮�Ńf�[�^�o�C���h"

        ' �͂��߂ɃN���A

        ' DataGridViewTextBoxColumn
        Dim textColumn As DataGridViewTextBoxColumn

        Dim checkColumn As DataGridViewCheckBoxColumn

        '�f�[�^�\�[�X��"ProductID"����o�C���h����
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "ProductID"
        textColumn.Name = "ProductID"
        textColumn.HeaderText = "ProductID"

        ' ��L�[�͓ǂݎ���p
        textColumn.[ReadOnly] = True

        Me.dataGridView3.Columns.Add(textColumn)

        '�f�[�^�\�[�X��"ProductName"����o�C���h����
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "ProductName"
        textColumn.Name = "ProductName"
        textColumn.HeaderText = "ProductName"
        Me.dataGridView3.Columns.Add(textColumn)

        '�f�[�^�\�[�X��"SupplierID"����o�C���h����
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "SupplierID"
        textColumn.Name = "SupplierID"
        textColumn.HeaderText = "SupplierID"
        Me.dataGridView3.Columns.Add(textColumn)

        ' �����Ȃ����ă}�X�^���R���{��ǉ�
        Me.dataGridView3.Columns("SupplierID").Visible = False
        Me.dataGridView3.Columns.Add(cmbColSuppliers)

        '�f�[�^�\�[�X��"CategoryID"����o�C���h����
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "CategoryID"
        textColumn.Name = "CategoryID"
        textColumn.HeaderText = "CategoryID"
        Me.dataGridView3.Columns.Add(textColumn)

        ' �����Ȃ����ă}�X�^���R���{��ǉ�
        Me.dataGridView3.Columns("CategoryID").Visible = False
        Me.dataGridView3.Columns.Add(cmbColCategory)

        '�f�[�^�\�[�X��"QuantityPerUnit"����o�C���h����
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "QuantityPerUnit"
        textColumn.Name = "QuantityPerUnit"
        textColumn.HeaderText = "QuantityPerUnit"
        Me.dataGridView3.Columns.Add(textColumn)

        '�f�[�^�\�[�X��"UnitPrice"����o�C���h����
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "UnitPrice"
        textColumn.Name = "UnitPrice"
        textColumn.HeaderText = "UnitPrice"
        Me.dataGridView3.Columns.Add(textColumn)

        '�f�[�^�\�[�X��"UnitsInStock"����o�C���h����
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "UnitsInStock"
        textColumn.Name = "UnitsInStock"
        textColumn.HeaderText = "UnitsInStock"
        Me.dataGridView3.Columns.Add(textColumn)

        '�f�[�^�\�[�X��"UnitsOnOrder"����o�C���h����
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "UnitsOnOrder"
        textColumn.Name = "UnitsOnOrder"
        textColumn.HeaderText = "UnitsOnOrder"
        Me.dataGridView3.Columns.Add(textColumn)

        '�f�[�^�\�[�X��"ReorderLevel"����o�C���h����
        textColumn = New DataGridViewTextBoxColumn()
        textColumn.DataPropertyName = "ReorderLevel"
        textColumn.Name = "ReorderLevel"
        textColumn.HeaderText = "ReorderLevel"
        Me.dataGridView3.Columns.Add(textColumn)

        '�f�[�^�\�[�X��"Discontinued"����o�C���h����
        checkColumn = New DataGridViewCheckBoxColumn()
        checkColumn.DataPropertyName = "Discontinued"
        checkColumn.Name = "Discontinued"
        checkColumn.HeaderText = "Discontinued"
        Me.dataGridView3.Columns.Add(checkColumn)

        '#End Region
    End Sub

    ''' <summary>DataGridViewComboBoxColumn�̃X�^�C��������������B</summary>
    Private Sub InitDataGridViewComboBoxColumn(ByVal cmbCol As DataGridViewComboBoxColumn)
        ' ���݂̃Z�������R���{�{�b�N�X���\������Ȃ��悤�ɂ���B
        cmbCol.DisplayStyleForCurrentCellOnly = True
        ' �ҏW���[�h�̎������R���{�{�b�N�X��\������B
        cmbCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.[Nothing]

        ' �}�E�X�|�C���^���̃Z���������\�������悤�ɂ���B
        cmbCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
        ' �}�E�X�|�C���^���̃Z���Ƀ|�b�v�A�b�v���\�������悤�ɂ���B
        cmbCol.FlatStyle = FlatStyle.Popup
    End Sub

    ''' <summary>�o�b�`�X�V</summary>
    Private Sub btnBatUpd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnBatUpd.Click
        ' ����
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "BatUpd", "SQL", Me.myUserInfo)

        ' �ҏW�ς݂�DataTable��ݒ�
        testParameterValue.dt = DirectCast(Me.dataGridView3.DataSource, DataTable)

        ' �a�w�Ăяo��
        Dim lb As New LayerB_BatUpd()

        Dim testReturnValue As TestReturnValue = DirectCast(lb.DoBusinessLogic(testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted), TestReturnValue)

        ' �R�~�b�g
        BaseLogic2CS.CommitAndClose()

        ' �f�[�^�O���b�h���X�V
        Me.btnSelectAll3_Click(sender, e)
    End Sub

    ''' <summary>�f�[�^�G���[���̃C�x���g�n���h��</summary>
    Private Sub dataGridView3_DataError(ByVal sender As Object, ByVal e As DataGridViewDataErrorEventArgs)
        MessageBox.Show(e.Exception.Message)
    End Sub

    ''' <summary>�N���A</summary>
    Private Sub btnClear3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClear3.Click
        ' �N���A
        Me.dataGridView3.DataSource = Nothing
    End Sub

#End Region
End Class

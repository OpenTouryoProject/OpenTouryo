'**********************************************************************************
'* �T���v�� �A�v�����
'**********************************************************************************

'**********************************************************************************
'* �N���X��        �FForm1
'* �N���X���{�ꖼ  �F�T���v�� �A�v�����
'*
'* �쐬����        �F�|
'* �쐬��          �Fsas ���Z
'* �X�V����        �F
'*
'*  ����        �X�V��            ���e
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  �w�w �w�w         �w�w�w�w
'**********************************************************************************

' Window�A�v���P�[�V����
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel

' �^���
Imports _2CSClientWin_sample.Common
Imports _2CSClientWin_sample.Business

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

Imports Touryo.Infrastructure.Business.RichClient.Asynchronous
Imports Touryo.Infrastructure.Business.RichClient.Presentation

' �t���[�����[�N
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

Imports Touryo.Infrastructure.Framework.RichClient.Presentation

' ���i
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

''' <summary>�T���v�� �A�v�����</summary>
Partial Public Class Form1

#Region "��������"

    ''' <summary>�R���X�g���N�^</summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' �t�H�[�����[�h��UOC���\�b�h�i�ʁj
    ''' </summary>
    Protected Overrides Sub UOC_FormInit()
        ' �t�H�[���������i���񃍁[�h�j���Ɏ��s���鏈������������

        ' TODO:

        ' ddlDap
        Me.ddlDap.Items.Add(New ComboBoxItem("SQL Server / SQL Client", "SQL"))
        Me.ddlDap.Items.Add(New ComboBoxItem("Multi-DB / OLEDB.NET", "OLE"))
        Me.ddlDap.Items.Add(New ComboBoxItem("Multi-DB / ODCB.NET", "ODB"))
        Me.ddlDap.Items.Add(New ComboBoxItem("Oracle / ODP.NET", "ODP"))
        Me.ddlDap.Items.Add(New ComboBoxItem("DB2 / DB2.NET", "DB2"))
        Me.ddlDap.Items.Add(New ComboBoxItem("HiRDB / HiRDB-DP", "HIR"))
        Me.ddlDap.Items.Add(New ComboBoxItem("MySQL Cnn/NET", "MCN"))
        Me.ddlDap.Items.Add(New ComboBoxItem("PostgreSQL / Npgsql", "NPS"))
        Me.ddlDap.SelectedIndex = 0

        ' ddlMode1
        Me.ddlMode1.Items.Add(New ComboBoxItem("�ʂc����", "individual"))
        Me.ddlMode1.Items.Add(New ComboBoxItem("���ʂc����", "common"))
        Me.ddlMode1.Items.Add(New ComboBoxItem("���������c�����i�X�V�̂݁j", "generate"))
        Me.ddlMode1.SelectedIndex = 0

        ' ddlMode2
        Me.ddlMode2.Items.Add(New ComboBoxItem("�ÓI�N�G��", "static"))
        Me.ddlMode2.Items.Add(New ComboBoxItem("���I�N�G��", "dynamic"))
        Me.ddlMode2.SelectedIndex = 0

        ' ddlIso
        Me.ddlIso.Items.Add(New ComboBoxItem("�m�b�g�R�l�N�g", "NC"))
        Me.ddlIso.Items.Add(New ComboBoxItem("�m�[�g�����U�N�V����", "NT"))
        Me.ddlIso.Items.Add(New ComboBoxItem("�_�[�e�B���[�h", "RU"))
        Me.ddlIso.Items.Add(New ComboBoxItem("���[�h�R�~�b�g", "RC"))
        Me.ddlIso.Items.Add(New ComboBoxItem("���s�[�^�u�����[�h", "RR"))
        Me.ddlIso.Items.Add(New ComboBoxItem("�V���A���C�U�u��", "SZ"))
        Me.ddlIso.Items.Add(New ComboBoxItem("�X�i�b�v�V���b�g", "SS"))
        Me.ddlIso.Items.Add(New ComboBoxItem("�f�t�H���g", "DF"))
        Me.ddlIso.SelectedIndex = 1

        ' ddlExRollback
        Me.ddlExRollback.Items.Add(New ComboBoxItem("���펞", "-"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("�Ɩ���O", "Business"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("�V�X�e����O", "System"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("���̑��A��ʓI�ȗ�O", "Other"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("�Ɩ���O�ւ̐U��", "Other-Business"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("�V�X�e����O�ւ̐U��", "Other-System"))
        Me.ddlExRollback.SelectedIndex = 0

        ' ddlOrderColumn
        Me.ddlOrderColumn.Items.Add(New ComboBoxItem("c1", "c1"))
        Me.ddlOrderColumn.Items.Add(New ComboBoxItem("c2", "c2"))
        Me.ddlOrderColumn.Items.Add(New ComboBoxItem("c3", "c3"))
        Me.ddlOrderColumn.SelectedIndex = 0

        ' ddlOrderSequence
        Me.ddlOrderSequence.Items.Add(New ComboBoxItem("ASC", "A"))
        Me.ddlOrderSequence.Items.Add(New ComboBoxItem("DESC", "D"))
        Me.ddlOrderSequence.SelectedIndex = 0
    End Sub

#Region "�R���{�{�b�N�X�p"

    ''' <summary>�R���{�{�b�N�X�p�C���i�[�N���X</summary>
    Private Class ComboBoxItem
        ''' <summary>�\����</summary>
        Private m_name As String = ""

        ''' <summary>�l</summary>
        Private m_value As String = ""

        ''' <summary>�R���X�g���N�^</summary>
        Public Sub New(ByVal name As String, ByVal value As String)
            m_name = name
            m_value = value
        End Sub

        ''' <summary>�\����</summary>
        Public ReadOnly Property Name() As String
            Get
                Return m_name
            End Get
        End Property

        ''' <summary>�l</summary>
        Public ReadOnly Property Value() As String
            Get
                Return m_value
            End Get
        End Property

        ''' <summary>
        ''' �I�[�o�[���C�h�������\�b�h
        ''' ���ꂪ�R���{�{�b�N�X�ɕ\�������
        ''' </summary>
        Public Overrides Function ToString() As String
            Return m_name
        End Function
    End Class

#End Region

#End Region

#Region "�b�q�t�c�������\�b�h"

#Region "�Q�ƌn"

    ''' <summary>�����擾</summary>
    ''' <param name="rcFxEventArgs">�C�x���g�n���h���̋��ʈ���</param>
    Protected Sub UOC_btnButton1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' �����N���X�𐶐�
        ' ���ʁi�a�E�c�w�j�́A�e�X�g �N���X�𗬗p����
        Dim testParameterValue As New TestParameterValue(Me.Name, rcFxEventArgs.ControlName, "SelectCount", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' �߂�l
        Dim testReturnValue As TestReturnValue

        ' �������x���̐ݒ�
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' �a�w�ďo���{�s�x�R�~�b�g
        Dim layerB__1 As New LayerB()
        testReturnValue = DirectCast(layerB__1.DoBusinessLogic(testParameterValue, iso), TestReturnValue)
        LayerB.CommitAndClose()

        ' ���ʕ\�����郁�b�Z�[�W �G���A
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' ���ʁi�Ɩ����s�\�ȃG���[�j
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' ���ʁi����n�j
            labelMessage.Text = testReturnValue.Obj.ToString() & "���̃f�[�^������܂�"
        End If
    End Sub

    ''' <summary>�ꗗ�擾�idt�j</summary>
    ''' <param name="rcFxEventArgs">�C�x���g�n���h���̋��ʈ���</param>
    Protected Sub UOC_btnButton2_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' �����N���X�𐶐�
        ' ���ʁi�a�E�c�w�j�́A�e�X�g �N���X�𗬗p����
        Dim testParameterValue As New TestParameterValue(Me.Name, rcFxEventArgs.ControlName, "SelectAll_DT", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' �߂�l
        Dim testReturnValue As TestReturnValue

        ' �������x���̐ݒ�
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' �a�w�ďo���{�s�x�R�~�b�g
        Dim layerB__1 As New LayerB()
        testReturnValue = DirectCast(layerB__1.DoBusinessLogic(testParameterValue, iso), TestReturnValue)
        LayerB.CommitAndClose()

        ' ���ʕ\�����郁�b�Z�[�W �G���A
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' ���ʁi�Ɩ����s�\�ȃG���[�j
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' ���ʁi����n�j
            Me.dataGridView1.DataSource = testReturnValue.Obj
        End If
    End Sub

    ''' <summary>�ꗗ�擾�ids�j</summary>
    ''' <param name="rcFxEventArgs">�C�x���g�n���h���̋��ʈ���</param>
    Protected Sub UOC_btnButton3_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' �����N���X�𐶐�
        ' ���ʁi�a�E�c�w�j�́A�e�X�g �N���X�𗬗p����
        Dim testParameterValue As New TestParameterValue(Me.Name, rcFxEventArgs.ControlName, "SelectAll_DS", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' �߂�l
        Dim testReturnValue As TestReturnValue

        ' �������x���̐ݒ�
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' �a�w�ďo���{�s�x�R�~�b�g
        Dim layerB__1 As New LayerB()
        testReturnValue = DirectCast(layerB__1.DoBusinessLogic(testParameterValue, iso), TestReturnValue)
        LayerB.CommitAndClose()

        ' ���ʕ\�����郁�b�Z�[�W �G���A
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' ���ʁi�Ɩ����s�\�ȃG���[�j
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' ���ʁi����n�j
            Me.dataGridView1.DataSource = DirectCast(testReturnValue.Obj, DataSet).Tables(0)
        End If
    End Sub

    ''' <summary>�ꗗ�擾�idr�j</summary>
    ''' <param name="rcFxEventArgs">�C�x���g�n���h���̋��ʈ���</param>
    Protected Sub UOC_btnButton4_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' �����N���X�𐶐�
        ' ���ʁi�a�E�c�w�j�́A�e�X�g �N���X�𗬗p����
        Dim testParameterValue As New TestParameterValue(Me.Name, rcFxEventArgs.ControlName, "SelectAll_DR", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' �߂�l
        Dim testReturnValue As TestReturnValue

        ' �������x���̐ݒ�
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' �a�w�ďo���{�s�x�R�~�b�g
        Dim layerB__1 As New LayerB()
        testReturnValue = DirectCast(layerB__1.DoBusinessLogic(testParameterValue, iso), TestReturnValue)
        LayerB.CommitAndClose()

        ' ���ʕ\�����郁�b�Z�[�W �G���A
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' ���ʁi�Ɩ����s�\�ȃG���[�j
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' ���ʁi����n�j
            Me.dataGridView1.DataSource = testReturnValue.Obj
        End If
    End Sub

    ''' <summary>�ꗗ�擾�i���Isql�j</summary>
    ''' <param name="rcFxEventArgs">�C�x���g�n���h���̋��ʈ���</param>
    Protected Sub UOC_btnButton5_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' �����N���X�𐶐�
        ' ���ʁi�a�E�c�w�j�́A�e�X�g �N���X�𗬗p����
        Dim testParameterValue As New TestParameterValue(Me.Name, rcFxEventArgs.ControlName, "SelectAll_DSQL", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' ���ISQL�̗v�f��ݒ�
        testParameterValue.OrderColumn = DirectCast(Me.ddlOrderColumn.SelectedItem, ComboBoxItem).Value
        testParameterValue.OrderSequence = DirectCast(Me.ddlOrderSequence.SelectedItem, ComboBoxItem).Value

        ' �߂�l
        Dim testReturnValue As TestReturnValue

        ' �������x���̐ݒ�
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' �a�w�ďo���{�s�x�R�~�b�g
        Dim layerB__1 As New LayerB()
        testReturnValue = DirectCast(layerB__1.DoBusinessLogic(testParameterValue, iso), TestReturnValue)
        LayerB.CommitAndClose()

        ' ���ʕ\�����郁�b�Z�[�W �G���A
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' ���ʁi�Ɩ����s�\�ȃG���[�j
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' ���ʁi����n�j
            Me.dataGridView1.DataSource = testReturnValue.Obj
        End If
    End Sub

    ''' <summary>�Q�Ə���</summary>
    ''' <param name="rcFxEventArgs">�C�x���g�n���h���̋��ʈ���</param>
    Protected Sub UOC_btnButton6_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' �����N���X�𐶐�
        ' ���ʁi�a�E�c�w�j�́A�e�X�g �N���X�𗬗p����
        Dim testParameterValue As New TestParameterValue(Me.Name, rcFxEventArgs.ControlName, "Select", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' ���̐ݒ�
        testParameterValue.ShipperID = Integer.Parse(Me.textBox1.Text)

        ' �߂�l
        Dim testReturnValue As TestReturnValue

        ' �������x���̐ݒ�
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' �a�w�ďo���{�s�x�R�~�b�g
        Dim layerB__1 As New LayerB()
        testReturnValue = DirectCast(layerB__1.DoBusinessLogic(testParameterValue, iso), TestReturnValue)
        LayerB.CommitAndClose()

        ' ���ʕ\�����郁�b�Z�[�W �G���A
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' ���ʁi�Ɩ����s�\�ȃG���[�j
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' ���ʁi����n�j
            Me.textBox1.Text = testReturnValue.ShipperID.ToString()
            Me.textBox2.Text = testReturnValue.CompanyName
            Me.textBox3.Text = testReturnValue.Phone
        End If
    End Sub

#End Region

#Region "�X�V�n"

    ''' <summary>�ǉ�����</summary>
    ''' <param name="rcFxEventArgs">�C�x���g�n���h���̋��ʈ���</param>
    Protected Sub UOC_btnButton7_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' �����N���X�𐶐�
        ' ���ʁi�a�E�c�w�j�́A�e�X�g �N���X�𗬗p����
        Dim testParameterValue As New TestParameterValue(Me.Name, rcFxEventArgs.ControlName, "Insert", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' ���̐ݒ�
        testParameterValue.CompanyName = Me.textBox2.Text
        testParameterValue.Phone = Me.textBox3.Text

        ' �߂�l
        Dim testReturnValue As TestReturnValue

        ' �������x���̐ݒ�
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' �a�w�ďo���{�s�x�R�~�b�g
        Dim layerB__1 As New LayerB()
        testReturnValue = DirectCast(layerB__1.DoBusinessLogic(testParameterValue, iso), TestReturnValue)
        LayerB.CommitAndClose()

        ' ���ʕ\�����郁�b�Z�[�W �G���A
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' ���ʁi�Ɩ����s�\�ȃG���[�j
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' ���ʁi����n�j
            labelMessage.Text = testReturnValue.Obj.ToString() & "���ǉ�"
        End If
    End Sub

    ''' <summary>�X�V����</summary>
    ''' <param name="rcFxEventArgs">�C�x���g�n���h���̋��ʈ���</param>
    Protected Sub UOC_btnButton8_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' �����N���X�𐶐�
        ' ���ʁi�a�E�c�w�j�́A�e�X�g �N���X�𗬗p����
        Dim testParameterValue As New TestParameterValue(Me.Name, rcFxEventArgs.ControlName, "Update", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' ���̐ݒ�
        testParameterValue.ShipperID = Integer.Parse(Me.textBox1.Text)
        testParameterValue.CompanyName = Me.textBox2.Text
        testParameterValue.Phone = Me.textBox3.Text

        ' �߂�l
        Dim testReturnValue As TestReturnValue

        ' �������x���̐ݒ�
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' �a�w�ďo���{�s�x�R�~�b�g
        Dim layerB__1 As New LayerB()
        testReturnValue = DirectCast(layerB__1.DoBusinessLogic(testParameterValue, iso), TestReturnValue)
        LayerB.CommitAndClose()

        ' ���ʕ\�����郁�b�Z�[�W �G���A
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' ���ʁi�Ɩ����s�\�ȃG���[�j
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' ���ʁi����n�j
            labelMessage.Text = testReturnValue.Obj.ToString() & "���X�V"
        End If
    End Sub

    ''' <summary>�폜����</summary>
    ''' <param name="rcFxEventArgs">�C�x���g�n���h���̋��ʈ���</param>
    Protected Sub UOC_btnButton9_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' �����N���X�𐶐�
        ' ���ʁi�a�E�c�w�j�́A�e�X�g �N���X�𗬗p����
        Dim testParameterValue As New TestParameterValue(Me.Name, rcFxEventArgs.ControlName, "Delete", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' ���̐ݒ�
        testParameterValue.ShipperID = Integer.Parse(textBox1.Text)

        ' �߂�l
        Dim testReturnValue As TestReturnValue

        ' �������x���̐ݒ�
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' �a�w�ďo���{�s�x�R�~�b�g
        Dim layerB__1 As New LayerB()
        testReturnValue = DirectCast(layerB__1.DoBusinessLogic(testParameterValue, iso), TestReturnValue)
        LayerB.CommitAndClose()

        ' ���ʕ\�����郁�b�Z�[�W �G���A
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' ���ʁi�Ɩ����s�\�ȃG���[�j
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' ���ʁi����n�j
            labelMessage.Text = testReturnValue.Obj.ToString() & "���폜"
        End If
    End Sub

#End Region

#End Region

#Region "���̑�"

    ''' <summary>�N���A</summary>
    ''' <param name="rcFxEventArgs">�C�x���g�n���h���̋��ʈ���</param>
    Protected Sub UOC_btnButton10_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Me.dataGridView1.DataSource = Nothing
    End Sub

    ''' <summary>���b�Z�[�W�擾�i���ߍ��܂ꂽ���\�[�X�Ή��j</summary>
    ''' <param name="rcFxEventArgs">�C�x���g�n���h���̋��ʈ���</param>
    Protected Sub UOC_btnButton11_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Me.textBox5.Text = GetMessage.GetMessageDescription(Me.textBox4.Text)
    End Sub

    ''' <summary>���L���擾�i���ߍ��܂ꂽ���\�[�X�Ή��j</summary>
    ''' <param name="rcFxEventArgs">�C�x���g�n���h���̋��ʈ���</param>
    Protected Sub UOC_btnButton12_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Me.textBox7.Text = GetSharedProperty.GetSharedPropertyValue(Me.textBox6.Text)
    End Sub

#End Region

#Region "�������x���̐ݒ胁�\�b�h"

    ''' <summary>�������x���̐ݒ�</summary>
    Private Function SelectIsolationLevel() As DbEnum.IsolationLevelEnum
        If DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "NC" Then
            Return DbEnum.IsolationLevelEnum.NotConnect
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "NT" Then
            Return DbEnum.IsolationLevelEnum.NoTransaction
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "RU" Then
            Return DbEnum.IsolationLevelEnum.ReadUncommitted
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "RC" Then
            Return DbEnum.IsolationLevelEnum.ReadCommitted
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "RR" Then
            Return DbEnum.IsolationLevelEnum.RepeatableRead
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "SZ" Then
            Return DbEnum.IsolationLevelEnum.Serializable
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "SS" Then
            Return DbEnum.IsolationLevelEnum.Snapshot
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "DF" Then
            Return DbEnum.IsolationLevelEnum.DefaultTransaction
        Else
            Throw New Exception("�������x���̐ݒ肪��������")
        End If
    End Function

#End Region
End Class

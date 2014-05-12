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
'*
'**********************************************************************************

' Window�A�v���P�[�V����
Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel

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

' �t���[�����[�N
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

' ���i
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

''' <summary>�T���v�� �A�v�����</summary>
Partial Public Class Form1
    Inherits Form
    ' �^�C���X�^���v �I�u�W�F�N�g�̊i�[
    Private ts As Object

#Region "�f�[�^�A�N�Z�X"

    ' �f�[�^�A�N�Z�X����N���X
    Private dam As DamSqlSvr = Nothing

    ' Dao

    ' datetime
    ' ���[
    Private dao1 As Daots_test_table1 = Nothing
    ' ����
    Private dao2 As Daots_test_table2 = Nothing
    ' �擪
    Private dao3 As Daots_test_table3 = Nothing

    ' timestamp
    ' ���[
    Private daoA As Daots_test_tableA = Nothing
    ' ����
    Private daoB As Daots_test_tableB = Nothing
    ' �擪
    Private daoC As Daots_test_tableC = Nothing

#End Region

#Region "�J�n-�I������"

    ''' <summary>�R���X�g���N�^</summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>�J�n����</summary>
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' �C�x���g�n���h��
        AddHandler Me.dataGridView1.DataError, AddressOf DataGridView_DataError

        ' �X�e�[�^�X
        Me.cmbTSColType.SelectedIndex = 0
        Me.cmbTableType.SelectedIndex = 0

        dam = New DamSqlSvr()
        dam.Obj = New MyUserInfo("userName", Environment.MachineName)
        Me.dam.ConnectionOpen(GetConfigParameter.GetConnectionString("ConnectionString_SQL"))
    End Sub

    ''' <summary>�I������</summary>
    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
        Me.dam.ConnectionClose()
    End Sub

    'DataError�C�x���g�n���h��
    Private Sub DataGridView_DataError(ByVal sender As Object, ByVal e As DataGridViewDataErrorEventArgs)
        e.Cancel = False
    End Sub

#End Region

#Region "��Ԃ̎擾����"

    ''' <summary>��Ԃ̎擾</summary>
    ''' <returns>��Ԃ�\�����l</returns>
    Private Function GetStatus() As Integer
        If Me.cmbTSColType.Text = "RAND�ifloat�j��" Then
            If Me.cmbTableType.Text = "TS�񖖒[" Then
                Return 1
            ElseIf Me.cmbTableType.Text = "TS�񒆊�" Then
                Return 2
            ElseIf Me.cmbTableType.Text = "TS��擪" Then
                Return 3
            End If
        ElseIf Me.cmbTSColType.Text = "timestamp��" Then
            If Me.cmbTableType.Text = "TS�񖖒[" Then
                Return 4
            ElseIf Me.cmbTableType.Text = "TS�񒆊�" Then
                Return 5
            ElseIf Me.cmbTableType.Text = "TS��擪" Then
                Return 6
            End If
        End If

        Throw New Exception("�s���ȏ�Ԃł��B")
    End Function

#End Region

#Region "�e�[�u���`�F�b�N"

    ''' <summary>�S���擾</summary>
    Private Sub btnGetAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGetAll.Click
        Dim dt As New DataTable()

        If Me.GetStatus() = 1 Then
            Me.dao1 = New Daots_test_table1(Me.dam)
            Me.dao1.D2_Select(dt)
        ElseIf Me.GetStatus() = 2 Then
            Me.dao2 = New Daots_test_table2(Me.dam)
            Me.dao2.D2_Select(dt)
        ElseIf Me.GetStatus() = 3 Then
            Me.dao3 = New Daots_test_table3(Me.dam)
            Me.dao3.D2_Select(dt)
        ElseIf Me.GetStatus() = 4 Then
            Me.daoA = New Daots_test_tableA(Me.dam)
            Me.daoA.D2_Select(dt)
        ElseIf Me.GetStatus() = 5 Then
            Me.daoB = New Daots_test_tableB(Me.dam)
            Me.daoB.D2_Select(dt)
        ElseIf Me.GetStatus() = 6 Then
            Me.daoC = New Daots_test_tableC(Me.dam)
            Me.daoC.D2_Select(dt)
        End If

        Me.dataGridView1.DataSource = dt

    End Sub

    ''' <summary>�N���A</summary>
    Private Sub btnClear_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClear.Click
        Me.dataGridView1.DataSource = Nothing
    End Sub

#End Region

    ''' <summary>�^�C���X�^���v������</summary>
    Private Sub btnClearTS_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClearTS.Click
        Me.txtTS.Text = ""
        Me.ts = Nothing
    End Sub

#Region "Insert"

    ''' <summary>Insert</summary>
    Private Sub btnInsert_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInsert.Click
        ' �}���i�ÓI�j
        ' �Eid �F�I�[�g�C���N�������g�̂��ߕs�v
        ' �Eval�F�K�{
        ' �Ets �F�����X�V�idao�����j�̂��ߕs�v
        If Me.GetStatus() = 1 Then
            Me.dao1 = New Daots_test_table1(Me.dam)

            'this.dao1.PK_id = int.Parse(this.txtID.Text);
            Me.dao1.val = Me.txtVAL.Text
            'this.dao1.ts = this.txtTS.Text;

            Me.dao1.S1_Insert()
        ElseIf Me.GetStatus() = 2 Then
            Me.dao2 = New Daots_test_table2(Me.dam)

            'this.dao2.PK_id = int.Parse(this.txtID.Text);
            Me.dao2.val = Me.txtVAL.Text
            'this.dao2.ts = this.txtTS.Text;

            Me.dao2.S1_Insert()
        ElseIf Me.GetStatus() = 3 Then
            Me.dao3 = New Daots_test_table3(Me.dam)

            'this.dao3.PK_id = int.Parse(this.txtID.Text);
            Me.dao3.val = Me.txtVAL.Text
            'this.dao3.ts = this.txtTS.Text;

            Me.dao3.S1_Insert()
        ElseIf Me.GetStatus() = 4 Then
            Me.daoA = New Daots_test_tableA(Me.dam)

            'this.daoA.PK_id = int.Parse(this.txtID.Text);
            Me.daoA.val = Me.txtVAL.Text
            'this.daoA.ts = this.txtTS.Text;

            Me.daoA.S1_Insert()
        ElseIf Me.GetStatus() = 5 Then
            Me.daoB = New Daots_test_tableB(Me.dam)

            'this.daoB.PK_id = int.Parse(this.txtID.Text);
            Me.daoB.val = Me.txtVAL.Text
            'this.daoB.ts = this.txtTS.Text;

            Me.daoB.S1_Insert()
        ElseIf Me.GetStatus() = 6 Then
            Me.daoC = New Daots_test_tableC(Me.dam)

            'this.daoC.PK_id = int.Parse(this.txtID.Text);
            Me.daoC.val = Me.txtVAL.Text
            'this.daoC.ts = this.txtTS.Text;

            Me.daoC.S1_Insert()
        End If

        ' �X�V
        Me.btnGetAll_Click(sender, e)
    End Sub

#End Region

#Region "Select"

    ''' <summary>Select</summary>
    Private Sub btnSelect_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelect.Click
        Dim id As Integer = 0
        Dim dt As New DataTable()

        If Integer.TryParse(Me.txtID.Text, id) Then
        Else
            MessageBox.Show("ID�̒l���s���ł��B")
            Return
        End If

        ' �Q�Ɓi�ÓI�j
        ' �Eid �F�ÓI
        ' �Eval�F�Ȃ�
        ' �Ets �F���I
        If Me.GetStatus() = 1 Then
            Me.dao1 = New Daots_test_table1(Me.dam)

            Me.dao1.PK_id = id
            'this.dao1.val = this.txtVAL.Text;
            If Me.txtTS.Text <> "" Then
                Me.dao1.ts = Me.ts
            End If

            Me.dao1.S2_Select(dt)
        ElseIf Me.GetStatus() = 2 Then
            Me.dao2 = New Daots_test_table2(Me.dam)

            Me.dao2.PK_id = id
            'this.dao2.val = this.txtVAL.Text;
            If Me.txtTS.Text <> "" Then
                Me.dao2.ts = Me.ts
            End If

            Me.dao2.S2_Select(dt)
        ElseIf Me.GetStatus() = 3 Then
            Me.dao3 = New Daots_test_table3(Me.dam)

            Me.dao3.PK_id = id
            'this.dao3.val = this.txtVAL.Text;
            If Me.txtTS.Text <> "" Then
                Me.dao3.ts = Me.ts
            End If

            Me.dao3.S2_Select(dt)
        ElseIf Me.GetStatus() = 4 Then
            Me.daoA = New Daots_test_tableA(Me.dam)

            Me.daoA.PK_id = id
            'this.daoA.val = this.txtVAL.Text;
            If Me.txtTS.Text <> "" Then
                Me.daoA.ts = Me.ts
            End If

            Me.daoA.S2_Select(dt)
        ElseIf Me.GetStatus() = 5 Then
            Me.daoB = New Daots_test_tableB(Me.dam)

            Me.daoB.PK_id = id
            'this.daoB.val = this.txtVAL.Text;
            If Me.txtTS.Text <> "" Then
                Me.daoB.ts = Me.ts
            End If

            Me.daoB.S2_Select(dt)
        ElseIf Me.GetStatus() = 6 Then
            Me.daoC = New Daots_test_tableC(Me.dam)

            Me.daoC.PK_id = id
            'this.daoC.val = this.txtVAL.Text;
            If Me.txtTS.Text <> "" Then
                Me.daoC.ts = Me.ts
            End If

            Me.daoC.S2_Select(dt)
        End If

        ' �\��
        If dt.Rows.Count <> 0 Then
            Me.txtID.Text = dt.Rows(0)("id").ToString()
            Me.txtVAL.Text = dt.Rows(0)("val").ToString()

            ' �����񉻂̕��@
            If dt.Rows(0)("ts").ToString() = "System.Byte[]" Then
                ' timestamp
                Me.txtTS.Text = BitConverter.ToString(DirectCast(dt.Rows(0)("ts"), Byte()))
            Else
                ' timeticks
                Me.txtTS.Text = dt.Rows(0)("ts").ToString()
            End If

            ' �� ������ �� �o�C�g���Ƃ������̂őޔ����Ă����E�E�E
            Me.ts = dt.Rows(0)("ts")
        Else
            Me.txtID.Text = ""
            Me.txtVAL.Text = ""
            Me.txtTS.Text = ""
            Me.ts = Nothing
        End If
    End Sub

#End Region

#Region "Update"

    ''' <summary>Update</summary>
    Private Sub btnUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdate.Click
        Dim id As Integer = 0

        If Integer.TryParse(Me.txtID.Text, id) Then
        Else
            MessageBox.Show("ID�̒l���s���ł��B")
            Return
        End If

        ' �X�V�i�ÓI�j
        ' �Eid �F���������i�ÓI�j
        ' �Eval�F�X�V�l�E�@�B�I�Ɏw��i�p�����^���O�ɂȂ�̂Łj
        ' �Ets �F���������i���I�j
        If Me.GetStatus() = 1 Then
            Me.dao1 = New Daots_test_table1(Me.dam)

            Me.dao1.PK_id = id
            Me.dao1.Set_val_forUPD = Me.txtVAL.Text
            If Me.txtTS.Text <> "" Then
                Me.dao1.ts = Me.ts
            End If

            Me.dao1.S3_Update()
        ElseIf Me.GetStatus() = 2 Then
            Me.dao2 = New Daots_test_table2(Me.dam)

            Me.dao2.PK_id = id
            Me.dao2.Set_val_forUPD = Me.txtVAL.Text
            If Me.txtTS.Text <> "" Then
                Me.dao2.ts = Me.ts
            End If

            Me.dao2.S3_Update()
        ElseIf Me.GetStatus() = 3 Then
            Me.dao3 = New Daots_test_table3(Me.dam)

            Me.dao3.PK_id = id
            Me.dao3.Set_val_forUPD = Me.txtVAL.Text
            If Me.txtTS.Text <> "" Then
                Me.dao3.ts = Me.ts
            End If

            Me.dao3.S3_Update()
        ElseIf Me.GetStatus() = 4 Then
            Me.daoA = New Daots_test_tableA(Me.dam)

            Me.daoA.PK_id = id
            Me.daoA.Set_val_forUPD = Me.txtVAL.Text
            If Me.txtTS.Text <> "" Then
                Me.daoA.ts = Me.ts
            End If

            Me.daoA.S3_Update()
        ElseIf Me.GetStatus() = 5 Then
            Me.daoB = New Daots_test_tableB(Me.dam)

            Me.daoB.PK_id = id
            Me.daoB.Set_val_forUPD = Me.txtVAL.Text
            If Me.txtTS.Text <> "" Then
                Me.daoB.ts = Me.ts
            End If

            Me.daoB.S3_Update()
        ElseIf Me.GetStatus() = 6 Then
            Me.daoC = New Daots_test_tableC(Me.dam)

            Me.daoC.PK_id = id
            Me.daoC.Set_val_forUPD = Me.txtVAL.Text
            If Me.txtTS.Text <> "" Then
                Me.daoC.ts = Me.ts
            End If

            Me.daoC.S3_Update()
        End If

        ' �X�V
        Me.btnGetAll_Click(sender, e)
    End Sub

#End Region

#Region "Delete"

    ''' <summary>Delete</summary>
    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete.Click
        Dim id As Integer = 0
        Dim dt As New DataTable()

        If Integer.TryParse(Me.txtID.Text, id) Then
        Else
            MessageBox.Show("ID�̒l���s���ł��B")
            Return
        End If

        ' �폜�i�ÓI�j
        ' �Eid �F�ÓI
        ' �Eval�F�Ȃ�
        ' �Ets �F���I
        If Me.GetStatus() = 1 Then
            Me.dao1 = New Daots_test_table1(Me.dam)

            Me.dao1.PK_id = id
            'this.dao1.val = this.txtVAL.Text;
            If Me.txtTS.Text <> "" Then
                Me.dao1.ts = Me.ts
            End If

            Me.dao1.S4_Delete()
        ElseIf Me.GetStatus() = 2 Then
            Me.dao2 = New Daots_test_table2(Me.dam)

            Me.dao2.PK_id = id
            'this.dao2.val = this.txtVAL.Text;
            If Me.txtTS.Text <> "" Then
                Me.dao2.ts = Me.ts
            End If

            Me.dao2.S4_Delete()
        ElseIf Me.GetStatus() = 3 Then
            Me.dao3 = New Daots_test_table3(Me.dam)

            Me.dao3.PK_id = id
            'this.dao3.val = this.txtVAL.Text;
            If Me.txtTS.Text <> "" Then
                Me.dao3.ts = Me.ts
            End If

            Me.dao3.S4_Delete()
        ElseIf Me.GetStatus() = 4 Then
            Me.daoA = New Daots_test_tableA(Me.dam)

            Me.daoA.PK_id = id
            'this.daoA.val = this.txtVAL.Text;
            If Me.txtTS.Text <> "" Then
                Me.daoA.ts = Me.ts
            End If

            Me.daoA.S4_Delete()
        ElseIf Me.GetStatus() = 5 Then
            Me.daoB = New Daots_test_tableB(Me.dam)

            Me.daoB.PK_id = id
            'this.daoB.val = this.txtVAL.Text;
            If Me.txtTS.Text <> "" Then
                Me.daoB.ts = Me.ts
            End If

            Me.daoB.S4_Delete()
        ElseIf Me.GetStatus() = 6 Then
            Me.daoC = New Daots_test_tableC(Me.dam)

            Me.daoC.PK_id = id
            'this.daoC.val = this.txtVAL.Text;
            If Me.txtTS.Text <> "" Then
                Me.daoC.ts = Me.ts
            End If

            Me.daoC.S4_Delete()
        End If

        ' �X�V
        Me.btnGetAll_Click(sender, e)
    End Sub

#End Region

#Region "DynIns"

    ''' <summary>DynIns</summary>
    Private Sub btnDynIns_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDynIns.Click
        ' �}���i���I�j
        ' �Eid �F�I�[�g�C���N�������g�̂��ߕs�v
        ' �Eval�F�@�B�I�Ɏw��i�p�����^���O�ɂȂ�̂Łj
        ' �Ets �F�����X�V�idao�����j�̂��ߕs�v
        If Me.GetStatus() = 1 Then
            Me.dao1 = New Daots_test_table1(Me.dam)

            'this.dao1.PK_id = int.Parse(this.txtID.Text);
            Me.dao1.val = Me.txtVAL.Text
            'this.dao1.ts = this.txtTS.Text;

            Me.dao1.D1_Insert()
        ElseIf Me.GetStatus() = 2 Then
            Me.dao2 = New Daots_test_table2(Me.dam)

            'this.dao2.PK_id = int.Parse(this.txtID.Text);
            Me.dao2.val = Me.txtVAL.Text
            'this.dao2.ts = this.txtTS.Text;

            Me.dao2.D1_Insert()
        ElseIf Me.GetStatus() = 3 Then
            Me.dao3 = New Daots_test_table3(Me.dam)

            'this.dao3.PK_id = int.Parse(this.txtID.Text);
            Me.dao3.val = Me.txtVAL.Text
            'this.dao3.ts = this.txtTS.Text;

            Me.dao3.D1_Insert()
        ElseIf Me.GetStatus() = 4 Then
            Me.daoA = New Daots_test_tableA(Me.dam)

            'this.daoA.PK_id = int.Parse(this.txtID.Text);
            Me.daoA.val = Me.txtVAL.Text
            'this.daoA.ts = this.txtTS.Text;

            Me.daoA.D1_Insert()
        ElseIf Me.GetStatus() = 5 Then
            Me.daoB = New Daots_test_tableB(Me.dam)

            'this.daoB.PK_id = int.Parse(this.txtID.Text);
            Me.daoB.val = Me.txtVAL.Text
            'this.daoB.ts = this.txtTS.Text;

            Me.daoB.D1_Insert()
        ElseIf Me.GetStatus() = 6 Then
            Me.daoC = New Daots_test_tableC(Me.dam)

            'this.daoC.PK_id = int.Parse(this.txtID.Text);
            Me.daoC.val = Me.txtVAL.Text
            'this.daoC.ts = this.txtTS.Text;

            Me.daoC.D1_Insert()
        End If

        ' �X�V
        Me.btnGetAll_Click(sender, e)
    End Sub

#End Region

#Region "DynSel"

    ''' <summary>DynSel</summary>
    Private Sub btnDynSel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDynSel.Click
        Dim id As Integer = 0
        Dim flg As Boolean = False

        Dim dt As New DataTable()

        flg = Integer.TryParse(Me.txtID.Text, id)

        ' �Q�Ɓi���I�j
        ' �Eid �F���I
        ' �Eval�F���I
        ' �Ets �F���I
        If Me.GetStatus() = 1 Then
            Me.dao1 = New Daots_test_table1(Me.dam)

            If flg Then
                Me.dao1.PK_id = id
            End If
            If Me.txtVAL.Text <> "" Then
                Me.dao1.val = Me.txtVAL.Text
            End If
            If Me.txtTS.Text <> "" Then
                Me.dao1.ts = Me.ts
            End If

            Me.dao1.D2_Select(dt)
        ElseIf Me.GetStatus() = 2 Then
            Me.dao2 = New Daots_test_table2(Me.dam)

            If flg Then
                Me.dao2.PK_id = id
            End If
            If Me.txtVAL.Text <> "" Then
                Me.dao2.val = Me.txtVAL.Text
            End If
            If Me.txtTS.Text <> "" Then
                Me.dao2.ts = Me.ts
            End If

            Me.dao2.D2_Select(dt)
        ElseIf Me.GetStatus() = 3 Then
            Me.dao3 = New Daots_test_table3(Me.dam)

            If flg Then
                Me.dao3.PK_id = id
            End If
            If Me.txtVAL.Text <> "" Then
                Me.dao3.val = Me.txtVAL.Text
            End If
            If Me.txtTS.Text <> "" Then
                Me.dao3.ts = Me.ts
            End If

            Me.dao3.D2_Select(dt)
        ElseIf Me.GetStatus() = 4 Then
            Me.daoA = New Daots_test_tableA(Me.dam)

            If flg Then
                Me.daoA.PK_id = id
            End If
            If Me.txtVAL.Text <> "" Then
                Me.daoA.val = Me.txtVAL.Text
            End If
            If Me.txtTS.Text <> "" Then
                Me.daoA.ts = Me.ts
            End If

            Me.daoA.D2_Select(dt)
        ElseIf Me.GetStatus() = 5 Then
            Me.daoB = New Daots_test_tableB(Me.dam)

            If flg Then
                Me.daoB.PK_id = id
            End If
            If Me.txtVAL.Text <> "" Then
                Me.daoB.val = Me.txtVAL.Text
            End If
            If Me.txtTS.Text <> "" Then
                Me.daoB.ts = Me.ts
            End If

            Me.daoB.D2_Select(dt)
        ElseIf Me.GetStatus() = 6 Then
            Me.daoC = New Daots_test_tableC(Me.dam)

            If flg Then
                Me.daoC.PK_id = id
            End If
            If Me.txtVAL.Text <> "" Then
                Me.daoC.val = Me.txtVAL.Text
            End If
            If Me.txtTS.Text <> "" Then
                Me.daoC.ts = Me.ts
            End If

            Me.daoC.D2_Select(dt)
        End If

        Me.dataGridView1.DataSource = dt
    End Sub

#End Region

#Region "DynUpd"

    ''' <summary>DynUpd</summary>
    Private Sub btnDynUpd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDynUpd.Click
        Dim id As Integer = 0

        If Integer.TryParse(Me.txtID.Text, id) Then
        Else
            MessageBox.Show("ID�̒l���s���ł��B")
            Return
        End If

        ' �X�V�i���I�j
        ' �Eid �F���������i�ÓI�j
        ' �Eval�F�X�V�l�E�@�B�I�Ɏw��i�p�����^���O�ɂȂ�̂Łj
        ' �Ets �F���������i���I�j
        If Me.GetStatus() = 1 Then
            Me.dao1 = New Daots_test_table1(Me.dam)

            Me.dao1.PK_id = id
            Me.dao1.Set_val_forUPD = Me.txtVAL.Text
            If Me.txtTS.Text <> "" Then
                Me.dao1.ts = Me.ts
            End If

            Me.dao1.D3_Update()
        ElseIf Me.GetStatus() = 2 Then
            Me.dao2 = New Daots_test_table2(Me.dam)

            Me.dao2.PK_id = id
            Me.dao2.Set_val_forUPD = Me.txtVAL.Text
            If Me.txtTS.Text <> "" Then
                Me.dao2.ts = Me.ts
            End If

            Me.dao2.D3_Update()
        ElseIf Me.GetStatus() = 3 Then
            Me.dao3 = New Daots_test_table3(Me.dam)

            Me.dao3.PK_id = id
            Me.dao3.Set_val_forUPD = Me.txtVAL.Text
            If Me.txtTS.Text <> "" Then
                Me.dao3.ts = Me.ts
            End If

            Me.dao3.D3_Update()
        ElseIf Me.GetStatus() = 4 Then
            Me.daoA = New Daots_test_tableA(Me.dam)

            Me.daoA.PK_id = id
            Me.daoA.Set_val_forUPD = Me.txtVAL.Text
            If Me.txtTS.Text <> "" Then
                Me.daoA.ts = Me.ts
            End If

            Me.daoA.D3_Update()
        ElseIf Me.GetStatus() = 5 Then
            Me.daoB = New Daots_test_tableB(Me.dam)

            Me.daoB.PK_id = id
            Me.daoB.Set_val_forUPD = Me.txtVAL.Text
            If Me.txtTS.Text <> "" Then
                Me.daoB.ts = Me.ts
            End If

            Me.daoB.D3_Update()
        ElseIf Me.GetStatus() = 6 Then
            Me.daoC = New Daots_test_tableC(Me.dam)

            Me.daoC.PK_id = id
            Me.daoC.Set_val_forUPD = Me.txtVAL.Text
            If Me.txtTS.Text <> "" Then
                Me.daoC.ts = Me.ts
            End If

            Me.daoC.D3_Update()
        End If

        ' �X�V
        Me.btnGetAll_Click(sender, e)
    End Sub

#End Region

#Region "DynDel"

    ''' <summary>DynDel</summary>
    Private Sub btnDynDel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDynDel.Click
        Dim id As Integer = 0
        Dim flg As Boolean = False

        flg = Integer.TryParse(Me.txtID.Text, id)

        ' �폜�i���I�j
        ' �Eid �F���I
        ' �Eval�F���I
        ' �Ets �F���I
        If Me.GetStatus() = 1 Then
            Me.dao1 = New Daots_test_table1(Me.dam)

            If flg Then
                Me.dao1.PK_id = id
            End If
            If Me.txtVAL.Text <> "" Then
                Me.dao1.val = Me.txtVAL.Text
            End If
            If Me.txtTS.Text <> "" Then
                Me.dao1.ts = Me.ts
            End If

            Me.dao1.D4_Delete()
        ElseIf Me.GetStatus() = 2 Then
            Me.dao2 = New Daots_test_table2(Me.dam)

            If flg Then
                Me.dao2.PK_id = id
            End If
            If Me.txtVAL.Text <> "" Then
                Me.dao2.val = Me.txtVAL.Text
            End If
            If Me.txtTS.Text <> "" Then
                Me.dao2.ts = Me.ts
            End If

            Me.dao2.D4_Delete()
        ElseIf Me.GetStatus() = 3 Then
            Me.dao3 = New Daots_test_table3(Me.dam)

            If flg Then
                Me.dao3.PK_id = id
            End If
            If Me.txtVAL.Text <> "" Then
                Me.dao3.val = Me.txtVAL.Text
            End If
            If Me.txtTS.Text <> "" Then
                Me.dao3.ts = Me.ts
            End If

            Me.dao3.D4_Delete()
        ElseIf Me.GetStatus() = 4 Then
            Me.daoA = New Daots_test_tableA(Me.dam)

            If flg Then
                Me.daoA.PK_id = id
            End If
            If Me.txtVAL.Text <> "" Then
                Me.daoA.val = Me.txtVAL.Text
            End If
            If Me.txtTS.Text <> "" Then
                Me.daoA.ts = Me.ts
            End If

            Me.daoA.D4_Delete()
        ElseIf Me.GetStatus() = 5 Then
            Me.daoB = New Daots_test_tableB(Me.dam)

            If flg Then
                Me.daoB.PK_id = id
            End If
            If Me.txtVAL.Text <> "" Then
                Me.daoB.val = Me.txtVAL.Text
            End If
            If Me.txtTS.Text <> "" Then
                Me.daoB.ts = Me.ts
            End If

            Me.daoB.D4_Delete()
        ElseIf Me.GetStatus() = 6 Then
            Me.daoC = New Daots_test_tableC(Me.dam)

            If flg Then
                Me.daoC.PK_id = id
            End If
            If Me.txtVAL.Text <> "" Then
                Me.daoC.val = Me.txtVAL.Text
            End If
            If Me.txtTS.Text <> "" Then
                Me.daoC.ts = Me.ts
            End If

            Me.daoC.D4_Delete()
        End If

        ' �X�V
        Me.btnGetAll_Click(sender, e)
    End Sub

#End Region
End Class

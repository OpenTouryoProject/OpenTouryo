'**********************************************************************************
'* サンプル アプリ画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Form1
'* クラス日本語名  ：サンプル アプリ画面
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*
'**********************************************************************************

' Windowアプリケーション
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

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Framework.Transmission

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

''' <summary>サンプル アプリ画面</summary>
Partial Public Class Form1
    Inherits Form
    ' タイムスタンプ オブジェクトの格納
    Private ts As Object

#Region "データアクセス"

    ' データアクセス制御クラス
    Private dam As DamSqlSvr = Nothing

    ' Dao

    ' datetime
    ' 末端
    Private dao1 As Daots_test_table1 = Nothing
    ' 中間
    Private dao2 As Daots_test_table2 = Nothing
    ' 先頭
    Private dao3 As Daots_test_table3 = Nothing

    ' timestamp
    ' 末端
    Private daoA As Daots_test_tableA = Nothing
    ' 中間
    Private daoB As Daots_test_tableB = Nothing
    ' 先頭
    Private daoC As Daots_test_tableC = Nothing

#End Region

#Region "開始-終了処理"

    ''' <summary>コンストラクタ</summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>開始処理</summary>
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' イベントハンドラ
        AddHandler Me.dataGridView1.DataError, AddressOf DataGridView_DataError

        ' ステータス
        Me.cmbTSColType.SelectedIndex = 0
        Me.cmbTableType.SelectedIndex = 0

        dam = New DamSqlSvr()
        dam.Obj = New MyUserInfo("userName", Environment.MachineName)
        Me.dam.ConnectionOpen(GetConfigParameter.GetConnectionString("ConnectionString_SQL"))
    End Sub

    ''' <summary>終了処理</summary>
    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
        Me.dam.ConnectionClose()
    End Sub

    'DataErrorイベントハンドラ
    Private Sub DataGridView_DataError(ByVal sender As Object, ByVal e As DataGridViewDataErrorEventArgs)
        e.Cancel = False
    End Sub

#End Region

#Region "状態の取得処理"

    ''' <summary>状態の取得</summary>
    ''' <returns>状態を表す数値</returns>
    Private Function GetStatus() As Integer
        If Me.cmbTSColType.Text = "RAND（float）列" Then
            If Me.cmbTableType.Text = "TS列末端" Then
                Return 1
            ElseIf Me.cmbTableType.Text = "TS列中間" Then
                Return 2
            ElseIf Me.cmbTableType.Text = "TS列先頭" Then
                Return 3
            End If
        ElseIf Me.cmbTSColType.Text = "timestamp列" Then
            If Me.cmbTableType.Text = "TS列末端" Then
                Return 4
            ElseIf Me.cmbTableType.Text = "TS列中間" Then
                Return 5
            ElseIf Me.cmbTableType.Text = "TS列先頭" Then
                Return 6
            End If
        End If

        Throw New Exception("不明な状態です。")
    End Function

#End Region

#Region "テーブルチェック"

    ''' <summary>全件取得</summary>
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

    ''' <summary>クリア</summary>
    Private Sub btnClear_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClear.Click
        Me.dataGridView1.DataSource = Nothing
    End Sub

#End Region

    ''' <summary>タイムスタンプを消す</summary>
    Private Sub btnClearTS_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClearTS.Click
        Me.txtTS.Text = ""
        Me.ts = Nothing
    End Sub

#Region "Insert"

    ''' <summary>Insert</summary>
    Private Sub btnInsert_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInsert.Click
        ' 挿入（静的）
        ' ・id ：オートインクリメントのため不要
        ' ・val：必須
        ' ・ts ：自動更新（dao同梱）のため不要
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

        ' 更新
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
            MessageBox.Show("IDの値が不正です。")
            Return
        End If

        ' 参照（静的）
        ' ・id ：静的
        ' ・val：なし
        ' ・ts ：動的
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

        ' 表示
        If dt.Rows.Count <> 0 Then
            Me.txtID.Text = dt.Rows(0)("id").ToString()
            Me.txtVAL.Text = dt.Rows(0)("val").ToString()

            ' 文字列化の方法
            If dt.Rows(0)("ts").ToString() = "System.Byte[]" Then
                ' timestamp
                Me.txtTS.Text = BitConverter.ToString(DirectCast(dt.Rows(0)("ts"), Byte()))
            Else
                ' timeticks
                Me.txtTS.Text = dt.Rows(0)("ts").ToString()
            End If

            ' → 文字列化 → バイト化とか解らんので退避しておく・・・
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
            MessageBox.Show("IDの値が不正です。")
            Return
        End If

        ' 更新（静的）
        ' ・id ：検索条件（静的）
        ' ・val：更新値・機械的に指定（パラメタが０個になるので）
        ' ・ts ：検索条件（動的）
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

        ' 更新
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
            MessageBox.Show("IDの値が不正です。")
            Return
        End If

        ' 削除（静的）
        ' ・id ：静的
        ' ・val：なし
        ' ・ts ：動的
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

        ' 更新
        Me.btnGetAll_Click(sender, e)
    End Sub

#End Region

#Region "DynIns"

    ''' <summary>DynIns</summary>
    Private Sub btnDynIns_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDynIns.Click
        ' 挿入（動的）
        ' ・id ：オートインクリメントのため不要
        ' ・val：機械的に指定（パラメタが０個になるので）
        ' ・ts ：自動更新（dao同梱）のため不要
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

        ' 更新
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

        ' 参照（動的）
        ' ・id ：動的
        ' ・val：動的
        ' ・ts ：動的
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
            MessageBox.Show("IDの値が不正です。")
            Return
        End If

        ' 更新（動的）
        ' ・id ：検索条件（静的）
        ' ・val：更新値・機械的に指定（パラメタが０個になるので）
        ' ・ts ：検索条件（動的）
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

        ' 更新
        Me.btnGetAll_Click(sender, e)
    End Sub

#End Region

#Region "DynDel"

    ''' <summary>DynDel</summary>
    Private Sub btnDynDel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDynDel.Click
        Dim id As Integer = 0
        Dim flg As Boolean = False

        flg = Integer.TryParse(Me.txtID.Text, id)

        ' 削除（動的）
        ' ・id ：動的
        ' ・val：動的
        ' ・ts ：動的
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

        ' 更新
        Me.btnGetAll_Click(sender, e)
    End Sub

#End Region
End Class

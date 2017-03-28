'**********************************************************************************
'* カスタム コントロール・サンプル アプリ画面
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：Form1
'* クラス日本語名  ：Form1
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System.Text

Imports Touryo.Infrastructure.CustomControl.RichClient

''' <summary>フォーム</summary>
Partial Public Class Form1
    Inherits Form
    ''' <summary>コンストラクタ</summary>
    Public Sub New()
        InitializeComponent()

        ' 異常な設定状態をテスト

        '' 「HowToCut = null」では、数値と認識されないこと（例外も起きないこと）。
        'Me.winCustomTextBox1.EditDigitsAfterDP = New EditDigitsAfterDP()
        'Me.winCustomTextBox1.EditDigitsAfterDP.HowToCut = Nothing
        'Me.winCustomTextBox1.EditDigitsAfterDP.DigitsAfterDP = 100

    End Sub

    Private Dt As DataTable = Nothing
    Private BindingSource1 As BindingSource = Nothing

    ''' <summary>ロード</summary>
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' ハンドラ
        AddHandler Me.winCustomTextBox8.ValueChanged, AddressOf winCustomTextBox8_ValueChanged

        '/////////////////////////////////////////////////////////////////////////////////////////////////
        ' コンボ初期化
        '/////////////////////////////////////////////////////////////////////////////////////////////////

        ' Webサービスでマスタをロード
        Dim al As ArrayList = Nothing
        CmnMasterDatasForList.ClearMasterData()

        al = New ArrayList()
        al.Add(New ListItem("1", "aaa"))
        al.Add(New ListItem("2", "bbb"))
        al.Add(New ListItem("3", "ccc"))
        CmnMasterDatasForList.SetMasterData("Test1", al)

        al = New ArrayList()
        al.Add(New ListItem("1", "AAA"))
        al.Add(New ListItem("2", "BBB"))
        al.Add(New ListItem("3", "CCC"))
        CmnMasterDatasForList.SetMasterData("Test2", al)

        al = New ArrayList()
        al.Add(New ListItem("1", "あ"))
        al.Add(New ListItem("2", "い"))
        al.Add(New ListItem("3", "う"))
        CmnMasterDatasForList.SetMasterData("Test3", al)

        '/ InitItemsで初期化
        'this.winCustomDropDownList1.InitItems();
        'this.winCustomDropDownList2.InitItems();
        'this.winCustomDropDownList3.InitItems();

        ' InitDataSourceで初期化
        Me.winCustomDropDownList1.InitDataSource()
        Me.winCustomDropDownList2.InitDataSource()
        Me.winCustomDropDownList3.InitDataSource()

        '/////////////////////////////////////////////////////////////////////////////////////////////////
        ' データバインディングをテストする。
        '/////////////////////////////////////////////////////////////////////////////////////////////////

        ' DataBindingsのFormatString（桁区切り）はdecimalで無いと効かない。
        ' また、DataBindingsでは初期設定時のTextのReEditも効かない。
        ' 従って、DataBindings時の方式としてはdecimal＆FormatStringに寄せる必要がある。

        '/////////////////////////////////////////////////////////////////////////////////////////////////
        ' 単項目の入力コントロールとのデータバインディングをテストする。
        '/////////////////////////////////////////////////////////////////////////////////////////////////

        Dim dv As DataView = Nothing
        Me.Dt = Me.CreateDataTable()

        ' FormatStringとは相性が悪いので併用NGとした。

        ' 設定なし
        dv = New DataView(Me.Dt, "id = 1", "", DataViewRowState.Unchanged)
        Me.winCustomTextBox1.DataBindings.Add("Text", dv, "aaa", True, DataSourceUpdateMode.OnPropertyChanged, Nothing)
        ', "#,##0.########");

        ' 桁区切り3
        dv = New DataView(Me.Dt, "id = 2", "", DataViewRowState.Unchanged)
        Me.winCustomTextBox2.DataBindings.Add("Text", dv, "aaa", True, DataSourceUpdateMode.OnPropertyChanged, Nothing)
        ', "#,##0.########");

        ' 桁区切り4
        dv = New DataView(Me.Dt, "id = 3", "", DataViewRowState.Unchanged)
        Me.winCustomTextBox3.DataBindings.Add("Text", dv, "aaa", True, DataSourceUpdateMode.OnPropertyChanged, Nothing)
        ', "#,##0.########");

        ' 小数点以下2, 6
        dv = New DataView(Me.Dt, "id = 4", "", DataViewRowState.Unchanged)
        Me.winCustomTextBox4.DataBindings.Add("Text", dv, "aaa", True, DataSourceUpdateMode.OnPropertyChanged, Nothing)
        ', "#,##0.########");

        ' 小数点以下4, 8
        dv = New DataView(Me.Dt, "id = 5", "", DataViewRowState.Unchanged)
        Me.winCustomTextBox5.DataBindings.Add("Text", dv, "aaa", True, DataSourceUpdateMode.OnPropertyChanged, Nothing)
        ', "#,##0.########");

        ' パッド
        dv = New DataView(Me.Dt, "id = 6", "", DataViewRowState.Unchanged)
        Me.winCustomTextBox6.DataBindings.Add("Text", dv, "aaa", True, DataSourceUpdateMode.OnPropertyChanged, Nothing)
        ', "#,##0.########");

        ' パッド
        dv = New DataView(Me.Dt, "id = 7", "", DataViewRowState.Unchanged)
        Me.winCustomTextBox7.DataBindings.Add("Text", dv, "aaa", True, DataSourceUpdateMode.OnPropertyChanged, Nothing)
        ', "#,##0.########");

        '---

        Me.BindingSource1 = New BindingSource()
        Me.BindingSource1.DataSource = New Bean(88888888, DateTime.Now, "88888888")

        ' 複合（桁区切り3＋小数点以下2、6＋単位変換100万→10^6乗）
        'dv = New DataView(Me.Dt, "id = 8", "", DataViewRowState.Unchanged)
        'Me.winCustomTextBox8.DataBindings.Add("Value", dv, "aaa", True, DataSourceUpdateMode.OnPropertyChanged, Nothing)
        Me.winCustomTextBox8.DataBindings.Add("Value", Me.BindingSource1, "AAA", True, DataSourceUpdateMode.OnPropertyChanged, Nothing)
        ', "#,##0.########");

        ' Textはdatetime、Text2はstringとのバインディングもテスト（日付）
        'dv = New DataView(Me.Dt, "id = 1", "", DataViewRowState.Unchanged)
        'Me.winCustomMaskedTextBox9.DataBindings.Add("Text2", dv, "bbb", True, DataSourceUpdateMode.OnPropertyChanged, Nothing)
        Me.winCustomMaskedTextBox9.DataBindings.Add("Text2", Me.BindingSource1, "BBB", True, DataSourceUpdateMode.OnPropertyChanged, Nothing)
        ', "yyyy/MM/dd");

        '/////////////////////////////////////////////////////////////////////////////////////////////////
        ' データグリッドとのデータバインディングをテストする。
        '/////////////////////////////////////////////////////////////////////////////////////////////////

        '///////////////////////////////////////////////
        ' WinCustomTextBoxの場合
        '///////////////////////////////////////////////
        ' WinCustomTextBoxDgvColを作成
        Dim nomalColumn As New WinCustomTextBoxDgvCol()

        nomalColumn.MaxLength = 15 '10 '20 '15
        nomalColumn.IsNumeric = True

        ' EditInitialValue
        'nomalColumn.EditInitialValue = EditInitialValue.Zero

        ' 編集中、小数点以下（Editingのみ実装）
        nomalColumn.EditDigitsAfterDP_Editing = New EditDigitsAfterDP(CutMethod.Ceiling, 6)

        ' パッド
        'nomalColumn.EditPadding = New EditPadding(PadDirection.Right, "0C")
        'nomalColumn.EditPadding = New EditPadding(PadDirection.Left, "9C")

        ' 桁区切り（FormatStringで対応）
        ' DisplayUnits（処理で対応）

        nomalColumn.DataPropertyName = "aaa"
        nomalColumn.HeaderText = "aaa"

        ' FormatString（編集後、カンマ区切りで小数点2桁）
        nomalColumn.DefaultCellStyle.Format = "#,##0.##"

        Me.dataGridView1.Columns.Add(nomalColumn)

        '///////////////////////////////////////////////
        ' WinCustomMaskedTextBoxの場合
        '///////////////////////////////////////////////

        ' WinCustomMaskedTextBoxDgvColを作成
        Dim maskedColumn As New WinCustomMaskedTextBoxDgvCol()
        maskedColumn.DataPropertyName = "bbb"
        maskedColumn.HeaderText = "bbb"

        ' Maskと、Mask_Editingを逆にすると上手くいかない。
        ' 初期表示時と、編集後で、セル（バインド先）のFormatが変わってしまうため。

        maskedColumn.EditInitialValue = EditInitialValue.Blank

        maskedColumn.Mask = "9999/99/99"
        maskedColumn.Mask_Editing = "9999年99月99日"

        maskedColumn.EditToHankaku = True
        maskedColumn.EditToYYYYMMDD = True

        ' FormatString（編集後、カンマ区切りで小数点2桁）
        maskedColumn.DefaultCellStyle.Format = "yyyy/MM/dd"

        Me.dataGridView1.Columns.Add(maskedColumn)

        '///////////////////////////////////////////////
        ' WinCustomDropDownListの場合
        '///////////////////////////////////////////////

        ' WinCustomDropDownListDgvColを作成
        Dim comboColumn As New DataGridViewComboBoxColumn()
        comboColumn.DataPropertyName = "ccc"
        comboColumn.HeaderText = "ccc"
        comboColumn.DataSource = CmnMasterDatasForList.GetMasterData("Test1")
        ' ↓どちらでも良い
        'MasterDatasForList.GetMasterData("Test1", comboColumn.Items); // ↑どちらでも良い
        comboColumn.ValueMember = "ID"
        ' 必須
        comboColumn.DisplayMember = "Name"
        ' 必須
        Me.dataGridView1.Columns.Add(comboColumn)

        ' ---

        ' また、DataBindingsでは初期設定時のTextのReEditも効かない。
        ' 従って、DataBindings時の方式としてはdecimal＆FormatStringに寄せる必要がある。
        Me.dataGridView1.DataSource = Me.CreateDataTable()
        Me.dataGridView1.Columns("id").Visible = False
        Me.dataGridView1.Columns("ddd").Visible = False ' 変更通知を発生させる用途の列。
    End Sub

    ''' <summary>DataTable生成</summary>
    ''' <returns>DataTable</returns>
    Private Function CreateDataTable() As DataTable
        Dim dt As New DataTable()

        dt.Columns.Add("id", GetType(Integer))
        dt.Columns.Add("aaa", GetType(Decimal))
        dt.Columns.Add("bbb", GetType(DateTime))
        dt.Columns.Add("ccc")
        dt.Columns.Add("ddd")

        Dim dr As DataRow = dt.NewRow()

        dr("id") = "1"
        dr("aaa") = "11111111"
        dr("bbb") = "2001/01/01"
        dr("ccc") = "1"
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr("id") = "2"
        dr("aaa") = "22222222"
        dr("bbb") = "2002/02/02"
        dr("ccc") = "2"
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr("id") = "3"
        dr("aaa") = "33333333"
        dr("bbb") = "2003/03/03"
        dr("ccc") = "3"
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr("id") = "4"
        dr("aaa") = "44444444"
        dr("bbb") = "2004/04/04"
        dr("ccc") = "1"
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr("id") = "5"
        dr("aaa") = "55555555"
        dr("bbb") = "2005/05/05"
        dr("ccc") = "2"
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr("id") = "6"
        dr("aaa") = "66666666"
        dr("bbb") = "2006/06/06"
        dr("ccc") = "3"
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr("id") = "7"
        dr("aaa") = "77777777"
        dr("bbb") = "2007/07/07"
        dr("ccc") = "1"
        dt.Rows.Add(dr)

        dr = dt.NewRow()
        dr("id") = "8"
        dr("aaa") = "88888888"
        dr("bbb") = "2008/08/08"
        dr("ccc") = "1"
        dt.Rows.Add(dr)

        dt.AcceptChanges()

        Return dt
    End Function


    ''' <summary>一括チェックのテスト</summary>
    Private Sub button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button1.Click
        Dim ret As String = ""

        Dim lcr As New List(Of CheckResult)()
        If CmnCheckFunction.HasErrors(Me, lcr) Then
            For Each cr As CheckResult In lcr
                ret += cr.CtrlName + vbCr & vbLf
                For Each checkErrorInfo As String In cr.CheckErrorInfo
                    ret += "・" & checkErrorInfo & vbCr & vbLf
                Next
                ret += vbCr & vbLf
            Next
        End If

        MessageBox.Show(ret)
    End Sub

    ''' <summary>値取得プロパティ プロシージャのテスト（WinCustomTextBox）</summary>
    Private Sub button2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button2.Click
        'winCustomMaskedTextBox1.GetDateTime();
        Dim sb As New StringBuilder()
        sb.AppendLine(Me.GetValue(Me.winCustomTextBox1))
        sb.AppendLine(Me.GetValue(Me.winCustomTextBox2))
        sb.AppendLine(Me.GetValue(Me.winCustomTextBox3))
        sb.AppendLine(Me.GetValue(Me.winCustomTextBox4))
        sb.AppendLine(Me.GetValue(Me.winCustomTextBox5))
        sb.AppendLine(Me.GetValue(Me.winCustomTextBox6))
        sb.AppendLine(Me.GetValue(Me.winCustomTextBox7))
        sb.AppendLine(Me.GetValue(Me.winCustomTextBox8))
        MessageBox.Show(sb.ToString())
    End Sub

    ''' <summary>値取得プロパティ プロシージャのテスト（WinCustomMaskedTextBox）</summary>
    Private Sub button3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button3.Click
        'winCustomMaskedTextBox1.GetDateTime();
        Dim sb As New StringBuilder()
        sb.AppendLine(Me.GetValue(Me.winCustomMaskedTextBox1))
        sb.AppendLine(Me.GetValue(Me.winCustomMaskedTextBox2))
        sb.AppendLine(Me.GetValue(Me.winCustomMaskedTextBox3))
        sb.AppendLine(Me.GetValue(Me.winCustomMaskedTextBox4))
        sb.AppendLine(Me.GetValue(Me.winCustomMaskedTextBox5))
        sb.AppendLine(Me.GetValue(Me.winCustomMaskedTextBox6))
        sb.AppendLine(Me.GetValue(Me.winCustomMaskedTextBox7))
        sb.AppendLine(Me.GetValue(Me.winCustomMaskedTextBox8))
        sb.AppendLine(Me.GetValue(Me.winCustomMaskedTextBox9))
        sb.AppendLine(Me.GetValue(Me.winCustomMaskedTextBox10))
        sb.AppendLine(Me.GetValue(Me.winCustomMaskedTextBox11))
        sb.AppendLine(Me.GetValue(Me.winCustomMaskedTextBox12))
        sb.AppendLine(Me.GetValue(Me.winCustomMaskedTextBox13))
        MessageBox.Show(sb.ToString())
    End Sub

    ''' <summary>データソースからの変更通知を発生</summary>
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button4.Click
        Dim dt As DataTable = Me.Dt
        For Each row As DataRow In dt.Rows
            row("ddd") = Me.textBox1.Text
        Next
        dt.AcceptChanges()

        dt = DirectCast(Me.dataGridView1.DataSource, DataTable)
        For Each row As DataRow In dt.Rows
            row("ddd") = Me.textBox1.Text
        Next
        dt.AcceptChanges()

        DirectCast(Me.BindingSource1.DataSource, Bean).AAA = 99999999
        DirectCast(Me.BindingSource1.DataSource, Bean).BBB = DateTime.Now
        DirectCast(Me.BindingSource1.DataSource, Bean).CCC = "99999999"
        Me.BindingSource1.ResetBindings(False)
    End Sub

    ''' <summary>値取得プロパティ プロシージャのテスト</summary>
    Private Function GetValue(ByVal igv As IGetValue) As String
        Dim sb As New StringBuilder()

        sb.AppendLine(DirectCast(igv, Control).Name)
        Try
            sb.AppendLine("GetInt16:" & igv.GetInt16().ToString())
        Catch
        End Try
        Try
            sb.AppendLine("GetInt32:" & igv.GetInt32().ToString())
        Catch
        End Try
        Try
            sb.AppendLine("GetInt64:" & igv.GetInt64().ToString())
        Catch
        End Try
        Try
            sb.AppendLine("GetFloat:" & igv.GetFloat().ToString())
        Catch
        End Try
        Try
            sb.AppendLine("GetDouble:" & igv.GetDouble().ToString())
        Catch
        End Try
        Try
            sb.AppendLine("GetDecimal:" & igv.GetDecimal().ToString())
        Catch
        End Try
        Try
            sb.AppendLine("GetDateTime:" & igv.GetDateTime().ToString())
        Catch
        End Try

        If TypeOf igv Is WinCustomTextBox Then
            Dim wctbx As WinCustomTextBox = DirectCast(igv, WinCustomTextBox)
            ' 通常のTextプロパティ（可変）
            sb.AppendLine("Text:" + wctbx.Text)
            ' ユーザの入力値だけ取得する
            sb.AppendLine("Text2:" + wctbx.Text2)
            ' 編集処理を適用した値を取得する
            sb.AppendLine("Text3:" + wctbx.Text3)
            ' データバインディング用プロパティ値を取得する
            sb.AppendLine("Value:" + wctbx.Value)
        ElseIf TypeOf igv Is WinCustomMaskedTextBox Then
            Dim wcmtbx As WinCustomMaskedTextBox = DirectCast(igv, WinCustomMaskedTextBox)
            ' 通常のTextプロパティ（可変）
            sb.AppendLine("Text:" + wcmtbx.Text)
            ' ユーザの入力値だけ取得する
            sb.AppendLine("Text2:" + wcmtbx.Text2)
            ' 入力時マスクを適用した値を取得する
            sb.AppendLine("Text3:" + wcmtbx.Text3)
        End If

        Return sb.ToString()
    End Function

    Private Sub winCustomTextBox_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles winCustomTextBox7.TextChanged, winCustomTextBox6.TextChanged, winCustomTextBox5.TextChanged, winCustomTextBox4.TextChanged, winCustomTextBox3.TextChanged, winCustomTextBox2.TextChanged
        Dim s As String = DirectCast(sender, TextBox).Name

        If (s.Length = 0) Then
            Return
        End If

        Select Case s.Substring(s.Length - 1, 1)
            Case "2"
                Me.winCustomTextBox2_2.Text = Me.winCustomTextBox2.Text2
                Exit Select
            Case "3"
                Me.winCustomTextBox3_2.Text = Me.winCustomTextBox3.Text2
                Exit Select
            Case "4"
                Me.winCustomTextBox4_2.Text = Me.winCustomTextBox4.Text2
                Exit Select
            Case "5"
                Me.winCustomTextBox5_2.Text = Me.winCustomTextBox5.Text2
                Exit Select
            Case "6"
                Me.winCustomTextBox6_2.Text = Me.winCustomTextBox6.Text2
                Exit Select
            Case "7"
                Me.winCustomTextBox7_2.Text = Me.winCustomTextBox7.Text2
                Exit Select
            Case Else
                Exit Select
        End Select
    End Sub

    Private Sub winCustomMaskedTextBox_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles winCustomMaskedTextBox9.TextChanged, winCustomMaskedTextBox8.TextChanged, winCustomMaskedTextBox7.TextChanged, winCustomMaskedTextBox6.TextChanged, winCustomMaskedTextBox5.TextChanged, winCustomMaskedTextBox4.TextChanged, winCustomMaskedTextBox3.TextChanged, winCustomMaskedTextBox2.TextChanged, winCustomMaskedTextBox11.TextChanged, winCustomMaskedTextBox10.TextChanged
        Dim s As String = DirectCast(sender, MaskedTextBox).Name

        If (s.Length = 0) Then
            Return
        End If

        Select Case s.Substring(s.Length - 1, 1)
            Case "2"
                Me.winCustomMaskedTextBox2_2.Text = Me.winCustomMaskedTextBox2.Text2
                Exit Select
            Case "3"
                Me.winCustomMaskedTextBox3_2.Text = Me.winCustomMaskedTextBox3.Text2
                Exit Select
            Case "4"
                Me.winCustomMaskedTextBox4_2.Text = Me.winCustomMaskedTextBox4.Text2
                Exit Select
            Case "5"
                Me.winCustomMaskedTextBox5_2.Text = Me.winCustomMaskedTextBox5.Text2
                Exit Select
            Case "6"
                Me.winCustomMaskedTextBox6_2.Text = Me.winCustomMaskedTextBox6.Text2
                Exit Select
            Case "7"
                Me.winCustomMaskedTextBox7_2.Text = Me.winCustomMaskedTextBox7.Text2
                Exit Select
            Case "8"
                Me.winCustomMaskedTextBox8_2.Text = Me.winCustomMaskedTextBox8.Text2
                Exit Select
            Case "9"
                Me.winCustomMaskedTextBox9_2.Text = Me.winCustomMaskedTextBox9.Text2
                Exit Select
            Case "10"
                Me.winCustomMaskedTextBox10_2.Text = Me.winCustomMaskedTextBox10.Text2
                Exit Select
            Case "11"
                Me.winCustomMaskedTextBox11_2.Text = Me.winCustomMaskedTextBox11.Text2
                Exit Select
            Case Else
                Exit Select
        End Select
    End Sub

    ''' <summary>デザイナで設定できなくした</summary>
    Private Sub winCustomTextBox8_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles winCustomTextBox8.ValueChanged
        System.Diagnostics.Debug.WriteLine("ValueChanged:" + DirectCast(sender, WinCustomTextBox).Name)
    End Sub

End Class

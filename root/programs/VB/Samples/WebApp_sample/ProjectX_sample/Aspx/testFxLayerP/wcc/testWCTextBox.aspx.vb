'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testWCTextBox
'* クラス日本語名  ：Web Custom Control部品テスト画面
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports Touryo.Infrastructure.CustomControl

Namespace Aspx.TestFxLayerP.Wcc
    ''' <summary>testWCTextBox class</summary>
    Partial Public Class testWCTextBox
        Inherits System.Web.UI.Page
#Region "初期処理"


        ''' <summary>ヘッダーに表示する文字列</summary>
        Public HeaderInfo As New Dictionary(Of String, String)()

        Protected Sub Page_Load(sender As Object, e As EventArgs)
            ' 初回Load時に、データソースを
            ' 生成 ＆ データバインドする。
            Me.gvwGridView1.Columns(0).HeaderText = "ID"
            Me.gvwGridView1.Columns(1).HeaderText = "チェック"
            Me.gvwGridView1.Columns(2).HeaderText = "必須入力"
            Me.gvwGridView1.Columns(3).HeaderText = "半角"
            Me.gvwGridView1.Columns(4).HeaderText = "全角"
            Me.gvwGridView1.Columns(5).HeaderText = "数値"
            Me.gvwGridView1.Columns(6).HeaderText = "片仮名"
            Me.gvwGridView1.Columns(7).HeaderText = "半角片仮名"
            Me.gvwGridView1.Columns(8).HeaderText = "平仮名"
            Me.gvwGridView1.Columns(9).HeaderText = "日付"
            Me.gvwGridView1.Columns(10).HeaderText = "正規表現（メアド）"
            Me.gvwGridView1.Columns(11).HeaderText = "禁則"
            Me.gvwGridView1.Columns(12).HeaderText = "半角＆禁則"
            Me.gvwGridView1.Columns(13).HeaderText = "全角＆数値"

            If Not Me.IsPostBack Then
                Me.CreateDataSource()
                Me.BindGridData()
            End If

            Me.TextBox1.Text = ""
        End Sub


        ''' <summary>DataSourceを生成</summary>
        Private Sub CreateDataSource()
            Dim dt As New DataTable()
            Dim dr As DataRow

            ' 列生成
            dt.Columns.Add(New DataColumn("fileid", GetType(Integer)))
            dt.Columns.Add(New DataColumn("field1", GetType(String)))
            dt.Columns.Add(New DataColumn("field2", GetType(String)))
            dt.Columns.Add(New DataColumn("field3", GetType(String)))
            dt.Columns.Add(New DataColumn("field4", GetType(String)))
            dt.Columns.Add(New DataColumn("field5", GetType(String)))
            dt.Columns.Add(New DataColumn("field6", GetType(String)))
            dt.Columns.Add(New DataColumn("field7", GetType(String)))
            dt.Columns.Add(New DataColumn("field8", GetType(String)))
            dt.Columns.Add(New DataColumn("field9", GetType(String)))
            dt.Columns.Add(New DataColumn("field10", GetType(String)))
            dt.Columns.Add(New DataColumn("field11", GetType(String)))
            dt.Columns.Add(New DataColumn("field12", GetType(String)))
            dt.Columns.Add(New DataColumn("field13", GetType(String)))

            ' 行生成
            For i As Integer = 1 To 9
                dr = dt.NewRow()
                dr("fileid") = i
                dt.Rows.Add(dr)
            Next

            ' 変更のコミット
            dt.AcceptChanges()

            ' DataTableをSessionに格納する
            Session("SampleData") = dt
        End Sub

        ''' <summary>データバインドする</summary>
        Private Sub BindGridData()
            Me.gvwGridView1.DataSource = Session("SampleData")
            Me.gvwGridView1.DataBind()
        End Sub

#End Region

        ''' <summary>バッチ・チェック処理</summary>
        Protected Sub btnCheckText_Click(sender As Object, e As EventArgs)
            ' 一括チェック処理
            Dim rets As New List(Of CheckResult)()
            If CmnCheckFunction.HasErrors(Me, rets) Then
                For Each ret As CheckResult In rets
                    Me.TextBox1.Text += ret.CtrlName + vbCr & vbLf
                    For Each s As String In ret.CheckErrorInfo
                        Me.TextBox1.Text += "・" & s & vbCr & vbLf
                    Next
                Next
            End If
        End Sub
    End Class
End Namespace

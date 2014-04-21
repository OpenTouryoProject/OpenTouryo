'**********************************************************************************
'* フレームワーク・テスト画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Aspx_testFxLayerP_table_testGridView
'* クラス日本語名  ：GridViewテスト画面（Ｐ層）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

' System
Imports System
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic

' System.Web
Imports System.Web
Imports System.Web.Security

Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Business.Exceptions
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.CustomControl

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

''' <summary>GridViewテスト画面（Ｐ層）</summary>
Public Partial Class Aspx_testFxLayerP_table_testGridView
	Inherits MyBaseController
	#Region "初期化"

	''' <summary>ヘッダーに表示する文字列</summary>
	Public HeaderInfo As New Dictionary(Of String, String)()

	''' <summary>Page_InitイベントでASP.NET標準イベントハンドラを設定</summary>
	Protected Sub Page_Init(sender As Object, e As EventArgs)
		' 行編集についてのイベント
        AddHandler Me.gvwGridView1.RowCreated, AddressOf gvwGridView1_RowCreated
        AddHandler Me.gvwGridView1.RowEditing, AddressOf gvwGridView1_RowEditing
        AddHandler Me.gvwGridView1.RowCancelingEdit, AddressOf gvwGridView1_RowCancelingEdit
        AddHandler Me.gvwGridView1.RowUpdated, AddressOf gvwGridView1_RowUpdated
        AddHandler Me.gvwGridView1.RowDeleted, AddressOf gvwGridView1_RowDeleted

		' 行選択についてのイベント
        AddHandler Me.gvwGridView1.SelectedIndexChanging, AddressOf gvwGridView1_SelectedIndexChanging

		' 列ソートについてのイベント
        AddHandler Me.gvwGridView1.Sorted, AddressOf gvwGridView1_Sorted
	End Sub

	''' <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
	''' <remarks>実装必須</remarks>
	Protected Overrides Sub UOC_FormInit()
		' フォーム初期化（初回ロード）時に実行する処理を実装する
		' TODO:
		Me.CmnInit()

		' 初回ロード時に、データソースを
		' 生成 ＆ データバインドする。
		Me.CreateDataSource()
		Me.gvwGridView1.Columns(0).HeaderText = Me.HeaderInfo("col0")
		Me.gvwGridView1.Columns(1).HeaderText = Me.HeaderInfo("col1")
		Me.gvwGridView1.Columns(2).HeaderText = Me.HeaderInfo("col2")
		Me.gvwGridView1.Columns(3).HeaderText = Me.HeaderInfo("col3")
		Me.gvwGridView1.Columns(4).HeaderText = Me.HeaderInfo("col4")
		Me.gvwGridView1.Columns(5).HeaderText = Me.HeaderInfo("col5")
		Me.gvwGridView1.Columns(6).HeaderText = Me.HeaderInfo("col6")
		Me.gvwGridView1.Columns(7).HeaderText = Me.HeaderInfo("col7")
		Me.gvwGridView1.Columns(8).HeaderText = Me.HeaderInfo("col8")
		Me.gvwGridView1.Columns(9).HeaderText = Me.HeaderInfo("col9")
		Me.gvwGridView1.Columns(10).HeaderText = Me.HeaderInfo("col10")

		Me.BindGridData()
	End Sub

	''' <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
	''' <remarks>実装必須</remarks>
	Protected Overrides Sub UOC_FormInit_PostBack()
		' フォーム初期化（ポストバック）時に実行する処理を実装する
		' TODO:
		Me.CmnInit()

		' Radio Buttonの選択状態を出力
		If Request.Form("radio-grp1") IsNot Nothing Then
			Response.Write(String.Format("name=""radio-grp1"" value=""{0}""が選択されました。<br/>", Request.Form("radio-grp1").ToString()))
		End If

		Dim i As Integer = 0
		For Each gvr As GridViewRow In Me.gvwGridView1.Rows
			i += 1
			Dim rbn As WebCustomRadioButton = DirectCast(gvr.FindControl("rbnRadioButton"), WebCustomRadioButton)

			' チェック
					' == null
			If rbn Is Nothing Then
			Else
				' != null
				If rbn.Checked Then
					Response.Write(String.Format("name=""radio-grp1"" value=""{0}""行目が選択されました。<br/>", i.ToString()))
				End If
			End If
		Next
	End Sub

	Private Sub CmnInit()
		' ヘッダーに表示する文字列を初期化
		Me.HeaderInfo.Add("col0", "select1")
		Me.HeaderInfo.Add("col1", "select2")
		Me.HeaderInfo.Add("col2", "custom")
		Me.HeaderInfo.Add("col3", "fileid")
		Me.HeaderInfo.Add("col4", "readonly")
		Me.HeaderInfo.Add("col5", "filename")
		Me.HeaderInfo.Add("col6", "filesize")
		Me.HeaderInfo.Add("col7", "date")
		Me.HeaderInfo.Add("col8", "edit1")
		Me.HeaderInfo.Add("col9", "edit2")
		Me.HeaderInfo.Add("col10", "dropdown")
	End Sub

	#End Region

	#Region "データソースの生成"

	''' <summary>DataSourceを生成</summary>
	Private Sub CreateDataSource()
		' Server.MapPathはアプリケーション ディレクトリを指す。
		Dim di As New DirectoryInfo(Server.MapPath("/ProjectX_sample/Aspx/Common"))
		Dim fi As FileInfo() = di.GetFiles()

		' Datatableに
		' アプリケーション ディレクトリの
		' ファイル情報を設定する。
		Dim dt As New DataTable()
		Dim dr As DataRow

		' 列生成
		dt.Columns.Add(New DataColumn("fileid", GetType(Integer)))
		dt.Columns.Add(New DataColumn("filename", GetType([String])))
		dt.Columns.Add(New DataColumn("readonly", GetType([Boolean])))
		dt.Columns.Add(New DataColumn("filesize", GetType(Long)))
		dt.Columns.Add(New DataColumn("date", GetType(DateTime)))

		' 行生成
		For i As Integer = 0 To fi.Length - 1
			dr = dt.NewRow()
			dr("fileid") = i
			dr("filename") = fi(i).Name
			dr("readonly") = fi(i).IsReadOnly
			dr("filesize") = fi(i).Length
			dr("date") = fi(i).LastWriteTime
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

	#Region "イベントハンドラ"

	#Region "通常のイベント"

	''' <summary>btnButton1のクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
		' ポストバックをまたいで値が保存されるかの確認
		Return ""
	End Function

	#End Region

	#Region "GridViewのイベント"

	#Region "標準イベント"

	''' <summary>RowCreatedのテスト</summary>
	Protected Sub gvwGridView1_RowCreated(sender As Object, e As GridViewRowEventArgs)
	End Sub

	#End Region

	#Region "Command"

	''' <summary>gvwGridView1のコマンドイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_gvwGridView1_RowCommand(fxEventArgs As FxEventArgs) As String
		' 選択されたコマンド名を取得する
		' fxEventArgs.InnerButtonID
		'   Select : 選択
		'   Edit   : 編集
		'   Update : 更新
		'   Cancel : キャンセル
		'   Delete : 削除
		'   Page   : ページ切り替え
		'   Sort   : ソート
		'   その他カスタム コマンド

		System.Diagnostics.Debug.WriteLine("--------------------")
		System.Diagnostics.Debug.WriteLine("Event:RowCommand")
		System.Diagnostics.Debug.WriteLine("ButtonID:" & Convert.ToString(fxEventArgs.ButtonID))
		System.Diagnostics.Debug.WriteLine("InnerButtonID:" & Convert.ToString(fxEventArgs.InnerButtonID))
		System.Diagnostics.Debug.WriteLine("PostBackValue:" & Convert.ToString(fxEventArgs.PostBackValue))

		Return ""
	End Function

	#End Region

	#Region "選択"

	''' <summary>GridViewの行の選択ボタンがクリックされ、行が選択される前に発生するイベント</summary>
	Protected Sub gvwGridView1_SelectedIndexChanging(sender As Object, e As GridViewSelectEventArgs)
		' ここでは何もしない
	End Sub

	''' <summary>gvwGridView1の行選択後イベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_gvwGridView1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
		' ここでは何もしない

		System.Diagnostics.Debug.WriteLine("--------------------")
		System.Diagnostics.Debug.WriteLine("Event:SelectedIndexChanged")
		System.Diagnostics.Debug.WriteLine("ButtonID:" & Convert.ToString(fxEventArgs.ButtonID))
		System.Diagnostics.Debug.WriteLine("InnerButtonID:" & Convert.ToString(fxEventArgs.InnerButtonID))
		System.Diagnostics.Debug.WriteLine("PostBackValue:" & Convert.ToString(fxEventArgs.PostBackValue))

		Return ""
	End Function

	#End Region

	#Region "編集"

	' Updating、Deletingのみ棟梁でハンドル。

	''' <summary>GridViewの行の編集ボタンがクリックされ、編集モードになる前に発生するイベント</summary>
	Protected Sub gvwGridView1_RowEditing(sender As Object, e As GridViewEditEventArgs)
		' GridViewを編集モードにする
		Me.gvwGridView1.EditIndex = e.NewEditIndex
		Me.BindGridData()
	End Sub

	''' <summary>編集モードの行のキャンセルボタンがクリックされ、編集モードが終了する前に発生するイベント</summary>
	Protected Sub gvwGridView1_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs)
		' GridViewを編集モードから解除する
		Me.gvwGridView1.EditIndex = -1
		Me.BindGridData()
	End Sub

	''' <summary>gvwGridView1の行更新前イベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <param name="e">オリジナルのイベント引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_gvwGridView1_RowUpdating(fxEventArgs As FxEventArgs, e As GridViewUpdateEventArgs) As String
		' 編集行のコントロールを取得する
		Dim gvRow As GridViewRow = Me.gvwGridView1.Rows(e.RowIndex)

		Dim txt1 As TextBox = DirectCast(gvRow.FindControl("TextBox1"), TextBox)
		Dim txt2 As TextBox = DirectCast(gvRow.FindControl("TextBox2"), TextBox)
		Dim cbx3 As CheckBox = DirectCast(gvRow.FindControl("cbxCheckBox3"), CheckBox)
		Dim txt4 As TextBox = DirectCast(gvRow.FindControl("TextBox4"), TextBox)
		Dim txt5 As TextBox = DirectCast(gvRow.FindControl("TextBox5"), TextBox)

		' 編集後の値に書き換える
		Dim fileid As Integer = CInt(Me.gvwGridView1.DataKeys(e.RowIndex).Value)
		Dim dt As DataTable = DirectCast(Session("SampleData"), DataTable)
		Dim row As DataRow = dt.[Select](String.Format("fileid = '{0}'", fileid))(0)
		row("fileid") = txt1.Text
		row("filename") = txt2.Text
		row("readonly") = cbx3.Checked
		row("filesize") = txt4.Text
		row("date") = txt5.Text

		' GridViewを編集モードから解除する
		Me.gvwGridView1.EditIndex = -1
		Me.BindGridData()

		Return ""
	End Function

	''' <summary>GridViewの行の更新ボタンがクリックされ、行が更新された後に発生するイベント</summary>
	Protected Sub gvwGridView1_RowUpdated(sender As Object, e As GridViewUpdatedEventArgs)
		' ここでは何もしない
	End Sub

	''' <summary>gvwGridView1の行削除前イベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <param name="e">オリジナルのイベント引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_gvwGridView1_RowDeleting(fxEventArgs As FxEventArgs, e As GridViewDeleteEventArgs) As String
		' 選択された行を削除する
		Dim dt As DataTable = DirectCast(Session("SampleData"), DataTable)
		Dim fileid As Integer = CInt(Me.gvwGridView1.DataKeys(e.RowIndex).Value)
		dt.[Select](String.Format("fileid = '{0}'", fileid))(0).Delete()

		' GridViewを編集モードから解除する
		Me.gvwGridView1.EditIndex = -1
		Me.BindGridData()

		Return ""
	End Function

	''' <summary>GridViewの行の削除ボタンがクリックされ、行が削除された後に発生するイベント</summary>
	Protected Sub gvwGridView1_RowDeleted(sender As Object, e As GridViewDeletedEventArgs)
		' ここでは何もしない
	End Sub

	#End Region

	#Region "ページング・ソート"

	' PageIndexChanging、Sortingのみ棟梁でハンドル。

	''' <summary>gvwGridView1のPageIndexChangingイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <param name="e">オリジナルのイベント引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_gvwGridView1_PageIndexChanging(fxEventArgs As FxEventArgs, e As GridViewPageEventArgs) As String
		Me.gvwGridView1.PageIndex = e.NewPageIndex
		Me.BindGridData()

		Return ""
	End Function

	''' <summary>ページが切り替わったときに発生するイベント</summary>
	Protected Sub gvwGridView1_PageIndexChanged(sender As Object, e As EventArgs)
		' ここでは何もしない
	End Sub

	''' <summary>gvwGridView1のSortingイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <param name="e">オリジナルのイベント引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_gvwGridView1_Sorting(fxEventArgs As FxEventArgs, e As GridViewSortEventArgs) As String
		' 元のデータ
		Dim dt1 As DataTable = DirectCast(Session("SampleData"), DataTable)

		' ソート後のデータを格納するためのDataTable
		Dim dt2 As DataTable = dt1.Clone()

		' データソート用のDataView
		Dim dv As New DataView(dt1)

		If Session("SortDirection") Is Nothing Then
			' ソートの定義情報を格納するためのDictionaryがない場合は作成する
			Session("SortDirection") = New Dictionary(Of String, SortDirection)()
		End If

		' ソート定義情報にしたがい、データをソートする
		If Not DirectCast(Session("SortDirection"), Dictionary(Of String, SortDirection)).ContainsKey(e.SortExpression) Then
			' ソート定義情報がない場合。デフォルトは昇順とする
			dv.Sort = e.SortExpression

			' ソート定義情報を追加する
			DirectCast(Session("SortDirection"), Dictionary(Of String, SortDirection)).Add(e.SortExpression, SortDirection.Descending)
		Else
			' ソート定義情報をもとに、当該列のソート方向を取得する
			Dim direction As SortDirection = DirectCast(Session("SortDirection"), Dictionary(Of String, SortDirection))(e.SortExpression)

			If direction = SortDirection.Ascending Then
				' 昇順
				dv.Sort = e.SortExpression

				' ソート定義情報を更新する
				DirectCast(Session("SortDirection"), Dictionary(Of String, SortDirection))(e.SortExpression) = SortDirection.Descending
			Else
				' 降順
				dv.Sort = Convert.ToString(e.SortExpression) & " DESC"

				' ソート定義情報を更新する
				DirectCast(Session("SortDirection"), Dictionary(Of String, SortDirection))(e.SortExpression) = SortDirection.Ascending
			End If
		End If

		' ソート後のデータをDataTableにインポートする
		For Each drv As DataRowView In dv
			dt2.ImportRow(drv.Row)
		Next

		' データの再バインド
		Session("SampleData") = dt2
		Me.BindGridData()

		Return ""
	End Function

	''' <summary>GridViewの列ヘッダーがクリックされ、行がソートされた後に発生するイベント</summary>
	Protected Sub gvwGridView1_Sorted(sender As Object, e As EventArgs)
		' ここでは何もしない
	End Sub

	#End Region

	#Region "GridView内のCommand、Click以外のイベント"

	' GridViewのイベントに行かないので通常通りハンドルする。
	' （各コントロールのAutoPostBackを"true"に設定する）

	''' <summary>cbxCheckBox3のCheckedChangedイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_cbxCheckBox3_CheckedChanged(fxEventArgs As FxEventArgs) As String
		System.Diagnostics.Debug.WriteLine("--------------------")
		System.Diagnostics.Debug.WriteLine("ButtonID:" & Convert.ToString(fxEventArgs.ButtonID))
		System.Diagnostics.Debug.WriteLine("InnerButtonID:" & Convert.ToString(fxEventArgs.InnerButtonID))
		System.Diagnostics.Debug.WriteLine("PostBackValue:" & Convert.ToString(fxEventArgs.PostBackValue))

		Dim cbx As CheckBox = DirectCast(Me.gvwGridView1.Rows(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("cbxCheckBox3"), CheckBox)

		System.Diagnostics.Debug.WriteLine(cbx.Checked.ToString())

		Return ""
	End Function

	''' <summary>rbnRadioButton3のCheckedChangedイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_rbnRadioButton3_CheckedChanged(fxEventArgs As FxEventArgs) As String
		System.Diagnostics.Debug.WriteLine("--------------------")
		System.Diagnostics.Debug.WriteLine("ButtonID:" & Convert.ToString(fxEventArgs.ButtonID))
		System.Diagnostics.Debug.WriteLine("InnerButtonID:" & Convert.ToString(fxEventArgs.InnerButtonID))
		System.Diagnostics.Debug.WriteLine("PostBackValue:" & Convert.ToString(fxEventArgs.PostBackValue))

		Dim cbx As RadioButton = DirectCast(Me.gvwGridView1.Rows(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("rbnRadioButton3"), RadioButton)

		System.Diagnostics.Debug.WriteLine(cbx.Checked.ToString())

		Return ""
	End Function

	''' <summary>ddlDropDownList1のSelectedIndexChangedイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_ddlDropDownList1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
		System.Diagnostics.Debug.WriteLine("--------------------")
		System.Diagnostics.Debug.WriteLine("ButtonID:" & Convert.ToString(fxEventArgs.ButtonID))
		System.Diagnostics.Debug.WriteLine("InnerButtonID:" & Convert.ToString(fxEventArgs.InnerButtonID))
		System.Diagnostics.Debug.WriteLine("PostBackValue:" & Convert.ToString(fxEventArgs.PostBackValue))

		Dim ddl As DropDownList = DirectCast(Me.gvwGridView1.Rows(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("ddlDropDownList1"), DropDownList)

		System.Diagnostics.Debug.WriteLine(ddl.SelectedValue)

		Return ""
	End Function

	''' <summary>lbxListBox1のSelectedIndexChangedイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_lbxListBox1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
		System.Diagnostics.Debug.WriteLine("--------------------")
		System.Diagnostics.Debug.WriteLine("ButtonID:" & Convert.ToString(fxEventArgs.ButtonID))
		System.Diagnostics.Debug.WriteLine("InnerButtonID:" & Convert.ToString(fxEventArgs.InnerButtonID))
		System.Diagnostics.Debug.WriteLine("PostBackValue:" & Convert.ToString(fxEventArgs.PostBackValue))

		Dim ddl As ListBox = DirectCast(Me.gvwGridView1.Rows(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("lbxListBox1"), ListBox)

		System.Diagnostics.Debug.WriteLine(ddl.SelectedValue)

		Return ""
	End Function

	#End Region

	#End Region

	#End Region
End Class

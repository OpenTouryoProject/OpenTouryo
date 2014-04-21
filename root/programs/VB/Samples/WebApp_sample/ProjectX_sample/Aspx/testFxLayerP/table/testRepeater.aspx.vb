'**********************************************************************************
'* フレームワーク・テスト画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：Aspx_testFxLayerP_table_testRepeater
'* クラス日本語名  ：Repeaterテスト画面（Ｐ層）
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

Imports System.Text.RegularExpressions

''' <summary>Repeaterテスト画面（Ｐ層）</summary>
Public Partial Class Aspx_testFxLayerP_table_testRepeater
	Inherits MyBaseController
	#Region "初期化"

	''' <summary>ヘッダーに表示する文字列</summary>
	Public HeaderInfo As New Dictionary(Of String, String)()

	''' <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
	''' <remarks>実装必須</remarks>
	Protected Overrides Sub UOC_FormInit()
		' フォーム初期化（初回ロード）時に実行する処理を実装する
		' TODO:
		Me.CmnInit()

		Dim dt As DataTable = Nothing

		' DropDownListのデータソースを初期化
		dt = Me.CreateDataSource2()
		Me.DropDownListDataSource = dt

		' データバインド
		dt = Me.CreateDataSource1()
		Me.RepeaterDataSource = dt
		Me.rptRepeater1.DataSource = dt
		Me.rptRepeater1.DataBind()
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
		For Each ri As RepeaterItem In Me.rptRepeater1.Items
			i += 1
			Dim rbn As WebCustomRadioButton = DirectCast(ri.FindControl("rbnRadioButton"), WebCustomRadioButton)

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
		Me.HeaderInfo.Add("col0", "select")
		Me.HeaderInfo.Add("col1", "fileid")
		Me.HeaderInfo.Add("col2", "textbox<br/>filename")
		Me.HeaderInfo.Add("col3", "checkbox<br/>（IsReadOnly）")
		Me.HeaderInfo.Add("col4", "dropdownlist")
	End Sub

	#End Region

	#Region "データソースの生成"

	''' <summary>DataSourceを生成</summary>
	''' <returns>Datatableを返す</returns>
	''' <remarks>repeater1用</remarks>
	Private Function CreateDataSource1() As DataTable
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
		dt.Columns.Add(New DataColumn("textbox", GetType([String])))
		dt.Columns.Add(New DataColumn("checkbox", GetType(Boolean)))
		dt.Columns.Add(New DataColumn("dropdownlist", GetType(Integer)))

		' 行生成
		For i As Integer = 0 To fi.Length - 1
			dr = dt.NewRow()
			dr("fileid") = i
			dr("textbox") = fi(i).Name
			dr("checkbox") = fi(i).IsReadOnly
			dr("dropdownlist") = Me.GetRandomValue(5)
			dt.Rows.Add(dr)
		Next

		' 変更のコミット
		dt.AcceptChanges()

		' Datatableを返す。
		Return dt
	End Function

	''' <summary>DataSourceを生成</summary>
	''' <returns>Datatableを返す</returns>
	''' <remarks>DropDownList1用</remarks>
	Private Function CreateDataSource2() As DataTable
		Dim dt As New DataTable()
		Dim dr As DataRow

		' 列生成
		dt.Columns.Add(New DataColumn("value", GetType(Integer)))
		dt.Columns.Add(New DataColumn("text", GetType([String])))

		' 行生成
		For i As Integer = 0 To 4
			dr = dt.NewRow()
			dr("value") = i
			dr("text") = "選択肢" & i.ToString()
			dt.Rows.Add(dr)
		Next

		' 変更のコミット
		dt.AcceptChanges()

		' Datatableを返す。
		Return dt
	End Function

	''' <summary>Randomオブジェクト</summary>
	Private rnd As New Random(Environment.TickCount)

	''' <summary>０～最大値の値をランダムに生成</summary>
	''' <param name="maxVal">最大値</param>
	''' <returns>０～最大値の値</returns>
	Private Function GetRandomValue(maxVal As Integer) As Integer
		Return rnd.[Next](0, maxVal)
	End Function

	#End Region

	#Region "データソースの保持"

	''' <summary>Repeaterのデータソース</summary>
	Public Property RepeaterDataSource() As DataTable
		Get
			Return DirectCast(Session("Repeater1.DataSource"), DataTable)
		End Get
		Set
			Session("Repeater1.DataSource") = value
		End Set
	End Property


	''' <summary>DropDownListのデータソース</summary>
	Public Property DropDownListDataSource() As DataTable
		Get
			Return DirectCast(Session("DropDownList1.DataSource"), DataTable)
		End Get
		Set
			Session("DropDownList1.DataSource") = value
		End Set
	End Property

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

	''' <summary>btnButton2のクリックイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_btnButton2_Click(fxEventArgs As FxEventArgs) As String
		' Repeater1に対する変更をDataTableに反映する。
		Dim dt As DataTable = Me.RepeaterDataSource

		' 変更の検知
		Dim isUpd As Boolean = False

		For i As Integer = 0 To Me.rptRepeater1.Items.Count - 2
			' Repeater1の行毎に処理
			Dim dr As DataRow = dt.Rows(i)

			' 変更されていればDataTableに反映（RowStateが変更される）
			Dim txt As TextBox = DirectCast(Me.rptRepeater1.Items(i).FindControl("TextBox1"), TextBox)
			If dr("textbox").ToString() <> txt.Text Then
				dr("textbox") = txt.Text
				isUpd = True
			End If

			' 変更されていればDataTableに反映（RowStateが変更される）
			Dim cbx As CheckBox = DirectCast(Me.rptRepeater1.Items(i).FindControl("cbxCheckBox1"), CheckBox)
			'RadioButton cbx = ((RadioButton)this.rptRepeater1.Items[i].FindControl("rbnRadioButton1"));
			If CBool(dr("checkbox")) <> cbx.Checked Then
				dr("checkbox") = cbx.Checked
				isUpd = True
			End If

			' 変更されていればDataTableに反映（RowStateが変更される）
			Dim ddl As DropDownList = DirectCast(Me.rptRepeater1.Items(i).FindControl("ddlDropDownList1"), DropDownList)
			'ListBox ddl = ((ListBox)this.rptRepeater1.Items[i].FindControl("lbxListBox1"));
			If dr("dropdownlist").ToString() <> ddl.SelectedValue Then
				dr("dropdownlist") = ddl.SelectedValue
				isUpd = True
			End If
		Next

		' 変更時のみ実行
		If isUpd Then
			' 再データバインド
			Me.RepeaterDataSource = dt
			Me.rptRepeater1.DataSource = dt
			Me.rptRepeater1.DataBind()
		End If

		Return ""
	End Function

	#End Region

	#Region "Repeater内のClickイベント（Command）"

	''' <summary>rptRepeater1のコマンドイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_rptRepeater1_ItemCommand(fxEventArgs As FxEventArgs) As String
		System.Diagnostics.Debug.WriteLine("--------------------")
		System.Diagnostics.Debug.WriteLine("ButtonID:" & Convert.ToString(fxEventArgs.ButtonID))
		System.Diagnostics.Debug.WriteLine("InnerButtonID:" & Convert.ToString(fxEventArgs.InnerButtonID))
		System.Diagnostics.Debug.WriteLine("PostBackValue:" & Convert.ToString(fxEventArgs.PostBackValue))

		Return ""
	End Function

	#End Region

	#Region "Repeater内のClick以外のイベント"

	' ItemCommandイベントに行かないので通常通りハンドルする。
	' （各コントロールのAutoPostBackを"true"に設定する）

	''' <summary>cbxCheckBox1のCheckedChangedイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_cbxCheckBox1_CheckedChanged(fxEventArgs As FxEventArgs) As String
		System.Diagnostics.Debug.WriteLine("--------------------")
		System.Diagnostics.Debug.WriteLine("ButtonID:" & Convert.ToString(fxEventArgs.ButtonID))
		System.Diagnostics.Debug.WriteLine("InnerButtonID:" & Convert.ToString(fxEventArgs.InnerButtonID))
		System.Diagnostics.Debug.WriteLine("PostBackValue:" & Convert.ToString(fxEventArgs.PostBackValue))

		Dim cbx As CheckBox = DirectCast(Me.rptRepeater1.Items(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("cbxCheckBox1"), CheckBox)

		System.Diagnostics.Debug.WriteLine(cbx.Checked.ToString())

		Return ""
	End Function

	''' <summary>rbnRadioButton1のCheckedChangedイベント</summary>
	''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
	''' <returns>URL</returns>
	Protected Function UOC_rbnRadioButton1_CheckedChanged(fxEventArgs As FxEventArgs) As String
		System.Diagnostics.Debug.WriteLine("--------------------")
		System.Diagnostics.Debug.WriteLine("ButtonID:" & Convert.ToString(fxEventArgs.ButtonID))
		System.Diagnostics.Debug.WriteLine("InnerButtonID:" & Convert.ToString(fxEventArgs.InnerButtonID))
		System.Diagnostics.Debug.WriteLine("PostBackValue:" & Convert.ToString(fxEventArgs.PostBackValue))

		Dim cbx As RadioButton = DirectCast(Me.rptRepeater1.Items(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("rbnRadioButton1"), RadioButton)

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

		Dim ddl As DropDownList = DirectCast(Me.rptRepeater1.Items(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("ddlDropDownList1"), DropDownList)

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

		Dim ddl As ListBox = DirectCast(Me.rptRepeater1.Items(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("lbxListBox1"), ListBox)

		System.Diagnostics.Debug.WriteLine(ddl.SelectedValue)

		Return ""
	End Function

	#End Region

	#End Region
End Class

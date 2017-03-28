'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testListView
'* クラス日本語名  ：ListViewテスト画面（Ｐ層）
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  
'*  2014/08/18  Sai-San           Created test page for ListView control
'*  2014/10/03  Rituparna         Added ItemCommandEvent to ListView control
'**********************************************************************************

Imports System.IO

Imports Touryo.Infrastructure.CustomControl
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Framework.Presentation

Namespace Aspx.TestFxLayerP.Table
    ''' <summary>ListView test screen (P layer）</summary>
    Partial Public Class testListView
        Inherits MyBaseController
#Region "初期化"

        ''' <summary>ヘッダーに表示する文字列</summary>
        Public HeaderInfo As New Dictionary(Of String, String)()

        ''' <summary>Page_InitイベントでASP.NET標準Event Handlerを設定</summary>
        Protected Sub Page_Init(sender As Object, e As EventArgs)
        End Sub

        ''' <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する
            ' TODO:
            ' 初回Load時に、データソースを
            ' 生成 ＆ データバインドする。
            Me.CreateDataSource()
            Me.BindListViewData()
        End Sub

        ''' <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する
            Me.CmnInit()

            ' Radio Buttonの選択状態を出力
            If Request.Form("radio-grp1") IsNot Nothing Then
                Me.lblResult.Text = String.Format("name=""radio-grp1"" value=""{0}""が選択されました。<br/>", Request.Form("radio-grp1").ToString())
            End If

            Dim i As Integer = 0
            For Each lvwItem As ListViewDataItem In Me.lvwListView1.Items
                i += 1
                Dim rbn As WebCustomRadioButton = DirectCast(lvwItem.FindControl("rbnRadioButton"), WebCustomRadioButton)

                ' チェック
                ' == null
                If rbn Is Nothing Then
                Else
                    ' != null
                    If rbn.Checked Then
                        Me.lblResult.Text = String.Format("name=""radio-grp1"" value=""{0}""行目が選択されました。<br/>", i.ToString())
                    End If
                End If
            Next
        End Sub

        ''' <summary>
        ''' Sets the header information
        ''' </summary>
        Private Sub CmnInit()
            ' ヘッダーに表示する文字列を初期化
            Me.HeaderInfo.Add("col0", "Select1")
            Me.HeaderInfo.Add("col1", "Select2")
            Me.HeaderInfo.Add("col2", "Custom")
            Me.HeaderInfo.Add("col3", "FileID")
            Me.HeaderInfo.Add("col4", "Readonly")
            Me.HeaderInfo.Add("col5", "FileName")
            Me.HeaderInfo.Add("col6", "FileSize")
            Me.HeaderInfo.Add("col7", "Date")
            Me.HeaderInfo.Add("col8", "Edit")
            Me.HeaderInfo.Add("col9", "Delete")
            Me.HeaderInfo.Add("col10", "Dropdown")
        End Sub

        ''' <summary>
        ''' ★Re[1]: DataPagerでページングされない
        ''' </summary>
        Protected Sub UOC_lvwListView1_PagePropertiesChanged(fxEventArgs As FxEventArgs, e As EventArgs)
            Me.CreateDataSource()
            Me.BindListViewData()
        End Sub

#End Region

#Region "データソースの生成"

        ''' <summary>DataSourceを生成</summary>
        Private Sub CreateDataSource()
            ' Server.MapPathはアプリケーション ディレクトリを指す。
            Dim di As New DirectoryInfo(Server.MapPath("~/Aspx/Common"))
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
        Private Sub BindListViewData()
            Me.lvwListView1.DataSource = Session("SampleData")
            Me.lvwListView1.DataBind()
        End Sub

#Region "通常のイベント"

        ''' <summary>btnButton1のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
            ' Post Backをまたいで値が保存されるかの確認
            Return ""
        End Function

#End Region

        ''' <summary>
        ''' ListView Item Editing event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Protected Sub lvwListView1_ItemEditing(sender As Object, e As ListViewEditEventArgs)
            lvwListView1.EditIndex = e.NewEditIndex
            Me.lvwListView1.DataSource = Session("SampleData")
            Me.lvwListView1.DataBind()
        End Sub

        ''' <summary>
        ''' ListView Item Canceling event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Protected Sub lvwListView1_ItemCanceling(sender As Object, e As ListViewCancelEventArgs)
            lvwListView1.EditIndex = -1
            Me.lvwListView1.DataSource = Session("SampleData")
            Me.lvwListView1.DataBind()
        End Sub

        ''' <summary>
        ''' ListView Item Updating event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Protected Sub UOC_lvwListView1_ItemUpdating(sender As Object, e As ListViewUpdateEventArgs)
            Dim txtFileID As TextBox = DirectCast(lvwListView1.Items(e.ItemIndex).FindControl("txtFileID"), TextBox)
            Dim txtFileName As TextBox = DirectCast(lvwListView1.Items(e.ItemIndex).FindControl("txtFileName"), TextBox)
            Dim cbxReadonly As CheckBox = DirectCast(lvwListView1.Items(e.ItemIndex).FindControl("cbxReadonly"), CheckBox)
            Dim txtFileSize As TextBox = DirectCast(lvwListView1.Items(e.ItemIndex).FindControl("txtFileSize"), TextBox)
            Dim txtDate As TextBox = DirectCast(lvwListView1.Items(e.ItemIndex).FindControl("txtDate"), TextBox)

            ' Gets the updated values from controls for update
            Dim fileid As Integer = CInt(Me.lvwListView1.DataKeys(e.ItemIndex).Value)
            Dim dt As DataTable = DirectCast(Session("SampleData"), DataTable)
            Dim row As DataRow = dt.[Select](String.Format("fileid = '{0}'", fileid))(0)
            row("fileid") = txtFileID.Text
            row("filename") = txtFileName.Text
            row("readonly") = cbxReadonly.Checked
            row("filesize") = txtFileSize.Text
            row("date") = txtDate.Text

            'Sets ListView Edit mode to Normal mode
            Me.lvwListView1.EditIndex = -1
            Me.BindListViewData()
        End Sub

        ''' <summary>
        ''' ListView Item Deleting event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Protected Function UOC_lvwListView1_ItemDeleting(sender As Object, e As ListViewDeleteEventArgs) As String
            Dim dt As DataTable = DirectCast(Session("SampleData"), DataTable)
            Dim fileid As Integer = CInt(Me.lvwListView1.DataKeys(e.ItemIndex).Value)
            dt.[Select](String.Format("fileid = '{0}'", fileid))(0).Delete()

            'Sets ListView Edit mode to Normal mode
            Me.lvwListView1.EditIndex = -1
            Me.BindListViewData()

            Return ""
        End Function

        ''' <summary>
        ''' ListView ItemCommand event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Protected Function UOC_lvwListView1_OnItemCommand(sender As Object, e As ListViewCommandEventArgs) As String
            If [String].Equals(e.CommandName, "GetFiedID") Then
                lblResultOfItemCommand.Text = "You have clicked the FieldID : " & e.CommandArgument.ToString()
                Return ""
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' ListView Item Sorting event
        ''' </summary>
        ''' <param name="fxEventArgs"></param>
        ''' <param name="e"></param>
        ''' <returns></returns>
        Protected Function UOC_lvwListView1_Sorting(fxEventArgs As FxEventArgs, e As ListViewSortEventArgs) As String
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
                    dv.Sort = e.SortExpression & " DESC"

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
            Me.BindListViewData()

            Return ""
        End Function

        ''' <summary>cbxCheckBox3のCheckedChangedイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_cbxCheckBox3_CheckedChanged(fxEventArgs As FxEventArgs) As String
            Debug.WriteLine("--------------------")
            Debug.WriteLine("ButtonID:" & fxEventArgs.ButtonID)
            Debug.WriteLine("InnerButtonID:" & fxEventArgs.InnerButtonID)
            Debug.WriteLine("PostBackValue:" & fxEventArgs.PostBackValue)

            Dim cbx As CheckBox = DirectCast(Me.lvwListView1.Items(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("TextBox1").FindControl("cbxCheckBox3"), CheckBox)

            Debug.WriteLine(cbx.Checked.ToString())

            Return ""
        End Function

        ''' <summary>ddlDropDownList1のSelectedIndexChangedイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_ddlDropDownList1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            Debug.WriteLine("--------------------")
            Debug.WriteLine("ButtonID:" & fxEventArgs.ButtonID)
            Debug.WriteLine("InnerButtonID:" & fxEventArgs.InnerButtonID)
            Debug.WriteLine("PostBackValue:" & fxEventArgs.PostBackValue)

            Dim ddl As DropDownList = DirectCast(Me.lvwListView1.Items(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("ddlDropDownList1"), DropDownList)

            Debug.WriteLine(ddl.SelectedValue)

            Return ""
        End Function

#End Region

    End Class
End Namespace

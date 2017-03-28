'**********************************************************************************
'* フレームワーク・テスト画面（Ｐ層）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testRepeater
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

Imports System.IO

Imports Touryo.Infrastructure.CustomControl
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Framework.Presentation

Namespace Aspx.TestFxLayerP.Table
    ''' <summary>Repeaterテスト画面（Ｐ層）</summary>
    Partial Public Class testRepeater
        Inherits MyBaseController
#Region "初期化"

        ''' <summary>ヘッダーに表示する文字列</summary>
        Public HeaderInfo As New Dictionary(Of String, String)()

        ''' <summary>Page LoadのUOCメソッド（個別：初回Load）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する
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

        ''' <summary>Page LoadのUOCメソッド（個別：Post Back）</summary>
        ''' <remarks>実装必須</remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する
            ' TODO:
            Me.CmnInit()

            ' Radio Buttonの選択状態を出力
            If Request.Form("radio-grp1") IsNot Nothing Then
                Me.lblResult.Text = String.Format("name=""radio-grp1"" value=""{0}""が選択されました。<br/>", Request.Form("radio-grp1").ToString())
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
                        Me.lblResult.Text = String.Format("name=""radio-grp1"" value=""{0}""行目が選択されました。<br/>", i.ToString())
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
            Dim di As New DirectoryInfo(Server.MapPath("~/Aspx/Common"))
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
                Session("Repeater1.DataSource") = Value
            End Set
        End Property


        ''' <summary>DropDownListのデータソース</summary>
        Public Property DropDownListDataSource() As DataTable
            Get
                Return DirectCast(Session("DropDownList1.DataSource"), DataTable)
            End Get
            Set
                Session("DropDownList1.DataSource") = Value
            End Set
        End Property

#End Region

#Region "Event Handler"

#Region "通常のイベント"

        ''' <summary>btnButton1のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnButton1_Click(fxEventArgs As FxEventArgs) As String
            ' Post Backをまたいで値が保存されるかの確認
            Return ""
        End Function

        ''' <summary>btnButton2のClickイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
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
                'RadioButton cbx = ((RadioButton)Me.rptRepeater1.Items[i].FindControl("rbnRadioButton1"));
                If CBool(dr("checkbox")) <> cbx.Checked Then
                    dr("checkbox") = cbx.Checked
                    isUpd = True
                End If

                ' 変更されていればDataTableに反映（RowStateが変更される）
                Dim ddl As DropDownList = DirectCast(Me.rptRepeater1.Items(i).FindControl("ddlDropDownList1"), DropDownList)
                'ListBox ddl = ((ListBox)Me.rptRepeater1.Items[i].FindControl("lbxListBox1"));
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
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_rptRepeater1_ItemCommand(fxEventArgs As FxEventArgs) As String
            Debug.WriteLine("--------------------")
            Debug.WriteLine("ButtonID:" & fxEventArgs.ButtonID)
            Debug.WriteLine("InnerButtonID:" & fxEventArgs.InnerButtonID)
            Debug.WriteLine("PostBackValue:" & fxEventArgs.PostBackValue)

            Return ""
        End Function

#End Region

#Region "Repeater内のClick以外のイベント"

        ' ItemCommandイベントに行かないので通常通りハンドルする。
        ' （各ControlのAutoPostBackを"true"に設定する）

        ''' <summary>cbxCheckBox1のCheckedChangedイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_cbxCheckBox1_CheckedChanged(fxEventArgs As FxEventArgs) As String
            Debug.WriteLine("--------------------")
            Debug.WriteLine("ButtonID:" & fxEventArgs.ButtonID)
            Debug.WriteLine("InnerButtonID:" & fxEventArgs.InnerButtonID)
            Debug.WriteLine("PostBackValue:" & fxEventArgs.PostBackValue)

            Dim cbx As CheckBox = DirectCast(Me.rptRepeater1.Items(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("cbxCheckBox1"), CheckBox)

            Debug.WriteLine(cbx.Checked.ToString())

            Return ""
        End Function

        ''' <summary>rbnRadioButton1のCheckedChangedイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_rbnRadioButton1_CheckedChanged(fxEventArgs As FxEventArgs) As String
            Debug.WriteLine("--------------------")
            Debug.WriteLine("ButtonID:" & fxEventArgs.ButtonID)
            Debug.WriteLine("InnerButtonID:" & fxEventArgs.InnerButtonID)
            Debug.WriteLine("PostBackValue:" & fxEventArgs.PostBackValue)

            Dim cbx As RadioButton = DirectCast(Me.rptRepeater1.Items(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("rbnRadioButton1"), RadioButton)

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

            Dim ddl As DropDownList = DirectCast(Me.rptRepeater1.Items(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("ddlDropDownList1"), DropDownList)

            Debug.WriteLine(ddl.SelectedValue)

            Return ""
        End Function

        ''' <summary>lbxListBox1のSelectedIndexChangedイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_lbxListBox1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
            Debug.WriteLine("--------------------")
            Debug.WriteLine("ButtonID:" & fxEventArgs.ButtonID)
            Debug.WriteLine("InnerButtonID:" & fxEventArgs.InnerButtonID)
            Debug.WriteLine("PostBackValue:" & fxEventArgs.PostBackValue)

            Dim ddl As ListBox = DirectCast(Me.rptRepeater1.Items(Integer.Parse(fxEventArgs.PostBackValue)).FindControl("lbxListBox1"), ListBox)

            Debug.WriteLine(ddl.SelectedValue)

            Return ""
        End Function

#End Region

#End Region
    End Class

End Namespace

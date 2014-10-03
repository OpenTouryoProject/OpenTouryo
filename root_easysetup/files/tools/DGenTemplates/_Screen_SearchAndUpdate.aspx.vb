'**********************************************************************************
'* 三層データバインド・アプリ画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：_JoinTableName__Screen_SearchAndUpdate
'* クラス日本語名  ：三層データバインド・検索一覧更新画面（_JoinTableName_）
'*
'* 作成日時        ：_TimeStamp_
'* 作成者          ：自動生成ツール（墨壺２）, _UserName_
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************
' System
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


Partial Public Class _JoinTableName__Screen_SearchAndUpdate
    Inherits MyBaseController

#Region "ページロードのUOCメソッド UOC Method of Page Load"

    ''' <summary>
    ''' ページロードのUOCメソッド（個別：初回ロード）
    ''' </summary>
    ''' <remarks>
    ''' 実装必須
    ''' </remarks>
    Protected Overrides Sub UOC_FormInit()
        ' フォーム初期化（初回ロード）時に実行する処理を実装する

        ' TODO:

        ' 更新ボタンの非活性化
        Me.btnBatUpd.Enabled = False
    End Sub

    ''' <summary>
    ''' ページロードのUOCメソッド（個別：ポストバック）
    ''' </summary>
    ''' <remarks>
    ''' 実装必須
    ''' </remarks>
    Protected Overrides Sub UOC_FormInit_PostBack()
        ' フォーム初期化（ポストバック）時に実行する処理を実装する

        ' TODO:
        Session("DAP") = "_DAP_"
        Session("DBMS") = DbEnum.DBMSType._DBMS_
    End Sub

#End Region

#Region "イベントハンドラ EVENT HANDLER"

#Region "一覧検索 SEARCH LIST"

    ''' <summary>検索ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnSearch_Click(ByVal fxEventArgs As FxEventArgs) As String
        ' 更新ボタンの非活性化
        Me.btnBatUpd.Enabled = False

        ' GridViewをリセット
        Me.gvwGridView1.PageIndex = 0
        Me.gvwGridView1.Sort("", SortDirection.Ascending)

        ' 検索条件の収集
        ' AndEqualSearchConditions
        Dim andEqualSearchConditions As New Dictionary(Of String, Object)()
        ' ControlComment:LoopStart-PKColumn
        andEqualSearchConditions.Add("_JoinTextboxColumnName_", Me.txt_JoinTextboxColumnName__And.Text)
        ' ControlComment:LoopEnd-PKColumn
        ' ControlComment:LoopStart-ElseColumn
        andEqualSearchConditions.Add("_JoinTextboxColumnName_", Me.txt_JoinTextboxColumnName__And.Text)
        ' ControlComment:LoopEnd-ElseColumn
        Session("AndEqualSearchConditions") = andEqualSearchConditions

        ' 引数クラスを生成
        Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectRecord", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

        ' テーブル
        parameterValue.TableName = "_JoinTableName_"

        ' 主キーとタイムスタンプ列
        parameterValue.AndEqualSearchConditions = DirectCast(Session("AndEqualSearchConditions"), Dictionary(Of String, Object))

        ' B層を生成
        Dim b As New _3TierEngine()

        ' データ取得処理を実行
        Dim returnValue As _3TierReturnValue = DirectCast(b.DoBusinessLogic(DirectCast(parameterValue, BaseParameterValue), DbEnum.IsolationLevelEnum.ReadCommitted), _3TierReturnValue)
        'Declare Table to bind data to gridview
        Dim dt As New DataTable()
        dt = returnValue.Dt
        HttpContext.Current.Session("SearchResult") = dt

        ' Set Header
        Me.gvwGridView1.Columns(_ColumnNmbr_).HeaderText = "Delete"
        Me.gvwGridView1.Columns(_ColumnNmbr_).HeaderText = "Update"
        ' ControlComment:LoopStart-PKColumn
        Me.gvwGridView1.Columns(_ColumnNmbr_).HeaderText = "_JoinTextboxColumnName_"
        ' ControlComment:LoopEnd-PKColumn
        ' ControlComment:LoopStart-ElseColumn
        Me.gvwGridView1.Columns(_ColumnNmbr_).HeaderText = "_JoinTextboxColumnName_"
        ' ControlComment:LoopEnd-ElseColumn

        'Bind gridview
        Me.gvwGridView1.DataSource = dt
        Me.gvwGridView1.DataBind()

        ' 画面遷移しない。
        Return String.Empty
    End Function

    ''' <summary>gvwGridView1のSortingイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <param name="e">オリジナルのイベント引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_gvwGridView1_Sorting(ByVal fxEventArgs As FxEventArgs, ByVal e As GridViewSortEventArgs) As String
        ' ソート条件の変更
        Session("SortExpression") = e.SortExpression

        If DirectCast(Session("SortDirection"), String) = "ASC" Then
            e.SortDirection = SortDirection.Descending
            Session("SortDirection") = "DESC"
        Else
            e.SortDirection = SortDirection.Ascending
            Session("SortDirection") = "ASC"
        End If

        ' 画面遷移しない。
        Return String.Empty
    End Function

#End Region

#Region "CRUD USING BATCH UPDATE"

    ''' <summary>追加ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnInsert_Click(ByVal fxEventArgs As FxEventArgs) As String
        ' 画面遷移（詳細表示）
        Return "_JoinTableName__Screen_Detail.aspx"
    End Function

    ''' <summary>バッチ更新ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnBatUpd_Click(ByVal fxEventArgs As FxEventArgs) As String
        '#Region "Create the instance of classes here"

        ' 引数クラスを生成
        Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "BatchUpdate", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

        'Initialize the data access procedure
        Dim returnValue As _3TierReturnValue = Nothing
        ' B layer Initialize
        Dim b As New _3TierEngine()

        'Keep the copy of the table in session because change in the column name causes the problem in the temperory update after batch update. So keep the copy of the table.
        Dim dtSession As DataTable = DirectCast(Session("SearchResult"), DataTable).Copy()
        '#End Region
        ' ControlComment:LoopStart-JoinTables

        '#Region "Batch Update for _TableName_  table"
        '#Region "This is much needed to handle the duplicate column issue while udpating  _TableName_ using batch update"
        ' to change the column names of table as per Table we should have copy of dtSession table.
        Dim dt_TableName_ As DataTable = dtSession.Copy()

        For Each dc As DataColumn In dt_TableName_.Columns
            'Remove the '_TableName_.' from column names of _TableName_ only so that update will not have any problem.
            'Otherwise When two tables are having same column names then we get an  error "A column named 'Columname' already belongs to this DataTable."
            If dc.ColumnName.Split("."c)(0).Trim() = "_TableName_" Then
                dc.ColumnName = dc.ColumnName.Split("."c)(1).Trim()
            Else
                'Replace "." in column names of other tables with "_". This is needed becuase if columns are having "." then we get sql error, so we need to replace "." with "_"

                dc.ColumnName = dc.ColumnName.Replace("."c, "_"c)
            End If
        Next

        '#End Region

        ' DataTableを設定
        parameterValue.Obj = dt_TableName_

        'Reset returnvalue with null;
        returnValue = Nothing

        parameterValue.AndEqualSearchConditions = New Dictionary(Of String, Object)()
        'Primary Key Columns
        ' ControlComment:LoopStart-PKColumn
        parameterValue.AndEqualSearchConditions.Add("_ColumnName_", "")
        ' ControlComment:LoopEnd-PKColumn
        'Timestamp Column.
        TS_CommentOut_ parameterValue.AndEqualSearchConditions.Add("_TimeStampColName_", "")

        ' Table Name
        parameterValue.TableName = "_TableName_"

        ' Run the Database access process
        returnValue = DirectCast(b.DoBusinessLogic(DirectCast(parameterValue, BaseParameterValue), DbEnum.IsolationLevelEnum.ReadCommitted), _3TierReturnValue)

        '#End Region
        ' ControlComment:LoopEnd-JoinTables

        ' Disable the button
        Me.btnBatUpd.Enabled = False
        ' Keep the original session table with actual column names.
        Session("SearchResult") = dtSession
        'No Screen transition
        Return String.Empty
    End Function

    ''' <summary>gvwGridView1のコマンドイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_gvwGridView1_RowCommand(ByVal fxEventArgs As FxEventArgs) As String
        ' ソートの場合は無視
        If fxEventArgs.InnerButtonID = "Sort" Then
            Return String.Empty
        End If

        ' DataTableの取得
        Dim dt As DataTable = DirectCast(Session("SearchResult"), DataTable)

        ' インデックスを取得
        Dim index As Integer = Integer.Parse(fxEventArgs.PostBackValue)

        ' e.NewSelectedIndexRowsのインデックスが一致しないので。
        ' キーで探すのは主キーを意識するため自動生成では面倒になる。
        Dim i As Integer = -1

        Select Case fxEventArgs.InnerButtonID
            Case "Delete"

                ' 選択されたレコードを削除
                For Each dr As DataRow In dt.Rows
                    If dr.RowState = DataRowState.Added Then
                        '　Added行はDeleteできないのでスキップ
                        Continue For
                    ElseIf dr.RowState <> DataRowState.Deleted Then
                        ' != Added、Deleted

                        ' e.NewSelectedIndexとRowsのインデックスをチェック
                        i += 1
                        If index = i Then
                            ' 削除
                            dr.Delete()
                            Exit For
                        End If
                    Else
                        ' Delete行は表示されないのでスキップ
                        Continue For
                    End If
                Next

                Exit Select

            Case "Update"

                ' 選択されたレコードを更新
                For Each dr As DataRow In dt.Rows
                    If dr.RowState <> DataRowState.Deleted Then
                        ' != Deleted

                        ' e.NewSelectedIndexとRowsのインデックスをチェック
                        i += 1
                        If index = i Then
                            ' 更新
                            Dim gvRow As GridViewRow = Me.gvwGridView1.Rows(index)
                            For Each dc As DataColumn In dt.Columns
                                Dim txtBox As TextBox = DirectCast(gvRow.FindControl("txt" + dc.ColumnName.Replace("."c, "_"c)), TextBox)

                                If txtBox IsNot Nothing Then

                                    If TypeOf (dr(dc)) Is Byte Then
                                    Else
                                        dr(dc) = txtBox.Text
                                    End If
                                End If

                                '#Region "追加コード（ComboBox化）"

                                Dim ddl As DropDownList = DirectCast(gvRow.FindControl("ddl" + dc.ColumnName.Replace("."c, "_"c)), DropDownList)

                                If ddl IsNot Nothing Then
                                    dr(dc) = ddl.SelectedValue

                                    '#End Region
                                End If
                            Next

                            Exit For
                        End If
                    Else
                        ' Delete行はスキップ
                        Continue For
                    End If
                Next

                Exit Select
            Case Else

                ' 不明
                Return String.Empty
        End Select

        ' GridViewをリセット
        Me.gvwGridView1.PageIndex = 0
        Me.gvwGridView1.Sort("", SortDirection.Ascending)

        ' ページングの中止
        Me.gvwGridView1.AllowPaging = False

        ' GridViewのDataSourceを変更してDataBindする。
        Me.gvwGridView1.DataSource = dt
        Me.gvwGridView1.DataSourceID = Nothing
        Me.gvwGridView1.DataBind()

        ' DataTableの設定
        Session("SearchResult") = dt

        ' 更新ボタンの活性化
        Me.btnBatUpd.Enabled = True

        ' 画面遷移しない。
        Return String.Empty
    End Function

#End Region

#End Region

End Class


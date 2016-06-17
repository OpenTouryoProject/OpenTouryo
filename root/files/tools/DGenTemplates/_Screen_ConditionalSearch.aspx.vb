'**********************************************************************************
'* 三層データバインド・アプリ画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：_JoinTableName__Screen_ConditionalSearch
'* クラス日本語名  ：三層データバインド・検索一覧表示画面（_JoinTableName_）
'*
'* 作成日時        ：_TimeStamp_
'* 作成者          ：自動生成ツール（墨壺２）, _UserName_
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2016/06/15  Shashikiran       Added UOC_gvwGridView1_PageIndexChanging function to handle Gridview Paging event
'*  2016/06/15  Shashikiran       Modified gvwGridView1_SelectedIndexChanging function to handle Gridview row selection during paging
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

''' <summary>三層データバインド・サンプル アプリ画面（検索一覧表示）</summary>
Partial Public Class _JoinTableName__Screen_ConditionalSearch
    Inherits MyBaseController

#Region "ASP.NET EVENT HANDLER"
    ''' <summary>Page_InitイベントでASP.NET標準イベントハンドラを設定</summary>
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        ' 行選択についてのイベント
        'Me.gvwGridView1.SelectedIndexChanging += New GridViewSelectEventHandler(AddressOf gvwGridView1_SelectedIndexChanging)
        AddHandler Me.gvwGridView1.SelectedIndexChanging, New GridViewSelectEventHandler(AddressOf gvwGridView1_SelectedIndexChanging)
    End Sub
#End Region

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

    ''' <summary>検索ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnSearch_Click(ByVal fxEventArgs As FxEventArgs) As String
        ' GridViewをリセット
        Me.gvwGridView1.PageIndex = 0
        Me.gvwGridView1.Sort("", SortDirection.Ascending)

        ' 検索条件の収集
        ' AndEqualSearchConditions
        Dim andEqualSearchConditions As New Dictionary(Of String, Object)()

        'ControlComment:LoopStart-PKColumn
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

        ' Set the Gridview Header
        Me.gvwGridView1.Columns(_ColumnNmbr_).HeaderText = "Select"
        ' ControlComment:LoopStart-PKColumn
        Me.gvwGridView1.Columns(_ColumnNmbr_).HeaderText = "_JoinTextboxColumnName_"
        ' ControlComment:LoopEnd-PKColumn
        ' ControlComment:LoopStart-ElseColumn
        Me.gvwGridView1.Columns(_ColumnNmbr_).HeaderText = "_JoinTextboxColumnName_"
        ' ControlComment:LoopEnd-ElseColumn

        'Bind gridview
        Me.gvwGridView1.DataSource = dt
        Me.gvwGridView1.DataBind()

        'Return empty string since there is no need to redirect to any other page.
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
        'Return empty string since there is no need to redirect to any other page.
        Return String.Empty
    End Function

    ''' <summary>GridView Even That occurs before selection of row buttons</summary>
    Protected Sub gvwGridView1_SelectedIndexChanging(ByVal sender As Object, ByVal e As GridViewSelectEventArgs)
        'Get Primary key and timestamp Column for the selected record
        Dim dt As DataTable = DirectCast(Session("SearchResult"), DataTable)
        Dim PrimaryKeyAndTimeStamp As New Dictionary(Of String, Object)()

        ' Primary key columns
        ' ControlComment:LoopStart-PKColumn
        PrimaryKeyAndTimeStamp.Add("_JoinTextboxColumnName_", dt.Rows(e.NewSelectedIndex + ((gvwGridView1.PageSize * gvwGridView1.PageIndex)))("_JoinColumnName_").ToString())
        ' ControlComment:LoopEnd-PKColumn
        'Timestamp Column
        ' タイムスタンプ列	
        ' ControlComment:LoopStart-JoinTables   
        TS_CommentOut_ If dt.Rows(e.NewSelectedIndex)("_TableName_._TimeStampColName_").GetType() is GetType(System.DBNull) Then
        TS_CommentOut_ Else
        TS_CommentOut_PrimaryKeyAndTimeStamp.Add("_TableName___TimeStampColName_", dt.Rows(e.NewSelectedIndex)("_TableName_._TimeStampColName_"))
        TS_CommentOut_ End If
        'ControlComment:LoopEnd-JoinTables

        Session("PrimaryKeyAndTimeStamp") = PrimaryKeyAndTimeStamp
    End Sub

    ''' <summary>gvwGridView1の行選択後イベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_gvwGridView1_SelectedIndexChanged(ByVal fxEventArgs As FxEventArgs) As String
        'Screen Transition is required to show more
        Return "_JoinTableName__Screen_Detail.aspx"
    End Function

    ''' <summary>gvwGridView1 Paging Event</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_gvwGridView1_PageIndexChanging(ByVal fxEventArgs As FxEventArgs, ByVal e As GridViewPageEventArgs) As String
        Me.gvwGridView1.PageIndex = e.NewPageIndex
        Me.gvwGridView1.DataSource = DirectCast(Session("SearchResult"), DataTable)
        Me.gvwGridView1.DataBind()
        'Return empty string since there is no need to redirect to any other page.
        Return String.Empty
    End Function

#End Region

End Class
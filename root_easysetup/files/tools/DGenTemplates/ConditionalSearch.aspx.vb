'**********************************************************************************
'* 三層データバインド・アプリ画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：_TableName_ConditionalSearch
'* クラス日本語名  ：三層データバインド・検索一覧表示画面（_TableName_）
'*
'* 作成日時        ：_TimeStamp_
'* 作成者          ：自動生成ツール（墨壺２）, _UserName_
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports MyType

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
Partial Public Class _TableName_ConditionalSearch
    Inherits MyBaseController
    ''' <summary>Page_InitイベントでASP.NET標準イベントハンドラを設定</summary>
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        ' 行選択についてのイベント
        'Me.gvwGridView1.SelectedIndexChanging += New GridViewSelectEventHandler(AddressOf gvwGridView1_SelectedIndexChanging)
        AddHandler Me.gvwGridView1.SelectedIndexChanging, New GridViewSelectEventHandler(AddressOf gvwGridView1_SelectedIndexChanging)
    End Sub

#Region "ページロードのUOCメソッド"

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

#Region "イベントハンドラ"

    ''' <summary>追加ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnInsert_Click(ByVal fxEventArgs As FxEventArgs) As String
        ' 画面遷移（詳細表示）
        Return "_TableName_Detail.aspx"
    End Function

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
        ' ControlComment:LoopStart-PKColumn
        andEqualSearchConditions.Add("_ColumnName_", Me.txt_ColumnName__And.Text)
        ' ControlComment:LoopEnd-PKColumn
        ' ControlComment:LoopStart-ElseColumn
        andEqualSearchConditions.Add("_ColumnName_", Me.txt_ColumnName__And.Text)
        ' ControlComment:LoopEnd-ElseColumn
        Session("AndEqualSearchConditions") = andEqualSearchConditions

        ' AndLikeSearchConditions
        Dim andLikeSearchConditions As New Dictionary(Of String, String)()
        ' ControlComment:LoopStart-PKColumn
        andLikeSearchConditions.Add("_ColumnName_", Me.txt_ColumnName__And_Like.Text)
        ' ControlComment:LoopEnd-PKColumn
        ' ControlComment:LoopStart-ElseColumn
        andLikeSearchConditions.Add("_ColumnName_", Me.txt_ColumnName__And_Like.Text)
        ' ControlComment:LoopEnd-ElseColumn
        Session("AndLikeSearchConditions") = andLikeSearchConditions

        ' OrEqualSearchConditions
        Dim orEqualSearchConditions As New Dictionary(Of String, Object())()
        ' ControlComment:LoopStart-PKColumn
        orEqualSearchConditions.Add("_ColumnName_", Me.txt_ColumnName__OR.Text.Split(" "c))
        ' ControlComment:LoopEnd-PKColumn
        ' ControlComment:LoopStart-ElseColumn
        orEqualSearchConditions.Add("_ColumnName_", Me.txt_ColumnName__OR.Text.Split(" "c))
        ' ControlComment:LoopEnd-ElseColumn
        Session("OrEqualSearchConditions") = orEqualSearchConditions

        ' OrLikeSearchConditions
        Dim orLikeSearchConditions As New Dictionary(Of String, String())()
        ' ControlComment:LoopStart-PKColumn
        orLikeSearchConditions.Add("_ColumnName_", Me.txt_ColumnName__OR_Like.Text.Split(" "c))
        ' ControlComment:LoopEnd-PKColumn
        ' ControlComment:LoopStart-ElseColumn
        orLikeSearchConditions.Add("_ColumnName_", Me.txt_ColumnName__OR_Like.Text.Split(" "c))
        ' ControlComment:LoopEnd-ElseColumn
        Session("OrLikeSearchConditions") = orLikeSearchConditions

        '''/ ElseSearchConditions
        'Dictionary<string, object> ElseSearchConditions = new Dictionary<string, object>();
        'ElseSearchConditions.Add("myp1", 1);
        'ElseSearchConditions.Add("myp2", 40);
        'Session["ElseSearchConditions"] = ElseSearchConditions;
        'Session["ElseWhereSQL"] = "AND [ProductID] BETWEEN @myp1 AND @myp2";

        ' ソート条件の初期化
        Session("SortExpression") = "_PKFirstColumn_"
        ' 主キーを指定
        Session("SortDirection") = "ASC"
        ' ASCを指定
        ' gvwGridView1をObjectDataSourceに連結。
        Me.gvwGridView1.DataSourceID = "ObjectDataSource1"

        ' ヘッダーを設定する。
        Me.gvwGridView1.Columns(_ColumnNmbr_).HeaderText = "選択"
        ' ControlComment:LoopStart-PKColumn
        Me.gvwGridView1.Columns(_ColumnNmbr_).HeaderText = "_ColumnName_"
        ' ControlComment:LoopEnd-PKColumn
        ' ControlComment:LoopStart-ElseColumn
        Me.gvwGridView1.Columns(_ColumnNmbr_).HeaderText = "_ColumnName_"
        ' ControlComment:LoopEnd-ElseColumn

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

    ''' <summary>GridViewの行の選択ボタンがクリックされ、行が選択される前に発生するイベント</summary>
    Protected Sub gvwGridView1_SelectedIndexChanging(ByVal sender As Object, ByVal e As GridViewSelectEventArgs)
        ' 選択されたレコードの主キーとタイムスタンプ列を取得
        Dim dt As DataTable = DirectCast(Session("SearchResult"), DataTable)
        Dim PrimaryKeyAndTimeStamp As New Dictionary(Of String, Object)()

        ' 主キーとタイムスタンプ列
        ' 主キー列
        ' ControlComment:LoopStart-PKColumn
        PrimaryKeyAndTimeStamp.Add("_ColumnName_", dt.Rows(e.NewSelectedIndex)("_ColumnName_").ToString())
        ' ControlComment:LoopEnd-PKColumn

        ' タイムスタンプ列	        
        TS_CommentOut_ If dt.Rows(e.NewSelectedIndex)("_TimeStampColName_").[GetType]() = GetType(System.DBNull) Then

        TS_CommentOut_ Else
        TS_CommentOut_(PrimaryKeyAndTimeStamp.Add("_TimeStampColName_", dt.Rows(e.NewSelectedIndex)("_TimeStampColName_")))
        TS_CommentOut_ End If

        Session("PrimaryKeyAndTimeStamp") = PrimaryKeyAndTimeStamp
    End Sub

    ''' <summary>gvwGridView1の行選択後イベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_gvwGridView1_SelectedIndexChanged(ByVal fxEventArgs As FxEventArgs) As String
        ' 画面遷移（詳細表示）
        Return "_TableName_Detail.aspx"
    End Function

#End Region
End Class
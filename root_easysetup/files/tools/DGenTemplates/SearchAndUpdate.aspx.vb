'**********************************************************************************
'* 三層データバインド・アプリ画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：_TableName_SearchAndUpdate
'* クラス日本語名  ：三層データバインド・検索一覧更新画面（_TableName_）
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

''' <summary>三層データバインド・サンプル アプリ画面（検索一覧更新）</summary>
Partial Public Class _TableName_SearchAndUpdate
    Inherits MyBaseController
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

#Region "イベントハンドラ"

#Region "一覧検索"

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
        ' ページング
        Me.gvwGridView1.AllowPaging = True

        ' gvwGridView1をObjectDataSourceに連結。
        Me.gvwGridView1.DataSource = Nothing
        Me.gvwGridView1.DataSourceID = "ObjectDataSource1"

        ' ヘッダーを設定する。
        Me.gvwGridView1.Columns(_ColumnNmbr_).HeaderText = "削除"
        Me.gvwGridView1.Columns(_ColumnNmbr_).HeaderText = "更新"
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

#End Region

#Region "CRUD"

    ''' <summary>追加ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnInsert_Click(ByVal fxEventArgs As FxEventArgs) As String
        ' 画面遷移（詳細表示）
        Return "_TableName_Detail.aspx"
    End Function

    ''' <summary>バッチ更新ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnBatUpd_Click(ByVal fxEventArgs As FxEventArgs) As String
        ' 引数クラスを生成
        Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "BatchUpdate", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

        ' テーブル
        parameterValue.TableName = "_TableName_"

        ' 主キーとタイムスタンプ列
        parameterValue.AndEqualSearchConditions = New Dictionary(Of String, Object)()

        ' 主キー列
        ' ControlComment:LoopStart-PKColumn
        parameterValue.AndEqualSearchConditions.Add("_ColumnName_", "")
        ' ControlComment:LoopEnd-PKColumn

        ' タイムスタンプ列
        ' ・・・

        ' DataTableを設定
        parameterValue.Obj = DirectCast(Session("SearchResult"), DataTable)

        ' B層を生成
        Dim b As New _3TierEngine()

        ' データ取得処理を実行
        Dim returnValue As _3TierReturnValue = DirectCast(b.DoBusinessLogic(DirectCast(parameterValue, BaseParameterValue), DbEnum.IsolationLevelEnum.ReadCommitted), _3TierReturnValue)

        ' 結果表示
        'this.lblResult.Text = returnValue.Obj.ToString() + "件更新しました。";

        ' 更新ボタンの非活性化
        Me.btnBatUpd.Enabled = False

        ' 画面遷移しない。
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
                                Dim txtBox As TextBox = DirectCast(gvRow.FindControl("txt" + dc.ColumnName), TextBox)

                                If txtBox IsNot Nothing Then
                                    If TypeOf (dr(dc)) Is Byte Then
                                    Else
                                        dr(dc) = txtBox.Text
                                    End If
                                End If

                                '#Region "追加コード（ComboBox化）"

                                Dim ddl As DropDownList = DirectCast(gvRow.FindControl("ddl" + dc.ColumnName), DropDownList)

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
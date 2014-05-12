'**********************************************************************************
'* 三層データバインド・アプリ画面
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：_ConditionalSearch_
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
Partial Public Class ProductsConditionalSearch
    Inherits MyBaseController
    ''' <summary>Page_InitイベントでASP.NET標準イベントハンドラを設定</summary>
    Protected Sub Page_Init(sender As Object, e As EventArgs)
        ' 行選択についてのイベント
        AddHandler Me.gvwGridView1.SelectedIndexChanging, AddressOf gvwGridView1_SelectedIndexChanging
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

        '#Region "マスタ・データのロードと設定"

        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, "FormInit_PostBack", "Invoke", Me.ddlDap.SelectedValue, Me.UserInfo)

        ' B層を生成
        Dim getMasterData As New GetMasterData()

        ' 業務処理を実行
        Dim returnValue As _3TierReturnValue = DirectCast(getMasterData.DoBusinessLogic(DirectCast(parameterValue, BaseParameterValue), DbEnum.IsolationLevelEnum.ReadCommitted), _3TierReturnValue)

        Dim dts As DataTable() = DirectCast(returnValue.Obj, DataTable())
        Dim dt As DataTable = Nothing
        Dim dr As DataRow = Nothing

        ' daoSuppliers
        _3TierEngine.CreateDropDownListDataSourceDataTable(dts(0), "SupplierID", "CompanyName", dt, "value", "text")

        ' 空の値
        dr = dt.NewRow()
        dr("value") = ""
        dr("text") = "empty"
        dt.Rows.Add(dr)
        dt.AcceptChanges()

        Me.ddlSupplierID_And.DataValueField = "value"
        Me.ddlSupplierID_And.DataTextField = "text"
        Me.ddlSupplierID_And.SelectedValue = ""
        Me.ddlSupplierID_And.DataSource = dt
        Me.ddlSupplierID_And.DataBind()

        Me.ddldsdt_Suppliers = dt

        ' daoCategories
        _3TierEngine.CreateDropDownListDataSourceDataTable(dts(1), "CategoryID", "CategoryName", dt, "value", "text")

        ' 空の値
        dr = dt.NewRow()
        dr("value") = ""
        dr("text") = "empty"
        dt.Rows.Add(dr)
        dt.AcceptChanges()

        Me.ddlCategoryID_And.DataValueField = "value"
        Me.ddlCategoryID_And.DataTextField = "text"
        Me.ddlCategoryID_And.SelectedValue = ""
        Me.ddlCategoryID_And.DataSource = dt
        Me.ddlCategoryID_And.DataBind()

        Me.ddldsdt_Categories = dt

        '#End Region
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
        Session("DAP") = Me.ddlDap.SelectedValue

        If Me.ddlDap.SelectedValue = "SQL" Then
            Session("DBMS") = DbEnum.DBMSType.SQLServer
        Else
            Session("DBMS") = DbEnum.DBMSType.Oracle
        End If
    End Sub

#Region "マスタ・データの設定用プロパティ"

    ''' <summary>DropDownList生成用のプロパティ</summary>
    Public Property ddldsdt_Suppliers() As DataTable
        Get
            Return DirectCast(Session("ddldsdt_SupplierID"), DataTable)
        End Get
        Set(value As DataTable)
            Session("ddldsdt_SupplierID") = Value
        End Set
    End Property

    ''' <summary>DropDownList生成用のプロパティ</summary>
    Public Property ddldsdt_Categories() As DataTable
        Get
            Return DirectCast(Session("ddldsdt_CategoryID"), DataTable)
        End Get
        Set(value As DataTable)
            Session("ddldsdt_CategoryID") = Value
        End Set
    End Property

#End Region

#End Region

#Region "イベントハンドラ"

    ''' <summary>追加ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnInsert_Click(fxEventArgs As FxEventArgs) As String
        ' 画面遷移（詳細表示）
        Return "ProductsDetail.aspx"
    End Function

    ''' <summary>検索ボタン</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_btnSearch_Click(fxEventArgs As FxEventArgs) As String
        ' GridViewをリセット
        Me.gvwGridView1.PageIndex = 0
        Me.gvwGridView1.Sort("", SortDirection.Ascending)

        ' 検索条件の収集
        ' AndEqualSearchConditions
        Dim andEqualSearchConditions As New Dictionary(Of String, Object)()
        andEqualSearchConditions.Add("ProductID", Me.txtProductID_And.Text)
        andEqualSearchConditions.Add("ProductName", Me.txtProductName_And.Text)

        'andEqualSearchConditions.Add("SupplierID", Me.txtSupplierID_And.Text)
        andEqualSearchConditions.Add("SupplierID", Me.ddlSupplierID_And.SelectedValue)
        'andEqualSearchConditions.Add("CategoryID", Me.txtCategoryID_And.Text)
        andEqualSearchConditions.Add("CategoryID", Me.ddlCategoryID_And.SelectedValue)

        andEqualSearchConditions.Add("QuantityPerUnit", Me.txtQuantityPerUnit_And.Text)
        andEqualSearchConditions.Add("UnitPrice", Me.txtUnitPrice_And.Text)
        andEqualSearchConditions.Add("UnitsInStock", Me.txtUnitsInStock_And.Text)
        andEqualSearchConditions.Add("UnitsOnOrder", Me.txtUnitsOnOrder_And.Text)
        andEqualSearchConditions.Add("ReorderLevel", Me.txtReorderLevel_And.Text)
        andEqualSearchConditions.Add("Discontinued", Me.txtDiscontinued_And.Text)
        Session("AndEqualSearchConditions") = andEqualSearchConditions

        ' AndLikeSearchConditions
        Dim andLikeSearchConditions As New Dictionary(Of String, String)()
        andLikeSearchConditions.Add("ProductID", Me.txtProductID_And_Like.Text)
        andLikeSearchConditions.Add("ProductName", Me.txtProductName_And_Like.Text)
        andLikeSearchConditions.Add("SupplierID", Me.txtSupplierID_And_Like.Text)
        andLikeSearchConditions.Add("CategoryID", Me.txtCategoryID_And_Like.Text)
        andLikeSearchConditions.Add("QuantityPerUnit", Me.txtQuantityPerUnit_And_Like.Text)
        andLikeSearchConditions.Add("UnitPrice", Me.txtUnitPrice_And_Like.Text)
        andLikeSearchConditions.Add("UnitsInStock", Me.txtUnitsInStock_And_Like.Text)
        andLikeSearchConditions.Add("UnitsOnOrder", Me.txtUnitsOnOrder_And_Like.Text)
        andLikeSearchConditions.Add("ReorderLevel", Me.txtReorderLevel_And_Like.Text)
        andLikeSearchConditions.Add("Discontinued", Me.txtDiscontinued_And_Like.Text)
        Session("AndLikeSearchConditions") = andLikeSearchConditions

        ' OrEqualSearchConditions
        Dim orEqualSearchConditions As New Dictionary(Of String, Object())()
        orEqualSearchConditions.Add("ProductID", Me.txtProductID_OR.Text.Split(" "c))
        orEqualSearchConditions.Add("ProductName", Me.txtProductName_OR.Text.Split(" "c))
        orEqualSearchConditions.Add("SupplierID", Me.txtSupplierID_OR.Text.Split(" "c))
        orEqualSearchConditions.Add("CategoryID", Me.txtCategoryID_OR.Text.Split(" "c))
        orEqualSearchConditions.Add("QuantityPerUnit", Me.txtQuantityPerUnit_OR.Text.Split(" "c))
        orEqualSearchConditions.Add("UnitPrice", Me.txtUnitPrice_OR.Text.Split(" "c))
        orEqualSearchConditions.Add("UnitsInStock", Me.txtUnitsInStock_OR.Text.Split(" "c))
        orEqualSearchConditions.Add("UnitsOnOrder", Me.txtUnitsOnOrder_OR.Text.Split(" "c))
        orEqualSearchConditions.Add("ReorderLevel", Me.txtReorderLevel_OR.Text.Split(" "c))
        orEqualSearchConditions.Add("Discontinued", Me.txtDiscontinued_OR.Text.Split(" "c))
        Session("OrEqualSearchConditions") = orEqualSearchConditions

        ' OrLikeSearchConditions
        Dim orLikeSearchConditions As New Dictionary(Of String, String())()
        orLikeSearchConditions.Add("ProductID", Me.txtProductID_OR_Like.Text.Split(" "c))
        orLikeSearchConditions.Add("ProductName", Me.txtProductName_OR_Like.Text.Split(" "c))
        orLikeSearchConditions.Add("SupplierID", Me.txtSupplierID_OR_Like.Text.Split(" "c))
        orLikeSearchConditions.Add("CategoryID", Me.txtCategoryID_OR_Like.Text.Split(" "c))
        orLikeSearchConditions.Add("QuantityPerUnit", Me.txtQuantityPerUnit_OR_Like.Text.Split(" "c))
        orLikeSearchConditions.Add("UnitPrice", Me.txtUnitPrice_OR_Like.Text.Split(" "c))
        orLikeSearchConditions.Add("UnitsInStock", Me.txtUnitsInStock_OR_Like.Text.Split(" "c))
        orLikeSearchConditions.Add("UnitsOnOrder", Me.txtUnitsOnOrder_OR_Like.Text.Split(" "c))
        orLikeSearchConditions.Add("ReorderLevel", Me.txtReorderLevel_OR_Like.Text.Split(" "c))
        orLikeSearchConditions.Add("Discontinued", Me.txtDiscontinued_OR_Like.Text.Split(" "c))
        Session("OrLikeSearchConditions") = orLikeSearchConditions

        '' ElseSearchConditions
        'Dim ElseSearchConditions As New Dictionary(Of String, Object)()
        'ElseSearchConditions.Add("myp1", 1)
        'ElseSearchConditions.Add("myp2", 40)
        'Session("ElseSearchConditions") = ElseSearchConditions
        'Session("ElseWhereSQL") = "AND [ProductID] BETWEEN @myp1 AND @myp2"

        ' ソート条件の初期化
        Session("SortExpression") = "ProductID"
        ' 主キーを指定
        Session("SortDirection") = "ASC"
        ' ASCを指定
        ' gvwGridView1をObjectDataSourceに連結。
        Me.gvwGridView1.DataSourceID = "ObjectDataSource1"

        ' ヘッダーを設定する。
        Me.gvwGridView1.Columns(0).HeaderText = "選択"
        Me.gvwGridView1.Columns(1).HeaderText = "ProductID"
        Me.gvwGridView1.Columns(2).HeaderText = "ProductName"
        Me.gvwGridView1.Columns(3).HeaderText = "SupplierID"
        Me.gvwGridView1.Columns(4).HeaderText = "CategoryID"
        Me.gvwGridView1.Columns(5).HeaderText = "QuantityPerUnit"
        Me.gvwGridView1.Columns(6).HeaderText = "UnitPrice"
        Me.gvwGridView1.Columns(7).HeaderText = "UnitsInStock"
        Me.gvwGridView1.Columns(8).HeaderText = "UnitsOnOrder"
        Me.gvwGridView1.Columns(9).HeaderText = "ReorderLevel"
        Me.gvwGridView1.Columns(10).HeaderText = "Discontinued"

        ' 画面遷移しない。
        Return String.Empty
    End Function

    ''' <summary>gvwGridView1のSortingイベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <param name="e">オリジナルのイベント引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_gvwGridView1_Sorting(fxEventArgs As FxEventArgs, e As GridViewSortEventArgs) As String
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
    Protected Sub gvwGridView1_SelectedIndexChanging(sender As Object, e As GridViewSelectEventArgs)
        ' 選択されたレコードの主キーとタイムスタンプ列を取得
        Dim dt As DataTable = DirectCast(Session("SearchResult"), DataTable)
        Dim PrimaryKeyAndTimeStamp As New Dictionary(Of String, Object)()

        ' 主キーとタイムスタンプ列
        ' 主キー列
        PrimaryKeyAndTimeStamp.Add("ProductID", dt.Rows(e.NewSelectedIndex)("ProductID").ToString())
        ' タイムスタンプ列
        ' ・・・

        Session("PrimaryKeyAndTimeStamp") = PrimaryKeyAndTimeStamp
    End Sub

    ''' <summary>gvwGridView1の行選択後イベント</summary>
    ''' <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ''' <returns>URL</returns>
    Protected Function UOC_gvwGridView1_SelectedIndexChanged(fxEventArgs As FxEventArgs) As String
        ' 画面遷移（詳細表示）
        Return "ProductsDetail.aspx"
    End Function

#End Region
End Class

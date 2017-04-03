'**********************************************************************************
'* テーブル・メンテナンス自動生成・テスト画面
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：_SearchAndUpdate_
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

Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Public.Db

Namespace Aspx.Sample._3Tier
    ''' <summary>三層データバインド・検索一覧更新画面</summary>
    Partial Public Class ProductsSearchAndUpdate
        Inherits MyBaseController
#Region "Page LoadのUOCメソッド"

        ''' <summary>
        ''' Page LoadのUOCメソッド（個別：初回Load）
        ''' </summary>
        ''' <remarks>
        ''' 実装必須
        ''' </remarks>
        Protected Overrides Sub UOC_FormInit()
            ' Form初期化（初回Load）時に実行する処理を実装する

            ' TODO:

            ' 更新Buttonの非活性化
            Me.btnBatUpd.Enabled = False

            '#Region "マスタ・データのロードと設定"

            ' 引数クラスを生成
            ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, "FormInit_PostBack", "Invoke", Me.ddlDap.SelectedValue, Me.UserInfo)

            ' B層を生成
            Dim getMasterData As New GetMasterData()

            ' 業務処理を実行
            Dim returnValue As _3TierReturnValue = getMasterData.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted)

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
        ''' Page LoadのUOCメソッド（個別：Post Back）
        ''' </summary>
        ''' <remarks>
        ''' 実装必須
        ''' </remarks>
        Protected Overrides Sub UOC_FormInit_PostBack()
            ' Form初期化（Post Back）時に実行する処理を実装する

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
                Session("ddldsdt_SupplierID") = value
            End Set
        End Property

        ''' <summary>DropDownList生成用のプロパティ</summary>
        Public Property ddldsdt_Categories() As DataTable
            Get
                Return DirectCast(Session("ddldsdt_CategoryID"), DataTable)
            End Get
            Set(value As DataTable)
                Session("ddldsdt_CategoryID") = value
            End Set
        End Property

#End Region

#End Region

#Region "Event Handler"

#Region "一覧検索"

        ''' <summary>検索Button</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnSearch_Click(fxEventArgs As FxEventArgs) As String

            ' 更新Buttonの非活性化
            Me.btnBatUpd.Enabled = False

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
            ' ページング
            Me.gvwGridView1.AllowPaging = True

            ' gvwGridView1をObjectDataSourceに連結。
            Me.gvwGridView1.DataSource = Nothing
            Me.gvwGridView1.DataSourceID = "ObjectDataSource1"

            ' ヘッダーを設定する。
            Me.gvwGridView1.Columns(0).HeaderText = "削除"
            Me.gvwGridView1.Columns(1).HeaderText = "更新"
            Me.gvwGridView1.Columns(2).HeaderText = "ProductID"
            Me.gvwGridView1.Columns(3).HeaderText = "ProductName"
            Me.gvwGridView1.Columns(4).HeaderText = "SupplierID"
            Me.gvwGridView1.Columns(5).HeaderText = "CategoryID"
            Me.gvwGridView1.Columns(6).HeaderText = "QuantityPerUnit"
            Me.gvwGridView1.Columns(7).HeaderText = "UnitPrice"
            Me.gvwGridView1.Columns(8).HeaderText = "UnitsInStock"
            Me.gvwGridView1.Columns(9).HeaderText = "UnitsOnOrder"
            Me.gvwGridView1.Columns(10).HeaderText = "ReorderLevel"
            Me.gvwGridView1.Columns(11).HeaderText = "Discontinued"

            ' 画面遷移しない。
            Return String.Empty
        End Function

        ''' <summary>gvwGridView1のSortingイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
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

#End Region

#Region "CRUD"

        ''' <summary>追加Button</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnInsert_Click(fxEventArgs As FxEventArgs) As String
            ' 画面遷移（詳細表示）
            Return "ProductsDetail.aspx"
        End Function

        ''' <summary>バッチ更新Button</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnBatUpd_Click(fxEventArgs As FxEventArgs) As String
            ' 引数クラスを生成
            Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "BatchUpdate", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

            ' テーブル
            parameterValue.TableName = "Products"

            ' 主キーとタイムスタンプ列
            parameterValue.AndEqualSearchConditions = New Dictionary(Of String, Object)()

            ' 主キー列
            parameterValue.AndEqualSearchConditions.Add("ProductID", "")

            ' タイムスタンプ列
            ' ・・・

            ' DataTableを設定
            parameterValue.Obj = DirectCast(Session("SearchResult"), DataTable)

            ' B層を生成
            Dim b As New _3TierEngine()

            ' データ取得処理を実行
            Dim returnValue As _3TierReturnValue = b.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted)

            ' 結果表示
            'Me.lblResult.Text = returnValue.Obj.ToString() + "件更新しました。";

            ' 更新Buttonの非活性化
            Me.btnBatUpd.Enabled = False

            ' 画面遷移しない。
            Return String.Empty
        End Function

        ''' <summary>gvwGridView1のコマンドイベント</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_gvwGridView1_RowCommand(fxEventArgs As FxEventArgs) As String
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
                                        dr(dc) = txtBox.Text
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

            ' 更新Buttonの活性化
            Me.btnBatUpd.Enabled = True

            ' 画面遷移しない。
            Return String.Empty
        End Function

#End Region

#End Region
    End Class
End Namespace
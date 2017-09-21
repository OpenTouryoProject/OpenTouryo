'**********************************************************************************
'* 三層データバインド・アプリ画面
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：_Detail_
'* クラス日本語名  ：三層データバインド・詳細表示画面（_TableName_）
'*
'* 作成日時        ：_TimeStamp_
'* 作成者          ：自動生成ツール（墨壺２）, _UserName_
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2015/12/22  Sai               Modified ReadOnly property of the primary key column textbox to true.  
'**********************************************************************************

Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Presentation
Imports Touryo.Infrastructure.Business.Common
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Presentation
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Public.Db

Namespace Aspx.Sample._3Tier
    ''' <summary>三層データバインド・詳細表示画面</summary>
    Partial Public Class ProductsDetail
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
            Dim parameterValue As _3TierParameterValue = Nothing
            Dim returnValue As _3TierReturnValue = Nothing

            If Session("PrimaryKeyAndTimeStamp") Is Nothing Then
                ' 追加処理のみ。
                Me.btnEdit.Enabled = False
                Me.btnUpdate.Enabled = False
                Me.btnDelete.Enabled = False

                ' 編集
                Me.SetControlReadOnly(False)
            Else
                ' 詳細表示処理

                ' 引数クラスを生成
                parameterValue = New _3TierParameterValue(Me.ContentPageFileNoEx, "FormInit", "SelectRecord", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

                ' テーブル
                parameterValue.TableName = "Products"

                ' 主キーとタイムスタンプ列
                parameterValue.AndEqualSearchConditions = DirectCast(Session("PrimaryKeyAndTimeStamp"), Dictionary(Of String, Object))

                ' B層を生成
                Dim b As New _3TierEngine()

                ' データ取得処理を実行
                returnValue = b.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted)

                ' 編集状態の初期化

                ' 値
                Me.txtProductID.Text = returnValue.Dt.Rows(0)("ProductID").ToString()
                Me.txtProductName.Text = returnValue.Dt.Rows(0)("ProductName").ToString()

                'Me.txtSupplierID.Text = returnValue.Dt.Rows(0)("SupplierID").ToString()
                Me.ddlSupplierID.SelectedValue = returnValue.Dt.Rows(0)("SupplierID").ToString()
                'Me.txtCategoryID.Text = returnValue.Dt.Rows(0)("CategoryID").ToString()
                Me.ddlCategoryID.SelectedValue = returnValue.Dt.Rows(0)("CategoryID").ToString()

                Me.txtQuantityPerUnit.Text = returnValue.Dt.Rows(0)("QuantityPerUnit").ToString()
                Me.txtUnitPrice.Text = returnValue.Dt.Rows(0)("UnitPrice").ToString()
                Me.txtUnitsInStock.Text = returnValue.Dt.Rows(0)("UnitsInStock").ToString()
                Me.txtUnitsOnOrder.Text = returnValue.Dt.Rows(0)("UnitsOnOrder").ToString()
                Me.txtReorderLevel.Text = returnValue.Dt.Rows(0)("ReorderLevel").ToString()
                Me.txtDiscontinued.Text = returnValue.Dt.Rows(0)("Discontinued").ToString()

                ' 編集
                Me.SetControlReadOnly(True)
            End If

            '#Region "マスタ・データのロードと設定"

            ' 引数クラスを生成
            ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            parameterValue = New _3TierParameterValue(Me.ContentPageFileNoEx, "FormInit_PostBack", "Invoke", Me.ddlDap.SelectedValue, Me.UserInfo)

            ' B層を生成
            Dim getMasterData As New GetMasterData()

            ' 業務処理を実行
            returnValue = getMasterData.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted)

            Dim dts As DataTable() = DirectCast(returnValue.Obj, DataTable())
            Dim dt As DataTable = Nothing
            Dim dr As DataRow = Nothing

            ' daoSuppliers
            _3TierEngine.CreateDropDownListDataSourceDataTable(dts(0), "SupplierID", "CompanyName", dt, "value", "text")

            dr = dt.NewRow()
            dr("value") = ""
            dr("text") = "empty"
            dt.Rows.Add(dr)
            dt.AcceptChanges()

            Me.ddlSupplierID.DataValueField = "value"
            Me.ddlSupplierID.DataTextField = "text"

            Me.ddlSupplierID.DataSource = dt
            Me.ddlSupplierID.DataBind()

            ' daoCategories
            _3TierEngine.CreateDropDownListDataSourceDataTable(dts(1), "CategoryID", "CategoryName", dt, "value", "text")

            dr = dt.NewRow()
            dr("value") = ""
            dr("text") = "empty"
            dt.Rows.Add(dr)
            dt.AcceptChanges()

            Me.ddlCategoryID.DataValueField = "value"
            Me.ddlCategoryID.DataTextField = "text"

            Me.ddlCategoryID.DataSource = dt
            Me.ddlCategoryID.DataBind()

            ' 追加時はSelectedValueに空の値を設定
            If Session("PrimaryKeyAndTimeStamp") Is Nothing Then
                Me.ddlSupplierID.SelectedValue = ""
                Me.ddlCategoryID.SelectedValue = ""
            End If

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
        End Sub

#End Region

#Region "Event Handler"

#Region "編集状態の変更"

        ''' <summary>編集Button</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnEdit_Click(fxEventArgs As FxEventArgs) As String
            ' 編集状態の変更

            ' 編集
            Me.SetControlReadOnly(False)

            ' 画面遷移しない。
            Return String.Empty
        End Function

#End Region

#Region "更新系"

        ''' <summary>追加Button</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnInsert_Click(fxEventArgs As FxEventArgs) As String
            ' 引数クラスを生成
            Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "InsertRecord", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

            ' テーブル
            parameterValue.TableName = "Products"

            ' 追加値（TimeStamp列は外す。主キーは採番方法次第。
            parameterValue.InsertUpdateValues = New Dictionary(Of String, Object)()
            'parameterValue.InsertUpdateValues.Add("ProductID", Me.txtProductID.Text)
            parameterValue.InsertUpdateValues.Add("ProductName", Me.txtProductName.Text)

            'parameterValue.InsertUpdateValues.Add("SupplierID", Me.txtSupplierID.Text)
            parameterValue.InsertUpdateValues.Add("SupplierID", Me.ddlSupplierID.SelectedValue)
            'parameterValue.InsertUpdateValues.Add("CategoryID", Me.txtCategoryID.Text)
            parameterValue.InsertUpdateValues.Add("CategoryID", Me.ddlCategoryID.SelectedValue)

            parameterValue.InsertUpdateValues.Add("QuantityPerUnit", Me.txtQuantityPerUnit.Text)
            parameterValue.InsertUpdateValues.Add("UnitPrice", Me.txtUnitPrice.Text)
            parameterValue.InsertUpdateValues.Add("UnitsInStock", Me.txtUnitsInStock.Text)
            parameterValue.InsertUpdateValues.Add("UnitsOnOrder", Me.txtUnitsOnOrder.Text)
            parameterValue.InsertUpdateValues.Add("ReorderLevel", Me.txtReorderLevel.Text)
            parameterValue.InsertUpdateValues.Add("Discontinued", Me.txtDiscontinued.Text)

            ' B層を生成
            Dim b As New _3TierEngine()

            ' データ取得処理を実行
            Dim returnValue As _3TierReturnValue = b.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted)

            ' 結果表示
            Me.lblResult.Text = returnValue.Obj.ToString() & "件追加しました。"

            ' 画面遷移しない。
            Return String.Empty
        End Function

        ''' <summary>更新Button</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnUpdate_Click(fxEventArgs As FxEventArgs) As String
            ' 引数クラスを生成
            Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "UpdateRecord", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

            ' テーブル
            parameterValue.TableName = "Products"

            ' 主キーとタイムスタンプ列
            parameterValue.AndEqualSearchConditions = DirectCast(Session("PrimaryKeyAndTimeStamp"), Dictionary(Of String, Object))

            ' 更新値（TimeStamp列は外す。主キーは採番方法次第。
            parameterValue.InsertUpdateValues = New Dictionary(Of String, Object)()
            'parameterValue.InsertUpdateValues.Add("ProductID", Me.txtProductID.Text)
            parameterValue.InsertUpdateValues.Add("ProductName", Me.txtProductName.Text)

            'parameterValue.InsertUpdateValues.Add("SupplierID", Me.txtSupplierID.Text)
            parameterValue.InsertUpdateValues.Add("SupplierID", Me.ddlSupplierID.SelectedValue)
            'parameterValue.InsertUpdateValues.Add("CategoryID", Me.txtCategoryID.Text)
            parameterValue.InsertUpdateValues.Add("CategoryID", Me.ddlCategoryID.SelectedValue)

            parameterValue.InsertUpdateValues.Add("QuantityPerUnit", Me.txtQuantityPerUnit.Text)
            parameterValue.InsertUpdateValues.Add("UnitPrice", Me.txtUnitPrice.Text)
            parameterValue.InsertUpdateValues.Add("UnitsInStock", Me.txtUnitsInStock.Text)
            parameterValue.InsertUpdateValues.Add("UnitsOnOrder", Me.txtUnitsOnOrder.Text)
            parameterValue.InsertUpdateValues.Add("ReorderLevel", Me.txtReorderLevel.Text)
            parameterValue.InsertUpdateValues.Add("Discontinued", Me.txtDiscontinued.Text)

            ' B層を生成
            Dim b As New _3TierEngine()

            ' データ取得処理を実行
            Dim returnValue As _3TierReturnValue = b.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted)

            ' 結果表示
            Me.lblResult.Text = returnValue.Obj.ToString() & "件更新しました。"

            ' 画面遷移しない。
            Return String.Empty
        End Function

        ''' <summary>削除Button</summary>
        ''' <param name="fxEventArgs">Event Handlerの共通引数</param>
        ''' <returns>URL</returns>
        Protected Function UOC_btnDelete_Click(fxEventArgs As FxEventArgs) As String
            ' 引数クラスを生成
            Dim parameterValue As New _3TierParameterValue(Me.ContentPageFileNoEx, fxEventArgs.ButtonID, "DeleteRecord", DirectCast(Session("DAP"), String), DirectCast(Me.UserInfo, MyUserInfo))

            ' テーブル
            parameterValue.TableName = "Products"

            ' 主キーとタイムスタンプ列
            parameterValue.AndEqualSearchConditions = DirectCast(Session("PrimaryKeyAndTimeStamp"), Dictionary(Of String, Object))

            ' B層を生成
            Dim b As New _3TierEngine()

            ' データ取得処理を実行
            Dim returnValue As _3TierReturnValue = b.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted)

            ' 結果表示
            Me.lblResult.Text = returnValue.Obj.ToString() & "件削除しました。"

            ' 画面遷移しない。
            Return String.Empty
        End Function

#End Region

        ''' <summary>編集可否の制御</summary>
        ''' <param name="readOnly">読取専用プロパティ</param>
        Private Sub SetControlReadOnly([readOnly] As Boolean)
            ' 編集可否
            ' ReadOnly

            ' 主キー
            Me.txtProductID.[ReadOnly] = True

            ' 主キー以外
            Me.txtProductName.[ReadOnly] = [readOnly]

            'Me.txtSupplierID.ReadOnly = readOnly;
            Me.ddlSupplierID.[ReadOnly] = [readOnly]
            'Me.txtCategoryID.ReadOnly = readOnly;
            Me.ddlCategoryID.[ReadOnly] = [readOnly]

            Me.txtQuantityPerUnit.[ReadOnly] = [readOnly]
            Me.txtUnitPrice.[ReadOnly] = [readOnly]
            Me.txtUnitsInStock.[ReadOnly] = [readOnly]
            Me.txtUnitsOnOrder.[ReadOnly] = [readOnly]
            Me.txtReorderLevel.[ReadOnly] = [readOnly]
            Me.txtDiscontinued.[ReadOnly] = [readOnly]

            ' 背景色
            ' BackColor
            Dim backColor As System.Drawing.Color

            If [readOnly] Then
                backColor = System.Drawing.Color.LightGray
            Else
                backColor = System.Drawing.Color.Empty
            End If

            ' 主キー
            Me.txtProductID.BackColor = System.Drawing.Color.LightGray

            ' 主キー以外
            Me.txtProductName.BackColor = backColor

            'Me.txtSupplierID.BackColor = backColor;
            Me.ddlSupplierID.BackColor = backColor
            'Me.txtCategoryID.BackColor = backColor;
            Me.ddlCategoryID.BackColor = backColor

            Me.txtQuantityPerUnit.BackColor = backColor
            Me.txtUnitPrice.BackColor = backColor
            Me.txtUnitsInStock.BackColor = backColor
            Me.txtUnitsOnOrder.BackColor = backColor
            Me.txtReorderLevel.BackColor = backColor
            Me.txtDiscontinued.BackColor = backColor
        End Sub

#End Region
    End Class
End Namespace

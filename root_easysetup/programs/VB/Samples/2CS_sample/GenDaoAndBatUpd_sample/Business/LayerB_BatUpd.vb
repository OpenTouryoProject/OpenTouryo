'**********************************************************************************
'* フレームワーク・テストクラス（Ｂ層）
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：LayerB_BatUpd
'* クラス日本語名  ：Ｂ層（静的SQLのCRUD：Productsテーブル）
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

' 型情報
Imports GenDaoAndBatUpd_sample.Common

' System
Imports System

' データセット利用
Imports System.Data

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.RichClient.Business

' フレームワーク
Imports Touryo.Infrastructure.Framework.Business
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Framework.Exceptions
Imports Touryo.Infrastructure.Framework.RichClient.Business

' 部品
Imports Touryo.Infrastructure.Public.Util

Namespace Business
    ''' <summary>Ｂ層（静的SQLのCRUD：Productsテーブル）</summary>
    Class LayerB_BatUpd
        Inherits MyFcBaseLogic2CS
        ''' <summary>業務処理を実装</summary>
        ''' <param name="parameterValue">引数クラス</param>
        Private Sub UOC_BatUpd(ByVal parameterValue As BaseParameterValue)
            ' 引数クラスをアップキャスト
            Dim testParameter As TestParameterValue = DirectCast(parameterValue, TestParameterValue)

            ' 戻り値クラスを生成
            Dim testReturn As New TestReturnValue()

            ' ↓業務処理-----------------------------------------------------

            ' データアクセス クラスを生成する
            Dim daoProducts As New DaoProducts(Me.GetDam())

            ' ROW毎に処理
            For Each dr As DataRow In testParameter.dt.Rows
                ' パラメタをクリアする。
                daoProducts.ClearParametersFromHt()

                Select Case dr.RowState
                    Case DataRowState.Added

                        '#Region "１件挿入"

                        ' 設定（インサート値）
                        daoProducts.PK_ProductID = dr("ProductID").ToString()
                        daoProducts.ProductName = dr("ProductName").ToString()
                        daoProducts.SupplierID = dr("SupplierID").ToString()
                        daoProducts.CategoryID = dr("CategoryID").ToString()
                        daoProducts.QuantityPerUnit = dr("QuantityPerUnit").ToString()
                        daoProducts.UnitPrice = dr("UnitPrice").ToString()
                        daoProducts.UnitsInStock = dr("UnitsInStock").ToString()
                        daoProducts.UnitsOnOrder = dr("UnitsOnOrder").ToString()
                        daoProducts.ReorderLevel = dr("ReorderLevel").ToString()
                        daoProducts.Discontinued = dr("Discontinued").ToString()

                        ' インサート（S1でよい）
                        testReturn.obj = daoProducts.S1_Insert()

                        '#End Region

                        Exit Select

                    Case DataRowState.Deleted

                        '#Region "１件削除"

                        ' 設定（主キー）
                        daoProducts.PK_ProductID = dr("ProductID", DataRowVersion.Original).ToString()
                        ' ★ 楽観排他をする場合は、ここにタイムスタンプを追加する。

                        ' デリート（タイムスタンプを指定する場合は、D4_Delete）
                        testReturn.obj = daoProducts.D4_Delete()

                        '#End Region

                        Exit Select

                    Case DataRowState.Modified

                        '#Region "１件更新"

                        ' 設定（主キー）
                        daoProducts.PK_ProductID = dr("ProductID").ToString()

                        ' ★ 楽観排他をする場合は、ここにタイムスタンプを追加する。
                        ' ↓は、DataRowVersion.Originalを使用した楽観排他の例
                        daoProducts.ProductName = dr("ProductName", DataRowVersion.Original).ToString()
                        daoProducts.SupplierID = dr("SupplierID", DataRowVersion.Original).ToString()
                        daoProducts.CategoryID = dr("CategoryID", DataRowVersion.Original).ToString()
                        daoProducts.QuantityPerUnit = dr("QuantityPerUnit", DataRowVersion.Original).ToString()
                        daoProducts.UnitPrice = dr("UnitPrice", DataRowVersion.Original).ToString()
                        daoProducts.UnitsInStock = dr("UnitsInStock", DataRowVersion.Original).ToString()
                        daoProducts.UnitsOnOrder = dr("UnitsOnOrder", DataRowVersion.Original).ToString()
                        daoProducts.ReorderLevel = dr("ReorderLevel", DataRowVersion.Original).ToString()
                        daoProducts.Discontinued = dr("Discontinued", DataRowVersion.Original).ToString()

                        ' 更新値設定
                        daoProducts.Set_ProductName_forUPD = dr("ProductName").ToString()
                        daoProducts.Set_SupplierID_forUPD = dr("SupplierID").ToString()
                        daoProducts.Set_CategoryID_forUPD = dr("CategoryID").ToString()
                        daoProducts.Set_QuantityPerUnit_forUPD = dr("QuantityPerUnit").ToString()
                        daoProducts.Set_UnitPrice_forUPD = dr("UnitPrice").ToString()
                        daoProducts.Set_UnitsInStock_forUPD = dr("UnitsInStock").ToString()
                        daoProducts.Set_UnitsOnOrder_forUPD = dr("UnitsOnOrder").ToString()
                        daoProducts.Set_ReorderLevel_forUPD = dr("ReorderLevel").ToString()
                        daoProducts.Set_Discontinued_forUPD = dr("Discontinued").ToString()

                        ' アップデート（タイムスタンプを指定する場合は、D3_Update）
                        testReturn.obj = daoProducts.D3_Update()

                        '#End Region

                        Exit Select
                    Case Else

                        Exit Select
                End Select
            Next

            ' ↑業務処理-----------------------------------------------------

            ' 戻り値クラスをダウンキャストして戻す
            Me.ReturnValue = DirectCast(testReturn, BaseReturnValue)
        End Sub

        ''' <summary>業務処理を実装</summary>
        ''' <param name="parameterValue">引数クラス</param>
        Private Sub UOC_SelectAll(ByVal parameterValue As BaseParameterValue)
            ' 引数クラスをアップキャスト
            Dim testParameter As TestParameterValue = DirectCast(parameterValue, TestParameterValue)

            ' 戻り値クラスを生成
            Dim testReturn As New TestReturnValue()

            ' ↓業務処理-----------------------------------------------------

            ' データアクセス クラスを生成する
            Dim daoProducts As New DaoProducts(Me.GetDam())

            ' 全件取得
            Dim dt As New DataTable()
            daoProducts.D2_Select(dt)

            ' 戻り値を戻す
            testReturn.dt = dt

            ' ↑業務処理-----------------------------------------------------

            ' 戻り値クラスをダウンキャストして戻す
            Me.ReturnValue = DirectCast(testReturn, BaseReturnValue)
        End Sub
    End Class
End Namespace

'**********************************************************************************
'* フレームワーク・テストクラス（Ｂ層）
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：LayerB
'* クラス日本語名  ：Ｂ層のテスト
'*
'* 作成日時        ：－
'* 作成者          ：生技セ
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*
'**********************************************************************************

' 型情報
Imports RerunnableBatch_sample.Common

' System
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic

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

Namespace Business

    ''' <summary>
    ''' LayerB の概要の説明です
    ''' </summary>
    Public Class LayerB
        Inherits MyFcBaseLogic

#Region "UOCメソッド"

#Region "SelectPkList"

        ''' <summary>主キー一覧を取得</summary>
        ''' <param name="parameter">引数クラス</param>
        Private Sub UOC_SelectPkList(ByVal parameter As VoidParameterValue)
            ' 戻り値クラスを生成して、事前に戻り値に設定しておく。
            Dim returnValue As New SelectPkListReturnValue()
            Me.ReturnValue = returnValue

            ' ↓業務処理-----------------------------------------------------

            Dim pkTable As New DataTable()

            ' ↓DBアクセス-----------------------------------------------------
            ' 共通Daoを生成
            Dim cmnDao As New CmnDao(Me.GetDam())

            ' 動的SQLを指定
            cmnDao.SQLFileName = "SelectAllOrderID.xml"

            ' 共通Daoを実行
            cmnDao.ExecSelectFill_DT(pkTable)
            ' ↑DBアクセス-----------------------------------------------------

            ' 戻り値を設定
            Dim pkList As New ArrayList()
            For index As Integer = 0 To pkTable.Rows.Count - 1
                'データテーブルからArrayListに詰め直す
                pkList.Add(pkTable.Rows(index)("OrderID"))
            Next
            returnValue.PkList = pkList

            ' ↑業務処理-----------------------------------------------------

        End Sub

#End Region

#Region "ExecuteBatchProcess"

        ''' <summary>バッチ処理を実行する</summary>
        ''' <param name="parameter">引数クラス</param>
        Private Sub UOC_ExecuteBatchProcess(ByVal parameter As ExecuteBatchProcessParameterValue)
            ' 戻り値クラスを生成して、事前に戻り値に設定しておく。
            Me.ReturnValue = New VoidReturnValue()

            ' ↓業務処理-----------------------------------------------------

            Dim pkList As ArrayList = parameter.SubPkList   '主キー一覧(1トランザクション分)
            Dim dataTable As New DataTable()    'データ一覧(主キーを元に検索したデータ)

            'Ordersテーブルからデータを検索する
            ' ↓DBアクセス-----------------------------------------------------
            ' 共通Daoを生成
            Dim cmnDao As New CmnDao(Me.GetDam())

            ' 動的SQLを指定
            cmnDao.SQLFileName = "SelectInOrderID.xml"

            ' パラメータを設定
            cmnDao.SetParameter("OrderID", pkList)

            ' 共通Daoを実行
            cmnDao.ExecSelectFill_DT(dataTable)
            ' ↑DBアクセス-----------------------------------------------------


            'Orders2テーブルに1件ずつ追加する
            For index As Integer = 0 To dataTable.Rows.Count - 1
                Dim row As DataRow = dataTable.Rows(index)  '1件分のデータ

                'todo：編集処理など

                ' ↓DBアクセス-----------------------------------------------------
                ' 自動生成Daoを生成
                Dim dao As New DaoOrders2(Me.GetDam())

                ' パラメータを設定
                dao.PK_OrderID = row("OrderID")
                dao.CustomerID = row("CustomerID")
                dao.EmployeeID = row("EmployeeID")
                dao.OrderDate = row("OrderDate")
                dao.RequiredDate = row("RequiredDate")
                dao.ShippedDate = row("ShippedDate")
                dao.ShipVia = row("ShipVia")
                dao.Freight = row("Freight")
                dao.ShipName = row("ShipName")
                dao.ShipAddress = row("ShipAddress")
                dao.ShipCity = row("ShipCity")
                dao.ShipRegion = row("ShipRegion")
                dao.ShipPostalCode = row("ShipPostalCode")
                dao.ShipCountry = row("ShipCountry")

                ' 共通Daoを実行
                dao.S1_Insert()

                ' ↑DBアクセス-----------------------------------------------------
            Next

            ' todo:中間コミット情報をDBに登録 ※最終処理主キー値の登録など

            ' ↑業務処理-----------------------------------------------------
        End Sub

#End Region

#End Region
    End Class
End Namespace

'**********************************************************************************
'* フレームワーク・テストクラス（Ｂ層）
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

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
'**********************************************************************************

Imports RerunnableBatch_sample3.Common

Imports System
Imports System.Data
Imports System.Text
Imports System.Collections

Imports Touryo.Infrastructure.Business.Business
Imports Touryo.Infrastructure.Business.Dao
Imports Touryo.Infrastructure.Public.Db

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

            Dim pkList As ArrayList = parameter.SubPkList
            '主キー一覧(1トランザクション分)
            Dim dataTable As New DataTable()
            'データ一覧(主キーを元に検索したデータ)
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


            'Orders2テーブルに複数件まとめて追加する。
			Dim insertHeader As String = "INSERT INTO [Orders2] {0} VALUES {1}"
			Dim columnList As String = ""

			Dim sQLUtility As New SQLUtility(DbEnum.DBMSType.SQLServer)
			Dim insertSQLParts As String() = sQLUtility.GetInsertSQLParts(dataTable)

			Dim sb As New StringBuilder()
			For Each insertSQLPart As String In insertSQLParts
				If String.IsNullOrEmpty(columnList) Then
					' columnList
					columnList = insertSQLPart
				Else
					' insertSQLPart
					sb.Append(String.Format(insertHeader, columnList, insertSQLPart) & Environment.NewLine)
				End If
            Next

            ' 共通Daoでバッチ更新
            Dim cd As New CmnDao(Me.GetDam())
            cd.SQLText = sb.ToString()
            cd.ExecInsUpDel_NonQuery()

            ' todo:中間コミット情報をDBに登録 ※最終処理主キー値の登録など

            ' ↑業務処理-----------------------------------------------------
        End Sub

#End Region

#End Region

    End Class
End Namespace

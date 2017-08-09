'**********************************************************************************
'* テーブル・メンテナンス自動生成・テストクラス（TableAdapter
'**********************************************************************************

' テスト用クラスなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：_TableAdapterClassName_
'* クラス日本語名  ：三層データバインド・カスタムTableAdapter（_TableName_）
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
Imports Touryo.Infrastructure.Framework.Common
Imports Touryo.Infrastructure.Public.Db

''' <summary>三層データバインド・カスタムTableAdapter（_TableName_）</summary>
Public Class ProductsTableAdapter
	Inherits CmnTableAdapter
	''' <summary>データ件数取得処理を実装</summary>
	''' <returns>データ件数</returns>
	Public Function SelectCountMethod() As Integer
		Dim returnValue As _3TierReturnValue = Nothing

		Try
			' ここのデータアクセス処理は棟梁のB層を呼び出して実装する。

			' ユーザ情報を取得
			Dim myUserInfo As MyUserInfo = MyBaseController.GetUserInfo2()

			' 引数クラスを生成
			Dim parameterValue As _3TierParameterValue = Me.CreateParameter("Products", "SelectCountMethod", myUserInfo)

			' B層を生成
			Dim b As New _3TierEngine()

            ' データ件数取得処理を実行
            returnValue = b.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted)
        Catch ex As Exception
			MyBaseController.TransferErrorScreen2(ex)
			Return 0
		End Try

		' データ件数を返却
		'（OracleでdecimalになるケースがあるのでParseしている。）
		Return Integer.Parse(returnValue.Obj.ToString())
	End Function

	''' <summary>データ取得処理を実装</summary>
	''' <param name="startRowIndex">開始位置</param>
	''' <param name="maximumRows">取得行数</param>
	''' <returns>DataTable</returns>
	Public Function SelectMethod(startRowIndex As Integer, maximumRows As Integer) As DataTable
		Dim returnValue As _3TierReturnValue = Nothing

		Try
			' ここのデータアクセス処理は棟梁のB層を呼び出して実装する。

			' ユーザ情報を取得
			Dim myUserInfo As MyUserInfo = MyBaseController.GetUserInfo2()

			' 引数クラスを生成
			Dim parameterValue As _3TierParameterValue = Me.CreateParameter("Products", "SelectMethod", myUserInfo)

            ' カラムリスト（射影
            parameterValue.ColumnList =
                "_s_ProductID_e_, " &
                "_s_ProductName_e_, " &
                "_s_SupplierID_e_, " &
                "_s_CategoryID_e_, " &
                "_s_QuantityPerUnit_e_, " &
                "_s_UnitPrice_e_, " &
                "_s_UnitsInStock_e_, " &
                "_s_UnitsOnOrder_e_, " &
                "_s_ReorderLevel_e_, " &
                "_s_Discontinued_e_"

            ' ソート条件
            parameterValue.SortExpression = DirectCast(HttpContext.Current.Session("SortExpression"), String)
			parameterValue.SortDirection = DirectCast(HttpContext.Current.Session("SortDirection"), String)

			' ページング条件
			parameterValue.MaximumRows = maximumRows
			parameterValue.StartRowIndex = startRowIndex

			' B層を生成
			Dim b As New _3TierEngine()

            ' データ取得処理を実行
            returnValue = b.DoBusinessLogic(parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted)

            ' GridView1.DataSourceから取得できなかったのでSessionに格納。
            HttpContext.Current.Session("SearchResult") = returnValue.Dt
		Catch ex As Exception
			MyBaseController.TransferErrorScreen2(ex)
			Return Nothing
		End Try

		' 取得データを返却
		Return returnValue.Dt
	End Function
End Class

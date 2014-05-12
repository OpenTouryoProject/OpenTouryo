'**********************************************************************************
'* 3層データバインド・カスタムTableAdapter
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：_TableName_TableAdapter
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

''' <summary>三層データバインド・カスタムTableAdapter（_TableName_）</summary>
Public Class _TableName_TableAdapter
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
			Dim parameterValue As _3TierParameterValue = Me.CreateParameter("_TableName_", "SelectCountMethod", myUserInfo)

			' B層を生成
			Dim b As New _3TierEngine()

			' データ件数取得処理を実行
			returnValue = DirectCast(b.DoBusinessLogic(DirectCast(parameterValue, BaseParameterValue), DbEnum.IsolationLevelEnum.ReadCommitted), _3TierReturnValue)
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
			Dim parameterValue As _3TierParameterValue = Me.CreateParameter("_TableName_", "SelectMethod", myUserInfo)

			' カラムリスト（射影
			parameterValue.ColumnList = "_AllColumnListTableAdapterSQL_"

			' ソート条件
			parameterValue.SortExpression = DirectCast(HttpContext.Current.Session("SortExpression"), String)
			parameterValue.SortDirection = DirectCast(HttpContext.Current.Session("SortDirection"), String)

			' ページング条件
			parameterValue.MaximumRows = maximumRows
			parameterValue.StartRowIndex = startRowIndex

			' B層を生成
			Dim b As New _3TierEngine()

			' データ取得処理を実行
			returnValue = DirectCast(b.DoBusinessLogic(DirectCast(parameterValue, BaseParameterValue), DbEnum.IsolationLevelEnum.ReadCommitted), _3TierReturnValue)

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

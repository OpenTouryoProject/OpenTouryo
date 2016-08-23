'**********************************************************************************
'* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
'**********************************************************************************

#Region "Apache License"
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License. 
' You may obtain a copy of the License at
'
' http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.
'
#End Region

'**********************************************************************************
'* クラス名        ：_3TierEngine
'* クラス日本語名  ：三層データバインド用の業務コードクラス・エンジン
'*
'* 作成者          ：生技 西野
'* 更新履歴        ：
'* 
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2013/01/10  西野　大介        新規作成
'*  2014/07/14  西野　大介        関連チェック処理を実装可能に
'*  2014/07/17  Sai-San           Added Select count query and select paging query constants and checks for PostgreSQL db support 
'*                                Added UOC_RelatedCheck override method and method calls in methods
'*                                'UOC_InsertRecord', 'UOC_UpdateRecord', 'UOC_DeleteRecord' and 'UOC_BatchUpdate' 
'*  2014/07/21  Rituparna         Added SelectCount and SelectPaging query constatnts and check for MySql db support
'*
'*  2014/08/14  Santosh Avaji     Added and modidfied code for DB2 support
'*  2014/12/10  西野　大介        Modified because there was a problem with the SELECT_PAGING_SQL_TEMPLATE_ORACLE.
'*  2014/12/10  西野　大介        Implementations of the related check process has been changed for problem.
'*                                Change the signature of the CRUD methods. "private" ---> "protected virtual"
'*  2015/04/29  Sandeep          Modified the code of 'UOC_SelectMethod' to retrive 30 records instead of 31 records
'*  2016/06/14  Shashikiran      Implemented 'UOC_UpdateRecordDM' method to perform multiple table update in single transaction
'*  2016/06/21  Shashikiran      Implemented 'UOC_BatchUpdateDM' method to perform multiple table Batch update in single transaction
'*  2016/06/27  Shashikiran      Implemented 'UOC_DeleteRecordDM' method to perform multiple table Delete in single transaction
'*  2016/06/27  Shashikiran      Modified 'UOC_BatchUpdateDM' method to perform multiple table Batch delete operation in single transaction
'**********************************************************************************

' レイトバインド用
Imports System.Reflection

' System
Imports System
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

Namespace Touryo.Infrastructure.Business.Business
	''' <summary>三層データバインド用の業務コードクラス・エンジン</summary>
	Public Class _3TierEngine
		Inherits MyFcBaseLogic
		#Region "定数・変数"

		#Region "テンプレート類"

		''' <summary>データ件数取得SQLテンプレート</summary>
		Private Const SELECT_COUNT_SQL_TEMPLATE As String = "SELECT COUNT(*) FROM {0} {1}"

		''' <summary>Select count query for PostgreSQL</summary>
		Private Const SELECT_COUNT_POSTGRESQL_TEMPLATE As String = "SELECT COUNT(*) FROM ""{0}"" {1}"

		''' <summary>データ取得SQLテンプレート（DBMSによって可変となる）</summary>
		''' <remarks>SQL Server用</remarks>
		Private Const SELECT_PAGING_SQL_TEMPLATE_SQL_SERVER As String = "WITH [OrderedTable] AS (SELECT {0}, ROW_NUMBER() OVER (ORDER BY [{1}] {2}) [rnum] FROM {3} {4}) SELECT {0} FROM [OrderedTable] WHERE [rnum] BETWEEN {5} AND {6}"

		''' <summary>Select query for PostGreSQL</summary>
		Private Const SELECT_PAGING_SQL_TEMPLATE_POSTGRESQL As String = "SELECT {0} FROM ( SELECT {0}, ROW_NUMBER() OVER (ORDER BY ""{1}"" {2}) ""RNUM"" FROM ""{3}"" {4} ) AS Temp WHERE ""RNUM"" BETWEEN {5} AND {6}"

		''' <summary>データ取得SQLテンプレート（DBMSによって可変となる）</summary>
		''' <remarks>Oracle用</remarks>
		Private Const SELECT_PAGING_SQL_TEMPLATE_ORACLE As String = "SELECT {0} FROM ( SELECT {0}, ROW_NUMBER() OVER (ORDER BY ""{1}"" {2}) ""RNUM"" FROM {3} {4} ) WHERE ""RNUM"" BETWEEN {5} AND {6}"
		'"SELECT {0} FROM ( SELECT {0}, ROW_NUMBER() OVER (ORDER BY \"{1}\" {2}) \"RNUM\" FROM \"{3}\" {4} ) WHERE \"RNUM\" BETWEEN {5} AND {6}";

		''' <summary>Selectpaging query from Mysql Database</summary>
		Private Const SELECT_PAGING_MYSQL_TEMPLATE As String = "SELECT * FROM(SELECT * FROM ( SELECT *,  @i := @i + 1 AS RESULT FROM {3},(SELECT @i := 0) TEMP ORDER BY ""{1}""  {2}) TEMP1 {4})TEMP3 WHERE RESULT BETWEEN {5} AND {6}"

		''' <summary>Select Paging Query For DB2 database</summary>
		Private Const SELECT_PAGING_DB2_TEMPLATE As String = "SELECT * FROM (SELECT {0}, ROW_NUMBER() OVER (ORDER BY {1} {2}) AS ROWNUM FROM {3} {4}) WHERE ROWNUM BETWEEN {5} AND {6}"

		''' <summary>Where句生成SQLテンプレート（＝）</summary>
		Private Const WHERE_SQL_TEMPLATE_EQUAL As String = "_s__ColName__e_ = _p__ParamName_"

		''' <summary>Where句生成SQLテンプレート（Like）</summary>
		Private Const WHERE_SQL_TEMPLATE_LIKE As String = "_s__ColName__f__e_ Like _p__ParamName_"

		#End Region

		#Region "墨壷2のパラメタ"

		' Daoクラス名のヘッダ・フッタ
		Private DaoClassNameHeader As String
		Private DaoClassNameFooter As String

		' メソッド名のヘッダ・フッタ（静的）
		Private MethodNameHeaderS As String
		Private MethodNameFooterS As String

		' メソッド名のCRUD名
		Private MethodLabel_Ins As String
		Private MethodLabel_Sel As String
		Private MethodLabel_Upd As String
		Private MethodLabel_Del As String
		Private MethodLabel_SelCnt As String

		' メソッド名のヘッダ・フッタ（動的）
		Private MethodNameHeaderD As String
		Private MethodNameFooterD As String

		' Updateのパラメタ名のヘッダ・フッタ
		Private UpdateParamHeader As String
		Private UpdateParamFooter As String

		' Likeのパラメタ名のヘッダ・フッタ
		Private LikeParamHeader As String
		Private LikeParamFooter As String

		#End Region

		#End Region

		#Region "初期化"

		''' <summary>コンストラクタ</summary>
		Public Sub New()
			' Daoクラス名のヘッダ・フッタ
			Me.DaoClassNameHeader = GetConfigParameter.GetConfigValue("DaoClassNameHeader")
			Me.DaoClassNameFooter = GetConfigParameter.GetConfigValue("DaoClassNameFooter")

			' メソッド名のヘッダ・フッタ（静的）
			Me.MethodNameHeaderS = GetConfigParameter.GetConfigValue("MethodNameHeaderS")
			Me.MethodNameFooterS = GetConfigParameter.GetConfigValue("MethodNameFooterS")

			' メソッド名のCRUD名
			Me.MethodLabel_Ins = GetConfigParameter.GetConfigValue("MethodLabel_Ins")
			Me.MethodLabel_Sel = GetConfigParameter.GetConfigValue("MethodLabel_Sel")
			Me.MethodLabel_Upd = GetConfigParameter.GetConfigValue("MethodLabel_Upd")
			Me.MethodLabel_Del = GetConfigParameter.GetConfigValue("MethodLabel_Del")
			Me.MethodLabel_SelCnt = GetConfigParameter.GetConfigValue("MethodLabel_SelCnt")

			' メソッド名のヘッダ・フッタ（動的）
			Me.MethodNameHeaderD = GetConfigParameter.GetConfigValue("MethodNameHeaderD")
			Me.MethodNameFooterD = GetConfigParameter.GetConfigValue("MethodNameFooterD")

			' Updateのパラメタ名のヘッダ・フッタ
			Me.UpdateParamHeader = GetConfigParameter.GetConfigValue("UpdateParamHeader")
			Me.UpdateParamFooter = GetConfigParameter.GetConfigValue("UpdateParamFooter")

			' Likeのパラメタ名のヘッダ・フッタ
			Me.LikeParamHeader = GetConfigParameter.GetConfigValue("LikeParamHeader")
			Me.LikeParamFooter = GetConfigParameter.GetConfigValue("LikeParamFooter")
		End Sub

		''' <summary>ListItem用のDatatableを生成する</summary>
		''' <param name="pdt">引数Datatable</param>
		''' <param name="pvalueIndex">値のIndex</param>
		''' <param name="ptextIndex">表示名のIndex</param>
		''' <param name="rdt">DropDownListItemのDatatable</param>
		''' <param name="rvalueIndex">値のIndex</param>
		''' <param name="rtextIndex">表示名のIndex</param>
		Public Shared Sub CreateDropDownListDataSourceDataTable(pdt As DataTable, pvalueIndex As String, ptextIndex As String, ByRef rdt As DataTable, rvalueIndex As String, rtextIndex As String)
			' 列生成
			rdt = New DataTable()
			' 自動生成テンプレートのベースがテキスト・ボックスなのでStringで良い。
			rdt.Columns.Add(New DataColumn(rvalueIndex, GetType([String])))
			rdt.Columns.Add(New DataColumn(rtextIndex, GetType([String])))

			' 行生成
			For Each pdr As DataRow In pdt.Rows
				Dim rdr As DataRow = rdt.NewRow()
				rdr(rvalueIndex) = pdr(pvalueIndex).ToString()
				rdr(rtextIndex) = pdr(pvalueIndex).ToString() & " : " & pdr(ptextIndex).ToString()
				rdt.Rows.Add(rdr)
			Next

			' 変更のコミット
			rdt.AcceptChanges()
		End Sub

		#End Region

		#Region "テンプレ"

		''' <summary>業務処理を実装</summary>
		''' <param name="parameterValue">引数クラス</param>
		Private Sub UOC_メソッド名(parameterValue As BaseParameterValue)
			' 戻り値クラスを生成して、事前に戻り地に設定しておく。
			Dim testReturn As New _3TierReturnValue()
			Me.ReturnValue = testReturn

			' ↓業務処理-----------------------------------------------------

			' 共通Dao
			Dim cmnDao As New CmnDao(Me.GetDam())
			cmnDao.ExecSelectScalar()

			' ↑業務処理-----------------------------------------------------
		End Sub

		#End Region

		#Region "一覧系"

		''' <summary>データ件数取得処理を実装</summary>
		''' <param name="parameterValue">引数クラス</param>
		Private Sub UOC_SelectCountMethod(parameterValue As _3TierParameterValue)
			' 戻り値クラスを生成して、事前に戻り地に設定しておく。
			Dim returnValue As New _3TierReturnValue()
			Me.ReturnValue = returnValue

			' ↓業務処理-----------------------------------------------------

			' 共通Dao
			Dim cmnDao As New CmnDao(Me.GetDam())

			' 検索条件の生成＆指定
			Dim whereSQL As String = Me.SetSearchConditions(parameterValue, cmnDao)

			Dim p As String = ""
			' パラメタ記号
			Dim s As String = ""
			' 囲い記号開始
			Dim e As String = ""
			' 囲い記号終了
			Dim f As String = ""
			' For supporting type casting in PostgreSQL
			' 囲い文字の選択
			If parameterValue.DBMSType = DbEnum.DBMSType.SQLServer Then
				p = "@"
				s = "["
				e = "]"
			'MYSQL and DB2
			ElseIf parameterValue.DBMSType = DbEnum.DBMSType.MySQL OrElse parameterValue.DBMSType = DbEnum.DBMSType.DB2 Then
				p = "@"
				s = """"
				e = """"
			ElseIf parameterValue.DBMSType = DbEnum.DBMSType.Oracle Then
				p = ":"
				s = """"
				e = """"
			ElseIf parameterValue.DBMSType = DbEnum.DBMSType.PstGrS Then
				p = "@"
				f = "::text"
			Else
				p = "@"
				s = "["
				e = "]"
			End If

			If parameterValue.DBMSType = DbEnum.DBMSType.PstGrS Then
				'Set the Query for PostgreSQL database
				cmnDao.SQLText = String.Format(SELECT_COUNT_POSTGRESQL_TEMPLATE, s & Convert.ToString(parameterValue.TableName) & e, whereSQL).Replace("_p_", p).Replace("_s_", s).Replace("_e_", e).Replace("_f_", f)
			'MYSQL
			ElseIf parameterValue.DBMSType = DbEnum.DBMSType.MySQL Then

				Dim SQLtext As String = String.Format(SELECT_COUNT_SQL_TEMPLATE, parameterValue.TableName, whereSQL).Replace("_p_", p).Replace("_s_", s).Replace("_e_", e).Replace("_f_", f).Replace("""", String.Empty)
				cmnDao.SQLText = SQLtext
			Else

				' SQLを設定して And DB2
				cmnDao.SQLText = String.Format(SELECT_COUNT_SQL_TEMPLATE, s & Convert.ToString(parameterValue.TableName) & e, whereSQL).Replace("_p_", p).Replace("_s_", s).Replace("_e_", e).Replace("_f_", f)
			End If

			' パラメタは指定済み

			' データ件数を取得
			returnValue.Obj = cmnDao.ExecSelectScalar()

			' ↑業務処理-----------------------------------------------------
		End Sub

		''' <summary>データ取得処理を実装</summary>
		''' <param name="parameterValue">引数クラス</param>
		Private Sub UOC_SelectMethod(parameterValue As _3TierParameterValue)
			' 戻り値クラスを生成して、事前に戻り地に設定しておく。
			Dim returnValue As New _3TierReturnValue()
			Me.ReturnValue = returnValue

			' ↓業務処理-----------------------------------------------------

			' 共通Dao
			Dim cmnDao As New CmnDao(Me.GetDam())

			' 検索条件の生成＆指定
			Dim whereSQL As String = Me.SetSearchConditions(parameterValue, cmnDao)

			Dim selectPagingSqlTemplate As String = ""

			Dim p As String = ""
			' パラメタ記号
			Dim s As String = ""
			' 囲い記号開始
			Dim e As String = ""
			' 囲い記号終了
			Dim f As String = ""
			' For supporting type casting in PostgreSQL
			' テンプレート、囲い文字の選択
			If parameterValue.DBMSType = DbEnum.DBMSType.SQLServer Then
				selectPagingSqlTemplate = SELECT_PAGING_SQL_TEMPLATE_SQL_SERVER

				p = "@"
				s = "["
				e = "]"
			ElseIf parameterValue.DBMSType = DbEnum.DBMSType.MySQL Then
				selectPagingSqlTemplate = SELECT_PAGING_MYSQL_TEMPLATE
				p = "@"
				s = """"
				e = """"
			ElseIf parameterValue.DBMSType = DbEnum.DBMSType.DB2 Then
				selectPagingSqlTemplate = SELECT_PAGING_DB2_TEMPLATE
				p = "@"
				s = """"
				e = """"
			ElseIf parameterValue.DBMSType = DbEnum.DBMSType.Oracle Then
				selectPagingSqlTemplate = SELECT_PAGING_SQL_TEMPLATE_ORACLE

				p = ":"
				s = """"
				e = """"
			ElseIf parameterValue.DBMSType = DbEnum.DBMSType.PstGrS Then
				selectPagingSqlTemplate = SELECT_PAGING_SQL_TEMPLATE_POSTGRESQL

				p = "@"
				f = "::text"
			Else
				selectPagingSqlTemplate = SELECT_PAGING_SQL_TEMPLATE_SQL_SERVER

				p = "@"
				s = "["
				e = "]"
			End If

			Dim startRowNum As Integer = parameterValue.StartRowIndex + 1

			Dim selectPagingSQL As String = ""
			If parameterValue.DBMSType = DbEnum.DBMSType.MySQL Then


				selectPagingSQL = String.Format(selectPagingSqlTemplate, New String() {parameterValue.ColumnList, parameterValue.SortExpression, parameterValue.SortDirection, parameterValue.TableName, whereSQL, startRowNum.ToString(), _
					(startRowNum + parameterValue.MaximumRows - 1).ToString()}).Replace("_p_", p).Replace("_s_", s).Replace("_e_", e).Replace("_f_", f).Replace("""", String.Empty)
			Else
				' SQL本体の生成（いろいろ組み込み DB2
				'（DBMSによって可変となる可能性有り）

				selectPagingSQL = String.Format(selectPagingSqlTemplate, New String() {parameterValue.ColumnList, parameterValue.SortExpression, parameterValue.SortDirection, s & Convert.ToString(parameterValue.TableName) & e, whereSQL, startRowNum.ToString(), _
					(startRowNum + parameterValue.MaximumRows - 1).ToString()}).Replace("_p_", p).Replace("_s_", s).Replace("_e_", e).Replace("_f_", f)
			End If
			' DataTableをインスタンス化
			If parameterValue.DataTableType Is Nothing Then
				' == null
				returnValue.Dt = New DataTable()
			Else
				' != null

				' 型付きDataTable
				'（パブリック・デフォルト・コンストラクタ）
				returnValue.Dt = DirectCast(parameterValue.DataTableType.InvokeMember(Nothing, BindingFlags.CreateInstance, Nothing, Nothing, Nothing), DataTable)
			End If

			' SQLを設定して
			cmnDao.SQLText = selectPagingSQL

			' パラメタは指定済み

			' DataTableを取得
			cmnDao.ExecSelectFill_DT(returnValue.Dt)

			' ↑業務処理-----------------------------------------------------
		End Sub

		#Region "共通処理"

		''' <summary>検索条件の設定</summary>
		''' <param name="parameterValue">引数クラス</param>
		''' <param name="cmnDao">共通Dao</param>
		''' <returns>Where句</returns>
		Private Function SetSearchConditions(parameterValue As _3TierParameterValue, cmnDao As CmnDao) As String
			'  検索条件
			Dim whereSQL As String = ""

			'#Region "AND"

			' AndEqualSearchConditions
			' nullチェック
					' == null
			If parameterValue.AndEqualSearchConditions Is Nothing Then
			Else
				' != null
				For Each k As String In parameterValue.AndEqualSearchConditions.Keys
					' フラグ
					Dim isSetted As Boolean = False

					' nullチェック（null相当を要検討
							' == null
					If parameterValue.AndEqualSearchConditions(k) Is Nothing Then
					Else
						' != null

						' 文字列チェック
						If TypeOf parameterValue.AndEqualSearchConditions(k) Is String Then
							' 文字列の場合
									' 空文字列（★ 扱いを検討 → 検索条件の空文字列は検索しない扱い
							If DirectCast(parameterValue.AndEqualSearchConditions(k), String) = "" Then
							Else
								' 空文字列でない。
								isSetted = True
							End If
						Else
							' オブジェクトの場合
							isSetted = True
						End If
					End If

					' パラメタを設定
					If isSetted Then
						whereSQL = GenWhereAndSetParameter(WHERE_SQL_TEMPLATE_EQUAL, whereSQL, cmnDao, k, parameterValue.AndEqualSearchConditions(k), False)

						isSetted = False
					End If
				Next
			End If

			' AndLikeSearchConditions
			' nullチェック
					' == null
			If parameterValue.AndLikeSearchConditions Is Nothing Then
			Else
				' != null
				For Each k As String In parameterValue.AndLikeSearchConditions.Keys
					' nullチェック（null相当を要検討
							' 空文字列
					If String.IsNullOrEmpty(parameterValue.AndLikeSearchConditions(k)) Then
					Else
						' 空文字列でない。

						' パラメタを設定
						whereSQL = GenWhereAndSetParameter(WHERE_SQL_TEMPLATE_LIKE, whereSQL, cmnDao, k, parameterValue.AndLikeSearchConditions(k), True)
					End If
				Next
			End If

			'#End Region

			'#Region "OR"

			' OrEqualSearchConditions
			' nullチェック
					' == null
			If parameterValue.OrEqualSearchConditions Is Nothing Then
			Else
				' != null
				For Each k As String In parameterValue.OrEqualSearchConditions.Keys
					' フラグ
					Dim isSetted As Boolean = False

					' nullチェック（null相当を要検討
							' == null
					If parameterValue.OrEqualSearchConditions(k) Is Nothing Then
					Else
						' != null

						' OR条件はループする。
						Dim i As Integer = 0
						For Each o As Object In parameterValue.OrEqualSearchConditions(k)
							' 文字列チェック
							If TypeOf o Is String Then
								' 文字列の場合
										' 空文字列（★ 扱いを検討 → 検索条件の空文字列は検索しない扱い
								If DirectCast(o, String) = "" Then
								Else
									' 空文字列でない。
									isSetted = True
								End If
							Else
								' オブジェクトの場合
								isSetted = True
							End If

							' パラメタを設定
							If isSetted Then
								whereSQL = GenWhereOrSetParameter(WHERE_SQL_TEMPLATE_EQUAL, whereSQL, cmnDao, k, o, i, _
									False)

								isSetted = False
								i += 1
							End If
						Next
					End If
				Next
			End If

			' OrLikeSearchConditions
			' nullチェック
					' == null
			If parameterValue.OrLikeSearchConditions Is Nothing Then
			Else
				' != null
				For Each k As String In parameterValue.OrLikeSearchConditions.Keys
					' nullチェック（null相当を要検討
							' == null
					If parameterValue.OrLikeSearchConditions(k) Is Nothing Then
					Else
						' != null

						' OR条件はループする。
						Dim i As Integer = 0
						For Each s As String In parameterValue.OrLikeSearchConditions(k)
							' 文字列の場合
									' 空文字列
							If DirectCast(s, String) = "" Then
							Else
								' 空文字列でない。
								' パラメタを設定
								whereSQL = GenWhereOrSetParameter(WHERE_SQL_TEMPLATE_LIKE, whereSQL, cmnDao, k, s, i, _
									True)
								i += 1
							End If
						Next
					End If
				Next
			End If

			'#End Region

			'#Region "その他"

			' 追加の検索条件（要：半角スペース）
			whereSQL += " " & Convert.ToString(parameterValue.ElseWhereSQL)

			' ElseSearchConditions
			' nullチェック
					' == null
			If parameterValue.ElseSearchConditions Is Nothing Then
			Else
				' != null
				For Each k As String In parameterValue.ElseSearchConditions.Keys
					' nullチェック（null相当を要検討
							' == null
					If parameterValue.ElseSearchConditions(k) Is Nothing Then
					Else
						' != null
						cmnDao.SetParameter(k, parameterValue.ElseSearchConditions(k))
					End If
				Next
			End If

			'#End Region

			' Where句の付与（要：Trim）
			If Not String.IsNullOrEmpty(whereSQL.Trim()) Then
				' （要：半角スペース）
				whereSQL = "WHERE " & whereSQL
			End If

			' 先頭の論理演算子を削除
			Return BaseDam.DeleteFirstLogicalOperatoronWhereClause(whereSQL)
		End Function

		''' <summary>Where句生成＆パラメタ指定（and）</summary>
		''' <param name="whereSqlTemplate">Where句SQLテンプレート</param>
		''' <param name="whereSQL">生成中のWhere句SQL</param>
		''' <param name="cmnDao">共通Dao</param>
		''' <param name="parameterName">パラメタ名</param>
		''' <param name="parameterValue">パラメタ値</param>
		''' <param name="isLike">Likeか？</param>
		''' <returns>生成したWhere句SQL</returns>
		Private Function GenWhereAndSetParameter(whereSqlTemplate As String, whereSQL As String, cmnDao As CmnDao, parameterName As String, parameterValue As Object, isLike As Boolean) As String
			' Where句生成
					' 先頭は何もしない。
			If String.IsNullOrEmpty(whereSQL) Then
			Else
				' 以降はAND
				whereSQL += " AND "
			End If

			Dim temp As String = ""
			temp = whereSqlTemplate.Replace("_ColName_", parameterName)

			' パラメタ指定
			If isLike Then
				' Like
				whereSQL += temp.Replace("_ParamName_", Me.LikeParamHeader & parameterName & Me.LikeParamFooter)
				cmnDao.SetParameter(Me.LikeParamHeader & parameterName & Me.LikeParamFooter, parameterValue)
			Else
				' Equal
				whereSQL += temp.Replace("_ParamName_", parameterName)
				cmnDao.SetParameter(parameterName, parameterValue)
			End If

			Return whereSQL
		End Function

		''' <summary>Where句生成＆パラメタ指定（or）</summary>
		''' <param name="whereSqlTemplate">Where句SQLテンプレート</param>
		''' <param name="whereSQL">生成中のWhere句SQL</param>
		''' <param name="cmnDao">共通Dao</param>
		''' <param name="parameterName">パラメタ名</param>
		''' <param name="parameterValue">パラメタ値</param>
		''' <param name="parameterNumber">パラメタ番号</param>
		''' <param name="isLike">Likeか？</param>
		''' <returns>生成したWhere句SQL</returns>
		Private Function GenWhereOrSetParameter(whereSqlTemplate As String, whereSQL As String, cmnDao As CmnDao, parameterName As String, parameterValue As Object, parameterNumber As Integer, _
			isLike As Boolean) As String
			' Where句生成
					' 先頭は何もしない。
			If String.IsNullOrEmpty(whereSQL) Then
			Else
				' 以降はOR
				whereSQL += " OR "
			End If

			Dim temp As String = ""
			temp = whereSqlTemplate.Replace("_ColName_", parameterName)

			' パラメタ指定
			If isLike Then
				' Like
				whereSQL += temp.Replace("_ParamName_", Me.LikeParamHeader & parameterName & parameterNumber.ToString() & Me.LikeParamFooter)
				cmnDao.SetParameter(Me.LikeParamHeader & parameterName & parameterNumber.ToString() & Me.LikeParamFooter, parameterValue)
			Else
				' Equal
				whereSQL += temp.Replace("_ParamName_", parameterName & parameterNumber.ToString())
				cmnDao.SetParameter(parameterName & parameterNumber.ToString(), parameterValue)
			End If

			Return whereSQL
		End Function

		#End Region

		#End Region

		#Region "CRUD系"

		''' <summary>１件追加処理を実装</summary>
		''' <param name="parameterValue">引数クラス</param>
		Protected Overridable Sub UOC_InsertRecord(parameterValue As _3TierParameterValue)
			' 戻り値クラスを生成して、事前に戻り地に設定しておく。
			Dim returnValue As New _3TierReturnValue()
			Me.ReturnValue = returnValue

			' 関連チェック処理
			Me.UOC_RelatedCheck(parameterValue)

			' ↓業務処理-----------------------------------------------------

			' 共通Dao
			Dim cmnDao As New CmnDao(Me.GetDam())

			' 検索条件の指定

			' 追加値
			' InsertUpdateValues
			For Each k As String In parameterValue.InsertUpdateValues.Keys
				' nullチェック（null相当を要検討
						' == null
				If parameterValue.InsertUpdateValues(k) Is Nothing Then
				Else
					' != null

					' 文字列の場合の扱い
					If TypeOf parameterValue.InsertUpdateValues(k) Is String Then
						If Not String.IsNullOrEmpty(DirectCast(parameterValue.InsertUpdateValues(k), String)) Then
							' パラメタ指定
							cmnDao.SetParameter(k, parameterValue.InsertUpdateValues(k))
						End If
					Else
						' パラメタ指定
						cmnDao.SetParameter(k, parameterValue.InsertUpdateValues(k))
					End If
				End If
			Next

			' SQLを設定して
			cmnDao.SQLFileName = Me.DaoClassNameHeader & Convert.ToString(parameterValue.TableName) & Me.DaoClassNameFooter & "_" & Me.MethodNameHeaderD & Me.MethodLabel_Ins & Me.MethodNameFooterD & ".xml"

			' パラメタは指定済み

			' 追加処理を実行
			returnValue.Obj = cmnDao.ExecInsUpDel_NonQuery()

			' ↑業務処理-----------------------------------------------------
		End Sub

		''' <summary>１件取得処理を実装</summary>
		''' <param name="parameterValue">引数クラス</param>
		Protected Overridable Sub UOC_SelectRecord(parameterValue As _3TierParameterValue)
			' 戻り値クラスを生成して、事前に戻り地に設定しておく。
			Dim returnValue As New _3TierReturnValue()
			Me.ReturnValue = returnValue

			' 関連チェック処理
			Me.UOC_RelatedCheck(parameterValue)

			' ↓業務処理-----------------------------------------------------

			' 共通Dao
			Dim cmnDao As New CmnDao(Me.GetDam())

			' 検索条件の指定

			' AndEqualSearchConditions（主キー
			For Each k As String In parameterValue.AndEqualSearchConditions.Keys
				' nullチェック（null相当を要検討
						' == null
				If parameterValue.AndEqualSearchConditions(k) Is Nothing Then
				Else
					' != null

					' 文字列の場合の扱い
					If TypeOf parameterValue.AndEqualSearchConditions(k) Is String Then
						If Not String.IsNullOrEmpty(DirectCast(parameterValue.AndEqualSearchConditions(k), String)) Then
							' パラメタ指定
							cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions(k))
						End If
					Else
						' パラメタ指定
						cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions(k))
					End If
				End If
			Next

			' DataTableをインスタンス化
			If parameterValue.DataTableType Is Nothing Then
				' == null
				returnValue.Dt = New DataTable()
			Else
				' != null

				' 型付きDataTable
				'（パブリック・デフォルト・コンストラクタ）
				returnValue.Dt = DirectCast(parameterValue.DataTableType.InvokeMember(Nothing, BindingFlags.CreateInstance, Nothing, Nothing, Nothing), DataTable)
			End If

			' SQLを設定して
			cmnDao.SQLFileName = Me.DaoClassNameHeader & Convert.ToString(parameterValue.TableName) & Me.DaoClassNameFooter & "_" & Me.MethodNameHeaderS & Me.MethodLabel_Sel & Me.MethodNameFooterS & ".xml"

			' パラメタは指定済み

			' DataTableを取得
			cmnDao.ExecSelectFill_DT(returnValue.Dt)

			' ↑業務処理-----------------------------------------------------
		End Sub

		''' <summary>１件更新処理を実装</summary>
		''' <param name="parameterValue">引数クラス</param>
		Protected Overridable Sub UOC_UpdateRecord(parameterValue As _3TierParameterValue)
			' 戻り値クラスを生成して、事前に戻り地に設定しておく。
			Dim returnValue As New _3TierReturnValue()
			Me.ReturnValue = returnValue

			' 関連チェック処理
			Me.UOC_RelatedCheck(parameterValue)

			' ↓業務処理-----------------------------------------------------

			' 共通Dao
			Dim cmnDao As New CmnDao(Me.GetDam())

			' 検索条件の指定

			' AndEqualSearchConditions（主キー
			For Each k As String In parameterValue.AndEqualSearchConditions.Keys
				' nullチェック（null相当を要検討
						' == null
				If parameterValue.AndEqualSearchConditions(k) Is Nothing Then
				Else
					' != null

					' 文字列の場合の扱い
					If TypeOf parameterValue.AndEqualSearchConditions(k) Is String Then
						If Not String.IsNullOrEmpty(DirectCast(parameterValue.AndEqualSearchConditions(k), String)) Then
							' パラメタ指定
							cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions(k))
						End If
					Else
						' パラメタ指定
						cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions(k))
					End If
				End If
			Next

			' 更新値
			' InsertUpdateValues
			For Each k As String In parameterValue.InsertUpdateValues.Keys
				' nullチェック（null相当を要検討
						' == null
				If parameterValue.InsertUpdateValues(k) Is Nothing Then
				Else
					' != null

					' 文字列の場合の扱い
					If TypeOf parameterValue.InsertUpdateValues(k) Is String Then
						If Not String.IsNullOrEmpty(DirectCast(parameterValue.InsertUpdateValues(k), String)) Then
							' パラメタ指定
							cmnDao.SetParameter(Me.UpdateParamHeader & k & Me.UpdateParamFooter, parameterValue.InsertUpdateValues(k))
						End If
					Else
						' パラメタ指定
						cmnDao.SetParameter(Me.UpdateParamHeader & k & Me.UpdateParamFooter, parameterValue.InsertUpdateValues(k))
					End If
				End If
			Next

			' SQLを設定して
			cmnDao.SQLFileName = Me.DaoClassNameHeader & Convert.ToString(parameterValue.TableName) & Me.DaoClassNameFooter & "_" & Me.MethodNameHeaderS & Me.MethodLabel_Upd & Me.MethodNameFooterS & ".xml"

			' パラメタは指定済み

			' 更新処理を実行
			returnValue.Obj = cmnDao.ExecInsUpDel_NonQuery()

			' ↑業務処理-----------------------------------------------------
		End Sub

		''' <summary>１件削除処理を実装</summary>
		''' <param name="parameterValue">引数クラス</param>
		Protected Overridable Sub UOC_DeleteRecord(parameterValue As _3TierParameterValue)
			' 戻り値クラスを生成して、事前に戻り地に設定しておく。
			Dim returnValue As New _3TierReturnValue()
			Me.ReturnValue = returnValue

			' 関連チェック処理
			Me.UOC_RelatedCheck(parameterValue)

			' ↓業務処理-----------------------------------------------------

			' 共通Dao
			Dim cmnDao As New CmnDao(Me.GetDam())

			' 検索条件の指定

			' AndEqualSearchConditions（主キー
			For Each k As String In parameterValue.AndEqualSearchConditions.Keys
				' nullチェック（null相当を要検討
						' == null
				If parameterValue.AndEqualSearchConditions(k) Is Nothing Then
				Else
					' != null

					' 文字列の場合の扱い
					If TypeOf parameterValue.AndEqualSearchConditions(k) Is String Then
						If Not String.IsNullOrEmpty(DirectCast(parameterValue.AndEqualSearchConditions(k), String)) Then
							' パラメタ指定
							cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions(k))
						End If
					Else
						' パラメタ指定
						cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions(k))
					End If
				End If
			Next

			' SQLを設定して
			cmnDao.SQLFileName = Me.DaoClassNameHeader & Convert.ToString(parameterValue.TableName) & Me.DaoClassNameFooter & "_" & Me.MethodNameHeaderS & Me.MethodLabel_Del & Me.MethodNameFooterS & ".xml"

			' パラメタは指定済み

			' 削除処理を実行
			returnValue.Obj = cmnDao.ExecInsUpDel_NonQuery()

			' ↑業務処理-----------------------------------------------------
		End Sub

		''' <summary>バッチ更新処理を実装</summary>
		''' <param name="parameterValue">引数クラス</param>
		Protected Overridable Sub UOC_BatchUpdate(parameterValue As _3TierParameterValue)
			' 戻り値クラスを生成して、事前に戻り地に設定しておく。
			Dim returnValue As New _3TierReturnValue()
			Me.ReturnValue = returnValue

			' 関連チェック処理
			Me.UOC_RelatedCheck(parameterValue)

			' ↓業務処理-----------------------------------------------------

			' 共通Dao
			Dim cmnDao As New CmnDao(Me.GetDam())

			Dim i As Integer = 0
			' 件数のカウント
			Dim dt As DataTable = DirectCast(parameterValue.Obj, DataTable)

			' バッチ更新
			For Each dr As DataRow In dt.Rows
				Select Case dr.RowState
					Case DataRowState.Added
						' 追加
						' SQLを設定して
						cmnDao.SQLFileName = Me.DaoClassNameHeader & Convert.ToString(parameterValue.TableName) & Me.DaoClassNameFooter & "_" & Me.MethodNameHeaderD & Me.MethodLabel_Ins & Me.MethodNameFooterD & ".xml"

						' パラメタ指定
						For Each c As DataColumn In dt.Columns
							' 空文字列も通常の値と同一に扱う
							cmnDao.SetParameter(c.ColumnName, dr(c))
						Next

						' 更新処理を実行
						i += cmnDao.ExecInsUpDel_NonQuery()

						Exit Select

					Case DataRowState.Modified
						' 更新
						' SQLを設定して
						cmnDao.SQLFileName = Me.DaoClassNameHeader & Convert.ToString(parameterValue.TableName) & Me.DaoClassNameFooter & "_" & Me.MethodNameHeaderS & Me.MethodLabel_Upd & Me.MethodNameFooterS & ".xml"

						' パラメタ指定
						For Each dc As DataColumn In dt.Columns
							' 主キー・タイムスタンプ列の設定はUP側で。
							' また、空文字列も通常の値と同一に扱う。
							If parameterValue.AndEqualSearchConditions.ContainsKey(dc.ColumnName) Then
								' Where条件は、DataRowVersion.Originalを付与
								cmnDao.SetParameter(dc.ColumnName, dr(dc, DataRowVersion.Original))
							Else
								cmnDao.SetParameter(Me.UpdateParamHeader + dc.ColumnName & Me.UpdateParamFooter, dr(dc))
							End If
						Next

						' 更新処理を実行
						i += cmnDao.ExecInsUpDel_NonQuery()

						Exit Select

					Case DataRowState.Deleted
						' 削除
						' SQLを設定して
						cmnDao.SQLFileName = Me.DaoClassNameHeader & Convert.ToString(parameterValue.TableName) & Me.DaoClassNameFooter & "_" & Me.MethodNameHeaderS & Me.MethodLabel_Del & Me.MethodNameFooterS & ".xml"

						' パラメタ指定
						For Each c As DataColumn In dt.Columns
							' 主キー・タイムスタンプ列の設定はUP側で。
							' また、空文字列も通常の値と同一に扱う。
							If parameterValue.AndEqualSearchConditions.ContainsKey(c.ColumnName) Then
								' Where条件は、DataRowVersion.Originalを付与
								cmnDao.SetParameter(c.ColumnName, dr(c, DataRowVersion.Original))
							End If
						Next

						' 更新処理を実行
						i += cmnDao.ExecInsUpDel_NonQuery()

						Exit Select
					Case Else

						' 上記以外
						' なにもしない。
						Exit Select
				End Select
			Next

			' 件数を返却
			returnValue.Obj = i

			' ↑業務処理-----------------------------------------------------
		End Sub

        ''' <summary>Implementing Update process of Multiple tables in Single Transaction</summary>
        ''' <param name="parameterValue">Argument class</param>
        Protected Overridable Sub UOC_UpdateRecordDM(ByVal parameterValue As _3TierParameterValue)
            ' 戻り値クラスを生成して、事前に戻り地に設定しておく。
            Dim returnValue As New _3TierReturnValue()
            Me.ReturnValue = returnValue

            ' 関連チェック処理
            Me.UOC_RelatedCheck(parameterValue)

            ' ↓業務処理-----------------------------------------------------

            Dim i As Integer = 0 ' Number count of row update

            ' Loop through multiple tables for update operation
            For Each TableName As String In parameterValue.TargetTableNames.Values

                ' 共通Dao
                Dim cmnDao As New CmnDao(Me.GetDam())

                ' AndEqualSearchConditions（主キー
                For Each k As String In parameterValue.AndEqualSearchConditions.Keys
                    ' nullチェック（null相当を要検討
                    ' == null
                    If parameterValue.AndEqualSearchConditions(k) Is Nothing Then
                    Else
                        ' != null

                        ' 文字列の場合の扱い
                        If TypeOf parameterValue.AndEqualSearchConditions(k) Is String Then
                            If Not String.IsNullOrEmpty(DirectCast(parameterValue.AndEqualSearchConditions(k), String)) Then
                                ' パラメタ指定
                                cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions(k))
                            End If
                        Else
                            ' パラメタ指定
                            cmnDao.SetParameter(k, parameterValue.AndEqualSearchConditions(k))
                        End If
                    End If
                Next

                ' 更新値
                ' InsertUpdateValues
                For Each k As String In parameterValue.InsertUpdateValues.Keys
                    ' nullチェック（null相当を要検討
                    ' == null
                    If parameterValue.InsertUpdateValues(k) Is Nothing Then
                    Else
                        ' != null

                        ' 文字列の場合の扱い
                        If TypeOf parameterValue.InsertUpdateValues(k) Is String Then
                            If (k.Contains(TableName)) Then
                                ' Setting up parameters by removing tablename from the alias
                                cmnDao.SetParameter(Me.UpdateParamHeader & k.Remove(0, (TableName & "_").Length) & Me.UpdateParamFooter, parameterValue.InsertUpdateValues(k))
                            End If

                        Else
                            If (k.Contains(TableName)) Then
                                ' Setting up parameters by removing tablename from the alias
                                cmnDao.SetParameter(Me.UpdateParamHeader & k.Remove(0, (TableName & "_").Length) & Me.UpdateParamFooter, parameterValue.InsertUpdateValues(k))
                            End If

                        End If
                    End If
                Next

                ' SQLを設定して
                cmnDao.SQLFileName = Me.DaoClassNameHeader & Convert.ToString(TableName) & Me.DaoClassNameFooter & "_" & Me.MethodNameHeaderS & Me.MethodLabel_Upd & Me.MethodNameFooterS & ".xml"

                ' パラメタは指定済み

                ' 更新処理を実行
                i += cmnDao.ExecInsUpDel_NonQuery()
            Next

            returnValue.Obj = i

            ' ↑業務処理-----------------------------------------------------
        End Sub

        ''' <summary>Implementing Batch Update and Delete process of Multiple tables in Single Transaction</summary>
        ''' <param name="parameterValue">Argument class</param>
        Protected Overridable Sub UOC_BatchUpdateDM(ByVal parameterValue As _3TierParameterValue)
            ' 戻り値クラスを生成して、事前に戻り地に設定しておく。
            Dim returnValue As New _3TierReturnValue()
            Me.ReturnValue = returnValue

            ' 関連チェック処理
            Me.UOC_RelatedCheck(parameterValue)

            Dim i As Integer = 0 ' Number count of row update

            ' ↓業務処理-----------------------------------------------------

            ' Loop through multiple tables for update operation
            For Each TableName As String In parameterValue.TargetTableNames.Values
                ' 共通Dao
                Dim cmnDao As New CmnDao(Me.GetDam())

                ' 件数のカウント
                Dim dt As DataTable = DirectCast(parameterValue.Obj, DataTable)

                ' バッチ更新
                For Each dr As DataRow In dt.Rows
                    Select Case dr.RowState
                        Case DataRowState.Modified
                            ' 更新
                            ' SQLを設定して
                            cmnDao.SQLFileName = Me.DaoClassNameHeader & Convert.ToString(TableName) & Me.DaoClassNameFooter & "_" & Me.MethodNameHeaderS & Me.MethodLabel_Upd & Me.MethodNameFooterS & ".xml"

                            ' パラメタ指定
                            For Each dc As DataColumn In dt.Columns
                                ' 主キー・タイムスタンプ列の設定はUP側で。
                                ' また、空文字列も通常の値と同一に扱う。
                                If parameterValue.AndEqualSearchConditions.ContainsKey(dc.ColumnName) Then
                                    ' Where条件は、DataRowVersion.Originalを付与
                                    ' Setting up parameters by removing tablename from the alias
                                    cmnDao.SetParameter(dc.ColumnName.Replace(TableName & "_", ""), dr(dc, DataRowVersion.Original))
                                Else
                                    ' Setting up parameters by removing tablename from the alias
                                    cmnDao.SetParameter(Me.UpdateParamHeader + dc.ColumnName.Replace(TableName & "_", "") & Me.UpdateParamFooter, dr(dc))
                                End If
                            Next

                            ' 更新処理を実行
                            i += cmnDao.ExecInsUpDel_NonQuery()

                            Exit Select
                        Case DataRowState.Deleted
                            ' 削除
                            ' SQLを設定して
                            cmnDao.SQLFileName = Me.DaoClassNameHeader & Convert.ToString(TableName) & Me.DaoClassNameFooter & "_" & Me.MethodNameHeaderS & Me.MethodLabel_Del & Me.MethodNameFooterS & ".xml"

                            ' パラメタ指定
                            For Each c As DataColumn In dt.Columns
                                ' 主キー・タイムスタンプ列の設定はUP側で。
                                ' また、空文字列も通常の値と同一に扱う。
                                If parameterValue.AndEqualSearchConditions.ContainsKey(c.ColumnName) Then
                                    ' Where条件は、DataRowVersion.Originalを付与
                                    ' Setting up parameters by removing tablename from the alias
                                    cmnDao.SetParameter(c.ColumnName.Replace(TableName & "_", ""), dr(c, DataRowVersion.Original))
                                End If
                            Next

                            ' 更新処理を実行
                            i += cmnDao.ExecInsUpDel_NonQuery()

                            Exit Select
                        Case Else

                            ' 上記以外
                            ' なにもしない。
                            Exit Select
                    End Select
                Next
            Next

            ' 件数を返却
            returnValue.Obj = i

            ' ↑業務処理-----------------------------------------------------
        End Sub

        ''' <summary>Implementing Delete process of Multiple tables in Single Transaction</summary>
        ''' <param name="parameterValue">Argument class</param>
        Protected Overridable Sub UOC_DeleteRecordDM(ByVal parameterValue As _3TierParameterValue)
            ' 戻り値クラスを生成して、事前に戻り地に設定しておく。
            Dim returnValue As New _3TierReturnValue()
            Me.ReturnValue = returnValue

            ' 関連チェック処理
            Me.UOC_RelatedCheck(parameterValue)

            ' ↓業務処理-----------------------------------------------------

            Dim i As Integer = 0 ' Number count of row update

            ' Loop through multiple tables for update operation
            For Each TableName As String In parameterValue.TargetTableNames.Values
                ' 共通Dao
                Dim cmnDao As New CmnDao(Me.GetDam())

                ' 検索条件の指定

                ' AndEqualSearchConditions（主キー
                For Each k As String In parameterValue.AndEqualSearchConditions.Keys
                    ' nullチェック（null相当を要検討
                    ' == null
                    If parameterValue.AndEqualSearchConditions(k) Is Nothing Then
                    Else
                        ' != null

                        ' 文字列の場合の扱い
                        If TypeOf parameterValue.AndEqualSearchConditions(k) Is String Then
                            If Not String.IsNullOrEmpty(DirectCast(parameterValue.AndEqualSearchConditions(k), String)) Then
                                ' パラメタ指定
                                ' Removing Tablename from the parameters
                                cmnDao.SetParameter(k.Replace(TableName + "_", ""), parameterValue.AndEqualSearchConditions(k))
                            End If
                        Else
                            ' パラメタ指定
                            ' Removing Tablename from the parameters
                            cmnDao.SetParameter(k.Replace(TableName + "_", ""), parameterValue.AndEqualSearchConditions(k))
                        End If
                    End If
                Next

                ' SQLを設定して
                cmnDao.SQLFileName = Me.DaoClassNameHeader & Convert.ToString(TableName) & Me.DaoClassNameFooter & "_" & Me.MethodNameHeaderS & Me.MethodLabel_Del & Me.MethodNameFooterS & ".xml"

                ' パラメタは指定済み

                ' 削除処理を実行
                i += cmnDao.ExecInsUpDel_NonQuery()
            Next

            returnValue.Obj = i
            ' ↑業務処理-----------------------------------------------------
        End Sub

#End Region

        ''' <summary>関連チェック処理を実装可能に</summary>
        ''' <param name="parameterValue">引数</param>
        Protected Overridable Sub UOC_RelatedCheck(ByVal parameterValue As _3TierParameterValue)
        End Sub
    End Class
End Namespace

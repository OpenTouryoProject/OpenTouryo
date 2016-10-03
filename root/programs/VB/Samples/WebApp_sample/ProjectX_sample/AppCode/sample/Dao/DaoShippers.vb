'**********************************************************************************
'* クラス名        ：DaoShippers
'* クラス日本語名  ：自動生成Ｄａｏクラス
'*
'* 作成日時        ：2014/2/9
'* 作成者          ：棟梁 D層自動生成ツール（墨壺）, 日立 太郎
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*  2012/06/14  西野  大介        ResourceLoaderに加え、EmbeddedResourceLoaderに対応
'*  2013/09/09  西野  大介        ExecGenerateSQLメソッドを追加した（バッチ更新用）。
'**********************************************************************************

#Region "using"

' System～
Imports System
Imports System.IO
Imports System.Data
Imports System.Collections

' フレームワーク
Imports Touryo.Infrastructure.Framework.Dao
Imports Touryo.Infrastructure.Framework.Common

' 部品
Imports Touryo.Infrastructure.Public.Db
Imports Touryo.Infrastructure.Public.Util

' 業務フレームワーク
Imports Touryo.Infrastructure.Business.Dao

#End Region

''' <summary>自動生成Ｄａｏクラス</summary>
Public Class DaoShippers
	Inherits MyBaseDao
	#Region "インスタンス変数"

	''' <summary>ユーザ パラメタ（文字列置換）用ハッシュ テーブル</summary>
	Protected HtUserParameter As New Hashtable()
	''' <summary>パラメタ ライズド クエリのパラメタ用ハッシュ テーブル</summary>
	Protected HtParameter As New Hashtable()

	#End Region

	#Region "コンストラクタ"

	''' <summary>コンストラクタ</summary>
	Public Sub New(dam As BaseDam)
		MyBase.New(dam)
	End Sub

	#End Region

	#Region "共通関数（パラメタの制御）"

	''' <summary>ユーザ パラメタ（文字列置換）をハッシュ テーブルに設定する。</summary>
	''' <param name="userParamName">ユーザ パラメタ名</param>
	''' <param name="userParamValue">ユーザ パラメタ値</param>
	Public Sub SetUserParameteToHt(userParamName As String, userParamValue As String)
		' ユーザ パラメタをハッシュ テーブルに設定
		Me.HtUserParameter(userParamName) = userParamValue
	End Sub

	''' <summary>パラメタ ライズド クエリのパラメタをハッシュ テーブルに設定する。</summary>
	''' <param name="paramName">パラメタ名</param>
	''' <param name="paramValue">パラメタ値</param>
	Public Sub SetParameteToHt(paramName As String, paramValue As Object)
		' ユーザ パラメタをハッシュ テーブルに設定
		Me.HtParameter(paramName) = paramValue
	End Sub

	''' <summary>
	''' ・ユーザ パラメタ（文字列置換）
	''' ・パラメタ ライズド クエリのパラメタ
	''' を格納するハッシュ テーブルをクリアする。
	''' </summary>
	Public Sub ClearParametersFromHt()
		' ユーザ パラメタ（文字列置換）用ハッシュ テーブルを初期化
		Me.HtUserParameter = New Hashtable()
		' パラメタ ライズド クエリのパラメタ用ハッシュ テーブルを初期化
		Me.HtParameter = New Hashtable()
	End Sub

	''' <summary>パラメタの設定（内部用）</summary>
	Protected Sub SetParametersFromHt()
		' ユーザ パラメタ（文字列置換）を設定する。
		For Each userParamName As String In Me.HtUserParameter.Keys
			Me.SetUserParameter(userParamName, Me.HtUserParameter(userParamName).ToString())
		Next

		' パラメタ ライズド クエリのパラメタを設定する。
		For Each paramName As String In Me.HtParameter.Keys
			Me.SetParameter(paramName, Me.HtParameter(paramName))
		Next
	End Sub

	#End Region

	#Region "プロパティ プロシージャ（setter、getter）"


	''' <summary>ShipperID列（主キー列）に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property PK_ShipperID() As Object
		Get
			Return Me.HtParameter("ShipperID")
		End Get
		Set
			Me.HtParameter("ShipperID") = value
		End Set
	End Property



	''' <summary>CompanyName列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property CompanyName() As Object
		Get
			Return Me.HtParameter("CompanyName")
		End Get
		Set
			Me.HtParameter("CompanyName") = value
		End Set
	End Property

	''' <summary>Phone列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property Phone() As Object
		Get
			Return Me.HtParameter("Phone")
		End Get
		Set
			Me.HtParameter("Phone") = value
		End Set
	End Property


	''' <summary>Set_ShipperID_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ShipperID_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ShipperID_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ShipperID_forUPD") = value
		End Set
	End Property


	''' <summary>Set_CompanyName_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_CompanyName_forUPD() As Object
		Get
			Return Me.HtParameter("Set_CompanyName_forUPD")
		End Get
		Set
			Me.HtParameter("Set_CompanyName_forUPD") = value
		End Set
	End Property


	''' <summary>Set_Phone_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_Phone_forUPD() As Object
		Get
			Return Me.HtParameter("Set_Phone_forUPD")
		End Get
		Set
			Me.HtParameter("Set_Phone_forUPD") = value
		End Set
	End Property



	''' <summary>ShipperID_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ShipperID_Like() As Object
		Get
			Return Me.HtParameter("ShipperID_Like")
		End Get
		Set
			Me.HtParameter("ShipperID_Like") = value
		End Set
	End Property


	''' <summary>CompanyName_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property CompanyName_Like() As Object
		Get
			Return Me.HtParameter("CompanyName_Like")
		End Get
		Set
			Me.HtParameter("CompanyName_Like") = value
		End Set
	End Property


	''' <summary>Phone_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property Phone_Like() As Object
		Get
			Return Me.HtParameter("Phone_Like")
		End Get
		Set
			Me.HtParameter("Phone_Like") = value
		End Set
	End Property


	#End Region

	#Region "クエリ メソッド"

	#Region "Insert"

	''' <summary>１レコード挿入する。</summary>
	''' <returns>挿入された行の数</returns>
	Public Function S1_Insert() As Integer
		' ファイルからSQL（Insert）を設定する。
		Me.SetSqlByFile2("DaoShippers_S1_Insert.sql")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Insert）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	''' <summary>１レコード挿入する。</summary>
	''' <returns>挿入された行の数</returns>
	''' <remarks>パラメタで指定した列のみ挿入値が有効になる。</remarks>
	Public Function D1_Insert() As Integer
		' ファイルからSQL（DynIns）を設定する。
		Me.SetSqlByFile2("DaoShippers_D1_Insert.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（DynIns）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	#End Region

	#Region "Select"

	''' <summary>主キーを指定し、１レコード参照する。</summary>
	''' <param name="dt">結果を格納するDataTable</param>
	Public Sub S2_Select(dt As DataTable)
		' ファイルからSQL（Select）を設定する。
		Me.SetSqlByFile2("DaoShippers_S2_Select.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Select）を実行し、戻り値を戻す。
		Me.ExecSelectFill_DT(dt)
	End Sub

	''' <summary>検索条件を指定し、結果セットを参照する。</summary>
	''' <param name="dt">結果を格納するDataTable</param>
	Public Sub D2_Select(dt As DataTable)
		' ファイルからSQL（DynSel）を設定する。
		Me.SetSqlByFile2("DaoShippers_D2_Select.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（DynSel）を実行し、戻り値を戻す。
		Me.ExecSelectFill_DT(dt)
	End Sub

	#End Region

	#Region "Update"

	''' <summary>主キーを指定し、１レコード更新する。</summary>
	''' <returns>更新された行の数</returns>
	''' <remarks>パラメタで指定した列のみ更新値が有効になる。</remarks>
	Public Function S3_Update() As Integer
		' ファイルからSQL（Update）を設定する。
		Me.SetSqlByFile2("DaoShippers_S3_Update.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Update）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	''' <summary>任意の検索条件でデータを更新する。</summary>
	''' <returns>更新された行の数</returns>
	''' <remarks>パラメタで指定した列のみ更新値が有効になる。</remarks>
	Public Function D3_Update() As Integer
		' ファイルからSQL（DynUpd）を設定する。
		Me.SetSqlByFile2("DaoShippers_D3_Update.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（DynUpd）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	#End Region

	#Region "Delete"

	''' <summary>主キーを指定し、１レコード削除する。</summary>
	''' <returns>削除された行の数</returns>
	Public Function S4_Delete() As Integer
		' ファイルからSQL（Delete）を設定する。
		Me.SetSqlByFile2("DaoShippers_S4_Delete.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Delete）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	''' <summary>任意の検索条件でデータを削除する。</summary>
	''' <returns>削除された行の数</returns>
	Public Function D4_Delete() As Integer
		' ファイルからSQL（DynDel）を設定する。
		Me.SetSqlByFile2("DaoShippers_D4_Delete.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（DynDel）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	#End Region

	#Region "拡張メソッド"

	''' <summary>テーブルのレコード件数を取得する</summary>
	''' <returns>テーブルのレコード件数</returns>
	Public Function D5_SelCnt() As Object
		' ファイルからSQL（DynSelCnt）を設定する。
		Me.SetSqlByFile2("DaoShippers_D5_SelCnt.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（SELECT COUNT）を実行し、戻り値を戻す。
		Return Me.ExecSelectScalar()
	End Function

	''' <summary>静的SQLを生成する。</summary>
	''' <param name="fileName">ファイル名</param>
	''' <param name="sqlUtil">SQLユーティリティ</param>
	''' <returns>生成した静的SQL</returns>
	Public Overloads Function ExecGenerateSQL(fileName As String, sqlUtil As SQLUtility) As String
		' ファイルからSQLを設定する。
		Me.SetSqlByFile2(fileName)

		' パラメタの設定
		Me.SetParametersFromHt()

		Return MyBase.ExecGenerateSQL(sqlUtil)
	End Function

	#End Region

	#End Region
End Class

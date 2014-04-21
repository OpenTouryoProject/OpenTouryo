'**********************************************************************************
'* クラス名        ：DaoOrders2
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
Public Class DaoOrders2
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


	''' <summary>OrderID列（主キー列）に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property PK_OrderID() As Object
		Get
			Return Me.HtParameter("OrderID")
		End Get
		Set
			Me.HtParameter("OrderID") = value
		End Set
	End Property



	''' <summary>CustomerID列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property CustomerID() As Object
		Get
			Return Me.HtParameter("CustomerID")
		End Get
		Set
			Me.HtParameter("CustomerID") = value
		End Set
	End Property

	''' <summary>EmployeeID列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property EmployeeID() As Object
		Get
			Return Me.HtParameter("EmployeeID")
		End Get
		Set
			Me.HtParameter("EmployeeID") = value
		End Set
	End Property

	''' <summary>OrderDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property OrderDate() As Object
		Get
			Return Me.HtParameter("OrderDate")
		End Get
		Set
			Me.HtParameter("OrderDate") = value
		End Set
	End Property

	''' <summary>RequiredDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property RequiredDate() As Object
		Get
			Return Me.HtParameter("RequiredDate")
		End Get
		Set
			Me.HtParameter("RequiredDate") = value
		End Set
	End Property

	''' <summary>ShippedDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ShippedDate() As Object
		Get
			Return Me.HtParameter("ShippedDate")
		End Get
		Set
			Me.HtParameter("ShippedDate") = value
		End Set
	End Property

	''' <summary>ShipVia列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ShipVia() As Object
		Get
			Return Me.HtParameter("ShipVia")
		End Get
		Set
			Me.HtParameter("ShipVia") = value
		End Set
	End Property

	''' <summary>Freight列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property Freight() As Object
		Get
			Return Me.HtParameter("Freight")
		End Get
		Set
			Me.HtParameter("Freight") = value
		End Set
	End Property

	''' <summary>ShipName列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ShipName() As Object
		Get
			Return Me.HtParameter("ShipName")
		End Get
		Set
			Me.HtParameter("ShipName") = value
		End Set
	End Property

	''' <summary>ShipAddress列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ShipAddress() As Object
		Get
			Return Me.HtParameter("ShipAddress")
		End Get
		Set
			Me.HtParameter("ShipAddress") = value
		End Set
	End Property

	''' <summary>ShipCity列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ShipCity() As Object
		Get
			Return Me.HtParameter("ShipCity")
		End Get
		Set
			Me.HtParameter("ShipCity") = value
		End Set
	End Property

	''' <summary>ShipRegion列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ShipRegion() As Object
		Get
			Return Me.HtParameter("ShipRegion")
		End Get
		Set
			Me.HtParameter("ShipRegion") = value
		End Set
	End Property

	''' <summary>ShipPostalCode列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ShipPostalCode() As Object
		Get
			Return Me.HtParameter("ShipPostalCode")
		End Get
		Set
			Me.HtParameter("ShipPostalCode") = value
		End Set
	End Property

	''' <summary>ShipCountry列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ShipCountry() As Object
		Get
			Return Me.HtParameter("ShipCountry")
		End Get
		Set
			Me.HtParameter("ShipCountry") = value
		End Set
	End Property


	''' <summary>Set_OrderID_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_OrderID_forUPD() As Object
		Get
			Return Me.HtParameter("Set_OrderID_forUPD")
		End Get
		Set
			Me.HtParameter("Set_OrderID_forUPD") = value
		End Set
	End Property


	''' <summary>Set_CustomerID_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_CustomerID_forUPD() As Object
		Get
			Return Me.HtParameter("Set_CustomerID_forUPD")
		End Get
		Set
			Me.HtParameter("Set_CustomerID_forUPD") = value
		End Set
	End Property


	''' <summary>Set_EmployeeID_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_EmployeeID_forUPD() As Object
		Get
			Return Me.HtParameter("Set_EmployeeID_forUPD")
		End Get
		Set
			Me.HtParameter("Set_EmployeeID_forUPD") = value
		End Set
	End Property


	''' <summary>Set_OrderDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_OrderDate_forUPD() As Object
		Get
			Return Me.HtParameter("Set_OrderDate_forUPD")
		End Get
		Set
			Me.HtParameter("Set_OrderDate_forUPD") = value
		End Set
	End Property


	''' <summary>Set_RequiredDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_RequiredDate_forUPD() As Object
		Get
			Return Me.HtParameter("Set_RequiredDate_forUPD")
		End Get
		Set
			Me.HtParameter("Set_RequiredDate_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ShippedDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ShippedDate_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ShippedDate_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ShippedDate_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ShipVia_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ShipVia_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ShipVia_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ShipVia_forUPD") = value
		End Set
	End Property


	''' <summary>Set_Freight_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_Freight_forUPD() As Object
		Get
			Return Me.HtParameter("Set_Freight_forUPD")
		End Get
		Set
			Me.HtParameter("Set_Freight_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ShipName_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ShipName_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ShipName_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ShipName_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ShipAddress_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ShipAddress_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ShipAddress_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ShipAddress_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ShipCity_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ShipCity_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ShipCity_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ShipCity_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ShipRegion_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ShipRegion_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ShipRegion_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ShipRegion_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ShipPostalCode_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ShipPostalCode_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ShipPostalCode_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ShipPostalCode_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ShipCountry_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ShipCountry_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ShipCountry_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ShipCountry_forUPD") = value
		End Set
	End Property



	''' <summary>OrderID_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property OrderID_Like() As Object
		Get
			Return Me.HtParameter("OrderID_Like")
		End Get
		Set
			Me.HtParameter("OrderID_Like") = value
		End Set
	End Property


	''' <summary>CustomerID_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property CustomerID_Like() As Object
		Get
			Return Me.HtParameter("CustomerID_Like")
		End Get
		Set
			Me.HtParameter("CustomerID_Like") = value
		End Set
	End Property


	''' <summary>EmployeeID_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property EmployeeID_Like() As Object
		Get
			Return Me.HtParameter("EmployeeID_Like")
		End Get
		Set
			Me.HtParameter("EmployeeID_Like") = value
		End Set
	End Property


	''' <summary>OrderDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property OrderDate_Like() As Object
		Get
			Return Me.HtParameter("OrderDate_Like")
		End Get
		Set
			Me.HtParameter("OrderDate_Like") = value
		End Set
	End Property


	''' <summary>RequiredDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property RequiredDate_Like() As Object
		Get
			Return Me.HtParameter("RequiredDate_Like")
		End Get
		Set
			Me.HtParameter("RequiredDate_Like") = value
		End Set
	End Property


	''' <summary>ShippedDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ShippedDate_Like() As Object
		Get
			Return Me.HtParameter("ShippedDate_Like")
		End Get
		Set
			Me.HtParameter("ShippedDate_Like") = value
		End Set
	End Property


	''' <summary>ShipVia_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ShipVia_Like() As Object
		Get
			Return Me.HtParameter("ShipVia_Like")
		End Get
		Set
			Me.HtParameter("ShipVia_Like") = value
		End Set
	End Property


	''' <summary>Freight_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property Freight_Like() As Object
		Get
			Return Me.HtParameter("Freight_Like")
		End Get
		Set
			Me.HtParameter("Freight_Like") = value
		End Set
	End Property


	''' <summary>ShipName_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ShipName_Like() As Object
		Get
			Return Me.HtParameter("ShipName_Like")
		End Get
		Set
			Me.HtParameter("ShipName_Like") = value
		End Set
	End Property


	''' <summary>ShipAddress_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ShipAddress_Like() As Object
		Get
			Return Me.HtParameter("ShipAddress_Like")
		End Get
		Set
			Me.HtParameter("ShipAddress_Like") = value
		End Set
	End Property


	''' <summary>ShipCity_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ShipCity_Like() As Object
		Get
			Return Me.HtParameter("ShipCity_Like")
		End Get
		Set
			Me.HtParameter("ShipCity_Like") = value
		End Set
	End Property


	''' <summary>ShipRegion_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ShipRegion_Like() As Object
		Get
			Return Me.HtParameter("ShipRegion_Like")
		End Get
		Set
			Me.HtParameter("ShipRegion_Like") = value
		End Set
	End Property


	''' <summary>ShipPostalCode_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ShipPostalCode_Like() As Object
		Get
			Return Me.HtParameter("ShipPostalCode_Like")
		End Get
		Set
			Me.HtParameter("ShipPostalCode_Like") = value
		End Set
	End Property


	''' <summary>ShipCountry_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ShipCountry_Like() As Object
		Get
			Return Me.HtParameter("ShipCountry_Like")
		End Get
		Set
			Me.HtParameter("ShipCountry_Like") = value
		End Set
	End Property


	#End Region

	#Region "クエリ メソッド"

	#Region "Insert"

	''' <summary>１レコード挿入する。</summary>
	''' <returns>挿入された行の数</returns>
	Public Function S1_Insert() As Integer
		' ファイルからSQL（Insert）を設定する。
		Me.SetSqlByFile2("DaoOrders2_S1_Insert.sql")

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
		Me.SetSqlByFile2("DaoOrders2_D1_Insert.xml")

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
		Me.SetSqlByFile2("DaoOrders2_S2_Select.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Select）を実行し、戻り値を戻す。
		Me.ExecSelectFill_DT(dt)
	End Sub

	''' <summary>検索条件を指定し、結果セットを参照する。</summary>
	''' <param name="dt">結果を格納するDataTable</param>
	Public Sub D2_Select(dt As DataTable)
		' ファイルからSQL（DynSel）を設定する。
		Me.SetSqlByFile2("DaoOrders2_D2_Select.xml")

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
		Me.SetSqlByFile2("DaoOrders2_S3_Update.xml")

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
		Me.SetSqlByFile2("DaoOrders2_D3_Update.xml")

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
		Me.SetSqlByFile2("DaoOrders2_S4_Delete.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Delete）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	''' <summary>任意の検索条件でデータを削除する。</summary>
	''' <returns>削除された行の数</returns>
	Public Function D4_Delete() As Integer
		' ファイルからSQL（DynDel）を設定する。
		Me.SetSqlByFile2("DaoOrders2_D4_Delete.xml")

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
		Me.SetSqlByFile2("DaoOrders2_D5_SelCnt.xml")

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

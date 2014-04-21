'**********************************************************************************
'* クラス名        ：DaoEmployees
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
Public Class DaoEmployees
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


	''' <summary>EmployeeID列（主キー列）に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property PK_EmployeeID() As Object
		Get
			Return Me.HtParameter("EmployeeID")
		End Get
		Set
			Me.HtParameter("EmployeeID") = value
		End Set
	End Property



	''' <summary>LastName列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property LastName() As Object
		Get
			Return Me.HtParameter("LastName")
		End Get
		Set
			Me.HtParameter("LastName") = value
		End Set
	End Property

	''' <summary>FirstName列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property FirstName() As Object
		Get
			Return Me.HtParameter("FirstName")
		End Get
		Set
			Me.HtParameter("FirstName") = value
		End Set
	End Property

	''' <summary>Title列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property Title() As Object
		Get
			Return Me.HtParameter("Title")
		End Get
		Set
			Me.HtParameter("Title") = value
		End Set
	End Property

	''' <summary>TitleOfCourtesy列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property TitleOfCourtesy() As Object
		Get
			Return Me.HtParameter("TitleOfCourtesy")
		End Get
		Set
			Me.HtParameter("TitleOfCourtesy") = value
		End Set
	End Property

	''' <summary>BirthDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property BirthDate() As Object
		Get
			Return Me.HtParameter("BirthDate")
		End Get
		Set
			Me.HtParameter("BirthDate") = value
		End Set
	End Property

	''' <summary>HireDate列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property HireDate() As Object
		Get
			Return Me.HtParameter("HireDate")
		End Get
		Set
			Me.HtParameter("HireDate") = value
		End Set
	End Property

	''' <summary>Address列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property Address() As Object
		Get
			Return Me.HtParameter("Address")
		End Get
		Set
			Me.HtParameter("Address") = value
		End Set
	End Property

	''' <summary>City列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property City() As Object
		Get
			Return Me.HtParameter("City")
		End Get
		Set
			Me.HtParameter("City") = value
		End Set
	End Property

	''' <summary>Region列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property Region() As Object
		Get
			Return Me.HtParameter("Region")
		End Get
		Set
			Me.HtParameter("Region") = value
		End Set
	End Property

	''' <summary>PostalCode列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property PostalCode() As Object
		Get
			Return Me.HtParameter("PostalCode")
		End Get
		Set
			Me.HtParameter("PostalCode") = value
		End Set
	End Property

	''' <summary>Country列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property Country() As Object
		Get
			Return Me.HtParameter("Country")
		End Get
		Set
			Me.HtParameter("Country") = value
		End Set
	End Property

	''' <summary>HomePhone列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property HomePhone() As Object
		Get
			Return Me.HtParameter("HomePhone")
		End Get
		Set
			Me.HtParameter("HomePhone") = value
		End Set
	End Property

	''' <summary>Extension列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property Extension() As Object
		Get
			Return Me.HtParameter("Extension")
		End Get
		Set
			Me.HtParameter("Extension") = value
		End Set
	End Property

	''' <summary>Photo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property Photo() As Object
		Get
			Return Me.HtParameter("Photo")
		End Get
		Set
			Me.HtParameter("Photo") = value
		End Set
	End Property

	''' <summary>Notes列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property Notes() As Object
		Get
			Return Me.HtParameter("Notes")
		End Get
		Set
			Me.HtParameter("Notes") = value
		End Set
	End Property

	''' <summary>ReportsTo列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property ReportsTo() As Object
		Get
			Return Me.HtParameter("ReportsTo")
		End Get
		Set
			Me.HtParameter("ReportsTo") = value
		End Set
	End Property

	''' <summary>PhotoPath列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタを除く</remarks>
	Public Property PhotoPath() As Object
		Get
			Return Me.HtParameter("PhotoPath")
		End Get
		Set
			Me.HtParameter("PhotoPath") = value
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


	''' <summary>Set_LastName_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_LastName_forUPD() As Object
		Get
			Return Me.HtParameter("Set_LastName_forUPD")
		End Get
		Set
			Me.HtParameter("Set_LastName_forUPD") = value
		End Set
	End Property


	''' <summary>Set_FirstName_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_FirstName_forUPD() As Object
		Get
			Return Me.HtParameter("Set_FirstName_forUPD")
		End Get
		Set
			Me.HtParameter("Set_FirstName_forUPD") = value
		End Set
	End Property


	''' <summary>Set_Title_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_Title_forUPD() As Object
		Get
			Return Me.HtParameter("Set_Title_forUPD")
		End Get
		Set
			Me.HtParameter("Set_Title_forUPD") = value
		End Set
	End Property


	''' <summary>Set_TitleOfCourtesy_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_TitleOfCourtesy_forUPD() As Object
		Get
			Return Me.HtParameter("Set_TitleOfCourtesy_forUPD")
		End Get
		Set
			Me.HtParameter("Set_TitleOfCourtesy_forUPD") = value
		End Set
	End Property


	''' <summary>Set_BirthDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_BirthDate_forUPD() As Object
		Get
			Return Me.HtParameter("Set_BirthDate_forUPD")
		End Get
		Set
			Me.HtParameter("Set_BirthDate_forUPD") = value
		End Set
	End Property


	''' <summary>Set_HireDate_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_HireDate_forUPD() As Object
		Get
			Return Me.HtParameter("Set_HireDate_forUPD")
		End Get
		Set
			Me.HtParameter("Set_HireDate_forUPD") = value
		End Set
	End Property


	''' <summary>Set_Address_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_Address_forUPD() As Object
		Get
			Return Me.HtParameter("Set_Address_forUPD")
		End Get
		Set
			Me.HtParameter("Set_Address_forUPD") = value
		End Set
	End Property


	''' <summary>Set_City_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_City_forUPD() As Object
		Get
			Return Me.HtParameter("Set_City_forUPD")
		End Get
		Set
			Me.HtParameter("Set_City_forUPD") = value
		End Set
	End Property


	''' <summary>Set_Region_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_Region_forUPD() As Object
		Get
			Return Me.HtParameter("Set_Region_forUPD")
		End Get
		Set
			Me.HtParameter("Set_Region_forUPD") = value
		End Set
	End Property


	''' <summary>Set_PostalCode_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_PostalCode_forUPD() As Object
		Get
			Return Me.HtParameter("Set_PostalCode_forUPD")
		End Get
		Set
			Me.HtParameter("Set_PostalCode_forUPD") = value
		End Set
	End Property


	''' <summary>Set_Country_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_Country_forUPD() As Object
		Get
			Return Me.HtParameter("Set_Country_forUPD")
		End Get
		Set
			Me.HtParameter("Set_Country_forUPD") = value
		End Set
	End Property


	''' <summary>Set_HomePhone_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_HomePhone_forUPD() As Object
		Get
			Return Me.HtParameter("Set_HomePhone_forUPD")
		End Get
		Set
			Me.HtParameter("Set_HomePhone_forUPD") = value
		End Set
	End Property


	''' <summary>Set_Extension_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_Extension_forUPD() As Object
		Get
			Return Me.HtParameter("Set_Extension_forUPD")
		End Get
		Set
			Me.HtParameter("Set_Extension_forUPD") = value
		End Set
	End Property


	''' <summary>Set_Photo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_Photo_forUPD() As Object
		Get
			Return Me.HtParameter("Set_Photo_forUPD")
		End Get
		Set
			Me.HtParameter("Set_Photo_forUPD") = value
		End Set
	End Property


	''' <summary>Set_Notes_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_Notes_forUPD() As Object
		Get
			Return Me.HtParameter("Set_Notes_forUPD")
		End Get
		Set
			Me.HtParameter("Set_Notes_forUPD") = value
		End Set
	End Property


	''' <summary>Set_ReportsTo_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_ReportsTo_forUPD() As Object
		Get
			Return Me.HtParameter("Set_ReportsTo_forUPD")
		End Get
		Set
			Me.HtParameter("Set_ReportsTo_forUPD") = value
		End Set
	End Property


	''' <summary>Set_PhotoPath_forUPD列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>更新処理時のSET句で使用するパラメタ専用</remarks>
	Public Property Set_PhotoPath_forUPD() As Object
		Get
			Return Me.HtParameter("Set_PhotoPath_forUPD")
		End Get
		Set
			Me.HtParameter("Set_PhotoPath_forUPD") = value
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


	''' <summary>LastName_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property LastName_Like() As Object
		Get
			Return Me.HtParameter("LastName_Like")
		End Get
		Set
			Me.HtParameter("LastName_Like") = value
		End Set
	End Property


	''' <summary>FirstName_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property FirstName_Like() As Object
		Get
			Return Me.HtParameter("FirstName_Like")
		End Get
		Set
			Me.HtParameter("FirstName_Like") = value
		End Set
	End Property


	''' <summary>Title_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property Title_Like() As Object
		Get
			Return Me.HtParameter("Title_Like")
		End Get
		Set
			Me.HtParameter("Title_Like") = value
		End Set
	End Property


	''' <summary>TitleOfCourtesy_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property TitleOfCourtesy_Like() As Object
		Get
			Return Me.HtParameter("TitleOfCourtesy_Like")
		End Get
		Set
			Me.HtParameter("TitleOfCourtesy_Like") = value
		End Set
	End Property


	''' <summary>BirthDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property BirthDate_Like() As Object
		Get
			Return Me.HtParameter("BirthDate_Like")
		End Get
		Set
			Me.HtParameter("BirthDate_Like") = value
		End Set
	End Property


	''' <summary>HireDate_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property HireDate_Like() As Object
		Get
			Return Me.HtParameter("HireDate_Like")
		End Get
		Set
			Me.HtParameter("HireDate_Like") = value
		End Set
	End Property


	''' <summary>Address_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property Address_Like() As Object
		Get
			Return Me.HtParameter("Address_Like")
		End Get
		Set
			Me.HtParameter("Address_Like") = value
		End Set
	End Property


	''' <summary>City_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property City_Like() As Object
		Get
			Return Me.HtParameter("City_Like")
		End Get
		Set
			Me.HtParameter("City_Like") = value
		End Set
	End Property


	''' <summary>Region_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property Region_Like() As Object
		Get
			Return Me.HtParameter("Region_Like")
		End Get
		Set
			Me.HtParameter("Region_Like") = value
		End Set
	End Property


	''' <summary>PostalCode_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property PostalCode_Like() As Object
		Get
			Return Me.HtParameter("PostalCode_Like")
		End Get
		Set
			Me.HtParameter("PostalCode_Like") = value
		End Set
	End Property


	''' <summary>Country_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property Country_Like() As Object
		Get
			Return Me.HtParameter("Country_Like")
		End Get
		Set
			Me.HtParameter("Country_Like") = value
		End Set
	End Property


	''' <summary>HomePhone_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property HomePhone_Like() As Object
		Get
			Return Me.HtParameter("HomePhone_Like")
		End Get
		Set
			Me.HtParameter("HomePhone_Like") = value
		End Set
	End Property


	''' <summary>Extension_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property Extension_Like() As Object
		Get
			Return Me.HtParameter("Extension_Like")
		End Get
		Set
			Me.HtParameter("Extension_Like") = value
		End Set
	End Property


	''' <summary>Photo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property Photo_Like() As Object
		Get
			Return Me.HtParameter("Photo_Like")
		End Get
		Set
			Me.HtParameter("Photo_Like") = value
		End Set
	End Property


	''' <summary>Notes_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property Notes_Like() As Object
		Get
			Return Me.HtParameter("Notes_Like")
		End Get
		Set
			Me.HtParameter("Notes_Like") = value
		End Set
	End Property


	''' <summary>ReportsTo_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property ReportsTo_Like() As Object
		Get
			Return Me.HtParameter("ReportsTo_Like")
		End Get
		Set
			Me.HtParameter("ReportsTo_Like") = value
		End Set
	End Property


	''' <summary>PhotoPath_Like列に対するパラメタ ライズド クエリのパラメタを設定する。</summary>
	''' <remarks>動的参照処理時のLIKE検索で使用するパラメタ専用</remarks>
	Public Property PhotoPath_Like() As Object
		Get
			Return Me.HtParameter("PhotoPath_Like")
		End Get
		Set
			Me.HtParameter("PhotoPath_Like") = value
		End Set
	End Property


	#End Region

	#Region "クエリ メソッド"

	#Region "Insert"

	''' <summary>１レコード挿入する。</summary>
	''' <returns>挿入された行の数</returns>
	Public Function S1_Insert() As Integer
		' ファイルからSQL（Insert）を設定する。
		Me.SetSqlByFile2("DaoEmployees_S1_Insert.sql")

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
		Me.SetSqlByFile2("DaoEmployees_D1_Insert.xml")

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
		Me.SetSqlByFile2("DaoEmployees_S2_Select.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Select）を実行し、戻り値を戻す。
		Me.ExecSelectFill_DT(dt)
	End Sub

	''' <summary>検索条件を指定し、結果セットを参照する。</summary>
	''' <param name="dt">結果を格納するDataTable</param>
	Public Sub D2_Select(dt As DataTable)
		' ファイルからSQL（DynSel）を設定する。
		Me.SetSqlByFile2("DaoEmployees_D2_Select.xml")

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
		Me.SetSqlByFile2("DaoEmployees_S3_Update.xml")

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
		Me.SetSqlByFile2("DaoEmployees_D3_Update.xml")

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
		Me.SetSqlByFile2("DaoEmployees_S4_Delete.xml")

		' パラメタの設定
		Me.SetParametersFromHt()

		' SQL（Delete）を実行し、戻り値を戻す。
		Return Me.ExecInsUpDel_NonQuery()
	End Function

	''' <summary>任意の検索条件でデータを削除する。</summary>
	''' <returns>削除された行の数</returns>
	Public Function D4_Delete() As Integer
		' ファイルからSQL（DynDel）を設定する。
		Me.SetSqlByFile2("DaoEmployees_D4_Delete.xml")

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
		Me.SetSqlByFile2("DaoEmployees_D5_SelCnt.xml")

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

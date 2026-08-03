'**********************************************************************************
'* クラス名        ：SuppliersEntity
'* クラス日本語名  ：自動生成Entityクラス
'*
'* 作成日時        ：2014/2/9
'* 作成者          ：棟梁 D層自動生成ツール（墨壺）, 日立 太郎
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

' System～
Imports System

''' <summary>自動生成Entityクラス</summary>
<Serializable> _
Public Class SuppliersEntity
	#Region "メンバ変数"

	''' <summary>設定フラグ：SupplierID</summary>
	Public IsSetPK_SupplierID As Boolean = False

	''' <summary>メンバ変数：SupplierID</summary>
	Private _PK_SupplierID As Nullable(Of System.Int32)

	''' <summary>プロパティ：SupplierID</summary>
	Public Property PK_SupplierID() As Nullable(Of System.Int32)
		Get
			Return Me._PK_SupplierID
		End Get
		Set
			Me.IsSetPK_SupplierID = True
			Me._PK_SupplierID = value
		End Set
	End Property

	''' <summary>設定フラグ：CompanyName</summary>
	Public IsSet_CompanyName As Boolean = False

	''' <summary>メンバ変数：CompanyName</summary>
	Private _CompanyName As System.String

	''' <summary>プロパティ：CompanyName</summary>
	Public Property CompanyName() As System.String
		Get
			Return Me._CompanyName
		End Get
		Set
			Me.IsSet_CompanyName = True
			Me._CompanyName = value
		End Set
	End Property
	''' <summary>設定フラグ：ContactName</summary>
	Public IsSet_ContactName As Boolean = False

	''' <summary>メンバ変数：ContactName</summary>
	Private _ContactName As System.String

	''' <summary>プロパティ：ContactName</summary>
	Public Property ContactName() As System.String
		Get
			Return Me._ContactName
		End Get
		Set
			Me.IsSet_ContactName = True
			Me._ContactName = value
		End Set
	End Property
	''' <summary>設定フラグ：ContactTitle</summary>
	Public IsSet_ContactTitle As Boolean = False

	''' <summary>メンバ変数：ContactTitle</summary>
	Private _ContactTitle As System.String

	''' <summary>プロパティ：ContactTitle</summary>
	Public Property ContactTitle() As System.String
		Get
			Return Me._ContactTitle
		End Get
		Set
			Me.IsSet_ContactTitle = True
			Me._ContactTitle = value
		End Set
	End Property
	''' <summary>設定フラグ：Address</summary>
	Public IsSet_Address As Boolean = False

	''' <summary>メンバ変数：Address</summary>
	Private _Address As System.String

	''' <summary>プロパティ：Address</summary>
	Public Property Address() As System.String
		Get
			Return Me._Address
		End Get
		Set
			Me.IsSet_Address = True
			Me._Address = value
		End Set
	End Property
	''' <summary>設定フラグ：City</summary>
	Public IsSet_City As Boolean = False

	''' <summary>メンバ変数：City</summary>
	Private _City As System.String

	''' <summary>プロパティ：City</summary>
	Public Property City() As System.String
		Get
			Return Me._City
		End Get
		Set
			Me.IsSet_City = True
			Me._City = value
		End Set
	End Property
	''' <summary>設定フラグ：Region</summary>
	Public IsSet_Region As Boolean = False

	''' <summary>メンバ変数：Region</summary>
	Private _Region As System.String

	''' <summary>プロパティ：Region</summary>
	Public Property Region() As System.String
		Get
			Return Me._Region
		End Get
		Set
			Me.IsSet_Region = True
			Me._Region = value
		End Set
	End Property
	''' <summary>設定フラグ：PostalCode</summary>
	Public IsSet_PostalCode As Boolean = False

	''' <summary>メンバ変数：PostalCode</summary>
	Private _PostalCode As System.String

	''' <summary>プロパティ：PostalCode</summary>
	Public Property PostalCode() As System.String
		Get
			Return Me._PostalCode
		End Get
		Set
			Me.IsSet_PostalCode = True
			Me._PostalCode = value
		End Set
	End Property
	''' <summary>設定フラグ：Country</summary>
	Public IsSet_Country As Boolean = False

	''' <summary>メンバ変数：Country</summary>
	Private _Country As System.String

	''' <summary>プロパティ：Country</summary>
	Public Property Country() As System.String
		Get
			Return Me._Country
		End Get
		Set
			Me.IsSet_Country = True
			Me._Country = value
		End Set
	End Property
	''' <summary>設定フラグ：Phone</summary>
	Public IsSet_Phone As Boolean = False

	''' <summary>メンバ変数：Phone</summary>
	Private _Phone As System.String

	''' <summary>プロパティ：Phone</summary>
	Public Property Phone() As System.String
		Get
			Return Me._Phone
		End Get
		Set
			Me.IsSet_Phone = True
			Me._Phone = value
		End Set
	End Property
	''' <summary>設定フラグ：Fax</summary>
	Public IsSet_Fax As Boolean = False

	''' <summary>メンバ変数：Fax</summary>
	Private _Fax As System.String

	''' <summary>プロパティ：Fax</summary>
	Public Property Fax() As System.String
		Get
			Return Me._Fax
		End Get
		Set
			Me.IsSet_Fax = True
			Me._Fax = value
		End Set
	End Property
	''' <summary>設定フラグ：HomePage</summary>
	Public IsSet_HomePage As Boolean = False

	''' <summary>メンバ変数：HomePage</summary>
	Private _HomePage As System.String

	''' <summary>プロパティ：HomePage</summary>
	Public Property HomePage() As System.String
		Get
			Return Me._HomePage
		End Get
		Set
			Me.IsSet_HomePage = True
			Me._HomePage = value
		End Set
	End Property

	''' <summary>設定フラグ：Set_SupplierID_forUPD</summary>
	Public IsSet_Set_SupplierID_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_SupplierID_forUPD</summary>
	Private _Set_SupplierID_forUPD As Nullable(Of System.Int32)

	''' <summary>プロパティ：Set_SupplierID_forUPD</summary>
	Public Property Set_SupplierID_forUPD() As Nullable(Of System.Int32)
		Get
			Return Me._Set_SupplierID_forUPD
		End Get
		Set
			Me.IsSet_Set_SupplierID_forUPD = True
			Me._Set_SupplierID_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_CompanyName_forUPD</summary>
	Public IsSet_Set_CompanyName_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_CompanyName_forUPD</summary>
	Private _Set_CompanyName_forUPD As System.String

	''' <summary>プロパティ：Set_CompanyName_forUPD</summary>
	Public Property Set_CompanyName_forUPD() As System.String
		Get
			Return Me._Set_CompanyName_forUPD
		End Get
		Set
			Me.IsSet_Set_CompanyName_forUPD = True
			Me._Set_CompanyName_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ContactName_forUPD</summary>
	Public IsSet_Set_ContactName_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ContactName_forUPD</summary>
	Private _Set_ContactName_forUPD As System.String

	''' <summary>プロパティ：Set_ContactName_forUPD</summary>
	Public Property Set_ContactName_forUPD() As System.String
		Get
			Return Me._Set_ContactName_forUPD
		End Get
		Set
			Me.IsSet_Set_ContactName_forUPD = True
			Me._Set_ContactName_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ContactTitle_forUPD</summary>
	Public IsSet_Set_ContactTitle_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ContactTitle_forUPD</summary>
	Private _Set_ContactTitle_forUPD As System.String

	''' <summary>プロパティ：Set_ContactTitle_forUPD</summary>
	Public Property Set_ContactTitle_forUPD() As System.String
		Get
			Return Me._Set_ContactTitle_forUPD
		End Get
		Set
			Me.IsSet_Set_ContactTitle_forUPD = True
			Me._Set_ContactTitle_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Address_forUPD</summary>
	Public IsSet_Set_Address_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Address_forUPD</summary>
	Private _Set_Address_forUPD As System.String

	''' <summary>プロパティ：Set_Address_forUPD</summary>
	Public Property Set_Address_forUPD() As System.String
		Get
			Return Me._Set_Address_forUPD
		End Get
		Set
			Me.IsSet_Set_Address_forUPD = True
			Me._Set_Address_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_City_forUPD</summary>
	Public IsSet_Set_City_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_City_forUPD</summary>
	Private _Set_City_forUPD As System.String

	''' <summary>プロパティ：Set_City_forUPD</summary>
	Public Property Set_City_forUPD() As System.String
		Get
			Return Me._Set_City_forUPD
		End Get
		Set
			Me.IsSet_Set_City_forUPD = True
			Me._Set_City_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Region_forUPD</summary>
	Public IsSet_Set_Region_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Region_forUPD</summary>
	Private _Set_Region_forUPD As System.String

	''' <summary>プロパティ：Set_Region_forUPD</summary>
	Public Property Set_Region_forUPD() As System.String
		Get
			Return Me._Set_Region_forUPD
		End Get
		Set
			Me.IsSet_Set_Region_forUPD = True
			Me._Set_Region_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_PostalCode_forUPD</summary>
	Public IsSet_Set_PostalCode_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_PostalCode_forUPD</summary>
	Private _Set_PostalCode_forUPD As System.String

	''' <summary>プロパティ：Set_PostalCode_forUPD</summary>
	Public Property Set_PostalCode_forUPD() As System.String
		Get
			Return Me._Set_PostalCode_forUPD
		End Get
		Set
			Me.IsSet_Set_PostalCode_forUPD = True
			Me._Set_PostalCode_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Country_forUPD</summary>
	Public IsSet_Set_Country_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Country_forUPD</summary>
	Private _Set_Country_forUPD As System.String

	''' <summary>プロパティ：Set_Country_forUPD</summary>
	Public Property Set_Country_forUPD() As System.String
		Get
			Return Me._Set_Country_forUPD
		End Get
		Set
			Me.IsSet_Set_Country_forUPD = True
			Me._Set_Country_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Phone_forUPD</summary>
	Public IsSet_Set_Phone_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Phone_forUPD</summary>
	Private _Set_Phone_forUPD As System.String

	''' <summary>プロパティ：Set_Phone_forUPD</summary>
	Public Property Set_Phone_forUPD() As System.String
		Get
			Return Me._Set_Phone_forUPD
		End Get
		Set
			Me.IsSet_Set_Phone_forUPD = True
			Me._Set_Phone_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Fax_forUPD</summary>
	Public IsSet_Set_Fax_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Fax_forUPD</summary>
	Private _Set_Fax_forUPD As System.String

	''' <summary>プロパティ：Set_Fax_forUPD</summary>
	Public Property Set_Fax_forUPD() As System.String
		Get
			Return Me._Set_Fax_forUPD
		End Get
		Set
			Me.IsSet_Set_Fax_forUPD = True
			Me._Set_Fax_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_HomePage_forUPD</summary>
	Public IsSet_Set_HomePage_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_HomePage_forUPD</summary>
	Private _Set_HomePage_forUPD As System.String

	''' <summary>プロパティ：Set_HomePage_forUPD</summary>
	Public Property Set_HomePage_forUPD() As System.String
		Get
			Return Me._Set_HomePage_forUPD
		End Get
		Set
			Me.IsSet_Set_HomePage_forUPD = True
			Me._Set_HomePage_forUPD = value
		End Set
	End Property

	''' <summary>設定フラグ：SupplierID_Like</summary>
	Public IsSet_SupplierID_Like As Boolean = False

	''' <summary>メンバ変数：SupplierID_Like</summary>
	Private _SupplierID_Like As Nullable(Of System.Int32)

	''' <summary>プロパティ：SupplierID_Like</summary>
	Public Property SupplierID_Like() As Nullable(Of System.Int32)
		Get
			Return Me._SupplierID_Like
		End Get
		Set
			Me.IsSet_SupplierID_Like = True
			Me._SupplierID_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：CompanyName_Like</summary>
	Public IsSet_CompanyName_Like As Boolean = False

	''' <summary>メンバ変数：CompanyName_Like</summary>
	Private _CompanyName_Like As System.String

	''' <summary>プロパティ：CompanyName_Like</summary>
	Public Property CompanyName_Like() As System.String
		Get
			Return Me._CompanyName_Like
		End Get
		Set
			Me.IsSet_CompanyName_Like = True
			Me._CompanyName_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ContactName_Like</summary>
	Public IsSet_ContactName_Like As Boolean = False

	''' <summary>メンバ変数：ContactName_Like</summary>
	Private _ContactName_Like As System.String

	''' <summary>プロパティ：ContactName_Like</summary>
	Public Property ContactName_Like() As System.String
		Get
			Return Me._ContactName_Like
		End Get
		Set
			Me.IsSet_ContactName_Like = True
			Me._ContactName_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ContactTitle_Like</summary>
	Public IsSet_ContactTitle_Like As Boolean = False

	''' <summary>メンバ変数：ContactTitle_Like</summary>
	Private _ContactTitle_Like As System.String

	''' <summary>プロパティ：ContactTitle_Like</summary>
	Public Property ContactTitle_Like() As System.String
		Get
			Return Me._ContactTitle_Like
		End Get
		Set
			Me.IsSet_ContactTitle_Like = True
			Me._ContactTitle_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Address_Like</summary>
	Public IsSet_Address_Like As Boolean = False

	''' <summary>メンバ変数：Address_Like</summary>
	Private _Address_Like As System.String

	''' <summary>プロパティ：Address_Like</summary>
	Public Property Address_Like() As System.String
		Get
			Return Me._Address_Like
		End Get
		Set
			Me.IsSet_Address_Like = True
			Me._Address_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：City_Like</summary>
	Public IsSet_City_Like As Boolean = False

	''' <summary>メンバ変数：City_Like</summary>
	Private _City_Like As System.String

	''' <summary>プロパティ：City_Like</summary>
	Public Property City_Like() As System.String
		Get
			Return Me._City_Like
		End Get
		Set
			Me.IsSet_City_Like = True
			Me._City_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Region_Like</summary>
	Public IsSet_Region_Like As Boolean = False

	''' <summary>メンバ変数：Region_Like</summary>
	Private _Region_Like As System.String

	''' <summary>プロパティ：Region_Like</summary>
	Public Property Region_Like() As System.String
		Get
			Return Me._Region_Like
		End Get
		Set
			Me.IsSet_Region_Like = True
			Me._Region_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：PostalCode_Like</summary>
	Public IsSet_PostalCode_Like As Boolean = False

	''' <summary>メンバ変数：PostalCode_Like</summary>
	Private _PostalCode_Like As System.String

	''' <summary>プロパティ：PostalCode_Like</summary>
	Public Property PostalCode_Like() As System.String
		Get
			Return Me._PostalCode_Like
		End Get
		Set
			Me.IsSet_PostalCode_Like = True
			Me._PostalCode_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Country_Like</summary>
	Public IsSet_Country_Like As Boolean = False

	''' <summary>メンバ変数：Country_Like</summary>
	Private _Country_Like As System.String

	''' <summary>プロパティ：Country_Like</summary>
	Public Property Country_Like() As System.String
		Get
			Return Me._Country_Like
		End Get
		Set
			Me.IsSet_Country_Like = True
			Me._Country_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Phone_Like</summary>
	Public IsSet_Phone_Like As Boolean = False

	''' <summary>メンバ変数：Phone_Like</summary>
	Private _Phone_Like As System.String

	''' <summary>プロパティ：Phone_Like</summary>
	Public Property Phone_Like() As System.String
		Get
			Return Me._Phone_Like
		End Get
		Set
			Me.IsSet_Phone_Like = True
			Me._Phone_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Fax_Like</summary>
	Public IsSet_Fax_Like As Boolean = False

	''' <summary>メンバ変数：Fax_Like</summary>
	Private _Fax_Like As System.String

	''' <summary>プロパティ：Fax_Like</summary>
	Public Property Fax_Like() As System.String
		Get
			Return Me._Fax_Like
		End Get
		Set
			Me.IsSet_Fax_Like = True
			Me._Fax_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：HomePage_Like</summary>
	Public IsSet_HomePage_Like As Boolean = False

	''' <summary>メンバ変数：HomePage_Like</summary>
	Private _HomePage_Like As System.String

	''' <summary>プロパティ：HomePage_Like</summary>
	Public Property HomePage_Like() As System.String
		Get
			Return Me._HomePage_Like
		End Get
		Set
			Me.IsSet_HomePage_Like = True
			Me._HomePage_Like = value
		End Set
	End Property

	#End Region
End Class

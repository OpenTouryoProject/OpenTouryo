'**********************************************************************************
'* クラス名        ：EmployeesEntity
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
Public Class EmployeesEntity
	#Region "メンバ変数"

	''' <summary>設定フラグ：EmployeeID</summary>
	Public IsSetPK_EmployeeID As Boolean = False

	''' <summary>メンバ変数：EmployeeID</summary>
	Private _PK_EmployeeID As Nullable(Of System.Int32)

	''' <summary>プロパティ：EmployeeID</summary>
	Public Property PK_EmployeeID() As Nullable(Of System.Int32)
		Get
			Return Me._PK_EmployeeID
		End Get
		Set
			Me.IsSetPK_EmployeeID = True
			Me._PK_EmployeeID = value
		End Set
	End Property

	''' <summary>設定フラグ：LastName</summary>
	Public IsSet_LastName As Boolean = False

	''' <summary>メンバ変数：LastName</summary>
	Private _LastName As System.String

	''' <summary>プロパティ：LastName</summary>
	Public Property LastName() As System.String
		Get
			Return Me._LastName
		End Get
		Set
			Me.IsSet_LastName = True
			Me._LastName = value
		End Set
	End Property
	''' <summary>設定フラグ：FirstName</summary>
	Public IsSet_FirstName As Boolean = False

	''' <summary>メンバ変数：FirstName</summary>
	Private _FirstName As System.String

	''' <summary>プロパティ：FirstName</summary>
	Public Property FirstName() As System.String
		Get
			Return Me._FirstName
		End Get
		Set
			Me.IsSet_FirstName = True
			Me._FirstName = value
		End Set
	End Property
	''' <summary>設定フラグ：Title</summary>
	Public IsSet_Title As Boolean = False

	''' <summary>メンバ変数：Title</summary>
	Private _Title As System.String

	''' <summary>プロパティ：Title</summary>
	Public Property Title() As System.String
		Get
			Return Me._Title
		End Get
		Set
			Me.IsSet_Title = True
			Me._Title = value
		End Set
	End Property
	''' <summary>設定フラグ：TitleOfCourtesy</summary>
	Public IsSet_TitleOfCourtesy As Boolean = False

	''' <summary>メンバ変数：TitleOfCourtesy</summary>
	Private _TitleOfCourtesy As System.String

	''' <summary>プロパティ：TitleOfCourtesy</summary>
	Public Property TitleOfCourtesy() As System.String
		Get
			Return Me._TitleOfCourtesy
		End Get
		Set
			Me.IsSet_TitleOfCourtesy = True
			Me._TitleOfCourtesy = value
		End Set
	End Property
	''' <summary>設定フラグ：BirthDate</summary>
	Public IsSet_BirthDate As Boolean = False

	''' <summary>メンバ変数：BirthDate</summary>
	Private _BirthDate As Nullable(Of System.DateTime)

	''' <summary>プロパティ：BirthDate</summary>
	Public Property BirthDate() As Nullable(Of System.DateTime)
		Get
			Return Me._BirthDate
		End Get
		Set
			Me.IsSet_BirthDate = True
			Me._BirthDate = value
		End Set
	End Property
	''' <summary>設定フラグ：HireDate</summary>
	Public IsSet_HireDate As Boolean = False

	''' <summary>メンバ変数：HireDate</summary>
	Private _HireDate As Nullable(Of System.DateTime)

	''' <summary>プロパティ：HireDate</summary>
	Public Property HireDate() As Nullable(Of System.DateTime)
		Get
			Return Me._HireDate
		End Get
		Set
			Me.IsSet_HireDate = True
			Me._HireDate = value
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
	''' <summary>設定フラグ：HomePhone</summary>
	Public IsSet_HomePhone As Boolean = False

	''' <summary>メンバ変数：HomePhone</summary>
	Private _HomePhone As System.String

	''' <summary>プロパティ：HomePhone</summary>
	Public Property HomePhone() As System.String
		Get
			Return Me._HomePhone
		End Get
		Set
			Me.IsSet_HomePhone = True
			Me._HomePhone = value
		End Set
	End Property
	''' <summary>設定フラグ：Extension</summary>
	Public IsSet_Extension As Boolean = False

	''' <summary>メンバ変数：Extension</summary>
	Private _Extension As System.String

	''' <summary>プロパティ：Extension</summary>
	Public Property Extension() As System.String
		Get
			Return Me._Extension
		End Get
		Set
			Me.IsSet_Extension = True
			Me._Extension = value
		End Set
	End Property
	''' <summary>設定フラグ：Photo</summary>
	Public IsSet_Photo As Boolean = False

	''' <summary>メンバ変数：Photo</summary>
	Private _Photo As System.Byte()

	''' <summary>プロパティ：Photo</summary>
	Public Property Photo() As System.Byte()
		Get
			Return Me._Photo
		End Get
		Set
			Me.IsSet_Photo = True
			Me._Photo = value
		End Set
	End Property
	''' <summary>設定フラグ：Notes</summary>
	Public IsSet_Notes As Boolean = False

	''' <summary>メンバ変数：Notes</summary>
	Private _Notes As System.String

	''' <summary>プロパティ：Notes</summary>
	Public Property Notes() As System.String
		Get
			Return Me._Notes
		End Get
		Set
			Me.IsSet_Notes = True
			Me._Notes = value
		End Set
	End Property
	''' <summary>設定フラグ：ReportsTo</summary>
	Public IsSet_ReportsTo As Boolean = False

	''' <summary>メンバ変数：ReportsTo</summary>
	Private _ReportsTo As Nullable(Of System.Int32)

	''' <summary>プロパティ：ReportsTo</summary>
	Public Property ReportsTo() As Nullable(Of System.Int32)
		Get
			Return Me._ReportsTo
		End Get
		Set
			Me.IsSet_ReportsTo = True
			Me._ReportsTo = value
		End Set
	End Property
	''' <summary>設定フラグ：PhotoPath</summary>
	Public IsSet_PhotoPath As Boolean = False

	''' <summary>メンバ変数：PhotoPath</summary>
	Private _PhotoPath As System.String

	''' <summary>プロパティ：PhotoPath</summary>
	Public Property PhotoPath() As System.String
		Get
			Return Me._PhotoPath
		End Get
		Set
			Me.IsSet_PhotoPath = True
			Me._PhotoPath = value
		End Set
	End Property

	''' <summary>設定フラグ：Set_EmployeeID_forUPD</summary>
	Public IsSet_Set_EmployeeID_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_EmployeeID_forUPD</summary>
	Private _Set_EmployeeID_forUPD As Nullable(Of System.Int32)

	''' <summary>プロパティ：Set_EmployeeID_forUPD</summary>
	Public Property Set_EmployeeID_forUPD() As Nullable(Of System.Int32)
		Get
			Return Me._Set_EmployeeID_forUPD
		End Get
		Set
			Me.IsSet_Set_EmployeeID_forUPD = True
			Me._Set_EmployeeID_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_LastName_forUPD</summary>
	Public IsSet_Set_LastName_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_LastName_forUPD</summary>
	Private _Set_LastName_forUPD As System.String

	''' <summary>プロパティ：Set_LastName_forUPD</summary>
	Public Property Set_LastName_forUPD() As System.String
		Get
			Return Me._Set_LastName_forUPD
		End Get
		Set
			Me.IsSet_Set_LastName_forUPD = True
			Me._Set_LastName_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_FirstName_forUPD</summary>
	Public IsSet_Set_FirstName_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_FirstName_forUPD</summary>
	Private _Set_FirstName_forUPD As System.String

	''' <summary>プロパティ：Set_FirstName_forUPD</summary>
	Public Property Set_FirstName_forUPD() As System.String
		Get
			Return Me._Set_FirstName_forUPD
		End Get
		Set
			Me.IsSet_Set_FirstName_forUPD = True
			Me._Set_FirstName_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Title_forUPD</summary>
	Public IsSet_Set_Title_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Title_forUPD</summary>
	Private _Set_Title_forUPD As System.String

	''' <summary>プロパティ：Set_Title_forUPD</summary>
	Public Property Set_Title_forUPD() As System.String
		Get
			Return Me._Set_Title_forUPD
		End Get
		Set
			Me.IsSet_Set_Title_forUPD = True
			Me._Set_Title_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_TitleOfCourtesy_forUPD</summary>
	Public IsSet_Set_TitleOfCourtesy_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_TitleOfCourtesy_forUPD</summary>
	Private _Set_TitleOfCourtesy_forUPD As System.String

	''' <summary>プロパティ：Set_TitleOfCourtesy_forUPD</summary>
	Public Property Set_TitleOfCourtesy_forUPD() As System.String
		Get
			Return Me._Set_TitleOfCourtesy_forUPD
		End Get
		Set
			Me.IsSet_Set_TitleOfCourtesy_forUPD = True
			Me._Set_TitleOfCourtesy_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_BirthDate_forUPD</summary>
	Public IsSet_Set_BirthDate_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_BirthDate_forUPD</summary>
	Private _Set_BirthDate_forUPD As Nullable(Of System.DateTime)

	''' <summary>プロパティ：Set_BirthDate_forUPD</summary>
	Public Property Set_BirthDate_forUPD() As Nullable(Of System.DateTime)
		Get
			Return Me._Set_BirthDate_forUPD
		End Get
		Set
			Me.IsSet_Set_BirthDate_forUPD = True
			Me._Set_BirthDate_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_HireDate_forUPD</summary>
	Public IsSet_Set_HireDate_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_HireDate_forUPD</summary>
	Private _Set_HireDate_forUPD As Nullable(Of System.DateTime)

	''' <summary>プロパティ：Set_HireDate_forUPD</summary>
	Public Property Set_HireDate_forUPD() As Nullable(Of System.DateTime)
		Get
			Return Me._Set_HireDate_forUPD
		End Get
		Set
			Me.IsSet_Set_HireDate_forUPD = True
			Me._Set_HireDate_forUPD = value
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
	''' <summary>設定フラグ：Set_HomePhone_forUPD</summary>
	Public IsSet_Set_HomePhone_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_HomePhone_forUPD</summary>
	Private _Set_HomePhone_forUPD As System.String

	''' <summary>プロパティ：Set_HomePhone_forUPD</summary>
	Public Property Set_HomePhone_forUPD() As System.String
		Get
			Return Me._Set_HomePhone_forUPD
		End Get
		Set
			Me.IsSet_Set_HomePhone_forUPD = True
			Me._Set_HomePhone_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Extension_forUPD</summary>
	Public IsSet_Set_Extension_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Extension_forUPD</summary>
	Private _Set_Extension_forUPD As System.String

	''' <summary>プロパティ：Set_Extension_forUPD</summary>
	Public Property Set_Extension_forUPD() As System.String
		Get
			Return Me._Set_Extension_forUPD
		End Get
		Set
			Me.IsSet_Set_Extension_forUPD = True
			Me._Set_Extension_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Photo_forUPD</summary>
	Public IsSet_Set_Photo_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Photo_forUPD</summary>
	Private _Set_Photo_forUPD As System.Byte()

	''' <summary>プロパティ：Set_Photo_forUPD</summary>
	Public Property Set_Photo_forUPD() As System.Byte()
		Get
			Return Me._Set_Photo_forUPD
		End Get
		Set
			Me.IsSet_Set_Photo_forUPD = True
			Me._Set_Photo_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Notes_forUPD</summary>
	Public IsSet_Set_Notes_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Notes_forUPD</summary>
	Private _Set_Notes_forUPD As System.String

	''' <summary>プロパティ：Set_Notes_forUPD</summary>
	Public Property Set_Notes_forUPD() As System.String
		Get
			Return Me._Set_Notes_forUPD
		End Get
		Set
			Me.IsSet_Set_Notes_forUPD = True
			Me._Set_Notes_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ReportsTo_forUPD</summary>
	Public IsSet_Set_ReportsTo_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ReportsTo_forUPD</summary>
	Private _Set_ReportsTo_forUPD As Nullable(Of System.Int32)

	''' <summary>プロパティ：Set_ReportsTo_forUPD</summary>
	Public Property Set_ReportsTo_forUPD() As Nullable(Of System.Int32)
		Get
			Return Me._Set_ReportsTo_forUPD
		End Get
		Set
			Me.IsSet_Set_ReportsTo_forUPD = True
			Me._Set_ReportsTo_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_PhotoPath_forUPD</summary>
	Public IsSet_Set_PhotoPath_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_PhotoPath_forUPD</summary>
	Private _Set_PhotoPath_forUPD As System.String

	''' <summary>プロパティ：Set_PhotoPath_forUPD</summary>
	Public Property Set_PhotoPath_forUPD() As System.String
		Get
			Return Me._Set_PhotoPath_forUPD
		End Get
		Set
			Me.IsSet_Set_PhotoPath_forUPD = True
			Me._Set_PhotoPath_forUPD = value
		End Set
	End Property

	''' <summary>設定フラグ：EmployeeID_Like</summary>
	Public IsSet_EmployeeID_Like As Boolean = False

	''' <summary>メンバ変数：EmployeeID_Like</summary>
	Private _EmployeeID_Like As Nullable(Of System.Int32)

	''' <summary>プロパティ：EmployeeID_Like</summary>
	Public Property EmployeeID_Like() As Nullable(Of System.Int32)
		Get
			Return Me._EmployeeID_Like
		End Get
		Set
			Me.IsSet_EmployeeID_Like = True
			Me._EmployeeID_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：LastName_Like</summary>
	Public IsSet_LastName_Like As Boolean = False

	''' <summary>メンバ変数：LastName_Like</summary>
	Private _LastName_Like As System.String

	''' <summary>プロパティ：LastName_Like</summary>
	Public Property LastName_Like() As System.String
		Get
			Return Me._LastName_Like
		End Get
		Set
			Me.IsSet_LastName_Like = True
			Me._LastName_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：FirstName_Like</summary>
	Public IsSet_FirstName_Like As Boolean = False

	''' <summary>メンバ変数：FirstName_Like</summary>
	Private _FirstName_Like As System.String

	''' <summary>プロパティ：FirstName_Like</summary>
	Public Property FirstName_Like() As System.String
		Get
			Return Me._FirstName_Like
		End Get
		Set
			Me.IsSet_FirstName_Like = True
			Me._FirstName_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Title_Like</summary>
	Public IsSet_Title_Like As Boolean = False

	''' <summary>メンバ変数：Title_Like</summary>
	Private _Title_Like As System.String

	''' <summary>プロパティ：Title_Like</summary>
	Public Property Title_Like() As System.String
		Get
			Return Me._Title_Like
		End Get
		Set
			Me.IsSet_Title_Like = True
			Me._Title_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：TitleOfCourtesy_Like</summary>
	Public IsSet_TitleOfCourtesy_Like As Boolean = False

	''' <summary>メンバ変数：TitleOfCourtesy_Like</summary>
	Private _TitleOfCourtesy_Like As System.String

	''' <summary>プロパティ：TitleOfCourtesy_Like</summary>
	Public Property TitleOfCourtesy_Like() As System.String
		Get
			Return Me._TitleOfCourtesy_Like
		End Get
		Set
			Me.IsSet_TitleOfCourtesy_Like = True
			Me._TitleOfCourtesy_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：BirthDate_Like</summary>
	Public IsSet_BirthDate_Like As Boolean = False

	''' <summary>メンバ変数：BirthDate_Like</summary>
	Private _BirthDate_Like As Nullable(Of System.DateTime)

	''' <summary>プロパティ：BirthDate_Like</summary>
	Public Property BirthDate_Like() As Nullable(Of System.DateTime)
		Get
			Return Me._BirthDate_Like
		End Get
		Set
			Me.IsSet_BirthDate_Like = True
			Me._BirthDate_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：HireDate_Like</summary>
	Public IsSet_HireDate_Like As Boolean = False

	''' <summary>メンバ変数：HireDate_Like</summary>
	Private _HireDate_Like As Nullable(Of System.DateTime)

	''' <summary>プロパティ：HireDate_Like</summary>
	Public Property HireDate_Like() As Nullable(Of System.DateTime)
		Get
			Return Me._HireDate_Like
		End Get
		Set
			Me.IsSet_HireDate_Like = True
			Me._HireDate_Like = value
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
	''' <summary>設定フラグ：HomePhone_Like</summary>
	Public IsSet_HomePhone_Like As Boolean = False

	''' <summary>メンバ変数：HomePhone_Like</summary>
	Private _HomePhone_Like As System.String

	''' <summary>プロパティ：HomePhone_Like</summary>
	Public Property HomePhone_Like() As System.String
		Get
			Return Me._HomePhone_Like
		End Get
		Set
			Me.IsSet_HomePhone_Like = True
			Me._HomePhone_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Extension_Like</summary>
	Public IsSet_Extension_Like As Boolean = False

	''' <summary>メンバ変数：Extension_Like</summary>
	Private _Extension_Like As System.String

	''' <summary>プロパティ：Extension_Like</summary>
	Public Property Extension_Like() As System.String
		Get
			Return Me._Extension_Like
		End Get
		Set
			Me.IsSet_Extension_Like = True
			Me._Extension_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Photo_Like</summary>
	Public IsSet_Photo_Like As Boolean = False

	''' <summary>メンバ変数：Photo_Like</summary>
	Private _Photo_Like As System.Byte()

	''' <summary>プロパティ：Photo_Like</summary>
	Public Property Photo_Like() As System.Byte()
		Get
			Return Me._Photo_Like
		End Get
		Set
			Me.IsSet_Photo_Like = True
			Me._Photo_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Notes_Like</summary>
	Public IsSet_Notes_Like As Boolean = False

	''' <summary>メンバ変数：Notes_Like</summary>
	Private _Notes_Like As System.String

	''' <summary>プロパティ：Notes_Like</summary>
	Public Property Notes_Like() As System.String
		Get
			Return Me._Notes_Like
		End Get
		Set
			Me.IsSet_Notes_Like = True
			Me._Notes_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ReportsTo_Like</summary>
	Public IsSet_ReportsTo_Like As Boolean = False

	''' <summary>メンバ変数：ReportsTo_Like</summary>
	Private _ReportsTo_Like As Nullable(Of System.Int32)

	''' <summary>プロパティ：ReportsTo_Like</summary>
	Public Property ReportsTo_Like() As Nullable(Of System.Int32)
		Get
			Return Me._ReportsTo_Like
		End Get
		Set
			Me.IsSet_ReportsTo_Like = True
			Me._ReportsTo_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：PhotoPath_Like</summary>
	Public IsSet_PhotoPath_Like As Boolean = False

	''' <summary>メンバ変数：PhotoPath_Like</summary>
	Private _PhotoPath_Like As System.String

	''' <summary>プロパティ：PhotoPath_Like</summary>
	Public Property PhotoPath_Like() As System.String
		Get
			Return Me._PhotoPath_Like
		End Get
		Set
			Me.IsSet_PhotoPath_Like = True
			Me._PhotoPath_Like = value
		End Set
	End Property

	#End Region
End Class

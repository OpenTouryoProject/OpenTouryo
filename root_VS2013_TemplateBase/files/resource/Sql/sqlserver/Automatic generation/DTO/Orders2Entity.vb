'**********************************************************************************
'* クラス名        ：Orders2Entity
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
Public Class Orders2Entity
	#Region "メンバ変数"

	''' <summary>設定フラグ：OrderID</summary>
	Public IsSetPK_OrderID As Boolean = False

	''' <summary>メンバ変数：OrderID</summary>
	Private _PK_OrderID As Nullable(Of System.Int32)

	''' <summary>プロパティ：OrderID</summary>
	Public Property PK_OrderID() As Nullable(Of System.Int32)
		Get
			Return Me._PK_OrderID
		End Get
		Set
			Me.IsSetPK_OrderID = True
			Me._PK_OrderID = value
		End Set
	End Property

	''' <summary>設定フラグ：CustomerID</summary>
	Public IsSet_CustomerID As Boolean = False

	''' <summary>メンバ変数：CustomerID</summary>
	Private _CustomerID As System.String

	''' <summary>プロパティ：CustomerID</summary>
	Public Property CustomerID() As System.String
		Get
			Return Me._CustomerID
		End Get
		Set
			Me.IsSet_CustomerID = True
			Me._CustomerID = value
		End Set
	End Property
	''' <summary>設定フラグ：EmployeeID</summary>
	Public IsSet_EmployeeID As Boolean = False

	''' <summary>メンバ変数：EmployeeID</summary>
	Private _EmployeeID As Nullable(Of System.Int32)

	''' <summary>プロパティ：EmployeeID</summary>
	Public Property EmployeeID() As Nullable(Of System.Int32)
		Get
			Return Me._EmployeeID
		End Get
		Set
			Me.IsSet_EmployeeID = True
			Me._EmployeeID = value
		End Set
	End Property
	''' <summary>設定フラグ：OrderDate</summary>
	Public IsSet_OrderDate As Boolean = False

	''' <summary>メンバ変数：OrderDate</summary>
	Private _OrderDate As Nullable(Of System.DateTime)

	''' <summary>プロパティ：OrderDate</summary>
	Public Property OrderDate() As Nullable(Of System.DateTime)
		Get
			Return Me._OrderDate
		End Get
		Set
			Me.IsSet_OrderDate = True
			Me._OrderDate = value
		End Set
	End Property
	''' <summary>設定フラグ：RequiredDate</summary>
	Public IsSet_RequiredDate As Boolean = False

	''' <summary>メンバ変数：RequiredDate</summary>
	Private _RequiredDate As Nullable(Of System.DateTime)

	''' <summary>プロパティ：RequiredDate</summary>
	Public Property RequiredDate() As Nullable(Of System.DateTime)
		Get
			Return Me._RequiredDate
		End Get
		Set
			Me.IsSet_RequiredDate = True
			Me._RequiredDate = value
		End Set
	End Property
	''' <summary>設定フラグ：ShippedDate</summary>
	Public IsSet_ShippedDate As Boolean = False

	''' <summary>メンバ変数：ShippedDate</summary>
	Private _ShippedDate As Nullable(Of System.DateTime)

	''' <summary>プロパティ：ShippedDate</summary>
	Public Property ShippedDate() As Nullable(Of System.DateTime)
		Get
			Return Me._ShippedDate
		End Get
		Set
			Me.IsSet_ShippedDate = True
			Me._ShippedDate = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipVia</summary>
	Public IsSet_ShipVia As Boolean = False

	''' <summary>メンバ変数：ShipVia</summary>
	Private _ShipVia As Nullable(Of System.Int32)

	''' <summary>プロパティ：ShipVia</summary>
	Public Property ShipVia() As Nullable(Of System.Int32)
		Get
			Return Me._ShipVia
		End Get
		Set
			Me.IsSet_ShipVia = True
			Me._ShipVia = value
		End Set
	End Property
	''' <summary>設定フラグ：Freight</summary>
	Public IsSet_Freight As Boolean = False

	''' <summary>メンバ変数：Freight</summary>
	Private _Freight As Nullable(Of System.Decimal)

	''' <summary>プロパティ：Freight</summary>
	Public Property Freight() As Nullable(Of System.Decimal)
		Get
			Return Me._Freight
		End Get
		Set
			Me.IsSet_Freight = True
			Me._Freight = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipName</summary>
	Public IsSet_ShipName As Boolean = False

	''' <summary>メンバ変数：ShipName</summary>
	Private _ShipName As System.String

	''' <summary>プロパティ：ShipName</summary>
	Public Property ShipName() As System.String
		Get
			Return Me._ShipName
		End Get
		Set
			Me.IsSet_ShipName = True
			Me._ShipName = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipAddress</summary>
	Public IsSet_ShipAddress As Boolean = False

	''' <summary>メンバ変数：ShipAddress</summary>
	Private _ShipAddress As System.String

	''' <summary>プロパティ：ShipAddress</summary>
	Public Property ShipAddress() As System.String
		Get
			Return Me._ShipAddress
		End Get
		Set
			Me.IsSet_ShipAddress = True
			Me._ShipAddress = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipCity</summary>
	Public IsSet_ShipCity As Boolean = False

	''' <summary>メンバ変数：ShipCity</summary>
	Private _ShipCity As System.String

	''' <summary>プロパティ：ShipCity</summary>
	Public Property ShipCity() As System.String
		Get
			Return Me._ShipCity
		End Get
		Set
			Me.IsSet_ShipCity = True
			Me._ShipCity = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipRegion</summary>
	Public IsSet_ShipRegion As Boolean = False

	''' <summary>メンバ変数：ShipRegion</summary>
	Private _ShipRegion As System.String

	''' <summary>プロパティ：ShipRegion</summary>
	Public Property ShipRegion() As System.String
		Get
			Return Me._ShipRegion
		End Get
		Set
			Me.IsSet_ShipRegion = True
			Me._ShipRegion = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipPostalCode</summary>
	Public IsSet_ShipPostalCode As Boolean = False

	''' <summary>メンバ変数：ShipPostalCode</summary>
	Private _ShipPostalCode As System.String

	''' <summary>プロパティ：ShipPostalCode</summary>
	Public Property ShipPostalCode() As System.String
		Get
			Return Me._ShipPostalCode
		End Get
		Set
			Me.IsSet_ShipPostalCode = True
			Me._ShipPostalCode = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipCountry</summary>
	Public IsSet_ShipCountry As Boolean = False

	''' <summary>メンバ変数：ShipCountry</summary>
	Private _ShipCountry As System.String

	''' <summary>プロパティ：ShipCountry</summary>
	Public Property ShipCountry() As System.String
		Get
			Return Me._ShipCountry
		End Get
		Set
			Me.IsSet_ShipCountry = True
			Me._ShipCountry = value
		End Set
	End Property

	''' <summary>設定フラグ：Set_OrderID_forUPD</summary>
	Public IsSet_Set_OrderID_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_OrderID_forUPD</summary>
	Private _Set_OrderID_forUPD As Nullable(Of System.Int32)

	''' <summary>プロパティ：Set_OrderID_forUPD</summary>
	Public Property Set_OrderID_forUPD() As Nullable(Of System.Int32)
		Get
			Return Me._Set_OrderID_forUPD
		End Get
		Set
			Me.IsSet_Set_OrderID_forUPD = True
			Me._Set_OrderID_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_CustomerID_forUPD</summary>
	Public IsSet_Set_CustomerID_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_CustomerID_forUPD</summary>
	Private _Set_CustomerID_forUPD As System.String

	''' <summary>プロパティ：Set_CustomerID_forUPD</summary>
	Public Property Set_CustomerID_forUPD() As System.String
		Get
			Return Me._Set_CustomerID_forUPD
		End Get
		Set
			Me.IsSet_Set_CustomerID_forUPD = True
			Me._Set_CustomerID_forUPD = value
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
	''' <summary>設定フラグ：Set_OrderDate_forUPD</summary>
	Public IsSet_Set_OrderDate_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_OrderDate_forUPD</summary>
	Private _Set_OrderDate_forUPD As Nullable(Of System.DateTime)

	''' <summary>プロパティ：Set_OrderDate_forUPD</summary>
	Public Property Set_OrderDate_forUPD() As Nullable(Of System.DateTime)
		Get
			Return Me._Set_OrderDate_forUPD
		End Get
		Set
			Me.IsSet_Set_OrderDate_forUPD = True
			Me._Set_OrderDate_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_RequiredDate_forUPD</summary>
	Public IsSet_Set_RequiredDate_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_RequiredDate_forUPD</summary>
	Private _Set_RequiredDate_forUPD As Nullable(Of System.DateTime)

	''' <summary>プロパティ：Set_RequiredDate_forUPD</summary>
	Public Property Set_RequiredDate_forUPD() As Nullable(Of System.DateTime)
		Get
			Return Me._Set_RequiredDate_forUPD
		End Get
		Set
			Me.IsSet_Set_RequiredDate_forUPD = True
			Me._Set_RequiredDate_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ShippedDate_forUPD</summary>
	Public IsSet_Set_ShippedDate_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ShippedDate_forUPD</summary>
	Private _Set_ShippedDate_forUPD As Nullable(Of System.DateTime)

	''' <summary>プロパティ：Set_ShippedDate_forUPD</summary>
	Public Property Set_ShippedDate_forUPD() As Nullable(Of System.DateTime)
		Get
			Return Me._Set_ShippedDate_forUPD
		End Get
		Set
			Me.IsSet_Set_ShippedDate_forUPD = True
			Me._Set_ShippedDate_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ShipVia_forUPD</summary>
	Public IsSet_Set_ShipVia_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ShipVia_forUPD</summary>
	Private _Set_ShipVia_forUPD As Nullable(Of System.Int32)

	''' <summary>プロパティ：Set_ShipVia_forUPD</summary>
	Public Property Set_ShipVia_forUPD() As Nullable(Of System.Int32)
		Get
			Return Me._Set_ShipVia_forUPD
		End Get
		Set
			Me.IsSet_Set_ShipVia_forUPD = True
			Me._Set_ShipVia_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Freight_forUPD</summary>
	Public IsSet_Set_Freight_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Freight_forUPD</summary>
	Private _Set_Freight_forUPD As Nullable(Of System.Decimal)

	''' <summary>プロパティ：Set_Freight_forUPD</summary>
	Public Property Set_Freight_forUPD() As Nullable(Of System.Decimal)
		Get
			Return Me._Set_Freight_forUPD
		End Get
		Set
			Me.IsSet_Set_Freight_forUPD = True
			Me._Set_Freight_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ShipName_forUPD</summary>
	Public IsSet_Set_ShipName_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ShipName_forUPD</summary>
	Private _Set_ShipName_forUPD As System.String

	''' <summary>プロパティ：Set_ShipName_forUPD</summary>
	Public Property Set_ShipName_forUPD() As System.String
		Get
			Return Me._Set_ShipName_forUPD
		End Get
		Set
			Me.IsSet_Set_ShipName_forUPD = True
			Me._Set_ShipName_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ShipAddress_forUPD</summary>
	Public IsSet_Set_ShipAddress_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ShipAddress_forUPD</summary>
	Private _Set_ShipAddress_forUPD As System.String

	''' <summary>プロパティ：Set_ShipAddress_forUPD</summary>
	Public Property Set_ShipAddress_forUPD() As System.String
		Get
			Return Me._Set_ShipAddress_forUPD
		End Get
		Set
			Me.IsSet_Set_ShipAddress_forUPD = True
			Me._Set_ShipAddress_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ShipCity_forUPD</summary>
	Public IsSet_Set_ShipCity_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ShipCity_forUPD</summary>
	Private _Set_ShipCity_forUPD As System.String

	''' <summary>プロパティ：Set_ShipCity_forUPD</summary>
	Public Property Set_ShipCity_forUPD() As System.String
		Get
			Return Me._Set_ShipCity_forUPD
		End Get
		Set
			Me.IsSet_Set_ShipCity_forUPD = True
			Me._Set_ShipCity_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ShipRegion_forUPD</summary>
	Public IsSet_Set_ShipRegion_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ShipRegion_forUPD</summary>
	Private _Set_ShipRegion_forUPD As System.String

	''' <summary>プロパティ：Set_ShipRegion_forUPD</summary>
	Public Property Set_ShipRegion_forUPD() As System.String
		Get
			Return Me._Set_ShipRegion_forUPD
		End Get
		Set
			Me.IsSet_Set_ShipRegion_forUPD = True
			Me._Set_ShipRegion_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ShipPostalCode_forUPD</summary>
	Public IsSet_Set_ShipPostalCode_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ShipPostalCode_forUPD</summary>
	Private _Set_ShipPostalCode_forUPD As System.String

	''' <summary>プロパティ：Set_ShipPostalCode_forUPD</summary>
	Public Property Set_ShipPostalCode_forUPD() As System.String
		Get
			Return Me._Set_ShipPostalCode_forUPD
		End Get
		Set
			Me.IsSet_Set_ShipPostalCode_forUPD = True
			Me._Set_ShipPostalCode_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ShipCountry_forUPD</summary>
	Public IsSet_Set_ShipCountry_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ShipCountry_forUPD</summary>
	Private _Set_ShipCountry_forUPD As System.String

	''' <summary>プロパティ：Set_ShipCountry_forUPD</summary>
	Public Property Set_ShipCountry_forUPD() As System.String
		Get
			Return Me._Set_ShipCountry_forUPD
		End Get
		Set
			Me.IsSet_Set_ShipCountry_forUPD = True
			Me._Set_ShipCountry_forUPD = value
		End Set
	End Property

	''' <summary>設定フラグ：OrderID_Like</summary>
	Public IsSet_OrderID_Like As Boolean = False

	''' <summary>メンバ変数：OrderID_Like</summary>
	Private _OrderID_Like As Nullable(Of System.Int32)

	''' <summary>プロパティ：OrderID_Like</summary>
	Public Property OrderID_Like() As Nullable(Of System.Int32)
		Get
			Return Me._OrderID_Like
		End Get
		Set
			Me.IsSet_OrderID_Like = True
			Me._OrderID_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：CustomerID_Like</summary>
	Public IsSet_CustomerID_Like As Boolean = False

	''' <summary>メンバ変数：CustomerID_Like</summary>
	Private _CustomerID_Like As System.String

	''' <summary>プロパティ：CustomerID_Like</summary>
	Public Property CustomerID_Like() As System.String
		Get
			Return Me._CustomerID_Like
		End Get
		Set
			Me.IsSet_CustomerID_Like = True
			Me._CustomerID_Like = value
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
	''' <summary>設定フラグ：OrderDate_Like</summary>
	Public IsSet_OrderDate_Like As Boolean = False

	''' <summary>メンバ変数：OrderDate_Like</summary>
	Private _OrderDate_Like As Nullable(Of System.DateTime)

	''' <summary>プロパティ：OrderDate_Like</summary>
	Public Property OrderDate_Like() As Nullable(Of System.DateTime)
		Get
			Return Me._OrderDate_Like
		End Get
		Set
			Me.IsSet_OrderDate_Like = True
			Me._OrderDate_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：RequiredDate_Like</summary>
	Public IsSet_RequiredDate_Like As Boolean = False

	''' <summary>メンバ変数：RequiredDate_Like</summary>
	Private _RequiredDate_Like As Nullable(Of System.DateTime)

	''' <summary>プロパティ：RequiredDate_Like</summary>
	Public Property RequiredDate_Like() As Nullable(Of System.DateTime)
		Get
			Return Me._RequiredDate_Like
		End Get
		Set
			Me.IsSet_RequiredDate_Like = True
			Me._RequiredDate_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ShippedDate_Like</summary>
	Public IsSet_ShippedDate_Like As Boolean = False

	''' <summary>メンバ変数：ShippedDate_Like</summary>
	Private _ShippedDate_Like As Nullable(Of System.DateTime)

	''' <summary>プロパティ：ShippedDate_Like</summary>
	Public Property ShippedDate_Like() As Nullable(Of System.DateTime)
		Get
			Return Me._ShippedDate_Like
		End Get
		Set
			Me.IsSet_ShippedDate_Like = True
			Me._ShippedDate_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipVia_Like</summary>
	Public IsSet_ShipVia_Like As Boolean = False

	''' <summary>メンバ変数：ShipVia_Like</summary>
	Private _ShipVia_Like As Nullable(Of System.Int32)

	''' <summary>プロパティ：ShipVia_Like</summary>
	Public Property ShipVia_Like() As Nullable(Of System.Int32)
		Get
			Return Me._ShipVia_Like
		End Get
		Set
			Me.IsSet_ShipVia_Like = True
			Me._ShipVia_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Freight_Like</summary>
	Public IsSet_Freight_Like As Boolean = False

	''' <summary>メンバ変数：Freight_Like</summary>
	Private _Freight_Like As Nullable(Of System.Decimal)

	''' <summary>プロパティ：Freight_Like</summary>
	Public Property Freight_Like() As Nullable(Of System.Decimal)
		Get
			Return Me._Freight_Like
		End Get
		Set
			Me.IsSet_Freight_Like = True
			Me._Freight_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipName_Like</summary>
	Public IsSet_ShipName_Like As Boolean = False

	''' <summary>メンバ変数：ShipName_Like</summary>
	Private _ShipName_Like As System.String

	''' <summary>プロパティ：ShipName_Like</summary>
	Public Property ShipName_Like() As System.String
		Get
			Return Me._ShipName_Like
		End Get
		Set
			Me.IsSet_ShipName_Like = True
			Me._ShipName_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipAddress_Like</summary>
	Public IsSet_ShipAddress_Like As Boolean = False

	''' <summary>メンバ変数：ShipAddress_Like</summary>
	Private _ShipAddress_Like As System.String

	''' <summary>プロパティ：ShipAddress_Like</summary>
	Public Property ShipAddress_Like() As System.String
		Get
			Return Me._ShipAddress_Like
		End Get
		Set
			Me.IsSet_ShipAddress_Like = True
			Me._ShipAddress_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipCity_Like</summary>
	Public IsSet_ShipCity_Like As Boolean = False

	''' <summary>メンバ変数：ShipCity_Like</summary>
	Private _ShipCity_Like As System.String

	''' <summary>プロパティ：ShipCity_Like</summary>
	Public Property ShipCity_Like() As System.String
		Get
			Return Me._ShipCity_Like
		End Get
		Set
			Me.IsSet_ShipCity_Like = True
			Me._ShipCity_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipRegion_Like</summary>
	Public IsSet_ShipRegion_Like As Boolean = False

	''' <summary>メンバ変数：ShipRegion_Like</summary>
	Private _ShipRegion_Like As System.String

	''' <summary>プロパティ：ShipRegion_Like</summary>
	Public Property ShipRegion_Like() As System.String
		Get
			Return Me._ShipRegion_Like
		End Get
		Set
			Me.IsSet_ShipRegion_Like = True
			Me._ShipRegion_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipPostalCode_Like</summary>
	Public IsSet_ShipPostalCode_Like As Boolean = False

	''' <summary>メンバ変数：ShipPostalCode_Like</summary>
	Private _ShipPostalCode_Like As System.String

	''' <summary>プロパティ：ShipPostalCode_Like</summary>
	Public Property ShipPostalCode_Like() As System.String
		Get
			Return Me._ShipPostalCode_Like
		End Get
		Set
			Me.IsSet_ShipPostalCode_Like = True
			Me._ShipPostalCode_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ShipCountry_Like</summary>
	Public IsSet_ShipCountry_Like As Boolean = False

	''' <summary>メンバ変数：ShipCountry_Like</summary>
	Private _ShipCountry_Like As System.String

	''' <summary>プロパティ：ShipCountry_Like</summary>
	Public Property ShipCountry_Like() As System.String
		Get
			Return Me._ShipCountry_Like
		End Get
		Set
			Me.IsSet_ShipCountry_Like = True
			Me._ShipCountry_Like = value
		End Set
	End Property

	#End Region
End Class

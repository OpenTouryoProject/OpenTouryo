'**********************************************************************************
'* クラス名        ：ProductsEntity
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
Public Class ProductsEntity
	#Region "メンバ変数"

	''' <summary>設定フラグ：ProductID</summary>
	Public IsSetPK_ProductID As Boolean = False

	''' <summary>メンバ変数：ProductID</summary>
	Private _PK_ProductID As Nullable(Of System.Int32)

	''' <summary>プロパティ：ProductID</summary>
	Public Property PK_ProductID() As Nullable(Of System.Int32)
		Get
			Return Me._PK_ProductID
		End Get
		Set
			Me.IsSetPK_ProductID = True
			Me._PK_ProductID = value
		End Set
	End Property

	''' <summary>設定フラグ：ProductName</summary>
	Public IsSet_ProductName As Boolean = False

	''' <summary>メンバ変数：ProductName</summary>
	Private _ProductName As System.String

	''' <summary>プロパティ：ProductName</summary>
	Public Property ProductName() As System.String
		Get
			Return Me._ProductName
		End Get
		Set
			Me.IsSet_ProductName = True
			Me._ProductName = value
		End Set
	End Property
	''' <summary>設定フラグ：SupplierID</summary>
	Public IsSet_SupplierID As Boolean = False

	''' <summary>メンバ変数：SupplierID</summary>
	Private _SupplierID As Nullable(Of System.Int32)

	''' <summary>プロパティ：SupplierID</summary>
	Public Property SupplierID() As Nullable(Of System.Int32)
		Get
			Return Me._SupplierID
		End Get
		Set
			Me.IsSet_SupplierID = True
			Me._SupplierID = value
		End Set
	End Property
	''' <summary>設定フラグ：CategoryID</summary>
	Public IsSet_CategoryID As Boolean = False

	''' <summary>メンバ変数：CategoryID</summary>
	Private _CategoryID As Nullable(Of System.Int32)

	''' <summary>プロパティ：CategoryID</summary>
	Public Property CategoryID() As Nullable(Of System.Int32)
		Get
			Return Me._CategoryID
		End Get
		Set
			Me.IsSet_CategoryID = True
			Me._CategoryID = value
		End Set
	End Property
	''' <summary>設定フラグ：QuantityPerUnit</summary>
	Public IsSet_QuantityPerUnit As Boolean = False

	''' <summary>メンバ変数：QuantityPerUnit</summary>
	Private _QuantityPerUnit As System.String

	''' <summary>プロパティ：QuantityPerUnit</summary>
	Public Property QuantityPerUnit() As System.String
		Get
			Return Me._QuantityPerUnit
		End Get
		Set
			Me.IsSet_QuantityPerUnit = True
			Me._QuantityPerUnit = value
		End Set
	End Property
	''' <summary>設定フラグ：UnitPrice</summary>
	Public IsSet_UnitPrice As Boolean = False

	''' <summary>メンバ変数：UnitPrice</summary>
	Private _UnitPrice As Nullable(Of System.Decimal)

	''' <summary>プロパティ：UnitPrice</summary>
	Public Property UnitPrice() As Nullable(Of System.Decimal)
		Get
			Return Me._UnitPrice
		End Get
		Set
			Me.IsSet_UnitPrice = True
			Me._UnitPrice = value
		End Set
	End Property
	''' <summary>設定フラグ：UnitsInStock</summary>
	Public IsSet_UnitsInStock As Boolean = False

	''' <summary>メンバ変数：UnitsInStock</summary>
	Private _UnitsInStock As Nullable(Of System.Int16)

	''' <summary>プロパティ：UnitsInStock</summary>
	Public Property UnitsInStock() As Nullable(Of System.Int16)
		Get
			Return Me._UnitsInStock
		End Get
		Set
			Me.IsSet_UnitsInStock = True
			Me._UnitsInStock = value
		End Set
	End Property
	''' <summary>設定フラグ：UnitsOnOrder</summary>
	Public IsSet_UnitsOnOrder As Boolean = False

	''' <summary>メンバ変数：UnitsOnOrder</summary>
	Private _UnitsOnOrder As Nullable(Of System.Int16)

	''' <summary>プロパティ：UnitsOnOrder</summary>
	Public Property UnitsOnOrder() As Nullable(Of System.Int16)
		Get
			Return Me._UnitsOnOrder
		End Get
		Set
			Me.IsSet_UnitsOnOrder = True
			Me._UnitsOnOrder = value
		End Set
	End Property
	''' <summary>設定フラグ：ReorderLevel</summary>
	Public IsSet_ReorderLevel As Boolean = False

	''' <summary>メンバ変数：ReorderLevel</summary>
	Private _ReorderLevel As Nullable(Of System.Int16)

	''' <summary>プロパティ：ReorderLevel</summary>
	Public Property ReorderLevel() As Nullable(Of System.Int16)
		Get
			Return Me._ReorderLevel
		End Get
		Set
			Me.IsSet_ReorderLevel = True
			Me._ReorderLevel = value
		End Set
	End Property
	''' <summary>設定フラグ：Discontinued</summary>
	Public IsSet_Discontinued As Boolean = False

	''' <summary>メンバ変数：Discontinued</summary>
	Private _Discontinued As Nullable(Of System.Boolean)

	''' <summary>プロパティ：Discontinued</summary>
	Public Property Discontinued() As Nullable(Of System.Boolean)
		Get
			Return Me._Discontinued
		End Get
		Set
			Me.IsSet_Discontinued = True
			Me._Discontinued = value
		End Set
	End Property

	''' <summary>設定フラグ：Set_ProductID_forUPD</summary>
	Public IsSet_Set_ProductID_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ProductID_forUPD</summary>
	Private _Set_ProductID_forUPD As Nullable(Of System.Int32)

	''' <summary>プロパティ：Set_ProductID_forUPD</summary>
	Public Property Set_ProductID_forUPD() As Nullable(Of System.Int32)
		Get
			Return Me._Set_ProductID_forUPD
		End Get
		Set
			Me.IsSet_Set_ProductID_forUPD = True
			Me._Set_ProductID_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ProductName_forUPD</summary>
	Public IsSet_Set_ProductName_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ProductName_forUPD</summary>
	Private _Set_ProductName_forUPD As System.String

	''' <summary>プロパティ：Set_ProductName_forUPD</summary>
	Public Property Set_ProductName_forUPD() As System.String
		Get
			Return Me._Set_ProductName_forUPD
		End Get
		Set
			Me.IsSet_Set_ProductName_forUPD = True
			Me._Set_ProductName_forUPD = value
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
	''' <summary>設定フラグ：Set_CategoryID_forUPD</summary>
	Public IsSet_Set_CategoryID_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_CategoryID_forUPD</summary>
	Private _Set_CategoryID_forUPD As Nullable(Of System.Int32)

	''' <summary>プロパティ：Set_CategoryID_forUPD</summary>
	Public Property Set_CategoryID_forUPD() As Nullable(Of System.Int32)
		Get
			Return Me._Set_CategoryID_forUPD
		End Get
		Set
			Me.IsSet_Set_CategoryID_forUPD = True
			Me._Set_CategoryID_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_QuantityPerUnit_forUPD</summary>
	Public IsSet_Set_QuantityPerUnit_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_QuantityPerUnit_forUPD</summary>
	Private _Set_QuantityPerUnit_forUPD As System.String

	''' <summary>プロパティ：Set_QuantityPerUnit_forUPD</summary>
	Public Property Set_QuantityPerUnit_forUPD() As System.String
		Get
			Return Me._Set_QuantityPerUnit_forUPD
		End Get
		Set
			Me.IsSet_Set_QuantityPerUnit_forUPD = True
			Me._Set_QuantityPerUnit_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_UnitPrice_forUPD</summary>
	Public IsSet_Set_UnitPrice_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_UnitPrice_forUPD</summary>
	Private _Set_UnitPrice_forUPD As Nullable(Of System.Decimal)

	''' <summary>プロパティ：Set_UnitPrice_forUPD</summary>
	Public Property Set_UnitPrice_forUPD() As Nullable(Of System.Decimal)
		Get
			Return Me._Set_UnitPrice_forUPD
		End Get
		Set
			Me.IsSet_Set_UnitPrice_forUPD = True
			Me._Set_UnitPrice_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_UnitsInStock_forUPD</summary>
	Public IsSet_Set_UnitsInStock_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_UnitsInStock_forUPD</summary>
	Private _Set_UnitsInStock_forUPD As Nullable(Of System.Int16)

	''' <summary>プロパティ：Set_UnitsInStock_forUPD</summary>
	Public Property Set_UnitsInStock_forUPD() As Nullable(Of System.Int16)
		Get
			Return Me._Set_UnitsInStock_forUPD
		End Get
		Set
			Me.IsSet_Set_UnitsInStock_forUPD = True
			Me._Set_UnitsInStock_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_UnitsOnOrder_forUPD</summary>
	Public IsSet_Set_UnitsOnOrder_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_UnitsOnOrder_forUPD</summary>
	Private _Set_UnitsOnOrder_forUPD As Nullable(Of System.Int16)

	''' <summary>プロパティ：Set_UnitsOnOrder_forUPD</summary>
	Public Property Set_UnitsOnOrder_forUPD() As Nullable(Of System.Int16)
		Get
			Return Me._Set_UnitsOnOrder_forUPD
		End Get
		Set
			Me.IsSet_Set_UnitsOnOrder_forUPD = True
			Me._Set_UnitsOnOrder_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ReorderLevel_forUPD</summary>
	Public IsSet_Set_ReorderLevel_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ReorderLevel_forUPD</summary>
	Private _Set_ReorderLevel_forUPD As Nullable(Of System.Int16)

	''' <summary>プロパティ：Set_ReorderLevel_forUPD</summary>
	Public Property Set_ReorderLevel_forUPD() As Nullable(Of System.Int16)
		Get
			Return Me._Set_ReorderLevel_forUPD
		End Get
		Set
			Me.IsSet_Set_ReorderLevel_forUPD = True
			Me._Set_ReorderLevel_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Discontinued_forUPD</summary>
	Public IsSet_Set_Discontinued_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Discontinued_forUPD</summary>
	Private _Set_Discontinued_forUPD As Nullable(Of System.Boolean)

	''' <summary>プロパティ：Set_Discontinued_forUPD</summary>
	Public Property Set_Discontinued_forUPD() As Nullable(Of System.Boolean)
		Get
			Return Me._Set_Discontinued_forUPD
		End Get
		Set
			Me.IsSet_Set_Discontinued_forUPD = True
			Me._Set_Discontinued_forUPD = value
		End Set
	End Property

	''' <summary>設定フラグ：ProductID_Like</summary>
	Public IsSet_ProductID_Like As Boolean = False

	''' <summary>メンバ変数：ProductID_Like</summary>
	Private _ProductID_Like As Nullable(Of System.Int32)

	''' <summary>プロパティ：ProductID_Like</summary>
	Public Property ProductID_Like() As Nullable(Of System.Int32)
		Get
			Return Me._ProductID_Like
		End Get
		Set
			Me.IsSet_ProductID_Like = True
			Me._ProductID_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ProductName_Like</summary>
	Public IsSet_ProductName_Like As Boolean = False

	''' <summary>メンバ変数：ProductName_Like</summary>
	Private _ProductName_Like As System.String

	''' <summary>プロパティ：ProductName_Like</summary>
	Public Property ProductName_Like() As System.String
		Get
			Return Me._ProductName_Like
		End Get
		Set
			Me.IsSet_ProductName_Like = True
			Me._ProductName_Like = value
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
	''' <summary>設定フラグ：CategoryID_Like</summary>
	Public IsSet_CategoryID_Like As Boolean = False

	''' <summary>メンバ変数：CategoryID_Like</summary>
	Private _CategoryID_Like As Nullable(Of System.Int32)

	''' <summary>プロパティ：CategoryID_Like</summary>
	Public Property CategoryID_Like() As Nullable(Of System.Int32)
		Get
			Return Me._CategoryID_Like
		End Get
		Set
			Me.IsSet_CategoryID_Like = True
			Me._CategoryID_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：QuantityPerUnit_Like</summary>
	Public IsSet_QuantityPerUnit_Like As Boolean = False

	''' <summary>メンバ変数：QuantityPerUnit_Like</summary>
	Private _QuantityPerUnit_Like As System.String

	''' <summary>プロパティ：QuantityPerUnit_Like</summary>
	Public Property QuantityPerUnit_Like() As System.String
		Get
			Return Me._QuantityPerUnit_Like
		End Get
		Set
			Me.IsSet_QuantityPerUnit_Like = True
			Me._QuantityPerUnit_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：UnitPrice_Like</summary>
	Public IsSet_UnitPrice_Like As Boolean = False

	''' <summary>メンバ変数：UnitPrice_Like</summary>
	Private _UnitPrice_Like As Nullable(Of System.Decimal)

	''' <summary>プロパティ：UnitPrice_Like</summary>
	Public Property UnitPrice_Like() As Nullable(Of System.Decimal)
		Get
			Return Me._UnitPrice_Like
		End Get
		Set
			Me.IsSet_UnitPrice_Like = True
			Me._UnitPrice_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：UnitsInStock_Like</summary>
	Public IsSet_UnitsInStock_Like As Boolean = False

	''' <summary>メンバ変数：UnitsInStock_Like</summary>
	Private _UnitsInStock_Like As Nullable(Of System.Int16)

	''' <summary>プロパティ：UnitsInStock_Like</summary>
	Public Property UnitsInStock_Like() As Nullable(Of System.Int16)
		Get
			Return Me._UnitsInStock_Like
		End Get
		Set
			Me.IsSet_UnitsInStock_Like = True
			Me._UnitsInStock_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：UnitsOnOrder_Like</summary>
	Public IsSet_UnitsOnOrder_Like As Boolean = False

	''' <summary>メンバ変数：UnitsOnOrder_Like</summary>
	Private _UnitsOnOrder_Like As Nullable(Of System.Int16)

	''' <summary>プロパティ：UnitsOnOrder_Like</summary>
	Public Property UnitsOnOrder_Like() As Nullable(Of System.Int16)
		Get
			Return Me._UnitsOnOrder_Like
		End Get
		Set
			Me.IsSet_UnitsOnOrder_Like = True
			Me._UnitsOnOrder_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ReorderLevel_Like</summary>
	Public IsSet_ReorderLevel_Like As Boolean = False

	''' <summary>メンバ変数：ReorderLevel_Like</summary>
	Private _ReorderLevel_Like As Nullable(Of System.Int16)

	''' <summary>プロパティ：ReorderLevel_Like</summary>
	Public Property ReorderLevel_Like() As Nullable(Of System.Int16)
		Get
			Return Me._ReorderLevel_Like
		End Get
		Set
			Me.IsSet_ReorderLevel_Like = True
			Me._ReorderLevel_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Discontinued_Like</summary>
	Public IsSet_Discontinued_Like As Boolean = False

	''' <summary>メンバ変数：Discontinued_Like</summary>
	Private _Discontinued_Like As Nullable(Of System.Boolean)

	''' <summary>プロパティ：Discontinued_Like</summary>
	Public Property Discontinued_Like() As Nullable(Of System.Boolean)
		Get
			Return Me._Discontinued_Like
		End Get
		Set
			Me.IsSet_Discontinued_Like = True
			Me._Discontinued_Like = value
		End Set
	End Property

	#End Region
End Class

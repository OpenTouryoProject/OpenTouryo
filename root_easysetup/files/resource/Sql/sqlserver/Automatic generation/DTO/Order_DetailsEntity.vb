'**********************************************************************************
'* クラス名        ：Order_DetailsEntity
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
Public Class Order_DetailsEntity
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
	''' <summary>設定フラグ：Quantity</summary>
	Public IsSet_Quantity As Boolean = False

	''' <summary>メンバ変数：Quantity</summary>
	Private _Quantity As Nullable(Of System.Int16)

	''' <summary>プロパティ：Quantity</summary>
	Public Property Quantity() As Nullable(Of System.Int16)
		Get
			Return Me._Quantity
		End Get
		Set
			Me.IsSet_Quantity = True
			Me._Quantity = value
		End Set
	End Property
	''' <summary>設定フラグ：Discount</summary>
	Public IsSet_Discount As Boolean = False

	''' <summary>メンバ変数：Discount</summary>
	Private _Discount As Nullable(Of System.Single)

	''' <summary>プロパティ：Discount</summary>
	Public Property Discount() As Nullable(Of System.Single)
		Get
			Return Me._Discount
		End Get
		Set
			Me.IsSet_Discount = True
			Me._Discount = value
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
	''' <summary>設定フラグ：Set_Quantity_forUPD</summary>
	Public IsSet_Set_Quantity_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Quantity_forUPD</summary>
	Private _Set_Quantity_forUPD As Nullable(Of System.Int16)

	''' <summary>プロパティ：Set_Quantity_forUPD</summary>
	Public Property Set_Quantity_forUPD() As Nullable(Of System.Int16)
		Get
			Return Me._Set_Quantity_forUPD
		End Get
		Set
			Me.IsSet_Set_Quantity_forUPD = True
			Me._Set_Quantity_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Discount_forUPD</summary>
	Public IsSet_Set_Discount_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Discount_forUPD</summary>
	Private _Set_Discount_forUPD As Nullable(Of System.Single)

	''' <summary>プロパティ：Set_Discount_forUPD</summary>
	Public Property Set_Discount_forUPD() As Nullable(Of System.Single)
		Get
			Return Me._Set_Discount_forUPD
		End Get
		Set
			Me.IsSet_Set_Discount_forUPD = True
			Me._Set_Discount_forUPD = value
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
	''' <summary>設定フラグ：Quantity_Like</summary>
	Public IsSet_Quantity_Like As Boolean = False

	''' <summary>メンバ変数：Quantity_Like</summary>
	Private _Quantity_Like As Nullable(Of System.Int16)

	''' <summary>プロパティ：Quantity_Like</summary>
	Public Property Quantity_Like() As Nullable(Of System.Int16)
		Get
			Return Me._Quantity_Like
		End Get
		Set
			Me.IsSet_Quantity_Like = True
			Me._Quantity_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Discount_Like</summary>
	Public IsSet_Discount_Like As Boolean = False

	''' <summary>メンバ変数：Discount_Like</summary>
	Private _Discount_Like As Nullable(Of System.Single)

	''' <summary>プロパティ：Discount_Like</summary>
	Public Property Discount_Like() As Nullable(Of System.Single)
		Get
			Return Me._Discount_Like
		End Get
		Set
			Me.IsSet_Discount_Like = True
			Me._Discount_Like = value
		End Set
	End Property

	#End Region
End Class

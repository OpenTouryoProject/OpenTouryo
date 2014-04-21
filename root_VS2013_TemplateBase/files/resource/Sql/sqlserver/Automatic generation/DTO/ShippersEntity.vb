'**********************************************************************************
'* クラス名        ：ShippersEntity
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
Public Class ShippersEntity
	#Region "メンバ変数"

	''' <summary>設定フラグ：ShipperID</summary>
	Public IsSetPK_ShipperID As Boolean = False

	''' <summary>メンバ変数：ShipperID</summary>
	Private _PK_ShipperID As Nullable(Of System.Int32)

	''' <summary>プロパティ：ShipperID</summary>
	Public Property PK_ShipperID() As Nullable(Of System.Int32)
		Get
			Return Me._PK_ShipperID
		End Get
		Set
			Me.IsSetPK_ShipperID = True
			Me._PK_ShipperID = value
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

	''' <summary>設定フラグ：Set_ShipperID_forUPD</summary>
	Public IsSet_Set_ShipperID_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ShipperID_forUPD</summary>
	Private _Set_ShipperID_forUPD As Nullable(Of System.Int32)

	''' <summary>プロパティ：Set_ShipperID_forUPD</summary>
	Public Property Set_ShipperID_forUPD() As Nullable(Of System.Int32)
		Get
			Return Me._Set_ShipperID_forUPD
		End Get
		Set
			Me.IsSet_Set_ShipperID_forUPD = True
			Me._Set_ShipperID_forUPD = value
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

	''' <summary>設定フラグ：ShipperID_Like</summary>
	Public IsSet_ShipperID_Like As Boolean = False

	''' <summary>メンバ変数：ShipperID_Like</summary>
	Private _ShipperID_Like As Nullable(Of System.Int32)

	''' <summary>プロパティ：ShipperID_Like</summary>
	Public Property ShipperID_Like() As Nullable(Of System.Int32)
		Get
			Return Me._ShipperID_Like
		End Get
		Set
			Me.IsSet_ShipperID_Like = True
			Me._ShipperID_Like = value
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

	#End Region
End Class

'**********************************************************************************
'* クラス名        ：SHIPPERSEntity
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
Public Class SHIPPERSEntity
	#Region "メンバ変数"

	''' <summary>設定フラグ：SHIPPERID</summary>
	Public IsSetPK_SHIPPERID As Boolean = False

	''' <summary>メンバ変数：SHIPPERID</summary>
	Private _PK_SHIPPERID As Nullable(Of System.Decimal)

	''' <summary>プロパティ：SHIPPERID</summary>
	Public Property PK_SHIPPERID() As Nullable(Of System.Decimal)
		Get
			Return Me._PK_SHIPPERID
		End Get
		Set
			Me.IsSetPK_SHIPPERID = True
			Me._PK_SHIPPERID = value
		End Set
	End Property

	''' <summary>設定フラグ：COMPANYNAME</summary>
	Public IsSet_COMPANYNAME As Boolean = False

	''' <summary>メンバ変数：COMPANYNAME</summary>
	Private _COMPANYNAME As System.String

	''' <summary>プロパティ：COMPANYNAME</summary>
	Public Property COMPANYNAME() As System.String
		Get
			Return Me._COMPANYNAME
		End Get
		Set
			Me.IsSet_COMPANYNAME = True
			Me._COMPANYNAME = value
		End Set
	End Property
	''' <summary>設定フラグ：PHONE</summary>
	Public IsSet_PHONE As Boolean = False

	''' <summary>メンバ変数：PHONE</summary>
	Private _PHONE As System.String

	''' <summary>プロパティ：PHONE</summary>
	Public Property PHONE() As System.String
		Get
			Return Me._PHONE
		End Get
		Set
			Me.IsSet_PHONE = True
			Me._PHONE = value
		End Set
	End Property

	''' <summary>設定フラグ：Set_SHIPPERID_forUPD</summary>
	Public IsSet_Set_SHIPPERID_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_SHIPPERID_forUPD</summary>
	Private _Set_SHIPPERID_forUPD As Nullable(Of System.Decimal)

	''' <summary>プロパティ：Set_SHIPPERID_forUPD</summary>
	Public Property Set_SHIPPERID_forUPD() As Nullable(Of System.Decimal)
		Get
			Return Me._Set_SHIPPERID_forUPD
		End Get
		Set
			Me.IsSet_Set_SHIPPERID_forUPD = True
			Me._Set_SHIPPERID_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_COMPANYNAME_forUPD</summary>
	Public IsSet_Set_COMPANYNAME_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_COMPANYNAME_forUPD</summary>
	Private _Set_COMPANYNAME_forUPD As System.String

	''' <summary>プロパティ：Set_COMPANYNAME_forUPD</summary>
	Public Property Set_COMPANYNAME_forUPD() As System.String
		Get
			Return Me._Set_COMPANYNAME_forUPD
		End Get
		Set
			Me.IsSet_Set_COMPANYNAME_forUPD = True
			Me._Set_COMPANYNAME_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_PHONE_forUPD</summary>
	Public IsSet_Set_PHONE_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_PHONE_forUPD</summary>
	Private _Set_PHONE_forUPD As System.String

	''' <summary>プロパティ：Set_PHONE_forUPD</summary>
	Public Property Set_PHONE_forUPD() As System.String
		Get
			Return Me._Set_PHONE_forUPD
		End Get
		Set
			Me.IsSet_Set_PHONE_forUPD = True
			Me._Set_PHONE_forUPD = value
		End Set
	End Property

	''' <summary>設定フラグ：SHIPPERID_Like</summary>
	Public IsSet_SHIPPERID_Like As Boolean = False

	''' <summary>メンバ変数：SHIPPERID_Like</summary>
	Private _SHIPPERID_Like As Nullable(Of System.Decimal)

	''' <summary>プロパティ：SHIPPERID_Like</summary>
	Public Property SHIPPERID_Like() As Nullable(Of System.Decimal)
		Get
			Return Me._SHIPPERID_Like
		End Get
		Set
			Me.IsSet_SHIPPERID_Like = True
			Me._SHIPPERID_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：COMPANYNAME_Like</summary>
	Public IsSet_COMPANYNAME_Like As Boolean = False

	''' <summary>メンバ変数：COMPANYNAME_Like</summary>
	Private _COMPANYNAME_Like As System.String

	''' <summary>プロパティ：COMPANYNAME_Like</summary>
	Public Property COMPANYNAME_Like() As System.String
		Get
			Return Me._COMPANYNAME_Like
		End Get
		Set
			Me.IsSet_COMPANYNAME_Like = True
			Me._COMPANYNAME_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：PHONE_Like</summary>
	Public IsSet_PHONE_Like As Boolean = False

	''' <summary>メンバ変数：PHONE_Like</summary>
	Private _PHONE_Like As System.String

	''' <summary>プロパティ：PHONE_Like</summary>
	Public Property PHONE_Like() As System.String
		Get
			Return Me._PHONE_Like
		End Get
		Set
			Me.IsSet_PHONE_Like = True
			Me._PHONE_Like = value
		End Set
	End Property

	#End Region
End Class

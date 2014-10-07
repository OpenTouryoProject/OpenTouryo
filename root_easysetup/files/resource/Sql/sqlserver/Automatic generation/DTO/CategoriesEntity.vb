'**********************************************************************************
'* クラス名        ：CategoriesEntity
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
Public Class CategoriesEntity
	#Region "メンバ変数"

	''' <summary>設定フラグ：CategoryID</summary>
	Public IsSetPK_CategoryID As Boolean = False

	''' <summary>メンバ変数：CategoryID</summary>
	Private _PK_CategoryID As Nullable(Of System.Int32)

	''' <summary>プロパティ：CategoryID</summary>
	Public Property PK_CategoryID() As Nullable(Of System.Int32)
		Get
			Return Me._PK_CategoryID
		End Get
		Set
			Me.IsSetPK_CategoryID = True
			Me._PK_CategoryID = value
		End Set
	End Property

	''' <summary>設定フラグ：CategoryName</summary>
	Public IsSet_CategoryName As Boolean = False

	''' <summary>メンバ変数：CategoryName</summary>
	Private _CategoryName As System.String

	''' <summary>プロパティ：CategoryName</summary>
	Public Property CategoryName() As System.String
		Get
			Return Me._CategoryName
		End Get
		Set
			Me.IsSet_CategoryName = True
			Me._CategoryName = value
		End Set
	End Property
	''' <summary>設定フラグ：Description</summary>
	Public IsSet_Description As Boolean = False

	''' <summary>メンバ変数：Description</summary>
	Private _Description As System.String

	''' <summary>プロパティ：Description</summary>
	Public Property Description() As System.String
		Get
			Return Me._Description
		End Get
		Set
			Me.IsSet_Description = True
			Me._Description = value
		End Set
	End Property
	''' <summary>設定フラグ：Picture</summary>
	Public IsSet_Picture As Boolean = False

	''' <summary>メンバ変数：Picture</summary>
	Private _Picture As System.Byte()

	''' <summary>プロパティ：Picture</summary>
	Public Property Picture() As System.Byte()
		Get
			Return Me._Picture
		End Get
		Set
			Me.IsSet_Picture = True
			Me._Picture = value
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
	''' <summary>設定フラグ：Set_CategoryName_forUPD</summary>
	Public IsSet_Set_CategoryName_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_CategoryName_forUPD</summary>
	Private _Set_CategoryName_forUPD As System.String

	''' <summary>プロパティ：Set_CategoryName_forUPD</summary>
	Public Property Set_CategoryName_forUPD() As System.String
		Get
			Return Me._Set_CategoryName_forUPD
		End Get
		Set
			Me.IsSet_Set_CategoryName_forUPD = True
			Me._Set_CategoryName_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Description_forUPD</summary>
	Public IsSet_Set_Description_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Description_forUPD</summary>
	Private _Set_Description_forUPD As System.String

	''' <summary>プロパティ：Set_Description_forUPD</summary>
	Public Property Set_Description_forUPD() As System.String
		Get
			Return Me._Set_Description_forUPD
		End Get
		Set
			Me.IsSet_Set_Description_forUPD = True
			Me._Set_Description_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_Picture_forUPD</summary>
	Public IsSet_Set_Picture_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_Picture_forUPD</summary>
	Private _Set_Picture_forUPD As System.Byte()

	''' <summary>プロパティ：Set_Picture_forUPD</summary>
	Public Property Set_Picture_forUPD() As System.Byte()
		Get
			Return Me._Set_Picture_forUPD
		End Get
		Set
			Me.IsSet_Set_Picture_forUPD = True
			Me._Set_Picture_forUPD = value
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
	''' <summary>設定フラグ：CategoryName_Like</summary>
	Public IsSet_CategoryName_Like As Boolean = False

	''' <summary>メンバ変数：CategoryName_Like</summary>
	Private _CategoryName_Like As System.String

	''' <summary>プロパティ：CategoryName_Like</summary>
	Public Property CategoryName_Like() As System.String
		Get
			Return Me._CategoryName_Like
		End Get
		Set
			Me.IsSet_CategoryName_Like = True
			Me._CategoryName_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Description_Like</summary>
	Public IsSet_Description_Like As Boolean = False

	''' <summary>メンバ変数：Description_Like</summary>
	Private _Description_Like As System.String

	''' <summary>プロパティ：Description_Like</summary>
	Public Property Description_Like() As System.String
		Get
			Return Me._Description_Like
		End Get
		Set
			Me.IsSet_Description_Like = True
			Me._Description_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：Picture_Like</summary>
	Public IsSet_Picture_Like As Boolean = False

	''' <summary>メンバ変数：Picture_Like</summary>
	Private _Picture_Like As System.Byte()

	''' <summary>プロパティ：Picture_Like</summary>
	Public Property Picture_Like() As System.Byte()
		Get
			Return Me._Picture_Like
		End Get
		Set
			Me.IsSet_Picture_Like = True
			Me._Picture_Like = value
		End Set
	End Property

	#End Region
End Class

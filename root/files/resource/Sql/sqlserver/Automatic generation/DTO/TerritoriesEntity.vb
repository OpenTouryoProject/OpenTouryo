'**********************************************************************************
'* クラス名        ：TerritoriesEntity
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
Public Class TerritoriesEntity
	#Region "メンバ変数"

	''' <summary>設定フラグ：TerritoryID</summary>
	Public IsSetPK_TerritoryID As Boolean = False

	''' <summary>メンバ変数：TerritoryID</summary>
	Private _PK_TerritoryID As System.String

	''' <summary>プロパティ：TerritoryID</summary>
	Public Property PK_TerritoryID() As System.String
		Get
			Return Me._PK_TerritoryID
		End Get
		Set
			Me.IsSetPK_TerritoryID = True
			Me._PK_TerritoryID = value
		End Set
	End Property

	''' <summary>設定フラグ：TerritoryDescription</summary>
	Public IsSet_TerritoryDescription As Boolean = False

	''' <summary>メンバ変数：TerritoryDescription</summary>
	Private _TerritoryDescription As System.String

	''' <summary>プロパティ：TerritoryDescription</summary>
	Public Property TerritoryDescription() As System.String
		Get
			Return Me._TerritoryDescription
		End Get
		Set
			Me.IsSet_TerritoryDescription = True
			Me._TerritoryDescription = value
		End Set
	End Property
	''' <summary>設定フラグ：RegionID</summary>
	Public IsSet_RegionID As Boolean = False

	''' <summary>メンバ変数：RegionID</summary>
	Private _RegionID As Nullable(Of System.Int32)

	''' <summary>プロパティ：RegionID</summary>
	Public Property RegionID() As Nullable(Of System.Int32)
		Get
			Return Me._RegionID
		End Get
		Set
			Me.IsSet_RegionID = True
			Me._RegionID = value
		End Set
	End Property

	''' <summary>設定フラグ：Set_TerritoryID_forUPD</summary>
	Public IsSet_Set_TerritoryID_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_TerritoryID_forUPD</summary>
	Private _Set_TerritoryID_forUPD As System.String

	''' <summary>プロパティ：Set_TerritoryID_forUPD</summary>
	Public Property Set_TerritoryID_forUPD() As System.String
		Get
			Return Me._Set_TerritoryID_forUPD
		End Get
		Set
			Me.IsSet_Set_TerritoryID_forUPD = True
			Me._Set_TerritoryID_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_TerritoryDescription_forUPD</summary>
	Public IsSet_Set_TerritoryDescription_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_TerritoryDescription_forUPD</summary>
	Private _Set_TerritoryDescription_forUPD As System.String

	''' <summary>プロパティ：Set_TerritoryDescription_forUPD</summary>
	Public Property Set_TerritoryDescription_forUPD() As System.String
		Get
			Return Me._Set_TerritoryDescription_forUPD
		End Get
		Set
			Me.IsSet_Set_TerritoryDescription_forUPD = True
			Me._Set_TerritoryDescription_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_RegionID_forUPD</summary>
	Public IsSet_Set_RegionID_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_RegionID_forUPD</summary>
	Private _Set_RegionID_forUPD As Nullable(Of System.Int32)

	''' <summary>プロパティ：Set_RegionID_forUPD</summary>
	Public Property Set_RegionID_forUPD() As Nullable(Of System.Int32)
		Get
			Return Me._Set_RegionID_forUPD
		End Get
		Set
			Me.IsSet_Set_RegionID_forUPD = True
			Me._Set_RegionID_forUPD = value
		End Set
	End Property

	''' <summary>設定フラグ：TerritoryID_Like</summary>
	Public IsSet_TerritoryID_Like As Boolean = False

	''' <summary>メンバ変数：TerritoryID_Like</summary>
	Private _TerritoryID_Like As System.String

	''' <summary>プロパティ：TerritoryID_Like</summary>
	Public Property TerritoryID_Like() As System.String
		Get
			Return Me._TerritoryID_Like
		End Get
		Set
			Me.IsSet_TerritoryID_Like = True
			Me._TerritoryID_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：TerritoryDescription_Like</summary>
	Public IsSet_TerritoryDescription_Like As Boolean = False

	''' <summary>メンバ変数：TerritoryDescription_Like</summary>
	Private _TerritoryDescription_Like As System.String

	''' <summary>プロパティ：TerritoryDescription_Like</summary>
	Public Property TerritoryDescription_Like() As System.String
		Get
			Return Me._TerritoryDescription_Like
		End Get
		Set
			Me.IsSet_TerritoryDescription_Like = True
			Me._TerritoryDescription_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：RegionID_Like</summary>
	Public IsSet_RegionID_Like As Boolean = False

	''' <summary>メンバ変数：RegionID_Like</summary>
	Private _RegionID_Like As Nullable(Of System.Int32)

	''' <summary>プロパティ：RegionID_Like</summary>
	Public Property RegionID_Like() As Nullable(Of System.Int32)
		Get
			Return Me._RegionID_Like
		End Get
		Set
			Me.IsSet_RegionID_Like = True
			Me._RegionID_Like = value
		End Set
	End Property

	#End Region
End Class

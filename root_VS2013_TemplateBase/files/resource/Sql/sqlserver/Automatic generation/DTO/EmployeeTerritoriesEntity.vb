'**********************************************************************************
'* クラス名        ：EmployeeTerritoriesEntity
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
Public Class EmployeeTerritoriesEntity
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

	#End Region
End Class

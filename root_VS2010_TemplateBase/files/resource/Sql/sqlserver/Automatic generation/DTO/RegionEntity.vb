'**********************************************************************************
'* クラス名        ：RegionEntity
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
Public Class RegionEntity
	#Region "メンバ変数"

	''' <summary>設定フラグ：RegionID</summary>
	Public IsSetPK_RegionID As Boolean = False

	''' <summary>メンバ変数：RegionID</summary>
	Private _PK_RegionID As Nullable(Of System.Int32)

	''' <summary>プロパティ：RegionID</summary>
	Public Property PK_RegionID() As Nullable(Of System.Int32)
		Get
			Return Me._PK_RegionID
		End Get
		Set
			Me.IsSetPK_RegionID = True
			Me._PK_RegionID = value
		End Set
	End Property

	''' <summary>設定フラグ：RegionDescription</summary>
	Public IsSet_RegionDescription As Boolean = False

	''' <summary>メンバ変数：RegionDescription</summary>
	Private _RegionDescription As System.String

	''' <summary>プロパティ：RegionDescription</summary>
	Public Property RegionDescription() As System.String
		Get
			Return Me._RegionDescription
		End Get
		Set
			Me.IsSet_RegionDescription = True
			Me._RegionDescription = value
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
	''' <summary>設定フラグ：Set_RegionDescription_forUPD</summary>
	Public IsSet_Set_RegionDescription_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_RegionDescription_forUPD</summary>
	Private _Set_RegionDescription_forUPD As System.String

	''' <summary>プロパティ：Set_RegionDescription_forUPD</summary>
	Public Property Set_RegionDescription_forUPD() As System.String
		Get
			Return Me._Set_RegionDescription_forUPD
		End Get
		Set
			Me.IsSet_Set_RegionDescription_forUPD = True
			Me._Set_RegionDescription_forUPD = value
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
	''' <summary>設定フラグ：RegionDescription_Like</summary>
	Public IsSet_RegionDescription_Like As Boolean = False

	''' <summary>メンバ変数：RegionDescription_Like</summary>
	Private _RegionDescription_Like As System.String

	''' <summary>プロパティ：RegionDescription_Like</summary>
	Public Property RegionDescription_Like() As System.String
		Get
			Return Me._RegionDescription_Like
		End Get
		Set
			Me.IsSet_RegionDescription_Like = True
			Me._RegionDescription_Like = value
		End Set
	End Property

	#End Region
End Class

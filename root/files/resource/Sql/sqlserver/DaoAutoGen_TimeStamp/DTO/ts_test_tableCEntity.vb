'**********************************************************************************
'* クラス名        ：ts_test_tableCEntity
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
Public Class ts_test_tableCEntity
	#Region "メンバ変数"

	''' <summary>設定フラグ：id</summary>
	Public IsSetPK_id As Boolean = False

	''' <summary>メンバ変数：id</summary>
	Private _PK_id As Nullable(Of System.Int32)

	''' <summary>プロパティ：id</summary>
	Public Property PK_id() As Nullable(Of System.Int32)
		Get
			Return Me._PK_id
		End Get
		Set
			Me.IsSetPK_id = True
			Me._PK_id = value
		End Set
	End Property

	''' <summary>設定フラグ：ts</summary>
	Public IsSet_ts As Boolean = False

	''' <summary>メンバ変数：ts</summary>
	Private _ts As System.Byte()

	''' <summary>プロパティ：ts</summary>
	Public Property ts() As System.Byte()
		Get
			Return Me._ts
		End Get
		Set
			Me.IsSet_ts = True
			Me._ts = value
		End Set
	End Property
	''' <summary>設定フラグ：val</summary>
	Public IsSet_val As Boolean = False

	''' <summary>メンバ変数：val</summary>
	Private _val As System.String

	''' <summary>プロパティ：val</summary>
	Public Property val() As System.String
		Get
			Return Me._val
		End Get
		Set
			Me.IsSet_val = True
			Me._val = value
		End Set
	End Property

	''' <summary>設定フラグ：Set_id_forUPD</summary>
	Public IsSet_Set_id_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_id_forUPD</summary>
	Private _Set_id_forUPD As Nullable(Of System.Int32)

	''' <summary>プロパティ：Set_id_forUPD</summary>
	Public Property Set_id_forUPD() As Nullable(Of System.Int32)
		Get
			Return Me._Set_id_forUPD
		End Get
		Set
			Me.IsSet_Set_id_forUPD = True
			Me._Set_id_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_ts_forUPD</summary>
	Public IsSet_Set_ts_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_ts_forUPD</summary>
	Private _Set_ts_forUPD As System.Byte()

	''' <summary>プロパティ：Set_ts_forUPD</summary>
	Public Property Set_ts_forUPD() As System.Byte()
		Get
			Return Me._Set_ts_forUPD
		End Get
		Set
			Me.IsSet_Set_ts_forUPD = True
			Me._Set_ts_forUPD = value
		End Set
	End Property
	''' <summary>設定フラグ：Set_val_forUPD</summary>
	Public IsSet_Set_val_forUPD As Boolean = False

	''' <summary>メンバ変数：Set_val_forUPD</summary>
	Private _Set_val_forUPD As System.String

	''' <summary>プロパティ：Set_val_forUPD</summary>
	Public Property Set_val_forUPD() As System.String
		Get
			Return Me._Set_val_forUPD
		End Get
		Set
			Me.IsSet_Set_val_forUPD = True
			Me._Set_val_forUPD = value
		End Set
	End Property

	''' <summary>設定フラグ：id_Like</summary>
	Public IsSet_id_Like As Boolean = False

	''' <summary>メンバ変数：id_Like</summary>
	Private _id_Like As Nullable(Of System.Int32)

	''' <summary>プロパティ：id_Like</summary>
	Public Property id_Like() As Nullable(Of System.Int32)
		Get
			Return Me._id_Like
		End Get
		Set
			Me.IsSet_id_Like = True
			Me._id_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：ts_Like</summary>
	Public IsSet_ts_Like As Boolean = False

	''' <summary>メンバ変数：ts_Like</summary>
	Private _ts_Like As System.Byte()

	''' <summary>プロパティ：ts_Like</summary>
	Public Property ts_Like() As System.Byte()
		Get
			Return Me._ts_Like
		End Get
		Set
			Me.IsSet_ts_Like = True
			Me._ts_Like = value
		End Set
	End Property
	''' <summary>設定フラグ：val_Like</summary>
	Public IsSet_val_Like As Boolean = False

	''' <summary>メンバ変数：val_Like</summary>
	Private _val_Like As System.String

	''' <summary>プロパティ：val_Like</summary>
	Public Property val_Like() As System.String
		Get
			Return Me._val_Like
		End Get
		Set
			Me.IsSet_val_Like = True
			Me._val_Like = value
		End Set
	End Property

	#End Region
End Class

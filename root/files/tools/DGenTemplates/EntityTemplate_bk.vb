'**********************************************************************************
'* クラス名        ：_EntityClassName_
'* クラス日本語名  ：自動生成Entityクラス
'*
'* 作成日時        ：_TimeStamp_
'* 作成者          ：棟梁 D層自動生成ツール（墨壺）, _UserName_
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
Public Class _EntityClassName_
	#Region "メンバ変数"

	' ControlComment:LoopStart-PKColumn
	''' <summary>設定フラグ：_ColumnName_</summary>
	Public IsSetPK__ColumnName_ As Boolean = False

	''' <summary>メンバ変数：_ColumnName_</summary>
	Private _PK__ColumnName_ As _EntityTypeInfo_

	''' <summary>プロパティ：_ColumnName_</summary>
	Public Property PK__ColumnName_() As _EntityTypeInfo_
		Get
			Return Me._PK__ColumnName_
		End Get
		Set
			Me.IsSetPK__ColumnName_ = True
			Me._PK__ColumnName_ = value
		End Set
	End Property
	' ControlComment:LoopEnd-PKColumn

	' ControlComment:LoopStart-ElseColumn
	''' <summary>設定フラグ：_ColumnName_</summary>
	Public IsSet__ColumnName_ As Boolean = False

	''' <summary>メンバ変数：_ColumnName_</summary>
	Private __ColumnName_ As _EntityTypeInfo_

	''' <summary>プロパティ：_ColumnName_</summary>
	Public Property _ColumnName_() As _EntityTypeInfo_
		Get
			Return Me.__ColumnName_
		End Get
		Set
			Me.IsSet__ColumnName_ = True
			Me.__ColumnName_ = value
		End Set
	End Property
	' ControlComment:LoopEnd-ElseColumn

	' ControlComment:LoopStart-PPUpdSet
	''' <summary>設定フラグ：_ColumnName_</summary>
	Public IsSet__ColumnName_ As Boolean = False

	''' <summary>メンバ変数：_ColumnName_</summary>
	Private __ColumnName_ As _EntityTypeInfo_

	''' <summary>プロパティ：_ColumnName_</summary>
	Public Property _ColumnName_() As _EntityTypeInfo_
		Get
			Return Me.__ColumnName_
		End Get
		Set
			Me.IsSet__ColumnName_ = True
			Me.__ColumnName_ = value
		End Set
	End Property
	' ControlComment:LoopEnd-PPUpdSet

	' ControlComment:LoopStart-PPLike
	''' <summary>設定フラグ：_ColumnName_</summary>
	Public IsSet__ColumnName_ As Boolean = False

	''' <summary>メンバ変数：_ColumnName_</summary>
	Private __ColumnName_ As _EntityTypeInfo_

	''' <summary>プロパティ：_ColumnName_</summary>
	Public Property _ColumnName_() As _EntityTypeInfo_
		Get
			Return Me.__ColumnName_
		End Get
		Set
			Me.IsSet__ColumnName_ = True
			Me.__ColumnName_ = value
		End Set
	End Property
	' ControlComment:LoopEnd-PPLike

	#End Region
End Class

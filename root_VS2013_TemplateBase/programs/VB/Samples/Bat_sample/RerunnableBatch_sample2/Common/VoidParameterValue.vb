'**********************************************************************************
'* フレームワーク・テストクラス
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：VoidParameterValue
'* クラス日本語名  ：テスト用の引数クラス
'*
'* 作成日時        ：－
'* 作成者          ：生技セ
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*
'**********************************************************************************

' ベースクラス
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Business.Common

Namespace Common
	''' <summary>
	''' VoidParameterValue の概要の説明です
	''' </summary>
	Public Class VoidParameterValue
		Inherits MyParameterValue
		#Region "コンストラクタ"

		''' <summary>コンストラクタ</summary>
		Public Sub New(screenId As String, controlId As String, methodName As String, actionType As String, user As MyUserInfo)
				' Baseのコンストラクタに引数を渡すために必要。
			MyBase.New(screenId, controlId, methodName, actionType, user)
		End Sub

		#End Region
	End Class
End Namespace

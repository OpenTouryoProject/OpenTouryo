'**********************************************************************************
'* フレームワーク・テストクラス
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：ExecuteBatchProcessParameterValue
'* クラス日本語名  ：テスト用の引数クラス
'*
'* 作成日時        ：－
'* 作成者          ：生技セ
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System.Collections

Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Business.Common

Namespace Common
	''' <summary>
	''' ExecuteBatchProcessParameterValue の概要の説明です
	''' </summary>
	Public Class ExecuteBatchProcessParameterValue
		Inherits MyParameterValue
		''' <summary>1トランザクションで処理を行う主キー一覧</summary>
		Public SubPkList As ArrayList

		#Region "コンストラクタ"

		''' <summary>コンストラクタ</summary>
		Public Sub New(screenId As String, controlId As String, methodName As String, actionType As String, user As MyUserInfo)
				' Baseのコンストラクタに引数を渡すために必要。
			MyBase.New(screenId, controlId, methodName, actionType, user)
		End Sub

		#End Region
	End Class
End Namespace

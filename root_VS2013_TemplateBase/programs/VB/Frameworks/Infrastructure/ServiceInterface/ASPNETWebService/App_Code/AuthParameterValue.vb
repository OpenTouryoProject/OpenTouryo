'**********************************************************************************
'* フレームワーク・テストクラス
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：AuthParameterValue
'* クラス日本語名  ：認証処理用の引数クラス
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  2011/xx/xx  西野 大介         新規作成
'**********************************************************************************

' System
Imports System

' ベースクラス
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Business.Common

Namespace Touryo.Infrastructure.Framework.ServiceInterface.ASPNETWebService
	''' <summary>
	''' AuthParameterValue の概要の説明です
	''' </summary>
	''' <remarks>シリアライズ可能にする（WS対応）</remarks>
	<Serializable> _
	Public Class AuthParameterValue
		Inherits MyParameterValue
		''' <summary>パスワード</summary>
		Public Password As Object


		#Region "コンストラクタ"

		''' <summary>コンストラクタ</summary>
		Public Sub New(screenId As String, controlId As String, methodName As String, actionType As String, user As MyUserInfo, password As String)
			MyBase.New(screenId, controlId, methodName, actionType, user)
			' Baseのコンストラクタに引数を渡すために必要。
			' パスワードを設定する。
			Me.Password = password
		End Sub

		#End Region
	End Class
End Namespace

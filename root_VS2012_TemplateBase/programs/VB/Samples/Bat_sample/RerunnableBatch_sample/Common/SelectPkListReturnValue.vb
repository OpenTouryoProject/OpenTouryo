'**********************************************************************************
'* フレームワーク・テストクラス
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：SelectPkListReturnValue
'* クラス日本語名  ：テスト用の戻り値クラス
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

' System
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Collections
Imports System.Collections.Generic

' ベースクラス
Imports Touryo.Infrastructure.Business.Common

Namespace Common
	''' <summary>
	''' SelectPkListReturnValueの概要の説明です
	''' </summary>
	Public Class SelectPkListReturnValue
		Inherits MyReturnValue
		''' <summary>PkList</summary>
		Public PkList As ArrayList
	End Class
End Namespace

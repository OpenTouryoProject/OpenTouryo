'**********************************************************************************
'* フレームワーク・テストクラス
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

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
'**********************************************************************************

Imports System.Collections
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

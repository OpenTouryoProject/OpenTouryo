'**********************************************************************************
'* フレームワーク・テストクラス
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：TestReturnValue
'* クラス日本語名  ：テスト用の戻り値クラス
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports Touryo.Infrastructure.Business.Common

Namespace Common
    ''' <summary>
    ''' TestReturnValueの概要の説明です
    ''' </summary>
    Public Class TestReturnValue
        Inherits MyReturnValue
        ''' <summary>汎用エリア</summary>
        Public Obj As Object

        ''' <summary>ShipperID</summary>
        Public ShipperID As Integer

        ''' <summary>CompanyName</summary>
        Public CompanyName As String

        ''' <summary>Phone</summary>
        Public Phone As String
    End Class
End Namespace

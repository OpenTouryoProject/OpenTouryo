'**********************************************************************************
'* フレームワーク・テストクラス
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：TestParameterValue
'* クラス日本語名  ：テスト用の引数クラス
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
Imports Touryo.Infrastructure.Business.Util

Namespace Common
    ''' <summary>
    ''' TestParameterValue の概要の説明です
    ''' </summary>
    Public Class TestParameterValue
        Inherits MyParameterValue
        ''' <summary>汎用エリア</summary>
        Public Obj As Object

        ''' <summary>ShipperID</summary>
        Public ShipperID As Integer

        ''' <summary>CompanyName</summary>
        Public CompanyName As String

        ''' <summary>Phone</summary>
        Public Phone As String

        ''' <summary>OrderColumn</summary>
        Public OrderColumn As String

        ''' <summary>OrderSequence</summary>
        Public OrderSequence As String

#Region "コンストラクタ"

        ''' <summary>コンストラクタ</summary>
        Public Sub New(ByVal screenId As String, ByVal controlId As String, ByVal methodName As String, ByVal actionType As String, ByVal user As MyUserInfo)
            ' Baseのコンストラクタに引数を渡すために必要。
            MyBase.New(screenId, controlId, methodName, actionType, user)
        End Sub

#End Region
    End Class
End Namespace

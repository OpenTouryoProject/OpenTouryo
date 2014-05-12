'**********************************************************************************
'* フレームワーク・テストクラス
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：TestReturnValue
'* クラス日本語名  ：テスト用の戻り値クラス
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*
'**********************************************************************************

' System
Imports System

' ベースクラス
Imports Touryo.Infrastructure.Business.Common

Namespace WSIFType_sample
    ''' <summary>
    ''' TestReturnValue の概要の説明です
    ''' </summary>
    ''' <remarks>シリアライズ可能にする（WS対応）</remarks>
    <Serializable()> _
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

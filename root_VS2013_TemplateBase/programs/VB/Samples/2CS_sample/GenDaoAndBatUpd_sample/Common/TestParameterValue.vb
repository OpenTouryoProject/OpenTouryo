'**********************************************************************************
'* フレームワーク・テストクラス
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：TestParameterValue
'* クラス日本語名  ：テスト用の引数クラス
'*
'* 作成日時        ：－
'* 作成者          ：sas 生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System
Imports System.Data

' ベースクラス
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Business.Common

Namespace Common
    ''' <summary>
    ''' TestParameterValue の概要の説明です
    ''' </summary>
    Public Class TestParameterValue
        Inherits MyParameterValue
#Region "コンストラクタ"

        Public Sub New(ByVal screenId As String, ByVal controlId As String, ByVal methodName As String, ByVal actionType As String, ByVal user As MyUserInfo)
            ' Baseのコンストラクタに引数を渡すために必要。
            MyBase.New(screenId, controlId, methodName, actionType, user)
        End Sub

#End Region

#Region "フィールド"

        ' 値（インサート、主キー値など）
        Public field1 As Object
        Public field2 As Object
        Public field3 As Object
        Public field4 As Object
        Public field5 As Object
        Public field6 As Object
        Public field7 As Object
        Public field8 As Object
        Public field9 As Object
        Public field10 As Object
        Public field11 As Object
        Public field12 As Object
        Public field13 As Object
        Public field14 As Object
        Public field15 As Object
        Public field16 As Object
        Public field17 As Object
        Public field18 As Object
        Public field19 As Object
        Public field20 As Object

        ' 更新時
        Public field1_ForUpd As Object
        Public field2_ForUpd As Object
        Public field3_ForUpd As Object
        Public field4_ForUpd As Object
        Public field5_ForUpd As Object
        Public field6_ForUpd As Object
        Public field7_ForUpd As Object
        Public field8_ForUpd As Object
        Public field9_ForUpd As Object
        Public field10_ForUpd As Object
        Public field11_ForUpd As Object
        Public field12_ForUpd As Object
        Public field13_ForUpd As Object
        Public field14_ForUpd As Object
        Public field15_ForUpd As Object
        Public field16_ForUpd As Object
        Public field17_ForUpd As Object
        Public field18_ForUpd As Object
        Public field19_ForUpd As Object
        Public field20_ForUpd As Object

        ' 検索条件
        Public field1_ForSearch As Object
        Public field2_ForSearch As Object
        Public field3_ForSearch As Object
        Public field4_ForSearch As Object
        Public field5_ForSearch As Object
        Public field6_ForSearch As Object
        Public field7_ForSearch As Object
        Public field8_ForSearch As Object
        Public field9_ForSearch As Object
        Public field10_ForSearch As Object
        Public field11_ForSearch As Object
        Public field12_ForSearch As Object
        Public field13_ForSearch As Object
        Public field14_ForSearch As Object
        Public field15_ForSearch As Object
        Public field16_ForSearch As Object
        Public field17_ForSearch As Object
        Public field18_ForSearch As Object
        Public field19_ForSearch As Object
        Public field20_ForSearch As Object

#End Region

        Public dt As DataTable
        Public obj As Object
    End Class
End Namespace

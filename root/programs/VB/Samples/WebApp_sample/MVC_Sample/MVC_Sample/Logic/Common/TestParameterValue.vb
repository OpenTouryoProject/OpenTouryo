Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

' ベースクラス
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Business.Common

Namespace Logic.Common
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
        Public Sub New(screenId As String, controlId As String, methodName As String, actionType As String, user As MyUserInfo)
            ' Baseのコンストラクタに引数を渡すために必要。
            MyBase.New(screenId, controlId, methodName, actionType, user)
        End Sub

#End Region
    End Class
End Namespace

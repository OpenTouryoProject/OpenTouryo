Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

' ベースクラス
Imports Touryo.Infrastructure.Business.Common

Namespace Logic.Common
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

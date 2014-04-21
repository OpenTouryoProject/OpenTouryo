Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Public Class Bean
    Public Property AAA() As Decimal
        Get
            Return m_AAA
        End Get
        Set(ByVal value As Decimal)
            m_AAA = Value
        End Set
    End Property
    Private m_AAA As Decimal
    Public Property BBB() As DateTime
        Get
            Return m_BBB
        End Get
        Set(ByVal value As DateTime)
            m_BBB = Value
        End Set
    End Property
    Private m_BBB As DateTime
    Public Property CCC() As String
        Get
            Return m_CCC
        End Get
        Set(ByVal value As String)
            m_CCC = Value
        End Set
    End Property
    Private m_CCC As String

    Public Sub New(ByVal aaa As Decimal, ByVal bbb As DateTime, ByVal ccc As String)
        Me.AAA = aaa
        Me.BBB = bbb
        Me.CCC = ccc
    End Sub
End Class

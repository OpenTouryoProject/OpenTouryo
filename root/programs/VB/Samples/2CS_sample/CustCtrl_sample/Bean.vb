'**********************************************************************************
'* カスタム コントロール・サンプル アプリ
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：Bean
'* クラス日本語名  ：Bean
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Public Class Bean
    Public Property AAA() As Decimal
        Get
            Return m_AAA
        End Get
        Set(ByVal value As Decimal)
            m_AAA = value
        End Set
    End Property
    Private m_AAA As Decimal
    Public Property BBB() As DateTime
        Get
            Return m_BBB
        End Get
        Set(ByVal value As DateTime)
            m_BBB = value
        End Set
    End Property
    Private m_BBB As DateTime
    Public Property CCC() As String
        Get
            Return m_CCC
        End Get
        Set(ByVal value As String)
            m_CCC = value
        End Set
    End Property
    Private m_CCC As String

    Public Sub New(ByVal aaa As Decimal, ByVal bbb As DateTime, ByVal ccc As String)
        Me.AAA = aaa
        Me.BBB = bbb
        Me.CCC = ccc
    End Sub
End Class

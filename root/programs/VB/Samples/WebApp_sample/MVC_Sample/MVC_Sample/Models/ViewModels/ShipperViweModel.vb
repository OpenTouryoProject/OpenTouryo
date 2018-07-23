'**********************************************************************************
'* サンプル アプリ・モデル
'**********************************************************************************

' テスト用クラスなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：ShipperViweModel
'* クラス日本語名  ：サンプル アプリ・モデル
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'*
'**********************************************************************************

Namespace Models.ViewModels
    ''' <summary>
    ''' サンプル アプリ・モデル
    ''' </summary>
    Public Class ShipperViweModel
        ''' <summary>ShipperID</summary>
        Public Property ShipperID() As Int64
            Get
                Return m_ShipperID
            End Get
            Set
                m_ShipperID = Value
            End Set
        End Property
        Private m_ShipperID As Int64
        ' Oracle対応
        ''' <summary>CompanyName</summary>
        Public Property CompanyName() As String
            Get
                Return m_CompanyName
            End Get
            Set
                m_CompanyName = Value
            End Set
        End Property
        Private m_CompanyName As String

        ''' <summary>Phone</summary>
        Public Property Phone() As String
            Get
                Return m_Phone
            End Get
            Set
                m_Phone = Value
            End Set
        End Property
        Private m_Phone As String
    End Class
End Namespace

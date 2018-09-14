'**********************************************************************************
'* サンプル アプリ・モデル
'**********************************************************************************

' テスト用クラスなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：ShipperViweModel
'* クラス日本語名  ：サンプル アプリ・モデル
'*
'* 作成日時        ：2018/8/1
'* 作成者          ：棟梁 D層自動生成ツール（墨壺）, 日立 太郎
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
    <Serializable>
    Public Class ShipperViweModel

#Region "メンバ変数"

        ''' <summary>メンバ変数：ShipperID</summary>
        Private _PK_ShipperID As Nullable(Of System.Int64) ' Oracle対応 32 -> 64

        ''' <summary>プロパティ：ShipperID</summary>
        Public Property ShipperID() As Nullable(Of System.Int64) ' Oracle対応 32 -> 64
            Get
                Return Me._PK_ShipperID
            End Get
            Set
                Me._PK_ShipperID = Value
            End Set
        End Property

        ''' <summary>メンバ変数：CompanyName</summary>
        Private _CompanyName As System.String

        ''' <summary>プロパティ：CompanyName</summary>
        Public Property CompanyName() As System.String
            Get
                Return Me._CompanyName
            End Get
            Set
                Me._CompanyName = Value
            End Set
        End Property
        ''' <summary>メンバ変数：Phone</summary>
        Private _Phone As System.String

        ''' <summary>プロパティ：Phone</summary>
        Public Property Phone() As System.String
            Get
                Return Me._Phone
            End Get
            Set
                Me._Phone = Value
            End Set
        End Property

#End Region

    End Class
End Namespace

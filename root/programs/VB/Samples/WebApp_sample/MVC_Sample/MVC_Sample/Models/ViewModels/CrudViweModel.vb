'**********************************************************************************
'* サンプル アプリ・モデル
'**********************************************************************************

' テスト用クラスなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：CrudViweModel
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
    <Serializable>
    Public Class CrudViweModel
        Inherits BaseViewModel

        ''' <summary>shipper</summary>
        Public Property Shipper() As ShipperViweModel
            Get
                Return m_Shipper
            End Get
            Set
                m_Shipper = Value
            End Set
        End Property
        Private m_Shipper As ShipperViweModel

        ''' <summary>shippers</summary>
        Public Property Shippers() As List(Of ShipperViweModel)
            Get
                Return m_Shippers
            End Get
            Set
                m_Shippers = Value
            End Set
        End Property
        Private m_Shippers As List(Of ShipperViweModel)

        ''' <summary>メッセージ</summary>
        Public Property Message() As String
            Get
                Return m_Message
            End Get
            Set
                m_Message = Value
            End Set
        End Property
        Private m_Message As String

#Region "ドロップダウンリストに表示するアイテム"

        ''' <summary>データアクセス制御クラス（データプロバイダ）</summary>
        Public Property DdlDap() As String
            Get
                Return m_DdlDap
            End Get
            Set
                m_DdlDap = Value
            End Set
        End Property
        Private m_DdlDap As String

        ''' <summary>個別、共通、自動生成のＤａｏ種別</summary>
        Public Property DdlMode1() As String
            Get
                Return m_DdlMode1
            End Get
            Set
                m_DdlMode1 = Value
            End Set
        End Property
        Private m_DdlMode1 As String

        ''' <summary>静的、動的のクエリ モード</summary>
        Public Property DdlMode2() As String
            Get
                Return m_DdlMode2
            End Get
            Set
                m_DdlMode2 = Value
            End Set
        End Property
        Private m_DdlMode2 As String

        ''' <summary>分離レベル</summary>
        Public Property DdlIso() As String
            Get
                Return m_DdlIso
            End Get
            Set
                m_DdlIso = Value
            End Set
        End Property
        Private m_DdlIso As String

        ''' <summary>コミット、ロールバック</summary>
        Public Property DdlExRollback() As String
            Get
                Return m_DdlExRollback
            End Get
            Set
                m_DdlExRollback = Value
            End Set
        End Property
        Private m_DdlExRollback As String

        ''' <summary>コミット、ロールバック</summary>
        Public Property DdlOrderColumn() As String
            Get
                Return m_DdlOrderColumn
            End Get
            Set
                m_DdlOrderColumn = Value
            End Set
        End Property
        Private m_DdlOrderColumn As String

        ''' <summary>コミット、ロールバック</summary>
        Public Property DdlOrderSequence() As String
            Get
                Return m_DdlOrderSequence
            End Get
            Set
                m_DdlOrderSequence = Value
            End Set
        End Property
        Private m_DdlOrderSequence As String

        ''' <summary>データアクセス制御クラス（データプロバイダ） アイテムリスト</summary>
        Public ReadOnly Property DdlDapItems() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From {
                    New SelectListItem() With {
                        .Text = "SQL Server / SQL Client",
                        .Value = "SQL",
                        .Selected = True
                    },
                    New SelectListItem() With {
                        .Text = "Multi-DB / OLEDB.NET",
                        .Value = "OLE"
                    },
                    New SelectListItem() With {
                        .Text = "Multi-DB / ODBC.NET",
                        .Value = "ODB"
                    },
                    New SelectListItem() With {
                        .Text = "Oracle / ODP.NET",
                        .Value = "ODP"
                    },
                    New SelectListItem() With {
                        .Text = "DB2 / DB2.NET",
                        .Value = "DB2"
                    },
                    New SelectListItem() With {
                        .Text = "HiRDB / HiRDB-DP",
                        .Value = "HIR"
                    },
                    New SelectListItem() With {
                        .Text = "MySQL Cnn/NET",
                        .Value = "MCN"
                    },
                    New SelectListItem() With {
                        .Text = "PostgreSQL / Npgsql",
                        .Value = "NPS"
                    }
                }
            End Get
        End Property

        ''' <summary>個別、共通、自動生成のＤａｏ種別 アイテムリスト</summary>
        Public ReadOnly Property DdlMode1Items() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From {
                    New SelectListItem() With {
                        .Text = "個別Ｄａｏ",
                        .Value = "individual",
                        .Selected = True
                    },
                    New SelectListItem() With {
                        .Text = "共通Ｄａｏ",
                        .Value = "common"
                    },
                    New SelectListItem() With {
                        .Text = "自動生成Ｄａｏ（更新のみ）",
                        .Value = "generate"
                    }
                }
            End Get
        End Property

        ''' <summary>静的、動的のクエリ モード アイテムリスト</summary>
        Public ReadOnly Property DdlMode2Items() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From {
                    New SelectListItem() With {
                        .Text = "静的クエリ",
                        .Value = "static",
                        .Selected = True
                    },
                    New SelectListItem() With {
                        .Text = "動的クエリ",
                        .Value = "dynamic"
                    }
                }
            End Get
        End Property

        ''' <summary>分離レベル アイテムリスト</summary>
        Public ReadOnly Property DdlIsoItems() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From {
                    New SelectListItem() With {
                        .Text = "ノットコネクト",
                        .Value = "NC"
                    },
                    New SelectListItem() With {
                        .Text = "ノートランザクション",
                        .Value = "NT",
                        .Selected = True
                    },
                    New SelectListItem() With {
                        .Text = "ダーティリード",
                        .Value = "RU"
                    },
                    New SelectListItem() With {
                        .Text = "リードコミット",
                        .Value = "RC"
                    },
                    New SelectListItem() With {
                        .Text = "リピータブルリード",
                        .Value = "RR"
                    },
                    New SelectListItem() With {
                        .Text = "シリアライザブル",
                        .Value = "SZ"
                    },
                    New SelectListItem() With {
                        .Text = "スナップショット",
                        .Value = "SS"
                    },
                    New SelectListItem() With {
                        .Text = "デフォルト",
                        .Value = "DF"
                    }
                }
            End Get
        End Property

        ''' <summary>コミット、ロールバック アイテムリスト</summary>
        Public ReadOnly Property DdlExRollbackItems() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From {
                    New SelectListItem() With {
                        .Text = "正常時",
                        .Value = "-",
                        .Selected = True
                    },
                    New SelectListItem() With {
                        .Text = "業務例外",
                        .Value = "Business"
                    },
                    New SelectListItem() With {
                        .Text = "システム例外",
                        .Value = "System"
                    },
                    New SelectListItem() With {
                        .Text = "その他、一般的な例外",
                        .Value = "Other"
                    },
                    New SelectListItem() With {
                        .Text = "業務例外への振替",
                        .Value = "Other-Business"
                    },
                    New SelectListItem() With {
                        .Text = "システム例外への振替",
                        .Value = "Other-System"
                    }
                }
            End Get
        End Property

        ''' <summary>並び替え対象列 アイテムリスト</summary>
        Public ReadOnly Property DdlOrderColumnItems() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From {
                    New SelectListItem() With {
                        .Text = "c1",
                        .Value = "c1",
                        .Selected = True
                    },
                    New SelectListItem() With {
                        .Text = "c2",
                        .Value = "c2"
                    },
                    New SelectListItem() With {
                        .Text = "c3",
                        .Value = "c3"
                    }
                }
            End Get
        End Property

        ''' <summary>昇順・降順 アイテムリスト</summary>
        Public ReadOnly Property DdlOrderSequenceItems() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From {
                    New SelectListItem() With {
                        .Text = "ASC",
                        .Value = "A",
                        .Selected = True
                    },
                    New SelectListItem() With {
                        .Text = "DESC",
                        .Value = "D"
                    }
                }
            End Get
        End Property

#End Region
    End Class
End Namespace

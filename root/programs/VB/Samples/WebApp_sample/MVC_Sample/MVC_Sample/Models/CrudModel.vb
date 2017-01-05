'**********************************************************************************
'* サンプル アプリ・モデル
'**********************************************************************************

'**********************************************************************************
'* クラス名        ：CrudModel
'* クラス日本語名  ：サンプル アプリ・モデル
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

'System
Imports System
Imports System.Web
Imports System.Web.Mvc

Imports System.Collections.Generic

' DataSet をインポート
Imports MVC_Sample.DataSets

Namespace Models
    ''' <summary>
    ''' サンプル アプリ・モデル
    ''' </summary>
    Public Class CrudModel
        ''' <summary>shippersテーブル</summary>
        Public Property shippers() As DsNorthwind.ShippersDataTable

        ''' <summary>メッセージ</summary>
        Public Property Message() As String

#Region "ドロップダウンリストに表示するアイテム"

        ''' <summary>
        ''' ddlDap に表示するアイテムリスト
        ''' </summary>
        Public ReadOnly Property DdlDapItems() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From { _
                    New SelectListItem() With {.Text = "SQL Server / SQL Client", .Value = "SQL", .Selected = True}, _
                    New SelectListItem() With {.Text = "Multi-DB / OLEDB.NET", .Value = "OLE"}, _
                    New SelectListItem() With {.Text = "Multi-DB / ODBC.NET", .Value = "ODB"}, _
                    New SelectListItem() With {.Text = "Oracle / ODP.NET", .Value = "SQL"}, _
                    New SelectListItem() With {.Text = "DB2 / DB2.NET", .Value = "DB2"}, _
                    New SelectListItem() With {.Text = "HiRDB / HiRDB-DP", .Value = "HIR"}, _
                    New SelectListItem() With {.Text = "MySQL Cnn/NET", .Value = "MCN"}, _
                    New SelectListItem() With {.Text = "PostgreSQL / Npgsql", .Value = "NPS"} _
                }
            End Get
        End Property

        ''' <summary>
        ''' ddlMode1 に表示するアイテムリスト
        ''' </summary>
        Public ReadOnly Property DdlMode1Items() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From { _
                    New SelectListItem() With {.Text = "個別Ｄａｏ", .Value = "individual", .Selected = True}, _
                    New SelectListItem() With {.Text = "共通Ｄａｏ", .Value = "common"}, _
                    New SelectListItem() With {.Text = "自動生成Ｄａｏ（更新のみ）", .Value = "generate"} _
                }
            End Get
        End Property

        ''' <summary>
        ''' ddlMode2 に表示するアイテムリスト
        ''' </summary>
        Public ReadOnly Property DdlMode2Items() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From { _
                    New SelectListItem() With {.Text = "静的クエリ", .Value = "static", .Selected = True}, _
                    New SelectListItem() With {.Text = "動的クエリ", .Value = "dynamic"} _
                }
            End Get
        End Property

        ''' <summary>
        ''' ddlIso に表示するアイテムリスト
        ''' </summary>
        Public ReadOnly Property DdlIsoItems() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From { _
                    New SelectListItem() With {.Text = "ノットコネクト", .Value = "NC"}, _
                    New SelectListItem() With {.Text = "ノートランザクション", .Value = "NT", .Selected = True}, _
                    New SelectListItem() With {.Text = "ダーティリード", .Value = "RU"}, _
                    New SelectListItem() With {.Text = "リードコミット", .Value = "RC"}, _
                    New SelectListItem() With {.Text = "リピータブルリード", .Value = "RR"}, _
                    New SelectListItem() With {.Text = "シリアライザブル", .Value = "SZ"}, _
                    New SelectListItem() With {.Text = "スナップショット", .Value = "SS"}, _
                    New SelectListItem() With {.Text = "デフォルト", .Value = "DF"} _
                }
            End Get
        End Property

        ''' <summary>
        ''' ddlExRollback に表示するアイテムリスト
        ''' </summary>
        Public ReadOnly Property DdlExRollbackItems() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From { _
                    New SelectListItem() With {.Text = "正常時", .Value = "-", .Selected = True}, _
                    New SelectListItem() With {.Text = "業務例外", .Value = "Business"}, _
                    New SelectListItem() With {.Text = "システム例外", .Value = "System"}, _
                    New SelectListItem() With {.Text = "その他、一般的な例外", .Value = "Other"}, _
                    New SelectListItem() With {.Text = "業務例外への振替", .Value = "Other-Business"}, _
                    New SelectListItem() With {.Text = "システム例外への振替", .Value = "Other-System"} _
                }
            End Get
        End Property

        ''' <summary>
        ''' ddlTransmission に表示するアイテムリスト
        ''' </summary>
        Public ReadOnly Property DdlTransmissionItems() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From { _
                    New SelectListItem() With {.Text = "Webサービス呼出", .Value = "testWebService", .Selected = True}, _
                    New SelectListItem() With {.Text = "インプロセス呼出", .Value = "testInProcess"} _
                }
            End Get
        End Property

        ''' <summary>
        ''' ddlOrderColumn に表示するアイテムリスト
        ''' </summary>
        Public ReadOnly Property DdlOrderColumnItems() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From { _
                    New SelectListItem() With {.Text = "c1", .Value = "c1", .Selected = True}, _
                    New SelectListItem() With {.Text = "c2", .Value = "c2"}, _
                    New SelectListItem() With {.Text = "c3", .Value = "c3"} _
                }
            End Get
        End Property

        ''' <summary>
        ''' ddlOrderSequence に表示するアイテムリスト
        ''' </summary>
        Public ReadOnly Property DdlOrderSequenceItems() As List(Of SelectListItem)
            Get
                Return New List(Of SelectListItem)() From { _
                    New SelectListItem() With {.Text = "ASC", .Value = "A", .Selected = True}, _
                    New SelectListItem() With {.Text = "DESC", .Value = "D"} _
                }
            End Get
        End Property

#End Region

#Region "HTML.BeginFormで値を復元用途"

        ''' <summary>HTML.BeginFormで値を復元するためのワーク領域</summary>
        Public Property InputValues() As Dictionary(Of String, String)

        ''' <summary>HTML.BeginFormDe値を復元するためのワーク領域の初期化</summary>
        ''' <param name="form">入力フォームの情報</param>
        Public Sub CopyInputValues(form As FormCollection)
            InputValues = New Dictionary(Of String, String)()

            For Each key As String In form.AllKeys
                InputValues.Add(key, form(key))
            Next
        End Sub

#End Region
    End Class
End Namespace

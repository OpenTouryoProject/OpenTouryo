'**********************************************************************************
'* サンプル アプリ画面
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：Window1
'* クラス日本語名  ：サンプル アプリ画面
'*
'* 作成日時        ：－
'* 作成者          ：生技
'* 更新履歴        ：
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports _2CSClientWPF_sample.Common
Imports _2CSClientWPF_sample.Business

Imports System
Imports System.Data
Imports System.Windows
Imports System.Windows.Controls

Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Util
Imports Touryo.Infrastructure.Public.Db


''' <summary>Window1.xaml の相互作用ロジック（サンプル アプリ画面）</summary>
Partial Public Class Window1
    ''' <summary>ユーザ情報</summary>
    Private myUserInfo As MyUserInfo

#Region "初期処理"

    ''' <summary>コンストラクタ</summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>ロード イベント</summary>
    Private Sub Window_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' ddlDap
        Me.ddlDap.Items.Add(New ComboBoxItem("SQL Server / SQL Client", "SQL"))
        Me.ddlDap.Items.Add(New ComboBoxItem("Multi-DB / OLEDB.NET", "OLE"))
        Me.ddlDap.Items.Add(New ComboBoxItem("Multi-DB / ODCB.NET", "ODB"))
        Me.ddlDap.Items.Add(New ComboBoxItem("Oracle / ODP.NET", "ODP"))
        Me.ddlDap.Items.Add(New ComboBoxItem("DB2 / DB2.NET", "DB2"))
        Me.ddlDap.Items.Add(New ComboBoxItem("HiRDB / HiRDB-DP", "HIR"))
        Me.ddlDap.Items.Add(New ComboBoxItem("MySQL Cnn/NET", "MCN"))
        Me.ddlDap.Items.Add(New ComboBoxItem("PostgreSQL / Npgsql", "NPS"))

        Me.ddlDap.SelectedIndex = 0

        ' ddlMode1
        Me.ddlMode1.Items.Add(New ComboBoxItem("個別Ｄａｏ", "individual"))
        Me.ddlMode1.Items.Add(New ComboBoxItem("共通Ｄａｏ", "common"))
        Me.ddlMode1.Items.Add(New ComboBoxItem("自動生成Ｄａｏ（更新のみ）", "generate"))
        Me.ddlMode1.SelectedIndex = 0

        ' ddlMode2
        Me.ddlMode2.Items.Add(New ComboBoxItem("静的クエリ", "static"))
        Me.ddlMode2.Items.Add(New ComboBoxItem("動的クエリ", "dynamic"))
        Me.ddlMode2.SelectedIndex = 0

        ' ddlIso
        Me.ddlIso.Items.Add(New ComboBoxItem("ノットコネクト", "NC"))
        Me.ddlIso.Items.Add(New ComboBoxItem("ノートランザクション", "NT"))
        Me.ddlIso.Items.Add(New ComboBoxItem("ダーティリード", "RU"))
        Me.ddlIso.Items.Add(New ComboBoxItem("リードコミット", "RC"))
        Me.ddlIso.Items.Add(New ComboBoxItem("リピータブルリード", "RR"))
        Me.ddlIso.Items.Add(New ComboBoxItem("シリアライザブル", "SZ"))
        Me.ddlIso.Items.Add(New ComboBoxItem("スナップショット", "SS"))
        Me.ddlIso.Items.Add(New ComboBoxItem("デフォルト", "DF"))
        Me.ddlIso.SelectedIndex = 1

        ' ddlExRollback
        Me.ddlExRollback.Items.Add(New ComboBoxItem("正常時", "-"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("業務例外", "Business"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("システム例外", "System"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("その他、一般的な例外", "Other"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("業務例外への振替", "Other-Business"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("システム例外への振替", "Other-System"))
        Me.ddlExRollback.SelectedIndex = 0

        ' ddlOrderColumn
        Me.ddlOrderColumn.Items.Add(New ComboBoxItem("c1", "c1"))
        Me.ddlOrderColumn.Items.Add(New ComboBoxItem("c2", "c2"))
        Me.ddlOrderColumn.Items.Add(New ComboBoxItem("c3", "c3"))
        Me.ddlOrderColumn.SelectedIndex = 0

        ' ddlOrderSequence
        Me.ddlOrderSequence.Items.Add(New ComboBoxItem("ASC", "A"))
        Me.ddlOrderSequence.Items.Add(New ComboBoxItem("DESC", "D"))
        Me.ddlOrderSequence.SelectedIndex = 0

        ' ユーザ情報
        Me.myUserInfo = New MyUserInfo("userName", Environment.MachineName)
    End Sub

#Region "コンボボックス用"

    ''' <summary>コンボボックス用インナークラス</summary>
    Private Class ComboBoxItem
        ''' <summary>表示名</summary>
        Private m_name As String = ""

        ''' <summary>値</summary>
        Private m_value As String = ""

        ''' <summary>コンストラクタ</summary>
        Public Sub New(ByVal name As String, ByVal value As String)
            m_name = name
            m_value = value
        End Sub

        ''' <summary>表示名</summary>
        Public ReadOnly Property Name() As String
            Get
                Return m_name
            End Get
        End Property

        ''' <summary>値</summary>
        Public ReadOnly Property Value() As String
            Get
                Return m_value
            End Get
        End Property

        ''' <summary>
        ''' オーバーライドしたメソッド
        ''' これがコンボボックスに表示される
        ''' </summary>
        Public Overrides Function ToString() As String
            Return m_name
        End Function
    End Class

#End Region

#End Region

#Region "ＣＲＵＤ処理メソッド"

#Region "参照系"

    ''' <summary>件数取得</summary>
    Private Sub button1_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "SelectCount", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' 分離レベルの設定
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' Ｂ層呼出し＋都度コミット
        Dim layerB__1 As New LayerB()
        testReturnValue = layerB__1.DoBusinessLogic(testParameterValue, iso)
        LayerB.CommitAndClose()

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Content = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            labelMessage.Content = testReturnValue.Obj.ToString() & "件のデータがあります"
        End If
    End Sub

    ''' <summary>一覧取得（dt）</summary>
    Private Sub button2_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "SelectAll_DT", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' 分離レベルの設定
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' Ｂ層呼出し＋都度コミット
        Dim layerB__1 As New LayerB()
        testReturnValue = layerB__1.DoBusinessLogic(testParameterValue, iso)
        LayerB.CommitAndClose()

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Content = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            Me.dataGridControl1.Columns.Clear()
            Me.dataGridControl1.DataContext = testReturnValue.Obj
        End If
    End Sub

    ''' <summary>一覧取得（ds）</summary>
    Private Sub button3_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "SelectAll_DS", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' 分離レベルの設定
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' Ｂ層呼出し＋都度コミット
        Dim layerB__1 As New LayerB()
        testReturnValue = layerB__1.DoBusinessLogic(testParameterValue, iso)
        LayerB.CommitAndClose()

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Content = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            Me.dataGridControl1.Columns.Clear()
            Me.dataGridControl1.DataContext = DirectCast(testReturnValue.Obj, DataSet).Tables(0)
        End If
    End Sub

    ''' <summary>一覧取得（dr）</summary>
    Private Sub button4_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "SelectAll_DR", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' 分離レベルの設定
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' Ｂ層呼出し＋都度コミット
        Dim layerB__1 As New LayerB()
        testReturnValue = layerB__1.DoBusinessLogic(testParameterValue, iso)
        LayerB.CommitAndClose()

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Content = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            Me.dataGridControl1.Columns.Clear()
            Me.dataGridControl1.DataContext = testReturnValue.Obj
        End If
    End Sub

    ''' <summary>一覧取得（動的sql）</summary>
    Private Sub button5_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "SelectAll_DSQL", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 動的SQLの要素を設定
        testParameterValue.OrderColumn = DirectCast(Me.ddlOrderColumn.SelectedItem, ComboBoxItem).Value
        testParameterValue.OrderSequence = DirectCast(Me.ddlOrderSequence.SelectedItem, ComboBoxItem).Value

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' 分離レベルの設定
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' Ｂ層呼出し＋都度コミット
        Dim layerB__1 As New LayerB()
        testReturnValue = layerB__1.DoBusinessLogic(testParameterValue, iso)
        LayerB.CommitAndClose()

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Content = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            Me.dataGridControl1.Columns.Clear()
            Me.dataGridControl1.DataContext = testReturnValue.Obj
        End If
    End Sub

    ''' <summary>参照処理</summary>
    Private Sub button6_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Select", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 情報の設定
        testParameterValue.ShipperID = Integer.Parse(Me.textBox1.Text)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' 分離レベルの設定
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' Ｂ層呼出し＋都度コミット
        Dim layerB__1 As New LayerB()
        testReturnValue = layerB__1.DoBusinessLogic(testParameterValue, iso)
        LayerB.CommitAndClose()

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Content = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            Me.textBox1.Text = testReturnValue.ShipperID.ToString()
            Me.textBox2.Text = testReturnValue.CompanyName
            Me.textBox3.Text = testReturnValue.Phone
        End If
    End Sub

#End Region

#Region "更新系"

    ''' <summary>追加処理</summary>
    Private Sub button7_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Insert", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 情報の設定
        testParameterValue.CompanyName = Me.textBox2.Text
        testParameterValue.Phone = Me.textBox3.Text

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' 分離レベルの設定
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' Ｂ層呼出し＋都度コミット
        Dim layerB__1 As New LayerB()
        testReturnValue = layerB__1.DoBusinessLogic(testParameterValue, iso)
        LayerB.CommitAndClose()

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Content = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            labelMessage.Content = testReturnValue.Obj.ToString() & "件追加"
        End If
    End Sub

    ''' <summary>更新処理</summary>
    Private Sub button8_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Update", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 情報の設定
        testParameterValue.ShipperID = Integer.Parse(Me.textBox1.Text)
        testParameterValue.CompanyName = Me.textBox2.Text
        testParameterValue.Phone = Me.textBox3.Text

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' 分離レベルの設定
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' Ｂ層呼出し＋都度コミット
        Dim layerB__1 As New LayerB()
        testReturnValue = layerB__1.DoBusinessLogic(testParameterValue, iso)
        LayerB.CommitAndClose()

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Content = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            labelMessage.Content = testReturnValue.Obj.ToString() & "件更新"
        End If
    End Sub

    ''' <summary>削除処理</summary>
    Private Sub button9_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(Me.Name, DirectCast(sender, Button).Name, "Delete", DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 情報の設定
        testParameterValue.ShipperID = Integer.Parse(textBox1.Text)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' 分離レベルの設定
        Dim iso As DbEnum.IsolationLevelEnum = Me.SelectIsolationLevel()

        ' Ｂ層呼出し＋都度コミット
        Dim layerB__1 As New LayerB()
        testReturnValue = layerB__1.DoBusinessLogic(testParameterValue, iso)
        LayerB.CommitAndClose()

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Content = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            labelMessage.Content = testReturnValue.Obj.ToString() & "件削除"
        End If
    End Sub

#End Region

#End Region

#Region "その他"

    ''' <summary>クリア</summary>
    Private Sub button10_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.dataGridControl1.Columns.Clear()
        Me.dataGridControl1.DataContext = Nothing
    End Sub

    ''' <summary>メッセージ取得（埋め込まれたリソース対応）</summary>
    Private Sub button11_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.textBox5.Text = GetMessage.GetMessageDescription(Me.textBox4.Text)
    End Sub

    ''' <summary>共有情報取得（埋め込まれたリソース対応）</summary>
    Private Sub button12_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.textBox7.Text = GetSharedProperty.GetSharedPropertyValue(Me.textBox6.Text)
    End Sub

#End Region

#Region "分離レベルの設定メソッド"

    ''' <summary>分離レベルの設定</summary>
    Private Function SelectIsolationLevel() As DbEnum.IsolationLevelEnum
        If DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "NC" Then
            Return DbEnum.IsolationLevelEnum.NotConnect
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "NT" Then
            Return DbEnum.IsolationLevelEnum.NoTransaction
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "RU" Then
            Return DbEnum.IsolationLevelEnum.ReadUncommitted
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "RC" Then
            Return DbEnum.IsolationLevelEnum.ReadCommitted
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "RR" Then
            Return DbEnum.IsolationLevelEnum.RepeatableRead
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "SZ" Then
            Return DbEnum.IsolationLevelEnum.Serializable
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "SS" Then
            Return DbEnum.IsolationLevelEnum.Snapshot
        ElseIf DirectCast(Me.ddlIso.SelectedItem, ComboBoxItem).Value = "DF" Then
            Return DbEnum.IsolationLevelEnum.DefaultTransaction
        Else
            Throw New Exception("分離レベルの設定がおかしい")
        End If
    End Function

#End Region
End Class
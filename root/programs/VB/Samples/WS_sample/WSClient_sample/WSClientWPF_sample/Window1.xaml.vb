'**********************************************************************************
'* ３層型 サンプル アプリ
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

Imports WSIFType_sample

Imports System.Data
Imports System.Threading

Imports Touryo.Infrastructure.Business.RichClient.Asynchronous
Imports Touryo.Infrastructure.Business.RichClient.Util
Imports Touryo.Infrastructure.Business.Util

Imports Touryo.Infrastructure.Framework.RichClient.Asynchronous
Imports Touryo.Infrastructure.Framework.Transmission
Imports Touryo.Infrastructure.Framework.Util

''' <summary>Window1.xaml の相互作用ロジック（サンプル アプリ画面）</summary>
Partial Public Class Window1
    ''' <summary>ユーザ情報</summary>
    Private myUserInfo As MyUserInfo

    ''' <summary>呼出し制御部品</summary>
    Private CallCtrl As CallController

    ''' <summary>非同期実行クラス</summary>
    Private Af1 As MyBaseAsyncFunc = Nothing

    ''' <summary>非同期実行クラス</summary>
    Private Af2 As AsyncFunc = Nothing

    ''' <summary>画面上のデータは退避する</summary>
    Private LogicalName As String

#Region "初期処理"

    ''' <summary>ロード イベント</summary>
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
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

        ' WSでは使用しない（設定できないので）。
        Me.ddlIso.IsEnabled = False

        ' ddlExRollback
        Me.ddlExRollback.Items.Add(New ComboBoxItem("正常時", "-"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("業務例外", "Business"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("システム例外", "System"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("その他、一般的な例外", "Other"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("業務例外への振替", "Other-Business"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("システム例外への振替", "Other-System"))
        Me.ddlExRollback.SelectedIndex = 0

        ' ddlTransmission
        Me.ddlTransmission.Items.Add(New ComboBoxItem("Webサービス呼出", "testWebService"))
        Me.ddlTransmission.Items.Add(New ComboBoxItem("インプロセス呼出", "testInProcess"))
        Me.ddlTransmission.SelectedIndex = 0

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

        ' 呼出し制御部品
        Me.CallCtrl = New CallController("")

        ' スレッドプール
        ThreadPool.SetMinThreads(10, 10)
        ' 待機状態スレッド数
        ThreadPool.SetMaxThreads(10, 10)
        ' 最大スレッド起動数
    End Sub

#Region "コンボボックス用"

    ''' <summary>コンボボックス用インナークラス</summary>
    Private Class ComboBoxItem
        ''' <summary>表示名</summary>
        Private m_name As String = ""

        ''' <summary>値</summary>
        Private m_value As String = ""

        ''' <summary>コンストラクタ</summary>
        Public Sub New(name As String, value As String)
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
    ''' <remarks>
    ''' 非同期フレームワークを使用してB層の呼び出し処理を非同期化
    ''' （非同期実行、結果表示の双方に匿名デリゲードを使用するパターン）
    ''' </remarks>
    Private Sub button1_Click(sender As Object, e As RoutedEventArgs)
        ' 非同期処理クラスを生成
        ' 匿名デリゲードの場合は、ベース２で良い。
        Me.Af1 = New MyBaseAsyncFunc(Me)

        ' 引数を纏め非同期処理クラスに設定
        Me.Af1.Parameter = DirectCast(New TestParameterValue( _
                Me.Name, DirectCast(sender, Button).Name, "SelectCount", _
                DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & _
                DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & _
                DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & _
                DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo), Object)

        ' 画面上のデータは退避する
        '（オブジェクトであれば、クローンする。）
        Me.LogicalName = DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value

        ' 引数を纏める
        Me.Af1.Parameter = New TestParameterValue( _
            Me.Name, DirectCast(sender, Button).Name, "SelectCount", _
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 非同期実行するメソッドを指定（匿名デリゲード）
        ' ここは副スレッドから実行されるので注意
        ' （画面上のメンバに触らないこと！）。
        Me.Af1.AsyncFunc = New BaseAsyncFunc.AsyncFuncDelegate(AddressOf MyAsyncFunc1)

        ' 結果表示のメソッドを指定（匿名デリゲード）
        ' このメソッドは必ず主スレッドで実行される。
        ' （画面上のメンバを更新できる！）。
        Me.Af1.SetResult = New BaseAsyncFunc.SetResultDelegate(AddressOf MySetResult1)

        ' 非同期実行する。
        If Not Me.Af1.StartByThreadPool() Then
            MessageBox.Show("別の非同期処理が実行中です。")
        End If
    End Sub

    Function MyAsyncFunc1(ByVal param As Object)

        ' 引数クラス（キャスト）
        Dim testParameterValue As TestParameterValue = DirectCast(param, TestParameterValue)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' 呼出し制御部品（スレッドセーフでないため副スレッド内で作る）
        Dim callCtrl As New CallController("")

        ' Invoke
        testReturnValue = DirectCast(callCtrl.Invoke(Me.LogicalName, testParameterValue), TestReturnValue)

        '' 進捗表示のテスト
        'Me.Af1.ChangeProgress = New BaseAsyncFunc.ChangeProgressDelegate(AddressOf MyChangeProgress1)
        'Me.Af1.ExecChangeProgress("進捗表示")

        '' 非同期メッセージボックス表示のテスト
        'Dim dr As DialogResult = Me.Af1.ShowAsyncMessageBoxWin( _
        '    "メッセージ", "タイトル", MessageBoxButtons.YesNo, MessageBoxIcon.Information)

        ' '' 非同期メッセージボックス表示のテスト（エラー）
        ''System.Windows.MessageBoxResult mr = af.ShowAsyncMessageBoxWPF("メッセージ", "タイトル",
        ' ''System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information);

        ' 結果表示
        Return testReturnValue
    End Function

    Sub MyChangeProgress1(ByVal o As Object)
        MessageBox.Show(o.ToString())
    End Sub

    Sub MySetResult1(ByVal retVal As Object)
        If TypeOf retVal Is Exception Then
            ' 例外発生時
            RcMyCmnFunction.ShowErrorMessageWin(retVal, "非同期処理で例外発生！")
        Else
            ' 正常時

            ' 戻り値（キャスト）
            Dim testReturnValue As TestReturnValue = DirectCast(retVal, TestReturnValue)

            ' 結果表示するメッセージ エリア
            Me.labelMessage.Content = ""

            If testReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
                Me.labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCrLf
                Me.labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCrLf
                Me.labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCrLf
            Else
                ' 結果（正常系）
                Me.labelMessage.Content = testReturnValue.Obj.ToString() + "件のデータがあります"
            End If
        End If
    End Sub

    ''' <summary>一覧取得（dt）</summary>
    Private Sub button2_Click(sender As Object, e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(
            Me.Name, DirectCast(sender, Button).Name, "SelectAll_DT",
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Content = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            Me.dataGrid1.Columns.Clear()
            Me.dataGrid1.DataContext = testReturnValue.Obj
        End If
    End Sub

    ''' <summary>一覧取得（ds）</summary>
    Private Sub button3_Click(sender As Object, e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(
            Me.Name, DirectCast(sender, Button).Name, "SelectAll_DS",
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Content = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            Me.dataGrid1.Columns.Clear()
            Me.dataGrid1.DataContext = DirectCast(testReturnValue.Obj, DataSet).Tables(0)
        End If
    End Sub

    ''' <summary>一覧取得（dr）</summary>
    Private Sub button4_Click(sender As Object, e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(
            Me.Name, DirectCast(sender, Button).Name, "SelectAll_DR",
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Content = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            Me.dataGrid1.Columns.Clear()
            Me.dataGrid1.DataContext = testReturnValue.Obj
        End If
    End Sub

    ''' <summary>一覧取得（動的sql）</summary>
    Private Sub button5_Click(sender As Object, e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(
            Me.Name, DirectCast(sender, Button).Name, "SelectAll_DSQL",
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 動的SQLの要素を設定
        testParameterValue.OrderColumn = DirectCast(Me.ddlOrderColumn.SelectedItem, ComboBoxItem).Value
        testParameterValue.OrderSequence = DirectCast(Me.ddlOrderSequence.SelectedItem, ComboBoxItem).Value

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Content = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCr & vbLf
            labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCr & vbLf
            labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCr & vbLf
        Else
            ' 結果（正常系）
            Me.dataGrid1.Columns.Clear()
            Me.dataGrid1.DataContext = testReturnValue.Obj
        End If
    End Sub

    ''' <summary>参照処理</summary>
    ''' <remarks>
    ''' 非同期フレームワークを使用してB層の呼び出し処理を非同期化
    ''' （結果表示にだけ匿名デリゲードを使用するパターン）
    ''' </remarks>
    Private Sub button6_Click(sender As Object, e As RoutedEventArgs)
        ' 非同期処理クラスを生成
        Me.Af2 = New AsyncFunc(Me)

        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue( _
            Me.Name, DirectCast(sender, Button).Name, "Select", _
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 情報の設定
        testParameterValue.ShipperID = Integer.Parse(Me.textBox1.Text)

        ' 引数を非同期処理クラスに設定
        Me.Af2.Parameter = testParameterValue

        ' 画面上のデータは退避する（オブジェクトであれば、クローンする。）
        Me.Af2.LogicalName = DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value

        ' 非同期実行するメソッドを指定
        ' ここは副スレッドから実行されるので注意。
        Me.Af2.AsyncFunc = New BaseAsyncFunc.AsyncFuncDelegate(AddressOf Me.Af2.btn6_Exec)

        ' 結果表示のメソッドを指定（匿名デリゲード）
        ' このメソッドは必ず主スレッドで実行される。
        Me.Af2.SetResult = New BaseAsyncFunc.SetResultDelegate(AddressOf MySetResult2)

        ' 非同期実行する。
        If Not Me.Af2.Start() Then
            MessageBox.Show("別の非同期処理が実行中です。")
        End If
    End Sub

    Sub MySetResult2(ByVal retVal As Object)
        If TypeOf retVal Is Exception Then
            ' 例外発生時
            RcMyCmnFunction.ShowErrorMessageWin(retVal, "非同期処理で例外発生！")
        Else
            ' 正常時

            ' 戻り値（キャスト）
            Dim testReturnValue As TestReturnValue = DirectCast(retVal, TestReturnValue)
            ' 結果表示するメッセージ エリア
            Me.labelMessage.Content = ""

            If testReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
                Me.labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCrLf
                Me.labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCrLf
                Me.labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCrLf
            Else
                ' 結果（正常系）
                Me.textBox1.Text = testReturnValue.ShipperID.ToString()
                Me.textBox2.Text = testReturnValue.CompanyName
                Me.textBox3.Text = testReturnValue.Phone
            End If
        End If
    End Sub

#End Region

#Region "更新系"

    ''' <summary>追加処理</summary>
    Private Sub button7_Click(sender As Object, e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(
            Me.Name, DirectCast(sender, Button).Name, "Insert",
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 情報の設定
        testParameterValue.CompanyName = Me.textBox2.Text
        testParameterValue.Phone = Me.textBox3.Text

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

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
    Private Sub button8_Click(sender As Object, e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(
            Me.Name, DirectCast(sender, Button).Name, "Update",
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 情報の設定
        testParameterValue.ShipperID = Integer.Parse(Me.textBox1.Text)
        testParameterValue.CompanyName = Me.textBox2.Text
        testParameterValue.Phone = Me.textBox3.Text

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

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
    Private Sub button9_Click(sender As Object, e As RoutedEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue(
            Me.Name, DirectCast(sender, Button).Name, "Delete",
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" &
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, Me.myUserInfo)

        ' 情報の設定
        testParameterValue.ShipperID = Integer.Parse(textBox1.Text)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

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
    Private Sub button10_Click(sender As Object, e As RoutedEventArgs)
        Me.dataGrid1.Columns.Clear()
        Me.dataGrid1.DataContext = Nothing
    End Sub

    ''' <summary>メッセージ取得（埋め込まれたリソース対応）</summary>
    Private Sub button11_Click(sender As Object, e As RoutedEventArgs)
        Me.textBox5.Text = GetMessage.GetMessageDescription(Me.textBox4.Text)
    End Sub

    ''' <summary>共有情報取得（埋め込まれたリソース対応）</summary>
    Private Sub button12_Click(sender As Object, e As RoutedEventArgs)
        Me.textBox7.Text = GetSharedProperty.GetSharedPropertyValue(Me.textBox6.Text)
    End Sub

#End Region
End Class
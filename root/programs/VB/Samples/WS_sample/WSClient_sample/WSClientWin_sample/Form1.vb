'**********************************************************************************
'* ３層型 サンプル アプリ画面
'**********************************************************************************

' テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：Form1
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

Imports Touryo.Infrastructure.Business.RichClient.Presentation
Imports Touryo.Infrastructure.Business.RichClient.Asynchronous
Imports Touryo.Infrastructure.Business.RichClient.Util
Imports Touryo.Infrastructure.Framework.RichClient.Presentation
Imports Touryo.Infrastructure.Framework.RichClient.Asynchronous
Imports Touryo.Infrastructure.Framework.Transmission
Imports Touryo.Infrastructure.Framework.Util

''' <summary>サンプル アプリ画面</summary>
''' <remarks>
''' [ファイル内にデザインできるクラスがないため、このファイルのデザイナを表示できませんでした。]
''' というエラーメッセージが出力される場合は、プログラム一式のフォルダの検索ンデックスをOFFにします。
''' 
''' 1.プログラム一式が格納されているフォルダを選択して右クリック
''' 2.詳細設定で「検索を早くするために」のチェックをOFFにする。
''' （サブフォルダ、ファイルにも適用すること。）
''' </remarks>
Partial Public Class Form1
    Inherits MyBaseControllerWin

    ''' <summary>呼出し制御部品</summary>
    Private CallCtrl As CallController

    ''' <summary>非同期実行クラス</summary>
    Private Af1 As MyBaseAsyncFunc = Nothing

    ''' <summary>非同期実行クラス</summary>
    Private Af2 As AsyncFunc = Nothing

    ''' <summary>画面上のデータは退避する</summary>
    Private LogicalName As String

#Region "初期処理"

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' フォームロードのUOCメソッド
    ''' </summary>
    Protected Overrides Sub UOC_FormInit()
        ' フォーム初期化（初回ロード）時に実行する処理を実装する

        ' TODO:

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
        Me.ddlIso.Enabled = False

        ' ddlExRollback
        Me.ddlExRollback.Items.Add(New ComboBoxItem("正常時", "-"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("業務例外", "Business"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("システム例外", "System"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("その他、一般的な例外", "Other"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("業務例外への振替", "Other-Business"))
        Me.ddlExRollback.Items.Add(New ComboBoxItem("システム例外への振替", "Other-System"))
        Me.ddlExRollback.SelectedIndex = 0

        ' ddlTransmission
        Me.ddlTransmission.Items.Add(New ComboBoxItem("ASP.NET Webサービス呼出", "testWebService"))
        Me.ddlTransmission.Items.Add(New ComboBoxItem("WCF Webサービス呼出", "testWebService2"))
        Me.ddlTransmission.Items.Add(New ComboBoxItem("WCF TCPサービス呼出", "testWebService3"))
        Me.ddlTransmission.Items.Add(New ComboBoxItem("ASP.NET WebAPI呼出", "testWebService4"))
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

        ' 呼出し制御部品
        Me.CallCtrl = New CallController(Program.AccessToken)

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
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    ''' <remarks>
    ''' 非同期フレームワークを使用してB層の呼び出し処理を非同期化
    ''' （非同期実行、結果表示の双方に匿名デリゲードを使用するパターン）
    ''' </remarks>
    Protected Sub UOC_btnButton1_Click(ByVal rcFxEventArgs As RcFxEventArgs)
       
        ' 非同期処理クラスを生成
        ' 匿名デリゲードの場合は、ベース２で良い。
        Me.Af1 = New MyBaseAsyncFunc(Me)

        Me.LogicalName = DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value

        ' 引数を纏める
        Me.Af1.Parameter = New TestParameterValue( _
            Me.Name, rcFxEventArgs.ControlName, "SelectCount", _
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' 非同期実行するメソッドを指定（匿名デリゲード）
        ' ここは副スレッドから実行されるので注意
        ' （画面上のメンバに触らないこと！）。
        Me.Af1.AsyncFunc = New BaseAsyncFunc.AsyncFuncDelegate(AddressOf MyAsyncFunc1)

        ' 結果表示のメソッドを指定（匿名デリゲード）
        ' このメソッドは必ず主スレッドで実行される。
        ' （画面上のメンバを更新できる！）。
        Me.Af1.SetResult = New BaseAsyncFunc.SetResultDelegate(AddressOf MySetResult1)

        ' 非同期実行する。
        If Not Me.Af1.Start() Then
            MessageBox.Show("別の非同期処理が実行中です。")
        End If
    End Sub

    Function MyAsyncFunc1(ByVal param As Object)
        
        ' 引数クラス（キャスト）
        Dim testParameterValue As TestParameterValue = DirectCast(param, TestParameterValue)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' 呼出し制御部品（スレッドセーフでないため副スレッド内で作る）
        Dim callCtrl As New CallController(Program.AccessToken)

        ' Invoke
        testReturnValue = callCtrl.Invoke(Me.LogicalName, testParameterValue)

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
            Me.labelMessage.Text = ""

            If testReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
                Me.labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCrLf
                Me.labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCrLf
                Me.labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCrLf
            Else
                ' 結果（正常系）
                Me.labelMessage.Text = testReturnValue.Obj.ToString() + "件のデータがあります"
            End If
        End If
    End Sub

    ''' <summary>一覧取得（dt）</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_btnButton2_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue( _
            Me.Name, rcFxEventArgs.ControlName, "SelectAll_DT", _
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCrLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCrLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCrLf
        Else
            ' 結果（正常系）
            Me.dataGridView1.DataSource = testReturnValue.Obj
        End If
    End Sub

    ''' <summary>一覧取得（ds）</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_btnButton3_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue( _
            Me.Name, rcFxEventArgs.ControlName, "SelectAll_DS", _
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCrLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCrLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCrLf
        Else
            ' 結果（正常系）
            Me.dataGridView1.DataSource = DirectCast(testReturnValue.Obj, DataSet).Tables(0)
        End If
    End Sub

    ''' <summary>一覧取得（dr）</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_btnButton4_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue( _
            Me.Name, rcFxEventArgs.ControlName, "SelectAll_DR", _
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCrLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCrLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCrLf
        Else
            ' 結果（正常系）
            Me.dataGridView1.DataSource = testReturnValue.Obj
        End If
    End Sub

    ''' <summary>一覧取得（動的sql）</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_btnButton5_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue( _
            Me.Name, rcFxEventArgs.ControlName, "SelectAll_DSQL", _
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' 動的SQLの要素を設定
        testParameterValue.OrderColumn = DirectCast(Me.ddlOrderColumn.SelectedItem, ComboBoxItem).Value
        testParameterValue.OrderSequence = DirectCast(Me.ddlOrderSequence.SelectedItem, ComboBoxItem).Value

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCrLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCrLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCrLf
        Else
            ' 結果（正常系）
            Me.dataGridView1.DataSource = testReturnValue.Obj
        End If
    End Sub

    ''' <summary>参照処理</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    ''' <remarks>
    ''' 非同期フレームワークを使用してB層の呼び出し処理を非同期化
    ''' （結果表示にだけ匿名デリゲードを使用するパターン）
    ''' </remarks>
    Protected Sub UOC_btnButton6_Click(ByVal rcFxEventArgs As RcFxEventArgs)

        ' 非同期処理クラスを生成
        Me.Af2 = New AsyncFunc(Me)

        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue( _
            Me.Name, rcFxEventArgs.ControlName, "Select", _
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

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
            Me.labelMessage.Text = ""

            If testReturnValue.ErrorFlag = True Then
                ' 結果（業務続行可能なエラー）
                Me.labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCrLf
                Me.labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCrLf
                Me.labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCrLf
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
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_btnButton7_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue( _
            Me.Name, rcFxEventArgs.ControlName, "Insert", _
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' 情報の設定
        testParameterValue.CompanyName = Me.textBox2.Text
        testParameterValue.Phone = Me.textBox3.Text

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCrLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCrLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCrLf
        Else
            ' 結果（正常系）
            labelMessage.Text = testReturnValue.Obj.ToString() + "件追加"
        End If
    End Sub

    ''' <summary>更新処理</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_btnButton8_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue( _
            Me.Name, rcFxEventArgs.ControlName, "Update", _
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' 情報の設定
        testParameterValue.ShipperID = Integer.Parse(Me.textBox1.Text)
        testParameterValue.CompanyName = Me.textBox2.Text
        testParameterValue.Phone = Me.textBox3.Text

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCrLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCrLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCrLf
        Else
            ' 結果（正常系）
            labelMessage.Text = testReturnValue.Obj.ToString() + "件更新"
        End If
    End Sub

    ''' <summary>削除処理</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_btnButton9_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        ' 引数クラスを生成
        ' 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        Dim testParameterValue As New TestParameterValue( _
            Me.Name, rcFxEventArgs.ControlName, "Delete", _
            DirectCast(Me.ddlDap.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode1.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlMode2.SelectedItem, ComboBoxItem).Value & "%" & _
            DirectCast(Me.ddlExRollback.SelectedItem, ComboBoxItem).Value, MyBaseControllerWin.UserInfo)

        ' 情報の設定
        testParameterValue.ShipperID = Integer.Parse(textBox1.Text)

        ' 戻り値
        Dim testReturnValue As TestReturnValue

        ' Invoke
        testReturnValue = Me.CallCtrl.Invoke(DirectCast(Me.ddlTransmission.SelectedItem, ComboBoxItem).Value, testParameterValue)

        ' 結果表示するメッセージ エリア
        Me.labelMessage.Text = ""

        If testReturnValue.ErrorFlag = True Then
            ' 結果（業務続行可能なエラー）
            labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID & vbCrLf
            labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage & vbCrLf
            labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo & vbCrLf
        Else
            ' 結果（正常系）
            labelMessage.Text = testReturnValue.Obj.ToString() + "件削除"
        End If
    End Sub

#End Region

#End Region

#Region "その他"

    ''' <summary>クリア</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_btnButton10_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Me.dataGridView1.DataSource = Nothing
    End Sub

    ''' <summary>メッセージ取得（埋め込まれたリソース対応）</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_btnButton11_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Me.textBox5.Text = GetMessage.GetMessageDescription(Me.textBox4.Text)
    End Sub

    ''' <summary>共有情報取得（埋め込まれたリソース対応）</summary>
    ''' <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
    Protected Sub UOC_btnButton12_Click(ByVal rcFxEventArgs As RcFxEventArgs)
        Me.textBox7.Text = GetSharedProperty.GetSharedPropertyValue(Me.textBox6.Text)
    End Sub

#End Region

End Class
'**********************************************************************************
'* フレームワーク・テスト画面（部品）
'**********************************************************************************

' テスト画面なので、必要に応じて流用 or 削除して下さい。

'**********************************************************************************
'* クラス名        ：testScreen
'* クラス日本語名  ：共通部品テスト画面
'*
'* 作成日時        ：－
'* 作成者          ：－
'* 更新履歴        ：－
'*
'*  日時        更新者            内容
'*  ----------  ----------------  -------------------------------------------------
'*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
'**********************************************************************************

Imports System.IO
Imports System.Security.Principal
Imports System.Collections.ObjectModel

Imports Touryo.Infrastructure.Business.Str
Imports Touryo.Infrastructure.Business.Util
Imports Touryo.Infrastructure.Framework.Util

Imports Touryo.Infrastructure.Public.IO
Imports Touryo.Infrastructure.Public.Log
Imports Touryo.Infrastructure.Public.Str
Imports Touryo.Infrastructure.Public.Util

Namespace Aspx.TestPublic
    ''' <summary>共通部品テスト画面</summary>
    Partial Public Class testScreen
        Inherits System.Web.UI.Page
#Region "初期化処理"

        ''' <summary>初期化処理</summary>
        Protected Sub Page_Load(sender As Object, e As EventArgs)
            If Not Me.IsPostBack Then
                Me.txtFilePath1.Text = GetConfigParameter.GetConfigValue("TestFilePath") & "\testWrite.txt"

                Me.txtFilePath2.Text = GetConfigParameter.GetConfigValue("TestFilePath") & "\testRead.txt"

                Me.txtStrIn.Text = "az、ａｚ、AZ、ＡＺ、09、０９、@%&、＠％＆、ｱア、あ亜、 　"

                '---

                'コンピュータ上に存在するすべてのタイムゾーンをComboBoxに表示
                Dim dic_tzs As New Dictionary(Of String, String)()
                Dim roc_tzs As ReadOnlyCollection(Of TimeZoneInfo) = TimeZoneInfo.GetSystemTimeZones()
                For Each tz As TimeZoneInfo In roc_tzs
                    dic_tzs.Add(tz.DisplayName, tz.Id)
                Next

                Me.ddlTimeZone.DataSource = dic_tzs
                Me.ddlTimeZone.DataTextField = "Key"
                Me.ddlTimeZone.DataValueField = "Value"
                Me.ddlTimeZone.DataBind()
            End If

            Me.MaintainScrollPositionOnPostBack = True
        End Sub

#End Region

#Region "ログ出力のテスト"

        ''' <summary>デバッグログ</summary>
        Protected Sub btnDebugLog_Click(sender As Object, e As EventArgs)
            ' Log4Netへデバッグ・ログ出力
            LogIF.DebugLog(ddlLog.SelectedItem.Text, "わしょ～い")
        End Sub

        ''' <summary>情報ログ</summary>
        Protected Sub btnInfoLog_Click(sender As Object, e As EventArgs)
            ' Log4Netへデバッグ・ログ出力
            LogIF.InfoLog(ddlLog.SelectedItem.Text, "わしょ～い")
        End Sub

        ''' <summary>警告ログ</summary>
        Protected Sub btnWarnLog_Click(sender As Object, e As EventArgs)
            ' Log4Netへデバッグ・ログ出力
            LogIF.WarnLog(ddlLog.SelectedItem.Text, "わしょ～い")
        End Sub

        ''' <summary>（通常の）エラーログ</summary>
        Protected Sub btnErrLog_Click(sender As Object, e As EventArgs)
            ' Log4Netへデバッグ・ログ出力
            LogIF.ErrorLog(ddlLog.SelectedItem.Text, "わしょ～い")
        End Sub

        ''' <summary>致命的なエラーログ</summary>
        Protected Sub btnFatalLog_Click(sender As Object, e As EventArgs)
            ' Log4Netへデバッグ・ログ出力
            LogIF.FatalLog(ddlLog.SelectedItem.Text, "わしょ～い")
        End Sub

#End Region

#Region "性能測定のテスト"

        ''' <summary>インクリメント</summary>
        Protected Sub btnRoop_Click(sender As Object, e As EventArgs)
            ' ループ回数
            Dim j As Integer = Integer.Parse(Me.txtExecCnt.Text)

            Dim calc As Integer = 1

            ' 性能測定オブジェクト
            Dim prec As New PerformanceRecorder()

            ' 開始
            prec.StartsPerformanceRecord()

            ' ループ処理
            For i As Integer = 1 To j
                calc += 1
            Next

            ' 終了
            Me.lblPrefRec1.Text = prec.EndsPerformanceRecord()

            ' 結果の出力
            Me.lblPrefRec2.Text = prec.ExecTime
            Me.lblPrefRec3.Text = prec.CpuTime
            Me.lblPrefRec4.Text = prec.CpuKernelTime
            Me.lblPrefRec5.Text = prec.CpuUserTime
        End Sub

        ''' <summary>スリープ</summary>
        Protected Sub btnSleep_Click(sender As Object, e As EventArgs)
            ' ループ回数
            Dim j As Integer = Integer.Parse(Me.txtExecCnt.Text)

            ' 性能測定オブジェクト
            Dim prec As New PerformanceRecorder()

            ' 開始
            prec.StartsPerformanceRecord()

            ' ループ処理
            For i As Integer = 1 To j
                System.Threading.Thread.Sleep(0)
            Next

            ' 終了
            Me.lblPrefRec1.Text = prec.EndsPerformanceRecord()

            ' 結果の出力
            Me.lblPrefRec2.Text = prec.ExecTime
            Me.lblPrefRec3.Text = prec.CpuTime
            Me.lblPrefRec4.Text = prec.CpuKernelTime
            Me.lblPrefRec5.Text = prec.CpuUserTime
        End Sub

        ''' <summary>ファイルIO（書込）</summary>
        Protected Sub btnFileIOO_Click(sender As Object, e As EventArgs)
            ' ループ回数
            Dim j As Integer = Integer.Parse(Me.txtExecCnt.Text)

            ' 性能測定オブジェクト
            Dim prec As New PerformanceRecorder()

            ' 開始
            prec.StartsPerformanceRecord()

            ' StreamWriter
            Dim sw As New StreamWriter(Me.txtFilePath1.Text, False, Encoding.GetEncoding(CustomEncode.UTF_8))

            ' ループ処理
            For i As Integer = 1 To j
                sw.WriteLine("テスト てすと 手素戸")
            Next

            sw.Close()

            ' 終了
            Me.lblPrefRec1.Text = prec.EndsPerformanceRecord()

            ' 結果の出力
            Me.lblPrefRec2.Text = prec.ExecTime
            Me.lblPrefRec3.Text = prec.CpuTime
            Me.lblPrefRec4.Text = prec.CpuKernelTime
            Me.lblPrefRec5.Text = prec.CpuUserTime
        End Sub

        ''' <summary>ファイルIO（読込）</summary>
        Protected Sub btnFileIOI_Click(sender As Object, e As EventArgs)
            ' ループ回数
            Dim j As Integer = Integer.Parse(Me.txtExecCnt.Text)

            ' 性能測定オブジェクト
            Dim prec As New PerformanceRecorder()

            ' 開始
            prec.StartsPerformanceRecord()

            Dim sr As New StreamReader(Me.txtFilePath1.Text, Encoding.GetEncoding(CustomEncode.UTF_8))

            ' ループ処理
            For i As Integer = 1 To j
                Me.lblFileData.Text = sr.ReadLine()
            Next

            sr.Close()

            ' 終了
            Me.lblPrefRec1.Text = prec.EndsPerformanceRecord()

            ' 結果の出力
            Me.lblPrefRec2.Text = prec.ExecTime
            Me.lblPrefRec3.Text = prec.CpuTime
            Me.lblPrefRec4.Text = prec.CpuKernelTime
            Me.lblPrefRec5.Text = prec.CpuUserTime
        End Sub

#End Region

#Region "ファイルIO部品のテスト"

        ''' <summary>ファイルIO（リソースのロード１）</summary>
        Protected Sub btnLoadResource1_Click(sender As Object, e As EventArgs)
            ' チェック
            If ResourceLoader.Exists(txtFilePath2.Text, False) Then
                ' 存在する場合

                If Me.cbxBinaryMode.Checked Then
                    ' BinaryWriter
                    Dim fs As New FileStream(Me.txtFilePath2.Text, FileMode.Open, FileAccess.Read)
                    Dim br As New BinaryReader(fs)

                    ' １Ｍ読み込み
                    Dim buffer As Byte() = br.ReadBytes(1024 * 1024)
                    br.Close()

                    Me.txtRetFileLoad.Text = CustomEncode.ByteToString(buffer, CustomEncode.UTF_8)
                Else
                    ' ResourceLoader
                    Me.txtRetFileLoad.Text = ResourceLoader.LoadAsString(txtFilePath2.Text, Encoding.GetEncoding(CustomEncode.UTF_8))
                End If
            Else
                ' 存在しない場合、例外
                ResourceLoader.Exists(txtFilePath2.Text, True)
            End If
        End Sub

        ''' <summary>ファイルIO（リソースのロード２）</summary>
        Protected Sub btnLoadResource2_Click(sender As Object, e As EventArgs)
            ' チェック
            If ResourceLoader.Exists(txtFilePath2.Text, txtFileName.Text, False) Then
                ' 存在する場合

                If Me.cbxBinaryMode.Checked Then
                    ' BinaryWriter
                    Dim fs As New FileStream(Path.Combine(txtFilePath2.Text, txtFileName.Text), FileMode.Open, FileAccess.Read)

                    Dim br As New BinaryReader(fs)

                    ' １Ｍ読み込み
                    Dim buffer As Byte() = br.ReadBytes(1024 * 1024)
                    br.Close()

                    Me.txtRetFileLoad.Text = CustomEncode.ByteToString(buffer, CustomEncode.UTF_8)
                Else
                    ' ResourceLoader
                    Me.txtRetFileLoad.Text = ResourceLoader.LoadAsString(txtFilePath2.Text, txtFileName.Text, Encoding.GetEncoding(CustomEncode.UTF_8))
                End If
            Else
                ' 存在しない場合、例外
                ResourceLoader.Exists(txtFilePath2.Text, txtFileName.Text, True)
            End If
        End Sub

#End Region

#Region "共有情報取得部品"

        ''' <summary>共有情報の取得</summary>
        Protected Sub btnGetSP_Click(sender As Object, e As EventArgs)
            ' 共有情報を取得する。
            Me.lblSP.Text = GetSharedProperty.GetSharedPropertyValue(Me.txtSPID.Text)
        End Sub

#End Region

#Region "Message取得部品"

        ''' <summary>Messageの取得</summary>
        Protected Sub btnGetMSG_Click(sender As Object, e As EventArgs)
            ' Messageを取得する。
            Me.lblMSG.Text = GetMessage.GetMessageDescription(Me.txtMSGID.Text)
        End Sub

#End Region

#Region "トランザクション制御機能"

        '// <summary>トランザクション制御機能のテスト（InitDam）</summary>
        'protected void btnTxPID_Click(object sender, EventArgs e)
        '{
        '    // 引数クラスを生成
        '    // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        '    MyParameterValue myParameterValue
        '        = new MyParameterValue(
        '              "画面ID", "ButtonID",
        '              Me.ddlDap.SelectedValue + "%"
        '              + Me.ddlExRollback.SelectedValue + "%"
        '              + Me.ddlExStatus.SelectedValue,
        '              new MyUserInfo("ユーザ名", Request.UserHostAddress));

        '    // ※ ActionTypeのフォーマット：Dap%Err%Stat%

        '    MyBaseLogic testMTC;

        '    // B層を生成
        '    if (Me.cbxCnnMode.Checked)
        '    {
        '        // マルチ コネクション モード
        '        testMTC = new TestMTC_mcn();
        '    }
        '    else
        '    {
        '        // シングル コネクション モード
        '        testMTC = new TestMTC();
        '    }

        '    // 業務処理を実行
        '    MyReturnValue myReturnValue =
        '        (MyReturnValue)testMTC.DoBusinessLogic(
        '            (BaseParameterValue)myParameterValue,
        '            DbEnum.IsolationLevelEnum.User);
        '}

        '// <summary>トランザクション制御機能のテスト（GetTransactionPatterns）</summary>
        'protected void btnTxGID_Click(object sender, EventArgs e)
        '{
        '    // 引数クラスを生成
        '    // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        '    MyType.TestParameterValue testParameterValue
        '        = new MyType.TestParameterValue(
        '              "", "画面ID", "ButtonID",
        '              Me.ddlDap.SelectedValue + "%"
        '              + Me.ddlExRollback.SelectedValue + "%"
        '              + Me.ddlExStatus.SelectedValue,
        '              new MyUserInfo("ユーザ名", Request.UserHostAddress));

        '    // ※ ActionTypeのフォーマット：Dap

        '    // TransactionGroupIDを設定
        '    testParameterValue.Obj = Me.ddlTxGpID.SelectedValue;

        '    // 業務処理を実行
        '    TestMTC_txg testMTC = new TestMTC_txg();

        '    MyReturnValue myReturnValue =
        '        (MyReturnValue)testMTC.DoBusinessLogic(
        '            (BaseParameterValue)testParameterValue,
        '            DbEnum.IsolationLevelEnum.User);

        '    Me.lblTxID.Text = "";

        '    // 例外判定
        '    if (myReturnValue.ErrorFlag)
        '    {
        '        // Error Message
        '        Me.lblTxID.Text = myReturnValue.ErrorMessage;
        '    }
        '    else
        '    {
        '        string[] temp1 = (string[])((MyType.TestReturnValue)myReturnValue).Obj;

        '        // TransactionPatternIDをリストする。
        '        foreach (string temp2 in temp1)
        '        {
        '            Me.lblTxID.Text += temp2 + "<br/>";
        '        }
        '    }
        '}

#End Region

#Region "JIS系"

#Region "JIS2004"

#Region "情報表示"

        ''' <summary>JIS2004追加文字の文字列を表示</summary>
        Protected Sub btnDispJis2K4_Click(sender As Object, e As EventArgs)
            Dim jis2k4 As New JIS2k4Checker()
            Me.lblJis2K4.Text = jis2k4.JIS2k4String
        End Sub

        ''' <summary>文字列の情報を出力する。</summary>
        Protected Sub btnDispJis2K4Info_Click(sender As Object, e As EventArgs)
            ' GetStringInfoメソッドのテスト
            Dim jis2k4 As New JIS2k4Checker()

            Dim s_length As Integer
            Dim si_length As Integer
            Dim byte_length As Integer

            jis2k4.GetStringInfo(Me.txtJis2K4Input.Text, s_length, si_length, byte_length)

            Me.lblJis2K4Output.Text = "Char長:" & s_length.ToString() & "; " & "文字列長" & si_length.ToString() & "; " & "バイト長" & byte_length.ToString() & "; "
        End Sub

#End Region

#Region "サロゲート ペア文字"

        ''' <summary>サロゲート ペア文字のチェック１</summary>
        Protected Sub btnCheckSPC1_Click(sender As Object, e As EventArgs)
            ' CheckSurrogatesPairChar(1)メソッドのテスト
            Dim jis2k4 As New JIS2k4Checker()

            If jis2k4.CheckSurrogatesPairChar(Me.txtJis2K4Input.Text) Then
                Me.lblJis2K4Output.Text = "サロゲート ペア文字が含まれます。"
            Else
                Me.lblJis2K4Output.Text = "サロゲート ペア文字が含まれません。"
            End If
        End Sub

        ''' <summary>サロゲート ペア文字のチェック２</summary>
        Protected Sub btnCheckSPC2_Click(sender As Object, e As EventArgs)
            ' CheckSurrogatesPairChar(2)メソッドのテスト
            Dim jis2k4 As New JIS2k4Checker()

            Dim index As Integer

            If jis2k4.CheckSurrogatesPairChar(Me.txtJis2K4Input.Text, index) Then
                Me.lblJis2K4Output.Text = "サロゲート ペア文字が" & (index + 1).ToString() & "文字目に含まれます。"
            Else
                Me.lblJis2K4Output.Text = "サロゲート ペア文字が含まれません。"
            End If
        End Sub

        ''' <summary>サロゲート ペア文字の削除</summary>
        Protected Sub btnDelSPC_Click(sender As Object, e As EventArgs)
            ' DeleteSurrogatesPairCharメソッドのテスト
            Dim jis2k4 As New JIS2k4Checker()
            Me.lblJis2K4Output.Text = jis2k4.DeleteSurrogatesPairChar(Me.txtJis2K4Input.Text)
        End Sub

        ''' <summary>サロゲート ペア文字の置換</summary>
        Protected Sub btnRepSPC1_Click(sender As Object, e As EventArgs)
            ' DeleteSurrogatesPairCharメソッドのテスト
            Dim jis2k4 As New JIS2k4Checker()

            If Me.txtJis2K4Replace.Text <> "" Then
                Me.lblJis2K4Output.Text = jis2k4.DeleteSurrogatesPairChar(Me.txtJis2K4Input.Text, Me.txtJis2K4Replace.Text(0))
            End If
        End Sub

        ''' <summary>サロゲート ペア文字の置換</summary>
        Protected Sub btnRepSPC2_Click(sender As Object, e As EventArgs)
            ' DeleteSurrogatesPairCharメソッドのテスト
            Dim jis2k4 As New JIS2k4Checker()
            Me.lblJis2K4Output.Text = jis2k4.DeleteSurrogatesPairChar(Me.txtJis2K4Input.Text, Me.txtJis2K4Replace.Text)
        End Sub

#End Region

#Region "追加文字"

        ''' <summary>JIS2004追加文字のチェック１</summary>
        Protected Sub btnCheckJis2K4_1_Click(sender As Object, e As EventArgs)
            ' CheckCharAddedWithJIS2k4(1)メソッドのテスト
            Dim jis2k4 As New JIS2k4Checker()

            If jis2k4.CheckCharAddedWithJIS2k4(Me.txtJis2K4Input.Text) Then
                Me.lblJis2K4Output.Text = "JIS2004追加文字が含まれます。"
            Else
                Me.lblJis2K4Output.Text = "JIS2004追加文字が含まれません。"
            End If
        End Sub

        ''' <summary>JIS2004追加文字のチェック２</summary>
        Protected Sub btnCheckJis2K4_2_Click(sender As Object, e As EventArgs)
            ' CheckCharAddedWithJIS2k4(2)メソッドのテスト
            Dim jis2k4 As New JIS2k4Checker()

            Dim index As Integer

            If jis2k4.CheckCharAddedWithJIS2k4(Me.txtJis2K4Input.Text, index) Then
                Me.lblJis2K4Output.Text = "JIS2004追加文字が" & (index + 1).ToString() & "文字目に含まれます。"
            Else
                Me.lblJis2K4Output.Text = "JIS2004追加文字が含まれません。"
            End If
        End Sub

        ''' <summary>JIS2004追加文字の削除</summary>
        Protected Sub btnDelJis2K4_Click(sender As Object, e As EventArgs)
            ' DeleteCharAddedWithJIS2k4メソッドのテスト
            Dim jis2k4 As New JIS2k4Checker()
            Me.lblJis2K4Output.Text = jis2k4.DeleteCharAddedWithJIS2k4(Me.txtJis2K4Input.Text)
        End Sub

        ''' <summary>JIS2004追加文字の置換</summary>
        Protected Sub btnRepJis2K4_1_Click(sender As Object, e As EventArgs)
            ' DeleteSurrogatesPairCharメソッドのテスト
            Dim jis2k4 As New JIS2k4Checker()

            If Me.txtJis2K4Replace.Text <> "" Then
                Me.lblJis2K4Output.Text = jis2k4.DeleteCharAddedWithJIS2k4(Me.txtJis2K4Input.Text, Me.txtJis2K4Replace.Text(0))
            End If
        End Sub

        ''' <summary>JIS2004追加文字の置換</summary>
        Protected Sub btnRepJis2K4_2_Click(sender As Object, e As EventArgs)
            ' DeleteSurrogatesPairCharメソッドのテスト
            Dim jis2k4 As New JIS2k4Checker()
            Me.lblJis2K4Output.Text = jis2k4.DeleteCharAddedWithJIS2k4(Me.txtJis2K4Input.Text, Me.txtJis2K4Replace.Text)
        End Sub

#End Region

#End Region

#Region "JISX0208-1983"

        ''' <summary>JISX0208-1983チェック</summary>
        Protected Sub btnCheckJISX0208_1983_Click(sender As Object, e As EventArgs)
            Dim index As Integer
            Dim ch As String = ""
            If JISX0208_1983Checker.IsJISX0208_1983(Me.txtCheckJISX0208_1983.Text, index, ch) Then
                Me.lblCheckJISX0208_1983.Text = "成功！"
            Else
                Me.lblCheckJISX0208_1983.Text = "失敗 index:" & index & ", ch:" & ch
            End If
        End Sub

#End Region

#End Region

#Region "ローカル⇔UTC対応"

        ''' <summary>ローカル→UTC対応</summary>
        Protected Sub btnLocalToUtc_Click(sender As Object, e As EventArgs)
            'DateTime dt = new DateTime(Me.Calendar1.SelectedDate.Ticks, DateTimeKind.Local); // 正
            Dim dt As New DateTime(Me.Calendar1.SelectedDate.Ticks, DateTimeKind.Unspecified)
            ' 略
            'DateTime dt = new DateTime(Me.Calendar1.SelectedDate.Ticks, DateTimeKind.Utc); // 不正
            If Me.cbxTimeZone.Checked Then
                dt = GMTMaster.ConvertLocalTimeToUtcTime35(dt, TimeZoneInfo.FindSystemTimeZoneById(Me.ddlTimeZone.SelectedItem.Value))
            Else
                dt = GMTMaster.ConvertLocalTimeToUtcTime35(dt, TimeZoneInfo.Local)
            End If
            Me.lblDateString.Text = dt.ToString("yyyy/MM/dd HH:mm:ss.fff")
        End Sub

        ''' <summary>UTC→ローカル対応</summary>
        Protected Sub btnUtcToLocal_Click(sender As Object, e As EventArgs)
            'DateTime dt = new DateTime(Me.Calendar1.SelectedDate.Ticks, DateTimeKind.Utc); // 正
            Dim dt As New DateTime(Me.Calendar1.SelectedDate.Ticks, DateTimeKind.Unspecified)
            ' 略
            'DateTime dt = new DateTime(Me.Calendar1.SelectedDate.Ticks, DateTimeKind.Local); // 不正
            If Me.cbxTimeZone.Checked Then
                dt = GMTMaster.ConvertUtcTimeToLocalTime35(dt, TimeZoneInfo.FindSystemTimeZoneById(Me.ddlTimeZone.SelectedItem.Value))
            Else
                dt = GMTMaster.ConvertUtcTimeToLocalTime35(dt, TimeZoneInfo.Local)
            End If
            Me.lblDateString.Text = dt.ToString("yyyy/MM/dd HH:mm:ss.fff")
        End Sub

#End Region

#Region "文字列処理"

        ''' <summary>出力を入力に設定</summary>
        Protected Sub btnCopy_Click(sender As Object, e As EventArgs)
            Me.txtStrIn.Text = Me.lblStrOut.Text
        End Sub

#Region "CustomEncode"

        ''' <summary>HTMLエンコード（サニタイジング）</summary>
        Protected Sub btnHtmlEncode_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = CustomEncode.HtmlEncode(""" id=""txtXXXXX"" />" & "<script type=""text/javascript"">alert(""XSS!!!"")</script>" & "<input name=""txtXXXXX"" type=""text"" value=""")
        End Sub

        ''' <summary>URLエンコード</summary>
        Protected Sub btnUrlEncode_Click(sender As Object, e As EventArgs)
            ' グーグルで「&」を検索したい。
            Response.Redirect("http://www.google.co.jp/search?hl=ja&q=" & CustomEncode.UrlEncode("&"))

            '/ ASP.NETでは？
            'Response.Redirect(
            '    "testScreen.aspx?"
            '    + CustomEncode.UrlEncode("&name&")
            '    + "="
            '    + CustomEncode.UrlEncode("&value&"));
        End Sub

#End Region

#Region "StringConverter"

        ''' <summary>半角へ</summary>
        Protected Sub btnToHankaku_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringConverter.ToHankaku(Me.txtStrIn.Text)

        End Sub

        ''' <summary>全角へ</summary>
        Protected Sub btnToZenkaku_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringConverter.ToZenkaku(Me.txtStrIn.Text)
        End Sub

        ''' <summary>片仮名へ</summary>
        Protected Sub btnToKatakana_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringConverter.ToKatakana(Me.txtStrIn.Text)
        End Sub

        ''' <summary>平仮名へ</summary>
        Protected Sub btnToHiragana_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringConverter.ToHiragana(Me.txtStrIn.Text)
        End Sub

#End Region

#Region "StringChecker"

#Region "数字"

        ''' <summary>数字チェック</summary>
        Protected Sub btnIsNumbers_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringChecker.IsNumbers(Me.txtStrIn.Text).ToString()
        End Sub
        ''' <summary>数字（半角）チェック</summary>
        Protected Sub btnIsNumbers_Hankaku_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringChecker.IsNumbers_Hankaku(Me.txtStrIn.Text).ToString()
        End Sub
        ''' <summary>数字（全角）チェック</summary>
        Protected Sub btnIsNumbers_Zenkaku_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringChecker.IsNumbers_Zenkaku(Me.txtStrIn.Text).ToString()
        End Sub

#End Region

#Region "英字"

        ''' <summary>英字チェック</summary>
        Protected Sub btnIsAlphabet_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringChecker.IsAlphabet(Me.txtStrIn.Text).ToString()
        End Sub
        ''' <summary>英字（全角）チェック</summary>
        Protected Sub btnIsAlphabet_Hankaku_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringChecker.IsAlphabet_Hankaku(Me.txtStrIn.Text).ToString()
        End Sub
        ''' <summary>英字（半角）チェック</summary>
        Protected Sub btnIsAlphabet_Zenkaku_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringChecker.IsAlphabet_Zenkaku(Me.txtStrIn.Text).ToString()
        End Sub


#End Region

#Region "日本語"

        ''' <summary>平仮名チェック</summary>
        Protected Sub btnIsHiragana_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringChecker.IsHiragana(Me.txtStrIn.Text).ToString()
        End Sub
        ''' <summary>平仮名（全角）チェック</summary>
        Protected Sub btnIsKatakana_Zenkaku_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringChecker.IsKatakana_Zenkaku(Me.txtStrIn.Text).ToString()
        End Sub
        ''' <summary>平仮名（半角）チェック</summary>
        Protected Sub btnIsKatakana_Hankaku_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringChecker.IsKatakana_Hankaku(Me.txtStrIn.Text).ToString()
        End Sub
        ''' <summary>漢字チェック</summary>
        Protected Sub btnIsKanji_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringChecker.IsKanji(Me.txtStrIn.Text).ToString()
        End Sub

#End Region

#Region "SJIS"

        ''' <summary>SJISチェック</summary>
        Protected Sub btnIsShift_Jis_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringChecker.IsShift_Jis(Me.txtStrIn.Text).ToString()
        End Sub

        ''' <summary>SJIS全角チェック</summary>
        Protected Sub btnIsShift_Jis_Zenkaku_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringChecker.IsShift_Jis_Zenkaku(Me.txtStrIn.Text).ToString()
        End Sub
        ''' <summary>SJIS半角チェック</summary>
        Protected Sub btnIsShift_Jis_Hankaku_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = StringChecker.IsShift_Jis_Hankaku(Me.txtStrIn.Text).ToString()
        End Sub

#End Region

#End Region

#Region "FormatConverter"

#Region "和暦・西暦"

        ''' <summary>和暦→西暦</summary>
        Protected Sub btnSeirekiToWareki_Click(sender As Object, e As EventArgs)
            Dim ret As String = ""

            ' 基本バージョン
            ret = FormatConverter.SeirekiToWareki(DateTime.Parse("1977/4/24"), "ggy年M月d日（ddd）")
            Debug.WriteLine(ret)

            ' パターンだけ時間あり
            ret = FormatConverter.SeirekiToWareki(DateTime.Parse("1977/4/24"), "ggy年M月d日（ddd）H:m:s")
            Debug.WriteLine(ret)

            ' DateTimeだけ時間あり
            ret = FormatConverter.SeirekiToWareki(DateTime.Parse("1977/4/24 19:15:12"), "ggy年M月d日（ddd）")
            Debug.WriteLine(ret)

            ' 時間情報込みバージョン（24時間表記）
            ret = FormatConverter.SeirekiToWareki(DateTime.Parse("1977/4/24 19:15:12"), "ggy年M月d日（ddd）H:m:s")
            Debug.WriteLine(ret)

            ' 時間情報込みバージョン（12時間表記）
            ret = FormatConverter.SeirekiToWareki(DateTime.Parse("1977/4/24 19:15:12"), "ggy年M月d日（ddd）tt h:m:s")
            Debug.WriteLine(ret)

            ' 上記のパターン文字列の変更版
            ret = FormatConverter.SeirekiToWareki(DateTime.Parse("1992/2/6 1:1:1"), "ggyy年MM月dd日 dddd HH:mm:ss")
            ' 0埋め2桁
            Debug.WriteLine(ret)

            ret = FormatConverter.SeirekiToWareki(DateTime.Parse("1992/2/6 13:1:1"), "ggyy年MM月dd日 dddd tt hh:mm:ss")
            ' 0埋め2桁
            Debug.WriteLine(ret)
        End Sub

        ''' <summary>西暦→和暦</summary>
        Protected Sub btnWarekiToSeireki_Click(sender As Object, e As EventArgs)
            Dim ret As String = ""

            ' 基本バージョン
            ret = FormatConverter.WarekiToSeireki("昭和52年4月24日（日）", "ggy年M月d日（ddd）").ToString()
            Debug.WriteLine(ret)

            '/ パターンだけ時間あり
            'ret = FormatConverter.WarekiToSeireki(
            '    "昭和52年4月24日（日）", "ggy年M月d日（ddd）H:m:s").ToString();

            '/ 和暦文字列だけ時間あり
            'ret = FormatConverter.WarekiToSeireki(
            '    "昭和52年4月24日（日）12:12:12", "ggy年M月d日（ddd）").ToString();

            ' 時間情報込みバージョン（24時間表記）
            ret = FormatConverter.WarekiToSeireki("昭和52年4月24日（日）19:15:12", "ggy年M月d日（ddd）H:m:s").ToString()
            Debug.WriteLine(ret)

            ' 時間情報込みバージョン（12時間表記）
            ret = FormatConverter.WarekiToSeireki("昭和52年4月24日（日）午後 7:15:12", "ggy年M月d日（ddd）tt h:m:s").ToString()
            Debug.WriteLine(ret)

            ' 上記のパターン文字列の変更版
            ret = FormatConverter.WarekiToSeireki("平成04年02月06日 木曜日 01:01:01", "ggyy年MM月dd日 dddd HH:mm:ss").ToString()
            ' 0埋め2桁
            Debug.WriteLine(ret)

            ret = FormatConverter.WarekiToSeireki("平成04年02月06日 木曜日 午後 01:01:01", "ggyy年MM月dd日 dddd tt hh:mm:ss").ToString()
            ' 0埋め2桁
            Debug.WriteLine(ret)
        End Sub

#End Region

#Region "桁区切り"

        ''' <summary>３桁区切り</summary>
        Protected Sub btnAddFigure3_Click(sender As Object, e As EventArgs)
            Debug.WriteLine(FormatConverter.AddFigure3(12345))
            Debug.WriteLine(FormatConverter.AddFigure3(123456789))
            Debug.WriteLine(FormatConverter.AddFigure3(123.45))
            Debug.WriteLine(FormatConverter.AddFigure3(12345.6789))
            Debug.WriteLine(FormatConverter.AddFigure3(-12345))
            Debug.WriteLine(FormatConverter.AddFigure3(-123456789))
            Debug.WriteLine(FormatConverter.AddFigure3(-123.45))
            Debug.WriteLine(FormatConverter.AddFigure3(-12345.6789))

            Debug.WriteLine(FormatConverter.AddFigure3("12345"))
            Debug.WriteLine(FormatConverter.AddFigure3("123456789"))
            Debug.WriteLine(FormatConverter.AddFigure3("123.45"))
            Debug.WriteLine(FormatConverter.AddFigure3("12345.6789"))
            Debug.WriteLine(FormatConverter.AddFigure3("-12345"))
            Debug.WriteLine(FormatConverter.AddFigure3("-123456789"))
            Debug.WriteLine(FormatConverter.AddFigure3("-123.45"))
            Debug.WriteLine(FormatConverter.AddFigure3("-12345.6789"))

            Debug.WriteLine(FormatConverter.AddFigure3("abcdefghijklnm"))
        End Sub

        ''' <summary>４桁区切り</summary>
        Protected Sub btnAddFigure4_Click(sender As Object, e As EventArgs)
            Debug.WriteLine(FormatConverter.AddFigure4(12345))
            Debug.WriteLine(FormatConverter.AddFigure4(123456789))
            Debug.WriteLine(FormatConverter.AddFigure4(123.45))
            Debug.WriteLine(FormatConverter.AddFigure4(12345.6789))
            Debug.WriteLine(FormatConverter.AddFigure4(-12345))
            Debug.WriteLine(FormatConverter.AddFigure4(-123456789))
            Debug.WriteLine(FormatConverter.AddFigure4(-123.45))
            Debug.WriteLine(FormatConverter.AddFigure4(-12345.6789))

            Debug.WriteLine(FormatConverter.AddFigure4("12345"))
            Debug.WriteLine(FormatConverter.AddFigure4("123456789"))
            Debug.WriteLine(FormatConverter.AddFigure4("123.45"))
            Debug.WriteLine(FormatConverter.AddFigure4("12345.6789"))
            Debug.WriteLine(FormatConverter.AddFigure4("-12345"))
            Debug.WriteLine(FormatConverter.AddFigure4("-123456789"))
            Debug.WriteLine(FormatConverter.AddFigure4("-123.45"))
            Debug.WriteLine(FormatConverter.AddFigure4("-12345.6789"))

            Debug.WriteLine(FormatConverter.AddFigure4("abcdefghijklnm"))
        End Sub

#End Region

#Region "サプレス"

        ''' <summary>サプレス</summary>
        Protected Sub btnSuppress_Click(sender As Object, e As EventArgs)
            Debug.WriteLine(FormatConverter.Suppress("", 10, "＠"c))
            'Debug.WriteLine(FormatConverter.Suppress("123456789", -1, '＠'));
            Debug.WriteLine(FormatConverter.Suppress("123456789", 0, "＠"c))
            Debug.WriteLine(FormatConverter.Suppress("123456789", 1, "＠"c))
            Debug.WriteLine(FormatConverter.Suppress("123456789", 5, "＠"c))
            Debug.WriteLine(FormatConverter.Suppress("123456789", 9, "＠"c))
            Debug.WriteLine(FormatConverter.Suppress("123456789", 10, "＠"c))
            Debug.WriteLine(FormatConverter.Suppress("123456789", 11, "＠"c))
            Debug.WriteLine(FormatConverter.Suppress("123456789", 20, "＠"c))

            Debug.WriteLine(FormatConverter.Suppress("", 10, "0"c))
            'Debug.WriteLine(FormatConverter.Suppress("abcdefg", -1, '0'));
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 0, "0"c))
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 1, "0"c))
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 5, "0"c))
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 7, "0"c))
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 8, "0"c))
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 10, "0"c))
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 20, "0"c))
        End Sub

#End Region

#End Region

#Region "FormatChecker"

#Region "郵便"

#Region "郵便（区）番号"

        ''' <summary>郵便（区）番号</summary>
        Protected Sub btnIsJpZipCode_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpZipCode(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpZipCode--")

            ' 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode("000-0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("aaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000-00000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000-0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("000-00000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("00-000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("00-0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("000-000"))
            Debug.WriteLine("-----")
            ' 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode("000-00"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("aaa-aa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000-000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000-00"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("000-000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("00-0"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("00-00"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("000-0"))
            Debug.WriteLine("-----")
            ' 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("aaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("00000000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("000000"))
            Debug.WriteLine("-----")
            ' 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode("00000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("aaaaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("000000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000"))
            Debug.WriteLine("-----")
            ' 郵便 区 番号２
            Debug.WriteLine(FormatChecker.IsJpZipCode("000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("aaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode("00"))
        End Sub

        ''' <summary>郵便（区）番号（ハイフン有り）</summary>
        Protected Sub btnIsJpZipCode_H_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpZipCode_Hyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpZipCode_Hyphen--")

            ' 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000-0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("aaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("0000-00000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("0000-0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000-00000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("00-000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("00-0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000-000"))
            Debug.WriteLine("-----")
            ' 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000-00"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("aaa-aa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("0000-000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("0000-00"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000-000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("00-0"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("00-00"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000-0"))
            Debug.WriteLine("-----")
            ' 郵便 区 番号２
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("aaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("00"))
        End Sub

        ''' <summary>郵便（区）番号（ハイフン無し）</summary>
        Protected Sub btnIsJpZipCode_N_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpZipCode_NoHyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpZipCode_NoHyphen--")

            ' 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("0000000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("aaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("00000000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("000000"))
            Debug.WriteLine("-----")
            ' 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("00000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("aaaaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("000000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("0000"))
            Debug.WriteLine("-----")
            ' 郵便 区 番号２
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("aaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("00"))
        End Sub

#End Region

#Region "郵便 番号"

        ''' <summary>郵便 番号</summary>
        Protected Sub btnIsJpZipCode7_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpZipCode7(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpZipCode7--")

            ' 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode7("000-0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7("aaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7("0000-00000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7("0000-0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7("000-00000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7("00-000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7("00-0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7("000-000"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpZipCode7("0000000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7("aaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7("00000000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7("000000"))
        End Sub

        ''' <summary>郵便 番号（ハイフン有り）</summary>
        Protected Sub btnIsJpZipCode7_H_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpZipCode7_Hyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpZipCode7_Hyphen--")

            ' 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("000-0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("aaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("0000-00000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("0000-0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("000-00000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("00-000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("00-0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("000-000"))
        End Sub

        ''' <summary>郵便 番号（ハイフン無し）</summary>
        Protected Sub btnIsJpZipCode7_N_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpZipCode7_NoHyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpZipCode7_NoHyphen--")

            ' 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode7_NoHyphen("0000000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7_NoHyphen("aaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7_NoHyphen("00000000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode7_NoHyphen("000000"))
        End Sub

#End Region

#Region "郵便 区 番号"

        ''' <summary>郵便 区 番号</summary>
        Protected Sub btnIsJpZipCode5_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpZipCode5(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpZipCode5--")

            ' 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode5("000-00"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5("aaa-aa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5("0000-000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5("0000-00"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5("000-000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5("00-0"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5("00-00"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5("000-0"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpZipCode5("00000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5("aaaaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5("000000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5("0000"))
            Debug.WriteLine("-----")

            ' 郵便 区 番号２
            Debug.WriteLine(FormatChecker.IsJpZipCode5("000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5("aaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5("0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5("00"))
        End Sub

        ''' <summary>郵便 区 番号（ハイフン有り）</summary>
        Protected Sub btnIsJpZipCode5_H_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpZipCode5_Hyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpZipCode5_Hyphen--")

            ' 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("000-00"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("aaa-aa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("0000-000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("0000-00"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("000-000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("00-0"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("00-00"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("000-0"))
            Debug.WriteLine("-----")
            ' 郵便 区 番号２
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("aaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("00"))
        End Sub

        ''' <summary>郵便 区 番号（ハイフン無し）</summary>
        Protected Sub btnIsJpZipCode5_N_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpZipCode5_NoHyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpZipCode5_NoHyphen--")

            ' 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("00000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("aaaaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("000000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("0000"))
            Debug.WriteLine("-----")
            ' 郵便 区 番号２
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("aaa"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("0000"))
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("00"))
        End Sub

#End Region

#End Region

#Region "電話"

#Region "電話番号（日本）"

        ''' <summary>電話番号（日本）</summary>
        Protected Sub btnIsJpTelephoneNumber_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpTelephoneNumber(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpTelephoneNumber--")

            ' 固定電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("99999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0aaaa-a-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999-9-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999--999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999--9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999-9-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("9999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0aaa-aa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999-999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-99-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-9-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-99-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("999-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0aa-aaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-99-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("99-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0a-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("999999999999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("9999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0aaaaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099999999"))
            Debug.WriteLine("-----")

            ' 携帯電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("929-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0209-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0209-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("02-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("02-9999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("969-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0609-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0609-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("06-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("06-9999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("979-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0709-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0709-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("07-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("07-9999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("989-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0809-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0809-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("08-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("08-9999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("999-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0909-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0909-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-9999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("02099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("92999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020aaaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0209999999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("06099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("07099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("08099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09099999999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("01099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("03099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("04099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("05099999999"))
            ' → IP電話番号と同じになる。
            Debug.WriteLine("-----")

            ' IP電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("959-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0509-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0509-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("05-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("05-9999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050-999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("9999999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("010-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-9999-9999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("030-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("040-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-9999-9999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-9999-9999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-9999-9999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-9999-9999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("05099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("95999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050aaaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0509999999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("01099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("02099999999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("03099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("04099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("06099999999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("07099999999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("08099999999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09099999999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine("-----")
        End Sub
        ''' <summary>電話番号（日本, ハイフン有り）</summary>
        Protected Sub btnIsJpTelephoneNumber_H_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpTelephoneNumber_Hyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpTelephoneNumber_Hyphen--")

            ' 固定電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("99999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0aaaa-a-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999-9-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999--999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999--9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999-9-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("9999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0aaa-aa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999-999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-99-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-9-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-99-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("999-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0aa-aaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-99-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("99-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0a-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-9999-999"))
            Debug.WriteLine("-----")

            ' 携帯電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("929-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0209-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0209-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("02-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("02-9999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("969-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0609-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0609-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("06-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("06-9999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("979-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0709-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0709-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("07-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("07-9999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("989-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0809-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0809-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("08-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("08-9999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("999-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0909-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0909-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-9999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-9999-999"))
            Debug.WriteLine("-----")

            ' IP電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("050-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("959-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("050-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0509-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0509-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("050-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("050-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("05-999-999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("05-9999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("050-999-9999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("050-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("9999999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("010-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-9999-9999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("030-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("040-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-9999-9999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-9999-9999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-9999-9999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-9999-9999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine("-----")
        End Sub
        ''' <summary>電話番号（日本, ハイフン無し）</summary>
        Protected Sub btnIsJpTelephoneNumber_N_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpTelephoneNumber_NoHyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpTelephoneNumber_NoHyphen--")

            ' 固定電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("0999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("9999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("0aaaaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("09999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("099999999"))
            Debug.WriteLine("-----")

            ' 携帯電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("02099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("92999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("020aaaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("020999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("0209999999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("06099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("07099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("08099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("09099999999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("01099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("03099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("04099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("05099999999"))
            ' → IP電話番号と同じになる。
            Debug.WriteLine("-----")

            ' IP電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("05099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("95999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("050aaaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("050999999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("0509999999"))
            ' → 固定電話番号と同じになる。
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("01099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("02099999999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("03099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("04099999999"))
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("06099999999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("07099999999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("08099999999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("09099999999"))
            ' → 携帯電話番号と同じになる。
            Debug.WriteLine("-----")
        End Sub

#End Region

#Region "固定電話番号（日本）"

        ''' <summary>固定電話番号（日本）</summary>
        Protected Sub btnIsJpFixedLinePhoneNumber_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpFixedLinePhoneNumber(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpFixedLinePhoneNumber--")

            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("99999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0aaaa-a-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999-9-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999--999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999--9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999-9-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("9999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0aaa-aa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999-999-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-99-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-9-999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-99-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("999-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0aa-aaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-999-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-99-999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("99-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0a-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0-999-999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("999999999999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999999999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("9999999999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0aaaaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999999999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099999999"))
        End Sub
        ''' <summary>固定電話番号（日本, ハイフン有り）</summary>
        Protected Sub btnIsJpFixedLinePhoneNumber_H_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpFixedLinePhoneNumber_Hyphen--")

            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("99999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0aaaa-a-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999-9-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999--999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999--9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999-9-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("9999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0aaa-aa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999-999-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-99-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-9-999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-9-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-99-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("999-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0aa-aaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-999-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-99-999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-99-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("99-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0a-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0-999-999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("999999999999"))

        End Sub
        ''' <summary>固定電話番号（日本, ハイフン無し）</summary>
        Protected Sub btnIsJpFixedLinePhoneNumber_N_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpFixedLinePhoneNumber_NoHyphen--")

            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen("0999999999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen("9999999999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen("0aaaaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen("09999999999"))
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen("099999999"))
        End Sub

#End Region

#Region "携帯電話番号（日本）"

        ''' <summary>携帯電話番号（日本）</summary>
        Protected Sub btnIsJpCellularPhoneNumber_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpCellularPhoneNumber(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpCellularPhoneNumber--")

            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("929-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0209-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0209-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("02-999-999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("02-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("060-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("969-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("060-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0609-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0609-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("060-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("060-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("06-999-999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("06-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("060-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("060-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("070-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("979-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("070-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0709-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0709-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("070-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("070-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("07-999-999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("07-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("070-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("070-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("080-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("989-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("080-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0809-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0809-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("080-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("080-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("08-999-999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("08-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("080-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("080-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("090-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("999-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("090-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0909-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0909-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("090-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("090-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("09-999-999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("09-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("090-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("090-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("02099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("92999999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020aaaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020999999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0209999999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("06099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("07099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("08099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("09099999999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("01099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("03099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("04099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("05099999999"))
        End Sub
        ''' <summary>携帯電話番号（日本, ハイフン有り）</summary>
        Protected Sub btnIsJpCellularPhoneNumber_H_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpCellularPhoneNumber_Hyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpCellularPhoneNumber_Hyphen--")

            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("020-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("929-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("020-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0209-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0209-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("020-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("020-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("02-999-999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("02-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("020-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("020-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("060-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("969-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("060-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0609-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0609-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("060-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("060-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("06-999-999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("06-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("060-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("060-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("070-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("979-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("070-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0709-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0709-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("070-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("070-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("07-999-999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("07-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("070-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("070-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("080-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("989-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("080-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0809-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0809-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("080-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("080-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("08-999-999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("08-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("080-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("080-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("090-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("999-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("090-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0909-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0909-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("090-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("090-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("09-999-999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("09-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("090-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("090-9999-999"))
        End Sub
        ''' <summary>携帯電話番号（日本, ハイフン無し）</summary>
        Protected Sub btnIsJpCellularPhoneNumber_N_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpCellularPhoneNumber_NoHyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpCellularPhoneNumber_NoHyphen--")

            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("02099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("92999999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("020aaaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("020999999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("0209999999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("06099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("07099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("08099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("09099999999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("01099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("03099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("04099999999"))
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("05099999999"))
        End Sub

#End Region

#Region "IP電話番号（日本）"

        ''' <summary>IP電話番号（日本）</summary>
        Protected Sub btnIsJpIpPhoneNumber_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpIpPhoneNumber(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpIpPhoneNumber--")

            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("959-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("0509-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("0509-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("05-999-999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("05-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("9999999999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("010-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("020-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("030-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("040-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("060-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("070-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("080-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("090-9999-9999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("05099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("95999999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050aaaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050999999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("0509999999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("01099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("02099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("03099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("04099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("06099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("07099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("08099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("09099999999"))
        End Sub
        ''' <summary>IP電話番号（日本, ハイフン有り）</summary>
        Protected Sub btnIsJpIpPhoneNumber_H_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpIpPhoneNumber_Hyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpIpPhoneNumber_Hyphen--")

            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("959-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-aaaa-aaaa"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("0509-99999-99999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("0509-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-99999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-9999-99999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("05-999-999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("05-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-9999-999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("9999999999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("010-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("020-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("030-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("040-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("060-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("070-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("080-9999-9999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("090-9999-9999"))
        End Sub
        ''' <summary>IP電話番号（日本, ハイフン無し）</summary>
        Protected Sub btnIsJpIpPhoneNumber_N_Click(sender As Object, e As EventArgs)
            Me.lblStrOut.Text = FormatChecker.IsJpIpPhoneNumber_NoHyphen(Me.txtStrIn.Text).ToString()

            Debug.WriteLine("--IsJpIpPhoneNumber_NoHyphen--")

            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("05099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("95999999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050aaaaaaaa"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050999999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("0509999999"))
            Debug.WriteLine("-----")
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("01099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("02099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("03099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("04099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("06099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("07099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("08099999999"))
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("09099999999"))
        End Sub

#End Region

#End Region

#End Region

#End Region

#Region "その他"

        ''' <summary>Sessionサイズ</summary>
        Protected Sub btnSessionSize_Click(sender As Object, e As EventArgs)
            Me.lblElse.Text = MyCmnFunction.CalculateSessionSize().ToString() & "バイト"
        End Sub

        ''' <summary>Sessionサイズ(KB)</summary>
        Protected Sub btnSessionSizeKB_Click(sender As Object, e As EventArgs)
            Me.lblElse.Text = MyCmnFunction.CalculateSessionSizeKB().ToString() & "Kバイト"
        End Sub

        ''' <summary>Sessionサイズ(MB)</summary>
        Protected Sub btnSessionSizeMB_Click(sender As Object, e As EventArgs)
            Me.lblElse.Text = MyCmnFunction.CalculateSessionSizeMB().ToString() & "Mバイト"
        End Sub

        ''' <summary>偽装のテスト</summary>
        Protected Sub btnImpersonation_Click(sender As Object, e As EventArgs)
            ' クリア
            Me.lblElse.Text = ""

            Try
                ' 偽装前の実行アカウント（ASP.NET偽装はある）

                '/ 偽装起動のテスト → OK 正しく動作する。
                'Me.testCreateProcessAsImpersonationUser();

                ' 存在チェック
                Debug.WriteLine(System.Security.Principal.WindowsIdentity.GetCurrent().Name)
                Me.lblElse.Text += String.Format("偽装前（実行アカウント「{0}」）：", WindowsIdentity.GetCurrent().Name) & ResourceLoader.LoadAsString("c:\test.txt", Encoding.GetEncoding(CustomEncode.UTF_8))
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try

            '--

            Dim ret As Boolean
            'string temp;

            Dim ii As IdentityImpersonation = Nothing

            Try
                ' コードの特定部分を実行するときのみ、任意のユーザを偽装する。

                ' 偽装して
                ii = New IdentityImpersonation()
                ret = ii.ImpersonateValidUser("x", "", "x")

                '/ 偽装起動のテスト → OK 正しく動作する。
                'Me.testCreateProcessAsImpersonationUser();

                ' 存在チェック
                Me.lblElse.Text += String.Format("、偽装後（任意のユーザ「{0}」を偽装）：", WindowsIdentity.GetCurrent().Name) & ResourceLoader.LoadAsString("c:\test.txt", Encoding.GetEncoding(CustomEncode.UTF_8))
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            Finally
                ' 偽装解除
                ret = ii.UndoImpersonation()
            End Try

            '--

            If TypeOf User.Identity Is WindowsIdentity Then
                Try
                    ' コードの特定部分を実行するときのみ、認証中のユーザ (User.Identity) を偽装する。
                    ' このため、Windows認証で認証する必要がある。

                    ' 偽装して
                    ii = New IdentityImpersonation()
                    ret = ii.ImpersonateWinIdUser(DirectCast(User.Identity, WindowsIdentity))

                    '/ 偽装起動のテスト → OK 正しく動作する。
                    'Me.testCreateProcessAsImpersonationUser();

                    ' 存在チェック
                    Me.lblElse.Text += String.Format("、偽装後（認証中のユーザ「{0}」を偽装）：", WindowsIdentity.GetCurrent().Name) & ResourceLoader.LoadAsString("c:\test.txt", Encoding.GetEncoding(CustomEncode.UTF_8))
                Catch ex As Exception
                    Debug.WriteLine(ex.Message)
                Finally
                    ' 偽装解除
                    ret = ii.UndoImpersonation()
                End Try
            End If

            '--

            Try
                ' 偽装解除後、存在チェック
                Me.lblElse.Text += String.Format("、偽装解除後「{0}」：", WindowsIdentity.GetCurrent().Name) & ResourceLoader.LoadAsString("c:\test.txt", Encoding.GetEncoding(CustomEncode.UTF_8))
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' 偽装ユーザでノートパッドを起動する
        ''' （タスクマネージャで実行アカウントをチェックする）。
        ''' 注意：UIは表示されないので、タスクマネージャで確認
        ''' </summary>
        Private Sub testCreateProcessAsImpersonationUser()
            Dim ret As Boolean
            'string temp;

            Try
                Dim cmdNotepad As String = Environment.GetEnvironmentVariable("SystemRoot", EnvironmentVariableTarget.Process) & "\system32\notepad.exe"

                ' 通常起動
                Process.Start(cmdNotepad)

                ' 偽装起動
                ' ・ASP.NET偽装や、ImpersonateValidUserの偽装レベルはSecurityImpersonationなので、これに合わせる必要がある。
                ' ・独自偽装の、偽装レベルは、SecurityImpersonation、SecurityDelegationどちらでも良いが、双方を合わせる必要がある。
                ' ・実行アカウントには、「プロセス レベル トークンの置き換え」セキュリティ・ポリシー設定が必要になる。
                ret = IdentityImpersonation.CreateProcessAsImpersonationUser(cmdNotepad, "")
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try
        End Sub

        ''' <summary>その他、なんでも（カバレージ上げる用）</summary>
        Protected Sub btnElse_Click(sender As Object, e As EventArgs)
            ' TextBox9.Text = (string)BinarySerialize.BytesToObject(BinarySerialize.ObjectToBytes(TextBox8.Text));
            ' BinarySerialize.ObjectToBytes(null); // 引数例外
            ' BinarySerialize.BytesToObject(null); // 引数例外
        End Sub

#End Region
    End Class

End Namespace

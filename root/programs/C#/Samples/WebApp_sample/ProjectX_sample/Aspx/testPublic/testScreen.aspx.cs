//**********************************************************************************
//* フレームワーク・テスト画面（部品）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testScreen
//* クラス日本語名  ：共通部品テスト画面
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Security.Principal;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using Touryo.Infrastructure.Business.Str;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Util;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace ProjectX_sample.Aspx.TestPublic
{
    /// <summary>共通部品テスト画面</summary>
    public partial class testScreen : System.Web.UI.Page
    {
        #region 初期化処理

        /// <summary>初期化処理</summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.txtFilePath1.Text = GetConfigParameter.GetConfigValue("TestFilePath")
                    + @"\testWrite.txt";

                this.txtFilePath2.Text = GetConfigParameter.GetConfigValue("TestFilePath")
                    + @"\testRead.txt";

                this.txtStrIn.Text = "az、ａｚ、AZ、ＡＺ、09、０９、@%&、＠％＆、ｱア、あ亜、 　";

                //---

                //コンピュータ上に存在するすべてのタイムゾーンをComboBoxに表示
                Dictionary<string, string> dic_tzs = new Dictionary<string, string>();
                ReadOnlyCollection<TimeZoneInfo> roc_tzs = TimeZoneInfo.GetSystemTimeZones();
                foreach (TimeZoneInfo tz in roc_tzs)
                {
                    dic_tzs.Add(tz.DisplayName, tz.Id);
                }

                this.ddlTimeZone.DataSource = dic_tzs;
                this.ddlTimeZone.DataTextField = "Key";
                this.ddlTimeZone.DataValueField = "Value";
                this.ddlTimeZone.DataBind();
            }

            this.MaintainScrollPositionOnPostBack = true;
        }

        #endregion

        #region ログ出力のテスト

        /// <summary>デバッグログ</summary>
        protected void btnDebugLog_Click(object sender, EventArgs e)
        {
            // Log4Netへデバッグ・ログ出力
            LogIF.DebugLog(ddlLog.SelectedItem.Text, "わしょ～い");
        }

        /// <summary>情報ログ</summary>
        protected void btnInfoLog_Click(object sender, EventArgs e)
        {
            // Log4Netへデバッグ・ログ出力
            LogIF.InfoLog(ddlLog.SelectedItem.Text, "わしょ～い");
        }

        /// <summary>警告ログ</summary>
        protected void btnWarnLog_Click(object sender, EventArgs e)
        {
            // Log4Netへデバッグ・ログ出力
            LogIF.WarnLog(ddlLog.SelectedItem.Text, "わしょ～い");
        }

        /// <summary>（通常の）エラーログ</summary>
        protected void btnErrLog_Click(object sender, EventArgs e)
        {
            // Log4Netへデバッグ・ログ出力
            LogIF.ErrorLog(ddlLog.SelectedItem.Text, "わしょ～い");
        }

        /// <summary>致命的なエラーログ</summary>
        protected void btnFatalLog_Click(object sender, EventArgs e)
        {
            // Log4Netへデバッグ・ログ出力
            LogIF.FatalLog(ddlLog.SelectedItem.Text, "わしょ～い");
        }

        #endregion

        #region 性能測定のテスト

        /// <summary>インクリメント</summary>
        protected void btnRoop_Click(object sender, EventArgs e)
        {
            // ループ回数
            int j = int.Parse(this.txtExecCnt.Text);

            int calc = 1;

            // 性能測定オブジェクト
            PerformanceRecorder prec = new PerformanceRecorder();

            // 開始
            prec.StartsPerformanceRecord();

            // ループ処理
            for (int i = 1; i <= j; i++)
            {
                calc++;
            }

            // 終了
            this.lblPrefRec1.Text = prec.EndsPerformanceRecord();

            // 結果の出力
            this.lblPrefRec2.Text = prec.ExecTime;
            this.lblPrefRec3.Text = prec.CpuTime;
            this.lblPrefRec4.Text = prec.CpuKernelTime;
            this.lblPrefRec5.Text = prec.CpuUserTime;
        }

        /// <summary>スリープ</summary>
        protected void btnSleep_Click(object sender, EventArgs e)
        {
            // ループ回数
            int j = int.Parse(this.txtExecCnt.Text);

            // 性能測定オブジェクト
            PerformanceRecorder prec = new PerformanceRecorder();

            // 開始
            prec.StartsPerformanceRecord();

            // ループ処理
            for (int i = 1; i <= j; i++)
            {
                System.Threading.Thread.Sleep(0);
            }

            // 終了
            this.lblPrefRec1.Text = prec.EndsPerformanceRecord();

            // 結果の出力
            this.lblPrefRec2.Text = prec.ExecTime;
            this.lblPrefRec3.Text = prec.CpuTime;
            this.lblPrefRec4.Text = prec.CpuKernelTime;
            this.lblPrefRec5.Text = prec.CpuUserTime;
        }

        /// <summary>ファイルIO（書込）</summary>
        protected void btnFileIOO_Click(object sender, EventArgs e)
        {
            // ループ回数
            int j = int.Parse(this.txtExecCnt.Text);

            // 性能測定オブジェクト
            PerformanceRecorder prec = new PerformanceRecorder();

            // 開始
            prec.StartsPerformanceRecord();

            // StreamWriter
            StreamWriter sw = new StreamWriter(
                this.txtFilePath1.Text, false, Encoding.GetEncoding(CustomEncode.UTF_8));

            // ループ処理
            for (int i = 1; i <= j; i++)
            {
                sw.WriteLine("テスト てすと 手素戸");
            }

            sw.Close();

            // 終了
            this.lblPrefRec1.Text = prec.EndsPerformanceRecord();

            // 結果の出力
            this.lblPrefRec2.Text = prec.ExecTime;
            this.lblPrefRec3.Text = prec.CpuTime;
            this.lblPrefRec4.Text = prec.CpuKernelTime;
            this.lblPrefRec5.Text = prec.CpuUserTime;
        }

        /// <summary>ファイルIO（読込）</summary>
        protected void btnFileIOI_Click(object sender, EventArgs e)
        {
            // ループ回数
            int j = int.Parse(this.txtExecCnt.Text);

            // 性能測定オブジェクト
            PerformanceRecorder prec = new PerformanceRecorder();

            // 開始
            prec.StartsPerformanceRecord();

            StreamReader sr = new StreamReader(
                this.txtFilePath1.Text, Encoding.GetEncoding(CustomEncode.UTF_8));

            // ループ処理
            for (int i = 1; i <= j; i++)
            {
                this.lblFileData.Text = sr.ReadLine();
            }

            sr.Close();

            // 終了
            this.lblPrefRec1.Text = prec.EndsPerformanceRecord();

            // 結果の出力
            this.lblPrefRec2.Text = prec.ExecTime;
            this.lblPrefRec3.Text = prec.CpuTime;
            this.lblPrefRec4.Text = prec.CpuKernelTime;
            this.lblPrefRec5.Text = prec.CpuUserTime;
        }

        #endregion

        #region ファイルIO部品のテスト

        /// <summary>ファイルIO（リソースのロード１）</summary>
        protected void btnLoadResource1_Click(object sender, EventArgs e)
        {
            // チェック
            if (ResourceLoader.Exists(txtFilePath2.Text, false))
            {
                // 存在する場合

                if (this.cbxBinaryMode.Checked)
                {
                    // BinaryWriter
                    FileStream fs = new FileStream(this.txtFilePath2.Text, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);

                    // １Ｍ読み込み
                    byte[] buffer = br.ReadBytes(1024 * 1024);
                    br.Close();

                    this.txtRetFileLoad.Text =
                        CustomEncode.ByteToString(buffer, CustomEncode.UTF_8);
                }
                else
                {
                    // ResourceLoader
                    this.txtRetFileLoad.Text =
                    ResourceLoader.LoadAsString(txtFilePath2.Text,
                        Encoding.GetEncoding(CustomEncode.UTF_8));
                }
            }
            else
            {
                // 存在しない場合、例外
                ResourceLoader.Exists(txtFilePath2.Text, true);
            }
        }

        /// <summary>ファイルIO（リソースのロード２）</summary>
        protected void btnLoadResource2_Click(object sender, EventArgs e)
        {
            // チェック
            if (ResourceLoader.Exists(txtFilePath2.Text, txtFileName.Text, false))
            {
                // 存在する場合

                if (this.cbxBinaryMode.Checked)
                {
                    // BinaryWriter
                    FileStream fs = new FileStream(
                        Path.Combine(txtFilePath2.Text, txtFileName.Text),
                        FileMode.Open, FileAccess.Read);

                    BinaryReader br = new BinaryReader(fs);

                    // １Ｍ読み込み
                    byte[] buffer = br.ReadBytes(1024 * 1024);
                    br.Close();

                    this.txtRetFileLoad.Text =
                        CustomEncode.ByteToString(buffer, CustomEncode.UTF_8);
                }
                else
                {
                    // ResourceLoader
                    this.txtRetFileLoad.Text =
                    ResourceLoader.LoadAsString(txtFilePath2.Text, txtFileName.Text,
                        Encoding.GetEncoding(CustomEncode.UTF_8));
                }
            }
            else
            {
                // 存在しない場合、例外
                ResourceLoader.Exists(txtFilePath2.Text, txtFileName.Text, true);
            }
        }

        #endregion

        #region 共有情報取得部品

        /// <summary>共有情報の取得</summary>
        protected void btnGetSP_Click(object sender, EventArgs e)
        {
            // 共有情報を取得する。
            this.lblSP.Text = GetSharedProperty.GetSharedPropertyValue(this.txtSPID.Text);
        }

        #endregion

        #region Message取得部品

        /// <summary>Messageの取得</summary>
        protected void btnGetMSG_Click(object sender, EventArgs e)
        {
            // Messageを取得する。
            this.lblMSG.Text = GetMessage.GetMessageDescription(this.txtMSGID.Text);
        }

        #endregion

        #region トランザクション制御機能

        ///// <summary>トランザクション制御機能のテスト（InitDam）</summary>
        //protected void btnTxPID_Click(object sender, EventArgs e)
        //{
        //    // 引数クラスを生成
        //    // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        //    MyParameterValue myParameterValue
        //        = new MyParameterValue(
        //              "画面ID", "ButtonID",
        //              this.ddlDap.SelectedValue + "%"
        //              + this.ddlExRollback.SelectedValue + "%"
        //              + this.ddlExStatus.SelectedValue,
        //              new MyUserInfo("ユーザ名", Request.UserHostAddress));

        //    // ※ ActionTypeのフォーマット：Dap%Err%Stat%

        //    MyBaseLogic testMTC;

        //    // B層を生成
        //    if (this.cbxCnnMode.Checked)
        //    {
        //        // マルチ コネクション モード
        //        testMTC = new TestMTC_mcn();
        //    }
        //    else
        //    {
        //        // シングル コネクション モード
        //        testMTC = new TestMTC();
        //    }

        //    // 業務処理を実行
        //    MyReturnValue myReturnValue =
        //        (MyReturnValue)testMTC.DoBusinessLogic(
        //            (BaseParameterValue)myParameterValue,
        //            DbEnum.IsolationLevelEnum.User);
        //}

        ///// <summary>トランザクション制御機能のテスト（GetTransactionPatterns）</summary>
        //protected void btnTxGID_Click(object sender, EventArgs e)
        //{
        //    // 引数クラスを生成
        //    // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        //    MyType.TestParameterValue testParameterValue
        //        = new MyType.TestParameterValue(
        //              "", "画面ID", "ButtonID",
        //              this.ddlDap.SelectedValue + "%"
        //              + this.ddlExRollback.SelectedValue + "%"
        //              + this.ddlExStatus.SelectedValue,
        //              new MyUserInfo("ユーザ名", Request.UserHostAddress));

        //    // ※ ActionTypeのフォーマット：Dap

        //    // TransactionGroupIDを設定
        //    testParameterValue.Obj = this.ddlTxGpID.SelectedValue;

        //    // 業務処理を実行
        //    TestMTC_txg testMTC = new TestMTC_txg();

        //    MyReturnValue myReturnValue =
        //        (MyReturnValue)testMTC.DoBusinessLogic(
        //            (BaseParameterValue)testParameterValue,
        //            DbEnum.IsolationLevelEnum.User);

        //    this.lblTxID.Text = "";

        //    // 例外判定
        //    if (myReturnValue.ErrorFlag)
        //    {
        //        // Error Message
        //        this.lblTxID.Text = myReturnValue.ErrorMessage;
        //    }
        //    else
        //    {
        //        string[] temp1 = (string[])((MyType.TestReturnValue)myReturnValue).Obj;

        //        // TransactionPatternIDをリストする。
        //        foreach (string temp2 in temp1)
        //        {
        //            this.lblTxID.Text += temp2 + "<br/>";
        //        }
        //    }
        //}

        #endregion

        #region JIS系

        #region JIS2004

        #region 情報表示

        /// <summary>JIS2004追加文字の文字列を表示</summary>
        protected void btnDispJis2K4_Click(object sender, EventArgs e)
        {
            JIS2k4Checker jis2k4 = new JIS2k4Checker();
            this.lblJis2K4.Text = jis2k4.JIS2k4String;
        }

        /// <summary>文字列の情報を出力する。</summary>
        protected void btnDispJis2K4Info_Click(object sender, EventArgs e)
        {
            // GetStringInfoメソッドのテスト
            JIS2k4Checker jis2k4 = new JIS2k4Checker();

            int s_length;
            int si_length;
            int byte_length;

            jis2k4.GetStringInfo(this.txtJis2K4Input.Text, out s_length, out si_length, out byte_length);

            this.lblJis2K4Output.Text = "Char長:" + s_length.ToString() + "; "
                                + "文字列長" + si_length.ToString() + "; "
                                + "バイト長" + byte_length.ToString() + "; ";
        }

        #endregion

        #region サロゲート ペア文字

        /// <summary>サロゲート ペア文字のチェック１</summary>
        protected void btnCheckSPC1_Click(object sender, EventArgs e)
        {
            // CheckSurrogatesPairChar(1)メソッドのテスト
            JIS2k4Checker jis2k4 = new JIS2k4Checker();

            if (jis2k4.CheckSurrogatesPairChar(this.txtJis2K4Input.Text))
            {
                this.lblJis2K4Output.Text = "サロゲート ペア文字が含まれます。";
            }
            else
            {
                this.lblJis2K4Output.Text = "サロゲート ペア文字が含まれません。";
            }
        }

        /// <summary>サロゲート ペア文字のチェック２</summary>
        protected void btnCheckSPC2_Click(object sender, EventArgs e)
        {
            // CheckSurrogatesPairChar(2)メソッドのテスト
            JIS2k4Checker jis2k4 = new JIS2k4Checker();

            int index;

            if (jis2k4.CheckSurrogatesPairChar(this.txtJis2K4Input.Text, out index))
            {
                this.lblJis2K4Output.Text = "サロゲート ペア文字が" +
                    (index + 1).ToString() + "文字目に含まれます。";
            }
            else
            {
                this.lblJis2K4Output.Text = "サロゲート ペア文字が含まれません。";
            }
        }

        /// <summary>サロゲート ペア文字の削除</summary>
        protected void btnDelSPC_Click(object sender, EventArgs e)
        {
            // DeleteSurrogatesPairCharメソッドのテスト
            JIS2k4Checker jis2k4 = new JIS2k4Checker();
            this.lblJis2K4Output.Text
                = jis2k4.DeleteSurrogatesPairChar(this.txtJis2K4Input.Text);
        }

        /// <summary>サロゲート ペア文字の置換</summary>
        protected void btnRepSPC1_Click(object sender, EventArgs e)
        {
            // DeleteSurrogatesPairCharメソッドのテスト
            JIS2k4Checker jis2k4 = new JIS2k4Checker();

            if (this.txtJis2K4Replace.Text != "")
            {
                this.lblJis2K4Output.Text
                = jis2k4.DeleteSurrogatesPairChar(
                    this.txtJis2K4Input.Text, this.txtJis2K4Replace.Text[0]);
            }
        }

        /// <summary>サロゲート ペア文字の置換</summary>
        protected void btnRepSPC2_Click(object sender, EventArgs e)
        {
            // DeleteSurrogatesPairCharメソッドのテスト
            JIS2k4Checker jis2k4 = new JIS2k4Checker();
            this.lblJis2K4Output.Text
                = jis2k4.DeleteSurrogatesPairChar(
                    this.txtJis2K4Input.Text, this.txtJis2K4Replace.Text);
        }

        #endregion

        #region 追加文字

        /// <summary>JIS2004追加文字のチェック１</summary>
        protected void btnCheckJis2K4_1_Click(object sender, EventArgs e)
        {
            // CheckCharAddedWithJIS2k4(1)メソッドのテスト
            JIS2k4Checker jis2k4 = new JIS2k4Checker();

            if (jis2k4.CheckCharAddedWithJIS2k4(this.txtJis2K4Input.Text))
            {
                this.lblJis2K4Output.Text = "JIS2004追加文字が含まれます。";
            }
            else
            {
                this.lblJis2K4Output.Text = "JIS2004追加文字が含まれません。";
            }
        }

        /// <summary>JIS2004追加文字のチェック２</summary>
        protected void btnCheckJis2K4_2_Click(object sender, EventArgs e)
        {
            // CheckCharAddedWithJIS2k4(2)メソッドのテスト
            JIS2k4Checker jis2k4 = new JIS2k4Checker();

            int index;

            if (jis2k4.CheckCharAddedWithJIS2k4(this.txtJis2K4Input.Text, out index))
            {
                this.lblJis2K4Output.Text = "JIS2004追加文字が" +
                    (index + 1).ToString() + "文字目に含まれます。";
            }
            else
            {
                this.lblJis2K4Output.Text = "JIS2004追加文字が含まれません。";
            }
        }

        /// <summary>JIS2004追加文字の削除</summary>
        protected void btnDelJis2K4_Click(object sender, EventArgs e)
        {
            // DeleteCharAddedWithJIS2k4メソッドのテスト
            JIS2k4Checker jis2k4 = new JIS2k4Checker();
            this.lblJis2K4Output.Text
                = jis2k4.DeleteCharAddedWithJIS2k4(this.txtJis2K4Input.Text);
        }

        /// <summary>JIS2004追加文字の置換</summary>
        protected void btnRepJis2K4_1_Click(object sender, EventArgs e)
        {
            // DeleteSurrogatesPairCharメソッドのテスト
            JIS2k4Checker jis2k4 = new JIS2k4Checker();

            if (this.txtJis2K4Replace.Text != "")
            {
                this.lblJis2K4Output.Text
                    = jis2k4.DeleteCharAddedWithJIS2k4(
                        this.txtJis2K4Input.Text, this.txtJis2K4Replace.Text[0]);
            }
        }

        /// <summary>JIS2004追加文字の置換</summary>
        protected void btnRepJis2K4_2_Click(object sender, EventArgs e)
        {
            // DeleteSurrogatesPairCharメソッドのテスト
            JIS2k4Checker jis2k4 = new JIS2k4Checker();
            this.lblJis2K4Output.Text
                = jis2k4.DeleteCharAddedWithJIS2k4(
                    this.txtJis2K4Input.Text, this.txtJis2K4Replace.Text);
        }

        #endregion

        #endregion

        #region JISX0208-1983

        /// <summary>JISX0208-1983チェック</summary>
        protected void btnCheckJISX0208_1983_Click(object sender, EventArgs e)
        {
            int index;
            string ch;
            if (JISX0208_1983Checker.IsJISX0208_1983(this.txtCheckJISX0208_1983.Text, out index, out ch))
            {
                this.lblCheckJISX0208_1983.Text = "成功！";
            }
            else
            {
                this.lblCheckJISX0208_1983.Text = "失敗 index:" + index + ", ch:" + ch;
            }
        }

        #endregion

        #endregion

        #region ローカル⇔UTC対応

        /// <summary>ローカル→UTC対応</summary>
        protected void btnLocalToUtc_Click(object sender, EventArgs e)
        {
            //DateTime dt = new DateTime(this.Calendar1.SelectedDate.Ticks, DateTimeKind.Local); // 正
            DateTime dt = new DateTime(this.Calendar1.SelectedDate.Ticks, DateTimeKind.Unspecified); // 略
            //DateTime dt = new DateTime(this.Calendar1.SelectedDate.Ticks, DateTimeKind.Utc); // 不正

            if (this.cbxTimeZone.Checked)
            {
                dt = GMTMaster.ConvertLocalTimeToUtcTime35(dt, TimeZoneInfo.FindSystemTimeZoneById(this.ddlTimeZone.SelectedItem.Value));
            }
            else
            {
                dt = GMTMaster.ConvertLocalTimeToUtcTime35(dt, TimeZoneInfo.Local);
            }
            this.lblDateString.Text = dt.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }

        /// <summary>UTC→ローカル対応</summary>
        protected void btnUtcToLocal_Click(object sender, EventArgs e)
        {
            //DateTime dt = new DateTime(this.Calendar1.SelectedDate.Ticks, DateTimeKind.Utc); // 正
            DateTime dt = new DateTime(this.Calendar1.SelectedDate.Ticks, DateTimeKind.Unspecified); // 略
            //DateTime dt = new DateTime(this.Calendar1.SelectedDate.Ticks, DateTimeKind.Local); // 不正

            if (this.cbxTimeZone.Checked)
            {
                dt = GMTMaster.ConvertUtcTimeToLocalTime35(dt, TimeZoneInfo.FindSystemTimeZoneById(this.ddlTimeZone.SelectedItem.Value));
            }
            else
            {
                dt = GMTMaster.ConvertUtcTimeToLocalTime35(dt, TimeZoneInfo.Local);
            }
            this.lblDateString.Text = dt.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }

        #endregion

        #region 文字列処理

        /// <summary>出力を入力に設定</summary>
        protected void btnCopy_Click(object sender, EventArgs e)
        {
            this.txtStrIn.Text = this.lblStrOut.Text;
        }

        #region CustomEncode

        /// <summary>HTMLエンコード（サニタイジング）</summary>
        protected void btnHtmlEncode_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = CustomEncode.HtmlEncode(
                "\" id=\"txtXXXXX\" />"
                + "<script type=\"text/javascript\">alert(\"XSS!!!\")</script>"
                + "<input name=\"txtXXXXX\" type=\"text\" value=\"");
        }

        /// <summary>URLエンコード</summary>
        protected void btnUrlEncode_Click(object sender, EventArgs e)
        {
            // グーグルで「&」を検索したい。
            Response.Redirect(
                "http://www.google.co.jp/search?hl=ja&q="
                + CustomEncode.UrlEncode("&"));

            //// ASP.NETでは？
            //Response.Redirect(
            //    "testScreen.aspx?"
            //    + CustomEncode.UrlEncode("&name&")
            //    + "="
            //    + CustomEncode.UrlEncode("&value&"));
        }

        #endregion

        #region StringConverter

        /// <summary>半角へ</summary>
        protected void btnToHankaku_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringConverter.ToHankaku(this.txtStrIn.Text);

        }

        /// <summary>全角へ</summary>
        protected void btnToZenkaku_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringConverter.ToZenkaku(this.txtStrIn.Text);
        }

        /// <summary>片仮名へ</summary>
        protected void btnToKatakana_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringConverter.ToKatakana(this.txtStrIn.Text);
        }

        /// <summary>平仮名へ</summary>
        protected void btnToHiragana_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringConverter.ToHiragana(this.txtStrIn.Text);
        }

        #endregion

        #region StringChecker

        #region 数字

        /// <summary>数字チェック</summary>
        protected void btnIsNumbers_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringChecker.IsNumbers(this.txtStrIn.Text).ToString();
        }
        /// <summary>数字（半角）チェック</summary>
        protected void btnIsNumbers_Hankaku_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringChecker.IsNumbers_Hankaku(this.txtStrIn.Text).ToString();
        }
        /// <summary>数字（全角）チェック</summary>
        protected void btnIsNumbers_Zenkaku_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringChecker.IsNumbers_Zenkaku(this.txtStrIn.Text).ToString();
        }

        #endregion

        #region 英字

        /// <summary>英字チェック</summary>
        protected void btnIsAlphabet_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringChecker.IsAlphabet(this.txtStrIn.Text).ToString();
        }
        /// <summary>英字（全角）チェック</summary>
        protected void btnIsAlphabet_Hankaku_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringChecker.IsAlphabet_Hankaku(this.txtStrIn.Text).ToString();
        }
        /// <summary>英字（半角）チェック</summary>
        protected void btnIsAlphabet_Zenkaku_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringChecker.IsAlphabet_Zenkaku(this.txtStrIn.Text).ToString();
        }


        #endregion

        #region 日本語

        /// <summary>平仮名チェック</summary>
        protected void btnIsHiragana_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringChecker.IsHiragana(this.txtStrIn.Text).ToString();
        }
        /// <summary>平仮名（全角）チェック</summary>
        protected void btnIsKatakana_Zenkaku_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringChecker.IsKatakana_Zenkaku(this.txtStrIn.Text).ToString();
        }
        /// <summary>平仮名（半角）チェック</summary>
        protected void btnIsKatakana_Hankaku_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringChecker.IsKatakana_Hankaku(this.txtStrIn.Text).ToString();
        }
        /// <summary>漢字チェック</summary>
        protected void btnIsKanji_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringChecker.IsKanji(this.txtStrIn.Text).ToString();
        }

        #endregion

        #region SJIS

        /// <summary>SJISチェック</summary>
        protected void btnIsShift_Jis_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringChecker.IsShift_Jis(this.txtStrIn.Text).ToString();
        }

        /// <summary>SJIS全角チェック</summary>
        protected void btnIsShift_Jis_Zenkaku_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringChecker.IsShift_Jis_Zenkaku(this.txtStrIn.Text).ToString();
        }
        /// <summary>SJIS半角チェック</summary>
        protected void btnIsShift_Jis_Hankaku_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = StringChecker.IsShift_Jis_Hankaku(this.txtStrIn.Text).ToString();
        }

        #endregion

        #endregion

        #region FormatConverter

        #region 和暦・西暦

        /// <summary>和暦→西暦</summary>
        protected void btnSeirekiToWareki_Click(object sender, EventArgs e)
        {
            string ret = "";

            // 基本バージョン
            ret = FormatConverter.SeirekiToWareki(
                DateTime.Parse("1977/4/24"), "ggy年M月d日（ddd）");
            Debug.WriteLine(ret);

            // パターンだけ時間あり
            ret = FormatConverter.SeirekiToWareki(
                DateTime.Parse("1977/4/24"), "ggy年M月d日（ddd）H:m:s");
            Debug.WriteLine(ret);

            // DateTimeだけ時間あり
            ret = FormatConverter.SeirekiToWareki(
                DateTime.Parse("1977/4/24 19:15:12"), "ggy年M月d日（ddd）");
            Debug.WriteLine(ret);

            // 時間情報込みバージョン（24時間表記）
            ret = FormatConverter.SeirekiToWareki(
                DateTime.Parse("1977/4/24 19:15:12"), "ggy年M月d日（ddd）H:m:s");
            Debug.WriteLine(ret);

            // 時間情報込みバージョン（12時間表記）
            ret = FormatConverter.SeirekiToWareki(
                DateTime.Parse("1977/4/24 19:15:12"), "ggy年M月d日（ddd）tt h:m:s");
            Debug.WriteLine(ret);

            // 上記のパターン文字列の変更版
            ret = FormatConverter.SeirekiToWareki(
                DateTime.Parse("1992/2/6 1:1:1"), "ggyy年MM月dd日 dddd HH:mm:ss"); // 0埋め2桁
            Debug.WriteLine(ret);

            ret = FormatConverter.SeirekiToWareki(
                DateTime.Parse("1992/2/6 13:1:1"), "ggyy年MM月dd日 dddd tt hh:mm:ss"); // 0埋め2桁
            Debug.WriteLine(ret);
        }

        /// <summary>西暦→和暦</summary>
        protected void btnWarekiToSeireki_Click(object sender, EventArgs e)
        {
            string ret = "";

            // 基本バージョン
            ret = FormatConverter.WarekiToSeireki(
                "昭和52年4月24日（日）", "ggy年M月d日（ddd）").ToString();
            Debug.WriteLine(ret);

            //// パターンだけ時間あり
            //ret = FormatConverter.WarekiToSeireki(
            //    "昭和52年4月24日（日）", "ggy年M月d日（ddd）H:m:s").ToString();

            //// 和暦文字列だけ時間あり
            //ret = FormatConverter.WarekiToSeireki(
            //    "昭和52年4月24日（日）12:12:12", "ggy年M月d日（ddd）").ToString();

            // 時間情報込みバージョン（24時間表記）
            ret = FormatConverter.WarekiToSeireki(
                "昭和52年4月24日（日）19:15:12", "ggy年M月d日（ddd）H:m:s").ToString();
            Debug.WriteLine(ret);

            // 時間情報込みバージョン（12時間表記）
            ret = FormatConverter.WarekiToSeireki(
                "昭和52年4月24日（日）午後 7:15:12", "ggy年M月d日（ddd）tt h:m:s").ToString();
            Debug.WriteLine(ret);

            // 上記のパターン文字列の変更版
            ret = FormatConverter.WarekiToSeireki(
                "平成04年02月06日 木曜日 01:01:01", "ggyy年MM月dd日 dddd HH:mm:ss").ToString(); // 0埋め2桁
            Debug.WriteLine(ret);

            ret = FormatConverter.WarekiToSeireki(
                "平成04年02月06日 木曜日 午後 01:01:01", "ggyy年MM月dd日 dddd tt hh:mm:ss").ToString(); // 0埋め2桁
            Debug.WriteLine(ret);
        }

        #endregion

        #region 桁区切り

        /// <summary>３桁区切り</summary>
        protected void btnAddFigure3_Click(object sender, EventArgs e)
        {
            Debug.WriteLine(FormatConverter.AddFigure3(12345));
            Debug.WriteLine(FormatConverter.AddFigure3(123456789));
            Debug.WriteLine(FormatConverter.AddFigure3(123.45));
            Debug.WriteLine(FormatConverter.AddFigure3(12345.6789));
            Debug.WriteLine(FormatConverter.AddFigure3(-12345));
            Debug.WriteLine(FormatConverter.AddFigure3(-123456789));
            Debug.WriteLine(FormatConverter.AddFigure3(-123.45));
            Debug.WriteLine(FormatConverter.AddFigure3(-12345.6789));

            Debug.WriteLine(FormatConverter.AddFigure3("12345"));
            Debug.WriteLine(FormatConverter.AddFigure3("123456789"));
            Debug.WriteLine(FormatConverter.AddFigure3("123.45"));
            Debug.WriteLine(FormatConverter.AddFigure3("12345.6789"));
            Debug.WriteLine(FormatConverter.AddFigure3("-12345"));
            Debug.WriteLine(FormatConverter.AddFigure3("-123456789"));
            Debug.WriteLine(FormatConverter.AddFigure3("-123.45"));
            Debug.WriteLine(FormatConverter.AddFigure3("-12345.6789"));

            Debug.WriteLine(FormatConverter.AddFigure3("abcdefghijklnm"));
        }

        /// <summary>４桁区切り</summary>
        protected void btnAddFigure4_Click(object sender, EventArgs e)
        {
            Debug.WriteLine(FormatConverter.AddFigure4(12345));
            Debug.WriteLine(FormatConverter.AddFigure4(123456789));
            Debug.WriteLine(FormatConverter.AddFigure4(123.45));
            Debug.WriteLine(FormatConverter.AddFigure4(12345.6789));
            Debug.WriteLine(FormatConverter.AddFigure4(-12345));
            Debug.WriteLine(FormatConverter.AddFigure4(-123456789));
            Debug.WriteLine(FormatConverter.AddFigure4(-123.45));
            Debug.WriteLine(FormatConverter.AddFigure4(-12345.6789));

            Debug.WriteLine(FormatConverter.AddFigure4("12345"));
            Debug.WriteLine(FormatConverter.AddFigure4("123456789"));
            Debug.WriteLine(FormatConverter.AddFigure4("123.45"));
            Debug.WriteLine(FormatConverter.AddFigure4("12345.6789"));
            Debug.WriteLine(FormatConverter.AddFigure4("-12345"));
            Debug.WriteLine(FormatConverter.AddFigure4("-123456789"));
            Debug.WriteLine(FormatConverter.AddFigure4("-123.45"));
            Debug.WriteLine(FormatConverter.AddFigure4("-12345.6789"));

            Debug.WriteLine(FormatConverter.AddFigure4("abcdefghijklnm"));
        }

        #endregion

        #region サプレス

        /// <summary>サプレス</summary>
        protected void btnSuppress_Click(object sender, EventArgs e)
        {
            Debug.WriteLine(FormatConverter.Suppress("", 10, '＠'));
            //Debug.WriteLine(FormatConverter.Suppress("123456789", -1, '＠'));
            Debug.WriteLine(FormatConverter.Suppress("123456789", 0, '＠'));
            Debug.WriteLine(FormatConverter.Suppress("123456789", 1, '＠'));
            Debug.WriteLine(FormatConverter.Suppress("123456789", 5, '＠'));
            Debug.WriteLine(FormatConverter.Suppress("123456789", 9, '＠'));
            Debug.WriteLine(FormatConverter.Suppress("123456789", 10, '＠'));
            Debug.WriteLine(FormatConverter.Suppress("123456789", 11, '＠'));
            Debug.WriteLine(FormatConverter.Suppress("123456789", 20, '＠'));

            Debug.WriteLine(FormatConverter.Suppress("", 10, '0'));
            //Debug.WriteLine(FormatConverter.Suppress("abcdefg", -1, '0'));
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 0, '0'));
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 1, '0'));
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 5, '0'));
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 7, '0'));
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 8, '0'));
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 10, '0'));
            Debug.WriteLine(FormatConverter.Suppress("abcdefg", 20, '0'));
        }

        #endregion

        #endregion

        #region FormatChecker

        #region 郵便

        #region 郵便（区）番号

        /// <summary>郵便（区）番号</summary>
        protected void btnIsJpZipCode_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpZipCode(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpZipCode--");

            // 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode("000-0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("aaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000-00000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000-0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("000-00000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("00-000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("00-0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("000-000"));
            Debug.WriteLine("-----");
            // 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode("000-00"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("aaa-aa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000-000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000-00"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("000-000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("00-0"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("00-00"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("000-0"));
            Debug.WriteLine("-----");
            // 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("aaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("00000000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("000000"));
            Debug.WriteLine("-----");
            // 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode("00000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("aaaaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("000000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000"));
            Debug.WriteLine("-----");
            // 郵便 区 番号２
            Debug.WriteLine(FormatChecker.IsJpZipCode("000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("aaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode("00"));
        }

        /// <summary>郵便（区）番号（ハイフン有り）</summary>
        protected void btnIsJpZipCode_H_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpZipCode_Hyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpZipCode_Hyphen--");

            // 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000-0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("aaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("0000-00000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("0000-0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000-00000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("00-000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("00-0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000-000"));
            Debug.WriteLine("-----");
            // 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000-00"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("aaa-aa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("0000-000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("0000-00"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000-000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("00-0"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("00-00"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000-0"));
            Debug.WriteLine("-----");
            // 郵便 区 番号２
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("aaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_Hyphen("00"));
        }

        /// <summary>郵便（区）番号（ハイフン無し）</summary>
        protected void btnIsJpZipCode_N_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpZipCode_NoHyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpZipCode_NoHyphen--");

            // 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("0000000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("aaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("00000000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("000000"));
            Debug.WriteLine("-----");
            // 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("00000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("aaaaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("000000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("0000"));
            Debug.WriteLine("-----");
            // 郵便 区 番号２
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("aaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode_NoHyphen("00"));
        }

        #endregion

        #region 郵便 番号

        /// <summary>郵便 番号</summary>
        protected void btnIsJpZipCode7_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpZipCode7(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpZipCode7--");

            // 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode7("000-0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7("aaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7("0000-00000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7("0000-0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7("000-00000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7("00-000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7("00-0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7("000-000"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpZipCode7("0000000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7("aaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7("00000000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7("000000"));
        }

        /// <summary>郵便 番号（ハイフン有り）</summary>
        protected void btnIsJpZipCode7_H_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpZipCode7_Hyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpZipCode7_Hyphen--");

            // 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("000-0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("aaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("0000-00000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("0000-0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("000-00000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("00-000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("00-0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7_Hyphen("000-000"));
        }

        /// <summary>郵便 番号（ハイフン無し）</summary>
        protected void btnIsJpZipCode7_N_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpZipCode7_NoHyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpZipCode7_NoHyphen--");

            // 郵便番号
            Debug.WriteLine(FormatChecker.IsJpZipCode7_NoHyphen("0000000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7_NoHyphen("aaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7_NoHyphen("00000000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode7_NoHyphen("000000"));
        }

        #endregion

        #region 郵便 区 番号

        /// <summary>郵便 区 番号</summary>
        protected void btnIsJpZipCode5_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpZipCode5(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpZipCode5--");

            // 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode5("000-00"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5("aaa-aa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5("0000-000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5("0000-00"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5("000-000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5("00-0"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5("00-00"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5("000-0"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpZipCode5("00000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5("aaaaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5("000000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5("0000"));
            Debug.WriteLine("-----");

            // 郵便 区 番号２
            Debug.WriteLine(FormatChecker.IsJpZipCode5("000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5("aaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5("0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5("00"));
        }

        /// <summary>郵便 区 番号（ハイフン有り）</summary>
        protected void btnIsJpZipCode5_H_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpZipCode5_Hyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpZipCode5_Hyphen--");

            // 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("000-00"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("aaa-aa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("0000-000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("0000-00"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("000-000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("00-0"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("00-00"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("000-0"));
            Debug.WriteLine("-----");
            // 郵便 区 番号２
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("aaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_Hyphen("00"));
        }

        /// <summary>郵便 区 番号（ハイフン無し）</summary>
        protected void btnIsJpZipCode5_N_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpZipCode5_NoHyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpZipCode5_NoHyphen--");

            // 郵便 区 番号１
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("00000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("aaaaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("000000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("0000"));
            Debug.WriteLine("-----");
            // 郵便 区 番号２
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("aaa"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("0000"));
            Debug.WriteLine(FormatChecker.IsJpZipCode5_NoHyphen("00"));
        }

        #endregion

        #endregion

        #region 電話

        #region 電話番号（日本）

        /// <summary>電話番号（日本）</summary>
        protected void btnIsJpTelephoneNumber_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpTelephoneNumber(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpTelephoneNumber--");

            // 固定電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("99999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0aaaa-a-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999-9-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999--999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999--9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999-9-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("9999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0aaa-aa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999-999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-99-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-9-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-99-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("999-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0aa-aaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-99-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("99-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0a-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("999999999999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("9999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0aaaaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("099999999"));
            Debug.WriteLine("-----");

            // 携帯電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("929-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0209-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0209-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("02-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("02-9999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("969-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0609-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0609-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("06-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("06-9999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("979-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0709-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0709-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("07-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("07-9999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("989-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0809-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0809-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("08-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("08-9999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("999-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0909-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0909-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09-9999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("02099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("92999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020aaaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0209999999"));// → 固定電話番号と同じになる。
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("06099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("07099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("08099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09099999999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("01099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("03099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("04099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("05099999999"));// → IP電話番号と同じになる。
            Debug.WriteLine("-----");

            // IP電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("959-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0509-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0509-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("05-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("05-9999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050-999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("9999999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("010-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("020-9999-9999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("030-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("040-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("060-9999-9999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("070-9999-9999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("080-9999-9999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("090-9999-9999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("05099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("95999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050aaaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("050999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("0509999999"));// → 固定電話番号と同じになる。
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("01099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("02099999999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("03099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("04099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("06099999999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("07099999999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("08099999999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber("09099999999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine("-----");
        }
        /// <summary>電話番号（日本, ハイフン有り）</summary>
        protected void btnIsJpTelephoneNumber_H_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpTelephoneNumber_Hyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpTelephoneNumber_Hyphen--");

            // 固定電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("99999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0aaaa-a-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999-9-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999--999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999--9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999-9-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("9999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0aaa-aa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999-999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-99-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-9-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-99-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("999-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0aa-aaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0999-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-99-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("99-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0a-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("099-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-9999-999"));
            Debug.WriteLine("-----");

            // 携帯電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("929-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0209-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0209-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("02-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("02-9999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("969-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0609-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0609-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("06-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("06-9999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("979-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0709-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0709-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("07-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("07-9999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("989-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0809-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0809-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("08-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("08-9999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("999-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0909-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0909-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("09-9999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-9999-999"));
            Debug.WriteLine("-----");

            // IP電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("050-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("959-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("050-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0509-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("0509-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("050-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("050-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("05-999-999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("05-9999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("050-999-9999"));// → 固定電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("050-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("9999999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("010-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("020-9999-9999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("030-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("040-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("060-9999-9999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("070-9999-9999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("080-9999-9999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_Hyphen("090-9999-9999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine("-----");
        }
        /// <summary>電話番号（日本, ハイフン無し）</summary>
        protected void btnIsJpTelephoneNumber_N_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpTelephoneNumber_NoHyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpTelephoneNumber_NoHyphen--");

            // 固定電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("0999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("9999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("0aaaaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("09999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("099999999"));
            Debug.WriteLine("-----");

            // 携帯電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("02099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("92999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("020aaaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("020999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("0209999999"));// → 固定電話番号と同じになる。
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("06099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("07099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("08099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("09099999999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("01099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("03099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("04099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("05099999999"));// → IP電話番号と同じになる。
            Debug.WriteLine("-----");

            // IP電話番号（日本）
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("05099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("95999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("050aaaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("050999999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("0509999999"));// → 固定電話番号と同じになる。
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("01099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("02099999999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("03099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("04099999999"));
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("06099999999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("07099999999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("08099999999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine(FormatChecker.IsJpTelephoneNumber_NoHyphen("09099999999"));// → 携帯電話番号と同じになる。
            Debug.WriteLine("-----");
        }

        #endregion

        #region 固定電話番号（日本）

        /// <summary>固定電話番号（日本）</summary>
        protected void btnIsJpFixedLinePhoneNumber_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpFixedLinePhoneNumber(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpFixedLinePhoneNumber--");

            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("99999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0aaaa-a-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999-9-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999--999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999--9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999-9-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("9999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0aaa-aa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999-999-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-99-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-9-999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-99-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("999-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0aa-aaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-999-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-99-999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("99-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0a-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0-999-999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("999999999999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0999999999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("9999999999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("0aaaaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("09999999999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber("099999999"));
        }
        /// <summary>固定電話番号（日本, ハイフン有り）</summary>
        protected void btnIsJpFixedLinePhoneNumber_H_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpFixedLinePhoneNumber_Hyphen--");

            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("99999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0aaaa-a-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999-9-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999--999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999--9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999-9-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("9999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0aaa-aa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999-999-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09999-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-99-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-9-999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-9-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-99-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("999-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0aa-aaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0999-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-999-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-99-999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-99-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("99-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0a-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("099-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0-999-999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("0-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("09-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen("999999999999"));

        }
        /// <summary>固定電話番号（日本, ハイフン無し）</summary>
        protected void btnIsJpFixedLinePhoneNumber_N_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpFixedLinePhoneNumber_NoHyphen--");

            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen("0999999999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen("9999999999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen("0aaaaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen("09999999999"));
            Debug.WriteLine(FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen("099999999"));
        }

        #endregion

        #region 携帯電話番号（日本）

        /// <summary>携帯電話番号（日本）</summary>
        protected void btnIsJpCellularPhoneNumber_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpCellularPhoneNumber(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpCellularPhoneNumber--");

            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("929-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0209-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0209-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("02-999-999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("02-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("060-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("969-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("060-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0609-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0609-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("060-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("060-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("06-999-999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("06-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("060-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("060-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("070-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("979-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("070-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0709-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0709-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("070-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("070-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("07-999-999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("07-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("070-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("070-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("080-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("989-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("080-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0809-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0809-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("080-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("080-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("08-999-999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("08-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("080-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("080-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("090-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("999-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("090-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0909-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0909-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("090-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("090-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("09-999-999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("09-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("090-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("090-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("02099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("92999999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020aaaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("020999999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("0209999999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("06099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("07099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("08099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("09099999999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("01099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("03099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("04099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber("05099999999"));
        }
        /// <summary>携帯電話番号（日本, ハイフン有り）</summary>
        protected void btnIsJpCellularPhoneNumber_H_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpCellularPhoneNumber_Hyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpCellularPhoneNumber_Hyphen--");

            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("020-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("929-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("020-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0209-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0209-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("020-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("020-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("02-999-999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("02-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("020-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("020-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("060-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("969-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("060-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0609-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0609-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("060-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("060-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("06-999-999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("06-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("060-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("060-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("070-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("979-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("070-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0709-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0709-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("070-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("070-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("07-999-999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("07-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("070-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("070-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("080-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("989-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("080-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0809-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0809-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("080-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("080-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("08-999-999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("08-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("080-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("080-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("090-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("999-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("090-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0909-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("0909-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("090-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("090-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("09-999-999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("09-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("090-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_Hyphen("090-9999-999"));
        }
        /// <summary>携帯電話番号（日本, ハイフン無し）</summary>
        protected void btnIsJpCellularPhoneNumber_N_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpCellularPhoneNumber_NoHyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpCellularPhoneNumber_NoHyphen--");

            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("02099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("92999999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("020aaaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("020999999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("0209999999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("06099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("07099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("08099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("09099999999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("01099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("03099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("04099999999"));
            Debug.WriteLine(FormatChecker.IsJpCellularPhoneNumber_NoHyphen("05099999999"));
        }

        #endregion

        #region IP電話番号（日本）

        /// <summary>IP電話番号（日本）</summary>
        protected void btnIsJpIpPhoneNumber_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpIpPhoneNumber(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpIpPhoneNumber--");

            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("959-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("0509-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("0509-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("05-999-999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("05-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("9999999999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("010-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("020-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("030-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("040-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("060-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("070-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("080-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("090-9999-9999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("05099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("95999999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050aaaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050999999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("0509999999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("01099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("02099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("03099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("04099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("06099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("07099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("08099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("09099999999"));
        }
        /// <summary>IP電話番号（日本, ハイフン有り）</summary>
        protected void btnIsJpIpPhoneNumber_H_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpIpPhoneNumber_Hyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpIpPhoneNumber_Hyphen--");

            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("959-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-aaaa-aaaa"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("0509-99999-99999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("0509-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-99999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-9999-99999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("05-999-999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("05-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050-9999-999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("9999999999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("010-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("020-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("030-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("040-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("060-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("070-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("080-9999-9999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("090-9999-9999"));
        }
        /// <summary>IP電話番号（日本, ハイフン無し）</summary>
        protected void btnIsJpIpPhoneNumber_N_Click(object sender, EventArgs e)
        {
            this.lblStrOut.Text = FormatChecker.IsJpIpPhoneNumber_NoHyphen(this.txtStrIn.Text).ToString();

            Debug.WriteLine("--IsJpIpPhoneNumber_NoHyphen--");

            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("05099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("95999999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050aaaaaaaa"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("050999999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("0509999999"));
            Debug.WriteLine("-----");
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("01099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("02099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("03099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("04099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("06099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("07099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("08099999999"));
            Debug.WriteLine(FormatChecker.IsJpIpPhoneNumber("09099999999"));
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region その他

        /// <summary>Sessionサイズ</summary>
        protected void btnSessionSize_Click(object sender, EventArgs e)
        {
            this.lblElse.Text = MyCmnFunction.CalculateSessionSize().ToString() + "バイト";
        }

        /// <summary>Sessionサイズ(KB)</summary>
        protected void btnSessionSizeKB_Click(object sender, EventArgs e)
        {
            this.lblElse.Text = MyCmnFunction.CalculateSessionSizeKB().ToString() + "Kバイト";
        }

        /// <summary>Sessionサイズ(MB)</summary>
        protected void btnSessionSizeMB_Click(object sender, EventArgs e)
        {
            this.lblElse.Text = MyCmnFunction.CalculateSessionSizeMB().ToString() + "Mバイト";
        }

        /// <summary>偽装のテスト</summary>
        protected void btnImpersonation_Click(object sender, EventArgs e)
        {
            // クリア
            this.lblElse.Text = "";

            try
            {
                // 偽装前の実行アカウント（ASP.NET偽装はある）

                //// 偽装起動のテスト → OK 正しく動作する。
                //this.testCreateProcessAsImpersonationUser();

                // 存在チェック
                Debug.WriteLine(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
                this.lblElse.Text
                    += string.Format("偽装前（実行アカウント「{0}」）：", WindowsIdentity.GetCurrent().Name)
                    + ResourceLoader.LoadAsString(@"c:\test.txt", Encoding.GetEncoding(CustomEncode.UTF_8));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            //--

            bool ret;
            //string temp;

            IdentityImpersonation ii = null;

            try
            {
                // コードの特定部分を実行するときのみ、任意のユーザを偽装する。

                // 偽装して
                ii = new IdentityImpersonation();
                ret = ii.ImpersonateValidUser("x", "", "x");

                //// 偽装起動のテスト → OK 正しく動作する。
                //this.testCreateProcessAsImpersonationUser();

                // 存在チェック
                this.lblElse.Text
                    += string.Format("、偽装後（任意のユーザ「{0}」を偽装）：", WindowsIdentity.GetCurrent().Name)
                    + ResourceLoader.LoadAsString(@"c:\test.txt", Encoding.GetEncoding(CustomEncode.UTF_8));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                // 偽装解除
                ret = ii.UndoImpersonation();
            }

            //--

            if (User.Identity is WindowsIdentity)
            {
                try
                {
                    // コードの特定部分を実行するときのみ、認証中のユーザ (User.Identity) を偽装する。
                    // このため、Windows認証で認証する必要がある。

                    // 偽装して
                    ii = new IdentityImpersonation();
                    ret = ii.ImpersonateWinIdUser((WindowsIdentity)User.Identity);

                    //// 偽装起動のテスト → OK 正しく動作する。
                    //this.testCreateProcessAsImpersonationUser();

                    // 存在チェック
                    this.lblElse.Text
                        += string.Format("、偽装後（認証中のユーザ「{0}」を偽装）：", WindowsIdentity.GetCurrent().Name)
                        + ResourceLoader.LoadAsString(@"c:\test.txt", Encoding.GetEncoding(CustomEncode.UTF_8));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    // 偽装解除
                    ret = ii.UndoImpersonation();
                }
            }

            //--

            try
            {
                // 偽装解除後、存在チェック
                this.lblElse.Text
                    += string.Format("、偽装解除後「{0}」：", WindowsIdentity.GetCurrent().Name)
                    + ResourceLoader.LoadAsString(@"c:\test.txt", Encoding.GetEncoding(CustomEncode.UTF_8));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 偽装ユーザでノートパッドを起動する
        /// （タスクマネージャで実行アカウントをチェックする）。
        /// 注意：UIは表示されないので、タスクマネージャで確認
        /// </summary>
        private void testCreateProcessAsImpersonationUser()
        {
            bool ret;
            //string temp;

            try
            {
                string cmdNotepad = Environment.GetEnvironmentVariable(
                    "SystemRoot", EnvironmentVariableTarget.Process) + @"\system32\notepad.exe";

                // 通常起動
                Process.Start(cmdNotepad);

                // 偽装起動
                // ・ASP.NET偽装や、ImpersonateValidUserの偽装レベルはSecurityImpersonationなので、これに合わせる必要がある。
                // ・独自偽装の、偽装レベルは、SecurityImpersonation、SecurityDelegationどちらでも良いが、双方を合わせる必要がある。
                // ・実行アカウントには、「プロセス レベル トークンの置き換え」セキュリティ・ポリシー設定が必要になる。
                ret = IdentityImpersonation.CreateProcessAsImpersonationUser(cmdNotepad, "");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>その他、なんでも（カバレージ上げる用）</summary>
        protected void btnElse_Click(object sender, EventArgs e)
        {
            // TextBox9.Text = (string)BinarySerialize.BytesToObject(BinarySerialize.ObjectToBytes(TextBox8.Text));
            // BinarySerialize.ObjectToBytes(null); // 引数例外
            // BinarySerialize.BytesToObject(null); // 引数例外
        }

        #endregion
    }
    
}
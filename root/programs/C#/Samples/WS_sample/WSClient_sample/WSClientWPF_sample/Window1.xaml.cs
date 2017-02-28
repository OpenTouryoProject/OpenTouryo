//**********************************************************************************
//* ３層型 サンプル アプリ
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Window1
//* クラス日本語名  ：サンプル アプリ画面
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using WSIFType_sample;

using System;
using System.Data;
using System.Threading;
using System.Windows.Controls;
using System.Windows;

using Touryo.Infrastructure.Business.RichClient.Asynchronous;
using Touryo.Infrastructure.Business.RichClient.Util;
using Touryo.Infrastructure.Business.Util;

using Touryo.Infrastructure.Framework.RichClient.Asynchronous;
using Touryo.Infrastructure.Framework.Transmission;
using Touryo.Infrastructure.Framework.Util;

namespace WSClientWPF_sample
{
    /// <summary>Window1.xaml の相互作用（サンプル アプリ画面）</summary>
    public partial class Window1 : Window
    {
        /// <summary>ユーザ情報</summary>
        MyUserInfo myUserInfo;

        /// <summary>呼出し制御部品</summary>
        CallController CallCtrl;

        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public Window1()
        {
            InitializeComponent();
        }

        /// <summary>ロード イベント</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ddlDap
            this.ddlDap.Items.Add(new ComboBoxItem("SQL Server / SQL Client", "SQL"));
            this.ddlDap.Items.Add(new ComboBoxItem("Multi-DB / OLEDB.NET", "OLE"));
            this.ddlDap.Items.Add(new ComboBoxItem("Multi-DB / ODCB.NET", "ODB"));
            this.ddlDap.Items.Add(new ComboBoxItem("Oracle / ODP.NET", "ODP"));
            this.ddlDap.Items.Add(new ComboBoxItem("DB2 / DB2.NET", "DB2"));
            this.ddlDap.Items.Add(new ComboBoxItem("HiRDB / HiRDB-DP", "HIR"));
            this.ddlDap.Items.Add(new ComboBoxItem("MySQL Cnn/NET", "MCN"));
            this.ddlDap.Items.Add(new ComboBoxItem("PostgreSQL / Npgsql", "NPS"));
            this.ddlDap.SelectedIndex = 0;

            // ddlMode1
            this.ddlMode1.Items.Add(new ComboBoxItem("個別Ｄａｏ", "individual"));
            this.ddlMode1.Items.Add(new ComboBoxItem("共通Ｄａｏ", "common"));
            this.ddlMode1.Items.Add(new ComboBoxItem("自動生成Ｄａｏ（更新のみ）", "generate"));
            this.ddlMode1.SelectedIndex = 0;

            // ddlMode2
            this.ddlMode2.Items.Add(new ComboBoxItem("静的クエリ", "static"));
            this.ddlMode2.Items.Add(new ComboBoxItem("動的クエリ", "dynamic"));
            this.ddlMode2.SelectedIndex = 0;

            // ddlIso
            this.ddlIso.Items.Add(new ComboBoxItem("ノットコネクト", "NC"));
            this.ddlIso.Items.Add(new ComboBoxItem("ノートランザクション", "NT"));
            this.ddlIso.Items.Add(new ComboBoxItem("ダーティリード", "RU"));
            this.ddlIso.Items.Add(new ComboBoxItem("リードコミット", "RC"));
            this.ddlIso.Items.Add(new ComboBoxItem("リピータブルリード", "RR"));
            this.ddlIso.Items.Add(new ComboBoxItem("シリアライザブル", "SZ"));
            this.ddlIso.Items.Add(new ComboBoxItem("スナップショット", "SS"));
            this.ddlIso.Items.Add(new ComboBoxItem("デフォルト", "DF"));
            this.ddlIso.SelectedIndex = 1;

            // WSでは使用しない（設定できないので）。
            this.ddlIso.IsEnabled = false;

            // ddlExRollback
            this.ddlExRollback.Items.Add(new ComboBoxItem("正常時", "-"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("業務例外", "Business"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("システム例外", "System"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("その他、一般的な例外", "Other"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("業務例外への振替", "Other-Business"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("システム例外への振替", "Other-System"));
            this.ddlExRollback.SelectedIndex = 0;

            // ddlTransmission
            this.ddlTransmission.Items.Add(new ComboBoxItem("Webサービス呼出", "testWebService"));
            this.ddlTransmission.Items.Add(new ComboBoxItem("インプロセス呼出", "testInProcess"));
            this.ddlTransmission.SelectedIndex = 0;

            // ddlOrderColumn
            this.ddlOrderColumn.Items.Add(new ComboBoxItem("c1", "c1"));
            this.ddlOrderColumn.Items.Add(new ComboBoxItem("c2", "c2"));
            this.ddlOrderColumn.Items.Add(new ComboBoxItem("c3", "c3"));
            this.ddlOrderColumn.SelectedIndex = 0;

            // ddlOrderSequence
            this.ddlOrderSequence.Items.Add(new ComboBoxItem("ASC", "A"));
            this.ddlOrderSequence.Items.Add(new ComboBoxItem("DESC", "D"));
            this.ddlOrderSequence.SelectedIndex = 0;

            // ユーザ情報
            this.myUserInfo = new MyUserInfo("userName", Environment.MachineName);

            // 呼出し制御部品
            this.CallCtrl = new CallController("");

            // スレッドプール
            ThreadPool.SetMinThreads(10, 10); // 待機状態スレッド数
            ThreadPool.SetMaxThreads(10, 10); // 最大スレッド起動数
        }

        #region コンボボックス用

        /// <summary>コンボボックス用インナークラス</summary>
        private class ComboBoxItem
        {
            /// <summary>表示名</summary>
            private string m_name = "";

            /// <summary>値</summary>
            private string m_value = "";

            /// <summary>コンストラクタ</summary>
            public ComboBoxItem(string name, string value)
            {
                m_name = name;
                m_value = value;
            }

            /// <summary>表示名</summary>
            public string Name
            {
                get
                {
                    return m_name;
                }
            }

            /// <summary>値</summary>
            public string Value
            {
                get
                {
                    return m_value;
                }
            }

            /// <summary>
            /// オーバーライドしたメソッド
            /// これがコンボボックスに表示される
            /// </summary>
            public override string ToString()
            {
                return m_name;
            }
        }

        #endregion        

        #endregion

        #region ＣＲＵＤ処理メソッド

        #region 参照系

        /// <summary>件数取得</summary>
        /// <remarks>
        /// 非同期フレームワークを使用してB層の呼び出し処理を非同期化
        /// （非同期実行、結果表示の双方に匿名デリゲードを使用するパターン）
        /// </remarks>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // 非同期処理クラスを生成
            // 匿名デリゲードの場合は、ベース２で良い。
            MyBaseAsyncFunc af = new MyBaseAsyncFunc(this);

            // 引数を纏め非同期処理クラスに設定
            af.Parameter = (object)new TestParameterValue(
                this.Name, ((Button)sender).Name, "SelectCount",
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                this.myUserInfo);

            // 画面上のデータは退避する
            //（オブジェクトであれば、クローンする。）
            string logicalName = ((ComboBoxItem)this.ddlTransmission.SelectedItem).Value;

            // 非同期実行するメソッドを指定（匿名デリゲード）
            // ここは副スレッドから実行されるので注意
            // （画面上のメンバに触らないこと！）。
            af.AsyncFunc = delegate(object param)
            {
                // 引数クラス（キャスト）
                TestParameterValue testParameterValue = (TestParameterValue)param;

                // 戻り値
                TestReturnValue testReturnValue;

                // 呼出し制御部品（スレッドセーフでないため副スレッド内で作る）
                CallController callCtrl = new CallController("");

                // Invoke
                testReturnValue = (TestReturnValue)callCtrl.Invoke(
                    logicalName, testParameterValue);

                //// 進捗表示のテスト
                //af.ChangeProgress = delegate(object o)
                //{
                //    MessageBox.Show(o.ToString());
                //};

                //af.ExecChangeProgress("進捗表示");

                //// 非同期メッセージボックス表示のテスト
                //MessageBoxResult mr = af.ShowAsyncMessageBoxWPF(
                //    "メッセージ", "タイトル", MessageBoxButton.YesNo, MessageBoxImage.Information);
                //// 非同期メッセージボックス表示のテスト（エラー）
                //System.Windows.Forms.DialogResult dr = af.ShowAsyncMessageBoxWin("メッセージ", "タイトル",
                //    System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Information);

                // 結果表示
                return testReturnValue;
            };

            // 結果表示のメソッドを指定（匿名デリゲード）
            // このメソッドは必ず主スレッドで実行される。
            // （画面上のメンバを更新できる！）。
            af.SetResult = delegate(object retVal)
            {
                if (retVal is Exception)
                {
                    // 例外発生時
                    RcMyCmnFunction.ShowErrorMessageWPF((Exception)retVal, "非同期処理で例外発生！");
                }
                else
                {
                    // 正常時

                    // 戻り値（キャスト）
                    TestReturnValue testReturnValue = (TestReturnValue)retVal;

                    // 結果表示するメッセージ エリア
                    this.labelMessage.Content = "";

                    if (testReturnValue.ErrorFlag == true)
                    {
                        // 結果（業務続行可能なエラー）
                        this.labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                        this.labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                        this.labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
                    }
                    else
                    {
                        // 結果（正常系）
                        this.labelMessage.Content = testReturnValue.Obj.ToString() + "件のデータがあります";
                    }
                }
            };

            // 非同期実行する。
            if (!af.StartByThreadPool())
            {
                MessageBox.Show("別の非同期処理が実行中です。");
            }
        }

        /// <summary>一覧取得（dt）</summary>
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, ((Button)sender).Name, "SelectAll_DT",
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    this.myUserInfo);

            // 戻り値
            TestReturnValue testReturnValue;

            // Invoke
            testReturnValue = (TestReturnValue)this.CallCtrl.Invoke(
                ((ComboBoxItem)this.ddlTransmission.SelectedItem).Value, testParameterValue);

            // 結果表示するメッセージ エリア
            this.labelMessage.Content = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                this.dataGrid1.Columns.Clear();
                this.dataGrid1.DataContext = testReturnValue.Obj;
            }
        }

        /// <summary>一覧取得（ds）</summary>
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, ((Button)sender).Name, "SelectAll_DS",
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    this.myUserInfo);

            // 戻り値
            TestReturnValue testReturnValue;

            // Invoke
            testReturnValue = (TestReturnValue)this.CallCtrl.Invoke(
                ((ComboBoxItem)this.ddlTransmission.SelectedItem).Value, testParameterValue);

            // 結果表示するメッセージ エリア
            this.labelMessage.Content = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                this.dataGrid1.Columns.Clear();
                this.dataGrid1.DataContext = ((DataSet)testReturnValue.Obj).Tables[0];
            }
        }

        /// <summary>一覧取得（dr）</summary>
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, ((Button)sender).Name, "SelectAll_DR",
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    this.myUserInfo);

            // 戻り値
            TestReturnValue testReturnValue;

            // Invoke
            testReturnValue = (TestReturnValue)this.CallCtrl.Invoke(
                ((ComboBoxItem)this.ddlTransmission.SelectedItem).Value, testParameterValue);

            // 結果表示するメッセージ エリア
            this.labelMessage.Content = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                this.dataGrid1.Columns.Clear();
                this.dataGrid1.DataContext = testReturnValue.Obj;
            }
        }

        /// <summary>一覧取得（動的sql）</summary>
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, ((Button)sender).Name, "SelectAll_DSQL",
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    this.myUserInfo);

            // 動的SQLの要素を設定
            testParameterValue.OrderColumn = ((ComboBoxItem)this.ddlOrderColumn.SelectedItem).Value;
            testParameterValue.OrderSequence = ((ComboBoxItem)this.ddlOrderSequence.SelectedItem).Value;

            // 戻り値
            TestReturnValue testReturnValue;

            // Invoke
            testReturnValue = (TestReturnValue)this.CallCtrl.Invoke(
                ((ComboBoxItem)this.ddlTransmission.SelectedItem).Value, testParameterValue);

            // 結果表示するメッセージ エリア
            this.labelMessage.Content = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                this.dataGrid1.Columns.Clear();
                this.dataGrid1.DataContext = testReturnValue.Obj;
            }
        }

        /// <summary>参照処理</summary>
        /// <remarks>
        /// 非同期フレームワークを使用してB層の呼び出し処理を非同期化
        /// （結果表示にだけ匿名デリゲードを使用するパターン）
        /// </remarks>
        private void button6_Click(object sender, RoutedEventArgs e)
        {
            // 非同期処理クラスを生成
            AsyncFunc af = new AsyncFunc(this);

            // 引数を纏める
            TestParameterValue testParameterValue = new TestParameterValue(
                this.Name, ((Button)sender).Name, "Select",
                ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                this.myUserInfo);

            // 情報の設定
            testParameterValue.ShipperID = int.Parse(this.textBox1.Text);

            // 引数を非同期処理クラスに設定
            af.Parameter = testParameterValue;

            // 画面上のデータは退避する（オブジェクトであれば、クローンする。）
            af.LogicalName = ((ComboBoxItem)this.ddlTransmission.SelectedItem).Value;

            // 非同期実行するメソッドを指定
            // ここは副スレッドから実行されるので注意。
            af.AsyncFunc = new BaseAsyncFunc.AsyncFuncDelegate(af.btn6_Exec);

            // 結果表示のメソッドを指定（匿名デリゲード）
            // このメソッドは必ず主スレッドで実行される。
            af.SetResult = delegate(object retVal)
            {
                if (retVal is Exception)
                {
                    // 例外発生時
                    RcMyCmnFunction.ShowErrorMessageWPF((Exception)retVal, "非同期処理で例外発生！");
                }
                else
                {
                    // 正常時

                    // 戻り値（キャスト）
                    TestReturnValue testReturnValue = (TestReturnValue)retVal;

                    // 結果表示するメッセージ エリア
                    this.labelMessage.Content = "";

                    if (testReturnValue.ErrorFlag == true)
                    {
                        // 結果（業務続行可能なエラー）
                        this.labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                        this.labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                        this.labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
                    }
                    else
                    {
                        // 結果（正常系）
                        this.textBox1.Text = testReturnValue.ShipperID.ToString();
                        this.textBox2.Text = testReturnValue.CompanyName;
                        this.textBox3.Text = testReturnValue.Phone;
                    }
                }
            };

            // 非同期実行する。
            if (!af.StartByThreadPool())
            {
                MessageBox.Show("別の非同期処理が実行中です。");
            }
        }

        #endregion

        #region 更新系

        /// <summary>追加処理</summary>
        private void button7_Click(object sender, RoutedEventArgs e)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, ((Button)sender).Name, "Insert",
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    this.myUserInfo);

            // 情報の設定
            testParameterValue.CompanyName = this.textBox2.Text;
            testParameterValue.Phone = this.textBox3.Text;

            // 戻り値
            TestReturnValue testReturnValue;

            // Invoke
            testReturnValue = (TestReturnValue)this.CallCtrl.Invoke(
                ((ComboBoxItem)this.ddlTransmission.SelectedItem).Value, testParameterValue);

            // 結果表示するメッセージ エリア
            this.labelMessage.Content = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                labelMessage.Content = testReturnValue.Obj.ToString() + "件追加";
            }
        }

        /// <summary>更新処理</summary>
        private void button8_Click(object sender, RoutedEventArgs e)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, ((Button)sender).Name, "Update",
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    this.myUserInfo);

            // 情報の設定
            testParameterValue.ShipperID = int.Parse(this.textBox1.Text);
            testParameterValue.CompanyName = this.textBox2.Text;
            testParameterValue.Phone = this.textBox3.Text;

            // 戻り値
            TestReturnValue testReturnValue;

            // Invoke
            testReturnValue = (TestReturnValue)this.CallCtrl.Invoke(
                ((ComboBoxItem)this.ddlTransmission.SelectedItem).Value, testParameterValue);

            // 結果表示するメッセージ エリア
            this.labelMessage.Content = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                labelMessage.Content = testReturnValue.Obj.ToString() + "件更新";
            }
        }

        /// <summary>削除処理</summary>
        private void button9_Click(object sender, RoutedEventArgs e)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, ((Button)sender).Name, "Delete",
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    this.myUserInfo);

            // 情報の設定
            testParameterValue.ShipperID = int.Parse(textBox1.Text);

            // 戻り値
            TestReturnValue testReturnValue;

            // Invoke
            testReturnValue = (TestReturnValue)this.CallCtrl.Invoke(
                ((ComboBoxItem)this.ddlTransmission.SelectedItem).Value, testParameterValue);

            // 結果表示するメッセージ エリア
            this.labelMessage.Content = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Content = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Content += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Content += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                labelMessage.Content = testReturnValue.Obj.ToString() + "件削除";
            }
        }

        #endregion

        #endregion

        #region その他

        /// <summary>クリア</summary>
        private void button10_Click(object sender, RoutedEventArgs e)
        {
            this.dataGrid1.Columns.Clear();
            this.dataGrid1.DataContext = null;
        }

        /// <summary>メッセージ取得（埋め込まれたリソース対応）</summary>
        private void button11_Click(object sender, RoutedEventArgs e)
        {
            this.textBox5.Text = GetMessage.GetMessageDescription(this.textBox4.Text);
        }

        /// <summary>共有情報取得（埋め込まれたリソース対応）</summary>
        private void button12_Click(object sender, RoutedEventArgs e)
        {
            this.textBox7.Text = GetSharedProperty.GetSharedPropertyValue(this.textBox6.Text);
        }

        #endregion
    }
}

//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Form1
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

using _2CSClientWin_sample.Business;
using _2CSClientWin_sample.Common;

using System;
using System.Data;

using Touryo.Infrastructure.Business.RichClient.Presentation;
using Touryo.Infrastructure.Framework.RichClient.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Db;

namespace _2CSClientWin_sample
{
    /// <summary>サンプル アプリ画面</summary>
    public partial class Form1 : MyBaseControllerWin
    {

        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// フォームロードのUOCメソッド（個別）
        /// </summary>
        protected override void UOC_FormInit()
        {
            // フォーム初期化（初回ロード）時に実行する処理を実装する

            // TODO:

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

            // ddlExRollback
            this.ddlExRollback.Items.Add(new ComboBoxItem("正常時", "-"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("業務例外", "Business"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("システム例外", "System"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("その他、一般的な例外", "Other"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("業務例外への振替", "Other-Business"));
            this.ddlExRollback.Items.Add(new ComboBoxItem("システム例外への振替", "Other-System"));
            this.ddlExRollback.SelectedIndex = 0;

            // ddlOrderColumn
            this.ddlOrderColumn.Items.Add(new ComboBoxItem("c1", "c1"));
            this.ddlOrderColumn.Items.Add(new ComboBoxItem("c2", "c2"));
            this.ddlOrderColumn.Items.Add(new ComboBoxItem("c3", "c3"));
            this.ddlOrderColumn.SelectedIndex = 0;

            // ddlOrderSequence
            this.ddlOrderSequence.Items.Add(new ComboBoxItem("ASC", "A"));
            this.ddlOrderSequence.Items.Add(new ComboBoxItem("DESC", "D"));
            this.ddlOrderSequence.SelectedIndex = 0;
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
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, rcFxEventArgs.ControlName, "SelectCount", 
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    MyBaseControllerWin.UserInfo);

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);
            LayerB.CommitAndClose();

            // 結果表示するメッセージ エリア
            this.labelMessage.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                labelMessage.Text = testReturnValue.Obj.ToString() + "件のデータがあります";
            }
        }

        /// <summary>一覧取得（dt）</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton2_Click(RcFxEventArgs rcFxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, rcFxEventArgs.ControlName, "SelectAll_DT", 
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    MyBaseControllerWin.UserInfo);

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);
            LayerB.CommitAndClose();
            
            // 結果表示するメッセージ エリア
            this.labelMessage.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                this.dataGridView1.DataSource = testReturnValue.Obj;
            }
        }

        /// <summary>一覧取得（ds）</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton3_Click(RcFxEventArgs rcFxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, rcFxEventArgs.ControlName, "SelectAll_DS", 
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    MyBaseControllerWin.UserInfo);

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);
            LayerB.CommitAndClose();

            // 結果表示するメッセージ エリア
            this.labelMessage.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                this.dataGridView1.DataSource = ((DataSet)testReturnValue.Obj).Tables[0];
            }
        }

        /// <summary>一覧取得（dr）</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton4_Click(RcFxEventArgs rcFxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, rcFxEventArgs.ControlName, "SelectAll_DR", 
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    MyBaseControllerWin.UserInfo);

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);
            LayerB.CommitAndClose();

            // 結果表示するメッセージ エリア
            this.labelMessage.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                this.dataGridView1.DataSource = testReturnValue.Obj;
            }
        }

        /// <summary>一覧取得（動的sql）</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton5_Click(RcFxEventArgs rcFxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, rcFxEventArgs.ControlName, "SelectAll_DSQL", 
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    MyBaseControllerWin.UserInfo);

            // 動的SQLの要素を設定
            testParameterValue.OrderColumn = ((ComboBoxItem)this.ddlOrderColumn.SelectedItem).Value;
            testParameterValue.OrderSequence = ((ComboBoxItem)this.ddlOrderSequence.SelectedItem).Value;

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);
            LayerB.CommitAndClose();

            // 結果表示するメッセージ エリア
            this.labelMessage.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                this.dataGridView1.DataSource = testReturnValue.Obj;
            }
        }

        /// <summary>参照処理</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton6_Click(RcFxEventArgs rcFxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, rcFxEventArgs.ControlName, "Select", 
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    MyBaseControllerWin.UserInfo);

            // 情報の設定
            testParameterValue.ShipperID = int.Parse(this.textBox1.Text);

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);
            LayerB.CommitAndClose();

            // 結果表示するメッセージ エリア
            this.labelMessage.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                this.textBox1.Text = testReturnValue.ShipperID.ToString();
                this.textBox2.Text = testReturnValue.CompanyName;
                this.textBox3.Text = testReturnValue.Phone;
            }
        }

        #endregion

        #region 更新系

        /// <summary>追加処理</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton7_Click(RcFxEventArgs rcFxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, rcFxEventArgs.ControlName, "Insert", 
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    MyBaseControllerWin.UserInfo);

            // 情報の設定
            testParameterValue.CompanyName = this.textBox2.Text;
            testParameterValue.Phone = this.textBox3.Text;

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);
            LayerB.CommitAndClose();

            // 結果表示するメッセージ エリア
            this.labelMessage.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                labelMessage.Text = testReturnValue.Obj.ToString() + "件追加";
            }
        }

        /// <summary>更新処理</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton8_Click(RcFxEventArgs rcFxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, rcFxEventArgs.ControlName, "Update", 
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    MyBaseControllerWin.UserInfo);

            // 情報の設定
            testParameterValue.ShipperID = int.Parse(this.textBox1.Text);
            testParameterValue.CompanyName = this.textBox2.Text;
            testParameterValue.Phone = this.textBox3.Text;

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);
            LayerB.CommitAndClose();

            // 結果表示するメッセージ エリア
            this.labelMessage.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                labelMessage.Text = testReturnValue.Obj.ToString() + "件更新";
            }
        }

        /// <summary>削除処理</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton9_Click(RcFxEventArgs rcFxEventArgs)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    this.Name, rcFxEventArgs.ControlName, "Delete", 
                    ((ComboBoxItem)this.ddlDap.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode1.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlMode2.SelectedItem).Value + "%"
                    + ((ComboBoxItem)this.ddlExRollback.SelectedItem).Value,
                    MyBaseControllerWin.UserInfo);

            // 情報の設定
            testParameterValue.ShipperID = int.Parse(textBox1.Text);

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = this.SelectIsolationLevel();

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);
            LayerB.CommitAndClose();

            // 結果表示するメッセージ エリア
            this.labelMessage.Text = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                labelMessage.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
                labelMessage.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
                labelMessage.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
            }
            else
            {
                // 結果（正常系）
                labelMessage.Text = testReturnValue.Obj.ToString() + "件削除";
            }
        }

        #endregion

        #endregion

        #region その他

        /// <summary>クリア</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton10_Click(RcFxEventArgs rcFxEventArgs)
        {
            this.dataGridView1.DataSource = null;
        }

        /// <summary>メッセージ取得（埋め込まれたリソース対応）</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton11_Click(RcFxEventArgs rcFxEventArgs)
        {
            this.textBox5.Text = GetMessage.GetMessageDescription(this.textBox4.Text);
        }

        /// <summary>共有情報取得（埋め込まれたリソース対応）</summary>
        /// <param name="rcFxEventArgs">イベントハンドラの共通引数</param>
        protected void UOC_btnButton12_Click(RcFxEventArgs rcFxEventArgs)
        {
            this.textBox7.Text = GetSharedProperty.GetSharedPropertyValue(this.textBox6.Text);
        }

        #endregion

        #region 分離レベルの設定メソッド

        /// <summary>分離レベルの設定</summary>
        private DbEnum.IsolationLevelEnum SelectIsolationLevel()
        {
            if (((ComboBoxItem)this.ddlIso.SelectedItem).Value == "NC")
            {
                return DbEnum.IsolationLevelEnum.NotConnect;
            }
            else if (((ComboBoxItem)this.ddlIso.SelectedItem).Value == "NT")
            {
                return DbEnum.IsolationLevelEnum.NoTransaction;
            }
            else if (((ComboBoxItem)this.ddlIso.SelectedItem).Value == "RU")
            {
                return DbEnum.IsolationLevelEnum.ReadUncommitted;
            }
            else if (((ComboBoxItem)this.ddlIso.SelectedItem).Value == "RC")
            {
                return DbEnum.IsolationLevelEnum.ReadCommitted;
            }
            else if (((ComboBoxItem)this.ddlIso.SelectedItem).Value == "RR")
            {
                return DbEnum.IsolationLevelEnum.RepeatableRead;
            }
            else if (((ComboBoxItem)this.ddlIso.SelectedItem).Value == "SZ")
            {
                return DbEnum.IsolationLevelEnum.Serializable;
            }
            else if (((ComboBoxItem)this.ddlIso.SelectedItem).Value == "SS")
            {
                return DbEnum.IsolationLevelEnum.Snapshot;
            }
            else if (((ComboBoxItem)this.ddlIso.SelectedItem).Value == "DF")
            {
                return DbEnum.IsolationLevelEnum.DefaultTransaction;
            }
            else
            {
                throw new Exception("分離レベルの設定がおかしい");
            }
        }

        #endregion
    }
}

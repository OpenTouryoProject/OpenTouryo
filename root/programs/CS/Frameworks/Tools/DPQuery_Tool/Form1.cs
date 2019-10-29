//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：Form1
//* クラス日本語名  ：動的パラメタライズド・クエリ実行ツール（メイン画面）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2008/xx/xx  西野 大介         新規作成
//*  2008/10/20  西野 大介         問題点の修正
//*  2008/12/18  西野 大介         MySQL対応
//*  2009/05/02  西野 大介         配列バインド対応と、型推測モードの実装
//*  2009/06/02  西野 大介         sln - IR版からの修正
//*                                ・#3  ： 配列バインド対応方法を「手動」にする。
//*                                ・#24 ： 空値でセーブした場合エラーとなる。
//*                                ・#25 ： DataReader＋トランザクション制御部分の見直し。
//*                                ・#26 ： Form1_Loadに例外のハンドル処理を追加する。
//*                                ・#27 ： 接続時、コミット/ロールバックが活性になる。
//*                                ・#28 ： 「に保存」ボタン押下時、「閉じる」「上書き」ボタン活性
//*                                ・#x  ： デバッグ用に機能追加。
//*  2009/08/12  西野 大介         比較演算子の向きを「<」に統一した。
//*  2009/09/18  西野 大介         比較/論理演算の記述部を括弧を付与（可読性）。
//*  2009/09/25  西野 大介         実行時、性能測定機能を追加した。
//*  2010/02/18  西野 大介         データプロバイダ追加（HiRDB、PostgreSQL）
//*  2012/02/09  西野 大介         HiRDBデータプロバイダのコメントアウト（（ソフト）対応せず）
//*  2012/02/09  西野 大介         OLEDB、ODBCのデータプロバイダ対応
//*  2012/09/28  西野 大介         ファイル実行箇所明示、IDE・D&D機能追加
//*  2013/03/05  西野 大介         「閉じる」で保存されないまま画面上のクエリが消える現象を修正
//*  2013/03/05  西野 大介         「閉じる」に、キャンセルボタンを追加
//*  2014/01/20  西野 大介         「保存」時のエンコーディング制御方式を見直し。
//*  2014/01/20  西野 大介         「保存」時のエンコーディング制御方式を見直し。
//*  2014/02/05  西野 大介         System.Data.OracleClientデータプロバイダ対応
//*  2014/04/24  Rituparna         Created Resource files for UI language changes and moved the English and Japanese languages
//*                                to proper Resouce files.Changed the control size to adjust the text properly in different languages.
//*  2014/04/25  Rituparna         Created Resource folder and Resource.ja-JP.resx,Resource.resx files inside the Resource folder.
//*                                Added proper key and values in those files for English and Japanese languages.
//*  2014/05/12  Rituparna         Removed <start> and <End> tags.
//*  2015/07/19  Sandeep           Improved UI of tools and button controls.
//*  2015/10/28  Sandeep           Optimized messages in the resource file and implemented code to format it.
//*  2018/10/29  西野 大介         NETCOREAPP対応で、サポートされないDBを「#if」した。
//**********************************************************************************

// --------------------
// データプロバイダ
// --------------------
using System.Data.SqlClient;
//using System.Data.OracleClient; // 衝突するのでエイリアスを作成
//using Oracle.DataAccess.Client; // Managedに移行
//using Oracle.ManagedDataAccess.Client; // 衝突するのでエイリアスを作成
using System.Data.Odbc;
using Npgsql;
using MySql.Data.MySqlClient;
#if NETCOREAPP
#else
using System.Data.OleDb;
using IBM.Data.DB2;
//using Hitachi.HiRDB;
#endif
// --------------------

using System;
using System.IO;
using System.Text;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace DPQuery_Tool
{
    using DntOraClient = System.Data.OracleClient;
    using OdpOraClient = Oracle.ManagedDataAccess.Client;

    /// <summary>動的パラメタライズド・クエリ実行ツール（メイン画面）</summary>
    public partial class Form1 : Form
    {
        #region メンバ変数

        /// <summary>
        /// SQLエンコーディング
        /// </summary>
        private string _sqlEncoding;

        /// <summary>
        /// データアクセス制御クラス
        /// </summary>
        private object _dam;

        /// <summary>
        /// ipアドレス
        /// </summary>
        private string _ip = "";

        /// <summary>
        /// ユーザID
        /// </summary>
        private string _uid = "";

        /// <summary>
        /// パスワード
        /// </summary>
        private string _pwd = "";

        /// <summary>
        /// ファイルから読み込んだ場合、ファイルから実行する。そのファイルパス。
        /// </summary>
        private string _loadFilePath = "";

        /// <summary>
        /// ファイルダイアログを使用すると、カレントディレクトリが
        /// 変更になるので、初回ロード時のカレントディレクトリを保持しておく。
        /// </summary>
        private string _orgCurrentDirectory = "";

        #endregion

        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public Form1()
        {
            InitializeComponent();

            #region フローレイアウト風にする。

            // テキスト
            this.txtSQL.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.label0.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);

            // ラベル類
            this.label1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.label2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.label3.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);

            this.label5.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.label8.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);

            // いろいろ
            this.lblFilePath.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.cmbDataProvider.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.txtCnnStr.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.cbxType.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.nudNumOfBind.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.cmbSelMethod.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);

            // グループボックス
            this.groupBoxR.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            this.groupBoxTx.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.panel1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            //this.groupBoxEXE.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            // 実行ボタン
            this.btnExecQuery.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
            this.btnOpenQueryFile.Anchor = (AnchorStyles.Left | AnchorStyles.Bottom);
            this.btnCloseQueryFile.Anchor = (AnchorStyles.Left | AnchorStyles.Bottom);
            this.btnOverwriteQueryFile.Anchor = (AnchorStyles.Left | AnchorStyles.Bottom);
            this.btnSaveQueryFile.Anchor = (AnchorStyles.Left | AnchorStyles.Bottom);

            #endregion

            #region コンボ系

            // 初期設定（DataProvider）
            this.cmbDataProvider.Items.Add(Literal.DAP_SQL);
            this.cmbDataProvider.Items.Add(Literal.DAP_ODB);
            this.cmbDataProvider.Items.Add(Literal.DAP_ORA); 
            this.cmbDataProvider.Items.Add(Literal.DAP_ODP);
            this.cmbDataProvider.Items.Add(Literal.DAP_MySQL);
            this.cmbDataProvider.Items.Add(Literal.DAP_PstgrS);
#if NETCOREAPP
#else
            this.cmbDataProvider.Items.Add(Literal.DAP_OLE);
            this.cmbDataProvider.Items.Add(Literal.DAP_DB2);
            //this.cmbDataProvider.Items.Add(Literal.DAP_HiRDB);
#endif
            this.cmbDataProvider.SelectedIndex = 0;

            // 初期設定（スロット）
            this.cmbSaveSlot.Items.AddRange(Literal.SLOT);
            this.cmbSaveSlot.SelectedIndex = 0;

            // 初期設定（メソッド）
            this.cmbSelMethod.Items.Add(Literal.METHOD_DATA_TABLE);
            this.cmbSelMethod.Items.Add(Literal.METHOD_DATA_SET);
            this.cmbSelMethod.Items.Add(Literal.METHOD_DATA_READER);
            this.cmbSelMethod.Items.Add(Literal.METHOD_SCALAR);
            this.cmbSelMethod.Items.Add(Literal.METHOD_NON_QUERY);
            this.cmbSelMethod.SelectedIndex = 0;

            // 初期設定（分離レベル）
            this.cmbSelIso.Items.Add(Literal.ISO_LEVEL_NO_TRANSACTION);
            this.cmbSelIso.Items.Add(Literal.ISO_LEVEL_DEFAULT_TRANSACTION);
            this.cmbSelIso.Items.Add(Literal.ISO_LEVEL_READ_UNCOMMITTED);
            this.cmbSelIso.Items.Add(Literal.ISO_LEVEL_READ_COMMITTED);
            this.cmbSelIso.Items.Add(Literal.ISO_LEVEL_REPEATABLE_READ);
            this.cmbSelIso.Items.Add(Literal.ISO_LEVEL_SERIALIZABLE);
            this.cmbSelIso.Items.Add(Literal.ISO_LEVEL_SNAPSHOT);

            // デバッグ用-start
            if (Environment.CommandLine.ToUpper().IndexOf("/DEBUG") != -1)
            {
                this.cmbSelIso.Items.Add(Literal.ISO_LEVEL_USER);
                this.cmbSelIso.Items.Add(Literal.ISO_LEVEL_NOT_CONNECT);
            }
            // デバッグ用-end

            this.cmbSelIso.SelectedIndex = 1; // DefaultTransactionが相応しいので。

            // 初期設定（トランザクション制御方式）
            this.cmbSelTxCtrl.Items.Add(Literal.COMMIT_MODE_AUTO);
            this.cmbSelTxCtrl.Items.Add(Literal.COMMIT_MODE_MANUAL);
            this.cmbSelTxCtrl.SelectedIndex = 0;

            #endregion

            // 初回ロード時のカレントディレクトリを退避しておく。
            this._orgCurrentDirectory = System.Environment.CurrentDirectory;

            #region 状態の更新

            // クエリ ファイル
            this.btnCloseQueryFile.Enabled = false;
            this.btnOverwriteQueryFile.Enabled = false;

            // 接続・実行
            this.btnCnClose.Enabled = false;
            this.btnExecQuery.Enabled = false;

            // トランザクション系
            this.cmbSelTxCtrl.Enabled = false;
            this.btnBeginTx.Enabled = false;
            this.btnCommitTx.Enabled = false;
            this.btnRollbackTx.Enabled = false;

            #endregion

            // 初期状態
            ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_INIT");

            // 最初のフォーカス
            this.cmbDataProvider.Focus();
        }

        /// <summary>フォームのロード</summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // デュアル フォントを無効にする。
                this.txtSQL.DualFont = false;

                // D&D受付
                this.txtSQL.AllowDrop = true;
                this.txtSQL.DragEnter += new DragEventHandler(this.TextBox_DragEnter);
                this.txtSQL.DragDrop += new DragEventHandler(this.TextBox_DragDrop);

                // メニューの表示
                this.ContextMenuStrip = this.contextMenuStrip1;
                this.txtSQL.ContextMenuStrip = this.ContextMenuStrip;

                this._sqlEncoding = GetConfigParameter.GetConfigValue(PubLiteral.SQL_ENCODING);

                if (this._sqlEncoding == null)
                {
                    // SQLエンコーディングが指定されていない場合。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.NO_CONFIG, PubLiteral.SQL_ENCODING));
                }
            }
            catch (Exception ex)
            {
                // メッセージを表示する
                MessageBox.Show(ex.Message);

                // ここで例外となる場合は、実行できないので自画面を閉じる。
                this.Close();
            }
        }

        #endregion

        #region 設定のセーブ・ロード

        /// <summary>To handle UI of button controls</summary>
        private void btnColor1_EnabledChanged(object sender, System.EventArgs e)
        {
            Button btnColor1 = (Button)sender;
            if (btnColor1.Enabled)
            {
                btnColor1.BackColor = Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(163)))), ((int)(((byte)(189)))));
                btnColor1.ForeColor = Color.White;
            }
            else
            {
                btnColor1.BackColor = System.Drawing.SystemColors.Control;
                btnColor1.ForeColor = Color.White;
            }
        }

        /// <summary>To handle UI of button controls</summary>
        private void btnColor2_EnabledChanged(object sender, System.EventArgs e)
        {
            Button btnColor2 = (Button)sender;
            if (btnColor2.Enabled)
            {
                btnColor2.BackColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(166)))), ((int)(((byte)(44)))));
                btnColor2.ForeColor = Color.White;
            }
            else
            {
                btnColor2.BackColor = System.Drawing.SystemColors.Control;
                btnColor2.ForeColor = Color.White;
            }
        }

        /// <summary>設定の新規作成</summary>
        private void btnCreateConfig_Click(object sender, EventArgs e)
        {
            try
            {
                // ダイアログを表示して設定する。
                this._ip = this.ShowInputDialog(this.RM_GetString("MSG_INPUT_IP"), false);
                this._uid = this.ShowInputDialog(this.RM_GetString("MSG_INPUT_UID"), false);
                this._pwd = this.ShowInputDialog(this.RM_GetString("MSG_INPUT_PWD"), true);

                // 設定を反映するために・・・
                this.cmbDataProvider_SelectedIndexChanged(sender, e);

                // 状態
                ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_CREATE_CONFIGURATION");

            }
            catch (Exception ex)
            {
                // メッセージを表示する
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>設定のセーブ</summary>
        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            // セーブする。

            try
            {
                // エンコーディング指定してセーブ
                StreamWriter sw = new StreamWriter(
                    this._orgCurrentDirectory + @"\" + this.MakeConfigFileName(),
                    false, Encoding.GetEncoding(CustomEncode.shift_jis));

                // 書き込み
                sw.WriteLine(Literal.CONFIGURATION_DAP + " " + this.cmbDataProvider.SelectedItem.ToString());
                sw.WriteLine(Literal.CONFIGURATION_IP + " " + this._ip);
                sw.WriteLine(Literal.CONFIGURATION_UID + " " + this._uid);
                sw.WriteLine(Literal.CONFIGURATION_PWD + " " + this._pwd);
                sw.WriteLine(Literal.CONFIGURATION_CSR + " " + this.txtCnnStr.Text);

                // クローズ
                sw.Close();

                // 状態
                ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_CONFIGURATION_SAVED");

            }
            catch (Exception ex)
            {
                // メッセージを表示する
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>設定のロード</summary>
        private void btnLoadConfig_Click(object sender, EventArgs e)
        {
            // ロードする。

            try
            {
                // ファイルの存在を確認する。
                if (File.Exists(this._orgCurrentDirectory + @"\" + this.MakeConfigFileName()))
                {
                    // エンコーディング指定してロード
                    StreamReader sr = new StreamReader(
                        this._orgCurrentDirectory + @"\" + this.MakeConfigFileName(),
                        Encoding.GetEncoding(CustomEncode.shift_jis));

                    while (!sr.EndOfStream)
                    {
                        string str = sr.ReadLine();

                        int len = 0;

                        // dap
                        len = Literal.CONFIGURATION_DAP.Length + 1;

                        if (len <= str.Length) // 2009/08/12-この行
                        {
                            if (str.Substring(0, len).Trim() == Literal.CONFIGURATION_DAP)
                            {
                                if (str.Substring(len) == Literal.DAP_SQL)
                                {
                                    this.cmbDataProvider.SelectedIndex = 0;
                                }
                                else if (str.Substring(len) == Literal.DAP_ODB)
                                {
                                    this.cmbDataProvider.SelectedIndex = 2;
                                }
                                else if (str.Substring(len) == Literal.DAP_ORA)
                                {
                                    this.cmbDataProvider.SelectedIndex = 3;
                                }
                                else if (str.Substring(len) == Literal.DAP_ODP)
                                {
                                    this.cmbDataProvider.SelectedIndex = 4;
                                }
                                else if (str.Substring(len) == Literal.DAP_MySQL)
                                {
                                    this.cmbDataProvider.SelectedIndex = 6;
                                }
                                else if (str.Substring(len) == Literal.DAP_PstgrS)
                                {
                                    this.cmbDataProvider.SelectedIndex = 7;
                                }
#if NETCOREAPP
#else
                                else if (str.Substring(len) == Literal.DAP_OLE)
                                {
                                    this.cmbDataProvider.SelectedIndex = 1;
                                }
                                else if (str.Substring(len) == Literal.DAP_DB2)
                                {
                                    this.cmbDataProvider.SelectedIndex = 5;
                                }
                                //else if (str.Substring(len) == Literal.DAP_HiRDB)
                                //{
                                //    this.cmbDataProvider.SelectedIndex = x;
                                //}
#endif
                                else
                                {
                                    // ありえない
                                    this.cmbDataProvider.SelectedIndex = 0;
                                }
                            }
                        }

                        // ip
                        len = Literal.CONFIGURATION_IP.Length + 1;

                        if (len <= str.Length) // 2009/08/12-この行
                        {
                            if (str.Substring(0, len).Trim() == Literal.CONFIGURATION_IP)
                            {
                                this._ip = str.Substring(len);
                            }
                        }

                        // uid
                        len = Literal.CONFIGURATION_UID.Length + 1;

                        if (len <= str.Length) // 2009/08/12-この行
                        {
                            if (str.Substring(0, len).Trim() == Literal.CONFIGURATION_UID)
                            {
                                this._uid = str.Substring(len);
                            }
                        }

                        // pwd
                        len = Literal.CONFIGURATION_PWD.Length + 1;

                        if (len <= str.Length) // 2009/08/12-この行
                        {
                            if (str.Substring(0, len).Trim() == Literal.CONFIGURATION_PWD)
                            {
                                this._pwd = str.Substring(len);
                            }
                        }

                        // csr
                        len = Literal.CONFIGURATION_CSR.Length + 1;

                        if (len <= str.Length) // 2009/08/12-この行
                        {
                            if (str.Substring(0, len).Trim() == Literal.CONFIGURATION_CSR)
                            {
                                this.txtCnnStr.Text = str.Substring(len);
                            }
                        }
                    }

                    // クローズ
                    sr.Close();

                    // 状態
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_CONFIGURATION_LOADED");
                }
                else
                {
                    // ファイルが存在しない。
                    MessageBox.Show(this.RM_GetString("MSG_CONFIG_FILE_NOT_EXIST"));
                }
            }
            catch (Exception ex)
            {
                // メッセージを表示する
                MessageBox.Show(ex.Message);
            }
        }

        #region ユーティリティメソッド

        /// <summary>入力ダイアログを表示→消し</summary>
        /// <param name="lbl">メッセージ</param>
        /// <param name="IsPWD">パスワードの場合</param>
        /// <returns>入力値</returns>
        private string ShowInputDialog(string lbl, bool IsPWD)
        {
            // ダイアログ
            InputDialog id = new InputDialog();

            // 設定、パスワード
            id._isPWD = IsPWD;

            // ラベル
            id._lbl = lbl;

            // 表示位置
            id.StartPosition = FormStartPosition.CenterParent;

            // 入力値を取得
            id.ShowDialog(this);
            return id._ret;
        }

        /// <summary>スロット毎のコンフィグ ファイル名を作成する。</summary>
        /// <returns>スロット毎のコンフィグ ファイル名</returns>
        private string MakeConfigFileName()
        {
            // ファイル名を作成
            return this.cmbSaveSlot.SelectedItem.ToString() + Literal.CONFIG_FILE_FOOTER;
        }

        #endregion

        #endregion

        #region Damの選択

        /// <summary>Damの選択</summary>
        private void cmbDataProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // 初期は、無効にして、ODP.NET、HiRDB選択時のみ有効にする。
                this.nudNumOfBind.Enabled = false;

                if (this.cmbDataProvider.SelectedItem.ToString() == Literal.DAP_SQL)
                {
                    //sqlClient
                    this._dam = new DamSqlSvr();

                    //接続文字列のサンプルを設定する（空の場合）。
                    SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();

                    csb.DataSource = this._ip;
                    csb.InitialCatalog = "Northwind";
                    csb.UserID = this._uid;
                    csb.Password = this._pwd;

                    this.txtCnnStr.Text = csb.ConnectionString;

                    // 状態
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = string.Format(this.RM_GetString("STATUS_DATA_PROVIDER_SELECTED"), Literal.DAP_SQL);

                }
                else if (this.cmbDataProvider.SelectedItem.ToString() == Literal.DAP_ODB)
                {
                    //ODBC.NET
                    this._dam = new DamODBC();

                    //接続文字列のサンプルを設定する（空の場合）。
                    OdbcConnectionStringBuilder csb = new OdbcConnectionStringBuilder();

                    csb.Driver = "DriverName";
                    csb.Dsn = "DataSourceName";

                    this.txtCnnStr.Text = csb.ConnectionString;

                    // 活性
                    this.nudNumOfBind.Enabled = true;

                    // 状態
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = string.Format(this.RM_GetString("STATUS_DATA_PROVIDER_SELECTED"), Literal.DAP_ODB);

                }
                else if (this.cmbDataProvider.SelectedItem.ToString() == Literal.DAP_ORA)
                {
                    //Oracle Client
                    this._dam = new DamOraClient();

                    //接続文字列のサンプルを設定する（空の場合）。
                    DntOraClient.OracleConnectionStringBuilder csb = new DntOraClient.OracleConnectionStringBuilder();

                    csb.DataSource = this._ip + "/orcl";
                    csb.UserID = this._uid;
                    csb.Password = this._pwd;

                    this.txtCnnStr.Text = csb.ConnectionString;

                    // 活性
                    this.nudNumOfBind.Enabled = true;

                    // 状態
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = string.Format(this.RM_GetString("STATUS_DATA_PROVIDER_SELECTED"), Literal.DAP_ORA);

                } 
                else if (this.cmbDataProvider.SelectedItem.ToString() == Literal.DAP_ODP)
                {
                    //ODP.NET
                    this._dam = new DamManagedOdp();

                    //接続文字列のサンプルを設定する（空の場合）。
                    OdpOraClient.OracleConnectionStringBuilder csb = new OdpOraClient.OracleConnectionStringBuilder();

                    csb.DataSource = this._ip + "/orcl";
                    csb.UserID = this._uid;
                    csb.Password = this._pwd;

                    this.txtCnnStr.Text = csb.ConnectionString;

                    // 活性
                    this.nudNumOfBind.Enabled = true;

                    // 状態
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = string.Format(this.RM_GetString("STATUS_DATA_PROVIDER_SELECTED"), Literal.DAP_ODP);

                }
                else if (this.cmbDataProvider.SelectedItem.ToString() == Literal.DAP_MySQL)
                {
                    //MySQL Connector/NET
                    this._dam = new DamMySQL();

                    //接続文字列のサンプルを設定する（空の場合）。
                    MySqlConnectionStringBuilder csb = new MySqlConnectionStringBuilder();

                    csb.Server = this._ip;
                    csb.Database = "test";
                    csb.UserID = this._uid;
                    csb.Password = this._pwd;

                    this.txtCnnStr.Text = csb.ConnectionString;

                    // 状態
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = string.Format(this.RM_GetString("STATUS_DATA_PROVIDER_SELECTED"), Literal.DAP_MySQL);

                }
                else if (this.cmbDataProvider.SelectedItem.ToString() == Literal.DAP_PstgrS)
                {
                    //Npgsql
                    this._dam = new DamPstGrS();

                    //接続文字列のサンプルを設定する（空の場合）。

                    //NpgsqlConnectionStringBuilderは、冗長な接続文字列を返すので使わない。
                    NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder();
                    csb.Host = this._ip;
                    csb.Database = "postgres";
                    csb.Username = this._uid;
                    csb.Password = this._pwd;

                    string cnnStr = "";
                    cnnStr += "HOST=" + this._ip + ";";
                    cnnStr += "DATABASE=postgres;";
                    cnnStr += "UID=" + this._uid + ";";
                    cnnStr += "PWD=" + this._pwd + ";";

                    this.txtCnnStr.Text = cnnStr;

                    // 状態
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = string.Format(this.RM_GetString("STATUS_DATA_PROVIDER_SELECTED"), Literal.DAP_PstgrS);
                }
#if NETCOREAPP
#else
                else if (this.cmbDataProvider.SelectedItem.ToString() == Literal.DAP_OLE)
                {
                    //OLEDB.NET
                    this._dam = new DamOLEDB();

                    //接続文字列のサンプルを設定する（空の場合）。
                    OleDbConnectionStringBuilder csb = new OleDbConnectionStringBuilder();

                    csb.Provider = "Provider";
                    csb.DataSource = "DataSourceName";
                    csb.FileName = "FileName";

                    this.txtCnnStr.Text = csb.ConnectionString;

                    // 活性
                    this.nudNumOfBind.Enabled = true;

                    // 状態
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = string.Format(this.RM_GetString("STATUS_DATA_PROVIDER_SELECTED"), Literal.DAP_OLE);

                }
                else if (this.cmbDataProvider.SelectedItem.ToString() == Literal.DAP_DB2)
                {
                    //DB2.NET
                    this._dam = new DamDB2();

                    //接続文字列のサンプルを設定する（空の場合）。
                    DB2ConnectionStringBuilder csb = new DB2ConnectionStringBuilder();

                    //csb.Server = this._ip + ":50000";
                    csb.Database = "SAMPLE";
                    csb.UserID = this._uid;
                    csb.Password = this._pwd;

                    this.txtCnnStr.Text = csb.ConnectionString;

                    // 状態
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = string.Format(this.RM_GetString("STATUS_DATA_PROVIDER_SELECTED"), Literal.DAP_DB2);

                }
                //else if (this.cmbDataProvider.SelectedItem.ToString() == Literal.DAP_HiRDB)
                //{
                //    //HiRDBデータ プロバイダ
                //    this._dam = new DamHiRDB();

                //    //接続文字列のサンプルを設定する（空の場合）。
                //    //HiRDBデータ プロバイダは、ConnectionStringBuilderがない。
                //    string csb = "";
                //    csb += "DataSource=C:\\Windows\\HiRDB.ini;";
                //    csb += "UID=" + this._uid + ";";
                //    csb += "PWD=" + this._pwd + ";";

                //    this.txtCnnStr.Text = csb;

                //    // 活性
                //    this.nudNumOfBind.Enabled = true;

                //    // 状態
                //    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = Literal.STATUS_HRD_CREATED;
                //}
#endif          
                else
                {
                    //ありえない
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region コネクション オープン

        /// <summary>コネクション オープン</summary>
        private void btnCnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                // コネクション オープン
                ((BaseDam)this._dam).ConnectionOpen(this.txtCnnStr.Text);

                // ボタンの状態を変更

                // 設定関連
                this.cmbDataProvider.Enabled = false;
                this.txtCnnStr.Enabled = false;

                this.btnCreateConfig.Enabled = false;

                this.cmbSaveSlot.Enabled = false;
                this.btnSaveConfig.Enabled = false;
                this.btnLoadConfig.Enabled = false;

                // SQL実行ボタン
                this.btnExecQuery.Enabled = true;

                // コネクションボタン
                this.btnCnOpen.Enabled = false;
                this.btnCnClose.Enabled = true;

                // トランザクション系
                this.cmbSelIso.Enabled = true;
                this.cmbSelTxCtrl.Enabled = true;

                if (this.cmbSelTxCtrl.SelectedItem.ToString() == Literal.COMMIT_MODE_MANUAL)
                {
                    this.btnBeginTx.Enabled = true;
                    //this.btnCommitTx.Enabled = true;
                    //this.btnRollbackTx.Enabled = true;
                }

                // 状態
                ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_CONNECTION_OPENED");

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        #endregion

        #region クエリ ファイルの操作

        /// <summary>クエリ ファイルを開く。</summary>
        private void btnOpenQueryFile_Click(object sender, EventArgs e)
        {
            try
            {
                // オープン時の動作を記述

                // ファイルを選択
                this.openFileDialog.Multiselect = false;
                this.openFileDialog.FileName = "";
                this.openFileDialog.ShowDialog();

                // ファイルが選択されなかった場合
                if (this.openFileDialog.FileName == "")
                {
                    // 何もしないで戻る（キャンセル扱い）。
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_PROC_CANCELED");

                }
                else
                {
                    // SQLファイルを開く
                    this.OpenQueryFile(this.openFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                // メッセージを表示する
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>ファイルのD&D</summary>
        private void TextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // ドラッグ中のファイルやディレクトリの取得
                string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string d in drags)
                {
                    if (!System.IO.File.Exists(d))
                    {
                        // ファイル以外であればイベント・ハンドラを抜ける
                        return;
                    }
                }

                e.Effect = DragDropEffects.Copy;
            }
        }

        /// <summary>ファイルのD&D</summary>
        private void TextBox_DragDrop(object sender, DragEventArgs e)
        {
            // ドラッグ＆ドロップされたファイル
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // SQLファイルを開く
            this.OpenQueryFile(files[0]);
        }

        /// <summary>ファイルを開く</summary>
        /// <param name="filePath">ファイルのパス</param>
        private void OpenQueryFile(string filePath)
        {
            // パスを設定
            this._loadFilePath = filePath;

            // ロードする。
            // エンコーディング指定してロード
            StreamReader sr = new StreamReader(
                this._loadFilePath,
                Encoding.GetEncoding(this._sqlEncoding));

            // テキストボックスに内容を記述。
            this.txtSQL.Text = sr.ReadToEnd();

            // クローズ
            sr.Close();

            //　ファイル名をラベルに記述。
            string[] temp = this._loadFilePath.Split('\\');
            this.lblFilePath.Text = temp[temp.Length - 1];

            // 状態
            ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_QUERY_FILE_OPENED");


            // 有効にするボタン
            this.btnCloseQueryFile.Enabled = true;
            this.btnOverwriteQueryFile.Enabled = true;
        }

        /// <summary>クエリ ファイルを上書き保存する。</summary>
        private void btnOverwriteQueryFile_Click(object sender, EventArgs e)
        {
            try
            {
                // エンコーディング指定してセーブ
                File.WriteAllText(
                    this._loadFilePath,
                    this.txtSQL.Text.Replace(
                            "<?xml version=\"1.0\"?>",
                            string.Format("<?xml version=\"1.0\" encoding=\"{0}\"?>", this._sqlEncoding)),
                    Encoding.GetEncoding(this._sqlEncoding));

                // 状態
                ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_QUERY_FILE_OVERWRITED");

            }
            catch (Exception ex)
            {
                // メッセージを表示する
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>クエリ ファイルを閉じる。</summary>
        private void btnCloseQueryFile_Click(object sender, EventArgs e)
        {
            try
            {
                // クローズ時の動作を記述

                // ファイルをセーブするか、セーブしないかを選択可能とする。
                DialogResult dr = MessageBox.Show(
                    this.RM_GetString("MSG_IS_QUERY_FILE_SAVED"), this.RM_GetString("ClosingProcess"), MessageBoxButtons.YesNoCancel);


                if (dr == DialogResult.Yes)
                {
                    //// 上書きのイベントを発生させる。
                    //this.btnOverwriteQueryFile.PerformClick();

                    // エンコーディング指定してセーブ
                    File.WriteAllText(
                        this._loadFilePath,
                        this.txtSQL.Text.Replace(
                            "<?xml version=\"1.0\"?>",
                            string.Format("<?xml version=\"1.0\" encoding=\"{0}\"?>", this._sqlEncoding)),
                        Encoding.GetEncoding(this._sqlEncoding));
                }
                else if (dr == DialogResult.No)
                {
                    // セーブしない（継続）。
                    // なにもしない。
                }
                else if (dr == DialogResult.Cancel)
                {
                    // セーブしない（脱出）。
                    return;
                }
                else
                {
                    // ありえん
                }

                // ・・・で閉じる。

                // パスをクリア
                this._loadFilePath = "";

                //　ファイル名をラベルに記述。
                this.lblFilePath.Text = "－";

                //　テキストボックスをクリア
                this.txtSQL.Text = "";

                // 状態
                ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_QUERY_FILE_CLOSED");

                // 無効にするボタン
                this.btnCloseQueryFile.Enabled = false;
                this.btnOverwriteQueryFile.Enabled = false;
            }
            catch (Exception ex)
            {
                // メッセージを表示する
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>クエリ ファイルに保存する。</summary>
        private void btnSaveQueryFile_Click(object sender, EventArgs e)
        {
            try
            {
                // セーブする。
                this.saveFileDialog.FileName = "";
                this.saveFileDialog.ShowDialog();

                // ファイルが選択されなかった場合
                if (this.saveFileDialog.FileName == "")
                {
                    // 何もしないで戻る（キャンセル扱い）。
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_PROC_CANCELED");

                }
                else
                {
                    // パスを設定
                    this._loadFilePath = this.saveFileDialog.FileName;

                    // エンコーディング指定してセーブ
                    File.WriteAllText(
                        this._loadFilePath, 
                        this.txtSQL.Text.Replace(
                            "<?xml version=\"1.0\"?>",
                            string.Format("<?xml version=\"1.0\" encoding=\"{0}\"?>", this._sqlEncoding)),
                        Encoding.GetEncoding(this._sqlEncoding));

                    //　ファイル名をラベルに記述。
                    string[] temp = this._loadFilePath.Split('\\');
                    this.lblFilePath.Text = temp[temp.Length - 1];

                    // 状態
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_QUERY_FILE_SAVED");

                    // 有効にするボタン
                    this.btnCloseQueryFile.Enabled = true;
                    this.btnOverwriteQueryFile.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                // メッセージを表示する
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region トランザクション制御

        /// <summary>トランザクションの開始</summary>
        private void btnBeginTx_Click(object sender, EventArgs e)
        {
            try
            {
                // トランザクションの開始
                ((BaseDam)this._dam).BeginTransaction(this.SelectIso());

                // トランザクションの制御コンボを非アクティブにする。
                this.cmbSelTxCtrl.Enabled = false;
                this.cmbSelIso.Enabled = false;

                // トランザクションの開始ボタンを非アクティブにする。
                this.btnBeginTx.Enabled = false;

                // トランザクションの終了ボタンをアクティブにする。
                this.btnCommitTx.Enabled = true;
                this.btnRollbackTx.Enabled = true;

                // 初期状態
                ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_TRANSACTION_STARTED");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>トランザクションのコミット</summary>
        private void btnCommitTx_Click(object sender, EventArgs e)
        {
            try
            {
                // トランザクションのコミット
                ((BaseDam)this._dam).CommitTransaction();

                // トランザクションの制御コンボをアクティブにする。
                this.cmbSelTxCtrl.Enabled = true;
                this.cmbSelIso.Enabled = true;

                // トランザクションの開始ボタンをアクティブにする。
                this.btnBeginTx.Enabled = true;

                // トランザクションの終了ボタンを非アクティブにする。
                this.btnCommitTx.Enabled = false;
                this.btnRollbackTx.Enabled = false;

                // 初期状態
                ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_TRANSACTION_COMMITED");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>トランザクションのロールバック</summary>
        private void btnRollbackTx_Click(object sender, EventArgs e)
        {
            try
            {
                // トランザクションのロールバック
                ((BaseDam)this._dam).RollbackTransaction();

                // トランザクションの制御コンボをアクティブにする。
                this.cmbSelTxCtrl.Enabled = true;
                this.cmbSelIso.Enabled = true;

                // トランザクションの開始ボタンをアクティブにする。
                this.btnBeginTx.Enabled = true;

                // トランザクションの終了ボタンを非アクティブにする。
                this.btnCommitTx.Enabled = false;
                this.btnRollbackTx.Enabled = false;

                // 初期状態
                ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_TRANSACTION_ROLLBACKED");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>制御ボタンが変更された場合</summary>
        private void cmbSelTxCtrl_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cmbSelTxCtrl.SelectedItem.ToString() == Literal.COMMIT_MODE_AUTO)
                {
                    // 自動トランザクション
                    this.btnBeginTx.Enabled = false;
                    this.btnCommitTx.Enabled = false;
                    this.btnRollbackTx.Enabled = false;

                    // 状態
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_AUTO_MODE_WAS_SELECTED");

                }
                else if (this.cmbSelTxCtrl.SelectedItem.ToString() == Literal.COMMIT_MODE_MANUAL)
                {
                    // 手動トランザクション
                    this.btnBeginTx.Enabled = true;
                    this.btnCommitTx.Enabled = false;
                    this.btnRollbackTx.Enabled = false;

                    // 状態
                    ((ToolStripStatusLabel)this.statBar.Items[0]).Text =
                       this.RM_GetString("STATUS_MANUAL_MODE_WAS_SELECTED");

                }
                else
                {
                    //ありえない
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region SQLの実行

        /// <summary>SQLの実行</summary>
        private void btnExecQuery_Click(object sender, EventArgs e)
        {
            string sql = "";
            string log = "";
            string caption = "";

            #region 性能測定関係の変数

            // 2009/09/25-start

            // 性能測定
            PerformanceRecorder prfRec = null;

            //// 性能測定－開始
            //prfRec = new PerformanceRecorder();
            //prfRec.StartsPerformanceRecord();

            //// 性能情報を出力
            //log += "【性能情報－チェック処理】" + "\r\n";
            //log += prfRec.EndsPerformanceRecord() + "\r\n\r\n";

            // 2009/09/25-end

            #endregion

            #region コマンドの設定

            try
            {
                // 実行中はトランザクションの制御方式を変更できないようにする。
                this.groupBoxTx.Enabled = false;

                // コマンド
                if (this._loadFilePath == "")
                {
                    // ファイルパスが指定されている。
                    ((BaseDam)this._dam).SetSqlByCommand(this.txtSQL.Text);
                    caption = this.RM_GetString("RunTextBox");
                   
                }
                else
                {
                    // ファイルパスが指定されていない。
                    ((BaseDam)this._dam).SetSqlByFile(this._loadFilePath); 
                    caption = PubCmnFunction.GetFileNameNoEx(this._loadFilePath, '\\') + this.RM_GetString("RunFile");
                   
                }
            }
            catch (Exception Ex)
            {
                // チェックエラーのメッセージを表示する
                MessageBox.Show(Ex.Message);

                // 処理の中断
                return;
            }

            #endregion

            //// HiRDBデータプロバイダの配列バインド
            //bool hrdArryBind = false;

            try
            {
                #region メッセージ

                if (((BaseDam)this._dam).IsDPQ)
                {
                    MessageBox.Show(this.RM_GetString("MSG_EXEC_DQP"), caption);

                }
                else
                {
                    MessageBox.Show(this.RM_GetString("MSG_EXEC_SPQ"), caption);

                }

                #endregion

                #region パラメタの設定

                DataTable dtParams = ((BaseDam)this._dam).GetParametersFromPARAMTag();

                foreach (DataRow dr in dtParams.Rows)
                {
                    if (((bool)dr[0]))
                    {
                        // ユーザパラメタ
                        ((BaseDam)this._dam).SetUserParameter(dr[1].ToString(), dr[2].ToString());
                    }
                    else
                    {
                        //通常のパラメタ

                        if (dr[3].ToString() == PubLiteral.VALUE_STR_NULL)
                        {
                            // nullを指定
                            ((BaseDam)this._dam).SetParameter(dr[1].ToString(), null);
                        }
                        else
                        {
                            // null以外の値を指定

                            if ((this._dam.GetType() == typeof(DamManagedOdp))
                                & (0 < (int)this.nudNumOfBind.Value) & (dr[2] is System.Array))
                            {
                                // ODP.NETの配列バインドの場合

                                // ・ODP.NETで実行されている。
                                // ・配列バインド数が０より大きい値に設定されている。
                                // ・パラメタに配列が指定されている。

                                // ArrayBindCountプロパティを指定
                                ((DamManagedOdp)this._dam).ArrayBindCount = (int)this.nudNumOfBind.Value;

                                // 型情報を取得（ODP.NETの配列バインドでは必須のため）
                                object dbTypeInfo = null;

                                // 推論ロジックで推論できない型の場合は、nullが指定される。
                                this.InferODPType(((Array)dr[2]).GetValue(0).GetType(), out dbTypeInfo);

                                // 配列バインド（型情報が必要）
                                ((BaseDam)this._dam).SetParameter(dr[1].ToString(), dr[2], dbTypeInfo);
                            }
                            //else if ((this._dam.GetType() == typeof(DamHiRDB))
                            //    & (0 < (int)this.nudNumOfBind.Value) & (dr[2] is System.Array))
                            //{
                            //    // HiRDBデータプロバイダの配列バインドの場合

                            //    // ・HiRDBデータプロバイダで実行されている。
                            //    // ・配列バインド数が０より大きい値に設定されている。
                            //    // ・パラメタに配列が指定されている。

                            //    // HiRDBデータプロバイダの場合、NonQueryのオーバーロード
                            //    // に配列バインド数を指定するのでフラグを立てておく。
                            //    hrdArryBind = true;

                            //    // 型の推論の有・無
                            //    if (this.cbxType.Checked == true)
                            //    {
                            //        // 型の推論（有）

                            //        // 型情報を取得
                            //        object dbTypeInfo = null;

                            //        // HiRDBデータプロバイダの型情報を推論
                            //        // 推論不明（マニュアル無し）

                            //        // 配列パラメタを設定（型情報・有）
                            //        ((BaseDam)this._dam).SetParameter(dr[1].ToString(), dr[2], dbTypeInfo);
                            //    }
                            //    else
                            //    {
                            //        // 型の推論（無）

                            //        // 配列パラメタを設定（型情報・無）
                            //        ((BaseDam)this._dam).SetParameter(dr[1].ToString(), dr[2]);
                            //    }
                            //}
                            else
                            {
                                // 通常の場合

                                // 型の推論の有・無
                                if (this.cbxType.Checked == true)
                                {
                                    // 型の推論（有）

                                    // 型情報を取得
                                    object dbTypeInfo = null;

                                    if (this._dam.GetType() == typeof(DamSqlSvr))
                                    {
                                        // sqlClientの型情報を推論
                                        this.InferSQLType(dr[2].GetType(), out dbTypeInfo);
                                    }
                                    else if (this._dam.GetType() == typeof(DamODBC))
                                    {
                                        // ODBCの型情報を推論
                                        this.InferODBType(dr[2].GetType(), out dbTypeInfo);
                                    }
                                    else if (this._dam.GetType() == typeof(DamOraClient))
                                    {
                                        // Oracle Clientの型情報を推論
                                        this.InferORAType(dr[2].GetType(), out dbTypeInfo);
                                    }
                                    else if (this._dam.GetType() == typeof(DamManagedOdp))
                                    {
                                        // ODP.NETの型情報を推論
                                        this.InferODPType(dr[2].GetType(), out dbTypeInfo);
                                    }
#if NETCOREAPP
#else
                                    else if (this._dam.GetType() == typeof(DamOLEDB))
                                    {
                                        // OLEDBの型情報を推論
                                        this.InferOLEType(dr[2].GetType(), out dbTypeInfo);
                                    }
                                    else if (this._dam.GetType() == typeof(DamDB2))
                                    {
                                        // DB2.NETの型情報を推論
                                        this.InferDB2Type(dr[2].GetType(), out dbTypeInfo);
                                    }
                                    // HiRDBデータプロバイダ、MySQL Connector/NET、PostgreSQL Npgsql
                                    // については、推論不明（マニュアル無し）。推論非対応。
#endif

                                    // パラメタを設定（型情報・有）
                                    ((BaseDam)this._dam).SetParameter(dr[1].ToString(), dr[2], dbTypeInfo);
                                }
                                else
                                {
                                    // 型の推論（無）

                                    // パラメタを設定（型情報・無）
                                    ((BaseDam)this._dam).SetParameter(dr[1].ToString(), dr[2]);
                                }
                            }
                        }
                    }
                }

                #endregion

                // 2009/09/25-start

                // 性能測定－開始
                prfRec = new PerformanceRecorder();
                prfRec.StartsPerformanceRecord();

                // 2009/09/25-end

                if (this.cmbSelTxCtrl.SelectedItem.ToString() == Literal.COMMIT_MODE_AUTO)
                {
                    // トランザクションの開始
                    ((BaseDam)this._dam).BeginTransaction(this.SelectIso());
                }

                #region SQLの実行

                // 検索
                DataSet ds = new DataSet();

                if (this.cmbSelMethod.SelectedItem.ToString() == Literal.METHOD_DATA_TABLE)
                {
                    // DataTable

                    // DataTableを生成
                    DataTable dt = new DataTable();

                    // Select
                    ((BaseDam)this._dam).ExecSelectFill_DT(dt);

                    // DataSetに追加
                    ds.Tables.Add(dt);
                }
                else if (this.cmbSelMethod.SelectedItem.ToString() == Literal.METHOD_DATA_SET)
                {
                    // DataSet

                    // Select
                    ((BaseDam)this._dam).ExecSelectFill_DS(ds);

                    // DataTableが無い場合
                    if (ds.Tables.Count == 0)
                    {
                        // DataTableを生成
                        ds.Tables.Add(new DataTable());
                    }
                }
                else if (this.cmbSelMethod.SelectedItem.ToString() == Literal.METHOD_SCALAR)
                {
                    // Scalar

                    // DataTableを生成
                    DataTable dt = new DataTable();

                    // Select
                    object obj = ((BaseDam)this._dam).ExecSelectScalar();

                    // DataTableに設定する。
                    dt.Columns.Add("Scalar");
                    DataRow dr = dt.NewRow();
                    dr["Scalar"] = obj;
                    dt.Rows.Add(dr);

                    // DataSetに追加
                    ds.Tables.Add(dt);
                }
                else if (this.cmbSelMethod.SelectedItem.ToString() == Literal.METHOD_NON_QUERY)
                {
                    // NonQuery

                    // DataTableを生成
                    DataTable dt = new DataTable();

                    // insert、update、delete
                    int i = 0;

                    //if (hrdArryBind)
                    //{
                    //    // HiRDBの配列バインドの場合
                    //    i = ((DamHiRDB)this._dam).ExecInsUpDel_NonQuery((int)this.nudNumOfBind.Value);
                    //}
                    //else
                    //{
                    // HiRDBの配列バインドでない場合
                    i = ((BaseDam)this._dam).ExecInsUpDel_NonQuery();
                    //}

                    // DataTableに設定する。
                    dt.Columns.Add("Number of effect");
                    DataRow dr = dt.NewRow();
                    dr["Number of effect"] = i;
                    dt.Rows.Add(dr);

                    // DataSetに追加
                    ds.Tables.Add(dt);
                }
                else
                {
                    // ありえん
                }

                #endregion

                if (this.cmbSelTxCtrl.SelectedItem.ToString() == Literal.COMMIT_MODE_AUTO)
                {
                    // トランザクションのコミット
                    ((BaseDam)this._dam).CommitTransaction();
                }

                // 2009/09/25-start

                // 性能情報を出力
                log += this.RM_GetString("PerformanceExecution") + "\r\n";

                log += prfRec.EndsPerformanceRecord() + "\r\n\r\n";

                // 2009/09/25-end

                #region SQLの実行（DataReader独自）

                // DataReaderの場合は、トランザクションをコミットしてから
                if (this.cmbSelMethod.SelectedItem.ToString() == Literal.METHOD_DATA_READER)
                {
                    // DataReader

                    // Select
                    IDataReader idr = (IDataReader)((BaseDam)this._dam).ExecSelect_DR();

                    // DataTableのスキーマを取得
                    DataTable dtSchema = idr.GetSchemaTable();

                    // スキーマからDataTableを作成
                    DataTable dt = new DataTable();

                    if (dtSchema != null)
                    {
                        // スキーマあり（参照系のSQLでは、こちらになる）

                        foreach (DataRow drSchema in dtSchema.Rows)
                        {
                            dt.Columns.Add(drSchema[0].ToString());
                        }

                        // DataReaderからDataTableにデータを移動
                        while (idr.Read())
                        {
                            // DataRowに設定する。
                            DataRow dr = dt.NewRow();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                dr[dc] = idr[dc.ColumnName];
                            }

                            // DataTableに設定する。
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        // スキーマなし（更新系のSQLでは、こちらになる）
                    }

                    // 終了したらクローズ
                    idr.Close();

                    // DataSetに追加
                    ds.Tables.Add(dt);

                    //// マニュアルの場合はコミット or ロールバックが必要
                    //if (this.cmbSelTxCtrl.SelectedItem.ToString() == Literal.COMMIT_MODE_MANUAL)
                    //{
                    //    // MANUALの場合

                    //    // トランザクションをコミットするかを選択可能とする。
                    //    DialogResult dlgRet = MessageBox.Show(
                    //        "トランザクションをコミットします。",
                    //        "DataReaderの場合", MessageBoxButtons.YesNo);

                    //    if (dlgRet == DialogResult.Yes)
                    //    {
                    //        // トランザクションのコミット
                    //        //((BaseDam)this._dam).CommitTransaction();
                    //        this.btnCommitTx_Click(sender, e);

                    //        // メッセージを表示する
                    //        MessageBox.Show(Literal.MSG_TRANSACTION_COMMITED);
                    //    }
                    //    else if (dlgRet == DialogResult.No)
                    //    {
                    //        // トランザクションのロールバック
                    //        //((BaseDam)this._dam).RollbackTransaction();
                    //        this.btnRollbackTx_Click(sender, e);

                    //        // メッセージを表示する
                    //        MessageBox.Show(Literal.MSG_TRANSACTION_ROLLBACKED);
                    //    }
                    //    else
                    //    {
                    //        // ありえん
                    //    }
                    //}
                }

                #endregion

                #region 結果表示（正常時）

                // 2009/09/16-start

                // 【SQL】タブ

                // 実行したSQLを取得
                sql = ((BaseDam)this._dam).GetCurrentQuery();

                // 【LOG】タブ

                // ログ出力されるSQLを出力
                log += this.RM_GetString("LogOutputText") + "\r\n";

                log += ((BaseDam)this._dam).GetCurrentQueryForLog();

                // 2009/09/16-end

                // 取得した結果セットを表示
                foreach (DataTable dt in ds.Tables)
                {
                    Form2 viw = new Form2();

                    // プロパティを設定
                    viw._dt = dt;
                    viw._sql = sql;
                    viw._log = log;

                    // 表示位置
                    viw.StartPosition = FormStartPosition.CenterParent;

                    // 表示
                    viw.Show(this);
                }

                #endregion
            }
            catch (Exception Ex)
            {
                if (this.cmbSelTxCtrl.SelectedItem.ToString() == Literal.COMMIT_MODE_AUTO)
                {
                    // トランザクションのロールバック
                    ((BaseDam)this._dam).RollbackTransaction();
                }

                #region 結果表示（異常時）

                // メッセージを表示する
                MessageBox.Show(Ex.Message);

                // 一応結果を表示

                // 2009/09/16-start

                // 【SQL】タブ

                // 実行したSQLを取得
                sql = ((BaseDam)this._dam).GetCurrentQuery();

                // 【LOG】タブ

                // ログ出力されるSQLを出力
                log += this.RM_GetString("LogOutputText") + "\r\n";

                log += ((BaseDam)this._dam).GetCurrentQueryForLog();

                // 2009/09/16-end

                // エラーとなったSQLを表示
                Form2 viw = new Form2();

                // プロパティを設定
                viw._dt = new DataTable();
                viw._sql = sql;
                viw._log = log;

                // 表示位置
                viw.StartPosition = FormStartPosition.CenterParent;

                // 表示
                viw.Show(this);

                #endregion
            }
            finally
            {
                // 実行中はトランザクションの制御方式を変更できないようにする。
                this.groupBoxTx.Enabled = true;

                // クエリ実行完了
                ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_QUERY_EXECED");

            }
        }

        #region 分離レベルの選択

        /// <summary>分離レベルの選択メソッド</summary>
        /// <returns>分離レベルの列挙型</returns>
        private DbEnum.IsolationLevelEnum SelectIso()
        {
            if (this.cmbSelIso.SelectedItem.ToString() == Literal.ISO_LEVEL_NO_TRANSACTION)
            {
                return DbEnum.IsolationLevelEnum.NoTransaction;
            }
            else if (this.cmbSelIso.SelectedItem.ToString() == Literal.ISO_LEVEL_DEFAULT_TRANSACTION)
            {
                return DbEnum.IsolationLevelEnum.DefaultTransaction;
            }
            else if (this.cmbSelIso.SelectedItem.ToString() == Literal.ISO_LEVEL_READ_UNCOMMITTED)
            {
                return DbEnum.IsolationLevelEnum.ReadUncommitted;
            }
            else if (this.cmbSelIso.SelectedItem.ToString() == Literal.ISO_LEVEL_READ_COMMITTED)
            {
                return DbEnum.IsolationLevelEnum.ReadCommitted;
            }
            else if (this.cmbSelIso.SelectedItem.ToString() == Literal.ISO_LEVEL_REPEATABLE_READ)
            {
                return DbEnum.IsolationLevelEnum.RepeatableRead;
            }
            else if (this.cmbSelIso.SelectedItem.ToString() == Literal.ISO_LEVEL_SERIALIZABLE)
            {
                return DbEnum.IsolationLevelEnum.Serializable;
            }
            else if (this.cmbSelIso.SelectedItem.ToString() == Literal.ISO_LEVEL_SNAPSHOT)
            {
                return DbEnum.IsolationLevelEnum.Snapshot;
            }
            // デバッグ用-start
            else if (this.cmbSelIso.SelectedItem.ToString() == Literal.ISO_LEVEL_USER)
            {
                return DbEnum.IsolationLevelEnum.User;
            }
            else if (this.cmbSelIso.SelectedItem.ToString() == Literal.ISO_LEVEL_NOT_CONNECT)
            {
                return DbEnum.IsolationLevelEnum.NotConnect;
            }
            // デバッグ用-end
            else
            {
                // ありえん
                return DbEnum.IsolationLevelEnum.NoTransaction;
            }
        }

        #endregion

        #region 型の推論（デバッグ）

        #region sqlClient

        /// <summary>
        /// .NETの型情報（System.Type）から
        /// sqlClientの型情報（SqlDbType）を推論する。
        /// </summary>
        /// <param name="type">.NETの型情報（System.Type）</param>
        /// <param name="sqlDbType">sqlClientの型情報（SqlDbType）</param>
        /// <returns>推論できた：true、推論できなかった：false</returns>
        private bool InferSQLType(Type type, out object sqlDbType)
        {
            // http://msdn.microsoft.com/ja-jp/library/yy6y35y8.aspx

            if (type == typeof(Boolean))
            {
                sqlDbType = SqlDbType.Bit;
                return true;
            }
            else if (type == typeof(Byte))
            {
                sqlDbType = SqlDbType.TinyInt;
                return true;
            }
            else if (type == typeof(Byte[]))
            {
                sqlDbType = SqlDbType.VarBinary;
                return true;
            }
            else if (type == typeof(DateTime))
            {
                sqlDbType = SqlDbType.DateTime;
                return true;
            }
            //else if (type == typeof(DateTimeOffset))
            //{
            //    sqlDbType = SqlDbType.DateTimeOffset;
            //    return true;
            //}
            else if (type == typeof(Decimal))
            {
                sqlDbType = SqlDbType.Decimal;
                return true;
            }
            else if (type == typeof(Double))
            {
                sqlDbType = SqlDbType.Float;
                return true;
            }
            else if (type == typeof(Single))
            {
                sqlDbType = SqlDbType.Real;
                return true;
            }
            //else if (type == typeof(Guid))
            //{
            //    sqlDbType = SqlDbType.UniqueIdentifier;
            //    return true;
            //}
            else if (type == typeof(Int16))
            {
                sqlDbType = SqlDbType.SmallInt;
                return true;
            }
            else if (type == typeof(Int32))
            {
                sqlDbType = SqlDbType.Int;
                return true;
            }
            else if (type == typeof(Int64))
            {
                sqlDbType = SqlDbType.BigInt;
                return true;
            }
            //else if (type == typeof(Object))
            //{
            //    sqlDbType = SqlDbType.Variant;
            //    return true;
            //}
            else if (type == typeof(String))
            {
                sqlDbType = SqlDbType.NVarChar;
                return true;
            }

            sqlDbType = null;
            return false;
        }

        #endregion

#if NETCOREAPP
#else
        #region OLEDB

        /// <summary>
        /// .NETの型情報（System.Type）から
        /// OLEDBの型情報（OleDbType）を推論する。
        /// </summary>
        /// <param name="type">.NETの型情報（System.Type）</param>
        /// <param name="oleDbType">OLEDBの型情報（OleDbType）</param>
        /// <returns>推論できた：true、推論できなかった：false</returns>
        private bool InferOLEType(Type type, out object oleDbType)
        {
            // http://msdn.microsoft.com/ja-jp/library/yy6y35y8(v=vs.110).aspx

            if (type == typeof(Boolean))
            {
                oleDbType = OleDbType.Boolean;
                return true;
            }
            else if (type == typeof(Byte))
            {
                oleDbType = OleDbType.UnsignedTinyInt;
                return true;
            }
            else if (type == typeof(Byte[]))
            {
                oleDbType = OleDbType.VarBinary;
                return true;
            }
            else if (type == typeof(Char))
            {
                oleDbType = OleDbType.Char;
                return true;
            }
            //else if (type == typeof(Char[]))
            //{
            //    oleDbType = OleDbType.;
            //    return true;
            //}
            else if (type == typeof(DateTime))
            {
                oleDbType = OleDbType.DBTimeStamp;
                return true;
            }
            //else if (type == typeof(DateTimeOffset))
            //{
            //    oleDbType = OleDbType.;
            //    return true;
            //}
            else if (type == typeof(Decimal))
            {
                oleDbType = OleDbType.Decimal;
                return true;
            }
            else if (type == typeof(Double))
            {
                oleDbType = OleDbType.Double;
                return true;
            }
            else if (type == typeof(Single))
            {
                oleDbType = OleDbType.Single;
                return true;
            }
            else if (type == typeof(Guid))
            {
                oleDbType = OleDbType.Guid;
                return true;
            }
            else if (type == typeof(Int16))
            {
                oleDbType = OleDbType.SmallInt;
                return true;
            }
            else if (type == typeof(Int32))
            {
                oleDbType = OleDbType.Integer;
                return true;
            }
            else if (type == typeof(Int64))
            {
                oleDbType = OleDbType.BigInt;
                return true;
            }
            else if (type == typeof(Object))
            {
                oleDbType = OleDbType.Variant;
                return true;
            }
            else if (type == typeof(String))
            {
                oleDbType = OleDbType.VarWChar;
                return true;
            }
            else if (type == typeof(TimeSpan))
            {
                oleDbType = OleDbType.DBTime;
                return true;
            }
            else if (type == typeof(UInt16))
            {
                oleDbType = OleDbType.UnsignedSmallInt;
                return true;
            }
            else if (type == typeof(UInt32))
            {
                oleDbType = OleDbType.UnsignedInt;
                return true;
            }
            else if (type == typeof(UInt64))
            {
                oleDbType = OleDbType.UnsignedBigInt;
                return true;
            }

            oleDbType = null;
            return false;
        }

        #endregion
#endif

        #region ODBC

        /// <summary>
        /// .NETの型情報（System.Type）から
        /// ODBCの型情報（OdbcType）を推論する。
        /// </summary>
        /// <param name="type">.NETの型情報（System.Type）</param>
        /// <param name="odbcType">ODBCの型情報（OdbcType）</param>
        /// <returns>推論できた：true、推論できなかった：false</returns>
        private bool InferODBType(Type type, out object odbcType)
        {
            // http://msdn.microsoft.com/ja-jp/library/yy6y35y8(v=vs.110).aspx

            if (type == typeof(Boolean))
            {
                odbcType = OdbcType.Bit;
                return true;
            }
            else if (type == typeof(Byte))
            {
                odbcType = OdbcType.TinyInt;
                return true;
            }
            else if (type == typeof(Byte[]))
            {
                odbcType = OdbcType.Binary;
                return true;
            }
            else if (type == typeof(Char))
            {
                odbcType = OdbcType.Char;
                return true;
            }
            //else if (type == typeof(Char[]))
            //{
            //    odbcType = OdbcType.;
            //    return true;
            //}
            else if (type == typeof(DateTime))
            {
                odbcType = OdbcType.DateTime;
                return true;
            }
            //else if (type == typeof(DateTimeOffset))
            //{
            //    odbcType = OdbcType.;
            //    return true;
            //}
            else if (type == typeof(Decimal))
            {
                odbcType = OdbcType.Numeric;
                return true;
            }
            else if (type == typeof(Double))
            {
                odbcType = OdbcType.Double;
                return true;
            }
            else if (type == typeof(Single))
            {
                odbcType = OdbcType.Real;
                return true;
            }
            else if (type == typeof(Guid))
            {
                odbcType = OdbcType.UniqueIdentifier;
                return true;
            }
            else if (type == typeof(Int16))
            {
                odbcType = OdbcType.SmallInt;
                return true;
            }
            else if (type == typeof(Int32))
            {
                odbcType = OdbcType.Int;
                return true;
            }
            else if (type == typeof(Int64))
            {
                odbcType = OdbcType.Numeric;
                return true;
            }
            //else if (type == typeof(Object))
            //{
            //    odbcType = OdbcType.;
            //    return true;
            //}
            else if (type == typeof(String))
            {
                odbcType = OdbcType.NVarChar;
                return true;
            }
            else if (type == typeof(TimeSpan))
            {
                odbcType = OdbcType.Time;
                return true;
            }
            else if (type == typeof(UInt16))
            {
                odbcType = OdbcType.Int;
                return true;
            }
            else if (type == typeof(UInt32))
            {
                odbcType = OdbcType.BigInt;
                return true;
            }
            else if (type == typeof(UInt64))
            {
                odbcType = OdbcType.Numeric;
                return true;
            }

            odbcType = null;
            return false;
        }

        #endregion

        #region Oracle Client

        /// <summary>
        /// .NETの型情報（System.Type）から
        /// Oracle Clientの型情報（OracleType）を推論する。
        /// </summary>
        /// <param name="type">.NETの型情報（System.Type）</param>
        /// <param name="oracleType">Oracle Clientの型情報（OracleType）</param>
        /// <returns>推論できた：true、推論できなかった：false</returns>
        private bool InferORAType(Type type, out object oracleType)
        {
            // http://msdn.microsoft.com/ja-jp/library/yy6y35y8(v=vs.110).aspx

            if (type == typeof(Boolean))
            {
                oracleType = DntOraClient.OracleType.Byte;
                return true;
            }
            else if (type == typeof(Byte))
            {
                oracleType = DntOraClient.OracleType.Byte;
                return true;
            }
            else if (type == typeof(Byte[]))
            {
                oracleType = DntOraClient.OracleType.Raw;
                return true;
            }
            else if (type == typeof(Char))
            {
                oracleType = DntOraClient.OracleType.Byte;
                return true;
            }
            //else if (type == typeof(Char[]))
            //{
            //    oracleType = DntOraClient.OracleType.;
            //    return true;
            //}
            else if (type == typeof(DateTime))
            {
                oracleType = DntOraClient.OracleType.DateTime;
                return true;
            }
            else if (type == typeof(DateTimeOffset))
            {
                oracleType = DntOraClient.OracleType.DateTime;
                return true;
            }
            else if (type == typeof(Decimal))
            {
                oracleType = DntOraClient.OracleType.Number;
                return true;
            }
            else if (type == typeof(Double))
            {
                oracleType = DntOraClient.OracleType.Double;
                return true;
            }
            else if (type == typeof(Single))
            {
                oracleType = DntOraClient.OracleType.Float;
                return true;
            }
            else if (type == typeof(Guid))
            {
                oracleType = DntOraClient.OracleType.Raw;
                return true;
            }
            else if (type == typeof(Int16))
            {
                oracleType = DntOraClient.OracleType.Int16;
                return true;
            }
            else if (type == typeof(Int32))
            {
                oracleType = DntOraClient.OracleType.Int32;
                return true;
            }
            else if (type == typeof(Int64))
            {
                oracleType = DntOraClient.OracleType.Number;
                return true;
            }
            else if (type == typeof(Object))
            {
                oracleType = DntOraClient.OracleType.Blob;
                return true;
            }
            else if (type == typeof(String))
            {
                oracleType = DntOraClient.OracleType.NVarChar;
                return true;
            }
            else if (type == typeof(TimeSpan))
            {
                oracleType = DntOraClient.OracleType.DateTime;
                return true;
            }
            else if (type == typeof(UInt16))
            {
                oracleType = DntOraClient.OracleType.UInt16;
                return true;
            }
            else if (type == typeof(UInt32))
            {
                oracleType = DntOraClient.OracleType.UInt32;
                return true;
            }
            else if (type == typeof(UInt64))
            {
                oracleType = DntOraClient.OracleType.Number;
                return true;
            }

            oracleType = null;
            return false;
        }

        #endregion

        #region ODP.NET

        /// <summary>
        /// .NETの型情報（System.Type）から
        /// ODP.NETの型情報（OracleDbType）を推論する。
        /// </summary>
        /// <param name="type">.NETの型情報（System.Type）</param>
        /// <param name="oracleDbType">ODP.NETの型情報（OracleDbType）</param>
        /// <returns>推論できた：true、推論できなかった：false</returns>
        private bool InferODPType(Type type, out object oracleDbType)
        {
            // http://otndnld.oracle.co.jp/document/products/oracle10g/
            // 101/doc_v12/win.101/B15519-01/featOraCommand.htm

            if (type == typeof(Byte))
            {
                oracleDbType = OdpOraClient.OracleDbType.Byte;
                return true;
            }
            else if (type == typeof(Byte[]))
            {
                oracleDbType = OdpOraClient.OracleDbType.Raw;
                return true;
            }
            else if (type == typeof(Char))
            {
                oracleDbType = OdpOraClient.OracleDbType.Varchar2;
                return true;
            }
            else if (type == typeof(Char[]))
            {
                oracleDbType = OdpOraClient.OracleDbType.Varchar2;
                return true;
            }

            else if (type == typeof(DateTime))
            {
                oracleDbType = OdpOraClient.OracleDbType.TimeStamp;
                return true;
            }
            else if (type == typeof(Decimal))
            {
                oracleDbType = OdpOraClient.OracleDbType.Decimal;
                return true;
            }
            else if (type == typeof(Double))
            {
                oracleDbType = OdpOraClient.OracleDbType.Double;
                return true;
            }
            else if (type == typeof(Int16))
            {
                oracleDbType = OdpOraClient.OracleDbType.Int16;
                return true;
            }
            else if (type == typeof(Int32))
            {
                oracleDbType = OdpOraClient.OracleDbType.Int32;
                return true;
            }
            else if (type == typeof(Int64))
            {
                oracleDbType = OdpOraClient.OracleDbType.Int64;
                return true;
            }
            else if (type == typeof(Single))
            {
                oracleDbType = OdpOraClient.OracleDbType.Single;
                return true;
            }
            else if (type == typeof(String))
            {
                oracleDbType = OdpOraClient.OracleDbType.Varchar2;
                return true;
            }
            //else if (type == typeof(TimeSpan))
            //{
            //    oracleDbType = OdpOraClient.OracleDbType.IntervalDS;
            //    return true;
            //}

            oracleDbType = null;
            return false;
        }

        #endregion

#if NETCOREAPP
#else
        #region DB2.NET

        /// <summary>
        /// .NETの型情報（System.Type）から
        /// DB2.NETの型情報（DB2Type）を推論する。
        /// </summary>
        /// <param name="type">.NETの型情報（System.Type）</param>
        /// <param name="db2DbType">DB2.NETの型情報（DB2Type）</param>
        /// <returns>推論できた：true、推論できなかった：false</returns>
        private bool InferDB2Type(Type type, out object db2DbType)
        {
            // http://publib.boulder.ibm.com/infocenter/db2luw/v8/index.jsp?
            // topic=/com.ibm.db2.udb.dndp.doc/htm/frlrfIBMDataDB2DB2TypeClassTopic.htm

            if (type == typeof(Int16))
            {
                db2DbType = DB2Type.SmallInt;
                return true;
            }
            else if (type == typeof(Int32))
            {
                db2DbType = DB2Type.Integer;
                return true;
            }
            else if (type == typeof(Int64))
            {
                db2DbType = DB2Type.BigInt;
                return true;
            }
            else if (type == typeof(Single))
            {
                db2DbType = DB2Type.Real;
                return true;
            }
            else if (type == typeof(Double))
            {
                db2DbType = DB2Type.Double;
                return true;
            }
            else if (type == typeof(Decimal))
            {
                db2DbType = DB2Type.Decimal;
                return true;
            }
            else if (type == typeof(DateTime))
            {
                db2DbType = DB2Type.Date;
                return true;
            }
            //else if (type == typeof(TimeSpan))
            //{
            //    db2DbType = DB2Type.Time;
            //    return true;
            //}
            else if (type == typeof(String))
            {
                db2DbType = DB2Type.VarChar;
                return true;
            }
            else if (type == typeof(Byte[]))
            {
                db2DbType = DB2Type.Binary;
                return true;
            }

            db2DbType = null;
            return false;
        }

        #endregion
#endif

        // HiRDBデータ プロバイダは情報が見当たらず

        // MySQL Connector/NETは情報が見当たらず

        // PostgreSQL Npgsqlは情報が見当たらず

        #endregion

        #endregion

        #region コネクション クローズ

        /// <summary>コネクション クローズ </summary>
        private void btnCnClose_Click(object sender, EventArgs e)
        {
            try
            {
                // コネクション クローズ
                ((BaseDam)this._dam).ConnectionClose();

                // ボタンの状態を変更

                // 設定関連
                this.cmbDataProvider.Enabled = true;
                this.txtCnnStr.Enabled = true;

                this.btnCreateConfig.Enabled = true;

                this.cmbSaveSlot.Enabled = true;
                this.btnSaveConfig.Enabled = true;
                this.btnLoadConfig.Enabled = true;

                // SQL実行ボタン
                this.btnExecQuery.Enabled = false;

                // コネクションボタン
                this.btnCnOpen.Enabled = true;
                this.btnCnClose.Enabled = false;

                // トランザクション系
                this.cmbSelIso.Enabled = false;
                this.cmbSelTxCtrl.Enabled = false;
                this.btnBeginTx.Enabled = false;
                this.btnCommitTx.Enabled = false;
                this.btnRollbackTx.Enabled = false;

                // 状態
                ((ToolStripStatusLabel)this.statBar.Items[0]).Text = this.RM_GetString("STATUS_CONNECTION_CLOSED");

            }
            catch (Exception Ex)
            {
                // メッセージを表示する
                MessageBox.Show(Ex.Message);
            }
        }

        #endregion

        #region ショートカット操作の実装

        /// <summary>ショートカット操作の実装</summary>
        private void txtSQL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt)
            {
                if (e.KeyCode == Keys.O)
                {
                    // O が押されたら、開く。

                    // 開くボタンのクリック イベントを発生させる。
                    this.btnOpenQueryFile.PerformClick();
                }
                else if (e.KeyCode == Keys.S)
                {
                    // S が押されたら、セーブ

                    if (this._loadFilePath == "")
                    {
                        // ファイルが開かれていない状態
                        // ファイル保存のボタンのクリック イベントを発生させる。
                        this.btnSaveQueryFile.PerformClick();
                    }
                    else
                    {
                        // ファイルが開かれている状態
                        // 上書き保存のボタンのクリック イベントを発生させる。
                        this.btnOverwriteQueryFile.PerformClick();
                    }
                }
                else if (e.KeyCode == Keys.C)
                {
                    // C が押されたら、閉じる。

                    if (this._loadFilePath == "")
                    {
                        // ファイルが開かれていない状態
                        // なにもしない。
                    }
                    else
                    {
                        // ファイルが開かれている状態
                        // 閉じるボタンのクリック イベントを発生させる。
                        this.btnCloseQueryFile.PerformClick();
                    }
                }
                else if (e.KeyCode == Keys.E)
                {
                    // E が押されたら、クエリを実行する。

                    // クエリ実行ボタンのクリック イベントを発生させる。
                    this.btnExecQuery.PerformClick();
                }
            }
        }

        #endregion

        #region IDE機能

        /// <summary>IDE機能のマルチプルイベント</summary>
        private void TSMI_Click(object sender, EventArgs e)
        {
            // キーを取得
            string key = ((ToolStripMenuItem)sender).Name.Replace("_TSMI", "");

            // 値を取得
            try
            {
                // xxx1, xxx2, DIV, COMMENT, CDATA
                this.txtSQL.SelectedText =
                    global::DPQuery_Tool.Properties.Resources.ResourceManager.GetString(key);
            }
            catch
            {
                switch (key)
                {
                    #region IF-ELSEの組み合わせ

                    case "IF_ELSE":
                        // テキスト内
                        this.txtSQL.SelectedText =
                            global::DPQuery_Tool.Properties.Resources.IF1
                            + global::DPQuery_Tool.Properties.Resources.ELSE1
                            + global::DPQuery_Tool.Properties.Resources.ELSE2
                            + global::DPQuery_Tool.Properties.Resources.IF2;
                        break;
                    case "IF_TXT":
                        // テキスト内
                        this.txtSQL.SelectedText =
                            global::DPQuery_Tool.Properties.Resources.IF1
                            + global::DPQuery_Tool.Properties.Resources.IF2;
                        break;
                    case "IF_TXT1":
                        // テキスト内
                        this.txtSQL.SelectedText =
                            global::DPQuery_Tool.Properties.Resources.IF1;
                        break;
                    case "IF_TXT2":
                        // テキスト内
                        this.txtSQL.SelectedText =
                            global::DPQuery_Tool.Properties.Resources.IF2;
                        break;
                    case "IF_TAG":
                        // タグ内
                        this.txtSQL.SelectedText =
                            global::DPQuery_Tool.Properties.Resources.IF3
                            + global::DPQuery_Tool.Properties.Resources.IF2;
                        break;
                    case "IF_TAG1":
                        // タグ内
                        this.txtSQL.SelectedText =
                            global::DPQuery_Tool.Properties.Resources.IF3;
                        break;
                    case "IF_TAG2":
                        // タグ内
                        this.txtSQL.SelectedText =
                            global::DPQuery_Tool.Properties.Resources.IF2;
                        break;

                    #endregion

                    default:

                        if (key == "PARAM" || key == "CPARAM")
                        {
                            // xxx →　xxx1 p1, Int64, 1 xxx2
                            this.txtSQL.SelectedText =
                                global::DPQuery_Tool.Properties.Resources.ResourceManager.GetString(key + "1")
                                + " p1, Int64, 1 "
                                + global::DPQuery_Tool.Properties.Resources.ResourceManager.GetString(key + "2");
                        }
                        else
                        {
                            // xxx →　xxx1 + xxx2
                            this.txtSQL.SelectedText =
                                global::DPQuery_Tool.Properties.Resources.ResourceManager.GetString(key + "1")
                                + global::DPQuery_Tool.Properties.Resources.ResourceManager.GetString(key + "2");
                        }

                        break;
                }
            }
        }

        #endregion

        /// <summary>RM_GetString</summary>
        /// <param name="key">string</param>
        /// <returns>RM string</returns>
        private string RM_GetString(string key)
        {
            //get the string value from resource file  by proper passing key.
            ResourceManager rm = Resources.Resource.ResourceManager;
            return rm.GetString(key);
        }
    }
}
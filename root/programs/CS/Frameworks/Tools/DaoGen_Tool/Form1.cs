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
//* クラス日本語名  ：D層自動生成ツール（墨壺） - D層定義情報ファイル生成画面
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2008/xx/xx  西野 大介         新規作成
//*  2009/12/09  西野 大介         主キー取得機能の追加
//*  2009/12/09  西野 大介         カラム名をキーにしたハッシュテーブルを追加
//*  2009/12/09  西野 大介         ODP.NETにて、テーブルとインデックスの
//*                                カラム ポジション不一致により、主キー取得に失敗する。
//*                                （主キー位置がテーブルの中間から定義されている場合）
//*  2010/02/18  西野 大介         データプロバイダ追加（HiRDB、PostgreSQL）
//*  2012/02/09  西野 大介         HiRDBデータプロバイダのコメントアウト（（ソフト）対応せず）
//*  2012/02/09  西野 大介         OLEDB、ODBCのデータプロバイダ対応
//*  2012/08/21  西野 大介         データプロバイダ設定の引き継ぎ処理を追加
//*  2012/08/21  西野 大介         リストボックスのソート処理を追加
//*  2012/08/21  西野 大介         SQL Serverの主キー設定機能で別スキーマ上の
//*                                テーブルが見つからない場合があったため対応した。
//*  2012/11/21  西野 大介         Entity、DataSet自動生成の対応
//*  2012/11/21  西野 大介         デフォルト・エンコーディングをSJISに指定
//*  2012/12/21  西野 大介         HiRDB戻し検討・・・
//*  2014/01/20  西野 大介         I/O時のエンコーディング制御方式を見直し。
//*  2014/04/30  Santosh san       Internationalization:
//*                                Added Method to get the strings from the resource files based on the keys values passed.
//*                                and and replaced this method wherever hard coded values.
//*  2014/08/19  西野 大介         カラム取得時のスキーマ考慮が無かったため追加（奥井さんからの提供）
//*  2017/09/06  西野 大介         Oracle.ManagedDataAccess.Clientで主キーが取れなくなった対応
//*  2018/10/29  西野 大介         NETCOREAPP対応で、サポートされないDBを「#if」した。
//*  2026/07/31  ＸＸ ＸＸ         GUIのCUI化（イベント ハンドラのロジックを関数化）
//*  2026/08/01  ＸＸ ＸＸ         SQL Serverの主キー取得を、非公開のストアド プロシージャ
//*                                （sp_helpconstraint／sp_MShelpindex）からカタログ ビューに変更
//**********************************************************************************

// --------------------
// データプロバイダ
// --------------------
using System.Data.Odbc;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using MySql.Data.MySqlClient;

#if NETCOREAPP
using Npgsql;
#else
using System.Data.OleDb;
//using System.Data.OracleClient; // ODP.NETに移行
//using Oracle.DataAccess.Client; // Managedに移行
//using IBM.Data.DB2;
//using Hitachi.HiRDB;
#endif
// --------------------

using System;
using System.IO;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Resources;
using System.Drawing;
using System.Windows.Forms;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace DaoGen_Tool
{
    /// <summary>墨壺 - D層定義情報ファイル生成画面</summary>
    public partial class Form1 : Form
    {
        #region インスタンス変数

        /// <summary>データ・プロバイダ</summary>
        string Dap = "";

        #region コネクション

        /// <summary>SqlConnection</summary>
        private SqlConnection SqlCn;
        /// <summary>OdbcConnection</summary>
        private OdbcConnection OdbCn;
        /// <summary>OracleConnection</summary>
        private OracleConnection OdpCn;
        /// <summary>MySqlConnection</summary>
        private MySqlConnection MySqlCn;


#if NETCOREAPP
        /// <summary>NpgsqlConnection</summary>
        private NpgsqlConnection NpgsqlCn;
#else
        /// <summary>OleDbConnection</summary>
        private OleDbConnection OleCn;
        /// <summary>DB2Connection</summary>
        //private DB2Connection DB2Cn;
        /// <summary>HiRDBConnection</summary>
        //private HiRDBConnection HiRDBCn;
#endif

        #endregion

        #region スキーマ情報

        /// <summary>スキーマの情報（汎用）</summary>
        private DataTable DtSchma;

        /// <summary>スキーマの情報（カスタム）</summary>
        private Hashtable HtSchemaCustom;

        #region 取得したスキーマ情報（デバッグ表示用）

        // ↓↓↓【追加】↓↓↓
        // イベント ハンドラから外に出した関数の取得結果を、
        // イベント ハンドラ側でデバッグ表示するために保持する。

        /// <summary>スキーマの情報（テーブル）</summary>
        private DataTable DtSchmaTables;

        /// <summary>スキーマの情報（ビュー）</summary>
        private DataTable DtSchmaViews;

        /// <summary>スキーマの情報（カラム）</summary>
        private DataTable DtSchmaColumns;

        /// <summary>スキーマの情報（主キー）</summary>
        private DataTable DtSchmaPrimaryKeys;

        /// <summary>スキーマの情報（インデックス カラム）</summary>
        private DataTable DtSchmaIndexColumns;

        // ↑↑↑【追加】↑↑↑

        #endregion

        #endregion

        #region CUI（ヘッドレス実行）用

        // ↓↓↓【追加】↓↓↓

        /// <summary>接続文字列</summary>
        /// <remarks>
        /// GUI 起動時はイベント ハンドラが txtConnString から、
        /// CUI 起動時は DaoDefinitionGen がコマンドライン引数から設定する。
        /// </remarks>
        private string ConnString = "";

        /// <summary>CUI（非対話）で起動しているか</summary>
        /// <remarks>true の場合、MessageBox の代わりに標準出力へメッセージを出す。</remarks>
        public bool IsCui = false;

        // ↑↑↑【追加】↑↑↑

        #endregion

        #region Help Information

        /// <summary>strEnglishHelpDoc</summary>
        string strEnglishHelpDoc;
        /// <summary>strJapaneseHelpDoc</summary>
        string strJapaneseHelpDoc;

        #endregion

        #endregion

        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>初期設定（load）</summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // SQLClientデータプロバイダを初期設定とする。
            this.rbnSQL.Select();

            // エンコーディングをコンボ ボックスのアイテムに一括設定する。
            // デフォルトのエンコーディングは、utf-8とする。
            this.cmbEncoding.ValueMember = "key";
            this.cmbEncoding.DisplayMember = "value";
            this.cmbEncoding.DataSource = CustomEncode.GetEncodings();
            this.cmbEncoding.SelectedValue = 65001;

            // コンボを初期化する。
            this.Init_cmb();

            if (System.Diagnostics.Debugger.IsAttached || (Environment.GetCommandLineArgs().Length > 1 && Environment.GetCommandLineArgs()[1].ToUpper() == "DBG"))
            {
                // デバッグ実行のときは、リサイズ可とする
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
            else
            {
                // デバッグなしで実行するときは、リサイズ不可とする
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
            }

            // ボタン状態の変更
            this.btnDelTable.Enabled = false;
            this.btnLoadColumn.Enabled = false;
            this.btnSetPrimaryKey.Enabled = false;

            this.btnDaoDefinitionGen.Enabled = false;

            // ↓↓↓【変更】↓↓↓
            // このビルドでサポートされないデータ プロバイダは、選択できないようにする。
            // ※ DB2・HiRDBは、両ビルドともコードがコメントアウトされているためサポートされない。
#if NETCOREAPP
            // OLEDBは、NETCOREAPPではサポートされない。
            this.rbnOLE.Enabled = false;
            this.rbnDB2.Enabled = false;
            this.rbnHiRDB.Enabled = false;
#else
            // PostgreSQLは、NETCOREAPPでのみサポートされる。
            this.rbnPstgrs.Enabled = false;
            this.rbnDB2.Enabled = false;
            this.rbnHiRDB.Enabled = false;
#endif
            // ↑↑↑【変更】↑↑↑
        }

        #endregion

        #region メニューの処理

        /// <summary>やり直す（再起動）</summary>
        private void Restart_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        /// <summary>閉じる</summary>
        private void Close_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 共通関数

        #region メッセージ表示

        // ↓↓↓【追加】↓↓↓

        /// <summary>メッセージを表示する（GUI:MessageBox／CUI:標準出力）</summary>
        /// <param name="message">メッセージ</param>
        private void ShowMessage(string message)
        {
            if (this.IsCui)
            {
                Console.WriteLine(message);
            }
            else
            {
                MessageBox.Show(message);
            }
        }

        /// <summary>メッセージを表示する（GUI:MessageBox／CUI:標準出力）</summary>
        /// <param name="message">メッセージ</param>
        /// <param name="caption">キャプション</param>
        private void ShowMessage(string message, string caption)
        {
            if (this.IsCui)
            {
                Console.WriteLine(caption + " " + message);
            }
            else
            {
                MessageBox.Show(message, caption);
            }
        }

        // ↑↑↑【追加】↑↑↑

        #endregion

        #region コネクション オープン クローズ

        /// <summary>
        /// コネクション オープン
        /// </summary>
        /// <remarks>
        /// CUI からも呼び出せるよう、UIコントロール（rbnXxx、txtConnString）ではなく、
        /// this.Dap、this.ConnString を参照する。
        /// </remarks>
        private void ConnectionOpen()
        {
            if (this.Dap == "SQL")
            {
                this.SqlCn = new SqlConnection(this.ConnString);
                this.SqlCn.Open();
            }
            else if (this.Dap == "ODB")
            {
                this.OdbCn = new OdbcConnection(this.ConnString);
                this.OdbCn.Open();
            }
            else if (this.Dap == "ODP")
            {
                this.OdpCn = new OracleConnection(this.ConnString);
                this.OdpCn.Open();
            }
            else if (this.Dap == "MCN")
            {
                this.MySqlCn = new MySqlConnection(this.ConnString);
                this.MySqlCn.Open();
            }
#if NETCOREAPP
            else if (this.Dap == "NPS")
            {
                this.NpgsqlCn = new NpgsqlConnection(this.ConnString);
                this.NpgsqlCn.Open();
            }
#else
            else if (this.Dap == "OLE")
            {
                this.OleCn = new OleDbConnection(this.ConnString);
                this.OleCn.Open();
            }
            /*else if (this.Dap == "DB2")
            {
                this.DB2Cn = new DB2Connection(this.ConnString);
                this.DB2Cn.Open();
            }
            else if (this.Dap == "HIR")
            {
                this.HiRDBCn = new HiRDBConnection(this.ConnString);
                this.HiRDBCn.Open();
            }*/
#endif
            else
            {
                // ↓↓↓【変更】↓↓↓
                // データプロバイダ指定無し、または、このビルドでサポートされないデータ プロバイダ。
                // 以前は何もしなかったため、コネクションがnullのまま後続処理へ進み、
                // NullReferenceExceptionになっていた。呼び出し元のcatchで処理できるよう、例外を送出する。
                throw new NotSupportedException(string.Format(
                    "データ プロバイダ[{0}]は、このビルドではサポートされていません。", this.Dap));
                // ↑↑↑【変更】↑↑↑
            }
        }

        /// <summary>
        /// コネクション クローズ
        /// </summary>
        private void ConnectionClose()
        {
            // 全部閉じる

            if (this.SqlCn != null)
            {
                this.SqlCn.Close();
            }

            if (this.OdbCn != null)
            {
                this.OdbCn.Close();
            }

            if (this.OdpCn != null)
            {
                this.OdpCn.Close();
            }

            if (this.MySqlCn != null)
            {
                this.MySqlCn.Close();
            }

#if NETCOREAPP
            if (this.NpgsqlCn != null)
            {
                this.NpgsqlCn.Close();
            }
#else
            if (this.OleCn != null)
            {
                this.OleCn.Close();
            }

            /*if (this.DB2Cn != null)
            {
                this.DB2Cn.Close();
            }

            if (this.HiRDBCn != null)
            {
                this.HiRDBCn.Close();
            }*/
#endif
        }

        #endregion

        #region ラジオボタン変更（接続文字列変更

        /// <summary>SQLの場合</summary>
        private void rdbSQL_CheckedChanged(object sender, EventArgs e)
        {
            // 接続文字列を初期化
            if (this.rbnSQL.Checked)
            {
                this.Dap = "SQL";
                this.txtConnString.Text = GetConfigParameter.GetConfigValue("ConnectionString_SQL");
            }

            // コンボを初期化する。
            this.Init_cmb();
        }

        /// <summary>OLEDB.NETの場合</summary>
        private void rbnOLE_CheckedChanged(object sender, EventArgs e)
        {
            // 接続文字列を初期化 
            if (this.rbnOLE.Checked)
            {
                this.Dap = "OLE";
                this.txtConnString.Text = GetConfigParameter.GetConfigValue("ConnectionString_OLE");
            }

            // コンボを初期化する。
            this.Init_cmb();
        }

        /// <summary>ODBC.NETの場合</summary>
        private void rbnODB_CheckedChanged(object sender, EventArgs e)
        {
            // 接続文字列を初期化 
            if (this.rbnODB.Checked)
            {
                this.Dap = "ODB";
                this.txtConnString.Text = GetConfigParameter.GetConfigValue("ConnectionString_ODBC");
            }

            // コンボを初期化する。
            this.Init_cmb();
        }

        /// <summary>ODPの場合</summary>
        private void rdbODP_CheckedChanged(object sender, EventArgs e)
        {
            // 接続文字列を初期化
            if (this.rbnODP.Checked)
            {
                this.Dap = "ODP";
                this.txtConnString.Text = GetConfigParameter.GetConfigValue("ConnectionString_ODP");
            }

            // コンボを初期化する。
            this.Init_cmb();
        }

        /// <summary>DB2の場合</summary>
        private void rdbDB2_CheckedChanged(object sender, EventArgs e)
        {
            // 接続文字列を初期化 
            if (this.rbnDB2.Checked)
            {
                this.Dap = "DB2";
                this.txtConnString.Text = GetConfigParameter.GetConfigValue("ConnectionString_DB2");
            }

            // コンボを初期化する。
            this.Init_cmb();
        }

        /// <summary>HiRDBの場合</summary>
        private void rbnHiRDB_CheckedChanged(object sender, EventArgs e)
        {
            // 接続文字列を初期化 
            if (this.rbnHiRDB.Checked)
            {
                this.Dap = "HIR";
                this.txtConnString.Text = GetConfigParameter.GetConfigValue("ConnectionString_HIR");
            }

            // コンボを初期化する。
            this.Init_cmb();
        }

        /// <summary>MySQLの場合</summary>
        private void rdbMySQL_CheckedChanged(object sender, EventArgs e)
        {
            // 接続文字列を初期化 
            if (this.rbnMySQL.Checked)
            {
                this.Dap = "MCN";
                this.txtConnString.Text = GetConfigParameter.GetConfigValue("ConnectionString_MCN");
            }

            // コンボを初期化する。
            this.Init_cmb();
        }

        /// <summary>PostgreSQLの場合</summary>
        private void rbnPstgrs_CheckedChanged(object sender, EventArgs e)
        {
            // 接続文字列を初期化 
            if (this.rbnPstgrs.Checked)
            {
                this.Dap = "NPS";
                this.txtConnString.Text = GetConfigParameter.GetConfigValue("ConnectionString_NPS");
            }

            // コンボを初期化する。
            this.Init_cmb();
        }

        #endregion

        #region コンボ ボックス 関連

        /// <summary>コンボ ボックスを初期化する</summary>
        private void Init_cmb()
        {
            // アイテムをクリアする。
            this.cmbSchemaInfo.Items.Clear();

            // アイテムを追加する。
            // this.cmbSchemaInfo.Items.Add("概要情報");
            this.cmbSchemaInfo.Items.Add(this.RM_GetString("SummaryInfo"));
            //this.cmbSchemaInfo.Items.Add("型情報");
            this.cmbSchemaInfo.Items.Add(this.RM_GetString("TypeInfo"));
            // this.cmbSchemaInfo.Items.Add("予約語情報");
            this.cmbSchemaInfo.Items.Add(this.RM_GetString("ReservedWordInfo"));
            //this.cmbSchemaInfo.Items.Add("制限情報");
            this.cmbSchemaInfo.Items.Add(this.RM_GetString("RestrictionInfo"));
            //this.cmbSchemaInfo.Items.Add("メタデータ情報");
            this.cmbSchemaInfo.Items.Add(this.RM_GetString("MetadataInfo"));

            this.cmbSchemaInfo.SelectedIndex = 0;
        }

        /// <summary>サポートされるメタデータの追加</summary>
        private void CmbAddItems()
        {
            foreach (System.Data.DataRow row in this.DtSchma.Rows)
            {
                this.cmbSchemaInfo.Items.Add("・ " + row["CollectionName"].ToString());
            }
        }

        #endregion

        #region リスト ボックス 関連

        /// <summary>リスト ボックスを更新する</summary>
        private void LbxUpdateItems()
        {
            // ソートしてから、
            SortedList sortedList = new SortedList();
            foreach (CTable table in this.HtSchemaCustom.Values)
            {
                sortedList.Add(table.Name, table);
            }

            // アイテムをクリアする。
            this.lbxTables.Items.Clear();

            // アイテムを更新する。
            foreach (string key in sortedList.Keys)
            {
                CTable table = ((CTable)sortedList[key]);

                if (table.Effective)
                {
                    this.lbxTables.Items.Add(table.Name);
                }
            }

            //// アイテムを更新する。
            //foreach (CTable table in this.HtSchemaCustom.Values)
            //{
            //    if (table.Effective)
            //    {
            //        this.lbxTables.Items.Add(table.Name);
            //    }
            //}
        }

        /// <summary>リスト ボックスからテーブルを削除する</summary>
        private void LbxDeleteItems()
        {
            // 無効にする。
            foreach (string tableName in this.lbxTables.SelectedItems)
            {
                // 選択されたテーブルを無効にする。
                ((CTable)this.HtSchemaCustom[tableName]).Effective = false;
            }

            // リスト ボックスを更新する。
            this.LbxUpdateItems();
        }

        #endregion

        #region デバッグ 関連

        /// <summary>
        /// DataGridViewでDataErrorとなったときの対処
        /// </summary>
        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // なにもしない。
            e.Cancel = true;
        }

        /// <summary>
        /// DataGridViewをクリアする。
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            //this.tabPage1.Text = "タブ１";
            this.tabPage1.Text = this.RM_GetString("TabPage1");
            //this.tabPage2.Text = "タブ２";
            this.tabPage2.Text = this.RM_GetString("TabPage2");
            //this.tabPage3.Text = "タブ３";
            this.tabPage3.Text = this.RM_GetString("TabPage3");

            this.dataGridView1.DataSource = null;
            this.dataGridView2.DataSource = null;
            this.dataGridView3.DataSource = null;
        }

        #endregion

        #endregion

        #region DBMSのスキーマ情報の表示

        /// <summary>DBMSのスキーマ情報の表示</summary>
        /// <remarks>
        /// 下記が共通のスキーマ コレクション
        /// ・ DataSourceInformation
        /// ・ DataTypes
        /// ・ ReservedWords
        /// ・ Restrictions
        /// ・ MetaDataCollections
        /// また、インターフェイスの共通化ができないので、下記のようにコピペで対応。
        /// （IConnectionにGetSchemaメソッドが存在しないため。）
        /// </remarks>
        private void btnGetSchemaInfo_Click(object sender, EventArgs e)
        {
            try
            {
                // カーソルを待機状態にする
                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

                // UIコントロールから情報を取得する。
                this.ConnString = this.txtConnString.Text;

                // コネクション オープン
                this.ConnectionOpen();

                // ラインを書くか、書かないか。
                bool writeLineFlag = false;

                #region DBMSのスキーマ情報の表示
                if (this.rbnSQL.Checked)
                {
                    #region SQL Server

                    if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("SummaryInfo"))
                    {
                        // DataSourceInformation
                        this.DtSchma = this.SqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataSourceInformation);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("TypeInfo"))
                    {
                        // DataTypes
                        this.DtSchma = this.SqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("ReservedWordInfo"))
                    {
                        // ReservedWords
                        this.DtSchma = this.SqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.ReservedWords);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("RestrictionInfo"))
                    {
                        // Restrictions
                        this.DtSchma = this.SqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.Restrictions);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("MetadataInfo"))
                    {
                        // MetaDataCollections
                        this.DtSchma = this.SqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections);

                        // アイテムの追加
                        if (this.cmbSchemaInfo.Items.Count <= 5)
                        {
                            this.CmbAddItems();
                        }

                        writeLineFlag = true;
                    }
                    else
                    {
                        // その他
                        this.DtSchma = this.SqlCn.GetSchema(this.cmbSchemaInfo.SelectedItem.ToString().Substring(2));
                        writeLineFlag = true;
                    }

                    #endregion
                }
                else if (this.rbnODB.Checked)
                {
                    #region ODBC.NET

                    if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("SummaryInfo"))
                    {
                        // DataSourceInformation
                        this.DtSchma = this.OdbCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataSourceInformation);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("TypeInfo"))
                    {
                        // DataTypes
                        this.DtSchma = this.OdbCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("ReservedWordInfo"))
                    {
                        // ReservedWords
                        this.DtSchma = this.OdbCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.ReservedWords);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("RestrictionInfo"))
                    {
                        // Restrictions
                        this.DtSchma = this.OdbCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.Restrictions);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("MetadataInfo"))
                    {
                        // MetaDataCollections
                        this.DtSchma = this.OdbCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections);

                        // アイテムの追加
                        if (this.cmbSchemaInfo.Items.Count <= 5)
                        {
                            this.CmbAddItems();
                        }

                        writeLineFlag = true;
                    }
                    else
                    {
                        // その他
                        this.DtSchma = this.OdbCn.GetSchema(this.cmbSchemaInfo.SelectedItem.ToString().Substring(2));
                        writeLineFlag = true;
                    }

                    #endregion
                }
                else if (this.rbnODP.Checked)
                {
                    #region Oracle

                    if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("SummaryInfo"))
                    {
                        // DataSourceInformation
                        this.DtSchma = this.OdpCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataSourceInformation);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("TypeInfo"))
                    {
                        // DataTypes
                        this.DtSchma = this.OdpCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("ReservedWordInfo"))
                    {
                        // ReservedWords
                        this.DtSchma = this.OdpCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.ReservedWords);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("RestrictionInfo"))
                    {
                        // Restrictions
                        this.DtSchma = this.OdpCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.Restrictions);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("MetadataInfo"))
                    {
                        // MetaDataCollections
                        this.DtSchma = this.OdpCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections);

                        // アイテムの追加
                        if (this.cmbSchemaInfo.Items.Count <= 5)
                        {
                            this.CmbAddItems();
                        }

                        writeLineFlag = true;
                    }
                    else
                    {
                        // その他
                        this.DtSchma = this.OdpCn.GetSchema(this.cmbSchemaInfo.SelectedItem.ToString().Substring(2));
                        writeLineFlag = true;
                    }

                    #endregion
                }
                else if (this.rbnMySQL.Checked)
                {
                    #region MySQL

                    if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("SummaryInfo"))
                    {
                        // DataSourceInformation
                        this.DtSchma = this.MySqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataSourceInformation);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("TypeInfo"))
                    {
                        // DataTypes
                        this.DtSchma = this.MySqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("ReservedWordInfo"))
                    {
                        // ReservedWords
                        this.DtSchma = this.MySqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.ReservedWords);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("RestrictionInfo"))
                    {
                        // Restrictions
                        this.DtSchma = this.MySqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.Restrictions);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("MetadataInfo"))
                    {
                        // MetaDataCollections
                        this.DtSchma = this.MySqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections);

                        // アイテムの追加
                        if (this.cmbSchemaInfo.Items.Count <= 5)
                        {
                            this.CmbAddItems();
                        }

                        writeLineFlag = true;
                    }
                    else
                    {
                        // その他
                        this.DtSchma = this.MySqlCn.GetSchema(this.cmbSchemaInfo.SelectedItem.ToString().Substring(2));
                        writeLineFlag = true;
                    }

                    #endregion
                }
#if NETCOREAPP
                else if (this.rbnPstgrs.Checked)
                {
                    #region PostgreSQL

                    if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("SummaryInfo"))
                    {
                        // DataSourceInformation
                        this.DtSchma = this.NpgsqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataSourceInformation);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("TypeInfo"))
                    {
                        // DataTypes
                        this.DtSchma = this.NpgsqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("ReservedWordInfo"))
                    {
                        // ReservedWords
                        this.DtSchma = this.NpgsqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.ReservedWords);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("RestrictionInfo"))
                    {
                        // Restrictions
                        this.DtSchma = this.NpgsqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.Restrictions);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("MetadataInfo"))
                    {
                        // MetaDataCollections
                        this.DtSchma = this.NpgsqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections);

                        // アイテムの追加
                        if (this.cmbSchemaInfo.Items.Count <= 5)
                        {
                            this.CmbAddItems();
                        }

                        writeLineFlag = true;
                    }
                    else
                    {
                        // その他
                        this.DtSchma = this.NpgsqlCn.GetSchema(this.cmbSchemaInfo.SelectedItem.ToString().Substring(2));
                        writeLineFlag = true;
                    }

                    #endregion
                }
#else
                else if (this.rbnOLE.Checked)
                {
                    #region OLEDB.NET

                    if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("SummaryInfo"))
                    {
                        // DataSourceInformation
                        this.DtSchma = this.OleCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataSourceInformation);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("TypeInfo"))
                    {
                        // DataTypes
                        this.DtSchma = this.OleCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("ReservedWordInfo"))
                    {
                        // ReservedWords
                        this.DtSchma = this.OleCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.ReservedWords);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("RestrictionInfo"))
                    {
                        // Restrictions
                        this.DtSchma = this.OleCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.Restrictions);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("MetadataInfo"))
                    {
                        // MetaDataCollections
                        this.DtSchma = this.OleCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections);

                        // アイテムの追加
                        if (this.cmbSchemaInfo.Items.Count <= 5)
                        {
                            this.CmbAddItems();
                        }

                        writeLineFlag = true;
                    }
                    else
                    {
                        // その他
                        this.DtSchma = this.OleCn.GetSchema(this.cmbSchemaInfo.SelectedItem.ToString().Substring(2));
                        writeLineFlag = true;
                    }

                    #endregion
                }
                else if (this.rbnDB2.Checked)
                {
                    #region DB2

                    /*if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("SummaryInfo"))
                    {
                        // DataSourceInformation
                        this.DtSchma = this.DB2Cn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataSourceInformation);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("TypeInfo"))
                    {
                        // DataTypes
                        this.DtSchma = this.DB2Cn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("ReservedWordInfo"))
                    {
                        // ReservedWords
                        this.DtSchma = this.DB2Cn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.ReservedWords);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("RestrictionInfo"))
                    {
                        // Restrictions
                        this.DtSchma = this.DB2Cn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.Restrictions);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("MetadataInfo"))
                    {
                        // MetaDataCollections
                        this.DtSchma = this.DB2Cn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections);

                        // アイテムの追加
                        if (this.cmbSchemaInfo.Items.Count <= 5)
                        {
                            this.CmbAddItems();
                        }

                        writeLineFlag = true;
                    }
                    else
                    {
                        // その他
                        this.DtSchma = this.DB2Cn.GetSchema(this.cmbSchemaInfo.SelectedItem.ToString().Substring(2));
                        writeLineFlag = true;
                    }*/

                    #endregion
                }
                else if (this.rbnHiRDB.Checked)
                {
                    #region HiRDB

                    /*if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("SummaryInfo"))
                    {
                        // DataSourceInformation
                        this.DtSchma = this.HiRDBCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataSourceInformation);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("TypeInfo"))
                    {
                        // DataTypes
                        this.DtSchma = this.HiRDBCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("ReservedWordInfo"))
                    {
                        // ReservedWords
                        this.DtSchma = this.HiRDBCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.ReservedWords);
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("RestrictionInfo"))
                    {
                        // Restrictions
                        this.DtSchma = this.HiRDBCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.Restrictions);
                        writeLineFlag = true;
                    }
                    else if (this.cmbSchemaInfo.SelectedItem.ToString() == this.RM_GetString("MetadataInfo"))
                    {
                        // MetaDataCollections
                        this.DtSchma = this.HiRDBCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.MetaDataCollections);

                        // アイテムの追加
                        if (this.cmbSchemaInfo.Items.Count <= 5)
                        {
                            this.CmbAddItems();
                        }

                        writeLineFlag = true;
                    }
                    else
                    {
                        // その他
                        this.DtSchma = this.HiRDBCn.GetSchema(this.cmbSchemaInfo.SelectedItem.ToString().Substring(2));
                        writeLineFlag = true;
                    }*/

                    #endregion
                }
#endif
                else
                {
                    // データプロバイダ指定無し（ありえない）
                }
                #endregion

                #region デバッグ
                if (this.cbxDebug.Checked)
                {
                    // 各種スキーマ情報
                    //this.tabPage1.Text = "スキーマ情報";
                    this.tabPage1.Text = RM_GetString("TabPage1SchemaInfo");
                    this.dataGridView1.DataSource = DtSchma;
                }
                #endregion

                // 子画面タイトル編集
                string temp = "";

                if (this.cmbSchemaInfo.SelectedItem.ToString().Substring(0, 2) == "・ ")
                {
                    temp = this.cmbSchemaInfo.SelectedItem.ToString().Substring(2);
                }
                else
                {
                    temp = this.cmbSchemaInfo.SelectedItem.ToString();
                }

                // スキーマ情報を子画面に表示
                SimpleTextBoxWindow win = new SimpleTextBoxWindow();
                //win._title = "DBMSのスキーマ情報の表示（" + temp + "）ダイアログ";
                win._title = string.Format(this.RM_GetString("DisplaySchemaInfoDialogBox"), temp);
                win._param = CmnMethods.DisplayDataString(this.DtSchma, writeLineFlag);
                win.ShowDialog(this);

            }
            catch (Exception ex)
            {
                //MessageBox.Show("ランタイムエラーです：" + ex.Message);
                MessageBox.Show(this.RM_GetString("RuntimeError") + ex.Message);
            }
            finally
            {
                // コネクション クローズ
                this.ConnectionClose();

                // カーソルを元に戻す
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
        }

        #endregion

        #region テーブルを一覧

        /// <summary>スキーマ情報を読み込み、テーブル・ビューをリストする。</summary>
        private void btnListTable_Click(object sender, EventArgs e)
        {
            // カーソルを待機状態にする
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            try
            {
                // UIコントロールから情報を取得し、リスト処理を呼び出す。
                this.ConnString = this.txtConnString.Text;

                if (!this.ListTable())
                {
                    // 失敗した場合、UIコントロールへの反映は行わない。
                    return;
                }

                #region デバッグ
                if (this.cbxDebug.Checked)
                {
                    // テーブル情報
                    if (this.DtSchmaTables != null)
                    {
                        //this.tabPage1.Text = "テーブル情報";
                        this.tabPage1.Text = this.RM_GetString("TabPage1TableInfo");
                        this.dataGridView1.DataSource = this.DtSchmaTables;
                    }
                    // ヴュー情報
                    if (this.DtSchmaViews != null)
                    {
                        //this.tabPage2.Text = "ヴュー情報";
                        this.tabPage2.Text = this.RM_GetString("TabPage2VieuxInfo");
                        this.dataGridView2.DataSource = this.DtSchmaViews;
                    }
                }
                #endregion

                // リスト ボックスを更新する。
                this.LbxUpdateItems();

                // ボタン状態の変更
                this.gbxDataProviders.Enabled = false;
                this.rbnMySQL.Enabled = false;
                this.txtConnString.Enabled = false;

                this.btnDelTable.Enabled = true;
                this.btnLoadColumn.Enabled = true;

                this.btnSetPrimaryKey.Enabled = false;

                this.cmbEncoding.Enabled = false;
                this.btnDaoDefinitionGen.Enabled = false;
            }
            finally
            {
                // カーソルを元に戻す
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>スキーマ情報を読み込み、テーブル・ビューをリストする（GUI／CUI 共通）。</summary>
        /// <returns>成功した場合 true</returns>
        /// <remarks>UIコントロールを参照しないため、フォームを表示せずに呼び出せる。</remarks>
        private bool ListTable()
        {
            try
            {
                // コネクション オープン
                this.ConnectionOpen();

                // スキーマの情報（テーブル）
                DataTable dtSchmaTables = null;
                // スキーマの情報（ビュー）
                DataTable dtSchmaViews = null;

                #region スキーマ情報を読み込み
                // テーブル・ビューをリストする。
                if (this.Dap == "SQL")
                {
                    #region SQL Server

                    // 注釈
                    //this.ShowMessage("同一ＤＢ上で同一名の複数のテーブルを持たないこと（SQL Server用のＤ層自動生成ツールの仕様です）", "－注意（前提条件）－");
                    this.ShowMessage(string.Format(this.RM_GetString("CautionPrerequisite"), "SQL Server"), this.RM_GetString("CautionPrerequisiteCaption"));

                    #region テーブル・ビューの情報を取得

                    dtSchmaTables = this.SqlCn.GetSchema("Tables");

                    // スキーマの情報（カスタム）の作成
                    this.HtSchemaCustom = new Hashtable();

                    // テーブル・ビューの取り込み
                    foreach (System.Data.DataRow row in dtSchmaTables.Rows)
                    {
                        if ((string)row["TABLE_TYPE"] == "BASE TABLE")
                        {
                            // テーブルの取り込み
                            HtSchemaCustom.Add((string)row["TABLE_NAME"],
                                new CTable((string)row["TABLE_NAME"], false));
                        }
                        else if ((string)row["TABLE_TYPE"] == "VIEW")
                        {
                            // ビューの取り込み。更新可能かどうかは判断しない。
                            HtSchemaCustom.Add((string)row["TABLE_NAME"],
                                new CTable((string)row["TABLE_NAME"], true));
                        }
                        else
                        {
                            // BASE TABLE・VIEW以外
                        }
                    }

                    #endregion

                    #endregion
                }
                else if (this.Dap == "ODB")
                {
                    #region ODBC

                    // 注釈
                    //this.ShowMessage("同一ＤＢ上で同一名の複数のテーブルを持たないこと（ODBC用のＤ層自動生成ツールの仕様です）", "－注意（前提条件）－");
                    this.ShowMessage(string.Format(this.RM_GetString("CautionPrerequisite"), "ODBC"), this.RM_GetString("CautionPrerequisiteCaption"));

                    #region テーブル・ビューの情報を取得

                    dtSchmaTables = this.OdbCn.GetSchema("Tables");

                    // スキーマの情報（カスタム）の作成
                    this.HtSchemaCustom = new Hashtable();

                    // テーブル・ビューの取り込み
                    foreach (System.Data.DataRow row in dtSchmaTables.Rows)
                    {
                        if ((string)row["TABLE_TYPE"] == "TABLE")
                        {
                            // テーブルの取り込み
                            HtSchemaCustom.Add((string)row["TABLE_NAME"],
                                new CTable((string)row["TABLE_NAME"], false));
                        }
                        else if ((string)row["TABLE_TYPE"] == "VIEW")
                        {
                            // ビューの取り込み。更新可能かどうかは判断しない。
                            HtSchemaCustom.Add((string)row["TABLE_NAME"],
                                new CTable((string)row["TABLE_NAME"], true));
                        }
                        else
                        {
                            // BASE TABLE・VIEW以外
                        }
                    }

                    #endregion

                    #endregion
                }
                else if (this.Dap == "ODP")
                {
                    #region Oracle

                    #region ユーザー名を取得する。

                    // Substringする。
                    string temp = this.ConnString;
                    int start = temp.IndexOf("User Id=") + "User Id=".Length;
                    int end = temp.IndexOf(";", start);
                    temp = temp.Substring(start, end - start);

                    // Trimする。
                    string userId = temp.Trim();

                    #endregion

                    // 注釈
                    //this.ShowMessage("[" + userId + "]スキーマ（ユーザ）の所有するテーブルのみを対象とします（Oracle用のＤ層自動生成ツールの仕様です）", "－注意（前提条件）－");
                    this.ShowMessage(string.Format(this.RM_GetString("CautionPrerequisiteOracle"), userId), this.RM_GetString("CautionPrerequisiteCaption"));

                    #region テーブル・ビューの情報を取得

                    dtSchmaTables = this.OdpCn.GetSchema("Tables");
                    dtSchmaViews = this.OdpCn.GetSchema("Views");

                    // スキーマの情報（カスタム）の作成
                    this.HtSchemaCustom = new Hashtable();

                    // テーブルの取り込み。
                    foreach (System.Data.DataRow row in dtSchmaTables.Rows)
                    {
                        // システムテーブルは避ける。
                        if ((string)row["TYPE"] == "System") { }
                        else
                        {
                            // 自分のもの以外は取らない。
                            if (row["OWNER"].ToString().ToUpper() == userId.ToUpper())
                            {
                                HtSchemaCustom.Add((string)row["TABLE_NAME"],
                                    new CTable((string)row["TABLE_NAME"], false));
                            }
                            else { }
                        }
                    }

                    // ビューの取り込み。更新可能かどうかは判断しない。
                    foreach (System.Data.DataRow row in dtSchmaViews.Rows)
                    {
                        // 自分のもの以外は取らない。
                        if (row["OWNER"].ToString().ToUpper() == userId.ToUpper())
                        {
                            HtSchemaCustom.Add((string)row["VIEW_NAME"],
                            new CTable((string)row["VIEW_NAME"], true));
                        }
                        else
                        { }
                    }

                    #endregion

                    #endregion
                }
                else if (this.Dap == "MCN")
                {
                    #region MySQL

                    // 注釈
                    // this.ShowMessage("同一ＤＢ上で同一名の複数のテーブルを持たないこと（MySQL用のＤ層自動生成ツールの仕様です）", "－注意（前提条件）－");
                    this.ShowMessage(string.Format(this.RM_GetString("CautionPrerequisite"), "MySQL"), this.RM_GetString("CautionPrerequisiteCaption"));

                    #region テーブル・ビューの情報を取得

                    dtSchmaTables = this.MySqlCn.GetSchema("Tables");
                    dtSchmaViews = this.MySqlCn.GetSchema("Views");

                    // スキーマの情報（カスタム）の作成
                    this.HtSchemaCustom = new Hashtable();

                    // テーブルの取り込み。
                    foreach (System.Data.DataRow row in dtSchmaTables.Rows)
                    {
                        HtSchemaCustom.Add((string)row["TABLE_NAME"],
                            new CTable((string)row["TABLE_NAME"], false));
                    }

                    // ビューの取り込み。更新可能かどうかは判断しない。
                    foreach (System.Data.DataRow row in dtSchmaViews.Rows)
                    {
                        HtSchemaCustom.Add((string)row["TABLE_NAME"],
                        new CTable((string)row["TABLE_NAME"], true));
                    }

                    #endregion

                    #endregion
                }
#if NETCOREAPP
                else if (this.Dap == "NPS")
                {
                    #region PostgreSQL

                    // 注釈
                    //this.ShowMessage("同一ＤＢ上で同一名の複数のテーブルを持たないこと（PostgreSQL用のＤ層自動生成ツールの仕様です）", "－注意（前提条件）－");
                    this.ShowMessage(string.Format(this.RM_GetString("CautionPrerequisite"), "PostgreSQL"), this.RM_GetString("CautionPrerequisiteCaption"));

                    #region テーブル・ビューの情報を取得

                    dtSchmaTables = this.NpgsqlCn.GetSchema("Tables");

                    // スキーマの情報（カスタム）の作成
                    this.HtSchemaCustom = new Hashtable();

                    // テーブル・ビューの取り込み。
                    foreach (System.Data.DataRow row in dtSchmaTables.Rows)
                    {
                        string tableSchema = ((string)row["TABLE_SCHEMA"]).ToUpper();

                        if (tableSchema == "INFORMATION_SCHEMA" || tableSchema == "PG_CATALOG")
                        {
                            // システム スキーマは無視する。
                        }
                        else
                        {
                            // ユーザ スキーマのみ対象にする。
                            HtSchemaCustom.Add((string)row["TABLE_NAME"],
                            new CTable((string)row["TABLE_NAME"], false));
                        }
                    }

                    #endregion

                    #endregion
                }
#else
                else if (this.Dap == "OLE")
                {
                    #region OLEDB

                    // 注釈
                    //this.ShowMessage("同一ＤＢ上で同一名の複数のテーブルを持たないこと（OLEDB用のＤ層自動生成ツールの仕様です）", "－注意（前提条件）－");
                    this.ShowMessage(string.Format(this.RM_GetString("CautionPrerequisite"), "OLEDB"), this.RM_GetString("CautionPrerequisiteCaption"));

                    #region テーブル・ビューの情報を取得

                    dtSchmaTables = this.OleCn.GetSchema("Tables");

                    // スキーマの情報（カスタム）の作成
                    this.HtSchemaCustom = new Hashtable();

                    // テーブル・ビューの取り込み
                    foreach (System.Data.DataRow row in dtSchmaTables.Rows)
                    {
                        if ((string)row["TABLE_TYPE"] == "TABLE")
                        {
                            // テーブルの取り込み
                            HtSchemaCustom.Add((string)row["TABLE_NAME"],
                                new CTable((string)row["TABLE_NAME"], false));
                        }
                        else if ((string)row["TABLE_TYPE"] == "VIEW")
                        {
                            // ビューの取り込み。更新可能かどうかは判断しない。
                            HtSchemaCustom.Add((string)row["TABLE_NAME"],
                                new CTable((string)row["TABLE_NAME"], true));
                        }
                        else
                        {
                            // TABLE・VIEW以外
                        }
                    }

                    #endregion

                    #endregion
                }
                else if (this.Dap == "DB2")
                {
                    #region DB2

                    // 注釈
                    //this.ShowMessage("同一ＤＢ上で同一名の複数のテーブルを持たないこと（DB2用のＤ層自動生成ツールの仕様です）", "－注意（前提条件）－");
                    this.ShowMessage(string.Format(this.RM_GetString("CautionPrerequisite"), "DB2"), this.RM_GetString("CautionPrerequisiteCaption"));

                    #region テーブル・ビューの情報を取得
                    /*
                    dtSchmaTables = this.DB2Cn.GetSchema("Tables");

                    // スキーマの情報（カスタム）の作成
                    this.HtSchemaCustom = new Hashtable();

                    // テーブル・ビューの取り込み
                    foreach (System.Data.DataRow row in dtSchmaTables.Rows)
                    {
                        // システム テーブルは避ける。
                        if ((string)row["TABLE_TYPE"] == "TABLE"
                            //|| (string)row["TABLE_TYPE"] == "ALIAS" →　別名はサポートしない
                            || (string)row["TABLE_TYPE"] == "MATERIALIZED QUERY TABLE")
                        {
                            // システム テーブルは避ける。
                            if ((string)row["TABLE_SCHEMA"].ToString().Substring(0, 3) == "SYS")
                            {
                            }
                            else
                            {
                                // テーブルの取り込み。
                                HtSchemaCustom.Add((string)row["TABLE_NAME"],
                                    new CTable((string)row["TABLE_NAME"], false));
                            }
                        }
                        else if ((string)row["TABLE_TYPE"] == "VIEW")
                        {
                            // システム ビューは避ける。
                            if ((string)row["TABLE_SCHEMA"].ToString().Substring(0, 3) == "SYS")
                            {
                            }
                            else
                            {
                                // ビューの取り込み。更新可能かどうかは判断しない。
                                HtSchemaCustom.Add((string)row["TABLE_NAME"],
                                    new CTable((string)row["TABLE_NAME"], true));
                            }
                        }
                        else
                        {
                            // 上記以外
                        }
                    }
                    */
                    #endregion

                    #endregion
                }
                else if (this.Dap == "HIR")
                {
                    #region HiRDB

                    // 注釈
                    //this.ShowMessage("同一ＤＢ上で同一名の複数のテーブルを持たないこと（HiRDB用のＤ層自動生成ツールの仕様です）", "－注意（前提条件）－");
                    this.ShowMessage(string.Format(this.RM_GetString("CautionPrerequisite"), "ODBC"), this.RM_GetString("CautionPrerequisiteCaption"));

                    #region テーブル・ビューの情報を取得
                    /*
                    dtSchmaTables = this.HiRDBCn.GetSchema("Tables");

                    // スキーマの情報（カスタム）の作成
                    this.HtSchemaCustom = new Hashtable();

                    // テーブル・ビューの取り込み
                    foreach (System.Data.DataRow row in dtSchmaTables.Rows)
                    {
                        // システム テーブルは避ける。
                        if ((string)row["TABLE_TYPE"] == "TABLE")
                        //|| (string)row["TABLE_TYPE"] == "ALIAS" →　別名はサポートしない
                        //|| (string)row["TABLE_TYPE"] == "MATERIALIZED QUERY TABLE")
                        {
                            // システム テーブルは避ける。
                            if ((string)row["TABLE_SCHEMA"].ToString().Substring(0, 3) == "SYS")
                            {
                            }
                            else
                            {
                                // テーブルの取り込み。
                                HtSchemaCustom.Add((string)row["TABLE_NAME"],
                                    new CTable((string)row["TABLE_NAME"], false));
                            }
                        }
                        else if ((string)row["TABLE_TYPE"] == "VIEW")
                        {
                            // システム ビューは避ける。
                            if ((string)row["TABLE_SCHEMA"].ToString().Substring(0, 3) == "SYS")
                            {
                            }
                            else
                            {
                                // ビューの取り込み。更新可能かどうかは判断しない。
                                HtSchemaCustom.Add((string)row["TABLE_NAME"],
                                    new CTable((string)row["TABLE_NAME"], true));
                            }
                        }
                        else
                        {
                            // 上記以外
                        }
                    }
                    */
                    #endregion

                    #endregion
                }
#endif
                else
                {
                    // データプロバイダ指定無し（ありえない）
                }
                #endregion

                // 取得したスキーマ情報を保持する（デバッグ表示用）。
                this.DtSchmaTables = dtSchmaTables;
                this.DtSchmaViews = dtSchmaViews;

                return true;
            }
            catch (Exception ex)
            {
                this.ShowMessage(this.RM_GetString("RuntimeError") + ex.Message);
                return false;
            }
            finally
            {
                // コネクション クローズ
                this.ConnectionClose();
            }
        }

        /// <summary>テーブルの削除処理</summary>
        private void btnDelTable_Click(object sender, EventArgs e)
        {
            this.LbxDeleteItems();
        }

        #endregion

        # region 列情報のロード

        /// <summary>スキーマ情報を読み込み、列情報をロードする。</summary>
        private void btnLoadColumn_Click(object sender, EventArgs e)
        {
            // カーソルを待機状態にする
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            try
            {
                // UIコントロールから情報を取得し、ロード処理を呼び出す。
                this.ConnString = this.txtConnString.Text;

                if (!this.LoadColumn())
                {
                    // 失敗した場合、UIコントロールへの反映は行わない。
                    return;
                }

                #region デバッグ
                if (this.cbxDebug.Checked)
                {
                    // カラム情報
                    if (this.DtSchmaColumns != null)
                    {
                        // this.tabPage1.Text = "カラム情報";
                        this.tabPage1.Text = this.RM_GetString("TabPage1ColumnInfo");
                        this.dataGridView1.DataSource = this.DtSchmaColumns;
                    }
                    // 主キー情報
                    if (this.DtSchmaPrimaryKeys != null)
                    {
                        //this.tabPage2.Text = "主キー情報";
                        this.tabPage2.Text = this.RM_GetString("TabPage2PrimarykeyInfo");
                        this.dataGridView2.DataSource = this.DtSchmaPrimaryKeys;
                    }
                    // インデックス カラム情報
                    if (this.DtSchmaIndexColumns != null)
                    {
                        //this.tabPage3.Text = "インデックス カラム情報";
                        this.tabPage3.Text = this.RM_GetString("TabPage3IndexColInfo");
                        this.dataGridView3.DataSource = this.DtSchmaIndexColumns;
                    }
                }
                #endregion

                // ボタン状態の変更
                this.btnLoadColumn.Enabled = false;

                this.btnSetPrimaryKey.Enabled = true;

                this.cmbEncoding.Enabled = true;
                this.btnDaoDefinitionGen.Enabled = true;
            }
            finally
            {
                // カーソルを元に戻す
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>スキーマ情報を読み込み、列情報をロードする（GUI／CUI 共通）。</summary>
        /// <returns>成功した場合 true</returns>
        /// <remarks>UIコントロールを参照しないため、フォームを表示せずに呼び出せる。</remarks>
        private bool LoadColumn()
        {
            try
            {
                #region ユーザー名取得用変数

                int start = 0;
                int end = 0;
                string userId = "";

                #endregion

                // コネクション オープン
                this.ConnectionOpen();

                #region 型情報を取得

                if (this.Dap == "SQL")
                {
                    // DataTypes
                    CmnMethods.DataTypes = this.SqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                }
                else if (this.Dap == "ODB")
                {
                    // DataTypes
                    CmnMethods.DataTypes = this.OdbCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                }
                else if (this.Dap == "ODP")
                {
                    // DataTypes
                    CmnMethods.DataTypes = this.OdpCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                }
                else if (this.Dap == "MCN")
                {
                    // DataTypes
                    CmnMethods.DataTypes = this.MySqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                }
                else if (this.Dap == "NPS")
                {
                    //// DataTypes（NpgsqlではDataTypesがサポートされていない）
                    //CmnMethods.DataTypes = this.NpgsqlCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                }
#if NETCOREAPP
#else
                else if (this.Dap == "OLE")
                {
                    // DataTypes
                    CmnMethods.DataTypes = this.OleCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                }
                /*else if (this.Dap == "DB2")
                {
                    // DataTypes
                    CmnMethods.DataTypes = this.DB2Cn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                }
                else if (this.Dap == "HIR")
                {
                    // DataTypes
                    CmnMethods.DataTypes = this.HiRDBCn.GetSchema(System.Data.Common.DbMetaDataCollectionNames.DataTypes);
                }*/
#endif
                else
                {
                    // データプロバイダ指定無し（ありえない）
                }

                #endregion

                // スキーマの情報（カラム）
                DataTable dtSchmaColumns = null;
                // スキーマの情報（主キー）
                DataTable dtSchmaPrimaryKeys = null;
                // スキーマの情報（インデックス カラム）
                DataTable dtSchmaIndexColumns = null;

                #region 列情報をロード
                if (this.Dap == "SQL")
                {
                    #region SQL Server

                    #region カラムの情報を取得

                    dtSchmaColumns = this.SqlCn.GetSchema("Columns");

                    // カラムの取り込み
                    foreach (System.Data.DataRow row in dtSchmaColumns.Rows)
                    {
                        // テーブルを取得
                        CTable table = (CTable)this.HtSchemaCustom[(string)row["TABLE_NAME"]];

                        // 有効なテーブルにのみロードする。
                        if (table == null)
                        {
                            // 不明なテーブル
                        }
                        else
                        {
                            // 有効なテーブル
                            if (table.Effective)
                            {
                                CColumn column = new CColumn(
                                    (string)row["COLUMN_NAME"], (string)row["DATA_TYPE"],
                                    CmnMethods.ConvertToDotNetTypeInfo((string)row["DATA_TYPE"]));

                                // ポジションをキーにしてカラムを追加
                                table.HtColumns_Position[row["ORDINAL_POSITION"].ToString()] = column;
                                // カラム名をキーにしてカラムを追加
                                table.HtColumns_Name[(string)row["COLUMN_NAME"]] = column;
                            }
                        }
                    }

                    #endregion

                    #region 主キーの情報を取得

                    // ↓↓↓【変更】↓↓↓
                    // 以前は、sp_helpconstraint で主キー インデックス名を取得し、
                    // sp_MShelpindex で主キー列名（indCol1～indCol16）を取得していたが、下記の理由でカタログ ビューに変更した。
                    // ・sp_MShelpindex は非公開のストアド プロシージャであり、内部で一時テーブル（tempdbの照合順序）と
                    //   sysindexes.name（DBの照合順序）を文字列比較するため、両者が異なる環境では照合順序の競合で失敗する。
                    // ・主キーが16列までという制限があった。
                    // ・テーブルごとに2回、DBへ問い合わせていた。
                    // カタログ ビューの結合キーは全てID（整数）であるため、照合順序の影響を受けない。

                    // 主キー情報を取得
                    SqlCommand cmdPrimaryKeys = new SqlCommand(
                        "SELECT t.name AS TABLE_NAME, c.name AS COLUMN_NAME, ic.key_ordinal AS KEY_ORDINAL"
                        + " FROM sys.indexes AS i"
                        + " INNER JOIN sys.index_columns AS ic"
                        + " ON i.object_id = ic.object_id AND i.index_id = ic.index_id"
                        + " INNER JOIN sys.columns AS c"
                        + " ON ic.object_id = c.object_id AND ic.column_id = c.column_id"
                        + " INNER JOIN sys.tables AS t ON i.object_id = t.object_id"
                        + " WHERE i.is_primary_key = 1"
                        + " ORDER BY t.name, ic.key_ordinal", this.SqlCn);

                    dtSchmaPrimaryKeys = new DataTable();

                    SqlDataReader drd = cmdPrimaryKeys.ExecuteReader();
                    dtSchmaPrimaryKeys.Load(drd);
                    drd.Close();

                    // 主キーの設定
                    foreach (System.Data.DataRow row in dtSchmaPrimaryKeys.Rows)
                    {
                        // テーブルを取得
                        CTable table = (CTable)this.HtSchemaCustom[(string)row["TABLE_NAME"]];

                        // 有効なテーブルにのみ設定する。
                        if (table == null)
                        {
                            // 不明なテーブル
                        }
                        else
                        {
                            // 有効なテーブル
                            if (table.Effective)
                            {
                                // 主キー列
                                CColumn column = (CColumn)table.HtColumns_Name[(string)row["COLUMN_NAME"]];

                                // nullの時があるようなので、この場合は処理しない。
                                if (column == null) { }
                                else
                                {
                                    // 列のIsKeyフラグを立てる。
                                    column.IsKey = true;
                                }
                            }
                        }
                    }
                    // ↑↑↑【変更】↑↑↑

                    #endregion

                    #endregion
                }
                else if (this.Dap == "ODB")
                {
                    #region ODBC.NET

                    dtSchmaColumns = this.OdbCn.GetSchema("Columns");

                    // カラムの取り込み
                    foreach (System.Data.DataRow row in dtSchmaColumns.Rows)
                    {
                        // テーブルを取得
                        CTable table = (CTable)this.HtSchemaCustom[(string)row["TABLE_NAME"]];

                        // 有効なテーブルにのみロードする。
                        if (table == null)
                        {
                            // 不明なテーブル
                        }
                        else
                        {
                            // 有効なテーブル
                            if (table.Effective)
                            {
                                CColumn column = new CColumn(
                                    (string)row["COLUMN_NAME"], (string)row["TYPE_NAME"],
                                    CmnMethods.ConvertToDotNetTypeInfo((string)row["TYPE_NAME"]));

                                // ポジションをキーにしてカラムを追加
                                table.HtColumns_Position[row["ORDINAL_POSITION"].ToString()] = column;
                                // カラム名をキーにしてカラムを追加
                                table.HtColumns_Name[(string)row["COLUMN_NAME"]] = column;
                            }
                        }
                    }

                    // 主キーの情報をロード・・・しない。

                    #endregion
                }
                else if (this.Dap == "ODP")
                {
                    #region Oracle

                    // 接続文字列から"User Id"値を取得。
                    start = this.ConnString.IndexOf("User Id=") + "User Id=".Length;
                    end = this.ConnString.IndexOf(";", start);
                    userId = this.ConnString.Substring(start, end - start).Trim();

                    #region カラムの情報を取得

                    // カラムの情報を取得
                    dtSchmaColumns = this.OdpCn.GetSchema("Columns");

                    // カラムの取り込み
                    foreach (System.Data.DataRow row in dtSchmaColumns.Rows)
                    {
                        // テーブルを取得
                        CTable table = (CTable)this.HtSchemaCustom[(string)row["TABLE_NAME"]];

                        // 有効なテーブルにのみロードする。
                        if (table == null)
                        {
                            // 不明なテーブル
                        }
                        else
                        {
                            // 有効なテーブル
                            if (table.Effective)
                            {
                                // 自分のもの以外は取らない。
                                if (row["OWNER"].ToString().ToUpper() == userId.ToUpper())
                                {
                                    // 自分のもの
                                    CColumn column = new CColumn(
                                        (string)row["COLUMN_NAME"], (string)row["DATATYPE"],
                                        CmnMethods.ConvertToDotNetTypeInfo((string)row["DATATYPE"]));

                                    // ポジションをキーにしてカラムを追加
                                    table.HtColumns_Position[row["ID"].ToString()] = column;
                                    // カラム名をキーにしてカラムを追加
                                    table.HtColumns_Name[(string)row["COLUMN_NAME"]] = column;
                                }
                                else
                                {
                                    // 自分のもの以外
                                }
                            }
                        }
                    }

                    #endregion

                    #region 主キーの情報を取得

                    // 主キー情報を取得
                    dtSchmaPrimaryKeys = this.OdpCn.GetSchema("PrimaryKeys");
                    // インデックス カラム情報を取得
                    dtSchmaIndexColumns = this.OdpCn.GetSchema("IndexColumns");

                    // 主キーの設定

                    // テーブルを取得
                    foreach (CTable table in this.HtSchemaCustom.Values)
                    {
                        if (table.Effective)
                        {
                            // プライマリキー
                            foreach (System.Data.DataRow row1 in dtSchmaPrimaryKeys.Rows)
                            {
                                // 有効なテーブルのプライマリキー
                                if (table.Name == row1["TABLE_NAME"].ToString())
                                {
                                    //// 自分のもの以外は取らない。
                                    //if (row1["INDEX_OWNER"].ToString().ToUpper() == userId.ToUpper())
                                    //{

                                    // 自分のもの以外
                                    foreach (System.Data.DataRow row2 in dtSchmaIndexColumns.Rows)
                                    {
                                        // 有効なテーブルのプライマリキーのカラム
                                        if (row1["INDEX_NAME"].ToString() == row2["INDEX_NAME"].ToString())
                                        {
                                            // 自分のもの以外は取らない。
                                            if (row2["INDEX_OWNER"].ToString().ToUpper() == userId.ToUpper())
                                            {
                                                // 自分のもの
                                                // 列のIsKeyフラグを立てる。
                                                CColumn column = (CColumn)table.HtColumns_Name[row2["COLUMN_NAME"].ToString()];

                                                // nullの時があるようなので、この場合は処理しない。
                                                if (column == null) { }
                                                else
                                                {
                                                    column.IsKey = true;
                                                }
                                            }
                                            else
                                            {
                                                // 自分のもの以外
                                            }
                                        }
                                    }

                                    //}
                                    //else
                                    //{
                                    //    // 自分のもの以外
                                    //}
                                }
                            }
                        }
                    }

                    #endregion

                    #endregion
                }
                else if (this.Dap == "MCN")
                {
                    #region MySQL

                    // カラムの情報を取得
                    dtSchmaColumns = this.MySqlCn.GetSchema("Columns");

                    // カラムの取り込み
                    foreach (System.Data.DataRow row in dtSchmaColumns.Rows)
                    {
                        // テーブルを取得
                        CTable table = (CTable)this.HtSchemaCustom[(string)row["TABLE_NAME"]];

                        // 有効なテーブルにのみロードする。
                        if (table == null)
                        {
                            // 不明なテーブル
                        }
                        else
                        {
                            // 有効なテーブル
                            if (table.Effective)
                            {
                                CColumn column = new CColumn(
                                    (string)row["COLUMN_NAME"], (string)row["DATA_TYPE"],
                                    CmnMethods.ConvertToDotNetTypeInfo((string)row["DATA_TYPE"]));

                                // ポジションをキーにしてカラムを追加
                                table.HtColumns_Position[row["ORDINAL_POSITION"].ToString()] = column;
                                // カラム名をキーにしてカラムを追加
                                table.HtColumns_Name[(string)row["COLUMN_NAME"]] = column;
                            }
                        }
                    }

                    // 主キーの情報をロード・・・しない。

                    #endregion
                }
#if NETCOREAPP
                else if (this.Dap == "NPS")
                {
                    #region PostgreSQL

                    // カラムの情報を取得
                    dtSchmaColumns = this.NpgsqlCn.GetSchema("Columns");

                    // カラムの取り込み
                    foreach (System.Data.DataRow row in dtSchmaColumns.Rows)
                    {
                        // テーブルを取得
                        CTable table = (CTable)this.HtSchemaCustom[(string)row["TABLE_NAME"]];

                        // 有効なテーブルにのみロードする。
                        if (table == null)
                        {
                            // 不明なテーブル
                        }
                        else
                        {
                            // 有効なテーブル
                            if (table.Effective)
                            {
                                CColumn column = new CColumn(
                                    (string)row["COLUMN_NAME"], (string)row["DATA_TYPE"], "System.Object");
                                //CmnMethods.ConvertToDotNetTypeInfo((string)row["DATA_TYPE"]));
                                //（NpgsqlではDataTypesがサポートされていないため）

                                // ポジションをキーにしてカラムを追加
                                table.HtColumns_Position[row["ORDINAL_POSITION"].ToString()] = column;
                                // カラム名をキーにしてカラムを追加
                                table.HtColumns_Name[(string)row["COLUMN_NAME"]] = column;
                            }
                        }
                    }

                    // 主キーの情報をロード・・・しない。

                    #endregion
                }
#else
                else if (this.Dap == "OLE")
                {
                    #region OLEDB.NET

                    dtSchmaColumns = this.OleCn.GetSchema("Columns");

                    // カラムの取り込み
                    foreach (System.Data.DataRow row in dtSchmaColumns.Rows)
                    {
                        // テーブルを取得
                        CTable table = (CTable)this.HtSchemaCustom[(string)row["TABLE_NAME"]];

                        // 有効なテーブルにのみロードする。
                        if (table == null)
                        {
                            // 不明なテーブル
                        }
                        else
                        {
                            // 有効なテーブル
                            if (table.Effective)
                            {
                                CColumn column = new CColumn(
                                    (string)row["COLUMN_NAME"], CmnMethods.ConvertToDBTypeInfo_OLEDB(row["DATA_TYPE"].ToString()),
                                    CmnMethods.ConvertToDotNetTypeInfo(CmnMethods.ConvertToDBTypeInfo_OLEDB(row["DATA_TYPE"].ToString())));

                                // ポジションをキーにしてカラムを追加
                                table.HtColumns_Position[row["ORDINAL_POSITION"].ToString()] = column;
                                // カラム名をキーにしてカラムを追加
                                table.HtColumns_Name[(string)row["COLUMN_NAME"]] = column;
                            }
                        }
                    }

                    // 主キーの情報をロード・・・しない。

                    #endregion
                }

                /*else if (this.Dap == "DB2")
                {
                    #region DB2

                    // カラムの情報を取得
                    dtSchmaColumns = this.DB2Cn.GetSchema("Columns");

                    // カラムの取り込み
                    foreach (System.Data.DataRow row in dtSchmaColumns.Rows)
                    {
                        // テーブルを取得
                        CTable table = (CTable)this.HtSchemaCustom[(string)row["TABLE_NAME"]];

                        // 有効なテーブルにのみロードする。
                        if (table == null)
                        {
                            // 不明なテーブル
                        }
                        else
                        {
                            // 有効なテーブル
                            if (table.Effective)
                            {
                                CColumn column = new CColumn(
                                    (string)row["COLUMN_NAME"], (string)row["DATA_TYPE_NAME"],
                                    CmnMethods.ConvertToDotNetTypeInfo_DB2((string)row["DATA_TYPE_NAME"]));

                                // ポジションをキーにしてカラムを追加
                                table.HtColumns_Position[row["ORDINAL_POSITION"].ToString()] = column;
                                // カラム名をキーにしてカラムを追加
                                table.HtColumns_Name[(string)row["COLUMN_NAME"]] = column;
                            }
                        }
                    }

                    // 主キーの情報をロード・・・しない。

                    #endregion
                }
                else if (this.Dap == "HIR")
                {
                    #region HiRDB

                    // カラムの情報を取得
                    dtSchmaColumns = this.HiRDBCn.GetSchema("Columns");

                    // カラムの取り込み
                    foreach (System.Data.DataRow row in dtSchmaColumns.Rows)
                    {
                        // テーブルを取得
                        CTable table = (CTable)this.HtSchemaCustom[(string)row["TABLE_NAME"]];

                        // 有効なテーブルにのみロードする。
                        if (table == null)
                        {
                            // 不明なテーブル
                        }
                        else
                        {
                            // 有効なテーブル
                            if (table.Effective)
                            {
                                CColumn column = new CColumn(
                                    (string)row["COLUMN_NAME"], (string)row["DATA_TYPE_NAME"],
                                    CmnMethods.ConvertToDotNetTypeInfo_DB2((string)row["DATA_TYPE_NAME"]));

                                // ポジションをキーにしてカラムを追加
                                table.HtColumns_Position[row["ORDINAL_POSITION"].ToString()] = column;
                                // カラム名をキーにしてカラムを追加
                                table.HtColumns_Name[(string)row["COLUMN_NAME"]] = column;
                            }
                        }
                    }

                    // 主キーの情報をロード・・・しない。

                    #endregion
                }*/
#endif
                else
                {
                    // データプロバイダ指定無し（ありえない）
                }
                #endregion

                // 取得したスキーマ情報を保持する（デバッグ表示用）。
                this.DtSchmaColumns = dtSchmaColumns;
                this.DtSchmaPrimaryKeys = dtSchmaPrimaryKeys;
                this.DtSchmaIndexColumns = dtSchmaIndexColumns;

                return true;
            }
            catch (Exception ex)
            {
                this.ShowMessage(this.RM_GetString("RuntimeError") + ex.Message);
                return false;
            }
            finally
            {
                // コネクション クローズ
                this.ConnectionClose();
            }
        }

        #endregion

        #region 主キー情報の設定

        /// <summary>主キー情報の設定</summary>
        private void btnSetPrimaryKey_Click(object sender, EventArgs e)
        {
            bool isFirst = true;

            foreach (string tableName in this.lbxTables.SelectedItems)
            {
                // キャンセル処理
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    //DialogResult dRet = MessageBox.Show("次のテーブルに進みます。", "確認", MessageBoxButtons.OKCancel);
                    DialogResult dRet = MessageBox.Show(this.RM_GetString("GotoNextTable"), this.RM_GetString("Confirm"), MessageBoxButtons.OKCancel);

                    if (dRet == DialogResult.Cancel)
                    {
                        // キャンセル
                        break;
                    }
                    else
                    {
                        //処理を継続
                    }
                }

                // スキーマ情報を子画面に表示
                SetPrimaryKeyWindow win = new SetPrimaryKeyWindow();
                //win._title = "主キー情報の設定（" + tableName + "）ダイアログ";
                win._title = string.Format(this.RM_GetString("SetPrimaryKeyInfo"), tableName);
                win._htColumns_Position = ((CTable)this.HtSchemaCustom[tableName]).HtColumns_Position;
                win.ShowDialog(this);
            }
        }

        #endregion

        #region D層定義情報を生成する

        /// <summary>D層定義情報を生成する</summary>
        private void btnDaoDefinitionGen_Click(object sender, EventArgs e)
        {
            // セーブ先の設定
            SaveFileDialog sfd = new SaveFileDialog();
            //sfd.Filter = "CSVファイル(*.csv)|*.csv";
            sfd.Filter = this.RM_GetString("SaveFileDialogFilter");
            //sfd.Title = "D層定義情報ファイル";
            sfd.Title = this.RM_GetString("SaveFileDialogTitle");
            DialogResult dRet = sfd.ShowDialog();

            // カーソルを待機状態にする
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (dRet == DialogResult.OK)
                {
                    // OKで
                    // UIコントロールから情報を取得し、生成処理を呼び出す。
                    if (this.WriteDaoDefinition(sfd.FileName, (int)this.cmbEncoding.SelectedValue))
                    {
                        // メッセージ
                        //MessageBox.Show("Ｄ層定義情報の生成完了！");
                        MessageBox.Show(this.RM_GetString("DlayerGeneratedMessage"));
                    }
                }
                else
                {
                    // メッセージ
                    //MessageBox.Show("操作はキャンセルされました。");
                    MessageBox.Show(this.RM_GetString("DLayerCancelledmessage"));
                }
            }
            finally
            {
                // カーソルを元に戻す
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>D層定義情報ファイルを出力する（GUI／CUI 共通）。</summary>
        /// <param name="filePath">出力先ファイル パス（*.csv）</param>
        /// <param name="codePage">出力エンコーディングのコード ページ</param>
        /// <returns>成功した場合 true</returns>
        /// <remarks>UIコントロールを参照しないため、フォームを表示せずに呼び出せる。</remarks>
        private bool WriteDaoDefinition(string filePath, int codePage)
        {
            // ストリームライター
            StreamWriter sw = null;
            StreamWriter sw_DBTypeInfo = null;
            StreamWriter sw_DotNetTypeInfo = null;

            try
            {
                if (filePath == "")
                {
                    // ファイルが指定されていない。
                }
                else
                {
                    // ファイルが指定されている。

                    // ファイル出力

                    // エンコーディングの指定
                    Encoding enc = Encoding.GetEncoding(codePage);

                    // ファイル ストリームを生成する。
                    int temp = filePath.Length - 4;
                    sw = new StreamWriter(filePath, false, enc);
                    sw_DBTypeInfo = new StreamWriter(filePath.Substring(0, temp) + "_DBTypeInfo" + filePath.Substring(temp), false, enc);
                    sw_DotNetTypeInfo = new StreamWriter(filePath.Substring(0, temp) + "_DotNetTypeInfo" + filePath.Substring(temp), false, enc);

                    // ファイル ヘッダーを出力
                    //sw.WriteLine("テーブル名,カラム情報～");
                    sw.WriteLine(this.RM_GetString("TableNameColInfo"));
                    //sw_DBTypeInfo.WriteLine("テーブル名,カラムDB型情報～");
                    sw_DBTypeInfo.WriteLine(this.RM_GetString("TableNameColDbTypeInfo"));
                    //sw_DotNetTypeInfo.WriteLine("テーブル名,カラム.NET型情報～");
                    sw_DotNetTypeInfo.WriteLine(this.RM_GetString("TableNameColdotNetTypeInfo"));

                    // テーブルを処理
                    foreach (CTable table in this.HtSchemaCustom.Values)
                    {
                        // 有効なテーブルだけ出力
                        if (table.Effective)
                        {
                            // カラム ポジションを昇順にソートする
                            ArrayList sort = CmnMethods.sortColumn(table.HtColumns_Position);

                            // １行目
                            string pk = "";
                            string pk_DBTypeInfo = "";
                            string pk_DotNetTypeInfo = "";
                            // ２行目
                            string _else = "";
                            string _else_DBTypeInfo = "";
                            string _else_DotNetTypeInfo = "";

                            // ソート後のカラム ポジション配列を廻す
                            foreach (Int32 position in sort)
                            {
                                // カラムを取得
                                CColumn column = (CColumn)table.HtColumns_Position[position.ToString()];

                                if (column.IsKey)
                                {
                                    // 主キー列 // １行目
                                    pk += column.Name + ",";
                                    pk_DBTypeInfo += column.DBTypeInfo + ",";
                                    pk_DotNetTypeInfo += column.DotNetTypeInfo + ",";
                                }
                                else
                                {
                                    // その他の列 // ２行目
                                    _else += column.Name + ",";
                                    _else_DBTypeInfo += column.DBTypeInfo + ",";
                                    _else_DotNetTypeInfo += column.DotNetTypeInfo + ",";
                                }
                            }

                            // １行目
                            sw.WriteLine(table.Name + "," + pk);
                            sw_DBTypeInfo.WriteLine(table.Name + "," + pk_DBTypeInfo);
                            sw_DotNetTypeInfo.WriteLine(table.Name + "," + pk_DotNetTypeInfo);
                            // ２行目
                            sw.WriteLine("," + _else);
                            sw_DBTypeInfo.WriteLine("," + _else_DBTypeInfo);
                            sw_DotNetTypeInfo.WriteLine("," + _else_DotNetTypeInfo);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                this.ShowMessage(this.RM_GetString("RuntimeError") + ex.Message);
                return false;
            }
            finally
            {
                // ストリームライターを閉じる
                if (sw == null)
                {
                    // null
                }
                else
                {
                    // nullでない場合、閉じる。
                    sw.Close();
                }

                if (sw_DBTypeInfo == null)
                {
                    // null
                }
                else
                {
                    // nullでない場合、閉じる。
                    sw_DBTypeInfo.Close();
                }

                if (sw_DotNetTypeInfo == null)
                {
                    // null
                }
                else
                {
                    // nullでない場合、閉じる。
                    sw_DotNetTypeInfo.Close();
                }
            }
        }

        #endregion

        #region CUI（ヘッドレス実行）

        // ↓↓↓【追加】↓↓↓

        /// <summary>Ｄ層定義情報ファイルを生成する（CUI）。</summary>
        /// <param name="opt">生成オプション（UIコントロールに依存しない）</param>
        /// <returns>成功した場合 true</returns>
        /// <remarks>
        /// GUI で「テーブルを一覧」→「列情報のロード」→「主キー情報の設定」
        /// →「Ｄ層定義情報の生成」と操作するのと同じ処理を、非対話で順に実行する。
        /// </remarks>
        public bool DaoDefinitionGen(DaoDefinitionOptions opt)
        {
            // 接続情報の設定（GUIではラジオ ボタンとテキスト ボックスから設定される）
            this.Dap = opt.Dap;
            this.ConnString = opt.ConnectionString;

            // テーブル・ビューをリストする。
            if (!this.ListTable())
            {
                return false;
            }

            // 生成対象のテーブル・ビューを絞り込む。
            if (!this.SelectTables(opt.Tables, opt.ExcludeTables))
            {
                return false;
            }

            // 列情報をロードする。
            if (!this.LoadColumn())
            {
                return false;
            }

            // 主キー情報を設定する。
            if (!this.SetPrimaryKeys(opt.PrimaryKeys))
            {
                return false;
            }

            // 生成対象を表示する。
            foreach (CTable table in this.HtSchemaCustom.Values)
            {
                if (table.Effective)
                {
                    this.ShowMessage("対象：" + table.Name);
                }
            }

            // Ｄ層定義情報ファイルを出力する。
            return this.WriteDaoDefinition(opt.OutputFilePath, opt.CodePage);
        }

        /// <summary>生成対象のテーブル・ビューを絞り込む。</summary>
        /// <param name="tables">生成対象とするテーブル・ビュー名の配列（空の場合は全て）</param>
        /// <param name="excludeTables">生成対象から除外するテーブル・ビュー名の配列</param>
        /// <returns>成功した場合 true</returns>
        /// <remarks>GUI の「テーブルを削除（LbxDeleteItems）」に相当する。</remarks>
        private bool SelectTables(string[] tables, string[] excludeTables)
        {
            // 生成対象の指定がある場合、指定されたテーブル・ビューのみを有効にする。
            if (tables != null && tables.Length != 0)
            {
                // 一旦、全て無効にする。
                foreach (CTable table in this.HtSchemaCustom.Values)
                {
                    table.Effective = false;
                }

                foreach (string tableName in tables)
                {
                    CTable table = this.FindTable(tableName);

                    if (table == null)
                    {
                        this.ShowMessage(string.Format(
                            "エラー：テーブル・ビュー[{0}]が見つかりません。", tableName));
                        return false;
                    }
                    else
                    {
                        table.Effective = true;
                    }
                }
            }

            // 除外の指定がある場合、指定されたテーブル・ビューを無効にする。
            if (excludeTables != null)
            {
                foreach (string tableName in excludeTables)
                {
                    CTable table = this.FindTable(tableName);

                    if (table == null)
                    {
                        this.ShowMessage(string.Format(
                            "エラー：テーブル・ビュー[{0}]が見つかりません。", tableName));
                        return false;
                    }
                    else
                    {
                        table.Effective = false;
                    }
                }
            }

            return true;
        }

        /// <summary>主キー情報を設定する。</summary>
        /// <param name="primaryKeys">主キー情報（キー：テーブル名、値：主キー列名の配列）</param>
        /// <returns>成功した場合 true</returns>
        /// <remarks>
        /// GUI の「主キー情報の設定（SetPrimaryKeyWindow）」に相当する。
        /// 指定されたテーブルの主キーは、DBMS から取得した情報ではなく、この指定で置き換える。
        /// </remarks>
        private bool SetPrimaryKeys(Dictionary<string, string[]> primaryKeys)
        {
            if (primaryKeys == null)
            {
                return true;
            }

            foreach (KeyValuePair<string, string[]> kvp in primaryKeys)
            {
                CTable table = this.FindTable(kvp.Key);

                if (table == null)
                {
                    this.ShowMessage(string.Format(
                        "エラー：テーブル・ビュー[{0}]が見つかりません。", kvp.Key));
                    return false;
                }

                // 指定されたテーブルの主キーを一旦クリアする。
                foreach (CColumn column in table.HtColumns_Name.Values)
                {
                    column.IsKey = false;
                }

                // 主キー列のIsKeyフラグを立てる。
                foreach (string columnName in kvp.Value)
                {
                    CColumn column = this.FindColumn(table, columnName);

                    if (column == null)
                    {
                        this.ShowMessage(string.Format(
                            "エラー：テーブル・ビュー[{0}]にカラム[{1}]が見つかりません。",
                            table.Name, columnName));
                        return false;
                    }
                    else
                    {
                        column.IsKey = true;
                    }
                }
            }

            return true;
        }

        /// <summary>テーブル・ビューを検索する（大文字・小文字を区別しない）。</summary>
        /// <param name="tableName">テーブル・ビュー名</param>
        /// <returns>テーブル・ビュー（見つからない場合はnull）</returns>
        private CTable FindTable(string tableName)
        {
            foreach (CTable table in this.HtSchemaCustom.Values)
            {
                if (string.Compare(table.Name, tableName, true) == 0)
                {
                    return table;
                }
            }

            return null;
        }

        /// <summary>カラムを検索する（大文字・小文字を区別しない）。</summary>
        /// <param name="table">テーブル・ビュー</param>
        /// <param name="columnName">カラム名</param>
        /// <returns>カラム（見つからない場合はnull）</returns>
        private CColumn FindColumn(CTable table, string columnName)
        {
            foreach (CColumn column in table.HtColumns_Name.Values)
            {
                if (string.Compare(column.Name, columnName, true) == 0)
                {
                    return column;
                }
            }

            return null;
        }

        // ↑↑↑【追加】↑↑↑

        #endregion

        /// <summary>D層、Daoクラスファイル、SQLファイルを生成する。</summary>
        private void btnDaoAndSqlGen_Click(object sender, EventArgs e)
        {
            // D層、Daoクラスファイル、SQLファイル生成ダイアログ
            Form2 win = new Form2();
            // データプロバイダ設定の引き継ぎ
            win.Init(this.Dap);
            // ダイアログとして表示
            win.ShowDialog(this);
        }

        /// <summary>This Method gets the string values from resource file based on the key passed</summary>      
        private string RM_GetString(string key)
        {
            ResourceManager rm = Resources.Resource.ResourceManager;
            return rm.GetString(key);
        }
        /// <summary> To handle UI of button controls</summary> 
        private void btnLoadColumn_EnabledChanged(object sender, EventArgs e)
        {
            Button btnLoadColor = (Button)sender;
            if (btnLoadColor.Enabled)
            {
                btnLoadColor.BackColor = Color.FromArgb(91, 155, 213);
                btnLoadColor.ForeColor = Color.White;
            }
            else
            {
                btnLoadColor.BackColor = Color.LightGray;
                btnLoadColor.ForeColor = Color.DarkGray;
            }
        }

        /// <summary> To handle UI of button controls</summary> 
        private void btnDaoDefinitionGen_EnabledChanged(object sender, EventArgs e)
        {
            Button btnDaoDefinitionGenColor = (Button)sender;
            if (btnDaoDefinitionGenColor.Enabled)
            {
                btnDaoDefinitionGenColor.BackColor = Color.FromArgb(34, 42, 53);
                btnDaoDefinitionGenColor.ForeColor = Color.White;
            }
            else
            {
                btnDaoDefinitionGenColor.BackColor = Color.FromArgb(191, 202, 207);
                btnDaoDefinitionGenColor.ForeColor = Color.White;
            }
        }

        /// <summary> To get more information on DaoGen tool.</summary> 
        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.CurrentCulture.Name == "ja-JP")
            {
                strJapaneseHelpDoc = GetConfigParameter.GetConfigValue("LnkHelpDoc_Ja");
                Process.Start(strJapaneseHelpDoc);
            }
            else
            {
                strEnglishHelpDoc = GetConfigParameter.GetConfigValue("LnkHelpDoc_En");
                Process.Start(strEnglishHelpDoc);
            }
        }
    }
}

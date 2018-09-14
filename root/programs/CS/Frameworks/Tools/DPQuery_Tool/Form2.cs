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
//* クラス名        ：Form2
//* クラス日本語名  ：動的パラメタライズド・クエリ実行ツール（結果画面）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2008/xx/xx  西野 大介         新規作成
//*  2014/04/24  Rituparna         Created Resource files for UI language changes and moved the English 
//*                                and Japanese languages to proper Resouce files.Changed the control size
//*                                to adjust the text properly in different languages.
//*
//*  2014/04/25  Rituparna         Created Resource folder and Resource.ja-JP.resx,Resource.resx files inside
//*                                the Resource folder.Added proper key and values in those files for English and
//*                                Japanese languages.
//*  2014/05/12  Rituparna         Removed <start> and <End> tags
//*  2014/09/16  西野 大介         Overcoming .NET problem of displaying binary columns in a DataGridView.
//*  2014/09/16  Santosh Avaji     Added Code which is required for Automatic Screen generation for Select join statements
//**********************************************************************************

using System;
using System.Data;
using System.Collections;
using System.Resources;
using System.Windows.Forms;

namespace DPQuery_Tool
{
    /// <summary>動的パラメタライズド・クエリ実行ツール（結果画面）</summary>
    public partial class Form2 : Form
    {
        /// <summary>コンストラクタ</summary>
        public Form2()
        {
            InitializeComponent();

            // フローレイアウト風にする。
            this.tabControl1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.dataGridView1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.richTextBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.richTextBox2.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.btnCloseScreen.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.btnGenerateScreens.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
        }

        #region プロパティ

        /// <summary>
        /// 表示する結果セット（dt）
        /// </summary>
        public DataTable _dt;

        /// <summary>
        /// 結果セットを取得したSQL文
        /// </summary>
        public string _sql;

        /// <summary>
        /// ログ出力されるSQL文
        /// </summary>
        public string _log;

        #endregion

        // Select join screen variables
        ArrayList allcol = new ArrayList();
        ArrayList allTables = new ArrayList();
        ArrayList FullDataset = new ArrayList();
        string[] arraclos = null;

        #region 処理

        /// <summary>
        /// ロード処理のみ。
        /// </summary>
        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = this.RM_GetString("Result") + this._dt.TableName;

            this.dataGridView1.DataSource = this._dt;
            this.richTextBox1.Text = this._sql;
            this.richTextBox2.Text = this._log;
            
            this.dataGridView1.DataError += new DataGridViewDataErrorEventHandler(DataGridView_DataError);
        }

        //DataErrorイベントハンドラ
        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = false;
        }

        /// <summary>
        /// 閉じる。
        /// </summary>
        private void btnCloseScreen_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>This Method gets the string values from resource file based on the key passed</summary>
        private string RM_GetString(string key)
        {
            // get the string value from resource file  by proper passing key.
            ResourceManager rm = Resources.Resource.ResourceManager;
            return rm.GetString(key);
        }

        #endregion

        /// <summary>
        /// Generate Automatic Screen
        /// </summary>
        private void btnGenerateScreens_Click(object sender, EventArgs e)
        {
            if (_dt.Columns.Count > 0)
            {
                // Get all Table names in Select join Statements data table
                foreach (DataColumn column in _dt.Columns)
                {
                    arraclos = column.ColumnName.Split('.');
                    if (arraclos.Length > 1)
                    {
                        if (!allTables.Contains(arraclos[0].ToString()))
                        {
                            allTables.Add(arraclos[0]);
                        }
                    }
                }
                //Get all Column names from Select join Statements data table
                foreach (DataColumn column in _dt.Columns)
                {

                    if (!FullDataset.Contains(column.ColumnName))
                    {
                        FullDataset.Add(column.ColumnName);
                    }

                }
                //Call the form2 to generate screens and pass the required information
                Form3 frm2 = new Form3();
                frm2.DPQTableNames = allTables;
                frm2.DPQAllCoumns = FullDataset;
                frm2.Show();
            }
            else
            {
                MessageBox.Show(this.RM_GetString("Nocolumns"));
            }
        }
    }
}

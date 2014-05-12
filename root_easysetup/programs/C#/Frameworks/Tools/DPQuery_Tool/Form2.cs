//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//*  2008/xx/xx  西野  大介        新規作成
//**********************************************************************************

// Windowアプリケーション
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;

// 業務フレームワーク（参照しない）
// フレームワーク（参照しない）

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

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
            this.button1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
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

        #region 処理
        
        /// <summary>
        /// ロード処理のみ。
        /// </summary>
        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = "結果：" + this._dt.TableName;
            this.dataGridView1.DataSource = this._dt;
            this.richTextBox1.Text = this._sql;
            this.richTextBox2.Text = this._log;
        }

        /// <summary>
        /// 閉じる。
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}

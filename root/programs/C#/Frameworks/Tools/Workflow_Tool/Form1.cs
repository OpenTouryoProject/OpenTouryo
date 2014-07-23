//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//* クラス日本語名  ：Workflowシミュレータツール
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2014/07/23  西野  大介        新規作成
//*
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

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Business.Workflow;

//using Touryo.Infrastructure.Business.RichClient.Asynchronous;
//using Touryo.Infrastructure.Business.RichClient.Presentation;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

//using Touryo.Infrastructure.Framework.RichClient.Presentation;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using System.Resources;

namespace Workflow_Tool
{
    /// <summary>Workflowシミュレータツール</summary>
    public partial class Form1 : Form
    {
        /// <summary>Dam</summary>
        BaseDam _dam = null;

        /// <summary>コンストラクタ</summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>ロード</summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // 現状SQLServerのみ対応
            this.txtCnnstr.Text = GetConfigParameter.GetConnectionString("ConnectionString_SQL");

            this.txtSubSystemId.Text = GetConfigParameter.GetConfigValue("SubSystemId");
            this.txtWorkflowName.Text = GetConfigParameter.GetConfigValue("WorkflowName");

            // SQLは埋め込まれたリソースを使用する。
            Touryo.Infrastructure.Business.Dao.MyBaseDao.UseEmbeddedResource = true;
        }

        /// <summary>新しいワークフローを準備します。</summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // Damの初期化
            this.InitDam();

            try
            {
                Workflow wf = new Workflow(this._dam);

                DataTable dt = new DataTable();
                DataTable dt1 = null;
                DataTable dt2 = null;

                if (!string.IsNullOrEmpty(this.txtFromDearSirUID.Text))
                {
                    dt1 = wf.PreStartWorkflow(
                        this.txtSubSystemId.Text, this.txtWorkflowName.Text, decimal.Parse(this.txtFromDearSirUID.Text));
                }
                if (!string.IsNullOrEmpty(this.txtFromUserID.Text))
                {
                    dt2 = wf.PreStartWorkflow(
                        this.txtSubSystemId.Text, this.txtWorkflowName.Text, decimal.Parse(this.txtFromUserID.Text));
                }

                // 表示
                if (dt1 != null)
                {
                    dt.Merge(dt1);
                }
                if (dt2 != null)
                {
                    dt.Merge(dt2);
                }

                this.dataGridView1.DataSource = dt;

                this._dam.CommitTransaction();
            }
            catch(Exception ex)
            {
                this._dam.RollbackTransaction();

                MessageBox.Show(
                    "Message:" + ex.Message + "\n" + "StackTrace:" + ex.StackTrace,
                    "エラーです", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this._dam.ConnectionClose();
            }
        }

        /// <summary>新しいワークフローを準備します。</summary>
        private void button2_Click(object sender, EventArgs e)
        {
            // Damの初期化
            this.InitDam();

            // ワークフロー管理番号の生成
            if(string.IsNullOrEmpty(txtWorkflowControlNo.Text))
            {
                txtWorkflowControlNo.Text = Guid.NewGuid().ToString();
            }

            try
            {
                Workflow wf = new Workflow(this._dam);

                DataRow startWorkflow = null; 
                DataTable dt = (DataTable)this.dataGridView1.DataSource;

                DataGridViewRow dgvr = (this.dataGridView1.SelectedRows[0]); 
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr[0] == dgvr.Cells[0].Value)
                    {
                        startWorkflow = dr;
                    }
                }

                string fromUserInfo = "";
                string toUserInfo ="";


                int mail = 0;
                mail = wf.StartWorkflow(startWorkflow, txtWorkflowControlNo.Text, );

                //
                //if (!string.IsNullOrEmpty(this.txtFromDearSirUID.Text))
                //{
                //    
                //}

                this._dam.CommitTransaction();

                
            }
            catch (Exception ex)
            {
                this._dam.RollbackTransaction();
            }
            finally
            {
                this._dam.ConnectionClose();
            }
        }

        #region 共通関数

        /// <summary>Damの初期化</summary>
        private void InitDam()
        {
            // 現状SQLServerのみ対応
            this._dam = new DamSqlSvr();
            this._dam.Obj = new MyUserInfo("Workflow_Tool", "127.0.0.1");

            this._dam.ConnectionOpen(this.txtCnnstr.Text);
            this._dam.BeginTransaction(DbEnum.IsolationLevelEnum.ReadCommitted);
        }

        private string GetUserInfo(int uid)
        {

        }
        #endregion
    }
}

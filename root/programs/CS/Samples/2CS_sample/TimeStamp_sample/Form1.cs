//**********************************************************************************
//* タイムスタンプ・サンプル アプリ
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
//*
//**********************************************************************************

using System;
using System.Data;
using System.Windows.Forms;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Util;

namespace _TimeStamp_sample
{
    /// <summary>サンプル アプリ画面</summary>
    public partial class Form1 : Form
    {
        // タイムスタンプ オブジェクトの格納
        private object ts;

        #region データアクセス

        // データアクセス制御クラス
        DamSqlSvr dam = null;

        // Dao

        // datetime
        // 末端
        Daots_test_table1 dao1 = null;
        // 中間
        Daots_test_table2 dao2 = null;
        // 先頭
        Daots_test_table3 dao3 = null;

        // timestamp
        // 末端
        Daots_test_tableA daoA = null;
        // 中間
        Daots_test_tableB daoB = null;
        // 先頭
        Daots_test_tableC daoC = null;

        #endregion

        #region 開始-終了処理

        /// <summary>コンストラクタ</summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>開始処理</summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // イベントハンドラ
            this.dataGridView1.DataError += new DataGridViewDataErrorEventHandler(DataGridView_DataError);
 
            // ステータス
            this.cmbTSColType.SelectedIndex = 0;
            this.cmbTableType.SelectedIndex = 0;

            dam = new DamSqlSvr();
            dam.Obj = new MyUserInfo("userName", Environment.MachineName);
            this.dam.ConnectionOpen(GetConfigParameter.GetConnectionString("ConnectionString_SQL"));
        }

        /// <summary>終了処理</summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.dam.ConnectionClose();
        }

        //DataErrorイベントハンドラ
        private void DataGridView_DataError(object sender,
            DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = false;
        }

        #endregion

        #region 状態の取得処理

        /// <summary>状態の取得</summary>
        /// <returns>状態を表す数値</returns>
        private int GetStatus()
        {
            if (this.cmbTSColType.Text == "RAND（float）列")
            {
                if (this.cmbTableType.Text == "TS列末端") { return 1; }
                else if (this.cmbTableType.Text == "TS列中間") { return 2; }
                else if (this.cmbTableType.Text == "TS列先頭") { return 3; }
            }
            else if (this.cmbTSColType.Text == "timestamp列")
            {
                if (this.cmbTableType.Text == "TS列末端") { return 4; }
                else if (this.cmbTableType.Text == "TS列中間") { return 5; }
                else if (this.cmbTableType.Text == "TS列先頭") { return 6; }
            }

            throw new Exception("不明な状態です。");
        }

        #endregion

        #region テーブルチェック

        /// <summary>全件取得</summary>
        private void btnGetAll_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            if(this.GetStatus() == 1)
            {
                this.dao1 = new Daots_test_table1(this.dam);
                this.dao1.D2_Select(dt);
            }
            else if (this.GetStatus() == 2)
            {
                this.dao2 = new Daots_test_table2(this.dam);
                this.dao2.D2_Select(dt);
            }
            else if (this.GetStatus() == 3)
            {
                this.dao3 = new Daots_test_table3(this.dam);
                this.dao3.D2_Select(dt);
            }
            else if (this.GetStatus() == 4)
            {
                this.daoA = new Daots_test_tableA(this.dam);
                this.daoA.D2_Select(dt);
            }
            else if (this.GetStatus() == 5)
            {
                this.daoB = new Daots_test_tableB(this.dam);
                this.daoB.D2_Select(dt);
            }
            else if (this.GetStatus() == 6)
            {
                this.daoC = new Daots_test_tableC(this.dam);
                this.daoC.D2_Select(dt);
            }

            this.dataGridView1.DataSource = dt;

        }

        /// <summary>クリア</summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.dataGridView1.DataSource = null;
        }

        #endregion

        /// <summary>タイムスタンプを消す</summary>
        private void btnClearTS_Click(object sender, EventArgs e)
        {
            this.txtTS.Text = "";
            this.ts = null;
        }

        #region Insert

        /// <summary>Insert</summary>
        private void btnInsert_Click(object sender, EventArgs e)
        {
            // 挿入（静的）
            // ・id ：オートインクリメントのため不要
            // ・val：必須
            // ・ts ：自動更新（dao同梱）のため不要
            if (this.GetStatus() == 1)
            {
                this.dao1 = new Daots_test_table1(this.dam);

                //this.dao1.PK_id = int.Parse(this.txtID.Text);
                this.dao1.val = this.txtVAL.Text;
                //this.dao1.ts = this.txtTS.Text;

                this.dao1.S1_Insert();
            }
            else if (this.GetStatus() == 2)
            {
                this.dao2 = new Daots_test_table2(this.dam);

                //this.dao2.PK_id = int.Parse(this.txtID.Text);
                this.dao2.val = this.txtVAL.Text;
                //this.dao2.ts = this.txtTS.Text;

                this.dao2.S1_Insert();
            }
            else if (this.GetStatus() == 3)
            {
                this.dao3 = new Daots_test_table3(this.dam);

                //this.dao3.PK_id = int.Parse(this.txtID.Text);
                this.dao3.val = this.txtVAL.Text;
                //this.dao3.ts = this.txtTS.Text;

                this.dao3.S1_Insert();
            }
            else if (this.GetStatus() == 4)
            {
                this.daoA = new Daots_test_tableA(this.dam);

                //this.daoA.PK_id = int.Parse(this.txtID.Text);
                this.daoA.val = this.txtVAL.Text;
                //this.daoA.ts = this.txtTS.Text;

                this.daoA.S1_Insert();
            }
            else if (this.GetStatus() == 5)
            {
                this.daoB = new Daots_test_tableB(this.dam);

                //this.daoB.PK_id = int.Parse(this.txtID.Text);
                this.daoB.val = this.txtVAL.Text;
                //this.daoB.ts = this.txtTS.Text;

                this.daoB.S1_Insert();
            }
            else if (this.GetStatus() == 6)
            {
                this.daoC = new Daots_test_tableC(this.dam);

                //this.daoC.PK_id = int.Parse(this.txtID.Text);
                this.daoC.val = this.txtVAL.Text;
                //this.daoC.ts = this.txtTS.Text;

                this.daoC.S1_Insert();
            }

            // 更新
            this.btnGetAll_Click(sender, e);
        }

        #endregion

        #region Select

        /// <summary>Select</summary>
        private void btnSelect_Click(object sender, EventArgs e)
        {
            int id = 0;
            DataTable dt = new DataTable();

            if (int.TryParse(this.txtID.Text, out id)) { }
            else
            {
                MessageBox.Show("IDの値が不正です。");
                return;
            }

            // 参照（静的）
            // ・id ：静的
            // ・val：なし
            // ・ts ：動的
            if (this.GetStatus() == 1)
            {
                this.dao1 = new Daots_test_table1(this.dam);

                this.dao1.PK_id = id;
                //this.dao1.val = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.dao1.ts = this.ts; }

                this.dao1.S2_Select(dt);
            }
            else if (this.GetStatus() == 2)
            {
                this.dao2 = new Daots_test_table2(this.dam);

                this.dao2.PK_id = id;
                //this.dao2.val = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.dao2.ts = this.ts; }

                this.dao2.S2_Select(dt);
            }
            else if (this.GetStatus() == 3)
            {
                this.dao3 = new Daots_test_table3(this.dam);

                this.dao3.PK_id = id;
                //this.dao3.val = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.dao3.ts = this.ts; }

                this.dao3.S2_Select(dt);
            }
            else if (this.GetStatus() == 4)
            {
                this.daoA = new Daots_test_tableA(this.dam);

                this.daoA.PK_id = id;
                //this.daoA.val = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.daoA.ts = this.ts; }

                this.daoA.S2_Select(dt);
            }
            else if (this.GetStatus() == 5)
            {
                this.daoB = new Daots_test_tableB(this.dam);

                this.daoB.PK_id = id;
                //this.daoB.val = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.daoB.ts = this.ts; }

                this.daoB.S2_Select(dt);
            }
            else if (this.GetStatus() == 6)
            {
                this.daoC = new Daots_test_tableC(this.dam);

                this.daoC.PK_id = id;
                //this.daoC.val = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.daoC.ts = this.ts; }

                this.daoC.S2_Select(dt);
            }

            // 表示
            if (dt.Rows.Count != 0)
            {
                this.txtID.Text = dt.Rows[0]["id"].ToString();
                this.txtVAL.Text = dt.Rows[0]["val"].ToString();

                // 文字列化の方法
                if (dt.Rows[0]["ts"].ToString() == "System.Byte[]")
                {
                    // timestamp
                    this.txtTS.Text = BitConverter.ToString((byte[])dt.Rows[0]["ts"]);
                }
                else
                {
                    // timeticks
                    this.txtTS.Text = dt.Rows[0]["ts"].ToString();
                }

                // → 文字列化 → バイト化とか解らんので退避しておく・・・
                this.ts = dt.Rows[0]["ts"];
            }
            else
            {
                this.txtID.Text = "";
                this.txtVAL.Text = "";
                this.txtTS.Text = "";
                this.ts = null;
            }
        }

        #endregion

        #region Update

        /// <summary>Update</summary>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int id = 0;

            if (int.TryParse(this.txtID.Text, out id)) { }
            else
            {
                MessageBox.Show("IDの値が不正です。");
                return;
            }

            // 更新（静的）
            // ・id ：検索条件（静的）
            // ・val：更新値・機械的に指定（パラメタが０個になるので）
            // ・ts ：検索条件（動的）
            if (this.GetStatus() == 1)
            {
                this.dao1 = new Daots_test_table1(this.dam);

                this.dao1.PK_id = id;
                this.dao1.Set_val_forUPD = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.dao1.ts = this.ts; }

                this.dao1.S3_Update();
            }
            else if (this.GetStatus() == 2)
            {
                this.dao2 = new Daots_test_table2(this.dam);

                this.dao2.PK_id = id;
                this.dao2.Set_val_forUPD = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.dao2.ts = this.ts; }

                this.dao2.S3_Update();
            }
            else if (this.GetStatus() == 3)
            {
                this.dao3 = new Daots_test_table3(this.dam);

                this.dao3.PK_id = id;
                this.dao3.Set_val_forUPD = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.dao3.ts = this.ts; }

                this.dao3.S3_Update();
            }
            else if (this.GetStatus() == 4)
            {
                this.daoA = new Daots_test_tableA(this.dam);

                this.daoA.PK_id = id;
                this.daoA.Set_val_forUPD = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.daoA.ts = this.ts; }

                this.daoA.S3_Update();
            }
            else if (this.GetStatus() == 5)
            {
                this.daoB = new Daots_test_tableB(this.dam);

                this.daoB.PK_id = id;
                this.daoB.Set_val_forUPD = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.daoB.ts = this.ts; }

                this.daoB.S3_Update();
            }
            else if (this.GetStatus() == 6)
            {
                this.daoC = new Daots_test_tableC(this.dam);

                this.daoC.PK_id = id;
                this.daoC.Set_val_forUPD = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.daoC.ts = this.ts; }

                this.daoC.S3_Update();
            }

            // 更新
            this.btnGetAll_Click(sender, e);
        }

        #endregion

        #region Delete

        /// <summary>Delete</summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int id = 0;
            DataTable dt = new DataTable();

            if (int.TryParse(this.txtID.Text, out id)) { }
            else
            {
                MessageBox.Show("IDの値が不正です。");
                return;
            }

            // 削除（静的）
            // ・id ：静的
            // ・val：なし
            // ・ts ：動的
            if (this.GetStatus() == 1)
            {
                this.dao1 = new Daots_test_table1(this.dam);

                this.dao1.PK_id = id;
                //this.dao1.val = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.dao1.ts = this.ts; }

                this.dao1.S4_Delete();
            }
            else if (this.GetStatus() == 2)
            {
                this.dao2 = new Daots_test_table2(this.dam);

                this.dao2.PK_id = id;
                //this.dao2.val = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.dao2.ts = this.ts; }

                this.dao2.S4_Delete();
            }
            else if (this.GetStatus() == 3)
            {
                this.dao3 = new Daots_test_table3(this.dam);

                this.dao3.PK_id = id;
                //this.dao3.val = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.dao3.ts = this.ts; }

                this.dao3.S4_Delete();
            }
            else if (this.GetStatus() == 4)
            {
                this.daoA = new Daots_test_tableA(this.dam);

                this.daoA.PK_id = id;
                //this.daoA.val = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.daoA.ts = this.ts; }

                this.daoA.S4_Delete();
            }
            else if (this.GetStatus() == 5)
            {
                this.daoB = new Daots_test_tableB(this.dam);

                this.daoB.PK_id = id;
                //this.daoB.val = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.daoB.ts = this.ts; }

                this.daoB.S4_Delete();
            }
            else if (this.GetStatus() == 6)
            {
                this.daoC = new Daots_test_tableC(this.dam);

                this.daoC.PK_id = id;
                //this.daoC.val = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.daoC.ts = this.ts; }

                this.daoC.S4_Delete();
            }

            // 更新
            this.btnGetAll_Click(sender, e);
        }

        #endregion

        #region DynIns

        /// <summary>DynIns</summary>
        private void btnDynIns_Click(object sender, EventArgs e)
        {
            // 挿入（動的）
            // ・id ：オートインクリメントのため不要
            // ・val：機械的に指定（パラメタが０個になるので）
            // ・ts ：自動更新（dao同梱）のため不要
            if (this.GetStatus() == 1)
            {
                this.dao1 = new Daots_test_table1(this.dam);

                //this.dao1.PK_id = int.Parse(this.txtID.Text);
                this.dao1.val = this.txtVAL.Text;
                //this.dao1.ts = this.txtTS.Text;

                this.dao1.D1_Insert();
            }
            else if (this.GetStatus() == 2)
            {
                this.dao2 = new Daots_test_table2(this.dam);

                //this.dao2.PK_id = int.Parse(this.txtID.Text);
                this.dao2.val = this.txtVAL.Text;
                //this.dao2.ts = this.txtTS.Text;

                this.dao2.D1_Insert();
            }
            else if (this.GetStatus() == 3)
            {
                this.dao3 = new Daots_test_table3(this.dam);

                //this.dao3.PK_id = int.Parse(this.txtID.Text);
                this.dao3.val = this.txtVAL.Text;
                //this.dao3.ts = this.txtTS.Text;

                this.dao3.D1_Insert();
            }
            else if (this.GetStatus() == 4)
            {
                this.daoA = new Daots_test_tableA(this.dam);

                //this.daoA.PK_id = int.Parse(this.txtID.Text);
                this.daoA.val = this.txtVAL.Text;
                //this.daoA.ts = this.txtTS.Text;

                this.daoA.D1_Insert();
            }
            else if (this.GetStatus() == 5)
            {
                this.daoB = new Daots_test_tableB(this.dam);

                //this.daoB.PK_id = int.Parse(this.txtID.Text);
                this.daoB.val = this.txtVAL.Text;
                //this.daoB.ts = this.txtTS.Text;

                this.daoB.D1_Insert();
            }
            else if (this.GetStatus() == 6)
            {
                this.daoC = new Daots_test_tableC(this.dam);

                //this.daoC.PK_id = int.Parse(this.txtID.Text);
                this.daoC.val = this.txtVAL.Text;
                //this.daoC.ts = this.txtTS.Text;

                this.daoC.D1_Insert();
            }

            // 更新
            this.btnGetAll_Click(sender, e);
        }

        #endregion

        #region DynSel

        /// <summary>DynSel</summary>
        private void btnDynSel_Click(object sender, EventArgs e)
        {
            int id = 0;
            bool flg = false;

            DataTable dt = new DataTable();

            flg = int.TryParse(this.txtID.Text, out id);

            // 参照（動的）
            // ・id ：動的
            // ・val：動的
            // ・ts ：動的
            if (this.GetStatus() == 1)
            {
                this.dao1 = new Daots_test_table1(this.dam);

                if (flg) { this.dao1.PK_id = id; }
                if (this.txtVAL.Text != "") { this.dao1.val = this.txtVAL.Text; }
                if (this.txtTS.Text != "") { this.dao1.ts = this.ts; }

                this.dao1.D2_Select(dt);
            }
            else if (this.GetStatus() == 2)
            {
                this.dao2 = new Daots_test_table2(this.dam);

                if (flg) { this.dao2.PK_id = id; }
                if (this.txtVAL.Text != "") { this.dao2.val = this.txtVAL.Text; }
                if (this.txtTS.Text != "") { this.dao2.ts = this.ts; }

                this.dao2.D2_Select(dt);
            }
            else if (this.GetStatus() == 3)
            {
                this.dao3 = new Daots_test_table3(this.dam);

                if (flg) { this.dao3.PK_id = id; }
                if (this.txtVAL.Text != "") { this.dao3.val = this.txtVAL.Text; }
                if (this.txtTS.Text != "") { this.dao3.ts = this.ts; }

                this.dao3.D2_Select(dt);
            }
            else if (this.GetStatus() == 4)
            {
                this.daoA = new Daots_test_tableA(this.dam);

                if (flg) { this.daoA.PK_id = id; }
                if (this.txtVAL.Text != "") { this.daoA.val = this.txtVAL.Text; }
                if (this.txtTS.Text != "") { this.daoA.ts = this.ts; }

                this.daoA.D2_Select(dt);
            }
            else if (this.GetStatus() == 5)
            {
                this.daoB = new Daots_test_tableB(this.dam);

                if (flg) { this.daoB.PK_id = id; }
                if (this.txtVAL.Text != "") { this.daoB.val = this.txtVAL.Text; }
                if (this.txtTS.Text != "") { this.daoB.ts = this.ts; }

                this.daoB.D2_Select(dt);
            }
            else if (this.GetStatus() == 6)
            {
                this.daoC = new Daots_test_tableC(this.dam);

                if (flg) { this.daoC.PK_id = id; }
                if (this.txtVAL.Text != "") { this.daoC.val = this.txtVAL.Text; }
                if (this.txtTS.Text != "") { this.daoC.ts = this.ts; }

                this.daoC.D2_Select(dt);
            }

            this.dataGridView1.DataSource = dt;
        }

        #endregion

        #region DynUpd

        /// <summary>DynUpd</summary>
        private void btnDynUpd_Click(object sender, EventArgs e)
        {
            int id = 0;

            if (int.TryParse(this.txtID.Text, out id)) { }
            else
            {
                MessageBox.Show("IDの値が不正です。");
                return;
            }

            // 更新（動的）
            // ・id ：検索条件（静的）
            // ・val：更新値・機械的に指定（パラメタが０個になるので）
            // ・ts ：検索条件（動的）
            if (this.GetStatus() == 1)
            {
                this.dao1 = new Daots_test_table1(this.dam);

                this.dao1.PK_id = id;
                this.dao1.Set_val_forUPD = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.dao1.ts = this.ts; }

                this.dao1.D3_Update();
            }
            else if (this.GetStatus() == 2)
            {
                this.dao2 = new Daots_test_table2(this.dam);

                this.dao2.PK_id = id;
                this.dao2.Set_val_forUPD = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.dao2.ts = this.ts; }

                this.dao2.D3_Update();
            }
            else if (this.GetStatus() == 3)
            {
                this.dao3 = new Daots_test_table3(this.dam);

                this.dao3.PK_id = id;
                this.dao3.Set_val_forUPD = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.dao3.ts = this.ts; }

                this.dao3.D3_Update();
            }
            else if (this.GetStatus() == 4)
            {
                this.daoA = new Daots_test_tableA(this.dam);

                this.daoA.PK_id = id;
                this.daoA.Set_val_forUPD = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.daoA.ts = this.ts; }

                this.daoA.D3_Update();
            }
            else if (this.GetStatus() == 5)
            {
                this.daoB = new Daots_test_tableB(this.dam);

                this.daoB.PK_id = id;
                this.daoB.Set_val_forUPD = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.daoB.ts = this.ts; }

                this.daoB.D3_Update();
            }
            else if (this.GetStatus() == 6)
            {
                this.daoC = new Daots_test_tableC(this.dam);

                this.daoC.PK_id = id;
                this.daoC.Set_val_forUPD = this.txtVAL.Text;
                if (this.txtTS.Text != "") { this.daoC.ts = this.ts; }

                this.daoC.D3_Update();
            }

            // 更新
            this.btnGetAll_Click(sender, e);
        }

        #endregion

        #region DynDel

        /// <summary>DynDel</summary>
        private void btnDynDel_Click(object sender, EventArgs e)
        {
            int id = 0;
            bool flg = false;

            flg = int.TryParse(this.txtID.Text, out id);

            // 削除（動的）
            // ・id ：動的
            // ・val：動的
            // ・ts ：動的
            if (this.GetStatus() == 1)
            {
                this.dao1 = new Daots_test_table1(this.dam);

                if (flg) { this.dao1.PK_id = id; }
                if (this.txtVAL.Text != "") { this.dao1.val = this.txtVAL.Text; }
                if (this.txtTS.Text != "") { this.dao1.ts = this.ts; }

                this.dao1.D4_Delete();
            }
            else if (this.GetStatus() == 2)
            {
                this.dao2 = new Daots_test_table2(this.dam);

                if (flg) { this.dao2.PK_id = id; }
                if (this.txtVAL.Text != "") { this.dao2.val = this.txtVAL.Text; }
                if (this.txtTS.Text != "") { this.dao2.ts = this.ts; }

                this.dao2.D4_Delete();
            }
            else if (this.GetStatus() == 3)
            {
                this.dao3 = new Daots_test_table3(this.dam);

                if (flg) { this.dao3.PK_id = id; }
                if (this.txtVAL.Text != "") { this.dao3.val = this.txtVAL.Text; }
                if (this.txtTS.Text != "") { this.dao3.ts = this.ts; }

                this.dao3.D4_Delete();
            }
            else if (this.GetStatus() == 4)
            {
                this.daoA = new Daots_test_tableA(this.dam);

                if (flg) { this.daoA.PK_id = id; }
                if (this.txtVAL.Text != "") { this.daoA.val = this.txtVAL.Text; }
                if (this.txtTS.Text != "") { this.daoA.ts = this.ts; }

                this.daoA.D4_Delete();
            }
            else if (this.GetStatus() == 5)
            {
                this.daoB = new Daots_test_tableB(this.dam);

                if (flg) { this.daoB.PK_id = id; }
                if (this.txtVAL.Text != "") { this.daoB.val = this.txtVAL.Text; }
                if (this.txtTS.Text != "") { this.daoB.ts = this.ts; }

                this.daoB.D4_Delete();
            }
            else if (this.GetStatus() == 6)
            {
                this.daoC = new Daots_test_tableC(this.dam);

                if (flg) { this.daoC.PK_id = id; }
                if (this.txtVAL.Text != "") { this.daoC.val = this.txtVAL.Text; }
                if (this.txtTS.Text != "") { this.daoC.ts = this.ts; }

                this.daoC.D4_Delete();
            }

            // 更新
            this.btnGetAll_Click(sender, e);
        }

        #endregion
    }
}

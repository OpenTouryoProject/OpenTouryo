//**********************************************************************************
//* フレームワーク・テスト画面（Ｐ層）
//**********************************************************************************

// テスト画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：testWCTextBox
//* クラス日本語名  ：Web Custom Control部品テスト画面
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
using System.Data;
using System.Collections.Generic;

using Touryo.Infrastructure.CustomControl;

namespace ProjectX_sample.Aspx.TestFxLayerP.Wcc
{
    /// <summary>testWCTextBox class</summary>
    public partial class testWCTextBox : System.Web.UI.Page
    {
        #region 初期処理


        /// <summary>ヘッダーに表示する文字列</summary>
        public Dictionary<string, string> HeaderInfo = new Dictionary<string, string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            // 初回Load時に、データソースを
            // 生成 ＆ データバインドする。
            this.gvwGridView1.Columns[0].HeaderText = "ID";
            this.gvwGridView1.Columns[1].HeaderText = "チェック";
            this.gvwGridView1.Columns[2].HeaderText = "必須入力";
            this.gvwGridView1.Columns[3].HeaderText = "半角";
            this.gvwGridView1.Columns[4].HeaderText = "全角";
            this.gvwGridView1.Columns[5].HeaderText = "数値";
            this.gvwGridView1.Columns[6].HeaderText = "片仮名";
            this.gvwGridView1.Columns[7].HeaderText = "半角片仮名";
            this.gvwGridView1.Columns[8].HeaderText = "平仮名";
            this.gvwGridView1.Columns[9].HeaderText = "日付";
            this.gvwGridView1.Columns[10].HeaderText = "正規表現（メアド）";
            this.gvwGridView1.Columns[11].HeaderText = "禁則";
            this.gvwGridView1.Columns[12].HeaderText = "半角＆禁則";
            this.gvwGridView1.Columns[13].HeaderText = "全角＆数値";

            if (!this.IsPostBack)
            {
                this.CreateDataSource();
                this.BindGridData();
            }

            this.TextBox1.Text = "";
        }


        /// <summary>DataSourceを生成</summary>
        /// <returns>Datatableを返す</returns>
        private void CreateDataSource()
        {
            DataTable dt = new DataTable();
            DataRow dr;

            // 列生成
            dt.Columns.Add(new DataColumn("fileid", typeof(int)));
            dt.Columns.Add(new DataColumn("field1", typeof(string)));
            dt.Columns.Add(new DataColumn("field2", typeof(string)));
            dt.Columns.Add(new DataColumn("field3", typeof(string)));
            dt.Columns.Add(new DataColumn("field4", typeof(string)));
            dt.Columns.Add(new DataColumn("field5", typeof(string)));
            dt.Columns.Add(new DataColumn("field6", typeof(string)));
            dt.Columns.Add(new DataColumn("field7", typeof(string)));
            dt.Columns.Add(new DataColumn("field8", typeof(string)));
            dt.Columns.Add(new DataColumn("field9", typeof(string)));
            dt.Columns.Add(new DataColumn("field10", typeof(string)));
            dt.Columns.Add(new DataColumn("field11", typeof(string)));
            dt.Columns.Add(new DataColumn("field12", typeof(string)));
            dt.Columns.Add(new DataColumn("field13", typeof(string)));

            // 行生成
            for (int i = 1; i < 10; i++)
            {
                dr = dt.NewRow();
                dr["fileid"] = i;
                dt.Rows.Add(dr);
            }

            // 変更のコミット
            dt.AcceptChanges();

            // DataTableをSessionに格納する
            Session["SampleData"] = dt;
        }

        /// <summary>データバインドする</summary>
        private void BindGridData()
        {
            this.gvwGridView1.DataSource = Session["SampleData"];
            this.gvwGridView1.DataBind();
        }

        #endregion

        /// <summary>バッチ・チェック処理</summary>
        protected void btnCheckText_Click(object sender, EventArgs e)
        {
            // 一括チェック処理
            List<CheckResult> rets = new List<CheckResult>();
            if (CmnCheckFunction.HasErrors(this, rets))
            {
                foreach (CheckResult ret in rets)
                {
                    this.TextBox1.Text += ret.CtrlName + "\r\n";
                    foreach (string s in ret.CheckErrorInfo)
                    {
                        this.TextBox1.Text += "・" + s + "\r\n";
                    }
                }
            }
        }
    } 
}

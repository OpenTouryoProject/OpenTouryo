//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testFxLayerP_table_testListView
//* クラス日本語名  ：ListViewテスト画面（Ｐ層）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  
//*  2014/08/18  Sai-San           Created test page for ListView control
//*  2014/10/03  Rituparna         Added ItemCommandEvent to ListView control
//**********************************************************************************

// System
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
// System.Web

using System.Web.UI.WebControls;
// 業務フレームワーク
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.CustomControl;
// フレームワーク
using Touryo.Infrastructure.Framework.Presentation;

// 部品

namespace ProjectX_sample.Aspx.testFxLayerP.table
{
    /// <summary>ListView test screen (P layer）</summary>
    public partial class testListView : MyBaseController
    {
        #region 初期化

        /// <summary>ヘッダーに表示する文字列</summary>
        public Dictionary<string, string> HeaderInfo = new Dictionary<string, string>();

        /// <summary>Page_InitイベントでASP.NET標準イベントハンドラを設定</summary>
        protected void Page_Init(object sender, EventArgs e)
        {
        }

        /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit()
        {
            // フォーム初期化（初回ロード）時に実行する処理を実装する
            // TODO:
            // 初回ロード時に、データソースを
            // 生成 ＆ データバインドする。
            this.CreateDataSource();
            this.BindListViewData();
        }

        /// <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
        /// <remarks>実装必須</remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // フォーム初期化（ポストバック）時に実行する処理を実装する
            this.CmnInit();

            // Radio Buttonの選択状態を出力
            if (Request.Form["radio-grp1"] != null)
            {
                Response.Write(string.Format(
                        "name=\"radio-grp1\" value=\"{0}\"が選択されました。<br/>",
                        Request.Form["radio-grp1"].ToString()));
            }

            int i = 0;
            foreach (ListViewDataItem lvwItem in this.lvwListView1.Items)
            {
                i++;
                WebCustomRadioButton rbn = (WebCustomRadioButton)lvwItem.FindControl("rbnRadioButton");

                // チェック
                if (rbn == null)
                {
                    // == null
                }
                else
                {
                    // != null
                    if (rbn.Checked)
                    {
                        Response.Write(string.Format(
                            "name=\"radio-grp1\" value=\"{0}\"行目が選択されました。<br/>", i.ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// Sets the header information
        /// </summary>
        private void CmnInit()
        {
            // ヘッダーに表示する文字列を初期化
            this.HeaderInfo.Add("col0", "Select1");
            this.HeaderInfo.Add("col1", "Select2");
            this.HeaderInfo.Add("col2", "Custom");
            this.HeaderInfo.Add("col3", "FileID");
            this.HeaderInfo.Add("col4", "Readonly");
            this.HeaderInfo.Add("col5", "FileName");
            this.HeaderInfo.Add("col6", "FileSize");
            this.HeaderInfo.Add("col7", "Date");
            this.HeaderInfo.Add("col8", "Edit");
            this.HeaderInfo.Add("col9", "Delete");
            this.HeaderInfo.Add("col10", "Dropdown");
        }

        /// <summary>
        /// ★http://bbs.wankuma.com/index.cgi?mode=al2&namber=41245&KLOG=71
        /// </summary>
        protected void UOC_lvwListView1_PagePropertiesChanged(FxEventArgs fxEventArgs, EventArgs e)
        {
            this.CreateDataSource();
            this.BindListViewData();
        }

        #endregion

        #region データソースの生成

        /// <summary>DataSourceを生成</summary>
        /// <returns>Datatableを返す</returns>
        private void CreateDataSource()
        {
            // Server.MapPathはアプリケーション ディレクトリを指す。
            DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Aspx/Common"));
            FileInfo[] fi = di.GetFiles();

            // Datatableに
            // アプリケーション ディレクトリの
            // ファイル情報を設定する。
            DataTable dt = new DataTable();
            DataRow dr;

            // 列生成
            dt.Columns.Add(new DataColumn("fileid", typeof(int)));
            dt.Columns.Add(new DataColumn("filename", typeof(String)));
            dt.Columns.Add(new DataColumn("readonly", typeof(Boolean)));
            dt.Columns.Add(new DataColumn("filesize", typeof(long)));
            dt.Columns.Add(new DataColumn("date", typeof(DateTime)));

            // 行生成
            for (int i = 0; i < fi.Length; i++)
            {
                dr = dt.NewRow();
                dr["fileid"] = i;
                dr["filename"] = fi[i].Name;
                dr["readonly"] = fi[i].IsReadOnly;
                dr["filesize"] = fi[i].Length;
                dr["date"] = fi[i].LastWriteTime;
                dt.Rows.Add(dr);
            }

            // 変更のコミット
            dt.AcceptChanges();

            // DataTableをSessionに格納する
            Session["SampleData"] = dt;
        }

        /// <summary>データバインドする</summary>
        private void BindListViewData()
        {
            this.lvwListView1.DataSource = Session["SampleData"];
            this.lvwListView1.DataBind();
        }

        #region 通常のイベント

        /// <summary>btnButton1のクリックイベント</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
        {
            // ポストバックをまたいで値が保存されるかの確認
            return "";
        }

        #endregion

        /// <summary>
        /// ListView Item Editing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lvwListView1_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            lvwListView1.EditIndex = e.NewEditIndex;
            this.lvwListView1.DataSource = Session["SampleData"];
            this.lvwListView1.DataBind();
        }

        /// <summary>
        /// ListView Item Canceling event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lvwListView1_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            lvwListView1.EditIndex = -1;
            this.lvwListView1.DataSource = Session["SampleData"];
            this.lvwListView1.DataBind();
        }

        /// <summary>
        /// ListView Item Updating event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UOC_lvwListView1_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            TextBox txtFileID = (TextBox)lvwListView1.Items[e.ItemIndex].FindControl("txtFileID");
            TextBox txtFileName = (TextBox)lvwListView1.Items[e.ItemIndex].FindControl("txtFileName");
            CheckBox cbxReadonly = (CheckBox)lvwListView1.Items[e.ItemIndex].FindControl("cbxReadonly");
            TextBox txtFileSize = (TextBox)lvwListView1.Items[e.ItemIndex].FindControl("txtFileSize");
            TextBox txtDate = (TextBox)lvwListView1.Items[e.ItemIndex].FindControl("txtDate");

            // Gets the updated values from controls for update
            int fileid = (int)this.lvwListView1.DataKeys[e.ItemIndex].Value;
            DataTable dt = (DataTable)Session["SampleData"];
            DataRow row = dt.Select(string.Format("fileid = '{0}'", fileid))[0];
            row["fileid"] = txtFileID.Text;
            row["filename"] = txtFileName.Text;
            row["readonly"] = cbxReadonly.Checked;
            row["filesize"] = txtFileSize.Text;
            row["date"] = txtDate.Text;

            //Sets ListView Edit mode to Normal mode
            this.lvwListView1.EditIndex = -1;
            this.BindListViewData();
        }

        /// <summary>
        /// ListView Item Deleting event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected string UOC_lvwListView1_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            DataTable dt = (DataTable)Session["SampleData"];
            int fileid = (int)this.lvwListView1.DataKeys[e.ItemIndex].Value;
            dt.Select(string.Format("fileid = '{0}'", fileid))[0].Delete();

            //Sets ListView Edit mode to Normal mode
            this.lvwListView1.EditIndex = -1;
            this.BindListViewData();

            return "";
        }

        /// <summary>
        /// ListView ItemCommand event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected string UOC_lvwListView1_OnItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (String.Equals(e.CommandName, "GetFiedID"))
            {
                lblResultOfItemCommand.Text = "You have clicked the FieldID : " + e.CommandArgument.ToString();
                return "";
            }
            else
                return null;
        }

        /// <summary>
        /// ListView Item Sorting event
        /// </summary>
        /// <param name="fxEventArgs"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected string UOC_lvwListView1_Sorting(FxEventArgs fxEventArgs, ListViewSortEventArgs e)
        {
            // 元のデータ
            DataTable dt1 = (DataTable)Session["SampleData"];

            // ソート後のデータを格納するためのDataTable
            DataTable dt2 = dt1.Clone();

            // データソート用のDataView
            DataView dv = new DataView(dt1);

            if (Session["SortDirection"] == null)
            {
                // ソートの定義情報を格納するためのDictionaryがない場合は作成する
                Session["SortDirection"] = new Dictionary<string, SortDirection>();
            }

            // ソート定義情報にしたがい、データをソートする
            if (!((Dictionary<string, SortDirection>)Session["SortDirection"]).ContainsKey(e.SortExpression))
            {
                // ソート定義情報がない場合。デフォルトは昇順とする
                dv.Sort = e.SortExpression;

                // ソート定義情報を追加する
                ((Dictionary<string, SortDirection>)Session["SortDirection"]).Add(e.SortExpression, SortDirection.Descending);
            }
            else
            {
                // ソート定義情報をもとに、当該列のソート方向を取得する
                SortDirection direction = ((Dictionary<string, SortDirection>)Session["SortDirection"])[e.SortExpression];

                if (direction == SortDirection.Ascending)
                {
                    // 昇順
                    dv.Sort = e.SortExpression;

                    // ソート定義情報を更新する
                    ((Dictionary<string, SortDirection>)Session["SortDirection"])[e.SortExpression] = SortDirection.Descending;
                }
                else
                {
                    // 降順
                    dv.Sort = e.SortExpression + " DESC";

                    // ソート定義情報を更新する
                    ((Dictionary<string, SortDirection>)Session["SortDirection"])[e.SortExpression] = SortDirection.Ascending;
                }
            }

            // ソート後のデータをDataTableにインポートする
            foreach (DataRowView drv in dv)
            {
                dt2.ImportRow(drv.Row);
            }

            // データの再バインド
            Session["SampleData"] = dt2;
            this.BindListViewData();

            return "";
        }

        /// <summary>cbxCheckBox3のCheckedChangedイベント</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_cbxCheckBox3_CheckedChanged(FxEventArgs fxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("--------------------");
            System.Diagnostics.Debug.WriteLine("ButtonID:" + fxEventArgs.ButtonID);
            System.Diagnostics.Debug.WriteLine("InnerButtonID:" + fxEventArgs.InnerButtonID);
            System.Diagnostics.Debug.WriteLine("PostBackValue:" + fxEventArgs.PostBackValue);

            CheckBox cbx = (CheckBox)this.lvwListView1.Items[int.Parse(fxEventArgs.PostBackValue)].FindControl("TextBox1").FindControl("cbxCheckBox3");

            System.Diagnostics.Debug.WriteLine(cbx.Checked.ToString());

            return "";
        }

        /// <summary>ddlDropDownList1のSelectedIndexChangedイベント</summary>
        /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_ddlDropDownList1_SelectedIndexChanged(FxEventArgs fxEventArgs)
        {
            System.Diagnostics.Debug.WriteLine("--------------------");
            System.Diagnostics.Debug.WriteLine("ButtonID:" + fxEventArgs.ButtonID);
            System.Diagnostics.Debug.WriteLine("InnerButtonID:" + fxEventArgs.InnerButtonID);
            System.Diagnostics.Debug.WriteLine("PostBackValue:" + fxEventArgs.PostBackValue);

            DropDownList ddl = (DropDownList)this.lvwListView1.Items[int.Parse(fxEventArgs.PostBackValue)].FindControl("ddlDropDownList1");

            System.Diagnostics.Debug.WriteLine(ddl.SelectedValue);

            return "";
        }

        #endregion

    } 
}
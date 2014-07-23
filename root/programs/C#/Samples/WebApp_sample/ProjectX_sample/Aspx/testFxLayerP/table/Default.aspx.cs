using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// System
using System.IO;
using System.Data;
using System.Text;
using System.Collections;

public partial class Aspx_testFxLayerP_table_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            this.CreateDataSource();
            this.BindGridData();
        }
    }

    protected void ListView1_PagePropertiesChanged(object sender, EventArgs e)
    {
        this.CreateDataSource();
        this.BindGridData();
    }

    #region データソースの生成

    /// <summary>DataSourceを生成</summary>
    /// <returns>Datatableを返す</returns>
    private void CreateDataSource()
    {
        // Server.MapPathはアプリケーション ディレクトリを指す。
        DirectoryInfo di = new DirectoryInfo(Server.MapPath("/ProjectX_sample/Aspx/Common"));
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
    private void BindGridData()
    {
        this.ListView1.DataSource = Session["SampleData"];
        this.ListView1.DataBind();
    }

    #endregion

}
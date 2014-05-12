//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testFxLayerP_table_testGridView
//* クラス日本語名  ：GridViewテスト画面（Ｐ層）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

// System
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;

// System.Web
using System.Web;
using System.Web.Security;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.CustomControl;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

/// <summary>GridViewテスト画面（Ｐ層）</summary>
public partial class Aspx_testFxLayerP_table_testGridView : MyBaseController
{
    #region 初期化

    /// <summary>ヘッダーに表示する文字列</summary>
    public Dictionary<string, string> HeaderInfo = new Dictionary<string, string>();

    /// <summary>Page_InitイベントでASP.NET標準イベントハンドラを設定</summary>
    protected void Page_Init(object sender, EventArgs e)
    {
        // 行編集についてのイベント
        this.gvwGridView1.RowCreated += new GridViewRowEventHandler(gvwGridView1_RowCreated); 
        this.gvwGridView1.RowEditing += new GridViewEditEventHandler(gvwGridView1_RowEditing);
        this.gvwGridView1.RowCancelingEdit += new GridViewCancelEditEventHandler(gvwGridView1_RowCancelingEdit);
        this.gvwGridView1.RowUpdated += new GridViewUpdatedEventHandler(gvwGridView1_RowUpdated);
        this.gvwGridView1.RowDeleted += new GridViewDeletedEventHandler(gvwGridView1_RowDeleted);

        // 行選択についてのイベント
        this.gvwGridView1.SelectedIndexChanging += new GridViewSelectEventHandler(gvwGridView1_SelectedIndexChanging);

        // 列ソートについてのイベント
        this.gvwGridView1.Sorted += new EventHandler(gvwGridView1_Sorted);
    }

    /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit()
    {
        // フォーム初期化（初回ロード）時に実行する処理を実装する
        // TODO:
        this.CmnInit();

        // 初回ロード時に、データソースを
        // 生成 ＆ データバインドする。
        this.CreateDataSource();
        this.gvwGridView1.Columns[0].HeaderText = this.HeaderInfo["col0"];
        this.gvwGridView1.Columns[1].HeaderText = this.HeaderInfo["col1"];
        this.gvwGridView1.Columns[2].HeaderText = this.HeaderInfo["col2"];
        this.gvwGridView1.Columns[3].HeaderText = this.HeaderInfo["col3"];
        this.gvwGridView1.Columns[4].HeaderText = this.HeaderInfo["col4"];
        this.gvwGridView1.Columns[5].HeaderText = this.HeaderInfo["col5"];
        this.gvwGridView1.Columns[6].HeaderText = this.HeaderInfo["col6"];
        this.gvwGridView1.Columns[7].HeaderText = this.HeaderInfo["col7"];
        this.gvwGridView1.Columns[8].HeaderText = this.HeaderInfo["col8"];
        this.gvwGridView1.Columns[9].HeaderText = this.HeaderInfo["col9"];
        this.gvwGridView1.Columns[10].HeaderText = this.HeaderInfo["col10"];

        this.BindGridData();
    }

    /// <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit_PostBack()
    {
        // フォーム初期化（ポストバック）時に実行する処理を実装する
        // TODO:
        this.CmnInit();

        // Radio Buttonの選択状態を出力
        if (Request.Form["radio-grp1"] != null)
        {
            Response.Write(string.Format(
                    "name=\"radio-grp1\" value=\"{0}\"が選択されました。<br/>",
                    Request.Form["radio-grp1"].ToString()));
        }

        int i = 0;
        foreach (GridViewRow gvr in this.gvwGridView1.Rows)
        {
            i++;
            WebCustomRadioButton rbn = (WebCustomRadioButton)gvr.FindControl("rbnRadioButton");

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

    private void CmnInit()
    {
        // ヘッダーに表示する文字列を初期化
        this.HeaderInfo.Add("col0", "select1");
        this.HeaderInfo.Add("col1", "select2");
        this.HeaderInfo.Add("col2", "custom");
        this.HeaderInfo.Add("col3", "fileid");
        this.HeaderInfo.Add("col4", "readonly");
        this.HeaderInfo.Add("col5", "filename");
        this.HeaderInfo.Add("col6", "filesize");
        this.HeaderInfo.Add("col7", "date");
        this.HeaderInfo.Add("col8", "edit1");
        this.HeaderInfo.Add("col9", "edit2");
        this.HeaderInfo.Add("col10", "dropdown");
    }

    #endregion

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
        this.gvwGridView1.DataSource = Session["SampleData"];
        this.gvwGridView1.DataBind();
    }

    #endregion

    #region イベントハンドラ

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

    #region GridViewのイベント

    #region 標準イベント

    /// <summary>RowCreatedのテスト</summary>
    protected void gvwGridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {
    }

    #endregion

    #region Command

    /// <summary>gvwGridView1のコマンドイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_gvwGridView1_RowCommand(FxEventArgs fxEventArgs)
    {
        // 選択されたコマンド名を取得する
        // fxEventArgs.InnerButtonID
        //   Select : 選択
        //   Edit   : 編集
        //   Update : 更新
        //   Cancel : キャンセル
        //   Delete : 削除
        //   Page   : ページ切り替え
        //   Sort   : ソート
        //   その他カスタム コマンド

        System.Diagnostics.Debug.WriteLine("--------------------");
        System.Diagnostics.Debug.WriteLine("Event:RowCommand");
        System.Diagnostics.Debug.WriteLine("ButtonID:" + fxEventArgs.ButtonID);
        System.Diagnostics.Debug.WriteLine("InnerButtonID:" + fxEventArgs.InnerButtonID);
        System.Diagnostics.Debug.WriteLine("PostBackValue:" + fxEventArgs.PostBackValue);

        return "";
    }

    #endregion

    #region 選択

    /// <summary>GridViewの行の選択ボタンがクリックされ、行が選択される前に発生するイベント</summary>
    protected void gvwGridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
        // ここでは何もしない
    }

    /// <summary>gvwGridView1の行選択後イベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_gvwGridView1_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // ここでは何もしない

        System.Diagnostics.Debug.WriteLine("--------------------");
        System.Diagnostics.Debug.WriteLine("Event:SelectedIndexChanged");
        System.Diagnostics.Debug.WriteLine("ButtonID:" + fxEventArgs.ButtonID);
        System.Diagnostics.Debug.WriteLine("InnerButtonID:" + fxEventArgs.InnerButtonID);
        System.Diagnostics.Debug.WriteLine("PostBackValue:" + fxEventArgs.PostBackValue);

        return "";
    }

    #endregion

    #region 編集

    // Updating、Deletingのみ棟梁でハンドル。

    /// <summary>GridViewの行の編集ボタンがクリックされ、編集モードになる前に発生するイベント</summary>
    protected void gvwGridView1_RowEditing(object sender, GridViewEditEventArgs e)
    {
        // GridViewを編集モードにする
        this.gvwGridView1.EditIndex = e.NewEditIndex;
        this.BindGridData();
    }

    /// <summary>編集モードの行のキャンセルボタンがクリックされ、編集モードが終了する前に発生するイベント</summary>
    protected void gvwGridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        // GridViewを編集モードから解除する
        this.gvwGridView1.EditIndex = -1;
        this.BindGridData();
    }

    /// <summary>gvwGridView1の行更新前イベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <param name="e">オリジナルのイベント引数</param>
    /// <returns>URL</returns>
    protected string UOC_gvwGridView1_RowUpdating(FxEventArgs fxEventArgs, GridViewUpdateEventArgs e)
    {
        // 編集行のコントロールを取得する
        GridViewRow gvRow = this.gvwGridView1.Rows[e.RowIndex];

        TextBox txt1 = (TextBox)gvRow.FindControl("TextBox1");
        TextBox txt2 = (TextBox)gvRow.FindControl("TextBox2");
        CheckBox cbx3 = (CheckBox)gvRow.FindControl("cbxCheckBox3");
        TextBox txt4 = (TextBox)gvRow.FindControl("TextBox4");
        TextBox txt5 = (TextBox)gvRow.FindControl("TextBox5");

        // 編集後の値に書き換える
        int fileid = (int)this.gvwGridView1.DataKeys[e.RowIndex].Value;
        DataTable dt = (DataTable)Session["SampleData"];
        DataRow row = dt.Select(string.Format("fileid = '{0}'", fileid))[0];
        row["fileid"] = txt1.Text;
        row["filename"] = txt2.Text;
        row["readonly"] = cbx3.Checked;
        row["filesize"] = txt4.Text;
        row["date"] = txt5.Text;

        // GridViewを編集モードから解除する
        this.gvwGridView1.EditIndex = -1;
        this.BindGridData();

        return "";
    }

    /// <summary>GridViewの行の更新ボタンがクリックされ、行が更新された後に発生するイベント</summary>
    protected void gvwGridView1_RowUpdated(object sender, GridViewUpdatedEventArgs e)
    {
        // ここでは何もしない
    }

    /// <summary>gvwGridView1の行削除前イベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <param name="e">オリジナルのイベント引数</param>
    /// <returns>URL</returns>
    protected string UOC_gvwGridView1_RowDeleting(FxEventArgs fxEventArgs, GridViewDeleteEventArgs e)
    {
        // 選択された行を削除する
        DataTable dt = (DataTable)Session["SampleData"];
        int fileid = (int)this.gvwGridView1.DataKeys[e.RowIndex].Value;
        dt.Select(string.Format("fileid = '{0}'", fileid))[0].Delete();

        // GridViewを編集モードから解除する
        this.gvwGridView1.EditIndex = -1;
        this.BindGridData();

        return "";
    }

    /// <summary>GridViewの行の削除ボタンがクリックされ、行が削除された後に発生するイベント</summary>
    protected void gvwGridView1_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
        // ここでは何もしない
    }

    #endregion

    #region ページング・ソート

    // PageIndexChanging、Sortingのみ棟梁でハンドル。

    /// <summary>gvwGridView1のPageIndexChangingイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <param name="e">オリジナルのイベント引数</param>
    /// <returns>URL</returns>
    protected string UOC_gvwGridView1_PageIndexChanging(FxEventArgs fxEventArgs, GridViewPageEventArgs e)
    {
        this.gvwGridView1.PageIndex = e.NewPageIndex;
        this.BindGridData();

        return "";
    }

    /// <summary>ページが切り替わったときに発生するイベント</summary>
    protected void gvwGridView1_PageIndexChanged(object sender, EventArgs e)
    {
        // ここでは何もしない
    }

    /// <summary>gvwGridView1のSortingイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <param name="e">オリジナルのイベント引数</param>
    /// <returns>URL</returns>
    protected string UOC_gvwGridView1_Sorting(FxEventArgs fxEventArgs, GridViewSortEventArgs e)
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
        this.BindGridData();

        return "";
    }

    /// <summary>GridViewの列ヘッダーがクリックされ、行がソートされた後に発生するイベント</summary>
    protected void gvwGridView1_Sorted(object sender, EventArgs e)
    {
        // ここでは何もしない
    }

    #endregion

    #region GridView内のCommand、Click以外のイベント

    // GridViewのイベントに行かないので通常通りハンドルする。
    // （各コントロールのAutoPostBackを"true"に設定する）

    /// <summary>cbxCheckBox3のCheckedChangedイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_cbxCheckBox3_CheckedChanged(FxEventArgs fxEventArgs)
    {
        System.Diagnostics.Debug.WriteLine("--------------------");
        System.Diagnostics.Debug.WriteLine("ButtonID:" + fxEventArgs.ButtonID);
        System.Diagnostics.Debug.WriteLine("InnerButtonID:" + fxEventArgs.InnerButtonID);
        System.Diagnostics.Debug.WriteLine("PostBackValue:" + fxEventArgs.PostBackValue);

        CheckBox cbx = (CheckBox)this.gvwGridView1.Rows
            [int.Parse(fxEventArgs.PostBackValue)].FindControl("cbxCheckBox3");

        System.Diagnostics.Debug.WriteLine(cbx.Checked.ToString());

        return "";
    }

    /// <summary>rbnRadioButton3のCheckedChangedイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_rbnRadioButton3_CheckedChanged(FxEventArgs fxEventArgs)
    {
        System.Diagnostics.Debug.WriteLine("--------------------");
        System.Diagnostics.Debug.WriteLine("ButtonID:" + fxEventArgs.ButtonID);
        System.Diagnostics.Debug.WriteLine("InnerButtonID:" + fxEventArgs.InnerButtonID);
        System.Diagnostics.Debug.WriteLine("PostBackValue:" + fxEventArgs.PostBackValue);

        RadioButton cbx = (RadioButton)this.gvwGridView1.Rows
            [int.Parse(fxEventArgs.PostBackValue)].FindControl("rbnRadioButton3");

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

        DropDownList ddl = (DropDownList)this.gvwGridView1.Rows
            [int.Parse(fxEventArgs.PostBackValue)].FindControl("ddlDropDownList1");

        System.Diagnostics.Debug.WriteLine(ddl.SelectedValue);

        return "";
    }

    /// <summary>lbxListBox1のSelectedIndexChangedイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_lbxListBox1_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        System.Diagnostics.Debug.WriteLine("--------------------");
        System.Diagnostics.Debug.WriteLine("ButtonID:" + fxEventArgs.ButtonID);
        System.Diagnostics.Debug.WriteLine("InnerButtonID:" + fxEventArgs.InnerButtonID);
        System.Diagnostics.Debug.WriteLine("PostBackValue:" + fxEventArgs.PostBackValue);

        ListBox ddl = (ListBox)this.gvwGridView1.Rows
            [int.Parse(fxEventArgs.PostBackValue)].FindControl("lbxListBox1");

        System.Diagnostics.Debug.WriteLine(ddl.SelectedValue);

        return "";
    }

    #endregion

    #endregion

    #endregion
}

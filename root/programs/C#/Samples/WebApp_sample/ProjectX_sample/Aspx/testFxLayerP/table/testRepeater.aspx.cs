//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testFxLayerP_table_testRepeater
//* クラス日本語名  ：Repeaterテスト画面（Ｐ層）
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

using System.Text.RegularExpressions;

/// <summary>Repeaterテスト画面（Ｐ層）</summary>
public partial class Aspx_testFxLayerP_table_testRepeater : MyBaseController
{
    #region 初期化

    /// <summary>ヘッダーに表示する文字列</summary>
    public Dictionary<string, string> HeaderInfo = new Dictionary<string, string>();

    /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit()
    {
        // フォーム初期化（初回ロード）時に実行する処理を実装する
        // TODO:
        this.CmnInit();

        DataTable dt = null;

        // DropDownListのデータソースを初期化
        dt = this.CreateDataSource2();
        this.DropDownListDataSource = dt;

        // データバインド
        dt = this.CreateDataSource1();
        this.RepeaterDataSource = dt;
        this.rptRepeater1.DataSource = dt;
        this.rptRepeater1.DataBind();
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
        foreach (RepeaterItem ri in this.rptRepeater1.Items)
        {
            i++;
            WebCustomRadioButton rbn = (WebCustomRadioButton)ri.FindControl("rbnRadioButton");

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
        this.HeaderInfo.Add("col0", "select");
        this.HeaderInfo.Add("col1", "fileid");
        this.HeaderInfo.Add("col2", "textbox<br/>filename");
        this.HeaderInfo.Add("col3", "checkbox<br/>（IsReadOnly）");
        this.HeaderInfo.Add("col4", "dropdownlist");
    }

    #endregion

    #region データソースの生成
    
    /// <summary>DataSourceを生成</summary>
    /// <returns>Datatableを返す</returns>
    /// <remarks>repeater1用</remarks>
    private DataTable CreateDataSource1()
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
        dt.Columns.Add(new DataColumn("textbox", typeof(String)));
        dt.Columns.Add(new DataColumn("checkbox", typeof(bool)));
        dt.Columns.Add(new DataColumn("dropdownlist", typeof(int)));

        // 行生成
        for (int i = 0; i < fi.Length; i++)
        {
            dr = dt.NewRow();
            dr["fileid"] = i;
            dr["textbox"] = fi[i].Name;
            dr["checkbox"] = fi[i].IsReadOnly;
            dr["dropdownlist"] = this.GetRandomValue(5);
            dt.Rows.Add(dr);
        }

        // 変更のコミット
        dt.AcceptChanges();

        // Datatableを返す。
        return dt;
    }

    /// <summary>DataSourceを生成</summary>
    /// <returns>Datatableを返す</returns>
    /// <remarks>DropDownList1用</remarks>
    private DataTable CreateDataSource2()
    {
        DataTable dt = new DataTable();
        DataRow dr;

        // 列生成
        dt.Columns.Add(new DataColumn("value", typeof(int)));
        dt.Columns.Add(new DataColumn("text", typeof(String)));

        // 行生成
        for (int i = 0; i < 5; i++)
        {
            dr = dt.NewRow();
            dr["value"] = i;
            dr["text"] = "選択肢" + i.ToString();
            dt.Rows.Add(dr);
        }

        // 変更のコミット
        dt.AcceptChanges();

        // Datatableを返す。
        return dt;
    }

    /// <summary>Randomオブジェクト</summary>
    Random rnd = new Random(Environment.TickCount);

    /// <summary>０～最大値の値をランダムに生成</summary>
    /// <param name="maxVal">最大値</param>
    /// <returns>０～最大値の値</returns>
    private int GetRandomValue(int maxVal)
    {
        return rnd.Next(0, maxVal);
    }

    #endregion

    #region データソースの保持

    /// <summary>Repeaterのデータソース</summary>
    public DataTable RepeaterDataSource
    {
        set
        {
            Session["Repeater1.DataSource"] = value;
        }
        get
        {
            return (DataTable)Session["Repeater1.DataSource"];
        }

    }

    /// <summary>DropDownListのデータソース</summary>
    public DataTable DropDownListDataSource
    {
        set
        {
            Session["DropDownList1.DataSource"] = value;
        }
        get
        {
            return (DataTable)Session["DropDownList1.DataSource"];
        }
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

    /// <summary>btnButton2のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton2_Click(FxEventArgs fxEventArgs)
    {
        // Repeater1に対する変更をDataTableに反映する。
        DataTable dt = this.RepeaterDataSource;

        // 変更の検知
        bool isUpd = false;

        for (int i = 0; i < this.rptRepeater1.Items.Count - 1; i++)
        {
            // Repeater1の行毎に処理
            DataRow dr = dt.Rows[i];

            // 変更されていればDataTableに反映（RowStateが変更される）
            TextBox txt = ((TextBox)this.rptRepeater1.Items[i].FindControl("TextBox1"));
            if (dr["textbox"].ToString() != txt.Text)
            {
                dr["textbox"] = txt.Text;
                isUpd = true;
            }

            // 変更されていればDataTableに反映（RowStateが変更される）
            CheckBox cbx = ((CheckBox)this.rptRepeater1.Items[i].FindControl("cbxCheckBox1"));
            //RadioButton cbx = ((RadioButton)this.rptRepeater1.Items[i].FindControl("rbnRadioButton1"));
            if ((bool)dr["checkbox"] != cbx.Checked)
            {
                dr["checkbox"] = cbx.Checked;
                isUpd = true;
            }

            // 変更されていればDataTableに反映（RowStateが変更される）
            DropDownList ddl = ((DropDownList)this.rptRepeater1.Items[i].FindControl("ddlDropDownList1"));
            //ListBox ddl = ((ListBox)this.rptRepeater1.Items[i].FindControl("lbxListBox1"));
            if (dr["dropdownlist"].ToString() != ddl.SelectedValue)
            {
                dr["dropdownlist"] = ddl.SelectedValue;
                isUpd = true;
            }
        }

        // 変更時のみ実行
        if (isUpd)
        {
            // 再データバインド
            this.RepeaterDataSource = dt;
            this.rptRepeater1.DataSource = dt;
            this.rptRepeater1.DataBind();
        }

        return "";
    }

    #endregion

    #region Repeater内のClickイベント（Command）

    /// <summary>rptRepeater1のコマンドイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_rptRepeater1_ItemCommand(FxEventArgs fxEventArgs)
    {
        System.Diagnostics.Debug.WriteLine("--------------------");
        System.Diagnostics.Debug.WriteLine("ButtonID:" + fxEventArgs.ButtonID);
        System.Diagnostics.Debug.WriteLine("InnerButtonID:" + fxEventArgs.InnerButtonID);
        System.Diagnostics.Debug.WriteLine("PostBackValue:" + fxEventArgs.PostBackValue);

        return "";
    }

    #endregion

    #region Repeater内のClick以外のイベント

    // ItemCommandイベントに行かないので通常通りハンドルする。
    // （各コントロールのAutoPostBackを"true"に設定する）

    /// <summary>cbxCheckBox1のCheckedChangedイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_cbxCheckBox1_CheckedChanged(FxEventArgs fxEventArgs)
    {
        System.Diagnostics.Debug.WriteLine("--------------------");
        System.Diagnostics.Debug.WriteLine("ButtonID:" + fxEventArgs.ButtonID);
        System.Diagnostics.Debug.WriteLine("InnerButtonID:" + fxEventArgs.InnerButtonID);
        System.Diagnostics.Debug.WriteLine("PostBackValue:" + fxEventArgs.PostBackValue);

        CheckBox cbx = (CheckBox)this.rptRepeater1.Items
            [int.Parse(fxEventArgs.PostBackValue)].FindControl("cbxCheckBox1");

        System.Diagnostics.Debug.WriteLine(cbx.Checked.ToString());
        
        return "";
    }

    /// <summary>rbnRadioButton1のCheckedChangedイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_rbnRadioButton1_CheckedChanged(FxEventArgs fxEventArgs)
    {
        System.Diagnostics.Debug.WriteLine("--------------------");
        System.Diagnostics.Debug.WriteLine("ButtonID:" + fxEventArgs.ButtonID);
        System.Diagnostics.Debug.WriteLine("InnerButtonID:" + fxEventArgs.InnerButtonID);
        System.Diagnostics.Debug.WriteLine("PostBackValue:" + fxEventArgs.PostBackValue);

        RadioButton cbx = (RadioButton)this.rptRepeater1.Items
            [int.Parse(fxEventArgs.PostBackValue)].FindControl("rbnRadioButton1");

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

        DropDownList ddl = (DropDownList)this.rptRepeater1.Items
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

        ListBox ddl = (ListBox)this.rptRepeater1.Items
            [int.Parse(fxEventArgs.PostBackValue)].FindControl("lbxListBox1");

        System.Diagnostics.Debug.WriteLine(ddl.SelectedValue);

        return "";
    }

    #endregion

    #endregion
}

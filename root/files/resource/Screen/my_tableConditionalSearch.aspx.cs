//**********************************************************************************
//* 三層データバインド・アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：my_tableConditionalSearch
//* クラス日本語名  ：三層データバインド・検索一覧表示画面（my_table）
//*
//* 作成日時        ：2014/3/30
//* 作成者          ：自動生成ツール（墨壺２）, 日立 太郎
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using MyType;

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

/// <summary>三層データバインド・サンプル アプリ画面（検索一覧表示）</summary>
public partial class my_tableConditionalSearch : MyBaseController
{
    /// <summary>Page_InitイベントでASP.NET標準イベントハンドラを設定</summary>
    protected void Page_Init(object sender, EventArgs e)
    {
        // 行選択についてのイベント
        this.gvwGridView1.SelectedIndexChanging += new GridViewSelectEventHandler(gvwGridView1_SelectedIndexChanging);
    }

    #region ページロードのUOCメソッド

    /// <summary>
    /// ページロードのUOCメソッド（個別：初回ロード）
    /// </summary>
    /// <remarks>
    /// 実装必須
    /// </remarks>
    protected override void UOC_FormInit()
    {
        // フォーム初期化（初回ロード）時に実行する処理を実装する

        // TODO:
    }

    /// <summary>
    /// ページロードのUOCメソッド（個別：ポストバック）
    /// </summary>
    /// <remarks>
    /// 実装必須
    /// </remarks>
    protected override void UOC_FormInit_PostBack()
    {
        // フォーム初期化（ポストバック）時に実行する処理を実装する

        // TODO:
        Session["DAP"] = this.ddlDap.SelectedValue;

        if (this.ddlDap.SelectedValue == "SQL")
        {
            Session["DBMS"] = DbEnum.DBMSType.SQLServer;
        }
        else
        {
            Session["DBMS"] = DbEnum.DBMSType.Oracle;
        }
    }

    #endregion

    #region イベントハンドラ

    /// <summary>追加ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnInsert_Click(FxEventArgs fxEventArgs)
    {
        // 画面遷移（詳細表示）
        return "my_tableDetail.aspx";
    }

    /// <summary>検索ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnSearch_Click(FxEventArgs fxEventArgs)
    {
        // GridViewをリセット
        this.gvwGridView1.PageIndex = 0;
        this.gvwGridView1.Sort("", SortDirection.Ascending);

        // 検索条件の収集
        // AndEqualSearchConditions
        Dictionary<string, object> andEqualSearchConditions = new Dictionary<string, object>();
        andEqualSearchConditions.Add("columna", this.txtcolumna_And.Text);
        andEqualSearchConditions.Add("columnb", this.txtcolumnb_And.Text);
        andEqualSearchConditions.Add("columnc", this.txtcolumnc_And.Text);
        andEqualSearchConditions.Add("columnd", this.txtcolumnd_And.Text);
        Session["AndEqualSearchConditions"] = andEqualSearchConditions;

        // AndLikeSearchConditions
        Dictionary<string, string> andLikeSearchConditions = new Dictionary<string, string>();
        andLikeSearchConditions.Add("columna", this.txtcolumna_And_Like.Text);
        andLikeSearchConditions.Add("columnb", this.txtcolumnb_And_Like.Text);
        andLikeSearchConditions.Add("columnc", this.txtcolumnc_And_Like.Text);
        andLikeSearchConditions.Add("columnd", this.txtcolumnd_And_Like.Text);
        Session["AndLikeSearchConditions"] = andLikeSearchConditions;

        // OrEqualSearchConditions
        Dictionary<string, object[]> orEqualSearchConditions = new Dictionary<string, object[]>();
        orEqualSearchConditions.Add("columna", this.txtcolumna_OR.Text.Split(' '));
        orEqualSearchConditions.Add("columnb", this.txtcolumnb_OR.Text.Split(' '));
        orEqualSearchConditions.Add("columnc", this.txtcolumnc_OR.Text.Split(' '));
        orEqualSearchConditions.Add("columnd", this.txtcolumnd_OR.Text.Split(' '));
        Session["OrEqualSearchConditions"] = orEqualSearchConditions;

        // OrLikeSearchConditions
        Dictionary<string, string[]> orLikeSearchConditions = new Dictionary<string, string[]>();
        orLikeSearchConditions.Add("columna", this.txtcolumna_OR_Like.Text.Split(' '));
        orLikeSearchConditions.Add("columnb", this.txtcolumnb_OR_Like.Text.Split(' '));
        orLikeSearchConditions.Add("columnc", this.txtcolumnc_OR_Like.Text.Split(' '));
        orLikeSearchConditions.Add("columnd", this.txtcolumnd_OR_Like.Text.Split(' '));
        Session["OrLikeSearchConditions"] = orLikeSearchConditions;

        //// ElseSearchConditions
        //Dictionary<string, object> ElseSearchConditions = new Dictionary<string, object>();
        //ElseSearchConditions.Add("myp1", 1);
        //ElseSearchConditions.Add("myp2", 40);
        //Session["ElseSearchConditions"] = ElseSearchConditions;
        //Session["ElseWhereSQL"] = "AND [ProductID] BETWEEN @myp1 AND @myp2";

        // ソート条件の初期化
        Session["SortExpression"] = "columna"; // 主キーを指定
        Session["SortDirection"] = "ASC";        // ASCを指定

        // gvwGridView1をObjectDataSourceに連結。
        this.gvwGridView1.DataSourceID = "ObjectDataSource1";

        // ヘッダーを設定する。
        this.gvwGridView1.Columns[0].HeaderText = "選択";
        this.gvwGridView1.Columns[1].HeaderText = "columna";
        this.gvwGridView1.Columns[2].HeaderText = "columnb";
        this.gvwGridView1.Columns[3].HeaderText = "columnc";
        this.gvwGridView1.Columns[4].HeaderText = "columnd";

        // 画面遷移しない。
        return string.Empty;
    }

    /// <summary>gvwGridView1のSortingイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <param name="e">オリジナルのイベント引数</param>
    /// <returns>URL</returns>
    protected string UOC_gvwGridView1_Sorting(FxEventArgs fxEventArgs, GridViewSortEventArgs e)
    {
        // ソート条件の変更
        Session["SortExpression"] = e.SortExpression;

        if ((string)Session["SortDirection"] == "ASC")
        {
            e.SortDirection = SortDirection.Descending;
            Session["SortDirection"] = "DESC";
        }
        else
        {
            e.SortDirection = SortDirection.Ascending;
            Session["SortDirection"] = "ASC";
        }

        // 画面遷移しない。
        return string.Empty;
    }

    /// <summary>GridViewの行の選択ボタンがクリックされ、行が選択される前に発生するイベント</summary>
    protected void gvwGridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
       
        // 選択されたレコードの主キーとタイムスタンプ列を取得
        DataTable dt = (DataTable)Session["SearchResult"];
        Dictionary<string, object> PrimaryKeyAndTimeStamp = new Dictionary<string, object>();

        // 主キーとタイムスタンプ列
        // 主キー列
        PrimaryKeyAndTimeStamp.Add("columna", dt.Rows[e.NewSelectedIndex]["columna"].ToString());
        PrimaryKeyAndTimeStamp.Add("columnb", dt.Rows[e.NewSelectedIndex]["columnb"].ToString());
        
        // タイムスタンプ列
        //<"03-03-2014","Ritu"," Adding timestamp column to dic when its not null. And removed the typecast as it is giving conversion error" <START>
        if(dt.Rows[e.NewSelectedIndex]["columnd"].GetType()!=typeof(System.DBNull))
        {
         PrimaryKeyAndTimeStamp.Add("columnd", dt.Rows[e.NewSelectedIndex]["columnd"]);
        }
	    //<"03-03-2014","Ritu", <END>
        Session["PrimaryKeyAndTimeStamp"] = PrimaryKeyAndTimeStamp;
       
    }

    /// <summary>gvwGridView1の行選択後イベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_gvwGridView1_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // 画面遷移（詳細表示）
        return "my_tableDetail.aspx";
    }

    #endregion
}

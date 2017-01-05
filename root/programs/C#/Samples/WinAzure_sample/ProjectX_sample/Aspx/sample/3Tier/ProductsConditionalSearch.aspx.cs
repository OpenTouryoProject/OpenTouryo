//**********************************************************************************
//* 三層データバインド・アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：_ConditionalSearch_
//* クラス日本語名  ：三層データバインド・検索一覧表示画面（_TableName_）
//*
//* 作成日時        ：_TimeStamp_
//* 作成者          ：自動生成ツール（墨壺２）, _UserName_
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
public partial class Aspx_sample_3Tier_ProductsConditionalSearch : MyBaseController
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

        #region マスタ・データのロードと設定

        // 引数クラスを生成
        // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        TestParameterValue testParameterValue
            = new TestParameterValue(
                this.ContentPageFileNoEx,
                "FormInit_PostBack", "Invoke",
                this.ddlDap.SelectedValue,
                this.UserInfo);

        // B層を生成
        GetMasterData getMasterData = new GetMasterData();

        // 業務処理を実行
        TestReturnValue testReturnValue =
            (TestReturnValue)getMasterData.DoBusinessLogic(
                (BaseParameterValue)testParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

        DataTable[] dts = (DataTable[])testReturnValue.Obj;
        DataTable dt = null;

        // daoSuppliers
        _3TierEngine.CreateDropDownListDataSourceDataTable(
            dts[0], "SupplierID", "CompanyName", out dt, "value", "text");
        this.ddldsdt_Suppliers = dt;

        // daoCategories
        _3TierEngine.CreateDropDownListDataSourceDataTable(
            dts[1], "CategoryID", "CategoryName", out dt, "value", "text");
        this.ddldsdt_Categories = dt;

        #endregion
    }

    #region マスタ・データの設定用プロパティ

    /// <summary>DropDownList生成用のプロパティ</summary>
    public DataTable ddldsdt_Suppliers
    {
        set
        {
            Session["ddldsdt_SupplierID"] = value;
        }
        get
        {
            return (DataTable)Session["ddldsdt_SupplierID"];
        }
    }

    /// <summary>DropDownList生成用のプロパティ</summary>
    public DataTable ddldsdt_Categories
    {
        set
        {
            Session["ddldsdt_CategoryID"] = value;
        }
        get
        {
            return (DataTable)Session["ddldsdt_CategoryID"];
        }
    }

    #endregion

    #endregion

    #region イベントハンドラ

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
        andEqualSearchConditions.Add("ProductID", this.txtProductID_And.Text);
        andEqualSearchConditions.Add("ProductName", this.txtProductName_And.Text);
        andEqualSearchConditions.Add("SupplierID", this.txtSupplierID_And.Text);
        andEqualSearchConditions.Add("CategoryID", this.txtCategoryID_And.Text);
        andEqualSearchConditions.Add("QuantityPerUnit", this.txtQuantityPerUnit_And.Text);
        andEqualSearchConditions.Add("UnitPrice", this.txtUnitPrice_And.Text);
        andEqualSearchConditions.Add("UnitsInStock", this.txtUnitsInStock_And.Text);
        andEqualSearchConditions.Add("UnitsOnOrder", this.txtUnitsOnOrder_And.Text);
        andEqualSearchConditions.Add("ReorderLevel", this.txtReorderLevel_And.Text);
        andEqualSearchConditions.Add("Discontinued", this.txtDiscontinued_And.Text);
        Session["AndEqualSearchConditions"] = andEqualSearchConditions;

        // AndLikeSearchConditions
        Dictionary<string, string> andLikeSearchConditions = new Dictionary<string, string>();
        andLikeSearchConditions.Add("ProductID", this.txtProductID_And_Like.Text);
        andLikeSearchConditions.Add("ProductName", this.txtProductName_And_Like.Text);
        andLikeSearchConditions.Add("SupplierID", this.txtSupplierID_And_Like.Text);
        andLikeSearchConditions.Add("CategoryID", this.txtCategoryID_And_Like.Text);
        andLikeSearchConditions.Add("QuantityPerUnit", this.txtQuantityPerUnit_And_Like.Text);
        andLikeSearchConditions.Add("UnitPrice", this.txtUnitPrice_And_Like.Text);
        andLikeSearchConditions.Add("UnitsInStock", this.txtUnitsInStock_And_Like.Text);
        andLikeSearchConditions.Add("UnitsOnOrder", this.txtUnitsOnOrder_And_Like.Text);
        andLikeSearchConditions.Add("ReorderLevel", this.txtReorderLevel_And_Like.Text);
        andLikeSearchConditions.Add("Discontinued", this.txtDiscontinued_And_Like.Text);
        Session["AndLikeSearchConditions"] = andLikeSearchConditions;

        // OrEqualSearchConditions
        Dictionary<string, object[]> orEqualSearchConditions = new Dictionary<string, object[]>();
        orEqualSearchConditions.Add("ProductID", this.txtProductID_OR.Text.Split(' '));
        orEqualSearchConditions.Add("ProductName", this.txtProductName_OR.Text.Split(' '));
        orEqualSearchConditions.Add("SupplierID", this.txtSupplierID_OR.Text.Split(' '));
        orEqualSearchConditions.Add("CategoryID", this.txtCategoryID_OR.Text.Split(' '));
        orEqualSearchConditions.Add("QuantityPerUnit", this.txtQuantityPerUnit_OR.Text.Split(' '));
        orEqualSearchConditions.Add("UnitPrice", this.txtUnitPrice_OR.Text.Split(' '));
        orEqualSearchConditions.Add("UnitsInStock", this.txtUnitsInStock_OR.Text.Split(' '));
        orEqualSearchConditions.Add("UnitsOnOrder", this.txtUnitsOnOrder_OR.Text.Split(' '));
        orEqualSearchConditions.Add("ReorderLevel", this.txtReorderLevel_OR.Text.Split(' '));
        orEqualSearchConditions.Add("Discontinued", this.txtDiscontinued_OR.Text.Split(' '));
        Session["OrEqualSearchConditions"] = orEqualSearchConditions;

        // OrLikeSearchConditions
        Dictionary<string, string[]> orLikeSearchConditions = new Dictionary<string, string[]>();
        orLikeSearchConditions.Add("ProductID", this.txtProductID_OR_Like.Text.Split(' '));
        orLikeSearchConditions.Add("ProductName", this.txtProductName_OR_Like.Text.Split(' '));
        orLikeSearchConditions.Add("SupplierID", this.txtSupplierID_OR_Like.Text.Split(' '));
        orLikeSearchConditions.Add("CategoryID", this.txtCategoryID_OR_Like.Text.Split(' '));
        orLikeSearchConditions.Add("QuantityPerUnit", this.txtQuantityPerUnit_OR_Like.Text.Split(' '));
        orLikeSearchConditions.Add("UnitPrice", this.txtUnitPrice_OR_Like.Text.Split(' '));
        orLikeSearchConditions.Add("UnitsInStock", this.txtUnitsInStock_OR_Like.Text.Split(' '));
        orLikeSearchConditions.Add("UnitsOnOrder", this.txtUnitsOnOrder_OR_Like.Text.Split(' '));
        orLikeSearchConditions.Add("ReorderLevel", this.txtReorderLevel_OR_Like.Text.Split(' '));
        orLikeSearchConditions.Add("Discontinued", this.txtDiscontinued_OR_Like.Text.Split(' '));
        Session["OrLikeSearchConditions"] = orLikeSearchConditions;

        //// ElseSearchConditions
        //Dictionary<string, object> ElseSearchConditions = new Dictionary<string, object>();
        //ElseSearchConditions.Add("myp1", 1);
        //ElseSearchConditions.Add("myp2", 40);
        //Session["ElseSearchConditions"] = ElseSearchConditions;
        //Session["ElseWhereSQL"] = "AND [ProductID] BETWEEN @myp1 AND @myp2";

        // ソート条件の初期化
        Session["SortExpression"] = "ProductID"; // 主キーを指定
        Session["SortDirection"] = "ASC";        // ASCを指定

        // gvwGridView1をObjectDataSourceに連結。
        this.gvwGridView1.DataSourceID = "ObjectDataSource1";

        // ヘッダーを設定する。
        this.gvwGridView1.Columns[0].HeaderText = "選択"; 
        this.gvwGridView1.Columns[1].HeaderText = "ProductID";
        this.gvwGridView1.Columns[2].HeaderText = "ProductName";
        this.gvwGridView1.Columns[3].HeaderText = "SupplierID";
        this.gvwGridView1.Columns[4].HeaderText = "CategoryID";
        this.gvwGridView1.Columns[5].HeaderText = "QuantityPerUnit";
        this.gvwGridView1.Columns[6].HeaderText = "UnitPrice";
        this.gvwGridView1.Columns[7].HeaderText = "UnitsInStock";
        this.gvwGridView1.Columns[8].HeaderText = "UnitsOnOrder";
        this.gvwGridView1.Columns[9].HeaderText = "ReorderLevel";
        this.gvwGridView1.Columns[10].HeaderText = "Discontinued";

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
        // 選択されたレコードの主キーを取得
        DataTable dt = (DataTable)Session["SearchResult"];
        Dictionary<string, object> PrimaryKeyAndTimeStamp = new Dictionary<string, object>();

        // 主キーとタイムスタンプ列
        // 主キー列
        PrimaryKeyAndTimeStamp.Add("ProductID", dt.Rows[e.NewSelectedIndex]["ProductID"].ToString());
        // タイムスタンプ列

        Session["PrimaryKeyAndTimeStamp"] = PrimaryKeyAndTimeStamp;
    }

    /// <summary>gvwGridView1の行選択後イベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_gvwGridView1_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // 画面遷移（詳細表示）
        return "ProductsDetail.aspx";
    }

    #endregion
}

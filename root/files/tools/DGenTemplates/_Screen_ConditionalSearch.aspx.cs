//**********************************************************************************
//* 三層データバインド・アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：_JoinTableName__Screen_ConditionalSearch
//* クラス日本語名  ：三層データバインド・検索一覧表示画面（_JoinTableName_）
//*
//* 作成日時        ：_TimeStamp_
//* 作成者          ：自動生成ツール（墨壺２）, _UserName_
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2016/05/12  Shashikiran       Added UOC_gvwGridView1_PageIndexChanging function to handle Gridview Paging event
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
public partial class _JoinTableName__Screen_ConditionalSearch : MyBaseController
{
    #region ASP.NET EVENT HANDLER
    /// <summary>Page_InitイベントでASP.NET標準イベントハンドラを設定</summary>
    protected void Page_Init(object sender, EventArgs e)
    {
        // 行選択についてのイベント
        this.gvwGridView1.SelectedIndexChanging += new GridViewSelectEventHandler(gvwGridView1_SelectedIndexChanging);
    }
    #endregion

    #region ページロードのUOCメソッド UOC Method of Page Load

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
        Session["DAP"] = "_DAP_";
        Session["DBMS"] = DbEnum.DBMSType._DBMS_;
    }

    #endregion

    #region イベントハンドラ EVENT HANDLER

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
        // ControlComment:LoopStart-PKColumn
        andEqualSearchConditions.Add("_JoinTextboxColumnName_", this.txt_JoinTextboxColumnName__And.Text);
        // ControlComment:LoopEnd-PKColumn
        // ControlComment:LoopStart-ElseColumn
        andEqualSearchConditions.Add("_JoinTextboxColumnName_", this.txt_JoinTextboxColumnName__And.Text);
        // ControlComment:LoopEnd-ElseColumn
        Session["AndEqualSearchConditions"] = andEqualSearchConditions;

        // 引数クラスを生成
        _3TierParameterValue parameterValue = new _3TierParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectRecord",
                (string)Session["DAP"], (MyUserInfo)this.UserInfo);

        // テーブル
        parameterValue.TableName = "_JoinTableName_";

        // 主キーとタイムスタンプ列
        parameterValue.AndEqualSearchConditions = (Dictionary<string, object>)Session["AndEqualSearchConditions"];

        // B層を生成
        _3TierEngine b = new _3TierEngine();

        // データ取得処理を実行
        _3TierReturnValue returnValue =
            (_3TierReturnValue)b.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);
        //Declare Table to bind data to gridview
        DataTable dt = new DataTable();
        dt = returnValue.Dt;
        HttpContext.Current.Session["SearchResult"] = dt;

        // ヘッダーを設定する。
        this.gvwGridView1.Columns[_ColumnNmbr_].HeaderText = "Select";
        // ControlComment:LoopStart-PKColumn
        this.gvwGridView1.Columns[_ColumnNmbr_].HeaderText = "_JoinTextboxColumnName_";
        // ControlComment:LoopEnd-PKColumn
        // ControlComment:LoopStart-ElseColumn
        this.gvwGridView1.Columns[_ColumnNmbr_].HeaderText = "_JoinTextboxColumnName_";
        // ControlComment:LoopEnd-ElseColumn

        //Bind gridview
        this.gvwGridView1.DataSource = dt;
        this.gvwGridView1.DataBind();

        //Return empty string since there is no need to redirect to any other page.
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

    /// <summary>GridView Even That occurs before selection of row buttons</summary>
    protected void gvwGridView1_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
    {
       
        // Get Primary key and timestamp Column for the selected record
        DataTable dt = (DataTable)Session["SearchResult"];
        Dictionary<string, object> PrimaryKeyAndTimeStamp = new Dictionary<string, object>();

        //  Primary key columns
        // ControlComment:LoopStart-PKColumn
        PrimaryKeyAndTimeStamp.Add("_JoinTextboxColumnName_", dt.Rows[e.NewSelectedIndex + (gvwGridView1.PageSize * gvwGridView1.PageIndex)]["_JoinColumnName_"].ToString());
        // ControlComment:LoopEnd-PKColumn

        // Timestamp Column
          // タイムスタンプ列  
          // ControlComment:LoopStart-JoinTables      
        TS_CommentOut_ if(dt.Rows[e.NewSelectedIndex]["_TableName_._TimeStampColName_"].GetType()!=typeof(System.DBNull))
        TS_CommentOut_ {
        TS_CommentOut_ PrimaryKeyAndTimeStamp.Add("_TableName___TimeStampColName_", dt.Rows[e.NewSelectedIndex]["_TableName_._TimeStampColName_"]);
        TS_CommentOut_ }
          // ControlComment:LoopEnd-JoinTables
        Session["PrimaryKeyAndTimeStamp"] = PrimaryKeyAndTimeStamp;
       
    }

    /// <summary>gvwGridView1の行選択後イベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_gvwGridView1_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        //Screen Transition is required to show more
        return "_JoinTableName__Screen_Detail.aspx";
    }

    /// <summary>gvwGridView1 Paging Event</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_gvwGridView1_PageIndexChanging(FxEventArgs fxEventArgs, GridViewPageEventArgs e)
    {
        this.gvwGridView1.PageIndex = e.NewPageIndex;
        this.gvwGridView1.DataSource = (DataTable)Session["SearchResult"];
        this.gvwGridView1.DataBind();
        return "";
    }

    #endregion
}

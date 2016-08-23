//**********************************************************************************
//* 三層データバインド・アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：_JoinTableName__Screen_SearchAndUpdate
//* クラス日本語名  ：三層データバインド・検索一覧更新画面（_JoinTableName_）
//*
//* 作成日時        ：_TimeStamp_
//* 作成者          ：自動生成ツール（墨壺２）, _UserName_
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2016/05/09  Shashikiran       Modified UOC_btnBatUpd_Click function to call DoBusinessLogic function in single transaction
//*  2016/05/12  Shashikiran       Added UOC_gvwGridView1_PageIndexChanging function to handle Gridview Paging event
//*  2016/05/16  Shashikiran       Commented this.gvwGridView1.AllowPaging = false; line of code in UOC_gvwGridView1_RowCommand function to enable paging
//*  2016/05/27  Shashikiran       Added remarks above UOC_btnBatUpd_Click event as a guideline for developers to modify the code to set the
//*                                table sequence appropriately for successful delete operation
//*  2016/07/08  Shashikiran       Modified code UOC_gvwGridView1_RowCommand to fix the gridview row replacement issue
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

/// <summary>三層データバインド・サンプル アプリ画面（検索一覧更新）</summary>
public partial class _JoinTableName__Screen_SearchAndUpdate : MyBaseController
{
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

        // 更新ボタンの非活性化
        this.btnBatUpd.Enabled = false;
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

    #region 一覧検索 SEARCH LIST

    /// <summary>検索ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnSearch_Click(FxEventArgs fxEventArgs)
    {
        // 更新ボタンの非活性化
        this.btnBatUpd.Enabled = false;

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

        // Set Header
        this.gvwGridView1.Columns[_ColumnNmbr_].HeaderText = "Delete";
        this.gvwGridView1.Columns[_ColumnNmbr_].HeaderText = "Update";
        // ControlComment:LoopStart-PKColumn
        this.gvwGridView1.Columns[_ColumnNmbr_].HeaderText = "_JoinTextboxColumnName_";
        // ControlComment:LoopEnd-PKColumn
        // ControlComment:LoopStart-ElseColumn
        this.gvwGridView1.Columns[_ColumnNmbr_].HeaderText = "_JoinTextboxColumnName_";
        // ControlComment:LoopEnd-ElseColumn

        //Bind gridview
        this.gvwGridView1.DataSource = dt;
        this.gvwGridView1.DataBind();

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

    #endregion

    #region CRUD USING BATCH UPDATE

    /// <summary>バッチ更新ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>In case of deleting from multiple tables and when the tables have dependent relation, the sequence of execution of delete statements for these tables become necessary. 
    /// Developer should decide the sequence of table for the delete operation.
    /// This can be managed by altering the position of code block present in the region 'Batch Update for [TableName]  table' as required</remarks>
    protected string UOC_btnBatUpd_Click(FxEventArgs fxEventArgs)
    {
        #region  Create the instance of classes here

        // 引数クラスを生成
        _3TierParameterValue parameterValue = new _3TierParameterValue(
            this.ContentPageFileNoEx, fxEventArgs.ButtonID, "BatchUpdateDM",
            (string)Session["DAP"], (MyUserInfo)this.UserInfo);

        //Initialize the data access procedure
        _3TierReturnValue returnValue = null;
        // B layer Initialize
        _3TierEngine b = new _3TierEngine();
        parameterValue.AndEqualSearchConditions = new Dictionary<string, object>();
        parameterValue.TargetTableNames = new Dictionary<int, string>();
         //Declaring the table counter to add it to TargetTableNames Dictionary
        int TableCounter = 0;
        //Keep the copy of the table in session because change in the column name causes the problem in the temperory update after batch update. So keep the copy of the table.
        DataTable dtSession = ((DataTable)Session["SearchResult"]).Copy();
        #endregion

        // ControlComment:LoopStart-JoinTables

        #region  Batch Update for _TableName_  table

      
        #region This is much needed to handle the duplicate column issue while udpating  _TableName_ using batch update
        TableCounter = TableCounter + 1;
        foreach (DataColumn dc in dtSession.Columns)
        {
            //Replace "." in column names of other tables with "_". This is needed becuase if columns are having "." then we get sql error, so we need to replace "." with "_"
            dc.ColumnName = dc.ColumnName.Replace('.', '_');
        }

        #endregion

        //Reset returnvalue with null;
        returnValue = null;

       
        //Primary Key Columns
        // ControlComment:LoopStart-PKColumn
        if(!parameterValue.AndEqualSearchConditions.ContainsKey("_JoinTextboxColumnName_"))
        {
            parameterValue.AndEqualSearchConditions.Add("_JoinTextboxColumnName_", "");
        }
        // ControlComment:LoopEnd-PKColumn
        //Timestamp column
	TS_CommentOut_ parameterValue.AndEqualSearchConditions.Add("_TimeStampColName_", "");

        // Table Name
        parameterValue.TargetTableNames.Add(TableCounter, "_TableName_");
        #endregion
        // ControlComment:LoopEnd-JoinTables

       // DataTableを設定
       parameterValue.Obj = dtSession;

      // Run the Database access process       
     returnValue =
            (_3TierReturnValue)b.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

        // Disable the button
        this.btnBatUpd.Enabled = false;
        //No Screen transition
        return string.Empty;
    }

    /// <summary>gvwGridView1のコマンドイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_gvwGridView1_RowCommand(FxEventArgs fxEventArgs)
    {
        // ソートの場合は無視
        if (fxEventArgs.InnerButtonID == "Sort") { return string.Empty; }

        // DataTableの取得
        DataTable dt = (DataTable)Session["SearchResult"];

        // インデックスを取得
        int index = int.Parse(fxEventArgs.PostBackValue);

        // e.NewSelectedIndexRowsのインデックスが一致しないので。
        // キーで探すのは主キーを意識するため自動生成では面倒になる。
        int i = -1;

        switch (fxEventArgs.InnerButtonID)
        {
            case "Delete":

                // 選択されたレコードを削除
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState == DataRowState.Added)
                    {
                        //　Added行はDeleteできないのでスキップ
                        continue;
                    }
                    else if (dr.RowState != DataRowState.Deleted)
                    {
                        // != Added、Deleted
                        // Pick the exact gridview row value and avoid row replacement
                        // e.NewSelectedIndexとRowsのインデックスをチェック
                        i++;
                        if (index + (gvwGridView1.PageSize * gvwGridView1.PageIndex) == i)
                        {
                            // 削除
                            dr.Delete();
                            break;
                        }
                    }
                    else
                    {
                        // Delete行は表示されないのでスキップ
                        continue;
                    }
                }

                break;

            case "Update":

                // 選択されたレコードを更新
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        // != Deleted
                        // Pick the exact gridview row value and avoid row replacement
                        // e.NewSelectedIndexとRowsのインデックスをチェック
                        i++;
                        if (index + (gvwGridView1.PageSize * gvwGridView1.PageIndex) == i)
                        {
                            // 更新
                            GridViewRow gvRow = this.gvwGridView1.Rows[index];
                            foreach (DataColumn dc in dt.Columns)
                            {
                                TextBox txtBox = ((TextBox)gvRow.FindControl("txt" + dc.ColumnName.Replace('.', '_')));

                                if (txtBox != null)
                                {
                                    if (dr[dc].GetType() == typeof(Byte[]))
                                    {

                                    }
                                    else { dr[dc] = txtBox.Text; }
                                }

                                #region 追加コード（ComboBox化）

                                DropDownList ddl = ((DropDownList)gvRow.FindControl("ddl" + dc.ColumnName.Replace('.', '_')));

                                if (ddl != null)
                                {
                                    dr[dc] = ddl.SelectedValue;
                                }

                                #endregion
                            }

                            break;
                        }
                    }
                    else
                    {
                        // Delete行はスキップ
                        continue;
                    }
                }

                break;

            default:
                // 不明
                return string.Empty;
        }

        // Commenting the code as it is not Necessary to reset and to avoid confusion
        // GridViewをリセット
        //this.gvwGridView1.PageIndex = 0;
        //this.gvwGridView1.Sort("", SortDirection.Ascending);

        // ページングの中止
        //this.gvwGridView1.AllowPaging = false;

        // GridViewのDataSourceを変更してDataBindする。
        this.gvwGridView1.DataSource = dt;
        this.gvwGridView1.DataSourceID = null;
        this.gvwGridView1.DataBind();

        // DataTableの設定
        Session["SearchResult"] = dt;

        // 更新ボタンの活性化
        this.btnBatUpd.Enabled = true;

        // 画面遷移しない。
        return string.Empty;
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

    #endregion
}

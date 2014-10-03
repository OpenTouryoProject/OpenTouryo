//**********************************************************************************
//* 三層データバインド・アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：_TableName_SearchAndUpdate
//* クラス日本語名  ：三層データバインド・検索一覧更新画面（_TableName_）
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

/// <summary>三層データバインド・サンプル アプリ画面（検索一覧更新）</summary>
public partial class _TableName_SearchAndUpdate : MyBaseController
{
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

    #region イベントハンドラ

    #region 一覧検索

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
        andEqualSearchConditions.Add("_ColumnName_", this.txt_ColumnName__And.Text);
        // ControlComment:LoopEnd-PKColumn
        // ControlComment:LoopStart-ElseColumn
        andEqualSearchConditions.Add("_ColumnName_", this.txt_ColumnName__And.Text);
        // ControlComment:LoopEnd-ElseColumn
        Session["AndEqualSearchConditions"] = andEqualSearchConditions;


        // AndLikeSearchConditions
        Dictionary<string, string> andLikeSearchConditions = new Dictionary<string, string>();
        // ControlComment:LoopStart-PKColumn
        andLikeSearchConditions.Add("_ColumnName_", this.txt_ColumnName__And_Like.Text);
        // ControlComment:LoopEnd-PKColumn
        // ControlComment:LoopStart-ElseColumn
        andLikeSearchConditions.Add("_ColumnName_", this.txt_ColumnName__And_Like.Text);
        // ControlComment:LoopEnd-ElseColumn
        Session["AndLikeSearchConditions"] = andLikeSearchConditions;

        // OrEqualSearchConditions
        Dictionary<string, object[]> orEqualSearchConditions = new Dictionary<string, object[]>();
        // ControlComment:LoopStart-PKColumn
        orEqualSearchConditions.Add("_ColumnName_", this.txt_ColumnName__OR.Text.Split(' '));
        // ControlComment:LoopEnd-PKColumn
        // ControlComment:LoopStart-ElseColumn
        orEqualSearchConditions.Add("_ColumnName_", this.txt_ColumnName__OR.Text.Split(' '));
        // ControlComment:LoopEnd-ElseColumn
        Session["OrEqualSearchConditions"] = orEqualSearchConditions;

        // OrLikeSearchConditions
        Dictionary<string, string[]> orLikeSearchConditions = new Dictionary<string, string[]>();
        // ControlComment:LoopStart-PKColumn
        orLikeSearchConditions.Add("_ColumnName_", this.txt_ColumnName__OR_Like.Text.Split(' '));
        // ControlComment:LoopEnd-PKColumn
        // ControlComment:LoopStart-ElseColumn
        orLikeSearchConditions.Add("_ColumnName_", this.txt_ColumnName__OR_Like.Text.Split(' '));
        // ControlComment:LoopEnd-ElseColumn
        Session["OrLikeSearchConditions"] = orLikeSearchConditions;

        //// ElseSearchConditions
        //Dictionary<string, object> ElseSearchConditions = new Dictionary<string, object>();
        //ElseSearchConditions.Add("myp1", 1);
        //ElseSearchConditions.Add("myp2", 40);
        //Session["ElseSearchConditions"] = ElseSearchConditions;
        //Session["ElseWhereSQL"] = "AND [ProductID] BETWEEN @myp1 AND @myp2";

        // ソート条件の初期化
        Session["SortExpression"] = "_PKFirstColumn_"; // 主キーを指定
        Session["SortDirection"] = "ASC";        // ASCを指定

        // ページング
        this.gvwGridView1.AllowPaging = true;

        // gvwGridView1をObjectDataSourceに連結。
        this.gvwGridView1.DataSource = null;
        this.gvwGridView1.DataSourceID = "ObjectDataSource1";

        // ヘッダーを設定する。
        this.gvwGridView1.Columns[_ColumnNmbr_].HeaderText = "削除";
        this.gvwGridView1.Columns[_ColumnNmbr_].HeaderText = "更新"; 
        // ControlComment:LoopStart-PKColumn
        this.gvwGridView1.Columns[_ColumnNmbr_].HeaderText = "_ColumnName_";
        // ControlComment:LoopEnd-PKColumn
        // ControlComment:LoopStart-ElseColumn
        this.gvwGridView1.Columns[_ColumnNmbr_].HeaderText = "_ColumnName_";
        // ControlComment:LoopEnd-ElseColumn

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

    #region CRUD

    /// <summary>追加ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnInsert_Click(FxEventArgs fxEventArgs)
    {
        // 画面遷移（詳細表示）
        return "_TableName_Detail.aspx";
    }

    /// <summary>バッチ更新ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnBatUpd_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        _3TierParameterValue parameterValue = new _3TierParameterValue(
            this.ContentPageFileNoEx, fxEventArgs.ButtonID, "BatchUpdate",
            (string)Session["DAP"], (MyUserInfo)this.UserInfo);

        // テーブル
        parameterValue.TableName = "_TableName_";

        // 主キーとタイムスタンプ列
        parameterValue.AndEqualSearchConditions = new Dictionary<string, object>();

        // 主キー列
        // ControlComment:LoopStart-PKColumn
        parameterValue.AndEqualSearchConditions.Add("_ColumnName_", "");
        // ControlComment:LoopEnd-PKColumn

        // タイムスタンプ列
        // ・・・

        // DataTableを設定
        parameterValue.Obj = (DataTable)Session["SearchResult"];

        // B層を生成
        _3TierEngine b = new _3TierEngine();

        // データ取得処理を実行
        _3TierReturnValue returnValue =
            (_3TierReturnValue)b.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

        // 結果表示
        //this.lblResult.Text = returnValue.Obj.ToString() + "件更新しました。";

        // 更新ボタンの非活性化
        this.btnBatUpd.Enabled = false;

        // 画面遷移しない。
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

                        // e.NewSelectedIndexとRowsのインデックスをチェック
                        i++;
                        if (index == i)
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

                        // e.NewSelectedIndexとRowsのインデックスをチェック
                        i++;
                        if (index == i)
                        {
                            // 更新
                            GridViewRow gvRow = this.gvwGridView1.Rows[index];
                            foreach (DataColumn dc in dt.Columns)
                            {
                                TextBox txtBox = ((TextBox)gvRow.FindControl("txt" + dc.ColumnName));

                                if (txtBox != null)
                                {
                                    if (dr[dc].GetType() == typeof(Byte[]))
                                    {

                                    }
                                    else { dr[dc] = txtBox.Text; }
                                }

                                #region 追加コード（ComboBox化）

                                DropDownList ddl = ((DropDownList)gvRow.FindControl("ddl" + dc.ColumnName));

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

        // GridViewをリセット
        this.gvwGridView1.PageIndex = 0;
        this.gvwGridView1.Sort("", SortDirection.Ascending);

        // ページングの中止
        this.gvwGridView1.AllowPaging = false;

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

    #endregion

    #endregion
}
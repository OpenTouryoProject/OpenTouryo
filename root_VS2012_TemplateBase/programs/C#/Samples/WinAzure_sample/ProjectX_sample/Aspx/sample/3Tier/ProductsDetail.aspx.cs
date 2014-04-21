//**********************************************************************************
//* 三層データバインド・アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：_Detail_
//* クラス日本語名  ：三層データバインド・詳細表示画面（_TableName_）
//*
//* 作成日時        ：_TimeStamp_
//* 作成者          ：自動生成ツール（墨壺２）, _UserName_
//* 更新履歴        ：
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

/// <summary>三層データバインド・サンプル アプリ画面（詳細表示）</summary>
public partial class Aspx_sample_3Tier_ProductsDetail : MyBaseController
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

        // 詳細表示処理

        // 引数クラスを生成
        _3TierParameterValue parameterValue = new _3TierParameterValue(
                this.ContentPageFileNoEx, "FormInit", "SelectRecord",
                (string)Session["DAP"], (MyUserInfo)UserInfoHandle.GetUserInformation());

        // テーブル
        parameterValue.TableName = "Products";

        // 主キーとタイムスタンプ列
        // 主キー列
        parameterValue.AndEqualSearchConditions = (Dictionary<string, object>)Session["PrimaryKeyAndTimeStamp"];
        // タイムスタンプ列

        // B層を生成
        _3TierEngine b = new _3TierEngine();

        // データ取得処理を実行
        _3TierReturnValue returnValue =
            (_3TierReturnValue)b.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

        // 編集状態の初期化

        // 値
        this.txtProductID.Text = returnValue.Dt.Rows[0]["ProductID"].ToString();
        this.txtProductName.Text = returnValue.Dt.Rows[0]["ProductName"].ToString();
        this.txtSupplierID.Text = returnValue.Dt.Rows[0]["SupplierID"].ToString();
        this.txtCategoryID.Text = returnValue.Dt.Rows[0]["CategoryID"].ToString();
        this.txtQuantityPerUnit.Text = returnValue.Dt.Rows[0]["QuantityPerUnit"].ToString();
        this.txtUnitPrice.Text = returnValue.Dt.Rows[0]["UnitPrice"].ToString();
        this.txtUnitsInStock.Text = returnValue.Dt.Rows[0]["UnitsInStock"].ToString();
        this.txtUnitsOnOrder.Text = returnValue.Dt.Rows[0]["UnitsOnOrder"].ToString();
        this.txtReorderLevel.Text = returnValue.Dt.Rows[0]["ReorderLevel"].ToString();
        this.txtDiscontinued.Text = returnValue.Dt.Rows[0]["Discontinued"].ToString();

        // 編集
        this.txtProductID.ReadOnly = true;
        this.txtProductName.ReadOnly = true;
        this.txtSupplierID.ReadOnly = true;
        this.txtCategoryID.ReadOnly = true;
        this.txtQuantityPerUnit.ReadOnly = true;
        this.txtUnitPrice.ReadOnly = true;
        this.txtUnitsInStock.ReadOnly = true;
        this.txtUnitsOnOrder.ReadOnly = true;
        this.txtReorderLevel.ReadOnly = true;
        this.txtDiscontinued.ReadOnly = true;

        // 背景色
        this.txtProductID.BackColor = System.Drawing.Color.LightGray;
        this.txtProductName.BackColor = System.Drawing.Color.LightGray;
        this.txtSupplierID.BackColor = System.Drawing.Color.LightGray;
        this.txtCategoryID.BackColor = System.Drawing.Color.LightGray;
        this.txtQuantityPerUnit.BackColor = System.Drawing.Color.LightGray;
        this.txtUnitPrice.BackColor = System.Drawing.Color.LightGray;
        this.txtUnitsInStock.BackColor = System.Drawing.Color.LightGray;
        this.txtUnitsOnOrder.BackColor = System.Drawing.Color.LightGray;
        this.txtReorderLevel.BackColor = System.Drawing.Color.LightGray;
        this.txtDiscontinued.BackColor = System.Drawing.Color.LightGray;
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
    }

    #endregion

    #region イベントハンドラ

    #region 編集状態の変更

    /// <summary>編集ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnEdit_Click(FxEventArgs fxEventArgs)
    {
        // 編集状態の変更

        // 編集
        //this.txtProductID.ReadOnly = false;
        this.txtProductName.ReadOnly = false;
        this.txtSupplierID.ReadOnly = false;
        this.txtCategoryID.ReadOnly = false;
        this.txtQuantityPerUnit.ReadOnly = false;
        this.txtUnitPrice.ReadOnly = false;
        this.txtUnitsInStock.ReadOnly = false;
        this.txtUnitsOnOrder.ReadOnly = false;
        this.txtReorderLevel.ReadOnly = false;
        this.txtDiscontinued.ReadOnly = false;

        // 背景色
        //this.txtProductID.BackColor = System.Drawing.Color.Empty;
        this.txtProductName.BackColor = System.Drawing.Color.Empty;
        this.txtSupplierID.BackColor = System.Drawing.Color.Empty;
        this.txtCategoryID.BackColor = System.Drawing.Color.Empty;
        this.txtQuantityPerUnit.BackColor = System.Drawing.Color.Empty;
        this.txtUnitPrice.BackColor = System.Drawing.Color.Empty;
        this.txtUnitsInStock.BackColor = System.Drawing.Color.Empty;
        this.txtUnitsOnOrder.BackColor = System.Drawing.Color.Empty;
        this.txtReorderLevel.BackColor = System.Drawing.Color.Empty;
        this.txtDiscontinued.BackColor = System.Drawing.Color.Empty;

        // 画面遷移しない。
        return string.Empty;
    }

    /// <summary>キャンセル・ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnCancel_Click(FxEventArgs fxEventArgs)
    {
        // 編集状態の変更

        // 編集
        //this.txtProductID.ReadOnly = true;
        this.txtProductName.ReadOnly = true;
        this.txtSupplierID.ReadOnly = true;
        this.txtCategoryID.ReadOnly = true;
        this.txtQuantityPerUnit.ReadOnly = true;
        this.txtUnitPrice.ReadOnly = true;
        this.txtUnitsInStock.ReadOnly = true;
        this.txtUnitsOnOrder.ReadOnly = true;
        this.txtReorderLevel.ReadOnly = true;
        this.txtDiscontinued.ReadOnly = true;

        // 背景色
        //this.txtProductID.BackColor = System.Drawing.Color.LightGray;
        this.txtProductName.BackColor = System.Drawing.Color.LightGray;
        this.txtSupplierID.BackColor = System.Drawing.Color.LightGray;
        this.txtCategoryID.BackColor = System.Drawing.Color.LightGray;
        this.txtQuantityPerUnit.BackColor = System.Drawing.Color.LightGray;
        this.txtUnitPrice.BackColor = System.Drawing.Color.LightGray;
        this.txtUnitsInStock.BackColor = System.Drawing.Color.LightGray;
        this.txtUnitsOnOrder.BackColor = System.Drawing.Color.LightGray;
        this.txtReorderLevel.BackColor = System.Drawing.Color.LightGray;
        this.txtDiscontinued.BackColor = System.Drawing.Color.LightGray;

        // 画面遷移しない。
        return string.Empty;
    }

    #endregion

    #region 更新系

    /// <summary>追加ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnInsert_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        _3TierParameterValue parameterValue = new _3TierParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "InsertRecord",
                (string)Session["DAP"], (MyUserInfo)UserInfoHandle.GetUserInformation());

        // テーブル
        parameterValue.TableName = "Products";

        // 追加値（TimeStamp列は外す。
        parameterValue.InsertUpdateValues = new Dictionary<string, object>();
        //parameterValue.InsertUpdateValues.Add("ProductID", this.txtProductID.Text);
        parameterValue.InsertUpdateValues.Add("ProductName", this.txtProductName.Text);
        parameterValue.InsertUpdateValues.Add("SupplierID", this.txtSupplierID.Text);
        parameterValue.InsertUpdateValues.Add("CategoryID", this.txtCategoryID.Text);
        parameterValue.InsertUpdateValues.Add("QuantityPerUnit", this.txtQuantityPerUnit.Text);
        parameterValue.InsertUpdateValues.Add("UnitPrice", this.txtUnitPrice.Text);
        parameterValue.InsertUpdateValues.Add("UnitsInStock", this.txtUnitsInStock.Text);
        parameterValue.InsertUpdateValues.Add("UnitsOnOrder", this.txtUnitsOnOrder.Text);
        parameterValue.InsertUpdateValues.Add("ReorderLevel", this.txtReorderLevel.Text);
        parameterValue.InsertUpdateValues.Add("Discontinued", this.txtDiscontinued.Text);

        // B層を生成
        _3TierEngine b = new _3TierEngine();

        // データ取得処理を実行
        _3TierReturnValue returnValue =
            (_3TierReturnValue)b.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

        // 結果表示
        this.lblResult.Text = returnValue.Obj.ToString() + "件追加しました。";

        // 画面遷移しない。
        return string.Empty;
    }

    /// <summary>更新ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnUpdate_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        _3TierParameterValue parameterValue = new _3TierParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "UpdateRecord",
                (string)Session["DAP"], (MyUserInfo)UserInfoHandle.GetUserInformation());

        // テーブル
        parameterValue.TableName = "Products";

        // 主キーとタイムスタンプ列
        // 主キー列
        parameterValue.AndEqualSearchConditions = (Dictionary<string, object>)Session["PrimaryKeyAndTimeStamp"];
        // タイムスタンプ列

        // 更新値（TimeStamp列は外す。
        parameterValue.InsertUpdateValues = new Dictionary<string, object>();
        //parameterValue.InsertUpdateValues.Add("ProductID", this.txtProductID.Text);
        parameterValue.InsertUpdateValues.Add("ProductName", this.txtProductName.Text);
        parameterValue.InsertUpdateValues.Add("SupplierID", this.txtSupplierID.Text);
        parameterValue.InsertUpdateValues.Add("CategoryID", this.txtCategoryID.Text);
        parameterValue.InsertUpdateValues.Add("QuantityPerUnit", this.txtQuantityPerUnit.Text);
        parameterValue.InsertUpdateValues.Add("UnitPrice", this.txtUnitPrice.Text);
        parameterValue.InsertUpdateValues.Add("UnitsInStock", this.txtUnitsInStock.Text);
        parameterValue.InsertUpdateValues.Add("UnitsOnOrder", this.txtUnitsOnOrder.Text);
        parameterValue.InsertUpdateValues.Add("ReorderLevel", this.txtReorderLevel.Text);
        parameterValue.InsertUpdateValues.Add("Discontinued", this.txtDiscontinued.Text);

        // B層を生成
        _3TierEngine b = new _3TierEngine();

        // データ取得処理を実行
        _3TierReturnValue returnValue =
            (_3TierReturnValue)b.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

        // 結果表示
        this.lblResult.Text = returnValue.Obj.ToString() + "件更新しました。";

        // 画面遷移しない。
        return string.Empty;
    }

    /// <summary>削除ボタン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnDelete_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        _3TierParameterValue parameterValue = new _3TierParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "DeleteRecord",
                (string)Session["DAP"], (MyUserInfo)UserInfoHandle.GetUserInformation());

        // テーブル
        parameterValue.TableName = "Products";

        // 主キーとタイムスタンプ列
        // 主キー列
        parameterValue.AndEqualSearchConditions = (Dictionary<string, object>)Session["PrimaryKeyAndTimeStamp"];
        // タイムスタンプ列

        // B層を生成
        _3TierEngine b = new _3TierEngine();

        // データ取得処理を実行
        _3TierReturnValue returnValue =
            (_3TierReturnValue)b.DoBusinessLogic(
                (BaseParameterValue)parameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

        // 結果表示
        this.lblResult.Text = returnValue.Obj.ToString() + "件削除しました。";

        // 画面遷移しない。
        return string.Empty;
    }

    #endregion

    #endregion
}
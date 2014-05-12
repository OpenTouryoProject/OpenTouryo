//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_sample_crud_sampleScreen_cc
//* クラス日本語名  ：サンプル アプリ画面
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

// 型情報
using WSIFType_sample;

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

/// <summary>サンプル アプリ画面</summary>
public partial class Aspx_sample_crud_sampleScreen_cc : MyBaseController
{
    #region ページロードのUOCメソッド

    /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit()
    {
        // フォーム初期化（初回ロード）時に実行する処理を実装する
        // TODO:
    }

    /// <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit_PostBack()
    {
        // フォーム初期化（ポストバック）時に実行する処理を実装する
        // TODO:
    }
    
    #endregion

    #region ＣＲＵＤ処理メソッド

    #region 参照系

    /// <summary>
    /// btnMButton1のクリックイベント（件数取得）
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_sampleScreen_btnMButton1_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        TestParameterValue testParameterValue
            = new TestParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectCount",
                this.ddlDap.SelectedValue + "%"
                + this.ddlMode1.SelectedValue + "%"
                + this.ddlMode2.SelectedValue + "%"
                + this.ddlExRollback.SelectedValue,
                this.UserInfo);

        // 戻り値
        TestReturnValue testReturnValue;

        // 呼出し制御部品
        CallController cctrl = new CallController(this.UserInfo);

        // Invoke
        testReturnValue = (TestReturnValue)cctrl.Invoke(
            this.ddlCmctCtrl.SelectedValue, testParameterValue);

        // 結果表示するメッセージ エリア
        Label label = (Label)this.GetMasterWebControl("Label1");
        label.Text = "";

        if (testReturnValue.ErrorFlag == true)
        {
            // 結果（業務続行可能なエラー）
            label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
            label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
            label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
        }
        else
        {
            // 結果（正常系）
            label.Text = testReturnValue.Obj.ToString() + "件のデータがあります";
        }

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// btnMButton2のクリックイベント（一覧取得（dt））
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_sampleScreen_btnMButton2_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        TestParameterValue testParameterValue
            = new TestParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectAll_DT",
                this.ddlDap.SelectedValue + "%"
                + this.ddlMode1.SelectedValue + "%"
                + this.ddlMode2.SelectedValue + "%"
                + this.ddlExRollback.SelectedValue,
                this.UserInfo);

        // 戻り値
        TestReturnValue testReturnValue;

        // 呼出し制御部品
        CallController cctrl = new CallController(this.UserInfo);

        // Invoke
        testReturnValue = (TestReturnValue)cctrl.Invoke(
            this.ddlCmctCtrl.SelectedValue, testParameterValue);

        // 結果表示するメッセージ エリア
        Label label = (Label)this.GetMasterWebControl("Label1");
        label.Text = "";

        if (testReturnValue.ErrorFlag == true)
        {
            // 結果（業務続行可能なエラー）
            label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
            label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
            label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
        }
        else
        {
            // 結果（正常系）
            this.GridView1.DataSource = testReturnValue.Obj;
            this.GridView1.DataBind();
        }

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// btnMButton3のクリックイベント（一覧取得（ds））
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_sampleScreen_btnMButton3_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        TestParameterValue testParameterValue
            = new TestParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectAll_DS",
                this.ddlDap.SelectedValue + "%"
                + this.ddlMode1.SelectedValue + "%"
                + this.ddlMode2.SelectedValue + "%"
                + this.ddlExRollback.SelectedValue,
                this.UserInfo);

        // 戻り値
        TestReturnValue testReturnValue;

        // 呼出し制御部品
        CallController cctrl = new CallController(this.UserInfo);

        // Invoke
        testReturnValue = (TestReturnValue)cctrl.Invoke(
            this.ddlCmctCtrl.SelectedValue, testParameterValue);

        // 結果表示するメッセージ エリア
        Label label = (Label)this.GetMasterWebControl("Label1");
        label.Text = "";

        if (testReturnValue.ErrorFlag == true)
        {
            // 結果（業務続行可能なエラー）
            label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
            label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
            label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
        }
        else
        {
            // 結果（正常系）
            DataSet ds = (DataSet)testReturnValue.Obj;
            this.GridView1.DataSource = ds.Tables[0];
            this.GridView1.DataBind();
        }

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// btnMButton4のクリックイベント（一覧取得（dr））
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_sampleScreen_btnMButton4_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        TestParameterValue testParameterValue
            = new TestParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectAll_DR",
                this.ddlDap.SelectedValue + "%"
                + this.ddlMode1.SelectedValue + "%"
                + this.ddlMode2.SelectedValue + "%"
                + this.ddlExRollback.SelectedValue,
                this.UserInfo);

        // 戻り値
        TestReturnValue testReturnValue;

        // 呼出し制御部品
        CallController cctrl = new CallController(this.UserInfo);

        // Invoke
        testReturnValue = (TestReturnValue)cctrl.Invoke(
            this.ddlCmctCtrl.SelectedValue, testParameterValue);

        // 結果表示するメッセージ エリア
        Label label = (Label)this.GetMasterWebControl("Label1");
        label.Text = "";

        if (testReturnValue.ErrorFlag == true)
        {
            // 結果（業務続行可能なエラー）
            label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
            label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
            label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
        }
        else
        {
            // 結果（正常系）
            this.GridView1.DataSource = testReturnValue.Obj;
            this.GridView1.DataBind();
        }

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// btnMButton5のクリックイベント（一覧取得（動的sql））
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_sampleScreen_btnMButton5_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        TestParameterValue testParameterValue
            = new TestParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "SelectAll_DSQL",
                this.ddlDap.SelectedValue + "%"
                + this.ddlMode1.SelectedValue + "%"
                + this.ddlMode2.SelectedValue + "%"
                + this.ddlExRollback.SelectedValue,
                this.UserInfo);

        // 動的SQLの要素を設定
        testParameterValue.OrderColumn = this.ddlOrderColumn.SelectedValue;
        testParameterValue.OrderSequence = this.ddlOrderSequence.SelectedValue;

        // 戻り値
        TestReturnValue testReturnValue;

        // 呼出し制御部品
        CallController cctrl = new CallController(this.UserInfo);

        // Invoke
        testReturnValue = (TestReturnValue)cctrl.Invoke(
            this.ddlCmctCtrl.SelectedValue, testParameterValue);

        // 結果表示するメッセージ エリア
        Label label = (Label)this.GetMasterWebControl("Label1");
        label.Text = "";

        if (testReturnValue.ErrorFlag == true)
        {
            // 結果（業務続行可能なエラー）
            label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
            label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
            label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
        }
        else
        {
            // 結果（正常系）
            this.GridView1.DataSource = testReturnValue.Obj;
            this.GridView1.DataBind();
        }

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// btnMButton6のクリックイベント（参照処理）
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_sampleScreen_btnMButton6_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        TestParameterValue testParameterValue
            = new TestParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "Select",
                this.ddlDap.SelectedValue + "%"
                + this.ddlMode1.SelectedValue + "%"
                + this.ddlMode2.SelectedValue + "%"
                + this.ddlExRollback.SelectedValue,
                this.UserInfo);

        // 情報の設定
        testParameterValue.ShipperID = int.Parse(this.TextBox1.Text);

        // 戻り値
        TestReturnValue testReturnValue;

        // 呼出し制御部品
        CallController cctrl = new CallController(this.UserInfo);

        // Invoke
        testReturnValue = (TestReturnValue)cctrl.Invoke(
            this.ddlCmctCtrl.SelectedValue, testParameterValue);

        // 結果表示するメッセージ エリア
        Label label = (Label)this.GetMasterWebControl("Label1");
        label.Text = "";

        if (testReturnValue.ErrorFlag == true)
        {
            // 結果（業務続行可能なエラー）
            label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
            label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
            label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
        }
        else
        {
            // 結果（正常系）
            this.TextBox1.Text = testReturnValue.ShipperID.ToString();
            this.TextBox2.Text = testReturnValue.CompanyName;
            this.TextBox3.Text = testReturnValue.Phone;
        }

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #region 更新系

    /// <summary>
    /// btnMButton7のクリックイベント（追加処理）
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_sampleScreen_btnMButton7_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        TestParameterValue testParameterValue
            = new TestParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "Insert",
                this.ddlDap.SelectedValue + "%"
                + this.ddlMode1.SelectedValue + "%"
                + this.ddlMode2.SelectedValue + "%"
                + this.ddlExRollback.SelectedValue,
                this.UserInfo);

        // 情報の設定
        testParameterValue.CompanyName = this.TextBox2.Text;
        testParameterValue.Phone = this.TextBox3.Text;

        // 戻り値
        TestReturnValue testReturnValue;

        // 呼出し制御部品
        CallController cctrl = new CallController(this.UserInfo);

        // Invoke
        testReturnValue = (TestReturnValue)cctrl.Invoke(
            this.ddlCmctCtrl.SelectedValue, testParameterValue);

        // 結果表示するメッセージ エリア
        Label label = (Label)this.GetMasterWebControl("Label1");
        label.Text = "";

        if (testReturnValue.ErrorFlag == true)
        {
            // 結果（業務続行可能なエラー）
            label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
            label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
            label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
        }
        else
        {
            // 結果（正常系）
            label.Text = testReturnValue.Obj.ToString() + "件追加";
        }

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// btnMButton8のクリックイベント（更新処理）
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_sampleScreen_btnMButton8_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        TestParameterValue testParameterValue
            = new TestParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "Update",
                this.ddlDap.SelectedValue + "%"
                + this.ddlMode1.SelectedValue + "%"
                + this.ddlMode2.SelectedValue + "%"
                + this.ddlExRollback.SelectedValue,
                this.UserInfo);

        // 情報の設定
        testParameterValue.ShipperID = int.Parse(this.TextBox1.Text);
        testParameterValue.CompanyName = this.TextBox2.Text;
        testParameterValue.Phone = this.TextBox3.Text;

        // 戻り値
        TestReturnValue testReturnValue;

        // 呼出し制御部品
        CallController cctrl = new CallController(this.UserInfo);

        // Invoke
        testReturnValue = (TestReturnValue)cctrl.Invoke(
            this.ddlCmctCtrl.SelectedValue, testParameterValue);

        // 結果表示するメッセージ エリア
        Label label = (Label)this.GetMasterWebControl("Label1");
        label.Text = "";

        if (testReturnValue.ErrorFlag == true)
        {
            // 結果（業務続行可能なエラー）
            label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
            label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
            label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
        }
        else
        {
            // 結果（正常系）
            label.Text = testReturnValue.Obj.ToString() + "件更新";
        }

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// btnMButton9のクリックイベント（削除処理）
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_sampleScreen_btnMButton9_Click(FxEventArgs fxEventArgs)
    {
        // 引数クラスを生成
        // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
        TestParameterValue testParameterValue
            = new TestParameterValue(
                this.ContentPageFileNoEx, fxEventArgs.ButtonID, "Delete",
                this.ddlDap.SelectedValue + "%"
                + this.ddlMode1.SelectedValue + "%"
                + this.ddlMode2.SelectedValue + "%"
                + this.ddlExRollback.SelectedValue,
                this.UserInfo);

        // 情報の設定
        testParameterValue.ShipperID = int.Parse(TextBox1.Text);

        // 戻り値
        TestReturnValue testReturnValue;

        // 呼出し制御部品
        CallController cctrl = new CallController(this.UserInfo);

        // Invoke
        testReturnValue = (TestReturnValue)cctrl.Invoke(
            this.ddlCmctCtrl.SelectedValue, testParameterValue);

        // 結果表示するメッセージ エリア
        Label label = (Label)this.GetMasterWebControl("Label1");
        label.Text = "";

        if (testReturnValue.ErrorFlag == true)
        {
            // 結果（業務続行可能なエラー）
            label.Text = "ErrorMessageID:" + testReturnValue.ErrorMessageID + "\r\n";
            label.Text += "ErrorMessage:" + testReturnValue.ErrorMessage + "\r\n";
            label.Text += "ErrorInfo:" + testReturnValue.ErrorInfo + "\r\n";
        }
        else
        {
            // 結果（正常系）
            label.Text = testReturnValue.Obj.ToString() + "件削除";
        }

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #endregion

    #region Ｐ層で例外をスロー

    /// <summary>
    /// btnButton1のクリックイベント（業務例外）
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
    {
        throw new BusinessApplicationException(
            "Ｐ層で「業務例外」をスロー",
            "Ｐ層で「業務例外」をスロー",
            "Ｐ層で「業務例外」をスロー");
    }

    /// <summary>
    /// btnButton2のクリックイベント（システム例外）
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton2_Click(FxEventArgs fxEventArgs)
    {
        throw new BusinessSystemException(
            "Ｐ層で「システム例外」をスロー",
            "Ｐ層で「システム例外」をスロー");
    }

    /// <summary>
    /// btnButton3のクリックイベント（その他、一般的な例外）
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton3_Click(FxEventArgs fxEventArgs)
    {
        throw new Exception("Ｐ層で「その他、一般的な例外」をスロー");
    }

    /// <summary>
    /// btnButton4のクリックイベント（その他、一般的な例外）
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton4_Click(FxEventArgs fxEventArgs)
    {
        this.GridView1.DataSource = null;
        this.GridView1.DataBind();

        return "";
    }

    #endregion    

    #region マスタページ、ユーザコントロールのイベント

    ///// <summary>マスタページにイベントハンドラを実装可能にしたのでそのテスト。</summary>
    ///// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ///// <returns>URL</returns>
    //protected string UOC_sampleScreen_btnMPButton_Click(FxEventArgs fxEventArgs)
    //{
    //    Response.Write("UOC_btnMPButton_Clickを実行できた。（ｃｃ）");

    //    return "";
    //}

    ///// <summary>ユーザコントロールにイベントハンドラを実装可能にしたのでそのテスト。</summary>
    ///// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    ///// <returns>URL</returns>
    //protected string UOC_sampleControl_btnUCButton_Click(FxEventArgs fxEventArgs)
    //{
    //    Response.Write("UOC_btnUCButton_Clickを実行できた。（ｃｃ）");

    //    return "";
    //}

    #endregion

}

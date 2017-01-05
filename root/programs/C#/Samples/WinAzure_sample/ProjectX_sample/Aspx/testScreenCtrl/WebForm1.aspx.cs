//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testScreenCtrl_WebForm1
//* クラス日本語名  ：画面遷移制御機能テスト画面１（Ｐ層）
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

/// <summary>画面遷移制御機能テスト画面１（Ｐ層）</summary>
public partial class Aspx_testScreenCtrl_WebForm1 : MyBaseController
{
    // 部品使用する・しないフラグ
    private bool IsFx = false;

    #region ページロードのUOCメソッド

    /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit()
    {
        // フォーム初期化（初回ロード）時に実行する処理を実装する
        // TODO:

        // 状態の通知
        Label lblStatus = (Label)this.GetMasterWebControl("lblStatus");

        if (Request.HttpMethod.ToUpper() == "GET")
        {
            lblStatus.Text = "これは、Redirectによる遷移";
        }
        else if (Request.HttpMethod.ToUpper() == "POST")
        {
            lblStatus.Text = "これは、Transferによる遷移";
        }
        else
        {
            lblStatus.Text = "不明な遷移";
        }

        // ---

        // QueryStringの通知
        Label lblQueryString = (Label)this.GetMasterWebControl("lblQueryString");

        foreach (string qsKey in Request.QueryString.AllKeys)
        {
            lblQueryString.Text += qsKey + "=" + Request.QueryString[qsKey] + ";";
        }
    }

    /// <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit_PostBack()
    {
        // フォーム初期化（ポストバック）時に実行する処理を実装する
        // TODO:

        // 状態の通知
        Label lblStatus = (Label)this.GetMasterWebControl("lblStatus");
        lblStatus.Text = "これは、ポスト（ポストバックです）";

        // ---

        // Fxを使用するモード
        if (((CheckBox)this.GetContentWebControl("CheckBox1")).Checked)
        {
            this.IsFx = true;
        }
    }

    #endregion

    #region マスタ ページ上のフレームワーク対象コントロール

    #region チェック可能な画面遷移（外サイトへ）

    /// <summary>
    /// ibnMImageButton1のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_testScreenCtrl_ibnMImageButton1_Click(FxEventArgs fxEventArgs)
    {
        // 外部サイトへ（QueryString付き）
        return "google?q=WebForm1";
    }

    #endregion

    #endregion

    #region コンテンツ ページ上のフレームワーク対象コントロール

    #region チェック可能な画面遷移

    /// <summary>btnButton1のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>画面遷移不可能（×）</remarks>
    protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
    {
        return "1→1";
    }

    /// <summary>btnButton2のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>画面遷移可能（○）</remarks>
    protected string UOC_btnButton2_Click(FxEventArgs fxEventArgs)
    {
        return "1→2";
    }

    /// <summary>btnButton3のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>画面遷移不可能（×）</remarks>
    protected string UOC_btnButton3_Click(FxEventArgs fxEventArgs)
    {
        return "1→3";
    }

    /// <summary>btnButton3のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>画面遷移不可能（×）</remarks>
    protected string UOC_btnButton4_Click(FxEventArgs fxEventArgs)
    {
        return "1→4";
    }

    /// <summary>btnButton3のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>画面遷移不可能（×）</remarks>
    protected string UOC_btnButton5_Click(FxEventArgs fxEventArgs)
    {
        return "1→5";
    }

    #endregion

    #region チェック不可能な画面遷移

    #region Transfer or FxTransfer

    /// <summary>btnButton6のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>違法な画面遷移（Transfer）（×）</remarks>
    protected string UOC_btnButton6_Click(FxEventArgs fxEventArgs)
    {
        if (this.IsFx)
        {
            this.FxTransfer("./WebForm1.aspx");
        }
        else
        {
            Server.Transfer("./WebForm1.aspx");
        }

        return "";
    }

    /// <summary>btnButton7のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>違法な画面遷移（Transfer）（×）</remarks>
    protected string UOC_btnButton7_Click(FxEventArgs fxEventArgs)
    {
        if (this.IsFx)
        {
            this.FxTransfer("./WebForm2.aspx");
        }
        else
        {
            Server.Transfer("./WebForm2.aspx");
        }

        return "";
    }

    /// <summary>btnButton8のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>違法な画面遷移（Transfer）（×）</remarks>
    protected string UOC_btnButton8_Click(FxEventArgs fxEventArgs)
    {
        if (this.IsFx)
        {
            this.FxTransfer("./WebForm3.aspx");
        }
        else
        {
            Server.Transfer("./WebForm3.aspx");
        }

        return "";
    }

    /// <summary>btnButton9のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>違法な画面遷移（Transfer）（×）</remarks>
    protected string UOC_btnButton9_Click(FxEventArgs fxEventArgs)
    {
        if (this.IsFx)
        {
            this.FxTransfer("./WebForm4.aspx");
        }
        else
        {
            Server.Transfer("./WebForm4.aspx");
        }

        return "";
    }

    /// <summary>btnButton10のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>違法な画面遷移（Transfer）（×）</remarks>
    protected string UOC_btnButton10_Click(FxEventArgs fxEventArgs)
    {
        if (this.IsFx)
        {
            this.FxTransfer("./WebForm5.aspx");
        }
        else
        {
            Server.Transfer("./WebForm5.aspx");
        }

        return "";
    }

    #endregion

    #region Redirect or FxRedirect

    /// <summary>btnButton11のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>違法な画面遷移（Redirect）（×）</remarks>
    protected string UOC_btnButton11_Click(FxEventArgs fxEventArgs)
    {
        if (this.IsFx)
        {
            this.FxRedirect("./WebForm1.aspx");
        }
        else
        {
            Response.Redirect("./WebForm1.aspx");
        }

        return "";
    }

    /// <summary>btnButton12のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>違法な画面遷移（Redirect）（×）</remarks>
    protected string UOC_btnButton12_Click(FxEventArgs fxEventArgs)
    {
        if (this.IsFx)
        {
            this.FxRedirect("./WebForm2.aspx");
        }
        else
        {
            Response.Redirect("./WebForm2.aspx");
        }

        return "";
    }

    /// <summary>btnButton13のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>違法な画面遷移（Redirect）（×）</remarks>
    protected string UOC_btnButton13_Click(FxEventArgs fxEventArgs)
    {
        if (this.IsFx)
        {
            this.FxRedirect("./WebForm3.aspx");
        }
        else
        {
            Response.Redirect("./WebForm3.aspx");
        }

        return "";
    }

    /// <summary>btnButton14のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>違法な画面遷移（Redirect）（×）</remarks>
    protected string UOC_btnButton14_Click(FxEventArgs fxEventArgs)
    {
        if (this.IsFx)
        {
            this.FxRedirect("./WebForm4.aspx");
        }
        else
        {
            Response.Redirect("./WebForm4.aspx");
        }

        return "";
    }

    /// <summary>btnButton15のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>違法な画面遷移（Redirect）（×）</remarks>
    protected string UOC_btnButton15_Click(FxEventArgs fxEventArgs)
    {
        if (this.IsFx)
        {
            this.FxRedirect("./WebForm5.aspx");
        }
        else
        {
            Response.Redirect("./WebForm5.aspx");
        }

        return "";
    }

    #endregion

    #endregion

    #region ポストバック

    /// <summary>btnButton16のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>違法な画面遷移（Redirect）（×）</remarks>
    protected string UOC_btnButton16_Click(FxEventArgs fxEventArgs)
    {
        this.ShowOKMessageDialog("ポストバックの", "テストです", FxEnum.IconType.Information, "テスト");
        return "";
    }

    #endregion

    #region 子画面の表示

    #region window open

    /// <summary>btnButton17のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>window open</remarks>
    protected string UOC_btnButton17_Click(FxEventArgs fxEventArgs)
    {
        this.ShowNormalScreen("./WebForm1.aspx");
        return "";
    }

    /// <summary>btnButton18のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>window open</remarks>
    protected string UOC_btnButton18_Click(FxEventArgs fxEventArgs)
    {
        this.ShowNormalScreen("./WebForm2.aspx");
        return "";
    }

    /// <summary>btnButton19のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>window open</remarks>
    protected string UOC_btnButton19_Click(FxEventArgs fxEventArgs)
    {
        this.ShowNormalScreen("./WebForm3.aspx");
        return "";
    }

    /// <summary>btnButton20のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>window open</remarks>
    protected string UOC_btnButton20_Click(FxEventArgs fxEventArgs)
    {
        this.ShowNormalScreen("./WebForm4.aspx");
        return "";
    }

    /// <summary>btnButton21のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>window open</remarks>
    protected string UOC_btnButton21_Click(FxEventArgs fxEventArgs)
    {
        this.ShowNormalScreen("./WebForm5.aspx");
        return "";
    }

    #endregion

    #region dialog

    /// <summary>btnButton22のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>dialog</remarks>
    protected string UOC_btnButton22_Click(FxEventArgs fxEventArgs)
    {
        this.ShowModalScreen("~/Aspx/testScreenCtrl/WebForm1.aspx");
        return "";
    }

    /// <summary>btnButton23のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>dialog</remarks>
    protected string UOC_btnButton23_Click(FxEventArgs fxEventArgs)
    {
        this.ShowModalScreen("~/Aspx/testScreenCtrl/WebForm2.aspx");
        return "";
    }

    /// <summary>btnButton24のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>dialog</remarks>
    protected string UOC_btnButton24_Click(FxEventArgs fxEventArgs)
    {
        this.ShowModalScreen("~/Aspx/testScreenCtrl/WebForm3.aspx");
        return "";
    }

    /// <summary>btnButton25のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>dialog</remarks>
    protected string UOC_btnButton25_Click(FxEventArgs fxEventArgs)
    {
        this.ShowModalScreen("~/Aspx/testScreenCtrl/WebForm4.aspx");
        return "";
    }

    /// <summary>btnButton26のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>dialog</remarks>
    protected string UOC_btnButton26_Click(FxEventArgs fxEventArgs)
    {
        this.ShowModalScreen("~/Aspx/testScreenCtrl/WebForm5.aspx");
        return "";
    }

    #endregion

    #endregion

    #region ブラウザ ウィンドウ別セッション領域

    /// <summary>btnButton27のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>dialog</remarks>
    protected string UOC_btnButton27_Click(FxEventArgs fxEventArgs)
    {
        // ブラウザ ウィンドウ別セッション領域 - 設定
        this.SetDataToBrowserWindow("msg", this.TextBox1.Text);
        return "";
    }

    /// <summary>btnButton28のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    /// <remarks>dialog</remarks>
    protected string UOC_btnButton28_Click(FxEventArgs fxEventArgs)
    {
        // ブラウザ ウィンドウ別セッション領域 - 取得
        this.TextBox1.Text = (string)this.GetDataFromBrowserWindow("msg");
        return "";
    }

    #endregion

    #endregion
}

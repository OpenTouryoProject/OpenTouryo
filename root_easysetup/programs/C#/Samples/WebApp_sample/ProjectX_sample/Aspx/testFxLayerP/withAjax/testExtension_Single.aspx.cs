//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testFxLayerP_withAjax_testExtension_Single
//* クラス日本語名  ：ASP.NET AJAX Extensionのテスト画面（Ｐ層）
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

/// <summary>ASP.NET AJAX Extensionのテスト画面（Ｐ層）</summary>
public partial class Aspx_testFxLayerP_withAjax_testExtension_Single : MyBaseController
{
    #region ページロードのUOCメソッド

    /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit()
    {
        // フォーム初期化（初回ロード）時に実行する処理を実装する
        // TODO:

        // ScriptManagerにコントロールの動作を指定する。
        // Init、PostBackの双方で都度実行する必要がある。
        this.InitScriptManagerRegister();
    }

    /// <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit_PostBack()
    {
        // フォーム初期化（ポストバック）時に実行する処理を実装する
        // TODO:

        // ScriptManagerにコントロールの動作を指定する。
        // Init、PostBackの双方で都度実行する必要がある。
        this.InitScriptManagerRegister();
    }

    /// <summary>
    /// ScriptManagerにコントロールの動作を指定する。
    /// </summary>
    private void InitScriptManagerRegister()
    {
        // RegisterPostBackControlメソッドで、
        // ・btnButton2
        // ・ddlDropDownList2
        // を非Ajax化する。

        // ※ 逆の動作は、RegisterAsyncPostBackControlになる。

        this.CurrentScriptManager.RegisterPostBackControl(
            this.GetContentWebControl("btnButton2"));
        this.CurrentScriptManager.RegisterPostBackControl(
            this.GetContentWebControl("ddlDropDownList2"));
    }

    #endregion

    #region マスタ ページ上のフレームワーク対象コントロール

    /// <summary>btnMButton4のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_testAspNetAjaxExtension_Single_btnMButton4_Click(FxEventArgs fxEventArgs)
    {
        // 待機する（UpdateProgress、二重送信確認用）
        System.Threading.Thread.Sleep(3000);

        // テキストボックスの値を変更
        TextBox textBox = (TextBox)this.GetMasterWebControl("TextBox5");
        textBox.Text = "ajaxのポストバック（ボタンクリック）";

        // ajaxのイベントハンドラでは画面遷移しないこと。
        return "";
    }

    /// <summary>btnMButton5のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_testAspNetAjaxExtension_Single_btnMButton5_Click(FxEventArgs fxEventArgs)
    {
        // 待機する（二重送信確認用）
        System.Threading.Thread.Sleep(3000);

        // テキストボックスの値を変更
        TextBox textBox = (TextBox)this.GetMasterWebControl("TextBox6");
        textBox.Text = "通常のポストバック（ボタンクリック）";

        return "";
    }

    /// <summary>
    /// ddlMDropDownList3のSelectedIndexChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_testAspNetAjaxExtension_Single_ddlMDropDownList3_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // 待機する（UpdateProgress、二重送信確認用）
        System.Threading.Thread.Sleep(3000);

        // テキストボックスの値を変更
        TextBox textBox = (TextBox)this.GetMasterWebControl("TextBox7");
        textBox.Text = "ajaxのポストバック（ＤＤＬのセレクト インデックス チェンジ）";

        // ajaxのイベントハンドラでは画面遷移しないこと。
        return "";
    }

    /// <summary>
    /// ddlMDropDownList4のSelectedIndexChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_testAspNetAjaxExtension_Single_ddlMDropDownList4_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // 待機する（二重送信確認用）
        System.Threading.Thread.Sleep(3000);

        // テキストボックスの値を変更
        TextBox textBox = (TextBox)this.GetMasterWebControl("TextBox8");
        textBox.Text = "通常のポストバック（ＤＤＬのセレクト インデックス チェンジ）";

        return "";
    }

    /// <summary>btnMButton6のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_testAspNetAjaxExtension_Single_btnMButton6_Click(FxEventArgs fxEventArgs)
    {
        // 待機する（二重送信確認用）
        System.Threading.Thread.Sleep(3000);

        throw new Exception("Ajaxでエラー");

        //return "";
    }

    #endregion

    #region コンテンツ ページ上のフレームワーク対象コントロール

    /// <summary>btnButton1のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
    {
        // Ajaxを制御する場合は、ScriptManagerを使用する。
        // このクラスを使用すると、Ajax中であるかどうかを判別できる。
        bool isInAsyncPostBack = this.CurrentScriptManager.IsInAsyncPostBack;
        FxEnum.AjaxExtStat ajaxES = this.AjaxExtensionStatus;

        // 待機する（UpdateProgress、二重送信確認用）
        System.Threading.Thread.Sleep(3000);

        // テキストボックスの値を変更
        TextBox textBox = (TextBox)this.GetContentWebControl("TextBox1");
        textBox.Text = "ajaxのポストバック（ボタンクリック）";

        // ajaxのイベントハンドラでは画面遷移しないこと。
        return "";
    }

    /// <summary>btnButton2のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton2_Click(FxEventArgs fxEventArgs)
    {
        // Ajaxを制御する場合は、ScriptManagerを使用する。
        // このクラスを使用すると、Ajax中であるかどうかを判別できる。
        bool isInAsyncPostBack = this.CurrentScriptManager.IsInAsyncPostBack;
        FxEnum.AjaxExtStat ajaxES = this.AjaxExtensionStatus;

        // 待機する（二重送信確認用）
        System.Threading.Thread.Sleep(3000);

        // テキストボックスの値を変更
        TextBox textBox = (TextBox)this.GetContentWebControl("TextBox2");
        textBox.Text = "通常のポストバック（ボタンクリック）";

        return "";
    }

    /// <summary>
    /// ddlDropDownList1のSelectedIndexChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ddlDropDownList1_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // Ajaxを制御する場合は、ScriptManagerを使用する。
        // このクラスを使用すると、Ajax中であるかどうかを判別できる。
        bool isInAsyncPostBack = this.CurrentScriptManager.IsInAsyncPostBack;
        FxEnum.AjaxExtStat ajaxES = this.AjaxExtensionStatus;

        // 待機する（UpdateProgress、二重送信確認用）
        System.Threading.Thread.Sleep(3000);

        // テキストボックスの値を変更
        TextBox textBox = (TextBox)this.GetContentWebControl("TextBox3");
        textBox.Text = "ajaxのポストバック（ＤＤＬのセレクト インデックス チェンジ）";

        // ajaxのイベントハンドラでは画面遷移しないこと。
        return "";
    }

    /// <summary>
    /// ddlDropDownList2のSelectedIndexChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ddlDropDownList2_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // Ajaxを制御する場合は、ScriptManagerを使用する。
        // このクラスを使用すると、Ajax中であるかどうかを判別できる。
        bool isInAsyncPostBack = this.CurrentScriptManager.IsInAsyncPostBack;
        FxEnum.AjaxExtStat ajaxES = this.AjaxExtensionStatus;

        // 待機する（二重送信確認用）
        System.Threading.Thread.Sleep(3000);

        // テキストボックスの値を変更
        TextBox textBox = (TextBox)this.GetContentWebControl("TextBox4");
        textBox.Text = "通常のポストバック（ＤＤＬのセレクト インデックス チェンジ）";

        return "";
    }

    /// <summary>btnButton3のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton3_Click(FxEventArgs fxEventArgs)
    {
        // Ajaxを制御する場合は、ScriptManagerを使用する。
        // このクラスを使用すると、Ajax中であるかどうかを判別できる。
        bool isInAsyncPostBack = this.CurrentScriptManager.IsInAsyncPostBack;
        FxEnum.AjaxExtStat ajaxES = this.AjaxExtensionStatus;

        // 待機する（UpdateProgress、二重送信確認用）
        System.Threading.Thread.Sleep(3000);

        throw new Exception("Ajaxでエラー");

        //return "";
    }

    #endregion
}

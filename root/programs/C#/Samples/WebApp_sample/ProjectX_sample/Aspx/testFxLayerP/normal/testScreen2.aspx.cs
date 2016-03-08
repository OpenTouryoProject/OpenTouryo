//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testFxLayerP_normal_testScreen2
//* クラス日本語名  ：テスト画面２（Ｐ層）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*  2015/04/17  Supragyan         Created Textbox Textchanged event
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

/// <summary>テスト画面２（Ｐ層）</summary>
public partial class Aspx_testFxLayerP_normal_testScreen2 : MyBaseController
{
    #region ページロードのUOCメソッド

    /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit()
    {
        // フォーム初期化（初回ロード）時に実行する処理を実装する
        // TODO:
        Response.Write(this.ContentPageFileNoEx + "<br/>");

        // クライアントからの業務モーダル画面起動
        // スタイル指定なし
        this.btnButton2.OnClientClick =
            "return " + this.GetScriptToShowModalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx") + ";";
        this.btnButton3.OnClientClick =
            "return " + this.GetScriptToShowModalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx?test=test") + ";";

        // スタイル指定あり（空）
        this.btnButton4.OnClientClick =
            "return " + this.GetScriptToShowModalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx", "") + ";";
        this.btnButton5.OnClientClick =
            "return " + this.GetScriptToShowModalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx?test=test", "") + ";";

        // ---

        // クライアントからの業務モードレス画面起動
        this.btnButton9.OnClientClick =
            this.GetScriptToShowNormalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx")
            + "; return false;";

        this.btnButton10.OnClientClick =
            this.GetScriptToShowNormalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx?test=test", "")
            + "; return false;";

        this.btnButton11.OnClientClick =
            this.GetScriptToShowNormalScreen("~/Aspx/testFxLayerP/normal/testScreen1.aspx?test=test", "", "t")
            + "; return false;";
    }

    /// <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit_PostBack()
    {
        // フォーム初期化（ポストバック）時に実行する処理を実装する
        // TODO:
    }

    #endregion

    #region 外部パラメータ（アイコン）

    #region コンテンツ ページ上のフレームワーク対象コントロール

    /// <summary>
    /// btnButton1のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowYesNoMessageDialog(
            "メッセージＩＤ",
            "メッセージ",
            "テスト結果");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// lbnLinkButton1のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_lbnLinkButton1_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "メッセージＩＤ",
            "メッセージ",
            FxEnum.IconType.Information, "テスト結果（情報）");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// ibnImageButton1のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ibnImageButton1_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "メッセージＩＤ",
            "メッセージ",
            FxEnum.IconType.Exclamation, "テスト結果（警告）");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// impImageMap1のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_impImageMap1_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "メッセージＩＤ",
            "メッセージ",
            FxEnum.IconType.StopMark, "テスト結果（エラー）");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #endregion

    #region イベント追加

    #region DropDownList

    #region マスタ ページ上のフレームワーク対象コントロール

    /// <summary>
    /// ddlMDropDownList1のSelectedIndexChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen2_ddlMDropDownList1_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "ddlMDropDownList1のSelectedIndexChangedイベント",
             ((DropDownList)this.GetFxWebControl("ddlMDropDownList1")).SelectedValue,
            FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #region コンテンツ ページ上のフレームワーク対象コントロール

    /// <summary>
    /// ddlDropDownList1のSelectedIndexChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ddlDropDownList1_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "ddlDropDownList1のSelectedIndexChangedイベント",
             ((DropDownList)this.GetFxWebControl("ddlDropDownList1")).SelectedValue,
            FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #endregion

    #region ListBox

    #region マスタ ページ上のフレームワーク対象コントロール

    /// <summary>
    /// lbxMListBox1のSelectedIndexChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen2_lbxMListBox1_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "lbxMListBox1のSelectedIndexChangedイベント",
             ((ListBox)this.GetFxWebControl("lbxMListBox1")).SelectedValue,
            FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #region コンテンツ ページ上のフレームワーク対象コントロール

    /// <summary>
    /// lbxListBox1のSelectedIndexChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_lbxListBox1_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "lbxListBox1のSelectedIndexChangedイベント",
             ((ListBox)this.GetFxWebControl("lbxListBox1")).SelectedValue,
            FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #endregion

    #region RadioButton

    #region マスタ ページ上のフレームワーク対象コントロール

    /// <summary>
    /// rbnMRadioButton1のCheckedChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen2_rbnMRadioButton1_CheckedChanged(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "rbnMRadioButton1のCheckedChangedイベント",
             ((RadioButton)(this.GetFxWebControl("rbnMRadioButton1"))).Checked.ToString(),
             FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// rbnMRadioButton2のCheckedChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen2_rbnMRadioButton2_CheckedChanged(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "rbnMRadioButton2のCheckedChangedイベント",
             ((RadioButton)(this.GetFxWebControl("rbnMRadioButton2"))).Checked.ToString(),
             FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #region コンテンツ ページ上のフレームワーク対象コントロール

    /// <summary>
    /// rbnRadioButton1のCheckedChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_rbnRadioButton1_CheckedChanged(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "rbnRadioButton1のCheckedChangedイベント",
             ((RadioButton)(this.GetFxWebControl("rbnRadioButton1"))).Checked.ToString(),
             FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// rbnRadioButton2のCheckedChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_rbnRadioButton2_CheckedChanged(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "rbnRadioButton2のCheckedChangedイベント",
             ((RadioButton)(this.GetFxWebControl("rbnRadioButton2"))).Checked.ToString(),
             FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #endregion

    #region RadioButtonList
    /// <summary>
    /// rblRadioButtonList1_SelectedIndexChanged
    /// </summary>
    /// <param name="fxEventArgs"></param>
    /// <returns>URL</returns>
    protected string UOC_rblRadioButtonList1_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        // ShowOKMessageDialog for radiobuttonlist
        this.ShowOKMessageDialog(
             "rblRadioButtonList1のSelectedIndexChangedイベント",
             ((RadioButtonList)this.GetFxWebControl("rblRadioButtonList1")).SelectedValue,
            FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }
    #endregion

    #region CheckBox

    #region マスタ ページ上のフレームワーク対象コントロール

    /// <summary>
    /// cbxMCheckBox1のCheckedChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen2_cbxMCheckBox1_CheckedChanged(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "cbxMCheckBox1のCheckedChangedイベント",
             ((CheckBox)(this.GetFxWebControl("cbxMCheckBox1"))).Checked.ToString(),
             FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// cbxMCheckBox2のCheckedChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen2_cbxMCheckBox2_CheckedChanged(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "cbxMCheckBox2のCheckedChangedイベント",
             ((CheckBox)(this.GetFxWebControl("cbxMCheckBox2"))).Checked.ToString(),
             FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #region コンテンツ ページ上のフレームワーク対象コントロール

    /// <summary>
    /// cbxCheckBox1のCheckedChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_cbxCheckBox1_CheckedChanged(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "cbxCheckBox1のCheckedChangedイベント",
             ((CheckBox)(this.GetFxWebControl("cbxCheckBox1"))).Checked.ToString(),
             FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// cbxCheckBox2のCheckedChangedイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_cbxCheckBox2_CheckedChanged(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
             "cbxCheckBox2のCheckedChangedイベント",
             ((CheckBox)(this.GetFxWebControl("cbxCheckBox2"))).Checked.ToString(),
             FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #endregion

    #region CheckboxList
    /// <summary>
    /// UOC_cblCheckBoxList1_SelectedIndexChanged
    /// </summary>
    /// <param name="fxEventArgs"></param>
    /// <returns>URL</returns>
    protected string UOC_cblCheckBoxList1_SelectedIndexChanged(FxEventArgs fxEventArgs)
    {
        //  ShowOKMessageDialog for checkboxlist
        this.ShowOKMessageDialog(
             "cblCheckBoxList1のSelectedIndexChangedイベント",
             ((CheckBoxList)this.GetFxWebControl("cblCheckBoxList1")).SelectedValue,
            FxEnum.IconType.Information, "ＧＪ！");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }
    #endregion

    #region モーダル ダイアログのＩ / Ｆ

    /// <summary>
    /// UOC_btnButton6のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton6_Click(FxEventArgs fxEventArgs)
    {
        // 親画面別セッション領域 - 設定
        this.SetDataToModalInterface("msg", this.TextBox1.Text);
        return "";
    }

    /// <summary>
    /// UOC_btnButton7のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton7_Click(FxEventArgs fxEventArgs)
    {
        // 親画面別セッション領域 - 取得

        // メッセージ表示
        this.ShowOKMessageDialog(
            "親画面別セッション（キー：msg）は、",
            (string)this.GetDataFromModalInterface("msg"),
            FxEnum.IconType.Information, "テスト結果");

        return "";
    }

    /// <summary>
    /// UOC_btnButton8のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton8_Click(FxEventArgs fxEventArgs)
    {
        // 親画面別セッション領域 - キー：msgのみ削除
        this.DeleteDataFromModalInterface("msg");
        return "";
    }

    /// <summary>
    /// UOC_txtTextBox2のテキスト変更イベント
    /// </summary>
    /// <param name="fxEventArgs"></param>
    protected void UOC_txtTextBox2_TextChanged(FxEventArgs fxEventArgs)
    {
        this.ShowOKMessageDialog(
            "親画面別セッション（キー：msg）は、",
            "You changed text to" + " " + txtTextBox2.Text,
            FxEnum.IconType.Information, "テスト結果");
    }

    #endregion

    #endregion

    #region 後処理のUOCメソッド

    /// <summary>「YES」・「NO」メッセージ・ダイアログの「×」が押され閉じられた場合の処理を実装する。</summary>
    /// <param name="parentFxEventArgs">「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴</param>
    protected override void UOC_YesNoDialog_X_Click(FxEventArgs parentFxEventArgs)
    {
        // 「YES」・「NO」メッセージ・ダイアログの「×」が押され閉じられた場合の処理を実装
        // TODO:

        // switch文

        // メッセージ表示
        this.ShowOKMessageDialog(
            parentFxEventArgs.ButtonID + "で開いた「YES」・「NO」メッセージ・ダイアログ",
            "[×]ボタンを押した時の後処理",
            FxEnum.IconType.Information, "テスト結果");
    }

    /// <summary>「YES」・「NO」メッセージ・ダイアログの「YES」が押され閉じられた場合の処理を実装する。</summary>
    /// <param name="parentFxEventArgs">「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴</param>
    protected override void UOC_YesNoDialog_Yes_Click(FxEventArgs parentFxEventArgs)
    {
        // 「YES」・「NO」メッセージ・ダイアログの「YES」が押され閉じられた場合の処理を実装
        // TODO:

        // switch文

        // メッセージ表示
        this.ShowOKMessageDialog(
            parentFxEventArgs.ButtonID + "で開いた「YES」・「NO」メッセージ・ダイアログ",
            "[Yes]ボタンを押した時の後処理",
            FxEnum.IconType.Information, "テスト結果");
    }

    /// <summary>「YES」・「NO」メッセージ・ダイアログの「NO」が押され閉じられた場合の処理を実装する。</summary>
    /// <param name="parentFxEventArgs">「YES」・「NO」メッセージ・ダイアログを開いた（親画面側の）ボタンのボタン履歴</param>
    protected override void UOC_YesNoDialog_No_Click(FxEventArgs parentFxEventArgs)
    {
        // 「YES」・「NO」メッセージ・ダイアログの「NO」が押され閉じられた場合の処理を実装
        // TODO:

        // switch文

        // メッセージ表示
        this.ShowOKMessageDialog(
            parentFxEventArgs.ButtonID + "で開いた「YES」・「NO」メッセージ・ダイアログ",
            "[No]ボタンを押した時の後処理",
            FxEnum.IconType.Information, "テスト結果");
    }

    /// <summary>業務モーダル画面の後処理を実装する。</summary>
    /// <param name="parentFxEventArgs">業務モーダル画面を開いた（親画面側の）ボタンのボタン履歴</param>
    /// <param name="childFxEventArgs">業務モーダル画面を閉じた（若しくは一番最後に押された子画面側の）ボタンのボタン履歴</param>
    protected override void UOC_ModalDialog_End(FxEventArgs parentFxEventArgs, FxEventArgs childFxEventArgs)
    {
        // 業務モーダル画面の後処理を実装
        // TODO:

        // switch文

        // メッセージ表示
        this.ShowOKMessageDialog(
            parentFxEventArgs.ButtonID + "で開いた業務モーダル・ダイアログの",
            childFxEventArgs.ButtonID + "ボタンを押して閉じた時の後処理",
            FxEnum.IconType.Information, "テスト結果");
    }

    #endregion

}

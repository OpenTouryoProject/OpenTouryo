//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testFxLayerP_normal_testScreen1nest
//* クラス日本語名  ：テスト画面１のネスト版（Ｐ層）
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

/// <summary>テスト画面１のネスト版（Ｐ層）</summary>
public partial class Aspx_testFxLayerP_normal_testScreen1nest : MyBaseController
{
    #region ページロードのUOCメソッド

    /// <summary>ページロードのUOCメソッド（個別：初回ロード）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit()
    {
        // フォーム初期化（初回ロード）時に実行する処理を実装する
        // TODO:
        Response.Write(this.ContentPageFileNoEx + "<br/>");

        // QueryStringの通知
        string qs = "";
        foreach (string qsKey in Request.QueryString.AllKeys)
        {
            qs += qsKey + "=" + Request.QueryString[qsKey] + ";";
        }
        Response.Write(qs);
    }

    /// <summary>ページロードのUOCメソッド（個別：ポストバック）</summary>
    /// <remarks>実装必須</remarks>
    protected override void UOC_FormInit_PostBack()
    {
        // フォーム初期化（ポストバック）時に実行する処理を実装する
        // TODO:

    }

    #endregion

    #region マスタ ページ上のフレームワーク対象コントロール

    #region 既存のルートのマスタ ページ上のフレームワーク対象コントロール

    #region 基本処理

    /// <summary>
    /// btnMButton21のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_btnMButton21_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行",
            FxEnum.IconType.Information, "テスト結果");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// lbnMLinkButton21のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_lbnMLinkButton21_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行",
            FxEnum.IconType.Information, "テスト結果");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// ibnMImageButton21のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_ibnMImageButton21_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行 - " +
            "x:" + fxEventArgs.X.ToString() +
            ",y:" + fxEventArgs.Y.ToString(),
            FxEnum.IconType.Information, "テスト結果");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// impMImageMap21のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_impMImageMap21_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行 - " + 
            "pbv:" + fxEventArgs.PostBackValue,
            FxEnum.IconType.Information, "テスト結果");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #region 画面遷移処理

    /// <summary>
    /// btnMButton22のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_btnMButton22_Click(FxEventArgs fxEventArgs)
    {
        return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx";
    }

    /// <summary>
    /// lbnMLinkButton22のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_lbnMLinkButton22_Click(FxEventArgs fxEventArgs)
    {
        return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx";
    }

    /// <summary>
    /// ibnMImageButton22のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_ibnMImageButton22_Click(FxEventArgs fxEventArgs)
    {
        return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx";
    }

    /// <summary>
    /// impMImageMap2のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_impMImageMap22_Click(FxEventArgs fxEventArgs)
    {
        return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx";
    }

    #endregion

    #region コントロール取得

    /// <summary>
    /// btnMButton23のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_btnMButton23_Click(FxEventArgs fxEventArgs)
    {
        // コントロールを取得し
        Control temp = (Control)this.GetFxWebControl(((TextBox)this.GetMasterWebControl("TextBox4")).Text);

        if (temp == null)
        {
            // 取得できなかった

            // メッセージ表示
            this.ShowOKMessageDialog(
                "GetFxWebControl",
                "コントロールを取得できませんでした。",
                FxEnum.IconType.Information, "テスト結果");
        }
        else
        {
            // 取得できた

            // 消したり出したり
            if (temp.Visible == true)
            {
                temp.Visible = false;
            }
            else
            {
                temp.Visible = true;
            }
        }

        return "";
    }

    /// <summary>
    /// lbnMLinkButton23のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_lbnMLinkButton23_Click(FxEventArgs fxEventArgs)
    {
        // コントロールを取得し
        Control temp = (Control)this.GetMasterWebControl(((TextBox)this.GetMasterWebControl("TextBox4")).Text);

        if (temp == null)
        {
            // 取得できなかった

            // メッセージ表示
            this.ShowOKMessageDialog(
                "GetMasterWebControl",
                "コントロールを取得できませんでした。",
                FxEnum.IconType.Information, "テスト結果");
        }
        else
        {
            // 取得できた

            // 消したり出したり
            if (temp.Visible == true)
            {
                temp.Visible = false;
            }
            else
            {
                temp.Visible = true;
            }
        }

        return "";
    }

    /// <summary>
    /// ibnMImageButton23のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_ibnMImageButton23_Click(FxEventArgs fxEventArgs)
    {
        // コントロールを取得し
        Control temp = (Control)this.GetContentWebControl(((TextBox)this.GetMasterWebControl("TextBox4")).Text);

        if (temp == null)
        {
            // 取得できなかった

            // メッセージ表示
            this.ShowOKMessageDialog(
                "GetContentWebControl",
                "コントロールを取得できませんでした。",
                FxEnum.IconType.Information, "テスト結果");
        }
        else
        {
            // 取得できた

            // 消したり出したり
            if (temp.Visible == true)
            {
                temp.Visible = false;
            }
            else
            {
                temp.Visible = true;
            }
        }

        return "";
    }

    #endregion

    #region ダイアログ表示

    /// <summary>
    /// btnMButton24のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_btnMButton24_Click(FxEventArgs fxEventArgs)
    {
        // スタイルを取得
        string style = ((TextBox)this.GetMasterWebControl("TextBox5")).Text;

        // 受け渡しデータの設定
        string msg = ((TextBox)this.GetMasterWebControl("TextBox6")).Text;

        if (((CheckBox)this.GetMasterWebControl("CheckBox2")).Checked == true)
        {
            // スタイル指定あり
            this.ShowOKMessageDialog(
                "メッセージＩＤ", "メッセージ：" + msg,
                FxEnum.IconType.Information, "テスト", style);
        }
        else
        {
            // スタイル指定なし
            this.ShowOKMessageDialog(
                "メッセージＩＤ", "メッセージ：" + msg,
                FxEnum.IconType.Information, "テスト");
        }

        return "";
    }

    /// <summary>
    /// lbnMLinkButton24のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_lbnMLinkButton24_Click(FxEventArgs fxEventArgs)
    {
        // スタイルを取得
        string style = ((TextBox)this.GetMasterWebControl("TextBox5")).Text;

        // 受け渡しデータの設定
        string msg = ((TextBox)this.GetMasterWebControl("TextBox6")).Text;

        if (((CheckBox)this.GetMasterWebControl("CheckBox2")).Checked == true)
        {
            // スタイル指定あり
            this.ShowYesNoMessageDialog(
                "メッセージＩＤ", "メッセージ：" + msg,
                "ダイアログ表示テスト",
                style);
        }
        else
        {
            // スタイル指定なし
            this.ShowYesNoMessageDialog(
                "メッセージＩＤ", "メッセージ：" + msg,
                "ダイアログ表示テスト");
        }

        return "";
    }

    /// <summary>
    /// ibnMImageButton24のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_ibnMImageButton24_Click(FxEventArgs fxEventArgs)
    {
        // スタイルを取得
        string style = ((TextBox)this.GetMasterWebControl("TextBox5")).Text;

        // ネストしていた場合、連結

        // ModalInterface（Session）からデータを取得
        string msg = (string)this.GetDataFromModalInterface("msg");

        // 受け渡しデータの設定
        msg += "," + ((TextBox)this.GetMasterWebControl("TextBox6")).Text;

        // ModalInterface（Session）からデータを取得
        this.SetDataToModalInterface("msg", msg);

        if (((CheckBox)this.GetMasterWebControl("CheckBox2")).Checked == true)
        {
            // スタイル指定あり
            // 注意：ここだけDialogLoader.htmからの相対パス or 仮想パスを指定する。
            this.ShowModalScreen("~/Aspx/testFxLayerP/nest/testScreen1nest.aspx", style);
        }
        else
        {
            // スタイル指定なし
            // 注意：ここだけDialogLoader.htmからの相対パス or 仮想パスを指定する。
            this.ShowModalScreen("~/Aspx/testFxLayerP/nest/testScreen1nest.aspx");
        }

        return "";
    }

    /// <summary>
    /// impMImageMap24のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_TestScreen1_impMImageMap24_Click(FxEventArgs fxEventArgs)
    {
        // スタイルを取得
        string style = ((TextBox)this.GetMasterWebControl("TextBox5")).Text;

        if (((CheckBox)this.GetMasterWebControl("CheckBox2")).Checked == true)
        {
            // スタイル指定あり
            this.ShowNormalScreen("~/Aspx/testFxLayerP/nest/testScreen1nest.aspx", style);
        }
        else
        {
            // スタイル指定なし
            this.ShowNormalScreen("~/Aspx/testFxLayerP/nest/testScreen1nest.aspx");
        }

        return "";
    }

    #endregion

    #endregion

    #region 追加のブランチのマスタ ページ上のフレームワーク対象コントロール

    #region testScreen1bmp1.master上

    /// <summary>
    /// btnCPF_Aのクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_testScreen1bmp1_btnCPF_A_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行",
            FxEnum.IconType.Information, "テスト結果");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// btnCPF_Bのクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_testScreen1bmp1_btnCPF_B_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行",
            FxEnum.IconType.Information, "テスト結果");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// btnCPF_Cのクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_testScreen1bmp1_btnCPF_C_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行",
            FxEnum.IconType.Information, "テスト結果");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #region testScreen1bmp2.master上

    /// <summary>
    /// btnCPF_A1のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_testScreen1bmp2_btnCPF_A1_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行",
            FxEnum.IconType.Information, "テスト結果");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// btnCPF_B1のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_testScreen1bmp2_btnCPF_B1_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行",
            FxEnum.IconType.Information, "テスト結果");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    /// <summary>
    /// btnCPF_C1のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_testScreen1bmp2_btnCPF_C1_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行",
            FxEnum.IconType.Information, "テスト結果");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #endregion

    #endregion

    #region コンテンツ ページ上のフレームワーク対象コントロール

    #region コンテンツ ページ１

    #region 基本処理

    /// <summary>
    /// btnButton1のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
    {
        // メッセージ表示
        this.ShowOKMessageDialog(
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行",
            FxEnum.IconType.Information, "テスト結果");

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
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行",
            FxEnum.IconType.Information, "テスト結果");

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
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行 - " +
            "x:" + fxEventArgs.X.ToString() +
            ",y:" + fxEventArgs.Y.ToString(),
            FxEnum.IconType.Information, "テスト結果");

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
            fxEventArgs.ButtonID + "クリック イベント",
            fxEventArgs.MethodName + "の実行 - " +
            "pbv:" + fxEventArgs.PostBackValue,
            FxEnum.IconType.Information, "テスト結果");

        // 画面遷移しないポストバックの場合は、urlを空文字列に設定する
        return "";
    }

    #endregion

    #region 画面遷移処理

    /// <summary>
    /// btnButton2のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton2_Click(FxEventArgs fxEventArgs)
    {
        return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx";
    }

    /// <summary>
    /// lbnLinkButton2のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_lbnLinkButton2_Click(FxEventArgs fxEventArgs)
    {
        return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx";
    }

    /// <summary>
    /// ibnImageButton2のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ibnImageButton2_Click(FxEventArgs fxEventArgs)
    {
        return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx";
    }

    /// <summary>
    /// impImageMap2のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_impImageMap2_Click(FxEventArgs fxEventArgs)
    {
        return "~/Aspx/testFxLayerP/testTransitionAheadScreen.aspx";
    }

    #endregion

    #region コントロール取得
    
    /// <summary>
    /// btnButton3のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton3_Click(FxEventArgs fxEventArgs)
    {
        // コントロールを取得し
        Control temp = (Control)this.GetFxWebControl(this.TextBox1.Text);

        if (temp == null)
        {
            // 取得できなかった

            // メッセージ表示
            this.ShowOKMessageDialog(
                "GetFxWebControl",
                "コントロールを取得できませんでした。",
                FxEnum.IconType.Information, "テスト結果");
        }
        else
        {
            // 取得できた

            // 消したり出したり
            if (temp.Visible == true)
            {
                temp.Visible = false;
            }
            else
            {
                temp.Visible = true;
            }
        }
        
        return "";
    }

    /// <summary>
    /// lbnLinkButton3のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_lbnLinkButton3_Click(FxEventArgs fxEventArgs)
    {
        // コントロールを取得し
        Control temp = (Control)this.GetMasterWebControl(this.TextBox1.Text);

        if (temp == null)
        {
            // 取得できなかった

            // メッセージ表示
            this.ShowOKMessageDialog(
                "GetMasterWebControl",
                "コントロールを取得できませんでした。",
                FxEnum.IconType.Information, "テスト結果");
        }
        else
        {
            // 取得できた

            // 消したり出したり
            if (temp.Visible == true)
            {
                temp.Visible = false;
            }
            else
            {
                temp.Visible = true;
            }
        }

        return "";
    }

    /// <summary>
    /// ibnImageButton3のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ibnImageButton3_Click(FxEventArgs fxEventArgs)
    {
        // コントロールを取得し
        Control temp = (Control)this.GetContentWebControl(this.TextBox1.Text);

        if (temp == null)
        { 
            // 取得できなかった

            // メッセージ表示
            this.ShowOKMessageDialog(
                "GetContentWebControl",
                "コントロールを取得できませんでした。",
                FxEnum.IconType.Information, "テスト結果");
        }
        else
        {
            // 取得できた

            // 消したり出したり
            if (temp.Visible == true)
            {
                temp.Visible = false;
            }
            else
            {
                temp.Visible = true;
            }
        }

        return "";
    }

    #endregion

    #region ダイアログ表示

    /// <summary>
    /// btnButton4のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton4_Click(FxEventArgs fxEventArgs)
    {
        // スタイルを取得
        string style = this.TextBox2.Text;

        // 受け渡しデータの設定
        string msg = this.TextBox3.Text;

        if (this.CheckBox1.Checked == true)
        {
            // スタイル指定あり
            this.ShowOKMessageDialog(
                "メッセージＩＤ", "メッセージ：" + msg,
                FxEnum.IconType.Information, "テスト", style);
        }
        else
        {
            // スタイル指定なし
            this.ShowOKMessageDialog(
                "メッセージＩＤ", "メッセージ：" + msg,
                FxEnum.IconType.Information, "テスト");
        }

        return "";
    }

    /// <summary>
    /// lbnLinkButton4のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_lbnLinkButton4_Click(FxEventArgs fxEventArgs)
    {
        // スタイルを取得
        string style = this.TextBox2.Text;

        // 受け渡しデータの設定
        string msg = this.TextBox3.Text;

        if (this.CheckBox1.Checked == true)
        {
            // スタイル指定あり
            this.ShowYesNoMessageDialog(
                "メッセージＩＤ", "メッセージ：" + msg,
                "ダイアログ表示テスト",
                style);
        }
        else
        {
            // スタイル指定なし
            this.ShowYesNoMessageDialog(
                "メッセージＩＤ", "メッセージ：" + msg,
                "ダイアログ表示テスト");
        }

        return "";
    }

    /// <summary>
    /// ibnImageButton4のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ibnImageButton4_Click(FxEventArgs fxEventArgs)
    {
        // ネストしていた場合、連結

        // ModalInterface（Session）からデータを取得
        string msg = (string)this.GetDataFromModalInterface("msg");

        // 受け渡しデータの設定
        if (this.TextBox3.Text != "")
        {
            msg += "," + this.TextBox3.Text;
        }        

        // ModalInterface（Session）にデータを設定
        this.SetDataToModalInterface("msg", msg);

        // スタイルを取得
        string style = this.TextBox2.Text;

        if (this.CheckBox1.Checked == true)
        {
            // スタイル指定あり
            // 注意：ここだけDialogLoader.htmからの相対パス or 仮想パスを指定する。
            this.ShowModalScreen("~/Aspx/testFxLayerP/nest/testScreen1nest.aspx?test=test", style);
            // ※ QueryString指定あり
        }
        else
        {
            // スタイル指定なし
            // 注意：ここだけDialogLoader.htmからの相対パス or 仮想パスを指定する。
            this.ShowModalScreen("~/Aspx/testFxLayerP/nest/testScreen1nest.aspx?test=test");
            // ※ QueryString指定あり
        }

        return "";
    }

    /// <summary>
    /// impImageMap4のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_impImageMap4_Click(FxEventArgs fxEventArgs)
    {
        // スタイルを取得
        string style = this.TextBox2.Text;
        // ターゲットを取得
        string target = this.TextBox2a.Text;


        if (this.CheckBox1.Checked == true && this.CheckBox1a.Checked == true)
        {
            // スタイル指定あり
            this.ShowNormalScreen("~/Aspx/testFxLayerP/nest/testScreen1nest.aspx?test=test", style, target);
            // ※ QueryString指定あり
        }
        else if (this.CheckBox1.Checked == true)
        {
            // スタイル指定あり
            this.ShowNormalScreen("~/Aspx/testFxLayerP/nest/testScreen1nest.aspx?test=test", style);
            // ※ QueryString指定あり
        }
        else if (this.CheckBox1a.Checked == true)
        {
            // スタイル指定あり
            this.ShowNormalScreen("~/Aspx/testFxLayerP/nest/testScreen1nest.aspx?test=test", "", target);
            // ※ QueryString指定あり
        }
        else
        {
            // スタイル指定なし
            this.ShowNormalScreen("~/Aspx/testFxLayerP/nest/testScreen1nest.aspx?test=test");
            // ※ QueryString指定あり
        }

        return "";
    }

    #endregion

    #endregion

    #region コンテンツ ページ２

    #region モーダルダイアログのインターフェイス

    /// <summary>
    /// btnButton21のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton21_Click(FxEventArgs fxEventArgs)
    {
        // 親画面別セッション領域 - 設定
        this.SetDataToModalInterface("msg", this.TextBox4.Text);
        return "";
    }

    /// <summary>
    /// lbnLinkButton21のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_lbnLinkButton21_Click(FxEventArgs fxEventArgs)
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
    /// ibnImageButton21のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ibnImageButton21_Click(FxEventArgs fxEventArgs)
    {
        // 親画面別セッション領域 - キー：msgのみ削除
        this.DeleteDataFromModalInterface("msg");
        return "";
    }

    /// <summary>
    /// impImageMap21のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_impImageMap21_Click(FxEventArgs fxEventArgs)
    {
        // 親画面別セッション領域 - 全て削除
        this.DeleteDataFromModalInterface();
        return "";
    }

    #endregion

    #region 自画面を閉じる

    /// <summary>
    /// btnButton22のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton22_Click(FxEventArgs fxEventArgs)
    {
        // 自画面を閉じる
        this.CloseModalScreen();
        return "";
    }

    /// <summary>
    /// lbnLinkButton22のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_lbnLinkButton22_Click(FxEventArgs fxEventArgs)
    {
        // 自画面を閉じる
        this.CloseModalScreen_NoPostback();
        return "";
    }

    /// <summary>
    /// ImageButton22のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ibnImageButton22_Click(FxEventArgs fxEventArgs)
    {
        // 自画面を閉じる
        this.CloseModalScreen_WithAllParent();
        return "";
    }

    /// <summary>
    /// ImageMap22のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_impImageMap22_Click(FxEventArgs fxEventArgs)
    {
        // 自画面を閉じる
        this.CloseModalScreen();
        return "";
    }

    #endregion

    #region ２重送信防止テスト

    /// <summary>
    /// btnButton23のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton23_Click(FxEventArgs fxEventArgs)
    {
        // ２重送信防止テスト
        System.Threading.Thread.Sleep(2000);

        // 確認用のカウンタ
        if (Session["cnt"] == null)
        {
            Session["cnt"] = 1;
        }
        else
        {
            Session["cnt"] =((int)Session["cnt"]) + 1;
        }

        Response.Write(Session["cnt"].ToString());

        return "";
    }

    /// <summary>
    /// lbnLinkButton23のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_lbnLinkButton23_Click(FxEventArgs fxEventArgs)
    {
        // ２重送信防止テスト
        System.Threading.Thread.Sleep(2000);

        // 確認用のカウンタ
        if (Session["cnt"] == null)
        {
            Session["cnt"] = 1;
        }
        else
        {
            Session["cnt"] = ((int)Session["cnt"]) + 1;
        }

        Response.Write(Session["cnt"].ToString());

        return "";
    }

    /// <summary>
    /// ImageButton23のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ibnImageButton23_Click(FxEventArgs fxEventArgs)
    {
        // ２重送信防止テスト
        System.Threading.Thread.Sleep(2000);

        // 確認用のカウンタ
        if (Session["cnt"] == null)
        {
            Session["cnt"] = 1;
        }
        else
        {
            Session["cnt"] = ((int)Session["cnt"]) + 1;
        }

        Response.Write(Session["cnt"].ToString());

        return "";
    }

    /// <summary>
    /// ImageMap23のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_impImageMap23_Click(FxEventArgs fxEventArgs)
    {
        // ２重送信防止テスト
        System.Threading.Thread.Sleep(2000);

        // 確認用のカウンタ
        if (Session["cnt"] == null)
        {
            Session["cnt"] = 1;
        }
        else
        {
            Session["cnt"] = ((int)Session["cnt"]) + 1;
        }

        Response.Write(Session["cnt"].ToString());

        return "";
    }

    #endregion

    #region エラーを起こす

    /// <summary>
    /// btnButton24のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton24_Click(FxEventArgs fxEventArgs)
    {
        // その他、一般的な例外
        throw new Exception(fxEventArgs.MethodName + "で、Exceptionをスロー。");

        //return "";
    }

    /// <summary>
    /// lbnLinkButton24のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_lbnLinkButton24_Click(FxEventArgs fxEventArgs)
    {
        // システム例外
        throw new BusinessSystemException(
            "xxxxx", fxEventArgs.MethodName + "で、BusinessSystemExceptionをスロー。");

        //return "";
    }

    /// <summary>
    /// ImageButton24のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ibnImageButton24_Click(FxEventArgs fxEventArgs)
    {
        // 業務例外
        throw new BusinessApplicationException(
            "xxxxx", fxEventArgs.MethodName + "で、BusinessApplicationExceptionをスロー。",
            "エラー情報はここでは無視される。");

        //return "";
    }

    #endregion

    #endregion

    #region コンテンツ ページ３

    #region 自画面に画面遷移

    /// <summary>
    /// btnButton31のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton31_Click(FxEventArgs fxEventArgs)
    {
        // ウィンドウ別セッション領域 - 設定
        this.SetDataToBrowserWindow("msg", this.TextBox5.Text);

        return "";
    }

    /// <summary>
    /// lbnLinkButton31のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_lbnLinkButton31_Click(FxEventArgs fxEventArgs)
    {
        // ウィンドウ別セッション領域 - 取得

        // メッセージ表示
        this.ShowOKMessageDialog(
            "ウィンドウ別セッション（キー：msg）は、",
            (string)this.GetDataFromBrowserWindow("msg"),
            FxEnum.IconType.Information, "テスト結果"); 

        return "";
    }

    /// <summary>
    /// ibnImageButton31のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ibnImageButton31_Click(FxEventArgs fxEventArgs)
    {
        // 次画面（自画面）に画面遷移
        return "~/Aspx/testFxLayerP/nest/testScreen1nest.aspx";
    }

    /// <summary>
    /// impImageMap31のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_impImageMap31_Click(FxEventArgs fxEventArgs)
    {
        // ウィンドウ別セッション領域 - 削除
        switch (fxEventArgs.PostBackValue)
        {
            case "spot1":
                // キー：msgのみ削除
                this.DeleteDataFromBrowserWindow("msg");
                break;
            case "spot2":
                // 全て削除
                this.DeleteDataFromBrowserWindow();
                break;
            case "spot3":
                // 全て削除
                this.DeleteDataFromBrowserWindow();
                break;
        }
        
        return "";
    }

    #endregion

    #region オンロードで子画面表示

    /// <summary>
    /// btnButton32のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton32_Click(FxEventArgs fxEventArgs)
    {
        Session["DialogAtOnLoad"] = "ok";
        return "~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx";
    }

    /// <summary>
    /// lbnLinkButton32のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_lbnLinkButton32_Click(FxEventArgs fxEventArgs)
    {
        Session["DialogAtOnLoad"] = "yesno";
        return "~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx";
    }

    /// <summary>
    /// ibnImageButton32のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ibnImageButton32_Click(FxEventArgs fxEventArgs)
    {
        Session["DialogAtOnLoad"] = "modal";
        return "~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx";
    }

    /// <summary>
    /// impImageMap32のクリックイベント
    /// </summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_impImageMap32_Click(FxEventArgs fxEventArgs)
    {
        Session["DialogAtOnLoad"] = "modaless";
        return "~/Aspx/testFxLayerP/testDialogAtOnLoad.aspx";
    }

    #endregion

    #region ファイルのダウンロード

    /// <summary>btnButton33のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton33_Click(FxEventArgs fxEventArgs)
    {
        Response.Clear();
        
        Response.ContentType = "application/pdf";

        //attachmentで開く場合は、キャッシュを無効にしないこと。
        Response.CacheControl = "private";

        //こっちは、専用アプリケーションで開く
        Response.AppendHeader("Content-Disposition", "attachment;filename=test.pdf");

        Response.WriteFile(
            Path.Combine(
                GetConfigParameter.GetConfigValue("TestFilePath"), "test.pdf"));
        
        Response.End(); 

        return "";
    }

    /// <summary>lbnLinkButton33のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_lbnLinkButton33_Click(FxEventArgs fxEventArgs)
    {
        Response.Clear();

        Response.ContentType = "application/pdf";

        //attachmentで開く場合は、キャッシュを無効にしないこと。
        Response.CacheControl = "private";

        //こっちは、IEからOLEオブジェクトを開く
        Response.AppendHeader("Content-Disposition", "inline;filename=test.pdf");

        Response.WriteFile(
            Path.Combine(
                GetConfigParameter.GetConfigValue("TestFilePath"), "test.pdf"));

        Response.End();

        return "";        
    }

    /// <summary>ibnImageButton33のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_ibnImageButton33_Click(FxEventArgs fxEventArgs)
    {
        this.ShowModalScreen("~/Aspx/testFxLayerP/testDLScreen.aspx");
        return "";
    }

    /// <summary>ibnImageButton33のクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_impImageMap33_Click(FxEventArgs fxEventArgs)
    {
        //this.ShowNormalScreen("~/Aspx/testFxLayerP/testDLScreen.aspx");
        this.ShowNormalScreen("~/Aspx/testFxLayerP/testDLFrame.aspx");
        return "";
    }
    
    #endregion

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

//**********************************************************************************
//* フレームワーク・テスト画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_testFxLayerP_normal_testNestMasterScreen
//* クラス日本語名  ：マスタページのテスト画面（Ｐ層）
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

/// <summary>テスト画面ネスト（Ｐ層）</summary>
public partial class Aspx_testFxLayerP_normal_testNestMasterScreen : MyBaseController
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

    #region 共通処理

    /// <summary>共通処理１</summary>
    /// <param name="buttonID">ボタンID</param>
    /// <param name="x">CPFを表す文字数</param>
    private void commonM(string buttonID, int x)
    {
        // コントロールの取得
        ((Label)this.GetMasterWebControl("lblMSG")).Text = buttonID;

        // ラベルを非表示
        Control ctrl = this.GetMasterWebControl(
            "lblTest" + buttonID.Substring(buttonID.Length - x, x));

        ctrl.Visible = !(ctrl.Visible);
    }

    /// <summary>共通処理２</summary>
    /// <param name="buttonID">ボタンID</param>
    /// <param name="x">CPFを表す文字数</param>
    private void commonC(string buttonID, int x)
    {
        // コントロールの取得
        ((Label)this.GetMasterWebControl("lblMSG")).Text = buttonID;

        // ラベルを非表示
        Control ctrl = this.GetContentWebControl(
            "lblTest" + buttonID.Substring(buttonID.Length - x, x));

        ctrl.Visible = !(ctrl.Visible);
    }

    #endregion

    #region イベント処理

    #region マスタ

    #region rootMasterPage

    /// <summary>btnButtonのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_rootMasterPage_btnButton_Click(FxEventArgs fxEventArgs)
    {
        this.commonM(fxEventArgs.ButtonID, 0);
        return System.String.Empty;
    }

    #endregion

    #region branchMasterPage1

    /// <summary>btnButtonAのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_branchMasterPage1_btnButtonA_Click(FxEventArgs fxEventArgs)
    {
        this.commonM(fxEventArgs.ButtonID, 1);
        return System.String.Empty;
    }

    /// <summary>btnButtonBのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_branchMasterPage1_btnButtonB_Click(FxEventArgs fxEventArgs)
    {
        this.commonM(fxEventArgs.ButtonID, 1);
        return System.String.Empty;
    }

    /// <summary>btnButtonCのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_branchMasterPage1_btnButtonC_Click(FxEventArgs fxEventArgs)
    {
        this.commonM(fxEventArgs.ButtonID, 1);
        return System.String.Empty;
    }

    #endregion

    #region branchMasterPage2

    /// <summary>btnButtonAAのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_branchMasterPage2_btnButtonAA_Click(FxEventArgs fxEventArgs)
    {
        this.commonM(fxEventArgs.ButtonID, 2);
        return System.String.Empty;
    }

    /// <summary>btnButtonABのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_branchMasterPage2_btnButtonAB_Click(FxEventArgs fxEventArgs)
    {
        this.commonM(fxEventArgs.ButtonID, 2);
        return System.String.Empty;
    }

    /// <summary>btnButtonACのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_branchMasterPage2_btnButtonAC_Click(FxEventArgs fxEventArgs)
    {
        this.commonM(fxEventArgs.ButtonID, 2);
        return System.String.Empty;
    }

    /// <summary>btnButtonBAのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_branchMasterPage2_btnButtonBA_Click(FxEventArgs fxEventArgs)
    {
        this.commonM(fxEventArgs.ButtonID, 2);
        return System.String.Empty;
    }

    /// <summary>btnButtonBBのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_branchMasterPage2_btnButtonBB_Click(FxEventArgs fxEventArgs)
    {
        this.commonM(fxEventArgs.ButtonID, 2);
        return System.String.Empty;
    }

    /// <summary>btnButtonBCのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_branchMasterPage2_btnButtonBC_Click(FxEventArgs fxEventArgs)
    {
        this.commonM(fxEventArgs.ButtonID, 2);
        return System.String.Empty;
    }

    /// <summary>btnButtonCAのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_branchMasterPage2_btnButtonCA_Click(FxEventArgs fxEventArgs)
    {
        this.commonM(fxEventArgs.ButtonID, 2);
        return System.String.Empty;
    }

    /// <summary>btnButtonCBのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_branchMasterPage2_btnButtonCB_Click(FxEventArgs fxEventArgs)
    {
        this.commonM(fxEventArgs.ButtonID, 2);
        return System.String.Empty;
    }

    /// <summary>btnButtonCCのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_branchMasterPage2_btnButtonCC_Click(FxEventArgs fxEventArgs)
    {
        this.commonM(fxEventArgs.ButtonID, 2);
        return System.String.Empty;
    }

    #endregion

    #endregion

    #region コンテンツ

    /// <summary>btnButtonAAAのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonAAA_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonAABのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonAAB_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonAACのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonAAC_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonABAのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonABA_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonABBのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonABB_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonABCのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonABC_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonACAのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonACA_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonACBのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonACB_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonACCのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonACC_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonBAAのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonBAA_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonBABのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonBAB_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonBACのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonBAC_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonBBAのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonBBA_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonBBBのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonBBB_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonBBCのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonBBC_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonBCAのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonBCA_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonBCBのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonBCB_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonBCCのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonBCC_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonCAAのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonCAA_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonCABのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonCAB_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonCACのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonCAC_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonCBAのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonCBA_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonCBBのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonCBB_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonCBCのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonCBC_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonCCAのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonCCA_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonCCBのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonCCB_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    /// <summary>btnButtonCCCのクリックイベント</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButtonCCC_Click(FxEventArgs fxEventArgs)
    {
        this.commonC(fxEventArgs.ButtonID, 3);
        return System.String.Empty;
    }

    ////// ちなみに存在しないコントロールを検索した場合どうなるかチェックする。
    ////Control ctrl = null;
    ////ctrl = this.GetMasterWebControl("xxxx");
    ////ctrl = this.GetContentWebControl("xxxx");

    #endregion

    #endregion
}

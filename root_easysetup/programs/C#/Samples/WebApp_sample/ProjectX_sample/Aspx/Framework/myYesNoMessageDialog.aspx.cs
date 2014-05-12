﻿//**********************************************************************************
//* サンプル
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_Framework_myYesNoMessageDialog
//* クラス日本語名  ：「YES」・「NO」メッセージ・ダイアログ（サンプル ※ プロジェクト毎、必要に応じて改修）
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
using System.Data;
using System.Configuration;
using System.Collections;

// System.Web
using System.Web;
using System.Web.Security;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Util;

// フレームワーク
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Exceptions;

// 部品
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Log;

/// <summary>「YES」・「NO」メッセージ・ダイアログ</summary>
/// <remarks>サンプル ※ プロジェクト毎、必要に応じて改修</remarks>
public partial class Aspx_Framework_myYesNoMessageDialog : System.Web.UI.Page
{
    /// <summary>初期化処理</summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // 親画面の画面GUIDを設定（QueryString）から取得
        string parentScreenGuid
            = (string)Request.QueryString[FxHttpQueryStringIndex.PARENT_SCREEN_GUID];

        // メッセージIDとメッセージをセッションより取得し、設定
        this.lblmessage.Text = 
            (string)Session[parentScreenGuid + FxHttpSessionIndex.MODAL_DIALOG_MESSAGE];

        this.lblmessageID.Text =
            (string)Session[parentScreenGuid + FxHttpSessionIndex.MODAL_DIALOG_MESSAGEID];

        // タイトル設定
        this.Title = 
            (string)Session[parentScreenGuid + FxHttpSessionIndex.MODAL_DIALOG_NAME];

        // アイコンへのパスを取得
        string iconPath;

        // 選択入力を促すアイコン（？）を設定
        iconPath = GetConfigParameter.GetConfigValue(FxLiteral.QUESTION_ICON_PATH);

        // エラー処理
        if (iconPath == "")
        {
            throw new FrameworkException(
                FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1[0],
                String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH1[1],
                    FxLiteral.QUESTION_ICON_PATH));
        }

        this.imgIcon.ImageUrl = iconPath;

        // 表示の完了後、セッションを消去する。
        Session.Remove(parentScreenGuid + FxHttpSessionIndex.MODAL_DIALOG_MESSAGE);
        Session.Remove(parentScreenGuid + FxHttpSessionIndex.MODAL_DIALOG_MESSAGEID);
        Session.Remove(parentScreenGuid + FxHttpSessionIndex.MODAL_DIALOG_NAME);

        // ACCESSログ出力 ----------------------------------------------

        // ------------
        // メッセージ部
        // ------------
        // ユーザ名, IPアドレス,
        // レイヤ, 画面名, コントロール名, 処理名
        // 処理時間（実行時間）, 処理時間（CPU時間）
        // エラーメッセージID, エラーメッセージ等
        // ------------
        string strLogMessage =
            "," + this.GetUserInfo().UserName +
            "," + Request.UserHostAddress +
            ",init" +
            ",YesNoMessageDialog" +
            ",";

        // Log4Netへログ出力
        LogIF.InfoLog("ACCESS", strLogMessage);
    }

    /// <summary>ユーザ情報を取得する</summary>
    /// <returns>ユーザ情報</returns>
    private MyUserInfo GetUserInfo()
    {
        MyUserInfo ui = (MyUserInfo)UserInfoHandle.GetUserInformation();

        // 再取得する。
        if (ui == null)
        {
            // Cookie認証チケット
            HttpCookie authCookie
                = Context.Request.Cookies["formauth"];

            if (authCookie == null) // 認証チケットがない場合
            {
                // ダミーのユーザ情報を設定する。
                ui = new MyUserInfo("未認証", Request.UserHostAddress);
            }
            else // 認証チケットがある場合
            {
                // ユーザ情報を再取得する。

                // 認証チケット
                FormsAuthenticationTicket authTicket
                    = FormsAuthentication.Decrypt(authCookie.Value);

                // ユーザ名を復元
                ui = new MyUserInfo(authTicket.Name, Request.UserHostAddress);
            }
        }

        return ui;
    }
}

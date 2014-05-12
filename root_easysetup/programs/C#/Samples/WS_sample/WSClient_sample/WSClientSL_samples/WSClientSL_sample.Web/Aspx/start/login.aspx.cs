//**********************************************************************************
//* サンプル
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Aspx_Start_login
//* クラス日本語名  ：ログイン画面（Forms認証対応）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

// System～
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

/// <summary>ログイン画面（Forms認証対応）</summary>
public partial class Aspx_Start_login : MyBaseController
{
    public Aspx_Start_login()
    {
        this.IsNoSession = true;
    }

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
        // ここでは何もしない

        // Session消去
        this.FxSessionAbandon();
        
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
        // ここでは何もしない

        // btnButton1のイベントであれば、Session消去しない
        if (Request.Form["ctl00$ContentPlaceHolder_A$btnButton1"] == null)
        {
            // Session消去
            this.FxSessionAbandon();
        }
    }

    #endregion

    #region イベントハンドラ 

    /// <summary>ログイン</summary>
    /// <param name="fxEventArgs">イベントハンドラの共通引数</param>
    /// <returns>URL</returns>
    protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
    {
        // ここで、入力されたユーザIDと、パスワードをチェックし、ユーザ認証する。

        if (this.txtUserID.Text != "")  // 現時点では、全て(空文字以外)認証する。
        {
            //// 認証か完了した場合、認証チケットを生成。
            //// 第２引数は、「クライアントがCookieを永続化（ファイルとして保存）するかどうか。」
            //// を設定する引数であるが、セキュリティを考慮して、falseの設定を勧める。
            //FormsAuthentication.SetAuthCookie(this.txtUserID.Text, false);

            // 認証か完了した場合、認証チケットを生成し、元のページにRedirectする。
            // 第２引数は、「クライアントがCookieを永続化（ファイルとして保存）するかどうか。」
            // を設定する引数であるが、セキュリティを考慮して、falseの設定を勧める。
            FormsAuthentication.RedirectFromLoginPage(this.txtUserID.Text, false);

            // 認証情報を保存する。
            MyUserInfo ui = new MyUserInfo(this.txtUserID.Text, Request.UserHostAddress);
            UserInfoHandle.SetUserInformation(ui);

            // 認証Sessionの場合のテスト
            Session["test"] = "test";
        }
        else
        {
            // 認証に失敗した場合は、メッセージを表示する
            this.lblMessage.Text = "認証に失敗しました。ユーザIDか、パスワードが間違っています。";

            // Session消去
            this.FxSessionAbandon();
        }

        //// Silverlightを起動
        //return "/WSClientSL_sample/WSClientSL_sampleTestPage.aspx";

        // 画面遷移はしない（基盤に任せるため）。
        return string.Empty;
    }

    #endregion
}

﻿//**********************************************************************************
//* サンプル画面（Ｐ層）
//**********************************************************************************

// サンプル画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：login
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

using System.Web.Security;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security.Pwd;

namespace WebForms_Sample.Aspx.Start
{
    /// <summary>ログイン画面（Forms認証対応）</summary>
    public partial class login : MyBaseController
    {
        /// <summary>Nonce</summary>
        public string Nonce
        {
            get
            {
                if (Session["nonce"] == null)
                {
                    Session["nonce"] = GetPassword.Base64UrlSecret(10);
                }
                return (string)Session["nonce"];
            }
        }

        /// <summary>State</summary>
        public string State
        {
            get
            {
                if (Session["state"] == null)
                {
                    Session["state"] = GetPassword.Base64UrlSecret(10);
                }
                return (string)Session["state"];
            }
        }

        /// <summary>constructor</summary>
        public login()
        {
            this.IsNoSession = true;
        }

        #region Page LoadのUOCメソッド

        /// <summary>
        /// Page LoadのUOCメソッド（個別：初回Load）
        /// </summary>
        /// <remarks>
        /// 実装必須
        /// </remarks>
        protected override void UOC_FormInit()
        {
            // Form初期化（初回Load）時に実行する処理を実装する

            // TODO:
            // ここでは何もしない

            // Session消去
            this.FxSessionAbandon();
        }

        /// <summary>
        /// Page LoadのUOCメソッド（個別：Post Back）
        /// </summary>
        /// <remarks>
        /// 実装必須
        /// </remarks>
        protected override void UOC_FormInit_PostBack()
        {
            // Form初期化（Post Back）時に実行する処理を実装する

            // TODO:
            // ここでは何もしない

            // btnButton1, 2のイベントであれば、Session消去しない
            if (Request.Form["ctl00$ContentPlaceHolder_A$btnButton1"] == null 
                && Request.Form["ctl00$ContentPlaceHolder_A$btnButton2"] == null)
            {
                // Session消去
                this.FxSessionAbandon();
            }
        }

        #endregion

        #region Event Handler

        /// <summary>ログイン</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton1_Click(FxEventArgs fxEventArgs)
        {
            // ここで、入力されたユーザIDと、パスワードをチェックし、ユーザ認証する。

            if (!string.IsNullOrEmpty(this.txtUserID.Text))  // 現時点では、全て(空文字以外)認証する。
            {
                // 認証か完了した場合、認証チケットを生成し、元のページにRedirectする。
                // 第２引数は、「クライアントがCookieを永続化（ファイルとして保存）するかどうか。」
                // を設定する引数であるが、セキュリティを考慮して、falseの設定を勧める。
                FormsAuthentication.RedirectFromLoginPage(this.txtUserID.Text, false);

                // 認証情報を保存する。
                MyUserInfo ui = new MyUserInfo(this.txtUserID.Text, Request.UserHostAddress);
                UserInfoHandle.SetUserInformation(ui);
            }
            else
            {
                // 認証に失敗した場合は、Messageを表示する
                this.lblMessage.Text = "認証に失敗しました。ユーザIDか、パスワードが間違っています。";

                // Session消去
                this.FxSessionAbandon();
            }

            // 画面遷移はしない（基盤に任せるため）。
            return string.Empty;
        }

        /// <summary>外部ログイン</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        protected string UOC_btnButton2_Click(FxEventArgs fxEventArgs)
        {
            return CmnClientParams.SpRp_AuthRequestUri
                + "?client_id=" + OAuth2AndOIDCParams.ClientID
                + "&response_type=code"
                + "&scope=profile%20email%20phone%20address%20roles%20openid"
                + "&state=" + this.State
                + "&nonce=" + this.Nonce
                + "&redirect_uri=" + CustomEncode.UrlEncode(CmnClientParams.SpRp_RedirectUri)
                + "&prompt=none";
        }

        #endregion
    } 
}

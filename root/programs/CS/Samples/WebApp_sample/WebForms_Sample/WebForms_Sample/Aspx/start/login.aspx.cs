//**********************************************************************************
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
//*  2026/11/04  玄人 幸道         外部ログインの要求を、MPAS のスタブに合わせた（prompt=none を外す・値をエンコード。#649）
//*  2026/11/04  玄人 幸道         SAML2 の外部ログイン（btnButton3）を追加した（#649）
//**********************************************************************************

using System;
using System.Threading.Tasks;
using System.Web.Security;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Security.Pwd;

namespace WebForms_Sample.Aspx.Start
{
    /// <summary>ログイン画面（Forms認証対応）</summary>
    public partial class login : MyBaseController
    {
        #region プロパティ（OIDC の state・nonce）

        /// <summary>Nonce</summary>
        public string Nonce
        {
            get
            {
                if (Session["nonce"] == null)
                {
                    Session["nonce"] = GetPassword.Base64UrlSecret(32);
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
                    Session["state"] = GetPassword.Base64UrlSecret(32);
                }
                return (string)Session["state"];
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>constructor</summary>
        public login()
        {
            this.IsNoSession = true;
        }

        #endregion

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

            // btnButton1, 2, 3のイベントであれば、Session消去しない
            if (Request.Form["ctl00$ContentPlaceHolder_A$btnButton1"] == null
                && Request.Form["ctl00$ContentPlaceHolder_A$btnButton2"] == null
                && Request.Form["ctl00$ContentPlaceHolder_A$btnButton3"] == null)
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
        /// <remarks>
        /// OIDC の認可コード フロー。応答は OAuth2/OAuth2AuthorizationCodeGrantClient.aspx が受ける。
        /// **prompt=none は付けない。** IdP にまだセッションが無いと、
        /// ログイン画面を出さずに login_required で戻ってしまう（OpenID Connect Core 1.0 3.1.2.1）。
        /// </remarks>
        protected string UOC_btnButton2_Click(FxEventArgs fxEventArgs)
        {
            return CmnClientParams.SpRp_AuthRequestUri
                + "?client_id=" + Uri.EscapeDataString(OAuth2AndOIDCParams.ClientID)
                + "&response_type=code"
                + "&scope=" + Uri.EscapeDataString("openid profile email phone address roles")
                + "&state=" + Uri.EscapeDataString(this.State)
                + "&nonce=" + Uri.EscapeDataString(this.Nonce)
                + "&redirect_uri=" + Uri.EscapeDataString(CmnClientParams.SpRp_RedirectUri);
        }

        /// <summary>外部ログイン（SAML2）</summary>
        /// <param name="fxEventArgs">Event Handlerの共通引数</param>
        /// <returns>URL</returns>
        /// <remarks>
        /// 応答は Saml2/Saml2AssertionConsumerService.aspx が受ける。
        /// **IdP の SSO の口は、メタデータ（SpRp_Saml2MetadataUri）から引く**（entityID は SpRp_Saml2EntityId と照合する）。
        /// **要求は HTTP-Redirect で送り、応答も HTTP-Redirect で受ける**（ProtocolBinding）。
        /// **NameID は Persistent を求める**（SP ごとに固定の値。識別子にする。SAML Core 8.3.7）。
        /// 要求には SpRp_RsaPfxFilePath の鍵で署名する（IdP が WantAuthnRequestsSigned）。
        /// **Issuer は "http://" ＋ client_id**（汎用認証サイトは、Issuer から client_id を引く）。
        /// </remarks>
        protected string UOC_btnButton3_Click(FxEventArgs fxEventArgs)
        {
            // **イベント ハンドラは同期なので、別スレッドで待つ**（同期コンテキストでの .Result はデッドロックしうる）
            SAML2Metadata metadata = Task.Run(() => SAML2Metadata.GetAsync()).Result;
            if (metadata == null || string.IsNullOrEmpty(metadata.SsoRedirectLocation))
            {
                // メタデータが引けない（IdP が動いていない、entityID が違う）
                this.lblMessage.Text = "SAML2 の IdP のメタデータを取得できませんでした。";
                return string.Empty;
            }

            // **RelayState と要求の ID を控え、応答と照合する**（CSRF・応答の差し替え）
            string relayState = GetPassword.Base64UrlSecret(32);
            string id = "";
            string queryString = SAML2Client.CreateRedirectRequest(
                SAML2Enum.RequestOrResponse.Request,
                SAML2Enum.ProtocolBinding.HttpRedirect,
                SAML2Enum.NameIDFormat.Persistent,
                "http://" + OAuth2AndOIDCParams.ClientID,
                CmnClientParams.SpRp_Saml2AcsUri, relayState, out id);

            Session["saml2_relay_state"] = relayState;
            Session["saml2_request_id"] = id;

            return metadata.SsoRedirectLocation + "?" + queryString;
        }

        #endregion
    } 
}

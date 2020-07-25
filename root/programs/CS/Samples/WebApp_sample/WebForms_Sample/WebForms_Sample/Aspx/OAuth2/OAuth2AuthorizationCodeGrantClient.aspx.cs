//**********************************************************************************
//* サンプル画面（認証）
//**********************************************************************************

// サンプル画面なので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：OAuth2AuthorizationCodeGrantClient
//* クラス日本語名  ：OAuth2, OIDC認証画面
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System;
using System.Collections.Generic;

using System.Net.Http;
using System.Web;
using System.Web.Security;

using Microsoft.Owin.Security.DataHandler.Encoder;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Security.Pwd;

namespace WebForms_Sample.Aspx.OAuth2
{
    /// <summary>認証画面</summary>
    public partial class OAuth2AuthorizationCodeGrantClient : System.Web.UI.Page
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

        /// <summary>Page_Load</summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        protected async void Page_Load(object sender, EventArgs e)
        {
            string code = Request.QueryString["code"];
            string state = Request.QueryString["state"];

            try
            {
                string response = "";

                if (state == this.State) // CSRF(XSRF)対策のstateの検証は重要
                {
                    response = await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                        new Uri("https://localhost:44300/MultiPurposeAuthSite/token"),
                        OAuth2AndOIDCParams.ClientID, OAuth2AndOIDCParams.ClientSecret, "", code);

                    // 汎用認証サイトはOIDCをサポートしたのでid_tokenを取得し、検証可能。
                    Base64UrlTextEncoder base64UrlEncoder = new Base64UrlTextEncoder();
                    Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

                    string sub = "";
                    string nonce = "";
                    JObject jobj = null;

                    // id_tokenの検証
                    if (IdToken.Verify(dic["id_token"], dic["access_token"],
                        code, state, out sub, out nonce, out jobj) && nonce == this.Nonce)
                    {
                        // ログインに成功
                        // /userinfoエンドポイントにアクセスする場合
                        response = await OAuth2AndOIDCClient.GetUserInfoAsync(
                        new Uri("https://localhost:44300/MultiPurposeAuthSite/userinfo"), dic["access_token"]);

                        FormsAuthentication.RedirectFromLoginPage(sub, false);
                        MyUserInfo ui = new MyUserInfo(sub, Request.UserHostAddress);
                        UserInfoHandle.SetUserInformation(ui);

                        return;
                    }
                    else { }
                }
                else { }

                // ResolveClientUrlがInvalidOperationExceptionを吐くので...
                //// ログインに失敗
                //Response.Redirect("../Start/login.aspx");
            }
            finally
            {
                this.ClearExLoginsParams();
            }
        }

        /// <summary>ClearExLoginsParam</summary>
        private void ClearExLoginsParams()
        {
            Session["nonce"] = null;
            Session["state"] = null;
        }
    }
}
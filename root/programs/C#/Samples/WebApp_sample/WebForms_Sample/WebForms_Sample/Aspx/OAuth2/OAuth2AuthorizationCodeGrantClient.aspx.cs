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
using Touryo.Infrastructure.Public.Security;

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
                OAuth2AndOIDCClient.HttpClient = new HttpClient();
                string response = "";

                if (state == this.State) // CSRF(XSRF)対策のstateの検証は重要
                {
                    response = await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                        new Uri("http://localhost:63359/MultiPurposeAuthSite/OAuthBearerToken"),
                        OAuth2AndOIDCParams.ClientID, OAuth2AndOIDCParams.ClientSecret,
                        HttpUtility.HtmlEncode("http://localhost:9999/WebForms_Sample/Aspx/Auth/OAuthAuthorizationCodeGrantClient.aspx"), code);
                    
                    // 汎用認証サイトはOIDCをサポートしたのでid_tokenを取得し、検証可能。
                    Base64UrlTextEncoder base64UrlEncoder = new Base64UrlTextEncoder();
                    Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

                    // id_tokenの検証コード
                    if (dic.ContainsKey("id_token"))
                    {
                        string id_token = dic["id_token"];

                        string sub = "";
                        List<string> roles = null;
                        List<string> scopes = null;
                        JObject jobj = null;

                        if (JwtToken.Verify(id_token, out sub, out roles, out scopes, out jobj)
                            && jobj["nonce"].ToString() == this.Nonce)
                        {
                            // ログインに成功

                            // /userinfoエンドポイントにアクセスする場合
                            response = await OAuth2AndOIDCClient.CallUserInfoEndpointAsync(
                                new Uri("http://localhost:63359/MultiPurposeAuthSite/userinfo"), dic["access_token"]);

                            FormsAuthentication.RedirectFromLoginPage(sub, false);
                            MyUserInfo ui = new MyUserInfo(sub, Request.UserHostAddress);
                            UserInfoHandle.SetUserInformation(ui);

                            
                            return;
                        }

                    }
                    else { }
                }
                else { }

                // ログインに失敗
                Response.Redirect("../Start/login.aspx");
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
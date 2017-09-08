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
using System.Net.Http.Headers;
using System.Web.Security;

using Microsoft.Owin.Security.DataHandler.Encoder;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Util;

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

        /// <summary></summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void Page_Load(object sender, EventArgs e)
        {
            string code = Request.QueryString["code"];
            string state = Request.QueryString["state"];

            try
            {
                if (state == this.State) // CSRF(XSRF)対策のstateの検証は重要
                {
                    HttpClient httpClient = new HttpClient();

                    HttpRequestMessage httpRequestMessage = null;
                    HttpResponseMessage httpResponseMessage = null;

                    // HttpRequestMessage (Method & RequestUri)
                    httpRequestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("http://localhost:63359/MultiPurposeAuthSite/OAuthBearerToken"),
                    };
                    
                    // HttpRequestMessage (Headers & Content)
                    httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", new string[] { OAuth2AndOIDCParams.ClientID, OAuth2AndOIDCParams.ClientSecret }))));

                    httpRequestMessage.Content = new FormUrlEncodedContent(
                        new Dictionary<string, string>
                        {
                            { "grant_type", "authorization_code" },
                            { "code", code },
                            { "redirect_uri", System.Web.HttpUtility.HtmlEncode("http://localhost:9999/WebForms_Sample/Aspx/Auth/OAuthAuthorizationCodeGrantClient.aspx") },
                        });

                    // HttpResponseMessage
                    httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                    string response = await httpResponseMessage.Content.ReadAsStringAsync();

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
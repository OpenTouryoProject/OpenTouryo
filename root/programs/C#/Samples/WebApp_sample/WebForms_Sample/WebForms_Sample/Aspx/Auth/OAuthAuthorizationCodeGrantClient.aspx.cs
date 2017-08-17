using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Security;

using Microsoft.Owin.Security.DataHandler.Encoder;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Util.JWT;

namespace WebForms_Sample.Aspx.Auth
{
    /// <summary>認証画面</summary>
    public partial class OAuthAuthorizationCodeGrantClient : System.Web.UI.Page
    {
        /// <summary>Nonce</summary>
        public string Nonce
        {
            get
            {
                if (Session["nonce"] == null)
                {
                    Session["nonce"] = GetPassword.Base64UrlSecret(10);
                    return (string)Session["nonce"];
                }
                else
                {
                    return (string)Session["nonce"]; 
                }                
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
                    return (string)Session["state"];
                }
                else
                {
                    return (string)Session["state"];
                }
            }
        }

        /// <summary></summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void Page_Load(object sender, EventArgs e)
        {
            string code = Request.QueryString["code"];
            string state = Request.QueryString["state"];

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
                        string.Format("{0}:{1}",
                            "b6b393fe861b430eb4ee061006826b03",
                            "p2RgAFKF-JaF0A9F1tyDXp4wMq-uQZYyvTBM8wr_v8g"))));

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
                string id_token = dic["id_token"];

                JWT_RS256 jwtRS256 = new JWT_RS256(
                    @"C:\Git1\MultiPurposeAuthSite\root\programs\MultiPurposeAuthSite\CreateClientsIdentity\CreateClientsIdentity_RS256.cer", "test");

                if (jwtRS256.Verify(id_token))
                {
                    string jwtPayload = Encoding.UTF8.GetString(base64UrlEncoder.Decode(dic["id_token"].Split('.')[1]));

                    // id_tokenライクなJWTなので、中からsubなどを取り出すことができる。
                    JObject jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                    string nonce = (string)jobj["nonce"];
                    string iss = (string)jobj["iss"];
                    string aud = (string)jobj["aud"];
                    string iat = (string)jobj["iat"];
                    string exp = (string)jobj["exp"];

                    string sub = (string)jobj["sub"];

                    if (nonce == this.Nonce && 
                        iss == "http://jwtssoauth.opentouryo.com" &&
                        aud == "b6b393fe861b430eb4ee061006826b03" &&
                        long.Parse(exp) >= DateTimeOffset.Now.ToUnixTimeSeconds())
                    {
                        // ログインに成功
                        FormsAuthentication.RedirectFromLoginPage(sub, false);
                        MyUserInfo ui = new MyUserInfo(sub, Request.UserHostAddress);
                        UserInfoHandle.SetUserInformation(ui);

                        return;
                    }
                    else
                    { }
                }
                else
                { }

                // ログインに失敗
                Response.Redirect("../Start/login.aspx");
            }

            // スターター
        }
    }
}
//**********************************************************************************
//* サンプル アプリ・コントローラ
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：HomeController
//* クラス日本語名  ：認証用サンプル アプリ・コントローラ
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using SPA_Sample.Models.ViewModels;

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.Owin.Security.DataHandler.Encoder;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Util.JWT;

namespace SPA_Sample.Controllers
{
    /// <summary>HomeController</summary>
    public class HomeController : MyBaseMVController
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

        /// <summary>
        /// GET: Home
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Home/Login
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        public ActionResult Login()
        {
            // Session消去
            this.FxSessionAbandon();

            return this.View();
        }

        /// <summary>
        /// POST: /Home/Login
        /// </summary>
        /// <param name="model">LoginViewModel</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!Request.Form.AllKeys.Any(x => x == "external"))
            {
                // 通常ログイン
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.UserName))
                    {
                        // 認証か完了した場合、認証チケットを生成し、元のページにRedirectする。
                        // 第２引数は、「クライアントがCookieを永続化（ファイルとして保存）するかどうか。」
                        // を設定する引数であるが、セキュリティを考慮して、falseの設定を勧める。
                        FormsAuthentication.RedirectFromLoginPage(model.UserName, false);

                        // 認証情報を保存する。
                        MyUserInfo ui = new MyUserInfo(model.UserName, Request.UserHostAddress);
                        UserInfoHandle.SetUserInformation(ui);

                        //基盤に任せるのでリダイレクトしない。
                        //return this.Redirect(ReturnUrl);
                        return new EmptyResult();
                    }
                    else
                    {
                        // ユーザー認証 失敗
                        this.ModelState.AddModelError(string.Empty, "指定されたユーザー名またはパスワードが正しくありません。");

                        // Session消去
                        this.FxSessionAbandon();
                    }
                }
                else
                {
                    // Session消去
                    this.FxSessionAbandon();
                }

                // ポストバック的な
                return this.View(model);
            }
            else
            {
                // 外部ログイン
                return Redirect(string.Format(
                    "http://localhost:63359/MultiPurposeAuthSite/Account/OAuthAuthorize?client_id=f374a155909d486a9234693c34e94479&response_type=code&scope=profile%20email%20phone%20address%20userid%20auth%20openid&state={0}&nonce={1}",
                    this.State, this.Nonce));
            }
        }

        /// <summary>
        /// Get: /Home/Scroll
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Scroll()
        {
            return this.View();
        }

        /// <summary>
        /// Get: /Home/Error
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Error()
        {
            throw new System.Exception("");
        }

        /// <summary>
        /// Get: /Home/Logout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return this.Redirect(Url.Action("Index", "Home"));
        }

        /// <summary>OAuthAuthorizationCodeGrantClient</summary>
        /// <param name="code">string</param>
        /// <param name="state">string</param>
        /// <returns>ActionResultを非同期的に返す</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> OAuthAuthorizationCodeGrantClient(string code, string state)
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
                        string.Format("{0}:{1}",
                            "f374a155909d486a9234693c34e94479",
                            "z54lhkewWPl4hk3eF1WYwvdqt7Fz24jYamLPZFVnWpA"))));

                httpRequestMessage.Content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        { "grant_type", "authorization_code" },
                        { "code", code },
                        { "redirect_uri", System.Web.HttpUtility.HtmlEncode("http://localhost:63877/SPA_Sample/Home/OAuthAuthorizationCodeGrantClient") },
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
                        aud == "f374a155909d486a9234693c34e94479" &&
                        long.Parse(exp) >= DateTimeOffset.Now.ToUnixTimeSeconds())
                    {
                        // ログインに成功
                        FormsAuthentication.RedirectFromLoginPage(sub, false);
                        MyUserInfo ui = new MyUserInfo(sub, Request.UserHostAddress);
                        UserInfoHandle.SetUserInformation(ui);

                        return new EmptyResult();
                    }
                    else
                    { }
                }
                else
                { }
            }

            // ログインに失敗
            return RedirectToAction("Login");
        }
    }
}
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

using MVC_Sample.Models.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using Microsoft.Owin.Security.DataHandler.Encoder;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Security.Pwd;

namespace MVC_Sample.Controllers
{
    /// <summary>HomeController</summary>
    [Authorize]
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

        /// <summary>
        /// GET: Home
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Home/Login
        /// </summary>
        /// <returns>ActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
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
                    }
                }
                else
                {
                    // LoginViewModelの検証に失敗
                }

                // Session消去
                this.FxSessionAbandon();

                // ポストバック的な
                return this.View(model);
            }
            else
            {
                // 外部ログイン
                return Redirect(string.Format(
                    "https://localhost:44300/MultiPurposeAuthSite/authorize"
                    + "?client_id=" + OAuth2AndOIDCParams.ClientID
                    + "&response_type=code" 
                    + "&scope=profile%20email%20phone%20address%20openid" 
                    + "&state={0}" 
                    + "&nonce={1}"
                    + "&prompt=none",
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
        /// Get: /Home/Logout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return this.Redirect(Url.Action("Index", "Home"));
        }

        /// <summary>OAuth2AuthorizationCodeGrantClient</summary>
        /// <param name="code">string</param>
        /// <param name="state">string</param>
        /// <returns>ActionResultを非同期的に返す</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> OAuth2AuthorizationCodeGrantClient(string code, string state)
        {
            try
            {
                string response = "";

                if (state == this.State) // CSRF(XSRF)対策のstateの検証は重要
                {
                    response = await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                        new Uri("https://localhost:44300/MultiPurposeAuthSite/token"),
                        OAuth2AndOIDCParams.ClientID, OAuth2AndOIDCParams.ClientSecret,
                        HttpUtility.HtmlEncode("http://localhost:58496/MVC_Sample/Home/OAuth2AuthorizationCodeGrantClient"), code);
                    
                    // 汎用認証サイトはOIDCをサポートしたのでid_tokenを取得し、検証可能。
                    Base64UrlTextEncoder base64UrlEncoder = new Base64UrlTextEncoder();
                    Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

                    // id_tokenの検証コード
                    if (dic.ContainsKey("id_token"))
                    {
                        string sub = "";
                        string nonce = "";
                        JObject jobj = null;

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

                            return new EmptyResult();
                        }

                    }
                    else { }
                }
                else { }

                // ログインに失敗
                return RedirectToAction("Login");
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
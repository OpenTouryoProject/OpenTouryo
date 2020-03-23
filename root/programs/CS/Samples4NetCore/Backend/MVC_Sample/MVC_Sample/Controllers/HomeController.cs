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
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

using System.Web;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Security.Pwd;

namespace MVC_Sample.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class HomeController : MyBaseMVControllerCore
    {
        /// <summary>Nonce</summary>
        public string Nonce
        {
            get
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("nonce")))
                {
                    HttpContext.Session.SetString("nonce", GetPassword.Base64UrlSecret(10));
                }
                return HttpContext.Session.GetString("nonce");
            }
        }

        /// <summary>State</summary>
        public string State
        {
            get
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("state")))
                {
                    HttpContext.Session.SetString("state", GetPassword.Base64UrlSecret(10));
                }
                return HttpContext.Session.GetString("state");
            }
        }

        /// <summary>
        /// GET: Home
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Home/Login
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Session消去
            //this.FxSessionAbandon();

            return this.View();
        }

        /// <summary>
        /// POST: /Home/Login
        /// </summary>
        /// <param name="model">LoginViewModel</param>
        /// <returns>IActionResultを非同期的に返す。</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!Request.Form.Keys.Any(x => x == "external"))
            {
                // 通常ログイン
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.UserName))
                    {
                        // 認証情報を作成する。
                        List<Claim> claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, model.UserName));

                        // 認証情報を保存する。
                        ClaimsIdentity userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);

                        // サイン アップする。
                        await AuthenticationHttpContextExtensions.SignInAsync(
                            this.HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                        // 認証情報を保存する。
                        MyUserInfo ui = new MyUserInfo(model.UserName, (new GetClientIpAddress()).GetAddress());
                        UserInfoHandle.SetUserInformation(ui);

                        //基盤に任せるのでリダイレクトしない。
                        return View(model);

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
                //this.FxSessionAbandon();

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
        /// <returns>IActionResult</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Scroll()
        {
            return View();
        }

        /// <summary>
        /// Get: /Home/Logout
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            //FormsAuthentication.SignOut();
            await AuthenticationHttpContextExtensions.SignOutAsync(
                this.HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);

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
                        HttpUtility.HtmlEncode("http://localhost:58496/Home/OAuth2AuthorizationCodeGrantClient"), code);

                    // 汎用認証サイトはOIDCをサポートしたのでid_tokenを取得し、検証可能。
                    //Base64UrlTextEncoder base64UrlEncoder = new Base64UrlTextEncoder();
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

                            // 認証情報を作成する。
                            List<Claim> claims = new List<Claim>();
                            claims.Add(new Claim(ClaimTypes.Name, sub));

                            // 認証情報を保存する。
                            ClaimsIdentity userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);

                            // サイン アップする。
                            await AuthenticationHttpContextExtensions.SignInAsync(
                                this.HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                            // 認証情報を保存する。
                            MyUserInfo ui = new MyUserInfo(sub, (new GetClientIpAddress()).GetAddress());
                            UserInfoHandle.SetUserInformation(ui);

                            return this.Redirect(Url.Action("Index", "Home"));
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
            HttpContext.Session.SetString("nonce", "");
            HttpContext.Session.SetString("state", "");
        }
    }
}

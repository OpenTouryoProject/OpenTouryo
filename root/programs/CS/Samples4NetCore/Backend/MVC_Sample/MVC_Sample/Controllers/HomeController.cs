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
//*  2026/11/04  玄人 幸道         外部ログインを、今のライブラリと MPAS のスタブに合わせた（#649）
//*                                （prompt=none を外す・トークン要求に redirect_uri・id_token を jwks_uri の鍵と iss / aud で検証）
//*  2026/11/04  玄人 幸道         表示名を UserInfo の email にした（識別子は sub のまま。UserInfo の sub を id_token と照合。#649）
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
using Touryo.Infrastructure.Public.Security.Jwt;
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
                    HttpContext.Session.SetString("nonce", GetPassword.Base64UrlSecret(32));
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
                    HttpContext.Session.SetString("state", GetPassword.Base64UrlSecret(32));
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
                // 外部ログイン（OIDC の認可コード フロー）
                //   **prompt=none は付けない。** IdP にまだセッションが無いと、
                //   ログイン画面を出さずに login_required で戻ってしまう（OpenID Connect Core 1.0 3.1.2.1）。
                return Redirect(CmnClientParams.SpRp_AuthRequestUri
                    + "?client_id=" + Uri.EscapeDataString(OAuth2AndOIDCParams.ClientID)
                    + "&response_type=code"
                    + "&scope=" + Uri.EscapeDataString("openid profile email phone address")
                    + "&state=" + Uri.EscapeDataString(this.State)
                    + "&nonce=" + Uri.EscapeDataString(this.Nonce)
                    + "&redirect_uri=" + Uri.EscapeDataString(CmnClientParams.SpRp_RedirectUri));
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
        /// <param name="iss">string（認可応答の iss。RFC 9207）</param>
        /// <returns>ActionResultを非同期的に返す</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> OAuth2AuthorizationCodeGrantClient(string code, string state, string iss)
        {
            try
            {
                string response = "";

                if (!string.IsNullOrEmpty(code)
                    && state == this.State // CSRF(XSRF)対策のstateの検証は重要
                    && (string.IsNullOrEmpty(iss) || iss == CmnClientParams.Isser)) // Mix-Up 対策（RFC 9207）
                {
                    // **redirect_uri は、認可要求と同じ値を送る**（RFC 6749 4.1.3）
                    response = await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                        new Uri(CmnClientParams.SpRp_TokenRequestUri),
                        OAuth2AndOIDCParams.ClientID, OAuth2AndOIDCParams.ClientSecret,
                        CmnClientParams.SpRp_RedirectUri, code);

                    JObject token = JObject.Parse(response);
                    string id_token = (string)token[OAuth2AndOIDCConst.IDToken];
                    string access_token = (string)token[OAuth2AndOIDCConst.AccessToken];

                    // id_tokenの検証コード
                    if (!string.IsNullOrEmpty(id_token))
                    {
                        string sub = await this.VerifyIdTokenAsync(id_token, access_token, code);

                        if (!string.IsNullOrEmpty(sub))
                        {
                            // /userinfoエンドポイントにアクセスし、表示名（email）を得る
                            string displayName = HomeController.GetDisplayName(
                                await OAuth2AndOIDCClient.GetUserInfoAsync(
                                    new Uri(CmnClientParams.SpRp_UserInfoUri), access_token), sub);

                            if (displayName == null)
                            {
                                // UserInfo の sub が id_token と違う（OpenID Connect Core 1.0 5.3.2）
                                return RedirectToAction("Login");
                            }

                            // ログインに成功

                            // 認証情報を作成する。
                            //   **利用者の識別子は sub**（iss と組で一意・不変。OpenID Connect Core 1.0 5.7）。
                            //   **email は表示名**（変更・再割り当てがありうるので、識別子にしない）。
                            List<Claim> claims = new List<Claim>();
                            claims.Add(new Claim(ClaimTypes.Name, sub));
                            claims.Add(new Claim(ClaimTypes.NameIdentifier, sub));
                            claims.Add(new Claim(ClaimTypes.Email, displayName));

                            // 認証情報を保存する。
                            ClaimsIdentity userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);

                            // サイン アップする。
                            await AuthenticationHttpContextExtensions.SignInAsync(
                                this.HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                            // 認証情報を保存する（画面・ログに出すユーザ名は、表示名）。
                            MyUserInfo ui = new MyUserInfo(displayName, (new GetClientIpAddress()).GetAddress());
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

        /// <summary>相手（OP）の id_token を検証する</summary>
        /// <param name="id_token">id_token</param>
        /// <param name="access_token">access_token（at_hash の検証）</param>
        /// <param name="code">code（c_hash の検証）</param>
        /// <returns>sub（検証に失敗したら null）</returns>
        /// <remarks>
        /// **鍵は、ヘッダの kid で jwks_uri から引く**（SpRp_DiscoveryUri の Discovery。issuer は SpRp_Isser と照合する）。
        /// **alg は非対称のもの（JwsFactory.SupportedAlgs）に限る**（none・HS* を受けない）。
        /// **iss・aud を指定する版の IdToken.Verify を使う**（#601。相手の id_token を、設定の自分の鍵で検証しない）。
        /// </remarks>
        private async Task<string> VerifyIdTokenAsync(string id_token, string access_token, string code)
        {
            string alg = JwtReader.TryReadAlg(id_token);
            if (!JwsFactory.IsSupported(alg)) return null;

            JObject jwk = await RemoteJwkSetStore.GetJwkByDiscoveryAsync(JwtReader.TryReadKid(id_token));
            if (jwk == null) return null;

            JWS jws = JwsFactory.CreateFromJwk(alg, jwk);
            if (jws == null) return null;

            if (IdToken.Verify(id_token, access_token, code, "",
                jws, CmnClientParams.Isser, OAuth2AndOIDCParams.ClientID,
                out string sub, out string nonce, out JObject _)
                && nonce == this.Nonce)
            {
                return sub;
            }

            return null;
        }

        /// <summary>UserInfo の応答から、表示名を得る</summary>
        /// <param name="userInfoJson">UserInfo の応答</param>
        /// <param name="sub">id_token の sub</param>
        /// <returns>email（無ければ sub）。UserInfo の sub が id_token と違えば null</returns>
        /// <remarks>
        /// **UserInfo の sub は、id_token の sub と一致しなければならない**（OpenID Connect Core 1.0 5.3.2）。
        /// 一致しない応答は使わない（トークンの差し替え）。
        /// **email は表示にだけ使う。** 利用者の識別子（UserStore のキー）は sub である。
        /// </remarks>
        private static string GetDisplayName(string userInfoJson, string sub)
        {
            JObject userInfo;
            try
            {
                userInfo = JObject.Parse(userInfoJson);
            }
            catch (JsonReaderException)
            {
                return null;
            }

            if ((string)userInfo[OAuth2AndOIDCConst.sub] != sub) return null;

            string email = (string)userInfo["email"]; // email クレーム（OpenID Connect Core 1.0 5.1）
            return string.IsNullOrEmpty(email) ? sub : email;
        }

        /// <summary>ClearExLoginsParam</summary>
        private void ClearExLoginsParams()
        {
            HttpContext.Session.SetString("nonce", "");
            HttpContext.Session.SetString("state", "");
        }
    }
}

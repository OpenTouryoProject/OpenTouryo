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
//*  2026/11/04  玄人 幸道         SAML2 の外部ログインを追加した（Redirect Binding。NameID は Persistent、表示名は mail 属性。#649）
//*  2026/11/04  玄人 幸道         並びを統一し、Region でまとめた（定数・プロパティ・画面・OIDC・SAML2・共通。net48 版と同じ。#649）
//*  2026/11/06  玄人 幸道         コメントの設定キーを、改名後の SpRp_Issuer にした（#666）
//**********************************************************************************

using MVC_Sample.Models.ViewModels;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Cryptography;

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
    /// <summary>HomeController</summary>
    /// <remarks>
    /// 並び順（net48 版の HomeController と同じ）
    /// | Region | 内容 |
    /// |---|---|
    /// | 定数 | Session のキー |
    /// | プロパティ | OIDC の state・nonce |
    /// | 画面 | Index・Login（GET / POST）・Scroll・Logout |
    /// | 外部ログイン（OIDC） | 要求 → 応答（リダイレクト先）→ 検証 |
    /// | 外部ログイン（SAML2） | 要求 → 応答（ACS） |
    /// | 外部ログイン（共通） | サイン イン |
    /// </remarks>
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class HomeController : MyBaseMVControllerCore
    {
        #region 定数（Session のキー）

        /// <summary>OIDC の nonce</summary>
        private const string SessionKeyNonce = "nonce";

        /// <summary>OIDC の state</summary>
        private const string SessionKeyState = "state";

        /// <summary>SAML2 の RelayState</summary>
        private const string SessionKeySaml2RelayState = "saml2_relay_state";

        /// <summary>SAML2 の AuthnRequest の ID（応答の InResponseTo と照合する）</summary>
        private const string SessionKeySaml2RequestId = "saml2_request_id";

        #endregion

        #region プロパティ（OIDC の state・nonce）

        /// <summary>Nonce</summary>
        public string Nonce
        {
            get
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString(HomeController.SessionKeyNonce)))
                {
                    HttpContext.Session.SetString(HomeController.SessionKeyNonce, GetPassword.Base64UrlSecret(32));
                }
                return HttpContext.Session.GetString(HomeController.SessionKeyNonce);
            }
        }

        /// <summary>State</summary>
        public string State
        {
            get
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString(HomeController.SessionKeyState)))
                {
                    HttpContext.Session.SetString(HomeController.SessionKeyState, GetPassword.Base64UrlSecret(32));
                }
                return HttpContext.Session.GetString(HomeController.SessionKeyState);
            }
        }

        #endregion

        #region 画面（Index・Login・Scroll・Logout）

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
        /// <remarks>
        /// 押されたボタンで分ける（saml2 → SAML2、external → OIDC、それ以外 → 通常ログイン）。
        /// </remarks>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (Request.Form.Keys.Any(x => x == "saml2"))
            {
                // 外部ログイン（SAML2）
                return await this.StartSaml2LoginAsync();
            }

            if (Request.Form.Keys.Any(x => x == "external"))
            {
                // 外部ログイン（OIDC）
                return this.StartOidcLogin();
            }

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

        #endregion

        #region 外部ログイン（OIDC）

        #region 要求

        /// <summary>OIDC の外部ログインを始める（認可要求へリダイレクトする）</summary>
        /// <returns>IActionResult</returns>
        /// <remarks>
        /// 認可コード フロー。応答は OAuth2AuthorizationCodeGrantClient が受ける。
        /// **prompt=none は付けない。** IdP にまだセッションが無いと、
        /// ログイン画面を出さずに login_required で戻ってしまう（OpenID Connect Core 1.0 3.1.2.1）。
        /// </remarks>
        private IActionResult StartOidcLogin()
        {
            return Redirect(CmnClientParams.SpRp_AuthRequestUri
                + "?client_id=" + Uri.EscapeDataString(OAuth2AndOIDCParams.ClientID)
                + "&response_type=code"
                + "&scope=" + Uri.EscapeDataString("openid profile email phone address")
                + "&state=" + Uri.EscapeDataString(this.State)
                + "&nonce=" + Uri.EscapeDataString(this.Nonce)
                + "&redirect_uri=" + Uri.EscapeDataString(CmnClientParams.SpRp_RedirectUri));
        }

        #endregion

        #region 応答（リダイレクト先）

        /// <summary>OAuth2AuthorizationCodeGrantClient（OIDC の応答を受ける口。SpRp_RedirectUri）</summary>
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

                            if (displayName != null) // UserInfo の sub が id_token と同じ（OpenID Connect Core 1.0 5.3.2）
                            {
                                // ログインに成功
                                //   **利用者の識別子は sub**（iss と組で一意・不変。OpenID Connect Core 1.0 5.7）。
                                await this.SignInExternalAsync(sub, displayName);

                                return this.Redirect(Url.Action("Index", "Home"));
                            }
                        }
                        else { }
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

        #endregion

        #region 検証

        /// <summary>相手（OP）の id_token を検証する</summary>
        /// <param name="id_token">id_token</param>
        /// <param name="access_token">access_token（at_hash の検証）</param>
        /// <param name="code">code（c_hash の検証）</param>
        /// <returns>sub（検証に失敗したら null）</returns>
        /// <remarks>
        /// **鍵は、ヘッダの kid で jwks_uri から引く**（SpRp_DiscoveryUri の Discovery。issuer は SpRp_Issuer と照合する）。
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

            string sub, nonce;
            JObject claims;
            if (IdToken.Verify(id_token, access_token, code, "",
                jws, CmnClientParams.Isser, OAuth2AndOIDCParams.ClientID,
                out sub, out nonce, out claims)
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

        /// <summary>OIDC の state・nonce を消す（1 度しか使わない）</summary>
        private void ClearExLoginsParams()
        {
            HttpContext.Session.SetString(HomeController.SessionKeyNonce, "");
            HttpContext.Session.SetString(HomeController.SessionKeyState, "");
        }

        #endregion

        #endregion

        #region 外部ログイン（SAML2）

        #region 要求

        /// <summary>SAML2 の外部ログインを始める（AuthnRequest を IdP へ送る）</summary>
        /// <returns>IActionResultを非同期的に返す</returns>
        /// <remarks>
        /// 応答は Saml2AssertionConsumerService が受ける。
        /// **IdP の SSO の口と署名鍵は、メタデータ（SpRp_Saml2MetadataUri）から引く**（entityID は SpRp_Saml2EntityId と照合する）。
        /// **要求は HTTP-Redirect で送り、応答も HTTP-Redirect で受ける**（ProtocolBinding）。
        /// **NameID は Persistent を求める**（SP ごとに固定の値。識別子にする。SAML Core 8.3.7）。
        /// 要求には SpRp_RsaPfxFilePath の鍵で署名する（IdP が WantAuthnRequestsSigned）。
        /// **Issuer は "http://" ＋ client_id**（汎用認証サイトは、Issuer から client_id を引く）。
        /// </remarks>
        private async Task<IActionResult> StartSaml2LoginAsync()
        {
            SAML2Metadata metadata = await SAML2Metadata.GetAsync();
            if (metadata == null || string.IsNullOrEmpty(metadata.SsoRedirectLocation))
            {
                // メタデータが引けない（IdP が動いていない、entityID が違う）
                return RedirectToAction("Login");
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

            HttpContext.Session.SetString(HomeController.SessionKeySaml2RelayState, relayState);
            HttpContext.Session.SetString(HomeController.SessionKeySaml2RequestId, id);

            return Redirect(metadata.SsoRedirectLocation + "?" + queryString);
        }

        #endregion

        #region 応答（ACS）

        /// <summary>Saml2AssertionConsumerService（SAML2 の応答を受ける口。SpRp_Saml2AcsUri。HTTP-Redirect）</summary>
        /// <returns>ActionResultを非同期的に返す</returns>
        /// <remarks>
        /// **検証は SAML2Client.VerifyResponseWithReason**（署名・Issuer・Audience・Recipient・InResponseTo・RelayState・期限）。
        /// **使ってよいかの判断は IsValid だけで行う。**
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Saml2AssertionConsumerService()
        {
            // **Redirect Binding は、クエリ文字列そのものが署名の対象**。受け取ったままの値を使う。
            string queryString = (Request.QueryString.Value ?? "").TrimStart('?');

            // 1 度しか使わない
            string expectedRelayState = HttpContext.Session.GetString(HomeController.SessionKeySaml2RelayState);
            string expectedRequestId = HttpContext.Session.GetString(HomeController.SessionKeySaml2RequestId);
            HttpContext.Session.SetString(HomeController.SessionKeySaml2RelayState, "");
            HttpContext.Session.SetString(HomeController.SessionKeySaml2RequestId, "");

            if (string.IsNullOrEmpty(expectedRelayState) || string.IsNullOrEmpty(expectedRequestId))
            {
                // 自分が送った要求への応答ではない
                return RedirectToAction("Login");
            }

            // **署名を検証する鍵は、メタデータの署名鍵**（ロールオーバー中は複数。順に試す）
            SAML2Metadata metadata = await SAML2Metadata.GetAsync();
            List<RSA> keys = (metadata == null) ? new List<RSA>() : metadata.GetSigningKeys();

            SAML2VerifyResult result = SAML2Client.VerifyResponseWithReason(
                queryString, null, keys,
                CmnClientParams.SpRp_Saml2EntityId, CmnClientParams.SpRp_Saml2AcsUri,
                expectedRequestId, Request.Query["RelayState"], expectedRelayState);

            //  **識別子は NameID**（IdP の entityID と組で使う）。**Transient は受けない**（毎回変わる。SAML Core 8.3.8）。
            if (!result.IsValid
                || string.IsNullOrEmpty(result.NameId)
                || result.NameIdFormat == SAML2Enum.NameIDFormat.Transient)
            {
                // ログインに失敗
                return RedirectToAction("Login");
            }

            // ログインに成功
            //   **表示名は属性の mail**（無ければ NameID）。
            string email = SAML2Attributes.GetEmail(result.Attributes);
            await this.SignInExternalAsync(result.NameId, string.IsNullOrEmpty(email) ? result.NameId : email);

            return this.Redirect(Url.Action("Index", "Home"));
        }

        #endregion

        #endregion

        #region 外部ログイン（共通）

        /// <summary>外部ログイン（OIDC・SAML2）の結果で、サイン インする</summary>
        /// <param name="id">利用者の識別子（OIDC は sub、SAML2 は NameID）</param>
        /// <param name="displayName">表示名（email）</param>
        /// <returns>Task</returns>
        /// <remarks>
        /// **識別子は ClaimTypes.Name / NameIdentifier**（UserStore のキーにするなら、これ）。
        /// **表示名は ClaimTypes.Email と MyUserInfo**（変更・再割り当てがありうるので、識別子にしない）。
        /// </remarks>
        private async Task SignInExternalAsync(string id, string displayName)
        {
            // 認証情報を作成する。
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, id));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, id));
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
        }

        #endregion
    }
}

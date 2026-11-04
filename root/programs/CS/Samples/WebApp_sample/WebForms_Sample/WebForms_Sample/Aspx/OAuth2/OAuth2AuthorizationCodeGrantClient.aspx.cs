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
//*  2026/11/04  玄人 幸道         今のライブラリと MPAS のスタブに合わせた（#649）
//*                                （トークン要求に redirect_uri・id_token を jwks_uri の鍵と iss / aud で検証・エラー応答で落ちない）
//*  2026/11/04  玄人 幸道         表示名を UserInfo の email にした（識別子は sub のまま。UserInfo の sub を id_token と照合。#649）
//**********************************************************************************

using System;
using System.Threading.Tasks;

using System.Web;
using System.Web.Security;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Security.Jwt;
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
            string iss = Request.QueryString["iss"]; // 認可応答の iss（RFC 9207）

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

                    // **エラーの応答には id_token が無い**（以前は、ここで KeyNotFoundException になっていた）
                    JObject token = JObject.Parse(response);
                    string id_token = (string)token[OAuth2AndOIDCConst.IDToken];
                    string access_token = (string)token[OAuth2AndOIDCConst.AccessToken];

                    // id_tokenの検証
                    string sub = string.IsNullOrEmpty(id_token)
                        ? null : await this.VerifyIdTokenAsync(id_token, access_token, code);

                    // /userinfoエンドポイントにアクセスし、表示名（email）を得る
                    //   UserInfo の sub が id_token と違えば null（OpenID Connect Core 1.0 5.3.2）
                    string displayName = string.IsNullOrEmpty(sub)
                        ? null : OAuth2AuthorizationCodeGrantClient.GetDisplayName(
                            await OAuth2AndOIDCClient.GetUserInfoAsync(
                                new Uri(CmnClientParams.SpRp_UserInfoUri), access_token), sub);

                    if (displayName != null)
                    {
                        // ログインに成功

                        //   **利用者の識別子は sub**（Forms 認証の名前。iss と組で一意・不変。OpenID Connect Core 1.0 5.7）。
                        //   **email は表示名**（MyUserInfo。変更・再割り当てがありうるので、識別子にしない）。
                        FormsAuthentication.RedirectFromLoginPage(sub, false);
                        MyUserInfo ui = new MyUserInfo(displayName, Request.UserHostAddress);
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

        /// <summary>ClearExLoginsParam</summary>
        private void ClearExLoginsParams()
        {
            Session["nonce"] = null;
            Session["state"] = null;
        }
    }
}
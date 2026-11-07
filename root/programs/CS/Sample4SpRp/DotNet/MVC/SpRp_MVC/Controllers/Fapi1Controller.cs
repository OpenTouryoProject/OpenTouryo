#region Apache License
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：Fapi1Controller
//* クラス日本語名  ：FAPI 1.0 の登録（client_mode = fapi1）で、認可コード フローを通す
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/07  玄人 幸道         新規作成（#668。汎用認証サイトの自己テストの FAPI1 を移した）
//*  2026/11/07  玄人 幸道         oauth2・oidc は private_key_jwt に PKCE を併せた（GetAccessTokenByCodeWithPkceAsync。#670）
//*  2026/11/07  玄人 幸道         Start は、照合した後に入力の文字列ではなく定数を使う（code scanning #59。振る舞いは変えない）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Pwd;
using Touryo.Infrastructure.Public.Util;

using SpRp_MVC.Auth;
using SpRp_MVC.Models;

namespace SpRp_MVC.Controllers
{
    /// <summary>FAPI 1.0 の登録で、認可コード フローを通す</summary>
    /// <remarks>
    /// **汎用認証サイト（MPAS）の自己テストの FAPI1 を、別の配備で動く本物のクライアントに移したもの**（#668）。
    /// 自己テストは 1 つの環境でサーバとクライアントを兼ねるので、**クライアント役の設定（`SpRp_*`）だけで通ることは、ここで確かめる。**
    ///
    /// | flow | client_id | scope | トークン要求のクライアント認証 |
    /// |---|---|---|---|
    /// | `oauth2` | `OAuth2AndOidcClientID` | `SpRp_Fapi1OAuth2Scope`（openid なし） | **`private_key_jwt`（RFC 7523 §2.2）＋ PKCE S256** |
    /// | `oidc` | 同上 | `SpRp_Scope`（openid あり。nonce を送る） | 同上 |
    /// | `pkce` | **`SpRp_Fapi1PkceClientID`** | `SpRp_Scope`（同上） | **無し**（PKCE S256 の code_verifier だけ。RFC 7636） |
    ///
    /// **MPAS は fapi1 の登録に、認可コードの引き換えで PKCE S256 か `private_key_jwt`（か mTLS）を求める**
    /// （client_secret では通らない。Hybrid（`code id_token`）も通らない）。
    /// **`pkce` は、秘密を持たない別のクライアントで通す。** `private_key_jwt` を登録したクライアント（コンフィデンシャル）は、
    /// **PKCE だけでは引き換えられない**（クライアント認証を省けない）。
    /// **`oauth2`・`oidc` は、`private_key_jwt` に PKCE を併せる**（`OAuth2AndOIDCClient.GetAccessTokenByCodeWithPkceAsync`。#670）。
    /// **どの flow も code_challenge（S256）を送り、トークン要求で code_verifier を送る**
    /// （IdP が code_verifier を検証すれば、コンフィデンシャル クライアントでも code の差し替えを防げる）。
    ///
    /// **response_mode は `query`（既定）か `query.jwt`（JARM）。** JARM を求めたのに素の応答が来たら、断る（素の応答へ落とさせない）。
    /// </remarks>
    public class Fapi1Controller : Controller
    {
        #region 定数

        /// <summary>flow : openid なし ＋ private_key_jwt ＋ PKCE</summary>
        public const string FlowOAuth2 = "oauth2";

        /// <summary>flow : openid ＋ private_key_jwt ＋ PKCE</summary>
        public const string FlowOidc = "oidc";

        /// <summary>flow : openid ＋ PKCE S256（クライアント認証なし）</summary>
        public const string FlowPkce = "pkce";

        /// <summary>response_mode : JARM（JARM §2.3.1）</summary>
        public const string ResponseModeQueryJwt = "query.jwt";

        /// <summary>セッションのキー（認可要求から戻りまでの間だけ置く）</summary>
        private const string SessionKey = "SpRp_MVC.Fapi1";

        #endregion

        #region 認可要求

        /// <summary>認可要求を組み立てて、IdP へリダイレクトする</summary>
        /// <param name="flow">oauth2 / oidc / pkce</param>
        /// <param name="responseMode">空（query）/ query.jwt</param>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Start(string flow = FlowOidc, string responseMode = "")
        {
            if (flow != FlowOAuth2 && flow != FlowOidc && flow != FlowPkce)
            {
                return this.BadRequest("flow は oauth2 / oidc / pkce のどれかです。");
            }

            responseMode = responseMode ?? "";
            if (responseMode != "" && responseMode != ResponseModeQueryJwt)
            {
                return this.BadRequest("responseMode は 空 / query.jwt のどちらかです。");
            }

            // **照合した後は、入力の文字列ではなく、定数を使う**（リダイレクト先の URL・セッションに、利用者の値を流さない）。
            //   許可リストで照合済みなので値は同じだが、入力の変数をそのまま連結すると、
            //   照合を経ていない値と区別が付かない（CodeQL の cs/web/unvalidated-url-redirection もそう読む）。
            string checkedFlow = (flow == FlowOAuth2) ? FlowOAuth2 : (flow == FlowPkce) ? FlowPkce : FlowOidc;
            string checkedMode = (responseMode == ResponseModeQueryJwt) ? ResponseModeQueryJwt : "";

            Discovery discovery = await Discovery.LoadAsync(
                CmnClientParams.SpRp_DiscoveryUri, CmnClientParams.Isser);

            // **state・nonce・code_verifier は、推測できない乱数**（RFC 6749 §10.12、RFC 7636 §7.1）
            Fapi1Session s = new Fapi1Session
            {
                Flow = checkedFlow,
                ClientId = Fapi1Controller.ClientIdOf(checkedFlow),
                ResponseMode = checkedMode,
                State = GetPassword.Base64UrlSecret(32),
                Nonce = checkedFlow == FlowOAuth2 ? null : GetPassword.Base64UrlSecret(32),
                // **どの flow も PKCE を付ける**（private_key_jwt の flow も併せる。#670）
                CodeVerifier = GetPassword.Base64UrlSecret(50),
            };

            Dictionary<string, string> query = new Dictionary<string, string>
            {
                { OAuth2AndOIDCConst.client_id, s.ClientId },
                { OAuth2AndOIDCConst.response_type, OAuth2AndOIDCConst.AuthorizationCodeResponseType },
                { OAuth2AndOIDCConst.scope, checkedFlow == FlowOAuth2
                    ? GetConfigParameter.GetConfigValue("SpRp_Fapi1OAuth2Scope")
                    : GetConfigParameter.GetConfigValue("SpRp_Scope") },
                { OAuth2AndOIDCConst.redirect_uri, CmnClientParams.SpRp_RedirectUri },
                { OAuth2AndOIDCConst.state, s.State },
            };

            if (s.Nonce != null)
            {
                query.Add(OAuth2AndOIDCConst.nonce, s.Nonce);
            }

            if (s.CodeVerifier != null)
            {
                query.Add(OAuth2AndOIDCConst.code_challenge, OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod(s.CodeVerifier));
                query.Add(OAuth2AndOIDCConst.code_challenge_method, OAuth2AndOIDCConst.PKCE_S256);
            }

            if (checkedMode != "")
            {
                query.Add(OAuth2AndOIDCConst.response_mode, checkedMode);
            }

            this.HttpContext.Session.SetString(SessionKey, JsonConvert.SerializeObject(s));

            return this.Redirect(discovery.AuthorizationEndpoint.AbsoluteUri
                + "?" + Fapi1Controller.BuildQuery(query));
        }

        #endregion

        #region 戻り（SpRp_RedirectUri）

        /// <summary>認可応答を受け、トークンを引き換えて確かめる</summary>
        /// <param name="code">code（素の応答）</param>
        /// <param name="state">state（素の応答）</param>
        /// <param name="error">error（素の応答）</param>
        /// <param name="iss">iss（素の応答。RFC 9207）</param>
        /// <param name="response">response（JARM の応答）</param>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Callback(
            string code, string state, string error, string iss, string response)
        {
            Fapi1Result result = new Fapi1Result();

            try
            {
                result.Error = await this.HandleCallbackAsync(result, code, state, error, iss, response);
            }
            catch (Exception ex)
            {
                // **読めない値（JWT の形でない、など）は、例外になることがある**（CmnJwtToken.Verify）。失敗として見せる。
                result.Error = "例外 : " + ex.GetType().Name + " : " + ex.Message;
            }

            result.Ok = result.Error == null;
            return this.View("Result", result);
        }

        /// <summary>戻りの処理の本体</summary>
        /// <returns>失敗の理由（成功なら null）</returns>
        private async Task<string> HandleCallbackAsync(Fapi1Result result,
            string code, string state, string error, string iss, string response)
        {
            #region 認可要求のときの値（1 回だけ使う）

            string saved = this.HttpContext.Session.GetString(SessionKey);
            this.HttpContext.Session.Remove(SessionKey);

            if (string.IsNullOrEmpty(saved))
            {
                return "認可要求の記録がありません（セッションが切れたか、このサンプルから始めていない）。";
            }

            Fapi1Session s = JsonConvert.DeserializeObject<Fapi1Session>(saved);
            result.Flow = s.Flow;
            result.ResponseMode = s.ResponseMode == "" ? "query" : s.ResponseMode;

            string issuer = CmnClientParams.Isser;
            string clientId = s.ClientId;

            #endregion

            #region 応答を読む（JARM なら検証してから）

            if (s.ResponseMode == ResponseModeQueryJwt)
            {
                // **JARM を求めたのに素の応答なら、断る**（署名の無い応答に落とさせない）
                if (string.IsNullOrEmpty(response))
                {
                    return "JARM（query.jwt）を求めたのに、response がありません（素の応答は受けない）。";
                }

                (string jarmError, JObject payload) = await OpJwt.VerifyResponseObjectAsync(response, issuer, clientId);
                if (jarmError != null)
                {
                    return jarmError;
                }

                // **検証した応答の中の値だけを使う**（クエリの code・state は見ない）
                code = (string)payload[OAuth2AndOIDCConst.code];
                state = (string)payload[OAuth2AndOIDCConst.state];
                error = (string)payload[OAuth2AndOIDCConst.error];
                iss = null; // JARM の iss は、ResponseObject.Verify が確かめた
            }
            else if (!string.IsNullOrEmpty(response))
            {
                return "JARM を求めていないのに、response が来ました。";
            }

            #endregion

            #region state・iss・error

            // **state は、送った値と一致するか**（RFC 6749 §10.12。CSRF）
            if (!string.Equals(state, s.State, StringComparison.Ordinal))
            {
                return "state が、認可要求で送った値と違います。";
            }

            // **応答の iss は、在れば確かめる**（RFC 9207 §2.4。IdP の取り違え）
            if (!string.IsNullOrEmpty(iss) && !string.Equals(iss, issuer, StringComparison.Ordinal))
            {
                return "応答の iss（" + iss + "）が、期待する値と違います。";
            }

            if (!string.IsNullOrEmpty(error))
            {
                return "IdP がエラーを返しました : " + error;
            }

            if (string.IsNullOrEmpty(code))
            {
                return "code がありません。";
            }

            #endregion

            #region トークン要求

            Discovery discovery = await Discovery.LoadAsync(CmnClientParams.SpRp_DiscoveryUri, issuer);
            string tokenResponse;

            if (s.Flow == FlowPkce)
            {
                // **PKCE の code_verifier だけで引き換える**（秘密は送らない。fapi1 の登録は client_secret を通さない）
                result.AuthMethod = "PKCE（S256）";
                tokenResponse = await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                    discovery.TokenEndpoint, clientId, null, CmnClientParams.SpRp_RedirectUri,
                    code, s.CodeVerifier);
            }
            else
            {
                // **private_key_jwt**（RFC 7523 §2.2。aud はトークン エンドポイント。§3）**＋ PKCE の code_verifier**（RFC 7636 §4.5。#670）
                result.AuthMethod = "private_key_jwt ＋ PKCE（S256）";
                tokenResponse = await OAuth2AndOIDCClient.GetAccessTokenByCodeWithPkceAsync(
                    discovery.TokenEndpoint, CmnClientParams.SpRp_RedirectUri, code, s.CodeVerifier,
                    Fapi1Controller.CreateClientAssertion(clientId, discovery.TokenEndpoint.AbsoluteUri));
            }

            JObject token = Fapi1Controller.ParseJson(tokenResponse);
            if (token == null)
            {
                return "トークン応答を JSON として読めません。";
            }

            if (token.ContainsKey(OAuth2AndOIDCConst.error))
            {
                return "トークン要求が断られました : " + (string)token[OAuth2AndOIDCConst.error]
                    + " " + (string)token[OAuth2AndOIDCConst.error_description];
            }

            string accessToken = (string)token[OAuth2AndOIDCConst.AccessToken];
            if (string.IsNullOrEmpty(accessToken))
            {
                return "access_token がありません。";
            }

            result.TokenType = (string)token[OAuth2AndOIDCConst.token_type];

            #endregion

            if (s.Flow == FlowOAuth2)
            {
                // **openid を求めていないので、id_token も UserInfo も無い**（access_token を得たところまで）
                return null;
            }

            #region id_token・UserInfo

            string idToken = (string)token[OAuth2AndOIDCConst.IDToken];
            if (string.IsNullOrEmpty(idToken))
            {
                return "openid を求めたのに、id_token がありません。";
            }

            (string idError, JObject claims) = await OpJwt.VerifyIdTokenAsync(
                idToken, accessToken, code, issuer, clientId, s.Nonce);
            if (idError != null)
            {
                return idError;
            }

            result.IdTokenClaims = claims.ToString(Formatting.Indented);

            JObject userInfo = Fapi1Controller.ParseJson(
                await OAuth2AndOIDCClient.GetUserInfoAsync(discovery.UserInfoEndpoint, accessToken));
            if (userInfo == null)
            {
                return "UserInfo の応答を JSON として読めません。";
            }

            // **エラーも JSON で返る**（RFC 6750 §3.1）ので、表示するだけでは成功と区別できない
            if (userInfo.ContainsKey(OAuth2AndOIDCConst.error))
            {
                return "UserInfo が断られました : " + (string)userInfo[OAuth2AndOIDCConst.error];
            }

            // **UserInfo の sub は、id_token の sub と一致しなければならない**（OpenID Connect Core 1.0 §5.3.2）
            if ((string)userInfo[OAuth2AndOIDCConst.sub] != (string)claims[OAuth2AndOIDCConst.sub])
            {
                return "UserInfo の sub が、id_token の sub と違います。";
            }

            result.UserInfo = userInfo.ToString(Formatting.Indented);

            #endregion

            return null;
        }

        #endregion

        #region 補助

        /// <summary>flow で使う client_id</summary>
        /// <param name="flow">flow</param>
        /// <returns>client_id</returns>
        private static string ClientIdOf(string flow)
        {
            return flow == FlowPkce
                ? GetConfigParameter.GetConfigValue("SpRp_Fapi1PkceClientID")
                : OAuth2AndOIDCParams.ClientID;
        }

        /// <summary>client_assertion（private_key_jwt）を作る</summary>
        /// <param name="clientId">client_id（iss・sub に入る）</param>
        /// <param name="aud">aud（トークン エンドポイント）</param>
        /// <returns>署名付き JWT</returns>
        /// <remarks>
        /// **鍵は SpRp_RsaPfxFilePath（クライアントの秘密鍵）。** IdP には、その公開鍵（JWK）を登録してある。
        /// **有効期間は短くする**（使い回されても、すぐ使えなくなるように）。
        /// </remarks>
        private static string CreateClientAssertion(string clientId, string aud)
        {
            DigitalSignX509 dsX509 = new DigitalSignX509(
                CmnClientParams.RsaPfxFilePath, CmnClientParams.RsaPfxPassword, HashAlgorithmName.SHA256);

            RSAParameters rsa = ((RSA)dsX509.AsymmetricAlgorithm).ExportParameters(true);

            return JwtAssertion.CreateByRsa(clientId, aud, TimeSpan.FromMinutes(1), "", rsa);
        }

        /// <summary>JSON を読む（読めなければ null）</summary>
        /// <param name="json">JSON</param>
        /// <returns>JObject</returns>
        private static JObject ParseJson(string json)
        {
            try
            {
                return JObject.Parse(json ?? "");
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        /// <summary>クエリ文字列を組み立てる（値は URL 符号化する）</summary>
        /// <param name="query">パラメタ</param>
        /// <returns>a=1&amp;b=2</returns>
        private static string BuildQuery(Dictionary<string, string> query)
        {
            List<string> pairs = new List<string>();
            foreach (KeyValuePair<string, string> kv in query)
            {
                pairs.Add(Uri.EscapeDataString(kv.Key) + "=" + Uri.EscapeDataString(kv.Value ?? ""));
            }
            return string.Join("&", pairs);
        }

        #endregion
    }
}

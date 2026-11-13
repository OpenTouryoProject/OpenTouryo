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
//* クラス名        ：Fapi2Controller
//* クラス日本語名  ：FAPI 2.0 の登録（client_mode = fapi2）で、認可コード フローを通す
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/09  玄人 幸道         新規作成（#668 の追加。汎用認証サイトの自己テストの FAPI2（CIBA を除く）を移した）
//*  2026/11/10  玄人 幸道         暗号化した id_token は JWE のまま IdToken.Verify に渡す（復号は鍵を引くためだけ）。par_mtls（tls_client_auth）を足した
//*  2026/11/13  玄人 幸道         id_token の検証のコメントを、今の部品（JWE の中でも RS256・ES256 を受ける。#683）に合わせた（#692）
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
using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Public.Security.Pwd;
using Touryo.Infrastructure.Public.Util;

using SpRp_MVC.Auth;
using SpRp_MVC.Models;

namespace SpRp_MVC.Controllers
{
    /// <summary>FAPI 2.0 の登録で、認可コード フローを通す</summary>
    /// <remarks>
    /// **汎用認証サイト（MPAS）の自己テストの FAPI2（CIBA を除く）を、別の配備で動く本物のクライアントに移したもの**（#668 の追加）。
    /// **認可要求は、署名した Request Object（JAR。RFC 9101）を先に預け、`client_id` ＋ `request_uri` だけでリダイレクトする。**
    ///
    /// | flow | 預け先 | 預けるときのクライアント認証 | トークン要求 |
    /// |---|---|---|---|
    /// | `par` | **PAR**（RFC 9126。`pushed_authorization_request_endpoint`） | `private_key_jwt` | `private_key_jwt` ＋ PKCE S256 |
    /// | `ros` | **MPAS 独自の `/ros`**（`request_object_endpoint`。後方互換の口で、RFC の口は PAR） | 無し（Request Object の署名だけ） | 同上 |
    /// | `par_mtls` | PAR | **`tls_client_auth`**（RFC 8705 §2。クライアント証明書。本文には client_id だけ） | **`tls_client_auth` ＋ PKCE S256** |
    ///
    /// - **Request Object には、PKCE の `code_challenge`（S256）と `exp`・`nbf`・`iat` を入れる**
    ///   （`RequestObject.Create` の有効期間・PKCE を受ける版。OpenTouryo#660・#675。FAPI 2.0 Security Profile §5.3.2.2・Message Signing §5.3.1）
    /// - **`aud` は、Request Object も `client_assertion` も、IdP の issuer**（RFC 9101 §4、FAPI 2.0 Security Profile §5.3.2.1）。
    ///   MPAS は、fapi2 の登録の `client_assertion` の `aud` を issuer に限る
    /// - **トークン要求は、方式を問わない版の `GetAccessTokenByCodeAsync`**（`client_assertion` ＋ `code_verifier`。OpenTouryo#676）
    /// - **response_mode は `query`（既定）か `query.jwt`（JARM）**（Request Object に入れる）。戻りの確かめは `Fapi1Controller` と同じ
    /// - **id_token は、暗号化されて返ることがある**（MPAS は、fapi2 の登録に公開鍵があると Nested JWT（JWE の中に JWS）にする。
    ///   Keycloak は、登録で暗号化を指定したとき。`Sample4SpRp/container/KCK`）。
    ///   自分の秘密鍵で復号してから、中の JWS を検証する
    ///
    /// **`par_mtls` は、クライアント証明書を求める IdP で通る**（スタブは `StubCompose.ps1 -Mtls`。MultiPurposeAuthSite#351）。
    /// 証明書は、起動のときに部品の HttpClient に載せる（`Program.CreateHttpClient`。`SpRp_ClientCertPfxFilePath`）。
    /// 求めない IdP（既定のスタブ）では、PAR が `invalid_client` で断られる。
    /// </remarks>
    public class Fapi2Controller : Controller
    {
        #region 定数

        /// <summary>flow : PAR に預ける（private_key_jwt）</summary>
        public const string FlowPar = "par";

        /// <summary>flow : /ros に預ける（MPAS 独自。署名だけ）</summary>
        public const string FlowRos = "ros";

        /// <summary>flow : PAR に預ける（tls_client_auth。クライアント証明書）</summary>
        public const string FlowParMtls = "par_mtls";

        /// <summary>Request Object の有効期間（FAPI 2.0 Message Signing §5.3.1 は、exp - nbf を 60 分以下に求める）</summary>
        private static readonly TimeSpan RequestObjectLifetime = TimeSpan.FromMinutes(5);

        /// <summary>セッションのキー（認可要求から戻りまでの間だけ置く）</summary>
        private const string SessionKey = "SpRp_MVC.Fapi2";

        #endregion

        #region 認可要求

        /// <summary>Request Object を預けて、IdP へリダイレクトする</summary>
        /// <param name="flow">par / ros / par_mtls</param>
        /// <param name="responseMode">空（query）/ query.jwt</param>
        /// <returns>IActionResult</returns>
        [HttpGet]
        public async Task<IActionResult> Start(string flow = FlowPar, string responseMode = "")
        {
            if (flow != FlowPar && flow != FlowRos && flow != FlowParMtls)
            {
                return this.BadRequest("flow は par / ros / par_mtls のどれかです。");
            }

            responseMode = responseMode ?? "";
            if (responseMode != "" && responseMode != Fapi1Controller.ResponseModeQueryJwt)
            {
                return this.BadRequest("responseMode は 空 / query.jwt のどちらかです。");
            }

            // **照合した後は、入力の文字列ではなく、定数を使う**（Fapi1Controller.Start と同じ。code scanning #59）
            string checkedFlow = (flow == FlowRos) ? FlowRos : (flow == FlowParMtls) ? FlowParMtls : FlowPar;
            string checkedMode = (responseMode == Fapi1Controller.ResponseModeQueryJwt) ? Fapi1Controller.ResponseModeQueryJwt : "";

            string issuer = CmnClientParams.Isser;
            Discovery discovery = await Discovery.LoadAsync(CmnClientParams.SpRp_DiscoveryUri, issuer);

            // **state・nonce・code_verifier は、推測できない乱数**（RFC 6749 §10.12、RFC 7636 §7.1）
            Fapi1Session s = new Fapi1Session
            {
                Flow = checkedFlow,
                ClientId = Fapi2Controller.ClientIdOf(checkedFlow),
                ResponseMode = checkedMode,
                State = GetPassword.Base64UrlSecret(32),
                Nonce = GetPassword.Base64UrlSecret(32),
                CodeVerifier = GetPassword.Base64UrlSecret(50),
            };

            Fapi1Result failed = new Fapi1Result
            {
                Profile = "FAPI 2.0",
                Flow = checkedFlow,
                PushedTo = Fapi2Controller.PushedToOf(checkedFlow),
                ResponseMode = checkedMode == "" ? "query" : checkedMode,
            };

            #region Request Object（JAR）を作る

            // **code_challenge_method は空で渡す**（RequestObject.Create が S256 を明示して入れる。RFC 7636 §4.3 では、省くと plain）
            string requestObject = RequestObject.Create(
                s.ClientId, issuer,
                OAuth2AndOIDCConst.AuthorizationCodeResponseType, checkedMode,
                Fapi2Controller.RedirectUri, GetConfigParameter.GetConfigValue("SpRp_Scope"), s.State, s.Nonce,
                "", "", "", new ClaimsInRO(null, null, null),
                OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod(s.CodeVerifier), "",
                Fapi2Controller.RequestObjectLifetime, Fapi2Controller.ClientRsaPrivateKey());

            #endregion

            #region 預ける

            string response;
            if (checkedFlow == FlowPar || checkedFlow == FlowParMtls)
            {
                if (discovery.PushedAuthorizationRequestEndpoint == null)
                {
                    failed.Error = "Discovery に pushed_authorization_request_endpoint がありません。";
                    return this.View("Result", failed);
                }

                // **PAR は、トークン エンドポイントと同じクライアント認証を付ける**（RFC 9126 §2）。client_id は方式によらず送る（§2.1）
                //   par_mtls は tls_client_auth（RFC 8705 §2）：本文には client_id だけ。証明書は HttpClient が載せる
                response = checkedFlow == FlowParMtls
                    ? await OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                        discovery.PushedAuthorizationRequestEndpoint, requestObject,
                        s.ClientId, null, null, OAuth2AndOIDCEnum.AuthMethods.tls_client_auth)
                    : await OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                        discovery.PushedAuthorizationRequestEndpoint, requestObject,
                        s.ClientId, null, Fapi1Controller.CreateClientAssertion(s.ClientId, issuer),
                        OAuth2AndOIDCEnum.AuthMethods.private_key_jwt);
            }
            else
            {
                if (discovery.RequestObjectEndpoint == null)
                {
                    failed.Error = "Discovery に request_object_endpoint（/ros）がありません。";
                    return this.View("Result", failed);
                }

                // **/ros は、署名付き JWT を生の本文で送る**（クライアント認証は無い。Request Object の署名で確かめられる）
                response = await OAuth2AndOIDCClient.RegisterRequestObjectAsync(
                    discovery.RequestObjectEndpoint, requestObject);
            }

            JObject pushed = Fapi1Controller.ParseJson(response);
            string requestUri = pushed == null ? null : (string)pushed[OAuth2AndOIDCConst.request_uri];
            if (string.IsNullOrEmpty(requestUri))
            {
                // **預けられなければ、リダイレクトしない**（IdP の応答をそのまま見せる）
                failed.Error = "Request Object を預けられませんでした : "
                    + (pushed == null ? "（JSON でない応答）" : (string)pushed[OAuth2AndOIDCConst.error] + " " + (string)pushed[OAuth2AndOIDCConst.error_description]);
                return this.View("Result", failed);
            }

            #endregion

            this.HttpContext.Session.SetString(SessionKey, JsonConvert.SerializeObject(s));

            // **認可要求は client_id と request_uri だけ**（RFC 9126 §4、RFC 9101 §5.2。値は預けた Request Object の中にある）
            return this.Redirect(discovery.AuthorizationEndpoint.AbsoluteUri + "?" + Fapi1Controller.BuildQuery(
                new Dictionary<string, string>
                {
                    { OAuth2AndOIDCConst.client_id, s.ClientId },
                    { OAuth2AndOIDCConst.request_uri, requestUri },
                }));
        }

        #endregion

        #region 戻り（SpRp_Fapi2RedirectUri）

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
            Fapi1Result result = new Fapi1Result { Profile = "FAPI 2.0" };

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

        /// <summary>戻りの処理の本体（Fapi1Controller と同じ確かめ方。違うのはトークン要求だけ）</summary>
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
            result.PushedTo = Fapi2Controller.PushedToOf(s.Flow);
            result.ResponseMode = s.ResponseMode == "" ? "query" : s.ResponseMode;

            string issuer = CmnClientParams.Isser;
            string clientId = s.ClientId;

            #endregion

            #region 応答を読む（JARM なら検証してから）

            if (s.ResponseMode == Fapi1Controller.ResponseModeQueryJwt)
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

            // **private_key_jwt（aud は issuer。FAPI 2.0 Security Profile §5.3.2.1）か tls_client_auth（RFC 8705 §2）＋ PKCE の code_verifier**（OpenTouryo#676 の版）
            bool mtls = s.Flow == FlowParMtls;
            result.AuthMethod = (mtls ? "tls_client_auth" : "private_key_jwt") + " ＋ PKCE（S256）";
            string tokenResponse = await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                discovery.TokenEndpoint,
                client_id: clientId, client_secret: null,
                client_assertion: mtls ? null : Fapi1Controller.CreateClientAssertion(clientId, issuer),
                redirect_uri: Fapi2Controller.RedirectUri, code: code, code_verifier: s.CodeVerifier,
                authMethod: mtls ? OAuth2AndOIDCEnum.AuthMethods.tls_client_auth : OAuth2AndOIDCEnum.AuthMethods.private_key_jwt);

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

            #region id_token・UserInfo

            string idToken = (string)token[OAuth2AndOIDCConst.IDToken];
            if (string.IsNullOrEmpty(idToken))
            {
                return "openid を求めたのに、id_token がありません。";
            }

            // **MPAS は、fapi2 の登録に公開鍵（jwk_rsa_publickey）があると、id_token を暗号化して返す**
            //   （Nested JWT：外側が JWE（RSA-OAEP・A256GCM。鍵はクライアントの公開鍵）、内側が IdP の署名した JWS。RFC 7516、OpenID Connect Core 1.0 §10.2）。
            //   **復号は、署名の鍵を引くため（中の JWS のヘッダの kid を読むため）だけに行う。** JWE のヘッダには、署名の鍵の kid が無い。
            //   **検証には、JWE のまま渡す**（IdToken.Verify（CmnJwtToken.Verify）が、自分の秘密鍵（SpRp_RsaPfxFilePath）で復号してから、中の JWS を検証する）。
            //   **中の JWS の alg は RS256・ES256 のどちらでもよい**（#683 から。それまでは、JWE の中なら ES256 だけを受けていた。
            //   Keycloak は、暗号化した id_token の中も RS256 で署名する。#692）。
            string keySource = null;
            if (idToken.Split('.').Length == 5)
            {
                JWE_RsaOaepAesGcm_X509 jwe = new JWE_RsaOaepAesGcm_X509(
                    CmnClientParams.RsaPfxFilePath, CmnClientParams.RsaPfxPassword);
                if (!jwe.Decrypt(idToken, out string innerJws) || string.IsNullOrEmpty(innerJws))
                {
                    return "id_token（JWE）を、自分の秘密鍵で復号できません。";
                }

                keySource = innerJws;
                result.IdTokenForm = "JWE（RSA-OAEP・A256GCM）の中の JWS（" + JwtReader.TryReadAlg(innerJws) + "）";
            }
            else
            {
                result.IdTokenForm = "JWS（" + JwtReader.TryReadAlg(idToken) + "）";
            }

            (string idError, JObject claims) = await OpJwt.VerifyIdTokenAsync(
                idToken, accessToken, code, issuer, clientId, s.Nonce, keySource);
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

        /// <summary>flow で使う client_id（fapi2 の登録。サンプル独自のキー）</summary>
        /// <param name="flow">flow</param>
        /// <returns>client_id</returns>
        /// <remarks>**par_mtls は、tls_client_auth を登録した別のクライアント**（登録の方法でしか通らない。MPAS#307）</remarks>
        private static string ClientIdOf(string flow)
        {
            return flow == FlowParMtls
                ? GetConfigParameter.GetConfigValue("SpRp_Fapi2MtlsClientID")
                : GetConfigParameter.GetConfigValue("SpRp_Fapi2ClientID");
        }

        /// <summary>flow の預け先（画面の表示）</summary>
        /// <param name="flow">flow</param>
        /// <returns>PAR / PAR（MTLS）/ /ros</returns>
        private static string PushedToOf(string flow)
        {
            return flow == FlowRos ? "/ros" : flow == FlowParMtls ? "PAR（tls_client_auth）" : "PAR（private_key_jwt）";
        }

        /// <summary>戻り先（サンプル独自のキー。FAPI1 の SpRp_RedirectUri とは別の登録）</summary>
        private static string RedirectUri
        {
            get { return GetConfigParameter.GetConfigValue("SpRp_Fapi2RedirectUri"); }
        }

        /// <summary>Request Object の署名鍵（SpRp_RsaPfxFilePath。client_assertion と同じ鍵）</summary>
        /// <returns>RSAParameters（秘密鍵を含む）</returns>
        private static RSAParameters ClientRsaPrivateKey()
        {
            DigitalSignX509 dsX509 = new DigitalSignX509(
                CmnClientParams.RsaPfxFilePath, CmnClientParams.RsaPfxPassword, HashAlgorithmName.SHA256);

            return ((RSA)dsX509.AsymmetricAlgorithm).ExportParameters(true);
        }

        #endregion
    }
}

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
//* クラス名        ：OAuth2AndOIDCClient（Grants）
//* クラス日本語名  ：OAuth2AndOIDCClient（ライブラリ）の基本フロー（認可コード・PKCE・Client Credentials・ROPC）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（OAuth2AndOIDCClient.cs から処理区分ごとに分割した）
//*  2026/11/07  玄人 幸道         アサーションと code_verifier を一緒に送る GetAccessTokenByCodeWithPkceAsync を追加（#670）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Web;
using System.Net.Http;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Security;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>OAuth2AndOIDCClient（ライブラリ）の基本フロー（認可コード・PKCE・Client Credentials・ROPC）</summary>
    public partial class OAuth2AndOIDCClient
    {
        #region Authorization Code（RFC 6749 4.1）

        /// <summary>
        /// Authentication Code : codeからAccess Tokenを取得する。
        /// </summary>
        /// <param name="tokenEndpointUri">TokenエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="redirect_uri">redirect_uri</param>
        /// <param name="code">code</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> GetAccessTokenByCodeAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string redirect_uri, string code,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            return await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                tokenEndpointUri, client_id, client_secret, redirect_uri, code, null, null, authMethod);
        }

        /// <summary>
        ///PKCE : code, code_verifierからAccess Tokenを取得する。
        /// </summary>
        /// <param name="tokenEndpointUri">TokenエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="redirect_uri">redirect_uri</param>
        /// <param name="code">code</param>
        /// <param name="code_verifier">code_verifier</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> GetAccessTokenByCodeAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string redirect_uri, string code, string code_verifier,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_post)
        {
            return await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                tokenEndpointUri, client_id, client_secret, redirect_uri, code, code_verifier, null, authMethod);
        }

        /// <summary>
        /// FAPI1 : code, assertionからAccess Tokenを取得する。
        /// </summary>
        /// <param name="tokenEndpointUri">TokenエンドポイントのUri</param>
        /// <param name="redirect_uri">redirect_uri</param>
        /// <param name="code">code</param>
        /// <param name="assertion">assertion</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>
        /// アサーションは、RFC 7523 §2.2 のclient_assertion_type ＋ client_assertionと、
        /// 従来のassertionの両方で送る。どちらを読む認可サーバにも繋がる。
        /// </remarks>
        public static async Task<string> GetAccessTokenByCodeAsync(
            Uri tokenEndpointUri, string redirect_uri, string code, string assertion,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.private_key_jwt)
        {
            return await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                tokenEndpointUri, null, null, redirect_uri, code, null, assertion, authMethod);
        }

        /// <summary>
        /// PKCE ＋ アサーション : code, code_verifier, assertionからAccess Tokenを取得する。
        /// </summary>
        /// <param name="tokenEndpointUri">TokenエンドポイントのUri</param>
        /// <param name="redirect_uri">redirect_uri</param>
        /// <param name="code">code</param>
        /// <param name="code_verifier">code_verifier（必須）</param>
        /// <param name="assertion">assertion（クライアント認証のアサーション。必須）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>
        /// **private_key_jwt（RFC 7523 §2.2）でクライアント認証し、PKCE（RFC 7636 §4.5）の code_verifier も送る**（#670）。
        /// code_challenge を付けて認可を求めたコードは、トークン要求で code_verifier が要るため、
        /// アサーションで認証するクライアントが PKCE を併用するときに使う。
        ///
        /// - 本文は、アサーションの版（code・assertion を受ける GetAccessTokenByCodeAsync）と同じで、code_verifier を足したもの。
        ///   アサーションは client_assertion_type ＋ client_assertion と、従来の assertion の両方で送る
        /// - **code_verifier・assertion は、どちらも必須**（空なら ArgumentNullException。送らない）。
        ///   PKCE を使わないなら、アサーションの版を使う
        /// - **認証の方法（authMethod）は受けない。** 本文の形は、アサーションを共有鍵で署名した client_secret_jwt でも同じで、
        ///   どちらになるかは、呼び出し側が作るアサーション（alg と鍵）で決まる
        /// - **既存の版と同じ名前にしなかった**のは、(Uri, string × 4, AuthMethods) の版がすでに在り、
        ///   引数の型の並びが同じ版を足せないため（引数名と既定値は、シグネチャに含まれない）
        /// </remarks>
        public static async Task<string> GetAccessTokenByCodeWithPkceAsync(
            Uri tokenEndpointUri, string redirect_uri, string code, string code_verifier, string assertion)
        {
            if (string.IsNullOrEmpty(code_verifier))
            {
                throw new ArgumentNullException(nameof(code_verifier));
            }
            if (string.IsNullOrEmpty(assertion))
            {
                throw new ArgumentNullException(nameof(assertion));
            }

            return await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                tokenEndpointUri, null, null, redirect_uri, code, code_verifier, assertion,
                OAuth2AndOIDCEnum.AuthMethods.private_key_jwt);
        }

        /// <summary>
        /// code, etc. からAccess Tokenを取得する。
        /// </summary>
        /// <param name="tokenEndpointUri">TokenエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="redirect_uri">redirect_uri</param>
        /// <param name="code">code</param>
        /// <param name="code_verifier">code_verifier</param>
        /// <param name="assertion">assertion</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列</returns>
        private static async Task<string> GetAccessTokenByCodeAsync(Uri tokenEndpointUri,
            string client_id, string client_secret, string redirect_uri,
            string code, string code_verifier, string assertion,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            // 4.1.3.  アクセストークンリクエスト
            // http://openid-foundation-japan.github.io/rfc6749.ja.html#token-req

            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = tokenEndpointUri,
            };

            if (string.IsNullOrEmpty(code_verifier) && string.IsNullOrEmpty(assertion))
            {
                // 通常のアクセストークン・リクエスト
                Dictionary<string, string> body = new Dictionary<string, string>
                {
                    { OAuth2AndOIDCConst.grant_type, OAuth2AndOIDCConst.AuthorizationCodeGrantType },
                    { OAuth2AndOIDCConst.code, code },
                    { OAuth2AndOIDCConst.redirect_uri, HttpUtility.HtmlEncode(redirect_uri) },
                };

                // 認証情報の付加
                if (authMethod == OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
                {
                    httpRequestMessage.Headers.Authorization
                        = AuthenticationHeader.CreateBasicAuthenticationHeaderValue(client_id, client_secret);
                }
                else if (authMethod == OAuth2AndOIDCEnum.AuthMethods.client_secret_post)
                {
                    body.Add(OAuth2AndOIDCConst.client_id, client_id);
                    body.Add(OAuth2AndOIDCConst.client_secret, client_secret);
                }
                else
                {
                    throw new ArgumentException(
                        PublicExceptionMessage.ARGUMENT_INCORRECT, "authMethod");
                }

                httpRequestMessage.Content = new FormUrlEncodedContent(body);
            }
            else if (!string.IsNullOrEmpty(code_verifier) &&
                authMethod == OAuth2AndOIDCEnum.AuthMethods.client_secret_post)
            {
                // OAuth PKCEのアクセストークン・リクエスト
                httpRequestMessage.Content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        { OAuth2AndOIDCConst.grant_type, OAuth2AndOIDCConst.AuthorizationCodeGrantType },
                        { OAuth2AndOIDCConst.code, code },
                        { OAuth2AndOIDCConst.client_id, client_id },
                        { OAuth2AndOIDCConst.code_verifier, code_verifier },
                        { OAuth2AndOIDCConst.redirect_uri, HttpUtility.HtmlEncode(redirect_uri) },
                    });
            }
            else if (!string.IsNullOrEmpty(assertion) &&
                authMethod == OAuth2AndOIDCEnum.AuthMethods.private_key_jwt)
            {
                // FAPI1のアクセストークン・リクエスト
                //
                // クライアント認証のアサーションは、RFC 7523 §2.2 では
                // client_assertion_type ＋ client_assertion で送る。
                // 従来のassertionしか読まない認可サーバも在るため、両方送る。
                // grant_typeはauthorization_codeなので、
                // JWT Bearerグラント（§2.1）のassertionとして解釈されることはない。
                Dictionary<string, string> body = new Dictionary<string, string>
                {
                    { OAuth2AndOIDCConst.grant_type, OAuth2AndOIDCConst.AuthorizationCodeGrantType },
                    { OAuth2AndOIDCConst.code, code },
                    { OAuth2AndOIDCConst.assertion, assertion },
                    { OAuth2AndOIDCConst.client_assertion_type, OAuth2AndOIDCConst.JwtBearerClientAssertionType },
                    { OAuth2AndOIDCConst.client_assertion, assertion },
                    { OAuth2AndOIDCConst.redirect_uri, HttpUtility.HtmlEncode(redirect_uri) },
                };

                // **PKCE を併用するなら、code_verifier も送る**（RFC 7636 §4.5。#670）。
                //   ここに code_verifier を持って来るのは GetAccessTokenByCodeWithPkceAsync だけ
                //   （アサーションの版は null を渡す）なので、既存の版の本文は変わらない。
                if (!string.IsNullOrEmpty(code_verifier))
                {
                    body.Add(OAuth2AndOIDCConst.code_verifier, code_verifier);
                }

                httpRequestMessage.Content = new FormUrlEncodedContent(body);
            }
            else
            {
                throw new ArgumentException(
                    PublicExceptionMessage.ARGUMENT_INCORRECT, "code_verifier, assertion, authMethod");
            }

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion

        #region PKCE（RFC 7636）

        /// <summary>
        /// code_challenge_method=S256
        /// BASE64URL-ENCODE(SHA256(ASCII(code_verifier)))</summary>
        /// <param name="code_verifier">string</param>
        /// <returns>code_challenge</returns>
        public static string PKCE_S256_CodeChallengeMethod(string code_verifier)
        {
            return CustomEncode.ToBase64UrlString(GetHash.GetHashBytes(CustomEncode.StringToByte(
                code_verifier, CustomEncode.us_ascii), EnumHashAlgorithm.SHA256));
        }

        #endregion

        #region Client Credentials（RFC 6749 4.4）

        /// <summary>
        /// Client Credentials Grant
        /// </summary>
        /// <param name="tokenEndpointUri">TokenエンドポイントのUri</param>
        /// <param name="client_id">string</param>
        /// <param name="client_secret">string</param>
        /// <param name="scopes">string</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> ClientCredentialsGrantAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string scopes)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = tokenEndpointUri,
            };

            // HttpRequestMessage (Headers & Content)
            httpRequestMessage.Headers.Authorization = 
                AuthenticationHeader.CreateBasicAuthenticationHeaderValue(client_id, client_secret);

            httpRequestMessage.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { OAuth2AndOIDCConst.grant_type, OAuth2AndOIDCConst.ClientCredentialsGrantType },
                    { OAuth2AndOIDCConst.scope, scopes },
                });

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion

        #region Resource Owner Password Credentials（RFC 6749 4.3）

        /// <summary>
        /// Resource Owner Password Credentials Grant
        /// </summary>
        /// <param name="tokenEndpointUri">TokenエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="userId">userId</param>
        /// <param name="password">password</param>
        /// <param name="scopes">scopes</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> ResourceOwnerPasswordCredentialsGrantAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string userId, string password, string scopes)
        {
            // 4.1.3.  アクセストークンリクエスト
            // http://openid-foundation-japan.github.io/rfc6749.ja.html#token-req

            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = tokenEndpointUri,
            };

            // HttpRequestMessage (Headers & Content)
            httpRequestMessage.Headers.Authorization = 
                AuthenticationHeader.CreateBasicAuthenticationHeaderValue(client_id, client_secret);

            httpRequestMessage.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { OAuth2AndOIDCConst.grant_type, OAuth2AndOIDCConst.ResourceOwnerPasswordCredentialsGrantType },
                    { "username", userId },
                    { "password", password },
                    { OAuth2AndOIDCConst.scope, scopes },
                });

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion
    }
}

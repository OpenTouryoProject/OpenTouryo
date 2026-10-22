//**********************************************************************************
//* Copyright (C) 2017 Hitachi Solutions,Ltd.
//**********************************************************************************

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
//* クラス名        ：OAuth2AndOIDCClient
//* クラス日本語名  ：OAuth2AndOIDCClient（ライブラリ）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/04/24  西野 大介         新規
//*  2018/08/10  西野 大介         汎用認証サイトからのコード移行
//*  2019/08/01  西野 大介         client_secret_postのサポートを追加
//*  2020/03/04  西野 大介         FAPI CIBAの認可リクエスト（WebAPI）を追加
//*  2020/12/18  西野 大介         Device AuthZの認可リクエスト（WebAPI）を追加
//*  2026/09/25  玄人 幸道         PAR（RFC 9126）とCIBAのRequest Object直接送信を追加
//*  2026/09/25  玄人 幸道         FAPI 2.0向けにprivate_key_jwt / tls_client_authを追加
//*  2026/09/26  玄人 幸道         private_key_jwtのトークン要求で、client_assertionも併せて送る
//*  2026/10/22  玄人 幸道         HttpClientのget・SendAsync、DPoP（RFC 9449）で送る口、
//*                                DCR（RFC 7591 / 7592）のクライアント側を追加（#634）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace Touryo.Infrastructure.Framework.Authentication
{
    // ライブラリ内でawaitする場合は、ConfigureAwait(false)を使う。

    /// <summary>OAuth2AndOIDCClient（ライブラリ）</summary>
    public class OAuth2AndOIDCClient
    {
        /// <summary>HttpClient</summary>
        private static HttpClient _HttpClient = null;

        /// <summary>HttpClient</summary>
        /// <remarks>
        /// get は #634（H-1）で足した。利用側が、自分の渡した共有の HttpClient
        /// （TLS・プロキシ・クライアント証明書の扱いを揃えたもの）を、他のサーバ間通信にも使えるように。
        /// </remarks>
        public static HttpClient HttpClient
        {
            get
            {
                return OAuth2AndOIDCClient._HttpClient;
            }
            set
            {
                OAuth2AndOIDCClient._HttpClient = value;
            }
        }

        /// <summary>この HttpClient で送る</summary>
        /// <param name="httpRequestMessage">HttpRequestMessage</param>
        /// <returns>HttpResponseMessage</returns>
        /// <remarks>
        /// 自由に利用できる（#634 の H-1）。
        /// ライブラリが口を持たない要求（ヘッダを足したい・状態コードを見たいなど）を、
        /// 同じ HttpClient で送るために使う。
        /// </remarks>
        public static async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
        {
            if (httpRequestMessage == null)
            {
                throw new ArgumentNullException(nameof(httpRequestMessage));
            }

            return await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
        }

        #region 基本 4 フローのWebAPI

        #region Authentication Code

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
                httpRequestMessage.Content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        { OAuth2AndOIDCConst.grant_type, OAuth2AndOIDCConst.AuthorizationCodeGrantType },
                        { OAuth2AndOIDCConst.code, code },
                        { OAuth2AndOIDCConst.assertion, assertion },
                        { OAuth2AndOIDCConst.client_assertion_type, OAuth2AndOIDCConst.JwtBearerClientAssertionType },
                        { OAuth2AndOIDCConst.client_assertion, assertion },
                        { OAuth2AndOIDCConst.redirect_uri, HttpUtility.HtmlEncode(redirect_uri) },
                    });
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

        #region Client Credentials Grant

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

        #region Resource Owner Password Credentials Grant

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

        #endregion

        #region その他の 基本 WebAPI

        #region Refresh Token

        /// <summary>Refresh Tokenを使用してAccess Tokenを更新</summary>
        /// <param name="tokenEndpointUri">tokenEndpointUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="refreshToken">refreshToken</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> UpdateAccessTokenByRefreshTokenAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string refreshToken)
        {
            // 6.  アクセストークンの更新
            // http://openid-foundation-japan.github.io/rfc6749.ja.html#token-refresh

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
                    { OAuth2AndOIDCConst.grant_type, OAuth2AndOIDCConst.RefreshTokenGrantType },
                    { OAuth2AndOIDCConst.RefreshToken, refreshToken },
                });

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion

        #region UserInfo

        /// <summary>認可したユーザのClaim情報を取得するWebAPIを呼び出す</summary>
        /// <param name="userInfoEndpointUri">Uri</param>
        /// <param name="accessToken">accessToken</param>
        /// <returns>結果のJSON文字列（認可したユーザのClaim情報）</returns>
        public static async Task<string> GetUserInfoAsync(Uri userInfoEndpointUri, string accessToken)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = userInfoEndpointUri,
            };

            // HttpRequestMessage (Headers)
            httpRequestMessage.Headers.Authorization = AuthenticationHeader.CreateBearerAuthenticationHeaderValue(accessToken);

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region OAuth2拡張

        #region PKCE

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

        #region Revoke & Introspect

        /// <summary>Revokeエンドポイントで、Tokenを無効化する。</summary>
        /// <param name="revokeTokenEndpointUri">RevokeエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="token">token</param>
        /// <param name="token_type_hint">token_type_hint</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> RevokeTokenAsync(
            Uri revokeTokenEndpointUri, string client_id, string client_secret, string token, string token_type_hint,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = revokeTokenEndpointUri,
            };

            if (authMethod == OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
            {
                // HttpRequestMessage (Headers & Content)
                httpRequestMessage.Headers.Authorization =
                    AuthenticationHeader.CreateBasicAuthenticationHeaderValue(client_id, client_secret);

                httpRequestMessage.Content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        { OAuth2AndOIDCConst.token, token },
                        { OAuth2AndOIDCConst.token_type_hint, token_type_hint },
                    });
            }
            else if (authMethod == OAuth2AndOIDCEnum.AuthMethods.client_secret_post)
            {
                // HttpRequestMessage (Content)
                httpRequestMessage.Content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        { OAuth2AndOIDCConst.client_id, client_id },
                        { OAuth2AndOIDCConst.client_secret, client_secret },
                        { OAuth2AndOIDCConst.token, token },
                        { OAuth2AndOIDCConst.token_type_hint, token_type_hint },
                    });
            }
            else
            {
                throw new ArgumentException(
                    PublicExceptionMessage.ARGUMENT_INCORRECT, "authMethod");
            }
            

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>Introspectエンドポイントで、Tokenを無効化する。</summary>
        /// <param name="introspectTokenEndpointUri">IntrospectエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="token">token</param>
        /// <param name="token_type_hint">token_type_hint</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> IntrospectTokenAsync(
            Uri introspectTokenEndpointUri, string client_id, string client_secret, string token, string token_type_hint,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = introspectTokenEndpointUri,
            };

            if (authMethod == OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
            {
                // HttpRequestMessage (Headers & Content)
                httpRequestMessage.Headers.Authorization =
                    AuthenticationHeader.CreateBasicAuthenticationHeaderValue(client_id, client_secret);

                httpRequestMessage.Content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        { OAuth2AndOIDCConst.token, token },
                        { OAuth2AndOIDCConst.token_type_hint, token_type_hint },
                    });
            }
            else if (authMethod == OAuth2AndOIDCEnum.AuthMethods.client_secret_post)
            {
                // HttpRequestMessage (Content)
                httpRequestMessage.Content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                        { OAuth2AndOIDCConst.client_id, client_id },
                        { OAuth2AndOIDCConst.client_secret, client_secret },
                        { OAuth2AndOIDCConst.token, token },
                        { OAuth2AndOIDCConst.token_type_hint, token_type_hint },
                    });
            }
            else
            {
                throw new ArgumentException(
                    PublicExceptionMessage.ARGUMENT_INCORRECT, "authMethod");
            }

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion

        #region JWT Bearer Token Flow

        /// <summary>
        /// Token2エンドポイントで、
        /// JWT bearer token authorizationグラント種別の要求を行う。</summary>
        /// <param name="token2EndpointUri">Token2エンドポイントのUri</param>
        /// <param name="assertion">string</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> JwtBearerTokenFlowAsync(Uri token2EndpointUri, string assertion)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = token2EndpointUri,
            };

            // HttpRequestMessage (Headers & Content)

            // httpRequestMessage.Headers.Authorization = // ヘッダを使わない。

            httpRequestMessage.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { OAuth2AndOIDCConst.grant_type, OAuth2AndOIDCConst.JwtBearerTokenFlowGrantType },
                    { OAuth2AndOIDCConst.assertion, assertion },
                });

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion

        #region Device AuthZ
        /// <summary>Device AuthZの認可リクエスト（WebAPI）</summary>
        /// <param name="deviceAuthZUri">Uri</param>
        /// <param name="clientId">string</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> DeviceAuthZRequestAsync(Uri deviceAuthZUri, string clientId)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = deviceAuthZUri,
            };

            httpRequestMessage.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { OAuth2AndOIDCConst.client_id, clientId }
                });

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>Device AuthZのTokenリクエスト</summary>
        /// <param name="tokenEndpointUri">Uri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="device_code">string</param>    
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> GetAccessTokenByDeviceAuthZAsync(
            Uri tokenEndpointUri, string client_id, string device_code)
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

            // body
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                { OAuth2AndOIDCConst.grant_type, OAuth2AndOIDCConst.DeviceAuthZGrantType },
                { OAuth2AndOIDCConst.client_id, client_id },
                { OAuth2AndOIDCConst.device_code, device_code }
            };

            // 認証情報の付加（不要

            httpRequestMessage.Content = new FormUrlEncodedContent(body);

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        #endregion

        #endregion

        #region OpenID Connect

        /// <summary>JwkSetのWebAPIを呼び出す</summary>
        /// <param name="jwkSetEndpointUri">Uri</param>
        /// <returns>JwkSetのJSON文字列</returns>
        public static async Task<string> GetJwkSetAsync(Uri jwkSetEndpointUri)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = jwkSetEndpointUri,
            };

            // HttpResponseMessage
            
            // ピーキーな jwks_uri 実装に例外処理を追加
            if (OAuth2AndOIDCClient._HttpClient == null)
            {
                // HttpClientが無い。
                Debug.WriteLine("HttpClient is not set in OAuth2AndOIDCClient.GetJwkSetAsync method.");
            }
            else
            {
                try
                {
                    httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
                    return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // HttpClientで失敗。
                    Debug.WriteLine("Exception was catched in OAuth2AndOIDCClient.GetJwkSetAsync method: " + ex.ToString());
                }
            }

            return ""; // 空
        }

        #endregion

        #region FAPI (Financial-grade API) 

        #region クライアント認証

        /// <summary>クライアント認証を付加する</summary>
        /// <param name="httpRequestMessage">HttpRequestMessage</param>
        /// <param name="body">Dictionary（本文。client_idと資格情報を追加する）</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <param name="alwaysSetClientId">bool（認証方式に依らずclient_idを本文に載せるか）</param>
        /// <remarks>
        /// FAPI 2.0 は、秘密ベースの方式（client_secret_basic / client_secret_post）をprofileから外し、
        /// private_key_jwt と MTLS（tls_client_auth）だけを認めている。
        /// 
        /// client_idを本文に載せるかは、エンドポイントによって違う。
        /// PARは認可リクエストのパラメタを送る口なので、client_idが必須（RFC 9126 §2.1）だが、
        /// CIBAのバックチャネル認証は「認証方式が要求するパラメタ以外をJWTの外に置いてはならない」
        /// （CIBA Core 1.0 §7.1.1）ため、client_secret_postとtls_client_authのときだけになる。
        /// そこで、常に載せるかどうかをalwaysSetClientIdで受け取る。
        /// 
        /// tls_client_authは、クライアント証明書がTLSの層で示されている必要があるため、
        /// HttpClientプロパティに、証明書を載せたHttpClientHandlerのHttpClientを設定しておく
        /// （証明書は、CmnClientParams.ClientCertPfxFilePath / ClientCertPfxPasswordで取得できる）。
        /// HttpClientプロパティはstaticなので、差し替えはプロセス全体に効く。
        /// 1つのプロセスが複数のクライアントとして振る舞い、一部だけmTLSという構成では、
        /// 差し替えの順序に注意する。
        /// </remarks>
        private static void SetClientAuthentication(
            HttpRequestMessage httpRequestMessage, Dictionary<string, string> body,
            string client_id, string client_secret, string client_assertion,
            OAuth2AndOIDCEnum.AuthMethods authMethod, bool alwaysSetClientId)
        {
            if (alwaysSetClientId)
            {
                body[OAuth2AndOIDCConst.client_id] = client_id;
            }

            if (authMethod == OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
            {
                // RFC 6749 §2.3.1
                httpRequestMessage.Headers.Authorization
                    = AuthenticationHeader.CreateBasicAuthenticationHeaderValue(client_id, client_secret);
            }
            else if (authMethod == OAuth2AndOIDCEnum.AuthMethods.client_secret_post)
            {
                // RFC 6749 §2.3.1（この方式はclient_idも本文で送る）
                body[OAuth2AndOIDCConst.client_id] = client_id;
                body[OAuth2AndOIDCConst.client_secret] = client_secret;
            }
            else if (authMethod == OAuth2AndOIDCEnum.AuthMethods.private_key_jwt
                || authMethod == OAuth2AndOIDCEnum.AuthMethods.client_secret_jwt)
            {
                // RFC 7523 §2.2（署名の鍵が違うだけで、送り方は同じ）
                body[OAuth2AndOIDCConst.client_assertion_type] = OAuth2AndOIDCConst.JwtBearerClientAssertionType;
                body[OAuth2AndOIDCConst.client_assertion] = client_assertion;
            }
            else if (authMethod == OAuth2AndOIDCEnum.AuthMethods.tls_client_auth)
            {
                // RFC 8705 §2.1（本文はclient_idだけで、証明書はTLSの層で示す）
                body[OAuth2AndOIDCConst.client_id] = client_id;
            }
            else
            {
                throw new ArgumentException(
                    PublicExceptionMessage.ARGUMENT_INCORRECT, nameof(authMethod));
            }
        }

        #endregion

        #region PAR (Pushed Authorization Requests)
        /// <summary>RequestObjectを登録する</summary>
        /// <param name="requestObjectRegUri">Uri</param>
        /// <param name="requestObject">string</param>
        /// <returns>RequestObjectの登録結果</returns>
        /// <remarks>
        /// Request Object（署名付きJWT）を、生の本文としてPOSTし、request_uriを得る。
        /// RFC 9101（JAR）§5.2.1 が認可サーバによるこの預け先を認めており、この形で正しい。
        /// 本文の形式もクライアント認証も異なるPAR（RFC 9126）には、
        /// PushAuthorizationRequestAsyncメソッドを使用する。
        /// </remarks>
        public static async Task<string> RegisterRequestObjectAsync(
            Uri requestObjectRegUri, string requestObject)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = requestObjectRegUri,
            };

            httpRequestMessage.Content = new StringContent(requestObject);

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>認可リクエストをPushする（PAR : RFC 9126）</summary>
        /// <param name="authRequestPushUri">Uri（PARエンドポイント）</param>
        /// <param name="requestObject">string（Request Object＝署名付きJWT）</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（tls_client_authではnull）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列（request_uri と expires_in）</returns>
        /// <remarks>
        /// Request Objectをrequestパラメタに載せて送る形（RFC 9126 §3、RFC 9101）。
        /// private_key_jwt / client_secret_jwtには、client_assertionを取るオーバーロードを使用する。
        /// </remarks>
        public static async Task<string> PushAuthorizationRequestAsync(
            Uri authRequestPushUri, string requestObject,
            string client_id, string client_secret,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            return await OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                authRequestPushUri, requestObject,
                client_id, client_secret, null, authMethod).ConfigureAwait(false);
        }

        /// <summary>認可リクエストをPushする（PAR : RFC 9126）</summary>
        /// <param name="authRequestPushUri">Uri（PARエンドポイント）</param>
        /// <param name="requestObject">string（Request Object＝署名付きJWT）</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列（request_uri と expires_in）</returns>
        /// <remarks>Request Objectをrequestパラメタに載せて送る形（RFC 9126 §3、RFC 9101）。</remarks>
        public static async Task<string> PushAuthorizationRequestAsync(
            Uri authRequestPushUri, string requestObject,
            string client_id, string client_secret, string client_assertion,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            return await OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                authRequestPushUri,
                new Dictionary<string, string>
                {
                    { OAuth2AndOIDCConst.request, requestObject }
                },
                client_id, client_secret, client_assertion, authMethod).ConfigureAwait(false);
        }

        /// <summary>認可リクエストをPushする（PAR : RFC 9126）</summary>
        /// <param name="authRequestPushUri">Uri（PARエンドポイント）</param>
        /// <param name="authZRequestParams">Dictionary（認可リクエストのパラメタ）</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（tls_client_authではnull）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列（request_uri と expires_in）</returns>
        /// <remarks>
        /// private_key_jwt / client_secret_jwtには、client_assertionを取るオーバーロードを使用する。
        /// </remarks>
        public static async Task<string> PushAuthorizationRequestAsync(
            Uri authRequestPushUri, Dictionary<string, string> authZRequestParams,
            string client_id, string client_secret,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            return await OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                authRequestPushUri, authZRequestParams,
                client_id, client_secret, null, authMethod).ConfigureAwait(false);
        }

        /// <summary>認可リクエストをPushする（PAR : RFC 9126）</summary>
        /// <param name="authRequestPushUri">Uri（PARエンドポイント）</param>
        /// <param name="authZRequestParams">Dictionary（認可リクエストのパラメタ）</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列（request_uri と expires_in）</returns>
        /// <remarks>
        /// PARは、認可エンドポイントへ送るはずのパラメタを、
        /// Tokenエンドポイントと同じクライアント認証を付けてPOSTする（§2）。
        /// FAPI 2.0 は、この認証をprivate_key_jwtかMTLS（tls_client_auth）に限っている。
        /// client_idは認可リクエストの必須パラメタなので、認証方式に依らず本文に載せる（§2.1）。
        /// 応答のrequest_uriを、認可リクエストのrequest_uriパラメタに指定して使用する。
        /// </remarks>
        public static async Task<string> PushAuthorizationRequestAsync(
            Uri authRequestPushUri, Dictionary<string, string> authZRequestParams,
            string client_id, string client_secret, string client_assertion,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = authRequestPushUri,
            };

            // body（引数のDictionaryは変更しない）
            Dictionary<string, string> body = new Dictionary<string, string>(authZRequestParams);

            // 認証情報の付加（PARはclient_idが必須 : RFC 9126 §2.1）
            OAuth2AndOIDCClient.SetClientAuthentication(
                httpRequestMessage, body, client_id, client_secret, client_assertion, authMethod, true);

            httpRequestMessage.Content = new FormUrlEncodedContent(body);

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion

        #region FAPI CIBA
        /// <summary>FAPI CIBAの認可リクエスト（WebAPI）</summary>
        /// <param name="cibaAuthZUri">Uri</param>
        /// <param name="requestObjectUri">string</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>
        /// request_uriでRequest Objectの在り処を渡す（CIBA Core 1.0 に無い、独自の形）。
        /// 標準の形（§7.1.1）で送るには、requestパラメタで送るオーバーロードを使用する。
        /// </remarks>
        public static async Task<string> CibaAuthZRequestAsync(Uri cibaAuthZUri, string requestObjectUri)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = cibaAuthZUri,
            };

            httpRequestMessage.Content = new FormUrlEncodedContent(
                   new Dictionary<string, string>
                   {
                        { OAuth2AndOIDCConst.request_uri, requestObjectUri }
                   });

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>FAPI CIBAの認可リクエスト（WebAPI、CIBA Core 1.0 §7.1.1）</summary>
        /// <param name="cibaAuthZUri">Uri（バックチャネル認証エンドポイント）</param>
        /// <param name="requestObject">string（Request Object＝署名付きJWT）</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（tls_client_authではnull）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>
        /// private_key_jwt / client_secret_jwtには、client_assertionを取るオーバーロードを使用する。
        /// </remarks>
        public static async Task<string> CibaAuthZRequestAsync(
            Uri cibaAuthZUri, string requestObject,
            string client_id, string client_secret,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            return await OAuth2AndOIDCClient.CibaAuthZRequestAsync(
                cibaAuthZUri, requestObject,
                client_id, client_secret, null, authMethod).ConfigureAwait(false);
        }

        /// <summary>FAPI CIBAの認可リクエスト（WebAPI、CIBA Core 1.0 §7.1.1）</summary>
        /// <param name="cibaAuthZUri">Uri（バックチャネル認証エンドポイント）</param>
        /// <param name="requestObject">string（Request Object＝署名付きJWT）</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>
        /// CIBA Core 1.0 は、署名付きJWTをrequestパラメタで送る形（§7.1.1）を定めており、
        /// バックチャネル認証エンドポイントには、Tokenエンドポイントと同じクライアント認証が要る。
        /// FAPI-CIBAは、この認証にprivate_key_jwt（またはMTLS）を要求する。
        /// </remarks>
        public static async Task<string> CibaAuthZRequestAsync(
            Uri cibaAuthZUri, string requestObject,
            string client_id, string client_secret, string client_assertion,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = cibaAuthZUri,
            };

            // body
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                { OAuth2AndOIDCConst.request, requestObject }
            };

            // 認証情報の付加
            // （CIBAは、認証方式が要求するパラメタ以外をJWTの外に置けない : CIBA Core 1.0 §7.1.1）
            OAuth2AndOIDCClient.SetClientAuthentication(
                httpRequestMessage, body, client_id, client_secret, client_assertion, authMethod, false);

            httpRequestMessage.Content = new FormUrlEncodedContent(body);

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>FAPI CIBAのTokenリクエスト</summary>
        /// <param name="tokenEndpointUri">Uri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="auth_req_id">string</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>     
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> GetAccessTokenByCibaAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string auth_req_id,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
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

            // body
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                { OAuth2AndOIDCConst.grant_type, OAuth2AndOIDCConst.CibaGrantType },
                { OAuth2AndOIDCConst.auth_req_id, auth_req_id }
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

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        #endregion

        #endregion

        #region DPoP（RFC 9449）

        /// <summary>サーバから受け取った DPoP-Nonce（鍵はオリジン）</summary>
        private static readonly Dictionary<string, string> _dpopNonces = new Dictionary<string, string>();

        /// <summary>_dpopNonces の排他</summary>
        private static readonly object _dpopNoncesLock = new object();

        /// <summary>DPoP の証明を添えて送る</summary>
        /// <param name="createRequest">Func（要求を作る。nonce を求められたら作り直して送り直すため、毎回新しく作ること）</param>
        /// <param name="signer">JWS（クライアントの秘密鍵。JwsFactory.CreateFromPrivateJwk など）</param>
        /// <param name="publicJwk">JObject（その鍵の公開鍵）</param>
        /// <param name="accessToken">string（リソースへの要求なら access_token。Authorization: DPoP と ath を付ける。null なら付けない）</param>
        /// <returns>HttpResponseMessage</returns>
        /// <remarks>
        /// 自由に利用できる（#634 の A-2）。
        /// - 要求ごとに証明を作り、DPoP ヘッダに添える（htm / htu は要求から取る）
        /// - 応答の DPoP-Nonce をオリジンごとに覚え、次の要求の証明に入れる（RFC 9449 8 / 9）
        /// - use_dpop_nonce で断られたら、受け取った nonce を入れて 1 回だけ送り直す
        /// 縛られたトークンを Bearer で送ると断られるので、リソースへの要求では accessToken を渡すこと。
        /// </remarks>
        public static async Task<HttpResponseMessage> SendWithDPoPAsync(
            Func<HttpRequestMessage> createRequest, JWS signer, JObject publicJwk, string accessToken)
        {
            if (createRequest == null)
            {
                throw new ArgumentNullException(nameof(createRequest));
            }

            HttpResponseMessage httpResponseMessage = null;
            string usedNonce = null;

            for (int i = 0; i < 2; i++)
            {
                HttpRequestMessage httpRequestMessage = createRequest();
                string origin = httpRequestMessage.RequestUri.GetLeftPart(UriPartial.Authority);

                lock (OAuth2AndOIDCClient._dpopNoncesLock)
                {
                    OAuth2AndOIDCClient._dpopNonces.TryGetValue(origin, out usedNonce);
                }

                httpRequestMessage.Headers.Add(OAuth2AndOIDCConst.HttpHeader_DPoP,
                    DPoPProof.Create(signer, publicJwk,
                        httpRequestMessage.Method.Method, httpRequestMessage.RequestUri.AbsoluteUri,
                        accessToken, usedNonce));

                if (!string.IsNullOrEmpty(accessToken))
                {
                    httpRequestMessage.Headers.Authorization =
                        AuthenticationHeader.CreateDPoPAuthenticationHeaderValue(accessToken);
                }

                httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

                // 応答の DPoP-Nonce を覚える（成功の応答にも付くことがある）。
                string nonce = OAuth2AndOIDCClient.GetDPoPNonce(httpResponseMessage);

                if (string.IsNullOrEmpty(nonce))
                {
                    break;
                }

                lock (OAuth2AndOIDCClient._dpopNoncesLock)
                {
                    OAuth2AndOIDCClient._dpopNonces[origin] = nonce;
                }

                if (nonce == usedNonce
                    || !await OAuth2AndOIDCClient.IsUseDPoPNonceAsync(httpResponseMessage).ConfigureAwait(false))
                {
                    break;
                }

                // use_dpop_nonce : 新しい nonce を入れて、1 回だけ送り直す。
            }

            return httpResponseMessage;
        }

        /// <summary>DPoP の証明を添えて、Token エンドポイントへ要求する（パブリック クライアント）</summary>
        /// <param name="tokenEndpointUri">Uri</param>
        /// <param name="body">Dictionary（grant_type などの本文。client_id も入れること）</param>
        /// <param name="signer">JWS（クライアントの秘密鍵）</param>
        /// <param name="publicJwk">JObject（その鍵の公開鍵）</param>
        /// <returns>結果のJSON文字列（token_type は DPoP になる）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// 認可コード（PKCE）・refresh_token など、grant_type を問わない。引数の Dictionary は変えない。
        /// 返る access_token は鍵に縛られる（cnf.jkt）ので、使うときも同じ鍵の証明が要る。
        /// </remarks>
        public static async Task<string> TokenRequestWithDPoPAsync(
            Uri tokenEndpointUri, Dictionary<string, string> body, JWS signer, JObject publicJwk)
        {
            HttpResponseMessage httpResponseMessage = await OAuth2AndOIDCClient.SendWithDPoPAsync(
                () => new HttpRequestMessage(HttpMethod.Post, tokenEndpointUri)
                {
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>(body))
                },
                signer, publicJwk, null).ConfigureAwait(false);

            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>DPoP の証明を添えて、Token エンドポイントへ要求する（クライアント認証あり）</summary>
        /// <param name="tokenEndpointUri">Uri</param>
        /// <param name="body">Dictionary（grant_type などの本文）</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <param name="signer">JWS（クライアントの DPoP の秘密鍵）</param>
        /// <param name="publicJwk">JObject（その鍵の公開鍵）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる。引数の Dictionary は変えない。</remarks>
        public static async Task<string> TokenRequestWithDPoPAsync(
            Uri tokenEndpointUri, Dictionary<string, string> body,
            string client_id, string client_secret, string client_assertion,
            OAuth2AndOIDCEnum.AuthMethods authMethod, JWS signer, JObject publicJwk)
        {
            HttpResponseMessage httpResponseMessage = await OAuth2AndOIDCClient.SendWithDPoPAsync(
                () =>
                {
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, tokenEndpointUri);
                    Dictionary<string, string> b = new Dictionary<string, string>(body);

                    OAuth2AndOIDCClient.SetClientAuthentication(
                        httpRequestMessage, b, client_id, client_secret, client_assertion, authMethod, false);

                    httpRequestMessage.Content = new FormUrlEncodedContent(b);
                    return httpRequestMessage;
                },
                signer, publicJwk, null).ConfigureAwait(false);

            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>DPoP で縛られた access_token で、UserInfo を呼び出す</summary>
        /// <param name="userInfoEndpointUri">Uri</param>
        /// <param name="accessToken">string</param>
        /// <param name="signer">JWS（トークンを縛った鍵の秘密鍵）</param>
        /// <param name="publicJwk">JObject（その鍵の公開鍵）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる。Authorization: DPoP と、ath を入れた証明を添える（RFC 9449 7.1）。</remarks>
        public static async Task<string> GetUserInfoAsync(
            Uri userInfoEndpointUri, string accessToken, JWS signer, JObject publicJwk)
        {
            HttpResponseMessage httpResponseMessage = await OAuth2AndOIDCClient.SendWithDPoPAsync(
                () => new HttpRequestMessage(HttpMethod.Get, userInfoEndpointUri),
                signer, publicJwk, accessToken).ConfigureAwait(false);

            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>応答の DPoP-Nonce を読む</summary>
        /// <param name="httpResponseMessage">HttpResponseMessage</param>
        /// <returns>nonce（無ければ null）</returns>
        private static string GetDPoPNonce(HttpResponseMessage httpResponseMessage)
        {
            IEnumerable<string> values = null;

            if (httpResponseMessage.Headers.TryGetValues(OAuth2AndOIDCConst.HttpHeader_DPoPNonce, out values))
            {
                foreach (string value in values)
                {
                    return value;
                }
            }

            return null;
        }

        /// <summary>use_dpop_nonce で断られたか</summary>
        /// <param name="httpResponseMessage">HttpResponseMessage</param>
        /// <returns>断られたなら true</returns>
        /// <remarks>
        /// Token エンドポイントは本文（400）で、リソースは WWW-Authenticate（401）で返す（RFC 9449 8 / 9）。
        /// </remarks>
        private static async Task<bool> IsUseDPoPNonceAsync(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                foreach (AuthenticationHeaderValue value in httpResponseMessage.Headers.WwwAuthenticate)
                {
                    if (value.Parameter != null && 0 <= value.Parameter.IndexOf(
                        OAuth2AndOIDCConst.use_dpop_nonce, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest
                || httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                try
                {
                    // 本文はバッファされるので、呼び出し側でも読める。
                    string json = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                    JObject obj = JToken.Parse(json) as JObject;

                    return obj != null
                        && (string)obj[OAuth2AndOIDCConst.error] == OAuth2AndOIDCConst.use_dpop_nonce;
                }
                catch (Exception)
                {
                    // JSON でない。
                }
            }

            return false;
        }

        #endregion

        #region DCR（RFC 7591 / 7592）

        /// <summary>Discovery から registration_endpoint を引く</summary>
        /// <param name="discoveryUri">Uri（/.well-known/openid-configuration）</param>
        /// <returns>registration_endpoint（広告が無い・読めなければ空文字）</returns>
        /// <remarks>
        /// 自由に利用できる（#634 の F-1）。
        /// RegisterRequestObjectAsync は /ros（RFC 9101 の Request Object）で、DCR ではない。
        /// </remarks>
        public static async Task<string> GetRegistrationEndpointAsync(Uri discoveryUri)
        {
            try
            {
                JObject discovery = JToken.Parse(
                    await OAuth2AndOIDCClient.GetJwkSetAsync(discoveryUri).ConfigureAwait(false)) as JObject;

                JToken endpoint = (discovery == null) ? null : discovery[OAuth2AndOIDCConst.registration_endpoint];
                return (endpoint != null && endpoint.Type == JTokenType.String) ? (string)endpoint : "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>クライアントを登録する（RFC 7591 3.1）</summary>
        /// <param name="registrationEndpointUri">Uri（registration_endpoint）</param>
        /// <param name="metadataJson">string（クライアント メタデータの JSON。redirect_uris など）</param>
        /// <param name="initialAccessToken">string（登録の口が求めるアクセス トークン。null か空なら付けない）</param>
        /// <returns>結果のJSON文字列（client_id・registration_access_token・registration_client_uri など）</returns>
        /// <remarks>
        /// 自由に利用できる（#634 の F-1）。
        /// registration_access_token は**この応答で 1 回だけ返る**。受け取って保持するのは呼び出し側である。
        /// software_statement を添えるなら、metadataJson に入れる（貰った文字列をそのまま入れる）。
        /// </remarks>
        public static async Task<string> RegisterClientAsync(
            Uri registrationEndpointUri, string metadataJson, string initialAccessToken)
        {
            HttpResponseMessage httpResponseMessage = await OAuth2AndOIDCClient.SendRegistrationAsync(
                HttpMethod.Post, registrationEndpointUri, initialAccessToken, metadataJson).ConfigureAwait(false);

            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>登録を読む（RFC 7592 2.1）</summary>
        /// <param name="registrationClientUri">Uri（registration_client_uri）</param>
        /// <param name="registrationAccessToken">string（registration_access_token）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる（#634 の F-2）。</remarks>
        public static async Task<string> ReadClientRegistrationAsync(
            Uri registrationClientUri, string registrationAccessToken)
        {
            HttpResponseMessage httpResponseMessage = await OAuth2AndOIDCClient.SendRegistrationAsync(
                HttpMethod.Get, registrationClientUri, registrationAccessToken, null).ConfigureAwait(false);

            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>登録を直す（RFC 7592 2.2。全置換）</summary>
        /// <param name="registrationClientUri">Uri（registration_client_uri）</param>
        /// <param name="registrationAccessToken">string（registration_access_token）</param>
        /// <param name="metadataJson">string（クライアント メタデータの JSON。client_id を入れ、残したい項目も全て入れる）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる（#634 の F-2）。PUT は全置換なので、入れなかった項目は消える。</remarks>
        public static async Task<string> UpdateClientRegistrationAsync(
            Uri registrationClientUri, string registrationAccessToken, string metadataJson)
        {
            HttpResponseMessage httpResponseMessage = await OAuth2AndOIDCClient.SendRegistrationAsync(
                HttpMethod.Put, registrationClientUri, registrationAccessToken, metadataJson).ConfigureAwait(false);

            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>登録を消す（RFC 7592 2.3）</summary>
        /// <param name="registrationClientUri">Uri（registration_client_uri）</param>
        /// <param name="registrationAccessToken">string（registration_access_token）</param>
        /// <returns>消せたなら true（成功は 204 で、本文が無い）</returns>
        /// <remarks>自由に利用できる（#634 の F-2）。</remarks>
        public static async Task<bool> DeleteClientRegistrationAsync(
            Uri registrationClientUri, string registrationAccessToken)
        {
            HttpResponseMessage httpResponseMessage = await OAuth2AndOIDCClient.SendRegistrationAsync(
                HttpMethod.Delete, registrationClientUri, registrationAccessToken, null).ConfigureAwait(false);

            return httpResponseMessage.IsSuccessStatusCode;
        }

        /// <summary>登録の口へ送る</summary>
        /// <param name="method">HttpMethod</param>
        /// <param name="uri">Uri</param>
        /// <param name="accessToken">string（Bearer で付ける。null か空なら付けない。#634 の F-3）</param>
        /// <param name="metadataJson">string（本文。null なら付けない）</param>
        /// <returns>HttpResponseMessage</returns>
        private static async Task<HttpResponseMessage> SendRegistrationAsync(
            HttpMethod method, Uri uri, string accessToken, string metadataJson)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, uri);

            if (!string.IsNullOrEmpty(accessToken))
            {
                httpRequestMessage.Headers.Authorization =
                    AuthenticationHeader.CreateBearerAuthenticationHeaderValue(accessToken);
            }

            if (metadataJson != null)
            {
                httpRequestMessage.Content = new StringContent(metadataJson, Encoding.UTF8, "application/json");
            }

            return await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
        }

        #endregion
    }
}
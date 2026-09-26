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
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

using System.Web;
using System.Net.Http;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Security;

namespace Touryo.Infrastructure.Framework.Authentication
{
    // ライブラリ内でawaitする場合は、ConfigureAwait(false)を使う。

    /// <summary>OAuth2AndOIDCClient（ライブラリ）</summary>
    public class OAuth2AndOIDCClient
    {
        /// <summary>HttpClient</summary>
        private static HttpClient _HttpClient = null;

        /// <summary>HttpClient</summary>
        public static HttpClient HttpClient
        {
            set
            {
                OAuth2AndOIDCClient._HttpClient = value;
            }
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
    }
}
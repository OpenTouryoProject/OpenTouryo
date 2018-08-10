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
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Web;

using Touryo.Infrastructure.Public.Str;
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

        #region Authentication Code or Hybrid Flow or PKCE

        /// <summary>
        /// Authentication Code or Hybrid Flow : codeからAccess Tokenを取得する。
        /// </summary>
        /// <param name="tokenEndpointUri">TokenエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="redirect_uri">redirect_uri</param>
        /// <param name="code">code</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> GetAccessTokenByCodeAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string redirect_uri, string code)
        {
            return await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                tokenEndpointUri, client_id, client_secret, redirect_uri, code, null);
        }

        /// <summary>
        /// PKCE : code, code_verifierからAccess Tokenを取得する。
        /// </summary>
        /// <param name="tokenEndpointUri">TokenエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="redirect_uri">redirect_uri</param>
        /// <param name="code">code</param>
        /// <param name="code_verifier">code_verifier</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> GetAccessTokenByCodeAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string redirect_uri, string code, string code_verifier)
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

            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                CustomEncode.ToBase64String(CustomEncode.StringToByte(
                    string.Format("{0}:{1}", client_id, client_secret), CustomEncode.us_ascii)));

            if (string.IsNullOrEmpty(code_verifier))
            {
                // 通常のアクセストークン・リクエスト
                httpRequestMessage.Content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                    { "grant_type", OAuth2AndOIDCConst.AuthorizationCodeGrantType },
                    { "code", code },
                    { "redirect_uri", HttpUtility.HtmlEncode(redirect_uri) },
                    });
            }
            else
            {
                // OAuth PKCEのアクセストークン・リクエスト
                httpRequestMessage.Content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                    { "grant_type", OAuth2AndOIDCConst.AuthorizationCodeGrantType },
                    { "code", code },
                    { "code_verifier", code_verifier },
                    { "redirect_uri", HttpUtility.HtmlEncode(redirect_uri) },
                    });
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
        public static async Task<string> ClientCredentialsGrantAsync(Uri tokenEndpointUri, string client_id, string client_secret, string scopes)
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

            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                CustomEncode.ToBase64String(CustomEncode.StringToByte(
                    string.Format("{0}:{1}", client_id, client_secret), CustomEncode.us_ascii)));

            httpRequestMessage.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "grant_type", OAuth2AndOIDCConst.ClientCredentialsGrantType },
                    { "scope", scopes },
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

            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(
                    string.Format("{0}:{1}",
                    client_id, client_secret))));

            httpRequestMessage.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "username", userId },
                    { "password", password },
                    { "scope", scopes },
                });

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region その他の基本WebAPI

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
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                CustomEncode.ToBase64String(CustomEncode.StringToByte(
                    string.Format("{0}:{1}", client_id, client_secret), CustomEncode.us_ascii)));

            httpRequestMessage.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "grant_type", OAuth2AndOIDCConst.RefreshTokenGrantType },
                    { "refresh_token", refreshToken },
                });

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion

        #region UserInfo

        /// <summary>認可したユーザのClaim情報を取得するWebAPIを呼び出す</summary>
        /// <param name="userInfoUri">Uri</param>
        /// <param name="accessToken">accessToken</param>
        /// <returns>結果のJSON文字列（認可したユーザのClaim情報）</returns>
        public static async Task<string> CallUserInfoEndpointAsync(Uri userInfoUri, string accessToken)
        {
            // 通信用の変数
            HttpRequestMessage httpRequestMessage = null;
            HttpResponseMessage httpResponseMessage = null;

            // HttpRequestMessage (Method & RequestUri)
            httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = userInfoUri,
            };

            // HttpRequestMessage (Headers)
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

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
            return CustomEncode.ToBase64UrlString(
                GetHash.GetHashBytes(
                    CustomEncode.StringToByte(code_verifier, CustomEncode.us_ascii),
                    EnumHashAlgorithm.SHA256Managed));
        }

        #endregion

        #region Revoke & Introspect

        /// <summary>Revokeエンドポイントで、Tokenを無効化する。</summary>
        /// <param name="revokeTokenEndpointUri">RevokeエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="token">token</param>
        /// <param name="token_type_hint">token_type_hint</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> RevokeTokenAsync(
            Uri revokeTokenEndpointUri, string client_id, string client_secret, string token, string token_type_hint)
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

            // HttpRequestMessage (Headers & Content)

            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                CustomEncode.ToBase64String(CustomEncode.StringToByte(
                    string.Format("{0}:{1}", client_id, client_secret), CustomEncode.us_ascii)));

            httpRequestMessage.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "token", token },
                    { "token_type_hint", token_type_hint },
                });

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
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> IntrospectTokenAsync(
            Uri introspectTokenEndpointUri, string client_id, string client_secret, string token, string token_type_hint)
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

            // HttpRequestMessage (Headers & Content)

            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                CustomEncode.ToBase64String(CustomEncode.StringToByte(
                    string.Format("{0}:{1}", client_id, client_secret), CustomEncode.us_ascii)));

            httpRequestMessage.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "token", token },
                    { "token_type_hint", token_type_hint },
                });

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion

        #region 追加のClient認証

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

            httpRequestMessage.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "grant_type", OAuth2AndOIDCConst.JwtBearerTokenFlowGrantType },
                    { "assertion", assertion },
                });

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion

        #endregion

        #endregion
    }
}
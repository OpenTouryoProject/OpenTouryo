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
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Web;

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

        /// <summary>
        /// Authorization Code Grant
        /// 仲介コードからAccess Tokenを取得する。
        /// </summary>
        /// <param name="tokenEndpointUri">TokenエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="redirect_uri">redirect_uri</param>
        /// <param name="code">仲介コード</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> GetAccessTokenByCodeAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string redirect_uri, string code)
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
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "redirect_uri", HttpUtility.HtmlEncode(redirect_uri) },
                });

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

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
        public static async Task<string> GetAccessTokenByROPAsync(
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

        /// <summary>Refresh Tokenを使用してAccess Tokenを更新</summary>
        /// <param name="tokenEndpointUri">tokenEndpointUri</param>
        /// <param name="refreshToken">refreshToken</param>
        /// <returns>結果のJSON文字列</returns>
        public static async Task<string> UpdateAccessTokenByRefreshTokenAsync(
            Uri tokenEndpointUri, string refreshToken)
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

            // HttpRequestMessage (Content)
            httpRequestMessage.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshToken },
                });

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

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
    }
}
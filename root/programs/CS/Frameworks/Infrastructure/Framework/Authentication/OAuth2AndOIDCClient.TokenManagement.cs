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
//* クラス名        ：OAuth2AndOIDCClient（TokenManagement）
//* クラス日本語名  ：OAuth2AndOIDCClient（ライブラリ）のトークン運用・管理（Refresh・Revoke・Introspect）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（OAuth2AndOIDCClient.cs から処理区分ごとに分割した）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net.Http;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>OAuth2AndOIDCClient（ライブラリ）のトークン運用・管理（Refresh・Revoke・Introspect）</summary>
    public partial class OAuth2AndOIDCClient
    {
        #region Refresh Token（RFC 6749 6）

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

        #region Revoke（RFC 7009）

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

        #endregion

        #region Introspect（RFC 7662）

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
    }
}

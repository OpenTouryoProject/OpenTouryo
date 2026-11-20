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
//*  2026/11/08  玄人 幸道         Refresh・Revoke・Introspect に、クライアント認証の方式を問わない版を追加（private_key_jwt・tls_client_auth。#676）
//*  2026/11/08  玄人 幸道         空の値の検査を SetClientAuthentication に移した（#678）
//*  2026/11/20  玄人 幸道         Refresh・Revoke・Introspect の各形に、使う HttpClient を指定できる版を追加した（#719）
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
        public static Task<string> UpdateAccessTokenByRefreshTokenAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string refreshToken)
        {
            return OAuth2AndOIDCClient.UpdateAccessTokenByRefreshTokenAsync(
                tokenEndpointUri, client_id, client_secret, refreshToken, null);
        }

        /// <summary>Refresh Tokenを使用してAccess Tokenを更新（使う HttpClient を指定する版。#719）</summary>
        /// <param name="tokenEndpointUri">tokenEndpointUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="refreshToken">refreshToken</param>
        /// <param name="httpClient">HttpClient（この呼び出しで送るもの。null なら HttpClient プロパティの共有のもの）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる。動作は UpdateAccessTokenByRefreshTokenAsync(tokenEndpointUri, client_id, client_secret, refreshToken) と同じ。</remarks>
        public static async Task<string> UpdateAccessTokenByRefreshTokenAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string refreshToken, HttpClient httpClient)
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
            httpResponseMessage = await OAuth2AndOIDCClient.SendCoreAsync(httpRequestMessage, httpClient).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>Refresh Tokenを使用してAccess Tokenを更新（クライアント認証の方式を問わない）</summary>
        /// <param name="tokenEndpointUri">tokenEndpointUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="refreshToken">refreshToken</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods（既定値は無い。必ず指定する）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>
        /// **private_key_jwt と tls_client_auth でも更新できる**（#676。既存の版は Basic だけ）。
        /// 認証の付け方は SetClientAuthentication（GetAccessTokenByCodeAsync のクライアント認証の方式を問わない版と同じ）。
        /// client_id は、認証方式が要求するときだけ本文に載せる（client_secret_post / tls_client_auth）。
        /// 既存の版の振る舞いは変えていない。
        /// </remarks>
        public static Task<string> UpdateAccessTokenByRefreshTokenAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string client_assertion, string refreshToken,
            OAuth2AndOIDCEnum.AuthMethods authMethod)
        {
            return OAuth2AndOIDCClient.UpdateAccessTokenByRefreshTokenAsync(
                tokenEndpointUri, client_id, client_secret, client_assertion, refreshToken, authMethod, null);
        }

        /// <summary>Refresh Tokenを使用してAccess Tokenを更新（クライアント認証の方式を問わない。使う HttpClient を指定する版。#719）</summary>
        /// <param name="tokenEndpointUri">tokenEndpointUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="refreshToken">refreshToken</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods（既定値は無い。必ず指定する）</param>
        /// <param name="httpClient">HttpClient（この呼び出しで送るもの。null なら HttpClient プロパティの共有のもの）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>
        /// 自由に利用できる。動作は、HttpClient を受けない同じ引数の版と同じ。
        /// tls_client_auth では、クライアント証明書を載せた HttpClient を、この引数か HttpClient プロパティに渡す。
        /// </remarks>
        public static async Task<string> UpdateAccessTokenByRefreshTokenAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string client_assertion, string refreshToken,
            OAuth2AndOIDCEnum.AuthMethods authMethod, HttpClient httpClient)
        {
            // RFC 6749 §6
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                { OAuth2AndOIDCConst.grant_type, OAuth2AndOIDCConst.RefreshTokenGrantType },
                { OAuth2AndOIDCConst.RefreshToken, refreshToken },
            };

            return await OAuth2AndOIDCClient.PostWithClientAuthenticationAsync(
                tokenEndpointUri, body, client_id, client_secret, client_assertion, authMethod, httpClient).ConfigureAwait(false);
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
        public static Task<string> RevokeTokenAsync(
            Uri revokeTokenEndpointUri, string client_id, string client_secret, string token, string token_type_hint,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            return OAuth2AndOIDCClient.RevokeTokenAsync(
                revokeTokenEndpointUri, client_id, client_secret, token, token_type_hint, authMethod, (HttpClient)null);
        }

        /// <summary>Revokeエンドポイントで、Tokenを無効化する。（使う HttpClient を指定する版。#719）</summary>
        /// <param name="revokeTokenEndpointUri">RevokeエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="token">token</param>
        /// <param name="token_type_hint">token_type_hint</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <param name="httpClient">HttpClient（この呼び出しで送るもの。null なら HttpClient プロパティの共有のもの）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる。動作は、HttpClient を受けない同じ引数の版と同じ。</remarks>
        public static async Task<string> RevokeTokenAsync(
            Uri revokeTokenEndpointUri, string client_id, string client_secret, string token, string token_type_hint,
            OAuth2AndOIDCEnum.AuthMethods authMethod, HttpClient httpClient)
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
            httpResponseMessage = await OAuth2AndOIDCClient.SendCoreAsync(httpRequestMessage, httpClient).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>Revokeエンドポイントで、Tokenを無効化する（クライアント認証の方式を問わない）。</summary>
        /// <param name="revokeTokenEndpointUri">RevokeエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="token">token</param>
        /// <param name="token_type_hint">token_type_hint（空なら送らない）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods（既定値は無い。必ず指定する）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>
        /// **private_key_jwt と tls_client_auth でも取り消せる**（#676。既存の版は Basic・Post だけ）。
        /// 認証の付け方は SetClientAuthentication。token_type_hint は任意（RFC 7009 §2.1）なので、空なら送らない。
        /// 既存の版の振る舞いは変えていない。
        /// </remarks>
        public static Task<string> RevokeTokenAsync(
            Uri revokeTokenEndpointUri, string client_id, string client_secret, string client_assertion,
            string token, string token_type_hint, OAuth2AndOIDCEnum.AuthMethods authMethod)
        {
            return OAuth2AndOIDCClient.RevokeTokenAsync(
                revokeTokenEndpointUri, client_id, client_secret, client_assertion, token, token_type_hint, authMethod, null);
        }

        /// <summary>Revokeエンドポイントで、Tokenを無効化する（クライアント認証の方式を問わない。使う HttpClient を指定する版。#719）。</summary>
        /// <param name="revokeTokenEndpointUri">RevokeエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="token">token</param>
        /// <param name="token_type_hint">token_type_hint（空なら送らない）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods（既定値は無い。必ず指定する）</param>
        /// <param name="httpClient">HttpClient（この呼び出しで送るもの。null なら HttpClient プロパティの共有のもの）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる。動作は、HttpClient を受けない同じ引数の版と同じ。</remarks>
        public static async Task<string> RevokeTokenAsync(
            Uri revokeTokenEndpointUri, string client_id, string client_secret, string client_assertion,
            string token, string token_type_hint, OAuth2AndOIDCEnum.AuthMethods authMethod, HttpClient httpClient)
        {
            return await OAuth2AndOIDCClient.PostWithClientAuthenticationAsync(
                revokeTokenEndpointUri, OAuth2AndOIDCClient.CreateTokenBody(token, token_type_hint),
                client_id, client_secret, client_assertion, authMethod, httpClient).ConfigureAwait(false);
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
        public static Task<string> IntrospectTokenAsync(
            Uri introspectTokenEndpointUri, string client_id, string client_secret, string token, string token_type_hint,
            OAuth2AndOIDCEnum.AuthMethods authMethod = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic)
        {
            return OAuth2AndOIDCClient.IntrospectTokenAsync(
                introspectTokenEndpointUri, client_id, client_secret, token, token_type_hint, authMethod, (HttpClient)null);
        }

        /// <summary>Introspectエンドポイントで、Tokenの状態を照会する。（使う HttpClient を指定する版。#719）</summary>
        /// <param name="introspectTokenEndpointUri">IntrospectエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret</param>
        /// <param name="token">token</param>
        /// <param name="token_type_hint">token_type_hint</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <param name="httpClient">HttpClient（この呼び出しで送るもの。null なら HttpClient プロパティの共有のもの）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる。動作は、HttpClient を受けない同じ引数の版と同じ。</remarks>
        public static async Task<string> IntrospectTokenAsync(
            Uri introspectTokenEndpointUri, string client_id, string client_secret, string token, string token_type_hint,
            OAuth2AndOIDCEnum.AuthMethods authMethod, HttpClient httpClient)
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
            httpResponseMessage = await OAuth2AndOIDCClient.SendCoreAsync(httpRequestMessage, httpClient).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>Introspectエンドポイントで、Tokenの状態を照会する（クライアント認証の方式を問わない）。</summary>
        /// <param name="introspectTokenEndpointUri">IntrospectエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="token">token</param>
        /// <param name="token_type_hint">token_type_hint（空なら送らない）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods（既定値は無い。必ず指定する）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>
        /// **private_key_jwt と tls_client_auth でも照会できる**（#676。既存の版は Basic・Post だけ）。
        /// 認証の付け方は SetClientAuthentication。token_type_hint は任意（RFC 7662 §2.1）なので、空なら送らない。
        /// 既存の版の振る舞いは変えていない。
        /// </remarks>
        public static Task<string> IntrospectTokenAsync(
            Uri introspectTokenEndpointUri, string client_id, string client_secret, string client_assertion,
            string token, string token_type_hint, OAuth2AndOIDCEnum.AuthMethods authMethod)
        {
            return OAuth2AndOIDCClient.IntrospectTokenAsync(
                introspectTokenEndpointUri, client_id, client_secret, client_assertion, token, token_type_hint, authMethod, null);
        }

        /// <summary>Introspectエンドポイントで、Tokenの状態を照会する（クライアント認証の方式を問わない。使う HttpClient を指定する版。#719）。</summary>
        /// <param name="introspectTokenEndpointUri">IntrospectエンドポイントのUri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="token">token</param>
        /// <param name="token_type_hint">token_type_hint（空なら送らない）</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods（既定値は無い。必ず指定する）</param>
        /// <param name="httpClient">HttpClient（この呼び出しで送るもの。null なら HttpClient プロパティの共有のもの）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる。動作は、HttpClient を受けない同じ引数の版と同じ。</remarks>
        public static async Task<string> IntrospectTokenAsync(
            Uri introspectTokenEndpointUri, string client_id, string client_secret, string client_assertion,
            string token, string token_type_hint, OAuth2AndOIDCEnum.AuthMethods authMethod, HttpClient httpClient)
        {
            return await OAuth2AndOIDCClient.PostWithClientAuthenticationAsync(
                introspectTokenEndpointUri, OAuth2AndOIDCClient.CreateTokenBody(token, token_type_hint),
                client_id, client_secret, client_assertion, authMethod, httpClient).ConfigureAwait(false);
        }

        #endregion

        #region 共通（#676）

        /// <summary>Revoke・Introspect の本文（token と、空でなければ token_type_hint）</summary>
        private static Dictionary<string, string> CreateTokenBody(string token, string token_type_hint)
        {
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                { OAuth2AndOIDCConst.token, token },
            };

            if (!string.IsNullOrEmpty(token_type_hint))
            {
                body.Add(OAuth2AndOIDCConst.token_type_hint, token_type_hint);
            }

            return body;
        }

        /// <summary>本文にクライアント認証を付けて POST する（SetClientAuthentication。client_id は方式が要求するときだけ）</summary>
        /// <remarks>
        /// 空では認証として成り立たない値は、SetClientAuthentication が送る前に ArgumentNullException にする
        /// （jwt 系は client_assertion、client_secret_* と tls_client_auth は client_id。client_secret は検査しない）。
        /// 検査は #676 でここに入れ、#678 で SetClientAuthentication に移した（PAR・CIBA・DPoP と同じ検査になる）。
        /// </remarks>
        private static async Task<string> PostWithClientAuthenticationAsync(
            Uri endpointUri, Dictionary<string, string> body,
            string client_id, string client_secret, string client_assertion,
            OAuth2AndOIDCEnum.AuthMethods authMethod, HttpClient httpClient)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = endpointUri,
            };

            // 認証情報の付加（送る前に方式と値を確かめる。未知の方式は ArgumentException、空の値は ArgumentNullException で、送らない）
            OAuth2AndOIDCClient.SetClientAuthentication(
                httpRequestMessage, body, client_id, client_secret, client_assertion, authMethod, false);

            httpRequestMessage.Content = new FormUrlEncodedContent(body);

            HttpResponseMessage httpResponseMessage =
                await OAuth2AndOIDCClient.SendCoreAsync(httpRequestMessage, httpClient).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion
    }
}

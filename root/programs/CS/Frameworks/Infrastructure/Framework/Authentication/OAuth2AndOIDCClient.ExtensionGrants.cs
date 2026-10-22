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
//* クラス名        ：OAuth2AndOIDCClient（ExtensionGrants）
//* クラス日本語名  ：OAuth2AndOIDCClient（ライブラリ）の拡張グラント・分離フロー（JWT Bearer・Device AuthZ・CIBA）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（OAuth2AndOIDCClient.cs から処理区分ごとに分割した）
//*  2026/10/22  玄人 幸道         CIBAのTokenリクエストに、client_assertionを受ける版を追加
//*                                （private_key_jwt / client_secret_jwt / tls_client_authで取れなかった）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net.Http;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>OAuth2AndOIDCClient（ライブラリ）の拡張グラント・分離フロー（JWT Bearer・Device AuthZ・CIBA）</summary>
    public partial class OAuth2AndOIDCClient
    {
        #region JWT Bearer（RFC 7523 2.1）

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

        #region Device AuthZ（RFC 8628）

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

        #region CIBA（CIBA Core 1.0）

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

        /// <summary>FAPI CIBAのTokenリクエスト（クライアント認証の方式を問わない）</summary>
        /// <param name="tokenEndpointUri">Uri</param>
        /// <param name="client_id">client_id</param>
        /// <param name="client_secret">client_secret（client_secret_basic / client_secret_postで使用）</param>
        /// <param name="client_assertion">client_assertion（private_key_jwt / client_secret_jwtで使用）</param>
        /// <param name="auth_req_id">string</param>
        /// <param name="authMethod">OAuth2AndOIDCEnum.AuthMethods</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// FAPI-CIBA は、Tokenエンドポイントのクライアント認証に private_key_jwt（または MTLS）を要求する。
        /// client_assertion を受けない版は client_secret_basic / client_secret_post しか使えず、
        /// 認可リクエスト（CibaAuthZRequestAsync）では使える private_key_jwt で、トークンを取れなかった。
        ///
        /// 認証の付け方は SetClientAuthentication（PAR・CIBA の認可リクエストと同じ）。
        /// client_id は、認証方式が要求するときだけ本文に載せる（client_secret_post / tls_client_auth）。
        /// tls_client_auth では、クライアント証明書を載せた HttpClient を HttpClient プロパティに設定しておく。
        /// 既存の版（client_assertion を受けない版）の振る舞いは変えていない。
        /// </remarks>
        public static async Task<string> GetAccessTokenByCibaAsync(
            Uri tokenEndpointUri, string client_id, string client_secret, string client_assertion, string auth_req_id,
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
            OAuth2AndOIDCClient.SetClientAuthentication(
                httpRequestMessage, body, client_id, client_secret, client_assertion, authMethod, false);

            httpRequestMessage.Content = new FormUrlEncodedContent(body);

            // HttpResponseMessage
            httpResponseMessage = await OAuth2AndOIDCClient._HttpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        #endregion
    }
}

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
//* クラス名        ：OAuth2AndOIDCClient（ClientRegistration）
//* クラス日本語名  ：OAuth2AndOIDCClient（ライブラリ）の動的クライアント登録（DCR）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（OAuth2AndOIDCClient.cs から処理区分ごとに分割した）
//*  2026/10/22  玄人 幸道         Content-Typeの直書きをPubLiteral.HTTP_CONTENT_TYPE_JSONに寄せた
//*  2026/11/20  玄人 幸道         4 つの口に、使う HttpClient を指定できる版を追加した（#719）
//**********************************************************************************

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>OAuth2AndOIDCClient（ライブラリ）の動的クライアント登録（DCR）</summary>
    public partial class OAuth2AndOIDCClient
    {
        #region DCR（RFC 7591 / 7592）

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
        public static Task<string> RegisterClientAsync(
            Uri registrationEndpointUri, string metadataJson, string initialAccessToken)
        {
            return OAuth2AndOIDCClient.RegisterClientAsync(registrationEndpointUri, metadataJson, initialAccessToken, null);
        }

        /// <summary>クライアントを登録する（RFC 7591 3.1。使う HttpClient を指定する版。#719）</summary>
        /// <param name="registrationEndpointUri">Uri（registration_endpoint）</param>
        /// <param name="metadataJson">string（クライアント メタデータの JSON。redirect_uris など）</param>
        /// <param name="initialAccessToken">string（登録の口が求めるアクセス トークン。null か空なら付けない）</param>
        /// <param name="httpClient">HttpClient（この呼び出しで送るもの。null なら HttpClient プロパティの共有のもの）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる。動作は RegisterClientAsync(registrationEndpointUri, metadataJson, initialAccessToken) と同じ。</remarks>
        public static async Task<string> RegisterClientAsync(
            Uri registrationEndpointUri, string metadataJson, string initialAccessToken, HttpClient httpClient)
        {
            HttpResponseMessage httpResponseMessage = await OAuth2AndOIDCClient.SendRegistrationAsync(
                HttpMethod.Post, registrationEndpointUri, initialAccessToken, metadataJson, httpClient).ConfigureAwait(false);

            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>登録を読む（RFC 7592 2.1）</summary>
        /// <param name="registrationClientUri">Uri（registration_client_uri）</param>
        /// <param name="registrationAccessToken">string（registration_access_token）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる（#634 の F-2）。</remarks>
        public static Task<string> ReadClientRegistrationAsync(
            Uri registrationClientUri, string registrationAccessToken)
        {
            return OAuth2AndOIDCClient.ReadClientRegistrationAsync(registrationClientUri, registrationAccessToken, null);
        }

        /// <summary>登録を読む（RFC 7592 2.1。使う HttpClient を指定する版。#719）</summary>
        /// <param name="registrationClientUri">Uri（registration_client_uri）</param>
        /// <param name="registrationAccessToken">string（registration_access_token）</param>
        /// <param name="httpClient">HttpClient（この呼び出しで送るもの。null なら HttpClient プロパティの共有のもの）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる。動作は ReadClientRegistrationAsync(registrationClientUri, registrationAccessToken) と同じ。</remarks>
        public static async Task<string> ReadClientRegistrationAsync(
            Uri registrationClientUri, string registrationAccessToken, HttpClient httpClient)
        {
            HttpResponseMessage httpResponseMessage = await OAuth2AndOIDCClient.SendRegistrationAsync(
                HttpMethod.Get, registrationClientUri, registrationAccessToken, null, httpClient).ConfigureAwait(false);

            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>登録を直す（RFC 7592 2.2。全置換）</summary>
        /// <param name="registrationClientUri">Uri（registration_client_uri）</param>
        /// <param name="registrationAccessToken">string（registration_access_token）</param>
        /// <param name="metadataJson">string（クライアント メタデータの JSON。client_id を入れ、残したい項目も全て入れる）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる（#634 の F-2）。PUT は全置換なので、入れなかった項目は消える。</remarks>
        public static Task<string> UpdateClientRegistrationAsync(
            Uri registrationClientUri, string registrationAccessToken, string metadataJson)
        {
            return OAuth2AndOIDCClient.UpdateClientRegistrationAsync(
                registrationClientUri, registrationAccessToken, metadataJson, null);
        }

        /// <summary>登録を直す（RFC 7592 2.2。全置換。使う HttpClient を指定する版。#719）</summary>
        /// <param name="registrationClientUri">Uri（registration_client_uri）</param>
        /// <param name="registrationAccessToken">string（registration_access_token）</param>
        /// <param name="metadataJson">string（クライアント メタデータの JSON。client_id を入れ、残したい項目も全て入れる）</param>
        /// <param name="httpClient">HttpClient（この呼び出しで送るもの。null なら HttpClient プロパティの共有のもの）</param>
        /// <returns>結果のJSON文字列</returns>
        /// <remarks>自由に利用できる。動作は UpdateClientRegistrationAsync(registrationClientUri, registrationAccessToken, metadataJson) と同じ。</remarks>
        public static async Task<string> UpdateClientRegistrationAsync(
            Uri registrationClientUri, string registrationAccessToken, string metadataJson, HttpClient httpClient)
        {
            HttpResponseMessage httpResponseMessage = await OAuth2AndOIDCClient.SendRegistrationAsync(
                HttpMethod.Put, registrationClientUri, registrationAccessToken, metadataJson, httpClient).ConfigureAwait(false);

            return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>登録を消す（RFC 7592 2.3）</summary>
        /// <param name="registrationClientUri">Uri（registration_client_uri）</param>
        /// <param name="registrationAccessToken">string（registration_access_token）</param>
        /// <returns>消せたなら true（成功は 204 で、本文が無い）</returns>
        /// <remarks>自由に利用できる（#634 の F-2）。</remarks>
        public static Task<bool> DeleteClientRegistrationAsync(
            Uri registrationClientUri, string registrationAccessToken)
        {
            return OAuth2AndOIDCClient.DeleteClientRegistrationAsync(registrationClientUri, registrationAccessToken, null);
        }

        /// <summary>登録を消す（RFC 7592 2.3。使う HttpClient を指定する版。#719）</summary>
        /// <param name="registrationClientUri">Uri（registration_client_uri）</param>
        /// <param name="registrationAccessToken">string（registration_access_token）</param>
        /// <param name="httpClient">HttpClient（この呼び出しで送るもの。null なら HttpClient プロパティの共有のもの）</param>
        /// <returns>消せたなら true（成功は 204 で、本文が無い）</returns>
        /// <remarks>自由に利用できる。動作は DeleteClientRegistrationAsync(registrationClientUri, registrationAccessToken) と同じ。</remarks>
        public static async Task<bool> DeleteClientRegistrationAsync(
            Uri registrationClientUri, string registrationAccessToken, HttpClient httpClient)
        {
            HttpResponseMessage httpResponseMessage = await OAuth2AndOIDCClient.SendRegistrationAsync(
                HttpMethod.Delete, registrationClientUri, registrationAccessToken, null, httpClient).ConfigureAwait(false);

            return httpResponseMessage.IsSuccessStatusCode;
        }

        /// <summary>登録の口へ送る</summary>
        /// <param name="method">HttpMethod</param>
        /// <param name="uri">Uri</param>
        /// <param name="accessToken">string（Bearer で付ける。null か空なら付けない。#634 の F-3）</param>
        /// <param name="metadataJson">string（本文。null なら付けない）</param>
        /// <param name="httpClient">HttpClient（送るもの。null なら共有。#719）</param>
        /// <returns>HttpResponseMessage</returns>
        private static async Task<HttpResponseMessage> SendRegistrationAsync(
            HttpMethod method, Uri uri, string accessToken, string metadataJson, HttpClient httpClient)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, uri);

            if (!string.IsNullOrEmpty(accessToken))
            {
                httpRequestMessage.Headers.Authorization =
                    AuthenticationHeader.CreateBearerAuthenticationHeaderValue(accessToken);
            }

            if (metadataJson != null)
            {
                httpRequestMessage.Content = new StringContent(metadataJson, Encoding.UTF8, PubLiteral.HTTP_CONTENT_TYPE_JSON);
            }

            return await OAuth2AndOIDCClient.SendCoreAsync(httpRequestMessage, httpClient).ConfigureAwait(false);
        }

        #endregion
    }
}

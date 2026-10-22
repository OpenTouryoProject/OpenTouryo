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
//**********************************************************************************

using System;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;

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

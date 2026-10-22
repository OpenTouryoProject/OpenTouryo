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
//* クラス名        ：OAuth2AndOIDCClient（OpenIDConnect）
//* クラス日本語名  ：OAuth2AndOIDCClient（ライブラリ）のOpenID Connect・メタデータ・鍵（UserInfo・JWK Set・Discovery）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（OAuth2AndOIDCClient.cs から処理区分ごとに分割した）
//*  2026/10/22  玄人 幸道         GetJwkSetAsyncの取得をGetStringAsyncへ寄せ、GetDiscoveryDocumentAsyncを追加
//**********************************************************************************

using System;
using System.Threading.Tasks;

using System.Net.Http;

using Newtonsoft.Json.Linq;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>OAuth2AndOIDCClient（ライブラリ）のOpenID Connect・メタデータ・鍵（UserInfo・JWK Set・Discovery）</summary>
    public partial class OAuth2AndOIDCClient
    {
        #region UserInfo（OIDC Core 5.3）

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

        #region JWK Set（RFC 7517）

        /// <summary>JwkSetのWebAPIを呼び出す</summary>
        /// <param name="jwkSetEndpointUri">Uri</param>
        /// <returns>JwkSetのJSON文字列</returns>
        /// <remarks>
        /// 取得は GetStringAsync（共有基盤）に委ねる。失敗しても例外にせず、空文字を返す（従来どおり）。
        /// </remarks>
        public static async Task<string> GetJwkSetAsync(Uri jwkSetEndpointUri)
        {
            return await OAuth2AndOIDCClient.GetStringAsync(jwkSetEndpointUri).ConfigureAwait(false);
        }

        #endregion

        #region Discovery（OIDC Discovery 4 / RFC 8414）

        /// <summary>Discovery のメタデータを取得する</summary>
        /// <param name="discoveryUri">Uri（/.well-known/openid-configuration など）</param>
        /// <returns>メタデータのJSON文字列（取得できなければ空文字）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// 以前は口が無く、GetJwkSetAsync を「URL を GET する」代わりに使っていた。
        /// 失敗しても例外にせず、空文字を返す（GetJwkSetAsync と同じ）。
        /// issuer が期待値と一致するかは、呼び出し側で照合すること（OIDC Discovery 4.3）。
        /// </remarks>
        public static async Task<string> GetDiscoveryDocumentAsync(Uri discoveryUri)
        {
            return await OAuth2AndOIDCClient.GetStringAsync(discoveryUri).ConfigureAwait(false);
        }

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
                    await OAuth2AndOIDCClient.GetDiscoveryDocumentAsync(discoveryUri).ConfigureAwait(false)) as JObject;

                JToken endpoint = (discovery == null) ? null : discovery[OAuth2AndOIDCConst.registration_endpoint];
                return (endpoint != null && endpoint.Type == JTokenType.String) ? (string)endpoint : "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        #endregion
    }
}

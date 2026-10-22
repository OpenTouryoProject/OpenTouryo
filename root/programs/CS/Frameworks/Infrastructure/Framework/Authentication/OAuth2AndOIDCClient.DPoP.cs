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
//* クラス名        ：OAuth2AndOIDCClient（DPoP）
//* クラス日本語名  ：OAuth2AndOIDCClient（ライブラリ）の送信者制約トークン（DPoP）
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

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Security.Jwt;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>OAuth2AndOIDCClient（ライブラリ）の送信者制約トークン（DPoP）</summary>
    public partial class OAuth2AndOIDCClient
    {
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
    }
}

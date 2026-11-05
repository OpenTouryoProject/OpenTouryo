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
//* クラス名        ：DeviceFlow
//* クラス日本語名  ：Device Authorization Grant（RFC 8628）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/04  玄人 幸道         新規作成（#649）
//**********************************************************************************

using System;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Util;

namespace SpRp_CLI
{
    /// <summary>Device Authorization Grant（RFC 8628）</summary>
    /// <remarks>
    /// **ブラウザを持たない（または入力しにくい）端末**のためのフロー。
    /// 端末はコードを表示するだけで、利用者は**別の端末のブラウザ**で、そのコードを入れて承認する。
    ///
    /// **ポーリングは、IdP の指示に従う**（RFC 8628 §3.5）。
    /// | 応答の error | すること |
    /// |---|---|
    /// | `authorization_pending` | `interval` 秒あけて、もう一度 |
    /// | `slow_down` | **`interval` を 5 秒延ばして**、もう一度 |
    /// | `access_denied` / `expired_token` / その他 | やめる |
    /// </remarks>
    public class DeviceFlow
    {
        /// <summary>interval の既定（RFC 8628 §3.2 : 無ければ 5 秒）</summary>
        private const int DefaultIntervalSeconds = 5;

        /// <summary>slow_down で延ばす秒数（RFC 8628 §3.5）</summary>
        private const int SlowDownSeconds = 5;

        /// <summary>フローを実行する</summary>
        /// <param name="discovery">Discovery</param>
        /// <returns>成功なら true</returns>
        public static async Task<bool> RunAsync(Discovery discovery)
        {
            if (discovery.DeviceAuthorizationEndpoint == null)
            {
                Console.WriteLine("NG   IdP が Device AuthZ を広告していません（device_authorization_endpoint が無い）。");
                return false;
            }

            string client_id = OAuth2AndOIDCParams.ClientID;
            string scope = GetConfigParameter.GetConfigValue("SpRp_Scope");

            // ---------------------------------------------------------------- 1. デバイス認可要求（§3.1）
            //   **scope を送る**（#650）。openid を含めないと、id_token が返らず、UserInfo も insufficient_scope で断られる。
            JObject authz = JObject.Parse(await OAuth2AndOIDCClient.DeviceAuthZRequestAsync(
                discovery.DeviceAuthorizationEndpoint, client_id, scope));

            if (authz.ContainsKey(OAuth2AndOIDCConst.error))
            {
                Console.WriteLine("NG   デバイス認可要求が断られました : " + DeviceFlow.ErrorOf(authz));
                return false;
            }

            string device_code = (string)authz[OAuth2AndOIDCConst.device_code];
            int interval = (int?)authz["interval"] ?? DeviceFlow.DefaultIntervalSeconds;
            int expiresIn = (int?)authz[OAuth2AndOIDCConst.expires_in] ?? 0;

            // **verification_uri は絶対 URI**（§3.2）。**別の端末で開く**ので、組み立て直さない。
            Console.WriteLine("ブラウザで次を開き、コードを入力して承認してください。");
            Console.WriteLine("  URL    : " + (string)authz[OAuth2AndOIDCConst.verification_uri]);
            Console.WriteLine("  コード : " + (string)authz[OAuth2AndOIDCConst.user_code]);
            Console.WriteLine("  （コード入力済みの URL : " + (string)authz[OAuth2AndOIDCConst.verification_uri_complete] + "）");
            Console.WriteLine();

            // ---------------------------------------------------------------- 2. ポーリング（§3.4・§3.5）
            DateTime deadline = (expiresIn > 0) ? DateTime.UtcNow.AddSeconds(expiresIn) : DateTime.MaxValue;
            JObject token = null;

            while (token == null)
            {
                if (DateTime.UtcNow >= deadline)
                {
                    Console.WriteLine("NG   期限（expires_in）を過ぎました。");
                    return false;
                }

                await Task.Delay(TimeSpan.FromSeconds(interval));

                JObject response = JObject.Parse(await OAuth2AndOIDCClient
                    .GetAccessTokenByDeviceAuthZAsync(discovery.TokenEndpoint, client_id, device_code));

                string error = (string)response[OAuth2AndOIDCConst.error];

                if (string.IsNullOrEmpty(error))
                {
                    token = response;
                }
                else if (error == "authorization_pending")
                {
                    Console.WriteLine("  ... 承認を待っています（" + interval + " 秒ごと）");
                }
                else if (error == "slow_down")
                {
                    interval += DeviceFlow.SlowDownSeconds;
                    Console.WriteLine("  ... slow_down を受けました。間隔を " + interval + " 秒にします");
                }
                else
                {
                    // access_denied（利用者が拒否した）、expired_token（期限切れ）など
                    Console.WriteLine("NG   " + DeviceFlow.ErrorOf(response));
                    return false;
                }
            }

            Console.WriteLine("OK   トークンを得ました。");

            string access_token = (string)token[OAuth2AndOIDCConst.AccessToken];
            string id_token = (string)token[OAuth2AndOIDCConst.IDToken];

            // ---------------------------------------------------------------- 3. id_token の検証（来たときだけ）
            //   **Device AuthZ には nonce が無い**（認可要求をブラウザから出さないため）。署名・iss・aud・exp を見る。
            if (!string.IsNullOrEmpty(id_token))
            {
                JObject claims = await IdTokenVerifier.VerifyAsync(
                    id_token, access_token, "", discovery.Issuer, client_id, null);

                if (claims == null)
                {
                    return false;
                }

                Console.WriteLine("OK   id_token を検証しました（sub : " + (string)claims["sub"] + "）。");
            }

            // ---------------------------------------------------------------- 4. UserInfo
            return await UserInfo.ShowAsync(discovery, access_token);
        }

        /// <summary>エラーの応答を、表示用の文字列にする</summary>
        /// <param name="response">応答</param>
        /// <returns>error（error_description）</returns>
        public static string ErrorOf(JObject response)
        {
            string error = (string)response[OAuth2AndOIDCConst.error];
            string description = (string)response[OAuth2AndOIDCConst.error_description];

            // **error_description は任意**（RFC 6749 §5.2）。無ければ括弧を付けない
            return string.IsNullOrEmpty(description) ? error : string.Format("{0}（{1}）", error, description);
        }
    }
}

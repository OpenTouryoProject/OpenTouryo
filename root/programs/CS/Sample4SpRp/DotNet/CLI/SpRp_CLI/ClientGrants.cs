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
//* クラス名        ：ClientGrants
//* クラス日本語名  ：ブラウザを使わないグラント（Client Credentials・JWT Bearer・ROPC）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/07  玄人 幸道         新規作成（#668。汎用認証サイトの自己テストから移した）
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Util;

namespace SpRp_CLI
{
    /// <summary>ブラウザを使わないグラント</summary>
    /// <remarks>
    /// **サーバで動くバッチを模したコンフィデンシャル クライアント**（`SpRp_BatchClientID`）で送る。
    /// **秘密（`SpRp_BatchClientSecret`）と鍵（`SpRp_RsaPfxFilePath`）を、設定ファイルに持つ**ので、
    /// **利用者の端末に配る CLI には使えない**（`device`・`pkce` はパブリック クライアント）。
    ///
    /// | サブコマンド | グラント | クライアントの証明 |
    /// |---|---|---|
    /// | `cc` | Client Credentials（RFC 6749 §4.4） | client_secret（Basic） |
    /// | `jwtbearer` | JWT Bearer（RFC 7523 §2.1） | 自分の鍵で署名したアサーション（iss・sub は client_id） |
    /// | `ropc` | Resource Owner Password Credentials（RFC 6749 §4.3） | client_secret（Basic） ＋ 利用者の ID・パスワード |
    ///
    /// **ROPC は、OAuth 2.0 Security BCP（RFC 9700 §2.4）で使ってはならないとされる。**
    /// 汎用認証サイトの自己テストから移したもので、**新しく使うなら、`device` か `pkce` にする。**
    /// スタブは既定で閉じているので、`Sample4SpRp.stub.json` で開けている。
    /// </remarks>
    public static class ClientGrants
    {
        /// <summary>Client Credentials</summary>
        /// <param name="discovery">Discovery</param>
        /// <returns>成功したら true</returns>
        public static async Task<bool> RunClientCredentialsAsync(Discovery discovery)
        {
            string response = await OAuth2AndOIDCClient.ClientCredentialsGrantAsync(
                discovery.TokenEndpoint,
                GetConfigParameter.GetConfigValue("SpRp_BatchClientID"),
                GetConfigParameter.GetConfigValue("SpRp_BatchClientSecret"),
                GetConfigParameter.GetConfigValue("SpRp_BatchScope"));

            return ClientGrants.ReadTokenResponse(response) != null;
        }

        /// <summary>JWT Bearer</summary>
        /// <param name="discovery">Discovery</param>
        /// <returns>成功したら true</returns>
        /// <remarks>
        /// **アサーションの aud はトークン エンドポイント**（RFC 7523 §3 の 3）。**有効期間は短くする**（使い回されても、すぐ使えなくなるように）。
        /// **scope はアサーションに入れる**（`JwtBearerTokenFlowAsync` は、要求に scope を付けない）。
        /// </remarks>
        public static async Task<bool> RunJwtBearerAsync(Discovery discovery)
        {
            string clientId = GetConfigParameter.GetConfigValue("SpRp_BatchClientID");

            DigitalSignX509 dsX509 = new DigitalSignX509(
                CmnClientParams.RsaPfxFilePath, CmnClientParams.RsaPfxPassword, HashAlgorithmName.SHA256);
            RSAParameters rsa = ((RSA)dsX509.AsymmetricAlgorithm).ExportParameters(true);

            string assertion = JwtAssertion.CreateByRsa(
                clientId, discovery.TokenEndpoint.AbsoluteUri, TimeSpan.FromMinutes(1),
                GetConfigParameter.GetConfigValue("SpRp_BatchScope"), rsa);

            string response = await OAuth2AndOIDCClient.JwtBearerTokenFlowAsync(discovery.TokenEndpoint, assertion);

            return ClientGrants.ReadTokenResponse(response) != null;
        }

        /// <summary>Resource Owner Password Credentials</summary>
        /// <param name="discovery">Discovery</param>
        /// <param name="userId">利用者の ID</param>
        /// <returns>成功したら true</returns>
        /// <remarks>
        /// **パスワードは標準入力から読む**（コマンド ラインに書くと、プロセスの一覧から見える）。
        /// **利用者の ID は、利用者名**（MPAS のログイン画面で入れるメール アドレスではない。テスト利用者なら `tanaka`）。
        /// **id_token を検証し、UserInfo の sub と照合する**（`pkce` と同じ確かめ方）。
        /// </remarks>
        public static async Task<bool> RunRopcAsync(Discovery discovery, string userId)
        {
            string password = ClientGrants.ReadPassword(userId);

            string response = await OAuth2AndOIDCClient.ResourceOwnerPasswordCredentialsGrantAsync(
                discovery.TokenEndpoint,
                GetConfigParameter.GetConfigValue("SpRp_BatchClientID"),
                GetConfigParameter.GetConfigValue("SpRp_BatchClientSecret"),
                userId, password,
                GetConfigParameter.GetConfigValue("SpRp_Scope"));

            JObject token = ClientGrants.ReadTokenResponse(response);
            if (token == null)
            {
                return false;
            }

            string accessToken = (string)token[OAuth2AndOIDCConst.AccessToken];
            string idToken = (string)token[OAuth2AndOIDCConst.IDToken];

            // **openid を求めたので、id_token が要る**（無ければ、誰のトークンかを確かめられない）
            if (string.IsNullOrEmpty(idToken))
            {
                Console.WriteLine("NG   openid を求めたのに、id_token がありません。");
                return false;
            }

            // **署名・iss・aud・exp・at_hash を確かめる**（nonce は送っていない）
            JObject claims = await IdTokenVerifier.VerifyAsync(
                idToken, accessToken, null, CmnClientParams.Isser,
                GetConfigParameter.GetConfigValue("SpRp_BatchClientID"), null);
            if (claims == null)
            {
                return false;
            }

            string json = await OAuth2AndOIDCClient.GetUserInfoAsync(discovery.UserInfoEndpoint, accessToken);
            JObject userInfo = JObject.Parse(json);

            if (userInfo.ContainsKey(OAuth2AndOIDCConst.error))
            {
                Console.WriteLine("NG   UserInfo が断られました : " + DeviceFlow.ErrorOf(userInfo));
                return false;
            }

            // **UserInfo の sub は、id_token の sub と一致しなければならない**（OpenID Connect Core 1.0 §5.3.2）
            if ((string)userInfo[OAuth2AndOIDCConst.sub] != (string)claims[OAuth2AndOIDCConst.sub])
            {
                Console.WriteLine("NG   UserInfo の sub が、id_token の sub と違います。");
                return false;
            }

            Console.WriteLine("OK   id_token を検証し、UserInfo を得ました（利用者 " + userId + "）。");
            Console.WriteLine(userInfo.ToString(Formatting.Indented));
            return true;
        }

        /// <summary>トークン応答を読む</summary>
        /// <param name="response">応答</param>
        /// <returns>応答（失敗なら null）</returns>
        /// <remarks>**トークンそのものは表示しない**（token_type・expires_in・scope だけ）。</remarks>
        private static JObject ReadTokenResponse(string response)
        {
            JObject token = JObject.Parse(response);

            if (token.ContainsKey(OAuth2AndOIDCConst.error))
            {
                Console.WriteLine("NG   トークン要求が断られました : " + DeviceFlow.ErrorOf(token));
                return null;
            }

            if (string.IsNullOrEmpty((string)token[OAuth2AndOIDCConst.AccessToken]))
            {
                Console.WriteLine("NG   access_token がありません。");
                return null;
            }

            Console.WriteLine(string.Format(
                "OK   access_token を得ました（token_type = {0}、expires_in = {1}、scope = {2}）。",
                (string)token[OAuth2AndOIDCConst.token_type],
                (string)token[OAuth2AndOIDCConst.expires_in],
                (string)token[OAuth2AndOIDCConst.scope]));
            return token;
        }

        /// <summary>パスワードを標準入力から読む</summary>
        /// <param name="userId">利用者名（端末なら、誰のパスワードかを示す）</param>
        /// <returns>パスワード</returns>
        /// <remarks>**リダイレクトされていれば 1 行を読み、端末なら表示せずに読む。**</remarks>
        private static string ReadPassword(string userId)
        {
            if (Console.IsInputRedirected)
            {
                // **先頭の BOM は読み飛ばす**（Windows PowerShell 5.1 から流し込むと付くことがある）
                return (Console.In.ReadLine() ?? "").TrimStart('\uFEFF');
            }

            Console.Write("利用者 " + userId + " のパスワード : ");
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        sb.Length--;
                    }
                    continue;
                }
                sb.Append(key.KeyChar);
            }
            Console.WriteLine();
            return sb.ToString();
        }
    }
}

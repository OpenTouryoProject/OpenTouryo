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
//* クラス名        ：PkceFlow
//* クラス日本語名  ：認可コード ＋ PKCE（RFC 7636）。戻り先はループバック（RFC 8252 §7.3）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/04  玄人 幸道         新規作成（#649）
//**********************************************************************************

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security.Pwd;
using Touryo.Infrastructure.Public.Util;

namespace SpRp_CLI
{
    /// <summary>認可コード ＋ PKCE（ループバック）</summary>
    /// <remarks>
    /// **ネイティブ アプリの推奨のフロー**（RFC 8252）。
    /// システムのブラウザで認可し、**自分の端末の上の HTTP（ループバック）で応答を受ける。**
    ///
    /// **確かめること**
    /// | 値 | 何のため |
    /// |---|---|
    /// | `state` | 自分が出した要求への応答か（CSRF。RFC 6749 §10.12） |
    /// | `iss`（応答に在れば） | どの IdP の応答か（Mix-Up。RFC 9207） |
    /// | `code_verifier` | コードを横取りされても引き換えられない（RFC 7636） |
    /// | id_token の署名・`iss`・`aud`・`nonce` | OpenID Connect Core 1.0 §3.1.3.7 |
    /// </remarks>
    public class PkceFlow
    {
        /// <summary>ブラウザからの応答を待つ上限</summary>
        private static readonly TimeSpan ResponseTimeout = TimeSpan.FromMinutes(5);

        /// <summary>フローを実行する</summary>
        /// <param name="discovery">Discovery</param>
        /// <returns>成功なら true</returns>
        public static async Task<bool> RunAsync(Discovery discovery)
        {
            string client_id = OAuth2AndOIDCParams.ClientID;
            string redirect_uri = CmnClientParams.SpRp_RedirectUri;
            string scope = GetConfigParameter.GetConfigValue("SpRp_Scope");

            // **毎回、作り直す**（使い回すと、確かめる意味が無くなる）
            string state = GetPassword.Base64UrlSecret(32);
            string nonce = GetPassword.Base64UrlSecret(32);
            string code_verifier = GetPassword.Base64UrlSecret(32); // 43 文字（RFC 7636 §4.1 の下限）
            string code_challenge = OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod(code_verifier);

            string authorizeUrl = discovery.AuthorizationEndpoint.AbsoluteUri
                + "?" + OAuth2AndOIDCConst.client_id + "=" + Uri.EscapeDataString(client_id)
                + "&" + OAuth2AndOIDCConst.response_type + "=" + OAuth2AndOIDCConst.AuthorizationCodeResponseType
                + "&" + OAuth2AndOIDCConst.scope + "=" + Uri.EscapeDataString(scope)
                + "&" + OAuth2AndOIDCConst.redirect_uri + "=" + Uri.EscapeDataString(redirect_uri)
                + "&" + OAuth2AndOIDCConst.state + "=" + Uri.EscapeDataString(state)
                + "&" + OAuth2AndOIDCConst.nonce + "=" + Uri.EscapeDataString(nonce)
                + "&" + OAuth2AndOIDCConst.code_challenge + "=" + Uri.EscapeDataString(code_challenge)
                + "&" + OAuth2AndOIDCConst.code_challenge_method + "=" + OAuth2AndOIDCConst.PKCE_S256;

            // ---------------------------------------------------------------- 1. 認可要求と応答
            NameValueCollection query = await PkceFlow.AuthorizeInBrowserAsync(authorizeUrl, redirect_uri);
            if (query == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(query[OAuth2AndOIDCConst.error]))
            {
                Console.WriteLine(string.Format("NG   認可が断られました : {0}（{1}）",
                    query[OAuth2AndOIDCConst.error], query[OAuth2AndOIDCConst.error_description]));
                return false;
            }

            if (query[OAuth2AndOIDCConst.state] != state)
            {
                Console.WriteLine("NG   state が一致しません（自分が出した要求への応答ではない）。");
                return false;
            }

            string iss = query[OAuth2AndOIDCConst.iss];
            if (!string.IsNullOrEmpty(iss) && iss != discovery.Issuer)
            {
                Console.WriteLine("NG   応答の iss（" + iss + "）が、期待する IdP と違います。");
                return false;
            }

            string code = query[OAuth2AndOIDCConst.code];

            // ---------------------------------------------------------------- 2. トークン要求（秘密は送らない）
            JObject token = JObject.Parse(await OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                discovery.TokenEndpoint, client_id, "", redirect_uri, code, code_verifier));

            if (token.ContainsKey(OAuth2AndOIDCConst.error))
            {
                Console.WriteLine("NG   トークン要求が断られました : " + DeviceFlow.ErrorOf(token));
                return false;
            }

            Console.WriteLine("OK   トークンを得ました。");

            string access_token = (string)token[OAuth2AndOIDCConst.AccessToken];
            string id_token = (string)token[OAuth2AndOIDCConst.IDToken];

            // ---------------------------------------------------------------- 3. id_token の検証
            if (!string.IsNullOrEmpty(id_token))
            {
                JObject claims = await IdTokenVerifier.VerifyAsync(
                    id_token, access_token, code, discovery.Issuer, client_id, nonce);

                if (claims == null)
                {
                    return false;
                }

                Console.WriteLine("OK   id_token を検証しました（sub : " + (string)claims["sub"] + "）。");
            }
            else if (scope.Split(' ').Contains("openid"))
            {
                // **openid を求めたのに来ないのは、おかしい**（OpenID Connect Core 1.0 §3.1.3.3）
                Console.WriteLine("NG   scope に openid を含めたのに、id_token がありません。");
                return false;
            }

            // ---------------------------------------------------------------- 4. UserInfo
            return await UserInfo.ShowAsync(discovery, access_token);
        }

        /// <summary>ブラウザで認可要求を出し、ループバックで応答を受ける</summary>
        /// <param name="authorizeUrl">認可要求の URL</param>
        /// <param name="redirect_uri">戻り先（ループバック）</param>
        /// <returns>応答のクエリ（受けられなければ null）</returns>
        private static async Task<NameValueCollection> AuthorizeInBrowserAsync(
            string authorizeUrl, string redirect_uri)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("NG   この環境では HttpListener を使えません。");
                return null;
            }

            HttpListener listener = new HttpListener();

            // **戻り先は IdP に登録した値と、1 文字も違ってはならない**（末尾の / も含む）
            listener.Prefixes.Add(redirect_uri);

            try
            {
                // **ブラウザを開く前に待ち受ける**（先に開くと、応答を取りこぼすことがある）
                listener.Start();

                Console.WriteLine("ブラウザで認可します（開かなければ、次を手で開いてください）。");
                Console.WriteLine("  " + authorizeUrl);
                PkceFlow.OpenBrowser(authorizeUrl);

                Task<HttpListenerContext> getContext = listener.GetContextAsync();
                if (await Task.WhenAny(getContext, Task.Delay(PkceFlow.ResponseTimeout)) != getContext)
                {
                    Console.WriteLine("NG   応答を待ちきれませんでした（" + PkceFlow.ResponseTimeout.TotalMinutes + " 分）。");
                    return null;
                }

                HttpListenerContext context = getContext.Result;
                NameValueCollection query = context.Request.QueryString;

                // **ブラウザに返す画面は、結果だけ**（トークンは画面に出さない）
                byte[] body = Encoding.UTF8.GetBytes(
                    "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><title>SpRp_CLI</title></head>"
                    + "<body><p>応答を受け取りました。このタブを閉じて、端末に戻ってください。</p></body></html>");
                context.Response.ContentType = "text/html; charset=utf-8";
                context.Response.ContentLength64 = body.Length;
                context.Response.OutputStream.Write(body, 0, body.Length);
                context.Response.OutputStream.Close();

                return query;
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine("NG   " + redirect_uri + " で待ち受けられません : " + ex.Message);
                return null;
            }
            finally
            {
                listener.Close();
            }
        }

        /// <summary>システムのブラウザで URL を開く</summary>
        /// <param name="url">URL</param>
        /// <remarks>開けなくても止めない（URL は表示してある）。</remarks>
        private static void OpenBrowser(string url)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // **UseShellExecute で既定のブラウザに渡す**（cmd /c start は & を解釈してしまう）
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    Process.Start("xdg-open", url);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("     ブラウザを開けませんでした : " + ex.Message);
            }
        }
    }
}

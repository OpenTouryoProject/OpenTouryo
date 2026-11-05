//**********************************************************************************
//* LIRログインCLIサンプル アプリ
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Program
//* クラス日本語名  ：LIRログインCLIサンプル アプリ
//*                   LIR：Loopback Interface Redirection of OAuth 2.0 for Native Apps
//*
//* 作成日時        ：－
//* 作成者          ：開発基盤部会
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*  2026/11/05  玄人 幸道         認証処理を MPAS のスタブに合わせた（#649）
//*                                （エンドポイントを Discovery から引く・scope と redirect_uri を送る・
//*                                  state・iss・nonce を確かめる・id_token を検証する・応答を待つ上限・終了コードを返す・
//*                                  ブラウザを開かない指定（--no-browser）を足す）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Http;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Public.Security.Pwd;
using Touryo.Infrastructure.Framework.Authentication;

using System.CommandLine;
using System.CommandLine.Invocation;

using Sharprompt;

using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LIR_Login_CLI
{
    /// <summary>Program</summary>
    public class Program
    {
        /// <summary>ブラウザからの応答を待つ上限</summary>
        private static readonly TimeSpan ResponseTimeout = TimeSpan.FromMinutes(5);

        /// <summary>
        /// 'async main' は C# 7.1 以上の言語バージョンが必要（→ VS 2019）。
        /// </summary>
        /// <param name="args">string[]</param>
        /// <returns>int</returns>
        static async Task<int> Main(string[] args)
        {
            // 初期化
            string dir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory
                .FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            GetConfigParameter.InitConfiguration(dir + "/appsettings.json");
            // OAuth PKCE用
            OAuth2AndOIDCClient.HttpClient = new HttpClient();

            #region rootCommand
            // Create a root command with some options
            Command rootCommand = new RootCommand
            {
                // alias、default value、description
                new Option<int>(
                    alias: "--int-option",
                    getDefaultValue: () => 42,
                    description: "An option whose argument is parsed as an int"),
                new Option<bool>(
                    alias: "--bool-option",
                    description: "An option whose argument is parsed as a bool"),
                new Option<FileInfo>(
                    alias: "--file-option",
                    description: "An option whose argument is parsed as a FileInfo"),
                new Option<FileAccess>(
                    alias: "--file-access-option",
                    getDefaultValue: () => FileAccess.Read,
                    description: "An option whose argument is parsed as a FileAccess")
            };

            rootCommand.Description = "My sample app";

            // Note that the parameters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler.Create<int, bool, FileInfo, FileAccess>(Program.RootCommand);
            #endregion

            #region subCommand

            #region subCommand1
            Command subCommand1 = new Command(name: "cmd1", description: "Sub command cmd1");
            subCommand1.AddOption(new Option<int>(alias: "--an-int"));
            subCommand1.Handler = CommandHandler.Create<int>(
                (int anInt) =>
                {
                    Console.WriteLine($"Sub command cmd1: {anInt}");
                }
            );
            rootCommand.AddCommand(subCommand1);
            #endregion

            #region subCommand2
            Command subCommand2 = new Command(name: "cmd2", description: "Sub command cmd2");
            subCommand2.AddOption(new Option<string>(alias: "--a-string"));
            subCommand2.Handler = CommandHandler.Create<string>(
                (string aString) =>
                {
                    Console.WriteLine($"Sub command cmd2: {aString}");
                }
            );
            rootCommand.AddCommand(subCommand2);
            #endregion

            #region LoginCommand
            Command LoginCommand = new Command(name: "login", description: "Sub command login");
            LoginCommand.AddOption(new Option<string>(alias: "--a-string"));
            // **ブラウザを開かない**（表示した URL を、手で、または別の端末で開く。E2E もこれを使う）
            LoginCommand.AddOption(new Option<bool>(alias: "--no-browser", description: "Do not open the browser (open the displayed URL yourself)"));
            // **終了コードを返す**（成功なら 0、失敗なら 1）
            LoginCommand.Handler = CommandHandler.Create<string, bool>((Func<string, bool, Task<int>>)Program.LoginCommand);
            rootCommand.AddCommand(LoginCommand);
            #endregion

            #endregion

            // テストの実行
            await Program.Test(rootCommand);

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }

        #region Command

        #region RootCommand
        /// <summary>RootCommand</summary>
        /// <param name="intOption">int</param>
        /// <param name="boolOption">bool</param>
        /// <param name="fileOption">FileInfo</param>
        /// <param name="fileAccessOption">FileAccess</param>
        private static void RootCommand(
            int intOption, bool boolOption, FileInfo fileOption, FileAccess fileAccessOption)
        {
            Console.WriteLine(
                $"--int-option is: {intOption}, " +
                $"--bool-option is: {boolOption}, " +
                $"--file-option is: {fileOption?.Name ?? "null"}, " +
                $"--file-access-option is: {fileAccessOption.ToString()}");
        }
        #endregion

        #region LoginCommand
        /// <summary>LoginCommand</summary>
        /// <param name="aString">aString</param>
        /// <param name="noBrowser">ブラウザを開かない（--no-browser）</param>
        /// <returns>終了コード（成功なら 0、失敗なら 1）</returns>
        /// <remarks>
        /// 認可コード ＋ PKCE（RFC 7636）。戻り先はループバック（RFC 8252 §7.3）。
        /// 接続先とクライアントは appsettings.json にある（MPAS のスタブと、その雛形のクライアント。Sample4SpRp/README.md）。
        /// **エンドポイントは Discovery から引く**（issuer を SpRp_Isser と照合する）。
        ///
        /// **確かめること**
        /// | 値 | 何のため |
        /// |---|---|
        /// | `state` | 自分が出した要求への応答か（CSRF。RFC 6749 §10.12） |
        /// | `iss`（応答に在れば） | どの IdP の応答か（Mix-Up。RFC 9207） |
        /// | `code_verifier` | コードを横取りされても引き換えられない（RFC 7636） |
        /// | id_token の署名・`iss`・`aud`・`nonce` | OpenID Connect Core 1.0 §3.1.3.7 |
        /// </remarks>
        private static async Task<int> LoginCommand(string aString, bool noBrowser)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("HttpListener is not supported.");
                return 1;
            }

            Console.WriteLine($"Sub command login: {aString}");

            // Discovery
            JObject discovery = await Program.LoadDiscoveryAsync();
            if (discovery == null) return 1;

            string issuer = (string)discovery[OAuth2AndOIDCConst.issuer];
            Uri authorizeEndpointUri = Program.ToUri(discovery, "authorization_endpoint");
            Uri tokenEndpointUri = Program.ToUri(discovery, "token_endpoint");
            Uri userInfoEndpointUri = Program.ToUri(discovery, "userinfo_endpoint");

            // パラメタ
            //   **state・nonce・code_verifier は、毎回作り直す**（使い回すと、確かめる意味が無くなる）
            string client_id = OAuth2AndOIDCParams.ClientID;
            string redirect_uri = CmnClientParams.SpRp_RedirectUri;
            string scope = GetConfigParameter.GetConfigValue("SpRp_Scope");
            string state = GetPassword.Base64UrlSecret(32);
            string nonce = GetPassword.Base64UrlSecret(32);
            string code_verifier = GetPassword.Base64UrlSecret(32); // 43 文字（RFC 7636 §4.1 の下限）
            string code_challenge = OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod(code_verifier);

            // **値はエンコードする**（scope の空白など）。**redirect_uri は、トークン要求と同じ値を送る**（RFC 6749 §4.1.3）
            string target = authorizeEndpointUri.AbsoluteUri
                + "?" + OAuth2AndOIDCConst.client_id + "=" + Uri.EscapeDataString(client_id)
                + "&" + OAuth2AndOIDCConst.response_type + "=" + OAuth2AndOIDCConst.AuthorizationCodeResponseType
                + "&" + OAuth2AndOIDCConst.scope + "=" + Uri.EscapeDataString(scope)
                + "&" + OAuth2AndOIDCConst.redirect_uri + "=" + Uri.EscapeDataString(redirect_uri)
                + "&" + OAuth2AndOIDCConst.state + "=" + Uri.EscapeDataString(state)
                + "&" + OAuth2AndOIDCConst.nonce + "=" + Uri.EscapeDataString(nonce)
                + "&" + OAuth2AndOIDCConst.code_challenge + "=" + Uri.EscapeDataString(code_challenge)
                + "&" + OAuth2AndOIDCConst.code_challenge_method + "=" + OAuth2AndOIDCConst.PKCE_S256;

            // 認可リクエスト・レスポンス
            NameValueCollection query = await Program.AuthorizeInBrowserAsync(target, redirect_uri, !noBrowser);
            if (query == null) return 1;

            if (!string.IsNullOrEmpty(query[OAuth2AndOIDCConst.error]))
            {
                Console.WriteLine(string.Format("ABNORMAL_END : 認可が断られました : {0}（{1}）",
                    query[OAuth2AndOIDCConst.error], query[OAuth2AndOIDCConst.error_description]));
                return 1;
            }

            if (query[OAuth2AndOIDCConst.state] != state)
            {
                Console.WriteLine("ABNORMAL_END : state が一致しません（自分が出した要求への応答ではない）。");
                return 1;
            }

            string iss = query[OAuth2AndOIDCConst.iss];
            if (!string.IsNullOrEmpty(iss) && iss != issuer)
            {
                Console.WriteLine("ABNORMAL_END : 応答の iss（" + iss + "）が、期待する IdP と違います。");
                return 1;
            }

            string code = query[OAuth2AndOIDCConst.code];

            // Tokenリクエスト（パブリック クライアントなので、秘密は送らない）
            JObject token = JObject.Parse(await OAuth2AndOIDCClient
                .GetAccessTokenByCodeAsync(
                    tokenEndpointUri, client_id, "",
                    redirect_uri, code, code_verifier));

            if (token.ContainsKey(OAuth2AndOIDCConst.error))
            {
                Console.WriteLine("ABNORMAL_END : トークン要求が断られました : " + Program.ErrorOf(token));
                return 1;
            }

            string access_token = (string)token[OAuth2AndOIDCConst.AccessToken];
            string id_token = (string)token[OAuth2AndOIDCConst.IDToken];

            // id_token の検証
            if (!string.IsNullOrEmpty(id_token))
            {
                string sub = await Program.VerifyIdTokenAsync(
                    id_token, access_token, code, issuer, client_id, nonce);
                if (sub == null) return 1;

                Console.WriteLine("id_token is verified (sub: " + sub + ")");
            }
            else if (scope.Split(' ').Contains("openid"))
            {
                // **openid を求めたのに来ないのは、おかしい**（OpenID Connect Core 1.0 §3.1.3.3）
                Console.WriteLine("ABNORMAL_END : scope に openid を含めたのに、id_token がありません。");
                return 1;
            }

            // UserInfoリクエスト
            return await Program.ShowUserInfoAsync(userInfoEndpointUri, access_token) ? 0 : 1;
        }

        /// <summary>ブラウザで認可要求を出し、ループバックで応答を受ける</summary>
        /// <param name="target">認可要求の URL</param>
        /// <param name="redirect_uri">戻り先（ループバック）</param>
        /// <param name="openBrowser">ブラウザを開くか</param>
        /// <returns>応答のクエリ（受けられなければ null）</returns>
        private static async Task<NameValueCollection> AuthorizeInBrowserAsync(string target, string redirect_uri, bool openBrowser)
        {
            HttpListener listener = new HttpListener();

            // **戻り先は IdP に登録した値と、1 文字も違ってはならない**（末尾の / も含む）
            listener.Prefixes.Add(redirect_uri);

            try
            {
                // レスポンスの準備
                //   **ブラウザを開く前に待ち受ける**（先に開くと、応答を取りこぼすことがある）
                listener.Start();
                Console.WriteLine("Listening...");

                // 認可リクエスト（開かなければ、表示した URL を手で開く）
                //   **URL は 1 行で出す**（E2E は、この行を読んで認可要求を出す）
                Console.WriteLine(target);
                if (openBrowser)
                {
                    Program.OpenBrowser(target);
                }

                // 受信
                Task<HttpListenerContext> getContext = listener.GetContextAsync();
                if (await Task.WhenAny(getContext, Task.Delay(Program.ResponseTimeout)) != getContext)
                {
                    Console.WriteLine("ABNORMAL_END : 応答を待ちきれませんでした（" + Program.ResponseTimeout.TotalMinutes + " 分）。");
                    return null;
                }

                HttpListenerContext context = getContext.Result;
                NameValueCollection query = context.Request.QueryString;

                // 返信（**結果だけを返す。トークンは画面に出さない**）
                byte[] buffer = Encoding.UTF8.GetBytes(
                    "<HTML><BODY>Accepted the authorization response.</BODY></HTML>");
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();

                return query;
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine("ABNORMAL_END : " + redirect_uri + " で待ち受けられません : " + ex.Message);
                return null;
            }
            finally
            {
                // 後処理
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
                    // Windowsのとき
                    //   **UseShellExecute で既定のブラウザに渡す**（cmd /c start は & を解釈してしまう）
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // Macのとき
                    Process.Start("open", url);
                }
                else
                {
                    // Linuxのとき
                    Process.Start("xdg-open", url);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ブラウザを開けませんでした : " + ex.Message);
            }
        }
        #endregion

        #endregion

        #region 認証の共通（Discovery・id_token・UserInfo）

        /// <summary>OP の Discovery（OpenID Connect Discovery 1.0）を読む</summary>
        /// <returns>Discovery（読めない、または issuer が違えば null）</returns>
        /// <remarks>
        /// **issuer を照合する**（OpenID Connect Discovery 1.0 §4.3）。
        /// 照合しないと、Discovery の URL を差し替えられたときに、別の OP の口へ送ってしまう。
        /// </remarks>
        private static async Task<JObject> LoadDiscoveryAsync()
        {
            string discoveryUri = CmnClientParams.SpRp_DiscoveryUri;
            string json = await OAuth2AndOIDCClient.GetDiscoveryDocumentAsync(new Uri(discoveryUri));

            // **繋がらないときは、例外ではなく空が返る**（GetDiscoveryDocumentAsync）
            if (string.IsNullOrEmpty(json))
            {
                Console.WriteLine("ABNORMAL_END : Discovery（" + discoveryUri + "）を読めません。IdP（スタブ）は起動していますか？");
                return null;
            }

            JObject discovery = JObject.Parse(json);
            string issuer = (string)discovery[OAuth2AndOIDCConst.issuer];

            if (issuer != CmnClientParams.Isser)
            {
                Console.WriteLine(string.Format(
                    "ABNORMAL_END : Discovery の issuer（{0}）が、期待する値（{1}）と違います。", issuer, CmnClientParams.Isser));
                return null;
            }

            return discovery;
        }

        /// <summary>Discovery の項目を Uri にする</summary>
        /// <param name="discovery">Discovery</param>
        /// <param name="name">項目名</param>
        /// <returns>Uri（無ければ null）</returns>
        private static Uri ToUri(JObject discovery, string name)
        {
            string value = (string)discovery[name];
            return string.IsNullOrEmpty(value) ? null : new Uri(value);
        }

        /// <summary>相手（OP）の id_token を、jwks_uri の鍵で検証する</summary>
        /// <param name="id_token">id_token</param>
        /// <param name="access_token">access_token（at_hash の検証）</param>
        /// <param name="code">code（c_hash の検証）</param>
        /// <param name="issuer">期待する iss</param>
        /// <param name="client_id">期待する aud</param>
        /// <param name="expectedNonce">認可要求で送った nonce（null なら見ない）</param>
        /// <returns>sub（検証に失敗したら null）</returns>
        /// <remarks>
        /// **鍵は、ヘッダの kid で jwks_uri から引く**（RemoteJwkSetStore。鍵のロールオーバーに追随する）。
        /// **alg は非対称のもの（JwsFactory.SupportedAlgs）に限る**（none・HS* を受けない）。
        /// </remarks>
        private static async Task<string> VerifyIdTokenAsync(
            string id_token, string access_token, string code,
            string issuer, string client_id, string expectedNonce)
        {
            string alg = JwtReader.TryReadAlg(id_token);
            string kid = JwtReader.TryReadKid(id_token);

            if (!JwsFactory.IsSupported(alg))
            {
                Console.WriteLine("ABNORMAL_END : id_token の alg（" + alg + "）は受けません。");
                return null;
            }

            JObject jwk = await RemoteJwkSetStore.GetJwkByDiscoveryAsync(kid);
            if (jwk == null)
            {
                Console.WriteLine("ABNORMAL_END : id_token の kid（" + kid + "）の鍵を、jwks_uri から引けません。");
                return null;
            }

            JWS jws = JwsFactory.CreateFromJwk(alg, jwk);
            if (jws == null)
            {
                Console.WriteLine("ABNORMAL_END : id_token の alg（" + alg + "）と、鍵の種類が合いません。");
                return null;
            }

            // **署名・iss・aud・exp と、at_hash / c_hash を確かめる**（IdToken.Verify。#601）
            if (!IdToken.Verify(id_token, access_token, code, "",
                jws, issuer, client_id, out string sub, out string nonce, out JObject _))
            {
                Console.WriteLine("ABNORMAL_END : id_token の検証に失敗しました（署名・iss・aud・exp・ハッシュのいずれか）。");
                return null;
            }

            // **nonce は、呼ぶ側が確かめる**（IdToken.Verify は取り出すだけ。OpenID Connect Core 1.0 §3.1.3.7 の 11）
            if (expectedNonce != null && nonce != expectedNonce)
            {
                Console.WriteLine("ABNORMAL_END : id_token の nonce が、認可要求で送った値と違います。");
                return null;
            }

            return sub;
        }

        /// <summary>UserInfo を引いて表示する</summary>
        /// <param name="userInfoEndpointUri">userinfo_endpoint</param>
        /// <param name="access_token">access_token</param>
        /// <returns>引けたら true</returns>
        /// <remarks>
        /// **エラーの応答も JSON で返る**（insufficient_scope など。RFC 6750 §3.1）。
        /// 表示するだけでは成功と区別できないので、error を見て失敗にする。
        /// </remarks>
        private static async Task<bool> ShowUserInfoAsync(Uri userInfoEndpointUri, string access_token)
        {
            string json = await OAuth2AndOIDCClient.GetUserInfoAsync(userInfoEndpointUri, access_token);

            JObject userInfo;
            try
            {
                userInfo = JObject.Parse(json);
            }
            catch (JsonReaderException)
            {
                Console.WriteLine("ABNORMAL_END : UserInfo の応答を JSON として読めません : " + json);
                return false;
            }

            if (userInfo.ContainsKey(OAuth2AndOIDCConst.error))
            {
                Console.WriteLine("ABNORMAL_END : UserInfo が断られました : " + Program.ErrorOf(userInfo));
                return false;
            }

            Console.WriteLine("NORMAL_END :");
            Console.WriteLine(userInfo.ToString(Formatting.Indented));
            return true;
        }

        /// <summary>エラーの応答を、表示用の文字列にする</summary>
        /// <param name="response">応答</param>
        /// <returns>error（error_description）</returns>
        private static string ErrorOf(JObject response)
        {
            string error = (string)response[OAuth2AndOIDCConst.error];
            string description = (string)response[OAuth2AndOIDCConst.error_description];

            // **error_description は任意**（RFC 6749 §5.2）。無ければ括弧を付けない
            return string.IsNullOrEmpty(description) ? error : string.Format("{0}（{1}）", error, description);
        }

        #endregion

        #region TEST
        /// <summary>Test</summary>
        /// <param name="rootCommand">Command</param>
        /// <returns>Task</returns>
        private static async Task Test(Command rootCommand)
        {
            // デバッグ実行時だけ実行
            if (!Debugger.IsAttached) return;

            #region rootCommand
            await rootCommand.InvokeAsync("");
            // --int-option
            await rootCommand.InvokeAsync("--int-option");
            await rootCommand.InvokeAsync("--int-option 123");
            await rootCommand.InvokeAsync("--int-option hoge");
            // --bool-option
            await rootCommand.InvokeAsync("--bool-option");
            await rootCommand.InvokeAsync("--bool-option False");
            await rootCommand.InvokeAsync("--bool-option True");
            await rootCommand.InvokeAsync("--bool-option hoge");
            // --file-option
            await rootCommand.InvokeAsync("--file-option ../Program.cs");
            // --file-access-option
            await rootCommand.InvokeAsync("--file-access-option Read");
            await rootCommand.InvokeAsync("--file-access-option Write");
            await rootCommand.InvokeAsync("--file-access-option hoge");
            #endregion

            #region subCommand
            // subCommand1
            await rootCommand.InvokeAsync("cmd1 --an-int 123");
            // subCommand2
            await rootCommand.InvokeAsync("cmd2 --a-string hoge");
            // subCommandLogin
            await rootCommand.InvokeAsync("login");
            #endregion
        }
        #endregion
    }
}

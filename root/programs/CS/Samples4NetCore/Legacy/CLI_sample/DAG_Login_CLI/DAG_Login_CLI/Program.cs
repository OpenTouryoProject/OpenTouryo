//**********************************************************************************
//* DAGログインCLIサンプル アプリ
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Program
//* クラス日本語名  ：DAGログインCLIサンプル アプリ
//*                   DAG : OAuth 2.0 Device Authorization Grant
//*
//* 作成日時        ：－
//* 作成者          ：開発基盤部会
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*  2026/11/05  玄人 幸道         認証処理を MPAS のスタブに合わせた（#649）
//*                                （エンドポイントを Discovery から引く・scope を送る・verification_uri をそのまま使う・
//*                                  ポーリングを IdP の指示に従わせる・id_token を検証する・終了コードを返す・
//*                                  確認を飛ばす指定（--yes）を足す）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Net.Http;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Framework.Authentication;

using System.CommandLine;
using System.CommandLine.Invocation;

using Sharprompt;

using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DAG_Login_CLI
{
    /// <summary>Program</summary>
    public class Program
    {
        /// <summary>interval の既定（RFC 8628 §3.2 : 無ければ 5 秒）</summary>
        private const int DefaultIntervalSeconds = 5;

        /// <summary>slow_down で延ばす秒数（RFC 8628 §3.5）</summary>
        private const int SlowDownSeconds = 5;

        /// <summary>
        /// 'async main' は C# 7.1 以上の言語バージョンが必要（→ VS 2019）。
        /// </summary>
        /// <param name="args">string[]</param>
        /// <returns>int</returns>
        static async Task<int> Main(string[] args)
        {
            // 初期化
            // configの初期化
            string dir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory
                .FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            GetConfigParameter.InitConfiguration(dir + "/appsettings.json");
            // デバイスフロー用
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
            // **確認（Sharprompt）を飛ばす**（対話の無い環境で動かす。E2E もこれを使う）
            LoginCommand.AddOption(new Option<bool>(alias: "--yes", description: "Start polling without the confirmation"));
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
        /// <param name="yes">確認を飛ばす（--yes）</param>
        /// <returns>終了コード（成功なら 0、失敗なら 1）</returns>
        /// <remarks>
        /// DAG : OAuth 2.0 Device Authorization Grant（RFC 8628）。
        /// 接続先とクライアントは appsettings.json にある（MPAS のスタブと、その雛形のクライアント。Sample4SpRp/README.md）。
        /// **エンドポイントは Discovery から引く**（issuer を SpRp_Isser と照合する）。
        /// **ポーリングは IdP の指示に従う**（RFC 8628 §3.5）。
        /// | 応答の error | すること |
        /// |---|---|
        /// | `authorization_pending` | `interval` 秒あけて、もう一度 |
        /// | `slow_down` | **`interval` を 5 秒延ばして**、もう一度 |
        /// | `access_denied` / `expired_token` / その他 | やめる |
        /// </remarks>
        private static async Task<int> LoginCommand(string aString, bool yes)
        {
            Console.WriteLine($"Sub command login: {aString}");

            // Discovery
            JObject discovery = await Program.LoadDiscoveryAsync();
            if (discovery == null) return 1;

            string issuer = (string)discovery[OAuth2AndOIDCConst.issuer];
            Uri deviceAuthZEndpointUri = Program.ToUri(discovery, "device_authorization_endpoint");
            Uri tokenEndpointUri = Program.ToUri(discovery, "token_endpoint");
            Uri userInfoEndpointUri = Program.ToUri(discovery, "userinfo_endpoint");

            if (deviceAuthZEndpointUri == null)
            {
                Console.WriteLine("ABNORMAL_END : IdP が Device AuthZ を広告していません（device_authorization_endpoint が無い）。");
                return 1;
            }

            // パラメタ
            string client_id = OAuth2AndOIDCParams.ClientID;
            string scope = GetConfigParameter.GetConfigValue("SpRp_Scope");

            // デバイス認可要求（§3.1）
            //   **scope を送る**（#650）。openid を含めないと、id_token が返らず、UserInfo も insufficient_scope で断られる。
            JObject authz = JObject.Parse(await OAuth2AndOIDCClient
                .DeviceAuthZRequestAsync(deviceAuthZEndpointUri, client_id, scope));

            if (authz.ContainsKey(OAuth2AndOIDCConst.error))
            {
                Console.WriteLine("ABNORMAL_END : デバイス認可要求が断られました : " + Program.ErrorOf(authz));
                return 1;
            }

            string device_code = (string)authz[OAuth2AndOIDCConst.device_code];
            int interval = (int?)authz["interval"] ?? Program.DefaultIntervalSeconds;
            int expiresIn = (int?)authz[OAuth2AndOIDCConst.expires_in] ?? 0;

            // 結果表示
            //   **verification_uri は絶対 URI**（§3.2）。**別の端末で開く**ので、組み立て直さない。
            Console.WriteLine("userCode: " + (string)authz[OAuth2AndOIDCConst.user_code]);
            Console.WriteLine("verificationUri: " + (string)authz[OAuth2AndOIDCConst.verification_uri]);
            Console.WriteLine("verificationUriComplete: " + (string)authz[OAuth2AndOIDCConst.verification_uri_complete]);

            // ポーリング開始
            //   **--yes なら確かめない**（Sharprompt は対話環境でしか動かない。入力をリダイレクトすると例外になる）
            bool answer = yes || Prompt.Confirm("Are you ready?");
            Console.WriteLine($"Your answer is {answer}");
            if (!answer)
            {
                Console.WriteLine("ABNORMAL_END : 中止しました。");
                return 1;
            }

            // ポーリング（§3.4・§3.5）
            DateTime deadline = (expiresIn > 0) ? DateTime.UtcNow.AddSeconds(expiresIn) : DateTime.MaxValue;
            JObject token = null;

            while (token == null)
            {
                if (DateTime.UtcNow >= deadline)
                {
                    Console.WriteLine("ABNORMAL_END : 期限（expires_in）を過ぎました。");
                    return 1;
                }

                await Task.Delay(TimeSpan.FromSeconds(interval));
                Console.WriteLine("... polling ...");

                // Tokenリクエスト
                JObject response = JObject.Parse(await OAuth2AndOIDCClient
                    .GetAccessTokenByDeviceAuthZAsync(tokenEndpointUri, client_id, device_code));

                string error = (string)response[OAuth2AndOIDCConst.error];

                if (string.IsNullOrEmpty(error))
                {
                    // 正常系
                    token = response;
                }
                else if (error == "authorization_pending")
                {
                    // 承認を待っている
                }
                else if (error == "slow_down")
                {
                    interval += Program.SlowDownSeconds;
                }
                else
                {
                    // access_denied（利用者が拒否した）、expired_token（期限切れ）など
                    Console.WriteLine("ABNORMAL_END : " + Program.ErrorOf(response));
                    return 1;
                }
            }

            string access_token = (string)token[OAuth2AndOIDCConst.AccessToken];
            string id_token = (string)token[OAuth2AndOIDCConst.IDToken];

            // id_token の検証
            //   **Device AuthZ には nonce が無い**（認可要求をブラウザから出さないため）。署名・iss・aud・exp・at_hash を見る。
            if (!string.IsNullOrEmpty(id_token))
            {
                string sub = await Program.VerifyIdTokenAsync(
                    id_token, access_token, "", issuer, client_id, null);
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

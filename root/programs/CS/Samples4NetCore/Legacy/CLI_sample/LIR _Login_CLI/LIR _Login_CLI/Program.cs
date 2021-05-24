﻿//**********************************************************************************
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
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Http;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.FastReflection;
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
        /// <summary>
        /// 'async main' は C# 7.1 以上の言語バージョンが必要（→ VS 2019）。
        /// </summary>
        /// <param name="args">string[]</param>
        /// <returns>int</returns>
        static async Task<int> Main(string[] args)
        {
            // 初期化
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
            LoginCommand.Handler = CommandHandler.Create<string>(Program.LoginCommand);
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
        private static async Task LoginCommand(string aString)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("HttpListener is not supported.");
                return;
            }

            Console.WriteLine($"Sub command login: {aString}");

            // リクエスト

            // URL
            string rootURI = "https://localhost:44300/MultiPurposeAuthSite";
            string oAuth2AuthorizeEndpoint = "/authorize";
            string oAuth2TokenEndpoint = "/token";
            string oAuth2UselInfoEndpoint = "/userinfo";

            // パラメタ
            string client_id = "ae5a179813234ca290c8de93ef2e31dc";
            string redirect_uri = "http://localhost:12345/";
            string state = GetPassword.Generate(10, 0);
            string code_verifier = GetPassword.Base64UrlSecret(50);
            string code_challenge = OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod(code_verifier);
            string target = rootURI + oAuth2AuthorizeEndpoint + string.Format(
                "?client_id={0}&response_type={1}&scope={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                    client_id,
                    OAuth2AndOIDCConst.AuthorizationCodeResponseType,
                    OAuth2AndOIDCConst.email_verified,
                    state, code_challenge,
                    OAuth2AndOIDCConst.PKCE_S256);

            // URI
            Uri tokenEndpointUri = new Uri(rootURI + oAuth2TokenEndpoint);
            Uri uselInfoEndpointUri = new Uri(rootURI + oAuth2UselInfoEndpoint);
            //Uri authorizeEndpointUri = new Uri(target);

            #region 認可リクエスト・レスポンス
            HttpListener listener = null;
            HttpListenerRequest listenerRequest = null;
            HttpListenerResponse listenerResponse = null;
            Stream output = null;
            try
            {
                #region レスポンスの準備
                // リスナーを起動して受信
                listener = new HttpListener();
                listener.Prefixes.Add(redirect_uri);
                listener.Start();
                Console.WriteLine("Listening...");
                #endregion

                #region 認可リクエスト。
                try
                {
                    // ブラウザを起動して送信
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        //Windowsのとき  
                        target = target.Replace("&", "^&");
                        Process.Start(
                            new ProcessStartInfo("cmd", $"/c start {target}") { CreateNoWindow = true });
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        // Linuxのとき  
                        Process.Start("xdg-open", target);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        // Macのとき  
                        Process.Start("open", target);
                    }
                    else
                    {
                        throw new Exception("Unknown OS platform.");
                    }
                }
                catch (Win32Exception noBrowser)
                {

                    Console.WriteLine(noBrowser.Message);
                    return;
                }
                catch (System.Exception other)
                {
                    Console.WriteLine(other.Message);
                    return;
                }

                // 受信と返信
                HttpListenerContext context = listener.GetContext();
                listenerRequest = context.Request;
                listenerResponse = context.Response;

                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(
                    "<HTML><BODY>Accepted the authorization response.</BODY></HTML>");

                // 返信
                listenerResponse.ContentLength64 = buffer.Length;
                output = listenerResponse.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                #endregion
            }
            finally
            {
                // 後処理
                output.Close();
                listener.Stop();
            }
            #endregion

            string code = listenerRequest.QueryString["code"];

            // Tokenリクエスト
            // GetAccessTokenByDeviceAuthZAsync
            string response = await OAuth2AndOIDCClient
                .GetAccessTokenByCodeAsync(
                    tokenEndpointUri, client_id, "",
                    redirect_uri, code, code_verifier);

            JObject temp = (JObject)JsonConvert.DeserializeObject(response);

            if (!temp.ContainsKey(OAuth2AndOIDCConst.error))
            {
                // 正常系

                // UserInfoリクエスト
                // GetUserInfoAsync
                string userInfo = await OAuth2AndOIDCClient
                    .GetUserInfoAsync(uselInfoEndpointUri, (string)temp[OAuth2AndOIDCConst.AccessToken]);

                Console.WriteLine("NORMAL_END :");
                Console.WriteLine(userInfo);
            }
            else
            {
                // 異常系
                Console.WriteLine("ABNORMAL_END");
            }
        }
        #endregion

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

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
//* クラス名        ：Program
//* クラス日本語名  ：SpRp サンプル（CLI）の入口
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/04  玄人 幸道         新規作成（#649）
//*  2026/11/05  玄人 幸道         pkce に --no-browser を足した（#649。E2E で使う）
//*  2026/11/06  玄人 幸道         コメントの設定キーを、改名後の SpRp_Issuer にした（#666）
//*  2026/11/07  玄人 幸道         cc・jwtbearer・ropc を足した（#668。汎用認証サイトの自己テストから移した）
//**********************************************************************************

using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Util;

namespace SpRp_CLI
{
    /// <summary>SpRp サンプル（CLI）の入口</summary>
    /// <remarks>
    /// **フローをサブコマンドで選ぶ。**
    /// | サブコマンド | フロー |
    /// |---|---|
    /// | `device` | Device Authorization Grant（RFC 8628） |
    /// | `pkce`   | 認可コード ＋ PKCE（RFC 7636）。戻り先はループバック（RFC 8252 §7.3） |
    /// | `cc`・`jwtbearer`・`ropc` | ブラウザを使わないグラント（`ClientGrants`） |
    ///
    /// **`device`・`pkce` はパブリック クライアント**（秘密を持たない）。利用者の端末に配る CLI は秘密を守れないため。
    /// **`cc`・`jwtbearer`・`ropc` は、サーバで動くバッチを模したコンフィデンシャル クライアント**（秘密と鍵を設定ファイルに持つ）。
    /// </remarks>
    public class Program
    {
        /// <summary>終了コード：正常</summary>
        public const int ExitOk = 0;

        /// <summary>終了コード：フローが失敗した（拒否・期限切れ・検証の失敗など）</summary>
        public const int ExitFailed = 1;

        /// <summary>終了コード：引数の誤り</summary>
        public const int ExitUsage = 2;

        /// <summary>ブラウザを開かない（表示した URL を、手で、または別の端末で開く。E2E もこれを使う）</summary>
        private const string NoBrowserOption = "--no-browser";

        /// <summary>入口</summary>
        /// <param name="args">string[]</param>
        /// <returns>終了コード</returns>
        public static async Task<int> Main(string[] args)
        {
            if (args.Length < 1 || 2 < args.Length)
            {
                Program.WriteUsage();
                return Program.ExitUsage;
            }

            string command = args[0].ToLowerInvariant();
            if (command != "device" && command != "pkce"
                && command != "cc" && command != "jwtbearer" && command != "ropc")
            {
                Program.WriteUsage();
                return Program.ExitUsage;
            }

            // **2 つ目の引数は、pkce の --no-browser と、ropc の利用者の ID だけ**（device はもともとブラウザを開かない）
            bool openBrowser = true;
            string userId = null;
            if (command == "ropc")
            {
                if (args.Length != 2)
                {
                    Program.WriteUsage();
                    return Program.ExitUsage;
                }
                userId = args[1];
            }
            else if (args.Length == 2)
            {
                if (command != "pkce" || args[1].ToLowerInvariant() != Program.NoBrowserOption)
                {
                    Program.WriteUsage();
                    return Program.ExitUsage;
                }
                openBrowser = false;
            }

            Program.Initialize();

            try
            {
                // **エンドポイントは Discovery から引く**（設定には issuer と Discovery の URL だけを書く）。
                Discovery discovery = await Discovery.LoadAsync(
                    CmnClientParams.SpRp_DiscoveryUri, CmnClientParams.Isser);

                bool result;
                switch (command)
                {
                    case "device": result = await DeviceFlow.RunAsync(discovery); break;
                    case "pkce": result = await PkceFlow.RunAsync(discovery, openBrowser); break;
                    case "cc": result = await ClientGrants.RunClientCredentialsAsync(discovery); break;
                    case "jwtbearer": result = await ClientGrants.RunJwtBearerAsync(discovery); break;
                    default: result = await ClientGrants.RunRopcAsync(discovery, userId); break;
                }

                return result ? Program.ExitOk : Program.ExitFailed;
            }
            catch (HttpRequestException ex)
            {
                // **IdP（スタブ）が起動していないと、ここに来る**
                Console.WriteLine("NG   IdP に接続できません : " + ex.Message);
                Console.WriteLine("     スタブは起動していますか？（Sample4SpRp/README.md の 2 節）");
                return Program.ExitFailed;
            }
            catch (InvalidOperationException ex)
            {
                // **Discovery を読めない、または issuer が設定（SpRp_Issuer）と違う**
                Console.WriteLine("NG   " + ex.Message);
                return Program.ExitFailed;
            }
            catch (JsonReaderException ex)
            {
                // **IdP の応答が JSON でない**（エラーの画面が返った、など）
                Console.WriteLine("NG   IdP の応答を JSON として読めません : " + ex.Message);
                return Program.ExitFailed;
            }
        }

        /// <summary>設定と HttpClient を初期化する</summary>
        private static void Initialize()
        {
#if NETCOREAPP
            // **net10.0 は appsettings.json を明示的に読む**（net48 は App.config を自動で読む）。
            string dir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory
                .FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            GetConfigParameter.InitConfiguration(dir + "/appsettings.json");
#endif

            // **HttpClient は 1 つを使い回す**（OAuth2AndOIDCClient の静的プロパティ）。
            OAuth2AndOIDCClient.HttpClient = new HttpClient();
        }

        /// <summary>使い方を表示する</summary>
        private static void WriteUsage()
        {
            Console.WriteLine("使い方 : SpRp_CLI <device | pkce [--no-browser] | cc | jwtbearer | ropc <利用者の ID>>");
            Console.WriteLine("  device    : Device Authorization Grant（RFC 8628）。表示されたコードを、ブラウザで入力する");
            Console.WriteLine("  pkce      : 認可コード ＋ PKCE（RFC 7636）。ブラウザが開き、ループバックで応答を受ける");
            Console.WriteLine("              --no-browser : ブラウザを開かない（表示した URL を手で開く）");
            Console.WriteLine("  cc        : Client Credentials（RFC 6749 4.4）。サーバで動くバッチを模した、秘密を持つクライアント");
            Console.WriteLine("  jwtbearer : JWT Bearer（RFC 7523 2.1）。同上のクライアントの鍵で、アサーションに署名する");
            Console.WriteLine("  ropc      : Resource Owner Password Credentials（RFC 6749 4.3。非推奨）。パスワードは標準入力から読む");
            Console.WriteLine();
            Console.WriteLine("終了コード : 0 = 成功、1 = 失敗、2 = 引数の誤り");
        }
    }
}

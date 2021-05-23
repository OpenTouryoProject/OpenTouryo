//**********************************************************************************
//* 単純CLIサンプル アプリ
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Program
//* クラス日本語名  ：単純CLIサンプル アプリ
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

using System.CommandLine;
using System.CommandLine.Invocation;

using Sharprompt;

using Newtonsoft;
using Newtonsoft.Json;

namespace Simple_CLI
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

            #region subCommandComplex
            Command subCommandComplex = new Command(name: "complex", description: "Sub command complex")
            {
                new Option<int>("--an-int"),
                new Option<string>("--a-string")
            };
            subCommandComplex.Handler = CommandHandler.Create((ComplexType complexType) =>
            {
                Console.WriteLine($"Sub command complex: {JsonConvert.SerializeObject(complexType)}");
            });
            rootCommand.AddCommand(subCommandComplex);
            #endregion

            #region subCommandInteractive
            Command subCommandInteractive = new Command(name: "interactive", description: "Sub command interactive");
            subCommandInteractive.AddOption(new Option<string>(alias: "--a-string"));
            subCommandInteractive.Handler = CommandHandler.Create((string aString) =>
            {
                Console.WriteLine($"Sub command interactive (Ctrl-C terminate): {aString}");
                
                Prompt.ColorSchema.Answer = ConsoleColor.DarkRed;
                Prompt.ColorSchema.Select = ConsoleColor.DarkCyan;
                Console.OutputEncoding = Encoding.UTF8;

                string name = Prompt.Input<string>("名前", validators: new[] { Validators.Required() });
                Console.WriteLine($"こんにちは, {name}");

                int age = Prompt.Input<int>("年齢");
                Console.WriteLine($"年齢: {age}");

                string password = Prompt.Password("Type new password");
                Console.WriteLine("Password OK");

                bool answer = Prompt.Confirm("Are you ready?");
                Console.WriteLine($"Your answer is {answer}");

                EnumMonth value = Prompt.Select<EnumMonth>("Select enum value");
                Console.WriteLine($"You selected {value}");

                Program.GetPrefectureList(out string[] prefectureList);
                string prefecture = Prompt.Select("都道府県", prefectureList, pageSize: 5);
                Console.WriteLine($"都道府県: {prefecture}");
            });
            rootCommand.AddCommand(subCommandInteractive);
            #endregion

            #region subCommandDelay
            Command subCommandDelay = new Command(name: "delay", description: "Sub command delay");
            subCommandDelay.AddOption(new Option<string>(alias: "--a-string"));
            subCommandDelay.Handler = CommandHandler.Create<string, IConsole, CancellationToken>(
                async (string aString, IConsole console, CancellationToken token) =>
                {
                    Console.WriteLine($"Sub command delay(Ctrl-C terminate): {aString}");

                    try
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Console.WriteLine($"Sub command delay: {i}");

                            await Task.Delay(1000, token);
                        }

                        // 例えば...
                        //using (var httpClient = new HttpClient())
                        //{
                        //    await httpClient.GetAsync("http://www.example.com", token);
                        //}

                        Console.WriteLine("Sub command delay was completed.");
                        return 0;
                    }
                    catch (OperationCanceledException)
                    {
                        // ココのコードは.NET Fx系だとハングする。
                        // .NET Core系だとハングしない。
                        Console.WriteLine("Sub command delay was aborted.");
                        return 1;
                    }
                }
            );
            rootCommand.AddCommand(subCommandDelay);
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
            // subCommandComplex
            await rootCommand.InvokeAsync("complex --an-int 123 --a-string hogehoge");
            // subCommandInteractive
            await rootCommand.InvokeAsync("interactive --a-string hoge");
            // subCommandDelay
            await rootCommand.InvokeAsync("delay --a-string hogehoge");
            #endregion
        }
        #endregion

        #region SELECT

        /// <summary>EnumMonth</summary>
        private enum EnumMonth : byte
        {
            January = 1, February, March, April,
            May, June, July, August,
            September, October, November, December
        }

        /// <summary>GetPrefectureList</summary>
        /// <param name="prefectureList">string[]</param>
        private static void GetPrefectureList(out string[] prefectureList)
        {
            prefectureList = new[]
            {
              "北海道",
              "青森県",
              "岩手県",
              "宮城県",
              "秋田県",
              "山形県",
              "福島県",
              "茨城県",
              "栃木県",
              "群馬県",
              "埼玉県",
              "千葉県",
              "東京都",
              "神奈川県",
              "新潟県",
              "富山県",
              "石川県",
              "福井県",
              "山梨県",
              "長野県",
              "岐阜県",
              "静岡県",
              "愛知県",
              "三重県",
              "滋賀県",
              "京都府",
              "大阪府",
              "兵庫県",
              "奈良県",
              "和歌山県",
              "鳥取県",
              "島根県",
              "岡山県",
              "広島県",
              "山口県",
              "徳島県",
              "香川県",
              "愛媛県",
              "高知県",
              "福岡県",
              "佐賀県",
              "長崎県",
              "熊本県",
              "大分県",
              "宮崎県",
              "鹿児島県",
              "沖縄県"
            };
        }
        #endregion
    }

    /// <summary>ComplexType</summary>
    public class ComplexType
    {
        // public ComplexType(int anInt, string aString)
        // {
        //     AnInt = anInt;
        //     AString = aString;
        // }
        public int AnInt { get; set; }
        public string AString { get; set; }
    }
}

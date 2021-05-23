//**********************************************************************************
//* 単純CLIサンプル アプリ
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
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Threading;
using System.Diagnostics;

using Sharprompt;

using System.CommandLine;
using System.CommandLine.Invocation;

using Newtonsoft;
using Newtonsoft.Json;

namespace DAG_Login_CLI
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
        private static void LoginCommand(string aString)
        {
            Console.WriteLine($"Sub command login: {aString}");
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

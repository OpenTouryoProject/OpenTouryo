//**********************************************************************************
//* クラス名        ：Program
//* クラス日本語名  ：修正ファイルの配布ツール
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
                new Option<FileInfo>(
                    alias: "--src-file",
                    description: "Copy source file."),
                new Option<DirectoryInfo>(
                    alias: "--dst-root-directory",
                    description: "Copy destination root directory."),
                new Option<string>(
                    alias: "--excluded-directory-list",
                    getDefaultValue: () => "bin;obj;.git;.svn;.vs;packages;node_modules;temp",
                    description: "Semicolon delimited excluded directory list."),
            };

            rootCommand.Description = "My sample app";

            // Note that the parameters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler.Create< FileInfo, DirectoryInfo, string> (Program.RootCommand);
            #endregion

            // テストの実行
            await Program.Test(rootCommand);

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }

        #region Command

        /// <summary>StringBuilder</summary>
        static StringBuilder sb4Log = null;
        
        /// <summary>RootCommand</summary>
        /// <param name="srcFile">FileInfo</param>
        /// <param name="dstRootDirectory">DirectoryInfo</param>
        /// <param name="excludedDirectoryList">string</param>
        private static void RootCommand(
            FileInfo srcFile, DirectoryInfo dstRootDirectory, string excludedDirectoryList)
        {   
            // 引数確認
            Console.WriteLine(
                $"--src-file is: {srcFile?.Name ?? "null"}, " +
                $"--dst-root-directory is: {dstRootDirectory?.Name?? "null"}, " +
                $"--excluded-directory-list is: {excludedDirectoryList}, ");

            // 実行準備
            Program.sb4Log = new StringBuilder();
            string[] _excludedDirectoryList = excludedDirectoryList.Split(';');

            // 再帰実行
            Console.CursorVisible = false;
            Program.WalkDirectoryTree(srcFile, dstRootDirectory, _excludedDirectoryList);
            Console.CursorVisible = true;

            // 終了処理
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("- Result Log -");
            Console.WriteLine(Program.sb4Log.ToString());
            Console.ReadKey(); // デバッグ用
        }

        /// <summary>ShowProgress</summary>
        /// <param name="str">string</param>
        static void ShowProgress(string str)
        {
            int prePos = Console.CursorLeft;//現在カーソル位置を取得
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(str.PadRight(prePos));//前のカーソル位置まで空白埋めする

            //（進行が見えるように）処理を5ミリ秒間休止
            Thread.Sleep(5);
        }

        /// <summary>WalkDirectoryTree</summary>
        /// <param name="srcFile">FileInfo</param>
        /// <param name="currentDirectory">DirectoryInfo</param>
        /// <param name="excludedDirectoryList">string[]</param>
        static void WalkDirectoryTree(
            FileInfo srcFile, DirectoryInfo currentDirectory, string[] excludedDirectoryList)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;

            if (currentDirectory == null)
            {
                return;
            }
            else
            {
                // 進行状況を表示
                Program.ShowProgress(currentDirectory.Name);
            }

            // First, process all the files directly under this folder
            // 最初に、このフォルダ直下のすべてのファイルを処理する。
            files = currentDirectory.GetFiles("*.*");
            foreach (FileInfo fi in files)
            {
                // 進行状況を表示
                Program.ShowProgress(fi.Name);

                if (fi.Name.ToLower() == srcFile.Name.ToLower())
                {
                    // 置き換え
                    try
                    {
                        // 正常
                        srcFile.CopyTo(fi.FullName, true);
                        // ログ
                        sb4Log.AppendLine(fi.FullName);
                    }
                    catch (Exception e)
                    {
                        // 異常
                        // ログ
                        sb4Log.AppendLine(e.Message);
                    }
                }
            }

            subDirs = currentDirectory.GetDirectories();
            foreach (DirectoryInfo di in subDirs)
            {
                // チェック
                bool skip = false;
                foreach (string exDL in excludedDirectoryList)
                {
                    if (di.Name.ToLower() == exDL.ToLower())
                    {
                        // スキップ
                        skip = true;
                    }
                }

                // 再帰
                if (!skip)
                {
                    if(di != null)
                    Program.WalkDirectoryTree(srcFile, di, excludedDirectoryList);
                }
            }
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

            await rootCommand.InvokeAsync("--src-file OpenTouryo.DistributeFile_Tool.exe --dst-root-directory .");
        }
        #endregion
    }
}

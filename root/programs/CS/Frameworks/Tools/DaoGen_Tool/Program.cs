//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

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
//* クラス日本語名  ：アプリケーションのメイン エントリ ポイント
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2008/xx/xx  西野 大介         新規作成
//*  2014/04/30  Santosh san       Internationalization: Added Method to get the strings
//*                                from the resource files based on the keys values passed.
//*                                and replaced to this method wherever hard coded values.
//*                                Also Added code to get the Culture information from app.config file.
//*  2018/10/29  西野 大介         NETCOREAPP対応で、configの初期化
//*  2026/07/31  ＸＸ ＸＸ         CUI起動（/CUI）とヘルプ（/HELP）を追加
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Threading;
using System.Resources;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace DaoGen_Tool
{
    /// <summary>アプリケーションのメイン エントリ ポイント</summary>
    static class Program
    {
        /// <summary>CUI（非対話）で起動しているか</summary>
        private static bool _isCui = false;

        #region コンソール制御

        /// <summary>親プロセスのコンソールにアタッチする</summary>
        /// <param name="dwProcessId">-1 で親プロセス</param>
        /// <returns>成否</returns>
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int dwProcessId);

        /// <summary>コンソールをデタッチする</summary>
        /// <returns>成否</returns>
        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        #endregion

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
#if NETCOREAPP
            // configの初期化
            GetConfigParameter.InitConfiguration("appsettings.json");
#else
#endif
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                 string strDefaultCulture = "";

                 /// To Get the Culture info from app.config file 
                 strDefaultCulture = GetConfigParameter.GetConfigValue("DefaultCulture");
                 if (!string.IsNullOrEmpty(strDefaultCulture))
                 {
                     Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(strDefaultCulture);
                     Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(strDefaultCulture);
                 }

                // コマンドライン引数を取得する（キーは大文字化される）。
                List<string> valsLst;
                Dictionary<string, string> argsDic;
                StringVariableOperator.GetCommandArgs('/', out argsDic, out valsLst);

                // 起動の切り替え
                if (argsDic.ContainsKey("/HELP"))
                {
                    // ヘルプ
                    Program.AttachConsole(-1);
                    Program.WriteHelp();
                    Program.FreeConsole();
                }
                else if (argsDic.ContainsKey("/CUI"))
                {
                    // CUI起動（非対話）
                    Program._isCui = true;
                    Program.AttachConsole(-1);
                    try
                    {
                        Environment.ExitCode = Program.RunAsCui(argsDic);
                    }
                    catch (Exception cuiEx)
                    {
                        Console.WriteLine("エラー：" + cuiEx.ToString());
                        Environment.ExitCode = 2;
                    }
                    Program.FreeConsole();
                }
                else
                {
                    // GUI起動
                    Application.Run(new Form1());
                }
            }
            catch (Exception ex)
            {
                if (Program._isCui)
                {
                    // CUI起動時は MessageBox で停止させない。
                    Console.WriteLine(RM_GetString("EntryPoint") + ex.ToString());
                    Environment.ExitCode = 2;
                }
                else
                {
                    MessageBox.Show(RM_GetString("EntryPoint") + ex.Message);
                }
            }
        }

        #region CUI

        /// <summary>ヘルプを標準出力に表示する</summary>
        private static void WriteHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("DaoGen_Tool（D層自動生成ツール／墨壺）");
            Console.WriteLine("");
            Console.WriteLine("  引数なし          GUIで起動します。");
            Console.WriteLine("  /HELP             このヘルプを表示します。");
            Console.WriteLine("  /CUI              CUI（非対話）で生成処理を実行します。");
            Console.WriteLine("");
            Console.WriteLine("＜/CUI 時の共通引数＞");
            Console.WriteLine("");
            Console.WriteLine("  /MODE <DAODEFGEN|DAOSQLGEN>  実行する処理（既定:DAOSQLGEN）");
            Console.WriteLine("    DAODEFGEN  ＤＢのスキーマから、Ｄ層定義情報ファイル（*.csv）を生成");
            Console.WriteLine("    DAOSQLGEN  Ｄ層定義情報ファイルから、Dao・DTO・SQL を生成");
            Console.WriteLine("");
            Console.WriteLine("＜/MODE DAODEFGEN 時の引数＞");
            Console.WriteLine("");
            Console.WriteLine("  必須");
            Console.WriteLine("    /OUTPUT   <path>  出力先ファイル（*.csv）");
            Console.WriteLine("");
            Console.WriteLine("  任意");
            Console.WriteLine("    /DAP <SQL|OLE|ODB|ODP|DB2|MCN|NPS>  データ プロバイダ（既定:SQL）");
            Console.WriteLine("    /CONNSTR <string>                   接続文字列");
            Console.WriteLine("                                        （既定:設定ファイルの ConnectionString_*）");
            Console.WriteLine("    /TABLES <name,name,...>             生成対象（既定:全テーブル・ビュー）");
            Console.WriteLine("    /EXCLUDETABLES <name,name,...>      生成対象から除外");
            Console.WriteLine("    /PRIMARYKEYS <T1:C1|C2,T2:C3>       主キー（DBから取得できない場合に指定）");
            Console.WriteLine("    /CODEPAGE <n>                       出力のコード ページ（既定:65001）");
            Console.WriteLine("");
            Console.WriteLine("＜/MODE DAOSQLGEN 時の引数＞");
            Console.WriteLine("");
            Console.WriteLine("  必須");
            Console.WriteLine("    /DAODEF   <path>  Ｄ層定義情報ファイル（*.csv）へのパス");
            Console.WriteLine("    /TEMPLATE <path>  テンプレート ファイルのルート フォルダ");
            Console.WriteLine("    /OUTPUT   <path>  出力先フォルダ");
            Console.WriteLine("");
            Console.WriteLine("  任意");
            Console.WriteLine("    /DAP <SQL|OLE|ODB|ODP|DB2|MCN|NPS>  データ プロバイダ（既定:SQL）");
            Console.WriteLine("    /LANG <CS|VB>                       生成言語（既定:CS）");
            Console.WriteLine("    /ENTITY                             エンティティ（DTO）を生成");
            Console.WriteLine("    /TYPEDDATASET                       型付きデータセットを生成");
            Console.WriteLine("    /TABLEMAINTENANCE                   テーブル メンテナンス画面を生成");
            Console.WriteLine("    /ONLYDTO                            DTOのみ生成（Daoを生成しない）");
            Console.WriteLine("    /ONLYTABLEMAINTENANCE               メンテ画面のみ生成");
            Console.WriteLine("    /NOHEADER                           定義ファイルの1行目をヘッダとして扱わない");
            Console.WriteLine("    /ESCAPECHAR <c>                     エスケープ文字（ODP時は1文字必須）");
            Console.WriteLine("    /TSCOLNAME <name>                   タイム スタンプ列名");
            Console.WriteLine("    /TSUPDMETHOD <method>               タイム スタンプの更新方法");
            Console.WriteLine("    /FAMILYNAME <name>                  作成者（姓）");
            Console.WriteLine("    /PERSONALNAME <name>                作成者（名）");
            Console.WriteLine("    /XMLENCODING <name>                 SQL(XML)のエンコーディング（既定:utf-8）");
            Console.WriteLine("    /CODEPAGE <n>                       クラス ファイルのコード ページ（既定:65001）");
            Console.WriteLine("");
            Console.WriteLine("＜終了コード＞ 0:成功 / 1:引数エラー / 2:生成エラー");
            Console.WriteLine("");
            Console.WriteLine("＜パス区切りの注意＞");
            Console.WriteLine("  コマンドライン解析では \\ をエスケープ文字として扱うため、");
            Console.WriteLine("  パスの区切りには / を使用してください（\\\\ でも可）。");
            Console.WriteLine("    OK : /OUTPUT \"C:/temp/out\"");
            Console.WriteLine("    OK : /OUTPUT \"C:\\\\temp\\\\out\"");
            Console.WriteLine("    NG : /OUTPUT \"C:\\temp\\out\"   ← \\ が消えます");
            Console.WriteLine("");
            Console.WriteLine("＜/PRIMARYKEYS の書式＞");
            Console.WriteLine("  テーブル名 : 主キー列名 | 主キー列名 ... を、カンマ区切りで複数指定します。");
            Console.WriteLine("  主キー情報をDBMSから取得できないデータ プロバイダ");
            Console.WriteLine("  （ODBC・OLEDB・MySQL・PostgreSQL）で使用します。");
            Console.WriteLine("    例 : /PRIMARYKEYS \"Orders:OrderID,OrderDetails:OrderID|ProductID\"");
            Console.WriteLine("");
        }

        /// <summary>コマンドライン引数から値を取得する</summary>
        /// <param name="argsDic">コマンドライン引数</param>
        /// <param name="key">キー（"/XXX"）</param>
        /// <param name="defaultValue">既定値</param>
        /// <returns>値</returns>
        private static string GetArg(Dictionary<string, string> argsDic, string key, string defaultValue)
        {
            if (argsDic.ContainsKey(key) && !string.IsNullOrEmpty(argsDic[key]))
            {
                return argsDic[key];
            }
            return defaultValue;
        }

        /// <summary>コマンドライン引数から、カンマ区切りの値を配列で取得する</summary>
        /// <param name="argsDic">コマンドライン引数</param>
        /// <param name="key">キー（"/XXX"）</param>
        /// <returns>値の配列（指定が無い場合は空配列）</returns>
        private static string[] GetArrayArg(Dictionary<string, string> argsDic, string key)
        {
            string value = Program.GetArg(argsDic, key, "");

            if (string.IsNullOrEmpty(value))
            {
                return new string[0];
            }

            List<string> valueLst = new List<string>();
            foreach (string temp in value.Split(','))
            {
                if (temp.Trim() != "")
                {
                    valueLst.Add(temp.Trim());
                }
            }

            return valueLst.ToArray();
        }

        /// <summary>CUI（非対話）で生成処理を実行する</summary>
        /// <param name="argsDic">コマンドライン引数</param>
        /// <returns>終了コード（0:成功 / 1:引数エラー / 2:生成エラー）</returns>
        private static int RunAsCui(Dictionary<string, string> argsDic)
        {
            // 実行する処理の切り替え
            string mode = Program.GetArg(argsDic, "/MODE", "DAOSQLGEN").ToUpper();

            if (mode == "DAODEFGEN")
            {
                // ＤＢのスキーマ → Ｄ層定義情報ファイル
                return Program.RunDaoDefinitionGen(argsDic);
            }
            else if (mode == "DAOSQLGEN")
            {
                // Ｄ層定義情報ファイル → Dao・DTO・SQL
                return Program.RunDaoAndSqlGen(argsDic);
            }
            else
            {
                Console.WriteLine("引数エラー：/MODE には DAODEFGEN または DAOSQLGEN を指定してください。");
                Console.WriteLine("使用方法は /HELP で確認してください。");
                return 1;
            }
        }

        /// <summary>CUI（非対話）で、ＤＢのスキーマからＤ層定義情報ファイルを生成する</summary>
        /// <param name="argsDic">コマンドライン引数</param>
        /// <returns>終了コード（0:成功 / 1:引数エラー / 2:生成エラー）</returns>
        private static int RunDaoDefinitionGen(Dictionary<string, string> argsDic)
        {
            DaoDefinitionOptions opt = new DaoDefinitionOptions();

            // 出力先（必須）
            opt.OutputFilePath = Program.GetArg(argsDic, "/OUTPUT", "");

            if (string.IsNullOrEmpty(opt.OutputFilePath))
            {
                Console.WriteLine("引数エラー：/OUTPUT は必須です。");
                Console.WriteLine("使用方法は /HELP で確認してください。");
                return 1;
            }

            // データ プロバイダ
            opt.Dap = Program.GetArg(argsDic, "/DAP", "SQL").ToUpper();

            // 接続文字列（省略時は設定ファイルの値を使用する）
            opt.ConnectionString = Program.GetArg(argsDic, "/CONNSTR", "");

            if (string.IsNullOrEmpty(opt.ConnectionString))
            {
                string configKey = Program.GetConnectionStringKey(opt.Dap);

                if (configKey == "")
                {
                    Console.WriteLine("引数エラー：/DAP には SQL, OLE, ODB, ODP, DB2, MCN, NPS を指定してください。");
                    return 1;
                }

                opt.ConnectionString = GetConfigParameter.GetConfigValue(configKey);

                if (string.IsNullOrEmpty(opt.ConnectionString))
                {
                    Console.WriteLine("引数エラー：/CONNSTR が指定されておらず、"
                        + "設定ファイルにも " + configKey + " がありません。");
                    return 1;
                }
            }

            // 生成対象
            opt.Tables = Program.GetArrayArg(argsDic, "/TABLES");
            opt.ExcludeTables = Program.GetArrayArg(argsDic, "/EXCLUDETABLES");

            // 主キー（"T1:C1|C2,T2:C3" 形式）
            foreach (string temp in Program.GetArrayArg(argsDic, "/PRIMARYKEYS"))
            {
                int index = temp.IndexOf(':');

                if (index <= 0 || index == temp.Length - 1)
                {
                    Console.WriteLine("引数エラー：/PRIMARYKEYS の書式が不正です（" + temp + "）。");
                    Console.WriteLine("使用方法は /HELP で確認してください。");
                    return 1;
                }

                opt.PrimaryKeys[temp.Substring(0, index).Trim()]
                    = temp.Substring(index + 1).Split('|');
            }

            // エンコーディング
            int codePage;
            if (!int.TryParse(Program.GetArg(argsDic, "/CODEPAGE", "65001"), out codePage))
            {
                Console.WriteLine("引数エラー：/CODEPAGE には数値を指定してください。");
                return 1;
            }
            opt.CodePage = codePage;

            // 生成処理の呼び出し
            // ※ Form1 は表示しない。UIコントロールは初期化のみ行われる。
            Form1 form1 = new Form1();
            form1.IsCui = true;

            Console.WriteLine("生成を開始します。");
            bool result = form1.DaoDefinitionGen(opt);

            if (result)
            {
                Console.WriteLine("生成が完了しました。");
                return 0;
            }
            else
            {
                Console.WriteLine("生成に失敗しました。");
                return 2;
            }
        }

        /// <summary>データ プロバイダに対応する、接続文字列の設定キーを取得する</summary>
        /// <param name="dap">データ プロバイダ</param>
        /// <returns>設定キー（対応しないデータ プロバイダの場合は空文字）</returns>
        private static string GetConnectionStringKey(string dap)
        {
            switch (dap)
            {
                case "SQL": return "ConnectionString_SQL";
                case "OLE": return "ConnectionString_OLE";
                case "ODB": return "ConnectionString_ODBC";
                case "ODP": return "ConnectionString_ODP";
                case "DB2": return "ConnectionString_DB2";
                case "MCN": return "ConnectionString_MCN";
                case "NPS": return "ConnectionString_NPS";
                default: return "";
            }
        }

        /// <summary>CUI（非対話）で、Ｄ層定義情報ファイルから Dao・DTO・SQL を生成する</summary>
        /// <param name="argsDic">コマンドライン引数</param>
        /// <returns>終了コード（0:成功 / 1:引数エラー / 2:生成エラー）</returns>
        private static int RunDaoAndSqlGen(Dictionary<string, string> argsDic)
        {
            DaoGenOptions opt = new DaoGenOptions();

            // 入出力パス（必須）
            opt.DaoDefinitionFilePath = Program.GetArg(argsDic, "/DAODEF", "");
            opt.TemplateRootPath      = Program.GetArg(argsDic, "/TEMPLATE", "");
            opt.OutputPath            = Program.GetArg(argsDic, "/OUTPUT", "");

            if (string.IsNullOrEmpty(opt.DaoDefinitionFilePath)
                || string.IsNullOrEmpty(opt.TemplateRootPath)
                || string.IsNullOrEmpty(opt.OutputPath))
            {
                Console.WriteLine("引数エラー：/DAODEF, /TEMPLATE, /OUTPUT は必須です。");
                Console.WriteLine("使用方法は /HELP で確認してください。");
                return 1;
            }

            // データ プロバイダ・言語
            opt.Dap      = Program.GetArg(argsDic, "/DAP", "SQL").ToUpper();
            opt.IsCSharp = (Program.GetArg(argsDic, "/LANG", "CS").ToUpper() != "VB");

            // 生成対象
            opt.CreateEntity           = argsDic.ContainsKey("/ENTITY");
            opt.CreateTypedDataSet     = argsDic.ContainsKey("/TYPEDDATASET");
            opt.CreateTableMaintenance = argsDic.ContainsKey("/TABLEMAINTENANCE");
            opt.OnlyDTO                = argsDic.ContainsKey("/ONLYDTO");
            opt.OnlyTableMaintenance   = argsDic.ContainsKey("/ONLYTABLEMAINTENANCE");

            // 生成オプション
            opt.DaoDefinitionHeader = !argsDic.ContainsKey("/NOHEADER");
            opt.EscapeChar          = Program.GetArg(argsDic, "/ESCAPECHAR", "");
            opt.TimeStampColName    = Program.GetArg(argsDic, "/TSCOLNAME", "");
            opt.TimeStampUpdMethod  = Program.GetArg(argsDic, "/TSUPDMETHOD", "");
            opt.FamilyName          = Program.GetArg(argsDic, "/FAMILYNAME", "");
            opt.PersonalName        = Program.GetArg(argsDic, "/PERSONALNAME", "");

            // エンコーディング
            opt.XmlEncoding = Program.GetArg(argsDic, "/XMLENCODING", "utf-8");
            int codePage;
            if (!int.TryParse(Program.GetArg(argsDic, "/CODEPAGE", "65001"), out codePage))
            {
                Console.WriteLine("引数エラー：/CODEPAGE には数値を指定してください。");
                return 1;
            }
            opt.ClassFileCodePage = codePage;

            // 生成処理の呼び出し
            // ※ Form2 は表示しない。UIコントロールは初期化のみ行われる。
            Form2 form2 = new Form2();
            form2.IsCui = true;
            form2.Init(opt.Dap);
            // Form2_Load は表示時にしか発火しないため、設定の読み込みを明示的に呼ぶ。
            form2.LoadSettingsFromConfig();

            Console.WriteLine("生成を開始します。");
            bool result = form2.DaoAndSqlGen(opt);

            if (result)
            {
                Console.WriteLine("生成が完了しました。");
                return 0;
            }
            else
            {
                Console.WriteLine("生成に失敗しました。");
                return 2;
            }
        }

        #endregion
        
        /// <summary>This Method gets the string values from resource file based on the key passed</summary>
        private static string RM_GetString(string key)
        {
            ResourceManager rm = Resources.Resource.ResourceManager;
            return rm.GetString(key);
        }
    }
}

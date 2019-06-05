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
//* クラス日本語名  ：エントリポイント
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/04/18  西野 大介         新規作成
//*  2011/07/01  西野 大介         SelectionCriteriaDlgt2のSplit文字を修正
//*  2011/07/01  西野 大介         プロキシ設定、プロキシ認証のコードを修正
//*  2011/07/01  西野 大介         強制モードで、インストール ディレクトリ
//*                                の削除処理の前に存在チェックを追加した。
//*  2011/07/01  西野 大介         マニュフェストの起動exeのチェックエラー処理に
//*                                MyExceptionが使用されていたところを修正。
//*  2011/07/01  西野 大介         ZIP部位もマニュフェスト ファイル不正例外を使用
//*  2011/07/01  西野 大介         コマンドライン引数のハッシュキーを全て大文字化
//*  2011/07/04  西野 大介         インストール ディレクトリ変更は例外に
//*  2011/07/04  西野 大介         アンインストール相当の処理コマンドを追加した。
//*                                （ZIPを全て消去する方法ではEXE起動で例外になるので）
//*  2011/07/04  西野 大介         マニュフェスト ファイルのZIPのラインに何も無いとき、
//*                                処理が中断してしまう件を、例外を発生するよう修正
//*  2011/08/01  西野 大介         /QUIET クワイエット モードを追加した。
//*  2011/08/01  西野 大介         進捗値の進み具合を調整した。
//*  2011/08/30  西野 大介         /NB モードを追加した。
//*  2011/08/30  西野 大介         異常終了時に退避ディレクトリからリカバリ。
//*  2011/08/30  西野 大介         テンポラリの削除処理を追加した（例外処理を追加）。
//*  2011/09/01  西野 大介         オープン状態の場合、MyExceptionに振り替える。
//*  2011/09/01  西野 大介         モジュール比較を大文字化してからに変更。
//*  2011/09/01  西野 大介         マニュフェストファイルのfinally、Close処理を追加
//*  2011/09/01  西野 大介         サーバに接続できない場合の例外処理を追加した。
//*                                接続できない場合でEXE起動する場合はログを出力する。
//*  2011/09/05  西野 大介         GCするコードを追加→修正（解凍ZIPがCloseされないため）
//*  2011/09/05  西野 大介         起動プロセスのウィンドウを最前面にする処理を追加
//*  2011/09/08  西野 大介         リカバリ処理のログ出力位置の変更
//*                                （リカバリしない時ログを出力しないよう変更）。
//*  2011/09/09  西野 大介         先頭部分でインストール ディレクトリの
//*                                読み取り専用属性を消去する処理を追加。
//*  2011/09/12  西野 大介         画面表示せず、ログ出力のみする例外処理方式を追加
//*  2014/04/26  Sai               Replaced all the Japanese language in both UI and code with ResorceManager.GetString() method call
//*  2014/05/12  Sai               Removed <start> and <End> tags, added check while reading FxBusinessMessageCulture from app.config file
//*  2016/01/07  Sandeep           Comment out the DefaultWebProxy property, because there is no use of this property in the component   
//**********************************************************************************

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

using Microsoft.VisualBasic.FileIO;
using Ionic.Zip;

using Touryo.Infrastructure.Business.RichClient.Asynchronous;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;


namespace DeployZipPackWithHTTP
{
    /// <summary>メインと共通</summary>
    static class Program
    {
        // DOBON.NET
        // > ファイルをバイト型配列に読み込む、バイト型配列をファイルに書き込む
        // http://dobon.net/vb/dotnet/file/filestream.html
        // > 複数の配列をマージ（合併、連結、合体）する 
        // http://dobon.net/vb/dotnet/programing/arraymerge.html
        // > WebRequest、WebResponseクラスを使ってファイルをダウンロードし保存する
        // http://dobon.net/vb/dotnet/internet/webrequestsavefile.html
        // > 二重起動を禁止する > Mutexを使用する方法
        // http://dobon.net/vb/dotnet/process/checkprevinstance.html

        /// <summary>二重起動禁止Mutex</summary>
        private static System.Threading.Mutex _mutex;

        #region フラグ

        /// <summary>FORCE実行かどうか</summary>
        private static bool IsForce = false;

        /// <summary>起動フラグ</summary>
        private static bool IsBoot = true;

        #endregion

        #region 定数

        /// <summary>カレントファイル</summary>
        private static readonly string CurrentFileName = "\\current.bin";
        /// <summary>履歴ファイル</summary>
        private static readonly string HistoryFileName = "\\histories.bin";

        /// <summary>マニュフェスト一時ファイル</summary>
        public static readonly string TempMftFileName = "\\temp.mft";
        /// <summary>ZIP一時ファイル</summary>
        public static readonly string TempZipFileName = "\\temp.zip";

        #endregion

        #region 初回・初期化

        /// <summary>カレントディレクトリ</summary>
        public static string OrgCurrentDirectory = "";

        #endregion

        #region 都度・初期化

        /// <summary>履歴情報</summary>
        public static List<Entry> Histories = new List<Entry>();

        /// <summary>バックアップ（インストール）の有・無</summary>
        public static bool HaveBackup = false;
        /// <summary>インストール ディレクトリを保存</summary>
        public static string InstallDirectory = "";
        /// <summary>バックアップ ディレクトリを保存</summary>
        public static string BackupDirectory = "";

        /// <summary>
        /// Getting ResourceManager instance from Resources to apply internationalization
        /// </summary>
        private static ResourceManager ResourceMgr
        {
            get
            {
                return Resources.Resource.ResourceManager;
            }
        }

        #endregion

        #region Win32

        #region コンソール処理
        
        /// <summary>
        /// Allocates a new console for the calling process.
        /// </summary>
        [DllImport("kernel32.dll")]
        private static extern Boolean AllocConsole();

        /// <summary>
        /// Attaches the calling process to the console of the specified process.
        /// </summary>
        /// <param name="dwProcessId">
        /// コンソール プロセスのID（-1の場合、親プロセス）
        /// </param>
        [DllImport("kernel32.dll")]
        public static extern Boolean AttachConsole(int dwProcessId);

        /// <summary>
        /// Detaches the calling process from its console.
        /// </summary>
        [DllImport("kernel32.dll")]
        private static extern Boolean FreeConsole();

        #endregion

        #region ウィンドウ制御

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        // Z オーダーだけを目的に使う場合は
        // SWP_NOMOVE と SWP_NOSIZE を指定することで
        // (x, y), (cx, cy)を無視するようにする.
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002; 
        const uint TOPMOST_FLAGS = (SWP_NOSIZE | SWP_NOMOVE);

        /// <summary>
        /// 子ウィンドウ、ポップアップウィンドウ、
        /// またはトップレベルウィンドウの
        /// サイズ、位置、および Z オーダーを変更。
        /// これらのウィンドウは、その画面上
        /// での表示に従って順序が決められます。
        /// 最前面にあるウィンドウは最も高いランク
        /// を与えられ、Z オーダーの先頭に置かれます。
        /// http://msdn.microsoft.com/ja-jp/library/cc411206.aspx
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="hWndInsertAfter">
        /// Z オーダーを決めるためのウィンドウハンドル
        /// 
        /// ・HWND_BOTTOM
        /// 　ウィンドウを Z オーダーの最後に置きます。
        /// 
        /// ・hWnd
        /// 　パラメタで指定したウィンドウが最前面の場合、
        /// 　このウィンドウは最前面ウィンドウではなくなり、
        /// 　ほかのすべてのウィンドウの下に置かれます。 
        /// 
        /// ・HWND_NOTOPMOST
        /// 　ウィンドウを最前面ウィンドウ以外のすべてのウィンドウの前
        /// 　（つまり、すべての最前面ウィンドウの後ろ）に挿入します。
        /// 　
        /// 　hWnd パラメータで指定したウィンドウが
        /// 　既に最前面ウィンドウではなかった場合、
        /// 　このフラグは意味を持ちません。
        /// 　
        /// ・HWND_TOP
        /// 　ウィンドウを Z オーダーの先頭に置きます。 
        /// 　
        /// ・HWND_TOPMOST
        /// 　ウィンドウを最前面ウィンドウではない
        /// 　すべてのウィンドウの前に挿入します。
        /// 　このウィンドウは、アクティブでない
        /// 　ときにも最前面に表示されます。 
        /// </param>
        /// <param name="x">横方向の位置</param>
        /// <param name="y">縦方向の位置</param>
        /// <param name="cx">幅</param>
        /// <param name="cy">高さ</param>
        /// <param name="flags">
        /// ウィンドウ位置のオプション
        /// （詳細は上記のURLを参照のこと）
        /// </param>
        /// <returns>
        /// 成功：0 以外の値が返ります。
        /// 失敗：0 が返ります。
        /// 拡張エラー情報を取得するには、関数を使います。
        /// </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(
            IntPtr hWnd, IntPtr hWndInsertAfter,
            int x, int y, int cx, int cy, uint flags);

        #endregion

        #endregion

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Add DefaultCulture key in app.Config file and take the culture value from app.Config file.
            string culture = GetConfigParameter.GetConfigValue("DefaultCulture");
            if (!string.IsNullOrEmpty(culture))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(culture);
            }

            // exe名
            string exeName = Process.GetCurrentProcess().MainModule.ModuleName;

            // Mutexクラスの作成（exe名を利用する）
            Program._mutex = new Mutex(false, exeName);

            // ミューテックスの所有権を要求する
            if (Program._mutex.WaitOne(0, false) == false)
            {
                // 二重起動
                return;
            }
            else
            {
                // 二重起動でない。

                // カレントディレクトリの退避
                // ファイル選択ダイアログでカレントディレクトリが変わる

                // コマンドライン起動しても
                // ローカル ディレクトリを使用するように変更
                string[] temp = Process.GetCurrentProcess().MainModule.FileName.Split('\\');

                for (int i = 0; i < temp.Length - 1; i++)
                {
                    if (i == 0)
                    {
                        Program.OrgCurrentDirectory = temp[i];
                    }
                    else
                    {
                        Program.OrgCurrentDirectory += "\\" + temp[i];
                    }
                }

                // コマンドライン：値
                List<string> valsLst;
                // コマンドライン：コマンド・値
                Dictionary<string, string> argsDic;

                // コマンドライン引数を取得する。
                StringVariableOperator.GetCommandArgs('/', out argsDic, out valsLst);

                // ヘルプ
                if (argsDic.ContainsKey("/HELP"))
                {
                    // コンソール アタッチ
                    Program.AttachConsole(-1);

                    Console.WriteLine("");
                    
                    //Console.WriteLine("/HELP HELPを表示します。");
                    //Console.WriteLine("/CUI CUIで起動します。");
                    //Console.WriteLine("");
                    //Console.WriteLine("以下、CUI時のみ有効なオプション引数");
                    //Console.WriteLine("");
                    //Console.WriteLine("/FORCE 履歴ファイルを消去して強制更新します。");
                    //Console.WriteLine("/QUIET クワイエット モードで実行します（メッセージを非表示）。");
                    //Console.WriteLine("/SILENT サイレント モードで実行します（メッセージ、進捗を非表示）。");
                    //Console.WriteLine("/NB マニフェストファイルで起動指示アセンブリが指定されていても無視します。");
                    //Console.WriteLine("");
                    //Console.WriteLine("/WWWURL \"http://xxxx\" WWWサーバ上のマニュフェスト ファイルへのURLを指定します。");
                    //Console.WriteLine("");
                    //Console.WriteLine("/WWWUID xxxx WWWサーバへアクセスする際のユーザIDを指定します。");
                    //Console.WriteLine("/WWWPWD xxxx WWWサーバへアクセスする際のパスワードを指定します。");
                    //Console.WriteLine("/WWWDomain xxxx WWWサーバへアクセスする際のドメインを指定します。");
                    //Console.WriteLine("");
                    //Console.WriteLine("/ProxyURL \"http://yyyy\" ProxyサーバへのURLを指定します。");
                    //Console.WriteLine("                          指定の無い場合は、IE設定を適用します。");
                    //Console.WriteLine("                          [none]指定時はプロキシを使用しません。");
                    //Console.WriteLine("");
                    //Console.WriteLine("/ProxyUID yyyy Proxyサーバへアクセスする際のユーザIDを指定します。");
                    //Console.WriteLine("/ProxyPWD yyyy Proxyサーバへアクセスする際のパスワードを指定します。");
                    //Console.WriteLine("/ProxyDomain yyyy Proxyサーバへアクセスする際のドメインを指定します。");
                    //Console.WriteLine("");
                    //Console.WriteLine("/UnIns \"http://xxxx\" WWWサーバ上のマニュフェスト ファイルへのURLを指定します（アンインストール）。");
                    
                    //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                    Console.WriteLine(ResourceMgr.GetString("A0001"));
                    Console.WriteLine(ResourceMgr.GetString("A0002"));
                    Console.WriteLine("");
                    Console.WriteLine(ResourceMgr.GetString("A0003"));
                    Console.WriteLine("");
                    Console.WriteLine(ResourceMgr.GetString("A0004"));
                    Console.WriteLine(ResourceMgr.GetString("A0005"));
                    Console.WriteLine(ResourceMgr.GetString("A0006"));
                    Console.WriteLine(ResourceMgr.GetString("A0007"));
                    Console.WriteLine("");
                    Console.WriteLine(ResourceMgr.GetString("A0008"));
                    Console.WriteLine("");
                    Console.WriteLine(ResourceMgr.GetString("A0009"));
                    Console.WriteLine(ResourceMgr.GetString("A0010"));
                    Console.WriteLine(ResourceMgr.GetString("A0011"));
                    Console.WriteLine("");
                    Console.WriteLine(ResourceMgr.GetString("A0012"));
                    Console.WriteLine(ResourceMgr.GetString("A0013"));
                    Console.WriteLine(ResourceMgr.GetString("A0014"));
                    Console.WriteLine("");
                    Console.WriteLine(ResourceMgr.GetString("A0015"));
                    Console.WriteLine(ResourceMgr.GetString("A0016"));
                    Console.WriteLine(ResourceMgr.GetString("A0017"));
                    Console.WriteLine("");
                    Console.WriteLine(ResourceMgr.GetString("A0018"));

                    // コンソール デタッチ
                    Program.FreeConsole();

                    return;　// 終了
                }

                // アンインストール
                if (argsDic.ContainsKey("/UNINS"))
                {
                    string url = argsDic["/UNINS"];

                    // 履歴をロード
                    Program.LoadHistories();

                    // 履歴の検索
                    Entry history = null;
                    foreach (Entry entry in Program.Histories)
                    {
                        if (entry.WWWURL == url)
                        {
                            history = entry;
                        }
                    }

                    if (history != null)
                    {
                        // 履歴を更新
                        Program.Histories.Remove(history);

                        // 履歴を保存
                        Program.SaveHistories();

                        // インストール ディレクトリを削除
                        if (Directory.Exists(history.InstallDir))
                        {
                            Directory.Delete(history.InstallDir, true);
                        }
                    }

                    return;
                }

                // 起動の切り替え
                if (argsDic.ContainsKey("/CUI"))
                {
                    // 強制更新
                    if (argsDic.ContainsKey("/FORCE"))
                    {
                        Program.IsForce = true;

                        // 履歴ファイルを消去して実行
                        File.Delete(Program.OrgCurrentDirectory + Program.HistoryFileName);
                    }

                    // 起動アセンブリフラグ
                    if (argsDic.ContainsKey("/NB"))
                    {
                        Program.IsBoot = false;
                    }

                    // CUI起動

                    // コンソール アタッチ
                    Program.AttachConsole(-1);

                    //Program.OutPutMessage("CUI起動", LogLevel.InfoLog);
                    //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                    Program.OutPutMessage(ResourceMgr.GetString("M0006"), LogLevel.InfoLog);

                    if (argsDic.ContainsKey("/SILENT"))
                    {
                        // 進捗ダイアログ＆メッセージなし
                        Program.CheckUpdateAndInstall_Silent(valsLst, argsDic);
                    }
                    else
                    {
                        // 進捗ダイアログ＆メッセージあり
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new Form2(valsLst, argsDic));
                    }

                    // コンソール デタッチ
                    Program.FreeConsole();
                }
                else
                {
                    // GUI起動
                    //Program.OutPutMessage("GUI起動", LogLevel.InfoLog);
                    //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                    Program.OutPutMessage(ResourceMgr.GetString("M00007"), LogLevel.InfoLog);

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                }
            }
        }

        /// <summary>更新チェック＆インストール（サイレントモード）</summary>
        /// <param name="valsLst">値</param>
        /// <param name="argsDic">コマンド・値</param>
        public static void CheckUpdateAndInstall_Silent(List<string> valsLst, Dictionary<string, string> argsDic)
        {
            try
            {
                // エントリを生成
                Entry entry = new Entry();

                // コマンドラインから各値を取得する。
                entry.WWWURL = Program.NullToEmptyString(argsDic, "/WWWURL");
                entry.WWWUID = Program.NullToEmptyString(argsDic, "/WWWUID");
                entry.WWWPWD = Program.NullToEmptyString(argsDic, "/WWWPWD");
                entry.WWWDomain = Program.NullToEmptyString(argsDic, "/WWWDOMAIN");
                //---
                entry.ProxyURL = Program.NullToEmptyString(argsDic, "/PROXYURL");
                entry.ProxyUID = Program.NullToEmptyString(argsDic, "/PROXYUID");
                entry.ProxyPWD = Program.NullToEmptyString(argsDic, "/PROXYPWD");
                entry.ProxyDomain = Program.NullToEmptyString(argsDic, "/PROXYDOMAIN");

                // エントリを保存
                Program.SaveCurrent(entry);

                // 履歴をロード
                Program.LoadHistories();

                // 更新処理の実行
                Program.ExecUpdate(entry);

                // 履歴を保存
                Program.SaveHistories();
            }
            catch (MyException my_ex)
            {
                // 戻し
                Program.Recover();

                // メッセージ表示時
                string message = my_ex.Message;

                // ログ出力用のフィールドがあるか？ないか？
                if (string.IsNullOrEmpty(my_ex.ToLog))
                {
                    Program.OutPutMessage(message, LogLevel.InfoLog);
                }
                else
                {
                    Program.OutPutMessage(message + "\r\n" + my_ex.ToLog, LogLevel.InfoLog);
                }
            }
            catch (Exception ex)
            {
                // 戻し
                Program.Recover();

                // 例外発生時
                string message = "";

                //message += "＜メッセージ＞\r\n";
                //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                message += ResourceMgr.GetString("M0002")+"\r\n";
                message += ex.Message;
                message += "\r\n";

                message += "\r\n";
                //message += "＜スタック トレース＞\r\n";
                //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                message += ResourceMgr.GetString("M0003")+"\r\n";
                message += ex.StackTrace;
                message += "\r\n";

                if (ex.InnerException != null)
                {
                    message += "\r\n";
                    //message += "＜内部例外＞\r\n";
                    //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                    message += ResourceMgr.GetString("M0004")+"\r\n";
                    message += ex.InnerException.ToString();
                    message += "\r\n";
                }

                Program.OutPutMessage(message, LogLevel.ErrorLog);
            }
            finally
            {
                // テンポラリの削除
                File.Delete(Program.OrgCurrentDirectory + Program.TempMftFileName);

                // GCでZIPが解放される可能性
                //GC.Collect();
                Program.system_gc_collecting();
                try
                {
                    File.Delete(Program.OrgCurrentDirectory + Program.TempZipFileName);
                }
                catch (Exception ex)
                {
                    // 例外を潰してログに出力
                    //Program.OutPutMessage(Program.TempZipFileName + "削除例外：" + ex.ToString(), LogLevel.ErrorLog);
                    //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                    Program.OutPutMessage(Program.TempZipFileName + ResourceMgr.GetString("M0008") + ex.ToString(), LogLevel.ErrorLog);
                }
            }
        }

        /// <summary>nullは空文字列に変換する</summary>
        /// <param name="dic">ディクショナリ</param>
        /// <param name="key">キー</param>
        /// <returns>出力文字列（nullを含まない）</returns>
        public static string NullToEmptyString(Dictionary<string, string> dic, string key)
        {
            if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            else
            {
                return "";
            }
        }

        #region 更新処理

        /// <summary>非同期フレームワーク</summary>
        /// <remarks>進捗・メッセージ表示などで使用する</remarks>
        public static MyBaseAsyncFunc Af { set; get; }

        /// <summary>更新処理</summary>
        /// <param name="o">エントリ, Form1</param>
        /// <returns>空のオブジェクト</returns>
        /// <remarks>非同期処理から実行する</remarks>
        public static object ExecUpdate(object o)
        {
            // フラグをクリア
            Program.HaveBackup = false;
            Program.InstallDirectory = "";
            Program.BackupDirectory = "";

            // ワーク
            Process p = null;

            // エントリを初期化
            Entry entry = (Entry)o;

            // マニュフェスト ファイル上に
            // 存在するZIPファイルのリスト
            List<string> existZipList = new List<string>();

            #region 履歴エントリを取得

            // 履歴エントリを取得
            Entry history = null;
            foreach (Entry temp in Program.Histories)
            {
                if (entry.WWWURL == temp.WWWURL)
                {
                    history = temp;
                    break;
                }
            }

            #endregion

            #region 主処理

            // HEAD（Last-Modifiedチェック）
            bool lmcRet = false;

            try
            {
                lmcRet = Program.LastModifiedCheck_ByHead(entry, history, "");
            }
            catch (WebException wex)
            {
                // WebExceptionで、
                if (history != null)
                {
                    // 履歴あり

                    // 更新されていない 
                    lmcRet = false;

                    // ログ出力して起動（正常終了する）
                    Program.OutPutMessage(string.Format(
                        GetMessage.GetMessageDescription("E0009"), entry.WWWURL)
                        + "\r\n" + wex.ToString(), LogLevel.InfoLog);
                }
                else
                {
                    // 履歴なし

                    //// リスロー
                    //throw;

                    // 例外（MyException）
                    throw new MyException(string.Format(
                        GetMessage.GetMessageDescription("E0009"),
                        entry.WWWURL), wex.ToString());
                }

                #region 消し

                // SocketExceptionだけ
                // ハンドルしようと考えていたが・・・

                //if (wex.InnerException != null)
                //{
                //    // InnerException：!= null

                //    if (wex.InnerException is SocketException)
                //    {
                //        // InnerException：ソケット例外
                //        if (history != null)
                //        {
                //            // 履歴あり
                //            // 更新されていない 
                //            lmcRet = false;
                //        }
                //        else
                //        {
                //            // 履歴なし
                //            // リスロー
                //            throw;
                //        }
                //    }
                //    else
                //    {
                //        // InnerException：その他
                //        // リスロー
                //        throw;
                //    }
                //}
                //else
                //{
                //    // InnerException：== null
                //    // リスロー
                //    throw;
                //}

                #endregion
            }

            if (!lmcRet)
            {
                // 更新されていない
                // 履歴エントリはそのまま

                #region 起動のチェック

                // → 起動exeのキック（historyから）
                if (Program.IsBoot)
                {
                    // /NB スイッチが指定されていない。
                    if (Program.ExistProcess(history.InstallDir, history.StartExe, out p))
                    {
                        // プロセスが起動されていない。

                        if (File.Exists(p.StartInfo.FileName))
                        {
                            // ファイルが存在する。

                            // 起動する。
                            p.Start();

                            // 最前面へ
                            Program.SetWindowPos(
                                p.MainWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
                        }
                        else
                        {
                            // ファイルが存在しない。
                            throw new Exception(string.Format(
                                GetMessage.GetMessageDescription("E0001"), p.StartInfo.FileName));
                        }
                    }
                    else
                    {
                        // プロセスが起動されている。
                        throw new MyException(string.Format(
                            GetMessage.GetMessageDescription("E0002"), p.StartInfo.FileName));
                    }
                }
                else
                {
                    // /NB スイッチが指定されている。
                    Program.OutPutMessage(GetMessage.GetMessageDescription("I0012"), LogLevel.InfoLog);
                }

                #endregion
            }
            else
            {
                // 更新されている

                // 画面表示を更新
                if (Program.Af != null)
                {
                    // 開始（進捗20％）
                    Program.Af.ExecChangeProgress(new ChangeProgressParameter(
                        string.Format(GetMessage.GetMessageDescription("I0001"), entry.WWWURL), 20));
                }

                // マニュフェスト ファイルを取得
                // GET、Content保存
                Program.GetAndSaveContent(entry, "");

                // マニュフェスト ファイルを読む
                FileStream fs = null;
                StreamReader sr = null;
                StringReader manifesto = null;

                try
                {
                    fs = new FileStream(
                        Program.OrgCurrentDirectory
                        + Program.TempMftFileName, FileMode.Open);

                    sr = new StreamReader(fs);
                    manifesto = new StringReader(sr.ReadToEnd());
                }
                finally
                {
                    if (sr != null) { sr.Close(); }
                    if (fs != null) { fs.Close(); }
                }

                try
                {
                    #region マニュフェスト ファイルの処理

                    // ZIPファイルを
                    // ・更新チェック
                    // ・ダウンロード
                    // ・インストール
                    string line = "";

                    // インストール ディレクトリ
                    line = manifesto.ReadLine().Trim();
                    if (string.IsNullOrEmpty(line) || line.Length <= 4)
                    {
                        // マニュフェスト ファイルの不正
                        throw new Exception(string.Format(
                            GetMessage.GetMessageDescription("E0003"), line));
                    }
                    else
                    {
                        if (line.Substring(0, 3) == "ins")
                        {
                            // エントリに追加
                            entry.InstallDir = StringVariableOperator.
                                BuiltStringIntoEnvironmentVariable(line.Substring(4));
                        }
                        else
                        {
                            // マニュフェスト ファイルの不正
                            throw new Exception(string.Format(
                                GetMessage.GetMessageDescription("E0003"), line));
                        }
                    }

                    // インストール ディレクトリの変更には対応しない。
                    if (history != null)
                    {
                        if (history.InstallDir != entry.InstallDir)
                        {
                            //throw new Exception(string.Format(
                            //    GetMessage.GetMessageDescription("E0003"), string.Format(
                            //        "インストール ディレクトリが変更されています{0}。", line)));
                                            
                            //For internationalization, Replaced all the Japanese language to  GetMessage.GetMessageDescription() method call
                            throw new Exception(string.Format(
                                GetMessage.GetMessageDescription("E0003"), string.Format(
                                    GetMessage.GetMessageDescription("M0006"), line)));
                        }
                    }

                    // 起動exe
                    line = manifesto.ReadLine().Trim();
                    if (string.IsNullOrEmpty(line) || line.Length <= 4)
                    {
                        // マニュフェスト ファイルの不正
                        throw new Exception(string.Format(
                            GetMessage.GetMessageDescription("E0003"), line));
                    }
                    else
                    {
                        if (line.Substring(0, 3) == "exe")
                        {
                            // エントリに追加
                            entry.StartExe = line.Substring(4);
                        }
                        else
                        {
                            // マニュフェスト ファイルの不正
                            throw new Exception(string.Format(
                                GetMessage.GetMessageDescription("E0003"), line));
                        }
                    }

                    #endregion

                    #region 起動のチェック

                    if (Program.ExistProcess(entry.InstallDir, entry.StartExe, out p))
                    {
                        // プロセスが起動されていない。→ 処理続行
                    }
                    else
                    {
                        // プロセスが起動されている。→ 処理中止
                        throw new MyException(string.Format(
                            GetMessage.GetMessageDescription("E0002"), p.StartInfo.FileName));
                    }

                    #endregion

                    // 読み取り専用属性の全削除
                    Program.RemoveReadonlyAttribute(
                        new DirectoryInfo(entry.InstallDir));

                    // 強制モードでは一度削除してから
                    if (Program.IsForce)
                    {
                        // 存在チェックを追加
                        if (Directory.Exists(entry.InstallDir))
                        {
                            Directory.Delete(entry.InstallDir, true);
                        }
                    }

                    #region ZIPファイルのDL & Ins

                    // ZIPファイル
                    line = manifesto.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        line.Trim();
                    }

                    string zipFile = "";
                    bool isUpdated = false;

                    while (true)
                    {
                        if (string.IsNullOrEmpty(line))
                        {
                            // 終了
                            break;
                        }
                        else if (line.Length <= 4)
                        {
                            // マニュフェスト ファイルの不正
                            throw new Exception(string.Format(
                                GetMessage.GetMessageDescription("E0003"), line));
                        }
                        else
                        {
                            if (line.Substring(0, 3) == "zip")
                            {
                                zipFile = line.Substring(4);

                                if (zipFile.Length > 4 &&
                                    zipFile.Substring(zipFile.Length - 4, 4) == ".zip")
                                {
                                    // エントリに追加
                                    entry.ZipFiles.Add(zipFile);

                                    // マニュフェスト ファイル上に
                                    // 存在するZIPファイルを保存
                                    existZipList.Add(zipFile);

                                    // 画面表示を更新
                                    if (Program.Af != null)
                                    {
                                        // ZIPを確認（進捗＋8％）
                                        Program.Af.ExecChangeProgress(new ChangeProgressParameter(
                                            string.Format(GetMessage.GetMessageDescription("I0001"), zipFile), 8));
                                    }

                                    // HEAD（Last-Modifiedチェック）
                                    if (Program.LastModifiedCheck_ByHead(entry, history, zipFile))
                                    {
                                        // GET、Content保存
                                        Program.GetAndSaveContent(entry, zipFile);

                                        // ステータス変更
                                        isUpdated = true;
                                    }
                                    else
                                    {
                                        // ステータス変更
                                        isUpdated = false;
                                    }
                                }
                                else
                                {
                                    // 例外（.ZIPじゃない）
                                    throw new Exception(GetMessage.GetMessageDescription("E0004") + zipFile);
                                }
                            }
                            else if (line.Substring(0, 3) == "md5")
                            {
                                // HEAD（Last-Modifiedチェック）
                                if (isUpdated)
                                {
                                    string md5HashMft = line.Substring(4);
                                    string md5HashCur = Program.GetMD5Hash(
                                        Program.OrgCurrentDirectory + Program.TempZipFileName);

                                    if (md5HashMft == md5HashCur)
                                    {
                                        // 指定の位置にインストール
                                        // （必要な履歴の情報を引き継ぐ）
                                        Program.InstallZip(entry, zipFile, history, "utf-8");
                                    }
                                    else
                                    {
                                        // 例外（MD5ハッシュの不一致）
                                        throw new MyException(string.Format(
                                            GetMessage.GetMessageDescription("E0005"),
                                            zipFile, md5HashMft, md5HashCur));
                                    }
                                }
                                else
                                {
                                    // 更新されていない
                                    //（必要な履歴の情報を引き継ぐ）
                                    entry.HttpZipLastModified[zipFile] = history.HttpZipLastModified[zipFile];
                                    entry.HttpZipContents[zipFile] = history.HttpZipContents[zipFile];
                                    // メッセージ（HeadAndLastModifiedで済み）
                                }
                            }
                            else
                            {
                                //// 終了
                                //break;

                                // マニュフェスト ファイルの不正
                                throw new Exception(string.Format(
                                    GetMessage.GetMessageDescription("E0003"), line));
                            }
                        }

                        // 次へ
                        line = manifesto.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            line.Trim();
                        }
                    }
                }
                finally
                {
                    // クローズ（エラーになるので）
                    manifesto.Close();
                }

                    #endregion

                // 画面表示を更新
                if (Program.Af != null)
                {
                    // 終了（進捗－％）→最後に強制的に100％になる。
                    Program.Af.ExecChangeProgress(
                        new ChangeProgressParameter(
                        GetMessage.GetMessageDescription("I0002"), 1));
                }

                #region 履歴エントリを保存

                // 履歴を置き換える。
                for (int i = 0; i < Program.Histories.Count; i++)
                {
                    if (Program.Histories[i].WWWURL == entry.WWWURL)
                    {
                        Program.Histories[i] = entry;
                    }
                }

                if (history == null)
                {
                    // 新規のエントリの場合だけ、
                    // エントリを履歴に追加する。
                    Program.Histories.Add(entry);

                    // メッセージ
                    Program.OutPutMessage(string.Format(
                        GetMessage.GetMessageDescription("I0003"), entry.WWWURL), LogLevel.InfoLog);
                }
                else
                {
                    // 旧エントリの場合だけ、
                    // 無くなったZIPファイルを削除する。
                    foreach (string zip1 in history.ZipFiles)
                    {
                        // 使ってないフラグ
                        bool isUnused = true;

                        // 使ってないチェック
                        foreach (string zip2 in existZipList)
                        {
                            if (zip1 == zip2)
                            {
                                // ZIPファイルを使いました。
                                isUnused = false;
                            }
                        }

                        // このZIPファイルを使っていない。
                        if (isUnused)
                        {
                            // ZIPファイル内のコンテンツを削除
                            Program.DeleteZipContents(zip1, history);

                            // ZIPファイルの情報を消す。
                            entry.ZipFiles.Remove(zip1);
                            entry.HttpZipContents.Remove(zip1);
                            entry.HttpZipLastModified.Remove(zip1);

                            // メッセージ
                            Program.OutPutMessage(string.Format(
                                GetMessage.GetMessageDescription("I0004"), zip1), LogLevel.InfoLog);
                        }
                    }

                    // メッセージ
                    Program.OutPutMessage(string.Format(
                        GetMessage.GetMessageDescription("I0005"), entry.WWWURL), LogLevel.InfoLog);
                }

                #endregion

                #region 起動のチェック

                // → 起動exeのキック（entryから）
                if (Program.IsBoot)
                {
                    // /NB スイッチが指定されていない。
                    if (Program.ExistProcess(entry.InstallDir, entry.StartExe, out p))
                    {
                        // プロセスが起動されていない。

                        if (File.Exists(p.StartInfo.FileName))
                        {
                            // ファイルが存在する。

                            // バックアップ・ディレクトリを削除
                            Program.DeleteBackupDirectory();

                            // 起動する。
                            p.Start();

                            // 最前面へ
                            Program.SetWindowPos(
                                p.MainWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
                        }
                        else
                        {
                            // ファイルが存在しない。
                            throw new Exception(string.Format(
                                GetMessage.GetMessageDescription("E0001"), p.StartInfo.FileName));
                        }
                    }
                    else
                    {
                        // プロセスが起動されている。
                        throw new MyException(string.Format(
                                GetMessage.GetMessageDescription("E0002"), p.StartInfo.FileName));
                    }
                }
                else
                {
                    // バックアップ・ディレクトリを削除
                    Program.DeleteBackupDirectory();

                    // /NB スイッチが指定されている。
                    Program.OutPutMessage(GetMessage.GetMessageDescription("I0012"), LogLevel.InfoLog);
                }

                #endregion
            }

            #endregion

            return new object();
        }

        /// <summary>バックアップ・ディレクトリを削除</summary>
        private static void DeleteBackupDirectory()
        {
            if (Program.HaveBackup)
            {
                // インストール済み。
                Directory.Delete(Program.BackupDirectory, true);
            }
        }

        /// <summary>プロセススタートのためのオブジェクトを返す</summary>
        /// <param name="installDir">インストール ディレクトリ</param>
        /// <param name="exeFiles">起動EXE名のカンマ区切りリスト</param>
        /// <param name="p">プロセス</param>
        /// <returns>
        /// 真：当該EXEは起動されていない。
        /// 偽：当該EXEは起動されている。
        /// </returns>
        private static bool ExistProcess(string installDir, string exeFiles, out Process p)
        {
            // 分割する。
            string[] ary_exeFiles = exeFiles.Split(',');

            // パスに変換
            for (int i = 0; i < ary_exeFiles.Length; i++)
            {
                // プレーンな連結のみサポート
                ary_exeFiles[i] = installDir + ary_exeFiles[i].Trim();
            }

            // プロセスを生成
            p = new Process();

            // 二重起動のチェック
            foreach (Process pp in System.Diagnostics.Process.GetProcesses())
            {
                try
                {
                    for (int i = 0; i < ary_exeFiles.Length; i++)
                    {
                        // ToUpper！
                        if (ary_exeFiles[i].ToUpper() == pp.MainModule.FileName.ToUpper())
                        {
                            // 起動されている。
                            p.StartInfo = new ProcessStartInfo(ary_exeFiles[i]);
                            return false;
                        }
                    }
                }
                catch
                {
                    // アクセス違反をつぶす。
                }
            }

            // 起動されていない。
            p.StartInfo = new ProcessStartInfo(ary_exeFiles[0]);
            return true;
        }

        #endregion

        #region 共通関数

        #region ファイルIO

        #region セーブ

        /// <summary>ファイルにセーブ</summary>
        /// <param name="obj">オブジェクト</param>
        /// <param name="filePath">ファイル パス</param>
        public static void SaveFile(object obj, string filePath)
        {
            FileStream fs = null;

            try
            {
                // バイナリ シリアライズしてファイル出力
                byte[] temp = BinarySerialize.ObjectToBytes(obj);
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                // バイト型配列の内容をすべて書き込む
                fs.Write(temp, 0, temp.Length);
            }
            finally
            {
                // 閉じる
                if (fs != null) { fs.Close(); }
            }
        }

        #endregion

        #region ロード

        /// <summary>ファイルをロード</summary>
        /// <param name="filePath">ファイル パス</param>
        /// <returns>バイト配列</returns>
        public static byte[] LoadFile(string filePath)
        {
            byte[] ret = null;
            FileStream fs = null;
            MemoryStream ms = null;

            try
            {
                // ファイル入力をバイナリ デシリアライズ
                if (File.Exists(filePath))
                {
                    fs = new FileStream(filePath,
                        FileMode.Open, FileAccess.Read);

                    ms = new MemoryStream();
                    byte[] buffer = new byte[0x1000]; // 4KB

                    for (; ; )
                    {
                        // ファイルの一部を読み込む
                        int readSize = fs.Read(buffer, 0, buffer.Length);

                        // ファイルをすべて読み込んだときは終了する
                        if (readSize == 0) { break; }

                        // マージ
                        ms.Write(buffer, 0, buffer.Length);
                    }

                    // オブジェクト化して設定
                    ret = ms.ToArray();
                }
            }
            finally
            {
                // 閉じる
                if (ms != null) { ms.Close(); }
                if (fs != null) { fs.Close(); }
            }

            // 戻す。
            return ret;
        }

        #endregion

        #region Current

        /// <summary>current.binのLoad処理</summary>
        /// <returns>Entry（前回入力）</returns>
        public static Entry LoadCurrent()
        {
            byte[] current = null;
            current = Program.LoadFile(
                Program.OrgCurrentDirectory + Program.CurrentFileName);

            if (current == null)
            {
                return null;
            }
            else
            {
                return (Entry)BinarySerialize.BytesToObject(current);
            }
        }

        /// <summary>current.binのSave処理</summary>
        /// <param name="entry">Entry（現状入力）</param>
        public static void SaveCurrent(Entry entry)
        {
            Program.SaveFile(entry,
                Program.OrgCurrentDirectory + Program.CurrentFileName);
        }

        #endregion

        #region Histories

        /// <summary>histories.binのLoad処理</summary>
        /// <returns>List＜Entry＞（履歴）</returns>
        public static List<Entry> LoadHistories()
        {
            byte[] current = null;
            current = Program.LoadFile(Program.OrgCurrentDirectory + Program.HistoryFileName);

            if (current == null)
            {
                //return null;
                Program.Histories = new List<Entry>();
            }
            else
            {
                //return (List<Entry>)BinarySerialize.BytesToObject(current);
                Program.Histories = (List<Entry>)BinarySerialize.BytesToObject(current);
            }

            // 設定しつつ戻す
            return Program.Histories;
        }

        /// <summary>histories.binのSave処理</summary>
        public static void SaveHistories()
        {
            Program.SaveFile(Program.Histories, Program.OrgCurrentDirectory + Program.HistoryFileName);
        }

        #endregion

        #region 読み取り専用属性の削除

        /// <summary>削除処理の追加</summary>
        /// <param name="di">DirectoryInfo</param>
        /// <remarks>
        /// DOBON.NET > ファイル、フォルダ
        /// 読み取り専用ファイルがあるときでもフォルダを削除する 
        /// http://dobon.net/vb/dotnet/file/deletedirectory.html
        /// </remarks>
        public static void RemoveReadonlyAttribute(DirectoryInfo di)
        {
            if (di.Exists)
            {
                // 存在する場合

                // 当該フォルダ（読み取り専用属性の削除）
                if ((di.Attributes & FileAttributes.ReadOnly)
                    == FileAttributes.ReadOnly)
                {
                    di.Attributes = FileAttributes.Normal;
                }

                // フォルダ内のすべてのファイル
                // （読み取り専用属性の削除）
                foreach (FileInfo fi in di.GetFiles())
                {
                    if ((fi.Attributes & FileAttributes.ReadOnly)
                        == FileAttributes.ReadOnly)
                    {
                        fi.Attributes = FileAttributes.Normal;
                    }
                }

                // 回帰
                foreach (DirectoryInfo child_di in di.GetDirectories())
                {
                    Program.RemoveReadonlyAttribute(child_di);
                }
            }
        }

        #endregion

        #region ZIPファイル内のコンテンツの削除

        /// <summary>ZIPファイル内のコンテンツの削除</summary>
        /// <param name="zipFile">ZIPファイル名</param>
        /// <param name="history">エントリ（履歴）</param>
        private static void DeleteZipContents(string zipFile, Entry history)
        {
            if (history != null)
            {
                // 履歴情報があれば、過去の解凍コンテンツを削除する。
                if (history.HttpZipContents.ContainsKey(zipFile))
                {
                    // ファイルパスを取得してコンテンツを削除
                    foreach (string contentFile in history.HttpZipContents[zipFile])
                    {
                        // 存在チェック
                        if (File.Exists(contentFile))
                        {
                            // 存在しなくても例外を返さない。
                            // ・・・と言いつつパスが
                            //       存在しないと例外となる。
                            // ※ オープン状態では例外になる。
                            try
                            {
                                File.Delete(contentFile);
                            }
                            catch
                            {
                                // 例外（オープン状態）
                                throw new MyException(string.Format(
                                    GetMessage.GetMessageDescription("E0006"), contentFile));
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region 戻し操作

        /// <summary>戻し操作</summary>
        public static void Recover()
        {
            // バックアップが存在すれば、戻し操作を実行。
            if (Program.HaveBackup)
            {
                // 戻しを実行
                Program.OutPutMessage(
                        GetMessage.GetMessageDescription("E0008"), LogLevel.InfoLog);

                // 消す
                Program.DeleteAllFile(
                    Program.InstallDirectory);

                // 戻す
                Program.CreateMirror(
                    Program.BackupDirectory,
                    Program.InstallDirectory);

                // 退避ディレクトリを消去
                Program.DeleteBackupDirectory();
            }
        }

        /// <summary>
        /// すべてのファイルを消去する。
        /// ディレクトリは残して、例外はつぶす。
        /// </summary>
        /// <param name="path">パス</param>
        private static void DeleteAllFile(string path)
        {
            // パスの存在チェック
            if (Directory.Exists(path))
            {
                // ファイルを全消しして
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // 例外は潰す
                    }
                }

                // ディレクトリを再起処理
                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                {
                    Program.DeleteAllFile(directory);
                }
            }
        }

        /// <summary>ミラーを作成する。</summary>
        /// <param name="src">退避したディレクトリ</param>
        /// <param name="dst">インストール ディレクトリ</param>
        private static void CreateMirror(string src, string dst)
        {
            // パスの存在チェック
            if (Directory.Exists(src))
            {
                // ファイルをコピー
                string[] files = Directory.GetFiles(src);

                // コピー先のディレクトリがなければ作成
                if (!Directory.Exists(dst))
                {
                    Directory.CreateDirectory(dst);
                }

                // コピー
                foreach (string file in files)
                {
                    try
                    {
                        string[] temp = file.Split('\\');
                        string fileName = temp[temp.Length - 1];

                        string dstFile = "";
                        if (dst[dst.Length - 1] == '\\')
                        {
                            dstFile = dst + fileName;
                        }
                        else
                        {
                            dstFile = dst + "\\" + fileName;
                        }

                        File.Copy(file, dstFile);
                    }
                    catch
                    {
                        // 例外は潰す
                    }
                }

                // ディレクトリを再起処理
                string[] directories = Directory.GetDirectories(src);
                foreach (string directory in directories)
                {
                    Program.CreateMirror(directory, directory.Replace(src, dst));
                }
            }
        }

        #endregion

        #endregion

        #region MD5ハッシュ

        /// <summary>MD5ハッシュ値のBase64文字列を取得する。</summary>
        /// <param name="FilePath">ファイル パス</param>
        /// <returns>MD5ハッシュ値のBase64文字列</returns>
        public static string GetMD5Hash(string FilePath)
        {
            // 暗号化サービスプロバイダ
            // MD5CryptoServiceProviderサービスプロバイダ
            HashAlgorithm ha = new MD5CryptoServiceProvider();

            // ハッシュ値を計算する
            return CustomEncode.ToBase64String(
                ha.ComputeHash(Program.LoadFile(FilePath)));
        }

        #endregion

        #region HTTP系メソッド

        #region HEAD

        /// <summary>HEAD（Last-Modifiedチェック）</summary>
        /// <param name="entry">エントリ</param>
        /// <param name="history">エントリ（履歴）</param>
        /// <param name="zipFile">ZIPファイル</param>
        /// <returns>
        /// true：更新あり、DLへ（インスコ）
        /// false：更新なし、次へ（パス）
        /// </returns>
        public static bool LastModifiedCheck_ByHead(Entry entry, Entry history, string zipFile)
        {
            HttpWebRequest hwReq = null;
            HttpWebResponse hwRes = null;

            try
            {
                hwReq = Program.GetHttpWebRequest(entry, zipFile);
                hwReq.Timeout = 5000;
                hwReq.Method = "HEAD";
                hwRes = (HttpWebResponse)hwReq.GetResponse();

                // 更新日付のチェック
                string httpLastModifiedHis = "";
                string httpLastModifiedWeb = hwRes.LastModified.ToString(
                    "yyyy-MM-dd HH:mm:ss:fff"); //.Headers["Last-Modified"];

                if (httpLastModifiedWeb == null && httpLastModifiedWeb == "")
                {
                    // 更新日付ヘッダなし
                    Program.OutPutMessage(string.Format(
                        GetMessage.GetMessageDescription("I0006"), hwReq.RequestUri.AbsoluteUri), LogLevel.InfoLog);
                }
                else
                {
                    // 更新日付の更新
                    if (string.IsNullOrEmpty(zipFile))
                    {
                        // マニュフェスト ファイルの場合
                        entry.HttpLastModified = httpLastModifiedWeb;
                    }
                    else
                    {
                        // ZIPファイルの場合
                        entry.HttpZipLastModified[zipFile] = httpLastModifiedWeb;
                    }

                    // 履歴エントリの更新日付の履歴をチェックする。
                    if (history != null)
                    {
                        // 履歴あり

                        if (string.IsNullOrEmpty(zipFile))
                        {
                            // マニュフェスト ファイルの場合
                            httpLastModifiedHis = history.HttpLastModified;
                        }
                        else
                        {
                            // ZIPファイルの場合
                            // 無い場合があるので
                            if (history.HttpZipLastModified.ContainsKey(zipFile))
                            {
                                httpLastModifiedHis = history.HttpZipLastModified[zipFile];
                            }
                            else
                            {
                                // NullOrEmptyにしない
                                //（↓の更新日付ヘッダの不一致へ）
                                httpLastModifiedHis = "xxx";
                            }
                        }

                        // 更新日付の履歴あり
                        if (!string.IsNullOrEmpty(httpLastModifiedHis))
                        {
                            if (httpLastModifiedHis == httpLastModifiedWeb)
                            {
                                // 更新日付ヘッダの一致

                                // メッセージ
                                Program.OutPutMessage(string.Format(
                                    GetMessage.GetMessageDescription("I0007"), hwReq.RequestUri.AbsoluteUri), LogLevel.InfoLog);

                                // → パス
                                return false;
                            }
                            else
                            {
                                // 更新日付ヘッダの不一致（大小の評価ではない！）

                                // メッセージ
                                Program.OutPutMessage(string.Format(
                                    GetMessage.GetMessageDescription("I0008"), hwReq.RequestUri.AbsoluteUri), LogLevel.InfoLog);
                            }
                        }
                    }
                    else
                    {
                        // 履歴なし

                        // メッセージ
                        Program.OutPutMessage(string.Format(
                            GetMessage.GetMessageDescription("I0009"), hwReq.RequestUri.AbsoluteUri), LogLevel.InfoLog);
                    }
                }

                // → インスコ
                return true;
            }
            finally
            {
                // 閉じる（これが無いと２回目実行できない。）
                if (hwRes != null) { hwRes.Close(); }
                if (hwReq != null) { hwReq.Abort(); }
            }
        }

        #endregion

        #region GET

        /// <summary>ファイルを取得</summary>
        /// <param name="entry">エントリ</param>
        /// <param name="zipFile">ZIPファイル</param>
        public static void GetAndSaveContent(Entry entry, string zipFile)
        {
            HttpWebRequest hwReq = null;
            HttpWebResponse hwRes = null;

            try
            {
                hwReq = Program.GetHttpWebRequest(entry, zipFile);
                hwReq.Timeout = 5000;
                hwReq.Method = "GET";
                hwRes = (HttpWebResponse)hwReq.GetResponse();

                if (hwRes.ContentLength != -1)
                {
                    Stream sm = null;
                    FileStream fs = null;

                    try
                    {
                        // 応答データを受信するためのStreamを取得
                        sm = hwRes.GetResponseStream();

                        // ファイルに書き込むためのFileStreamを作成
                        string saveFileName = "";

                        if (string.IsNullOrEmpty(zipFile))
                        {
                            // マニュフェスト
                            saveFileName = Program.TempMftFileName;
                        }
                        else
                        {
                            // ZIPファイル
                            saveFileName = Program.TempZipFileName;
                        }

                        fs = new FileStream(
                            Program.OrgCurrentDirectory + saveFileName, FileMode.Create, FileAccess.Write);

                        // 応答データをファイルに書き込む
                        int b;
                        while ((b = sm.ReadByte()) != -1)
                        {
                            fs.WriteByte(Convert.ToByte(b));
                        }

                        // メッセージ
                        Program.OutPutMessage(string.Format(
                            GetMessage.GetMessageDescription("I0010"), hwReq.RequestUri.AbsoluteUri), LogLevel.InfoLog);
                    }
                    finally
                    {
                        // 閉じる
                        if (fs != null) { fs.Close(); }
                        if (sm != null) { sm.Close(); }
                    }
                }
            }
            finally
            {
                // 閉じる（これが無いと２回目実行できない。）
                if (hwRes != null) { hwRes.Close(); }
                if (hwReq != null) { hwReq.Abort(); }
            }
        }

        #endregion

        #region new HttpWebRequest

        /// <summary>HttpWebRequestを取得する。</summary>
        /// <param name="entry">エントリ</param>
        /// <param name="zipFile">ZIPファイル名</param>
        /// <returns>HttpWebRequest</returns>
        public static HttpWebRequest GetHttpWebRequest(Entry entry, string zipFile)
        {
            string zipURL = entry.WWWURL;
            HttpWebRequest hwReq = null;

            if (!string.IsNullOrEmpty(zipFile))
            {
                // ファイル指定がされている場合。
                // マニュフェスト ファイルを・・・
                string[] temp = zipURL.Split('/');
                for (int i = 0; i < temp.Length - 1; i++)
                {
                    if (i == 0)
                    {
                        zipURL = temp[i];
                    }
                    else
                    {
                        zipURL += "/" + temp[i];
                    }
                }

                // ZIPファイルに置き換える。
                zipURL += "/" + zipFile;
            }

            // リクエストを生成する。
            hwReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(zipURL));

            // WWWサーバのNetworkCredential
            if (entry.WWWUID == null && entry.WWWUID == "")
            {
                // NetworkCredentialなし（デフォルト）
                //hwReq.UseDefaultCredentials = true;// ★要・検討
            }
            else
            {
                // NetworkCredentialあり
                if (entry.WWWDomain == null || entry.WWWDomain == "")
                {
                    // UID、PWD
                    hwReq.Credentials = new NetworkCredential(
                        entry.WWWUID, entry.WWWPWD);
                }
                else
                {
                    // UID、PWD、Domain
                    hwReq.Credentials = new NetworkCredential(
                        entry.WWWUID, entry.WWWPWD, entry.WWWDomain);
                }
            }

            // プロキシ使うか？
            WebProxy proxy = null;

            // Proxyサーバ
            if (entry.ProxyURL.ToLower() == "none")
            {
                // Proxyサーバなし（null）
                hwReq.Proxy = null;

                // ※ GlobalProxySelection.GetEmptyWebProxy()は古い形式
            }
            else if (entry.ProxyURL == null || entry.ProxyURL == "")
            {
                // proxy = (WebProxy)WebProxy.GetDefaultProxy();// 古い形式
                // 何もしない → DefaultWebProxyプロパティの値を使用する。
            }
            else
            {
                // Proxyサーバあり
                proxy = new WebProxy(new Uri(entry.ProxyURL));
            }

            // Proxyサーバがあれば
            if (proxy != null)
            {
                // NetworkCredentialがあれば
                if (entry.ProxyUID == null || entry.ProxyUID == "")
                {
                    // NetworkCredentialなし（デフォルト）
                    proxy.UseDefaultCredentials = true;// ★要・検討
                }
                else
                {
                    // NetworkCredentialあり
                    if (entry.ProxyDomain == null || entry.ProxyDomain == "")
                    {
                        // UID、PWD
                        proxy.Credentials = new NetworkCredential(
                            entry.ProxyUID, entry.ProxyPWD);
                    }
                    else
                    {
                        // UID、PWD、Domain
                        proxy.Credentials = new NetworkCredential(
                            entry.ProxyUID, entry.ProxyPWD, entry.ProxyDomain);
                    }
                }

                // プロキシ設定
                hwReq.Proxy = proxy;
            }

            return hwReq;
        }

        #endregion

        #endregion

        #region インストール

        /// <summary>ZIPファイルをインストール</summary>
        /// <param name="entry">エントリ</param>
        /// <param name="zipFile">ZIPファイル名</param>
        /// <param name="history">エントリ（履歴）</param>
        /// <param name="encStr">エンコーディング</param>
        public static void InstallZip(Entry entry, string zipFile, Entry history, string encStr)
        {
            if (Program.HaveBackup)
            {
                // バックアップ済み。
            }
            else
            {
                if (Directory.Exists(entry.InstallDir))
                {
                    // インストール先を指定
                    Program.InstallDirectory = entry.InstallDir;

                    // バックアップ先を指定
                    string guid = Guid.NewGuid().ToString();
                    if (entry.InstallDir[entry.InstallDir.Length - 1] == '\\')
                    {
                        Program.BackupDirectory =
                            entry.InstallDir.Substring(
                                0, entry.InstallDir.Length - 1) + "_" + guid;
                    }
                    else
                    {
                        Program.BackupDirectory = entry.InstallDir + "_" + guid;
                    }

                    // バックアップする。
                    Program.HaveBackup = true;
                    FileSystem.CopyDirectory(entry.InstallDir, BackupDirectory, UIOption.OnlyErrorDialogs, UICancelOption.DoNothing);
                }
            }

            // ZIPファイル内のコンテンツを削除
            Program.DeleteZipContents(zipFile, history);

            // 環境変数に対応
            string InsDir = StringVariableOperator.
                BuiltStringIntoEnvironmentVariable(entry.InstallDir);

            // 解凍

            // 解凍部品
            UnZipper uz = new UnZipper();

            // 選択基準なしで
            string[] exts = null;
            Zipper.SelectionDelegate scd = null;

            //if (this.txtExt.Enabled)
            //{
            //    exts = this.txtExt.Text.Split(',');
            //    scd = SelectionCriteriaDlgt2;
            //}

            // 解凍時、上書き制御
            uz.ExtractProgress = Program.MyExtractProgressEventHandler;

            // 解凍（１）デリゲートでフィルタ
            uz.ExtractFileFromZip(
                Program.OrgCurrentDirectory + Program.TempZipFileName,
                StringVariableOperator.BuiltStringIntoEnvironmentVariable(entry.InstallDir),
                scd, exts, ExtractExistingFileAction.OverwriteSilently,
                Encoding.GetEncoding(encStr), "");

            // メッセージ
            Program.OutPutMessage(string.Format(
                GetMessage.GetMessageDescription("I0011"), zipFile), LogLevel.InfoLog);

            // 解凍先のファイルのパスを抽出
            string header = "extract file ";
            StringReader srZipContents = new StringReader(uz.StatusMSG);
            List<string> zipContents = new List<string>();

            string tempContentFile = srZipContents.ReadLine();
            while (tempContentFile != null && tempContentFile.Trim() != "")
            {
                if (tempContentFile.IndexOf(header) == 0)
                {
                    // 解凍先のファイルのパスを抽出（[- 3] は[...]の分）
                    zipContents.Add(tempContentFile.Substring(header.Length, tempContentFile.Length - header.Length - 3));
                }

                // 次へ
                tempContentFile = srZipContents.ReadLine();
            }

            // 解凍先のファイルのパスを保存する（次回の削除処理に使用）。
            entry.HttpZipContents[zipFile] = zipContents.ToArray();
        }

        #endregion

        #region デリゲート

        #region 選択基準デリゲート

        /// <summary>選択基準を実装するデリゲート</summary>
        /// <param name="o">ファイル・エントリ情報</param>
        /// <param name="info">選択規準情報</param>
        /// <returns>
        /// 真：ファイルを書庫に追加する。
        /// 偽：ファイルを書庫に追加しない。
        /// </returns>
        public static bool SelectionCriteriaDlgt1(object o, object info)
        {
            FileInfo f = (FileInfo)o;

            // 選択規準情報（文字列配列）の
            // 指定が無い場合は、全て[true]

            if (info == null)
            {
                return true;
            }

            // 選択規準情報（文字列配列）の
            // 指定がある場合は、[true], [false]か判別

            string[] exts = (string[])info;
            foreach (string ext in exts)
            {
                if (f.Extension == "." + ext.Trim())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>選択基準を実装するデリゲート</summary>
        /// <param name="o">ファイル・エントリ情報</param>
        /// <param name="info">選択規準情報</param>
        /// <returns>
        /// 真：ファイルを書庫に追加する。
        /// 偽：ファイルを書庫に追加しない。
        /// </returns>
        public static bool SelectionCriteriaDlgt2(object o, object info)
        {
            string fileName = (string)o;

            // 選択規準情報（文字列配列）の
            // 指定が無い場合は、全て[true]

            if (info == null)
            {
                return true;
            }

            // 選択規準情報（文字列配列）の
            // 指定がある場合は、[true], [false]か判別

            string[] exts = (string[])info;
            foreach (string ext in exts)
            {
                string[] aryStr = fileName.Split('.'); // 修正
                if (aryStr[aryStr.Length - 1] == ext.Trim())
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region 組込デリゲート

        /// <summary>セーブ処理の進捗表示デリゲート</summary>
        public static void MySaveProgressEventHandler(Object sender, SaveProgressEventArgs e)
        {
            if (e.EventType == ZipProgressEventType.Saving_Started)
            {
                // 書庫の作成を開始
                //Debug.WriteLine(string.Format("{0} の作成開始", e.ArchiveName));
                //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                Debug.WriteLine(string.Format(ResourceMgr.GetString("I0001"), e.ArchiveName));
            }
            else if (e.EventType == ZipProgressEventType.Saving_BeforeWriteEntry)
            {
                // エントリの書き込み開始
                //Debug.WriteLine(string.Format("{0} の書き込み開始", e.CurrentEntry.FileName));
                //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                Debug.WriteLine(string.Format(ResourceMgr.GetString("I0002"), e.CurrentEntry.FileName));
            }
            else if (e.EventType == ZipProgressEventType.Saving_EntryBytesRead)
            {
                // エントリを書き込み中
                //Debug.WriteLine(string.Format("{0}/{1} バイト 書き込みました", e.BytesTransferred, e.TotalBytesToTransfer));
                //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                Debug.WriteLine(string.Format(ResourceMgr.GetString("I0003"), e.BytesTransferred, e.TotalBytesToTransfer));
            }
            else if (e.EventType == ZipProgressEventType.Saving_AfterWriteEntry)
            {
                // エントリの書き込み終了
                //Debug.WriteLine(string.Format("{0} の書き込み終了", e.CurrentEntry.FileName));
                //Debug.WriteLine(string.Format("{0} 個中 {1} 個のエントリの書き込みが完了しました", e.EntriesTotal, e.EntriesSaved));
                //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                Debug.WriteLine(string.Format(ResourceMgr.GetString("I0004"), e.CurrentEntry.FileName));
                Debug.WriteLine(string.Format(ResourceMgr.GetString("I0005"), e.EntriesTotal, e.EntriesSaved));
                    
            }
            else if (e.EventType == ZipProgressEventType.Saving_Completed)
            {
                // 書庫の作成が完了
                //Debug.WriteLine(string.Format("{0} の作成終了", e.ArchiveName));
                
                //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                Debug.WriteLine(string.Format(ResourceMgr.GetString("I0006"), e.ArchiveName));
            }
        }

        /// <summary>上書き確認デリゲート</summary>
        public static void MyExtractProgressEventHandler(Object sender, ExtractProgressEventArgs e)
        {
            if (e.EventType == ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite)
            {
                // 展開するファイル名
                string filePath = Path.Combine(
                    e.ExtractLocation, e.CurrentEntry.FileName.Replace('/', '\\'));

                // ダイアログを表示する
                //DialogResult res = MessageBox.Show(
                //    "'" + filePath + "'はすでに存在します。\r\n" +
                //    "'はい'で上書き 'いいえ'で何もしない 'キャンセル'で中止",
                //    "上書きの確認",
                //    MessageBoxButtons.YesNoCancel,
                //    MessageBoxIcon.Question);
                
                //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                DialogResult res = MessageBox.Show(
                    "'" + filePath + ResourceMgr.GetString("M0009")+ "\r\n" +
                    ResourceMgr.GetString("M0010"),
                    ResourceMgr.GetString("M0011"),
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                    
                if (res == DialogResult.Yes)
                {
                    // 上書きする
                    e.CurrentEntry.ExtractExistingFile =
                        ExtractExistingFileAction.OverwriteSilently;
                }
                else if (res == DialogResult.No)
                {
                    // 上書きしない
                    e.CurrentEntry.ExtractExistingFile =
                        ExtractExistingFileAction.DoNotOverwrite;
                }
                else if (res == DialogResult.Cancel)
                {
                    // 展開を中止する
                    e.Cancel = true;
                }
            }
        }

        #endregion

        #endregion

        #region メッセージ表示

        /// <summary>メッセージを表示する。</summary>
        /// <param name="message">メッセージ</param>
        /// <param name="lb">ログ・レベル</param>
        public static void OutPutMessage(string message, LogLevel lb)
        {
            Debug.WriteLine(message);
            Console.WriteLine(message);

            switch (lb)
            {
                case LogLevel.DebugLog:
                    LogIF.DebugLog("ACCESS", message);
                    break;
                case LogLevel.InfoLog:
                    LogIF.InfoLog("ACCESS", message);
                    break;
                case LogLevel.WarnLog:
                    LogIF.WarnLog("ACCESS", message);
                    break;
                case LogLevel.ErrorLog:
                    LogIF.ErrorLog("ACCESS", message);
                    break;
                case LogLevel.FatalLog:
                    LogIF.FatalLog("ACCESS", message);
                    break;
            }
        }

        /// <summary>コマンドの確認</summary>
        /// <param name="valsLst">値</param>
        /// <param name="argsDic">コマンド・値</param>
        public static void ConfirmCommand(List<string> valsLst, Dictionary<string, string> argsDic)
        {
            // 整形
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("");

            // カレント ディレクトリ
            //sb.AppendLine("カレント ディレクトリ");
            //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
            sb.AppendLine(ResourceMgr.GetString("M0012"));
            sb.AppendLine(Program.OrgCurrentDirectory);

            sb.AppendLine("");

            // 起動条件
            //sb.AppendLine("起動条件");
            //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
            sb.AppendLine(ResourceMgr.GetString("M0013"));

            sb.AppendLine("");

            // 値
            //sb.AppendLine("値");
            //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
            sb.AppendLine(ResourceMgr.GetString("M0014"));
            foreach (string vals in valsLst)
            {
                sb.AppendLine(vals);
            }

            sb.AppendLine("");

            // コマンド・値
            //sb.AppendLine("コマンド・値");
            //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
            sb.AppendLine(ResourceMgr.GetString("M0015"));
            foreach (string key in argsDic.Keys)
            {
                sb.AppendLine(string.Format("{0} : {1}", key, argsDic[key]));
            }

            // 出力
            Program.OutPutMessage(sb.ToString(), LogLevel.DebugLog);
        }

        #endregion

        #endregion

        /// <summary>
        /// GCする際のお作法
        /// WaitForPendingFinalizersで待機
        /// → 解凍ZIPファイルをCloseさせる
        /// </summary>
        public static void system_gc_collecting()
        {
            try
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.GC.Collect();
            }
#pragma warning disable
            catch (Exception ex)
#pragma warning restore
            {
                // ・・・
            }
        }
    }
}

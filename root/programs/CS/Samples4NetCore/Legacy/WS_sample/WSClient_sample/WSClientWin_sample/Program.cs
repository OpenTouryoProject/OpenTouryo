//**********************************************************************************
//* ３層型 サンプル アプリ
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Program
//* クラス日本語名  ：アプリケーションのメイン エントリ ポイント
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using System;
using System.Threading;
using System.Windows.Forms;

using Touryo.Infrastructure.Business.RichClient.Util;

namespace WSClientWin_sample
{
    /// <summary>アプリケーションのメイン エントリ ポイント</summary>
    static class Program
    {
        /// <summary>AccessToken</summary>
        public static string AccessToken = "";

        /// <summary>終了するかどうかを表すフラグ</summary>
        public static bool FlagEnd = true;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 既定の処理
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // UnhandledExceptionイベント・ハンドラを登録する
            Thread.GetDomain().UnhandledException += new
              UnhandledExceptionEventHandler(Application_UnhandledException);

            // ThreadExceptionイベント・ハンドラを登録する
            Application.ThreadException += new
              ThreadExceptionEventHandler(Application_ThreadException);

            // スプラッシュ画面の表示
            Splash.ShowSplash(new Login());
                        
            // ＜スピンロック＞
            // SleepすればCPUオーバヘッドはほとんど無いが
            // Sleep時間を長く、ループ回数を短くする
            // ことでよりCPUオーバヘッドを軽減できる。

            for (int i = 0; i < 30; i++ )
            {
                if (Splash.SpinLock)
                {
                    break; // 直ちに抜ける
                }

                Thread.Sleep(100);
            }

            // ThreadExceptionイベント・ハンドラを登録する
            Application.ThreadException += new
              ThreadExceptionEventHandler(Application_ThreadException);

            // 次の画面（ログイン画面）の表示
            Application.Run(Splash.NextForm);
            if(Program.FlagEnd) 
            {
                return; // ログインしないで終わった場合
            }

            // ThreadExceptionイベント・ハンドラを登録する
            Application.ThreadException += new
              ThreadExceptionEventHandler(Application_ThreadException);

            // 業務画面の表示（業務の開始）
            Application.Run(new Form1());
        }

        // .NET TIPS > 適切に処理されなかった例外をキャッチするには？
        // http://www.atmarkit.co.jp/fdotnet/dotnettips/320appexception/appexception.html

        /// <summary>
        /// 未処理例外をキャッチするイベント・ハンドラ
        /// </summary>
        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            RcMyCmnFunction.ShowErrorMessageWin(e.Exception, "Application_ThreadExceptionによる例外通知です。");
        }

        /// <summary>
        /// 未処理例外をキャッチするイベント・ハンドラ 
        /// </summary>
        /// <remarks>
        /// メイン・スレッド以外の例外はUnhandledExceptionでハンドル
        /// </remarks>
        public static void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                RcMyCmnFunction.ShowErrorMessageWin(ex, "Application_UnhandledExceptionによる例外通知です。");
            }
        }
    }
}

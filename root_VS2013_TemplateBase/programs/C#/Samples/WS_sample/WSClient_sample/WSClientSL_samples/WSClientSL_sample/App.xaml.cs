using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Net.Browser;

namespace WSClientSL_sample
{
    /// <summary>エントリポイント</summary>
    public partial class App : Application
    {
        /// <summary>コンストラクタ</summary>
        public App()
        {
            // 各種イベントハンドラの指定
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            // 初期化
            InitializeComponent();
        }

        /// <summary>スタートアップ イベントハンドラ</summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.RootVisual = new MainPage();

            //HttpWebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
            //HttpWebRequest.RegisterPrefix("https://", WebRequestCreator.ClientHttp);
            // を、
            HttpWebRequest.RegisterPrefix("http://", WebRequestCreator.BrowserHttp);
            HttpWebRequest.RegisterPrefix("https://", WebRequestCreator.BrowserHttp);
            // にするか、この 2 行を削除するとうまくいく。

            // 方法: ブラウザーまたはクライアントによる HTTP 処理を指定する
            // http://msdn.microsoft.com/ja-jp/library/dd920295(v=VS.95).aspx
            // ClientHttp は「クライアントによる HTTP 処理を指定する」もので、
            // BrowserHttp は「ブラウザーによる HTTP 処理を指定する」もの

            // Silverlight - ChipStar Lightのメモ - livedoor Wiki（ウィキ）
            // http://wiki.livedoor.jp/chipstar_light/d/Silverlight
            // BrowserHttpはブラウザ上でSLアプリを利用する場合の設定
            // ClientHttpは、ブラウザ外部でSLアプリを利用する場合の設定
        }
        /// <summary>終了イベントハンドラ</summary>
        private void Application_Exit(object sender, EventArgs e) { }

        /// <summary>未処理例外発生イベントハンドラ</summary>
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // アプリケーションがデバッガーの外側で実行されている場合、ブラウザーの
            // 例外メカニズムによって例外が報告されます。これにより、IE ではステータス バーに
            // 黄色の通知アイコンが表示され、Firefox にはスクリプト エラーが表示されます。
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // メモ : これにより、アプリケーションは例外がスロー
                // された後も実行され続け、例外はハンドルされません。 
                // 実稼動アプリケーションでは、このエラー処理は、Web サイトにエラーを
                // 報告し、アプリケーションを停止させるものに置換される必要があります。
                e.Handled = true;

                // JavaScriptで例外レポートを報告
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }

            // MessageBoxで例外情報表示
            MessageBox.Show(e.ExceptionObject.Message);

            // MessageBoxで内部例外情報表示
            if (e.ExceptionObject.InnerException != null)
            {
                MessageBox.Show(e.ExceptionObject.InnerException.Message);
            }
        }

        /// <summary>Evalを使用して、JavaScriptで例外レポートを報告する。</summary>
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                // エラーメッセージの整形
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                // 例外レポートを報告用のJavaScriptを生成する。
                System.Windows.Browser.HtmlPage.Window.Eval(
                    "throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}

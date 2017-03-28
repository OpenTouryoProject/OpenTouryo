//**********************************************************************************
//* Windows Forms用 Ｐ層 フレームワーク・テスト アプリ画面
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Splash
//* クラス日本語名  ：スプラッシュ画面
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

using Touryo.Infrastructure.Public.Log;

namespace WSClientWin2_sample
{
    /// <summary>
    /// スプラッシュ画面
    /// </summary>
    /// <remarks>
    /// DOBON.NET > プログラミング道 > .NET Tips
    ///  >  フォーム >  スプラッシュウィンドウを表示する
    /// http://dobon.net/vb/dotnet/form/splashwindow.html
    /// 
    /// ここでは、フレームワークは使用しない。
    /// </remarks>
    public partial class Splash : Form
    {
        /// <summary>コンストラクタ</summary>
        public Splash()
        {
            InitializeComponent();

            // プロパティの初期化
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;

            // イベントの設定
            this.Click += new System.EventHandler(Splash.Splash_Click);
            this.label1.Click += new System.EventHandler(Splash.Splash_Click);

            // ログの初期化
            LogIF.InfoLog("ACCESS", "Splash");
        }

        /// <summary>
        /// スプラッシュ画面のクリックイベント
        /// </summary>
        private static void Splash_Click(object sender, EventArgs e)
        {
            // 副スレッド（スプラッシュ画面を生成したスレッド）

            // スピンロックを終了させ次画面を表示する。
            Splash._spinLock = true; 
        }

        #region 静的変数

        /// <summary>実行済みフラグ</summary>
        private static bool _hasExecuted = false;

        /// <summary>スプラッシュ画面表示スレッド</summary>
        private static Thread _thread = null;

        /// <summary>スピンロック用フラグ</summary>
        /// <remarks>volatile:スレッドセーフ</remarks>
        private static volatile bool _spinLock = false;

        /// <summary>スピンロック用フラグ（Getter）</summary>
        public static bool SpinLock
        {
            get { return Splash._spinLock; }
        }

        /// <summary>スプラッシュ画面（シングルトン）</summary>
        /// <remarks>volatile:スレッドセーフ</remarks>
        private static volatile Splash _splashForm = new Splash();

        /// <summary>次の画面（シングルトン）</summary>
        /// <remarks>volatile:スレッドセーフ</remarks>
        private static volatile Form _nextForm = null;

        /// <summary>次の画面（Getter）</summary>
        public static Form NextForm
        {
            get { return Splash._nextForm; }
        }

        #endregion

        #region スプラッシュ画面を表示する

        /// <summary>スプラッシュ画面を表示する</summary>
        /// <param name="nextForm">次の画面</param>
        public static void ShowSplash(Form nextForm)
        {
            // 主スレッド（スプラッシュ画面を生成していないスレッド）

            // 二回以上は起動できない。
            if (Splash._hasExecuted)
            {
                return;
            }
            else
            {
                Splash._hasExecuted = true;
            }

            #region スプラッシュ画面を表示

            // 次の画面を設定する。
            Splash._nextForm = nextForm;
            　
            // スレッドの作成
            Splash._thread = new Thread(
                new ThreadStart(ShowSplashByThread));
            
            // スレッドの開始
            Splash._thread.Start();

            #endregion
        }

        /// <summary>Thread関数でスプラッシュ画面を表示する。</summary>
        private static void ShowSplashByThread()
        {
            // 副スレッド（スプラッシュ画面を生成したスレッド）

            // スプラッシュ画面を

            // ・作成
            Splash._splashForm = new Splash();

            // ・閉じるイベントハンドラを仕掛
            Splash._nextForm.Activated += new EventHandler(Splash.Login_Activated);

            // ・表示
            Application.Run(Splash._splashForm);
        }

        #endregion

        #region スプラッシュ画面を閉じる

        /// <summary>
        /// ログイン画面がアクティブになった時、スプラッシュ画面を閉じる
        /// </summary>
        private static void Login_Activated(object sender, EventArgs e)
        {   
            // 主スレッド（スプラッシュ画面を生成していないスレッド）

            // なので、スプラッシュ画面を閉じるメソッドをInvoke
            if (Splash._splashForm != null && !Splash._splashForm.IsDisposed)
            {
                Splash._splashForm.Invoke(new MethodInvoker(Splash.CloseSplash));
            }

            // nullクリア
            Splash._splashForm = null;
            Splash._nextForm = null;
            Splash._thread = null;
        }

        /// <summary>スプラッシュ画面を閉じる。</summary>
        private static void CloseSplash()
        {
            // 副スレッド（スプラッシュ画面を生成したスレッド）

            // なので、スプラッシュ画面をそのまま閉じる
            Splash._splashForm.Close();
        }

        #endregion
    }
}
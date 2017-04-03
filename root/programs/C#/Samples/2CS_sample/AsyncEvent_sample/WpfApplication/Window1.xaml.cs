//**********************************************************************************
//* 非同期イベント・サンプル アプリ画面
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Form1
//* クラス日本語名  ：Form1
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
using System.IO;
using System.Threading;
using System.Windows;

using Touryo.Infrastructure.Framework.RichClient.Asynchronous;
using Touryo.Infrastructure.Public.IO;

namespace WpfApplication
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class Window1 : Window
    {
        #region メンバ変数

        /// <summary>名前付きパイプ・サーバ名</summary>
        private string NPS = null;
        /// <summary>名前付きパイプ・クライアント</summary>
        private string[] NPCS = null;

        /// <summary>登録エントリ（Thread）</summary>
        private AsyncEventEntry AeeTh = null;
        /// <summary>登録エントリ（ThreadPool）</summary>
        private AsyncEventEntry AeePl = null;
        /// <summary>登録エントリ（WinForm）</summary>
        private AsyncEventEntry AeeWin = null;
        /// <summary>登録エントリ（WPF）</summary>
        private AsyncEventEntry AeeWPF = null;

        #endregion

        #region 開始・終了処理

        #region 開始処理

        /// <summary>コンストラクタ</summary>
        public Window1()
        {
            InitializeComponent();

            // サーバを起動
            string[] args = Environment.CommandLine.Split('/');
            args = args[1].Trim().Split(',');

            // this.NPS
            this.NPS = args[0].Trim();
            this.Title = this.NPS;

            // this.NPCS
            this.NPCS = new string[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
            {
                this.NPCS[i - 1] = args[i].Trim();
            }

            // 初期化

            // 初めが自分の名称、

            //// ２つ目からが相手の名称
            //MessageBox.Show(
            //    "this.NPS:" + this.NPS
            //    + "\r\nthis.NPCS:" + string.Join(",", this.NPCS),
            //    "コマンドラインのチェック");

            AsyncEventFx.Init(this.NPS, this.NPCS, 3000);
        }

        /// <summary>ロード</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 登録エントリ

            // スレッド
            this.AeeTh = new AsyncEventEntry(
                AsyncEventEnum.EventClass.Thread, "Thread", null,
                new ParameterizedThreadStart(this.ParameterizedThreadStartDgt));

            AsyncEventFx.RegisterAsyncEvent(this.AeeTh);

            // ---

            // スレッド プール
            this.AeePl = new AsyncEventEntry(
                AsyncEventEnum.EventClass.ThreadPool, "ThreadPool", null,
                new WaitCallback(this.WaitCallbackDgt));

            AsyncEventFx.RegisterAsyncEvent(this.AeePl);

            // ---

            // WinForm
            this.AeeWin = new AsyncEventEntry(
                AsyncEventEnum.EventClass.WinForm, "WinForm", this,
                new AsyncEventFx.SetResultDelegate(this.SetResultDgt));

            AsyncEventFx.RegisterAsyncEvent(this.AeeWin);

            // ---

            // WPF
            this.AeeWPF = new AsyncEventEntry(
                AsyncEventEnum.EventClass.WPF, "WPF", this,
                new AsyncEventFx.SetResultDelegate(this.SetResultDgt));

            AsyncEventFx.RegisterAsyncEvent(this.AeeWPF);
        }

        #endregion

        #region 終了処理

        /// <summary>アンロード</summary>
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            // 終了
            AsyncEventFx.Final();
        }

        // ↑↓どっち？？

        /// <summary>クローズ</summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            // 終了
            AsyncEventFx.Final();
        }

        #endregion

        #endregion

        #region 各種デリゲード

        /// <summary>デリゲード</summary>
        private void ParameterizedThreadStartDgt(object obj)
        {
            object[] param = (object[])obj;
            AsyncEventHeader aeh = (AsyncEventHeader)param[0];
            string msg = (string)BinarySerialize.BytesToObject((byte[])param[1]);

            // ファイルにテキストを書き出し。
            using (StreamWriter sw = new StreamWriter(this.NPS + @"_test_pts.txt", true))
            {
                sw.WriteLine("\r\n" + this.NPS + " - "
                + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                + "WpfApplication.Window1.ParameterizedThreadStartDgtメソッドが呼ばれた。"
                + "\r\nDstEventClass:" + aeh.DstEventClass.ToString()
                + "\r\nDstFuncID:" + (new string(aeh.DstFuncID)).Trim()
                + "\r\nSrcEventClass:" + aeh.SrcEventClass.ToString()
                + "\r\nSrcFuncID:" + (new string(aeh.SrcFuncID)).Trim()
                + "\r\nSrcPipeName:" + (new string(aeh.SrcPipeName)).Trim()
                + "\r\nメッセージ:" + msg, this.NPS);
            }
        }

        /// <summary>デリゲード</summary>
        private void WaitCallbackDgt(object state)
        {
            object[] param = (object[])state;
            AsyncEventHeader aeh = (AsyncEventHeader)param[0];
            string msg = (string)BinarySerialize.BytesToObject((byte[])param[1]);

            // ファイルにテキストを書き出し。
            using (StreamWriter sw = new StreamWriter(this.NPS + @"_test_tpl.txt", true))
            {
                sw.WriteLine("\r\n" + this.NPS + " - "
                + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                + "WpfApplication.Window1.WaitCallbackDgtメソッドが呼ばれた。"
                + "\r\nDstEventClass:" + aeh.DstEventClass.ToString()
                + "\r\nDstFuncID:" + (new string(aeh.DstFuncID)).Trim()
                + "\r\nSrcEventClass:" + aeh.SrcEventClass.ToString()
                + "\r\nSrcFuncID:" + (new string(aeh.SrcFuncID)).Trim()
                + "\r\nSrcPipeName:" + (new string(aeh.SrcPipeName)).Trim()
                + "\r\nメッセージ:" + msg, this.NPS);
            }
        }

        /// <summary>デリゲード</summary>
        private void SetResultDgt(object result)
        {
            object[] param = (object[])result;
            AsyncEventHeader aeh = (AsyncEventHeader)param[0];
            string msg = (string)BinarySerialize.BytesToObject((byte[])param[1]);

            MessageBox.Show(this,this.NPS + " - "
                + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                + "WpfApplication.Window1.SetResultDgtメソッドが呼ばれた。"
                + "\r\nDstEventClass:" + aeh.DstEventClass.ToString()
                + "\r\nDstFuncID:" + (new string(aeh.DstFuncID)).Trim()
                + "\r\nSrcEventClass:" + aeh.SrcEventClass.ToString()
                + "\r\nSrcFuncID:" + (new string(aeh.SrcFuncID)).Trim()
                + "\r\nSrcPipeName:" + (new string(aeh.SrcPipeName)).Trim()
                + "\r\nメッセージ:" + msg, this.NPS);
        }

        #endregion

        #region 各種ボタン

        /// <summary>WPFのThread</summary>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            byte[] bytes = BinarySerialize.ObjectToBytes(this.txtMSG.Text);

            AsyncEventFx.SendAsyncEvent(
                AsyncEventEnum.EventClass.Thread, "Thread",
                AsyncEventEnum.EventClass.Thread, "Thread",
                this.NPCS[0], this.NPS, (uint)bytes.Length, bytes);
        }

        /// <summary>WPFのThreadPool</summary>
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            byte[] bytes = BinarySerialize.ObjectToBytes(this.txtMSG.Text);

            AsyncEventFx.SendAsyncEvent(
                AsyncEventEnum.EventClass.ThreadPool, "ThreadPool",
                AsyncEventEnum.EventClass.ThreadPool, "ThreadPool",
                this.NPCS[0], this.NPS, (uint)bytes.Length, bytes);
        }

        /// <summary>WPFのUIInvoke</summary>
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            byte[] bytes = BinarySerialize.ObjectToBytes(this.txtMSG.Text);

            AsyncEventFx.SendAsyncEvent(
                AsyncEventEnum.EventClass.WPF, "WPF",
                AsyncEventEnum.EventClass.WPF, "WPF",
                this.NPCS[0], this.NPS, (uint)bytes.Length, bytes);
        }

        /// <summary>WinFormのUIInvoke</summary>
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            byte[] bytes = BinarySerialize.ObjectToBytes(this.txtMSG.Text);

            AsyncEventFx.SendAsyncEvent(
                AsyncEventEnum.EventClass.WinForm, "WinForm",
                AsyncEventEnum.EventClass.WinForm, "WinForm",
                this.NPCS[1], this.NPS, (uint)bytes.Length, bytes);
        }

        /// <summary>へんなところ</summary>
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            byte[] bytes = BinarySerialize.ObjectToBytes(this.txtMSG.Text);

            AsyncEventFx.SendAsyncEvent(
                AsyncEventEnum.EventClass.WinForm, "いいい",
                AsyncEventEnum.EventClass.WinForm, "いいい",
                this.NPCS[1], "あああ", (uint)bytes.Length, bytes);
                //this.NPCS[1], this.NPS, (uint)bytes.Length, bytes);
        }

        #endregion        

        #region 各種ボタン（エントリ）

        /// <summary>エントリを登録</summary>
        private void button6_Click(object sender, RoutedEventArgs e)
        {   
            AsyncEventFx.RegisterAsyncEvent(this.AeeTh);
            AsyncEventFx.RegisterAsyncEvent(this.AeePl);
            AsyncEventFx.RegisterAsyncEvent(this.AeeWin);
            AsyncEventFx.RegisterAsyncEvent(this.AeeWPF);
        }

        /// <summary>エントリを削除</summary>
        private void button7_Click(object sender, RoutedEventArgs e)
        {
            AsyncEventFx.UnRegisterAsyncEvent(this.AeeTh);
            AsyncEventFx.UnRegisterAsyncEvent(this.AeePl);
            AsyncEventFx.UnRegisterAsyncEvent(this.AeeWin);
            AsyncEventFx.UnRegisterAsyncEvent(this.AeeWPF);
        }

        #endregion
    }
}

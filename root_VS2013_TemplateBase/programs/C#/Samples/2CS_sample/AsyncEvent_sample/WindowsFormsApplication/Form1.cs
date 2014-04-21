using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Threading;
using System.Diagnostics;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Framework.RichClient.Asynchronous;

namespace WindowsFormsApplication
{
    public partial class Form1 : Form
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
        public Form1()
        {
            InitializeComponent();

            // サーバを起動
            string[] args = Environment.CommandLine.Split('/');
            args = args[1].Trim().Split(',');

            // this.NPS
            this.NPS = args[0].Trim();
            this.Text = this.NPS;

            // this.NPCS
            this.NPCS = new string[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
            {
                this.NPCS[i-1] = args[i].Trim();
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

        /// <summary>初期処理</summary>
        private void Form1_Load(object sender, EventArgs e)
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

        /// <summary>終了処理</summary>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
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
                + "WindowsFormsApplication.Form1.ParameterizedThreadStartDgtメソッドが呼ばれた。"
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
                + "WindowsFormsApplication.Form1.WaitCallbackDgtメソッドが呼ばれた。"
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

            MessageBox.Show(this.NPS + " - " 
                + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                + "WindowsFormsApplication.Form1.SetResultDgtメソッドが呼ばれた。"
                + "\r\nDstEventClass:" + aeh.DstEventClass.ToString()
                + "\r\nDstFuncID:" + (new string(aeh.DstFuncID)).Trim()
                + "\r\nSrcEventClass:" + aeh.SrcEventClass.ToString()
                + "\r\nSrcFuncID:" + (new string(aeh.SrcFuncID)).Trim()
                + "\r\nSrcPipeName:" + (new string(aeh.SrcPipeName)).Trim()
                + "\r\nメッセージ:" + msg, this.NPS);
        }

        #endregion

        #region 各種ボタン（イベント送信）

        /// <summary>WinFormのThread</summary>
        private void button1_Click(object sender, EventArgs e)
        {
            byte[] bytes = BinarySerialize.ObjectToBytes(this.txtMSG.Text);

            AsyncEventFx.SendAsyncEvent(
                AsyncEventEnum.EventClass.Thread, "Thread",
                AsyncEventEnum.EventClass.Thread, "Thread",
                this.NPCS[0], this.NPS, (uint)bytes.Length, bytes);
        }

        /// <summary>WinFormのThreadPool</summary>
        private void button2_Click(object sender, EventArgs e)
        {
            byte[] bytes = BinarySerialize.ObjectToBytes(this.txtMSG.Text);

            AsyncEventFx.SendAsyncEvent(
                AsyncEventEnum.EventClass.ThreadPool, "ThreadPool",
                AsyncEventEnum.EventClass.ThreadPool, "ThreadPool",
                this.NPCS[0], this.NPS, (uint)bytes.Length, bytes);
        }

        /// <summary>WinFormのUIInvoke</summary>
        private void button3_Click(object sender, EventArgs e)
        {
            byte[] bytes = BinarySerialize.ObjectToBytes(this.txtMSG.Text);

            AsyncEventFx.SendAsyncEvent(
                AsyncEventEnum.EventClass.WinForm, "WinForm",
                AsyncEventEnum.EventClass.WinForm, "WinForm",
                this.NPCS[0], this.NPS, (uint)bytes.Length, bytes);
        }

        /// <summary>WPFのUIInvoke</summary>
        private void button4_Click(object sender, EventArgs e)
        {
            byte[] bytes = BinarySerialize.ObjectToBytes(this.txtMSG.Text);

            AsyncEventFx.SendAsyncEvent(
                AsyncEventEnum.EventClass.WPF, "WPF",
                AsyncEventEnum.EventClass.WPF, "WPF",
                this.NPCS[1], this.NPS, (uint)bytes.Length, bytes);
        }

        /// <summary>へんなところ</summary>
        private void button5_Click(object sender, EventArgs e)
        {
            byte[] bytes = BinarySerialize.ObjectToBytes(this.txtMSG.Text);

            AsyncEventFx.SendAsyncEvent(
                AsyncEventEnum.EventClass.WPF, "いいい",
                AsyncEventEnum.EventClass.WPF, "いいい",
                this.NPCS[1], "あああ", (uint)bytes.Length, bytes);
            //this.NPCS[1], this.NPS, (uint)bytes.Length, bytes);
        }

        #endregion

        #region 各種ボタン（エントリ）

        /// <summary>エントリを登録</summary>
        private void button6_Click(object sender, EventArgs e)
        {
            AsyncEventFx.RegisterAsyncEvent(this.AeeTh);
            AsyncEventFx.RegisterAsyncEvent(this.AeePl);
            AsyncEventFx.RegisterAsyncEvent(this.AeeWin);
            AsyncEventFx.RegisterAsyncEvent(this.AeeWPF);
        }

        /// <summary>エントリを削除</summary>
        private void button7_Click(object sender, EventArgs e)
        {
            AsyncEventFx.UnRegisterAsyncEvent(this.AeeTh);
            AsyncEventFx.UnRegisterAsyncEvent(this.AeePl);
            AsyncEventFx.UnRegisterAsyncEvent(this.AeeWin);
            AsyncEventFx.UnRegisterAsyncEvent(this.AeeWPF);
        }

        #endregion
    }
}

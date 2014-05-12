//**********************************************************************************
//* サンプル アプリ画面
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：Form1
//* クラス日本語名  ：サンプル アプリ画面
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

using System.Threading;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;

// Windowアプリケーション
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;

using Touryo.Infrastructure.Business.RichClient.Asynchronous;
using Touryo.Infrastructure.Business.RichClient.Presentation;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

using Touryo.Infrastructure.Framework.RichClient.Asynchronous;
using Touryo.Infrastructure.Framework.RichClient.Presentation;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

using System.Diagnostics;

namespace WSClientWin2_sample
{
    /// <summary>Form1</summary>
    public partial class Form1 : MyBaseControllerWin
    {
        /// <summary>コンストラクタ</summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>フォームロードのUOCメソッド</summary>
        protected override void UOC_FormInit()
        {
            this.numericUpDown1.Value = 5;
            this.comboBox1.SelectedIndex = 0;
        }

        /// <summary>同期実行</summary>
        protected void UOC_btnSync_Click(RcFxEventArgs rcFxEventArgs)
        {
            int wait = (int)this.numericUpDown1.Value;

            this.AddStatus(string.Format("主スレッド実行中: {0}秒待つ", wait));

            Thread.Sleep(wait * 1000);

            this.AddStatus("スレッド実行終了");

            // 結果表示のテスト
            this.TestOfResultDisplay();
        }

        /// <summary>非同期実行</summary>
        private void UOC_btnASync_Click(RcFxEventArgs rcFxEventArgs)
        {
            int wait = (int)this.numericUpDown1.Value;

            MyBaseAsyncFunc af = new MyBaseAsyncFunc(this);
            //MyBaseAsyncFunc af = new MyBaseAsyncFunc(this.panel1);

            // 非同期処理本体・無名関数デレゲード
            af.AsyncFunc = delegate(object param)
            {
                // 進捗報告
                af.ExecChangeProgress(string.Format("スレッド実行中: {0}秒待つ", wait));

                System.Threading.Thread.Sleep(wait * 1000);

                return "終わり";
            };

            // 進捗報告・無名関数デレゲード
            af.ChangeProgress = delegate(object param)
            {
                string text = (string)param;
                this.AddStatus(text);
            };

            // 結果設定・無名関数デレゲード
            af.SetResult = delegate(object retVal)
            {
                if (retVal is Exception)
                {
                    // 例外発生時
                    Exception ex = (Exception)retVal;
                    this.AddStatus(string.Format("スレッド実行終了: エラー発生:{0}", ex.Message));
                }
                else
                {
                    this.AddStatus("スレッド実行終了");
                    //throw new Exception("SetResultでエラーとなった場合。");
                }

                // 結果表示のテスト
                this.TestOfResultDisplay();

                // フォーカス制御をする場合、
                this.BeginInvoke(new MethodInvoker(this.SetForcus));

            };

            // 非同期処理を開始させる。
            if (af.Start())
            {
                this.AddStatus(string.Format(
                    "キューイングされました、現在のスレッド数:{0}",
                    BaseAsyncFunc.ThreadCount.ToString()));
            }
            else
            {
                this.AddStatus(string.Format(
                    "非同期スレッドが最大数に達しています。:{0}",
                    BaseAsyncFunc.ThreadCount.ToString()));
            }
        }

        /// <summary>フォーカス制御のメソッド</summary>
        private void SetForcus()
        {
            if (this.Enabled)
            {
                this.textBox3.Focus();
            }
            else
            {
                // Form.Enabled=falseの場合、再キューイング
                this.BeginInvoke(new MethodInvoker(this.SetForcus));
            }            
        }

        /// <summary>テキストボックスに書き込み</summary>
        /// <param name="text">追加するテキスト</param>
        private void AddStatus(string text)
        {
            this.txtStatus.Text = 
                string.Format("{0}{1}\r\n", this.txtStatus.Text, text);
        }

        /// <summary>結果表示のテスト</summary>
        private void TestOfResultDisplay()
        {
            if (cbxWindow.Checked)
            {
                // ダイアログの表示
                Form2 f = new Form2();
                f.Show();
                return;
            }
            else if (cbxDialog.Checked)
            {
                // フォームの表示
                Form2 f = new Form2();
                f.ShowDialog();
                return;
            }
            else if (cbxClick.Checked)
            {
                // PerformClickは動作しない。
                this.btnButton1.PerformClick();
                return;
            }
            else if (cbxDoClick.Checked || cbxDoClick2.Checked)
            {
                // DoClickは動作する。
                this.btnHdnBtn1.DoClick();
                return;
            }
        }

        /// <summary>画面起動</summary>
        protected void UOC_btnOpenForm2_Click(RcFxEventArgs rcFxEventArgs)
        {
            if (MyBaseControllerWin.GetWindowsCount(typeof(Form2)) > 3)
            {
                MessageBox.Show("５画面以上は起動できません。");
            }
            else
            {
                Form2 f = new Form2();
                f.Show();
            }
        }

        /// <summary>メソッド実装あり</summary>
        protected void UOC_btnButton1_Click(RcFxEventArgs rcFxEventArgs)
        {
            MessageBox.Show("UOC_btnButton1_Click");
        }

        /// <summary>隠しボタンのイベント実装</summary>
        protected void UOC_btnHdnBtn1_Click(RcFxEventArgs rcFxEventArgs)
        {
            MessageBox.Show("UOC_btnHdnBtn1_Click");

            if (cbxDoClick2.Checked && txtStatus.Text.Length < 500)
            {
                //// 反転
                //cbxDoClick2.Checked = !cbxDoClick2.Checked;

                int wait = (int)this.numericUpDown1.Value;

                //MyBaseAsyncFunc af = new MyBaseAsyncFunc(this);
                MyBaseAsyncFunc af = new MyBaseAsyncFunc(this.panel1);

                // 非同期処理本体・無名関数デレゲード
                af.AsyncFunc = delegate(object param)
                {
                    // 進捗報告
                    af.ExecChangeProgress(string.Format("スレッド実行中: {0}秒待つ", wait));

                    System.Threading.Thread.Sleep(wait * 1000);

                    return "終わり";
                };

                // 進捗報告・無名関数デレゲード
                af.ChangeProgress = delegate(object param)
                {
                    string text = (string)param;
                    this.AddStatus(text);
                };

                // 結果設定・無名関数デレゲード
                af.SetResult = delegate(object retVal)
                {
                    if (retVal is Exception)
                    {
                        // 例外発生時
                        Exception ex = (Exception)retVal;
                        this.AddStatus(string.Format("スレッド実行終了: エラー発生:{0}", ex.Message));
                    }
                    else
                    {
                        this.AddStatus("スレッド実行終了");
                    }

                    // 結果表示のテスト
                    this.TestOfResultDisplay();
                };

                // 非同期処理を開始させる。
                if (af.Start())
                {
                    this.AddStatus(string.Format(
                        "キューイングされました、現在のスレッド数:{0}",
                        BaseAsyncFunc.ThreadCount.ToString()));
                }
                else
                {
                    this.AddStatus(string.Format(
                        "非同期スレッドが最大数に達しています。:{0}",
                        BaseAsyncFunc.ThreadCount.ToString()));
                }
            }
        }

        /// <summary>SetResultで動作するか確認する。</summary>
        private void txtStatus_TextChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("txtStatus_TextChanged");
        }
    }
}

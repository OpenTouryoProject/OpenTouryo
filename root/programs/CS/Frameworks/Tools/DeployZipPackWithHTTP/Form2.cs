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
//* クラス名        ：Form2
//* クラス日本語名  ：サブ画面（CUIの進捗報告画面）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/06/17  西野 大介         新規作成
//*  2011/07/01  西野 大介         最後に進捗が100％になるように変更
//*  2011/07/01  西野 大介         コマンドライン引数のハッシュキーを全て大文字化
//*  2011/08/01  西野 大介         タイトルをメッセージ定義から取得するように変更。
//*  2011/08/01  西野 大介         最大化、最小化、閉じるボタンを無効にした。
//*  2011/08/01  西野 大介         最後、自身を閉じる（非同期待機時間を設定可能に）
//*  2011/08/01  西野 大介         /QUIET クワイエット モードを追加した。
//*  2011/08/01  西野 大介         進捗値の進み具合を調整した。
//*  2011/08/01  西野 大介         ステータスの書き込み（終了時）
//*  2011/08/30  西野 大介         異常終了時に退避ディレクトリからリカバリ。
//*  2011/08/30  西野 大介         テンポラリの削除処理を追加した（例外処理を追加）。
//*  2011/08/30  西野 大介         Exceptionではメッセージ表示しないよう修正した。
//*  2011/09/05  西野 大介         GCするコードを追加→修正（解凍ZIPがCloseされないため）
//*  2011/09/08  西野 大介         リカバリ処理のログ出力位置の変更
//*                                （リカバリしない時ログを出力しないよう変更）。
//*  2011/09/12  西野 大介         画面表示せず、ログ出力のみする例外処理方式を追加
//*  2014/04/26  Sai               Replaced all the Japanese language in both UI and code with ResorceManager.GetString() method call
//**********************************************************************************

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Security.Permissions;
using System.Resources;
using System.Windows.Forms;

using Touryo.Infrastructure.Business.RichClient.Asynchronous;
using Touryo.Infrastructure.Framework.RichClient.Asynchronous;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Util;

namespace DeployZipPackWithHTTP
{
    public partial class Form2 : Form
    {
        /// <summary>値</summary>
        private List<string> ValsLst;
        /// <summary>コマンド・値</summary>
        private Dictionary<string, string> ArgsDic;

        /// <summary>ステータス表示用</summary>
        public string Status
        {
            set
            {
                this.lblStatus.Text = value;
            }
            get
            {
                return this.lblStatus.Text;
            }
        }
        
        /// <summary>
        /// Getting ResourceManager instance from Resources to apply internationalization
        /// </summary>
        private ResourceManager ResourceMgr
        {
            get
            {
                return Resources.Resource.ResourceManager;
            }
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="valsLst">値</param>
        /// <param name="argsDic">コマンド・値</param>
        public Form2(List<string> valsLst, Dictionary<string, string> argsDic)
        {
            InitializeComponent();

            this.ValsLst = valsLst;
            this.ArgsDic = argsDic;

            // タイトルの設定
            string temp 
                = GetMessage.GetMessageDescription("I0000");

            if (string.IsNullOrEmpty(temp))
            {
                // IsNullOrEmptyの場合はデフォルト
            }
            else
            {
                // タイトルを設定した。
                this.Text = temp;
            }
        }

        /// <summary>Form2_Load</summary>
        private void Form2_Load(object sender, EventArgs e)
        {
            // コマンドの確認
            Program.ConfirmCommand(this.ValsLst, this.ArgsDic);

            // エントリを生成
            Entry entry = new Entry();

            // コマンドラインから各値を取得する。
            entry.WWWURL = Program.NullToEmptyString(this.ArgsDic, "/WWWURL");
            entry.WWWUID = Program.NullToEmptyString(this.ArgsDic, "/WWWUID");
            entry.WWWPWD = Program.NullToEmptyString(this.ArgsDic, "/WWWPWD");
            entry.WWWDomain = Program.NullToEmptyString(this.ArgsDic, "/WWWDOMAIN");
            //---
            entry.ProxyURL = Program.NullToEmptyString(this.ArgsDic, "/PROXYURL");
            entry.ProxyUID = Program.NullToEmptyString(this.ArgsDic, "/PROXYUID");
            entry.ProxyPWD = Program.NullToEmptyString(this.ArgsDic, "/PROXYPWD");
            entry.ProxyDomain = Program.NullToEmptyString(this.ArgsDic, "/PROXYDOMAIN");

            //// エントリを保存
            //Program.SaveCurrent(entry);

            // 履歴をロード
            Program.LoadHistories();

            #region 更新処理の実行

            // 非同期呼び出し
            Program.Af = new MyBaseAsyncFunc(this.panel1);

            // 非同期処理本体デレゲード
            Program.Af.AsyncFunc = new BaseAsyncFunc.AsyncFuncDelegate(Program.ExecUpdate);

            // 進捗報告・無名関数デレゲード
            Program.Af.ChangeProgress = delegate(object param)
            {
                ChangeProgressParameter cpp = (ChangeProgressParameter)param;

                this.Status = cpp.Status;

                // 知的でないプログレス的なもの
                int progressVal = this.progressBar1.Value + cpp.ProgressVal;
                if (progressVal < this.progressBar1.Maximum - 5)
                {
                    // 95％を超えない場合、変更する。
                    this.progressBar1.Value = progressVal;
                }
                else
                {
                    // 95％を超える場合、変更しない。
                }

                this.Refresh();
            };

            // 結果設定・無名関数デレゲード
            Program.Af.SetResult = delegate(object retVal)
            {
                if (retVal is MyException)
                {
                    // 戻し
                    Program.Recover();
                    
                    // メッセージ表示時
                    MyException my_ex = (MyException)retVal;
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

                    // ステータスを書き込む
                    //this.Status = "正常終了";
                    //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                    this.Status = ResourceMgr.GetString("S0001");
                    if (ArgsDic.ContainsKey("/QUIET"))
                    {
                        // メッセージ表示しない
                    }
                    else
                    {
                        // メッセージ表示する
                        //MessageBox.Show(message, "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                        MessageBox.Show(message, ResourceMgr.GetString("M0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (retVal is Exception)
                {
                    // 戻し
                    Program.Recover();

                    // 例外発生時
                    string message = "";
                    Exception ex = (Exception)retVal;

                    //message += "＜メッセージ＞\r\n";
                    //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                    message += ResourceMgr.GetString("M0002") + "\r\n";
                    message += ex.Message;
                    message += "\r\n";

                    message += "\r\n";
                    //message += "＜スタック トレース＞\r\n";
                    //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                    message += ResourceMgr.GetString("M0003") + "\r\n";
                    message += ex.StackTrace;
                    message += "\r\n";

                    if (ex.InnerException != null)
                    {
                        message += "\r\n";
                        //message += "＜内部例外＞\r\n";
                        //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                        message += ResourceMgr.GetString("M0004") + "\r\n";
                        message += ex.InnerException.ToString();
                        message += "\r\n";
                    }

                    Program.OutPutMessage(message, LogLevel.ErrorLog);

                    // ステータスを書き込む
                    //this.Status = "異常終了";
                    //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                    this.Status = ResourceMgr.GetString("S0002");

                    // Exceptionではメッセージ表示しない。

                    //if (ArgsDic.ContainsKey("/QUIET"))
                    //{
                    //    // メッセージ表示しない
                    //}
                    //else
                    //{
                    //    // メッセージ表示する
                    //    CustMsgBox custMsgBox = new CustMsgBox(
                    //        "エラー", message, SystemIcons.Error);

                    //    custMsgBox.ShowDialog();
                    //}
                }
                else
                {
                    // 正常終了
                    
                    // ステータスを書き込む
                    //this.Status = "正常終了";
                    //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                    this.Status = ResourceMgr.GetString("S0001");

                    // 履歴を保存
                    Program.SaveHistories();
                }

                // 進捗を100％に変更
                this.progressBar1.Value = 100;

                // テンポラリの削除
                File.Delete(Program.OrgCurrentDirectory + Program.TempMftFileName);

                // GCでZIPが解放される可能性
                //GC.Collect();
                Program.system_gc_collecting();
                try
                {
                    File.Delete(Program.OrgCurrentDirectory + Program.TempZipFileName);
                }
                catch(Exception ex)
                {
                    // 例外を潰してログに出力
                    //Program.OutPutMessage(Program.TempZipFileName + "削除例外：" + ex.ToString(), LogLevel.ErrorLog);
                    //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                    Program.OutPutMessage(Program.TempZipFileName + ResourceMgr.GetString("E0002") + ex.ToString(), LogLevel.ErrorLog);
                }

                // 終了処理
                this.BeginInvoke(new MethodInvoker(FinalBeginInvoke));
            };

            // 非同期処理を開始させる。
            Program.Af.Parameter = entry;

            if (Program.Af.Start())
            {
                //this.ｘｘｘ(string.Format(
                //    "キューイングされました、現在のスレッド数:{0}",
                //    BaseAsyncFunc.ThreadCount.ToString()));
            }
            else
            {
                //this.ｘｘｘ(string.Format(
                //    "非同期スレッドが最大数に達しています。:{0}",
                //    BaseAsyncFunc.ThreadCount.ToString()));
            }

            #endregion
        }

        /// <summary>終了処理（３秒待って終了）</summary>
        private void FinalBeginInvoke()
        {
            // 非同期呼び出し
            Program.Af = new MyBaseAsyncFunc(this.panel1);

            // 非同期処理本体デレゲード
            Program.Af.AsyncFunc = delegate(object o)
            {
                // 指定時間スリープ
                string temp = GetConfigParameter.GetConfigValue("PDWSleepSec");

                if (!string.IsNullOrEmpty(temp))
                {
                    int sleepSec = 0;
                    if (int.TryParse(temp, out sleepSec))
                    {
                        // 指定の時間スリープ
                        Thread.Sleep(sleepSec * 1000);
                    }
                }

                return null;
            };

            // 進捗報告・無名関数デレゲード
            Program.Af.ChangeProgress = null;

            // 結果設定・無名関数デレゲード
            Program.Af.SetResult = delegate(object retVal)
            {
                // スリープの後、自身を閉じる。
                this.Close();
            };
            
            // 非同期処理を開始させる。
            Program.Af.Parameter = null;

            if (Program.Af.Start())
            {
                //this.ｘｘｘ(string.Format(
                //    "キューイングされました、現在のスレッド数:{0}",
                //    BaseAsyncFunc.ThreadCount.ToString()));
            }
            else
            {
                //this.ｘｘｘ(string.Format(
                //    "非同期スレッドが最大数に達しています。:{0}",
                //    BaseAsyncFunc.ThreadCount.ToString()));
            }
        }

        /// <summary>
        /// 閉じるボタンの無効化
        /// http://dobon.net/vb/dotnet/form/disabledclosebutton.html
        /// </summary>
        /// <param name="m">Windowメッセージ</param>
        [SecurityPermission(
            SecurityAction.LinkDemand,
            Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x112;
            const int SC_CLOSE = 0xF060;

            if (m.Msg == WM_SYSCOMMAND && m.WParam.ToInt32() == SC_CLOSE)
            {
                return;
            }

            base.WndProc(ref m);
        }
    }
}

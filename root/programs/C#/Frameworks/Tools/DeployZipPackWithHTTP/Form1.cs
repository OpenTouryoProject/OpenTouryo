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
//* クラス名        ：Form1
//* クラス日本語名  ：メイン画面
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/04/18  西野 大介         新規作成
//*  2011/07/01  西野 大介         履歴コンボは都度クリア
//*  2011/07/04  西野 大介         圧縮・解凍に入力チェック＋例外処理を追加
//*  2011/07/04  西野 大介         マニュフェスト作成に入力チェック＋例外処理を追加
//*  2011/08/30  西野 大介         異常終了時に退避ディレクトリからリカバリ。
//*  2011/08/30  西野 大介         テンポラリの削除処理を追加した（例外処理を追加）。
//*  2011/08/30  西野 大介         ファイル ダイアログのリセット処理を追加した。
//*  2011/08/30  西野 大介         ChangeProgressParameterの使用に漏れがあった。
//*  2011/09/05  西野 大介         GCするコードを追加→修正（解凍ZIPがCloseされないため）
//*  2011/09/08  西野 大介         リカバリ処理のログ出力位置の変更
//*                                （リカバリしない時ログを出力しないよう変更）。
//*  2011/09/12  西野 大介         画面表示せず、ログ出力のみする例外処理方式を追加
//*  2014/04/24  Sai               For internationalization, Replaced all the Japanese language exception messages with GetMessage.GetMessageDescription() method call 
//*                                Moved all the Japanese language exception messages to MSGDefinition_ja-JP.xml file Placed all the converted
//*                                Japanese language exception messages to MSGDefinition.xml file for internationalization supporting English Language.
//*  2014/04/25  Sai               Replaced all the Japanese language in both UI and code with ResorceManager.GetString() method call
//**********************************************************************************

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

using Ionic.Zip;
using Ionic.Zlib;

using Touryo.Infrastructure.Business.RichClient.Asynchronous;
using Touryo.Infrastructure.Framework.RichClient.Asynchronous;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;

namespace DeployZipPackWithHTTP
{
    /// <summary>メイン画面</summary>
    public partial class Form1 : Form
    {
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
        
        #region 初期処理

        /// <summary>コンストラクタ</summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>画面ロード</summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // デプロイ

            // 列挙型設定
            this.cmbEnc.SelectedIndex = 0;
            this.cmbCyp.DataSource = Enum.GetValues(typeof(EncryptionAlgorithm));
            this.cmbCmpLv.DataSource = Enum.GetValues(typeof(CompressionLevel));
            this.cmbFormat.DataSource = Enum.GetValues(typeof(SelfExtractorFlavor));
            this.cmbEEFA.DataSource = Enum.GetValues(typeof(ExtractExistingFileAction));

            // ZIP作成
            this.txtExt.Text = "txt,csv,cs";

            this.txtExt.Enabled = false;
            this.cmbFormat.Enabled = false;

            // 声明文作成

            //---

            // 履歴を復元
            this.LoadBins();
        }

        /// <summary>履歴で初期化</summary>
        private void cmbHistorys_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 履歴で初期化
            Entry entry = null;

            foreach (Entry temp in Program.Histories)
            {
                if (this.cmbHistorys.Text == temp.WWWURL)
                {
                    entry = temp;
                }
            }

            if (entry != null)
            {
                this.txtURL.Text = entry.WWWURL;
                this.txtUID.Text = entry.WWWUID;
                this.txtPWD.Text = entry.WWWPWD;
                this.txtDomain.Text = entry.WWWDomain;

                this.txtProxyURL.Text = entry.ProxyURL;
                this.txtProxyUID.Text = entry.ProxyUID;
                this.txtProxyPWD.Text = entry.ProxyPWD;
                this.txtProxyDomain.Text = entry.ProxyDomain;
            }
        }

        #region データ ロード
        
        /// <summary>バイナリファイルをロード</summary>
        private void LoadBins()
        {
            this.LoadEntry();
            this.LoadHistories();

            this.Refresh();
        }

        /// <summary>エントリのロード</summary>
        private void LoadEntry()
        {
            // current.bin（前回入力）
            Entry entry = Program.LoadCurrent();

            // 設定
            if (entry != null)
            {
                this.txtURL.Text = entry.WWWURL;
                this.txtUID.Text = entry.WWWUID;
                this.txtPWD.Text = entry.WWWPWD;
                this.txtDomain.Text = entry.WWWDomain;

                this.txtProxyURL.Text = entry.ProxyURL;
                this.txtProxyUID.Text = entry.ProxyUID;
                this.txtProxyPWD.Text = entry.ProxyPWD;
                this.txtProxyDomain.Text = entry.ProxyDomain;
            }
        }

        /// <summary>履歴のロード</summary>
        private void LoadHistories()
        {
            // histories.bin（履歴）
            //Program.Histories = Program.LoadHistories();
            Program.LoadHistories();

            // 設定
            this.cmbHistorys.Items.Clear();
            if (Program.Histories != null)
            {
                foreach (Entry history in Program.Histories)
                {
                    this.cmbHistorys.Items.Add(history.WWWURL);
                }
            }
            //else
            //{
            //    Program.Histories = new List<Entry>();
            //}
        }

        #endregion

        #endregion

        #region 圧縮・解凍

        /// <summary>圧縮処理</summary>
        private void btnCompress_Click(object sender, EventArgs e)
        {
            try
            {
                // チェック処理
                this.CheckComp_DeComp();

                // 圧縮部品
                Zipper z = new Zipper();

                // 選択基準
                string[] exts = null;
                Zipper.SelectionDelegate scd = null;

                if (this.txtExt.Enabled)
                {
                    exts = this.txtExt.Text.Split(',');
                    scd = Program.SelectionCriteriaDlgt1;
                }

                // 形式指定
                SelfExtractorFlavor? selfEx = null;

                if (this.cmbFormat.Enabled)
                { selfEx = (SelfExtractorFlavor)this.cmbFormat.SelectedItem; }

                // ZIP内パスのルート名
                string[] temp = this.txtFile.Text.Split('\\');

                // 進捗報告処理
                z.SaveProgress = Program.MySaveProgressEventHandler;

                // ルートのディレクトリを作るか作らないか。
                string rootPathInArchive = "";

                if (!this.cbxRootDir.Checked)
                {
                    rootPathInArchive = temp[temp.Length - 1];
                }

                // 圧縮（１）デリゲートでフィルタ
                z.CreateZipFromFolder(
                    this.txtFile.Text, this.txtFolder.Text,
                    scd, exts, rootPathInArchive, // ここを空文字列にするとルートフォルダ無しになる。
                    Encoding.GetEncoding((string)this.cmbEnc.SelectedItem),
                    (EncryptionAlgorithm)this.cmbCyp.SelectedItem, this.txtPass.Text,
                    (CompressionLevel)this.cmbCmpLv.SelectedItem, selfEx);

                //// 圧縮（２）：selectionCriteriaStringでフィルタ
                //string selectionCriteriaString = "";
                //if (exts != null)
                //{
                //    foreach (string ext in exts)
                //    {
                //        if (selectionCriteriaString == "")
                //        {
                //            selectionCriteriaString = "name != *." + ext;
                //        }
                //        else
                //        {
                //            selectionCriteriaString += " and name != *." + ext;
                //        }
                //    }
                //}

                //z.CreateZipFromFolder(
                //    this.txtFile.Text, this.txtFolder.Text,
                //    selectionCriteriaString,
                //    temp[temp.Length - 1],
                //    Encoding.GetEncoding((string)this.cmbEnc.SelectedItem),
                //    (EncryptionAlgorithm)this.cmbCyp.SelectedItem, this.txtPass.Text,
                //    (CompressionLevel)this.cmbCmpLv.SelectedItem, selfEx);

                //MessageBox.Show(z.StatusMSG,"サマリ",
                //    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CustMsgBox custMsgBox = new CustMsgBox(ResourceMgr.GetString("Error0002"), z.StatusMSG, SystemIcons.Information);
                custMsgBox.ShowDialog();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "エラーが発生しました。", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                MessageBox.Show(ex.Message, ResourceMgr.GetString("E0001"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>解凍処理</summary>
        private void btnDecomp_Click(object sender, EventArgs e)
        {
            try
            {
                // チェック処理
                this.CheckComp_DeComp();

                // 解凍部品
                UnZipper uz = new UnZipper();

                // 選択基準
                string[] exts = null;
                Zipper.SelectionDelegate scd = null;

                if (this.txtExt.Enabled)
                {
                    exts = this.txtExt.Text.Split(',');
                    scd = Program.SelectionCriteriaDlgt2;
                }

                // 解凍時、上書き制御
                uz.ExtractProgress = Program.MyExtractProgressEventHandler;

                // 解凍（１）デリゲートでフィルタ
                uz.ExtractFileFromZip(
                    this.txtFile.Text, this.txtFolder.Text, scd, exts,
                    (ExtractExistingFileAction)this.cmbEEFA.SelectedItem,
                    Encoding.GetEncoding((string)this.cmbEnc.SelectedItem),
                    this.txtPass.Text);

                //// 解凍（２）：selectionCriteriaStringでフィルタ
                //string selectionCriteriaString = "";
                //if (exts != null)
                //{
                //    foreach (string ext in exts)
                //    {
                //        if (selectionCriteriaString == "")
                //        {
                //            selectionCriteriaString = "name != *." + ext;
                //        }
                //        else
                //        {
                //            selectionCriteriaString += " and name != *." + ext;
                //        }
                //    }
                //}

                //uz.ExtractFileFromZip(
                //    this.txtFile.Text,
                //    this.txtFolder.Text,
                //    selectionCriteriaString,
                //    (ExtractExistingFileAction)this.cmbEEFA.SelectedItem,
                //    Encoding.GetEncoding((string)this.cmbEnc.SelectedItem),
                //    this.txtPass.Text);

                //MessageBox.Show(uz.StatusMSG, "サマリ",
                //    MessageBoxButtons.OK, MessageBoxIcon.Information);

                //CustMsgBox custMsgBox = new CustMsgBox("サマリ（解凍）", uz.StatusMSG, SystemIcons.Information);
                //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                CustMsgBox custMsgBox = new CustMsgBox(ResourceMgr.GetString("Error0003"), uz.StatusMSG, SystemIcons.Information);
                custMsgBox.ShowDialog();
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message, "エラーが発生しました。", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                MessageBox.Show(ex.Message, ResourceMgr.GetString("E0001"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>チェック処理</summary>
        private void CheckComp_DeComp()
        {
            if (string.IsNullOrEmpty(this.txtFolder.Text))
            {
                //throw new Exception(string.Format(GetMessage.GetMessageDescription("E0007"), "フォルダ パス"));
                //For internationalization, Replaced all the Japanese language exception messages with GetMessage.GetMessageDescription() method call
                throw new Exception(string.Format(GetMessage.GetMessageDescription("E0007"), GetMessage.GetMessageDescription("M0001")));
            }

            if(string.IsNullOrEmpty(this.txtFile.Text))
            {
                //throw new Exception(string.Format(GetMessage.GetMessageDescription("E0007"), "ファイル パス"));
                //For internationalization, Replaced all the Japanese language exception messages with GetMessage.GetMessageDescription() method call
                throw new Exception(string.Format(GetMessage.GetMessageDescription("E0007"), GetMessage.GetMessageDescription("M0002")));
            }
        }

        #region いろいろ選択

        /// <summary>フォルダ選択</summary>
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.ShowDialog();
            this.txtFolder.Text = folderBrowserDialog1.SelectedPath;
        }

        /// <summary>ファイル選択</summary>
        private void btnSelectSaveFile_Click(object sender, EventArgs e)
        {
            //if (this.tabZipUnZip.SelectedTab.Text == "圧縮")
            //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
            if (this.tabZipUnZip.SelectedTab.Text == ResourceMgr.GetString("T0001"))
            {
                // 保存
                this.saveFileDialog1.DefaultExt = "";
                this.saveFileDialog1.ShowDialog();
                this.txtFile.Text = saveFileDialog1.FileName;
            }
            else
            {
                // 開く
                //this.openFileDialog1.Filter = "ZIPファイル|*.zip";
                //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                this.openFileDialog1.Filter = ResourceMgr.GetString("EXT0001");
                
                this.openFileDialog1.ShowDialog();
                this.txtFile.Text = openFileDialog1.FileName;
            }
        }

        /// <summary>拡張子選択</summary>
        private void cbxExt_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked == true)
            {
                this.txtExt.Enabled = true;
            }
            else
            {
                this.txtExt.Enabled = false;
            }
        }

        /// <summary>形式選択</summary>
        private void cmbFormat_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked == true)
            {
                this.cmbFormat.Enabled = true;
            }
            else
            {
                this.cmbFormat.Enabled = false;
            }
        }

        #endregion

        #endregion

        #region マニュフェスト

        /// <summary>ZIPを追加する</summary>
        private void btnAddZIPFile_Click(object sender, EventArgs e)
        {
            this.openFilesDialog1.Reset(); // ごみを消去
            this.openFilesDialog1.ShowDialog();
            string[] zipFiles = this.openFilesDialog1.FileNames;

            foreach (string zipFile in zipFiles)
            {
                // 追加済みフラグ
                bool isAdded = false;

                if (zipFile.Substring(zipFile.Length - 4, 4).ToLower() == ".zip")
                {
                    foreach (string temp in this.lbxZIPFiles.Items)
                    {
                        // 追加済みフラグ
                        if (temp == zipFile)
                        {
                            isAdded = true;
                        }
                    }

                    // 追加済みで無い場合。
                    if (!isAdded)
                    {
                        this.lbxZIPFiles.Items.Add(zipFile);
                    }
                }
            }
        }

        /// <summary>ZIPを削除する</summary>
        private void btnRemoveZIPFile_Click(object sender, EventArgs e)
        {
            this.lbxZIPFiles.Items.Remove(
                        this.lbxZIPFiles.SelectedItem);
        }

        /// <summary>マニュフェストファイルを生成</summary>
        private void btnCreateManifesto_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.lbxZIPFiles.Items.Count ==0)
                {
                    //throw new Exception(string.Format(GetMessage.GetMessageDescription("E0007"), "ZIPファイル"));
                    //For internationalization, Replaced all the Japanese language exception messages with GetMessage.GetMessageDescription() method call
                    throw new Exception(string.Format(GetMessage.GetMessageDescription("E0007"), GetMessage.GetMessageDescription("M0003")));
                }

                if (string.IsNullOrEmpty(this.txtInsDir.Text))
                {
                    //throw new Exception(string.Format(GetMessage.GetMessageDescription("E0007"), "インストール ディレクトリ"));
                    //For internationalization, Replaced all the Japanese language exception messages with GetMessage.GetMessageDescription() method call
                    throw new Exception(string.Format(GetMessage.GetMessageDescription("E0007"), GetMessage.GetMessageDescription("M0004")));
                }

                if (string.IsNullOrEmpty(this.txtExeName.Text))
                {
                    //throw new Exception(string.Format(GetMessage.GetMessageDescription("E0007"), "EXE名（パス）"));
                    //For internationalization, Replaced all the Japanese language exception messages with GetMessage.GetMessageDescription() method call
                    throw new Exception(string.Format(GetMessage.GetMessageDescription("E0007"), GetMessage.GetMessageDescription("M0005")));
                }

                // 保存
                this.saveFileDialog1.Reset(); // ごみを消去
                this.saveFileDialog1.DefaultExt = "mft";
                this.saveFileDialog1.ShowDialog();

                using (StreamWriter sw = new StreamWriter(
                    saveFileDialog1.FileName, false, Encoding.GetEncoding(CustomEncode.UTF_8)))
                {
                    sw.WriteLine("ins " + this.txtInsDir.Text);
                    sw.WriteLine("exe " + this.txtExeName.Text);

                    foreach (string zipFile in this.lbxZIPFiles.Items)
                    {
                        string[] temp = zipFile.Split('\\');

                        sw.WriteLine("zip " + temp[temp.Length - 1]);
                        sw.WriteLine("md5 " + Program.GetMD5Hash(zipFile));
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "エラーが発生しました。", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                MessageBox.Show(ex.Message, ResourceMgr.GetString("E0001"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 更新チェック＆インストール
        
        /// <summary>更新チェック＆インストール</summary>
        private void btnCheckUpdateAndInstall_Click(object sender, EventArgs e)
        {
            // エントリを生成
            Entry entry = new Entry();

            entry.WWWURL = txtURL.Text;
            entry.WWWUID = txtUID.Text;
            entry.WWWPWD = txtPWD.Text;
            entry.WWWDomain = txtDomain.Text;
            //---
            entry.ProxyURL = txtProxyURL.Text;
            entry.ProxyUID = txtProxyUID.Text;
            entry.ProxyPWD = txtProxyPWD.Text;
            entry.ProxyDomain = txtProxyDomain.Text;

            // エントリを保存
            Program.SaveCurrent(entry);

            // 履歴をロード
            this.LoadHistories();

            #region 更新処理の実行

            // 非同期呼び出し
            Program.Af = new MyBaseAsyncFunc(this.panel1);

            // 非同期処理本体デレゲード
            Program.Af.AsyncFunc = new BaseAsyncFunc.AsyncFuncDelegate(Program.ExecUpdate);

            // 進捗報告・無名関数デレゲード
            Program.Af.ChangeProgress = delegate(object param)
            {
                this.Status = ((ChangeProgressParameter)param).Status;
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

                    //MessageBox.Show(message, "メッセージ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                    MessageBox.Show(message, ResourceMgr.GetString("M0001"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                    //CustMsgBox custMsgBox = new CustMsgBox("エラー", message, SystemIcons.Error);
                    //For internationalization, Replaced all the Japanese language to ResourceMgr.GetString() method call
                    CustMsgBox custMsgBox = new CustMsgBox(ResourceMgr.GetString("Error0001"), string.Format(message), SystemIcons.Error);
                    custMsgBox.ShowDialog();
                }
                else
                {
                    // 正常終了

                    // 履歴を保存
                    Program.SaveHistories();

                    // エントリ・履歴の復元する。
                    this.LoadBins();
                }

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
                    Program.OutPutMessage(Program.TempZipFileName + ResourceMgr.GetString("E0002") + ex.ToString(), LogLevel.ErrorLog);
                }
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

        #endregion

        /// <summary>履歴を消す</summary>
        private void btnDelHistory_Click(object sender, EventArgs e)
        {
            // 削除対象エントリ
            Entry entry = null;

            // 履歴を消す
            foreach (Entry temp in Program.Histories)
            {
                // 履歴に一致
                if (this.cmbHistorys.Text == temp.WWWURL)
                {
                    entry = temp;
                }
            }

            if (entry != null)
            {
                // 履歴を更新
                Program.Histories.Remove(entry);
                // 履歴を保存
                Program.SaveHistories();
                
                // 画面を変更

                // コンボボックス
                this.LoadHistories();

                // テキストボックス
                this.txtURL.Text = "";
                this.txtUID.Text = "";
                this.txtPWD.Text = "";
                this.txtDomain.Text = "";

                this.txtProxyURL.Text = "";
                this.txtProxyUID.Text = "";
                this.txtProxyPWD.Text = "";
                this.txtProxyDomain.Text = "";
            }
        }
    }
}

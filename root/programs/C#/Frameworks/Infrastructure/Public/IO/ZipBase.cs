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
//* クラス名        ：ZipBase
//* クラス日本語名  ：DotNetZip部品ベース クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/04/18  西野  大介        新規作成
//*  2012/04/05  西野 大介         \n → \r\n 化
//*  2012/09/21  西野 大介         abstractを追加
//**********************************************************************************

// System
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

// Ionic
using Ionic;
using Ionic.Zip;
using Ionic.Zlib;

using System.Diagnostics;

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>DotNetZip部品ベース クラス</summary>
    /// <see>
    /// DotNetZip Library
    /// http://dotnetzip.codeplex.com/
    /// 
    /// 【ハウツー】C#でZIPファイルを扱えるDotNetZip
    /// http://journal.mycom.co.jp/articles/2009/08/21/DotNetZip/menu.html
    /// 
    /// DoboWiki > .NETプログラミング研究
    /// 
    /// ・DotNetZip（Ionic Zip Library）を使ってZIP書庫を作成する 
    /// 　http://wiki.dobon.net/index.php?.NET%A5%D7%A5%ED%A5%B0%A5%E9%A5%DF%A5%F3%A5%B0%B8%A6%B5%E6%2F93#content_1_0
    /// ・DotNetZip（Ionic Zip Library）を使ってZIP書庫を展開する
    /// 　http://wiki.dobon.net/index.php?.NET%A5%D7%A5%ED%A5%B0%A5%E9%A5%DF%A5%F3%A5%B0%B8%A6%B5%E6%2F94#content_1_0
    /// ・DotNetZip（Ionic Zip Library）を使ってZIP書庫のリスト表示などを行う
    /// 　http://wiki.dobon.net/index.php?.NET%A5%D7%A5%ED%A5%B0%A5%E9%A5%DF%A5%F3%A5%B0%B8%A6%B5%E6%2F95#content_1_0
    /// </see>
    public abstract class ZipBase
    {
        /// <summary>コンストラクタ</summary>
        public ZipBase()
        {
            // 各種イベント ハンドラ
            this._addProgress
                = new EventHandler<AddProgressEventArgs>(this.AddProgressEventHandler);
            this._saveProgress
                = new EventHandler<SaveProgressEventArgs>(this.SaveProgressEventHandler);
            this._readProgress
                = new EventHandler<ReadProgressEventArgs>(this.ReadProgressEventHandler);
            this._extractProgress
                = new EventHandler<ExtractProgressEventArgs>(this.ExtractProgressEventHandler);
            this._zipError
                = new EventHandler<ZipErrorEventArgs>(this.ZipErrorEventHandler);

            // 選択処理を実装するデリゲート
            this._selectionDlgt
                = new SelectionDelegate(this.DefaultSelectionDlgt);
        }

        #region 選択処理

        /// <summary>選択処理を実装するデリゲート</summary>
        /// <param name="o">
        /// 圧縮時：ファイル
        /// 解凍時：エントリ情報
        /// </param>
        /// <param name="info">選択規準情報</param>
        /// <returns>
        /// true：ファイルを圧縮 or 解凍する。
        /// false：ファイルを圧縮 or 解凍しない。
        /// </returns>
        public delegate bool SelectionDelegate(object o, object info);

        /// <summary>選択処理を実装するデリゲート</summary>
        protected SelectionDelegate _selectionDlgt = null;

        /// <summary>選択処理を実装するデリゲート</summary>
        public SelectionDelegate SelectionDlgt
        {
            private set { this._selectionDlgt = value; }
            get { return this._selectionDlgt; }
        }

        /// <summary>選択処理を実装するデリゲート</summary>
        /// <param name="o">
        /// 圧縮時：ファイル
        /// 解凍時：エントリ情報
        /// </param>
        /// <param name="info">選択規準情報</param>
        /// <returns>
        /// true：ファイルを圧縮 or 解凍する。
        /// false：ファイルを圧縮 or 解凍しない。
        /// </returns>
        protected bool DefaultSelectionDlgt(object o, object info)
        {
            return true;
        }

        /// <summary>選択基準情報</summary>
        protected object _selectionCriteriaInfo = null;

        /// <summary>選択基準情報</summary>
        public object SelectionCriteriaInfo
        {
            private set { this._selectionCriteriaInfo = value; }
            get { return this._selectionCriteriaInfo; }
        }

        #endregion 

        #region 各種イベント

        //---

        /// <summary>AddProgressイベント ハンドラ</summary>
        protected EventHandler<AddProgressEventArgs> _addProgress = null;

        /// <summary>AddProgressイベント ハンドラ</summary>
        public EventHandler<AddProgressEventArgs> AddProgress
        {
            set { this._addProgress = value; }
            get { return this._addProgress; }
        }

        /// <summary>AddProgressイベント ハンドラ</summary>
        protected void AddProgressEventHandler(Object sender, AddProgressEventArgs e)
        {
            //Debug.WriteLine("AddProgressEventHandler\r\n"
            //    + "sender:" + sender.ToString() + ", " + "e.ArchiveName:" + e.ArchiveName);
        }

        //---

        /// <summary>SaveProgressイベント ハンドラ</summary>
        protected EventHandler<SaveProgressEventArgs> _saveProgress = null;

        /// <summary>SaveProgressイベント ハンドラ</summary>
        public EventHandler<SaveProgressEventArgs> SaveProgress
        {
            set { this._saveProgress = value; }
            get { return this._saveProgress; }
        }

        /// <summary>SaveProgressイベント ハンドラ</summary>
        protected void SaveProgressEventHandler(Object sender, SaveProgressEventArgs e)
        {
            //Debug.WriteLine("SaveProgressEventHandler\r\n"
            //    + "sender:" + sender.ToString() + ", " + "e.ArchiveName:" + e.ArchiveName);
        }

        //---

        /// <summary>ReadProgressイベント ハンドラ</summary>
        protected EventHandler<ReadProgressEventArgs> _readProgress = null;

        /// <summary>ReadProgressイベント ハンドラ</summary>
        public EventHandler<ReadProgressEventArgs> ReadProgress
        {
            set { this._readProgress = value; }
            get { return this._readProgress; }
        }

        /// <summary>ReadProgressイベント ハンドラ</summary>
        protected void ReadProgressEventHandler(Object sender, ReadProgressEventArgs e)
        {
            //Debug.WriteLine("ReadProgressEventHandler\r\n"
            //    + "sender:" + sender.ToString() + ", " + "e.ArchiveName:" + e.ArchiveName);
        }

        //---

        /// <summary>ExtractProgressイベント ハンドラ</summary>
        protected EventHandler<ExtractProgressEventArgs> _extractProgress = null;

        /// <summary>ExtractProgressイベント ハンドラ</summary>
        public EventHandler<ExtractProgressEventArgs> ExtractProgress
        {
            set { this._extractProgress = value; }
            get { return this._extractProgress; }
        }

        /// <summary>ExtractProgressイベント ハンドラ</summary>
        protected void ExtractProgressEventHandler(Object sender, ExtractProgressEventArgs e)
        {
            //Debug.WriteLine("ExtractProgressEventHandler\r\n"
            //    + "sender:" + sender.ToString() + ", " + "e.ArchiveName:" + e.ArchiveName);
        }

        //---

        /// <summary>ZipErrorイベント ハンドラ</summary>
        protected EventHandler<ZipErrorEventArgs> _zipError = null;

        /// <summary>ZipErrorイベント ハンドラ</summary>
        public EventHandler<ZipErrorEventArgs> ZipError
        {
            set { this._zipError = value; }
            get { return this._zipError; }
        }

        /// <summary>ZipErrorイベント ハンドラ</summary>
        protected void ZipErrorEventHandler(Object sender, ZipErrorEventArgs e)
        {
            //Debug.WriteLine("ZipErrorEventHandler\r\n"
            //    + "sender:" + sender.ToString() + ", " + "e.ArchiveName:" + e.ArchiveName);
        }

        #endregion

        #region 実行状態チェック

        /// <summary>実行状態用ライタ</summary>
        protected StringWriter _statusMSGWriter = null;

        /// <summary>実行状態</summary>
        public string StatusMSG
        {
            get
            {
                // 実行状態を返す。
                return this._statusMSGWriter.ToString();
            }
        }

        #endregion

        #region ZipFile取得

        /// <summary>ZipFileを取得</summary>
        /// <param name="zip">ZipFile</param>
        /// <param name="selfEx">書庫形式（zip形式はnullを指定）</param>
        /// <returns>ZipFile</returns>
        protected ZipFile SetZipFile(ZipFile zip, SelfExtractorFlavor? selfEx)
        {
            // 状態ライタの指定
            this._statusMSGWriter = new StringWriter();
            zip.StatusMessageTextWriter = this._statusMSGWriter;

            // 各種ハンドラ指定
            zip.AddProgress += this._addProgress;
            zip.ExtractProgress += this._extractProgress;
            zip.ReadProgress += this._readProgress;
            zip.SaveProgress += this._saveProgress;
            zip.ZipError += this._zipError;

            // 4G以上のファイルがある時には、ZIP64を使用
            zip.UseZip64WhenSaving = Zip64Option.AsNecessary;

            // 必要に応じてUnicodeを使用
            // （自動解凍書庫の文字化け対策）
            if (selfEx != null)
            {
                zip.UseUnicodeAsNecessary = true;
            }

            // コメント付与
            zip.Comment = "Zipper @ Powered by DotNetZip";

            return zip;
        }

        #endregion
    }
}

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
//* クラス名        ：Zipper
//* クラス日本語名  ：DotNetZipを使用した圧縮クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2011/04/18  西野  大介        新規作成
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

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>DotNetZipを使用した圧縮クラス</summary>
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
    public class Zipper : ZipBase
    {
        #region CreateZipFromFolder（１）

        /// <summary>フォルダ以下を圧縮</summary>
        /// <param name="zipFileToCreate">圧縮ファイル名（zip、exe）</param>
        /// <param name="directoryToZip">圧縮対象フォルダ</param>
        /// <param name="selectionDlgt">ファイル選択デリゲード</param>
        /// <param name="selectionCriteriaInfo">ファイル選択基準情報</param>
        /// <param name="rootPathInArchive">書庫内ルートフォルダ</param>
        /// <param name="enc">エンコーディング</param>
        /// <param name="cyp">暗号化</param>
        /// <param name="zipPassword">パスワード</param>
        /// <param name="cmpLv">圧縮レベル</param>
        /// <param name="selfEx">書庫形式（zip形式はnullを指定）</param>
        public void CreateZipFromFolder(
            string zipFileToCreate,
            string directoryToZip,
            SelectionDelegate selectionDlgt,
            object selectionCriteriaInfo,
            string rootPathInArchive,
            Encoding enc,
            EncryptionAlgorithm cyp,
            string zipPassword,
            CompressionLevel cmpLv,
            SelfExtractorFlavor? selfEx)
        {
            #region ファイル選択基準

            // ファイル選択デリゲード
            if (selectionDlgt != null)
            {
                // 指定のデリゲード
                this._selectionDlgt = selectionDlgt;
                // ファイル選択基準情報
                this._selectionCriteriaInfo = selectionCriteriaInfo;
            }

            #endregion

            // ZipFileを取得
            ZipFile zip = this.GetZipFile(
                enc, cyp, zipPassword, cmpLv, selfEx);

            using (zip) // 使い終ったら「zip.Dispose」する。
            {
                //// ● フォルダのアーカイブ
                //zip.AddDirectory(directoryToZip, rootPathInArchive);

                // ● ファイルを個別に追加する（UOCにて実装）。
                this.CreateZipFromFolderRecursive(
                    zip, directoryToZip, rootPathInArchive);

                if (selfEx == null)
                {
                    // ZIPファイル
                    zip.Save(zipFileToCreate + ".zip");
                }
                else
                {
                    // 自動解凍書庫
                    zip.SaveSelfExtractor(
                        zipFileToCreate + ".exe",
                        (SelfExtractorFlavor)selfEx);
                }
            }
        }

        #region 再帰でフォルダ以下のファイルを手動で追加する。

        /// <summary>フォルダ以下を圧縮（再帰＋ファイル追加）</summary>
        /// <param name="zip">ZipFileクラス インスタンス</param>
        /// <param name="directoryToZip">圧縮対象フォルダ</param>
        /// <param name="directoryPathInArchive">ZIP内パス</param>
        private void CreateZipFromFolderRecursive(
            ZipFile zip,
            string directoryToZip,
            string directoryPathInArchive)
        {
            // ファイルの圧縮処理
            String[] fileNames = Directory.GetFiles(directoryToZip);
            foreach (string fileName in fileNames)
            {
                FileInfo f = new FileInfo(fileName);

                // 解凍対象ファイルを選択（ファイル選択デリゲードを使用）
                if (this.SelectionDlgt(f, this.SelectionCriteriaInfo))
                {
                    //　ファイルを追加する。
                    ZipEntry e = zip.AddFile(
                    Path.Combine(directoryToZip, fileName), directoryPathInArchive);
                }
            }

            // フォルダの圧縮処理
            String[] directoryNames = Directory.GetDirectories(directoryToZip);
            foreach (string directoryName in directoryNames)
            {
                // 追加するZIP内パス
                string[] temp = directoryName.Split('\\');

                // 再帰する・・・。
                CreateZipFromFolderRecursive(zip, directoryName,
                    Path.Combine(directoryPathInArchive, temp[temp.Length - 1]));
            }
        }

        #endregion

        #endregion

        #region CreateZipFromFolder（２）

        /// <summary>フォルダ以下を圧縮</summary>
        /// <param name="zipFileToCreate">圧縮ファイル名（zip、exe）</param>
        /// <param name="directoryToZip">圧縮対象フォルダ</param>
        /// <param name="selectionCriteriaString">ファイル選択基準文字列</param>
        /// <param name="rootPathInArchive">書庫内ルートフォルダ</param>
        /// <param name="enc">エンコーディング</param>
        /// <param name="cyp">暗号化</param>
        /// <param name="zipPassword">パスワード</param>
        /// <param name="cmpLv">圧縮レベル</param>
        /// <param name="selfEx">書庫形式（zip形式はnullを指定）</param>
        public void CreateZipFromFolder(
            string zipFileToCreate,
            string directoryToZip,
            string selectionCriteriaString,
            string rootPathInArchive,
            Encoding enc,
            EncryptionAlgorithm cyp,
            string zipPassword,
            CompressionLevel cmpLv,
            SelfExtractorFlavor? selfEx)
        {
            // ZipFileを取得
            ZipFile zip = this.GetZipFile(
                enc, cyp, zipPassword, cmpLv, selfEx);

            using (zip)
            {
                // フィルタ条件を確認
                if (selectionCriteriaString == null || selectionCriteriaString == "")
                {
                    // ● フォルダのアーカイブ
                    zip.AddDirectory(directoryToZip, rootPathInArchive);
                }
                else
                {
                    // ● フォルダのアーカイブ
                    //    ファイルをフィルタして追加
                    //    selectionCriteriaがファイル選択基準文字列
                    zip.AddSelectedFiles(selectionCriteriaString, directoryToZip, rootPathInArchive, true);
                }

                if (selfEx == null)
                {
                    // ZIPファイル
                    zip.Save(zipFileToCreate + ".zip");
                }
                else
                {
                    // 自動解凍書庫
                    zip.SaveSelfExtractor(
                        zipFileToCreate + ".exe",
                        (SelfExtractorFlavor)selfEx);
                }
            }
        }

        #endregion

        #region 共通関数

        /// <summary>ZipFileを取得</summary>
        /// <param name="enc">エンコーディング</param>
        /// <param name="cyp">暗号化</param>
        /// <param name="zipPassword">パスワード</param>
        /// <param name="cmpLv">圧縮レベル</param>
        /// <param name="selfEx">書庫形式（zip形式はnullを指定）</param>
        /// <returns>ZipFile</returns>
        protected ZipFile GetZipFile(
            Encoding enc,
            EncryptionAlgorithm cyp,
            string zipPassword,
            CompressionLevel cmpLv,
            SelfExtractorFlavor? selfEx)
        {
            // ZipFileの初期化
            ZipFile zip = null;
            if (enc == null) { zip = new ZipFile(); }
            else { zip = new ZipFile(enc); }
            zip = base.SetZipFile(zip, selfEx);

            // 圧縮方法の指定

            // 暗号化
            if (cyp != EncryptionAlgorithm.Unsupported)
            {
                // EncryptionAlgorithm.Unsupportedは設定不可能。
                zip.Encryption = cyp;

                // 解凍パスワード
                if (zipPassword == null || zipPassword == "")
                {
                    // null、空文字なので設定しない。
                    if (cyp != EncryptionAlgorithm.None)
                    { throw new ArgumentException(PublicExceptionMessage.ZIP_PASSWORD, "zipPassword"); }
                }
                else
                {
                    // null、空文字で無いので設定する。
                    zip.Password = zipPassword;
                }
            }

            // 圧縮レベル
            zip.CompressionLevel = cmpLv;

            return zip;
        }

        #endregion
    }
}

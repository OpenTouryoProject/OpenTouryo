//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//* クラス名        ：UnZipper
//* クラス日本語名  ：DotNetZipを使用した解凍クラス
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

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>DotNetZipを使用した解凍クラス</summary>
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
    public class UnZipper : ZipBase
    {
        #region ExtractFileFromZip（１）

        /// <summary>ZIPファイルを解凍</summary>
        /// <param name="zipFileName">ZIPファイル名</param>
        /// <param name="directoryToUnZip">解凍先</param>
        /// <param name="selectionDlgt">ファイル選択デリゲード</param>
        /// <param name="selectionCriteriaInfo">ファイル選択基準情報</param>
        /// <param name="extractExistingFile">上書き時の動作</param>
        /// <param name="enc">エンコーディング</param>
        /// <param name="zipPassword">パスワード</param>
        public void ExtractFileFromZip(
            string zipFileName, string directoryToUnZip,
            SelectionDelegate selectionDlgt,
            object selectionCriteriaInfo,
            ExtractExistingFileAction extractExistingFile,
            Encoding enc, string zipPassword)
        {
            #region ファイル選択

            // ファイル選択デリゲード
            if (selectionCriteriaInfo != null)
            {
                // 指定のデリゲード
                this._selectionDlgt = selectionDlgt;
                // ファイル選択基準情報
                this._selectionCriteriaInfo = selectionCriteriaInfo;
            }

            #endregion

            // ZipFileを取得
            ZipFile zip = this.GetZipFile(zipFileName, enc, zipPassword);

            using (zip) // 使い終ったら「zip.Dispose」する。
            {
                //// ● フォルダのアーカイブ
                //zip.ExtractAll(directoryToUnZip, extractExistingFile);

                // ● ファイルを個別に追加する（UOCにて実装）。
                foreach (ZipEntry ze in zip.Entries)
                {
                    // 圧縮対象ファイルを選択（ファイル選択デリゲードを使用）
                    if (this.SelectionDlgt(ze.FileName, selectionCriteriaInfo))
                    {
                        // デフォルトでは、zipファイルの位置に解凍される。
                        ze.Extract(directoryToUnZip, extractExistingFile);
                    }
                }
            }
        }

        #endregion

        #region ExtractFileFromZip（２）

        /// <summary>ZIPファイルを解凍</summary>
        /// <param name="zipFileName">ZIPファイル名</param>
        /// <param name="directoryToUnZip">解凍先ディレクトリ</param>
        /// <param name="selectionCriteriaString">ファイル選択基準文字列</param>
        /// <param name="extractExistingFile">上書き時の動作</param>
        /// <param name="enc">エンコーディング</param>
        /// <param name="zipPassword">パスワード</param>
        public void ExtractFileFromZip(
            string zipFileName,
            string directoryToUnZip,
            string selectionCriteriaString,
            ExtractExistingFileAction extractExistingFile,
            Encoding enc, string zipPassword)
        {
            // ZipFileを取得
            ZipFile zip = this.GetZipFile(zipFileName, enc, zipPassword);

            using (zip) // 使い終ったら「zip.Dispose」する。
            {
                // フィルタ条件を確認
                if (selectionCriteriaString == null || selectionCriteriaString == "")
                {
                    // ● フォルダのアーカイブ
                    zip.ExtractAll(directoryToUnZip, extractExistingFile);
                }
                else
                {
                    // ● フォルダのアーカイブ
                    //    ファイルをフィルタして解凍
                    //    selectionCriteriaがファイル選択基準文字列
                    zip.ExtractSelectedEntries(selectionCriteriaString,
                        null, // null指定だとルートから。
                        directoryToUnZip, extractExistingFile);
                }
            }
        }

        #endregion

        #region 共通関数

        /// <summary>ZipFileを取得</summary>
        /// <param name="zipFileName">ZIPファイル名</param>
        /// <param name="enc">エンコーディング</param>
        /// <param name="zipPassword">パスワード</param>
        /// <returns>ZipFile</returns>
        private ZipFile GetZipFile(
            string zipFileName,
            Encoding enc,
            string zipPassword)
        {
            // ZipFileの初期化
            ZipFile zip = null;

            if (enc == null) { zip = ZipFile.Read(zipFileName); }
            else { zip = new ZipFile(zipFileName, enc); }
            zip = base.SetZipFile(zip, null);

            // 解凍方法の指定

            // 解凍パスワード
            if (zipPassword == null || zipPassword == "")
            {
                // パスワード無し
            }
            else
            {
                // パスワード有り
                // null、空文字で無いので設定する。
                zip.Password = zipPassword;
            }

            return zip;
        }

        #endregion
    }
}

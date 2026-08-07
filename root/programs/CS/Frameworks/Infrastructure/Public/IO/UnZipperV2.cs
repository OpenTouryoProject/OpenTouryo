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
//* クラス名        ：UnZipperV2
//* クラス日本語名  ：SharpZipLibを使用した解凍クラス
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/08  玄人 幸道         新規作成（#524）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.SharpZipLib.Zip;

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>SharpZipLibを使用した解凍クラス</summary>
    /// <remarks>
    /// 旧 UnZipper（DotNetZip）の代替（#524）。
    /// **引数の並びと名前は旧に合わせてある。** 型だけが自前の列挙型に変わる。
    ///
    /// **StatusMSG は無い。** 旧の呼び出し元（DeployZipPackWithHTTP）は
    /// StatusMSG のログ文言（"extract file ..."）を解析して解凍先の一覧を得ていたが、
    /// DotNetZip 固有の文字列であり再現できない。
    /// **ExtractedFiles で受け取ること。**
    /// </remarks>
    public class UnZipperV2 : ZipBaseV2
    {
        #region ExtractedFiles

        /// <summary>解凍したファイルのパス</summary>
        private List<string> _extractedFiles = new List<string>();

        /// <summary>解凍したファイルのパス</summary>
        /// <remarks>
        /// 旧 StatusMSG の解析で得ていたものの代替。
        /// **ExtractFileFromZip のたびに作り直す**（前回の内容は残らない）。
        /// 上書きせず飛ばしたファイルは含まない。
        /// </remarks>
        public string[] ExtractedFiles
        {
            get { return this._extractedFiles.ToArray(); }
        }

        #endregion

        #region ExtractFileFromZip

        /// <summary>ZIPファイルを解凍</summary>
        /// <param name="zipFileName">ZIPファイル名</param>
        /// <param name="directoryToUnZip">解凍先</param>
        /// <param name="selectionDlgt">ファイル選択デリゲード</param>
        /// <param name="selectionCriteriaInfo">ファイル選択基準情報</param>
        /// <param name="extractExistingFile">上書き時の動作</param>
        /// <param name="enc">エンコーディング</param>
        /// <param name="zipPassword">パスワード</param>
        public void ExtractFileFromZip(
            string zipFileName,
            string directoryToUnZip,
            SelectionDelegate selectionDlgt,
            object selectionCriteriaInfo,
            ExtractExistingFileActionV2 extractExistingFile,
            Encoding enc,
            string zipPassword)
        {
            // ファイル選択基準
            base.SetSelectionCriteria(selectionDlgt, selectionCriteriaInfo);

            this._extractedFiles = new List<string>();

            using (FileStream fs = File.OpenRead(zipFileName))
            using (ZipFile zip = new ZipFile(fs, false, base.GetStringCodec(enc)))
            {
                if (!string.IsNullOrEmpty(zipPassword)) { zip.Password = zipPassword; }

                ZipProgressEventArgsV2 e = new ZipProgressEventArgsV2(
                    ZipProgressEventTypeV2.Extracting_Started, zipFileName);
                e.EntriesTotal = (int)zip.Count;
                base.OnExtractProgress(e);

                int processed = 0;

                foreach (ZipEntry entry in zip)
                {
                    // ディレクトリのエントリは、ファイルの展開時に必要に応じて作る。
                    if (!entry.IsFile) { continue; }

                    // 解凍対象ファイルを選択（ファイル選択デリゲードを使用）
                    if (base.SelectionDlgt(entry.Name, base.SelectionCriteriaInfo))
                    {
                        this.ExtractEntry(zip, entry, zipFileName, directoryToUnZip,
                            extractExistingFile, (int)zip.Count, processed);
                    }

                    processed++;
                }

                e = new ZipProgressEventArgsV2(
                    ZipProgressEventTypeV2.Extracting_Completed, zipFileName);
                e.EntriesTotal = (int)zip.Count;
                e.EntriesProcessed = processed;
                base.OnExtractProgress(e);
            }
        }

        #endregion

        #region private

        /// <summary>1エントリを解凍する</summary>
        /// <param name="zip">ZipFile</param>
        /// <param name="entry">ZipEntry</param>
        /// <param name="zipFileName">書庫のファイル名</param>
        /// <param name="directoryToUnZip">解凍先</param>
        /// <param name="extractExistingFile">上書き時の動作</param>
        /// <param name="entriesTotal">エントリの総数</param>
        /// <param name="entriesProcessed">処理済みのエントリ数</param>
        private void ExtractEntry(
            ZipFile zip,
            ZipEntry entry,
            string zipFileName,
            string directoryToUnZip,
            ExtractExistingFileActionV2 extractExistingFile,
            int entriesTotal,
            int entriesProcessed)
        {
            string path = UnZipperV2.GetSafePath(directoryToUnZip, entry.Name);

            #region 既存ファイルの扱い

            if (File.Exists(path))
            {
                if (extractExistingFile == ExtractExistingFileActionV2.Throw)
                {
                    throw new IOException(path);
                }
                else if (extractExistingFile == ExtractExistingFileActionV2.DoNotOverwrite)
                {
                    ZipProgressEventArgsV2 skip = new ZipProgressEventArgsV2(
                        ZipProgressEventTypeV2.Extracting_ExtractEntryWouldOverwrite, zipFileName);
                    skip.EntriesTotal = entriesTotal;
                    skip.EntriesProcessed = entriesProcessed;
                    skip.CurrentEntryName = entry.Name;
                    base.OnExtractProgress(skip);

                    return;
                }
            }

            #endregion

            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            ZipProgressEventArgsV2 e = new ZipProgressEventArgsV2(
                ZipProgressEventTypeV2.Extracting_BeforeExtractEntry, zipFileName);
            e.EntriesTotal = entriesTotal;
            e.EntriesProcessed = entriesProcessed;
            e.CurrentEntryName = entry.Name;
            e.TotalBytesToTransfer = entry.Size;
            base.OnExtractProgress(e);

            byte[] buffer = new byte[81920];
            long transferred = 0;

            using (Stream src = zip.GetInputStream(entry))
            using (FileStream dst = File.Create(path))
            {
                int read = src.Read(buffer, 0, buffer.Length);

                while (0 < read)
                {
                    dst.Write(buffer, 0, read);
                    transferred += read;

                    e = new ZipProgressEventArgsV2(
                        ZipProgressEventTypeV2.Extracting_EntryBytesWritten, zipFileName);
                    e.EntriesTotal = entriesTotal;
                    e.EntriesProcessed = entriesProcessed;
                    e.CurrentEntryName = entry.Name;
                    e.TotalBytesToTransfer = entry.Size;
                    e.BytesTransferred = transferred;
                    base.OnExtractProgress(e);

                    read = src.Read(buffer, 0, buffer.Length);
                }
            }

            // 更新日時を書庫の値に合わせる。
            File.SetLastWriteTime(path, entry.DateTime);

            this._extractedFiles.Add(path);

            e = new ZipProgressEventArgsV2(
                ZipProgressEventTypeV2.Extracting_AfterExtractEntry, zipFileName);
            e.EntriesTotal = entriesTotal;
            e.EntriesProcessed = entriesProcessed + 1;
            e.CurrentEntryName = entry.Name;
            e.TotalBytesToTransfer = entry.Size;
            e.BytesTransferred = transferred;
            base.OnExtractProgress(e);
        }

        /// <summary>解凍先のパスを求める</summary>
        /// <param name="directoryToUnZip">解凍先</param>
        /// <param name="entryName">エントリ名</param>
        /// <returns>解凍先のパス</returns>
        /// <remarks>
        /// **解凍先の外に出るエントリ名を弾く（Zip Slip 対策）。**
        /// エントリ名は書庫を作った側が自由に決められるため、
        /// "../" を含めて任意の場所へ書かせることができる。
        /// 旧実装が使っていた DotNetZip は、この脆弱性
        /// （GHSA-xhg6-9j5j-w4vf）で非推奨になった。**同じ轍を踏まない。**
        /// </remarks>
        private static string GetSafePath(string directoryToUnZip, string entryName)
        {
            string root = Path.GetFullPath(directoryToUnZip);
            string path = Path.GetFullPath(Path.Combine(root, entryName));

            // 区切り文字を足してから比較する。
            // 足さないと "C:\dir" と "C:\dir2" が前方一致してしまう。
            string prefix = root.EndsWith(Path.DirectorySeparatorChar.ToString())
                ? root : root + Path.DirectorySeparatorChar;

            if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new IOException(entryName);
            }

            return path;
        }

        #endregion
    }
}

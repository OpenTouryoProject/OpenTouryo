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
//* クラス名        ：ZipperV2
//* クラス日本語名  ：SharpZipLibを使用した圧縮クラス
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

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>SharpZipLibを使用した圧縮クラス</summary>
    /// <remarks>
    /// 旧 Zipper（DotNetZip）の代替（#524）。
    /// **引数の並びと名前は旧に合わせてある。** 型だけが自前の列挙型に変わる。
    /// 自己解凍書庫（selfEx）を取るオーバーロードは持たない。
    /// </remarks>
    public class ZipperV2 : ZipBaseV2
    {
        #region CreateZipFromFolder

        /// <summary>フォルダ以下を圧縮</summary>
        /// <param name="zipFileToCreate">圧縮ファイル名（拡張子は付けない。".zip"が付く）</param>
        /// <param name="directoryToZip">圧縮対象フォルダ</param>
        /// <param name="selectionDlgt">ファイル選択デリゲード</param>
        /// <param name="selectionCriteriaInfo">ファイル選択基準情報</param>
        /// <param name="rootPathInArchive">書庫内ルートフォルダ</param>
        /// <param name="enc">エンコーディング</param>
        /// <param name="cyp">暗号化</param>
        /// <param name="zipPassword">パスワード</param>
        /// <param name="cmpLv">圧縮レベル</param>
        public void CreateZipFromFolder(
            string zipFileToCreate,
            string directoryToZip,
            SelectionDelegate selectionDlgt,
            object selectionCriteriaInfo,
            string rootPathInArchive,
            Encoding enc,
            ZipEncryptionAlgorithmV2 cyp,
            string zipPassword,
            ZipCompressionLevelV2 cmpLv)
        {
            // ファイル選択基準
            base.SetSelectionCriteria(selectionDlgt, selectionCriteriaInfo);

            string zipFileName = zipFileToCreate + ".zip";

            // 圧縮対象を先に数え上げる。
            // **進捗の総数を知るために必要。** 逐次追加では総数が分からない。
            List<ZipperV2.Target> targets = new List<ZipperV2.Target>();
            this.CollectRecursive(targets, directoryToZip, rootPathInArchive);

            ZipProgressEventArgsV2 e = new ZipProgressEventArgsV2(
                ZipProgressEventTypeV2.Saving_Started, zipFileName);
            e.EntriesTotal = targets.Count;
            base.OnSaveProgress(e);

            using (FileStream fs = File.Create(zipFileName))
            using (ZipOutputStream zos = new ZipOutputStream(fs, base.GetStringCodec(enc)))
            {
                this.SetZipOutputStream(zos, cyp, zipPassword, cmpLv);
                zos.SetComment(ZipBaseV2.ZipComment);

                int processed = 0;

                foreach (ZipperV2.Target t in targets)
                {
                    this.AddEntry(zos, t, zipFileName, cyp, targets.Count, processed);
                    processed++;
                }

                zos.Finish();
            }

            e = new ZipProgressEventArgsV2(
                ZipProgressEventTypeV2.Saving_Completed, zipFileName);
            e.EntriesTotal = targets.Count;
            e.EntriesProcessed = targets.Count;
            base.OnSaveProgress(e);
        }

        #endregion

        #region private

        /// <summary>圧縮対象</summary>
        private class Target
        {
            /// <summary>実ファイルのパス</summary>
            public string FilePath;

            /// <summary>書庫内のパス</summary>
            public string EntryName;
        }

        /// <summary>圧縮対象を再帰で集める</summary>
        /// <param name="targets">集めた結果</param>
        /// <param name="directoryToZip">圧縮対象フォルダ</param>
        /// <param name="directoryPathInArchive">ZIP内パス</param>
        private void CollectRecursive(
            List<ZipperV2.Target> targets,
            string directoryToZip,
            string directoryPathInArchive)
        {
            // ファイル
            foreach (string fileName in Directory.GetFiles(directoryToZip))
            {
                FileInfo f = new FileInfo(fileName);

                // 圧縮対象ファイルを選択（ファイル選択デリゲードを使用）
                if (base.SelectionDlgt(f, base.SelectionCriteriaInfo))
                {
                    ZipperV2.Target t = new ZipperV2.Target();
                    t.FilePath = f.FullName;
                    t.EntryName = ZipperV2.CombineEntryName(directoryPathInArchive, f.Name);
                    targets.Add(t);
                }
            }

            // フォルダ
            foreach (string directoryName in Directory.GetDirectories(directoryToZip))
            {
                DirectoryInfo d = new DirectoryInfo(directoryName);

                this.CollectRecursive(targets, directoryName,
                    ZipperV2.CombineEntryName(directoryPathInArchive, d.Name));
            }
        }

        /// <summary>書庫内のパスを繋ぐ</summary>
        /// <param name="parent">親</param>
        /// <param name="name">名前</param>
        /// <returns>書庫内のパス</returns>
        /// <remarks>
        /// **区切りは "/" にする。** ZIP の仕様であり、Path.Combine を使うと
        /// Windows では "\" になって、他のツールで開いたときに階層にならない。
        /// </remarks>
        private static string CombineEntryName(string parent, string name)
        {
            if (string.IsNullOrEmpty(parent)) { return name; }
            return parent.Replace('\\', '/').TrimEnd('/') + "/" + name;
        }

        /// <summary>ZipOutputStreamに圧縮方法を設定する</summary>
        /// <param name="zos">ZipOutputStream</param>
        /// <param name="cyp">暗号化</param>
        /// <param name="zipPassword">パスワード</param>
        /// <param name="cmpLv">圧縮レベル</param>
        private void SetZipOutputStream(
            ZipOutputStream zos,
            ZipEncryptionAlgorithmV2 cyp,
            string zipPassword,
            ZipCompressionLevelV2 cmpLv)
        {
            // 4G以上のファイルがある時には、ZIP64を使用
            zos.UseZip64 = UseZip64.Dynamic;

            // 圧縮レベル
            zos.SetLevel((int)cmpLv);

            // 解凍パスワード
            if (string.IsNullOrEmpty(zipPassword))
            {
                // null、空文字なので設定しない。
                if (cyp != ZipEncryptionAlgorithmV2.None)
                { throw new ArgumentException(PublicExceptionMessage.ZIP_PASSWORD, "zipPassword"); }
            }
            else
            {
                if (cyp == ZipEncryptionAlgorithmV2.None)
                {
                    // 暗号化しないので、パスワードは設定しない。
                }
                else
                {
                    zos.Password = zipPassword;
                }
            }
        }

        /// <summary>1エントリを書き込む</summary>
        /// <param name="zos">ZipOutputStream</param>
        /// <param name="t">圧縮対象</param>
        /// <param name="zipFileName">書庫のファイル名</param>
        /// <param name="cyp">暗号化</param>
        /// <param name="entriesTotal">エントリの総数</param>
        /// <param name="entriesProcessed">処理済みのエントリ数</param>
        /// <remarks>
        /// **FastZip を使わない理由。** FastZip の進捗はバイト単位だけで、
        /// エントリの前後が取れない。旧 Zipper の呼び出し元は
        /// Saving_BeforeWriteEntry / Saving_EntryBytesRead / Saving_AfterWriteEntry の
        /// 3 段階で進捗バーを動かしていたため、ZipOutputStream を自前で回す。
        /// </remarks>
        private void AddEntry(
            ZipOutputStream zos,
            ZipperV2.Target t,
            string zipFileName,
            ZipEncryptionAlgorithmV2 cyp,
            int entriesTotal,
            int entriesProcessed)
        {
            FileInfo f = new FileInfo(t.FilePath);

            ZipEntry entry = new ZipEntry(t.EntryName);
            entry.DateTime = f.LastWriteTime;
            entry.Size = f.Length;

            // 暗号化方式
            // AESKeySize は 0 で ZipCrypto（PKZIP 伝統方式）になる。
            switch (cyp)
            {
                case ZipEncryptionAlgorithmV2.WinZipAes128:
                    entry.AESKeySize = 128;
                    break;
                case ZipEncryptionAlgorithmV2.WinZipAes256:
                    entry.AESKeySize = 256;
                    break;
                default:
                    entry.AESKeySize = 0;
                    break;
            }

            ZipProgressEventArgsV2 e = new ZipProgressEventArgsV2(
                ZipProgressEventTypeV2.Saving_BeforeWriteEntry, zipFileName);
            e.EntriesTotal = entriesTotal;
            e.EntriesProcessed = entriesProcessed;
            e.CurrentEntryName = t.EntryName;
            e.TotalBytesToTransfer = f.Length;
            base.OnSaveProgress(e);

            zos.PutNextEntry(entry);

            byte[] buffer = new byte[81920];
            long transferred = 0;

            using (FileStream src = File.OpenRead(t.FilePath))
            {
                int read = src.Read(buffer, 0, buffer.Length);

                while (0 < read)
                {
                    zos.Write(buffer, 0, read);
                    transferred += read;

                    e = new ZipProgressEventArgsV2(
                        ZipProgressEventTypeV2.Saving_EntryBytesRead, zipFileName);
                    e.EntriesTotal = entriesTotal;
                    e.EntriesProcessed = entriesProcessed;
                    e.CurrentEntryName = t.EntryName;
                    e.TotalBytesToTransfer = f.Length;
                    e.BytesTransferred = transferred;
                    base.OnSaveProgress(e);

                    read = src.Read(buffer, 0, buffer.Length);
                }
            }

            zos.CloseEntry();

            e = new ZipProgressEventArgsV2(
                ZipProgressEventTypeV2.Saving_AfterWriteEntry, zipFileName);
            e.EntriesTotal = entriesTotal;
            e.EntriesProcessed = entriesProcessed + 1;
            e.CurrentEntryName = t.EntryName;
            e.TotalBytesToTransfer = f.Length;
            e.BytesTransferred = transferred;
            base.OnSaveProgress(e);
        }

        #endregion
    }
}

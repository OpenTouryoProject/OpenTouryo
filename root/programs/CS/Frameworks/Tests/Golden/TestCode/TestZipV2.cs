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
//* クラス名        ：TestZipV2
//* クラス日本語名  ：ZipperV2・UnZipperV2のテスト
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

using Touryo.Infrastructure.Public.Diagnostics;
using Touryo.Infrastructure.Public.IO;

namespace TestCode
{
    /// <summary>ZipperV2・UnZipperV2のテスト</summary>
    /// <remarks>
    /// SharpZipLib による ZIP 部品の復元（#524）。
    ///
    /// ＜ZIP のバイト列は結果ファイルに載せない＞
    ///   圧縮結果はライブラリの実装と実行日時で変わるため、比較できない。
    ///   **「往復して一致するか」「エントリ名」「例外の型名」だけを記録する。**
    ///
    /// ＜作業フォルダ＞
    ///   実行のたびに一時フォルダを作って、終わったら消す。
    ///   **パスは出力しない**（環境依存になるため）。
    /// </remarks>
    public class TestZipV2
    {
        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            TestZipV2.Run("往復", TestZipV2.TestRoundTrip);

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestZipV2.Run("暗号化", TestZipV2.TestEncryption);

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestZipV2.Run("選択デリゲート", TestZipV2.TestSelectionAndOverwrite);

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestZipV2.Run("進捗イベント", TestZipV2.TestProgressAndSafety);
        }

        #endregion

        #region 実行のヘルパ

        /// <summary>テストの本体</summary>
        private delegate void TestBody();

        /// <summary>1 節を実行する</summary>
        /// <param name="name">節の名前</param>
        /// <param name="body">テストの本体</param>
        /// <remarks>
        /// **例外を必ず捕まえる。** 捕まえないと Program.Main まで抜けて
        /// 以降のテストが動かなくなり、さらに**パス入りのスタック トレースが
        /// 結果ファイルに出る**（環境依存の差分になる）。
        /// </remarks>
        private static void Run(string name, TestBody body)
        {
            try
            {
                body();
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole("[" + name + "] 例外 " + ex.GetType().FullName);
            }
        }

        #endregion

        #region private

        /// <summary>圧縮 → 解凍の往復</summary>
        /// <remarks>
        /// 階層と日本語のファイル名を含める。
        /// **区切りは "/" になること**も見る（ZIP の仕様）。
        /// </remarks>
        private static void TestRoundTrip()
        {
            MyDebug.OutputDebugAndConsole("ZipperV2 → UnZipperV2（往復）");

            string work = TestZipV2.CreateWorkDir();

            try
            {
                string src = TestZipV2.CreateSampleTree(work);
                string dst = Path.Combine(work, "out");

                ZipperV2 z = new ZipperV2();
                z.CreateZipFromFolder(
                    Path.Combine(work, "archive"), src,
                    null, null, "root", null,
                    ZipEncryptionAlgorithmV2.None, null,
                    ZipCompressionLevelV2.Default);

                UnZipperV2 uz = new UnZipperV2();
                uz.ExtractFileFromZip(
                    Path.Combine(work, "archive.zip"), dst,
                    null, null,
                    ExtractExistingFileActionV2.OverwriteSilently, null, null);

                // 解凍したファイルの一覧（旧 StatusMSG の解析の代替）
                MyDebug.OutputDebugAndConsole("解凍した件数 : " + uz.ExtractedFiles.Length);

                foreach (string s in TestZipV2.ToRelativeSorted(uz.ExtractedFiles, dst))
                {
                    MyDebug.OutputDebugAndConsole("  " + s);
                }

                // 内容が一致するか
                MyDebug.OutputDebugAndConsole(
                    "内容が一致 : " + TestZipV2.AreSameTree(src, Path.Combine(dst, "root")));
            }
            finally { TestZipV2.DeleteWorkDir(work); }
        }

        /// <summary>暗号化</summary>
        private static void TestEncryption()
        {
            MyDebug.OutputDebugAndConsole("暗号化");

            TestZipV2.OutputEncryption("暗号化なし", ZipEncryptionAlgorithmV2.None, null, null);
            TestZipV2.OutputEncryption("PkzipWeak", ZipEncryptionAlgorithmV2.PkzipWeak, "pass", "pass");
            TestZipV2.OutputEncryption("WinZipAes128", ZipEncryptionAlgorithmV2.WinZipAes128, "pass", "pass");
            TestZipV2.OutputEncryption("WinZipAes256", ZipEncryptionAlgorithmV2.WinZipAes256, "pass", "pass");

            // パスワードが違う
            TestZipV2.OutputEncryption("パスワード誤り", ZipEncryptionAlgorithmV2.WinZipAes256, "pass", "wrong");

            // 暗号化するのにパスワードが無い
            TestZipV2.OutputEncryption("パスワード未指定", ZipEncryptionAlgorithmV2.WinZipAes256, null, null);
        }

        /// <summary>選択デリゲートと上書きの動作</summary>
        private static void TestSelectionAndOverwrite()
        {
            MyDebug.OutputDebugAndConsole("選択デリゲート");

            string work = TestZipV2.CreateWorkDir();

            try
            {
                string src = TestZipV2.CreateSampleTree(work);
                string dst = Path.Combine(work, "out");

                // 圧縮時に .log を除く
                ZipperV2 z = new ZipperV2();
                z.CreateZipFromFolder(
                    Path.Combine(work, "archive"), src,
                    new ZipBaseV2.SelectionDelegate(TestZipV2.SelectByExtension), ".log",
                    "", null,
                    ZipEncryptionAlgorithmV2.None, null,
                    ZipCompressionLevelV2.Default);

                UnZipperV2 uz = new UnZipperV2();
                uz.ExtractFileFromZip(
                    Path.Combine(work, "archive.zip"), dst,
                    null, null,
                    ExtractExistingFileActionV2.OverwriteSilently, null, null);

                MyDebug.OutputDebugAndConsole("[圧縮時に .log を除く]");
                foreach (string s in TestZipV2.ToRelativeSorted(uz.ExtractedFiles, dst))
                {
                    MyDebug.OutputDebugAndConsole("  " + s);
                }

                #region 上書き時の動作

                MyDebug.OutputDebugAndConsole("[上書き時の動作]");

                // DoNotOverwrite … 飛ばすので ExtractedFiles に載らない
                uz.ExtractFileFromZip(
                    Path.Combine(work, "archive.zip"), dst,
                    null, null,
                    ExtractExistingFileActionV2.DoNotOverwrite, null, null);
                MyDebug.OutputDebugAndConsole("  DoNotOverwrite の解凍件数 : " + uz.ExtractedFiles.Length);

                // OverwriteSilently … 全件やり直す
                uz.ExtractFileFromZip(
                    Path.Combine(work, "archive.zip"), dst,
                    null, null,
                    ExtractExistingFileActionV2.OverwriteSilently, null, null);
                MyDebug.OutputDebugAndConsole("  OverwriteSilently の解凍件数 : " + uz.ExtractedFiles.Length);

                // Throw … 例外
                try
                {
                    uz.ExtractFileFromZip(
                        Path.Combine(work, "archive.zip"), dst,
                        null, null,
                        ExtractExistingFileActionV2.Throw, null, null);
                    MyDebug.OutputDebugAndConsole("  Throw : 例外にならなかった");
                }
                catch (Exception ex)
                {
                    MyDebug.OutputDebugAndConsole("  Throw : 例外 " + ex.GetType().FullName);
                }

                #endregion

                #region 上書きをハンドラに問い合わせる

                // InvokeExtractProgressEvent … 1 件ずつ問い合わせる。
                // DeployZipPackWithHTTP が上書き確認ダイアログに使っている（#528）。
                MyDebug.OutputDebugAndConsole("[上書きの問い合わせ]");

                // (1) 問い合わせに「上書きする」と答える
                int asked = 0;
                uz.ExtractProgress = delegate (object sender, ZipProgressEventArgsV2 e)
                {
                    if (e.EventType == ZipProgressEventTypeV2.Extracting_ExtractEntryWouldOverwrite
                        && e.IsQuery)
                    {
                        asked++;
                        e.ExtractExistingFile = ExtractExistingFileActionV2.OverwriteSilently;
                    }
                };
                uz.ExtractFileFromZip(
                    Path.Combine(work, "archive.zip"), dst,
                    null, null,
                    ExtractExistingFileActionV2.InvokeExtractProgressEvent, null, null);
                MyDebug.OutputDebugAndConsole(
                    "  上書きすると答えた : 問い合わせ " + asked + " 件 / 解凍 " + uz.ExtractedFiles.Length + " 件");

                // (2) 何も答えない … 既定は「上書きしない」（安全側）
                uz.ExtractProgress = null;
                uz.ExtractFileFromZip(
                    Path.Combine(work, "archive.zip"), dst,
                    null, null,
                    ExtractExistingFileActionV2.InvokeExtractProgressEvent, null, null);
                MyDebug.OutputDebugAndConsole(
                    "  答えない（既定）   : 解凍 " + uz.ExtractedFiles.Length + " 件");

                // (3) 1 件目で打ち切る
                uz.ExtractProgress = delegate (object sender, ZipProgressEventArgsV2 e)
                {
                    if (e.EventType == ZipProgressEventTypeV2.Extracting_ExtractEntryWouldOverwrite
                        && e.IsQuery)
                    {
                        e.Cancel = true;
                    }
                };
                uz.ExtractFileFromZip(
                    Path.Combine(work, "archive.zip"), dst,
                    null, null,
                    ExtractExistingFileActionV2.InvokeExtractProgressEvent, null, null);
                MyDebug.OutputDebugAndConsole(
                    "  打ち切る           : 解凍 " + uz.ExtractedFiles.Length + " 件");

                uz.ExtractProgress = null;

                #endregion
            }
            finally { TestZipV2.DeleteWorkDir(work); }
        }

        /// <summary>進捗イベントと安全性</summary>
        private static void TestProgressAndSafety()
        {
            MyDebug.OutputDebugAndConsole("進捗イベント");

            string work = TestZipV2.CreateWorkDir();

            try
            {
                string src = TestZipV2.CreateSampleTree(work);
                string dst = Path.Combine(work, "out");

                // **回数は数えない。** バッファ サイズとファイル サイズで変わるため。
                // 「どの種別が起きたか」だけを記録する。
                List<string> saving = new List<string>();
                List<string> extracting = new List<string>();

                ZipperV2 z = new ZipperV2();
                z.SaveProgress = delegate (object sender, ZipProgressEventArgsV2 e)
                {
                    string s = e.EventType.ToString();
                    if (!saving.Contains(s)) { saving.Add(s); }
                };

                z.CreateZipFromFolder(
                    Path.Combine(work, "archive"), src,
                    null, null, "", null,
                    ZipEncryptionAlgorithmV2.None, null,
                    ZipCompressionLevelV2.Default);

                UnZipperV2 uz = new UnZipperV2();
                uz.ExtractProgress = delegate (object sender, ZipProgressEventArgsV2 e)
                {
                    string s = e.EventType.ToString();
                    if (!extracting.Contains(s)) { extracting.Add(s); }
                };

                uz.ExtractFileFromZip(
                    Path.Combine(work, "archive.zip"), dst,
                    null, null,
                    ExtractExistingFileActionV2.OverwriteSilently, null, null);

                MyDebug.OutputDebugAndConsole("SaveProgress    : " + string.Join(", ", saving.ToArray()));
                MyDebug.OutputDebugAndConsole("ExtractProgress : " + string.Join(", ", extracting.ToArray()));

                #region 圧縮レベル

                // **サイズは出さない。** 実装で変わるため、大小関係だけを見る。
                long none = TestZipV2.GetZipLength(work, src, ZipCompressionLevelV2.None);
                long best = TestZipV2.GetZipLength(work, src, ZipCompressionLevelV2.BestCompression);

                MyDebug.OutputDebugAndConsole(
                    "無圧縮より BestCompression の方が小さい : " + (best < none));

                #endregion
            }
            finally { TestZipV2.DeleteWorkDir(work); }
        }

        #endregion

        #region ヘルパ

        /// <summary>拡張子で選択する</summary>
        /// <param name="o">FileInfo</param>
        /// <param name="info">除く拡張子</param>
        /// <returns>圧縮するか</returns>
        private static bool SelectByExtension(object o, object info)
        {
            FileInfo f = o as FileInfo;
            if (f == null) { return true; }

            return !string.Equals(f.Extension, (string)info, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>暗号化の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="cyp">暗号化</param>
        /// <param name="passOnZip">圧縮時のパスワード</param>
        /// <param name="passOnUnZip">解凍時のパスワード</param>
        private static void OutputEncryption(
            string caseName,
            ZipEncryptionAlgorithmV2 cyp,
            string passOnZip,
            string passOnUnZip)
        {
            string work = TestZipV2.CreateWorkDir();

            try
            {
                string src = TestZipV2.CreateSampleTree(work);
                string dst = Path.Combine(work, "out");

                ZipperV2 z = new ZipperV2();
                z.CreateZipFromFolder(
                    Path.Combine(work, "archive"), src,
                    null, null, "", null, cyp, passOnZip,
                    ZipCompressionLevelV2.Default);

                UnZipperV2 uz = new UnZipperV2();
                uz.ExtractFileFromZip(
                    Path.Combine(work, "archive.zip"), dst,
                    null, null,
                    ExtractExistingFileActionV2.OverwriteSilently, null, passOnUnZip);

                MyDebug.OutputDebugAndConsole(
                    "[" + caseName + "] 往復して一致 : " + TestZipV2.AreSameTree(src, dst));
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole("[" + caseName + "] 例外 " + ex.GetType().FullName);
            }
            finally { TestZipV2.DeleteWorkDir(work); }
        }

        /// <summary>指定の圧縮レベルで圧縮し、サイズを返す</summary>
        /// <param name="work">作業フォルダ</param>
        /// <param name="src">圧縮対象</param>
        /// <param name="cmpLv">圧縮レベル</param>
        /// <returns>書庫のバイト数</returns>
        private static long GetZipLength(string work, string src, ZipCompressionLevelV2 cmpLv)
        {
            string name = Path.Combine(work, "lv" + ((int)cmpLv).ToString());

            ZipperV2 z = new ZipperV2();
            z.CreateZipFromFolder(
                name, src, null, null, "", null,
                ZipEncryptionAlgorithmV2.None, null, cmpLv);

            return new FileInfo(name + ".zip").Length;
        }

        /// <summary>作業フォルダを作る</summary>
        /// <returns>作業フォルダ</returns>
        private static string CreateWorkDir()
        {
            string work = Path.Combine(Path.GetTempPath(), "OpenTouryoZipV2_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(work);
            return work;
        }

        /// <summary>作業フォルダを消す</summary>
        /// <param name="work">作業フォルダ</param>
        private static void DeleteWorkDir(string work)
        {
            try { Directory.Delete(work, true); }
            catch { /* 後始末なので握り潰す。 */ }
        }

        /// <summary>圧縮対象のフォルダを作る</summary>
        /// <param name="work">作業フォルダ</param>
        /// <returns>圧縮対象フォルダ</returns>
        /// <remarks>
        /// 階層・日本語のファイル名・空でない複数ファイルを含める。
        /// **内容は固定**（実行のたびに変わってはならない）。
        /// </remarks>
        private static string CreateSampleTree(string work)
        {
            string src = Path.Combine(work, "src");
            Directory.CreateDirectory(src);
            Directory.CreateDirectory(Path.Combine(src, "sub"));

            // 圧縮が効くように、同じ内容を繰り返す。
            string body = string.Empty;
            for (int i = 0; i < 200; i++) { body += "あいうえお ABCDE 12345\r\n"; }

            File.WriteAllText(Path.Combine(src, "a.txt"), body, Encoding.UTF8);
            File.WriteAllText(Path.Combine(src, "b.log"), body, Encoding.UTF8);
            File.WriteAllText(Path.Combine(src, "日本語.txt"), body, Encoding.UTF8);
            File.WriteAllText(Path.Combine(src, "sub", "c.txt"), body, Encoding.UTF8);

            return src;
        }

        /// <summary>解凍したパスを、比較できる形にして並べ替える</summary>
        /// <param name="paths">解凍したパス</param>
        /// <param name="root">基準フォルダ</param>
        /// <returns>並べ替えた相対パス</returns>
        /// <remarks>
        /// **絶対パスは出力しない**（環境依存になるため）。
        /// 列挙の順序はファイル システム依存なので、**必ず並べ替える。**
        /// </remarks>
        private static string[] ToRelativeSorted(string[] paths, string root)
        {
            List<string> list = new List<string>();

            foreach (string p in paths)
            {
                string r = p.Substring(root.Length).Replace('\\', '/').TrimStart('/');
                list.Add(r);
            }

            list.Sort(StringComparer.Ordinal);
            return list.ToArray();
        }

        /// <summary>2 つのフォルダの内容が同じか</summary>
        /// <param name="dir1">フォルダ1</param>
        /// <param name="dir2">フォルダ2</param>
        /// <returns>同じならtrue</returns>
        private static bool AreSameTree(string dir1, string dir2)
        {
            if (!Directory.Exists(dir1) || !Directory.Exists(dir2)) { return false; }

            Dictionary<string, string> m1 = TestZipV2.ReadAll(dir1);
            Dictionary<string, string> m2 = TestZipV2.ReadAll(dir2);

            if (m1.Count != m2.Count) { return false; }

            foreach (KeyValuePair<string, string> kv in m1)
            {
                if (!m2.ContainsKey(kv.Key)) { return false; }
                if (m2[kv.Key] != kv.Value) { return false; }
            }

            return true;
        }

        /// <summary>フォルダ以下を「相対パス → 内容」で読む</summary>
        /// <param name="dir">フォルダ</param>
        /// <returns>相対パスと内容</returns>
        private static Dictionary<string, string> ReadAll(string dir)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            string root = Path.GetFullPath(dir);

            foreach (string p in Directory.GetFiles(root, "*", SearchOption.AllDirectories))
            {
                string r = Path.GetFullPath(p).Substring(root.Length)
                    .Replace('\\', '/').TrimStart('/');

                map[r] = File.ReadAllText(p, Encoding.UTF8);
            }

            return map;
        }

        #endregion
    }
}

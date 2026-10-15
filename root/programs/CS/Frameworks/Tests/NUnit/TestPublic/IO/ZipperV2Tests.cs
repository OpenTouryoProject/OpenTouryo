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
//* クラス名        ：ZipperV2Tests
//* クラス日本語名  ：ZipperV2 / UnZipperV2 の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.IO;

namespace TestPublic.IO
{
    /// <summary>ZipperV2 / UnZipperV2 の単体テスト</summary>
    /// <remarks>
    /// V2 は SharpZipLib を使う版で、net48 / .NET の両方で動く
    /// （旧版の Zipper / UnZipper は net48 専用）。
    /// </remarks>
    [TestFixture]
    public class ZipperV2Tests
    {
        /// <summary>テストごとの作業フォルダ</summary>
        private string _work;
        private string _src;

        /// <summary>
        /// 作業フォルダに、圧縮対象（日本語名・サブフォルダを含む）を作る。
        ///   src\a.txt / src\sub\日本語.txt
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this._work = Path.Combine(Path.GetTempPath(), "OpenTouryoNUnit", Guid.NewGuid().ToString("N"));
            this._src = Path.Combine(this._work, "src");
            Directory.CreateDirectory(Path.Combine(this._src, "sub"));

            File.WriteAllText(Path.Combine(this._src, "a.txt"), "AAA", Encoding.UTF8);
            File.WriteAllText(Path.Combine(this._src, "sub", "日本語.txt"), "あいう", Encoding.UTF8);
        }

        /// <summary>作業フォルダを消す。</summary>
        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(this._work)) { Directory.Delete(this._work, true); }
        }

        /// <summary>圧縮する（拡張子 .zip は自動で付く）。</summary>
        private string Zip(string password, ZipEncryptionAlgorithmV2 cyp)
        {
            string zipBase = Path.Combine(this._work, "out");

            ZipperV2 z = new ZipperV2();
            z.CreateZipFromFolder(zipBase, this._src, null, null, "",
                Encoding.UTF8, cyp, password, ZipCompressionLevelV2.Default);

            Assert.That(z.ZippedFiles.Length, Is.EqualTo(2));
            return zipBase + ".zip";
        }

        /// <summary>フォルダ構成と日本語のファイル名が往復する。</summary>
        [Test]
        public void RoundTrip()
        {
            string zip = this.Zip("", ZipEncryptionAlgorithmV2.None);
            Assert.That(File.Exists(zip), Is.True);

            string dst = Path.Combine(this._work, "dst");
            UnZipperV2 u = new UnZipperV2();
            u.ExtractFileFromZip(zip, dst, null, null,
                ExtractExistingFileActionV2.OverwriteSilently, Encoding.UTF8, "");

            Assert.That(u.ExtractedFiles.Length, Is.EqualTo(2));
            Assert.That(File.ReadAllText(Path.Combine(dst, "a.txt"), Encoding.UTF8), Is.EqualTo("AAA"));
            Assert.That(File.ReadAllText(Path.Combine(dst, "sub", "日本語.txt"), Encoding.UTF8), Is.EqualTo("あいう"));
        }

        /// <summary>パスワード付き（AES 256）で往復する。パスワードが違えば解凍できない。</summary>
        [Test]
        public void RoundTrip_WithPassword()
        {
            string zip = this.Zip("p@ss", ZipEncryptionAlgorithmV2.WinZipAes256);

            string dst = Path.Combine(this._work, "dst");
            UnZipperV2 u = new UnZipperV2();
            u.ExtractFileFromZip(zip, dst, null, null,
                ExtractExistingFileActionV2.OverwriteSilently, Encoding.UTF8, "p@ss");
            Assert.That(File.ReadAllText(Path.Combine(dst, "a.txt"), Encoding.UTF8), Is.EqualTo("AAA"));

            string dst2 = Path.Combine(this._work, "dst2");
            Assert.Catch<Exception>(() => new UnZipperV2().ExtractFileFromZip(zip, dst2, null, null,
                ExtractExistingFileActionV2.OverwriteSilently, Encoding.UTF8, "wrong"));
        }

        /// <summary>選択デリゲートで、圧縮する対象を絞れる（圧縮では FileInfo が渡る）。</summary>
        [Test]
        public void Selection()
        {
            string zipBase = Path.Combine(this._work, "sel");

            ZipperV2 z = new ZipperV2();
            z.CreateZipFromFolder(zipBase, this._src,
                (o, info) => ((FileInfo)o).Name.EndsWith((string)info), ".txt", "",
                Encoding.UTF8, ZipEncryptionAlgorithmV2.None, "", ZipCompressionLevelV2.Default);
            Assert.That(z.ZippedFiles.Length, Is.EqualTo(2));

            ZipperV2 z2 = new ZipperV2();
            z2.CreateZipFromFolder(zipBase + "2", this._src,
                (o, info) => ((FileInfo)o).Name.EndsWith((string)info), "a.txt", "",
                Encoding.UTF8, ZipEncryptionAlgorithmV2.None, "", ZipCompressionLevelV2.Default);
            Assert.That(z2.ZippedFiles.Length, Is.EqualTo(1));
        }
    }
}

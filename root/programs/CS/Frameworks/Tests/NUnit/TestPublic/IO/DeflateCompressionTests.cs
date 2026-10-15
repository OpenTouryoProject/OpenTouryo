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
//* クラス名        ：DeflateCompressionTests
//* クラス日本語名  ：DeflateCompression の単体テスト
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
    /// <summary>DeflateCompression の単体テスト</summary>
    [TestFixture]
    public class DeflateCompressionTests
    {
        /// <summary>テストごとの作業フォルダ</summary>
        private string _work;

        /// <summary>作業フォルダを作る。</summary>
        [SetUp]
        public void SetUp()
        {
            this._work = Path.Combine(Path.GetTempPath(), "OpenTouryoNUnit", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(this._work);
        }

        /// <summary>作業フォルダを消す。</summary>
        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(this._work)) { Directory.Delete(this._work, true); }
        }

        #region byte[]

        /// <summary>圧縮して伸張すると元に戻る。繰り返しの多いデータは小さくなる。</summary>
        [Test]
        public void Bytes_RoundTrip()
        {
            byte[] input = Encoding.UTF8.GetBytes(new string('あ', 5000));

            byte[] compressed = DeflateCompression.Compress(input);

            Assert.That(compressed.Length, Is.LessThan(input.Length));
            Assert.That(DeflateCompression.Decompress(compressed), Is.EqualTo(input));
        }

        /// <summary>
        /// 1K ずつ処理する実装なので、境界の前後と空を確かめる。
        /// </summary>
        [TestCase(0)]
        [TestCase(1023)]
        [TestCase(1024)]
        [TestCase(1025)]
        [TestCase(10000)]
        public void Bytes_RoundTrip_Boundary(int length)
        {
            byte[] input = new byte[length];
            new Random(length).NextBytes(input);

            Assert.That(DeflateCompression.Decompress(DeflateCompression.Compress(input)), Is.EqualTo(input));
        }

        #endregion

        #region ファイル

        /// <summary>
        /// 出力先を省略すると、入力と同じフォルダに「元の名前.拡張子」で出す
        /// （既定は圧縮 .cmp、伸張 .dcmp）。
        /// </summary>
        [Test]
        public void File_RoundTrip_DefaultOutput()
        {
            string src = Path.Combine(this._work, "data.txt");
            File.WriteAllText(src, "Open棟梁 Deflate テスト\r\n", Encoding.UTF8);

            string cmp = DeflateCompression.Compress(src);
            Assert.That(cmp, Is.EqualTo(Path.Combine(this._work, "data.cmp")));

            string dcmp = DeflateCompression.Decompress(cmp);
            Assert.That(dcmp, Is.EqualTo(Path.Combine(this._work, "data.dcmp")));

            Assert.That(File.ReadAllBytes(dcmp), Is.EqualTo(File.ReadAllBytes(src)));
        }

        /// <summary>出力先に拡張子が無ければ、指定の拡張子を足す。</summary>
        [Test]
        public void File_OutputWithoutExtension_AddsExtension()
        {
            string src = Path.Combine(this._work, "data.txt");
            File.WriteAllText(src, "abc");

            string cmp = DeflateCompression.Compress(src, "z", Path.Combine(this._work, "out"));

            Assert.That(cmp, Is.EqualTo(Path.Combine(this._work, "out.z")));
            Assert.That(File.Exists(cmp), Is.True);
        }

        #endregion
    }
}

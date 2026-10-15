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
//* クラス名        ：CustomEncodeTests
//* クラス日本語名  ：CustomEncode の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

using System;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Str
{
    /// <summary>CustomEncode の単体テスト</summary>
    [TestFixture]
    public class CustomEncodeTests
    {
        #region Base64 / Base64URL

        /// <summary>RFC 4648 10 節のテスト ベクタ（Base64）。</summary>
        [TestCase("", "")]
        [TestCase("f", "Zg==")]
        [TestCase("fo", "Zm8=")]
        [TestCase("foo", "Zm9v")]
        [TestCase("foob", "Zm9vYg==")]
        [TestCase("fooba", "Zm9vYmE=")]
        [TestCase("foobar", "Zm9vYmFy")]
        public void Base64_Rfc4648(string plain, string encoded)
        {
            byte[] bytes = CustomEncode.StringToByte(plain, CustomEncode.UTF_8);

            Assert.That(CustomEncode.ToBase64String(bytes), Is.EqualTo(encoded));
            Assert.That(CustomEncode.FromBase64String(encoded), Is.EqualTo(bytes));
        }

        /// <summary>Base64URL は、パディングを除き、62・63 番目の文字を - と _ にする。</summary>
        [Test]
        public void Base64Url_ReplacesCharsAndStripsPadding()
        {
            // Base64 では "+/8=" になるバイト列
            byte[] bytes = new byte[] { 0xFB, 0xFF };

            Assert.That(CustomEncode.ToBase64String(bytes), Is.EqualTo("+/8="));
            Assert.That(CustomEncode.ToBase64UrlString(bytes), Is.EqualTo("-_8"));
            Assert.That(CustomEncode.FromBase64UrlString("-_8"), Is.EqualTo(bytes));
        }

        /// <summary>パディングの要否（長さ mod 4 が 0 / 2 / 3）によらず往復できる。</summary>
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(31)]
        public void Base64Url_RoundTrip(int length)
        {
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++) { bytes[i] = (byte)(i * 37 + 11); }

            string s = CustomEncode.ToBase64UrlString(bytes);

            Assert.That(s, Does.Not.Contain("=").And.Not.Contain("+").And.Not.Contain("/"));
            Assert.That(CustomEncode.FromBase64UrlString(s), Is.EqualTo(bytes));
        }

        /// <summary>長さ mod 4 が 1 の文字列は Base64URL として不正。</summary>
        [Test]
        public void FromBase64UrlString_IllegalLength_Throws()
        {
            Assert.Catch<Exception>(new TestDelegate(() => CustomEncode.FromBase64UrlString("abcde")));
        }

        #endregion

        #region 16 進

        /// <summary>ToHexString は大文字 2 桁を空白区切りで並べる。FormHexString はその逆。</summary>
        [Test]
        public void HexString_RoundTrip()
        {
            byte[] bytes = new byte[] { 0x00, 0x0A, 0xFF, 0x7F };

            Assert.That(CustomEncode.ToHexString(bytes), Is.EqualTo("00 0A FF 7F"));
            Assert.That(CustomEncode.FormHexString("00 0A FF 7F"), Is.EqualTo(bytes));
        }

        /// <summary>空の配列は空文字列になる。</summary>
        [Test]
        public void ToHexString_Empty()
        {
            Assert.That(CustomEncode.ToHexString(new byte[0]), Is.EqualTo(""));
        }

        #endregion

        #region Web エンコード

        /// <summary>HtmlEncode は &lt; &gt; &amp; &quot; を実体参照にし、HtmlDecode で戻る。</summary>
        [Test]
        public void Html_RoundTrip()
        {
            string input = "<a href=\"x\">&</a>";
            string encoded = CustomEncode.HtmlEncode(input);

            Assert.That(encoded, Is.EqualTo("&lt;a href=&quot;x&quot;&gt;&amp;&lt;/a&gt;"));
            Assert.That(CustomEncode.HtmlDecode(encoded), Is.EqualTo(input));
        }

        /// <summary>UrlEncode は RFC 3986 の非予約文字以外をパーセント エンコードする（UTF-8）。</summary>
        [Test]
        public void Url_RoundTrip()
        {
            string input = "a b&c=d/あ";
            string encoded = CustomEncode.UrlEncode(input);

            Assert.That(encoded, Is.EqualTo("a%20b%26c%3Dd%2F%E3%81%82"));
            Assert.That(CustomEncode.UrlDecode(encoded), Is.EqualTo(input));
        }

        /// <summary>
        /// **UrlDecode は + を空白にする**（WebUtility.UrlDecode の仕様）。
        /// UrlEncode は空白を %20 にするので、往復では問題にならない。
        /// </summary>
        [Test]
        public void UrlDecode_PlusIsSpace()
        {
            Assert.That(CustomEncode.UrlDecode("a+b"), Is.EqualTo("a b"));
        }

        /// <summary>
        /// **UrlEncode2 は TFM で結果が違う。**
        /// net48 は EscapeUriString（/ や ? を残す）、.NET は EscapeDataString（すべて符号化）。
        /// </summary>
        [Test]
        public void UrlEncode2_DependsOnTargetFramework()
        {
#if NETCOREAPP
            Assert.That(CustomEncode.UrlEncode2("a/b?c"), Is.EqualTo("a%2Fb%3Fc"));
#else
            Assert.That(CustomEncode.UrlEncode2("a/b?c"), Is.EqualTo("a/b?c"));
#endif
        }

        #endregion

        #region 文字コード

        /// <summary>UTF-8 での往復。</summary>
        [Test]
        public void StringToByte_Utf8_RoundTrip()
        {
            byte[] bytes = CustomEncode.StringToByte("あ", CustomEncode.UTF_8);

            Assert.That(bytes, Is.EqualTo(new byte[] { 0xE3, 0x81, 0x82 }));
            Assert.That(CustomEncode.ByteToString(bytes, CustomEncode.UTF_8), Is.EqualTo("あ"));
        }

        #endregion
    }
}

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
//* クラス名        ：UriOperatorTests
//* クラス日本語名  ：UriOperator と LogSanitizer の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（#634）
//*  2026/10/22  玄人 幸道         LogSanitizer の名前空間の変更（Public.Log）に追随した
//*  2026/10/22  玄人 幸道         htu の比較のテストを DPoPProofTests へ移し（比較を DPoPProof へ移したため）、
//*                                GetUrlWithoutQueryAndFragment（WithoutQuery を改名）と、
//*                                StringExtractor から移した GetParameterFromQueryString のテストを足した
//*  2026/10/22  玄人 幸道         GetUrlWithoutQueryAndFragment が http / https 以外を空文字にすること（file:// を含む）のケースを足した
//**********************************************************************************

using System;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Str
{
    /// <summary>UriOperator（URL の判定・抽出）と LogSanitizer（ログに出す値の衛生）の単体テスト</summary>
    [TestFixture]
    public class UriOperatorTests
    {
        /// <summary>QueryString の先頭・途中・末尾のパラメタ値を取り出す。無ければ空文字。デコードしない。</summary>
        [TestCase("a", "http://x/y?a=1&b=2&c=3", ExpectedResult = "1")]
        [TestCase("b", "http://x/y?a=1&b=2&c=3", ExpectedResult = "2")]
        [TestCase("c", "http://x/y?a=1&b=2&c=3", ExpectedResult = "3")]
        [TestCase("d", "http://x/y?a=1&b=2&c=3", ExpectedResult = "")]
        [TestCase("code", "https://x/cb?code=abc%2Fdef&state=xyz", ExpectedResult = "abc%2Fdef")]
        public string GetParameterFromQueryString(string paramName, string rawUrl)
        {
            return UriOperator.GetParameterFromQueryString(paramName, rawUrl);
        }

        /// <summary>
        /// クエリと素片を落とす。末尾の "/"・パスの大文字小文字はそのまま。既定のポートは省き、スキームとホストは小文字。
        /// 絶対 URL でなければ空文字。
        /// </summary>
        [TestCase("https://a.example.com/p/?q=1#f", ExpectedResult = "https://a.example.com/p/")]
        [TestCase("https://a.example.com/p#f", ExpectedResult = "https://a.example.com/p")]
        [TestCase("HTTPS://A.EXAMPLE.COM:443/P", ExpectedResult = "https://a.example.com/P")]
        [TestCase("http://a.example.com:8080/p?q", ExpectedResult = "http://a.example.com:8080/p")]
        [TestCase("/p?q", ExpectedResult = "")]
        [TestCase("file:///p?q", ExpectedResult = "")]
        [TestCase("ftp://a.example.com/p?q", ExpectedResult = "")]
        [TestCase("", ExpectedResult = "")]
        [TestCase(null, ExpectedResult = "")]
        public string GetUrlWithoutQueryAndFragment(string url)
        {
            return UriOperator.GetUrlWithoutQueryAndFragment(url);
        }

        /// <summary>https は可。http はループバックだけ可。</summary>
        [TestCase("https://idp.example.com/jwks", true)]
        [TestCase("http://localhost:5000/jwks", true)]
        [TestCase("http://127.0.0.1/jwks", true)]
        [TestCase("http://[::1]/jwks", true)]
        [TestCase("http://idp.example.com/jwks", false)]
        [TestCase("ftp://idp.example.com/jwks", false)]
        [TestCase("not a url", false)]
        [TestCase(null, false)]
        public void IsHttpsOrLoopback(string url, bool expected)
        {
            Assert.That(UriOperator.IsHttpsOrLoopback(url), Is.EqualTo(expected));
        }

        /// <summary>LogSanitizer.Sanitize：改行を落とし、長さを切る。空は "(empty)"。</summary>
        [Test]
        public void LogSanitizer_Sanitize()
        {
            Assert.That(LogSanitizer.Sanitize("a\r\nb\nc"), Is.EqualTo("a  b c"));
            Assert.That(LogSanitizer.Sanitize(new string('x', 70)), Is.EqualTo(new string('x', 64) + "..."));
            Assert.That(LogSanitizer.Sanitize(new string('x', 64)), Is.EqualTo(new string('x', 64)));
            Assert.That(LogSanitizer.Sanitize("abcdef", 3), Is.EqualTo("abc..."));
            Assert.That(LogSanitizer.Sanitize(null), Is.EqualTo("(empty)"));
            Assert.That(LogSanitizer.Sanitize(""), Is.EqualTo("(empty)"));
        }

        /// <summary>LogSanitizer.Describe：値を出さず、空か長さだけを出す。</summary>
        [Test]
        public void LogSanitizer_Describe()
        {
            Assert.That(LogSanitizer.Describe("secret-state"), Is.EqualTo("len=12"));
            Assert.That(LogSanitizer.Describe(null), Is.EqualTo("(empty)"));
        }
    }
}

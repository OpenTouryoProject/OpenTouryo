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
//**********************************************************************************

using System;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Str
{
    /// <summary>UriOperator（URL の比較・判定）と LogSanitizer（ログに出す値の衛生）の単体テスト</summary>
    [TestFixture]
    public class UriOperatorTests
    {
        /// <summary>クエリ・素片・末尾の "/"・大文字小文字・既定のポートの違いは同じとみなす。</summary>
        [TestCase("https://as.example.com/token", "https://as.example.com/token")]
        [TestCase("https://as.example.com/token?x=1#f", "https://as.example.com/token")]
        [TestCase("https://as.example.com/token/", "https://as.example.com/token")]
        [TestCase("HTTPS://AS.example.com/Token", "https://as.example.com/token")]
        [TestCase("https://as.example.com:443/token", "https://as.example.com/token")]
        public void IsSameWithoutQuery_Same(string actual, string expected)
        {
            Assert.That(UriOperator.IsSameWithoutQuery(actual, expected), Is.True);
        }

        /// <summary>パス・ホスト・ポート・スキームが違えば別。相対 URL・空は false。</summary>
        [TestCase("https://as.example.com/token2", "https://as.example.com/token")]
        [TestCase("https://evil.example.com/token", "https://as.example.com/token")]
        [TestCase("https://as.example.com:8443/token", "https://as.example.com/token")]
        [TestCase("http://as.example.com/token", "https://as.example.com/token")]
        [TestCase("/token", "https://as.example.com/token")]
        [TestCase("", "https://as.example.com/token")]
        [TestCase(null, "https://as.example.com/token")]
        public void IsSameWithoutQuery_Different(string actual, string expected)
        {
            Assert.That(UriOperator.IsSameWithoutQuery(actual, expected), Is.False);
        }

        /// <summary>WithoutQuery：null は ArgumentNullException。</summary>
        [Test]
        public void WithoutQuery()
        {
            Assert.That(UriOperator.WithoutQuery(new Uri("https://a.example.com/p/?q#f")), Is.EqualTo("https://a.example.com/p"));
            Assert.Throws<ArgumentNullException>(() => UriOperator.WithoutQuery(null));
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

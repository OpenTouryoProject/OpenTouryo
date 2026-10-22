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
//* クラス名        ：StringExtractorTests
//* クラス日本語名  ：StringExtractor の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/16  玄人 幸道         新規作成（カバレッジの拡充）
//*  2026/10/22  玄人 幸道         GetParameterFromQueryString を UriOperator へ移した（#634）。
//*                                ここでは、転送だけ残したメソッドが移動先と同じ結果を返すことを確かめる
//**********************************************************************************

using NUnit.Framework;

using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Str
{
    /// <summary>StringExtractor の単体テスト</summary>
    [TestFixture]
    public class StringExtractorTests
    {
#pragma warning disable CS0618 // 互換のために残した転送用メソッドを、あえて呼ぶ。
        /// <summary>UriOperator へ移したメソッドは、転送するだけ（#634）。結果は移動先と同じ。</summary>
        [TestCase("a", "http://x/y?a=1&b=2&c=3")]
        [TestCase("c", "http://x/y?a=1&b=2&c=3")]
        [TestCase("d", "http://x/y?a=1&b=2&c=3")]
        public void ObsoleteForwarder_SameAsUriOperator(string paramName, string rawUrl)
        {
            Assert.That(StringExtractor.GetParameterFromQueryString(paramName, rawUrl),
                Is.EqualTo(UriOperator.GetParameterFromQueryString(paramName, rawUrl)));
        }
#pragma warning restore CS0618

        /// <summary>XML 文字列から、最初に現れた属性の値を取り出す。</summary>
        [Test]
        public void GetAttributeFromXml()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"shift_jis\"?><a b=\"1\"><c b=\"2\"/></a>";

            Assert.That(StringExtractor.GetAttributeFromXml(xml, "encoding"), Is.EqualTo("shift_jis"));
            Assert.That(StringExtractor.GetAttributeFromXml(xml, "b"), Is.EqualTo("1"));
        }
    }
}

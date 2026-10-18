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
//* クラス名        ：JIS2k4CheckerTests
//* クラス日本語名  ：JIS2k4Checker の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/16  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using NUnit.Framework;

using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Str
{
    /// <summary>JIS2k4Checker の単体テスト</summary>
    /// <remarks>
    /// 対象は 2 種類。
    ///   ・４バイト文字（サロゲート ペア文字）… 例: 𠮷（U+20BB7）
    ///   ・JIS2004 で追加された文字（BMP 内）  … 例: 俱（U+4FF1）、€（U+20AC）
    /// </remarks>
    [TestFixture]
    public class JIS2k4CheckerTests
    {
        /// <summary>𠮷（つちよし。U+20BB7、サロゲート ペア）</summary>
        private const string Surrogate = "𠮷";

        /// <summary>俱（U+4FF1、JIS2004 追加文字）</summary>
        private const string Added = "俱";

        private JIS2k4Checker _checker = null;

        /// <summary>前処理</summary>
        [SetUp]
        public void SetUp()
        {
            this._checker = new JIS2k4Checker();
        }

        #region プロパティ・文字列情報

        /// <summary>追加文字の配列と文字列は同じ内容（コンストラクタで配列から作る）。</summary>
        [Test]
        public void JIS2k4Chars_And_String_AreSame()
        {
            Assert.That(this._checker.JIS2k4Chars.Length, Is.GreaterThan(0));
            Assert.That(this._checker.JIS2k4String, Is.EqualTo(new string(this._checker.JIS2k4Chars)));
            Assert.That(this._checker.JIS2k4String, Does.Contain(Added));
        }

        /// <summary>Length はサロゲート ペアを 2 文字、LengthInTextElements は 1 文字と数える。</summary>
        [Test]
        public void GetStringInfo()
        {
            int sLength, siLength, byteLength;
            this._checker.GetStringInfo("a" + Surrogate, out sLength, out siLength, out byteLength);

            Assert.That(sLength, Is.EqualTo(3));
            Assert.That(siLength, Is.EqualTo(2));
            Assert.That(byteLength, Is.EqualTo(6)); // UTF-16
        }

        #endregion

        #region サロゲート ペア文字

        /// <summary>サロゲート ペア文字の有無と、最初の位置。</summary>
        [TestCase("abc", false, -1)]
        [TestCase("ab" + Surrogate + "c", true, 2)]
        [TestCase("", false, -1)]
        public void CheckSurrogatesPairChar(string input, bool expected, int expectedIndex)
        {
            int index;

            Assert.That(this._checker.CheckSurrogatesPairChar(input), Is.EqualTo(expected));
            Assert.That(this._checker.CheckSurrogatesPairChar(input, out index), Is.EqualTo(expected));
            Assert.That(index, Is.EqualTo(expectedIndex));
        }

        /// <summary>**ペア 1 組を 1 つの置換文字にする**（上位・下位の 2 つにはしない）。</summary>
        [Test]
        public void DeleteSurrogatesPairChar()
        {
            string input = "a" + Surrogate + "b";

            Assert.That(this._checker.DeleteSurrogatesPairChar(input), Is.EqualTo("ab"));
            Assert.That(this._checker.DeleteSurrogatesPairChar(input, '?'), Is.EqualTo("a?b"));
            Assert.That(this._checker.DeleteSurrogatesPairChar(input, "[x]"), Is.EqualTo("a[x]b"));
        }

        /// <summary>ペアが連続しても、組ごとに 1 つずつ置換する。</summary>
        [TestCase(2, "a??b")]
        [TestCase(3, "a???b")]
        public void DeleteSurrogatesPairChar_Consecutive(int count, string expected)
        {
            string input = "a";
            for (int i = 0; i < count; i++) { input += Surrogate; }
            input += "b";

            Assert.That(this._checker.DeleteSurrogatesPairChar(input, '?'), Is.EqualTo(expected));
        }

        #endregion

        #region JIS2004 追加文字

        /// <summary>
        /// 追加文字の有無と位置。**サロゲート ペア文字も「含まれる」と判定し、前にある方の位置を返す。**
        /// </summary>
        [TestCase("abc", false, -1)]
        [TestCase("ab" + Added, true, 2)]
        [TestCase("a" + Surrogate, true, 1)]                 // サロゲート ペアだけ
        [TestCase(Added + "a" + Surrogate, true, 0)]         // 追加文字が先
        [TestCase("a" + Surrogate + Added, true, 1)]         // サロゲート ペアが先
        public void CheckCharAddedWithJIS2k4(string input, bool expected, int expectedIndex)
        {
            int index;

            Assert.That(this._checker.CheckCharAddedWithJIS2k4(input), Is.EqualTo(expected));
            Assert.That(this._checker.CheckCharAddedWithJIS2k4(input, out index), Is.EqualTo(expected));
            Assert.That(index, Is.EqualTo(expectedIndex));
        }

        /// <summary>追加文字とサロゲート ペア文字の両方を削除・置換する。</summary>
        [Test]
        public void DeleteCharAddedWithJIS2k4()
        {
            string input = "a" + Added + "b" + Surrogate + "c";

            Assert.That(this._checker.DeleteCharAddedWithJIS2k4(input), Is.EqualTo("abc"));
            Assert.That(this._checker.DeleteCharAddedWithJIS2k4(input, '?'), Is.EqualTo("a?b?c"));
            Assert.That(this._checker.DeleteCharAddedWithJIS2k4(input, "__"), Is.EqualTo("a__b__c"));
        }

        #endregion
    }
}

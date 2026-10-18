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
//* クラス名        ：StringCheckerTests
//* クラス日本語名  ：StringChecker の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//*  2026/10/16  玄人 幸道         コード ページ（IsInCodePage・IsShift_Jis 系）のケースを追加（カバレッジの拡充）
//**********************************************************************************

using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Str
{
    /// <summary>StringChecker の単体テスト</summary>
    [TestFixture]
    public class StringCheckerTests
    {
        #region 数値チェック

        /// <summary>IsNumbers は、すべての文字が数字かを見る（全角も数字）。</summary>
        [TestCase("0123456789", ExpectedResult = true)]
        [TestCase("０１２３", ExpectedResult = true)]
        [TestCase("12a", ExpectedResult = false)]
        [TestCase("-1", ExpectedResult = false)]
        [TestCase("1.5", ExpectedResult = false)]
        public bool IsNumbers(string input)
        {
            return StringChecker.IsNumbers(input);
        }

        /// <summary>IsNumbers_Hankaku / _Zenkaku は、半角・全角を区別する。</summary>
        [TestCase("123", true, false)]
        [TestCase("１２３", false, true)]
        [TestCase("1２3", false, false)]
        public void IsNumbers_HankakuZenkaku(string input, bool hankaku, bool zenkaku)
        {
            Assert.That(StringChecker.IsNumbers_Hankaku(input), Is.EqualTo(hankaku));
            Assert.That(StringChecker.IsNumbers_Zenkaku(input), Is.EqualTo(zenkaku));
        }

        /// <summary>IsNumeric は、数値として評価できるかを見る（全角は半角化してから）。</summary>
        [TestCase("123", ExpectedResult = true)]
        [TestCase("-1", ExpectedResult = true)]
        [TestCase("1.5", ExpectedResult = true)]
        [TestCase("１２３", ExpectedResult = true)]
        [TestCase("abc", ExpectedResult = false)]
        public bool IsNumeric(string input)
        {
            return StringChecker.IsNumeric(input);
        }

        /// <summary>
        /// **空文字列の扱いが IsNumbers と IsNumeric で逆になる。**
        /// 前者は「数字以外が無い」ので true、後者は「数値として評価できない」ので false。
        /// </summary>
        [Test]
        public void EmptyString_IsNumbersAndIsNumericDiffer()
        {
            Assert.That(StringChecker.IsNumbers(""), Is.True);
            Assert.That(StringChecker.IsNumeric(""), Is.False);
        }

        #endregion

        #region 全角半角

        /// <summary>半角カナは半角に含まれる。</summary>
        [TestCase("abc 123 ~", true, false)]
        [TestCase("ｱｲｳﾞ", true, false)]
        [TestCase("あいう漢字", false, true)]
        [TestCase("ＡＢＣ", false, true)]
        [TestCase("aあ", false, false)]
        [TestCase("", true, true)]
        public void IsHankakuZenkaku(string input, bool hankaku, bool zenkaku)
        {
            Assert.That(StringChecker.IsHankaku(input), Is.EqualTo(hankaku));
            Assert.That(StringChecker.IsZenkaku(input), Is.EqualTo(zenkaku));
        }

        #endregion

        #region 英字チェック

        /// <summary>IsAlphabet は全角・半角の両方を受け付ける。</summary>
        [TestCase("abcXYZ", true, true, false)]
        [TestCase("ａｂｃＸＹＺ", true, false, true)]
        [TestCase("aＢ", true, false, false)]
        [TestCase("a1", false, false, false)]
        public void IsAlphabet(string input, bool any, bool hankaku, bool zenkaku)
        {
            Assert.That(StringChecker.IsAlphabet(input), Is.EqualTo(any));
            Assert.That(StringChecker.IsAlphabet_Hankaku(input), Is.EqualTo(hankaku));
            Assert.That(StringChecker.IsAlphabet_Zenkaku(input), Is.EqualTo(zenkaku));
        }

        #endregion

        #region 日本語チェック

        /// <summary>平仮名・片仮名（全角・半角）・漢字の判定。</summary>
        [TestCase("あいうえお", true, false, false, false, false)]
        [TestCase("アイウエオ", false, true, true, false, false)]
        [TestCase("ｱｲｳｴｵ", false, true, false, true, false)]
        [TestCase("漢字", false, false, false, false, true)]
        [TestCase("あア", false, false, false, false, false)]
        public void Japanese(string input,
            bool hiragana, bool katakana, bool katakanaZenkaku, bool katakanaHankaku, bool kanji)
        {
            Assert.That(StringChecker.IsHiragana(input), Is.EqualTo(hiragana), "IsHiragana");
            Assert.That(StringChecker.IsKatakana(input), Is.EqualTo(katakana), "IsKatakana");
            Assert.That(StringChecker.IsKatakana_Zenkaku(input), Is.EqualTo(katakanaZenkaku), "IsKatakana_Zenkaku");
            Assert.That(StringChecker.IsKatakana_Hankaku(input), Is.EqualTo(katakanaHankaku), "IsKatakana_Hankaku");
            Assert.That(StringChecker.IsKanji(input), Is.EqualTo(kanji), "IsKanji");
        }

        /// <summary>
        /// **全角と半角が混じった片仮名は IsKatakana にならない。**
        /// IsKatakana は「全部が全角」か「全部が半角」かの論理和であるため。
        /// </summary>
        [Test]
        public void IsKatakana_MixedWidthIsFalse()
        {
            Assert.That(StringChecker.IsKatakana("アｲ"), Is.False);
        }

        #endregion

        #region 正規表現

        /// <summary>Match / Matches は Regex の薄いラッパ。</summary>
        [Test]
        public void MatchAndMatches()
        {
            Assert.That(StringChecker.Match("abc123", "^[a-z]+\\d+$"), Is.True);
            Assert.That(StringChecker.Match("ABC", "^[a-z]+$"), Is.False);
            Assert.That(StringChecker.Match("ABC", "^[a-z]+$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase), Is.True);
            Assert.That(StringChecker.Matches("a1b22c333", "\\d+").Count, Is.EqualTo(3));
        }

        #endregion

        #region コード ページ

        /// <summary>😀（U+1F600、サロゲート ペア）。Shift_JIS には無い。</summary>
        private const string Emoji = "😀";

        /// <summary>
        /// **.NET では、Shift_JIS を使う前に CodePagesEncodingProvider の登録が要る。**
        /// 登録しないと Encoding.GetEncoding(932) が NotSupportedException になる。
        /// 登録は利用側（アプリ）の責務で、フレームワークは行わない。net48 は標準で使える。
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
#if NETCOREAPP
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        /// <summary>
        /// IsInCodePage は、そのコード ページで**往復しても変わらない**かを見る。空文字は true。
        /// </summary>
        [TestCase("abc", CustomEncode.us_ascii, ExpectedResult = true)]
        [TestCase("あ", CustomEncode.us_ascii, ExpectedResult = false)]
        [TestCase("あ亜ｱ", CustomEncode.shift_jis, ExpectedResult = true)]
        [TestCase(Emoji, CustomEncode.shift_jis, ExpectedResult = false)]
        [TestCase(Emoji, CustomEncode.UTF_8, ExpectedResult = true)]
        [TestCase("", CustomEncode.us_ascii, ExpectedResult = true)]
        public bool IsInCodePage(string input, int codePageNum)
        {
            return StringChecker.IsInCodePage(input, codePageNum);
        }

        /// <summary>
        /// IsShift_Jis（Shift_JIS の文字か）、_Zenkaku（2 バイト文字だけか）、_Hankaku（1 バイト文字だけか）。
        /// **半角カナ（ｱ）は 1 バイトなので半角。** Shift_JIS に無い文字は、どれも false。
        /// </summary>
        [TestCase("あア亜", true, true, false)]
        [TestCase("ｱa1", true, false, true)]
        [TestCase("aあ", true, false, false)]
        [TestCase("a" + Emoji, false, false, false)]
        [TestCase("", true, true, true)]
        public void IsShift_Jis(string input, bool sjis, bool zenkaku, bool hankaku)
        {
            Assert.That(StringChecker.IsShift_Jis(input), Is.EqualTo(sjis), "IsShift_Jis");
            Assert.That(StringChecker.IsShift_Jis_Zenkaku(input), Is.EqualTo(zenkaku), "IsShift_Jis_Zenkaku");
            Assert.That(StringChecker.IsShift_Jis_Hankaku(input), Is.EqualTo(hankaku), "IsShift_Jis_Hankaku");
        }

        #endregion
    }
}

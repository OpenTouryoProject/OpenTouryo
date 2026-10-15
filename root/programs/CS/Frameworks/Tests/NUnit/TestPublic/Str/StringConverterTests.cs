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
//* クラス名        ：StringConverterTests
//* クラス日本語名  ：StringConverter の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

using NUnit.Framework;

using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Str
{
    /// <summary>StringConverter の単体テスト</summary>
    /// <remarks>
    /// **全角・半角、かな変換は TFM で実装が違う。**
    /// net48 は VB の StrConv、.NET は Zipangu。
    /// ここでは両方で同じになる範囲（英数字・記号、平仮名⇔片仮名）を確かめる。
    /// </remarks>
    [TestFixture]
    public class StringConverterTests
    {
        #region 全角 / 半角

        /// <summary>英数字・記号の全角化と半角化。</summary>
        [TestCase("abcXYZ019", "ａｂｃＸＹＺ０１９")]
        [TestCase("A-B (C)", "Ａ－Ｂ　（Ｃ）")]
        public void ZenkakuHankaku_Ascii(string hankaku, string zenkaku)
        {
            Assert.That(StringConverter.ToZenkaku(hankaku), Is.EqualTo(zenkaku), "ToZenkaku");
            Assert.That(StringConverter.ToHankaku(zenkaku), Is.EqualTo(hankaku), "ToHankaku");
        }

        /// <summary>漢字・平仮名は変換の対象外。</summary>
        [Test]
        public void ToHankaku_LeavesKanjiAndHiragana()
        {
            Assert.That(StringConverter.ToHankaku("漢字ひらがな１"), Is.EqualTo("漢字ひらがな1"));
        }

        #endregion

        #region 平仮名 / 片仮名

        /// <summary>平仮名 ⇔ 片仮名（全角）。</summary>
        [TestCase("あいうえお", "アイウエオ")]
        [TestCase("がぱっ", "ガパッ")]
        public void HiraganaKatakana(string hiragana, string katakana)
        {
            Assert.That(StringConverter.ToKatakana(hiragana), Is.EqualTo(katakana), "ToKatakana");
            Assert.That(StringConverter.ToHiragana(katakana), Is.EqualTo(hiragana), "ToHiragana");
        }

        #endregion

        #region 入力補完

        /// <summary>
        /// 7 桁・6 桁の年月日を 8 桁に補う。
        /// 7 桁は 5・6 桁目が 13 以上なら「月が 1 桁」、そうでなければ「日が 1 桁」とみなす。
        /// </summary>
        [TestCase("2026123", "20261203")]
        [TestCase("2026131", "20260131")]
        [TestCase("202613", "20260103")]
        public void EditYYYYMMDDString_Completes(string input, string expected)
        {
            string text = input;

            Assert.That(StringConverter.EditYYYYMMDDString(ref text), Is.True);
            Assert.That(text, Is.EqualTo(expected));
        }

        /// <summary>8 桁や数字以外は、補わずに false を返し、値も変えない。</summary>
        [TestCase("20261015")]
        [TestCase("2026/1/5")]
        [TestCase("１２３４５６７")]
        public void EditYYYYMMDDString_NotTarget(string input)
        {
            string text = input;

            Assert.That(StringConverter.EditYYYYMMDDString(ref text), Is.False);
            Assert.That(text, Is.EqualTo(input));
        }

        #endregion

        #region 1 行ログ

        /// <summary>
        /// 改行を空白にし、連続する空白・タブを 1 つに詰める。
        /// **引用符（'）の内側は詰めない**（SQL のリテラルを壊さないため）。
        /// </summary>
        [Test]
        public void FormattingForOneLineLog()
        {
            string sql = "SELECT  *\r\nFROM\tT\n WHERE A = '  x  ' AND B = 'it''s'";

            Assert.That(StringConverter.FormattingForOneLineLog(sql),
                Is.EqualTo("SELECT * FROM\tT WHERE A = '  x  ' AND B = 'it''s'"));
        }

        #endregion
    }
}

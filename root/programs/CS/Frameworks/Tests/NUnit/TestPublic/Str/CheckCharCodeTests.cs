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
//* クラス名        ：CheckCharCodeTests
//* クラス日本語名  ：CheckCharCode の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/16  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Str
{
    /// <summary>CheckCharCode の単体テスト</summary>
    /// <remarks>
    /// 文字をその符号化のバイト列にし、**先頭のバイトを上位とする数値**で範囲を比べる。
    /// </remarks>
    [TestFixture]
    public class CheckCharCodeTests
    {
        /// <summary>1 バイトの範囲（ASCII の A〜Z）。両端を含む。</summary>
        [TestCase("A", ExpectedResult = true)]
        [TestCase("M", ExpectedResult = true)]
        [TestCase("Z", ExpectedResult = true)]
        [TestCase("a", ExpectedResult = false)]
        [TestCase("@", ExpectedResult = false)]
        public bool IsInRange_Ascii(string ch)
        {
            return new CheckCharCode("A", "Z", Encoding.ASCII).IsInRange(ch);
        }

        /// <summary>複数バイトの範囲（UTF-8 のひらがな ぁ〜ん。E3 81 81 〜 E3 82 93）。</summary>
        [TestCase("あ", ExpectedResult = true)]    // E3 81 82
        [TestCase("ん", ExpectedResult = true)]    // E3 82 93
        [TestCase("ア", ExpectedResult = false)]   // E3 82 A2（カタカナ）
        [TestCase("a", ExpectedResult = false)]    // 61（1 バイト）
        public bool IsInRange_Utf8(string ch)
        {
            return new CheckCharCode("ぁ", "ん", Encoding.UTF8).IsInRange(ch);
        }

        /// <summary>
        /// **符号化によって範囲が変わる。** 同じ ぁ〜ん でも、UTF-16（ビッグ エンディアン）なら
        /// U+3041〜U+3093 の比較になり、カタカナ（U+30A2）は外れ、ゐ（U+3090）は入る。
        /// </summary>
        [TestCase("ゐ", ExpectedResult = true)]
        [TestCase("ア", ExpectedResult = false)]
        public bool IsInRange_Utf16BigEndian(string ch)
        {
            return new CheckCharCode("ぁ", "ん", Encoding.BigEndianUnicode).IsInRange(ch);
        }
    }
}

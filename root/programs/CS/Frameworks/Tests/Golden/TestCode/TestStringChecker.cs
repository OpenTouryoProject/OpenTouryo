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
//* クラス名        ：TestStringChecker
//* クラス日本語名  ：StringCheckerのテスト
//*
//* 作成者          ：西野 大介
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2020/07/31  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestStringChecker
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            string temp;

            temp = "あああ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsNumbers - " + temp + ": "
                + StringChecker.IsNumbers(temp));

            temp = "１１１";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsNumbers - " + temp + ": "
                + StringChecker.IsNumbers(temp));

            temp = "あああ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsNumbers_Hankaku - " + temp + ": "
                + StringChecker.IsNumbers_Hankaku(temp));

            temp = "111111";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsNumbers_Hankaku - " + temp + ": "
                + StringChecker.IsNumbers_Hankaku(temp));

            temp = "あああ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsNumbers_Zenkaku - " + temp + ": "
                + StringChecker.IsNumbers_Zenkaku(temp));

            temp = "１１１";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsNumbers_Zenkaku - " + temp + ": "
                + StringChecker.IsNumbers_Zenkaku(temp));

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            temp = "あああ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsAlphabet - " + temp + ": "
                + StringChecker.IsAlphabet(temp));

            temp = "ａａａ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsAlphabet - " + temp + ": "
                + StringChecker.IsAlphabet(temp));

            temp = "あああ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsAlphabet_Hankaku - " + temp + ": "
                + StringChecker.IsAlphabet_Hankaku(temp));

            temp = "aaaaaa";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsAlphabet_Hankaku - " + temp + ": "
                + StringChecker.IsAlphabet_Hankaku(temp));

            temp = "あああ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsAlphabet_Zenkaku - " + temp + ": "
                + StringChecker.IsAlphabet_Zenkaku(temp));

            temp = "ａａａ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsAlphabet_Zenkaku - " + temp + ": "
                + StringChecker.IsAlphabet_Zenkaku(temp));

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            temp = "ａａａ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsHiragana - " + temp + ": "
                + StringChecker.IsHiragana(temp));

            temp = "あああ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsHiragana - " + temp + ": "
                + StringChecker.IsHiragana(temp));

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            temp = "あああ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsKatakana - " + temp + ": "
                + StringChecker.IsKatakana(temp));

            temp = "アアア";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsKatakana - " + temp + ": "
                + StringChecker.IsKatakana(temp));


            temp = "あああ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsKatakana_Zenkaku - " + temp + ": "
                + StringChecker.IsKatakana_Zenkaku(temp));

            temp = "アアア";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsKatakana_Zenkaku - " + temp + ": "
                + StringChecker.IsKatakana_Zenkaku(temp));

            temp = "アアア";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsKatakana_Zenkaku - " + temp + ": "
                + StringChecker.IsKatakana_Hankaku(temp));

            temp = "ｱｱｱｱｱｱ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsKatakana_Zenkaku - " + temp + ": "
                + StringChecker.IsKatakana_Hankaku(temp));

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            temp = "あああ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsKanji - " + temp + ": "
                + StringChecker.IsKanji(temp));

            temp = "亜亜亜";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsKanji - " + temp + ": "
                + StringChecker.IsKanji(temp));

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            temp = "鱓鱓鱓";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsShift_Jis - " + temp + ": "
                + StringChecker.IsShift_Jis(temp));

            temp = "亜亜亜";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsShift_Jis - " + temp + ": "
                + StringChecker.IsShift_Jis(temp));

            temp = "aaaaaa";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsShift_Jis_Zenkaku - " + temp + ": "
                + StringChecker.IsShift_Jis_Zenkaku(temp));

            temp = "亜亜亜";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsShift_Jis_Zenkaku - " + temp + ": "
                + StringChecker.IsShift_Jis_Zenkaku(temp));

            temp = "ａａａ";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsShift_Jis_Hankaku - " + temp + ": "
                + StringChecker.IsShift_Jis_Hankaku(temp));

            temp = "aaaaaa";
            MyDebug.OutputDebugAndConsole(
                "StringChecker.IsShift_Jis_Hankaku - " + temp + ": "
                + StringChecker.IsShift_Jis_Hankaku(temp));

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestStringChecker.HankakuZenkakuTest();
            TestStringChecker.IsNumericTest();
            TestStringChecker.IsInCodePageTest();
            TestStringChecker.RegexTest();
        }
        #endregion

        #region private

        /// <summary>半角・全角の判定（IsHankaku、IsZenkaku）</summary>
        /// <remarks>
        /// **どちらも「空文字列は true」。** 0 回以上の連続マッチ（*）なので、
        /// 空なら両方 true になる。呼ぶ側が見落としやすい。
        /// </remarks>
        private static void HankakuZenkakuTest()
        {
            // 半角空白・記号・半角カナも「半角」に含まれる（^[ -~｡-ﾟ]*$）
            string[] inputs = new string[] { "abc123", "ｱｲｳ", "あいう", "ａｂｃ", "abcあ", " ", "～", "" };

            foreach (string s in inputs)
            {
                MyDebug.OutputDebugAndConsole(
                    "StringChecker.IsHankaku/IsZenkaku - [" + s + "]: "
                    + StringChecker.IsHankaku(s) + " / " + StringChecker.IsZenkaku(s));
            }
        }

        /// <summary>数値として読めるかの判定（IsNumeric）</summary>
        /// <remarks>
        /// **IsNumbers との違いを見る。**
        /// IsNumbers は「数字のみ」、IsNumeric は「数値として読めるか」で、
        /// 符号・小数点・指数が通る。全角は半角化してから判定される。
        /// </remarks>
        private static void IsNumericTest()
        {
            string[] inputs = new string[]
            {
                "123", "-123", "1.5", "+1", "1e3", "１２３", "１．５", "12 3", "abc", "", " "
            };

            foreach (string s in inputs)
            {
                MyDebug.OutputDebugAndConsole(
                    "StringChecker.IsNumeric - [" + s + "]: " + StringChecker.IsNumeric(s)
                    + "（IsNumbers: " + StringChecker.IsNumbers(s) + "）");
            }
        }

        /// <summary>コードページに収まるかの判定（IsInCodePage）</summary>
        /// <remarks>
        /// **往復して戻るかで判定している。** 収まらない文字は「?」や U+FFFD に化けるため、
        /// 元と一致しなくなる。
        /// </remarks>
        private static void IsInCodePageTest()
        {
            string[] inputs = new string[] { "abc", "あいう", "①", "𩸽", "" };

            foreach (string s in inputs)
            {
                MyDebug.OutputDebugAndConsole(
                    "StringChecker.IsInCodePage - [" + s + "]: "
                    + "shift_jis " + StringChecker.IsInCodePage(s, CustomEncode.shift_jis)
                    + " / us_ascii " + StringChecker.IsInCodePage(s, CustomEncode.us_ascii)
                    + " / utf_8 " + StringChecker.IsInCodePage(s, CustomEncode.UTF_8));
            }
        }

        /// <summary>正規表現（Match、Matches）</summary>
        private static void RegexTest()
        {
            // **Match は「どこかに一致」。** 先頭・末尾を固定しないと部分一致になる。
            MyDebug.OutputDebugAndConsole(
                "StringChecker.Match - [abc123] [0-9]+ : " + StringChecker.Match("abc123", "[0-9]+"));

            MyDebug.OutputDebugAndConsole(
                "StringChecker.Match - [abc123] ^[0-9]+$ : " + StringChecker.Match("abc123", "^[0-9]+$"));

            MyDebug.OutputDebugAndConsole(
                "StringChecker.Match - [abc] [0-9]+ : " + StringChecker.Match("abc", "[0-9]+"));

            // オプション付き（大文字小文字を無視）
            MyDebug.OutputDebugAndConsole(
                "StringChecker.Match - [ABC] ^abc$ : " + StringChecker.Match("ABC", "^abc$"));

            MyDebug.OutputDebugAndConsole(
                "StringChecker.Match - [ABC] ^abc$ (IgnoreCase) : "
                + StringChecker.Match("ABC", "^abc$", RegexOptions.IgnoreCase));

            // Matches は一致の個数と中身
            MatchCollection mc = StringChecker.Matches("a1b22c333", "[0-9]+");

            MyDebug.OutputDebugAndConsole(
                "StringChecker.Matches - [a1b22c333] [0-9]+ : " + mc.Count.ToString() + " 件");

            foreach (Match m in mc)
            {
                MyDebug.OutputDebugAndConsole(
                    "  位置 " + m.Index.ToString() + " : " + m.Value);
            }

            // 一致なし（空のコレクション）
            MyDebug.OutputDebugAndConsole(
                "StringChecker.Matches - [abc] [0-9]+ : "
                + StringChecker.Matches("abc", "[0-9]+").Count.ToString() + " 件");
        }

        #endregion
    }
}
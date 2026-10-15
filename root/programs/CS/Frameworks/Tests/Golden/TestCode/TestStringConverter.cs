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
//* クラス名        ：TestStringConverter
//* クラス日本語名  ：StringConverterのテスト
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
using System.IO;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestStringConverter
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            string temp = "アアア";
            string result = StringConverter.ToHankaku(temp);
            MyDebug.OutputDebugAndConsole("StringConverter.ToHankaku - " + temp + ": " + result);
            temp = result;
            result = StringConverter.ToZenkaku(temp);
            MyDebug.OutputDebugAndConsole("StringConverter.ToZenkaku - " + temp + ": " + result);
            temp = result;
            result = StringConverter.ToHiragana(temp);
            MyDebug.OutputDebugAndConsole("StringConverter.ToHiragana - " + temp + ": " + result);
            temp = result;
            result = StringConverter.ToKatakana(temp);
            MyDebug.OutputDebugAndConsole("StringConverter.ToKatakana - " + temp + ": " + result);

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestStringConverter.EditYYYYMMDDStringTest();
            TestStringConverter.FormattingForOneLineLogTest();
        }
        #endregion

        #region private

        /// <summary>日付文字列の桁揃え（EditYYYYMMDDString）</summary>
        /// <remarks>
        /// **7 桁のときの解釈が 2 通りある。**
        /// 5〜6 文字目が 13 以上なら「月が 1 桁」、13 未満なら「日が 1 桁」とみなす。
        /// 両方を通さないと、この判定が効いているか分からない。
        /// </remarks>
        private static void EditYYYYMMDDStringTest()
        {
            string[] inputs = new string[]
            {
                "20200102",   // 8 桁（そのまま）
                "2020115",    // 7 桁・5〜6文字目 "11" → 13 未満 → 月 11、日 5
                "2020155",    // 7 桁・5〜6文字目 "15" → 13 以上 → 月 1、日 55
                "202012",     // 6 桁 → 月・日とも 1 桁
                "2020",       // 桁が足りない
                "20201a02",   // 数字以外を含む
                ""            // 空文字列
            };

            foreach (string s in inputs)
            {
                // **ref 引数なので、渡した変数が書き換わる。**
                string work = s;
                bool ret = StringConverter.EditYYYYMMDDString(ref work);

                MyDebug.OutputDebugAndConsole(
                    "StringConverter.EditYYYYMMDDString - [" + s + "]: " + ret + " → [" + work + "]");
            }
        }

        /// <summary>1 行ログ向けの整形（FormattingForOneLineLog）</summary>
        /// <remarks>
        /// **文字列の中（シングルクォートで囲まれた範囲）では空白を詰めない。**
        /// クォートの内と外を 1 つの入力に混ぜないと、その分岐を通らない。
        /// </remarks>
        private static void FormattingForOneLineLogTest()
        {
            string[] inputs = new string[]
            {
                "SELECT  *   FROM  Orders",                  // 連続した空白を詰める
                "SELECT * FROM T WHERE C = 'a   b'",         // 文字列中の空白は残る
                "a\r\nb\rc\nd",                              // 改行は空白になる
                "A & B",                                     // & のエスケープ
                "WHERE C = 'It''s'",                         // エスケープされたシングルクォート
                "",                                          // 空文字列
                "   "                                        // 空白のみ
            };

            foreach (string s in inputs)
            {
                MyDebug.OutputDebugAndConsole(
                    "StringConverter.FormattingForOneLineLog - ["
                    + s.Replace("\r", "\\r").Replace("\n", "\\n") + "]: ["
                    + StringConverter.FormattingForOneLineLog(s) + "]");
            }
        }

        #endregion
    }
}
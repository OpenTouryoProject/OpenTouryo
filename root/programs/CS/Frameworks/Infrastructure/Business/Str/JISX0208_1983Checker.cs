//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

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
//* クラス名        ：JISX0208_1983Checker
//* クラス日本語名  ：JIS X 0208-1983文字コード範囲チェック・クラス
//* 　　　　　　　　　・01～08区：記号、英数字、かな
//* 　　　　　　　　　・16～47区：JIS第1水準漢字
//* 　　　　　　　　　・48～84区：JIS第2水準漢字
//*   　　　　　　　　※JIS X 0208-1990で追加された「凜[7425]」「熙[7426]」は含まれない
//* 　　　　　　　　　※NEC機種依存文字、NECのIBM拡張文字、IBM拡張文字は含まれない
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/06/20  西野 大介         新規作成
//**********************************************************************************

using System.Text;
using System.Collections.Generic;
using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Business.Str
{
    /// <summary>
    /// JIS X 0208-1983文字コード範囲チェック・クラス
    /// ・01～08区：記号、英数字、かな
    /// ・16～47区：JIS第1水準漢字
    /// ・48～84区：JIS第2水準漢字
    /// ※JIS X 0208-1990で追加された「凜[7425]」「熙[7426]」は含まれない
    /// ※NEC機種依存文字、NECのIBM拡張文字、IBM拡張文字は含まれない
    /// </summary>
    /// <remarks>同じアルゴリズムで他の文字コード範囲チェック・クラスも開発できる</remarks>
    public class JISX0208_1983Checker
    {
        /// <summary>CheckCharCodeのリスト（許可されるコード範囲のリスト）</summary>
        private static List<CheckCharCode> CCCList;

        /// <summary>シングルトン</summary>
        private static JISX0208_1983Checker _JISX0208_1983Checker = new JISX0208_1983Checker();
        
        /// <summary>コンストラクタ</summary>
        public JISX0208_1983Checker()
        {
            // エンコーディングと、コード範囲のリストを定義する。
            JISX0208_1983Checker.CCCList = new List<CheckCharCode>();
            Encoding sjisEncoding = Encoding.GetEncoding(CustomEncode.shift_jis);

            // 文字コード表 シフトJIS(Shift_JIS)
            // http://charset.7jp.net/sjis.html

            JISX0208_1983Checker.CCCList.Add(new CheckCharCode(
                '\u0000'.ToString(), // NUL文字
                '\u007F'.ToString(), // DEL文字
                sjisEncoding));

            // 半角カナ
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("｡", "ﾟ", sjisEncoding));

            // 以下、全角の範囲
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("　", "〓", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("∈", "∩", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("∧", "∃", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("∠", "∬", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("Å", "¶", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("◯", "◯", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("０", "９", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("Ａ", "Ｚ", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("ａ", "ｚ", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("ぁ", "ん", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("ァ", "ヶ", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("Α", "Ω", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("α", "ω", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("А", "Я", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("а", "я", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("─", "╂", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("亜", "腕", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("弌", "滌", sjisEncoding));
            JISX0208_1983Checker.CCCList.Add(new CheckCharCode("漾", "瑤", sjisEncoding));
        }

        /// <summary>範囲チェックする</summary>
        /// <param name="input">チェック対象文字列</param>
        /// <param name="index">
        /// エラー文字のインデックス
        /// 戻り値 = falseでindex = -1の場合は、SJISでない場合を表す。
        /// </param>
        /// <param name="ch">
        /// エラーの文字
        /// </param>
        /// <returns>
        /// true：範囲内
        /// false：範囲外
        /// </returns>
        /// <remarks>
        /// 空文字列が指定された場合は、trueが返ります。
        /// </remarks>
        public static bool IsJISX0208_1983(string input, out int index, out string ch)
        {
            // trueの場合
            index = -1;
            ch = "";

            //// 改行コードを除去
            //input = input.Replace("\r", "");
            //input = input.Replace("\n", "");

            if (input.Length == 0)
            {
                // 空文字列が指定された場合は、true
                return true;
            }
            else
            {
                // SJISチェック
                if (StringChecker.IsShift_Jis(input))
                {
                    // SJISである。
                    for (int i = 0; i <= input.Length - 1; i++)
                    {
                        // 当該文字は、

                        // チェック対象文字・チェック結果
                        bool result = false;
                        string tempChar = input[i].ToString();

                        foreach (CheckCharCode ccc in JISX0208_1983Checker.CCCList)
                        {
                            // 当該範囲の、
                            if (ccc.IsInRange(tempChar))
                            {
                                // 範囲内
                                result = true;
                            }
                            else
                            {
                                // 範囲外
                            }
                        }

                        // 当該文字は、全範囲外
                        if (!result)
                        {
                            index = i;
                            ch = tempChar;
                            return false;
                        }
                    }

                    // 全文字、範囲内
                    return true;
                }
                else
                {
                    // SJISでない。
                    return false;
                }
            }
        }
    }
}

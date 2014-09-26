//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//* クラス名        ：StringConverter
//* クラス日本語名  ：文字列の変換処理クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/xx/xx  西野  大介        新規作成
//*  2009/11/06  西野  大介        平仮名 / 片仮名 変換処理を追加
//*  2012/10/07  西野  大介        MやDが１桁の場合に、YYYYMMDDに変換（入力補完）
//**********************************************************************************

// VB.NET関数活用
using Microsoft.VisualBasic;

// System
using System;
using System.Text;
using System.Collections;

using System.Globalization;
using System.Text.RegularExpressions;

namespace Touryo.Infrastructure.Public.Str
{
    /// <summary>文字列の変換処理クラス</summary>
    public class StringConverter
    {
        #region 全角 / 半角 変換処理

        /// <summary>→ 全角変換</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>（全角化された）出力文字列</returns>
        public static string ToZenkaku(string input)
        {
            // VB関数を使用する。
            return Strings.StrConv(input, VbStrConv.Wide, 0);
        }

        /// <summary>→ 半角変換</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>（半角化された）出力文字列</returns>
        public static string ToHankaku(string input)
        {
            // VB関数を使用する。
            return Strings.StrConv(input, VbStrConv.Narrow, 0);
        }

        #endregion

        #region 平仮名 / 片仮名 変換処理

        /// <summary>→ 平仮名変換</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>（平仮名化された）出力文字列</returns>
        public static string ToHiragana(string input)
        {
            // VB関数を使用する。
            return Strings.StrConv(input, VbStrConv.Hiragana, 0);
        }

        /// <summary>→ 片仮名変換</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>（片仮名化された）出力文字列</returns>
        public static string ToKatakana(string input)
        {
            // VB関数を使用する。
            return Strings.StrConv(input, VbStrConv.Katakana, 0);
        }

        #endregion

        #region 入力補完 変換処理

        /// <summary>MやDが１桁の場合に、YYYYMMDDに変換</summary>
        /// <param name="text">
        /// YYYYMMDDに変換する、
        /// ・YYYYMMD
        /// ・YYYYMDD
        /// ・YYYYMD
        /// などの文字列。
        /// </param>
        /// <returns>
        /// true：変換した場合。
        /// false：変換しなかった場合。
        /// </returns>
        public static bool EditYYYYMMDDString(ref string text)
        {
            // (1) 入力文字数が 7 桁の場合、
            //     5 文字目 ～ 6 文字目をチェックし、
            //     ・13 以上ならば、
            //       ・0 を 5 文字目の前に付加し月とする。
            //       ・6 文字目～ 7 文字目を日とする。
            //     ・13 未満ならば、
            //       ・5 文字目 ～ 6文字目を月とする。
            //       ・0 を 7 文字目の前に付加し日とする。
            // (2) 入力文字数が 6 桁の場合、
            //     ・0 を 5 文字目の前に付加し月とする。
            //     ・0 を 6 文字目の前に付加し日とする。

            if (StringChecker.IsNumbers_Hankaku(text))
            {
                if (text.Length == 7)
                {
                    // (1) 入力文字数が 7 桁の場合、
                    int temp = 0;

                    if (int.TryParse(text.Substring(4, 2), out temp))
                    {
                        if (temp >= 13)
                        {
                            text =
                                text.Substring(0, 4)
                                + "0" + text.Substring(4, 1)
                                + text.Substring(5, 2);

                            // 変更された。
                            return true;
                        }
                        else
                        {
                            text =
                                text.Substring(0, 4)
                                + text.Substring(4, 2)
                                + "0" + text.Substring(6, 1);

                            // 変更された。
                            return true;
                        }
                    }
                }
                else if (text.Length == 6)
                {
                    // (2) 入力文字数が 6 桁の場合
                    text =
                        text.Substring(0, 4)
                        + "0" + text.Substring(4, 1)
                        + "0" + text.Substring(5, 1);

                    // 変更された。
                    return true;
                }
            }

            // 何もしない。
            return false;
        }

        #endregion
    }
}

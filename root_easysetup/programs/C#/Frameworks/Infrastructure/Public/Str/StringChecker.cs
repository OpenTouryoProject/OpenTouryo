//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//* クラス名        ：StringChecker
//* クラス日本語名  ：正規表現を用いた文字列チェック処理クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/xx/xx  西野  大介        新規作成
//*  2009/11/06  西野  大介        VB版でメソッド名の大/小文字の区別が付かないので、
//*                                区別が付くようにメソッド名を変更した。
//*  2009/11/16  西野  大介        ニーズを考慮してAPIを見直し。
//*  2011/04/11  西野  大介        メソッド追加（IsHankaku、IsZenkaku、IsKatakana）
//*  2011/04/11  西野  大介        連続一致を０回以上に変更（必須入力と被る：+ → *）
//*  2011/05/23  西野  大介        Matchesメソッドを追加
//*  2012/09/26  西野  大介        IsNumericメソッドを追加
//*  2013/08/30  西野  大介        IsInCodePageメソッドを追加
//**********************************************************************************

// System
using System;
using System.Text;
using System.Collections;

using System.Globalization;
using System.Text.RegularExpressions;

namespace Touryo.Infrastructure.Public.Str
{
    /// <summary>
    /// 正規表現を用いた文字列チェック処理クラス
    /// </summary>
    /// <remarks>
    /// 必要であれば、（生技）から提供している
    /// ＜方式設計アシスト＞
    /// DMSG-0083:.NETに於ける正規表現、概要
    /// を参照し、必要なメソッドを追加下さい。
    /// </remarks>
    public class StringChecker
    {
        #region 数値チェック

        /// <summary>数字（double）に変換可能か確認する。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：数字に変換可能。
        /// false：数字に変換不可能。
        /// </returns>
        /// <remarks>
        /// double.TryParseでの実装。
        /// http://support.microsoft.com/kb/329488/ja
        /// 全角文字もチェック可能（半角変換後にチェック）。
        /// </remarks>
        public static bool IsNumeric(string input)
        {
            double temp;

            // 全角文字が混じる場合は半角化する。
            if (!StringChecker.IsHankaku(input))
            {
                input = StringConverter.ToHankaku(input);
            }

            return double.TryParse(input, out temp);
        }

        /// <summary>指定された文字列が、数字のみで構成されているかどうか確認する。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：数字のみで構成されている。
        /// false：数字のみで構成されていない。
        /// </returns>
        /// <remarks>
        /// 数値チェックという意味ではParse、TryParse
        /// メソッドを使用すべきかどうかも検討下さい。
        /// 空文字列が指定された場合は、trueが返ります。
        /// </remarks>
        public static bool IsNumbers(string input)
        {
            // 先頭から末尾まで、0-9, ０-９が０回以上連続マッチ

            //// 文字コード表利用
            //return StringChecker.Match(input, "^[0-9０-９]*$");
            // 数値文字クラス利用
            return StringChecker.Match(input, "^\\d*$");
        }

        /// <summary>指定された文字列が、数字（半角）のみで構成されているかどうか確認する。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：数字（半角）のみで構成されている。
        /// false：数字（半角）のみで構成されていない。
        /// </returns>
        /// <remarks>
        /// 数値チェックという意味ではParse、TryParse
        /// メソッドを使用すべきかどうかも検討下さい。
        /// 空文字列が指定された場合は、trueが返ります。
        /// </remarks>
        public static bool IsNumbers_Hankaku(string input)
        {
            // 先頭から末尾まで、0-9が０回以上連続マッチ

            // 文字コード表利用
            return StringChecker.Match(input, "^[0-9]*$");
        }

        /// <summary>指定された文字列が、数字（全角）のみで構成されているかどうか確認する。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：数字（全角）のみで構成されている。
        /// false：数字（全角）のみで構成されていない。
        /// </returns>
        /// <remarks>
        /// 数値チェックという意味ではParse、TryParse
        /// メソッドを使用すべきかどうかも検討下さい。
        /// 空文字列が指定された場合は、trueが返ります。
        /// </remarks>
        public static bool IsNumbers_Zenkaku(string input)
        {
            // 先頭から末尾まで、０-９が０回以上連続マッチ

            // 文字コード表利用（文字クラス無し）
            return StringChecker.Match(input, "^[０-９]*$");
        }

        #endregion

        #region 全角半角

        /// <summary>指定された文字列が、半角文字のみで構成されているかどうか確認する。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：半角文字のみで構成されている。
        /// false：半角文字のみで構成されていない。
        /// </returns>
        /// <remarks>空文字列が指定された場合は、trueが返ります。</remarks>
        public static bool IsHankaku(string input)
        {
            // 先頭から末尾まで、「[ ]-~、｡-ﾟ」が０回以上連続マッチ

            // 文字コード表利用（文字クラス無し）
            return StringChecker.Match(input, "^[ -~｡-ﾟ]*$");
        }

        /// <summary>指定された文字列が、全角文字のみで構成されているかどうか確認する。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：全角文字のみで構成されている。
        /// false：全角文字のみで構成されていない。
        /// </returns>
        /// <remarks>空文字列が指定された場合は、trueが返ります。</remarks>
        public static bool IsZenkaku(string input)
        {
            // 先頭から末尾まで、「[ ]-~、｡-ﾟ」（を除く）が０回以上連続マッチ

            // 文字コード表利用（文字クラス無し）
            return StringChecker.Match(input, "^[^ -~｡-ﾟ]*$");
        }

        #endregion

        #region 英字チェック

        /// <summary>指定された文字列が、英字のみで構成されているかどうか確認する。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：英字のみで構成されている。
        /// false：英字のみで構成されていない。
        /// </returns>
        /// <remarks>空文字列が指定された場合は、trueが返ります。</remarks>
        public static bool IsAlphabet(string input)
        {
            // 先頭から末尾まで、Ａ-Ｚ、ａ-ｚ、A-Z、a-zが０回以上連続マッチ

            // 文字コード表利用（文字クラス無し）
            return StringChecker.Match(input, "^[Ａ-Ｚａ-ｚA-Za-z]*$");
        }

        /// <summary>指定された文字列が、英字（半角）のみで構成されているかどうか確認する。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：英字（半角）のみで構成されている。
        /// false：英字（半角）のみで構成されていない。
        /// </returns>
        /// <remarks>空文字列が指定された場合は、trueが返ります。</remarks>
        public static bool IsAlphabet_Hankaku(string input)
        {
            // 先頭から末尾まで、A-Z、a-zが０回以上連続マッチ

            // 文字コード表利用（文字クラス無し）
            return StringChecker.Match(input, "^[A-Za-z]*$");
        }

        /// <summary>指定された文字列が、英字（全角）のみで構成されているかどうか確認する。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：英字（全角）のみで構成されている。
        /// false：英字（全角）のみで構成されていない。
        /// </returns>
        /// <remarks>空文字列が指定された場合は、trueが返ります。</remarks>
        public static bool IsAlphabet_Zenkaku(string input)
        {
            // 先頭から末尾まで、Ａ-Ｚ、ａ-ｚが０回以上連続マッチ

            // 文字コード表利用（文字クラス無し）
            return StringChecker.Match(input, "^[Ａ-Ｚａ-ｚ]*$");
        }

        #endregion        

        #region 日本語チェック

        /// <summary>指定された文字列が平仮名であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：平仮名である。
        /// false：平仮名でない。
        /// </returns>
        /// <remarks>空文字列が指定された場合は、trueが返ります。</remarks>
        public static bool IsHiragana(string input)
        {
            // 先頭から末尾まで、ぁ-ゞが０回以上連続マッチ

            //// 文字コード表利用
            //return StringChecker.Match(input, "^[ぁ-ゞ]*$");
            // 日本語文字クラス利用
            return StringChecker.Match(input, "^\\p{IsHiragana}*$");
        }

        /// <summary>指定された文字列が片仮名であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：片仮名である。
        /// false：片仮名でない。
        /// </returns>
        /// <remarks>空文字列が指定された場合は、trueが返ります。</remarks>
        public static bool IsKatakana(string input)
        {
            return StringChecker.IsKatakana_Hankaku(input)
                || StringChecker.IsKatakana_Zenkaku(input);
        }

        /// <summary>指定された文字列が片仮名（全角）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：片仮名（全角）である。
        /// false：片仮名（全角）でない。
        /// </returns>
        /// <remarks>空文字列が指定された場合は、trueが返ります。</remarks>
        public static bool IsKatakana_Zenkaku(string input)
        {
            // 先頭から末尾まで、ァ-ヾが０回以上連続マッチ

            //// 文字コード表利用
            //return StringChecker.Match(input, "^[ァ-ヾ]*$");
            // 日本語文字クラス利用
            return StringChecker.Match(input, "^\\p{IsKatakana}*$");
        }

        /// <summary>指定された文字列が片仮名（半角）であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：片仮名（半角）である。
        /// false：片仮名（半角）でない。
        /// </returns>
        /// <remarks>空文字列が指定された場合は、trueが返ります。</remarks>
        public static bool IsKatakana_Hankaku(string input)
        {
            // 先頭から末尾まで、ｦ-ﾟが０回以上連続マッチ

            // 文字コード表利用
            return StringChecker.Match(input, "^[ｦ-ﾟ]*$");
        }

        /// <summary>指定された文字列が漢字であるか。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：漢字である。
        /// false：漢字でない。
        /// </returns>
        /// <remarks>空文字列が指定された場合は、trueが返ります。</remarks>
        public static bool IsKanji(string input)
        {
            // 先頭から末尾まで、漢字文字が０回以上連続マッチ

            // 漢字文字クラス利用
            return StringChecker.Match(input, "^\\p{IsCJKUnifiedIdeographs}*$");
        }

        #endregion

        #region 指定コードページの可逆エンコード・チェック

        /// <summary>
        /// 指定された文字列が、
        /// ・当該コードページのみで構成されており
        /// ・可逆エンコード可能かどうか？
        /// を確認する。
        /// </summary>
        /// <param name="input">
        /// 入力文字列
        /// </param>
        /// <param name="codePageNum">
        /// エンコーディングに使用するコードページ
        /// </param>
        /// <returns>
        /// true：当該コードページの可逆エンコードな文字で構成されている。
        /// false：当該コードページの可逆エンコードな文字で構成されていない。
        /// </returns>
        /// <remarks>
        /// 指定コードページのエンコーディング
        /// を使用した可逆チェックを使用して判定する。
        /// 空文字列が指定された場合は、trueが返ります。
        /// </remarks>
        public static bool IsInCodePage(string input, int codePageNum)
        {
            // 空文字列が指定された場合は、true
            if (input == "") return true;

            // 可逆性チェック
            byte[] byteData = CustomEncode.StringToByte(input, codePageNum);
            if (input == CustomEncode.ByteToString(byteData, codePageNum))
            {
                // 可逆性、有り = 指定コードページの文字である。
                return true;
            }
            else
            {
                // 可逆性、無し = 指定コードページの文字でない。
                return false;
            }
        }

        #endregion

        #region SJISチェック

        /// <summary>指定された文字列が、SJIS文字のみで構成されているかどうか確認する。</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：SJIS文字のみで構成されている。
        /// false：SJIS文字のみで構成されていない。
        /// </returns>
        /// <remarks>
        /// SJISエンコーディングの可逆チェックを使用して判定する。
        /// 空文字列が指定された場合は、trueが返ります。
        /// </remarks>
        public static bool IsShift_Jis(string input)
        {
            // 空文字列が指定された場合は、true
            if (input == "") return true;

            // 可逆性チェック
            byte[] sjisData = CustomEncode.StringToByte(input, CustomEncode.shift_jis);
            if (input == CustomEncode.ByteToString(sjisData, CustomEncode.shift_jis))
            {
                // 可逆性、有り = SJISである。
                return true;
            }
            else
            {
                // 可逆性、無し = SJISでない。
                return false;
            }
        }

        /// <summary>
        /// 指定された文字列が、SJIS全角
        /// （にエンコーディング可能な）文字
        /// のみで構成されているかどうか確認する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：SJIS全角文字のみで構成されている。
        /// false：SJIS全角文字のみで構成されていない。
        /// </returns>
        /// <remarks>
        /// SJISエンコーディングを使用して判定する。
        /// .NET TIPS > 文字列の全角／半角をチェックするには？
        /// http://www.atmarkit.co.jp/fdotnet/dotnettips/014strcheck/strcheck.html
        /// 空文字列が指定された場合は、trueが返ります。
        /// </remarks>
        public static bool IsShift_Jis_Zenkaku(string input)
        {
            // SJIS文字チェック
            if (StringChecker.IsShift_Jis(input))
            {
                // SJIS文字である。

                // バイト数を取得
                Encoding sjisEnc = Encoding.GetEncoding(CustomEncode.shift_jis);
                int byteLen = sjisEnc.GetByteCount(input);

                if (byteLen == input.Length * 2)
                {
                    // バイト数 ＝ 文字数 × ２の場合はSJIS全角
                    return true;
                }
                else
                {
                    // バイト数 ≠ 文字数 × ２の場合はSJIS半角が混じる
                    return false;
                }
            }
            else
            {
                // SJIS文字でない。
                return false;
            }
        }

        /// <summary>
        /// 指定された文字列が、SJIS半角
        /// （にエンコーディング可能な）文字
        /// のみで構成されているかどうか確認する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>
        /// true：SJIS半角文字のみで構成されている。
        /// false：SJIS半角文字のみで構成されていない。
        /// </returns>
        /// <remarks>
        /// SJISエンコーディングを使用して判定する。
        /// .NET TIPS > 文字列の全角／半角をチェックするには？
        /// http://www.atmarkit.co.jp/fdotnet/dotnettips/014strcheck/strcheck.html
        /// 空文字列が指定された場合は、trueが返ります。
        /// </remarks>
        public static bool IsShift_Jis_Hankaku(string input)
        {
            // SJIS文字チェック
            if (StringChecker.IsShift_Jis(input))
            {
                // SJIS文字である。
                
                // バイト数を取得
                Encoding sjisEnc = Encoding.GetEncoding(CustomEncode.shift_jis);
                int byteLen = sjisEnc.GetByteCount(input);

                if (byteLen == input.Length)
                {
                    // バイト数 ＝ 文字数の場合はSJIS半角
                    return true;
                }
                else
                {
                    // バイト数 ≠ 文字数の場合はSJIS全角が混じる
                    return false;
                }
            }
            else
            {
                // SJIS文字でない。
                return false;
            }
        }

        #endregion

        #region 汎用部品

        /// <summary>
        /// RegexクラスとMatchクラスを使用して、
        /// 正規表現のパターン マッチングを実行する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <returns>
        /// true：一致した。
        /// false：一致しなかった。
        /// </returns>
        public static bool Match(string input, string pattern)
        {
            return StringChecker.Match(input, pattern, RegexOptions.None);
        }

        /// <summary>
        /// RegexクラスとMatchクラスを使用して、
        /// 正規表現のパターン マッチングを実行する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <param name="options">オプション</param>
        /// <returns>
        /// true：一致した。
        /// false：一致しなかった。
        /// </returns>
        public static bool Match(string input, string pattern, RegexOptions options)
        {
            Regex rgx = new Regex(pattern, options);
            Match mch = rgx.Match(input);
            return mch.Success;
        }

        /// <summary>
        /// RegexクラスとMatchCollectionクラスを使用して、
        /// 正規表現のパターン マッチングを実行する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <returns>
        /// 一致のコレクション
        /// </returns>
        public static MatchCollection Matches(string input, string pattern)
        {
            return StringChecker.Matches(input, pattern, RegexOptions.None);
        }
        /// <summary>
        /// RegexクラスとMatchCollectionクラスを使用して、
        /// 正規表現のパターン マッチングを実行する。
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <param name="pattern">正規表現パターン</param>
        /// <param name="options">オプション</param>
        /// <returns>
        /// 一致のコレクション
        /// </returns>
        public static MatchCollection Matches(string input, string pattern, RegexOptions options)
        {
            Regex rgx = new Regex(pattern, options);
            return rgx.Matches(input);
        }

        #endregion
    }
}

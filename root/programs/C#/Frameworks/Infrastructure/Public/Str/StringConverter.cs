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
//* クラス名        ：StringConverter
//* クラス日本語名  ：文字列の変換処理クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/xx/xx  西野 大介         新規作成
//*  2009/11/06  西野 大介         平仮名 / 片仮名 変換処理を追加
//*  2012/10/07  西野 大介         MやDが１桁の場合に、YYYYMMDDに変換（入力補完）
//*  2015/09/30  Sai-san           Changed the parameter locale ID to 1041(Japanese) in StrConv method
//*  2017/08/11  西野 大介         BaseDam.ClearText ---> StringConverter.FormattingForOneLineLog
//*  2018/03/28  西野 大介         .NET Standard対応で、Microsoft.VisualBasicのサポート無し。
//**********************************************************************************

using System.Text;
#if NETSTANDARD2_0
#else
using Microsoft.VisualBasic;
#endif


namespace Touryo.Infrastructure.Public.Str
{
    /// <summary>文字列の変換処理クラス</summary>
    public class StringConverter
    {
#if NETSTANDARD2_0
#else
        #region 全角 / 半角 変換処理

        /// <summary>→ 全角変換</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>（全角化された）出力文字列</returns>
        public static string ToZenkaku(string input)
        {
            // VB関数を使用する。
            return Strings.StrConv(input, VbStrConv.Wide, 1041);
        }

        /// <summary>→ 半角変換</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>（半角化された）出力文字列</returns>
        public static string ToHankaku(string input)
        {
            // VB関数を使用する。
            return Strings.StrConv(input, VbStrConv.Narrow, 1041);
        }

        #endregion

        #region 平仮名 / 片仮名 変換処理

        /// <summary>→ 平仮名変換</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>（平仮名化された）出力文字列</returns>
        public static string ToHiragana(string input)
        {
            // VB関数を使用する。
            return Strings.StrConv(input, VbStrConv.Hiragana, 1041);
        }

        /// <summary>→ 片仮名変換</summary>
        /// <param name="input">入力文字列</param>
        /// <returns>（片仮名化された）出力文字列</returns>
        public static string ToKatakana(string input)
        {
            // VB関数を使用する。
            return Strings.StrConv(input, VbStrConv.Katakana, 1041);
        }

        #endregion
#endif

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

        #region その他

        /// <summary>
        /// 1行ログ出力用整形。
        /// 
        /// （１）
        /// 以下の文字を半角空白に変換する。
        /// キャリッジリターン文字とラインフィード文字
        /// '\r\n'
        /// キャリッジリターン文字
        /// '\r'
        /// ラインフィード文字
        /// '\n'
        /// 
        /// （２）
        /// ２文字以上連続する
        /// 半角スペース・タブ（\t）は削除する。
        /// （ただし、文字列中は、詰めない。）
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>処理後のテキスト</returns>
        public static string FormattingForOneLineLog(string text)
        {
            // StringBuilderを使用して
            // インナーテキストをキレイにする。
            StringBuilder sb = new StringBuilder();

            // キャリッジリターン文字とラインフィード文字
            // '\r\n'
            // キャリッジリターン文字
            // '\r'
            // ラインフィード文字
            // '\n'
            //// タブ文字
            //// '\t'
            // を取り除く
            text = text.Replace("\r\n", " ");
            text = text.Replace('\r', ' ');
            text = text.Replace('\n', ' ');
            //text = text.Replace('\t', ' ');

            // & → &amp;置換
            text = text.Replace("&", "&amp;");
            // エスケープされているシングルクォートを置換
            text = text.Replace("''", "&SingleQuote2;");

            // 連続した空白は、詰める
            bool isConsecutive = false;

            // 文字列中は、詰めない
            bool isString = false;

            foreach (char ch in text)
            {
                if (ch == '\'')
                {
                    // 出たり入ったり（文字列）。
                    isString = !isString;
                }

                if (ch == ' ')
                {
                    if (isConsecutive && !isString)
                    {
                        // 空白（半角スペース）が連続＆文字列外。
                        // → アペンドしない。
                    }
                    else
                    {
                        // 空白（半角スペース）が初回 or 文字列中。
                        // → アペンドする。
                        sb.Append(ch);

                        // 空白（半角スペース）が連続しているフラグを立てる。
                        isConsecutive = true;
                    }
                }
                else if (ch == '\t')
                {
                    if (isConsecutive && !isString)
                    {
                        // 空白（タブ文字）が連続＆文字列外。
                        // → アペンドしない。
                    }
                    else
                    {
                        // 空白（タブ文字）が初回 or 文字列中。
                        // → アペンドする。
                        sb.Append(ch);

                        // 空白（タブ文字）が連続しているフラグを立てる。
                        isConsecutive = true;
                    }
                }
                else
                {
                    // アペンドする。
                    sb.Append(ch);

                    // 連続した空白が途切れたので、フラグを倒す。
                    isConsecutive = false;
                }
            }

            // 戻し（エスケープされているシングルクォートを置換）。
            text = sb.ToString().Replace("&SingleQuote2;", "''");

            // 戻し（& → &amp;置換）
            text = text.Replace("&amp;", "&");

            // 結果を返す
            return text;
        }

        #endregion
    }
}

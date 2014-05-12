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
//* クラス名        ：FormatConverter
//* クラス日本語名  ：文字列書式の変換処理クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/xx/xx  西野  大介        新規作成
//*  2009/11/16  西野  大介        ニーズを考慮してAPIを見直し。
//*  2010/09/24  西野  大介        型チェック方式の見直し（ GetType() & typeof() ）
//*  2010/09/27  西野  大介        AddFigureｘ：非数値型の場合、エラー → ０を返す。
//*  2011/04/13  西野  大介        AddFigureｘ：小数点数以下に対応。
//*  2011/01/31  西野  大介        Roundｘ関数を追加（四捨五入、銀行家の丸め）
//*  2011/02/06  西野  大介        AddFigureｘ：リファクタリング
//*  2013/04/04  西野  大介        Math.Roundに合わせ処理後に「0」を付与（Floor、Ceiling）
//*  2013/07/17  西野  大介        AddFigure内のlongをdecimalに変更（オーバーフロー対策対策）。
//**********************************************************************************

// VB.NET関数活用
using Microsoft.VisualBasic;

// System
using System;
using System.Text;
using System.Collections;

using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Touryo.Infrastructure.Public.Str
{
    /// <summary>切り捨て処理の方向</summary>
    /// <remarks>
    /// ・0への丸め (rounding toward zero; RZ)
    /// ・負の無限大への丸め (rounding toward minus infinity; RM)
    /// </remarks>
    public enum FloorToward : int
    {
        /// <summary>rounding toward zero（0への丸め：絶対値・一般）</summary>
        RZ,

        /// <summary>rounding toward minus infinity（負の無限大への丸め：大小・算術）</summary>
        RM
    }

    /// <summary>切り上げ処理の方向</summary>
    /// <remarks>
    /// ・無限大への丸め (rounding toward infinity; RI)
    /// ・正の無限大への丸め (rounding toward plus infinity; RP)
    /// </remarks>
    public enum CeilingToward : int
    {
        /// <summary>rounding toward infinity（無限大への丸め：絶対値・一般）</summary>
        RI,

        /// <summary>rounding toward plus infinity（正の無限大への丸め：大小・算術）</summary>
        RP
    }

    /// <summary>文字列書式の変換処理クラス</summary>
    public class FormatConverter
    {
        #region 数値丸め

        /// <summary>
        /// 小数点数以下ｘ桁最近接偶数編集
        /// </summary>
        /// <param name="number">数値</param>
        /// <param name="digitsAfterDecimalPoint">小数点数以下の桁数</param>
        /// <returns>小数点数以下ｘ桁最近接偶数編集後の文字列</returns>
        /// <remarks>
        /// ＠IT > .NET TIPS > 数値を四捨五入するには？［2.0のみ、C#、VB］
        /// http://www.atmarkit.co.jp/fdotnet/dotnettips/700mathround/mathround.html
        /// 小数点を切り捨て、切り上げ、四捨五入する .NET Tips C#, VB.NET
        /// http://dobon.net/vb/dotnet/programing/round.html
        /// </remarks>
        public static string Round_Banker(object number, int digitsAfterDecimalPoint)
        {
            decimal dcm;

            // decimalにParseしてみる。
            if (decimal.TryParse(number.ToString(), out dcm))
            {
                // 最近接偶数への（銀行家の）丸め（端数処理の累積時の誤差小）
                return Math.Round(
                    dcm, digitsAfterDecimalPoint,
                    MidpointRounding.ToEven).ToString();
            }
            else
            {
                return "0";
            }
        }

        /// <summary>
        /// 小数点数以下ｘ桁四捨五入編集
        /// </summary>
        /// <param name="number">数値</param>
        /// <param name="digitsAfterDecimalPoint">小数点数以下の桁数</param>
        /// <returns>小数点数以下ｘ桁四捨五入編集後の文字列</returns>
        /// <remarks>
        /// ＠IT > .NET TIPS > 数値を四捨五入するには？［2.0のみ、C#、VB］
        /// http://www.atmarkit.co.jp/fdotnet/dotnettips/700mathround/mathround.html
        /// 小数点を切り捨て、切り上げ、四捨五入する .NET Tips C#, VB.NET
        /// http://dobon.net/vb/dotnet/programing/round.html
        /// </remarks>
        public static string Round_4sya5nyu(object number, int digitsAfterDecimalPoint)
        {
            decimal dcm;

            // decimalにParseしてみる。
            if (decimal.TryParse(number.ToString(), out dcm))
            {
                // 四捨五入
                return Math.Round(
                    dcm, digitsAfterDecimalPoint,
                    MidpointRounding.AwayFromZero).ToString();
            }
            else
            {
                return "0";
            }
        }

        /// <summary>
        /// 小数点数以下ｘ桁切り捨て（負の無限大への丸め：大小）
        /// </summary>
        /// <param name="number">数値</param>
        /// <param name="digitsAfterDecimalPoint">小数点数以下の桁数</param>
        /// <returns>小数点数以下ｘ桁切り捨て編集後の文字列</returns>
        public static string Floor(object number, uint digitsAfterDecimalPoint)
        {
            // デフォルトは、負の無限大への丸め：大小・算術（下位互換のため
            return FormatConverter.Floor(number, digitsAfterDecimalPoint, FloorToward.RM);
        }

        /// <summary>
        /// 小数点数以下ｘ桁切り捨て
        /// </summary>
        /// <param name="number">数値</param>
        /// <param name="digitsAfterDecimalPoint">小数点数以下の桁数</param>
        /// <param name="ft">切り捨て処理の方向</param>
        /// <returns>小数点数以下ｘ桁切り捨て編集後の文字列</returns>
        /// <remarks>
        /// ＠IT > .NET TIPS > 数値の切り捨て／切り上げを行うには？［C#、VB］
        /// http://www.atmarkit.co.jp/fdotnet/dotnettips/703mathfloorceiling/mathfloorceiling.html
        /// 小数点を切り捨て、切り上げ、四捨五入する .NET Tips C#, VB.NET
        /// http://dobon.net/vb/dotnet/programing/round.html
        /// </remarks>
        public static string Floor(object number, uint digitsAfterDecimalPoint, FloorToward ft)
        {
            decimal dcm;

            // decimalにParseしてみる。
            if (decimal.TryParse(number.ToString(), out dcm))
            {
                // シフトさせる数を計算。
                decimal shift = 1;

                // 小数点数以下２桁切り捨てで、100
                for (int i = 0; i < digitsAfterDecimalPoint;  i++)
                {
                    // 10倍
                    shift *= 10;
                }

                // シフトさせる
                dcm *= shift;

                // 切り捨て
                if (ft == FloorToward.RZ)
                {
                    // 絶対値・一般
                    if (0 <= dcm)
                    {
                        // 0以上
                        dcm = Math.Floor(dcm);
                    }
                    else
                    {
                        // 0未満
                        dcm = Math.Truncate(dcm);
                        //dcm = Microsoft.VisualBasic.Conversion.Fix(dcm);
                    }
                }
                else
                {
                    // 大小・算術
                    dcm = Math.Floor(dcm);
                }
                
                // シフトさせる
                dcm /= shift;

                // 戻す（0の追加
                return FormatConverter.AddZerosAfterDecimal(dcm, digitsAfterDecimalPoint);
            }
            else
            {
                return "0";
            }
        }

        /// <summary>
        /// 小数点数以下ｘ桁切り上げ（正の無限大への丸め：大小）
        /// </summary>
        /// <param name="number">数値</param>
        /// <param name="digitsAfterDecimalPoint">小数点数以下の桁数</param>
        /// <returns>小数点数以下ｘ桁切り上げ編集後の文字列</returns>
        public static string Ceiling(object number, uint digitsAfterDecimalPoint)
        {
            // デフォルトは、正の無限大への丸め：大小とする（下位互換のため
            return FormatConverter.Ceiling(number, digitsAfterDecimalPoint, CeilingToward.RP);
        }

        /// <summary>
        /// 小数点数以下ｘ桁切り上げ
        /// </summary>
        /// <param name="number">数値</param>
        /// <param name="digitsAfterDecimalPoint">小数点数以下の桁数</param>
        /// <param name="ct">切り上げ処理の方向</param>
        /// <returns>小数点数以下ｘ桁切り上げ編集後の文字列</returns>
        /// <remarks>
        /// ＠IT > .NET TIPS > 数値の切り捨て／切り上げを行うには？［C#、VB］
        /// http://www.atmarkit.co.jp/fdotnet/dotnettips/703mathfloorceiling/mathfloorceiling.html
        /// 小数点を切り捨て、切り上げ、四捨五入する .NET Tips C#, VB.NET
        /// http://dobon.net/vb/dotnet/programing/round.html
        /// </remarks>
        public static string Ceiling(object number, uint digitsAfterDecimalPoint, CeilingToward ct)
        {
            decimal dcm;

            // decimalにParseしてみる。
            if (decimal.TryParse(number.ToString(), out dcm))
            {
                // シフトさせる数を計算。
                decimal shift = 1;

                // 小数点数以下２桁切り上げで、100
                for (int i = 0; i < digitsAfterDecimalPoint; i++)
                {
                    // 10倍
                    shift *= 10;
                }

                // シフトさせる
                dcm *= shift;

                // 切り上げ
                if (ct == CeilingToward.RI)
                {
                    // 絶対値・一般
                    if (0 <= dcm)
                    {
                        // 0以上
                        dcm = Math.Ceiling(dcm);
                    }
                    else
                    {
                        // 0未満
                        dcm = Microsoft.VisualBasic.Conversion.Int(dcm);
                    }
                }
                else
                {
                    // 大小・算術
                    dcm = Math.Ceiling(dcm);
                }

                // シフトさせる
                dcm /= shift;

                // 戻す（0の追加
                return FormatConverter.AddZerosAfterDecimal(dcm, digitsAfterDecimalPoint);
            }
            else
            {
                return "0";
            }
        }

        /// <summary>小数点以下に、指定桁数まで０を補充</summary>
        /// <param name="dcm">decimalデータ</param>
        /// <param name="digitsAfterDecimalPoint">小数点以下</param>
        /// <returns></returns>
        public static string AddZerosAfterDecimal(decimal dcm, uint digitsAfterDecimalPoint)
        {
            // 一時変数
            string temp = dcm.ToString();
            string[] temps = temp.Split('.');
            
            // 戻り
            StringBuilder ret = null;

            // 長さの確認
            if (temps.Length == 1)
            {
                // 整数部のみ
                ret = new StringBuilder();

                // 少数部を処理
                for (int i = 0; i < digitsAfterDecimalPoint; i++)
                {
                    // ０を追加していく。
                    ret.Append('0');
                }

                if (ret.Length == 0)
                {
                    // 処理結果を返却する
                    //（整数まで切り上げ・切り捨て）
                    return temps[0];
                }
                else
                {
                    // 処理結果を（整数部を戻して）返却する。
                    return temps[0] + "." + ret.ToString();
                }
            }
            else if (temps.Length == 2)
            {
                // 少数部を含む

                // 少数部を処理
                ret = new StringBuilder(temps[1]);
                for (int i = ret.Length; i < digitsAfterDecimalPoint; i++)
                {
                    // ０を追加していく。
                    ret.Append('0');
                }

                // 処理結果を（整数部を戻して）返却する。
                return temps[0] + "." + ret.ToString();
            }
            else
            {
                // 処理不可能
                return dcm.ToString();
            }
        }

        #endregion

        #region 数値桁区切り

        /// <summary>３桁区切りの文字列に変換</summary>
        /// <param name="number">数値</param>
        /// <returns>
        /// ３桁区切りの数値文字列
        /// </returns>
        public static string AddFigure3(object number)
        {
            return FormatConverter.AddFigureX(number, 3);
        }

        /// <summary>４桁区切りの文字列に変換</summary>
        /// <param name="number">数値</param>
        /// <returns>
        /// ４桁区切りの数値文字列
        /// </returns>
        public static string AddFigure4(object number)
        {
            return FormatConverter.AddFigureX(number, 4);
        }

        /// <summary>ｘ桁区切りの文字列に変換</summary>
        /// <param name="number">数値</param>
        /// <param name="numberGroupSizes">ｘ桁区切りのｘ（数値）</param>
        /// <returns>
        /// ｘ桁区切りの数値文字列
        /// </returns>
        /// <remarks>
        /// 数値をｘけた区切りの文字列に変換するには？
        /// http://www.atmarkit.co.jp/fdotnet/dotnettips/620number3groupsep/number3groupsep.html
        /// http://www.atmarkit.co.jp/fdotnet/dotnettips/622number4groupsep/number4groupsep.html
        /// </remarks>
        public static string AddFigureX(object number, int numberGroupSizes)
        {
            decimal dcm;
            string temp = "";

            // decimalにParseしてみる。
            if (decimal.TryParse(number.ToString(), out dcm))
            {
                // 数値

                // マイナスの場合
                bool isMinus = false;

                if (dcm < 0)
                {
                    isMinus = true;
                }

                // 絶対値
                dcm = Math.Abs(dcm);

                // 数値の切り捨て／切り上げを行うには？
                // http://www.atmarkit.co.jp/fdotnet/dotnettips/703mathfloorceiling/mathfloorceiling.html
                // ・Math.Floor(decimal or double) → 切捨
                // ・Math.Ceiling(decimal or double) → 切上

                // マイナスの場合の処理は意識しない。
                decimal numberPart = Math.Floor(dcm); // 整数部（切捨）
                decimal decimalPart = dcm % 1.0m; // 小数部（剰余）

                // もともとのカルチャをバックアップ
                CultureInfo backup = CultureInfo.CurrentCulture;

                try
                {
                    // 現在のスレッドのカルチャを書き換える
                    CultureInfo ci =
                      (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    ci.NumberFormat.NumberGroupSizes = new int[] { numberGroupSizes };
                    Thread.CurrentThread.CurrentCulture = ci;

                    // ４桁区切りの文字列に変換（小数点以下四捨五入）
                    temp = String.Format("{0:#,0}", numberPart);
                }
                finally
                {
                    // もともとのカルチャを復元
                    Thread.CurrentThread.CurrentCulture = backup;
                }

                // 小数部の結合
                if (decimalPart != 0)
                {
                    // 整数部に小数部を結合して戻す
                    //（小数部の「0.nnn」の0を削除）。
                    temp += decimalPart.ToString().Substring(1);
                }
                else
                {
                    // 文字列の場合は[n.000]の.000を復元する。
                    // 小数点以下の桁区切りは（現状）しない。
                    if (number is string)
                    {
                        if (((string)number).IndexOf('.') == -1)
                        {
                            // 小数点以下無し
                        }
                        else
                        {

                            // 小数点以下有り
                            string[] arrayStr = ((string)number).Split('.');

                            // 復元して戻す
                            // ・「.」が複数あるケースは考慮しない
                            // ・国際化対応も（現状）考慮しない 
                            //　 「.」→「,」のケースがある。
                            // 小数点 - Wikipedia
                            // http://ja.wikipedia.org/wiki/%E5%B0%8F%E6%95%B0%E7%82%B9
                            temp += "." + arrayStr[arrayStr.Length - 1];
                        }
                    }
                }

                // マイナス記号の付与
                if (isMinus)
                {
                    temp = "-" + temp;
                }

                // 処理して戻す
                return temp;
            }
            else
            {
                // 数値でない。
                return "0";
            }
        }

        #endregion

        #region サプレス

        /// <summary>指定の文字で、桁数にあわせてサプレスする</summary>
        /// <param name="input">入力文字列</param>
        /// <param name="totalWidth">サプレスする桁</param>
        /// <param name="paddingChar">サプレスする文字</param>
        /// <returns>サプレスされた文字列</returns>
        public static string Suppress(string input, int totalWidth, char paddingChar)
        {
            return input.PadLeft(totalWidth, paddingChar);
        }

        #endregion

        #region 和暦 ⇔ 西暦変換

        #region 西暦（DateTime） → 和暦（文字列）

        /// <summary>西暦（DateTime）を和暦文字列に変換する。</summary>
        /// <param name="seireki">西暦（DateTime）</param>
        /// <param name="warekiPattern">
        /// 和暦パターン文字
        /// 
        /// 　書式指定子  説明            出力例 
        /// -------------+----------------+---------------
        /// ・gg         | 元号名         | 平成
        ///              |                |
        /// ・y          | 年             | n or nn 
        /// ・yy         | 0埋め2桁の年   | 0n or nn
        ///              |                |
        /// ・M          | 月             | n or nn
        /// ・MM         | 0埋め2桁の月   | 0n or nn
        ///              |                |
        /// ・d          | 日             | n or nn
        /// ・dd         | 0埋め2桁の日   | 0n or nn
        ///              |                |
        /// ・ddd        | 曜日の省略名   | 火
        /// ・dddd       | 曜日の完全名   | 火曜日
        ///              |                |
        /// ・H          | 時間           | n or nn（24時間表記）  
        /// ・HH         | 0埋め2桁の時間 | 0n or nn（24時間表記）  
        /// ・h          | 時間           | n or nn（12時間表記）
        /// ・hh         | 0埋め2桁の時間 | 0n or nn（12時間表記）
        /// ・tt         | 午前 / 午後    | 午前 or 午後
        ///              |                | 
        /// ・m          | 分             | n or nn
        /// ・mm         | 0埋め2桁の分   | 0n or nn
        /// ・s          | 秒             | n or nn
        /// ・ss         | 0埋め2桁の秒   | 0n or nn
        /// -------------+----------------+---------------
        /// </param>
        /// <returns>和暦文字列</returns>
        /// <example>
        /// FormatConverter.SeirekiToWareki(
        ///     DateTime.Parse("1977/4/24"), "ggy年M月d日（ddd）");
        /// </example>
        public static string SeirekiToWareki(DateTime seireki, string warekiPattern)
        {
            // カルチャを生成
            //CultureInfo culture = new CultureInfo(0x0411, true);
            CultureInfo culture = new CultureInfo("ja-JP", true);

            // カルチャに和暦カレンダを設定
            culture.DateTimeFormat.Calendar = new JapaneseCalendar();

            // 和暦カレンダを使用して、パターンで文字列化
            return seireki.ToString(warekiPattern, culture);
        }

        #endregion

        #region 和暦（文字列） →　西暦（DateTime）

        /// <summary>和暦文字列を西暦（DateTime）に変換する。</summary>
        /// <param name="wareki">和暦文字列</param>
        /// <param name="warekiPattern">
        /// 和暦パターン文字
        /// 
        /// 　書式指定子  説明            出力例 
        /// -------------+----------------+---------------
        /// ・gg         | 元号名         | 平成
        ///              |                |
        /// ・y          | 年             | n or nn 
        /// ・yy         | 0埋め2桁の年   | 0n or nn
        ///              |                |
        /// ・M          | 月             | n or nn
        /// ・MM         | 0埋め2桁の月   | 0n or nn
        ///              |                |
        /// ・d          | 日             | n or nn
        /// ・dd         | 0埋め2桁の日   | 0n or nn
        ///              |                |
        /// ・ddd        | 曜日の省略名   | 火
        /// ・dddd       | 曜日の完全名   | 火曜日
        ///              |                |
        /// ・H          | 時間           | n or nn（24時間表記）  
        /// ・HH         | 0埋め2桁の時間 | 0n or nn（24時間表記）  
        /// ・h          | 時間           | n or nn（12時間表記）
        /// ・hh         | 0埋め2桁の時間 | 0n or nn（12時間表記）
        /// ・tt         | 午前 / 午後    | 午前 or 午後
        ///              |                | 
        /// ・m          | 分             | n or nn
        /// ・mm         | 0埋め2桁の分   | 0n or nn
        /// ・s          | 秒             | n or nn
        /// ・ss         | 0埋め2桁の秒   | 0n or nn
        /// -------------+----------------+---------------
        /// </param>
        /// <returns>西暦（DateTime）</returns>
        /// <example>
        /// FormatConverter.WarekiToSeireki(
        ///     "昭和52年4月24日（日）", "ggy年M月d日（ddd）")
        /// </example>
        public static DateTime WarekiToSeireki(string wareki, string warekiPattern)
        {
            // カルチャを生成
            //CultureInfo culture = new CultureInfo(0x0411, true);
            CultureInfo culture = new CultureInfo("ja-JP", true);

            // カルチャに和暦カレンダを設定
            culture.DateTimeFormat.Calendar = new JapaneseCalendar();

            // 和暦カレンダを使用して、パターンで文字列化
            return DateTime.ParseExact(wareki, warekiPattern, culture);
        }

        #endregion

        #endregion
    }
}

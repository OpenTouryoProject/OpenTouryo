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
//* クラス名        ：TestFormatConverter
//* クラス日本語名  ：FormatConverterのテスト
//*
//* 作成者          ：西野 大介
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2020/07/31  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Collections.Generic;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestFormatConverter
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            TestFormatConverter.SeirekiToWarekiTest();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatConverter.WarekiToSeirekiTest();
            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatConverter.AddFigureAndSuppressTest();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatConverter.RoundingTest();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatConverter.FloorAndCeilingTest();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatConverter.AddFigureXTest();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatConverter.UnixAndW3cTest();
        }
        #endregion

        #region private

        #region 丸め・桁合わせ

        /// <summary>四捨五入と銀行家の丸め（Round_4sya5nyu、Round_Banker）</summary>
        /// <remarks>
        /// **同じ入力を並べて対比する。** 差が出るのは 0.5 ちょうどのときだけで、
        /// 片方だけ試しても「違い」が見えない。
        /// </remarks>
        private static void RoundingTest()
        {
            MyDebug.OutputDebugAndConsole("FormatConverter.Round_4sya5nyu / Round_Banker");

            object[] numbers = new object[] { 0.5m, 1.5m, 2.5m, 3.5m, -0.5m, -2.5m, 2.4m, 2.6m };

            foreach (object n in numbers)
            {
                MyDebug.OutputDebugAndConsole(
                    "  " + n.ToString() + " → 四捨五入 " + FormatConverter.Round_4sya5nyu(n, 0)
                    + " / 銀行家 " + FormatConverter.Round_Banker(n, 0));
            }

            // 小数点以下の桁数を指定
            MyDebug.OutputDebugAndConsole(
                "  1.005(2桁) → 四捨五入 " + FormatConverter.Round_4sya5nyu(1.005m, 2)
                + " / 銀行家 " + FormatConverter.Round_Banker(1.005m, 2));

            // **数値として読めないと "0" を返す。** else 側の分岐。
            MyDebug.OutputDebugAndConsole(
                "  \"abc\" → 四捨五入 " + FormatConverter.Round_4sya5nyu("abc", 0)
                + " / 銀行家 " + FormatConverter.Round_Banker("abc", 0));

            // 文字列で渡しても、数値として読めれば処理される
            MyDebug.OutputDebugAndConsole(
                "  \"2.5\"(文字列) → 四捨五入 " + FormatConverter.Round_4sya5nyu("2.5", 0));
        }

        /// <summary>切り捨てと切り上げ（Floor、Ceiling）</summary>
        /// <remarks>
        /// **負の数で向きの違いが出る。** 正の数だけでは RZ と RM、RI と RP が同じ値になり、
        /// 引数の意味が確かめられない。
        /// </remarks>
        private static void FloorAndCeilingTest()
        {
            MyDebug.OutputDebugAndConsole("FormatConverter.Floor / Ceiling");

            object[] numbers = new object[] { 1.567m, -1.567m };

            foreach (object n in numbers)
            {
                MyDebug.OutputDebugAndConsole(
                    "  Floor " + n.ToString() + "(2桁) : 既定 " + FormatConverter.Floor(n, 2)
                    + " / RZ(0方向) " + FormatConverter.Floor(n, 2, FloorToward.RZ)
                    + " / RM(負の無限大) " + FormatConverter.Floor(n, 2, FloorToward.RM));

                MyDebug.OutputDebugAndConsole(
                    "  Ceiling " + n.ToString() + "(2桁) : 既定 " + FormatConverter.Ceiling(n, 2)
                    + " / RI(絶対値) " + FormatConverter.Ceiling(n, 2, CeilingToward.RI)
                    + " / RP(正の無限大) " + FormatConverter.Ceiling(n, 2, CeilingToward.RP));
            }

            // 桁数 0（シフトのループを 1 度も回らない）
            MyDebug.OutputDebugAndConsole(
                "  Floor 1.9(0桁) : " + FormatConverter.Floor(1.9m, 0)
                + " / Ceiling 1.1(0桁) : " + FormatConverter.Ceiling(1.1m, 0));

            // **数値として読めないと "0" を返す。**
            MyDebug.OutputDebugAndConsole(
                "  \"abc\" : Floor " + FormatConverter.Floor("abc", 2)
                + " / Ceiling " + FormatConverter.Ceiling("abc", 2));
        }

        /// <summary>桁区切りと 0 の補充（AddFigureX、AddZerosAfterDecimal）</summary>
        private static void AddFigureXTest()
        {
            MyDebug.OutputDebugAndConsole("FormatConverter.AddFigureX");

            foreach (int size in new int[] { 2, 3, 4 })
            {
                MyDebug.OutputDebugAndConsole(
                    "  1234567.891 を " + size.ToString() + "桁区切り : "
                    + FormatConverter.AddFigureX(1234567.891m, size));
            }

            // 負の数（絶対値にしてから戻す分岐）
            MyDebug.OutputDebugAndConsole(
                "  -1234567.891 を 3桁区切り : " + FormatConverter.AddFigureX(-1234567.891m, 3));

            // **数値として読めないと "0" を返す。**
            MyDebug.OutputDebugAndConsole(
                "  \"abc\" を 3桁区切り : " + FormatConverter.AddFigureX("abc", 3));

            MyDebug.OutputDebugAndConsole("FormatConverter.AddZerosAfterDecimal");

            // **整数部のみ（split の結果が 1 個）と、小数部あり（2 個）で分岐が違う。**
            MyDebug.OutputDebugAndConsole("  123(3桁) : " + FormatConverter.AddZerosAfterDecimal(123m, 3));
            MyDebug.OutputDebugAndConsole("  123.4(3桁) : " + FormatConverter.AddZerosAfterDecimal(123.4m, 3));

            // 既に桁数を満たしている（0 を足さない）
            MyDebug.OutputDebugAndConsole("  123.456(2桁) : " + FormatConverter.AddZerosAfterDecimal(123.456m, 2));

            // 桁数 0（整数部のみのとき、小数点も付かない）
            MyDebug.OutputDebugAndConsole("  123(0桁) : " + FormatConverter.AddZerosAfterDecimal(123m, 0));

            // 負の数
            MyDebug.OutputDebugAndConsole("  -1.2(3桁) : " + FormatConverter.AddZerosAfterDecimal(-1.2m, 3));
        }

        #endregion

        #region UNIX時間・W3C時間

        /// <summary>UNIX時間と W3C Timestamp（ToUnixTime、FromUnixTime、ToW3cTimestamp、FromW3cTimestamp）</summary>
        /// <remarks>
        /// **DateTimeKind.Utc を明示する。**
        /// ToUnixTime は ToUniversalTime() を通すため、Kind が Local や Unspecified だと
        /// **実行環境の時刻帯で結果が変わる。** 結果は Result*.txt と突き合わせるので、
        /// 環境で変わる値をここに出してはいけない。
        ///
        /// 出力の書式も明示する。DateTime.ToString() は文化圏で形が変わる。
        /// </remarks>
        private static void UnixAndW3cTest()
        {
            const string fmt = "yyyy/MM/dd HH:mm:ss";

            MyDebug.OutputDebugAndConsole("FormatConverter.ToUnixTime / FromUnixTime");

            DateTime[] times = new DateTime[]
            {
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc),     // epoch
                new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2038, 1, 19, 3, 14, 7, DateTimeKind.Utc),   // 32bit の上限
                new DateTime(1960, 1, 1, 0, 0, 0, DateTimeKind.Utc)      // epoch より前（負の値）
            };

            foreach (DateTime t in times)
            {
                long unix = FormatConverter.ToUnixTime(t);

                MyDebug.OutputDebugAndConsole(
                    "  " + t.ToString(fmt) + " → " + unix.ToString()
                    + " → " + FormatConverter.FromUnixTime(unix).ToString(fmt)
                    + "（Kind=" + FormatConverter.FromUnixTime(unix).Kind.ToString() + "）");
            }

            // **秒未満は切り捨てられる。** TotalSeconds を long にキャストしているため。
            MyDebug.OutputDebugAndConsole(
                "  秒未満 : " + FormatConverter.ToUnixTime(
                    new DateTime(1970, 1, 1, 0, 0, 0, 999, DateTimeKind.Utc)).ToString());

            MyDebug.OutputDebugAndConsole("FormatConverter.ToW3cTimestamp / FromW3cTimestamp");

            DateTime utc = new DateTime(2020, 5, 6, 7, 8, 9, DateTimeKind.Utc);

            string w3c = FormatConverter.ToW3cTimestamp(utc);
            MyDebug.OutputDebugAndConsole("  既定の書式 : " + w3c);

            MyDebug.OutputDebugAndConsole(
                "  書式を指定 : " + FormatConverter.ToW3cTimestamp(utc, "yyyy-MM-dd HH:mm:ssZ"));

            DateTime back = FormatConverter.FromW3cTimestamp(w3c);
            MyDebug.OutputDebugAndConsole(
                "  戻り : " + back.ToString(fmt) + "（Kind=" + back.Kind.ToString() + "）");
        }

        #endregion

        #region 和暦・西暦
        /// <summary>SeirekiToWarekiTest</summary>
        private static void SeirekiToWarekiTest()
        {
            MyDebug.OutputDebugAndConsole("FormatConverter.SeirekiToWareki");

            string datetimeString = "";
            string warekiPattern = "";
            DateTime dt = DateTime.Now;

            // 基本バージョン
            datetimeString = "1977/4/24";
            warekiPattern = "ggy年M月d日（ddd）";
            MyDebug.OutputDebugAndConsole(
                datetimeString + ", " + warekiPattern + ": "
                + FormatConverter.SeirekiToWareki(DateTime.Parse(datetimeString), warekiPattern));

            // パターンだけ時間あり
            datetimeString = "1977/4/24";
            warekiPattern = "ggy年M月d日（ddd）H:m:s";
            MyDebug.OutputDebugAndConsole(
                datetimeString + ", " + warekiPattern + ": "
                + FormatConverter.SeirekiToWareki(DateTime.Parse(datetimeString), warekiPattern));

            // DateTimeだけ時間あり
            datetimeString = "1977/4/24 19:15:12";
            warekiPattern = "ggy年M月d日（ddd）";
            MyDebug.OutputDebugAndConsole(
                datetimeString + ", " + warekiPattern + ": "
                + FormatConverter.SeirekiToWareki(DateTime.Parse(datetimeString), warekiPattern));

            // 時間情報込みバージョン（24時間表記）
            datetimeString = "1977/4/24 19:15:12";
            warekiPattern = "ggy年M月d日（ddd）H:m:s";
            MyDebug.OutputDebugAndConsole(
                datetimeString + ", " + warekiPattern + ": "
                + FormatConverter.SeirekiToWareki(DateTime.Parse(datetimeString), warekiPattern));

            // 時間情報込みバージョン（12時間表記）
            datetimeString = "1977/4/24 19:15:12";
            warekiPattern = "ggy年M月d日（ddd）tt h:m:s";
            MyDebug.OutputDebugAndConsole(
                datetimeString + ", " + warekiPattern + ": "
                + FormatConverter.SeirekiToWareki(DateTime.Parse(datetimeString), warekiPattern));

            // 上記のパターン文字列の変更版
            datetimeString = "1992/2/6 1:1:1";
            warekiPattern = "ggyy年MM月dd日 dddd HH:mm:ss"; // 0埋め2桁
            MyDebug.OutputDebugAndConsole(
                datetimeString + ", " + warekiPattern + ": "
                + FormatConverter.SeirekiToWareki(DateTime.Parse(datetimeString), warekiPattern));

            datetimeString = "1992/2/6 13:1:1";
            warekiPattern = "ggyy年MM月dd日 dddd tt hh:mm:ss"; // 0埋め2桁
            MyDebug.OutputDebugAndConsole(
                datetimeString + ", " + warekiPattern + ": "
                + FormatConverter.SeirekiToWareki(DateTime.Parse(datetimeString), warekiPattern));
        }

        /// <summary>WarekiToSeirekiTest</summary>
        private static void WarekiToSeirekiTest()
        {
            MyDebug.OutputDebugAndConsole("FormatConverter.WarekiToSeireki");

            string warekiString = "";
            string warekiPattern = "";
            DateTime dt = DateTime.Now;

            // 基本バージョン
            warekiString = "昭和52年4月24日（日）";
            warekiPattern = "ggy年M月d日（ddd）";
            MyDebug.OutputDebugAndConsole(
                warekiString + ", " + warekiPattern + ": "
                + FormatConverter.WarekiToSeireki(warekiString, warekiPattern));

            //// パターンだけ時間あり
            //warekiString = "昭和52年4月24日（日）";
            //warekiPattern = "ggy年M月d日（ddd）H:m:s";
            //MyDebug.OutputDebugAndConsole(
            //    warekiString + ", " + warekiPattern + ": "
            //    + FormatConverter.WarekiToSeireki(warekiString, warekiPattern));

            //// 和暦文字列だけ時間あり
            //warekiString = "昭和52年4月24日（日）12:12:12";
            //warekiPattern = "ggy年M月d日（ddd）";
            //MyDebug.OutputDebugAndConsole(
            //    warekiString + ", " + warekiPattern + ": "
            //    + FormatConverter.WarekiToSeireki(warekiString, warekiPattern));

            // 時間情報込みバージョン（24時間表記）
            warekiString = "昭和52年4月24日（日）19:15:12";
            warekiPattern = "ggy年M月d日（ddd）H:m:s";
            MyDebug.OutputDebugAndConsole(
                warekiString + ", " + warekiPattern + ": "
                + FormatConverter.WarekiToSeireki(warekiString, warekiPattern));

            // 時間情報込みバージョン（12時間表記）
            warekiString = "昭和52年4月24日（日）午後 7:15:12";
            warekiPattern = "ggy年M月d日（ddd）tt h:m:s";
            MyDebug.OutputDebugAndConsole(
                warekiString + ", " + warekiPattern + ": "
                + FormatConverter.WarekiToSeireki(warekiString, warekiPattern));

            // 上記のパターン文字列の変更版
            warekiString = "平成04年02月06日 木曜日 01:01:01";
            warekiPattern = "ggyy年MM月dd日 dddd HH:mm:ss"; // 0埋め2桁
            MyDebug.OutputDebugAndConsole(
                warekiString + ", " + warekiPattern + ": "
                + FormatConverter.WarekiToSeireki(warekiString, warekiPattern));

            warekiString = "平成04年02月06日 木曜日 午後 01:01:01";
            warekiPattern = "ggyy年MM月dd日 dddd tt hh:mm:ss"; // 0埋め2桁
            MyDebug.OutputDebugAndConsole(
                warekiString + ", " + warekiPattern + ": "
                + FormatConverter.WarekiToSeireki(warekiString, warekiPattern));
        }
        #endregion

        #region AddFigureAndSuppress

        /// <summary>AddFigureAndSuppressTest</summary>
        private static void AddFigureAndSuppressTest()
        {
            List<object> list1 = new List<object>();
            list1.Add(12345);
            list1.Add(123456789);
            list1.Add(123.45);
            list1.Add(12345.6789);
            list1.Add(-12345);
            list1.Add(-123456789);
            list1.Add(-123.45);
            list1.Add(-12345.6789);

            List<object> list2 = new List<object>();
            list2.Add("12345");
            list2.Add("123456789");
            list2.Add("123.45");
            list2.Add("12345.6789");
            list2.Add("-12345");
            list2.Add("-123456789");
            list2.Add("-123.45");
            list2.Add("-12345.6789");

            MyDebug.OutputDebugAndConsole("FormatConverter.AddFigure3");
            foreach (object o in list1)
            {
                MyDebug.OutputDebugAndConsole(FormatConverter.AddFigure3(o));
            }
            foreach (object o in list2)
            {
                MyDebug.OutputDebugAndConsole(FormatConverter.AddFigure3(o));
            }

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
            MyDebug.OutputDebugAndConsole("FormatConverter.AddFigure4");
            foreach (object o in list1)
            {
                MyDebug.OutputDebugAndConsole(FormatConverter.AddFigure4(o));
            }
            foreach (object o in list2)
            {
                MyDebug.OutputDebugAndConsole(FormatConverter.AddFigure4(o));
            }

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            MyDebug.OutputDebugAndConsole("FormatConverter.Suppress");


            MyDebug.OutputDebugAndConsole("\"\", 10, '＠': " + (FormatConverter.Suppress("", 10, '＠')));
            //MyDebug.OutputDebugAndConsole("\"123456789\", -1, '＠': " + (FormatConverter.Suppress("123456789", -1, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 0, '＠': " + (FormatConverter.Suppress("123456789", 0, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 1, '＠': " + (FormatConverter.Suppress("123456789", 1, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 5, '＠': " + (FormatConverter.Suppress("123456789", 5, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 9, '＠': " + (FormatConverter.Suppress("123456789", 9, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 10, '＠': " + (FormatConverter.Suppress("123456789", 10, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 11, '＠': " + (FormatConverter.Suppress("123456789", 11, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 20, '＠': " + (FormatConverter.Suppress("123456789", 20, '＠')));

            MyDebug.OutputDebugAndConsole("\"\", 10, '＠': " + (FormatConverter.Suppress("", 10, '＠')));
            //MyDebug.OutputDebugAndConsole("\"123456789\", -1, '＠': " + (FormatConverter.Suppress("abcdefg", -1, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 0, '＠': " + (FormatConverter.Suppress("abcdefg", 0, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 1, '＠': " + (FormatConverter.Suppress("abcdefg", 1, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 5, '＠': " + (FormatConverter.Suppress("abcdefg", 5, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 9, '＠': " + (FormatConverter.Suppress("abcdefg", 9, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 10, '＠': " + (FormatConverter.Suppress("abcdefg", 10, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 11, '＠': " + (FormatConverter.Suppress("abcdefg", 11, '＠')));
            MyDebug.OutputDebugAndConsole("\"123456789\", 20, '＠': " + (FormatConverter.Suppress("abcdefg", 20, '＠')));
        }
        #endregion

        #endregion
    }
}
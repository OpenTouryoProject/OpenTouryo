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
        }
        #endregion

        #region private

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
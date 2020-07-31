using System;

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
        }
        #endregion

        #region private
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
    }
}
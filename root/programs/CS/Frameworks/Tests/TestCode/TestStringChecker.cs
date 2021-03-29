using System;
using System.Text;
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
        }
        #endregion
    }
}
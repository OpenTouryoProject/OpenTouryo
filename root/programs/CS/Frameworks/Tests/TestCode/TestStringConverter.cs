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
        }
        #endregion
    }
}
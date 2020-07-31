using System;
using System.Text;
using System.IO;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestCustomEncode
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            MyDebug.OutputDebugAndConsole(
                "CustomEncode.HtmlEncode: "
                + CustomEncode.HtmlEncode(
                    "\" id=\"txtXXXXX\" />"
                    + "<script type=\"text/javascript\">alert(\"XSS!!!\")</script>"
                    + "<input name=\"txtXXXXX\" type=\"text\" value=\""));

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            MyDebug.OutputDebugAndConsole(
                "CustomEncode.UrlEncode: "
                + "http://www.google.co.jp/search?hl=ja&q=" + CustomEncode.UrlEncode("&"));

            MyDebug.OutputDebugAndConsole(
                "CustomEncode.UrlEncode2: "
                + CustomEncode.UrlEncode2("http://www.google.co.jp/search?hl=ja&q=&"));

            MyDebug.OutputDebugAndConsole(
                "CustomEncode.UrlEncode2: "
                + CustomEncode.UrlEncode2("http://www.google.co.jp/search?hl=ja&q=<>"));
        }
        #endregion
    }
}
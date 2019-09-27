using System;
using System.Configuration;

using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class Program
    {
        /// <summary></summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // configの初期化(無くても動くようにせねば。)
#if NETCORE
            GetConfigParameter.InitConfiguration("appsettings.json");
#endif

            try
            {
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestEnumToStringExtensions.Root();
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestXmlLib.Root();
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestDeflateCompression.Root();
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

                // echoすると例外
                try
                {
                    Console.ReadKey();
                }
                catch { }
            }
            catch (Exception ex)
            {
                MyDebug.OutputDebugAndConsole(ex.ToString());
            }
        }
    }
}

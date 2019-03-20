using System;

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
            //GetConfigParameter.InitConfiguration("appsettings.json");
            
            try
            {
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestEnumToStringExtensions.Root();
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

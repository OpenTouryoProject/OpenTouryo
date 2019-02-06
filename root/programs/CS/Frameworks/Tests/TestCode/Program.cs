using System;

using Touryo.Infrastructure.Public.Dbg;
using Touryo.Infrastructure.Public.Util;

namespace TestCode
{
    /// <summary>Program</summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            // configの初期化(無くても動くようにせねば。)
            //GetConfigParameter.InitConfiguration("appsettings.json");

            try
            {
                string output = "hoge";
                WriteLine.OutPutDebugAndConsole(output);

                // echoすると例外
                try
                {
                    Console.ReadKey();
                }
                catch { }
            }
            catch (Exception ex)
            {
                WriteLine.OutPutDebugAndConsole(ex.ToString());
            }
        }
    }
}

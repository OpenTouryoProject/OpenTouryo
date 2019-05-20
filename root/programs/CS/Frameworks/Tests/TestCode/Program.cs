using System;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
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

                // DeflateCompression
                string hoge = "hogehogehogehogehogehogehogehogehogehogehogehogehogehogehogehoge";
                byte[] input = CustomEncode.StringToByte("", CustomEncode.UTF_8);
                byte[] compressed = DeflateCompression.Compress(input);
                byte[] decompressed = DeflateCompression.Decompress(compressed);
                if (hoge == CustomEncode.ByteToString(decompressed, CustomEncode.UTF_8))
                {
                    MyDebug.OutputDebugAndConsole("DeflateCompression", "is working properly.");
                }
                else
                {
                    MyDebug.OutputDebugAndConsole("DeflateCompression", "is not working properly.");
                }

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

                // TestEnumToStringExtensions
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

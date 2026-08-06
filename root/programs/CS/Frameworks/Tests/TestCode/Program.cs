using System;
using System.Text;
using System.Configuration;

using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class Program
    {
        /// <summary>Main</summary>
        /// <param name="args">string[]</param>
        public static void Main(string[] args)
        {
            // configの初期化(無くても動くようにせねば。)
#if NETCOREAPP
            GetConfigParameter.InitConfiguration("appsettings.json");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            try
            {
                #region Public
                #region Basic
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestOutputLog.Root();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestGetMessageAndProperty.Root();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestStringChecker.Root();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestFormatChecker.Root();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestStringVariableOperator.Root();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestStringConverter.Root();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestFormatConverter.Root();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestCustomEncode.Root();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                JISCode.Root();
                #endregion
                #region Extension
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestEnumToStringExtensions.Root();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestXmlLib.Root();
                
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestDeflateCompression.Root();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestResourceLoader.Root();
                #endregion
                #region Dto
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestDto.Root();
                #endregion
                #region Diagnostics
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestObjectInspector.Root();
                #endregion
                #region Reflection
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestLatebind.Root();

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestFastReflection.Root();
                #endregion
                // Db は TestDataAccess へ移した（#520）。
                // DB に接続するテストと前提が異なるため、プロジェクトを分けている。
                #endregion

                #region Business
                // Touryo.Infrastructure.Business
                // GMTMaster
                // JISX0208_1983Checker
                #endregion

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

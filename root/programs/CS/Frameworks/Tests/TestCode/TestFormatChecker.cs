using System;
using System.Text;
using System.IO;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Diagnostics;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestFormatChecker
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            TestFormatChecker.InitTestCase();

            TestFormatChecker.IsJpZipCode();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpZipCode_Hyphen();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpZipCode_NoHyphen();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpZipCode7();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpZipCode7_Hyphen();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpZipCode7_NoHyphen();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpZipCode5();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpZipCode5_Hyphen();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpZipCode5_NoHyphen();
        }
        #endregion

        #region private

        #region テストケース
        /// <summary>JpZipCode_Hyphen</summary>
        private static List<string> JpZipCode_Hyphen = null;
        /// <summary>JpZipKuCode1_Hyphen</summary>
        private static List<string> JpZipKuCode1_Hyphen = null;
        /// <summary>JpZipKuCode2_Hyphen</summary>
        private static List<string> JpZipKuCode2_Hyphen = null;

        /// <summary>JpZipCode_NoHyphen</summary>
        private static List<string> JpZipCode_NoHyphen = null;
        /// <summary>JpZipKuCode1_NoHyphen</summary>
        private static List<string> JpZipKuCode1_NoHyphen = null;
        /// <summary>JpZipKuCode2_NoHyphen</summary>
        private static List<string> JpZipKuCode2_NoHyphen = null;

        /// <summary>IsJpZipCode</summary>
        private static void InitTestCase()
        {
            TestFormatChecker.JpZipCode_Hyphen = new List<string>
            {
                "000-0000",
                "aaa-aaaa",
                "0000-00000",
                "0000-0000",
                "000-00000",
                "00-000",
                "00-0000",
                "000-000"
            };

            TestFormatChecker.JpZipKuCode1_Hyphen = new List<string>
            {
                "000-00",
                "aaa-aa",
                "0000-000",
                "0000-00",
                "000-000",
                "00-0",
                "00-00",
                "000-0"
            };

            TestFormatChecker.JpZipKuCode2_Hyphen = new List<string>
            {
                "000",
                "aaa",
                "0000",
                "00"
            };

            TestFormatChecker.JpZipCode_NoHyphen = new List<string>
            {
                "0000000",
                "aaaaaaa",
                "00000000",
                "000000"
            };

            TestFormatChecker.JpZipKuCode1_NoHyphen = new List<string>
            {
                "00000",
                "aaaaa",
                "000000",
                "0000"
            };

            TestFormatChecker.JpZipKuCode2_NoHyphen = new List<string>
            {
                "000",
                "aaa",
                "0000",
                "00"
            };
        }

        #endregion

            #region 郵便番号
            #region 郵便（区）番号
            /// <summary>IsJpZipCode</summary>
            private static void IsJpZipCode()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpZipCode");

            MyDebug.OutputDebugAndConsole("JpZipCode_Hyphen");
            foreach (string temp in TestFormatChecker.JpZipCode_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipKuCode1_Hyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode1_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipKuCode2_Hyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode2_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipCode_NoHyphen");
            foreach (string temp in TestFormatChecker.JpZipCode_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipKuCode1_NoHyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode1_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipKuCode2_NoHyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode2_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode(temp));
            }
        }

        #region 郵便（区）番号（ハイフン有り）
        /// <summary>IsJpZipCode_Hyphen</summary>
        private static void IsJpZipCode_Hyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpZipCode_Hyphen");

            MyDebug.OutputDebugAndConsole("JpZipCode_Hyphen");
            foreach (string temp in TestFormatChecker.JpZipCode_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode_Hyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipKuCode1_Hyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode1_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode_Hyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipKuCode2_Hyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode2_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode_Hyphen(temp));
            }
        }
        #endregion

        #region 郵便（区）番号（ハイフン無し）
        /// <summary>IsJpZipCode_NoHyphen</summary>
        private static void IsJpZipCode_NoHyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpZipCode_NoHyphen");

            MyDebug.OutputDebugAndConsole("JpZipCode_NoHyphen");
            foreach (string temp in TestFormatChecker.JpZipCode_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode_NoHyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipKuCode1_NoHyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode1_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode_NoHyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipKuCode2_NoHyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode2_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode_NoHyphen(temp));
            }
        }
        #endregion

        #region ７桁
        #region 郵便（区）番号
        /// <summary>IsJpZipCode7</summary>
        private static void IsJpZipCode7()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpZipCode7");

            MyDebug.OutputDebugAndConsole("JpZipCode_Hyphen");
            foreach (string temp in TestFormatChecker.JpZipCode_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode7(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipCode_NoHyphen");
            foreach (string temp in TestFormatChecker.JpZipCode_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode7(temp));
            }
        }
        #endregion

        #region 郵便（区）番号（ハイフン有り）
        /// <summary>IsJpZipCode7_Hyphen</summary>
        private static void IsJpZipCode7_Hyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpZipCode7_Hyphen");

            MyDebug.OutputDebugAndConsole("JpZipCode_Hyphen");
            foreach (string temp in TestFormatChecker.JpZipCode_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode7_Hyphen(temp));
            }
        }
        #endregion

        #region 郵便（区）番号（ハイフン無し）
        /// <summary>IsJpZipCode7_NoHyphen</summary>
        private static void IsJpZipCode7_NoHyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpZipCode7_NoHyphen");

            MyDebug.OutputDebugAndConsole("JpZipCode_NoHyphen");
            foreach (string temp in TestFormatChecker.JpZipCode_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode7_NoHyphen(temp));
            }
        }
        #endregion
        #endregion

        #region ５桁
        #region 郵便（区）番号
        /// <summary>IsJpZipCode_NoHyphen</summary>
        private static void IsJpZipCode5()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpZipCode5");
            
            MyDebug.OutputDebugAndConsole("JpZipKuCode1_Hyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode1_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode5(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipKuCode2_Hyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode2_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode5(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipKuCode1_NoHyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode1_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode5(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipKuCode2_NoHyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode2_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode5(temp));
            }
        }
        #endregion

        #region 郵便（区）番号（ハイフン有り）
        /// <summary>IsJpZipCode5_Hyphen</summary>
        private static void IsJpZipCode5_Hyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpZipCode5_Hyphen");

            MyDebug.OutputDebugAndConsole("JpZipKuCode1_Hyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode1_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode5(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipKuCode2_Hyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode2_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode5(temp));
            }
        }
        #endregion

        #region 郵便（区）番号（ハイフン無し）
        /// <summary>IsJpZipCode5_NoHyphen</summary>
        private static void IsJpZipCode5_NoHyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpZipCode5_NoHyphen");

            MyDebug.OutputDebugAndConsole("JpZipKuCode1_NoHyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode1_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode5(temp));
            }

            MyDebug.OutputDebugAndConsole("JpZipKuCode2_NoHyphen");
            foreach (string temp in TestFormatChecker.JpZipKuCode2_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpZipCode5(temp));
            }
        }
        #endregion
        #endregion

        #endregion
        #endregion
        #endregion
    }
}
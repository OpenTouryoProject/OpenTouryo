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
            TestFormatChecker.InitJpZipCodeTestCase();
            TestFormatChecker.InitJpTelephoneNumberTestCase();

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

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpTelephoneNumber();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpTelephoneNumber_Hyphen();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpTelephoneNumber_NoHyphen();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpFixedLinePhoneNumber();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpFixedLinePhoneNumber_Hyphen();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpFixedLinePhoneNumber_NoHyphen();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpCellularPhoneNumber();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpCellularPhoneNumber_Hyphen();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpCellularPhoneNumber_NoHyphen();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpIpPhoneNumber();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpIpPhoneNumber_Hyphen();

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestFormatChecker.IsJpIpPhoneNumber_NoHyphen();
        }
        #endregion

        #region private
        #region テストケース

        #region 郵便番号
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

        /// <summary>InitJpZipCodeTestCase</summary>
        private static void InitJpZipCodeTestCase()
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

        #region 電話番号
        /// <summary>JpFixedLinePhoneNumber_Hyphen</summary>
        private static List<string> JpFixedLinePhoneNumber_Hyphen = null;
        /// <summary>JpFixedLinePhoneNumber_NoHyphen</summary>
        private static List<string> JpFixedLinePhoneNumber_NoHyphen = null;

        /// <summary>JpCellularPhoneNumber_Hyphen</summary>
        private static List<string> JpCellularPhoneNumber_Hyphen = null;
        /// <summary>JpCellularPhoneNumber_NoHyphen</summary>
        private static List<string> JpCellularPhoneNumber_NoHyphen = null;

        /// <summary>JpIpPhoneNumber_Hyphen</summary>
        private static List<string> JpIpPhoneNumber_Hyphen = null;
        /// <summary>JpIpPhoneNumber_NoHyphen</summary>
        private static List<string> JpIpPhoneNumber_NoHyphen = null;

        /// <summary>InitJpTelephoneNumberTestCase</summary>
        private static void InitJpTelephoneNumberTestCase()
        {
            TestFormatChecker.JpFixedLinePhoneNumber_Hyphen = new List<string>
            {
                "09999-9-9999",
                "99999-9-9999",
                "0aaaa-a-aaaa",
                "099999-9-9999",
                "09999-99-9999",
                "09999-9-99999",
                "0999--999",
                "0999-9-9999",
                "09999--9999",
                "09999-9-999",
                "0999-99-9999",
                "9999-99-9999",
                "0aaa-aa-aaaa",
                "09999-999-99999",
                "09999-99-9999",
                "0999-999-9999",
                "0999-99-99999",
                "099-9-999",
                "099-99-9999",
                "0999-9-9999",
                "0999-99-999",
                "099-999-9999",
                "999-999-9999",
                "0aa-aaa-aaaa",
                "0999-9999-99999",
                "0999-999-9999",
                "099-9999-9999",
                "099-999-99999",
                "09-99-999",
                "09-999-9999",
                "099-99-9999",
                "099-999-999",
                "09-9999-9999",
                "99-9999-9999",
                "0a-aaaa-aaaa",
                "099-99999-99999",
                "099-9999-9999",
                "09-99999-9999",
                "09-9999-99999",
                "0-999-999",
                "0-9999-9999",
                "09-999-9999",
                "09-9999-999"
            };

            TestFormatChecker.JpFixedLinePhoneNumber_NoHyphen = new List<string>
            {
                "0999999999",
                "9999999999",
                "0aaaaaaaaa",
                "09999999999",
                "099999999"
            };

            TestFormatChecker.JpCellularPhoneNumber_Hyphen = new List<string>
            {
                "020-9999-9999",
                "929-9999-9999",
                "020-aaaa-aaaa",
                "0209-99999-99999",
                "0209-9999-9999",
                "020-99999-9999",
                "020-9999-99999",
                "02-999-999",
                "02-9999-9999",
                "020-999-9999",
                "020-9999-999",
                "060-9999-9999",
                "969-9999-9999",
                "060-aaaa-aaaa",
                "0609-99999-99999",
                "0609-9999-9999",
                "060-99999-9999",
                "060-9999-99999",
                "06-999-999",
                "06-9999-9999",
                "060-999-9999",
                "060-9999-999",
                "070-9999-9999",
                "979-9999-9999",
                "070-aaaa-aaaa",
                "0709-99999-99999",
                "0709-9999-9999",
                "070-99999-9999",
                "070-9999-99999",
                "07-999-999",
                "07-9999-9999",
                "070-999-9999",
                "070-9999-999",
                "080-9999-9999",
                "989-9999-9999",
                "080-aaaa-aaaa",
                "0809-99999-99999",
                "0809-9999-9999",
                "080-99999-9999",
                "080-9999-99999",
                "08-999-999",
                "08-9999-9999",
                "080-999-9999",
                "080-9999-999",
                "090-9999-9999",
                "999-9999-9999",
                "090-aaaa-aaaa",
                "0909-99999-99999",
                "0909-9999-9999",
                "090-99999-9999",
                "090-9999-99999",
                "09-999-999",
                "09-9999-9999",
                "090-999-9999",
                "090-9999-999",
            };

            TestFormatChecker.JpCellularPhoneNumber_NoHyphen = new List<string>
            {
                "02099999999",
                "92999999999",
                "020aaaaaaaa",
                "020999999999",
                "0209999999",
                "06099999999",
                "07099999999",
                "08099999999",
                "09099999999",
                "01099999999",
                "03099999999",
                "04099999999",
                "05099999999"
            };

            TestFormatChecker.JpIpPhoneNumber_Hyphen = new List<string>
            {
                "050-9999-9999",
                "959-9999-9999",
                "050-aaaa-aaaa",
                "0509-99999-99999",
                "0509-9999-9999",
                "050-99999-9999",
                "050-9999-99999",
                "05-999-999",
                "05-9999-9999",
                "050-999-9999",
                "050-9999-999",
                "9999999999999",
                "010-9999-9999",
                "020-9999-9999",
                "030-9999-9999",
                "040-9999-9999",
                "060-9999-9999",
                "070-9999-9999",
                "080-9999-9999",
                "090-9999-9999"
            };

            TestFormatChecker.JpIpPhoneNumber_NoHyphen = new List<string>
            {
                "05099999999",
                "95999999999",
                "050aaaaaaaa",
                "050999999999",
                "0509999999",
                "01099999999",
                "02099999999",
                "03099999999",
                "04099999999",
                "06099999999",
                "07099999999",
                "08099999999",
                "09099999999"
            };
        }
        #endregion

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

        #region ハイフン有り
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

        #region ハイフン無し
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

        #endregion

        #region ７桁 郵便（区）番号
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
        
        #region ハイフン有り
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

        #region ハイフン無し
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

        #region ５桁 郵便（区）番号
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

        #region ハイフン有り
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

        #region ハイフン無し
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

        #region 電話番号

        #region IsJpTelephoneNumber_XXX
        /// <summary>IsJpTelephoneNumber</summary>
        private static void IsJpTelephoneNumber()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpTelephoneNumber");

            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber(temp));
            }
            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber(temp));
            }

            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber(temp));
            }
            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber(temp));
            }

            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber(temp));
            }
            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber(temp));
            }
        }

        /// <summary>IsJpTelephoneNumber_Hyphen</summary>
        private static void IsJpTelephoneNumber_Hyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpTelephoneNumber_Hyphen");

            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber_Hyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber_Hyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber_Hyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber_Hyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber_Hyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber_Hyphen(temp));
            }
        }

        /// <summary>IsJpTelephoneNumber_NoHyphen</summary>
        private static void IsJpTelephoneNumber_NoHyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpTelephoneNumber_NoHyphen");

            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber_NoHyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber_NoHyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber_NoHyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber_NoHyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber_NoHyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpTelephoneNumber_NoHyphen(temp));
            }
        }
        #endregion

        #region IsJpXXXPhoneNumber

        #region 固定
        /// <summary>IsJpFixedLinePhoneNumber</summary>
        private static void IsJpFixedLinePhoneNumber()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpFixedLinePhoneNumber");

            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber(temp));
            }
            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber(temp));
            }

            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber(temp));
            }
            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber(temp));
            }

            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber(temp));
            }
            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber(temp));
            }
        }

        /// <summary>IsJpFixedLinePhoneNumber_Hyphen</summary>
        private static void IsJpFixedLinePhoneNumber_Hyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpFixedLinePhoneNumber_Hyphen");

            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(temp));
            }
        }

        /// <summary>IsJpFixedLinePhoneNumber_NoHyphen</summary>
        private static void IsJpFixedLinePhoneNumber_NoHyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpFixedLinePhoneNumber_NoHyphen");

            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(temp));
            }
        }
        #endregion

        #region 携帯
        /// <summary>IsJpCellularPhoneNumber</summary>
        private static void IsJpCellularPhoneNumber()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpCellularPhoneNumber");

            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber(temp));
            }
            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber(temp));
            }

            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber(temp));
            }
            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber(temp));
            }

            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber(temp));
            }
            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber(temp));
            }
        }

        /// <summary>IsJpCellularPhoneNumber_Hyphen</summary>
        private static void IsJpCellularPhoneNumber_Hyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpCellularPhoneNumber_Hyphen");

            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber_Hyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber_Hyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber_Hyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber_Hyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber_Hyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber_Hyphen(temp));
            }
        }

        /// <summary>IsJpCellularPhoneNumber_NoHyphen</summary>
        private static void IsJpCellularPhoneNumber_NoHyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpCellularPhoneNumber_NoHyphen");

            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber_NoHyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber_NoHyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber_NoHyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber_NoHyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber_NoHyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpCellularPhoneNumber_NoHyphen(temp));
            }
        }
        #endregion

        #region IP
        /// <summary>IsJpIpPhoneNumber</summary>
        private static void IsJpIpPhoneNumber()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpIpPhoneNumber");

            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber(temp));
            }
            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber(temp));
            }

            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber(temp));
            }
            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber(temp));
            }

            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber(temp));
            }
            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber(temp));
            }
        }

        /// <summary>IsJpIpPhoneNumber_Hyphen</summary>
        private static void IsJpIpPhoneNumber_Hyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpIpPhoneNumber_Hyphen");

            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber_Hyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber_Hyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber_Hyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber_Hyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber_Hyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber_Hyphen(temp));
            }
        }

        /// <summary>IsJpIpPhoneNumber_NoHyphen</summary>
        private static void IsJpIpPhoneNumber_NoHyphen()
        {
            MyDebug.OutputDebugAndConsole("TestFormatChecker.IsJpIpPhoneNumber_NoHyphen");

            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber_NoHyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpFixedLinePhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpFixedLinePhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber_NoHyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber_NoHyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpCellularPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpCellularPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber_NoHyphen(temp));
            }

            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_Hyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_Hyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber_NoHyphen(temp));
            }
            MyDebug.OutputDebugAndConsole("JpIpPhoneNumber_NoHyphen");
            foreach (string temp in TestFormatChecker.JpIpPhoneNumber_NoHyphen)
            {
                MyDebug.OutputDebugAndConsole(temp + ": " + FormatChecker.IsJpIpPhoneNumber_NoHyphen(temp));
            }
        }
        #endregion

        #endregion

        #endregion
        #endregion
    }
}
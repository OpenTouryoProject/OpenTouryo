//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//**********************************************************************************
//* クラス名        ：CustomEncodeTest
//* クラス日本語名  ：Test of the class to CustomEncode
//*
//* 作成者          ：Rituparna
//* 更新履歴        ：
//* 
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/08/2014   Rituparna      Testcode development for CustomEncode.
//*
//**********************************************************************************

#region Includes

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// touryo library
using Touryo.Infrastructure.Public.IO;

// testing framework
using NUnit.Framework;
using Touryo.Infrastructure.Public.Str;
using System.Data;

#endregion

namespace Public.Test.Str
{
    [TestFixture]
    public class CustomEncodeTest
    {
        [TestFixtureSetUp]
        public void Init()
        {
            // This is a test pre-processing.
            // This is done only once at the beginning.
        }

        /// <summary>Test case pre-processing.</summary>
        [SetUp]
        public void SetUp()
        {
            // This is a test case pre-processing.
            // It runs for each test case.
        }

        #region Test data

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method GetEncodings.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfGetEncodingsTest
        {
            get
            {
                this.SetUp();
                yield return new TestCaseData("TestID-000N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method StringToByte.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfStringToByteTest
        {
            get
            {
                this.SetUp();

                //passing  codePageNum as proper Unicode value or null value
                yield return new TestCaseData("TestID-000N", "abcde", CustomEncode.UTF_16LE);
                yield return new TestCaseData("TestID-001N", "abcde", CustomEncode.UTF_16BE);
                yield return new TestCaseData("TestID-002N", "abcde", CustomEncode.UTF_7);
                yield return new TestCaseData("TestID-003N", "abcde", CustomEncode.UTF_8);
                yield return new TestCaseData("TestID-004N", "abcde", 1200);
                yield return new TestCaseData("TestID-005N", "abcde", 1201);
                yield return new TestCaseData("TestID-006N", "abcde", 65000);
                yield return new TestCaseData("TestID-007N", "abcde", 65001);
                yield return new TestCaseData("TestID-008N", "abcde", null);
                yield return new TestCaseData("TestID-009L", string.Empty, null);
                yield return new TestCaseData("TestID-010N", "11", CustomEncode.UTF_16LE);

                //passing wrong value as codePageNum 
                yield return new TestCaseData("TestID-011A", "abcde", 1).Throws(typeof(ArgumentException));

                //passing interger value instead of string value
                yield return new TestCaseData("TestID-012A", 11, CustomEncode.UTF_16LE).Throws(typeof(ArgumentException));

                //passing integer value instead of string value and passing wrong value as codePageNum
                yield return new TestCaseData("TestID-013A", 11, 1).Throws(typeof(ArgumentException));

                //passing null value instead of string value
                yield return new TestCaseData("TestID-014A", null, CustomEncode.UTF_16LE).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-015A", null, CustomEncode.UTF_16BE).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-016A", null, CustomEncode.UTF_7).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-017A", null, CustomEncode.UTF_8).Throws(typeof(ArgumentNullException));

                //passing empty string 
                yield return new TestCaseData("TestID-018L", string.Empty, CustomEncode.UTF_16LE);
                yield return new TestCaseData("TestID-019L", string.Empty, CustomEncode.UTF_16BE);
                yield return new TestCaseData("TestID-020L", string.Empty, CustomEncode.UTF_7);
                yield return new TestCaseData("TestID-021L", string.Empty, CustomEncode.UTF_8);

                //passing empty value in codePageNum
                yield return new TestCaseData("TestID-022A", "abcde", string.Empty).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-023A", "11", string.Empty).Throws(typeof(ArgumentException));

                //passing  codePageNum as proper JIS value or その他 value
                yield return new TestCaseData("TestID-024N", "abcde", CustomEncode.x_mac_japanese);
                yield return new TestCaseData("TestID-025N", "abcde", CustomEncode.us_ascii);
                yield return new TestCaseData("TestID-026N", "abcde", CustomEncode.EUC_JP);
                yield return new TestCaseData("TestID-027N", "abcde", CustomEncode.shift_jis);
                yield return new TestCaseData("TestID-028N", "abcde", CustomEncode.ISO_2022_JP);
                yield return new TestCaseData("TestID-029N", "abcde", CustomEncode._iso_2022_jp_Dollar_ESC);
                yield return new TestCaseData("TestID-030N", "abcde", CustomEncode._iso_2022_jp_Dollar_SIO);
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method ByteToString.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfByteToStringTest
        {
            get
            {
                this.SetUp();

                //passing  codePageNum as proper Unicode value or proper JIS value or proper その他 value 
                yield return new TestCaseData("TestID-000N", Encoding.UTF8.GetBytes("abcde"), CustomEncode.UTF_8);
                yield return new TestCaseData("TestID-001N", Encoding.UTF7.GetBytes("abcde"), CustomEncode.UTF_7);
                yield return new TestCaseData("TestID-002N", Encoding.UTF32.GetBytes("abcde"), CustomEncode.UTF_16LE);
                yield return new TestCaseData("TestID-002N", Encoding.UTF32.GetBytes("abcde"), CustomEncode.UTF_16BE);
                yield return new TestCaseData("TestID-003N", Encoding.UTF8.GetBytes("abcde"), CustomEncode.x_mac_japanese);
                yield return new TestCaseData("TestID-004N", Encoding.UTF7.GetBytes("abcde"), CustomEncode.x_mac_japanese);
                yield return new TestCaseData("TestID-005N", Encoding.UTF32.GetBytes("abcde"), CustomEncode.x_mac_japanese);
                yield return new TestCaseData("TestID-006N", Encoding.UTF8.GetBytes("abcde"), CustomEncode.us_ascii);
                yield return new TestCaseData("TestID-007N", Encoding.UTF7.GetBytes("abcde"), CustomEncode.us_ascii);
                yield return new TestCaseData("TestID-008N", Encoding.UTF32.GetBytes("abcde"), CustomEncode.us_ascii);
                yield return new TestCaseData("TestID-009N", Encoding.UTF8.GetBytes("abcde"), CustomEncode.EUC_JP);
                yield return new TestCaseData("TestID-010N", Encoding.UTF7.GetBytes("abcde"), CustomEncode.EUC_JP);
                yield return new TestCaseData("TestID-011N", Encoding.UTF32.GetBytes("abcde"), CustomEncode.EUC_JP);
                yield return new TestCaseData("TestID-012N", Encoding.UTF8.GetBytes("abcde"), CustomEncode.shift_jis);
                yield return new TestCaseData("TestID-013N", Encoding.UTF7.GetBytes("abcde"), CustomEncode.shift_jis);
                yield return new TestCaseData("TestID-014N", Encoding.UTF32.GetBytes("abcde"), CustomEncode.shift_jis);
                yield return new TestCaseData("TestID-015N", Encoding.UTF8.GetBytes("abcde"), CustomEncode.ISO_2022_JP);
                yield return new TestCaseData("TestID-016N", Encoding.UTF7.GetBytes("abcde"), CustomEncode.ISO_2022_JP);
                yield return new TestCaseData("TestID-017N", Encoding.UTF32.GetBytes("abcde"), CustomEncode.ISO_2022_JP);
                yield return new TestCaseData("TestID-018N", Encoding.UTF8.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC);
                yield return new TestCaseData("TestID-019N", Encoding.UTF7.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC);
                yield return new TestCaseData("TestID-020N", Encoding.UTF32.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC);
                yield return new TestCaseData("TestID-021N", Encoding.UTF8.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO);
                yield return new TestCaseData("TestID-022N", Encoding.UTF7.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO);
                yield return new TestCaseData("TestID-023N", Encoding.UTF32.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO);
                yield return new TestCaseData("TestID-024N", Encoding.ASCII.GetBytes("abcde"), CustomEncode.x_mac_japanese);
                yield return new TestCaseData("TestID-025N", Encoding.ASCII.GetBytes("abcde"), CustomEncode.us_ascii);
                yield return new TestCaseData("TestID-026N", Encoding.ASCII.GetBytes("abcde"), CustomEncode.EUC_JP);
                yield return new TestCaseData("TestID-027N", Encoding.ASCII.GetBytes("abcde"), CustomEncode.shift_jis);
                yield return new TestCaseData("TestID-028N", Encoding.ASCII.GetBytes("abcde"), CustomEncode.ISO_2022_JP);
                yield return new TestCaseData("TestID-029N", Encoding.ASCII.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC);
                yield return new TestCaseData("TestID-030N", Encoding.ASCII.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO);
                yield return new TestCaseData("TestID-031N", Encoding.ASCII.GetBytes("abcde"), CustomEncode.UTF_8);
                yield return new TestCaseData("TestID-032N", Encoding.ASCII.GetBytes("abcde"), CustomEncode.UTF_7);
                yield return new TestCaseData("TestID-033N", Encoding.Unicode.GetBytes("abcde"), CustomEncode.x_mac_japanese);
                yield return new TestCaseData("TestID-034N", Encoding.Unicode.GetBytes("abcde"), CustomEncode.us_ascii);
                yield return new TestCaseData("TestID-035N", Encoding.Unicode.GetBytes("abcde"), CustomEncode.EUC_JP);
                yield return new TestCaseData("TestID-036N", Encoding.Unicode.GetBytes("abcde"), CustomEncode.shift_jis);
                yield return new TestCaseData("TestID-037N", Encoding.Unicode.GetBytes("abcde"), CustomEncode.ISO_2022_JP);
                yield return new TestCaseData("TestID-038N", Encoding.Unicode.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC);
                yield return new TestCaseData("TestID-039N", Encoding.Unicode.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO);
                yield return new TestCaseData("TestID-040N", Encoding.Unicode.GetBytes("abcde"), CustomEncode.UTF_8);
                yield return new TestCaseData("TestID-041N", Encoding.Unicode.GetBytes("あいうえお"), CustomEncode.UTF_7);
                yield return new TestCaseData("TestID-042N", Encoding.Unicode.GetBytes("abcde"), CustomEncode.UTF_16LE);
                yield return new TestCaseData("TestID-043N", Encoding.Unicode.GetBytes("abcde"), CustomEncode.UTF_16BE);
                yield return new TestCaseData("TestID-044N", Encoding.Default.GetBytes("abcde"), CustomEncode.x_mac_japanese);
                yield return new TestCaseData("TestID-045N", Encoding.Default.GetBytes("abcde"), CustomEncode.us_ascii);
                yield return new TestCaseData("TestID-046N", Encoding.Default.GetBytes("abcde"), CustomEncode.EUC_JP);
                yield return new TestCaseData("TestID-047N", Encoding.Default.GetBytes("abcde"), CustomEncode.shift_jis);
                yield return new TestCaseData("TestID-048N", Encoding.Default.GetBytes("abcde"), CustomEncode.ISO_2022_JP);
                yield return new TestCaseData("TestID-049N", Encoding.Default.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC);
                yield return new TestCaseData("TestID-050N", Encoding.Default.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO);
                yield return new TestCaseData("TestID-051N", Encoding.Default.GetBytes("abcde"), CustomEncode.UTF_8);
                yield return new TestCaseData("TestID-052N", Encoding.Default.GetBytes("abcde"), CustomEncode.UTF_7);
                yield return new TestCaseData("TestID-058N", Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode.x_mac_japanese);
                yield return new TestCaseData("TestID-059N", Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode.us_ascii);
                yield return new TestCaseData("TestID-060N", Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode.EUC_JP);
                yield return new TestCaseData("TestID-061N", Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode.shift_jis);
                yield return new TestCaseData("TestID-062N", Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode.ISO_2022_JP);
                yield return new TestCaseData("TestID-063N", Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC);
                yield return new TestCaseData("TestID-064N", Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO);
                yield return new TestCaseData("TestID-065N", Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode.UTF_8);

                //passing  codePageNum as null value 
                yield return new TestCaseData("TestID-066N", Encoding.UTF8.GetBytes("abcde"), null);
                yield return new TestCaseData("TestID-067N", Encoding.UTF7.GetBytes("abcde"), null);
                yield return new TestCaseData("TestID-068N", Encoding.UTF32.GetBytes("abcde"), null);
                yield return new TestCaseData("TestID-069N", Encoding.ASCII.GetBytes("abcde"), null);
                yield return new TestCaseData("TestID-070N", Encoding.Default.GetBytes("abcde"), null);
                yield return new TestCaseData("TestID-071N", Encoding.BigEndianUnicode.GetBytes("abcde"), null);
                yield return new TestCaseData("TestID-072N", Encoding.Unicode.GetBytes("abcde"), null);

                //passing  codePageNum as string.empty value 
                yield return new TestCaseData("TestID-073A", Encoding.UTF8.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-074A", Encoding.UTF7.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-075A", Encoding.UTF32.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-076A", Encoding.ASCII.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-077A", Encoding.Default.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-078A", Encoding.BigEndianUnicode.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-079A", Encoding.Unicode.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException));

                //passing  byte value as string.empty value 
                yield return new TestCaseData("TestID-080A", string.Empty, CustomEncode.us_ascii).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-081L", Encoding.UTF8.GetBytes(string.Empty), CustomEncode.us_ascii);
                yield return new TestCaseData("TestID-082A", string.Empty, CustomEncode.UTF_16BE).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-083L", Encoding.UTF8.GetBytes(string.Empty), CustomEncode.UTF_16BE);
                yield return new TestCaseData("TestID-084A", string.Empty, CustomEncode.UTF_16LE).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-085L", Encoding.UTF8.GetBytes(string.Empty), CustomEncode.UTF_16LE);
                yield return new TestCaseData("TestID-086A", string.Empty, CustomEncode.UTF_7).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-087L", Encoding.UTF8.GetBytes(string.Empty), CustomEncode.UTF_7);
                yield return new TestCaseData("TestID-088A", string.Empty, CustomEncode.UTF_8).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-089L", Encoding.UTF8.GetBytes(string.Empty), CustomEncode.UTF_8);
                yield return new TestCaseData("TestID-090A", string.Empty, CustomEncode._iso_2022_jp_Dollar_ESC).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-091L", Encoding.UTF8.GetBytes(string.Empty), CustomEncode._iso_2022_jp_Dollar_ESC);
                yield return new TestCaseData("TestID-092A", string.Empty, CustomEncode._iso_2022_jp_Dollar_SIO).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-093L", Encoding.UTF8.GetBytes(string.Empty), CustomEncode._iso_2022_jp_Dollar_SIO);
                yield return new TestCaseData("TestID-094A", string.Empty, CustomEncode.shift_jis).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-095L", Encoding.UTF8.GetBytes(string.Empty), CustomEncode.shift_jis);
                yield return new TestCaseData("TestID-096A", string.Empty, CustomEncode.ISO_2022_JP).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-097L", Encoding.UTF8.GetBytes(string.Empty), CustomEncode.ISO_2022_JP);
                yield return new TestCaseData("TestID-098A", string.Empty, CustomEncode.x_mac_japanese).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-099L", Encoding.UTF8.GetBytes(string.Empty), CustomEncode.x_mac_japanese);
                yield return new TestCaseData("TestID-100A", string.Empty, CustomEncode.EUC_JP).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-101L", Encoding.UTF8.GetBytes(string.Empty), CustomEncode.EUC_JP);
                yield return new TestCaseData("TestID-102A", string.Empty, null).Throws(typeof(ArgumentException));
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method HtmlEncode.
        /// </summary>
        public IEnumerable<TestCaseData> TestHtmlEncodeTest
        {
            get
            {
                this.SetUp();
                yield return new TestCaseData("TestID-000N", "abcde");
                yield return new TestCaseData("TestID-001N", "あいうえお");
                yield return new TestCaseData("TestID-002N", "1");
                yield return new TestCaseData("TestID-003A", 1).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-004N", "abcde" + "あいうえお");
                yield return new TestCaseData("TestID-005N", null);
                yield return new TestCaseData("TestID-006L", string.Empty);
                yield return new TestCaseData("TestID-007N", "This string contains the unicode character Pi (\u03a0)");
                yield return new TestCaseData("TestID-008N", "&lt;root&gt;abcde&lt;/root&gt;");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method HtmlDecode.
        /// </summary>
        public IEnumerable<TestCaseData> TestHtmlDecodeTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", "abcde");
                yield return new TestCaseData("TestID-001N", "あいうえお");
                yield return new TestCaseData("TestID-002N", "1");
                yield return new TestCaseData("TestID-003A", 1).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-004N", "abcde" + "あいうえお");
                yield return new TestCaseData("TestID-005N", null);
                yield return new TestCaseData("TestID-006L", string.Empty);
                yield return new TestCaseData("TestID-007N", "<root>abcde</root>");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method UrlEncode.
        /// </summary>
        public IEnumerable<TestCaseData> TestUrlEncodeTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", "hello");
                yield return new TestCaseData("TestID-001N", "google.com");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method UrlDecode.
        /// </summary>
        public IEnumerable<TestCaseData> TestUrlDecodeTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", "hello");
                yield return new TestCaseData("TestID-001N", "google.com");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method FormHexString.
        /// </summary>
        public IEnumerable<TestCaseData> TestFormHexStringTest
        {
            get
            {
                this.SetUp();
                string str = "00 01 00 00 00 FF FF FF FF 01 00 00 00 00 00 00 00 06 01 00 00 00 0E 59 61 68 6F 6F 6F 6F 6F 6F 6F 6F 6F 6F 6F 0B";
                yield return new TestCaseData("TestID-000N", str);
                yield return new TestCaseData("TestID-001N", "01");
                yield return new TestCaseData("TestID-002N", "0A 0B 0C");
                yield return new TestCaseData("TestID-003A", 1).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-004A", "abcde" + "あいうえお").Throws(typeof(FormatException)); ;
                yield return new TestCaseData("TestID-005A", null).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-006A", string.Empty).Throws(typeof(ArgumentOutOfRangeException));
                yield return new TestCaseData("TestID-007N", "AB 0C DE");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method ToHexString.
        /// </summary>
        public IEnumerable<TestCaseData> TestToHexStringTest
        {
            get
            {
                this.SetUp();

                var sevenItems = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
                string str = "00 01 00 00 00 FF FF FF FF 01 00 00 00 00 00 00 00 06 01 00 00 00 0E 59 61 68 6F 6F 6F 6F 6F 6F 6F 6F 6F 6F 6F 0B";
                byte[] data = { };
                yield return new TestCaseData("TestID-000N", Encoding.UTF8.GetBytes("abcde"));
                yield return new TestCaseData("TestID-001N", Encoding.ASCII.GetBytes(str));
                yield return new TestCaseData("TestID-002N", Encoding.Unicode.GetBytes("abcde" + "あいうえお"));
                yield return new TestCaseData("TestID-003A", null).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-004A", string.Empty).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-005N", sevenItems);
                yield return new TestCaseData("TestID-006N", data).Throws(typeof(ArgumentOutOfRangeException));
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method FromBase64String.
        /// </summary>
        public IEnumerable<TestCaseData> TestFromBase64StringTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", "AAECBAqQIEA=");
                yield return new TestCaseData("TestID-001A", "abcde=").Throws(typeof(FormatException));
                yield return new TestCaseData("TestID-002N", "YYYYYWJjZGUT");
                yield return new TestCaseData("TestID-003N", string.Empty);
                yield return new TestCaseData("TestID-004A", null).Throws(typeof(System.ArgumentNullException));
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method ToBase64String.
        /// </summary>
        public IEnumerable<TestCaseData> TestToBase64StringTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", Encoding.ASCII.GetBytes("abcde"));
                yield return new TestCaseData("TestID-001N", Encoding.UTF8.GetBytes("abcde"));
                yield return new TestCaseData("TestID-002A", string.Empty).Throws(typeof(System.ArgumentException));
                yield return new TestCaseData("TestID-003A", null).Throws(typeof(System.ArgumentNullException));
                yield return new TestCaseData("TestID-004A", Encoding.UTF8.GetBytes(""));
            }
        }

        #endregion

        #region Test Code

        /// <summary>
        /// TestCasesOfGetEncodingsTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        [TestCaseSource("TestCasesOfGetEncodingsTest")]
        public static void GetEncodingsTest(string testCaseID)
        {
            System.Data.DataTable dt = CustomEncode.GetEncodings();
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("key", typeof(int));
            dt1.Columns.Add("value", typeof(string));

            Assert.AreNotEqual(dt, dt1);
            Assert.AreEqual(dt.Columns.GetType(), dt1.Columns.GetType());
            Assert.AreEqual(dt.Columns.Count, dt1.Columns.Count);
            Assert.AreNotEqual(dt.Rows.Count, dt1.Rows.Count);
        }

        /// <summary>
        /// StringToByteTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="str">str</param>
        /// <param name="codePageNum">codePageNum</param>
        [TestCaseSource("TestCasesOfStringToByteTest")]
        public static void StringToByteTest(string testCaseID, string str, int codePageNum)
        {
            try
            {
                // Convert to byte using the components touryo.
                byte[] abyt = CustomEncode.StringToByte(str, codePageNum);

                // Convert to string using the components touryo.
                string strValue = CustomEncode.ByteToString(abyt, codePageNum);

                Assert.AreNotSame(str, abyt);

                //Check whether it is converted into the original string.
                Assert.AreEqual(str, strValue);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestCasesOfByteToStringTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="abyt">abyt</param>
        /// <param name="codePageNum">codePageNum</param>
        [TestCaseSource("TestCasesOfByteToStringTest")]
        public static void ByteToStringTest(string testCaseID, byte[] abyt, int codePageNum)
        {
            try
            {
                // Convert to string using the components touryo.
                string str = CustomEncode.ByteToString(abyt, codePageNum);

                // Convert to byte using the components touryo.
                byte[] byteValue = CustomEncode.StringToByte(str, codePageNum);

                Assert.AreNotSame(abyt, str);
                //Check whether it is converted into the original byte.
                Assert.AreEqual(abyt, byteValue);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// HtmlEncodeTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="str">str</param>
        [TestCaseSource("TestHtmlEncodeTest")]
        public static void HtmlEncodeTest(string testCaseID, string str)
        {
            try
            {
                // Performs decryption using the components touryo.
                string strValue = CustomEncode.HtmlDecode(str);

                // Performs encryption using the components touryo.
                string str1 = CustomEncode.HtmlEncode(strValue);

                // Check whether it is encrypted into the original string.
                Assert.AreEqual(str, str1);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// HtmlDecodeTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="str">str</param>
        [TestCaseSource("TestHtmlDecodeTest")]
        public static void HtmlDecodeTest(string testCaseID, string str)
        {
            try
            {
                // Performs encryption using the components touryo.
                string strValue = CustomEncode.HtmlEncode(str);

                // Performs decrypted using the components touryo.
                string str1 = CustomEncode.HtmlDecode(strValue);

                // Check whether it is decrypted into the original string.
                Assert.AreEqual(str, str1);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestUrlEncodeTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="str">str</param>
        [TestCaseSource("TestUrlEncodeTest")]
        public static void UrlEncodeTest(string testCaseID, string str)
        {
            try
            {
                // Performs decryption using the components touryo.
                string strValue = CustomEncode.UrlDecode(str);

                // Performs encryption using the components touryo.
                string str1 = CustomEncode.UrlEncode(strValue);

                // Check whether it is decrypted into the original string.
                Assert.AreEqual(str, str1);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestUrlDecodeTestMethod
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="str">str</param>
        [TestCaseSource("TestUrlDecodeTest")]
        public static void UrlDecodeTest(string testCaseID, string str)
        {
            try
            {
                // Performs encryption using the components touryo.
                string strValue = CustomEncode.UrlEncode(str);

                // Performs decrypted using the components touryo.
                string str1 = CustomEncode.UrlDecode(strValue);

                // Check whether it is decrypted into the original string.
                Assert.AreEqual(str, str1);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestFormHexStringTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="str">str</param>
        [TestCaseSource("TestFormHexStringTest")]
        public void FormHexStringTest(string testCaseID, string str)
        {
            try
            {
                //convert to byte using components of Touryo
                byte[] results = CustomEncode.FormHexString(str);

                //convert to string unsing components of Touryo
                string strValue = CustomEncode.ToHexString(results);

                // Check whether the two strings are equal.
                Assert.AreEqual(str, strValue);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestToHexStringTest Method
        /// </summary>
        /// <param name="testCaseID"></param>
        /// <param name="bytes"></param>
        [TestCaseSource("TestToHexStringTest")]
        public void ToHexStringTest(string testCaseID, byte[] bytes)
        {
            try
            {
                //convert to string using components of Touryo
                string strValue = CustomEncode.ToHexString(bytes);

                //convert to byte unsing components of Touryo
                byte[] bytesValue = CustomEncode.FormHexString(strValue);

                // Check whether it is converted to original byte.
                Assert.AreEqual(bytes, bytesValue);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestFromBase64StringTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="base64Str">base64Str</param>
        [TestCaseSource("TestFromBase64StringTest")]
        public static void FromBase64StringTest(string testCaseID, string base64Str)
        {
            string base64Decoded;
            //convert to byte using components of Touryo
            byte[] data = CustomEncode.FromBase64String(base64Str);

            //convert to string unsing components of Touryo
            base64Decoded = CustomEncode.ToBase64String(data);

            //check whether the two strings are equal.
            Assert.AreEqual(base64Str, base64Decoded);
        }

        /// <summary>
        /// TestToBase64StringTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="aryByt">aryByt</param>
        [TestCaseSource("TestToBase64StringTest")]
        public static void ToBase64StringTest(string testCaseID, byte[] aryByt)
        {
            string base64Decoded;
            //convert to byte using components of Touryo
            base64Decoded = CustomEncode.ToBase64String(aryByt);

            //convert to string using components of Touryo
            byte[] data = CustomEncode.FromBase64String(base64Decoded);

            //check whether it is converted to original byte
            Assert.AreEqual(aryByt, data);
        }

        #endregion
        /// <summary>Test case post-processing.</summary>
        [TearDown]
        public void TearDown()
        {
            // This is a test case post-processing.
            // It runs for each test case.
        }

        /// <summary>Test post-processing.</summary>
        [TestFixtureTearDown]
        public void CleanUp()
        {
            // This is a test post-processing.
            // This is done only once at the ending.
        }
    }
}

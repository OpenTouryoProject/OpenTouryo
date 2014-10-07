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
//*  Date:        Author:          Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/08/2014   Rituparna        Testcode development for CustomEncode.
//*  08/11/2014   Rituparna        Added TestcaseID using SetName method as per Nishino-San comments
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
               yield return new TestCaseData().SetName("TestID-000N");
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
                //passing  codePageNum as proper Unicode value or null value
                yield return new TestCaseData("abcde", CustomEncode.UTF_16LE).SetName("TestID-000N");
                yield return new TestCaseData("abcde", CustomEncode.UTF_16BE).SetName("TestID-001N");
                yield return new TestCaseData("abcde", CustomEncode.UTF_7).SetName("TestID-002N");
                yield return new TestCaseData("abcde", CustomEncode.UTF_8).SetName("TestID-003N");
                yield return new TestCaseData("abcde", 1200).SetName("TestID-004N");
                yield return new TestCaseData("abcde", 1201).SetName("TestID-005N");
                yield return new TestCaseData("abcde", 65000).SetName("TestID-006N");
                yield return new TestCaseData("abcde", 65001).SetName("TestID-007N");
                yield return new TestCaseData("abcde", null).SetName("TestID-008N");
                yield return new TestCaseData(string.Empty, null).SetName("TestID-009N");
                yield return new TestCaseData("11", CustomEncode.UTF_16LE).SetName("TestID-010N");

                //passing wrong value as codePageNum 
                yield return new TestCaseData("abcde", 1).Throws(typeof(ArgumentException)).SetName("TestID-011N");

                //passing interger value instead of string value
                yield return new TestCaseData(11, CustomEncode.UTF_16LE).Throws(typeof(ArgumentException)).SetName("TestID-012N");

                //passing integer value instead of string value and passing wrong value as codePageNum
                yield return new TestCaseData(11, 1).Throws(typeof(ArgumentException)).SetName("TestID-013N");

                //passing null value instead of string value
                yield return new TestCaseData(null, CustomEncode.UTF_16LE).Throws(typeof(ArgumentNullException)).SetName("TestID-014A");
                yield return new TestCaseData(null, CustomEncode.UTF_16BE).Throws(typeof(ArgumentNullException)).SetName("TestID-015A");
                yield return new TestCaseData(null, CustomEncode.UTF_7).Throws(typeof(ArgumentNullException)).SetName("TestID-016A");
                yield return new TestCaseData(null, CustomEncode.UTF_8).Throws(typeof(ArgumentNullException)).SetName("TestID-017A");

                //passing empty string 
                yield return new TestCaseData(string.Empty, CustomEncode.UTF_16LE).SetName("TestID-018N");
                yield return new TestCaseData(string.Empty, CustomEncode.UTF_16BE).SetName("TestID-019N");
                yield return new TestCaseData(string.Empty, CustomEncode.UTF_7).SetName("TestID-020N");
                yield return new TestCaseData(string.Empty, CustomEncode.UTF_8).SetName("TestID-021N");

                //passing empty value in codePageNum
                yield return new TestCaseData("abcde", string.Empty).Throws(typeof(ArgumentException)).SetName("TestID-022A");
                yield return new TestCaseData("11", string.Empty).Throws(typeof(ArgumentException)).SetName("TestID-023A");

                //passing  codePageNum as proper JIS value or その他 value
                yield return new TestCaseData("abcde", CustomEncode.x_mac_japanese).SetName("TestID-024N");
                yield return new TestCaseData("abcde", CustomEncode.us_ascii).SetName("TestID-025N");
                yield return new TestCaseData("abcde", CustomEncode.EUC_JP).SetName("TestID-026N");
                yield return new TestCaseData("abcde", CustomEncode.shift_jis).SetName("TestID-027N");
                yield return new TestCaseData("abcde", CustomEncode.ISO_2022_JP).SetName("TestID-028N");
                yield return new TestCaseData("abcde", CustomEncode._iso_2022_jp_Dollar_ESC).SetName("TestID-029N");
                yield return new TestCaseData("abcde", CustomEncode._iso_2022_jp_Dollar_SIO).SetName("TestID-030N");
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
                //passing  codePageNum as proper Unicode value or proper JIS value or proper その他 value 
                yield return new TestCaseData(Encoding.UTF8.GetBytes("abcde"), CustomEncode.UTF_8).SetName("TestID-000N");
                yield return new TestCaseData(Encoding.UTF7.GetBytes("abcde"), CustomEncode.UTF_7).SetName("TestID-001N");
                yield return new TestCaseData(Encoding.UTF32.GetBytes("abcde"), CustomEncode.UTF_16LE).SetName("TestID-002N");
                yield return new TestCaseData(Encoding.UTF32.GetBytes("abcde"), CustomEncode.UTF_16BE).SetName("TestID-003N");
                yield return new TestCaseData(Encoding.UTF8.GetBytes("abcde"), CustomEncode.x_mac_japanese).SetName("TestID-004N");
                yield return new TestCaseData(Encoding.UTF7.GetBytes("abcde"), CustomEncode.x_mac_japanese).SetName("TestID-005N");
                yield return new TestCaseData(Encoding.UTF32.GetBytes("abcde"), CustomEncode.x_mac_japanese).SetName("TestID-006N");
                yield return new TestCaseData(Encoding.UTF8.GetBytes("abcde"), CustomEncode.us_ascii).SetName("TestID-007N");
                yield return new TestCaseData(Encoding.UTF7.GetBytes("abcde"), CustomEncode.us_ascii).SetName("TestID-008N");
                yield return new TestCaseData(Encoding.UTF32.GetBytes("abcde"), CustomEncode.us_ascii).SetName("TestID-009N");
                yield return new TestCaseData(Encoding.UTF8.GetBytes("abcde"), CustomEncode.EUC_JP).SetName("TestID-010N");
                yield return new TestCaseData(Encoding.UTF7.GetBytes("abcde"), CustomEncode.EUC_JP).SetName("TestID-011N");
                yield return new TestCaseData(Encoding.UTF32.GetBytes("abcde"), CustomEncode.EUC_JP).SetName("TestID-012N");
                yield return new TestCaseData(Encoding.UTF8.GetBytes("abcde"), CustomEncode.shift_jis).SetName("TestID-013N");
                yield return new TestCaseData(Encoding.UTF7.GetBytes("abcde"), CustomEncode.shift_jis).SetName("TestID-014N");
                yield return new TestCaseData(Encoding.UTF32.GetBytes("abcde"), CustomEncode.shift_jis).SetName("TestID-015N");
                yield return new TestCaseData(Encoding.UTF8.GetBytes("abcde"), CustomEncode.ISO_2022_JP).SetName("TestID-016N");
                yield return new TestCaseData(Encoding.UTF7.GetBytes("abcde"), CustomEncode.ISO_2022_JP).SetName("TestID-017N");
                yield return new TestCaseData(Encoding.UTF32.GetBytes("abcde"), CustomEncode.ISO_2022_JP).SetName("TestID-018N");
                yield return new TestCaseData(Encoding.UTF8.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC).SetName("TestID-019N");
                yield return new TestCaseData(Encoding.UTF7.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC).SetName("TestID-020N");
                yield return new TestCaseData(Encoding.UTF32.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC).SetName("TestID-021N");
                yield return new TestCaseData(Encoding.UTF8.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO).SetName("TestID-022N");
                yield return new TestCaseData(Encoding.UTF7.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO).SetName("TestID-023N");
                yield return new TestCaseData(Encoding.UTF32.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO).SetName("TestID-024N");
                yield return new TestCaseData(Encoding.ASCII.GetBytes("abcde"), CustomEncode.x_mac_japanese).SetName("TestID-025N");
                yield return new TestCaseData(Encoding.ASCII.GetBytes("abcde"), CustomEncode.us_ascii).SetName("TestID-026N");
                yield return new TestCaseData(Encoding.ASCII.GetBytes("abcde"), CustomEncode.EUC_JP).SetName("TestID-027N");
                yield return new TestCaseData(Encoding.ASCII.GetBytes("abcde"), CustomEncode.shift_jis).SetName("TestID-028N");
                yield return new TestCaseData(Encoding.ASCII.GetBytes("abcde"), CustomEncode.ISO_2022_JP).SetName("TestID-029N");
                yield return new TestCaseData(Encoding.ASCII.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC).SetName("TestID-030N");
                yield return new TestCaseData(Encoding.ASCII.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO).SetName("TestID-031N");
                yield return new TestCaseData(Encoding.ASCII.GetBytes("abcde"), CustomEncode.UTF_8).SetName("TestID-032N");
                yield return new TestCaseData(Encoding.ASCII.GetBytes("abcde"), CustomEncode.UTF_7).SetName("TestID-033N");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("abcde"), CustomEncode.x_mac_japanese).SetName("TestID-034N");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("abcde"), CustomEncode.us_ascii).SetName("TestID-035N");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("abcde"), CustomEncode.EUC_JP).SetName("TestID-036N");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("abcde"), CustomEncode.shift_jis).SetName("TestID-037N");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("abcde"), CustomEncode.ISO_2022_JP).SetName("TestID-038N");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC).SetName("TestID-039N");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO).SetName("TestID-040N");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("abcde"), CustomEncode.UTF_8).SetName("TestID-041N");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("あいうえお"), CustomEncode.UTF_7).SetName("TestID-042N");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("abcde"), CustomEncode.UTF_16LE).SetName("TestID-043N");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("abcde"), CustomEncode.UTF_16BE).SetName("TestID-044N");
                yield return new TestCaseData(Encoding.Default.GetBytes("abcde"), CustomEncode.x_mac_japanese).SetName("TestID-045N");
                yield return new TestCaseData(Encoding.Default.GetBytes("abcde"), CustomEncode.us_ascii).SetName("TestID-046N");
                yield return new TestCaseData(Encoding.Default.GetBytes("abcde"), CustomEncode.EUC_JP).SetName("TestID-047N");
                yield return new TestCaseData(Encoding.Default.GetBytes("abcde"), CustomEncode.shift_jis).SetName("TestID-048N"); ;
                yield return new TestCaseData(Encoding.Default.GetBytes("abcde"), CustomEncode.ISO_2022_JP).SetName("TestID-049N"); ;
                yield return new TestCaseData(Encoding.Default.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC).SetName("TestID-050N");
                yield return new TestCaseData(Encoding.Default.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO).SetName("TestID-051N");
                yield return new TestCaseData(Encoding.Default.GetBytes("abcde"), CustomEncode.UTF_8).SetName("TestID-052N");
                yield return new TestCaseData(Encoding.Default.GetBytes("abcde"), CustomEncode.UTF_7).SetName("TestID-053N");
                yield return new TestCaseData(Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode.x_mac_japanese).SetName("TestID-054N");
                yield return new TestCaseData(Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode.us_ascii).SetName("TestID-055N");
                yield return new TestCaseData(Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode.EUC_JP).SetName("TestID-056N");
                yield return new TestCaseData(Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode.shift_jis).SetName("TestID-057N");
                yield return new TestCaseData(Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode.ISO_2022_JP).SetName("TestID-058N");
                yield return new TestCaseData(Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_ESC).SetName("TestID-059N");
                yield return new TestCaseData(Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode._iso_2022_jp_Dollar_SIO).SetName("TestID-060N");
                yield return new TestCaseData(Encoding.BigEndianUnicode.GetBytes("abcde"), CustomEncode.UTF_8).SetName("TestID-061N");

                //passing  codePageNum as null value 
                yield return new TestCaseData(Encoding.UTF8.GetBytes("abcde"), null).SetName("TestID-062N");
                yield return new TestCaseData(Encoding.UTF7.GetBytes("abcde"), null).SetName("TestID-063N");
                yield return new TestCaseData(Encoding.UTF32.GetBytes("abcde"), null).SetName("TestID-064N");
                yield return new TestCaseData(Encoding.ASCII.GetBytes("abcde"), null).SetName("TestID-065N");
                yield return new TestCaseData(Encoding.Default.GetBytes("abcde"), null).SetName("TestID-066N");
                yield return new TestCaseData(Encoding.BigEndianUnicode.GetBytes("abcde"), null).SetName("TestID-067N");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("abcde"), null).SetName("TestID-068N");

                //passing  codePageNum as string.empty value 
                yield return new TestCaseData(Encoding.UTF8.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException)).SetName("TestID-069A");
                yield return new TestCaseData(Encoding.UTF7.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException)).SetName("TestID-070A");
                yield return new TestCaseData(Encoding.UTF32.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException)).SetName("TestID-071A");
                yield return new TestCaseData(Encoding.ASCII.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException)).SetName("TestID-072A");
                yield return new TestCaseData(Encoding.Default.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException)).SetName("TestID-073A");
                yield return new TestCaseData(Encoding.BigEndianUnicode.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException)).SetName("TestID-074A");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("abcde"), string.Empty).Throws(typeof(ArgumentException)).SetName("TestID-075A");

                //passing  byte value as string.empty value 
                yield return new TestCaseData(string.Empty, CustomEncode.us_ascii).Throws(typeof(ArgumentException)).SetName("TestID-076A");
                yield return new TestCaseData(Encoding.UTF8.GetBytes(string.Empty), CustomEncode.us_ascii).SetName("TestID-077L");
                yield return new TestCaseData(string.Empty, CustomEncode.UTF_16BE).Throws(typeof(ArgumentException)).SetName("TestID-078A");
                yield return new TestCaseData(Encoding.UTF8.GetBytes(string.Empty), CustomEncode.UTF_16BE).SetName("TestID-079L");
                yield return new TestCaseData(string.Empty, CustomEncode.UTF_16LE).Throws(typeof(ArgumentException)).SetName("TestID-080A");
                yield return new TestCaseData(Encoding.UTF8.GetBytes(string.Empty), CustomEncode.UTF_16LE).SetName("TestID-081L");
                yield return new TestCaseData(string.Empty, CustomEncode.UTF_7).Throws(typeof(ArgumentException)).SetName("TestID-082A");
                yield return new TestCaseData(Encoding.UTF8.GetBytes(string.Empty), CustomEncode.UTF_7).SetName("TestID-083L");
                yield return new TestCaseData(string.Empty, CustomEncode.UTF_8).Throws(typeof(ArgumentException)).SetName("TestID-084A");
                yield return new TestCaseData(Encoding.UTF8.GetBytes(string.Empty), CustomEncode.UTF_8).SetName("TestID-085L");
                yield return new TestCaseData(string.Empty, CustomEncode._iso_2022_jp_Dollar_ESC).Throws(typeof(ArgumentException)).SetName("TestID-086A");
                yield return new TestCaseData(Encoding.UTF8.GetBytes(string.Empty), CustomEncode._iso_2022_jp_Dollar_ESC).SetName("TestID-087L");
                yield return new TestCaseData(string.Empty, CustomEncode._iso_2022_jp_Dollar_SIO).Throws(typeof(ArgumentException)).SetName("TestID-088A");
                yield return new TestCaseData(Encoding.UTF8.GetBytes(string.Empty), CustomEncode._iso_2022_jp_Dollar_SIO).SetName("TestID-089L");
                yield return new TestCaseData(string.Empty, CustomEncode.shift_jis).Throws(typeof(ArgumentException)).SetName("TestID-090A");
                yield return new TestCaseData(Encoding.UTF8.GetBytes(string.Empty), CustomEncode.shift_jis).SetName("TestID-091L");
                yield return new TestCaseData(string.Empty, CustomEncode.ISO_2022_JP).Throws(typeof(ArgumentException)).SetName("TestID-092A");
                yield return new TestCaseData(Encoding.UTF8.GetBytes(string.Empty), CustomEncode.ISO_2022_JP).SetName("TestID-093L");
                yield return new TestCaseData(string.Empty, CustomEncode.x_mac_japanese).Throws(typeof(ArgumentException)).SetName("TestID-094A");
                yield return new TestCaseData(Encoding.UTF8.GetBytes(string.Empty), CustomEncode.x_mac_japanese).SetName("TestID-095L");
                yield return new TestCaseData(string.Empty, CustomEncode.EUC_JP).Throws(typeof(ArgumentException)).SetName("TestID-096A");
                yield return new TestCaseData(Encoding.UTF8.GetBytes(string.Empty), CustomEncode.EUC_JP).SetName("TestID-097L");
                yield return new TestCaseData(string.Empty, null).Throws(typeof(ArgumentException)).SetName("TestID-099A");
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
                yield return new TestCaseData("abcde").SetName("TestID-000N");
                yield return new TestCaseData("あいうえお").SetName("TestID-001N");
                yield return new TestCaseData("1").SetName("TestID-002N");
                yield return new TestCaseData(1).Throws(typeof(ArgumentException)).SetName("TestID-003A");
                yield return new TestCaseData("abcde" + "あいうえお").SetName("TestID-004N");
                yield return new TestCaseData(null).SetName("TestID-005N");
                yield return new TestCaseData(string.Empty).SetName("TestID-006L");
                yield return new TestCaseData("This string contains the unicode character Pi (\u03a0)").SetName("TestID-007N");
                yield return new TestCaseData("&lt;root&gt;abcde&lt;/root&gt;").SetName("TestID-008N");
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
                yield return new TestCaseData("abcde").SetName("TestID-000N");
                yield return new TestCaseData("あいうえお").SetName("TestID-001N");
                yield return new TestCaseData("1").SetName("TestID-002N");
                yield return new TestCaseData(1).Throws(typeof(ArgumentException)).SetName("TestID-003A");
                yield return new TestCaseData("abcde" + "あいうえお").SetName("TestID-004N");
                yield return new TestCaseData(null).SetName("TestID-005N");
                yield return new TestCaseData(string.Empty).SetName("TestID-006L");
                yield return new TestCaseData("<root>abcde</root>").SetName("TestID-007N");
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
                yield return new TestCaseData("hello").SetName("TestID-000N");
                yield return new TestCaseData("google.com").SetName("TestID-001N");
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
                yield return new TestCaseData("hello").SetName("TestID-000N");
                yield return new TestCaseData("google.com").SetName("TestID-001N");
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
                string str = "00 01 00 00 00 FF FF FF FF 01 00 00 00 00 00 00 00 06 01 00 00 00 0E 59 61 68 6F 6F 6F 6F 6F 6F 6F 6F 6F 6F 6F 0B";
                yield return new TestCaseData(str).SetName("TestID-000N");
                yield return new TestCaseData("01").SetName("TestID-001N");
                yield return new TestCaseData("0A 0B 0C").SetName("TestID-002N");
                yield return new TestCaseData(1).Throws(typeof(ArgumentException)).SetName("TestID-003A");
                yield return new TestCaseData("abcde" + "あいうえお").Throws(typeof(FormatException)).SetName("TestID-004A");
                yield return new TestCaseData(null).Throws(typeof(NullReferenceException)).SetName("TestID-005A");
                yield return new TestCaseData(string.Empty).Throws(typeof(ArgumentOutOfRangeException)).SetName("TestID-006A");
                yield return new TestCaseData("AB 0C DE").SetName("TestID-007N");
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
                var sevenItems = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
                string str = "00 01 00 00 00 FF FF FF FF 01 00 00 00 00 00 00 00 06 01 00 00 00 0E 59 61 68 6F 6F 6F 6F 6F 6F 6F 6F 6F 6F 6F 0B";
                byte[] data = { };
                yield return new TestCaseData(Encoding.UTF8.GetBytes("abcde")).SetName("TestID-000N");
                yield return new TestCaseData(Encoding.ASCII.GetBytes(str)).SetName("TestID-001N");
                yield return new TestCaseData(Encoding.Unicode.GetBytes("abcde" + "あいうえお")).SetName("TestID-002N");
                yield return new TestCaseData(null).Throws(typeof(NullReferenceException)).SetName("TestID-003A");
                yield return new TestCaseData(string.Empty).Throws(typeof(ArgumentException)).SetName("TestID-004A");
                yield return new TestCaseData(sevenItems).SetName("TestID-005N");
                yield return new TestCaseData(data).Throws(typeof(ArgumentOutOfRangeException)).SetName("TestID-006N");
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
                yield return new TestCaseData("AAECBAqQIEA=").SetName("TestID-000N");
                yield return new TestCaseData("abcde=").Throws(typeof(FormatException)).SetName("TestID-001A");
                yield return new TestCaseData("YYYYYWJjZGUT").SetName("TestID-002N");
                yield return new TestCaseData(string.Empty).SetName("TestID-003N");
                yield return new TestCaseData(null).Throws(typeof(System.ArgumentNullException)).SetName("TestID-004A");
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
                yield return new TestCaseData(Encoding.ASCII.GetBytes("abcde")).SetName("TestID-000N");
                yield return new TestCaseData(Encoding.UTF8.GetBytes("abcde")).SetName("TestID-001N");
                yield return new TestCaseData(string.Empty).Throws(typeof(System.ArgumentException)).SetName("TestID-002A");
                yield return new TestCaseData(null).Throws(typeof(System.ArgumentNullException)).SetName("TestID-003A");
                yield return new TestCaseData(Encoding.UTF8.GetBytes("")).SetName("TestID-004A");
            }
        }

        #endregion

        #region Test Code

        /// <summary>
        /// TestCasesOfGetEncodingsTest Method
        /// </summary>
        [TestCaseSource("TestCasesOfGetEncodingsTest")]
        public static void GetEncodingsTest()
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
        /// <param name="str">str</param>
        /// <param name="codePageNum">codePageNum</param>
        [TestCaseSource("TestCasesOfStringToByteTest")]
        public static void StringToByteTest(string str, int codePageNum)
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
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestCasesOfByteToStringTest Method
        /// </summary>
        /// <param name="abyt">abyt</param>
        /// <param name="codePageNum">codePageNum</param>
        [TestCaseSource("TestCasesOfByteToStringTest")]
        public static void ByteToStringTest(byte[] abyt, int codePageNum)
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
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// HtmlEncodeTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestHtmlEncodeTest")]
        public static void HtmlEncodeTest(string str)
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
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// HtmlDecodeTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestHtmlDecodeTest")]
        public static void HtmlDecodeTest(string str)
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
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestUrlEncodeTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestUrlEncodeTest")]
        public static void UrlEncodeTest(string str)
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
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestUrlDecodeTestMethod
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestUrlDecodeTest")]
        public static void UrlDecodeTest(string str)
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
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestFormHexStringTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestFormHexStringTest")]
        public void FormHexStringTest(string str)
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
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestToHexStringTest Method
        /// </summary>
        /// <param name="bytes"></param>
        [TestCaseSource("TestToHexStringTest")]
        public void ToHexStringTest(byte[] bytes)
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
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestFromBase64StringTest Method
        /// </summary>
        /// <param name="base64Str">base64Str</param>
        [TestCaseSource("TestFromBase64StringTest")]
        public static void FromBase64StringTest(string base64Str)
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
        /// <param name="aryByt">aryByt</param>
        [TestCaseSource("TestToBase64StringTest")]
        public static void ToBase64StringTest(byte[] aryByt)
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

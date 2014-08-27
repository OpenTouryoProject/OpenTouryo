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
//* クラス名        ：StringConverterTest
//* クラス日本語名  ：Test of the class to Convert the String
//*
//* 作成者          ：Rituparna
//* 更新履歴        ：
//* 
//*  Date:        Author:          Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/19/2014    Rituparna       Testcode development for StringConverter.
//*  08/12/2014   Sai              Added TestcaseID using SetName method as per Nishino-San comments
//*
//**********************************************************************************

#region Includes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.Str;

#endregion

namespace Public.Test.Str
{
    public class StringConverterTest
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

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method ToHankaku.
        /// </summary>
        public IEnumerable<TestCaseData> TestToHankakuTest
        {
            get
            {
                yield return new TestCaseData("達磨さん", "達磨さん").SetName("TestID-000N");
                yield return new TestCaseData("eigodesu", "eigodesu").SetName("TestID-001N");
                yield return new TestCaseData(string.Empty, string.Empty).SetName("TestID-002L");
                yield return new TestCaseData(null, string.Empty).SetName("TestID-003N");
                yield return new TestCaseData("ナルト", "ﾅﾙﾄ").SetName("TestID-004N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method ToZenkaku.
        /// </summary>
        public IEnumerable<TestCaseData> TestToZenkakuTest
        {
            get
            {
                yield return new TestCaseData("達磨さん", "達磨さん").SetName("TestID-000N");
                yield return new TestCaseData("eigodesu", "ｅｉｇｏｄｅｓｕ").SetName("TestID-001N");
                yield return new TestCaseData(string.Empty, string.Empty).SetName("TestID-002L");
                yield return new TestCaseData(null, string.Empty).SetName("TestID-003N");
                yield return new TestCaseData("ナルト", "ナルト").SetName("TestID-004N");
                yield return new TestCaseData("ﾀﾊﾞｺ", "タバコ").SetName("TestID-005N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method ToHiragana.
        /// </summary>
        public IEnumerable<TestCaseData> TestToHiraganaTest
        {
            get
            {
                yield return new TestCaseData("達磨さん", "達磨さん").SetName("TestID-000N");
                yield return new TestCaseData("eigodesu", "eigodesu").SetName("TestID-001N");
                yield return new TestCaseData(string.Empty, string.Empty).SetName("TestID-002L");
                yield return new TestCaseData(null, string.Empty).SetName("TestID-003N");
                yield return new TestCaseData("ナルト", "なると").SetName("TestID-004N");
                yield return new TestCaseData("ﾀﾊﾞｺ", "ﾀﾊﾞｺ").SetName("TestID-005N");
            }
        }


        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method ToKatakana.
        /// </summary>
        public IEnumerable<TestCaseData> TestToKatakanaTest
        {
            get
            {
                yield return new TestCaseData("達磨さん", "達磨サン").SetName("TestID-000N");
                yield return new TestCaseData("eigodesu", "eigodesu").SetName("TestID-001N");
                yield return new TestCaseData(string.Empty, string.Empty).SetName("TestID-002L");
                yield return new TestCaseData(null, string.Empty).SetName("TestID-003N");
                yield return new TestCaseData("ナルト", "ナルト").SetName("TestID-004N");
                yield return new TestCaseData("ﾀﾊﾞｺ", "ﾀﾊﾞｺ").SetName("TestID-005N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method EditYYYYMMDDString.
        /// </summary>
        public IEnumerable<TestCaseData> TestEditYYYYMMDDStringTest
        {
            get
            {
                string mystring = "01010101";
                yield return new TestCaseData("1234567").SetName("TestID-000N");
                yield return new TestCaseData("123456").SetName("TestID-001N");
                yield return new TestCaseData("2345128").SetName("TestID-002N");
                yield return new TestCaseData("abcdef").SetName("TestID-003N");
                yield return new TestCaseData("12345678").SetName("TestID-004N");
                yield return new TestCaseData(string.Empty).SetName("TestID-005N");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-006A");
                yield return new TestCaseData("12345").SetName("TestID-007N");
                yield return new TestCaseData("1234137").SetName("TestID-008N");
                yield return new TestCaseData("1234127").SetName("TestID-009N");
                yield return new TestCaseData("0").SetName("TestID-010N");
                yield return new TestCaseData("１２３４５").SetName("TestID-011N");
                yield return new TestCaseData(mystring).SetName("TestID-012N");
                yield return new TestCaseData("000000").SetName("TestID-013N");
                yield return new TestCaseData("-1234567").SetName("TestID-014N");
            }
        }

        /// <summary>
        /// ToHankakuTest Method
        /// </summary>        
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestToHankakuTest")]
        public static void ToHankakuTest(string input, string result)
        {
            string output = StringConverter.ToHankaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// ToZenkakuTest
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestToZenkakuTest")]
        public static void ToZenkakuTest(string input, string result)
        {
            string output = StringConverter.ToZenkaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// ToHiraganaTest Method
        /// </summary>        
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestToHiraganaTest")]
        public static void ToHiraganaTest(string input, string result)
        {
            string output = StringConverter.ToHiragana(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// ToKatakanaTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestToKatakanaTest")]
        public static void ToKatakanaTest(string input, string result)
        {
            string output = StringConverter.ToKatakana(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// EditYYYYMMDDStringTest Method
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestEditYYYYMMDDStringTest")]
        public static void EditYYYYMMDDStringTest(ref string text)
        {
            bool output = StringConverter.EditYYYYMMDDString(ref text);
            if (output)
            {
                Assert.True(output);
            }
            else
            {
                Assert.False(output);
            }
        }

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

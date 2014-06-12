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
                this.SetUp();

                yield return new TestCaseData("TestID-000N", "達磨さん", "達磨さん");
                yield return new TestCaseData("TestID-001N", "eigodesu", "eigodesu");
                yield return new TestCaseData("TestID-002L", string.Empty, string.Empty);
                yield return new TestCaseData("TestID-003N", null, string.Empty);
                yield return new TestCaseData("TestID-004N", "ナルト", "ﾅﾙﾄ");
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
                this.SetUp();

                yield return new TestCaseData("TestID-000N", "達磨さん", "達磨さん");
                yield return new TestCaseData("TestID-001N", "eigodesu", "ｅｉｇｏｄｅｓｕ");
                yield return new TestCaseData("TestID-002L", string.Empty, string.Empty);
                yield return new TestCaseData("TestID-003N", null, string.Empty);
                yield return new TestCaseData("TestID-004N", "ナルト", "ナルト");
                yield return new TestCaseData("TestID-005N", "ﾀﾊﾞｺ", "タバコ");
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
                this.SetUp();

                yield return new TestCaseData("TestID-000N", "達磨さん", "達磨さん");
                yield return new TestCaseData("TestID-001N", "eigodesu", "eigodesu");
                yield return new TestCaseData("TestID-002L", string.Empty, string.Empty);
                yield return new TestCaseData("TestID-003N", null, string.Empty);
                yield return new TestCaseData("TestID-004N", "ナルト", "なると");
                yield return new TestCaseData("TestID-005N", "ﾀﾊﾞｺ", "ﾀﾊﾞｺ");
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
                this.SetUp();

                yield return new TestCaseData("TestID-000N", "達磨さん", "達磨サン");
                yield return new TestCaseData("TestID-001N", "eigodesu", "eigodesu");
                yield return new TestCaseData("TestID-002L", string.Empty, string.Empty);
                yield return new TestCaseData("TestID-003N", null, string.Empty);
                yield return new TestCaseData("TestID-004N", "ナルト", "ナルト");
                yield return new TestCaseData("TestID-005N", "ﾀﾊﾞｺ", "ﾀﾊﾞｺ");
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
                this.SetUp();

                string mystring = "01010101";
                yield return new TestCaseData("TestID-000N", "1234567", true);
                yield return new TestCaseData("TestID-001N", "123456", true);
                yield return new TestCaseData("TestID-002N", "2345128", true);
                yield return new TestCaseData("TestID-003N", "abcdef", false);
                yield return new TestCaseData("TestID-004N", "12345678", false);
                yield return new TestCaseData("TestID-005N", string.Empty, false);
                yield return new TestCaseData("TestID-006A", null, false).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-007N", "12345", false);
                yield return new TestCaseData("TestID-008N", "1234137", true);
                yield return new TestCaseData("TestID-009N", "1234127", true);
                yield return new TestCaseData("TestID-010N", "0", false);
                yield return new TestCaseData("TestID-011N", "１２３４５", false);
                yield return new TestCaseData("TestID-012N", mystring, false);
                yield return new TestCaseData("TestID-013N", "000000", true);
                yield return new TestCaseData("TestID-014N", "-1234567", false);
            }
        }

        /// <summary>
        /// ToHankakuTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestToHankakuTest")]
        public static void ToHankakuTest(string testCaseID, string input, string result)
        {
            string output = StringConverter.ToHankaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// ToZenkakuTest
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestToZenkakuTest")]
        public static void ToZenkakuTest(string testCaseID, string input, string result)
        {
            string output = StringConverter.ToZenkaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// ToHiraganaTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestToHiraganaTest")]
        public static void ToHiraganaTest(string testCaseID, string input, string result)
        {
            string output = StringConverter.ToHiragana(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// ToKatakanaTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestToKatakanaTest")]
        public static void ToKatakanaTest(string testCaseID, string input, string result)
        {
            string output = StringConverter.ToKatakana(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// EditYYYYMMDDStringTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="text">text</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestEditYYYYMMDDStringTest")]
        public static void EditYYYYMMDDStringTest(string testCaseID, ref string text, bool result)
        {
            bool output = StringConverter.EditYYYYMMDDString(ref text);
            Assert.AreEqual(output, result);

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

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
//* クラス名        ：StringCheckerTest
//* クラス日本語名  ：Test of the class to Check the String
//*
//* 作成者          ：Rituparna
//* 更新履歴        ：
//* 
//*  Date:        Author:          Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/16/2014    Rituparna       Testcode development for StringChecker.
//*  08/12/2014   Rituparna        Added TestcaseID using SetName method as per Nishino-San comments
//**********************************************************************************

#region Includes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.Str;
using System.Text.RegularExpressions;
#endregion

namespace Public.Test.Str
{
    public class StringCheckerTest
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
        /// This method to generate test data to be passed to the method IsZenkaku.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsZenkakuTest
        {
            get
            {
                yield return new TestCaseData("貴社ますますご盛栄のこととお慶び申し上げます。平素は格別のご高配を賜り、厚く御礼申し上げます。", true).SetName("TestID-000N");
                yield return new TestCaseData("貴社ますますご盛栄のこととお慶び申し上げます。平素は格別のお引き立てをいただき、厚く御礼申し上げます。", true).SetName("TestID-001N");
                yield return new TestCaseData("１２３４５", true).SetName("TestID-002N");
                yield return new TestCaseData(string.Empty, true).SetName("TestID-003N");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-004A");
                yield return new TestCaseData("abcde", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-005A");
                yield return new TestCaseData("一二三四五六七八九十", true).SetName("TestID-006N");
                yield return new TestCaseData(1, true).Throws(typeof(ArgumentException)).SetName("TestID-007A");
                yield return new TestCaseData("0.1 0.2", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-008A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsHankaku.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsHankakuTest
        {
            get
            {
                yield return new TestCaseData("abcde", true).SetName("TestID-000N");
                yield return new TestCaseData("12345", true).SetName("TestID-001N");
                yield return new TestCaseData("ｦﾀ", true).SetName("TestID-002N");
                yield return new TestCaseData(string.Empty, true).SetName("TestID-003N");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-004A");
                yield return new TestCaseData("一二三四五六七八九十", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-005A");
                yield return new TestCaseData(1, true).Throws(typeof(ArgumentException)).SetName("TestID-006A");
                yield return new TestCaseData("0.1 0.2", true).SetName("TestID-000N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsNumeric.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsNumericTest
        {
            get
            {
                yield return new TestCaseData("12345", true).SetName("TestID-000N");
                yield return new TestCaseData("abcde", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-001A");
                yield return new TestCaseData("ｦﾀ", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-002A");
                yield return new TestCaseData(string.Empty, true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-003A");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-004A");
                yield return new TestCaseData("ｪｫｬｭｮ", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-005A");
                yield return new TestCaseData("12.78", true).SetName("TestID-006N");
                yield return new TestCaseData(12.78, true).Throws(typeof(ArgumentException)).SetName("TestID-007A");
                yield return new TestCaseData("１２３４５", true).SetName("TestID-008N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsNumbers.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsNumbersTest
        {
            get
            {
                yield return new TestCaseData("12345", true).SetName("TestID-000N");
                yield return new TestCaseData("abcde", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-001A");
                yield return new TestCaseData("ｦﾀ", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-002A");
                yield return new TestCaseData(string.Empty, true).SetName("TestID-003L");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-004A");
                yield return new TestCaseData("ｪｫｬｭｮ", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-005A");
                yield return new TestCaseData("一二三四五六七八九十", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-006A");
                yield return new TestCaseData(1, true).Throws(typeof(ArgumentException)).SetName("TestID-007A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsNumbersZenkaku.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsNumbersZenkakuTest
        {
            get
            {
                yield return new TestCaseData("１２３４５", true).SetName("TestID-000N");
                yield return new TestCaseData(string.Empty, true).SetName("TestID-001L");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-002A");
                yield return new TestCaseData("abcde", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-003A");
                yield return new TestCaseData("一二三四五六七八九十", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-004A");
                yield return new TestCaseData("貴社ますますご盛栄のこととお慶び申し上げます。平素は格別のご高配を賜り、厚く御礼申し上げます。", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-005A");
                yield return new TestCaseData("貴社ますますご盛栄のこととお慶び申し上げます。平素は格別のお引き立てをいただき、厚く御礼申し上げます。", true).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-006A");
                yield return new TestCaseData(1, true).Throws(typeof(ArgumentException)).SetName("TestID-007A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsAlphabet.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsAlphabetTest
        {
            get
            {
                yield return new TestCaseData(string.Empty, true).SetName("TestID-000L");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-001A");
                yield return new TestCaseData("abcde", true).SetName("TestID-002N");
                yield return new TestCaseData(1, true).Throws(typeof(ArgumentException)).SetName("TestID-003A");
                yield return new TestCaseData("一二三四五六七八九十", false).SetName("TestID-004N");
                yield return new TestCaseData("12345", false).SetName("TestID-005N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsNumbersHankaku.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsNumbersHankakuTest
        {
            get
            {
                yield return new TestCaseData("abcde", false).SetName("TestID-000N");
                yield return new TestCaseData("12345", true).SetName("TestID-001N");
                yield return new TestCaseData("ｦﾀ", false).SetName("TestID-002N");
                yield return new TestCaseData(string.Empty, true).SetName("TestID-003L");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-004A");
                yield return new TestCaseData("一二三四五六七八九十", false).SetName("TestID-005N");
                yield return new TestCaseData(1, true).Throws(typeof(ArgumentException)).SetName("TestID-006A");
                yield return new TestCaseData("0.1 0.2", false).SetName("TestID-007N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsAlphabetHankaku.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsAlphabetHankakuTest
        {
            get
            {
                yield return new TestCaseData("abcde", true).SetName("TestID-000N");
                yield return new TestCaseData("12345", false).SetName("TestID-001N");
                yield return new TestCaseData("ｦﾀ", false).SetName("TestID-002N");
                yield return new TestCaseData(string.Empty, true).SetName("TestID-003N");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-004A");
                yield return new TestCaseData("一二三四五六七八九十", false).SetName("TestID-005N");
                yield return new TestCaseData(1, true).Throws(typeof(ArgumentException)).SetName("TestID-006A");
                yield return new TestCaseData("0.1 0.2", false).SetName("TestID-007N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsAlphabetZenkaku.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsAlphabetZenkakuTest
        {
            get
            {
                yield return new TestCaseData("abcde", false).SetName("TestID-000N");
                yield return new TestCaseData("12345", false).SetName("TestID-001N");
                yield return new TestCaseData("ｦﾀ", false).SetName("TestID-002N");
                yield return new TestCaseData(string.Empty, true).SetName("TestID-003N");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-004A");
                yield return new TestCaseData("一二三四五六七八九十", false).SetName("TestID-005N");
                yield return new TestCaseData(1, true).Throws(typeof(ArgumentException)).SetName("TestID-006A");
                yield return new TestCaseData("0.1 0.2", false).SetName("TestID-007N");
                yield return new TestCaseData("Ａ", true).SetName("TestID-008N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsHiragana.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsHiraganaTest
        {
            get
            {
                yield return new TestCaseData("abcde", false).SetName("TestID-000N");
                yield return new TestCaseData("12345", false).SetName("TestID-001N");
                yield return new TestCaseData("ｦﾀ", false).SetName("TestID-002N");
                yield return new TestCaseData(string.Empty, true).SetName("TestID-003L");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-005A");
                yield return new TestCaseData("一二三四五六七八九十", false).SetName("TestID-006N");
                yield return new TestCaseData(1, true).Throws(typeof(ArgumentException)).SetName("TestID-007A");
                yield return new TestCaseData("0.1 0.2", false).SetName("TestID-008N");
                yield return new TestCaseData("さん", true).SetName("TestID-009N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsKatakana.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsKatakanaTest
        {
            get
            {
                yield return new TestCaseData("abcde", false).SetName("TestID-000N");
                yield return new TestCaseData("12345", false).SetName("TestID-000N");
                yield return new TestCaseData("ｦﾀ", true).SetName("TestID-000N");
                yield return new TestCaseData(string.Empty, true).SetName("TestID-000N");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-000N");
                yield return new TestCaseData("一二三四五六七八九十", false).SetName("TestID-000N");
                yield return new TestCaseData(1, true).Throws(typeof(ArgumentException)).SetName("TestID-000N");
                yield return new TestCaseData("0.1 0.2", false).SetName("TestID-000N");
                yield return new TestCaseData("ナルト", true).SetName("TestID-000N");
            }
        }

        public IEnumerable<TestCaseData> TestIsKatakanaZenkakuTest
        {
            get
            {
                yield return new TestCaseData("abcde", false).SetName("TestID-000N");
                yield return new TestCaseData("12345", false).SetName("TestID-001N");
                yield return new TestCaseData("ｦﾀ", false).SetName("TestID-002N");
                yield return new TestCaseData(string.Empty, true).SetName("TestID-003N");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-004A");
                yield return new TestCaseData("一二三四五六七八九十", false).SetName("TestID-005N");
                yield return new TestCaseData(1, true).Throws(typeof(ArgumentException)).SetName("TestID-006A");
                yield return new TestCaseData("0.1 0.2", false).SetName("TestID-007N");
                yield return new TestCaseData("ナルト", true).SetName("TestID-008N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsKatakanaHankaku.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsKatakanaHankakuTest
        {
            get
            {
                yield return new TestCaseData("abcde", false).SetName("TestID-000N");
                yield return new TestCaseData("12345", false).SetName("TestID-001N");
                yield return new TestCaseData("ｦﾀ", true).SetName("TestID-002N");
                yield return new TestCaseData(string.Empty, true).SetName("TestID-003N");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-004A");
                yield return new TestCaseData("一二三四五六七八九十", false).SetName("TestID-005N");
                yield return new TestCaseData(1, true).Throws(typeof(ArgumentException)).SetName("TestID-006A");
                yield return new TestCaseData("0.1 0.2", false).SetName("TestID-007N");
                yield return new TestCaseData("ナルト", false).SetName("TestID-008N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsKanji.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsKanjiTest
        {
            get
            {
                yield return new TestCaseData("達磨", true).SetName("TestID-000N");
                yield return new TestCaseData("12345", false).SetName("TestID-001N");
                yield return new TestCaseData("ｦﾀ", false).SetName("TestID-002N");
                yield return new TestCaseData(string.Empty, true).SetName("TestID-003N");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-004A");
                yield return new TestCaseData("一二三四五六七八九十", true).SetName("TestID-005N");
                yield return new TestCaseData(1, true).Throws(typeof(ArgumentException)).SetName("TestID-006A");
                yield return new TestCaseData("0.1 0.2", false).SetName("TestID-007N");
                yield return new TestCaseData("ナルト", false).SetName("TestID-008N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsInCodePage.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsInCodePageTest
        {
            get
            {
                yield return new TestCaseData(string.Empty, 1200, true).SetName("TestID-000N");
                yield return new TestCaseData("abcde", CustomEncode.UTF_16LE, true).SetName("TestID-001N");
                yield return new TestCaseData("abcde", CustomEncode.UTF_8, true).SetName("TestID-002N");
                yield return new TestCaseData(null, CustomEncode.UTF_8, true).Throws(typeof(ArgumentNullException)).SetName("TestID-003A");
                yield return new TestCaseData("abcd", string.Empty, true).Throws(typeof(ArgumentException)).SetName("TestID-004A");
                yield return new TestCaseData("abcd", CustomEncode._iso_2022_jp_Dollar_ESC, true).SetName("TestID-005N");
                yield return new TestCaseData("abcd", null, true).SetName("TestID-006N");
                yield return new TestCaseData("\\\\@!", 20108, false).SetName("TestID-007N");
                yield return new TestCaseData("\\\\*&", 20108, false).SetName("TestID-008N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsShiftJis.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsShiftJisTest
        {
            get
            {
                yield return new TestCaseData(string.Empty, true).SetName("TestID-000N");
                yield return new TestCaseData("abcde", true).SetName("TestID-001N");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-002A");
                yield return new TestCaseData("ƒ`ƒƒƒlƒ‹ƒp[ƒgƒi[‚Ì‘I‘ð", false).SetName("TestID-003A");
                yield return new TestCaseData("#", true).SetName("TestID-004A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsShiftJisZenkaku.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsShiftJisZenkakuTest
        {
            get
            {
                yield return new TestCaseData(string.Empty, true).SetName("TestID-000N");
                yield return new TestCaseData("ナルト", true).SetName("TestID-001N");
                yield return new TestCaseData("abcde", false).SetName("TestID-002N");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-003A");
                yield return new TestCaseData("ƒ`ƒƒƒlƒ‹ƒp[ƒgƒi[‚Ì‘I‘ð", false).SetName("TestID-004N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsShiftJisHankaku.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsShiftJisHankakuTest
        {
            get
            {
                yield return new TestCaseData(string.Empty, true).SetName("TestID-000N");
                yield return new TestCaseData("ナルト", false).SetName("TestID-001N");
                yield return new TestCaseData("abcde", true).SetName("TestID-002N");
                yield return new TestCaseData(null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-003A");
                yield return new TestCaseData("ƒ`ƒƒƒlƒ‹ƒp[ƒgƒi[‚Ì‘I‘ð", false).SetName("TestID-004N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method Match.
        /// </summary>
        public IEnumerable<TestCaseData> TestMatch1Test
        {
            get
            {
                yield return new TestCaseData("Hello World", @"d \w+ \s", false).SetName("TestID-000N");
                yield return new TestCaseData("Hello World", @"d \w+ \s", false).SetName("TestID-001N");
                yield return new TestCaseData("The the quick brown fox  fox jumped over the lazy dog dog.", @"\b(?<word>\w+)\s+(\k<word>)\b", true).SetName("TestID-002N");
                yield return new TestCaseData("Hello World", string.Empty, true).SetName("TestID-003N");
                yield return new TestCaseData("Hello World", @"d \w+ \s", false).SetName("TestID-004N");
                yield return new TestCaseData("Hello World", @"d \w+ \s", false).SetName("TestID-005N");
                yield return new TestCaseData("Hello World", string.Empty, true).SetName("TestID-006N");
                yield return new TestCaseData(string.Empty, @"d \w+ \s", false).SetName("TestID-007N");
                yield return new TestCaseData(string.Empty, null, false).Throws(typeof(ArgumentNullException)).SetName("TestID-008A");
                yield return new TestCaseData(null, @"d \w+ \s", false).Throws(typeof(ArgumentNullException)).SetName("TestID-009A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method Match.
        /// </summary>
        public IEnumerable<TestCaseData> TestMatchTest
        {
            get
            {
                yield return new TestCaseData("Hello World", @"d \w+ \s", RegexOptions.Singleline, false).SetName("TestID-000N");
                yield return new TestCaseData("Hello World", @"d \w+ \s", RegexOptions.RightToLeft, false).SetName("TestID-001N");
                yield return new TestCaseData("The the quick brown fox  fox jumped over the lazy dog dog.", @"\b(?<word>\w+)\s+(\k<word>)\b", RegexOptions.Compiled, true).SetName("TestID-002N");
                yield return new TestCaseData("Hello World", string.Empty, RegexOptions.Singleline, true).SetName("TestID-003N");
                yield return new TestCaseData("Hello World", @"d \w+ \s", string.Empty, true).Throws(typeof(ArgumentException)).SetName("TestID-004A");
                yield return new TestCaseData("Hello World", @"d \w+ \s", null, false).SetName("TestID-005N");
                yield return new TestCaseData("Hello World", string.Empty, null, true).SetName("TestID-006N");
                yield return new TestCaseData(string.Empty, @"d \w+ \s", RegexOptions.Singleline, false).SetName("TestID-007N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method Matches.
        /// </summary>
        public IEnumerable<TestCaseData> TestMatches1Test
        {
            get
            {
                yield return new TestCaseData("Hello World", @"d \w+ \s", 0).SetName("TestID-000N");
                yield return new TestCaseData("The the quick brown fox  fox jumped over the lazy dog dog.", @"\b(?<word>\w+)\s+(\k<word>)\b", 2).SetName("TestID-001N");
                yield return new TestCaseData(string.Empty, @"d \w+ \s", 0).SetName("TestID-002N");
                yield return new TestCaseData(null, @"d \w+ \s", 0).Throws(typeof(ArgumentNullException)).SetName("TestID-003A");
                yield return new TestCaseData("hi", string.Empty, 3).SetName("TestID-004N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method Matches.
        /// </summary>
        public IEnumerable<TestCaseData> TestMatchesTest
        {
            get
            {
                yield return new TestCaseData("Hello World", @"d \w+ \s", RegexOptions.Singleline, 0).SetName("TestID-000N");
                yield return new TestCaseData("Hello World", @"d \w+ \s", RegexOptions.RightToLeft, 0).SetName("TestID-001N");
                yield return new TestCaseData("The the quick brown fox  fox jumped over the lazy dog dog.", @"\b(?<word>\w+)\s+(\k<word>)\b", RegexOptions.Compiled, 2).SetName("TestID-002N");
                yield return new TestCaseData("Hello World", string.Empty, RegexOptions.Singleline, 12).SetName("TestID-003N");
                yield return new TestCaseData("Hello World", @"d \w+ \s", string.Empty, 2).Throws(typeof(ArgumentException)).SetName("TestID-004A");
                yield return new TestCaseData("Hello World", @"d \w+ \s", null, 0).SetName("TestID-005N");
                yield return new TestCaseData("Hello World", string.Empty, null, 12).SetName("TestID-006N");
                yield return new TestCaseData(string.Empty, @"d \w+ \s", RegexOptions.Singleline, 0).SetName("TestID-007N");
            }
        }

        #endregion

        /// <summary>
        /// IsZenkakuTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>        
        [TestCaseSource("TestIsZenkakuTest")]
        public static void IsZenkakuTest(string input, bool result)
        {
            bool output = StringChecker.IsZenkaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsHankakuTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsHankakuTest")]
        public static void IsHankakuTest(string input, bool result)
        {
            bool output = StringChecker.IsHankaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsNumericTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsNumericTest")]
        public static void IsNumericTest(string input, bool result)
        {
            bool output = StringChecker.IsNumeric(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsNumbersTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsNumbersTest")]
        public static void IsNumbersTest(string input, bool result)
        {
            bool output = StringChecker.IsNumbers(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsNumbers_ZenkakuTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsNumbersZenkakuTest")]
        public static void IsNumbers_ZenkakuTest(string input, bool result)
        {
            bool output = StringChecker.IsNumbers_Zenkaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsAlphabetTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsAlphabetTest")]
        public static void IsAlphabetTest(string input, bool result)
        {
            bool output = StringChecker.IsAlphabet(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsNumbers_HankakuTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsNumbersHankakuTest")]
        public static void IsNumbers_HankakuTest(string input, bool result)
        {
            bool output = StringChecker.IsNumbers_Hankaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsAlphabet_HankakuTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsAlphabetHankakuTest")]
        public static void IsAlphabet_HankakuTest(string input, bool result)
        {
            bool output = StringChecker.IsAlphabet_Hankaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsAlphabet_ZenkakuTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsAlphabetZenkakuTest")]
        public static void IsAlphabet_ZenkakuTest(string input, bool result)
        {
            bool output = StringChecker.IsAlphabet_Zenkaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsHiraganaTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsHiraganaTest")]
        public static void IsHiraganaTest(string input, bool result)
        {
            bool output = StringChecker.IsHiragana(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsKatakanaTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsKatakanaTest")]
        public static void IsKatakanaTest(string input, bool result)
        {
            bool output = StringChecker.IsKatakana(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsKatakana_ZenkakuTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsKatakanaZenkakuTest")]
        public static void IsKatakana_ZenkakuTest(string input, bool result)
        {
            bool output = StringChecker.IsKatakana_Zenkaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsKatakana_HankakuTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsKatakanaHankakuTest")]
        public static void IsKatakana_HankakuTest(string input, bool result)
        {
            bool output = StringChecker.IsKatakana_Hankaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsKanjiTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsKanjiTest")]
        public static void IsKanjiTest(string input, bool result)
        {
            bool output = StringChecker.IsKanji(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsInCodePageTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="codePageNum">codePageNum</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsInCodePageTest")]
        public static void IsInCodePageTest(string input, int codePageNum, bool result)
        {
            bool output = StringChecker.IsInCodePage(input, codePageNum);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsShiftJisTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsShiftJisTest")]
        public static void IsShiftJisTest(string input, bool result)
        {
            bool output = StringChecker.IsShift_Jis(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsIsShift_Jis_ZenkakuTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsShiftJisZenkakuTest")]
        public static void IsIsShift_Jis_ZenkakuTest(string input, bool result)
        {
            bool output = StringChecker.IsShift_Jis_Zenkaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// IsShift_Jis_HankakuTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestIsShiftJisHankakuTest")]
        public static void IsShift_Jis_HankakuTest(string input, bool result)
        {
            bool output = StringChecker.IsShift_Jis_Hankaku(input);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// MatchTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="pattern">pattern</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestMatch1Test")]
        public static void MatchTest(string input, string pattern, bool result)
        {
            bool output = StringChecker.Match(input, pattern);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// MatchTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="pattern">pattern</param>
        /// <param name="options">options</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestMatchTest")]
        public static void MatchTest(string input, string pattern, RegexOptions options, bool result)
        {
            bool output = StringChecker.Match(input, pattern, options);
            Assert.AreEqual(output, result);
        }

        /// <summary>
        /// MatchesTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="pattern">pattern</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestMatches1Test")]
        public static void MatchesTest(string input, string pattern, int result)
        {
            MatchCollection output = StringChecker.Matches(input, pattern);
            Assert.NotNull(output);
            Assert.AreEqual(output.Count, result);
        }

        /// <summary>
        /// MatchesTest Method
        /// </summary>
        /// <param name="input">input</param>
        /// <param name="pattern">pattern</param>
        /// <param name="options">options</param>
        /// <param name="result">result</param>
        [TestCaseSource("TestMatchesTest")]
        public static void MatchesTest(string input, string pattern, RegexOptions options, int result)
        {
            MatchCollection output = StringChecker.Matches(input, pattern, options);
            Assert.NotNull(output);
            Assert.AreEqual(output.Count, result);
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

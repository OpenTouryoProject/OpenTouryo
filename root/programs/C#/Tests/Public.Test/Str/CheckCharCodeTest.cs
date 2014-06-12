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
//* クラス名        ：CheckCharCodeTest
//* クラス日本語名  ：Test of the class to Check CharCode
//*
//* 作成者          ：Santosh Avaji
//* 更新履歴        ：
//* 
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/08/2014   Santosh Avaji      Testcode development for CheckCharCode.
//*
//**********************************************************************************

#region Includes

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.Str;
using System.IO;

#endregion
namespace Public.Test.Str
{
    [TestFixture, Description("Tests for Check Char Code")]
    public class CheckCharCodeTest
    {
        #region Class Variables
        private CheckCharCode _checkCharCode;
        #endregion
        #region Setup/Teardown
        /// <summary>
        /// Code that is run once for a suite of tests
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
        }
        /// <summary>
        /// Code that is run once after a suite of tests has finished executing
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }
        /// <summary>
        /// Code that is run before each test
        /// </summary>
        [SetUp]
        public void Initialize()
        {
            _checkCharCode = new CheckCharCode("aaaa", "bbbb", Encoding.UTF7);
        }

        /// <summary>
        /// Code that is run after each test
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            //TODO:  Put dispose in here for _checkCharCode or delete this line
        }
        #endregion
        #region Method Tests
        #region GeneratedMethods
        /// <summary> Is In Range Method Test</summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="test">test</param>
        [TestCase("TestID-001N", "bbba", Description = "Returns true")]
        [TestCase("TestID-002N", "bbac", Description = "Returns true")]
        [TestCase("TestID-003N", "abcd", Description = "Returns true")]
        [TestCase("TestID-004N", "desc", ExpectedException = typeof(NUnit.Framework.AssertionException), Description = "Returns false hence throwing Assestion exception")]
        [TestCase("TestID-005A", "", ExpectedException = typeof(ArgumentOutOfRangeException), Description = "Pass Zero length string")]
        [TestCase("TestID-006A", "Abcdefghi", ExpectedException = typeof(ArgumentOutOfRangeException), Description = "More than eight Characters")]
        [TestCase("TestID-007A", null, Description = "Pass null value", ExpectedException = typeof(ArgumentNullException))]
        public void IsInRangeTest(string testCaseID, string test)
        {
            try
            {
                bool results = _checkCharCode.IsInRange(test);
                Assert.IsTrue(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>ConstructorTest Method</summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="startChar">startChar</param>
        /// <param name="endChar">endChar</param>
        /// <param name="stringEncoding">stringEncoding</param>
        [TestCaseSource("Test")]
        public void ConstructorTest(string testCaseID, string startChar, string endChar, Encoding stringEncoding)
        {
            try
            {
                var CheckCharCode = new CheckCharCode(startChar, endChar, stringEncoding);
            }
            catch (Exception ex)
            {
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method ToBase64String.
        /// </summary>
        public IEnumerable<TestCaseData> Test
        {
            get
            {
                yield return new TestCaseData("TestID-001N", "start", "end", Encoding.ASCII);
                yield return new TestCaseData("TestID-002N", "Compare", "Compared", Encoding.UTF7);
                yield return new TestCaseData("TestID-003A", "", "end", Encoding.ASCII).Throws(typeof(ArgumentOutOfRangeException));
                yield return new TestCaseData("TestID-004A", "MoreThanEightCharacter", "end", Encoding.ASCII).Throws(typeof(ArgumentOutOfRangeException));
                yield return new TestCaseData("TestID-005A", "MoreThanEightCharacter", "", Encoding.ASCII).Throws(typeof(ArgumentOutOfRangeException));
                yield return new TestCaseData("TestID-006A", "Correct", "MoreThanEightCharacter", Encoding.ASCII).Throws(typeof(ArgumentOutOfRangeException));
                yield return new TestCaseData("TestID-007A", null, "MoreThanEightCharacter", Encoding.ASCII).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-008A", "Correct", null, Encoding.ASCII).Throws(typeof(ArgumentNullException));
            }
        }
        #endregion // End of GeneratedMethods
        #endregion

    }
}

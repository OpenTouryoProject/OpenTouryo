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
//*  Date:        Author:          Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/08/2014   Santosh Avaji    Testcode development for CheckCharCode.
//*  08/11/2014   Rituparna        Added TestcaseID using SetName and TestName method as per Nishino-San comments
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
        /// <param name="test">test</param>
        [TestCase("bbba", Description = "Returns true",TestName="TestID-001N")]
        [TestCase("bbac", Description = "Returns true", TestName = "TestID-002N")]
        [TestCase("abcd", Description = "Returns true", TestName = "TestID-003N")]
        [TestCase("desc", ExpectedException = typeof(NUnit.Framework.AssertionException), Description = "Returns false hence throwing Assestion exception", TestName = "TestID-004N")]
        [TestCase("", ExpectedException = typeof(ArgumentOutOfRangeException), Description = "Pass Zero length string",TestName="TestID-005A")]
        [TestCase("Abcdefghi", ExpectedException = typeof(ArgumentOutOfRangeException), Description = "More than eight Characters",TestName="TestID-006A")]
        [TestCase(null, Description = "Pass null value", ExpectedException = typeof(ArgumentNullException), TestName = "TestID-007A")]
        public void IsInRangeTest(string test)
        {
            try
            {
                bool results = _checkCharCode.IsInRange(test);
                Assert.IsTrue(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>ConstructorTest Method</summary>        
        /// <param name="startChar">startChar</param>
        /// <param name="endChar">endChar</param>
        /// <param name="stringEncoding">stringEncoding</param>
        [TestCaseSource("Test")]
        public void ConstructorTest(string startChar, string endChar, Encoding stringEncoding)
        {
            try
            {
                var CheckCharCode = new CheckCharCode(startChar, endChar, stringEncoding);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
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
                yield return new TestCaseData("start", "end", Encoding.ASCII).SetName("TestID-001N");
                yield return new TestCaseData("Compare", "Compared", Encoding.UTF7).SetName("TestID-002N");
                yield return new TestCaseData("", "end", Encoding.ASCII).Throws(typeof(ArgumentOutOfRangeException)).SetName("TestID-003A");
                yield return new TestCaseData("MoreThanEightCharacter", "end", Encoding.ASCII).Throws(typeof(ArgumentOutOfRangeException)).SetName("TestID-004A");
                yield return new TestCaseData("MoreThanEightCharacter", "", Encoding.ASCII).Throws(typeof(ArgumentOutOfRangeException)).SetName("TestID-005A");
                yield return new TestCaseData("Correct", "MoreThanEightCharacter", Encoding.ASCII).Throws(typeof(ArgumentOutOfRangeException)).SetName("TestID-006A");
                yield return new TestCaseData(null, "MoreThanEightCharacter", Encoding.ASCII).Throws(typeof(ArgumentNullException)).SetName("TestID-007A");
                yield return new TestCaseData("Correct", null, Encoding.ASCII).Throws(typeof(ArgumentNullException)).SetName("TestID-008A");
            }
        }
        #endregion // End of GeneratedMethods
        #endregion

    }
}

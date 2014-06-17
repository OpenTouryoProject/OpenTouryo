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
//* クラス名        ：PubCmnFunctionTest
//* クラス日本語名  ：Test of the class to PubCmnFunction
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2014/05/08  Sai               Testcode development for PubCmnFunction class .
//*
//**********************************************************************************

#region Includes

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.Util;
using System.IO;
using System.Xml;
using System.Data;
using System.Web;
using System.Collections;
using System.Diagnostics;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Win32;

#endregion

namespace Public.Test.Util
{
    /// <summary>
    /// Tests for the Pub Cmn Function Class
    /// </summary>
    [TestFixture, Description("Tests for Pub Cmn Function")]
    public class PubCmnFunctionTest
    {
        #region Class Variables

        private PubCmnFunction _pubCmnFunction;

        #endregion

        #region Setup/Teardown

        /// <summary>
        /// Code that is run once for a suite of tests
        /// </summary>
        [SetUp]
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
            //New instance of Pub Cmn Function
            _pubCmnFunction = new PubCmnFunction();
        }

        /// <summary>
        /// Code that is run after each test
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            //TODO:  Put dispose in here for _pubCmnFunction or delete this line
        }
        #endregion

        #region Property Tests

        #region GeneratedProperties

        // No public properties were found. No tests are generated for non-public scoped properties.

        #endregion // End of GeneratedProperties

        #endregion

        #region Method Tests

        #region Test data
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method GetPropsFromPropStringTest.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfGetPropsFromPropStringTest
        {
            get
            {
                this.TestFixtureSetup();
                //Normal test case
                yield return new TestCaseData("TestID-001N", "CategoryID=0001;CategoryName=Test");
                yield return new TestCaseData("TestID-002N", "CategoryID{=}0001;CategoryName=Test").Throws(typeof(ArgumentException));

                //Abnormal test case
                yield return new TestCaseData("TestID-003A", "=CategoryID=0001;CategoryName=Test").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-004A", ";CategoryID=0001;CategoryName=Test").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-005A", "}CategoryID=0001;CategoryName=Test").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-006A", "CategoryID0001;CategoryName=Test").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-007A", "Category=ID0001CategoryName=Test").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-008A", "{CategoryID=0001;CategoryName=Test").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-009A", "=0001;CategoryName=Test").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-010A", "CategoryID=;CategoryName=Test").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-011A", "{CategoryID=0;CategoryName=Test").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-012A", "CategoryID=0;CategoryName=");
                yield return new TestCaseData("TestID-013A", "CategoryID=0;CategoryName=cat\r\ndog\r\nanimal\r\nperson");
                yield return new TestCaseData("TestID-014A", "3CategoryID=0,CategoryName=cat").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-015A", "CategoryID{=0001;CategoryName=Test").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-016A", "CategoryID}=0001;CategoryName=Test").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-017A", "CategoryID=0001;=Test").Throws(typeof(ArgumentException));
            }
        }
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method GetCommandArgs.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfGetCommandArgsTest
        {
            get
            {
                this.TestFixtureSetup();
                //Normal test case
                yield return new TestCaseData("TestID-001N", '/');
                yield return new TestCaseData("TestID-002N", 'Z');
                yield return new TestCaseData("TestID-003N", '\\');
                //Abnormal test case
                yield return new TestCaseData("TestID-004A", 'o');
                yield return new TestCaseData("TestID-005A", '"');
                yield return new TestCaseData("TestID-006A", '\"');
                yield return new TestCaseData("TestID-007A", ' ');
                yield return new TestCaseData("TestID-008A", '2');
                yield return new TestCaseData("TestID-009A", "Vtry").Throws(typeof(ArgumentException));
            }
        }
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method BuiltStringIntoEnvironmentVariable.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfBuiltStringIntoEnvironmentVariableTest
        {
            get
            {
                this.TestFixtureSetup();
                //Normal test case
                yield return new TestCaseData("TestID-001N", "%test");
                yield return new TestCaseData("TestID-002N", "%test test");
                yield return new TestCaseData("TestID-003N", @"C:\Windows\system32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\
                                                               v1.0\;C:\Program Files\Windows Imaging\;C:\Program Files\Microsoft SQL Server\100\Tools\Binn\;
                                                               C:\Program Files\Microsoft SQL Server\100\DTS\Binn\;C:\Program Files\Microsoft SQL Server\100\
                                                               Tools\Binn\VSShell\Common7\IDE\;C:\Program Files\Microsoft\Web Platform Installer\;C:\Program Files
                                                               \Microsoft ASP.NET\ASP.NET Web Pages\v1.0\;C:\Program Files\Microsoft SQL Server\110\Tools\Binn\");
                yield return new TestCaseData("TestID-003N", @"%USERPROFILE%\AppData\Local\Temp");
                yield return new TestCaseData("TestID-004N", @".;C:\PROGRA~1\IBM\SQLLIB\java\db2java.zip;C:\PROGRA~1\IBM\SQLLIB\java\db2jcc.jar;C:\PROGRA~1\IBM\
                                               SQLLIB\java\sqlj.zip;C:\PROGRA~1\IBM\SQLLIB\java\db2jcc_license_cu.jar;C:\PROGRA~1\IBM\SQLLIB\bin;C:\PROGRA~1\IBM\
                                               SQLLIB\java\common.jar");
                //Abnormal test case
                yield return new TestCaseData("TestID-005L", string.Empty);
                yield return new TestCaseData("TestID-006L", null);
                yield return new TestCaseData("TestID-007A", @"--***-**-8j@\|//^&$#!~`?<>:;");
                yield return new TestCaseData("TestID-008A", @"%USEuRPROFILE%\AppData\Local\Tempu");
            }
        }
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method CalculateSessionSizeMB.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfCalculateSessionSizeMBTest
        {
            get
            {
                this.TestFixtureSetup();
                //Normal test case
                yield return new TestCaseData("TestID-001N").Throws(typeof(NullReferenceException));
            }
        }
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method CalculateSessionSize.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfCalculateSessionSizeTest
        {
            get
            {
                this.TestFixtureSetup();
                //Normal test case
                yield return new TestCaseData("TestID-001N").Throws(typeof(NullReferenceException));
            }
        }
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method CalculateSessionSize.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfCalculateSessionSizeKBTest
        {
            get
            {
                this.TestFixtureSetup();
                //Normal test case
                yield return new TestCaseData("TestID-001N").Throws(typeof(NullReferenceException));
            }
        }
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method CalculateSessionSize.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfGetCurrentMethodNameTest
        {
            get
            {
                this.TestFixtureSetup();
                //Normal test case
                yield return new TestCaseData("TestID-001N");
            }
        }
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method CalculateSessionSize.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfGetCurrentCodeInfoTest
        {
            get
            {
                this.TestFixtureSetup();
                //Normal test case
                yield return new TestCaseData("TestID-001N");
            }
        }
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method CalculateSessionSize.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfGetCurrentPropertyNameTest
        {
            get
            {
                this.TestFixtureSetup();
                //Normal test case
                yield return new TestCaseData("TestID-001N");
            }
        }
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method CalculateSessionSize.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfGetFileNameNoExTest
        {
            get
            {
                this.TestFixtureSetup();
                //Normal test case
                yield return new TestCaseData("TestID-001N", "DaoOrders.cs,DaoOrdersTest.vb", ',');
                yield return new TestCaseData("TestID-002L", "DaoOrders.cs,DaoOrdersd.vb", '\0');
                yield return new TestCaseData("TestID-003L", "DaoOrders.cs", '\0');
                yield return new TestCaseData("TestID-004L", "DaoOrders.csDaoOrdersTest", '\0');
                yield return new TestCaseData("TestID-005A", "DaoOrders.cs", ',');
                yield return new TestCaseData("TestID-006A", "DaoOrders.cs,", '\0');
                yield return new TestCaseData("TestID-007A", "DaoOrders.cs,", ',');
                yield return new TestCaseData("TestID-008A", "DaoOrders.cs,DaoOrders.vb", '-');
                yield return new TestCaseData("TestID-009A", "DaoOrders.cs,DaoOrders.vb", ',');
                yield return new TestCaseData("TestID-010A", "DaoOrders.cs,DaoOrdersTest", ',');
            }
        }
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method CalculateSessionSize.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfShortenByteArrayTest
        {
            get
            {
                this.TestFixtureSetup();
                //Normal test case
                yield return new TestCaseData("TestID-001N", new byte[] { 0, 1, 2 }, 1);
                yield return new TestCaseData("TestID-002N", new byte[] { 0, 1, 2, 3 }, 2);
                yield return new TestCaseData("TestID-003L", new byte[] { 0, 1, 2 }, 0).Throws(typeof(IndexOutOfRangeException));
                yield return new TestCaseData("TestID-004L", new byte[] { }, 0);
                yield return new TestCaseData("TestID-005L", new byte[] { }, 2);
                yield return new TestCaseData("TestID-006L", new byte[] { }, 2);
                yield return new TestCaseData("TestID-007N", new byte[] { 0, 1, 2 }, 4);
                yield return new TestCaseData("TestID-008A", new byte[] { 0, 1, 2, 3, 4 }, 5);
            }
        }
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method CalculateSessionSize.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfGetLongFromByteTest
        {
            get
            {
                this.TestFixtureSetup();
                //Normal test case
                yield return new TestCaseData("TestID-001N", new byte[] { 0, 1, 2 });
                yield return new TestCaseData("TestID-002L", new byte[] { 1 });
                yield return new TestCaseData("TestID-003L", new byte[] { }).Throws(typeof(ArgumentOutOfRangeException));
                yield return new TestCaseData("TestID-004L", new byte[] { }).Throws(typeof(ArgumentOutOfRangeException));
                yield return new TestCaseData("TestID-005L", new byte[] { 1, 2, 3, 1, 2, 1, 1, 23, 236 }).Throws(typeof(ArgumentOutOfRangeException));
            }
        }
        #endregion

        #region Test Code

        /// <summary>        
        /// GetPropsFromPropString Method Test      
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="propString">propString</param>
        [TestCaseSource("TestCasesOfGetPropsFromPropStringTest")]
        public void GetPropsFromPropStringTest(string testCaseID, string propString)
        {
            try
            {
                Dictionary<string, string> expected = PubCmnFunction.GetPropsFromPropString(propString);

                Dictionary<string, string> results = PubCmnFunction.GetPropsFromPropString(propString);
                Assert.AreEqual(expected, results, "Touryo.Infrastructure.Public.Util.PubCmnFunction.GetPropsFromPropString method test failed");
                Assert.AreNotEqual(propString, results, "Touryo.Infrastructure.Public.Util.PubCmnFunction.GetPropsFromPropString method test failed");
                Assert.AreNotEqual(propString, expected, "Touryo.Infrastructure.Public.Util.PubCmnFunction.GetPropsFromPropString method test failed");
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// GetCommandArgs Method Test        
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="preFix">preFix</param>
        [TestCaseSource("TestCasesOfGetCommandArgsTest")]
        public void GetCommandArgsTest(string testCaseID, char preFix)
        {
            try
            {
                Dictionary<string, string> resultArgs = new Dictionary<string, string>();
                List<string> resultList = new List<string>();
                Dictionary<string, string> expectedArgs = new Dictionary<string, string>();
                List<string> expectedList = new List<string>();
                PubCmnFunction.GetCommandArgs(preFix, out expectedArgs, out expectedList);
                PubCmnFunction.GetCommandArgs(preFix, out resultArgs, out resultList);
                Assert.AreEqual(expectedArgs, resultArgs);
                Assert.AreEqual(expectedList, resultList);
            }
            catch (Exception ex)
            {
                //Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// GetCommandArgs Method Test        
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="builtString">builtString</param>
        [TestCaseSource("TestCasesOfBuiltStringIntoEnvironmentVariableTest")]
        public void BuiltStringIntoEnvironmentVariableTest(string testCaseID, string builtString)
        {
            try
            {
                string expected = PubCmnFunction.BuiltStringIntoEnvironmentVariable(builtString);
                string result = PubCmnFunction.BuiltStringIntoEnvironmentVariable(builtString);
                Assert.AreEqual(expected, result);
                //Assert.AreNotEqual(builtString, result);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }
        [TestCaseSource("TestCasesOfCalculateSessionSizeMBTest")]
        public void CalculateSessionSizeMBTest(string testCaseID)
        {
            try
            {
                long expectedResult = PubCmnFunction.CalculateSessionSizeMB();
                long result = PubCmnFunction.CalculateSessionSizeMB();
                Assert.AreEqual(expectedResult, result);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// CalculateSessionSizeMB Method Test        
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        [TestCaseSource("TestCasesOfCalculateSessionSizeTest")]
        public void CalculateSessionSizeTest(string testCaseID)
        {
            try
            {
                long expectedResult = PubCmnFunction.CalculateSessionSize();
                long result = PubCmnFunction.CalculateSessionSize();
                Assert.AreEqual(expectedResult, result);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// CalculateSessionSizeKB Method Test        
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        [TestCaseSource("TestCasesOfCalculateSessionSizeKBTest")]
        public void CalculateSessionSizeKBTest(string testCaseID)
        {
            try
            {
                long expectedResult = PubCmnFunction.CalculateSessionSizeKB();
                long result = PubCmnFunction.CalculateSessionSizeKB();
                Assert.AreEqual(expectedResult, result);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// GetCurrentMethodName Method Test        
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        [TestCaseSource("TestCasesOfGetCurrentMethodNameTest")]
        public void GetCurrentMethodNameTest(string testCaseID)
        {
            try
            {
                string expectedResult = PubCmnFunction.GetCurrentMethodName();
                string result = PubCmnFunction.GetCurrentMethodName();
                Assert.AreEqual(expectedResult, result);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// GetCurrentMethodName Method Test        
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        [TestCaseSource("TestCasesOfGetCurrentPropertyNameTest")]
        public void GetCurrentPropertyNameTest(string testCaseID)
        {
            try
            {
                string expectedResult = PubCmnFunction.GetCurrentPropertyName();
                string result = PubCmnFunction.GetCurrentPropertyName();
                Assert.AreEqual(expectedResult, result);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// GetCurrentMethodName Method Test        
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        [TestCaseSource("TestCasesOfGetCurrentCodeInfoTest")]
        public void GetCurrentCodeInfo(string testCaseID)
        {
            try
            {
                //Expected output fields
                string expectedFilePath;
                string expectedFileLineNumber;
                string expectedMethodSignature;
                //Result output fields
                string resultFilePath;
                string resultFileLineNumber;
                string resultMethodSignature;
                PubCmnFunction.GetCurrentCodeInfo(out expectedFilePath, out expectedFileLineNumber, out expectedMethodSignature);
                PubCmnFunction.GetCurrentCodeInfo(out resultFilePath, out resultFileLineNumber, out resultMethodSignature);
                Assert.AreEqual(expectedFilePath, resultFilePath);
                int expFileLineNumber = Convert.ToInt32(expectedFileLineNumber) + 1;
                int resFileLineNumber = Convert.ToInt32(resultFileLineNumber);
                Assert.AreEqual(expFileLineNumber, resFileLineNumber);
                Assert.AreEqual(expectedMethodSignature, resultMethodSignature);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// GetCurrentMethodName Method Test        
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="str">str</param>
        /// <param name="divChar">divChar</param>
        [TestCaseSource("TestCasesOfGetFileNameNoExTest")]
        public void GetFileNameNoExTest(string testCaseID, string str, char divChar)
        {
            try
            {
                //Expected output fields
                string expectedResult;
                //Result output fields
                string result;
                expectedResult = PubCmnFunction.GetFileNameNoEx(str, divChar);
                result = PubCmnFunction.GetFileNameNoEx(str, divChar);
                Assert.AreEqual(expectedResult, result);
                Assert.AreNotEqual(str, result);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// ShortenByteArrayTest Method Test        
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="bytes">bytes</param>
        /// <param name="newSize">newSize</param>
        [TestCaseSource("TestCasesOfShortenByteArrayTest")]
        public void ShortenByteArrayTest(string testCaseID, byte[] bytes, int newSize)
        {
            try
            {
                byte[] expectedBytes = new byte[newSize];
                byte[] resultBytes = new byte[newSize];
                expectedBytes = PubCmnFunction.ShortenByteArray(bytes, newSize);
                resultBytes = PubCmnFunction.ShortenByteArray(bytes, newSize);
                Assert.AreEqual(expectedBytes, resultBytes);
                if (newSize > 0 && newSize != bytes.Length)
                    Assert.AreNotEqual(bytes, resultBytes);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// GetLongFromByteTest Method Test        
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="bytData">bytData</param>
        [TestCaseSource("TestCasesOfGetLongFromByteTest")]
        public void GetLongFromByteTest(string testCaseID, byte[] bytData)
        {
            try
            {
                long expectedLongFromByte = PubCmnFunction.GetLongFromByte(bytData);
                long resultLongFromByte = PubCmnFunction.GetLongFromByte(bytData);
                Assert.AreEqual(expectedLongFromByte, resultLongFromByte);
                Assert.AreNotEqual(bytData, resultLongFromByte);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }
        #endregion // End of TestMethods

        #endregion

    }
}

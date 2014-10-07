//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
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
//* クラス名        ：BinarySerializeTest
//* クラス日本語名  ：Test of the class to BinarySerialize
//*
//* 作成者          ：Santosh Avaji
//* 更新履歴        ：
//* 
//*  Date:        Author:          Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/08/2014   Santosh Avaji    Testcode development for BinarySerialize.
//*  08/12/2014   Sai              Added TestcaseID using SetName method as per Nishino-San comments
//*
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.IO;

namespace Public.Test.IO
{
    /// <summary>
    /// Tests for the Binary Serialize Class
    /// </summary>
    [TestFixture, Description("Tests for Binary Serialize")]
    class BinarySerializeTest
    {
        /// <summary>Test pre-processing.</summary>
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

        #region Test Data

        /// <summary>TestcasesOfObjectToBytesTest</summary>
        public IEnumerable<TestCaseData> TestcasesOfObjectToBytesTest
        {
            get
            {
                ///// Normal Test cases 
                yield return new TestCaseData("StringtoBytes").SetName("TestID-001N");
                //// pass int as object  to  convert into bytes
                yield return new TestCaseData(598).SetName("TestID-002N");
                ////datetime to bytes
                yield return new TestCaseData(System.DateTime.Now).SetName("TestID-003N");
                // Empty string
                yield return new TestCaseData(String.Empty).SetName("TestID-004A");
                /// Abnormal Test cases
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-005A");
            }
        }

        /// <summary>TestcasesOfBytesToObjectTest</summary>
        public IEnumerable<TestCaseData> TestcasesOfBytesToObjectTest
        {
            get
            {
                ////<Normal Test cases>
                yield return new TestCaseData(BinarySerialize.ObjectToBytes(new byte[] { 0, 1, 2 })).SetName("TestID-001N");
                yield return new TestCaseData(BinarySerialize.ObjectToBytes(ASCIIEncoding.UTF32.GetBytes("this is ASCIIEncoding test"))).SetName("TestID-002N");
                yield return new TestCaseData(BinarySerialize.ObjectToBytes(Encoding.UTF8.GetBytes(new char[] { }))).SetName("TestID-003N");
                yield return new TestCaseData(BinarySerialize.ObjectToBytes(Encoding.UTF8.GetBytes(new char[] { 'a', 'b' }))).SetName("TestID-004N");
                yield return new TestCaseData(BinarySerialize.ObjectToBytes(ASCIIEncoding.UTF32.GetBytes("this is ASCIIEncoding test"))).SetName("TestID-005N");
                yield return new TestCaseData(BinarySerialize.ObjectToBytes(Encoding.UTF8.GetBytes(new char[] { 'a', 'd', 'c', 'd', 'e', 'f', 'g', 'h', 'i' }, 3, 5))).SetName("TestID-006N");

                ///// <Abnormal Test cases>
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-007A");
            }
        }

        /// <summary>TestcasesOfDeepCloneTest</summary>
        public IEnumerable<TestCaseData> TestcasesOfDeepCloneTest
        {
            get
            {
                ////<Normal Test cases>
                yield return new TestCaseData(BinarySerialize.ObjectToBytes(new byte[] { 0, 1, 2 })).SetName("TestID-001N");
                yield return new TestCaseData(new byte[] { 0, 1, 2 }).SetName("TestID-002N");
                ///// <Abnormal Test cases>
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-003A");
            }
        }

        #endregion

        #region Test Code

        /// <summary>
        /// TestCasesOf ObjectToBytesTest Method
        /// </summary>        
        ///<param name="objExpected">objExpected</param>
        [TestCaseSource("TestcasesOfObjectToBytesTest")]
        public void ObjectToBytesTest(object objExpected)
        {
            try
            {
                byte[] bytFromObject = BinarySerialize.ObjectToBytes(objExpected);
                // parameter
                object objActual = BinarySerialize.BytesToObject(bytFromObject);
                Assert.AreEqual(objExpected, objActual);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestCasesOf BytesToObjectTest Method
        /// </summary>        
        ///<param name="bytActual">bytActual</param>
        [TestCaseSource("TestcasesOfBytesToObjectTest")]
        public void BytesToObjectTest(byte[] bytActual)
        {
            try
            {
                object objFromBytes = BinarySerialize.BytesToObject(bytActual);
                byte[] byteFromObject = BinarySerialize.ObjectToBytes(objFromBytes);
                Assert.AreEqual(byteFromObject, bytActual);
                Assert.AreNotEqual(objFromBytes, bytActual);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// TestCasesOf DeepCloneTest Method
        /// </summary>
        ///<param name="sourceObject">sourceObject</param>
        [TestCaseSource("TestcasesOfDeepCloneTest")]
        public void DeepCloneTest(object sourceObject)
        {
            try
            {
                object resultsObject = BinarySerialize.DeepClone(sourceObject);
                Assert.AreEqual(resultsObject, sourceObject);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
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

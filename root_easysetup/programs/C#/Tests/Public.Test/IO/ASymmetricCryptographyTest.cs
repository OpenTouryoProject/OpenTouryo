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
//* クラス名        ：ASymmetricCryptographyTest
//* クラス日本語名  ：非対称アルゴリズムによる暗号化・復号化クラスのテスト
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2014/03/31  西野  大介        新規作成
//*  08/12/2014  Sai               Added TestcaseID using SetName method as per Nishino-San comments
//*
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;

// touryo library
using Touryo.Infrastructure.Public.IO;

// testing framework
using NUnit.Framework;

namespace Public.Test.IO
{
    // - To create a test case --------------------------------------------------
    // (1) You will develop test code. 
    //     You need to think long and hard about how to assert.
    // (2) You increase the coverage rate by added normal test case.
    //     You increase the coverage rate by added abnormal test case.
    //     You increase the coverage rate by added boundary-value(limit) test case.
    // (3) If an exception occurs, Asserting exception.
    //     To record stack trace of the exception by using cosole.writeline().
    //     And consider whether or not there is a need for additional implementation of check processing from the stack trace.
    // --------------------------------------------------------------------------

    [TestFixture]
    public class ASymmetricCryptographyTest
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

        #region Test data

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method EncryptStringTest.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfEncryptStringTest
        {
            get
            {
                // If you need to prepare for the test data below so, you use the TestCaseData.

                // Get public key and private key.
                string publicKey;
                string privateKey;
                ASymmetricCryptography.GetKeys(out publicKey, out privateKey);

                // Public key that is obtained, using the private key.
                yield return new TestCaseData("abcde", publicKey, privateKey).SetName("TestID-001N");
                yield return new TestCaseData("あいうえお", publicKey, privateKey).SetName("TestID-002N");
                yield return new TestCaseData(string.Empty, publicKey, privateKey).SetName("TestID-003L");
                yield return new TestCaseData(null, publicKey, privateKey).Throws(typeof(ArgumentNullException)).SetName("TestID-004A");

                // This test case using a dummy string to the private key.
                yield return new TestCaseData("abcde", publicKey, "dummy").Throws(typeof(System.Security.XmlSyntaxException)).SetName("TestID-005A");
                yield return new TestCaseData("あいうえお", publicKey, "dummy").Throws(typeof(System.Security.XmlSyntaxException)).SetName("TestID-006A");
                yield return new TestCaseData(string.Empty, publicKey, "dummy").Throws(typeof(System.Security.XmlSyntaxException)).SetName("TestID-007A");
                yield return new TestCaseData(null, publicKey, "dummy").Throws(typeof(ArgumentNullException)).SetName("TestID-008A");

                // This test case using a dummy string in the public key.
                yield return new TestCaseData("abcde", "dummy", privateKey).Throws(typeof(System.Security.XmlSyntaxException)).SetName("TestID-009A");
                yield return new TestCaseData("あいうえお", "dummy", privateKey).Throws(typeof(System.Security.XmlSyntaxException)).SetName("TestID-010A");
                yield return new TestCaseData(string.Empty, "dummy", privateKey).Throws(typeof(System.Security.XmlSyntaxException)).SetName("TestID-011A");
                yield return new TestCaseData(null, "dummy", privateKey).Throws(typeof(ArgumentNullException)).SetName("TestID-012A");

                // This test case using an empty private key.
                yield return new TestCaseData("abcde", publicKey, string.Empty).Throws(typeof(System.Security.XmlSyntaxException)).SetName("TestID-013A");
                yield return new TestCaseData("あいうえお", publicKey, string.Empty).Throws(typeof(System.Security.XmlSyntaxException)).SetName("TestID-014A");
                yield return new TestCaseData(string.Empty, publicKey, string.Empty).Throws(typeof(System.Security.XmlSyntaxException)).SetName("TestID-015A");
                yield return new TestCaseData(null, publicKey, string.Empty).Throws(typeof(ArgumentNullException)).SetName("TestID-016A");

                // This test case using an empty public key
                yield return new TestCaseData("abcde", string.Empty, privateKey).Throws(typeof(System.Security.XmlSyntaxException)).SetName("TestID-017A");
                yield return new TestCaseData("あいうえお", string.Empty, privateKey).Throws(typeof(System.Security.XmlSyntaxException)).SetName("TestID-018A");
                yield return new TestCaseData(string.Empty, string.Empty, privateKey).Throws(typeof(System.Security.XmlSyntaxException)).SetName("TestID-019A");
                yield return new TestCaseData(null, string.Empty, privateKey).Throws(typeof(ArgumentNullException)).SetName("TestID-020A");

                // This test case using the private key to Null
                yield return new TestCaseData("abcde", publicKey, null).Throws(typeof(ArgumentNullException)).SetName("TestID-021A");
                yield return new TestCaseData("あいうえお", publicKey, null).Throws(typeof(ArgumentNullException)).SetName("TestID-022A");
                yield return new TestCaseData(string.Empty, publicKey, null).Throws(typeof(ArgumentNullException)).SetName("TestID-023A");
                yield return new TestCaseData(null, publicKey, null).Throws(typeof(ArgumentNullException)).SetName("TestID-024A");

                // This test case using the public key to Null
                yield return new TestCaseData("abcde", null, privateKey).Throws(typeof(ArgumentNullException)).SetName("TestID-025A");
                yield return new TestCaseData("あいうえお", null, privateKey).Throws(typeof(ArgumentNullException)).SetName("TestID-026A");
                yield return new TestCaseData(string.Empty, null, privateKey).Throws(typeof(ArgumentNullException)).SetName("TestID-027A");
                yield return new TestCaseData(null, null, privateKey).Throws(typeof(ArgumentNullException)).SetName("TestID-028A");
            }
        }

        #endregion

        #region Test Code

        ///<summary>Test execution.(CheckListID should be the first argument).</summary>         
        ///<param name="sourceString">String to be encrypted.</param> 
        ///<param name="publicKey">public key</param> 
        ///<param name="privateKey">private key</param>
        [TestCaseSource("TestCasesOfEncryptStringTest")]
        public void EncryptStringTest(string sourceString, string publicKey, string privateKey)
        {
            try
            {
                // Performs encryption using the components touryo.
                string encryptedString = ASymmetricCryptography.EncryptString(sourceString, publicKey);

                // Performs decrypted using the components touryo.
                string decryptedString = ASymmetricCryptography.DecryptString(encryptedString, privateKey);

                // Check whether it is decrypted into the original string.
                Assert.AreNotEqual(sourceString, encryptedString);
                Assert.AreEqual(sourceString, decryptedString);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
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

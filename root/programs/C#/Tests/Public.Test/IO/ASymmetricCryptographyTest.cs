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

                this.SetUp();

                // Public key that is obtained, using the private key.
                yield return new TestCaseData("TestID-001N", "abcde", publicKey, privateKey);
                yield return new TestCaseData("TestID-002N", "あいうえお", publicKey, privateKey);
                yield return new TestCaseData("TestID-003L", string.Empty, publicKey, privateKey);
                yield return new TestCaseData("TestID-004A", null, publicKey, privateKey).Throws(typeof(ArgumentNullException));

                // This test case using a dummy string to the private key.
                yield return new TestCaseData("TestID-005A", "abcde", publicKey, "dummy").Throws(typeof(System.Security.XmlSyntaxException));
                yield return new TestCaseData("TestID-006A", "あいうえお", publicKey, "dummy").Throws(typeof(System.Security.XmlSyntaxException));
                yield return new TestCaseData("TestID-007A", string.Empty, publicKey, "dummy").Throws(typeof(System.Security.XmlSyntaxException));
                yield return new TestCaseData("TestID-008A", null, publicKey, "dummy").Throws(typeof(ArgumentNullException));

                // This test case using a dummy string in the public key.
                yield return new TestCaseData("TestID-009A", "abcde", "dummy", privateKey).Throws(typeof(System.Security.XmlSyntaxException));
                yield return new TestCaseData("TestID-010A", "あいうえお", "dummy", privateKey).Throws(typeof(System.Security.XmlSyntaxException));
                yield return new TestCaseData("TestID-011A", string.Empty, "dummy", privateKey).Throws(typeof(System.Security.XmlSyntaxException));
                yield return new TestCaseData("TestID-012A", null, "dummy", privateKey).Throws(typeof(ArgumentNullException));

                // This test case using an empty private key.
                yield return new TestCaseData("TestID-013A", "abcde", publicKey, string.Empty).Throws(typeof(System.Security.XmlSyntaxException));
                yield return new TestCaseData("TestID-014A", "あいうえお", publicKey, string.Empty).Throws(typeof(System.Security.XmlSyntaxException));
                yield return new TestCaseData("TestID-015A", string.Empty, publicKey, string.Empty).Throws(typeof(System.Security.XmlSyntaxException));
                yield return new TestCaseData("TestID-016A", null, publicKey, string.Empty).Throws(typeof(ArgumentNullException));

                // This test case using an empty public key
                yield return new TestCaseData("TestID-017A", "abcde", string.Empty, privateKey).Throws(typeof(System.Security.XmlSyntaxException));
                yield return new TestCaseData("TestID-018A", "あいうえお", string.Empty, privateKey).Throws(typeof(System.Security.XmlSyntaxException));
                yield return new TestCaseData("TestID-019A", string.Empty, string.Empty, privateKey).Throws(typeof(System.Security.XmlSyntaxException));
                yield return new TestCaseData("TestID-020A", null, string.Empty, privateKey).Throws(typeof(ArgumentNullException));

                // This test case using the private key to Null
                yield return new TestCaseData("TestID-021A", "abcde", publicKey, null).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-022A", "あいうえお", publicKey, null).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-023A", string.Empty, publicKey, null).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-024A", null, publicKey, null).Throws(typeof(ArgumentNullException));

                // This test case using the public key to Null
                yield return new TestCaseData("TestID-025A", "abcde", null, privateKey).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-026A", "あいうえお", null, privateKey).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-027A", string.Empty, null, privateKey).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-028A", null, null, privateKey).Throws(typeof(ArgumentNullException));
            }
        }

        #endregion

        #region Test Code

        ///<summary>Test execution.(CheckListID should be the first argument).</summary> 
        ///<param name="testCaseID">test case ID</param> 
        ///<param name="sourceString">String to be encrypted.</param> 
        ///<param name="publicKey">public key</param> 
        ///<param name="privateKey">private key</param>
        [TestCaseSource("TestCasesOfEncryptStringTest")]
        public void EncryptStringTest(string testCaseID, string sourceString, string publicKey, string privateKey)
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
            catch(Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
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

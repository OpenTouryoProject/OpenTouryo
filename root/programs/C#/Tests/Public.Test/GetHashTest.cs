//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//* クラス名        ：GetHashTest
//* クラス日本語名  ：ハッシュを取得するクラスのテスト
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

// touryo library
using Touryo.Infrastructure.Public.Util;

// testing framework
using NUnit.Framework;

namespace Public.Test
{
    [TestFixture]
    public class GetHashTest
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

        #region Test Code
        
        /// <summary>Test execution.(CheckListID should be the first argument)</summary>
        /// <param name="testCaseID">test case ID</param> 
        /// <param name="sourceString">The original string to get the hash value.</param>
        /// <param name="eha">Hash algorithm</param>
        [TestCase("TestID-001N", "abcde", EnumHashAlgorithm.Default)]
        [TestCase("TestID-002N", "あいうえお", EnumHashAlgorithm.Default)]
        [TestCase("TestID-003L", "", EnumHashAlgorithm.Default)]
        [TestCase("TestID-004A", null, EnumHashAlgorithm.Default, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-005N", "abcde", EnumHashAlgorithm.MD5CryptoServiceProvider)]
        [TestCase("TestID-006N", "あいうえお", EnumHashAlgorithm.MD5CryptoServiceProvider)]
        [TestCase("TestID-007L", "", EnumHashAlgorithm.MD5CryptoServiceProvider)]
        [TestCase("TestID-008A", null, EnumHashAlgorithm.MD5CryptoServiceProvider, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-009N", "abcde", EnumHashAlgorithm.SHA1CryptoServiceProvider)]
        [TestCase("TestID-010N", "あいうえお", EnumHashAlgorithm.SHA1CryptoServiceProvider)]
        [TestCase("TestID-011L", "", EnumHashAlgorithm.SHA1CryptoServiceProvider)]
        [TestCase("TestID-012A", null, EnumHashAlgorithm.SHA1CryptoServiceProvider, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-013N", "abcde", EnumHashAlgorithm.SHA1Managed)]
        [TestCase("TestID-014N", "あいうえお", EnumHashAlgorithm.SHA1Managed)]
        [TestCase("TestID-015L", "", EnumHashAlgorithm.SHA1Managed)]
        [TestCase("TestID-016A", null, EnumHashAlgorithm.SHA1Managed, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-017N", "abcde", EnumHashAlgorithm.SHA256Managed)]
        [TestCase("TestID-018N", "あいうえお", EnumHashAlgorithm.SHA256Managed)]
        [TestCase("TestID-019L", "", EnumHashAlgorithm.SHA256Managed)]
        [TestCase("TestID-020A", null, EnumHashAlgorithm.SHA256Managed, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-021N", "abcde", EnumHashAlgorithm.SHA384Managed)]
        [TestCase("TestID-022N", "あいうえお", EnumHashAlgorithm.SHA384Managed)]
        [TestCase("TestID-023L", "", EnumHashAlgorithm.SHA384Managed)]
        [TestCase("TestID-024A", null, EnumHashAlgorithm.SHA384Managed, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-025N", "abcde", EnumHashAlgorithm.SHA512Managed)]
        [TestCase("TestID-026N", "あいうえお", EnumHashAlgorithm.SHA512Managed)]
        [TestCase("TestID-027L", "", EnumHashAlgorithm.SHA512Managed)]
        [TestCase("TestID-028A", null, EnumHashAlgorithm.SHA512Managed, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-029A", "abcde", 999, ExpectedException = typeof(NullReferenceException))]        // The encryption method that is not defined
        [TestCase("TestID-030A", "あいうえお", 999, ExpectedException = typeof(NullReferenceException))]   // The encryption method that is not defined
        [TestCase("TestID-031A", "", 999, ExpectedException = typeof(NullReferenceException))]             // The encryption method that is not defined
        [TestCase("TestID-032A", null, 999, ExpectedException = typeof(ArgumentNullException))]            // The encryption method that is not defined
        public void EncryptStringTest(string testCaseID, string sourceString, EnumHashAlgorithm eha)
        {
            try
            {
                // Get the hash value using the components of touryo. 
                string hashString = GetHash.GetHashString(sourceString, eha);

                // Using the components of touryo, and get the hash value again.
                string hashString2 = GetHash.GetHashString(sourceString, eha);

                // Check the hash value.
                Assert.AreNotEqual(sourceString, hashString);
                Assert.AreNotEqual(sourceString, hashString2);
                Assert.AreEqual(hashString, hashString2);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>>Test execution.(CheckListID should be the first argument)</summary>
        /// <param name="testCaseID">test case ID</param> 
        /// <param name="rawPwd">Raw password</param>
        /// <param name="eha">Hash algorithm</param>
        /// <param name="saltLength">Salt length</param>
        [TestCase("TestID-001N", "test@123", EnumHashAlgorithm.Default, 10)]
        [TestCase("TestID-002A", "test@123", EnumHashAlgorithm.Default, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-003A", "test@123", EnumHashAlgorithm.Default, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-004N", "test@123", EnumHashAlgorithm.MD5CryptoServiceProvider, 10)]
        [TestCase("TestID-005A", "test@123", EnumHashAlgorithm.MD5CryptoServiceProvider, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-006A", "test@123", EnumHashAlgorithm.MD5CryptoServiceProvider, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-007N", "test@123", EnumHashAlgorithm.SHA1CryptoServiceProvider, 10)]
        [TestCase("TestID-008A", "test@123", EnumHashAlgorithm.SHA1CryptoServiceProvider, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-009A", "test@123", EnumHashAlgorithm.SHA1CryptoServiceProvider, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-010N", "test@123", EnumHashAlgorithm.SHA1Managed, 10)]
        [TestCase("TestID-011A", "test@123", EnumHashAlgorithm.SHA1Managed, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-012A", "test@123", EnumHashAlgorithm.SHA1Managed, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-013N", "test@123", EnumHashAlgorithm.SHA256Managed, 10)]
        [TestCase("TestID-014A", "test@123", EnumHashAlgorithm.SHA256Managed, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-015A", "test@123", EnumHashAlgorithm.SHA256Managed, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-016N", "test@123", EnumHashAlgorithm.SHA384Managed, 10)]
        [TestCase("TestID-017A", "test@123", EnumHashAlgorithm.SHA384Managed, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-018A", "test@123", EnumHashAlgorithm.SHA384Managed, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-019N", "test@123", EnumHashAlgorithm.SHA512Managed, 10)]
        [TestCase("TestID-020A", "test@123", EnumHashAlgorithm.SHA512Managed, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-021A", "test@123", EnumHashAlgorithm.SHA512Managed, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-022A", "test@123", 999, 10, ExpectedException = typeof(NullReferenceException))]
        [TestCase("TestID-023A", "test@123", 999, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-024A", "test@123", 999, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-025N", "", EnumHashAlgorithm.Default, 10)]
        [TestCase("TestID-026A", "", EnumHashAlgorithm.Default, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-027A", "", EnumHashAlgorithm.Default, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-028N", "", EnumHashAlgorithm.MD5CryptoServiceProvider, 10)]
        [TestCase("TestID-029A", "", EnumHashAlgorithm.MD5CryptoServiceProvider, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-030A", "", EnumHashAlgorithm.MD5CryptoServiceProvider, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-031N", "", EnumHashAlgorithm.SHA1CryptoServiceProvider, 10)]
        [TestCase("TestID-032A", "", EnumHashAlgorithm.SHA1CryptoServiceProvider, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-033A", "", EnumHashAlgorithm.SHA1CryptoServiceProvider, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-034N", "", EnumHashAlgorithm.SHA1Managed, 10)]
        [TestCase("TestID-035A", "", EnumHashAlgorithm.SHA1Managed, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-036A", "", EnumHashAlgorithm.SHA1Managed, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-037N", "", EnumHashAlgorithm.SHA256Managed, 10)]
        [TestCase("TestID-038A", "", EnumHashAlgorithm.SHA256Managed, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-039A", "", EnumHashAlgorithm.SHA256Managed, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-040N", "", EnumHashAlgorithm.SHA384Managed, 10)]
        [TestCase("TestID-041A", "", EnumHashAlgorithm.SHA384Managed, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-042A", "", EnumHashAlgorithm.SHA384Managed, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-043N", "", EnumHashAlgorithm.SHA512Managed, 10)]
        [TestCase("TestID-044A", "", EnumHashAlgorithm.SHA512Managed, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-045A", "", EnumHashAlgorithm.SHA512Managed, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-046N", null, EnumHashAlgorithm.Default, 10)]
        [TestCase("TestID-047A", null, EnumHashAlgorithm.Default, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-048A", null, EnumHashAlgorithm.Default, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-049N", null, EnumHashAlgorithm.MD5CryptoServiceProvider, 10)]
        [TestCase("TestID-050A", null, EnumHashAlgorithm.MD5CryptoServiceProvider, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-051A", null, EnumHashAlgorithm.MD5CryptoServiceProvider, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-052N", null, EnumHashAlgorithm.SHA1CryptoServiceProvider, 10)]
        [TestCase("TestID-053A", null, EnumHashAlgorithm.SHA1CryptoServiceProvider, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-054A", null, EnumHashAlgorithm.SHA1CryptoServiceProvider, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-055N", null, EnumHashAlgorithm.SHA1Managed, 10)]
        [TestCase("TestID-056A", null, EnumHashAlgorithm.SHA1Managed, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-057A", null, EnumHashAlgorithm.SHA1Managed, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-058N", null, EnumHashAlgorithm.SHA256Managed, 10)]
        [TestCase("TestID-059A", null, EnumHashAlgorithm.SHA256Managed, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-060A", null, EnumHashAlgorithm.SHA256Managed, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-061N", null, EnumHashAlgorithm.SHA384Managed, 10)]
        [TestCase("TestID-062A", null, EnumHashAlgorithm.SHA384Managed, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-063A", null, EnumHashAlgorithm.SHA384Managed, -1, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-064N", null, EnumHashAlgorithm.SHA512Managed, 10)]
        [TestCase("TestID-065A", null, EnumHashAlgorithm.SHA512Managed, 0, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-066A", null, EnumHashAlgorithm.SHA512Managed, -1, ExpectedException = typeof(ArgumentException))]
        public void GetSaltedPasswdTest(string testCaseID, string rawPwd, EnumHashAlgorithm eha, int saltLength)
        {
            try
            {
                // Get the Salt password using the components of touryo. 
                string saltedPasswd = GetHash.GetSaltedPasswd(rawPwd, eha, saltLength);

                // Check salt password to see if they match.
                Assert.IsTrue(GetHash.EqualSaltedPasswd(rawPwd, saltedPasswd, eha, saltLength));

                // (Just in case) Salt length is different, make sure that the salt passwords do not match.
                Assert.IsFalse(GetHash.EqualSaltedPasswd(rawPwd, saltedPasswd, eha, saltLength + 1));
            }
            catch (Exception ex)
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

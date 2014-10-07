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
//* クラス名        ：GetHashTest
//* クラス日本語名  ：ハッシュを取得するクラスのテスト
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2014/03/31  西野  大介        新規作成
//*  08/11/2014   Sai              Added TestcaseID using TestName property as per Nishino-San comments
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

namespace Public.Test.Util
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
        /// <param name="sourceString">The original string to get the hash value.</param>
        /// <param name="eha">Hash algorithm</param>
        [TestCase("abcde", EnumHashAlgorithm.Default, TestName = "TestID-001N")]
        [TestCase("あいうえお", EnumHashAlgorithm.Default, TestName = "TestID-002N")]
        [TestCase("", EnumHashAlgorithm.Default, TestName = "TestID-003L")]
        [TestCase(null, EnumHashAlgorithm.Default, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-004A")]
        [TestCase("abcde", EnumHashAlgorithm.MD5CryptoServiceProvider, TestName = "TestID-005N")]
        [TestCase("あいうえお", EnumHashAlgorithm.MD5CryptoServiceProvider, TestName = "TestID-006N")]
        [TestCase("", EnumHashAlgorithm.MD5CryptoServiceProvider, TestName = "TestID-007L")]
        [TestCase(null, EnumHashAlgorithm.MD5CryptoServiceProvider, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-007L")]
        [TestCase("abcde", EnumHashAlgorithm.SHA1CryptoServiceProvider, TestName = "TestID-009N")]
        [TestCase("あいうえお", EnumHashAlgorithm.SHA1CryptoServiceProvider, TestName = "TestID-010N")]
        [TestCase("", EnumHashAlgorithm.SHA1CryptoServiceProvider, TestName = "TestID-011L")]
        [TestCase(null, EnumHashAlgorithm.SHA1CryptoServiceProvider, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-012A")]
        [TestCase("abcde", EnumHashAlgorithm.SHA1Managed, TestName = "TestID-013N")]
        [TestCase("あいうえお", EnumHashAlgorithm.SHA1Managed, TestName = "TestID-014N")]
        [TestCase("", EnumHashAlgorithm.SHA1Managed, TestName = "TestID-015L")]
        [TestCase(null, EnumHashAlgorithm.SHA1Managed, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-016A")]
        [TestCase("abcde", EnumHashAlgorithm.SHA256Managed, TestName = "TestID-017N")]
        [TestCase("あいうえお", EnumHashAlgorithm.SHA256Managed, TestName = "TestID-018N")]
        [TestCase("", EnumHashAlgorithm.SHA256Managed, TestName = "TestID-019L")]
        [TestCase(null, EnumHashAlgorithm.SHA256Managed, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-020A")]
        [TestCase("abcde", EnumHashAlgorithm.SHA384Managed, TestName = "TestID-021N")]
        [TestCase("あいうえお", EnumHashAlgorithm.SHA384Managed, TestName = "TestID-022N")]
        [TestCase("", EnumHashAlgorithm.SHA384Managed, TestName = "TestID-023L")]
        [TestCase(null, EnumHashAlgorithm.SHA384Managed, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-024A")]
        [TestCase("abcde", EnumHashAlgorithm.SHA512Managed, TestName = "TestID-025N")]
        [TestCase("あいうえお", EnumHashAlgorithm.SHA512Managed, TestName = "TestID-026N")]
        [TestCase("", EnumHashAlgorithm.SHA512Managed, TestName = "TestID-027L")]
        [TestCase(null, EnumHashAlgorithm.SHA512Managed, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-028A")]
        [TestCase("abcde", 999, ExpectedException = typeof(NullReferenceException), TestName = "TestID-029A")]        // The encryption method that is not defined
        [TestCase("あいうえお", 999, ExpectedException = typeof(NullReferenceException), TestName = "TestID-030A")]   // The encryption method that is not defined
        [TestCase("", 999, ExpectedException = typeof(NullReferenceException), TestName = "TestID-031A")]             // The encryption method that is not defined
        [TestCase(null, 999, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-032A")]            // The encryption method that is not defined
        public void EncryptStringTest(string sourceString, EnumHashAlgorithm eha)
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
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>>Test execution.(CheckListID should be the first argument)</summary>
        /// <param name="rawPwd">Raw password</param>
        /// <param name="eha">Hash algorithm</param>
        /// <param name="saltLength">Salt length</param>
        [TestCase("test@123", EnumHashAlgorithm.Default, 10, TestName = "TestID-001N")]
        [TestCase("test@123", EnumHashAlgorithm.Default, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-002A")]
        [TestCase("test@123", EnumHashAlgorithm.Default, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-003A")]
        [TestCase("test@123", EnumHashAlgorithm.MD5CryptoServiceProvider, 10, TestName = "TestID-004N")]
        [TestCase("test@123", EnumHashAlgorithm.MD5CryptoServiceProvider, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-005A")]
        [TestCase("test@123", EnumHashAlgorithm.MD5CryptoServiceProvider, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-006A")]
        [TestCase("test@123", EnumHashAlgorithm.SHA1CryptoServiceProvider, 10, TestName = "TestID-007N")]
        [TestCase("test@123", EnumHashAlgorithm.SHA1CryptoServiceProvider, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-008A")]
        [TestCase("test@123", EnumHashAlgorithm.SHA1CryptoServiceProvider, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-009A")]
        [TestCase("test@123", EnumHashAlgorithm.SHA1Managed, 10, TestName = "TestID-010N")]
        [TestCase("test@123", EnumHashAlgorithm.SHA1Managed, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-011A")]
        [TestCase("test@123", EnumHashAlgorithm.SHA1Managed, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-012A")]
        [TestCase("test@123", EnumHashAlgorithm.SHA256Managed, 10, TestName = "TestID-013N")]
        [TestCase("test@123", EnumHashAlgorithm.SHA256Managed, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-014A")]
        [TestCase("test@123", EnumHashAlgorithm.SHA256Managed, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-015A")]
        [TestCase("test@123", EnumHashAlgorithm.SHA384Managed, 10, TestName = "TestID-016N")]
        [TestCase("test@123", EnumHashAlgorithm.SHA384Managed, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-017A")]
        [TestCase("test@123", EnumHashAlgorithm.SHA384Managed, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-018A")]
        [TestCase("test@123", EnumHashAlgorithm.SHA512Managed, 10, TestName = "TestID-019N")]
        [TestCase("test@123", EnumHashAlgorithm.SHA512Managed, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-020A")]
        [TestCase("test@123", EnumHashAlgorithm.SHA512Managed, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-021A")]
        [TestCase("test@123", 999, 10, ExpectedException = typeof(NullReferenceException), TestName = "TestID-022A")]
        [TestCase("test@123", 999, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-023A")]
        [TestCase("test@123", 999, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-024A")]
        [TestCase("", EnumHashAlgorithm.Default, 10, TestName = "TestID-025N")]
        [TestCase("", EnumHashAlgorithm.Default, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-026A")]
        [TestCase("", EnumHashAlgorithm.Default, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-027A")]
        [TestCase("", EnumHashAlgorithm.MD5CryptoServiceProvider, 10, TestName = "TestID-028N")]
        [TestCase("", EnumHashAlgorithm.MD5CryptoServiceProvider, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-029A")]
        [TestCase("", EnumHashAlgorithm.MD5CryptoServiceProvider, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-030A")]
        [TestCase("", EnumHashAlgorithm.SHA1CryptoServiceProvider, 10, TestName = "TestID-031N")]
        [TestCase("", EnumHashAlgorithm.SHA1CryptoServiceProvider, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-032A")]
        [TestCase("", EnumHashAlgorithm.SHA1CryptoServiceProvider, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-033A")]
        [TestCase("", EnumHashAlgorithm.SHA1Managed, 10, TestName = "TestID-034N")]
        [TestCase("", EnumHashAlgorithm.SHA1Managed, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-035A")]
        [TestCase("", EnumHashAlgorithm.SHA1Managed, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-036A")]
        [TestCase("", EnumHashAlgorithm.SHA256Managed, 10, TestName = "TestID-037N")]
        [TestCase("", EnumHashAlgorithm.SHA256Managed, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-038A")]
        [TestCase("", EnumHashAlgorithm.SHA256Managed, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-039A")]
        [TestCase("", EnumHashAlgorithm.SHA384Managed, 10, TestName = "TestID-040N")]
        [TestCase("", EnumHashAlgorithm.SHA384Managed, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-041A")]
        [TestCase("", EnumHashAlgorithm.SHA384Managed, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-042A")]
        [TestCase("", EnumHashAlgorithm.SHA512Managed, 10, TestName = "TestID-043N")]
        [TestCase("", EnumHashAlgorithm.SHA512Managed, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-044A")]
        [TestCase("", EnumHashAlgorithm.SHA512Managed, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-045A")]
        [TestCase(null, EnumHashAlgorithm.Default, 10, TestName = "TestID-046N")]
        [TestCase(null, EnumHashAlgorithm.Default, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-047A")]
        [TestCase(null, EnumHashAlgorithm.Default, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-048A")]
        [TestCase(null, EnumHashAlgorithm.MD5CryptoServiceProvider, 10, TestName = "TestID-049N")]
        [TestCase(null, EnumHashAlgorithm.MD5CryptoServiceProvider, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-050A")]
        [TestCase(null, EnumHashAlgorithm.MD5CryptoServiceProvider, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-051A")]
        [TestCase(null, EnumHashAlgorithm.SHA1CryptoServiceProvider, 10, TestName = "TestID-052N")]
        [TestCase(null, EnumHashAlgorithm.SHA1CryptoServiceProvider, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-053A")]
        [TestCase(null, EnumHashAlgorithm.SHA1CryptoServiceProvider, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-054A")]
        [TestCase(null, EnumHashAlgorithm.SHA1Managed, 10, TestName = "TestID-055N")]
        [TestCase(null, EnumHashAlgorithm.SHA1Managed, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-056A")]
        [TestCase(null, EnumHashAlgorithm.SHA1Managed, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-057A")]
        [TestCase(null, EnumHashAlgorithm.SHA256Managed, 10, TestName = "TestID-058N")]
        [TestCase(null, EnumHashAlgorithm.SHA256Managed, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-059A")]
        [TestCase(null, EnumHashAlgorithm.SHA256Managed, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-060A")]
        [TestCase(null, EnumHashAlgorithm.SHA384Managed, 10, TestName = "TestID-061N")]
        [TestCase(null, EnumHashAlgorithm.SHA384Managed, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-062A")]
        [TestCase(null, EnumHashAlgorithm.SHA384Managed, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-063A")]
        [TestCase(null, EnumHashAlgorithm.SHA512Managed, 10, TestName = "TestID-064N")]
        [TestCase(null, EnumHashAlgorithm.SHA512Managed, 0, ExpectedException = typeof(ArgumentException), TestName = "TestID-065A")]
        [TestCase(null, EnumHashAlgorithm.SHA512Managed, -1, ExpectedException = typeof(ArgumentException), TestName = "TestID-066A")]
        public void GetSaltedPasswdTest(string rawPwd, EnumHashAlgorithm eha, int saltLength)
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

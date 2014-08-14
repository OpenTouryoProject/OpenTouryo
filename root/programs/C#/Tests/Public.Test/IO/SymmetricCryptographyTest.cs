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
//* クラス名        ：SymmetricCryptographyTest
//* クラス日本語名  ：対称アルゴリズムによる暗号化・復号化クラスのテスト
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2014/03/31  西野  大介        新規作成
//*  08/12/2014   Sai              Added TestcaseID using SetName method as per Nishino-San comments
//*
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public class SymmetricCryptographyTest
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
        /// This method to generate test data to be passed to the method EncryptStringTest2.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfEncryptStringTest2
        {
            get
            {
                // If you need to prepare for the test data below so, you use the TestCaseData.

                // Define the four type salt.
                byte[] UTF8Salt = Encoding.UTF8.GetBytes("Touryo.Infrastructure.Public.IO.SymmetricCryptography.Salt");
                byte[] SJISSalt = Encoding.GetEncoding(932).GetBytes("Touryo.Infrastructure.Public.IO.SymmetricCryptography.Salt");
                byte[] EmptySalt = new byte[0];
                byte[] NullSalt = null;

                // Salt which has been converted into a byte array using the UTF-8 encoding.
                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).SetName("TestID-001N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).SetName("TestID-002N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).SetName("TestID-003L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).Throws(typeof(ArgumentNullException)).SetName("TestID-004A");
                yield return new TestCaseData("abcde", "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).SetName("TestID-005L");
                yield return new TestCaseData("abcde", null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).Throws(typeof(ArgumentNullException)).SetName("TestID-006A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt).SetName("TestID-007N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt).SetName("TestID-008N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt).SetName("TestID-009L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt).Throws(typeof(ArgumentNullException)).SetName("TestID-010A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt).SetName("TestID-011N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt).SetName("TestID-012N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt).SetName("TestID-013L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt).Throws(typeof(ArgumentNullException)).SetName("TestID-014A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt).SetName("TestID-015N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt).SetName("TestID-016N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt).SetName("TestID-017L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt).Throws(typeof(ArgumentNullException)).SetName("TestID-018A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt).SetName("TestID-019N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt).SetName("TestID-020N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt).SetName("TestID-021L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt).Throws(typeof(ArgumentNullException)).SetName("TestID-022A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt).SetName("TestID-023N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt).SetName("TestID-024N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt).SetName("TestID-025L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt).Throws(typeof(ArgumentNullException)).SetName("TestID-026A");

                yield return new TestCaseData("abcde", "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException)).SetName("TestID-027N");
                yield return new TestCaseData("あいうえお", "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException)).SetName("TestID-028N");
                yield return new TestCaseData(string.Empty, "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException)).SetName("TestID-029L");
                yield return new TestCaseData(null, "test@123", 999, UTF8Salt).Throws(typeof(ArgumentNullException)).SetName("TestID-030A");

                // Salt which has been converted into a byte array using the Shift-JIS encoding.
                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).SetName("TestID-031N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).SetName("TestID-032N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).SetName("TestID-033L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-034A");
                yield return new TestCaseData("abcde", "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).SetName("TestID-035L");
                yield return new TestCaseData("abcde", null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-036A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt).SetName("TestID-037N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt).SetName("TestID-038N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt).SetName("TestID-039L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-040A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt).SetName("TestID-041N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt).SetName("TestID-042N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt).SetName("TestID-043L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-044A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt).SetName("TestID-045N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt).SetName("TestID-046N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt).SetName("TestID-047L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-048A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt).SetName("TestID-049N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt).SetName("TestID-050N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt).SetName("TestID-051L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-052A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt).SetName("TestID-053N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt).SetName("TestID-054N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt).SetName("TestID-055L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-056A");

                yield return new TestCaseData("abcde", "test@123", 999, SJISSalt).Throws(typeof(ArgumentException)).SetName("TestID-057N");
                yield return new TestCaseData("あいうえお", "test@123", 999, SJISSalt).Throws(typeof(ArgumentException)).SetName("TestID-058N");
                yield return new TestCaseData(string.Empty, "test@123", 999, SJISSalt).Throws(typeof(ArgumentException)).SetName("TestID-059L");
                yield return new TestCaseData(null, "test@123", 999, SJISSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-060A");

                // empty byte array salt
                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-061N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-062N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-063L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentNullException)).SetName("TestID-064A");
                yield return new TestCaseData("abcde", "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-065L");
                yield return new TestCaseData("abcde", null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentNullException)).SetName("TestID-066A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-067N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-068N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-069L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentNullException)).SetName("TestID-070A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-071N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-072N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-073L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentNullException)).SetName("TestID-074A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-075N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-076N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-077L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentNullException)).SetName("TestID-078A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-079N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-080N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-081L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentNullException)).SetName("TestID-082A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-083N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-084N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-085L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentNullException)).SetName("TestID-086A");

                yield return new TestCaseData("abcde", "test@123", 999, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-087N");
                yield return new TestCaseData("あいうえお", "test@123", 999, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-088N");
                yield return new TestCaseData(string.Empty, "test@123", 999, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-089L");
                yield return new TestCaseData(null, "test@123", 999, EmptySalt).Throws(typeof(ArgumentNullException)).SetName("TestID-090A");

                // null salt.
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-091N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-092L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-093A");
                yield return new TestCaseData("abcde", "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-094L");
                yield return new TestCaseData("abcde", null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-095A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-096N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-097N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-098L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-099A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-100N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-101N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-102L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-103A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-104N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-105N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-106L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-107A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-108N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-109N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-110L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-111A");

                yield return new TestCaseData("abcde", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-112N");
                yield return new TestCaseData("あいうえお", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-113N");
                yield return new TestCaseData(string.Empty, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-114L");
                yield return new TestCaseData(null, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-115A");

                yield return new TestCaseData("abcde", "test@123", 999, NullSalt).Throws(typeof(ArgumentException)).SetName("TestID-116N");
                yield return new TestCaseData("あいうえお", "test@123", 999, NullSalt).Throws(typeof(ArgumentException)).SetName("TestID-117N");
                yield return new TestCaseData(string.Empty, "test@123", 999, NullSalt).Throws(typeof(ArgumentException)).SetName("TestID-118L");
                yield return new TestCaseData(null, "test@123", 999, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-119A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method EncryptBytesTest.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfEncryptBytesTest
        {
            get
            {
                // If you need to prepare for the test data below so, you use the TestCaseData.

                // Convert the string to a byte array to be encrypted.
                byte[] abcdeBytes = Encoding.UTF8.GetBytes("abcde");
                byte[] aiueoBytes = Encoding.UTF8.GetBytes("あいうえお");
                byte[] emptyBytes = new Byte[0];
                byte[] nullBytes = null;

                // AesCryptoServiceProvider
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider).SetName("TestID-001N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider).SetName("TestID-002N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider).SetName("TestID-003L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider).Throws(typeof(NullReferenceException)).SetName("TestID-004A");
                yield return new TestCaseData(abcdeBytes, "", EnumSymmetricAlgorithm.AesCryptoServiceProvider).SetName("TestID-005L");
                yield return new TestCaseData(abcdeBytes, null, EnumSymmetricAlgorithm.AesCryptoServiceProvider).Throws(typeof(ArgumentNullException)).SetName("TestID-006A");

                // AesManaged
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesManaged).SetName("TestID-007N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesManaged).SetName("TestID-008N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.AesManaged).SetName("TestID-009L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.AesManaged).Throws(typeof(NullReferenceException)).SetName("TestID-010A");

                // TripleDESCryptoServiceProvider
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider).SetName("TestID-011N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider).SetName("TestID-012N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider).SetName("TestID-013L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider).Throws(typeof(NullReferenceException)).SetName("TestID-014A");

                // DESCryptoServiceProvider
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider).SetName("TestID-015N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider).SetName("TestID-016N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider).SetName("TestID-017L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider).Throws(typeof(NullReferenceException)).SetName("TestID-018A");

                // RC2CryptoServiceProvider
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider).SetName("TestID-019N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider).SetName("TestID-020N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider).SetName("TestID-021L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider).Throws(typeof(NullReferenceException)).SetName("TestID-022A");

                // RijndaelManaged
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged).SetName("TestID-023N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged).SetName("TestID-024N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged).SetName("TestID-025L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged).Throws(typeof(NullReferenceException)).SetName("TestID-026A");

                // The encryption method that is not defined
                yield return new TestCaseData(abcdeBytes, "test@123", 999).Throws(typeof(ArgumentException)).SetName("TestID-027N");
                yield return new TestCaseData(aiueoBytes, "test@123", 999).Throws(typeof(ArgumentException)).SetName("TestID-028N");
                yield return new TestCaseData(emptyBytes, "test@123", 999).Throws(typeof(ArgumentException)).SetName("TestID-029L");
                yield return new TestCaseData(nullBytes, "test@123", 999).Throws(typeof(ArgumentException)).SetName("TestID-030A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method EncryptBytesTest2.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfEncryptBytesTest2
        {
            get
            {
                // If you need to prepare for the test data below so, you use the TestCaseData.

                // Convert the string to a byte array to be encrypted.
                byte[] abcdeBytes = Encoding.UTF8.GetBytes("abcde");
                byte[] aiueoBytes = Encoding.UTF8.GetBytes("あいうえお");
                byte[] emptyBytes = new Byte[0];
                byte[] nullBytes = null;

                // Define the four type salt.
                byte[] UTF8Salt = Encoding.UTF8.GetBytes("Touryo.Infrastructure.Public.IO.SymmetricCryptography.Salt");
                byte[] SJISSalt = Encoding.GetEncoding(932).GetBytes("Touryo.Infrastructure.Public.IO.SymmetricCryptography.Salt");
                byte[] EmptySalt = new byte[0];
                byte[] NullSalt = null;

                // AesCryptoServiceProvider(UTF8Salt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).SetName("TestID-000N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).SetName("TestID-002N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).SetName("TestID-003L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).Throws(typeof(NullReferenceException)).SetName("TestID-004A");
                yield return new TestCaseData(abcdeBytes, "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).SetName("TestID-005L");
                yield return new TestCaseData(abcdeBytes, null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).Throws(typeof(ArgumentNullException)).SetName("TestID-006A");

                // AesManaged(UTF8Salt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt).SetName("TestID-007N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt).SetName("TestID-008N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt).SetName("TestID-009L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt).Throws(typeof(NullReferenceException)).SetName("TestID-010A");

                // TripleDESCryptoServiceProvider(UTF8Salt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt).SetName("TestID-011N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt).SetName("TestID-012N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt).SetName("TestID-013L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt).Throws(typeof(NullReferenceException)).SetName("TestID-014A");

                // DESCryptoServiceProvider(UTF8Salt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt).SetName("TestID-015N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt).SetName("TestID-016N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt).SetName("TestID-017L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt).Throws(typeof(NullReferenceException)).SetName("TestID-018A");

                // RC2CryptoServiceProvider(UTF8Salt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt).SetName("TestID-019N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt).SetName("TestID-020N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt).SetName("TestID-021L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt).Throws(typeof(NullReferenceException)).SetName("TestID-022A");

                // RijndaelManaged(UTF8Salt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt).SetName("TestID-023N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt).SetName("TestID-024N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt).SetName("TestID-025L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt).Throws(typeof(NullReferenceException)).SetName("TestID-026A");

                // The encryption method that is not defined(UTF8Salt)
                yield return new TestCaseData(abcdeBytes, "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException)).SetName("TestID-027N");
                yield return new TestCaseData(aiueoBytes, "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException)).SetName("TestID-028N");
                yield return new TestCaseData(emptyBytes, "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException)).SetName("TestID-029L");
                yield return new TestCaseData(nullBytes, "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException)).SetName("TestID-030A");

                // AesCryptoServiceProvider(SJISSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).SetName("TestID-031N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).SetName("TestID-032N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).SetName("TestID-033L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).Throws(typeof(NullReferenceException)).SetName("TestID-034A");
                yield return new TestCaseData(abcdeBytes, "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).SetName("TestID-035L");
                yield return new TestCaseData(abcdeBytes, null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-036A");

                // AesManaged(SJISSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt).SetName("TestID-037N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt).SetName("TestID-038N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt).SetName("TestID-039L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt).Throws(typeof(NullReferenceException)).SetName("TestID-040A");

                // TripleDESCryptoServiceProvider(SJISSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt).SetName("TestID-041N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt).SetName("TestID-042N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt).SetName("TestID-043L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt).Throws(typeof(NullReferenceException)).SetName("TestID-044A");

                // DESCryptoServiceProvider(SJISSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt).SetName("TestID-045N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt).SetName("TestID-046N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt).SetName("TestID-047L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt).Throws(typeof(NullReferenceException)).SetName("TestID-048A");

                // RC2CryptoServiceProvider(SJISSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt).SetName("TestID-049N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt).SetName("TestID-050N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt).SetName("TestID-051L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt).Throws(typeof(NullReferenceException)).SetName("TestID-052A");

                // RijndaelManaged(SJISSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt).SetName("TestID-053N");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt).SetName("TestID-054N");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt).SetName("TestID-055L");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt).Throws(typeof(NullReferenceException)).SetName("TestID-056A");

                // The encryption method that is not defined(SJISSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", 999, SJISSalt).Throws(typeof(ArgumentException)).SetName("TestID-057A");
                yield return new TestCaseData(aiueoBytes, "test@123", 999, SJISSalt).Throws(typeof(ArgumentException)).SetName("TestID-058A");
                yield return new TestCaseData(emptyBytes, "test@123", 999, SJISSalt).Throws(typeof(ArgumentException)).SetName("TestID-059A");
                yield return new TestCaseData(nullBytes, "test@123", 999, SJISSalt).Throws(typeof(ArgumentException)).SetName("TestID-060A");

                // AesCryptoServiceProvider(EmptySalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-061A");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-062A");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-063A");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-064A");
                yield return new TestCaseData(abcdeBytes, "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-065A");
                yield return new TestCaseData(abcdeBytes, null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentNullException)).SetName("TestID-066A");

                // AesManaged(EmptySalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-067A");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-068A");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-069A");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-070A");

                // TripleDESCryptoServiceProvider(EmptySalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-071A");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-072A");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-073A");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-074A");

                // DESCryptoServiceProvider(EmptySalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-075A");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-076A");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-077A");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-078A");

                // RC2CryptoServiceProvider(EmptySalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-079A");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-080A");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-081A");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-082A");

                // RijndaelManaged(EmptySalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-083A");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-084A");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-085A");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-086A");

                // The encryption method that is not defined(EmptySalt) 
                yield return new TestCaseData(abcdeBytes, "test@123", 999, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-087A");
                yield return new TestCaseData(aiueoBytes, "test@123", 999, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-088A");
                yield return new TestCaseData(emptyBytes, "test@123", 999, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-089A");
                yield return new TestCaseData(nullBytes, "test@123", 999, EmptySalt).Throws(typeof(ArgumentException)).SetName("TestID-090A");

                // AesCryptoServiceProvider(NullSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-091A");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-092A");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-093A");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-094A");
                yield return new TestCaseData(abcdeBytes, "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-095A");
                yield return new TestCaseData(abcdeBytes, null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-096A");

                // AesManaged(NullSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-097A");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-098A");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-099A");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-100A");

                // TripleDESCryptoServiceProvider(NullSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-101A");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-102A");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-103A");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-104A");

                // DESCryptoServiceProvider(NullSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-105A");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-106A");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-107A");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-108A");

                // RC2CryptoServiceProvider(NullSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-109A");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-110A");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-111A");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-112A");

                // RijndaelManaged(NullSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-113A");
                yield return new TestCaseData(aiueoBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-114A");
                yield return new TestCaseData(emptyBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-115A");
                yield return new TestCaseData(nullBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException)).SetName("TestID-116A");

                // The encryption method that is not defined(NullSalt)
                yield return new TestCaseData(abcdeBytes, "test@123", 999, NullSalt).Throws(typeof(ArgumentException)).SetName("TestID-117A");
                yield return new TestCaseData(aiueoBytes, "test@123", 999, NullSalt).Throws(typeof(ArgumentException)).SetName("TestID-118A");
                yield return new TestCaseData(emptyBytes, "test@123", 999, NullSalt).Throws(typeof(ArgumentException)).SetName("TestID-119A");
                yield return new TestCaseData(nullBytes, "test@123", 999, NullSalt).Throws(typeof(ArgumentException)).SetName("TestID-120A");
            }
        }

        #endregion

        #region Test Code

        /// <summary>Test execution.(CheckListID should be the first argument)</summary>
        /// <param name="testCaseID">test case ID</param> 
        /// <param name="sourceString">String to be encrypted.</param>
        /// <param name="password">Password to be used for encryption.</param>
        /// <param name="esa">Type of cryptographic service provider of symmetric algorithm.</param>
        // AesCryptoServiceProvider
        [TestCase("abcde", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, TestName = "TestID-001N")]
        [TestCase("あいうえお", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, TestName = "TestID-002N")]
        [TestCase("", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, TestName = "TestID-003L")]
        [TestCase(null, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-004A")]
        [TestCase("abcde", "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, TestName = "TestID-005L")]
        [TestCase("abcde", null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-006A")]
        // AesManaged
        [TestCase("abcde", "test@123", EnumSymmetricAlgorithm.AesManaged, TestName = "TestID-007N")]
        [TestCase("あいうえお", "test@123", EnumSymmetricAlgorithm.AesManaged, TestName = "TestID-008N")]
        [TestCase("", "test@123", EnumSymmetricAlgorithm.AesManaged, TestName = "TestID-009L")]
        [TestCase(null, "test@123", EnumSymmetricAlgorithm.AesManaged, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-010A")]
        // TripleDESCryptoServiceProvider
        [TestCase("abcde", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, TestName = "TestID-011N")]
        [TestCase("あいうえお", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, TestName = "TestID-012N")]
        [TestCase("", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, TestName = "TestID-013L")]
        [TestCase(null, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-014A")]
        // DESCryptoServiceProvider
        [TestCase("abcde", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, TestName = "TestID-015N")]
        [TestCase("あいうえお", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, TestName = "TestID-016N")]
        [TestCase("", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, TestName = "TestID-017L")]
        [TestCase(null, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-018A")]
        // RC2CryptoServiceProvider
        [TestCase("abcde", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, TestName = "TestID-019N")]
        [TestCase("あいうえお", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, TestName = "TestID-020N")]
        [TestCase("", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, TestName = "TestID-021L")]
        [TestCase(null, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-022A")]
        // RijndaelManaged
        [TestCase("abcde", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, TestName = "TestID-023N")]
        [TestCase("あいうえお", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, TestName = "TestID-024N")]
        [TestCase("", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, TestName = "TestID-025L")]
        [TestCase(null, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-026A")]
        // The encryption method that is not defined
        [TestCase("abcde", "test@123", 999, ExpectedException = typeof(ArgumentException), TestName = "TestID-027A")]
        [TestCase("あいうえお", "test@123", 999, ExpectedException = typeof(ArgumentException), TestName = "TestID-028A")]
        [TestCase("", "test@123", 999, ExpectedException = typeof(ArgumentException), TestName = "TestID-029A")]
        [TestCase(null, "test@123", 999, ExpectedException = typeof(ArgumentNullException), TestName = "TestID-030A")]
        public void EncryptStringTest(string sourceString, string password, EnumSymmetricAlgorithm esa)
        {
            try
            {
                // Performs encryption using the components touryo.
                string encryptedString = SymmetricCryptography.EncryptString(sourceString, password, esa);

                // Performs decrypted using the components touryo.
                string decryptedString = SymmetricCryptography.DecryptString(encryptedString, password, esa);

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

        /// <summary>Test execution.(CheckListID should be the first argument)</summary>        
        /// <param name="sourceString">String to be encrypted.</param>
        /// <param name="password">Password to be used for encryption.</param>
        /// <param name="esa">Type of cryptographic service provider of symmetric algorithm.</param>
        /// <param name="salt">salt</param>
        [TestCaseSource("TestCasesOfEncryptStringTest2")]
        public void EncryptStringTest2(string sourceString, string password, EnumSymmetricAlgorithm esa, byte[] salt)
        {
            try
            {
                // Performs encryption using the components touryo.
                string encryptedString = SymmetricCryptography.EncryptString(sourceString, password, esa, salt);

                // Performs decrypted using the components touryo.
                string decryptedString = SymmetricCryptography.DecryptString(encryptedString, password, esa, salt);

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

        /// <summary>Test execution.(CheckListID should be the first argument)</summary>        
        /// <param name="sourceBytes">String to be encrypted.</param>
        /// <param name="password">Password to be used for encryption.</param>
        /// <param name="esa">Type of cryptographic service provider of symmetric algorithm.</param>
        [TestCaseSource("TestCasesOfEncryptBytesTest")]
        public void EncryptBytesTest(byte[] sourceBytes, string password, EnumSymmetricAlgorithm esa)
        {
            try
            {
                // Performs encryption using the components touryo.
                byte[] encryptedBytes = SymmetricCryptography.EncryptBytes(sourceBytes, password, esa);

                // Performs decrypted using the components touryo.
                byte[] decryptedBytes = SymmetricCryptography.DecryptBytes(encryptedBytes, password, esa);

                // Check whether it is decrypted into the original string.
                Assert.AreNotEqual(sourceBytes, encryptedBytes);
                Assert.AreEqual(sourceBytes, decryptedBytes);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>Test execution.(CheckListID should be the first argument)</summary>        
        /// <param name="sourceBytes">String to be encrypted.</param>
        /// <param name="password">Password to be used for encryption.</param>
        /// <param name="esa">Type of cryptographic service provider of symmetric algorithm.</param>
        /// <param name="salt">salt</param>
        [TestCaseSource("TestCasesOfEncryptBytesTest2")]
        public void EncryptBytesTest2(byte[] sourceBytes, string password, EnumSymmetricAlgorithm esa, byte[] salt)
        {
            try
            {
                // Performs encryption using the components of touryo.
                byte[] encryptedBytes = SymmetricCryptography.EncryptBytes(sourceBytes, password, esa, salt);

                // Performs decrypted using the components of touryo.
                byte[] decryptedBytes = SymmetricCryptography.DecryptBytes(encryptedBytes, password, esa, salt);

                // Check whether it is decrypted into the original string.
                Assert.AreNotEqual(sourceBytes, encryptedBytes);
                Assert.AreEqual(sourceBytes, decryptedBytes);
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

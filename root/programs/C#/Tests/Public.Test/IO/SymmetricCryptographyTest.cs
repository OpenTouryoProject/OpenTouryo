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
//* クラス名        ：ASymmetricCryptographyTest
//* クラス日本語名  ：対称アルゴリズムによる暗号化・復号化クラスのテスト
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

                this.SetUp();

                // Salt which has been converted into a byte array using the UTF-8 encoding.
                yield return new TestCaseData("TestID-001N", "abcde", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-002N", "あいうえお", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-003L", string.Empty, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-004A", null, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-005L", "abcde", "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-006A", "abcde", null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-007N", "abcde", "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt);
                yield return new TestCaseData("TestID-008N", "あいうえお", "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt);
                yield return new TestCaseData("TestID-009L", string.Empty, "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt);
                yield return new TestCaseData("TestID-010A", null, "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-011N", "abcde", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-012N", "あいうえお", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-013L", string.Empty, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-014A", null, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-015N", "abcde", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-016N", "あいうえお", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-017L", string.Empty, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-018A", null, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-019N", "abcde", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-020N", "あいうえお", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-021L", string.Empty, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-022A", null, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-023N", "abcde", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt);
                yield return new TestCaseData("TestID-024N", "あいうえお", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt);
                yield return new TestCaseData("TestID-025L", string.Empty, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt);
                yield return new TestCaseData("TestID-026A", null, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-027N", "abcde", "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-028N", "あいうえお", "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-029L", string.Empty, "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-030A", null, "test@123", 999, UTF8Salt).Throws(typeof(ArgumentNullException));

                // Salt which has been converted into a byte array using the Shift-JIS encoding.
                yield return new TestCaseData("TestID-031N", "abcde", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-032N", "あいうえお", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-033L", string.Empty, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-034A", null, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-035L", "abcde", "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-036A", "abcde", null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-037N", "abcde", "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt);
                yield return new TestCaseData("TestID-038N", "あいうえお", "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt);
                yield return new TestCaseData("TestID-039L", string.Empty, "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt);
                yield return new TestCaseData("TestID-040A", null, "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-041N", "abcde", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-042N", "あいうえお", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-043L", string.Empty, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-044A", null, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-045N", "abcde", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-046N", "あいうえお", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-047L", string.Empty, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-048A", null, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-049N", "abcde", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-050N", "あいうえお", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-051L", string.Empty, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-052A", null, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-053N", "abcde", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt);
                yield return new TestCaseData("TestID-054N", "あいうえお", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt);
                yield return new TestCaseData("TestID-055L", string.Empty, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt);
                yield return new TestCaseData("TestID-056A", null, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-057N", "abcde", "test@123", 999, SJISSalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-058N", "あいうえお", "test@123", 999, SJISSalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-059L", string.Empty, "test@123", 999, SJISSalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-060A", null, "test@123", 999, SJISSalt).Throws(typeof(ArgumentNullException));

                // empty byte array salt
                yield return new TestCaseData("TestID-061N", "abcde", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-062N", "あいうえお", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-063L", string.Empty, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-064A", null, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-065L", "abcde", "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-066A", "abcde", null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-067N", "abcde", "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-068N", "あいうえお", "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-069L", string.Empty, "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-070A", null, "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-071N", "abcde", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-072N", "あいうえお", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-073L", string.Empty, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-074A", null, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-075N", "abcde", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-076N", "あいうえお", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-077L", string.Empty, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-078A", null, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-079N", "abcde", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-080N", "あいうえお", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-081L", string.Empty, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-082A", null, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-083N", "abcde", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-084N", "あいうえお", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-085L", string.Empty, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-086A", null, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-087N", "abcde", "test@123", 999, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-088N", "あいうえお", "test@123", 999, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-089L", string.Empty, "test@123", 999, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-090A", null, "test@123", 999, EmptySalt).Throws(typeof(ArgumentNullException));

                // null salt.
                yield return new TestCaseData("TestID-091N", "あいうえお", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-092L", string.Empty, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-093A", null, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-094L", "abcde", "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-095A", "abcde", null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-096N", "abcde", "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-097N", "あいうえお", "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-098L", string.Empty, "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-099A", null, "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-100N", "abcde", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-101N", "あいうえお", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-102L", string.Empty, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-103A", null, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-104N", "abcde", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-105N", "あいうえお", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-106L", string.Empty, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-107A", null, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-108N", "abcde", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-109N", "あいうえお", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-110L", string.Empty, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-111A", null, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-112N", "abcde", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-113N", "あいうえお", "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-114L", string.Empty, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-115A", null, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-116N", "abcde", "test@123", 999, NullSalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-117N", "あいうえお", "test@123", 999, NullSalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-118L", string.Empty, "test@123", 999, NullSalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-119A", null, "test@123", 999, NullSalt).Throws(typeof(ArgumentNullException));
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

                this.SetUp();

                // AesCryptoServiceProvider
                yield return new TestCaseData("TestID-001N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider);
                yield return new TestCaseData("TestID-002N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider);
                yield return new TestCaseData("TestID-003L", emptyBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider);
                yield return new TestCaseData("TestID-004A", nullBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-005L", abcdeBytes, "", EnumSymmetricAlgorithm.AesCryptoServiceProvider);
                yield return new TestCaseData("TestID-006A", abcdeBytes, null, EnumSymmetricAlgorithm.AesCryptoServiceProvider).Throws(typeof(ArgumentNullException));

                // AesManaged
                yield return new TestCaseData("TestID-007N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesManaged);
                yield return new TestCaseData("TestID-008N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesManaged);
                yield return new TestCaseData("TestID-009L", emptyBytes, "test@123", EnumSymmetricAlgorithm.AesManaged);
                yield return new TestCaseData("TestID-010A", nullBytes, "test@123", EnumSymmetricAlgorithm.AesManaged).Throws(typeof(NullReferenceException));

                // TripleDESCryptoServiceProvider
                yield return new TestCaseData("TestID-011N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider);
                yield return new TestCaseData("TestID-012N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider);
                yield return new TestCaseData("TestID-013L", emptyBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider);
                yield return new TestCaseData("TestID-014A", nullBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider).Throws(typeof(NullReferenceException));

                // DESCryptoServiceProvider
                yield return new TestCaseData("TestID-015N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider);
                yield return new TestCaseData("TestID-016N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider);
                yield return new TestCaseData("TestID-017L", emptyBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider);
                yield return new TestCaseData("TestID-018A", nullBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider).Throws(typeof(NullReferenceException));

                // RC2CryptoServiceProvider
                yield return new TestCaseData("TestID-019N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider);
                yield return new TestCaseData("TestID-020N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider);
                yield return new TestCaseData("TestID-021L", emptyBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider);
                yield return new TestCaseData("TestID-022A", nullBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider).Throws(typeof(NullReferenceException));

                // RijndaelManaged
                yield return new TestCaseData("TestID-023N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged);
                yield return new TestCaseData("TestID-024N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged);
                yield return new TestCaseData("TestID-025L", emptyBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged);
                yield return new TestCaseData("TestID-026A", nullBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged).Throws(typeof(NullReferenceException));

                // The encryption method that is not defined
                yield return new TestCaseData("TestID-027N", abcdeBytes, "test@123", 999).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-028N", aiueoBytes, "test@123", 999).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-029L", emptyBytes, "test@123", 999).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-030A", nullBytes, "test@123", 999).Throws(typeof(ArgumentException));
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

                this.SetUp();

                // AesCryptoServiceProvider(UTF8Salt)
                yield return new TestCaseData("TestID-000N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-002N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-003L", emptyBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-004A", nullBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-005L", abcdeBytes, "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-006A", abcdeBytes, null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, UTF8Salt).Throws(typeof(ArgumentNullException));

                // AesManaged(UTF8Salt)
                yield return new TestCaseData("TestID-007N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt);
                yield return new TestCaseData("TestID-008N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt);
                yield return new TestCaseData("TestID-009L", emptyBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt);
                yield return new TestCaseData("TestID-010A", nullBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, UTF8Salt).Throws(typeof(NullReferenceException));

                // TripleDESCryptoServiceProvider(UTF8Salt)
                yield return new TestCaseData("TestID-011N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-012N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-013L", emptyBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-014A", nullBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, UTF8Salt).Throws(typeof(NullReferenceException));

                // DESCryptoServiceProvider(UTF8Salt)
                yield return new TestCaseData("TestID-015N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-016N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-017L", emptyBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-018A", nullBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, UTF8Salt).Throws(typeof(NullReferenceException));

                // RC2CryptoServiceProvider(UTF8Salt)
                yield return new TestCaseData("TestID-019N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-020N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-021L", emptyBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt);
                yield return new TestCaseData("TestID-022A", nullBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, UTF8Salt).Throws(typeof(NullReferenceException));

                // RijndaelManaged(UTF8Salt)
                yield return new TestCaseData("TestID-023N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt);
                yield return new TestCaseData("TestID-024N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt);
                yield return new TestCaseData("TestID-025L", emptyBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt);
                yield return new TestCaseData("TestID-026A", nullBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, UTF8Salt).Throws(typeof(NullReferenceException));

                // The encryption method that is not defined(UTF8Salt)
                yield return new TestCaseData("TestID-027N", abcdeBytes, "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-028N", aiueoBytes, "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-029L", emptyBytes, "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-030A", nullBytes, "test@123", 999, UTF8Salt).Throws(typeof(ArgumentException));

                // AesCryptoServiceProvider(SJISSalt)
                yield return new TestCaseData("TestID-031N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-032N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-033L", emptyBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-034A", nullBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-035L", abcdeBytes, "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-036A", abcdeBytes, null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, SJISSalt).Throws(typeof(ArgumentNullException));

                // AesManaged(SJISSalt)
                yield return new TestCaseData("TestID-037N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt);
                yield return new TestCaseData("TestID-038N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt);
                yield return new TestCaseData("TestID-039L", emptyBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt);
                yield return new TestCaseData("TestID-040A", nullBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, SJISSalt).Throws(typeof(NullReferenceException));

                // TripleDESCryptoServiceProvider(SJISSalt)
                yield return new TestCaseData("TestID-041N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-042N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-043L", emptyBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-044A", nullBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, SJISSalt).Throws(typeof(NullReferenceException));

                // DESCryptoServiceProvider(SJISSalt)
                yield return new TestCaseData("TestID-045N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-046N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-047L", emptyBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-048A", nullBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, SJISSalt).Throws(typeof(NullReferenceException));

                // RC2CryptoServiceProvider(SJISSalt)
                yield return new TestCaseData("TestID-049N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-050N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-051L", emptyBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt);
                yield return new TestCaseData("TestID-052A", nullBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, SJISSalt).Throws(typeof(NullReferenceException));

                // RijndaelManaged(SJISSalt)
                yield return new TestCaseData("TestID-053N", abcdeBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt);
                yield return new TestCaseData("TestID-054N", aiueoBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt);
                yield return new TestCaseData("TestID-055L", emptyBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt);
                yield return new TestCaseData("TestID-056A", nullBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, SJISSalt).Throws(typeof(NullReferenceException));

                // The encryption method that is not defined(SJISSalt)
                yield return new TestCaseData("TestID-057A", abcdeBytes, "test@123", 999, SJISSalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-058A", aiueoBytes, "test@123", 999, SJISSalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-059A", emptyBytes, "test@123", 999, SJISSalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-060A", nullBytes, "test@123", 999, SJISSalt).Throws(typeof(ArgumentException));

                // AesCryptoServiceProvider(EmptySalt)
                yield return new TestCaseData("TestID-061A", abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-062A", aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-063A", emptyBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-064A", nullBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-065A", abcdeBytes, "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-066A", abcdeBytes, null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentNullException));

                // AesManaged(EmptySalt)
                yield return new TestCaseData("TestID-067A", abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-068A", aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-069A", emptyBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-070A", nullBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, EmptySalt).Throws(typeof(ArgumentException));

                // TripleDESCryptoServiceProvider(EmptySalt)
                yield return new TestCaseData("TestID-071A", abcdeBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-072A", aiueoBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-073A", emptyBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-074A", nullBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));

                // DESCryptoServiceProvider(EmptySalt)
                yield return new TestCaseData("TestID-075A", abcdeBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-076A", aiueoBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-077A", emptyBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-078A", nullBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));

                // RC2CryptoServiceProvider(EmptySalt)
                yield return new TestCaseData("TestID-079A", abcdeBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-080A", aiueoBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-081A", emptyBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-082A", nullBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, EmptySalt).Throws(typeof(ArgumentException));

                // RijndaelManaged(EmptySalt)
                yield return new TestCaseData("TestID-083A", abcdeBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-084A", aiueoBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-085A", emptyBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-086A", nullBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, EmptySalt).Throws(typeof(ArgumentException));

                // The encryption method that is not defined(EmptySalt) 
                yield return new TestCaseData("TestID-087A", abcdeBytes, "test@123", 999, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-088A", aiueoBytes, "test@123", 999, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-089A", emptyBytes, "test@123", 999, EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-090A", nullBytes, "test@123", 999, EmptySalt).Throws(typeof(ArgumentException));

                // AesCryptoServiceProvider(NullSalt)
                yield return new TestCaseData("TestID-091A", abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-092A", aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-093A", emptyBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-094A", nullBytes, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-095A", abcdeBytes, "", EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-096A", abcdeBytes, null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));

                // AesManaged(NullSalt)
                yield return new TestCaseData("TestID-097A", abcdeBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-098A", aiueoBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-099A", emptyBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-100A", nullBytes, "test@123", EnumSymmetricAlgorithm.AesManaged, NullSalt).Throws(typeof(ArgumentNullException));

                // TripleDESCryptoServiceProvider(NullSalt)
                yield return new TestCaseData("TestID-101A", abcdeBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-102A", aiueoBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-103A", emptyBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-104A", nullBytes, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));

                // DESCryptoServiceProvider(NullSalt)
                yield return new TestCaseData("TestID-105A", abcdeBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-106A", aiueoBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-107A", emptyBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-108A", nullBytes, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));

                // RC2CryptoServiceProvider(NullSalt)
                yield return new TestCaseData("TestID-109A", abcdeBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-110A", aiueoBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-111A", emptyBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-112A", nullBytes, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, NullSalt).Throws(typeof(ArgumentNullException));

                // RijndaelManaged(NullSalt)
                yield return new TestCaseData("TestID-113A", abcdeBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-114A", aiueoBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-115A", emptyBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-116A", nullBytes, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, NullSalt).Throws(typeof(ArgumentNullException));

                // The encryption method that is not defined(NullSalt)
                yield return new TestCaseData("TestID-117A", abcdeBytes, "test@123", 999, NullSalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-118A", aiueoBytes, "test@123", 999, NullSalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-119A", emptyBytes, "test@123", 999, NullSalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-120A", nullBytes, "test@123", 999, NullSalt).Throws(typeof(ArgumentException));
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
        [TestCase("TestID-001N", "abcde", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider)]
        [TestCase("TestID-002N", "あいうえお", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider)]
        [TestCase("TestID-003L", "", "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider)]
        [TestCase("TestID-004A", null, "test@123", EnumSymmetricAlgorithm.AesCryptoServiceProvider, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-005L", "abcde", "", EnumSymmetricAlgorithm.AesCryptoServiceProvider)]
        [TestCase("TestID-006A", "abcde", null, EnumSymmetricAlgorithm.AesCryptoServiceProvider, ExpectedException = typeof(ArgumentNullException))]
        // AesManaged
        [TestCase("TestID-007N", "abcde", "test@123", EnumSymmetricAlgorithm.AesManaged)]
        [TestCase("TestID-008N", "あいうえお", "test@123", EnumSymmetricAlgorithm.AesManaged)]
        [TestCase("TestID-009L", "", "test@123", EnumSymmetricAlgorithm.AesManaged)]
        [TestCase("TestID-010A", null, "test@123", EnumSymmetricAlgorithm.AesManaged, ExpectedException = typeof(ArgumentNullException))]
        // TripleDESCryptoServiceProvider
        [TestCase("TestID-011N", "abcde", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider)]
        [TestCase("TestID-012N", "あいうえお", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider)]
        [TestCase("TestID-013L", "", "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider)]
        [TestCase("TestID-014A", null, "test@123", EnumSymmetricAlgorithm.TripleDESCryptoServiceProvider, ExpectedException = typeof(ArgumentNullException))]
        // DESCryptoServiceProvider
        [TestCase("TestID-015N", "abcde", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider)]
        [TestCase("TestID-016N", "あいうえお", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider)]
        [TestCase("TestID-017L", "", "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider)]
        [TestCase("TestID-018A", null, "test@123", EnumSymmetricAlgorithm.DESCryptoServiceProvider, ExpectedException = typeof(ArgumentNullException))]
        // RC2CryptoServiceProvider
        [TestCase("TestID-019N", "abcde", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider)]
        [TestCase("TestID-020N", "あいうえお", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider)]
        [TestCase("TestID-021L", "", "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider)]
        [TestCase("TestID-022A", null, "test@123", EnumSymmetricAlgorithm.RC2CryptoServiceProvider, ExpectedException = typeof(ArgumentNullException))]
        // RijndaelManaged
        [TestCase("TestID-023N", "abcde", "test@123", EnumSymmetricAlgorithm.RijndaelManaged)]
        [TestCase("TestID-024N", "あいうえお", "test@123", EnumSymmetricAlgorithm.RijndaelManaged)]
        [TestCase("TestID-025L", "", "test@123", EnumSymmetricAlgorithm.RijndaelManaged)]
        [TestCase("TestID-026A", null, "test@123", EnumSymmetricAlgorithm.RijndaelManaged, ExpectedException = typeof(ArgumentNullException))]
        // The encryption method that is not defined
        [TestCase("TestID-027A", "abcde", "test@123", 999, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-028A", "あいうえお", "test@123", 999, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-029A", "", "test@123", 999, ExpectedException = typeof(ArgumentException))]
        [TestCase("TestID-030A", null, "test@123", 999, ExpectedException = typeof(ArgumentNullException))]
        public void EncryptStringTest(string testCaseID, string sourceString, string password, EnumSymmetricAlgorithm esa)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>Test execution.(CheckListID should be the first argument)</summary>
        /// <param name="testCaseID">test case ID</param> 
        /// <param name="sourceString">String to be encrypted.</param>
        /// <param name="password">Password to be used for encryption.</param>
        /// <param name="esa">Type of cryptographic service provider of symmetric algorithm.</param>
        /// <param name="salt">salt</param>
        [TestCaseSource("TestCasesOfEncryptStringTest2")]
        public void EncryptStringTest2(string testCaseID, string sourceString, string password, EnumSymmetricAlgorithm esa, byte[] salt)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>Test execution.(CheckListID should be the first argument)</summary>
        /// <param name="testCaseID">test case ID</param> 
        /// <param name="sourceBytes">String to be encrypted.</param>
        /// <param name="password">Password to be used for encryption.</param>
        /// <param name="esa">Type of cryptographic service provider of symmetric algorithm.</param>
        [TestCaseSource("TestCasesOfEncryptBytesTest")]
        public void EncryptBytesTest(string testCaseID, byte[] sourceBytes, string password, EnumSymmetricAlgorithm esa)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>Test execution.(CheckListID should be the first argument)</summary>
        /// <param name="testCaseID">test case ID</param> 
        /// <param name="sourceBytes">String to be encrypted.</param>
        /// <param name="password">Password to be used for encryption.</param>
        /// <param name="esa">Type of cryptographic service provider of symmetric algorithm.</param>
        /// <param name="salt">salt</param>
        [TestCaseSource("TestCasesOfEncryptBytesTest2")]
        public void EncryptBytesTest2(string testCaseID, byte[] sourceBytes, string password, EnumSymmetricAlgorithm esa, byte[] salt)
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

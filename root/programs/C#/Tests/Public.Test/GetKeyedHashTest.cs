//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
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
//* クラス名        ：GetKeyedHashTest
//* クラス日本語名  ：ハッシュ（キー付き）を取得するクラスのテスト
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
    public class GetKeyedHashTest
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

        #region Test data

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method GetKeyedHashStringTest2.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfGetKeyedHashStringTest2
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

                // Default(UTF8Salt)
                yield return new TestCaseData("TestID-001N", "abcde", EnumKeyedHashAlgorithm.Default, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-002N", "あいうえお", EnumKeyedHashAlgorithm.Default, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-003L", string.Empty, EnumKeyedHashAlgorithm.Default, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-004A", null, EnumKeyedHashAlgorithm.Default, "test@123", UTF8Salt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-005L", "abcde", EnumKeyedHashAlgorithm.Default, "", UTF8Salt);
                yield return new TestCaseData("TestID-006A", "abcde", EnumKeyedHashAlgorithm.Default, null, UTF8Salt).Throws(typeof(ArgumentNullException));

                // HMACSHA1(UTF8Salt)
                yield return new TestCaseData("TestID-007N", "abcde", EnumKeyedHashAlgorithm.HMACSHA1, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-008N", "あいうえお", EnumKeyedHashAlgorithm.HMACSHA1, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-009L", string.Empty, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-010A", null, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", UTF8Salt).Throws(typeof(ArgumentNullException));

                // MACTripleDES(UTF8Salt)
                yield return new TestCaseData("TestID-011N", "abcde", EnumKeyedHashAlgorithm.MACTripleDES, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-012N", "あいうえお", EnumKeyedHashAlgorithm.MACTripleDES, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-013L", string.Empty, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-014A", null, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", UTF8Salt).Throws(typeof(ArgumentNullException));

                // -(UTF8Salt)
                yield return new TestCaseData("TestID-015N", "abcde", 999, "test@123", UTF8Salt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-016N", "あいうえお", 999, "test@123", UTF8Salt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-017L", string.Empty, 999, "test@123", UTF8Salt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-018A", null, 999, "test@123", UTF8Salt).Throws(typeof(ArgumentNullException));

                // Default(SJISSalt)
                yield return new TestCaseData("TestID-019N", "abcde", EnumKeyedHashAlgorithm.Default, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-020N", "あいうえお", EnumKeyedHashAlgorithm.Default, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-021L", string.Empty, EnumKeyedHashAlgorithm.Default, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-022A", null, EnumKeyedHashAlgorithm.Default, "test@123", SJISSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-023L", "abcde", EnumKeyedHashAlgorithm.Default, "", SJISSalt);
                yield return new TestCaseData("TestID-024A", "abcde", EnumKeyedHashAlgorithm.Default, null, SJISSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-025N", "abcde", EnumKeyedHashAlgorithm.HMACSHA1, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-026N", "あいうえお", EnumKeyedHashAlgorithm.HMACSHA1, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-027L", string.Empty, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-028A", null, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", SJISSalt).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-029N", "abcde", EnumKeyedHashAlgorithm.MACTripleDES, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-030N", "あいうえお", EnumKeyedHashAlgorithm.MACTripleDES, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-031L", string.Empty, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-032A", null, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", SJISSalt).Throws(typeof(ArgumentNullException));

                // The encryption method that is not defined(SJISSalt)
                yield return new TestCaseData("TestID-033A", "abcde", 999, "test@123", SJISSalt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-034A", "あいうえお", 999, "test@123", SJISSalt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-035A", string.Empty, 999, "test@123", SJISSalt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-036A", null, 999, "test@123", SJISSalt).Throws(typeof(ArgumentNullException));

                // Default(EmptySalt)
                yield return new TestCaseData("TestID-037A", "abcde", EnumKeyedHashAlgorithm.Default, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-038A", "あいうえお", EnumKeyedHashAlgorithm.Default, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-039A", string.Empty, EnumKeyedHashAlgorithm.Default, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-040A", null, EnumKeyedHashAlgorithm.Default, "test@123", EmptySalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-041A", "abcde", EnumKeyedHashAlgorithm.Default, "", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-042A", "abcde", EnumKeyedHashAlgorithm.Default, null, EmptySalt).Throws(typeof(ArgumentNullException));

                // HMACSHA1(EmptySalt)
                yield return new TestCaseData("TestID-043A", "abcde", EnumKeyedHashAlgorithm.HMACSHA1, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-044A", "あいうえお", EnumKeyedHashAlgorithm.HMACSHA1, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-045A", string.Empty, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-046A", null, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", EmptySalt).Throws(typeof(ArgumentNullException));

                // MACTripleDES(EmptySalt)
                yield return new TestCaseData("TestID-047A", "abcde", EnumKeyedHashAlgorithm.MACTripleDES, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-048A", "あいうえお", EnumKeyedHashAlgorithm.MACTripleDES, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-049A", string.Empty, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-050A", null, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", EmptySalt).Throws(typeof(ArgumentNullException));

                // The encryption method that is not defined(EmptySalt)
                yield return new TestCaseData("TestID-051A", "abcde", 999, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-052A", "あいうえお", 999, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-053A", string.Empty, 999, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-054A", null, 999, "test@123", EmptySalt).Throws(typeof(ArgumentNullException));

                // Default(NullSalt)
                yield return new TestCaseData("TestID-055A", "abcde", EnumKeyedHashAlgorithm.Default, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-056A", "あいうえお", EnumKeyedHashAlgorithm.Default, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-057A", string.Empty, EnumKeyedHashAlgorithm.Default, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-058A", null, EnumKeyedHashAlgorithm.Default, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-059A", "abcde", EnumKeyedHashAlgorithm.Default, "", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-060A", "abcde", EnumKeyedHashAlgorithm.Default, null, NullSalt).Throws(typeof(ArgumentNullException));

                // HMACSHA1(NullSalt)
                yield return new TestCaseData("TestID-061A", "abcde", EnumKeyedHashAlgorithm.HMACSHA1, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-062A", "あいうえお", EnumKeyedHashAlgorithm.HMACSHA1, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-063A", string.Empty, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-064A", null, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", NullSalt).Throws(typeof(ArgumentNullException));

                // MACTripleDES(NullSalt)
                yield return new TestCaseData("TestID-065A", "abcde", EnumKeyedHashAlgorithm.MACTripleDES, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-066A", "あいうえお", EnumKeyedHashAlgorithm.MACTripleDES, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-067A", string.Empty, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-068A", null, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", NullSalt).Throws(typeof(ArgumentNullException));

                // The encryption method that is not defined(NullSalt)
                yield return new TestCaseData("TestID-069A", "abcde", 999, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-070A", "あいうえお", 999, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-071A", string.Empty, 999, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-072A", null, 999, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method GetKeyedHashBytesTest.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfGetKeyedHashBytesTest
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

                // Default
                yield return new TestCaseData("TestID-001N", abcdeBytes, EnumKeyedHashAlgorithm.Default, "test@123");
                yield return new TestCaseData("TestID-002N", aiueoBytes, EnumKeyedHashAlgorithm.Default, "test@123");
                yield return new TestCaseData("TestID-003L", emptyBytes, EnumKeyedHashAlgorithm.Default, "test@123");
                yield return new TestCaseData("TestID-004A", nullBytes, EnumKeyedHashAlgorithm.Default, "test@123").Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-005L", abcdeBytes, EnumKeyedHashAlgorithm.Default, "");
                yield return new TestCaseData("TestID-006A", abcdeBytes, EnumKeyedHashAlgorithm.Default, null).Throws(typeof(ArgumentNullException));

                // HMACSHA1
                yield return new TestCaseData("TestID-007N", abcdeBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123");
                yield return new TestCaseData("TestID-008N", aiueoBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123");
                yield return new TestCaseData("TestID-009L", emptyBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123");
                yield return new TestCaseData("TestID-010A", nullBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123").Throws(typeof(ArgumentNullException));

                // MACTripleDES
                yield return new TestCaseData("TestID-011N", abcdeBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123");
                yield return new TestCaseData("TestID-012N", aiueoBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123");
                yield return new TestCaseData("TestID-013L", emptyBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123");
                yield return new TestCaseData("TestID-014A", nullBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123").Throws(typeof(ArgumentNullException));

                // The encryption method that is not defined 
                yield return new TestCaseData("TestID-015A", abcdeBytes, 999, "test@123").Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-016A", aiueoBytes, 999, "test@123").Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-017A", emptyBytes, 999, "test@123").Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-018A", nullBytes, 999, "test@123").Throws(typeof(NullReferenceException));
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method GetKeyedHashBytesTest2.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfGetKeyedHashBytesTest2
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

                // Default(UTF8Salt)
                yield return new TestCaseData("TestID-001N", abcdeBytes, EnumKeyedHashAlgorithm.Default, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-002N", aiueoBytes, EnumKeyedHashAlgorithm.Default, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-003L", emptyBytes, EnumKeyedHashAlgorithm.Default, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-004A", nullBytes, EnumKeyedHashAlgorithm.Default, "test@123", UTF8Salt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-005L", abcdeBytes, EnumKeyedHashAlgorithm.Default, "", UTF8Salt);
                yield return new TestCaseData("TestID-006A", abcdeBytes, EnumKeyedHashAlgorithm.Default, null, UTF8Salt).Throws(typeof(ArgumentNullException));

                // HMACSHA1(UTF8Salt)
                yield return new TestCaseData("TestID-007N", abcdeBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-008N", aiueoBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-009L", emptyBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-010A", nullBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", UTF8Salt).Throws(typeof(ArgumentNullException));

                // MACTripleDES(UTF8Salt)
                yield return new TestCaseData("TestID-011N", abcdeBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-012N", aiueoBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-013L", emptyBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", UTF8Salt);
                yield return new TestCaseData("TestID-014A", nullBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", UTF8Salt).Throws(typeof(ArgumentNullException));

                // The encryption method that is not defined(UTF8Salt)
                yield return new TestCaseData("TestID-015N", abcdeBytes, 999, "test@123", UTF8Salt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-016N", aiueoBytes, 999, "test@123", UTF8Salt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-017L", emptyBytes, 999, "test@123", UTF8Salt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-018A", nullBytes, 999, "test@123", UTF8Salt).Throws(typeof(NullReferenceException));

                // Default(SJISSalt)
                yield return new TestCaseData("TestID-019N", abcdeBytes, EnumKeyedHashAlgorithm.Default, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-020N", aiueoBytes, EnumKeyedHashAlgorithm.Default, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-021L", emptyBytes, EnumKeyedHashAlgorithm.Default, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-022A", nullBytes, EnumKeyedHashAlgorithm.Default, "test@123", SJISSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-023L", abcdeBytes, EnumKeyedHashAlgorithm.Default, "", SJISSalt);
                yield return new TestCaseData("TestID-024A", abcdeBytes, EnumKeyedHashAlgorithm.Default, null, SJISSalt).Throws(typeof(ArgumentNullException));

                // HMACSHA1(SJISSalt)
                yield return new TestCaseData("TestID-025N", abcdeBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-026N", aiueoBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-027L", emptyBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-028A", nullBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", SJISSalt).Throws(typeof(ArgumentNullException));

                // MACTripleDES(SJISSalt)
                yield return new TestCaseData("TestID-029N", abcdeBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-030N", aiueoBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-031L", emptyBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", SJISSalt);
                yield return new TestCaseData("TestID-032A", nullBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", SJISSalt).Throws(typeof(ArgumentNullException));

                // The encryption method that is not defined(SJISSalt)
                yield return new TestCaseData("TestID-033A", abcdeBytes, 999, "test@123", SJISSalt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-034A", aiueoBytes, 999, "test@123", SJISSalt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-035A", emptyBytes, 999, "test@123", SJISSalt).Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-036A", nullBytes, 999, "test@123", SJISSalt).Throws(typeof(NullReferenceException));

                // Default(EmptySalt)
                yield return new TestCaseData("TestID-037A", abcdeBytes, EnumKeyedHashAlgorithm.Default, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-038A", aiueoBytes, EnumKeyedHashAlgorithm.Default, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-039A", emptyBytes, EnumKeyedHashAlgorithm.Default, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-040A", nullBytes, EnumKeyedHashAlgorithm.Default, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-041A", abcdeBytes, EnumKeyedHashAlgorithm.Default, "", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-042A", abcdeBytes, EnumKeyedHashAlgorithm.Default, null, EmptySalt).Throws(typeof(ArgumentNullException));

                // HMACSHA1(EmptySalt)
                yield return new TestCaseData("TestID-043A", abcdeBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-044A", aiueoBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-045A", emptyBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-046A", nullBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", EmptySalt).Throws(typeof(ArgumentException));

                // MACTripleDES(EmptySalt)
                yield return new TestCaseData("TestID-047A", abcdeBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-048A", aiueoBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-049A", emptyBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-050A", nullBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", EmptySalt).Throws(typeof(ArgumentException));

                // The encryption method that is not defined(EmptySalt)
                yield return new TestCaseData("TestID-051A", abcdeBytes, 999, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-052A", aiueoBytes, 999, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-053A", emptyBytes, 999, "test@123", EmptySalt).Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-054A", nullBytes, 999, "test@123", EmptySalt).Throws(typeof(ArgumentException));

                // Default(NullSalt)
                yield return new TestCaseData("TestID-055A", abcdeBytes, EnumKeyedHashAlgorithm.Default, "test@123", NullSalt).Throws(typeof(ArgumentNullException)); ;
                yield return new TestCaseData("TestID-056A", aiueoBytes, EnumKeyedHashAlgorithm.Default, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-057A", emptyBytes, EnumKeyedHashAlgorithm.Default, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-058A", nullBytes, EnumKeyedHashAlgorithm.Default, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-059A", abcdeBytes, EnumKeyedHashAlgorithm.Default, "", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-060A", abcdeBytes, EnumKeyedHashAlgorithm.Default, null, NullSalt).Throws(typeof(ArgumentNullException));

                // HMACSHA1(NullSalt)
                yield return new TestCaseData("TestID-061A", abcdeBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-062A", aiueoBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-063A", emptyBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-064A", nullBytes, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", NullSalt).Throws(typeof(ArgumentNullException));

                // MACTripleDES(NullSalt)
                yield return new TestCaseData("TestID-065A", abcdeBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-066A", aiueoBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-067A", emptyBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-068A", nullBytes, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", NullSalt).Throws(typeof(ArgumentNullException));

                // The encryption method that is not defined(NullSalt)
                yield return new TestCaseData("TestID-069A", abcdeBytes, 999, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-070A", aiueoBytes, 999, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-071A", emptyBytes, 999, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-072A", nullBytes, 999, "test@123", NullSalt).Throws(typeof(ArgumentNullException));
            }
        }

        #endregion

        #region Test Code

        /// <summary>>Test execution.(CheckListID should be the first argument)</summary>
        /// <param name="testCaseID">test case ID</param> 
        /// <param name="sourceString">The original string to get the hash value.</param>
        /// <param name="eha">>Hash algorithm</param>
        /// <param name="password">password</param>
        [TestCase("TestID-001N", "abcde", EnumKeyedHashAlgorithm.Default, "test@123")]
        [TestCase("TestID-002N", "あいうえお", EnumKeyedHashAlgorithm.Default, "test@123")]
        [TestCase("TestID-003L", "", EnumKeyedHashAlgorithm.Default, "test@123")]
        [TestCase("TestID-004A", null, EnumKeyedHashAlgorithm.Default, "test@123", ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-005L", "abcde", EnumKeyedHashAlgorithm.Default, "")]
        [TestCase("TestID-006A", "abcde", EnumKeyedHashAlgorithm.Default, null, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-007N", "abcde", EnumKeyedHashAlgorithm.HMACSHA1, "test@123")]
        [TestCase("TestID-008N", "あいうえお", EnumKeyedHashAlgorithm.HMACSHA1, "test@123")]
        [TestCase("TestID-009L", "", EnumKeyedHashAlgorithm.HMACSHA1, "test@123")]
        [TestCase("TestID-010A", null, EnumKeyedHashAlgorithm.HMACSHA1, "test@123", ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-011N", "abcde", EnumKeyedHashAlgorithm.MACTripleDES, "test@123")]
        [TestCase("TestID-012N", "あいうえお", EnumKeyedHashAlgorithm.MACTripleDES, "test@123")]
        [TestCase("TestID-013L", "", EnumKeyedHashAlgorithm.MACTripleDES, "test@123")]
        [TestCase("TestID-014A", null, EnumKeyedHashAlgorithm.MACTripleDES, "test@123", ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-015A", "abcde", 999, "test@123", ExpectedException = typeof(NullReferenceException))]        // The encryption method that is not defined
        [TestCase("TestID-016A", "あいうえお", 999, "test@123", ExpectedException = typeof(NullReferenceException))]   // The encryption method that is not defined
        [TestCase("TestID-017A", "", 999, "test@123", ExpectedException = typeof(NullReferenceException))]             // The encryption method that is not defined
        [TestCase("TestID-018A", null, 999, "test@123", ExpectedException = typeof(ArgumentNullException))]            // The encryption method that is not defined
        public void GetKeyedHashStringTest(string testCaseID, string sourceString, EnumKeyedHashAlgorithm eha, string password)
        {
            try
            {
                // Get the hash value using the components of touryo. 
                string hashString = GetKeyedHash.GetKeyedHashString(sourceString, eha, password);

                // Using the components of touryo, and get the hash value again.
                string hashString2 = GetKeyedHash.GetKeyedHashString(sourceString, eha, password);

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
        /// <param name="sourceString">The original string to get the hash value.</param>
        /// <param name="eha">Hash algorithm</param>
        /// <param name="password">password</param>
        /// <param name="salt">salt</param>
        [TestCaseSource("TestCasesOfGetKeyedHashStringTest2")]
        public void GetKeyedHashStringTest2(string testCaseID, string sourceString, EnumKeyedHashAlgorithm eha, string password, byte[] salt)
        {
            try
            {
                // Get the Salt password using the components of touryo. 
                string hashString = GetKeyedHash.GetKeyedHashString(sourceString, eha, password, salt);

                // Using the components of touryo, and get the hash value again.
                string hashString2 = GetKeyedHash.GetKeyedHashString(sourceString, eha, password, salt);

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
        /// <param name="asb">The original string to get the hash value.</param>
        /// <param name="eha">Hash algorithm</param>
        /// <param name="password">password</param>
        [TestCaseSource("TestCasesOfGetKeyedHashBytesTest")]
        public void GetKeyedHashBytesTest(string testCaseID, byte[] asb, EnumKeyedHashAlgorithm eha, string password)
        {
            try
            {
                // anyWarp 棟梁の部品を使用してハッシュ値を取得
                byte[] hashBytes = GetKeyedHash.GetKeyedHashBytes(asb, eha, password);

                // anyWarp 棟梁の部品を使用して、もう一度ハッシュ値を取得
                byte[] hashBytes2 = GetKeyedHash.GetKeyedHashBytes(asb, eha, password);

                // ハッシュ値が同じかどうかをチェック
                Assert.AreNotEqual(asb, hashBytes);
                Assert.AreNotEqual(asb, hashBytes2);
                Assert.AreEqual(hashBytes, hashBytes2);
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
        /// <param name="asb">The original string to get the hash value.</param>
        /// <param name="eha">Hash algorithm</param>
        /// <param name="password">password</param>
        /// <param name="salt">salt</param>
        [TestCaseSource("TestCasesOfGetKeyedHashBytesTest2")]
        public void GetKeyedHashBytesTest2(string testCaseID, byte[] asb, EnumKeyedHashAlgorithm eha, string password, byte[] salt)
        {
            try
            {
                // anyWarp 棟梁の部品を使用してハッシュ値を取得
                byte[] hashBytes = GetKeyedHash.GetKeyedHashBytes(asb, eha, password, salt);

                // anyWarp 棟梁の部品を使用して、もう一度ハッシュ値を取得
                byte[] hashBytes2 = GetKeyedHash.GetKeyedHashBytes(asb, eha, password, salt);

                // ハッシュ値が同じかどうかをチェック
                Assert.AreNotEqual(asb, hashBytes);
                Assert.AreNotEqual(asb, hashBytes2);
                Assert.AreEqual(hashBytes, hashBytes2);
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

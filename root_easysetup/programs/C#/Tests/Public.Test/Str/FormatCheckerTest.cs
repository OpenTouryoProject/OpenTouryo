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
//* クラス名        ：FormatCheckerTest
//* クラス日本語名  ：Test of the class to Check the Format
//*
//* 作成者          ：Rituparna
//* 更新履歴        ：
//* 
//*  Date:        Author:          Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/12/2014    Rituparna       Testcode development for FormatChecker.
//*  08/12/2014    Rituparna             Added TestcaseID using SetName method as per Nishino-San comments
//**********************************************************************************

#region Includes

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.Str;

#endregion

namespace Public.Test.Str
{
    public class FormatCheckerTest
    {
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
        /// This method to generate test data to be passed to the method IsJpZipCode.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpZipCodeTest
        {
            get
            {
                yield return new TestCaseData("000-0000").SetName("TestID-000N");
                yield return new TestCaseData("aaa-aaaa").SetName("TestID-001N");
                yield return new TestCaseData(string.Empty).SetName("TestID-002L");
                yield return new TestCaseData("0000-0000").SetName("TestID-003N");
                yield return new TestCaseData("000-00000").SetName("TestID-004N");
                yield return new TestCaseData("00-000").SetName("TestID-005N");
                yield return new TestCaseData("00-0000").SetName("TestID-006N");
                yield return new TestCaseData("000-000").SetName("TestID-007N");
                yield return new TestCaseData("000-00").SetName("TestID-008N");
                yield return new TestCaseData("aaa-aa").SetName("TestID-009N");
                yield return new TestCaseData("0000-000").SetName("TestID-010N");
                yield return new TestCaseData("0000-00").SetName("TestID-011N");
                yield return new TestCaseData("000-000").SetName("TestID-012N");
                yield return new TestCaseData("00-0").SetName("TestID-013N");
                yield return new TestCaseData("00-00").SetName("TestID-014N");
                yield return new TestCaseData("000-0").SetName("TestID-015N");
                yield return new TestCaseData("0000000").SetName("TestID-016N");
                yield return new TestCaseData("aaaaaaa").SetName("TestID-017N");
                yield return new TestCaseData("00000000").SetName("TestID-018N");
                yield return new TestCaseData("000000").SetName("TestID-019N");
                yield return new TestCaseData("00000").SetName("TestID-020N");
                yield return new TestCaseData("aaaaa").SetName("TestID-021N");
                yield return new TestCaseData("000000").SetName("TestID-022N");
                yield return new TestCaseData("0000").SetName("TestID-023N");
                yield return new TestCaseData("000").SetName("TestID-024N");
                yield return new TestCaseData("aaa").SetName("TestID-025N");
                yield return new TestCaseData("0000").SetName("TestID-026N");
                yield return new TestCaseData("00").SetName("TestID-027N");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-028A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpZipCodeHyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpZipCodeHyphenTest
        {
            get
            {
                yield return new TestCaseData("000-0000").SetName("TestID-000N");
                yield return new TestCaseData("aaa-aaaa").SetName("TestID-001N");
                yield return new TestCaseData(string.Empty).SetName("TestID-002L");
                yield return new TestCaseData("0000-0000").SetName("TestID-003N");
                yield return new TestCaseData("000-00000").SetName("TestID-004N");
                yield return new TestCaseData("00-000").SetName("TestID-005N");
                yield return new TestCaseData("00-0000").SetName("TestID-006N");
                yield return new TestCaseData("000-000").SetName("TestID-007N");
                yield return new TestCaseData("000-00").SetName("TestID-008N");
                yield return new TestCaseData("aaa-aa").SetName("TestID-009N");
                yield return new TestCaseData("0000-000").SetName("TestID-010N");
                yield return new TestCaseData("0000-00").SetName("TestID-011N");
                yield return new TestCaseData("000-000").SetName("TestID-012N");
                yield return new TestCaseData("00-0").SetName("TestID-013N");
                yield return new TestCaseData("00-00").SetName("TestID-014N");
                yield return new TestCaseData("000-0").SetName("TestID-015N");
                yield return new TestCaseData("0000000").SetName("TestID-016N");
                yield return new TestCaseData("aaaaaaa").SetName("TestID-017N");
                yield return new TestCaseData("00000000").SetName("TestID-018N");
                yield return new TestCaseData("000000").SetName("TestID-019N");
                yield return new TestCaseData("00000").SetName("TestID-020N");
                yield return new TestCaseData("aaaaa").SetName("TestID-021N");
                yield return new TestCaseData("000000").SetName("TestID-022N");
                yield return new TestCaseData("0000").SetName("TestID-023N");
                yield return new TestCaseData("000").SetName("TestID-024N");
                yield return new TestCaseData("aaa").SetName("TestID-025N");
                yield return new TestCaseData("0000").SetName("TestID-026N");
                yield return new TestCaseData("00").SetName("TestID-027N");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-028A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpZipCodeNoHyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpZipCodeNoHyphenTest
        {
            get
            {
                yield return new TestCaseData("000-0000").SetName("TestID-000N");
                yield return new TestCaseData("aaa-aaaa").SetName("TestID-001N");
                yield return new TestCaseData(string.Empty).SetName("TestID-002L");
                yield return new TestCaseData("0000-0000").SetName("TestID-003N");
                yield return new TestCaseData("000-00000").SetName("TestID-004N");
                yield return new TestCaseData("00-000").SetName("TestID-005N");
                yield return new TestCaseData("00-0000").SetName("TestID-006N");
                yield return new TestCaseData("000-000").SetName("TestID-007N");
                yield return new TestCaseData("000-00").SetName("TestID-008N");
                yield return new TestCaseData("aaa-aa").SetName("TestID-009N");
                yield return new TestCaseData("0000-000").SetName("TestID-010N");
                yield return new TestCaseData("0000-00").SetName("TestID-011N");
                yield return new TestCaseData("000-000").SetName("TestID-012N");
                yield return new TestCaseData("00-0").SetName("TestID-013N");
                yield return new TestCaseData("00-00").SetName("TestID-014N");
                yield return new TestCaseData("000-0").SetName("TestID-015N");
                yield return new TestCaseData("0000000").SetName("TestID-016N");
                yield return new TestCaseData("aaaaaaa").SetName("TestID-017N");
                yield return new TestCaseData("00000000").SetName("TestID-018N");
                yield return new TestCaseData("000000").SetName("TestID-019N");
                yield return new TestCaseData("00000").SetName("TestID-020N");
                yield return new TestCaseData("aaaaa").SetName("TestID-021N");
                yield return new TestCaseData("000000").SetName("TestID-022N");
                yield return new TestCaseData("0000").SetName("TestID-023N");
                yield return new TestCaseData("000").SetName("TestID-024N");
                yield return new TestCaseData("aaa").SetName("TestID-025N");
                yield return new TestCaseData("0000").SetName("TestID-026N");
                yield return new TestCaseData("00").SetName("TestID-027N");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-028A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpZipCode7.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpZipCode7Test
        {
            get
            {
                yield return new TestCaseData("000-0000").SetName("TestID-000N");
                yield return new TestCaseData("aaa-aaaa").SetName("TestID-001N");
                yield return new TestCaseData(string.Empty).SetName("TestID-002N");
                yield return new TestCaseData("0000-0000").SetName("TestID-003N");
                yield return new TestCaseData("000-00000").SetName("TestID-004N");
                yield return new TestCaseData("00-000").SetName("TestID-005N");
                yield return new TestCaseData("00-0000").SetName("TestID-006N");
                yield return new TestCaseData("000-000").SetName("TestID-007N");
                yield return new TestCaseData("000-00").SetName("TestID-008N");
                yield return new TestCaseData("aaa-aa").SetName("TestID-009N");
                yield return new TestCaseData("0000-000").SetName("TestID-010N");
                yield return new TestCaseData("0000-00").SetName("TestID-011N");
                yield return new TestCaseData("000-000").SetName("TestID-012N");
                yield return new TestCaseData("00-0").SetName("TestID-013N");
                yield return new TestCaseData("00-00").SetName("TestID-014N");
                yield return new TestCaseData("000-0").SetName("TestID-015N");
                yield return new TestCaseData("0000000").SetName("TestID-016N");
                yield return new TestCaseData("aaaaaaa").SetName("TestID-017N");
                yield return new TestCaseData("00000000").SetName("TestID-018N");
                yield return new TestCaseData("000000").SetName("TestID-019N");
                yield return new TestCaseData("00000").SetName("TestID-020N");
                yield return new TestCaseData("aaaaa").SetName("TestID-021N");
                yield return new TestCaseData("000000").SetName("TestID-022N");
                yield return new TestCaseData("0000").SetName("TestID-023N");
                yield return new TestCaseData("000").SetName("TestID-024N");
                yield return new TestCaseData("aaa").SetName("TestID-025N");
                yield return new TestCaseData("0000").SetName("TestID-026N");
                yield return new TestCaseData("00").SetName("TestID-027N");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-028A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpZipCode7Hyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpZipCode7HyphenTest
        {
            get
            {
                yield return new TestCaseData("000-0000").SetName("TestID-000N");
                yield return new TestCaseData("aaa-aaaa").SetName("TestID-001N");
                yield return new TestCaseData(string.Empty).SetName("TestID-002L");
                yield return new TestCaseData("0000-0000").SetName("TestID-003N");
                yield return new TestCaseData("000-00000").SetName("TestID-004N");
                yield return new TestCaseData("00-000").SetName("TestID-005N");
                yield return new TestCaseData("00-0000").SetName("TestID-006N");
                yield return new TestCaseData("000-000").SetName("TestID-007N");
                yield return new TestCaseData("000-00").SetName("TestID-008N");
                yield return new TestCaseData("aaa-aa").SetName("TestID-009N");
                yield return new TestCaseData("0000-000").SetName("TestID-010N");
                yield return new TestCaseData("0000-00").SetName("TestID-011N");
                yield return new TestCaseData("000-000").SetName("TestID-012N");
                yield return new TestCaseData("00-0").SetName("TestID-013N");
                yield return new TestCaseData("00-00").SetName("TestID-014N");
                yield return new TestCaseData("000-0").SetName("TestID-015N");
                yield return new TestCaseData("0000000").SetName("TestID-016N");
                yield return new TestCaseData("aaaaaaa").SetName("TestID-017N");
                yield return new TestCaseData("00000000").SetName("TestID-018N");
                yield return new TestCaseData("000000").SetName("TestID-019N");
                yield return new TestCaseData("00000").SetName("TestID-020N");
                yield return new TestCaseData("aaaaa").SetName("TestID-021N");
                yield return new TestCaseData("000000").SetName("TestID-022N");
                yield return new TestCaseData("0000").SetName("TestID-023N");
                yield return new TestCaseData("000").SetName("TestID-024N");
                yield return new TestCaseData("aaa").SetName("TestID-025N");
                yield return new TestCaseData("0000").SetName("TestID-026N");
                yield return new TestCaseData("00").SetName("TestID-027N");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-028A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpZipCode7NoHyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpZipCode7NoHyphenTest
        {
            get
            {
                yield return new TestCaseData("000-0000").SetName("TestID-000N");
                yield return new TestCaseData("aaa-aaaa").SetName("TestID-001N");
                yield return new TestCaseData(string.Empty).SetName("TestID-002L");
                yield return new TestCaseData("0000-0000").SetName("TestID-003N");
                yield return new TestCaseData("000-00000").SetName("TestID-004N");
                yield return new TestCaseData("00-000").SetName("TestID-005N");
                yield return new TestCaseData("00-0000").SetName("TestID-006N");
                yield return new TestCaseData("000-000").SetName("TestID-007N");
                yield return new TestCaseData("000-00").SetName("TestID-008N");
                yield return new TestCaseData("aaa-aa").SetName("TestID-009N");
                yield return new TestCaseData("0000-000").SetName("TestID-010N");
                yield return new TestCaseData("0000-00").SetName("TestID-011N");
                yield return new TestCaseData("000-000").SetName("TestID-012N");
                yield return new TestCaseData("00-0").SetName("TestID-013N");
                yield return new TestCaseData("00-00").SetName("TestID-014N");
                yield return new TestCaseData("000-0").SetName("TestID-015N");
                yield return new TestCaseData("0000000").SetName("TestID-016N");
                yield return new TestCaseData("aaaaaaa").SetName("TestID-017N");
                yield return new TestCaseData("00000000").SetName("TestID-018N");
                yield return new TestCaseData("000000").SetName("TestID-019N");
                yield return new TestCaseData("00000").SetName("TestID-020N");
                yield return new TestCaseData("aaaaa").SetName("TestID-021N");
                yield return new TestCaseData("000000").SetName("TestID-022N");
                yield return new TestCaseData("0000").SetName("TestID-023N");
                yield return new TestCaseData("000").SetName("TestID-024N");
                yield return new TestCaseData("aaa").SetName("TestID-025N");
                yield return new TestCaseData("0000").SetName("TestID-026N");
                yield return new TestCaseData("00").SetName("TestID-027N");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-028A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpZipCode5.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpZipCode5Test
        {
            get
            {
              
                yield return new TestCaseData("000-0000").SetName("TestID-000N");
                yield return new TestCaseData("aaa-aaaa").SetName("TestID-001N");
                yield return new TestCaseData(string.Empty).SetName("TestID-002L");
                yield return new TestCaseData("0000-0000").SetName("TestID-003N");
                yield return new TestCaseData("000-00000").SetName("TestID-004N");
                yield return new TestCaseData("00-000").SetName("TestID-005N");
                yield return new TestCaseData("00-0000").SetName("TestID-006N");
                yield return new TestCaseData("000-000").SetName("TestID-007N");
                yield return new TestCaseData("000-00").SetName("TestID-008N");
                yield return new TestCaseData("aaa-aa").SetName("TestID-009N");
                yield return new TestCaseData("0000-000").SetName("TestID-010N");
                yield return new TestCaseData("0000-00").SetName("TestID-011N");
                yield return new TestCaseData("000-000").SetName("TestID-012N");
                yield return new TestCaseData("00-0").SetName("TestID-013N");
                yield return new TestCaseData("00-00").SetName("TestID-014N");
                yield return new TestCaseData("000-0").SetName("TestID-015N");
                yield return new TestCaseData("0000000").SetName("TestID-016N");
                yield return new TestCaseData("aaaaaaa").SetName("TestID-017N");
                yield return new TestCaseData("00000000").SetName("TestID-018N");
                yield return new TestCaseData("000000").SetName("TestID-019N");
                yield return new TestCaseData("00000").SetName("TestID-020N");
                yield return new TestCaseData("aaaaa").SetName("TestID-021N");
                yield return new TestCaseData("000000").SetName("TestID-022N");
                yield return new TestCaseData("0000").SetName("TestID-023N");
                yield return new TestCaseData("000").SetName("TestID-024N");
                yield return new TestCaseData("aaa").SetName("TestID-025N");
                yield return new TestCaseData("0000").SetName("TestID-026N");
                yield return new TestCaseData("00").SetName("TestID-027N");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-028A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpZipCode5Hyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpZipCode5HyphenTest
        {
            get
            {
                yield return new TestCaseData("000-0000").SetName("TestID-000N");
                yield return new TestCaseData("aaa-aaaa").SetName("TestID-001N");
                yield return new TestCaseData(string.Empty).SetName("TestID-002L");
                yield return new TestCaseData("0000-0000").SetName("TestID-003N");
                yield return new TestCaseData("000-00000").SetName("TestID-004N");
                yield return new TestCaseData("00-000").SetName("TestID-005N");
                yield return new TestCaseData("00-0000").SetName("TestID-006N");
                yield return new TestCaseData("000-000").SetName("TestID-007N");
                yield return new TestCaseData("000-00").SetName("TestID-008N");
                yield return new TestCaseData("aaa-aa").SetName("TestID-009N");
                yield return new TestCaseData("0000-000").SetName("TestID-010N");
                yield return new TestCaseData("0000-00").SetName("TestID-011N");
                yield return new TestCaseData("000-000").SetName("TestID-012N");
                yield return new TestCaseData("00-0").SetName("TestID-013N");
                yield return new TestCaseData("00-00").SetName("TestID-014N");
                yield return new TestCaseData("000-0").SetName("TestID-015N");
                yield return new TestCaseData("0000000").SetName("TestID-016N");
                yield return new TestCaseData("aaaaaaa").SetName("TestID-017N");
                yield return new TestCaseData("00000000").SetName("TestID-018N");
                yield return new TestCaseData("000000").SetName("TestID-019N");
                yield return new TestCaseData("00000").SetName("TestID-020N");
                yield return new TestCaseData("aaaaa").SetName("TestID-021N");
                yield return new TestCaseData("000000").SetName("TestID-022N");
                yield return new TestCaseData("0000").SetName("TestID-023N");
                yield return new TestCaseData("000").SetName("TestID-024N");
                yield return new TestCaseData("aaa").SetName("TestID-025N");
                yield return new TestCaseData("0000").SetName("TestID-026N");
                yield return new TestCaseData("00").SetName("TestID-027N");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-028A");
            }
        }


        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpZipCode5NoHyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpZipCode5NoHyphenTest
        {
            get
            {
                yield return new TestCaseData("000-0000").SetName("TestID-000N");
                yield return new TestCaseData("aaa-aaaa").SetName("TestID-001N");
                yield return new TestCaseData(string.Empty).SetName("TestID-002L");
                yield return new TestCaseData("0000-0000").SetName("TestID-003N");
                yield return new TestCaseData("000-00000").SetName("TestID-004N");
                yield return new TestCaseData("00-000").SetName("TestID-005N");
                yield return new TestCaseData("00-0000").SetName("TestID-006N");
                yield return new TestCaseData("000-000").SetName("TestID-007N");
                yield return new TestCaseData("000-00").SetName("TestID-008N");
                yield return new TestCaseData("aaa-aa").SetName("TestID-009N");
                yield return new TestCaseData("0000-000").SetName("TestID-010N");
                yield return new TestCaseData("0000-00").SetName("TestID-011N");
                yield return new TestCaseData("000-000").SetName("TestID-012N");
                yield return new TestCaseData("00-0").SetName("TestID-013N");
                yield return new TestCaseData("00-00").SetName("TestID-014N");
                yield return new TestCaseData("000-0").SetName("TestID-015N");
                yield return new TestCaseData("0000000").SetName("TestID-016N");
                yield return new TestCaseData("aaaaaaa").SetName("TestID-017N");
                yield return new TestCaseData("00000000").SetName("TestID-018N");
                yield return new TestCaseData("000000").SetName("TestID-019N");
                yield return new TestCaseData("00000").SetName("TestID-020N");
                yield return new TestCaseData("aaaaa").SetName("TestID-021N");
                yield return new TestCaseData("000000").SetName("TestID-022N");
                yield return new TestCaseData("0000").SetName("TestID-023N");
                yield return new TestCaseData("000").SetName("TestID-024N");
                yield return new TestCaseData("aaa").SetName("TestID-025N");
                yield return new TestCaseData("0000").SetName("TestID-026N");
                yield return new TestCaseData("00").SetName("TestID-027N");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-028A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpTelephoneNumber.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpTelephoneNumberTest
        {
            get
            {
                yield return new TestCaseData("09999-9-9999").SetName("TestID-000N");
                yield return new TestCaseData("99999-9-9999").SetName("TestID-001N");
                yield return new TestCaseData("0aaaa-a-aaaa").SetName("TestID-002N");
                yield return new TestCaseData("099999-9-9999").SetName("TestID-003N");
                yield return new TestCaseData("09999-99-9999").SetName("TestID-004N");
                yield return new TestCaseData("09999-9-99999").SetName("TestID-005N");
                yield return new TestCaseData("0999--999").SetName("TestID-006N");
                yield return new TestCaseData("0999-9-9999").SetName("TestID-007N");
                yield return new TestCaseData("09999--9999").SetName("TestID-008N");
                yield return new TestCaseData("09999-9-999").SetName("TestID-009N");
                yield return new TestCaseData("0999-99-9999").SetName("TestID-010N");
                yield return new TestCaseData("9999-99-9999").SetName("TestID-011N");
                yield return new TestCaseData("0aaa-aa-aaaa").SetName("TestID-012N");
                yield return new TestCaseData("09999-999-99999").SetName("TestID-013N");
                yield return new TestCaseData("09999-99-9999").SetName("TestID-014N");
                yield return new TestCaseData("0999-999-9999").SetName("TestID-015N");
                yield return new TestCaseData("0999-99-99999").SetName("TestID-016N");
                yield return new TestCaseData("099-9-999").SetName("TestID-017N");
                yield return new TestCaseData("099-99-9999").SetName("TestID-018N");
                yield return new TestCaseData("0999-9-9999").SetName("TestID-019N");
                yield return new TestCaseData("0999-99-999").SetName("TestID-020N");
                yield return new TestCaseData("099-999-9999").SetName("TestID-021N");
                yield return new TestCaseData("999-999-9999").SetName("TestID-022N");
                yield return new TestCaseData("0aa-aaa-aaaa").SetName("TestID-023N");
                yield return new TestCaseData("0999-9999-99999").SetName("TestID-024N");
                yield return new TestCaseData("0999-999-9999").SetName("TestID-025N");
                yield return new TestCaseData("099-9999-9999").SetName("TestID-026N");
                yield return new TestCaseData("099-999-99999").SetName("TestID-027N");
                yield return new TestCaseData("09-99-999").SetName("TestID-028N");
                yield return new TestCaseData("09-999-9999").SetName("TestID-029N");
                yield return new TestCaseData("099-99-9999").SetName("TestID-030N");
                yield return new TestCaseData("099-999-999").SetName("TestID-031N");
                yield return new TestCaseData("09-9999-9999").SetName("TestID-031N");
                yield return new TestCaseData("99-9999-9999").SetName("TestID-033N");
                yield return new TestCaseData("0a-aaaa-aaaa").SetName("TestID-034N");
                yield return new TestCaseData("099-99999-99999").SetName("TestID-035N");
                yield return new TestCaseData("099-9999-9999").SetName("TestID-036N");
                yield return new TestCaseData("09-99999-9999").SetName("TestID-037N");
                yield return new TestCaseData("09-9999-99999").SetName("TestID-038N");
                yield return new TestCaseData("0-999-999").SetName("TestID-039N");
                yield return new TestCaseData("0-9999-9999").SetName("TestID-040N");
                yield return new TestCaseData("09-999-9999").SetName("TestID-041N");
                yield return new TestCaseData("09-9999-999").SetName("TestID-042N");
                yield return new TestCaseData("999999999999").SetName("TestID-043N");
                yield return new TestCaseData("0999999999").SetName("TestID-044N");
                yield return new TestCaseData("9999999999").SetName("TestID-045N");
                yield return new TestCaseData("0aaaaaaaaa").SetName("TestID-046N");
                yield return new TestCaseData("09999999999").SetName("TestID-047N");
                yield return new TestCaseData("099999999").SetName("TestID-048N");
                yield return new TestCaseData("020-9999-9999").SetName("TestID-049N");
                yield return new TestCaseData("929-9999-9999").SetName("TestID-050N");
                yield return new TestCaseData("020-aaaa-aaaa").SetName("TestID-051N");
                yield return new TestCaseData("0209-99999-99999").SetName("TestID-052N");
                yield return new TestCaseData(string.Empty).SetName("TestID-053L");
                yield return new TestCaseData(null).Throws(typeof(NullReferenceException)).SetName("TestID-054A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpTelephoneNumberHyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpTelephoneNumberHyphenTest
        {
            get
            {
                yield return new TestCaseData("09999-9-9999").SetName("TestID-000N");
                yield return new TestCaseData("99999-9-9999").SetName("TestID-001N");
                yield return new TestCaseData("0aaaa-a-aaaa").SetName("TestID-002N");
                yield return new TestCaseData("099999-9-9999").SetName("TestID-003N");
                yield return new TestCaseData("09999-99-9999").SetName("TestID-004N");
                yield return new TestCaseData("09999-9-99999").SetName("TestID-005N");
                yield return new TestCaseData("0999--999").SetName("TestID-006N");
                yield return new TestCaseData("0999-9-9999").SetName("TestID-007N");
                yield return new TestCaseData("09999--9999").SetName("TestID-008N");
                yield return new TestCaseData("09999-9-999").SetName("TestID-009N");
                yield return new TestCaseData("0999-99-9999").SetName("TestID-010N");
                yield return new TestCaseData("9999-99-9999").SetName("TestID-011N");
                yield return new TestCaseData("0aaa-aa-aaaa").SetName("TestID-012N");
                yield return new TestCaseData("09999-999-99999").SetName("TestID-013N");
                yield return new TestCaseData("09999-99-9999").SetName("TestID-014N");
                yield return new TestCaseData("0999-999-9999").SetName("TestID-015N");
                yield return new TestCaseData("0999-99-99999").SetName("TestID-016N");
                yield return new TestCaseData("099-9-999").SetName("TestID-017N");
                yield return new TestCaseData("099-99-9999").SetName("TestID-018N");
                yield return new TestCaseData("0999-9-9999").SetName("TestID-019N");
                yield return new TestCaseData("0999-99-999").SetName("TestID-020N");
                yield return new TestCaseData("099-999-9999").SetName("TestID-021N");
                yield return new TestCaseData("999-999-9999").SetName("TestID-022N");
                yield return new TestCaseData("0aa-aaa-aaaa").SetName("TestID-023N");
                yield return new TestCaseData("0999-9999-99999").SetName("TestID-024N");
                yield return new TestCaseData("0999-999-9999").SetName("TestID-025N");
                yield return new TestCaseData("099-9999-9999").SetName("TestID-026N");
                yield return new TestCaseData("099-999-99999").SetName("TestID-027N");
                yield return new TestCaseData("09-99-999").SetName("TestID-028N");
                yield return new TestCaseData("09-999-9999").SetName("TestID-029N");
                yield return new TestCaseData("099-99-9999").SetName("TestID-030N");
                yield return new TestCaseData("099-999-999").SetName("TestID-031N");
                yield return new TestCaseData("09-9999-9999").SetName("TestID-031N");
                yield return new TestCaseData("99-9999-9999").SetName("TestID-033N");
                yield return new TestCaseData("0a-aaaa-aaaa").SetName("TestID-034N");
                yield return new TestCaseData("099-99999-99999").SetName("TestID-035N");
                yield return new TestCaseData("099-9999-9999").SetName("TestID-036N");
                yield return new TestCaseData("09-99999-9999").SetName("TestID-037N");
                yield return new TestCaseData("09-9999-99999").SetName("TestID-038N");
                yield return new TestCaseData("0-999-999").SetName("TestID-039N");
                yield return new TestCaseData("0-9999-9999").SetName("TestID-040N");
                yield return new TestCaseData("09-999-9999").SetName("TestID-041N");
                yield return new TestCaseData("09-9999-999").SetName("TestID-042N");
                yield return new TestCaseData("999999999999").SetName("TestID-043N");
                yield return new TestCaseData("0999999999").SetName("TestID-044N");
                yield return new TestCaseData("9999999999").SetName("TestID-045N");
                yield return new TestCaseData("0aaaaaaaaa").SetName("TestID-046N");
                yield return new TestCaseData("09999999999").SetName("TestID-047N");
                yield return new TestCaseData("099999999").SetName("TestID-048N");
                yield return new TestCaseData("020-9999-9999").SetName("TestID-049N");
                yield return new TestCaseData("929-9999-9999").SetName("TestID-050N");
                yield return new TestCaseData("020-aaaa-aaaa").SetName("TestID-051N");
                yield return new TestCaseData("0209-99999-99999").SetName("TestID-052N");
                yield return new TestCaseData(string.Empty).SetName("TestID-053L");
                yield return new TestCaseData(null).Throws(typeof(NullReferenceException)).SetName("TestID-054A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpTelephoneNumberNoHyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpTelephoneNumberNoHyphenTest
        {
            get
            {
                yield return new TestCaseData("09999-9-9999").SetName("TestID-000N");
                yield return new TestCaseData("99999-9-9999").SetName("TestID-001N");
                yield return new TestCaseData("0aaaa-a-aaaa").SetName("TestID-002N");
                yield return new TestCaseData("099999-9-9999").SetName("TestID-003N");
                yield return new TestCaseData("09999-99-9999").SetName("TestID-004N");
                yield return new TestCaseData("09999-9-99999").SetName("TestID-005N");
                yield return new TestCaseData("0999--999").SetName("TestID-006N");
                yield return new TestCaseData("0999-9-9999").SetName("TestID-007N");
                yield return new TestCaseData("09999--9999").SetName("TestID-008N");
                yield return new TestCaseData("09999-9-999").SetName("TestID-009N");
                yield return new TestCaseData("0999-99-9999").SetName("TestID-010N");
                yield return new TestCaseData("9999-99-9999").SetName("TestID-011N");
                yield return new TestCaseData("0aaa-aa-aaaa").SetName("TestID-012N");
                yield return new TestCaseData("09999-999-99999").SetName("TestID-013N");
                yield return new TestCaseData("09999-99-9999").SetName("TestID-014N");
                yield return new TestCaseData("0999-999-9999").SetName("TestID-015N");
                yield return new TestCaseData("0999-99-99999").SetName("TestID-016N");
                yield return new TestCaseData("099-9-999").SetName("TestID-017N");
                yield return new TestCaseData("099-99-9999").SetName("TestID-018N");
                yield return new TestCaseData("0999-9-9999").SetName("TestID-019N");
                yield return new TestCaseData("0999-99-999").SetName("TestID-020N");
                yield return new TestCaseData("099-999-9999").SetName("TestID-021N");
                yield return new TestCaseData("999-999-9999").SetName("TestID-022N");
                yield return new TestCaseData("0aa-aaa-aaaa").SetName("TestID-023N");
                yield return new TestCaseData("0999-9999-99999").SetName("TestID-024N");
                yield return new TestCaseData("0999-999-9999").SetName("TestID-025N");
                yield return new TestCaseData("099-9999-9999").SetName("TestID-026N");
                yield return new TestCaseData("099-999-99999").SetName("TestID-027N");
                yield return new TestCaseData("09-99-999").SetName("TestID-028N");
                yield return new TestCaseData("09-999-9999").SetName("TestID-029N");
                yield return new TestCaseData("099-99-9999").SetName("TestID-030N");
                yield return new TestCaseData("099-999-999").SetName("TestID-031N");
                yield return new TestCaseData("09-9999-9999").SetName("TestID-031N");
                yield return new TestCaseData("99-9999-9999").SetName("TestID-033N");
                yield return new TestCaseData("0a-aaaa-aaaa").SetName("TestID-034N");
                yield return new TestCaseData("099-99999-99999").SetName("TestID-035N");
                yield return new TestCaseData("099-9999-9999").SetName("TestID-036N");
                yield return new TestCaseData("09-99999-9999").SetName("TestID-037N");
                yield return new TestCaseData("09-9999-99999").SetName("TestID-038N");
                yield return new TestCaseData("0-999-999").SetName("TestID-039N");
                yield return new TestCaseData("0-9999-9999").SetName("TestID-040N");
                yield return new TestCaseData("09-999-9999").SetName("TestID-041N");
                yield return new TestCaseData("09-9999-999").SetName("TestID-042N");
                yield return new TestCaseData("999999999999").SetName("TestID-043N");
                yield return new TestCaseData("0999999999").SetName("TestID-044N");
                yield return new TestCaseData("9999999999").SetName("TestID-045N");
                yield return new TestCaseData("0aaaaaaaaa").SetName("TestID-046N");
                yield return new TestCaseData("09999999999").SetName("TestID-047N");
                yield return new TestCaseData("099999999").SetName("TestID-048N");
                yield return new TestCaseData("020-9999-9999").SetName("TestID-049N");
                yield return new TestCaseData("929-9999-9999").SetName("TestID-050N");
                yield return new TestCaseData("020-aaaa-aaaa").SetName("TestID-051N");
                yield return new TestCaseData("0209-99999-99999").SetName("TestID-052N");
                yield return new TestCaseData(string.Empty).SetName("TestID-053L");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-054A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpFixedLinePhoneNumber.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpFixedLinePhoneNumberTest
        {
            get
            {
                yield return new TestCaseData("09999-9-9999").SetName("TestID-000N");
                yield return new TestCaseData("99999-9-9999").SetName("TestID-001N");
                yield return new TestCaseData("0aaaa-a-aaaa").SetName("TestID-002N");
                yield return new TestCaseData("099999-9-9999").SetName("TestID-003N");
                yield return new TestCaseData("09999-99-9999").SetName("TestID-004N");
                yield return new TestCaseData("09999-9-99999").SetName("TestID-005N");
                yield return new TestCaseData("0999--999").SetName("TestID-006N");
                yield return new TestCaseData("0999-9-9999").SetName("TestID-007N");
                yield return new TestCaseData("09999--9999").SetName("TestID-008N");
                yield return new TestCaseData("09999-9-999").SetName("TestID-009N");
                yield return new TestCaseData("0999-99-9999").SetName("TestID-010N");
                yield return new TestCaseData("9999-99-9999").SetName("TestID-011N");
                yield return new TestCaseData("0aaa-aa-aaaa").SetName("TestID-012N");
                yield return new TestCaseData("09999-999-99999").SetName("TestID-013N");
                yield return new TestCaseData("09999-99-9999").SetName("TestID-014N");
                yield return new TestCaseData("0999-999-9999").SetName("TestID-015N");
                yield return new TestCaseData("0999-99-99999").SetName("TestID-016N");
                yield return new TestCaseData("099-9-999").SetName("TestID-017N");
                yield return new TestCaseData("099-99-9999").SetName("TestID-018N");
                yield return new TestCaseData("0999-9-9999").SetName("TestID-019N");
                yield return new TestCaseData("0999-99-999").SetName("TestID-020N");
                yield return new TestCaseData("099-999-9999").SetName("TestID-021N");
                yield return new TestCaseData("999-999-9999").SetName("TestID-022N");
                yield return new TestCaseData("0aa-aaa-aaaa").SetName("TestID-023N");
                yield return new TestCaseData("0999-9999-99999").SetName("TestID-024N");
                yield return new TestCaseData("0999-999-9999").SetName("TestID-025N");
                yield return new TestCaseData("099-9999-9999").SetName("TestID-026N");
                yield return new TestCaseData("099-999-99999").SetName("TestID-027N");
                yield return new TestCaseData("09-99-999").SetName("TestID-028N");
                yield return new TestCaseData("09-999-9999").SetName("TestID-029N");
                yield return new TestCaseData("099-99-9999").SetName("TestID-030N");
                yield return new TestCaseData("099-999-999").SetName("TestID-031N");
                yield return new TestCaseData("09-9999-9999").SetName("TestID-032N");
                yield return new TestCaseData("99-9999-9999").SetName("TestID-033N");
                yield return new TestCaseData("0a-aaaa-aaaa").SetName("TestID-034N");
                yield return new TestCaseData("099-99999-99999").SetName("TestID-035N");
                yield return new TestCaseData("099-9999-9999").SetName("TestID-036N");
                yield return new TestCaseData("09-99999-9999").SetName("TestID-037N");
                yield return new TestCaseData("09-9999-99999").SetName("TestID-038N");
                yield return new TestCaseData("0-999-999").SetName("TestID-039N");
                yield return new TestCaseData("0-9999-9999").SetName("TestID-040N");
                yield return new TestCaseData("09-999-9999").SetName("TestID-041N");
                yield return new TestCaseData("09-9999-999").SetName("TestID-042N");
                yield return new TestCaseData("999999999999").SetName("TestID-043N");
                yield return new TestCaseData("0999999999").SetName("TestID-044N");
                yield return new TestCaseData("9999999999").SetName("TestID-045N");
                yield return new TestCaseData("0aaaaaaaaa").SetName("TestID-046N");
                yield return new TestCaseData("09999999999").SetName("TestID-047N");
                yield return new TestCaseData("099999999").SetName("TestID-048N");
                yield return new TestCaseData(string.Empty).SetName("TestID-049L");
                yield return new TestCaseData(null).Throws(typeof(NullReferenceException)).SetName("TestID-050A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpFixedLinePhoneNumberHyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpFixedLinePhoneNumberHyphenTest
        {
            get
            {
                yield return new TestCaseData("09999-9-9999").SetName("TestID-000N");
                yield return new TestCaseData("99999-9-9999").SetName("TestID-001N");
                yield return new TestCaseData("0aaaa-a-aaaa").SetName("TestID-002N");
                yield return new TestCaseData("099999-9-9999").SetName("TestID-003N");
                yield return new TestCaseData("09999-99-9999").SetName("TestID-004N");
                yield return new TestCaseData("09999-9-99999").SetName("TestID-005N");
                yield return new TestCaseData("0999--999").SetName("TestID-006N");
                yield return new TestCaseData("0999-9-9999").SetName("TestID-007N");
                yield return new TestCaseData("09999--9999").SetName("TestID-008N");
                yield return new TestCaseData("09999-9-999").SetName("TestID-009N");
                yield return new TestCaseData("0999-99-9999").SetName("TestID-010N");
                yield return new TestCaseData("9999-99-9999").SetName("TestID-011N");
                yield return new TestCaseData("0aaa-aa-aaaa").SetName("TestID-012N");
                yield return new TestCaseData("09999-999-99999").SetName("TestID-013N");
                yield return new TestCaseData("09999-99-9999").SetName("TestID-014N");
                yield return new TestCaseData("0999-999-9999").SetName("TestID-015N");
                yield return new TestCaseData("0999-99-99999").SetName("TestID-016N");
                yield return new TestCaseData("099-9-999").SetName("TestID-017N");
                yield return new TestCaseData("099-99-9999").SetName("TestID-018N");
                yield return new TestCaseData("0999-9-9999").SetName("TestID-019N");
                yield return new TestCaseData("0999-99-999").SetName("TestID-020N");
                yield return new TestCaseData("099-999-9999").SetName("TestID-021N");
                yield return new TestCaseData("999-999-9999").SetName("TestID-022N");
                yield return new TestCaseData("0aa-aaa-aaaa").SetName("TestID-023N");
                yield return new TestCaseData("0999-9999-99999").SetName("TestID-024N");
                yield return new TestCaseData("0999-999-9999").SetName("TestID-025N");
                yield return new TestCaseData("099-9999-9999").SetName("TestID-026N");
                yield return new TestCaseData("099-999-99999").SetName("TestID-027N");
                yield return new TestCaseData("09-99-999").SetName("TestID-028N");
                yield return new TestCaseData("09-999-9999").SetName("TestID-029N");
                yield return new TestCaseData("099-99-9999").SetName("TestID-030N");
                yield return new TestCaseData("099-999-999").SetName("TestID-031N");
                yield return new TestCaseData("09-9999-9999").SetName("TestID-032N");
                yield return new TestCaseData("99-9999-9999").SetName("TestID-033N");
                yield return new TestCaseData("0a-aaaa-aaaa").SetName("TestID-034N");
                yield return new TestCaseData("099-99999-99999").SetName("TestID-035N");
                yield return new TestCaseData("099-9999-9999").SetName("TestID-036N");
                yield return new TestCaseData("09-99999-9999").SetName("TestID-037N");
                yield return new TestCaseData("09-9999-99999").SetName("TestID-038N");
                yield return new TestCaseData("0-999-999").SetName("TestID-039N");
                yield return new TestCaseData("0-9999-9999").SetName("TestID-040N");
                yield return new TestCaseData("09-999-9999").SetName("TestID-041N");
                yield return new TestCaseData("09-9999-999").SetName("TestID-042N");
                yield return new TestCaseData("999999999999").SetName("TestID-043N");
                yield return new TestCaseData("0999999999").SetName("TestID-044N");
                yield return new TestCaseData("9999999999").SetName("TestID-045N");
                yield return new TestCaseData("0aaaaaaaaa").SetName("TestID-046N");
                yield return new TestCaseData("09999999999").SetName("TestID-047N");
                yield return new TestCaseData("099999999").SetName("TestID-048N");
                yield return new TestCaseData("999999999999").SetName("TestID-049N");
                yield return new TestCaseData(string.Empty).SetName("TestID-050L");
                yield return new TestCaseData(null).Throws(typeof(NullReferenceException)).SetName("TestID-051A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpFixedLinePhoneNumberNoHyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpFixedLinePhoneNumberNoHyphenTest
        {
            get
            {
                yield return new TestCaseData("09999-9-9999").SetName("TestID-000N");
                yield return new TestCaseData("99999-9-9999").SetName("TestID-001N");
                yield return new TestCaseData("0aaaa-a-aaaa").SetName("TestID-002N");
                yield return new TestCaseData("099999-9-9999").SetName("TestID-003N");
                yield return new TestCaseData("09999-99-9999").SetName("TestID-004N");
                yield return new TestCaseData("09999-9-99999").SetName("TestID-005N");
                yield return new TestCaseData("0999--999").SetName("TestID-006N");
                yield return new TestCaseData("0999-9-9999").SetName("TestID-007N");
                yield return new TestCaseData("09999--9999").SetName("TestID-008N");
                yield return new TestCaseData("09999-9-999").SetName("TestID-009N");
                yield return new TestCaseData("0999-99-9999").SetName("TestID-010N");
                yield return new TestCaseData("9999-99-9999").SetName("TestID-011N");
                yield return new TestCaseData("0aaa-aa-aaaa").SetName("TestID-012N");
                yield return new TestCaseData("09999-999-99999").SetName("TestID-013N");
                yield return new TestCaseData("09999-99-9999").SetName("TestID-014N");
                yield return new TestCaseData("0999-999-9999").SetName("TestID-015N");
                yield return new TestCaseData("0999-99-99999").SetName("TestID-016N");
                yield return new TestCaseData("099-9-999").SetName("TestID-017N");
                yield return new TestCaseData("099-99-9999").SetName("TestID-018N");
                yield return new TestCaseData("0999-9-9999").SetName("TestID-019N");
                yield return new TestCaseData("0999-99-999").SetName("TestID-020N");
                yield return new TestCaseData("099-999-9999").SetName("TestID-021N");
                yield return new TestCaseData("999-999-9999").SetName("TestID-022N");
                yield return new TestCaseData("0aa-aaa-aaaa").SetName("TestID-023N");
                yield return new TestCaseData("0999-9999-99999").SetName("TestID-024N");
                yield return new TestCaseData("0999-999-9999").SetName("TestID-025N");
                yield return new TestCaseData("099-9999-9999").SetName("TestID-026N");
                yield return new TestCaseData("099-999-99999").SetName("TestID-027N");
                yield return new TestCaseData("09-99-999").SetName("TestID-028N");
                yield return new TestCaseData("09-999-9999").SetName("TestID-029N");
                yield return new TestCaseData("099-99-9999").SetName("TestID-030N");
                yield return new TestCaseData("099-999-999").SetName("TestID-031N");
                yield return new TestCaseData("09-9999-9999").SetName("TestID-032N");
                yield return new TestCaseData("99-9999-9999").SetName("TestID-033N");
                yield return new TestCaseData("0a-aaaa-aaaa").SetName("TestID-034N");
                yield return new TestCaseData("099-99999-99999").SetName("TestID-035N");
                yield return new TestCaseData("099-9999-9999").SetName("TestID-036N");
                yield return new TestCaseData("09-99999-9999").SetName("TestID-037N");
                yield return new TestCaseData("09-9999-99999").SetName("TestID-038N");
                yield return new TestCaseData("0-999-999").SetName("TestID-039N");
                yield return new TestCaseData("0-9999-9999").SetName("TestID-040N");
                yield return new TestCaseData("09-999-9999").SetName("TestID-041N");
                yield return new TestCaseData("09-9999-999").SetName("TestID-042N");
                yield return new TestCaseData("999999999999").SetName("TestID-043N");
                yield return new TestCaseData("0999999999").SetName("TestID-044N");
                yield return new TestCaseData("9999999999").SetName("TestID-045N");
                yield return new TestCaseData("0aaaaaaaaa").SetName("TestID-046N");
                yield return new TestCaseData("09999999999").SetName("TestID-047N");
                yield return new TestCaseData("099999999").SetName("TestID-048N");
                yield return new TestCaseData("999999999999").SetName("TestID-049N");
                yield return new TestCaseData(string.Empty).SetName("TestID-050L");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-051A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpCellularPhoneNumber.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpCellularPhoneNumberTest
        {
            get
            {
                yield return new TestCaseData("020-9999-9999").SetName("TestID-000N");
                yield return new TestCaseData("929-9999-9999").SetName("TestID-001N");
                yield return new TestCaseData("020-aaaa-aaaa").SetName("TestID-002N");
                yield return new TestCaseData("0209-99999-99999").SetName("TestID-003N");
                yield return new TestCaseData("0209-99999-99999").SetName("TestID-004N");
                yield return new TestCaseData("020-99999-9999").SetName("TestID-005N");
                yield return new TestCaseData("020-99999-9999").SetName("TestID-006N");
                yield return new TestCaseData("02-999-999").SetName("TestID-007N");
                yield return new TestCaseData("02-9999-9999").SetName("TestID-008N");
                yield return new TestCaseData("020-999-9999").SetName("TestID-009N");
                yield return new TestCaseData("020-9999-999").SetName("TestID-010N");
                yield return new TestCaseData("060-9999-9999").SetName("TestID-011N");
                yield return new TestCaseData("969-9999-9999").SetName("TestID-012N");
                yield return new TestCaseData("060-aaaa-aaaa").SetName("TestID-013N");
                yield return new TestCaseData("0609-99999-99999").SetName("TestID-014N");
                yield return new TestCaseData("0609-9999-9999").SetName("TestID-015N");
                yield return new TestCaseData("060-99999-9999").SetName("TestID-016N");
                yield return new TestCaseData("060-9999-99999").SetName("TestID-017N");
                yield return new TestCaseData("06-999-999").SetName("TestID-018N");
                yield return new TestCaseData("06-9999-9999").SetName("TestID-019N");
                yield return new TestCaseData("060-999-9999").SetName("TestID-020N");
                yield return new TestCaseData("060-9999-999").SetName("TestID-021N");
                yield return new TestCaseData("070-9999-9999").SetName("TestID-022N");
                yield return new TestCaseData("979-9999-9999").SetName("TestID-023N");
                yield return new TestCaseData("070-aaaa-aaaa").SetName("TestID-024N");
                yield return new TestCaseData("0709-99999-99999").SetName("TestID-025N");
                yield return new TestCaseData("0709-9999-9999").SetName("TestID-026N");
                yield return new TestCaseData("070-99999-9999").SetName("TestID-027N");
                yield return new TestCaseData("070-9999-99999").SetName("TestID-028N");
                yield return new TestCaseData("07-999-999").SetName("TestID-029N");
                yield return new TestCaseData("07-9999-9999").SetName("TestID-030N");
                yield return new TestCaseData("070-999-9999").SetName("TestID-031N");
                yield return new TestCaseData("070-9999-999").SetName("TestID-032N");
                yield return new TestCaseData("080-9999-9999").SetName("TestID-033N");
                yield return new TestCaseData("989-9999-9999").SetName("TestID-034N");
                yield return new TestCaseData("080-aaaa-aaaa").SetName("TestID-035N");
                yield return new TestCaseData("0809-99999-99999").SetName("TestID-036N");
                yield return new TestCaseData("0809-9999-9999").SetName("TestID-037N");
                yield return new TestCaseData("080-99999-9999").SetName("TestID-038N");
                yield return new TestCaseData("080-9999-99999").SetName("TestID-039N");
                yield return new TestCaseData("08-999-999").SetName("TestID-040N");
                yield return new TestCaseData("08-9999-9999").SetName("TestID-041N");
                yield return new TestCaseData("080-999-9999").SetName("TestID-042N");
                yield return new TestCaseData("080-9999-999").SetName("TestID-043N");
                yield return new TestCaseData("090-9999-9999").SetName("TestID-044N");
                yield return new TestCaseData("999-9999-9999").SetName("TestID-045N");
                yield return new TestCaseData("090-aaaa-aaaa").SetName("TestID-046N");
                yield return new TestCaseData("0909-99999-99999").SetName("TestID-047N");
                yield return new TestCaseData("0909-9999-9999").SetName("TestID-048N");
                yield return new TestCaseData("090-99999-9999").SetName("TestID-049N");
                yield return new TestCaseData("02099999999").SetName("TestID-050N");
                yield return new TestCaseData("92999999999").SetName("TestID-051N");
                yield return new TestCaseData("020aaaaaaaa").SetName("TestID-052N");
                yield return new TestCaseData("020999999999").SetName("TestID-053N");
                yield return new TestCaseData("0209999999").SetName("TestID-054N");
                yield return new TestCaseData("06099999999").SetName("TestID-055N");
                yield return new TestCaseData("07099999999").SetName("TestID-056N");
                yield return new TestCaseData("08099999999").SetName("TestID-057N");
                yield return new TestCaseData("09099999999").SetName("TestID-058N");
                yield return new TestCaseData(string.Empty).SetName("TestID-059L");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-060A");
            }

        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpCellularPhoneNumberHyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpCellularPhoneNumberHyphenTest
        {
            get
            {
                yield return new TestCaseData("020aaaaaaaa").SetName("TestID-000N");
                yield return new TestCaseData("92999999999").SetName("TestID-001N");
                yield return new TestCaseData("020-aaaa-aaaa").SetName("TestID-002N");
                yield return new TestCaseData("020aaaaaaaa").SetName("TestID-003N");
                yield return new TestCaseData("0209-99999-99999").SetName("TestID-004N");
                yield return new TestCaseData("020-99999-9999").SetName("TestID-005N");
                yield return new TestCaseData("020-99999-9999").SetName("TestID-006N");
                yield return new TestCaseData("02-999-999").SetName("TestID-007N");
                yield return new TestCaseData("02-9999-9999").SetName("TestID-008N");
                yield return new TestCaseData("020-999-9999").SetName("TestID-009N");
                yield return new TestCaseData("020-9999-999").SetName("TestID-010N");
                yield return new TestCaseData("060-9999-9999").SetName("TestID-011N");
                yield return new TestCaseData("969-9999-9999").SetName("TestID-012N");
                yield return new TestCaseData("060-aaaa-aaaa").SetName("TestID-013N");
                yield return new TestCaseData("0609-99999-99999").SetName("TestID-014N");
                yield return new TestCaseData("0609-9999-9999").SetName("TestID-015N");
                yield return new TestCaseData("060-99999-9999").SetName("TestID-016N");
                yield return new TestCaseData("060-9999-99999").SetName("TestID-017N");
                yield return new TestCaseData("06-999-999").SetName("TestID-018N");
                yield return new TestCaseData("06-9999-9999").SetName("TestID-019N");
                yield return new TestCaseData("060-999-9999").SetName("TestID-020N");
                yield return new TestCaseData("060-9999-999").SetName("TestID-021N");
                yield return new TestCaseData("070-9999-9999").SetName("TestID-022N");
                yield return new TestCaseData("979-9999-9999").SetName("TestID-023N");
                yield return new TestCaseData("070-aaaa-aaaa").SetName("TestID-024N");
                yield return new TestCaseData("0709-99999-99999").SetName("TestID-025N");
                yield return new TestCaseData("0709-9999-9999").SetName("TestID-026N");
                yield return new TestCaseData("070-99999-9999").SetName("TestID-027N");
                yield return new TestCaseData("070-9999-99999").SetName("TestID-028N");
                yield return new TestCaseData("07-999-999").SetName("TestID-029N");
                yield return new TestCaseData("07-9999-9999").SetName("TestID-030N");
                yield return new TestCaseData("070-999-9999").SetName("TestID-031N");
                yield return new TestCaseData("070-9999-999").SetName("TestID-032N");
                yield return new TestCaseData("080-9999-9999").SetName("TestID-033N");
                yield return new TestCaseData("989-9999-9999").SetName("TestID-034N");
                yield return new TestCaseData("080-aaaa-aaaa").SetName("TestID-035N");
                yield return new TestCaseData("0809-99999-99999").SetName("TestID-036N");
                yield return new TestCaseData("0809-9999-9999").SetName("TestID-037N");
                yield return new TestCaseData("080-99999-9999").SetName("TestID-038N");
                yield return new TestCaseData("080-9999-99999").SetName("TestID-039N");
                yield return new TestCaseData("020999999999").SetName("TestID-040N");
                yield return new TestCaseData("0209999999").SetName("TestID-041N");
                yield return new TestCaseData("06099999999").SetName("TestID-042N");
                yield return new TestCaseData("07099999999").SetName("TestID-043N");
                yield return new TestCaseData("08099999999").SetName("TestID-044N");
                yield return new TestCaseData("09099999999").SetName("TestID-045N");
                yield return new TestCaseData("0109999999").SetName("TestID-046N");
                yield return new TestCaseData("03099999999").SetName("TestID-047N");
                yield return new TestCaseData("04099999999").SetName("TestID-048N");
                yield return new TestCaseData("05099999999").SetName("TestID-049N");
                yield return new TestCaseData("02099999999").SetName("TestID-050N");
                yield return new TestCaseData("92999999999").SetName("TestID-051N");
                yield return new TestCaseData("020aaaaaaaa").SetName("TestID-052N");
                yield return new TestCaseData("020999999999").SetName("TestID-053N");
                yield return new TestCaseData("0209999999").SetName("TestID-054N");
                yield return new TestCaseData("06099999999").SetName("TestID-055N");
                yield return new TestCaseData("07099999999").SetName("TestID-056N");
                yield return new TestCaseData("08099999999").SetName("TestID-057N");
                yield return new TestCaseData("09099999999").SetName("TestID-058N");
                yield return new TestCaseData(string.Empty).SetName("TestID-059L");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-060A");

            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpCellularPhoneNumberNoHyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpCellularPhoneNumberNoHyphenTest
        {
            get
            {
                yield return new TestCaseData("020aaaaaaaa").SetName("TestID-000N");
                yield return new TestCaseData("92999999999").SetName("TestID-001N");
                yield return new TestCaseData("020-aaaa-aaaa").SetName("TestID-002N");
                yield return new TestCaseData("020aaaaaaaa").SetName("TestID-003N");
                yield return new TestCaseData("0209-99999-99999").SetName("TestID-004N");
                yield return new TestCaseData("020-99999-9999").SetName("TestID-005N");
                yield return new TestCaseData("020-99999-9999").SetName("TestID-006N");
                yield return new TestCaseData("02-999-999").SetName("TestID-007N");
                yield return new TestCaseData("02-9999-9999").SetName("TestID-008N");
                yield return new TestCaseData("020-999-9999").SetName("TestID-009N");
                yield return new TestCaseData("020-9999-999").SetName("TestID-010N");
                yield return new TestCaseData("060-9999-9999").SetName("TestID-011N");
                yield return new TestCaseData("969-9999-9999").SetName("TestID-012N");
                yield return new TestCaseData("060-aaaa-aaaa").SetName("TestID-013N");
                yield return new TestCaseData("0609-99999-99999").SetName("TestID-014N");
                yield return new TestCaseData("0609-9999-9999").SetName("TestID-015N");
                yield return new TestCaseData("060-99999-9999").SetName("TestID-016N");
                yield return new TestCaseData("060-9999-99999").SetName("TestID-017N");
                yield return new TestCaseData("06-999-999").SetName("TestID-018N");
                yield return new TestCaseData("06-9999-9999").SetName("TestID-019N");
                yield return new TestCaseData("060-999-9999").SetName("TestID-020N");
                yield return new TestCaseData("060-9999-999").SetName("TestID-021N");
                yield return new TestCaseData("070-9999-9999").SetName("TestID-022N");
                yield return new TestCaseData("979-9999-9999").SetName("TestID-023N");
                yield return new TestCaseData("070-aaaa-aaaa").SetName("TestID-024N");
                yield return new TestCaseData("0709-99999-99999").SetName("TestID-025N");
                yield return new TestCaseData("0709-9999-9999").SetName("TestID-026N");
                yield return new TestCaseData("070-99999-9999").SetName("TestID-027N");
                yield return new TestCaseData("070-9999-99999").SetName("TestID-028N");
                yield return new TestCaseData("07-999-999").SetName("TestID-029N");
                yield return new TestCaseData("07-9999-9999").SetName("TestID-030N");
                yield return new TestCaseData("070-999-9999").SetName("TestID-031N");
                yield return new TestCaseData("070-9999-999").SetName("TestID-032N");
                yield return new TestCaseData("080-9999-9999").SetName("TestID-033N");
                yield return new TestCaseData("989-9999-9999").SetName("TestID-034N");
                yield return new TestCaseData("080-aaaa-aaaa").SetName("TestID-035N");
                yield return new TestCaseData("0809-99999-99999").SetName("TestID-036N");
                yield return new TestCaseData("0809-9999-9999").SetName("TestID-037N");
                yield return new TestCaseData("080-99999-9999").SetName("TestID-038N");
                yield return new TestCaseData("080-9999-99999").SetName("TestID-039N");
                yield return new TestCaseData("020999999999").SetName("TestID-040N");
                yield return new TestCaseData("0209999999").SetName("TestID-041N");
                yield return new TestCaseData("06099999999").SetName("TestID-042N");
                yield return new TestCaseData("07099999999").SetName("TestID-043N");
                yield return new TestCaseData("08099999999").SetName("TestID-044N");
                yield return new TestCaseData("09099999999").SetName("TestID-045N");
                yield return new TestCaseData("0109999999").SetName("TestID-046N");
                yield return new TestCaseData("03099999999").SetName("TestID-047N");
                yield return new TestCaseData("04099999999").SetName("TestID-048N");
                yield return new TestCaseData("05099999999").SetName("TestID-049N");
                yield return new TestCaseData("02099999999").SetName("TestID-050N");
                yield return new TestCaseData("92999999999").SetName("TestID-051N");
                yield return new TestCaseData("020aaaaaaaa").SetName("TestID-052N");
                yield return new TestCaseData("020999999999").SetName("TestID-053N");
                yield return new TestCaseData("0209999999").SetName("TestID-054N");
                yield return new TestCaseData("06099999999").SetName("TestID-055N");
                yield return new TestCaseData("07099999999").SetName("TestID-056N");
                yield return new TestCaseData("08099999999").SetName("TestID-057N");
                yield return new TestCaseData("09099999999").SetName("TestID-058N");
                yield return new TestCaseData(string.Empty).SetName("TestID-059L");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-060A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpIpPhoneNumbe.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpIpPhoneNumberTest
        {
            get
            {
                 yield return new TestCaseData("050-9999-9999").SetName("TestID-000N");
                yield return new TestCaseData("959-9999-9999").SetName("TestID-001N");
                yield return new TestCaseData("050-aaaa-aaaa").SetName("TestID-002N");
                yield return new TestCaseData("0509-99999-99999").SetName("TestID-003N");
                yield return new TestCaseData("0509-9999-9999").SetName("TestID-004N");
                yield return new TestCaseData("050-99999-9999").SetName("TestID-005N");
                yield return new TestCaseData("050-9999-99999").SetName("TestID-006N");
                yield return new TestCaseData("05-999-999").SetName("TestID-007N");
                yield return new TestCaseData("05-9999-9999").SetName("TestID-008N");
                yield return new TestCaseData("050-999-9999").SetName("TestID-009N");
                yield return new TestCaseData("050-9999-999").SetName("TestID-010N");
                yield return new TestCaseData("9999999999999").SetName("TestID-011N");
                yield return new TestCaseData("010-9999-9999").SetName("TestID-012N");
                yield return new TestCaseData("020-9999-9999").SetName("TestID-013N");
                yield return new TestCaseData("030-9999-9999").SetName("TestID-014N");
                yield return new TestCaseData("040-9999-9999").SetName("TestID-015N");
                yield return new TestCaseData("060-9999-9999").SetName("TestID-016N");
                yield return new TestCaseData("070-9999-9999").SetName("TestID-017N");
                yield return new TestCaseData("080-9999-9999").SetName("TestID-018N");
                yield return new TestCaseData("090-9999-9999").SetName("TestID-019N");
                yield return new TestCaseData("05099999999").SetName("TestID-020N");
                yield return new TestCaseData("95999999999").SetName("TestID-021N");
                yield return new TestCaseData("050aaaaaaaa").SetName("TestID-022N");
                yield return new TestCaseData("050999999999").SetName("TestID-023N");
                yield return new TestCaseData("0509999999").SetName("TestID-024N");
                yield return new TestCaseData("01099999999").SetName("TestID-025N");
                yield return new TestCaseData("02099999999").SetName("TestID-026N");
                yield return new TestCaseData("03099999999").SetName("TestID-027N");
                yield return new TestCaseData("04099999999").SetName("TestID-028N");
                yield return new TestCaseData("06099999999").SetName("TestID-029N");
                yield return new TestCaseData("07099999999").SetName("TestID-030N");
                yield return new TestCaseData("08099999999").SetName("TestID-031N");
                yield return new TestCaseData("09099999999").SetName("TestID-032N");
                yield return new TestCaseData(string.Empty).SetName("TestID-033L");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-034A");
            }
        }


        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpIpPhoneNumberHyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpIpPhoneNumberHyphenTest
        {
            get
            {
                yield return new TestCaseData("050-9999-9999").SetName("TestID-000N");
                yield return new TestCaseData("959-9999-9999").SetName("TestID-001N");
                yield return new TestCaseData("050-aaaa-aaaa").SetName("TestID-002N");
                yield return new TestCaseData("0509-99999-99999").SetName("TestID-003N");
                yield return new TestCaseData("0509-9999-9999").SetName("TestID-004N");
                yield return new TestCaseData("050-99999-9999").SetName("TestID-005N");
                yield return new TestCaseData("050-9999-99999").SetName("TestID-006N");
                yield return new TestCaseData("05-999-999").SetName("TestID-007N");
                yield return new TestCaseData("05-9999-9999").SetName("TestID-008N");
                yield return new TestCaseData("050-999-9999").SetName("TestID-009N");
                yield return new TestCaseData("050-9999-999").SetName("TestID-010N");
                yield return new TestCaseData("9999999999999").SetName("TestID-011N");
                yield return new TestCaseData("010-9999-9999").SetName("TestID-012N");
                yield return new TestCaseData("020-9999-9999").SetName("TestID-013N");
                yield return new TestCaseData("030-9999-9999").SetName("TestID-014N");
                yield return new TestCaseData("040-9999-9999").SetName("TestID-015N");
                yield return new TestCaseData("060-9999-9999").SetName("TestID-016N");
                yield return new TestCaseData("070-9999-9999").SetName("TestID-017N");
                yield return new TestCaseData("080-9999-9999").SetName("TestID-018N");
                yield return new TestCaseData("090-9999-9999").SetName("TestID-019N");
                yield return new TestCaseData("95-999999999").SetName("TestID-020N");
                yield return new TestCaseData("95999999999").SetName("TestID-021N");
                yield return new TestCaseData("050aaaaaaaa").SetName("TestID-022N");
                yield return new TestCaseData("050999999999").SetName("TestID-023N");
                yield return new TestCaseData("0509999999").SetName("TestID-024N");
                yield return new TestCaseData("01099999999").SetName("TestID-025N");
                yield return new TestCaseData("02099999999").SetName("TestID-026N");
                yield return new TestCaseData("03099999999").SetName("TestID-027N");
                yield return new TestCaseData("04099999999").SetName("TestID-028N");
                yield return new TestCaseData("06099999999").SetName("TestID-029N");
                yield return new TestCaseData("07099999999").SetName("TestID-030N");
                yield return new TestCaseData("08099999999").SetName("TestID-031N");
                yield return new TestCaseData("09099999999").SetName("TestID-032N");
                yield return new TestCaseData(string.Empty).SetName("TestID-033L");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-034A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method IsJpIpPhoneNumberNoHyphen.
        /// </summary>
        public IEnumerable<TestCaseData> TestIsJpIpPhoneNumberNoHyphenTest
        {
            get
            {
                yield return new TestCaseData("06-999-999").SetName("TestID-000N");
                yield return new TestCaseData("959-9999-9999").SetName("TestID-001N");
                yield return new TestCaseData("050-aaaa-aaaa").SetName("TestID-002N");
                yield return new TestCaseData("0509-99999-99999").SetName("TestID-003N");
                yield return new TestCaseData("0509-9999-9999").SetName("TestID-004N");
                yield return new TestCaseData("050-99999-9999").SetName("TestID-005N");
                yield return new TestCaseData("050-9999-99999").SetName("TestID-006N");
                yield return new TestCaseData("05-999-999").SetName("TestID-007N");
                yield return new TestCaseData("05-9999-9999").SetName("TestID-008N");
                yield return new TestCaseData("050-999-9999").SetName("TestID-009N");
                yield return new TestCaseData("050-9999-999").SetName("TestID-010N");
                yield return new TestCaseData("9999999999999").SetName("TestID-011N");
                yield return new TestCaseData("010-9999-9999").SetName("TestID-012N");
                yield return new TestCaseData("020-9999-9999").SetName("TestID-013N");
                yield return new TestCaseData("030-9999-9999").SetName("TestID-014N");
                yield return new TestCaseData("040-9999-9999").SetName("TestID-015N");
                yield return new TestCaseData("060-9999-9999").SetName("TestID-016N");
                yield return new TestCaseData("070-9999-9999").SetName("TestID-017N");
                yield return new TestCaseData("080-9999-9999").SetName("TestID-018N");
                yield return new TestCaseData("090-9999-9999").SetName("TestID-019N");
                yield return new TestCaseData("05099999999").SetName("TestID-020N");
                yield return new TestCaseData("95999999999").SetName("TestID-021N");
                yield return new TestCaseData("050aaaaaaaa").SetName("TestID-022N");
                yield return new TestCaseData("050999999999").SetName("TestID-023N");
                yield return new TestCaseData("0509999999").SetName("TestID-024N");
                yield return new TestCaseData("01099999999").SetName("TestID-025N");
                yield return new TestCaseData("02099999999").SetName("TestID-026N");
                yield return new TestCaseData("03099999999").SetName("TestID-027N");
                yield return new TestCaseData("04099999999").SetName("TestID-028N");
                yield return new TestCaseData("06099999999").SetName("TestID-029N");
                yield return new TestCaseData("07099999999").SetName("TestID-030N");
                yield return new TestCaseData("08099999999").SetName("TestID-031N");
                yield return new TestCaseData("09099999999").SetName("TestID-032N");
                yield return new TestCaseData(string.Empty).SetName("TestID-033L");
                yield return new TestCaseData(null).Throws(typeof(ArgumentNullException)).SetName("TestID-034A");
            }
        }

        #endregion

        #region Test Code

        /// <summary>
        /// IsJpZipCodeTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpZipCodeTest")]
        public static void IsJpZipCodeTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpZipCode(str))
                    Assert.That(FormatChecker.IsJpZipCode(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpZipCode(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpZipCode_HyphenTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpZipCodeHyphenTest")]
        public static void IsJpZipCode_HyphenTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpZipCode_Hyphen(str))
                    Assert.That(FormatChecker.IsJpZipCode_Hyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpZipCode_Hyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpZipCode_NoHyphenTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpZipCodeNoHyphenTest")]
        public static void IsJpZipCode_NoHyphenTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpZipCode_NoHyphen(str))
                    Assert.That(FormatChecker.IsJpZipCode_NoHyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpZipCode_NoHyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpZipCode7Test Method
        /// </summary>
        /// <param name="str"></param>
        [TestCaseSource("TestIsJpZipCode7Test")]
        public static void IsJpZipCode7Test(string str)
        {
            try
            {
                if (FormatChecker.IsJpZipCode7(str))
                    Assert.That(FormatChecker.IsJpZipCode7(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpZipCode7(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpZipCode7_HyphenTest Methos
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpZipCode7HyphenTest")]
        public static void IsJpZipCode7_HyphenTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpZipCode7_Hyphen(str))
                    Assert.That(FormatChecker.IsJpZipCode7_Hyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpZipCode7_Hyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpZipCode7_NoHyphenTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpZipCode7NoHyphenTest")]
        public static void IsJpZipCode7_NoHyphenTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpZipCode7_NoHyphen(str))
                    Assert.That(FormatChecker.IsJpZipCode7_NoHyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpZipCode7_NoHyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpZipCode5Test Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpZipCode5Test")]
        public static void IsJpZipCode5Test(string str)
        {
            try
            {
                if (FormatChecker.IsJpZipCode5(str))
                    Assert.That(FormatChecker.IsJpZipCode5(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpZipCode5(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpZipCode5_HyphenTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpZipCode5HyphenTest")]
        public static void IsJpZipCode5_HyphenTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpZipCode5_Hyphen(str))
                    Assert.That(FormatChecker.IsJpZipCode5_Hyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpZipCode5_Hyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpZipCode5_NoHyphenTest Method
        /// </summary>       
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpZipCode5NoHyphenTest")]
        public static void IsJpZipCode5_NoHyphenTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpZipCode5_NoHyphen(str))
                    Assert.That(FormatChecker.IsJpZipCode5_NoHyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpZipCode5_NoHyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpTelephoneNumberTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpTelephoneNumberTest")]
        public static void IsJpTelephoneNumberTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpTelephoneNumber(str))
                    Assert.That(FormatChecker.IsJpTelephoneNumber(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpTelephoneNumber(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpTelephoneNumber_HyphenTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpTelephoneNumberHyphenTest")]
        public static void IsJpTelephoneNumber_HyphenTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpTelephoneNumber_Hyphen(str))
                    Assert.That(FormatChecker.IsJpTelephoneNumber_Hyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpTelephoneNumber_Hyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpTelephoneNumber_NoHyphenTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpTelephoneNumberNoHyphenTest")]
        public static void IsJpTelephoneNumber_NoHyphenTest(string str)
        {
            try
            {

                if (FormatChecker.IsJpTelephoneNumber_NoHyphen(str))
                    Assert.That(FormatChecker.IsJpTelephoneNumber_NoHyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpTelephoneNumber_NoHyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpFixedLinePhoneNumberTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpFixedLinePhoneNumberTest")]
        public static void IsJpFixedLinePhoneNumberTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpFixedLinePhoneNumber(str))
                    Assert.That(FormatChecker.IsJpFixedLinePhoneNumber(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpFixedLinePhoneNumber(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpFixedLinePhoneNumber_HyphenTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpFixedLinePhoneNumberHyphenTest")]
        public static void IsJpFixedLinePhoneNumber_HyphenTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(str))
                    Assert.That(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpFixedLinePhoneNumber_Hyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpFixedLinePhoneNumber_NoHyphenTest Mehod
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpFixedLinePhoneNumberNoHyphenTest")]
        public static void IsJpFixedLinePhoneNumber_NoHyphenTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(str))
                    Assert.That(FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpFixedLinePhoneNumber_NoHyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpCellularPhoneNumberTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpCellularPhoneNumberTest")]
        public static void IsJpCellularPhoneNumberTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpCellularPhoneNumber(str))
                    Assert.That(FormatChecker.IsJpCellularPhoneNumber(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpCellularPhoneNumber(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpCellularPhoneNumber_HyphenTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpCellularPhoneNumberHyphenTest")]
        public static void IsJpCellularPhoneNumber_HyphenTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpCellularPhoneNumber_Hyphen(str))
                    Assert.That(FormatChecker.IsJpCellularPhoneNumber_Hyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpCellularPhoneNumber_Hyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpCellularPhoneNumber_NoHyphenTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpCellularPhoneNumberNoHyphenTest")]
        public static void IsJpCellularPhoneNumber_NoHyphenTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpCellularPhoneNumber_NoHyphen(str))
                    Assert.That(FormatChecker.IsJpCellularPhoneNumber_NoHyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpCellularPhoneNumber_NoHyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpIpPhoneNumberTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpIpPhoneNumberTest")]
        public static void IsJpIpPhoneNumberTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpIpPhoneNumber(str))
                    Assert.That(FormatChecker.IsJpIpPhoneNumber(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpIpPhoneNumber(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpIpPhoneNumber_HyphenTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpIpPhoneNumberHyphenTest")]
        public static void IsJpIpPhoneNumber_HyphenTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpIpPhoneNumber(str))
                    Assert.That(FormatChecker.IsJpIpPhoneNumber_Hyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpIpPhoneNumber_Hyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsJpIpPhoneNumber_NoHyphenTest Method
        /// </summary>
        /// <param name="str">str</param>
        [TestCaseSource("TestIsJpIpPhoneNumberNoHyphenTest")]
        public static void IsJpIpPhoneNumber_NoHyphenTest(string str)
        {
            try
            {
                if (FormatChecker.IsJpIpPhoneNumber(str))
                    Assert.That(FormatChecker.IsJpIpPhoneNumber_NoHyphen(str), Is.True);
                else
                    Assert.That(FormatChecker.IsJpIpPhoneNumber_NoHyphen(str), Is.False);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

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
        #endregion
    }
}

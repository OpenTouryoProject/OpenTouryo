
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
//* クラス名        ：FormatConverterTest.cs
//* クラス日本語名  ：Test of the class to Convert The Format
//*
//* 作成者          ：Rituparna
//* 更新履歴        ：
//* 
//*  Date:        Author:          Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/14/2014   Rituparna        Testcode development for FormatChecker.
//*
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
    public class FormatConverterTest
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
        /// This method to generate test data to be passed to the method RoundBanker.
        /// </summary>
        public IEnumerable<TestCaseData> TestRoundBankerTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", 12, 2, "12");
                yield return new TestCaseData("TestID-001N", 12.888, 2, "12.89");
                yield return new TestCaseData("TestID-002N", 5.61111, 2, "5.61");
                yield return new TestCaseData("TestID-003N", 10.135, 2, "10.14");
                yield return new TestCaseData("TestID-004N", 10.99999, 2, "11.00");
                yield return new TestCaseData("TestID-005L", string.Empty, 2, "0");
                yield return new TestCaseData("TestID-006N", 12.88, 3, "12.88");
                yield return new TestCaseData("TestID-007N", 12.888, 3, "12.888");
                yield return new TestCaseData("TestID-008N", 12.8888, 3, "12.889");
                yield return new TestCaseData("TestID-009N", 12.9999, 3, "13.000");
                yield return new TestCaseData("TestID-010N", 12.6111, 3, "12.611");
                yield return new TestCaseData("TestID-011A", null, 2, "0").Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-012N", 12.6111, 0, "13");
                yield return new TestCaseData("TestID-013A", 12.6111, -1, "12.611").Throws(typeof(ArgumentOutOfRangeException));
                yield return new TestCaseData("TestID-014N", 12.6111, 7, "12.6111");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method Round4sya5nyu.
        /// </summary>
        public IEnumerable<TestCaseData> TestRound4sya5nyuTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", 12, 2, "12");
                yield return new TestCaseData("TestID-001N", 12.888, 2, "12.89");
                yield return new TestCaseData("TestID-002N", 5.61111, 2, "5.61");
                yield return new TestCaseData("TestID-003N", 10.135, 2, "10.14");
                yield return new TestCaseData("TestID-004N", 10.99999, 2, "11.00");
                yield return new TestCaseData("TestID-005L", string.Empty, 2, "0");
                yield return new TestCaseData("TestID-006N", 12.88, 3, "12.88");
                yield return new TestCaseData("TestID-007N", 12.888, 3, "12.888");
                yield return new TestCaseData("TestID-008N", 12.8888, 3, "12.889");
                yield return new TestCaseData("TestID-009N", 12.9999, 3, "13.000");
                yield return new TestCaseData("TestID-010N", 12.6111, 3, "12.611");
                yield return new TestCaseData("TestID-011A", null, 2, "0").Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-012N", 12.6111, 0, "13");
                yield return new TestCaseData("TestID-013A", 12.6111, -1, "12.611").Throws(typeof(ArgumentOutOfRangeException));
                yield return new TestCaseData("TestID-014N", 12.6111, 7, "12.6111");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method Floor.
        /// </summary>
        public IEnumerable<TestCaseData> TestFloorTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", 12, Convert.ToUInt32("2"), "12.00");
                yield return new TestCaseData("TestID-001N", 12.888, Convert.ToUInt32("2"), "12.88");
                yield return new TestCaseData("TestID-002N", 5.61111, Convert.ToUInt32("2"), "5.61");
                yield return new TestCaseData("TestID-003N", 10.135, Convert.ToUInt32("2"), "10.13");
                yield return new TestCaseData("TestID-004N", 10.99999, Convert.ToUInt32("2"), "10.99");
                yield return new TestCaseData("TestID-005L", string.Empty, Convert.ToUInt32("2"), "0");
                yield return new TestCaseData("TestID-006N", 12.88, Convert.ToUInt32("3"), "12.880");
                yield return new TestCaseData("TestID-007N", 12.888, Convert.ToUInt32("3"), "12.888");
                yield return new TestCaseData("TestID-008N", 12.8888, Convert.ToUInt32("3"), "12.888");
                yield return new TestCaseData("TestID-009N", 12.9999, Convert.ToUInt32("3"), "12.999");
                yield return new TestCaseData("TestID-010N", 12.6111, Convert.ToUInt32("3"), "12.611");
                yield return new TestCaseData("TestID-011A", null, Convert.ToUInt32("2"), "0").Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-012N", 12.6111, Convert.ToUInt32("0"), "12");
                yield return new TestCaseData("TestID-013N", 12.6111, Convert.ToUInt32("7"), "12.6111000");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method Floor.
        /// </summary>
        public IEnumerable<TestCaseData> TestFloorftTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", 12, Convert.ToUInt32("2"), 2, "12.00");
                yield return new TestCaseData("TestID-001N", 12.888, Convert.ToUInt32("2"), 1, "12.88");
                yield return new TestCaseData("TestID-002N", 5.61111, Convert.ToUInt32("2"), 2, "5.61");
                yield return new TestCaseData("TestID-003N", 10.135, Convert.ToUInt32("2"), 2, "10.13");
                yield return new TestCaseData("TestID-004N", 10.99999, Convert.ToUInt32("2"), 1, "10.99");
                yield return new TestCaseData("TestID-005L", string.Empty, Convert.ToUInt32("2"), 1, "0");
                yield return new TestCaseData("TestID-006N", 12.88, Convert.ToUInt32("3"), 2, "12.880");
                yield return new TestCaseData("TestID-007N", 12.888, Convert.ToUInt32("3"), 2, "12.888");
                yield return new TestCaseData("TestID-008N", 12.8888, Convert.ToUInt32("3"), 3, "12.888");
                yield return new TestCaseData("TestID-009N", 12.9999, Convert.ToUInt32("3"), 3, "12.999");
                yield return new TestCaseData("TestID-010N", 12.6111, Convert.ToUInt32("3"), 2, "12.611");
                yield return new TestCaseData("TestID-011A", null, Convert.ToUInt32("2"), 2, "0").Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-012N", 0.09m, Convert.ToUInt32("2"), 2, "0.09");
                yield return new TestCaseData("TestID-013N", Convert.ToUInt32(20), Convert.ToUInt32("2"), 2, "20.00");
                yield return new TestCaseData("TestID-014N", 12, Convert.ToUInt32("2"), FloorToward.RZ, "12.00");
                yield return new TestCaseData("TestID-015N", 12, Convert.ToUInt32("2"), FloorToward.RM, "12.00");
                yield return new TestCaseData("TestID-016N", 0.09m, Convert.ToUInt32("2"), FloorToward.RM, "0.09");
                yield return new TestCaseData("TestID-017N", 0.09m, Convert.ToUInt32("2"), FloorToward.RZ, "0.09");
                yield return new TestCaseData("TestID-018N", -0.09m, Convert.ToUInt32("3"), FloorToward.RZ, "-0.090");
                yield return new TestCaseData("TestID-012N", 12.6111, Convert.ToUInt32("0"), FloorToward.RZ, "12");
                yield return new TestCaseData("TestID-013N", 12.6111, Convert.ToUInt32("7"), FloorToward.RZ, "12.6111000");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method Ceiling.
        /// </summary>
        public IEnumerable<TestCaseData> TestCeilingTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", 12, Convert.ToUInt32("2"), "12.00");
                yield return new TestCaseData("TestID-001N", 12.888, Convert.ToUInt32("2"), "12.89");
                yield return new TestCaseData("TestID-002N", 5.61111, Convert.ToUInt32("2"), "5.62");
                yield return new TestCaseData("TestID-003N", 10.135, Convert.ToUInt32("2"), "10.14");
                yield return new TestCaseData("TestID-004N", 10.99999, Convert.ToUInt32("2"), "11.00");
                yield return new TestCaseData("TestID-005L", string.Empty, Convert.ToUInt32("2"), "0");
                yield return new TestCaseData("TestID-006N", 12.88, Convert.ToUInt32("3"), "12.880");
                yield return new TestCaseData("TestID-007N", 12.888, Convert.ToUInt32("3"), "12.888");
                yield return new TestCaseData("TestID-008N", 12.8888, Convert.ToUInt32("3"), "12.889");
                yield return new TestCaseData("TestID-009N", 12.9999, Convert.ToUInt32("3"), "13.000");
                yield return new TestCaseData("TestID-010N", 12.6111, Convert.ToUInt32("3"), "12.612");
                yield return new TestCaseData("TestID-011A", null, Convert.ToUInt32("2"), "0").Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-012N", 12.6111, Convert.ToUInt32("0"), "13");
                yield return new TestCaseData("TestID-013N", 12.6111, Convert.ToUInt32("7"), "12.6111000");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method Ceiling.
        /// </summary>
        public IEnumerable<TestCaseData> TestCeilingctTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", 12, Convert.ToUInt32("2"), 2, "12.00");
                yield return new TestCaseData("TestID-001N", 12.888, Convert.ToUInt32("2"), 1, "12.89");
                yield return new TestCaseData("TestID-002N", 5.61111, Convert.ToUInt32("2"), 2, "5.62");
                yield return new TestCaseData("TestID-003N", 10.135, Convert.ToUInt32("2"), 2, "10.14");
                yield return new TestCaseData("TestID-004N", 10.99999, Convert.ToUInt32("2"), 3, "11.00");
                yield return new TestCaseData("TestID-005L", string.Empty, Convert.ToUInt32("2"), 2, "0");
                yield return new TestCaseData("TestID-006N", 12.88, Convert.ToUInt32("3"), 1, "12.880");
                yield return new TestCaseData("TestID-007N", 12.888, Convert.ToUInt32("3"), 3, "12.888");
                yield return new TestCaseData("TestID-008N", 12.8888, Convert.ToUInt32("3"), 4, "12.889");
                yield return new TestCaseData("TestID-009N", 12.9999, Convert.ToUInt32("3"), 2, "13.000");
                yield return new TestCaseData("TestID-010N", 12.6111, Convert.ToUInt32("3"), 1, "12.612");
                yield return new TestCaseData("TestID-011A", null, Convert.ToUInt32("2"), 2, "0").Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-012N", 0.09m, Convert.ToUInt32("2"), 2, "0.09");
                yield return new TestCaseData("TestID-013N", Convert.ToUInt32(20), Convert.ToUInt32("2"), 2, "20.00");
                yield return new TestCaseData("TestID-014N", 12, Convert.ToUInt32("2"), CeilingToward.RI, "12.00");
                yield return new TestCaseData("TestID-015N", 12, Convert.ToUInt32("2"), CeilingToward.RP, "12.00");
                yield return new TestCaseData("TestID-016N", 0.09m, Convert.ToUInt32("2"), CeilingToward.RI, "0.09");
                yield return new TestCaseData("TestID-017N", 0.09m, Convert.ToUInt32("2"), CeilingToward.RP, "0.09");
                yield return new TestCaseData("TestID-018N", -0.09m, Convert.ToUInt32("3"), CeilingToward.RI, "-0.090");
                yield return new TestCaseData("TestID-019N", 12.6111, Convert.ToUInt32("0"), CeilingToward.RI, "13");
                yield return new TestCaseData("TestID-020N", 12.6111, Convert.ToUInt32("7"), CeilingToward.RI, "12.6111000");

            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method AddZerosAfterDecimal.
        /// </summary>
        public IEnumerable<TestCaseData> TestAddZerosAfterDecimalTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", Convert.ToDecimal(12.00), Convert.ToUInt32("2"), "12.00");
                yield return new TestCaseData("TestID-001N", Convert.ToDecimal(12.888), Convert.ToUInt32("2"), "12.888");
                yield return new TestCaseData("TestID-002N", 300.5m, Convert.ToUInt32("2"), "300.50");
                yield return new TestCaseData("TestID-003N", 10.135m, Convert.ToUInt32("2"), "10.135");
                yield return new TestCaseData("TestID-004N", 10.99999m, Convert.ToUInt32("2"), "10.99999");
                yield return new TestCaseData("TestID-005N", 12m, Convert.ToUInt32("0"), "12");
                yield return new TestCaseData("TestID-006A", string.Empty, Convert.ToUInt32("2"), "0").Throws(typeof(System.ArgumentException));
                yield return new TestCaseData("TestID-007N", 12.888m, Convert.ToUInt32("3"), "12.888");
                yield return new TestCaseData("TestID-008N", 12.8888m, Convert.ToUInt32("3"), "12.8888");
                yield return new TestCaseData("TestID-009N", 12.9999m, Convert.ToUInt32("3"), "12.9999");
                yield return new TestCaseData("TestID-010N", 12.6111m, Convert.ToUInt32("3"), "12.6111");
                yield return new TestCaseData("TestID-011N", null, Convert.ToUInt32("2"), "0.00");
                yield return new TestCaseData("TestID-012N", Convert.ToDecimal(1), Convert.ToUInt32("2"), "1.00");
                yield return new TestCaseData("TestID-013A", 12, Convert.ToUInt32("2"), "12.00").Throws(typeof(System.ArgumentException));
                yield return new TestCaseData("TestID-014N", 12m, Convert.ToUInt32("2"), "12.00");
                yield return new TestCaseData("TestID-015N", .09m, Convert.ToUInt32("2"), "0.09");
                yield return new TestCaseData("TestID-016N", .9m, Convert.ToUInt32("3"), "0.900");
                yield return new TestCaseData("TestID-018N", Convert.ToDecimal(null), Convert.ToUInt32("3"), "0.000");
                yield return new TestCaseData("TestID-019N", 12.6111m, Convert.ToUInt32("0"), "12.6111");
                yield return new TestCaseData("TestID-020N", 12.6111m, Convert.ToUInt32("7"), "12.6111000");
                yield return new TestCaseData("TestID-021N", 12.61m, Convert.ToUInt32("0"), "12.61");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method AddFigure3.
        /// </summary>
        public IEnumerable<TestCaseData> TestAddFigure3Test
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", 12, "12");
                yield return new TestCaseData("TestID-001N", 12.888, "12.888");
                yield return new TestCaseData("TestID-002N", 5.61111, "5.61111");
                yield return new TestCaseData("TestID-003N", 10.135, "10.135");
                yield return new TestCaseData("TestID-004N", 10.99999, "10.99999");
                yield return new TestCaseData("TestID-005L", string.Empty, "0");
                yield return new TestCaseData("TestID-006N", 12.88, "12.88");
                yield return new TestCaseData("TestID-007N", 12.888, "12.888");
                yield return new TestCaseData("TestID-008N", 12.8888, "12.8888");
                yield return new TestCaseData("TestID-009N", 12.9999, "12.9999");
                yield return new TestCaseData("TestID-010N", 12.6111, "12.6111");
                yield return new TestCaseData("TestID-011N", 0.09, "0.09");
                yield return new TestCaseData("TestID-012A", null, "0").Throws(typeof(NullReferenceException));
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method AddFigure4.
        /// </summary>
        public IEnumerable<TestCaseData> TestAddFigure4Test
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", 12, "12");
                yield return new TestCaseData("TestID-001N", 12.888, "12.888");
                yield return new TestCaseData("TestID-002N", 5.61111, "5.61111");
                yield return new TestCaseData("TestID-003N", 10.135, "10.135");
                yield return new TestCaseData("TestID-004N", 10.99999, "10.99999");
                yield return new TestCaseData("TestID-005L", string.Empty, "0");
                yield return new TestCaseData("TestID-006N", 12.88, "12.88");
                yield return new TestCaseData("TestID-007N", 12.888, "12.888");
                yield return new TestCaseData("TestID-008N", 12.8888, "12.8888");
                yield return new TestCaseData("TestID-009N", 12.9999, "12.9999");
                yield return new TestCaseData("TestID-010N", 12.6111, "12.6111");
                yield return new TestCaseData("TestID-011N", 0.09, "0.09");
                yield return new TestCaseData("TestID-012A", null, "0").Throws(typeof(NullReferenceException));
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method AddFigureX.
        /// </summary>
        public IEnumerable<TestCaseData> TestAddFigureXTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", -12.888m, 3, "-12.888");
                yield return new TestCaseData("TestID-001N", -12m, 2, "-12");
                yield return new TestCaseData("TestID-002N", "0.09", 2, "0.09");
                yield return new TestCaseData("TestID-003N", "9", 2, "9");
                yield return new TestCaseData("TestID-004N", "-0.09", 2, "-0.09");
                yield return new TestCaseData("TestID-005N", "abcd", 2, "0");
                yield return new TestCaseData("TestID-006N", 10, 2, "10");
                yield return new TestCaseData("TestID-007N", -0.09m, 2, "-0.09");
                yield return new TestCaseData("TestID-008N", 5.61111, 4, "5.61111");
                yield return new TestCaseData("TestID-009N", 10.135, 2, "10.135");
                yield return new TestCaseData("TestID-0010N", 10.99999, 5, "10.99999");
                yield return new TestCaseData("TestID-011N", 12.88, 2, "12.88");
                yield return new TestCaseData("TestID-012N", 12.888, 2, "12.888");
                yield return new TestCaseData("TestID-013N", 12.8888, 3, "12.8888");
                yield return new TestCaseData("TestID-014N", 12.9999, 2, "12.9999");
                yield return new TestCaseData("TestID-015N", 12.6111, 3, "12.6111");
                yield return new TestCaseData("TestID-016N", 0.09, 2, "0.09");
                yield return new TestCaseData("TestID-017N", 0.09m, 2, "0.09");
                yield return new TestCaseData("TestID-018L", string.Empty, 2, "0");
                yield return new TestCaseData("TestID-019N", "1.0", 2, "1.0");
                yield return new TestCaseData("TestID-020A", -12.888m, -3, "-12.888").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-021A", null, 2, "0").Throws(typeof(NullReferenceException));
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method AddFigureX.
        /// </summary>

        public IEnumerable<TestCaseData> TestSuppressTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", "", 2, '@', "@@");
                yield return new TestCaseData("TestID-001N", "123456789", 0, '@', "123456789");
                yield return new TestCaseData("TestID-002N", "123456789", 1, '@', "123456789");
                yield return new TestCaseData("TestID-003N", "123456789", 5, '@', "123456789");
                yield return new TestCaseData("TestID-004N", "123456789", 9, '@', "123456789");
                yield return new TestCaseData("TestID-005N", "123456789", 10, '@', "@123456789");
                yield return new TestCaseData("TestID-006N", "123456789", 11, '@', "@@123456789");
                yield return new TestCaseData("TestID-007N", "123456789", 20, '@', "@@@@@@@@@@@123456789");
                yield return new TestCaseData("TestID-008N", "", 1, '0', "0");
                yield return new TestCaseData("TestID-009N", "abcdefg", 0, '0', "abcdefg");
                yield return new TestCaseData("TestID-010N", "abcdefg", 1, '0', "abcdefg");
                yield return new TestCaseData("TestID-011N", "abcdefg", 8, '0', "0abcdefg");
                yield return new TestCaseData("TestID-012L", string.Empty, 2, '1', "11");
                yield return new TestCaseData("TestID-013A", null, 2, '1', "11").Throws(typeof(NullReferenceException));
                yield return new TestCaseData("TestID-012A", "1234", -2, '1', "1234").Throws(typeof(ArgumentOutOfRangeException));
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method SeirekiToWareki.
        /// </summary>
        public IEnumerable<TestCaseData> TestSeirekiToWarekiTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", DateTime.Parse("1977/4/24"), "ggy年M月d日（ddd");
                yield return new TestCaseData("TestID-001N", DateTime.Parse("1977/4/24"), "ggy年M月d日（ddd）H:m:s");
                yield return new TestCaseData("TestID-002N", DateTime.Parse("1977/4/24 19:15:12"), "ggy年M月d日（ddd）");
                yield return new TestCaseData("TestID-003N", DateTime.Parse("1977/4/24 19:15:12"), "ggy年M月d日（ddd）H:m:s");
                yield return new TestCaseData("TestID-004N", DateTime.Parse("1977/4/24 19:15:12"), "ggy年M月d日（ddd）tt h:m:s");
                yield return new TestCaseData("TestID-005N", DateTime.Parse("1992/2/6 1:1:1"), "ggyy年MM月dd日 dddd HH:mm:ss");
                yield return new TestCaseData("TestID-006N", DateTime.Parse("1992/2/6 13:1:1"), "ggyy年MM月dd日 dddd tt hh:mm:ss");
                yield return new TestCaseData("TestID-012A", string.Empty, "ggyy年MM月dd日 dddd tt hh:mm:ss").Throws(typeof(ArgumentException));
                yield return new TestCaseData("TestID-013A", null, "ggy年M月d日（ddd").Throws(typeof(ArgumentOutOfRangeException));
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method WarekiToSeireki.
        /// </summary>
        public IEnumerable<TestCaseData> TestWarekiToSeirekiTest
        {
            get
            {
                this.SetUp();

                yield return new TestCaseData("TestID-000N", "昭和52年4月24日（日）", "ggy年M月d日（ddd）");
                yield return new TestCaseData("TestID-001N", "昭和52年4月24日（日）19:15:12", "ggy年M月d日（ddd）H:m:s");
                yield return new TestCaseData("TestID-002N", "昭和52年4月24日（日）午後 7:15:12", "ggy年M月d日（ddd）tt h:m:s");
                yield return new TestCaseData("TestID-003N", "平成04年02月06日 木曜日 01:01:01", "ggyy年MM月dd日 dddd HH:mm:ss");
                yield return new TestCaseData("TestID-004N", "平成04年02月06日 木曜日 午後 01:01:01", "ggyy年MM月dd日 dddd tt hh:mm:ss");
                yield return new TestCaseData("TestID-05A", string.Empty, "ggyy年MM月dd日 dddd tt hh:mm:ss").Throws(typeof(FormatException));
                yield return new TestCaseData("TestID-06A", null, "ggy年M月d日（ddd").Throws(typeof(ArgumentNullException));
            }
        }

        #endregion

        /// <summary>
        /// TestRoundBankerTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="number">number</param>
        /// <param name="digitsAfterDecimalPoint">digitsAfterDecimalPoint</param>
        /// <param name="expected">expected</param>
        [TestCaseSource("TestRoundBankerTest")]
        public static void Round_BankerTest(string testCaseID, object number, int digitsAfterDecimalPoint, object expected)
        {
            object returnvalue = FormatConverter.Round_Banker(number, digitsAfterDecimalPoint);

            Assert.AreEqual(returnvalue, expected);
        }

        /// <summary>
        /// TestRound4sya5nyuTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="number">number</param>
        /// <param name="digitsAfterDecimalPoint">digitsAfterDecimalPoint</param>
        /// <param name="expected">expected</param>
        [TestCaseSource("TestRound4sya5nyuTest")]
        public static void Round_4sya5nyuTest(string testCaseID, object number, int digitsAfterDecimalPoint, object expected)
        {
            object returnvalue = FormatConverter.Round_4sya5nyu(number, digitsAfterDecimalPoint);

            Assert.AreEqual(returnvalue, expected);
        }

        /// <summary>
        /// FloorTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="number">number</param>
        /// <param name="digitsAfterDecimalPoint">digitsAfterDecimalPoint</param>
        /// <param name="expected">expected</param>
        [TestCaseSource("TestFloorTest")]
        public static void FloorTest(string testCaseID, object number, uint digitsAfterDecimalPoint, object expected)
        {
            object returnvalue = FormatConverter.Floor(number, digitsAfterDecimalPoint);

            Assert.AreEqual(returnvalue, expected);
        }

        /// <summary>
        /// FloorTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="number">number</param>
        /// <param name="digitsAfterDecimalPoint">digitsAfterDecimalPoint</param>
        /// <param name="ft">ft</param>
        /// <param name="expected">expected</param>
        [TestCaseSource("TestFloorftTest")]
        public static void FloorTest(string testCaseID, object number, uint digitsAfterDecimalPoint, FloorToward ft, object expected)
        {
            object returnvalue = FormatConverter.Floor(number, digitsAfterDecimalPoint, ft);

            Assert.AreEqual(returnvalue, expected);
        }

        /// <summary>
        /// CeilingTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="number">number</param>
        /// <param name="digitsAfterDecimalPoint">digitsAfterDecimalPoint</param>
        /// <param name="expected">expected</param>
        [TestCaseSource("TestCeilingTest")]
        public static void CeilingTest(string testCaseID, object number, uint digitsAfterDecimalPoint, object expected)
        {
            object returnvalue = FormatConverter.Ceiling(number, digitsAfterDecimalPoint);

            Assert.AreEqual(returnvalue, expected);
        }

        /// <summary>
        /// CeilingTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="number">number</param>
        /// <param name="digitsAfterDecimalPoint">digitsAfterDecimalPoint</param>
        /// <param name="ct">ct</param>
        /// <param name="expected">expected</param>
        [TestCaseSource("TestCeilingctTest")]
        public static void CeilingTest(string testCaseID, object number, uint digitsAfterDecimalPoint, CeilingToward ct, object expected)
        {
            object returnvalue = FormatConverter.Ceiling(number, digitsAfterDecimalPoint, ct);

            Assert.AreEqual(returnvalue, expected);
        }

        /// <summary>
        /// AddZerosAfterDecimalTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="dcm">dcm</param>
        /// <param name="digitsAfterDecimalPoint">digitsAfterDecimalPoint</param>
        /// <param name="expected">expected</param>
        [TestCaseSource("TestAddZerosAfterDecimalTest")]
        public static void AddZerosAfterDecimalTest(string testCaseID, decimal dcm, uint digitsAfterDecimalPoint, object expected)
        {
            object returnvalue = FormatConverter.AddZerosAfterDecimal(dcm, digitsAfterDecimalPoint);

            Assert.AreEqual(returnvalue, expected);
        }

        /// <summary>
        /// AddFigure3Test Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="number">number</param>
        /// <param name="expected">expected</param>
        [TestCaseSource("TestAddFigure3Test")]
        public static void AddFigure3Test(string testCaseID, object number, object expected)
        {
            object returnvalue = FormatConverter.AddFigure3(number);

            Assert.AreEqual(returnvalue, expected);
        }

        /// <summary>
        /// AddFigure4Test Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="number">number</param>
        /// <param name="expected">expected</param>
        [TestCaseSource("TestAddFigure4Test")]
        public static void AddFigure4Test(string testCaseID, object number, object expected)
        {
            object returnvalue = FormatConverter.AddFigure4(number);

            Assert.AreEqual(returnvalue, expected);
        }

        /// <summary>
        /// AddFigureXTest Method
        /// </summary>
        /// <param name="testCaseID">number</param>
        /// <param name="number">numberGroupSizes</param>
        /// <param name="numberGroupSizes">numberGroupSizes</param>
        /// <param name="expected">expected</param>
        [TestCaseSource("TestAddFigureXTest")]
        public static void AddFigureXTest(string testCaseID, object number, int numberGroupSizes, object expected)
        {
            object returnvalue = FormatConverter.AddFigureX(number, numberGroupSizes);

            Assert.AreEqual(returnvalue, expected);
        }

        /// <summary>
        /// SuppressTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="input">input</param>
        /// <param name="totalWidth">totalWidth</param>
        /// <param name="paddingChar">paddingChar</param>
        /// <param name="expected">expected</param>
        [TestCaseSource("TestSuppressTest")]
        public static void SuppressTest(string testCaseID, string input, int totalWidth, char paddingChar, string expected)
        {
            object returnvalue = FormatConverter.Suppress(input, totalWidth, paddingChar);

            Assert.AreEqual(returnvalue, expected);
        }

        /// <summary>
        /// SeirekiToWarekiTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="seireki">seireki</param>
        /// <param name="warekiPattern">warekiPattern</param>
        [TestCaseSource("TestSeirekiToWarekiTest")]
        public static void SeirekiToWarekiTest(string testCaseID, DateTime seireki, string warekiPattern)
        {
            string output = null;

            output = FormatConverter.SeirekiToWareki(seireki, warekiPattern);

            Assert.NotNull(output);
        }

        /// <summary>
        /// WarekiToSeirekiTest Method
        /// </summary>
        /// <param name="testCaseID">testCaseID</param>
        /// <param name="wareki">wareki</param>
        /// <param name="warekiPattern">warekiPattern</param>
        [TestCaseSource("TestWarekiToSeirekiTest")]
        public static void WarekiToSeirekiTest(string testCaseID, string wareki, string warekiPattern)
        {
            object output;

            output = FormatConverter.WarekiToSeireki(wareki, warekiPattern);

            Assert.IsInstanceOf(typeof(DateTime), output);
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
    }
}

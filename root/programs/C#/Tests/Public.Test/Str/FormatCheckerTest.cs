#region Includes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.Str;
#endregion
///////////////////////////////////////////////////////////////////////////////
// Copyright 2014 (c) by Symphony Services All Rights Reserved.
//  
// Project:      Infrastructure
// Module:       FormatCheckerTest.cs
// Description:  Tests for the Format Checker class in the Public assembly.
//  
// Date:               Author:           Comments:
// 5/12/2014 11:39 AM  Rituparna         Testcode development for FormatChecker.
///////////////////////////////////////////////////////////////////////////////
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
               
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "000-0000");
                yield return new TestCaseData("TestID-001N", "aaa-aaaa");
                yield return new TestCaseData("TestID-002L", string.Empty);


                yield return new TestCaseData("TestID-003N", "0000-0000");
                yield return new TestCaseData("TestID-004N", "000-00000");
                yield return new TestCaseData("TestID-005N", "00-000");
                yield return new TestCaseData("TestID-006N", "00-0000");
                yield return new TestCaseData("TestID-007N", "000-000");

                yield return new TestCaseData("TestID-008N", "000-00");
                yield return new TestCaseData("TestID-009N", "aaa-aa");
                yield return new TestCaseData("TestID-010N", "0000-000");
                yield return new TestCaseData("TestID-011N", "0000-00");
                yield return new TestCaseData("TestID-012N", "000-000");
                yield return new TestCaseData("TestID-013N", "00-0");
                yield return new TestCaseData("TestID-014N", "00-00");
                yield return new TestCaseData("TestID-015N", "000-0");

                yield return new TestCaseData("TestID-016N", "0000000");
                yield return new TestCaseData("TestID-017N", "aaaaaaa");
                yield return new TestCaseData("TestID-018N", "00000000");
                yield return new TestCaseData("TestID-019N", "000000");

                yield return new TestCaseData("TestID-020N", "00000");
                yield return new TestCaseData("TestID-021N", "aaaaa");
                yield return new TestCaseData("TestID-022N", "000000");
                yield return new TestCaseData("TestID-023N", "0000");

                yield return new TestCaseData("TestID-024N", "000");
                yield return new TestCaseData("TestID-025N", "aaa");
                yield return new TestCaseData("TestID-026N", "0000");
                yield return new TestCaseData("TestID-027N", "00");
                yield return new TestCaseData("TestID-028A", null).Throws(typeof(ArgumentNullException));
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
               
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "000-0000");
                yield return new TestCaseData("TestID-001N", "aaa-aaaa");
                yield return new TestCaseData("TestID-002L", string.Empty);


                yield return new TestCaseData("TestID-003N", "0000-0000");
                yield return new TestCaseData("TestID-004N", "000-00000");
                yield return new TestCaseData("TestID-005N", "00-000");
                yield return new TestCaseData("TestID-006N", "00-0000");
                yield return new TestCaseData("TestID-007N", "000-000");

                yield return new TestCaseData("TestID-008N", "000-00");
                yield return new TestCaseData("TestID-009N", "aaa-aa");
                yield return new TestCaseData("TestID-010N", "0000-000");
                yield return new TestCaseData("TestID-011N", "0000-00");
                yield return new TestCaseData("TestID-012N", "000-000");
                yield return new TestCaseData("TestID-013N", "00-0");
                yield return new TestCaseData("TestID-014N", "00-00");
                yield return new TestCaseData("TestID-015N", "000-0");

                yield return new TestCaseData("TestID-016N", "0000000");
                yield return new TestCaseData("TestID-017N", "aaaaaaa");
                yield return new TestCaseData("TestID-018N", "00000000");
                yield return new TestCaseData("TestID-019N", "000000");

                yield return new TestCaseData("TestID-020N", "00000");
                yield return new TestCaseData("TestID-021N", "aaaaa");
                yield return new TestCaseData("TestID-022N", "000000");
                yield return new TestCaseData("TestID-023N", "0000");

                yield return new TestCaseData("TestID-024N", "000");
                yield return new TestCaseData("TestID-025N", "aaa");
                yield return new TestCaseData("TestID-026N", "0000");
                yield return new TestCaseData("TestID-027N", "00");
                yield return new TestCaseData("TestID-028A", null).Throws(typeof(ArgumentNullException));
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
             
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "000-0000");
                yield return new TestCaseData("TestID-001N", "aaa-aaaa");
                yield return new TestCaseData("TestID-002L", string.Empty);


                yield return new TestCaseData("TestID-003N", "0000-0000");
                yield return new TestCaseData("TestID-004N", "000-00000");
                yield return new TestCaseData("TestID-005N", "00-000");
                yield return new TestCaseData("TestID-006N", "00-0000");
                yield return new TestCaseData("TestID-007N", "000-000");

                yield return new TestCaseData("TestID-008N", "000-00");
                yield return new TestCaseData("TestID-009N", "aaa-aa");
                yield return new TestCaseData("TestID-010N", "0000-000");
                yield return new TestCaseData("TestID-011N", "0000-00");
                yield return new TestCaseData("TestID-012N", "000-000");
                yield return new TestCaseData("TestID-013N", "00-0");
                yield return new TestCaseData("TestID-014N", "00-00");
                yield return new TestCaseData("TestID-015N", "000-0");

                yield return new TestCaseData("TestID-016N", "0000000");
                yield return new TestCaseData("TestID-017N", "aaaaaaa");
                yield return new TestCaseData("TestID-018N", "00000000");
                yield return new TestCaseData("TestID-019N", "000000");

                yield return new TestCaseData("TestID-020N", "00000");
                yield return new TestCaseData("TestID-021N", "aaaaa");
                yield return new TestCaseData("TestID-022N", "000000");
                yield return new TestCaseData("TestID-023N", "0000");

                yield return new TestCaseData("TestID-024N", "000");
                yield return new TestCaseData("TestID-025N", "aaa");
                yield return new TestCaseData("TestID-026N", "0000");
                yield return new TestCaseData("TestID-027N", "00");
                yield return new TestCaseData("TestID-028A", null).Throws(typeof(ArgumentNullException));
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
                           

                this.SetUp();


                yield return new TestCaseData("TestID-000N", "000-0000");
                yield return new TestCaseData("TestID-001N", "aaa-aaaa");
                yield return new TestCaseData("TestID-002L", string.Empty);


                yield return new TestCaseData("TestID-003N", "0000-0000");
                yield return new TestCaseData("TestID-004N", "000-00000");
                yield return new TestCaseData("TestID-005N", "00-000");
                yield return new TestCaseData("TestID-006N", "00-0000");
                yield return new TestCaseData("TestID-007N", "000-000");

                yield return new TestCaseData("TestID-008N", "000-00");
                yield return new TestCaseData("TestID-009N", "aaa-aa");
                yield return new TestCaseData("TestID-010N", "0000-000");
                yield return new TestCaseData("TestID-011N", "0000-00");
                yield return new TestCaseData("TestID-012N", "000-000");
                yield return new TestCaseData("TestID-013N", "00-0");
                yield return new TestCaseData("TestID-014N", "00-00");
                yield return new TestCaseData("TestID-015N", "000-0");

                yield return new TestCaseData("TestID-016N", "0000000");
                yield return new TestCaseData("TestID-017N", "aaaaaaa");
                yield return new TestCaseData("TestID-018N", "00000000");
                yield return new TestCaseData("TestID-019N", "000000");

                yield return new TestCaseData("TestID-020N", "00000");
                yield return new TestCaseData("TestID-021N", "aaaaa");
                yield return new TestCaseData("TestID-022N", "000000");
                yield return new TestCaseData("TestID-023N", "0000");

                yield return new TestCaseData("TestID-024N", "000");
                yield return new TestCaseData("TestID-025N", "aaa");
                yield return new TestCaseData("TestID-026N", "0000");
                yield return new TestCaseData("TestID-027N", "00");
                yield return new TestCaseData("TestID-028A", null).Throws(typeof(ArgumentNullException));
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
               
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "000-0000");
                yield return new TestCaseData("TestID-001N", "aaa-aaaa");
                yield return new TestCaseData("TestID-002L", string.Empty);


                yield return new TestCaseData("TestID-003N", "0000-0000");
                yield return new TestCaseData("TestID-004N", "000-00000");
                yield return new TestCaseData("TestID-005N", "00-000");
                yield return new TestCaseData("TestID-006N", "00-0000");
                yield return new TestCaseData("TestID-007N", "000-000");

                yield return new TestCaseData("TestID-008N", "000-00");
                yield return new TestCaseData("TestID-009N", "aaa-aa");
                yield return new TestCaseData("TestID-010N", "0000-000");
                yield return new TestCaseData("TestID-011N", "0000-00");
                yield return new TestCaseData("TestID-012N", "000-000");
                yield return new TestCaseData("TestID-013N", "00-0");
                yield return new TestCaseData("TestID-014N", "00-00");
                yield return new TestCaseData("TestID-015N", "000-0");

                yield return new TestCaseData("TestID-016N", "0000000");
                yield return new TestCaseData("TestID-017N", "aaaaaaa");
                yield return new TestCaseData("TestID-018N", "00000000");
                yield return new TestCaseData("TestID-019N", "000000");

                yield return new TestCaseData("TestID-020N", "00000");
                yield return new TestCaseData("TestID-021N", "aaaaa");
                yield return new TestCaseData("TestID-022N", "000000");
                yield return new TestCaseData("TestID-023N", "0000");

                yield return new TestCaseData("TestID-024N", "000");
                yield return new TestCaseData("TestID-025N", "aaa");
                yield return new TestCaseData("TestID-026N", "0000");
                yield return new TestCaseData("TestID-027N", "00");
                yield return new TestCaseData("TestID-028A", null).Throws(typeof(ArgumentNullException));
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
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "000-0000");
                yield return new TestCaseData("TestID-001N", "aaa-aaaa");
                yield return new TestCaseData("TestID-002L", string.Empty);


                yield return new TestCaseData("TestID-003N", "0000-0000");
                yield return new TestCaseData("TestID-004N", "000-00000");
                yield return new TestCaseData("TestID-005N", "00-000");
                yield return new TestCaseData("TestID-006N", "00-0000");
                yield return new TestCaseData("TestID-007N", "000-000");

                yield return new TestCaseData("TestID-008N", "000-00");
                yield return new TestCaseData("TestID-009N", "aaa-aa");
                yield return new TestCaseData("TestID-010N", "0000-000");
                yield return new TestCaseData("TestID-011N", "0000-00");
                yield return new TestCaseData("TestID-012N", "000-000");
                yield return new TestCaseData("TestID-013N", "00-0");
                yield return new TestCaseData("TestID-014N", "00-00");
                yield return new TestCaseData("TestID-015N", "000-0");

                yield return new TestCaseData("TestID-016N", "0000000");
                yield return new TestCaseData("TestID-017N", "aaaaaaa");
                yield return new TestCaseData("TestID-018N", "00000000");
                yield return new TestCaseData("TestID-019N", "000000");

                yield return new TestCaseData("TestID-020N", "00000");
                yield return new TestCaseData("TestID-021N", "aaaaa");
                yield return new TestCaseData("TestID-022N", "000000");
                yield return new TestCaseData("TestID-023N", "0000");

                yield return new TestCaseData("TestID-024N", "000");
                yield return new TestCaseData("TestID-025N", "aaa");
                yield return new TestCaseData("TestID-026N", "0000");
                yield return new TestCaseData("TestID-027N", "00");
                yield return new TestCaseData("TestID-028A", null).Throws(typeof(ArgumentNullException));
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
               
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "000-0000");
                yield return new TestCaseData("TestID-001N", "aaa-aaaa");
                yield return new TestCaseData("TestID-002L", string.Empty);


                yield return new TestCaseData("TestID-003N", "0000-0000");
                yield return new TestCaseData("TestID-004N", "000-00000");
                yield return new TestCaseData("TestID-005N", "00-000");
                yield return new TestCaseData("TestID-006N", "00-0000");
                yield return new TestCaseData("TestID-007N", "000-000");

                yield return new TestCaseData("TestID-008N", "000-00");
                yield return new TestCaseData("TestID-009N", "aaa-aa");
                yield return new TestCaseData("TestID-010N", "0000-000");
                yield return new TestCaseData("TestID-011N", "0000-00");
                yield return new TestCaseData("TestID-012N", "000-000");
                yield return new TestCaseData("TestID-013N", "00-0");
                yield return new TestCaseData("TestID-014N", "00-00");
                yield return new TestCaseData("TestID-015N", "000-0");

                yield return new TestCaseData("TestID-016N", "0000000");
                yield return new TestCaseData("TestID-017N", "aaaaaaa");
                yield return new TestCaseData("TestID-018N", "00000000");
                yield return new TestCaseData("TestID-019N", "000000");

                yield return new TestCaseData("TestID-020N", "00000");
                yield return new TestCaseData("TestID-021N", "aaaaa");
                yield return new TestCaseData("TestID-022N", "000000");
                yield return new TestCaseData("TestID-023N", "0000");

                yield return new TestCaseData("TestID-024N", "000");
                yield return new TestCaseData("TestID-025N", "aaa");
                yield return new TestCaseData("TestID-026N", "0000");
                yield return new TestCaseData("TestID-027N", "00");
                yield return new TestCaseData("TestID-028A", null).Throws(typeof(ArgumentNullException));
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
              
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "000-0000");
                yield return new TestCaseData("TestID-001N", "aaa-aaaa");
                yield return new TestCaseData("TestID-002L", string.Empty);


                yield return new TestCaseData("TestID-003N", "0000-0000");
                yield return new TestCaseData("TestID-004N", "000-00000");
                yield return new TestCaseData("TestID-005N", "00-000");
                yield return new TestCaseData("TestID-006N", "00-0000");
                yield return new TestCaseData("TestID-007N", "000-000");

                yield return new TestCaseData("TestID-008N", "000-00");
                yield return new TestCaseData("TestID-009N", "aaa-aa");
                yield return new TestCaseData("TestID-010N", "0000-000");
                yield return new TestCaseData("TestID-011N", "0000-00");
                yield return new TestCaseData("TestID-012N", "000-000");
                yield return new TestCaseData("TestID-013N", "00-0");
                yield return new TestCaseData("TestID-014N", "00-00");
                yield return new TestCaseData("TestID-015N", "000-0");

                yield return new TestCaseData("TestID-016N", "0000000");
                yield return new TestCaseData("TestID-017N", "aaaaaaa");
                yield return new TestCaseData("TestID-018N", "00000000");
                yield return new TestCaseData("TestID-019N", "000000");

                yield return new TestCaseData("TestID-020N", "00000");
                yield return new TestCaseData("TestID-021N", "aaaaa");
                yield return new TestCaseData("TestID-022N", "000000");
                yield return new TestCaseData("TestID-023N", "0000");

                yield return new TestCaseData("TestID-024N", "000");
                yield return new TestCaseData("TestID-025N", "aaa");
                yield return new TestCaseData("TestID-026N", "0000");
                yield return new TestCaseData("TestID-027N", "00");
                yield return new TestCaseData("TestID-028A", null).Throws(typeof(ArgumentNullException));
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
               
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "000-0000");
                yield return new TestCaseData("TestID-001N", "aaa-aaaa");
                yield return new TestCaseData("TestID-002L", string.Empty);


                yield return new TestCaseData("TestID-003N", "0000-0000");
                yield return new TestCaseData("TestID-004N", "000-00000");
                yield return new TestCaseData("TestID-005N", "00-000");
                yield return new TestCaseData("TestID-006N", "00-0000");
                yield return new TestCaseData("TestID-007N", "000-000");

                yield return new TestCaseData("TestID-008N", "000-00");
                yield return new TestCaseData("TestID-009N", "aaa-aa");
                yield return new TestCaseData("TestID-010N", "0000-000");
                yield return new TestCaseData("TestID-011N", "0000-00");
                yield return new TestCaseData("TestID-012N", "000-000");
                yield return new TestCaseData("TestID-013N", "00-0");
                yield return new TestCaseData("TestID-014N", "00-00");
                yield return new TestCaseData("TestID-015N", "000-0");

                yield return new TestCaseData("TestID-016N", "0000000");
                yield return new TestCaseData("TestID-017N", "aaaaaaa");
                yield return new TestCaseData("TestID-018N", "00000000");
                yield return new TestCaseData("TestID-019N", "000000");

                yield return new TestCaseData("TestID-020N", "00000");
                yield return new TestCaseData("TestID-021N", "aaaaa");
                yield return new TestCaseData("TestID-022N", "000000");
                yield return new TestCaseData("TestID-023N", "0000");

                yield return new TestCaseData("TestID-024N", "000");
                yield return new TestCaseData("TestID-025N", "aaa");
                yield return new TestCaseData("TestID-026N", "0000");
                yield return new TestCaseData("TestID-027N", "00");
                yield return new TestCaseData("TestID-028A", null).Throws(typeof(ArgumentNullException));
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
               
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "09999-9-9999");
                yield return new TestCaseData("TestID-001N", "99999-9-9999");
                yield return new TestCaseData("TestID-002N", "0aaaa-a-aaaa");
                yield return new TestCaseData("TestID-003N", "099999-9-9999");
                yield return new TestCaseData("TestID-004N", "09999-99-9999");
                yield return new TestCaseData("TestID-005N", "09999-9-99999");
                yield return new TestCaseData("TestID-006N", "0999--999");
                yield return new TestCaseData("TestID-007N", "0999-9-9999");
                yield return new TestCaseData("TestID-008N", "09999--9999");
                yield return new TestCaseData("TestID-009N", "09999-9-999");

                yield return new TestCaseData("TestID-010N", "0999-99-9999");
                yield return new TestCaseData("TestID-011N", "9999-99-9999");
                yield return new TestCaseData("TestID-012N", "0aaa-aa-aaaa");
                yield return new TestCaseData("TestID-013N", "09999-999-99999");
                yield return new TestCaseData("TestID-014N", "09999-99-9999");
                yield return new TestCaseData("TestID-015N", "0999-999-9999");
                yield return new TestCaseData("TestID-016N", "0999-99-99999");
                yield return new TestCaseData("TestID-017N", "099-9-999");
                yield return new TestCaseData("TestID-018N", "099-99-9999");
                yield return new TestCaseData("TestID-019N", "0999-9-9999");
                yield return new TestCaseData("TestID-020N", "0999-99-999");

                yield return new TestCaseData("TestID-021N", "099-999-9999");
                yield return new TestCaseData("TestID-022N", "999-999-9999");
                yield return new TestCaseData("TestID-023N", "0aa-aaa-aaaa");
                yield return new TestCaseData("TestID-024N", "0999-9999-99999");
                yield return new TestCaseData("TestID-025N", "0999-999-9999");
                yield return new TestCaseData("TestID-026N", "099-9999-9999");
                yield return new TestCaseData("TestID-027N", "099-999-99999");
                yield return new TestCaseData("TestID-028N", "09-99-999");
                yield return new TestCaseData("TestID-029N", "09-999-9999");
                yield return new TestCaseData("TestID-030N", "099-99-9999");
                yield return new TestCaseData("TestID-031N", "099-999-999");

                yield return new TestCaseData("TestID-032N", "09-9999-9999");
                yield return new TestCaseData("TestID-033N", "99-9999-9999");
                yield return new TestCaseData("TestID-034N", "0a-aaaa-aaaa");
                yield return new TestCaseData("TestID-035N", "099-99999-99999");
                yield return new TestCaseData("TestID-036N", "099-9999-9999");
                yield return new TestCaseData("TestID-037N", "09-99999-9999");
                yield return new TestCaseData("TestID-038N", "09-9999-99999");
                yield return new TestCaseData("TestID-039N", "0-999-999");
                yield return new TestCaseData("TestID-040N", "0-9999-9999");
                yield return new TestCaseData("TestID-041N", "09-999-9999");
                yield return new TestCaseData("TestID-042N", "09-9999-999");

                yield return new TestCaseData("TestID-043N", "999999999999");

                yield return new TestCaseData("TestID-044N", "0999999999");
                yield return new TestCaseData("TestID-045N", "9999999999");
                yield return new TestCaseData("TestID-046N", "0aaaaaaaaa");
                yield return new TestCaseData("TestID-047N", "09999999999");
                yield return new TestCaseData("TestID-048N", "099999999");

                yield return new TestCaseData("TestID-049N", "020-9999-9999");
                yield return new TestCaseData("TestID-050N", "929-9999-9999");
                yield return new TestCaseData("TestID-051N", "020-aaaa-aaaa");
                yield return new TestCaseData("TestID-052N", "0209-99999-99999");
                yield return new TestCaseData("TestID-053L", string.Empty);
                yield return new TestCaseData("TestID-054A", null).Throws(typeof(NullReferenceException));

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
               
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "09999-9-9999");
                yield return new TestCaseData("TestID-001N", "99999-9-9999");
                yield return new TestCaseData("TestID-002N", "0aaaa-a-aaaa");
                yield return new TestCaseData("TestID-003N", "099999-9-9999");
                yield return new TestCaseData("TestID-004N", "09999-99-9999");
                yield return new TestCaseData("TestID-005N", "09999-9-99999");
                yield return new TestCaseData("TestID-006N", "0999--999");
                yield return new TestCaseData("TestID-007N", "0999-9-9999");
                yield return new TestCaseData("TestID-008N", "09999--9999");
                yield return new TestCaseData("TestID-009N", "09999-9-999");

                yield return new TestCaseData("TestID-010N", "0999-99-9999");
                yield return new TestCaseData("TestID-011N", "9999-99-9999");
                yield return new TestCaseData("TestID-012N", "0aaa-aa-aaaa");
                yield return new TestCaseData("TestID-013N", "09999-999-99999");
                yield return new TestCaseData("TestID-014N", "09999-99-9999");
                yield return new TestCaseData("TestID-015N", "0999-999-9999");
                yield return new TestCaseData("TestID-016N", "0999-99-99999");
                yield return new TestCaseData("TestID-017N", "099-9-999");
                yield return new TestCaseData("TestID-018N", "099-99-9999");
                yield return new TestCaseData("TestID-019N", "0999-9-9999");
                yield return new TestCaseData("TestID-020N", "0999-99-999");

                yield return new TestCaseData("TestID-021N", "099-999-9999");
                yield return new TestCaseData("TestID-022N", "999-999-9999");
                yield return new TestCaseData("TestID-023N", "0aa-aaa-aaaa");
                yield return new TestCaseData("TestID-024N", "0999-9999-99999");
                yield return new TestCaseData("TestID-025N", "0999-999-9999");
                yield return new TestCaseData("TestID-026N", "099-9999-9999");
                yield return new TestCaseData("TestID-027N", "099-999-99999");
                yield return new TestCaseData("TestID-028N", "09-99-999");
                yield return new TestCaseData("TestID-029N", "09-999-9999");
                yield return new TestCaseData("TestID-030N", "099-99-9999");
                yield return new TestCaseData("TestID-031N", "099-999-999");

                yield return new TestCaseData("TestID-032N", "09-9999-9999");
                yield return new TestCaseData("TestID-033N", "99-9999-9999");
                yield return new TestCaseData("TestID-034N", "0a-aaaa-aaaa");
                yield return new TestCaseData("TestID-035N", "099-99999-99999");
                yield return new TestCaseData("TestID-036N", "099-9999-9999");
                yield return new TestCaseData("TestID-037N", "09-99999-9999");
                yield return new TestCaseData("TestID-038N", "09-9999-99999");
                yield return new TestCaseData("TestID-039N", "0-999-999");
                yield return new TestCaseData("TestID-040N", "0-9999-9999");
                yield return new TestCaseData("TestID-041N", "09-999-9999");
                yield return new TestCaseData("TestID-042N", "09-9999-999");

                yield return new TestCaseData("TestID-043N", "999999999999");

                yield return new TestCaseData("TestID-044N", "0999999999");
                yield return new TestCaseData("TestID-045N", "9999999999");
                yield return new TestCaseData("TestID-046N", "0aaaaaaaaa");
                yield return new TestCaseData("TestID-047N", "09999999999");
                yield return new TestCaseData("TestID-048N", "099999999");

                yield return new TestCaseData("TestID-049N", "020-9999-9999");
                yield return new TestCaseData("TestID-050N", "929-9999-9999");
                yield return new TestCaseData("TestID-051N", "020-aaaa-aaaa");
                yield return new TestCaseData("TestID-052N", "0209-99999-99999");
                yield return new TestCaseData("TestID-053L", string.Empty);
                yield return new TestCaseData("TestID-054A", null).Throws(typeof(NullReferenceException));

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
               
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "09999-9-9999");
                yield return new TestCaseData("TestID-001N", "99999-9-9999");
                yield return new TestCaseData("TestID-002N", "0aaaa-a-aaaa");
                yield return new TestCaseData("TestID-003N", "099999-9-9999");
                yield return new TestCaseData("TestID-004N", "09999-99-9999");
                yield return new TestCaseData("TestID-005N", "09999-9-99999");
                yield return new TestCaseData("TestID-006N", "0999--999");
                yield return new TestCaseData("TestID-007N", "0999-9-9999");
                yield return new TestCaseData("TestID-008N", "09999--9999");
                yield return new TestCaseData("TestID-009N", "09999-9-999");

                yield return new TestCaseData("TestID-010N", "0999-99-9999");
                yield return new TestCaseData("TestID-011N", "9999-99-9999");
                yield return new TestCaseData("TestID-012N", "0aaa-aa-aaaa");
                yield return new TestCaseData("TestID-013N", "09999-999-99999");
                yield return new TestCaseData("TestID-014N", "09999-99-9999");
                yield return new TestCaseData("TestID-015N", "0999-999-9999");
                yield return new TestCaseData("TestID-016N", "0999-99-99999");
                yield return new TestCaseData("TestID-017N", "099-9-999");
                yield return new TestCaseData("TestID-018N", "099-99-9999");
                yield return new TestCaseData("TestID-019N", "0999-9-9999");
                yield return new TestCaseData("TestID-020N", "0999-99-999");

                yield return new TestCaseData("TestID-021N", "099-999-9999");
                yield return new TestCaseData("TestID-022N", "999-999-9999");
                yield return new TestCaseData("TestID-023N", "0aa-aaa-aaaa");
                yield return new TestCaseData("TestID-024N", "0999-9999-99999");
                yield return new TestCaseData("TestID-025N", "0999-999-9999");
                yield return new TestCaseData("TestID-026N", "099-9999-9999");
                yield return new TestCaseData("TestID-027N", "099-999-99999");
                yield return new TestCaseData("TestID-028N", "09-99-999");
                yield return new TestCaseData("TestID-029N", "09-999-9999");
                yield return new TestCaseData("TestID-030N", "099-99-9999");
                yield return new TestCaseData("TestID-031N", "099-999-999");

                yield return new TestCaseData("TestID-032N", "09-9999-9999");
                yield return new TestCaseData("TestID-033N", "99-9999-9999");
                yield return new TestCaseData("TestID-034N", "0a-aaaa-aaaa");
                yield return new TestCaseData("TestID-035N", "099-99999-99999");
                yield return new TestCaseData("TestID-036N", "099-9999-9999");
                yield return new TestCaseData("TestID-037N", "09-99999-9999");
                yield return new TestCaseData("TestID-038N", "09-9999-99999");
                yield return new TestCaseData("TestID-039N", "0-999-999");
                yield return new TestCaseData("TestID-040N", "0-9999-9999");
                yield return new TestCaseData("TestID-041N", "09-999-9999");
                yield return new TestCaseData("TestID-042N", "09-9999-999");

                yield return new TestCaseData("TestID-043N", "999999999999");

                yield return new TestCaseData("TestID-044N", "0999999999");
                yield return new TestCaseData("TestID-045N", "9999999999");
                yield return new TestCaseData("TestID-046N", "0aaaaaaaaa");
                yield return new TestCaseData("TestID-047N", "09999999999");
                yield return new TestCaseData("TestID-048N", "099999999");

                yield return new TestCaseData("TestID-049N", "020-9999-9999");
                yield return new TestCaseData("TestID-050N", "929-9999-9999");
                yield return new TestCaseData("TestID-051N", "020-aaaa-aaaa");
                yield return new TestCaseData("TestID-052N", "0209-99999-99999");
                yield return new TestCaseData("TestID-053L", string.Empty);
                yield return new TestCaseData("TestID-054A", null).Throws(typeof(ArgumentNullException));

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
               
                this.SetUp();

                yield return new TestCaseData("TestID-000N", "09999-9-9999");
                yield return new TestCaseData("TestID-001N", "99999-9-9999");
                yield return new TestCaseData("TestID-002N", "0aaaa-a-aaaa");
                yield return new TestCaseData("TestID-003N", "099999-9-9999");
                yield return new TestCaseData("TestID-004N", "09999-99-9999");
                yield return new TestCaseData("TestID-005N", "09999-9-99999");
                yield return new TestCaseData("TestID-006N", "0999--999");
                yield return new TestCaseData("TestID-007N", "0999-9-9999");
                yield return new TestCaseData("TestID-008N", "09999--9999");
                yield return new TestCaseData("TestID-009N", "09999-9-999");

                yield return new TestCaseData("TestID-010N", "0999-99-9999");
                yield return new TestCaseData("TestID-011N", "9999-99-9999");
                yield return new TestCaseData("TestID-012N", "0aaa-aa-aaaa");
                yield return new TestCaseData("TestID-013N", "09999-999-99999");
                yield return new TestCaseData("TestID-014N", "09999-99-9999");
                yield return new TestCaseData("TestID-015N", "0999-999-9999");
                yield return new TestCaseData("TestID-016N", "0999-99-99999");
                yield return new TestCaseData("TestID-017N", "099-9-999");
                yield return new TestCaseData("TestID-018N", "099-99-9999");
                yield return new TestCaseData("TestID-019N", "0999-9-9999");
                yield return new TestCaseData("TestID-020N", "0999-99-999");

                yield return new TestCaseData("TestID-021N", "099-999-9999");
                yield return new TestCaseData("TestID-022N", "999-999-9999");
                yield return new TestCaseData("TestID-023N", "0aa-aaa-aaaa");
                yield return new TestCaseData("TestID-024N", "0999-9999-99999");
                yield return new TestCaseData("TestID-025N", "0999-999-9999");
                yield return new TestCaseData("TestID-026N", "099-9999-9999");
                yield return new TestCaseData("TestID-027N", "099-999-99999");
                yield return new TestCaseData("TestID-028N", "09-99-999");
                yield return new TestCaseData("TestID-029N", "09-999-9999");
                yield return new TestCaseData("TestID-030N", "099-99-9999");
                yield return new TestCaseData("TestID-031N", "099-999-999");

                yield return new TestCaseData("TestID-032N", "09-9999-9999");
                yield return new TestCaseData("TestID-033N", "99-9999-9999");
                yield return new TestCaseData("TestID-034N", "0a-aaaa-aaaa");
                yield return new TestCaseData("TestID-035N", "099-99999-99999");
                yield return new TestCaseData("TestID-036N", "099-9999-9999");
                yield return new TestCaseData("TestID-037N", "09-99999-9999");
                yield return new TestCaseData("TestID-038N", "09-9999-99999");
                yield return new TestCaseData("TestID-039N", "0-999-999");
                yield return new TestCaseData("TestID-040N", "0-9999-9999");
                yield return new TestCaseData("TestID-041N", "09-999-9999");
                yield return new TestCaseData("TestID-042N", "09-9999-999");

                yield return new TestCaseData("TestID-043N", "999999999999");

                yield return new TestCaseData("TestID-044N", "0999999999");
                yield return new TestCaseData("TestID-045N", "9999999999");
                yield return new TestCaseData("TestID-046N", "0aaaaaaaaa");
                yield return new TestCaseData("TestID-047N", "09999999999");
                yield return new TestCaseData("TestID-048N", "099999999");
                yield return new TestCaseData("TestID-049L", string.Empty);
                yield return new TestCaseData("TestID-050A", null).Throws(typeof(NullReferenceException));
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
                
                this.SetUp();

                yield return new TestCaseData("TestID-000N", "09999-9-9999");
                yield return new TestCaseData("TestID-001N", "99999-9-9999");
                yield return new TestCaseData("TestID-002N", "0aaaa-a-aaaa");
                yield return new TestCaseData("TestID-003N", "099999-9-9999");
                yield return new TestCaseData("TestID-004N", "09999-99-9999");
                yield return new TestCaseData("TestID-005N", "09999-9-99999");
                yield return new TestCaseData("TestID-006N", "0999--999");
                yield return new TestCaseData("TestID-007N", "0999-9-9999");
                yield return new TestCaseData("TestID-008N", "09999--9999");
                yield return new TestCaseData("TestID-009N", "09999-9-999");

                yield return new TestCaseData("TestID-010N", "0999-99-9999");
                yield return new TestCaseData("TestID-011N", "9999-99-9999");
                yield return new TestCaseData("TestID-012N", "0aaa-aa-aaaa");
                yield return new TestCaseData("TestID-013N", "09999-999-99999");
                yield return new TestCaseData("TestID-014N", "09999-99-9999");
                yield return new TestCaseData("TestID-015N", "0999-999-9999");
                yield return new TestCaseData("TestID-016N", "0999-99-99999");
                yield return new TestCaseData("TestID-017N", "099-9-999");
                yield return new TestCaseData("TestID-018N", "099-99-9999");
                yield return new TestCaseData("TestID-019N", "0999-9-9999");
                yield return new TestCaseData("TestID-020N", "0999-99-999");

                yield return new TestCaseData("TestID-021N", "099-999-9999");
                yield return new TestCaseData("TestID-022N", "999-999-9999");
                yield return new TestCaseData("TestID-023N", "0aa-aaa-aaaa");
                yield return new TestCaseData("TestID-024N", "0999-9999-99999");
                yield return new TestCaseData("TestID-025N", "0999-999-9999");
                yield return new TestCaseData("TestID-026N", "099-9999-9999");
                yield return new TestCaseData("TestID-027N", "099-999-99999");
                yield return new TestCaseData("TestID-028N", "09-99-999");
                yield return new TestCaseData("TestID-029N", "09-999-9999");
                yield return new TestCaseData("TestID-030N", "099-99-9999");
                yield return new TestCaseData("TestID-031N", "099-999-999");

                yield return new TestCaseData("TestID-032N", "09-9999-9999");
                yield return new TestCaseData("TestID-033N", "99-9999-9999");
                yield return new TestCaseData("TestID-034N", "0a-aaaa-aaaa");
                yield return new TestCaseData("TestID-035N", "099-99999-99999");
                yield return new TestCaseData("TestID-036N", "099-9999-9999");
                yield return new TestCaseData("TestID-037N", "09-99999-9999");
                yield return new TestCaseData("TestID-038N", "09-9999-99999");
                yield return new TestCaseData("TestID-039N", "0-999-999");
                yield return new TestCaseData("TestID-040N", "0-9999-9999");
                yield return new TestCaseData("TestID-041N", "09-999-9999");
                yield return new TestCaseData("TestID-042N", "09-9999-999");

                yield return new TestCaseData("TestID-043N", "999999999999");

                yield return new TestCaseData("TestID-044N", "0999999999");
                yield return new TestCaseData("TestID-045N", "9999999999");
                yield return new TestCaseData("TestID-046N", "0aaaaaaaaa");
                yield return new TestCaseData("TestID-047N", "09999999999");
                yield return new TestCaseData("TestID-048N", "099999999");
                yield return new TestCaseData("TestID-049N", "999999999999");
                yield return new TestCaseData("TestID-050L", string.Empty);
                yield return new TestCaseData("TestID-051A", null).Throws(typeof(NullReferenceException));
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
                // If you need to prepare for the test data below so, you use the TestCaseData.

               

                this.SetUp();

                yield return new TestCaseData("TestID-000N", "09999-9-9999");
                yield return new TestCaseData("TestID-001N", "99999-9-9999");
                yield return new TestCaseData("TestID-002N", "0aaaa-a-aaaa");
                yield return new TestCaseData("TestID-003N", "099999-9-9999");
                yield return new TestCaseData("TestID-004N", "09999-99-9999");
                yield return new TestCaseData("TestID-005N", "09999-9-99999");
                yield return new TestCaseData("TestID-006N", "0999--999");
                yield return new TestCaseData("TestID-007N", "0999-9-9999");
                yield return new TestCaseData("TestID-008N", "09999--9999");
                yield return new TestCaseData("TestID-009N", "09999-9-999");

                yield return new TestCaseData("TestID-010N", "0999-99-9999");
                yield return new TestCaseData("TestID-011N", "9999-99-9999");
                yield return new TestCaseData("TestID-012N", "0aaa-aa-aaaa");
                yield return new TestCaseData("TestID-013N", "09999-999-99999");
                yield return new TestCaseData("TestID-014N", "09999-99-9999");
                yield return new TestCaseData("TestID-015N", "0999-999-9999");
                yield return new TestCaseData("TestID-016N", "0999-99-99999");
                yield return new TestCaseData("TestID-017N", "099-9-999");
                yield return new TestCaseData("TestID-018N", "099-99-9999");
                yield return new TestCaseData("TestID-019N", "0999-9-9999");
                yield return new TestCaseData("TestID-020N", "0999-99-999");

                yield return new TestCaseData("TestID-021N", "099-999-9999");
                yield return new TestCaseData("TestID-022N", "999-999-9999");
                yield return new TestCaseData("TestID-023N", "0aa-aaa-aaaa");
                yield return new TestCaseData("TestID-024N", "0999-9999-99999");
                yield return new TestCaseData("TestID-025N", "0999-999-9999");
                yield return new TestCaseData("TestID-026N", "099-9999-9999");
                yield return new TestCaseData("TestID-027N", "099-999-99999");
                yield return new TestCaseData("TestID-028N", "09-99-999");
                yield return new TestCaseData("TestID-029N", "09-999-9999");
                yield return new TestCaseData("TestID-030N", "099-99-9999");
                yield return new TestCaseData("TestID-031N", "099-999-999");

                yield return new TestCaseData("TestID-032N", "09-9999-9999");
                yield return new TestCaseData("TestID-033N", "99-9999-9999");
                yield return new TestCaseData("TestID-034N", "0a-aaaa-aaaa");
                yield return new TestCaseData("TestID-035N", "099-99999-99999");
                yield return new TestCaseData("TestID-036N", "099-9999-9999");
                yield return new TestCaseData("TestID-037N", "09-99999-9999");
                yield return new TestCaseData("TestID-038N", "09-9999-99999");
                yield return new TestCaseData("TestID-039N", "0-999-999");
                yield return new TestCaseData("TestID-040N", "0-9999-9999");
                yield return new TestCaseData("TestID-041N", "09-999-9999");
                yield return new TestCaseData("TestID-042N", "09-9999-999");

                yield return new TestCaseData("TestID-043N", "999999999999");

                yield return new TestCaseData("TestID-044N", "0999999999");
                yield return new TestCaseData("TestID-045N", "9999999999");
                yield return new TestCaseData("TestID-046N", "0aaaaaaaaa");
                yield return new TestCaseData("TestID-047N", "09999999999");
                yield return new TestCaseData("TestID-048N", "099999999");
                yield return new TestCaseData("TestID-049N", "999999999999");
                yield return new TestCaseData("TestID-050L", string.Empty);
                yield return new TestCaseData("TestID-051A", null).Throws(typeof(ArgumentNullException));
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
                // If you need to prepare for the test data below so, you use the TestCaseData.

               

                this.SetUp();


                yield return new TestCaseData("TestID-000N", "020-9999-9999");
                yield return new TestCaseData("TestID-001N", "929-9999-9999");
                yield return new TestCaseData("TestID-002N", "020-aaaa-aaaa");
                yield return new TestCaseData("TestID-003N", "0209-99999-99999");
                yield return new TestCaseData("TestID-004N", "0209-99999-99999");
                yield return new TestCaseData("TestID-005N", "020-99999-9999");
                yield return new TestCaseData("TestID-006N", "020-99999-9999");
                yield return new TestCaseData("TestID-007N", "02-999-999");
                yield return new TestCaseData("TestID-008N", "02-9999-9999");
                yield return new TestCaseData("TestID-009N", "020-999-9999");
                yield return new TestCaseData("TestID-010N", "020-9999-999");
                yield return new TestCaseData("TestID-010N", "060-9999-9999");
                yield return new TestCaseData("TestID-011N", "969-9999-9999");
                yield return new TestCaseData("TestID-012N", "060-aaaa-aaaa");
                yield return new TestCaseData("TestID-013N", "0609-99999-99999");
                yield return new TestCaseData("TestID-014N", "0609-9999-9999");
                yield return new TestCaseData("TestID-015N", "060-99999-9999");
                yield return new TestCaseData("TestID-016N", "060-9999-99999");
                yield return new TestCaseData("TestID-017N", "06-999-999");
                yield return new TestCaseData("TestID-018N", "06-9999-9999");
                yield return new TestCaseData("TestID-019N", "060-999-9999");
                yield return new TestCaseData("TestID-020N", "060-9999-999");
                yield return new TestCaseData("TestID-021N", "070-9999-9999");
                yield return new TestCaseData("TestID-022N", "979-9999-9999");
                yield return new TestCaseData("TestID-023N", "070-aaaa-aaaa");
                yield return new TestCaseData("TestID-024N", "0709-99999-99999");
                yield return new TestCaseData("TestID-025N", "0709-9999-9999");
                yield return new TestCaseData("TestID-026N", "070-99999-9999");
                yield return new TestCaseData("TestID-027N", "070-9999-99999");
                yield return new TestCaseData("TestID-028N", "07-999-999");
                yield return new TestCaseData("TestID-029N", "07-9999-9999");
                yield return new TestCaseData("TestID-030N", "070-999-9999");
                yield return new TestCaseData("TestID-031N", "070-9999-999");
                yield return new TestCaseData("TestID-032N", "080-9999-9999");
                yield return new TestCaseData("TestID-033N", "989-9999-9999");
                yield return new TestCaseData("TestID-034N", "080-aaaa-aaaa");
                yield return new TestCaseData("TestID-035N", "0809-99999-99999");
                yield return new TestCaseData("TestID-036N", "0809-9999-9999");
                yield return new TestCaseData("TestID-037N", "080-99999-9999");
                yield return new TestCaseData("TestID-038N", "080-9999-99999");
                yield return new TestCaseData("TestID-039N", "08-999-999");
                yield return new TestCaseData("TestID-040N", "08-9999-9999");
                yield return new TestCaseData("TestID-041N", "080-999-9999");
                yield return new TestCaseData("TestID-042N", "080-9999-999");
                yield return new TestCaseData("TestID-043N", "090-9999-9999");
                yield return new TestCaseData("TestID-044N", "999-9999-9999");
                yield return new TestCaseData("TestID-045N", "090-aaaa-aaaa");
                yield return new TestCaseData("TestID-046N", "0909-99999-99999");
                yield return new TestCaseData("TestID-047N", "0909-9999-9999");
                yield return new TestCaseData("TestID-048N", "090-99999-9999");
                yield return new TestCaseData("TestID-049N", "02099999999");
                yield return new TestCaseData("TestID-050N", "92999999999");
                yield return new TestCaseData("TestID-051N", "020aaaaaaaa");
                yield return new TestCaseData("TestID-052N", "020999999999");
                yield return new TestCaseData("TestID-053N", "0209999999");
                yield return new TestCaseData("TestID-054N", "06099999999");
                yield return new TestCaseData("TestID-055N", "07099999999");
                yield return new TestCaseData("TestID-056N", "08099999999");
                yield return new TestCaseData("TestID-057N", "09099999999");
                yield return new TestCaseData("TestID-058L", string.Empty);
                yield return new TestCaseData("TestID-059A", null).Throws(typeof(ArgumentNullException));
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
               
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "020-9999-9999");
                yield return new TestCaseData("TestID-001N", "929-9999-9999");
                yield return new TestCaseData("TestID-002N", "020-aaaa-aaaa");
                yield return new TestCaseData("TestID-003N", "0209-99999-99999");
                yield return new TestCaseData("TestID-004N", "0209-99999-99999");
                yield return new TestCaseData("TestID-005N", "020-99999-9999");
                yield return new TestCaseData("TestID-006N", "020-99999-9999");
                yield return new TestCaseData("TestID-007N", "02-999-999");
                yield return new TestCaseData("TestID-008N", "02-9999-9999");
                yield return new TestCaseData("TestID-009N", "020-999-9999");
                yield return new TestCaseData("TestID-010N", "020-9999-999");
                yield return new TestCaseData("TestID-010N", "060-9999-9999");
                yield return new TestCaseData("TestID-011N", "969-9999-9999");
                yield return new TestCaseData("TestID-012N", "060-aaaa-aaaa");
                yield return new TestCaseData("TestID-013N", "0609-99999-99999");
                yield return new TestCaseData("TestID-014N", "0609-9999-9999");
                yield return new TestCaseData("TestID-015N", "060-99999-9999");
                yield return new TestCaseData("TestID-016N", "060-9999-99999");
                yield return new TestCaseData("TestID-017N", "06-999-999");
                yield return new TestCaseData("TestID-018N", "06-9999-9999");
                yield return new TestCaseData("TestID-019N", "060-999-9999");
                yield return new TestCaseData("TestID-020N", "060-9999-999");
                yield return new TestCaseData("TestID-021N", "070-9999-9999");
                yield return new TestCaseData("TestID-022N", "979-9999-9999");
                yield return new TestCaseData("TestID-023N", "070-aaaa-aaaa");
                yield return new TestCaseData("TestID-024N", "0709-99999-99999");
                yield return new TestCaseData("TestID-025N", "0709-9999-9999");
                yield return new TestCaseData("TestID-026N", "070-99999-9999");
                yield return new TestCaseData("TestID-027N", "070-9999-99999");
                yield return new TestCaseData("TestID-028N", "07-999-999");
                yield return new TestCaseData("TestID-029N", "07-9999-9999");
                yield return new TestCaseData("TestID-030N", "070-999-9999");
                yield return new TestCaseData("TestID-031N", "070-9999-999");
                yield return new TestCaseData("TestID-032N", "080-9999-9999");
                yield return new TestCaseData("TestID-033N", "989-9999-9999");
                yield return new TestCaseData("TestID-034N", "080-aaaa-aaaa");
                yield return new TestCaseData("TestID-035N", "0809-99999-99999");
                yield return new TestCaseData("TestID-036N", "0809-9999-9999");
                yield return new TestCaseData("TestID-037N", "080-99999-9999");
                yield return new TestCaseData("TestID-038N", "080-9999-99999");
                yield return new TestCaseData("TestID-039N", "08-999-999");
                yield return new TestCaseData("TestID-040N", "08-9999-9999");
                yield return new TestCaseData("TestID-041N", "080-999-9999");
                yield return new TestCaseData("TestID-042N", "080-9999-999");
                yield return new TestCaseData("TestID-043N", "090-9999-9999");
                yield return new TestCaseData("TestID-044N", "999-9999-9999");
                yield return new TestCaseData("TestID-045N", "090-aaaa-aaaa");
                yield return new TestCaseData("TestID-046N", "0909-99999-99999");
                yield return new TestCaseData("TestID-047N", "0909-9999-9999");
                yield return new TestCaseData("TestID-048N", "090-99999-9999");
                yield return new TestCaseData("TestID-049N", "02099999999");
                yield return new TestCaseData("TestID-050N", "92999999999");
                yield return new TestCaseData("TestID-051N", "020aaaaaaaa");
                yield return new TestCaseData("TestID-052N", "020999999999");
                yield return new TestCaseData("TestID-053N", "0209999999");
                yield return new TestCaseData("TestID-054N", "06099999999");
                yield return new TestCaseData("TestID-055N", "07099999999");
                yield return new TestCaseData("TestID-056N", "08099999999");
                yield return new TestCaseData("TestID-057N", "09099999999");
                yield return new TestCaseData("TestID-058L", string.Empty);
                yield return new TestCaseData("TestID-059A", null).Throws(typeof(ArgumentNullException));
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
               
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "020aaaaaaaa");
                yield return new TestCaseData("TestID-001N", "92999999999");
                yield return new TestCaseData("TestID-002N", "020-aaaa-aaaa");
                yield return new TestCaseData("TestID-003N", "020aaaaaaaa");
                yield return new TestCaseData("TestID-004N", "0209-99999-99999");
                yield return new TestCaseData("TestID-005N", "020-99999-9999");
                yield return new TestCaseData("TestID-006N", "020-99999-9999");
                yield return new TestCaseData("TestID-007N", "02-999-999");
                yield return new TestCaseData("TestID-008N", "02-9999-9999");
                yield return new TestCaseData("TestID-009N", "020-999-9999");
                yield return new TestCaseData("TestID-010N", "020-9999-999");
                yield return new TestCaseData("TestID-010N", "060-9999-9999");
                yield return new TestCaseData("TestID-011N", "969-9999-9999");
                yield return new TestCaseData("TestID-012N", "060-aaaa-aaaa");
                yield return new TestCaseData("TestID-013N", "0609-99999-99999");
                yield return new TestCaseData("TestID-014N", "0609-9999-9999");
                yield return new TestCaseData("TestID-015N", "060-99999-9999");
                yield return new TestCaseData("TestID-016N", "060-9999-99999");
                yield return new TestCaseData("TestID-017N", "06-999-999");
                yield return new TestCaseData("TestID-018N", "06-9999-9999");
                yield return new TestCaseData("TestID-019N", "060-999-9999");
                yield return new TestCaseData("TestID-020N", "060-9999-999");
                yield return new TestCaseData("TestID-021N", "070-9999-9999");
                yield return new TestCaseData("TestID-022N", "979-9999-9999");
                yield return new TestCaseData("TestID-023N", "070-aaaa-aaaa");
                yield return new TestCaseData("TestID-024N", "0709-99999-99999");
                yield return new TestCaseData("TestID-025N", "0709-9999-9999");
                yield return new TestCaseData("TestID-026N", "070-99999-9999");
                yield return new TestCaseData("TestID-027N", "070-9999-99999");
                yield return new TestCaseData("TestID-028N", "07-999-999");
                yield return new TestCaseData("TestID-029N", "07-9999-9999");
                yield return new TestCaseData("TestID-030N", "070-999-9999");
                yield return new TestCaseData("TestID-031N", "070-9999-999");
                yield return new TestCaseData("TestID-032N", "080-9999-9999");
                yield return new TestCaseData("TestID-033N", "989-9999-9999");
                yield return new TestCaseData("TestID-034N", "080-aaaa-aaaa");
                yield return new TestCaseData("TestID-035N", "0809-99999-99999");
                yield return new TestCaseData("TestID-036N", "0809-9999-9999");
                yield return new TestCaseData("TestID-037N", "080-99999-9999");
                yield return new TestCaseData("TestID-038N", "080-9999-99999");
                yield return new TestCaseData("TestID-039N", "020999999999");
                yield return new TestCaseData("TestID-040N", "0209999999");
                yield return new TestCaseData("TestID-041N", "06099999999");
                yield return new TestCaseData("TestID-042N", "07099999999");
                yield return new TestCaseData("TestID-043N", "08099999999");
                yield return new TestCaseData("TestID-044N", "09099999999");
                yield return new TestCaseData("TestID-045N", "0109999999");
                yield return new TestCaseData("TestID-046N", "03099999999");
                yield return new TestCaseData("TestID-047N", "04099999999");
                yield return new TestCaseData("TestID-048N", "05099999999");
                yield return new TestCaseData("TestID-049N", "02099999999");
                yield return new TestCaseData("TestID-050N", "92999999999");
                yield return new TestCaseData("TestID-051N", "020aaaaaaaa");
                yield return new TestCaseData("TestID-052N", "020999999999");
                yield return new TestCaseData("TestID-053N", "0209999999");
                yield return new TestCaseData("TestID-054N", "06099999999");
                yield return new TestCaseData("TestID-055N", "07099999999");
                yield return new TestCaseData("TestID-056N", "08099999999");
                yield return new TestCaseData("TestID-057N", "09099999999");
                yield return new TestCaseData("TestID-058L", string.Empty);
                yield return new TestCaseData("TestID-059A", null).Throws(typeof(ArgumentNullException));
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
                
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "050-9999-9999");
                yield return new TestCaseData("TestID-001N", "959-9999-9999");
                yield return new TestCaseData("TestID-002N", "050-aaaa-aaaa");
                yield return new TestCaseData("TestID-003N", "0509-99999-99999");
                yield return new TestCaseData("TestID-004N", "0509-9999-9999");
                yield return new TestCaseData("TestID-005N", "050-99999-9999");
                yield return new TestCaseData("TestID-006N", "050-9999-99999");
                yield return new TestCaseData("TestID-007N", "05-999-999");
                yield return new TestCaseData("TestID-008N", "05-9999-9999");
                yield return new TestCaseData("TestID-009N", "050-999-9999");
                yield return new TestCaseData("TestID-010N", "050-9999-999");

                yield return new TestCaseData("TestID-011N", "9999999999999");
                yield return new TestCaseData("TestID-012N", "010-9999-9999");
                yield return new TestCaseData("TestID-013N", "020-9999-9999");
                yield return new TestCaseData("TestID-014N", "030-9999-9999");
                yield return new TestCaseData("TestID-015N", "040-9999-9999");
                yield return new TestCaseData("TestID-016N", "060-9999-9999");
                yield return new TestCaseData("TestID-017N", "070-9999-9999");
                yield return new TestCaseData("TestID-018N", "080-9999-9999");
                yield return new TestCaseData("TestID-019N", "090-9999-9999");

                yield return new TestCaseData("TestID-020N", "05099999999");
                yield return new TestCaseData("TestID-021N", "95999999999");
                yield return new TestCaseData("TestID-022N", "050aaaaaaaa");
                yield return new TestCaseData("TestID-023N", "050999999999");
                yield return new TestCaseData("TestID-024N", "0509999999");

                yield return new TestCaseData("TestID-025N", "01099999999");
                yield return new TestCaseData("TestID-026N", "02099999999");
                yield return new TestCaseData("TestID-027N", "03099999999");
                yield return new TestCaseData("TestID-028N", "04099999999");
                yield return new TestCaseData("TestID-029N", "06099999999");
                yield return new TestCaseData("TestID-030N", "07099999999");
                yield return new TestCaseData("TestID-031N", "08099999999");
                yield return new TestCaseData("TestID-032N", "09099999999");
                yield return new TestCaseData("TestID-033L", string.Empty);
                yield return new TestCaseData("TestID-034A", null).Throws(typeof(ArgumentNullException));
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
                
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "050-9999-9999");
                yield return new TestCaseData("TestID-001N", "959-9999-9999");
                yield return new TestCaseData("TestID-002N", "050-aaaa-aaaa");
                yield return new TestCaseData("TestID-003N", "0509-99999-99999");
                yield return new TestCaseData("TestID-004N", "0509-9999-9999");
                yield return new TestCaseData("TestID-005N", "050-99999-9999");
                yield return new TestCaseData("TestID-006N", "050-9999-99999");
                yield return new TestCaseData("TestID-007N", "05-999-999");
                yield return new TestCaseData("TestID-008N", "05-9999-9999");
                yield return new TestCaseData("TestID-009N", "050-999-9999");
                yield return new TestCaseData("TestID-010N", "050-9999-999");

                yield return new TestCaseData("TestID-011N", "9999999999999");
                yield return new TestCaseData("TestID-012N", "010-9999-9999");
                yield return new TestCaseData("TestID-013N", "020-9999-9999");
                yield return new TestCaseData("TestID-014N", "030-9999-9999");
                yield return new TestCaseData("TestID-015N", "040-9999-9999");
                yield return new TestCaseData("TestID-016N", "060-9999-9999");
                yield return new TestCaseData("TestID-017N", "070-9999-9999");
                yield return new TestCaseData("TestID-018N", "080-9999-9999");
                yield return new TestCaseData("TestID-019N", "090-9999-9999");
                yield return new TestCaseData("TestID-020N", "95-999999999");

                yield return new TestCaseData("TestID-021N", "95999999999");
                yield return new TestCaseData("TestID-022N", "050aaaaaaaa");
                yield return new TestCaseData("TestID-023N", "050999999999");
                yield return new TestCaseData("TestID-024N", "0509999999");

                yield return new TestCaseData("TestID-025N", "01099999999");
                yield return new TestCaseData("TestID-026N", "02099999999");
                yield return new TestCaseData("TestID-027N", "03099999999");
                yield return new TestCaseData("TestID-028N", "04099999999");
                yield return new TestCaseData("TestID-029N", "06099999999");
                yield return new TestCaseData("TestID-030N", "07099999999");
                yield return new TestCaseData("TestID-031N", "08099999999");
                yield return new TestCaseData("TestID-032N", "09099999999");
                yield return new TestCaseData("TestID-033L", string.Empty);
                yield return new TestCaseData("TestID-034A", null).Throws(typeof(ArgumentNullException));
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
               
                this.SetUp();


                yield return new TestCaseData("TestID-000N", "06-999-999");
                yield return new TestCaseData("TestID-001N", "959-9999-9999");
                yield return new TestCaseData("TestID-002N", "050-aaaa-aaaa");
                yield return new TestCaseData("TestID-003N", "0509-99999-99999");
                yield return new TestCaseData("TestID-004N", "0509-9999-9999");
                yield return new TestCaseData("TestID-005N", "050-99999-9999");
                yield return new TestCaseData("TestID-006N", "050-9999-99999");
                yield return new TestCaseData("TestID-007N", "05-999-999");
                yield return new TestCaseData("TestID-008N", "05-9999-9999");
                yield return new TestCaseData("TestID-009N", "050-999-9999");
                yield return new TestCaseData("TestID-010N", "050-9999-999");

                yield return new TestCaseData("TestID-011N", "9999999999999");
                yield return new TestCaseData("TestID-012N", "010-9999-9999");
                yield return new TestCaseData("TestID-013N", "020-9999-9999");
                yield return new TestCaseData("TestID-014N", "030-9999-9999");
                yield return new TestCaseData("TestID-015N", "040-9999-9999");
                yield return new TestCaseData("TestID-016N", "060-9999-9999");
                yield return new TestCaseData("TestID-017N", "070-9999-9999");
                yield return new TestCaseData("TestID-018N", "080-9999-9999");
                yield return new TestCaseData("TestID-019N", "090-9999-9999");

                yield return new TestCaseData("TestID-020N", "05099999999");
                yield return new TestCaseData("TestID-021N", "95999999999");
                yield return new TestCaseData("TestID-022N", "050aaaaaaaa");
                yield return new TestCaseData("TestID-023N", "050999999999");
                yield return new TestCaseData("TestID-024N", "0509999999");

                yield return new TestCaseData("TestID-025N", "01099999999");
                yield return new TestCaseData("TestID-026N", "02099999999");
                yield return new TestCaseData("TestID-027N", "03099999999");
                yield return new TestCaseData("TestID-028N", "04099999999");
                yield return new TestCaseData("TestID-029N", "06099999999");
                yield return new TestCaseData("TestID-030N", "07099999999");
                yield return new TestCaseData("TestID-031N", "08099999999");
                yield return new TestCaseData("TestID-032N", "09099999999");
                yield return new TestCaseData("TestID-033L", string.Empty);
                yield return new TestCaseData("TestID-034A", null).Throws(typeof(ArgumentNullException));
            }

        }
        #endregion
        #region Test Code
        [TestCaseSource("TestIsJpZipCodeTest")]
        public static void IsJpZipCodeTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        [TestCaseSource("TestIsJpZipCodeHyphenTest")]
        public static void IsJpZipCode_HyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        [TestCaseSource("TestIsJpZipCodeNoHyphenTest")]
        public static void IsJpZipCode_NoHyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }
        [TestCaseSource("TestIsJpZipCode7Test")]
        public static void IsJpZipCode7Test(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        [TestCaseSource("TestIsJpZipCode7HyphenTest")]
        public static void IsJpZipCode7_HyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }


        [TestCaseSource("TestIsJpZipCode7NoHyphenTest")]
        public static void IsJpZipCode7_NoHyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }
        [TestCaseSource("TestIsJpZipCode5Test")]
        public static void IsJpZipCode5Test(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        [TestCaseSource("TestIsJpZipCode5HyphenTest")]
        public static void IsJpZipCode5_HyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        [TestCaseSource("TestIsJpZipCode5NoHyphenTest")]
        public static void IsJpZipCode5_NoHyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        [TestCaseSource("TestIsJpTelephoneNumberTest")]
        public static void IsJpTelephoneNumberTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        [TestCaseSource("TestIsJpTelephoneNumberHyphenTest")]
        public static void IsJpTelephoneNumber_HyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        [TestCaseSource("TestIsJpTelephoneNumberNoHyphenTest")]
        public static void IsJpTelephoneNumber_NoHyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        [TestCaseSource("TestIsJpFixedLinePhoneNumberTest")]
        public static void IsJpFixedLinePhoneNumberTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        [TestCaseSource("TestIsJpFixedLinePhoneNumberHyphenTest")]
        public static void IsJpFixedLinePhoneNumber_HyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }



        [TestCaseSource("TestIsJpFixedLinePhoneNumberNoHyphenTest")]
        public static void IsJpFixedLinePhoneNumber_NoHyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }


        [TestCaseSource("TestIsJpCellularPhoneNumberTest")]
        public static void IsJpCellularPhoneNumberTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }


        [TestCaseSource("TestIsJpCellularPhoneNumberHyphenTest")]
        public static void IsJpCellularPhoneNumber_HyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }



        [TestCaseSource("TestIsJpCellularPhoneNumberNoHyphenTest")]
        public static void IsJpCellularPhoneNumber_NoHyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }



        [TestCaseSource("TestIsJpIpPhoneNumberTest")]
        public static void IsJpIpPhoneNumberTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }



        [TestCaseSource("TestIsJpIpPhoneNumberHyphenTest")]
        public static void IsJpIpPhoneNumber_HyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }


        [TestCaseSource("TestIsJpIpPhoneNumberNoHyphenTest")]
        public static void IsJpIpPhoneNumber_NoHyphenTest(string testCaseID, string str)
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
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
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

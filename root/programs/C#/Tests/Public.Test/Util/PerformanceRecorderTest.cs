#region Includes
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.Util;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Data;
using System.Collections;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Win32;
#endregion

///////////////////////////////////////////////////////////////////////////////
// Project:      Infrastructure
// Module:       PerformanceRecorderTest.cs
// Description:  Tests for the Performance Recorder class in the Public assembly.
//  
// Date:       Author:           Comments:
// 5/22/2014 11:38 AM  sandukur     Module created.
///////////////////////////////////////////////////////////////////////////////
namespace Public.Test.Util
{

    /// <summary>
    /// Tests for the Performance Recorder Class
    /// </summary>
    [TestFixture, Description("Tests for Performance Recorder")]
    public class PerformanceRecorderTest
    {
        #region Class Variables
        private PerformanceRecorder _performanceRecorder;
        #endregion

        #region Setup/Teardown

        /// <summary>
        /// Code that is run once for a suite of tests
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {

        }

        /// <summary>
        /// Code that is run once after a suite of tests has finished executing
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {

        }

        /// <summary>
        /// Code that is run before each test
        /// </summary>
        [SetUp]
        public void Initialize()
        {
            //New instance of Performance Recorder
            _performanceRecorder = new PerformanceRecorder();
        }

        /// <summary>
        /// Code that is run after each test
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            
        }
        #endregion

        #region Test data
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method InspectTest.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfStartsPerformanceRecordTest
        {
            get
            {
                //Normal test case
                yield return new TestCaseData("TestID-001N");
            }
        }
        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method InspectTest.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfEndsPerformanceRecordTest
        {
            get
            {
                //Normal test case
                yield return new TestCaseData("TestID-001N");
            }
        }
        #endregion

        #region Method Tests

        #region GeneratedMethods

        /// <summary>
        /// Test method for StartsPerformanceRecord        
        /// </summary>
        [TestCaseSource("TestCasesOfStartsPerformanceRecordTest")]
        public void StartsPerformanceRecordTest(string testCaseID)
        {
            try
            {
                bool expected = true;

                bool results = _performanceRecorder.StartsPerformanceRecord();
                Assert.AreEqual(expected, results);
                Assert.AreNotEqual(false, results);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Test method for EndsPerformanceRecord     
        /// </summary>
        [TestCaseSource("TestCasesOfEndsPerformanceRecordTest")]
        public void EndsPerformanceRecordTest(string testCaseID)
        {
            try
            {
                string expected = _performanceRecorder.EndsPerformanceRecord();

                string results = _performanceRecorder.EndsPerformanceRecord();
                Assert.AreEqual(expected, results);
                Assert.AreNotEqual(string.Empty, results);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        #endregion // End of Test Methods

        #endregion

    }
}

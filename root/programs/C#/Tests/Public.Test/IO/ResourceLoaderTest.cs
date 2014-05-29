using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;

///////////////////////////////////////////////////////////////////////////////
// Copyright 2014 (c) by Symphony Services All Rights Reserved.
//  
// Project:      Infrastructure
// Module:       ResourceLoaderTest.cs
// Description:  Tests for the Resource Loader class in the Public assembly.
//  
// Date:       Author:           Comments:
// 5/8/2014 11:57 AM  insavaji     Module created.
///////////////////////////////////////////////////////////////////////////////

namespace Public.Test.IO
{
    /// <summary>
    /// Tests for the Resource Loader Class
    /// </summary>
    [TestFixture, Description("Tests for Resource Loader")]
   public class ResourceLoaderTest
    {
        #region Test code for ResourceLoader
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
            //New instance of Resource Loader
           
        }

        /// <summary>
        /// Code that is run after each test
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            //TODO:  Put dispose in here for _resourceLoader or delete this line
        }
        #endregion


        #region Methods

        /// <summary>
        /// Exists Method Test
        /// Documentation   :  å­˜åœ¨ãƒã‚§ãƒƒã‚¯ã®ã¿ã®ãƒ¡ã‚½ãƒƒãƒ‰
        /// Method Signature:  bool Exists(string loadfilepath, bool throwException)
        /// </summary>
        /// 
        [TestCase("TestID-001A", @"", true, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-002A", @"", false,ExpectedException = typeof(NUnit.Framework.AssertionException))]
        [TestCase("TestID-003A", null, null, ExpectedException = typeof(NUnit.Framework.AssertionException))]

        [TestCase("TestID-004N", @"C:\root\files\tools\DGenTemplates\ConditionalSearch.aspx", true)]
        [TestCase("TestID-005N", @"C:\root\files\tools\DGenTemplates\ConditionalSearch.aspx", false)]
        [TestCase("TestID-006A", @"F:\1234.txt", false,ExpectedException = typeof(NUnit.Framework.AssertionException))]
        [TestCase("TestID-007A", @"F:\1234.txt", true, ExpectedException = typeof(ArgumentNullException))]
        public void ExistsTest(string testCaseID, string loadfilepath, bool throwException)
        {
            try
            {
                   bool result = ResourceLoader.Exists(loadfilepath, throwException);
           
                    Assert.True(result);
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
                
            }
        }

        /// <summary>
        /// Exists Method Test
        /// Documentation   :  å­˜åœ¨ãƒã‚§ãƒƒã‚¯ã®ã¿ã®ãƒ¡ã‚½ãƒƒãƒ‰
        /// Method Signature:  bool Exists(string filePath, string fileName, bool throwException)
        /// </summary>
         [TestCase("TestID-001A", @"", "", true, ExpectedException = typeof(ArgumentNullException))]
         [TestCase("TestID-002N", @"","", false)]
         [TestCase("TestID-003A", null, null,true,ExpectedException=typeof(ArgumentNullException))]
         [TestCase("TestID-004A", null, null, false, ExpectedException = typeof(ArgumentNullException))]

         [TestCase("TestID-005N", @"C:\root\files\tools\DGenTemplates\", "ConditionalSearch.aspx", true)]
         [TestCase("TestID-006N", @"C:\root\files\tools\DGenTemplates\", "ConditionalSearch.aspx", false)]
         [TestCase("TestID-007N", @"E:\","123.txt" ,false)]
         [TestCase("TestID-008A", @"E:\", "123.txt", true, ExpectedException = typeof(ArgumentNullException))]
         [TestCase("TestID-009A", @"F", "123.txt", true, ExpectedException = typeof(ArgumentNullException))]
         public void ExistsTest1(string testCaseID, string filePath,string fileName,  bool throwException)
        {

            try
            {
                bool result = ResourceLoader.Exists(filePath, fileName,throwException);
                if (result)
                {
                    Assert.True(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;

            }
        
        }

        /// <summary>
        /// Load As String Method Test
        /// Documentation   :  [ãƒªã‚½ãƒ¼ã‚¹ ãƒ•ã‚¡ã‚¤ãƒ«]ã‹ã‚‰æ–‡å­—åˆ—ã‚’èª­ã¿è¾¼ã‚€ã€‚
        /// Method Signature:  string LoadAsString(string loadfilepath, Encoding enc)
        /// </summary>
         //[TestCase("TestID-001A", @"F:\123.txt")]
         //[TestCase("TestID-002A", @"F:\123d.txt",ExpectedException=typeof(ArgumentException))]
         //[TestCase("TestID-003A", null)]
        [TestCaseSource("TestcasesOfLoadStringTest")]
         public void LoadAsStringTest(string testCaseID, string loadfilepath, Encoding enc)
         {
             try
             {
                 string strResult = null;
                 strResult = ResourceLoader.LoadAsString(loadfilepath, enc);
                 Assert.NotNull(strResult);
             }
             catch (Exception ex)
             {

                 Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                 throw;
             }
         }

        /// <summary>
        /// Load As String Method Test
        /// Documentation   :  [ãƒªã‚½ãƒ¼ã‚¹ ãƒ•ã‚¡ã‚¤ãƒ«]ã‹ã‚‰æ–‡å­—åˆ—ã‚’èª­ã¿è¾¼ã‚€ã€‚
        /// Method Signature:  string LoadAsString(string filePath, string fileName, Encoding enc)
        /// </summary>
        [TestCaseSource("TestcasesOfLoadString1Test")]
        public void LoadAsString1Test(string testCaseID, string filePath, string fileName, Encoding enc)
        {
            try
            {
                string strResult = null;
                strResult = ResourceLoader.LoadAsString(filePath, fileName, enc);
                Assert.NotNull(strResult);
            }
            catch (Exception ex)
            {

                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        public IEnumerable<TestCaseData> TestcasesOfLoadStringTest
        {
            get
            {

                ///// Normal Test cases 
                yield return new TestCaseData("TestID-001N", @"C:\root\files\tools\DGenTemplates\ConditionalSearch.aspx", Encoding.UTF8);

                yield return new TestCaseData("TestID-002N", @"C:\root\files\tools\DGenTemplates\ConditionalSearch.aspx", Encoding.ASCII);

                yield return new TestCaseData("TestID-003N", @"C:\root\files\tools\DGenTemplates\ConditionalSearch.aspx", Encoding.UTF7);
                
                /// Abnormal Test cases
                yield return new TestCaseData("TestID-004A", null, Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-005A", @"C:\root\files\tools\DGenTemplates\ConditionalSearch.aspx", null).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-006A", @"F:\123a.txt", Encoding.UTF7).Throws(typeof(ArgumentNullException));

            }
        }

        public IEnumerable<TestCaseData> TestcasesOfLoadString1Test
        {
            get
            {
                ///// Normal Test cases C:\root\files\tools\DGenTemplates\ConditionalSearch.aspx
                yield return new TestCaseData("TestID-001N", @"C:\root\files\tools\DGenTemplates\", "ConditionalSearch.aspx", Encoding.UTF8);
                yield return new TestCaseData("TestID-002N", @"C:\root\files\tools\DGenTemplates\", "ConditionalSearch.aspx", Encoding.ASCII);
                yield return new TestCaseData("TestID-003N", @"C:\root\files\tools\DGenTemplates\", "ConditionalSearch.aspx", Encoding.UTF7);
                /// Abnormal Test cases
                yield return new TestCaseData("TestID-004A", @"F\", "ConditionalSearch.aspx", Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-005A", @"C:\root\files\tools\DGenTemplates\", "123a.txt", Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-006A", "", "123.txt", Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-007A", null, "123.txt", Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-008A", @"F\", null, Encoding.UTF8).Throws(typeof(ArgumentNullException));
            }
        }
        #endregion // End of GeneratedMethods

        #endregion

    }
}

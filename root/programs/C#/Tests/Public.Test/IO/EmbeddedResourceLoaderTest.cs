#region Includes
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.IO;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Data;
using System.Collections;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
#endregion

///////////////////////////////////////////////////////////////////////////////
// Copyright 2014 (c) by Symphony Services All Rights Reserved.
//  
// Project:      Infrastructure
// Module:       EmbeddedResourceLoaderTest.cs
// Description:  Tests for the Embedded Resource Loader class in the Public assembly.
//  
// Date:       Author:           Comments:
// 5/19/2014 11:57 AM  insavaji     Module created.
///////////////////////////////////////////////////////////////////////////////
namespace Public.Test.IO
{
    /// <summary>
    /// Tests for the Embedded Resource Loader Class
    /// Documentation: [åŸ‹ã‚è¾¼ã¾ã‚ŒãŸãƒªã‚½ãƒ¼ã‚¹ ãƒ•ã‚¡ã‚¤ãƒ«]ã®èª­ã¿è¾¼ã¿ã‚¯ãƒ©ã‚¹
    /// </summary>
    [TestFixture, Description("Tests for Embedded Resource Loader")]
    public class EmbeddedResourceLoaderTest
    {
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
        }

        /// <summary>
        /// Code that is run after each test
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            //TODO:  Put dispose in here for _embeddedResourceLoader or delete this line
        }
        #endregion

        #region Property Tests

        #region GeneratedProperties

        // No public properties were found. No tests are generated for non-public scoped properties.

        #endregion // End of GeneratedProperties

        #endregion

        #region Method Tests

        #region GeneratedMethods


        /// <summary>
        /// Exists Method Test
        /// Documentation   :  å­˜åœ¨ãƒã‚§ãƒƒã‚¯ã®ã¿ã®ãƒ¡ã‚½ãƒƒãƒ‰
        /// Method Signature:  bool Exists(string loadfileName, bool throwException)
        /// </summary>



        // Add the key to app.config file in Public.Test Solution:   <add key="Azure" value="Public"/>
        [TestCase("TestID-001N", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", false)]
        [TestCase("TestID-002N", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", true,ExpectedException=typeof(ArgumentNullException))]

        //Wrong File name
        [TestCase("TestID-003N", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource1.resources", false)]
        [TestCase("TestID-004A", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource1.resources", true,ExpectedException=typeof(ArgumentNullException))]
        [TestCase("TestID-005A", "", true, ExpectedException = typeof(ArgumentNullException))]
        [TestCase("TestID-006A", null, true, ExpectedException = typeof(ArgumentNullException))]

        public void ExistsTest(string testCaseID, string loadfileName, bool throwException)
        {

            try
            {
                bool results = EmbeddedResourceLoader.Exists(loadfileName, throwException);
                Assert.IsFalse(results);
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
        /// Method Signature:  bool Exists(string assemblyString, string loadfileName, bool throwException)
        /// </summary>
      [TestCaseSource("TestcasesOfExists1Test")]
        public void Exists1Test(string testCaseID, string assemblyString, string loadfileName, bool throwException)
        {
            try
            {
                bool results = EmbeddedResourceLoader.Exists(assemblyString, loadfileName, throwException);

                Assert.True(results);
               
            }
            catch (Exception ex)
            {
                
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }


        public IEnumerable<ITestCaseData> TestcasesOfExists1Test
        {
            get 
            {
                //Correct Assemblyname and resourcefile names 
                yield return new TestCaseData("TestID-001N","Public", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", false);
                yield return new TestCaseData("TestID-002N", "Public", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", true);

                // Empty assembly name and throw exception parameter false whcih return false.

                yield return new TestCaseData("TestID-003N", "", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", false).Throws(typeof(NUnit.Framework.AssertionException));

                // Empty assembly name and throw exception parameter true which throws exception.
                yield return new TestCaseData("TestID-004A", "", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", true).Throws(typeof(ArgumentNullException));

                //Throw exception parameter false and which inturn returns false
                yield return new TestCaseData("TestID-005A", null, "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", false).Throws(typeof(NUnit.Framework.AssertionException));
                yield return new TestCaseData("TestID-006A", null, null, false).Throws(typeof(NUnit.Framework.AssertionException));

                //Throw exception parameter true to throw exception
                yield return new TestCaseData("TestID-007A", null, "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", true).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-008A", null, null, true).Throws(typeof(ArgumentNullException));


            }
            
        }


        /// <summary>
        /// Load As String Method Test
        /// Documentation   :  [åŸ‹ã‚è¾¼ã¾ã‚ŒãŸãƒªã‚½ãƒ¼ã‚¹ ãƒ•ã‚¡ã‚¤ãƒ«]ã‹ã‚‰æ–‡å­—åˆ—ã‚’èª­ã¿è¾¼ã‚€ã€‚
        /// Method Signature:  string LoadAsString(string loadfileName, Encoding enc)
        /// </summary>
        [TestCaseSource("TestcasesOfLoadAsStringTest")]
        public void LoadAsStringTest(string testCaseID, string loadfileName, Encoding enc)
        {
            try
            {
                string results = EmbeddedResourceLoader.LoadAsString(loadfileName, enc);

                Assert.IsNotNullOrEmpty(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        public IEnumerable<TestCaseData> TestcasesOfLoadAsStringTest
        {
            get
            {
                yield return new TestCaseData("TestID-001N","Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", Encoding.ASCII).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-002A", "", Encoding.UTF32).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-003A", "", Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-004A", null, null).Throws(typeof(ArgumentNullException));
               
            }
        
        }




        /// <summary>
        /// Load As String Method Test
        /// Documentation   :  [åŸ‹ã‚è¾¼ã¾ã‚ŒãŸãƒªã‚½ãƒ¼ã‚¹ ãƒ•ã‚¡ã‚¤ãƒ«]ã‹ã‚‰æ–‡å­—åˆ—ã‚’èª­ã¿è¾¼ã‚€ã€‚
        /// Method Signature:  string LoadAsString(string assemblyString, string loadfileName, Encoding enc)
        /// </summary>
        [TestCaseSource("TestcasesOfLoadAsString1Test")]
        public void LoadAsString1Test(string testCaseID, string assemblyString, string loadfileName, Encoding enc)
        {

            try
            {
                string results = EmbeddedResourceLoader.LoadAsString(assemblyString, loadfileName, enc);

                Assert.IsNotNullOrEmpty(results);
            }
            catch (Exception ex)
            {
               Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }

        }


        public IEnumerable<TestCaseData> TestcasesOfLoadAsString1Test
        {
            get
            {

                //Normal test cases 
                yield return new TestCaseData("TestID-001N", "Public", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", Encoding.ASCII);
                yield return new TestCaseData("TestID-002N", "Public", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", Encoding.Default);
                yield return new TestCaseData("TestID-003N", "Framework", "Touryo.Infrastructure.Framework.Resources.FrameworkExceptionMessageResource.resources", Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-004N", "Framework", "Touryo.Infrastructure.Framework.Resources.FrameworkExceptionMessageResource.resources", Encoding.UTF7).Throws(typeof(ArgumentNullException));

                //Abnormal cases   Public.Test.Resource1.resources
                yield return new TestCaseData("TestID-005A", "Publics", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", Encoding.ASCII).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-006A", "Publics", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", Encoding.Default).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-007A", "Frameworks", "Touryo.Infrastructure.Public.Resources.FrameworkExceptionMessageResource.resources", Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-008A", "Frameworks", "Touryo.Infrastructure.Public.Resources.FrameworkExceptionMessageResource.resources", Encoding.UTF8).Throws(typeof(ArgumentNullException));
                // Wrong resource files name
                yield return new TestCaseData("TestID-009A", "Public", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource1.resources", Encoding.Default).Throws(typeof(ArgumentNullException));
                // resource file name as emtpy string
                yield return new TestCaseData("TestID-0010A", "Public", "", Encoding.Default).Throws(typeof(ArgumentNullException));

                //encoding as null
                yield return new TestCaseData("TestID-0011A", "Public", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", null).Throws(typeof(ArgumentNullException));



            }
        }




        /// <summary>
        /// Load XML As String Method Test
        /// Documentation   :  [åŸ‹ã‚è¾¼ã¾ã‚ŒãŸãƒªã‚½ãƒ¼ã‚¹XMLãƒ•ã‚¡ã‚¤ãƒ«]ã‹ã‚‰æ–‡å­—åˆ—ã‚’èª­ã¿è¾¼ã‚€ã€‚
        /// Method Signature:  string LoadXMLAsString(string loadfileName)
        /// </summary>
       [TestCaseSource("TestcasesOfLoadXMLAsStringTest")]
        public void LoadXMLAsStringTest(string testCaseID, string loadfileName)
        {
            try
            {

                string results = EmbeddedResourceLoader.LoadXMLAsString(loadfileName);
                Assert.IsNotNullOrEmpty(results);
            }
            catch (Exception ex)
            {

                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

       public IEnumerable<TestCaseData> TestcasesOfLoadXMLAsStringTest
       {
           get
           {
               yield return new TestCaseData("TestID-001N", "Public.Test.XMLLoadAsString.xml").Throws(typeof(ArgumentNullException)); ;
               yield return new TestCaseData("TestID-002N", "Public.Test.Load.xml").Throws(typeof(ArgumentNullException)); ;

               yield return new TestCaseData("TestID-003N", "Wrongfile.xml").Throws(typeof(ArgumentNullException)); ;
               yield return new TestCaseData("TestID-004N", "").Throws(typeof(ArgumentNullException)); ;
               yield return new TestCaseData("TestID-005N", null).Throws(typeof(ArgumentNullException)); ;
           }
       }


        /// <summary>
        /// Load XML As String Method Test
        /// Documentation   :  [åŸ‹ã‚è¾¼ã¾ã‚ŒãŸãƒªã‚½ãƒ¼ã‚¹XMLãƒ•ã‚¡ã‚¤ãƒ«]ã‹ã‚‰æ–‡å­—åˆ—ã‚’èª­ã¿è¾¼ã‚€ã€‚
        /// Method Signature:  string LoadXMLAsString(string assemblyString, string loadfileName)
        /// </summary>
        [TestCaseSource("TestcasesOfLoadXMLAsString1Test")]
        public void LoadXMLAsString1Test(string testCaseID, string assemblyString, string loadfileName)
        {
            try
            {

                string results = EmbeddedResourceLoader.LoadXMLAsString(assemblyString, loadfileName);
                Assert.IsNotNullOrEmpty(results);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }


        public IEnumerable<TestCaseData> TestcasesOfLoadXMLAsString1Test
        {
            get
            {
                yield return new TestCaseData("TestID-001N", "Public.Test", "Public.Test.XMLLoadAsString.xml");
                yield return new TestCaseData("TestID-002N", "Public.Test", "Public.Test.Load.xml");


                //pass empty string as assembly name 
                yield return new TestCaseData("TestID-003A", "", "Public.Test.XMLLoadAsString.xml").Throws(typeof(ArgumentNullException));
                //pass empty string as file name 
                yield return new TestCaseData("TestID-004A", "Public.Test", "").Throws(typeof(ArgumentNullException));


                //pass null value
                yield return new TestCaseData("TestID-005A", null, "Public.Test.XMLLoadAsString.xml").Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-006A", "Public.Test", null).Throws(typeof(ArgumentNullException));

                //Pass wrong file name
                yield return new TestCaseData("TestID-007A", "Public.Test", "Wrongfile.xml").Throws(typeof(ArgumentNullException));

                // pass file name which is not having any data inside
                yield return new TestCaseData("TestID-008A", "Public.Test", "Public.Test.Empty.xml").Throws(typeof(ArgumentNullException));
            }
        }


        #endregion // End of GeneratedMethods

        #endregion

    }
}

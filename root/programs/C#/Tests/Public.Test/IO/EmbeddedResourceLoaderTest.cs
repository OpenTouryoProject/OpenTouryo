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
//* クラス名        ：EmbeddedResourceLoaderTest
//* クラス日本語名  ：Test of the class to Load EmbeddedResource
//*
//* 作成者          ：Santosh Avaji
//* 更新履歴        ：
//* 
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/08/2014   Santosh Avaji      Testcode development for EmbeddedResourceLoaderTest.
//*
//**********************************************************************************

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

namespace Public.Test.IO
{
    /// <summary>
    /// Tests for the Embedded Resource Loader Class
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
        #region Method Tests

        #region GeneratedMethods
        ///<summary>Exists Method Test</summary> 
        ///<param name="testCaseID">test case ID</param> 
        ///<param name="loadfileName">file name to be loaded</param> 
        ///<param name="throwException">True to throw exception and false for not to throw exception </param> 
        // Add the key to app.config file in Public.Test Solution:   <add key="Azure" value="Public"/>
        [TestCase("TestID-001N", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", false)]
        [TestCase("TestID-002N", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", true, ExpectedException = typeof(ArgumentNullException))]
        //Wrong File name
        [TestCase("TestID-003N", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource1.resources", false)]
        [TestCase("TestID-004A", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource1.resources", true, ExpectedException = typeof(ArgumentNullException))]
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
        ///<summary>Exists Method Test</summary> 
        ///<param name="testCaseID">test case ID</param> 
        /// ///<param name="assemblyString">Assembly name</param> 
        ///<param name="loadfileName">file name to be loaded</param> 
        ///<param name="throwException">True to throw exception and false for not to throw exception </param> 
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
                yield return new TestCaseData("TestID-001N", "Public", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", false);
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
        ///<summary>LoadAsString Method Test</summary> 
        ///<param name="testCaseID">test case ID</param> 
        ///<param name="loadfileName">file name to be loaded</param> 
        ///<param name="enc">Type of Encoding</param> 
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
                yield return new TestCaseData("TestID-001N", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", Encoding.ASCII).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-002A", "", Encoding.UTF32).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-003A", "", Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-004A", null, null).Throws(typeof(ArgumentNullException));
            }
        }
        ///<summary>LoadAsString Method Test</summary> 
        ///<param name="testCaseID">test case ID</param> 
        /// ///<param name="assemblyString">Assembly name</param> 
        ///<param name="loadfileName">file name to be loaded</param> 
        ///<param name="enc">Type of Encoding</param> 
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

        ///<summary>LoadXMLAsString Method Test</summary> 
        ///<param name="testCaseID">test case ID</param> 
        ///<param name="loadfileName">file name to be loaded</param> 
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
                yield return new TestCaseData("TestID-004A", "").Throws(typeof(ArgumentNullException)); ;
                yield return new TestCaseData("TestID-005A", null).Throws(typeof(ArgumentNullException)); ;
            }
        }
        ///<summary>LoadXMLAsString Method Test</summary> 
        ///<param name="testCaseID">test case ID</param> 
        ///<param name="assemblyString">Assembly Name</param> 
        ///<param name="loadfileName">file name to be loaded</param> 
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

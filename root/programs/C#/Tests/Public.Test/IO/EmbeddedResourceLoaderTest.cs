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
//*  Date:        Author:          Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/08/2014   Santosh Avaji    Testcode development for EmbeddedResourceLoaderTest.
//*  08/12/2014   Sai              Added TestcaseID using SetName method as per Nishino-San comments
//*
//**********************************************************************************

#region Includes
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
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
        ///<param name="loadfileName">file name to be loaded</param> 
        ///<param name="throwException">True to throw exception and false for not to throw exception </param> 
        // Add the key to app.config file in Public.Test Solution:   <add key="Azure" value="Public"/>
        [TestCase("Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", false, TestName = "TestID-001N")]
        [TestCase("Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", true, ExpectedException = typeof(ArgumentException), TestName = "TestID-002N")]
        //Wrong File name
        [TestCase("Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource1.resources", false, TestName = "TestID-003N")]
        [TestCase("Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource1.resources", true, ExpectedException = typeof(ArgumentException), TestName = "TestID-004A")]
        [TestCase("", true, ExpectedException = typeof(ArgumentException), TestName = "TestID-005A")]
        [TestCase(null, true, ExpectedException = typeof(ArgumentException), TestName = "TestID-006A")]
        public void ExistsTest(string loadfileName, bool throwException)
        {
            try
            {
                bool results = EmbeddedResourceLoader.Exists(loadfileName, throwException);
                Assert.IsFalse(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
        ///<summary>Exists Method Test</summary> 
        /// ///<param name="assemblyString">Assembly name</param> 
        ///<param name="loadfileName">file name to be loaded</param> 
        ///<param name="throwException">True to throw exception and false for not to throw exception </param> 
        [TestCaseSource("TestcasesOfExists1Test")]
        public void Exists1Test(string assemblyString, string loadfileName, bool throwException)
        {
            try
            {
                bool results = EmbeddedResourceLoader.Exists(assemblyString, loadfileName, throwException);
                Assert.True(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
        public IEnumerable<ITestCaseData> TestcasesOfExists1Test
        {
            get
            {
                //Correct Assemblyname and resourcefile names 
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", false).SetName("TestID-001N");
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", true).SetName("TestID-002N");
                // Empty assembly name and throw exception parameter false whcih return false.
                yield return new TestCaseData("", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", false).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-003N");
                // Empty assembly name and throw exception parameter true which throws exception.
                yield return new TestCaseData("", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", true).Throws(typeof(ArgumentException)).SetName("TestID-004A");
                //Throw exception parameter false and which inturn returns false
                yield return new TestCaseData(null, "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", false).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-005A");
                yield return new TestCaseData(null, null, false).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-006A");
                //Throw exception parameter true to throw exception
                yield return new TestCaseData(null, "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", true).Throws(typeof(ArgumentException)).SetName("TestID-007A");
                yield return new TestCaseData(null, null, true).Throws(typeof(ArgumentException)).SetName("TestID-008A");
            }
        }
        ///<summary>LoadAsString Method Test</summary> 
        ///<param name="loadfileName">file name to be loaded</param> 
        ///<param name="enc">Type of Encoding</param> 
        [TestCaseSource("TestcasesOfLoadAsStringTest")]
        public void LoadAsStringTest(string loadfileName, Encoding enc)
        {
            try
            {
                string results = EmbeddedResourceLoader.LoadAsString(loadfileName, enc);

                Assert.IsNotNullOrEmpty(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
        public IEnumerable<TestCaseData> TestcasesOfLoadAsStringTest
        {
            get
            {
                yield return new TestCaseData("Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", Encoding.ASCII).Throws(typeof(ArgumentException)).SetName("TestID-001N");
                yield return new TestCaseData("", Encoding.UTF32).Throws(typeof(ArgumentException)).SetName("TestID-002A");
                yield return new TestCaseData("", Encoding.UTF8).Throws(typeof(ArgumentException)).SetName("TestID-003A");
                yield return new TestCaseData(null, null).Throws(typeof(ArgumentException)).SetName("TestID-004A");
            }
        }
        ///<summary>LoadAsString Method Test</summary> 
        /// ///<param name="assemblyString">Assembly name</param> 
        ///<param name="loadfileName">file name to be loaded</param> 
        ///<param name="enc">Type of Encoding</param> 
        [TestCaseSource("TestcasesOfLoadAsString1Test")]
        public void LoadAsString1Test(string assemblyString, string loadfileName, Encoding enc)
        {
            try
            {
                string results = EmbeddedResourceLoader.LoadAsString(assemblyString, loadfileName, enc);
                Assert.IsNotNullOrEmpty(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public IEnumerable<TestCaseData> TestcasesOfLoadAsString1Test
        {
            get
            {
                //Normal test cases 
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", Encoding.ASCII).SetName("TestID-001N");
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", Encoding.Default).SetName("TestID-002N");
                yield return new TestCaseData("Framework", "Touryo.Infrastructure.Framework.Resources.FrameworkExceptionMessageResource.resources", Encoding.UTF8).SetName("TestID-003N");
                yield return new TestCaseData("Framework", "Touryo.Infrastructure.Framework.Resources.FrameworkExceptionMessageResource.resources", Encoding.UTF7).SetName("TestID-004N");
                //Abnormal cases   Public.Test.Resource1.resources
                yield return new TestCaseData("Publics", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", Encoding.ASCII).Throws(typeof(ArgumentException)).SetName("TestID-005A");
                yield return new TestCaseData("Publics", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", Encoding.Default).Throws(typeof(ArgumentException)).SetName("TestID-006A");
                yield return new TestCaseData("Frameworks", "Touryo.Infrastructure.Public.Resources.FrameworkExceptionMessageResource.resources", Encoding.UTF8).Throws(typeof(ArgumentException)).SetName("TestID-007A");
                yield return new TestCaseData("Frameworks", "Touryo.Infrastructure.Public.Resources.FrameworkExceptionMessageResource.resources", Encoding.UTF8).Throws(typeof(ArgumentException)).SetName("TestID-008A");
                // Wrong resource files name
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource1.resources", Encoding.Default).Throws(typeof(ArgumentException)).SetName("TestID-009A");
                // resource file name as emtpy string
                yield return new TestCaseData("Public", "", Encoding.Default).Throws(typeof(ArgumentException)).SetName("TestID-0010A");
                //encoding as null
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource.resources", null).Throws(typeof(ArgumentNullException)).SetName("TestID-0011A");
            }
        }

        ///<summary>LoadXMLAsString Method Test</summary> 
        ///<param name="loadfileName">file name to be loaded</param> 
        [TestCaseSource("TestcasesOfLoadXMLAsStringTest")]
        public void LoadXMLAsStringTest(string loadfileName)
        {
            try
            {
                string results = EmbeddedResourceLoader.LoadXMLAsString(loadfileName);
                Assert.IsNotNullOrEmpty(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public IEnumerable<TestCaseData> TestcasesOfLoadXMLAsStringTest
        {
            get
            {
                yield return new TestCaseData("Public.Test.XMLLoadAsString.xml").Throws(typeof(ArgumentException)).SetName("TestID-001N");
                yield return new TestCaseData("Public.Test.Load.xml").Throws(typeof(ArgumentException)).SetName("TestID-002N");
                yield return new TestCaseData("Wrongfile.xml").Throws(typeof(ArgumentException)).SetName("TestID-003N");
                yield return new TestCaseData("").Throws(typeof(ArgumentException)).SetName("TestID-004A");
                yield return new TestCaseData(null).Throws(typeof(ArgumentException)).SetName("TestID-005A");
            }
        }
        ///<summary>LoadXMLAsString Method Test</summary> 
        ///<param name="assemblyString">Assembly Name</param> 
        ///<param name="loadfileName">file name to be loaded</param> 
        [TestCaseSource("TestcasesOfLoadXMLAsString1Test")]
        public void LoadXMLAsString1Test(string assemblyString, string loadfileName)
        {
            try
            {
                string results = EmbeddedResourceLoader.LoadXMLAsString(assemblyString, loadfileName);
                Assert.IsNotNullOrEmpty(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public IEnumerable<TestCaseData> TestcasesOfLoadXMLAsString1Test
        {
            get
            {
                yield return new TestCaseData("Public.Test", "Public.Test.XMLLoadAsString.xml").SetName("TestID-001N");
                yield return new TestCaseData("Public.Test", "Public.Test.Load.xml").SetName("TestID-002N");
                //pass empty string as assembly name 
                yield return new TestCaseData("", "Public.Test.XMLLoadAsString.xml").Throws(typeof(ArgumentException)).SetName("TestID-003A");
                //pass empty string as file name 
                yield return new TestCaseData("Public.Test", "").Throws(typeof(ArgumentException)).SetName("TestID-004A");
                //pass null value
                yield return new TestCaseData(null, "Public.Test.XMLLoadAsString.xml").Throws(typeof(ArgumentException)).SetName("TestID-005A");
                yield return new TestCaseData("Public.Test", null).Throws(typeof(ArgumentException)).SetName("TestID-006A");
                //Pass wrong file name
                yield return new TestCaseData("Public.Test", "Wrongfile.xml").Throws(typeof(ArgumentException)).SetName("TestID-007A");
                // pass file name which is not having any data inside
                yield return new TestCaseData("Public.Test", "Public.Test.Empty.xml").Throws(typeof(ArgumentException)).SetName("TestID-008A");
            }
        }
        #endregion // End of GeneratedMethods

        #endregion

    }
}

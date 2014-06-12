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
//* クラス名        ：ResourceLoaderTest.cs
//* クラス日本語名  ：Test of the class to Load Resource
//*
//* 作成者          ：Santosh Avaji
//* 更新履歴        ：
//* 
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/08/2014   Santosh Avaji      Testcode development for ResourceLoader.
//*
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
using System.IO;
using System.Reflection;
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
        ///<summary>Exists Method Test</summary> 
        ///<param name="testCaseID">test case ID</param> 
        ///<param name="loadfilepath">Path of the file to load</param> 
        ///<param name="throwException">Throw Exception or not</param> 
        [TestCaseSource("TestcasesOfExistTest")]
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
        public IEnumerable<TestCaseData> TestcasesOfExistTest
        {
            get
            {
                yield return new TestCaseData("TestID-001A", @"", true).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-002A", @"", false).Throws(typeof(NUnit.Framework.AssertionException));
                yield return new TestCaseData("TestID-003A", null, null).Throws(typeof(NUnit.Framework.AssertionException));
                yield return new TestCaseData("TestID-004N", MakeRelativePathFile() + "ConditionalSearch.aspx", true);
                yield return new TestCaseData("TestID-005N", MakeRelativePathFile() + "ConditionalSearch.aspx", false);
                yield return new TestCaseData("TestID-006A", MakeRelativePathFile() + "1234.txt", false).Throws(typeof(NUnit.Framework.AssertionException));
                yield return new TestCaseData("TestID-007A", MakeRelativePathFile() + "1234.txt", true).Throws(typeof(ArgumentNullException)); ;
            }
        }

        ///<summary>Exists Method Test</summary> 
        ///<param name="testCaseID">test case ID</param> 
        ///<param name="loadfilepath">Path of the file to load</param> 
        ///<param name="fileName">File name </param> 
        ///<param name="throwException">Throw Exception or not</param> 
        [TestCaseSource("TestcasesOfExist1Test")]
        public void ExistsTest1(string testCaseID, string filePath, string fileName, bool throwException)
        {
            try
            {
                bool result = ResourceLoader.Exists(filePath, fileName, throwException);
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

        public IEnumerable<TestCaseData> TestcasesOfExist1Test
        {
            get
            {
                yield return new TestCaseData("TestID-001A", @"", "", true).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-002N", @"", "", false);
                yield return new TestCaseData("TestID-003A", null, null, true).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-004A", null, null, false).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-005N", MakeRelativePathFile(), "ConditionalSearch.aspx", true);
                yield return new TestCaseData("TestID-006N", MakeRelativePathFile(), "ConditionalSearch.aspx", false);
                yield return new TestCaseData("TestID-007N", MakeRelativePathFile(), @"\123.txt", false);
                yield return new TestCaseData("TestID-008A", MakeRelativePathFile() + @"/", "123.txt", true).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-009A", @"F", "123.txt", true).Throws(typeof(ArgumentNullException));
            }
        }

        ///<summary>LoadAsString Method Test</summary> 
        ///<param name="testCaseID">test case ID</param> 
        ///<param name="loadfilepath">Path of the file to load</param> 
        ///<param name="enc">Type of Encoding</param> 
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
        ///<summary>LoadAsString Method Test</summary> 
        ///<param name="testCaseID">test case ID</param> 
        ///<param name="filePath">Path of the file to load</param> 
        /// ///<param name="fileName">name of the file</param> 
        ///<param name="enc">Type of Encoding</param> 
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
                yield return new TestCaseData("TestID-001N", MakeRelativePathFile() + "ConditionalSearch.aspx", Encoding.UTF8);
                yield return new TestCaseData("TestID-002N", MakeRelativePathFile() + "ConditionalSearch.aspx", Encoding.ASCII);
                yield return new TestCaseData("TestID-003N", MakeRelativePathFile() + "ConditionalSearch.aspx", Encoding.UTF7);

                /// Abnormal Test cases
                yield return new TestCaseData("TestID-004A", null, Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-005A", MakeRelativePathFile() + "ConditionalSearch.aspx", null).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-006A", MakeRelativePathFile() + "123a.txt", Encoding.UTF7).Throws(typeof(ArgumentNullException));
            }
        }

        public IEnumerable<TestCaseData> TestcasesOfLoadString1Test
        {
            get
            {
                ///// Normal Test cases C:\root\files\tools\DGenTemplates\ConditionalSearch.aspx
                yield return new TestCaseData("TestID-001N", MakeRelativePathFile(), "ConditionalSearch.aspx", Encoding.UTF8);
                yield return new TestCaseData("TestID-002N", MakeRelativePathFile(), "ConditionalSearch.aspx", Encoding.ASCII);
                yield return new TestCaseData("TestID-003N", MakeRelativePathFile(), "ConditionalSearch.aspx", Encoding.UTF7);
                
                /// Abnormal Test cases
                yield return new TestCaseData("TestID-004A", MakeRelativePathFile() + "F", "ConditionalSearch.aspx", Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-005A", MakeRelativePathFile(), "123a.txt", Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-006A", "", "123.txt", Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-007A", null, "123.txt", Encoding.UTF8).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-008A", @"F\", null, Encoding.UTF8).Throws(typeof(ArgumentNullException));
            }
        }
        #endregion // End of GeneratedMethods
        #endregion

        public static string MakeRelativePathFile()
        {
            string file = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            FileInfo fi = new FileInfo(file);
            string dllPath = fi.DirectoryName;
            int l = dllPath.IndexOf("programs");
            dllPath = dllPath.Substring(0, l) + @"files\tools\DGenTemplates\";
            return dllPath;
        }
    }
}

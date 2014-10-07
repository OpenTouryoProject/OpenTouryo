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
//*  Date:        Author:          Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/08/2014   Santosh Avaji    Testcode development for ResourceLoader.
//*  08/12/2014   Sai              Added TestcaseID using SetName method as per Nishino-San comments
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
        ///<param name="loadfilepath">Path of the file to load</param> 
        ///<param name="throwException">Throw Exception or not</param> 
        [TestCaseSource("TestcasesOfExistTest")]
        public void ExistsTest(string loadfilepath, bool throwException)
        {
            try
            {
                bool result = ResourceLoader.Exists(loadfilepath, throwException);
                Assert.True(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
        public IEnumerable<TestCaseData> TestcasesOfExistTest
        {
            get
            {
                yield return new TestCaseData(@"", true).Throws(typeof(ArgumentException)).SetName("TestID-001A");
                yield return new TestCaseData(@"", false).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-002A");
                yield return new TestCaseData(null, null).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-003A");
                yield return new TestCaseData(MakeRelativePathFile() + "ConditionalSearch.aspx", true).SetName("TestID-004N");
                yield return new TestCaseData(MakeRelativePathFile() + "ConditionalSearch.aspx", false).SetName("TestID-005N");
                yield return new TestCaseData(MakeRelativePathFile() + "1234.txt", false).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-006A");
                yield return new TestCaseData(MakeRelativePathFile() + "1234.txt", true).Throws(typeof(ArgumentException)).SetName("TestID-007A");
            }
        }

        ///<summary>Exists Method Test</summary> 
        ///<param name="loadfilepath">Path of the file to load</param> 
        ///<param name="fileName">File name </param> 
        ///<param name="throwException">Throw Exception or not</param> 
        [TestCaseSource("TestcasesOfExist1Test")]
        public void ExistsTest1(string filePath, string fileName, bool throwException)
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
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public IEnumerable<TestCaseData> TestcasesOfExist1Test
        {
            get
            {
                yield return new TestCaseData(@"", "", true).Throws(typeof(ArgumentException)).SetName("TestID-001A");
                yield return new TestCaseData(@"", "", false).SetName("TestID-002N");
                yield return new TestCaseData(null, null, true).Throws(typeof(ArgumentNullException)).SetName("TestID-003A");
                yield return new TestCaseData(null, null, false).Throws(typeof(ArgumentNullException)).SetName("TestID-004A");
                yield return new TestCaseData(MakeRelativePathFile(), "ConditionalSearch.aspx", true).SetName("TestID-005N");
                yield return new TestCaseData(MakeRelativePathFile(), "ConditionalSearch.aspx", false).SetName("TestID-006N");
                yield return new TestCaseData(MakeRelativePathFile(), @"\123.txt", false).SetName("TestID-007N");
                yield return new TestCaseData(MakeRelativePathFile() + @"/", "123.txt", true).Throws(typeof(ArgumentException)).SetName("TestID-008A");
                yield return new TestCaseData(@"F", "123.txt", true).Throws(typeof(ArgumentException)).SetName("TestID-009A");
            }
        }

        ///<summary>LoadAsString Method Test</summary> 
        ///<param name="loadfilepath">Path of the file to load</param> 
        ///<param name="enc">Type of Encoding</param> 
        [TestCaseSource("TestcasesOfLoadStringTest")]
        public void LoadAsStringTest(string loadfilepath, Encoding enc)
        {
            try
            {
                string strResult = null;
                strResult = ResourceLoader.LoadAsString(loadfilepath, enc);
                Assert.NotNull(strResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
        ///<summary>LoadAsString Method Test</summary> 
        ///<param name="filePath">Path of the file to load</param> 
        /// ///<param name="fileName">name of the file</param> 
        ///<param name="enc">Type of Encoding</param> 
        [TestCaseSource("TestcasesOfLoadString1Test")]
        public void LoadAsString1Test(string filePath, string fileName, Encoding enc)
        {
            try
            {
                string strResult = null;
                strResult = ResourceLoader.LoadAsString(filePath, fileName, enc);
                Assert.NotNull(strResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public IEnumerable<TestCaseData> TestcasesOfLoadStringTest
        {
            get
            {
                ///// Normal Test cases 
                yield return new TestCaseData(MakeRelativePathFile() + "ConditionalSearch.aspx", Encoding.UTF8).SetName("TestID-001N");
                yield return new TestCaseData(MakeRelativePathFile() + "ConditionalSearch.aspx", Encoding.ASCII).SetName("TestID-002N");
                yield return new TestCaseData(MakeRelativePathFile() + "ConditionalSearch.aspx", Encoding.UTF7).SetName("TestID-003N");

                /// Abnormal Test cases
                yield return new TestCaseData(null, Encoding.UTF8).Throws(typeof(ArgumentException)).SetName("TestID-004A");
                yield return new TestCaseData(MakeRelativePathFile() + "ConditionalSearch.aspx", null).Throws(typeof(ArgumentNullException)).SetName("TestID-005A");
                yield return new TestCaseData(MakeRelativePathFile() + "123a.txt", Encoding.UTF7).Throws(typeof(ArgumentException)).SetName("TestID-006A");
            }
        }

        public IEnumerable<TestCaseData> TestcasesOfLoadString1Test
        {
            get
            {
                ///// Normal Test cases C:\root\files\tools\DGenTemplates\ConditionalSearch.aspx
                yield return new TestCaseData(MakeRelativePathFile(), "ConditionalSearch.aspx", Encoding.UTF8).SetName("TestID-001N");
                yield return new TestCaseData(MakeRelativePathFile(), "ConditionalSearch.aspx", Encoding.ASCII).SetName("TestID-002N");
                yield return new TestCaseData(MakeRelativePathFile(), "ConditionalSearch.aspx", Encoding.UTF7).SetName("TestID-003N");

                /// Abnormal Test cases
                yield return new TestCaseData(MakeRelativePathFile() + "F", "ConditionalSearch.aspx", Encoding.UTF8).Throws(typeof(ArgumentException)).SetName("TestID-004A");
                yield return new TestCaseData(MakeRelativePathFile(), "123a.txt", Encoding.UTF8).Throws(typeof(ArgumentException)).SetName("TestID-005A");
                yield return new TestCaseData("", "123.txt", Encoding.UTF8).Throws(typeof(ArgumentException)).SetName("TestID-006A");
                yield return new TestCaseData(null, "123.txt", Encoding.UTF8).Throws(typeof(ArgumentNullException)).SetName("TestID-007A");
                yield return new TestCaseData(@"F\", null, Encoding.UTF8).Throws(typeof(ArgumentNullException)).SetName("TestID-008A");
            }
        }
        #endregion // End of GeneratedMethods
        #endregion

        /// <summary>
        /// Converts absolute path to Relative path
        /// </summary>
        /// <returns></returns>
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

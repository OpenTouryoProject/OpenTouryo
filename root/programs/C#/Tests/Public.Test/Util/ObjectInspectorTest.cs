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
//* クラス名        ：ObjectInspectorTest
//* クラス日本語名  ：Test of the class to Inspect the Object
//*
//* 作成者          ：Sai
//* 更新履歴        ：
//* 
//*  Date:        Author:          Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/08/2014   Sai              Testcode development for ObjectInspector.
//*  08/11/2014   Sai              Added TestcaseID using SetName method as per Nishino-San comments
//*
//**********************************************************************************

#region Includes

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.Util;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Collections;
using System.Runtime.InteropServices;

#endregion

namespace Public.Test.Util
{
    /// <summary>
    /// Tests for the Object Inspector Class
    /// </summary>
    [TestFixture, Description("Tests for Object Inspector")]
    public class ObjectInspectorTest
    {
        #region Class Variables

        private ObjectInspector _objectInspector;

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
            //New instance of Object Inspector
            _objectInspector = new ObjectInspector();
        }

        /// <summary>
        /// Code that is run after each test
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            //TODO:  Put dispose in here for _objectInspector or delete this line
        }
        #endregion

        #region Test data

        /// <summary>
        /// This class is created to generate test cases.
        /// This class to generate test data to be passed to the method InspectTest.
        /// </summary>
        public class MyClass
        {
            string[] _myArray = new string[5];

            public string this[int index]
            {
                get
                {
                    return _myArray[index];
                }
                set
                {
                    _myArray[index] = value;
                }
            }
        }

        /// <summary>
        /// This class is created to generate test cases.
        /// This class to generate test data to be passed to the method InspectTest.
        /// </summary>
        public class MyClass1
        {
            string[] _myArray = null;

            public string this[int index]
            {
                get
                {
                    return _myArray[index];
                }
                set
                {
                    _myArray[index] = value;
                }
            }
        }

        /// <summary>
        /// This class is created to generate test cases.
        /// This class to generate test data to be passed to the method InspectTest.
        /// </summary>
        public class MyClass2
        {
            ArrayList _myArray = null;

            public ArrayList this[ArrayList index]
            {
                get
                {
                    return _myArray;
                }
                set
                {
                    _myArray = value;
                }
            }
        }

        /// <summary>
        /// This class is created to generate test cases.
        /// This class to generate test data to be passed to the method InspectTest.
        /// </summary>
        public class MyClass3
        {
            ArrayList _myArray = null;

            public ArrayList this[ArrayList index, int index1]
            {
                get
                {
                    return _myArray;
                }
                set
                {
                    _myArray = value;
                }
            }
        }

        /// <summary>
        /// This structure is created to generate test cases.
        /// This structure to generate test data to be passed to the method InspectTest.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct XYData
        {

            public int x;

            public int y;

            public XYData(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public int EmpNo
            {
                get
                {
                    return 121;
                }
            }

            public string EName
            {
                get
                {
                    return "John";
                }
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method InspectTest.
        /// </summary>
        public IEnumerable<TestCaseData> TestData
        {
            get
            {
                // If you need to prepare for the test data below so, you use the TestCaseData.
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("EmpNo", "1234");
                dic.Add("EmpName", "John");
                Dictionary<string, string> iDic = dic;
                yield return new TestCaseData(new object()).SetName("TestID-001N");
                yield return new TestCaseData(new object[] { "Test1", "Test2" }).SetName("TestID-002N");
                yield return new TestCaseData(234).SetName("TestID-003N");
                yield return new TestCaseData("Test").SetName("TestID-004N");
                yield return new TestCaseData(new string[] { "", "test" }).SetName("TestID-005N");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method InspectTest.
        /// </summary>
        public IEnumerable<TestCaseData> TestCasesOfInspectTest
        {
            get
            {
                // If you need to prepare for the test data below so, you use the TestCaseData.
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("EmpNo", "1234");
                dic.Add("EmpName", "John");
                dic.Add("Design", "Engineer");
                dic.Add("Dept", "Engineering");
                dic.Add("Section", "E");
                Dictionary<string, string> iDic = dic;
                IEnumerable<IEnumerable<IEnumerable<IEnumerable<IEnumerable<IEnumerable<int>>>>>> result = new[] { new[] { new[] { new[] { new[] { new[] { 1, 2 }, new[] { 3, 4 } } } } } };
                DateTime d1 = DateTime.Now;
                IDictionary<string, int> dictionary = new Dictionary<string, int>();
                object mytest1 = new object[] { "Test1", "Test2" };
                object mytest2 = new string[] { "", "test" };
                //Normal test case
                yield return new TestCaseData(new object()).SetName("TestID-001N");
                yield return new TestCaseData(mytest1).SetName("TestID-002N");
                yield return new TestCaseData(234).SetName("TestID-003N");
                yield return new TestCaseData("Test").SetName("TestID-004N");
                yield return new TestCaseData(mytest2).SetName("TestID-005N");
                ObjectInspector.DateTimeFormat = string.Empty;
                object mytest3 = new object[]{null,"test",DateTime.Now,DateTime.Now.TimeOfDay,2.5,new StringBuilder("Test String builder")
                                               ,DateTimeKind.Local,new int[]{1,2,3},TestData,new XYData(),iDic};
                yield return new TestCaseData(mytest3).SetName("TestID-006N");
                ObjectInspector.DateTimeFormat = "dd-MMM-yyyy";
                ObjectInspector.ExclusionFullyQualifiedNameParts = new string[] { "Object" };
                object mytest4 = new object[]{null,"test",DateTime.Now,DateTime.Now.TimeOfDay,2.5,new StringBuilder("Test String builder")
                                               ,DateTimeKind.Local,new int[]{1,2,3},TestData,new XYData(),iDic};
                yield return new TestCaseData(mytest4).SetName("TestID-007N");
                iDic.Remove("EmpNo");
                iDic.Remove("EmpName");
                object mytest5 = new object[]{null,"test",DateTime.Now,DateTime.Now.TimeOfDay,2.5,new StringBuilder("Test String builder")
                                               ,DateTimeKind.Local,new int[]{},TestData,new XYData(),iDic};
                yield return new TestCaseData(mytest5).SetName("TestID-008N");
                ObjectInspector.ExclusionFullyQualifiedNameParts = new string[] { };
                object mytest6 = new object[] {null,"test",DateTime.Now,DateTime.Now.TimeOfDay,2.5,new StringBuilder("Test String builder")
                                               ,DateTimeKind.Local,new int[]{},TestData,new XYData(),iDic, new MyClass() };
                //ObjectInspector.DateTimeFormat = string.Empty;
                yield return new TestCaseData(mytest6).SetName("TestID-009N");
                yield return new TestCaseData(result).SetName("TestID-010N");
                yield return new TestCaseData(d1).SetName("TestID-011N");
                yield return new TestCaseData(dictionary).SetName("TestID-012N");
                yield return new TestCaseData(new XYData()).SetName("TestID-013N");
                yield return new TestCaseData(new MyClass1()).SetName("TestID-014N");
                yield return new TestCaseData(new MyClass2()).SetName("TestID-015N");
                yield return new TestCaseData(new MyClass3()).SetName("TestID-016N");
                //Abnormal test case
                yield return new TestCaseData(null).SetName("TestID-0017L");
                yield return new TestCaseData(string.Empty).SetName("TestID-0018L");
                // yield return new TestCaseData("TestID-0019L", myvalue);
            }
        }

        #endregion

        /// <summary>
        /// Test code development for Inspect Method
        /// </summary>        
        /// <param name="obj">obj</param>
        [TestCaseSource("TestCasesOfInspectTest")]
        public void InspectTest(object obj)
        {
            try
            {
                if (obj is DateTime)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            ObjectInspector.DateTimeFormat = "dd/mm/yyyy";
                            CallInspect(obj);
                        }

                        if (i == 1)
                        {
                            ObjectInspector.DateTimeFormat = string.Empty;
                            CallInspect(obj);
                        }
                    }
                }

                if (obj != null)
                {
                    if (obj.GetType().StructLayoutAttribute != null)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            if (i == 0)
                            {
                                ObjectInspector.ExclusionFullyQualifiedNameParts = new string[] { "a" };
                                CallInspect(obj);
                            }

                            if (i == 1)
                            {
                                ObjectInspector.ExclusionFullyQualifiedNameParts = new string[] { };
                                CallInspect(obj);
                            }
                        }
                    }
                }

                string expected = ObjectInspector.Inspect(obj);
                string results = ObjectInspector.Inspect(obj);
                Assert.AreEqual(expected, results);
                //Assert.AreNotEqual(obj, results);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// CallInspect Method
        /// This Method has been called in InspectTest Method
        /// </summary>
        /// <param name="obj">obj</param>
        private void CallInspect(object obj)
        {
            string expected;
            string results;

            expected = ObjectInspector.Inspect(obj);
            results = ObjectInspector.Inspect(obj);
            Assert.AreEqual(expected, results);
        }
    }
}

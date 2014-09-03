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
//* クラス名        ：LatebindTest
//* クラス日本語名  ：Test of the class to LateBind
//*
//* 作成者          ：Santosh
//* 更新履歴        ：
//* 
//*  Date:        Author:          Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  05/21/2014   Santosh Avaji    Testcode development for Latebind.
//*  08/11/2014   Sai              Added TestcaseID using SetName method as per Nishino-San comments
//*
//**********************************************************************************

#region Includes

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.IO;
using System.IO;

#endregion

namespace Public.Test.Util
{
    /// <summary>
    /// Tests for the Latebind Class
    /// </summary>
    [TestFixture, Description("Tests for Latebind")]
    public class LatebindTest
    {
        #region Class Variables
        private Latebind _latebind;
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
            //New instance of Latebind
            _latebind = new Latebind();
        }

        /// <summary>
        /// Code that is run after each test
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            //TODO:  Put dispose in here for _latebind or delete this line
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
        /// Invoke Method Method Test
        /// Method Signature:  object InvokeMethod(string assemblyName, string className, string methodName, object[] paramSet)
        /// </summary>        
        /// <param name="assemblyName">assemblyName</param>
        /// <param name="className">className</param>
        /// <param name="methodName">methodName</param>
        /// <param name="paramSet">paramSet</param>
        [TestCaseSource("TestCasesOfInvokeMethodTest")]
        public void InvokeMethodTest(string assemblyName, string className, string methodName, object[] paramSet)
        {
            try
            {
                object results = Latebind.InvokeMethod(assemblyName, className, methodName, paramSet);
                Assert.NotNull(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }

        }

        /// <summary>
        /// Invoke Method No Err Method Test
        /// Method Signature:  object InvokeMethod_NoErr(string assemblyName, string className, string methodName, object[] paramSet)
        /// </summary>
        /// <param name="assemblyName">assemblyName</param>
        /// <param name="className">className</param>
        /// <param name="methodName">methodName</param>
        /// <param name="paramSet"></param>
        [TestCaseSource("TestCasesOfInvokeMethod_NoErrTest")]
        public void InvokeMethod_NoErrTest(string assemblyName, string className, string methodName, object[] paramSet)
        {
            try
            {
                object results = Latebind.InvokeMethod_NoErr(assemblyName, className, methodName, paramSet);
                Assert.NotNull(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Invoke Method Method Test
        /// Method Signature:  object InvokeMethod(object objectClass, string methodName, object[] paramSet)
        /// </summary>        
        /// <param name="objectClass">objectClass</param>
        /// <param name="methodName">methodName</param>
        /// <param name="paramSet">paramSet</param>
        [TestCaseSource("TestCasesOfInvokeMethod1Test")]
        public void InvokeMethod1Test(object objectClass, string methodName, object[] paramSet)
        {
            try
            {
                object results = Latebind.InvokeMethod(objectClass, methodName, paramSet);

                Assert.NotNull(results);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Invoke Method No Err Method Test
        /// Method Signature:  object InvokeMethod_NoErr(object objectClass, string methodName, object[] paramSet)
        /// </summary>
        /// <param name="objectClass">objectClass</param>
        /// <param name="methodName">methodName</param>
        /// <param name="paramSet">paramSet</param>
        [TestCaseSource("TestCasesOfInvokeMethod_NoErr1Test")]
        public void InvokeMethod_NoErr1Test(object objectClass, string methodName, object[] paramSet)
        {

            try
            {
                object results = Latebind.InvokeMethod_NoErr(objectClass, methodName, paramSet);

                Assert.NotNull(results);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Check Type Of Base Class Method Test
        /// Method Signature:  bool CheckTypeOfBaseClass(Type classType, Type baseType)
        /// </summary>
        /// <param name="classType">classType</param>
        /// <param name="baseType">baseType</param>
        [TestCaseSource("TestCasesOfCheckTypeOfBaseClass")]
        public void CheckTypeOfBaseClassTest(Type classType, Type baseType)
        {
            try
            {
                Assert.IsTrue(Latebind.CheckTypeOfBaseClass(classType, baseType));
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Check Type Of Method By Name Method Test
        /// Method Signature:  bool CheckTypeOfMethodByName(object obj, string methodName)
        /// </summary>
        /// <param name="obj">obj</param>
        /// <param name="methodName">methodName</param>
        [TestCaseSource("TestCasesOfCheckTypeOfMethodByName")]
        public void CheckTypeOfMethodByNameTest(object obj, string methodName)
        {
            try
            {
                //Parameters
                Assert.IsTrue(Latebind.CheckTypeOfMethodByName(obj, methodName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        #endregion // End of GeneratedMethods

        #endregion

        #region Testcasersource data

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method CheckTypeOfMethodByNameTest.
        /// </summary>
        IEnumerable<TestCaseData> TestCasesOfCheckTypeOfMethodByName
        {
            get
            {
                //Normal test cases
                yield return new TestCaseData(new Latebind(), "CheckTypeOfMethodByName").SetName("TestID-001N");
                yield return new TestCaseData(new Latebind(), "InvokeMethod").SetName("TestID-002N");
                yield return new TestCaseData(new Latebind(), "InvokeMethod_NoErr").SetName("TestID-003N");

                //AbNormal test cases
                yield return new TestCaseData(new Latebind(), "WrongMethodName").Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-004A");
                yield return new TestCaseData(new Latebind(), null).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-005A");
                yield return new TestCaseData(null, "WrongMethodName").Throws(typeof(NullReferenceException)).SetName("TestID-006A");
                yield return new TestCaseData(new Latebind(), string.Empty).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-007A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method CheckTypeOfBaseClassTest.
        /// </summary>
        IEnumerable<TestCaseData> TestCasesOfCheckTypeOfBaseClass
        {
            get
            {
                yield return new TestCaseData(typeof(Latebind), typeof(Object)).SetName("TestID-001N");
                yield return new TestCaseData(typeof(string), typeof(Object)).SetName("TestID-002N");
                yield return new TestCaseData(typeof(double), typeof(object)).SetName("TestID-003N");
                yield return new TestCaseData(typeof(LatebindTest), typeof(object)).SetName("TestID-004N");
                yield return new TestCaseData(typeof(Latebind), null).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-005A");
                yield return new TestCaseData(null, typeof(Latebind)).Throws(typeof(NullReferenceException)).SetName("TestID-006A");
                yield return new TestCaseData(null, null).Throws(typeof(NullReferenceException)).SetName("TestID-007A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method InvokeMethod_NoErr1Test.
        /// </summary>
        IEnumerable<TestCaseData> TestCasesOfInvokeMethod_NoErr1Test
        {
            get
            {
                yield return new TestCaseData(new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).SetName("TestID-001N");
                yield return new TestCaseData(new Latebind(), "CheckTypeOfBaseClass", new object[2] { typeof(Latebind), typeof(Object) }).SetName("TestID-002N");
                yield return new TestCaseData(new BinarySerialize(), "DeepClone", new object[1] { new byte[] { 0, 1, 2 } }).SetName("TestID-003N");
                yield return new TestCaseData(new TestLateBind(), "Add", new object[2] { 5, 5 }).SetName("TestID-004N");
                yield return new TestCaseData(new TestLateBind(), "Add", new object[3] { 5, 10, 15 }).SetName("TestID-005N");
                yield return new TestCaseData(new TestLateBind(), "DisplayData", new object[0]).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-006N");
                yield return new TestCaseData(new Latebind(), "WrongCheckTypeOfBaseClass", new object[2] { typeof(Latebind), typeof(Object) }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-007A");
                yield return new TestCaseData(new Latebind(), "CheckTypeOfBaseClass", new object[1] { typeof(Latebind) }).Throws(typeof(System.MissingMethodException)).SetName("TestID-008A");
                yield return new TestCaseData(new TestLateBind(), "Add", new object[4] { 5, 10, 15, 10 }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-009A");
                yield return new TestCaseData(new TestLateBind(), "Add", new object[1] { 5 }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-0010A");
                yield return new TestCaseData(new TestLateBind(), "Add", new object[2] { 5, "" }).Throws(typeof(ArgumentException)).SetName("TestID-0011A");
                yield return new TestCaseData(new TestLateBind(), null, new object[2] { 5, "" }).Throws(typeof(ArgumentNullException)).SetName("TestID-0012A");
                yield return new TestCaseData(null, "Add", new object[2] { 5, "" }).Throws(typeof(System.NullReferenceException)).SetName("TestID-0013A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method InvokeMethod1Test.
        /// </summary>
        IEnumerable<TestCaseData> TestCasesOfInvokeMethod1Test
        {
            get
            {
                yield return new TestCaseData(new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).SetName("TestID-001N");
                yield return new TestCaseData(new Latebind(), "CheckTypeOfBaseClass", new object[2] { typeof(Latebind), typeof(Object) }).SetName("TestID-002N");
                yield return new TestCaseData(new BinarySerialize(), "DeepClone", new object[1] { new byte[] { 0, 1, 2 } }).SetName("TestID-003N");
                yield return new TestCaseData(new TestLateBind(), "Add", new object[2] { 5, 5 }).SetName("TestID-004N");
                yield return new TestCaseData(new TestLateBind(), "Add", new object[3] { 5, 10, 15 }).SetName("TestID-005N");
                yield return new TestCaseData(new TestLateBind(), "DisplayData", new object[0]).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-006N");
                yield return new TestCaseData(new Latebind(), "WrongCheckTypeOfBaseClass", new object[2] { typeof(Latebind), typeof(Object) }).Throws(typeof(ArgumentException)).SetName("TestID-006A");
                yield return new TestCaseData(new Latebind(), "CheckTypeOfBaseClass", new object[1] { typeof(Latebind) }).Throws(typeof(System.MissingMethodException)).SetName("TestID-007A");
                yield return new TestCaseData(new TestLateBind(), "Add", new object[4] { 5, 10, 15, 10 }).Throws(typeof(ArgumentException)).SetName("TestID-008A");
                yield return new TestCaseData(new TestLateBind(), "Add", new object[1] { 5 }).Throws(typeof(ArgumentException)).SetName("TestID-009A");
                yield return new TestCaseData(new TestLateBind(), "Add", new object[2] { 5, "" }).Throws(typeof(ArgumentException)).SetName("TestID-0010A");
                yield return new TestCaseData(new TestLateBind(), null, new object[2] { 5, "" }).Throws(typeof(ArgumentNullException)).SetName("TestID-0011A");
                yield return new TestCaseData(null, "Add", new object[2] { 5, "" }).Throws(typeof(System.NullReferenceException)).SetName("TestID-0012A");
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method InvokeMethodTest.
        /// </summary>
        IEnumerable<TestCaseData> TestCasesOfInvokeMethodTest
        {
            get
            {
                #region Normal TestCases

                //Pass All valid parameters and  assembly with full path.
                yield return new TestCaseData(MakeRelativePathFile() + "Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).SetName("TestID-001N");
                yield return new TestCaseData(MakeRelativePathFile() + "Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "WrongMethodName" }).SetName("TestID-002N");
                //Test case to cover AmbiguousMatchException..(Oveloading Methods)
                yield return new TestCaseData(MakeRelativePathFile() + "Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[3] { new Latebind(), "InvokeMethod", new object[3] { new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "InvokeMethod" } } }).SetName("TestID-003N");
                //Pass All valid parameters(Only name of the Assembly)
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).SetName("TestID-004N");
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "WrongMethodName" }).SetName("TestID-005N");
                //Test case to cover AmbiguousMatchException..(Oveloading Methods)
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[3] { new Latebind(), "InvokeMethod", new object[3] { new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "InvokeMethod" } } }).SetName("TestID-006N");

                #endregion

                #region AbNormal TestCases

                // provide  wrong Assembly path name   to the method 
                yield return new TestCaseData(MakeRelativePathFile() + "WrongPublic.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException)).SetName("TestID-007A");
                yield return new TestCaseData(MakeRelativePathFile() + "Public.", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException)).SetName("TestID-008A");
                yield return new TestCaseData("AbcdAssembly", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException)).SetName("TestID-009A");
                //Provide Wrong class Name
                yield return new TestCaseData(MakeRelativePathFile() + "Public.dll", "Touryo.Infrastructure.Public.Util.Latebind.cs", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException)).SetName("TestID-0010A");
                yield return new TestCaseData("Public", "WrongClassLatebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException)).SetName("TestID-0011A");
                // Provide Wrong Method Name 
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "WrongMethodName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException)).SetName("TestID-0012A");
                yield return new TestCaseData(MakeRelativePathFile() + "Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByNameNotExist", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException)).SetName("TestID-0013A");
                // Provide Wrong number of parametrs to the Method
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[2] { new Latebind(), "InvokeMethod" }).Throws(typeof(ArgumentException)).SetName("TestID-0014A");//Method takes two parameters but only two are passed
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[5] { new Latebind(), "InvokeMethod", new object[3] { new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "InvokeMethod" } }, "Extra Parametr", 123456 }).Throws(typeof(ArgumentException)).SetName("TestID-0015A");////Method takes four parameters but five are passed
                // Provide string.empty values to Assembly, Class and Methods Name
                yield return new TestCaseData(string.Empty, "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException)).SetName("TestID-0016A");// pass Empty string to Assemblyname
                yield return new TestCaseData("Public", string.Empty, "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException)).SetName("TestID-0017A");// pass Empty string to Classname
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", string.Empty, new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException)).SetName("TestID-0018A");// pass Empty string to MethodName
                //Provide Null values to Assembly, Class and Methods Name
                yield return new TestCaseData(null, "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException)).SetName("TestID-0019A");// pass null to Assemblyname
                yield return new TestCaseData("Public", null, "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException)).SetName("TestID-0020A");// pass null to Classname
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", null, new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException)).SetName("TestID-0021A");// pass null to MethodName
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", null).Throws(typeof(MissingMethodException)).SetName("TestID-0022A");// pass null to Parameset

                #endregion
            }
        }

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method InvokeMethod_NoErrTest.
        /// </summary>
        IEnumerable<TestCaseData> TestCasesOfInvokeMethod_NoErrTest
        {
            get
            {
                #region Normal TestCases

                //Pass All valid parameters and  assembly with full path.
                yield return new TestCaseData(MakeRelativePathFile() + "Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).SetName("TestID-001N");
                yield return new TestCaseData(MakeRelativePathFile() + "Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "WrongMethodName" }).SetName("TestID-002N");
                //Test case to cover AmbiguousMatchException..(Oveloading Methods)
                yield return new TestCaseData(MakeRelativePathFile() + "Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[3] { new Latebind(), "InvokeMethod", new object[3] { new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "InvokeMethod" } } }).SetName("TestID-003N");
                //Pass All valid parameters(Only name of the Assembly)
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).SetName("TestID-004N");
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "WrongMethodName" }).SetName("TestID-005N");
                //Test case to cover AmbiguousMatchException..(Oveloading Methods)
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[3] { new Latebind(), "InvokeMethod", new object[3] { new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "InvokeMethod" } } }).SetName("TestID-006N");
                yield return new TestCaseData("Public.Test", "Public.Test.Util.TestLateBind", "Display", new object[0] { }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-007N");

                #endregion

                #region AbNormal TestCases

                // provide  wrong Assembly path name   to the method 
                yield return new TestCaseData(MakeRelativePathFile() + "WrongPublic.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-007A");
                yield return new TestCaseData(MakeRelativePathFile() + "Public.", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-008A");
                yield return new TestCaseData("AbcdAssembly", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-009A");
                //Provide Wrong class Name
                yield return new TestCaseData(MakeRelativePathFile() + "Public.dll", "Touryo.Infrastructure.Public.Util.Latebind.cs", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-0010A");
                yield return new TestCaseData("Public", "WrongClassLatebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-0011A");
                // Provide Wrong Method Name 
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "WrongMethodName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-0012A");
                yield return new TestCaseData(MakeRelativePathFile() + "Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByNameNotExist", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-0013A");
                // Provide Wrong number of parametrs to the Method
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[2] { new Latebind(), "InvokeMethod" }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-0014A");//Method takes two parameters but only two are passed
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[5] { new Latebind(), "InvokeMethod", new object[3] { new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "InvokeMethod" } }, "Extra Parametr", 123456 }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-0015A");////Method takes four parameters but five are passed
                // Provide string.empty values to Assembly, Class and Methods Name
                yield return new TestCaseData(string.Empty, "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-0016A");// pass Empty string to Assemblyname
                yield return new TestCaseData("Public", string.Empty, "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException)).SetName("TestID-0017A");// pass Empty string to Classname
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", string.Empty, new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(NUnit.Framework.AssertionException)).SetName("TestID-0018A");// pass Empty string to MethodName
                //Provide Null values to Assembly, Class and Methods Name
                yield return new TestCaseData(null, "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException)).SetName("TestID-0019A");// pass null to Assemblyname
                yield return new TestCaseData("Public", null, "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException)).SetName("TestID-0020A");// pass null to Classname
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", null, new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException)).SetName("TestID-0021A");// pass null to MethodName
                yield return new TestCaseData("Public", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", null).Throws(typeof(MissingMethodException)).SetName("TestID-0022A");// pass null to Parameset

                #endregion
            }
        }

        #endregion

        /// <summary>
        /// Converts absolute path to Relative path
        /// </summary>
        /// <returns></returns>
        private object MakeRelativePathFile()
        {
            string dllPath = Directory.GetCurrentDirectory() + "\\";
            return dllPath;
        }

    }

    /// <summary>
    /// This class to generate test cases
    /// This class to generate test data to be passed to the method InvokeMethod_NoErr1Test and InvokeMethod1Test.
    /// </summary>
    public class TestLateBind
    {
        public object Add(int intNum1, int intNum2)
        {
            return intNum1 + intNum2;
        }

        public object Add(int intNum1, int intNum2, int intNum3)
        {
            return intNum1 + intNum2 + intNum3;
        }

        public object AddString(string strName, string strAddress)
        {
            return strName + " is available in " + strAddress;
        }

        public void DisplayData(string strDisplay)
        {
            Console.WriteLine("Test the Display Method " + strDisplay);
        }

        public void DisplayData()
        {
            Console.WriteLine("Test the Display Method ");
        }

    }
}
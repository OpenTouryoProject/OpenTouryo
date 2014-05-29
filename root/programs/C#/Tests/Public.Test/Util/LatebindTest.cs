using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.IO;

namespace Public.Test.Util
{
    /// <summary>
    /// Tests for the Latebind Class
    /// Documentation: ãƒ¬ã‚¤ãƒˆãƒã‚¤ãƒ³ãƒ‰å‡¦ç†ã‚¯ãƒ©ã‚¹
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
        /// Documentation   :  é…å»¶ãƒã‚¤ãƒ³ãƒ‰ï¼ˆãƒ¡ã‚½ãƒƒãƒ‰ï¼‰
        /// Method Signature:  object InvokeMethod(string assemblyName, string className, string methodName, object[] paramSet)
        /// </summary>
        [TestCaseSource("TestCasesOfInvokeMethodTest")]
        public void InvokeMethodTest(string testCaseID, string assemblyName, string className, string methodName, object[] paramSet)
        {
            try
            {
                object results = Latebind.InvokeMethod(assemblyName, className, methodName, paramSet);
                Assert.NotNull(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;  
            }
           
        }

        /// <summary>
        /// Invoke Method No Err Method Test
        /// Documentation   :  é…å»¶ãƒã‚¤ãƒ³ãƒ‰ï¼ˆãƒ¡ã‚½ãƒƒãƒ‰ï¼‰
        /// Method Signature:  object InvokeMethod_NoErr(string assemblyName, string className, string methodName, object[] paramSet)
        /// </summary>
       [TestCaseSource("TestCasesOfInvokeMethod_NoErrTest")]
        public void InvokeMethod_NoErrTest(string testCaseID, string assemblyName, string className, string methodName, object[] paramSet)
        {
            try
            {
                object results = Latebind.InvokeMethod_NoErr(assemblyName, className, methodName, paramSet);
                Assert.NotNull(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Invoke Method Method Test
        /// Documentation   :  é…å»¶ãƒã‚¤ãƒ³ãƒ‰ï¼ˆãƒ¡ã‚½ãƒƒãƒ‰ï¼‰
        /// Method Signature:  object InvokeMethod(object objectClass, string methodName, object[] paramSet)
        /// </summary>
       [TestCaseSource("TestCasesOfInvokeMethod1Test")]
        public void InvokeMethod1Test(string testCaseID, object objectClass, string methodName, object[] paramSet)
        {
            try
            {
                object results = Latebind.InvokeMethod(objectClass, methodName, paramSet);

                Assert.NotNull(results);
            }
            catch (Exception ex)
            {

                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;  
            }
        }

        /// <summary>
        /// Invoke Method No Err Method Test
        /// Documentation   :  é…å»¶ãƒã‚¤ãƒ³ãƒ‰ï¼ˆãƒ¡ã‚½ãƒƒãƒ‰ï¼‰
        /// Method Signature:  object InvokeMethod_NoErr(object objectClass, string methodName, object[] paramSet)
        /// </summary>
        [TestCaseSource("TestCasesOfInvokeMethod_NoErr1Test")]
        public void InvokeMethod_NoErr1Test(string testCaseID, object objectClass, string methodName, object[] paramSet)
        {

            try
            {
                object results = Latebind.InvokeMethod_NoErr(objectClass, methodName, paramSet);

                Assert.NotNull(results);
            }
            catch (Exception ex)
            {

                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
            

            
        }

        /// <summary>
        /// Check Type Of Base Class Method Test
        /// Documentation   :  åž‹ã®ãƒ™ãƒ¼ã‚¹åž‹ãŒä¸€è‡´ã—ã¦ã„ã‚‹ã‹ãƒã‚§ãƒƒã‚¯
        /// Method Signature:  bool CheckTypeOfBaseClass(Type classType, Type baseType)
        /// </summary>
        [TestCaseSource("TestCasesOfCheckTypeOfBaseClass")] 
        public void CheckTypeOfBaseClassTest(string testCaseID, Type classType, Type baseType)
        {
            try
            {
                Assert.IsTrue(Latebind.CheckTypeOfBaseClass(classType, baseType));
            }
            catch (Exception ex)
            {
                
               Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Check Type Of Method By Name Method Test
        /// Documentation   :  å½“è©²ã‚ªãƒ–ã‚¸ã‚§ã‚¯ãƒˆã«æŒ‡å®šã®ãƒ¡ã‚½ãƒƒãƒ‰ãŒå­˜åœ¨ã™ã‚‹ã‹ãƒã‚§ãƒƒã‚¯ã™ã‚‹ã€‚
        /// Method Signature:  bool CheckTypeOfMethodByName(object obj, string methodName)
        /// </summary>
        
        [TestCaseSource("TestCasesOfCheckTypeOfMethodByName")]       
        public void CheckTypeOfMethodByNameTest(string testCaseID,object obj, string methodName)
        {
            try
            {
                //Parameters
                Assert.IsTrue(Latebind.CheckTypeOfMethodByName(obj, methodName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(testCaseID + ":" + ex.StackTrace);
                throw;
            }
        }

        #endregion // End of GeneratedMethods

        #endregion

        #region Testcasersource data

        IEnumerable<TestCaseData> TestCasesOfCheckTypeOfMethodByName
        {
            get
            { 
                //Normal test cases
               yield return new TestCaseData("TestID-001N",new Latebind(),"CheckTypeOfMethodByName") ;
               yield return new TestCaseData("TestID-002N",new Latebind(),"InvokeMethod") ;
               yield return new TestCaseData("TestID-003N",new Latebind(),"InvokeMethod_NoErr") ;
                //AbNormal test cases
               yield return new TestCaseData("TestID-004A",new Latebind(),"WrongMethodName").Throws(typeof(NUnit.Framework.AssertionException));
               yield return new TestCaseData("TestID-005A",new Latebind(),null).Throws(typeof(NUnit.Framework.AssertionException));
               yield return new TestCaseData("TestID-006A", null, "WrongMethodName").Throws(typeof(NullReferenceException));
               yield return new TestCaseData("TestID-007A", new Latebind(), string.Empty).Throws(typeof(NUnit.Framework.AssertionException));
             
            }
         }

        IEnumerable<TestCaseData> TestCasesOfCheckTypeOfBaseClass
        {
            get
            { 
             yield return new TestCaseData("TestID-001N",typeof(Latebind), typeof(Object));
             yield return new TestCaseData("TestID-002N",typeof(string), typeof(Object));
             yield return new TestCaseData("TestID-003N",typeof(double), typeof(object));
             yield return new TestCaseData("TestID-004N", typeof(LatebindTest), typeof(object));

             yield return new TestCaseData("TestID-005A",typeof(Latebind), null).Throws(typeof(NUnit.Framework.AssertionException));
             yield return new TestCaseData("TestID-006A", null, typeof(Latebind)).Throws(typeof(NullReferenceException));
             yield return new TestCaseData("TestID-007A", null, null).Throws(typeof(NullReferenceException));
            }
        
        }

        IEnumerable<TestCaseData> TestCasesOfInvokeMethod_NoErr1Test
        {
            get
            {
                yield return new TestCaseData("TestID-001N", new Latebind(), "CheckTypeOfMethodByName", new object[2]{new Latebind(),"CheckTypeOfMethodByName"});

                yield return new TestCaseData("TestID-002N", new Latebind(), "CheckTypeOfBaseClass", new object[2] { typeof(Latebind), typeof(Object) });

                yield return new TestCaseData("TestID-003N", new BinarySerialize(), "DeepClone", new object[1] { new byte[] { 0, 1, 2 } });

                yield return new TestCaseData("TestID-004N", new TestLateBind(),"Add", new object[2]{5,5});
                yield return new TestCaseData("TestID-005N", new TestLateBind(), "Add", new object[3] {5,10,15});
                yield return new TestCaseData("TestID-006N", new TestLateBind(), "DisplayData", new object[0]).Throws(typeof(NUnit.Framework.AssertionException));

                yield return new TestCaseData("TestID-007A", new Latebind(), "WrongCheckTypeOfBaseClass", new object[2] { typeof(Latebind), typeof(Object) }).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-008A", new Latebind(), "CheckTypeOfBaseClass", new object[1] { typeof(Latebind) }).Throws(typeof(System.MissingMethodException));

                yield return new TestCaseData("TestID-009A", new TestLateBind(), "Add", new object[4] { 5, 10, 15, 10 }).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-0010A", new TestLateBind(), "Add", new object[1] { 5 }).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-0011A", new TestLateBind(), "Add", new object[2] { 5, "" }).Throws(typeof(ArgumentException));

                yield return new TestCaseData("TestID-0012A", new TestLateBind(), null, new object[2] { 5, "" }).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-0013A", null, "Add", new object[2] { 5, "" }).Throws(typeof(System.NullReferenceException));

            }
        }

        IEnumerable<TestCaseData> TestCasesOfInvokeMethod1Test
        {
            get
            {
                yield return new TestCaseData("TestID-001N", new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" });

                yield return new TestCaseData("TestID-002N", new Latebind(), "CheckTypeOfBaseClass", new object[2] { typeof(Latebind), typeof(Object) });

                yield return new TestCaseData("TestID-003N", new BinarySerialize(), "DeepClone", new object[1] { new byte[] { 0, 1, 2 } });

                yield return new TestCaseData("TestID-004N", new TestLateBind(), "Add", new object[2] { 5, 5 });
                yield return new TestCaseData("TestID-005N", new TestLateBind(), "Add", new object[3] { 5, 10, 15 });
                yield return new TestCaseData("TestID-006N", new TestLateBind(), "DisplayData", new object[0]).Throws(typeof(NUnit.Framework.AssertionException));

                yield return new TestCaseData("TestID-006A", new Latebind(), "WrongCheckTypeOfBaseClass", new object[2] { typeof(Latebind), typeof(Object) }).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-007A", new Latebind(), "CheckTypeOfBaseClass", new object[1] { typeof(Latebind) }).Throws(typeof(System.MissingMethodException));

                yield return new TestCaseData("TestID-008A", new TestLateBind(), "Add", new object[4] { 5, 10, 15, 10 }).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-009A", new TestLateBind(), "Add", new object[1] { 5 }).Throws(typeof(ArgumentNullException));

                yield return new TestCaseData("TestID-0010A", new TestLateBind(), "Add", new object[2] { 5, "" }).Throws(typeof(ArgumentException));

                yield return new TestCaseData("TestID-0011A", new TestLateBind(), null, new object[2] { 5, "" }).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-0012A", null, "Add", new object[2] { 5, "" }).Throws(typeof(System.NullReferenceException));

            }
        }
        #endregion


        IEnumerable<TestCaseData> TestCasesOfInvokeMethodTest
        {
            get
            {
                #region Normal TestCases
                //Pass All valid parameters and  assembly with full path.
                yield return new TestCaseData("TestID-001N", @"C:\root\Tests\Dll\Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName"});
                yield return new TestCaseData("TestID-002N", @"C:\root\Tests\Dll\Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "WrongMethodName" });
                //Test case to cover AmbiguousMatchException..(Oveloading Methods)
                yield return new TestCaseData("TestID-003N",@"C:\root\Tests\Dll\Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[3] { new Latebind(), "InvokeMethod", new object[3] { new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "InvokeMethod" } } });

                //Pass All valid parameters(Only name of the Assembly)
                yield return new TestCaseData("TestID-004N", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" });
                yield return new TestCaseData("TestID-005N", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "WrongMethodName" });
                //Test case to cover AmbiguousMatchException..(Oveloading Methods)
                yield return new TestCaseData("TestID-006N", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[3] { new Latebind(), "InvokeMethod", new object[3] { new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "InvokeMethod" } } });

                #endregion 

                #region AbNormal TestCases
                // provide  wrong Assembly path name   to the method 
                yield return new TestCaseData("TestID-007A", @"C:\root\Tests\Dll\WrongPublic.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-008A", @"C:\root\Tests\Dll\Public.", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-009A", "AbcdAssembly", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));
                //Provide Wrong class Name
                yield return new TestCaseData("TestID-0010A", @"C:\root\Tests\Dll\Public.dll", "Touryo.Infrastructure.Public.Util.Latebind.cs", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-0011A", "Public", "WrongClassLatebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));
                // Provide Wrong Method Name 
                yield return new TestCaseData("TestID-0012A", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "WrongMethodName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-0013A", @"C:\root\Tests\Dll\Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByNameNotExist", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));

                // Provide Wrong number of parametrs to the Method
                yield return new TestCaseData("TestID-0014A", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[2] { new Latebind(), "InvokeMethod" }).Throws(typeof(ArgumentNullException));//Method takes two parameters but only two are passed
                yield return new TestCaseData("TestID-0015A", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[5] { new Latebind(), "InvokeMethod", new object[3] { new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "InvokeMethod" } }, "Extra Parametr", 123456 }).Throws(typeof(ArgumentNullException));////Method takes four parameters but five are passed


                // Provide string.empty values to Assembly, Class and Methods Name
                yield return new TestCaseData("TestID-0016A", string.Empty, "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));// pass Empty string to Assemblyname
                yield return new TestCaseData("TestID-0017A", "Public", string.Empty, "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException));// pass Empty string to Classname
                yield return new TestCaseData("TestID-0018A", "Public", "Touryo.Infrastructure.Public.Util.Latebind", string.Empty, new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));// pass Empty string to MethodName

                //Provide Null values to Assembly, Class and Methods Name
                yield return new TestCaseData("TestID-0019A", null, "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));// pass null to Assemblyname
                yield return new TestCaseData("TestID-0020A", "Public", null, "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));// pass null to Classname
                yield return new TestCaseData("TestID-0021A", "Public", "Touryo.Infrastructure.Public.Util.Latebind", null, new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));// pass null to MethodName
                yield return new TestCaseData("TestID-0022A", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", null).Throws(typeof(MissingMethodException));// pass null to Parameset
                
                #endregion 

            }
        }

        IEnumerable<TestCaseData> TestCasesOfInvokeMethod_NoErrTest
        {
            get
            {
                #region Normal TestCases
                //Pass All valid parameters and  assembly with full path.
                yield return new TestCaseData("TestID-001N", @"C:\root\Tests\Dll\Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" });
                yield return new TestCaseData("TestID-002N", @"C:\root\Tests\Dll\Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "WrongMethodName" });
                //Test case to cover AmbiguousMatchException..(Oveloading Methods)
                yield return new TestCaseData("TestID-003N", @"C:\root\Tests\Dll\Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[3] { new Latebind(), "InvokeMethod", new object[3] { new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "InvokeMethod" } } });

                //Pass All valid parameters(Only name of the Assembly)
                yield return new TestCaseData("TestID-004N", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" });
                yield return new TestCaseData("TestID-005N", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "WrongMethodName" });
                //Test case to cover AmbiguousMatchException..(Oveloading Methods)
                yield return new TestCaseData("TestID-006N", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[3] { new Latebind(), "InvokeMethod", new object[3] { new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "InvokeMethod" } } });

                yield return new TestCaseData("TestID-007N", "Public.Test", "Public.Test.Util.TestLateBind", "Display", new object[0] { }).Throws(typeof(ArgumentNullException));

                #endregion

                #region AbNormal TestCases
                // provide  wrong Assembly path name   to the method 
                yield return new TestCaseData("TestID-007A", @"C:\root\Tests\Dll\WrongPublic.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-008A", @"C:\root\Tests\Dll\Public.", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-009A", "AbcdAssembly", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));
                //Provide Wrong class Name
                yield return new TestCaseData("TestID-0010A", @"C:\root\Tests\Dll\Public.dll", "Touryo.Infrastructure.Public.Util.Latebind.cs", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-0011A", "Public", "WrongClassLatebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));
                // Provide Wrong Method Name 
                yield return new TestCaseData("TestID-0012A", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "WrongMethodName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));
                yield return new TestCaseData("TestID-0013A", @"C:\root\Tests\Dll\Public.dll", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByNameNotExist", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));

                // Provide Wrong number of parametrs to the Method
                yield return new TestCaseData("TestID-0014A", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[2] { new Latebind(), "InvokeMethod" }).Throws(typeof(ArgumentNullException));//Method takes two parameters but only two are passed
                yield return new TestCaseData("TestID-0015A", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "InvokeMethod", new object[5] { new Latebind(), "InvokeMethod", new object[3] { new Latebind(), "CheckTypeOfMethodByName", new object[2] { new Latebind(), "InvokeMethod" } }, "Extra Parametr", 123456 }).Throws(typeof(ArgumentNullException));////Method takes four parameters but five are passed


                // Provide string.empty values to Assembly, Class and Methods Name
                yield return new TestCaseData("TestID-0016A", string.Empty, "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(NUnit.Framework.AssertionException));// pass Empty string to Assemblyname
                yield return new TestCaseData("TestID-0017A", "Public", string.Empty, "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentException));// pass Empty string to Classname
                yield return new TestCaseData("TestID-0018A", "Public", "Touryo.Infrastructure.Public.Util.Latebind", string.Empty, new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));// pass Empty string to MethodName

                //Provide Null values to Assembly, Class and Methods Name
                yield return new TestCaseData("TestID-0019A", null, "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));// pass null to Assemblyname
                yield return new TestCaseData("TestID-0020A", "Public", null, "CheckTypeOfMethodByName", new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));// pass null to Classname
                yield return new TestCaseData("TestID-0021A", "Public", "Touryo.Infrastructure.Public.Util.Latebind", null, new object[2] { new Latebind(), "CheckTypeOfMethodByName" }).Throws(typeof(ArgumentNullException));// pass null to MethodName
                yield return new TestCaseData("TestID-0022A", "Public", "Touryo.Infrastructure.Public.Util.Latebind", "CheckTypeOfMethodByName", null).Throws(typeof(MissingMethodException));// pass null to Parameset

                #endregion

            }
        }
    }


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

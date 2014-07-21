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
//* クラス名        ：LogTest.cs
//* クラス日本語名  ：
//*
//* 作成者          ：Rituparna & Sai
//* 更新履歴        ：
//* 
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  07/17/2014   Sai               TestCode development for LofIF class.
//*  07/18/2014   Rituparna & Sai   Testcode development for LofIF and LogManager class.
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;

namespace Public.Test.Log
{
    [TestFixture]
    class LogTest
    {
        /// <summary>
        /// テスト前処理
        /// </summary>
        [TestFixtureSetUp]
        public void Init()
        {
        }
        /// <summary>
        /// テストケース前処理
        /// </summary>
        [SetUp]
        public void SetUp()
        {
        }
        /// <summary>
        /// TearDown Method
        /// </summary>
        [TearDown]
        public void TearDown()
        {

        }
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        #region TestCode

        /// <summary>
        /// DebugLog_Test Method(Test Case Execution)
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="loggerName">loggerName</param>
        [TestCase("Test message for DebugLog Of Access", "ACCESS", TestName = "TestID-000N")]
        [TestCase("Test message for DebugLog Of SQL Trace", "SQLTRACE", TestName = "TestID-001N")]
        [TestCase("Test message for DebugLog Of Operation", "OPERATION", TestName = "TestID-002N")]
        public void DebugLog_Test(string message, string loggerName)
        {
            try
            {
                LogIF.DebugLog(loggerName, message);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// InfoLog_Test Method
        /// </summary>
        /// <param name="loggerName"></param>
        /// <param name="message"></param>
        [TestCase("Test message for InfoLog Of Access", "ACCESS", TestName = "TestID-000N")]
        [TestCase("Test message for InfoLog Of SQL Trace", "SQLTRACE", TestName = "TestID-001N")]
        [TestCase("Test message for InfoLog Of Operation", "OPERATION", TestName = "TestID-002N")]
        public void InfoLog_Test(string loggerName, string message)
        {
            try
            {
                LogIF.InfoLog(loggerName, message);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// ErrorLog_Test Method(Test Case Execution)
        /// </summary>
        /// <param name="loggerName">loggerName</param>
        /// <param name="message">message</param>
        [TestCase("Test message for ErrorLog Of Access", "ACCESS", TestName = "TestID-000N")]
        [TestCase("Test message for ErrorLog Of SQL Trace", "SQLTRACE", TestName = "TestID-001N")]
        [TestCase("Test message for ErrorLog Of Operation", "OPERATION", TestName = "TestID-002N")]
        public void ErrorLog_Test(string loggerName, string message)
        {
            try
            {
                LogIF.ErrorLog(loggerName, message);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(":" + ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// FatalLog_Test Method(Test Case Execution)
        /// </summary>
        /// <param name="loggerName">loggerName</param>
        /// <param name="message">message</param>
        [TestCase("Test message for FatalLog Of Access", "ACCESS", TestName = "TestID-000N")]
        [TestCase("Test message for FatalLog Of SQL Trace", "SQLTRACE", TestName = "TestID-001N")]
        [TestCase("Test message for FatalLog Of Operation", "OPERATION", TestName = "TestID-002N")]
        public void FatalLog_Test(string loggerName, string message)
        {
            try
            {
                LogIF.FatalLog(loggerName, message);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// WarnLog_Test Method(Test Case Execution)
        /// </summary>
        /// <param name="loggerName">loggerName</param>
        /// <param name="message">message</param>
        [TestCase("Test message for WarnLog Of Access", "ACCESS", TestName = "TestID-000N")]
        [TestCase("Test message for WarnLog Of SQL Trace", "SQLTRACE", TestName = "TestID-001N")]
        [TestCase("Test message for WarnLog Of Operation", "OPERATION", TestName = "TestID-002N")]
        public void WarnLog_Test(string loggerName, string message)
        {
            try
            {
                LogIF.WarnLog(loggerName, message);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// GetLoggerLogLevel_Test Method(Test Case Execution)
        /// </summary>
        /// <param name="resultLoggerLogLevel">resultLoggerLogLevel</param>
        /// <param name="loggerName">loggerName</param>
        [TestCase("DEBUG", "ACCESS", TestName = "TestID-000N")]
        [TestCase("DEBUG", "SQLTRACE", TestName = "TestID-001N")]
        [TestCase("DEBUG", "OPERATION", TestName = "TestID-002N")]
        public void GetLoggerLogLevel_Test(string expectedLoggerLogLevel , string loggerName)
        {
            try
            {
                string resultLoggerLogLevel = LogIF.GetLoggerLogLevel(loggerName);

                Assert.AreEqual(resultLoggerLogLevel, expectedLoggerLogLevel);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// GetRootLoggerLogLevel_Test Method(Test Case Execution)
        /// </summary>
        /// <param name="resultRootLoggerLogLevel">resultRootLoggerLogLevel</param>
        /// <param name="loggerName">loggerName</param>
        [TestCase("DEBUG", "ACCESS", TestName = "TestID-000N")]
        [TestCase("DEBUG", "SQLTRACE", TestName = "TestID-001N")]
        [TestCase("DEBUG", "OPERATION", TestName = "TestID-002N")]
        public void GetRootLoggerLogLevel_Test(string expectedRootLoggerLogLevel, string loggerName)
        {
            try
            {
                string resultRootLoggerLogLevel = LogIF.GetRootLoggerLogLevel(loggerName);

                Assert.AreEqual(resultRootLoggerLogLevel, expectedRootLoggerLogLevel);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsDebugEnabled_Test Method(Test Case Execution)
        /// </summary>
        /// <param name="resultDebugEnabled">resultDebugEnabled</param>
        /// <param name="loggerName">loggerName</param>
        [TestCase(true, "ACCESS", TestName = "TestID-000N")]
        [TestCase(true, "SQLTRACE", TestName = "TestID-001N")]
        [TestCase(true, "OPERATION", TestName = "TestID-002N")]
        public void IsDebugEnabled_Test(bool expectedDebugEnabled, string loggerName)
        {
            try
            {
                bool resultDebugEnabled = LogIF.IsDebugEnabled(loggerName);
                Assert.AreEqual(resultDebugEnabled, expectedDebugEnabled);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsErrorEnabled_Test(Test Case Execution)
        /// </summary>
        /// <param name="resultErrorEnabled">resultErrorEnabled</param>
        /// <param name="loggerName">loggerName</param>
        [TestCase(true, "ACCESS", TestName = "TestID-000N")]
        [TestCase(true, "SQLTRACE", TestName = "TestID-001N")]
        [TestCase(true, "OPERATION", TestName = "TestID-002N")]
        public void IsErrorEnabled_Test(bool expectedErrorEnabled, string loggerName)
        {
            try
            {
                bool resultErrorEnabled = LogIF.IsErrorEnabled(loggerName);

                Assert.AreEqual(resultErrorEnabled, expectedErrorEnabled);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsFatalEnabled_Test Method(Test Case Execution)
        /// </summary>
        /// <param name="resultFatalEnabled">resultFatalEnabled</param>
        /// <param name="loggerName">loggerName</param>
        [TestCase(true, "ACCESS", TestName = "TestID-000N")]
        [TestCase(true, "SQLTRACE", TestName = "TestID-001N")]
        [TestCase(true, "OPERATION", TestName = "TestID-002N")]
        public void IsFatalEnabled_Test(bool expectedFatalEnabled ,string loggerName)
        {
            try
            {

                bool resultFatalEnabled = LogIF.IsFatalEnabled(loggerName);

                Assert.AreEqual(resultFatalEnabled, expectedFatalEnabled);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsInfoEnabled_Test Method(Test Case Execution)
        /// </summary>
        /// <param name="resultInfoEnabled">resultInfoEnabled</param>
        /// <param name="loggerName">loggerName</param>
        [TestCase(true, "ACCESS", TestName = "TestID-000N")]
        [TestCase(true, "SQLTRACE", TestName = "TestID-001N")]
        [TestCase(true, "OPERATION", TestName = "TestID-002N")]
        public void IsInfoEnabled_Test(bool expectedInfoEnabled,string loggerName)
        {
            try
            {
                bool resultInfoEnabled = LogIF.IsInfoEnabled(loggerName);

                Assert.AreEqual(resultInfoEnabled, expectedInfoEnabled);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// IsWarnEnabled_Test Method(Test Case Execution)
        /// </summary>
        /// <param name="resultWarnEnabled">resultWarnEnabled</param>
        /// <param name="loggerName">loggerName</param>
        [TestCase(true, "ACCESS", TestName = "TestID-000N")]
        [TestCase(true, "SQLTRACE", TestName = "TestID-001N")]
        [TestCase(true, "OPERATION", TestName = "TestID-002N")]
        public void IsWarnEnabled_Test(bool expectedWarnEnabled, string loggerName)
        {
            try
            {
                bool resultWarnEnabled = LogIF.IsWarnEnabled(loggerName);

                Assert.AreEqual(resultWarnEnabled, expectedWarnEnabled);
            }
            catch (Exception ex)
            {
                // Print a stack trace when an exception occurs.
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        #endregion
    }
}

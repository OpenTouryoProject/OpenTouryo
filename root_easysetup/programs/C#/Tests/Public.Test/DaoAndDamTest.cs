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
//* クラス名        ：CRUDTest.cs
//* クラス日本語名  ：
//*
//* 作成者          ：Rituparna & Santosh
//* 更新履歴        ：
//* 
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  06/13/2014   Rituparna & Santosh   Testcode development for CRUDTest(Framework classes).
//*  06/24/2014   Rituparna & Santosh   Testcode development for CRUDTest(Public classes).
//*  07/02/2014   Santosh               Added code and modified test cases to prevent database changes after running the test cases
//*  07/04/2014   Rituparna & Santosh   Added code and modified test cases to Increase the code coverage of BaseDam.cs Class
//*  07/10/2014   Rituparna             Added code and modified test cases to Increase the code coverage of BaseDam.cs Class
//*  08/11/2014   Rituparna             Added TestcaseID using SetName method as per Nishino-San comments
//**********************************************************************************
// 型情報
// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;
// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
//// 型情報
//using BDLayer.Test.Common;
//using BDLayer.Test.Business;
// testing framework
using NUnit.Framework;
using MyType;
using System.Reflection;
using System.Data.SqlClient;

namespace Public.Test
{

    [TestFixture]
    class DaoAndDamTest
    {
        /// <summary>
        /// テスト前処理
        /// </summary>
        [TestFixtureSetUp]
        public void Init()
        {
            testList.Clear();
            testList = GetListData();
            Identity = GetIdentityValue();
            Console.WriteLine("これはテスト前処理です。最初に一度だけ実行されます。");
        }

        /// <summary>
        /// テストケース前処理
        /// </summary>
        [SetUp]
        public void SetUp()
        {

            Console.WriteLine("これはテストケース前処理です。テストケースごとに実行されます。");

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
            //Executes Last after all tests have run.
            DeleteData();
        }

        List<int> testList = new List<int>();
        int Identity = 0;

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method SampleScreen_CRUD_Test.
        /// </summary>
        public IEnumerable<TestCaseData> TestSampleScreen_DaoAndDam_Test
        {
            get
            {
                /*SelectCount*/
                yield return new TestCaseData("screen1", "control1", "SelectCount", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).SetName("TestID-000N");
                yield return new TestCaseData("screen2", "control2", "SelectCount", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", null, null, null).SetName("TestID-001N");
                yield return new TestCaseData("screen3", "control3", "SelectCount", "SQL%common%static%-", "User3", "Hostname3", "RC", null, null, null).SetName("TestID-002N");
                yield return new TestCaseData("screen4", "control4", "SelectCount", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", null, null, null).SetName("TestID-003N");
                yield return new TestCaseData("screen1", "control1", "SelectCount", "SQL%individual%static%-", "User1", "Hostname1", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-004A");
                yield return new TestCaseData("screen2", "control2", "SelectCount", "SQL%individual%dynamic%-", "User2", "Hostname2", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-005A");
                yield return new TestCaseData("screen3", "control3", "SelectCount", "SQL%common%static%-", "User3", "Hostname3", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-006A");
                yield return new TestCaseData("screen4", "control4", "SelectCount", "SQL%common%dynamic%-", "User4", "Hostname4", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-007A");
                yield return new TestCaseData("screen1", "control1", "SelectCount", "SQL%individual%static%-", "User1", "Hostname1", "NT", null, null, null).SetName("TestID-008N");
                yield return new TestCaseData("screen2", "control2", "SelectCount", "SQL%individual%dynamic%-", "User2", "Hostname2", "NT", null, null, null).SetName("TestID-009N");
                yield return new TestCaseData("screen3", "control3", "SelectCount", "SQL%common%static%-", "User3", "Hostname3", "NT", null, null, null).SetName("TestID-010N");
                yield return new TestCaseData("screen4", "control4", "SelectCount", "SQL%common%dynamic%-", "User4", "Hostname4", "NT", null, null, null).SetName("TestID-011N");
                yield return new TestCaseData("screen1", "control1", "SelectCount", "SQL%individual%static%-", "User1", "Hostname1", "RU", null, null, null).SetName("TestID-012N");
                yield return new TestCaseData("screen2", "control2", "SelectCount", "SQL%individual%dynamic%-", "User2", "Hostname2", "RU", null, null, null).SetName("TestID-013N");
                yield return new TestCaseData("screen3", "control3", "SelectCount", "SQL%common%static%-", "User3", "Hostname3", "RU", null, null, null).SetName("TestID-014N");
                yield return new TestCaseData("screen4", "control4", "SelectCount", "SQL%common%dynamic%-", "User4", "Hostname4", "RU", null, null, null).SetName("TestID-015N");
                yield return new TestCaseData("screen1", "control1", "SelectCount", "SQL%individual%static%-", "User1", "Hostname1", "RR", null, null, null).SetName("TestID-016N");
                yield return new TestCaseData("screen2", "control2", "SelectCount", "SQL%individual%dynamic%-", "User2", "Hostname2", "RR", null, null, null).SetName("TestID-017N");
                yield return new TestCaseData("screen3", "control3", "SelectCount", "SQL%common%static%-", "User3", "Hostname3", "RR", null, null, null).SetName("TestID-018N");
                yield return new TestCaseData("screen4", "control4", "SelectCount", "SQL%common%dynamic%-", "User4", "Hostname4", "RR", null, null, null).SetName("TestID-019N");
                yield return new TestCaseData("screen1", "control1", "SelectCount", "SQL%individual%static%-", "User1", "Hostname1", "SS", null, null, null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-020A");
                yield return new TestCaseData("screen2", "control2", "SelectCount", "SQL%individual%dynamic%-", "User2", "Hostname2", "SS", null, null, null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-021A");
                yield return new TestCaseData("screen3", "control3", "SelectCount", "SQL%common%static%-", "User3", "Hostname3", "SS", null, null, null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-022A");
                yield return new TestCaseData("screen4", "control4", "SelectCount", "SQL%common%dynamic%-", "User4", "Hostname4", "SS", null, null, null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-023A");
                yield return new TestCaseData("screen1", "control1", "SelectCount", "SQL%individual%static%-", "User1", "Hostname1", "DF", null, null, null).SetName("TestID-036N");
                yield return new TestCaseData("screen2", "control2", "SelectCount", "SQL%individual%dynamic%-", "User2", "Hostname2", "DF", null, null, null).SetName("TestID-037N");
                yield return new TestCaseData("screen3", "control3", "SelectCount", "SQL%common%static%-", "User3", "Hostname3", "DF", null, null, null).SetName("TestID-038N");
                yield return new TestCaseData("screen4", "control4", "SelectCount", "SQL%common%dynamic%-", "User4", "Hostname4", "DF", null, null, null).SetName("TestID-039N");
                yield return new TestCaseData("screen6", "control6", "SelectCount", "", "", "", "DF", null, null, null).Throws(typeof(IndexOutOfRangeException)).SetName("TestID-040A");
                /*SelectCount*/

                /*SelectAll_DT*/
                 yield return new TestCaseData("screen1", "control1", "SelectAll_DT", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).SetName("TestID-041N");
                 yield return new TestCaseData("screen2", "control2", "SelectAll_DT", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", null, null, null).SetName("TestID-042N");
                 yield return new TestCaseData("screen3", "control3", "SelectAll_DT", "SQL%common%static%-", "User3", "Hostname3", "RC", null, null, null).SetName("TestID-043N");
                 yield return new TestCaseData("screen4", "control4", "SelectAll_DT", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", null, null, null).SetName("TestID-044N");
                 yield return new TestCaseData("screen1", "control1", "SelectAll_DT", "SQL%individual%static%-", "User1", "Hostname1", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-045A");
                 yield return new TestCaseData("screen2", "control2", "SelectAll_DT", "SQL%individual%dynamic%-", "User2", "Hostname2", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-046A");
                 yield return new TestCaseData("screen3", "control3", "SelectAll_DT", "SQL%common%static%-", "User3", "Hostname3", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-047A");
                 yield return new TestCaseData("screen4", "control4", "SelectAll_DT", "SQL%common%dynamic%-", "User4", "Hostname4", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-048N");
                 yield return new TestCaseData("screen1", "control1", "SelectAll_DT", "SQL%individual%static%-", "User1", "Hostname1", "NT", null, null, null).SetName("TestID-049N");
                 yield return new TestCaseData("screen2", "control2", "SelectAll_DT", "SQL%individual%dynamic%-", "User2", "Hostname2", "RU", null, null, null).SetName("TestID-050N");
                 yield return new TestCaseData("screen3", "control3", "SelectAll_DT", "SQL%common%static%-", "User3", "Hostname3", "RR", null, null, null).SetName("TestID-051N");
                 yield return new TestCaseData("screen4", "control4", "SelectAll_DT", "SQL%common%dynamic%-", "User4", "Hostname4", "SS", null, null, null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-052A");
                 yield return new TestCaseData("screen1", "control4", "", "SQL%generate%static%-", "User5", "Hostname5", "DF", null, null, null).SetName("TestID-053N");
                 yield return new TestCaseData("screen1", "control4", "", "", "User5", "Hostname5", "DF", null, null, null).SetName("TestID-054N");
                 yield return new TestCaseData("", "control4", "", "", "User5", "Hostname5", "RC", null, null, null).SetName("TestID-055N");
                 /*SelectAll_DT*/

                /*SelectAll_DR*/
                 yield return new TestCaseData("screen1", "control1", "SelectAll_DR", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).SetName("TestID-056N");
                yield return new TestCaseData("screen2", "control2", "SelectAll_DR", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", null, null, null).SetName("TestID-057N");
                yield return new TestCaseData("screen3", "control3", "SelectAll_DR", "SQL%common%static%-", "User3", "Hostname3", "RC", null, null, null).SetName("TestID-058N");
                yield return new TestCaseData("screen4", "control4", "SelectAll_DR", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", null, null, null).SetName("TestID-059N");


                yield return new TestCaseData("screen1", "control1", "SelectAll_DR", "SQL%individual%static%-", "User1", "Hostname1", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-060A");
                yield return new TestCaseData("screen2", "control2", "SelectAll_DR", "SQL%individual%dynamic%-", "User2", "Hostname2", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-061A");
                yield return new TestCaseData("screen3", "control3", "SelectAll_DR", "SQL%common%static%-", "User3", "Hostname3", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-062A");
                yield return new TestCaseData("screen4", "control4", "SelectAll_DR", "SQL%common%dynamic%-", "User4", "Hostname4", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-063A");
                yield return new TestCaseData("screen1", "control1", "SelectAll_DR", "SQL%individual%static%-", "User1", "Hostname1", "NT", null, null, null).SetName("TestID-063N");
                yield return new TestCaseData("screen2", "control2", "SelectAll_DR", "SQL%individual%dynamic%-", "User2", "Hostname2", "RU", null, null, null).SetName("TestID-064N");
                yield return new TestCaseData("screen3", "control3", "SelectAll_DR", "SQL%common%static%-", "User3", "Hostname3", "RR", null, null, null).SetName("TestID-065N");
                yield return new TestCaseData("screen4", "control4", "SelectAll_DR", "SQL%individual%static%-", "User4", "Hostname4", "SS", null, null, null).Throws(typeof(System.InvalidOperationException)).SetName("TestID-066N");
                yield return new TestCaseData("screen1", "control4", "", "", "User5", "Hostname5", "DF", null, null, null).SetName("TestID-067N");
                yield return new TestCaseData("", "control4", "", "", "User5", "Hostname5", "RC", null, null, null).SetName("TestID-068N");
                /*SelectAll_DR*/

                /*SelectAll_DSQL*/
                 yield return new TestCaseData("screen1", "control1", "SelectAll_DSQL", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).SetName("TestID-069N");
                 yield return new TestCaseData("screen2", "control2", "SelectAll_DSQL", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", null, null, null).SetName("TestID-070N");
                 yield return new TestCaseData("screen3", "control3", "SelectAll_DSQL", "SQL%common%static%-", "User3", "Hostname3", "RC", null, null, null).SetName("TestID-071N");
                 yield return new TestCaseData("screen4", "control4", "SelectAll_DSQL", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", null, null, null).SetName("TestID-072N");
                 yield return new TestCaseData("screen1", "control1", "SelectAll_DSQL", "SQL%individual%static%-", "User1", "Hostname1", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-073A");
                 yield return new TestCaseData("screen2", "control2", "SelectAll_DSQL", "SQL%individual%dynamic%-", "User2", "Hostname2", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-074A");
                 yield return new TestCaseData("screen3", "control3", "SelectAll_DSQL", "SQL%common%static%-", "User3", "Hostname3", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-075A");
                 yield return new TestCaseData("screen4", "control4", "SelectAll_DSQL", "SQL%common%dynamic%-", "User4", "Hostname4", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-076A");
                 yield return new TestCaseData("screen1", "control1", "SelectAll_DSQL", "SQL%individual%static%-", "User1", "Hostname1", "NT", null, null, null).SetName("TestID-077N");
                 yield return new TestCaseData("screen2", "control2", "SelectAll_DSQL", "SQL%individual%dynamic%-", "User2", "Hostname2", "RU", null, null, null).SetName("TestID-078N");
                 yield return new TestCaseData("screen3", "control3", "SelectAll_DSQL", "SQL%common%static%-", "User3", "Hostname3", "RR", null, null, null).SetName("TestID-079N");
                 yield return new TestCaseData("screen4", "control4", "SelectAll_DSQL", "SQL%common%dynamic%-", "User4", "Hostname4", "SS", null, null, null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-080A");
                 yield return new TestCaseData("screen1", "control4", "", "", "User5", "Hostname5", "DF", null, null, null).SetName("TestID-081N");
                 yield return new TestCaseData("", "control4", "", "", "User5", "Hostname5", "RC", null, null, null).SetName("TestID-082N");
                 /*SelectAll_DSQL*/

                /*SelectAll_DS*/
                yield return new TestCaseData("screen1", "control1", "SelectAll_DS", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).SetName("TestID-083N");
                yield return new TestCaseData("screen2", "control2", "SelectAll_DS", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", null, null, null).SetName("TestID-084N");
                yield return new TestCaseData("screen3", "control3", "SelectAll_DS", "SQL%common%static%-", "User3", "Hostname3", "RC", null, null, null).SetName("TestID-085N");
                yield return new TestCaseData("screen4", "control4", "SelectAll_DS", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", null, null, null).SetName("TestID-086N");
                yield return new TestCaseData("screen1", "control1", "SelectAll_DS", "SQL%individual%static%-", "User1", "Hostname1", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-087A");
                yield return new TestCaseData("screen2", "control2", "SelectAll_DS", "SQL%individual%dynamic%-", "User2", "Hostname2", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-088A");
                yield return new TestCaseData("screen3", "control3", "SelectAll_DS", "SQL%common%static%-", "User3", "Hostname3", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-089A");
                yield return new TestCaseData("screen4", "control4", "SelectAll_DS", "SQL%common%dynamic%-", "User4", "Hostname4", "NC", null, null, null).Throws(typeof(NullReferenceException)).SetName("TestID-090A");
                yield return new TestCaseData("screen1", "control1", "SelectAll_DS", "SQL%individual%static%-", "User1", "Hostname1", "NT", null, null, null).SetName("TestID-091N");
                yield return new TestCaseData("screen2", "control2", "SelectAll_DS", "SQL%individual%dynamic%-", "User2", "Hostname2", "RU", null, null, null).SetName("TestID-092N");
                yield return new TestCaseData("screen3", "control3", "SelectAll_DS", "SQL%common%static%-", "User3", "Hostname3", "RR", null, null, null).SetName("TestID-093N");
                yield return new TestCaseData("screen4", "control4", "SelectAll_DS", "SQL%common%dynamic%-", "User4", "Hostname4", "SS", null, null, null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-094A");
                yield return new TestCaseData("screen1", "control4", "", "", "User5", "Hostname5", "DF", null, null, null).SetName("TestID-095N");
                yield return new TestCaseData("", "control4", "", "", "User5", "Hostname5", "RC", null, null, null).SetName("TestID-096N");
                /*SelectAll_DS*/

                /*Select*/
                yield return new TestCaseData("screen1", "control1", "Select", "SQL%individual%static%-", "User1", "Hostname1", "RC", "1", null, null).SetName("TestID-097N");
                yield return new TestCaseData("screen2", "control2", "Select", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", "2", null, null).SetName("TestID-098N");
                yield return new TestCaseData("screen3", "control3", "Select", "SQL%common%static%-", "User3", "Hostname3", "RC", "3", null, null).SetName("TestID-099N");
                yield return new TestCaseData("screen4", "control4", "Select", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", "1", null, null).SetName("TestID-100A");
                yield return new TestCaseData("screen1", "control1", "Select", "SQL%individual%static%-", "User1", "Hostname1", "NC", "1", null, null).Throws(typeof(NullReferenceException)).SetName("TestID-101A");
                yield return new TestCaseData("screen2", "control2", "Select", "SQL%individual%dynamic%-", "User2", "Hostname2", "NC", "1", null, null).Throws(typeof(NullReferenceException)).SetName("TestID-102A");
                yield return new TestCaseData("screen3", "control3", "Select", "SQL%common%static%-", "User3", "Hostname3", "NC", "1", null, null).Throws(typeof(NullReferenceException)).SetName("TestID-103A");
                yield return new TestCaseData("screen4", "control4", "Select", "SQL%common%dynamic%-", "User4", "Hostname4", "NC", "2", null, null).Throws(typeof(NullReferenceException)).SetName("TestID-104A");
                yield return new TestCaseData("screen1", "control1", "Select", "SQL%individual%static%-", "User1", "Hostname1", "NT", "1", null, null).SetName("TestID-105N");
                yield return new TestCaseData("screen2", "control2", "Select", "SQL%individual%dynamic%-", "User2", "Hostname2", "RU", "3", null, null).SetName("TestID-106N");
                yield return new TestCaseData("screen3", "control3", "Select", "SQL%common%static%-", "User3", "Hostname3", "RR", "2", null, null).SetName("TestID-107N");
                yield return new TestCaseData("screen4", "control4", "Select", "SQL%common%dynamic%-", "User4", "Hostname4", "SS", "1", null, null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-108A");
                yield return new TestCaseData("screen1", "control4", "", "", "User5", "Hostname5", "DF", "1", null, null).SetName("TestID-109N");
                yield return new TestCaseData("", "control4", "", "", "User5", "Hostname5", "RC", "1", null, null).SetName("TestID-110N");
                /*Select*/

                /*Insert*/
                 this.Init();
                 this.SetUp();
                 yield return new TestCaseData("screen1", "control1", "Insert", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, "Hitachi", "00180099666").SetName("TestID-111N");
                 yield return new TestCaseData("screen2", "control2", "Insert", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", null, "Hitachi1", "001800996662").SetName("TestID-112N");
                 yield return new TestCaseData("screen3", "control3", "Insert", "SQL%common%static%-", "User3", "Hostname3", "RC", "4", "Hitachi2", "001800996626").SetName("TestID-113N");
                 yield return new TestCaseData("screen4", "control4", "Insert", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", "5", "Hitachi4", "001800996626").SetName("TestID-114A");
                 yield return new TestCaseData("screen1", "control1", "Insert", "SQL%individual%static%-", "User1", "Hostname1", "NC", "1", null, null).Throws(typeof(NullReferenceException)).SetName("TestID-115A");
                 yield return new TestCaseData("screen2", "control2", "Insert", "SQL%individual%dynamic%-", "User2", "Hostname2", "NC", "1", null, null).Throws(typeof(NullReferenceException)).SetName("TestID-116A");
                 yield return new TestCaseData("screen3", "control3", "Insert", "SQL%common%static%-", "User3", "Hostname3", "NC", "1", null, null).Throws(typeof(NullReferenceException)).SetName("TestID-117A");
                 yield return new TestCaseData("screen4", "control4", "Insert", "SQL%common%dynamic%-", "User4", "Hostname4", "NC", "2", null, null).Throws(typeof(NullReferenceException)).SetName("TestID-118A");
                 yield return new TestCaseData("screen1", "control1", "Insert", "SQL%individual%static%-", "User1", "Hostname1", "NT", "6", "Hitachi5", "001800996626").SetName("TestID-119N");
                 yield return new TestCaseData("screen2", "control2", "Insert", "SQL%individual%dynamic%-", "User2", "Hostname2", "RU", null, "Hitachi6", "001800996626").SetName("TestID-120N");
                 yield return new TestCaseData("screen3", "control3", "Insert", "SQL%common%static%-", "User3", "Hostname3", "RR", "7", "Hitachi7", "001800996626").SetName("TestID-121N");
                 yield return new TestCaseData("screen4", "control4", "Insert", "SQL%common%dynamic%-", "User4", "Hostname4", "SS", null, null, null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-122A");
                 yield return new TestCaseData("screen1", "control4", "", "", "User5", "Hostname5", "DF", "9", null, null).SetName("TestID-123N");
                 yield return new TestCaseData("", "control4", "", "", "User5", "Hostname5", "RC", "10", null, null).SetName("TestID-124N");

                 /* Update*/
                 yield return new TestCaseData("screen1", "control1", "Update", "SQL%individual%static%-", "User1", "Hostname1", "RC", (Identity + 1).ToString(), "Company_NameUpdate", "125987").SetName("TestID-125N");
                 yield return new TestCaseData("screen2", "control2", "Update", "SQL%individual%static%-", "User2", "Hostname2", "RC", (Identity + 2).ToString(), "Company_NameUpdate", "987456").SetName("TestID-126N");
                 yield return new TestCaseData("screen3", "control3", "Update", "SQL%individual%static%-", "User3", "Hostname3", "RC", (Identity + 3).ToString(), "Company_NameUpdate", "125987").SetName("TestID-127N");
                 yield return new TestCaseData("screen4", "control4", "Update", "SQL%individual%static%-", "User4", "Hostname4", "RC", (Identity + 4).ToString(), "Company_NameUpdate", "").SetName("TestID-128N");
                 yield return new TestCaseData("screen5", "control5", "Update", "SQL%individual%static%-", "User5", "Hostname5", "RC", (Identity + 1).ToString(), "Company_NameUpdate", null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-129A");
                 yield return new TestCaseData("screen6", "control6", "Update", "SQL%individual%static%-", "User6", "Hostname6", "RC", (Identity + 1).ToString(), null, "20042360").Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-130A");
                 yield return new TestCaseData("screen7", "control7", "Update", "SQL%individual%static%-", "User7", "Hostname7", "RC", null, "Company1_update", "20042360").Throws(typeof(ArgumentNullException)).SetName("TestID-131A");
                 yield return new TestCaseData("screen8", "control8", "Update", "SQL%individual%static%-", "User8", "Hostname8", "RC", null, null, null).Throws(typeof(ArgumentNullException)).SetName("TestID-132A");
                 yield return new TestCaseData("screen9", "control9", "Update", "SQL%individual%static%-", "User9", "Hostname9", "RC", "", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-133A");
                 yield return new TestCaseData("screen10", "control10", "Update", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-134A");
                 yield return new TestCaseData("screen1", "control1", "Update", "SQL%individual%static%-", "User1", "Hostname1", "NC", (Identity + 1).ToString(), "Company_NameUpdate", "125987").Throws(typeof(NullReferenceException)).SetName("TestID-135A");
                 yield return new TestCaseData("screen2", "control2", "Update", "SQL%individual%static%-", "User2", "Hostname2", "NC", (Identity + 2).ToString(), "Company2_update", "987456").Throws(typeof(NullReferenceException)).SetName("TestID-136A");
                 yield return new TestCaseData("screen3", "control3", "Update", "SQL%individual%static%-", "User3", "Hostname3", "NC", (Identity + 3).ToString(), "", "125987").Throws(typeof(NullReferenceException)).SetName("TestID-137A");
                 yield return new TestCaseData("screen4", "control4", "Update", "SQL%individual%static%-", "User4", "Hostname4", "NC", (Identity + 4).ToString(), "Company3_update", "").Throws(typeof(NullReferenceException)).SetName("TestID-138A");
                 yield return new TestCaseData("screen5", "control5", "Update", "SQL%individual%static%-", "User5", "Hostname5", "NC", (Identity + 5).ToString(), "Company1_update", null).Throws(typeof(NullReferenceException)).SetName("TestID-139A");
                 yield return new TestCaseData("screen6", "control6", "Update", "SQL%individual%static%-", "User6", "Hostname6", "NC", (Identity + 6).ToString(), null, "20042360").Throws(typeof(NullReferenceException)).SetName("TestID-140A");
                 yield return new TestCaseData("screen7", "control7", "Update", "SQL%individual%static%-", "User7", "Hostname7", "NC", null, "Company1_update", "20042360").Throws(typeof(ArgumentNullException)).SetName("TestID-141A");
                 yield return new TestCaseData("screen8", "control8", "Update", "SQL%individual%static%-", "User8", "Hostname8", "NC", null, null, null).Throws(typeof(ArgumentNullException)).SetName("TestID-142A");
                 yield return new TestCaseData("screen9", "control9", "Update", "SQL%individual%static%-", "User9", "Hostname9", "NC", "", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-143A");
                 yield return new TestCaseData("screen10", "control10", "Update", "SQL%individual%static%-", "User10", "Hostname10", "NC", "12N", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-144A");
                 yield return new TestCaseData("screen1", "control1", "Update", "SQL%individual%static%-", "User1", "Hostname1", "NT", (Identity + 1).ToString(), "Company_NameUpdated", "125987").SetName("TestID-145N");
                 yield return new TestCaseData("screen2", "control2", "Update", "SQL%individual%static%-", "User2", "Hostname2", "NT", (Identity + 4).ToString(), "Company_NameUpdated", "987456").SetName("TestID-146N");
                 yield return new TestCaseData("screen3", "control3", "Update", "SQL%individual%static%-", "User3", "Hostname3", "NT", (Identity + 5).ToString(), "", "125987").SetName("TestID-147N");
                 yield return new TestCaseData("screen4", "control4", "Update", "SQL%individual%static%-", "User4", "Hostname4", "NT", (Identity + 6).ToString(), "Company_NameUpdated", "").SetName("TestID-148N");
                 yield return new TestCaseData("screen5", "control5", "Update", "SQL%individual%static%-", "User5", "Hostname5", "NT", "1", "Company1_update", null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-149A");
                 yield return new TestCaseData("screen6", "control6", "Update", "SQL%individual%static%-", "User6", "Hostname6", "NT", "1", null, "20042360").Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-150A");
                 yield return new TestCaseData("screen7", "control7", "Update", "SQL%individual%static%-", "User7", "Hostname7", "NT", null, "Company1_update", "20042360").Throws(typeof(ArgumentNullException)).SetName("TestID-151A");
                 yield return new TestCaseData("screen8", "control8", "Update", "SQL%individual%static%-", "User8", "Hostname8", "NT", null, null, null).Throws(typeof(ArgumentNullException)).SetName("TestID-152A");
                 yield return new TestCaseData("screen9", "control9", "Update", "SQL%individual%static%-", "User9", "Hostname9", "NT", "", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-153A");
                 yield return new TestCaseData("screen10", "control10", "Update", "SQL%individual%static%-", "User10", "Hostname10", "NT", "12N", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-154A");
                 yield return new TestCaseData("screen1", "control1", "Update", "SQL%individual%static%-", "User1", "Hostname1", "RU", (Identity + 1).ToString(), "Company_NameUpdated", "125987").SetName("TestID-155N");
                 yield return new TestCaseData("screen2", "control2", "Update", "SQL%individual%static%-", "User2", "Hostname2", "RU", (Identity + 1).ToString(), "Company_NameUpdated", "987456").SetName("TestID-156N");
                 yield return new TestCaseData("screen3", "control3", "Update", "SQL%individual%static%-", "User3", "Hostname3", "RU", (Identity + 1).ToString(), "", "125987").SetName("TestID-157N");
                 yield return new TestCaseData("screen4", "control4", "Update", "SQL%individual%static%-", "User4", "Hostname4", "RU", (Identity + 1).ToString(), "Company_NameUpdated", "").SetName("TestID-158N");
                 yield return new TestCaseData("screen5", "control5", "Update", "SQL%individual%static%-", "User5", "Hostname5", "RU", "1", "Company1_update", null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-159A");
                 yield return new TestCaseData("screen6", "control6", "Update", "SQL%individual%static%-", "User6", "Hostname6", "RU", "1", null, "20042360").Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-160A");
                 yield return new TestCaseData("screen7", "control7", "Update", "SQL%individual%static%-", "User7", "Hostname7", "RU", null, "Company1_update", "20042360").Throws(typeof(ArgumentNullException)).SetName("TestID-161A");
                 yield return new TestCaseData("screen8", "control8", "Update", "SQL%individual%static%-", "User8", "Hostname8", "RU", null, null, null).Throws(typeof(ArgumentNullException)).SetName("TestID-162A");
                 yield return new TestCaseData("screen9", "control9", "Update", "SQL%individual%static%-", "User9", "Hostname9", "RU", "", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-163A");
                 yield return new TestCaseData("screen10", "control10", "Update", "SQL%individual%static%-", "User10", "Hostname10", "RU", "12N", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-164N");
                 yield return new TestCaseData("screen1", "control1", "Update", "SQL%individual%static%-", "User1", "Hostname1", "RU", (Identity + 1).ToString(), "Company_NameUpdated", "125987").SetName("TestID-165N");
                 yield return new TestCaseData("screen2", "control2", "Update", "SQL%individual%static%-", "User2", "Hostname2", "RU", (Identity + 7).ToString(), "Company_NameUpdated", "987456").SetName("TestID-166N");
                 yield return new TestCaseData("screen3", "control3", "Update", "SQL%individual%static%-", "User3", "Hostname3", "RU", (Identity + 9).ToString(), "", "125987").SetName("TestID-167N");
                 yield return new TestCaseData("screen4", "control4", "Update", "SQL%individual%static%-", "User4", "Hostname4", "RU", (Identity + 10).ToString(), "Company_NameUpdated", "").SetName("TestID-168N");
                 yield return new TestCaseData("screen5", "control5", "Update", "SQL%individual%static%-", "User5", "Hostname5", "RU", "1", "Company1_update", null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-169A");
                 yield return new TestCaseData("screen6", "control6", "Update", "SQL%individual%static%-", "User6", "Hostname6", "RU", "1", null, "20042360").Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-170A");
                 yield return new TestCaseData("screen7", "control7", "Update", "SQL%individual%static%-", "User7", "Hostname7", "RU", null, "Company1_update", "20042360").Throws(typeof(ArgumentNullException)).SetName("TestID-171A");
                 yield return new TestCaseData("screen8", "control8", "Update", "SQL%individual%static%-", "User8", "Hostname8", "RU", null, null, null).Throws(typeof(ArgumentNullException)).SetName("TestID-172A");
                 yield return new TestCaseData("screen9", "control9", "Update", "SQL%individual%static%-", "User9", "Hostname9", "RU", "", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-173A");
                 yield return new TestCaseData("screen10", "control10", "Update", "SQL%individual%static%-", "User10", "Hostname10", "RU", "12N", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-174A");
                 yield return new TestCaseData("screen1", "control1", "Update", "SQL%individual%static%-", "User1", "Hostname1", "RR", (Identity + 1).ToString(), "Company_NameUpdated", "125987").SetName("TestID-175N");
                 yield return new TestCaseData("screen2", "control2", "Update", "SQL%individual%static%-", "User2", "Hostname2", "RR", (Identity + 2).ToString(), "Company_NameUpdated", "987456").SetName("TestID-176N");
                 yield return new TestCaseData("screen3", "control3", "Update", "SQL%individual%static%-", "User3", "Hostname3", "RR", (Identity + 8).ToString(), "", "125987").SetName("TestID-177N");
                 yield return new TestCaseData("screen4", "control4", "Update", "SQL%individual%static%-", "User4", "Hostname4", "RR", (Identity + 1).ToString(), "Company_NameUpdated", "").SetName("TestID-178N");
                 yield return new TestCaseData("screen5", "control5", "Update", "SQL%individual%static%-", "User5", "Hostname5", "RR", "1", "Company1_update", null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-179A");
                 yield return new TestCaseData("screen6", "control6", "Update", "SQL%individual%static%-", "User6", "Hostname6", "RR", "1", null, "20042360").Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-180A");
                 yield return new TestCaseData("screen7", "control7", "Update", "SQL%individual%static%-", "User7", "Hostname7", "RR", null, "Company1_update", "20042360").Throws(typeof(ArgumentNullException)).SetName("TestID-181A");
                 yield return new TestCaseData("screen8", "control8", "Update", "SQL%individual%static%-", "User8", "Hostname8", "RR", null, null, null).Throws(typeof(ArgumentNullException)).SetName("TestID-182A");
                 yield return new TestCaseData("screen9", "control9", "Update", "SQL%individual%static%-", "User9", "Hostname9", "RR", "", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-183A");
                 yield return new TestCaseData("screen10", "control10", "Update", "SQL%individual%static%-", "User10", "Hostname10", "RR", "12N", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-184A");
                 yield return new TestCaseData("screen1", "control1", "Update", "SQL%individual%static%-", "User1", "Hostname1", "SZ", (Identity + 1).ToString(), "Company_NameUpdated", "125987").SetName("TestID-185N");
                 yield return new TestCaseData("screen2", "control2", "Update", "SQL%individual%static%-", "User2", "Hostname2", "SZ", (Identity + 7).ToString(), "Company_NameUpdated", "987456").SetName("TestID-186N");
                 yield return new TestCaseData("screen3", "control3", "Update", "SQL%individual%static%-", "User3", "Hostname3", "SZ", (Identity + 6).ToString(), "", "125987").SetName("TestID-187N");
                 yield return new TestCaseData("screen4", "control4", "Update", "SQL%individual%static%-", "User4", "Hostname4", "SZ", (Identity + 15).ToString(), "Company_NameUpdated", "").SetName("TestID-188N");
                 yield return new TestCaseData("screen5", "control5", "Update", "SQL%individual%static%-", "User5", "Hostname5", "SZ", "1", "Company1_update", null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-189A");
                 yield return new TestCaseData("screen6", "control6", "Update", "SQL%individual%static%-", "User6", "Hostname6", "SZ", "1", null, "20042360").Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-190A");
                 yield return new TestCaseData("screen7", "control7", "Update", "SQL%individual%static%-", "User7", "Hostname7", "SZ", null, "Company1_update", "20042360").Throws(typeof(ArgumentNullException)).SetName("TestID-191A");
                 yield return new TestCaseData("screen8", "control8", "Update", "SQL%individual%static%-", "User8", "Hostname8", "SZ", null, null, null).Throws(typeof(ArgumentNullException)).SetName("TestID-192A");
                 yield return new TestCaseData("screen9", "control9", "Update", "SQL%individual%static%-", "User9", "Hostname9", "SZ", "", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-193A");
                 yield return new TestCaseData("screen10", "control10", "Update", "SQL%individual%static%-", "User10", "Hostname10", "SZ", "12N", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-194A");
                 yield return new TestCaseData("screen1", "control1", "Update", "SQL%individual%static%-", "User1", "Hostname1", "SS", "1", "Company1_update", "125987").Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-195A");
                 yield return new TestCaseData("screen2", "control2", "Update", "SQL%individual%static%-", "User2", "Hostname2", "SS", "2", "Company2_update", "987456").Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-196A");
                 yield return new TestCaseData("screen3", "control3", "Update", "SQL%individual%static%-", "User3", "Hostname3", "SS", "3", "", "125987").Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-197A");
                 yield return new TestCaseData("screen4", "control4", "Update", "SQL%individual%static%-", "User4", "Hostname4", "SS", "3", "Company3_update", "").Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-198A");
                 yield return new TestCaseData("screen5", "control5", "Update", "SQL%individual%static%-", "User5", "Hostname5", "SS", "1", "Company1_update", null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-199A");
                 yield return new TestCaseData("screen6", "control6", "Update", "SQL%individual%static%-", "User6", "Hostname6", "SS", "1", null, "20042360").Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-200A");
                 yield return new TestCaseData("screen7", "control7", "Update", "SQL%individual%static%-", "User7", "Hostname7", "SS", null, "Company1_update", "20042360").Throws(typeof(ArgumentNullException)).SetName("TestID-201A");
                 yield return new TestCaseData("screen8", "control8", "Update", "SQL%individual%static%-", "User8", "Hostname8", "SS", null, null, null).Throws(typeof(ArgumentNullException)).SetName("TestID-202A");
                 yield return new TestCaseData("screen9", "control9", "Update", "SQL%individual%static%-", "User9", "Hostname9", "SS", "", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-203A");
                 yield return new TestCaseData("screen10", "control10", "Update", "SQL%individual%static%-", "User10", "Hostname10", "SS", "12N", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-204A");
                 yield return new TestCaseData("screen1", "control1", "Update", "SQL%individual%static%-", "User1", "Hostname1", "DF", (Identity + 1).ToString(), "Company_NameUpdated", "125987").SetName("TestID-205N");
                 yield return new TestCaseData("screen2", "control2", "Update", "SQL%individual%static%-", "User2", "Hostname2", "DF", (Identity + 2).ToString(), "Company_NameUpdated", "987456").SetName("TestID-206N");
                 yield return new TestCaseData("screen3", "control3", "Update", "SQL%individual%static%-", "User3", "Hostname3", "DF", (Identity + 3).ToString(), "", "125987").SetName("TestID-207N");
                 yield return new TestCaseData("screen4", "control4", "Update", "SQL%individual%static%-", "User4", "Hostname4", "DF", (Identity + 4).ToString(), "Company_NameUpdated", "").SetName("TestID-208N");
                 yield return new TestCaseData("screen5", "control5", "Update", "SQL%individual%static%-", "User5", "Hostname5", "DF", "1", "Company1_update", null).Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-209A");
                 yield return new TestCaseData("screen6", "control6", "Update", "SQL%individual%static%-", "User6", "Hostname6", "DF", "1", null, "20042360").Throws(typeof(System.Data.SqlClient.SqlException)).SetName("TestID-210A");
                 yield return new TestCaseData("screen7", "control7", "Update", "SQL%individual%static%-", "User7", "Hostname7", "DF", null, "Company1_update", "20042360").Throws(typeof(ArgumentNullException)).SetName("TestID-211A");
                 yield return new TestCaseData("screen8", "control8", "Update", "SQL%individual%static%-", "User8", "Hostname8", "DF", null, null, null).Throws(typeof(ArgumentNullException)).SetName("TestID-212A");
                 yield return new TestCaseData("screen9", "control9", "Update", "SQL%individual%static%-", "User9", "Hostname9", "DF", "", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-213A");
                 yield return new TestCaseData("screen10", "control10", "Update", "SQL%individual%static%-", "User10", "Hostname10", "DF", "12N", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-214A");
                 /* Update*/

                /*Delete*/
                 yield return new TestCaseData("screen1", "control1", "Delete", "SQL%individual%static%-", "User1", "Hostname1", "NT", (Identity + 5).ToString(), "Hitachi", "125987").SetName("TestID-215N");
                 yield return new TestCaseData("screen2", "control2", "Delete", "SQL%individual%static%-", "User2", "Hostname2", "NT", (Identity + 6).ToString(), "Hitachi", "987456").SetName("TestID-216N");
                 yield return new TestCaseData("screen3", "control3", "Delete", "SQL%individual%static%-", "User3", "Hostname3", "NT", (Identity + 7).ToString(), "", "125987").SetName("TestID-217N");
                 yield return new TestCaseData("screen4", "control4", "Delete", "SQL%individual%static%-", "User4", "Hostname4", "NT", (Identity + 8).ToString(), "HiSOL", "").SetName("TestID-218N");
                 yield return new TestCaseData("screen5", "control5", "Delete", "SQL%individual%static%-", "User5", "Hostname5", "NC", (Identity + 4).ToString(), "Company1_update", null).Throws(typeof(NullReferenceException)).SetName("TestID-219A");
                 yield return new TestCaseData("screen6", "control6", "Delete", "SQL%individual%static%-", "User6", "Hostname6", "NC", (Identity + 9).ToString(), null, "20042360").Throws(typeof(NullReferenceException)).SetName("TestID-220A");
                 yield return new TestCaseData("screen7", "control7", "Delete", "SQL%individual%static%-", "User7", "Hostname7", "RC", null, "Company1_update", "20042360").Throws(typeof(ArgumentNullException)).SetName("TestID-221A");
                 yield return new TestCaseData("screen8", "control8", "Delete", "SQL%individual%static%-", "User8", "Hostname8", "RC", null, null, null).Throws(typeof(ArgumentNullException)).SetName("TestID-222A");
                 yield return new TestCaseData("screen9", "control9", "Delete", "SQL%individual%static%-", "User9", "Hostname9", "RC", "", "Company1_update", "20042360").Throws(typeof(System.FormatException)).SetName("TestID-223A");
                 /*Delete*/

                /*SelectJoin1*/
                 yield return new TestCaseData("screen10", "control10", "SelectJoin1", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-224N");
                 yield return new TestCaseData("screen10", "control10", "SelectJoin1", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-225N");
                 yield return new TestCaseData("screen10", "control10", "SelectJoin1", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-226N");
                 /*SelectJoin*/

                /*SelectJoin2*/
                 yield return new TestCaseData("screen10", "control10", "SelectJoin2", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-227N");
                 yield return new TestCaseData("screen10", "control10", "SelectJoin2", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-228N");
                 yield return new TestCaseData("screen10", "control10", "SelectJoin2", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-229N");
                 /*SelectJoin2*/

                /*testSqlsvr4c*/
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr4c", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-230N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr4c", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-231N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr4c", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-232N");
                 /*testSqlsvr4c*/

                /*testSqlsvr4b*/
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr4b", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-233N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr4b", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-234N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr4b", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-235N");
                 /*testSqlsvr4b*/

                /*testSqlsvr4a*/
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr4a", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-236N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr4a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-237N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr4a", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-238N");
                /*testSqlsvr4a*/

                /*List*/
                 yield return new TestCaseData("screen10", "control10", "check_7a", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-239N");
                 yield return new TestCaseData("screen10", "control10", "check_7a", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-240N");
                 yield return new TestCaseData("screen10", "control10", "check_7a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-241N");

                 yield return new TestCaseData("screen10", "control10", "check_11a", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-242N");
                 yield return new TestCaseData("screen10", "control10", "check_11a", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-243N");
                 yield return new TestCaseData("screen10", "control10", "check_11a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-244N");

                 yield return new TestCaseData("screen10", "control10", "check_11c", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-245A");
                 yield return new TestCaseData("screen10", "control10", "check_11c", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-246A");
                 yield return new TestCaseData("screen10", "control10", "check_11c", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-247A");
                 /*List*/

                /*Select Case*/
                 yield return new TestCaseData("screen10", "control10", "SelectCase1a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-248N");
                 yield return new TestCaseData("screen10", "control10", "SelectCase1b", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-249N");
                 yield return new TestCaseData("screen10", "control10", "SelectCase2a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-250N");
                 yield return new TestCaseData("screen10", "control10", "SelectCase2b", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-251N");
                 yield return new TestCaseData("screen10", "control10", "SelectCase3a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-252N");
                 yield return new TestCaseData("screen10", "control10", "SelectCase3b", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-253N");
                 yield return new TestCaseData("screen10", "control10", "SelectCase4a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-254N");
                 yield return new TestCaseData("screen10", "control10", "SelectCase4b", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-255N");
                /*Select Case*/

                /*Select Case Default*/
                 yield return new TestCaseData("screen10", "control10", "SelectCaseDefault1a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-256N");
                 yield return new TestCaseData("screen10", "control10", "SelectCaseDefault1b", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-257N");
                 yield return new TestCaseData("screen10", "control10", "SelectCaseDefault2a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-258N");
                 yield return new TestCaseData("screen10", "control10", "SelectCaseDefault2b", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-259N");
                 yield return new TestCaseData("screen10", "control10", "SelectCaseDefault3a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-260N");
                 yield return new TestCaseData("screen10", "control10", "SelectCaseDefault3b", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-261N");
                 yield return new TestCaseData("screen10", "control10", "SelectCaseDefault4a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-262N");
                 yield return new TestCaseData("screen10", "control10", "SelectCaseDefault4b", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-263N");
                 /*Select Case Default*/

                /*D layer Execution*/
                yield return new TestCaseData("screen10", "control10", "SelectCaseDefault1a", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-264N");
                yield return new TestCaseData("screen10", "control10", "SelectCaseDefault1b", "SQL%individual%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-265N");
                yield return new TestCaseData("screen10", "control10", "SelectCaseDefault2a", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-266N");
                yield return new TestCaseData("screen10", "control10", "SelectCaseDefault2b", "SQL%individual%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-267N");
                yield return new TestCaseData("screen10", "control10", "SelectCaseDefault3a", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-268N");
                yield return new TestCaseData("screen10", "control10", "SelectCaseDefault3b", "SQL%individual%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-269N");
                yield return new TestCaseData("screen10", "control10", "SelectCaseDefault4a", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-270N");
                yield return new TestCaseData("screen10", "control10", "SelectCaseDefault4b", "SQL%individual%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-271N");
                /*D layer Execution*/

                /*edit*/
                 yield return new TestCaseData("screen10", "control10", "edit_8e", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-272N");
                 yield return new TestCaseData("screen10", "control10", "edit_4e", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-273N");
                 yield return new TestCaseData("screen10", "control10", "edit_3e", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-274N");
                 yield return new TestCaseData("screen10", "control10", "edit_9a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-275N");
                 yield return new TestCaseData("screen10", "control10", "edit_9b1", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-276N");
                 yield return new TestCaseData("screen10", "control10", "edit_9c1", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").Throws(typeof(SqlException)).SetName("TestID-277A");
                 yield return new TestCaseData("screen10", "control10", "edit_2a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-278N");
                 yield return new TestCaseData("screen10", "control10", "edit_5e", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-279N");
                 /*edit*/

                /*testSqlsvr2c*/
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr2c", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-280N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr2c", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-281N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr2c", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-282N");
                 /*testSqlsvr2c*/

                /*TestSqlsvr*/
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr2d", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-283N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr2d", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-284N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr2d", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-285N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr_n", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-286N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr_n", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-287N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr_n", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-288N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr_1e", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-289N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr_1e", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-290N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr_1e", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-291N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr1a", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-292N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr1a", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-293N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr1a", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-294N");
                 /*TestSqlsvr*/

                /*List*/
                yield return new TestCaseData("screen10", "control10", "check_3f", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-295A");
                yield return new TestCaseData("screen10", "control10", "check_3f", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-296A");
                yield return new TestCaseData("screen10", "control10", "check_3f", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-297A");
                yield return new TestCaseData("screen10", "control10", "check_9c", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-298A");
                yield return new TestCaseData("screen10", "control10", "check_9c", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-299A");
                yield return new TestCaseData("screen10", "control10", "check_9c", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-300A");
                /*List*/

                /*SelectAll_DSQL1*/
                 yield return new TestCaseData("screen10", "control10", "ArgumentException0", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-301A");
                 yield return new TestCaseData("screen10", "control10", "ArgumentException0", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-302A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException1", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-303A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException2", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-304A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException3", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-305A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException4", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-306A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException5", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-307A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException6", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-308A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException7", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-309A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException8", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-310A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException9", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-311A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException10", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-312A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException11", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-313A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException12", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-314A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException13", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-315A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException14", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-316A");
                 yield return new TestCaseData("screen1", "control1", "ArgumentException15", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null, null).Throws(typeof(ArgumentException)).SetName("TestID-317A");
                 yield return new TestCaseData("screen10", "control10", "ArgumentException16", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").Throws(typeof(ArgumentException)).SetName("TestID-318A");
                 yield return new TestCaseData("screen10", "control10", "ArgumentException17", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").Throws(typeof(ArgumentException)).SetName("TestID-319A");
                 yield return new TestCaseData("screen10", "control10", "ArgumentException18", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").Throws(typeof(ArgumentException)).SetName("TestID-320A");
                 yield return new TestCaseData("screen10", "control10", "ArgumentException19", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").Throws(typeof(ArgumentException)).SetName("TestID-321A");
                 yield return new TestCaseData("screen10", "control10", "ArgumentException20", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").Throws(typeof(ArgumentException)).SetName("TestID-322A");
                 yield return new TestCaseData("screen10", "control10", "ArgumentException21", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").Throws(typeof(ArgumentException)).SetName("TestID-323A");
                 yield return new TestCaseData("screen10", "control10", "ArgumentException22", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").Throws(typeof(ArgumentException)).SetName("TestID-324A");
                 yield return new TestCaseData("screen10", "control10", "ArgumentException23", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").Throws(typeof(ArgumentException)).SetName("TestID-325A");
                 /*SelectAll_DSQL1*/

                /*NEW TEST CASES ADDED*/
                 yield return new TestCaseData("screen10", "control10", "check_1", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").Throws(typeof(ArgumentException)).SetName("TestID-326A");
                 yield return new TestCaseData("screen10", "control10", "check_2", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").Throws(typeof(ArgumentException)).SetName("TestID-327A");
                 yield return new TestCaseData("screen10", "control10", "check_3", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").Throws(typeof(ArgumentException)).SetName("TestID-328A");
                 yield return new TestCaseData("screen10", "control10", "check_4", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-329N");
                 yield return new TestCaseData("screen10", "control10", "check_5", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-330N");

                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr2e", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-331N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr2e", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-332N");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr2e", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-333N");

                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr2f", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").Throws(typeof(ArgumentException)).SetName("TestID-334A");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr2f", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-335A");
                 yield return new TestCaseData("screen10", "control10", "TestSqlsvr2f", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-336A");
               
                 yield return new TestCaseData("screen10", "control10", "edit_9e", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").SetName("TestID-337N");
                 yield return new TestCaseData("screen10", "control10", "edit_10e", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-338A");
                 yield return new TestCaseData("screen10", "control10", "edit_11e", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "20042360").Throws(typeof(ArgumentException)).SetName("TestID-339A");
                 /*SelectJoin0*/
                yield return new TestCaseData("screen10", "control10", "SelectJoin0", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-340N");
                yield return new TestCaseData("screen10", "control10", "SelectJoin0", "SQL%common%static%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-341N");
                yield return new TestCaseData("screen10", "control10", "SelectJoin0", "SQL%common%dynamic%-", "User10", "Hostname10", "RC", "12N", "Company1_update", "20042360").SetName("TestID-342N");
                /*SelectJoin0*/
                yield return new TestCaseData("screen10", "control10", "SelectCase5a", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-343N");
                yield return new TestCaseData("screen10", "control10", "SelectCase5b", "SQL%individual%static%-", "User10", "Hostname10", "RC", "12N", "symphony", "").SetName("TestID-344N");
                /*NEW TEST CASES ADDED*/
            }
        }

        #region TestCode
        /// <summary>CallBusinessLogic Method</summary>       
        /// <param name="screen">screen ID</param>
        /// <param name="buttonID">Button ID</param>
        /// <param name="action">Control action name</param>
        /// <param name="dbGeneration">Db Generation</param>
        /// <param name="user">User Info</param>
        /// <param name="ipAddress">Ip address</param>
        /// <param name="isolationLevel">Isolation level</param>
        /// <param name="testParameterValue">Test Parameter values</param>
        /// <param name="shipperID">Shipper Id</param>
        /// <param name="companyName">Company name</param>
        /// <param name="phone">Phone Number</param>
        [TestCaseSource("TestSampleScreen_DaoAndDam_Test")]
        public void SampleScreen_DaoAndDam_Test(string screen, string buttonID, string action, string dbGeneration, string user, string ipAddress,
                                          string isolationLevel, string shipperID, string companyName, string phone)
        {
            //using (TransactionScope scope = new TransactionScope())
            //{
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            MyUserInfo userInfo = new MyUserInfo(user, ipAddress);
            TestParameterValue testParameterValue
                 = new TestParameterValue(
                     screen, buttonID, action,
                     dbGeneration,
                    userInfo);
            TestReturnValue resultTestReturnValue;
            TestReturnValue expectedTestReturnValue;
            DataSet expectedDataSet = new DataSet();
            DataSet resultDataSet = new DataSet();
            //Assert conditions
            switch (action)
            {
                case "SelectCount":
                    CallBusinessLogic(screen, buttonID, action, dbGeneration, user, ipAddress, isolationLevel, testParameterValue, out resultTestReturnValue,
                                      out expectedTestReturnValue);
                    Assert.AreEqual(resultTestReturnValue.Obj.ToString(), expectedTestReturnValue.Obj.ToString());
                    break;
                case "SelectAll_DT":
                case "SelectAll_DR":
                case "SelectAll_DSQL":
                    testParameterValue.OrderColumn = "c1";
                    testParameterValue.OrderSequence = "A";
                    CallBusinessLogic(screen, buttonID, action, dbGeneration, user, ipAddress, isolationLevel, testParameterValue, out resultTestReturnValue,
                                     out expectedTestReturnValue);
                    DataTable expectedDataTable = (DataTable)expectedTestReturnValue.Obj;
                    DataTable resultDataTable = (DataTable)resultTestReturnValue.Obj;
                    if (!resultTestReturnValue.ErrorFlag)
                        Assert.AreEqual(expectedDataTable.Rows.Count, resultDataTable.Rows.Count);
                    else
                        Assert.AreEqual(resultTestReturnValue.ErrorFlag, true);
                    break;
                case "SelectAll_DS":
                    CallBusinessLogic(screen, buttonID, action, dbGeneration, user, ipAddress, isolationLevel, testParameterValue, out resultTestReturnValue,
                                     out expectedTestReturnValue);
                    expectedDataSet = (DataSet)expectedTestReturnValue.Obj;
                    resultDataSet = (DataSet)resultTestReturnValue.Obj;
                    if (!resultTestReturnValue.ErrorFlag)
                        Assert.AreEqual(resultDataSet.Tables.Count, expectedDataSet.Tables.Count);
                    else
                        Assert.AreEqual(resultTestReturnValue.ErrorFlag, true);
                    break;
                case "Select":
                    testParameterValue.OrderColumn = "c1";
                    testParameterValue.OrderSequence = "A";
                    testParameterValue.ShipperID = 1;
                    CallBusinessLogic(screen, buttonID, action, dbGeneration, user, ipAddress, isolationLevel, testParameterValue, out resultTestReturnValue,
                                     out expectedTestReturnValue);
                    if (!resultTestReturnValue.ErrorFlag)
                    {
                        Assert.AreEqual(resultTestReturnValue.ShipperID, expectedTestReturnValue.ShipperID);
                        Assert.AreEqual(resultTestReturnValue.Phone, expectedTestReturnValue.Phone);
                        Assert.AreEqual(resultTestReturnValue.CompanyName, expectedTestReturnValue.CompanyName);
                    }
                    else
                        Assert.AreEqual(resultTestReturnValue.ErrorFlag, true);
                    break;
                case "Insert":
                    testParameterValue.CompanyName = companyName;
                    testParameterValue.Phone = phone;
                    CallBusinessLogic(screen, buttonID, action, dbGeneration, user, ipAddress, isolationLevel, testParameterValue, out resultTestReturnValue,
                                     out expectedTestReturnValue);

                    if (!resultTestReturnValue.ErrorFlag)
                    {
                        Assert.AreEqual(resultTestReturnValue.Obj.ToString(), expectedTestReturnValue.Obj.ToString());
                    }
                    else
                        Assert.AreEqual(resultTestReturnValue.ErrorFlag, true);
                    break;

                case "Update":
                    testParameterValue.ShipperID = int.Parse(shipperID);
                    testParameterValue.CompanyName = companyName;
                    testParameterValue.Phone = phone;
                    CallBusinessLogic(screen, buttonID, action, dbGeneration, user, ipAddress, isolationLevel, testParameterValue, out resultTestReturnValue,
                                     out expectedTestReturnValue);
                    if (!resultTestReturnValue.ErrorFlag)
                    {
                        Assert.AreEqual(resultTestReturnValue.Obj.ToString(), expectedTestReturnValue.Obj.ToString());
                    }
                    else
                        Assert.AreEqual(resultTestReturnValue.ErrorFlag, true);
                    break;
                case "Delete":
                    // 情報の設定
                    testParameterValue.ShipperID = int.Parse(shipperID);
                    CallBusinessLogic(screen, buttonID, action, dbGeneration, user, ipAddress, isolationLevel, testParameterValue, out resultTestReturnValue,
                                     out expectedTestReturnValue);
                    if (!resultTestReturnValue.ErrorFlag)
                    {
                        if (resultTestReturnValue.Obj.ToString() == "1")
                        {
                            Assert.AreEqual(resultTestReturnValue.Obj.ToString(), "1");
                        }
                        else
                        {
                            Assert.AreEqual(resultTestReturnValue.Obj.ToString(), "0");
                        }
                    }
                    else
                        Assert.AreEqual(resultTestReturnValue.ErrorFlag, true);
                    break;
                case "SelectJoin0":
                case "SelectJoin1":
                case "SelectJoin2":
                case "TestSqlsvr4c":
                case "TestSqlsvr4b":
                case "TestSqlsvr4a":
                case "TestSqlsvr2c":
                case "TestSqlsvr2d":
                case "TestSqlsvr_n":
                case "TestSqlsvr_1e":
                case "TestSqlsvr1a":
                case "TestSqlsvr1b":
                case "TestSqlsvr2e":
                case "TestSqlsvr2f":
                    testParameterValue.CompanyName = companyName;
                    testParameterValue.OrderColumn = "c1";
                    testParameterValue.OrderSequence = "A";
                    CallBusinessLogic(screen, buttonID, action, dbGeneration, user, ipAddress, isolationLevel, testParameterValue, out resultTestReturnValue,
                                     out expectedTestReturnValue);
                    expectedDataSet = (DataSet)expectedTestReturnValue.Obj;
                    resultDataSet = (DataSet)resultTestReturnValue.Obj;
                    if (!resultTestReturnValue.ErrorFlag)
                        Assert.AreEqual(resultDataSet.Tables.Count, expectedDataSet.Tables.Count);
                    else
                        Assert.AreEqual(resultTestReturnValue.ErrorFlag, true);
                    break;
                case "SelectCase1a":
                case "SelectCase1b":
                case "SelectCase2a":
                case "SelectCase2b":
                case "SelectCase3a":
                case "SelectCase3b":
                case "SelectCase4a":
                case "SelectCase4b":
                case "SelectCase5a":
                case "SelectCase5b":
                case "SelectCaseDefault1a":
                case "SelectCaseDefault1b":
                case "SelectCaseDefault2a":
                case "SelectCaseDefault2b":
                case "SelectCaseDefault3a":
                case "SelectCaseDefault3b":
                case "SelectCaseDefault4a":
                case "SelectCaseDefault4b":
                    TestParameterValue testParameterValue1
                 = new TestParameterValue(
                     screen, buttonID, "SelectCase",
                     dbGeneration,
                    userInfo);
                    testParameterValue1.SelectCase = action;
                    CallBusinessLogic(screen, buttonID, action, dbGeneration, user, ipAddress, isolationLevel, testParameterValue1, out resultTestReturnValue,
                                   out expectedTestReturnValue);
                    DataTable expectedDatatable = (DataTable)expectedTestReturnValue.Obj;
                    DataTable resultDatatable = (DataTable)resultTestReturnValue.Obj;
                    if (!resultTestReturnValue.ErrorFlag)
                        Assert.AreEqual(expectedDatatable.Rows.Count, resultDatatable.Rows.Count);
                    else
                        Assert.AreEqual(resultTestReturnValue.ErrorFlag, true);
                    break;
                case "check_1":
                case "check_2":
                case "check_3":
                case "check_4":
                case "check_7a":
                case "check_11a":
                case "check_11c":
                case "check_6b":
                case "check_3f":
                case "check_9c":
                case "check_5":
                    TestParameterValue testParameterValue2
                = new TestParameterValue(
                    screen, buttonID, "check",
                    dbGeneration,
                   userInfo);
                    testParameterValue2.check = action;
                    testParameterValue.OrderColumn = "c1";
                    testParameterValue.OrderSequence = "A";
                    CallBusinessLogic(screen, buttonID, action, dbGeneration, user, ipAddress, isolationLevel, testParameterValue2, out resultTestReturnValue,
                                  out expectedTestReturnValue);
                    expectedDataSet = (DataSet)expectedTestReturnValue.Obj;
                    resultDataSet = (DataSet)resultTestReturnValue.Obj;
                    if (!resultTestReturnValue.ErrorFlag)
                        Assert.AreEqual(expectedDataSet.Tables.Count, resultDataSet.Tables.Count);
                    else
                        Assert.AreEqual(resultTestReturnValue.ErrorFlag, true);
                    break;

                case "edit_8e":
                case "edit_4e":
                case "edit_3e":
                case "edit_9a":
                case "edit_9b1":
                case "edit_9c1":
                case "edit_2a":
                case "edit_5e":
                case "edit_9e":
                case "edit_10e":
                case "edit_11e":
                    TestParameterValue testParameterValue3
                 = new TestParameterValue(
                     screen, buttonID, "edit",
                     dbGeneration,
                    userInfo);
                    testParameterValue3.SelectCase = action;

                    CallBusinessLogic(screen, buttonID, action, dbGeneration, user, ipAddress, isolationLevel, testParameterValue3, out resultTestReturnValue,
                                   out expectedTestReturnValue);
                    expectedDataSet = (DataSet)expectedTestReturnValue.Obj;
                    resultDataSet = (DataSet)resultTestReturnValue.Obj;
                    if (!resultTestReturnValue.ErrorFlag)
                        Assert.AreEqual(expectedDataSet.Tables.Count, resultDataSet.Tables.Count);
                    else
                        Assert.AreEqual(resultTestReturnValue.ErrorFlag, true);
                    break;
                case "ArgumentException0":
                case "ArgumentException1":
                case "ArgumentException2":
                case "ArgumentException3":
                case "ArgumentException4":
                case "ArgumentException5":
                case "ArgumentException6":
                case "ArgumentException7":
                case "ArgumentException8":
                case "ArgumentException9":
                case "ArgumentException10":
                case "ArgumentException11":
                case "ArgumentException12":
                case "ArgumentException13":
                case "ArgumentException14":
                case "ArgumentException15":
                case "ArgumentException16":
                case "ArgumentException17":
                case "ArgumentException18":
                case "ArgumentException19":
                case "ArgumentException20":
                case "ArgumentException21":
                case "ArgumentException22":
                case "ArgumentException23":
                    TestParameterValue testParameterValue4
                = new TestParameterValue(
                    screen, buttonID, "ArgumentException",
                    dbGeneration,
                   userInfo);
                    testParameterValue4.SelectCase = action;
                    CallBusinessLogic(screen, buttonID, action, dbGeneration, user, ipAddress, isolationLevel, testParameterValue4, out resultTestReturnValue,
                                  out expectedTestReturnValue);
                    expectedDataSet = (DataSet)expectedTestReturnValue.Obj;
                    resultDataSet = (DataSet)resultTestReturnValue.Obj;
                    if (!resultTestReturnValue.ErrorFlag)
                        Assert.AreEqual(expectedDataSet.Tables.Count, resultDataSet.Tables.Count);
                    else
                        Assert.AreEqual(resultTestReturnValue.ErrorFlag, true);
                    break;
            }
            //}
        }


        #region CallBusinessLogic
        /// <summary>CallBusinessLogic Method</summary>
        /// <param name="screen">screen ID</param>
        /// <param name="buttonID">Button ID</param>
        /// <param name="action">Control action name</param>
        /// <param name="dbGeneration">Db Generation</param>
        /// <param name="user">User Info</param>
        /// <param name="ipAddress">Ip address</param>
        /// <param name="isolationLevel">Isolation level</param>
        /// <param name="testParameterValue">Test Parameter values</param>
        /// <returns>resultTestReturnValue</returns>
        /// <returns>expectedTestReturnValue</returns>
        private void CallBusinessLogic(string screen, string buttonID, string action, string dbGeneration, string user, string ipAddress, string isolationLevel, TestParameterValue testParameterValue, out TestReturnValue resultTestReturnValue, out TestReturnValue expectedTestReturnValue)
        {
            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = SelectIsolationLevel(isolationLevel);

            // B層を生成
            LayerB myBusiness = new LayerB();

            // 業務処理を実行
            resultTestReturnValue = (TestReturnValue)myBusiness.DoBusinessLogic((BaseParameterValue)testParameterValue, iso);
            expectedTestReturnValue = (TestReturnValue)myBusiness.DoBusinessLogic((BaseParameterValue)testParameterValue, iso);
        }
        #endregion

        #region SelectIsolationLevel

        /// <summary>分離レベルの設定</summary>
        /// <param name="isolationLevel">分離レベルの指定</param>
        /// <returns>分離レベル列挙型</returns>
        private DbEnum.IsolationLevelEnum SelectIsolationLevel(string isolationLevel)
        {
            if (isolationLevel == "NC")
            {
                return DbEnum.IsolationLevelEnum.NotConnect;
            }
            else if (isolationLevel == "NT")
            {
                return DbEnum.IsolationLevelEnum.NoTransaction;
            }
            else if (isolationLevel == "RU")
            {
                return DbEnum.IsolationLevelEnum.ReadUncommitted;
            }
            else if (isolationLevel == "RC")
            {
                return DbEnum.IsolationLevelEnum.ReadCommitted;
            }
            else if (isolationLevel == "RR")
            {
                return DbEnum.IsolationLevelEnum.RepeatableRead;
            }
            else if (isolationLevel == "SZ")
            {
                return DbEnum.IsolationLevelEnum.Serializable;
            }
            else if (isolationLevel == "SS")
            {
                return DbEnum.IsolationLevelEnum.Snapshot;
            }
            else if (isolationLevel == "DF")
            {
                return DbEnum.IsolationLevelEnum.DefaultTransaction;
            }
            else
            {
                throw new Exception("分離レベルの設定がおかしい");
            }
        }
        #endregion

        /// <summary>
        /// This method to generate test cases. 
        /// This method to generate test data to be passed to the method GetParametersFromPARAMTagTest.
        /// </summary>
        public IEnumerable<TestCaseData> TestParamtag
        {
            get
            {
                yield return new TestCaseData(MakeRelativePathFile() + "select-case1a.dpq.xml", "SQL%individual%static%-").SetName("Testcase01");
                yield return new TestCaseData(MakeRelativePathFile() + "select-case1b.dpq.xml", "SQL%individual%static%-").SetName("Testcase02");
                yield return new TestCaseData(MakeRelativePathFile() + "select-case2a.dpq.xml", "SQL%individual%static%-").SetName("Testcase03");
                yield return new TestCaseData(MakeRelativePathFile() + "select-case2b.dpq.xml", "SQL%individual%static%-").SetName("Testcase04");
                yield return new TestCaseData(MakeRelativePathFile() + "select-case3a.dpq.xml", "SQL%individual%static%-").SetName("Testcase05");
                yield return new TestCaseData(MakeRelativePathFile() + "select-case4b.dpq.xml", "SQL%individual%static%-").SetName("Testcase06");
                yield return new TestCaseData(MakeRelativePathFile() + "ShipperSelect.sql", "SQL%individual%static%-").SetName("Testcase07");
                yield return new TestCaseData(MakeRelativePathFile() + "testSqlsvr4.sql", "SQL%individual%static%-").SetName("Testcase08");
                yield return new TestCaseData(MakeRelativePathFile() + "SelectCaseParam.xml", "SQL%individual%static%-").SetName("Testcase09");
                yield return new TestCaseData(MakeRelativePathFile() + "SelectCaseParamLength.dpq.xml", "SQL%individual%static%-").SetName("Testcase10");
                yield return new TestCaseData(MakeRelativePathFile() + "SelectCaseParamArray.dpq.xml", "SQL%individual%static%-").SetName("Testcase11");
                yield return new TestCaseData(MakeRelativePathFile() + "SelectCaseParamAllTypes.dpq.xml", "SQL%individual%static%-").SetName("Testcase12");
            }
        }

        /// <summary>
        /// GetParametersFromPARAMTagTest Method
        /// </summary>
        /// <param name="TestCaseID">TestCaseID</param>
        /// <param name="filePath">filePath</param>
        /// <param name="dbGeneration">dbGeneration</param>
        [TestCaseSource("TestParamtag")]
        public void GetParametersFromPARAMTagTest(string filePath, string dbGeneration)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            GetParam(filePath, out dt, out dt1, dbGeneration);
            Assert.AreEqual(dt.Rows.Count, dt1.Rows.Count);
        }

        #region GetParam
        /// <summary>
        /// GetParam Method
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="dt">dt</param>
        /// <param name="dt1">dt1</param>
        /// <param name="dbGeneration">dbGeneration</param>
        private void GetParam(string path, out  DataTable dt, out DataTable dt1, string dbGeneration)
        {
            MyUserInfo userInfo = new MyUserInfo("user1", "Hostname");
            LayerB lb = new LayerB();
            TestParameterValue test = new TestParameterValue("screen1", "button1", "GetParametersFromPARAMTag", dbGeneration, userInfo);
            test.Filepath = path;
            TestReturnValue testreturn = (TestReturnValue)lb.DoBusinessLogic((BaseParameterValue)test);
            dt = (DataTable)testreturn.Obj;
            dt1 = (DataTable)testreturn.Obj;

        }
        #endregion

        #region MakeRelativePathFile
        /// <summary>
        /// MakeRelativePathFile Method
        /// </summary>
        /// <returns></returns>
        private static string MakeRelativePathFile()
        {
            try
            {
                return Path.GetFullPath("sql") + "\\";
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #endregion

        #region Methods to revert datbase Changes
        /// <summary>
        /// ToCommaString Method to get the comma(,) Separted string of list of integer 
        /// </summary>
        /// <param name="list">list</param>
        private string ToCommaString(List<int> list)
        {
            if (list.Count <= 0)
                return ("0");
            if (list.Count == 1)
                return (list[0].ToString());
            System.Text.StringBuilder sb = new System.Text.StringBuilder(list[0].ToString());
            for (int x = 1; x < list.Count; x++)
                sb.Append("," + list[x].ToString());
            return (sb.ToString());
        }
        /// <summary>
        /// GetListData() Method to get list of integer of ShipperID in shippers table before running the test cases 
        /// </summary>
        /// <param name="list">list</param>
        /// <returns value="getList">List of Integer data type<int></returns>
        private List<int> GetListData()
        {
            List<int> getList = new List<int>();
            MyUserInfo userInfo = new MyUserInfo("user1", "Hostname");
            LayerB lb = new LayerB();
            TestParameterValue test = new TestParameterValue("Select ShipperID from Shippers", "button1", "GetList", "SQL%individual%static%-", userInfo);
            TestReturnValue testreturn = (TestReturnValue)lb.DoBusinessLogic((BaseParameterValue)test);
            DataTable dt = (DataTable)testreturn.Obj;
            foreach (DataRow dr in dt.Rows)
            {
                getList.Add((int)dr[0]);
            }
            return getList;
        }

        /// <summary>
        /// DeleteData() Method to delete the ShipperID's from shippers table which are inserted while running test cases. 
        /// </summary>
        private void DeleteData()
        {
            MyUserInfo userInfo = new MyUserInfo("user1", "Hostname");
            LayerB lb = new LayerB();
            string strIDdelete = ToCommaString(testList);
            TestParameterValue test = new TestParameterValue("Delete from Shippers where ShipperID not in(" + strIDdelete + ")", "button1", "GetDelete", "SQL%individual%static%-", userInfo);
            TestReturnValue testreturn = (TestReturnValue)lb.DoBusinessLogic((BaseParameterValue)test);
            testList.Clear();
        }
        /// <summary>
        /// GetIdentityValue() Method to get the Current Identity value of Shipper ID column in Shippers table 
        /// </summary>
        /// <returns value="intIdentity">intIdentity as integer data type</returns>
        private int GetIdentityValue()
        {
            int intIdentity = 0;
            MyUserInfo userInfo = new MyUserInfo("user1", "Hostname");
            LayerB lb = new LayerB();
            TestParameterValue test = new TestParameterValue("SELECT IDENT_CURRENT('shippers')", "button1", "GetID", "SQL%individual%static%-", userInfo);
            TestReturnValue testreturn = (TestReturnValue)lb.DoBusinessLogic((BaseParameterValue)test);
            intIdentity = Convert.ToInt16(testreturn.Obj);
            return intIdentity;
        }
        #endregion
    }
}

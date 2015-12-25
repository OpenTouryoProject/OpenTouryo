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
//*  08/11/2014   Sai                   Added Test case ID using TestName property as per Nishino-San comments
//*  10/20/2015   Sai                   Did test code developemnt for the methods SelectAll_DSQL, SelectAll_DR,
//*                                     Select, Insert, Update and Delete in BDLayer      
//**********************************************************************************

// 型情報
using BDLayer.Test.Common;
using BDLayer.Test.Business;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// 業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Common;
using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Business.Exceptions;
using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

// testing framework
using NUnit.Framework;

namespace BDLayer.Test
{
    [TestFixture]
    public class CRUDTest
    {
        /// <summary>
        /// テスト前処理
        /// </summary>
        [TestFixtureSetUp]
        public void Init()
        {
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

        #region SelectCount

        /// <summary>
        /// SelectCount
        /// </summary>
        /// <param name="screenId">
        /// 画面ID
        /// </param>
        /// <param name="controlId">
        /// コントロールID
        /// </param>
        /// <param name="actionType">
        /// アクションタイプ
        /// ・Dap  ：データプロバイダ
        /// ・Mode1：個別、共通、自動生成
        /// ・Mode2：静的、動的
        /// </param>
        /// <param name="userName">
        /// ユーザ名
        /// </param>
        /// <param name="ipAddress">
        /// IPアドレス（ホスト名）
        /// </param>
        /// <param name="isolationLevel">
        /// 分離レベルの指定
        /// ・NC：NotConnect
        /// ・NT：NoTransaction
        /// ・RU：ReadUncommitted
        /// ・RC：ReadCommitted
        /// ・RR：RepeatableRead
        /// ・SZ：Serializable
        /// ・SS：Snapshot
        /// ・DF：DefaultTransaction;
        /// </param>
        [TestCase("screen1", "control1", "SQL%individual%static%-", "User1", "Hostname1", "RC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-001N")]
        [TestCase("screen2", "control2", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-002N")]
        [TestCase("screen3", "control3", "SQL%common%static%-", "User3", "Hostname3", "RC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-003N")]
        [TestCase("screen4", "control4", "SQL%common%dynamic%-", "User4", "Hostname4", "RC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-004N")]
        [TestCase("screen5", "control5", "SQL%generate%static%-", "User5", "Hostname5", "RC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-005N")]
        [TestCase("screen6", "control6", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-006N")]
        public void _1_SelectCount(
            string screenId, string controlId, string actionType,
            string userName, string ipAddress, string isolationLevel)
        {
            string methodName = "SelectCount";

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    screenId, controlId, methodName, actionType, new MyUserInfo(userName, ipAddress));

            // 戻り値
            TestReturnValue testReturnValue = null;

            // Ｂ層呼出し
            LayerB layerB = new LayerB();

            try
            {
                testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(
                    testParameterValue, this.SelectIsolationLevel(isolationLevel));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            // 戻り値検証。
            Assert.AreEqual(testReturnValue.ErrorFlag, false);
            Assert.AreEqual(StringChecker.IsNumbers(testReturnValue.Obj.ToString()), true);
        }

        #endregion

        #region SelectAll_XX

        /// <summary>
        /// SelectAll_XX
        /// </summary>
        /// <param name="screenId">
        /// 画面ID
        /// </param>
        /// <param name="controlId">
        /// コントロールID
        /// </param>
        /// <param name="methodName">
        /// メソッド名
        /// </param>
        /// <param name="actionType">
        /// アクションタイプ
        /// ・Dap  ：データプロバイダ
        /// ・Mode1：個別、共通、自動生成
        /// ・Mode2：静的、動的
        /// </param>
        /// <param name="userName">
        /// ユーザ名
        /// </param>
        /// <param name="ipAddress">
        /// IPアドレス（ホスト名）
        /// </param>
        /// <param name="isolationLevel">
        /// 分離レベルの指定
        /// ・NC：NotConnect
        /// ・NT：NoTransaction
        /// ・RU：ReadUncommitted
        /// ・RC：ReadCommitted
        /// ・RR：RepeatableRead
        /// ・SZ：Serializable
        /// ・SS：Snapshot
        /// ・DF：DefaultTransaction;
        /// </param>
        [TestCase("screen1", "control1", "SelectAll_DT", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null,
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-001N")]
        [TestCase("screen2", "control2", "SelectAll_DT", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", null, null,
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-002N")]
        [TestCase("screen3", "control3", "SelectAll_DT", "SQL%common%static%-", "User3", "Hostname3", "RC", null, null,
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-003N")]
        [TestCase("screen4", "control4", "SelectAll_DT", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", null, null,
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-004N")]
        [TestCase("screen5", "control5", "SelectAll_DT", "SQL%generate%static%-", "User5", "Hostname5", "RC", null, null,
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-005N")]
        [TestCase("screen6", "control6", "SelectAll_DT", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC", null, null,
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-006N")]
        [TestCase("screen1", "control1", "SelectAll_DS", "SQL%individual%static%-", "User1", "Hostname1", "RC", null, null,
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-007N")]
        [TestCase("screen2", "control2", "SelectAll_DS", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", null, null,
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-008N")]
        [TestCase("screen3", "control3", "SelectAll_DS", "SQL%common%static%-", "User3", "Hostname3", "RC", null, null,
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-009N")]
        [TestCase("screen4", "control4", "SelectAll_DS", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", null, null,
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-010N")]
        [TestCase("screen5", "control5", "SelectAll_DS", "SQL%generate%static%-", "User5", "Hostname5", "RC", null, null,
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-011N")]
        [TestCase("screen6", "control6", "SelectAll_DS", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC", null, null,
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-012N")]
        [TestCase("screen1", "control1", "SelectAll_DSQL", "SQL%individual%static%-", "User1", "Hostname1", "RC", "c1", "ASC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-013N")]
        [TestCase("screen2", "control2", "SelectAll_DSQL", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", "c2", "ASC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-014N")]
        [TestCase("screen3", "control3", "SelectAll_DSQL", "SQL%common%static%-", "User3", "Hostname3", "RC", "c3", "ASC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-015N")]
        [TestCase("screen4", "control4", "SelectAll_DSQL", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", "c1", "DSC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-016N")]
        [TestCase("screen5", "control5", "SelectAll_DSQL", "SQL%generate%static%-", "User5", "Hostname5", "RC", "c2", "DSC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-017N")]
        [TestCase("screen6", "control6", "SelectAll_DSQL", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC", "c3", "DSC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-018N")]
        [TestCase("screen1", "control1", "SelectAll_DR", "SQL%individual%static%-", "User1", "Hostname1", "RC", "c1", "ASC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-019N")]
        [TestCase("screen2", "control2", "SelectAll_DR", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", "c2", "ASC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-020N")]
        [TestCase("screen3", "control3", "SelectAll_DR", "SQL%common%static%-", "User3", "Hostname3", "RC", "c3", "ASC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-021N")]
        [TestCase("screen4", "control4", "SelectAll_DR", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", "c1", "DSC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-022N")]
        [TestCase("screen5", "control5", "SelectAll_DR", "SQL%generate%static%-", "User5", "Hostname5", "RC", "c2", "DSC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-023N")]
        [TestCase("screen6", "control6", "SelectAll_DR", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC", "c3", "DSC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-024N")]
        [TestCase("screen6", "control6", "SelectAll_DR", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC", null, "DSC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-025A")]
        [TestCase("screen6", "control6", "SelectAll_DR", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC", "c3", null,
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-026A")]
        [TestCase("screen6", "control6", "SelectAll_DR", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC", "", "DSC",
                 ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-027A")]
        [TestCase("screen6", "control6", "SelectAll_DR", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC", "c3", "",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-028A")]
        public void _2_SelectAll_XX(
            string screenId, string controlId,
            string methodName, string actionType,
            string userName, string ipAddress, string isolationLevel, string orderCol, string orderSeq)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    screenId, controlId, methodName, actionType, new MyUserInfo(userName, ipAddress));

            testParameterValue.OrderColumn = orderCol;
            testParameterValue.OrderSequence = orderSeq;

            // 戻り値
            TestReturnValue testReturnValue = null; ;

            // Ｂ層呼出し
            LayerB layerB = new LayerB();

            try
            {
                testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(
                    testParameterValue, this.SelectIsolationLevel(isolationLevel));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            // 戻り値検証。
            Assert.AreEqual(testReturnValue.ErrorFlag, false);

            switch (methodName)
            {
                case "SelectAll_DT":
                    DataTable dt = (DataTable)testReturnValue.Obj;
                    Assert.GreaterOrEqual(dt.Rows.Count, 1);
                    break;
                case "SelectAll_DS":
                    DataSet ds = (DataSet)testReturnValue.Obj;
                    Assert.AreEqual(ds.Tables.Count, 1);
                    Assert.GreaterOrEqual(ds.Tables[0].Rows.Count, 1);
                    break;
                case "SelectAll_DR":
                    DataTable dataTable = (DataTable)testReturnValue.Obj;
                    Assert.GreaterOrEqual(dataTable.Rows.Count, 1);
                    break;
                case "SelectAll_DSQL":
                    DataTable table = (DataTable)testReturnValue.Obj;
                    Assert.GreaterOrEqual(table.Rows.Count, 1);
                    break;
                default:
                    break;

            }
        }

        #endregion

        #region Select

        /// <summary>
        /// Select
        /// </summary>
        /// <param name="screenId">
        /// 画面ID
        /// </param>
        /// <param name="controlId">
        /// コントロールID
        /// </param>
        /// <param name="actionType">
        /// アクションタイプ
        /// ・Dap  ：データプロバイダ
        /// ・Mode1：個別、共通、自動生成
        /// ・Mode2：静的、動的
        /// </param>
        /// <param name="userName">
        /// ユーザ名
        /// </param>
        /// <param name="ipAddress">
        /// IPアドレス（ホスト名）
        /// </param>
        /// <param name="isolationLevel">
        /// 分離レベルの指定
        /// ・NC：NotConnect
        /// ・NT：NoTransaction
        /// ・RU：ReadUncommitted
        /// ・RC：ReadCommitted
        /// ・RR：RepeatableRead
        /// ・SZ：Serializable
        /// ・SS：Snapshot
        /// ・DF：DefaultTransaction;
        /// </param>
        [TestCase("screen1", "control1", "SQL%individual%static%-", "User1", "Hostname1", "RC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-001N")]
        [TestCase("screen2", "control2", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-002N")]
        [TestCase("screen3", "control3", "SQL%common%static%-", "User3", "Hostname3", "RC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-003N")]
        [TestCase("screen4", "control4", "SQL%common%dynamic%-", "User4", "Hostname4", "RC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-004N")]
        [TestCase("screen5", "control5", "SQL%generate%static%-", "User5", "Hostname5", "RC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-005N")]
        [TestCase("screen6", "control6", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-006N")]
        public void _3_Select(
            string screenId, string controlId, string actionType,
            string userName, string ipAddress, string isolationLevel)
        {
            string methodName = "SelectAll_DT";

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    screenId, controlId, methodName, actionType, new MyUserInfo(userName, ipAddress));

            // 戻り値
            TestReturnValue testReturnValue = null;

            // Ｂ層呼出し
            LayerB layerB = new LayerB();

            try
            {
                testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(
                    testParameterValue, this.SelectIsolationLevel(isolationLevel));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            DataTable dt = (DataTable)testReturnValue.Obj;

            methodName = "Select";

            testParameterValue
               = new TestParameterValue(
                   screenId, controlId, methodName, actionType, new MyUserInfo(userName, ipAddress));

            foreach (DataRow row in dt.Rows)
            {
                testParameterValue.ShipperID = Convert.ToInt32(row["ShipperID"]);
                testParameterValue.CompanyName = row["CompanyName"].ToString();
                testParameterValue.Phone = row["Phone"].ToString();

                // 戻り値
                testReturnValue = null;

                // Ｂ層呼出し
                layerB = new LayerB();

                try
                {
                    testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(
                        testParameterValue, this.SelectIsolationLevel(isolationLevel));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                // 戻り値検証。
                Assert.AreEqual(testReturnValue.ErrorFlag, false);
                Assert.AreEqual(testReturnValue.ShipperID, testParameterValue.ShipperID);
                Assert.AreEqual(testReturnValue.CompanyName, testParameterValue.CompanyName);
            }
        }

        #endregion

        #region Insert

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="screenId">
        /// 画面ID
        /// </param>
        /// <param name="controlId">
        /// コントロールID
        /// </param>
        /// <param name="actionType">
        /// アクションタイプ
        /// ・Dap  ：データプロバイダ
        /// ・Mode1：個別、共通、自動生成
        /// ・Mode2：静的、動的
        /// </param>
        /// <param name="userName">
        /// ユーザ名
        /// </param>
        /// <param name="ipAddress">
        /// IPアドレス（ホスト名）
        /// </param>
        /// <param name="isolationLevel">
        /// 分離レベルの指定
        /// ・NC：NotConnect
        /// ・NT：NoTransaction
        /// ・RU：ReadUncommitted
        /// ・RC：ReadCommitted
        /// ・RR：RepeatableRead
        /// ・SZ：Serializable
        /// ・SS：Snapshot
        /// ・DF：DefaultTransaction;
        /// </param>
        [TestCase("screen1", "control1", "SQL%individual%static%-", "User1", "Hostname1", "RC", "Symphony", "9008031613",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-001N")]
        [TestCase("screen2", "control2", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", "Harman", "9008031614",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-002N")]
        [TestCase("screen3", "control3", "SQL%common%static%-", "User3", "Hostname3", "RC", "Symphony Harman", "9008031615",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-003N")]
        [TestCase("screen4", "control4", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", "Harman Symphony", "9008031616",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-004N")]
        [TestCase("screen5", "control5", "SQL%generate%static%-", "User5", "Hostname5", "RC", "Harman Connected", "9008031617",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-005N")]
        [TestCase("screen6", "control6", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC", "Symphony Connected", "9008031618",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-006N")]
        public void _4_Insert(
            string screenId, string controlId, string actionType,
            string userName, string ipAddress, string isolationLevel, string companyname, string phone)
        {
            string methodName = "Insert";

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    screenId, controlId, methodName, actionType, new MyUserInfo(userName, ipAddress));

            // 情報の設定
            testParameterValue.CompanyName = companyname;
            testParameterValue.Phone = phone;

            // 戻り値
            TestReturnValue testReturnValue = null;

            // Ｂ層呼出し
            LayerB layerB = new LayerB();

            try
            {
                testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(
                    testParameterValue, this.SelectIsolationLevel(isolationLevel));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            // 戻り値検証。
            Assert.AreEqual(testReturnValue.ErrorFlag, false);
            Assert.AreEqual(Convert.ToInt32(testReturnValue.Obj), 1);

        }

        #endregion

        #region Update

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="screenId">
        /// 画面ID
        /// </param>
        /// <param name="controlId">
        /// コントロールID
        /// </param>
        /// <param name="actionType">
        /// アクションタイプ
        /// ・Dap  ：データプロバイダ
        /// ・Mode1：個別、共通、自動生成
        /// ・Mode2：静的、動的
        /// </param>
        /// <param name="userName">
        /// ユーザ名
        /// </param>
        /// <param name="ipAddress">
        /// IPアドレス（ホスト名）
        /// </param>
        /// <param name="isolationLevel">
        /// 分離レベルの指定
        /// ・NC：NotConnect
        /// ・NT：NoTransaction
        /// ・RU：ReadUncommitted
        /// ・RC：ReadCommitted
        /// ・RR：RepeatableRead
        /// ・SZ：Serializable
        /// ・SS：Snapshot
        /// ・DF：DefaultTransaction;
        /// </param>
        [TestCase("screen1", "control1", "SQL%individual%static%-", "User1", "Hostname1", "RC", "1247", "Symphony", "9008031613",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-001N")]
        [TestCase("screen2", "control2", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", "1247", "Harman", "9008031614",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-002N")]
        [TestCase("screen3", "control3", "SQL%common%static%-", "User3", "Hostname3", "RC", "1247", "Symphony Harman", "9008031615",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-003N")]
        [TestCase("screen4", "control4", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", "1247", "Harman Symphony", "9008031616",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-004N")]
        [TestCase("screen5", "control5", "SQL%generate%static%-", "User5", "Hostname5", "RC", null, "Harman Connected", "9008031617",
                  ExpectedException = typeof(System.ArgumentNullException), TestName = "TestID-005A")]
        [TestCase("screen6", "control6", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC", "111", "Symphony Connected", "9008031618",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-006A")]
        [TestCase("screen6", "control6", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC", "", "Symphony Connected", "9008031618",
                  ExpectedException = typeof(System.FormatException), TestName = "TestID-007A")]
        public void _5_Update(
            string screenId, string controlId, string actionType,
            string userName, string ipAddress, string isolationLevel, string shipperID, string companyName, string phone)
        {
            string methodName = "Update";

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    screenId, controlId, methodName, actionType, new MyUserInfo(userName, ipAddress));

            // 情報の設定
            testParameterValue.ShipperID = int.Parse(shipperID);
            testParameterValue.CompanyName = companyName;
            testParameterValue.Phone = phone;

            // 戻り値
            TestReturnValue testReturnValue = null;

            // Ｂ層呼出し
            LayerB layerB = new LayerB();

            try
            {
                testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(
                    testParameterValue, this.SelectIsolationLevel(isolationLevel));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            // 戻り値検証。
            Assert.AreEqual(testReturnValue.ErrorFlag, false);
            Assert.Contains(Convert.ToInt32(testReturnValue.Obj), (new int[] { 0, 1 }));

        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="screenId">
        /// 画面ID
        /// </param>
        /// <param name="controlId">
        /// コントロールID
        /// </param>
        /// <param name="actionType">
        /// アクションタイプ
        /// ・Dap  ：データプロバイダ
        /// ・Mode1：個別、共通、自動生成
        /// ・Mode2：静的、動的
        /// </param>
        /// <param name="userName">
        /// ユーザ名
        /// </param>
        /// <param name="ipAddress">
        /// IPアドレス（ホスト名）
        /// </param>
        /// <param name="isolationLevel">
        /// 分離レベルの指定
        /// ・NC：NotConnect
        /// ・NT：NoTransaction
        /// ・RU：ReadUncommitted
        /// ・RC：ReadCommitted
        /// ・RR：RepeatableRead
        /// ・SZ：Serializable
        /// ・SS：Snapshot
        /// ・DF：DefaultTransaction;
        /// </param>
        [TestCase("screen1", "control1", "SQL%individual%static%-", "User1", "Hostname1", "RC", "1248",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-001N")]
        [TestCase("screen2", "control2", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC", "1249",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-002N")]
        [TestCase("screen3", "control3", "SQL%common%static%-", "User3", "Hostname3", "RC", "1250",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-003N")]
        [TestCase("screen4", "control4", "SQL%common%dynamic%-", "User4", "Hostname4", "RC", "1251",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-004N")]
        [TestCase("screen5", "control5", "SQL%generate%static%-", "User5", "Hostname5", "RC", null,
                  ExpectedException = typeof(System.ArgumentNullException), TestName = "TestID-005A")]
        [TestCase("screen6", "control6", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC", "111",
                  ExpectedException = typeof(System.NullReferenceException), TestName = "TestID-006A")]
        [TestCase("screen6", "control6", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC", "",
                  ExpectedException = typeof(System.FormatException), TestName = "TestID-007A")]
        public void _5_Delete(
            string screenId, string controlId, string actionType,
            string userName, string ipAddress, string isolationLevel, string shipperID)
        {
            string methodName = "Delete";

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    screenId, controlId, methodName, actionType, new MyUserInfo(userName, ipAddress));

            // 情報の設定
            testParameterValue.ShipperID = int.Parse(shipperID);

            // 戻り値
            TestReturnValue testReturnValue = null;

            // Ｂ層呼出し
            LayerB layerB = new LayerB();

            try
            {
                testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(
                    testParameterValue, this.SelectIsolationLevel(isolationLevel));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            // 戻り値検証。
            Assert.AreEqual(testReturnValue.ErrorFlag, false);
            Assert.Contains(Convert.ToInt32(testReturnValue.Obj), (new int[] { 0, 1 }));
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

        /// <summary>
        /// テストケース後処理
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("これはテストケース後処理です。テストケースごとに実行されます。");
        }

        #endregion

        /// <summary>
        /// テスト後処理
        /// </summary>
        [TestFixtureTearDown]
        public void CleanUp()
        {
            Console.WriteLine("これはテスト後処理です。最後に一度だけ実行されます。");
        }
    }
}
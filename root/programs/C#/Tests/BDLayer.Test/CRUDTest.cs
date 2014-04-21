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
        [TestCase("screen1", "control1", "SQL%individual%static%-", "User1", "Hostname1", "RC")]
        [TestCase("screen2", "control2", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC")]
        [TestCase("screen3", "control3", "SQL%common%static%-", "User3", "Hostname3", "RC")]
        [TestCase("screen4", "control4", "SQL%common%dynamic%-", "User4", "Hostname4", "RC")]
        [TestCase("screen5", "control5", "SQL%generate%static%-", "User5", "Hostname5", "RC")]
        [TestCase("screen6", "control6", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC")]
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
        [TestCase("screen1", "control1", "SelectAll_DT", "SQL%individual%static%-", "User1", "Hostname1", "RC")]
        [TestCase("screen2", "control2", "SelectAll_DT", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC")]
        [TestCase("screen3", "control3", "SelectAll_DT", "SQL%common%static%-", "User3", "Hostname3", "RC")]
        [TestCase("screen4", "control4", "SelectAll_DT", "SQL%common%dynamic%-", "User4", "Hostname4", "RC")]
        [TestCase("screen5", "control5", "SelectAll_DT", "SQL%generate%static%-", "User5", "Hostname5", "RC")]
        [TestCase("screen6", "control6", "SelectAll_DT", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC")]
        [TestCase("screen1", "control1", "SelectAll_DS", "SQL%individual%static%-", "User1", "Hostname1", "RC")]
        [TestCase("screen2", "control2", "SelectAll_DS", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC")]
        [TestCase("screen3", "control3", "SelectAll_DS", "SQL%common%static%-", "User3", "Hostname3", "RC")]
        [TestCase("screen4", "control4", "SelectAll_DS", "SQL%common%dynamic%-", "User4", "Hostname4", "RC")]
        [TestCase("screen5", "control5", "SelectAll_DS", "SQL%generate%static%-", "User5", "Hostname5", "RC")]
        [TestCase("screen6", "control6", "SelectAll_DS", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC")]
        //[TestCase("screen1", "control1", "SelectAll_DR", "SQL%individual%static%-", "User1", "Hostname1", "RC")]
        //[TestCase("screen2", "control2", "SelectAll_DR", "SQL%individual%dynamic%-", "User2", "Hostname2", "RC")]
        //[TestCase("screen3", "control3", "SelectAll_DR", "SQL%common%static%-", "User3", "Hostname3", "RC")]
        //[TestCase("screen4", "control4", "SelectAll_DR", "SQL%common%dynamic%-", "User4", "Hostname4", "RC")]
        //[TestCase("screen5", "control5", "SelectAll_DR", "SQL%generate%static%-", "User5", "Hostname5", "RC")]
        //[TestCase("screen6", "control6", "SelectAll_DR", "SQL%generate%dynamic%-", "User6", "Hostname6", "RC")]
        public void _2_SelectAll_XX(
            string screenId, string controlId,
            string methodName, string actionType,
            string userName, string ipAddress, string isolationLevel)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    screenId, controlId, methodName, actionType, new MyUserInfo(userName, ipAddress));

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
                    break;
                default:
                    break;

            }
        }

        #endregion

            //    case "SelectAll_DSQL":
            //        break;
            //    case "Select":
            //        break;
            //    case "Insert":
            //        break;
            //    case "Update":
            //        break;
            //    case "Delete":
            //        break;
            //    default:
            //        break;
        //}

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
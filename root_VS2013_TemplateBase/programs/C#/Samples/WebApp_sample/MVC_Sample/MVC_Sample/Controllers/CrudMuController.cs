//**********************************************************************************
//* サンプル アプリ・コントローラ
//**********************************************************************************

//**********************************************************************************
//* クラス名        ：CrudMuController
//* クラス日本語名  ：Ajax.BeginForm用サンプル アプリ・コントローラ
//*
//* 作成日時        ：－
//* 作成者          ：sas 生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//*
//**********************************************************************************

//System
using System;
using System.Web;
using System.Web.Mvc;

using System.Data;

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

using MVC_Sample.Logic.Business;
using MVC_Sample.Logic.Common;
using MVC_Sample.Models;

namespace MVC_Sample.Controllers
{
    /// <summary>
    /// Ajax.BeginForm用サンプル アプリ・コントローラ
    /// </summary>
    public class CrudMuController : MyBaseMVController
    {
        //
        // GET: /CrudMu/

        /// <summary>
        /// 画面の初期表示
        /// </summary>
        /// <returns>初期表示状態の画面 (ViewResult)</returns>
        [HttpGet]
        public ActionResult Index()
        {
            CrudModel model = new CrudModel();
            return View(model);
        }

        /// <summary>
        /// Shippers テーブルのレコード数をカウントする
        /// </summary>
        /// <param name="ddlDap">データプロバイダ</param>
        /// <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        /// <param name="ddlMode2">静的、動的のクエリ モード</param>
        /// <param name="ddlExRollback">コミット、ロールバック</param>
        /// <returns>Shippers テーブルのレコード数を表示するためのPartialViewResult</returns>
        public ActionResult GetCount(string ddlDap, string ddlMode1, string ddlMode2, string ddlExRollback)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button1", "SelectCount",
                    ddlDap + "%" + ddlMode1 + "%" + ddlMode2 + "%" + ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);

            // 結果表示するメッセージ
            string message = "";
            CrudModel model = new CrudModel();

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;
            }
            else
            {
                // 結果（正常系）
                message = testReturnValue.Obj.ToString() + "件のデータがあります";
            }

            // メッセージを設定。
            model.Message = message;

            // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            return PartialView("_MessageView", model);
        }

        /// <summary>
        /// Shippers テーブルのレコード全件を DataTable として取得する
        /// </summary>
        /// <param name="ddlDap">データプロバイダ</param>
        /// <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        /// <param name="ddlMode2">静的、動的のクエリ モード</param>
        /// <param name="ddlExRollback">コミット、ロールバック</param>
        /// <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        public ActionResult SelectAll_DT(string ddlDap, string ddlMode1, string ddlMode2, string ddlExRollback)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button2", "SelectAll_DT",
                    ddlDap + "%" + ddlMode1 + "%" + ddlMode2 + "%" + ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);

            // 結果表示するメッセージ
            string message = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                // Ajax.BeginFormでは、UpdateTargetIdで指定した
                // ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                // ＃ここではjQueryを使用している。
                string scriptText = "$('#lblMessage').text('" + message + "');";
                return JavaScript(scriptText);
            }
            else
            {
                // 結果（正常系）
                CrudModel model = new CrudModel();
                model.shippers = new DataSets.DsNorthwind.ShippersDataTable();
                DataTable dt = (DataTable)testReturnValue.Obj;

                foreach (DataRow row in dt.Rows)
                {
                    DataSets.DsNorthwind.ShippersRow srow = model.shippers.NewShippersRow();
                    srow.ShipperID = int.Parse(row[0].ToString());
                    srow.CompanyName = row[1].ToString();
                    srow.Phone = row[2].ToString();

                    model.shippers.Rows.Add(srow);
                }

                // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
                return PartialView("_ChartView", model);
            }
        }

        /// <summary>
        /// Shippers テーブルのレコード全件を DataSet として取得する
        /// </summary>
        /// <param name="ddlDap">データプロバイダ</param>
        /// <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        /// <param name="ddlMode2">静的、動的のクエリ モード</param>
        /// <param name="ddlExRollback">コミット、ロールバック</param>
        /// <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        public ActionResult SelectAll_DS(string ddlDap, string ddlMode1, string ddlMode2, string ddlExRollback)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button3", "SelectAll_DS",
                    ddlDap + "%" + ddlMode1 + "%" + ddlMode2 + "%" + ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);

            // 結果表示するメッセージ
            string message = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                // Ajax.BeginFormでは、UpdateTargetIdで指定した
                // ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                // ＃ここではjQueryを使用している。
                string scriptText = "$('#lblMessage').text('" + message + "');";
                return JavaScript(scriptText);
            }
            else
            {
                // 結果（正常系）
                CrudModel model = new CrudModel();
                model.shippers = new DataSets.DsNorthwind.ShippersDataTable();
                DataSet ds = (DataSet)testReturnValue.Obj;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DataSets.DsNorthwind.ShippersRow srow = model.shippers.NewShippersRow();
                    srow.ShipperID = int.Parse(row[0].ToString());
                    srow.CompanyName = row[1].ToString();
                    srow.Phone = row[2].ToString();

                    model.shippers.Rows.Add(srow);
                }

                // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
                return PartialView("_ChartView", model);
            }
        }

        /// <summary>
        /// Shippers テーブルのレコード全件を DataReader として取得する
        /// </summary>
        /// <param name="ddlDap">データプロバイダ</param>
        /// <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        /// <param name="ddlMode2">静的、動的のクエリ モード</param>
        /// <param name="ddlExRollback">コミット、ロールバック</param>
        /// <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        public ActionResult SelectAll_DR(string ddlDap, string ddlMode1, string ddlMode2, string ddlExRollback)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button4", "SelectAll_DR",
                    ddlDap + "%" + ddlMode1 + "%" + ddlMode2 + "%" + ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);

            // 結果表示するメッセージ
            string message = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                // Ajax.BeginFormでは、UpdateTargetIdで指定した
                // ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                // ＃ここではjQueryを使用している。
                string scriptText = "$('#lblMessage').text('" + message + "');";
                return JavaScript(scriptText);
            }
            else
            {
                // 結果（正常系）
                CrudModel model = new CrudModel();
                model.shippers = new DataSets.DsNorthwind.ShippersDataTable();
                DataTable dt = (DataTable)testReturnValue.Obj;

                foreach (DataRow row in dt.Rows)
                {
                    DataSets.DsNorthwind.ShippersRow srow = model.shippers.NewShippersRow();
                    srow.ShipperID = int.Parse(row[0].ToString());
                    srow.CompanyName = row[1].ToString();
                    srow.Phone = row[2].ToString();

                    model.shippers.Rows.Add(srow);
                }

                // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
                return PartialView("_ChartView", model);
            }
        }

        /// <summary>
        /// Shippers テーブルのレコード全件を、動的 SQL を使用して取得する
        /// </summary>
        /// <param name="ddlDap">データプロバイダ</param>
        /// <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        /// <param name="ddlMode2">静的、動的のクエリ モード</param>
        /// <param name="ddlExRollback">コミット、ロールバック</param>
        /// <param name="ddlOrderColumn">並び替え対象列</param>
        /// <param name="ddlOrderSequence">昇順・降順</param>
        /// <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        public ActionResult SelectAll_DSQL(string ddlDap, string ddlMode1, string ddlMode2, string ddlExRollback, string ddlOrderColumn, string ddlOrderSequence)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button5", "SelectAll_DSQL",
                    ddlDap + "%" + ddlMode1 + "%" + ddlMode2 + "%" + ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));

            // 動的SQLの要素を設定
            testParameterValue.OrderColumn = ddlOrderColumn;
            testParameterValue.OrderSequence = ddlOrderSequence;

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);

            // 結果表示するメッセージ
            string message = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                // Ajax.BeginFormでは、UpdateTargetIdで指定した
                // ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                // ＃ここではjQueryを使用している。
                string scriptText = "$('#lblMessage').text('" + message + "');";
                return JavaScript(scriptText);
            }
            else
            {
                // 結果（正常系）
                CrudModel model = new CrudModel();
                model.shippers = new DataSets.DsNorthwind.ShippersDataTable();
                DataTable dt = (DataTable)testReturnValue.Obj;

                foreach (DataRow row in dt.Rows)
                {
                    DataSets.DsNorthwind.ShippersRow srow = model.shippers.NewShippersRow();
                    srow.ShipperID = int.Parse(row[0].ToString());
                    srow.CompanyName = row[1].ToString();
                    srow.Phone = row[2].ToString();

                    model.shippers.Rows.Add(srow);
                }

                // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
                return PartialView("_ChartView", model);
            }
        }

        /// <summary>
        /// ShipperId をキーにして Shippers テーブルのレコードを取得する
        /// </summary>
        /// <param name="ddlDap">データプロバイダ</param>
        /// <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        /// <param name="ddlMode2">静的、動的のクエリ モード</param>
        /// <param name="ddlExRollback">コミット、ロールバック</param>
        /// <param name="textBox1">ShipperId</param>
        /// <returns>Shippers テーブルのレコードを表示するJavaScriptResult</returns>
        public ActionResult Select(string ddlDap, string ddlMode1, string ddlMode2, string ddlExRollback, string textBox1)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button6", "Select",
                    ddlDap + "%" + ddlMode1 + "%" + ddlMode2 + "%" + ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));

            // 動的SQLの要素を設定
            testParameterValue.ShipperID = int.Parse(textBox1);

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);

            // 結果表示するメッセージ
            string message = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                CrudModel model = new CrudModel() { Message = message };

                // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
                return PartialView("_MessageView", model);
            }
            else
            {
                // 結果（正常系）

                // Ajax.BeginFormでは、UpdateTargetIdで指定した
                // ターゲット以外を更新する場合、JavaScriptでの対応が必要。
                // ＃ここではjQueryを使用している。
                string scriptText = 
                    string.Format("$('#textBox1').val('{0}');$('#textBox2').val('{1}');$('#textBox3').val('{2}');", 
                        testReturnValue.ShipperID, 
                        testReturnValue.CompanyName, 
                        testReturnValue.Phone);
                return JavaScript(scriptText);
            }
        }

        /// <summary>
        /// Shippers テーブルに新規レコードを追加する
        /// </summary>
        /// <param name="ddlDap">データプロバイダ</param>
        /// <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        /// <param name="ddlMode2">静的、動的のクエリ モード</param>
        /// <param name="ddlExRollback">コミット、ロールバック</param>
        /// <param name="textBox2">CompanyName</param>
        /// <param name="textBox3">Phone</param>
        /// <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        public ActionResult Insert(string ddlDap, string ddlMode1, string ddlMode2, string ddlExRollback, string textBox2, string textBox3)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button7", "Insert",
                    ddlDap + "%" + ddlMode1 + "%" + ddlMode2 + "%" + ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));

            // 動的SQLの要素を設定
            testParameterValue.CompanyName = textBox2;
            testParameterValue.Phone = textBox3;

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);

            // 結果表示するメッセージ
            string message = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;
            }
            else
            {
                // 結果（正常系）
                message = testReturnValue.Obj.ToString() + "件追加";
            }

            CrudModel model = new CrudModel() { Message = message };

            // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            return PartialView("_MessageView", model);
        }

        /// <summary>
        /// Shippers テーブルに新規レコードを更新する
        /// </summary>
        /// <param name="ddlDap">データプロバイダ</param>
        /// <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        /// <param name="ddlMode2">静的、動的のクエリ モード</param>
        /// <param name="ddlExRollback">コミット、ロールバック</param>
        /// <param name="textBox1">ShipperId</param>
        /// <param name="textBox2">CompanyName</param>
        /// <param name="textBox3">Phone</param>
        /// <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        public ActionResult Update(string ddlDap, string ddlMode1, string ddlMode2, string ddlExRollback, string textBox1, string textBox2, string textBox3)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button8", "Update",
                    ddlDap + "%" + ddlMode1 + "%" + ddlMode2 + "%" + ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));

            // 動的SQLの要素を設定
            testParameterValue.ShipperID = int.Parse(textBox1);
            testParameterValue.CompanyName = textBox2;
            testParameterValue.Phone = textBox3;

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);

            // 結果表示するメッセージ
            string message = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;
            }
            else
            {
                // 結果（正常系）
                message = testReturnValue.Obj.ToString() + "件更新";
            }

            CrudModel model = new CrudModel() { Message = message };

            // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            return PartialView("_MessageView", model);
        }

        /// <summary>
        /// Shippers テーブルに新規レコードを削除する
        /// </summary>
        /// <param name="ddlDap">データプロバイダ</param>
        /// <param name="ddlMode1">静的、動的のクエリ モード（共通Dao選択時）</param>
        /// <param name="ddlMode2">静的、動的のクエリ モード</param>
        /// <param name="ddlExRollback">コミット、ロールバック</param>
        /// <param name="textBox1">ShipperId</param>
        /// <returns>Shippers テーブルのレコードを表示するPartialViewResult</returns>
        public ActionResult Delete(string ddlDap, string ddlMode1, string ddlMode2, string ddlExRollback, string textBox1)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button9", "Delete",
                    ddlDap + "%" + ddlMode1 + "%" + ddlMode2 + "%" + ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));

            // 動的SQLの要素を設定
            testParameterValue.ShipperID = int.Parse(textBox1);

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);

            // 結果表示するメッセージ
            string message = "";

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;
            }
            else
            {
                // 結果（正常系）
                message = testReturnValue.Obj.ToString() + "件削除";
            }

            CrudModel model = new CrudModel() { Message = message };

            // Ajax.BeginFormでは、以下のように記述することで部分更新が可能。
            return PartialView("_MessageView", model);
        }
        
        /// <summary>画面遷移する</summary>
        /// <returns>画面遷移のためのJavaScriptResult</returns>
        public ActionResult Transitions()
        {
            return JavaScript("location.href='/CrudMu2/';");
        }
    }
}

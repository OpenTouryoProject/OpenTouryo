//**********************************************************************************
//* サンプル アプリ・コントローラ
//**********************************************************************************

// テスト用クラスなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Crud1Controller
//* クラス日本語名  ：knockout.js用サンプル アプリ・コントローラ
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         ＸＸＸＸ
//**********************************************************************************

using SPA_Sample.Logic.Business;
using SPA_Sample.Logic.Common;
using SPA_Sample.Models;

using System.Data;
using System.Web.Mvc;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Public.Db;

namespace SPA_Sample.Controllers
{
    /// <summary>
    /// Crud1Controller
    /// knockout.js用サンプル アプリ・コントローラ
    /// </summary>
    public class Crud1Controller : Controller
    {
        /// <summary>
        /// GET: /Crud1/
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Index()
        {
            return View();
        }
    }

    /// <summary>GetCountController</summary>
    public class GetCountController : ApiController
    {
        /// <summary>
        /// POST api/GetCount
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(WebApiParams param)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button1", "SelectCount",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
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

            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                dic.Add("Error", message);
            }
            else
            {
                // 結果（正常系）
                message = testReturnValue.Obj.ToString() + "件のデータがあります";

                dic.Add("Message", message);
            }

            //dic.Add("Message", "Test");
            return Request.CreateResponse(HttpStatusCode.OK, dic);
        }
    }

    /// <summary>SelectDTController</summary>
    public class SelectDTController : ApiController
    {
        /// <summary>
        /// POST api/SelectDT
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(WebApiParams param)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button1", "SelectAll_DT",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
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

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("Error", message);

                return Request.CreateResponse(HttpStatusCode.OK, dic);
            }
            else
            {
                // 結果（正常系）
                DataTable dt = (DataTable)testReturnValue.Obj;

                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

                foreach (DataRow row in dt.Rows)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add(dt.Columns[0].ColumnName, row[0].ToString());
                    dic.Add(dt.Columns[1].ColumnName, row[1].ToString());
                    dic.Add(dt.Columns[2].ColumnName, row[2].ToString());

                    list.Add(dic);
                }

                return Request.CreateResponse(HttpStatusCode.OK, list);
            }
        }
    }

    /// <summary>SelectDSController</summary>
    public class SelectDSController : ApiController
    {
        /// <summary>
        /// POST api/SelectDS
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(WebApiParams param)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button1", "SelectAll_DS",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
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

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("Error", message);

                return Request.CreateResponse(HttpStatusCode.OK, dic);
            }
            else
            {
                // 結果（正常系）
                DataTable dt = ((DataSet)testReturnValue.Obj).Tables[0];

                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

                foreach (DataRow row in dt.Rows)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add(dt.Columns[0].ColumnName, row[0].ToString());
                    dic.Add(dt.Columns[1].ColumnName, row[1].ToString());
                    dic.Add(dt.Columns[2].ColumnName, row[2].ToString());

                    list.Add(dic);
                }

                return Request.CreateResponse(HttpStatusCode.OK, list);
            }
        }
    }

    /// <summary>SelectDRController</summary>
    public class SelectDRController : ApiController
    {
        /// <summary>
        /// POST api/SelectDR
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(WebApiParams param)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button1", "SelectAll_DR",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
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

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("Error", message);

                return Request.CreateResponse(HttpStatusCode.OK, dic);
            }
            else
            {
                // 結果（正常系）
                DataTable dt = (DataTable)testReturnValue.Obj;

                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

                foreach (DataRow row in dt.Rows)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    //dic.Add(dt.Columns[0].ColumnName, row[0].ToString());
                    //dic.Add(dt.Columns[1].ColumnName, row[1].ToString());
                    //dic.Add(dt.Columns[2].ColumnName, row[2].ToString());
                    dic.Add("ShipperID", row[0].ToString());
                    dic.Add("CompanyName", row[1].ToString());
                    dic.Add("Phone", row[2].ToString());

                    list.Add(dic);
                }

                return Request.CreateResponse(HttpStatusCode.OK, list);
            }
        }
    }

    /// <summary>SelectDSQLController</summary>
    public class SelectDSQLController : ApiController
    {
        /// <summary>
        /// POST api/SelectDSQL
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(WebApiParams param)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button1", "SelectAll_DSQL",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));
            testParameterValue.OrderColumn = param.OrderColumn;
            testParameterValue.OrderSequence = param.OrderSequence;

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

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("Error", message);

                return Request.CreateResponse(HttpStatusCode.OK, dic);
            }
            else
            {
                // 結果（正常系）
                DataTable dt = (DataTable)testReturnValue.Obj;

                List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

                foreach (DataRow row in dt.Rows)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add(dt.Columns[0].ColumnName, row[0].ToString());
                    dic.Add(dt.Columns[1].ColumnName, row[1].ToString());
                    dic.Add(dt.Columns[2].ColumnName, row[2].ToString());

                    list.Add(dic);
                }

                return Request.CreateResponse(HttpStatusCode.OK, list);
            }
        }
    }

    /// <summary>SelectController</summary>
    public class SelectController : ApiController
    {
        /// <summary>
        /// POST api/Select
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(WebApiParams param)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button1", "Select",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));
            testParameterValue.ShipperID = param.ShipperId;

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);

            // 結果表示するメッセージ
            string message = "";

            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                dic.Add("Error", message);
            }
            else
            {
                // 結果（正常系）
                dic.Add("ShipperID", testReturnValue.ShipperID.ToString());
                dic.Add("CompanyName", testReturnValue.CompanyName);
                dic.Add("Phone", testReturnValue.Phone);
            }

            return Request.CreateResponse(HttpStatusCode.OK, dic);
        }
    }

    /// <summary>InsertController</summary>
    public class InsertController : ApiController
    {
        /// <summary>
        /// POST api/Insert
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(WebApiParams param)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button1", "Insert",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));
            testParameterValue.CompanyName = param.CompanyName;
            testParameterValue.Phone = param.Phone;

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);

            // 結果表示するメッセージ
            string message = "";

            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                dic.Add("Error", message);
            }
            else
            {
                // 結果（正常系）
                message = testReturnValue.Obj.ToString() + "件追加";

                dic.Add("Message", message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, dic);
        }
    }

    /// <summary>UpdateController</summary>
    public class UpdateController : ApiController
    {
        /// <summary>
        /// POST api/Update
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(WebApiParams param)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button1", "Update",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));
            testParameterValue.ShipperID = param.ShipperId;
            testParameterValue.CompanyName = param.CompanyName;
            testParameterValue.Phone = param.Phone;

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);

            // 結果表示するメッセージ
            string message = "";

            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                dic.Add("Error", message);
            }
            else
            {
                // 結果（正常系）
                message = testReturnValue.Obj.ToString() + "件更新";

                dic.Add("Message", message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, dic);
        }
    }

    /// <summary>DeleteController</summary>
    public class DeleteController : ApiController
    {
        /// <summary>
        /// POST api/Delete
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(WebApiParams param)
        {
            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "CrudMu", "button1", "Delete",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo("aaa", "192.168.1.1"));
            testParameterValue.ShipperID = param.ShipperId;

            // 戻り値
            TestReturnValue testReturnValue;

            // 分離レベルの設定
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

            // Ｂ層呼出し＋都度コミット
            LayerB layerB = new LayerB();
            testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testParameterValue, iso);

            // 結果表示するメッセージ
            string message = "";

            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (testReturnValue.ErrorFlag == true)
            {
                // 結果（業務続行可能なエラー）
                message = "ErrorMessageID:" + testReturnValue.ErrorMessageID + ";";
                message += "ErrorMessage:" + testReturnValue.ErrorMessage + ";";
                message += "ErrorInfo:" + testReturnValue.ErrorInfo;

                dic.Add("Error", message);
            }
            else
            {
                // 結果（正常系）
                message = testReturnValue.Obj.ToString() + "件削除";

                dic.Add("Message", message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, dic);
        }
    }
}

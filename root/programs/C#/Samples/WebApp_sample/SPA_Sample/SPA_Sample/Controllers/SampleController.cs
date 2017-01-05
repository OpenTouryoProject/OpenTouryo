using SPA_Sample.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

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
using SPA_Sample.Codes.Business;
using SPA_Sample.Codes.Common;

namespace SPA_Sample.Controllers
{
    public class SampleController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }

    public class GetCountController : ApiController
    {
        // POST api/GetCount
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

    public class SelectDTController : ApiController
    {
        // POST api/SelectDT
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

    public class SelectDSController : ApiController
    {
        // POST api/SelectDS
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

    public class SelectDRController : ApiController
    {
        // POST api/SelectDR
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

    public class SelectDSQLController : ApiController
    {
        // POST api/SelectDSQL
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

    public class SelectController : ApiController
    {
        // POST api/Select
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

    public class InsertController : ApiController
    {
        // POST api/Insert
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

    public class UpdateController : ApiController
    {
        // POST api/Update
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

    public class DeleteController : ApiController
    {
        // POST api/Delete
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

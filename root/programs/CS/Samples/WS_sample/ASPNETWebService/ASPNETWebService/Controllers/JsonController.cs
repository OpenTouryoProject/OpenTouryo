//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//  
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
//* クラス名        ：JsonController
//* クラス日本語名  ：ASP.NET WebAPI JSON-RPCの個別Webメソッドを公開するサービス インターフェイス基盤。
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/08/18  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

using System.Web.Http;
using System.Web.Http.Cors;
using System.Net;
using System.Net.Http;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Transmission;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Dto;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;

using WSIFType_sample;
using WSServer_sample.Common;

namespace ASPNETWebService.Controllers
{
    /// <summary>
    /// ASP.NET WebAPI JSON-RPCの個別Webメソッドを公開するサービス インターフェイス基盤。
    /// </summary>
    [EnableCors(
        // リソースへのアクセスを許可されている発生元
        origins: "*",
        // リソースによってサポートされているヘッダー
        headers: "*",
        // リソースによってサポートされているメソッド
        methods: "*",
        // 
        SupportsCredentials = true)]
    [MyBaseAsyncApiController()]
    [RoutePrefix("api/json")]
    public class JsonController : ApiController
    {
        #region 疎通テスト用

        /// <summary>
        /// 疎通テスト用
        /// http(s)://hostName:portNum/api/json/testで疎通テスト可能。
        /// </summary>
        /// <returns>string</returns>
        [HttpGet]
        [Route("test")]
        public string test()
        {
            return "test";
        }

        #endregion

        #region グローバル変数

        /// <summary>インプロセス呼び出しの名前解決シングルトン クラス</summary>
        /// <remarks>
        /// 初期化は起動時の１回のみであり、
        /// 読み取り専用のデータを保持する場合
        /// のみに適用するデザインパターンとする。
        /// </remarks>
        private static InProcessNameService IPR_NS = new InProcessNameService();

        #endregion

        /// <summary>非同期化のため</summary>
        public class AsyncRetVal
        {
            /// <summary>返すべきエラーの情報</summary>
            public Dictionary<string, string> WsErrorInfo = null;
            /// <summary>戻り値</summary>
            public BaseReturnValue ReturnValue = null;
        }

        #region ASP.NET WebAPI JSON-RPCのWebメソッド

        #region 個別部

        /// <summary>
        /// POST JsonController/SelectCount
        /// </summary>
        /// <param name="param">引数</param>
        /// <returns>戻り値</returns>
        [HttpPost]
        [Route("SelectCount")]
        public async Task<HttpResponseMessage> SelectCount(WebApiParams param)
        {
            // Claimを取得する。
            string userName, roles, scopes, ipAddress;
            MyBaseAsyncApiController.GetClaims(out userName, out roles, out scopes, out ipAddress);

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "JsonController", "SelectCount", "SelectCount",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo(userName, ipAddress));

            // 非同期呼び出し
            AsyncRetVal asyncRetVal = await this.Call("testInProcess", testParameterValue);

            object ret = null;

            if (asyncRetVal.WsErrorInfo != null)
            {
                // ランタイムエラー
                ret = new { ExceptionMSG = asyncRetVal.WsErrorInfo };
            }
            else
            {
                TestReturnValue testReturnValue = (TestReturnValue)asyncRetVal.ReturnValue;

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    asyncRetVal.WsErrorInfo = new Dictionary<string, string>();
                    asyncRetVal.WsErrorInfo["ErrorMessageID"] = testReturnValue.ErrorMessageID;
                    asyncRetVal.WsErrorInfo["ErrorMessage"] = testReturnValue.ErrorMessage;
                    asyncRetVal.WsErrorInfo["ErrorInfo"] = testReturnValue.ErrorInfo;

                    ret = new { ErrorMSG = asyncRetVal.WsErrorInfo };
                }
                else
                {
                    // 結果（正常系）
                    string message = testReturnValue.Obj.ToString() + "件のデータがあります";
                    ret = new { Message = message };
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ret);
        }

        /// <summary>
        /// POST JsonController/SelectAll_DT
        /// </summary>
        /// <param name="param">引数</param>
        /// <returns>戻り値</returns>
        [HttpPost]
        [Route("SelectAll_DT")]
        public async Task<HttpResponseMessage> SelectAll_DT(WebApiParams param)
        {
            // Claimを取得する。
            string userName, roles, scopes, ipAddress;
            MyBaseAsyncApiController.GetClaims(out userName, out roles, out scopes, out ipAddress);

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "JsonController", "SelectAll_DT", "SelectAll_DT",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo(userName, ipAddress));

            // 非同期呼び出し
            AsyncRetVal asyncRetVal = await this.Call("testInProcess", testParameterValue);

            object ret = null;

            if (asyncRetVal.WsErrorInfo != null)
            {
                // ランタイムエラー
                ret = new { ExceptionMSG = asyncRetVal.WsErrorInfo };
            }
            else
            {
                TestReturnValue testReturnValue = (TestReturnValue)asyncRetVal.ReturnValue;

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    asyncRetVal.WsErrorInfo = new Dictionary<string, string>();
                    asyncRetVal.WsErrorInfo["ErrorMessageID"] = testReturnValue.ErrorMessageID;
                    asyncRetVal.WsErrorInfo["ErrorMessage"] = testReturnValue.ErrorMessage;
                    asyncRetVal.WsErrorInfo["ErrorInfo"] = testReturnValue.ErrorInfo;

                    ret = new { ErrorMSG = asyncRetVal.WsErrorInfo };
                }
                else
                {
                    // 結果（正常系）
                    DataTable dt = (DataTable)testReturnValue.Obj;

                    // 一部、DataToDictionaryのテストコード
                    DataToDictionary d2d = null;
                    List<Dictionary<string, string>> list = null;

                    d2d = new DataToDictionary(
                        new Dictionary<string, string>()
                        {
                            { "ShipperID", "_ShipperID"},
                            { "CompanyName", "_CompanyName"},
                            { "Phone", "_Phone"}
                        },
                        null, null);
                    list = d2d.DataTableToDictionaryList(dt);
                    Debug.WriteLine(ObjectInspector.Inspect(list));

                    d2d = new DataToDictionary(null, null, null);
                    list = d2d.DataTableToDictionaryList(dt);
                    Debug.WriteLine(ObjectInspector.Inspect(list));
                    
                    ret = new { Message = "", Result = list };
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ret);
        }

        /// <summary>
        /// POST JsonController/SelectAll_DS
        /// </summary>
        /// <param name="param">引数</param>
        /// <returns>戻り値</returns>
        [HttpPost]
        [Route("SelectAll_DS")]
        public async Task<HttpResponseMessage> SelectAll_DS(WebApiParams param)
        {
            // Claimを取得する。
            string userName, roles, scopes, ipAddress;
            MyBaseAsyncApiController.GetClaims(out userName, out roles, out scopes, out ipAddress);

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "JsonController", "SelectAll_DS", "SelectAll_DS",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo(userName, ipAddress));

            // 非同期呼び出し
            AsyncRetVal asyncRetVal = await this.Call("testInProcess", testParameterValue);

            object ret = null;

            if (asyncRetVal.WsErrorInfo != null)
            {
                // ランタイムエラー
                ret = new { ExceptionMSG = asyncRetVal.WsErrorInfo };
            }
            else
            {
                TestReturnValue testReturnValue = (TestReturnValue)asyncRetVal.ReturnValue;

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    asyncRetVal.WsErrorInfo = new Dictionary<string, string>();
                    asyncRetVal.WsErrorInfo["ErrorMessageID"] = testReturnValue.ErrorMessageID;
                    asyncRetVal.WsErrorInfo["ErrorMessage"] = testReturnValue.ErrorMessage;
                    asyncRetVal.WsErrorInfo["ErrorInfo"] = testReturnValue.ErrorInfo;

                    ret = new { ErrorMSG = asyncRetVal.WsErrorInfo };
                }
                else
                {
                    // 結果（正常系）
                    DataTable dt = ((DataSet)testReturnValue.Obj).Tables[0];
                    DataToDictionary d2d = new DataToDictionary(null, null, null);
                    ret = new { Message = "", Result = d2d.DataTableToDictionaryList(dt) };
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ret);
        }

        /// <summary>
        /// POST JsonController/SelectAll_DR
        /// </summary>
        /// <param name="param">引数</param>
        /// <returns>戻り値</returns>
        [HttpPost]
        [Route("SelectAll_DR")]
        public async Task<HttpResponseMessage> SelectAll_DR(WebApiParams param)
        {
            // Claimを取得する。
            string userName, roles, scopes, ipAddress;
            MyBaseAsyncApiController.GetClaims(out userName, out roles, out scopes, out ipAddress);

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "JsonController", "SelectAll_DR", "SelectAll_DR",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo(userName, ipAddress));

            // 非同期呼び出し
            AsyncRetVal asyncRetVal = await this.Call("testInProcess", testParameterValue);

            object ret = null;

            if (asyncRetVal.WsErrorInfo != null)
            {
                // ランタイムエラー
                ret = new { ExceptionMSG = asyncRetVal.WsErrorInfo };
            }
            else
            {
                TestReturnValue testReturnValue = (TestReturnValue)asyncRetVal.ReturnValue;

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    asyncRetVal.WsErrorInfo = new Dictionary<string, string>();
                    asyncRetVal.WsErrorInfo["ErrorMessageID"] = testReturnValue.ErrorMessageID;
                    asyncRetVal.WsErrorInfo["ErrorMessage"] = testReturnValue.ErrorMessage;
                    asyncRetVal.WsErrorInfo["ErrorInfo"] = testReturnValue.ErrorInfo;

                    ret = new { ErrorMSG = asyncRetVal.WsErrorInfo };
                }
                else
                {
                    // 結果（正常系）
                    DataTable dt = (DataTable)testReturnValue.Obj;
                    DataToDictionary d2d = new DataToDictionary(
                        new Dictionary<string, string>
                        {
                            { "c1", "ShipperID" },
                            { "c2", "CompanyName" },
                            { "c3", "Phone" }
                        }, "", "");
                    ret = new { Message = "", Result = d2d.DataTableToDictionaryList(dt) };
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ret);
        }

        /// <summary>
        /// POST JsonController/SelectAll_DSQL
        /// </summary>
        /// <param name="param">引数</param>
        /// <returns>戻り値</returns>
        [HttpPost]
        [Route("SelectAll_DSQL")]
        public async Task<HttpResponseMessage> SelectAll_DSQL(WebApiParams param)
        {
            // Claimを取得する。
            string userName, roles, scopes, ipAddress;
            MyBaseAsyncApiController.GetClaims(out userName, out roles, out scopes, out ipAddress);

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "JsonController", "SelectAll_DSQL", "SelectAll_DSQL",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo(userName, ipAddress));

            testParameterValue.OrderColumn = param.OrderColumn;
            testParameterValue.OrderSequence = param.OrderSequence;

            // 非同期呼び出し
            AsyncRetVal asyncRetVal = await this.Call("testInProcess", testParameterValue);

            object ret = null;

            if (asyncRetVal.WsErrorInfo != null)
            {
                // ランタイムエラー
                ret = new { ExceptionMSG = asyncRetVal.WsErrorInfo };
            }
            else
            {
                TestReturnValue testReturnValue = (TestReturnValue)asyncRetVal.ReturnValue;

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    asyncRetVal.WsErrorInfo = new Dictionary<string, string>();
                    asyncRetVal.WsErrorInfo["ErrorMessageID"] = testReturnValue.ErrorMessageID;
                    asyncRetVal.WsErrorInfo["ErrorMessage"] = testReturnValue.ErrorMessage;
                    asyncRetVal.WsErrorInfo["ErrorInfo"] = testReturnValue.ErrorInfo;

                    ret = new { ErrorMSG = asyncRetVal.WsErrorInfo };
                }
                else
                {
                    // 結果（正常系）
                    DataTable dt = (DataTable)testReturnValue.Obj;
                    DataToDictionary d2d = new DataToDictionary(null, null, null);
                    ret = new { Message = "", Result = d2d.DataTableToDictionaryList(dt) };
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ret);
        }

        /// <summary>
        /// POST JsonController/Select
        /// </summary>
        /// <param name="param">引数</param>
        /// <returns>戻り値</returns>
        [HttpPost]
        [Route("Select")]
        public async Task<HttpResponseMessage> Select(WebApiParams param)
        {
            // Claimを取得する。
            string userName, roles, scopes, ipAddress;
            MyBaseAsyncApiController.GetClaims(out userName, out roles, out scopes, out ipAddress);

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "JsonController", "Select", "Select",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo(userName, ipAddress));

            testParameterValue.ShipperID = param.Shipper.ShipperID;

            // 非同期呼び出し
            AsyncRetVal asyncRetVal = await this.Call("testInProcess", testParameterValue);

            object ret = null;

            if (asyncRetVal.WsErrorInfo != null)
            {
                // ランタイムエラー
                ret = new { ExceptionMSG = asyncRetVal.WsErrorInfo };
            }
            else
            {
                TestReturnValue testReturnValue = (TestReturnValue)asyncRetVal.ReturnValue;

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    asyncRetVal.WsErrorInfo = new Dictionary<string, string>();
                    asyncRetVal.WsErrorInfo["ErrorMessageID"] = testReturnValue.ErrorMessageID;
                    asyncRetVal.WsErrorInfo["ErrorMessage"] = testReturnValue.ErrorMessage;
                    asyncRetVal.WsErrorInfo["ErrorInfo"] = testReturnValue.ErrorInfo;

                    ret = new { ErrorMSG = asyncRetVal.WsErrorInfo };
                }
                else
                {
                    // 結果（正常系）
                    Dictionary<string, string> dic = new Dictionary<string, string>()
                    {
                        { "ShipperID", testReturnValue.ShipperID.ToString()},
                        { "CompanyName", testReturnValue.CompanyName},
                        { "Phone", testReturnValue.Phone}
                    };
                    ret = new { Message = "", Result = dic };
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ret);
        }

        /// <summary>
        /// POST JsonController/Insert
        /// </summary>
        /// <param name="param">引数</param>
        /// <returns>戻り値</returns>
        [HttpPost]
        [Route("Insert")]
        public async Task<HttpResponseMessage> Insert(WebApiParams param)
        {
            // Claimを取得する。
            string userName, roles, scopes, ipAddress;
            MyBaseAsyncApiController.GetClaims(out userName, out roles, out scopes, out ipAddress);

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "JsonController", "Insert", "Insert",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo(userName, ipAddress));

            testParameterValue.CompanyName = param.Shipper.CompanyName;
            testParameterValue.Phone = param.Shipper.Phone;

            // 非同期呼び出し
            AsyncRetVal asyncRetVal = await this.Call("testInProcess", testParameterValue);

            object ret = null;

            if (asyncRetVal.WsErrorInfo != null)
            {
                // ランタイムエラー
                ret = new { ExceptionMSG = asyncRetVal.WsErrorInfo };
            }
            else
            {
                TestReturnValue testReturnValue = (TestReturnValue)asyncRetVal.ReturnValue;

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    asyncRetVal.WsErrorInfo = new Dictionary<string, string>();
                    asyncRetVal.WsErrorInfo["ErrorMessageID"] = testReturnValue.ErrorMessageID;
                    asyncRetVal.WsErrorInfo["ErrorMessage"] = testReturnValue.ErrorMessage;
                    asyncRetVal.WsErrorInfo["ErrorInfo"] = testReturnValue.ErrorInfo;

                    ret = new { ErrorMSG = asyncRetVal.WsErrorInfo };
                }
                else
                {
                    // 結果（正常系）
                    string message = testReturnValue.Obj.ToString() + "件追加";
                    
                    ret = new { Message = message };
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ret);
        }

        /// <summary>
        /// POST JsonController/Update
        /// </summary>
        /// <param name="param">引数</param>
        /// <returns>戻り値</returns>
        [HttpPost]
        [Route("Update")]
        public async Task<HttpResponseMessage> Update(WebApiParams param)
        {
            // Claimを取得する。
            string userName, roles, scopes, ipAddress;
            MyBaseAsyncApiController.GetClaims(out userName, out roles, out scopes, out ipAddress);

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "JsonController", "Update", "Update",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo(userName, ipAddress));

            testParameterValue.ShipperID = param.Shipper.ShipperID;
            testParameterValue.CompanyName = param.Shipper.CompanyName;
            testParameterValue.Phone = param.Shipper.Phone;

            // 非同期呼び出し
            AsyncRetVal asyncRetVal = await this.Call("testInProcess", testParameterValue);

            object ret = null;

            if (asyncRetVal.WsErrorInfo != null)
            {
                // ランタイムエラー
                ret = new { ExceptionMSG = asyncRetVal.WsErrorInfo };
            }
            else
            {
                TestReturnValue testReturnValue = (TestReturnValue)asyncRetVal.ReturnValue;

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    asyncRetVal.WsErrorInfo = new Dictionary<string, string>();
                    asyncRetVal.WsErrorInfo["ErrorMessageID"] = testReturnValue.ErrorMessageID;
                    asyncRetVal.WsErrorInfo["ErrorMessage"] = testReturnValue.ErrorMessage;
                    asyncRetVal.WsErrorInfo["ErrorInfo"] = testReturnValue.ErrorInfo;

                    ret = new { ErrorMSG = asyncRetVal.WsErrorInfo };
                }
                else
                {
                    // 結果（正常系）
                    string message = testReturnValue.Obj.ToString() + "件更新";
                    
                    ret = new { Message = message };
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ret);
        }

        /// <summary>
        /// POST JsonController/Delete
        /// </summary>
        /// <param name="param">引数</param>
        /// <returns>戻り値</returns>
        [HttpPost]
        [Route("Delete")]
        public async Task<HttpResponseMessage> Delete(WebApiParams param)
        {
            // Claimを取得する。
            string userName, roles, scopes, ipAddress;
            MyBaseAsyncApiController.GetClaims(out userName, out roles, out scopes, out ipAddress);

            // 引数クラスを生成
            // 下位（Ｂ・Ｄ層）は、テスト クラスを流用する
            TestParameterValue testParameterValue
                = new TestParameterValue(
                    "JsonController", "Delete", "Delete",
                    param.ddlDap + "%" + param.ddlMode1 + "%" + param.ddlMode2 + "%" + param.ddlExRollback,
                    new MyUserInfo(userName, ipAddress));

            testParameterValue.ShipperID = param.Shipper.ShipperID;

            // 非同期呼び出し
            AsyncRetVal asyncRetVal = await this.Call("testInProcess", testParameterValue);            

            object ret = null;

            if (asyncRetVal.WsErrorInfo != null)
            {
                // ランタイムエラー
                ret = new { ExceptionMSG = asyncRetVal.WsErrorInfo };
            }
            else
            {
                TestReturnValue testReturnValue = (TestReturnValue)asyncRetVal.ReturnValue;

                if (testReturnValue.ErrorFlag == true)
                {
                    // 結果（業務続行可能なエラー）
                    asyncRetVal.WsErrorInfo = new Dictionary<string, string>();
                    asyncRetVal.WsErrorInfo["ErrorMessageID"] = testReturnValue.ErrorMessageID;
                    asyncRetVal.WsErrorInfo["ErrorMessage"] = testReturnValue.ErrorMessage;
                    asyncRetVal.WsErrorInfo["ErrorInfo"] = testReturnValue.ErrorInfo;
                    
                    ret = new { ErrorMSG = asyncRetVal.WsErrorInfo };
                }
                else
                {
                    // 結果（正常系）
                    string message = testReturnValue.Obj.ToString() + "件削除";
                    
                    ret = new { Message = message };
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, ret);
        }

        #endregion

        #region 共通部

        /// <summary>ASP.NET WebAPI JSON-RPCの個別Webメソッドの共通部</summary>
        /// <param name="serviceName">サービス名</param>
        /// <param name="parameterValue">引数</param>
        /// <returns>
        /// AsyncRetVal（非同期化のため）
        /// ・WsErrorInfo：返すべきエラーの情報
        /// ・ReturnValue：戻り値
        /// </returns>
        private async Task<AsyncRetVal> Call(
            string serviceName, 
            BaseParameterValue parameterValue)
        {
            // ステータス
            string status = "－";
            
            #region 呼出し制御関係の変数

            // アセンブリ名
            string assemblyName = "";

            // クラス名
            string className = "";

            #endregion

            #region 引数・戻り値関係の変数

            BaseReturnValue returnValue = null;

            // エラー情報（XMLフォーマット）
            Dictionary<string, string> wsErrorInfo = new Dictionary<string, string>();

            // エラー情報（ログ出力用）
            string errorType = ""; // 2009/09/15-この行
            string errorMessageID = "";
            string errorMessage = "";
            string errorToString = "";

            #endregion

            try
            {
                // 開始ログの出力
                LogIF.InfoLog("SERVICE-IF", FxLiteral.SIF_STATUS_START);

                #region 名前解決

                // ★
                status = FxLiteral.SIF_STATUS_NAME_SERVICE;

                // 名前解決（インプロセス）
                JsonController.IPR_NS.NameResolution(serviceName, out assemblyName, out className);

                #endregion

                #region 引数の.NETオブジェクト化（ＵＯＣ）

                // ★
                status = FxLiteral.SIF_STATUS_DESERIALIZE;
                                
                // 引数クラスをパラメタ セットに格納
                object[] paramSet = new object[] { parameterValue, DbEnum.IsolationLevelEnum.User };

                #endregion

                #region 認証処理（ＵＯＣ）

                // MyBaseApiControllerに実装する。

                #endregion

                #region Ｂ層・Ｄ層呼出し

                // ★
                status = FxLiteral.SIF_STATUS_INVOKE;

                try
                {
                    // Ｂ層・Ｄ層呼出し

                    try
                    {
                        // Ｂ層・Ｄ層呼出し
                        Task<BaseReturnValue> result = (Task<BaseReturnValue>)Latebind.InvokeMethod(
                            assemblyName, className,
                            FxLiteral.TRANSMISSION_INPROCESS_ASYNC_METHOD_NAME, paramSet);
                        returnValue = await result;
                    }
                    catch (System.Reflection.TargetInvocationException rtEx)
                    {
                        //// InnerExceptionを投げなおす。
                        //throw rtEx.InnerException;

                        // スタックトレースを保って InnerException を throw
                        ExceptionDispatchInfo.Capture(rtEx.InnerException).Throw();
                    }
                }
                catch (System.Reflection.TargetInvocationException rtEx)
                {
                    // InnerExceptionを投げなおす。
                    throw rtEx.InnerException;
                }

                #endregion

                // ★
                status = "";

                // 戻り値を返す。
                return new AsyncRetVal
                {
                    WsErrorInfo = null,
                    ReturnValue = returnValue
                };
            }
            //catch (BusinessApplicationException baEx)
            //{
            // ここには来ない↑
            //}
            catch (BusinessSystemException bsEx)
            {
                // エラー情報を設定する。
                // システム例外
                wsErrorInfo["ErrorType"] = FxEnum.ErrorType.BusinessSystemException.ToString();
                wsErrorInfo["MessageID"] = bsEx.messageID;
                wsErrorInfo["Message"] = bsEx.Message;

                // ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.BusinessSystemException.ToString(); // 2009/09/15-この行
                errorMessageID = bsEx.messageID;
                errorMessage = bsEx.Message;

                errorToString = bsEx.ToString();

                // エラー情報を戻す。
                return new AsyncRetVal
                {
                    WsErrorInfo = wsErrorInfo,
                    ReturnValue = returnValue
                };
            }
            catch (FrameworkException fxEx)
            {
                // エラー情報を設定する。
                // フレームワーク例外
                // ★ インナーエクセプション情報は消失
                wsErrorInfo["ErrorType"] = FxEnum.ErrorType.FrameworkException.ToString();
                wsErrorInfo["MessageID"] = fxEx.messageID;
                wsErrorInfo["Message"] = fxEx.Message;
                
                // ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.FrameworkException.ToString(); // 2009/09/15-この行
                errorMessageID = fxEx.messageID;
                errorMessage = fxEx.Message;

                errorToString = fxEx.ToString();

                // エラー情報を戻す。
                return new AsyncRetVal
                {
                    WsErrorInfo = wsErrorInfo,
                    ReturnValue = returnValue
                };
            }
            catch (Exception ex)
            {
                // エラー情報を設定する。
                // フレームワーク例外
                // ★ インナーエクセプション情報は消失
                wsErrorInfo["ErrorType"] = FxEnum.ErrorType.ElseException.ToString();
                wsErrorInfo["MessageID"] = "-";
                wsErrorInfo["Message"] = ex.ToString();
                
                // ログ出力用の情報を保存
                errorType = FxEnum.ErrorType.ElseException.ToString(); // 2009/09/15-この行
                errorMessageID = "-";
                errorMessage = ex.Message;

                // どちらを戻すべきか？
                // Muの場合は、Messageがデフォ
                errorToString = ex.Message;
                //errorToString = ex.ToString();

                // エラー情報を戻す。
                return new AsyncRetVal
                {
                    WsErrorInfo = wsErrorInfo,
                    ReturnValue = returnValue
                };
            }
            finally
            {
                // 用途によってSessionを解放するかどうかを検討。

                //// Sessionステートレス
                //Session.Clear();
                //Session.Abandon();

                // 終了ログの出力
                if (status == "")
                {
                    // 終了ログ出力
                    LogIF.InfoLog("SERVICE-IF", "正常終了");
                }
                else
                {
                    // 終了ログ出力
                    LogIF.ErrorLog("SERVICE-IF",
                        "異常終了"
                        + "：" + status + "\r\n"
                        + "エラー タイプ：" + errorType + "\r\n" // 2009/09/15-この行
                        + "エラー メッセージID：" + errorMessageID + "\r\n"
                        + "エラー メッセージ：" + errorMessage + "\r\n"
                        + errorToString + "\r\n");
                }
            }
        }

        #endregion

        #endregion
    }
}

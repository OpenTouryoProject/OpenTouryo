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
//* クラス名        ：BatchUpdateController
//* クラス日本語名  ：DTO を使用したバッチ更新処理の WebAPI
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/20  玄人 幸道         新規作成（#570）
//**********************************************************************************

using System;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

using ASPNETWebService.Logic.Business;
using ASPNETWebService.Logic.Common;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Dto;
using Touryo.Infrastructure.Public.Security;

namespace ASPNETWebService.Controllers
{
    /// <summary>DTO を使用したバッチ更新処理の WebAPI</summary>
    /// <remarks>
    /// **DataTable を DTTables 経由で JSON にして往復させる。**
    ///
    ///   一覧 : DataTable → DTTable.FromDataTable(dt, keepOriginal: true) → DTTables → JSON
    ///   更新 : JSON → DTTables → ToDataTable() → RowState と Original が戻った DataTable
    ///
    /// **keepOriginal を立てないと、楽観排他が組めない。**（#567）
    /// Modified 行の WHERE には「取得時の値（Original）」が要るが、
    /// 素の JSON では現在値しか運べない。
    ///
    /// Ｂ層の呼び出しは MVC_Sample の Crud1Controller と同じく DoBusinessLogicAsync を使う。
    /// </remarks>
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
    [MyBaseAsyncApiController(httpAuthHeader:
        EnumHttpAuthHeader.None      // 認証無くても通すので、
        | EnumHttpAuthHeader.Bearer)] // Bearer認証の結果をGetClaimsで検証。
    [RoutePrefix("api/batchupdate")]
    public class BatchUpdateController : ApiController
    {
        /// <summary>テーブル名</summary>
        /// <remarks>DTTables の中の識別に使う。</remarks>
        private const string TableName = "Suppliers";

        #region 疎通

        /// <summary>疎通確認</summary>
        /// <returns>string</returns>
        /// <remarks>http(s)://hostName:portNum/api/batchupdate/test で疎通テスト可能。</remarks>
        [HttpGet]
        [Route("test")]
        public string test()
        {
            return "test";
        }

        #endregion

        #region 件数確認

        /// <summary>Suppliers の件数を返す</summary>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost]
        [Route("SelectCount")]
        public async Task<HttpResponseMessage> SelectCount()
        {
            SuppliersReturnValue returnValue = await this.CallLayerB("SelectCount", null);

            if (returnValue.ErrorFlag)
            {
                return this.CreateErrorResponse(returnValue);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { Count = returnValue.Count });
        }

        #endregion

        #region 一覧取得

        /// <summary>Suppliers の一覧を DTTables の JSON で返す</summary>
        /// <returns>HttpResponseMessage</returns>
        /// <remarks>
        /// **keepOriginal: true で作る。**
        /// このあとクライアント側で編集され、バッチ更新へ戻ってくるため、
        /// 取得時の値（Original）を保った状態で渡す必要がある。
        /// </remarks>
        [HttpPost]
        [Route("SelectAll")]
        public async Task<HttpResponseMessage> SelectAll()
        {
            SuppliersReturnValue returnValue = await this.CallLayerB("SelectAll", null);

            if (returnValue.ErrorFlag)
            {
                return this.CreateErrorResponse(returnValue);
            }

            DTTables dtts = new DTTables();
            dtts.Add(DTTable.FromDataTable(returnValue.Suppliers, true));

            return Request.CreateResponse(HttpStatusCode.OK,
                new { Suppliers = DTTables.DTTablesToJson(dtts) });
        }

        #endregion

        #region バッチ更新

        /// <summary>編集済みの DTTables を受け取り、バッチ更新する</summary>
        /// <param name="param">引数</param>
        /// <returns>HttpResponseMessage</returns>
        /// <remarks>
        /// **RowState と Original が復元されることが肝。**
        /// ToDataTable() が戻した DataTable は、そのまま
        /// dr.RowState での振り分けと dr[col, DataRowVersion.Original] に使える。
        /// </remarks>
        [HttpPost]
        [Route("BatchUpdate")]
        public async Task<HttpResponseMessage> BatchUpdate(BatchUpdateParams param)
        {
            if (param == null || string.IsNullOrEmpty(param.Suppliers))
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    new { ErrorMSG = "更新対象がありません。" });
            }

            DTTables dtts = DTTables.JsonToDTTables(param.Suppliers);

            DataTable dt = null;
            foreach (DTTable dtt in dtts)
            {
                if (dtt.TableName == BatchUpdateController.TableName)
                {
                    dt = dtt.ToDataTable();
                    break;
                }
            }

            SuppliersReturnValue returnValue = await this.CallLayerB("BatchUpdate", dt);

            if (returnValue.ErrorFlag)
            {
                return this.CreateErrorResponse(returnValue);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                returnValue.InsertCount,
                returnValue.UpdateCount,
                returnValue.DeleteCount
            });
        }

        #endregion

        #region ユーティリティ

        /// <summary>Ｂ層を呼ぶ</summary>
        /// <param name="methodName">メソッド名（UOC_〈methodName〉 が呼ばれる）</param>
        /// <param name="dt">バッチ更新の対象（不要なら null）</param>
        /// <returns>戻り値クラス</returns>
        /// <remarks>MVC_Sample の Crud1Controller と同じく DoBusinessLogicAsync を使う。</remarks>
        private async Task<SuppliersReturnValue> CallLayerB(string methodName, DataTable dt)
        {
            // Claim を取得する。
            string userName, roles, scopes, ipAddress;
            MyBaseAsyncApiController.GetClaims(out userName, out roles, out scopes, out ipAddress);

            SuppliersParameterValue parameterValue = new SuppliersParameterValue(
                "BatchUpdateController", "-", methodName, methodName,
                new MyUserInfo(userName, ipAddress));

            parameterValue.Suppliers = dt;

            SuppliersLayerB layerB = new SuppliersLayerB();

            // Ｂ層呼出し＋都度コミット
            return (SuppliersReturnValue)await layerB.DoBusinessLogicAsync(
                parameterValue, DbEnum.IsolationLevelEnum.DefaultTransaction);
        }

        /// <summary>業務エラーの応答を作る</summary>
        /// <param name="returnValue">戻り値クラス</param>
        /// <returns>HttpResponseMessage</returns>
        private HttpResponseMessage CreateErrorResponse(SuppliersReturnValue returnValue)
        {
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                ErrorMessageID = returnValue.ErrorMessageID,
                ErrorMessage = returnValue.ErrorMessage,
                ErrorInfo = returnValue.ErrorInfo
            });
        }

        #endregion
    }

    /// <summary>バッチ更新の引数</summary>
    public class BatchUpdateParams
    {
        /// <summary>DTTables の JSON</summary>
        public string Suppliers { get; set; }
    }
}

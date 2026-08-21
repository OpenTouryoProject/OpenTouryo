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
//*  2026/08/21  玄人 幸道         CSRFが成立しない前提をコメントに明記（CodeQL誤検知）
//**********************************************************************************

using System;
using System.Data;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using ASPNETWebService.Logic.Business;
using ASPNETWebService.Logic.Common;

using Touryo.Infrastructure.Business.Presentation;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Dto;
using Touryo.Infrastructure.Public.Security;

namespace ASPNETWebService.Controllers
{
    /// <summary>DTO を使用したバッチ更新処理の WebAPI</summary>
    /// <remarks>
    /// **net48 側と同じ内容。**（Samples/WS_sample/ASPNETWebService）
    /// 違うのは ASP.NET Core の作法だけで、DTO の往復のしかたは同じ。
    ///
    ///   一覧 : DataTable → DTTable.FromDataTable(dt, keepOriginal: true) → DTTables → JSON
    ///   更新 : JSON → DTTables → ToDataTable() → RowState と Original が戻った DataTable
    ///
    /// **keepOriginal を立てないと、楽観排他が組めない。**（#567）
    ///
    /// **OAuth2 の Resource Server であり、CSRF は成立しない。**
    ///   Cookie 認証を使わず（Startup.cs で UseAuthentication を無効化）、
    ///   認証は Authorization: Bearer ヘッダ。**ブラウザは自動付与しない。**
    ///   CORS も AllowCredentials を外してあり、資格情報を送らない構成。
    ///
    /// **ValidateAntiForgeryToken は付けないこと。**
    ///   非ブラウザのクライアント（Frameworks/Tests/TestWebAPIClient）は
    ///   トークンを持たないため、付けると疎通が壊れる。
    ///   CodeQL の cs/web/missing-token-validation は false positive として dismiss 済み。
    /// </remarks>
    [EnableCors]
    [ApiController]
    [MyBaseAsyncApiController(httpAuthHeader:
        EnumHttpAuthHeader.None      // 認証無くても通すので、
        | EnumHttpAuthHeader.Bearer)] // Bearer認証の結果をGetClaimsで検証。
    [Route("api/[controller]/[action]")]
    public class BatchUpdateController : ControllerBase
    {
        /// <summary>テーブル名</summary>
        private const string TableName = "Suppliers";

        /// <summary>シリアライズの設定</summary>
        /// <remarks>net48 側（WebAPI の既定）と同じく camelCase で返す。</remarks>
        private readonly JsonSerializerSettings JSS = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        #region 疎通

        /// <summary>疎通確認</summary>
        /// <returns>string</returns>
        /// <remarks>http(s)://hostName:portNum/api/batchupdate/test で疎通テスト可能。</remarks>
        [HttpGet]
        public string test()
        {
            return "test";
        }

        #endregion

        #region 件数確認

        /// <summary>Suppliers の件数を返す</summary>
        /// <returns>ContentResult</returns>
        [HttpPost]
        public async Task<ContentResult> SelectCount()
        {
            SuppliersReturnValue returnValue = await this.CallLayerB("SelectCount", null);

            if (returnValue.ErrorFlag)
            {
                return this.CreateErrorResponse(returnValue);
            }

            return this.Content(JsonConvert.SerializeObject(
                new { Count = returnValue.Count }, this.JSS));
        }

        #endregion

        #region 一覧取得

        /// <summary>Suppliers の一覧を DTTables の JSON で返す</summary>
        /// <returns>ContentResult</returns>
        /// <remarks>**keepOriginal: true で作る。** 編集後にバッチ更新へ戻ってくるため。</remarks>
        [HttpPost]
        public async Task<ContentResult> SelectAll()
        {
            SuppliersReturnValue returnValue = await this.CallLayerB("SelectAll", null);

            if (returnValue.ErrorFlag)
            {
                return this.CreateErrorResponse(returnValue);
            }

            DTTables dtts = new DTTables();
            dtts.Add(DTTable.FromDataTable(returnValue.Suppliers, true));

            return this.Content(JsonConvert.SerializeObject(
                new { Suppliers = DTTables.DTTablesToJson(dtts) }, this.JSS));
        }

        #endregion

        #region バッチ更新

        /// <summary>編集済みの DTTables を受け取り、バッチ更新する</summary>
        /// <param name="param">引数</param>
        /// <returns>ContentResult</returns>
        /// <remarks>
        /// **JSON で受ける（[FromForm] にしない）。**
        /// DTTables の JSON はそれ自体が長い文字列で、フォーム エンコードには向かない。
        /// </remarks>
        [HttpPost]
        public async Task<ContentResult> BatchUpdate(BatchUpdateParams param)
        {
            if (param == null || string.IsNullOrEmpty(param.Suppliers))
            {
                return this.Content(JsonConvert.SerializeObject(
                    new { ErrorMSG = "更新対象がありません。" }, this.JSS));
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

            return this.Content(JsonConvert.SerializeObject(new
            {
                returnValue.InsertCount,
                returnValue.UpdateCount,
                returnValue.DeleteCount
            }, this.JSS));
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
        /// <returns>ContentResult</returns>
        private ContentResult CreateErrorResponse(SuppliersReturnValue returnValue)
        {
            return this.Content(JsonConvert.SerializeObject(new
            {
                ErrorMessageID = returnValue.ErrorMessageID,
                ErrorMessage = returnValue.ErrorMessage,
                ErrorInfo = returnValue.ErrorInfo
            }, this.JSS));
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

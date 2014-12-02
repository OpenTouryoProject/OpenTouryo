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

//**********************************************************************************
//* クラス名            :LayerD.cs
//* クラス名クラス名     :
//*
//* 作成者              :Supragyan
//* クラス日本語名       :
//* 更新履歴
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  11/28/2014   Supragyan      LayerD class for AsyncProcessing Service
//**********************************************************************************

// System
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//業務フレームワーク
using Touryo.Infrastructure.Business.Dao;

//部品
using Touryo.Infrastructure.Public.Db;

namespace Touryo.Infrastructure.Framework.AsyncProcessingService
{

    /// <summary>
    /// LayerD class for AsyncProcessing Service
    /// </summary>
    class LayerD : MyBaseDao
    {
        public LayerD(BaseDam dam) : base(dam) { }

        /// <summary>
        /// Inserts async parameter values to database
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        /// <param name="asyncReturnValue"></param>
        public void Insert(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;

            if ((asyncParameterValue.ActionType.Split('%'))[2] == "Static")
            {
                // 静的SQL
                filename = "AsyncProcessingServiceInsert.sql";
            }
            else if ((asyncParameterValue.ActionType.Split('%'))[2] == "dynamic")
            {
                // 動的SQL
                filename = "AsyncProcessingServiceInsert.xml";
            }

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2(filename);

            // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            this.SetParameter("P2", asyncParameterValue.UserId);
            this.SetParameter("P3", asyncParameterValue.ProcessName);
            this.SetParameter("P4", asyncParameterValue.Data);
            this.SetParameter("P5", asyncParameterValue.RegistrationDateTime);
            this.SetParameter("P6", asyncParameterValue.ExecutionStartDateTime);
            this.SetParameter("P7", asyncParameterValue.NumberOfRetries);
            this.SetParameter("P8", asyncParameterValue.CompletionDateTime);
            this.SetParameter("P9", asyncParameterValue.Status);
            this.SetParameter("P10", asyncParameterValue.ProgressRate);
            this.SetParameter("P11", asyncParameterValue.Command);
            this.SetParameter("P12", asyncParameterValue.ReservedArea);

            object obj;

            //   -- 追加（件数を確認できる）
            obj = this.ExecInsUpDel_NonQuery();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            asyncReturnValue.Obj = obj;
        }

        #region Update

        /// <summary>
        /// Updates async parameter values to database
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        /// <param name="asyncReturnValue"></param>
        public void Update(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;

            if ((asyncParameterValue.ActionType.Split('%'))[2] == "Static")
            {
                // 静的SQL
                filename = "AsyncProcessingServiceUpdate.sql";
            }
            else if ((asyncParameterValue.ActionType.Split('%'))[2] == "dynamic")
            {
                // 動的SQL
                filename = "AsyncProcessingServiceUpdate.xml";
            }

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2(filename);

            // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            this.SetParameter("P2", asyncParameterValue.UserId);
            this.SetParameter("P3", asyncParameterValue.RegistrationDateTime);
            this.SetParameter("P4", asyncParameterValue.ExecutionStartDateTime);
            this.SetParameter("P5", asyncParameterValue.NumberOfRetries);
            this.SetParameter("P6", asyncParameterValue.CompletionDateTime);
            this.SetParameter("P7", asyncParameterValue.ProgressRate);
            this.SetParameter("P8", asyncParameterValue.Status);
            this.SetParameter("P9", asyncParameterValue.Command);

            object obj;

            //   -- 更新（件数を確認できる）
            obj = this.ExecInsUpDel_NonQuery();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            asyncReturnValue.Obj = obj;
        }
        #endregion
    }

}

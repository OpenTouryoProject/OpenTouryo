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
//* クラス名            :LayerD.cs
//* クラス名クラス名     :
//*
//* 作成者              :Supragyan
//* クラス日本語名       :
//* 更新履歴
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  11/28/2014   Supragyan      Created LayerD class for AsyncProcessing Service
//*  11/28/2014   Supragyan      Created Insert,Update,Select method for AsyncProcessing Service
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

namespace AsyncProcessingService
{
    #region LayerD
    /// <summary>
    /// LayerD class for AsyncProcessing Service
    /// </summary>
    public class LayerD : MyBaseDao
    {
        public LayerD(BaseDam dam) : base(dam) { }

        #region Insert

        /// <summary>
        /// Inserts async parameter values to database
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        /// <param name="asyncReturnValue"></param>
        public void Insert(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;
            // 静的SQL
            filename = "AsyncProcessingServiceInsert.sql";

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
            this.SetParameter("P9", asyncParameterValue.StatusId);
            this.SetParameter("P10", asyncParameterValue.ProgressRate);
            this.SetParameter("P11", asyncParameterValue.CommandId);
            this.SetParameter("P12", asyncParameterValue.ReservedArea);

            object obj;

            //   -- 追加（件数を確認できる）
            obj = this.ExecInsUpDel_NonQuery();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            asyncReturnValue.Obj = obj;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates async parameter values to database
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        /// <param name="asyncReturnValue"></param>
        public void Update(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;

            // 静的SQL
            filename = "AsyncProcessingServiceUpdate.sql";

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2(filename);

            // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            this.SetParameter("P2", asyncParameterValue.UserId);
            this.SetParameter("P3", asyncParameterValue.RegistrationDateTime);
            this.SetParameter("P4", asyncParameterValue.ExecutionStartDateTime);
            this.SetParameter("P5", asyncParameterValue.NumberOfRetries);
            this.SetParameter("P6", asyncParameterValue.CompletionDateTime);
            this.SetParameter("P7", asyncParameterValue.ProgressRate);
            this.SetParameter("P8", asyncParameterValue.StatusId);
            this.SetParameter("P9", asyncParameterValue.CommandId);

            object obj;

            //   -- 更新（件数を確認できる）
            obj = this.ExecInsUpDel_NonQuery();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            asyncReturnValue.Obj = obj;
        }

        #endregion

        #region UpdateByStatusId

        /// <summary>
        /// Updates async parameter values to database
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        /// <param name="asyncReturnValue"></param>
        public void UpdateByStatusId(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;

            // 静的SQL
            filename = "AsyncProcessingServiceStatusUpdate.sql";

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2(filename);

            // パラメタ ライズド クエリのパラメタに対して、動的に値を設定する。
            this.SetParameter("P1", asyncParameterValue.UserId);

            object obj;

            //   -- 更新（件数を確認できる）
            obj = this.ExecInsUpDel_NonQuery();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            asyncReturnValue.Obj = obj;
        }

        #endregion

        #region Select

        /// <summary>
        /// Selects async parameter values from database
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        /// <param name="asyncReturnValue"></param>
        public void Select(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;

            // 静的SQL
            filename = "AsyncProcessingServiceSelect.sql";

            //   -- ファイルから読み込む場合。
            this.SetSqlByFile2(filename);

            object obj;

            //   -- 更新（件数を確認できる）
            obj = this.ExecSelectScalar();

            // ↑DBアクセス-----------------------------------------------------

            // 戻り値を設定
            asyncReturnValue.Obj = obj;
        }

        #endregion
    }

    #endregion
}

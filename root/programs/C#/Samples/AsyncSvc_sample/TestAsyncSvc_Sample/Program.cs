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
//* クラス名            :Program.cs
//* クラス名クラス名     :
//*
//* 作成者              :Supragyan
//* クラス日本語名       :
//* 更新履歴
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  11/28/2014   Supragyan      For Inserts data to database 
//*  17/08/2015   Sandeep        Modified insert method name from 'Start' to 'InsertTask'.
//*                              Modified object of LayerB that is related to Business project,
//*                              instead of AsyncSvc_sample project. 
//**********************************************************************************
//system
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//部品
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Db;

//業務フレームワーク
using Touryo.Infrastructure.Business.Util;

//AsyncSvc_Sample
using AsyncSvc_sample;

//AsyncProcessingService
using AsyncProcessingService;

namespace TestAsyncSvc_Sample
{
    /// <summary>
    /// Program class for test user code
    /// </summary>
    public class Program
    {
        /// <summary>This is the main entry point for the application.</summary>
        static void Main(string[] args)
        {
            Program program = new Program();
            program.InsertData();
        }

        /// <summary>
        /// Inserts asynchronous task information to the database
        /// </summary>
        /// <returns></returns>
        public AsyncProcessingServiceParameterValue InsertData()
        {
            // Create array data to serilize.
            byte[] arrayData = { 1, 2, 3, 4, 5 };

            // Sets parameters of AsyncProcessingServiceParameterValue to insert asynchronous task information.
            AsyncProcessingServiceParameterValue asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "InsertTask", "InsertTask", "SQL",
                                                                            new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));
            asyncParameterValue.UserId = "A";
            asyncParameterValue.ProcessName = "AAA";
            asyncParameterValue.Data = AsyncSvc_sample.LayerB.SerializeToBase64String(arrayData);
            asyncParameterValue.ExecutionStartDateTime = DateTime.Now;
            asyncParameterValue.RegistrationDateTime = DateTime.Now;
            asyncParameterValue.NumberOfRetries = 0;
            asyncParameterValue.ProgressRate = 0;
            asyncParameterValue.CompletionDateTime = DateTime.Now;
            asyncParameterValue.StatusId = (int)(AsyncProcessingServiceParameterValue.AsyncStatus.Register);
            asyncParameterValue.CommandId = 0;
            asyncParameterValue.ReservedArea = "xxxxxx";

            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;
            AsyncProcessingServiceReturnValue asyncReturnValue;

            // Execute do business logic method.
            AsyncProcessingService.LayerB layerB = new AsyncProcessingService.LayerB();
            asyncReturnValue = (AsyncProcessingServiceReturnValue)layerB.DoBusinessLogic((AsyncProcessingServiceParameterValue)asyncParameterValue, iso);
            return asyncParameterValue;
        }
    }
}
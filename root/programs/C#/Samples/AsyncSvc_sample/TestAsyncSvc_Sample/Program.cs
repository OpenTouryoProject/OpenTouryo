//**********************************************************************************
//* 非同期処理サービス・サンプル アプリ
//**********************************************************************************

// テスト用サンプルなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：Program
//* クラス日本語名  ：Program
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  11/28/2014  Supragyan         For Inserts data to database.
//*  17/08/2015  Sandeep           Modified insert method name from 'Start' to 'InsertTask'.
//*                                Modified object of LayerB that is related to Business project,
//*                                instead of AsyncSvc_sample project. 
//**********************************************************************************

using System;

using Touryo.Infrastructure.Business.AsyncProcessingService;
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Public.Db;

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
            LayerB layerB = new LayerB();
            asyncReturnValue = (AsyncProcessingServiceReturnValue)layerB.DoBusinessLogic((AsyncProcessingServiceParameterValue)asyncParameterValue, iso);
            return asyncParameterValue;
        }
    }
}
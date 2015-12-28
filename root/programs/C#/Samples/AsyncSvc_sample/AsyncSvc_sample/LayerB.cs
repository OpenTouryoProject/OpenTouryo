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
//* クラス名            :LayerB.cs
//* クラス名クラス名     :
//*
//* 作成者              :Supragyan
//* クラス日本語名       :
//* 更新履歴
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  11/28/2014   Supragyan      LayerB class for AsyncProcessing Service.
//*  17/08/2015   Sandeep        Implemented serialization and deserialization methods.
//*                              Modified the code to start and update asynchronous task.
//*                              Implemented code to get command value and update progress rate.
//*                              Implemented code to declare and initialize the member variable.
//*                              Implemented code to handle abnormal termination, while updating the asynchronous process.
//*                              Implemented code to resume asynchronous process in the middle of the processing.
//*  21/08/2015   Sandeep        Modified code to call layerD of AsynProcessingService instead of do business logic.
//*  28/08/2015   Sandeep        Resolved transaction timeout issue by using DamKeyForABT and DamKeyForAMT properties.
//**********************************************************************************

// System
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

//業務フレームワーク
using Touryo.Infrastructure.Business.Business;
using Touryo.Infrastructure.Business.Util;

// Framework
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Util;

//部品
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Db;

//AsyncProcessingService
using AsyncProcessingService;

namespace AsyncSvc_sample
{
    /// <summary>
    /// LayerB class for AsyncProcessing service sample
    /// </summary>
    public class LayerB : MyApsBaseLogic
    {
        #region Member declartion

        // Number of seconds
        private int NumberOfSeconds;

        #endregion

        #region Member initialization

        /// <summary>Constructor</summary>
        public LayerB()
        {
            // Number of seconds to sleep the thread.
            string numberOfSeconds = GetConfigParameter.GetConfigValue("FxSleepUserProcess");
            if (!string.IsNullOrEmpty(numberOfSeconds))
            {
                this.NumberOfSeconds = int.Parse(numberOfSeconds);
            }
            else
            {
                this.NumberOfSeconds = 5;
            }
        }

        #endregion

        #region Member methods

        /// <summary>
        ///  Converts base64 string to deserialized byte array.
        /// </summary>
        /// <param name="base64String">Base64 String</param>
        /// <returns>byte array</returns>
        private byte[] DeserializeFromBase64String(string base64String)
        {
            byte[] deserializeData = null;
            if (string.IsNullOrEmpty(base64String))
            {
                deserializeData = CustomEncode.FromBase64String(base64String);
            }
            return deserializeData;
        }

        /// <summary>
        ///  Converts byte array to serialized base64 string
        /// </summary>
        /// <param name="arrayData">byte array</param>
        /// <returns>base64 string</returns>
        public static string SerializeToBase64String(byte[] arrayData)
        {
            string base64String = string.Empty;
            if (arrayData != null)
            {
                CustomEncode.ToBase64String(arrayData);
            }
            return base64String;
        }

        /// <summary>
        ///  Get command information from database. 
        /// </summary>
        /// <param name="taskID">asynchronous task id</param>
        /// <param name="userReturnValue">asynchronous return value</param>
        private void GetCommandValue(int taskID, AsyncProcessingServiceReturnValue userReturnValue)
        {
            // Sets parameters of AsyncProcessingServiceParameterValue to get command value.
            AsyncProcessingServiceParameterValue asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "SelectCommand", "SelectCommand", "SQL",
                                                                             new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));
            asyncParameterValue.TaskId = taskID;

            // Calls data access part of asynchronous processing service.
            LayerD myDao = new LayerD(this.GetDam(this.DamKeyforAMT));
            myDao.SelectCommand(asyncParameterValue, userReturnValue);
            userReturnValue.CommandId = (int) userReturnValue.Obj;
        }

        /// <summary>
        ///  Resumes asynchronous process in the middle of the processing.
        /// </summary>
        /// <param name="taskID">Task ID</param>
        /// <param name="userReturnValue">asynchronous return value</param>
        private void ResumeProcessing(int taskID, AsyncProcessingServiceReturnValue userReturnValue)
        {
            // Sets parameters of AsyncProcessingServiceParameterValue to resume asynchronous process in the middle of the processing.
            AsyncProcessingServiceParameterValue asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "UpdateTaskCommand", "UpdateTaskCommand", "SQL",
                                                                         new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));
            asyncParameterValue.TaskId = taskID;
            asyncParameterValue.CommandId = 0;

            // Calls data access part of asynchronous processing service.
            LayerD myDao = new LayerD(this.GetDam(this.DamKeyforAMT));
            myDao.UpdateTaskCommand(asyncParameterValue, userReturnValue);
        }

        /// <summary>
        ///  Updates the progress rate in the database. 
        /// </summary>
        /// <param name="taskID">asynchronous task id</param>
        /// <param name="progressRate">progress rate</param>
        private void UpdateProgressRate(int taskID, AsyncProcessingServiceReturnValue userReturnValue, decimal progressRate)
        {
            // Sets parameters of AsyncProcessingServiceParameterValue to Update progress rate
            AsyncProcessingServiceParameterValue asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "UpdateTaskProgress", "UpdateTaskProgress", "SQL",
                                                                             new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));
            asyncParameterValue.TaskId = taskID;
            asyncParameterValue.ProgressRate = progressRate;

            // Calls data access part of asynchronous processing service.
            LayerD myDao = new LayerD(this.GetDam(this.DamKeyforAMT));
            myDao.UpdateTaskProgress(asyncParameterValue, userReturnValue);
        }       

        /// <summary>
        /// Initiate the processing of asynchronous task.
        /// </summary>
        /// <param name="userParameterValue">asynchronous parameter values</param>
        public void UOC_Start(AsyncProcessingServiceParameterValue userParameterValue)
        {
            // Generates a return value class.
            AsyncProcessingServiceReturnValue userReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = userReturnValue;

            // Get array data from serialized base64 string.
            byte[] arrayData = this.DeserializeFromBase64String(userParameterValue.Data);

            // Get command information from database to check for retry.
            this.GetCommandValue(userParameterValue.TaskId, userReturnValue);
            
            if (userReturnValue.CommandId == (int)AsyncProcessingServiceParameterValue.AsyncCommand.Stop)
            {
                // Retry task: to resume asynchronous process in the middle of the processing.
                this.ResumeProcessing(userParameterValue.TaskId, userReturnValue);
            }
            else
            {
                // Otherwise, implement code to initiating a new task. 
                //...
            }

            // Updates the progress rate and handles abnormal termination of the process.
            this.Update(userParameterValue.TaskId, userReturnValue);
        }

        /// <summary>
        ///  Updates the progress rate and handles abnormal termination of the process.
        /// </summary>
        /// <param name="taskID">Task ID</param>
        /// <param name="userReturnValue">user parameter value</param>
        private void Update(int taskID, AsyncProcessingServiceReturnValue userReturnValue)
        {
            // Place the following statements in the loop, till the completion of task.
            // AsyncProcess: Loop-Start

            // Get command information from database to check for retry.
            this.GetCommandValue(taskID, userReturnValue);

            switch (userReturnValue.CommandId)
            {
                case (int)AsyncProcessingServiceParameterValue.AsyncCommand.Stop:
                    // If you want to retry, then throw the following exception.
                    throw new BusinessApplicationException("APSStopCommand", GetMessage.GetMessageDescription("CTE0003"), "");
                case (int)AsyncProcessingServiceParameterValue.AsyncCommand.Abort:
                    // Implement code to forcefully Abort the task.
                    //...

                    // If the task is abnormal terminated, then throw the exception .
                    throw new BusinessSystemException("APSAbortCommand", GetMessage.GetMessageDescription("CTE0004"));
                default:
                    // Update the progress rate in database.
                    this.UpdateProgressRate(taskID, userReturnValue, 50);

                    // Sleeps the thread, to minimize the CPU utilization.
                    System.Threading.Thread.Sleep(this.NumberOfSeconds * 1000);
                    break;
            }
            //AsyncProcess: Loop-End

            // If loop ends with no error, that indicates the task is completed sucessfully.
            return;
        }        

        #endregion
    }       
}
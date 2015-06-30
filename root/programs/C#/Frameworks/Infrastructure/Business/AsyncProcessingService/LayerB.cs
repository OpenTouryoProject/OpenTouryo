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
//*  11/28/2014   Supragyan      Created LayerB class for AsyncProcessing Service
//*  11/28/2014   Supragyan      Created Insert,Update,Select method for AsyncProcessing Service
//*  04/15/2015   Sandeep        Did code modification of insert, update and select for AsyncProcessing Service
//*  06/09/2015   Sandeep        Implemented code to update stop command to all the running asynchronous task
//*                              Modified code to reset Exception information, before starting asynchronous task 
//*  06/26/2015   Sandeep        Implemented code to get commandID in the SelectTask method,
//*                              to resolve unstable "Register" state, when you invoke [Abort] to AsyncTask, at this "Register" state
//**********************************************************************************

// System
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//業務フレームワーク
using Touryo.Infrastructure.Business.Business;

namespace AsyncProcessingService
{
    #region LayerB

    /// <summary>
    /// LayerB class for AsyncProcessing Service
    /// </summary>
    public class LayerB : MyFcBaseLogic
    {
        #region Insert

        /// <summary>
        /// Inserts Async Parameter values to Database through LayerD 
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        public void UOC_InsertTask(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            // 戻り値クラスを生成して、事前に戻り地に設定しておく。
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.InsertTask(asyncParameterValue, asyncReturnValue);            
        }

        #endregion

        #region Update

        #region UpdateTaskStart 

        /// <summary>
        ///  Updates information in the database that the asynchronous task is started
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        private void UOC_UpdateTaskStart(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.UpdateTaskStart(asyncParameterValue, asyncReturnValue);
        }

        #endregion

        #region UpdateTaskRetry

        /// <summary>
        ///  Updates information in the database that the asynchronous task is failed and can be retried later
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        private void UOC_UpdateTaskRetry(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.UpdateTaskRetry(asyncParameterValue, asyncReturnValue);
        }

        #endregion

        #region UpdateTaskFail

        /// <summary>
        ///  Updates information in the database that the asynchronous task is failed and abort this task [status=Abort] 
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        private void UOC_UpdateTaskFail(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.UpdateTaskFail(asyncParameterValue, asyncReturnValue);
        }

        #endregion

        #region UpdateTaskSuccess

        /// <summary>
        ///  Updates information in the database that the asynchronous task is completed
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        private void UOC_UpdateTaskSuccess(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.UpdateTaskSuccess(asyncParameterValue, asyncReturnValue);
        }

        #endregion

        #region UpdateTaskProgress

        /// <summary>
        ///  Updates progress rate of the asynchronous task in the database.
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        private void UOC_UpdateTaskProgress(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.UpdateTaskProgress(asyncParameterValue, asyncReturnValue);
        }

        #endregion

        #region UpdateTaskCommand

        /// <summary>
        ///  Updates command value information of a selected asynchronous task
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        private void UOC_UpdateTaskCommand(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.UpdateTaskCommand(asyncParameterValue, asyncReturnValue);
        }

        #endregion

        #region StopAllTask

        /// <summary>
        ///  Set stop command for all running asynchronous task
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        private void UOC_StopAllTask(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.StopAllTask(asyncParameterValue, asyncReturnValue);
        }

        #endregion

        #endregion        

        #region Select

        #region SelectCommand

        /// <summary>
        /// Selects user command from Database through LayerD 
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        private void UOC_SelectCommand(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.SelectCommand(asyncParameterValue, asyncReturnValue);
        }

        #endregion

        #region SelectTask

        /// <summary>
        /// Selects Asynchronous task from LayerD 
        /// </summary>
        /// <param name="asyncParameterValue">Async Parameter Value</param>
        private void UOC_SelectTask(AsyncProcessingServiceParameterValue asyncParameterValue)
        {
            AsyncProcessingServiceReturnValue asyncReturnValue = new AsyncProcessingServiceReturnValue();
            this.ReturnValue = asyncReturnValue;

            LayerD myDao = new LayerD(this.GetDam());
            myDao.SelectTask(asyncParameterValue, asyncReturnValue);

            DataTable dt = (DataTable)asyncReturnValue.Obj;
            asyncReturnValue.Obj = null;

            if (dt != null)
            {
                if (dt.Rows.Count != 0)
                {
                    asyncReturnValue.TaskId = Convert.ToInt32(dt.Rows[0]["Id"]);
                    asyncReturnValue.UserId = dt.Rows[0]["UserId"].ToString();
                    asyncReturnValue.ProcessName = dt.Rows[0]["ProcessName"].ToString();
                    asyncReturnValue.Data = dt.Rows[0]["Data"].ToString();
                    asyncReturnValue.NumberOfRetries = Convert.ToInt32(dt.Rows[0]["NumberOfRetries"]);
                    asyncReturnValue.ReservedArea = dt.Rows[0]["ReservedArea"].ToString();
                    asyncReturnValue.CommandId = Convert.ToInt32(dt.Rows[0]["CommandId"]);
                }
            }
        }

        #endregion

        #endregion
    }

    #endregion
}


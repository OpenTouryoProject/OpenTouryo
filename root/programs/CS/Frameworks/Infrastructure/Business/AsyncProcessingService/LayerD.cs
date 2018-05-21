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
//* クラス名        ：LayerD
//* クラス日本語名  ：LayerD
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  11/28/2014  Supragyan         Created LayerD class for AsyncProcessing Service
//*  11/28/2014  Supragyan         Created Insert,Update,Select method for AsyncProcessing Service
//*  04/14/2015  Sandeep           Did code modification of update and select asynchronous task 
//*  04/14/2015  Sandeep           Did code implementation of SetSqlByFile3 to access the SQL from embedded resource
//*  05/28/2015  Sandeep           Did code implementation to update Exception information to the database
//*  06/09/2015  Sandeep           Implemented code to update stop command to all the running asynchronous task
//*                                Modified code to reset Exception information, before starting asynchronous task 
//*  06/26/2015  Sandeep           Removed the where condition command <> 'Abort' from the SelectTask asynchronous task,
//*                                to resolve unstable "Register" state, when you invoke [Abort] to AsyncTask, at this "Register" state
//**********************************************************************************

using System;
using System.Data;
using System.Text;

using Touryo.Infrastructure.Business.Dao;
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Business.AsyncProcessingService
{
    /// <summary>
    /// LayerD class for AsyncProcessing Service
    /// </summary>
    public class LayerD : MyBaseDao
    {
        /// <summary>AsyncProcessingService用B層</summary>
        /// <param name="dam">dam</param>
        public LayerD(BaseDam dam) : base(dam) { }

        #region SetSqlByFile3

        /// <summary>
        ///  Get SQL query from the embedded resource assembly
        /// </summary>
        public void SetSqlByFile3(string filename)
        {
            // SQLファイルのEncoding情報の取得
            string sqlEncoding = GetConfigParameter.GetConfigValue(PubLiteral.SQL_ENCODING);

            if (string.IsNullOrEmpty(sqlEncoding))
            {
                // デフォルト：UTF-8
                sqlEncoding = "utf-8";
            }

            string assemblyString = "Business";
            string assemblyNameSpace = "Touryo.Infrastructure.Business.AsyncProcessingService";
            string loadFileName = assemblyNameSpace + "." + filename;

            // Get SQL query from embedded resource file. 
            string commandText = EmbeddedResourceLoader.LoadAsString(assemblyString, loadFileName, Encoding.GetEncoding(sqlEncoding));

            // Set sql command as text
            this.SetSqlByCommand(commandText);
        }

        #endregion        

        #region Insert

        /// <summary>
        /// Inserts async parameter values to database
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        /// <param name="asyncReturnValue"></param>
        public void InsertTask(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;
            filename = "AsyncProcessingServiceInsert.sql";

            // Get SQL query from file.
            this.SetSqlByFile3(filename);

            // Set SQL parameter values
            this.SetParameter("P2", asyncParameterValue.UserId);
            this.SetParameter("P3", asyncParameterValue.ProcessName);
            this.SetParameter("P4", asyncParameterValue.Data);
            this.SetParameter("P5", asyncParameterValue.RegistrationDateTime);
            this.SetParameter("P6", DBNull.Value);
            this.SetParameter("P7", asyncParameterValue.NumberOfRetries);
            this.SetParameter("P8", DBNull.Value);
            this.SetParameter("P9", asyncParameterValue.StatusId);
            this.SetParameter("P10", asyncParameterValue.ProgressRate);
            this.SetParameter("P11", asyncParameterValue.CommandId);
            this.SetParameter("P12", asyncParameterValue.ReservedArea);

            // Execute SQL query
            asyncReturnValue.Obj = this.ExecInsUpDel_NonQuery();
        }

        #endregion

        #region Update

        #region UpdateTaskStart

        /// <summary>
        ///  Updates information in the database that the asynchronous task is started
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        /// <param name="asyncReturnValue">Asynchronous Return Values</param>
        public void UpdateTaskStart(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;
            filename = "UpdateTaskStart.sql";

            // Get SQL query from file.
            this.SetSqlByFile3(filename);

            // Set SQL parameter values
            this.SetParameter("P1", asyncParameterValue.TaskId);
            this.SetParameter("P2", asyncParameterValue.ExecutionStartDateTime);
            this.SetParameter("P3", asyncParameterValue.StatusId);
            this.SetParameter("P4", DBNull.Value);

            // Execute SQL query
            asyncReturnValue.Obj = this.ExecInsUpDel_NonQuery();
        }

        #endregion

        #region UpdateTaskRetry

        /// <summary>
        ///  Updates information in the database that the asynchronous task is failed and can be retried later
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        /// <param name="asyncReturnValue">Asynchronous Return Values</param>
        public void UpdateTaskRetry(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;
            filename = "UpdateTaskRetry.sql";

            // Get SQL query from file.
            this.SetSqlByFile3(filename);

            // Set SQL parameter values
            this.SetParameter("P1", asyncParameterValue.TaskId);
            this.SetParameter("P2", asyncParameterValue.NumberOfRetries);
            this.SetParameter("P3", asyncParameterValue.CompletionDateTime);
            this.SetParameter("P4", asyncParameterValue.StatusId);
            this.SetParameter("P5", asyncParameterValue.ExceptionInfo);

            // Execute SQL query
            asyncReturnValue.Obj = this.ExecInsUpDel_NonQuery();
        }

        #endregion

        #region UpdateTaskFail

        /// <summary>
        ///  Updates information in the database that the asynchronous task is failed and abort this task [status=Abort]
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        /// <param name="asyncReturnValue">Asynchronous Return Values</param>
        public void UpdateTaskFail(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;
            filename = "UpdateTaskFail.sql";

            // Get SQL query from file.
            this.SetSqlByFile3(filename);

            // Set SQL parameter values
            this.SetParameter("P1", asyncParameterValue.TaskId);
            this.SetParameter("P2", asyncParameterValue.CompletionDateTime);
            this.SetParameter("P3", asyncParameterValue.StatusId);
            this.SetParameter("P4", asyncParameterValue.ExceptionInfo);

            // Execute SQL query
            asyncReturnValue.Obj = this.ExecInsUpDel_NonQuery();
        }

        #endregion

        #region UpdateTaskSuccess

        /// <summary>
        ///  Updates information in the database that the asynchronous task is completed
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        /// <param name="asyncReturnValue">Asynchronous Return Values</param>
        public void UpdateTaskSuccess(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;
            filename = "UpdateTaskSuccess.sql";

            // Get SQL query from file.
            this.SetSqlByFile3(filename);

            // Set SQL parameter values
            this.SetParameter("P1", asyncParameterValue.TaskId);
            this.SetParameter("P2", asyncParameterValue.CompletionDateTime);
            this.SetParameter("P3", asyncParameterValue.ProgressRate);
            this.SetParameter("P4", asyncParameterValue.StatusId);

            // Execute SQL query
            asyncReturnValue.Obj = this.ExecInsUpDel_NonQuery();
        }

        #endregion

        #region UpdateTaskProgress

        /// <summary>
        ///  Updates progress rate of the asynchronous task in the database.
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        /// <param name="asyncReturnValue">Asynchronous Return Values</param>
        public void UpdateTaskProgress(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;
            filename = "UpdateTaskProgress.sql";

            // Get SQL query from file.
            this.SetSqlByFile3(filename);

            // Set SQL parameter values
            this.SetParameter("P1", asyncParameterValue.TaskId);
            this.SetParameter("P2", asyncParameterValue.ProgressRate);

            // Execute SQL query
            asyncReturnValue.Obj = this.ExecInsUpDel_NonQuery();
        }

        #endregion

        #region UpdateTaskCommand

        /// <summary>
        ///  Updates command value information of a selected asynchronous task
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        /// <param name="asyncReturnValue">Asynchronous Return Values</param>
        public void UpdateTaskCommand(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;
            filename = "UpdateTaskCommand.sql";

            // Get SQL query from file.
            this.SetSqlByFile3(filename);

            // Set SQL parameter values
            this.SetParameter("P1", asyncParameterValue.TaskId);
            this.SetParameter("P2", asyncParameterValue.CommandId);

            // Execute SQL query
            asyncReturnValue.Obj = this.ExecInsUpDel_NonQuery();
        }

        #endregion

        #region StopAllTask

        /// <summary>
        ///  Set stop command for all running asynchronous task.
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        /// <param name="asyncReturnValue">Asynchronous Return Values</param>
        public void StopAllTask(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;
            filename = "StopAllTask.sql";

            // Get SQL query from file.
            this.SetSqlByFile3(filename);

            // Set SQL parameter values
            this.SetParameter("P1", asyncParameterValue.StatusId);
            this.SetParameter("P2", asyncParameterValue.CommandId);

            // Execute SQL query
            asyncReturnValue.Obj = this.ExecInsUpDel_NonQuery();
        }

        #endregion

        #endregion

        #region Select

        #region SelectCommand

        /// <summary>
        ///  Selects user command from database
        /// </summary>
        /// <param name="asyncParameterValue">Asynchronous Parameter Values</param>
        /// <param name="asyncReturnValue">Asynchronous Return Values</param>
        public void SelectCommand(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;
            filename = "SelectCommand.sql";

            // Get SQL query from file.
            this.SetSqlByFile3(filename);

            // Set SQL parameter values
            this.SetParameter("P1", asyncParameterValue.TaskId);

            // Execute SQL query
            asyncReturnValue.Obj = this.ExecSelectScalar();
        }

        #endregion

        #region SelectTask

        /// <summary>
        ///  To get Asynchronous Task from the database
        /// </summary>
        /// <param name="asyncParameterValue"></param>
        /// <param name="asyncReturnValue"></param>
        public void SelectTask(AsyncProcessingServiceParameterValue asyncParameterValue, AsyncProcessingServiceReturnValue asyncReturnValue)
        {
            string filename = string.Empty;
            filename = "SelectTask.sql";

            // Get SQL query from file.
            this.SetSqlByFile3(filename);

            // Set SQL parameter values
            this.SetParameter("P1", asyncParameterValue.RegistrationDateTime);
            this.SetParameter("P2", asyncParameterValue.NumberOfRetries);
            this.SetParameter("P3", (int)AsyncProcessingServiceParameterValue.AsyncStatus.Register);
            this.SetParameter("P4", (int)AsyncProcessingServiceParameterValue.AsyncStatus.AbnormalEnd);
            this.SetParameter("P7", asyncParameterValue.CompletionDateTime);

            DataTable dt = new DataTable();

            // Get Asynchronous Task from the database
            this.ExecSelectFill_DT(dt);
            asyncReturnValue.Obj = dt;
        }

        #endregion

        #endregion        
    }
}

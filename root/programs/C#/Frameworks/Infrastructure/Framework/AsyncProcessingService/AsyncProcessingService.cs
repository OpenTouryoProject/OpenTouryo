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
//* クラス名            :AsyncProcessingService.cs
//* クラス名クラス名     :
//*
//* 作成者              :Supragyan
//* クラス日本語名       :
//* 更新履歴
//*  Date:        Author:        Comments:
//*  ----------  ----------------  -------------------------------------------------
//*  11/28/2014   Supragyan      Asynchronous Processing Service class for windows service
//**********************************************************************************
// System
using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;

//業務フレームワーク
using Touryo.Infrastructure.Business.Util;

//部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Framework.AsyncProcessingService
{

    /// <summary>
    /// Asynchronous Processing Service class for windows service
    /// </summary>
    public class AsyncProcessingService : System.ServiceProcess.ServiceBase
    {
        int _threadCount = 0;
        AsyncProcessingServiceParameterValue _asyncParameterValue;

        /// <summary>
        /// Async Processing Service constructor to set property.
        /// </summary>
        public AsyncProcessingService()
        {
            //Sets CanPauseAndContinue property to true for enable Pause and Continue properties in service.
            this.CanPauseAndContinue = true;

            //Sets Stop property to true for enable stop property in service.
            this.CanStop = true;

            //Sets AutoLog property to true for enable all log files in service
            this.AutoLog = true;
        }

        /// <summary>
        /// Main method to run service.
        /// </summary>
        public static void Main()
        {
            System.ServiceProcess.ServiceBase[] ServicesToRun;
            ServicesToRun =
              new System.ServiceProcess.ServiceBase[] { new AsyncProcessingService() };
            System.ServiceProcess.ServiceBase.Run(ServicesToRun);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ServiceName = "AsyncProcessingService";
        }

        /// <summary>
        /// Sets parameters of AsyncProcessingServiceParameterValue for Insert and Update based on status.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public AsyncProcessingServiceParameterValue InsertOrUpdateServiceData(object checkStatus, string userId, string processName, byte[] arr, DateTime executionStartDateTime,
                                                                              DateTime registrationDateTime, int numberOfRetries, int progressRate, DateTime completionDateTime,
                                                                              string status, string command, string reservedArea)
        {
            if (checkStatus == "Start")
            {
                _asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "button", "Start", "SQL" + "%" + "individual" +
                                                                               "%" + "Static" + "%" + "-", new MyUserInfo("aaa", "192.168.1.1"));
                _asyncParameterValue.UserId = userId;
                _asyncParameterValue.ProcessName = processName;
                _asyncParameterValue.Data = CustomEncode.ToBase64String(arr);
                _asyncParameterValue.ExecutionStartDateTime = executionStartDateTime;
                _asyncParameterValue.RegistrationDateTime = registrationDateTime;
                _asyncParameterValue.NumberOfRetries = numberOfRetries;
                _asyncParameterValue.ProgressRate = progressRate;
                _asyncParameterValue.CompletionDateTime = completionDateTime;
                _asyncParameterValue.Status = status;
                _asyncParameterValue.Command = command;
                //asyncparameterValue.Command = DBNull.Value.ToString();
                _asyncParameterValue.ReservedArea = reservedArea;
            }
            else
            {
                _asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "button", "Update", "SQL" + "%" + "individual" +
                                                                               "%" + "Static" + "%" + "-", new MyUserInfo("aaa", "192.168.1.1"));
                _asyncParameterValue.UserId = userId;
                _asyncParameterValue.ExecutionStartDateTime = executionStartDateTime;
                _asyncParameterValue.RegistrationDateTime = registrationDateTime;
                _asyncParameterValue.Status = checkStatus.ToString();
                //Based on the service status setting database command value
                switch (checkStatus.ToString())
                {
                    case "Processing":
                        _asyncParameterValue.ProgressRate = 50;
                        _asyncParameterValue.Command = "Stop";
                        break;
                    case "Stop":
                        _asyncParameterValue.ProgressRate = 100;
                        _asyncParameterValue.Command = "null";
                        break;
                    case "Abnormal End":
                        _asyncParameterValue.ProgressRate = 0;
                        _asyncParameterValue.Command = "null";
                        break;
                    case "Abort":
                        _asyncParameterValue.ProgressRate = 0;
                        _asyncParameterValue.Command = "null";
                        break;
                }
                _asyncParameterValue.NumberOfRetries = numberOfRetries;
                _asyncParameterValue.CompletionDateTime = completionDateTime;
            }
            return _asyncParameterValue;
        }

        /// <summary>
        /// Callback method for Inserting and Updating Data to database using BLayer
        /// </summary>
        private void InsertUpdateUsingLayerB(object asyncParameterValue)
        {
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;
            AsyncProcessingServiceReturnValue asyncReturnValue;

            LayerB layerB = new LayerB();
            asyncReturnValue = (AsyncProcessingServiceReturnValue)layerB.DoBusinessLogic((AsyncProcessingServiceParameterValue)asyncParameterValue, iso);
        }

        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        protected override void OnStart(string[] args)
        {
            StartByThreadPool("Start");
        }

        /// <summary>
        /// Calls callback method using Threadpool worker item.
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public bool StartByThreadPool(object check)
        {
            int maxThreadCount;
            byte[] arr = { 1, 2, 3, 4, 5 };

            lock (this)
            {
                maxThreadCount
                    = int.Parse(ConfigurationManager.AppSettings.Get("FxMaxThreadCount").ToString());

                //Checks if threadcount greater than maxthreadcount sets in config file 
                //then service status will be inserted or updated as Abort
                if (_threadCount >= maxThreadCount)
                {
                    InsertOrUpdateServiceData("Abort", "A", "AAA", arr, DateTime.Now, DateTime.Now, _threadCount, 0, DateTime.Now, null, null, "xxxxxx");
                    return false;
                }

                _threadCount++;

                //Based on the service status sets parameter values for Insert and Update.
                if (check == "Start")
                {
                    _asyncParameterValue = InsertOrUpdateServiceData(check, "A", "AAA", arr, DateTime.Now, DateTime.Now, 0, 0, DateTime.Now, "Register", "null", "xxxxxx");
                }
                else
                {
                    _asyncParameterValue = InsertOrUpdateServiceData(check, "A", "AAA", arr, DateTime.Now, DateTime.Now, _threadCount, 0, DateTime.Now, string.Empty, string.Empty, "xxxxxx");
                }
                //Calls callback method based on Threadpool availability for calling Business layer.
                ThreadPool.QueueUserWorkItem(new WaitCallback(InsertUpdateUsingLayerB), (object)_asyncParameterValue);
            }
            return true;
        }

        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop()
        {
            StartByThreadPool("Stop");
        }

        /// <summary>
        /// Pause this service.
        /// </summary>
        protected override void OnPause()
        {
            StartByThreadPool("Abort");
        }

        /// <summary>
        /// Shutdown this service.
        /// </summary>
        protected override void OnShutdown()
        {
            StartByThreadPool("Abnormal End");
        }
    }

}


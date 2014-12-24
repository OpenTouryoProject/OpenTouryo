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

//Framework
using Touryo.Infrastructure.Framework.Transmission;

//AsyncSvc_sample
using AsyncSvc_sample;

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
        /// Sets parameters of AsyncProcessingServiceParameterValue Update based on status.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>        
        public AsyncProcessingServiceParameterValue UpdateServiceData(AsyncProcessingServiceParameterValue.AsyncStatus checkStatus, string userId, DateTime executionStartDateTime,
                                                                      DateTime registrationDateTime, int numberOfRetries, int progressRate, DateTime completionDateTime,
                                                                      int statusId, int commandId)
        {
            _asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "Update", "Update", "SQL",
                                                                             new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));
            _asyncParameterValue.UserId = userId;
            _asyncParameterValue.ExecutionStartDateTime = executionStartDateTime;
            _asyncParameterValue.RegistrationDateTime = registrationDateTime;

            // Based on the service status setting database command value
            switch (checkStatus)
            {
                case AsyncProcessingServiceParameterValue.AsyncStatus.Processing:
                    _asyncParameterValue.ProgressRate = 50;
                    _asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.Processing;
                    _asyncParameterValue.CommandId = (int)AsyncProcessingServiceParameterValue.AsyncCommand.Stop;
                    break;
                case AsyncProcessingServiceParameterValue.AsyncStatus.Stop:
                    _asyncParameterValue.ProgressRate = 100;
                    _asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.Stop;
                    _asyncParameterValue.CommandId = 0;
                    break;
                case AsyncProcessingServiceParameterValue.AsyncStatus.AbnormalEnd:
                    _asyncParameterValue.ProgressRate = 0;
                    _asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.AbnormalEnd;
                    _asyncParameterValue.CommandId = 0;
                    break;
                case AsyncProcessingServiceParameterValue.AsyncStatus.Abort:
                    _asyncParameterValue.ProgressRate = 0;
                    _asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.Abort;
                    _asyncParameterValue.CommandId = 0;
                    break;
            }
            _asyncParameterValue.NumberOfRetries = numberOfRetries;
            _asyncParameterValue.CompletionDateTime = completionDateTime;

            return _asyncParameterValue;
        }

        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        protected override void OnStart(string[] args)
        {
            StartByThreadPool(AsyncProcessingServiceParameterValue.AsyncStatus.Processing);
        }

        /// <summary>
        /// Calls callback method using Threadpool worker item.
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public bool StartByThreadPool(AsyncProcessingServiceParameterValue.AsyncStatus check)
        {
            int maxThreadCount;

            lock (this)
            {
                maxThreadCount
                    = int.Parse(ConfigurationManager.AppSettings.Get("FxMaxThreadCount").ToString());

                //Checks if threadcount greater than maxthreadcount sets in config file 
                //then service status will be inserted or updated as Abort
                if (_threadCount >= maxThreadCount)
                {
                    UpdateServiceData(AsyncProcessingServiceParameterValue.AsyncStatus.Abort, "A", DateTime.Now, DateTime.Now, _threadCount, 0, DateTime.Now, 0, 0);
                    return false;
                }

                _threadCount++;

                _asyncParameterValue = UpdateServiceData(check, "A", DateTime.Now, DateTime.Now, _threadCount, 0, DateTime.Now, 0, 0);

                ThreadPool.QueueUserWorkItem(new WaitCallback(InvokeController), (object)_asyncParameterValue);
            }
            return true;
        }

        /// <summary>
        /// Updates using LayerB through callcontroller class
        /// </summary>
        /// <param name="_asyncParameterValue"></param>
        private void InvokeController(object _asyncParameterValue)
        {
            AsyncProcessingServiceReturnValue asyncReturnValue;

            // InProcess Call using CallController for updating service in database
            CallController callController = new CallController(this._asyncParameterValue.User);
            asyncReturnValue = (AsyncProcessingServiceReturnValue)callController.Invoke("UpdateInProcessCall", _asyncParameterValue);
        }

        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop()
        {
            StartByThreadPool(AsyncProcessingServiceParameterValue.AsyncStatus.Stop);
        }

        /// <summary>
        /// Pause this service.
        /// </summary>
        protected override void OnPause()
        {
            StartByThreadPool(AsyncProcessingServiceParameterValue.AsyncStatus.Abort);
        }

        /// <summary>
        /// Shutdown this service.
        /// </summary>
        protected override void OnShutdown()
        {
            StartByThreadPool(AsyncProcessingServiceParameterValue.AsyncStatus.AbnormalEnd);
        }
    }

}


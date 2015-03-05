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
//*  11/28/2014   Supragyan      Created Asynchronous Processing Service class for windows service
//*  02/20/2015   Supragyan      Created SelectProcessName method from service data.
//*  02/20/2015   Supragyan      Modified call controller class by implement process name column's information.
//*  02/24/2015   Supragyan      Created main thread inside OnStart method.
//*  02/24/2015   Supragyan      Created MainThreadInvoke method for implementing main thread and worker thread functionality.
//*  02/26/2015   Supragyan      Modified code in Invoke controller. 
//*  03/04/2015   Sai            Modifed the code as per the review comments given by Niahino-san on 03/3/2015.   
//**********************************************************************************

// System
using System;
using System.Data;
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
using Touryo.Infrastructure.Framework.Common;

//AsyncProcessingService
using AsyncProcessingService;

namespace Touryo.Infrastructure.Framework.AsyncProcessingService
{
    /// <summary>
    /// Asynchronous Processing Service class for windows service
    /// </summary>
    public class AsyncProcessingService : System.ServiceProcess.ServiceBase
    {
        private Thread _mainthread = null;
        int _threadCount = 0;
        AsyncProcessingServiceParameterValue _asyncParameterValue;
        AsyncProcessingServiceReturnValue _asyncReturnValue;

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

        #region OnStart

        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        protected override void OnStart(string[] args)
        {
            //Starts main thread invocation.
            _mainthread = new Thread(MainThreadInvoke);

            // Thread pool initialization with maxium threads. 
            ThreadPool.SetMaxThreads(10, 10);
            _mainthread.Start();

            // If no. of worker threads exceeds maximum threads then putting the 
            // current thread in waiting state.
            if (_threadCount > 10)
            {
                Thread.Sleep(30000);
            }
        }

        #endregion

        #region MainThreadInvoke

        /// <summary>
        /// calling main thread
        /// </summary>
        public void MainThreadInvoke()
        {
            // Infinte loop processing for selecting register task.
            while (true)
            {
                _asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "Select", "Select", "SQL",
                                                                                new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));

                AsyncProcessingService.LayerB myBusiness = new AsyncProcessingService.LayerB();
                AsyncProcessingServiceReturnValue asyncReturnValue = (AsyncProcessingServiceReturnValue)myBusiness.DoBusinessLogic(
                                                                     (BaseParameterValue)_asyncParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

                _asyncReturnValue = asyncReturnValue;

                _asyncParameterValue = (AsyncProcessingServiceParameterValue)asyncReturnValue.Obj;

                // Counting no. of worker threads.
                _threadCount++;
                ThreadPool.QueueUserWorkItem(new WaitCallback(InvokeController), (object)_asyncParameterValue.ProcessName);

                //Update in mainthread
                AsyncProcessingServiceParameterValue asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "Update", "Update", "SQL",
                                                                                new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));

                DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

                asyncReturnValue = (AsyncProcessingServiceReturnValue)myBusiness.DoBusinessLogic((AsyncProcessingServiceParameterValue)_asyncParameterValue, iso);
            }
        }

        #endregion

        #region InvokeController

        /// <summary>
        /// Updates using LayerB through callcontroller class
        /// </summary>
        /// <param name="_asyncParameterValue"></param>
        private void InvokeController(object processName)
        {
            lock (this)
            {
                int retries;
                AsyncProcessingServiceReturnValue asyncReturnValue = _asyncReturnValue;
                object deserializeDatas = DeserializeFromBase64(_asyncReturnValue.Data);
                string[] strDatas = deserializeDatas.ToString().Split('/');

                asyncReturnValue.UserId = strDatas.GetValue(0).ToString();
                asyncReturnValue.RegistrationDateTime = Convert.ToDateTime(strDatas.GetValue(2));
                asyncReturnValue.ExecutionStartDateTime = Convert.ToDateTime(strDatas.GetValue(3));
                asyncReturnValue.NumberOfRetries = Convert.ToInt32(strDatas.GetValue(4));
                asyncReturnValue.CompletionDateTime = Convert.ToDateTime(strDatas.GetValue(5));
                string[] strReservedArea = strDatas.GetValue(6).ToString().Split('-');
                asyncReturnValue.ReservedArea = strReservedArea.GetValue(0) + " " + strReservedArea.GetValue(1);

                // InProcess Call using CallController for updating service in database
                CallController callController = new CallController(this._asyncParameterValue.User);
                asyncReturnValue = (AsyncProcessingServiceReturnValue)callController.Invoke(_asyncParameterValue.ToString(), asyncReturnValue);

                int maxThreadCount
                    = int.Parse(ConfigurationManager.AppSettings.Get("FxMaxThreadCount").ToString());

                _asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "UpdateStatus", "UpdateStatus", "SQL",
                                                                                new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));
                if (Convert.ToInt32(asyncReturnValue.Obj) == 1)
                {
                    //Update in worker thread
                    this._asyncParameterValue.UserId = _asyncData.UserId;
                    this._asyncParameterValue.ProgressRate = 100;
                    this._asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.End;
                    this._asyncParameterValue.NumberOfRetries = 0;

                    AsyncProcessingService.LayerB myBusiness = new AsyncProcessingService.LayerB();

                    DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

                    asyncReturnValue = (AsyncProcessingServiceReturnValue)myBusiness.DoBusinessLogic((AsyncProcessingServiceParameterValue)_asyncParameterValue, iso);
                }
                else
                {
                    this._asyncParameterValue.UserId = _asyncData.UserId;
                    this._asyncParameterValue.ProgressRate = 0;
                    this._asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.AbnormalEnd;
                    this._asyncParameterValue.NumberOfRetries += 1;
                    retries++;
                    AsyncProcessingService.LayerB myBusiness = new AsyncProcessingService.LayerB();

                    DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

                    asyncReturnValue = (AsyncProcessingServiceReturnValue)myBusiness.DoBusinessLogic((AsyncProcessingServiceParameterValue)_asyncParameterValue, iso);

                }
                //Checks if threadcount greater than maxthreadcount sets in config file 
                //then service status will be inserted or updated as Abort
                if (retries >= maxThreadCount)
                {
                    this._asyncParameterValue.UserId = _asyncData.UserId;
                    this._asyncParameterValue.ProgressRate = 0;
                    this._asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.Abort;

                    AsyncProcessingService.LayerB myBusiness = new AsyncProcessingService.LayerB();

                    DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

                    asyncReturnValue = (AsyncProcessingServiceReturnValue)myBusiness.DoBusinessLogic((AsyncProcessingServiceParameterValue)_asyncParameterValue, iso);
                }
            }
        }

        #endregion

        #region DeserializeFromBase64

        /// <summary>
        /// Converts string data to byte array and byte array data to object.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public object DeserializeFromBase64(string str)
        {
            byte[] deserializeDatas = CustomEncode.FromBase64String(str);
            object obj = ByteArrayToObject(deserializeDatas);
            return obj;
        }

        #endregion

        #region ByteArrayToObject

        /// <summary>
        /// Converts byte array to object data
        /// </summary>
        /// <param name="deserializeDatas"></param>
        /// <returns></returns>
        private Object ByteArrayToObject(byte[] deserializeDatas)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(deserializeDatas, 0, deserializeDatas.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }

        #endregion

        #region OnStop

        /// <summary>
        /// Stop this service with main thread and worker thread.
        /// </summary>
        protected override void OnStop()
        {
            Thread.CurrentThread.Abort();
            _mainthread.Abort();
        }

        #endregion

        #region OnPause

        /// <summary>
        /// Pause this service with main thread and worker thread..
        /// </summary>
        protected override void OnPause()
        {
            Thread.CurrentThread.Abort();
            _mainthread.Abort();
        }

        #endregion

        #region OnShutdown

        /// <summary>
        /// Shutdown this service with main thread and worker thread..
        /// </summary>
        protected override void OnShutdown()
        {
            Thread.CurrentThread.Abort();
            _mainthread.Abort();
        }

        #endregion
    }

}


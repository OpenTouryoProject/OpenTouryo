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
//*  03/16/2015   Sai            Modifed the code as per the change request given by Niahino-san on 03/9/2015.   
//*  03/20/2015   Sai            Added lock mechanism while selecting task from database and change resquests.
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
        Boolean _isInfinite = true;
        int _maxThreadCount;

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

            _maxThreadCount = int.Parse(ConfigurationManager.AppSettings.Get("FxMaxThreadCount").ToString());
            // Thread pool initialization with maxium threads. 
            ThreadPool.SetMaxThreads(_maxThreadCount, _maxThreadCount);
            _mainthread.Start();
            _mainthread.Join();
        }

        #endregion

        #region MainThreadInvoke

        /// <summary>
        /// calling main thread
        /// </summary>
        public void MainThreadInvoke()
        {
            AsyncProcessingServiceReturnValue asyncReturnValue;
            AsyncProcess.LayerB myBusiness;

            // Infinte loop processing for selecting register task.
            while (_isInfinite)
            {
                _asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "Select", "Select", "SQL",
                                                                                   new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));
                Monitor.Enter(_asyncParameterValue);
                try
                {
                    myBusiness = new AsyncProcess.LayerB();
                    asyncReturnValue = (AsyncProcessingServiceReturnValue)myBusiness.DoBusinessLogic(
                                                                          (BaseParameterValue)_asyncParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

                    _asyncReturnValue = asyncReturnValue;

                    _asyncParameterValue = (AsyncProcessingServiceParameterValue)asyncReturnValue.Obj;
                }
                finally
                {
                    Monitor.Exit(_asyncParameterValue);
                }

                if (string.IsNullOrEmpty(_asyncParameterValue.UserId))
                {
                    Thread.Sleep(30000);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(InvokeController), (object)_asyncParameterValue.ProcessName);

                    //Update in mainthread
                    AsyncProcessingServiceParameterValue asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "Update", "Update", "SQL",
                                                                                    new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));

                    DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

                    asyncReturnValue = (AsyncProcessingServiceReturnValue)myBusiness.DoBusinessLogic((AsyncProcessingServiceParameterValue)_asyncParameterValue, iso);
                }

                // If no. of worker threads exceeds maximum threads then putting the 
                // current thread in waiting state.
                if (_threadCount > _maxThreadCount)
                {
                    Thread.Sleep(30000);
                }
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
            // Counting no. of worker threads.
            _threadCount++;
            // Reads max thrad count from config.            
            int retries = 0;
            AsyncProcessingServiceReturnValue asyncReturnValue = _asyncReturnValue;
            object deserializeDatas = DeserializeFromBase64(_asyncReturnValue.Data);
            string[] strDatas = deserializeDatas.ToString().Split('/');

            asyncReturnValue.UserId = strDatas.GetValue(0).ToString();
            asyncReturnValue.ExecutionStartDateTime = Convert.ToDateTime(strDatas.GetValue(3));
            asyncReturnValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.Processing;

            // InProcess Call using CallController for updating service in database
            CallController callController = new CallController(this._asyncParameterValue.User);
            asyncReturnValue = (AsyncProcessingServiceReturnValue)callController.Invoke(processName.ToString(), asyncReturnValue);



            AsyncProcessingServiceParameterValue asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "UpdateStatus", "UpdateStatus", "SQL",
                                                                            new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));
            if (Convert.ToInt32(asyncReturnValue.Obj) == 1)
            {
                //Update in worker thread
                asyncParameterValue.UserId = _asyncParameterValue.UserId;
                asyncParameterValue.ProgressRate = 100;
                asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.End;
                asyncParameterValue.NumberOfRetries = 0;

                AsyncProcess.LayerB myBusiness = new AsyncProcess.LayerB();

                DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

                asyncReturnValue = (AsyncProcessingServiceReturnValue)myBusiness.DoBusinessLogic((AsyncProcessingServiceParameterValue)asyncParameterValue, iso);
                _threadCount--;
            }
            else
            {
                // If business exception then updating status to AbnormalEnd.
                if (asyncReturnValue.ErrorFlag == true)
                {
                    asyncParameterValue.UserId = _asyncParameterValue.UserId;
                    asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.AbnormalEnd;
                    asyncParameterValue.NumberOfRetries += 1;
                    retries++;
                    AsyncProcess.LayerB myBusiness = new AsyncProcess.LayerB();

                    DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

                    asyncReturnValue = (AsyncProcessingServiceReturnValue)myBusiness.DoBusinessLogic((AsyncProcessingServiceParameterValue)asyncParameterValue, iso);
                    _threadCount--;
                }
                // Else other exception then updating status to Abort
                else
                {
                    asyncParameterValue.UserId = _asyncParameterValue.UserId;
                    asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.Abort;
                    asyncParameterValue.NumberOfRetries += 1;
                    retries++;
                    AsyncProcess.LayerB myBusiness = new AsyncProcess.LayerB();

                    DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

                    asyncReturnValue = (AsyncProcessingServiceReturnValue)myBusiness.DoBusinessLogic((AsyncProcessingServiceParameterValue)asyncParameterValue, iso);
                    _threadCount--;
                }
            }
            //Checks if threadcount greater than maxthreadcount sets in config file 
            //then service status will be inserted or updated as Abort
            if (retries >= _maxThreadCount)
            {
                asyncParameterValue.UserId = _asyncParameterValue.UserId;
                this._asyncParameterValue.ProgressRate = 0;
                this._asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.Abort;

                AsyncProcess.LayerB myBusiness = new AsyncProcess.LayerB();

                DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;

                asyncReturnValue = (AsyncProcessingServiceReturnValue)myBusiness.DoBusinessLogic((AsyncProcessingServiceParameterValue)asyncParameterValue, iso);
                _threadCount--;
                _isInfinite = false;
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
            Thread.CurrentThread.Join();
            _mainthread.Join();
            _isInfinite = false;
        }

        #endregion

        #region OnPause

        /// <summary>
        /// Pause this service with main thread and worker thread..
        /// </summary>
        protected override void OnPause()
        {
            Thread.CurrentThread.Join();
            _mainthread.Join();
            _threadCount--;
            _isInfinite = false;
        }

        #endregion

        #region OnShutdown

        /// <summary>
        /// Shutdown this service with main thread and worker thread..
        /// </summary>
        protected override void OnShutdown()
        {
            Thread.CurrentThread.Abort();
            _mainthread.Join();
            _threadCount--;
            _isInfinite = false;
        }

        #endregion
    }

}


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
//*  03/26/2015   Sai            Did Nihsini-san review comment changes as on 25-Mar-2015.
//*  04/13/2015   Sandeep        Did Nihsini-san review comment changes as on 04-Apr-2015.
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
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Util;

//Framework
using Touryo.Infrastructure.Framework.Transmission;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Util;

//AsyncProcessingService
using AsyncProcessingService;

namespace Touryo.Infrastructure.Framework.AsyncProcessingService
{
    /// <summary>Method names to update asynchronous task</summary>
    public class AsyncTaskUpdate
    {
        /// <summary>Update start</summary>
        public const string START = "UpdateTaskStart";

        /// <summary>Update retry</summary>
        public const string RETRY = "UpdateTaskRetry";

        /// <summary>Update fail</summary>
        public const string FAIL = "UpdateTaskFail";

        /// <summary>Update success</summary>
        public const string SUCCESS = "UpdateTaskSuccess";
    }

    /// <summary>Asynchronous Processing Service class for windows service</summary>
    public class AsyncProcessingService : System.ServiceProcess.ServiceBase
    {
        #region Member variables

        /// <summary>Main thread</summary>
        private Thread _mainThread;

        /// <summary>Infinite loop flag (thread-safe)</summary>
        private volatile bool _infiniteLoop = true;

        // <summary>Maximum thread count (thread-safe)</summary>
        private volatile int _maxThreadCount = 0;

        // <summary>Number of seconds the thread must wait (thread-safe)</summary>
        private volatile int _numberOfSeconds = 0;

        // <summary>Maximum number of retries (thread-safe)</summary>
        private volatile int _maxNumberOfRetries = 0;

        // <summary>Maximum number of hours (thread-safe)</summary>
        private volatile int _maxNumberOfHours = 0;

        #endregion

        #region Constructor

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

        #endregion

        #region Service Methods

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

        #endregion

        #region Service Events

        #region OnStart

        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        protected override void OnStart(string[] args)
        {
            // Set initial values
            this._infiniteLoop = true;

            // Set initial values from .config file
            this.SetFromConfig();

            // Thread pool initialization with maxium and minimum threads. 
            ThreadPool.SetMinThreads(1, 0);
            ThreadPool.SetMaxThreads(_maxThreadCount, 0);

            // Starts main thread invocation.
            _mainThread = new Thread(MainThreadInvoke);
            _mainThread.Start();
        }

        #endregion

        #region OnStop

        /// <summary>
        /// Stop this service with main thread and worker thread.
        /// </summary>
        protected override void OnStop()
        {
            // Stop the process of asynchronous service and Waits to complete all worker thread to complete.
            this.StopAsyncProcess();
            LogIF.ErrorLog("ASYNC-SERVICE", GetMessage.GetMessageDescription("E0007"));
        }

        #endregion

        #region OnPause

        /// <summary>
        /// Pause this service with main thread and worker thread.
        /// </summary>
        protected override void OnPause()
        {
            // Stop the process of asynchronous service and Waits to complete all worker thread to complete.
            this.StopAsyncProcess();
            LogIF.ErrorLog("ASYNC-SERVICE", GetMessage.GetMessageDescription("E0008"));
        }

        #endregion

        #region OnShutdown

        /// <summary>
        /// Shutdown this service with main thread and worker thread..
        /// </summary>
        protected override void OnShutdown()
        {
            // Stop the process of asynchronous service and Waits to complete all worker thread to complete.
            this.StopAsyncProcess();
            LogIF.ErrorLog("ASYNC-SERVICE", GetMessage.GetMessageDescription("E0009"));
        }

        #endregion

        #endregion

        #region Thread method

        #region MainThreadInvoke

        /// <summary>
        ///  Maintains the Main thread of the Asynchronous Service
        /// </summary>
        public void MainThreadInvoke()
        {
            // Asynchronous service is started
            LogIF.InfoLog("ASYNC-SERVICE", GetMessage.GetMessageDescription("I0001"));

            // Infinte loop processing for selecting register task.
            while (this._infiniteLoop)
            {
                try
                {
                    // Get asynchronous task from the database.
                    AsyncProcessingServiceParameterValue asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "SelectTask", "SelectTask", "SQL",
                                                                                            new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));
                    asyncParameterValue.RegistrationDateTime = DateTime.Now - new TimeSpan(_maxNumberOfHours, 0, 0);
                    asyncParameterValue.NumberOfRetries = this._maxNumberOfRetries;
                    asyncParameterValue.CompletionDateTime = DateTime.Now - new TimeSpan(_maxNumberOfHours, 0, 0);
                    LayerB layerB = new LayerB();
                    AsyncProcessingServiceReturnValue selectedAsyncTask = (AsyncProcessingServiceReturnValue)layerB.DoBusinessLogic(
                                                                              (BaseParameterValue)asyncParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);

                    // Existence check of asynchronous task
                    if (string.IsNullOrEmpty(selectedAsyncTask.UserId))
                    {
                        // Asynchronous task does not exist.  

                        // Wait for the asynchronous task to be registered in the database.
                        Thread.Sleep(this._numberOfSeconds * 1000);

                        // Continue to this infinite loop.
                        continue;
                    }
                    // Asynchronous task exists.

                    // Check the number of free worker threads.
                    int freeWorkerThreads = 0;
                    int completionPortThreads = 0;

                    // Gets the available threads.
                    ThreadPool.GetAvailableThreads(out freeWorkerThreads, out completionPortThreads);

                    while (freeWorkerThreads == 0)
                    {
                        // Wait for the completion of the worker thread.
                        Thread.Sleep(this._numberOfSeconds * 1000);

                        // Get available threads.
                        ThreadPool.GetAvailableThreads(out freeWorkerThreads, out completionPortThreads);
                    }

                    // Selected asynchronous task is assigned to a worker thread
                    this.UpdateAsyncTask(selectedAsyncTask, AsyncTaskUpdate.START);
                    LogIF.InfoLog("ASYNC-SERVICE", string.Format(GetMessage.GetMessageDescription("I0002"), selectedAsyncTask.TaskId));

                    // Assign the task to the worker thread
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.WorkerThreadCallBack), (object)selectedAsyncTask);
                }
                catch (Exception ex)
                {
                    // Service Failed due to unexpected exception.
                    this.StopAsyncProcess();
                    LogIF.ErrorLog("ASYNC-SERVICE", string.Format(GetMessage.GetMessageDescription("E0000"), ex.Message.ToString()));
                }
            }
        }

        #endregion

        #region WorkerThreadCallBack

        /// <summary>
        ///  Maintains the single worker thread functionalities. 
        /// </summary>
        /// <param name="asyncTask">Selected Asynchronous Task</param>
        private void WorkerThreadCallBack(object asyncTask)
        {
            AsyncProcessingServiceReturnValue selectedAsyncTask = (AsyncProcessingServiceReturnValue)asyncTask;

            try
            {
                // Call User Program to execute by using communication control function
                AsyncProcessingServiceParameterValue asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", "StartCopyFromBlob", "StartCopyFromBlob", "SQL",
                                                                                        new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));
                asyncParameterValue.Data = selectedAsyncTask.Data;
                CallController callController = new CallController(asyncParameterValue.User);
                AsyncProcessingServiceReturnValue asyncReturnValue = (AsyncProcessingServiceReturnValue)callController.Invoke(selectedAsyncTask.ProcessName, asyncParameterValue);

                if (asyncReturnValue.ErrorFlag == true)
                {
                    selectedAsyncTask.NumberOfRetries += 1;

                    if (asyncReturnValue.ErrorMessageID == "APSStopCommand")
                    {
                        // Asynchronous task is stopped due to user 'stop' command.
                        this.UpdateAsyncTask(selectedAsyncTask, AsyncTaskUpdate.FAIL);
                        LogIF.ErrorLog("ASYNC-SERVICE", string.Format(GetMessage.GetMessageDescription("E0001") + asyncReturnValue.ErrorMessage, selectedAsyncTask.TaskId));
                    }
                    else
                    {
                        // Exception occurred by other than BusinessApplicationException

                        if (selectedAsyncTask.NumberOfRetries < this._maxNumberOfRetries)
                        {
                            // Asynchronous task does not exceeds the maximum number of retries
                            // Updated as retry later
                            this.UpdateAsyncTask(selectedAsyncTask, AsyncTaskUpdate.RETRY);
                            LogIF.ErrorLog("ASYNC-SERVICE", string.Format(GetMessage.GetMessageDescription("E0004"), selectedAsyncTask.TaskId));
                        }
                        else
                        {
                            // Asynchronous task exceeds maximum number of retries
                            // Update task as abort
                            this.UpdateAsyncTask(selectedAsyncTask, AsyncTaskUpdate.FAIL);
                            LogIF.ErrorLog("ASYNC-SERVICE", string.Format(GetMessage.GetMessageDescription("E0005"), selectedAsyncTask.TaskId));
                        }
                    }
                }
                else
                {
                    // Selected Asynchronous task is completed successfully.
                    this.UpdateAsyncTask(selectedAsyncTask, AsyncTaskUpdate.SUCCESS);
                    LogIF.InfoLog("ASYNC-SERVICE", string.Format(GetMessage.GetMessageDescription("I0002"), selectedAsyncTask.TaskId));
                }
            }
            catch (Exception ex)
            {
                // Asynchronous task is aborted due to unexpected exception.
                AsyncProcessingServiceReturnValue asyncReturnValue = this.UpdateAsyncTask(selectedAsyncTask, AsyncTaskUpdate.FAIL);
                LogIF.ErrorLog("ASYNC-SERVICE", string.Format(GetMessage.GetMessageDescription("E0006"), selectedAsyncTask.TaskId, ex.Message.ToString()));
            }
        }

        #endregion

        #endregion

        #region Other method

        #region SetFromConfig

        /// <summary>
        /// Sets all intial values to member variables from the .config file
        /// </summary>
        public void SetFromConfig()
        {
            try
            {
                // Get maximum thread count from .config file
                string maxThreadCount = GetConfigParameter.GetConfigValue("FxMaxThreadCount");
                if (string.IsNullOrEmpty(maxThreadCount))
                {
                    // Set default
                    _maxThreadCount = 10;
                }
                else
                {
                    _maxThreadCount = int.Parse(maxThreadCount);
                }

                // Get number of seconds from .config file
                string numberOfSeconds = GetConfigParameter.GetConfigValue("FxMaxThreadCount");
                if (string.IsNullOrEmpty(numberOfSeconds))
                {
                    // Set default
                    _numberOfSeconds = 1;
                }
                else
                {
                    _numberOfSeconds = int.Parse(numberOfSeconds);
                }

                // Get maximum number of retries from .config file
                string maxNumberOfRetries = GetConfigParameter.GetConfigValue("FxMaxNumberOfRetries");
                if (string.IsNullOrEmpty(maxNumberOfRetries))
                {
                    // Set default
                    _maxNumberOfRetries = 10;
                }
                else
                {
                    _maxNumberOfRetries = int.Parse(maxNumberOfRetries);
                }

                // Get maximum number of hours from .config file
                string maxNumberOfHours = GetConfigParameter.GetConfigValue("FxMaxNumberOfHours");
                if (string.IsNullOrEmpty(maxNumberOfHours))
                {
                    // Set default
                    _maxNumberOfHours = 24;
                }
                else
                {
                    // Get maximum number of hours from .config file
                    _maxNumberOfHours = int.Parse(maxNumberOfHours);
                }
            }
            catch 
            {
                // Error while setting from config
                LogIF.ErrorLog("ASYNC-SERVICE", GetMessage.GetMessageDescription("E0010"));
                throw new Exception();
            }
        }

        #endregion

        #region UpdateAsyncTask

        /// <summary>
        ///  Updates the selected asynchronous task based type of SQL_Update_Method_name
        /// </summary>
        /// <param name="selectedAsyncTask">Selected Asynchronous Task</param>
        /// <param name="updateTask">SQL_Update_Method_name</param>
        /// <returns></returns>
        public AsyncProcessingServiceReturnValue UpdateAsyncTask(AsyncProcessingServiceReturnValue selectedAsyncTask, string updateTask)
        {
            AsyncProcessingServiceParameterValue asyncParameterValue = new AsyncProcessingServiceParameterValue("AsyncProcessingService", updateTask, updateTask, "SQL",
                                                                                        new MyUserInfo("AsyncProcessingService", "AsyncProcessingService"));
            asyncParameterValue.TaskId = selectedAsyncTask.TaskId;

            // Update databse based on task
            switch (updateTask)
            {
                case AsyncTaskUpdate.START:
                    asyncParameterValue.ExecutionStartDateTime = DateTime.Now;
                    asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.Processing;
                    break;
                case AsyncTaskUpdate.RETRY:
                    asyncParameterValue.NumberOfRetries = selectedAsyncTask.NumberOfRetries + 1;
                    asyncParameterValue.CompletionDateTime = DateTime.Now;
                    asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.AbnormalEnd;
                    break;
                case AsyncTaskUpdate.FAIL:
                    asyncParameterValue.CompletionDateTime = DateTime.Now;
                    asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.Abort;
                    break;
                case AsyncTaskUpdate.SUCCESS:
                    asyncParameterValue.CompletionDateTime = DateTime.Now;
                    asyncParameterValue.ProgressRate = 100;
                    asyncParameterValue.StatusId = (int)AsyncProcessingServiceParameterValue.AsyncStatus.End;
                    break;
            }
            LayerB layerB = new LayerB();
            AsyncProcessingServiceReturnValue asyncReturnValue = (AsyncProcessingServiceReturnValue)layerB.DoBusinessLogic(
                                                                      (BaseParameterValue)asyncParameterValue, DbEnum.IsolationLevelEnum.ReadCommitted);
            return asyncReturnValue;
        }

        #endregion

        #region DeserializeFromBase64

        /// <summary>
        /// Converts string data to byte array and byte array data to object.
        /// </summary>
        /// <param name="strBase64">Base64 String</param>
        /// <returns>Deserialized Object</returns>
        public object DeserializeFromBase64(string strBase64)
        {
            byte[] deserializeData = null;
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();

            deserializeData = CustomEncode.FromBase64String(strBase64);
            memStream.Write(deserializeData, 0, deserializeData.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            object obj = (object)binForm.Deserialize(memStream);

            return obj;
        }

        #endregion

        #region StopAsyncProcess

        /// <summary>
        ///  Stop the asynchronous service and Waits to complete all worker thread to complete.
        /// </summary>
        public void StopAsyncProcess()
        {
            // Breaks the infinite loop of Main thread.
            this._infiniteLoop = false;

            // Wait the end of the main thread.
            this._mainThread.Join();

            // Check the number of free worker threads.
            int freeWorkerThreads = 0;
            int completionPortThreads = 0;

            // Get available threads.
            ThreadPool.GetAvailableThreads(out freeWorkerThreads, out completionPortThreads);

            while (freeWorkerThreads != this._maxThreadCount)
            {
                // Wait for the completion of the worker thread.
                Thread.Sleep(this._numberOfSeconds * 1000);

                // Get available threads.
                ThreadPool.GetAvailableThreads(out freeWorkerThreads, out completionPortThreads);
            }
        }

        #endregion

        #endregion
    }
}
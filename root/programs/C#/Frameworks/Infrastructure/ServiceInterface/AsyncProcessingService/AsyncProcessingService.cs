using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;

using AsyncProcessingService.Codes.Business;
using AsyncProcessingService.Codes.Common;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Public.Db;

namespace AsyncProcessingService
{
    public class AsyncProcessingService : System.ServiceProcess.ServiceBase
    {
        int _threadCount = 0;

        public AsyncProcessingService()
        {
            this.CanPauseAndContinue = true;
            this.CanStop = true;
            this.AutoLog = true;
        }

        static void Main()
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
        /// Inserts service information in database
        /// </summary>
        public void InsertServiceData(object state)
        {
            TestParameterValue testparameterValue = new TestParameterValue("AsyncProcessingService", "button", "Start", "SQL" + "%" + "individual" +
                                                                           "%" + "Static" + "%" + "-", new MyUserInfo("aaa", "192.168.1.1"));
            testparameterValue.UserId = "A";
            testparameterValue.ProcessName = "AAA";
            string data = "1234";
            testparameterValue.Data = ToBase64String(data);
            testparameterValue.ExecutionStartDateTime = DateTime.Now;
            testparameterValue.RegistrationDateTime = DateTime.Now;
            testparameterValue.NumberOfRetries = 0;
            testparameterValue.ProgressRate = 0;
            testparameterValue.CompletionDateTime = DateTime.Now;
            testparameterValue.Status = "Register";
            testparameterValue.Command = null;
            testparameterValue.ReservedArea = "xxxxxxxxxx";

            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;
            TestReturnValue testReturnValue;

            if (testparameterValue != null)
            {
                LayerB layerB = new LayerB();
                testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testparameterValue, iso);
            }
        }

        /// <summary>
        /// Updates service information in database
        /// </summary>
        public void UpdateServiceDataByStatus(object status)
        {
            //Setting update parameters
            TestParameterValue testparameterValue = new TestParameterValue("AsyncProcessingService", "button", "Update", "SQL" + "%" + "individual" +
                                                                            "%" + "Static" + "%" + "-", new MyUserInfo("aaa", "192.168.1.1"));
            testparameterValue.UserId = "A";
            testparameterValue.ExecutionStartDateTime = DateTime.Now;
            testparameterValue.RegistrationDateTime = DateTime.Now;
            testparameterValue.Status = status.ToString();

            //Based on the service status setting database command value
            switch (status.ToString())
            {
                case "processing":
                    testparameterValue.ProgressRate = 50;
                    testparameterValue.Command = "Stop";
                    break;
                case "stop":
                    testparameterValue.ProgressRate = 100;
                    testparameterValue.Command = null;
                    break;
                case "Abnormal End":
                    testparameterValue.ProgressRate = 0;
                    testparameterValue.Command = null;
                    break;
                case "Abort":
                    testparameterValue.ProgressRate = 0;
                    testparameterValue.Command = null;
                    break;
            }
            testparameterValue.NumberOfRetries = _threadCount;
            testparameterValue.CompletionDateTime = DateTime.Now;

            //Calling LayerB DoBusinessLogic method for updating
            DbEnum.IsolationLevelEnum iso = DbEnum.IsolationLevelEnum.DefaultTransaction;
            TestReturnValue testReturnValue;
            if (testparameterValue != null)
            {
                LayerB layerB = new LayerB();
                testReturnValue = (TestReturnValue)layerB.DoBusinessLogic(testparameterValue, iso);
            }
        }

        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        protected override void OnStart(string[] args)
        {
            StartByThreadPool("start");
        }

        public bool StartByThreadPool(object check)
        {
            int maxThreadCount;

            lock (this)
            {
                maxThreadCount
                    = int.Parse(ConfigurationManager.AppSettings.Get("FxMaxThreadCount").ToString());

                if (_threadCount >= maxThreadCount)
                {
                    UpdateServiceDataByStatus("Abort");
                    return false;
                }

                _threadCount++;

                //Based on the service status Calling callback method
                if (check == "start")
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(InsertServiceData));
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(UpdateServiceDataByStatus), check.ToString());
                }
            }
            return true;
        }

        private string ToBase64String(string p)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(p));
        }

        public object DeserializeBase64(string data)
        {
            // We need to know the exact length of the string - Base64 can sometimes pad us by a byte or two
            int p = data.IndexOf(':');
            int length = Convert.ToInt32(data.Substring(0, p));
            // Extract data from the base 64 string!
            byte[] memoryData = Convert.FromBase64String(data.Substring(p + 1));
            MemoryStream rs = new MemoryStream(memoryData, 0, length);
            BinaryFormatter sf = new BinaryFormatter();
            object obj = sf.Deserialize(rs);
            return obj;
        }

        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop()
        {
            StartByThreadPool("stop");
        }

        protected override void OnPause()
        {
            StartByThreadPool("Abort");
        }

        protected override void OnShutdown()
        {
            StartByThreadPool("Abnormal End");

        }
    }
}


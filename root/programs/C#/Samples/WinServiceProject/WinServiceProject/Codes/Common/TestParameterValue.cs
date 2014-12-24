using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// ベースクラス
using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Business.Common;

namespace WinServiceProject.Codes.Common
{
    public class TestParameterValue : MyParameterValue
    {
        /// <summary>汎用エリア</summary>
        public object Obj;

        /// <summary>Id</summary>
        public int Id;

        /// <summary>UserId</summary>
        public string UserId;

        /// <summary>ProcessName</summary>
        public string ProcessName;

        /// <summary>Data</summary>
       public string Data;

        /// <summary>RegistrationDateTime</summary>
        public DateTime RegistrationDateTime;

        /// <summary>ExecutionStartDateTime</summary>
        public DateTime ExecutionStartDateTime;

        /// <summary>NumberOfRetries</summary>
        public int NumberOfRetries;

        /// <summary>ProgressRate</summary>
        public int ProgressRate;

        /// <summary>Status</summary>
        public string Status;

        /// <summary>CompletionDateTime</summary>
        public DateTime CompletionDateTime;

        /// <summary>Command</summary>
        public string Command;

        /// <summary>ReservedArea</summary>
        public string ReservedArea;



        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        public TestParameterValue(string screenId, string controlId, string methodName, string actionType, MyUserInfo user)
            : base(screenId, controlId, methodName, actionType, user)
        {
            // Baseのコンストラクタに引数を渡すために必要。
        }

        #endregion
    }
}
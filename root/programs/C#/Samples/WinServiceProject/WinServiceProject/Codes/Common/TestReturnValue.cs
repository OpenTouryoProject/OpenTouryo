using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// ベースクラス
using Touryo.Infrastructure.Business.Common;

namespace WinServiceProject.Codes.Common
{
    public class TestReturnValue : MyReturnValue
    {
        /// <summary>汎用エリア</summary>
        public object Obj;

        /// <summary>UserId</summary>
        public string UserId;

        /// <summary>RegistrationDateTime</summary>
        public DateTime RegistrationDateTime;

        /// <summary> ExecutionStartDateTime;</summary>
        public DateTime ExecutionStartDateTime;

        /// <summary> NumberOfRetries;</summary>
        public int NumberOfRetries;

        /// <summary>ProgressRate</summary>
        public int ProgressRate;

        /// <summary> Status;</summary>
        public string Status;

        /// <summary> Command;</summary>
        public string Command;

        /// <summary>ReservedArea</summary>
        public string ReservedArea;
    }
}
using System;
using System.Text;
using System.IO;

using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestOutputLog
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            LogIF.DebugLog("ACCESS", "LogIF.DebugLog(\"ACCESS\");");
            LogIF.InfoLog("ACCESS", "LogIF.InfoLog(\"ACCESS\");");
            LogIF.WarnLog("ACCESS", "LogIF.WarnLog(\"ACCESS\");");
            LogIF.ErrorLog("ACCESS", "LogIF.ErrorLog(\"ACCESS\");");
            LogIF.FatalLog("ACCESS", "LogIF.FatalLog(\"ACCESS\");");
        }
        #endregion
    }
}
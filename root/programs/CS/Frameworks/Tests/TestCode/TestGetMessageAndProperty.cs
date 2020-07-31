using System;
using System.Text;
using System.IO;

using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestGetMessageAndProperty
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            MyDebug.OutputDebugAndConsole("GetMessage: " + GetMessage.GetMessageDescription("I0001"));
            MyDebug.OutputDebugAndConsole("GetMessage: " + GetMessage.GetMessageDescription("E0001"));

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            MyDebug.OutputDebugAndConsole("GetSharedProperty: " + GetSharedProperty.GetSharedPropertyValue("ConnectionString1"));
            MyDebug.OutputDebugAndConsole("GetSharedProperty: " + GetSharedProperty.GetSharedPropertyValue("HostName1"));
        }
        #endregion
    }
}
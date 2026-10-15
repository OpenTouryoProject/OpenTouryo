#region Apache License
//
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
//* クラス名        ：TestOutputLog
//* クラス日本語名  ：ログ出力のテスト
//*
//* 作成者          ：西野 大介
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2020/07/31  西野 大介         新規作成
//*  2026/08/17  玄人 幸道         ログレベルの有効・無効（IsDebugEnabledほか）のテストを追加（#552）
//**********************************************************************************

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

            TestOutputLog.IsEnabledTest();
        }
        #endregion

        #region private

        /// <summary>ログ レベルの有効・無効（IsDebugEnabled ほか）</summary>
        /// <remarks>
        /// **ロガー名ごとに設定が違う。**
        /// SampleLogConf.xml で ACCESS は ALL、他は既定になっているので、
        /// **設定に無いロガー名も渡して差を見る。**
        /// </remarks>
        private static void IsEnabledTest()
        {
            foreach (string logger in new string[] { "ACCESS", "UNDEFINED" })
            {
                MyDebug.OutputDebugAndConsole(
                    "LogIF.Is*Enabled - " + logger + " : "
                    + "Debug " + LogIF.IsDebugEnabled(logger)
                    + " / Info " + LogIF.IsInfoEnabled(logger)
                    + " / Warn " + LogIF.IsWarnEnabled(logger)
                    + " / Error " + LogIF.IsErrorEnabled(logger)
                    + " / Fatal " + LogIF.IsFatalEnabled(logger));
            }
        }

        #endregion
    }
}
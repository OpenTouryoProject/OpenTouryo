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
//* クラス名        ：LogIFNLogTests
//* クラス日本語名  ：LogIF（NLog）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//*  2026/10/15  玄人 幸道         TestPublic の Log フォルダを Logging に改めたのに追随（#610）
//**********************************************************************************

using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Log;

namespace TestPublicNLog
{
    /// <summary>LogIF（NLog）の単体テスト</summary>
    /// <remarks>
    /// 構成は NUnitNLogConf.xml。LogLib=nlog で、LogIF の呼び出しが NLog へ向かう。
    ///   NLOG_INFO … Info 以上を NUnitLog/NLOG_INFO.log へ
    ///   NLOG_WARN … Warn 以上を NUnitLog/NLOG_WARN.log へ
    /// log4net の側は TestPublic の Logging/LogIFTests.cs。
    /// </remarks>
    [TestFixture]
    public class LogIFNLogTests
    {
        /// <summary>出力ファイルを読む（BaseDirectory 基準）。</summary>
        private static string ReadLog(string name)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "NUnitLog", name + ".log");

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>Info のロガーは Debug を出さず、Info 以上を出す。</summary>
        [Test]
        public void InfoLogger_WritesInfoAndAbove()
        {
            string id = Guid.NewGuid().ToString("N");

            LogIF.DebugLog("NLOG_INFO", "debug " + id);
            LogIF.InfoLog("NLOG_INFO", "info " + id);
            LogIF.WarnLog("NLOG_INFO", "warn " + id);
            LogIF.ErrorLog("NLOG_INFO", "error " + id);
            LogIF.FatalLog("NLOG_INFO", "fatal " + id);

            string log = ReadLog("NLOG_INFO");

            Assert.That(log, Does.Not.Contain("debug " + id));
            Assert.That(log, Does.Contain("[INFO ],info " + id));
            Assert.That(log, Does.Contain("[WARN ],warn " + id));
            Assert.That(log, Does.Contain("[ERROR],error " + id));
            Assert.That(log, Does.Contain("[FATAL],fatal " + id));
        }

        /// <summary>Warn のロガーは Info 以下を出さない。ロガーごとに別のファイルへ出る。</summary>
        [Test]
        public void WarnLogger_WritesWarnAndAbove()
        {
            string id = Guid.NewGuid().ToString("N");

            LogIF.InfoLog("NLOG_WARN", "info " + id);
            LogIF.WarnLog("NLOG_WARN", "warn " + id);

            string log = ReadLog("NLOG_WARN");

            Assert.That(log, Does.Not.Contain("info " + id));
            Assert.That(log, Does.Contain("[WARN ],warn " + id));
        }

        /// <summary>Is*Enabled は規則（minlevel）どおりに答える。</summary>
        [Test]
        public void IsEnabled_FollowsLevel()
        {
            Assert.That(LogIF.IsDebugEnabled("NLOG_INFO"), Is.False, "INFO:Debug");
            Assert.That(LogIF.IsInfoEnabled("NLOG_INFO"), Is.True, "INFO:Info");
            Assert.That(LogIF.IsWarnEnabled("NLOG_INFO"), Is.True, "INFO:Warn");
            Assert.That(LogIF.IsErrorEnabled("NLOG_INFO"), Is.True, "INFO:Error");
            Assert.That(LogIF.IsFatalEnabled("NLOG_INFO"), Is.True, "INFO:Fatal");

            Assert.That(LogIF.IsInfoEnabled("NLOG_WARN"), Is.False, "WARN:Info");
            Assert.That(LogIF.IsWarnEnabled("NLOG_WARN"), Is.True, "WARN:Warn");
        }

        /// <summary>
        /// 規則に無いロガー名でも例外にならない。
        /// **log4net と違い、NLog は規則に無いロガーを無効として答える**（ルートの既定が無い）。
        /// </summary>
        [Test]
        public void UnknownLogger_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => LogIF.InfoLog("NLOG_UNKNOWN", "x"));
            Assert.That(LogIF.IsDebugEnabled("NLOG_UNKNOWN"), Is.False);
        }
    }
}

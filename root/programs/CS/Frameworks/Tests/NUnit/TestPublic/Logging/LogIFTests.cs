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
//* クラス名        ：LogIFTests
//* クラス日本語名  ：LogIF（log4net）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Log;

namespace TestPublic.Logging
{
    /// <summary>LogIF（log4net）の単体テスト</summary>
    /// <remarks>
    /// 構成は NUnitLogConf.xml（FxLog4NetConfFile）。
    ///   NUNIT_INFO … INFO 以上を NUnitLog/NUNIT_INFO.log へ
    ///   NUNIT_WARN … WARN 以上を NUnitLog/NUNIT_WARN.log へ
    /// **LogIF と log4net の状態はプロセスで 1 つ**なので、ロガーごとに 1 回だけ書いて確かめる。
    /// NLog（LogLib=nlog）への切り替えは、静的コンストラクタで 1 回だけ決まるため、
    /// 同じプロセスでは確かめられない（対象外）。
    /// </remarks>
    [TestFixture]
    public class LogIFTests
    {
        /// <summary>出力ファイル（BaseDirectory 基準）</summary>
        private static string LogFile(string name)
        {
            return Path.Combine(AppContext.BaseDirectory, "NUnitLog", name + ".log");
        }

        /// <summary>
        /// 書き込み中でも読めるよう、共有で開いて読む。
        /// </summary>
        private static string ReadLog(string name)
        {
            using (FileStream fs = new FileStream(LogFile(name), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>INFO のロガーは DEBUG を出さず、INFO 以上を出す。</summary>
        [Test]
        public void InfoLogger_WritesInfoAndAbove()
        {
            string id = Guid.NewGuid().ToString("N");

            LogIF.DebugLog("NUNIT_INFO", "debug " + id);
            LogIF.InfoLog("NUNIT_INFO", "info " + id);
            LogIF.WarnLog("NUNIT_INFO", "warn " + id);
            LogIF.ErrorLog("NUNIT_INFO", "error " + id);
            LogIF.FatalLog("NUNIT_INFO", "fatal " + id);

            string log = ReadLog("NUNIT_INFO");

            Assert.That(log, Does.Not.Contain("debug " + id));
            Assert.That(log, Does.Contain("[INFO ],info " + id));
            Assert.That(log, Does.Contain("[WARN ],warn " + id));
            Assert.That(log, Does.Contain("[ERROR],error " + id));
            Assert.That(log, Does.Contain("[FATAL],fatal " + id));
        }

        /// <summary>WARN のロガーは INFO 以下を出さない。ロガーごとに別のファイルへ出る。</summary>
        [Test]
        public void WarnLogger_WritesWarnAndAbove()
        {
            string id = Guid.NewGuid().ToString("N");

            LogIF.InfoLog("NUNIT_WARN", "info " + id);
            LogIF.WarnLog("NUNIT_WARN", "warn " + id);

            string log = ReadLog("NUNIT_WARN");

            Assert.That(log, Does.Not.Contain("info " + id));
            Assert.That(log, Does.Contain("[WARN ],warn " + id));
        }

        /// <summary>Is*Enabled はロガーの閾値どおりに答える。</summary>
        [Test]
        public void IsEnabled_FollowsLevel()
        {
            Assert.That(LogIF.IsDebugEnabled("NUNIT_INFO"), Is.False, "INFO:Debug");
            Assert.That(LogIF.IsInfoEnabled("NUNIT_INFO"), Is.True, "INFO:Info");
            Assert.That(LogIF.IsWarnEnabled("NUNIT_INFO"), Is.True, "INFO:Warn");
            Assert.That(LogIF.IsErrorEnabled("NUNIT_INFO"), Is.True, "INFO:Error");
            Assert.That(LogIF.IsFatalEnabled("NUNIT_INFO"), Is.True, "INFO:Fatal");

            Assert.That(LogIF.IsInfoEnabled("NUNIT_WARN"), Is.False, "WARN:Info");
            Assert.That(LogIF.IsWarnEnabled("NUNIT_WARN"), Is.True, "WARN:Warn");
        }

        /// <summary>
        /// 構成に無いロガー名でも例外にならない（ルート ロガーの設定に従う）。
        /// **Is*Enabled は true を返す。** log4net のルート ロガーの既定のレベルは DEBUG で、
        /// NUnitLogConf.xml はルートにアペンダを付けていないため、有効だが何も出ない。
        /// 「出ていないから無効」と読まないこと。
        /// </summary>
        [Test]
        public void UnknownLogger_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => LogIF.InfoLog("NUNIT_UNKNOWN", "x"));
            Assert.That(LogIF.IsDebugEnabled("NUNIT_UNKNOWN"), Is.True);
        }
    }
}

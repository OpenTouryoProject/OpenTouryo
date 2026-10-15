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
//* クラス名        ：CustomEventLogTests
//* クラス日本語名  ：CustomEventLog の単体テスト（net48・管理者のみ）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

#if !NETCOREAPP
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Log;

namespace TestPublic.Logging
{
    /// <summary>CustomEventLog の単体テスト（net48・管理者のみ）</summary>
    /// <remarks>
    /// **CustomEventLog は net48 専用**（Public_netcore100.csproj が除外している）。
    ///
    /// **イベント ソースの作成・削除には管理者権限が要る。** 管理者でなければスキップする。
    /// GitHub Actions の windows-latest は管理者で動くので、CI では実行される。
    ///
    /// **OS の状態を変える**（イベント ログとソースを作って消す）。
    /// 既存のログに触れないよう、このテスト専用のログ（OTNUnitLog）を使い、TearDown で必ず消す。
    ///
    /// **CustomEventLog.Delete は、ソースが属するログごと消す。**
    /// Application などの既存のログに属するソースを渡してはならない（テストでも渡さない）。
    /// </remarks>
    [TestFixture]
    public class CustomEventLogTests
    {
        /// <summary>専用のログ名（ログ名は先頭 8 文字で区別されるので、既存と重ならない名前にする）</summary>
        private const string LogName = "OTNUnitLog";

        /// <summary>テストごとのソース名</summary>
        private string _source;

        /// <summary>管理者でなければスキップする。</summary>
        [SetUp]
        public void SetUp()
        {
            using (WindowsIdentity id = WindowsIdentity.GetCurrent())
            {
                if (!new WindowsPrincipal(id).IsInRole(WindowsBuiltInRole.Administrator))
                {
                    Assert.Ignore("イベント ソースの作成には管理者権限が要るため、スキップした。");
                }
            }

            this._source = "OTNUnit_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        /// <summary>作ったソースとログを消す（テストが途中で失敗しても）。</summary>
        [TearDown]
        public void TearDown()
        {
            if (this._source == null) { return; }

            if (EventLog.SourceExists(this._source)) { EventLog.DeleteEventSource(this._source); }
            if (EventLog.Exists(LogName)) { EventLog.Delete(LogName); }
        }

        /// <summary>ソースが無ければ、指定のログに作る。</summary>
        [Test]
        public void Constructor_CreatesSource()
        {
            CustomEventLog log = new CustomEventLog(this._source, LogName);

            Assert.That(log.Source, Is.EqualTo(this._source));
            Assert.That(log.LogName, Is.EqualTo(LogName));
            Assert.That(EventLog.SourceExists(this._source), Is.True);
            Assert.That(EventLog.LogNameFromSourceName(this._source, "."), Is.EqualTo(LogName));
        }

        /// <summary>WriteInfo / Write で書いたエントリが、そのログに残る。</summary>
        [Test]
        public void Write_AddsEntries()
        {
            CustomEventLog log = new CustomEventLog(this._source, LogName);
            string id = Guid.NewGuid().ToString("N");

            log.WriteInfo("info " + id);
            log.Write("warn " + id, EventLogEntryType.Warning, 123, 0, null);

            using (EventLog el = new EventLog(LogName))
            {
                EventLogEntry[] entries = el.Entries.Cast<EventLogEntry>()
                    .Where(e => e.Source == this._source).ToArray();

                Assert.That(entries.Any(e => e.Message == "info " + id
                    && e.EntryType == EventLogEntryType.Information), Is.True, "WriteInfo");
                Assert.That(entries.Any(e => e.Message == "warn " + id
                    && e.EntryType == EventLogEntryType.Warning
                    && e.InstanceId == 123), Is.True, "Write");
            }
        }

        /// <summary>Delete はソースとログを消して true。無ければ false。</summary>
        [Test]
        public void Delete()
        {
            new CustomEventLog(this._source, LogName);

            Assert.That(CustomEventLog.Delete(this._source), Is.True);
            Assert.That(EventLog.SourceExists(this._source), Is.False);
            Assert.That(EventLog.Exists(LogName), Is.False);

            Assert.That(CustomEventLog.Delete(this._source), Is.False);
        }
    }
}
#endif

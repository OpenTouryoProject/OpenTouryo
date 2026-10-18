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
//* クラス名        ：EnvInfoTests
//* クラス日本語名  ：EnvInfo の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/16  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Util;

namespace TestPublic.Util
{
    /// <summary>EnvInfo の単体テスト</summary>
    /// <remarks>
    /// 値は実行環境で変わるので、**Environment から得られる値と食い違わないこと**を見る。
    /// </remarks>
    [TestFixture]
    public class EnvInfoTests
    {
        /// <summary>OS・プロセスのビット数は、真偽値と対応する。</summary>
        [Test]
        public void Bitness()
        {
            Assert.That(EnvInfo.Is64BitOS, Is.EqualTo(Environment.Is64BitOperatingSystem));
            Assert.That(EnvInfo.OsBit, Is.EqualTo(EnvInfo.Is64BitOS ? 64 : 32));
            Assert.That(EnvInfo.Is64BitProcess, Is.EqualTo(Environment.Is64BitProcess));
            Assert.That(EnvInfo.ProcessBit, Is.EqualTo(EnvInfo.Is64BitProcess ? 64 : 32));
        }

        /// <summary>OS・ランタイムの版と、マシン名。</summary>
        [Test]
        public void VersionsAndNames()
        {
            // OperatingSystem は Equals を持たないので、中身で比べる。
            Assert.That(EnvInfo.OsVersion.Platform, Is.EqualTo(Environment.OSVersion.Platform));
            Assert.That(EnvInfo.OsVersion.Version, Is.EqualTo(Environment.OSVersion.Version));
            Assert.That(EnvInfo.OsVersionString, Is.EqualTo(Environment.OSVersion.VersionString));
            Assert.That(EnvInfo.FrameworkVersion, Is.EqualTo(Environment.Version));
            Assert.That(EnvInfo.FrameworkVersionString, Is.EqualTo(Environment.Version.ToString()));
            Assert.That(EnvInfo.MachineName, Is.EqualTo(Environment.MachineName));
            Assert.That(EnvInfo.DnsHostName, Is.Not.Empty);
        }

#if !NETCOREAPP
        /// <summary>net48 だけ、レジストリから OS と .NET Framework の情報を読む。</summary>
        [Test]
        public void Registry_Net48Only()
        {
            Assert.That(EnvInfo.OsProductName, Is.Not.Empty);
            Assert.That(EnvInfo.OsBuild, Is.Not.Empty);
            Assert.That(EnvInfo.RegistryFrameworkVersion, Does.StartWith("4."));
            Assert.That(int.Parse(EnvInfo.RegistryFrameworkRelease), Is.GreaterThanOrEqualTo(528040)); // 4.8 以上
        }
#endif
    }
}

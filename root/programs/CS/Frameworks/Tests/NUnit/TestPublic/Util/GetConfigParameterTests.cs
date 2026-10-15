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
//* クラス名        ：GetConfigParameterTests
//* クラス日本語名  ：GetConfigParameter の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

using System;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Util;

#if NETCOREAPP
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Extensions.Configuration;
#endif

namespace TestPublic.Util
{
    /// <summary>GetConfigParameter の単体テスト</summary>
    /// <remarks>
    /// net48 は App.config（ConfigurationManager）、.NET は IConfiguration から読む。
    /// 値は TestPublic の App.config と TestSetup.cs に同じものを置いてある。
    ///
    /// **コンテナ・モード（FxContainerization=on で環境変数を優先）は .NET で確かめる。**
    /// 判定の処理（CheckContainerization）は両方で共通で、違うのは設定ファイルの読み方だけ。
    /// .NET は構成を差し替えられるが、net48 の App.config はプロセスで固定されるため。
    /// 差し替えた構成は TearDown で必ず戻す（他のテストが使っている）。
    /// </remarks>
    [TestFixture]
    public class GetConfigParameterTests
    {
        #region 共通（net48 / .NET）

        /// <summary>appSettings の値を読む。無いキーは null。</summary>
        [Test]
        public void GetConfigValue()
        {
            Assert.That(GetConfigParameter.GetConfigValue("NUnitKey"), Is.EqualTo("NUnitValue"));
            Assert.That(GetConfigParameter.GetConfigValue("NoSuchKey"), Is.Null);
        }

        /// <summary>connectionStrings の値を読む。無い名前は null（例外にしない）。</summary>
        [Test]
        public void GetConnectionString()
        {
            Assert.That(GetConfigParameter.GetConnectionString("NUnitConn"),
                Is.EqualTo("Data Source=nunit;Initial Catalog=x;"));
            Assert.That(GetConfigParameter.GetConnectionString("NoSuchConn"), Is.Null);
        }

        #endregion

#if NETCOREAPP
        #region .NET のみ（構成の差し替え）

        private const string EnvKey = "OT_NUNIT_CONFIG_KEY";

        /// <summary>差し替える前の構成</summary>
        private IConfiguration _original;

        /// <summary>構成を退避する。</summary>
        [SetUp]
        public void SetUp()
        {
            this._original = GetConfigParameter.Configuration;
        }

        /// <summary>構成と環境変数を戻す。</summary>
        [TearDown]
        public void TearDown()
        {
            GetConfigParameter.InitConfiguration(this._original);
            Environment.SetEnvironmentVariable(EnvKey, null, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("Sec__Sub", null, EnvironmentVariableTarget.Process);
        }

        /// <summary>メモリ上の構成で差し替える。</summary>
        private static void Use(Dictionary<string, string> values)
        {
            GetConfigParameter.InitConfiguration(new ConfigurationBuilder().AddInMemoryCollection(values));
        }

        /// <summary>
        /// **コンテナ・モードでは、環境変数が設定ファイルより優先される。**
        /// 環境変数が無ければ設定ファイルの値。
        /// </summary>
        [Test]
        public void Containerization_On_PrefersEnvironment()
        {
            Use(new Dictionary<string, string>
            {
                { "appSettings:FxContainerization", "on" },
                { "appSettings:" + EnvKey, "file" },
                { "connectionStrings:" + EnvKey, "file-conn" },
            });

            Assert.That(GetConfigParameter.GetConfigValue(EnvKey), Is.EqualTo("file"));

            Environment.SetEnvironmentVariable(EnvKey, "env", EnvironmentVariableTarget.Process);
            Assert.That(GetConfigParameter.GetConfigValue(EnvKey), Is.EqualTo("env"));
            Assert.That(GetConfigParameter.GetConnectionString(EnvKey), Is.EqualTo("env"));

            // checkContainerization = false なら、環境変数を見ない
            Assert.That(GetConfigParameter.GetConfigValue(EnvKey, false), Is.EqualTo("file"));
        }

        /// <summary>コンテナ・モードでなければ、環境変数があっても設定ファイルの値。</summary>
        [Test]
        public void Containerization_Off_IgnoresEnvironment()
        {
            Use(new Dictionary<string, string> { { "appSettings:" + EnvKey, "file" } });
            Environment.SetEnvironmentVariable(EnvKey, "env", EnvironmentVariableTarget.Process);

            Assert.That(GetConfigParameter.GetConfigValue(EnvKey), Is.EqualTo("file"));
        }

        /// <summary>
        /// FxContainerization の on / off は大文字・小文字を問わない。
        /// **on / off 以外の値はテストしない。** 現状は ArgumentException にならず、
        /// 例外メッセージの取得（PublicExceptionMessage.CmnFunc）が同じ判定を通って再帰し、
        /// スタック オーバーフローでテスト プロセスごと落ちるため（#610 で報告済み）。
        /// </summary>
        [Test]
        public void Containerization_SwitchIsCaseInsensitive()
        {
            Use(new Dictionary<string, string> { { "appSettings:FxContainerization", "ON" } });
            Environment.SetEnvironmentVariable(EnvKey, "env", EnvironmentVariableTarget.Process);

            Assert.That(GetConfigParameter.GetConfigValue(EnvKey), Is.EqualTo("env"));
        }

        /// <summary>
        /// GetAnyConfigValue はセクションを : で区切って読む。
        /// **コンテナ・モードの環境変数名は : を __ に置き換えたもの**（.NET の環境変数の慣習）。
        /// </summary>
        [Test]
        public void GetAnyConfigValueAndSection()
        {
            Use(new Dictionary<string, string>
            {
                { "appSettings:FxContainerization", "on" },
                { "Sec:Sub", "file" },
                { "Sec:Other", "o" },
            });

            Assert.That(GetConfigParameter.GetAnyConfigValue("Sec:Sub"), Is.EqualTo("file"));
            Assert.That(GetConfigParameter.GetAnyConfigSection("Sec")["Other"], Is.EqualTo("o"));

            Environment.SetEnvironmentVariable("Sec__Sub", "env", EnvironmentVariableTarget.Process);
            Assert.That(GetConfigParameter.GetAnyConfigValue("Sec:Sub"), Is.EqualTo("env"));
        }

        /// <summary>JSON ファイル（BaseDirectory 基準）とストリームから初期化できる。</summary>
        [Test]
        public void InitConfiguration_JsonFileAndStream()
        {
            const string json = "{ \"appSettings\": { \"FromJson\": \"j\" } }";

            string name = "nunit_" + Guid.NewGuid().ToString("N") + ".json";
            string path = Path.Combine(AppContext.BaseDirectory, name);
            try
            {
                File.WriteAllText(path, json);
                GetConfigParameter.InitConfiguration(name);
                Assert.That(GetConfigParameter.GetConfigValue("FromJson"), Is.EqualTo("j"));
            }
            finally
            {
                File.Delete(path);
            }

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                GetConfigParameter.InitConfiguration(ms);
            }
            Assert.That(GetConfigParameter.GetConfigValue("FromJson"), Is.EqualTo("j"));
        }

        /// <summary>
        /// **初期化していなければ、どの読み取りも ArgumentException（NOT_INITIALIZED）。**
        /// .NET で設定を読む部品の前に InitConfiguration が要る理由。
        /// </summary>
        [Test]
        public void NotInitialized_Throws()
        {
            GetConfigParameter.InitConfiguration((IConfiguration)null);

            Assert.Throws<ArgumentException>(() => GetConfigParameter.GetConfigValue("NUnitKey"));
            Assert.Throws<ArgumentException>(() => GetConfigParameter.GetConnectionString("NUnitConn"));
            Assert.Throws<ArgumentException>(() => GetConfigParameter.GetAnyConfigValue("Sec:Sub"));
            Assert.Throws<ArgumentException>(() => GetConfigParameter.GetAnyConfigSection("Sec"));
        }

        #endregion
#endif
    }
}

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
//* クラス名        ：PublicExceptionMessageTests
//* クラス日本語名  ：PublicExceptionMessage（と、そのリソース）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/16  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Util;

#if NETCOREAPP
using Microsoft.Extensions.Configuration;
#endif

namespace TestPublic.Util
{
    /// <summary>PublicExceptionMessage（と、そのリソース）の単体テスト</summary>
    /// <remarks>
    /// PublicExceptionMessage は、**プロパティ名をキーにして**リソースを引く。
    /// キーの書き間違い・リソースの追加漏れは、ビルドでは分からず、例外を投げる時に初めて null になる。
    /// そこで、すべてのプロパティ・すべてのリソースを読んで確かめる。
    /// </remarks>
    [TestFixture]
    public class PublicExceptionMessageTests
    {
        /// <summary>リソース クラス（internal なのでリフレクションで取る）</summary>
        private static readonly Type ResourceType = typeof(PublicExceptionMessage).Assembly.GetType(
            "Touryo.Infrastructure.Public.Resources.PublicExceptionMessageResource", true);

        /// <summary>リソースの ResourceManager</summary>
        private static ResourceManager GetResourceManager()
        {
            return (ResourceManager)ResourceType.GetProperty(
                "ResourceManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).GetValue(null);
        }

        /// <summary>PublicExceptionMessage の公開プロパティ</summary>
        private static IEnumerable<PropertyInfo> MessageProperties()
        {
            return typeof(PublicExceptionMessage).GetProperties(BindingFlags.Static | BindingFlags.Public);
        }

        /// <summary>リソースのキー（カルチャを指定し、親へは辿らない）</summary>
        private static HashSet<string> ResourceKeys(CultureInfo culture)
        {
            HashSet<string> keys = new HashSet<string>();
            ResourceSet set = GetResourceManager().GetResourceSet(culture, true, false);
            foreach (DictionaryEntry e in set) { keys.Add((string)e.Key); }
            return keys;
        }

        /// <summary>**すべてのメッセージが、空でない文字列を返す**（対応するリソースがある）。</summary>
        [Test]
        public void AllMessages_AreNotEmpty()
        {
            int count = 0;
            foreach (PropertyInfo p in MessageProperties())
            {
                Assert.That((string)p.GetValue(null), Is.Not.Null.And.Not.Empty, p.Name);
                count++;
            }
            Assert.That(count, Is.GreaterThanOrEqualTo(45));
        }

        /// <summary>メッセージのプロパティ名は、どれもリソースのキーにある。</summary>
        [Test]
        public void AllMessages_HaveResourceKey()
        {
            HashSet<string> keys = ResourceKeys(CultureInfo.InvariantCulture);
            foreach (PropertyInfo p in MessageProperties())
            {
                Assert.That(keys, Does.Contain(p.Name), p.Name);
            }
        }

        /// <summary>
        /// **日本語（ja-JP）のリソースに、既定（英語）と同じキーがすべてある。**
        /// 欠けていると、日本語の環境だけ英語にフォールバックする。
        /// </summary>
        [Test]
        public void JaJP_HasAllKeys()
        {
            HashSet<string> neutral = ResourceKeys(CultureInfo.InvariantCulture);
            HashSet<string> ja = ResourceKeys(new CultureInfo("ja-JP"));

            Assert.That(ja, Is.EquivalentTo(neutral));
        }

        /// <summary>
        /// リソース クラス（自動生成）の各プロパティは、ResourceManager の値をそのまま返す。
        /// 自動生成の取りこぼし（resx と Designer.cs の食い違い）が無いことも、ここで分かる。
        /// </summary>
        [Test]
        public void ResourceClass_PropertiesMatchResourceManager()
        {
            ResourceManager rm = GetResourceManager();
            HashSet<string> keys = ResourceKeys(CultureInfo.InvariantCulture);
            int count = 0;

            foreach (PropertyInfo p in ResourceType.GetProperties(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (p.PropertyType != typeof(string)) { continue; }

                Assert.That((string)p.GetValue(null), Is.EqualTo(rm.GetString(p.Name)), p.Name);
                Assert.That(keys, Does.Contain(p.Name), p.Name);
                count++;
            }

            Assert.That(count, Is.EqualTo(keys.Count), "resx のキーと、生成されたプロパティの数");

            // Culture は既定で null（CurrentUICulture を使う）。設定して戻す。
            PropertyInfo culture = ResourceType.GetProperty("Culture", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(culture.GetValue(null), Is.Null);
            try
            {
                culture.SetValue(null, CultureInfo.InvariantCulture);
                Assert.That(culture.GetValue(null), Is.EqualTo(CultureInfo.InvariantCulture));
            }
            finally
            {
                culture.SetValue(null, null);
            }
        }

        /// <summary>COMMANDTIMEOUT_ERROR だけは、リソースの末尾に設定キー名を付ける。</summary>
        [Test]
        public void CommandTimeoutError_AppendsLiteral()
        {
            Assert.That(PublicExceptionMessage.COMMANDTIMEOUT_ERROR, Does.EndWith(PubLiteral.SQL_COMMANDTIMEOUT));
        }

#if NETCOREAPP
        #region .NET のみ（FxExceptionMessageCulture の切り替え）

        /// <summary>差し替える前の構成</summary>
        private IConfiguration _original;

        /// <summary>構成を退避する。</summary>
        [SetUp]
        public void SetUp()
        {
            this._original = GetConfigParameter.Configuration;
        }

        /// <summary>構成を戻す。</summary>
        [TearDown]
        public void TearDown()
        {
            GetConfigParameter.InitConfiguration(this._original);
        }

        /// <summary>FxExceptionMessageCulture を指定した構成に差し替える。</summary>
        private static void UseCulture(string culture)
        {
            GetConfigParameter.InitConfiguration(new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string> { { "appSettings:" + PubLiteral.EXCEPTIONMESSAGECULTUER, culture } }));
        }

        /// <summary>
        /// **FxExceptionMessageCulture で、メッセージの言語を固定できる。**
        /// 指定が無ければ CurrentUICulture（環境で変わるので、ここでは見ない）。
        /// net48 は App.config を実行中に変えられないため、.NET だけで確かめる。
        /// </summary>
        [Test]
        public void Culture_FromConfig()
        {
            UseCulture("ja-JP");
            Assert.That(PublicExceptionMessage.NO_CONFIG, Does.StartWith("コンフィグに指定がありません"));

            UseCulture("en-US");
            Assert.That(PublicExceptionMessage.NO_CONFIG, Does.StartWith("Config does not have specification"));
        }

        /// <summary>カルチャ名として不正なら、例外にせず、既定（CurrentUICulture）で返す。</summary>
        [Test]
        public void Culture_Invalid_FallsBackToDefault()
        {
            UseCulture("not a culture name!");

            Assert.That(PublicExceptionMessage.NO_CONFIG,
                Is.EqualTo(GetResourceManager().GetString("NO_CONFIG")));
        }

        /// <summary>
        /// **構成を初期化していなくても、メッセージは引ける**（例外を投げる最中に、例外にならない）。
        /// </summary>
        [Test]
        public void Culture_NotInitialized_FallsBackToDefault()
        {
            GetConfigParameter.InitConfiguration((IConfiguration)null);

            Assert.That(PublicExceptionMessage.NO_CONFIG,
                Is.EqualTo(GetResourceManager().GetString("NO_CONFIG")));
        }

        #endregion
#endif
    }
}

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
//* クラス名        ：TestSetup
//* クラス日本語名  ：テスト全体の前処理
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//*  2026/10/19  玄人 幸道         FxSqlTraceLog を ON にした（GetCurrentQueryForLog のテスト用）
//**********************************************************************************

using NUnit.Framework;

#if NETCOREAPP
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

using Touryo.Infrastructure.Public.Util;
#endif

namespace TestPublicDb
{
    /// <summary>テスト全体の前処理</summary>
    [SetUpFixture]
    public class TestSetup
    {
        /// <summary>全テストの前に 1 回だけ実行する。</summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
#if NETCOREAPP
            // .NET では InitConfiguration が要る。**値は App.config と同じものを与える。**
            // PostgreSQL（NPS）は DamPstGrS が .NET 専用なので、こちらにだけある。
            GetConfigParameter.InitConfiguration(
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        { "connectionStrings:ConnectionString_SQL", "Data Source=localhost;Initial Catalog=Northwind;User ID=sa;Password=seigi@123;Encrypt=false;" },
                        { "connectionStrings:ConnectionString_ODP", "User Id=SCOTT;Password=tiger;Data Source=localhost/XE;" },
                        { "connectionStrings:ConnectionString_MCN", "Server=localhost;Database=test;User Id=root;Password=seigi@123" },
                        { "connectionStrings:ConnectionString_NPS", "HOST=localhost;DATABASE=postgres;USER ID=postgres;PASSWORD=seigi@123;" },
                        { "appSettings:FxSqlTraceLog", "ON" },
                    })
                    .Build());
#endif
        }
    }
}

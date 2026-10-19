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
//*  2026/10/19  玄人 幸道         新規作成
//**********************************************************************************

using NUnit.Framework;

#if NETCOREAPP
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

using Touryo.Infrastructure.Public.Util;
#endif

namespace TestFramework
{
    /// <summary>テスト全体の前処理</summary>
    /// <remarks>
    /// 名前空間 TestFramework の直下に置くので、すべてのテストの前に 1 回だけ走る。
    /// </remarks>
    [SetUpFixture]
    public class TestSetup
    {
        /// <summary>全テストの前に 1 回だけ実行する。</summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
#if NETCOREAPP
            // **.NET では、設定を読む部品の前に InitConfiguration が要る**（TestPublic と同じ）。
            //   ここで扱う部品は、鍵や値を引数で受ける版だけなので、設定の値は与えない（空の構成）。
            //   設定を読む版（CmnClientParams 等）は、配備の設定に依るので対象外。
            GetConfigParameter.InitConfiguration(
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>())
                    .Build());
#endif
        }
    }
}

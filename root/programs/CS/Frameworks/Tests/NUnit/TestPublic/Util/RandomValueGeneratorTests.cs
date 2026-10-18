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
//* クラス名        ：RandomValueGeneratorTests
//* クラス日本語名  ：RandomValueGenerator の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/16  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System.Collections.Generic;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Util;

namespace TestPublic.Util
{
    /// <summary>RandomValueGenerator の単体テスト</summary>
    /// <remarks>
    /// 乱数なので、**偶然に落ちる確率が無視できる回数**で性質を見る
    /// （例: 200 回すべて同じ値になる確率は 2^-199）。
    /// </remarks>
    [TestFixture]
    public class RandomValueGeneratorTests
    {
        /// <summary>GenerateRandomUint は、毎回ちがう値を返す。</summary>
        [Test]
        public void GenerateRandomUint_Varies()
        {
            HashSet<uint> values = new HashSet<uint>();
            for (int i = 0; i < 20; i++) { values.Add(RandomValueGenerator.GenerateRandomUint()); }

            // 32 bit の値が 20 回で 2 回以上重なることは、まず無い。
            Assert.That(values.Count, Is.GreaterThanOrEqualTo(19));
        }

        /// <summary>TrueOrFalse は、両方の値を返す。</summary>
        [Test]
        public void TrueOrFalse_ReturnsBoth()
        {
            bool sawTrue = false, sawFalse = false;
            for (int i = 0; i < 200; i++)
            {
                if (RandomValueGenerator.TrueOrFalse()) { sawTrue = true; } else { sawFalse = true; }
            }

            Assert.That(sawTrue && sawFalse, Is.True);
        }

        /// <summary>
        /// IncrementProgressRate は、進捗率を (乱数 % maxProgressRate) ずつ増やす。
        /// **戻り値は上限で頭打ちになる**（LastProgressRate は上限を超えて増え続ける）。
        /// </summary>
        [Test]
        public void IncrementProgressRate()
        {
            RandomValueGenerator rvg = new RandomValueGenerator();
            Assert.That(rvg.LastProgressRate, Is.EqualTo(0u));

            uint prev = 0;
            uint ret = 0;
            for (int i = 0; i < 1000 && ret < 100; i++)
            {
                ret = rvg.IncrementProgressRate(10);

                Assert.That(rvg.LastProgressRate - prev, Is.LessThan(10u), "1 回の増分は max 未満");
                Assert.That(ret, Is.LessThanOrEqualTo(100u));
                prev = rvg.LastProgressRate;
            }

            // 1 回あたり平均 4.5 増えるので、1000 回あれば上限に届く。
            Assert.That(ret, Is.EqualTo(100u));
            Assert.That(rvg.LastProgressRate, Is.GreaterThanOrEqualTo(100u));

            // 上限を変えられる
            Assert.That(new RandomValueGenerator().IncrementProgressRate(10, 0), Is.EqualTo(0u));
        }
    }
}

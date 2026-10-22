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
//* クラス名        ：ExponentialBackoffTests
//* クラス日本語名  ：ExponentialBackoff（指数バックオフ）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/19  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using NUnit.Framework;

using Touryo.Infrastructure.Public.IO;

namespace TestPublic.IO
{
    /// <summary>ExponentialBackoff（指数バックオフ）の単体テスト</summary>
    /// <remarks>
    /// 間隔は「2 の n 乗 秒 ＋ 1 秒未満の乱数」（n はそれまでのリトライ回数）。値そのものは乱数を含むので範囲で見る。
    /// **Sleep は実際に待つ**ので、待つ回は最大間隔を 0 秒にして、待ち時間を 0 にする。
    /// </remarks>
    [TestFixture]
    public class ExponentialBackoffTests
    {
        /// <summary>
        /// 間隔は回ごとに倍になる（1 秒台 → 2 秒台 → 4 秒台）。最大リトライ回数に達すると -1（ループ終了）。
        /// </summary>
        [Test]
        public void Interval_DoublesUntilMaxRetry()
        {
            ExponentialBackoff eb = new ExponentialBackoff(4, 64);
            Assert.That(eb.GetSleepIntervalMSec(), Is.EqualTo(-1), "まだ計算していない");

            int i1 = eb.CalculateSleepIntervalMSec();
            Assert.That(i1, Is.InRange(1000, 1999));
            Assert.That(eb.GetSleepIntervalMSec(), Is.EqualTo(i1));

            Assert.That(eb.CalculateSleepIntervalMSec(), Is.InRange(2000, 2999));
            Assert.That(eb.CalculateSleepIntervalMSec(), Is.InRange(4000, 4999));

            // 4 回目で最大リトライ回数（4）に達する
            Assert.That(eb.CalculateSleepIntervalMSec(), Is.EqualTo(-1));
        }

        /// <summary>計算した間隔が最大間隔（既定は 32 秒）以上なら、最大間隔に抑える。</summary>
        [Test]
        public void Interval_CappedByMaximumBackoff()
        {
            // 最大 2 秒：1 回目は 1 秒台、2 回目（2 秒台）からは 2,000 ミリ秒に抑えられる
            ExponentialBackoff eb = new ExponentialBackoff(10, 2);
            Assert.That(eb.CalculateSleepIntervalMSec(), Is.InRange(1000, 1999));
            Assert.That(eb.CalculateSleepIntervalMSec(), Is.EqualTo(2000));
            Assert.That(eb.CalculateSleepIntervalMSec(), Is.EqualTo(2000));

            // 引数 1 つの版は、最大間隔 32 秒
            ExponentialBackoff def = new ExponentialBackoff(10);
            for (int i = 0; i < 6; i++) { def.CalculateSleepIntervalMSec(); }
            Assert.That(def.GetSleepIntervalMSec(), Is.EqualTo(32000), "2 の 5 乗 秒（32 秒台）で抑えられる");
        }

        /// <summary>Sleep は、続けるなら待って true、最大リトライ回数に達したら待たずに false。</summary>
        [Test]
        public void Sleep_ContinueThenStop()
        {
            // 最大間隔 0 秒：待ち時間は 0
            ExponentialBackoff eb = new ExponentialBackoff(2, 0);
            Assert.That(eb.Sleep(), Is.True);
            Assert.That(eb.GetSleepIntervalMSec(), Is.EqualTo(0));
            Assert.That(eb.Sleep(), Is.False);
        }
    }
}

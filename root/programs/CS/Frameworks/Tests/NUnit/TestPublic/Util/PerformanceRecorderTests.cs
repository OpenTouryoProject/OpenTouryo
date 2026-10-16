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
//* クラス名        ：PerformanceRecorderTests
//* クラス日本語名  ：PerformanceRecorder の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/16  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System.Threading;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Util;

namespace TestPublic.Util
{
    /// <summary>PerformanceRecorder の単体テスト</summary>
    /// <remarks>
    /// **TFM で測るものが違う。**
    ///   ・net48 … 実行時間（QueryPerformanceCounter）と、CPU 時間（GetThreadTimes）
    ///   ・.NET  … 実行時間（Stopwatch）だけ。CPU 時間は "-" と出す
    /// 時間そのものは環境で変わるので、形と下限だけを見る。
    /// </remarks>
    [TestFixture]
    public class PerformanceRecorderTests
    {
        /// <summary>開始・終了で、ミリ秒の文字列が得られる。待った時間より短くはならない。</summary>
        [Test]
        public void StartAndEnd()
        {
            PerformanceRecorder pr = new PerformanceRecorder();

            Assert.That(pr.StartsPerformanceRecord(), Is.True);
            Thread.Sleep(50);
            string result = pr.EndsPerformanceRecord();

#if NETCOREAPP
            Assert.That(result, Does.Match(@"^ExT:\d+\[msec\], CT: - \[msec\], KT: - \[msec\], UT: - \[msec\]$"));
            Assert.That(pr.CpuTime, Is.Empty);
#else
            Assert.That(result, Does.Match(@"^ExT:\d+\[msec\], CT:\d+\[msec\], KT:\d+\[msec\], UT:\d+\[msec\]$"));
            Assert.That(long.Parse(pr.CpuTime),
                Is.EqualTo(long.Parse(pr.CpuKernelTime) + long.Parse(pr.CpuUserTime)).Within(1)); // 丸めの差
#endif
            // 待った分（50 ms）を下回らない（タイマの粒度の分だけ余裕を見る）
            Assert.That(long.Parse(pr.ExecTime), Is.GreaterThanOrEqualTo(40));
        }

        /// <summary>同じインスタンスで測り直せる。</summary>
        [Test]
        public void Restart()
        {
            PerformanceRecorder pr = new PerformanceRecorder();

            pr.StartsPerformanceRecord();
            pr.EndsPerformanceRecord();

            Assert.That(pr.StartsPerformanceRecord(), Is.True);
            Assert.That(pr.EndsPerformanceRecord(), Does.StartWith("ExT:"));
        }
    }
}

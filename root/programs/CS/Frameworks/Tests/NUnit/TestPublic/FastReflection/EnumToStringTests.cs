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
//* クラス名        ：EnumToStringTests
//* クラス日本語名  ：列挙体の高速な文字列化（Emit 版・式木版）の単体テスト
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

using Touryo.Infrastructure.Public.FastReflection;

namespace TestPublic.FastReflection
{
    /// <summary>0 から連番の列挙体（Emit 版は switch で引く）</summary>
    public enum Sequential { Zero, One, Two }

    /// <summary>連番でない列挙体（Emit 版は比較を連ねて引く）</summary>
    public enum Sparse { Ten = 10, Twenty = 20 }

    /// <summary>フラグ</summary>
    [Flags]
    public enum Flag { None = 0, A = 1, B = 2, C = 4 }

    /// <summary>列挙体の高速な文字列化（Emit 版）の単体テスト</summary>
    /// <remarks>
    /// Flags の列挙体は、Emit せずに ToString の結果から空白を除いて返す（"A,C"）。
    /// 未定義の値は InvalidOperationException。
    /// </remarks>
    [TestFixture]
    public class EnumToStringByEmitTests
    {
        /// <summary>連番・非連番のどちらでも、Enum.ToString と同じ名前を返す。</summary>
        [Test]
        public void DefinedValues()
        {
            foreach (Sequential v in Enum.GetValues(typeof(Sequential)))
            {
                Assert.That(v.ToStringByEmit(), Is.EqualTo(v.ToString()), v.ToString());
            }
            foreach (Sparse v in Enum.GetValues(typeof(Sparse)))
            {
                Assert.That(v.ToStringByEmit(), Is.EqualTo(v.ToString()), v.ToString());
            }
        }

        /// <summary>Nullable は、値があれば名前、null なら空文字。</summary>
        [Test]
        public void Nullable()
        {
            Sparse? v = Sparse.Twenty;
            Sparse? n = null;

            Assert.That(v.ToStringByEmit(), Is.EqualTo("Twenty"));
            Assert.That(n.ToStringByEmit(), Is.EqualTo(""));
        }

        /// <summary>列挙体でない struct は ArgumentException。</summary>
        [Test]
        public void NotEnum_Throws()
        {
            Assert.Throws<ArgumentException>(() => 1.ToStringByEmit());
        }

        /// <summary>Flags は空白なしで連結する。単独と 0 は名前。</summary>
        [Test]
        public void Flags()
        {
            Assert.That((Flag.A | Flag.C).ToStringByEmit(), Is.EqualTo("A,C"));
            Assert.That(Flag.B.ToStringByEmit(), Is.EqualTo("B"));
            Assert.That(Flag.None.ToStringByEmit(), Is.EqualTo("None"));
        }

        /// <summary>未定義の値は InvalidOperationException（連番の範囲外・非連番の隙間）。</summary>
        [Test]
        public void UndefinedValue_Throws()
        {
            Assert.Throws<InvalidOperationException>(() => ((Sequential)99).ToStringByEmit());
            Assert.Throws<InvalidOperationException>(() => ((Sequential)(-1)).ToStringByEmit());
            Assert.Throws<InvalidOperationException>(() => ((Sparse)15).ToStringByEmit());
        }
    }

#if NETCOREAPP
    /// <summary>列挙体の高速な文字列化（式木版）の単体テスト</summary>
    /// <remarks>
    /// **式木版は .NET だけ**（Public_net48.csproj に含まれていない。Golden の TestCode も同じ扱い）。
    /// **Emit 版とは境界の振る舞いが違う。**
    ///   ・Flags の区切り … Emit は "A,C"、式木は "A, C"
    ///   ・未定義の値     … Emit は例外、式木は空文字（0 なら "0"）
    /// </remarks>
    [TestFixture]
    public class EnumToStringByExpressionTreeTests
    {
        /// <summary>定義された値は Enum.ToString と同じ名前を返す。</summary>
        [Test]
        public void DefinedValues()
        {
            foreach (Sequential v in Enum.GetValues(typeof(Sequential)))
            {
                Assert.That(v.ToStringByExpressionTree(), Is.EqualTo(v.ToString()), v.ToString());
            }
            foreach (Sparse v in Enum.GetValues(typeof(Sparse)))
            {
                Assert.That(v.ToStringByExpressionTree(), Is.EqualTo(v.ToString()), v.ToString());
            }
        }

        /// <summary>Nullable は、値があれば名前、null なら空文字。</summary>
        [Test]
        public void Nullable()
        {
            Sparse? v = Sparse.Twenty;
            Sparse? n = null;

            Assert.That(v.ToStringByExpressionTree(), Is.EqualTo("Twenty"));
            Assert.That(n.ToStringByExpressionTree(), Is.EqualTo(""));
        }

        /// <summary>列挙体でない struct は ArgumentException。</summary>
        [Test]
        public void NotEnum_Throws()
        {
            Assert.Throws<ArgumentException>(() => 1.ToStringByExpressionTree());
        }

        /// <summary>**Flags は ", " で連結する**（Emit 版と違う）。単独と 0 は名前。</summary>
        [Test]
        public void Flags()
        {
            Assert.That((Flag.A | Flag.C).ToStringByExpressionTree(), Is.EqualTo("A, C"));
            Assert.That(Flag.B.ToStringByExpressionTree(), Is.EqualTo("B"));
            Assert.That(Flag.None.ToStringByExpressionTree(), Is.EqualTo("None"));
        }

        /// <summary>**未定義の値は例外にせず空文字**（Emit 版と違う）。0 を定義していなければ "0"。</summary>
        [Test]
        public void UndefinedValue()
        {
            Assert.That(((Sequential)99).ToStringByExpressionTree(), Is.EqualTo(""));
            Assert.That(((Sparse)15).ToStringByExpressionTree(), Is.EqualTo(""));
            Assert.That(default(Sparse).ToStringByExpressionTree(), Is.EqualTo("0"));
        }
    }
#endif
}

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
//* クラス名        ：LatebindTests
//* クラス日本語名  ：Latebind の単体テスト
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

using Touryo.Infrastructure.Public.Reflection;

namespace TestPublic.Reflection
{
    /// <summary>遅延バインドで呼ばれるクラス</summary>
    public class LatebindTarget
    {
        /// <summary>public のメソッド</summary>
        public string Hello(string name) { return "Hello " + name; }

        /// <summary>オーバーロード（引数の数で選ばれる）</summary>
        public int Add(int a, int b) { return a + b; }

        /// <summary>オーバーロード（引数の数で選ばれる）</summary>
        public int Add(int a, int b, int c) { return a + b + c; }

        /// <summary>非 public のメソッドも呼べる</summary>
        private string Secret() { return "secret"; }
    }

    /// <summary>LatebindTarget の派生</summary>
    public class LatebindDerived : LatebindTarget { }

    /// <summary>Latebind の単体テスト</summary>
    [TestFixture]
    public class LatebindTests
    {
        /// <summary>テスト アセンブリの名前（Assembly.Load に渡す）</summary>
        private static readonly string ThisAssembly = typeof(LatebindTests).Assembly.GetName().Name;
        private const string TargetClass = "TestPublic.Reflection.LatebindTarget";

        #region アセンブリ名・クラス名で呼ぶ

        /// <summary>アセンブリ名・クラス名・メソッド名で呼ぶ（インスタンスは都度作られる）。</summary>
        [Test]
        public void InvokeMethod_ByName()
        {
            Assert.That(Latebind.InvokeMethod(ThisAssembly, TargetClass, "Hello", new object[] { "A" }),
                Is.EqualTo("Hello A"));
            Assert.That(Latebind.InvokeMethod(ThisAssembly, TargetClass, "Secret", new object[] { }),
                Is.EqualTo("secret"));
        }

        /// <summary>
        /// **オーバーロードは、引数の数で選ぶ。**
        /// 名前だけでは決まらない（AmbiguousMatchException）ため、数の一致するものを呼ぶ。
        /// </summary>
        [Test]
        public void InvokeMethod_Overload()
        {
            Assert.That(Latebind.InvokeMethod(ThisAssembly, TargetClass, "Add", new object[] { 1, 2 }), Is.EqualTo(3));
            Assert.That(Latebind.InvokeMethod(ThisAssembly, TargetClass, "Add", new object[] { 1, 2, 3 }), Is.EqualTo(6));
        }

        /// <summary>アセンブリ・クラス・メソッドが無ければ ArgumentException。</summary>
        [Test]
        public void InvokeMethod_NotFound_Throws()
        {
            Assert.Throws<ArgumentException>(() => Latebind.InvokeMethod("No.Such.Assembly", TargetClass, "Hello", new object[] { "A" }));
            Assert.Throws<ArgumentException>(() => Latebind.InvokeMethod(ThisAssembly, "TestPublic.NoSuchClass", "Hello", new object[] { "A" }));
            Assert.Throws<ArgumentException>(() => Latebind.InvokeMethod(ThisAssembly, TargetClass, "NoSuchMethod", new object[] { }));
            Assert.Throws<ArgumentException>(() => Latebind.InvokeMethod(ThisAssembly, TargetClass, "Add", new object[] { 1 }));
        }

        /// <summary>_NoErr は、見つからないときに例外ではなく null を返す。</summary>
        [Test]
        public void InvokeMethod_NoErr()
        {
            Assert.That(Latebind.InvokeMethod_NoErr(ThisAssembly, TargetClass, "NoSuchMethod", new object[] { }), Is.Null);
            Assert.That(Latebind.InvokeMethod_NoErr(ThisAssembly, TargetClass, "Hello", new object[] { "B" }), Is.EqualTo("Hello B"));
        }

        #endregion

        #region インスタンスで呼ぶ

        /// <summary>既存のインスタンスに対して呼ぶ。</summary>
        [Test]
        public void InvokeMethod_ByInstance()
        {
            LatebindTarget target = new LatebindTarget();

            Assert.That(Latebind.InvokeMethod(target, "Hello", new object[] { "C" }), Is.EqualTo("Hello C"));
            Assert.That(Latebind.InvokeMethod(target, "Add", new object[] { 1, 2, 3 }), Is.EqualTo(6));
            Assert.Throws<ArgumentException>(() => Latebind.InvokeMethod(target, "NoSuchMethod", new object[] { }));
            Assert.That(Latebind.InvokeMethod_NoErr(target, "NoSuchMethod", new object[] { }), Is.Null);
        }

        #endregion

        #region 型の確認

        /// <summary>CheckTypeOfBaseClass は継承階層を遡って調べる（自分自身は含まない）。</summary>
        [Test]
        public void CheckTypeOfBaseClass()
        {
            Assert.That(Latebind.CheckTypeOfBaseClass(typeof(LatebindDerived), typeof(LatebindTarget)), Is.True);
            Assert.That(Latebind.CheckTypeOfBaseClass(typeof(LatebindDerived), typeof(object)), Is.True);
            Assert.That(Latebind.CheckTypeOfBaseClass(typeof(LatebindTarget), typeof(LatebindTarget)), Is.False);
            Assert.That(Latebind.CheckTypeOfBaseClass(typeof(LatebindTarget), typeof(string)), Is.False);
        }

        /// <summary>CheckTypeOfMethodByName は非 public も含めて名前で探す。</summary>
        [Test]
        public void CheckTypeOfMethodByName()
        {
            LatebindTarget target = new LatebindTarget();

            Assert.That(Latebind.CheckTypeOfMethodByName(target, "Secret"), Is.True);
            Assert.That(Latebind.CheckTypeOfMethodByName(target, "NoSuchMethod"), Is.False);
        }

        #endregion
    }
}

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
//* クラス名        ：FastReflectionTests
//* クラス日本語名  ：AccessorCacher / CompiledExpressionCreater / InstanceCreator の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//*  2026/10/18  玄人 幸道         Setter が構造体のボックスの中身に代入するケースを追加（#620）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Touryo.Infrastructure.Public.FastReflection;

namespace TestPublic.FastReflection
{
    /// <summary>アクセサを取る対象</summary>
    public class AccessorTarget
    {
        /// <summary>読み書きできるプロパティ</summary>
        public int Id { get; set; }

        /// <summary>Nullable のプロパティ</summary>
        public decimal? Price { get; set; }

        /// <summary>読み取り専用のプロパティ</summary>
        public string ReadOnly { get { return "ro"; } }

        /// <summary>public フィールド</summary>
        public string Name;
    }

    /// <summary>AccessorCacher / CompiledExpressionCreater / InstanceCreator の単体テスト</summary>
    [TestFixture]
    public class FastReflectionTests
    {
        #region CompiledExpressionCreater

        /// <summary>プロパティ・フィールドの getter / setter を作る。</summary>
        [Test]
        public void GetterAndSetter()
        {
            AccessorTarget t = new AccessorTarget();

            CompiledExpressionCreater.CreateSetterOfPropertyOrField(typeof(AccessorTarget), "Id")(t, 7);
            CompiledExpressionCreater.CreateSetterOfPropertyOrField(typeof(AccessorTarget), "Name")(t, "n");

            Assert.That(t.Id, Is.EqualTo(7));
            Assert.That(t.Name, Is.EqualTo("n"));
            Assert.That(CompiledExpressionCreater.CreateGetterOfPropertyOrField(typeof(AccessorTarget), "Id")(t), Is.EqualTo(7));
            Assert.That(CompiledExpressionCreater.CreateGetterOfPropertyOrField(typeof(AccessorTarget), "Name")(t), Is.EqualTo("n"));
        }

        /// <summary>Nullable には null も値も設定できる。</summary>
        [Test]
        public void Setter_Nullable()
        {
            AccessorTarget t = new AccessorTarget();
            Action<object, object> set = CompiledExpressionCreater.CreateSetterOfPropertyOrField(typeof(AccessorTarget), "Price");

            set(t, 1.5m);
            Assert.That(t.Price, Is.EqualTo(1.5m));

            set(t, null);
            Assert.That(t.Price, Is.Null);
        }

        /// <summary>
        /// **setter は型を変換しない**（Convert はキャストであって型変換ではない）。
        /// int のプロパティに long を渡すと InvalidCastException。
        /// </summary>
        [Test]
        public void Setter_WrongType_Throws()
        {
            Action<object, object> set = CompiledExpressionCreater.CreateSetterOfPropertyOrField(typeof(AccessorTarget), "Id");

            Assert.Throws<InvalidCastException>(() => set(new AccessorTarget(), 7L));
        }

        /// <summary>Setter のテストに使う構造体</summary>
        public struct StructTarget
        {
            /// <summary>Id（フィールド）</summary>
            public int Id;

            /// <summary>Name（プロパティ）</summary>
            public string Name { get; set; }
        }

        /// <summary>
        /// **値型（構造体）でも、渡したボックスの中身に代入する**（フィールドにもプロパティにも）。
        /// 2026/10/18 まで、アンボックス化したコピーに代入され、ボックスの中身は変わらなかった（#620）。
        /// </summary>
        [Test]
        public void Setter_Struct_WritesIntoBox()
        {
            Action<object, object> setId = CompiledExpressionCreater.CreateSetterOfPropertyOrField(typeof(StructTarget), "Id");
            Action<object, object> setName = CompiledExpressionCreater.CreateSetterOfPropertyOrField(typeof(StructTarget), "Name");

            object boxed = new StructTarget();
            setId(boxed, 5);
            setName(boxed, "s");

            StructTarget t = (StructTarget)boxed;
            Assert.That(t.Id, Is.EqualTo(5));
            Assert.That(t.Name, Is.EqualTo("s"));
        }

        /// <summary>存在しないメンバは、デリゲートを作る時点で ArgumentException。</summary>
        [Test]
        public void UnknownMember_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                CompiledExpressionCreater.CreateGetterOfPropertyOrField(typeof(AccessorTarget), "NoSuchMember"));
        }

        #endregion

        #region AccessorCacher

        /// <summary>
        /// 型ごとに、public のプロパティとフィールドのアクセサを 1 回だけ作ってキャッシュする。
        /// 読み取り専用のプロパティは SetDelegate が null。
        /// </summary>
        [Test]
        public void CacheAccessor()
        {
            AccessorTarget t = new AccessorTarget() { Id = 3, Price = 9.9m, Name = "x" };
            AccessorCacher.CacheAccessor(t);

            List<AccessorInfo> infos = AccessorCacher.CncDic[typeof(AccessorTarget)];
            Assert.That(infos.Select(x => x.AccessorName), Is.EquivalentTo(new[] { "Id", "Price", "ReadOnly", "Name" }));

            AccessorInfo id = infos.Single(x => x.AccessorName == "Id");
            Assert.That(id.AccessorType, Is.EqualTo(typeof(int)));
            Assert.That(id.UnderlyingType, Is.Null);
            Assert.That(id.GetDelegate(t), Is.EqualTo(3));

            AccessorInfo price = infos.Single(x => x.AccessorName == "Price");
            Assert.That(price.UnderlyingType, Is.EqualTo(typeof(decimal)));

            AccessorInfo ro = infos.Single(x => x.AccessorName == "ReadOnly");
            Assert.That(ro.SetDelegate, Is.Null);
            Assert.That(ro.GetDelegate(t), Is.EqualTo("ro"));

            // 2 回目は作り直さない（同じリストのまま）
            AccessorCacher.CacheAccessor(new AccessorTarget());
            Assert.That(AccessorCacher.CncDic[typeof(AccessorTarget)], Is.SameAs(infos));
        }

        #endregion

        #region InstanceCreator

        /// <summary>Factory は毎回新しいインスタンスを返す。</summary>
        [Test]
        public void InstanceCreator_Factory()
        {
            AccessorTarget a = InstanceCreator<AccessorTarget>.Factory();
            AccessorTarget b = InstanceCreator<AccessorTarget>.Factory();

            Assert.That(a, Is.Not.Null);
            Assert.That(a, Is.Not.SameAs(b));
            Assert.That(InstanceCreator<List<int>>.Factory().Count, Is.EqualTo(0));
        }

        #endregion
    }
}

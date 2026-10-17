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
//* クラス名        ：PocoToPocoTests
//* クラス日本語名  ：PocoToPoco（POCO → POCO の項目移送）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/17  玄人 幸道         新規作成（カバレッジの拡充）
//*  2026/10/17  玄人 幸道         公開の set / get を持たないプロパティのケースを追加（#618）
//*  2026/10/18  玄人 幸道         同名で型の違う項目のケースを追加（#619）
//*  2026/10/18  玄人 幸道         構造体のケースを追加（#620）
//*  2026/10/18  玄人 幸道         移送元が null のケースを追加（#621）
//**********************************************************************************

using System;
using System.Collections.Generic;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Dto;

namespace TestPublic.Dto
{
    /// <summary>PocoToPoco（POCO → POCO の項目移送）の単体テスト</summary>
    /// <remarks>
    /// 同名のプロパティ・フィールドを移送する。後退テストは Golden の TestCode（TestDto）にもある。
    /// </remarks>
    [TestFixture]
    public class PocoToPocoTests
    {
        #region 準備

        /// <summary>移送元</summary>
        public class OrderSource
        {
            /// <summary>OrderID</summary>
            public int OrderID { get; set; }

            /// <summary>Note</summary>
            public string Note { get; set; }

            /// <summary>Memo（移送先には同名が無い）</summary>
            public string Memo { get; set; }

            /// <summary>Code（フィールド）</summary>
            public string Code;
        }

        /// <summary>移送先</summary>
        public class OrderDestination
        {
            /// <summary>OrderID</summary>
            public int OrderID { get; set; }

            /// <summary>Note</summary>
            public string Note { get; set; }

            /// <summary>Qty（移送元には同名が無い）</summary>
            public int Qty { get; set; }

            /// <summary>Code（フィールド）</summary>
            public string Code;
        }

        /// <summary>移送元を作る。</summary>
        private static OrderSource CreateSource()
        {
            return new OrderSource() { OrderID = 3, Note = "z", Memo = "m", Code = "C3" };
        }

        #endregion

        /// <summary>同名のプロパティとフィールドだけを移送する。同名が無いものは既定値のまま。</summary>
        [Test]
        public void Map_SameNameOnly()
        {
            OrderDestination dst = PocoToPoco.Map<OrderSource, OrderDestination>(CreateSource());

            Assert.That(dst.OrderID, Is.EqualTo(3));
            Assert.That(dst.Note, Is.EqualTo("z"));
            Assert.That(dst.Code, Is.EqualTo("C3"));
            Assert.That(dst.Qty, Is.EqualTo(0));
        }

        /// <summary>
        /// マップの向きは「移送先の名前 → 移送元の名前」。
        /// マップに無い名前は同名で移送する（2018/12/11 の修正。以前はマップ有りで同名が移送されなかった）。
        /// </summary>
        [Test]
        public void Map_WithMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("Qty", "OrderID");

            OrderDestination dst = PocoToPoco.Map<OrderSource, OrderDestination>(CreateSource(), map);

            Assert.That(dst.Qty, Is.EqualTo(3));
            Assert.That(dst.OrderID, Is.EqualTo(3));
            Assert.That(dst.Note, Is.EqualTo("z"));
        }

        /// <summary>マップの移送元の名前が存在しなければ、その項目は既定値のまま（例外にならない）。</summary>
        [Test]
        public void Map_UnknownSourceName_NotMapped()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("Qty", "NoSuchMember");

            OrderDestination dst = PocoToPoco.Map<OrderSource, OrderDestination>(CreateSource(), map);

            Assert.That(dst.Qty, Is.EqualTo(0));
            Assert.That(dst.OrderID, Is.EqualTo(3));
        }

        /// <summary>
        /// 移送先のインスタンスを渡すと、そこへ上書きして同じインスタンスを返す。
        /// 移送元に同名が無い項目は、渡したときの値が残る。
        /// </summary>
        [Test]
        public void Map_IntoExistingInstance()
        {
            OrderDestination existing = new OrderDestination() { OrderID = 99, Qty = 7 };

            OrderDestination dst = PocoToPoco.Map<OrderSource, OrderDestination>(CreateSource(), existing);

            Assert.That(dst, Is.SameAs(existing));
            Assert.That(dst.OrderID, Is.EqualTo(3));
            Assert.That(dst.Qty, Is.EqualTo(7));
        }

        /// <summary>移送先に null を渡すと、新しいインスタンスを作る。</summary>
        [Test]
        public void Map_NullDestination_CreatesInstance()
        {
            OrderDestination dst = PocoToPoco.Map<OrderSource, OrderDestination>(CreateSource(), (OrderDestination)null, null);

            Assert.That(dst, Is.Not.Null);
            Assert.That(dst.OrderID, Is.EqualTo(3));
        }

        /// <summary>移送は値のコピーで、移送後に移送元を変えても移送先は変わらない（参照型は参照のコピー）。</summary>
        [Test]
        public void Map_CopiesValues()
        {
            OrderSource src = CreateSource();
            OrderDestination dst = PocoToPoco.Map<OrderSource, OrderDestination>(src);

            src.OrderID = 100;
            src.Note = "changed";

            Assert.That(dst.OrderID, Is.EqualTo(3));
            Assert.That(dst.Note, Is.EqualTo("z"));
        }

        #region 公開の set / get を持たないプロパティ（#618）

        /// <summary>移送先：get だけと private set のプロパティを持つ</summary>
        public class NoSetterDestination
        {
            /// <summary>OrderID（get だけ）</summary>
            public int OrderID { get { return -1; } }

            /// <summary>Note（private set）</summary>
            public string Note { get; private set; }

            /// <summary>Code（フィールド）</summary>
            public string Code;
        }

        /// <summary>移送元：set だけのプロパティを持つ</summary>
        public class NoGetterSource
        {
            /// <summary>OrderID の実体</summary>
            private int _orderID = 0;

            /// <summary>OrderID（set だけ）</summary>
            public int OrderID { set { this._orderID = value; } }

            /// <summary>OrderID の値（プロパティではないので移送の対象にならない）</summary>
            public int GetOrderID() { return this._orderID; }

            /// <summary>Note</summary>
            public string Note { get; set; }
        }

        /// <summary>
        /// **公開の set を持たない移送先のプロパティは、移送の対象から外す**（private set を含む）。他の項目は移送する。
        /// 2026/10/17 まで NullReferenceException になっていた（#618）。
        /// </summary>
        [Test]
        public void NoPublicSetterDestination_Skipped()
        {
            NoSetterDestination dst = PocoToPoco.Map<OrderSource, NoSetterDestination>(CreateSource());

            Assert.That(dst.OrderID, Is.EqualTo(-1));
            Assert.That(dst.Note, Is.Null);
            Assert.That(dst.Code, Is.EqualTo("C3"));
        }

        /// <summary>
        /// **公開の get を持たない移送元のプロパティは、値を取れないので飛ばす。** 移送先は既定値のまま。
        /// 2026/10/17 まで NullReferenceException になっていた（#618）。
        /// </summary>
        [Test]
        public void NoPublicGetterSource_Skipped()
        {
            NoGetterSource src = new NoGetterSource() { OrderID = 5, Note = "n" };

            OrderDestination dst = PocoToPoco.Map<NoGetterSource, OrderDestination>(src);

            Assert.That(dst.OrderID, Is.EqualTo(0));
            Assert.That(dst.Note, Is.EqualTo("n"));
        }

        #endregion

        #region 同名で型の違う項目（#619）

        /// <summary>列挙型</summary>
        public enum Kind
        {
            /// <summary>A</summary>
            A = 0,
            /// <summary>B</summary>
            B = 1
        }

        /// <summary>移送元（型の違う項目を持つ）</summary>
        public class TypedSource
        {
            /// <summary>int → long</summary>
            public int ToLong { get; set; }
            /// <summary>int → string</summary>
            public int ToStr { get; set; }
            /// <summary>string → int</summary>
            public string StrToInt { get; set; }
            /// <summary>int → int?</summary>
            public int ToNullable { get; set; }
            /// <summary>int? → int（値あり）</summary>
            public int? NullableToInt { get; set; }
            /// <summary>int? → int（null）</summary>
            public int? NullToInt { get; set; }
            /// <summary>int → long?</summary>
            public int ToNullableLong { get; set; }
        }

        /// <summary>移送先</summary>
        public class TypedDestination
        {
            /// <summary>int → long</summary>
            public long ToLong { get; set; }
            /// <summary>int → string</summary>
            public string ToStr { get; set; }
            /// <summary>string → int</summary>
            public int StrToInt { get; set; }
            /// <summary>int → int?</summary>
            public int? ToNullable { get; set; }
            /// <summary>int? → int（値あり）</summary>
            public int NullableToInt { get; set; }
            /// <summary>int? → int（null）</summary>
            public int NullToInt { get; set; } = -1;
            /// <summary>int → long?</summary>
            public long? ToNullableLong { get; set; }
        }

        /// <summary>移送元（キャストで済む項目だけ。以前から移送できていたもの）</summary>
        public class AssignableSource
        {
            /// <summary>int → int?</summary>
            public int ToNullable { get; set; }
            /// <summary>int → 列挙型</summary>
            public int ToEnum { get; set; }
            /// <summary>列挙型 → int</summary>
            public Kind EnumToInt { get; set; }
            /// <summary>派生型 → インタフェイス</summary>
            public List<string> ToEnumerable { get; set; }
        }

        /// <summary>移送先（キャストで済む項目だけ）</summary>
        public class AssignableDestination
        {
            /// <summary>int → int?</summary>
            public int? ToNullable { get; set; }
            /// <summary>int → 列挙型</summary>
            public Kind ToEnum { get; set; }
            /// <summary>列挙型 → int</summary>
            public int EnumToInt { get; set; }
            /// <summary>派生型 → インタフェイス</summary>
            public IEnumerable<string> ToEnumerable { get; set; }
        }

        /// <summary>移送元を作る。</summary>
        private static TypedSource CreateTypedSource()
        {
            return new TypedSource()
            {
                ToLong = 5, ToStr = 6, StrToInt = "12", ToNullable = 7, NullableToInt = 8, NullToInt = null,
                ToNullableLong = 9
            };
        }

        /// <summary>
        /// **同名で型が違えば、DataToPoco と同じく変換して移送する**（int → long / int → string / string → int / int → long?）。
        /// 2026/10/18 まで InvalidCastException になっていた（#619）。
        /// </summary>
        [Test]
        [SetCulture("ja-JP")]
        public void DifferentType_Converted()
        {
            TypedDestination dst = PocoToPoco.Map<TypedSource, TypedDestination>(CreateTypedSource());

            Assert.That(dst.ToLong, Is.EqualTo(5L));
            Assert.That(dst.ToStr, Is.EqualTo("6"));
            Assert.That(dst.StrToInt, Is.EqualTo(12));
            Assert.That(dst.ToNullableLong, Is.EqualTo(9L));
            Assert.That(dst.NullableToInt, Is.EqualTo(8));
        }

        /// <summary>
        /// キャストで済む組み合わせは、以前と同じくそのまま移送する
        /// （T → Nullable(T)・整数 ⇔ 列挙型・派生型 → インタフェイス。参照はそのまま）。
        /// #619 の前の DLL でも通ることを確かめてある（変換を足して壊していないことの確認）。
        /// </summary>
        [Test]
        public void AssignableType_AsBefore()
        {
            AssignableSource src = new AssignableSource()
            {
                ToNullable = 7, ToEnum = 1, EnumToInt = Kind.B, ToEnumerable = new List<string>() { "a" }
            };
            AssignableDestination dst = PocoToPoco.Map<AssignableSource, AssignableDestination>(src);

            Assert.That(dst.ToNullable, Is.EqualTo(7));
            Assert.That(dst.ToEnum, Is.EqualTo(Kind.B));
            Assert.That(dst.EnumToInt, Is.EqualTo(1));
            Assert.That(dst.ToEnumerable, Is.SameAs(src.ToEnumerable));
        }

        /// <summary>
        /// null は、Nullable でない値型には入らない。**既定値のまま**（DataToPoco の DBNull と同じ）。
        /// 2026/10/18 まで NullReferenceException になっていた（#619）。
        /// </summary>
        [Test]
        public void NullToValueType_LeavesValue()
        {
            TypedDestination dst = PocoToPoco.Map<TypedSource, TypedDestination>(CreateTypedSource());

            Assert.That(dst.NullToInt, Is.EqualTo(-1));
        }

        /// <summary>変換できない値は、DataToPoco と同じく FormatException（どの値かは例外で分かる）。</summary>
        [Test]
        public void NotConvertible_Throws()
        {
            TypedSource src = CreateTypedSource();
            src.StrToInt = "x";

            Assert.Throws<FormatException>(() => PocoToPoco.Map<TypedSource, TypedDestination>(src));
        }

        #endregion

        #region 構造体（#620）

        /// <summary>構造体（フィールドとプロパティ）</summary>
        public struct OrderStruct
        {
            /// <summary>OrderID（フィールド）</summary>
            public int OrderID;

            /// <summary>Note（プロパティ）</summary>
            public string Note { get; set; }
        }

        /// <summary>
        /// **移送先が構造体でも値が入る。** 移送先のインスタンスを渡した場合も、値の入った構造体を返す。
        /// 2026/10/18 まで、例外にならずに値が入らなかった（#620。ボックス化したコピーに値を入れていた）。
        /// </summary>
        [Test]
        public void StructDestination_Mapped()
        {
            OrderStruct dst = PocoToPoco.Map<OrderSource, OrderStruct>(CreateSource());
            Assert.That(dst.OrderID, Is.EqualTo(3));
            Assert.That(dst.Note, Is.EqualTo("z"));

            OrderStruct given = new OrderStruct() { OrderID = 99 };
            OrderStruct dst2 = PocoToPoco.Map<OrderSource, OrderStruct>(CreateSource(), given);
            Assert.That(dst2.OrderID, Is.EqualTo(3));
            Assert.That(given.OrderID, Is.EqualTo(99), "値渡しなので、渡した変数は変わらない");
        }

        /// <summary>
        /// **移送元が null なら ArgumentNullException**（引数名は src）。どのオーバーロードでも同じ。
        /// 2026/10/18 まで NullReferenceException だった（#621）。
        /// </summary>
        [Test]
        public void NullSource_ThrowsArgumentNull()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            Assert.That(Assert.Throws<ArgumentNullException>(() =>
                PocoToPoco.Map<OrderSource, OrderDestination>(null)).ParamName, Is.EqualTo("src"));
            Assert.That(Assert.Throws<ArgumentNullException>(() =>
                PocoToPoco.Map<OrderSource, OrderDestination>(null, new OrderDestination())).ParamName, Is.EqualTo("src"));
            Assert.That(Assert.Throws<ArgumentNullException>(() =>
                PocoToPoco.Map<OrderSource, OrderDestination>(null, map)).ParamName, Is.EqualTo("src"));
            Assert.That(Assert.Throws<ArgumentNullException>(() =>
                PocoToPoco.Map<OrderSource, OrderDestination>(null, new OrderDestination(), map)).ParamName, Is.EqualTo("src"));
        }

        /// <summary>移送元が構造体でも移送できる（読むだけなので、以前から移送できていた）。</summary>
        [Test]
        public void StructSource_Mapped()
        {
            OrderStruct src = new OrderStruct() { OrderID = 4, Note = "s" };

            OrderDestination dst = PocoToPoco.Map<OrderStruct, OrderDestination>(src);

            Assert.That(dst.OrderID, Is.EqualTo(4));
            Assert.That(dst.Note, Is.EqualTo("s"));
        }

        #endregion
    }
}

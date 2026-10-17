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
//**********************************************************************************

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
    }
}

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
//* クラス名        ：DataToPocoTests
//* クラス日本語名  ：DataToPoco（System.Data → POCO の項目移送）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/17  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Data;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Dto;

namespace TestPublic.Dto
{
    /// <summary>DataToPoco（System.Data → POCO の項目移送）の単体テスト</summary>
    /// <remarks>
    /// **突合は名前で行う。** 名前が合わない列は、例外にならずに値が入らないだけである。
    /// 後退テストは Golden の TestCode（TestDto）にもある。
    /// </remarks>
    [TestFixture]
    [SetCulture("ja-JP")]
    public class DataToPocoTests
    {
        #region 準備

        /// <summary>移送先の POCO（プロパティとフィールドの両方を持つ）</summary>
        public class OrderPoco
        {
            /// <summary>OrderID</summary>
            public int OrderID { get; set; }

            /// <summary>Note</summary>
            public string Note { get; set; }

            /// <summary>Qty（Nullable）</summary>
            public int? Qty { get; set; }

            /// <summary>Price（列は double で渡し、decimal へ変換させる）</summary>
            public decimal Price { get; set; }

            /// <summary>Code（フィールド）</summary>
            public string Code;
        }

        /// <summary>
        /// 2 行の DataTable を作る。
        ///   1 行目：全列に値 / 2 行目：Note と Qty が DBNull
        ///   Extra は POCO に無い列
        /// </summary>
        private static DataTable CreateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("OrderID", typeof(int));
            dt.Columns.Add("Note", typeof(string));
            dt.Columns.Add("Qty", typeof(int));
            dt.Columns.Add("Price", typeof(double));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Extra", typeof(string));

            dt.Rows.Add(1, "x", 3, 1.5d, "C1", "無視される");
            dt.Rows.Add(2, DBNull.Value, DBNull.Value, 2.25d, "C2", "無視される");

            return dt;
        }

        #endregion

        #region List

        /// <summary>
        /// 名前が一致する列だけ移送する。プロパティにもフィールドにも入る。
        /// 型が違えば変換し（double → decimal）、Nullable にも入る。
        /// </summary>
        [Test]
        public void DataTableToList_MapsByName()
        {
            List<OrderPoco> list = DataToPoco.DataTableToList<OrderPoco>(CreateTable());

            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0].OrderID, Is.EqualTo(1));
            Assert.That(list[0].Note, Is.EqualTo("x"));
            Assert.That(list[0].Qty, Is.EqualTo(3));
            Assert.That(list[0].Price, Is.EqualTo(1.5m));
            Assert.That(list[0].Code, Is.EqualTo("C1"));
            Assert.That(list[1].OrderID, Is.EqualTo(2));
            Assert.That(list[1].Price, Is.EqualTo(2.25m));
        }

        /// <summary>DBNull は「指定しない」。参照型は null、Nullable は null のまま。</summary>
        [Test]
        public void DataTableToList_DBNull_LeavesDefault()
        {
            List<OrderPoco> list = DataToPoco.DataTableToList<OrderPoco>(CreateTable());

            Assert.That(list[1].Note, Is.Null);
            Assert.That(list[1].Qty, Is.Null);
        }

        /// <summary>0 件なら空のリスト。</summary>
        [Test]
        public void DataTableToList_Empty()
        {
            DataTable dt = CreateTable();
            dt.Rows.Clear();

            Assert.That(DataToPoco.DataTableToList<OrderPoco>(dt), Is.Empty);
        }

        /// <summary>DataReader を直接渡しても、DataTable と同じ結果になる。</summary>
        [Test]
        public void DataReaderToList_SameAsDataTable()
        {
            using (IDataReader dr = CreateTable().CreateDataReader())
            {
                List<OrderPoco> list = DataToPoco.DataReaderToList<OrderPoco>(dr);

                Assert.That(list, Has.Count.EqualTo(2));
                Assert.That(list[0].OrderID, Is.EqualTo(1));
                Assert.That(list[1].OrderID, Is.EqualTo(2));
            }
        }

        #endregion

        #region T型のPOCO

        /// <summary>DataTableToPOCO は先頭の行だけを移送する。</summary>
        [Test]
        public void DataTableToPOCO_FirstRowOnly()
        {
            OrderPoco poco = DataToPoco.DataTableToPOCO<OrderPoco>(CreateTable());

            Assert.That(poco.OrderID, Is.EqualTo(1));
            Assert.That(poco.Note, Is.EqualTo("x"));
        }

        /// <summary>
        /// **0 件でも null にならず、既定値のインスタンスを返す。**
        /// DataToDictionary.DataTableToDictionary は 0 件で null を返すので、扱いが違う。
        /// </summary>
        [Test]
        public void DataTableToPOCO_Empty_ReturnsNewInstance()
        {
            DataTable dt = CreateTable();
            dt.Rows.Clear();

            OrderPoco poco = DataToPoco.DataTableToPOCO<OrderPoco>(dt);

            Assert.That(poco, Is.Not.Null);
            Assert.That(poco.OrderID, Is.EqualTo(0));
            Assert.That(poco.Note, Is.Null);
        }

        /// <summary>DataReader を直接渡す版。</summary>
        [Test]
        public void DataReaderToPOCO()
        {
            using (IDataReader dr = CreateTable().CreateDataReader())
            {
                OrderPoco poco = DataToPoco.DataReaderToPOCO<OrderPoco>(dr);

                Assert.That(poco.OrderID, Is.EqualTo(1));
                Assert.That(poco.Code, Is.EqualTo("C1"));
            }
        }

        #endregion

        #region マップと名前の突合

        /// <summary>
        /// 名前が合わない列は、**例外にならずに値が入らない。**
        /// 大文字と小文字も区別する（列名 orderid は OrderID に入らない）。
        /// </summary>
        [TestCase("PK_OrderID")]
        [TestCase("orderid")]
        public void NameMismatch_NotMapped(string columnName)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(columnName, typeof(int));
            dt.Rows.Add(9);

            Assert.That(DataToPoco.DataTableToPOCO<OrderPoco>(dt).OrderID, Is.EqualTo(0));
        }

        /// <summary>マップの向きは「POCO の名前 → 列名」。マップに無い名前は、同名の列から取る。</summary>
        [Test]
        public void Map_PocoNameToColumnName()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PK_OrderID", typeof(int));
            dt.Columns.Add("Note", typeof(string));
            dt.Rows.Add(9, "y");

            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("OrderID", "PK_OrderID");

            OrderPoco poco = DataToPoco.DataTableToPOCO<OrderPoco>(dt, map);
            Assert.That(poco.OrderID, Is.EqualTo(9));
            Assert.That(poco.Note, Is.EqualTo("y"));

            List<OrderPoco> list = DataToPoco.DataTableToList<OrderPoco>(dt, map);
            Assert.That(list[0].OrderID, Is.EqualTo(9));
        }

        /// <summary>文字列の列も、数値に変換できれば移送する。変換できなければ FormatException。</summary>
        [Test]
        public void StringColumn_ConvertedToInt()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("OrderID", typeof(string));
            dt.Rows.Add("12");

            Assert.That(DataToPoco.DataTableToPOCO<OrderPoco>(dt).OrderID, Is.EqualTo(12));

            dt.Rows[0]["OrderID"] = "x";
            Assert.Throws<FormatException>(() => DataToPoco.DataTableToPOCO<OrderPoco>(dt));
        }

        #endregion
    }
}

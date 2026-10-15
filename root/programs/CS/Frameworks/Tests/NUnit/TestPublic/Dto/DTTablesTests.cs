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
//* クラス名        ：DTTablesTests
//* クラス日本語名  ：DTTables（汎用 DTO）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

using System;
using System.Data;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Dto;

namespace TestPublic.Dto
{
    /// <summary>DTTables（汎用 DTO）の単体テスト</summary>
    /// <remarks>
    /// DTTables は、.NET（Core）の MVC で編集中の DataTable を Session に持つために使う
    /// （net48 は BinaryFormatter で DataTable を直接持てるが、.NET では使えない）。
    /// **DataTable → DTTables → JSON → DTTables → DataTable で、行の状態と変更前の値が
    /// 保たれること**が、RowState による一括更新（楽観排他を含む）の前提になる（#544 / #567）。
    /// </remarks>
    [TestFixture]
    public class DTTablesTests
    {
        #region 準備

        /// <summary>
        /// 4 つの行の状態を持つ DataTable を作る。
        ///   ID=1 Unchanged / ID=2 Modified（Price 10→20）/ ID=3 Deleted / ID=4 Added（Price は DBNull）
        /// </summary>
        private static DataTable CreateTable()
        {
            DataTable dt = new DataTable("Products");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Price", typeof(decimal));

            dt.Rows.Add(1, "A", 10m);
            dt.Rows.Add(2, "B", 10m);
            dt.Rows.Add(3, "C", 10m);
            dt.AcceptChanges();

            dt.Rows[1]["Price"] = 20m;
            dt.Rows[2].Delete();
            dt.Rows.Add(4, "D", DBNull.Value);

            return dt;
        }

        /// <summary>DataTable → DTTables → JSON → DTTables → DataTable</summary>
        private static DataTable RoundTripJson(DataTable src, bool keepOriginal)
        {
            DTTables dtts = new DTTables();
            dtts.Add(DTTable.FromDataTable(src, keepOriginal));

            string json = DTTables.DTTablesToJson(dtts);
            DTTables restored = DTTables.JsonToDTTables(json);

            return restored["Products"].ToDataTable();
        }

        #endregion

        #region 往復（JSON）

        /// <summary>行の状態が往復で保たれる。</summary>
        [Test]
        public void Json_RoundTrip_KeepsRowState()
        {
            DataTable dt = RoundTripJson(CreateTable(), true);

            Assert.That(dt.Rows.Count, Is.EqualTo(4));
            Assert.That(dt.Rows[0].RowState, Is.EqualTo(DataRowState.Unchanged));
            Assert.That(dt.Rows[1].RowState, Is.EqualTo(DataRowState.Modified));
            Assert.That(dt.Rows[2].RowState, Is.EqualTo(DataRowState.Deleted));
            Assert.That(dt.Rows[3].RowState, Is.EqualTo(DataRowState.Added));
        }

        /// <summary>
        /// **KeepOriginal なら、Modified の行の変更前の値も往復する**（#567）。
        /// 楽観排他の WHERE 句は変更前の値で組むため、これが無いと排他が効かない。
        /// </summary>
        [Test]
        public void Json_RoundTrip_KeepsOriginalValues()
        {
            DataTable dt = RoundTripJson(CreateTable(), true);

            Assert.That(dt.Rows[1]["Price", DataRowVersion.Current], Is.EqualTo(20m));
            Assert.That(dt.Rows[1]["Price", DataRowVersion.Original], Is.EqualTo(10m));

            // Deleted の行は変更前の値で読める
            Assert.That(dt.Rows[2]["Name", DataRowVersion.Original], Is.EqualTo("C"));
        }

        /// <summary>
        /// **KeepOriginal でなければ、変更前の値は失われる。**
        /// 行は Modified に戻るが、Original は現在値と同じになる。
        /// </summary>
        [Test]
        public void Json_RoundTrip_WithoutKeepOriginal_LosesOriginal()
        {
            DataTable dt = RoundTripJson(CreateTable(), false);

            Assert.That(dt.Rows[1].RowState, Is.EqualTo(DataRowState.Modified));
            Assert.That(dt.Rows[1]["Price", DataRowVersion.Original], Is.EqualTo(20m));
        }

        /// <summary>
        /// **値型の列の DBNull が往復する**（#544）。
        /// DTRow は値なしを null で持ち、DataRow へ戻すときに DBNull に置き換える。
        /// </summary>
        [Test]
        public void Json_RoundTrip_DBNullInValueTypeColumn()
        {
            DataTable dt = RoundTripJson(CreateTable(), true);

            Assert.That(dt.Rows[3]["Price"], Is.EqualTo(DBNull.Value));
            Assert.That(dt.Columns["Price"].DataType, Is.EqualTo(typeof(decimal)));
        }

        /// <summary>JSON ではない既定の形式（Save / Load）でも往復できる。</summary>
        [Test]
        public void String_RoundTrip()
        {
            DTTables dtts = new DTTables();
            dtts.Add(DTTable.FromDataTable(CreateTable(), true));

            DTTables restored = DTTables.StringToDTTables(DTTables.DTTablesToString(dtts));
            DataTable dt = restored["Products"].ToDataTable();

            Assert.That(dt.Rows.Count, Is.EqualTo(4));
            Assert.That(dt.Rows[1].RowState, Is.EqualTo(DataRowState.Modified));
            Assert.That(dt.Rows[3]["Name"], Is.EqualTo("D"));
        }

        #endregion

        #region 変更の確定・取り消し

        /// <summary>GetChanges は Added / Modified / Deleted の行。</summary>
        [Test]
        public void GetChanges()
        {
            DTTable t = DTTable.FromDataTable(CreateTable(), true);

            Assert.That(t.GetChanges().Count, Is.EqualTo(3));
        }

        /// <summary>
        /// AcceptChanges は Deleted を取り除き、残りを Unchanged にする。
        /// **変更前の値も捨てる**（#567。持ち続けると次の変更で古い値が Original に残る）。
        /// </summary>
        [Test]
        public void AcceptChanges()
        {
            DTTable t = DTTable.FromDataTable(CreateTable(), true);
            t.AcceptChanges();

            Assert.That(t.Rows.Count, Is.EqualTo(3));
            foreach (DTRow r in t.Rows)
            {
                Assert.That(r.RowState, Is.EqualTo(DTRowState.Unchanged));
                Assert.That(r.HasOriginal, Is.False);
            }
        }

        /// <summary>AcceptChanges の後の変更では、確定した値が変更前の値になる。</summary>
        [Test]
        public void AcceptChanges_ThenModify_OriginalIsAcceptedValue()
        {
            DTTable t = DTTable.FromDataTable(CreateTable(), true);
            t.AcceptChanges();

            t.Rows[1]["Price"] = 30m;

            Assert.That(t.Rows[1].RowState, Is.EqualTo(DTRowState.Modified));
            Assert.That(t.Rows[1]["Price", DTRowVersion.Original], Is.EqualTo(20m));
            Assert.That(t.Rows[1]["Price"], Is.EqualTo(30m));
        }

        /// <summary>RejectChanges は Added を取り除き、Modified と Deleted を元に戻す。</summary>
        [Test]
        public void RejectChanges()
        {
            DTTable t = DTTable.FromDataTable(CreateTable(), true);
            t.RejectChanges();

            Assert.That(t.Rows.Count, Is.EqualTo(3));
            Assert.That(t.Rows[1]["Price"], Is.EqualTo(10m));
            Assert.That(t.Rows[2].RowState, Is.EqualTo(DTRowState.Unchanged));
            foreach (DTRow r in t.Rows)
            {
                Assert.That(r.RowState, Is.EqualTo(DTRowState.Unchanged));
            }
        }

        #endregion

        #region 行と列

        /// <summary>同じ値を設定しても Modified にならない。</summary>
        [Test]
        public void SetSameValue_StaysUnchanged()
        {
            DTTable t = DTTable.FromDataTable(CreateTable(), true);
            t.Rows[0]["Price"] = 10m;

            Assert.That(t.Rows[0].RowState, Is.EqualTo(DTRowState.Unchanged));
        }

        /// <summary>列の型と違う値は、列の型へ自動で変換する。</summary>
        [Test]
        public void SetValue_AutoCast()
        {
            DTTable t = DTTable.FromDataTable(CreateTable(), true);
            t.Rows[0]["Price"] = 15;

            Assert.That(t.Rows[0]["Price"], Is.EqualTo(15m));
            Assert.That(t.Rows[0]["Price"], Is.TypeOf<decimal>());
        }

        /// <summary>存在しない列名は例外。</summary>
        [Test]
        public void UnknownColumn_Throws()
        {
            DTTable t = DTTable.FromDataTable(CreateTable(), true);

            Assert.Catch<Exception>(() => { object o = t.Rows[0]["NoSuchColumn"]; });
        }

        /// <summary>表名は英数字・_・- だけ。</summary>
        [TestCase("Products_2026-10")]
        public void TableName_Valid(string name)
        {
            Assert.That(new DTTable(name).TableName, Is.EqualTo(name));
        }

        /// <summary>表名に使えない文字を含むと例外。</summary>
        [TestCase("Prod ucts")]
        [TestCase("製品")]
        [TestCase("")]
        public void TableName_Invalid(string name)
        {
            Assert.Catch<Exception>(() => new DTTable(name));
        }

        /// <summary>DTTables は表名と添字の両方で引ける。</summary>
        [Test]
        public void Tables_Indexer()
        {
            DTTables dtts = new DTTables();
            dtts.Add(new DTTable("T1"));
            dtts.Add(new DTTable("T2"));

            Assert.That(dtts.Count, Is.EqualTo(2));
            Assert.That(dtts["T2"].TableName, Is.EqualTo("T2"));
            Assert.That(dtts[0].TableName, Is.EqualTo("T1"));
        }

        #endregion
    }
}

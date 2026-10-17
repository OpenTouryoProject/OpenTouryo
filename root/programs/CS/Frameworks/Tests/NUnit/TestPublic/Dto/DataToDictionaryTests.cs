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
//* クラス名        ：DataToDictionaryTests
//* クラス日本語名  ：DataToDictionary（System.Data → Dictionary の項目移送）の単体テスト
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
    /// <summary>DataToDictionary（System.Data → Dictionary の項目移送）の単体テスト</summary>
    /// <remarks>
    /// 値はすべて文字列にする。数値の文字列化はカルチャに依存するので、ja-JP に固定する。
    /// 後退テストは Golden の TestCode（TestDto）にもある。
    /// </remarks>
    [TestFixture]
    [SetCulture("ja-JP")]
    public class DataToDictionaryTests
    {
        #region 準備

        /// <summary>日時</summary>
        private static readonly DateTime OrderDate = new DateTime(2026, 8, 6, 12, 34, 56);

        /// <summary>時間</summary>
        private static readonly TimeSpan Elapsed = new TimeSpan(1, 2, 3);

        /// <summary>
        /// 2 行の DataTable を作る。
        ///   1 行目：全列に値 / 2 行目：Note が DBNull
        /// </summary>
        private static DataTable CreateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("OrderID", typeof(int));
            dt.Columns.Add("Note", typeof(string));
            dt.Columns.Add("OrderDate", typeof(DateTime));
            dt.Columns.Add("Elapsed", typeof(TimeSpan));

            dt.Rows.Add(4, "w", OrderDate, Elapsed);
            dt.Rows.Add(5, DBNull.Value, OrderDate, Elapsed);

            return dt;
        }

        #endregion

        #region コンストラクタ

        /// <summary>既定のコンストラクタは、マップ無し・書式無し。</summary>
        [Test]
        public void Constructor_Default()
        {
            DataToDictionary d2d = new DataToDictionary();

            Assert.That(d2d.Mapping, Is.Null);
            Assert.That(d2d.DateTimeFormat, Is.Empty);
            Assert.That(d2d.TimeSpanFormat, Is.Empty);
        }

        /// <summary>
        /// 引数はそれぞれ同名のフィールドに入る。
        /// 2026/08/06 まで、日時と時間の書式の代入先が入れ替わっていた（#522）。
        /// </summary>
        [Test]
        public void Constructor_AssignsEachFormat()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            DataToDictionary d2d = new DataToDictionary(map, "yyyy/MM/dd", @"hh\:mm");

            Assert.That(d2d.Mapping, Is.SameAs(map));
            Assert.That(d2d.DateTimeFormat, Is.EqualTo("yyyy/MM/dd"));
            Assert.That(d2d.TimeSpanFormat, Is.EqualTo(@"hh\:mm"));
        }

        #endregion

        #region 値の文字列化

        /// <summary>書式を指定すれば、日時と時間はその書式で文字列にする。</summary>
        [Test]
        public void Format_Specified()
        {
            DataToDictionary d2d = new DataToDictionary(null, "yyyy/MM/dd HH:mm:ss", @"hh\:mm\:ss");
            Dictionary<string, string> dic = d2d.DataTableToDictionary(CreateTable());

            Assert.That(dic["OrderID"], Is.EqualTo("4"));
            Assert.That(dic["Note"], Is.EqualTo("w"));
            Assert.That(dic["OrderDate"], Is.EqualTo("2026/08/06 12:34:56"));
            Assert.That(dic["Elapsed"], Is.EqualTo("01:02:03"));
        }

        /// <summary>
        /// **書式が空なら、日時と時間は Ticks になる**（精度を保つための仕様）。
        /// 書式の文字列ではないので、表示に使うなら書式を渡すこと。
        /// </summary>
        [Test]
        public void Format_Empty_Ticks()
        {
            Dictionary<string, string> dic = new DataToDictionary().DataTableToDictionary(CreateTable());

            Assert.That(dic["OrderDate"], Is.EqualTo(OrderDate.Ticks.ToString()));
            Assert.That(dic["Elapsed"], Is.EqualTo(Elapsed.Ticks.ToString()));
        }

        /// <summary>
        /// **DBNull は空文字になる**（キーは残る）。
        /// DataToPoco は DBNull を「指定しない」ので、扱いが違う。null と空文字の区別は付かない。
        /// </summary>
        [Test]
        public void DBNull_EmptyString()
        {
            List<Dictionary<string, string>> list = new DataToDictionary().DataTableToDictionaryList(CreateTable());

            Assert.That(list[1].ContainsKey("Note"), Is.True);
            Assert.That(list[1]["Note"], Is.Empty);
        }

        #endregion

        #region List と Dictionary

        /// <summary>DataTableToDictionaryList は全行を変換する。</summary>
        [Test]
        public void DataTableToDictionaryList_AllRows()
        {
            List<Dictionary<string, string>> list = new DataToDictionary().DataTableToDictionaryList(CreateTable());

            Assert.That(list, Has.Count.EqualTo(2));
            Assert.That(list[0]["OrderID"], Is.EqualTo("4"));
            Assert.That(list[1]["OrderID"], Is.EqualTo("5"));
            Assert.That(list[0].Keys, Is.EquivalentTo(new[] { "OrderID", "Note", "OrderDate", "Elapsed" }));
        }

        /// <summary>
        /// 0 件なら、List は空、Dictionary は **null**。
        /// DataToPoco.DataTableToPOCO は 0 件でもインスタンスを返すので、扱いが違う。
        /// </summary>
        [Test]
        public void Empty()
        {
            DataTable dt = CreateTable();
            dt.Rows.Clear();

            DataToDictionary d2d = new DataToDictionary();
            Assert.That(d2d.DataTableToDictionaryList(dt), Is.Empty);
            Assert.That(d2d.DataTableToDictionary(dt), Is.Null);
        }

        /// <summary>DataTableToDictionary は先頭の行だけを変換する。</summary>
        [Test]
        public void DataTableToDictionary_FirstRowOnly()
        {
            Dictionary<string, string> dic = new DataToDictionary().DataTableToDictionary(CreateTable());

            Assert.That(dic["OrderID"], Is.EqualTo("4"));
        }

        /// <summary>DataReader を直接渡しても、DataTable と同じ結果になる。</summary>
        [Test]
        public void DataReader_SameAsDataTable()
        {
            DataToDictionary d2d = new DataToDictionary();

            using (IDataReader dr = CreateTable().CreateDataReader())
            {
                Assert.That(d2d.DataReaderToDictionaryList(dr), Has.Count.EqualTo(2));
            }

            using (IDataReader dr = CreateTable().CreateDataReader())
            {
                Assert.That(d2d.DataReaderToDictionary(dr)["Note"], Is.EqualTo("w"));
            }
        }

        #endregion

        #region マップ

        /// <summary>
        /// マップの向きは「列名 → Dictionary のキー」（DataToPoco・PocoToPoco とは逆向き）。
        /// マップに無い列は、列名のまま入る。
        /// </summary>
        [Test]
        public void Map_ColumnNameToKey()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("OrderID", "id");

            DataToDictionary d2d = new DataToDictionary(map, "", "");

            Dictionary<string, string> dic = d2d.DataTableToDictionary(CreateTable());
            Assert.That(dic["id"], Is.EqualTo("4"));
            Assert.That(dic.ContainsKey("OrderID"), Is.False);
            Assert.That(dic["Note"], Is.EqualTo("w"));

            List<Dictionary<string, string>> list = d2d.DataTableToDictionaryList(CreateTable());
            Assert.That(list[1]["id"], Is.EqualTo("5"));
        }

        #endregion
    }
}

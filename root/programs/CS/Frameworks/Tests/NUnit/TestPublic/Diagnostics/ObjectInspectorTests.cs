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
//* クラス名        ：ObjectInspectorTests
//* クラス日本語名  ：ObjectInspector の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Diagnostics;

namespace TestPublic.Diagnostics
{
    /// <summary>検査される POCO</summary>
    public class InspectTarget
    {
        /// <summary>Name</summary>
        public string Name { get; set; }

        /// <summary>Kind</summary>
        public DayOfWeek Kind { get; set; }
    }

    /// <summary>ObjectInspector の単体テスト</summary>
    /// <remarks>
    /// 入れ子・配列・深さの上限は Golden の TestObjectInspector が出力比較で見ている。
    /// ここでは型ごとの書き方と、**静的な設定**（書式・除外）の効き方を確かめる。
    /// 静的な設定は後続のテストに影響するため、TearDown で必ず戻す。
    /// </remarks>
    [TestFixture]
    [SetCulture("ja-JP")]
    public class ObjectInspectorTests
    {
        private string _dateTimeFormat;
        private string _timeSpanFormat;
        private string[] _exclusions;

        /// <summary>静的な設定を退避する。</summary>
        [SetUp]
        public void SetUp()
        {
            this._dateTimeFormat = ObjectInspector.DateTimeFormat;
            this._timeSpanFormat = ObjectInspector.TimeSpanFormat;
            this._exclusions = ObjectInspector.ExclusionFullyQualifiedNameParts;
        }

        /// <summary>静的な設定を戻す。</summary>
        [TearDown]
        public void TearDown()
        {
            ObjectInspector.DateTimeFormat = this._dateTimeFormat;
            ObjectInspector.TimeSpanFormat = this._timeSpanFormat;
            ObjectInspector.ExclusionFullyQualifiedNameParts = this._exclusions;
        }

        /// <summary>基本の型。文字列は "" で囲む（StringBuilder も）。列挙体は名前。</summary>
        [Test]
        public void Primitives()
        {
            Assert.That(ObjectInspector.Inspect(null), Is.EqualTo("null"));
            Assert.That(ObjectInspector.Inspect(123), Is.EqualTo("123"));
            Assert.That(ObjectInspector.Inspect(true), Is.EqualTo("True"));
            Assert.That(ObjectInspector.Inspect("abc"), Is.EqualTo("\"abc\""));
            Assert.That(ObjectInspector.Inspect(new StringBuilder("sb")), Is.EqualTo("\"sb\""));
            Assert.That(ObjectInspector.Inspect(DayOfWeek.Friday), Is.EqualTo("Friday"));
        }

        /// <summary>配列・リストは [a, b]、空なら -empty-。辞書は [キー = 値]。</summary>
        [Test]
        public void Collections()
        {
            Assert.That(ObjectInspector.Inspect(new[] { 1, 2 }), Is.EqualTo("[1, 2]"));
            Assert.That(ObjectInspector.Inspect(new int[0]), Is.EqualTo("-empty-"));
            Assert.That(ObjectInspector.Inspect(new List<string> { "a" }), Is.EqualTo("[\"a\"]"));
            Assert.That(ObjectInspector.Inspect(new Dictionary<string, int> { { "k", 1 } }), Is.EqualTo("[k = 1]"));
            Assert.That(ObjectInspector.Inspect(new Dictionary<string, int>()), Is.EqualTo("-empty-"));
        }

        /// <summary>object[] は要素ごとに並べる（入口だけの特別扱い）。</summary>
        [Test]
        public void ObjectArray()
        {
            Assert.That(ObjectInspector.Inspect(new object[] { 1, "x", null }), Is.EqualTo("[1, \"x\", null]"));
        }

        /// <summary>POCO はプロパティを [名前 = 値, ...] で並べる。</summary>
        [Test]
        public void Poco()
        {
            InspectTarget t = new InspectTarget() { Name = "n", Kind = DayOfWeek.Monday };

            Assert.That(ObjectInspector.Inspect(t), Is.EqualTo("[Name = \"n\", Kind = Monday]"));
        }

        /// <summary>完全修飾名の一部で除外すると、中身を辿らない。</summary>
        [Test]
        public void Exclusion()
        {
            ObjectInspector.ExclusionFullyQualifiedNameParts = new[] { "TestPublic.Diagnostics.InspectTarget" };

            string s = ObjectInspector.Inspect(new InspectTarget() { Name = "secret" });

            Assert.That(s, Does.Not.Contain("secret"));
        }

        /// <summary>DateTime / TimeSpan は、書式を設定すればその書式で出す。</summary>
        [Test]
        public void DateTimeAndTimeSpanFormat()
        {
            ObjectInspector.DateTimeFormat = "yyyy-MM-dd";
            ObjectInspector.TimeSpanFormat = @"hh\:mm";

            Assert.That(ObjectInspector.Inspect(new DateTime(2026, 10, 15, 1, 2, 3)), Is.EqualTo("2026-10-15"));
            Assert.That(ObjectInspector.Inspect(new TimeSpan(1, 2, 3)), Is.EqualTo("01:02"));
        }
    }
}

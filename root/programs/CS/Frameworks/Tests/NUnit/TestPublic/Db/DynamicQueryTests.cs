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
//* クラス名        ：DynamicQueryTests
//* クラス日本語名  ：パラメタライズド クエリの組み立ての単体テスト（DB に接続しない）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Db;

namespace TestPublic.Db
{
    /// <summary>パラメタライズド クエリの組み立ての単体テスト（DB に接続しない）</summary>
    /// <remarks>
    /// **DamSqlSvr.ExecGenerateSQL は接続を使わない。** SQL を組み立て、
    /// パラメタを SQLUtility でリテラルにして返すだけである（Golden TestDataAccess/README.md）。
    /// これで、動的パラメタライズド クエリ（XML のタグ）の振る舞いを DB 無しで確かめられる。
    /// ExecGenerateSQL を実装しているのは DamSqlSvr だけだが、組み立ては BaseDam の共通処理。
    ///
    /// 組み立て結果の空白・改行は実装の都合で変わるため、比べる前に 1 つの空白に詰める。
    /// </remarks>
    [TestFixture]
    [SetCulture("ja-JP")]
    public class DynamicQueryTests
    {
        private static readonly SQLUtility Util = new SQLUtility(DbEnum.DBMSType.SQLServer);

        /// <summary>動的 SQL を設定する（ROOT で包む）。</summary>
        private static DamSqlSvr Dpq(string inner)
        {
            DamSqlSvr dam = new DamSqlSvr();
            dam.SetSqlByCommand("<?xml version=\"1.0\" encoding=\"UTF-8\" ?><ROOT>" + inner + "</ROOT>");
            return dam;
        }

        /// <summary>組み立てて、空白を詰める。</summary>
        private static string Generate(DamSqlSvr dam)
        {
            return Regex.Replace(dam.ExecGenerateSQL(Util), @"\s+", " ").Trim();
        }

        private const string Where =
            "SELECT [OrderID] FROM [Orders] " +
            "<WHERE>WHERE " +
            "<IF>AND [Qty] = @P1</IF> " +
            "<IF>AND [Note] = @P2</IF>" +
            "</WHERE>";

        #region 静的

        /// <summary>静的 SQL は、パラメタをリテラルに置き換えるだけ。IsDPQ は False。</summary>
        [Test]
        public void Static_ReplacesParameters()
        {
            DamSqlSvr dam = new DamSqlSvr();
            dam.SetSqlByCommand("SELECT * FROM [Orders] WHERE [OrderID] = @P1 AND [Note] = @P2");
            dam.SetParameter("P1", 10);
            dam.SetParameter("P2", "x");

            Assert.That(dam.IsDPQ, Is.False);
            Assert.That(Generate(dam),
                Is.EqualTo("SELECT * FROM [Orders] WHERE [OrderID] = 10 AND [Note] = Convert(nvarchar(1), 'x')"));
        }

        /// <summary>静的 SQL のユーザ パラメタ（%名前%）は、文字列のまま埋め込む。</summary>
        [Test]
        public void Static_UserParameter()
        {
            DamSqlSvr dam = new DamSqlSvr();
            dam.SetSqlByCommand("SELECT * FROM [Orders] ORDER BY %COL%");
            dam.SetUserParameter("COL", "[Qty] DESC");

            Assert.That(Generate(dam), Is.EqualTo("SELECT * FROM [Orders] ORDER BY [Qty] DESC"));
        }

        /// <summary>ファイルからも読める（拡張子ではなく中身で動的かどうかが決まる）。</summary>
        [Test]
        public void SetSqlByFile()
        {
            string path = Path.Combine(Path.GetTempPath(), "OpenTouryoNUnit_" + Guid.NewGuid().ToString("N") + ".sql");
            try
            {
                File.WriteAllText(path, "SELECT 1 WHERE 1 = @P1", Encoding.UTF8);

                DamSqlSvr dam = new DamSqlSvr();
                dam.SetSqlByFile(path);
                dam.SetParameter("P1", 1);

                Assert.That(dam.IsDPQ, Is.False);
                Assert.That(Generate(dam), Is.EqualTo("SELECT 1 WHERE 1 = 1"));
            }
            finally
            {
                File.Delete(path);
            }
        }

        #endregion

        #region WHERE / IF

        /// <summary>IsDPQ は、XML として解釈できる SQL で True。</summary>
        [Test]
        public void IsDPQ_True()
        {
            Assert.That(Dpq(Where).IsDPQ, Is.True);
        }

        /// <summary>パラメタを設定した IF だけが残る。WHERE 直後の AND は取り除かれる。</summary>
        [Test]
        public void Where_BothIf()
        {
            DamSqlSvr dam = Dpq(Where);
            dam.SetParameter("P1", 1);
            dam.SetParameter("P2", "a");

            Assert.That(Generate(dam),
                Is.EqualTo("SELECT [OrderID] FROM [Orders] WHERE [Qty] = 1 AND [Note] = Convert(nvarchar(1), 'a')"));
        }

        /// <summary>後ろの IF だけ有効でも、先頭の AND は取り除かれる。</summary>
        [Test]
        public void Where_SecondIfOnly()
        {
            DamSqlSvr dam = Dpq(Where);
            dam.SetParameter("P2", "a");

            Assert.That(Generate(dam),
                Is.EqualTo("SELECT [OrderID] FROM [Orders] WHERE [Note] = Convert(nvarchar(1), 'a')"));
        }

        /// <summary>**IF が 1 つも残らなければ、WHERE ごと消える。**</summary>
        [Test]
        public void Where_NoIf_RemovesWhere()
        {
            Assert.That(Generate(Dpq(Where)), Is.EqualTo("SELECT [OrderID] FROM [Orders]"));
        }

        /// <summary>タグ内パラメタ（name 属性）で有効・無効を決め、無効なら ELSE を使う。</summary>
        [Test]
        public void If_NameAndElse()
        {
            string xml =
                "SELECT [OrderID] FROM [Orders] <WHERE>WHERE " +
                "<IF name=\"F1\">AND [Note] = 'x'<ELSE>AND [Note] IS NULL</ELSE></IF>" +
                "</WHERE>";

            DamSqlSvr on = Dpq(xml);
            on.SetParameter("F1", true);
            Assert.That(Generate(on), Is.EqualTo("SELECT [OrderID] FROM [Orders] WHERE [Note] = 'x'"));

            DamSqlSvr off = Dpq(xml);
            off.SetParameter("F1", false);
            Assert.That(Generate(off), Is.EqualTo("SELECT [OrderID] FROM [Orders] WHERE [Note] IS NULL"));
        }

        /// <summary>比較演算子は &amp;lt; でも CDATA でも書ける。</summary>
        [Test]
        public void ComparisonOperators()
        {
            DamSqlSvr dam = Dpq(
                "SELECT [OrderID] FROM [Orders] <WHERE>WHERE " +
                "<IF>AND [Qty] &lt; @P1</IF>" +
                "<IF><![CDATA[AND [Qty] > @P2]]></IF>" +
                "</WHERE>");
            dam.SetParameter("P1", 900);
            dam.SetParameter("P2", 100);

            Assert.That(Generate(dam), Is.EqualTo("SELECT [OrderID] FROM [Orders] WHERE [Qty] < 900 AND [Qty] > 100"));
        }

        #endregion

        #region LIST / SELECT / JOIN / SUB / DELCMA

        /// <summary>LIST は配列の要素を , で並べて IN 句にする。</summary>
        [Test]
        public void List()
        {
            DamSqlSvr dam = Dpq(
                "SELECT [OrderID] FROM [Orders] <WHERE>WHERE " +
                "<LIST>AND [OrderID] IN (@PLIST)</LIST>" +
                "</WHERE>");
            dam.SetParameter("PLIST", new ArrayList { 1, 2, 4 });

            Assert.That(Generate(dam), Does.Match(@"^SELECT \[OrderID\] FROM \[Orders\] WHERE \[OrderID\] IN \(1 ?, ?2 ?, ?4\)$"));
        }

        /// <summary>SELECT / CASE / DEFAULT は値で分岐する。</summary>
        [TestCase("a1", "SELECT 1 AS X")]
        [TestCase("b2", "SELECT 2 AS X")]
        [TestCase("zz", "SELECT 9 AS X")]
        public void SelectCase(string value, string expected)
        {
            DamSqlSvr dam = Dpq(
                "<SELECT name=\"SEL\">" +
                "<CASE value=\"a1\">SELECT 1 AS X</CASE>" +
                "<CASE value=\"b2\">SELECT 2 AS X</CASE>" +
                "<DEFAULT>SELECT 9 AS X</DEFAULT>" +
                "</SELECT>");
            dam.SetParameter("SEL", value);

            Assert.That(Generate(dam), Is.EqualTo(expected));
        }

        /// <summary>
        /// JOIN / SUB は結合・副問い合わせを丸ごと出し入れする。**無効にするときは設定しない。**
        /// 実行しないので、存在しない表を書いてよい。
        /// </summary>
        [Test]
        public void JoinAndSub()
        {
            string xml =
                "SELECT o.[OrderID] FROM [Orders] o " +
                "<JOIN name=\"J1\">INNER JOIN [Other] t ON o.[OrderID] = t.[OrderID]</JOIN> " +
                "<WHERE>WHERE " +
                "<IF>AND o.[Qty] = @P1</IF> " +
                "<SUB name=\"S1\">AND o.[OrderID] IN (SELECT [OrderID] FROM [Other])</SUB>" +
                "</WHERE>";

            DamSqlSvr all = Dpq(xml);
            all.SetParameter("J1", true);
            all.SetParameter("S1", true);
            all.SetParameter("P1", 999);
            Assert.That(Generate(all), Is.EqualTo(
                "SELECT o.[OrderID] FROM [Orders] o INNER JOIN [Other] t ON o.[OrderID] = t.[OrderID] " +
                "WHERE o.[Qty] = 999 AND o.[OrderID] IN (SELECT [OrderID] FROM [Other])"));

            Assert.That(Generate(Dpq(xml)), Is.EqualTo("SELECT o.[OrderID] FROM [Orders] o"));
        }

        /// <summary>DELCMA は、残った要素の末尾のカンマを取り除く（列と値を対で増減できる）。</summary>
        [Test]
        public void DelcmaAndInscol()
        {
            string xml =
                "INSERT INTO [Orders] ([OrderID], " +
                "<DELCMA><INSCOL name=\"Qty\">[Qty],</INSCOL><INSCOL name=\"Note\">[Note],</INSCOL></DELCMA>" +
                ") VALUES (@OrderID, " +
                "<DELCMA><IF>@Qty,</IF><IF>@Note,</IF></DELCMA>" +
                ")";

            DamSqlSvr dam = Dpq(xml);
            dam.SetParameter("OrderID", 1);
            dam.SetParameter("Qty", 5);

            Assert.That(Regex.Replace(Generate(dam), @"\s*([(),])\s*", "$1"),
                Is.EqualTo("INSERT INTO [Orders]([OrderID],[Qty])VALUES(1,5)"));
        }

        #endregion

        #region 補助

        /// <summary>DeleteFirstLogicalOperatoronWhereClause は WHERE 句の先頭の AND / OR を取り除く。</summary>
        [TestCase("WHERE AND A = 1", "WHERE A = 1")]
        [TestCase("WHERE OR A = 1", "WHERE A = 1")]
        [TestCase("WHERE A = 1 AND B = 2", "WHERE A = 1 AND B = 2")]
        public void DeleteFirstLogicalOperator(string where, string expected)
        {
            Assert.That(Regex.Replace(BaseDam.DeleteFirstLogicalOperatoronWhereClause(where), @"\s+", " ").Trim(),
                Is.EqualTo(expected));
        }

        #endregion
    }
}

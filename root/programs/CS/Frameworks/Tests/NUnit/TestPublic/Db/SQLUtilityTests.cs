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
//* クラス名        ：SQLUtilityTests
//* クラス日本語名  ：SQLUtility の単体テスト（DB に接続しない）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//*  2026/10/15  玄人 幸道         #612 の修正に合わせ、修正後の挙動を確かめるケースを追加
//**********************************************************************************

using System;
using System.Data;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Db;

namespace TestPublic.Db
{
    /// <summary>SQLUtility の単体テスト（DB に接続しない）</summary>
    /// <remarks>
    /// INSERT / UPDATE の組み立て結果を 4 DBMS 分並べて見るのは Golden の TestSQLUtility。
    /// ここでは**値を SQL リテラルにする規則**（ConvertParameterToSQL）を型・DBMS ごとに確かめる。
    /// 数値・日時の文字列化はカルチャに依存するため、ja-JP に固定する。
    /// </remarks>
    [TestFixture]
    [SetCulture("ja-JP")]
    public class SQLUtilityTests
    {
        #region 囲い文字

        /// <summary>DBMS ごとの識別子の囲い文字。</summary>
        [TestCase(DbEnum.DBMSType.SQLServer, '[', ']')]
        [TestCase(DbEnum.DBMSType.Oracle, '"', '"')]
        [TestCase(DbEnum.DBMSType.DB2, '"', '"')]
        [TestCase(DbEnum.DBMSType.PstGrS, '"', '"')]
        [TestCase(DbEnum.DBMSType.MySQL, '`', '`')]
        public void Brackets(DbEnum.DBMSType dbms, char open, char close)
        {
            SQLUtility u = new SQLUtility(dbms);

            Assert.That(u.OpeningBracket, Is.EqualTo(open));
            Assert.That(u.ClosingBracket, Is.EqualTo(close));
        }

        /// <summary>HiRDB はコンストラクタが対応していない（囲い文字の定義はあるが使えない）。</summary>
        [Test]
        public void HiRDB_NotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => new SQLUtility(DbEnum.DBMSType.HiRDB));
        }

        #endregion

        #region 文字列

        /// <summary>
        /// 文字列は DBMS ごとの型変換で包み、**' を '' にエスケープする。**
        /// 長さ（エスケープ前の文字数）を型に付ける。
        /// </summary>
        [TestCase(DbEnum.DBMSType.SQLServer, "Convert(nvarchar(4), 'it''s')")]
        [TestCase(DbEnum.DBMSType.Oracle, "CAST('it''s' AS varchar2(4))")]
        [TestCase(DbEnum.DBMSType.PstGrS, "Cast('it''s' as text)")]
        [TestCase(DbEnum.DBMSType.MySQL, "Cast('it''s' as char(4))")]
        [TestCase(DbEnum.DBMSType.DB2, "CAST('it''s' AS varchar(4))")]
        public void String_EscapesQuote(DbEnum.DBMSType dbms, string expected)
        {
            Assert.That(new SQLUtility(dbms).ConvertParameterToSQL("it's"), Is.EqualTo(expected));
        }

        /// <summary>空文字列は、長さ 0 の型が作れない DBMS があるため別の書き方になる。</summary>
        [TestCase(DbEnum.DBMSType.SQLServer, "Convert(nvarchar, '')")]
        [TestCase(DbEnum.DBMSType.Oracle, "To_CHAR('')")]
        [TestCase(DbEnum.DBMSType.PstGrS, "''")]
        [TestCase(DbEnum.DBMSType.MySQL, "Cast('' as char)")]
        [TestCase(DbEnum.DBMSType.DB2, "CAST('' AS CHAR)")]
        public void String_Empty(DbEnum.DBMSType dbms, string expected)
        {
            Assert.That(new SQLUtility(dbms).ConvertParameterToSQL(""), Is.EqualTo(expected));
        }

        /// <summary>型変換の型名はコンストラクタで差し替えられる。</summary>
        [Test]
        public void String_CustomConvertType()
        {
            Assert.That(new SQLUtility(DbEnum.DBMSType.SQLServer, "varchar").ConvertParameterToSQL("ab"),
                Is.EqualTo("Convert(varchar(2), 'ab')"));
        }

        #endregion

        #region その他の型

        /// <summary>数値はそのまま文字列化する。</summary>
        [Test]
        public void Number()
        {
            SQLUtility u = new SQLUtility(DbEnum.DBMSType.SQLServer);

            Assert.That(u.ConvertParameterToSQL(123), Is.EqualTo("123"));
            Assert.That(u.ConvertParameterToSQL(-1.5m), Is.EqualTo("-1.5"));
        }

        /// <summary>バイト配列は 16 進のリテラルにする。</summary>
        [TestCase(DbEnum.DBMSType.SQLServer, "0x00AFFF")]
        [TestCase(DbEnum.DBMSType.MySQL, "0x00AFFF")]
        [TestCase(DbEnum.DBMSType.Oracle, "hextoraw('00AFFF')")]
        [TestCase(DbEnum.DBMSType.PstGrS, "decode('00AFFF', 'hex')")]
        [TestCase(DbEnum.DBMSType.DB2, "x'00AFFF'")]
        public void ByteArray(DbEnum.DBMSType dbms, string expected)
        {
            Assert.That(new SQLUtility(dbms).ConvertParameterToSQL(new byte[] { 0x00, 0xAF, 0xFF }), Is.EqualTo(expected));
        }

        /// <summary>日時は DBMS ごとの既定の書式で文字列リテラルにする。書式は差し替えられる。</summary>
        [Test]
        public void DateTimeValue()
        {
            DateTime dt = new DateTime(2026, 10, 15, 13, 4, 5, 6);

            Assert.That(new SQLUtility(DbEnum.DBMSType.SQLServer).ConvertParameterToSQL(dt),
                Is.EqualTo("'2026/10/15 13:04:05.006'"));
            Assert.That(new SQLUtility(DbEnum.DBMSType.DB2).ConvertParameterToSQL(dt),
                Is.EqualTo("'2026-10-15-13.04.05.006'"));
            Assert.That(new SQLUtility(DbEnum.DBMSType.PstGrS).ConvertParameterToSQL(dt),
                Is.EqualTo("Cast('2026/10/15 13:04:05.006' as date)"));
            Assert.That(new SQLUtility(DbEnum.DBMSType.SQLServer, "", "yyyyMMdd").ConvertParameterToSQL(dt),
                Is.EqualTo("'20261015'"));
        }

        /// <summary>DBNull は NULL。</summary>
        [Test]
        public void DBNullValue()
        {
            Assert.That(new SQLUtility(DbEnum.DBMSType.SQLServer).ConvertParameterToSQL(DBNull.Value), Is.EqualTo("NULL"));
            Assert.That(new SQLUtility(DbEnum.DBMSType.PstGrS).ConvertParameterToSQL(DBNull.Value), Is.EqualTo("NULL"));
        }

        /// <summary>
        /// null は NULL（PostgreSQL は DEFAULT）。
        /// **以前は NullReferenceException になっていた**（#612 で修正）。
        /// </summary>
        [TestCase(DbEnum.DBMSType.SQLServer, "NULL")]
        [TestCase(DbEnum.DBMSType.Oracle, "NULL")]
        [TestCase(DbEnum.DBMSType.MySQL, "NULL")]
        [TestCase(DbEnum.DBMSType.DB2, "NULL")]
        [TestCase(DbEnum.DBMSType.PstGrS, "DEFAULT")]
        public void NullValue(DbEnum.DBMSType dbms, string expected)
        {
            Assert.That(new SQLUtility(dbms).ConvertParameterToSQL(null), Is.EqualTo(expected));
        }

        /// <summary>
        /// 単体の byte も 16 進のリテラルにする。
        /// **以前は PostgreSQL だけ InvalidCastException になっていた**（#612 で修正）。
        /// </summary>
        [TestCase(DbEnum.DBMSType.SQLServer, "0xAB")]
        [TestCase(DbEnum.DBMSType.MySQL, "0xAB")]
        [TestCase(DbEnum.DBMSType.Oracle, "hextoraw('AB')")]
        [TestCase(DbEnum.DBMSType.PstGrS, "decode('AB', 'hex')")]
        [TestCase(DbEnum.DBMSType.DB2, "x'AB'")]
        public void SingleByte(DbEnum.DBMSType dbms, string expected)
        {
            Assert.That(new SQLUtility(dbms).ConvertParameterToSQL((byte)0xAB), Is.EqualTo(expected));
        }

        /// <summary>
        /// char も ' を '' にエスケープする（string と同じ）。
        /// **以前はエスケープせず、引用符が閉じない SQL になっていた**（#612 で修正）。
        /// </summary>
        [TestCase(DbEnum.DBMSType.SQLServer, "Convert(nvarchar, '''')")]
        [TestCase(DbEnum.DBMSType.MySQL, "Cast('''' as char)")]
        [TestCase(DbEnum.DBMSType.PstGrS, "Cast('''' as text)")]
        [TestCase(DbEnum.DBMSType.Oracle, "TO_CHAR('''')")]
        [TestCase(DbEnum.DBMSType.DB2, "CHAR('''')")]
        public void Char_EscapesQuote(DbEnum.DBMSType dbms, string expected)
        {
            Assert.That(new SQLUtility(dbms).ConvertParameterToSQL('\''), Is.EqualTo(expected));
            Assert.That(new SQLUtility(dbms).ConvertParameterToSQL('a'), Does.Contain("'a'"));
        }

        #endregion

        #region INSERT / UPDATE の部品

        /// <summary>検証用の表（主キー OrderID）</summary>
        private static DataTable CreateTable()
        {
            DataTable dt = new DataTable("Orders");
            dt.Columns.Add("OrderID", typeof(int));
            dt.Columns.Add("Qty", typeof(int));
            dt.Columns.Add("Note", typeof(string));
            dt.Rows.Add(1, 100, "a");
            dt.Rows.Add(2, 200, DBNull.Value);
            return dt;
        }

        /// <summary>GetInsertSQLParts は 1 つ目が列の並び、2 つ目以降が行ごとの値。</summary>
        [Test]
        public void GetInsertSQLParts()
        {
            string[] parts = new SQLUtility(DbEnum.DBMSType.SQLServer).GetInsertSQLParts(CreateTable());

            Assert.That(parts, Is.EqualTo(new[]
            {
                "([OrderID], [Qty], [Note])",
                "(1, 100, Convert(nvarchar(1), 'a'))",
                "(2, 200, NULL)"
            }));
        }

        /// <summary>
        /// SQL Server の GetUpdateSQLParts は行ごとの「SET ... WHERE ...」。主キーは WHERE、残りは SET へ。
        /// **「UPDATE 表名」は含まない**（呼ぶ側が前に付ける）。
        /// </summary>
        [Test]
        public void GetUpdateSQLParts_SQLServer()
        {
            string[] parts = new SQLUtility(DbEnum.DBMSType.SQLServer).GetUpdateSQLParts(CreateTable(), new[] { "OrderID" });

            Assert.That(parts.Length, Is.EqualTo(2));
            Assert.That(parts[0].Trim(),
                Is.EqualTo("SET [Qty] = 100, [Note] = Convert(nvarchar(1), 'a') WHERE [OrderID] = 1"));
            Assert.That(parts[1].Trim(),
                Is.EqualTo("SET [Qty] = 200, [Note] = NULL WHERE [OrderID] = 2"));
        }

        /// <summary>主キーが無い、または全列が主キーなら、何も返さない。</summary>
        [Test]
        public void GetUpdateSQLParts_NoKeyOrNoSet()
        {
            SQLUtility u = new SQLUtility(DbEnum.DBMSType.SQLServer);

            Assert.That(u.GetUpdateSQLParts(CreateTable(), new string[0]), Is.Empty);
            Assert.That(u.GetUpdateSQLParts(CreateTable(), new[] { "OrderID", "Qty", "Note" }), Is.Empty);
        }

        #endregion
    }
}

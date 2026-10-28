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
//* クラス名        ：SQLSanitizerTests
//* クラス日本語名  ：SQLSanitizer の単体テスト（DB に接続しない）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/28  玄人 幸道         新規作成
//**********************************************************************************

using System;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Db;

namespace TestPublic.Db
{
    /// <summary>SQLSanitizer の単体テスト（DB に接続しない）</summary>
    /// <remarks>
    /// 期待値は各 DBMS の文字列リテラル・識別子の規則（SQLSanitizer の remarks）から書く。
    /// 実際の DB に入れて同じ値で読み戻せることは、TestPublicDb の DamTests が確かめる。
    /// </remarks>
    [TestFixture]
    public class SQLSanitizerTests
    {
        #region 文字列リテラル

        /// <summary>
        /// SQL Server / Oracle / DB2 は、' を '' にして ' で囲う。**\ は特別な文字ではないので、そのまま。**
        /// </summary>
        [TestCase(DbEnum.DBMSType.SQLServer)]
        [TestCase(DbEnum.DBMSType.Oracle)]
        [TestCase(DbEnum.DBMSType.DB2)]
        public void ToStringLiteral_Standard(DbEnum.DBMSType dbms)
        {
            Assert.That(SQLSanitizer.ToStringLiteral("it's", dbms), Is.EqualTo("'it''s'"));
            Assert.That(SQLSanitizer.ToStringLiteral("a\\b", dbms), Is.EqualTo("'a\\b'"));
            Assert.That(SQLSanitizer.ToStringLiteral("x\\'", dbms), Is.EqualTo("'x\\'''"));
            Assert.That(SQLSanitizer.ToStringLiteral("", dbms), Is.EqualTo("''"));
            Assert.That(SQLSanitizer.ToStringLiteral("棟梁", dbms), Is.EqualTo("'棟梁'"));
        }

        /// <summary>
        /// PostgreSQL は E'～' にして、\ を \\ に、' を '' にする。
        /// **E'～' は standard_conforming_strings によらず \ をエスケープ文字として扱う**ので、設定に依らない。
        /// </summary>
        [Test]
        public void ToStringLiteral_PostgreSQL()
        {
            const DbEnum.DBMSType pg = DbEnum.DBMSType.PstGrS;

            Assert.That(SQLSanitizer.ToStringLiteral("it's", pg), Is.EqualTo("E'it''s'"));
            Assert.That(SQLSanitizer.ToStringLiteral("a\\b", pg), Is.EqualTo("E'a\\\\b'"));
            Assert.That(SQLSanitizer.ToStringLiteral("x\\'", pg), Is.EqualTo("E'x\\\\'''"));
            Assert.That(SQLSanitizer.ToStringLiteral("", pg), Is.EqualTo("E''"));
            Assert.That(SQLSanitizer.ToStringLiteral("棟梁", pg), Is.EqualTo("E'棟梁'"));
        }

        /// <summary>
        /// MySQL は UTF-8 の 16 進リテラル（_utf8mb4 0x～）にする。引用符も \ も使わないので、sql_mode に依らない。
        /// **空文字は 16 進リテラルで書けない**（0x だけは不可）ので '' にする（\ を含まないので安全）。
        /// </summary>
        [Test]
        public void ToStringLiteral_MySQL()
        {
            const DbEnum.DBMSType my = DbEnum.DBMSType.MySQL;

            Assert.That(SQLSanitizer.ToStringLiteral("it's", my), Is.EqualTo("_utf8mb4 0x69742773"));
            Assert.That(SQLSanitizer.ToStringLiteral("x\\'", my), Is.EqualTo("_utf8mb4 0x785C27"));
            Assert.That(SQLSanitizer.ToStringLiteral("棟梁😀", my), Is.EqualTo("_utf8mb4 0xE6A39FE6A281F09F9880"));
            Assert.That(SQLSanitizer.ToStringLiteral("", my), Is.EqualTo("''"));
        }

        /// <summary>MySQL の 16 進リテラルは、どの文字でも UTF-8 のバイト列そのものになる（制御文字・NUL を含む）。</summary>
        [Test]
        public void ToStringLiteral_MySQL_IsUtf8Bytes()
        {
            string value = "a\0b\r\n\t\"`;--é";
            string literal = SQLSanitizer.ToStringLiteral(value, DbEnum.DBMSType.MySQL);

            Assert.That(literal, Does.StartWith("_utf8mb4 0x"));
            string hex = literal.Substring("_utf8mb4 0x".Length);
            Assert.That(hex, Does.Match("^[0-9A-F]+$"));

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo(value));
        }

        /// <summary>HiRDB の文字列リテラルは対応していない（SQLUtility も HiRDB を受けない）。</summary>
        [Test]
        public void ToStringLiteral_HiRDB_NotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => SQLSanitizer.ToStringLiteral("a", DbEnum.DBMSType.HiRDB));
        }

        /// <summary>null は ArgumentNullException（NULL のリテラルにはしない。それは呼ぶ側が決める）。</summary>
        [Test]
        public void ToStringLiteral_Null_Throws()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                () => SQLSanitizer.ToStringLiteral(null, DbEnum.DBMSType.SQLServer));
            Assert.That(ex.ParamName, Is.EqualTo("value"));
        }

        #endregion

        #region 識別子

        /// <summary>
        /// 囲い文字で囲い、**名前に含まれる閉じ側の囲い文字を二重にする。** 開き側（SQL Server の [）はそのまま。
        /// 囲い文字は SQLUtility.OpeningBracket / ClosingBracket と同じ。
        /// </summary>
        [TestCase(DbEnum.DBMSType.SQLServer, "Order Details", "[Order Details]")]
        [TestCase(DbEnum.DBMSType.SQLServer, "a]b[c", "[a]]b[c]")]
        [TestCase(DbEnum.DBMSType.Oracle, "a\"b", "\"a\"\"b\"")]
        [TestCase(DbEnum.DBMSType.DB2, "a\"b", "\"a\"\"b\"")]
        [TestCase(DbEnum.DBMSType.HiRDB, "a\"b", "\"a\"\"b\"")]
        [TestCase(DbEnum.DBMSType.PstGrS, "a\"b", "\"a\"\"b\"")]
        [TestCase(DbEnum.DBMSType.MySQL, "a`b", "`a``b`")]
        [TestCase(DbEnum.DBMSType.MySQL, "a]b\"c", "`a]b\"c`")]
        public void QuoteIdentifier(DbEnum.DBMSType dbms, string name, string expected)
        {
            Assert.That(SQLSanitizer.QuoteIdentifier(name, dbms), Is.EqualTo(expected));
        }

        /// <summary>null は ArgumentNullException。</summary>
        [Test]
        public void QuoteIdentifier_Null_Throws()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                () => SQLSanitizer.QuoteIdentifier(null, DbEnum.DBMSType.SQLServer));
            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        #endregion
    }
}

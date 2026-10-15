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
//* クラス名        ：DamTests
//* クラス日本語名  ：Dam（データ アクセス）のクロス DB 単体テスト
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
using System.Data;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Db;

namespace TestPublicDb
{
    /// <summary>Dam（データ アクセス）のクロス DB 単体テスト</summary>
    /// <remarks>
    /// **同じテストを、データ プロバイダごとに 1 つのフィクスチャとして回す**（TestFixtureSource）。
    ///
    /// ＜接続できないとき＞
    ///   SQL Server … 失敗にする（必須。CI でも接続する）
    ///   それ以外   … スキップする（LocalServicesOnDocker を起動していれば走る）
    ///
    /// ＜表＞
    ///   **既存の表（Northwind 等）は使わない。** 他のテストと疎通の前提が壊れるため。
    ///   フィクスチャごとに一意な名前の表を作り、最後に落とす。行はテストごとに入れ直す。
    ///     ID int PK / NAME 文字列 / QTY int
    ///     (1, 'a', 10) / (2, 'b', 20) / (3, 'c', NULL)
    ///
    /// ＜値の型＞
    ///   Oracle の NUMBER は decimal で返るなど、DBMS で型が違う。数値は Convert で比べる。
    ///   Oracle は空文字を NULL として扱うため、空文字は使わない。
    /// </remarks>
    [TestFixtureSource(typeof(DbProvider), nameof(DbProvider.All))]
    public class DamTests
    {
        private readonly string _dap;
        private BaseDam _dam;
        private string _table;

        /// <summary>コンストラクタ</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        public DamTests(string dap)
        {
            this._dap = dap;
        }

        #region 準備・後始末

        /// <summary>接続して表を作る。接続できなければ、必須でない DBMS はスキップする。</summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            BaseDam dam = DbProvider.CreateDam(this._dap);
            try
            {
                dam.ConnectionOpen(DbProvider.GetConnectionString(this._dap));
            }
            catch (Exception ex)
            {
                if (this._dap == DbProvider.Required)
                {
                    Assert.Fail(this._dap + " に接続できません（必須）: " + ex.GetType().Name + " " + ex.Message);
                }
                Assert.Ignore(this._dap + " に接続できないため、スキップした: " + ex.GetType().Name);
            }

            this._dam = dam;
            this._table = "NUNIT_DAM_" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();

            this.Exec("CREATE TABLE " + this.T + " ("
                + this.Q("ID") + " " + DbProvider.IntType(this._dap) + " NOT NULL, "
                + this.Q("NAME") + " " + DbProvider.StrType(this._dap) + ", "
                + this.Q("QTY") + " " + DbProvider.IntType(this._dap) + ", "
                + "PRIMARY KEY (" + this.Q("ID") + "))");
        }

        /// <summary>表を落として切断する。</summary>
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (this._dam == null) { return; }

            try
            {
                this.Exec("DROP TABLE " + this.T);
            }
            finally
            {
                this._dam.ConnectionClose();
            }
        }

        /// <summary>行を入れ直す。</summary>
        [SetUp]
        public void SetUp()
        {
            this.Exec("DELETE FROM " + this.T);
            this.Exec("INSERT INTO " + this.T + " VALUES (1, 'a', 10)");
            this.Exec("INSERT INTO " + this.T + " VALUES (2, 'b', 20)");
            this.Exec("INSERT INTO " + this.T + " VALUES (3, 'c', NULL)");
        }

        #endregion

        #region 補助

        /// <summary>囲った表名</summary>
        private string T { get { return DbProvider.Quote(this._dap, this._table); } }

        /// <summary>囲った列名</summary>
        private string Q(string name) { return DbProvider.Quote(this._dap, name); }

        /// <summary>パラメタ（先頭記号付き）</summary>
        private string P(string name) { return DbProvider.Sign(this._dap) + name; }

        /// <summary>更新系を実行する。</summary>
        private int Exec(string sql)
        {
            this._dam.SetSqlByCommand(sql);
            return this._dam.ExecInsUpDel_NonQuery();
        }

        /// <summary>件数</summary>
        private int Count(string where = "")
        {
            this._dam.SetSqlByCommand("SELECT COUNT(*) FROM " + this.T + " " + where);
            return Convert.ToInt32(this._dam.ExecSelectScalar());
        }

        #endregion

        #region 参照系

        /// <summary>ExecSelectScalar は 1 行 1 列目の値。</summary>
        [Test]
        public void ExecSelectScalar()
        {
            Assert.That(this.Count(), Is.EqualTo(3));

            this._dam.SetSqlByCommand("SELECT " + this.Q("NAME") + " FROM " + this.T + " WHERE " + this.Q("ID") + " = 2");
            Assert.That(this._dam.ExecSelectScalar(), Is.EqualTo("b"));
        }

        /// <summary>ExecSelectFill_DT は DataTable に詰める。NULL は DBNull。</summary>
        [Test]
        public void ExecSelectFill_DT()
        {
            this._dam.SetSqlByCommand("SELECT " + this.Q("ID") + ", " + this.Q("NAME") + ", " + this.Q("QTY")
                + " FROM " + this.T + " ORDER BY " + this.Q("ID"));

            DataTable dt = new DataTable();
            this._dam.ExecSelectFill_DT(dt);

            Assert.That(dt.Rows.Count, Is.EqualTo(3));
            Assert.That(dt.Columns.Count, Is.EqualTo(3));
            Assert.That(Convert.ToInt32(dt.Rows[0]["ID"]), Is.EqualTo(1));
            Assert.That(dt.Rows[1]["NAME"], Is.EqualTo("b"));
            Assert.That(dt.Rows[2]["QTY"], Is.EqualTo(DBNull.Value));
        }

        /// <summary>ExecSelectFill_DS は DataSet に表を 1 つ詰める。</summary>
        [Test]
        public void ExecSelectFill_DS()
        {
            this._dam.SetSqlByCommand("SELECT * FROM " + this.T);

            DataSet ds = new DataSet();
            this._dam.ExecSelectFill_DS(ds);

            Assert.That(ds.Tables.Count, Is.EqualTo(1));
            Assert.That(ds.Tables[0].Rows.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// ExecSelect_DR はデータ リーダを返す。**閉じるのは呼ぶ側。**
        /// 閉じないと、次のコマンドが使えない DBMS がある（MySQL など）。
        /// </summary>
        [Test]
        public void ExecSelect_DR()
        {
            this._dam.SetSqlByCommand("SELECT " + this.Q("NAME") + " FROM " + this.T + " ORDER BY " + this.Q("ID"));

            List<string> names = new List<string>();
            IDataReader dr = this._dam.ExecSelect_DR();
            try
            {
                while (dr.Read()) { names.Add((string)dr[0]); }
            }
            finally
            {
                dr.Close();
            }

            Assert.That(names, Is.EqualTo(new[] { "a", "b", "c" }));
            Assert.That(this.Count(), Is.EqualTo(3), "リーダを閉じた後に次のコマンドが使える");
        }

        #endregion

        #region 更新系

        /// <summary>パラメタ付きの INSERT。DBNull で NULL を入れられる。戻り値は影響行数。</summary>
        [Test]
        public void Insert_WithParameters()
        {
            this._dam.SetSqlByCommand("INSERT INTO " + this.T + " (" + this.Q("ID") + ", " + this.Q("NAME") + ", " + this.Q("QTY")
                + ") VALUES (" + this.P("P1") + ", " + this.P("P2") + ", " + this.P("P3") + ")");
            this._dam.SetParameter("P1", 10);
            this._dam.SetParameter("P2", "x");
            this._dam.SetParameter("P3", DBNull.Value);

            Assert.That(this._dam.ExecInsUpDel_NonQuery(), Is.EqualTo(1));
            Assert.That(this.Count("WHERE " + this.Q("ID") + " = 10 AND " + this.Q("QTY") + " IS NULL"), Is.EqualTo(1));
        }

        /// <summary>UPDATE / DELETE の戻り値は影響行数。</summary>
        [Test]
        public void UpdateAndDelete_ReturnAffectedRows()
        {
            this._dam.SetSqlByCommand("UPDATE " + this.T + " SET " + this.Q("QTY") + " = " + this.Q("QTY") + " + 1"
                + " WHERE " + this.Q("ID") + " <= " + this.P("P1"));
            this._dam.SetParameter("P1", 2);
            Assert.That(this._dam.ExecInsUpDel_NonQuery(), Is.EqualTo(2));
            Assert.That(this.Count("WHERE " + this.Q("QTY") + " = 21"), Is.EqualTo(1));

            Assert.That(this.Exec("DELETE FROM " + this.T + " WHERE " + this.Q("ID") + " = 3"), Is.EqualTo(1));
            Assert.That(this.Count(), Is.EqualTo(2));
        }

        /// <summary>トランザクションは、ロールバックで取り消され、コミットで残る。</summary>
        [Test]
        public void Transaction_RollbackAndCommit()
        {
            this._dam.BeginTransaction(DbEnum.IsolationLevelEnum.ReadCommitted);
            this.Exec("INSERT INTO " + this.T + " VALUES (4, 'd', 40)");
            this._dam.RollbackTransaction();
            Assert.That(this.Count(), Is.EqualTo(3), "ロールバック");

            this._dam.BeginTransaction(DbEnum.IsolationLevelEnum.ReadCommitted);
            this.Exec("INSERT INTO " + this.T + " VALUES (4, 'd', 40)");
            this._dam.CommitTransaction();
            Assert.That(this.Count(), Is.EqualTo(4), "コミット");
        }

        #endregion

        #region 動的 SQL・ユーザ パラメタ

        /// <summary>動的パラメタライズド クエリを実行する（パラメタを設定した IF だけが効く）。</summary>
        [Test]
        public void DynamicQuery()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><ROOT>"
                + "SELECT COUNT(*) FROM " + this.T + " <WHERE>WHERE "
                + "<IF>AND " + this.Q("QTY") + " = " + this.P("P1") + "</IF>"
                + "</WHERE></ROOT>";

            this._dam.SetSqlByCommand(xml);
            this._dam.SetParameter("P1", 20);
            Assert.That(this._dam.IsDPQ, Is.True);
            Assert.That(Convert.ToInt32(this._dam.ExecSelectScalar()), Is.EqualTo(1));

            this._dam.SetSqlByCommand(xml);
            Assert.That(Convert.ToInt32(this._dam.ExecSelectScalar()), Is.EqualTo(3), "IF が消えて全件");
        }

        /// <summary>ユーザ パラメタ（%名前%）は文字列のまま埋め込む（ORDER BY の切り替えなど）。</summary>
        [Test]
        public void UserParameter()
        {
            this._dam.SetSqlByCommand("SELECT " + this.Q("ID") + " FROM " + this.T + " ORDER BY %COL% DESC");
            this._dam.SetUserParameter("COL", this.Q("ID"));

            DataTable dt = new DataTable();
            this._dam.ExecSelectFill_DT(dt);

            Assert.That(Convert.ToInt32(dt.Rows[0][0]), Is.EqualTo(3));
        }

        #endregion
    }
}

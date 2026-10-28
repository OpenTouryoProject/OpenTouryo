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
//*  2026/10/19  玄人 幸道         プロパティ・分離レベル・ファイルの SQL・型付きパラメタ・LIST タグ・
//*                                ExecGenerateSQL・GetCurrentQuery(ForLog) のケースを追加（カバレッジの拡充）
//*  2026/10/28  玄人 幸道         SQLUtility で組み立てた INSERT / UPDATE を実行し、' と \ を含む値を読み戻すケースを追加
//*  2026/10/28  玄人 幸道         SQL Server でコード ページに無い文字を読み戻すケースを追加（#636）
//*  2026/10/28  玄人 幸道         de-DE で bool と小数を読み戻すケースを追加（#637）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

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

        /// <summary>
        /// Dam 固有のプロパティ（DamSqlConnection / DamOracleCommand など）を、型で探して値を返す。
        /// **プロパティ名は Dam ごとに違う**ので、名前ではなく型で引く。DamIDbCommand（共通）は除く。
        /// </summary>
        private T Prop<T>() where T : class
        {
            List<PropertyInfo> found = new List<PropertyInfo>();
            foreach (PropertyInfo p in this._dam.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (p.Name != "DamIDbCommand" && typeof(T).IsAssignableFrom(p.PropertyType))
                {
                    found.Add(p);
                }
            }

            Assert.That(found.Count, Is.EqualTo(1), typeof(T).Name + " を返すプロパティ");
            return (T)found[0].GetValue(this._dam);
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

        /// <summary>LIST タグは、List&lt;T&gt; の値を IN 句に展開する（P_1, P_2 …）。</summary>
        [Test]
        public void DynamicQuery_List()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><ROOT>"
                + "SELECT COUNT(*) FROM " + this.T + " <WHERE>WHERE "
                + "<LIST>AND " + this.Q("ID") + " IN (" + this.P("PLIST") + ")</LIST>"
                + "</WHERE></ROOT>";

            this._dam.SetSqlByCommand(xml);
            this._dam.SetParameter("PLIST", new List<int> { 1, 3 });

            Assert.That(Convert.ToInt32(this._dam.ExecSelectScalar()), Is.EqualTo(2));
        }

        /// <summary>
        /// 動的 SQL でも、ユーザ パラメタは置き換え、型付きのパラメタは実行時にまとめて設定する。
        /// **動的 SQL のユーザ パラメタは VAL タグで書く**（%名前% ではない）。
        /// </summary>
        [Test]
        public void DynamicQuery_UserParameterAndTypedParameter()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><ROOT>"
                + "SELECT " + this.Q("ID") + " FROM " + this.T + " <WHERE>WHERE "
                + "<IF>AND " + this.Q("QTY") + " >= " + this.P("P1") + "</IF>"
                + "</WHERE> ORDER BY <VAL name=\"COL\"/> DESC</ROOT>";

            this._dam.SetSqlByCommand(xml);
            this._dam.SetParameter("P1", 10, DbProvider.IntDbType(this._dap), -1, ParameterDirection.Input);
            this._dam.SetUserParameter("COL", this.Q("ID"));

            DataTable dt = new DataTable();
            this._dam.ExecSelectFill_DT(dt);

            // QTY が NULL の 3 は外れ、ID の降順
            Assert.That(dt.Rows.Count, Is.EqualTo(2));
            Assert.That(Convert.ToInt32(dt.Rows[0][0]), Is.EqualTo(2));
        }

        #endregion

        #region プロパティ・接続

        /// <summary>
        /// Dam 固有のプロパティは、内部のコネクション・コマンド・アダプタ・トランザクションを返す。
        /// トランザクションは、開始していなければ null。
        /// </summary>
        [Test]
        public void Properties_ReturnProviderObjects()
        {
            this._dam.SetSqlByCommand("SELECT COUNT(*) FROM " + this.T);

            Assert.That(this.Prop<IDbConnection>().State, Is.EqualTo(ConnectionState.Open));
            Assert.That(this.Prop<IDbCommand>(), Is.SameAs(this._dam.DamIDbCommand));
            Assert.That(this.Prop<IDbDataAdapter>(), Is.Not.Null);
            Assert.That(this.Prop<IDbTransaction>(), Is.Null, "トランザクションの外");

            this._dam.BeginTransaction(DbEnum.IsolationLevelEnum.ReadCommitted);
            try
            {
                Assert.That(this.Prop<IDbTransaction>(), Is.Not.Null, "トランザクションの中");
            }
            finally
            {
                this._dam.RollbackTransaction();
            }

            // Oracle だけ：配列バインドの件数（コマンドに委ねる）
            DamManagedOdp odp = this._dam as DamManagedOdp;
            if (odp != null)
            {
                odp.ArrayBindCount = 0;
                Assert.That(odp.ArrayBindCount, Is.EqualTo(0));
            }
        }

        /// <summary>接続していない Dam は、ConnectionClose で何もしない。GetParameter は null。</summary>
        [Test]
        public void NotOpened_CloseAndGetParameter()
        {
            BaseDam dam = DbProvider.CreateDam(this._dap);

            Assert.DoesNotThrow(() => dam.ConnectionClose());
            Assert.That(dam.GetParameter("P1"), Is.Null, "コマンドが無い");
        }

        #endregion

        #region トランザクション（分離レベル）

        /// <summary>
        /// 分離レベルごとの BeginTransaction。**DBMS が持たない分離レベルは ArgumentException。**
        /// NoTransaction はトランザクションを開始せず、Commit / Rollback は何もしない。
        /// </summary>
        [Test]
        public void BeginTransaction_IsolationLevels()
        {
            foreach (DbEnum.IsolationLevelEnum iso in Enum.GetValues(typeof(DbEnum.IsolationLevelEnum)))
            {
                // **SQL Server の Snapshot は回さない。** Northwind はスナップショット分離を許可しておらず
                //   （許可には ALTER DATABASE が要る。共有の DB は変えない）、
                //   しかも分離レベルがセッションに残るので、後のテストまで失敗する。
                if (this._dap == "SQL" && iso == DbEnum.IsolationLevelEnum.Snapshot) { continue; }

                if (!DbProvider.CanBegin(this._dap, iso))
                {
                    Assert.Throws<ArgumentException>(() => this._dam.BeginTransaction(iso), iso.ToString());
                    continue;
                }

                this._dam.BeginTransaction(iso);
                try
                {
                    if (iso == DbEnum.IsolationLevelEnum.NoTransaction)
                    {
                        Assert.That(this.Prop<IDbTransaction>(), Is.Null, iso.ToString());
                        // 開始していないので、何もしない
                        this._dam.CommitTransaction();
                    }
                    else
                    {
                        Assert.That(this.Prop<IDbTransaction>(), Is.Not.Null, iso.ToString());
                    }
                }
                finally
                {
                    this._dam.RollbackTransaction();
                }
            }

            // SQL Server・MySQL は、分離レベルがセッションに残る。既定に戻して、後のテストに持ち込まない。
            //   既定は SQL Server が READ COMMITTED、MySQL が REPEATABLE READ（Oracle・PostgreSQL はトランザクションごと）。
            this._dam.BeginTransaction(this._dap == "MCN"
                ? DbEnum.IsolationLevelEnum.RepeatableRead : DbEnum.IsolationLevelEnum.ReadCommitted);
            this._dam.RollbackTransaction();

            Assert.That(this.Count(), Is.EqualTo(3), "どの回も何も変えていない");
        }

        #endregion

        #region SQL の設定・パラメタ

        /// <summary>SetSqlByFile はファイルの SQL を読む（既定のエンコードは UTF-8）。</summary>
        [Test]
        public void SetSqlByFile()
        {
            string path = Path.Combine(Path.GetTempPath(), "nunit_dam_" + Guid.NewGuid().ToString("N") + ".sql");
            File.WriteAllText(path, "SELECT COUNT(*) FROM " + this.T, new UTF8Encoding(false));
            try
            {
                this._dam.SetSqlByFile(path);
                Assert.That(Convert.ToInt32(this._dam.ExecSelectScalar()), Is.EqualTo(3));

                this._dam.SetSqlByFile(path, CommandType.Text);
                Assert.That(Convert.ToInt32(this._dam.ExecSelectScalar()), Is.EqualTo(3));
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>GetParameter は、設定したパラメタの値を返す。</summary>
        [Test]
        public void GetParameter()
        {
            this._dam.SetSqlByCommand("SELECT COUNT(*) FROM " + this.T + " WHERE " + this.Q("ID") + " = " + this.P("P1"));
            this._dam.SetParameter("P1", 2);

            Assert.That(this._dam.GetParameter("P1"), Is.EqualTo(2));
            Assert.That(Convert.ToInt32(this._dam.ExecSelectScalar()), Is.EqualTo(1));
        }

        /// <summary>
        /// 型（プロバイダ固有の型）・サイズを指定した SetParameter。
        /// FxSqlDotnetTypeInfo を設定していないので、dbTypeInfo はプロバイダ固有の型として扱われる。
        /// </summary>
        [Test]
        public void SetParameter_WithTypeAndSize()
        {
            this._dam.SetSqlByCommand("INSERT INTO " + this.T + " (" + this.Q("ID") + ", " + this.Q("NAME") + ", " + this.Q("QTY")
                + ") VALUES (" + this.P("P1") + ", " + this.P("P2") + ", " + this.P("P3") + ")");
            this._dam.SetParameter("P1", 11, DbProvider.IntDbType(this._dap));
            this._dam.SetParameter("P2", "y", DbProvider.StrDbType(this._dap), 20);
            this._dam.SetParameter("P3", 5, DbProvider.IntDbType(this._dap), -1, ParameterDirection.Input);

            Assert.That(this._dam.ExecInsUpDel_NonQuery(), Is.EqualTo(1));
            Assert.That(this.Count("WHERE " + this.Q("ID") + " = 11 AND " + this.Q("NAME") + " = 'y' AND "
                + this.Q("QTY") + " = 5"), Is.EqualTo(1));
        }

        #endregion

        #region SQL の生成・取得

        /// <summary>
        /// ExecGenerateSQL は、SQL Server ではパラメタを埋め込んだ SQL を返す（実行はしない）。
        /// **Oracle・MySQL・PostgreSQL は実装していない**（NotImplementedException）。
        /// </summary>
        [Test]
        public void ExecGenerateSQL()
        {
            this._dam.SetSqlByCommand("SELECT COUNT(*) FROM " + this.T + " WHERE " + this.Q("ID") + " = " + this.P("P1"));
            this._dam.SetParameter("P1", 2);
            SQLUtility util = new SQLUtility(DbEnum.DBMSType.SQLServer);

            if (this._dap == "SQL")
            {
                string sql = this._dam.ExecGenerateSQL(util);
                Assert.That(sql, Does.Contain("= 2").And.Not.Contain(this.P("P1")));
            }
            else
            {
                Assert.Throws<NotImplementedException>(() => this._dam.ExecGenerateSQL(util));
            }
        }

        /// <summary>
        /// GetCurrentQuery は設定した SQL を返す。
        /// GetCurrentQueryForLog は、FxSqlTraceLog が ON なら SQL とパラメタ（null・DBNull を含む）を返す。
        /// </summary>
        [Test]
        public void GetCurrentQuery_AndForLog()
        {
            string sql = "SELECT COUNT(*) FROM " + this.T + " WHERE " + this.Q("ID") + " = " + this.P("P1");
            this._dam.SetSqlByCommand(sql);
            this._dam.SetParameter("P1", 2);
            this._dam.SetParameter("P2", DBNull.Value);
            this._dam.SetParameter("P3", null);

            Assert.That(this._dam.GetCurrentQuery(), Is.EqualTo(sql));

            string log = this._dam.GetCurrentQueryForLog();
            Assert.That(log, Does.Contain("P1=2").And.Contain("P2=DBNull").And.Contain("P3=null"));
        }

        #endregion

        #region SQLUtility で組み立てた SQL の実行

        /// <summary>' と \ を含む値（NAME 列は 20 文字まで。Oracle は空文字を NULL にするので使わない）</summary>
        private static readonly string[] TrickyNames = { "it's", "C:\\new\\table", "x\\", "a\\'b", "\\\\'--" };

        /// <summary>ID → NAME を読む。</summary>
        private string Name(int id)
        {
            this._dam.SetSqlByCommand("SELECT " + this.Q("NAME") + " FROM " + this.T + " WHERE " + this.Q("ID") + " = " + id);
            return Convert.ToString(this._dam.ExecSelectScalar());
        }

        /// <summary>
        /// SQLUtility.GetInsertSQLParts / GetUpdateSQLParts で組み立てた SQL を実行し、**同じ値で読み戻せる。**
        /// 値は文字列リテラルとして SQL に埋め込まれるので、' と \ の扱いが DBMS の規則と合っていないと、
        /// 値が変わるか、リテラルが閉じずに SQL が壊れる（MySQL は既定で \ をエスケープ文字として扱う）。
        /// </summary>
        [Test]
        public void SQLUtility_InsertAndUpdate_RoundTrip()
        {
            SQLUtility util = new SQLUtility(DbProvider.GetDbmsType(this._dap));
            int n = TrickyNames.Length;

            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("QTY", typeof(int));
            for (int i = 0; i < n; i++)
            {
                dt.Rows.Add(100 + i, TrickyNames[i], i);
            }

            // INSERT（1 行 1 文）
            string[] ins = util.GetInsertSQLParts(dt);
            for (int i = 1; i < ins.Length; i++)
            {
                Assert.That(this.Exec("INSERT INTO " + this.T + " " + ins[0] + " VALUES " + ins[i]), Is.EqualTo(1));
            }
            for (int i = 0; i < n; i++)
            {
                Assert.That(this.Name(100 + i), Is.EqualTo(TrickyNames[i]), "INSERT " + i);
            }

            // UPDATE（値を逆順に入れ替える。PostgreSQL・MySQL は 1 文、他は 1 行 1 文）
            for (int i = 0; i < n; i++)
            {
                dt.Rows[i]["NAME"] = TrickyNames[n - 1 - i];
            }
            foreach (string part in util.GetUpdateSQLParts(dt, new[] { "ID" }))
            {
                this.Exec("UPDATE " + this.T + " " + part);
            }
            for (int i = 0; i < n; i++)
            {
                Assert.That(this.Name(100 + i), Is.EqualTo(TrickyNames[n - 1 - i]), "UPDATE " + i);
            }
        }

        /// <summary>
        /// SQL Server で、照合順序のコード ページに無い文字（アクセント付き文字・絵文字・ハングル）が**化けずに読み戻せる。**
        /// N の無いリテラルはコード ページの文字列として解釈され、無い文字が ? などに置き換わる（#636）。
        /// 他の DBMS は、列の文字セットが DB の設定に依るので対象外。
        /// </summary>
        [Test]
        public void SQLUtility_SQLServer_KeepsNonCodePageChars()
        {
            if (DbProvider.GetDbmsType(this._dap) != DbEnum.DBMSType.SQLServer)
            {
                Assert.Ignore("SQL Server だけが対象");
            }

            const string value = "café 😀 한";
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("QTY", typeof(int));
            dt.Rows.Add(100, value, 0);

            string[] ins = new SQLUtility(DbEnum.DBMSType.SQLServer).GetInsertSQLParts(dt);
            Assert.That(this.Exec("INSERT INTO " + this.T + " " + ins[0] + " VALUES " + ins[1]), Is.EqualTo(1));
            Assert.That(this.Name(100), Is.EqualTo(value));
        }

        /// <summary>
        /// 小数点が , のカルチャ（de-DE）で、bool と小数を SQLUtility の INSERT / UPDATE で入れ、**同じ値で読み戻せる。**
        /// 以前は 1.5 が 1,5 に（値の数がずれる）、bool が True / False になっていた（#637）。
        /// bool の列の型は DBMS ごとに違う（SQL Server は bit、Oracle は NUMBER(1)、MySQL・PostgreSQL は boolean）。
        /// </summary>
        [Test]
        [SetCulture("de-DE")]
        public void SQLUtility_BoolAndDecimal_DeDE()
        {
            string boolType;
            switch (this._dap)
            {
                case "SQL": boolType = "bit"; break;
                case "ODP": boolType = "NUMBER(1)"; break;
                default: boolType = "boolean"; break;
            }
            string decType = this._dap == "ODP" ? "NUMBER(10,4)" : "decimal(10,4)";

            string t = DbProvider.Quote(this._dap, this._table + "_B");
            this.Exec("CREATE TABLE " + t + " ("
                + this.Q("ID") + " " + DbProvider.IntType(this._dap) + " NOT NULL, "
                + this.Q("FLAG") + " " + boolType + ", "
                + this.Q("PRICE") + " " + decType + ", "
                + "PRIMARY KEY (" + this.Q("ID") + "))");
            try
            {
                SQLUtility util = new SQLUtility(DbProvider.GetDbmsType(this._dap));

                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("FLAG", typeof(bool));
                dt.Columns.Add("PRICE", typeof(decimal));
                dt.Rows.Add(1, true, 1.5m);
                dt.Rows.Add(2, false, -0.25m);

                string[] ins = util.GetInsertSQLParts(dt);
                for (int i = 1; i < ins.Length; i++)
                {
                    Assert.That(this.Exec("INSERT INTO " + t + " " + ins[0] + " VALUES " + ins[i]), Is.EqualTo(1));
                }
                this.AssertFlagAndPrice(t, 1, true, 1.5m);
                this.AssertFlagAndPrice(t, 2, false, -0.25m);

                // UPDATE（値を入れ替える。PostgreSQL・MySQL は CASE の 1 文、他は 1 行 1 文）
                dt.Rows[0]["FLAG"] = false; dt.Rows[0]["PRICE"] = 123.4567m;
                dt.Rows[1]["FLAG"] = true; dt.Rows[1]["PRICE"] = 0.5m;
                foreach (string part in util.GetUpdateSQLParts(dt, new[] { "ID" }))
                {
                    this.Exec("UPDATE " + t + " " + part);
                }
                this.AssertFlagAndPrice(t, 1, false, 123.4567m);
                this.AssertFlagAndPrice(t, 2, true, 0.5m);
            }
            finally
            {
                this.Exec("DROP TABLE " + t);
            }
        }

        /// <summary>ID の行の FLAG と PRICE を比べる（型は DBMS で違うので Convert で揃える）。</summary>
        private void AssertFlagAndPrice(string t, int id, bool flag, decimal price)
        {
            this._dam.SetSqlByCommand("SELECT " + this.Q("FLAG") + " FROM " + t + " WHERE " + this.Q("ID") + " = " + id);
            Assert.That(Convert.ToBoolean(this._dam.ExecSelectScalar()), Is.EqualTo(flag), "FLAG " + id);

            this._dam.SetSqlByCommand("SELECT " + this.Q("PRICE") + " FROM " + t + " WHERE " + this.Q("ID") + " = " + id);
            Assert.That(Convert.ToDecimal(this._dam.ExecSelectScalar()), Is.EqualTo(price), "PRICE " + id);
        }

        #endregion
    }
}

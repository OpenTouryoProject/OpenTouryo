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
//* クラス名        ：TestDataAccessUpdate
//* クラス日本語名  ：更新系のテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/06  玄人 幸道         新規作成（#520）
//**********************************************************************************

using System;
using System.Data;

using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestDataAccess
{
    /// <summary>更新系のテスト</summary>
    /// <remarks>
    /// INSERT・UPDATE・DELETE とトランザクションを、対象 DBMS ごとに確認する（#520）。
    /// あわせて **SQLUtility が生成した SQL を実際に実行**し、結果まで検証する。
    ///
    /// ＜専用の表を作る理由＞
    ///   Northwind の表を更新すると、他のテスト（SimpleBatch・3_SmokeTest.ps1）の前提が壊れる。
    ///   このテストは自分で表を作り、最後に落とす。**既存のデータには一切触れない。**
    ///
    /// ＜SQLUtility を実 DB で確認する理由＞
    ///   #515 は「生成された SQL が構文として妥当でも、**誤った行に誤った値が入る**」
    ///   という不具合だった。生成結果の目視だけでは、この種の誤りは見つけにくい。
    ///   ここでは更新対象外の行を 1 件混ぜ、**無傷であること**まで確認する。
    /// </remarks>
    public class TestDataAccessUpdate
    {
        #region 定数

        /// <summary>テスト用の表名</summary>
        /// <remarks>既存の表と衝突しない名前にすること。</remarks>
        private const string TableName = "TestOrders";

        #endregion

        #region public

        /// <summary>Root</summary>
        /// <param name="daps">対象のデータ プロバイダ</param>
        public static void Root(string[] daps)
        {
            foreach (string dap in daps)
            {
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestDataAccessUpdate.TestOneProvider(dap);
            }
        }

        #endregion

        #region private

        /// <summary>データ プロバイダ 1 つ分のテスト</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        private static void TestOneProvider(string dap)
        {
            MyDebug.OutputDebugAndConsole("[" + dap + "]");

            BaseDam dam = DataProvider.CreateDam(dap);

            if (dam == null)
            {
                MyDebug.OutputDebugAndConsole("- 未対応のデータ プロバイダのため、実行しない。");
                return;
            }

            try
            {
                dam.ConnectionOpen(DataProvider.GetConnectionString(dap));

                try
                {
                    // DDL は Oracle では暗黙にコミットされるため、トランザクションの外で行う。
                    TestDataAccessUpdate.DropTable(dam, dap);
                    TestDataAccessUpdate.CreateTable(dam, dap);

                    TestDataAccessUpdate.TestInsert(dam, dap);
                    TestDataAccessUpdate.TestUpdateBySQLUtility(dam, dap);
                    TestDataAccessUpdate.TestTransaction(dam, dap);
                    TestDataAccessUpdate.TestDelete(dam, dap);
                }
                finally
                {
                    // 失敗しても表を残さない。
                    TestDataAccessUpdate.DropTable(dam, dap);
                    dam.ConnectionClose();
                }
            }
            catch (Exception ex)
            {
                // メッセージは OS の表示言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole("- 例外 : " + ex.GetType().FullName);
            }
        }

        #region 各テスト

        /// <summary>INSERT（SQLUtility が生成したパーツを実行する）</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        private static void TestInsert(BaseDam dam, string dap)
        {
            // 3 件目は更新対象に含めない。UPDATE で無傷であることを確認するため。
            DataTable dt = TestDataAccessUpdate.CreateSourceTable(true);

            SQLUtility util = new SQLUtility(DataProvider.GetDbmsType(dap));
            string[] parts = util.GetInsertSQLParts(dt);

            // parts[0] が列リスト、parts[1] 以降が値の並び。
            int total = 0;
            for (int i = 1; i < parts.Length; i++)
            {
                dam.SetSqlByCommand(
                    "INSERT INTO " + TestDataAccessUpdate.Table(dap)
                    + " " + parts[0] + " VALUES " + parts[i]);
                total += dam.ExecInsUpDel_NonQuery();
            }

            MyDebug.OutputDebugAndConsole("- INSERT（GetInsertSQLParts）  : " + total + " 件");
            TestDataAccessUpdate.OutputRows(dam, dap, "  投入後");
        }

        /// <summary>UPDATE（SQLUtility が生成したパーツを実行する）</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <remarks>
        /// #515 の再発を実 DB で検知する。
        /// 更新するのは (1,10) と (2,20) の 2 件だけで、(3,30) は対象外。
        /// 複合主キーの扱いを誤ると、対象外の行が巻き込まれるか、値が入れ替わる。
        /// </remarks>
        private static void TestUpdateBySQLUtility(BaseDam dam, string dap)
        {
            DataTable dt = TestDataAccessUpdate.CreateSourceTable(false);

            SQLUtility util = new SQLUtility(DataProvider.GetDbmsType(dap));
            string[] parts = util.GetUpdateSQLParts(dt, new string[] { "OrderID", "ProductID" });

            int total = 0;
            foreach (string part in parts)
            {
                dam.SetSqlByCommand("UPDATE " + TestDataAccessUpdate.Table(dap) + " " + part);
                total += dam.ExecInsUpDel_NonQuery();
            }

            MyDebug.OutputDebugAndConsole("- UPDATE（GetUpdateSQLParts）  : " + total + " 件");
            TestDataAccessUpdate.OutputRows(dam, dap, "  更新後");
        }

        /// <summary>トランザクション（ロールバックとコミット）</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        private static void TestTransaction(BaseDam dam, string dap)
        {
            string table = TestDataAccessUpdate.Table(dap);

            // ロールバック : 全件消してから戻す。件数が元に戻ること。
            dam.BeginTransaction(DbEnum.IsolationLevelEnum.ReadCommitted);
            dam.SetSqlByCommand("DELETE FROM " + table);
            dam.ExecInsUpDel_NonQuery();
            dam.RollbackTransaction();
            MyDebug.OutputDebugAndConsole("- ロールバック後の件数        : " + TestDataAccessUpdate.Count(dam, dap));

            // コミット : 1 件消して確定する。件数が減ったままであること。
            dam.BeginTransaction(DbEnum.IsolationLevelEnum.ReadCommitted);
            dam.SetSqlByCommand(
                "DELETE FROM " + table
                + " WHERE " + DataProvider.Quote(dap, "OrderID") + " = 3");
            dam.ExecInsUpDel_NonQuery();
            dam.CommitTransaction();
            MyDebug.OutputDebugAndConsole("- コミット後の件数            : " + TestDataAccessUpdate.Count(dam, dap));
        }

        /// <summary>DELETE</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        private static void TestDelete(BaseDam dam, string dap)
        {
            dam.SetSqlByCommand("DELETE FROM " + TestDataAccessUpdate.Table(dap));
            MyDebug.OutputDebugAndConsole("- DELETE                      : " + dam.ExecInsUpDel_NonQuery() + " 件");
            MyDebug.OutputDebugAndConsole("- 削除後の件数                : " + TestDataAccessUpdate.Count(dam, dap));
        }

        #endregion

        #region 表の作成と破棄

        /// <summary>テスト用の表を作る</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <remarks>
        /// **囲い文字は SQLUtility が生成するものと必ず一致させる。**
        /// Oracle は囲わないと大文字、PostgreSQL は囲わないと小文字に畳まれるため、
        /// 囲わずに作ると、生成された SQL の "Qty" などが「存在しない列」になる。
        /// </remarks>
        private static void CreateTable(BaseDam dam, string dap)
        {
            string intType;
            string strType;

            switch (dap)
            {
                case "ODP":
                    intType = "NUMBER(10)";
                    strType = "VARCHAR2(10)";
                    break;

                case "MCN":
                    intType = "INT";
                    strType = "VARCHAR(10)";
                    break;

                case "NPS":
                    intType = "integer";
                    strType = "varchar(10)";
                    break;

                default:
                    intType = "int";
                    strType = "nvarchar(10)";
                    break;
            }

            dam.SetSqlByCommand(
                "CREATE TABLE " + TestDataAccessUpdate.Table(dap) + " ("
                + DataProvider.Quote(dap, "OrderID") + " " + intType + " NOT NULL, "
                + DataProvider.Quote(dap, "ProductID") + " " + intType + " NOT NULL, "
                + DataProvider.Quote(dap, "Qty") + " " + intType + ", "
                + DataProvider.Quote(dap, "Note") + " " + strType + ", "
                + "PRIMARY KEY (" + DataProvider.Quote(dap, "OrderID")
                + ", " + DataProvider.Quote(dap, "ProductID") + "))");

            dam.ExecInsUpDel_NonQuery();
        }

        /// <summary>テスト用の表を落とす</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <remarks>
        /// 存在しない場合の例外は無視する。
        /// DROP TABLE IF EXISTS は Oracle では 23ai 以降でしか使えないため、
        /// 分岐せずに「投げて握る」形にしている。
        /// </remarks>
        private static void DropTable(BaseDam dam, string dap)
        {
            try
            {
                dam.SetSqlByCommand("DROP TABLE " + TestDataAccessUpdate.Table(dap));
                dam.ExecInsUpDel_NonQuery();
            }
            catch
            {
                // 表が無いだけ。
            }
        }

        #endregion

        #region ヘルパ

        /// <summary>囲った表名</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <returns>囲った表名</returns>
        private static string Table(string dap)
        {
            return DataProvider.Quote(dap, TestDataAccessUpdate.TableName);
        }

        /// <summary>件数を取得する</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <returns>件数</returns>
        private static int Count(BaseDam dam, string dap)
        {
            dam.SetSqlByCommand("SELECT COUNT(*) FROM " + TestDataAccessUpdate.Table(dap));
            return Convert.ToInt32(dam.ExecSelectScalar());
        }

        /// <summary>全行を出力する</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <param name="caption">見出し</param>
        /// <remarks>
        /// DBMS によって数値の型が異なる（Oracle の NUMBER は decimal になる）ため、
        /// 明示的に変換してから出力する。並び順も固定する。
        /// </remarks>
        private static void OutputRows(BaseDam dam, string dap, string caption)
        {
            DataTable dt = new DataTable();

            dam.SetSqlByCommand(
                "SELECT " + DataProvider.Quote(dap, "OrderID")
                + ", " + DataProvider.Quote(dap, "ProductID")
                + ", " + DataProvider.Quote(dap, "Qty")
                + ", " + DataProvider.Quote(dap, "Note")
                + " FROM " + TestDataAccessUpdate.Table(dap)
                + " ORDER BY " + DataProvider.Quote(dap, "OrderID"));

            dam.ExecSelectFill_DT(dt);

            foreach (DataRow dr in dt.Rows)
            {
                MyDebug.OutputDebugAndConsole(
                    caption + " : "
                    + Convert.ToInt32(dr["OrderID"]) + ", "
                    + Convert.ToInt32(dr["ProductID"]) + ", "
                    + Convert.ToInt32(dr["Qty"]) + ", "
                    + Convert.ToString(dr["Note"]));
            }
        }

        /// <summary>テスト データの DataTable を生成する</summary>
        /// <param name="isForInsert">投入用（3 件）か、更新用（2 件）か</param>
        /// <returns>DataTable</returns>
        /// <remarks>
        /// 投入用の 3 件目 (3,30) は更新対象に含めない。
        /// 更新後も 777,'z' のままであることで、WHERE 句が正しいと分かる。
        /// </remarks>
        private static DataTable CreateSourceTable(bool isForInsert)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("OrderID", typeof(object));
            dt.Columns.Add("ProductID", typeof(object));
            dt.Columns.Add("Qty", typeof(object));
            dt.Columns.Add("Note", typeof(object));

            if (isForInsert)
            {
                TestDataAccessUpdate.AddRow(dt, 1, 10, 999, "x");
                TestDataAccessUpdate.AddRow(dt, 2, 20, 888, "y");
                TestDataAccessUpdate.AddRow(dt, 3, 30, 777, "z");
            }
            else
            {
                TestDataAccessUpdate.AddRow(dt, 1, 10, 100, "a");
                TestDataAccessUpdate.AddRow(dt, 2, 20, 200, "b");
            }

            return dt;
        }

        /// <summary>DataTable に 1 行足す</summary>
        /// <param name="dt">DataTable</param>
        /// <param name="orderId">OrderID</param>
        /// <param name="productId">ProductID</param>
        /// <param name="qty">Qty</param>
        /// <param name="note">Note</param>
        private static void AddRow(DataTable dt, int orderId, int productId, int qty, string note)
        {
            DataRow dr = dt.NewRow();

            dr["OrderID"] = orderId;
            dr["ProductID"] = productId;
            dr["Qty"] = qty;
            dr["Note"] = note;

            dt.Rows.Add(dr);
        }

        #endregion

        #endregion
    }
}

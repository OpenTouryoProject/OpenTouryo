using System;
using System.Data;

using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>SQLUtilityのテスト</summary>
    /// <remarks>
    /// 生成されるSQLは、目視しないと誤りに気付けない。
    /// 特にPostgreSQL・MySQL向けの一括UPDATE（CASE ... WHEN ... THEN）は、
    /// 複合主キーで誤った行に誤った値を書き込む不具合があった（#515）。
    /// このため、生成結果を結果ファイルに残して比較できるようにする。
    /// </remarks>
    public class TestSQLUtility
    {
        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            TestSQLUtility.TestGetUpdateSQLParts();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestSQLUtility.TestGetInsertSQLParts();
        }

        #endregion

        #region private

        /// <summary>Update系SQLのパーツ生成のテスト</summary>
        private static void TestGetUpdateSQLParts()
        {
            MyDebug.OutputDebugAndConsole("SQLUtility.GetUpdateSQLParts");

            // 単一主キー
            // ・PostgreSQL・MySQLは1文にまとめ、それ以外は1行1文になる。
            DataTable dtSingle = TestSQLUtility.CreateOrderTable(false);
            TestSQLUtility.OutputUpdateSQLParts("単一主キー", dtSingle, new string[] { "OrderID" });

            // 複合主キー
            // ・WHENとWHEREが主キーの「組み合わせ」になっていること。
            // ・列ごとに独立していると、WHEREが直積になって対象外の行に一致し、
            //   CASEも先に一致した枝が勝つため、誤った行に誤った値が入る（#515）。
            DataTable dtComposite = TestSQLUtility.CreateOrderTable(true);
            TestSQLUtility.OutputUpdateSQLParts("複合主キー", dtComposite, new string[] { "OrderID", "ProductID" });

            // 更新対象列が無い場合（全列が主キー）
            // ・SET句が空のSQLは構文エラーになるため、生成しないこと。
            DataTable dtAllPk = new DataTable();
            dtAllPk.Columns.Add("OrderID", typeof(object));
            DataRow drAllPk = dtAllPk.NewRow();
            drAllPk["OrderID"] = 1;
            dtAllPk.Rows.Add(drAllPk);
            TestSQLUtility.OutputUpdateSQLParts("更新対象列なし", dtAllPk, new string[] { "OrderID" });

            // 主キーが無い場合
            // ・WHERE句が無いと全行更新になるため、生成しないこと。
            TestSQLUtility.OutputUpdateSQLParts("主キーなし", dtSingle, new string[] { });
        }

        /// <summary>Insert系SQLのパーツ生成のテスト</summary>
        private static void TestGetInsertSQLParts()
        {
            MyDebug.OutputDebugAndConsole("SQLUtility.GetInsertSQLParts");

            DataTable dt = TestSQLUtility.CreateOrderTable(true);

            foreach (DbEnum.DBMSType dbms in TestSQLUtility.GetTargetDbms())
            {
                MyDebug.OutputDebugAndConsole("- " + dbms.ToString());

                string[] parts = (new SQLUtility(dbms)).GetInsertSQLParts(dt);
                foreach (string part in parts)
                {
                    MyDebug.OutputDebugAndConsole(part);
                }
            }
        }

        /// <summary>Update系SQLのパーツを生成して出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="dt">入力DataTable</param>
        /// <param name="primaryKeys">主キー情報</param>
        private static void OutputUpdateSQLParts(string caseName, DataTable dt, string[] primaryKeys)
        {
            MyDebug.OutputDebugAndConsole("[" + caseName + "]");

            foreach (DbEnum.DBMSType dbms in TestSQLUtility.GetTargetDbms())
            {
                MyDebug.OutputDebugAndConsole("- " + dbms.ToString());

                string[] parts = (new SQLUtility(dbms)).GetUpdateSQLParts(dt, primaryKeys);

                if (parts.Length == 0)
                {
                    // 生成しないケース（更新対象列なし・主キーなし）
                    MyDebug.OutputDebugAndConsole("(生成なし)");
                }
                else
                {
                    foreach (string part in parts)
                    {
                        // 呼び出し側が "UPDATE <テーブル名>" を前置して1文にする。
                        MyDebug.OutputDebugAndConsole("UPDATE Orders " + part);
                    }
                }
            }
        }

        /// <summary>テスト対象のDBMS</summary>
        /// <returns>DBMSの種類の配列</returns>
        /// <remarks>
        /// HiRDBは囲い文字などの実装が他と同じため、代表としてSQLServerを見る。
        /// PostgreSQL・MySQLはCASE式を使う別実装のため、両方を対象にする。
        /// </remarks>
        private static DbEnum.DBMSType[] GetTargetDbms()
        {
            return new DbEnum.DBMSType[]
            {
                DbEnum.DBMSType.SQLServer,
                DbEnum.DBMSType.Oracle,
                DbEnum.DBMSType.PstGrS,
                DbEnum.DBMSType.MySQL
            };
        }

        /// <summary>テスト用のDataTableを生成する</summary>
        /// <param name="isComposite">主キーを複合にするか</param>
        /// <returns>DataTable</returns>
        private static DataTable CreateOrderTable(bool isComposite)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("OrderID", typeof(object));
            if (isComposite)
            {
                dt.Columns.Add("ProductID", typeof(object));
            }
            dt.Columns.Add("Qty", typeof(object));
            dt.Columns.Add("Note", typeof(object));

            DataRow dr1 = dt.NewRow();
            dr1["OrderID"] = 1;
            if (isComposite)
            {
                dr1["ProductID"] = 10;
            }
            dr1["Qty"] = 100;
            dr1["Note"] = "a";
            dt.Rows.Add(dr1);

            DataRow dr2 = dt.NewRow();
            dr2["OrderID"] = 2;
            if (isComposite)
            {
                dr2["ProductID"] = 20;
            }
            dr2["Qty"] = 200;
            dr2["Note"] = "b";
            dt.Rows.Add(dr2);

            return dt;
        }

        #endregion
    }
}

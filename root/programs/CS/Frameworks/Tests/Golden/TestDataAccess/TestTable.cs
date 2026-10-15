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
//* クラス名        ：TestTable
//* クラス日本語名  ：テスト用の表
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/06  玄人 幸道         新規作成（#520）
//**********************************************************************************

using System;

using Touryo.Infrastructure.Public.Db;

namespace TestDataAccess
{
    /// <summary>テスト用の表</summary>
    /// <remarks>
    /// 更新系と動的パラメタライズドクエリのテストで共有する。
    ///
    /// **既存の表（Northwind 等）は使わない。** 更新すると SimpleBatch や
    /// 3_SmokeTest.ps1 の前提が壊れるため、テストのたびに作って落とす。
    /// </remarks>
    public class TestTable
    {
        /// <summary>表名</summary>
        /// <remarks>既存の表と衝突しない名前にすること。</remarks>
        public const string Name = "TestOrders";

        /// <summary>囲った表名</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <returns>囲った表名</returns>
        public static string Quoted(string dap)
        {
            return DataProvider.Quote(dap, TestTable.Name);
        }

        /// <summary>表を作る</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <remarks>
        /// **囲い文字は SQLUtility が生成するものと必ず一致させる。**
        /// Oracle は囲わないと大文字、PostgreSQL は囲わないと小文字に畳まれるため、
        /// 囲わずに作ると、生成された SQL の "Qty" などが「存在しない列」になる。
        /// </remarks>
        public static void Create(BaseDam dam, string dap)
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
                "CREATE TABLE " + TestTable.Quoted(dap) + " ("
                + DataProvider.Quote(dap, "OrderID") + " " + intType + " NOT NULL, "
                + DataProvider.Quote(dap, "ProductID") + " " + intType + " NOT NULL, "
                + DataProvider.Quote(dap, "Qty") + " " + intType + ", "
                + DataProvider.Quote(dap, "Note") + " " + strType + ", "
                + "PRIMARY KEY (" + DataProvider.Quote(dap, "OrderID")
                + ", " + DataProvider.Quote(dap, "ProductID") + "))");

            dam.ExecInsUpDel_NonQuery();
        }

        /// <summary>表を落とす</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <remarks>
        /// 存在しない場合の例外は無視する。
        /// DROP TABLE IF EXISTS は Oracle では 23ai 以降でしか使えないため、
        /// 分岐せずに「投げて握る」形にしている。
        /// </remarks>
        public static void Drop(BaseDam dam, string dap)
        {
            try
            {
                dam.SetSqlByCommand("DROP TABLE " + TestTable.Quoted(dap));
                dam.ExecInsUpDel_NonQuery();
            }
            catch
            {
                // 表が無いだけ。
            }
        }

        /// <summary>1 行追加する</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <param name="orderId">OrderID</param>
        /// <param name="productId">ProductID</param>
        /// <param name="qty">Qty</param>
        /// <param name="note">Note（null を渡すと NULL を入れる）</param>
        /// <remarks>
        /// Note に NULL を入れられるようにしてあるのは、動的 SQL の &lt;ELSE&gt;
        /// （IS NULL 側）を確認するため。
        /// </remarks>
        public static void InsertRow(
            BaseDam dam, string dap, int orderId, int productId, int qty, string note)
        {
            string noteValue = (note == null) ? "NULL" : "'" + note + "'";

            dam.SetSqlByCommand(
                "INSERT INTO " + TestTable.Quoted(dap)
                + " (" + DataProvider.Quote(dap, "OrderID")
                + ", " + DataProvider.Quote(dap, "ProductID")
                + ", " + DataProvider.Quote(dap, "Qty")
                + ", " + DataProvider.Quote(dap, "Note") + ")"
                + " VALUES (" + orderId + ", " + productId + ", " + qty + ", " + noteValue + ")");

            dam.ExecInsUpDel_NonQuery();
        }

        /// <summary>件数を取得する</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <returns>件数</returns>
        public static int Count(BaseDam dam, string dap)
        {
            dam.SetSqlByCommand("SELECT COUNT(*) FROM " + TestTable.Quoted(dap));
            return Convert.ToInt32(dam.ExecSelectScalar());
        }
    }
}

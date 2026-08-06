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
//* クラス名        ：TestDto
//* クラス日本語名  ：Public.Dtoのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/06  玄人 幸道         新規作成（#522）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Data;

using Touryo.Infrastructure.Public.Diagnostics;
using Touryo.Infrastructure.Public.Dto;

namespace TestCode
{
    /// <summary>Public.Dtoのテスト</summary>
    /// <remarks>
    /// 項目移送（DataToPoco / PocoToPoco / DataToDictionary）と、
    /// マーシャリング可能な自前 DataTable（DTTable）を確認する（#522）。
    ///
    /// ＜なぜここを優先するか＞
    ///   自動生成 Dao の項目移送の中核で、壊れると DTO 全体に波及する。
    ///   **突合はプロパティ名で行う**ため、名前が変わると
    ///   **例外にならずに値が入らない**。件数や型では気付けない。
    ///   実際、#293 では「PK_ 接頭辞を付けた EntityTemplate」が
    ///   この方式と噛み合わず、既定のテンプレートから外された経緯がある。
    /// </remarks>
    public class TestDto
    {
        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            TestDto.TestDataToPoco();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestDto.TestPocoToPoco();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestDto.TestDataToDictionary();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestDto.TestDTTable();
        }

        #endregion

        #region テスト用の POCO

        /// <summary>移送先の POCO</summary>
        /// <remarks>Qty は DataTable 側に列が無い。既定値のままになること。</remarks>
        public class OrderPoco
        {
            /// <summary>OrderID</summary>
            public int OrderID { get; set; }

            /// <summary>Note</summary>
            public string Note { get; set; }

            /// <summary>Qty</summary>
            public int Qty { get; set; }
        }

        /// <summary>移送元の POCO</summary>
        public class OrderSource
        {
            /// <summary>OrderID</summary>
            public int OrderID { get; set; }

            /// <summary>Note</summary>
            public string Note { get; set; }

            /// <summary>Memo（移送先には同名が無い）</summary>
            public string Memo { get; set; }
        }

        #endregion

        #region private

        /// <summary>DataToPoco のテスト</summary>
        private static void TestDataToPoco()
        {
            MyDebug.OutputDebugAndConsole("DataToPoco");

            // --- 名前が一致する場合 ---
            // ・一致した列だけ値が入る
            // ・POCO にしか無い Qty は既定値のまま
            // ・DataTable にしか無い Extra は無視される
            // ・DBNull は「指定しない」＝既定値のまま
            DataTable dt = new DataTable();
            dt.Columns.Add("OrderID", typeof(int));
            dt.Columns.Add("Note", typeof(string));
            dt.Columns.Add("Extra", typeof(string));

            DataRow dr1 = dt.NewRow();
            dr1["OrderID"] = 1;
            dr1["Note"] = "x";
            dr1["Extra"] = "無視される";
            dt.Rows.Add(dr1);

            DataRow dr2 = dt.NewRow();
            dr2["OrderID"] = 2;
            dr2["Note"] = DBNull.Value;
            dr2["Extra"] = "無視される";
            dt.Rows.Add(dr2);

            MyDebug.OutputDebugAndConsole("[DataTableToList : 名前が一致]");
            TestDto.OutputPocoList(DataToPoco.DataTableToList<OrderPoco>(dt));

            // --- 先頭行だけを取る ---
            MyDebug.OutputDebugAndConsole("[DataTableToPOCO : 先頭行だけ]");
            TestDto.OutputPoco(DataToPoco.DataTableToPOCO<OrderPoco>(dt));

            // --- 名前が一致しない場合 ---
            // **例外にならず、値が入らないだけ。** ここが気付きにくい。
            DataTable dtPk = new DataTable();
            dtPk.Columns.Add("PK_OrderID", typeof(int));
            dtPk.Columns.Add("Note", typeof(string));

            DataRow drPk = dtPk.NewRow();
            drPk["PK_OrderID"] = 9;
            drPk["Note"] = "y";
            dtPk.Rows.Add(drPk);

            MyDebug.OutputDebugAndConsole("[マップ無 : 列名が PK_OrderID]");
            TestDto.OutputPocoList(DataToPoco.DataTableToList<OrderPoco>(dtPk));

            // --- マップで対応付ける場合 ---
            // マップの向きは「POCO のプロパティ名 → DataTable の列名」。
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("OrderID", "PK_OrderID");

            MyDebug.OutputDebugAndConsole("[マップ有 : OrderID → PK_OrderID]");
            TestDto.OutputPocoList(DataToPoco.DataTableToList<OrderPoco>(dtPk, map));
        }

        /// <summary>PocoToPoco のテスト</summary>
        private static void TestPocoToPoco()
        {
            MyDebug.OutputDebugAndConsole("PocoToPoco");

            OrderSource src = new OrderSource();
            src.OrderID = 3;
            src.Note = "z";
            src.Memo = "移送先に同名が無い";

            // 同名のプロパティだけが移送される。
            MyDebug.OutputDebugAndConsole("[Map : 同名のみ]");
            TestDto.OutputPoco(PocoToPoco.Map<OrderSource, OrderPoco>(src));

            // マップで名前の違うプロパティを繋ぐ。
            Dictionary<string, string> map = new Dictionary<string, string>();
            map.Add("Qty", "OrderID");

            MyDebug.OutputDebugAndConsole("[Map : Qty ← OrderID]");
            TestDto.OutputPoco(PocoToPoco.Map<OrderSource, OrderPoco>(src, map));
        }

        /// <summary>DataToDictionary のテスト</summary>
        /// <remarks>
        /// 日時の書式は**必ず明示する**。既定のままだと実行環境のロケールで変わり、
        /// 期待結果と一致しなくなる。
        /// </remarks>
        private static void TestDataToDictionary()
        {
            MyDebug.OutputDebugAndConsole("DataToDictionary");

            DataTable dt = new DataTable();
            dt.Columns.Add("OrderID", typeof(int));
            dt.Columns.Add("Note", typeof(string));
            dt.Columns.Add("OrderDate", typeof(DateTime));

            DataRow dr = dt.NewRow();
            dr["OrderID"] = 4;
            dr["Note"] = "w";
            dr["OrderDate"] = new DateTime(2026, 8, 6, 12, 34, 56);
            dt.Rows.Add(dr);

            DataToDictionary d2d = new DataToDictionary(null, "yyyy/MM/dd HH:mm:ss", "");
            Dictionary<string, string> dic = d2d.DataTableToDictionary(dt);

            // キーの並びは実装依存のため、名前を指定して取り出す。
            MyDebug.OutputDebugAndConsole("OrderID   : " + TestDto.GetValue(dic, "OrderID"));
            MyDebug.OutputDebugAndConsole("Note      : " + TestDto.GetValue(dic, "Note"));
            MyDebug.OutputDebugAndConsole("OrderDate : " + TestDto.GetValue(dic, "OrderDate"));
        }

        /// <summary>DTTable のテスト</summary>
        /// <remarks>
        /// DTTable はマーシャリング可能な自前の DataTable。
        /// DataTable からの変換と、文字列との往復を確認する。
        /// </remarks>
        private static void TestDTTable()
        {
            MyDebug.OutputDebugAndConsole("DTTable");

            DataTable dt = new DataTable("TestOrders");
            dt.Columns.Add("OrderID", typeof(int));
            dt.Columns.Add("Note", typeof(string));

            DataRow dr1 = dt.NewRow();
            dr1["OrderID"] = 5;
            dr1["Note"] = "a";
            dt.Rows.Add(dr1);

            DataRow dr2 = dt.NewRow();
            dr2["OrderID"] = 6;
            dr2["Note"] = "b";
            dt.Rows.Add(dr2);

            DTTable dtt = DTTable.FromDataTable(dt);

            MyDebug.OutputDebugAndConsole("[FromDataTable]");
            TestDto.OutputDTTable(dtt);

            // 文字列との往復（マーシャリング）
            DTTables dtts = new DTTables();
            dtts.Add(dtt);

            string text = DTTables.DTTablesToString(dtts);
            DTTables restored = DTTables.StringToDTTables(text);

            MyDebug.OutputDebugAndConsole("[文字列との往復]");
            MyDebug.OutputDebugAndConsole("表の数 : " + restored.Count);
            TestDto.OutputDTTable(restored[0]);
        }

        #endregion

        #region 出力のヘルパ

        /// <summary>POCO の一覧を出力する</summary>
        /// <param name="list">POCO の一覧</param>
        private static void OutputPocoList(List<OrderPoco> list)
        {
            MyDebug.OutputDebugAndConsole("件数 : " + list.Count);

            foreach (OrderPoco poco in list)
            {
                TestDto.OutputPoco(poco);
            }
        }

        /// <summary>POCO を 1 件出力する</summary>
        /// <param name="poco">POCO</param>
        /// <remarks>null は "(null)" と出す。既定値のままかどうかを見るため。</remarks>
        private static void OutputPoco(OrderPoco poco)
        {
            if (poco == null)
            {
                MyDebug.OutputDebugAndConsole("(null)");
                return;
            }

            MyDebug.OutputDebugAndConsole(
                "OrderID=" + poco.OrderID
                + ", Note=" + (poco.Note == null ? "(null)" : poco.Note)
                + ", Qty=" + poco.Qty);
        }

        /// <summary>DTTable を出力する</summary>
        /// <param name="dtt">DTTable</param>
        private static void OutputDTTable(DTTable dtt)
        {
            MyDebug.OutputDebugAndConsole("表名 : " + dtt.TableName);

            string cols = "";
            foreach (DTColumn col in dtt.Cols)
            {
                cols += (cols == "" ? "" : ", ") + col.ColName;
            }
            MyDebug.OutputDebugAndConsole("列   : " + cols);

            foreach (DTRow row in dtt.Rows)
            {
                MyDebug.OutputDebugAndConsole(
                    "行   : " + row["OrderID"] + ", " + row["Note"]);
            }
        }

        /// <summary>Dictionary から値を取り出す</summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="key">キー</param>
        /// <returns>値（無い場合は "(なし)"）</returns>
        private static string GetValue(Dictionary<string, string> dic, string key)
        {
            if (dic != null && dic.ContainsKey(key))
            {
                return dic[key];
            }

            return "(なし)";
        }

        #endregion
    }
}

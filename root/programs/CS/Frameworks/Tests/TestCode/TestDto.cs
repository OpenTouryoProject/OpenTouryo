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
//*  2026/08/14  玄人 幸道         JSONの往復と、行ステータスの往復を追加（#544）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

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

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestDto.TestDTTablesJson();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestDto.TestRowState();
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

        /// <summary>DataSet との JSON 往復のテスト</summary>
        /// <remarks>
        /// SaveJson / LoadJson と、DataSet との相互変換を確認する（#544）。
        ///
        ///   DataSet → DTTables → JSON → DTTables → DataSet
        ///
        /// ＜値を型ごとに見る理由＞
        ///   JSON 上の値は DTColumn が文字列にしたもので、型ごとに書式が違う
        ///   （DateTime は ISO 8601、ByteArray は Base64）。
        ///   **列の型は JSON 側に持っており、読み戻しでそれを使う。**
        ///   ここが崩れると、値が入っているのに型が変わる、という壊れ方をする。
        ///
        /// ＜Double を「二進で割り切れない値」で見る理由＞
        ///   書式を指定しないと、.NET Framework 側は有効桁 15 桁に丸めてしまい、
        ///   0.1 + 0.2 が 0.3 になって**元の値に戻らない**（#544）。
        ///   ラウンドトリップ書式を使っているので、両者で同じ文字列になる。
        ///
        /// ＜改行を含む文字列と null を入れている＞
        ///   改行は JSON 側のエスケープに任せており、崩れると読み戻しで
        ///   **例外にならずに文字が欠ける**（テキスト版で実際に起きていた）。
        ///   null は JSON でも null になり、読み戻しで値を設定しない経路を通る。
        ///
        /// ＜表を 2 つにしている＞
        ///   DTTables は表のコレクションなので、複数表と表名での取得を通す。
        ///   2 つ目は、列名を JSON のキー（state / cels）とわざと重ねてある。
        /// </remarks>
        private static void TestDTTablesJson()
        {
            MyDebug.OutputDebugAndConsole("DataSet との JSON 往復");

            DataSet ds = new DataSet("TestDataSet");
            ds.Tables.Add(TestDto.MakeTypedTable());
            ds.Tables.Add(TestDto.MakeOddTable());

            MyDebug.OutputDebugAndConsole("[元の DataSet]");
            TestDto.OutputDataSet(ds);

            DTTables dtts = DTTables.FromDataSet(ds);

            // JSON へ
            string json = DTTables.DTTablesToJson(dtts);
            MyDebug.OutputDebugAndConsole("[JSON]");
            MyDebug.OutputDebugAndConsole(json);

            // JSON から
            DTTables restored = DTTables.JsonToDTTables(json);

            MyDebug.OutputDebugAndConsole("[JSONとの往復（DTTables）]");
            MyDebug.OutputDebugAndConsole("表の数 : " + restored.Count);

            // 表名でも引けること（インデクサ）
            MyDebug.OutputDebugAndConsole("表名で取得 : " + restored["TestOdd"].TableName);

            foreach (DTTable dtt in restored)
            {
                TestDto.OutputDTTableAll(dtt);
            }

            // DataSet へ戻す
            MyDebug.OutputDebugAndConsole("[DataSet へ戻す]");
            DataSet jsonBack = restored.ToDataSet();
            TestDto.OutputDataSet(jsonBack);

            // **値の表示だけに頼らない。**
            // CompareResult.ps1 は 16 文字以上の英数字の並びを <B64URL> に潰すため、
            // ラウンドトリップ書式の Double（17 桁）は期待値の上で読めなくなる。
            // 元の DataSet と突き合わせた結果を、明示的に出す。
            TestDto.OutputRoundTripResult(ds, jsonBack);

            // テキスト版でも同じものが往復すること。
            //
            // こちらは改行を「\rrnr:」「\rrnn:」に退避して行を分け、
            // 読み込み側で連結して戻す。以前は退避が効いておらず、
            // **改行以降が捨てられていた**（#544）。
            MyDebug.OutputDebugAndConsole("[テキストとの往復]");

            DataSet textBack = DTTables.StringToDTTables(DTTables.DTTablesToString(dtts)).ToDataSet();
            TestDto.OutputRoundTripResult(ds, textBack);
        }

        /// <summary>型ごとの値を持つ DataTable を作る</summary>
        /// <returns>DataTable</returns>
        private static DataTable MakeTypedTable()
        {
            DataTable dt = new DataTable("TestTypes");
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Price", typeof(decimal));
            dt.Columns.Add("Rate", typeof(double));
            dt.Columns.Add("Ordered", typeof(DateTime));
            dt.Columns.Add("Flag", typeof(bool));
            dt.Columns.Add("Blob", typeof(byte[]));

            dt.Rows.Add(1, "あいう", 1234.56m, 0.1d + 0.2d,
                new DateTime(2026, 8, 14, 12, 34, 56, 789), true, new byte[] { 0, 1, 255 });

            // 空文字と空のバイト配列（null と見分けられること）
            dt.Rows.Add(2, "", -0.01m, double.MaxValue,
                new DateTime(1977, 4, 24), false, new byte[0]);

            // 改行を含む文字列
            dt.Rows.Add(3, "あ\r\nい", 0m, 0d,
                new DateTime(2000, 1, 1), false, new byte[] { 1 });

            // すべて null（Id 以外を設定しない）
            DataRow nullRow = dt.NewRow();
            nullRow["Id"] = 4;
            dt.Rows.Add(nullRow);

            return dt;
        }

        /// <summary>列名を JSON のキーと重ねた DataTable を作る</summary>
        /// <returns>DataTable</returns>
        /// <remarks>
        /// 行ステータスをセルの外に出しているため、
        /// "state" や "cels" という名前の列があっても衝突しない。
        /// </remarks>
        private static DataTable MakeOddTable()
        {
            DataTable dt = new DataTable("TestOdd");
            dt.Columns.Add("state", typeof(string));
            dt.Columns.Add("cels", typeof(int));
            dt.Rows.Add("わな", 7);

            return dt;
        }

        /// <summary>行ステータスの往復のテスト</summary>
        /// <remarks>
        /// DataTable → DTTable → JSON → DTTable → DataTable を通して、
        /// Added / Modified / Deleted / Unchanged が保たれることを確認する（#544）。
        ///
        /// ＜ここが崩れると何が起きるか＞
        ///   受け取った側で「どの行を INSERT / UPDATE / DELETE するか」を
        ///   判別できなくなる。値は入っているので、**例外にならずに更新が漏れる**。
        ///
        /// ＜変更前の値は復元されない＞
        ///   DTRow は現在値と行ステータスだけを持つ。
        ///   Modified の DataRowVersion.Original には現在値が入る。
        /// </remarks>
        private static void TestRowState()
        {
            MyDebug.OutputDebugAndConsole("行ステータスの往復");

            DataTable dt = new DataTable("TestRowState");
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Note", typeof(string));

            dt.Rows.Add(1, "そのまま");
            dt.Rows.Add(2, "変更前");
            dt.Rows.Add(3, "削除される");
            dt.AcceptChanges();

            dt.Rows[1]["Note"] = "変更後";   // Modified
            dt.Rows[2].Delete();              // Deleted
            dt.Rows.Add(4, "追加");           // Added

            MyDebug.OutputDebugAndConsole("[元の DataTable]");
            TestDto.OutputDataTableRowState(dt);

            // DataTable → DTTable → JSON → DTTable
            DTTables dtts = new DTTables();
            dtts.Add(DTTable.FromDataTable(dt));

            DTTables restored = DTTables.JsonToDTTables(DTTables.DTTablesToJson(dtts));

            MyDebug.OutputDebugAndConsole("[JSON 往復後の DTTable]");
            foreach (DTRow row in restored[0].Rows)
            {
                MyDebug.OutputDebugAndConsole(
                    "行   : " + row["Id"] + ", " + row["Note"] + ", RowState=" + row.RowState);
            }

            // DTTable → DataTable
            MyDebug.OutputDebugAndConsole("[ToDataTable 後の DataTable]");
            TestDto.OutputDataTableRowState(restored[0].ToDataTable());
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

        /// <summary>DTTable を全列出力する</summary>
        /// <param name="dtt">DTTable</param>
        /// <remarks>
        /// **カルチャに依存しない書式で出す。**
        /// 既定の ToString は小数点や日付の区切りがカルチャで変わり、
        /// 期待値と一致しなくなる（CI は ja-JP に揃えているが、手元では違い得る）。
        ///
        /// 表ごとに列が違うため、列名は決め打ちにせず走査する。
        /// </remarks>
        private static void OutputDTTableAll(DTTable dtt)
        {
            MyDebug.OutputDebugAndConsole("表名 : " + dtt.TableName);

            string cols = "";
            foreach (DTColumn col in dtt.Cols)
            {
                cols += (cols == "" ? "" : ", ") + col.ColName + "(" + col.ColType + ")";
            }
            MyDebug.OutputDebugAndConsole("列   : " + cols);

            foreach (DTRow row in dtt.Rows)
            {
                string values = "";
                foreach (DTColumn col in dtt.Cols)
                {
                    values += (values == "" ? "" : ", ")
                        + col.ColName + "=" + TestDto.Fixed(row[col.ColName]);
                }

                MyDebug.OutputDebugAndConsole("行   : " + values + ", RowState=" + row.RowState);
            }
        }

        /// <summary>DataSet を全表・全列出力する</summary>
        /// <param name="ds">DataSet</param>
        private static void OutputDataSet(DataSet ds)
        {
            MyDebug.OutputDebugAndConsole("表の数 : " + ds.Tables.Count);

            foreach (DataTable dt in ds.Tables)
            {
                MyDebug.OutputDebugAndConsole("表名 : " + dt.TableName);

                string cols = "";
                foreach (DataColumn col in dt.Columns)
                {
                    cols += (cols == "" ? "" : ", ") + col.ColumnName + "(" + col.DataType.Name + ")";
                }
                MyDebug.OutputDebugAndConsole("列   : " + cols);

                foreach (DataRow row in dt.Rows)
                {
                    // Deleted の行は現在値を読めないため、元の値を出す
                    DataRowVersion ver = row.RowState == System.Data.DataRowState.Deleted
                        ? DataRowVersion.Original : DataRowVersion.Current;

                    string values = "";
                    foreach (DataColumn col in dt.Columns)
                    {
                        values += (values == "" ? "" : ", ")
                            + col.ColumnName + "=" + TestDto.Fixed(row[col.ColumnName, ver]);
                    }

                    MyDebug.OutputDebugAndConsole("行   : " + values + ", RowState=" + row.RowState);
                }
            }
        }

        /// <summary>往復の前後で値が一致したかを出力する</summary>
        /// <param name="src">元の DataSet</param>
        /// <param name="dst">往復後の DataSet</param>
        /// <remarks>
        /// **値そのものではなく、一致したかどうかを出す。**
        /// 期待値の比較は CompareResult.ps1 が正規化してから行うが、
        /// 16 文字以上の英数字の並びは Base64 と見なされて潰される。
        /// ラウンドトリップ書式の Double は 17 桁になるため、
        /// 値を出しただけでは**期待値の上で読めない**。
        /// </remarks>
        private static void OutputRoundTripResult(DataSet src, DataSet dst)
        {
            if (src.Tables.Count != dst.Tables.Count)
            {
                MyDebug.OutputDebugAndConsole(
                    "往復 : 表の数が違う（" + src.Tables.Count + " → " + dst.Tables.Count + "）");
                return;
            }

            for (int t = 0; t < src.Tables.Count; t++)
            {
                DataTable s = src.Tables[t];
                DataTable d = dst.Tables[t];

                if (s.TableName != d.TableName)
                {
                    MyDebug.OutputDebugAndConsole(
                        "往復 : 表名が違う（" + s.TableName + " → " + d.TableName + "）");
                    continue;
                }

                if (s.Rows.Count != d.Rows.Count)
                {
                    MyDebug.OutputDebugAndConsole(
                        "往復 : " + s.TableName + " の行数が違う（"
                        + s.Rows.Count + " → " + d.Rows.Count + "）");
                    continue;
                }

                for (int i = 0; i < s.Rows.Count; i++)
                {
                    string ng = "";

                    foreach (DataColumn col in s.Columns)
                    {
                        if (!d.Columns.Contains(col.ColumnName))
                        {
                            ng += (ng == "" ? "" : ", ") + col.ColumnName + "(列が無い)";
                            continue;
                        }

                        if (!TestDto.SameValue(s.Rows[i][col.ColumnName], d.Rows[i][col.ColumnName]))
                        {
                            ng += (ng == "" ? "" : ", ") + col.ColumnName;
                        }
                    }

                    MyDebug.OutputDebugAndConsole(
                        "往復 : " + s.TableName + " 行" + (i + 1) + " "
                        + (ng == "" ? "全列一致" : "不一致の列 = " + ng));
                }
            }
        }
        /// <summary>2 つの値が同じかを判定する</summary>
        /// <param name="a">値</param>
        /// <param name="b">値</param>
        /// <returns>同じなら true</returns>
        /// <remarks>byte[] は参照ではなく中身で比べる。</remarks>
        private static bool SameValue(object a, object b)
        {
            if (a == null || a is DBNull) { return b == null || b is DBNull; }
            if (b == null || b is DBNull) { return false; }

            if (a is byte[] && b is byte[])
            {
                byte[] x = (byte[])a;
                byte[] y = (byte[])b;

                if (x.Length != y.Length) { return false; }

                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] != y[i]) { return false; }
                }

                return true;
            }

            return a.Equals(b);
        }

        /// <summary>値をカルチャに依存しない形で文字列にする</summary>
        /// <param name="o">値</param>
        /// <returns>文字列</returns>
        private static string Fixed(object o)
        {
            if (o == null) { return "(null)"; }

            if (o is byte[])
            {
                byte[] bytes = (byte[])o;
                return "byte[" + bytes.Length + "]:" + BitConverter.ToString(bytes);
            }

            if (o is DateTime)
            {
                return ((DateTime)o).ToString("O", CultureInfo.InvariantCulture);
            }

            if (o is string)
            {
                // 空文字と null を見分けられるようにする。
                // 改行は見えるようにエスケープする（欠けても気付けるように）。
                return "\"" + ((string)o).Replace("\r", "\\r").Replace("\n", "\\n") + "\"";
            }

            // **浮動小数点はラウンドトリップ書式で出す。**
            // 既定の書式は .NET Framework が有効桁 15 桁に丸めるため、
            // net48 と net10.0 で期待値が割れる。
            if (o is double) { return ((double)o).ToString("R", CultureInfo.InvariantCulture); }
            if (o is float) { return ((float)o).ToString("R", CultureInfo.InvariantCulture); }

            return Convert.ToString(o, CultureInfo.InvariantCulture);
        }

        /// <summary>DataTable を行ステータス付きで出力する</summary>
        /// <param name="dt">DataTable</param>
        /// <remarks>Deleted の行は現在値を読めないため、元の値を出す。</remarks>
        private static void OutputDataTableRowState(DataTable dt)
        {
            MyDebug.OutputDebugAndConsole("行数 : " + dt.Rows.Count);

            foreach (DataRow row in dt.Rows)
            {
                string values;

                if (row.RowState == System.Data.DataRowState.Deleted)
                {
                    values = "Id=" + TestDto.Fixed(row["Id", DataRowVersion.Original])
                        + ", Note=" + TestDto.Fixed(row["Note", DataRowVersion.Original]);
                }
                else
                {
                    values = "Id=" + TestDto.Fixed(row["Id"])
                        + ", Note=" + TestDto.Fixed(row["Note"]);
                }

                MyDebug.OutputDebugAndConsole("行   : " + values + ", RowState=" + row.RowState);
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

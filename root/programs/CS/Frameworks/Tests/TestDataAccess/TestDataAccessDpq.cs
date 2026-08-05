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
//* クラス名        ：TestDataAccessDpq
//* クラス日本語名  ：動的パラメタライズドクエリのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/06  玄人 幸道         新規作成（#520）
//*  2026/08/06  玄人 幸道         IF/ELSE・LIST・DELCMA・作用範囲・SQL生成を追加
//**********************************************************************************

using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;

using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestDataAccess
{
    /// <summary>動的パラメタライズドクエリのテスト</summary>
    /// <remarks>
    /// 動的 SQL（.xml）は、**設定したパラメタによって SQL の形が変わる**（#520）。
    ///
    /// ＜確認する観点＞
    ///   1. &lt;WHERE&gt; / &lt;IF&gt; の組み合わせ … 条件の増減と、先頭 AND の除去
    ///   2. &lt;IF&gt; / &lt;ELSE&gt; の 3 状態  … **「未設定」と「null を設定」は別物**
    ///   3. &lt;LIST&gt;                      … IN 句への自動展開
    ///   4. パラメタの作用範囲             … テキスト内は全タグ、タグ内は最初の 1 タグ
    ///   5. &lt;DELCMA&gt; / &lt;INSCOL&gt;  … 要素が消えたときのカンマ処理
    ///   6. 組み立て結果そのもの           … ExecGenerateSQL（実行しない）
    ///
    /// ＜1〜5 を件数で見る理由＞
    ///   DBMS ごとに型や表示が変わるため、値を出すと差分になる。件数なら 4 DBMS で同じ。
    ///
    /// ＜6 を ExecGenerateSQL で見る理由＞
    ///   &lt;SELECT&gt;/&lt;CASE&gt; の分岐や比較演算子のエスケープは、
    ///   **実行しなくても組み立て結果を見れば分かる**。表の中身に依存せず、
    ///   期待値も安定する。IsDPQ（動的として扱われたか）も同時に見る。
    ///
    /// ＜XML を実行時に書き出す理由＞
    ///   パラメタの先頭記号が DBMS で異なり（Oracle は「:」、他は「@」）、
    ///   囲い文字も異なるため、共有の SQL 置き場に置くと DBMS 分の重複になる。
    ///
    /// ＜記法の参考＞
    ///   root/files/resource/Test/dpq/query/ に DPQuery_Tool 用の資産がある。
    ///   あちらの &lt;PARAM&gt; タグはツールが値を与えるためのもので、
    ///   実行時は SetParameter / SetUserParameter で与える。
    /// </remarks>
    public class TestDataAccessDpq
    {
        #region public

        /// <summary>Root</summary>
        /// <param name="daps">対象のデータ プロバイダ</param>
        public static void Root(string[] daps)
        {
            foreach (string dap in daps)
            {
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestDataAccessDpq.TestOneProvider(dap);
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
                    TestTable.Drop(dam, dap);
                    TestTable.Create(dam, dap);

                    // 4 行目は Note が NULL。<ELSE>（IS NULL 側）の確認に使う。
                    TestTable.InsertRow(dam, dap, 1, 10, 999, "x");
                    TestTable.InsertRow(dam, dap, 2, 20, 888, "y");
                    TestTable.InsertRow(dam, dap, 3, 30, 777, "z");
                    TestTable.InsertRow(dam, dap, 4, 40, 666, null);

                    TestDataAccessDpq.TestWhereIf(dam, dap);
                    TestDataAccessDpq.TestIfElse(dam, dap);
                    TestDataAccessDpq.TestList(dam, dap);
                    TestDataAccessDpq.TestParamScope(dam, dap);
                    TestDataAccessDpq.TestDelcmaInscol(dam, dap);
                    TestDataAccessDpq.TestGenerateOnly(dam, dap);
                }
                finally
                {
                    TestTable.Drop(dam, dap);
                    dam.ConnectionClose();
                }
            }
            catch (Exception ex)
            {
                MyDebug.OutputDebugAndConsole("- 例外 : " + ex.GetType().FullName);
            }
        }

        #region 1. WHERE / IF の組み合わせ

        /// <summary>&lt;WHERE&gt; / &lt;IF&gt; の組み合わせ</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <remarks>
        /// IF が 2 つなので、設定／未設定の組み合わせは 2 の 2 乗 = 4 通り。
        /// 全数を通したうえで、AND であることを判別する 1 件を足している。
        /// </remarks>
        private static void TestWhereIf(BaseDam dam, string dap)
        {
            MyDebug.OutputDebugAndConsole("- WHERE / IF の組み合わせ");

            string sign = TestDataAccessDpq.Sign(dap);
            string xml =
                "  SELECT " + TestDataAccessDpq.Col(dap, "OrderID") + " FROM " + TestTable.Quoted(dap) + "\n"
                + "  <WHERE>\n"
                + "    WHERE\n"
                + "    <IF>AND " + TestDataAccessDpq.Col(dap, "OrderID") + " = " + sign + "P1</IF>\n"
                + "    <IF>AND " + TestDataAccessDpq.Col(dap, "Qty") + " = " + sign + "P2</IF>\n"
                + "  </WHERE>\n";

            string path = TestDataAccessDpq.WriteXml(dap, "whereif", xml);

            try
            {
                // 両方。ただし同じ行を指すため、AND / OR / 片方無視 のいずれでも 1 件になる。
                TestDataAccessDpq.RunCount(dam, path, "  両方指定", "P1", 1, "P2", 999);

                // 指す行をずらす。**AND であることを判別する。**
                //   AND なら 0 件 ／ OR なら 2 件 ／ 片方を無視していれば 1 件
                TestDataAccessDpq.RunCount(dam, path, "  両方指定(不一致)", "P1", 1, "P2", 888);

                // 末尾の IF が落ちる。
                TestDataAccessDpq.RunCount(dam, path, "  OrderIDのみ", "P1", 1, "P2", null);

                // **先頭の IF が落ちる。** 残る IF は先頭に AND を持つため、
                // 除去されないと WHERE AND ... で構文エラーになる。
                TestDataAccessDpq.RunCount(dam, path, "  Qtyのみ", "P1", null, "P2", 888);

                // WHERE ごと消えて全件（4 件）になる。
                TestDataAccessDpq.RunCount(dam, path, "  指定なし", "P1", null, "P2", null);
            }
            finally
            {
                TestDataAccessDpq.DeleteXml(path);
            }
        }

        #endregion

        #region 2. IF / ELSE の 3 状態

        /// <summary>&lt;IF&gt; / &lt;ELSE&gt; の 3 状態</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <remarks>
        /// **「未設定」と「null を設定」は別物。**
        /// 条件から外したいときに null を渡すと、逆に IS NULL が残って外れない。
        /// この取り違えは実行時エラーにならず、**件数が静かに変わる**ので気付きにくい。
        ///
        /// **テキスト内パラメタとタグ内パラメタで、状態の数が違う。**
        ///
        ///   テキスト内（@P3）   値を設定 … IF ／ null … ELSE ／ 未設定 … 削除
        ///   タグ内（name="F1"） true    … IF ／ **false または null** … ELSE ／ 未設定 … 削除
        ///
        /// タグ内には false という状態が増える。両方とも通す。
        /// </remarks>
        private static void TestIfElse(BaseDam dam, string dap)
        {
            string sign = TestDataAccessDpq.Sign(dap);
            string col = TestDataAccessDpq.Col(dap, "Note");
            string head =
                "  SELECT " + TestDataAccessDpq.Col(dap, "OrderID") + " FROM " + TestTable.Quoted(dap) + "\n"
                + "  <WHERE>\n"
                + "    WHERE\n";

            // テキスト内パラメタ : 値 / null / 未設定
            string xmlText = head
                + "    <IF>AND " + col + " = " + sign + "P3<ELSE>AND " + col + " IS NULL</ELSE></IF>\n"
                + "  </WHERE>\n";

            // タグ内パラメタ : true / false / null / 未設定
            // ※ 条件式にパラメタを置かない（タグ内パラメタが有効・無効を決めるため）。
            string xmlTag = head
                + "    <IF name=\"F1\">AND " + col + " = 'x'<ELSE>AND " + col + " IS NULL</ELSE></IF>\n"
                + "  </WHERE>\n";

            string pathText = TestDataAccessDpq.WriteXml(dap, "ifelsetext", xmlText);
            string pathTag = TestDataAccessDpq.WriteXml(dap, "ifelsetag", xmlTag);

            try
            {
                MyDebug.OutputDebugAndConsole("- IF / ELSE（テキスト内パラメタ）");

                // 値を設定 → IF 側（Note = 'x'）
                TestDataAccessDpq.RunCountOne(dam, pathText, "  値を設定", "P3", "x");

                // null を設定 → **ELSE 側（Note IS NULL）**
                TestDataAccessDpq.RunCountOne(dam, pathText, "  nullを設定", "P3", null);

                // 未設定 → 条件ごと消えて全件
                TestDataAccessDpq.RunCountNone(dam, pathText, "  未設定");

                MyDebug.OutputDebugAndConsole("- IF / ELSE（タグ内パラメタ）");

                // **タグ内は false という状態が増える。** false も null も ELSE 側。
                TestDataAccessDpq.RunCountOne(dam, pathTag, "  true", "F1", true);
                TestDataAccessDpq.RunCountOne(dam, pathTag, "  false", "F1", false);
                TestDataAccessDpq.RunCountOne(dam, pathTag, "  null", "F1", null);
                TestDataAccessDpq.RunCountNone(dam, pathTag, "  未設定");
            }
            finally
            {
                TestDataAccessDpq.DeleteXml(pathText);
                TestDataAccessDpq.DeleteXml(pathTag);
            }
        }

        #endregion

        #region 3. LIST（IN 句）

        /// <summary>&lt;LIST&gt;（IN 句への展開）</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <remarks>
        /// 1 つのパラメタ名に複数値を与えると、@名_1, @名_2 … へ自動展開される。
        /// 与えた数がそのまま件数に出るため、展開されているかを件数で判別できる。
        /// </remarks>
        private static void TestList(BaseDam dam, string dap)
        {
            MyDebug.OutputDebugAndConsole("- LIST（IN 句）");

            string sign = TestDataAccessDpq.Sign(dap);
            string xml =
                "  SELECT " + TestDataAccessDpq.Col(dap, "OrderID") + " FROM " + TestTable.Quoted(dap) + "\n"
                + "  <WHERE>\n"
                + "    WHERE\n"
                + "    <LIST>AND " + TestDataAccessDpq.Col(dap, "OrderID") + " IN (" + sign + "PLIST)</LIST>\n"
                + "  </WHERE>\n";

            string path = TestDataAccessDpq.WriteXml(dap, "list", xml);

            try
            {
                ArrayList two = new ArrayList();
                two.Add(1);
                two.Add(3);
                TestDataAccessDpq.RunCountOne(dam, path, "  2 値", "PLIST", two);

                ArrayList three = new ArrayList();
                three.Add(1);
                three.Add(2);
                three.Add(4);
                TestDataAccessDpq.RunCountOne(dam, path, "  3 値", "PLIST", three);
            }
            finally
            {
                TestDataAccessDpq.DeleteXml(path);
            }
        }

        #endregion

        #region 4. パラメタの作用範囲

        /// <summary>テキスト内パラメタとタグ内パラメタの作用範囲</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <remarks>
        /// **同名を書いたとき、作用する範囲が違う。**
        ///   テキスト内（@名前）  … **同名を書いた全タグ**に作用する
        ///   タグ内（name 属性）  … **最初の 1 タグ**にだけ作用する
        ///
        /// 判別できるように、2 つの IF が両立しない条件になっている。
        ///   両方に作用すれば 0 件、最初の 1 つだけなら 1 件。
        /// </remarks>
        private static void TestParamScope(BaseDam dam, string dap)
        {
            MyDebug.OutputDebugAndConsole("- パラメタの作用範囲");

            string sign = TestDataAccessDpq.Sign(dap);
            string colOrder = TestDataAccessDpq.Col(dap, "OrderID");

            // テキスト内 : 同じ @P1 を 2 つの IF に書く。
            // 両方に作用するので、OrderID = 1 かつ OrderID = 2 となり 0 件。
            string xmlText =
                "  SELECT " + colOrder + " FROM " + TestTable.Quoted(dap) + "\n"
                + "  <WHERE>\n"
                + "    WHERE\n"
                + "    <IF>AND " + colOrder + " = " + sign + "P1</IF>\n"
                + "    <IF>AND " + colOrder + " = " + sign + "P1 + 1</IF>\n"
                + "  </WHERE>\n";

            // タグ内 : 同じ name="F1" を 2 つの IF に書く。
            // 最初の 1 つにしか作用しないので、OrderID = 1 だけが残り 1 件。
            string xmlTag =
                "  SELECT " + colOrder + " FROM " + TestTable.Quoted(dap) + "\n"
                + "  <WHERE>\n"
                + "    WHERE\n"
                + "    <IF name=\"F1\">AND " + colOrder + " = 1</IF>\n"
                + "    <IF name=\"F1\">AND " + colOrder + " = 2</IF>\n"
                + "  </WHERE>\n";

            string pathText = TestDataAccessDpq.WriteXml(dap, "scopetext", xmlText);
            string pathTag = TestDataAccessDpq.WriteXml(dap, "scopetag", xmlTag);

            try
            {
                TestDataAccessDpq.RunCountOne(dam, pathText, "  テキスト内(全タグ)", "P1", 1);
                TestDataAccessDpq.RunCountOne(dam, pathTag, "  タグ内(最初の1つ)", "F1", true);
            }
            finally
            {
                TestDataAccessDpq.DeleteXml(pathText);
                TestDataAccessDpq.DeleteXml(pathTag);
            }
        }

        #endregion

        #region 5. DELCMA / INSCOL

        /// <summary>&lt;DELCMA&gt; / &lt;INSCOL&gt;（カンマの処理）</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <remarks>
        /// 列とその値を対で増減させる。**各要素の末尾にカンマを付けて書き**、
        /// &lt;DELCMA&gt; が前後の余分なカンマを削る。
        /// どの要素が残っても構文が壊れないことを、実際に INSERT して確かめる。
        /// </remarks>
        private static void TestDelcmaInscol(BaseDam dam, string dap)
        {
            MyDebug.OutputDebugAndConsole("- DELCMA / INSCOL");

            string sign = TestDataAccessDpq.Sign(dap);
            string xml =
                "  INSERT INTO " + TestTable.Quoted(dap) + "\n"
                + "  (\n"
                + "    " + TestDataAccessDpq.Col(dap, "OrderID") + ",\n"
                + "    " + TestDataAccessDpq.Col(dap, "ProductID") + ",\n"
                + "    <DELCMA>\n"
                + "      <INSCOL name=\"Qty\">" + TestDataAccessDpq.Col(dap, "Qty") + ",</INSCOL>\n"
                + "      <INSCOL name=\"Note\">" + TestDataAccessDpq.Col(dap, "Note") + ",</INSCOL>\n"
                + "    </DELCMA>\n"
                + "  )\n"
                + "  VALUES\n"
                + "  (\n"
                + "    " + sign + "OrderID,\n"
                + "    " + sign + "ProductID,\n"
                + "    <DELCMA>\n"
                + "      <IF>" + sign + "Qty,</IF>\n"
                + "      <IF>" + sign + "Note,</IF>\n"
                + "    </DELCMA>\n"
                + "  )\n";

            string path = TestDataAccessDpq.WriteXml(dap, "delcma", xml);

            try
            {
                // 両方の列を与える。
                TestDataAccessDpq.RunInsert(dam, path, "  両方の列", 5, 50, 555, "v");

                // 末尾の列を落とす → 列リストと値の両方からカンマごと消える。
                TestDataAccessDpq.RunInsert(dam, path, "  Qtyのみ", 6, 60, 666, null);

                // 先頭の列を落とす → 残った要素の先頭カンマが消える。
                TestDataAccessDpq.RunInsert(dam, path, "  Noteのみ", 7, 70, -1, "w");

                MyDebug.OutputDebugAndConsole("  投入後の件数 : " + TestTable.Count(dam, dap));
            }
            finally
            {
                TestDataAccessDpq.DeleteXml(path);
            }
        }

        #endregion

        #region 6. 組み立て結果だけを見る（ExecGenerateSQL）

        /// <summary>実行せずに組み立て結果だけを見る</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <remarks>
        /// ExecGenerateSQL は SQL を組み立てて返すだけで、実行しない。
        /// 分岐の結果やエスケープの扱いは、**表の中身に依存せず組み立て結果で分かる**。
        /// あわせて IsDPQ（動的として扱われたか）を見る。
        /// </remarks>
        private static void TestGenerateOnly(BaseDam dam, string dap)
        {
            MyDebug.OutputDebugAndConsole("- 組み立てのみ（ExecGenerateSQL）");

            SQLUtility util = new SQLUtility(DataProvider.GetDbmsType(dap));
            string sign = TestDataAccessDpq.Sign(dap);
            string colQty = TestDataAccessDpq.Col(dap, "Qty");

            // ---- SELECT / CASE / DEFAULT : 値による分岐 ----
            string xmlSelect =
                "  <SELECT name=\"SEL\">\n"
                + "    <CASE value=\"a1\">SELECT 1 AS X</CASE>\n"
                + "    <CASE value=\"b2\">SELECT 2 AS X</CASE>\n"
                + "    <DEFAULT>SELECT 9 AS X</DEFAULT>\n"
                + "  </SELECT>\n";

            string pathSelect = TestDataAccessDpq.WriteXml(dap, "select", xmlSelect);

            // ---- 比較演算子 : < は &lt; と CDATA の 2 通りで書ける ----
            // ※ どちらの IF にも**テキスト内パラメタを置くこと。**
            // 　 パラメタも name 属性も無い IF は ArgumentException になる
            // 　 （BaseDam が「どちらで有効・無効を決めるか」を判断できないため）。
            // 　 CDATA の中のパラメタも認識される。
            string xmlLt =
                "  SELECT " + TestDataAccessDpq.Col(dap, "OrderID") + " FROM " + TestTable.Quoted(dap) + "\n"
                + "  <WHERE>\n"
                + "    WHERE\n"
                + "    <IF>AND " + colQty + " &lt; " + sign + "P1</IF>\n"
                + "    <IF><![CDATA[AND " + colQty + " > " + sign + "P2]]></IF>\n"
                + "  </WHERE>\n";

            string pathLt = TestDataAccessDpq.WriteXml(dap, "lt", xmlLt);

            // ---- JOIN / SUB : 結合・副問い合わせを丸ごと出し入れする ----
            // **実行しないので、存在しない表を書いてよい。**
            // 表を 1 つしか用意していなくても、組み立て結果は確認できる。
            //
            // ※ 無効にしたいときは**設定しない**（未設定＝ブロック削除）。
            // 　 false や null は <ELSE> が無いとエラーになる。
            string other = DataProvider.Quote(dap, "OtherTable");
            string xmlJoin =
                "  SELECT o." + TestDataAccessDpq.Col(dap, "OrderID")
                + " FROM " + TestTable.Quoted(dap) + " o\n"
                + "  <JOIN name=\"J1\">\n"
                + "    INNER JOIN " + other + " t ON o." + TestDataAccessDpq.Col(dap, "OrderID")
                + " = t." + TestDataAccessDpq.Col(dap, "OrderID") + "\n"
                + "  </JOIN>\n"
                + "  <WHERE>\n"
                + "    WHERE\n"
                + "    <IF>AND o." + colQty + " = " + sign + "P1</IF>\n"
                + "    <SUB name=\"S1\">AND o." + TestDataAccessDpq.Col(dap, "OrderID")
                + " IN (SELECT " + TestDataAccessDpq.Col(dap, "OrderID") + " FROM " + other + ")</SUB>\n"
                + "  </WHERE>\n";

            string pathJoin = TestDataAccessDpq.WriteXml(dap, "join", xmlJoin);

            try
            {
                TestDataAccessDpq.GenerateOne(dam, util, pathSelect, "  CASE(a1)", "SEL", "a1");
                TestDataAccessDpq.GenerateOne(dam, util, pathSelect, "  CASE(b2)", "SEL", "b2");
                TestDataAccessDpq.GenerateOne(dam, util, pathSelect, "  DEFAULT", "SEL", "zz");

                // &lt; の側と CDATA の側を、両方とも有効にして組み立てる。
                dam.SetSqlByFile(pathLt);
                dam.SetParameter("P1", 900);
                dam.SetParameter("P2", 100);
                TestDataAccessDpq.OutputSql(dam, util, "  比較演算子");

                // JOIN と SUB を両方入れる。
                dam.SetSqlByFile(pathJoin);
                dam.SetParameter("J1", true);
                dam.SetParameter("S1", true);
                dam.SetParameter("P1", 999);
                TestDataAccessDpq.OutputSql(dam, util, "  JOIN+SUB あり");

                // JOIN だけ入れる（SUB は設定しない＝ブロックごと削除）。
                dam.SetSqlByFile(pathJoin);
                dam.SetParameter("J1", true);
                dam.SetParameter("P1", 999);
                TestDataAccessDpq.OutputSql(dam, util, "  JOIN のみ");

                // どちらも設定しない → 両方消える。IF も落ちて WHERE ごと消える。
                dam.SetSqlByFile(pathJoin);
                TestDataAccessDpq.OutputSql(dam, util, "  JOIN+SUB なし");
            }
            catch (NotImplementedException)
            {
                // **ExecGenerateSQL を実装しているのは DamSqlSvr だけ。**
                // 他のデータ プロバイダは NotImplementedException を投げる。
                // 組み立ての結果は DBMS によらないため、SQL Server で見れば足りる。
                MyDebug.OutputDebugAndConsole("  ExecGenerateSQL : このデータ プロバイダでは未実装");
            }
            finally
            {
                TestDataAccessDpq.DeleteXml(pathSelect);
                TestDataAccessDpq.DeleteXml(pathJoin);
            }

            // ---- IsDPQ : 動的として扱われたか ----
            // 拡張子で切り替わる（.xml は動的、.sql は静的）。
            // ※ **整形式でない XML はフォールバックせず例外**になる。
            // 　 「書式が不正なら静的にフォールバック」はタグの綴り等の話で、
            // 　 XML として壊れている場合は対象外。ここでは .sql と対比する。
            string pathNotDpq = TestDataAccessDpq.WriteFile(
                Path.Combine(Path.GetTempPath(), "TestDataAccess_notdpq_" + dap + ".sql"),
                "SELECT 1 AS X\n");

            try
            {
                dam.SetSqlByFile(pathLt);
                MyDebug.OutputDebugAndConsole("  IsDPQ（タグあり）: " + dam.IsDPQ);

                dam.SetSqlByFile(pathNotDpq);
                MyDebug.OutputDebugAndConsole("  IsDPQ（タグなし）: " + dam.IsDPQ);
            }
            finally
            {
                // pathLt はここまで使うため、上の finally では消していない。
                TestDataAccessDpq.DeleteXml(pathLt);
                TestDataAccessDpq.DeleteXml(pathNotDpq);
            }
        }

        /// <summary>組み立てた SQL を 1 行にして出力する</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="util">SQLUtility</param>
        /// <param name="path">動的 SQL のパス</param>
        /// <param name="caseName">ケース名</param>
        /// <param name="paramName">パラメタ名</param>
        /// <param name="paramValue">パラメタの値</param>
        private static void GenerateOne(
            BaseDam dam, SQLUtility util, string path, string caseName,
            string paramName, object paramValue)
        {
            dam.SetSqlByFile(path);
            dam.SetParameter(paramName, paramValue);

            TestDataAccessDpq.OutputSql(dam, util, caseName);
        }

        /// <summary>組み立てた SQL を 1 行にして出力する</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="util">SQLUtility</param>
        /// <param name="caseName">ケース名</param>
        private static void OutputSql(BaseDam dam, SQLUtility util, string caseName)
        {
            // 改行・連続空白を畳んで、環境によらず同じ 1 行にする。
            string sql = dam.ExecGenerateSQL(util);
            sql = System.Text.RegularExpressions.Regex.Replace(sql, @"\s+", " ").Trim();

            MyDebug.OutputDebugAndConsole(caseName + " : " + sql);
        }

        #endregion

        #region 実行のヘルパ

        /// <summary>2 パラメタを与えて件数を出す</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="path">動的 SQL のパス</param>
        /// <param name="caseName">ケース名</param>
        /// <param name="name1">パラメタ名 1</param>
        /// <param name="value1">値 1（null なら設定しない）</param>
        /// <param name="name2">パラメタ名 2</param>
        /// <param name="value2">値 2（null なら設定しない）</param>
        private static void RunCount(
            BaseDam dam, string path, string caseName,
            string name1, object value1, string name2, object value2)
        {
            dam.SetSqlByFile(path);

            if (value1 != null) { dam.SetParameter(name1, value1); }
            if (value2 != null) { dam.SetParameter(name2, value2); }

            TestDataAccessDpq.Output(dam, caseName);
        }

        /// <summary>1 パラメタを与えて件数を出す</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="path">動的 SQL のパス</param>
        /// <param name="caseName">ケース名</param>
        /// <param name="name">パラメタ名</param>
        /// <param name="value">値（null も「null を設定」として渡す）</param>
        /// <remarks>
        /// **null は「設定しない」ではなく「null を設定」。**
        /// 設定しないケースは RunCountNone を使う。
        /// </remarks>
        private static void RunCountOne(
            BaseDam dam, string path, string caseName, string name, object value)
        {
            dam.SetSqlByFile(path);
            dam.SetParameter(name, value);

            TestDataAccessDpq.Output(dam, caseName);
        }

        /// <summary>パラメタを与えずに件数を出す</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="path">動的 SQL のパス</param>
        /// <param name="caseName">ケース名</param>
        private static void RunCountNone(BaseDam dam, string path, string caseName)
        {
            dam.SetSqlByFile(path);

            TestDataAccessDpq.Output(dam, caseName);
        }

        /// <summary>件数を出力する</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="caseName">ケース名</param>
        private static void Output(BaseDam dam, string caseName)
        {
            DataTable dt = new DataTable();
            dam.ExecSelectFill_DT(dt);

            MyDebug.OutputDebugAndConsole(caseName + " : " + dt.Rows.Count + " 件");
        }

        /// <summary>INSERT を実行する</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="path">動的 SQL のパス</param>
        /// <param name="caseName">ケース名</param>
        /// <param name="orderId">OrderID</param>
        /// <param name="productId">ProductID</param>
        /// <param name="qty">Qty（-1 なら設定しない）</param>
        /// <param name="note">Note（null なら設定しない）</param>
        private static void RunInsert(
            BaseDam dam, string path, string caseName,
            int orderId, int productId, int qty, string note)
        {
            dam.SetSqlByFile(path);

            dam.SetParameter("OrderID", orderId);
            dam.SetParameter("ProductID", productId);

            if (qty != -1) { dam.SetParameter("Qty", qty); }
            if (note != null) { dam.SetParameter("Note", note); }

            MyDebug.OutputDebugAndConsole(
                caseName + " : " + dam.ExecInsUpDel_NonQuery() + " 件");
        }

        #endregion

        #region XML のヘルパ

        /// <summary>パラメタの先頭記号</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <returns>先頭記号</returns>
        /// <remarks>Oracle だけ「:」で、他は「@」。</remarks>
        private static string Sign(string dap)
        {
            return (dap == "ODP") ? ":" : "@";
        }

        /// <summary>囲った列名</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <param name="name">列名</param>
        /// <returns>囲った列名</returns>
        private static string Col(string dap, string name)
        {
            return DataProvider.Quote(dap, name);
        }

        /// <summary>動的 SQL（.xml）を書き出す</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <param name="name">ファイル名の一部</param>
        /// <param name="inner">ROOT の中身</param>
        /// <returns>書き出したパス</returns>
        private static string WriteXml(string dap, string name, string inner)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\n");
            sb.Append("<ROOT>\n");
            sb.Append(inner);
            sb.Append("</ROOT>\n");

            return TestDataAccessDpq.WriteRaw(dap, name, sb.ToString());
        }

        /// <summary>ファイルをそのまま書き出す</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <param name="name">ファイル名の一部</param>
        /// <param name="text">中身</param>
        /// <returns>書き出したパス</returns>
        private static string WriteRaw(string dap, string name, string text)
        {
            return TestDataAccessDpq.WriteFile(
                Path.Combine(Path.GetTempPath(), "TestDataAccess_" + name + "_" + dap + ".xml"),
                text);
        }

        /// <summary>指定したパスへ書き出す</summary>
        /// <param name="path">パス</param>
        /// <param name="text">中身</param>
        /// <returns>書き出したパス</returns>
        private static string WriteFile(string path, string text)
        {
            File.WriteAllText(path, text, new UTF8Encoding(false));

            return path;
        }

        /// <summary>書き出した XML を消す</summary>
        /// <param name="path">パス</param>
        private static void DeleteXml(string path)
        {
            if (path != "" && File.Exists(path))
            {
                File.Delete(path);
            }
        }

        #endregion

        #endregion
    }
}

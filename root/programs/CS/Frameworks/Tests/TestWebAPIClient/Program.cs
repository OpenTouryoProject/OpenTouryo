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
//* クラス名        ：Program
//* クラス日本語名  ：DTO を使用したバッチ更新（WebAPI Client）の確認
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/20  玄人 幸道         新規作成（#570）
//**********************************************************************************

using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using Touryo.Infrastructure.Public.Dto;

namespace TestWebAPIClient
{
    /// <summary>応答</summary>
    /// <remarks>
    /// **状態コードを持たせる。**
    /// 本文の部分一致だけで判定すると、エラー ページに引っかかる。
    /// 実際、IIS Express の 500.19（構成エラー）が返す HTML に "test" が含まれており、
    /// **疎通が OK と表示された。**「通ったこと」は正しさの証拠にならない。
    /// </remarks>
    class Res
    {
        /// <summary>状態コード（取れないときは -1）</summary>
        public int Status;

        /// <summary>本文</summary>
        public string Body;
    }

    /// <summary>DTO を使用したバッチ更新（WebAPI Client）の確認</summary>
    /// <remarks>
    /// **何を確かめるか。**
    ///
    ///   DataTable を DTTables 経由で JSON にして往復させたとき、
    ///   RowState と Original が保たれ、**バッチ更新に使える**こと（#567 / #570）。
    ///
    /// **なぜクライアントを別に建てるか。**
    ///
    ///   サーバ側だけでは「同一プロセス内の DataTable」を触ってしまい、
    ///   **JSON をまたいだことにならない。**
    ///   HTTP 越しに送って戻すところまでやらないと、往復の検証にならない。
    ///
    /// **判定。** 項目ごとに OK / NG を出し、末尾に件数を出す（TestTransmission と同じ）。
    /// </remarks>
    class Program
    {
        /// <summary>NG の件数</summary>
        private static int NG = 0;

        /// <summary>接続先</summary>
        private static string BaseUrl = "http://localhost:51087/api/batchupdate";

        #region エントリ ポイント

        /// <summary>エントリ ポイント</summary>
        /// <param name="args">引数（第 1 引数で接続先を上書きできる）</param>
        static void Main(string[] args)
        {
            if (args != null && args.Length >= 1 && !string.IsNullOrEmpty(args[0]))
            {
                Program.BaseUrl = args[0].TrimEnd('/');
            }

            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("接続先 : {0}", Program.BaseUrl);
            Console.WriteLine();

            try
            {
                Program.CaseConnect();
                Program.CaseRoundTrip();
                Program.CaseOptimisticLock();
            }
            catch (Exception e)
            {
                // **ここで握って NG に数える。** 落として終わると件数が出ない。
                Program.NG++;
                Console.WriteLine("  [NG] 想定外の例外 : {0} : {1}", e.GetType().Name, e.Message);
            }

            Console.WriteLine();
            Console.WriteLine("NG : {0} 件", Program.NG);
        }

        #endregion

        #region 疎通

        /// <summary>疎通（DB を使わない）</summary>
        /// <remarks>
        /// **状態コードと本文の形の両方を見る。**
        /// 200 でないものは、内容を見るまでもなく NG。
        /// </remarks>
        private static void CaseConnect()
        {
            Console.WriteLine("=== 疎通 ===");

            // net48 は JSON なので "test"、net10.0 は素の text なので test で返る。
            Res res = Program.Get("/test");
            Program.Check("GET /test",
                res.Status == 200 && Program.Unquote(res.Body) == "test", res);

            res = Program.Post("/SelectCount", "");
            Program.Check("POST /SelectCount",
                res.Status == 200 && Program.Match(res.Body, "\"count\"\\s*:\\s*\\d+"), res);

            Console.WriteLine();
        }

        #endregion

        #region 往復（RowState と Original）

        /// <summary>DataTable → JSON → DataTable の往復で、CUD を振り分けられること</summary>
        private static void CaseRoundTrip()
        {
            Console.WriteLine("=== 往復（RowState と Original）===");

            // ---- 一覧を取る ----
            DataTable before = Program.SelectAll();
            if (before == null || before.Rows.Count == 0)
            {
                Program.Check("一覧の取得", false, "(取れない)");
                return;
            }
            Program.Check("一覧の取得", true, before.Rows.Count + " 件");

            Program.Check("全列が揃っている", before.Columns.Count == 12,
                before.Columns.Count + " 列");

            // ---- 編集する（追加と更新を 1 件ずつ）----
            string tag = "smoke-" + DateTime.Now.ToString("HHmmss");

            DataRow added = before.NewRow();
            added["CompanyName"] = tag;
            added["ContactName"] = "tester";
            added["Country"] = "Japan";
            before.Rows.Add(added);

            DataRow modified = before.Rows[0];
            string originalName = Program.Str(modified["CompanyName"]);
            modified["ContactName"] = tag;

            Program.Check("編集後の RowState",
                added.RowState == DataRowState.Added && modified.RowState == DataRowState.Modified,
                "Added=" + added.RowState + " / Modified=" + modified.RowState);

            // ---- JSON にして戻す（ここが往復）----
            DTTables dtts = new DTTables();
            dtts.Add(DTTable.FromDataTable(before, true));
            string json = DTTables.DTTablesToJson(dtts);

            DataTable after = Program.FirstTable(DTTables.JsonToDTTables(json));

            Program.Check("往復後も RowState が残る",
                Program.CountByState(after, DataRowState.Added) == 1
                && Program.CountByState(after, DataRowState.Modified) == 1,
                "Added=" + Program.CountByState(after, DataRowState.Added)
                + " / Modified=" + Program.CountByState(after, DataRowState.Modified));

            // **Original が残っているか。** これが無いと楽観排他が組めない。
            //   Current が編集後になっていることも同時に見る（両方揃って初めて使える）。
            DataRow afterModified = Program.FirstByState(after, DataRowState.Modified);
            Program.Check("往復後も Original が残る",
                afterModified != null
                && Program.Str(afterModified["CompanyName", DataRowVersion.Original]) == originalName
                && Program.Str(afterModified["ContactName"]) == tag,
                afterModified == null ? "(無し)"
                    : "Original=" + Program.Str(afterModified["CompanyName", DataRowVersion.Original])
                      + " / Current=" + Program.Str(afterModified["ContactName"]));

            Console.WriteLine();
        }

        #endregion

        #region 楽観排他

        /// <summary>Original を WHERE に入れた楽観排他が効くこと</summary>
        /// <remarks>
        /// **「他者が先に更新した」状況を作る。**
        ///   ① 一覧を取る（これが古い版になる）
        ///   ② 別の経路で 1 件更新する（他者の更新）
        ///   ③ ①を編集して送る → **更新件数 0 で業務例外**になるはず
        /// </remarks>
        private static void CaseOptimisticLock()
        {
            Console.WriteLine("=== 楽観排他（Original を WHERE に入れる）===");

            // ① 古い版
            DataTable stale = Program.SelectAll();
            if (stale == null || stale.Rows.Count == 0)
            {
                Program.Check("一覧の取得", false, "(取れない)");
                return;
            }

            int targetId = Convert.ToInt32(stale.Rows[0]["SupplierID"]);
            string tag = "lock-" + DateTime.Now.ToString("HHmmss");

            // ② 他者の更新（同じ行の ContactTitle を変える）
            DataTable other = Program.SelectAll();
            DataRow otherRow = Program.FindById(other, targetId);
            string keep = Program.Str(otherRow["ContactTitle"]);
            otherRow["ContactTitle"] = tag;

            Res res = Program.BatchUpdate(other);
            Program.Check("他者の更新が通る",
                res.Status == 200 && Program.Match(res.Body, "\"updateCount\"\\s*:\\s*1"), res);

            // ③ 古い版を編集して送る
            DataRow staleRow = Program.FindById(stale, targetId);
            staleRow["ContactName"] = tag;

            res = Program.BatchUpdate(stale);
            Program.Check("**古い版の更新が弾かれる**",
                res.Status == 200 && Program.Match(res.Body, "\"errorMessageID\"\\s*:\\s*\"W0002\""), res);

            // ---- 後片付け（他者の更新を戻す）----
            DataTable restore = Program.SelectAll();
            DataRow restoreRow = Program.FindById(restore, targetId);
            if (restoreRow != null)
            {
                restoreRow["ContactTitle"] = keep;
                res = Program.BatchUpdate(restore);
                Program.Check("後片付け",
                    res.Status == 200 && Program.Match(res.Body, "\"updateCount\"\\s*:\\s*1"), res);
            }

            Console.WriteLine();
        }

        #endregion

        #region WebAPI の呼び出し

        /// <summary>一覧を取る</summary>
        /// <returns>DataTable（取れなければ null）</returns>
        private static DataTable SelectAll()
        {
            Res res = Program.Post("/SelectAll", "");
            if (res.Status != 200) { return null; }

            string json = Program.ExtractJsonString(res.Body, "Suppliers");
            if (json == null) { return null; }

            return Program.FirstTable(DTTables.JsonToDTTables(json));
        }

        /// <summary>バッチ更新する</summary>
        /// <param name="dt">対象</param>
        /// <returns>応答</returns>
        private static Res BatchUpdate(DataTable dt)
        {
            DTTables dtts = new DTTables();
            dtts.Add(DTTable.FromDataTable(dt, true));

            string json = DTTables.DTTablesToJson(dtts);

            // JSON の中に JSON を入れるので、文字列としてエスケープする。
            return Program.Post("/BatchUpdate", "{\"Suppliers\":" + Program.Quote(json) + "}");
        }

        #endregion

        #region HTTP

        /// <summary>GET</summary>
        /// <param name="path">パス</param>
        /// <returns>応答</returns>
        private static Res Get(string path)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Program.BaseUrl + path);
            req.Method = "GET";
            return Program.ReadResponse(req);
        }

        /// <summary>POST（application/json）</summary>
        /// <param name="path">パス</param>
        /// <param name="body">本文</param>
        /// <returns>応答</returns>
        private static Res Post(string path, string body)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Program.BaseUrl + path);
            req.Method = "POST";
            req.ContentType = "application/json";

            byte[] bytes = Encoding.UTF8.GetBytes(body ?? "");
            req.ContentLength = bytes.Length;
            using (Stream s = req.GetRequestStream())
            {
                s.Write(bytes, 0, bytes.Length);
            }

            return Program.ReadResponse(req);
        }

        /// <summary>応答を読む</summary>
        /// <param name="req">要求</param>
        /// <returns>応答</returns>
        /// <remarks>**4xx / 5xx でも本文を読む。** 例外にすると内容が分からない。</remarks>
        private static Res ReadResponse(HttpWebRequest req)
        {
            try
            {
                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                {
                    return new Res { Status = (int)res.StatusCode, Body = sr.ReadToEnd() };
                }
            }
            catch (WebException we)
            {
                HttpWebResponse res = we.Response as HttpWebResponse;
                if (res == null)
                {
                    return new Res { Status = -1, Body = "(応答なし) " + we.Message };
                }

                using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                {
                    return new Res { Status = (int)res.StatusCode, Body = sr.ReadToEnd() };
                }
            }
        }

        #endregion

        #region ユーティリティ

        /// <summary>判定を出す（応答つき）</summary>
        /// <param name="title">項目</param>
        /// <param name="ok">合否</param>
        /// <param name="res">応答</param>
        private static void Check(string title, bool ok, Res res)
        {
            string detail = (res == null) ? "(応答なし)"
                : "HTTP " + res.Status + " : " + res.Body;

            Program.Check(title, ok, detail);
        }

        /// <summary>判定を出す</summary>
        /// <param name="title">項目</param>
        /// <param name="ok">合否</param>
        /// <param name="detail">内容</param>
        private static void Check(string title, bool ok, string detail)
        {
            if (!ok) { Program.NG++; }

            Console.WriteLine("  [{0}] {1,-28} : {2}",
                ok ? "OK" : "NG", title, Program.Trim(detail));
        }

        /// <summary>正規表現に一致するか</summary>
        /// <param name="s">文字列</param>
        /// <param name="pattern">パターン</param>
        /// <returns>一致するか</returns>
        /// <remarks>
        /// **部分一致ではなく形で見る。**
        /// 「その語が含まれるか」だと、エラー ページの HTML にも一致してしまう。
        /// </remarks>
        private static bool Match(string s, string pattern)
        {
            if (s == null) { return false; }
            return Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>前後の二重引用符を外す</summary>
        /// <param name="s">文字列</param>
        /// <returns>文字列</returns>
        private static string Unquote(string s)
        {
            if (s == null) { return ""; }

            s = s.Trim();
            if (s.Length >= 2 && s[0] == '"' && s[s.Length - 1] == '"')
            {
                return s.Substring(1, s.Length - 2);
            }
            return s;
        }

        /// <summary>長い応答を切り詰める</summary>
        /// <param name="s">文字列</param>
        /// <returns>文字列</returns>
        private static string Trim(string s)
        {
            if (s == null) { return "(null)"; }

            s = s.Replace("\r", "").Replace("\n", " ");
            return s.Length <= 110 ? s : s.Substring(0, 110) + " …";
        }

        /// <summary>DTTables の先頭テーブルを DataTable にする</summary>
        /// <param name="dtts">DTTables</param>
        /// <returns>DataTable</returns>
        private static DataTable FirstTable(DTTables dtts)
        {
            foreach (DTTable dtt in dtts)
            {
                return dtt.ToDataTable();
            }
            return null;
        }

        /// <summary>RowState ごとの件数</summary>
        /// <param name="dt">対象</param>
        /// <param name="state">RowState</param>
        /// <returns>件数</returns>
        private static int CountByState(DataTable dt, DataRowState state)
        {
            if (dt == null) { return -1; }

            int n = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr.RowState == state) { n++; }
            }
            return n;
        }

        /// <summary>RowState が一致する最初の行</summary>
        /// <param name="dt">対象</param>
        /// <param name="state">RowState</param>
        /// <returns>DataRow</returns>
        private static DataRow FirstByState(DataTable dt, DataRowState state)
        {
            if (dt == null) { return null; }

            foreach (DataRow dr in dt.Rows)
            {
                if (dr.RowState == state) { return dr; }
            }
            return null;
        }

        /// <summary>主キーで行を探す</summary>
        /// <param name="dt">対象</param>
        /// <param name="id">SupplierID</param>
        /// <returns>DataRow</returns>
        private static DataRow FindById(DataTable dt, int id)
        {
            if (dt == null) { return null; }

            foreach (DataRow dr in dt.Rows)
            {
                if (dr.RowState == DataRowState.Deleted) { continue; }
                if (Convert.ToInt32(dr["SupplierID"]) == id) { return dr; }
            }
            return null;
        }

        /// <summary>JSON から文字列の値を取り出す</summary>
        /// <param name="json">JSON</param>
        /// <param name="name">名前</param>
        /// <returns>値（見つからなければ null）</returns>
        /// <remarks>
        /// **素朴に取り出す。** ここで JSON ライブラリに依存させたくない
        /// （このクライアントは DTO の往復を見るのが目的で、JSON 処理は手段でしかない）。
        /// </remarks>
        private static string ExtractJsonString(string json, string name)
        {
            if (json == null) { return null; }

            string key = "\"" + name + "\":\"";
            int i = json.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (i < 0)
            {
                key = "\"" + Program.LowerFirst(name) + "\":\"";
                i = json.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                if (i < 0) { return null; }
            }

            i += key.Length;

            StringBuilder sb = new StringBuilder();
            while (i < json.Length)
            {
                char c = json[i];
                if (c == '\\' && i + 1 < json.Length)
                {
                    char n = json[i + 1];
                    switch (n)
                    {
                        case '"':  sb.Append('"');  break;
                        case '\\': sb.Append('\\'); break;
                        case '/':  sb.Append('/');  break;
                        case 'b':  sb.Append('\b'); break;
                        case 'f':  sb.Append('\f'); break;
                        case 'n':  sb.Append('\n'); break;
                        case 'r':  sb.Append('\r'); break;
                        case 't':  sb.Append('\t'); break;
                        case 'u':
                            sb.Append((char)Convert.ToInt32(json.Substring(i + 2, 4), 16));
                            i += 4;
                            break;
                        default:   sb.Append(n);    break;
                    }
                    i += 2;
                    continue;
                }
                if (c == '"') { break; }

                sb.Append(c);
                i++;
            }

            return sb.ToString();
        }

        /// <summary>JSON の文字列リテラルにする</summary>
        /// <param name="s">文字列</param>
        /// <returns>文字列リテラル</returns>
        private static string Quote(string s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('"');

            foreach (char c in s)
            {
                switch (c)
                {
                    case '"':  sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b");  break;
                    case '\f': sb.Append("\\f");  break;
                    case '\n': sb.Append("\\n");  break;
                    case '\r': sb.Append("\\r");  break;
                    case '\t': sb.Append("\\t");  break;
                    default:
                        if (c < ' ') { sb.Append("\\u").Append(((int)c).ToString("x4")); }
                        else { sb.Append(c); }
                        break;
                }
            }

            sb.Append('"');
            return sb.ToString();
        }

        /// <summary>先頭を小文字にする</summary>
        /// <param name="s">文字列</param>
        /// <returns>文字列</returns>
        private static string LowerFirst(string s)
        {
            if (string.IsNullOrEmpty(s)) { return s; }
            return char.ToLowerInvariant(s[0]) + s.Substring(1);
        }

        /// <summary>null 安全に文字列化する</summary>
        /// <param name="o">値</param>
        /// <returns>文字列</returns>
        private static string Str(object o)
        {
            if (o == null || o == DBNull.Value) { return ""; }
            return o.ToString();
        }

        #endregion
    }
}

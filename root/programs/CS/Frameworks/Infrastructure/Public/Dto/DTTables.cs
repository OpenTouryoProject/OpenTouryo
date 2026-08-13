//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

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
//* クラス名        ：DTTables
//* クラス日本語名  ：マーシャリング機能付き汎用DTO（表コレクション）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/03/xx  西野  大介        新規作成
//*  2010/11/11  前川  祐介        一覧更新処理対応（行ステータス）
//*  2010/11/11  前川  祐介        Silverlight対応（ジェネリック）
//*  2011/10/09  西野  大介        国際化対応
//*  2011/11/21  西野  大介        マーシャリングのサポート メソッドを追加
//*  2026/08/14  玄人 幸道         SaveJson / LoadJson を追加（#544）。
//*  2026/08/14  玄人 幸道         値と文字列の相互変換の呼び先をDTColumnに変更（#544）。
//*  2026/08/14  玄人 幸道         DataSetとの相互変換を追加（#544）。
//**********************************************************************************

using System;
using System.Data;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Touryo.Infrastructure.Public.Dto
{
    /// <summary>表コレクション</summary>
    public class DTTables : IEnumerable
    {
        #region インスタンス変数

        /// <summary>表を保持するList</summary>
        private List<DTTable> _tbls = new List<DTTable>();

        /// <summary>表名 ⇒ 表インデックスのマップを保持するDictionary</summary>
        private Dictionary<string, int> _tblsNameIndexMap = new Dictionary<string, int>();

        #endregion

        #region 表

        #region 設定

        /// <summary>表の追加</summary>
        /// <param name="dtTbl">表</param>
        public void Add(DTTable dtTbl)
        {
            // 表の追加
            this._tbls.Add(dtTbl);

            // 表名 ⇒ 表インデックスのマップ
            if (this._tblsNameIndexMap.ContainsKey(dtTbl.TableName))
            {
                // 表名が重複している。
                throw new Exception("Table name overlaps. ");
            }
            else
            {
                this._tblsNameIndexMap.Add(dtTbl.TableName, this._tbls.Count - 1);
            }
        }

        #endregion

        #region 取得（インデクサ）

        /// <summary>表を取得する</summary>
        /// <param name="tblName">表名</param>
        /// <returns>表</returns>
        public DTTable this[string tblName]
        {
            get
            {
                // 表名で取得
                return (DTTable)this._tbls[((int)this._tblsNameIndexMap[tblName])];
            }
        }

        /// <summary>表を取得する</summary>
        /// <param name="index">インデックス</param>
        /// <returns>表</returns>
        public DTTable this[int index]
        {
            get
            {
                // インデックスで取得
                return (DTTable)this._tbls[index];
            }
        }

        #endregion

        #region サポート情報

        /// <summary>表数の取得</summary>
        public int Count
        {
            get
            {
                return this._tbls.Count;
            }
        }

        #endregion

        #endregion

        #region 列挙

        /// <summary>列挙子を取得</summary>
        public IEnumerator GetEnumerator()
        {
            return this._tbls.GetEnumerator();
        }

        #endregion

        #region DataSetとの相互変換

        // DTTable の ToDataTable / FromDataTable を、表の数だけ回すだけである。
        // DTTables が DataSet に対応する層なので、ここに置く（#544）。
        //
        // ＜移らないもの＞
        //   ・DataSet の名前（DataSetName）… DTTables は名前を持たない
        //   ・リレーション（Relations）と制約（Constraints）… 同上
        //   移すのは「表・列・行・値・行ステータス」だけである。

        /// <summary>System.Data.DataSetをDTTablesに変換する</summary>
        /// <param name="ds">変換元のSystem.Data.DataSet</param>
        /// <returns>変換後のDTTables</returns>
        /// <remarks>
        /// **同じ名前の表が複数あると例外になる。**
        /// DTTables は表名でも引けるようにするため、名前の重複を許さない。
        /// </remarks>
        public static DTTables FromDataSet(DataSet ds)
        {
            DTTables dtts = new DTTables();

            foreach (DataTable dt in ds.Tables)
            {
                dtts.Add(DTTable.FromDataTable(dt));
            }

            return dtts;
        }

        /// <summary>DTTablesをSystem.Data.DataSetに変換する</summary>
        /// <returns>変換後のSystem.Data.DataSet</returns>
        /// <remarks>行ステータス（RowState）は保たれる。詳細は DTTable.ToDataTable。</remarks>
        public DataSet ToDataSet()
        {
            DataSet ds = new DataSet();

            foreach (DTTable dt in this._tbls)
            {
                ds.Tables.Add(dt.ToDataTable());
            }

            return ds;
        }

        #endregion

        #region セーブ＆ロード（テキスト化）

        /// <summary>テキストとしてセーブする</summary>
        /// <param name="tw">任意のTextWriter </param>
        public void Save(TextWriter tw)
        {
            // 表番号の初期化（負荷テスト用のID用）
            int tblNo = -1;

            foreach (DTTable dt in this._tbls)
            {
                // 表番号のインクリメント
                tblNo++;

                // 表名
                tw.WriteLine("tbl:" + dt.TableName);

                tw.WriteLine("---");

                // 列情報
                foreach (DTColumn col in dt.Cols)
                {
                    tw.WriteLine("col:" + col.ColName + "," + DTColumn.EnumToString(col.ColType));
                }

                tw.WriteLine("---");

                // 行番号の初期化（負荷テスト用のID用）
                int rowNo = -1;

                // 行のセル
                foreach (DTRow dr in dt.Rows)
                {
                    // 行番号のインクリメント
                    rowNo++;

                    // 列番号の初期化（負荷テスト用のID用）
                    int colNo = -1;

                    foreach (object o in dr)
                    {
                        // 列番号のインクリメント
                        colNo++;

                        string strTemp = DTColumn.StringFromPrimitivetype(o, true);

                        // **null は "null" と書く。**（#544）
                        //
                        //   Load 側には元から「celString == "null" なら値を設定しない」
                        //   という受け口があるのに、こちらが書いていなかった。
                        //   null は空文字として書かれ、読み込み時に
                        //   Convert.ToDecimal("") などが FormatException を投げていた。
                        //
                        //   ＜値が文字列の "null" だった場合＞
                        //     読み戻すと null になる。行単位・文字列だけの形式なので、
                        //     ここは区別できない。値を厳密に往復させたいときは
                        //     JSON 版（SaveJson / LoadJson）を使うこと。
                        //     あちらは JSON の null をそのまま使うため、曖昧さが無い。
                        tw.WriteLine(
                                  "cel:"
                                  + tblNo.ToString() + "," + rowNo.ToString() + "," + colNo.ToString() +
                                  ":" + (strTemp == null ? "null" : strTemp));
                    }

                    // 行ステータス
                    tw.WriteLine("row:" + (int)dr.RowState);

                    tw.WriteLine("---");
                }
            }
        }

        /// <summary>テキストからロードする</summary>
        /// <param name="tr">任意のTextReader </param>
        public void Load(TextReader tr)
        {
            // 初期化
            this._tbls = new List<DTTable>();
            this._tblsNameIndexMap = new Dictionary<string, int>();

            // ワーク
            string temp = "";

            DTTable tbl = null;
            DTColumn col = null;
            DTRow row = null;

            int bkColIndex = 0;

            while (true)
            {
                string line = tr.ReadLine();

                // 入力がなくなったら、ループを抜ける
                if (line == null) { break; }

                if (line.Length >= 4) // rnn:,rnrも考慮し「>=」とした。
                {
                    switch (line.Substring(0, 4))
                    {
                        case "tbl:":

                            // 表名
                            string tblName = line.Substring(4);

                            // 表を生成
                            tbl = new DTTable(tblName);

                            // 表を追加
                            this.Add(tbl);

                            break;

                        case "col:":

                            // 列情報
                            temp = line.Substring(4);
                            string colName = temp.Split(',')[0];
                            string colType = temp.Split(',')[1];

                            // 列を生成
                            col = new DTColumn(colName, DTColumn.StringToEnum(colType));

                            // 列を追加
                            tbl.Cols.Add(col);

                            break;

                        case "row:":

                            // 行ステータス
                            row.RowState = (DataRowState)int.Parse(line.Substring(4));

                            break;

                        case "cel:":

                            // セル情報
                            temp = line.Substring(4);
                            int clnIndex = temp.IndexOf(":");
                            int colIndex = int.Parse(temp.Substring(0, clnIndex).Split(',')[2]);
                            string celString = temp.Substring(clnIndex + 1);

                            // 列インデックスをチェック
                            if (colIndex == 0)
                            {
                                // 新しい行
                                row = tbl.Rows.AddNew();
                            }
                            else
                            {
                                // 継続行
                            }

                            // セルに値を設定
                            if (celString == "null")
                            {
                                // row[colIndex] = null;
                            }
                            else
                            {
                                // 列情報
                                col = (DTColumn)tbl.Cols.ColsInfo[colIndex];

                                // String もそのまま返るようになったため、分岐は要らない（#544）
                                row[colIndex] = DTColumn.PrimitivetypeFromString(col.ColType, celString);

                                if (col.ColType == DTType.String)
                                {
                                    // 改行の継続行（rnr: / rnn:）を連結する先として、インデックスを退避
                                    bkColIndex = colIndex;
                                }
                            }

                            break;

                        case "rnr:":

                            // 文字列の続き

                            temp = line.Substring(4);
                            row[bkColIndex] += "\r" + temp;

                            break;

                        case "rnn:":

                            // 文字列の続き

                            temp = line.Substring(4);
                            row[bkColIndex] += "\n" + temp;

                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    // 捨て
                }
            }
        }

        #endregion

        #region セーブ＆ロード（JSON化）

        #region 中間生成物（JSONとの相互変換に使うクラス）

        // ＜なぜ DTTables を直接シリアライズしないのか＞（#544）
        //   DTTables / DTTable / DTRow は内部にインデックスのマップや行数の
        //   カウンタを持ち、そのままでは JSON として素直な形にならない。
        //   間に単純なクラスを挟み、JSON の構造をこちらで決める。
        //
        // ＜列と行を「配列」で持つ理由＞
        //   ・JSON オブジェクトのキー順は、本来は無保証。
        //     列を Dictionary で持つと、行の値と列の対応が順序に依存して危うい。
        //   ・行ステータスをセルと同じ階層に置くと、
        //     「rowstate」という名前の列があったときに壊れる。
        //   このため列も行のセルも配列にし、行ステータスはセルの外に出してある。
        //
        // ＜ASP.NET Core から使うとき＞
        //   このクラス群はプレーンなクラスと List だけで出来ているため、
        //   コントローラの戻り値にすれば System.Text.Json でもそのまま直列化できる。
        //   ToJsonObject / FromJsonObject はそのために公開している。

        /// <summary>JSONとの相互変換に使う中間生成物（表コレクション）</summary>
        public class JsonTables
        {
            /// <summary>表</summary>
            public List<JsonTable> tbls { get; set; }
        }

        /// <summary>JSONとの相互変換に使う中間生成物（表）</summary>
        public class JsonTable
        {
            /// <summary>表名</summary>
            public string tbl { get; set; }

            /// <summary>列（順序に意味があるため配列）</summary>
            public List<JsonColumn> cols { get; set; }

            /// <summary>行</summary>
            public List<JsonRow> rows { get; set; }
        }

        /// <summary>JSONとの相互変換に使う中間生成物（列）</summary>
        public class JsonColumn
        {
            /// <summary>列名</summary>
            public string name { get; set; }

            /// <summary>列の型（DTTypeの文字列表現）</summary>
            public string type { get; set; }
        }

        /// <summary>JSONとの相互変換に使う中間生成物（行）</summary>
        public class JsonRow
        {
            /// <summary>セル（列と同じ順序。nullはnullのまま）</summary>
            public List<string> cels { get; set; }

            /// <summary>行ステータス（DataRowStateの数値）</summary>
            public int state { get; set; }
        }

        #endregion

        #region 中間生成物との相互変換

        /// <summary>中間生成物に変換する</summary>
        /// <returns>JsonTables</returns>
        public JsonTables ToJsonObject()
        {
            JsonTables jTbls = new JsonTables();
            jTbls.tbls = new List<JsonTable>();

            foreach (DTTable dt in this._tbls)
            {
                JsonTable jTbl = new JsonTable();
                jTbl.tbl = dt.TableName;

                // 列情報
                jTbl.cols = new List<JsonColumn>();
                foreach (DTColumn col in dt.Cols)
                {
                    JsonColumn jCol = new JsonColumn();
                    jCol.name = col.ColName;
                    jCol.type = DTColumn.EnumToString(col.ColType);
                    jTbl.cols.Add(jCol);
                }

                // 行のセル
                jTbl.rows = new List<JsonRow>();
                foreach (DTRow dr in dt.Rows)
                {
                    JsonRow jRow = new JsonRow();
                    jRow.cels = new List<string>();

                    foreach (object o in dr)
                    {
                        // テキスト版と違い、改行のエスケープは行わない（false）。
                        // 行単位で区切る形式ではなく、エスケープは JSON 側の仕事のため。
                        jRow.cels.Add(DTColumn.StringFromPrimitivetype(o, false));
                    }

                    jRow.state = (int)dr.RowState;
                    jTbl.rows.Add(jRow);
                }

                jTbls.tbls.Add(jTbl);
            }

            return jTbls;
        }

        /// <summary>中間生成物から復元する</summary>
        /// <param name="jTbls">JsonTables</param>
        public void FromJsonObject(JsonTables jTbls)
        {
            // 初期化
            this._tbls = new List<DTTable>();
            this._tblsNameIndexMap = new Dictionary<string, int>();

            if (jTbls == null || jTbls.tbls == null) { return; }

            foreach (JsonTable jTbl in jTbls.tbls)
            {
                // 表を生成して追加
                DTTable tbl = new DTTable(jTbl.tbl);
                this.Add(tbl);

                // 列情報
                if (jTbl.cols != null)
                {
                    foreach (JsonColumn jCol in jTbl.cols)
                    {
                        tbl.Cols.Add(new DTColumn(jCol.name, DTColumn.StringToEnum(jCol.type)));
                    }
                }

                if (jTbl.rows == null) { continue; }

                // 行のセル
                foreach (JsonRow jRow in jTbl.rows)
                {
                    // AddNew は行ステータスを Added にするため、値を入れ終えてから戻す。
                    DTRow row = tbl.Rows.AddNew();

                    if (jRow.cels != null)
                    {
                        for (int i = 0; i < jRow.cels.Count; i++)
                        {
                            string cel = jRow.cels[i];

                            // null は設定しない（DTRow の初期値のままにする）。
                            if (cel == null) { continue; }

                            DTColumn col = (DTColumn)tbl.Cols.ColsInfo[i];
                            row[i] = DTColumn.PrimitivetypeFromString(col.ColType, cel);
                        }
                    }

                    // 行ステータス（値を設定すると Modified になるため、最後に戻す）
                    row.RowState = (DataRowState)jRow.state;
                }
            }
        }

        #endregion

        #region セーブ＆ロード

        /// <summary>JSONとしてセーブする</summary>
        /// <param name="tw">任意のTextWriter</param>
        public void SaveJson(TextWriter tw)
        {
            tw.Write(JsonConvert.SerializeObject(this.ToJsonObject()));
        }

        /// <summary>JSONからロードする</summary>
        /// <param name="tr">任意のTextReader</param>
        public void LoadJson(TextReader tr)
        {
            this.FromJsonObject(JsonConvert.DeserializeObject<JsonTables>(tr.ReadToEnd()));
        }

        #endregion

        #endregion

        #region マーシャリングのサポート メソッド

        /// <summary>
        /// 汎用DTOのマーシャル処理
        /// </summary>
        public static string DTTablesToString(DTTables dtts)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            try
            {
                dtts.Save(sw);
                return sb.ToString();
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// 汎用DTOのアンマーシャル処理
        /// </summary>
        public static DTTables StringToDTTables(string str)
        {
            StringReader sr = new StringReader(str);

            try
            {
                DTTables dtts = new DTTables();
                dtts.Load(sr);
                return dtts;
            }
            finally
            {
                sr.Close();
            }
        }

        /// <summary>
        /// 汎用DTOのマーシャル処理（JSON）
        /// </summary>
        public static string DTTablesToJson(DTTables dtts)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            try
            {
                dtts.SaveJson(sw);
                return sb.ToString();
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// 汎用DTOのアンマーシャル処理（JSON）
        /// </summary>
        public static DTTables JsonToDTTables(string str)
        {
            StringReader sr = new StringReader(str);

            try
            {
                DTTables dtts = new DTTables();
                dtts.LoadJson(sr);
                return dtts;
            }
            finally
            {
                sr.Close();
            }
        }

        #endregion
    }
}

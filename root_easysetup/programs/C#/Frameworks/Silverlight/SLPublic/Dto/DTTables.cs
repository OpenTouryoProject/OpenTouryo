//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//**********************************************************************************

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Touryo.Infrastructure.Public.Dto
{
    /// <summary>表コレクション</summary>
    [System.Diagnostics.DebuggerStepThrough]
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

                        if (o == null)
                        {
                            // null値はnullと出力する。
                            tw.WriteLine(
                                "cel:"
                                + tblNo.ToString() + "," + rowNo.ToString() + "," + colNo.ToString() +
                                ":" + "null");
                        }
                        else if (DTColumn.CheckType(o, DTType.String))
                        {
                            // 文字列は改行を処理する
                            string strTemp = ((string)o);
                            strTemp = strTemp.Replace("\r", "\rrnr:");
                            strTemp = strTemp.Replace("\n", "\rrnn:");

                            strTemp = strTemp.Replace("\r", "\r\n");
                            tw.WriteLine(
                                "cel:"
                                + tblNo.ToString() + "," + rowNo.ToString() + "," + colNo.ToString() +
                                ":" + strTemp);
                        }
                        else if (DTColumn.CheckType(o, DTType.ByteArray))
                        {
                            // バイト配列は、Base64エンコードして電文に乗せる
                            string strBase64 = Convert.ToBase64String((byte[])o);
                            tw.WriteLine(
                                "cel:"
                                + tblNo.ToString() + "," + rowNo.ToString() + "," + colNo.ToString() +
                                ":" + strBase64);
                        }
                        else if (DTColumn.CheckType(o, DTType.DateTime))
                        {
                            // DateTimeは、yyyy/M/d-H:m:s.fffとする。
                            DateTime dttm = (DateTime)o;
                            string strDttm = "";

                            strDttm += dttm.Year + "/";
                            strDttm += dttm.Month + "/";
                            strDttm += dttm.Day + "-";

                            strDttm += dttm.Hour + ":";
                            strDttm += dttm.Minute + ":";
                            strDttm += dttm.Second + ".";
                            strDttm += dttm.Millisecond;

                            tw.WriteLine(
                                "cel:"
                                + tblNo.ToString() + "," + rowNo.ToString() + "," + colNo.ToString() +
                                ":" + strDttm);
                        }
                        else
                        {
                            // 通常通り、ToStringして出力
                            tw.WriteLine(
                                "cel:"
                                + tblNo.ToString() + "," + rowNo.ToString() + "," + colNo.ToString() +
                                ":" + o.ToString());
                        }
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

                                if (col.ColType == DTType.String)
                                {
                                    // そのまま
                                    row[colIndex] = celString;
                                    // インデックスを退避
                                    bkColIndex = colIndex;
                                }
                                else if (col.ColType == DTType.ByteArray)
                                {
                                    // バイト配列は、Base64デコードする。
                                    byte[] celByte = Convert.FromBase64String(celString);
                                    row[colIndex] = celByte;
                                }
                                else if (col.ColType == DTType.DateTime)
                                {
                                    // DateTimeは、yyyy/M/d-H:m:s.fff
                                    string ymd = celString.Split('-')[0];
                                    string hmsf = celString.Split('-')[1];

                                    DateTime cellDttm = new DateTime(
                                        int.Parse(ymd.Split('/')[0]),
                                        int.Parse(ymd.Split('/')[1]),
                                        int.Parse(ymd.Split('/')[2]),
                                        int.Parse(hmsf.Split(':')[0]),
                                        int.Parse(hmsf.Split(':')[1]),
                                        int.Parse(hmsf.Split(':')[2].Split('.')[0]),
                                        int.Parse(hmsf.Split(':')[2].Split('.')[1]));

                                    row[colIndex] = cellDttm;
                                }
                                else
                                {
                                    // 型変換を試みる。
                                    row[colIndex] = DTColumn.AutoCast(col.ColType, celString);
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

        #endregion
    }
}

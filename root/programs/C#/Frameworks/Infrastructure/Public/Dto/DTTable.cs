//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//* クラス名        ：DTTable
//* クラス日本語名  ：マーシャリング機能付き汎用DTO（表クラス）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/03/xx  西野  大介        新規作成
//*  2010/11/11  前川  祐介        メソッド追加
//*                                  一覧更新処理対応
//*                                    ・AcceptChanges、GetChanges
//*                                  Datatable対応
//*                                    ・ToDataTable、FromDataTable
//*                                    ・ConvertDTTypeToType、ConvertTypeToDTType
//*  2011/10/09  西野  大介        国際化対応
//*  2015/28/01  Supragyan         Crated CreateSampleData,GetJson method for creating and selecting data.
//*  2015/28/01  Supragyan         Crated JQGridDataClass,JQGridDataRowClass initializing properties.
//**********************************************************************************
//system
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Web;

//Newtonsoft
using Newtonsoft.Json;

namespace Touryo.Infrastructure.Public.Dto
{
    /// <summary>表クラス</summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class DTTable
    {
        #region インスタンス変数

        /// <summary>表情報の共有</summary>
        private DTTableStatus _tblStat = new DTTableStatus();

        /// <summary>表名</summary>
        private string _tblName = "";

        #region 行列コレクション

        /// <summary>列コレクション</summary>
        private DTColumns _cols;

        /// <summary>行コレクション</summary>
        private DTRows _rows;

        #endregion

        #endregion

        #region プロパティ

        /// <summary>表名</summary>
        /// <remarks>確認専用（変更されないように）</remarks>
        public string TableName
        {
            get
            {
                return this._tblName;
            }
        }

        #region 行列コレクション

        /// <summary>列コレクション</summary>
        public DTColumns Cols
        {
            get
            {
                return this._cols;
            }
        }

        /// <summary>行コレクション</summary>
        public DTRows Rows
        {
            get
            {
                return this._rows;
            }
        }

        #endregion

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="tblName">表名</param>
        public DTTable(string tblName)
        {
            // 表名の文字制限（正規表現チェックだが、
            // コンバートのことを考え棟梁部品は使用しない。）
            Regex rgx = new Regex("^[a-zA-Z0-9_-]+$");
            Match mch = rgx.Match(tblName);

            if (mch.Success)
            {
                // 表名が正しい

                // 表名を設定
                this._tblName = tblName;

                // 循環参照にならないように、表情報の共有オブジェクトを渡す。
                this._cols = new DTColumns(this._tblStat);
                this._rows = new DTRows(this._tblStat, this._cols);
            }
            else
            {
                // 表名が不正
                throw new Exception(
                    "A table name is inaccurate. "
                    + " Regular expression of the character which can be used:"
                    + " \"^[a-zA-Z0-9_-]+$\"");
            }
        }

        #endregion

        /// <summary>
        /// DTTableをSystem.Data.DataTableに変換する
        /// </summary>
        /// <returns>変換後のSystem.Data.DataTable</returns>
        public DataTable ToDataTable()
        {
            // DataTable 変換前に、変更を確定させる
            this.AcceptChanges();

            // テーブル定義
            DataTable dt = new DataTable(this.TableName);

            // 列定義
            foreach (DTColumn col in this.Cols)
            {
                Type type = this.ConvertDTTypeToType(col.ColType);
                dt.Columns.Add(col.ColName, type);
            }

            // 値追加
            DataRow dr;
            foreach (DTRow row in this.Rows)
            {
                // 行を新規作成
                dr = dt.NewRow();

                // 各列ごとに値を追加
                foreach (DTColumn col in this.Cols)
                {
                    dr[col.ColName] = row[col.ColName];
                }

                // 行をテーブルに追加
                dt.Rows.Add(dr);
            }

            // 値を確定させておく
            dt.AcceptChanges();

            // DataTable を返す
            return dt;
        }

        /// <summary>
        /// System.Data.DataTableをDTTableに変換する
        /// </summary>
        /// <param name="table">変換元のSystem.Data.DataTable</param>
        /// <returns>変換後のDTTable</returns>
        public static DTTable FromDataTable(DataTable table)
        {
            // テーブル定義
            DTTable dt = new DTTable(table.TableName);

            // 列定義
            foreach (DataColumn col in table.Columns)
            {
                DTType type = ConvertTypeToDTType(col.DataType);
                dt.Cols.Add(new DTColumn(col.ColumnName, type));
            }

            // 値追加(削除された行以外)
            DTRow dr;
            foreach (DataRow row in table.Rows)
            {
                // 行をテーブルに追加
                dr = dt.Rows.AddNew();

                // 各列ごとに値を追加
                foreach (DataColumn col in table.Columns)
                {
                    if (row.RowState != System.Data.DataRowState.Deleted)
                    {
                        dr[col.ColumnName] = row[col.ColumnName];
                    }
                    else
                    {
                        // 行が削除されている場合は、元の値を取得する
                        dr[col.ColumnName] = row[col.ColumnName, DataRowVersion.Original];
                    }
                }

                // 行ステータスを復元
                if (row.RowState == System.Data.DataRowState.Detached)
                {
                    dr.RowState = DataRowState.Detached;
                }
                else if (row.RowState == System.Data.DataRowState.Added)
                {
                    dr.RowState = DataRowState.Added;
                }
                else if (row.RowState == System.Data.DataRowState.Modified)
                {
                    dr.RowState = DataRowState.Modified;
                }
                else if (row.RowState == System.Data.DataRowState.Deleted)
                {
                    dr.RowState = DataRowState.Deleted;
                }
                else
                {
                    dr.RowState = DataRowState.Unchanged;
                }
            }

            // DTTableを返す
            return dt;
        }

        /// <summary>
        /// DTTypeの値を、対応するType型の値に変換する
        /// </summary>
        /// <param name="dtType">DTTypeの値</param>
        /// <returns>変換後のType型の値</returns>
        private Type ConvertDTTypeToType(DTType dtType)
        {
            Type retType;   // 変換後のType型の値

            // DTTypeは列挙型なので、switch文を使用する
            switch (dtType)
            {
                case DTType.Boolean:
                    retType = typeof(bool);
                    break;
                case DTType.ByteArray:
                    retType = typeof(byte[]);
                    break;
                case DTType.Char:
                    retType = typeof(char);
                    break;
                case DTType.DateTime:
                    retType = typeof(DateTime);
                    break;
                case DTType.Decimal:
                    retType = typeof(decimal);
                    break;
                case DTType.Double:
                    retType = typeof(double);
                    break;
                case DTType.Int16:
                    retType = typeof(short);
                    break;
                case DTType.Int32:
                    retType = typeof(int);
                    break;
                case DTType.Int64:
                    retType = typeof(long);
                    break;
                case DTType.Single:
                    retType = typeof(float);
                    break;
                case DTType.String:
                    retType = typeof(string);
                    break;
                default:
                    // 通常はここに来ることはない
                    retType = typeof(string);
                    break;
            }

            // 変換後のType型の値を返す
            return retType;
        }

        /// <summary>
        /// Type型の値を、対応するDTType型の値に変換する
        /// </summary>
        /// <param name="type">Type型の値</param>
        /// <returns>変換後のDTType型の値</returns>
        private static DTType ConvertTypeToDTType(Type type)
        {
            DTType retDTType;   // 変換後のDTType型の値

            // Type型はswitch文が使えないため、if文で記述する
            if (type == typeof(bool))
            {
                retDTType = DTType.Boolean;
            }
            else if (type == typeof(byte[]))
            {
                retDTType = DTType.ByteArray;
            }
            else if (type == typeof(char))
            {
                retDTType = DTType.Char;
            }
            else if (type == typeof(DateTime))
            {
                retDTType = DTType.DateTime;
            }
            else if (type == typeof(decimal))
            {
                retDTType = DTType.Decimal;
            }
            else if (type == typeof(double))
            {
                retDTType = DTType.Double;
            }
            else if (type == typeof(short))
            {
                retDTType = DTType.Int16;
            }
            else if (type == typeof(int))
            {
                retDTType = DTType.Int32;
            }
            else if (type == typeof(long))
            {
                retDTType = DTType.Int64;
            }
            else if (type == typeof(float))
            {
                retDTType = DTType.Single;
            }
            else if (type == typeof(string))
            {
                retDTType = DTType.String;
            }
            else
            {
                retDTType = DTType.String;
            }

            // 変換後のDTType型の値を返す
            return retDTType;
        }

        /// <summary>
        /// 修正された(Added, Modified, Deleted)行を取得する
        /// </summary>
        /// <returns></returns>
        public DTRows GetChanges()
        {
            return this._rows.Find(
                DataRowState.Added
                | DataRowState.Deleted
                | DataRowState.Modified);
        }

        /// <summary>
        /// 行の変更を確定させる
        /// </summary>
        public void AcceptChanges()
        {
            int i = 0;  // カウンタ用

            while (i < this.Rows.Count)
            {
                if (this.Rows[i].RowState == DataRowState.Deleted)
                {
                    // 行リストから明示的に削除する
                    this.Rows.DeleteFromList(i);
                }
                else
                {
                    // 行ステータスをUnchangedに変え、行の値を確定させる
                    this.Rows[i].RowState = DataRowState.Unchanged;
                    i++;
                }
            }
        }

        #region GetJson

        /// <summary>
        /// GetJson method for selecting data
        /// </summary>
        /// <returns></returns>
        public JQGridDataClass GetJson()
        {
            // サンプルデータ取得
            System.Data.DataTable dt = this.CreateSampleData();

            // クエリ文字列から、グリッドの表示に関する情報を取得
            System.Collections.Specialized.NameValueCollection queryStrings = HttpContext.Current.Request.QueryString;
            string page = queryStrings["page"]; // クエリの現在のページ
            string rows = queryStrings["rows"]; // 1 ページあたりの行数
            string sidx = queryStrings["sidx"]; // ソート列
            string sord = queryStrings["sord"]; // ソートの方向
            int intPage = int.Parse(page);
            int intRows = Math.Min(int.Parse(rows), dt.Rows.Count);

            // jqGrid に渡すデータを格納
            JQGridDataClass data = new JQGridDataClass();
            data.page = page;
            data.total = (int)Math.Ceiling((double)dt.Rows.Count / (double)intRows);
            data.records = dt.Rows.Count;

            // ソート
            System.Data.DataRow[] dataRows;
            if (!string.IsNullOrEmpty(sidx))
            {
                // ソートあり
                dataRows = dt.Select(null, sidx + " " + sord);
            }
            else
            {
                // ソートなし
                dataRows = dt.Select(null);
            }

            // jqGrid の各セルに表示するデータを格納
            data.rows = new System.Collections.Generic.List<JQGridDataRowClass>();
            for (int rIndex = 0; rIndex < intRows; rIndex++)
            {
                // ページなどを考慮し、DataTable から取得する行インデックスを取得
                int actualRowIndex = ((intPage - 1) * intRows) + rIndex;

                if ((actualRowIndex + 1) > dataRows.Length)
                {
                    break;
                }

                // jqGrid に表示する、1 行分のデータを格納
                JQGridDataRowClass subData = new JQGridDataRowClass();
                subData.id = dataRows[actualRowIndex][0].ToString();
                subData.cell = new System.Collections.Generic.List<string>();

                for (int cIndex = 0; cIndex < dt.Columns.Count; cIndex++)
                {
                    // 列のデータを、DataTable から取得
                    subData.cell.Add(dataRows[actualRowIndex][cIndex].ToString());
                }
                data.rows.Add(subData);
            }
            // jqGrid 
            return data;
        }

        #endregion

        #region CreateSampleData

        /// <summary>
        /// Creates Sample Data in datatable.
        /// </summary>
        /// <returns></returns>
        private System.Data.DataTable CreateSampleData()
        {
            System.Data.DataTable dt = new System.Data.DataTable("dummy");
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("email", typeof(string));
            dt.Columns.Add("phone", typeof(string));

            System.Data.DataRow row;
            for (int index = 0; index < 105; index++)
            {
                row = dt.NewRow();
                row["id"] = index + 1;
                row["name"] = "xxx";
                row["email"] = "xxx.xxx.xx@hitachi-solutions.com";
                row["phone"] = "xxxx-xxxx";
                dt.Rows.Add(row);
            }

            return dt;
        }

        #endregion

        #region JQGridDataClass

        /// <summary>
        /// jqGrid に渡すデータを格納するクラス
        /// </summary>
        public class JQGridDataClass
        {
            /// <summary>
            /// クエリの現在のページ
            /// </summary>
            public string page;

            /// <summary>
            /// クエリの総ページ数
            /// </summary>
            public int total;

            /// <summary>
            /// クエリに対するレコードの総数
            /// </summary>
            public int records;

            /// <summary>
            /// 実際のデータを含むリスト
            /// </summary>
            public System.Collections.Generic.List<JQGridDataRowClass> rows;
        }

        #endregion

        #region JQGridDataRowClass

        /// <summary>
        /// jqGrid に表示する、1 行分のデータを格納するクラス
        /// </summary>
        public class JQGridDataRowClass
        {
            /// <summary>
            /// 列の固有 ID
            /// </summary>
            public string id;

            /// <summary>
            /// 列のデータを含むリスト
            /// </summary>
            public System.Collections.Generic.List<string> cell;
        }

        #endregion
    }
}

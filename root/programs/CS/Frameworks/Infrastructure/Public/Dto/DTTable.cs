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
//*  2026/08/14  玄人 幸道         ToDataTableで行ステータスを保つようにした（#544）。
//*  2026/08/14  玄人 幸道         DataRowStateをDTRowStateに改名（#544）。
//*  2026/08/18  玄人 幸道         KeepOriginalを追加し、変更前の値を保持できるようにした（#567）。
//**********************************************************************************

using System;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        /// <summary>変更前の値（Original）を保持するか</summary>
        /// <remarks>
        /// **既定は false（保持しない）。**（#567）
        ///
        /// ＜有効にする場面＞
        ///   全列の変更前の値を WHERE 条件に使う楽観排他を、
        ///   DTTables 経由（WebAPI 転送・Session 往復）でも効かせたい場合。
        ///   保持しないと、往復後の Original に現在値が入り、
        ///   条件が必ず一致して他者の更新を上書きする。
        ///
        /// ＜有効にしなくてよい場面＞
        ///   timestamp（rowversion）列で排他する構成。
        ///   その列 1 つで衝突が分かるため、全列を持ち回るのは負担になるだけ。
        ///
        /// ＜設定するタイミング＞
        ///   **編集を始める前に設定すること。**
        ///   変更のたびに退避するかどうかを見るため、
        ///   途中で有効にしても、それ以前の変更は退避されない。
        /// </remarks>
        public bool KeepOriginal
        {
            set
            {
                this._tblStat.KeepOriginal = value;
            }
            get
            {
                return this._tblStat.KeepOriginal;
            }
        }

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
        /// <remarks>
        /// 行ステータス（RowState）を保って変換する（#544）。
        ///
        /// ＜以前は失われていた＞
        ///   変換の前後で AcceptChanges を呼んでいたため、全行が Unchanged になり、
        ///   Deleted の行は消えていた。FromDataTable が行ステータスを復元しているのに、
        ///   戻すと落ちる、という非対称な状態だった。
        ///   これでは受け取った側で Added / Modified / Deleted の振り分けができない。
        ///
        /// ＜Modified の元の値＞
        ///   **KeepOriginal が真なら復元される。**（#567）
        ///   偽の場合は DTRow が変更前の値を持たないため、
        ///   DataRowVersion.Original には現在値が入る（従来どおり）。
        ///
        ///   全列の Original を WHERE 条件に使う楽観排他を DTTables 経由で行うなら、
        ///   KeepOriginal を有効にすること。timestamp 列で排他する構成では不要。
        /// </remarks>
        public DataTable ToDataTable()
        {
            // テーブル定義
            DataTable dt = new DataTable(this.TableName);

            // 列定義
            foreach (DTColumn col in this.Cols)
            {
                Type type = this.ConvertDTTypeToType(col.ColType);
                dt.Columns.Add(col.ColName, type);
            }

            // 値追加
            //
            // Detached（どの行リストにも属さない）は DataTable では表せないため、
            // ここでは追加しない。行ステータスを戻すとき、位置がずれないよう
            // DTRow との対応を控えておく。
            List<DTRow> added = new List<DTRow>();

            foreach (DTRow row in this.Rows)
            {
                if (row.RowState == DTRowState.Detached) { continue; }

                // 行を新規作成
                DataRow dr = dt.NewRow();

                // **Original を持つ行は、まず変更前の値で入れる。**（#567）
                //   このあと AcceptChanges で確定させると、それが Original になる。
                //   現在値は、行ステータスを復元するときに上書きする。
                bool useOriginal = (row.RowState == DTRowState.Modified && row.HasOriginal);

                // 各列ごとに値を追加
                foreach (DTColumn col in this.Cols)
                {
                    object value = useOriginal
                        ? row[col.ColName, DTRowVersion.Original]
                        : row[col.ColName];

                    // **null は DBNull に置き換える。**（#544）
                    //   DTRow は値なしを null で持つが、DataRow は null を受け付けず
                    //   「Column を null に設定できません。DBNull を使用してください。」
                    //   と ArgumentException を投げる。
                    //   文字列の列は通ってしまうため、値型の列で初めて落ちる。
                    dr[col.ColName] = (value == null) ? DBNull.Value : value;
                }

                // 行をテーブルに追加
                dt.Rows.Add(dr);
                added.Add(row);
            }

            // いったん確定させ、全行を Unchanged にする。
            // SetAdded / SetModified は Unchanged の行にしか使えないため、この順である。
            dt.AcceptChanges();

            // 行ステータスを復元する
            for (int i = 0; i < added.Count; i++)
            {
                switch (added[i].RowState)
                {
                    case DTRowState.Added:
                        dt.Rows[i].SetAdded();
                        break;

                    case DTRowState.Modified:
                        // **確定済みの値が Original になっている。**（#567）
                        //   ここで現在値を当てると、Original ≠ Current の行になる。
                        if (added[i].HasOriginal)
                        {
                            foreach (DTColumn col in this.Cols)
                            {
                                object value = added[i][col.ColName];
                                dt.Rows[i][col.ColName] = (value == null) ? DBNull.Value : value;
                            }
                        }

                        // **SetModified は Unchanged の行にしか使えない。**
                        //   上で値を当てた場合、その時点で Modified になっているため、
                        //   ここで呼ぶと InvalidOperationException になる。
                        //   値がすべて同じで Unchanged のままの場合もあるので、状態で判断する。
                        if (dt.Rows[i].RowState == System.Data.DataRowState.Unchanged)
                        {
                            dt.Rows[i].SetModified();
                        }
                        break;

                    case DTRowState.Deleted:
                        // Delete しても行は Rows に残る（RowState が Deleted になる）ため、
                        // 以降の添字はずれない。
                        dt.Rows[i].Delete();
                        break;

                    default:
                        // Unchanged は、AcceptChanges 済みの状態がそのまま当てはまる。
                        break;
                }
            }

            // DataTable を返す
            return dt;
        }

        /// <summary>
        /// System.Data.DataTableをDTTableに変換する
        /// </summary>
        /// <param name="table">変換元のSystem.Data.DataTable</param>
        /// <param name="keepOriginal">
        /// 変更前の値（Original）を保持するか（既定は false）
        /// </param>
        /// <returns>変換後のDTTable</returns>
        /// <remarks>
        /// **keepOriginal は、ここで指定しないと取り込めない。**（#567）
        ///   生成したあとに DTTable.KeepOriginal を立てても、
        ///   変換は済んでおり、変更前の値は既に失われている。
        /// </remarks>
        public static DTTable FromDataTable(DataTable table, bool keepOriginal = false)
        {
            // テーブル定義
            DTTable dt = new DTTable(table.TableName);
            dt.KeepOriginal = keepOriginal;

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

                // **変更前の値を取り込む。**（#567）
                //   Modified の行にだけ Original がある。
                //   行ステータスを戻す前に入れる（戻したあとだと値の設定で退避が走り得る）。
                if (dt.KeepOriginal && row.RowState == System.Data.DataRowState.Modified)
                {
                    List<object> original = new List<object>();

                    foreach (DataColumn col in table.Columns)
                    {
                        object o = row[col.ColumnName, DataRowVersion.Original];
                        original.Add((o is System.DBNull) ? null : o);
                    }

                    dr.SetOriginal(original);
                }

                // 行ステータスを復元
                if (row.RowState == System.Data.DataRowState.Detached)
                {
                    dr.RowState = DTRowState.Detached;
                }
                else if (row.RowState == System.Data.DataRowState.Added)
                {
                    dr.RowState = DTRowState.Added;
                }
                else if (row.RowState == System.Data.DataRowState.Modified)
                {
                    dr.RowState = DTRowState.Modified;
                }
                else if (row.RowState == System.Data.DataRowState.Deleted)
                {
                    dr.RowState = DTRowState.Deleted;
                }
                else
                {
                    dr.RowState = DTRowState.Unchanged;
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
                DTRowState.Added
                | DTRowState.Deleted
                | DTRowState.Modified);
        }

        /// <summary>
        /// 行の変更を確定させる
        /// </summary>
        public void AcceptChanges()
        {
            int i = 0;  // カウンタ用

            while (i < this.Rows.Count)
            {
                if (this.Rows[i].RowState == DTRowState.Deleted)
                {
                    // 行リストから明示的に削除する
                    this.Rows.DeleteFromList(i);
                }
                else
                {
                    // 行ステータスをUnchangedに変え、行の値を確定させる
                    this.Rows[i].RowState = DTRowState.Unchanged;

                    // **確定したので、変更前の値は捨てる。**（#567）
                    //   現在値が確定値になるため、持ち続けると次の変更で
                    //   「もっと前の値」が Original として残ってしまう。
                    this.Rows[i].ClearOriginal();

                    i++;
                }
            }
        }

        /// <summary>
        /// 行の変更を取り消す
        /// </summary>
        /// <remarks>
        /// **KeepOriginal が真のときだけ、値が戻る。**（#567）
        ///   保持していない場合、行ステータスだけが Unchanged に戻る。
        ///
        /// Added の行は行リストから取り除き、Deleted の行は復帰させる。
        /// </remarks>
        public void RejectChanges()
        {
            int i = 0;  // カウンタ用

            while (i < this.Rows.Count)
            {
                if (this.Rows[i].RowState == DTRowState.Added)
                {
                    // 追加された行は、無かったことにする
                    this.Rows.DeleteFromList(i);
                }
                else
                {
                    // Modified は値を戻し、Deleted は復帰させる（どちらも Unchanged になる）
                    this.Rows[i].RejectChanges();
                    i++;
                }
            }
        }
    }
}

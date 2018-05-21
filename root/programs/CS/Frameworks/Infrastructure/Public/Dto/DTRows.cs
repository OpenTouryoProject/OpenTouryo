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
//* クラス名        ：DTRows
//* クラス日本語名  ：マーシャリング機能付き汎用DTO（行コレクション）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/03/xx  西野 大介         新規作成
//*  2010/11/11  前川 祐介         一覧更新処理対応
//*                                ・行ステータス
//*                                ・メソッド追加
//*                                  DeleteFromList、Find、ToDataSource
//*  2010/11/11  前川 祐介         Silverlight対応（ジェネリック）
//*  2011/10/09  西野 大介         国際化対応
//**********************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace Touryo.Infrastructure.Public.Dto
{
    /// <summary>行コレクション</summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class DTRows : IEnumerable
    {
        #region インスタンス変数

        /// <summary>行を保持するList</summary>
        private List<DTRow> _rows = new List<DTRow>();

        /// <summary>表情報の共有</summary>
        private DTTableStatus _tblStat;

        /// <summary>列コレクション</summary>
        private DTColumns _cols;

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="tblStat">表情報</param>
        /// <param name="cols">列コレクション</param>
        /// <remarks>外部からは生成できないようにする</remarks>
        internal DTRows(DTTableStatus tblStat, DTColumns cols)
        {
            this._tblStat = tblStat;
            this._cols = cols;
        }

        #endregion

        #region 行

        #region 設定

        /// <summary>行の生成と追加</summary>
        public DTRow AddNew()
        {
            // 列コレクションをチェック
            if (this._cols.Count == 0)
            {
                // 列がない
                throw new Exception("The column is not set up. ");
            }
            else
            {
                // 列がある
                DTRow dr = new DTRow(this._cols);

                // 行ステータスを "Added"(追加された行) に変更
                dr.RowState = DataRowState.Added;

                this._rows.Add(dr);

                // 行数をインクリメント
                this._tblStat.RowsCount++;

                return dr;
            }
        }

        /// <summary>行の追加</summary>
        /// <param name="dr">行</param>
        public void Add(DTRow dr)
        {
            // 列情報が同じ場合のみ追加を許可する。
            if (this._cols.ColsInfo.Equals(dr.ColsInfo))
            {
                // 列情報が同じ
                this._rows.Add(dr);

                // 行ステータスを "Added"(追加された行) に変更
                dr.RowState = DataRowState.Added;

                // 行数をインクリメント
                this._tblStat.RowsCount++;
            }
            else
            {
                // 列情報が異なる
                throw new Exception("column information differs. ");
            }
        }

        /// <summary>行の削除</summary>
        /// <param name="index">インデックス</param>
        public void Delete(int index)
        {
            if (0 < this._rows.Count)
            {
                // 行ステータスを "Deleted"(削除された行) に変更
                ((DTRow)this._rows[index]).RowState = DataRowState.Deleted;

                this._tblStat.RowsCount--;
            }
        }

        /// <summary>
        /// 物理的にリストからアイテムを削除する
        /// </summary>
        /// <param name="index">削除する行のインデックス</param>
        /// <remarks>DTTable.AcceptChanges からのみ呼ばれるメソッド</remarks>
        internal void DeleteFromList(int index)
        {
            if (0 < this._rows.Count)
            {
                this._rows.RemoveAt(index);
            }
        }

        #endregion

        #region 取得（インデクサ）

        /// <summary>行を取得する</summary>
        /// <param name="index">インデックス</param>
        /// <returns>行</returns>
        public DTRow this[int index]
        {
            get
            {
                // インデックスで取得
                return (DTRow)this._rows[index];
            }
        }

        #endregion

        #region サポート情報

        /// <summary>列数の取得</summary>
        public int Count
        {
            get
            {
                return this._rows.Count;
            }
        }

        #endregion

        #endregion

        #region 列挙

        /// <summary>列挙子を取得</summary>
        public IEnumerator GetEnumerator()
        {
            //return new DTRowsEnumerator(this._rows);
            return this._rows.GetEnumerator();
        }

        /// <summary>
        /// 指定したステータスの行を検索する
        /// </summary>
        /// <param name="rowState">行ステータス</param>
        /// <returns>指定したステータスの行</returns>
        /// <remarks>行ステータスは論理和演算で複数条件の指定が可能</remarks>
        public DTRows Find(DataRowState rowState)
        {
            DataRowState workState;     // 行ステータスの退避用

            // List型として、指定したステータスの行を取得する

            // .NET Framework 3.5 以降で開発する場合は
            // 以下の行のコメントを解除できます。
            //    List<DTRow> listRow = this._rows.Where<DTRow>(row => (row.RowState & rowState) != 0).ToList<DTRow>();

            // .NET Framework 2.0 以前で開発する場合は
            // 上記に代わり以下のコードブロックを使用 ---- start
            List<DTRow> listRow = this._rows.FindAll(
                delegate(DTRow row)
                {
                    if ((row.RowState & rowState) != 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            );
            // ------------------------------------------- end

            // List型⇒DTRows型に行をコピーする
            DTRows rows = new DTRows(this._tblStat, this._cols);
            foreach (DTRow row in listRow)
            {
                // 元の行ステータスを退避させる
                workState = row.RowState;

                // DTRowsに行を追加する
                rows.Add(row);

                // 行ステータスを元に戻す
                row.RowState = workState;
            }

            // 行リストを返す
            return rows;
        }

        /// <summary>
        /// DataGridなどへの表示用に、削除された(ステータス=Deleted)行を除く行リストを取得する
        /// </summary>
        /// <returns>削除された行を除く行リスト</returns>
        public DTRows ToDataSource()
        {
            // 削除された行を除外して返す
            return this.Find(
                DataRowState.Added
                | DataRowState.Detached
                | DataRowState.Modified
                | DataRowState.Unchanged);
        }

        #endregion
    }
}

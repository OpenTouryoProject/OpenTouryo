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
//* クラス名        ：DTRow
//* クラス日本語名  ：マーシャリング機能付き汎用DTO（列クラス）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/03/xx  西野 大介         新規作成
//*  2010/11/11  前川 祐介         一覧更新処理対応（行ステータス）
//*  2010/11/11  前川 祐介         Silverlight対応（ジェネリック）
//*  2010/11/30  前川 祐介         null設定可能に変更
//*  2010/11/30  前川 祐介         Added、Deletedは、Modifiedに変更しない
//*  2011/09/06  前川 祐介         共通化、同値が設定された場合、RowStateを変更しない
//*  2011/10/09  西野 大介         国際化対応
//*  2026/08/14  玄人 幸道         DataRowStateをDTRowStateに改名（#544）。
//*  2026/08/18  玄人 幸道         変更前の値（Original）の保持に対応（#567）
//**********************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace Touryo.Infrastructure.Public.Dto
{
    /// <summary>行クラス</summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class DTRow : IEnumerable
    {
        #region インスタンス変数

        /// <summary>列コレクション</summary>
        private DTColumns _cols;
        
        /// <summary>行のデータ本体</summary>
        private List<object> _row = new List<object>();

        /// <summary>行ステータス</summary>
        private DTRowState _rowState;

        /// <summary>変更前のデータ本体</summary>
        /// <remarks>
        /// **KeepOriginal が真で、Unchanged から初めて変更したときにだけ作る。**（#567）
        ///   null のままなら「変更前＝現在値」であり、Original を尋ねられても現在値を返す。
        ///   保持しない設定では常に null なので、記憶域は増えない。
        /// </remarks>
        private List<object> _original = null;
        
        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタでは列を必要とする。
        /// </summary>
        /// <param name="cols">列コレクション</param>
        /// <remarks>外部からは生成できないようにする</remarks>
        internal DTRow(DTColumns cols)
        {
            // 列コレクションを保持
            this._cols = cols;

            // 行データを生成する。
            foreach (DTColumn colInfo in this._cols.ColsInfo)
            {
                // 要素数分、初期値（null）
                this._row.Add(null);
            }

            // 行ステータスを初期化する
            this.RowState = DTRowState.Detached;
        }

        #endregion

        #region セル

        #region 取得（インデクサ）

        /// <summary>セルにアクセスするためのインデクサ</summary>
        /// <param name="colName">列名</param>
        /// <returns>セルの値</returns>
        public object this[string colName]
        {
            get
            {
                // 列名チェック
                if (this._cols.ColNameIndexMap.ContainsKey(colName))
                {
                    // 列名一致
                    // 2011.08.22 インデクサの処理を、インデックス番号を引数にした処理に統一
                    int index = (int)this._cols.ColNameIndexMap[colName];
                    return this[index];
                }
                else
                {
                    // 列名が不正
                    throw new Exception("A column name is inaccurate. ");
                }
            }
            set
            {
                //object temp = null;

                // 列名チェック
                if (this._cols.ColNameIndexMap.ContainsKey(colName))
                {
                    // 列名一致
                    // 2011.08.22 インデクサの処理を、インデックス番号を引数にした処理に統一
                    int index = (int)this._cols.ColNameIndexMap[colName];
                    this[index] = value;
                }
                else
                {
                    // 列名が不正
                    throw new Exception("A column name is inaccurate. ");
                }
            }
        }

        /// <summary>セルにアクセスするためのインデクサ</summary>
        /// <param name="index">インデックス</param>
        /// <returns>セルの値</returns>
        public object this[int index]
        {
            get
            {
                return this._row[index];
            }
            set
            {
                //object temp = null;

                // **値を書き換える前に、変更前の値を退避する。**（#567）
                //   書き換えたあとでは、退避されるのが「変更後の値」になってしまう。
                //   Added / Deleted の行は退避しない（変更前という概念が無い）。
                if (this.RowState != DTRowState.Added && this.RowState != DTRowState.Deleted)
                {
                    this.KeepOriginalIfNeeded();
                }

                // 列を確認し
                DTColumn dtCol = (DTColumn)this._cols[index];

                // 値の null チェック
                if (value == null || value is System.DBNull)
                {
                    // セットする値が null または DBNull の場合は、null をセットする

                    // 2011.08.22 もともと null だった場合は何もしないように修正
                    if (this._row[index] == null)
                    {
                        // もともと null だった場合は何もしない
                        return;
                    }
                    else
                    {
                        // 設定
                        this._row[index] = null;
                    }
                }
                else
                {
                    // セットする値が null でも DBNull でもない場合は、値の型チェックを行う

                    // 型が
                    if (DTColumn.CheckType(value, dtCol.ColType))
                    {
                        // 一致
                    }
                    else
                    {
                        // 一致しない

                        // 自動変換
                        value = DTColumn.AutoCast(dtCol.ColType, value);
                    }

                    // 2011.08.22 変更前後で値が変わらなかった場合は何もしないように修正
                    if (dtCol.ColType == DTType.ByteArray)
                    {
                        int len = ((byte[])value).Length;

                        // nullチェック
                        if (this._row[index] == null)
                        {
                            // nullの場合、設定
                            this._row[index] = value;
                        }
                        else
                        {
                            // nullでない場合、チェック
                            if (((byte[])this._row[index]).Length == len)
                            {
                                // 変更フラグ
                                bool isChanged = false;

                                // ループ
                                for (int i = 0; i < len; i++)
                                {
                                    if (((byte)(((byte[])this._row[index])[i])) != ((byte)((byte[])value)[i]))
                                    {
                                        // 異なる場合、設定して
                                        this._row[index] = value;
                                        // フラグを立てて
                                        isChanged = true;
                                        // ブレイク
                                        break;
                                    }
                                }

                                // 変更されていない場合、
                                if (!isChanged)
                                {
                                    // 変更前後で値が変わらなかった場合は何もしない
                                    return;
                                }
                            }
                            else
                            {
                                // 異なる場合、設定
                                this._row[index] = value;
                            }

                            //if (((byte[])value).SequenceEqual((byte[])this._row[index]))
                            //{
                            //    // 変更前後で値が変わらなかった場合は何もしない
                            //    return;
                            //}
                            //else
                            //{
                            //    // 設定
                            //    this._row[index] = value;
                            //}
                        }
                    }
                    else
                    {
                        if (value.Equals(this._row[index]))
                        {
                            // 変更前後で値が変わらなかった場合は何もしない
                            return;
                        }
                        else
                        {
                            // 設定
                            this._row[index] = value;
                        }
                    }
                }

                // 行ステータス
                if (this.RowState == DTRowState.Added || this.RowState == DTRowState.Deleted)
                {
                    // 行ステータスが "Added"(追加された行) または "Deleted"(削除された行) の場合は、行ステータスを変更しない
                }
                else
                {
                    // 上記以外の場合は、行ステータスを "Modified"(変更された行) に変更
                    this.RowState = DTRowState.Modified;
                }
            }
        }

        #endregion

        #region 変更前の値（Original）

        /// <summary>変更前の値を取得する</summary>
        /// <param name="colName">列名</param>
        /// <param name="version">セルのバージョン</param>
        /// <returns>セルの値</returns>
        /// <remarks>
        /// **Original が意味を持つのは、DTTable.KeepOriginal が真で、かつ Modified の行だけ**です。
        /// それ以外は現在の値が返ります（System.Data.DataRow のように例外にはなりません）。
        /// </remarks>
        public object this[string colName, DTRowVersion version]
        {
            get
            {
                // 列名チェック
                if (this._cols.ColNameIndexMap.ContainsKey(colName))
                {
                    int index = (int)this._cols.ColNameIndexMap[colName];
                    return this[index, version];
                }
                else
                {
                    // 列名が不正
                    throw new Exception("A column name is inaccurate. ");
                }
            }
        }

        /// <summary>変更前の値を取得する</summary>
        /// <param name="index">インデックス</param>
        /// <param name="version">セルのバージョン</param>
        /// <returns>セルの値</returns>
        public object this[int index, DTRowVersion version]
        {
            get
            {
                if (version == DTRowVersion.Original && this._original != null)
                {
                    return this._original[index];
                }

                // 保持していない場合は現在の値
                return this._row[index];
            }
        }

        /// <summary>変更前の値を保持しているか</summary>
        public bool HasOriginal
        {
            get
            {
                return (this._original != null);
            }
        }

        /// <summary>変更前の値を退避する（初回のみ）</summary>
        /// <remarks>
        /// **DTTable.KeepOriginal が真のときだけ退避する。**
        /// 既に退避済みなら何もしない（2 回目以降に上書きすると、
        /// 「1 回目の変更後の値」が変更前の値になってしまう）。
        /// </remarks>
        private void KeepOriginalIfNeeded()
        {
            if (this._original != null) { return; }
            if (!this._cols.KeepOriginal) { return; }

            this._original = new List<object>(this._row);
        }

        /// <summary>変更前の値を、外部から設定する</summary>
        /// <param name="original">変更前の値</param>
        /// <remarks>
        /// **復元のためだけに使う。**（FromDataTable、FromJsonObject）
        /// 通常の編集では KeepOriginalIfNeeded が自動で退避する。
        /// </remarks>
        internal void SetOriginal(List<object> original)
        {
            this._original = original;
        }

        /// <summary>変更前の値を破棄する</summary>
        /// <remarks>AcceptChanges から呼ばれる（現在値が確定値になるため）。</remarks>
        internal void ClearOriginal()
        {
            this._original = null;
        }

        /// <summary>変更前の値へ戻す</summary>
        /// <remarks>RejectChanges から呼ばれる。</remarks>
        internal void RejectChanges()
        {
            if (this._original != null)
            {
                this._row = this._original;
                this._original = null;
            }

            this.RowState = DTRowState.Unchanged;
        }

        #endregion

        #region サポート情報

        /// <summary>列情報の取得</summary>
        /// <remarks>変更させないため、外部に公開しない。</remarks>
        internal List<DTColumn> ColsInfo
        {
            get
            {
                return this._cols.ColsInfo;
            }
        }

        #endregion

        #endregion

        #region 列挙

        /// <summary>列挙子を取得</summary>
        public IEnumerator GetEnumerator()
        {
            //return new DTCellsEnumerator(this._row);
            return this._row.GetEnumerator();
        }

        #endregion

        /// <summary>行ステータス</summary>
        public DTRowState RowState
        {
            internal set
            {
                // 行ステータスの変更は、外部アセンブリからは不可とする
                this._rowState = value;
            }
            get
            {
                return this._rowState;
            }
        }
    }
}

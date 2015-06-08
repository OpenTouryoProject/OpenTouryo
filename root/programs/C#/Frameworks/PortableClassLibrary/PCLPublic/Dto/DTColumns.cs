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
//* クラス名        ：DTColumns
//* クラス日本語名  ：マーシャリング機能付き汎用DTO（列コレクション）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/03/xx  西野  大介        新規作成
//*  2010/11/11  前川  祐介        Silverlight対応（ジェネリック）
//*  2011/10/09  西野  大介        国際化対応
//**********************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;

namespace Touryo.Infrastructure.Public.Dto
{
    /// <summary>列コレクション</summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class DTColumns : IEnumerable
    {
        #region インスタンス変数

        /// <summary>列を保持するList</summary>
        private List<DTColumn> _cols = new List<DTColumn>();

        /// <summary>列名 ⇒ 列インデックスのマップを保持するDictionary</summary>
        private Dictionary<string, int> _colNameIndexMap = new Dictionary<string, int>();

        /// <summary>表情報の共有</summary>
        private DTTableStatus _tblStat;

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        /// <param name="tblStat">表情報</param>
        /// <remarks>外部からは生成できないようにする</remarks>
        internal DTColumns(DTTableStatus tblStat)
        {
            this._tblStat = tblStat;
        }

        #endregion

        #region 列

        #region 設定

        /// <summary>列の追加</summary>
        /// <param name="dtCol">列</param>
        public void Add(DTColumn dtCol)
        {
            // 列の確認
            if (this._tblStat.RowsCount == 0)
            {
                // 列の追加
                this._cols.Add(dtCol);

                // 列名 ⇒ 列インデックスのマップ
                if (this.ColNameIndexMap.ContainsKey(dtCol.ColName))
                {
                    // 列名が重複している。
                    throw new Exception("The column name overlaps. ");
                }
                else
                {
                    this.ColNameIndexMap.Add(dtCol.ColName, this._cols.Count - 1);
                }
            }
            else
            {
                throw new Exception("Since a row already exists, a column cannot be added. ");
            }
        }

        #endregion

        #region 取得（インデクサ）

        /// <summary>列を取得する（確認用）</summary>
        /// <param name="colName">列名</param>
        /// <returns>列</returns>
        public DTColumn this[string colName]
        {
            get
            {
                // 列名で取得
                return (DTColumn)this._cols[((int)this._colNameIndexMap[colName])];
            }
        }

        /// <summary>列を取得する（確認用）</summary>
        /// <param name="index">インデックス</param>
        /// <returns>列</returns>
        public DTColumn this[int index]
        {
            get
            {
                // インデックスで取得
                return (DTColumn)this._cols[index];
            }
        }

        #endregion

        #region サポート情報

        /// <summary>列数の取得</summary>
        public int Count
        {
            get
            {
                return this._cols.Count;
            }
        }

        #endregion

        #region モデル内共有情報

        /// <summary>列を保持するList</summary>
        /// <remarks>変更させないため、外部に公開しない。</remarks>
        internal List<DTColumn> ColsInfo
        {
            get
            {
                return this._cols;
            }
        }

        /// <summary>列名 ⇒ 列インデックスのマップを保持するDictionary</summary>
        /// <remarks>変更させないため、外部に公開しない。</remarks>
        internal Dictionary<string, int> ColNameIndexMap
        {
            get
            {
                return this._colNameIndexMap;
            }
        }

        #endregion

        #endregion

        #region 列挙

        /// <summary>列挙子を取得</summary>
        public IEnumerator GetEnumerator()
        {
            //return new DTColumnsEnumerator(this._cols);
            return this._cols.GetEnumerator();
        }

        #endregion
    }
}

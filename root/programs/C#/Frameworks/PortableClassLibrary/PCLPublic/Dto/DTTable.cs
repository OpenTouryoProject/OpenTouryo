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
//**********************************************************************************

using System;
using System.Collections;
using System.Collections.ObjectModel;
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
    }
}

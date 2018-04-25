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
//* クラス名        ：_3TierParameterValue
//* クラス日本語名  ：三層データバインド用の引数クラス
//*
//* 作成日時        ：－
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2013/01/10  西野 大介         新規作成
//*  2016/04/21  Shashikiran       Defined new dictionary parameter TargetTableNames for adding the multiple table names 
//**********************************************************************************

using System;
using System.Collections.Generic;

using Touryo.Infrastructure.Business.Util;
using Touryo.Infrastructure.Public.Db;

namespace Touryo.Infrastructure.Business.Common
{
    /// <summary>三層データバインド用の引数クラス</summary>
    public class _3TierParameterValue : MyParameterValue
    {
        /// <summary>テーブル名</summary>
        public DbEnum.DBMSType DBMSType;

        /// <summary>汎用エリア</summary>
        public object Obj;

        /// <summary>テーブル名</summary>
        public string TableName;

        /// <summary>カラムリスト（射影）</summary>
        public string ColumnList;

        /// <summary>検索条件　AND, ＝</summary>
        public Dictionary<string, object> AndEqualSearchConditions;

        /// <summary>検索条件　AND, Like</summary>
        public Dictionary<string, string> AndLikeSearchConditions;

        /// <summary>検索条件　OR, ＝</summary>
        public Dictionary<string, object[]> OrEqualSearchConditions;

        /// <summary>検索条件　OR, Like</summary>
        public Dictionary<string, string[]> OrLikeSearchConditions;

        /// <summary>検索条件　その他</summary>
        public Dictionary<string, object> ElseSearchConditions;

        /// <summary>検索条件　その他に対応するWhere句</summary>
        public string ElseWhereSQL;

        /// <summary>ソート列</summary>
        public string SortExpression;
        
        /// <summary>ソート方向</summary>
        public string SortDirection;

        /// <summary>開始行番号（ページング時）</summary>
        public int StartRowIndex;

        /// <summary>最大行番号（ページング時）</summary>
        public int MaximumRows;

        /// <summary>追加・更新値</summary>
        public Dictionary<string, object> InsertUpdateValues;

        /// <summary>Target Table Name</summary>
        public Dictionary<int,string> TargetTableNames;

        /// <summary>データテーブルの型情報</summary>
        /// <remarks>型付きデータテーブルのを指定可能にする</remarks>
        /// <example>typeof(xxxx.xxxDataTable)</example>
        public Type DataTableType;

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        public _3TierParameterValue(string screenId, string controlId, string methodName, string actionType, MyUserInfo user)
            : base(screenId, controlId, methodName, actionType, user)
        {
            // Baseのコンストラクタに引数を渡すために必要。
        }

        #endregion
    }
}

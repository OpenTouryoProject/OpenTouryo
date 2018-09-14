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
//* クラス名        ：CTable
//* クラス日本語名  ：テーブルクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2008/xx/xx  西野 大介         新規作成
//*  2009/12/09  西野 大介         SQL Server用主キー取得機能の追加
//*  2009/12/09  西野 大介         カラム名をキーにしたハッシュテーブルを追加
//**********************************************************************************

using System.Collections;

namespace DaoGen_Tool
{
    /// <summary>テーブルクラス</summary>
    public class CTable
    {
        /// <summary>テーブル名</summary>
        public string Name = "";

        /// <summary>タイプ（テーブル・ビュー）</summary>
        public bool IsView = true;

        /// <summary>有効・無効の制御</summary>
        public bool Effective = true;

        /// <summary>カラム情報（ハッシュはポジション）</summary>
        public Hashtable HtColumns_Position = new Hashtable();

        /// <summary>カラム情報（ハッシュはカラム名）</summary>
        public Hashtable HtColumns_Name = new Hashtable();

        /// <summary>コンストラクタ</summary>
        /// <param name="name">テーブル名</param>
        /// <param name="isView">ビューか、ビューでないか</param>
        public CTable(string name,bool isView)
        {
            this.Name = name;
            this.IsView = isView;
        }
    }
}

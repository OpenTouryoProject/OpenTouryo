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
//* クラス名        ：SubsysInfo
//* クラス日本語名  ：サブシステム情報クラス
//*                   サブシステムIDに対応する
//*                   ハッシュテーブルをインデクサで返す。
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/09/01  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Collections;

namespace Touryo.Infrastructure.Framework.Util
{
    /// <summary>サブシステム情報クラス</summary>
    /// <remarks>自由に利用できる。</remarks>
    [Serializable()]
    public class SubsysInfo
    {
        /// <summary>
        /// ルート ハッシュテーブル
        /// </summary>
        protected Hashtable HT;

        /// <summary>コンストラクタ</summary>
        public SubsysInfo()
        {
            // ルート ハッシュテーブル生成
            this.HT = new Hashtable();
        }

        /// <summary>インデクサ</summary>
        /// <param name="subsysID">サブシステムID</param>
        /// <returns>リーフ ハッシュテーブル</returns>
        public Hashtable this[string subsysID]
        {
            set
            {
                // 基本的にリーフ ハッシュテーブルのnullクリア用
                this.HT[subsysID] = value;
            }
            get
            {
                // リーフ ハッシュテーブルの取得用
                if (this.HT[subsysID] == null)
                {
                    // nullの場合、リーフ ハッシュテーブルを新規生成する。
                    this.HT[subsysID] = new Hashtable();
                }
                else
                {
                    // nullでない場合、何もしない。
                }

                // リーフ ハッシュテーブルを返す。
                return (Hashtable)this.HT[subsysID];
            }
        }
    }
}

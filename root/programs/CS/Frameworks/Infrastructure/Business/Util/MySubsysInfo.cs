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
//* クラス名        ：MySubsysInfo
//* クラス日本語名  ：サブシステム情報クラス（使いやすいインデクサを追加）（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//**********************************************************************************

using System;
using System.Collections;

using Touryo.Infrastructure.Framework.Util;

namespace Touryo.Infrastructure.Business.Util
{
    /// <summary>サブシステム情報クラス（使いやすいインデクサを追加）</summary>
    /// <remarks>自由に（拡張して）利用できる。</remarks>
    [Serializable()]
    public class MySubsysInfo : SubsysInfo
    {
        /// <summary>インデクサ</summary>
        /// <param name="subsysID">サブシステムID列挙型</param>
        /// <returns>リーフ ハッシュテーブル</returns>
        public Hashtable this[SubsysID subsysID]
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

        /// <summary>サブシステムID列挙型</summary>
        public enum SubsysID
        {
            /// <summary>aaa</summary>
            aaa = 1,
            /// <summary>bbb</summary>
            bbb,
            /// <summary>ccc</summary>
            ccc
        }
    }
}

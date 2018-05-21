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
//* クラス名        ：ListItem
//* クラス日本語名  ：リスト用アイテム クラス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//**********************************************************************************

namespace Touryo.Infrastructure.CustomControl
{
    /// <summary>
    /// コンボボックスなどのリスト系コントロールに追加する項目クラス
    /// 識別子（ID）、名称（Name）、表示（ToString）のそれぞれを分けて取得できる。
    /// </summary>
    public class ListItem
    {
        /// <summary>識別子</summary>
        private string _id = "";
        /// <summary>名称</summary>
        private string _name = "";

        /// <summary>コンストラクタ</summary>
        /// <param name="id">識別子</param>
        /// <param name="name">名称</param>
        public ListItem(string id, string name)
        {
            this._id = id;
            this._name = name;
        }

        /// <summary>識別子</summary>
        public string Id
        {
            get
            {
                return this._id;
            }
        }

        /// <summary>名称</summary>
        /// <remarks>
        /// ComboBoxで
        /// ・ .ValueMember = "ID";
        /// ・ .DisplayMember = "Name";
        /// などと設定する。
        /// </remarks>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>表示名</summary>
        /// <remarks>
        /// ComboBoxで
        /// ・ .ValueMember = "ID";
        /// ・ .DisplayMember = "Name2";
        /// などと設定することで表示を変更可能。
        /// </remarks>
        public string Name2
        {
            get
            {
                return this._id + " : " + this._name;
            }
        }

        /// <summary>表示名</summary>
        /// <remarks>
        /// ComboBoxの
        /// ・ .ValueMember = "ID";
        /// ・ .DisplayMember = "Name";
        /// などと設定することで表示を変更可能。
        /// </remarks>
        public override string ToString()
        {
            return this._id + " : " + this._name;
        }
    }
}

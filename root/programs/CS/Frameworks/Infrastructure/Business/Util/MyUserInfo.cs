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
//* クラス名        ：MyUserInfo
//* クラス日本語名  ：ユーザ情報クラス（必要なコンテキスト情報を追加）（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//*  2010/09/24  西野 大介         フィールドの追加
//*  2010/09/24  西野 大介         共通引数クラス内にユーザ情報を格納したので
//**********************************************************************************

using System;
using Touryo.Infrastructure.Framework.Util;

namespace Touryo.Infrastructure.Business.Util
{
    /// <summary>ユーザ情報クラス（必要なコンテキスト情報を追加）</summary>
    /// <remarks>自由に（拡張して）利用できる。</remarks>
    [Serializable()]
    public class MyUserInfo : UserInfo
    {
        /// <summary>ユーザ名</summary>
        private string _userName = "";

        /// <summary>IPアドレス</summary>
        private string _ipAddress;

        /// <summary>コンストラクタ</summary>
        /// <param name="userName">ユーザ名</param>
        /// <param name="ipAddress">IPアドレス</param>
        /// <remarks>自由に利用できる。</remarks>
        public MyUserInfo(string userName, string ipAddress)
        {
            this._userName = userName;
            this._ipAddress = ipAddress;
        }

        /// <summary>ユーザ名</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string UserName
        {
            set
            {
                this._userName = value;
            }
            get
            {
                return this._userName;
            }
        }

        /// <summary>IPアドレス</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string IPAddress
        {
            set
            {
                this._ipAddress = value;
            }
            get
            {
                return this._ipAddress;
            }
        }
    }
}

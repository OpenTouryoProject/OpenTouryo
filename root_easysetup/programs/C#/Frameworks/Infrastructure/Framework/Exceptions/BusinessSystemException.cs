//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//* クラス名        ：BusinessSystemException
//* クラス日本語名  ：システム例外
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2010/03/03  西野  大介        Serializable属性を付与
//*  2013/02/05  西野  大介        innerException対応
//**********************************************************************************

// System
using System;

namespace Touryo.Infrastructure.Framework.Exceptions
{
    /// <summary>システム例外</summary>
    /// <remarks>自由に利用できる。</remarks>
    [Serializable()]
    public class BusinessSystemException : Exception
    {
        /// <summary>メッセージID</summary>
        private string _messageID;

        /// <summary>コンストラクタ</summary>
        /// <param name="messageID">メッセージID</param>
        /// <param name="message">メッセージ</param>
        /// <remarks>
        /// コンストラクタは継承されないので、派生先で呼び出す必要がある。
        /// コンストラクタの実行順は、基本クラス→派生クラスの順
        /// ※ VB.NET では、MyBase.New() を派生クラスのコンストラクタから呼ぶ。
        /// 自由に利用できる。
        /// </remarks>
        public BusinessSystemException(string messageID, string message)
            : base(message)
        {
            //メッセージID
            _messageID = messageID;
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="messageID">メッセージID</param>
        /// <param name="message">メッセージ</param>
        /// <param name="innerException">振替元のException</param>
        /// <remarks>
        /// コンストラクタは継承されないので、派生先で呼び出す必要がある。
        /// コンストラクタの実行順は、基本クラス→派生クラスの順
        /// ※ VB.NET では、MyBase.New() を派生クラスのコンストラクタから呼ぶ。
        /// 自由に利用できる。
        /// </remarks>
        public BusinessSystemException(string messageID, string message, Exception innerException)
            : base(message, innerException)
        {
            //メッセージID
            _messageID = messageID;
        }

        /// <summary>メッセージID</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string messageID
        {
            get
            {
                return this._messageID;
            }
        }
    }
}

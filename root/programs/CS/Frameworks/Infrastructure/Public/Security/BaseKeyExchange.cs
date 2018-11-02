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
//* クラス名        ：BaseKeyExchange
//* クラス日本語名  ：キー交換抽象クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/31  西野 大介         新規作成
//**********************************************************************************

using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>キー交換抽象クラス</summary>
    public abstract class BaseKeyExchange
    {
        /// <summary>AsymmetricAlgorithm</summary>
        protected AsymmetricAlgorithm _asa = null;
        
        /// <summary>相方と交換する交換鍵</summary>
        protected byte[] _exchangeKey;
        /// <summary>相方と交換する交換鍵</summary>
        public byte[] ExchangeKey
        {
            get
            {
                return this._exchangeKey;
            }
        }
    }
}

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
//* クラス名        ：RsaPkcs1Bob
//* クラス日本語名  ：RSA1_5の「Bobクラス」
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/31  西野 大介         新規作成
//**********************************************************************************

using System.Diagnostics;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>RSA1_5の「Bobクラス」</summary>
    public class RsaPkcs1Bob : RsaBob
    {
        /// <summary>秘密鍵生成</summary>
        /// <param name="exchangeKeyOfAlice">Aliceの交換鍵</param>
        /// <param name="iv">初期化ベクター</param>
        public void GeneratePrivateKey(byte[] exchangeKeyOfAlice, byte[] iv)
        {
            this._aes = new AesCryptoServiceProvider();
            RSAPKCS1KeyExchangeDeformatter keyExchangeDeformatter = new RSAPKCS1KeyExchangeDeformatter(this._asa);

            this._aes.Key = keyExchangeDeformatter.DecryptKeyExchange(exchangeKeyOfAlice);
            this._aes.IV = iv;

            //Debug.WriteLine("Bob:Key" + CustomEncode.ToBase64String(this._aes.Key));
            //Debug.WriteLine("Bob:IV" + CustomEncode.ToBase64String(this._aes.IV));
            //Debug.WriteLine("Bob:Padding" + this._aes.Padding.ToString());
        }
    }
}

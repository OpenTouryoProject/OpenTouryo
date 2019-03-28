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
//* クラス名        ：RsaOaepAlice
//* クラス日本語名  ：RSA-OAEPの「Aliceクラス」
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/31  西野 大介         新規作成
//*  2018/11/09  西野 大介         RSAOpenSsl、DSAOpenSsl、HashAlgorithmName対応
//**********************************************************************************

using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Security.KeyExg
{
    /// <summary>RSA-OAEPの「Aliceクラス」</summary>
    public class RsaOaepAlice : RsaAlice
    {
        /// <summary>constructor</summary>
        /// <param name="exchangeKeyOfBob">Bobの交換鍵</param>
        public RsaOaepAlice(byte[] exchangeKeyOfBob) : base(exchangeKeyOfBob) { }

        /// <summary>constructor</summary>
        /// <param name="rsaParams">Bobの交換鍵</param>
        public RsaOaepAlice(RSAParameters rsaParams) : base(rsaParams) { }

        /// <summary>
        /// Bobの交換鍵から、
        /// 「暗号化に使用する秘密鍵」と「Bobと交換するAliceの交換鍵」を生成
        /// </summary>
        protected override void CreateKeys()
        {
            RSA rsa = (RSA)this._asa;
            this._aes = new AesCryptoServiceProvider(); // 秘密鍵
            RSAOAEPKeyExchangeFormatter keyExchangeFormatter = new RSAOAEPKeyExchangeFormatter(rsa);
            this._exchangeKey = keyExchangeFormatter.CreateKeyExchange(this._aes.Key, typeof(Aes)); // 交換鍵
        }
    }
}

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
//* クラス名        ：RsaOaepBob
//* クラス日本語名  ：RSA-OAEPの「Bobクラス」
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
using System.Security.Cryptography.X509Certificates;

namespace Touryo.Infrastructure.Public.Security.KeyExg
{
    /// <summary>RSA-OAEPの「Bobクラス」</summary>
    public class RsaOaepBob : RsaBob
    {
        /// <summary>constructor</summary>
        public RsaOaepBob() : base()
        { }

        /// <summary>constructor</summary>
        /// <param name="rsaPfxFilePath">RSAのX.509証明書(*.pfx)へのパス</param>
        /// <param name="password">パスワード</param>
        public RsaOaepBob(string rsaPfxFilePath, string password) :
            base(rsaPfxFilePath, password, X509KeyStorageFlags.DefaultKeySet) { }

        /// <summary>constructor</summary>
        /// <param name="rsaPfxFilePath">RSAのX.509証明書(*.pfx)へのパス</param>
        /// <param name="password">パスワード</param>
        /// <param name="flag">X509KeyStorageFlags</param>
        public RsaOaepBob(string rsaPfxFilePath, string password, X509KeyStorageFlags flag) :
            base(rsaPfxFilePath, password, flag) { }

        /// <summary>秘密鍵生成</summary>
        /// <param name="exchangeKeyOfAlice">Aliceの交換鍵</param>
        /// <param name="iv">初期化ベクター</param>
        public void GeneratePrivateKey(byte[] exchangeKeyOfAlice, byte[] iv)
        {
            this._aes = Aes.Create();
            RSAOAEPKeyExchangeDeformatter keyExchangeDeformatter = new RSAOAEPKeyExchangeDeformatter(this._asa);

            this._aes.Key = keyExchangeDeformatter.DecryptKeyExchange(exchangeKeyOfAlice);
            this._aes.IV = iv;
        }
    }
}

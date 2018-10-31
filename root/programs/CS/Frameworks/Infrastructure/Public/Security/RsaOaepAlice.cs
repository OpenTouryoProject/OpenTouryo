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
//**********************************************************************************

using System.IO;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>RSA-OAEPの「Aliceクラス」</summary>
    public class RsaOaepAlice : RsaPkcs1KeyExchange
    {
        /// <summary>constructor</summary>
        /// <param name="publicKeyOfBob">Bobの公開鍵</param>
        public RsaOaepAlice(byte[] publicKeyOfBob)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            this._asa = rsa;

            // BobのRSAキー情報
            rsa.ImportCspBlob(publicKeyOfBob);

            // 「暗号化に使用する秘密鍵」と「Bobと鍵交換する公開鍵」を生成
            this._aes = new AesCryptoServiceProvider(); // 秘密鍵
            RSAOAEPKeyExchangeFormatter keyExchangeFormatter = new RSAOAEPKeyExchangeFormatter(rsa);
            this._publicKey = keyExchangeFormatter.CreateKeyExchange(this._aes.Key, typeof(Aes)); // 公開鍵
        }

        /// <summary>暗号化</summary>
        /// <param name="msg">暗号化するメッセージ</param>
        /// <returns>暗号化したメッセージ</returns>
        public string Encrypt(string msg)
        {
            return CustomEncode.ByteToString(
                this.Encrypt(CustomEncode.StringToByte(msg, CustomEncode.UTF_8)),
                CustomEncode.UTF_8);
        }

        /// <summary>暗号化</summary>
        /// <param name="msg">暗号化するメッセージ</param>
        /// <returns>暗号化したメッセージ</returns>
        public byte[] Encrypt(byte[] msg)
        {
            using (MemoryStream ciphertext = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ciphertext, this._aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(msg, 0, msg.Length);
                return ciphertext.ToArray();
            }
        }
    }
}

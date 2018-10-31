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

using System.IO;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>RSA1_5の「Bobクラス」</summary>
    public class RsaPkcs1Bob : RsaPkcs1KeyExchange
    {
        /// <summary>constructor</summary>
        public RsaPkcs1Bob()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            this._asa = rsa;
            this._publicKey = rsa.ExportCspBlob(false);
        }

        /// <summary>秘密鍵生成</summary>
        /// <param name="publicKeyOfAlice">Aliceの公開鍵</param>
        /// <param name="iv">初期化ベクター</param>
        public void GeneratePrivateKey(byte[] publicKeyOfAlice, byte[] iv)
        {
            this._aes = new AesCryptoServiceProvider();
            RSAPKCS1KeyExchangeDeformatter keyExchangeDeformatter = new RSAPKCS1KeyExchangeDeformatter(this._asa);

            this._aes.IV = iv;
            this._aes.Key = keyExchangeDeformatter.DecryptKeyExchange(publicKeyOfAlice);
        }

        /// <summary>復号化</summary>
        /// <param name="msg">復号化するメッセージ</param>
        /// <returns>復号化したメッセージ</returns>
        public string Decrypt(string msg)
        {
            return CustomEncode.ByteToString(
                this.Decrypt(CustomEncode.StringToByte(msg, CustomEncode.UTF_8)),
                CustomEncode.UTF_8);
        }

        /// <summary>暗号化</summary>
        /// <param name="msg">暗号化するメッセージ</param>
        /// <returns>暗号化したメッセージ</returns>
        public byte[] Decrypt(byte[] msg)
        {
            using (MemoryStream ciphertext = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ciphertext, this._aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(msg, 0, msg.Length);
                return ciphertext.ToArray();
            }
        }
    }
}

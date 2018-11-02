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

using System.IO;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Str;

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

        /// <summary>暗号化・復号化に使用する秘密鍵</summary>
        protected Aes _aes = null;

        /// <summary>初期化ベクター</summary>
        public byte[] IV
        {
            get
            {
                return this._aes.IV;
            }
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
            using (MemoryStream plaintext = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(plaintext, this._aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(msg, 0, msg.Length);

                cs.Close(); // これが無いとエラーになる（「パディングは無効なので、削除できません。」）。

                return plaintext.ToArray();
            }
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
                cs.Close();
                return ciphertext.ToArray();
            }
        }
    }
}

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
//* クラス名        ：AeadA128CbcHS256
//* クラス日本語名  ：認証付き暗号（A128CBC-HS256）クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/01  西野 大介         新規作成
//*  2019/02/01  西野 大介         NotImplementedException → 中身を実装。
//**********************************************************************************

using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Security.Aead
{
    /// <summary>
    /// AeadA128CbcHs256
    /// 認証付き暗号（A128CBC-HS256）クラス
    /// A128CBC-HS256 (AES_128_CBC_HMAC_SHA_256
    /// </summary>
    public class AeadA128CbcHS256 : AeadAesCbc
    {
        /// <summary>constructor</summary>
        /// <param name="cek">コンテンツ暗号化キー（CEK）</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="aad">追加認証データ（AAD）</param>
        public AeadA128CbcHS256(byte[] cek, byte[] iv, byte[] aad) : base(cek, iv, aad) { }

        /// <summary>初期化（サイズ）</summary>
        protected override void InitSize()
        {
            // https://tools.ietf.org/html/rfc7518#section-5.2.3
            // - The input key K is 32 octets long.
            // - ENC_KEY_LEN is 16 octets.
            // - MAC_KEY_LEN is 16 octets.
            // - The SHA-256 hash algorithm is used for the HMAC.
            // - The HMAC - SHA - 256 output is truncated to T_LEN = 16 octets, by stripping off the final 16 octets.
            this.CEK_LEN = 32;
            this.ENC_KEY_LEN = CEK_LEN / 2;
            this.MAC_KEY_LEN = CEK_LEN / 2;
            this.TAG_LEN = 16;
            this._hmac = new HMACSHA256();
        }
    }
}

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
//* クラス名        ：AeadA192CbcHS384
//* クラス日本語名  ：認証付き暗号（AES_192_CBC_HMAC_SHA_384）クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/02/04  西野 大介         新規作成
//**********************************************************************************

using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Security.Aead
{
    /// <summary>
    /// AeadA192CbcHS384
    /// 認証付き暗号（AES_192_CBC_HMAC_SHA_384）クラス
    /// </summary>
    public class AeadA192CbcHS384 : AeadAesCbc
    {
        /// <summary>constructor</summary>
        /// <param name="cek">コンテンツ暗号化キー（CEK）</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="aad">追加認証データ（AAD）</param>
        public AeadA192CbcHS384(byte[] cek, byte[] iv, byte[] aad) : base(cek, iv, aad) { }

        /// <summary>初期化（サイズ）</summary>
        protected override void InitSize()
        {
            // https://tools.ietf.org/html/rfc7518#section-5.2.4
            // - The input key K is 48 octets long.
            // - ENC_KEY_LEN is 24 octets.
            // - MAC_KEY_LEN is 24 octets.
            // - The SHA - 384 is used for the HMAC.
            // - The HMAC SHA - 384 value is truncated to T_LEN = 24 octets.
            this.CEK_LEN = 48;
            this.ENC_KEY_LEN = CEK_LEN / 2;
            this.MAC_KEY_LEN = CEK_LEN / 2;
            this.TAG_LEN = 24;
            this._hmac = new HMACSHA384();
        }
    }
}

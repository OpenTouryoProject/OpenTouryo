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
//* クラス名        ：AeadA128CbcHs256
//* クラス日本語名  ：認証付き暗号（A128CBC-HS256）クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/01  西野 大介         新規作成
//**********************************************************************************

// https://github.com/cose-wg/cose-implementations/blob/master/csharp/JOSE/EncryptMessage.cs

using System;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// AeadA128CbcHs256
    /// 認証付き暗号（A128CBC-HS256）クラス
    /// A128CBC-HS256 (AES_128_CBC_HMAC_SHA_256
    /// </summary>
    public class AeadA128CbcHs256 : AuthEncrypt
    {
        /// <summary>constructor</summary>
        /// <param name="cek">コンテンツ暗号化キー（CEK）</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="aad">追加認証データ（AAD）</param>
        public AeadA128CbcHs256(byte[] cek, byte[] iv, byte[] aad) : base(cek, iv, aad)
        {
            throw new NotImplementedException();
        }

        /// <summary>暗号化</summary>
        /// <param name="plaint">平文（plaintext）</param>
        /// <returns>AEAD実行結果オブジェクト</returns>
        public override void Encrypt(byte[] plaint)
        {
            throw new NotImplementedException();
        }

        /// <summary>復号化</summary>
        /// <param name="result">AeadResult</param>
        /// <returns>平文（plaintext）</returns>
        public override byte[] Decrypt(AeadResult result)
        {
            throw new NotImplementedException();
        }
    }
}

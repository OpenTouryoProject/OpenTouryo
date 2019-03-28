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
//* クラス名        ：AeadA128Gcm
//* クラス日本語名  ：認証付き暗号（AES128GCM）クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/02/04  西野 大介         新規作成
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Security.Aead
{

    /// <summary>
    /// AeadA128Gcm
    /// 認証付き暗号（AES128GCM）クラス
    /// </summary>
    public class AeadA128Gcm : AeadAesGcm
    {
        /// <summary>constructor</summary>
        /// <param name="cek">コンテンツ暗号化キー（CEK）</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="aad">追加認証データ（AAD）</param>
        public AeadA128Gcm(byte[] cek, byte[] iv, byte[] aad) : base(cek, iv, aad) { }

        /// <summary>初期化（サイズ）</summary>
        protected override void InitSize()
        {
            // https://tools.ietf.org/html/rfc7518#section-5.3
            // The CEK is used as the encryption key.
            // A128GCM using 128-bit key
            this.CEK_LEN = 16;
            // The requested size of the Authentication Tag output MUST be 128 bits, regardless of the key size.
            this.TAG_LEN = 16;
        }
    }
}

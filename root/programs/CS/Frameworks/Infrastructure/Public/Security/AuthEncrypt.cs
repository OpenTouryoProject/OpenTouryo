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
//* クラス名        ：AuthEncrypt
//* クラス日本語名  ：認証付き暗号（AEAD）抽象クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/01  西野 大介         新規作成
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// 認証付き暗号（AEAD）抽象クラス
    /// </summary>
    public abstract class AuthEncrypt
    {
        /// <summary>処理結果</summary>
        protected AeadResult _result = null;

        /// <summary>
        /// 処理結果（AeadResult）
        /// ・暗号文
        /// ・認証タグ（MAC）
        /// </summary>
        public AeadResult Result
        {
            get
            {
                return this._result;
            }
        }

        /// <summary>constructor</summary>
        /// <param name="cek">コンテンツ暗号化キー（CEK）</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="aad">追加認証データ（AAD）</param>
        public AuthEncrypt(byte[] cek, byte[] iv, byte[] aad) { }

        /// <summary>暗号化</summary>
        /// <param name="plaint">平文（plaintext）</param>
        public abstract void Encrypt(byte[] plaint);

        /// <summary>復号化</summary>
        /// <param name="result">AeadResult</param>
        /// <returns>平文（plaintext）</returns>
        public abstract byte[] Decrypt(AeadResult result);
    }
}
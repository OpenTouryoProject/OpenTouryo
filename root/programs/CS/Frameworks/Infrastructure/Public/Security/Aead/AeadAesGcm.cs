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
//* クラス名        ：AeadAesGcm
//* クラス日本語名  ：認証付き暗号（AESGCM）クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/02/04  西野 大介         新規作成
//**********************************************************************************

using System;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

using Touryo.Infrastructure.Public.Util;

// https://qiita.com/hidelafoglia/items/d12550c8ffe6eca993c8
// https://github.com/cose-wg/cose-implementations/blob/master/csharp/JOSE/EncryptMessage.cs

namespace Touryo.Infrastructure.Public.Security.Aead
{

    /// <summary>
    /// AeadAesGcm
    /// 認証付き暗号（AES-GCM）クラス
    /// </summary>
    public abstract class AeadAesGcm : AuthEncrypt
    {
        /// <summary>
        /// GCMで計算されるTagサイズは 128ビット = 16バイト:
        /// https://tools.ietf.org/html/rfc7518#section-4.7
        ///  The requested size of the Authentication Tag output MUST be 128 bits, regardless of the key size.
        /// </summary>
        public int TAG_LEN { protected set; get; }

        /// <summary>
        /// </summary>
        public int CEK_LEN { protected set; get; }

        /// <summary>コンテンツ暗号化キー（CEK）</summary>
        public byte[] Cek { protected set; get; }

        /// <summary>初期化ベクトル</summary>
        private byte[] _iv = null;

        /// <summary>追加認証データ（AAD）</summary>
        private byte[] _aad = null;

        /// <summary>GcmBlockCipher</summary>
        private GcmBlockCipher _aesGcm = null;

        /// <summary>constructor</summary>
        /// <param name="cek">コンテンツ暗号化キー（CEK）</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="aad">追加認証データ（AAD）</param>
        public AeadAesGcm(byte[] cek, byte[] iv, byte[] aad) : base(cek, iv, aad)
        {
            // 各種サイズの初期化
            this.InitSize();

            if (cek.Length < CEK_LEN)
            {
                throw new ArgumentException("Length is less than 128 bits.", "cek");
            }
            else if (cek.Length == CEK_LEN)
            {
                this.Cek = cek;
            }
            else
            {
                this.Cek = PubCmnFunction.CopyArray(cek, CEK_LEN);
            }

            // Use of an IV of size 96 bits is REQUIRED with this algorithm.
            this._iv = iv;
            // The requested size of the Authentication Tag output MUST be 128 bits, regardless of the key size.
            this._aad = aad;
        }

        /// <summary>初期化（サイズ）</summary>
        protected abstract void InitSize();

        /// <summary>初期化（プロバイダ）</summary>
        /// <param name="forEncrypt">bool</param>
        private void InitAesGcm(bool forEncrypt)
        {
            // Aesをエンジンに指定してGcmBlockCipherを生成
            this._aesGcm = new GcmBlockCipher(new AesEngine());

            // >AesGcm実装を初期化
            this._aesGcm.Init(forEncrypt, new AeadParameters(new KeyParameter(
                this.Cek), 8 * this.TAG_LEN, this._iv, this._aad));
        }

        /// <summary>暗号化</summary>
        /// <param name="plaint">平文（plaintext）</param>
        /// <returns>AEAD実行結果オブジェクト</returns>
        public override void Encrypt(byte[] plaint)
        {
            // AesGcm実装を初期化
            this.InitAesGcm(true);

            // GCM操作の実行
            byte[] aead = new byte[this._aesGcm.GetOutputSize(plaint.Length)];
            int len = this._aesGcm.ProcessBytes(plaint, 0, plaint.Length, aead, 0);
            len += this._aesGcm.DoFinal(aead, len);

            // GetMacで認証タグ（MAC）を取得
            byte[] tag = this._aesGcm.GetMac();

            // 結果を返す
            this._result = new AeadResult()
            {
                Aead = aead, // aead = ciphert + tag
                // <ciphertの抽出>
                // plaint.Lengthと、ciphert.Length - tag.Length を使う方法がある。
                Ciphert = PubCmnFunction.CopyArray<byte>(aead, plaint.Length),
                Tag = tag,
            };
        }

        /// <summary>復号化</summary>
        /// <param name="input">AeadResult</param>
        /// <returns>平文（plaintext）</returns>
        public override byte[] Decrypt(AeadResult input)
        {
            // AesGcm実装を初期化
            this.InitAesGcm(false);

            // aead = ciphert + tag
            byte[] aead = null;
            if (input.Aead == null)
            {
                aead = input.CombineByteArrayForDecrypt();
            }
            else
            {
                aead = input.Aead;
            }

            // GCM操作の実行
            byte[] plaint = new byte[this._aesGcm.GetOutputSize(aead.Length)];
            int len = this._aesGcm.ProcessBytes(aead, 0, aead.Length, plaint, 0);
            len += this._aesGcm.DoFinal(plaint, len);

            // 平文を返す。
            return plaint;
        }
    }
}

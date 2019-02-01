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
//* クラス名        ：AeadA256Gcm
//* クラス日本語名  ：認証付き暗号（A128CBC-HS256）クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/01  西野 大介         新規作成
//**********************************************************************************

using System;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

using Touryo.Infrastructure.Public.Util;

// https://qiita.com/hidelafoglia/items/d12550c8ffe6eca993c8
// https://github.com/cose-wg/cose-implementations/blob/master/csharp/JOSE/EncryptMessage.cs

namespace Touryo.Infrastructure.Public.Security
{

    /// <summary>
    /// AeadA256Gcm
    /// 認証付き暗号（AES256GCM）クラス
    /// A256GCM(AES-GCM)
    /// </summary>
    public class AeadA256Gcm : AuthEncrypt
    {
        /// <summary>
        /// GCMで計算されるTagサイズは 128ビット = 16バイト:
        /// https://tools.ietf.org/html/rfc7518#section-4.7
        ///  The requested size of the Authentication Tag output MUST be 128 bits, regardless of the key size.
        /// </summary>
        public const int TAG_LEN = 16;

        /// <summary>GcmBlockCipher</summary>
        private GcmBlockCipher _aes256Gcm = null;

        /// <summary>コンテンツ暗号化キー（CEK）</summary>
        private byte[] _cek = null;

        /// <summary>初期化ベクトル</summary>
        private byte[] _iv = null;

        /// <summary>追加認証データ（AAD）</summary>
        private byte[] _aad = null;

        /// <summary>constructor</summary>
        /// <param name="cek">コンテンツ暗号化キー（CEK）</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="aad">追加認証データ（AAD）</param>
        public AeadA256Gcm(byte[] cek, byte[] iv, byte[] aad) : base(cek, iv, aad)
        {
            // GcmBlockCipherは、再利用不可らしい。

            // Key length is 128 / 192 / 256 bits.
            // = 16 / 24 / 32 bytes
            if (cek.Length < 16)
            {
                throw new ArgumentException("Length not 128 / 192 / 256 bits.", "cek");
            }
            else if (cek.Length < 24)
            {
                cek = PubCmnFunction.CopyArray(cek, 16);
            }
            else if (cek.Length < 32)
            {
                cek = PubCmnFunction.CopyArray(cek, 24);
            }
            else
            {
                cek = PubCmnFunction.CopyArray(cek, 32);
            }
            this._cek = cek;

            this._iv = iv;
            this._aad = aad;
        }

        /// <summary>AesGcm実装を初期化</summary>
        /// <param name="forEncrypt">bool</param>
        private void InitAesGcm(bool forEncrypt)
        {
            // Aesをエンジンに指定してGcmBlockCipherを生成
            this._aes256Gcm = new GcmBlockCipher(new AesEngine());

            // >AesGcm実装を初期化
            this._aes256Gcm.Init(forEncrypt, new AeadParameters(new KeyParameter(
                this._cek), 8 * AeadA256Gcm.TAG_LEN, this._iv, this._aad));
        }

        /// <summary>暗号化</summary>
        /// <param name="plaint">平文（plaintext）</param>
        /// <returns>AEAD実行結果オブジェクト</returns>
        public override void Encrypt(byte[] plaint)
        {
            // AesGcm実装を初期化
            this.InitAesGcm(true);

            // GCM操作の実行
            byte[] aead = new byte[this._aes256Gcm.GetOutputSize(plaint.Length)];
            int len = this._aes256Gcm.ProcessBytes(plaint, 0, plaint.Length, aead, 0);
            len += this._aes256Gcm.DoFinal(aead, len);

            // GetMacで認証タグ（MAC）を取得
            byte[] tag = this._aes256Gcm.GetMac();

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

            // GCM操作の実行(marged)
            byte[] plaint = new byte[this._aes256Gcm.GetOutputSize(aead.Length)];
            int len = this._aes256Gcm.ProcessBytes(aead, 0, aead.Length, plaint, 0);
            len += this._aes256Gcm.DoFinal(plaint, len);

            // 平文を返す。
            return plaint;
        }
    }
}

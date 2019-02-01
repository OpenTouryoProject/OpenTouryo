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

using System;
using System.Linq;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Paddings;

using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Digests;

using Touryo.Infrastructure.Public.Util;

// https://github.com/cose-wg/cose-implementations/blob/master/csharp/JOSE/EncryptMessage.cs

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// AeadA128CbcHs256
    /// 認証付き暗号（A128CBC-HS256）クラス
    /// A128CBC-HS256 (AES_128_CBC_HMAC_SHA_256
    /// </summary>
    public class AeadA128CbcHS256 : AuthEncrypt
    {
        /// <summary>
        /// AES_128_CBC_HMAC_SHA_256で計算されるTagサイズは 128ビット = 16バイト:
        /// https://tools.ietf.org/html/rfc7518#section-5.2.3
        ///  The HMAC-SHA-256 output is truncated to T_LEN=16 octets, by stripping off the final 16 octets.
        /// </summary>
        public const int TAG_LEN = 16;

        /// <summary>PaddedBufferedBlockCipher </summary>
        private PaddedBufferedBlockCipher _aes128CBC_HS256 = null;

        /// <summary>HMac</summary>
        private HMac _hmac = null;

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
        public AeadA128CbcHS256(byte[] cek, byte[] iv, byte[] aad) : base(cek, iv, aad)
        {
            // PaddedBufferedBlockCipherも、再利用不可？

            // Key length is 256 bits = 32 bytes
            if (cek.Length < 32)
            {
                throw new ArgumentException("Length is less than 256 bits.", "cek");
            }
            else
            {
                cek = PubCmnFunction.CopyArray(cek, 32);
            }
            this._cek = cek;

            // TAG_LEN
            if (iv.Length < TAG_LEN)
            {
                throw new ArgumentException("Length is less than 128 bits.", "iv");
            }
            else
            {
                iv = PubCmnFunction.CopyArray(cek, TAG_LEN);
            }
            this._iv = iv;

            this._aad = aad;
        }

        /// <summary>A128CBC-HS256実装を初期化</summary>
        /// <param name="forEncrypt">bool</param>
        private void Init128CBC_HS256(bool forEncrypt)
        {
            // Aesをエンジンに指定してGcmBlockCipherを生成
            this._aes128CBC_HS256 = new PaddedBufferedBlockCipher(
                new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());

            // 初期化

            // PaddedBufferedBlockCipher
            this._aes128CBC_HS256.Reset();
            this._aes128CBC_HS256.Init(forEncrypt, new ParametersWithIV(
                new KeyParameter(this._cek, TAG_LEN, TAG_LEN), this._iv));

            // HMac
            this._hmac = new HMac(new Sha256Digest());
            this._hmac.Init(new KeyParameter(this._cek, 0, TAG_LEN));
            this._hmac.BlockUpdate(this._aad, 0, this._aad.Length);
            this._hmac.BlockUpdate(this._iv, 0, this._iv.Length);
        }

        /// <summary>InitAL</summary>
        /// <returns>AL</returns>
        private byte[] InitAL()
        {
            //  HMAC 64bit int = cbit(AAD)
            byte[] al = new byte[8];

            int cbAAD = this._aad.Length * 8;
            for (int i = 7; i > 0; i--)
            {
                al[i] = (byte)(cbAAD % 256);
                cbAAD = cbAAD / 256;
                if (cbAAD == 0) break;
            }

            return al;
        }

        /// <summary>暗号化</summary>
        /// <param name="plaint">平文（plaintext）</param>
        /// <returns>AEAD実行結果オブジェクト</returns>
        public override void Encrypt(byte[] plaint)
        {
            // A128CBC-HS256実装を初期化
            this.Init128CBC_HS256(true);

            // CCM操作の実行
            byte[] ciphert = new byte[this._aes128CBC_HS256.GetOutputSize(plaint.Length)];
            int len = this._aes128CBC_HS256.ProcessBytes(plaint, 0, plaint.Length, ciphert, 0);
            len += this._aes128CBC_HS256.DoFinal(ciphert, len);

            // 認証タグ（MAC）を取得
            byte[] tag = new byte[this._hmac.GetMacSize()];

            byte[] al = this.InitAL();
            this._hmac.BlockUpdate(ciphert, 0, ciphert.Length);
            this._hmac.BlockUpdate(al, 0, al.Length);
            this._hmac.DoFinal(tag, 0);

            // 結果を返す
            this._result = new AeadResult()
            {
                Aead = PubCmnFunction.CombineArray(ciphert, tag),
                Ciphert = ciphert,
                Tag = tag,
            };
        }

        /// <summary>復号化</summary>
        /// <param name="input">AeadResult</param>
        /// <returns>平文（plaintext）</returns>
        public override byte[] Decrypt(AeadResult input)
        {
            // A128CBC-HS256実装を初期化
            this.Init128CBC_HS256(false);

            //認証タグ（MAC）を取得
            byte[] tag = new byte[this._hmac.GetMacSize()];

            byte[] al = this.InitAL();
            this._hmac.BlockUpdate(input.Ciphert, 0, input.Ciphert.Length);
            this._hmac.BlockUpdate(al, 0, al.Length);
            this._hmac.DoFinal(tag, 0);

            // タグの確認
            if (!tag.SequenceEqual(input.Tag))
            {
                return null;
            }

            // CCM操作の実行
            byte[] plaint = new byte[this._aes128CBC_HS256.GetOutputSize(input.Ciphert.Length)];
            int len = this._aes128CBC_HS256.ProcessBytes(input.Ciphert, 0, input.Ciphert.Length, plaint, 0);
            len += this._aes128CBC_HS256.DoFinal(plaint, len);

            // 平文を返す。
            return plaint;
        }
    }
}

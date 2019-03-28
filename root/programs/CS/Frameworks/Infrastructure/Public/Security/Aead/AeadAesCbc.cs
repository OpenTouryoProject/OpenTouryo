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
//* クラス名        ：AeadAesCbc
//* クラス日本語名  ：AeadAesCbc認証付き暗号クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/02/04  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Linq;
using System.Security.Cryptography;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

using Touryo.Infrastructure.Public.Util;

// https://github.com/cose-wg/cose-implementations/blob/master/csharp/JOSE/EncryptMessage.cs

namespace Touryo.Infrastructure.Public.Security.Aead
{
    /// <summary>AeadAesCbc認証付き暗号クラス</summary>
    public abstract class AeadAesCbc : AuthEncrypt
    {
        /// <summary>PaddedBufferedBlockCipher </summary>
        protected PaddedBufferedBlockCipher _aesCBC = null;

        /// <summary>HMAC</summary>
        protected HMAC _hmac = null;

        /// <summary>CEK_LEN</summary>
        public int CEK_LEN { protected set; get; }

        /// <summary>ENC_KEY_LEN</summary>
        public int ENC_KEY_LEN { protected set; get; }

        /// <summary>MAC_KEY_LEN</summary>
        public int MAC_KEY_LEN { protected set; get; }

        /// <summary>TAG_LEN</summary>
        public int TAG_LEN { protected set; get; }
        
        /// <summary>MAC_KEY as the SHA-256 key.</summary>
        public byte[] MacKey { protected set; get; }

        /// <summary>ENC_KEY as the AES-CBC key.</summary>
        public byte[] EncKey { protected set; get; }

        /// <summary>初期化ベクトル</summary>
        public byte[] IV { protected set; get; }

        /// <summary>追加認証データ（AAD）</summary>
        public byte[] AAD { protected set; get; }

        /// <summary>AL value</summary>
        public byte[] AL { protected set; get; }

        /// <summary>constructor</summary>
        /// <param name="cek">コンテンツ暗号化キー（CEK）</param>
        /// <param name="iv">初期化ベクトル</param>
        /// <param name="aad">追加認証データ（AAD）</param>
        public AeadAesCbc(byte[] cek, byte[] iv, byte[] aad) : base(cek, iv, aad)
        {
            // 各種サイズの初期化
            this.InitSize();

            // CEK -> MacKey & EncKey
            if (cek.Length < CEK_LEN)
            {
                throw new ArgumentException(string.Format("Length is less than {0} bytes.", this.CEK_LEN), "cek");
            }

            this.MacKey = PubCmnFunction.CopyArray<byte>(cek, MAC_KEY_LEN);
            this.EncKey = PubCmnFunction.CopyArray<byte>(cek, ENC_KEY_LEN, ENC_KEY_LEN, 0);

            // Initialization Vector.
            this.IV = iv;

            // Additional Authenticated Data
            this.AAD = aad;

            // HMac
            this.AL = this.InitAL();
        }

        /// <summary>初期化（サイズ）</summary>
        protected abstract void InitSize();

        /// <summary>初期化（プロバイダ）</summary>
        /// <param name="forEncrypt">bool</param>
        private void InitAesCbc(bool forEncrypt)
        {
            // PaddedBufferedBlockCipher
            this._aesCBC = new PaddedBufferedBlockCipher(
                new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());

            // Initialization Vector.
            // IV must be the same length as block size
            this.IV = PubCmnFunction.CopyArray<byte>(this.IV, this._aesCBC.GetBlockSize());

            // 初期化

            // EncKey length is 128 / 192 / 256 bits.
            this._aesCBC.Reset();
            this._aesCBC.Init(forEncrypt, new ParametersWithIV(
                new KeyParameter(this.EncKey, 0, this.EncKey.Length), this.IV));

            // MacKey
            this._hmac.Initialize();
            this._hmac.Key = this.MacKey;
        }

        /// <summary>InitAL</summary>
        /// <returns>AL value</returns>
        protected byte[] InitAL()
        {
            // https://tools.ietf.org/html/rfc7516#appendix-B.3
            byte[] al = new byte[8];

            int cbAAD = this.AAD.Length * 8;
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
            // 初期化
            this.InitAesCbc(true);

            // CCM操作の実行
            byte[] ciphert = new byte[this._aesCBC.GetOutputSize(plaint.Length)];
            int len = this._aesCBC.ProcessBytes(plaint, 0, plaint.Length, ciphert, 0);
            len += this._aesCBC.DoFinal(ciphert, len);

            // 認証タグ（MAC）を取得
            // Concatenate the [AAD], the [IV], the [ciphertext], and the [AL value].
            byte[] temp = PubCmnFunction.CombineArray(this.AAD, this.IV);
            temp = PubCmnFunction.CombineArray(temp, ciphert);
            temp = PubCmnFunction.CombineArray(temp, this.AL);
            byte[] tag = this._hmac.ComputeHash(temp);
            Array.Resize(ref tag, TAG_LEN);

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
            // 初期化
            this.InitAesCbc(false);

            //認証タグ（MAC）を取得
            // Concatenate the [AAD], the [Initialization Vector], the [ciphertext], and the [AL value].
            byte[] temp = PubCmnFunction.CombineArray(this.AAD, this.IV);
            temp = PubCmnFunction.CombineArray(temp, input.Ciphert);
            temp = PubCmnFunction.CombineArray(temp, this.AL);
            byte[] tag = this._hmac.ComputeHash(temp);
            Array.Resize(ref tag, TAG_LEN);

            // タグの確認
            if (!tag.SequenceEqual(input.Tag))
            {
                return null;
            }

            // CCM操作の実行
            byte[] plaint = new byte[this._aesCBC.GetOutputSize(input.Ciphert.Length)];
            int len = this._aesCBC.ProcessBytes(input.Ciphert, 0, input.Ciphert.Length, plaint, 0);
            len += this._aesCBC.DoFinal(plaint, len);
            Array.Resize(ref plaint, len);

            // 平文を返す。
            return plaint;
        }
    }
}

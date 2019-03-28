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
//* クラス名        ：JWE_Rsa15A128CbcHS256
//* クラス日本語名  ：JWE RSAES-PKCS1-v1_5 and AES_128_CBC_HMAC_SHA_256生成クラス
//*
//*                  RFC 7516 - JSON Web Encryption (JWE)
//*                  > A.2.  Example JWE using RSAES-PKCS1-v1_5 and AES_128_CBC_HMAC_SHA_256
//*                  https://tools.ietf.org/html/rfc7516#appendix-A.2 
//*
//*                  RFC 7518 - JSON Web Algorithms (JWA)
//*                  > 4.2.  Key Encryption with RSAES-PKCS1-v1_5
//*                  https://tools.ietf.org/html/rfc7518#section-4.2
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/02/01  西野 大介         新規作成
//**********************************************************************************

using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Security.Aead;

// https://tools.ietf.org/html/rfc7516#appendix-B
// https://paonejp.github.io/2014/12/21/encrypted_jwt_parsing_trial.html

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>JWE RSAES-PKCS1-v1_5 and AES_128_CBC_HMAC_SHA_256生成クラス</summary>
    public abstract class JWE_Rsa15A128CbcHS256 : JWE
    {
        /// <summary>Constructor</summary>
        public JWE_Rsa15A128CbcHS256()
        {
            this.JWEHeader = new JWE_Header
            {
                alg = JwtConst.RSA1_5,
                enc = JwtConst.A128CBC_HS256
            };

            // Generate a 256-bit random CEK.
            this.CekByteLength = (256 / 8);
            // Generate a random 128-bit JWE Initialization Vector.
            this.IvByteLength = (128 / 8);
        }

        #region 暗号化・復号化

        #region CEK 暗号化・復号化

#if NET45
        /// <summary>CEK 暗号化</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[]</returns>
        protected override byte[] CreateKey(byte[] data)
        {
            // RSAES-PKCS1-v1_5 は、fOAEP: false
            return this.ASymmetricCryptography.EncryptBytes(data, fOAEP: false);
        }

        /// <summary>CEK 復号化</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[] </returns>
        protected override byte[] DecryptKey(byte[] data)
        {
            return this.ASymmetricCryptography.DecryptBytes(data, fOAEP: false);
        }
#else
        /// <summary>CEK 暗号化</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[]</returns>
        protected override byte[] CreateKey(byte[] data)
        {
            // RSAES-PKCS1-v1_5 は、padding: RSAEncryptionPadding.Pkcs1
            return this.ASymmetricCryptography.EncryptBytes(
                data, padding: RSAEncryptionPadding.Pkcs1);
        }

        /// <summary>CEK 復号化</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[] </returns>
        protected override byte[] DecryptKey(byte[] data)
        {
            return this.ASymmetricCryptography.DecryptBytes(
                data, padding: RSAEncryptionPadding.Pkcs1);
        }
#endif
        #endregion

        #region 本文 暗号化・復号化

        /// <summary>認証付き暗号（AEAD）による本文 暗号化</summary>
        /// <param name="cekBytes">コンテンツ暗号化キー（CEK）</param>
        /// <param name="ivBytes">初期化ベクトル</param>
        /// <param name="aadBytes">追加認証データ（AAD）</param>
        /// <param name="payloadBytes">ペイロード</param>
        /// <returns>AeadResult</returns>
        protected override AeadResult CreateBody(byte[] cekBytes, byte[] ivBytes, byte[] aadBytes, byte[] payloadBytes)
        {
            AeadA128CbcHS256 aesA128CbcHs256 = new AeadA128CbcHS256(cekBytes, ivBytes, aadBytes);
            aesA128CbcHs256.Encrypt(payloadBytes);
            return aesA128CbcHs256.Result;
        }

        /// <summary>認証付き暗号（AEAD）による本文 復号化</summary>
        /// <param name="cekBytes">コンテンツ暗号化キー（CEK）</param>
        /// <param name="ivBytes">初期化ベクトル</param>
        /// <param name="aadBytes">追加認証データ（AAD）</param>
        /// <param name="aeadResult">AeadResult</param>
        /// <returns>byte[] </returns>
        protected override byte[] DecryptBody(byte[] cekBytes, byte[] ivBytes, byte[] aadBytes, AeadResult aeadResult)
        {
            AeadA128CbcHS256 aesA128CbcHs256 = new AeadA128CbcHS256(cekBytes, ivBytes, aadBytes);
            return aesA128CbcHs256.Decrypt(aeadResult);
        }

        #endregion

        #endregion
    }
}

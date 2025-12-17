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
//* クラス名        ：JWE_RsaOaepAesGcm
//* クラス日本語名  ：JWE RSAES-OAEP and AES GCM生成クラス
//*
//*                  RFC 7516 - JSON Web Encryption (JWE)
//*                  > A.1.  Example JWE using RSAES-OAEP and AES GCM
//*                  https://tools.ietf.org/html/rfc7516#appendix-A.1
//*
//*                  RFC 7518 - JSON Web Algorithms (JWA)
//*                  > 4.3.  Key Encryption with RSAES OAEP
//*                  https://tools.ietf.org/html/rfc7518#section-4.3
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/01/29  西野 大介         新規作成
//**********************************************************************************

using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Security.Aead;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>JWE RSAES-OAEP and AES GCM生成クラス</summary>
    public abstract class JWE_RsaOaepAesGcm : JWE
    {
        /// <summary>Constructor</summary>
        public JWE_RsaOaepAesGcm()
        {
            this.JWEHeader = new JWE_Header
            {
                alg = JwtConst.RSA_OAEP,
                enc = JwtConst.A256GCM
            };

            // Generate a 256-bit random CEK.
            this.CekByteLength = (256 / 8);
            // Generate a random 96-bit JWE Initialization Vector.
            this.IvByteLength = (96 / 8);
        }

        #region 暗号化・復号化

        #region CEK 暗号化・復号化

        /// <summary>CEK 暗号化</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[]</returns>
        protected override byte[] CreateKey(byte[] data)
        {
            // RSA-OAEP = RSAES OAEP using default parameters は、
            // SHA-1ハッシュ関数とSHA-1マスク生成機能付きMGF1
            return this.ASymmetricCryptography.EncryptBytes(
                data, padding: RSAEncryptionPadding.OaepSHA1);
        }

        /// <summary>CEK 復号化</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[] </returns>
        protected override byte[] DecryptKey(byte[] data)
        {
            return this.ASymmetricCryptography.DecryptBytes(
                data, padding: RSAEncryptionPadding.OaepSHA1);
        }

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
            AeadA256Gcm aesGcm = new AeadA256Gcm(cekBytes, ivBytes, aadBytes);
            aesGcm.Encrypt(payloadBytes);
            return aesGcm.Result;
        }

        /// <summary>認証付き暗号（AEAD）による本文 復号化</summary>
        /// <param name="cekBytes">コンテンツ暗号化キー（CEK）</param>
        /// <param name="ivBytes">初期化ベクトル</param>
        /// <param name="aadBytes">追加認証データ（AAD）</param>
        /// <param name="aeadResult">AeadResult</param>
        /// <returns>byte[] </returns>
        protected override byte[] DecryptBody(byte[] cekBytes, byte[] ivBytes, byte[] aadBytes, AeadResult aeadResult)
        {
            AeadA256Gcm aesGcm = new AeadA256Gcm(cekBytes, ivBytes, aadBytes);
            return aesGcm.Decrypt(aeadResult);
        }

        #endregion

        #endregion
    }
}

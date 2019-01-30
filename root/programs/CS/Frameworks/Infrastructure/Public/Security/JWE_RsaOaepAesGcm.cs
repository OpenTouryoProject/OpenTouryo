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
//*                  https://tools.ietf.org/html/rfc7516#appendix-A.1.1 
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

using Newtonsoft.Json;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>JWE RSAES-OAEP and AES GCM生成クラス</summary>
    public abstract class JWE_RsaOaepAesGcm : JWE
    {
        #region mem & prop & constructor

        /// <summary>_JWEHeader</summary>
        private JWE_Header _JWEHeader = new JWE_Header
        {
            alg = JwtConst.RSA_OAEP,
            enc = JwtConst.A256GCM
        };

        /// <summary>JWEHeader</summary>
        public JWE_Header JWEHeader
        {
            protected set
            {
                this._JWEHeader = value;
            }

            get
            {
                return this._JWEHeader;
            }
        }

        /// <summary>ASymmetricCryptography</summary>
        public ASymmetricCryptography ASymmetricCryptography { get; protected set; }

        #endregion

        #region JWE RSAES-OAEP and AES GCM 暗号化・復号化

        /// <summary>RSAES-OAEP and AES GCMのJWE生成メソッド</summary>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>JWEの文字列表現</returns>
        public override string Create(string payloadJson)
        {
            // ヘッダー
            string headerJson = JsonConvert.SerializeObject(
                this.JWEHeader,
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore
                });

            byte[] headerBytes = CustomEncode.StringToByte(headerJson, CustomEncode.UTF_8);
            string headerEncoded = CustomEncode.ToBase64UrlString(headerBytes);

            // コンテンツ暗号化キー（CEK）
            byte[] cekBytes = GetPassword.RandomByte(256 / 8); // Generate a 256-bit random CEK.
            byte[] encryptedCekBytes = this.CreateKey(cekBytes); // 派生を呼ぶ
            string encryptedCekEncoded = CustomEncode.ToBase64UrlString(encryptedCekBytes);

            // 初期化ベクトル
            byte[] ivBytes = GetPassword.RandomByte(96 / 8); // Generate a random 96-bit JWE Initialization Vector.
            string ivEncoded = CustomEncode.ToBase64UrlString(ivBytes);

            // 追加認証データ（AAD）
            byte[] aadBytes = CustomEncode.StringToByte(headerEncoded, CustomEncode.us_ascii);

            // ペイロード（認証付き暗号（AEAD）による暗号化）
            byte[] payloadBytes = CustomEncode.StringToByte(payloadJson, CustomEncode.UTF_8);
            AeadA256Gcm aesGcm = new AeadA256Gcm(cekBytes, ivBytes, aadBytes);
            aesGcm.Encrypt(payloadBytes);
            byte[] encryptedPayloadBytes = aesGcm.Result.Ciphert;
            string encryptedPayloadEncoded = CustomEncode.ToBase64UrlString(encryptedPayloadBytes);

            // 認証タグ（MAC）
            byte[] macBytes = aesGcm.Result.Tag;
            string macEncoded = CustomEncode.ToBase64UrlString(macBytes);

            // return JWE by RSAES-OAEP and AES GCM
            return headerEncoded + "." + 
                encryptedCekEncoded + "." + ivEncoded + "." + 
                encryptedPayloadEncoded + "." + macEncoded;
        }

        /// <summary>RSAES-OAEP and AES GCMのJWE復号メソッド</summary>
        /// <param name="jwtString">JWEの文字列表現</param>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>復号の結果</returns>
        public override bool Decrypt(string jwtString, out string payloadJson)
        {
            try
            {
                string[] temp = jwtString.Split('.');

                // 検証
                JWE_Header headerObject = (JWE_Header)JsonConvert.DeserializeObject(
                    CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8), typeof(JWE_Header));

                if (headerObject.alg.ToUpper() == JwtConst.RSA_OAEP &&
                    headerObject.enc.ToUpper() == JwtConst.A256GCM &&
                    headerObject.typ.ToUpper() == JwtConst.JWT)
                {
                    // コンテンツ暗号化キー（CEK）
                    byte[] encryptedCekBytes = CustomEncode.FromBase64UrlString(temp[1]);
                    byte[] cekBytes = this.DecryptKey(encryptedCekBytes); // 派生を呼ぶ

                    // 初期化ベクトル
                    byte[] ivBytes = CustomEncode.FromBase64UrlString(temp[2]);

                    // 追加認証データ（AAD）
                    byte[] aadBytes = CustomEncode.StringToByte(temp[0], CustomEncode.us_ascii);

                    // ペイロード（認証付き暗号（AEAD）による暗号化）
                    byte[] encryptedPayloadBytes = CustomEncode.FromBase64UrlString(temp[3]);

                    // 認証タグ（MAC）
                    byte[] macBytes = CustomEncode.FromBase64UrlString(temp[4]);

                    // 復号化
                    AeadA256Gcm aesGcm = new AeadA256Gcm(cekBytes, ivBytes, aadBytes);
                    byte[] payloadBytes = aesGcm.Decrypt(new AeadResult()
                    {
                        Ciphert = encryptedPayloadBytes,
                        Tag = macBytes
                    });

                    payloadJson = CustomEncode.ByteToString(payloadBytes, CustomEncode.UTF_8);
                    return true;
                }
                else
                {
                    payloadJson = "";
                    return false;
                }
            }
            catch
            {
                payloadJson = "";
                return false;
            }
        }

        #region CEK 暗号化・復号化

#if NET45
        /// <summary>CreateKey</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[]</returns>
        protected virtual byte[] CreateKey(byte[] data)
        {
            // RSA-OAEP = RSAES OAEP using default parameters は、
            // SHA-1ハッシュ関数とSHA-1マスク生成機能付きMGF1
            return this.ASymmetricCryptography.EncryptBytes(data, fOAEP: true);
        }

        /// <summary>DecryptKey</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[] </returns>
        protected virtual byte[] DecryptKey(byte[] data)
        {
            return this.ASymmetricCryptography.DecryptBytes(data, fOAEP: true);
        }
#else
        /// <summary>CreateKey</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[]</returns>
        protected virtual byte[] CreateKey(byte[] data)
        {
            // RSA-OAEP = RSAES OAEP using default parameters は、
            // SHA-1ハッシュ関数とSHA-1マスク生成機能付きMGF1
            return this.ASymmetricCryptography.EncryptBytes(
                data, padding: RSAEncryptionPadding.OaepSHA1);
        }

        /// <summary>DecryptKey</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[] </returns>
        protected virtual byte[] DecryptKey(byte[] data)
        {
            return this.ASymmetricCryptography.DecryptBytes(
                data, padding: RSAEncryptionPadding.OaepSHA1);
        }
#endif
        #endregion

        #endregion
    }
}

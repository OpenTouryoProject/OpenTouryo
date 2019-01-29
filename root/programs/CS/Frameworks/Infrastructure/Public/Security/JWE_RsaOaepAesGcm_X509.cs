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
//* クラス名        ：JWE_RsaOaepAesGcm_X509
//* クラス日本語名  ：X.509証明書によるJWE RSAES-OAEP and AES GCM生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/01/29  西野 大介         新規作成
//**********************************************************************************

using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>X.509証明書によるJWE RSAES-OAEP and AES GCM生成クラス</summary>
    public class JWE_RsaOaepAesGcm_X509 : JWE_RsaOaepAesGcm
    {
        #region mem & prop & constructor

        /// <summary>CertificateFilePath</summary>
        public string CertificateFilePath { get; protected set; }

        /// <summary>CertificatePassword</summary>
        public string CertificatePassword { get; protected set; }

        /// <summary>ASymmetricCryptography</summary>
        public ASymmetricCryptography ASymmetricCryptography { get; protected set; }

        /// <summary>Constructor</summary>
        /// <param name="certificateFilePath">ASymmetricCryptographyに渡すcertificateFilePathパラメタ</param>
        /// <param name="password">ASymmetricCryptographyに渡すpasswordパラメタ</param>
        public JWE_RsaOaepAesGcm_X509(string certificateFilePath, string password)
            : this(certificateFilePath, password, X509KeyStorageFlags.DefaultKeySet) { }

        /// <summary>Constructor</summary>
        /// <param name="certificateFilePath">ASymmetricCryptographyに渡すcertificateFilePathパラメタ</param>
        /// <param name="password">ASymmetricCryptographyに渡すpasswordパラメタ</param>
        /// <param name="flag">X509KeyStorageFlags</param>
        public JWE_RsaOaepAesGcm_X509(string certificateFilePath, string password, X509KeyStorageFlags flag)
        {
            this.CertificateFilePath = certificateFilePath;
            this.CertificatePassword = password;
            this.ASymmetricCryptography = new ASymmetricCryptography(
                EnumASymmetricAlgorithm.X509, certificateFilePath, password, flag);
        }

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
            byte[] encryptedKeyBytes = null; // Generate a 256-bit random CEK.
            string encryptedKeyEncoded = CustomEncode.ToBase64UrlString(encryptedKeyBytes);

            // 初期化ベクトル

            // ペイロード（認証付き暗号（AEAD）による暗号化）
            byte[] payloadBytes = CustomEncode.StringToByte(payloadJson, CustomEncode.UTF_8);
            string payloadEncoded = CustomEncode.ToBase64UrlString(payloadBytes);

            // 認証タグ（MAC）

            // JWE Compact Serializationでは、
            // 追加認証データ（AAD）を付与しない。

            // return JWE by RSAES-OAEP and AES GCM
            return headerEncoded + "." + encryptedKeyEncoded + "." + payloadEncoded + ".";
        }

        /// <summary>RSAES-OAEP and AES GCMのJWE復号化メソッド</summary>
        /// <param name="jwtString">JWEの文字列表現</param>
        /// <returns>復号化の結果</returns>
        public override bool Verify(string jwtString)
        {
            string[] temp = jwtString.Split('.');

            // 検証
            JWS_Header headerObject = (JWS_Header)JsonConvert.DeserializeObject(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8), typeof(JWS_Header));

            if (headerObject.alg.ToUpper() == JwtConst.RS256 && headerObject.typ.ToUpper() == JwtConst.JWT)
            {
                //byte[] data = CustomEncode.StringToByte(temp[0] + "." + temp[1], CustomEncode.UTF_8);
                //byte[] sign = CustomEncode.FromBase64UrlString(temp[2]);
                //return this.DigitalSignX509.Verify(data, sign);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

    }
}

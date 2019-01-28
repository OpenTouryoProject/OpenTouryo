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
//* クラス名        ：JWS_ES256_X509
//* クラス日本語名  ：X.509証明書によるJWS ES256生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/12/25  西野 大介         新規作成
//**********************************************************************************

using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>X.509証明書によるJWS ES256生成クラス</summary>
    public class JWS_ES256_X509 : JWS_ES256
    {
        #region mem & prop & constructor

        /// <summary>CertificateFilePath</summary>
        public string CertificateFilePath { get; protected set; }

        /// <summary>CertificatePassword</summary>
        public string CertificatePassword { get; protected set; }

        /// <summary>DigitalSignX509</summary>
        public DigitalSignX509 DigitalSignX509 { get; protected set; }

        /// <summary>Constructor</summary>
        /// <param name="certificateFilePath">DigitalSignX509に渡すcertificateFilePathパラメタ</param>
        /// <param name="password">DigitalSignX509に渡すpasswordパラメタ</param>
        public JWS_ES256_X509(string certificateFilePath, string password)
            : this(certificateFilePath, password, X509KeyStorageFlags.DefaultKeySet) { }

        /// <summary>Constructor</summary>
        /// <param name="certificateFilePath">DigitalSignX509に渡すcertificateFilePathパラメタ</param>
        /// <param name="password">DigitalSignX509に渡すpasswordパラメタ</param>
        /// <param name="flag">X509KeyStorageFlags</param>
        public JWS_ES256_X509(string certificateFilePath, string password, X509KeyStorageFlags flag)
        {
            this.CertificateFilePath = certificateFilePath;
            this.CertificatePassword = password;
            this.DigitalSignX509 = new DigitalSignX509(certificateFilePath, password, HashNameConst.SHA256, flag);
        }

        #endregion

        #region RS256署名・検証

        /// <summary>RS256のJWS生成メソッド</summary>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>JWSの文字列表現</returns>
        public override string Create(string payloadJson)
        {
            // ヘッダー
            string headerJson = JsonConvert.SerializeObject(
                this.JWSHeader,
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore
                });

            byte[] headerBytes = CustomEncode.StringToByte(headerJson, CustomEncode.UTF_8);
            string headerEncoded = CustomEncode.ToBase64UrlString(headerBytes);

            // ペイロード
            byte[] payloadBytes = CustomEncode.StringToByte(payloadJson, CustomEncode.UTF_8);
            string payloadEncoded = CustomEncode.ToBase64UrlString(payloadBytes);

            // 署名
            byte[] temp = CustomEncode.StringToByte(headerEncoded + "." + payloadEncoded, CustomEncode.UTF_8);
            string signEncoded = CustomEncode.ToBase64UrlString(this.DigitalSignX509.Sign(temp));

            // return JWS by RS256
            return headerEncoded + "." + payloadEncoded + "." + signEncoded;
        }

        /// <summary>RS256のJWS検証メソッド</summary>
        /// <param name="jwtString">JWSの文字列表現</param>
        /// <returns>署名の検証結果</returns>
        public override bool Verify(string jwtString)
        {
            string[] temp = jwtString.Split('.');

            // 検証
            JWS_Header headerObject = (JWS_Header)JsonConvert.DeserializeObject(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8), typeof(JWS_Header));

            if (headerObject.alg.ToUpper() == JwtConst.RS256 && headerObject.typ.ToUpper() == JwtConst.JWT)
            {
                byte[] data = CustomEncode.StringToByte(temp[0] + "." + temp[1], CustomEncode.UTF_8);
                byte[] sign = CustomEncode.FromBase64UrlString(temp[2]);
                return this.DigitalSignX509.Verify(data, sign);
            }
            else
            {
                return false;
            }
        }

        #endregion

    }
}

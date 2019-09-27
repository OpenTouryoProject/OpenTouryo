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
//* クラス名        ：JWS_ES512_X509
//* クラス日本語名  ：X.509証明書によるJWS ES512生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/25  西野 大介         新規作成（分割
//**********************************************************************************

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>X.509証明書によるJWS ES512生成クラス</summary>
    public class JWS_ES512_X509 : JWS_ES512
    {
        #region mem & prop & constructor

        /// <summary>CertificateFilePath</summary>
        public string CertificateFilePath { get; protected set; }

        /// <summary>CertificatePassword</summary>
        public string CertificatePassword { get; protected set; }

        /// <summary>DigitalSignECDsaX509</summary>
        public DigitalSignECDsaX509 DigitalSignECDsaX509 { get; protected set; }

        /// <summary>Constructor</summary>
        /// <param name="certificateFilePath">DigitalSignECDsaX509に渡すcertificateFilePathパラメタ</param>
        /// <param name="password">DigitalSignECDsaX509に渡すpasswordパラメタ</param>
        public JWS_ES512_X509(string certificateFilePath, string password)
        {
            this.CertificateFilePath = certificateFilePath;
            this.CertificatePassword = password;
            this.DigitalSignECDsaX509 = new DigitalSignECDsaX509(
                certificateFilePath, password, HashAlgorithmName.SHA512);
            // X509KeyStorageFlagsは、DigitalSignECDsaX509の既定値を使用
        }

        /// <summary>Constructor</summary>
        /// <param name="certificateFilePath">DigitalSignECDsaX509に渡すcertificateFilePathパラメタ</param>
        /// <param name="password">DigitalSignECDsaX509に渡すpasswordパラメタ</param>
        /// <param name="flag">X509KeyStorageFlags</param>
        public JWS_ES512_X509(string certificateFilePath, string password, X509KeyStorageFlags flag)
        {
            this.CertificateFilePath = certificateFilePath;
            this.CertificatePassword = password;
            this.DigitalSignECDsaX509 = new DigitalSignECDsaX509(
                certificateFilePath, password, HashAlgorithmName.SHA512, flag);
        }

        #endregion

        #region ES512署名・検証

        /// <summary>Create2</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[]</returns>
        protected override byte[] Create2(byte[] data)
        {
            return this.DigitalSignECDsaX509.Sign(data);
        }

        /// <summary>Verify2</summary>
        /// <param name="data">byte[]</param>
        /// <param name="sign">byte[]</param>
        /// <returns>bool</returns>
        protected override bool Verify2(byte[] data, byte[] sign)
        {
            return this.DigitalSignECDsaX509.Verify(data, sign);
        }

        #endregion
    }
}

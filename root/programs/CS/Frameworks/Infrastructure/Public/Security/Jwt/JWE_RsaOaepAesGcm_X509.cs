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
//*  2019/06/12  西野 大介         constructorのX509KeyStorageFlagsの既定値を変更
//**********************************************************************************

using System.Security.Cryptography.X509Certificates;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>X.509証明書によるJWE RSAES-OAEP and AES GCM生成クラス</summary>
    public class JWE_RsaOaepAesGcm_X509 : JWE_RsaOaepAesGcm
    {
        #region mem & prop & constructor

        /// <summary>CertificateFilePath</summary>
        public string CertificateFilePath { get; protected set; }

        /// <summary>CertificatePassword</summary>
        public string CertificatePassword { get; protected set; }

        /// <summary>Constructor</summary>
        /// <param name="certificateFilePath">ASymmetricCryptographyに渡すcertificateFilePathパラメタ</param>
        /// <param name="password">ASymmetricCryptographyに渡すpasswordパラメタ</param>
        /// <param name="flag">X509KeyStorageFlags</param>
        public JWE_RsaOaepAesGcm_X509(string certificateFilePath, string password,
            X509KeyStorageFlags flag = X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet)
        {
            this.CertificateFilePath = certificateFilePath;
            this.CertificatePassword = password;
            this.ASymmetricCryptography = new ASymmetricCryptography(
                EnumASymmetricAlgorithm.X509, certificateFilePath, password, flag);
        }

        #endregion
    }
}

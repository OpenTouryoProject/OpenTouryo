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
//* クラス名        ：PrivacyEnhancedMail
//* クラス日本語名  ：PrivacyEnhancedMail(PEM)クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/06  西野 大介         新規作成
//**********************************************************************************

using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>PrivacyEnhancedMail(PEM)クラス</summary>
    public class PrivacyEnhancedMail
    {
        #region const

        private const string CERTIFICATE = "CERTIFICATE";
        private const string X509_CERTIFICATE = "X509 CERTIFICATE";
        private const string X509_CERTIFICATE2 = "X.509 CERTIFICATE";
        private const string TRUSTED_CERTIFICATE = "TRUSTED CERTIFICATE";
        private const string NETSCAPE_CERTIFICATE_SEQUENCE = "NETSCAPE CERTIFICATE SEQUENCE";
        private const string ATTRIBUTE_CERTIFICATE = "ATTRIBUTE CERTIFICATE";

        private const string PKCS7 = "PKCS7";
        private const string CERTIFICATE_CHAIN = "CERTIFICATE CHAIN";
        private const string PKCS7_CERTIFICATE_CHAIN = "PKCS7 CERTIFICATE CHAIN";

        private const string CMS = "CMS";

        private const string PRIVATE_KEY = "PRIVATE KEY";
        private const string PUBLIC_KEY = "PUBLIC KEY";
        private const string RSA_PRIVATE_KEY = "RSA PRIVATE KEY";
        private const string RSA_PUBLIC_KEY = "RSA PUBLIC KEY";
        private const string DSA_PRIVATE_KEY = "DSA PRIVATE KEY";
        private const string DSA_PUBLIC_KEY = "DSA PUBLIC KEY";
        private const string ENCRYPTED_PRIVATE_KEY = "ENCRYPTED PRIVATE KEY";

        private const string CERTIFICATE_REQUEST = "CERTIFICATE REQUEST";
        private const string NEW_CERTIFICATE_REQUEST = "NEW CERTIFICATE REQUEST";

        private const string CRL = "CRL";
        private const string X509_CRL = "X509 CRL";
        
        private const string EC_PARAMETERS = "EC PARAMETERS";
        private const string DH_PARAMETERS = "DH PARAMETERS";
        #endregion

        #region enum
        /// <summary>RFC7468 Label</summary>
        public enum RFC7468Label : int
        {
            /// <summary>
            /// 証明書, PKCS #7証明書鎖,
            /// Netscape Certificate Sequence
            /// </summary>
            Certificate,
            /// <summary>X.509証明書</summary>
            X509Certificate,
            /// <summary>X.509証明書</summary>
            X509Certificate2,
            /// <summary>証明書</summary>
            TrustedCertificate,
            /// <summary>Netscape Certificate Sequence</summary>
            NetscapeCertificateSequence,
            /// <summary>属性証明書</summary>
            AttributeCertificate,

            /// <summary>PKCS7</summary>
            PKCS7,
            /// <summary>PKCS #7証明書鎖</summary>
            CertificateChain,
            /// <summary>PKCS #7証明書鎖</summary>
            PKCS7CertificateChain,
            
            /// <summary>CMS</summary>
            CMS,

            /// <summary> 秘密鍵 (PKCS #8)</summary>
            PrivateKey,
            /// <summary> 公開鍵</summary>
            PublicKey,
            /// <summary> RSA 秘密鍵 (PKCS #1)</summary>
            RsaPrivateKey,
            /// <summary> RSA 公開鍵 (PKCS #1)</summary>
            RsaPublicKey,
            /// <summary> DSA 秘密鍵 (PKCS #1)</summary>
            DsaPrivateKey,
            /// <summary> DSA 公開鍵 (PKCS #1)</summary>
            DsaPublicKey,
            /// <summary>暗号化された秘密鍵 (PKCS #8) </summary>
            EncryptedPrivateKey,
            
            /// <summary>CSR (PKCS #10)</summary>
            CertificateRequest,
            /// <summary>CSR (PKCS #10)</summary>
            NewCertificateRequest,

            /// <summary>CRL</summary>
            CRL,
            /// <summary>CRL</summary>
            X509CRL,
            
            /// <summary>楕円曲線のパラメタ</summary>
            EcParameters,
            /// <summary>Diffie-Hellmanのパラメタ</summary>
            DhParameters
        }

        /// <summary>EnumToString</summary>
        public static string EnumToString(RFC7468Label label)
        {
            string _label = "";

            switch (label)
            {
                // Certificate
                case RFC7468Label.Certificate:
                    _label = PrivacyEnhancedMail.CERTIFICATE;
                    break;
                case RFC7468Label.X509Certificate:
                    _label = PrivacyEnhancedMail.X509_CERTIFICATE;
                    break;
                case RFC7468Label.X509Certificate2:
                    _label = PrivacyEnhancedMail.X509_CERTIFICATE2;
                    break;
                case RFC7468Label.TrustedCertificate:
                    _label = PrivacyEnhancedMail.TRUSTED_CERTIFICATE;
                    break;
                case RFC7468Label.NetscapeCertificateSequence:
                    _label = PrivacyEnhancedMail.NETSCAPE_CERTIFICATE_SEQUENCE;
                    break;
                case RFC7468Label.AttributeCertificate:
                    _label = PrivacyEnhancedMail.ATTRIBUTE_CERTIFICATE;
                    break;

                // PKCS7
                case RFC7468Label.PKCS7:
                    _label = PrivacyEnhancedMail.PKCS7;
                    break;
                case RFC7468Label.CertificateChain:
                    _label = PrivacyEnhancedMail.CERTIFICATE_CHAIN;
                    break;
                case RFC7468Label.PKCS7CertificateChain:
                    _label = PrivacyEnhancedMail.PKCS7_CERTIFICATE_CHAIN;
                    break;

                case RFC7468Label.CMS:
                    _label = PrivacyEnhancedMail.CMS;
                    break;

                // Key
                case RFC7468Label.PrivateKey:
                    _label = PrivacyEnhancedMail.PRIVATE_KEY;
                    break;
                case RFC7468Label.PublicKey:
                    _label = PrivacyEnhancedMail.PUBLIC_KEY;
                    break;
                case RFC7468Label.RsaPrivateKey:
                    _label = PrivacyEnhancedMail.RSA_PRIVATE_KEY;
                    break;
                case RFC7468Label.RsaPublicKey:
                    _label = PrivacyEnhancedMail.RSA_PUBLIC_KEY;
                    break;
                case RFC7468Label.DsaPrivateKey:
                    _label = PrivacyEnhancedMail.DSA_PRIVATE_KEY;
                    break;
                case RFC7468Label.DsaPublicKey:
                    _label = PrivacyEnhancedMail.DSA_PUBLIC_KEY;
                    break;
                case RFC7468Label.EncryptedPrivateKey:
                    _label = PrivacyEnhancedMail.ENCRYPTED_PRIVATE_KEY;
                    break;

                // CertificateRequest
                case RFC7468Label.CertificateRequest:
                    _label = PrivacyEnhancedMail.CERTIFICATE_REQUEST;
                    break;
                case RFC7468Label.NewCertificateRequest:
                    _label = PrivacyEnhancedMail.NEW_CERTIFICATE_REQUEST;
                    break;

                // CRL
                case RFC7468Label.CRL:
                    _label = PrivacyEnhancedMail.CRL;
                    break;
                case RFC7468Label.X509CRL:
                    _label = PrivacyEnhancedMail.X509_CRL;
                    break;

                // Parameters
                case RFC7468Label.EcParameters:
                    _label = PrivacyEnhancedMail.EC_PARAMETERS;
                    break;
                case RFC7468Label.DhParameters:
                    _label = PrivacyEnhancedMail.DH_PARAMETERS;
                    break;

                default:
                    _label = "";
                    break;
            }

            return _label;
        }
        #endregion

        #region method

        /// <summary>GetX509FromPemFilePath</summary>
        /// <param name="pemFilePath">string</param>
        /// <param name="label">RFC7468Label</param>
        /// <returns>X509Certificate2</returns>
        public static X509Certificate2 GetX509FromPemFilePath(string pemFilePath, RFC7468Label label)
        {
            string pemString = File.ReadAllText(pemFilePath);

#if NETSTD
            return X509CertificateLoader.LoadCertificate(
                PrivacyEnhancedMail.GetBytesFromPemString(
                    pemString, PrivacyEnhancedMail.EnumToString(label)));
#else
            return new X509Certificate2(
                PrivacyEnhancedMail.GetBytesFromPemString(
                    pemString, PrivacyEnhancedMail.EnumToString(label)));
#endif
        }

        /// <summary>GetBase64StringFromPemFilePath</summary>
        /// <param name="pemFilePath">string</param>
        /// <param name="label">RFC7468Label</param>
        /// <returns>Base64String</returns>
        public static string GetBase64StringFromPemFilePath(string pemFilePath, RFC7468Label label)
        {
            string pemString = File.ReadAllText(pemFilePath);

            return CustomEncode.ToBase64String(
                PrivacyEnhancedMail.GetBytesFromPemString(
                    pemString, PrivacyEnhancedMail.EnumToString(label)));
        }

        /// <summary>GetBytesFromPemString</summary>
        /// <param name="pemString">string</param>
        /// <param name="label">string</param>
        /// <returns>Byte[]</returns>
        public static Byte[] GetBytesFromPemString(string pemString, string label)
        {
            string header = String.Format("-----BEGIN {0}-----", label);
            string footer = String.Format("-----END {0}-----", label);

            if(string.IsNullOrEmpty(header)) return null;
            if (string.IsNullOrEmpty(footer)) return null;

            int start = pemString.IndexOf(header, StringComparison.Ordinal);
            if (start < 0) return null;

            start += header.Length;
            int end = pemString.IndexOf(footer, start, StringComparison.Ordinal) - start;

            if (end < 0) return null;

            // X509Certificate2
            return CustomEncode.FromBase64String(pemString.Substring(start, end));
        }

        #endregion
    }
}

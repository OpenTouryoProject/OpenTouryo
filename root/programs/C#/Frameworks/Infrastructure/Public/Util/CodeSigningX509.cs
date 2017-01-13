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
//* クラス名        ：CodeSigning
//* クラス日本語名  ：CodeSigningクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/10  西野  大介        新規作成
//**********************************************************************************

// System
using System;

// 業務フレームワーク（循環参照になるため、参照しない）
// フレームワーク（循環参照になるため、参照しない）

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>
    /// CodeSigningクラス
    /// - RSACryptoServiceProvider:
    ///   MD5, SHA1, SHA256, SHA384, SHA512
    /// - DSACryptoServiceProvider:SHA1
    /// だけ、サポート。
    /// </summary>
    public class CodeSigningX509 : CodeSigning
    {
        // 署名の場合は、秘密鍵で署名して、公開鍵で検証。

        #region mem & prop & constructor

        /// <summary>
        /// RSAの場合、以下からHashAlgorithm名を指定する。
        /// MD5, SHA1, SHA256, SHA384, SHA512
        /// </summary>
        string _hashAlgorithmName = "";

        /// <summary>X.509証明書</summary>
        private X509Certificate2 _X509Certificate = null;

        /// <summary>X.509証明書の秘密鍵</summary>
        public string X509PrivateKey
        {
            get
            {
                return "- hidden -";
            }
        }

        /// <summary>X.509証明書の公開鍵</summary>
        public string X509PublicKey
        {
            get
            {
                return CustomEncode.ToBase64String(this._X509Certificate.GetPublicKey());
            }
        }

        /// <summary>
        /// Constructor
        /// X.509証明書(*.pfx, *.cer)からキーを設定する。
        /// *.cer証明書の場合は、証明書チェーンが繋がっている必要がある。
        /// 自己証明書の場合「信頼されたルート証明機関」にInstallするなどする。
        /// </summary>
        /// <param name="certificateFilePath">X.509証明書(*.pfx, *.cer)へのパス</param>
        /// <param name="password">パスワード</param>
        /// <param name="hashAlgorithmName">
        /// RSAの場合、以下からHashAlgorithm名を指定する。
        /// MD5, SHA1, SHA256, SHA384, SHA512
        /// </param>
        public CodeSigningX509(string certificateFilePath, string password, string hashAlgorithmName)
        {
            this._X509Certificate = new X509Certificate2(
                certificateFilePath, password, X509KeyStorageFlags.Exportable);

            this._hashAlgorithmName = hashAlgorithmName;
        }

        #endregion

        #region 署名(X509Certificate)

        /// <summary>Sign</summary>
        /// <param name="data">data</param>
        /// <returns>署名</returns>
        public override byte[] Sign(byte[] data)
        {
            // アルゴリズム（RSA or DSA）は、ココで決まるもよう。
            AsymmetricAlgorithm aa = this._X509Certificate.PrivateKey;

            // 署名
            byte[] signedByte = null;

            if (aa is RSACryptoServiceProvider)
            {
                // RSACryptoServiceProvider : ExportParametersして生成し直している。
                RSAParameters rsaPara = ((RSACryptoServiceProvider)(aa)).ExportParameters(true);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(this._X509Certificate.PrivateKey.KeySize);
                rsa.ImportParameters(rsaPara);
                signedByte = rsa.SignData(data, this._hashAlgorithmName);
            }
            else
            {
                // DSACryptoServiceProvider : ExportParametersして生成し直している。
                DSAParameters dsaPara = ((DSACryptoServiceProvider)(aa)).ExportParameters(true);
                DSACryptoServiceProvider dsa = new DSACryptoServiceProvider(this._X509Certificate.PrivateKey.KeySize);
                dsa.ImportParameters(dsaPara);
                signedByte = dsa.SignData(data); // SHA-1 固定のため？
            }

            return signedByte;
        }

        /// <summary>Verify</summary>
        /// <param name="data">data</param>
        /// <param name="sign">署名</param>
        /// <returns>検証結果</returns>
        public override bool Verify(byte[] data, byte[] sign)
        {
            AsymmetricAlgorithm aa = null;

            // アルゴリズム（RSA or DSA）は、ココで決まるもよう。
            aa = this._X509Certificate.PrivateKey;

            // 結果フラグ
            bool flg = false;

            if (aa is RSACryptoServiceProvider)
            {
                // RSACryptoServiceProvider : ExportParametersして生成し直している。
                RSAParameters rsaPara = ((RSACryptoServiceProvider)(aa)).ExportParameters(true);
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(this._X509Certificate.PrivateKey.KeySize);
                rsa.ImportParameters(rsaPara);
                flg = rsa.VerifyData(data, this._hashAlgorithmName, sign);
            }
            else
            {
                // DSACryptoServiceProvider : ExportParametersして生成し直している。
                DSAParameters dsaPara = ((DSACryptoServiceProvider)(aa)).ExportParameters(true);
                DSACryptoServiceProvider dsa = new DSACryptoServiceProvider(this._X509Certificate.PrivateKey.KeySize);
                dsa.ImportParameters(dsaPara);
                flg = dsa.VerifyData(data, sign); // SHA-1 固定のため？
            }

            return flg;
        }

        #endregion
    }
}
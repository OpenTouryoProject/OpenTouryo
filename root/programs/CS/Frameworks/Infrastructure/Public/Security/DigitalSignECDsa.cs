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
//* クラス名        ：DigitalSignECDsa
//* クラス日本語名  ：DigitalSignECDsaクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/31  西野 大介         新規作成
//*  2018/11/07  西野 大介         ECDSA証明書のサポートを追加（4.7以上）
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>DigitalSignECDsaクラス</summary>
    public class DigitalSignECDsa : DigitalSign
    {
        // デジタル署名の場合は、秘密鍵で署名して、公開鍵で検証。

        #region mem & prop & constructor

        #region mem & prop

        /// <summary>X.509証明書</summary>
        public X509Certificate2 X509Certificate { get; protected set; }

        /// <summary>_privateKey</summary>
        private CngKey _privateKey = null;
        /// <summary>PrivateKey</summary>
        public CngKey PrivateKey
        {
            get
            {
                return this._privateKey;
            }
            protected set
            {
                this._privateKey = value;
            }
        }

        /// <summary>_publicKey</summary>
        private byte[] _publicKey = null;
        /// <summary>PublicKey</summary>
        public byte[] PublicKey
        {
            get
            {
                return this._publicKey;
            }
            protected set
            {
                this._publicKey = value;
            }
        }

        #endregion

        #region constructor
        
        /// <summary>Constructor</summary>
        /// <param name="eaa">EnumDigitalSignAlgorithm</param>
        public DigitalSignECDsa(EnumDigitalSignAlgorithm eaa)
        {
            AsymmetricAlgorithm aa = null;
            HashAlgorithm ha = null;

            AsymmetricAlgorithmCmnFunc.CreateDigitalSignServiceProvider(eaa, out aa, out ha);

            this._privateKey = ((ECDsaCng)aa).Key;
            this._publicKey = this._privateKey.Export(CngKeyBlobFormat.GenericPublicBlob);

            this.AsymmetricAlgorithm = aa;
            this.HashAlgorithm = ha;
        }

        /// <summary>Constructor</summary>
        /// <param name="publicKey">公開鍵</param>
        public DigitalSignECDsa(byte[] publicKey)
        {
            this._privateKey = null;
            this._publicKey = publicKey;
        }

        /// <summary>Constructor</summary>
        /// <param name="privateKey">秘密鍵</param>
        public DigitalSignECDsa(CngKey privateKey)
        {
            this._privateKey = privateKey;
            this._publicKey = this._privateKey.Export(CngKeyBlobFormat.GenericPublicBlob);
        }

#if NET45 || NET46
#else
        /// <summary>
        /// Constructor
        /// X.509証明書(*.pfx, *.cer)からキーを設定する。
        /// *.cer証明書の場合は、証明書チェーンが繋がっている必要がある。
        /// 自己証明書の場合「信頼されたルート証明機関」にInstallするなどする。
        /// </summary>
        /// <param name="certificateFilePath">X.509証明書(*.pfx, *.cer)へのパス</param>
        /// <param name="password">パスワード</param>
        /// <param name="hashAlgorithmName">HashAlgorithmName</param>
        public DigitalSignECDsa(string certificateFilePath, string password, HashAlgorithmName hashAlgorithmName) :
            this(certificateFilePath, password, hashAlgorithmName, X509KeyStorageFlags.DefaultKeySet) { }

        /// <summary>
        /// Constructor
        /// X.509証明書(*.pfx, *.cer)からキーを設定する。
        /// *.cer証明書の場合は、証明書チェーンが繋がっている必要がある。
        /// 自己証明書の場合「信頼されたルート証明機関」にInstallするなどする。
        /// </summary>
        /// <param name="certificateFilePath">X.509証明書(*.pfx, *.cer)へのパス</param>
        /// <param name="password">パスワード</param>
        /// <param name="hashAlgorithmName">HashAlgorithmName</param>
        /// <param name="flag">X509KeyStorageFlags</param>
        public DigitalSignECDsa(string certificateFilePath, string password, HashAlgorithmName hashAlgorithmName, X509KeyStorageFlags flag)
        {
            this.X509Certificate = new X509Certificate2(certificateFilePath, password, flag);
            this._hashAlgorithmName = hashAlgorithmName;
        }
#endif

        #endregion

        #endregion

        #region デジタル署名(ECDsa)

        /// <summary>デジタル署名を作成する</summary>
        /// <param name="data">デジタル署名を行なう対象データ</param>
        /// <returns>対象データに対してデジタル署名したデジタル署名部分のデータ</returns>
        public override byte[] Sign(byte[] data)
        {
            if (this.X509Certificate == null)
            {
                ECDsaCng  aa = new ECDsaCng(this._privateKey);
                return aa.SignData(data);
            }

#if NET45 || NET46
            throw new NotImplementedException();
#else
            ECDsa aa2 = this.X509Certificate.GetECDsaPrivateKey();
            return aa2.SignData(data, this._hashAlgorithmName.Value);
#endif
        }

        /// <summary>デジタル署名を検証する</summary>
        /// <param name="data">デジタル署名を行なった対象データ</param>
        /// <param name="sign">対象データに対してデジタル署名したデジタル署名部分のデータ</param>
        /// <returns>検証結果( true:検証成功, false:検証失敗 )</returns>
        public override bool Verify(byte[] data, byte[] sign)
        {
            if (this.X509Certificate == null)
            {
                ECDsaCng aa = new ECDsaCng(CngKey.Import(
                this._publicKey, CngKeyBlobFormat.GenericPublicBlob));
                return aa.VerifyData(data, sign);
            }

#if NET45 || NET46
            throw new NotImplementedException();
#else
            ECDsa aa2 = this.X509Certificate.GetECDsaPublicKey();
            return aa2.VerifyData(data, sign, this._hashAlgorithmName.Value);
#endif
        }

        #endregion

        // こちらは、MyDispose (派生の末端を呼ぶ) の実装は不要。
    }
}

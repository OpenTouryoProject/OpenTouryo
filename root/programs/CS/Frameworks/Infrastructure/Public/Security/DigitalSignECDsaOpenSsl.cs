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
//* クラス名        ：DigitalSignECDsaOpenSsl
//* クラス日本語名  ：DigitalSignECDsaOpenSslクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/13  西野 大介         新規作成
//**********************************************************************************

// ECDsaOpenSsl Class (System.Security.Cryptography) | Microsoft Docs
// https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.ecdsaopenssl
//   ExportParameters(bool includePrivateParameters)でインポート・エクスポートする。

using System;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>DigitalSignECDsaOpenSslクラス</summary>
    public class DigitalSignECDsaOpenSsl : DigitalSign
    {
        // デジタル署名の場合は、秘密鍵で署名して、公開鍵で検証。

        #region mem & prop & constructor

        #region mem & prop

        /// <summary>_privateKey</summary>
        private ECParameters? _privateKey = null;
        /// <summary>PrivateKey</summary>
        public ECParameters? PrivateKey
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
        private ECParameters? _publicKey = null;
        /// <summary>PublicKey</summary>
        public ECParameters? PublicKey
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
        public DigitalSignECDsaOpenSsl(EnumDigitalSignAlgorithm eaa)
        {
            AsymmetricAlgorithm aa = null;
            HashAlgorithm ha = null;

            AsymmetricAlgorithmCmnFunc.CreateDigitalSignSP(eaa, out aa, out ha);

            ECDsaOpenSsl ecdsa = (ECDsaOpenSsl)aa;
            this._privateKey = ecdsa.ExportParameters(true);
            this._publicKey = ecdsa.ExportParameters(false);

            this.AsymmetricAlgorithm = aa;
            this.HashAlgorithm = ha;
        }

        /// <summary>Constructor</summary>
        /// <param name="eCParameters">ECParameters（任意）</param>
        /// <param name="hashAlgorithm">HashAlgorithm</param>
        public DigitalSignECDsaOpenSsl(ECParameters eCParameters, HashAlgorithm hashAlgorithm)
        {
            ECDsaOpenSsl ecdsa = new ECDsaOpenSsl(eCParameters.Curve);
            ecdsa.ImportParameters(eCParameters);
            if (eCParameters.D != null)
            {
                this._privateKey = ecdsa.ExportParameters(true);
            }
            this._publicKey = ecdsa.ExportParameters(false);

            this.AsymmetricAlgorithm = ecdsa;

            this.AsymmetricAlgorithm = ecdsa;
            this.HashAlgorithm = hashAlgorithm;
        }

        #endregion

        #endregion

        #region デジタル署名(ECDsa)

        /// <summary>デジタル署名を作成する</summary>
        /// <param name="data">デジタル署名を行なう対象データ</param>
        /// <returns>対象データに対してデジタル署名したデジタル署名部分のデータ</returns>
        public override byte[] Sign(byte[] data)
        {
            return ((ECDsaOpenSsl)this.AsymmetricAlgorithm).
                SignData(data, this.HashAlgorithmName);
        }

        /// <summary>デジタル署名を検証する</summary>
        /// <param name="data">デジタル署名を行なった対象データ</param>
        /// <param name="sign">対象データに対してデジタル署名したデジタル署名部分のデータ</param>
        /// <returns>検証結果( true:検証成功, false:検証失敗 )</returns>
        public override bool Verify(byte[] data, byte[] sign)
        {
            return ((ECDsaOpenSsl)this.AsymmetricAlgorithm).
                VerifyData(data, sign, this.HashAlgorithmName);
        }

        /// <summary>デジタル署名を作成する</summary>
        /// <param name="data">デジタル署名を行なう対象データ</param>
        /// <returns>対象データに対してデジタル署名したデジタル署名部分のデータ</returns>
        public override byte[] SignByFormatter(byte[] data)
        {
            throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
        }

        /// <summary>デジタル署名を検証する</summary>
        /// <param name="data">デジタル署名を行なった対象データ</param>
        /// <param name="sign">対象データに対してデジタル署名したデジタル署名部分のデータ</param>
        /// <returns>検証結果( true:検証成功, false:検証失敗 )</returns>
        public override bool VerifyByDeformatter(byte[] data, byte[] sign)
        {
            throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
        }

        #endregion

        // こちらは、MyDispose (派生の末端を呼ぶ) の実装は不要。
    }
}

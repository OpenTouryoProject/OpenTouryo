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
//* クラス名        ：DigitalSignECDsaCng
//* クラス日本語名  ：DigitalSignECDsaCngクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/31  西野 大介         新規作成
//*  2018/11/09  西野 大介         RSAOpenSsl、DSAOpenSsl、HashAlgorithmName対応
//*  2026/10/03  玄人 幸道         ダイジェストを指定するコンストラクタを追加（#595）
//*                                未指定のときは従来どおり ECDsaCng の既定（SHA-256）
//**********************************************************************************

// ECDsaCng Class (System.Security.Cryptography) | Microsoft Docs
// https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.ecdsacng
//   CngKeyBlobFormat.EccPublicBlobでインポート・エクスポートする。

using System;
using System.Runtime.Versioning;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>DigitalSignECDsaCngクラス</summary>
    public class DigitalSignECDsaCng : DigitalSign
    {
        // デジタル署名の場合は、秘密鍵で署名して、公開鍵で検証。

        #region mem & prop & constructor

        #region mem & prop

        /// <summary>OperatingSystem</summary>
        private OperatingSystem os = Environment.OSVersion;

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

        // ダイジェスト（hashAlgorithm）を指定しないコンストラクタは、
        // ECDsaCng の既定（SHA-256）で署名・検証する（従来の挙動）。
        // P-384 / P-521 で ES384 / ES512 を扱う場合は、指定する側を使うこと（#595）。

        /// <summary>Constructor</summary>
        /// <param name="eaa">EnumDigitalSignAlgorithm</param>
        public DigitalSignECDsaCng(EnumDigitalSignAlgorithm eaa) : this(eaa, null) { }

        /// <summary>Constructor</summary>
        /// <param name="eaa">EnumDigitalSignAlgorithm</param>
        /// <param name="hashAlgorithm">ダイジェスト（null なら ECDsaCng の既定）</param>
        public DigitalSignECDsaCng(EnumDigitalSignAlgorithm eaa, HashAlgorithm hashAlgorithm)
        {
            AsymmetricAlgorithm aa = null;
            HashAlgorithm ha = null;

            AsymmetricAlgorithmCmnFunc.CreateDigitalSignSP(eaa, out aa, out ha);

            ECDsaCng ecdsa = (ECDsaCng)aa;
            this._privateKey = ecdsa.Key;
            this._publicKey = this._privateKey.Export(CngKeyBlobFormat.EccPublicBlob);

            this.AsymmetricAlgorithm = aa;
            this.HashAlgorithm = hashAlgorithm; // ECDsaCng の場合、ha は常に null
        }

        /// <summary>Constructor</summary>
        /// <param name="publicKey">公開鍵</param>
        public DigitalSignECDsaCng(byte[] publicKey) : this(publicKey, null) { }

        /// <summary>Constructor</summary>
        /// <param name="publicKey">公開鍵</param>
        /// <param name="hashAlgorithm">ダイジェスト（null なら ECDsaCng の既定）</param>
        public DigitalSignECDsaCng(byte[] publicKey, HashAlgorithm hashAlgorithm)
        {
            this._privateKey = null;
            this._publicKey = publicKey;
            this.HashAlgorithm = hashAlgorithm;
        }

        /// <summary>Constructor</summary>
        /// <param name="cngKey">任意鍵</param>
        /// <param name="isPrivate">秘密鍵か否か</param>
        public DigitalSignECDsaCng(CngKey cngKey, bool isPrivate) : this(cngKey, isPrivate, null) { }

        /// <summary>Constructor</summary>
        /// <param name="cngKey">任意鍵</param>
        /// <param name="isPrivate">秘密鍵か否か</param>
        /// <param name="hashAlgorithm">ダイジェスト（null なら ECDsaCng の既定）</param>
        public DigitalSignECDsaCng(CngKey cngKey, bool isPrivate, HashAlgorithm hashAlgorithm)
        {
            this._publicKey = cngKey.Export(CngKeyBlobFormat.GenericPublicBlob);

            if (isPrivate)
            {
                this._privateKey = cngKey;
            }

            this.HashAlgorithm = hashAlgorithm;
        }

        /// <summary>Constructor</summary>
        /// <param name="ecp">任意鍵</param>
        /// <param name="isPrivate">秘密鍵か否か</param>
        //[SupportedOSPlatform("windows")] // #if できない。
        public DigitalSignECDsaCng(ECParameters ecp, bool isPrivate) : this(ecp, isPrivate, null) { }

        /// <summary>Constructor</summary>
        /// <param name="ecp">任意鍵</param>
        /// <param name="isPrivate">秘密鍵か否か</param>
        /// <param name="hashAlgorithm">ダイジェスト（null なら ECDsaCng の既定）</param>
        //[SupportedOSPlatform("windows")] // #if できない。
        public DigitalSignECDsaCng(ECParameters ecp, bool isPrivate, HashAlgorithm hashAlgorithm)
        {
            this.HashAlgorithm = hashAlgorithm;

#if NETSTD
            if (OperatingSystem.IsWindows())
#else
            if (os.Platform == PlatformID.Win32NT)
#endif
            {
                ECDsaCng ecDsaCng = new ECDsaCng();
                ecDsaCng.ImportParameters(ecp);
                CngKey cngKey = ecDsaCng.Key;

                this._publicKey = cngKey.Export(CngKeyBlobFormat.GenericPublicBlob);

                if (isPrivate)
                {
                    this._privateKey = cngKey;
                }
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }            
        }

        #endregion

        #endregion

        #region デジタル署名(ECDsa)

        /// <summary>デジタル署名を作成する</summary>
        /// <param name="data">デジタル署名を行なう対象データ</param>
        /// <returns>対象データに対してデジタル署名したデジタル署名部分のデータ</returns>
        //[SupportedOSPlatform("windows")] // #if できない。
        public override byte[] Sign(byte[] data)
        {
#if NETSTD
            if (OperatingSystem.IsWindows())
#else
            if (os.Platform == PlatformID.Win32NT)
#endif
            {
                ECDsaCng aa = new ECDsaCng(this._privateKey);
                if (this.HashAlgorithm == null)
                {
                    return aa.SignData(data); // ECDsaCng の既定（SHA-256）
                }
                else
                {
                    return aa.SignData(data, this.HashAlgorithmName);
                }
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }
        }

        /// <summary>デジタル署名を検証する</summary>
        /// <param name="data">デジタル署名を行なった対象データ</param>
        /// <param name="sign">対象データに対してデジタル署名したデジタル署名部分のデータ</param>
        /// <returns>検証結果( true:検証成功, false:検証失敗 )</returns>
        //[SupportedOSPlatform("windows")] // #if できない。
        public override bool Verify(byte[] data, byte[] sign)
        {
#if NETSTD
            if (OperatingSystem.IsWindows())
#else
            if (os.Platform == PlatformID.Win32NT)
#endif
            {
                ECDsaCng aa = new ECDsaCng(CngKey.Import(this._publicKey, CngKeyBlobFormat.EccPublicBlob));
                if (this.HashAlgorithm == null)
                {
                    return aa.VerifyData(data, sign); // ECDsaCng の既定（SHA-256）
                }
                else
                {
                    return aa.VerifyData(data, sign, this.HashAlgorithmName);
                }
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }
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

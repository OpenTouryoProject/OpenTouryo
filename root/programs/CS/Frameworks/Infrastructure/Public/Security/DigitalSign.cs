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
//* クラス名        ：DigitalSign
//* クラス日本語名  ：デジタル署名抽象クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/10  西野 大介         新規作成
//*  2017/09/08  西野 大介         名前空間の移動（ ---> Security ）
//**********************************************************************************

// 参考（少しでもミスると動かないので、結構難しい
// Microsoft Docs
// - 暗号署名
//   https://docs.microsoft.com/ja-jp/dotnet/standard/security/cryptographic-signatures
// - RSAPKCS1SignatureFormatter Class (System.Security.Cryptography)
//   https://docs.microsoft.com/ja-jp/dotnet/api/system.security.cryptography.rsapkcs1signatureformatter
// - RSAPKCS1SignatureDeformatter Class (System.Security.Cryptography)
//   https://docs.microsoft.com/ja-jp/dotnet/api/system.security.cryptography.rsapkcs1signaturedeformatter
// - DSASignatureFormatter Class (System.Security.Cryptography)
//   https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.dsasignatureformatter
// - DSASignatureDeformatter Class (System.Security.Cryptography)
//   https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.dsasignaturedeformatter

using System;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// デジタル署名抽象クラス
    /// </summary>
    public abstract class DigitalSign : IDisposable
    {
        #region mem & prop

        #region Algorithm
        /// <summary>AsymmetricAlgorithm</summary>
        public AsymmetricAlgorithm AsymmetricAlgorithm { get; protected set; }

#if NET45
#else
        /// <summary>RSASignaturePadding</summary>
        public RSASignaturePadding Padding = RSASignaturePadding.Pkcs1;
#endif

        #region Hash

        // ... 面倒なので以下のように一方通行にした。
        // HashAlgorithm(class) ---> HashAlgorithmString(string) ---> HashAlgorithmName(struct)

        /// <summary>HashAlgorithm</summary>
        public HashAlgorithm HashAlgorithm { get; protected set; }

        /// <summary>HashAlgorithmString</summary>
        public string HashAlgorithmString
        {
            get
            {
                return HashAlgorithmCmnFunc.GetHashAlgorithmName(this.HashAlgorithm);
            }
        }

#if NET45
#else
        /// <summary>HashAlgorithmName</summary>
        public HashAlgorithmName HashAlgorithmName
        {
            get
            {   
                return new HashAlgorithmName(this.HashAlgorithmString);
            }
        }
#endif
        #endregion

        #endregion

        #endregion

        #region デジタル署名

        #region Raw

        /// <summary>デジタル署名を作成する</summary>
        /// <param name="data">デジタル署名を行なう対象データ</param>
        /// <returns>対象データに対してデジタル署名したデジタル署名部分のデータ</returns>
        public virtual byte[] Sign(byte[] data)
        {
            AsymmetricAlgorithm aa = this.AsymmetricAlgorithm;

            // デジタル署名
            byte[] signedByte = null;

            if (aa is RSA)
            {
                // RSA
                RSA rsa = (RSA)aa;
#if NET45
                if (rsa is RSACryptoServiceProvider)
                {
                    signedByte = ((RSACryptoServiceProvider)rsa).SignData(data, this.HashAlgorithmString);
                }
                // NET45にRSACng、RSAOpenSsl等は無し。
#else
                signedByte = rsa.SignData(data, this.HashAlgorithmName, this.Padding);
#endif
            }
            else if (aa is DSA)
            {
                // DSA
                DSA dsa = (DSA)aa;
                signedByte = dsa.CreateSignature(data);
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }

            return signedByte;
        }

        /// <summary>デジタル署名を検証する</summary>
        /// <param name="data">デジタル署名を行なった対象データ</param>
        /// <param name="sign">対象データに対してデジタル署名したデジタル署名部分のデータ</param>
        /// <returns>検証結果( true:検証成功, false:検証失敗 )</returns>
        public virtual bool Verify(byte[] data, byte[] sign)
        {
            AsymmetricAlgorithm aa = this.AsymmetricAlgorithm;

            // 検証結果フラグ ( true:検証成功, false:検証失敗 )
            bool flg = false;

            if (aa is RSA)
            {
                // RSA
#if NET45
                if (aa is RSACryptoServiceProvider)
                {
                    return ((RSACryptoServiceProvider)aa).VerifyData(data, this.HashAlgorithmString, sign);
                }
                // NET45にRSACng、RSAOpenSsl等は無し。
#else
                return ((RSA)aa).VerifyData(data, sign, this.HashAlgorithmName, this.Padding);
#endif
            }
            else if (aa is DSA)
            {
                // DSA
                return ((DSA)aa).VerifySignature(data, sign);
            }

            return flg;
        }

        #endregion

        #region Formatter

        /// <summary>デジタル署名を作成する</summary>
        /// <param name="data">デジタル署名を行なう対象データ</param>
        /// <returns>対象データに対してデジタル署名したデジタル署名部分のデータ</returns>
        public virtual byte[] SignByFormatter(byte[] data)
        {
            // ハッシュ
            byte[] hash = this.HashAlgorithm.ComputeHash(data);

            // デジタル署名
            byte[] signedByte = null;

            if (this.AsymmetricAlgorithm is RSA)
            {
                // RSAPKCS1SignatureFormatterオブジェクトを作成
                RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(this.AsymmetricAlgorithm);
                rsaFormatter.SetHashAlgorithm(HashAlgorithmCmnFunc.GetHashAlgorithmName(this.HashAlgorithm));
                signedByte = rsaFormatter.CreateSignature(hash);
            }
            else if (this.AsymmetricAlgorithm is DSA)
            {
                // DSASignatureFormatterオブジェクトを作成
                DSASignatureFormatter dsaFormatter = new DSASignatureFormatter(this.AsymmetricAlgorithm);
                dsaFormatter.SetHashAlgorithm(CryptoConst.SHA1);
                signedByte = dsaFormatter.CreateSignature(hash);
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }

            return signedByte;
        }

        /// <summary>デジタル署名を検証する</summary>
        /// <param name="data">デジタル署名を行なった対象データ</param>
        /// <param name="sign">対象データに対してデジタル署名したデジタル署名部分のデータ</param>
        /// <returns>検証結果( true:検証成功, false:検証失敗 )</returns>
        public virtual bool VerifyByDeformatter(byte[] data, byte[] sign)
        {
            // ハッシュ
            byte[] hash = this.HashAlgorithm.ComputeHash(data);

            if (this.AsymmetricAlgorithm is RSA)
            {
                // RSAPKCS1SignatureDeformatterオブジェクトを作成
                RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(this.AsymmetricAlgorithm);
                rsaDeformatter.SetHashAlgorithm(this.HashAlgorithmString);
                return rsaDeformatter.VerifySignature(hash, sign);
            }
            else if (this.AsymmetricAlgorithm is DSA)
            {
                // DSASignatureDeformatterオブジェクトを作成
                DSASignatureDeformatter dsaDeformatter = new DSASignatureDeformatter(this.AsymmetricAlgorithm);
                dsaDeformatter.SetHashAlgorithm(this.HashAlgorithmString);
                return dsaDeformatter.VerifySignature(hash, sign);
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }
        }

        #endregion

        #endregion

        #region IDisposable

        /// <summary>IsDisposed</summary>
        protected bool IsDisposed = false;

        /// <summary>finalizer</summary>
        ~DigitalSign()
        {
            this.MyDispose(false);
        }

        /// <summary>Close</summary>
        public void Close()
        {
            this.Dispose();
        }

        /// <summary>Dispose</summary>
        public void Dispose()
        {
            this.MyDispose(true);
            // so that Dispose(false) isn't called later from finalizer.
            GC.SuppressFinalize(this);
        }

        /// <summary>MyDispose (派生の末端を呼ぶ)</summary>
        /// <param name="disposing">disposing</param>
        protected virtual void MyDispose(bool disposing) { }

        #endregion
    }
}
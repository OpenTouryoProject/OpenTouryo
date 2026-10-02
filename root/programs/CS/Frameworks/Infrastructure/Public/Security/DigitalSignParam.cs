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
//* クラス名        ：DigitalSignParam
//* クラス日本語名  ：DigitalSignParamクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/12/25  西野 大介         新規作成
//*  2018/11/09  西野 大介         RSAOpenSsl、DSAOpenSsl、HashAlgorithmName対応
//*  2018/11/27  西野 大介         コンストラクタをRSA秘密鍵にも対応させた。
//*  2026/10/03  玄人 幸道         パディングを指定するコンストラクタを追加（RSASSA-PSS 用）（#596）
//**********************************************************************************

using System;
using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// DigitalSignXMLクラス
    /// - RSACryptoServiceProvider:
    ///   MD5, SHA1, SHA256, SHA384, SHA512
    /// - DSACryptoServiceProvider:SHA1
    /// だけ、サポート。
    /// </summary>
    public class DigitalSignParam : DigitalSign
    {
        // デジタル署名の場合は、秘密鍵で署名して、公開鍵で検証。

        #region mem & prop & constructor

        /// <summary>PrivateKey</summary>
        public object PrivateKey
        {
            get
            {
                if (this.AsymmetricAlgorithm is RSA)
                {
                    return ((RSA)this.AsymmetricAlgorithm).ExportParameters(true);
                }
                else if (this.AsymmetricAlgorithm is DSA)
                {
                    return ((DSA)this.AsymmetricAlgorithm).ExportParameters(true);
                }
                return null;
            }
        }

        /// <summary>PublicKey</summary>
        public object PublicKey
        {
            get
            {
                if (this.AsymmetricAlgorithm is RSA)
                {
                    return ((RSA)this.AsymmetricAlgorithm).ExportParameters(false);
                }
                else if (this.AsymmetricAlgorithm is DSA)
                {
                    return ((DSA)this.AsymmetricAlgorithm).ExportParameters(false);
                }
                return null;
            }
        }

        /// <summary>Constructor</summary>
        /// <param name="eaa">EnumDigitalSignAlgorithm</param>
        public DigitalSignParam(EnumDigitalSignAlgorithm eaa)
        {
            AsymmetricAlgorithm aa = null;
            HashAlgorithm ha = null;

            AsymmetricAlgorithmCmnFunc.CreateDigitalSignSP(eaa, out aa, out ha);

            this.AsymmetricAlgorithm = aa;
            this.HashAlgorithm = ha;
        }

        /// <summary>Constructor</summary>
        /// <param name="rsaParameters">RSAParameters</param>
        /// <param name="eaa">EnumDigitalSignAlgorithm</param>
        public DigitalSignParam(RSAParameters rsaParameters, EnumDigitalSignAlgorithm eaa)
        {
            AsymmetricAlgorithm aa = null;
            HashAlgorithm ha = null;

            AsymmetricAlgorithmCmnFunc.CreateDigitalSignSP(eaa, out aa, out ha);

            if (aa is RSA)
            {
                DigitalSignParam.ImportRsaParameters((RSA)aa, rsaParameters);
            }
            else
            {
                throw new ArgumentException("unmatched");
            }

            this.AsymmetricAlgorithm = aa;
            this.HashAlgorithm = ha;
        }

        /// <summary>Constructor（パディングを指定する。鍵は新規に生成する）</summary>
        /// <param name="hashAlgorithmName">HashAlgorithmName</param>
        /// <param name="padding">RSASignaturePadding</param>
        /// <remarks>
        /// RSASSA-PSS（RSASignaturePadding.Pss）で使う。
        /// EnumDigitalSignAlgorithm を取るコンストラクタは RSACryptoServiceProvider を生成するが、
        /// RSACryptoServiceProvider は PSS に対応しないため、こちらは PSS に対応する実装を生成する。
        /// </remarks>
        public DigitalSignParam(HashAlgorithmName hashAlgorithmName, RSASignaturePadding padding)
        {
            this.AsymmetricAlgorithm = DigitalSignParam.CreateRsaForPadding();
            this.HashAlgorithmName = hashAlgorithmName;
            this.Padding = padding;
        }

        /// <summary>Constructor（パディングを指定する）</summary>
        /// <param name="rsaParameters">RSAParameters</param>
        /// <param name="hashAlgorithmName">HashAlgorithmName</param>
        /// <param name="padding">RSASignaturePadding</param>
        /// <remarks>
        /// RSASSA-PSS（RSASignaturePadding.Pss）で使う。
        /// 理由は DigitalSignParam(HashAlgorithmName, RSASignaturePadding) と同じ。
        /// </remarks>
        public DigitalSignParam(RSAParameters rsaParameters, HashAlgorithmName hashAlgorithmName, RSASignaturePadding padding)
        {
            RSA rsa = DigitalSignParam.CreateRsaForPadding();
            DigitalSignParam.ImportRsaParameters(rsa, rsaParameters);

            this.AsymmetricAlgorithm = rsa;
            this.HashAlgorithmName = hashAlgorithmName;
            this.Padding = padding;
        }

        /// <summary>Constructor</summary>
        /// <param name="dsaParameters">DSAParameters</param>
        /// <param name="eaa">EnumDigitalSignAlgorithm</param>
        public DigitalSignParam(DSAParameters dsaParameters, EnumDigitalSignAlgorithm eaa)
        {
            AsymmetricAlgorithm aa = null;
            HashAlgorithm ha = null;

            AsymmetricAlgorithmCmnFunc.CreateDigitalSignSP(eaa, out aa, out ha);

            if (aa is DSA)
            {
                ((DSA)aa).ImportParameters(dsaParameters);
            }
            else
            {
                throw new ArgumentException("unmatched");
            }

            this.AsymmetricAlgorithm = aa;
            this.HashAlgorithm = ha;
        }

        #endregion

        #region 内部関数

        /// <summary>RSAParametersをインポートする（公開鍵のみ、または秘密鍵）</summary>
        /// <param name="rsa">RSA</param>
        /// <param name="rsaParameters">RSAParameters</param>
        private static void ImportRsaParameters(RSA rsa, RSAParameters rsaParameters)
        {
            RSAParameters temp = new RSAParameters()
            {
                // Public
                Modulus = rsaParameters.Modulus,
                Exponent = rsaParameters.Exponent,
            };

            if (rsaParameters.D != null
                && rsaParameters.D.Length != 0)
            {
                // Private
                temp.D = rsaParameters.D;
                temp.P = rsaParameters.P;
                temp.Q = rsaParameters.Q;
                temp.DP = rsaParameters.DP;
                temp.DQ = rsaParameters.DQ;
                temp.InverseQ = rsaParameters.InverseQ;
            }

            rsa.ImportParameters(temp);
        }

        /// <summary>パディング（PSS）に対応する RSA を生成する</summary>
        /// <returns>RSA</returns>
        private static RSA CreateRsaForPadding()
        {
#if NETCOREAPP
            // Windows では RSACng、Linux では RSAOpenSsl になる。
            return RSA.Create();
#else
            // net48 の RSA.Create() は RSACryptoServiceProvider（PSS 非対応）を返す。
            return new RSACng();
#endif
        }

        #endregion

        #region MyDispose (派生の末端を呼ぶ)

        /// <summary>MyDispose (派生の末端を呼ぶ)</summary>
        /// <param name="isDisposing">isDisposing</param>
        protected override void MyDispose(bool isDisposing)
        {
            if (this.IsDisposed)
            {
                // 後処理済み。
                // 何もしない。
            }
            else
            {
                // 後処理。
                if (isDisposing)
                {
                    // Dispose all owned managed objects
                    if (this.AsymmetricAlgorithm is RSA)
                    {
                        if (this.AsymmetricAlgorithm is RSACryptoServiceProvider)
                        {
                            ((RSACryptoServiceProvider)this.AsymmetricAlgorithm).PersistKeyInCsp = false;
                        }
                        this.AsymmetricAlgorithm.Clear();
                    }
                    else
                    {
                        if (this.AsymmetricAlgorithm is DSACryptoServiceProvider)
                        {
                            ((DSACryptoServiceProvider)this.AsymmetricAlgorithm).PersistKeyInCsp = false;
                        }
                        this.AsymmetricAlgorithm.Clear();
                    }
                }

                // Release unmanaged resources
                // 無し

                this.IsDisposed = true;
            }
        }

        #endregion
    }
}

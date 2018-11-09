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
//* クラス名        ：DigitalSignXML
//* クラス日本語名  ：DigitalSignXMLクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/10  西野 大介         新規作成
//*  2017/09/08  西野 大介         名前空間の移動（ ---> Security ）
//*  2017/12/25  西野 大介         暗号化ライブラリ追加に伴うコード追加・修正
//**********************************************************************************

using System;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.FastReflection;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// DigitalSignXMLクラス
    /// - RSACryptoServiceProvider:
    ///   MD5, SHA1, SHA256, SHA384, SHA512
    /// - DSACryptoServiceProvider:SHA1
    /// だけ、サポート。
    /// </summary>
    public class DigitalSignXML : DigitalSign
    {
        // デジタル署名の場合は、秘密鍵で署名して、公開鍵で検証。

        #region mem & prop & constructor

        /// <summary>PrivateKey</summary>
        public string PrivateKey
        {
            get
            {
                if (this.AsymmetricAlgorithm is RSA)
                {
                    return ((RSA)this.AsymmetricAlgorithm).ToXmlString(true);
                }
                else if (this.AsymmetricAlgorithm is DSA)
                {
                    return ((DSA)this.AsymmetricAlgorithm).ToXmlString(true);
                }
                return null;
            }
        }

        /// <summary>PublicKey</summary>
        public string PublicKey
        {
            get
            {
                if (this.AsymmetricAlgorithm is RSA)
                {
                    return ((RSA)this.AsymmetricAlgorithm).ToXmlString(false);
                }
                else if (this.AsymmetricAlgorithm is DSA)
                {
                    return ((DSA)this.AsymmetricAlgorithm).ToXmlString(false);
                }
                return null;
            }
        }

        /// <summary>Constructor</summary>
        /// <param name="eaa">EnumDigitalSignAlgorithm</param>
        public DigitalSignXML(EnumDigitalSignAlgorithm eaa)
        {
            AsymmetricAlgorithm aa = null;
            HashAlgorithm ha = null;

            AsymmetricAlgorithmCmnFunc.CreateDigitalSignServiceProvider(eaa, out aa, out ha);

            this.AsymmetricAlgorithm = aa;
            this.HashAlgorithm = ha;
        }

        /// <summary>Constructor</summary>
        /// <param name="eaa">EnumDigitalSignAlgorithm</param>
        /// <param name="xmlKey">string</param>
        public DigitalSignXML(EnumDigitalSignAlgorithm eaa, string xmlKey)
        {
            AsymmetricAlgorithm aa = null;
            HashAlgorithm ha = null;

            AsymmetricAlgorithmCmnFunc.CreateDigitalSignServiceProvider(eaa, out aa, out ha);

            this.AsymmetricAlgorithm = aa;
            this.HashAlgorithm = ha;

            if (aa is RSA)
            {
                RSA rsa = (RSA)aa;
                rsa.FromXmlString(xmlKey);
                this.AsymmetricAlgorithm = rsa;
            }
            else if (aa is DSA)
            {
                DSA dsa = (DSACryptoServiceProvider)aa;
                dsa.FromXmlString(xmlKey);
                this.AsymmetricAlgorithm = dsa;
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }
        }

        #endregion

        #region デジタル署名(XML)

        /// <summary>デジタル署名を作成する</summary>
        /// <param name="data">デジタル署名を行なう対象データ</param>
        /// <returns>対象データに対してデジタル署名したデジタル署名部分のデータ</returns>
        public override byte[] Sign(byte[] data)
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
        public override bool Verify(byte[] data, byte[] sign)
        {
            // ハッシュ
            byte[] hash = this.HashAlgorithm.ComputeHash(data);

            if (this.AsymmetricAlgorithm is RSA)
            {
                // RSAPKCS1SignatureDeformatterオブジェクトを作成
                RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(this.AsymmetricAlgorithm);
                rsaDeformatter.SetHashAlgorithm(HashAlgorithmCmnFunc.GetHashAlgorithmName(this.HashAlgorithm));
                return rsaDeformatter.VerifySignature(hash, sign);
            }
            else if (this.AsymmetricAlgorithm is DSA)
            {
                // DSASignatureDeformatterオブジェクトを作成
                DSASignatureDeformatter dsaDeformatter = new DSASignatureDeformatter(this.AsymmetricAlgorithm);
                dsaDeformatter.SetHashAlgorithm(HashAlgorithmCmnFunc.GetHashAlgorithmName(this.HashAlgorithm));
                return dsaDeformatter.VerifySignature(hash, sign);
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }
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
                    if (this.AsymmetricAlgorithm is RSACryptoServiceProvider)
                    {
                        // https://msdn.microsoft.com/en-us/library/tswxhw92.aspx
                        // https://msdn.microsoft.com/ja-jp/library/tswxhw92.aspx
                        ((RSACryptoServiceProvider)this.AsymmetricAlgorithm).PersistKeyInCsp = false;
                        this.AsymmetricAlgorithm.Clear();
                    }
                    else
                    {
                        // https://msdn.microsoft.com/en-us/library/tswxhw92.aspx
                        // https://msdn.microsoft.com/ja-jp/library/tswxhw92.aspx
                        ((DSACryptoServiceProvider)this.AsymmetricAlgorithm).PersistKeyInCsp = false;
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

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
//**********************************************************************************

using System;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>DigitalSignECDsaクラス</summary>
    public class DigitalSignECDsa : DigitalSign
    {
        // デジタル署名の場合は、秘密鍵で署名して、公開鍵で検証。

        #region mem & prop & constructor

        /// <summary>AsymmetricAlgorithm</summary>
        public AsymmetricAlgorithm AsymmetricAlgorithm { get; protected set; }
        
        /// <summary>PrivateKey</summary>
        public CngKey PrivateKey { get; protected set; }

        /// <summary>PublicKey</summary>
        public byte[] PublicKey { get; protected set; }

        /// <summary>Constructor</summary>
        /// <param name="eaa">EnumDigitalSignAlgorithm</param>
        public DigitalSignECDsa(EnumDigitalSignAlgorithm eaa)
        {
            AsymmetricAlgorithm aa = null;
            HashAlgorithm ha = null;

            RsaAndDsaCmnFunc.CreateDigitalSignServiceProvider(eaa, out aa, out ha);

            this.AsymmetricAlgorithm = aa;

            if (eaa == EnumDigitalSignAlgorithm.ECDsaCng_P256)
            {
                this.CreateCngKey(CngAlgorithm.ECDsaP256);
            }
            else if (eaa == EnumDigitalSignAlgorithm.ECDsaCng_P384)
            {
                this.CreateCngKey(CngAlgorithm.ECDsaP384);
            }
            else if (eaa == EnumDigitalSignAlgorithm.ECDsaCng_P521)
            {
                this.CreateCngKey(CngAlgorithm.ECDsaP521);
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }
        }

        /// <summary>Constructor</summary>
        /// <param name="privateKey">秘密鍵</param>
        public DigitalSignECDsa(CngKey privateKey)
        {
            this.AsymmetricAlgorithm = new ECDsaCng(privateKey);
        }

        /// <summary>Constructor</summary>
        /// <param name="publicKey">公開鍵</param>
        public DigitalSignECDsa(byte[] publicKey)
        {
            this.AsymmetricAlgorithm = new ECDsaCng(CngKey.Import(publicKey, CngKeyBlobFormat.GenericPublicBlob));
        }

        /// <summary>CreateCngKey</summary>
        /// <param name="cngAlgorithm">CngAlgorithm</param>
        private void CreateCngKey(CngAlgorithm cngAlgorithm)
        {
            this.PrivateKey = CngKey.Create(cngAlgorithm);
            this.PublicKey = this.PrivateKey.Export(CngKeyBlobFormat.GenericPublicBlob);
            // ↓サポートされない操作であるらしい。
            //privateKey = cngKey.Export(CngKeyBlobFormat.GenericPrivateBlob);
        }

        #endregion

        #region デジタル署名(ECDsa)

        /// <summary>デジタル署名を作成する</summary>
        /// <param name="data">デジタル署名を行なう対象データ</param>
        /// <returns>対象データに対してデジタル署名したデジタル署名部分のデータ</returns>
        public override byte[] Sign(byte[] data)
        {
            return ((ECDsaCng)this.AsymmetricAlgorithm).SignData(data);
        }

        /// <summary>デジタル署名を検証する</summary>
        /// <param name="data">デジタル署名を行なった対象データ</param>
        /// <param name="sign">対象データに対してデジタル署名したデジタル署名部分のデータ</param>
        /// <returns>検証結果( true:検証成功, false:検証失敗 )</returns>
        public override bool Verify(byte[] data, byte[] sign)
        {
            return ((ECDsaCng)this.AsymmetricAlgorithm).VerifyData(data, sign);
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

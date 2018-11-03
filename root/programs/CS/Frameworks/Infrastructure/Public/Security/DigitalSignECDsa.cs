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

        /// <summary>Constructor</summary>
        /// <param name="eaa">EnumDigitalSignAlgorithm</param>
        public DigitalSignECDsa(EnumDigitalSignAlgorithm eaa)
        {
            AsymmetricAlgorithm aa = null;
            HashAlgorithm ha = null;

            AsymmetricAlgorithmCmnFunc.CreateDigitalSignServiceProvider(eaa, out aa, out ha);
            this._privateKey = ((ECDsaCng)aa).Key;
            this._publicKey = this._privateKey.Export(CngKeyBlobFormat.GenericPublicBlob);
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

        #endregion

        #region デジタル署名(ECDsa)

        /// <summary>デジタル署名を作成する</summary>
        /// <param name="data">デジタル署名を行なう対象データ</param>
        /// <returns>対象データに対してデジタル署名したデジタル署名部分のデータ</returns>
        public override byte[] Sign(byte[] data)
        {
            ECDsaCng aa = new ECDsaCng(this._privateKey);
            return aa.SignData(data);
        }

        /// <summary>デジタル署名を検証する</summary>
        /// <param name="data">デジタル署名を行なった対象データ</param>
        /// <param name="sign">対象データに対してデジタル署名したデジタル署名部分のデータ</param>
        /// <returns>検証結果( true:検証成功, false:検証失敗 )</returns>
        public override bool Verify(byte[] data, byte[] sign)
        {
            ECDsaCng aa = new ECDsaCng(CngKey.Import(
                this._publicKey, CngKeyBlobFormat.GenericPublicBlob));
            return aa.VerifyData(data, sign);
        }

        #endregion

        // こちらは、MyDispose (派生の末端を呼ぶ) の実装は不要。
    }
}

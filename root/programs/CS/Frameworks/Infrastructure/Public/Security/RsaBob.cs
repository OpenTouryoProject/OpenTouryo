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
//* クラス名        ：RsaBob
//* クラス日本語名  ：RSAのBob抽象クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/31  西野 大介         新規作成
//*  2018/11/09  西野 大介         RSAOpenSsl、DSAOpenSsl、HashAlgorithmName対応
//**********************************************************************************

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>RSAのBob抽象クラス</summary>
    public abstract class RsaBob : RsaKeyExchange
    {
        /// <summary>constructor</summary>
        protected RsaBob()
        {
            RSA rsa = AsymmetricAlgorithmCmnFunc.RsaFactory();
            this._asa = rsa;
            if(rsa is RSACryptoServiceProvider)
            {
                this._exchangeKey = ((RSACryptoServiceProvider)rsa).ExportCspBlob(false); // 交換鍵
            }
            // RSACng、RSAOpenSslはこっち（しかない）
            this._exchangeKey2 = rsa.ExportParameters(false); // 交換鍵（JWK対応
        }

        /// <summary>constructor</summary>
        /// <param name="rsaPfxFilePath">RSAのX.509証明書(*.pfx)へのパス</param>
        /// <param name="password">パスワード</param>
        protected RsaBob(string rsaPfxFilePath, string password) :
            this(rsaPfxFilePath, password, X509KeyStorageFlags.DefaultKeySet) { }

        /// <summary>constructor</summary>
        /// <param name="rsaPfxFilePath">RSAのX.509証明書(*.pfx)へのパス</param>
        /// <param name="password">パスワード</param>
        /// <param name="flag">X509KeyStorageFlags</param>
        protected RsaBob(string rsaPfxFilePath, string password, X509KeyStorageFlags flag)
        {
            X509Certificate2 x509Certificate = new X509Certificate2(rsaPfxFilePath, password, flag);
            
            // RSA
            // *.pfxの場合、ExportParameters(true)して生成し直している。
            AsymmetricAlgorithm aa = x509Certificate.PrivateKey;
            RSA rsa = (RSA)AsymmetricAlgorithmCmnFunc.CreateSameKeySizeSP(aa);
            rsa.ImportParameters(((RSA)(aa)).ExportParameters(true));

            this._asa = rsa;
            if (rsa is RSACryptoServiceProvider)
            {
                this._exchangeKey = ((RSACryptoServiceProvider)rsa).ExportCspBlob(false); // 交換鍵
            }
            // RSACng、RSAOpenSslはこっち（しかない）
            this._exchangeKey2 = rsa.ExportParameters(false); // 交換鍵（JWK対応
        }
    }
}

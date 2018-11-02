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
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            this._asa = rsa;
            this._exchangeKey = rsa.ExportCspBlob(false); // 交換鍵
            this._exchangeKey2 = rsa.ExportParameters(false); // 交換鍵（JWK対応
        }

        /// <summary>constructor</summary>
        /// <param name="rsaPfxFilePath">RSAのPFX証明書のパス</param>
        /// <param name="password">PFX証明書のパスワード</param>
        /// <param name="flag">X509KeyStorageFlags</param>
        protected RsaBob(string rsaPfxFilePath, string password, X509KeyStorageFlags flag)
        {
            X509Certificate2 x509Certificate = new X509Certificate2(rsaPfxFilePath, password, flag);
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)x509Certificate.PrivateKey;

            this._asa = rsa;
            this._exchangeKey = rsa.ExportCspBlob(false); // 交換鍵
            this._exchangeKey2 = rsa.ExportParameters(false); // 交換鍵（JWK対応
        }
    }
}

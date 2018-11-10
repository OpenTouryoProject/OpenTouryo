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

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// デジタル署名抽象クラス
    /// </summary>
    public abstract class DigitalSign : IDisposable
    {
        #region mem & prop

        /// <summary>AsymmetricAlgorithm</summary>
        public AsymmetricAlgorithm AsymmetricAlgorithm { get; protected set; }

        /// <summary>HashAlgorithm</summary>
        public HashAlgorithm HashAlgorithm { get; protected set; }

#if NET45
#else
        /// <summary>HashAlgorithmName</summary>
        public HashAlgorithmName? HashAlgorithmName { get; protected set; }
#endif
        #endregion

        #region インターフェイス

        /// <summary>Sign</summary>
        /// <param name="data">data</param>
        /// <returns>署名</returns>
        public abstract byte[] Sign(byte[] data);

        /// <summary>Verify</summary>
        /// <param name="data">data</param>
        /// <param name="sign">署名</param>
        /// <returns>検証結果</returns>
        public abstract bool Verify(byte[] data, byte[] sign);

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
//**********************************************************************************
//* Copyright (C) 2007,2017 Hitachi Solutions,Ltd.
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
//* クラス日本語名  ：DigitalSign抽象クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/01/10  西野 大介         新規作成
//**********************************************************************************

using System;

namespace Touryo.Infrastructure.Public.Util
{
    #region Enum

    /// <summary>
    /// アルゴリズムのサービスプロバイダの種類
    /// </summary>
    public enum EnumDigitalSignAlgorithm
    {
        /// <summary>RSACryptoServiceProvider:MD5</summary>
        RSACryptoServiceProvider_MD5,

        /// <summary>RSACryptoServiceProvider:SHA1</summary>
        RSACryptoServiceProvider_SHA1,

        /// <summary>RSACryptoServiceProvider:SHA256</summary>
        RSACryptoServiceProvider_SHA256,

        /// <summary>RSACryptoServiceProvider:SHA384</summary>
        RSACryptoServiceProvider_SHA384,

        /// <summary>RSACryptoServiceProvider:SHA512</summary>
        RSACryptoServiceProvider_SHA512,

        /// <summary>DSACryptoServiceProvider:SHA1</summary>
        DSACryptoServiceProvider_SHA1
    };

    #endregion

    /// <summary>
    /// DigitalSignクラス抽象クラス
    /// </summary>
    public abstract class DigitalSign : IDisposable
    {   
        /// <summary>Sign</summary>
        /// <param name="data">data</param>
        /// <returns>署名</returns>
        public abstract byte[] Sign(byte[] data);

        /// <summary>Verify</summary>
        /// <param name="data">data</param>
        /// <param name="sign">署名</param>
        /// <returns>検証結果</returns>
        public abstract bool Verify(byte[] data, byte[] sign);

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
    }
}
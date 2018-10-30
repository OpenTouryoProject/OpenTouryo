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
//* クラス名        ：EnumKeyedHashAlgorithm
//* クラス日本語名  ：ハッシュ（キー付き）アルゴリズムの列挙型
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/30  西野 大介         新規作成（分離）
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// ハッシュ（キー付き）アルゴリズムのサービスプロバイダの種類
    /// </summary>
    public enum EnumKeyedHashAlgorithm
    {
        // 基本的に、CSP (CryptoServiceProvider)
        // mscorlib.dll, netstandard.dll, System.Security.Cryptography.Csp.dll
        // HMACRIPEMD160, MACTripleDESだけ、Managed (mscorlib.dll)

        /// <summary>Default</summary>
        Default,
        
        /// <summary>HMACMD5</summary>
        HMACMD5,

#if NETSTD
#else
        /// <summary>HMACRIPEMD160</summary>
        HMACRIPEMD160,
#endif

        /// <summary>HMACSHA1</summary>
        HMACSHA1,

        /// <summary>HMACSHA256</summary>
        HMACSHA256,

        /// <summary>HMACSHA384</summary>
        HMACSHA384,

        /// <summary>HMACSHA512</summary>
        HMACSHA512,

#if NETSTD
#else
        /// <summary>MACTripleDES</summary>
        MACTripleDES,
#endif
    }
}
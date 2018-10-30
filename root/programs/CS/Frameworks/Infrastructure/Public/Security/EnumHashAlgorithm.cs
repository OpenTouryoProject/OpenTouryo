﻿//**********************************************************************************
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
//* クラス名        ：EnumHashAlgorithm
//* クラス日本語名  ：ハッシュ・アルゴリズムの列挙型
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
    /// ハッシュアルゴリズムのサービスプロバイダの種類
    /// </summary>
    public enum EnumHashAlgorithm
    {
        /// <summary>Default</summary>
        Default,

        #region CSP (CryptoServiceProvider)

        // mscorlib.dll, netstandard.dll, System.Security.Cryptography.Csp.dll

        /// <summary>MD5CryptoServiceProvider</summary>
        MD5CryptoServiceProvider,

        /// <summary>SHA1CryptoServiceProvider</summary>
        SHA1CryptoServiceProvider,

        /// <summary>SHA256CryptoServiceProvider</summary>
        SHA256CryptoServiceProvider,

        /// <summary>SHA384CryptoServiceProvider</summary>
        SHA384CryptoServiceProvider,

        /// <summary>SHA512CryptoServiceProvider</summary>
        SHA512CryptoServiceProvider,

        #endregion

        #region CNG (CryptographyNextGeneration)

        // System.Core.dll

        /// <summary>MD5CryptographyNextGeneration</summary>
        MD5CryptographyNextGeneration,

        /// <summary>SHA1CryptographyNextGeneration</summary>
        SHA1CryptographyNextGeneration,

        /// <summary>SHA256CryptographyNextGeneration</summary>
        SHA256CryptographyNextGeneration,

        /// <summary>SHA384CryptographyNextGeneration</summary>
        SHA384CryptographyNextGeneration,

        /// <summary>SHA512Managed</summary>
        SHA512CryptographyNextGeneration,

        #endregion

        #region Managed

        // mscorlib.dll

        /// <summary>RIPEMD160Managed</summary>
        RIPEMD160Managed,

        /// <summary>SHA1Managed</summary>
        SHA1Managed,

        /// <summary>SHA256Managed</summary>
        SHA256Managed,

        /// <summary>SHA384Managed</summary>
        SHA384Managed,

        /// <summary>SHA512Managed</summary>
        SHA512Managed,

        #endregion

    };
}

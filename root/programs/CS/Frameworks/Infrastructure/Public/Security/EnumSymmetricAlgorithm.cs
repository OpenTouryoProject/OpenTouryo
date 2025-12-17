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
//* クラス名        ：EnumSymmetricAlgorithm
//* クラス日本語名  ：対称アルゴリズムによる暗号化の列挙型
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/30  西野 大介         新規作成（分離）
//**********************************************************************************

using System;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// 対称アルゴリズムによる暗号化の列挙型
    /// </summary>
    [Flags]
    public enum EnumSymmetricAlgorithm
    {
        /// <summary>AES</summary>
        AES = 1,
        /// <summary>DES</summary>
        DES = 1 << 1,
        /// <summary>RC2</summary>
        RC2 = 1 << 2,
        /// <summary>TripleDES</summary>
        TDES = 1 << 3,

        #region CSP (CryptoServiceProvider)
        /*
        /// <summary>AesCryptoServiceProvider</summary>
        AES_CSP = 1,
        /// <summary>DESCryptoServiceProvider</summary>
        DES_CSP = 1 << 1,
        /// <summary>RC2CryptoServiceProvider</summary>
        RC2_CSP = 1 << 2,
        /// <summary>TripleDESCryptoServiceProvider</summary>
        TDES_CSP = 1 << 3,
        */
        #endregion

        #region CNG (CryptographyNextGeneration)

        /// <summary>AesCryptographyNextGeneration</summary>
        AES_CNG = 1 << 4,
        /// <summary>TripleDESCryptographyNextGeneration</summary>
        TDES_CNG = 1 << 5,

        #endregion

        #region Managed
        /*
        /// <summary>AesManaged</summary>
        AES_M = 1 << 6,
        /// <summary>RijndaelManaged</summary>
        Rijndael_M = 1 << 7,
        */
        #endregion

        #region CipherMode, PaddingMode指定

        #region CipherMode
        /// <summary>CipherMode.CBC</summary>
        CipherMode_CBC = 1 << 8,
        /// <summary>CipherMode.CFB</summary>
        CipherMode_CFB = 1 << 9,
        /// <summary>CipherMode.CTS</summary>
        CipherMode_CTS = 1 << 10,
        /// <summary>CipherMode.ECB</summary>
        CipherMode_ECB = 1 << 11,
        /// <summary>CipherMode.OFB</summary>
        CipherMode_OFB = 1 << 12,
        #endregion

        #region PaddingMode
        /// <summary>PaddingMode.None</summary>
        PaddingMode_None = 1 << 13,
        /// <summary>PaddingMode.Zeros</summary>
        PaddingMode_Zeros = 1 << 14,
        /// <summary>PaddingMode.ANSIX923</summary>
        PaddingMode_ANSIX923 = 1 << 15,
        /// <summary>PaddingMode.ISO10126</summary>
        PaddingMode_ISO10126 = 1 << 16,
        /// <summary>PaddingMode.PKCS7</summary>
        PaddingMode_PKCS7 = 1 << 17
        #endregion

        #endregion
    }
}

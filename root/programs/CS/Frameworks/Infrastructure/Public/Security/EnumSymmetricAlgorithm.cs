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
namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// 対称アルゴリズムによる暗号化の列挙型
    /// </summary>
    public enum EnumSymmetricAlgorithm
    {
        #region 古い方針
        // AesCryptoServiceProvider, AesManagedは.NET Framework 3.5からの提供。
        // 暗号化プロバイダ選択の優先順は、高い順に、Managed → CAPI(CSP) → CNG。
        // Aesは、ManagedがあるのでCAPI(CSP)のAesCryptoServiceProviderを削除。
        // サポート範囲の変更により、今後、CAPI(CSP)とCNGの優先順位の反転を検討。
        #endregion

        // Defaultは無し。
        ///// <summary>Default</summary>
        //Default,

        #region CSP (CryptoServiceProvider)

        /// <summary>AesCryptoServiceProvider</summary>
        AesCryptoServiceProvider,

        /// <summary>DESCryptoServiceProvider</summary>
        DESCryptoServiceProvider,

        /// <summary>RC2CryptoServiceProvider</summary>
        RC2CryptoServiceProvider,

        /// <summary>TripleDESCryptoServiceProvider</summary>
        TripleDESCryptoServiceProvider,

        #endregion

        #region CNG (CryptographyNextGeneration)
        
        /// <summary>AesCryptographyNextGeneration</summary>
        AesCryptographyNextGeneration,

        /// <summary>TripleDESCryptographyNextGeneration</summary>
        TripleDESCryptographyNextGeneration,

        #endregion

        #region Managed
        
        /// <summary>AesManaged</summary>
        AesManaged,
        
        /// <summary>RijndaelManaged</summary>
        RijndaelManaged,

        #endregion
    }
}

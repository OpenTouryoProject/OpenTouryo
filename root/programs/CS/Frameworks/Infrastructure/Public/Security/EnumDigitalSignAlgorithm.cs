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
//* クラス名        ：EnumDigitalSignAlgorithm
//* クラス日本語名  ：デジタル署名アルゴリズムの列挙型
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/31  西野 大介         新規作成（分離）
//*  2018/10/31  西野 大介         ECDsaCngアルゴリズムの追加
//*  2018/11/09  西野 大介         RSAOpenSsl & HashAlgorithmName対応
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>デジタル署名アルゴリズムの列挙型</summary>
    /// <remarks>コンストラクタで使用している。</remarks>
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
        DSACryptoServiceProvider_SHA1,

        /// <summary>ECDsaCng:P256</summary>
        ECDsaCng_P256,

        /// <summary>ECDsaCng:P384</summary>
        ECDsaCng_P384,

        /// <summary>ECDsaCng:P521</summary>
        ECDsaCng_P521,

#if NETSTD
        /// <summary>RSAOpenSsl:MD5</summary>
        RSAOpenSsl_MD5,

        /// <summary>RSAOpenSsl:SHA1</summary>
        RSAOpenSsl_SHA1,

        /// <summary>RSAOpenSsl:SHA256</summary>
        RSAOpenSsl_SHA256,

        /// <summary>RSAOpenSsl:SHA384</summary>
        RSAOpenSsl_SHA384,

        /// <summary>RSAOpenSsl:SHA512</summary>
        RSAOpenSsl_SHA512,
#endif

    };
}

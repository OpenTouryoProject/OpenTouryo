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
//* クラス名        ：RsaAndDsaCmnFunc
//* クラス日本語名  ：RsaAndDsaCmnFuncクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/12/25  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// RsaAndDsaCmnFuncクラス
    /// - RSACryptoServiceProvider:
    ///   MD5, SHA1, SHA256, SHA384, SHA512
    /// - DSACryptoServiceProvider:SHA1
    /// だけ、サポート。
    /// </summary>
    class RsaAndDsaCmnFunc
    {
        /// <summary>公開鍵・暗号化サービスプロバイダの生成(param)</summary>
        /// <param name="param">
        /// - RSAParameters
        /// - DSAParameters
        /// </param>
        /// <param name="ha">HashAlgorithm(使用可能かチェック)</param>
        /// <returns>
        /// AsymmetricAlgorithm
        /// - RSACryptoServiceProvider
        /// - DSACryptoServiceProvider
        /// </returns>
        public static AsymmetricAlgorithm CreateAsymmetricAlgorithmFromParam(object param, HashAlgorithm ha)
        {
            if (param is RSAParameters)
            {
                // RSACryptoServiceProvider
                RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider();
                rsaCryptoServiceProvider.ImportParameters((RSAParameters)param);
                
                // HashAlgorithm
                string temp = RsaAndDsaCmnFunc.GetHashAlgorithmName(ha);
                if ("MD5, SHA1, SHA256, SHA384, SHA512".IndexOf(temp) != -1)
                {
                    return rsaCryptoServiceProvider;
                }
                else
                {
                    throw new ArgumentException(
                        PublicExceptionMessage.ARGUMENT_INCORRECT,
                        "The hash algorithm parameter of rsa is incorrect.");
                }
            }
            else if (param is DSAParameters)
            {
                // DSACryptoServiceProvider
                DSACryptoServiceProvider dsaCryptoServiceProvider = new DSACryptoServiceProvider();
                dsaCryptoServiceProvider.ImportParameters((DSAParameters)param);
                
                // HashAlgorithm
                string temp = RsaAndDsaCmnFunc.GetHashAlgorithmName(ha);
                if (temp == "SHA1")
                {
                    return dsaCryptoServiceProvider;
                }
                else
                {
                    throw new ArgumentException(
                        PublicExceptionMessage.ARGUMENT_INCORRECT,
                        "The hash algorithm parameter of dsa is incorrect.");
                }
            }
            else
            {
                throw new ArgumentException(
                    PublicExceptionMessage.ARGUMENT_INCORRECT,
                    "The algorithm parameters is incorrect.");
            }
        }

        /// <summary>署名・検証サービスプロバイダの生成(EnumDigitalSignAlgorithm)</summary>
        /// <param name="eaa">EnumDigitalSignAlgorithm</param>
        /// <param name="aa">
        /// AsymmetricAlgorithm
        /// - RSACryptoServiceProvider
        /// - DSACryptoServiceProvider
        /// </param>
        /// <param name="ha">
        /// HashAlgorithm
        /// </param>
        public static void CreateDigitalSignServiceProvider(
            EnumDigitalSignAlgorithm eaa, out AsymmetricAlgorithm aa, out HashAlgorithm ha)
        {
            aa = null;
            ha = null;

            // 公開鍵・暗号化サービスプロバイダ
            if (eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_MD5)
            {
                // RSACryptoServiceProviderサービスプロバイダ
                aa = new RSACryptoServiceProvider();
                ha = MD5.Create();
            }
            else if (eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA1)
            {
                // RSACryptoServiceProviderサービスプロバイダ
                aa = new RSACryptoServiceProvider();
                ha = SHA1.Create();
            }
            else if (eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA256)
            {
                // RSACryptoServiceProviderサービスプロバイダ
                aa = new RSACryptoServiceProvider();
                ha = SHA256.Create();
            }
            else if (eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA384)
            {
                // RSACryptoServiceProviderサービスプロバイダ
                aa = new RSACryptoServiceProvider();
                ha = SHA384.Create();
            }
            else if (eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA512)
            {
                // RSACryptoServiceProviderサービスプロバイダ
                aa = new RSACryptoServiceProvider();
                ha = SHA512.Create();
            }
            else if (eaa == EnumDigitalSignAlgorithm.DSACryptoServiceProvider_SHA1)
            {
                // DSACryptoServiceProvider
                aa = new DSACryptoServiceProvider();
                ha = SHA1.Create();
            }
            else
            {
                throw new ArgumentException(
                    PublicExceptionMessage.ARGUMENT_INCORRECT,
                    "EnumDigitalSignAlgorithm parameter is incorrect.");
            }
        }

        /// <summary>GetHashAlgorithmName</summary>
        /// <param name="ha">HashAlgorithm</param>
        /// <returns>HashAlgorithmName</returns>
        public static string GetHashAlgorithmName(HashAlgorithm ha)
        {
            string haName = "";

            if (ha is MD5)
            {
                haName = "MD5";
            }
            else if (ha is SHA1)
            {
                haName = "SHA1";
            }
            else if (ha is SHA256)
            {
                haName = "SHA256";
            }
            else if (ha is SHA384)
            {
                haName = "SHA384";
            }
            else if (ha is SHA512)
            {
                haName = "SHA512";
            }
            else
            {
                throw new ArgumentException(
                    PublicExceptionMessage.ARGUMENT_INCORRECT,
                    "HashAlgorithm parameter is incorrect.");
            }

            return haName;
        }
    }
}

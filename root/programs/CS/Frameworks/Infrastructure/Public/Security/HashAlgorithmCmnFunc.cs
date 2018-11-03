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
//* クラス名        ：HashAlgorithmCmnFunc
//* クラス日本語名  ：HashCmnFuncクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/10/31  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>HashAlgorithmCmnFuncクラス</summary>
    public class HashAlgorithmCmnFunc
    {
        #region Algorithm <---> AlgorithmName

        /// <summary>GetHashAlgorithmName</summary>
        /// <param name="ha">HashAlgorithm</param>
        /// <returns>HashAlgorithmName</returns>
        public static string GetHashAlgorithmName(HashAlgorithm ha)
        {
            string haName = "";

            if (ha is MD5)
            {
                haName = CryptoConst.MD5;
            }
            else if (ha is SHA1)
            {
                haName = CryptoConst.SHA1;
            }
            else if (ha is SHA256)
            {
                haName = CryptoConst.SHA256;
            }
            else if (ha is SHA384)
            {
                haName = CryptoConst.SHA384;
            }
            else if (ha is SHA512)
            {
                haName = CryptoConst.SHA512;
            }
            else
            {
                throw new ArgumentException(
                    PublicExceptionMessage.ARGUMENT_INCORRECT,
                    "HashAlgorithm parameter is incorrect.");
            }

            return haName;
        }

        /// <summary>GetHashAlgorithmFromName</summary>
        /// <param name="hashAlgorithmName">string</param>
        /// <returns>HashAlgorithm</returns>
        public static HashAlgorithm GetHashAlgorithmFromName(string hashAlgorithmName)
        {
            return HashAlgorithm.Create(hashAlgorithmName);
        }

        #endregion

        #region CreateAlgorithmServiceProvider

        /// <summary>ハッシュ（キー無し）サービスプロバイダの生成</summary>
        /// <param name="eha">ハッシュ（キー無し）サービスプロバイダの列挙型</param>
        /// <returns>ハッシュ（キー無し）サービスプロバイダ</returns>
        public static HashAlgorithm CreateHashAlgorithmServiceProvider(EnumHashAlgorithm eha)
        {
            // ハッシュ（キー無し）サービスプロバイダ
            HashAlgorithm ha = null;

            if (eha == EnumHashAlgorithm.Default)
            {
                // 既定の暗号化サービスプロバイダ
                ha = HashAlgorithm.Create(); // devps(1703)
            }

            #region MD5
            else if (eha == EnumHashAlgorithm.MD5CryptoServiceProvider)
            {
                // MD5CryptoServiceProviderサービスプロバイダ
                ha = MD5CryptoServiceProvider.Create(); // devps(1703)
            }

#if NETSTD
#else
            else if (eha == EnumHashAlgorithm.MD5CryptographyNextGeneration)
            {
                // MD5Cngサービスプロバイダ
                ha = MD5Cng.Create(); // devps(1703)
            }
#endif

            #endregion

            #region RIPEMD160

#if NETSTD
#else
            else if (eha == EnumHashAlgorithm.RIPEMD160Managed)
            {
                // RIPEMD160Managedサービスプロバイダ
                ha = RIPEMD160Managed.Create(); // devps(1703)
            }
#endif

            #endregion

            #region SHA1
            else if (eha == EnumHashAlgorithm.SHA1CryptoServiceProvider)
            {
                // SHA1CryptoServiceProviderサービスプロバイダ
                ha = SHA1CryptoServiceProvider.Create(); // devps(1703)
            }

#if NETSTD
#else
            else if (eha == EnumHashAlgorithm.SHA1CryptographyNextGeneration)
            {
                // SHA1Cngサービスプロバイダ
                ha = SHA1Cng.Create(); // devps(1703)
            }
#endif

            else if (eha == EnumHashAlgorithm.SHA1Managed)
            {
                // SHA1Managedサービスプロバイダ
                ha = SHA1Managed.Create(); // devps(1703)
            }
            #endregion

            #region SHA256
            else if (eha == EnumHashAlgorithm.SHA256CryptoServiceProvider)
            {
                // SHA256CryptoServiceProviderサービスプロバイダ
                ha = SHA256CryptoServiceProvider.Create(); // devps(1703)
            }

#if NETSTD
#else
            else if (eha == EnumHashAlgorithm.SHA256CryptographyNextGeneration)
            {
                // SHA256Cngサービスプロバイダ
                ha = SHA256Cng.Create(); // devps(1703)
            }
#endif

            else if (eha == EnumHashAlgorithm.SHA256Managed)
            {
                // SHA256Managedサービスプロバイダ
                ha = SHA256Managed.Create(); // devps(1703)
            }
            #endregion

            #region SHA384
            else if (eha == EnumHashAlgorithm.SHA384CryptographyNextGeneration)
            {
                // SHA384CryptoServiceProviderサービスプロバイダ
                ha = SHA384CryptoServiceProvider.Create(); // devps(1703)
            }

#if NETSTD
#else
            else if (eha == EnumHashAlgorithm.SHA384CryptographyNextGeneration)
            {
                // SHA384Cngサービスプロバイダ
                ha = SHA384Cng.Create(); // devps(1703)
            }
#endif

            else if (eha == EnumHashAlgorithm.SHA384Managed)
            {
                // SHA384Managedサービスプロバイダ
                ha = SHA384Managed.Create(); // devps(1703)
            }
            #endregion

            #region SHA512
            else if (eha == EnumHashAlgorithm.SHA512CryptoServiceProvider)
            {
                // SHA512CryptoServiceProviderサービスプロバイダ
                ha = SHA512CryptoServiceProvider.Create(); // devps(1703)
            }

#if NETSTD
#else
            else if (eha == EnumHashAlgorithm.SHA512CryptographyNextGeneration)
            {
                // SHA512Cngサービスプロバイダ
                ha = SHA512Cng.Create(); // devps(1703)
            }
#endif

            else if (eha == EnumHashAlgorithm.SHA512Managed)
            {
                // SHA512Managedサービスプロバイダ
                ha = SHA512Managed.Create(); // devps(1703)
            }
            #endregion

            else
            {
                // 既定の暗号化サービスプロバイダ
                ha = HashAlgorithm.Create(); // devps(1703)
            }

            return ha;
        }

        /// <summary>ハッシュ（キー付き）サービスプロバイダの生成</summary>
        /// <param name="ekha">ハッシュ（キー付き）サービスプロバイダの列挙型</param>
        /// <param name="key">キー</param>
        /// <returns>ハッシュ（キー付き）サービスプロバイダ</returns>
        public static KeyedHashAlgorithm CreateKeyedHashAlgorithmServiceProvider(EnumKeyedHashAlgorithm ekha, byte[] key)
        {
            // ハッシュ（キー付き）サービスプロバイダ
            KeyedHashAlgorithm kha = null;

            // HMACSHA1.Create(); だと、全部、HMACSHA1になってしまう現象があったので、
            // 全部、= new HMACSHA1(key); のスタイルに変更した。

            if (ekha == EnumKeyedHashAlgorithm.Default)
            {
                // 既定の暗号化サービスプロバイダ
                kha = new HMACSHA1(key); // devps(1703)
            }

            else if (ekha == EnumKeyedHashAlgorithm.HMACSHA1)
            {
                // HMACSHA1サービスプロバイダ
                kha = new HMACSHA1(key); // devps(1703)
            }
            // -- ▼追加▼ --
            else if (ekha == EnumKeyedHashAlgorithm.HMACMD5)
            {
                // HMACMD5サービスプロバイダ
                kha = new HMACMD5(key);
            }

#if NETSTD
#else
            else if (ekha == EnumKeyedHashAlgorithm.HMACRIPEMD160)
            {
                // HMACRIPEMD160サービスプロバイダ
                kha = new HMACRIPEMD160(key);
            }
#endif

            else if (ekha == EnumKeyedHashAlgorithm.HMACSHA256)
            {
                // HMACSHA256サービスプロバイダ
                kha = new HMACSHA256(key);
            }
            else if (ekha == EnumKeyedHashAlgorithm.HMACSHA384)
            {
                // HMACSHA384サービスプロバイダ
                kha = new HMACSHA384(key);
            }
            else if (ekha == EnumKeyedHashAlgorithm.HMACSHA512)
            {
                // HMACSHA512サービスプロバイダ
                kha = new HMACSHA512(key);
            }
            // -- ▲追加▲ --

#if NETSTD
#else
            else if (ekha == EnumKeyedHashAlgorithm.MACTripleDES)
            {
                // MACTripleDESサービスプロバイダ
                kha = new MACTripleDES(key); // devps(1703)
            }
#endif

            else
            {
                throw new ArgumentException(
                    PublicExceptionMessage.ARGUMENT_INCORRECT, "EnumKeyedHashAlgorithm ekha");
            }

            return kha;
        }

        #endregion
    }
}

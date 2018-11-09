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
//* クラス名        ：RsaAndDsaCmnFunc
//* クラス日本語名  ：RsaAndDsaCmnFuncクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/12/25  西野 大介         新規作成
//*  2018/11/09  西野 大介         RSAOpenSsl & HashAlgorithmName対応
//**********************************************************************************

using System;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// AsymmetricAlgorithmCmnFuncクラス
    /// - RSACryptoServiceProvider:
    ///   MD5, SHA1, SHA256, SHA384, SHA512
    /// - DSACryptoServiceProvider:SHA1
    /// だけ、サポート。
    /// </summary>
    public class AsymmetricAlgorithmCmnFunc
    {
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
            if (eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_MD5
                || eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA1
                || eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA256
                || eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA384
                || eaa == EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA512)
            {
                // RSACryptoServiceProviderサービスプロバイダ
                aa = new RSACryptoServiceProvider();

                switch (eaa)
                {
                    case EnumDigitalSignAlgorithm.RSACryptoServiceProvider_MD5:
                        ha = MD5.Create();
                        break;
                    case EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA1:
                        ha = SHA1.Create();
                        break;
                    case EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA256:
                        ha = SHA256.Create();
                        break;
                    case EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA384:
                        ha = SHA384.Create();
                        break;
                    case EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA512:
                        ha = SHA512.Create();
                        break;
                }
            }
#if NETSTD
            else if (eaa == EnumDigitalSignAlgorithm.RSAOpenSsl_MD5
                || eaa == EnumDigitalSignAlgorithm.RSAOpenSsl_SHA1
                || eaa == EnumDigitalSignAlgorithm.RSAOpenSsl_SHA256
                || eaa == EnumDigitalSignAlgorithm.RSAOpenSsl_SHA384
                || eaa == EnumDigitalSignAlgorithm.RSAOpenSsl_SHA512)
            {
                // RSAOpenSslサービスプロバイダ
                aa = new RSAOpenSsl();

                switch (eaa)
                {
                    case EnumDigitalSignAlgorithm.RSAOpenSsl_MD5:
                        ha = MD5.Create();
                        break;
                    case EnumDigitalSignAlgorithm.RSAOpenSsl_SHA1:
                        ha = SHA1.Create();
                        break;
                    case EnumDigitalSignAlgorithm.RSAOpenSsl_SHA256:
                        ha = SHA256.Create();
                        break;
                    case EnumDigitalSignAlgorithm.RSAOpenSsl_SHA384:
                        ha = SHA384.Create();
                        break;
                    case EnumDigitalSignAlgorithm.RSAOpenSsl_SHA512:
                        ha = SHA512.Create();
                        break;
                }
            }
#endif
            else if (eaa == EnumDigitalSignAlgorithm.DSACryptoServiceProvider_SHA1)
            {
                // DSACryptoServiceProvider
                aa = new DSACryptoServiceProvider();
                ha = SHA1.Create();
            }
#if NETSTD
            else if (eaa == EnumDigitalSignAlgorithm.DSAOpenSsl_SHA1)
            {
                // DSAOpenSslサービスプロバイダ
                aa = new DSAOpenSsl();
                ha = SHA1.Create();
            }
#endif
            else if (
                eaa == EnumDigitalSignAlgorithm.ECDsaCng_P256
                || eaa == EnumDigitalSignAlgorithm.ECDsaCng_P384
                || eaa == EnumDigitalSignAlgorithm.ECDsaCng_P521)
            {
                // ECDsaCngはCngKeyが土台で、
                // ECDsaCng生成後にオプションとして設定するのではなく
                // CngKeyの生成時にCngAlgorithmの指定が必要であるもよう。
                CngAlgorithm cngAlgorithm = null;
                switch (eaa)
                {
                    case EnumDigitalSignAlgorithm.ECDsaCng_P256:
                        cngAlgorithm = CngAlgorithm.ECDsaP256;
                        break;
                    case EnumDigitalSignAlgorithm.ECDsaCng_P384:
                        cngAlgorithm = CngAlgorithm.ECDsaP384;
                        break;
                    case EnumDigitalSignAlgorithm.ECDsaCng_P521:
                        cngAlgorithm = CngAlgorithm.ECDsaP521;
                        break;
                }
                aa = new ECDsaCng(CngKey.Create(cngAlgorithm));
                ha = null; // ハッシュ無し
            }
            else
            {
                throw new ArgumentException(
                    PublicExceptionMessage.ARGUMENT_INCORRECT,
                    "EnumDigitalSignAlgorithm parameter is incorrect.");
            }
        }
    }
}

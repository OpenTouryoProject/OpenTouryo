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
//* クラス名        ：JWS_ES384_Param
//* クラス日本語名  ：ParamによるJWS ES384生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/25  西野 大介         新規作成（分割
//**********************************************************************************

using System;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Security;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>ParamによるJWS ES384生成クラス</summary>
    public class JWS_ES384_Param : JWS_ES384
    {
        #region mem & prop & constructor

        /// <summary>DigitalSignECDsaCng</summary>
        private DigitalSignECDsaCng DigitalSignECDsaCng { get; set; }

#if NETSTD
        /// <summary>DigitalSignECDsaOpenSsl</summary>
        private DigitalSignECDsaOpenSsl DigitalSignECDsaOpenSsl { get; set; }
#endif

        /// <summary>OperatingSystem</summary>
        private OperatingSystem os = Environment.OSVersion;

        /// <summary>秘密鍵のECParameters</summary>
        public ECParameters ECDsaPrivateParameters
        {
            get
            {
                // Cng or OpenSsl
                if (os.Platform == PlatformID.Win32NT)
                {
                    return ((ECDsa)this.DigitalSignECDsaCng.AsymmetricAlgorithm).ExportParameters(true);
                }
                else
                {
#if NETSTD
                    return ((ECDsa)this.DigitalSignECDsaOpenSsl.AsymmetricAlgorithm).ExportParameters(true);
#else
                    throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
#endif
                }
            }
        }

        /// <summary>公開鍵のECParameters</summary>
        public ECParameters ECDsaPublicParameters
        {
            get
            {
                // Cng or OpenSsl
                if (os.Platform == PlatformID.Win32NT)
                {
                    return ((ECDsa)this.DigitalSignECDsaCng.AsymmetricAlgorithm).ExportParameters(false);
                }
                else
                {
#if NETSTD
                    return ((ECDsa)this.DigitalSignECDsaOpenSsl.AsymmetricAlgorithm).ExportParameters(false);
#else
                    throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
#endif
                }
            }
        }

        /// <summary>Constructor</summary>
        public JWS_ES384_Param()
        {
            // Cng or OpenSsl
            if (os.Platform == PlatformID.Win32NT)
            {
                this.DigitalSignECDsaCng = new DigitalSignECDsaCng(JWS_ES384.DigitalSignAlgorithm);
            }
            else
            {
#if NETSTD
                this.DigitalSignECDsaOpenSsl = new DigitalSignECDsaOpenSsl(
                    JWS_ES384.DigitalSignAlgorithm, SHA384.Create());
                //HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.SHA384_M));
#else
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
#endif
            }
        }

        /// <summary>Constructor</summary>
        /// <param name="param">ECParameters</param>
        /// <param name="isPrivate">bool</param>
        public JWS_ES384_Param(ECParameters param, bool isPrivate)
        {
            // Cng or OpenSsl
#if NETSTD
            if (OperatingSystem.IsWindows())
#else
            if (os.Platform == PlatformID.Win32NT)
#endif
            {
                this.DigitalSignECDsaCng = new DigitalSignECDsaCng(param, isPrivate);
            }
#if NETSTD
            else if (OperatingSystem.IsLinux())
#else
            else if (os.Platform == PlatformID.Unix)
#endif
            {
#if NETSTD
                this.DigitalSignECDsaOpenSsl = new DigitalSignECDsaOpenSsl(param, SHA384.Create());
#else
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
#endif
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }
        }

        #endregion

        #region ES384署名・検証

        /// <summary>Create2</summary>
        /// <param name="data">byte[]</param>
        /// <returns>byte[]</returns>
        protected override byte[] Create2(byte[] data)
        {
#if NETSTD
            if (OperatingSystem.IsWindows())
#else
            if (os.Platform == PlatformID.Win32NT)
#endif
            {
                return this.DigitalSignECDsaCng.Sign(data);
            }
#if NETSTD
            else if (OperatingSystem.IsLinux())
#else
            else if (os.Platform == PlatformID.Unix)
#endif
            {
#if NETSTD
                return this.DigitalSignECDsaOpenSsl.Sign(data);
#else
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
#endif
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }
        }

        /// <summary>Verify2</summary>
        /// <param name="data">byte[]</param>
        /// <param name="sign">byte[]</param>
        /// <returns>bool</returns>
        protected override bool Verify2(byte[] data, byte[] sign)
        {
            // Cng or OpenSsl
#if NETSTD
            if (OperatingSystem.IsWindows())
#else
            if (os.Platform == PlatformID.Win32NT)
#endif
            {
                return this.DigitalSignECDsaCng.Verify(data, sign);
            }
#if NETSTD
            else if (OperatingSystem.IsLinux())
#else
            else if (os.Platform == PlatformID.Unix)
#endif
            {
#if NETSTD
                return this.DigitalSignECDsaOpenSsl.Verify(data, sign);
#else
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
#endif
            }
            else
            {
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
            }
        }

        #endregion
    }
}

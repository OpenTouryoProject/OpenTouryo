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
//* クラス名        ：JWS_ES256_Param
//* クラス日本語名  ：ParamによるJWS ES256生成クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/01/28  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Diagnostics;
using System.Security.Cryptography;

using Newtonsoft.Json;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>ParamによるJWS ES256生成クラス</summary>
    public class JWS_ES256_Param : JWS_ES256
    {
        #region mem & prop & constructor

        /// <summary>DigitalSignECDsaCng</summary>
        private DigitalSignECDsaCng DigitalSignECDsaCng { get; set; }

#if NETSTD
        /// <summary>DigitalSignECDsaOpenSsl</summary>
        private DigitalSignECDsaOpenSsl DigitalSignECDsaOpenSsl { get; set; }
#endif

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
        public JWS_ES256_Param()
        {
            // Cng or OpenSsl
            if (os.Platform == PlatformID.Win32NT)
            {
                this.DigitalSignECDsaCng = new DigitalSignECDsaCng(JWS_ES256.DigitalSignAlgorithm);
            }
            else
            {
#if NETSTD
                this.DigitalSignECDsaOpenSsl = new DigitalSignECDsaOpenSsl(JWS_ES256.DigitalSignAlgorithm);
#else
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
#endif
            }
        }

        /// <summary>Constructor</summary>
        /// <param name="param">ECParameters</param>
        /// <param name="isPrivate">bool</param>
        public JWS_ES256_Param(ECParameters param, bool isPrivate)
        {
            // Cng or OpenSsl
            if (os.Platform == PlatformID.Win32NT)
            {
                this.DigitalSignECDsaCng = new DigitalSignECDsaCng(param, isPrivate);
            }
            else
            {
#if NETSTD
                this.DigitalSignECDsaOpenSsl = new DigitalSignECDsaOpenSsl(param,
                    HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.SHA256_M));
#else
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
#endif
            }
        }

        #endregion

        #region ES256署名・検証

        /// <summary>ES256のJWS生成メソッド</summary>
        /// <param name="payloadJson">ペイロード部のJson文字列</param>
        /// <returns>JWSの文字列表現</returns>
        public override string Create(string payloadJson)
        {
            // ヘッダー
            string headerJson = JsonConvert.SerializeObject(
                this.JWSHeader,
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore
                });

            byte[] headerBytes = CustomEncode.StringToByte(headerJson, CustomEncode.UTF_8);
            string headerEncoded = CustomEncode.ToBase64UrlString(headerBytes);

            // ペイロード
            byte[] payloadBytes = CustomEncode.StringToByte(payloadJson, CustomEncode.UTF_8);
            string payloadEncoded = CustomEncode.ToBase64UrlString(payloadBytes);

            // 署名
            byte[] temp = CustomEncode.StringToByte(headerEncoded + "." + payloadEncoded, CustomEncode.UTF_8);

            // Cng or OpenSsl
            string signEncoded = "";
            if (os.Platform == PlatformID.Win32NT)
            {
                signEncoded = CustomEncode.ToBase64UrlString(this.DigitalSignECDsaCng.Sign(temp));
            }
            else
            {
#if NETSTD
                signEncoded = CustomEncode.ToBase64UrlString(this.DigitalSignECDsaOpenSsl.Sign(temp));
#else
                throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
#endif
            }

            // return JWS by RS256
            return headerEncoded + "." + payloadEncoded + "." + signEncoded;
        }

        /// <summary>RS256のJWS検証メソッド</summary>
        /// <param name="jwtString">JWSの文字列表現</param>
        /// <returns>署名の検証結果</returns>
        public override bool Verify(string jwtString)
        {
            string[] temp = jwtString.Split('.');

            // 検証
            JWS_Header headerObject = (JWS_Header)JsonConvert.DeserializeObject(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8), typeof(JWS_Header));

            if (headerObject.alg.ToUpper() == JwtConst.RS256 && headerObject.typ.ToUpper() == JwtConst.JWT)
            {
                byte[] data = CustomEncode.StringToByte(temp[0] + "." + temp[1], CustomEncode.UTF_8);
                byte[] sign = CustomEncode.FromBase64UrlString(temp[2]);

                // Cng or OpenSsl
                if (os.Platform == PlatformID.Win32NT)
                {
                    return this.DigitalSignECDsaCng.Verify(data, sign);
                }
                else
                {
#if NETSTD
                    return this.DigitalSignECDsaOpenSsl.Verify(data, sign);
#else
                    throw new NotImplementedException(PublicExceptionMessage.NOT_IMPLEMENTED);
#endif
                }
                
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}

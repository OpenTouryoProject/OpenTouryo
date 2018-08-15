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
//* クラス名        ：RS256_KeyConverter
//* クラス日本語名  ：RS256_KeyConverterクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/12/26  西野 大介         新規作成
//*  2018/08/15  西野 大介         エンハンス
//**********************************************************************************

using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// RSA関係のカギ変換処理を実装する。
    /// X.509 or Xml 鍵 → RSAParameters（公開鍵）→ Xml or Jwk 公開鍵
    /// </summary>
    public class RS256_KeyConverter
    {
        #region X.509 or Xml 鍵 → Xml or Jwk 公開鍵

        #region X.509 鍵 →

        #region *.cer

        /// <summary>
        /// X509CerToXmlPublicKey
        /// X.509鍵(*.cer)からXml公開鍵へ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <returns>Xml公開鍵</returns>
        public static string X509CerToXmlPublicKey(string certificateFilePath)
        {
            return RS256_KeyConverter.ParamToXmlPublicKey(
                RS256_KeyConverter.X509CerToProvider(certificateFilePath).ExportParameters(false));
        }

        /// <summary>
        /// X509CerToJwkPublicKey
        /// X.509鍵(*.cer)からJwk公開鍵へ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <returns>Jwk公開鍵</returns>
        public static string X509CerToJwkPublicKey(string certificateFilePath)
        {
            return RS256_KeyConverter.X509CerToJwkPublicKey(certificateFilePath, null);
        }

        /// <summary>
        /// X509CerToJwkPublicKey
        /// X.509鍵(*.cer)からJwk公開鍵へ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public static string X509CerToJwkPublicKey(string certificateFilePath, JsonSerializerSettings settings)
        {
            return RS256_KeyConverter.ParamToJwkPublicKey(
                RS256_KeyConverter.X509CerToProvider(certificateFilePath).ExportParameters(false), settings);
        }

        #endregion

        #region *.pfx

        /// <summary>
        /// X509CerToXmlPublicKey
        /// X.509鍵(*.pfx)からXml公開鍵へ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.pfx)</param>
        /// <param name="password">string</param>
        /// <returns>Xml公開鍵</returns>
        public static string X509PfxToXmlPublicKey(string certificateFilePath, string password)
        {
            return RS256_KeyConverter.ParamToXmlPublicKey(
                RS256_KeyConverter.X509PfxToProvider(certificateFilePath, password).ExportParameters(false));
        }

        /// <summary>
        /// X509CerToJwkPublicKey
        /// X.509鍵(*.pfx)からJwk公開鍵へ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.pfx)</param>
        /// <param name="password">string</param>
        /// <returns>Jwk公開鍵</returns>
        public static string X509PfxToJwkPublicKey(string certificateFilePath, string password)
        {
            return RS256_KeyConverter.X509PfxToJwkPublicKey(certificateFilePath, password, null);
        }

        /// <summary>
        /// X509CerToJwkPublicKey
        /// X.509鍵(*.pfx)からJwk公開鍵へ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.pfx)</param>
        /// <param name="password">string</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public static string X509PfxToJwkPublicKey(string certificateFilePath, string password, JsonSerializerSettings settings)
        {
            return RS256_KeyConverter.ParamToJwkPublicKey(
                RS256_KeyConverter.X509PfxToProvider(certificateFilePath, password).ExportParameters(false), settings);
        }

        #endregion

        #endregion

        #region Xml 鍵 →

        /// <summary>
        /// XmlToXmlPublicKey
        /// Xml鍵からXml公開鍵へ変換
        /// </summary>
        /// <param name="xmlKey">Xml鍵</param>
        /// <returns>Xml公開鍵</returns>
        public static string XmlToXmlPublicKey(string xmlKey)
        {
            return RS256_KeyConverter.ParamToXmlPublicKey(
                RS256_KeyConverter.XmlToProvider(xmlKey).ExportParameters(false));
        }

        /// <summary>
        /// XmlToJwkPublicKey
        /// Xml鍵からJwk公開鍵へ変換
        /// </summary>
        /// <param name="xmlKey">Xml鍵</param>
        /// <returns>Jwk公開鍵</returns>
        public static string XmlToJwkPublicKey(string xmlKey)
        {
            return RS256_KeyConverter.XmlToJwkPublicKey(xmlKey, null);
        }

        /// <summary>
        /// XmlToJwkPublicKey
        /// Xml鍵からJwk公開鍵へ変換
        /// </summary>
        /// <param name="xmlKey">Xml鍵</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public static string XmlToJwkPublicKey(string xmlKey, JsonSerializerSettings settings)
        {
            return RS256_KeyConverter.ParamToJwkPublicKey(
                RS256_KeyConverter.XmlToProvider(xmlKey).ExportParameters(false), settings);
        }

        #endregion

        #endregion

        #region ToProvider（X.509 or Xml 鍵 → RSAParameters（公開鍵）

        /// <summary>
        /// XmlToProvider
        /// Xml鍵からRsaProvider（公開鍵）へ変換
        /// </summary>
        /// <param name="xmlKey">Xml鍵</param>
        /// <returns>RSACryptoServiceProvider</returns>
        public static RSACryptoServiceProvider XmlToProvider(string xmlKey)
        {
            DigitalSignXML dsXML = null;

            // Public
            dsXML = new DigitalSignXML(
                EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA256, xmlKey);
            dsXML = new DigitalSignXML(
                EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA256, dsXML.XMLPublicKey); 

            return (RSACryptoServiceProvider)dsXML.AsymmetricAlgorithm;
        }

        /// <summary>
        /// JwkToProvider
        /// Jwk鍵からRSAProvider（公開鍵）へ変換
        /// </summary>
        /// <param name="jwkKey">jwkKey</param>
        /// <returns>
        /// RSACryptoServiceProvider
        ///   rsaCryptoServiceProvider.VerifyData(
        ///     data, signatureBytes,
        ///     HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        /// </returns>
        public static RSACryptoServiceProvider JwkToProvider(string jwkKey)
        {
            JObject jwk = JObject.Parse(jwkKey);

            if (jwk["alg"].ToString().ToLower() == "rs256")
            {
                // RSAParameters
                // FromBase64Stringだとエラーになる。
                RSAParameters rsaParameters = new RSAParameters()
                {
                    // Public
                    Modulus = CustomEncode.FromBase64UrlString((string)jwk["n"]),
                    Exponent = CustomEncode.FromBase64UrlString((string)jwk["e"]),
                };

                RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider();
                rsaCryptoServiceProvider.ImportParameters(rsaParameters);

                return rsaCryptoServiceProvider;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// X509CerToProvider
        /// X.509鍵(*.cer)からRsaProviderへ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <returns>RSACryptoServiceProvider</returns>
        public static RSACryptoServiceProvider X509CerToProvider(string certificateFilePath)
        {
            DigitalSignX509 dsX509 = new DigitalSignX509(
                certificateFilePath, "", "SHA256", X509KeyStorageFlags.DefaultKeySet);

            if (dsX509.X509Certificate.PrivateKey == null)
            {
                AsymmetricAlgorithm aa = dsX509.X509Certificate.PublicKey.Key; // Public
                if (aa is RSACryptoServiceProvider)
                {
                    return (RSACryptoServiceProvider)aa;
                }
                else { }
            }
            else { }

            return null;
        }

        /// <summary>
        /// X509CerToProvider
        /// X.509鍵(*.pfx)からRsaProviderへ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.pfx)</param>
        /// <param name="password">string</param>
        /// <returns>RSACryptoServiceProvider</returns>
        public static RSACryptoServiceProvider X509PfxToProvider(string certificateFilePath, string password)
        {
            DigitalSignX509 dsX509 = new DigitalSignX509(
                certificateFilePath, password, "SHA256", X509KeyStorageFlags.DefaultKeySet);

            AsymmetricAlgorithm aa = dsX509.X509Certificate.PublicKey.Key; // Public
            if (aa is RSACryptoServiceProvider)
            {
                return (RSACryptoServiceProvider)aa;
            }
            else { }

            return null;
        }

        #endregion

        #region ToPublicKey（RSAParameters → Xml or Jwk 公開鍵

        /// <summary>
        /// ParamToXmlPublicKey
        /// RSAParametersからXml公開鍵へ変換
        /// </summary>
        /// <param name="param">RSAParameters</param>
        /// <returns>XmlPublicKey</returns>
        public static string ParamToXmlPublicKey(RSAParameters param)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider();
            rsaCryptoServiceProvider.ImportParameters(param);
            return rsaCryptoServiceProvider.ToXmlString(false); // Public
        }

        /// <summary>
        /// ParamToJwkPublicKey
        /// RSAParametersからJwk公開鍵へ変換
        /// </summary>
        /// <param name="param">RSAParameters</param>
        /// <returns>JwkPublicKey</returns>
        public static string ParamToJwkPublicKey(RSAParameters param)
        {
            return RS256_KeyConverter.ParamToJwkPublicKey(param, null);
        }

        /// <summary>
        /// ParamToJwkPublicKey
        /// RSAParametersからJwk公開鍵へ変換
        /// </summary>
        /// <param name="param">RSAParameters</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>JwkPublicKey</returns>
        public static string ParamToJwkPublicKey(RSAParameters param, JsonSerializerSettings settings)
        {
            /*
             * FIDO2.0 の Web Authentication API が生成する公開鍵の例
              {
                "alg": "RS256",
                "e": "AQAB",
                "ext": false,
                "kty": "RSA",
                "n": "・・・"
              }
            */

            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic["kty"] = "RSA"; // 必須
            dic["alg"] = "RS256";

            // Public
            dic["n"] = CustomEncode.ToBase64UrlString(param.Modulus);
            dic["e"] = CustomEncode.ToBase64UrlString(param.Exponent); //"AQAB";

            // kid : https://openid-foundation-japan.github.io/rfc7638.ja.html#Example
            dic["kid"] = CustomEncode.ToBase64UrlString(
                GetHash.GetHashBytes(
                    CustomEncode.StringToByte(
                        JsonConvert.SerializeObject(new
                        {
                            e = dic["e"],
                            kty = dic["kty"],
                            n = dic["n"]
                        }),
                        CustomEncode.UTF_8),
                    EnumHashAlgorithm.SHA256Managed));

            //dic["ext"] = "false"; // 定義をRFC上に発見できない。

            if (settings == null)
            {
                return JsonConvert.SerializeObject(dic);
            }
            else
            {
                return JsonConvert.SerializeObject(dic, settings);
            }            
        }
        
        #endregion
    }
}

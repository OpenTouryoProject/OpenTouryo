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
//* クラス名        ：RsaPublicKeyConverter
//* クラス日本語名  ：RsaPublicKeyConverterクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/12/26  西野 大介         新規作成
//*  2018/08/15  西野 大介         エンハンス
//*  2018/11/09  西野 大介         RSAOpenSsl、DSAOpenSsl、HashAlgorithmName対応
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
    /// 基本的に変換先は公開鍵。変換元は秘密鍵も扱える。
    /// X.509 or Xml 鍵 → RSAParameters（公開鍵）⇔ Xml or Jwk 公開鍵
    /// </summary>
    public class RsaPublicKeyConverter
    {
        #region X.509 or Xml 鍵 → Xml or Jwk 公開鍵

        #region X.509 鍵 → Xml or Jwk 公開鍵

        #region *.cer

        #region Xml
        /// <summary>
        /// X509CerToXml
        /// X.509鍵(*.cer)からXml公開鍵へ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <returns>Xml公開鍵</returns>
        public static string X509CerToXml(string certificateFilePath)
        {
            return RsaPublicKeyConverter.ParamToXml( // *.cer is PublicKey -> ExportParameters(false)
                RsaPublicKeyConverter.X509CerToProvider(certificateFilePath).ExportParameters(false));
        }
        #endregion

        #region Jwk
        /// <summary>
        /// X509CerToJwk
        /// X.509鍵(*.cer)からJwk公開鍵へ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <returns>Jwk公開鍵</returns>
        public static string X509CerToJwk(string certificateFilePath)
        {
            return RsaPublicKeyConverter.X509CerToJwk(certificateFilePath, null);
        }

        /// <summary>
        /// X509CerToJwk
        /// X.509鍵(*.cer)からJwk公開鍵へ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public static string X509CerToJwk(string certificateFilePath, JsonSerializerSettings settings)
        {
            return RsaPublicKeyConverter.ParamToJwk( // *.cer is PublicKey -> ExportParameters(false)
                RsaPublicKeyConverter.X509CerToProvider(certificateFilePath).ExportParameters(false), settings);
        }
        #endregion

        #endregion

        #region *.pfx

        /// <summary>
        /// X509CerToXmlPublicKey
        /// X.509鍵(*.pfx)からXml公開鍵へ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.pfx)</param>
        /// <param name="password">string</param>
        /// <returns>Xml公開鍵</returns>
        public static string X509PfxToXml(string certificateFilePath, string password)
        {
            return RsaPublicKeyConverter.ParamToXml( // *.cer is PublicKey -> ExportParameters(false)
                RsaPublicKeyConverter.X509PfxToProvider(certificateFilePath, password).ExportParameters(false));
        }

        /// <summary>
        /// X509CerToJwk
        /// X.509鍵(*.pfx)からJwk公開鍵へ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.pfx)</param>
        /// <param name="password">string</param>
        /// <returns>Jwk公開鍵</returns>
        public static string X509PfxToJwk(string certificateFilePath, string password)
        {
            return RsaPublicKeyConverter.X509PfxToJwk(certificateFilePath, password, null);
        }

        /// <summary>
        /// X509CerToJwk
        /// X.509鍵(*.pfx)からJwk公開鍵へ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.pfx)</param>
        /// <param name="password">string</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public static string X509PfxToJwk(string certificateFilePath, string password, JsonSerializerSettings settings)
        {
            return RsaPublicKeyConverter.ParamToJwk( // *.cer is PublicKey -> ExportParameters(false)
                RsaPublicKeyConverter.X509PfxToProvider(certificateFilePath, password).ExportParameters(false), settings);
        }

        #endregion

        #endregion

        #region Xml 鍵 → Xml or Jwk 公開鍵

        /// <summary>
        /// XmlToXml
        /// Xml鍵からXml公開鍵へ変換
        /// </summary>
        /// <param name="xmlKey">Xml鍵</param>
        /// <returns>Xml公開鍵</returns>
        public static string XmlToXml(string xmlKey)
        {
            return RsaPublicKeyConverter.ParamToXml( // PublicKey -> ExportParameters(false)
                RsaPublicKeyConverter.XmlToProvider(xmlKey).ExportParameters(false));
        }

        /// <summary>
        /// XmlToJwk
        /// Xml鍵からJwk公開鍵へ変換
        /// </summary>
        /// <param name="xmlKey">Xml鍵</param>
        /// <returns>Jwk公開鍵</returns>
        public static string XmlToJwk(string xmlKey)
        {
            return RsaPublicKeyConverter.XmlToJwk(xmlKey, null);
        }

        /// <summary>
        /// XmlToJwkPublicKey
        /// Xml鍵からJwk公開鍵へ変換
        /// </summary>
        /// <param name="xmlKey">Xml鍵</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public static string XmlToJwk(string xmlKey, JsonSerializerSettings settings)
        {
            return RsaPublicKeyConverter.ParamToJwk( // PublicKey -> ExportParameters(false)
                RsaPublicKeyConverter.XmlToProvider(xmlKey).ExportParameters(false), settings);
        }

        #endregion

        #endregion

        #region X.509 or Xml 鍵 → RSA（公開鍵）

        /// <summary>
        /// XmlToProvider
        /// Xml鍵からRsaProvider（公開鍵）へ変換
        /// </summary>
        /// <param name="xmlKey">Xml鍵</param>
        /// <returns>RSA</returns>
        public static RSA XmlToProvider(string xmlKey)
        {
            DigitalSignXML dsXML = null;

            // Public
            dsXML = new DigitalSignXML(xmlKey, JWS_RS256.DigitalSignAlgorithm);
            dsXML = new DigitalSignXML(dsXML.PublicKey, JWS_RS256.DigitalSignAlgorithm); 

            return (RSA)dsXML.AsymmetricAlgorithm;
        }

        /// <summary>
        /// X509CerToProvider
        /// X.509鍵(*.cer)からRsaProviderへ変換
        /// </summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <returns>RSA</returns>
        public static RSA X509CerToProvider(string certificateFilePath)
        {
            // X509KeyStorageFlagsについて
            // PublicKey を取り出すため Exportableは不要
            // ASP.NET（サーバ）からの実行を想定しないため、MachineKeySetは不要
            DigitalSignX509 dsX509 = new DigitalSignX509(
                certificateFilePath, "", CryptoConst.SHA256, X509KeyStorageFlags.DefaultKeySet);

            if (dsX509.X509Certificate.PrivateKey == null)
            {
                AsymmetricAlgorithm aa = dsX509.X509Certificate.PublicKey.Key; // Public
                if (aa is RSA)
                {
                    return (RSA)aa;
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
        /// <returns>RSA</returns>
        public static RSA X509PfxToProvider(string certificateFilePath, string password)
        {
            // X509KeyStorageFlagsについて
            // PublicKey を取り出すため Exportableは不要
            // ASP.NET（サーバ）からの実行を想定しないため、MachineKeySetは不要
            DigitalSignX509 dsX509 = new DigitalSignX509(
                certificateFilePath, password, CryptoConst.SHA256, X509KeyStorageFlags.DefaultKeySet);

            AsymmetricAlgorithm aa = dsX509.X509Certificate.PublicKey.Key; // Public
            if (aa is RSA)
            {
                return (RSA)aa;
            }
            else { }

            return null;
        }

        #endregion

        #region RSA(RSAParameters) ⇔ Xml or Jwk 公開鍵

        #region Xml
        /// <summary>
        /// ParamToXml
        /// RSAParametersからXml公開鍵へ変換
        /// </summary>
        /// <param name="param">RSAParameters</param>
        /// <returns>XmlPublicKey</returns>
        public static string ParamToXml(RSAParameters param)
        {
            RSA rsa = AsymmetricAlgorithmCmnFunc.RsaFactory();
            rsa.ImportParameters(param);
            return rsa.ToXmlString(false); // Public
        }
        #endregion

        #region Jwk

        #region ParamToJwk
        /// <summary>
        /// ParamToJwk
        /// RSAParametersからJwk公開鍵へ変換
        /// </summary>
        /// <param name="param">RSAParameters</param>
        /// <returns>JwkPublicKey</returns>
        public static string ParamToJwk(RSAParameters param)
        {
            return RsaPublicKeyConverter.ParamToJwk(param, null);
        }

        /// <summary>
        /// ParamToJwk
        /// RSAParametersからJwk公開鍵へ変換
        /// </summary>
        /// <param name="param">RSAParameters</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>JwkPublicKey</returns>
        public static string ParamToJwk(RSAParameters param, JsonSerializerSettings settings)
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

            dic[JwtConst.kty] = "RSA"; // 必須
            dic[JwtConst.alg] = JwtConst.RS256;

            // Public
            dic[JwtConst.n] = CustomEncode.ToBase64UrlString(param.Modulus);
            dic[JwtConst.e] = CustomEncode.ToBase64UrlString(param.Exponent); //"AQAB";

            // kid : https://openid-foundation-japan.github.io/rfc7638.ja.html#Example
            dic[JwtConst.kid] = CustomEncode.ToBase64UrlString(
                GetHash.GetHashBytes(
                    CustomEncode.StringToByte(
                        JsonConvert.SerializeObject(new
                        {
                            kty = dic[JwtConst.kty],
                            e = dic[JwtConst.e],
                            n = dic[JwtConst.n]
                        }),
                        CustomEncode.UTF_8),
                    EnumHashAlgorithm.SHA256_M));

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

        #region JwkToProvider
        /// <summary>
        /// JwkToProvider
        /// JwkからRSAProvider（公開鍵）へ変換
        /// </summary>
        /// <param name="jwkString">string</param>
        /// <returns>RSA</returns>
        public static RSA JwkToProvider(string jwkString)
        {
            return RsaPublicKeyConverter.JwkToProvider(
                JsonConvert.DeserializeObject<JObject>(jwkString));
        }

        /// <summary>
        /// JwkToProvider
        /// JwkからRSAProvider（公開鍵）へ変換
        /// </summary>
        /// <param name="jwkObject">JObject</param>
        /// <returns>RSA</returns>
        public static RSA JwkToProvider(JObject jwkObject)
        {
            if (jwkObject[JwtConst.alg].ToString().ToUpper() == JwtConst.RS256)
            {
                // RSAParameters
                // FromBase64Stringだとエラーになる。
                RSAParameters rsaParameters = new RSAParameters()
                {
                    // Public
                    Modulus = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.n]),
                    Exponent = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.e]),
                };

                RSA rsa = AsymmetricAlgorithmCmnFunc.RsaFactory();
                rsa.ImportParameters(rsaParameters);

                return rsa;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #endregion

        #endregion
    }
}

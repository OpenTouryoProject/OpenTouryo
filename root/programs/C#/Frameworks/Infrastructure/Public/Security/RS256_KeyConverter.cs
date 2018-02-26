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
    /// X.509 or XML ⇔ RSAParameters ⇔ JWK
    /// </summary>
    public class RS256_KeyConverter
    {
        #region ToProvider
        
        /// <summary>
        /// XmlToProvider
        /// XML鍵からRsaProviderへ変換
        /// </summary>
        /// <param name="xmlKey">XML鍵</param>
        /// <returns>RSACryptoServiceProvider</returns>
        public static RSACryptoServiceProvider XmlToProvider(string xmlKey)
        {
            DigitalSignXML dsXML = new DigitalSignXML(
                EnumDigitalSignAlgorithm.RSACryptoServiceProvider_SHA256, xmlKey);

            return (RSACryptoServiceProvider)dsXML.AsymmetricAlgorithm;
        }

        /// <summary>
        /// JwkToProvider
        /// Jwk公開鍵からRSAProviderへ変換
        /// </summary>
        /// <param name="jwkPublicKey">jwkPublicKey</param>
        /// <returns>
        /// RSACryptoServiceProvider
        ///   rsaCryptoServiceProvider.VerifyData(
        ///     data, signatureBytes,
        ///     HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        /// </returns>
        public static RSACryptoServiceProvider JwkToProvider(string jwkPublicKey)
        {
            JObject jwk = JObject.Parse(jwkPublicKey);

            if (jwk["alg"].ToString().ToLower() == "rs256")
            {
                // RSAParameters
                // FromBase64Stringだとエラーになる。
                RSAParameters rsaParameters = new RSAParameters()
                {
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
        /// X.509公開鍵(*.cer)からRsaProviderへ変換
        /// </summary>
        /// <param name="certificateFilePath"></param>
        /// <returns>RSACryptoServiceProvider</returns>
        public static RSACryptoServiceProvider X509CerToProvider(string certificateFilePath)
        {
            DigitalSignX509 dsX509 = new DigitalSignX509(
                certificateFilePath, "", "SHA256", X509KeyStorageFlags.DefaultKeySet);

            if (dsX509.X509Certificate.PrivateKey == null)
            {
                AsymmetricAlgorithm aa = dsX509.X509Certificate.PublicKey.Key;
                if (aa is RSACryptoServiceProvider)
                {
                    return (RSACryptoServiceProvider)aa;
                }
                else { }
            }
            else { }

            return null;
        }

        #endregion

        #region ToPublicKey

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
            return rsaCryptoServiceProvider.ToXmlString(false);
        }
        
        /// <summary>
        /// ParamToJwkPublicKey
        /// RSAParametersからJwk公開鍵へ変換
        /// </summary>
        /// <param name="param">RSAParameters</param>
        /// <returns>JwkPublicKey</returns>
        public static string ParamToJwkPublicKey(RSAParameters param)
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
            dic["n"] = CustomEncode.ToBase64UrlString(param.Modulus);
            dic["e"] = CustomEncode.ToBase64UrlString(param.Exponent); //"AQAB";
            //dic["ext"] = "false"; // 定義をRFC上に発見できない。

            return JsonConvert.SerializeObject(dic);
        }
        
        #endregion
    }
}

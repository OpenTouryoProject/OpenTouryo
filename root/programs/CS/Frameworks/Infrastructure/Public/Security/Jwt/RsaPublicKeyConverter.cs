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
//*  2019/01/16  西野 大介         X509KeyStorageFlagsのオプション・名前付き引数対応
//*                                下位がExportableである必要性があった、また、ASP.NET上で実行する可能性もある。
//*  2019/06/25  西野 大介         インスタンス・メソッド化（RS256, 384, 512対応）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>
    /// RSA関係のカギ変換処理を実装する。
    /// 基本的に変換先は公開鍵。変換元は秘密鍵も扱える。
    /// X.509 or Xml 鍵 → RSAProvider(RSAParameters)（公開鍵）⇔ Xml or Jwk 公開鍵
    /// </summary>
    public class RsaPublicKeyConverter
    {
        #region constructor
        /// <summary>アルゴリズム</summary>
        private JWS_HMACSHA.RS RSnnn = JWS_HMACSHA.RS._256;
        /// <summary>アルゴリズム</summary>
        private string HashName = HashNameConst.SHA256;
        /// <summary>アルゴリズム</summary>
        private EnumHashAlgorithm HashAlgorithm = EnumHashAlgorithm.SHA256_M;

        /// <summary>constructor</summary>
        /// <param name="rsNNN">(JWS_HMACSHA.RS</param>
        public RsaPublicKeyConverter(JWS_HMACSHA.RS rsNNN = JWS_HMACSHA.RS._256)
        {
            this.RSnnn = rsNNN;

            switch (this.RSnnn)
            {
                case JWS_HMACSHA.RS._256:
                    this.HashName = HashNameConst.SHA256;
                    this.HashAlgorithm = EnumHashAlgorithm.SHA256_M;
                    break;

                case JWS_HMACSHA.RS._384:
                    this.HashName = HashNameConst.SHA384;
                    this.HashAlgorithm = EnumHashAlgorithm.SHA384_M;
                    break;

                case JWS_HMACSHA.RS._512:
                    this.HashName = HashNameConst.SHA384;
                    this.HashAlgorithm = EnumHashAlgorithm.SHA384_M;
                    break;
            }
        }
        #endregion

        // 保存鍵間のフォーマット変換
        #region X.509 or Xml 鍵 → Xml or Jwk 公開鍵

        #region X.509

        #region *.cer

        #region Xml
        /// <summary>X509CerToXml</summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <param name="flg">X509KeyStorageFlags</param>
        /// <returns>Xml公開鍵</returns>
        public string X509CerToXml(
            string certificateFilePath,
            X509KeyStorageFlags flg = X509KeyStorageFlags.DefaultKeySet)
        {
            return this.ParamToXml( // *.cer is PublicKey -> ExportParameters(false)
                this.X509CerToProvider(certificateFilePath, flg).ExportParameters(false));
        }
        #endregion

        #region Jwk
        /// <summary>X509CerToJwk</summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <param name="flg">X509KeyStorageFlags</param>
        /// <returns>Jwk公開鍵</returns>
        public string X509CerToJwk(
            string certificateFilePath,
            JsonSerializerSettings settings = null,
            X509KeyStorageFlags flg = X509KeyStorageFlags.DefaultKeySet)
        {
            return this.ParamToJwk( // *.cer is PublicKey -> ExportParameters(false)
                this.X509CerToProvider(certificateFilePath, flg).ExportParameters(false), settings);
        }
        #endregion

        #endregion

        #region *.pfx

        #region Xml
        /// <summary>X509PfxToXml</summary>
        /// <param name="certificateFilePath">X.509鍵(*.pfx)</param>
        /// <param name="password">string</param>
        /// <param name="flg">X509KeyStorageFlags</param>
        /// <returns>Xml公開鍵</returns>
        public string X509PfxToXml(
            string certificateFilePath, string password,
            X509KeyStorageFlags flg = X509KeyStorageFlags.DefaultKeySet)
        {
            return this.ParamToXml( // *.cer is PublicKey -> ExportParameters(false)
                this.X509PfxToProvider(certificateFilePath, password, flg).ExportParameters(false));
        }
        #endregion

        #region Jwk
        /// <summary>X509PfxToJwk</summary>
        /// <param name="certificateFilePath">X.509鍵(*.pfx)</param>
        /// <param name="password">string</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <param name="flg">X509KeyStorageFlags</param>
        /// <returns>Jwk公開鍵</returns>
        public string X509PfxToJwk(
            string certificateFilePath, string password,
            JsonSerializerSettings settings = null,
            X509KeyStorageFlags flg = X509KeyStorageFlags.DefaultKeySet)
        {
            return this.ParamToJwk( // *.cer is PublicKey -> ExportParameters(false)
                this.X509PfxToProvider(certificateFilePath, password, flg).ExportParameters(false), settings);
        }
        #endregion

        #endregion

        #endregion

        #region Xml
        /// <summary>XmlToJwk</summary>
        /// <param name="xmlKey">Xml鍵</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public string XmlToJwk(
            string xmlKey,
            JsonSerializerSettings settings = null)
        {
            return this.ParamToJwk( // PublicKey -> ExportParameters(false)
                this.XmlToProvider(xmlKey).ExportParameters(false), settings);
        }

        #endregion

        #endregion

        // 実用鍵への変換
        #region X.509 or Xml 鍵 → RSAProvider(RSAParameters)（公開鍵）

        #region X.509

        #region *.cer
        /// <summary>X509CerToParam</summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <param name="flg">X509KeyStorageFlags</param>
        /// <returns>RSAParameters（公開鍵）</returns>
        public RSAParameters X509CerToParam(
            string certificateFilePath,
            X509KeyStorageFlags flg = X509KeyStorageFlags.DefaultKeySet)
        {
            return this.X509CerToProvider(certificateFilePath, flg).ExportParameters(false);
        }

        /// <summary>X509CerToProvider</summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <param name="flg">X509KeyStorageFlags</param>
        /// <returns>RSA（公開鍵）</returns>
        public RSA X509CerToProvider(
            string certificateFilePath,
            X509KeyStorageFlags flg = X509KeyStorageFlags.DefaultKeySet)
        {
            DigitalSignX509 dsX509 = new DigitalSignX509(
                certificateFilePath, "", this.HashName, flg);

            AsymmetricAlgorithm aa = dsX509.PublicKey; // Public
            if (aa is RSA)
            {
                return (RSA)aa;
            }

            return null;
        }
        #endregion

        #region *.pfx
        /// <summary>X509PfxToParam</summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <param name="password">string</param>
        /// <param name="flg">X509KeyStorageFlags</param>
        /// <returns>RSAParameters（公開鍵）</returns>
        public RSAParameters X509PfxToParam(
            string certificateFilePath, string password,
            X509KeyStorageFlags flg = X509KeyStorageFlags.DefaultKeySet)
        {
            return this.X509PfxToProvider(certificateFilePath, password, flg).ExportParameters(false);
        }

        /// <summary>X509PfxToProvider</summary>
        /// <param name="certificateFilePath">X.509鍵(*.pfx)</param>
        /// <param name="password">string</param>
        /// <param name="flg">X509KeyStorageFlags</param>
        /// <returns>RSA（公開鍵）</returns>
        public RSA X509PfxToProvider(
            string certificateFilePath, string password,
            X509KeyStorageFlags flg = X509KeyStorageFlags.DefaultKeySet)
        {
            DigitalSignX509 dsX509 = new DigitalSignX509(
                certificateFilePath, password, this.HashName, flg);

            AsymmetricAlgorithm aa = dsX509.PublicKey; // Public
            if (aa is RSA)
            {
                return (RSA)aa;
            }

            return null;
        }
        #endregion

        #endregion

        #region Xml
        /// <summary>XmlToParam</summary>
        /// <param name="xmlKey">Xml鍵</param>
        /// <returns>RSAParameters（公開鍵）</returns>
        public RSAParameters XmlToParam(string xmlKey)
        {
            return this.XmlToProvider(xmlKey).ExportParameters(false);
        }

        /// <summary>XmlToProvider</summary>
        /// <param name="xmlKey">Xml鍵</param>
        /// <returns>RSA（公開鍵）</returns>
        public RSA XmlToProvider(string xmlKey)
        {
            DigitalSignXML dsXML = null;

            // Public
            switch (this.RSnnn)
            {
                case JWS_HMACSHA.RS._256:
                    dsXML = new DigitalSignXML(xmlKey, JWS_RS256.DigitalSignAlgorithm);
                    dsXML = new DigitalSignXML(dsXML.PublicKey, JWS_RS256.DigitalSignAlgorithm);
                    break;

                case JWS_HMACSHA.RS._384:
                    dsXML = new DigitalSignXML(xmlKey, JWS_RS384.DigitalSignAlgorithm);
                    dsXML = new DigitalSignXML(dsXML.PublicKey, JWS_RS384.DigitalSignAlgorithm);
                    break;

                case JWS_HMACSHA.RS._512:
                    dsXML = new DigitalSignXML(xmlKey, JWS_RS512.DigitalSignAlgorithm);
                    dsXML = new DigitalSignXML(dsXML.PublicKey, JWS_RS512.DigitalSignAlgorithm);
                    break;
            }

            return (RSA)dsXML.AsymmetricAlgorithm;
        }
        #endregion

        #endregion

        // 実用鍵からの変換
        #region RSAProvider(RSAParameters) ⇔ Xml or Jwk 公開鍵

        #region Xml

        /// <summary>
        /// RSAParametersからXml公開鍵へ変換
        /// </summary>
        /// <param name="param">RSAParameters</param>
        /// <returns>Xml公開鍵</returns>
        public string ParamToXml(RSAParameters param)
        {
            RSA rsa = AsymmetricAlgorithmCmnFunc.RsaFactory();
            rsa.ImportParameters(param);
            return rsa.ToXmlString(false); // Public
        }

        /// <summary>XmlToProvider</summary>
        /// <param name="param">RSAParameters</param>
        /// <returns>RSA（公開鍵）</returns>
        public RSA XmlToProvider(RSAParameters param)
        {
            RSA rsa = AsymmetricAlgorithmCmnFunc.RsaFactory();
            rsa.ImportParameters(param);
            return rsa;
        }

        #endregion

        #region Jwk

        // <参考>
        // JSON Web Key (JWK)
        // https://openid-foundation-japan.github.io/rfc7517.ja.html
        //   を、"kty":"RSA"で検索するとイイ。
        //
        // A.1.  Example Public Keys
        // https://openid-foundation-japan.github.io/rfc7517.ja.html#PublicExample
        // A.2.  Example Private Keys
        // https://openid-foundation-japan.github.io/rfc7517.ja.html#PrivateExample
        // C.1.  Plaintext RSA Private Key
        // https://openid-foundation-japan.github.io/rfc7517.ja.html#example-privkey-plaintext

        // ECCurve support of EccKey.
        // https://github.com/dvsekhvalnov/jose-jwt/issues/105
        // RSAParameters ⇔ Jwk
        // https://github.com/psteniusubi/jose-jwt/blob/master/jose-jwt/jwk/JwkRsa.cs
        // <Param to jwk>                             // <Jwk to param>
        //header.Set("kty", "RSA");                   //RSAParameters parameters = new RSAParameters();
        //header.Set("n", parameters.Modulus);        //parameters.Modulus = header.GetBytes("n");
        //header.Set("e", parameters.Exponent);       //parameters.Exponent = header.GetBytes("e");
        //
        // ココから下は秘密鍵の領域                   // ココから下は秘密鍵の領域
        //if (includePrivateParameters)               //if (header.ContainsKey("d"))
        //{                                           //{
        //    header.Set("d", parameters.D);          //    parameters.D = header.GetBytes("d");
        //    header.Set("p", parameters.P);          //    parameters.P = header.GetBytes("p");
        //    header.Set("q", parameters.Q);          //    parameters.Q = header.GetBytes("q");
        //    header.Set("dp", parameters.DP);        //    parameters.DP = header.GetBytes("dp");
        //    header.Set("dq", parameters.DQ);        //    parameters.DQ = header.GetBytes("dq");
        //    header.Set("qi", parameters.InverseQ);  //    parameters.InverseQ = header.GetBytes("qi");
        //}                                           //}
        //                                            // ↓↓↓
        //                                            //RSA rsa = RSA.Create();
        //                                            //rsa.ImportParameters(parameters);
        //                                            //return rsa;

        #region ParamToJwk
        /// <summary>ParamToJwk</summary>
        /// <param name="param">RSAParameters</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public string ParamToJwk(
            RSAParameters param,
            JsonSerializerSettings settings = null)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic[JwtConst.kty] = JwtConst.RSA; // 必須

            switch (this.RSnnn)
            {
                case JWS_HMACSHA.RS._256:
                    dic[JwtConst.alg] = JwtConst.RS256;
                    break;

                case JWS_HMACSHA.RS._384:
                    dic[JwtConst.alg] = JwtConst.RS384;
                    break;

                case JWS_HMACSHA.RS._512:
                    dic[JwtConst.alg] = JwtConst.RS512;
                    break;
            }

            // Public
            dic[JwtConst.n] = CustomEncode.ToBase64UrlString(param.Modulus);
            dic[JwtConst.e] = CustomEncode.ToBase64UrlString(param.Exponent); //"AQAB";

            // JSON Web Key (JWK) Thumbprint
            // https://openid-foundation-japan.github.io/rfc7638.ja.html
            // kid : https://openid-foundation-japan.github.io/rfc7638.ja.html#Example
            //       https://openid-foundation-japan.github.io/rfc7638.ja.html#MembersUsed
            //       kidには、JWK の JWK Thumbprint 値などが用いられるらしい。
            //       ★ RSA 公開鍵の必須メンバを辞書順に並べると、e, kty, n となる。

            dic[JwtConst.kid] = CustomEncode.ToBase64UrlString(
                GetHash.GetHashBytes(
                    CustomEncode.StringToByte(
                        JsonConvert.SerializeObject(new
                        {
                            e = dic[JwtConst.e],
                            kty = dic[JwtConst.kty],
                            n = dic[JwtConst.n]
                        }),
                        CustomEncode.UTF_8),
                    this.HashAlgorithm));

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

        #region JwkToParam(Provider)

        #region Provider
        /// <summary>JwkToProvider</summary>
        /// <param name="jwkString">string</param>
        /// <returns>RSA（公開鍵）</returns>
        public RSA JwkToProvider(string jwkString)
        {
            RSAParameters rsaParameters = this.JwkToParam(jwkString);
            RSA rsa = AsymmetricAlgorithmCmnFunc.RsaFactory();
            rsa.ImportParameters(rsaParameters);
            return rsa;
        }

        /// <summary>JwkToProvider</summary>
        /// <param name="jwkObject">JObject</param>
        /// <returns>RSA（公開鍵）</returns>
        public RSA JwkToProvider(JObject jwkObject)
        {
            RSAParameters rsaParameters = this.JwkToParam(jwkObject);
            RSA rsa = AsymmetricAlgorithmCmnFunc.RsaFactory();
            rsa.ImportParameters(rsaParameters);
            return rsa;
        }
        #endregion

        #region Param
        /// <summary>JwkToParam</summary>
        /// <param name="jwkString">string</param>
        /// <returns>RSAParameters（公開鍵）</returns>
        public RSAParameters JwkToParam(string jwkString)
        {
            return this.JwkToParam(
                JsonConvert.DeserializeObject<JObject>(jwkString));
        }

        /// <summary>JwkToParam</summary>
        /// <param name="jwkObject">JObject</param>
        /// <returns>RSAParameters（公開鍵）</returns>
        public RSAParameters JwkToParam(JObject jwkObject)
        {
            if (jwkObject[JwtConst.kty].ToString().ToUpper() == JwtConst.RSA)
            {
                // RSAParameters
                // FromBase64Stringだとエラーになる。
                RSAParameters rsaParameters = new RSAParameters()
                {
                    // Public
                    Modulus = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.n]),
                    Exponent = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.e]),
                };

                return rsaParameters;
            }

            throw new ArgumentOutOfRangeException("jwkObject", jwkObject, "Invalid");
        }
        #endregion

        #endregion

        #endregion

        #endregion
    }
}
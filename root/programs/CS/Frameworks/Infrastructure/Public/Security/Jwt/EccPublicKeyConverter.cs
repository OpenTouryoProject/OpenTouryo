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
//* クラス名        ：EccPublicKeyConverter
//* クラス日本語名  ：EccPublicKeyConverterクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/12  西野 大介         新規作成
//*  2019/01/16  西野 大介         X509KeyStorageFlags・名前付き引数対応
//*                                下位がExportableである必要性があった、
//*                                また、ASP.NET上で実行する可能性もある。
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//using Jose;
using Security.Cryptography;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security.Jwt
{
    /// <summary>
    /// ECC関係のカギ変換処理を実装する。
    /// 基本的に変換先は公開鍵。変換元は秘密鍵も扱える。
    /// X.509 → ECDsa（Cngkey, ECParameters）（公開鍵）⇔ Jwk 公開鍵
    /// </summary>
    public class EccPublicKeyConverter
    {
        // X.509からの変換
        #region X.509 鍵 → ECDsaProvider(Cngkey, ECParameters) → Jwk

        // X.509 は、NET47以降
#if NET45 || NET46
#else
        #region *.cer

        #region Jwk
        /// <summary>X509CerToJwk</summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public static string X509CerToJwk(
            string certificateFilePath,
            JsonSerializerSettings settings = null)
        {
            return EccPublicKeyConverter.ParamToJwk( // *.cer is PublicKey -> ExportParameters(false)
                EccPublicKeyConverter.X509CerToProvider(certificateFilePath).ExportParameters(false), settings);
        }
        #endregion

        #region ECDsaProvider(Cngkey, ECParameters)
        /// <summary>X509CerToCngkey</summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <returns>CngKey（公開鍵）</returns>
        public static CngKey X509CerToCngkey(string certificateFilePath)
        {
            return ((ECDsaCng)EccPublicKeyConverter.X509CerToProvider(certificateFilePath)).Key;
        }

        /// <summary>X509CerToECParam</summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <returns>ECParameters（公開鍵）</returns>
        public static ECParameters X509CerToECParam(string certificateFilePath)
        {
            return EccPublicKeyConverter.X509CerToProvider(
                certificateFilePath).ExportParameters(false);
        }

        /// <summary>X509CerToProvider</summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <param name="flg">X509KeyStorageFlags</param>
        /// <returns>ECDsa（公開鍵）</returns>
        public static ECDsa X509CerToProvider(
            string certificateFilePath, 
            X509KeyStorageFlags flg = X509KeyStorageFlags.DefaultKeySet)
        {
            // X509KeyStorageFlagsについて
            // PublicKey を取り出すため Exportableは不要
            // ASP.NET（サーバ）からの実行を想定しないため、MachineKeySetは不要
            DigitalSignECDsaX509 dsX509 = new DigitalSignECDsaX509(
                certificateFilePath, "", HashAlgorithmName.SHA256, flg);

            AsymmetricAlgorithm aa = dsX509.PublicKey;
            if (aa is ECDsa)
            {
                return (ECDsa)aa;
            }

            return null;
        }
        #endregion

        #endregion

        #region *.pfx

        #region Jwk
        /// <summary>X509CerToJwk</summary>
        /// <param name="certificateFilePath">X.509鍵(*.pfx)</param>
        /// <param name="password">string</param>
        /// <param name="hashAlgorithmName">HashAlgorithmName</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public static string X509PfxToJwk(
            string certificateFilePath, string password,
            HashAlgorithmName hashAlgorithmName,
            JsonSerializerSettings settings = null)
        {
            return EccPublicKeyConverter.ParamToJwk( // *.cer is PublicKey -> ExportParameters(false)
                EccPublicKeyConverter.X509PfxToProvider(certificateFilePath, password, hashAlgorithmName).ExportParameters(false), settings);
        }
        #endregion

        #region ECDsaProvider(ECParameters)
        /// <summary>X509PfxToParam</summary>
        /// <param name="certificateFilePath">X.509鍵(*.cer)</param>
        /// <param name="password">string</param>
        /// <param name="hashAlgorithmName">HashAlgorithmName</param>
        /// <returns>ECParameters（公開鍵）</returns>
        public static ECParameters X509PfxToParam(string certificateFilePath, string password, HashAlgorithmName hashAlgorithmName)
        {
            return EccPublicKeyConverter.X509PfxToProvider(certificateFilePath, password, hashAlgorithmName).ExportParameters(false);
        }

        /// <summary>X509CerToProvider</summary>
        /// <param name="certificateFilePath">X.509鍵(*.pfx)</param>
        /// <param name="password">string</param>
        /// <param name="hashAlgorithmName">HashAlgorithmName</param>
        /// <param name="flg">X509KeyStorageFlags</param>
        /// <returns>ECDsa（公開鍵）</returns>
        public static ECDsa X509PfxToProvider(
            string certificateFilePath, string password,
            HashAlgorithmName hashAlgorithmName,
            X509KeyStorageFlags flg = X509KeyStorageFlags.DefaultKeySet)
        {
            // X509KeyStorageFlagsについて
            // PublicKey を取り出すため Exportableは不要
            // ASP.NET（サーバ）からの実行を想定しないため、MachineKeySetは不要
            DigitalSignECDsaX509 dsX509 = new DigitalSignECDsaX509(
                certificateFilePath, password, hashAlgorithmName, flg);

            AsymmetricAlgorithm aa = dsX509.PublicKey; // Public
            if (aa is ECDsa)
            {
                return (ECDsa)aa;
            }

            return null;
        }
        #endregion

        #endregion
#endif

        #endregion

        // 実用鍵からの変換
        #region ECDsa(Cngkey, ECParameters) ⇔ Jwk 公開鍵

        // <参考>
        // JSON Web Key (JWK)
        // https://openid-foundation-japan.github.io/rfc7517.ja.html
        //   を、"kty":"EC"で検索するとイイ。
        //
        // A.1.  Example Public Keys
        // https://openid-foundation-japan.github.io/rfc7517.ja.html#PublicExample
        // A.2.  Example Private Keys
        // https://openid-foundation-japan.github.io/rfc7517.ja.html#PrivateExample

        // ECCurve support of EccKey.
        // https://github.com/dvsekhvalnov/jose-jwt/issues/105
        // CngKey ⇔ Jwt
        // https://github.com/dvsekhvalnov/jose-jwt/blob/master/jose-jwt/Security/Cryptography/EccKey.cs

        #region 共通

        #region GetCrvStringFromXCoordinate
        /// <summary>GetCrvString</summary>
        /// <param name="x">byte[]</param>
        /// <returns>CrvString</returns>
        public static string GetCrvStringFromXCoordinate(byte[] x)
        {
            int partSize = x.Length;

            if (partSize == 32)
            {
                return JwtConst.P256;
            }
            else if (partSize == 48)
            {
                return JwtConst.P384;
            }
            else if (partSize == 66)
            {
                return JwtConst.P521;
            }
            else
            {
                throw new ArgumentException("Size of X must equal to 32, 48 or 66 bytes");
            }
        }
        #endregion

        #region CreateJwkFromDictionary
        /// <summary>CreateJwkFromDictionary</summary>
        /// <param name="dic">Dictionary</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>JwkString</returns>
        internal static string CreateJwkFromDictionary(
            Dictionary<string, string> dic,
            JsonSerializerSettings settings = null)
        {
            // JSON Web Key (JWK) Thumbprint
            // https://openid-foundation-japan.github.io/rfc7638.ja.html
            // kid : https://openid-foundation-japan.github.io/rfc7638.ja.html#Example
            //       https://openid-foundation-japan.github.io/rfc7638.ja.html#MembersUsed
            //       kidには、JWK の JWK Thumbprint 値などが用いられるらしい。
            //       ★ EC 公開鍵の必須メンバを辞書順に並べると、crv, kty, x, y となる。
            dic[JwtConst.kid] = CustomEncode.ToBase64UrlString(
                GetHash.GetHashBytes(
                    CustomEncode.StringToByte(
                        JsonConvert.SerializeObject(new
                        {
                            crv = dic[JwtConst.crv],
                            kty = dic[JwtConst.kty],
                            x = dic[JwtConst.x],
                            y = dic[JwtConst.y]
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

        #endregion

        #region Cng

        #region CngToJwk
        /*
        /// <summary>CngToJwk</summary>
        /// <param name="cngkey">CngKey</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public static string CngToJwk(
            CngKey cngkey,
            JsonSerializerSettings settings = null)
        {
            EccKey eccKey = EccKey.Generate(cngkey); // ★★ この使い方が誤りらしい。
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic[JwtConst.kty] = JwtConst.EC; // 必須
            dic[JwtConst.alg] = JwtConst.ES256;

            // 楕円曲線
            dic[JwtConst.crv] = EccPublicKeyConverter.GetCrvStringFromXCoordinate(eccKey.X);
            // 公開鍵の部分
            dic[JwtConst.x] = CustomEncode.ToBase64UrlString(eccKey.X);
            dic[JwtConst.y] = CustomEncode.ToBase64UrlString(eccKey.Y);
            //if (eccKey.D != null) // 秘密鍵の部分は処理しない
            //{
            //    dic[JwtConst.d] = CustomEncode.ToBase64UrlString(eccKey.D);
            //}
            return EccPublicKeyConverter.CreateJwkFromDictionary(dic, settings);
        }
        */
        #endregion

        #region JwkToCng
        /// <summary>JwkToCng</summary>
        /// <param name="jwkString">string</param>
        /// <returns>CngKey（公開鍵）</returns>
        public static CngKey JwkToCng(string jwkString)
        {
            return EccPublicKeyConverter.JwkToCng(
                JsonConvert.DeserializeObject<Dictionary<string, string>>(jwkString));
        }

        /// <summary>JwkToCng</summary>
        /// <param name="jwk">JObject</param>
        /// <returns>CngKey（公開鍵）</returns>
        public static CngKey JwkToCng(Dictionary<string, string> jwk)
        {
            // 楕円曲線
            // 不要
            // 公開鍵の部分
            return EccKey.New(
                CustomEncode.FromBase64UrlString((string)jwk[JwtConst.x]),
                CustomEncode.FromBase64UrlString((string)jwk[JwtConst.y]));
        }
        #endregion

        #endregion

        #region ECParameters
#if NET45 || NET46
#else
        // ECCurve and ECParameters to Jwt
        // https://github.com/psteniusubi/jose-jwt/blob/master/jose-jwt/jwk/JwkEc.cs

        #region 共通
        /// <summary>ECCurveDic</summary>
        internal static Dictionary<string, ECCurve> ECCurveDic = new Dictionary<string, ECCurve>()
        {
            { JwtConst.P256 , ECCurve.NamedCurves.nistP256 },
            { JwtConst.P384 , ECCurve.NamedCurves.nistP384 },
            { JwtConst.P521 , ECCurve.NamedCurves.nistP521 },
        };

        /// <summary>GetECCurveFromCrvString</summary>
        /// <param name="crvString">string</param>
        /// <returns>ECCurve</returns>
        public static ECCurve GetECCurveFromCrvString(string crvString)
        {
            return EccPublicKeyConverter.ECCurveDic[crvString];
        }

        /// <summary>GetCrvStrFromECCurve</summary>
        /// <param name="ecc">ECCurve</param>
        /// <returns>crvの文字列値（P-nnn）</returns>
        public static string GetCrvStringFromECCurve(ECCurve ecc)
        {
            foreach (string key in EccPublicKeyConverter.ECCurveDic.Keys)
            {
                ECCurve test = EccPublicKeyConverter.ECCurveDic[key];
                
                // 1. compare  ECCurve
                if (object.Equals(test, ecc)) return key;

                // 2. compare ECCurve.Oid
                if (test.Oid != null && ecc.Oid != null)
                {
                    if (object.Equals(test.Oid, ecc.Oid)) return key;
                }

                // 3. compare ECCurve.Oid.Value
                if (test.Oid.Value != null && ecc.Oid.Value != null)
                {   
                    if (object.Equals(test.Oid.Value, ecc.Oid.Value)) return key;
                }

                // 4. compare ECCurve.Oid.FriendlyName
                if (test.Oid.FriendlyName != null && ecc.Oid.FriendlyName != null)
                {
                    if (object.Equals(test.Oid.FriendlyName, ecc.Oid.FriendlyName)) return key;
                }
            }

            throw new ArgumentOutOfRangeException("ecc", ecc, "Invalid");
        }
        #endregion

        #region ParamToJwk
        /// <summary>ParamToJwk</summary>
        /// <param name="ecParams">ECParameters</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk公開鍵</returns>
        public static string ParamToJwk(
            ECParameters ecParams,
            JsonSerializerSettings settings = null)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic[JwtConst.kty] = JwtConst.EC; // 必須
            dic[JwtConst.alg] = JwtConst.ES256;

            // 楕円曲線
            dic[JwtConst.crv] = EccPublicKeyConverter.GetCrvStringFromECCurve(ecParams.Curve);
            // 公開鍵の部分
            dic[JwtConst.x] = CustomEncode.ToBase64UrlString(ecParams.Q.X);
            dic[JwtConst.y] = CustomEncode.ToBase64UrlString(ecParams.Q.Y);
            //if (ecParams.D != null) // 秘密鍵の部分は処理しない
            //{
            //    dic[JwtConst.d] = CustomEncode.ToBase64UrlString(ecParams.D);
            //}

            return EccPublicKeyConverter.CreateJwkFromDictionary(dic, settings);
        }
        #endregion

        #region JwkToParam
        /// <summary>JwkToParam</summary>
        /// <param name="jwkString">string</param>
        /// <returns>ECParameters（公開鍵）</returns>
        public static ECParameters JwkToParam(string jwkString)
        {
            return EccPublicKeyConverter.JwkToParam(
                JsonConvert.DeserializeObject<JObject>(jwkString));
        }

        /// <summary>JwkToParam</summary>
        /// <param name="jwkObject">JObject</param>
        /// <returns>ECParameters（公開鍵）</returns>
        public static ECParameters JwkToParam(JObject jwkObject)
        {            
            ECParameters ecParams = new ECParameters();

            // 楕円曲線
            ecParams.Curve = EccPublicKeyConverter.ECCurveDic[(string)jwkObject[JwtConst.crv]]; 

            // 公開鍵の部分
            ecParams.Q.X = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.x]);
            ecParams.Q.Y = CustomEncode.FromBase64UrlString((string)jwkObject[JwtConst.y]);
            //if (jwk.ContainsKey(JwtConst.d)) // 秘密鍵の部分は処理しない
            //{
            //    ecParams.D = CustomEncode.FromBase64UrlString((string)jwk[JwtConst.d]);
            //}

            return ecParams;
        }
        #endregion
#endif
        #endregion

        #endregion
    }
}

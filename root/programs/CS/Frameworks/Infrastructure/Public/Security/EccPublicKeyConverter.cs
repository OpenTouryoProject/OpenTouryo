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
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security
{
    /// <summary>
    /// ECC関係のカギ変換処理を実装する。
    /// 基本的に変換先は公開鍵。変換元は秘密鍵も扱える。
    /// X.509 → Cngkey, ECParameters（公開鍵）⇔ Jwk 公開鍵
    /// </summary>
    public class EccPublicKeyConverter
    {
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
        // C.1.  Plaintext RSA Private Key
        // https://openid-foundation-japan.github.io/rfc7517.ja.html#example-privkey-plaintext

        // https://github.com/dvsekhvalnov/jose-jwt/issues/105
        
        // CngKey to Jwt
        // https://github.com/dvsekhvalnov/jose-jwt/blob/master/jose-jwt/Security/Cryptography/EccKey.cs

        #region CngToJwk
        /// <summary>CngKeyToJwk</summary>
        /// <param name="cngkey">CngKey</param>
        /// <returns>Jwk（公開鍵）</returns>
        public static string CngToJwk(CngKey cngkey)
        {
            return "";
        }

        /// <summary>CngToJwk</summary>
        /// <param name="cngkey">CngKey</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk（公開鍵）</returns>
        public static string CngToJwk(CngKey cngkey, JsonSerializerSettings settings)
        {
            return "";
        }
        #endregion

        #region JwkToCng
        /// <summary>JwkToCng</summary>
        /// <param name="jwkString">string</param>
        /// <returns>CngKey（公開鍵）</returns>
        public static CngKey JwkToCng(string jwkString)
        {
            return CngKey.Create(CngAlgorithm.ECDsaP256);
        }

        /// <summary>JwkToCng</summary>
        /// <param name="jwk">JObject</param>
        /// <returns>CngKey（公開鍵）</returns>
        public static CngKey JwkToCng(Dictionary<string, string> jwk)
        {
            return CngKey.Create(CngAlgorithm.ECDsaP256);
        }
        #endregion


#if NET45 || NET46
#else
        // ECCurve and ECParameters to Jwt
        // https://github.com/psteniusubi/jose-jwt/blob/master/jose-jwt/jwk/JwkEc.cs

        #region ECCurve
        /// <summary>ECCurveDic</summary>
        private static Dictionary<string, ECCurve> ECCurveDic = new Dictionary<string, ECCurve>()
        {
            { "P-256" , ECCurve.NamedCurves.nistP256 },
            { "P-384" , ECCurve.NamedCurves.nistP384 },
            { "P-521" , ECCurve.NamedCurves.nistP521 },
        };

        /// <summary>GetECCurveFromCrvString</summary>
        /// <param name="crvString">string</param>
        /// <returns>ECCurve</returns>
        private static ECCurve GetECCurveFromCrvString(string crvString)
        {
            return EccPublicKeyConverter.ECCurveDic[crvString];
        }

        /// <summary>GetCrvStrFromECCurve</summary>
        /// <param name="ecc">ECCurve</param>
        /// <returns>crvの文字列値（P-nnn）</returns>
        private static string GetCrvStringFromECCurve(ECCurve ecc)
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
        /// <returns>Jwk（公開鍵）</returns>
        public static string ParamToJwk(ECParameters ecParams)
        {
            return EccPublicKeyConverter.ParamToJwk(ecParams, null);
        }

        /// <summary>ParamToJwk</summary>
        /// <param name="ecParams">ECParameters</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>Jwk（公開鍵）</returns>
        public static string ParamToJwk(ECParameters ecParams, JsonSerializerSettings settings)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic[JwtConst.kty] = "EC"; // 必須
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

        #region JwkToParam
        /// <summary>JwkToParam</summary>
        /// <param name="jwkString">string</param>
        /// <returns>ECParameters（公開鍵）</returns>
        public static ECParameters JwkToParam(string jwkString)
        {
            return EccPublicKeyConverter.JwkToParam(
                JsonConvert.DeserializeObject<Dictionary<string, string>>(jwkString));
        }

        /// <summary>JwkToParam</summary>
        /// <param name="jwk">JObject</param>
        /// <returns>ECParameters（公開鍵）</returns>
        public static ECParameters JwkToParam(Dictionary<string, string> jwk)
        {            
            ECParameters ecParams = new ECParameters();

            // 楕円曲線
            ecParams.Curve = EccPublicKeyConverter.ECCurveDic[(string)jwk[JwtConst.crv]]; 

            // 公開鍵の部分
            ecParams.Q.X = CustomEncode.FromBase64UrlString((string)jwk[JwtConst.x]);
            ecParams.Q.Y = CustomEncode.FromBase64UrlString((string)jwk[JwtConst.y]);
            //if (jwk.ContainsKey(JwtConst.d)) // 秘密鍵の部分は処理しない
            //{
            //    ecParams.D = CustomEncode.FromBase64UrlString((string)jwk[JwtConst.d]);
            //}

            return ecParams;
        }
        #endregion
#endif
        #endregion
    }
}

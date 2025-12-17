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
//* クラス名        ：JwtAssertion
//* クラス日本語名  ：JwtAssertion
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/12/26  西野 大介         新規作成
//*  2018/03/28  西野 大介         .NET Standard対応で、幾らか、I/F変更あり。
//*  2018/11/27  西野 大介         XML(Base64) ---> Jwk(Base64Url)に変更。
//*  2018/11/27  西野 大介         秘密鍵 <---> JWKのサポートを追加
//*  2020/03/04  西野 大介         ...ECDsaのサポートを追加
//*  2020/03/04  西野 大介         Claim生成メソッドの利用
//**********************************************************************************

using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>
    /// JWT bearer token authorizationグラント種別
    /// </summary>
    public class JwtAssertion
    {
        #region Create
        /// <summary>Create</summary>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="forExp">DateTimeOffset</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jwkPrivateKey">RS256用のJWK秘密鍵</param>
        /// <returns>JwtAssertion</returns>
        public static string Create(
            string iss, string aud, TimeSpan forExp, string scopes, string jwkPrivateKey)
        {
            JObject temp = JsonConvert.DeserializeObject<JObject>(jwkPrivateKey);
            if (temp.ContainsKey("kty"))
            {
                if (((string)temp["kty"]).ToUpper() == "RSA")
                {
                    RsaPrivateKeyConverter rpkc = new RsaPrivateKeyConverter(JWS_RSA.RS._256);
                    return JwtAssertion.CreateByRsa(iss, aud, forExp, scopes,
                        rpkc.JwkToParam(jwkPrivateKey));
                }
                else if (((string)temp["kty"]).ToUpper() == "EC")
                {
                    EccPrivateKeyConverter epkc = new EccPrivateKeyConverter(JWS_ECDSA.ES._256);
                    return JwtAssertion.CreateByECDsa(iss, aud, forExp, scopes,
                        epkc.JwkToParam(jwkPrivateKey));
                }
            }

            return "";
        }

        /// <summary>CreateByRsa</summary>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="forExp">DateTimeOffset</param>
        /// <param name="scopes">scopes</param>
        /// <param name="rsaPrivateKey">RS256用のRSAParameters秘密鍵</param>
        /// <returns>JwtAssertion</returns>
        public static string CreateByRsa(
            string iss, string aud, TimeSpan forExp, string scopes, RSAParameters rsaPrivateKey)
        {
            string json = "";
            //string jws = "";

            #region ClaimSetの生成

            Dictionary<string, object> jwtAssertionClaimSet = new Dictionary<string, object>();

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iss, iss); // client_id
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.aud, aud); // Token EndPointのuri。

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.exp, CmnJwtToken.CreateExpClaim(forExp));
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iat, CmnJwtToken.CreateIatClaim());

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.jti, CmnJwtToken.CreateJitClaim());
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.scope, scopes); // scopes

            json = JsonConvert.SerializeObject(jwtAssertionClaimSet);

            #endregion

            #region JWT化

            JWS_RS256_Param jwtRS256 = new JWS_RS256_Param(rsaPrivateKey);
            return jwtRS256.Create(json);

            #endregion
        }

        /// <summary>CreateByECDsa</summary>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="forExp">DateTimeOffset</param>
        /// <param name="scopes">scopes</param>
        /// <param name="eccPrivateKey">ES256用のECParameters秘密鍵</param>
        /// <returns>JwtAssertion</returns>
        public static string CreateByECDsa(
            string iss, string aud, TimeSpan forExp, string scopes, ECParameters eccPrivateKey)
        {
            string json = "";
            //string jws = "";

            #region ClaimSetの生成

            Dictionary<string, object> jwtAssertionClaimSet = new Dictionary<string, object>();

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iss, iss); // client_id
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.aud, aud); // Token EndPointのuri。

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.exp, CmnJwtToken.CreateExpClaim(forExp));
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iat, CmnJwtToken.CreateIatClaim());

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.jti, CmnJwtToken.CreateJitClaim());
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.scope, scopes); // scopes

            json = JsonConvert.SerializeObject(jwtAssertionClaimSet);

            #endregion

            #region JWT化

            JWS_ES256_Param jwtES256 = new JWS_ES256_Param(eccPrivateKey, true);
            return jwtES256.Create(json);

            #endregion
        }

        /// <summary>CreateByECDsa</summary>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="forExp">DateTimeOffset</param>
        /// <param name="scopes">scopes</param>
        /// <param name="ecdsaX509FilePath">ES256用の X.509秘密鍵 の File Path</param>
        /// <param name="ecdsaX509Password">ES256用の X.509秘密鍵 の Password</param>
        /// <returns>JwtAssertion</returns>
        public static string CreateByECDsa(
            string iss, string aud, TimeSpan forExp, string scopes,
            string ecdsaX509FilePath, string ecdsaX509Password)
        ///// <param name="eccPrivateKey">ES256用のECParameters秘密鍵</param>
        //ECParameters ecPrivateKey) // ECDsa.ExportParameters(true)が動かねぇ。
        {
            string json = "";
            //string jws = "";

            #region ClaimSetの生成

            Dictionary<string, object> jwtAssertionClaimSet = new Dictionary<string, object>();

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iss, iss); // client_id
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.aud, aud); // Token EndPointのuri。

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.exp, CmnJwtToken.CreateExpClaim(forExp));
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iat, CmnJwtToken.CreateIatClaim());

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.jti, CmnJwtToken.CreateJitClaim());
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.scope, scopes); // scopes

            json = JsonConvert.SerializeObject(jwtAssertionClaimSet);

            #endregion

            #region JWT化

            JWS_ES256_X509 jwtES256 = new JWS_ES256_X509(ecdsaX509FilePath, ecdsaX509Password);
            return jwtES256.Create(json);

            #endregion
        }
        #endregion

        #region Verify
        /// <summary>Verify</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jobj">JObject</param>
        /// <param name="jwkPublicKey">RS256用のJWK公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool Verify(string jwtAssertion,
            out string iss, out string aud, out string scopes, out JObject jobj, string jwkPublicKey)
        {
            iss = "";
            aud = "";
            scopes = "";
            jobj = null;

            JObject temp = JsonConvert.DeserializeObject<JObject>(jwkPublicKey);
            if (temp.ContainsKey("kty"))
            {
                if (((string)temp["kty"]).ToUpper() == "RSA")
                {
                    RsaPublicKeyConverter rpkc = new RsaPublicKeyConverter();
                    return JwtAssertion.VerifyByRsa(jwtAssertion,
                        out iss, out aud, out scopes, out jobj,
                        rpkc.JwkToParam(jwkPublicKey));
                }
                else if (((string)temp["kty"]).ToUpper() == "EC")
                {
                    EccPublicKeyConverter epkc = new EccPublicKeyConverter();
                    return JwtAssertion.VerifyByECDsa(jwtAssertion,
                        out iss, out aud, out scopes, out jobj,
                        epkc.JwkToParam(jwkPublicKey));
                }
            }

            return false;
        }

        /// <summary>VerifyByRsa</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jobj">JObject</param>
        /// <param name="rsaPublicKey">RS256用のRSAParameters公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool VerifyByRsa(string jwtAssertion,
            out string iss, out string aud, out string scopes, out JObject jobj, RSAParameters rsaPublicKey)
        {
            iss = "";
            aud = "";
            scopes = "";
            jobj = null;

            JWS_RS256_Param jwtRS256 = new JWS_RS256_Param(rsaPublicKey);

            if (jwtRS256.Verify(jwtAssertion))
            {
                string jwtPayload = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(jwtAssertion.Split('.')[1]), CustomEncode.UTF_8);

                jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                iss = (string)jobj[OAuth2AndOIDCConst.iss];
                aud = (string)jobj[OAuth2AndOIDCConst.aud];
                //string iat = (string)jobj[OAuth2AndOIDCConst.iat];
                scopes = (string)jobj[OAuth2AndOIDCConst.scope];

                long unixTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

                string exp = (string)jobj[OAuth2AndOIDCConst.exp];
                if (long.Parse(exp) >= unixTimeSeconds)
                {
                    return true;
                }
                else
                {
                    // JWTの内容検証に失敗
                }
            }
            else
            {
                // JWTの署名検証に失敗
            }

            // 認証に失敗
            return false;
        }

        /// <summary>VerifyByECDsa</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jobj">JObject</param>
        /// <param name="eccPublicKey">ES256用のECParameters公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool VerifyByECDsa(string jwtAssertion,
            out string iss, out string aud, out string scopes, out JObject jobj, ECParameters eccPublicKey)
        {
            iss = "";
            aud = "";
            scopes = "";
            jobj = null;

            JWS_ES256_Param jwtES256 = new JWS_ES256_Param(eccPublicKey, false);

            if (jwtES256.Verify(jwtAssertion))
            {
                string jwtPayload = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(jwtAssertion.Split('.')[1]), CustomEncode.UTF_8);

                jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                iss = (string)jobj[OAuth2AndOIDCConst.iss];
                aud = (string)jobj[OAuth2AndOIDCConst.aud];
                //string iat = (string)jobj[OAuth2AndOIDCConst.iat];
                scopes = (string)jobj[OAuth2AndOIDCConst.scope];

                long unixTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
                string exp = (string)jobj[OAuth2AndOIDCConst.exp];
                if (long.Parse(exp) >= unixTimeSeconds)
                {
                    return true;
                }
                else
                {
                    // JWTの内容検証に失敗
                }
            }
            else
            {
                // JWTの署名検証に失敗
            }

            // 認証に失敗
            return false;
        }
        #endregion
    }
}

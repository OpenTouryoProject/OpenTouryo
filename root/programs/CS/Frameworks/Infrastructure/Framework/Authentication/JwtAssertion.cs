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
//*  2018/11/27  西野 大介         秘密鍵 <---> JWKサポート追加
//**********************************************************************************

using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

#if NETSTD
using Microsoft.AspNetCore.WebUtilities;
#elif NET45
using Touryo.Infrastructure.Public.Util;
using Microsoft.Owin.Security.DataHandler.Encoder;
#else
using Microsoft.Owin.Security.DataHandler.Encoder;
#endif

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>
    /// OAuth2やOIDC関連で、クライアント認証的に使用するJwtAssertion処理
    /// - JWT bearer token authorizationグラント種別
    /// - OpenID　Connectリクエスト・オブジェクト（予定）
    /// </summary>
    public class JwtAssertion
    {
        #region CreateJwtBearerTokenFlowAssertion
        /// <summary>CreateJwtBearerTokenFlowAssertionJWK</summary>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="forExp">DateTimeOffset</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jwkPrivateKey">RS256用のJWK秘密鍵</param>
        /// <returns>JwtAssertion</returns>
        public static string CreateJwtBearerTokenFlowAssertionJWK(
            string iss, string aud, TimeSpan forExp, string scopes, string jwkPrivateKey)
        {
            return JwtAssertion.CreateJwtBearerTokenFlowAssertion(iss, aud, forExp, scopes,
                PrivateKeyConverter.JwkToRsaParam(jwkPrivateKey));
        }

        /// <summary>CreateJwtBearerTokenFlowAssertion</summary>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="forExp">DateTimeOffset</param>
        /// <param name="scopes">scopes</param>
        /// <param name="rsaPrivateKey">RS256用のRSAParameters秘密鍵</param>
        /// <returns>JwtAssertion</returns>
        public static string CreateJwtBearerTokenFlowAssertion(
            string iss, string aud, TimeSpan forExp, string scopes, RSAParameters rsaPrivateKey)
        {
            string json = "";
            //string jws = "";

            #region ClaimSetの生成

            Dictionary<string, object> jwtAssertionClaimSet = new Dictionary<string, object>();
            
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iss, iss); // client_id
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.aud, aud); // Token2 EndPointのuri。

#if NET45
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.exp, PubCmnFunction.ToUnixTime(DateTimeOffset.Now.Add(forExp)).ToString());
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iat, PubCmnFunction.ToUnixTime(DateTimeOffset.Now).ToString());
#else
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.exp, (DateTimeOffset.Now.Add(forExp)).ToUnixTimeSeconds().ToString());
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
#endif

            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.jti, Guid.NewGuid().ToString("N"));
            jwtAssertionClaimSet.Add(OAuth2AndOIDCConst.scope, scopes); // scopes

            json = JsonConvert.SerializeObject(jwtAssertionClaimSet);

            #endregion

            #region JWT化

            JWS_RS256_Param jwtRS256 = new JWS_RS256_Param(rsaPrivateKey);
            return jwtRS256.Create(json);

            #endregion
        }
        #endregion

        #region VerifyJwtBearerTokenFlowAssertion
        /// <summary>VerifyJwtBearerTokenFlowAssertion</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jobj">JObject</param>
        /// <param name="jwkPublicKey">RS256用のJWK公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool VerifyJwtBearerTokenFlowAssertionJWK(string jwtAssertion,
            out string iss, out string aud, out string scopes, out JObject jobj, string jwkPublicKey)
        {
            return JwtAssertion.VerifyJwtBearerTokenFlowAssertion(
                jwtAssertion, out iss, out aud, out scopes, out jobj,
                RsaPublicKeyConverter.JwkToParam(jwkPublicKey));
        }

        /// <summary>VerifyJwtBearerTokenFlowAssertion</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jobj">JObject</param>
        /// <param name="rsaPublicKey">RS256用のRSAParameters公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool VerifyJwtBearerTokenFlowAssertion(string jwtAssertion,
            out string iss, out string aud, out string scopes, out JObject jobj, RSAParameters rsaPublicKey)
        {
            iss = "";
            aud = "";
            scopes = "";
            jobj = null;

            JWS_RS256_Param jwtRS256 = new JWS_RS256_Param(rsaPublicKey);

            if (jwtRS256.Verify(jwtAssertion))
            {
#if NETSTD
                string jwtPayload = Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(jwtAssertion.Split('.')[1]));
#else
                Base64UrlTextEncoder base64UrlEncoder = new Base64UrlTextEncoder();
                string jwtPayload = Encoding.UTF8.GetString(base64UrlEncoder.Decode(jwtAssertion.Split('.')[1]));
#endif
                jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                iss = (string)jobj[OAuth2AndOIDCConst.iss];
                aud = (string)jobj[OAuth2AndOIDCConst.aud];
                //string iat = (string)jobj[OAuth2AndOIDCConst.iat];
                scopes = (string)jobj[OAuth2AndOIDCConst.scope];
                
                long unixTimeSeconds = 0;
#if NET45
                unixTimeSeconds = PubCmnFunction.ToUnixTime(DateTimeOffset.Now);
#else
                unixTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
#endif
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

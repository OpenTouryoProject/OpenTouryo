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
//* クラス名        ：RequestObject
//* クラス日本語名  ：RequestObject
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/19  西野 大介         新規作成
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
    /// OpenID Connect > RequestObject
    /// </summary>
    public class RequestObject
    {

        #region Create
        /// <summary>Create</summary>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="forExp">DateTimeOffset</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jwkPrivateKey">RS256用のJWK秘密鍵</param>
        /// <returns>RequestObject</returns>
        public static string Create(
            string iss, string aud, TimeSpan forExp, string scopes, string jwkPrivateKey)
        {
            return RequestObject.Create(iss, aud, forExp, scopes,
                PrivateKeyConverter.JwkToRsaParam(jwkPrivateKey));
        }

        /// <summary>Create</summary>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="forExp">DateTimeOffset</param>
        /// <param name="scopes">scopes</param>
        /// <param name="rsaPrivateKey">RS256用のRSAParameters秘密鍵</param>
        /// <returns>RequestObject</returns>
        public static string Create(
            string iss, string aud, TimeSpan forExp, string scopes, RSAParameters rsaPrivateKey)
        {
            string json = "";
            //string jws = "";

            #region ClaimSetの生成

            Dictionary<string, object> requestObjectClaimSet = new Dictionary<string, object>();

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.iss, iss); // client_id
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.aud, aud); // Token2 EndPointのuri。

#if NET45
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.exp, PubCmnFunction.ToUnixTime(DateTimeOffset.Now.Add(forExp)).ToString());
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.iat, PubCmnFunction.ToUnixTime(DateTimeOffset.Now).ToString());
#else
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.exp, (DateTimeOffset.Now.Add(forExp)).ToUnixTimeSeconds().ToString());
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
#endif

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.jti, Guid.NewGuid().ToString("N"));
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.scope, scopes); // scopes

            json = JsonConvert.SerializeObject(requestObjectClaimSet);

            #endregion

            #region JWT化

            JWS_RS256_Param jwtRS256 = new JWS_RS256_Param(rsaPrivateKey);
            return jwtRS256.Create(json);

            #endregion
        }
        #endregion

        // c# - Convert JObject into Dictionary<string, object>. Is it possible? - Stack Overflow
        // https://stackoverflow.com/questions/14886800/convert-jobject-into-dictionarystring-object-is-it-possible

        #region Verify
        /// <summary>Verify</summary>
        /// <param name="requestObject">string</param>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jobj">JObject</param>
        /// <param name="jwkPublicKey">RS256用のJWK公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool Verify(string requestObject,
            out string iss, out string aud, out string scopes, out JObject jobj, string jwkPublicKey)
        {
            return RequestObject.Verify(
                requestObject, out iss, out aud, out scopes, out jobj,
                RsaPublicKeyConverter.JwkToParam(jwkPublicKey));
        }

        /// <summary>Verify</summary>
        /// <param name="requestObject">string</param>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jobj">JObject</param>
        /// <param name="rsaPublicKey">RS256用のRSAParameters公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool Verify(string requestObject,
            out string iss, out string aud, out string scopes, out JObject jobj, RSAParameters rsaPublicKey)
        {
            iss = "";
            aud = "";
            scopes = "";
            jobj = null;

            JWS_RS256_Param jwtRS256 = new JWS_RS256_Param(rsaPublicKey);

            if (jwtRS256.Verify(requestObject))
            {
                string jwtPayload = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(requestObject.Split('.')[1]), CustomEncode.UTF_8);

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

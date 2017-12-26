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
//**********************************************************************************

using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Owin.Security.DataHandler.Encoder;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>
    /// OAuth2やOIDC関連で、クライアント認証的に使用するJwtAssertion処理
    /// - JWT bearer token authorizationグラント種別
    /// - OpenID　Connectリクエスト・オブジェクト（予定）
    /// </summary>
    public class JwtAssertion
    {
        /// <summary>CreateJwtBearerTokenFlowAssertion</summary>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="forExp">DateTimeOffset</param>
        /// <param name="scopes">scopes</param>
        /// <param name="xmlPrivateKey">RS256用のXML秘密鍵</param>
        /// <returns>JwtAssertion</returns>
        public string CreateJwtBearerTokenFlowAssertion(string iss, string aud, TimeSpan forExp, string scopes, string xmlPrivateKey)
        {
            string json = "";
            string jwt = "";

            #region ClaimSetの生成

            Dictionary<string, object> jwtAssertionClaimSet = new Dictionary<string, object>();
            
            jwtAssertionClaimSet.Add("iss", iss); // client_id
            jwtAssertionClaimSet.Add("aud", aud); // Token2 EndPointのuri。

#if NET45
            jwtAssertionClaimSet.Add("exp", PubCmnFunction.ToUnixTime(DateTimeOffset.Now.Add(forExp)).ToString());
            jwtAssertionClaimSet.Add("iat", PubCmnFunction.ToUnixTime(DateTimeOffset.Now).ToString());
#else
            jwtAssertionClaimSet.Add("exp", (DateTimeOffset.Now.Add(forExp)).ToUnixTimeSeconds().ToString());
            jwtAssertionClaimSet.Add("iat", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
#endif

            jwtAssertionClaimSet.Add("jti", Guid.NewGuid().ToString("N"));
            jwtAssertionClaimSet.Add("scope", scopes); // scopes

            json = JsonConvert.SerializeObject(jwtAssertionClaimSet);

            #endregion

            #region JWT化

            JWT_RS256_XML jwtRS256 = null;

            // 署名
            jwtRS256 = new JWT_RS256_XML(xmlPrivateKey);
            jwt = jwtRS256.Create(json);

            // 検証
            jwtRS256 = new JWT_RS256_XML(xmlPrivateKey);
            if (jwtRS256.Verify(jwt))
            {
                return jwt; // 検証できた。
            }
            else
            {
                return ""; // 検証できなかった。
            }

            #endregion
        }

        /// <summary>VerifyJwtBearerTokenFlowAssertion</summary>
        /// <param name="jwtAssertion">string</param>
        /// <param name="iss">client_id</param>
        /// <param name="aud">Token2 EndPointのuri</param>
        /// <param name="scopes">scopes</param>
        /// <param name="jobj">JObject</param>
        /// <param name="xmlPublicKey">RS256用のXML公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool VerifyJwtBearerTokenFlowAssertion(string jwtAssertion,
            out string iss, out string aud, out List<string> scopes, out JObject jobj, string xmlPublicKey)
        {
            iss = "";
            aud = "";
            scopes = new List<string>();
            jobj = null;

            JWT_RS256_XML jwtRS256 = new JWT_RS256_XML(xmlPublicKey);

            if (jwtRS256.Verify(jwtAssertion))
            {
                Base64UrlTextEncoder base64UrlEncoder = new Base64UrlTextEncoder();
                string jwtPayload = Encoding.UTF8.GetString(base64UrlEncoder.Decode(jwtAssertion.Split('.')[1]));
                jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                iss = (string)jobj["iss"];
                aud = (string)jobj["aud"];
                //string iat = (string)jobj["iat"];
                string exp = (string)jobj["exp"];

                if (jobj["scopes"] != null)
                {
                    scopes = JsonConvert.DeserializeObject<List<string>>(jobj["scopes"].ToString());
                }

                long unixTimeSeconds = 0;
#if NET45
                unixTimeSeconds = PubCmnFunction.ToUnixTime(DateTimeOffset.Now);
#else
                unixTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
#endif

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
    }
}

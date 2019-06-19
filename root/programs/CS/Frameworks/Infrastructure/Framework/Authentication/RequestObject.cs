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
        // https://openid.net/specs/openid-connect-core-1_0.html#RequestObject
        //  {
        //   "iss": "s6BhdRkqt3",
        //   "aud": "https://server.example.com",
        //   "response_type": "code id_token",
        //   "client_id": "s6BhdRkqt3",
        //   "redirect_uri": "https://client.example.org/cb",
        //   "scope": "openid",
        //   "state": "af0ifjsldkj",
        //   "nonce": "n-0S6_WzA2Mj",
        //   "max_age": 86400,
        //   "claims": ... see : ClaimsInRO.cs
        //  }

        // 以下はI/F上に含めない。
        // - display ... promptの形式
        // - ui_locales ... UICulture的な
        // - max_age ... 最大認証期間
        // - id_token_hint ... 以前のid_token（再認証）
        // acr_values(これは、claimsでサポート)

        #region Create
        /// <summary>Create</summary>
        /// <param name="iss">string</param>
        /// <param name="aud">string</param>
        /// <param name="response_type">string</param>
        /// <param name="response_mode">string</param>
        /// <param name="redirect_uri">string</param>
        /// <param name="scopes">string</param>
        /// <param name="state">string</param>
        /// <param name="nonce">string</param>
        /// <param name="prompt">string</param>
        /// <param name="login_hint">string</param>
        /// <param name="claims">ClaimsInRO</param>
        /// <param name="jwkPrivateKey">string</param>
        /// <returns>RequestObject</returns>
        public static string Create(
            string iss, string aud, string response_type, string response_mode,
            string redirect_uri, string scopes, string state, string nonce,
            string prompt, string login_hint, ClaimsInRO claims, string jwkPrivateKey)
        {
            return RequestObject.Create(
                iss, aud, response_type, response_mode,
                redirect_uri, scopes, state, nonce,
                prompt, login_hint, claims,
                PrivateKeyConverter.JwkToRsaParam(jwkPrivateKey));
        }

        /// <summary>Create</summary>
        /// <param name="iss">string</param>
        /// <param name="aud">string</param>
        /// <param name="response_type">string</param>
        /// <param name="response_mode">string</param>
        /// <param name="redirect_uri">string</param>
        /// <param name="scopes">string</param>
        /// <param name="state">string</param>
        /// <param name="nonce">string</param>
        /// <param name="prompt">string</param>
        /// <param name="login_hint">string</param>
        /// <param name="claims">ClaimsInRO</param>
        /// <param name="rsaPrivateKey">RS256用のRSAParameters秘密鍵</param>
        /// <returns>RequestObject</returns>
        public static string Create(
            string iss, string aud, string response_type, string response_mode,
            string redirect_uri, string scopes, string state, string nonce,
            string prompt, string login_hint, ClaimsInRO claims, RSAParameters rsaPrivateKey)
        {
            string json = "";
            //string jws = "";

            #region ClaimSetの生成

            Dictionary<string, object> requestObjectClaimSet = new Dictionary<string, object>();

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.iss, iss); // client_id
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.aud, aud); // ROS EndPointのuri。

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.response_type, response_type);
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.client_id, iss);

            if (!string.IsNullOrEmpty(response_mode))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.response_mode, response_mode);
            if (!string.IsNullOrEmpty(redirect_uri))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.redirect_uri, redirect_uri);

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.scope, scopes);
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.state, state);

            if (!string.IsNullOrEmpty(nonce))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.nonce, nonce);
            if (!string.IsNullOrEmpty(prompt))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.prompt, prompt);
            if (!string.IsNullOrEmpty(login_hint))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.login_hint, login_hint);

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.claims, claims.Claims);

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
        /// <param name="iss">string</param>
        /// <param name="aud">string</param>
        /// <param name="response_type">string</param>
        /// <param name="response_mode">string</param>
        /// <param name="redirect_uri">string</param>
        /// <param name="scopes">string</param>
        /// <param name="state">string</param>
        /// <param name="nonce">string</param>
        /// <param name="prompt">string</param>
        /// <param name="login_hint">string</param>
        /// <param name="claims">JObject</param>
        /// <param name="jwkPublicKey">RS256用のJWK公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool Verify(string requestObject,
            out string iss, out string aud, out string response_type, out string response_mode,
            out string redirect_uri, out string scopes, out string state, out string nonce,
            out string prompt, out string login_hint, out JObject claims, string jwkPublicKey)
        {
            return RequestObject.Verify(requestObject,
                out iss, out aud, out response_type, out response_mode,
                out redirect_uri, out scopes, out state, out nonce,
                out prompt, out login_hint, out claims, RsaPublicKeyConverter.JwkToParam(jwkPublicKey));
        }

        /// <summary>Verify</summary>
        /// <param name="requestObject">string</param>
        /// <param name="iss">string</param>
        /// <param name="aud">string</param>
        /// <param name="response_type">string</param>
        /// <param name="response_mode">string</param>
        /// <param name="redirect_uri">string</param>
        /// <param name="scopes">string</param>
        /// <param name="state">string</param>
        /// <param name="nonce">string</param>
        /// <param name="prompt">string</param>
        /// <param name="login_hint">string</param>
        /// <param name="claims">JObject</param>
        /// <param name="rsaPublicKey">RS256用のRSAParameters公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool Verify(string requestObject,
            out string iss, out string aud, out string response_type, out string response_mode,
            out string redirect_uri, out string scopes, out string state, out string nonce,
            out string prompt, out string login_hint, out JObject claims, RSAParameters rsaPublicKey)
        {
            iss = "";
            aud = "";
            response_type = "";
            response_mode = "";
            redirect_uri = "";
            scopes = "";
            state = "";
            nonce = "";
            prompt = "";
            login_hint = "";
            claims = null;

            JWS_RS256_Param jwtRS256 = new JWS_RS256_Param(rsaPublicKey);

            if (jwtRS256.Verify(requestObject))
            {
                string jwtPayload = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(requestObject.Split('.')[1]), CustomEncode.UTF_8);

                JObject jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                iss = (string)jobj[OAuth2AndOIDCConst.iss];
                aud = (string)jobj[OAuth2AndOIDCConst.aud];
                response_type = (string)jobj[OAuth2AndOIDCConst.response_type];

                if(jobj.ContainsKey(OAuth2AndOIDCConst.response_mode))
                    response_mode = (string)jobj[OAuth2AndOIDCConst.response_mode];
                if (jobj.ContainsKey(OAuth2AndOIDCConst.redirect_uri))
                    redirect_uri = (string)jobj[OAuth2AndOIDCConst.redirect_uri];

                scopes = (string)jobj[OAuth2AndOIDCConst.scope];
                state = (string)jobj[OAuth2AndOIDCConst.state];

                if (jobj.ContainsKey(OAuth2AndOIDCConst.nonce))
                    nonce = (string)jobj[OAuth2AndOIDCConst.nonce];
                if (jobj.ContainsKey(OAuth2AndOIDCConst.prompt))
                    prompt = (string)jobj[OAuth2AndOIDCConst.prompt];
                if (jobj.ContainsKey(OAuth2AndOIDCConst.login_hint))
                    login_hint = (string)jobj[OAuth2AndOIDCConst.login_hint];

                if (jobj.ContainsKey(OAuth2AndOIDCConst.claims))
                    claims = (JObject)jobj[OAuth2AndOIDCConst.claims];

                return true; 
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

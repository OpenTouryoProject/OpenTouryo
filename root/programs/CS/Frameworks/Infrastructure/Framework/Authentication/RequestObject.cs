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
//*  2020/03/03  西野 大介         CIBA対応
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
        #region FAPI2 CC

        // OpenID Connect Core 1.0 incorporating errata set 1
        // 6.1.  Passing a Request Object by Value
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
        // - id_token_hint ... 以前のid_token（再認証）

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
        /// <param name="max_age">string</param>
        /// <param name="prompt">string</param>
        /// <param name="login_hint">string</param>
        /// <param name="claims">ClaimsInRO</param>
        /// <param name="jwkPrivateKey">RS256用のJWK秘密鍵</param>
        /// <returns>RequestObject</returns>
        public static string Create(
            string iss, string aud, string response_type, string response_mode,
            string redirect_uri, string scopes, string state, string nonce,
            string max_age, string prompt, string login_hint, ClaimsInRO claims, string jwkPrivateKey)
        {
            RsaPrivateKeyConverter rpkc = new RsaPrivateKeyConverter();
            return RequestObject.Create(
                iss, aud, response_type, response_mode,
                redirect_uri, scopes, state, nonce,
                max_age, prompt, login_hint, claims,
                rpkc.JwkToParam(jwkPrivateKey));
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
        /// <param name="max_age">string</param>
        /// <param name="prompt">string</param>
        /// <param name="login_hint">string</param>
        /// <param name="claims">ClaimsInRO</param>
        /// <param name="rsaPrivateKey">RS256用のRSAParameters秘密鍵</param>
        /// <returns>RequestObject</returns>
        public static string Create(
            string iss, string aud, string response_type, string response_mode,
            string redirect_uri, string scopes, string state, string nonce,
            string max_age, string prompt, string login_hint, ClaimsInRO claims, RSAParameters rsaPrivateKey)
        {
            string json = "";

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
            if (!string.IsNullOrEmpty(max_age))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.max_age, max_age);
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

        #region Verify
        /// <summary>Verify</summary>
        /// <param name="ro">string</param>
        /// <param name="iss">string</param>
        /// <param name="jwkPublicKey">RS256用のJWK公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool Verify(string ro, out string iss, string jwkPublicKey)
        {
            RsaPublicKeyConverter rpkc = new RsaPublicKeyConverter();
            return RequestObject.Verify(ro, out iss,
                rpkc.JwkToParam(jwkPublicKey));
        }

        /// <summary>Verify</summary>
        /// <param name="ro">string</param>
        /// <param name="iss">string</param>
        /// <param name="rsaPublicKey">RS256用のRSAParameters公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool Verify(string ro, out string iss, RSAParameters rsaPublicKey)
        {
            iss = "";
            string aud = "";
            string response_type = "";
            string scopes = "";
            string state = "";
            string nonce = "";

            JWS_RS256_Param jwtRS256 = new JWS_RS256_Param(rsaPublicKey);

            if (jwtRS256.Verify(ro))
            {
                string jwtPayload = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(ro.Split('.')[1]), CustomEncode.UTF_8);

                JObject jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                iss = (string)jobj[OAuth2AndOIDCConst.iss];
                aud = (string)jobj[OAuth2AndOIDCConst.aud];
                response_type = (string)jobj[OAuth2AndOIDCConst.response_type];
                scopes = (string)jobj[OAuth2AndOIDCConst.scope];
                state = (string)jobj[OAuth2AndOIDCConst.state];
                nonce = (string)jobj[OAuth2AndOIDCConst.nonce];

                if (!string.IsNullOrEmpty(iss) &&
                    !string.IsNullOrEmpty(aud) &&
                    !string.IsNullOrEmpty(response_type) &&
                    !string.IsNullOrEmpty(scopes) &&
                    !string.IsNullOrEmpty(state) &&
                    !string.IsNullOrEmpty(nonce))
                {
                    // OK
                    return true;
                }
                else
                {
                    // 必須項目の不足
                    return true;
                }
            }
            else
            {
                // JWTの署名検証に失敗
                return false;
            }
        }
        #endregion

        #endregion

#if NET45 || NET46
#else
        #region FAPI CIBA

        // - OpenID Connect Client Initiated Backchannel Authentication Flow - Core 1.0 draft-01
        //   https://openid.net/specs/openid-client-initiated-backchannel-authentication-core-1_0.html
        // - Financial-grade API: Client Initiated Backchannel Authentication Profile
        //   https://openid.net/specs/openid-financial-api-ciba-ID1.html

        #region Create
        /// <summary>CreateCiba</summary>
        /// <param name="iss">string</param>
        /// <param name="aud">string</param>
        /// <param name="exp">string</param>
        /// <param name="nbf">string</param>
        /// <param name="scopes">string</param>
        /// <param name="client_notification_token">string</param>
        /// <param name="binding_message">string</param>
        /// <param name="user_code">string</param>
        /// <param name="requested_expiry">string</param>
        /// <param name="login_hint">string</param>
        /// <param name="requestContextAndIntent">Dictionary(string, object)</param>
        /// <param name="jwkPrivateKey">ES256用のJWK秘密鍵</param>
        /// <returns>RequestObject</returns>
        public static string CreateCiba(
            string iss, string aud, string exp, string nbf, string scopes,
            string client_notification_token, string binding_message,
            string user_code, string requested_expiry, string login_hint,
            Dictionary<string, object> requestContextAndIntent, string jwkPrivateKey)
        {   
            EccPrivateKeyConverter epkc = new EccPrivateKeyConverter();
            return RequestObject.CreateCiba(
                iss, aud, exp, nbf, scopes, 
                client_notification_token, binding_message,
                user_code, requested_expiry, login_hint,
                requestContextAndIntent, epkc.JwkToParam(jwkPrivateKey));
        }

        /// <summary>CreateCiba</summary>
        /// <param name="iss">string</param>
        /// <param name="aud">string</param>
        /// <param name="exp">string</param>
        /// <param name="nbf">string</param>
        /// <param name="scopes">string</param>
        /// <param name="client_notification_token">string</param>
        /// <param name="binding_message">string</param>
        /// <param name="user_code">string</param>
        /// <param name="requested_expiry">string</param>
        /// <param name="login_hint">string</param>
        /// <param name="requestContextAndIntent">Dictionary(string, object)</param>
        /// <param name="ecPrivateKey">ES256用のECParameters秘密鍵</param>
        /// <returns>RequestObject</returns>
        public static string CreateCiba(
            string iss, string aud, string exp, string nbf, string scopes,
            string client_notification_token, string binding_message,
            string user_code, string requested_expiry, string login_hint,
            Dictionary<string, object> requestContextAndIntent, ECParameters ecPrivateKey)
        {
            string json = "";

            #region ClaimSetの生成

            Dictionary<string, object> requestObjectClaimSet = new Dictionary<string, object>();

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.iss, iss); // client_id
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.aud, aud); // ROS EndPointのuri。
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.exp, exp);
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.nbf, nbf);
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.jti, new Guid().ToString());
            
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.scope, scopes);
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.client_notification_token, client_notification_token);
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.binding_message, binding_message);
            
            if (!string.IsNullOrEmpty(user_code))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.user_code, user_code);
            if (!string.IsNullOrEmpty(requested_expiry))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.requested_expiry, requested_expiry);

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.login_hint, login_hint);

            if (requestContextAndIntent != null)
            {
                foreach (string key in requestContextAndIntent.Keys)
                {
                    requestObjectClaimSet.Add(key, requestContextAndIntent[key]);
                }   
            }   

            json = JsonConvert.SerializeObject(requestObjectClaimSet);

            #endregion

            #region JWT化

            JWS_ES256_Param jwtES256 = new JWS_ES256_Param(ecPrivateKey, true);
            return jwtES256.Create(json);

            #endregion
        }
        #endregion

        #region Verify
        /// <summary>VerifyCiba</summary>
        /// <param name="ro">string</param>
        /// <param name="iss">string</param>
        /// <param name="jwkPublicKey">ES256用のJWK公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool VerifyCiba(string ro, out string iss, string jwkPublicKey)
        {
            EccPublicKeyConverter epkc = new EccPublicKeyConverter();
            return RequestObject.VerifyCiba(ro, out iss,
                epkc.JwkToParam(jwkPublicKey));
        }

        /// <summary>VerifyCiba</summary>
        /// <param name="ro">string</param>
        /// <param name="iss">string</param>
        /// <param name="ecPublicKey">ES256用のECParameters公開鍵</param>
        /// <returns>検証結果</returns>
        public static bool VerifyCiba(string ro, out string iss, ECParameters ecPublicKey)
        {
            iss = "";
            string aud = "";
            string exp = "";
            string nbf = "";
            string scopes = "";
            string client_notification_token = "";
            string binding_message = "";
            string login_hint = "";

            JWS_ES256_Param jwtES256 = new JWS_ES256_Param(ecPublicKey, false);

            if (jwtES256.Verify(ro))
            {
                string jwtPayload = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(ro.Split('.')[1]), CustomEncode.UTF_8);

                JObject jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                iss = (string)jobj[OAuth2AndOIDCConst.iss];
                aud = (string)jobj[OAuth2AndOIDCConst.aud];
                exp = (string)jobj[OAuth2AndOIDCConst.exp];
                nbf = (string)jobj[OAuth2AndOIDCConst.nbf];
                scopes = (string)jobj[OAuth2AndOIDCConst.scope];
                client_notification_token = (string)jobj[OAuth2AndOIDCConst.client_notification_token];
                binding_message = (string)jobj[OAuth2AndOIDCConst.binding_message];
                login_hint = (string)jobj[OAuth2AndOIDCConst.login_hint];

                //if (...requestContextAndIntent

                if (!string.IsNullOrEmpty(iss) &&
                    !string.IsNullOrEmpty(aud) &&
                    !string.IsNullOrEmpty(exp) &&
                    !string.IsNullOrEmpty(nbf) &&
                    !string.IsNullOrEmpty(scopes) &&
                    !string.IsNullOrEmpty(client_notification_token) &&
                    !string.IsNullOrEmpty(binding_message) &&
                    !string.IsNullOrEmpty(login_hint))
                {
                    // OK
                    return true;
                }
                else
                {
                    // 必須項目の不足
                    return true;
                }
            }
            else
            {
                // JWTの署名検証に失敗
                return false;
            }
        }
        #endregion

        #endregion
#endif
    }
}

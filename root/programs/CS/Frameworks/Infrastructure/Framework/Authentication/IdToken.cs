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
//* クラス名        ：IdToken
//* クラス日本語名  ：IdToken
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/08/22  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

#if NETSTD
using Microsoft.AspNetCore.WebUtilities;
#else
using Microsoft.Owin.Security.DataHandler.Encoder;
#endif

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Security;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>
    /// HashClaimのタイプ
    /// ・None
    /// ・at_hash
    /// ・c_hash
    /// ・Both
    /// </summary>
    public enum HashClaimType
    {
        /// <summary>HashClaim無し</summary>
        None = 0x000,
        
        /// <summary>
        /// at_hashを付与(OIDC Hybrid)
        /// id_token token
        /// </summary>
        AtHash = 0x001,

        /// <summary>
        /// c_hashを付与(OIDC Hybrid)
        /// code id_token
        /// </summary>
        CHash = 0x002,

        /// <summary>
        /// s_hashを付与(FAPI2)
        /// </summary>
        SHash = 0x004
    }

    /// <summary>
    /// OIDCのIdToken処理
    /// </summary>
    public class IdToken
    {
        /// <summary>
        /// ChangeToIdTokenFromAccessToken
        ///   OIDC対応（AccessTokenからIdTokenを生成）
        /// </summary>
        /// <param name="access_token">string</param>
        /// <param name="code">string</param>
        /// <param name="state">string</param>
        /// <param name="hct">HashClaimType</param>
        /// <param name="pfxFilePath">string</param>
        /// <param name="pfxPassword">string</param>
        /// <returns>IdToken</returns>
        /// <remarks>
        /// OIDC対応
        /// </remarks>

        public static string ChangeToIdTokenFromAccessToken(
            string access_token, string code, string state, HashClaimType hct, string pfxFilePath, string pfxPassword)
        {
            if (access_token.Contains("."))
            {
                string[] temp = access_token.Split('.');
                string json = CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(temp[1]), CustomEncode.UTF_8);
                Dictionary<string, object> authTokenClaimSet = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                // ・access_tokenがJWTで、payloadに"nonce" and "scope=openidクレームが存在する場合、
                if (authTokenClaimSet.ContainsKey(OAuth2AndOIDCConst.nonce)
                    && authTokenClaimSet.ContainsKey(OAuth2AndOIDCConst.scopes))
                {
                    JArray scopes = (JArray)authTokenClaimSet[OAuth2AndOIDCConst.scopes];

                    // ・OpenID Connect : response_type=codeに対応する。
                    if (scopes.Any(x => x.ToString() == OAuth2AndOIDCConst.Scope_Openid))
                    {
                        //・payloadからscopeを削除する。
                        authTokenClaimSet.Remove(OAuth2AndOIDCConst.scopes);

                        //・payloadにat_hash, c_hash, s_hashを追加する。

                        if (hct.HasFlag(HashClaimType.AtHash))
                        {
                            // at_hash
                            authTokenClaimSet.Add(
                                OAuth2AndOIDCConst.at_hash,
                                IdToken.CreateHash(access_token));
                        }

                        if (hct.HasFlag(HashClaimType.CHash))
                        {
                            // c_hash
                            authTokenClaimSet.Add(
                                OAuth2AndOIDCConst.c_hash,
                                IdToken.CreateHash(code));
                        }

                        if (hct.HasFlag(HashClaimType.SHash))
                        {
                            // s_hash
                            authTokenClaimSet.Add(
                                OAuth2AndOIDCConst.s_hash,
                                IdToken.CreateHash(state));
                        }


                        //・編集したpayloadを再度JWTとして署名する。
                        string newPayload = JsonConvert.SerializeObject(authTokenClaimSet);
                        JWS_RS256_X509 jwsRS256 = null;

                        // 署名
                        jwsRS256 = new JWS_RS256_X509(pfxFilePath, pfxPassword,
                            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

                        // ヘッダ
                        JWS_Header jwsHeader =
                            JsonConvert.DeserializeObject<JWS_Header>(
                                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(temp[0]), CustomEncode.UTF_8));

                        if (!string.IsNullOrEmpty(jwsHeader.jku)
                            && !string.IsNullOrEmpty(jwsHeader.kid))
                        {
                            jwsRS256.JWSHeader.jku = jwsHeader.jku;
                            jwsRS256.JWSHeader.kid = jwsHeader.kid;
                        }

                        return jwsRS256.Create(newPayload);

                        //// 検証
                        //jwsRS256 = new JWS_RS256_X509(OAuth2AndOIDCParams.RS256Cer, pfxPassword,
                        //    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

                        //if (jwsRS256.Verify(id_token))
                        //{
                        //    // 検証できた。
                        //    return id_token;
                        //}
                        //else
                        //{
                        //    // 検証できなかった。
                        //}
                    }
                    else
                    {
                        // OIDCでない。
                    }
                }
                else
                {
                    // OIDCでない。
                }
            }
            else
            {
                // JWTでない。
            }

            return "";
        }

        /// <summary>汎用認証サイトの発行したIdTokenを検証する。</summary>
        /// <param name="idToken">string</param>
        /// <param name="access_token">string</param>
        /// <param name="code">string</param>
        /// <param name="state">string</param>
        /// <param name="sub">string</param>
        /// <param name="nonce">string</param>
        /// <param name="jobj">JObject</param>
        /// <returns>検証結果</returns>
        public static bool Verify(string idToken,
            string access_token, string code, string state,
            out string sub, out string nonce, out JObject jobj)
        {
            sub = "";
            nonce = "";
            jobj = null;

            // 検証
            JWS_RS256 jwsRS256 = null;

            // 証明書を使用するか、Jwkを使用するか判定
            JWS_Header jwsHeader = JsonConvert.DeserializeObject<JWS_Header>(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(idToken.Split('.')[0]), CustomEncode.UTF_8));

            if (string.IsNullOrEmpty(jwsHeader.jku)
                || string.IsNullOrEmpty(jwsHeader.kid))
            {
                // 証明書を使用
                jwsRS256 = new JWS_RS256_X509(OAuth2AndOIDCParams.RS256Cer, "");
            }
            else
            {
                // 読取
                Dictionary<string, string> jwkObject = JwkSetStore.GetInstance().GetJwkObject(jwsHeader.kid);

                // チェック
                if (jwkObject == null)
                {
                    // 書込
                    jwkObject = JwkSetStore.GetInstance().SetJwkSetObject(jwsHeader.jku, jwsHeader.kid);
                }

                // チェック
                if (jwkObject == null)
                {
                    // 証明書を使用
                    jwsRS256 = new JWS_RS256_X509(OAuth2AndOIDCParams.RS256Cer, "");
                }
                else
                {
                    // Jwkを使用
                    jwsRS256 = new JWS_RS256_Param(
                        RS256_KeyConverter.JwkToProvider(
                            JsonConvert.SerializeObject(jwkObject)).ExportParameters(false));
                }
            }

            // JWS検証
            if (jwsRS256.Verify(idToken))
            {
#if NETSTD
                string jwtPayload = Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(jwtAccessToken.Split('.')[1]));
#else
                Base64UrlTextEncoder base64UrlEncoder = new Base64UrlTextEncoder();
                string jwtPayload = Encoding.UTF8.GetString(base64UrlEncoder.Decode(idToken.Split('.')[1]));
#endif
                jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                #region クレーム検証

                // out
                sub = (string)jobj[OAuth2AndOIDCConst.sub];
                nonce = (string)jobj[OAuth2AndOIDCConst.nonce];

                string iss = (string)jobj[OAuth2AndOIDCConst.iss];
                string aud = (string)jobj[OAuth2AndOIDCConst.aud];
                //string iat = (string)jobj[OAuth2AndOIDCConst.iat];
                string exp = (string)jobj[OAuth2AndOIDCConst.exp];

                #region ハッシュ・クレーム検証

                string at_hash = (string)jobj[OAuth2AndOIDCConst.at_hash];
                string c_hash = (string)jobj[OAuth2AndOIDCConst.c_hash];
                string s_hash = (string)jobj[OAuth2AndOIDCConst.s_hash];

                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(at_hash))
                {
                    if (!IdToken.VerifyHash(code, at_hash))
                    {
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(c_hash))
                {
                    if (!IdToken.VerifyHash(code, c_hash))
                    {
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(c_hash))
                {
                    if (!IdToken.VerifyHash(code, c_hash))
                    {
                        return false;
                    }
                }

                #endregion

                long unixTimeSeconds = 0;
#if NET45
                unixTimeSeconds = PubCmnFunction.ToUnixTime(DateTimeOffset.Now);
#else
                unixTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
#endif

                if (iss == OAuth2AndOIDCParams.Isser &&
                    long.Parse(exp) >= unixTimeSeconds)
                {
                    if (aud == OAuth2AndOIDCParams.ClientID)
                    {
                        // OAuth2 Clientバージョンの実装で成功
                        return true;
                    }
                    else if (OAuth2AndOIDCParams.ClientIDs.Any(x => x == aud))
                    {
                        // OAuth2 ResourcesServerバージョンの実装で成功
                        return true;
                    }
                    else
                    {
                        // JWTの内容検証に失敗
                    }
                }
                else
                {
                    // JWTの内容検証に失敗
                }

                #endregion
            }
            else
            {
                // JWTの署名検証に失敗
            }

            // 認証に失敗
            return false;
        }

        #region at_hash, c_hash, s_hash

        /// <summary>
        /// at_hash, c_hash, s_hashを作成
        /// （SHA256→RS256，ES256対応可能）
        /// </summary>
        /// <param name="input">string</param>
        /// <returns>hash</returns>
        public static string CreateHash(string input)
        {
            // ID Token の JOSE Header にある
            // alg Header Parameterのアルゴリズムで使用されるハッシュアルゴリズムを用い、
            // input(access_token や code) のASCII オクテット列からハッシュ値を求め、
            byte[] bytes = GetHash.GetHashBytes(
                CustomEncode.StringToByte(input, CustomEncode.us_ascii),
                EnumHashAlgorithm.SHA256Managed);

            // 左半分を base64url エンコードした値。
            return CustomEncode.ToBase64UrlString(
                PubCmnFunction.ShortenByteArray(bytes, (bytes.Length / 2)));
        }

        /// <summary>
        /// at_hash, c_hash, s_hashの検証
        /// （SHA256→RS256，ES256対応可能）
        /// </summary>
        /// <param name="input">string</param>
        /// <param name="hash">string</param>
        /// <returns>検証結果</returns>
        public static bool VerifyHash(string input, string hash)
        {
            return (hash == IdToken.CreateHash(input));
        }

        #endregion
    }
}

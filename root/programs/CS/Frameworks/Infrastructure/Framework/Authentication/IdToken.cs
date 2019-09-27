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
//*  2018/11/28  西野 大介         証明書 & Jwk対応 + jkuチェック対応の追加
//**********************************************************************************

using System;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Jwt;

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
        #region Create
        // AuthZに実装（パラメタ体系が違うため）
        #endregion

        #region Verify

        /// <summary>汎用認証サイトの発行したIdTokenを検証する。</summary>
        /// <param name="id_token">string</param>
        /// <param name="access_token">string</param>
        /// <param name="code">string</param>
        /// <param name="state">string</param>
        /// <param name="sub">out string</param>
        /// <param name="nonce">out string</param>
        /// <param name="jobj">out JObject</param>
        /// <returns>検証結果</returns>
        public static bool Verify(string id_token,
            string access_token, string code, string state,
            out string sub, out string nonce, out JObject jobj)
        {
            sub = "";
            nonce = "";
            jobj = null;

            // JWS検証
            string jwtPayload = "";
            if (CmnJwtToken.Verify(id_token, out jwtPayload))
            {
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

                // at_hash
                string at_hash = (string)jobj[OAuth2AndOIDCConst.at_hash];
                if (!string.IsNullOrEmpty(access_token) && !string.IsNullOrEmpty(at_hash))
                {
                    if (!IdToken.VerifyHash(access_token, at_hash))
                    {
                        return false;
                    }
                }

                // c_hash
                string c_hash = (string)jobj[OAuth2AndOIDCConst.c_hash];
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(c_hash))
                {
                    if (!IdToken.VerifyHash(code, c_hash))
                    {
                        return false;
                    }
                }

                // s_hash
                string s_hash = (string)jobj[OAuth2AndOIDCConst.s_hash];
                if (!string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(s_hash))
                {
                    if (!IdToken.VerifyHash(state, s_hash))
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

                if (iss == CmnClientParams.Isser &&
                    long.Parse(exp) >= unixTimeSeconds)
                {
                    if (string.IsNullOrEmpty(OAuth2AndOIDCParams.JwkSetFilePath))
                    {
                        // Client側
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
                        // AuthZ側（検証用カバレッジ
                        // OAuth2 AuthZバージョンの実装で成功
                        return true;
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

        #endregion

        #region at_hash, c_hash, s_hash

        /// <summary>
        /// at_hash, c_hash, s_hashを作成
        /// （SHA256→HS256，RS256，ES256対応可能）
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
                EnumHashAlgorithm.SHA256_M);

            // 左半分を base64url エンコードした値。
            return CustomEncode.ToBase64UrlString(
                ArrayOperator.ShortenByteArray(bytes, (bytes.Length / 2)));
        }

        /// <summary>
        /// at_hash, c_hash, s_hashの検証
        /// （SHA256→HS256，RS256，ES256対応可能）
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

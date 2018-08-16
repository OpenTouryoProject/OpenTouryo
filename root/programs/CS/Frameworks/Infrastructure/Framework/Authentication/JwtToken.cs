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
//* クラス名        ：JwtToken
//* クラス日本語名  ：JwtToken
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/09/07  西野 大介         新規作成
//*  2017/11/29  西野 大介         DateTimeOffset.ToUnixTimeSecondsの前方互換メソッドの使用
//*  2018/03/28  西野 大介         .NET Standard対応で、幾らか、I/F変更あり。
//**********************************************************************************

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

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

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>
    /// OAuth2やOIDC関連のJwtToken処理
    /// </summary>
    public class JwtToken
    {
        /// <summary>汎用認証サイトの発行したJWT形式のTokenを検証する。</summary>
        /// <param name="jwtAccessToken">
        /// JWT形式のTokenで以下の項目が必要
        ///  - iss
        ///  - aud
        ///  - iat
        ///  - exp
        ///  - sub
        ///  - roles  (option)
        ///  - scopes (option)
        ///  - その他 (option)
        /// </param>
        /// <param name="sub">string</param>
        /// <param name="roles">List(string)</param>
        /// <param name="scopes">List(string)</param>
        /// <param name="jobj">JObject</param>
        /// <returns>検証結果</returns>
        public static bool Verify(string jwtAccessToken,
            out string sub, out List<string> roles, out List<string> scopes, out JObject jobj)
        {
            sub = "";
            roles =  new List<string>();
            scopes = new List<string>();
            jobj = null;

            // 検証
            JWS_RS256 jwsRS256 = null;

            // 証明書を使用するか、Jwkを使用するか判定
            JWS_Header jwsHeader = JsonConvert.DeserializeObject<JWS_Header>(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(jwtAccessToken.Split('.')[0]), CustomEncode.UTF_8));

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

            if (jwsRS256.Verify(jwtAccessToken))
            {
#if NETSTD
                string jwtPayload = Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(jwtAccessToken.Split('.')[1]));
#else
                Base64UrlTextEncoder base64UrlEncoder = new Base64UrlTextEncoder();
                string jwtPayload = Encoding.UTF8.GetString(base64UrlEncoder.Decode(jwtAccessToken.Split('.')[1]));
#endif
                jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                //string nonce = (string)jobj[OAuth2AndOIDCConst.nonce];
                string iss = (string)jobj[OAuth2AndOIDCConst.iss];
                string aud = (string)jobj[OAuth2AndOIDCConst.aud];
                //string iat = (string)jobj[OAuth2AndOIDCConst.iat];
                string exp = (string)jobj[OAuth2AndOIDCConst.exp];

                sub = (string)jobj[OAuth2AndOIDCConst.sub];

                if (jobj[OAuth2AndOIDCConst.Scope_Roles] != null)
                {
                    roles = JsonConvert.DeserializeObject<List<string>>(jobj[OAuth2AndOIDCConst.Scope_Roles].ToString());
                }
                if (jobj[OAuth2AndOIDCConst.scopes] != null)
                {
                    scopes = JsonConvert.DeserializeObject<List<string>>(jobj[OAuth2AndOIDCConst.scopes].ToString());
                }

                long unixTimeSeconds = 0;
#if NET45
                unixTimeSeconds = PubCmnFunction.ToUnixTime(DateTimeOffset.Now);
#else
                unixTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
#endif

                if (iss == OAuth2AndOIDCParams.Isser &&
                    aud　== OAuth2AndOIDCParams.ClientID &&
                    long.Parse(exp) >= unixTimeSeconds)
                {
                    // 認証に成功（OAuth2 Clientバージョンの実装）
                    return true;
                }
                else if (iss == OAuth2AndOIDCParams.Isser &&
                        OAuth2AndOIDCParams.ClientIDs.Any(x => x == aud) &&
                        long.Parse(exp) >= unixTimeSeconds)
                {
                    // 認証に成功（OAuth2 ResourcesServerバージョンの実装）
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

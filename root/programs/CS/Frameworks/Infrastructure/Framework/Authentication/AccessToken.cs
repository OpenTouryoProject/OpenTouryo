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
//* クラス名        ：AccessToken
//* クラス日本語名  ：AccessToken
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/09/07  西野 大介         新規作成
//*  2017/11/29  西野 大介         DateTimeOffset.ToUnixTimeSecondsの前方互換メソッドの使用
//*  2018/03/28  西野 大介         .NET Standard対応で、幾らか、I/F変更あり。
//*  2018/11/28  西野 大介         証明書 & Jwk対応 + jkuチェック対応の追加
//*  2018/11/28  西野 大介         リネーム（JwtToken -> AccessToken）
//**********************************************************************************

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>
    /// OAuth2やOIDC関連のAccessToken処理
    /// </summary>
    public class AccessToken
    {
        #region Create
        // AuthZに実装
        #endregion

        #region Verify
        /// <summary>汎用認証サイトの発行したAccessTokenを検証する。</summary>
        /// <param name="access_token">
        /// AccessTokenで以下の項目が必要
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
        public static bool Verify(string access_token,
            out string sub, out List<string> roles, out List<string> scopes, out JObject jobj)
        {
            sub = "";
            roles =  new List<string>();
            scopes = new List<string>();
            jobj = null;

            // JWS検証
            string jwtPayload = "";
            if (CmnJwtToken.Verify(access_token, out jwtPayload))
            {
                jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                #region クレーム検証

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
    }
}

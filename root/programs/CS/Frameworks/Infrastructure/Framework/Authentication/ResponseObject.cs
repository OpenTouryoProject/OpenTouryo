
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
//* クラス名        ：ResponseObject
//* クラス日本語名  ：ResponseObject
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/26  西野 大介         新規作成
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
    /// OAuth2やOIDC(FAPI2)関連のResponseObject処理
    /// 正確には、JWT Secured Authorization Response Mode for OAuth 2.0 (JARM)
    /// </summary>
    public class ResponseObject
    {
        #region Create
        // AuthZに実装（パラメタ体系が違うため）
        #endregion

        #region Verify
        /// <summary>汎用認証サイトの発行したResponseObjectを検証する。</summary>
        /// <param name="response">string</param>
        /// <param name="jobj">out JObject</param>
        /// <returns>検証結果</returns>
        public static bool Verify(string response, out JObject jobj)
        {
            // JWS検証
            string jwtPayload = "";
            if (CmnJwtToken.Verify(response, out jwtPayload))
            {
                jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                #region クレーム検証

                if (jobj.ContainsKey(OAuth2AndOIDCConst.error))
                {
                    // errorの場合
                    return true;
                }
                else
                {
                    // errorでない場合

                    string iss = (string)jobj[OAuth2AndOIDCConst.iss];
                    string aud = (string)jobj[OAuth2AndOIDCConst.aud];
                    //string iat = (string)jobj[OAuth2AndOIDCConst.iat];
                    string exp = (string)jobj[OAuth2AndOIDCConst.exp];

                    long unixTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

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
                }

                #endregion
            }
            else
            {
                // JWTの署名検証に失敗
                jobj = null;
            }

            // 認証に失敗
            return false;
        }
        #endregion
    }
}

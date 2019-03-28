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
//* クラス名        ：AuthenticationHeader
//* クラス日本語名  ：AuthenticationHeader
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/12/26  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Net.Http.Headers;
using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>AuthenticationHeader</summary>
    public class AuthenticationHeader
    {
        /// <summary>GetCredentials(Basic)</summary>
        /// <param name="authHeader">string</param>
        /// <param name="client_id">string</param>
        /// <param name="client_secret">string</param>
        /// <returns>bool</returns>
        public static bool GetCredentials(string authHeader, out string client_id, out string client_secret)
        {
            client_id = "";
            client_secret = "";
            string[] credentials = null;

            if (AuthenticationHeader.GetCredentials(authHeader, out credentials) == OAuth2AndOIDCConst.Basic)
            {
                // Length == 1 の ケースもサポート
                if (credentials.Length == 1)
                {
                    client_id = credentials[0];
                    return true;
                }
                else if (credentials.Length == 2)
                {
                    client_id = credentials[0];
                    client_secret = credentials[1];
                    return true;
                }
            }

            return false;
        }

        /// <summary>GetCredentials(Bearer)</summary>
        /// <param name="authHeader">string</param>
        /// <param name="bearerToken">string</param>
        /// <returns>bool</returns>
        public static bool GetCredentials(string authHeader, out string bearerToken)
        {
            bearerToken = "";
            string[] credentials = null;

            if (AuthenticationHeader.GetCredentials(authHeader, out credentials) == OAuth2AndOIDCConst.Bearer)
            {
                if (credentials.Length == 1)
                {
                    bearerToken = credentials[0];
                    return true;
                }
            }

            return false;
        }

        /// <summary>GetCredentials</summary>
        /// <param name="authHeader">string</param>
        /// <param name="credentials">string[]</param>
        /// <returns>AuthenticationScheme</returns>
        public static string GetCredentials(string authHeader, out string[] credentials)
        {
            if (!string.IsNullOrEmpty(authHeader))
            {
                string[] temp = authHeader.Split(' ');

                if (temp[0] == OAuth2AndOIDCConst.Basic)
                {
                    credentials = CustomEncode.ByteToString(
                        CustomEncode.FromBase64String(temp[1]), CustomEncode.us_ascii).Split(':');

                    return OAuth2AndOIDCConst.Basic;
                }
                else if (temp[0] == OAuth2AndOIDCConst.Bearer)
                {
                    credentials = new string[] { temp[1] };

                    return OAuth2AndOIDCConst.Bearer;
                }
            }

            credentials = new string[] { };
            return "";
        }

        /// <summary>CreateBasicAuthenticationHeaderValue</summary>
        /// <param name="id">string</param>
        /// <param name="secret">string</param>
        /// <returns>AuthenticationHeaderValue</returns>
        public static AuthenticationHeaderValue CreateBasicAuthenticationHeaderValue(string id, string secret)
        {
            // id + x509 のパターンをサポート
            if (!string.IsNullOrEmpty(id)) // && !string.IsNullOrEmpty(secret))
            {
                return new AuthenticationHeaderValue(
                    OAuth2AndOIDCConst.Basic,
                    CustomEncode.ToBase64String(CustomEncode.StringToByte(
                        string.Format("{0}:{1}", id, secret), CustomEncode.us_ascii)));
            }
            else
            {
                return null;
            }
        }

        /// <summary>CreateBearerAuthenticationHeaderValue</summary>
        /// <param name="accessToken">string</param>
        /// <returns>AuthenticationHeaderValue</returns>
        public static AuthenticationHeaderValue CreateBearerAuthenticationHeaderValue(string accessToken)
        {
            return new AuthenticationHeaderValue(OAuth2AndOIDCConst.Bearer, accessToken);
        }
    }
}

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
//*  2026/09/12  玄人 幸道         方式の後ろに値の無い Authorization ヘッダで
//*                                例外になっていたのを修正
//*  2026/09/25  玄人 幸道         Basic認証の資格情報を、RFC 6749 §2.3.1 に従って
//*                                符号化・復号するようにした
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

        /// <summary>GetCredentials(Basic、RFC 6749 §2.3.1 の復号あり)</summary>
        /// <param name="authHeader">string</param>
        /// <param name="client_id">string（復号後）</param>
        /// <param name="client_secret">string（復号後）</param>
        /// <param name="rawClientId">string（復号前＝ヘッダに載っていたまま）</param>
        /// <param name="rawClientSecret">string（復号前＝ヘッダに載っていたまま）</param>
        /// <returns>bool</returns>
        /// <remarks>
        /// RFC 6749 §2.3.1 は、client_idとclient_secretを
        /// application/x-www-form-urlencodedで符号化してからBase64にすることを求めている。
        /// このため、受け側は復号してから照合する。
        /// ただし、符号化しないクライアントも在るため、復号前の値も返す。
        /// 両方と照合すれば、どちらのクライアントも受けられる。
        /// </remarks>
        public static bool GetCredentials(string authHeader,
            out string client_id, out string client_secret,
            out string rawClientId, out string rawClientSecret)
        {
            client_id = "";
            client_secret = "";

            if (AuthenticationHeader.GetCredentials(authHeader, out rawClientId, out rawClientSecret))
            {
                client_id = CustomEncode.UrlDecode(rawClientId);
                client_secret = CustomEncode.UrlDecode(rawClientSecret);

                return true;
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
        /// <remarks>
        /// 資格情報を取り出せない場合は、空の配列と "" を返す。
        /// 呼び出し側は、それを受けて 401 を返せる。
        /// </remarks>
        public static string GetCredentials(string authHeader, out string[] credentials)
        {
            if (!string.IsNullOrEmpty(authHeader))
            {
                string[] temp = authHeader.Split(' ');

                // 方式の後ろに値が無い（"Bearer" や "Bearer " など）場合は、
                // 資格情報の無い要求として扱う。
                // 以前は temp[1] を確かめずに読み、IndexOutOfRangeException になっていた。
                if (temp.Length >= 2 && !string.IsNullOrEmpty(temp[1]))
                {
                    if (temp[0] == OAuth2AndOIDCConst.Basic)
                    {
                        try
                        {
                            credentials = CustomEncode.ByteToString(
                                CustomEncode.FromBase64String(temp[1]), CustomEncode.us_ascii).Split(':');

                            return OAuth2AndOIDCConst.Basic;
                        }
                        catch (FormatException)
                        {
                            // Base64 として読めない。資格情報が無いものとして扱う。
                            //
                            // **ここで throw すると、呼び出し側は 401 ではなく 500 を返すことになり、
                            //   クライアントは「サーバの不具合」と「認証の失敗」を区別できない。**
                            //
                            // Convert.TryFromBase64String は net48 に無いため、ここは try/catch で受ける
                            // （このファイルは net48 / net10.0 の共通コード）。
                        }
                    }
                    else if (temp[0] == OAuth2AndOIDCConst.Bearer)
                    {
                        credentials = new string[] { temp[1] };

                        return OAuth2AndOIDCConst.Bearer;
                    }
                }
            }

            credentials = new string[] { };
            return "";
        }

        /// <summary>CreateBasicAuthenticationHeaderValue</summary>
        /// <param name="id">string</param>
        /// <param name="secret">string</param>
        /// <returns>AuthenticationHeaderValue</returns>
        /// <remarks>
        /// RFC 6749 §2.3.1 は、idとsecretを
        /// application/x-www-form-urlencodedで符号化してから ":" で連結し、
        /// Base64にすることを求めている。
        /// 英数字だけなら符号化しても同じ文字列なので、見た目は変わらない。
        /// 
        /// 空白は "+" ではなく "%20" になる（CustomEncode.UrlEncodeがUri.EscapeDataStringのため）。
        /// RFC 6749 Appendix B の例は "+" だが、"%20" はフォーム形式の復号器でも
        /// 素朴なURI復号器（Uri.UnescapeDataString）でも空白に戻るのに対し、
        /// "+" は後者では "+" のまま残る。壊れにくい方を選んでいる。
        /// 字面どおりに寄せるなら、WebUtility.UrlEncodeを使う。
        /// 
        /// 変わるのは "+" "/" "=" "%" ":" などを含む場合で、
        /// 受け側は復号してから照合する必要がある
        /// （復号ありのGetCredentialsを参照。復号しないサーバも在るため、
        /// 復号前の値とも照合する形にしておくと、どちらとも繋がる）。
        /// </remarks>
        public static AuthenticationHeaderValue CreateBasicAuthenticationHeaderValue(string id, string secret)
        {
            // id + x509 のパターンをサポート
            if (!string.IsNullOrEmpty(id)) // && !string.IsNullOrEmpty(secret))
            {
                return new AuthenticationHeaderValue(
                    OAuth2AndOIDCConst.Basic,
                    CustomEncode.ToBase64String(CustomEncode.StringToByte(
                        string.Format("{0}:{1}",
                            CustomEncode.UrlEncode(id),
                            CustomEncode.UrlEncode(secret ?? "")), CustomEncode.us_ascii)));
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

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
//* クラス名        ：CookieExtensions
//* クラス日本語名  ：System.Web.HttpCookieのポーティング用クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成
//*  2018/03/29  西野 大介         .NET Standard対応で、HttpCookieのポーティング
//**********************************************************************************

using System;
using Microsoft.AspNetCore.Http;

namespace Touryo.Infrastructure.Framework.StdMigration
{
    /// <summary>System.Web.HttpCookieのポーティング用クラス</summary>
    public static class CookieExtensions
    {
        /// <summary>Get the cookie拡張メソッド</summary>
        /// <param name="requestCookies">拡張</param>
        /// <param name="key">string</param>
        /// <returns>string value</returns>
        public static string Get(this IRequestCookieCollection requestCookies, string key)
        {
            return requestCookies[key];
        }

        /// <summary>Set the cookie拡張メソッド</summary>
        /// <param name="responseCookies">拡張</param>
        /// <param name="key">string</param>
        /// <param name="value">string</param>
        public static void Set(this IResponseCookies responseCookies, string key, string value)
        {
            responseCookies.Delete(key);
            responseCookies.Append(key, value);
        }

        /// <summary>Set the cookie拡張メソッド</summary>
        /// <param name="responseCookies">拡張</param>
        /// <param name="key">string</param>
        /// <param name="value">string</param>
        /// <param name="expireTime">int</param>
        public static void Set(this IResponseCookies responseCookies, string key, string value, int expireTime)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddMinutes(expireTime);

            responseCookies.Delete(key);
            responseCookies.Append(key, value, option);
        }

        /// <summary>Set the cookie拡張メソッド</summary>
        /// <param name="responseCookies">拡張</param>
        /// <param name="key">string</param>
        /// <param name="value">string</param>
        /// <param name="option">CookieOptions</param>
        public static void Set(this IResponseCookies responseCookies, string key, string value, CookieOptions option)
        {
            responseCookies.Delete(key);
            responseCookies.Append(key, value, option);
        }

        /// <summary>Delete the key拡張メソッド</summary>
        /// <param name="responseCookies">拡張</param>
        /// <param name="key">string</param>
        public static void Remove(this IResponseCookies responseCookies, string key)
        {
            responseCookies.Delete(key);
        }
    }
}

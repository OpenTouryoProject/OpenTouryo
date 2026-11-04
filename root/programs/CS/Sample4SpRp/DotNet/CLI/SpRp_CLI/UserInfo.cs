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
//* クラス名        ：UserInfo
//* クラス日本語名  ：UserInfo（OpenID Connect Core 1.0 §5.3）を引いて表示する
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/04  玄人 幸道         新規作成（#649）
//**********************************************************************************

using System;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Framework.Authentication;

namespace SpRp_CLI
{
    /// <summary>UserInfo を引いて表示する</summary>
    /// <remarks>
    /// **エラーの応答も JSON で返る**（`insufficient_scope` など。RFC 6750 §3.1）。
    /// **表示するだけでは成功と区別できない**ので、`error` を見て失敗にする。
    /// </remarks>
    public class UserInfo
    {
        /// <summary>UserInfo を引いて表示する</summary>
        /// <param name="discovery">Discovery</param>
        /// <param name="access_token">access_token</param>
        /// <returns>引けたら true</returns>
        public static async Task<bool> ShowAsync(Discovery discovery, string access_token)
        {
            string json = await OAuth2AndOIDCClient
                .GetUserInfoAsync(discovery.UserInfoEndpoint, access_token);

            JObject userInfo;
            try
            {
                userInfo = JObject.Parse(json);
            }
            catch (JsonReaderException)
            {
                Console.WriteLine("NG   UserInfo の応答を JSON として読めません : " + json);
                return false;
            }

            if (userInfo.ContainsKey(OAuth2AndOIDCConst.error))
            {
                Console.WriteLine("NG   UserInfo が断られました : " + DeviceFlow.ErrorOf(userInfo));
                return false;
            }

            Console.WriteLine("OK   UserInfo を得ました。");
            Console.WriteLine(userInfo.ToString(Formatting.Indented));
            return true;
        }
    }
}

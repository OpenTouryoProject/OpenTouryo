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
//* クラス名        ：OAuth2AndOIDCParams
//* クラス日本語名  ：OAuth2とOIDCの各種パラメタ
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/09/06  西野 大介         新規作成
//**********************************************************************************

using System.Collections.Generic;

#if NETSTD
using Microsoft.Extensions.Configuration;
#else
using Newtonsoft.Json;
#endif

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>OAuth2とOIDCの各種パラメタ</summary>
    public class OAuth2AndOIDCParams
    {
        #region パラメタ関連

        /// <summary>ClientID</summary>
        public static string ClientID
        {
            get
            {
                return GetConfigParameter.GetConfigValue("OAuth2AndOidcClientID");
            }
        }

        /// <summary>ClientSecret</summary>
        public static string ClientSecret
        {
            get
            {
                return GetConfigParameter.GetConfigValue("OAuth2AndOidcSecret");
            }
        }

        /// <summary>Isser</summary>
        public static string Isser
        {
            get
            {
                return GetConfigParameter.GetConfigValue("OAuth2AndOidcIsser");
            }
        }

        /// <summary>Audience</summary>
        public static string Audience
        {
            get
            {
                return GetConfigParameter.GetConfigValue("OAuth2AndOidcAudience");
            }
        }

        /// <summary>ClientIDs</summary>
        public static List<string> ClientIDs
        {
            get
            {
#if NETSTD
                return GetConfigParameter.GetConfigSection("OAuth2AndOidcClientIDs").Get<List<string>>();
#else
                return JsonConvert.DeserializeObject<List<string>>(GetConfigParameter.GetConfigValue("OAuth2AndOidcClientIDs"));
#endif
            }
        }

        #endregion

        #region 鍵関連

        /// <summary>RS256Cer</summary>
        public static string RS256Cer
        {
            get
            {
                return GetConfigParameter.GetConfigValue("OAuth2AndOidcRS256Cer");
            }
        }

        /// <summary>ES256Cer</summary>
        public static string ES256Cer
        {
            get
            {
                return GetConfigParameter.GetConfigValue("OAuth2AndOidcES256Cer");
            }
        }

        /// <summary>OAuth2AndOidcRS256Pwd</summary>
        public static string OAuth2AndOidcRS256Pwd
        {
            get
            {
                return GetConfigParameter.GetConfigValue("OAuth2AndOidcRS256Pwd");
            }
        }

        /// <summary>OAuth2AndOidcRS256Pfx</summary>
        public static string OAuth2AndOidcRS256Pfx
        {
            get
            {
                return GetConfigParameter.GetConfigValue("OAuth2AndOidcRS256Pfx");
            }
        }

        /// <summary>JwkSetFilePath</summary>
        public static string JwkSetFilePath
        {
            get
            {
                return GetConfigParameter.GetConfigValue("JwkSetFilePath");
            }
        }

        /// <summary>JwkSetUri</summary>
        public static string JwkSetUri
        {
            get
            {
                return GetConfigParameter.GetConfigValue("JwkSetUri");
            }
        }

        /// <summary>JwkSetUpdateIntervalInSeconds</summary>
        public static int JwkSetUpdateIntervalInSeconds
        {
            get
            {
                string tmp = GetConfigParameter.GetConfigValue("JwkSetUpdateIntervalInSeconds");
                if (string.IsNullOrEmpty(tmp))
                {
                    return 10;
                }
                else
                {
                    return int.Parse(tmp);
                }
            }
        }

        #endregion
    }
}

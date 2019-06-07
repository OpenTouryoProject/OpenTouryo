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
//* クラス名        ：CmnClientParams
//* クラス日本語名  ：SAML2, OAuth2/OIDC, FAPIの各種パラメタ
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/07  西野 大介         新規作成（分割
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
    /// <summary>SAML2, OAuth2/OIDC, FAPIの各種パラメタ</summary>
    public class CmnClientParams
    {
        #region パラメタ関連

        /// <summary>Isser</summary>
        public static string Isser
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_Isser");
            }
        }

        #endregion

        #region 鍵関連

        /// <summary>RsaCer</summary>
        public static string RsaCer
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_RsaCer");
            }
        }

        /// <summary>EcdsaCer</summary>
        public static string EcdsaCer
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_EcdsaCer");
            }
        }

        /// <summary>RsaPwd</summary>
        public static string RsaPwd
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_RsaPwd");
            }
        }

        /// <summary>RsaPfx</summary>
        public static string RsaPfx
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_RsaPfx");
            }
        }

        /// <summary>ClientCertPwd</summary>
        public static string ClientCertPwd
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_ClientCertPwd");
            }
        }

        /// <summary>ClientCertPfx</summary>
        public static string ClientCertPfx
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_ClientCertPfx");
            }
        }

        #endregion
    }
}

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

        /// <summary>RsaCerFilePath</summary>
        public static string RsaCerFilePath
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_RsaCerFilePath");
            }
        }

        /// <summary>EcdsaCerFilePath</summary>
        public static string EcdsaCerFilePath
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_EcdsaCerFilePath");
            }
        }

        /// <summary>RsaPfxPassword</summary>
        public static string RsaPfxPassword
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_RsaPfxPassword");
            }
        }

        /// <summary>RsaPfxFilePath</summary>
        public static string RsaPfxFilePath
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_RsaPfxFilePath");
            }
        }

        /// <summary>ClientCertPfxPassword</summary>
        public static string ClientCertPfxPassword
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_ClientCertPfxPassword");
            }
        }

        /// <summary>ClientCertPfxFilePath</summary>
        public static string ClientCertPfxFilePath
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_ClientCertPfxFilePath");
            }
        }

        #endregion
    }
}

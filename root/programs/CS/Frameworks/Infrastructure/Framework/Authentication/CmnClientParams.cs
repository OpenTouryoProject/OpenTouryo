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
//* クラス日本語名  ：SAML2, OAuth2/OIDC, FAPIの各種Client側パラメタ
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/07  西野 大介         新規作成（分割
//*  2020/08/04  西野 大介         AuthZ(N) Uirの追加
//*  2026/10/22  玄人 幸道         相手（OP / IdP）のメタデータの所在と、SAML2の期待するentityID・自分のACSを追加
//*                                （汎用認証サイトで IdFederation* の設定だった値。#634）
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

        #region AuthZ(N) Uir
        /// <summary>SpRp_AuthRequestUri</summary>
        public static string SpRp_AuthRequestUri
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_AuthRequestUri");
            }
        }

        /// <summary>SpRp_TokenRequestUri</summary>
        public static string SpRp_TokenRequestUri
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_TokenRequestUri");
            }
        }

        /// <summary>SpRp_UserInfoUri</summary>
        public static string SpRp_UserInfoUri
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_UserInfoUri");
            }
        }
        
        /// <summary>SpRp_RedirectUri</summary>
        public static string SpRp_RedirectUri
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_RedirectUri");
            }
        }
        #endregion

        #region 相手（OP / IdP）のメタデータ
        /// <summary>SpRp_DiscoveryUri（相手の OP の Discovery の URL）</summary>
        /// <remarks>
        /// RemoteJwkSetStore.GetJwkByDiscoveryAsync(kid) が読む。期待する Issuer は SpRp_Isser。
        /// Issuer にパスを含む OP（https://host/tenant など）もあるので、口の URL から組み立てず、URL を設定で与える。
        /// 空なら、設定を読む版の口は使えない（null を返す）。
        /// </remarks>
        public static string SpRp_DiscoveryUri
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_DiscoveryUri");
            }
        }

        /// <summary>SpRp_Saml2MetadataUri（相手の IdP の SAML2 メタデータの URL）</summary>
        /// <remarks>
        /// SAML2Metadata.GetAsync() が読む。SAML2 にはメタデータの既知のパスが無いので、URL を設定で与える。
        /// 空なら、設定を読む版の口は使えない（null を返す）。
        /// </remarks>
        public static string SpRp_Saml2MetadataUri
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_Saml2MetadataUri");
            }
        }

        /// <summary>SpRp_Saml2EntityId（相手の IdP の entityID。期待する Issuer）</summary>
        /// <remarks>
        /// SAML2Metadata.GetAsync() と、設定を読む版の SAML2Client.VerifyResponseWithReason が読む。
        /// OIDC の SpRp_Isser に当たるものだが、OP の Issuer と IdP の entityID は違うことがあるので分けている。
        /// </remarks>
        public static string SpRp_Saml2EntityId
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_Saml2EntityId");
            }
        }

        /// <summary>SpRp_Saml2AcsUri（自分の ACS の URL）</summary>
        /// <remarks>
        /// 設定を読む版の SAML2Client.VerifyResponseWithReason が、Audience と Recipient に照合する。
        /// 空なら照合しない。
        /// </remarks>
        public static string SpRp_Saml2AcsUri
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_Saml2AcsUri");
            }
        }
        #endregion

        #region 鍵関連

        #region Token検証
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
        #endregion

        #region クライアント認証
        // OAuth 2.0 JWT Bearer Token Flow
        // JWT Secured Authorization Request (JAR)

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

        /// <summary>EcdsaPfxPassword</summary>
        public static string EcdsaPfxPassword
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_EcdsaPfxPassword");
            }
        }

        /// <summary>EcdsaPfxFilePath</summary>
        public static string EcdsaPfxFilePath
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_EcdsaPfxFilePath");
            }
        }

        #endregion

        #region クライアント証明書
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

        #endregion
    }
}

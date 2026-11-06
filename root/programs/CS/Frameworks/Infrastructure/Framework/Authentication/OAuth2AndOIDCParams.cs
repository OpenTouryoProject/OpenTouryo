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
//*  2020/06/19  西野 大介         GetConfigSectionメソッドを廃止に伴う変更
//*                                GetConfigSection → GetAnyConfigSection
//*  2026/09/25  玄人 幸道         AuthRequestPushUri（PARエンドポイント）を追加
//*  2026/10/12  玄人 幸道         ClientIDsは、未設定なら空のリストを返すようにした（#603）
//*  2026/11/06  玄人 幸道         JwkSetUri・RequestObjectRegUri・AuthRequestPushUri の設定キーを、
//*                                SpRp_JwkSetUri・SpRp_RequestObjectRegUri・SpRp_AuthRequestPushUri に改名した（相手の口の URL。
//*                                改名前の名前は、汎用認証サイトが認可サーバ自身の口のパスとして使うので読まない。#666）
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

        /// <summary>Audience</summary>
        public static string Audience
        {
            get
            {
                return GetConfigParameter.GetConfigValue("OAuth2AndOidcAudience");
            }
        }

        /// <summary>ClientIDs</summary>
        /// <remarks>未設定なら空のリストを返す（#603。null だと aud の確認で例外になっていた）</remarks>
        public static List<string> ClientIDs
        {
            get
            {
                List<string> clientIDs = null;
#if NETSTD
                clientIDs = GetConfigParameter.GetAnyConfigSection(
                    "appSettings:OAuth2AndOidcClientIDs").Get<List<string>>();
#else
                string value = GetConfigParameter.GetConfigValue("OAuth2AndOidcClientIDs");
                if (!string.IsNullOrEmpty(value))
                {
                    clientIDs = JsonConvert.DeserializeObject<List<string>>(value);
                }
#endif
                return clientIDs ?? new List<string>();
            }
        }

        #endregion

        #region Jwk関連

        /// <summary>JwkSetFilePath</summary>
        public static string JwkSetFilePath
        {
            get
            {
                return GetConfigParameter.GetConfigValue("JwkSetFilePath");
            }
        }

        /// <summary>JwkSetUri（相手の OP の JWK Set の URL）</summary>
        /// <remarks>
        /// **設定のキーは SpRp_JwkSetUri**（#666 で改名した。相手の値なので、ほかの SpRp_* に揃えた）。プロパティ名は変えていない。
        /// **改名前の JwkSetUri は読まない**（汎用認証サイトが認可サーバ自身の口のパスとして使い、
        /// 自己テストのクライアントと 1 つの環境に共存しうるため。CmnClientParams.RenamedKeys の remarks）。
        /// JwkSetStore と、RemoteJwkSetStore.GetJwkAsync(kid) が読む。空なら JwkSetStore は空の JWK Set を持つ（検証は鍵が無く失敗する）。
        /// </remarks>
        public static string JwkSetUri
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_JwkSetUri");
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

        #region FAPI2関連
        /// <summary>
        /// FAPI2のRequestObject登録エンドポイント 
        /// </summary>
        /// <remarks>
        /// **相手（OP）の口の URL**（クライアント役の値）。フレームワークの中では読まない。
        /// **設定のキーは SpRp_RequestObjectRegUri**（#666 で改名した。相手の値なので、ほかの SpRp_* に揃えた）。
        /// **改名前の RequestObjectRegUri は読まない**（JwkSetUri と同じ理由）。
        /// 認可サーバ自身の口のパスは、この値に置かない（汎用認証サイトは自分の設定で持つ）。
        /// </remarks>
        public static string RequestObjectRegUri
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_RequestObjectRegUri");
            }
        }

        /// <summary>
        /// PAR（RFC 9126）のエンドポイント
        /// （Discoveryのpushed_authorization_request_endpoint）
        /// </summary>
        /// <remarks>
        /// **相手（OP）の口の URL**（クライアント役の値）。フレームワークの中では読まない。
        /// **設定のキーは SpRp_AuthRequestPushUri**（#666 で改名した。相手の値なので、ほかの SpRp_* に揃えた）。
        /// **改名前の AuthRequestPushUri は読まない**（JwkSetUri と同じ理由）。
        /// 認可サーバ自身の口のパスは、この値に置かない（汎用認証サイトは自分の設定で持つ）。
        /// </remarks>
        public static string AuthRequestPushUri
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_AuthRequestPushUri");
            }
        }
        #endregion
    }
}

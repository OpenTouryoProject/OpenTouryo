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
//*  2026/11/06  玄人 幸道         設定キー SpRp_Isser を SpRp_Issuer に改名した。改名前の名前も読み、
//*                                改名前の名前が残っている設定を返す OldKeysStillUsed・RenamedKeysInConflict を追加（#666）
//*  2026/11/14  玄人 幸道         自分（SP）の entityID（SpRp_Saml2SpEntityId）を追加（SAML2 の Audience の照合。#693）
//**********************************************************************************

using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        /// <summary>Isser（相手の OP の Issuer。期待する iss）</summary>
        /// <remarks>
        /// **設定のキーは SpRp_Issuer**（#666 で改名した。綴りの誤りだった）。
        /// SpRp_Issuer が空なら、改名前の SpRp_Isser を読む（RenamedKeys）。プロパティ名は変えていない。
        /// </remarks>
        public static string Isser
        {
            get
            {
                return CmnClientParams.GetRenamedConfigValue("SpRp_Issuer");
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
        /// RemoteJwkSetStore.GetJwkByDiscoveryAsync(kid) が読む。期待する Issuer は SpRp_Issuer（改名前は SpRp_Isser）。
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
        /// OIDC の SpRp_Issuer に当たるものだが、OP の Issuer と IdP の entityID は違うことがあるので分けている。
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
        /// 設定を読む版の SAML2Client.VerifyResponseWithReason が、Recipient に照合する。空なら照合しない。
        /// **Audience にも照合するのは、SpRp_Saml2SpEntityId が空のときだけ**（従来の照合。#693）。
        /// </remarks>
        public static string SpRp_Saml2AcsUri
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_Saml2AcsUri");
            }
        }

        /// <summary>SpRp_Saml2SpEntityId（自分（SP）の entityID）</summary>
        /// <remarks>
        /// 設定を読む版の SAML2Client.VerifyResponseWithReason が、**Audience に照合する**（SAML Profiles 4.1.4.2。#693）。
        /// **空なら、従来どおり Audience を SpRp_Saml2AcsUri と照合する**（仕様と違うが、ACS の URL を Audience に入れる IdP と通るため。
        /// 汎用認証サイトがそうである）。
        /// 値は、要求（AuthnRequest）の Issuer に入れる値と同じにする。
        /// </remarks>
        public static string SpRp_Saml2SpEntityId
        {
            get
            {
                return GetConfigParameter.GetConfigValue("SpRp_Saml2SpEntityId");
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

        #region 改名した設定キー（#666）

        /// <summary>改名し、改名前の名前も読み続ける設定キー（新しい名前 → 改名前の名前。#666）</summary>
        /// <remarks>
        /// **改名しても、改名前の名前を読み続ける**（期限は決めていない）。
        /// 改名だけで既存の設定が黙って効かなくなる（値が空になる）と、検証の相手が変わるなど危ないため。
        ///
        /// | 新しい名前 | 改名前の名前 | 理由 |
        /// |---|---|---|
        /// | SpRp_Issuer | SpRp_Isser | 綴りの誤り（2017 年の OAuth2AndOidcIsser からの名前） |
        ///
        /// - **新しい名前を優先する。** 新しい名前の値が空（null・空文字）のときだけ、改名前の名前を読む
        /// - コンテナ・モード（FxContainerization）の環境変数も、名前ごとに読む。
        ///   順は、新しい名前（環境変数 → 設定ファイル）→ 改名前の名前（環境変数 → 設定ファイル）
        /// - **警告は、フレームワークでは出さない。** アプリが起動時に OldKeysStillUsed・RenamedKeysInConflict を呼び、ログに出す
        ///   （汎用認証サイトの Config.RenamedKeys・OldKeysStillUsed と同じ分担）
        ///
        /// **改名したが、改名前の名前は読まないキーもある**（この表には載せない）。
        ///
        /// | 新しい名前 | 改名前の名前（読まない） | 読むプロパティ |
        /// |---|---|---|
        /// | SpRp_JwkSetUri | JwkSetUri | OAuth2AndOIDCParams.JwkSetUri |
        /// | SpRp_RequestObjectRegUri | RequestObjectRegUri | OAuth2AndOIDCParams.RequestObjectRegUri |
        /// | SpRp_AuthRequestPushUri | AuthRequestPushUri | OAuth2AndOIDCParams.AuthRequestPushUri |
        ///
        /// 3 つとも相手（OP）の口の URL（クライアント役の値）だが、**改名前の名前は、汎用認証サイトが認可サーバ自身の口のパス**として使っている。
        /// 汎用認証サイトには自分に送る自己テストのクライアントがあり、**1 つの環境にサーバとクライアントが共存しうる**。
        /// 読み替えると、クライアント役の値にサーバのパスが黙って入り、警告の関数も誤って検出するので、読み替えない。
        /// </remarks>
        public static readonly IReadOnlyDictionary<string, string> RenamedKeys =
            new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()
            {
                // 綴りの誤り（Isser → Issuer）
                { "SpRp_Issuer", "SpRp_Isser" },
            });

        /// <summary>改名した設定キーの値を読む（新しい名前を優先し、空なら改名前の名前）</summary>
        /// <param name="key">新しい名前（RenamedKeys に在るもの）</param>
        /// <returns>値（改名前の名前も空なら、新しい名前の値をそのまま返す）</returns>
        internal static string GetRenamedConfigValue(string key)
        {
            string value = GetConfigParameter.GetConfigValue(key);

            if (string.IsNullOrEmpty(value))
            {
                string oldValue = GetConfigParameter.GetConfigValue(CmnClientParams.RenamedKeys[key]);

                if (!string.IsNullOrEmpty(oldValue))
                {
                    value = oldValue;
                }
            }

            return value;
        }

        /// <summary>改名前の名前だけが設定されているものを返す（起動時の警告に使う。#666）</summary>
        /// <returns>「改名前の名前 → 新しい名前」の一覧（無ければ空）</returns>
        /// <remarks>
        /// 値は改名前の名前から読まれている。動作はするが、新しい名前に書き換えるよう促す。
        /// 値は返さない（ログに秘密を出さないため）。
        /// </remarks>
        public static List<KeyValuePair<string, string>> OldKeysStillUsed()
        {
            List<KeyValuePair<string, string>> used = new List<KeyValuePair<string, string>>();

            foreach (KeyValuePair<string, string> renamed in CmnClientParams.RenamedKeys)
            {
                if (string.IsNullOrEmpty(GetConfigParameter.GetConfigValue(renamed.Key))
                    && !string.IsNullOrEmpty(GetConfigParameter.GetConfigValue(renamed.Value)))
                {
                    used.Add(new KeyValuePair<string, string>(renamed.Value, renamed.Key));
                }
            }

            return used;
        }

        /// <summary>新しい名前と改名前の名前の両方に、違う値が設定されているものを返す（起動時の警告に使う。#666）</summary>
        /// <returns>「改名前の名前 → 新しい名前」の一覧（無ければ空）</returns>
        /// <remarks>
        /// **使われるのは新しい名前の値**で、改名前の名前の値は読まれない。書き間違いか、移行の途中の設定である。
        /// 例えば、改名前の名前を環境変数で上書きしていた配備で、設定ファイルだけを新しい名前にすると、ここに出る。
        /// 値は大文字・小文字を区別して比べる。同じ値なら返さない。値は返さない（ログに秘密を出さないため）。
        /// </remarks>
        public static List<KeyValuePair<string, string>> RenamedKeysInConflict()
        {
            List<KeyValuePair<string, string>> conflicts = new List<KeyValuePair<string, string>>();

            foreach (KeyValuePair<string, string> renamed in CmnClientParams.RenamedKeys)
            {
                string newValue = GetConfigParameter.GetConfigValue(renamed.Key);
                string oldValue = GetConfigParameter.GetConfigValue(renamed.Value);

                if (!string.IsNullOrEmpty(newValue) && !string.IsNullOrEmpty(oldValue)
                    && !string.Equals(newValue, oldValue, System.StringComparison.Ordinal))
                {
                    conflicts.Add(new KeyValuePair<string, string>(renamed.Value, renamed.Key));
                }
            }

            return conflicts;
        }

        #endregion
    }
}

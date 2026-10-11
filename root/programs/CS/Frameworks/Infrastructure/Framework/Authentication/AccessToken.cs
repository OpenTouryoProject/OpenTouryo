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
//* クラス名        ：AccessToken
//* クラス日本語名  ：AccessToken
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2017/09/07  西野 大介         新規作成
//*  2017/11/29  西野 大介         DateTimeOffset.ToUnixTimeSecondsの前方互換メソッドの使用
//*  2018/03/28  西野 大介         .NET Standard対応で、幾らか、I/F変更あり。
//*  2018/11/28  西野 大介         証明書 & Jwk対応 + jkuチェック対応の追加
//*  2018/11/28  西野 大介         リネーム（JwtToken -> AccessToken）
//*  2020/06/23  西野 大介         audの検証を見直した（Keycloakコンパチ）
//*  2026/10/12  玄人 幸道         Verifyに、検証鍵（JWS）と期待するiss / audの集合を受けるオーバーロードを追加（#601）。
//*                                既存の版は鍵もissも自分の設定で判定し、audは確認しない。
//**********************************************************************************

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>
    /// OAuth2やOIDC関連のAccessToken処理
    /// </summary>
    public class AccessToken
    {
        #region Create
        // AuthZに実装（パラメタ体系が違うため）
        #endregion

        #region Verify
        /// <summary>汎用認証サイトの発行したAccessTokenを検証する。</summary>
        /// <param name="access_token">
        /// AccessTokenで以下の項目が必要
        ///  - iss
        ///  - aud
        ///  - iat
        ///  - exp
        ///  - sub
        ///  - roles  (option)
        ///  - scopes (option)
        ///  - その他 (option)
        /// </param>
        /// <param name="sub">out string</param>
        /// <param name="roles">out List(string)</param>
        /// <param name="scopes">out List(string)</param>
        /// <param name="jobj">out JObject</param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// 検証鍵・iss は、設定（CmnClientParams / OAuth2AndOIDCParams）から判定する。
        /// aud は確認しない（Keycloakコンパチ）。
        /// 鍵・iss を指定する場合や、aud を確認する場合は、jws / iss / auds を受けるオーバーロードを使う（#601）。
        /// </remarks>
        public static bool Verify(string access_token,
            out string sub, out List<string> roles, out List<string> scopes, out JObject jobj)
        {
            return AccessToken.VerifyCore(access_token,
                null, CmnClientParams.Isser,
                audToken =>
                {
                    // access_token の中身は仕様の外
                    return true;
                },
                out sub, out roles, out scopes, out jobj);
        }

        /// <summary>AccessTokenを、指定した検証鍵・iss・audの集合で検証する。</summary>
        /// <param name="access_token">string</param>
        /// <param name="jws">
        /// 署名の検証に使う、発行者（認可サーバ）の鍵（JWS_RS256_Param, JWS_RS256_X509 など）。
        /// null なら、CmnJwtToken.Verify(jwtToken, out jwtPayload) と同じ方法で選ぶ。
        /// </param>
        /// <param name="iss">期待する iss（発行者の Issuer）</param>
        /// <param name="auds">
        /// 期待する aud の集合。access_token の aud が、このいずれかと一致すれば通す。
        /// aud に client_id が入る形式なら、受け付けるクライアント（SP・RP）の client_id の一覧。
        /// aud に RS の識別子が入る形式（RFC 9068）なら、RS 自身の識別子。
        /// </param>
        /// <param name="sub">out string</param>
        /// <param name="roles">out List(string)</param>
        /// <param name="scopes">out List(string)</param>
        /// <param name="jobj">out JObject</param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// 自由に利用できる。
        ///
        /// 設定（CmnClientParams.Isser / OAuth2AndOIDCParams.JwkSetFilePath）は見ない。
        /// aud は必ず確認する（RFC 9700 2.3 / RFC 9068 4）。
        /// aud が array の場合は、期待する aud のいずれかを含むことを確認する。
        /// typ（at+jwt）は確認しない（RFC 9068 の形式には準拠していないため）。
        /// Keycloakの access_token を検証する場合は、Keycloak 側で aud を付与する設定（Audience mapper）が要る。
        /// </remarks>
        public static bool Verify(string access_token,
            JWS jws, string iss, string[] auds,
            out string sub, out List<string> roles, out List<string> scopes, out JObject jobj)
        {
            if (string.IsNullOrEmpty(iss))
            {
                throw new ArgumentNullException(nameof(iss));
            }

            if (auds == null || auds.Length == 0)
            {
                throw new ArgumentNullException(nameof(auds));
            }

            return AccessToken.VerifyCore(access_token,
                jws, iss,
                audToken =>
                {
                    // aud が無い、空の場合は、一致させない。
                    if (audToken is JArray)
                    {
                        return ((JArray)audToken).Any(x =>
                            !string.IsNullOrEmpty((string)x) && auds.Contains((string)x));
                    }
                    else
                    {
                        string aud = (string)audToken;
                        return !string.IsNullOrEmpty(aud) && auds.Contains(aud);
                    }
                },
                out sub, out roles, out scopes, out jobj);
        }

        /// <summary>AccessTokenを検証する（共通部分）。</summary>
        /// <param name="access_token">string</param>
        /// <param name="jws">検証鍵（null なら設定から選ぶ）</param>
        /// <param name="expectedIss">期待する iss</param>
        /// <param name="verifyAud">aud の確認</param>
        /// <param name="sub">out string</param>
        /// <param name="roles">out List(string)</param>
        /// <param name="scopes">out List(string)</param>
        /// <param name="jobj">out JObject</param>
        /// <returns>検証結果</returns>
        private static bool VerifyCore(string access_token,
            JWS jws, string expectedIss, Func<JToken, bool> verifyAud,
            out string sub, out List<string> roles, out List<string> scopes, out JObject jobj)
        {
            sub = "";
            roles = new List<string>();
            scopes = new List<string>();
            jobj = null;

            // JWS検証
            string jwtPayload = "";
            if (CmnJwtToken.Verify(access_token, jws, out jwtPayload))
            {
                jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                #region クレーム検証

                string iss = (string)jobj[OAuth2AndOIDCConst.iss];
                JToken aud = jobj[OAuth2AndOIDCConst.aud];
                //string iat = (string)jobj[OAuth2AndOIDCConst.iat];
                string exp = (string)jobj[OAuth2AndOIDCConst.exp];

                sub = (string)jobj[OAuth2AndOIDCConst.sub];

                if (jobj[OAuth2AndOIDCConst.Scope_Roles] != null)
                {
                    roles = JsonConvert.DeserializeObject<List<string>>(jobj[OAuth2AndOIDCConst.Scope_Roles].ToString());
                }
                if (jobj[OAuth2AndOIDCConst.scopes] != null)
                {
                    scopes = JsonConvert.DeserializeObject<List<string>>(jobj[OAuth2AndOIDCConst.scopes].ToString());
                }

                long unixTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

                if (iss == expectedIss &&
                    long.Parse(exp) >= unixTimeSeconds)
                {
                    if (verifyAud(aud))
                    {
                        return true;
                    }
                    else
                    {
                        // JWTの内容検証に失敗
                    }
                }
                else
                {
                    // JWTの内容検証に失敗
                }

                #endregion
            }
            else
            {
                // JWTの署名検証に失敗
            }

            // 認証に失敗
            return false;
        }
        #endregion
    }
}
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
//*  2026/11/05  玄人 幸道         Verifyで、exp を小数の NumericDate でも読み、無い・読めない値を例外にせず false にした（#647）
//*  2026/11/05  玄人 幸道         設定を読む Verify で、設定の client_id（ClientID / ClientIDs）が在れば aud を確かめるようにした（#655）。
//*                                設定が無ければ従来どおり確かめない（4.0 で断る予定）
//*  2026/11/05  玄人 幸道         設定を 1 回だけ読み、空白だけの値は無いもの、壊れた JSON は断るとした（#655 のレビュー）
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
        ///
        /// **aud は、設定の client_id（OAuth2AndOIDCParams.ClientID / ClientIDs）が在れば、そのいずれかを含むことを確かめる**（#655）。
        /// IdToken の設定を読む版（#603）と同じ判定（CmnJwtToken.VerifyAudByClientIDs）で、aud に client_id が入る形式
        /// （汎用認証サイトなど）に合う。リソース サーバは、受け付けるクライアントの client_id を OAuth2AndOidcClientIDs に並べる。
        /// 2026/11/05 まで、aud は確かめていなかった（Keycloak コンパチ）。
        ///
        /// **ClientID も ClientIDs も設定されていなければ、従来どおり aud を確かめない**（壊さないため）。
        /// 4.0 で、設定が無いときも断る既定に変える予定である（#655）。
        /// Keycloak のトークンを受けるなら、Keycloak 側で aud を付ける設定（Audience mapper）が要る。
        ///
        /// 鍵・iss を指定する場合や、aud を別の値で確かめる場合は、jws / iss / auds を受けるオーバーロードを使う（#601）。
        /// </remarks>
        public static bool Verify(string access_token,
            out string sub, out List<string> roles, out List<string> scopes, out JObject jobj)
        {
            return AccessToken.VerifyCore(access_token,
                null, CmnClientParams.Isser,
                AccessToken.VerifyAudBySettings,
                out sub, out roles, out scopes, out jobj);
        }

        /// <summary>aud を、設定の client_id で確かめる（設定を読む版の aud の判定）</summary>
        /// <param name="aud">aud のクレーム</param>
        /// <returns>bool</returns>
        /// <remarks>
        /// 設定（OAuth2AndOIDCParams.ClientID / ClientIDs）が在れば、そのいずれかを含むことを確かめる（#655）。
        /// 判定は CmnJwtToken.VerifyAudByClientIDs と同じ（ContainsAud。文字列でない aud は false）。
        /// - どちらも無ければ（空白だけの値も無いものとする）、従来どおり確かめない（true）。4.0 で、無いときも断る既定に変える予定
        /// - **ClientIDs の JSON が壊れていれば、設定の誤りとして断る**（false。例外にしない）
        /// 設定は 1 回の検証で 1 回だけ読む（CmnJwtToken.ReadConfiguredClientIDs）。
        /// </remarks>
        private static bool VerifyAudBySettings(JToken aud)
        {
            List<string> clientIDs = CmnJwtToken.ReadConfiguredClientIDs();

            if (clientIDs == null)
            {
                // 設定の誤り（JSON が壊れている）：断る（#655 のレビュー）
                return false;
            }

            if (clientIDs.Count == 0)
            {
                // 設定が無い：従来どおり aud を確かめない（Keycloak コンパチ。4.0 で断る予定。#655）
                return true;
            }

            return CmnJwtToken.ContainsAud(aud, clientIDs);
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

                // exp は小数の NumericDate も読む。無い・読めない値は、例外にせず検証の失敗にする（RFC 7519 2。#647）
                if (iss == expectedIss &&
                    CmnJwtToken.VerifyExpClaim(jobj[OAuth2AndOIDCConst.exp], unixTimeSeconds))
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

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
//* クラス名        ：ResponseObject
//* クラス日本語名  ：ResponseObject
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/26  西野 大介         新規作成
//*  2026/10/12  玄人 幸道         Verifyに、検証鍵（JWS）と期待するiss / audを受けるオーバーロードを追加（#601）。
//*  2026/10/12  玄人 幸道         設定で判定するVerifyでも、認可サーバを兼ねる配備でaudを確認するようにした（#603）。
//*  2026/10/22  玄人 幸道         Verifyの対となるCreateを追加（汎用認証サイトから移した。#634）
//*  2026/10/22  玄人 幸道         Createに、iss と鍵を設定（CmnServerParams）から読む版を追加
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
    /// OAuth2やOIDC(FAPI2)関連のResponseObject処理
    /// 正確には、JWT Secured Authorization Response Mode for OAuth 2.0 (JARM)
    /// </summary>
    public class ResponseObject
    {
        #region Create

        /// <summary>ResponseObject（JARM の response パラメタの値）を作る（設定を読む版）。</summary>
        /// <param name="responseParams">Dictionary（認可応答のパラメタ。code / state、または error など）</param>
        /// <param name="aud">aud（応答を受け取るクライアントの client_id）</param>
        /// <param name="expires">exp（有効期限）</param>
        /// <returns>ResponseObject（JWS）</returns>
        /// <remarks>
        /// 自由に利用できる。iss は CmnServerParams.IssuerId、
        /// 鍵は CmnServerParams.RsaPfxFilePath / RsaPfxPassword（RS256。kid は、その鍵の JWK の kid）。
        /// IssuerId / RsaPfxFilePath が無ければ FrameworkException。
        /// 鍵を替えるなら、jws を受ける版を使う。
        /// </remarks>
        public static string Create(Dictionary<string, string> responseParams, string aud, DateTimeOffset expires)
        {
            string iss = CmnServerParams.Required("IssuerId", CmnServerParams.IssuerId);
            string pfxFilePath = CmnServerParams.Required("RsaPfxFilePath", CmnServerParams.RsaPfxFilePath);
            string pfxPassword = CmnServerParams.RsaPfxPassword;

            JWS_RS256_X509 jws = new JWS_RS256_X509(pfxFilePath, pfxPassword);

            // jku は入れない。鍵は kid で引かせる。
            Dictionary<string, string> jwk = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                new RsaPublicKeyConverter(JWS_RSA.RS._256).X509PfxToJwk(pfxFilePath, pfxPassword));
            jws.JWSHeader.kid = jwk[JwtConst.kid];

            return ResponseObject.Create(responseParams, jws, iss, aud, expires);
        }

        /// <summary>ResponseObject（JARM の response パラメタの値）を作る。</summary>
        /// <param name="responseParams">Dictionary（認可応答のパラメタ。code / state、または error など）</param>
        /// <param name="jws">署名に使う、自分（認可サーバ）の鍵。kid は呼び出し側で JWSHeader に入れておく</param>
        /// <param name="iss">iss（自分の Issuer）</param>
        /// <param name="aud">aud（応答を受け取るクライアントの client_id）</param>
        /// <param name="expires">exp（有効期限）</param>
        /// <returns>ResponseObject（JWS）</returns>
        /// <remarks>
        /// 自由に利用できる。Verify の対である（汎用認証サイトから移した。#634）。
        ///
        /// - 応答のパラメタは文字列のまま入れ、iss / aud / exp を足す（JARM 2.1）
        /// - exp は NumericDate（JSON の数値）で入れる（RFC 7519 2。文字列にしない）
        /// - jku はヘッダに入れない（鍵は kid で引かせる）
        /// - 引数の Dictionary は変えない。iss / aud / exp を持っていても、引数の値で上書きする
        /// 鍵の選び方（どの alg・どの証明書）は、配備の決めなので呼び出し側で持つ。
        /// </remarks>
        public static string Create(Dictionary<string, string> responseParams,
            JWS jws, string iss, string aud, DateTimeOffset expires)
        {
            if (responseParams == null)
            {
                throw new ArgumentNullException(nameof(responseParams));
            }

            if (jws == null)
            {
                throw new ArgumentNullException(nameof(jws));
            }

            if (string.IsNullOrEmpty(iss))
            {
                throw new ArgumentNullException(nameof(iss));
            }

            if (string.IsNullOrEmpty(aud))
            {
                throw new ArgumentNullException(nameof(aud));
            }

            Dictionary<string, object> payload = new Dictionary<string, object>();

            foreach (KeyValuePair<string, string> item in responseParams)
            {
                payload[item.Key] = item.Value;
            }

            payload[OAuth2AndOIDCConst.iss] = iss;
            payload[OAuth2AndOIDCConst.aud] = aud;
            payload[OAuth2AndOIDCConst.exp] = expires.ToUnixTimeSeconds();

            return jws.Create(JsonConvert.SerializeObject(payload));
        }

        #endregion

        #region Verify

        // SPRP → (1) → 下流IdP → (2) → 上流IdP
        // SPRP ← (4) ← 下流IdP ← (3) ← 上流IdP
        // (3)で後者のVerifyを使い、(4)で前者のVerifyを使う。
        // （JARM を要求した場合のみ。error の応答は iss / aud を確認せずに true を返す）

        /// <summary>汎用認証サイトの発行したResponseObjectを検証する。</summary>
        /// <param name="response">string</param>
        /// <param name="jobj">out JObject</param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// 検証鍵・iss・aud は、設定（CmnClientParams / OAuth2AndOIDCParams）から判定する。
        /// aud は、OAuth2AndOIDCParams.ClientID / ClientIDs のいずれかを含むことを確認する（JARM 4.4）。
        /// 2026/10/12 まで、OAuth2AndOIDCParams.JwkSetFilePath が在る配備（認可サーバ兼クライアント）では
        /// aud を確認していなかった（#603）。
        /// 相手（認可サーバ）の ResponseObject を検証する場合は、jws / iss / aud を受けるオーバーロードを使う（#601）。
        /// </remarks>
        public static bool Verify(string response, out JObject jobj)
        {
            return ResponseObject.VerifyCore(response,
                null, CmnClientParams.Isser,
                // 認可サーバを兼ねる配備（JwkSetFilePath が在る）でも aud を確認する（#603）
                CmnJwtToken.VerifyAudByClientIDs,
                out jobj);
        }

        /// <summary>相手（認可サーバ）の発行したResponseObjectを、指定した検証鍵・iss・audで検証する。</summary>
        /// <param name="response">string</param>
        /// <param name="jws">
        /// 署名の検証に使う、相手（認可サーバ）の鍵（JWS_RS256_Param, JWS_RS256_X509 など）。
        /// null なら、CmnJwtToken.Verify(jwtToken, out jwtPayload) と同じ方法で選ぶ。
        /// </param>
        /// <param name="iss">期待する iss（相手の Issuer）</param>
        /// <param name="aud">期待する aud（相手に登録した、自分の client_id）</param>
        /// <param name="jobj">out JObject</param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// 自由に利用できる。
        ///
        /// 設定（CmnClientParams.Isser / OAuth2AndOIDCParams.ClientID / JwkSetFilePath）は見ない。
        /// aud は、認可サーバを兼ねる配備でも必ず確認する（JARM 4.4）。
        /// aud が array の場合は、期待する aud を含むことを確認する。
        /// </remarks>
        public static bool Verify(string response,
            JWS jws, string iss, string aud, out JObject jobj)
        {
            if (string.IsNullOrEmpty(iss))
            {
                throw new ArgumentNullException(nameof(iss));
            }

            if (string.IsNullOrEmpty(aud))
            {
                throw new ArgumentNullException(nameof(aud));
            }

            return ResponseObject.VerifyCore(response,
                jws, iss,
                audToken =>
                {
                    if (audToken is JArray)
                    {
                        return ((JArray)audToken).Any(x => (string)x == aud);
                    }
                    else
                    {
                        return (string)audToken == aud;
                    }
                },
                out jobj);
        }

        /// <summary>ResponseObjectを検証する（共通部分）。</summary>
        /// <param name="response">string</param>
        /// <param name="jws">検証鍵（null なら設定から選ぶ）</param>
        /// <param name="expectedIss">期待する iss</param>
        /// <param name="verifyAud">aud の確認</param>
        /// <param name="jobj">out JObject</param>
        /// <returns>検証結果</returns>
        private static bool VerifyCore(string response,
            JWS jws, string expectedIss, Func<JToken, bool> verifyAud,
            out JObject jobj)
        {
            // JWS検証
            string jwtPayload = "";
            if (CmnJwtToken.Verify(response, jws, out jwtPayload))
            {
                jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                #region クレーム検証

                if (jobj.ContainsKey(OAuth2AndOIDCConst.error))
                {
                    // errorの場合
                    return true;
                }
                else
                {
                    // errorでない場合

                    string iss = (string)jobj[OAuth2AndOIDCConst.iss];
                    JToken aud = jobj[OAuth2AndOIDCConst.aud];
                    //string iat = (string)jobj[OAuth2AndOIDCConst.iat];
                    string exp = (string)jobj[OAuth2AndOIDCConst.exp];

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
                }

                #endregion
            }
            else
            {
                // JWTの署名検証に失敗
                jobj = null;
            }

            // 認証に失敗
            return false;
        }
        #endregion
    }
}

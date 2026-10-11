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
//* クラス名        ：IdToken
//* クラス日本語名  ：IdToken
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/08/22  西野 大介         新規作成
//*  2018/11/28  西野 大介         証明書 & Jwk対応 + jkuチェック対応の追加
//*  2026/09/10  玄人 幸道         CreateHashが左半分でなく左右のXORを返していたのを修正（#584）
//*  2026/10/11  玄人 幸道         Verifyに、検証鍵（JWS）と期待するiss / audを受けるオーバーロードを追加（#601）。
//*                                認可サーバ兼クライアントでは、鍵もissも自分の設定で判定され、相手のid_tokenを検証できなかった。
//**********************************************************************************

using System;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>
    /// HashClaimのタイプ
    /// ・None
    /// ・at_hash
    /// ・c_hash
    /// ・Both
    /// </summary>
    public enum HashClaimType
    {
        /// <summary>HashClaim無し</summary>
        None = 0x000,

        /// <summary>
        /// at_hashを付与(OIDC Hybrid)
        /// id_token token
        /// </summary>
        AtHash = 0x001,

        /// <summary>
        /// c_hashを付与(OIDC Hybrid)
        /// code id_token
        /// </summary>
        CHash = 0x002,

        /// <summary>
        /// s_hashを付与(FAPI2)
        /// </summary>
        SHash = 0x004
    }

    /// <summary>
    /// OIDCのIdToken処理
    /// </summary>
    public class IdToken
    {
        #region Create
        // AuthZに実装（パラメタ体系が違うため）
        #endregion

        #region Verify

        // SPRP → (1) → 下流IdP → (2) → 上流IdP
        // SPRP ← (4) ← 下流IdP ← (3) ← 上流IdP
        // (3)で後者のVerifyを使い、(4)で前者のVerifyを使う。

        /// <summary>汎用認証サイトの発行したIdTokenを検証する。</summary>
        /// <param name="id_token">string</param>
        /// <param name="access_token">string</param>
        /// <param name="code">string</param>
        /// <param name="state">string</param>
        /// <param name="sub">out string</param>
        /// <param name="nonce">out string</param>
        /// <param name="jobj">out JObject</param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// 検証鍵・iss・aud は、設定（CmnClientParams / OAuth2AndOIDCParams）から判定する。
        /// OAuth2AndOIDCParams.JwkSetFilePath が在るのは自己テストの経路でaud は確認しない。
        /// 相手（OP）の id_token を検証する場合は、jws / iss / aud を受けるオーバーロードを使う（#601）。
        /// </remarks>
        public static bool Verify(string id_token,
            string access_token, string code, string state,
            out string sub, out string nonce, out JObject jobj)
        {
            return IdToken.VerifyCore(id_token, access_token, code, state,
                null, CmnClientParams.Isser,
                audToken =>
                {
                    string aud = (string)audToken;

                    if (string.IsNullOrEmpty(OAuth2AndOIDCParams.JwkSetFilePath))
                    {
                        // Client側
                        if (aud == OAuth2AndOIDCParams.ClientID)
                        {
                            // OAuth2 Clientバージョンの実装で成功
                            return true;
                        }
                        else if (OAuth2AndOIDCParams.ClientIDs.Any(x => x == aud))
                        {
                            // OAuth2 ResourcesServerバージョンの実装で成功
                            return true;
                        }
                        else
                        {
                            // JWTの内容検証に失敗
                            return false;
                        }
                    }
                    else
                    {
                        // AuthZ側（検証用カバレッジ＝自己テストの経路
                        // OAuth2 AuthZバージョンの実装で成功
                        return true;
                    }
                },
                out sub, out nonce, out jobj);
        }

        /// <summary>相手（OP）の発行したIdTokenを、指定した検証鍵・iss・audで検証する。</summary>
        /// <param name="id_token">string</param>
        /// <param name="access_token">string</param>
        /// <param name="code">string</param>
        /// <param name="state">string</param>
        /// <param name="jws">
        /// 署名の検証に使う、相手（OP）の鍵（JWS_RS256_Param, JWS_RS256_X509 など）。
        /// null なら、CmnJwtToken.Verify(jwtToken, out jwtPayload) と同じ方法で選ぶ。
        /// </param>
        /// <param name="iss">期待する iss（相手の Issuer）</param>
        /// <param name="aud">期待する aud（相手に登録した、自分の client_id）</param>
        /// <param name="sub">out string</param>
        /// <param name="nonce">out string</param>
        /// <param name="jobj">out JObject</param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// 自由に利用できる。
        ///
        /// 設定（CmnClientParams.Isser / OAuth2AndOIDCParams.ClientID / JwkSetFilePath）は見ない。
        /// aud は、認可サーバを兼ねる配備でも必ず確認する（#601）。
        /// aud が array の場合は、期待する aud を含むことを確認する（OpenID Connect Core 1.0 3.1.3.7）。
        /// </remarks>
        public static bool Verify(string id_token,
            string access_token, string code, string state,
            JWS jws, string iss, string aud,
            out string sub, out string nonce, out JObject jobj)
        {
            if (string.IsNullOrEmpty(iss))
            {
                throw new ArgumentNullException(nameof(iss));
            }

            if (string.IsNullOrEmpty(aud))
            {
                throw new ArgumentNullException(nameof(aud));
            }

            return IdToken.VerifyCore(id_token, access_token, code, state,
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
                out sub, out nonce, out jobj);
        }

        /// <summary>IdTokenを検証する（共通部分）。</summary>
        /// <param name="id_token">string</param>
        /// <param name="access_token">string</param>
        /// <param name="code">string</param>
        /// <param name="state">string</param>
        /// <param name="jws">検証鍵（null なら設定から選ぶ）</param>
        /// <param name="expectedIss">期待する iss</param>
        /// <param name="verifyAud">aud の確認</param>
        /// <param name="sub">out string</param>
        /// <param name="nonce">out string</param>
        /// <param name="jobj">out JObject</param>
        /// <returns>検証結果</returns>
        private static bool VerifyCore(string id_token,
            string access_token, string code, string state,
            JWS jws, string expectedIss, Func<JToken, bool> verifyAud,
            out string sub, out string nonce, out JObject jobj)
        {
            sub = "";
            nonce = "";
            jobj = null;

            // JWS検証
            string jwtPayload = "";
            if (CmnJwtToken.Verify(id_token, jws, out jwtPayload))
            {
                jobj = ((JObject)JsonConvert.DeserializeObject(jwtPayload));

                #region クレーム検証

                // out
                sub = (string)jobj[OAuth2AndOIDCConst.sub];
                nonce = (string)jobj[OAuth2AndOIDCConst.nonce];

                string iss = (string)jobj[OAuth2AndOIDCConst.iss];
                JToken aud = jobj[OAuth2AndOIDCConst.aud];
                //string iat = (string)jobj[OAuth2AndOIDCConst.iat];
                string exp = (string)jobj[OAuth2AndOIDCConst.exp];

                #region ハッシュ・クレーム検証

                // at_hash
                string at_hash = (string)jobj[OAuth2AndOIDCConst.at_hash];
                if (!string.IsNullOrEmpty(access_token) && !string.IsNullOrEmpty(at_hash))
                {
                    if (!IdToken.VerifyHash(access_token, at_hash))
                    {
                        return false;
                    }
                }

                // c_hash
                string c_hash = (string)jobj[OAuth2AndOIDCConst.c_hash];
                if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(c_hash))
                {
                    if (!IdToken.VerifyHash(code, c_hash))
                    {
                        return false;
                    }
                }

                // s_hash
                string s_hash = (string)jobj[OAuth2AndOIDCConst.s_hash];
                if (!string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(s_hash))
                {
                    if (!IdToken.VerifyHash(state, s_hash))
                    {
                        return false;
                    }
                }

                #endregion

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

        #region at_hash, c_hash, s_hash

        /// <summary>
        /// at_hash, c_hash, s_hashを作成
        /// （SHA256→HS256，RS256，ES256対応可能）
        /// </summary>
        /// <param name="input">string</param>
        /// <returns>hash</returns>
        /// <remarks>
        /// OIDC の定義（OpenID Connect Core 1.0 3.1.3.6 / 3.3.2.11）どおり、
        /// ハッシュの**左半分**（先頭 bytes.Length / 2 バイト）を base64url にする。
        ///
        /// 2026/09/10 まで ArrayOperator.ShortenByteArray で半分にしていたため、
        /// 左半分ではなく**左半分と右半分の XOR** になっていた（#584）。
        /// ShortenByteArray は切り詰めではなく XOR による畳み込みで、
        /// 鍵長の調整（GetKeyedHash）がその挙動に依存するため、あちらは直さない。
        ///
        /// VerifyHash も同じ CreateHash で作り直して比べるため、
        /// 本フレームワーク同士では誤っていても一致し、気付けなかった。
        /// **修正前の版で発行された値は、修正後の VerifyHash では検証に失敗する。**
        /// </remarks>
        public static string CreateHash(string input)
        {
            // ID Token の JOSE Header にある
            // alg Header Parameterのアルゴリズムで使用されるハッシュアルゴリズムを用い、
            // input(access_token や code) のASCII オクテット列からハッシュ値を求め、
            byte[] bytes = GetHash.GetHashBytes(
                CustomEncode.StringToByte(input, CustomEncode.us_ascii),
                EnumHashAlgorithm.SHA256);

            // 左半分を base64url エンコードした値。
            // （ShortenByteArray は XOR で畳み込むため使わない。#584）
            return CustomEncode.ToBase64UrlString(
                ArrayOperator.CopyArray<byte>(bytes, (bytes.Length / 2)));
        }

        /// <summary>
        /// at_hash, c_hash, s_hashの検証
        /// （SHA256→HS256，RS256，ES256対応可能）
        /// </summary>
        /// <param name="input">string</param>
        /// <param name="hash">string</param>
        /// <returns>検証結果</returns>
        public static bool VerifyHash(string input, string hash)
        {
            return (hash == IdToken.CreateHash(input));
        }

        #endregion
    }
}

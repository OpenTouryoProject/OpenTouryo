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
//* クラス名        ：CmnJwtToken
//* クラス日本語名  ：CmnJwtTokenクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/28  西野 大介         新規作成（分割）
//*  2020/03/02  西野 大介         検証メソッドの追加
//*  2020/03/04  西野 大介         Claim生成メソッドの追加
//*  2020/06/20  西野 大介         jku無しでもjwks_uriを使うように変更
//*  2026/10/11  玄人 幸道         Verifyに、検証鍵（JWS）を受けるオーバーロードを追加（#601）。
//*                                JwkSetFilePathの有無で鍵を選ぶため、認可サーバ兼クライアントでは相手の鍵を引けなかった。
//*  2026/10/12  玄人 幸道         設定のclient_idでaudを確認するVerifyAudByClientIDsを追加（#603）
//*  2026/11/05  玄人 幸道         exp・nbf・iat（NumericDate）を、小数・指数表記でも読めるようにした（RFC 7519 2。#647）。
//*                                読み方を TryReadNumericDate にまとめ、無い・読めない値は例外にせず検証の失敗にした
//*  2026/11/05  玄人 幸道         TryReadNumericDate：小数の double を 17 桁（G17）で読み（net48 の 15 桁の丸めを避ける）、
//*                                long の範囲を超える値を読めないとした（#647 のレビューの指摘 1・2）
//*  2026/11/05  玄人 幸道         nbf のクレームを確かめる VerifyNbfClaim を追加（RequestObject が使う。#653）
//*  2026/11/05  玄人 幸道         クレームの読み取り（ReadPayloadObject・ReadStringClaim・ContainsAud）を、
//*                                RequestObject から移した（JwtAssertion と共有する。#654）
//*  2026/11/05  玄人 幸道         形の壊れた JWS でも例外にしない TryVerifySignature を追加（#654 のレビューの指摘 5）
//*  2026/11/05  玄人 幸道         VerifyAudByClientIDs：aud を ContainsAud で照合し（文字列でないものは false。例外にしない）、
//*                                設定は ReadConfiguredClientIDs で 1 回だけ読む（空白だけの値は無いもの、壊れた JSON は false）（#655 のレビュー）
//*  2026/11/05  玄人 幸道         ReadConfiguredClientIDs：net10.0 で ClientIDs が文字列の配列の形でないとき（要素がオブジェクト、
//*                                配列でない 1 つの値）も、設定の誤りとした。バインドは形が違っても例外にしないため（#655 の 2 回目のレビュー）
//*  2026/11/06  玄人 幸道         TryReadNumericDate・VerifyExpClaim・VerifyNbfClaim を公開した（JToken を持つ呼び出し側が、
//*                                同じ読み方で exp・nbf を確かめられるようにする。#661。引数と振る舞いは変えていない）
//*  2026/11/06  玄人 幸道         CheckClaims：値がオブジェクト・配列でも例外にせず false にし、Obsolete（警告）にした。
//*                                無い・読めないクレームを invalid_request で返す TryGetStringClaim を追加（#662）
//*  2026/11/06  玄人 幸道         TryVerifySignature：ヘッダの alg が、検証鍵の alg と違えば false にする（RFC 8725 3.1。#664）
//**********************************************************************************

using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#if NETSTD
using Microsoft.Extensions.Configuration;
#endif

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>
    /// OAuth2やOIDC関連のJwtToken処理
    /// </summary>
    public class CmnJwtToken
    {
        /// <summary>汎用認証サイトの発行したJWT形式のTokenを検証する。</summary>
        /// <param name="jwtToken">JWT形式のToken</param>
        /// <param name="jwtPayload">
        /// JWS, JWS + JEWの場合があるのでペイロードを返す。
        /// </param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// 検証鍵は、kid の有無と OAuth2AndOIDCParams.JwkSetFilePath の有無で選ぶ。
        /// JwkSetFilePath が在る（＝認可サーバを兼ねる）と、自分の JWK Set で引くため、
        /// 相手（OP）の鍵で検証する場合は、jws を受けるオーバーロードを使う（#601）。
        /// </remarks>
        public static bool Verify(string jwtToken, out string jwtPayload)
        {
            return CmnJwtToken.Verify(jwtToken, null, out jwtPayload);
        }

        /// <summary>JWT形式のTokenを、指定した検証鍵で検証する。</summary>
        /// <param name="jwtToken">JWT形式のToken</param>
        /// <param name="jws">
        /// 署名の検証に使う、相手（署名者）の鍵（JWS_RS256_Param, JWS_RS256_X509 など）。
        /// null なら、Verify(jwtToken, out jwtPayload) と同じ方法で選ぶ。
        /// </param>
        /// <param name="jwtPayload">
        /// JWS, JWS + JEWの場合があるのでペイロードを返す。
        /// </param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// 自由に利用できる。
        ///
        /// JWE の復号には、従来どおり CmnClientParams.RsaPfxFilePath（自分の秘密鍵）を使う。
        /// jws で替わるのは、署名の検証鍵だけである（#601）。
        /// </remarks>
        public static bool Verify(string jwtToken, JWS jws, out string jwtPayload)
        {
            jwtPayload = "";

            JWE jwe = null;

            // 復号化（JWEの場合）
            bool isJWE_FAPI2 = false;
            if (3 < jwtToken.Split('.').Length)
            {
                isJWE_FAPI2 = true;

                // ヘッダ
                JWE_Header jweHeader = JsonConvert.DeserializeObject<JWE_Header>(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(jwtToken.Split('.')[0]), CustomEncode.UTF_8));

                if (jweHeader.alg == JwtConst.RSA_OAEP)
                {
                    jwe = new JWE_RsaOaepAesGcm_X509(
                        CmnClientParams.RsaPfxFilePath,
                        CmnClientParams.RsaPfxPassword);
                }
                else if (jweHeader.alg == JwtConst.RSA1_5)
                {
                    jwe = new JWE_Rsa15A128CbcHS256_X509(
                        CmnClientParams.RsaPfxFilePath,
                        CmnClientParams.RsaPfxPassword);
                }
                else
                {
                    throw new NotSupportedException(string.Format(
                        "This jwe alg of {0} is not supported.", jweHeader.alg));
                }

                jwe.Decrypt(jwtToken, out jwtToken);
            }
            else
            {
                isJWE_FAPI2 = false;
            }

            // 検証
            // ヘッダ
            JWS_Header jwsHeader = JsonConvert.DeserializeObject<JWS_Header>(
                CustomEncode.ByteToString(CustomEncode.FromBase64UrlString(jwtToken.Split('.')[0]), CustomEncode.UTF_8));

            if (jwsHeader.alg == JwtConst.ES256 && isJWE_FAPI2) { } // 正常
            else if (jwsHeader.alg == JwtConst.RS256 && !isJWE_FAPI2) { } // 正常
            else { throw new NotSupportedException("Unexpected combination of JWS and JWE."); }

            // 証明書を使用するか、Jwkを使用するか判定
            if (jws != null)
            {
                // 呼び出し側が指定した鍵を使用（#601）
            }
            else if ( //string.IsNullOrEmpty(jwsHeader.jku) || // jkuは非推奨
                string.IsNullOrEmpty(jwsHeader.kid))
            {
                // 旧バージョン（証明書を使用
                if (isJWE_FAPI2)
                {
                    jws = new JWS_ES256_X509(CmnClientParams.EcdsaCerFilePath, "");
                }
                else
                {
                    jws = new JWS_RS256_X509(CmnClientParams.RsaCerFilePath, "");
                }
            }
            else
            {
                // 新バージョン（Jwkを使用
                if (string.IsNullOrEmpty(OAuth2AndOIDCParams.JwkSetFilePath))
                {
                    // jku(jwks_uri)使用のカバレッジ

                    // Client側
                    JObject jwkObject = JwkSetStore.GetInstance().GetJwkObject(jwsHeader.kid);

                    // チェック
                    if (jwkObject == null)
                    {
                        // 書込
                        jwkObject = JwkSetStore.GetInstance().SetJwkSetObject(
                            // jwsHeader.jku, // jku廃止
                            jwsHeader.kid);
                    }

                    // チェック
                    if (jwkObject == null)
                    {
                        // 証明書を使用
                        if (isJWE_FAPI2)
                        {
                            jws = new JWS_ES256_X509(CmnClientParams.EcdsaCerFilePath, "");
                        }
                        else
                        {
                            jws = new JWS_RS256_X509(CmnClientParams.RsaCerFilePath, "");
                        }
                    }
                    else
                    {
                        // Jwkを使用
                        if (isJWE_FAPI2)
                        {
                            EccPublicKeyConverter epkc = new EccPublicKeyConverter();
                            jws = new JWS_ES256_Param(epkc.JwkToParam(jwkObject), false);
                        }
                        else
                        {
                            RsaPublicKeyConverter rpkc = new RsaPublicKeyConverter();
                            jws = new JWS_RS256_Param(rpkc.JwkToParam(jwkObject));
                        }
                    }
                }
                else
                {
                    // JwkSet使用のカバレッジ
                    // AuthZ側でClient側テストを行うためのカバレージ
                    JObject jwkObject = null;

                    if (ResourceLoader.Exists(OAuth2AndOIDCParams.JwkSetFilePath, false))
                    {
                        JwkSet jwkSet = JwkSet.LoadJwkSet(OAuth2AndOIDCParams.JwkSetFilePath);
                        jwkObject = JwkSet.GetJwkObject(jwkSet, jwsHeader.kid);
                    }

                    if (jwkObject == null)
                    {
                        // 証明書を使用
                        if (isJWE_FAPI2)
                        {
                            jws = new JWS_ES256_X509(CmnClientParams.EcdsaCerFilePath, "");
                        }
                        else
                        {
                            jws = new JWS_RS256_X509(CmnClientParams.RsaCerFilePath, "");
                        }
                    }
                    else
                    {
                        // Jwkを使用
                        if (isJWE_FAPI2)
                        {
                            EccPublicKeyConverter epkc = new EccPublicKeyConverter();
                            jws = new JWS_ES256_Param(epkc.JwkToParam(jwkObject), false);
                        }
                        else
                        {
                            RsaPublicKeyConverter rpkc = new RsaPublicKeyConverter();
                            jws = new JWS_RS256_Param(rpkc.JwkToParam(jwkObject));
                        }
                    }
                }
            }

            bool ret = jws.Verify(jwtToken);

            if (ret)
            {
                jwtPayload = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(jwtToken.Split('.')[1]), CustomEncode.us_ascii);
            }

            return ret;
        }

        /// <summary>
        /// OAuth2AndOIDCConst.jti
        /// </summary>
        /// <returns>
        /// JitClaim
        /// </returns>
        public static string CreateJitClaim()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// OAuth2AndOIDCConst.iat
        /// </summary>
        /// <returns>
        /// IatClaim
        /// </returns>
        public static string CreateIatClaim()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        }

        /// <summary>
        /// OAuth2AndOIDCConst.exp
        /// </summary>
        /// <param name="forExp">TimeSpan</param>
        /// <returns>
        /// ExpClaim
        /// </returns>
        public static string CreateExpClaim(TimeSpan forExp)
        {
            return DateTimeOffset.Now.Add(forExp).ToUnixTimeSeconds().ToString();
        }

        /// <summary>CheckClaims</summary>
        /// <param name="json">JObject</param>
        /// <param name="key">string</param>
        /// <param name="value">string</param>
        /// <param name="err">string</param>
        /// <param name="errDescription">string</param>
        /// <param name="nullable">bool</param>
        /// <returns>bool</returns>
        /// <remarks>
        /// - **無いクレームは server_error で返す**（従来どおり）。外から来た要求の不備なので、本来は invalid_request である。
        ///   **TryGetStringClaim を使う**（#662。このメソッドは Obsolete（警告）にした。振る舞いは下の 1 点を除いて変えていない）
        /// - **値がオブジェクト・配列なら false**（err は server_error）。2026/11/06 までは ArgumentException を投げていた（#662）
        /// - 数値・真偽値は文字列にして返し、明示的な null は value を null にして true を返す（従来どおり）
        /// </remarks>
        [Obsolete("This method returns server_error for a missing claim, please use TryGetStringClaim instead.")]
        public static bool CheckClaims(
            JObject json, string key, out string value,
            out string err, out string errDescription, bool nullable = false)
        {
            value = "";
            err = "";
            errDescription = "";

            if (json.ContainsKey(key))
            {
                if (json[key] is JContainer)
                {
                    // オブジェクト・配列は、文字列にできない（以前は ArgumentException。#662）
                    err = "server_error";
                    errDescription = "the value correspond this key is not a string: " + key;
                    return false;
                }

                value = (string)json[key];
                return true;
            }
            else
            {
                if (nullable)
                {
                    return true;
                }
                else
                {
                    err = "server_error";
                    errDescription = "the value correspond this key is not contained: " + key;
                    return false;
                }
            }
        }

        /// <summary>クレームの値を、文字列として取り出す（外から来た要求の検証に使う）</summary>
        /// <param name="json">ペイロード（JObject）</param>
        /// <param name="key">クレーム名</param>
        /// <param name="required">必須か。false なら、無くても true（value は null）</param>
        /// <param name="value">値（無ければ null）</param>
        /// <param name="err">取り出せなければ invalid_request。取り出せれば空文字</param>
        /// <param name="errDescription">取り出せなければ、その理由。取り出せれば空文字</param>
        /// <returns>取り出せたら true（必須でないクレームが無いときも true）</returns>
        /// <remarks>
        /// - **無い・読めないクレームは invalid_request**（CIBA Core 13・RFC 6749 5.2 : 必須のパラメタが無い、値が不正）。
        ///   CheckClaims は server_error を返していた（#662）
        /// - **文字列・数値・真偽値を、文字列として返す**（数値はカルチャに依らない字面）。
        ///   CIBA の requested_expiry のように、文字列でも数値でも来るクレームがある（CIBA Core 7.1.1）
        /// - **オブジェクト・配列・日付などは、読めないもの**として false。例外にしない（外から来た値なので）。
        ///   日付の形の文字列を日付に変えないよう、ペイロードは DateParseHandling.None で読んでおく
        /// - **明示的な null は、無いのと同じ**に扱う（必須なら false）
        /// - 空文字は、在るものとして返す（中身の妥当性は呼び出し側で見る）
        /// - json・key が null なら ArgumentNullException（呼び出し側の誤り）
        /// </remarks>
        public static bool TryGetStringClaim(
            JObject json, string key, bool required,
            out string value, out string err, out string errDescription)
        {
            if (json == null)
            {
                throw new ArgumentNullException("json");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            value = null;
            err = "";
            errDescription = "";

            JToken token = json[key];

            if (token == null || token.Type == JTokenType.Null)
            {
                if (!required)
                {
                    return true;
                }

                err = OAuth2AndOIDCConst.invalid_request;
                errDescription = "the value correspond this key is not contained: " + key;
                return false;
            }

            switch (token.Type)
            {
                case JTokenType.String:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.Boolean:
                    value = Convert.ToString(((JValue)token).Value, CultureInfo.InvariantCulture);
                    return true;

                default:
                    err = OAuth2AndOIDCConst.invalid_request;
                    errDescription = "the value correspond this key is not a string: " + key;
                    return false;
            }
        }

        #region ClaimVerifier
        /// <summary>VerifyExp</summary>
        /// <param name="exp">exp（NumericDate。整数・小数・指数表記）</param>
        /// <returns>いまが exp より前（または同じ秒）なら true。無い・読めない値は false</returns>
        /// <remarks>
        /// **小数の NumericDate も読む**（RFC 7519 2。#647）。2026/11/05 までは整数しか読めず、
        /// 小数や読めない値は例外（long.Parse）になっていた。
        /// </remarks>
        public static bool VerifyExp(string exp)
        {
            decimal seconds;
            return CmnJwtToken.TryParseNumericDate(exp, out seconds)
                && CmnJwtToken.IsNotExpired(seconds, DateTimeOffset.Now.ToUnixTimeSeconds());
        }

        /// <summary>VerifyNbf</summary>
        /// <param name="nbf">nbf（NumericDate。整数・小数・指数表記）</param>
        /// <returns>いまが nbf 以後なら true。無い・読めない値は false</returns>
        /// <remarks>VerifyExp と同じ（#647）。</remarks>
        public static bool VerifyNbf(string nbf)
        {
            decimal seconds;
            return CmnJwtToken.TryParseNumericDate(nbf, out seconds)
                && CmnJwtToken.IsNotBefore(seconds, DateTimeOffset.Now.ToUnixTimeSeconds());
        }

        /// <summary>exp のクレームを確かめる（トークンの検証の共通部分）</summary>
        /// <param name="exp">exp のクレーム（JToken。無ければ null）</param>
        /// <param name="unixTimeSeconds">いまの UNIX 時刻（秒）</param>
        /// <returns>いまが exp より前（または同じ秒）なら true。無い・読めない値は false</returns>
        /// <remarks>
        /// IdToken・AccessToken・JwtAssertion・ResponseObject の Verify が使う（#647）。
        /// **JWT のペイロード（JToken）を持つ呼び出し側も、これを使う**（#661 で公開した。文字列に直して VerifyExp に渡さない）。
        /// 読み方は TryReadNumericDate（acceptString は true）。**時計のずれの許容幅は取らない**（必要なら unixTimeSeconds で調整する）。
        /// **exp と同じ秒は通す**（RFC 7519 4.1.4 より 1 秒緩い。従来どおり）。
        /// </remarks>
        public static bool VerifyExpClaim(JToken exp, long unixTimeSeconds)
        {
            decimal seconds;
            return CmnJwtToken.TryReadNumericDate(exp, true, out seconds)
                && CmnJwtToken.IsNotExpired(seconds, unixTimeSeconds);
        }

        /// <summary>nbf のクレームを確かめる（トークンの検証の共通部分）</summary>
        /// <param name="nbf">nbf のクレーム（JToken。無ければ null）</param>
        /// <param name="unixTimeSeconds">いまの UNIX 時刻（秒）</param>
        /// <returns>いまが nbf 以後なら true。無い・読めない値は false</returns>
        /// <remarks>
        /// RequestObject の Verify・VerifyCiba が使う（#653）。読み方は VerifyExpClaim と同じ（#661 で公開した）。
        /// **無い nbf も false** なので、nbf を任意とするなら、呼び出し側で null を先に見る。
        /// </remarks>
        public static bool VerifyNbfClaim(JToken nbf, long unixTimeSeconds)
        {
            decimal seconds;
            return CmnJwtToken.TryReadNumericDate(nbf, true, out seconds)
                && CmnJwtToken.IsNotBefore(seconds, unixTimeSeconds);
        }

        /// <summary>いまが exp より前（または同じ秒）か</summary>
        /// <remarks>
        /// **小数はそのまま比べる。** いまの時刻は整数の秒なので、exp を切り捨ててから比べるのと同じ結果になり、
        /// 小数を受けても有効期間は広がらない（RFC 7519 4.1.4。以前の long の比較と同じく、同じ秒は通す）。
        /// </remarks>
        private static bool IsNotExpired(decimal exp, long unixTimeSeconds)
        {
            return exp >= unixTimeSeconds;
        }

        /// <summary>いまが nbf 以後か</summary>
        /// <remarks>
        /// **小数はそのまま比べる。** nbf を切り上げてから比べるのと同じ結果になる（RFC 7519 4.1.5）。
        /// </remarks>
        private static bool IsNotBefore(decimal nbf, long unixTimeSeconds)
        {
            return nbf <= unixTimeSeconds;
        }

        /// <summary>NumericDate（RFC 7519 2）を読む</summary>
        /// <param name="value">クレームの値（JToken。無ければ null）</param>
        /// <param name="acceptString">
        /// 数値の文字列も受けるか。**NumericDate は JSON の数値**だが、exp・nbf は従来から文字列でも読んでいた
        /// （CreateExpClaim などが文字列を返す）ので、互換のために受ける。DPoP の iat は数値だけを受ける（false）
        /// </param>
        /// <param name="seconds">UNIX 時刻（秒。小数を含みうる）</param>
        /// <returns>読めたら true</returns>
        /// <remarks>
        /// **整数・小数・指数表記を読む**（RFC 7519 2 : NumericDate は JSON の数値で、整数でない値も表せる。#647）。
        /// **カルチャに依らない**（CultureInfo.InvariantCulture）。long の範囲を超える値・数値でない値は読めない（false）。
        /// **net48 と net10.0 で結果を揃える**（小数の double を 17 桁で読む）。
        /// 読めなければ seconds は 0。exp・nbf・iat を自前で読む呼び出し側も、これを使う（#661 で公開した）。
        /// </remarks>
        public static bool TryReadNumericDate(JToken value, bool acceptString, out decimal seconds)
        {
            seconds = 0;

            if (value == null)
            {
                return false;
            }

            switch (value.Type)
            {
                case JTokenType.Integer:
                case JTokenType.Float:
                    // JSON の数値を、カルチャに依らない字面にしてから decimal で読み直す。
                    //   **小数は double で届くので、double の値を落とさない 17 桁（"G17"）で字面にする。**
                    //   既定の書式は net48 では 15 桁で、10 桁の秒だと小数が丸まり、exp が最大 1 秒長く通っていた
                    //   （net10.0 の既定は最短の往復できる書式。"G17" で両者を揃える。
                    //    .NET Framework の "R" には往復できない値があるので使わない）。
                    object raw = ((JValue)value).Value;
                    string text = (raw is double)
                        ? ((double)raw).ToString("G17", CultureInfo.InvariantCulture)
                        : ((JValue)value).ToString(CultureInfo.InvariantCulture);
                    return CmnJwtToken.TryParseNumericDate(text, out seconds);

                case JTokenType.String:
                    return acceptString && CmnJwtToken.TryParseNumericDate((string)value, out seconds);

                default:
                    return false;
            }
        }

        /// <summary>NumericDate の文字列を読む（整数・小数・指数表記。カルチャに依らない）</summary>
        /// <param name="value">文字列</param>
        /// <param name="seconds">UNIX 時刻（秒）</param>
        /// <returns>読めたら true</returns>
        /// <remarks>
        /// **long の範囲を超える値は読めない（false）とする。** 以前（long.Parse）は例外で受けなかった範囲で、
        /// 受ける範囲を以前と同じにするため（極端に遠い未来の exp を通さない）。
        /// </remarks>
        private static bool TryParseNumericDate(string value, out decimal seconds)
        {
            if (!decimal.TryParse(value,
                NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture, out seconds))
            {
                return false;
            }

            if (seconds > long.MaxValue || seconds < long.MinValue)
            {
                seconds = 0;
                return false;
            }

            return true;
        }

        /// <summary>
        /// aud が、設定の client_id（OAuth2AndOIDCParams.ClientID / ClientIDs）の
        /// いずれかを含むことを確認する。
        /// </summary>
        /// <param name="aud">aud（string か array）</param>
        /// <returns>bool</returns>
        /// <remarks>
        /// 設定で判定する IdToken.Verify / ResponseObject.Verify / AccessToken.Verify が使う（#603・#655）。
        /// 認可サーバを兼ねるか（JwkSetFilePath の有無）によらず、同じ判定をする。
        ///
        /// - **aud は文字列か、文字列の配列に限る**（ContainsAud。RFC 7519 4.1.3）。数値・オブジェクト・入れ子の配列、
        ///   文字列でない要素を含む配列は false（例外にしない）。2026/11/05 までは、数値は文字列に変えて照合し、
        ///   オブジェクトや入れ子の配列は例外（InvalidCastException）になっていた（#655 のレビュー）
        /// - 設定の読み方は ReadConfiguredClientIDs（空白だけの値は無いものとし、設定の JSON が壊れていれば false）
        /// - 設定が無ければ、一致するものが無いので false
        /// </remarks>
        internal static bool VerifyAudByClientIDs(JToken aud)
        {
            List<string> clientIDs = CmnJwtToken.ReadConfiguredClientIDs();

            if (clientIDs == null)
            {
                // 設定の誤り（JSON が壊れている）
                return false;
            }

            return CmnJwtToken.ContainsAud(aud, clientIDs);
        }

        /// <summary>設定の client_id（OAuth2AndOIDCParams.ClientID と ClientIDs を合わせたもの）を読む</summary>
        /// <returns>
        /// client_id の一覧（空白だけの値は除く。どちらも無ければ空）。
        /// ClientIDs が読めなければ null（設定の誤り。net48 は JSON が壊れている、net10.0 は文字列の配列としてバインドできない。呼び出し側は断る）
        /// </returns>
        /// <remarks>
        /// **1 回の検証で 1 回だけ読む**（net48 は、読むたびに JSON を読み直すため。#655 のレビュー）。
        /// 空白だけの値（環境変数の置き換えで空になった値など）は、設定が無いのと同じに扱う。
        /// </remarks>
        internal static List<string> ReadConfiguredClientIDs()
        {
            List<string> clientIDs = new List<string>();

            string clientID = OAuth2AndOIDCParams.ClientID;
            if (!string.IsNullOrWhiteSpace(clientID))
            {
                clientIDs.Add(clientID);
            }

#if NETSTD
            // **net10.0 の構成のバインドは、形が違っても例外にしない**（文字列の配列でない要素は飛ばし、
            //   配列でない値は null になる）。そのままでは「設定が無い」と同じになり、確かめない側に倒れるので、
            //   形を先に見て、文字列の配列でなければ設定の誤りとする（#655 の 2 回目のレビュー）。
            IConfigurationSection section = GetConfigParameter.GetAnyConfigSection("appSettings:OAuth2AndOidcClientIDs");
            if (section.GetChildren().Any(x => x.GetChildren().Any()) ||
                (!string.IsNullOrWhiteSpace(section.Value) && !section.GetChildren().Any()))
            {
                // 要素がオブジェクト・配列、または配列でなく 1 つの値
                return null;
            }
#endif

            List<string> configured;
            try
            {
                configured = OAuth2AndOIDCParams.ClientIDs;
            }
            catch (JsonException)
            {
                // OAuth2AndOidcClientIDs の JSON が壊れている（net48）
                return null;
            }
            catch (InvalidOperationException)
            {
                // OAuth2AndOidcClientIDs を、文字列の一覧としてバインドできない（net10.0）
                return null;
            }

            clientIDs.AddRange(configured.Where(x => !string.IsNullOrWhiteSpace(x)));
            return clientIDs;
        }
        #endregion

        #region クレームの読み取り（RequestObject・JwtAssertion。#653・#654）

        /// <summary>JWS の署名を確かめる（形の壊れた入力でも例外にしない）</summary>
        /// <param name="jws">検証鍵</param>
        /// <param name="jwt">JWS（コンパクト形式。3 区切り）</param>
        /// <returns>署名が正しければ true。形が壊れていれば false</returns>
        /// <remarks>
        /// JWS.Verify は、区切りが足りない・Base64url でない・ヘッダが JSON のオブジェクトでない入力で、
        /// 例外（IndexOutOfRangeException・FormatException・JsonException・NullReferenceException）を投げる。
        /// 外から来る値を検証する口（要求オブジェクト・アサーション）は、それを false にそろえる（#654 のレビュー）。
        ///
        /// **ヘッダの alg が、検証鍵の alg（JWS.JWSHeader.alg）と違えば false**（大文字・小文字を区別する。RFC 8725 3.1。#664）。
        /// 基盤の JWS の型（JWS_RSA・JWS_ECDSA・JWS_HMACSHA）は、Verify の中でも同じ照合をしている。
        /// 呼び出し側が JWS を渡せる版（#664）で、照合しない派生を渡されても、alg の取り違えを通さないために、ここでも見る。
        /// </remarks>
        internal static bool TryVerifySignature(JWS jws, string jwt)
        {
            if (string.IsNullOrEmpty(jwt) || jwt.Split('.').Length != 3)
            {
                return false;
            }

            if (JwtReader.TryReadHeader(jwt) == null)
            {
                // ヘッダが読めない（JSON のオブジェクトでない）
                return false;
            }

            if (jws == null || jws.JWSHeader == null || string.IsNullOrEmpty(jws.JWSHeader.alg) ||
                !string.Equals(JwtReader.TryReadAlg(jwt), jws.JWSHeader.alg, StringComparison.Ordinal))
            {
                // ヘッダの alg が、検証鍵の alg と違う（無い・文字列でないものも）
                return false;
            }

            try
            {
                return jws.Verify(jwt);
            }
            catch (FormatException)
            {
                // Base64url でない
                return false;
            }
            catch (JsonException)
            {
                // ヘッダの値の型が違う
                return false;
            }
            catch (CryptographicException)
            {
                // 署名の長さが違うなど
                return false;
            }
        }

        /// <summary>JWS のペイロードを読む（署名を確かめた後に呼ぶ）</summary>
        /// <param name="jws">JWS</param>
        /// <returns>JSON のオブジェクト。オブジェクトでなければ null</returns>
        /// <remarks>
        /// **日付の形の文字列を、日付に変えずに読む**（DateParseHandling.None）。
        /// 既定（DateTime）では "2026-11-05T00:00:00Z" のような値が JTokenType.Date になり、
        /// 文字列のクレーム（ReadStringClaim）として読めず、正しい JWT を断ってしまう（#653 ph1 のレビュー）。
        /// </remarks>
        internal static JObject ReadPayloadObject(string jws)
        {
            try
            {
                string jwtPayload = CustomEncode.ByteToString(
                    CustomEncode.FromBase64UrlString(jws.Split('.')[1]), CustomEncode.UTF_8);

                return JsonConvert.DeserializeObject(jwtPayload,
                    new JsonSerializerSettings { DateParseHandling = DateParseHandling.None }) as JObject;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>文字列のクレームを読む</summary>
        /// <param name="jobj">ペイロード</param>
        /// <param name="name">クレーム名</param>
        /// <returns>空でない文字列なら、その値。無い・空・文字列でないなら null</returns>
        internal static string ReadStringClaim(JObject jobj, string name)
        {
            JToken value = jobj[name];

            if (value == null || value.Type != JTokenType.String)
            {
                return null;
            }

            string text = (string)value;
            return string.IsNullOrEmpty(text) ? null : text;
        }

        /// <summary>aud が在るか（文字列、または文字列の配列。RFC 7519 4.1.3）。期待する値があれば照合する</summary>
        /// <param name="aud">aud のクレーム</param>
        /// <param name="expectedAuds">期待する aud の集合。null なら照合しない（在ることだけを見る）</param>
        /// <returns>
        /// expectedAuds が null なら、空でない値を 1 つ以上含めば true。
        /// expectedAuds があれば、そのいずれかと一致する値を含めば true（大文字・小文字を区別する完全一致。正規化しない）。
        /// 配列に文字列でない要素があれば false
        /// </returns>
        internal static bool ContainsAud(JToken aud, IEnumerable<string> expectedAuds)
        {
            Func<string, bool> match = value => (expectedAuds == null)
                ? !string.IsNullOrEmpty(value)
                : expectedAuds.Any(x => !string.IsNullOrEmpty(x) && string.Equals(value, x, StringComparison.Ordinal));

            if (aud == null)
            {
                return false;
            }
            else if (aud.Type == JTokenType.String)
            {
                return match((string)aud);
            }
            else if (aud.Type == JTokenType.Array)
            {
                bool any = false;
                foreach (JToken item in (JArray)aud)
                {
                    if (item.Type != JTokenType.String)
                    {
                        // 文字列でない要素は受けない
                        return false;
                    }

                    any |= match((string)item);
                }
                return any;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}

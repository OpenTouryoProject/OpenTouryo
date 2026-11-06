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
//* クラス名        ：RequestObject
//* クラス日本語名  ：RequestObject
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/19  西野 大介         新規作成
//*  2020/03/03  西野 大介         CIBA対応
//*  2026/11/05  玄人 幸道         Verify・VerifyCiba：必須のクレームが欠けていたら false にし、exp・nbf の値を確かめる（#653）。
//*                                必須の範囲を仕様に合わせた（Verify は state・nonce を外し、VerifyCiba は iat・jti を足して
//*                                client_notification_token・binding_message を外し、ヒントはちょうど 1 つ）
//*  2026/11/05  玄人 幸道         ペイロードを、日付の形の文字列を日付に変えずに読む（DateParseHandling.None）。
//*                                iat の確認を VerifyTimeClaims にまとめた（#653 ph1 のレビューの指摘 1・2）
//*  2026/11/05  玄人 幸道         Verify・VerifyCiba に、期待する aud を受けて照合するオーバーロードを追加（#653 ph2）
//*  2026/11/05  玄人 幸道         aud を受ける版の remarks に、正規化しないことを書いた（#653 ph2 のレビューの指摘 3）
//*  2026/11/05  玄人 幸道         aud を受けない Verify・VerifyCiba を Obsolete（警告）にした（#653）
//*  2026/11/05  玄人 幸道         クレームの読み取りを CmnJwtToken に移した（JwtAssertion と共有する。#654。振る舞いは変えていない）
//*  2026/11/05  玄人 幸道         形の壊れた入力でも、例外にせず false を返す（CmnJwtToken.TryVerifySignature。#654 のレビューの指摘 5）
//*  2026/11/06  玄人 幸道         Create に、有効期間を受けて exp・nbf・iat を入れるオーバーロードを追加（#660）
//*  2026/11/06  玄人 幸道         remarks に、時計のずれの余裕を取らないこと・上限で溢れないことを書いた（#660 のレビューの指摘 2・3）
//*  2026/11/06  玄人 幸道         Verify・VerifyCiba に、検証鍵（JWS）を受けて署名の alg を選べるオーバーロードを追加（#664）
//*  2026/11/06  玄人 幸道         remarks に、登録どおりの鍵の型だけを渡すことを書いた（#664 のレビューの指摘 2）
//**********************************************************************************

using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>
    /// OpenID Connect > RequestObject
    /// </summary>
    public class RequestObject
    {
        #region FAPI2 CC

        // OpenID Connect Core 1.0 incorporating errata set 1
        // 6.1.  Passing a Request Object by Value
        // https://openid.net/specs/openid-connect-core-1_0.html#RequestObject
        //  {
        //   "iss": "s6BhdRkqt3",
        //   "aud": "https://server.example.com",
        //   "response_type": "code id_token",
        //   "client_id": "s6BhdRkqt3",
        //   "redirect_uri": "https://client.example.org/cb",
        //   "scope": "openid",
        //   "state": "af0ifjsldkj",
        //   "nonce": "n-0S6_WzA2Mj",
        //   "max_age": 86400,
        //   "claims": ... see : ClaimsInRO.cs
        //  }

        // 以下はI/F上に含めない。
        // - display ... promptの形式
        // - ui_locales ... UICulture的な
        // - id_token_hint ... 以前のid_token（再認証）

        #region Create
        /// <summary>Create</summary>
        /// <param name="iss">string</param>
        /// <param name="aud">string</param>
        /// <param name="response_type">string</param>
        /// <param name="response_mode">string</param>
        /// <param name="redirect_uri">string</param>
        /// <param name="scopes">string</param>
        /// <param name="state">string</param>
        /// <param name="nonce">string</param>
        /// <param name="max_age">string</param>
        /// <param name="prompt">string</param>
        /// <param name="login_hint">string</param>
        /// <param name="claims">ClaimsInRO</param>
        /// <param name="jwkPrivateKey">RS256用のJWK秘密鍵</param>
        /// <returns>RequestObject</returns>
        public static string Create(
            string iss, string aud, string response_type, string response_mode,
            string redirect_uri, string scopes, string state, string nonce,
            string max_age, string prompt, string login_hint, ClaimsInRO claims, string jwkPrivateKey)
        {
            RsaPrivateKeyConverter rpkc = new RsaPrivateKeyConverter();
            return RequestObject.Create(
                iss, aud, response_type, response_mode,
                redirect_uri, scopes, state, nonce,
                max_age, prompt, login_hint, claims,
                rpkc.JwkToParam(jwkPrivateKey));
        }

        /// <summary>Create</summary>
        /// <param name="iss">string</param>
        /// <param name="aud">string</param>
        /// <param name="response_type">string</param>
        /// <param name="response_mode">string</param>
        /// <param name="redirect_uri">string</param>
        /// <param name="scopes">string</param>
        /// <param name="state">string</param>
        /// <param name="nonce">string</param>
        /// <param name="max_age">string</param>
        /// <param name="prompt">string</param>
        /// <param name="login_hint">string</param>
        /// <param name="claims">ClaimsInRO</param>
        /// <param name="rsaPrivateKey">RS256用のRSAParameters秘密鍵</param>
        /// <returns>RequestObject</returns>
        public static string Create(
            string iss, string aud, string response_type, string response_mode,
            string redirect_uri, string scopes, string state, string nonce,
            string max_age, string prompt, string login_hint, ClaimsInRO claims, RSAParameters rsaPrivateKey)
        {
            // exp・nbf・iat は入れない（従来どおり）
            return RequestObject.CreateCore(
                iss, aud, response_type, response_mode,
                redirect_uri, scopes, state, nonce,
                max_age, prompt, login_hint, claims,
                null, rsaPrivateKey);
        }

        /// <summary>Create（exp・nbf・iat を入れる）</summary>
        /// <param name="iss">string</param>
        /// <param name="aud">string</param>
        /// <param name="response_type">string</param>
        /// <param name="response_mode">string</param>
        /// <param name="redirect_uri">string</param>
        /// <param name="scopes">string</param>
        /// <param name="state">string</param>
        /// <param name="nonce">string</param>
        /// <param name="max_age">string</param>
        /// <param name="prompt">string</param>
        /// <param name="login_hint">string</param>
        /// <param name="claims">ClaimsInRO</param>
        /// <param name="lifetime">有効期間（1 秒以上。exp = nbf + 有効期間）</param>
        /// <param name="jwkPrivateKey">RS256用のJWK秘密鍵</param>
        /// <returns>RequestObject</returns>
        /// <remarks>
        /// 振る舞いは RSAParameters を受ける版（有効期間を受けるもの）の remarks を参照。
        /// </remarks>
        public static string Create(
            string iss, string aud, string response_type, string response_mode,
            string redirect_uri, string scopes, string state, string nonce,
            string max_age, string prompt, string login_hint, ClaimsInRO claims,
            TimeSpan lifetime, string jwkPrivateKey)
        {
            RsaPrivateKeyConverter rpkc = new RsaPrivateKeyConverter();
            return RequestObject.Create(
                iss, aud, response_type, response_mode,
                redirect_uri, scopes, state, nonce,
                max_age, prompt, login_hint, claims,
                lifetime, rpkc.JwkToParam(jwkPrivateKey));
        }

        /// <summary>Create（exp・nbf・iat を入れる）</summary>
        /// <param name="iss">string</param>
        /// <param name="aud">string</param>
        /// <param name="response_type">string</param>
        /// <param name="response_mode">string</param>
        /// <param name="redirect_uri">string</param>
        /// <param name="scopes">string</param>
        /// <param name="state">string</param>
        /// <param name="nonce">string</param>
        /// <param name="max_age">string</param>
        /// <param name="prompt">string</param>
        /// <param name="login_hint">string</param>
        /// <param name="claims">ClaimsInRO</param>
        /// <param name="lifetime">有効期間（1 秒以上。exp = nbf + 有効期間）</param>
        /// <param name="rsaPrivateKey">RS256用のRSAParameters秘密鍵</param>
        /// <returns>RequestObject</returns>
        /// <remarks>
        /// - **nbf・iat に今の時刻（UTC）、exp に nbf + 有効期間を入れる**（#660）。
        ///   FAPI（1.0 Advanced 5.2.3-11・14）は、クライアントに exp・nbf を送ることを求めている。
        ///   有効期間を受けない版は、どれも入れない（従来どおり）
        /// - 値は **JSON の数値**（秒。小数は切り捨て）で入れる（RFC 7519 2 の NumericDate）。
        ///   CreateCiba・CmnJwtToken.CreateExpClaim は文字列で入れているが、こちらは数値にした
        /// - **有効期間の上限は見ない。** FAPI の「60 分以内」はプロファイルの制約なので、呼び出し側が決める。
        ///   TimeSpan.MaxValue（約 9.2 × 10^11 秒）でも long は溢れない
        /// - **nbf は今の時刻ちょうどで、時計のずれの余裕を取らない。** 認可サーバの時計がクライアントより遅れていて、
        ///   nbf を許容幅なしで確かめる相手（この基盤の Verify も許容幅は 0）なら、nbf で断られうる
        /// - 1 秒未満の有効期間は ArgumentOutOfRangeException
        /// </remarks>
        public static string Create(
            string iss, string aud, string response_type, string response_mode,
            string redirect_uri, string scopes, string state, string nonce,
            string max_age, string prompt, string login_hint, ClaimsInRO claims,
            TimeSpan lifetime, RSAParameters rsaPrivateKey)
        {
            if (lifetime < TimeSpan.FromSeconds(1))
            {
                throw new ArgumentOutOfRangeException("lifetime", lifetime, "The lifetime must be at least one second.");
            }

            return RequestObject.CreateCore(
                iss, aud, response_type, response_mode,
                redirect_uri, scopes, state, nonce,
                max_age, prompt, login_hint, claims,
                lifetime, rsaPrivateKey);
        }

        /// <summary>Create の本体</summary>
        /// <param name="lifetime">有効期間。null なら exp・nbf・iat を入れない</param>
        private static string CreateCore(
            string iss, string aud, string response_type, string response_mode,
            string redirect_uri, string scopes, string state, string nonce,
            string max_age, string prompt, string login_hint, ClaimsInRO claims,
            TimeSpan? lifetime, RSAParameters rsaPrivateKey)
        {
            string json = "";

            #region ClaimSetの生成

            Dictionary<string, object> requestObjectClaimSet = new Dictionary<string, object>();

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.iss, iss); // client_id
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.aud, aud); // ROS EndPointのuri。

            if (lifetime.HasValue)
            {
                // NumericDate（JSON の数値）で入れる
                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.exp, now + (long)lifetime.Value.TotalSeconds);
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.nbf, now);
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.iat, now);
            }

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.response_type, response_type);
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.client_id, iss);

            if (!string.IsNullOrEmpty(response_mode))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.response_mode, response_mode);
            if (!string.IsNullOrEmpty(redirect_uri))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.redirect_uri, redirect_uri);

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.scope, scopes);
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.state, state);

            if (!string.IsNullOrEmpty(nonce))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.nonce, nonce);
            if (!string.IsNullOrEmpty(max_age))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.max_age, max_age);
            if (!string.IsNullOrEmpty(prompt))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.prompt, prompt);
            if (!string.IsNullOrEmpty(login_hint))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.login_hint, login_hint);

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.claims, claims.Claims);

            json = JsonConvert.SerializeObject(requestObjectClaimSet);

            #endregion

            #region JWT化

            JWS_RS256_Param jwtRS256 = new JWS_RS256_Param(rsaPrivateKey);
            return jwtRS256.Create(json);

            #endregion
        }
        #endregion

        #region Verify
        /// <summary>Verify</summary>
        /// <param name="ro">string</param>
        /// <param name="iss">string</param>
        /// <param name="jwkPublicKey">RS256用のJWK公開鍵</param>
        /// <returns>検証結果</returns>
        [Obsolete("This overload does not verify the aud claim, please use the overload that takes the expected aud instead.")]
        public static bool Verify(string ro, out string iss, string jwkPublicKey)
        {
            RsaPublicKeyConverter rpkc = new RsaPublicKeyConverter();
            return RequestObject.Verify(ro, out iss,
                rpkc.JwkToParam(jwkPublicKey));
        }

        /// <summary>Verify</summary>
        /// <param name="ro">string</param>
        /// <param name="iss">string</param>
        /// <param name="rsaPublicKey">RS256用のRSAParameters公開鍵</param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// **署名に加えて、必須のクレームと、時刻のクレームを確かめる**（#653）。
        /// 2026/11/05 までは、署名が正しければクレームによらず true を返していた。
        /// - 必須：iss・aud（RFC 9101 4 : 署名した Request Object は iss・aud を含める）、
        ///   response_type・client_id（RFC 6749 4.1.1。RFC 9101 6.3 : AS は Request Object の中のパラメタだけを使う）、
        ///   scope（OIDC Core 3.1.2.1）。**空の文字列は無いのと同じ**に扱う
        /// - **state・nonce は必須にしない**（どちらも RFC 6749・OIDC Core の認可コード フローでは任意。Create も空なら入れない）
        /// - exp・nbf は、**在れば**値を確かめる（RFC 7519 4.1.4・4.1.5。読み方は CmnJwtToken.TryReadNumericDate。#647）。
        ///   有効期間を受けない Create は exp を入れないので、無くてもよい（受ける版は入れる。#660）
        /// - **aud の値は照合しない**（期待する値を受けない版のため）。**照合するなら aud を受ける版を使う**（#653 ph2）
        /// 欠けている・読めない・期限の過ぎたものは false（例外にしない）。iss は、署名が正しければ読めた値を返す。
        /// </remarks>
        [Obsolete("This overload does not verify the aud claim, please use the overload that takes the expected aud instead.")]
        public static bool Verify(string ro, out string iss, RSAParameters rsaPublicKey)
        {
            return RequestObject.VerifyCore(ro, null, out iss, new JWS_RS256_Param(rsaPublicKey));
        }

        /// <summary>Verify（期待する aud と照合する）</summary>
        /// <param name="ro">string</param>
        /// <param name="aud">期待する aud（認可サーバの issuer。RFC 9101 4）</param>
        /// <param name="iss">string</param>
        /// <param name="jwkPublicKey">RS256用のJWK公開鍵</param>
        /// <returns>検証結果</returns>
        /// <remarks>aud を受けない版と同じ確認に加えて、aud を照合する（RSAParameters の版を参照）。</remarks>
        public static bool Verify(string ro, string aud, out string iss, string jwkPublicKey)
        {
            RsaPublicKeyConverter rpkc = new RsaPublicKeyConverter();
            return RequestObject.Verify(ro, aud, out iss,
                rpkc.JwkToParam(jwkPublicKey));
        }

        /// <summary>Verify（期待する aud と照合する）</summary>
        /// <param name="ro">string</param>
        /// <param name="aud">期待する aud（認可サーバの issuer。RFC 9101 4）</param>
        /// <param name="iss">string</param>
        /// <param name="rsaPublicKey">RS256用のRSAParameters公開鍵</param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// aud を受けない版と同じ確認に加えて、**aud が期待する値と一致する**（配列なら、それを含む）ことを確かめる（#653 ph2）。
        /// 比較は大文字・小文字を区別する完全一致（RFC 7519 4.1.3 : StringOrURI の比較）。
        /// **正規化はしない**（末尾のスラッシュの有無も別物）ので、メタデータの issuer の文字列をそのまま渡す。
        /// RFC 9101 4 は、aud を認可サーバの issuer（RFC 8414）にすると書く。何を期待するかは呼び出し側が決める。
        /// </remarks>
        /// <exception cref="ArgumentNullException">aud が null か空</exception>
        public static bool Verify(string ro, string aud, out string iss, RSAParameters rsaPublicKey)
        {
            if (string.IsNullOrEmpty(aud))
            {
                throw new ArgumentNullException(nameof(aud));
            }

            return RequestObject.VerifyCore(ro, aud, out iss, new JWS_RS256_Param(rsaPublicKey));
        }

        /// <summary>Verify（期待する aud と照合する。署名の alg は、渡した検証鍵で選ぶ）</summary>
        /// <param name="ro">string</param>
        /// <param name="aud">期待する aud（認可サーバの issuer。RFC 9101 4）</param>
        /// <param name="iss">string</param>
        /// <param name="jws">
        /// 署名の検証に使う、クライアントの鍵（JWS_PS256_Param など）。
        /// JWK からなら JwsFactory.CreateFromJwk(alg, jwk) で作る
        /// </param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// RSAParameters の版と同じ確認をする。違いは、**署名の alg を呼び出し側が選べる**ことだけである（#664）。
        /// FAPI 1.0 Advanced 8.6 は、PS256 か ES256 を使うとする（RS256 は使わないほうがよい）。
        /// - **ヘッダの alg が、渡した鍵の alg と違えば false**（大文字・小文字を区別する。RFC 8725 3.1）
        /// - **受ける alg は、呼び出し側が決める**（クライアントの登録など）。ヘッダの alg を読んで鍵を作ると、照合の意味が無くなる
        /// - 渡せる鍵の型は制限しない
        /// - **鍵の型は、クライアントの登録どおりのものだけを渡す。** 公開鍵の値を HS* の鍵に使うと、alg の取り違えになる（RFC 8725 3.1 : 鍵は 1 つの alg だけで使う）
        /// </remarks>
        /// <exception cref="ArgumentNullException">aud が null か空、jws が null</exception>
        public static bool Verify(string ro, string aud, out string iss, JWS jws)
        {
            if (string.IsNullOrEmpty(aud))
            {
                throw new ArgumentNullException(nameof(aud));
            }

            if (jws == null)
            {
                throw new ArgumentNullException(nameof(jws));
            }

            return RequestObject.VerifyCore(ro, aud, out iss, jws);
        }

        /// <summary>Verify の本体</summary>
        /// <param name="ro">string</param>
        /// <param name="expectedAud">期待する aud。null なら照合しない</param>
        /// <param name="iss">string</param>
        /// <param name="jws">検証鍵（ヘッダの alg と、この鍵の alg が一致すること。#664）</param>
        /// <returns>検証結果</returns>
        private static bool VerifyCore(string ro, string expectedAud, out string iss, JWS jws)
        {
            iss = "";

            if (!CmnJwtToken.TryVerifySignature(jws, ro))
            {
                // JWTの署名検証に失敗
                return false;
            }

            JObject jobj = RequestObject.ReadPayload(ro);

            if (jobj == null)
            {
                // ペイロードが JSON のオブジェクトでない
                return false;
            }

            iss = RequestObject.ReadString(jobj, OAuth2AndOIDCConst.iss) ?? "";

            if (string.IsNullOrEmpty(iss) ||
                !RequestObject.HasAud(jobj[OAuth2AndOIDCConst.aud], expectedAud) ||
                RequestObject.ReadString(jobj, OAuth2AndOIDCConst.response_type) == null ||
                RequestObject.ReadString(jobj, OAuth2AndOIDCConst.client_id) == null ||
                RequestObject.ReadString(jobj, OAuth2AndOIDCConst.scope) == null)
            {
                // 必須項目の不足
                return false;
            }

            // 時刻のクレーム（在れば確かめる）
            return RequestObject.VerifyTimeClaims(jobj, false);
        }
        #endregion

        #endregion

        #region FAPI CIBA

        // - OpenID Connect Client Initiated Backchannel Authentication Flow - Core 1.0 draft-01
        //   https://openid.net/specs/openid-client-initiated-backchannel-authentication-core-1_0.html
        // - Financial-grade API: Client Initiated Backchannel Authentication Profile
        //   https://openid.net/specs/openid-financial-api-ciba-ID1.html

        #region Create
        ///// <summary>CreateCiba</summary>
        ///// <param name="iss">string</param>
        ///// <param name="aud">string</param>
        ///// <param name="exp">string</param>
        ///// <param name="nbf">string</param>
        ///// <param name="scopes">string</param>
        ///// <param name="client_notification_token">string</param>
        ///// <param name="binding_message">string</param>
        ///// <param name="user_code">string</param>
        ///// <param name="requested_expiry">string</param>
        ///// <param name="login_hint">string</param>
        ///// <param name="requestContextAndIntent">Dictionary(string, object)</param>
        ///// <param name="jwkPrivateKey">ES256用のJWK秘密鍵</param>
        ///// <returns>RequestObject</returns>
        //public static string CreateCiba(
        //    string iss, string aud, string exp, string nbf, string scopes,
        //    string client_notification_token, string binding_message,
        //    string user_code, string requested_expiry, string login_hint,
        //    Dictionary<string, object> requestContextAndIntent, string jwkPrivateKey)
        //{   
        //    EccPrivateKeyConverter epkc = new EccPrivateKeyConverter();
        //    return RequestObject.CreateCiba(
        //        iss, aud, exp, nbf, scopes, 
        //        client_notification_token, binding_message,
        //        user_code, requested_expiry, login_hint,
        //        requestContextAndIntent, epkc.JwkToParam(jwkPrivateKey));
        //}

        /// <summary>CreateCiba</summary>
        /// <param name="iss">string</param>
        /// <param name="aud">string</param>
        /// <param name="exp">string</param>
        /// <param name="nbf">string</param>
        /// <param name="scopes">string</param>
        /// <param name="client_notification_token">string</param>
        /// <param name="binding_message">string</param>
        /// <param name="user_code">string</param>
        /// <param name="requested_expiry">string</param>
        /// <param name="login_hint">string</param>
        /// <param name="requestContextAndIntent">Dictionary(string, object)</param>
        /// <param name="ecdsaX509FilePath">ES256用の X.509秘密鍵 の File Path</param>
        /// <param name="ecdsaX509Password">ES256用の X.509秘密鍵 の Password</param>
        /// <returns>RequestObject</returns>
        public static string CreateCiba(
            string iss, string aud, string exp, string nbf, string scopes,
            string client_notification_token, string binding_message,
            string user_code, string requested_expiry, string login_hint,
            Dictionary<string, object> requestContextAndIntent,
            string ecdsaX509FilePath, string ecdsaX509Password)
        ///// <param name="ecPrivateKey">ES256用のECParameters秘密鍵</param>
        //ECParameters ecPrivateKey) // ECDsa.ExportParameters(true)が動かねぇ。
        {
            string json = "";

            #region ClaimSetの生成

            Dictionary<string, object> requestObjectClaimSet = new Dictionary<string, object>();

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.iss, iss); // client_id
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.aud, aud); // ROS EndPointのuri。
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.exp, exp);
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.iat, CmnJwtToken.CreateIatClaim());
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.nbf, nbf);
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.jti, CmnJwtToken.CreateJitClaim());
            
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.scope, scopes);
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.client_notification_token, client_notification_token);
            requestObjectClaimSet.Add(OAuth2AndOIDCConst.binding_message, binding_message);
            
            if (!string.IsNullOrEmpty(user_code))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.user_code, user_code);
            if (!string.IsNullOrEmpty(requested_expiry))
                requestObjectClaimSet.Add(OAuth2AndOIDCConst.requested_expiry, requested_expiry);

            requestObjectClaimSet.Add(OAuth2AndOIDCConst.login_hint, login_hint);

            if (requestContextAndIntent != null)
            {
                foreach (string key in requestContextAndIntent.Keys)
                {
                    requestObjectClaimSet.Add(key, requestContextAndIntent[key]);
                }   
            }   

            json = JsonConvert.SerializeObject(requestObjectClaimSet);

            #endregion

            #region JWT化

            JWS_ES256_X509 jwtES256 = new JWS_ES256_X509(ecdsaX509FilePath, ecdsaX509Password);
            return jwtES256.Create(json);

            #endregion
        }
        #endregion

        #region Verify
        /// <summary>VerifyCiba</summary>
        /// <param name="ro">string</param>
        /// <param name="iss">string</param>
        /// <param name="jwkPublicKey">ES256用のJWK公開鍵</param>
        /// <returns>検証結果</returns>
        [Obsolete("This overload does not verify the aud claim, please use the overload that takes the expected aud instead.")]
        public static bool VerifyCiba(string ro, out string iss, string jwkPublicKey)
        {
            EccPublicKeyConverter epkc = new EccPublicKeyConverter();
            return RequestObject.VerifyCiba(ro, out iss,
                epkc.JwkToParam(jwkPublicKey));
        }

        /// <summary>VerifyCiba</summary>
        /// <param name="ro">string</param>
        /// <param name="iss">string</param>
        /// <param name="ecPublicKey">ES256用のECParameters公開鍵</param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// **署名に加えて、必須のクレームと、時刻のクレームを確かめる**（#653）。
        /// 2026/11/05 までは、署名が正しければクレームによらず true を返していた。
        /// - 必須：aud・iss・exp・iat・nbf・jti（CIBA Core 7.1.1 : 署名した認証要求の JWT のクレーム）、
        ///   scope（CIBA Core 7.1）。**空の文字列は無いのと同じ**に扱う
        /// - **ヒントは、login_hint_token・id_token_hint・login_hint のうち、ちょうど 1 つ**（CIBA Core 7.1 : one (and only one)）
        /// - exp・nbf は値を確かめる（RFC 7519 4.1.4・4.1.5。読み方は CmnJwtToken.TryReadNumericDate。#647）。
        ///   iat は NumericDate として読めることだけを見る（どれだけ古いかは見ない）
        /// - **client_notification_token・binding_message は必須にしない**（CIBA Core 7.1 : 前者は Ping・Push のときだけ必須、後者は任意。
        ///   どの方式で登録されたクライアントかは、この関数には分からない）
        /// - **aud の値は照合しない**（期待する値を受けない版のため）。**照合するなら aud を受ける版を使う**（#653 ph2）
        /// 欠けている・読めない・期限の過ぎたものは false（例外にしない）。iss は、署名が正しければ読めた値を返す。
        /// </remarks>
        [Obsolete("This overload does not verify the aud claim, please use the overload that takes the expected aud instead.")]
        public static bool VerifyCiba(string ro, out string iss, ECParameters ecPublicKey)
        {
            return RequestObject.VerifyCibaCore(ro, null, out iss, new JWS_ES256_Param(ecPublicKey, false));
        }

        /// <summary>VerifyCiba（期待する aud と照合する）</summary>
        /// <param name="ro">string</param>
        /// <param name="aud">期待する aud（OP の Issuer Identifier。CIBA Core 7.1.1）</param>
        /// <param name="iss">string</param>
        /// <param name="jwkPublicKey">ES256用のJWK公開鍵</param>
        /// <returns>検証結果</returns>
        /// <remarks>aud を受けない版と同じ確認に加えて、aud を照合する（ECParameters の版を参照）。</remarks>
        public static bool VerifyCiba(string ro, string aud, out string iss, string jwkPublicKey)
        {
            EccPublicKeyConverter epkc = new EccPublicKeyConverter();
            return RequestObject.VerifyCiba(ro, aud, out iss,
                epkc.JwkToParam(jwkPublicKey));
        }

        /// <summary>VerifyCiba（期待する aud と照合する）</summary>
        /// <param name="ro">string</param>
        /// <param name="aud">期待する aud（OP の Issuer Identifier。CIBA Core 7.1.1）</param>
        /// <param name="iss">string</param>
        /// <param name="ecPublicKey">ES256用のECParameters公開鍵</param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// aud を受けない版と同じ確認に加えて、**aud が期待する値と一致する**（配列なら、それを含む）ことを確かめる（#653 ph2）。
        /// 比較は大文字・小文字を区別する完全一致（RFC 7519 4.1.3 : StringOrURI の比較）。
        /// **正規化はしない**（末尾のスラッシュの有無も別物）ので、メタデータの issuer の文字列をそのまま渡す。
        /// CIBA Core 7.1.1 は、aud に OP の Issuer Identifier を入れるとする。
        /// </remarks>
        /// <exception cref="ArgumentNullException">aud が null か空</exception>
        public static bool VerifyCiba(string ro, string aud, out string iss, ECParameters ecPublicKey)
        {
            if (string.IsNullOrEmpty(aud))
            {
                throw new ArgumentNullException(nameof(aud));
            }

            return RequestObject.VerifyCibaCore(ro, aud, out iss, new JWS_ES256_Param(ecPublicKey, false));
        }

        /// <summary>VerifyCiba（期待する aud と照合する。署名の alg は、渡した検証鍵で選ぶ）</summary>
        /// <param name="ro">string</param>
        /// <param name="aud">期待する aud（OP の Issuer Identifier。CIBA Core 7.1.1）</param>
        /// <param name="iss">string</param>
        /// <param name="jws">
        /// 署名の検証に使う、クライアントの鍵（JWS_PS256_Param・JWS_ES256_Param など）。
        /// JWK からなら JwsFactory.CreateFromJwk(alg, jwk) で作る
        /// </param>
        /// <returns>検証結果</returns>
        /// <remarks>
        /// ECParameters の版と同じ確認をする。違いは、**署名の alg を呼び出し側が選べる**ことだけである（#664）。
        /// FAPI CIBA 7.6 は、FAPI 1.0 Advanced 8.6 に従うとする（PS256 か ES256。RS256 は使わないほうがよい）。
        /// - **ヘッダの alg が、渡した鍵の alg と違えば false**（大文字・小文字を区別する。RFC 8725 3.1）
        /// - **受ける alg は、呼び出し側が決める**（クライアントの登録など）。ヘッダの alg を読んで鍵を作ると、照合の意味が無くなる
        /// - 渡せる鍵の型は制限しない
        /// - **鍵の型は、クライアントの登録どおりのものだけを渡す。** 公開鍵の値を HS* の鍵に使うと、alg の取り違えになる（RFC 8725 3.1 : 鍵は 1 つの alg だけで使う）
        /// </remarks>
        /// <exception cref="ArgumentNullException">aud が null か空、jws が null</exception>
        public static bool VerifyCiba(string ro, string aud, out string iss, JWS jws)
        {
            if (string.IsNullOrEmpty(aud))
            {
                throw new ArgumentNullException(nameof(aud));
            }

            if (jws == null)
            {
                throw new ArgumentNullException(nameof(jws));
            }

            return RequestObject.VerifyCibaCore(ro, aud, out iss, jws);
        }

        /// <summary>VerifyCiba の本体</summary>
        /// <param name="ro">string</param>
        /// <param name="expectedAud">期待する aud。null なら照合しない</param>
        /// <param name="iss">string</param>
        /// <param name="jws">検証鍵（ヘッダの alg と、この鍵の alg が一致すること。#664）</param>
        /// <returns>検証結果</returns>
        private static bool VerifyCibaCore(string ro, string expectedAud, out string iss, JWS jws)
        {
            iss = "";

            if (!CmnJwtToken.TryVerifySignature(jws, ro))
            {
                // JWTの署名検証に失敗
                return false;
            }

            JObject jobj = RequestObject.ReadPayload(ro);

            if (jobj == null)
            {
                // ペイロードが JSON のオブジェクトでない
                return false;
            }

            iss = RequestObject.ReadString(jobj, OAuth2AndOIDCConst.iss) ?? "";

            if (string.IsNullOrEmpty(iss) ||
                !RequestObject.HasAud(jobj[OAuth2AndOIDCConst.aud], expectedAud) ||
                RequestObject.ReadString(jobj, OAuth2AndOIDCConst.jti) == null ||
                RequestObject.ReadString(jobj, OAuth2AndOIDCConst.scope) == null)
            {
                // 必須項目の不足
                return false;
            }

            int hints = 0;
            foreach (string hint in new string[] {
                OAuth2AndOIDCConst.login_hint_token, OAuth2AndOIDCConst.id_token_hint, OAuth2AndOIDCConst.login_hint })
            {
                if (RequestObject.ReadString(jobj, hint) != null)
                {
                    hints++;
                }
            }

            if (hints != 1)
            {
                // ヒントが無い、または複数
                return false;
            }

            // 時刻のクレーム（exp・nbf・iat は必須）
            return RequestObject.VerifyTimeClaims(jobj, true);
        }
        #endregion

        #endregion

        #region 共通部分（#653）

        /// <summary>ペイロードを読む（署名を確かめた後に呼ぶ。CmnJwtToken.ReadPayloadObject）</summary>
        /// <param name="ro">要求オブジェクト（JWS）</param>
        /// <returns>JSON のオブジェクト。オブジェクトでなければ null</returns>
        private static JObject ReadPayload(string ro)
        {
            return CmnJwtToken.ReadPayloadObject(ro);
        }

        /// <summary>文字列のクレームを読む（CmnJwtToken.ReadStringClaim）</summary>
        /// <param name="jobj">ペイロード</param>
        /// <param name="name">クレーム名</param>
        /// <returns>空でない文字列なら、その値。無い・空・文字列でないなら null</returns>
        private static string ReadString(JObject jobj, string name)
        {
            return CmnJwtToken.ReadStringClaim(jobj, name);
        }

        /// <summary>aud が在るか。期待する値があれば照合する（CmnJwtToken.ContainsAud）</summary>
        /// <param name="aud">aud のクレーム</param>
        /// <param name="expectedAud">期待する aud。null なら照合しない（在ることだけを見る）</param>
        /// <returns>bool</returns>
        private static bool HasAud(JToken aud, string expectedAud)
        {
            return CmnJwtToken.ContainsAud(aud, (expectedAud == null) ? null : new string[] { expectedAud });
        }

        /// <summary>exp・nbf（と iat）を確かめる</summary>
        /// <param name="jobj">ペイロード</param>
        /// <param name="required">
        /// true なら、exp・nbf・iat を必須とし、無いものも失敗にする（VerifyCiba）。iat は読めることだけを見る（どれだけ古いかは見ない）。
        /// false なら、exp・nbf のうち在るものだけを確かめ、iat は見ない（Verify）
        /// </param>
        /// <returns>bool</returns>
        /// <remarks>**時計のずれは許さない**（許容幅 0。IdToken などの Verify と同じ。#647）。</remarks>
        private static bool VerifyTimeClaims(JObject jobj, bool required)
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();

            JToken exp = jobj[OAuth2AndOIDCConst.exp];
            if ((required || exp != null) && !CmnJwtToken.VerifyExpClaim(exp, now))
            {
                // 期限切れ・読めない（・無い）
                return false;
            }

            JToken nbf = jobj[OAuth2AndOIDCConst.nbf];
            if ((required || nbf != null) && !CmnJwtToken.VerifyNbfClaim(nbf, now))
            {
                // まだ使えない・読めない（・無い）
                return false;
            }

            decimal iat;
            if (required && !CmnJwtToken.TryReadNumericDate(jobj[OAuth2AndOIDCConst.iat], true, out iat))
            {
                // iat が無い・読めない
                return false;
            }

            return true;
        }

        #endregion
    }
}

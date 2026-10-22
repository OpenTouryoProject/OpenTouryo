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
//* クラス名        ：DPoPProof
//* クラス日本語名  ：DPoP の証明（RFC 9449）を作る・検証する
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した。#634 の A-2 / A-3）
//**********************************************************************************

using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>DPoP の証明（RFC 9449）を作る・検証する</summary>
    /// <remarks>
    /// DPoP は「鍵を持っていること」を要求ごとに示させる仕組みである。
    /// 要求の DPoP ヘッダに、クライアントが自分の鍵で署名した短い JWT（証明）を添える。
    /// 公開鍵は、その JWT のヘッダ（jwk）に入っている。
    ///
    /// | 側 | 使う口 |
    /// |---|---|
    /// | クライアント | Create（OAuth2AndOIDCClient の DPoP の口も、これで証明を作る） |
    /// | 認可サーバ（/token） | Verify。返る jkt を、発行するトークンの cnf.jkt に書く |
    /// | リソース サーバ | VerifyForResource（Verify ＋ ath と cnf.jkt の照合） |
    ///
    /// **配備ごとに決めることは、ここに持ち込まない**（呼び出し側が持つ）。
    /// - jti の使い回しを拒む台帳（CreateProofId で鍵を作れる。RFC 9449 11.1）
    /// - nonce の作り方と有効期間（payload の nonce を、呼び出し側が照合する。RFC 9449 8 / 9）
    /// - htu の期待値の決め方（要求の Host ヘッダを信じず、設定から作るのが安全）
    /// - どの口で何を要求するか（cnf.jkt / dpop_jkt を見る経路）
    /// </remarks>
    public class DPoPProof
    {
        #region 定数

        /// <summary>iat が古すぎるとみなす既定の線</summary>
        /// <remarks>
        /// 証明は要求ごとに作るので、長く遡る必要は無い。狭すぎると時計のズレで落ちる。
        /// 使い回しはこの窓では止めない（jti の台帳と nonce で止める）。
        /// </remarks>
        public static readonly TimeSpan DefaultPastWindow = TimeSpan.FromMinutes(5);

        /// <summary>iat が未来すぎるとみなす既定の線（時計のズレの分だけ許す）</summary>
        public static readonly TimeSpan DefaultFutureWindow = TimeSpan.FromMinutes(1);

        #endregion

        #region クライアント側（A-2）

        /// <summary>証明を作る（/token などへの要求用）</summary>
        /// <param name="signer">JWS（クライアントの秘密鍵。JwsFactory.CreateFromPrivateJwk など）</param>
        /// <param name="publicJwk">JObject（その鍵の公開鍵。JwkOperator.ToPublic で作れる）</param>
        /// <param name="htm">string（要求の HTTP メソッド。例 : POST）</param>
        /// <param name="htu">string（要求の URL。クエリと素片は落として入れる）</param>
        /// <returns>証明（DPoP ヘッダの値）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string Create(JWS signer, JObject publicJwk, string htm, string htu)
        {
            return DPoPProof.Create(signer, publicJwk, htm, htu, null, null);
        }

        /// <summary>証明を作る</summary>
        /// <param name="signer">JWS（クライアントの秘密鍵）</param>
        /// <param name="publicJwk">JObject（その鍵の公開鍵）</param>
        /// <param name="htm">string（要求の HTTP メソッド）</param>
        /// <param name="htu">string（要求の URL。クエリと素片は落として入れる）</param>
        /// <param name="accessToken">string（リソースへの要求なら access_token。ath を入れる。null なら入れない）</param>
        /// <param name="nonce">string（サーバが DPoP-Nonce で渡した値。null なら入れない）</param>
        /// <returns>証明（DPoP ヘッダの値）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// ヘッダ : typ=dpop+jwt、alg（signer が決める）、jwk（公開鍵）。
        /// ペイロード : jti、htm、htu、iat（数値）、ath（任意）、nonce（任意）。
        /// signer の JWSHeader の typ / jwk は、作る間だけ書き換え、元に戻す。
        /// 証明は要求ごとに作ること（jti を使い回すと、サーバに断られる）。
        /// </remarks>
        public static string Create(JWS signer, JObject publicJwk,
            string htm, string htu, string accessToken, string nonce)
        {
            if (signer == null)
            {
                throw new ArgumentNullException(nameof(signer));
            }

            if (publicJwk == null)
            {
                throw new ArgumentNullException(nameof(publicJwk));
            }

            if (string.IsNullOrEmpty(htm))
            {
                throw new ArgumentNullException(nameof(htm));
            }

            if (!Uri.TryCreate(htu, UriKind.Absolute, out Uri uri))
            {
                throw new ArgumentException(PublicExceptionMessage.ARGUMENT_INCORRECT, nameof(htu));
            }

            // RFC 9449 4.3 : jwk に秘密鍵が入っていてはならない。
            if (!JwkOperator.IsPublicOnly(publicJwk))
            {
                throw new ArgumentException(PublicExceptionMessage.ARGUMENT_INCORRECT, nameof(publicJwk));
            }

            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { OAuth2AndOIDCConst.jti, CmnJwtToken.CreateJitClaim() },
                { OAuth2AndOIDCConst.htm, htm },
                // RFC 9449 4.2 : クエリと素片を含まない。
                { OAuth2AndOIDCConst.htu, uri.GetLeftPart(UriPartial.Path) },
                // NumericDate（数値）で入れる（CmnJwtToken.CreateIatClaim は文字列を返すので使わない）。
                { OAuth2AndOIDCConst.iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
            };

            if (!string.IsNullOrEmpty(accessToken))
            {
                payload.Add(OAuth2AndOIDCConst.ath, DPoPProof.CreateAth(accessToken));
            }

            if (!string.IsNullOrEmpty(nonce))
            {
                payload.Add(OAuth2AndOIDCConst.nonce, nonce);
            }

            string typ = signer.JWSHeader.typ;
            JToken jwk = signer.JWSHeader.jwk;

            try
            {
                signer.JWSHeader.typ = JwtConst.DPOP_JWT;
                signer.JWSHeader.jwk = publicJwk;

                return signer.Create(JsonConvert.SerializeObject(payload));
            }
            finally
            {
                signer.JWSHeader.typ = typ;
                signer.JWSHeader.jwk = jwk;
            }
        }

        /// <summary>access_token の指紋（ath の値。RFC 9449 4.2）</summary>
        /// <param name="accessToken">string</param>
        /// <returns>BASE64URL(SHA-256(ASCII(access_token)))。空なら空文字</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string CreateAth(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return "";
            }

            return CustomEncode.ToBase64UrlString(GetHash.GetHashBytes(
                CustomEncode.StringToByte(accessToken, CustomEncode.us_ascii), EnumHashAlgorithm.SHA256));
        }

        #endregion

        #region サーバ側（A-3）

        /// <summary>証明を検証し、鍵の指紋（jkt）を返す（既定の窓と alg）</summary>
        /// <param name="proof">string（DPoP ヘッダの値）</param>
        /// <param name="htm">string（この要求の HTTP メソッド）</param>
        /// <param name="htu">string（この口の URL）</param>
        /// <param name="jkt">out string（公開鍵の指紋。RFC 7638 の SHA-256）</param>
        /// <param name="payload">out JObject（証明のペイロード。nonce / jti の照合に使う）</param>
        /// <param name="errDescription">out string（断る理由。そのまま応答に載せられる英文）</param>
        /// <returns>使ってよければ true</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static bool Verify(string proof, string htm, string htu,
            out string jkt, out JObject payload, out string errDescription)
        {
            return DPoPProof.Verify(proof, htm, htu,
                DPoPProof.DefaultPastWindow, DPoPProof.DefaultFutureWindow, null,
                out jkt, out payload, out errDescription);
        }

        /// <summary>証明を検証し、鍵の指紋（jkt）を返す</summary>
        /// <param name="proof">string（DPoP ヘッダの値）</param>
        /// <param name="htm">string（この要求の HTTP メソッド）</param>
        /// <param name="htu">string（この口の URL。要求の Host ヘッダではなく、設定から作ること）</param>
        /// <param name="pastWindow">TimeSpan（iat をどこまで遡って受けるか）</param>
        /// <param name="futureWindow">TimeSpan（iat をどこまで先まで受けるか）</param>
        /// <param name="algs">string[]（受ける alg。null なら JwsFactory.SupportedAlgs）</param>
        /// <param name="jkt">out string（公開鍵の指紋。RFC 7638 の SHA-256）</param>
        /// <param name="payload">out JObject（証明のペイロード。nonce / jti の照合に使う）</param>
        /// <param name="errDescription">out string（断る理由。そのまま応答に載せられる英文）</param>
        /// <returns>使ってよければ true</returns>
        /// <remarks>
        /// 自由に利用できる。見るもの（RFC 9449 4.3）は次のとおり。
        ///
        /// | | |
        /// |---|---|
        /// | 形 | JWS（3 区切り）であること |
        /// | typ | dpop+jwt |
        /// | alg | algs に入っていること（none・共通鍵は入らない） |
        /// | jwk | 在ること。公開鍵だけであること。alg と鍵の種類が合っていること |
        /// | 署名 | その jwk で検証できること |
        /// | jti | 在ること（使い回しの検出は、呼び出し側の台帳で行う） |
        /// | htm / htu | 期待どおりであること（htu はクエリと素片を除いて比べる） |
        /// | iat | 数値で、窓の中であること |
        ///
        /// **nonce と jti の使い回しは見ない**（配備の決めである）。通った後で、
        /// payload の nonce を照合し、CreateProofId(jkt, jti) で台帳に記録すること。
        /// **台帳への記録は、検証に通った後で行う**（壊した証明を投げるだけで、相手の jti を焼かせないため）。
        ///
        /// 外から来る値なので、読めなくても例外にせず false を返す。
        /// error の値は口によって違う（/token は invalid_dpop_proof、リソースは invalid_token）ので、
        /// 呼び出し側が決める。
        /// </remarks>
        public static bool Verify(string proof, string htm, string htu,
            TimeSpan pastWindow, TimeSpan futureWindow, string[] algs,
            out string jkt, out JObject payload, out string errDescription)
        {
            jkt = "";
            payload = null;
            errDescription = "";

            try
            {
                JObject header = JwtReader.TryReadHeader(proof);
                JObject body = JwtReader.TryReadPayload(proof);

                if (header == null || body == null)
                {
                    errDescription = "The DPoP proof is not a JWS.";
                    return false;
                }

                // typ（署名の検証でも確かめるが、先に読んで分かる誤りは分けて返す）
                if (JwtReader.TryReadTyp(proof) != JwtConst.DPOP_JWT)
                {
                    errDescription = "The DPoP proof must have typ=" + JwtConst.DPOP_JWT + ".";
                    return false;
                }

                // alg（非対称の、受けると決めたものだけ）
                string alg = JwtReader.TryReadAlg(proof);

                if (!JwsFactory.IsSupported(alg)
                    || (algs != null && Array.IndexOf(algs, alg) < 0))
                {
                    errDescription = "The DPoP proof is signed with an unsupported alg.";
                    return false;
                }

                // jwk（公開鍵だけ。alg と鍵の種類が合っていること）
                JObject jwk = header[JwtConst.jwk] as JObject;

                if (jwk == null)
                {
                    errDescription = "The DPoP proof must have a jwk header.";
                    return false;
                }

                if (!JwkOperator.IsPublicOnly(jwk))
                {
                    errDescription = "The jwk of the DPoP proof must not contain a private key.";
                    return false;
                }

                if (!JwsFactory.IsSameKeyType(jwk, alg))
                {
                    errDescription = "The jwk of the DPoP proof does not match the alg.";
                    return false;
                }

                // 署名（鍵は要求の中に在る）
                JWS jws = JwsFactory.CreateFromJwk(alg, jwk);

                if (jws == null || !jws.Verify(proof, JwtConst.DPOP_JWT))
                {
                    errDescription = "The signature of the DPoP proof is invalid.";
                    return false;
                }

                // jti（在ること）
                if (string.IsNullOrEmpty(DPoPProof.GetString(body, OAuth2AndOIDCConst.jti)))
                {
                    errDescription = "The DPoP proof must have a jti.";
                    return false;
                }

                // htm / htu
                if (!string.Equals(DPoPProof.GetString(body, OAuth2AndOIDCConst.htm), htm, StringComparison.Ordinal))
                {
                    errDescription = "The htm of the DPoP proof does not match the request.";
                    return false;
                }

                if (!UriOperator.IsSameWithoutQuery(DPoPProof.GetString(body, OAuth2AndOIDCConst.htu), htu))
                {
                    errDescription = "The htu of the DPoP proof does not match the request.";
                    return false;
                }

                // iat（窓の中）
                if (!DPoPProof.IsFreshIat(body[OAuth2AndOIDCConst.iat], pastWindow, futureWindow))
                {
                    errDescription = "The iat of the DPoP proof is not acceptable.";
                    return false;
                }

                jkt = JwkOperator.Thumbprint(jwk);

                if (string.IsNullOrEmpty(jkt))
                {
                    errDescription = "The jwk of the DPoP proof could not be read.";
                    return false;
                }

                payload = body;
                return true;
            }
            catch (Exception)
            {
                // 外から来る値なので、例外にしない。
                jkt = "";
                payload = null;
                errDescription = "The DPoP proof could not be verified.";
                return false;
            }
        }

        /// <summary>保護されたリソースで、証明とトークンの結び付きを照合する（既定の窓と alg）</summary>
        /// <param name="proof">string（DPoP ヘッダの値）</param>
        /// <param name="htm">string（この要求の HTTP メソッド）</param>
        /// <param name="htu">string（この口の URL）</param>
        /// <param name="accessToken">string（提示された access_token）</param>
        /// <param name="boundJkt">string（トークンの cnf.jkt）</param>
        /// <param name="payload">out JObject（証明のペイロード）</param>
        /// <param name="errDescription">out string（断る理由）</param>
        /// <returns>使ってよければ true</returns>
        /// <remarks>
        /// 自由に利用できる。Verify に、次の 2 つを足す（RFC 9449 7.1）。
        /// - ath が、提示された access_token の指紋と一致すること（別のトークンに付けた証明を使い回させない）
        /// - 証明の鍵が、トークンの cnf.jkt と同じであること（持ち主であることの確認）
        /// Authorization のスキームが DPoP であることは、呼び出し側が確かめること
        /// （AuthenticationHeader.GetAccessToken）。
        /// </remarks>
        public static bool VerifyForResource(string proof, string htm, string htu,
            string accessToken, string boundJkt, out JObject payload, out string errDescription)
        {
            payload = null;

            if (string.IsNullOrEmpty(proof))
            {
                errDescription = "A DPoP proof is required for this access token.";
                return false;
            }

            if (!DPoPProof.Verify(proof, htm, htu, out string jkt, out JObject body, out errDescription))
            {
                return false;
            }

            if (string.IsNullOrEmpty(boundJkt) || !string.Equals(jkt, boundJkt, StringComparison.Ordinal))
            {
                errDescription = "The DPoP proof was signed with another key.";
                return false;
            }

            string ath = DPoPProof.GetString(body, OAuth2AndOIDCConst.ath);

            if (string.IsNullOrEmpty(ath)
                || !string.Equals(ath, DPoPProof.CreateAth(accessToken), StringComparison.Ordinal))
            {
                errDescription = "The ath of the DPoP proof does not match the access token.";
                return false;
            }

            payload = body;
            return true;
        }

        /// <summary>使い回しを拒む台帳に記録する鍵</summary>
        /// <param name="jkt">string（鍵の指紋）</param>
        /// <param name="jti">string（証明の jti）</param>
        /// <returns>BASE64URL(SHA-256(UTF-8(jkt + "." + jti)))。どちらかが空なら空文字</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// - jti だけを鍵にしない。jti はクライアントが決めるので、他の相手の値を先に使って塞げてしまう。
        ///   jkt と組にすれば、台帳は「鍵ごと」になる（RFC 9449 11.1）
        /// - 区切りに "." を入れる（境目の違う 2 つの組が、同じ値にならないため）
        /// - UTF-8 で読む（jti は ASCII の外の文字を含みうる。ASCII で読むと違う jti が同じ値に潰れる）
        /// - 指紋にするので、jti の長さに依らず一定の長さになり、jkt もそのまま残らない
        /// 台帳は、DefaultPastWindow ＋ DefaultFutureWindow（使った窓の和）の間だけ持てばよい。
        /// </remarks>
        public static string CreateProofId(string jkt, string jti)
        {
            if (string.IsNullOrEmpty(jkt) || string.IsNullOrEmpty(jti))
            {
                return "";
            }

            return CustomEncode.ToBase64UrlString(GetHash.GetHashBytes(
                CustomEncode.StringToByte(jkt + "." + jti, CustomEncode.UTF_8), EnumHashAlgorithm.SHA256));
        }

        #endregion

        #region private

        /// <summary>iat が窓の中か</summary>
        /// <param name="iat">JToken（NumericDate）</param>
        /// <param name="pastWindow">TimeSpan</param>
        /// <param name="futureWindow">TimeSpan</param>
        /// <returns>窓の中なら true</returns>
        /// <remarks>数値でなければ受けない（RFC 7519 2 の NumericDate）。</remarks>
        private static bool IsFreshIat(JToken iat, TimeSpan pastWindow, TimeSpan futureWindow)
        {
            if (iat == null || iat.Type != JTokenType.Integer)
            {
                return false;
            }

            DateTimeOffset issued = DateTimeOffset.FromUnixTimeSeconds((long)iat);
            DateTimeOffset now = DateTimeOffset.UtcNow;

            return (now - pastWindow) <= issued && issued <= (now + futureWindow);
        }

        /// <summary>文字列の項目を読む（文字列でなければ null）</summary>
        /// <param name="obj">JObject</param>
        /// <param name="name">string</param>
        /// <returns>値</returns>
        private static string GetString(JObject obj, string name)
        {
            JToken token = obj[name];
            return (token != null && token.Type == JTokenType.String) ? (string)token : null;
        }

        #endregion
    }
}

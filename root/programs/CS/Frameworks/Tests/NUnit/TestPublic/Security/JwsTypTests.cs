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
//* クラス名        ：JwsTypTests
//* クラス日本語名  ：JWS の typ（署名時の指定と、検証時の照合）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#611）
//*  2026/10/16  玄人 幸道         JWSHeader が基底（JWS）に移ったので、リフレクションをやめた（#615）
//*  2026/10/29  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Security
{
    /// <summary>JWS の typ（署名時の指定と、検証時の照合）の単体テスト</summary>
    /// <remarks>
    /// RFC 9068 の access_token は typ: at+jwt を求める（#611）。
    ///   ・署名側 … JWSHeader.typ に設定する（既定は JWT。null なら出さない）
    ///   ・検証側 … Verify(jwt, typ) で照合する。**Verify(jwt) は typ を照合しない**
    ///             （以前も readonly のため照合が効いておらず、その振る舞いを保つ）
    /// 照合の規則は RFC 7515 4.1.9 に従う。大文字・小文字を区別せず、
    /// "application/" の有無を同一視する。期待値を指定したとき、typ が無ければ不一致。
    /// </remarks>
    [TestFixture]
    public class JwsTypTests
    {
        private const string Payload = "{\"sub\":\"nunit\"}";

        /// <summary>方式ごとの JWS（署名と検証を同じインスタンスで行える鍵を持つもの）</summary>
        public static IEnumerable<TestCaseData> Algorithms()
        {
            yield return new TestCaseData(new Func<JWS>(() => new JWS_HS256(Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef")))).SetName("HS256");
            yield return new TestCaseData(new Func<JWS>(() => new JWS_RS256_Param())).SetName("RS256");
            yield return new TestCaseData(new Func<JWS>(() => new JWS_ES256_Param())).SetName("ES256");
        }

        /// <summary>ヘッダの JSON</summary>
        private static JObject Header(string jwt)
        {
            return JObject.Parse(CustomEncode.ByteToString(
                CustomEncode.FromBase64UrlString(jwt.Split('.')[0]), CustomEncode.UTF_8));
        }

        #region 署名側

        /// <summary>既定の typ は JWT（従来どおり）。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Create_DefaultTypIsJwt(Func<JWS> factory)
        {
            string jwt = factory().Create(Payload);

            Assert.That((string)Header(jwt)["typ"], Is.EqualTo("JWT"));
        }

        /// <summary>typ を設定すると、ヘッダに出る。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Create_AtJwt(Func<JWS> factory)
        {
            JWS jws = factory();
            jws.JWSHeader.typ = JwtConst.AT_JWT;

            Assert.That((string)Header(jws.Create(Payload))["typ"], Is.EqualTo("at+jwt"));
        }

        /// <summary>typ を null にすると、ヘッダに出さない。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Create_NullTyp_Omitted(Func<JWS> factory)
        {
            JWS jws = factory();
            jws.JWSHeader.typ = null;

            Assert.That(Header(jws.Create(Payload))["typ"], Is.Null);
        }

        #endregion

        #region 検証側

        /// <summary>
        /// **Verify(jwt) は typ を照合しない。** どの typ でも、typ が無くても通る（以前の実際の振る舞い）。
        /// </summary>
        [TestCaseSource(nameof(Algorithms))]
        [Basis("設計：JWS.Verify(jwtString) の remarks（typ は照合しない。以前の振る舞いを保つ。#611）")]
        public void Verify_WithoutTyp_DoesNotCheckTyp(Func<JWS> factory)
        {
            JWS jws = factory();
            Assert.That(jws.Verify(jws.Create(Payload)), Is.True, "JWT");

            jws.JWSHeader.typ = JwtConst.AT_JWT;
            Assert.That(jws.Verify(jws.Create(Payload)), Is.True, "at+jwt");

            jws.JWSHeader.typ = "dpop+jwt";
            Assert.That(jws.Verify(jws.Create(Payload)), Is.True, "dpop+jwt");

            jws.JWSHeader.typ = null;
            Assert.That(jws.Verify(jws.Create(Payload)), Is.True, "typ なし");
        }

        /// <summary>期待値と一致すれば通る（大文字・小文字、application/ の有無を問わない）。</summary>
        [TestCaseSource(nameof(Algorithms))]
        [Basis("RFC 7515 4.1.9（メディア タイプは大文字小文字を区別しない。application/ が無ければ付けたものとして扱う）")]
        public void Verify_AtJwt_Match(Func<JWS> factory)
        {
            JWS jws = factory();
            jws.JWSHeader.typ = JwtConst.AT_JWT;
            string jwt = jws.Create(Payload);

            Assert.That(jws.Verify(jwt, JwtConst.AT_JWT), Is.True, "at+jwt");
            Assert.That(jws.Verify(jwt, "AT+JWT"), Is.True, "大文字");
            Assert.That(jws.Verify(jwt, "application/at+jwt"), Is.True, "application/ 付き");
        }

        /// <summary>トークン側が application/ 付きでも同じ。</summary>
        [Test]
        [Basis("RFC 7515 4.1.9（application/ が無ければ付けたものとして扱う）")]
        public void Verify_TokenWithApplicationPrefix_Match()
        {
            JWS_HS256 jws = new JWS_HS256(Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef"));
            jws.JWSHeader.typ = "application/at+jwt";

            Assert.That(jws.Verify(jws.Create(Payload), JwtConst.AT_JWT), Is.True);
        }

        /// <summary>
        /// 期待値と違えば通らない（RFC 9068 の「at+jwt 以外を拒否する」）。
        /// </summary>
        [TestCaseSource(nameof(Algorithms))]
        [Basis("RFC 9068 4（typ が at+jwt・application/at+jwt でなければ拒否する）")]
        public void Verify_Mismatch_Rejected(Func<JWS> factory)
        {
            JWS jws = factory();
            string jwtTyp = jws.Create(Payload);              // typ: JWT

            jws.JWSHeader.typ = JwtConst.AT_JWT;
            string atTyp = jws.Create(Payload);               // typ: at+jwt

            Assert.That(jws.Verify(jwtTyp, JwtConst.AT_JWT), Is.False, "JWT を at+jwt として");
            Assert.That(jws.Verify(atTyp, JwtConst.JWT), Is.False, "at+jwt を JWT として");
            Assert.That(jws.Verify(jwtTyp, JwtConst.JWT), Is.True, "JWT を JWT として");
        }

        /// <summary>**期待値を指定したとき、typ が無ければ不一致。** JWT を期待しても通らない。</summary>
        [TestCaseSource(nameof(Algorithms))]
        [Basis("RFC 9068 4（typ が at+jwt・application/at+jwt でなければ拒否する）")]
        [Basis("設計：JWS.Verify(jwtString, typ) の remarks（期待値を指定したとき、typ が無ければ不一致）")]
        public void Verify_NoTyp_WithExpected_Rejected(Func<JWS> factory)
        {
            JWS jws = factory();
            jws.JWSHeader.typ = null;
            string jwt = jws.Create(Payload);

            Assert.That(jws.Verify(jwt, JwtConst.JWT), Is.False);
            Assert.That(jws.Verify(jwt, JwtConst.AT_JWT), Is.False);
        }

        /// <summary>期待値が null・空なら、照合しない（Verify(jwt) と同じ）。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Verify_NullOrEmptyExpected_SameAsVerify(Func<JWS> factory)
        {
            JWS jws = factory();
            jws.JWSHeader.typ = "dpop+jwt";
            string jwt = jws.Create(Payload);

            Assert.That(jws.Verify(jwt, null), Is.True);
            Assert.That(jws.Verify(jwt, ""), Is.True);
        }

        /// <summary>typ が一致しても、署名が合わなければ通らない。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Verify_TypMatch_ButSignatureTampered(Func<JWS> factory)
        {
            JWS jws = factory();
            jws.JWSHeader.typ = JwtConst.AT_JWT;
            string[] parts = jws.Create(Payload).Split('.');

            string tampered = parts[0] + "." + CustomEncode.ToBase64UrlString(
                CustomEncode.StringToByte("{\"sub\":\"evil\"}", CustomEncode.UTF_8)) + "." + parts[2];

            Assert.That(jws.Verify(tampered, JwtConst.AT_JWT), Is.False);
        }

        #endregion
    }
}

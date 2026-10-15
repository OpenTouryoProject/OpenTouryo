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
//* クラス名        ：JweTypTests
//* クラス日本語名  ：JWE の typ（暗号化時の指定と、復号時の照合）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#615）
//**********************************************************************************

using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Security
{
    /// <summary>JWE の typ（暗号化時の指定と、復号時の照合）の単体テスト</summary>
    /// <remarks>
    /// #611（JWS）と同じ方針（#615）。
    ///   ・暗号化側 … JWEHeader.typ に設定する（既定は JWT。null なら出さない）
    ///   ・復号側   … Decrypt(jwt, typ, out payload) で照合する。
    ///                **Decrypt(jwt, out payload) は typ を照合しない**
    ///                （以前も readonly のため照合が効いておらず、その振る舞いを保つ）
    /// 照合の規則は JwsTypTests と同じ。
    /// </remarks>
    [TestFixture]
    public class JweTypTests
    {
        private const string Payload = "{\"sub\":\"nunit\"}";

        /// <summary>方式ごとの JWE（暗号化と復号を同じインスタンスで行える鍵を持つもの）</summary>
        public static IEnumerable<TestCaseData> Algorithms()
        {
            yield return new TestCaseData(new Func<JWE>(() => new JWE_RsaOaepAesGcm_Param(EnumASymmetricAlgorithm.RsaCng))).SetName("RSA-OAEP_A256GCM");
            yield return new TestCaseData(new Func<JWE>(() => new JWE_Rsa15A128CbcHS256_Param(EnumASymmetricAlgorithm.RsaCng))).SetName("RSA1_5_A128CBC-HS256");
        }

        /// <summary>ヘッダの JSON</summary>
        private static JObject Header(string jwt)
        {
            return JObject.Parse(CustomEncode.ByteToString(
                CustomEncode.FromBase64UrlString(jwt.Split('.')[0]), CustomEncode.UTF_8));
        }

        /// <summary>暗号化して、そのまま復号する（typ の照合なし）</summary>
        private static bool RoundTrip(JWE jwe, out string payload)
        {
            return jwe.Decrypt(jwe.Create(Payload), out payload);
        }

        #region 暗号化側

        /// <summary>既定の typ は JWT（従来どおり）。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Create_DefaultTypIsJwt(Func<JWE> factory)
        {
            string jwt = factory().Create(Payload);

            Assert.That((string)Header(jwt)["typ"], Is.EqualTo("JWT"));
        }

        /// <summary>typ を設定すると、ヘッダに出る。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Create_CustomTyp(Func<JWE> factory)
        {
            JWE jwe = factory();
            jwe.JWEHeader.typ = "secevent+jwt";

            Assert.That((string)Header(jwe.Create(Payload))["typ"], Is.EqualTo("secevent+jwt"));
        }

        /// <summary>typ を null にすると、ヘッダに出さない。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Create_NullTyp_Omitted(Func<JWE> factory)
        {
            JWE jwe = factory();
            jwe.JWEHeader.typ = null;

            Assert.That(Header(jwe.Create(Payload))["typ"], Is.Null);
        }

        #endregion

        #region 復号側

        /// <summary>
        /// **Decrypt(jwt, out payload) は typ を照合しない。** どの typ でも、typ が無くても通る（以前の実際の振る舞い）。
        /// </summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Decrypt_WithoutTyp_DoesNotCheckTyp(Func<JWE> factory)
        {
            JWE jwe = factory();
            string payload;

            Assert.That(RoundTrip(jwe, out payload), Is.True, "JWT");
            Assert.That(payload, Is.EqualTo(Payload));

            jwe.JWEHeader.typ = "secevent+jwt";
            Assert.That(RoundTrip(jwe, out payload), Is.True, "secevent+jwt");

            jwe.JWEHeader.typ = null;
            Assert.That(RoundTrip(jwe, out payload), Is.True, "typ なし");
        }

        /// <summary>期待値と一致すれば通る（大文字・小文字、application/ の有無を問わない）。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Decrypt_Match(Func<JWE> factory)
        {
            JWE jwe = factory();
            string jwt = jwe.Create(Payload);
            string payload;

            Assert.That(jwe.Decrypt(jwt, JwtConst.JWT, out payload), Is.True, "JWT");
            Assert.That(payload, Is.EqualTo(Payload));
            Assert.That(jwe.Decrypt(jwt, "jwt", out payload), Is.True, "小文字");
            Assert.That(jwe.Decrypt(jwt, "application/JWT", out payload), Is.True, "application/ 付き");
        }

        /// <summary>期待値と違えば通らない。payload は空になる。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Decrypt_Mismatch_Rejected(Func<JWE> factory)
        {
            JWE jwe = factory();
            string jwtTyp = jwe.Create(Payload);              // typ: JWT

            jwe.JWEHeader.typ = "secevent+jwt";
            string otherTyp = jwe.Create(Payload);            // typ: secevent+jwt

            string payload;
            Assert.That(jwe.Decrypt(jwtTyp, "secevent+jwt", out payload), Is.False, "JWT を secevent+jwt として");
            Assert.That(payload, Is.Empty);
            Assert.That(jwe.Decrypt(otherTyp, JwtConst.JWT, out payload), Is.False, "secevent+jwt を JWT として");
            Assert.That(payload, Is.Empty);
        }

        /// <summary>**期待値を指定したとき、typ が無ければ不一致。** JWT を期待しても通らない。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Decrypt_NoTyp_WithExpected_Rejected(Func<JWE> factory)
        {
            JWE jwe = factory();
            jwe.JWEHeader.typ = null;
            string jwt = jwe.Create(Payload);

            string payload;
            Assert.That(jwe.Decrypt(jwt, JwtConst.JWT, out payload), Is.False);
        }

        /// <summary>期待値が null・空なら、照合しない（Decrypt(jwt, out payload) と同じ）。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Decrypt_NullOrEmptyExpected_SameAsDecrypt(Func<JWE> factory)
        {
            JWE jwe = factory();
            jwe.JWEHeader.typ = "secevent+jwt";
            string jwt = jwe.Create(Payload);

            string payload;
            Assert.That(jwe.Decrypt(jwt, null, out payload), Is.True);
            Assert.That(jwe.Decrypt(jwt, "", out payload), Is.True);
        }

        /// <summary>typ が一致しても、暗号文が改ざんされていれば通らない。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Decrypt_TypMatch_ButCiphertextTampered(Func<JWE> factory)
        {
            JWE jwe = factory();
            string[] parts = jwe.Create(Payload).Split('.');

            byte[] cipher = CustomEncode.FromBase64UrlString(parts[3]);
            cipher[0] ^= 0x01;
            parts[3] = CustomEncode.ToBase64UrlString(cipher);

            string payload;
            Assert.That(jwe.Decrypt(string.Join(".", parts), JwtConst.JWT, out payload), Is.False);
        }

        /// <summary>ヘッダが壊れていれば、例外にせず失敗を返す（Decrypt と同じ）。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void Decrypt_BrokenHeader_ReturnsFalse(Func<JWE> factory)
        {
            JWE jwe = factory();
            string[] parts = jwe.Create(Payload).Split('.');
            parts[0] = CustomEncode.ToBase64UrlString(CustomEncode.StringToByte("not json", CustomEncode.UTF_8));

            string payload;
            Assert.That(jwe.Decrypt(string.Join(".", parts), JwtConst.JWT, out payload), Is.False);
            Assert.That(payload, Is.Empty);
        }

        #endregion
    }
}

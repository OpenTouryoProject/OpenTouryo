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
//* クラス名        ：JoseCaseSensitiveTests
//* クラス日本語名  ：JOSE（JWS・JWE・JWK）の alg・enc・kty・use の大文字小文字の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/30  玄人 幸道         新規作成（#643）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Security.Jwt
{
    /// <summary>JOSE（JWS・JWE・JWK）の alg・enc・kty・use の大文字小文字の単体テスト</summary>
    /// <remarks>
    /// **alg・enc・kty・use は、大文字小文字を区別する**（RFC 7515 4.1.1・RFC 7516 4.1.1・4.1.2・RFC 7517 4.1・4.2・4.4）。
    /// 2026/10/30 まで、ToUpper / ToLower で揃えてから比べていた（#643）。
    ///
    /// ヘッダを書き換えると署名（JWE は AAD）が合わなくなり、alg の照合を確かめられない。
    /// そこで、**作る側の JWSHeader.alg / JWEHeader.alg を小文字にしてから作り**、署名は合ったまま
    /// alg の照合だけで断られることを確かめる（元の綴りで作ったものが通ることを、対照として確かめる）。
    /// </remarks>
    [TestFixture]
    public class JoseCaseSensitiveTests
    {
        /// <summary>ペイロード</summary>
        private const string Payload = "{\"sub\":\"nunit\"}";

        /// <summary>JWS のヘッダの alg</summary>
        private static string AlgOf(string jwt)
        {
            return (string)JObject.Parse(Encoding.UTF8.GetString(CustomEncode.FromBase64UrlString(jwt.Split('.')[0])))["alg"];
        }

        /// <summary>JWS の方式（ECDSA を除く。ECDSA は Windows の CNG なので、別のメソッドで回す）</summary>
        public static IEnumerable<TestCaseData> Signers()
        {
            yield return new TestCaseData(new Func<JWS>(() => new JWS_RS256_Param())).SetName("RS256");
            yield return new TestCaseData(new Func<JWS>(() => new JWS_PS256_Param())).SetName("PS256");
            yield return new TestCaseData(new Func<JWS>(() => new JWS_HS256(new byte[32]))).SetName("HS256");
        }

        /// <summary>
        /// JWS：alg を小文字にして作った JWT は、署名が合っていても検証で断る。元の綴りなら通る。
        /// </summary>
        [TestCaseSource(nameof(Signers))]
        [Basis("RFC 7515 4.1.1（alg は大文字小文字を区別する ASCII 文字列）")]
        [Basis("#643")]
        public void Jws_LowerCaseAlg_False(Func<JWS> factory)
        {
            AssertLowerCaseAlgRejected(factory());
        }

        /// <summary>JWS（ES256）：alg を小文字にして作った JWT は、署名が合っていても検証で断る（Windows の CNG）。</summary>
        [Test]
        [Platform("Win")]
        [Basis("RFC 7515 4.1.1（alg は大文字小文字を区別する ASCII 文字列）")]
        [Basis("#643")]
        public void Jws_LowerCaseAlg_Es256_False()
        {
            AssertLowerCaseAlgRejected(new JWS_ES256_Param());
        }

        /// <summary>同じ署名器で、元の綴りなら通り、小文字の alg なら断ることを確かめる</summary>
        private static void AssertLowerCaseAlgRejected(JWS jws)
        {
            string alg = jws.JWSHeader.alg;
            Assert.That(jws.Verify(jws.Create(Payload)), Is.True, "元の綴り");

            jws.JWSHeader.alg = alg.ToLowerInvariant();
            string lower = jws.Create(Payload);
            jws.JWSHeader.alg = alg;

            Assert.That(AlgOf(lower), Is.EqualTo(alg.ToLowerInvariant()), "小文字の alg で作れている");
            Assert.That(jws.Verify(lower), Is.False, "小文字の alg");
        }

        /// <summary>JWE の方式</summary>
        public static IEnumerable<TestCaseData> Encrypters()
        {
            yield return new TestCaseData(new Func<JWE>(() => new JWE_RsaOaepAesGcm_Param(EnumASymmetricAlgorithm.RsaCng))).SetName("RSA-OAEP_A256GCM");
            yield return new TestCaseData(new Func<JWE>(() => new JWE_Rsa15A128CbcHS256_Param(EnumASymmetricAlgorithm.RsaCng))).SetName("RSA1_5_A128CBC-HS256");
        }

        /// <summary>
        /// JWE：alg・enc を小文字にして作った JWT は、AAD が合っていても復号で断る。元の綴りなら戻せる。
        /// </summary>
        [TestCaseSource(nameof(Encrypters))]
        [Basis("RFC 7516 4.1.1（alg は JWS の 4.1.1 と同じ規則。大文字小文字を区別する）・4.1.2（enc は大文字小文字を区別する）")]
        [Basis("#643")]
        public void Jwe_LowerCaseAlgOrEnc_False(Func<JWE> factory)
        {
            JWE jwe = factory();
            string alg = jwe.JWEHeader.alg;
            string enc = jwe.JWEHeader.enc;
            string payload;

            Assert.That(jwe.Decrypt(jwe.Create(Payload), out payload), Is.True, "元の綴り");
            Assert.That(payload, Is.EqualTo(Payload));

            jwe.JWEHeader.alg = alg.ToLowerInvariant();
            string lowerAlg = jwe.Create(Payload);
            jwe.JWEHeader.alg = alg;
            Assert.That(jwe.Decrypt(lowerAlg, out payload), Is.False, "小文字の alg");

            jwe.JWEHeader.enc = enc.ToLowerInvariant();
            string lowerEnc = jwe.Create(Payload);
            jwe.JWEHeader.enc = enc;
            Assert.That(jwe.Decrypt(lowerEnc, out payload), Is.False, "小文字の enc");
        }

        /// <summary>JWK（RSA）：kty が "RSA" でなければ（"rsa" など）、公開鍵・秘密鍵とも読めない（ArgumentOutOfRangeException）。</summary>
        [Test]
        [Basis("RFC 7517 4.1（kty は大文字小文字を区別する）・RFC 7518 6.1（RSA の kty は \"RSA\"）")]
        [Basis("#643")]
        public void Jwk_Rsa_LowerCaseKty_Throws()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                JObject pub = JObject.Parse(new RsaPublicKeyConverter().ParamToJwk(rsa.ExportParameters(false)));
                JObject priv = JObject.Parse(new RsaPrivateKeyConverter().ParamToJwk(rsa.ExportParameters(true)));

                Assert.That(new RsaPublicKeyConverter().JwkToParam(pub).Modulus, Is.EqualTo(rsa.ExportParameters(false).Modulus), "元の綴り");
                Assert.That(new RsaPrivateKeyConverter().JwkToParam(priv).D, Is.EqualTo(rsa.ExportParameters(true).D), "元の綴り");

                pub[JwtConst.kty] = "rsa";
                priv[JwtConst.kty] = "Rsa";
                Assert.Throws<ArgumentOutOfRangeException>(() => new RsaPublicKeyConverter().JwkToParam(pub));
                Assert.Throws<ArgumentOutOfRangeException>(() => new RsaPrivateKeyConverter().JwkToParam(priv));
            }
        }

        /// <summary>JWK（HMAC）：kty・use・alg の綴りが違えば（"OCT"・"SIG"・"hs256"）、JWK を受けるコンストラクタは ArgumentException。</summary>
        [TestCase("OCT", "sig", "HS256")]
        [TestCase("oct", "SIG", "HS256")]
        [TestCase("oct", "sig", "hs256")]
        [Basis("RFC 7517 4.1（kty）・4.2（use）・4.4（alg）は、どれも大文字小文字を区別する")]
        [Basis("RFC 7518 6.1（共通鍵の kty は \"oct\"）・RFC 7517 4.2（署名の use は \"sig\"）")]
        [Basis("#643")]
        public void Jwk_Hmac_WrongCase_Throws(string kty, string use, string alg)
        {
            string k = CustomEncode.ToBase64UrlString(new byte[32]);
            string ok = "{\"kty\":\"oct\",\"use\":\"sig\",\"alg\":\"HS256\",\"k\":\"" + k + "\"}";
            string wrong = "{\"kty\":\"" + kty + "\",\"use\":\"" + use + "\",\"alg\":\"" + alg + "\",\"k\":\"" + k + "\"}";

            Assert.DoesNotThrow(() => new JWS_HS256(ok), "元の綴り");
            Assert.Throws<ArgumentException>(() => new JWS_HS256(wrong));
        }
    }
}

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
//* クラス名        ：JwsFactoryTests
//* クラス日本語名  ：JwsFactory（alg から JWS を作る）と JwtReader の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（#634）
//*  2026/10/22  玄人 幸道         種類の合わない alg・読めない JWT の境界のテストを足した（カバレッジの拡充）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Security.Jwt
{
    /// <summary>JwsFactory（alg から JWS を作る）と JwtReader（検証せずに読む）の単体テスト</summary>
    [TestFixture]
    public class JwsFactoryTests
    {
        private const string Payload = "{\"sub\":\"nunit\"}";

        /// <summary>alg ごとの、秘密鍵の JWK</summary>
        public static IEnumerable<TestCaseData> Algorithms()
        {
            foreach (string alg in new string[] { "RS256", "RS384", "RS512", "PS256", "PS384", "PS512" })
            {
                yield return new TestCaseData(alg).SetName("Rsa_" + alg);
            }

            yield return new TestCaseData("ES256").SetName("Ec_ES256");
            yield return new TestCaseData("ES384").SetName("Ec_ES384");
            yield return new TestCaseData("ES512").SetName("Ec_ES512");
        }

        /// <summary>alg に合う秘密鍵の JWK を作る</summary>
        internal static JObject PrivateJwk(string alg)
        {
            if (alg.StartsWith("ES", StringComparison.Ordinal))
            {
                ECCurve curve = (alg == "ES384") ? ECCurve.NamedCurves.nistP384
                    : (alg == "ES512") ? ECCurve.NamedCurves.nistP521 : ECCurve.NamedCurves.nistP256;

                using (ECDsa ecdsa = ECDsa.Create(curve))
                {
                    return JObject.Parse(new EccPrivateKeyConverter().ParamToJwk(ecdsa.ExportParameters(true)));
                }
            }

            using (RSA rsa = RSA.Create(2048))
            {
                return JObject.Parse(new RsaPrivateKeyConverter().ParamToJwk(rsa.ExportParameters(true)));
            }
        }

        /// <summary>秘密鍵の JWK で署名し、公開鍵の JWK で検証できる（alg ごと）。</summary>
        [TestCaseSource(nameof(Algorithms))]
        public void SignWithPrivateJwk_VerifyWithPublicJwk(string alg)
        {
            JObject priv = PrivateJwk(alg);
            JWS signer = JwsFactory.CreateFromPrivateJwk(alg, priv);
            string jwt = signer.Create(Payload);

            Assert.That(JwtReader.TryReadAlg(jwt), Is.EqualTo(alg));

            JWS verifier = JwsFactory.CreateFromJwk(alg, JwkOperator.ToPublic(priv));
            Assert.That(verifier, Is.Not.Null);
            Assert.That(verifier.Verify(jwt), Is.True);
        }

        /// <summary>表：扱うのは RS* / PS* / ES* の 9 つ。HS* と none は扱わない。配列は複製を返す。</summary>
        [Test]
        public void SupportedAlgs()
        {
            Assert.That(JwsFactory.SupportedAlgs.Length, Is.EqualTo(9));
            Assert.That(JwsFactory.IsSupported("ES256"), Is.True);
            Assert.That(JwsFactory.IsSupported("HS256"), Is.False);
            Assert.That(JwsFactory.IsSupported("none"), Is.False);
            Assert.That(JwsFactory.IsSupported("rs256"), Is.False, "大文字小文字を区別する");
            Assert.That(JwsFactory.IsSupported(null), Is.False);

            JwsFactory.SupportedAlgs[0] = "none";
            Assert.That(JwsFactory.SupportedAlgs[0], Is.EqualTo("RS256"));

            Assert.That(JwsFactory.GetKty("PS384"), Is.EqualTo("RSA"));
            Assert.That(JwsFactory.GetCrv("ES512"), Is.EqualTo("P-521"));
            Assert.That(JwsFactory.GetCrv("RS256"), Is.Null);
        }

        /// <summary>
        /// IsSameKeyType：alg と鍵の取り違えを断る（P-256 の鍵で ES512 を名乗らせない。RSA の鍵で ES256 を名乗らせない）。
        /// 合わなければ CreateFromJwk も null。
        /// </summary>
        [Test]
        public void IsSameKeyType_RejectsAlgorithmConfusion()
        {
            JObject p256 = JwkOperator.ToPublic(PrivateJwk("ES256"));
            JObject rsa = JwkOperator.ToPublic(PrivateJwk("RS256"));

            Assert.That(JwsFactory.IsSameKeyType(p256, "ES256"), Is.True);
            Assert.That(JwsFactory.IsSameKeyType(p256, "ES512"), Is.False);
            Assert.That(JwsFactory.IsSameKeyType(rsa, "ES256"), Is.False);
            Assert.That(JwsFactory.IsSameKeyType(rsa, "PS512"), Is.True, "RSA の 1 本の鍵で RS* / PS*");
            Assert.That(JwsFactory.IsSameKeyType(rsa, "HS256"), Is.False);
            Assert.That(JwsFactory.IsSameKeyType(null, "RS256"), Is.False);

            Assert.That(JwsFactory.CreateFromJwk("ES512", p256), Is.Null);
            Assert.That(JwsFactory.CreateFromJwk("RS256", JObject.Parse("{\"kty\":\"RSA\",\"n\":\"!!\"}")), Is.Null, "読めない鍵は null");
        }

        /// <summary>公開鍵だけの JWK では、秘密鍵の版は作れない。</summary>
        [Test]
        public void CreateFromPrivateJwk_RequiresPrivateKey()
        {
            JObject pub = JwkOperator.ToPublic(PrivateJwk("RS256"));
            Assert.Throws<ArgumentException>(() => JwsFactory.CreateFromPrivateJwk("RS256", pub));
        }

        /// <summary>
        /// 証明書（*.cer）から検証器を作れる。扱わない alg は null。
        /// 署名側は、管理者でなくても読めるフラグで直に作る（CreateFromPfx の既定は MachineKeySet を含む）。
        /// </summary>
        [Test]
        public void CreateFromCertificate()
        {
            JWS signer = new JWS_RS256_X509(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags);
            string jwt = signer.Create(Payload);

            Assert.That(JwsFactory.CreateFromCertificate("RS256", TestCertificates.RsaCer).Verify(jwt), Is.True);
            Assert.That(JwsFactory.CreateFromCertificate("HS256", TestCertificates.RsaCer), Is.Null);
        }

        /// <summary>
        /// JwtReader：ヘッダ・ペイロード・alg / kid / typ を検証せずに読む。
        /// 読めない値は null（例外にしない）。JWE（5 区切り）はヘッダだけ読める。
        /// </summary>
        [Test]
        public void JwtReader_TryRead()
        {
            JWS signer = JwsFactory.CreateFromPrivateJwk("ES256", PrivateJwk("ES256"));
            signer.JWSHeader.kid = "k1";
            string jwt = signer.Create(Payload);

            Assert.That((string)JwtReader.TryReadPayload(jwt)["sub"], Is.EqualTo("nunit"));
            Assert.That(JwtReader.TryReadKid(jwt), Is.EqualTo("k1"));
            Assert.That(JwtReader.TryReadTyp(jwt), Is.EqualTo("JWT"));
            Assert.That(JwtReader.TryReadHeader(jwt)["alg"].ToString(), Is.EqualTo("ES256"));

            string header = jwt.Split('.')[0];
            Assert.That(JwtReader.TryReadHeader(header + ".a.b.c.d"), Is.Not.Null, "JWE のヘッダ");
            Assert.That(JwtReader.TryReadPayload(header + ".a.b.c.d"), Is.Null, "JWE のペイロードは読めない");

            Assert.That(JwtReader.TryReadHeader(null), Is.Null);
            Assert.That(JwtReader.TryReadHeader("abc"), Is.Null, "区切りが無い");
            Assert.That(JwtReader.TryReadHeader("!!.!!.!!"), Is.Null, "BASE64URL でない");
            Assert.That(JwtReader.TryReadPayload(CustomEncode.ToBase64UrlString(new byte[] { 0x5b, 0x5d }) + "." +
                CustomEncode.ToBase64UrlString(new byte[] { 0x5b, 0x5d }) + ".x"), Is.Null, "オブジェクトでない");
            Assert.That(JwtReader.TryReadKid(header.Replace(header, "e30") + ".e30.x"), Is.Null, "kid が無い");
            Assert.That(JwtReader.TryReadHeader(".e30.x"), Is.Null, "ヘッダが空");
            Assert.That(JwtReader.TryReadKid(null), Is.Null, "読めなければ null");
            Assert.That(JwtReader.TryReadAlg("abc"), Is.Null);
        }

        /// <summary>鍵の種類と alg が合わなければ null（RSA の鍵で ES*、EC の鍵で RS* を作らない）。</summary>
        [Test]
        public void CreateFromParameters_WrongAlg()
        {
            using (RSA rsa = RSA.Create(2048))
            using (ECDsa ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256))
            {
                Assert.That(JwsFactory.CreateFromRsaParameters("ES256", rsa.ExportParameters(false)), Is.Null);
                Assert.That(JwsFactory.CreateFromRsaParameters("HS256", rsa.ExportParameters(false)), Is.Null);
                Assert.That(JwsFactory.CreateFromEcParameters("RS256", ecdsa.ExportParameters(false)), Is.Null);
                Assert.That(JwsFactory.CreateFromRsaParameters("PS512", rsa.ExportParameters(false)), Is.Not.Null);
                Assert.That(JwsFactory.CreateFromEcParameters("ES256", ecdsa.ExportParameters(false)), Is.Not.Null);
            }
        }
    }
}

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
//* クラス名        ：JwtAssertionTests
//* クラス日本語名  ：JwtAssertion（JWT Bearer Token Flow のアサーション）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/19  玄人 幸道         新規作成
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace TestFramework.Authentication
{
    /// <summary>JwtAssertion（JWT Bearer Token Flow のアサーション）の単体テスト</summary>
    /// <remarks>
    /// クライアントが秘密鍵でアサーションを作り、認可サーバが公開鍵で検証する。鍵は RSAParameters・ECParameters・JWK の各形で渡せる。
    /// ECDSA は Windows では CNG で動く（Public の JwsEcdsaParamTests と同じく Windows で回す）。
    /// </remarks>
    [TestFixture]
    public class JwtAssertionTests
    {
        private const string Iss = "client01";
        private const string Aud = "https://as.example.com/token";
        private const string Scopes = "openid profile";

        /// <summary>RSA：RSAParameters で作って検証でき、iss・aud・scopes を返す。別の鍵・期限切れは通らない。</summary>
        [Test]
        public void Rsa_Param()
        {
            using (RSA rsa = RSA.Create(2048))
            using (RSA other = RSA.Create(2048))
            {
                string jwt = JwtAssertion.CreateByRsa(Iss, Aud, TimeSpan.FromMinutes(5), Scopes, rsa.ExportParameters(true));

                string iss, aud, scopes; JObject jobj;
                Assert.That(JwtAssertion.VerifyByRsa(jwt, out iss, out aud, out scopes, out jobj, rsa.ExportParameters(false)), Is.True);
                Assert.That(iss, Is.EqualTo(Iss));
                Assert.That(aud, Is.EqualTo(Aud));
                Assert.That(scopes, Is.EqualTo(Scopes));
                Assert.That((string)jobj[OAuth2AndOIDCConst.jti], Has.Length.EqualTo(32));

                Assert.That(JwtAssertion.VerifyByRsa(jwt, out iss, out aud, out scopes, out jobj, other.ExportParameters(false)), Is.False, "別の鍵");

                string expired = JwtAssertion.CreateByRsa(Iss, Aud, TimeSpan.FromMinutes(-5), Scopes, rsa.ExportParameters(true));
                Assert.That(JwtAssertion.VerifyByRsa(expired, out iss, out aud, out scopes, out jobj, rsa.ExportParameters(false)), Is.False, "期限切れ");
            }
        }

        /// <summary>RSA：JWK（秘密鍵・公開鍵の JSON）でも作って検証できる（kty で RSA / EC を選ぶ）。</summary>
        [Test]
        public void Rsa_Jwk()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                string privateJwk = new RsaPrivateKeyConverter().ParamToJwk(rsa.ExportParameters(true));
                string publicJwk = new RsaPublicKeyConverter().ParamToJwk(rsa.ExportParameters(false));

                string jwt = JwtAssertion.Create(Iss, Aud, TimeSpan.FromMinutes(5), Scopes, privateJwk);

                string iss, aud, scopes; JObject jobj;
                Assert.That(JwtAssertion.Verify(jwt, out iss, out aud, out scopes, out jobj, publicJwk), Is.True);
                Assert.That(iss, Is.EqualTo(Iss));
            }
        }

        /// <summary>kty が無い・知らない JWK は、作れず（空文字）、検証も通らない。</summary>
        [Test]
        public void UnknownKty()
        {
            Assert.That(JwtAssertion.Create(Iss, Aud, TimeSpan.FromMinutes(5), Scopes, "{\"kty\":\"oct\"}"), Is.Empty);
            Assert.That(JwtAssertion.Create(Iss, Aud, TimeSpan.FromMinutes(5), Scopes, "{}"), Is.Empty);

            string iss, aud, scopes; JObject jobj;
            Assert.That(JwtAssertion.Verify("a.b.c", out iss, out aud, out scopes, out jobj, "{\"kty\":\"oct\"}"), Is.False);
        }

        /// <summary>ECDSA（ES256）：ECParameters・JWK・証明書（*.pfx）で作って、公開鍵で検証できる。</summary>
        [Test]
        [Platform("Win")]
        public void Ecdsa()
        {
            ECParameters priv, pub;
            using (ECDsa ec = ECDsa.Create(ECCurve.NamedCurves.nistP256))
            {
                priv = ec.ExportParameters(true);
                pub = ec.ExportParameters(false);
            }

            string iss, aud, scopes; JObject jobj;

            string byParam = JwtAssertion.CreateByECDsa(Iss, Aud, TimeSpan.FromMinutes(5), Scopes, priv);
            Assert.That(JwtAssertion.VerifyByECDsa(byParam, out iss, out aud, out scopes, out jobj, pub), Is.True);
            Assert.That(iss, Is.EqualTo(Iss));

            string byJwk = JwtAssertion.Create(Iss, Aud, TimeSpan.FromMinutes(5), Scopes, new EccPrivateKeyConverter().ParamToJwk(priv));
            Assert.That(JwtAssertion.Verify(byJwk, out iss, out aud, out scopes, out jobj, new EccPublicKeyConverter().ParamToJwk(pub)), Is.True);

            using (ECDsa other = ECDsa.Create(ECCurve.NamedCurves.nistP256))
            {
                Assert.That(JwtAssertion.VerifyByECDsa(byParam, out iss, out aud, out scopes, out jobj, other.ExportParameters(false)), Is.False, "別の鍵");
            }

            // 証明書（*.pfx）で作り、証明書の公開鍵で検証する
            string byX509 = JwtAssertion.CreateByECDsa(Iss, Aud, TimeSpan.FromMinutes(5), Scopes,
                TestCertificates.EcdsaPfx256, TestCertificates.Password);
            ECParameters cerPub = TestCertificates.LoadPfx(TestCertificates.EcdsaPfx256).GetECDsaPublicKey().ExportParameters(false);
            Assert.That(JwtAssertion.VerifyByECDsa(byX509, out iss, out aud, out scopes, out jobj, cerPub), Is.True);
        }
    }
}

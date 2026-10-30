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
//* クラス名        ：RequestObjectTests
//* クラス日本語名  ：RequestObject / ClaimsInRO（OIDC・CIBA の要求オブジェクト）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/19  玄人 幸道         新規作成
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Public.Str;

namespace TestFramework.Authentication
{
    /// <summary>RequestObject / ClaimsInRO（OIDC・CIBA の要求オブジェクト）の単体テスト</summary>
    /// <remarks>
    /// 作った要求オブジェクトが署名どおりに検証できること（別の鍵では通らない）と、クレームの中身を確かめる。
    ///
    /// **必須のクレームが欠けたときの振る舞いは、ここでは確かめない。**
    /// Verify / VerifyCiba は、必須のクレームが欠けていても true を返す（「必須項目の不足」の分岐も true）。
    /// 不具合と思われるため、テストで固めずに報告している。
    /// </remarks>
    [TestFixture]
    public class RequestObjectTests
    {
        /// <summary>JWT のペイロード</summary>
        private static JObject PayloadOf(string jwt)
        {
            return JObject.Parse(Encoding.UTF8.GetString(CustomEncode.FromBase64UrlString(jwt.Split('.')[1])));
        }

        /// <summary>ClaimsInRO：userinfo・id_token（acr を含む）に分けて持つ。どちらも無ければ空。</summary>
        [Test]
        [Basis("OIDC Core 5.5（claims は userinfo と id_token に分ける）・5.5.1.1（acr は id_token で求める）")]
        public void ClaimsInRO()
        {
            ClaimsInRO claims = new ClaimsInRO(
                new Dictionary<string, object> { { "email", null } },
                new Dictionary<string, object> { { "auth_time", new { essential = true } } },
                new { values = new[] { "urn:mace:incommon:iap:silver" } });

            Assert.That(claims.Claims.ContainsKey(OAuth2AndOIDCConst.claims_userinfo), Is.True);
            Dictionary<string, object> idToken = (Dictionary<string, object>)claims.Claims[OAuth2AndOIDCConst.claims_id_token];
            Assert.That(idToken.ContainsKey("auth_time"), Is.True);
            Assert.That(idToken.ContainsKey(OAuth2AndOIDCConst.acr), Is.True);

            Assert.That(new ClaimsInRO(null, null, null).Claims, Is.Empty);
        }

        /// <summary>
        /// OIDC：RSAParameters で作り、公開鍵で検証できる。iss（= client_id）を返す。別の鍵では通らない。
        /// 渡したクレーム（claims を含む）が入り、空の任意項目は入らない。
        /// </summary>
        [Test]
        [Basis("RFC 9101 4（認可要求のパラメタを、Request Object のクレームにする）")]
        [Basis("RFC 9101 6.2（署名を検証する）")]
        public void Oidc_Rsa()
        {
            using (RSA rsa = RSA.Create(2048))
            using (RSA other = RSA.Create(2048))
            {
                ClaimsInRO claims = new ClaimsInRO(new Dictionary<string, object> { { "email", null } }, null, null);
                string ro = RequestObject.Create("client01", "https://op.example.com/ros",
                    "code", "", "https://rp.example.com/cb", "openid profile", "state01", "nonce01",
                    "", "", "", claims, rsa.ExportParameters(true));

                string iss;
                Assert.That(RequestObject.Verify(ro, out iss, rsa.ExportParameters(false)), Is.True);
                Assert.That(iss, Is.EqualTo("client01"));
                Assert.That(RequestObject.Verify(ro, out iss, other.ExportParameters(false)), Is.False, "別の鍵");

                JObject p = PayloadOf(ro);
                Assert.That((string)p[OAuth2AndOIDCConst.client_id], Is.EqualTo("client01"));
                Assert.That((string)p[OAuth2AndOIDCConst.response_type], Is.EqualTo("code"));
                Assert.That((string)p[OAuth2AndOIDCConst.state], Is.EqualTo("state01"));
                Assert.That(p.ContainsKey(OAuth2AndOIDCConst.response_mode), Is.False, "空の任意項目は入らない");
                Assert.That(p[OAuth2AndOIDCConst.claims][OAuth2AndOIDCConst.claims_userinfo], Is.Not.Null);
            }
        }

        /// <summary>OIDC：JWK（秘密鍵・公開鍵の JSON）でも作って検証できる。</summary>
        [Test]
        public void Oidc_Jwk()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                string ro = RequestObject.Create("client01", "https://op.example.com/ros",
                    "code", "form_post", "https://rp.example.com/cb", "openid", "state01", "nonce01",
                    "300", "login", "user01", new ClaimsInRO(null, null, null),
                    new RsaPrivateKeyConverter().ParamToJwk(rsa.ExportParameters(true)));

                string iss;
                Assert.That(RequestObject.Verify(ro, out iss, new RsaPublicKeyConverter().ParamToJwk(rsa.ExportParameters(false))), Is.True);

                JObject p = PayloadOf(ro);
                Assert.That((string)p[OAuth2AndOIDCConst.response_mode], Is.EqualTo("form_post"));
                Assert.That((string)p[OAuth2AndOIDCConst.max_age], Is.EqualTo("300"));
                Assert.That((string)p[OAuth2AndOIDCConst.prompt], Is.EqualTo("login"));
                Assert.That((string)p[OAuth2AndOIDCConst.login_hint], Is.EqualTo("user01"));
            }
        }

        /// <summary>
        /// CIBA：証明書（*.pfx）で作り、証明書の公開鍵（ECParameters・JWK）で検証できる。
        /// 要求の文脈（requestContextAndIntent）がクレームに入る。
        /// </summary>
        [Test]
        [Platform("Win")]
        [Basis("CIBA Core 7.1.1（署名した認証要求。iss は client_id）")]
        public void Ciba()
        {
            string ro = RequestObject.CreateCiba("client01", "https://op.example.com/",
                CmnJwtToken.CreateExpClaim(TimeSpan.FromMinutes(5)), CmnJwtToken.CreateIatClaim(), "openid",
                "token01", "message01", "", "120", "user01",
                new Dictionary<string, object> { { "intent", "payment" } },
                TestCertificates.EcdsaPfx256, TestCertificates.Password);

            ECParameters pub = TestCertificates.LoadPfx(TestCertificates.EcdsaPfx256).GetECDsaPublicKey().ExportParameters(false);

            string iss;
            Assert.That(RequestObject.VerifyCiba(ro, out iss, pub), Is.True);
            Assert.That(iss, Is.EqualTo("client01"));
            Assert.That(RequestObject.VerifyCiba(ro, out iss, new EccPublicKeyConverter().ParamToJwk(pub)), Is.True);

            using (ECDsa other = ECDsa.Create(ECCurve.NamedCurves.nistP256))
            {
                Assert.That(RequestObject.VerifyCiba(ro, out iss, other.ExportParameters(false)), Is.False, "別の鍵");
            }

            JObject p = PayloadOf(ro);
            Assert.That((string)p["intent"], Is.EqualTo("payment"));
            Assert.That((string)p[OAuth2AndOIDCConst.requested_expiry], Is.EqualTo("120"));
            Assert.That(p.ContainsKey(OAuth2AndOIDCConst.user_code), Is.False, "空の任意項目は入らない");
        }
    }
}

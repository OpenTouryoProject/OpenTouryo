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
//*  2026/11/05  玄人 幸道         必須のクレーム・時刻のクレームを確かめるテストを追加（#653）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Newtonsoft.Json;
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
    /// **必須のクレームが欠けたもの・期限の過ぎたものは false になる**（#653。2026/11/05 までは true を返していた）。
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

        #region 必須のクレーム・時刻のクレーム（#653）

        /// <summary>ペイロードを書き換える手と、その説明</summary>
        private class Mutation
        {
            public string Label;
            public Action<JObject> Apply;
            public Mutation(string label, Action<JObject> apply) { this.Label = label; this.Apply = apply; }
        }

        /// <summary>いまの UNIX 時刻（秒）</summary>
        private static long Now()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>OIDC の要求オブジェクトの、必須のクレームだけを持つペイロード</summary>
        private static JObject OidcPayload()
        {
            return new JObject
            {
                { OAuth2AndOIDCConst.iss, "client01" },
                { OAuth2AndOIDCConst.aud, "https://op.example.com/" },
                { OAuth2AndOIDCConst.response_type, "code" },
                { OAuth2AndOIDCConst.client_id, "client01" },
                { OAuth2AndOIDCConst.scope, "openid" },
            };
        }

        /// <summary>CIBA の認証要求の、必須のクレームだけを持つペイロード</summary>
        private static JObject CibaPayload()
        {
            long now = RequestObjectTests.Now();
            return new JObject
            {
                { OAuth2AndOIDCConst.iss, "client01" },
                { OAuth2AndOIDCConst.aud, "https://op.example.com/" },
                { OAuth2AndOIDCConst.exp, now + 300 },
                { OAuth2AndOIDCConst.iat, now },
                { OAuth2AndOIDCConst.nbf, now },
                { OAuth2AndOIDCConst.jti, "jti01" },
                { OAuth2AndOIDCConst.scope, "openid" },
                { OAuth2AndOIDCConst.login_hint, "user01" },
            };
        }

        /// <summary>
        /// OIDC：必須のクレーム（iss・aud・response_type・client_id・scope）がそろえば通り、
        /// 欠けている・空・文字列でないものは false。state・nonce・exp は無くてよい。exp・nbf は在れば値を確かめる。
        /// </summary>
        [Test]
        [Basis("RFC 9101 4（署名した Request Object は iss・aud を含める）")]
        [Basis("RFC 9101 6.3（AS は Request Object の中のパラメタだけを使う）・RFC 6749 4.1.1（response_type・client_id は必須）")]
        [Basis("OIDC Core 3.1.2.1（scope は必須。state・nonce は認可コード フローでは任意）")]
        [Basis("RFC 7519 4.1.4・4.1.5（exp を過ぎたら・nbf より前なら受けない）")]
        [Basis("#653")]
        public void Oidc_Claims()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                JWS_RS256_Param jws = new JWS_RS256_Param(rsa.ExportParameters(true));
                RSAParameters pub = rsa.ExportParameters(false);
                long now = RequestObjectTests.Now();
                string iss;

                // 通るもの
                Mutation[] valid = new Mutation[]
                {
                    new Mutation("必須だけ（state・nonce・exp は無い）", p => { }),
                    new Mutation("aud が配列", p => p[OAuth2AndOIDCConst.aud] = new JArray("https://op.example.com/", "other")),
                    new Mutation("exp が未来", p => p[OAuth2AndOIDCConst.exp] = now + 300),
                    new Mutation("exp が未来（文字列。CreateExpClaim の形）", p => p[OAuth2AndOIDCConst.exp] = (now + 300).ToString()),
                    new Mutation("nbf が過去", p => p[OAuth2AndOIDCConst.nbf] = now - 60),
                };

                foreach (Mutation m in valid)
                {
                    JObject p = RequestObjectTests.OidcPayload();
                    m.Apply(p);
                    Assert.That(RequestObject.Verify(jws.Create(p.ToString(Formatting.None)), out iss, pub), Is.True, m.Label);
                    Assert.That(iss, Is.EqualTo("client01"), m.Label);
                }

                // 断るもの
                List<Mutation> invalid = new List<Mutation>();
                foreach (string name in new string[] {
                    OAuth2AndOIDCConst.iss, OAuth2AndOIDCConst.aud, OAuth2AndOIDCConst.response_type,
                    OAuth2AndOIDCConst.client_id, OAuth2AndOIDCConst.scope })
                {
                    string n = name;
                    invalid.Add(new Mutation(n + " が無い", p => p.Remove(n)));
                    invalid.Add(new Mutation(n + " が空", p => p[n] = ""));
                    invalid.Add(new Mutation(n + " が数値", p => p[n] = 1));
                }
                invalid.Add(new Mutation("aud が空の配列", p => p[OAuth2AndOIDCConst.aud] = new JArray()));
                invalid.Add(new Mutation("aud の配列に数値", p => p[OAuth2AndOIDCConst.aud] = new JArray("https://op.example.com/", 1)));
                invalid.Add(new Mutation("exp が過去", p => p[OAuth2AndOIDCConst.exp] = now - 60));
                invalid.Add(new Mutation("exp が読めない", p => p[OAuth2AndOIDCConst.exp] = "abc"));
                invalid.Add(new Mutation("exp が null", p => p[OAuth2AndOIDCConst.exp] = JValue.CreateNull()));
                invalid.Add(new Mutation("nbf が未来", p => p[OAuth2AndOIDCConst.nbf] = now + 300));

                foreach (Mutation m in invalid)
                {
                    JObject p = RequestObjectTests.OidcPayload();
                    m.Apply(p);
                    Assert.That(RequestObject.Verify(jws.Create(p.ToString(Formatting.None)), out iss, pub), Is.False, m.Label);
                }

                // ペイロードがオブジェクトでない
                Assert.That(RequestObject.Verify(jws.Create("[1]"), out iss, pub), Is.False, "ペイロードが配列");

                // Create で、nonce・state を空にして作ったものも通る（Create は空の nonce を入れない）
                string ro = RequestObject.Create("client01", "https://op.example.com/",
                    "code", "", "https://rp.example.com/cb", "openid", "", "",
                    "", "", "", new ClaimsInRO(null, null, null), rsa.ExportParameters(true));
                Assert.That(RequestObject.Verify(ro, out iss, pub), Is.True, "Create（nonce 無し）");
            }
        }

        /// <summary>
        /// CIBA：必須のクレーム（aud・iss・exp・iat・nbf・jti・scope）とヒント 1 つがそろえば通り、
        /// 欠けている・空・読めない・期限の過ぎたもの、ヒントが無い・複数のものは false。
        /// client_notification_token・binding_message は無くてよい。
        /// </summary>
        [Test]
        [Platform("Win")]
        [Basis("CIBA Core 7.1.1（署名した認証要求は aud・iss・exp・iat・nbf・jti を含む）")]
        [Basis("CIBA Core 7.1（scope は必須、ヒントは one (and only one)、client_notification_token は Ping・Push のときだけ、binding_message は任意）")]
        [Basis("RFC 7519 4.1.4・4.1.5（exp を過ぎたら・nbf より前なら受けない）")]
        [Basis("#653")]
        public void Ciba_Claims()
        {
            JWS_ES256_X509 jws = new JWS_ES256_X509(TestCertificates.EcdsaPfx256, TestCertificates.Password);
            ECParameters pub = TestCertificates.LoadPfx(TestCertificates.EcdsaPfx256).GetECDsaPublicKey().ExportParameters(false);
            long now = RequestObjectTests.Now();
            string iss;

            // 通るもの
            Mutation[] valid = new Mutation[]
            {
                new Mutation("必須だけ（client_notification_token・binding_message は無い）", p => { }),
                new Mutation("ヒントが login_hint_token", p => { p.Remove(OAuth2AndOIDCConst.login_hint); p[OAuth2AndOIDCConst.login_hint_token] = "token"; }),
                new Mutation("ヒントが id_token_hint", p => { p.Remove(OAuth2AndOIDCConst.login_hint); p[OAuth2AndOIDCConst.id_token_hint] = "idt"; }),
                new Mutation("時刻が文字列（CreateCiba の形）", p =>
                {
                    p[OAuth2AndOIDCConst.exp] = (now + 300).ToString();
                    p[OAuth2AndOIDCConst.iat] = now.ToString();
                    p[OAuth2AndOIDCConst.nbf] = now.ToString();
                }),
                new Mutation("exp が小数", p => p[OAuth2AndOIDCConst.exp] = now + 300.5),
            };

            foreach (Mutation m in valid)
            {
                JObject p = RequestObjectTests.CibaPayload();
                m.Apply(p);
                Assert.That(RequestObject.VerifyCiba(jws.Create(p.ToString(Formatting.None)), out iss, pub), Is.True, m.Label);
                Assert.That(iss, Is.EqualTo("client01"), m.Label);
            }

            // 断るもの
            List<Mutation> invalid = new List<Mutation>();
            foreach (string name in new string[] {
                OAuth2AndOIDCConst.aud, OAuth2AndOIDCConst.iss, OAuth2AndOIDCConst.exp, OAuth2AndOIDCConst.iat,
                OAuth2AndOIDCConst.nbf, OAuth2AndOIDCConst.jti, OAuth2AndOIDCConst.scope })
            {
                string n = name;
                invalid.Add(new Mutation(n + " が無い", p => p.Remove(n)));
                invalid.Add(new Mutation(n + " が空", p => p[n] = ""));
            }
            invalid.Add(new Mutation("ヒントが無い", p => p.Remove(OAuth2AndOIDCConst.login_hint)));
            invalid.Add(new Mutation("ヒントが空", p => p[OAuth2AndOIDCConst.login_hint] = ""));
            invalid.Add(new Mutation("ヒントが 2 つ", p => p[OAuth2AndOIDCConst.id_token_hint] = "idt"));
            invalid.Add(new Mutation("exp が過去", p => p[OAuth2AndOIDCConst.exp] = now - 60));
            invalid.Add(new Mutation("nbf が未来", p => p[OAuth2AndOIDCConst.nbf] = now + 300));
            invalid.Add(new Mutation("iat が読めない", p => p[OAuth2AndOIDCConst.iat] = "abc"));
            invalid.Add(new Mutation("jti が数値", p => p[OAuth2AndOIDCConst.jti] = 1));

            foreach (Mutation m in invalid)
            {
                JObject p = RequestObjectTests.CibaPayload();
                m.Apply(p);
                Assert.That(RequestObject.VerifyCiba(jws.Create(p.ToString(Formatting.None)), out iss, pub), Is.False, m.Label);
            }
        }

        #endregion
    }
}

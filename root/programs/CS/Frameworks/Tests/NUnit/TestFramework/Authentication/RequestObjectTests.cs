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
//*  2026/11/05  玄人 幸道         日付の形の文字列のクレームを通すことを足した（#653 ph1 のレビューの指摘 1）
//*  2026/11/05  玄人 幸道         期待する aud を受ける版のテストを追加（#653 ph2）
//*  2026/11/05  玄人 幸道         aud の照合のケースを足し、OIDC と CIBA をそろえた（#653 ph2 のレビューの指摘 1・2・3）
//*  2026/11/05  玄人 幸道         aud を受けない版の Obsolete の警告（CS0618）を抑えた（#653）
//*  2026/11/05  玄人 幸道         形の壊れた入力を、例外にせず false にすることを足した（#654 のレビューの指摘 5）
//*  2026/11/06  玄人 幸道         有効期間を受ける Create（exp・nbf・iat を入れる）のテストを追加（#660）
//*  2026/11/06  玄人 幸道         nbf の範囲を Create の前後の時刻で挟み、TimeSpan.MaxValue を足した（#660 のレビューの指摘 3・4）
//*  2026/11/06  玄人 幸道         検証鍵（JWS）を受ける Verify・VerifyCiba のテストを追加（#664）
//*  2026/11/08  玄人 幸道         code_challenge・code_challenge_method を受ける Create のテストを追加（#675）
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

// aud を受けない Verify・VerifyCiba は Obsolete（#653）だが、利用者が使い続ける版なので、振る舞いを確かめ続ける。
#pragma warning disable CS0618

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
        /// OIDC：有効期間を受ける Create は、nbf・iat に今の時刻、exp に nbf + 有効期間を、JSON の数値で入れる。
        /// 期待する aud を受ける Verify で通る。RSAParameters 版・JWK 版のどちらも同じ。
        /// </summary>
        [Test]
        [Basis("FAPI 1.0 Advanced 5.2.3-11・14（クライアントは、要求オブジェクトに exp・nbf を送る）")]
        [Basis("RFC 7519 2（NumericDate は JSON の数値）・4.1.4・4.1.5・4.1.6")]
        [Basis("#660")]
        public void Oidc_Lifetime()
        {
            const string op = "https://op.example.com/";

            using (RSA rsa = RSA.Create(2048))
            {
                long before = RequestObjectTests.Now();
                string[] ros = new string[]
                {
                    RequestObject.Create("client01", op,
                        "code", "", "https://rp.example.com/cb", "openid", "state01", "nonce01",
                        "", "", "", new ClaimsInRO(null, null, null), TimeSpan.FromMinutes(10), rsa.ExportParameters(true)),
                    RequestObject.Create("client01", op,
                        "code", "", "https://rp.example.com/cb", "openid", "state01", "nonce01",
                        "", "", "", new ClaimsInRO(null, null, null), TimeSpan.FromMinutes(10),
                        new RsaPrivateKeyConverter().ParamToJwk(rsa.ExportParameters(true)))
                };
                long after = RequestObjectTests.Now();

                foreach (string ro in ros)
                {
                    JObject p = PayloadOf(ro);

                    Assert.That(p[OAuth2AndOIDCConst.exp].Type, Is.EqualTo(JTokenType.Integer), "exp は数値");
                    Assert.That(p[OAuth2AndOIDCConst.nbf].Type, Is.EqualTo(JTokenType.Integer), "nbf は数値");
                    Assert.That(p[OAuth2AndOIDCConst.iat].Type, Is.EqualTo(JTokenType.Integer), "iat は数値");

                    long nbf = (long)p[OAuth2AndOIDCConst.nbf];
                    Assert.That(nbf, Is.InRange(before, after), "nbf は今の時刻");
                    Assert.That((long)p[OAuth2AndOIDCConst.iat], Is.EqualTo(nbf), "iat = nbf");
                    Assert.That((long)p[OAuth2AndOIDCConst.exp], Is.EqualTo(nbf + 600), "exp = nbf + 有効期間");

                    string iss;
                    Assert.That(RequestObject.Verify(ro, op, out iss, rsa.ExportParameters(false)), Is.True);
                    Assert.That(iss, Is.EqualTo("client01"));
                }

                // 上限は見ない。TimeSpan.MaxValue でも long は溢れない
                JObject max = PayloadOf(RequestObject.Create("client01", op,
                    "code", "", "https://rp.example.com/cb", "openid", "state01", "nonce01",
                    "", "", "", new ClaimsInRO(null, null, null), TimeSpan.MaxValue, rsa.ExportParameters(true)));
                Assert.That((long)max[OAuth2AndOIDCConst.exp],
                    Is.EqualTo((long)max[OAuth2AndOIDCConst.nbf] + (long)TimeSpan.MaxValue.TotalSeconds), "TimeSpan.MaxValue");

                // 1 秒未満の有効期間は受けない
                foreach (TimeSpan lifetime in new TimeSpan[] { TimeSpan.Zero, TimeSpan.FromSeconds(-1), TimeSpan.FromMilliseconds(500) })
                {
                    ArgumentOutOfRangeException e = Assert.Throws<ArgumentOutOfRangeException>(() => RequestObject.Create("client01", op,
                        "code", "", "https://rp.example.com/cb", "openid", "state01", "nonce01",
                        "", "", "", new ClaimsInRO(null, null, null), lifetime, rsa.ExportParameters(true)), lifetime.ToString());
                    Assert.That(e.ParamName, Is.EqualTo("lifetime"));
                }
            }
        }

        /// <summary>OIDC：有効期間を受けない Create は、exp・nbf・iat を入れない（従来どおり）。</summary>
        [Test]
        [Basis("設計：RequestObject.Create（有効期間を受ける版）の remarks（有効期間を受けない版は、どれも入れない）")]
        [Basis("#660")]
        public void Oidc_NoLifetime_NoTimeClaims()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                JObject p = PayloadOf(RequestObject.Create("client01", "https://op.example.com/",
                    "code", "", "https://rp.example.com/cb", "openid", "state01", "nonce01",
                    "", "", "", new ClaimsInRO(null, null, null), rsa.ExportParameters(true)));

                Assert.That(p.ContainsKey(OAuth2AndOIDCConst.exp), Is.False);
                Assert.That(p.ContainsKey(OAuth2AndOIDCConst.nbf), Is.False);
                Assert.That(p.ContainsKey(OAuth2AndOIDCConst.iat), Is.False);
            }
        }

        /// <summary>
        /// OIDC：code_challenge を受ける Create は、code_challenge・code_challenge_method を Request Object に入れ、
        /// exp・nbf・iat も入れる。入った値で、認可サーバ側の PKCE の検証（VerifyPkce）が通る。RSAParameters 版・JWK 版のどちらも同じ。
        /// </summary>
        [Test]
        [Basis("RFC 9101 4（認可要求のパラメタは、Request Object のクレームにする）")]
        [Basis("RFC 7636 4.2（S256 の code_challenge は BASE64URL(SHA256(ASCII(code_verifier)))）・4.3（code_challenge・code_challenge_method を送る）")]
        [Basis("FAPI 2.0 Security Profile 5.3.3.2（クライアントは PAR と、S256 の PKCE を使う）")]
        [Basis("#675")]
        public void Oidc_Pkce()
        {
            const string op = "https://op.example.com/";
            const string code_verifier = "dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk";
            string code_challenge = OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod(code_verifier);

            using (RSA rsa = RSA.Create(2048))
            {
                long before = RequestObjectTests.Now();
                string[] ros = new string[]
                {
                    RequestObject.Create("client01", op,
                        "code", "", "https://rp.example.com/cb", "openid", "state01", "nonce01",
                        "", "", "", new ClaimsInRO(null, null, null),
                        code_challenge, OAuth2AndOIDCConst.PKCE_S256, TimeSpan.FromMinutes(10), rsa.ExportParameters(true)),
                    RequestObject.Create("client01", op,
                        "code", "", "https://rp.example.com/cb", "openid", "state01", "nonce01",
                        "", "", "", new ClaimsInRO(null, null, null),
                        code_challenge, OAuth2AndOIDCConst.PKCE_S256, TimeSpan.FromMinutes(10),
                        new RsaPrivateKeyConverter().ParamToJwk(rsa.ExportParameters(true)))
                };
                long after = RequestObjectTests.Now();

                foreach (string ro in ros)
                {
                    JObject p = PayloadOf(ro);

                    Assert.That((string)p[OAuth2AndOIDCConst.code_challenge], Is.EqualTo(code_challenge));
                    Assert.That((string)p[OAuth2AndOIDCConst.code_challenge_method], Is.EqualTo("S256"));

                    // 入った値で、認可サーバ側の PKCE の検証が通る（S256 だけを受ける設定で）
                    bool usedS256;
                    Assert.That(OAuth2AndOIDCSpec.VerifyPkce(
                        (string)p[OAuth2AndOIDCConst.code_challenge_method], (string)p[OAuth2AndOIDCConst.code_challenge],
                        code_verifier, false, out usedS256), Is.True, "VerifyPkce");
                    Assert.That(usedS256, Is.True);

                    // exp・nbf・iat も入る（有効期間を受ける版と同じ）
                    long nbf = (long)p[OAuth2AndOIDCConst.nbf];
                    Assert.That(nbf, Is.InRange(before, after), "nbf は今の時刻");
                    Assert.That((long)p[OAuth2AndOIDCConst.exp], Is.EqualTo(nbf + 600), "exp = nbf + 有効期間");

                    string iss;
                    Assert.That(RequestObject.Verify(ro, op, out iss, rsa.ExportParameters(false)), Is.True);
                    Assert.That(iss, Is.EqualTo("client01"));
                }
            }
        }

        /// <summary>
        /// OIDC：code_challenge を受ける Create の、code_challenge_method の扱い。
        /// 空なら S256 を入れ、渡した値（plain も）はそのまま入れる。
        /// code_challenge・code_challenge_method が両方空ならどちらも入れず、code_challenge だけが空なら ArgumentException。
        /// </summary>
        [Test]
        [Basis("RFC 7636 4.3（code_challenge_method を省くと plain として扱われる。だから S256 を明示する）")]
        [Basis("設計：RequestObject.Create（code_challenge を受ける版）の remarks（空なら S256、渡した値はそのまま、method だけなら ArgumentException）")]
        [Basis("#675")]
        public void Oidc_Pkce_Method()
        {
            const string op = "https://op.example.com/";

            using (RSA rsa = RSA.Create(2048))
            {
                Func<string, string, JObject> create = (challenge, method) => PayloadOf(RequestObject.Create("client01", op,
                    "code", "", "https://rp.example.com/cb", "openid", "state01", "nonce01",
                    "", "", "", new ClaimsInRO(null, null, null),
                    challenge, method, TimeSpan.FromMinutes(10), rsa.ExportParameters(true)));

                // 空（null も）なら S256
                foreach (string method in new string[] { "", null })
                {
                    JObject p = create("challenge01", method);
                    Assert.That((string)p[OAuth2AndOIDCConst.code_challenge], Is.EqualTo("challenge01"));
                    Assert.That((string)p[OAuth2AndOIDCConst.code_challenge_method], Is.EqualTo("S256"), "method = " + (method ?? "null"));
                }

                // 渡した値は、そのまま入れる（plain も断らない。受けるかは認可サーバが決める）
                Assert.That((string)create("challenge01", "plain")[OAuth2AndOIDCConst.code_challenge_method], Is.EqualTo("plain"));

                // 両方空なら、どちらも入れない（exp・nbf・iat は入る）
                foreach (string challenge in new string[] { "", null })
                {
                    JObject p = create(challenge, challenge);
                    Assert.That(p.ContainsKey(OAuth2AndOIDCConst.code_challenge), Is.False);
                    Assert.That(p.ContainsKey(OAuth2AndOIDCConst.code_challenge_method), Is.False);
                    Assert.That(p.ContainsKey(OAuth2AndOIDCConst.exp), Is.True);
                }

                // code_challenge が空で code_challenge_method だけがあれば、作り忘れとして断る
                foreach (string challenge in new string[] { "", null })
                {
                    ArgumentException e = Assert.Throws<ArgumentException>(() => create(challenge, "S256"));
                    Assert.That(e.ParamName, Is.EqualTo("code_challenge"));
                }

                // 1 秒未満の有効期間は受けない（有効期間を受ける版と同じ）
                ArgumentOutOfRangeException e2 = Assert.Throws<ArgumentOutOfRangeException>(() => RequestObject.Create("client01", op,
                    "code", "", "https://rp.example.com/cb", "openid", "state01", "nonce01",
                    "", "", "", new ClaimsInRO(null, null, null),
                    "challenge01", "S256", TimeSpan.Zero, rsa.ExportParameters(true)));
                Assert.That(e2.ParamName, Is.EqualTo("lifetime"));
            }
        }

        /// <summary>OIDC：code_challenge を受けない Create は、code_challenge・code_challenge_method を入れない（従来どおり）。</summary>
        [Test]
        [Basis("設計：RequestObject.Create（code_challenge を受ける版）の remarks（受けない版は、従来どおり入れない）")]
        [Basis("#675")]
        public void Oidc_NoPkce()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                JObject[] ps = new JObject[]
                {
                    PayloadOf(RequestObject.Create("client01", "https://op.example.com/",
                        "code", "", "https://rp.example.com/cb", "openid", "state01", "nonce01",
                        "", "", "", new ClaimsInRO(null, null, null), rsa.ExportParameters(true))),
                    PayloadOf(RequestObject.Create("client01", "https://op.example.com/",
                        "code", "", "https://rp.example.com/cb", "openid", "state01", "nonce01",
                        "", "", "", new ClaimsInRO(null, null, null), TimeSpan.FromMinutes(10), rsa.ExportParameters(true)))
                };

                foreach (JObject p in ps)
                {
                    Assert.That(p.ContainsKey(OAuth2AndOIDCConst.code_challenge), Is.False);
                    Assert.That(p.ContainsKey(OAuth2AndOIDCConst.code_challenge_method), Is.False);
                }
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
                    new Mutation("文字列のクレームが日付の形（日付に変えずに読む）", p =>
                    {
                        p[OAuth2AndOIDCConst.client_id] = "2026-11-05T00:00:00Z";
                        p[OAuth2AndOIDCConst.scope] = "2026-11-05T09:00:00+09:00";
                    }),
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
                new Mutation("文字列のクレームが日付の形（日付に変えずに読む）", p =>
                {
                    p[OAuth2AndOIDCConst.jti] = "2026-11-05T00:00:00Z";
                    p[OAuth2AndOIDCConst.login_hint] = "2026-11-05T00:00:00+09:00";
                }),
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

        /// <summary>
        /// OIDC：aud を受ける版は、aud が期待する値と一致する（配列なら含む）ものだけを通す。
        /// 大文字・小文字の違い・含まない配列は false。ほかの必須のクレームも、aud を受けない版と同じく確かめる。
        /// aud を受けない版は、aud の値を照合しない。
        /// </summary>
        [Test]
        [Basis("RFC 9101 4（aud は認可サーバの issuer）・RFC 7519 4.1.3（aud は StringOrURI か、その配列。自分を含まなければ断る）")]
        [Basis("設計：RequestObject.Verify（aud を受けない版）の remarks（aud の値は照合しない）")]
        [Basis("#653")]
        public void Oidc_Aud()
        {
            const string op = "https://op.example.com/";

            using (RSA rsa = RSA.Create(2048))
            {
                JWS_RS256_Param jws = new JWS_RS256_Param(rsa.ExportParameters(true));
                RSAParameters pub = rsa.ExportParameters(false);
                string pubJwk = new RsaPublicKeyConverter().ParamToJwk(pub);
                string iss;

                Func<Action<JObject>, string> sign = m =>
                {
                    JObject p = RequestObjectTests.OidcPayload();
                    m(p);
                    return jws.Create(p.ToString(Formatting.None));
                };

                string same = sign(p => { });
                Assert.That(RequestObject.Verify(same, op, out iss, pub), Is.True, "一致");
                Assert.That(iss, Is.EqualTo("client01"));
                Assert.That(RequestObject.Verify(same, op, out iss, pubJwk), Is.True, "一致（JWK の版）");
                Assert.That(RequestObject.Verify(sign(p => p[OAuth2AndOIDCConst.aud] = new JArray("other", op)), op, out iss, pub),
                    Is.True, "配列が含む");

                string other = sign(p => p[OAuth2AndOIDCConst.aud] = "https://other.example.com/");
                Assert.That(RequestObject.Verify(other, op, out iss, pub), Is.False, "違う");
                Assert.That(RequestObject.Verify(other, op, out iss, pubJwk), Is.False, "違う（JWK の版）");
                Assert.That(RequestObject.Verify(other, out iss, pub), Is.True, "aud を受けない版は照合しない");
                Assert.That(RequestObject.Verify(sign(p => p[OAuth2AndOIDCConst.aud] = op.ToUpperInvariant()), op, out iss, pub),
                    Is.False, "大文字・小文字が違う");
                Assert.That(RequestObject.Verify(sign(p => p[OAuth2AndOIDCConst.aud] = new JArray("a", "b")), op, out iss, pub),
                    Is.False, "配列が含まない");
                Assert.That(RequestObject.Verify(sign(p => p[OAuth2AndOIDCConst.aud] = new JArray(op, 1)), op, out iss, pub),
                    Is.False, "配列が含むが、文字列でない要素もある");
                Assert.That(RequestObject.Verify(sign(p => p[OAuth2AndOIDCConst.aud] = "https://op.example.com"), op, out iss, pub),
                    Is.False, "末尾のスラッシュが違う（正規化しない）");
                Assert.That(RequestObject.Verify(sign(p => p.Remove(OAuth2AndOIDCConst.aud)), op, out iss, pub), Is.False, "aud が無い");
                Assert.That(RequestObject.Verify(sign(p => p.Remove(OAuth2AndOIDCConst.scope)), op, out iss, pub),
                    Is.False, "ほかの必須（scope）が無い");

                foreach (string broken in JwtAssertionTests.BrokenJwts())
                {
                    Assert.That(RequestObject.Verify(broken, op, out iss, pub), Is.False, "形が壊れている : " + (broken ?? "null"));
                    Assert.That(RequestObject.Verify(broken, out iss, pub), Is.False, "形が壊れている（aud を受けない版） : " + (broken ?? "null"));
                }

                Assert.Throws<ArgumentNullException>(() => RequestObject.Verify(same, null, out iss, pub), "期待する aud が null");
                Assert.Throws<ArgumentNullException>(() => RequestObject.Verify(same, "", out iss, pub), "期待する aud が空");
            }
        }

        /// <summary>
        /// CIBA：aud を受ける版は、aud が期待する値（OP の Issuer Identifier）と一致する（配列なら含む）ものだけを通す。
        /// ほかの必須のクレームも、aud を受けない版と同じく確かめる。aud を受けない版は、aud の値を照合しない。
        /// </summary>
        [Test]
        [Platform("Win")]
        [Basis("CIBA Core 7.1.1（aud は OP の Issuer Identifier）・RFC 7519 4.1.3（自分を含まなければ断る）")]
        [Basis("設計：RequestObject.VerifyCiba（aud を受けない版）の remarks（aud の値は照合しない）")]
        [Basis("#653")]
        public void Ciba_Aud()
        {
            const string op = "https://op.example.com/";

            JWS_ES256_X509 jws = new JWS_ES256_X509(TestCertificates.EcdsaPfx256, TestCertificates.Password);
            ECParameters pub = TestCertificates.LoadPfx(TestCertificates.EcdsaPfx256).GetECDsaPublicKey().ExportParameters(false);
            string pubJwk = new EccPublicKeyConverter().ParamToJwk(pub);
            string iss;

            Func<Action<JObject>, string> sign = m =>
            {
                JObject p = RequestObjectTests.CibaPayload();
                m(p);
                return jws.Create(p.ToString(Formatting.None));
            };

            string same = sign(p => { });
            Assert.That(RequestObject.VerifyCiba(same, op, out iss, pub), Is.True, "一致");
            Assert.That(iss, Is.EqualTo("client01"));
            Assert.That(RequestObject.VerifyCiba(same, op, out iss, pubJwk), Is.True, "一致（JWK の版）");
            Assert.That(RequestObject.VerifyCiba(sign(p => p[OAuth2AndOIDCConst.aud] = new JArray(op, "other")), op, out iss, pub),
                Is.True, "配列が含む");

            string other = sign(p => p[OAuth2AndOIDCConst.aud] = "https://op.example.com/ros");
            Assert.That(RequestObject.VerifyCiba(other, op, out iss, pub), Is.False, "違う（エンドポイントの URL）");
            Assert.That(RequestObject.VerifyCiba(other, op, out iss, pubJwk), Is.False, "違う（JWK の版）");
            Assert.That(RequestObject.VerifyCiba(other, out iss, pub), Is.True, "aud を受けない版は照合しない");
            Assert.That(RequestObject.VerifyCiba(sign(p => p[OAuth2AndOIDCConst.aud] = op.ToUpperInvariant()), op, out iss, pub),
                Is.False, "大文字・小文字が違う");
            Assert.That(RequestObject.VerifyCiba(sign(p => p[OAuth2AndOIDCConst.aud] = new JArray("a", "b")), op, out iss, pub),
                Is.False, "配列が含まない");
            Assert.That(RequestObject.VerifyCiba(sign(p => p[OAuth2AndOIDCConst.aud] = new JArray(op, 1)), op, out iss, pub),
                Is.False, "配列が含むが、文字列でない要素もある");
            Assert.That(RequestObject.VerifyCiba(sign(p => p[OAuth2AndOIDCConst.aud] = "https://op.example.com"), op, out iss, pub),
                Is.False, "末尾のスラッシュが違う（正規化しない）");
            Assert.That(RequestObject.VerifyCiba(sign(p => p.Remove(OAuth2AndOIDCConst.aud)), op, out iss, pub), Is.False, "aud が無い");
            Assert.That(RequestObject.VerifyCiba(sign(p => p.Remove(OAuth2AndOIDCConst.jti)), op, out iss, pub),
                Is.False, "ほかの必須（jti）が無い");

            foreach (string broken in JwtAssertionTests.BrokenJwts())
            {
                Assert.That(RequestObject.VerifyCiba(broken, op, out iss, pub), Is.False, "形が壊れている : " + (broken ?? "null"));
            }

            Assert.Throws<ArgumentNullException>(() => RequestObject.VerifyCiba(same, null, out iss, pub), "期待する aud が null");
            Assert.Throws<ArgumentNullException>(() => RequestObject.VerifyCiba(same, "", out iss, pub), "期待する aud が空");
        }

        #endregion

        #region 検証鍵（JWS）を受ける版（#664）

        /// <summary>Verify は、ヘッダの alg を見ない検証器（テスト用）。CmnJwtToken.TryVerifySignature の alg の照合を確かめる。</summary>
        internal class AnyAlgJws : JWS
        {
            /// <summary>alg を名乗るだけで、署名は確かめない</summary>
            public AnyAlgJws(string alg)
            {
                this.JWSHeader = new JWS_Header() { alg = alg };
            }

            /// <summary>使わない</summary>
            public override string Create(string payloadJson)
            {
                throw new NotSupportedException();
            }

            /// <summary>常に true（ヘッダの alg も署名も見ない）</summary>
            public override bool Verify(string jwtString)
            {
                return true;
            }
        }

        /// <summary>ヘッダの alg を指定した、署名の無い（でたらめな）JWS</summary>
        internal static string UnsignedJws(string alg, JObject payload)
        {
            Func<string, string> b64 = x => CustomEncode.ToBase64UrlString(Encoding.UTF8.GetBytes(x));
            return b64(new JObject { { "alg", alg }, { "typ", "JWT" } }.ToString(Formatting.None)) + "."
                + b64(payload.ToString(Formatting.None)) + ".c2ln";
        }

        /// <summary>
        /// OIDC：渡した検証鍵の alg（RS256・RS384・RS512・PS256・PS384・PS512）で署名したものは通り、
        /// ヘッダの alg が鍵の alg と違うもの・別の鍵・クレームの足りないもの・aud の違うものは false。
        /// </summary>
        [Test]
        [Basis("FAPI 1.0 Advanced 8.6（JWS は PS256 か ES256 を使う）")]
        [Basis("RFC 8725 3.1（呼び出し側が受ける alg を決め、ほかの alg は使わない）")]
        [Basis("RFC 9101 4・6.2（署名を検証し、aud は認可サーバの issuer）")]
        [Basis("#664")]
        public void Oidc_Jws()
        {
            const string op = "https://op.example.com/";
            string iss;

            RSAParameters priv, pub, otherPub;
            using (RSA rsa = RSA.Create(2048))
            {
                priv = rsa.ExportParameters(true);
                pub = rsa.ExportParameters(false);
            }
            using (RSA other = RSA.Create(2048))
            {
                otherPub = other.ExportParameters(false);
            }

            string[] algs = new string[] {
                JwtConst.RS256, JwtConst.RS384, JwtConst.RS512, JwtConst.PS256, JwtConst.PS384, JwtConst.PS512 };

            foreach (string alg in algs)
            {
                string ro = JwsFactory.CreateFromRsaParameters(alg, priv).Create(RequestObjectTests.OidcPayload().ToString(Formatting.None));

                Assert.That(RequestObject.Verify(ro, op, out iss, JwsFactory.CreateFromRsaParameters(alg, pub)), Is.True, alg);
                Assert.That(iss, Is.EqualTo("client01"), alg);
                Assert.That(RequestObject.Verify(ro, op, out iss, JwsFactory.CreateFromRsaParameters(alg, otherPub)), Is.False, alg + "：別の鍵");
                Assert.That(RequestObject.Verify(ro, "https://other.example.com/", out iss, JwsFactory.CreateFromRsaParameters(alg, pub)),
                    Is.False, alg + "：aud が違う");

                foreach (string another in algs)
                {
                    if (another != alg)
                    {
                        Assert.That(RequestObject.Verify(ro, op, out iss, JwsFactory.CreateFromRsaParameters(another, pub)),
                            Is.False, alg + " で署名したものを " + another + " で検証する");
                    }
                }
            }

            // JWK から作る（JwsFactory.CreateFromJwk）
            JObject jwk = JObject.Parse(new RsaPublicKeyConverter().ParamToJwk(pub));
            string ps256 = new JWS_PS256_Param(priv).Create(RequestObjectTests.OidcPayload().ToString(Formatting.None));
            Assert.That(RequestObject.Verify(ps256, op, out iss, JwsFactory.CreateFromJwk(JwtConst.PS256, jwk)), Is.True, "JWK から作った PS256");

            // RS256 で決め打ちの版は、PS256 を通さない（従来どおり）
            Assert.That(RequestObject.Verify(ps256, op, out iss, pub), Is.False, "RSAParameters の版（RS256）");

            // クレームの確認は、ほかの版と同じ
            JObject noClientId = RequestObjectTests.OidcPayload();
            noClientId.Remove(OAuth2AndOIDCConst.client_id);
            Assert.That(RequestObject.Verify(new JWS_PS256_Param(priv).Create(noClientId.ToString(Formatting.None)), op, out iss,
                new JWS_PS256_Param(pub)), Is.False, "client_id が無い");

            foreach (string broken in JwtAssertionTests.BrokenJwts())
            {
                Assert.That(RequestObject.Verify(broken, op, out iss, new JWS_PS256_Param(pub)), Is.False, "形が壊れている : " + (broken ?? "null"));
            }

            Assert.Throws<ArgumentNullException>(() => RequestObject.Verify(ps256, op, out iss, (JWS)null), "jws が null");
            Assert.Throws<ArgumentNullException>(() => RequestObject.Verify(ps256, null, out iss, new JWS_PS256_Param(pub)), "期待する aud が null");
            Assert.Throws<ArgumentNullException>(() => RequestObject.Verify(ps256, "", out iss, new JWS_PS256_Param(pub)), "期待する aud が空");
        }

        /// <summary>
        /// ヘッダの alg を見ない検証器を渡されても、ヘッダの alg が検証器の alg と違えば false（大文字・小文字も区別する）。
        /// ヘッダの alg が無い・文字列でないものも false。
        /// </summary>
        [Test]
        [Basis("RFC 8725 3.1（呼び出し側が受ける alg を決め、ほかの alg は使わない）")]
        [Basis("RFC 7515 4.1.1（alg は大文字・小文字を区別する）")]
        [Basis("#664")]
        public void Jws_HeaderAlgMustMatch()
        {
            const string op = "https://op.example.com/";
            string iss;
            JWS anyAlg = new AnyAlgJws(JwtConst.PS256);
            JObject payload = RequestObjectTests.OidcPayload();

            Assert.That(RequestObject.Verify(RequestObjectTests.UnsignedJws(JwtConst.PS256, payload), op, out iss, anyAlg), Is.True,
                "alg が一致（検証器は署名を見ない）");
            Assert.That(RequestObject.Verify(RequestObjectTests.UnsignedJws(JwtConst.RS256, payload), op, out iss, anyAlg), Is.False, "alg が違う");
            Assert.That(RequestObject.Verify(RequestObjectTests.UnsignedJws("ps256", payload), op, out iss, anyAlg), Is.False, "大文字・小文字が違う");
            Assert.That(RequestObject.Verify(RequestObjectTests.UnsignedJws("none", payload), op, out iss, anyAlg), Is.False, "none");

            Func<string, string> b64 = x => CustomEncode.ToBase64UrlString(Encoding.UTF8.GetBytes(x));
            string body = "." + b64(payload.ToString(Formatting.None)) + ".c2ln";
            Assert.That(RequestObject.Verify(b64("{\"typ\":\"JWT\"}") + body, op, out iss, anyAlg), Is.False, "alg が無い");
            Assert.That(RequestObject.Verify(b64("{\"alg\":1}") + body, op, out iss, anyAlg), Is.False, "alg が数値");

            Assert.That(RequestObject.Verify(RequestObjectTests.UnsignedJws(JwtConst.PS256, payload), op, out iss, new AnyAlgJws("")),
                Is.False, "検証器の alg が空");
        }

        /// <summary>
        /// CIBA：渡した検証鍵の alg（ES256・ES384・PS256）で署名したものは通り、
        /// ヘッダの alg が鍵の alg と違うもの・クレームの足りないもの・aud の違うものは false。
        /// </summary>
        [Test]
        [Platform("Win")]
        [Basis("FAPI CIBA 7.6（FAPI 1.0 Advanced 8.6 に従う。JWS は PS256 か ES256 を使う）")]
        [Basis("RFC 8725 3.1（呼び出し側が受ける alg を決め、ほかの alg は使わない）")]
        [Basis("CIBA Core 7.1.1（署名した認証要求。aud は OP の Issuer Identifier）")]
        [Basis("#664")]
        public void Ciba_Jws()
        {
            const string op = "https://op.example.com/";
            string iss;
            string payload = RequestObjectTests.CibaPayload().ToString(Formatting.None);

            ECParameters priv384, pub384, pub256;
            using (ECDsa ec = ECDsa.Create(ECCurve.NamedCurves.nistP384))
            {
                priv384 = ec.ExportParameters(true);
                pub384 = ec.ExportParameters(false);
            }
            using (ECDsa ec = ECDsa.Create(ECCurve.NamedCurves.nistP256))
            {
                pub256 = ec.ExportParameters(false);
            }

            string es384 = JwsFactory.CreateFromEcParameters(JwtConst.ES384, priv384, true).Create(payload);
            Assert.That(RequestObject.VerifyCiba(es384, op, out iss, JwsFactory.CreateFromEcParameters(JwtConst.ES384, pub384)), Is.True, "ES384");
            Assert.That(iss, Is.EqualTo("client01"));
            Assert.That(RequestObject.VerifyCiba(es384, op, out iss, JwsFactory.CreateFromEcParameters(JwtConst.ES256, pub256)), Is.False,
                "ES384 で署名したものを ES256 で検証する");
            Assert.That(RequestObject.VerifyCiba(es384, op, out iss, pub256), Is.False, "ECParameters の版（ES256）");
            Assert.That(RequestObject.VerifyCiba(es384, "https://other.example.com/", out iss,
                JwsFactory.CreateFromEcParameters(JwtConst.ES384, pub384)), Is.False, "aud が違う");

            // ES256 の証明書（従来の版と同じ鍵）
            JWS_ES256_X509 x509 = new JWS_ES256_X509(TestCertificates.EcdsaPfx256, TestCertificates.Password);
            ECParameters cerPub = TestCertificates.LoadPfx(TestCertificates.EcdsaPfx256).GetECDsaPublicKey().ExportParameters(false);
            Assert.That(RequestObject.VerifyCiba(x509.Create(payload), op, out iss, new JWS_ES256_Param(cerPub, false)), Is.True, "ES256");

            // PS256（FAPI CIBA が認める、もう一方の alg）
            using (RSA rsa = RSA.Create(2048))
            {
                string ps256 = new JWS_PS256_Param(rsa.ExportParameters(true)).Create(payload);
                Assert.That(RequestObject.VerifyCiba(ps256, op, out iss, new JWS_PS256_Param(rsa.ExportParameters(false))), Is.True, "PS256");
                Assert.That(RequestObject.VerifyCiba(ps256, op, out iss, new JWS_RS256_Param(rsa.ExportParameters(false))), Is.False,
                    "PS256 で署名したものを RS256 で検証する");
            }

            // クレームの確認は、ほかの版と同じ
            JObject noJti = RequestObjectTests.CibaPayload();
            noJti.Remove(OAuth2AndOIDCConst.jti);
            Assert.That(RequestObject.VerifyCiba(JwsFactory.CreateFromEcParameters(JwtConst.ES384, priv384, true).Create(noJti.ToString(Formatting.None)),
                op, out iss, JwsFactory.CreateFromEcParameters(JwtConst.ES384, pub384)), Is.False, "jti が無い");

            Assert.Throws<ArgumentNullException>(() => RequestObject.VerifyCiba(es384, op, out iss, (JWS)null), "jws が null");
            Assert.Throws<ArgumentNullException>(() => RequestObject.VerifyCiba(es384, null, out iss,
                JwsFactory.CreateFromEcParameters(JwtConst.ES384, pub384)), "期待する aud が null");
        }

        #endregion
    }
}

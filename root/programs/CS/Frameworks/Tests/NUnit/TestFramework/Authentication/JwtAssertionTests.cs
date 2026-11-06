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
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//*  2026/10/30  玄人 幸道         kty の大文字小文字を区別することのテストを足した（#643）
//*  2026/11/05  玄人 幸道         小数の NumericDate（exp）のテストを足した（#647）
//*  2026/11/05  玄人 幸道         期待する iss・aud を受ける版と、Create の sub のテストを足した。受けない版の Obsolete の警告を抑えた（#654）
//*  2026/11/05  玄人 幸道         sub が iss と違っても通すこと（観測）・形の壊れた入力・scope の有無・ES256 の期限切れを足した（#654 のレビューの指摘 3・5）
//*  2026/11/05  玄人 幸道         aud を唯一の値に限ったことのテストを足した（#654 のレビューの指摘 1）
//*  2026/11/06  玄人 幸道         検証鍵（JWS）を受ける Verify のテストを追加（#664）
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security.Jwt;

// iss・aud を受けない Verify・VerifyByRsa・VerifyByECDsa は Obsolete（#654）だが、利用者が使い続ける版なので、振る舞いを確かめ続ける。
#pragma warning disable CS0618

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
        [Basis("RFC 7523 3 の 1・3・4（iss・aud を持つ。期限の過ぎた JWT を断る）")]
        [Basis("RFC 7523 3 の 9（署名が無い・正しくない JWT を断る）")]
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

        /// <summary>
        /// VerifyByRsa：exp が小数の数値でも読む。無い・読めない exp は、例外にせず false。
        /// </summary>
        [Test]
        [Basis("RFC 7519 2（NumericDate は JSON の数値で、整数でない値も表せる）")]
        [Basis("RFC 7523 3 の 4（exp を持ち、期限の過ぎた JWT を断る）")]
        [Basis("#647")]
        public void Rsa_ExpAsNumericDate()
        {
            JWS_RS256_Param signer = new JWS_RS256_Param();
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();

            Func<JToken, bool> verify = exp =>
            {
                JObject p = new JObject
                {
                    [OAuth2AndOIDCConst.iss] = Iss,
                    [OAuth2AndOIDCConst.aud] = Aud,
                    [OAuth2AndOIDCConst.scope] = Scopes
                };
                if (exp != null)
                {
                    p[OAuth2AndOIDCConst.exp] = exp;
                }

                string iss, aud, scopes; JObject jobj;
                return JwtAssertion.VerifyByRsa(signer.Create(p.ToString(Newtonsoft.Json.Formatting.None)),
                    out iss, out aud, out scopes, out jobj, signer.RsaPublicParameters);
            };

            Assert.That(verify(new JValue(now + 300.5)), Is.True, "小数（未来）");
            Assert.That(verify(new JValue((now + 300).ToString())), Is.True, "文字列（従来どおり）");
            Assert.That(verify(new JValue(now - 300.5)), Is.False, "小数（過去）");
            Assert.That(verify(null), Is.False, "無し（例外にしない）");
            Assert.That(verify(new JValue("abc")), Is.False, "読めない（例外にしない）");
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

        /// <summary>
        /// kty は大文字小文字を区別する（"rsa" は RSA ではない）。綴りの違う JWK は、作れず（空文字）、検証も通らない。
        /// 2026/10/30 まで、ToUpper で揃えてから比べていた（#643）。
        /// </summary>
        [Test]
        [Basis("RFC 7517 4.1（kty は大文字小文字を区別する）・RFC 7518 6.1（RSA の kty は \"RSA\"）")]
        [Basis("#643")]
        public void Kty_CaseSensitive()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                JObject privateJwk = JObject.Parse(new RsaPrivateKeyConverter().ParamToJwk(rsa.ExportParameters(true)));
                JObject publicJwk = JObject.Parse(new RsaPublicKeyConverter().ParamToJwk(rsa.ExportParameters(false)));
                string jwt = JwtAssertion.Create(Iss, Aud, TimeSpan.FromMinutes(5), Scopes, privateJwk.ToString());

                string iss, aud, scopes; JObject jobj;
                Assert.That(JwtAssertion.Verify(jwt, out iss, out aud, out scopes, out jobj, publicJwk.ToString()), Is.True, "元の綴り");

                privateJwk["kty"] = "rsa";
                publicJwk["kty"] = "rsa";
                Assert.That(JwtAssertion.Create(Iss, Aud, TimeSpan.FromMinutes(5), Scopes, privateJwk.ToString()), Is.Empty);
                Assert.That(JwtAssertion.Verify(jwt, out iss, out aud, out scopes, out jobj, publicJwk.ToString()), Is.False);
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
        [Basis("RFC 7518 3.4（ES256 は P-256 と SHA-256 の ECDSA）")]
        [Basis("RFC 7523 3 の 9（署名が無い・正しくない JWT を断る）")]
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

        #region 期待する iss・aud を受ける版（#654）

        /// <summary>認可サーバの issuer（期待する aud の 1 つ目）</summary>
        private const string AsIssuer = "https://as.example.com/";

        /// <summary>必須のクレームだけを持つペイロード</summary>
        private static JObject Payload()
        {
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            return new JObject
            {
                [OAuth2AndOIDCConst.iss] = Iss,
                [OAuth2AndOIDCConst.sub] = Iss,
                [OAuth2AndOIDCConst.aud] = Aud,
                [OAuth2AndOIDCConst.exp] = now + 300,
            };
        }

        /// <summary>形の壊れた JWS（区切りが足りない・Base64url でない・ヘッダが JSON のオブジェクトでない・ヘッダの値の型が違う）</summary>
        internal static string[] BrokenJwts()
        {
            Func<string, string> b64 = x => Touryo.Infrastructure.Public.Str.CustomEncode.ToBase64UrlString(System.Text.Encoding.UTF8.GetBytes(x));
            return new string[]
            {
                null, "", "a", "a.b", "a.b.c.d",
                "!!!.e30.c2ln",
                b64("null") + ".e30.c2ln",
                b64("[1]") + ".e30.c2ln",
                b64("{\"alg\":{}}") + ".e30.c2ln",
                b64("{\"alg\":\"RS256\"}") + ".e30.!!!",
                b64("{\"alg\":\"ES256\"}") + ".e30.!!!",
            };
        }

        /// <summary>
        /// sub が iss と違っても通す（sub と client_id の照合は、呼び出し側が out sub で行う）。
        /// </summary>
        [Test]
        [Observation("RFC 7523 3 の 2 は、クライアント認証なら sub を client_id とするが、認可グラントなら利用者になる。部品は用途を知らないので照合しない（設計の約束として remarks に書いたが、変わったら気付くために残す）")]
        public void Rsa_SubDiffersFromIss()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                JObject p = JwtAssertionTests.Payload();
                p[OAuth2AndOIDCConst.sub] = "user01";
                string jwt = new JWS_RS256_Param(rsa.ExportParameters(true)).Create(p.ToString(Newtonsoft.Json.Formatting.None));

                string sub, scopes; JObject jobj;
                Assert.That(JwtAssertion.VerifyByRsa(jwt, Iss, new string[] { Aud }, out sub, out scopes, out jobj, rsa.ExportParameters(false)), Is.True);
                Assert.That(sub, Is.EqualTo("user01"), "呼び出し側が照合できるよう、sub を返す");
            }
        }

        /// <summary>Create は sub（= iss）を入れる（RSAParameters・JWK）。</summary>
        [Test]
        [Basis("RFC 7523 3 の 2（sub を持つ）・2 の B（クライアント認証では client_id）")]
        [Basis("#654")]
        public void Create_HasSub()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                Func<string, JObject> payloadOf = jwt => JObject.Parse(System.Text.Encoding.UTF8.GetString(
                    Touryo.Infrastructure.Public.Str.CustomEncode.FromBase64UrlString(jwt.Split('.')[1])));

                Assert.That((string)payloadOf(JwtAssertion.CreateByRsa(Iss, Aud, TimeSpan.FromMinutes(5), Scopes,
                    rsa.ExportParameters(true)))[OAuth2AndOIDCConst.sub], Is.EqualTo(Iss), "RSAParameters");
                Assert.That((string)payloadOf(JwtAssertion.Create(Iss, Aud, TimeSpan.FromMinutes(5), Scopes,
                    new RsaPrivateKeyConverter().ParamToJwk(rsa.ExportParameters(true))))[OAuth2AndOIDCConst.sub], Is.EqualTo(Iss), "JWK");
            }
        }

        /// <summary>
        /// RSA：期待する iss・aud を受ける版は、iss が一致し、sub が在り、aud が唯一の値で期待する値のいずれかと一致し、exp が過ぎていない
        /// （nbf は在れば、その時刻以後）ものだけを通す。欠けている・違う・型の違う値は false。
        /// </summary>
        [Test]
        [Basis("RFC 7523 3 の 1〜5（iss・sub・aud・exp を持つ。aud が自分を示さなければ断る。exp を過ぎたら・nbf より前なら断る）")]
        [Basis("RFC 7519 4.1.3（aud は StringOrURI か、その配列）")]
        [Basis("draft-ietf-oauth-rfc7523bis-11 4（aud は issuer を唯一の値とし、ほかの値を含むものは断る。audience injection の対策）")]
        [Basis("#654")]
        public void Rsa_ExpectedIssAud()
        {
            string[] auds = new string[] { AsIssuer, Aud };
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();

            using (RSA rsa = RSA.Create(2048))
            using (RSA other = RSA.Create(2048))
            {
                JWS_RS256_Param signer = new JWS_RS256_Param(rsa.ExportParameters(true));
                RSAParameters pub = rsa.ExportParameters(false);

                Func<Action<JObject>, string> sign = m =>
                {
                    JObject p = JwtAssertionTests.Payload();
                    m(p);
                    return signer.Create(p.ToString(Newtonsoft.Json.Formatting.None));
                };

                string sub, scopes; JObject jobj;

                // 通るもの
                string created = JwtAssertion.CreateByRsa(Iss, Aud, TimeSpan.FromMinutes(5), Scopes, rsa.ExportParameters(true));
                Assert.That(JwtAssertion.VerifyByRsa(created, Iss, auds, out sub, out scopes, out jobj, pub), Is.True, "Create で作ったもの");
                Assert.That(sub, Is.EqualTo(Iss));
                Assert.That(scopes, Is.EqualTo(Scopes));
                Assert.That(jobj, Is.Not.Null);

                var valid = new[]
                {
                    new { Label = "必須だけ（scope は無い）", Apply = (Action<JObject>)(p => { }) },
                    new { Label = "aud が issuer（集合の 1 つ目）", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.aud] = AsIssuer) },
                    new { Label = "aud が要素 1 つの配列", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.aud] = new JArray(Aud)) },
                    new { Label = "exp が小数", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.exp] = now + 300.5) },
                    new { Label = "exp が文字列（CreateExpClaim の形）", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.exp] = (now + 300).ToString()) },
                    new { Label = "nbf が過去", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.nbf] = now - 60) },
                    new { Label = "sub が日付の形（日付に変えずに読む）", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.sub] = "2026-11-05T00:00:00Z") },
                };

                foreach (var m in valid)
                {
                    Assert.That(JwtAssertion.VerifyByRsa(sign(m.Apply), Iss, auds, out sub, out scopes, out jobj, pub), Is.True, m.Label);
                }

                Assert.That(JwtAssertion.VerifyByRsa(sign(p => { }), Iss, auds, out sub, out scopes, out jobj, pub), Is.True);
                Assert.That(scopes, Is.Empty, "scope が無ければ空文字");
                Assert.That(JwtAssertion.VerifyByRsa(sign(p => p[OAuth2AndOIDCConst.scope] = Scopes), Iss, auds, out sub, out scopes, out jobj, pub), Is.True);
                Assert.That(scopes, Is.EqualTo(Scopes), "scope が在れば、その値");

                // 断るもの
                var invalid = new[]
                {
                    new { Label = "iss が違う", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.iss] = "client02") },
                    new { Label = "iss が無い", Apply = (Action<JObject>)(p => p.Remove(OAuth2AndOIDCConst.iss)) },
                    new { Label = "iss が数値", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.iss] = 1) },
                    new { Label = "sub が無い", Apply = (Action<JObject>)(p => p.Remove(OAuth2AndOIDCConst.sub)) },
                    new { Label = "sub が空", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.sub] = "") },
                    new { Label = "sub が数値", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.sub] = 1) },
                    new { Label = "aud が無い", Apply = (Action<JObject>)(p => p.Remove(OAuth2AndOIDCConst.aud)) },
                    new { Label = "aud が違う", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.aud] = "https://other.example.com/token") },
                    new { Label = "aud の大文字・小文字が違う", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.aud] = Aud.ToUpperInvariant()) },
                    new { Label = "aud の末尾のスラッシュが違う（正規化しない）", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.aud] = "https://as.example.com") },
                    new { Label = "aud の配列が含まない", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.aud] = new JArray("a", "b")) },
                    new { Label = "aud の配列に、期待する値とほかの値（唯一の値でない）", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.aud] = new JArray("other", Aud)) },
                    new { Label = "aud の配列に、期待する値が 2 つ（唯一の値でない）", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.aud] = new JArray(AsIssuer, Aud)) },
                    new { Label = "aud が空の配列", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.aud] = new JArray()) },
                    new { Label = "aud の配列に文字列でない要素", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.aud] = new JArray(Aud, 1)) },
                    new { Label = "exp が無い", Apply = (Action<JObject>)(p => p.Remove(OAuth2AndOIDCConst.exp)) },
                    new { Label = "exp が過去", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.exp] = now - 60) },
                    new { Label = "exp が読めない", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.exp] = "abc") },
                    new { Label = "nbf が未来", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.nbf] = now + 300) },
                    new { Label = "nbf が読めない", Apply = (Action<JObject>)(p => p[OAuth2AndOIDCConst.nbf] = "abc") },
                };

                foreach (var m in invalid)
                {
                    Assert.That(JwtAssertion.VerifyByRsa(sign(m.Apply), Iss, auds, out sub, out scopes, out jobj, pub), Is.False, m.Label);
                    Assert.That(jobj, Is.Null, m.Label + "（jobj は返さない）");
                }

                Assert.That(JwtAssertion.VerifyByRsa(signer.Create("[1]"), Iss, auds, out sub, out scopes, out jobj, pub), Is.False, "ペイロードが配列");

                // 形の壊れた入力は、例外にせず false（#654 のレビューの指摘 5）
                foreach (string broken in JwtAssertionTests.BrokenJwts())
                {
                    Assert.That(JwtAssertion.VerifyByRsa(broken, Iss, auds, out sub, out scopes, out jobj, pub), Is.False, "形が壊れている : " + (broken ?? "null"));
                }
                Assert.That(JwtAssertion.VerifyByRsa(created, Iss, auds, out sub, out scopes, out jobj, other.ExportParameters(false)), Is.False, "別の鍵");

                // JWK の版
                string publicJwk = new RsaPublicKeyConverter().ParamToJwk(pub);
                Assert.That(JwtAssertion.Verify(created, Iss, auds, out sub, out scopes, out jobj, publicJwk), Is.True, "JWK の版");
                Assert.That(JwtAssertion.Verify(created, Iss, new string[] { AsIssuer }, out sub, out scopes, out jobj, publicJwk), Is.False, "JWK の版（aud が違う）");

                // 受けない版は、iss・aud を照合しない
                string iss, aud;
                Assert.That(JwtAssertion.VerifyByRsa(sign(p => p[OAuth2AndOIDCConst.aud] = "https://other.example.com/token"),
                    out iss, out aud, out scopes, out jobj, pub), Is.True, "受けない版は aud を照合しない");

                // 引数
                Assert.Throws<ArgumentNullException>(() => JwtAssertion.VerifyByRsa(created, null, auds, out sub, out scopes, out jobj, pub), "iss が null");
                Assert.Throws<ArgumentNullException>(() => JwtAssertion.VerifyByRsa(created, "", auds, out sub, out scopes, out jobj, pub), "iss が空");
                Assert.Throws<ArgumentNullException>(() => JwtAssertion.VerifyByRsa(created, Iss, null, out sub, out scopes, out jobj, pub), "auds が null");
                Assert.Throws<ArgumentNullException>(() => JwtAssertion.VerifyByRsa(created, Iss, new string[0], out sub, out scopes, out jobj, pub), "auds が空");
                Assert.Throws<ArgumentNullException>(() => JwtAssertion.VerifyByRsa(created, Iss, new string[] { "", null }, out sub, out scopes, out jobj, pub), "auds が空の値だけ");
                Assert.Throws<ArgumentNullException>(() => JwtAssertion.Verify(created, Iss, null, out sub, out scopes, out jobj, publicJwk), "JWK の版も auds が null");
            }
        }

        /// <summary>ECDSA（ES256）：期待する iss・aud を受ける版で、ECParameters・JWK・証明書（*.pfx）で作ったものを検証できる。aud が違えば false。</summary>
        [Test]
        [Platform("Win")]
        [Basis("RFC 7523 3 の 1〜4（iss・sub・aud・exp を持つ。aud が自分を示さなければ断る）")]
        [Basis("#654")]
        public void Ecdsa_ExpectedIssAud()
        {
            string[] auds = new string[] { AsIssuer, Aud };

            ECParameters priv, pub;
            using (ECDsa ec = ECDsa.Create(ECCurve.NamedCurves.nistP256))
            {
                priv = ec.ExportParameters(true);
                pub = ec.ExportParameters(false);
            }

            string sub, scopes; JObject jobj;

            string byParam = JwtAssertion.CreateByECDsa(Iss, Aud, TimeSpan.FromMinutes(5), Scopes, priv);
            Assert.That(JwtAssertion.VerifyByECDsa(byParam, Iss, auds, out sub, out scopes, out jobj, pub), Is.True, "ECParameters");
            Assert.That(sub, Is.EqualTo(Iss), "Create は sub を入れる");
            Assert.That(JwtAssertion.VerifyByECDsa(byParam, Iss, new string[] { AsIssuer }, out sub, out scopes, out jobj, pub), Is.False, "aud が違う");
            Assert.That(JwtAssertion.VerifyByECDsa(byParam, "client02", auds, out sub, out scopes, out jobj, pub), Is.False, "iss が違う");
            Assert.That(JwtAssertion.VerifyByECDsa(JwtAssertion.CreateByECDsa(Iss, Aud, TimeSpan.FromMinutes(-5), Scopes, priv),
                Iss, auds, out sub, out scopes, out jobj, pub), Is.False, "期限切れ");

            foreach (string broken in JwtAssertionTests.BrokenJwts())
            {
                Assert.That(JwtAssertion.VerifyByECDsa(broken, Iss, auds, out sub, out scopes, out jobj, pub), Is.False, "形が壊れている : " + (broken ?? "null"));
            }

            string byJwk = JwtAssertion.Create(Iss, Aud, TimeSpan.FromMinutes(5), Scopes, new EccPrivateKeyConverter().ParamToJwk(priv));
            Assert.That(JwtAssertion.Verify(byJwk, Iss, auds, out sub, out scopes, out jobj, new EccPublicKeyConverter().ParamToJwk(pub)), Is.True, "JWK");

            string byX509 = JwtAssertion.CreateByECDsa(Iss, Aud, TimeSpan.FromMinutes(5), Scopes,
                TestCertificates.EcdsaPfx256, TestCertificates.Password);
            ECParameters cerPub = TestCertificates.LoadPfx(TestCertificates.EcdsaPfx256).GetECDsaPublicKey().ExportParameters(false);
            Assert.That(JwtAssertion.VerifyByECDsa(byX509, Iss, auds, out sub, out scopes, out jobj, cerPub), Is.True, "証明書");
            Assert.That(sub, Is.EqualTo(Iss), "Create（証明書）は sub を入れる");
        }

        #endregion

        #region 検証鍵（JWS）を受ける版（#664）

        /// <summary>
        /// 渡した検証鍵の alg（PS256・RS256・HS256）で署名したものは通り、ヘッダの alg が鍵の alg と違うもの・
        /// 別の鍵・iss や aud の違うもの・期限切れは false。
        /// </summary>
        [Test]
        [Basis("FAPI 1.0 Advanced 8.6（JWS は PS256 か ES256 を使う）")]
        [Basis("RFC 8725 3.1（呼び出し側が受ける alg を決め、ほかの alg は使わない）")]
        [Basis("OIDC Core 9（client_secret_jwt は、client_secret を鍵にした HMAC）")]
        [Basis("RFC 7523 3 の 1〜4（iss・sub・aud・exp を持つ。aud が自分を示さなければ断る）")]
        [Basis("#664")]
        public void Jws_ExpectedIssAud()
        {
            string[] auds = new string[] { AsIssuer, Aud };
            string sub, scopes; JObject jobj;

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

            string payload = JwtAssertionTests.Payload().ToString(Newtonsoft.Json.Formatting.None);
            string ps256 = new JWS_PS256_Param(priv).Create(payload);

            Assert.That(JwtAssertion.Verify(ps256, Iss, auds, out sub, out scopes, out jobj, new JWS_PS256_Param(pub)), Is.True, "PS256");
            Assert.That(sub, Is.EqualTo(Iss));
            Assert.That(jobj, Is.Not.Null);
            Assert.That(JwtAssertion.Verify(ps256, Iss, auds, out sub, out scopes, out jobj,
                JwsFactory.CreateFromJwk(JwtConst.PS256, JObject.Parse(new RsaPublicKeyConverter().ParamToJwk(pub)))), Is.True, "JWK から作った PS256");
            Assert.That(JwtAssertion.Verify(ps256, Iss, auds, out sub, out scopes, out jobj, new JWS_RS256_Param(pub)), Is.False,
                "PS256 で署名したものを RS256 で検証する");
            Assert.That(jobj, Is.Null, "通らなければ jobj は null");
            Assert.That(JwtAssertion.Verify(ps256, Iss, auds, out sub, out scopes, out jobj, new JWS_PS384_Param(pub)), Is.False,
                "PS256 で署名したものを PS384 で検証する");
            Assert.That(JwtAssertion.Verify(ps256, Iss, auds, out sub, out scopes, out jobj, new JWS_PS256_Param(otherPub)), Is.False, "別の鍵");
            Assert.That(JwtAssertion.Verify(ps256, "client02", auds, out sub, out scopes, out jobj, new JWS_PS256_Param(pub)), Is.False, "iss が違う");
            Assert.That(JwtAssertion.Verify(ps256, Iss, new string[] { AsIssuer }, out sub, out scopes, out jobj, new JWS_PS256_Param(pub)),
                Is.False, "aud が違う");

            // JWK の版（kty で RS256 に決まる）は、PS256 を通さない（従来どおり）
            Assert.That(JwtAssertion.Verify(ps256, Iss, auds, out sub, out scopes, out jobj, new RsaPublicKeyConverter().ParamToJwk(pub)),
                Is.False, "JWK の版（RS256）");

            // RS256 も、渡せば通る
            string rs256 = new JWS_RS256_Param(priv).Create(payload);
            Assert.That(JwtAssertion.Verify(rs256, Iss, auds, out sub, out scopes, out jobj, new JWS_RS256_Param(pub)), Is.True, "RS256");
            Assert.That(JwtAssertion.Verify(rs256, Iss, auds, out sub, out scopes, out jobj, new JWS_PS256_Param(pub)), Is.False,
                "RS256 で署名したものを PS256 で検証する");

            // client_secret_jwt（HS256）
            byte[] secret = System.Text.Encoding.UTF8.GetBytes("client_secret_0123456789abcdef0123456789");
            string hs256 = new JWS_HS256(secret).Create(payload);
            Assert.That(JwtAssertion.Verify(hs256, Iss, auds, out sub, out scopes, out jobj, new JWS_HS256(secret)), Is.True, "HS256");
            Assert.That(JwtAssertion.Verify(hs256, Iss, auds, out sub, out scopes, out jobj,
                new JWS_HS256(System.Text.Encoding.UTF8.GetBytes("another_secret_0123456789abcdef012345"))), Is.False, "HS256：別の鍵");

            // 期限切れ
            JObject expired = JwtAssertionTests.Payload();
            expired[OAuth2AndOIDCConst.exp] = DateTimeOffset.Now.ToUnixTimeSeconds() - 60;
            Assert.That(JwtAssertion.Verify(new JWS_PS256_Param(priv).Create(expired.ToString(Newtonsoft.Json.Formatting.None)),
                Iss, auds, out sub, out scopes, out jobj, new JWS_PS256_Param(pub)), Is.False, "期限切れ");

            foreach (string broken in JwtAssertionTests.BrokenJwts())
            {
                Assert.That(JwtAssertion.Verify(broken, Iss, auds, out sub, out scopes, out jobj, new JWS_PS256_Param(pub)),
                    Is.False, "形が壊れている : " + (broken ?? "null"));
            }

            // ヘッダの alg を見ない検証器を渡されても、alg が違えば通さない
            JWS anyAlg = new RequestObjectTests.AnyAlgJws(JwtConst.PS256);
            Assert.That(JwtAssertion.Verify(RequestObjectTests.UnsignedJws(JwtConst.RS256, JwtAssertionTests.Payload()),
                Iss, auds, out sub, out scopes, out jobj, anyAlg), Is.False, "alg が違う（ヘッダの alg を見ない検証器）");

            Assert.Throws<ArgumentNullException>(() => JwtAssertion.Verify(ps256, Iss, auds, out sub, out scopes, out jobj, (JWS)null), "jws が null");
            Assert.Throws<ArgumentNullException>(() => JwtAssertion.Verify(ps256, null, auds, out sub, out scopes, out jobj, new JWS_PS256_Param(pub)),
                "期待する iss が null");
        }

        #endregion
    }
}

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
//* クラス名        ：DPoPProofTests
//* クラス日本語名  ：DPoPProof（RFC 9449 の証明）と、DPoP で送る口の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（#634）
//*  2026/10/22  玄人 幸道         htu の比較のテストを UriOperatorTests から移した（比較を DPoPProof の private へ移したため）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace TestFramework.Authentication
{
    /// <summary>DPoPProof（RFC 9449 の証明）と、OAuth2AndOIDCClient の DPoP で送る口の単体テスト</summary>
    [TestFixture]
    public class DPoPProofTests
    {
        private const string TokenEndpoint = "https://as.example.com/token";
        private const string UserInfoEndpoint = "https://rs.example.com/userinfo";

        /// <summary>鍵（秘密鍵の JWK と、その署名器・公開鍵）</summary>
        private class Key
        {
            public JObject Private;
            public JObject Public;
            public JWS Signer;
        }

        /// <summary>alg に合う鍵を作る</summary>
        private static Key CreateKey(string alg)
        {
            JObject priv;

            if (alg.StartsWith("ES", StringComparison.Ordinal))
            {
                using (ECDsa ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256))
                {
                    priv = JObject.Parse(new EccPrivateKeyConverter().ParamToJwk(ecdsa.ExportParameters(true)));
                }
            }
            else
            {
                using (RSA rsa = RSA.Create(2048))
                {
                    priv = JObject.Parse(new RsaPrivateKeyConverter().ParamToJwk(rsa.ExportParameters(true)));
                }
            }

            return new Key() { Private = priv, Public = JwkOperator.ToPublic(priv), Signer = JwsFactory.CreateFromPrivateJwk(alg, priv) };
        }

        /// <summary>任意のヘッダとペイロードで、証明もどきを作る（検証が断ることを確かめるため）</summary>
        private static string Craft(Key key, string typ, JToken jwk, Dictionary<string, object> payload)
        {
            key.Signer.JWSHeader.typ = typ;
            key.Signer.JWSHeader.jwk = jwk;

            try
            {
                return key.Signer.Create(JsonConvert.SerializeObject(payload));
            }
            finally
            {
                key.Signer.JWSHeader.typ = JwtConst.JWT;
                key.Signer.JWSHeader.jwk = null;
            }
        }

        /// <summary>正しいペイロード</summary>
        private static Dictionary<string, object> Payload(long iat)
        {
            return new Dictionary<string, object>
            {
                { "jti", Guid.NewGuid().ToString("N") }, { "htm", "POST" }, { "htu", TokenEndpoint }, { "iat", iat }
            };
        }

        #region 作る・検証する

        /// <summary>作った証明を検証でき、jkt は公開鍵の RFC 7638 の指紋になる（RS256 / ES256）。</summary>
        [TestCase("RS256")]
        [TestCase("ES256")]
        [TestCase("PS256")]
        public void CreateAndVerify(string alg)
        {
            Key key = CreateKey(alg);
            string proof = DPoPProof.Create(key.Signer, key.Public, "POST", TokenEndpoint + "?q=1#f");

            Assert.That(JwtReader.TryReadTyp(proof), Is.EqualTo("dpop+jwt"));
            Assert.That((string)JwtReader.TryReadPayload(proof)["htu"], Is.EqualTo(TokenEndpoint), "htu はクエリと素片を落とす");
            Assert.That(JwtReader.TryReadPayload(proof)["iat"].Type, Is.EqualTo(JTokenType.Integer), "iat は数値");
            Assert.That(JwtReader.TryReadHeader(proof)["jwk"]["d"], Is.Null, "秘密鍵を出さない");

            string jkt, err;
            JObject payload;
            Assert.That(DPoPProof.Verify(proof, "POST", TokenEndpoint, out jkt, out payload, out err), Is.True, err);
            Assert.That(jkt, Is.EqualTo(JwkOperator.Thumbprint(key.Public)));
            Assert.That(payload["jti"], Is.Not.Null);

            // 署名器のヘッダは元に戻る。
            Assert.That(key.Signer.JWSHeader.typ, Is.EqualTo(JwtConst.JWT));
            Assert.That(key.Signer.JWSHeader.jwk, Is.Null);
        }

        /// <summary>Create：秘密鍵の JWK をヘッダに入れようとすると ArgumentException。htu は絶対 URL。</summary>
        [Test]
        public void Create_RejectsBadArguments()
        {
            Key key = CreateKey("ES256");

            Assert.Throws<ArgumentException>(() => DPoPProof.Create(key.Signer, key.Private, "POST", TokenEndpoint));
            Assert.Throws<ArgumentException>(() => DPoPProof.Create(key.Signer, key.Public, "POST", "/token"));
            Assert.Throws<ArgumentNullException>(() => DPoPProof.Create(null, key.Public, "POST", TokenEndpoint));
            Assert.Throws<ArgumentNullException>(() => DPoPProof.Create(key.Signer, key.Public, "", TokenEndpoint));
        }

        /// <summary>Verify は、RFC 9449 4.3 の各項目で断る（例外にしない）。</summary>
        [Test]
        public void Verify_Rejects()
        {
            Key key = CreateKey("ES256");
            Key other = CreateKey("ES256");
            Key rsa = CreateKey("RS256");
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            string jkt, err;
            JObject payload;
            Func<string, string, string, bool> verify = (p, htm, htu) => DPoPProof.Verify(p, htm, htu, out jkt, out payload, out err);

            string ok = DPoPProof.Create(key.Signer, key.Public, "POST", TokenEndpoint);
            Assert.That(verify(ok, "GET", TokenEndpoint), Is.False, "htm");
            Assert.That(verify(ok, "POST", "https://as.example.com/other"), Is.False, "htu");
            Assert.That(verify(ok, "POST", TokenEndpoint + "/"), Is.True, "末尾の / は同じ");

            Assert.That(verify(Craft(key, "JWT", key.Public, Payload(now)), "POST", TokenEndpoint), Is.False, "typ");
            Assert.That(verify(Craft(key, "dpop+jwt", null, Payload(now)), "POST", TokenEndpoint), Is.False, "jwk が無い");
            Assert.That(verify(Craft(key, "dpop+jwt", key.Private, Payload(now)), "POST", TokenEndpoint), Is.False, "秘密鍵の jwk");
            Assert.That(verify(Craft(key, "dpop+jwt", other.Public, Payload(now)), "POST", TokenEndpoint), Is.False, "別の鍵の jwk（署名が合わない）");
            Assert.That(verify(Craft(key, "dpop+jwt", rsa.Public, Payload(now)), "POST", TokenEndpoint), Is.False, "alg と鍵の種類が違う");
            Assert.That(verify(Craft(key, "dpop+jwt", key.Public, Payload(now - 600)), "POST", TokenEndpoint), Is.False, "iat が古い");
            Assert.That(verify(Craft(key, "dpop+jwt", key.Public, Payload(now + 600)), "POST", TokenEndpoint), Is.False, "iat が未来");

            Dictionary<string, object> stringIat = Payload(now);
            stringIat["iat"] = now.ToString();
            Assert.That(verify(Craft(key, "dpop+jwt", key.Public, stringIat), "POST", TokenEndpoint), Is.False, "iat が文字列");

            Dictionary<string, object> noJti = Payload(now);
            noJti.Remove("jti");
            Assert.That(verify(Craft(key, "dpop+jwt", key.Public, noJti), "POST", TokenEndpoint), Is.False, "jti が無い");

            Assert.That(verify(null, "POST", TokenEndpoint), Is.False, "空");
            Assert.That(verify("a.b.c", "POST", TokenEndpoint), Is.False, "JWS でない");

            // 受ける alg を絞れる。
            Assert.That(DPoPProof.Verify(ok, "POST", TokenEndpoint, DPoPProof.DefaultPastWindow, DPoPProof.DefaultFutureWindow,
                new string[] { "RS256" }, out jkt, out payload, out err), Is.False, "algs に無い");
        }

        /// <summary>
        /// htu の比較（RFC 9449 4.3）：クエリ・素片・末尾の "/"・大文字小文字・既定のポートの違いは同じとみなす。
        /// 比較は private なので、Verify を通して確かめる（証明の htu と、期待する URL の両方を変える）。
        /// </summary>
        [TestCase("https://as.example.com/token", "https://as.example.com/token")]
        [TestCase("https://as.example.com/token?x=1#f", "https://as.example.com/token")]
        [TestCase("https://as.example.com/token/", "https://as.example.com/token")]
        [TestCase("HTTPS://AS.example.com/Token", "https://as.example.com/token")]
        [TestCase("https://as.example.com:443/token", "https://as.example.com/token")]
        [TestCase("https://as.example.com/token", "https://as.example.com/token?x=1")]
        public void Verify_Htu_Same(string proofHtu, string expectedHtu)
        {
            Assert.That(VerifyHtu(proofHtu, expectedHtu), Is.True);
        }

        /// <summary>htu の比較：パス・ホスト・ポート・スキームが違えば別。相対 URL・空は断る。</summary>
        [TestCase("https://as.example.com/token2", "https://as.example.com/token")]
        [TestCase("https://evil.example.com/token", "https://as.example.com/token")]
        [TestCase("https://as.example.com:8443/token", "https://as.example.com/token")]
        [TestCase("http://as.example.com/token", "https://as.example.com/token")]
        [TestCase("/token", "https://as.example.com/token")]
        [TestCase("", "https://as.example.com/token")]
        [TestCase("https://as.example.com/token", "/token")]
        [TestCase("https://as.example.com/token", null)]
        public void Verify_Htu_Different(string proofHtu, string expectedHtu)
        {
            Assert.That(VerifyHtu(proofHtu, expectedHtu), Is.False);
        }

        /// <summary>証明の htu を差し替えて作り、期待する URL で検証する</summary>
        private static bool VerifyHtu(string proofHtu, string expectedHtu)
        {
            Key key = CreateKey("ES256");
            Dictionary<string, object> p = Payload(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            p["htu"] = proofHtu;

            string jkt, err;
            JObject payload;
            return DPoPProof.Verify(Craft(key, JwtConst.DPOP_JWT, key.Public, p), "POST", expectedHtu, out jkt, out payload, out err);
        }

        /// <summary>
        /// VerifyForResource：ath が access_token の指紋と一致し、鍵が cnf.jkt と同じこと。
        /// ath は RFC 9449 7.1 の例と一致する。
        /// </summary>
        [Test]
        public void VerifyForResource()
        {
            const string token = "Kz~8mXK1EalYznwH-LC-1fBAo.4Ljp~zsPE_NeO.gxU";
            Assert.That(DPoPProof.CreateAth(token), Is.EqualTo("fUHyO2r2Z3DZ53EsNrWBb0xWXoaNy59IiKCAqksmQEo"));

            Key key = CreateKey("ES256");
            string jkt = JwkOperator.Thumbprint(key.Public);
            string proof = DPoPProof.Create(key.Signer, key.Public, "GET", UserInfoEndpoint, token, null);

            JObject payload;
            string err;
            Assert.That(DPoPProof.VerifyForResource(proof, "GET", UserInfoEndpoint, token, jkt, out payload, out err), Is.True, err);
            Assert.That(DPoPProof.VerifyForResource(proof, "GET", UserInfoEndpoint, token + "x", jkt, out payload, out err), Is.False, "別のトークン");
            Assert.That(DPoPProof.VerifyForResource(proof, "GET", UserInfoEndpoint, token, "other", out payload, out err), Is.False, "別の鍵");
            Assert.That(DPoPProof.VerifyForResource(null, "GET", UserInfoEndpoint, token, jkt, out payload, out err), Is.False, "証明が無い");

            string noAth = DPoPProof.Create(key.Signer, key.Public, "GET", UserInfoEndpoint);
            Assert.That(DPoPProof.VerifyForResource(noAth, "GET", UserInfoEndpoint, token, jkt, out payload, out err), Is.False, "ath が無い");
        }

        /// <summary>CreateProofId：jkt と jti の組で決まり、区切りがあるので境目の違う組は別の値になる。</summary>
        [Test]
        public void CreateProofId()
        {
            Assert.That(DPoPProof.CreateProofId("ab", "c"), Is.Not.EqualTo(DPoPProof.CreateProofId("a", "bc")));
            Assert.That(DPoPProof.CreateProofId("ab", "c"), Is.EqualTo(DPoPProof.CreateProofId("ab", "c")));
            Assert.That(DPoPProof.CreateProofId("", "c"), Is.Empty);
        }

        #endregion

        #region 送る（OAuth2AndOIDCClient）

        /// <summary>
        /// UserInfo：Authorization: DPoP と、ath を入れた証明を添える。
        /// use_dpop_nonce で断られたら、受け取った nonce を入れて 1 回だけ送り直す。
        /// </summary>
        [Test]
        public void SendWithDPoP_NonceRetry()
        {
            Key key = CreateKey("ES256");
            const string token = "at01";

            using (StubHttp http = new StubHttp((req, body) =>
            {
                string proof = req.Headers.GetValues("DPoP").First();
                string nonce = (string)JwtReader.TryReadPayload(proof)["nonce"];

                if (nonce != "n1")
                {
                    HttpResponseMessage challenge = StubHttp.Json(HttpStatusCode.Unauthorized, "");
                    challenge.Headers.Add("WWW-Authenticate", "DPoP error=\"use_dpop_nonce\"");
                    challenge.Headers.Add("DPoP-Nonce", "n1");
                    return challenge;
                }

                return StubHttp.Json(HttpStatusCode.OK, "{\"sub\":\"user01\"}");
            }))
            {
                string json = OAuth2AndOIDCClient.GetUserInfoAsync(new Uri(UserInfoEndpoint), token, key.Signer, key.Public).Result;

                Assert.That(json, Does.Contain("user01"));
                Assert.That(http.Requests.Count, Is.EqualTo(2), "1 回だけ送り直す");

                HttpRequestMessage last = http.Requests[1].Item1;
                Assert.That(last.Headers.Authorization.Scheme, Is.EqualTo("DPoP"));
                Assert.That(last.Headers.Authorization.Parameter, Is.EqualTo(token));

                JObject payload;
                string err;
                Assert.That(DPoPProof.VerifyForResource(last.Headers.GetValues("DPoP").First(), "GET", UserInfoEndpoint,
                    token, JwkOperator.Thumbprint(key.Public), out payload, out err), Is.True, err);
            }
        }

        /// <summary>Token：本文の use_dpop_nonce（400）でも送り直す。nonce を覚えて、次の要求に最初から入れる。</summary>
        [Test]
        public void TokenRequestWithDPoP_RemembersNonce()
        {
            Key key = CreateKey("RS256");
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" }, { "refresh_token", "rt" }, { "client_id", "spa01" }
            };

            using (StubHttp http = new StubHttp((req, b) =>
            {
                string nonce = (string)JwtReader.TryReadPayload(req.Headers.GetValues("DPoP").First())["nonce"];

                if (nonce != "n2")
                {
                    HttpResponseMessage err = StubHttp.Json(HttpStatusCode.BadRequest, "{\"error\":\"use_dpop_nonce\"}");
                    err.Headers.Add("DPoP-Nonce", "n2");
                    return err;
                }

                return StubHttp.Json(HttpStatusCode.OK, "{\"token_type\":\"DPoP\"}");
            }))
            {
                Uri uri = new Uri("https://as-nonce.example.com/token");
                Assert.That(OAuth2AndOIDCClient.TokenRequestWithDPoPAsync(uri, body, key.Signer, key.Public).Result, Does.Contain("DPoP"));
                Assert.That(http.Requests.Count, Is.EqualTo(2));
                Assert.That(http.Requests[0].Item2, Does.Contain("client_id=spa01"));
                Assert.That(body.Count, Is.EqualTo(3), "引数の Dictionary は変えない");

                OAuth2AndOIDCClient.TokenRequestWithDPoPAsync(uri, body, key.Signer, key.Public).Wait();
                Assert.That(http.Requests.Count, Is.EqualTo(3), "覚えた nonce で、1 回で通る");
            }
        }

        /// <summary>Token（クライアント認証あり）：client_secret_basic なら Authorization: Basic、DPoP ヘッダも付く。</summary>
        [Test]
        public void TokenRequestWithDPoP_ClientAuthentication()
        {
            Key key = CreateKey("ES256");

            using (StubHttp http = new StubHttp((req, b) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                OAuth2AndOIDCClient.TokenRequestWithDPoPAsync(new Uri(TokenEndpoint),
                    new Dictionary<string, string> { { "grant_type", "client_credentials" } },
                    "client01", "secret01", null, OAuth2AndOIDCEnum.AuthMethods.client_secret_basic,
                    key.Signer, key.Public).Wait();

                HttpRequestMessage req = http.Requests[0].Item1;
                Assert.That(req.Headers.Authorization.Scheme, Is.EqualTo("Basic"));
                Assert.That(req.Headers.Contains("DPoP"), Is.True);
            }
        }

        #endregion
    }
}

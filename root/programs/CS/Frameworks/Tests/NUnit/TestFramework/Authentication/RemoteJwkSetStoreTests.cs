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
//* クラス名        ：RemoteJwkSetStoreTests
//* クラス日本語名  ：RemoteJwkSetStore（相手の jwks_uri）と DCR のクライアント側の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（#634）
//*  2026/10/22  玄人 幸道         取れない・壊れた応答の扱いのテストを足した（カバレッジの拡充）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace TestFramework.Authentication
{
    /// <summary>RemoteJwkSetStore（相手の jwks_uri）と、OAuth2AndOIDCClient の DCR（RFC 7591 / 7592）の単体テスト</summary>
    /// <remarks>HTTP は StubHttp で差し替える（ネットワークに出ない）。</remarks>
    [TestFixture]
    public class RemoteJwkSetStoreTests
    {
        private const string Issuer = "https://op.example.com";
        private const string Discovery = "https://op.example.com/.well-known/openid-configuration";
        private const string JwksUri = "https://op.example.com/jwks";

        /// <summary>公開鍵（kid 付き）</summary>
        private JObject _jwk;

        /// <summary>前処理（キャッシュを空にし、鍵を作る）</summary>
        [SetUp]
        public void SetUp()
        {
            RemoteJwkSetStore.Clear();
            RemoteJwkSetStore.MinRefetchInterval = TimeSpan.FromMinutes(1);

            using (RSA rsa = RSA.Create(2048))
            {
                this._jwk = JObject.Parse(new RsaPublicKeyConverter().ParamToJwk(rsa.ExportParameters(false)));
            }
        }

        /// <summary>後処理</summary>
        [TearDown]
        public void TearDown()
        {
            RemoteJwkSetStore.Clear();
            RemoteJwkSetStore.MinRefetchInterval = TimeSpan.FromMinutes(1);
        }

        /// <summary>discovery と jwks_uri に答える</summary>
        private StubHttp Server(string issuer)
        {
            string jwks = new JObject(new JProperty("keys", new JArray(this._jwk))).ToString();

            return new StubHttp((req, body) =>
            {
                if (req.RequestUri.AbsoluteUri == Discovery)
                {
                    return StubHttp.Json(HttpStatusCode.OK,
                        new JObject(new JProperty("issuer", issuer), new JProperty("jwks_uri", JwksUri),
                        new JProperty("registration_endpoint", "https://op.example.com/register")).ToString());
                }

                if (req.RequestUri.AbsoluteUri == JwksUri)
                {
                    return StubHttp.Json(HttpStatusCode.OK, jwks);
                }

                return StubHttp.Json(HttpStatusCode.NotFound, "");
            });
        }

        /// <summary>JWK Set の URL から kid で引ける。2 回目はキャッシュから（取りに行かない）。</summary>
        [Test]
        public void GetJwkAsync_CachesByUrl()
        {
            string kid = (string)this._jwk["kid"];

            using (StubHttp http = this.Server(Issuer))
            {
                JObject jwk = RemoteJwkSetStore.GetJwkAsync(JwksUri, kid).Result;
                Assert.That((string)jwk["n"], Is.EqualTo((string)this._jwk["n"]));
                Assert.That(JwsFactory.CreateFromJwk("RS256", jwk), Is.Not.Null);

                RemoteJwkSetStore.GetJwkAsync(JwksUri, kid).Wait();
                Assert.That(http.Requests.Count, Is.EqualTo(1));

                // 無い kid は、下限の内側なら取りに行かない（kid を偽った要求で叩かせない）。
                Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, "unknown").Result, Is.Null);
                Assert.That(http.Requests.Count, Is.EqualTo(1));

                // 下限を過ぎていれば取り直す（鍵のロールオーバー）。
                RemoteJwkSetStore.MinRefetchInterval = TimeSpan.Zero;
                Thread.Sleep(20);
                Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, "unknown").Result, Is.Null);
                Assert.That(http.Requests.Count, Is.EqualTo(2));
            }
        }

        /// <summary>同期の経路：キャッシュだけを読む。kid が空なら上限まで順に返す。返すのは複製。</summary>
        [Test]
        public void PickJwksFromCache()
        {
            Assert.That(RemoteJwkSetStore.HasKeysInCache(JwksUri), Is.False);

            using (StubHttp http = this.Server(Issuer))
            {
                Assert.That(RemoteJwkSetStore.WarmUpAsync(JwksUri).Result, Is.True);
            }

            List<JObject> jwks = RemoteJwkSetStore.PickJwksFromCache(JwksUri, "", 5);
            Assert.That(jwks.Count, Is.EqualTo(1));

            jwks[0]["n"] = "changed";
            Assert.That((string)RemoteJwkSetStore.PickJwksFromCache(JwksUri, (string)this._jwk["kid"], 1)[0]["n"],
                Is.EqualTo((string)this._jwk["n"]), "キャッシュは書き換わらない");
        }

        /// <summary>背景の補充：待たずに戻り、後でキャッシュに載る。</summary>
        [Test]
        public void RefillInBackground()
        {
            using (StubHttp http = this.Server(Issuer))
            {
                RemoteJwkSetStore.RefillInBackground(JwksUri);

                for (int i = 0; i < 100 && !RemoteJwkSetStore.HasKeysInCache(JwksUri); i++)
                {
                    Thread.Sleep(50);
                }

                Assert.That(RemoteJwkSetStore.HasKeysInCache(JwksUri), Is.True);

                // 下限の内側では、もう取りに行かない。
                RemoteJwkSetStore.RefillInBackground(JwksUri);
                Thread.Sleep(100);
                Assert.That(http.Requests.Count, Is.EqualTo(1));
            }
        }

        /// <summary>discovery 経由：issuer が期待値と一致すれば引ける。違えば鍵を使わない。</summary>
        [Test]
        public void GetJwkByDiscoveryAsync_ChecksIssuer()
        {
            string kid = (string)this._jwk["kid"];

            using (StubHttp http = this.Server(Issuer))
            {
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer, kid).Result, Is.Not.Null);
            }

            RemoteJwkSetStore.Clear();

            using (StubHttp http = this.Server("https://evil.example.com"))
            {
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer, kid).Result, Is.Null);
                Assert.That(http.Requests.Count, Is.EqualTo(1), "jwks_uri へは行かない");
            }
        }

        /// <summary>URL の判定と、discovery の URL の組み立て。</summary>
        [Test]
        public void Urls()
        {
            Assert.That(RemoteJwkSetStore.IsUsableKeySource("https://op.example.com/jwks"), Is.True);
            Assert.That(RemoteJwkSetStore.IsUsableKeySource("http://op.example.com/jwks"), Is.False);
            Assert.That(RemoteJwkSetStore.IsUsableKeySource("http://localhost:5000/jwks"), Is.True);
            Assert.That(RemoteJwkSetStore.DiscoveryUrlOf("https://op.example.com:8443/authorize?x=1"),
                Is.EqualTo("https://op.example.com:8443/.well-known/openid-configuration"));
            Assert.That(RemoteJwkSetStore.DiscoveryUrlOf("not a url"), Is.Empty);
        }

        #region DCR

        /// <summary>
        /// DCR：registration_endpoint を Discovery から引き、登録・読む・直す・消す。
        /// トークンは Authorization: Bearer で、本文は application/json で送る。
        /// </summary>
        [Test]
        public void DynamicClientRegistration()
        {
            using (StubHttp http = new StubHttp((req, body) =>
            {
                if (req.RequestUri.AbsoluteUri == Discovery)
                {
                    return StubHttp.Json(HttpStatusCode.OK, "{\"registration_endpoint\":\"https://op.example.com/register\"}");
                }

                if (req.Method == HttpMethod.Delete)
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }

                return StubHttp.Json(req.Method == HttpMethod.Post ? HttpStatusCode.Created : HttpStatusCode.OK,
                    "{\"client_id\":\"c1\",\"registration_access_token\":\"rat\"," +
                    "\"registration_client_uri\":\"https://op.example.com/register/c1\"}");
            }))
            {
                string endpoint = OAuth2AndOIDCClient.GetRegistrationEndpointAsync(new Uri(Discovery)).Result;
                Assert.That(endpoint, Is.EqualTo("https://op.example.com/register"));

                JObject reg = JObject.Parse(OAuth2AndOIDCClient.RegisterClientAsync(
                    new Uri(endpoint), "{\"redirect_uris\":[\"https://rp/cb\"]}", "iat01").Result);
                Assert.That((string)reg[OAuth2AndOIDCConst.registration_access_token], Is.EqualTo("rat"));

                HttpRequestMessage post = http.Requests[1].Item1;
                Assert.That(post.Method, Is.EqualTo(HttpMethod.Post));
                Assert.That(post.Headers.Authorization.ToString(), Is.EqualTo("Bearer iat01"));
                Assert.That(post.Content.Headers.ContentType.MediaType, Is.EqualTo("application/json"));
                Assert.That(http.Requests[1].Item2, Does.Contain("redirect_uris"));

                Uri clientUri = new Uri((string)reg[OAuth2AndOIDCConst.registration_client_uri]);
                Assert.That(OAuth2AndOIDCClient.ReadClientRegistrationAsync(clientUri, "rat").Result, Does.Contain("c1"));
                Assert.That(OAuth2AndOIDCClient.UpdateClientRegistrationAsync(clientUri, "rat", "{\"client_id\":\"c1\"}").Result, Does.Contain("c1"));
                Assert.That(OAuth2AndOIDCClient.DeleteClientRegistrationAsync(clientUri, "rat").Result, Is.True);

                Assert.That(http.Requests[2].Item1.Method, Is.EqualTo(HttpMethod.Get));
                Assert.That(http.Requests[2].Item1.Headers.Authorization.ToString(), Is.EqualTo("Bearer rat"));
                Assert.That(http.Requests[3].Item1.Method, Is.EqualTo(HttpMethod.Put));
                Assert.That(http.Requests[4].Item1.Method, Is.EqualTo(HttpMethod.Delete));
            }
        }

        /// <summary>登録の口が広告されていなければ空文字。トークンが無ければ Authorization を付けない。</summary>
        [Test]
        public void DynamicClientRegistration_NoEndpointAndNoToken()
        {
            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                Assert.That(OAuth2AndOIDCClient.GetRegistrationEndpointAsync(new Uri(Discovery)).Result, Is.Empty);

                OAuth2AndOIDCClient.RegisterClientAsync(new Uri("https://op.example.com/register"), "{}", null).Wait();
                Assert.That(http.Requests[1].Item1.Headers.Authorization, Is.Null);
            }
        }

        #endregion

        #region 取れない・壊れた応答

        /// <summary>ResolveJwksUriAsync：discovery が取れない・JSON でない・配列なら空文字。issuer が合えば jwks_uri。</summary>
        [Test]
        public void ResolveJwksUriAsync_Unreadable()
        {
            foreach (string body in new string[] { "", "not json", "[]" })
            {
                using (StubHttp http = new StubHttp((req, b) => StubHttp.Json(HttpStatusCode.OK, body)))
                {
                    Assert.That(RemoteJwkSetStore.ResolveJwksUriAsync(Discovery, Issuer).Result, Is.Empty, "本文 : " + body);
                }
            }

            using (StubHttp http = new StubHttp((req, b) => { throw new HttpRequestException("down"); }))
            {
                Assert.That(RemoteJwkSetStore.ResolveJwksUriAsync(Discovery, Issuer).Result, Is.Empty, "通信の失敗");
            }

            using (StubHttp http = this.Server(Issuer))
            {
                Assert.That(RemoteJwkSetStore.ResolveJwksUriAsync(Discovery, Issuer).Result, Is.EqualTo(JwksUri));
            }

            Assert.That(RemoteJwkSetStore.ResolveJwksUriAsync("not a url", Issuer).Result, Is.Empty, "URL でない");
        }

        /// <summary>URL が空なら、何もせずに「無い」を返す（取りに行かない）。</summary>
        [Test]
        public void EmptyUrl()
        {
            using (StubHttp http = this.Server(Issuer))
            {
                Assert.That(RemoteJwkSetStore.WarmUpAsync("").Result, Is.False);
                Assert.That(RemoteJwkSetStore.PickJwksFromCache("", "", 5), Is.Empty);
                Assert.That(RemoteJwkSetStore.HasKeysInCache(""), Is.False);
                RemoteJwkSetStore.RefillInBackground("");
                RemoteJwkSetStore.RefillInBackground(null);

                Assert.That(http.Requests, Is.Empty);
            }
        }

        /// <summary>
        /// JWK Set に keys が無い・配列でない・要素がオブジェクトでないなら、その分は使わない。
        /// kid が空のときは、上限の本数まで順に返す。
        /// </summary>
        [Test]
        public void PickJwksFromCache_MalformedJwkSet()
        {
            JObject second = (JObject)this._jwk.DeepClone();
            second["kid"] = "second";

            foreach (string jwkSet in new string[] { "{}", "{\"keys\":\"x\"}" })
            {
                RemoteJwkSetStore.Clear();

                using (StubHttp http = new StubHttp((req, b) => StubHttp.Json(HttpStatusCode.OK, jwkSet)))
                {
                    Assert.That(RemoteJwkSetStore.WarmUpAsync(JwksUri).Result, Is.False, jwkSet);
                    Assert.That(RemoteJwkSetStore.PickJwksFromCache(JwksUri, "", 5), Is.Empty, jwkSet);
                }
            }

            RemoteJwkSetStore.Clear();
            string mixed = new JObject(new JProperty("keys", new JArray("not an object", this._jwk, second))).ToString();

            using (StubHttp http = new StubHttp((req, b) => StubHttp.Json(HttpStatusCode.OK, mixed)))
            {
                Assert.That(RemoteJwkSetStore.WarmUpAsync(JwksUri).Result, Is.True);
            }

            Assert.That(RemoteJwkSetStore.PickJwksFromCache(JwksUri, "", 5).Count, Is.EqualTo(2), "オブジェクトでない要素は飛ばす");
            Assert.That(RemoteJwkSetStore.PickJwksFromCache(JwksUri, "", 1).Count, Is.EqualTo(1), "上限の本数まで");
            Assert.That((string)RemoteJwkSetStore.PickJwksFromCache(JwksUri, "second", 5)[0]["kid"], Is.EqualTo("second"));
        }

        /// <summary>取り直しに失敗しても、前に取れた JWK Set を残す（相手の一時的な障害で、鍵を失わない）。</summary>
        [Test]
        public void FailedRefetch_KeepsPreviousJwkSet()
        {
            string kid = (string)this._jwk["kid"];

            using (StubHttp http = this.Server(Issuer))
            {
                Assert.That(RemoteJwkSetStore.WarmUpAsync(JwksUri).Result, Is.True);
            }

            using (StubHttp http = new StubHttp((req, b) => { throw new HttpRequestException("down"); }))
            {
                Assert.That(RemoteJwkSetStore.WarmUpAsync(JwksUri).Result, Is.True, "前の鍵が残る");
                Assert.That(RemoteJwkSetStore.PickJwksFromCache(JwksUri, kid, 1).Count, Is.EqualTo(1));
            }

            Assert.That(RemoteJwkSetStore.MinRefetchInterval, Is.EqualTo(TimeSpan.FromMinutes(1)));
        }

        #endregion
    }
}

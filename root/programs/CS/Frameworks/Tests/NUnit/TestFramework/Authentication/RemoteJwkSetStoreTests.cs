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
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//*  2026/11/20  玄人 幸道         HttpClient プロパティ（取得に使う HttpClient の差し替え）のテストを追加（#707）
//*  2026/11/20  玄人 幸道         取得の重なり（同じ URL の取得を 1 本にまとめる）のテストを追加（#686）
//*  2026/11/20  玄人 幸道         discovery のキャッシュを引くときの Issuer の照合のテストを追加（#715）
//*  2026/11/20  玄人 幸道         呼び出しごとの HttpClient（引数）のテストを追加（#718）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using TestCommon;

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
            RemoteJwkSetStore.HttpClient = null;

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
            RemoteJwkSetStore.HttpClient = null;
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
        [Basis("設計：RemoteJwkSetStore の remarks（URL を鍵にしたキャッシュ。同じ URL の取得は MinRefetchInterval に 1 回まで）")]
        [Basis("設計：RemoteJwkSetStore.GetJwkAsync(jwkSetUrl, kid) の remarks（無ければ、下限を過ぎていれば取り直す）")]
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
        [Basis("設計：RemoteJwkSetStore.PickJwksFromCache の remarks（取りに行かない。kid は任意なので本数に上限を置く）")]
        [Basis("設計：RemoteJwkSetStore の remarks（返す JWK は複製であり、キャッシュを書き換えない）")]
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
        [Basis("設計：RemoteJwkSetStore.RefillInBackground の remarks（待たない。下限の内側の URL は取りに行かない）")]
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
        [Basis("OIDC Discovery 4.3（issuer は、取得に使った Issuer と同じでなければならない）")]
        [Basis("設計：RemoteJwkSetStore.GetJwkByDiscoveryAsync の remarks（期待する Issuer を必ず受け取る）")]
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
        [Basis("OIDC Discovery 4（Issuer の後ろに /.well-known/openid-configuration を付ける）")]
        [Basis("設計：RemoteJwkSetStore.IsUsableKeySource の remarks（平文の http は鍵の出どころにしない）")]
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
        [Basis("RFC 7591 3.1（登録は application/json の POST）")]
        [Basis("RFC 7592 2.1・2.2・2.3（読むのは GET、直すのは PUT、消すのは DELETE。registration access token で認証する）")]
        [Basis("OIDC Discovery 3（registration_endpoint）")]
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
        [Basis("OIDC Discovery 4.3（issuer は、取得に使った Issuer と同じでなければならない）")]
        [Basis("設計：RemoteJwkSetStore の remarks（引けなければ空を返し、例外にしない）")]
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
        [Basis("設計：RemoteJwkSetStore の remarks（引けなければ空を返し、例外にしない）")]
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
        [Basis("RFC 7517 5（JWK Set は keys の配列を持つ）")]
        [Basis("設計：RemoteJwkSetStore.PickJwksFromCache の remarks（kid は任意なので本数に上限を置く）")]
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

        /// <summary>RemoteJwkSetStore.HttpClient 用の、要求を数えて JWK Set・discovery に答える HttpMessageHandler</summary>
        private sealed class CountingHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

            /// <summary>受けた要求の URL</summary>
            public List<string> Urls = new List<string>();

            public CountingHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
            {
                this._respond = respond;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                this.Urls.Add(request.RequestUri.AbsoluteUri);
                return Task.FromResult(this._respond(request));
            }
        }

        /// <summary>
        /// HttpClient を設定すると、jwks_uri・discovery の取得はそちらで行い、共有の HttpClient は使わない。
        /// null に戻せば共有に戻る。設定した HttpClient が失敗しても、共有には倒さない（絞った行き先を、共有で迂回しない）。#707
        /// </summary>
        [Test]
        [Basis("設計：RemoteJwkSetStore.HttpClient の remarks（null なら共有の HttpClient。設定すれば jwks_uri・discovery の取得はそちら）")]
        public void HttpClient_Dedicated()
        {
            string kid = (string)this._jwk["kid"];
            string jwks = new JObject(new JProperty("keys", new JArray(this._jwk))).ToString();

            CountingHandler dedicated = new CountingHandler(req =>
            {
                if (req.RequestUri.AbsoluteUri == Discovery)
                {
                    return StubHttp.Json(HttpStatusCode.OK,
                        new JObject(new JProperty("issuer", Issuer), new JProperty("jwks_uri", JwksUri)).ToString());
                }
                return StubHttp.Json(req.RequestUri.AbsoluteUri == JwksUri ? HttpStatusCode.OK : HttpStatusCode.NotFound, jwks);
            });

            // 共有の HttpClient は、使われたら分かるようにしておく
            using (StubHttp shared = this.Server(Issuer))
            {
                RemoteJwkSetStore.HttpClient = new HttpClient(dedicated, false);

                // jwks_uri
                Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, kid).Result, Is.Not.Null);
                Assert.That(dedicated.Urls, Is.EqualTo(new List<string> { JwksUri }));
                Assert.That(shared.Requests.Count, Is.EqualTo(0), "共有の HttpClient は使わない");

                // discovery（と、そこから引いた jwks_uri）
                RemoteJwkSetStore.Clear();
                dedicated.Urls.Clear();
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer, kid).Result, Is.Not.Null);
                Assert.That(dedicated.Urls, Is.EqualTo(new List<string> { Discovery, JwksUri }));
                Assert.That(shared.Requests.Count, Is.EqualTo(0));

                // 設定した HttpClient が失敗しても、共有には倒さない
                RemoteJwkSetStore.Clear();
                RemoteJwkSetStore.HttpClient = new HttpClient(new CountingHandler(
                    req => { throw new HttpRequestException("down"); }), false);
                Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, kid).Result, Is.Null);
                Assert.That(shared.Requests.Count, Is.EqualTo(0), "共有に倒さない");

                // null に戻せば、共有の HttpClient（既定）
                RemoteJwkSetStore.HttpClient = null;
                RemoteJwkSetStore.Clear();
                Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, kid).Result, Is.Not.Null);
                Assert.That(shared.Requests.Count, Is.EqualTo(1));
            }
        }

        /// <summary>取り直しに失敗しても、前に取れた JWK Set を残す（相手の一時的な障害で、鍵を失わない）。</summary>
        [Test]
        [Basis("設計：RemoteJwkSetStore.Store の remarks（取れなかったときは、前に取れた JWK Set を残す）")]
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

        #region 取得の重なり（#686）

        /// <summary>要求を数え、Gate が在れば、それが開くまで答えを返さない HttpMessageHandler（取得を「取得中」のまま止めるため）</summary>
        private sealed class GateHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;
            private int _count = 0;

            /// <summary>null でなければ、開く（完了する）まで答えない</summary>
            public volatile TaskCompletionSource<bool> Gate = null;

            /// <summary>受けた要求の数</summary>
            public int Count { get { return Volatile.Read(ref this._count); } }

            public GateHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
            {
                this._respond = respond;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Interlocked.Increment(ref this._count);

                TaskCompletionSource<bool> gate = this.Gate;

                if (gate != null)
                {
                    await gate.Task.ConfigureAwait(false);
                }

                return this._respond(request);
            }
        }

        /// <summary>要求が handler に届くまで待つ（最大 5 秒）</summary>
        private static void WaitUntilRequested(GateHandler handler, int count)
        {
            for (int i = 0; i < 500 && handler.Count < count; i++)
            {
                Thread.Sleep(10);
            }

            Assert.That(handler.Count, Is.GreaterThanOrEqualTo(count), "取得が始まらない");
        }

        /// <summary>JWK Set（鍵 1 本）に答える</summary>
        private GateHandler JwksHandler(Func<JObject> jwk)
        {
            return new GateHandler(req =>
                StubHttp.Json(HttpStatusCode.OK, new JObject(new JProperty("keys", new JArray(jwk()))).ToString()));
        }

        /// <summary>
        /// 鍵のロールオーバーの直後に、知らない kid の要求が同時に来ても、JWK Set の取得は 1 本で、全員が新しい鍵を引ける。
        /// 取得中は下限の判定が「取り直してよい」のままなので、重ねて取りに行きやすい所である。
        /// </summary>
        [Test]
        [Basis("#686（取得中の URL を重ねて取りに行かない。取得を 1 本にまとめ、待った要求も新しい鍵で通す）")]
        [Basis("設計：RemoteJwkSetStore の remarks（同じ URL の取得は、同時に 1 本までに抑える）")]
        public void GetJwkAsync_Concurrent_FetchesOnce()
        {
            JObject current = this._jwk;
            JObject rolled = (JObject)this._jwk.DeepClone();
            rolled["kid"] = "rolled";

            GateHandler handler = this.JwksHandler(() => current);
            RemoteJwkSetStore.HttpClient = new HttpClient(handler, false);

            // 旧い鍵を取っておき、下限を 0 にして、新しい鍵へ入れ替わった相手を作る
            Assert.That(RemoteJwkSetStore.WarmUpAsync(JwksUri).Result, Is.True);
            Assert.That(handler.Count, Is.EqualTo(1));

            RemoteJwkSetStore.MinRefetchInterval = TimeSpan.Zero;
            Thread.Sleep(20);
            current = rolled;
            handler.Gate = new TaskCompletionSource<bool>();

            const int N = 8;
            List<Task<JObject>> tasks = new List<Task<JObject>>();

            for (int i = 0; i < N; i++)
            {
                tasks.Add(Task.Run(() => RemoteJwkSetStore.GetJwkAsync(JwksUri, "rolled")));
            }

            WaitUntilRequested(handler, 2);
            Thread.Sleep(300); // 残りの要求が、取得中の URL に行き当たるまで
            handler.Gate.SetResult(true);

            Assert.That(Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(10)), Is.True, "待っている要求が戻らない");

            foreach (Task<JObject> task in tasks)
            {
                Assert.That(task.Result, Is.Not.Null, "待った要求も、新しい鍵を引ける");
                Assert.That((string)task.Result["kid"], Is.EqualTo("rolled"));
            }

            Assert.That(handler.Count, Is.EqualTo(2), "最初の 1 本 + ロールオーバーの 1 本だけ（重ねて取りに行かない）");
        }

        /// <summary>discovery 経由でも、同時の要求は discovery と jwks_uri を 1 回ずつしか取りに行かない。</summary>
        [Test]
        [Basis("#686（GetJwkByDiscoveryAsync も、取得中の URL を重ねて取りに行かない）")]
        public void GetJwkByDiscoveryAsync_Concurrent_FetchesOnce()
        {
            string kid = (string)this._jwk["kid"];
            string jwks = new JObject(new JProperty("keys", new JArray(this._jwk))).ToString();

            GateHandler handler = new GateHandler(req =>
            {
                if (req.RequestUri.AbsoluteUri == Discovery)
                {
                    return StubHttp.Json(HttpStatusCode.OK,
                        new JObject(new JProperty("issuer", Issuer), new JProperty("jwks_uri", JwksUri)).ToString());
                }

                return StubHttp.Json(req.RequestUri.AbsoluteUri == JwksUri ? HttpStatusCode.OK : HttpStatusCode.NotFound, jwks);
            });

            handler.Gate = new TaskCompletionSource<bool>();
            RemoteJwkSetStore.HttpClient = new HttpClient(handler, false);

            const int N = 8;
            List<Task<JObject>> tasks = new List<Task<JObject>>();

            for (int i = 0; i < N; i++)
            {
                tasks.Add(Task.Run(() => RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer, kid)));
            }

            WaitUntilRequested(handler, 1);
            Thread.Sleep(300);
            handler.Gate.SetResult(true);

            Assert.That(Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(10)), Is.True, "待っている要求が戻らない");

            foreach (Task<JObject> task in tasks)
            {
                Assert.That(task.Result, Is.Not.Null);
            }

            Assert.That(handler.Count, Is.EqualTo(2), "discovery 1 回 + jwks_uri 1 回");
        }

        /// <summary>背景の補充が取得中なら、待てる経路の要求はそれに乗る（重ねて取りに行かず、補充された鍵を引く）。</summary>
        [Test]
        [Basis("#686（RefillInBackground の取得にも乗る。取得中の URL は、どの口からも重ねて取りに行かない）")]
        public void GetJwkAsync_JoinsBackgroundRefill()
        {
            string kid = (string)this._jwk["kid"];
            GateHandler handler = this.JwksHandler(() => this._jwk);
            handler.Gate = new TaskCompletionSource<bool>();
            RemoteJwkSetStore.HttpClient = new HttpClient(handler, false);

            RemoteJwkSetStore.RefillInBackground(JwksUri);
            WaitUntilRequested(handler, 1);

            Task<JObject> waiting = Task.Run(() => RemoteJwkSetStore.GetJwkAsync(JwksUri, kid));
            Thread.Sleep(300);
            Assert.That(waiting.IsCompleted, Is.False, "取得の完了を待っている");

            // 取得中の URL への、続けての補充は何もしない
            RemoteJwkSetStore.RefillInBackground(JwksUri);

            handler.Gate.SetResult(true);

            Assert.That(waiting.Wait(TimeSpan.FromSeconds(10)), Is.True);
            Assert.That(waiting.Result, Is.Not.Null);
            Assert.That(handler.Count, Is.EqualTo(1));
        }

        /// <summary>取得が失敗しても、待っている要求は戻り（止まらず）、取得中の印は外れて、次は取り直せる。</summary>
        [Test]
        [Basis("#686（取得中の印は、失敗でも必ず外す。待っている要求を止めない）")]
        [Basis("設計：RemoteJwkSetStore の remarks（引けなければ null を返し、例外にしない）")]
        public void GetJwkAsync_Concurrent_FailedFetchDoesNotStall()
        {
            string kid = (string)this._jwk["kid"];
            bool fail = true;
            GateHandler handler = new GateHandler(req =>
            {
                if (fail) { throw new HttpRequestException("down"); }
                return StubHttp.Json(HttpStatusCode.OK, new JObject(new JProperty("keys", new JArray(this._jwk))).ToString());
            });

            handler.Gate = new TaskCompletionSource<bool>();
            RemoteJwkSetStore.HttpClient = new HttpClient(handler, false);

            List<Task<JObject>> tasks = new List<Task<JObject>>();

            for (int i = 0; i < 4; i++)
            {
                tasks.Add(Task.Run(() => RemoteJwkSetStore.GetJwkAsync(JwksUri, kid)));
            }

            WaitUntilRequested(handler, 1);
            Thread.Sleep(300);
            handler.Gate.SetResult(true);

            Assert.That(Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(10)), Is.True, "失敗でも、待っている要求は戻る");

            foreach (Task<JObject> task in tasks)
            {
                Assert.That(task.Result, Is.Null);
            }

            Assert.That(handler.Count, Is.EqualTo(1), "失敗した取得も 1 本");

            // 取得中の印が残っていないこと：下限を過ぎれば、また取りに行ける
            fail = false;
            RemoteJwkSetStore.MinRefetchInterval = TimeSpan.Zero;
            Thread.Sleep(20);
            Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, kid).Wait(TimeSpan.FromSeconds(10)), Is.True);
            Assert.That(handler.Count, Is.EqualTo(2));
            Assert.That(RemoteJwkSetStore.HasKeysInCache(JwksUri), Is.True);
        }


        /// <summary>WarmUpAsync は、取得中の取得には乗らず、自分で取りに行く（入れ替えた直後の鍵を、取りこぼさない）。</summary>
        [Test]
        [Basis("設計：RemoteJwkSetStore.WarmUpAsync の remarks（取得中かどうかは見ない。入れ替えた相手の新しい鍵を、その場で反映する）")]
        public void WarmUpAsync_DoesNotJoinRunningFetch()
        {
            JObject current = this._jwk;
            JObject rolled = (JObject)this._jwk.DeepClone();
            rolled["kid"] = "rolled";

            GateHandler handler = this.JwksHandler(() => current);
            RemoteJwkSetStore.HttpClient = new HttpClient(handler, false);

            handler.Gate = new TaskCompletionSource<bool>();
            RemoteJwkSetStore.RefillInBackground(JwksUri); // 入れ替え前の鍵を取りに行って、止まっている
            WaitUntilRequested(handler, 1);

            current = rolled; // 相手が鍵を入れ替えた
            Task<bool> warmUp = Task.Run(() => RemoteJwkSetStore.WarmUpAsync(JwksUri));
            WaitUntilRequested(handler, 2); // 乗らずに、自分で取りに行く

            handler.Gate.SetResult(true);
            Assert.That(warmUp.Wait(TimeSpan.FromSeconds(10)), Is.True);
            Assert.That(warmUp.Result, Is.True);
            Assert.That(handler.Count, Is.EqualTo(2));
        }

        /// <summary>discovery の発行者が違うときも、同時の要求は 1 回だけ取りに行き、待った要求は止まらずに null を得る。</summary>
        [Test]
        [Basis("OIDC Discovery 4.3（issuer は、取得に使った Issuer と同じでなければならない）")]
        [Basis("#686（取得が実を結ばなくても、待っている要求を止めない）")]
        public void GetJwkByDiscoveryAsync_Concurrent_IssuerMismatch()
        {
            string kid = (string)this._jwk["kid"];

            GateHandler handler = new GateHandler(req => StubHttp.Json(HttpStatusCode.OK,
                new JObject(new JProperty("issuer", "https://evil.example.com"), new JProperty("jwks_uri", JwksUri)).ToString()));
            handler.Gate = new TaskCompletionSource<bool>();
            RemoteJwkSetStore.HttpClient = new HttpClient(handler, false);

            List<Task<JObject>> tasks = new List<Task<JObject>>();

            for (int i = 0; i < 4; i++)
            {
                tasks.Add(Task.Run(() => RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer, kid)));
            }

            WaitUntilRequested(handler, 1);
            Thread.Sleep(300);
            handler.Gate.SetResult(true);

            Assert.That(Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(10)), Is.True, "待っている要求が戻らない");

            foreach (Task<JObject> task in tasks)
            {
                Assert.That(task.Result, Is.Null);
            }

            Assert.That(handler.Count, Is.EqualTo(1), "jwks_uri へは行かず、discovery は 1 回だけ");
        }

        #endregion

        #region discovery のキャッシュと Issuer（#715）

        /// <summary>
        /// 同じ discoveryUrl を別の expectedIssuer で呼んでも、キャッシュの鍵を返さない（2 回目以降も、Issuer を照合する）。
        /// 下限の内側なら、取りに行かない。同じ expectedIssuer なら、従来どおりキャッシュから引ける。
        /// </summary>
        [Test]
        [Basis("OIDC Discovery 4.3（issuer は、取得に使った Issuer と同じでなければならない）")]
        [Basis("#715（キャッシュから引くときも、expectedIssuer を照合する）")]
        public void GetJwkByDiscoveryAsync_CachedKey_ChecksIssuer()
        {
            string kid = (string)this._jwk["kid"];

            using (StubHttp http = this.Server(Issuer))
            {
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer, kid).Result, Is.Not.Null);
                Assert.That(http.Requests.Count, Is.EqualTo(2), "discovery と jwks_uri");

                // 別の Issuer：キャッシュの鍵を返さない。下限の内側なので、取りに行かない。
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, "https://other.example.com", kid).Result, Is.Null);
                Assert.That(http.Requests.Count, Is.EqualTo(2));

                // 同じ Issuer：キャッシュから引ける（取りに行かない）。
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer, kid).Result, Is.Not.Null);
                Assert.That(http.Requests.Count, Is.EqualTo(2));
            }
        }

        /// <summary>
        /// 別の expectedIssuer の呼び出しは、下限を過ぎていれば discovery を取り直し、そこで照合する。
        /// 照合が通れば（discovery の issuer が、その expectedIssuer なら）、その Issuer の鍵として引ける。
        /// </summary>
        [Test]
        [Basis("OIDC Discovery 4.3（issuer は、取得に使った Issuer と同じでなければならない）")]
        [Basis("#715（別の expectedIssuer は、取り直して照合する）")]
        public void GetJwkByDiscoveryAsync_OtherIssuer_RefetchesAndChecks()
        {
            string kid = (string)this._jwk["kid"];
            using (StubHttp http = this.Server(Issuer))
            {
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer, kid).Result, Is.Not.Null);
            }

            RemoteJwkSetStore.MinRefetchInterval = TimeSpan.Zero;
            Thread.Sleep(20);

            // 相手の issuer が変わった（別の発行者になった）。新しい Issuer の呼び出しは、キャッシュを使わず、
            // 取り直して照合が通り、引ける。
            using (StubHttp http = this.Server("https://new.example.com"))
            {
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, "https://new.example.com", kid).Result, Is.Not.Null);
                Assert.That(http.Requests.Count, Is.EqualTo(2), "discovery と jwks_uri を取り直した");

                // 古い Issuer の呼び出しは、いまのキャッシュ（新しい Issuer のもの）を使わず、取り直して照合し、断られる。
                Thread.Sleep(20);
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer, kid).Result, Is.Null);
                Assert.That(http.Requests.Count, Is.EqualTo(3), "discovery だけ（issuer が違うので jwks_uri へは行かない）");
            }
        }


        /// <summary>
        /// 取り直しに失敗しても、前に取れた JWK Set と、その Issuer を残す（別の Issuer の呼び出しが、失敗を機に鍵を使えるようにはならない）。
        /// Issuer の比較は、大文字小文字を区別する（OIDC Discovery 4.3 は、完全に同じ文字列）。
        /// </summary>
        [Test]
        [Basis("OIDC Discovery 4.3（issuer は、取得に使った Issuer と同じでなければならない）")]
        [Basis("#715（取得に失敗しても、前の Issuer を残す）")]
        public void GetJwkByDiscoveryAsync_FailedRefetch_KeepsIssuer()
        {
            string kid = (string)this._jwk["kid"];

            using (StubHttp http = this.Server(Issuer))
            {
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer, kid).Result, Is.Not.Null);
            }

            RemoteJwkSetStore.MinRefetchInterval = TimeSpan.Zero;
            Thread.Sleep(20);

            using (StubHttp http = new StubHttp((req, b) => { throw new HttpRequestException("down"); }))
            {
                // 取り直しに失敗しても、前の Issuer の鍵は引ける。別の Issuer には使わない。
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer, kid).Result, Is.Not.Null);

                Thread.Sleep(20);
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, "https://other.example.com", kid).Result, Is.Null);

                // 大文字小文字だけ違う Issuer は、別の Issuer である
                Thread.Sleep(20);
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer.ToUpperInvariant(), kid).Result, Is.Null);

                // 失敗の後も、前の Issuer の鍵は残っている
                Thread.Sleep(20);
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer, kid).Result, Is.Not.Null);
            }
        }

        #endregion

        #region 呼び出しごとの HttpClient（#718）

        /// <summary>discovery と jwks_uri に答える CountingHandler</summary>
        private CountingHandler OkHandler()
        {
            string jwks = new JObject(new JProperty("keys", new JArray(this._jwk))).ToString();

            return new CountingHandler(req =>
            {
                if (req.RequestUri.AbsoluteUri == Discovery)
                {
                    return StubHttp.Json(HttpStatusCode.OK,
                        new JObject(new JProperty("issuer", Issuer), new JProperty("jwks_uri", JwksUri)).ToString());
                }

                return StubHttp.Json(req.RequestUri.AbsoluteUri == JwksUri ? HttpStatusCode.OK : HttpStatusCode.NotFound, jwks);
            });
        }

        /// <summary>要求を断る（絞った HttpClient が、行き先を断るときの形）CountingHandler</summary>
        private CountingHandler RejectingHandler()
        {
            return new CountingHandler(req => { throw new HttpRequestException("rejected"); });
        }

        /// <summary>
        /// 引数の HttpClient が、HttpClient プロパティ・共有の HttpClient より先に使われる。
        /// 引数が null なら HttpClient プロパティ、それも null なら共有。口ごとに確かめる。
        /// </summary>
        [Test]
        [Basis("#718（取りに行く HttpClient は、引数 → HttpClient プロパティ → 共有の OAuth2AndOIDCClient.HttpClient の順に決める）")]
        [Basis("設計：RemoteJwkSetStore の remarks（httpClient 引数の各オーバーロード）")]
        public void HttpClientArgument_TakesPrecedence()
        {
            string kid = (string)this._jwk["kid"];
            CountingHandler arg = this.OkHandler();
            CountingHandler prop = this.OkHandler();

            using (StubHttp shared = this.Server(Issuer))
            {
                HttpClient argClient = new HttpClient(arg, false);
                RemoteJwkSetStore.HttpClient = new HttpClient(prop, false);

                // GetJwkAsync：引数が先
                Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, kid, argClient).Result, Is.Not.Null);
                Assert.That(arg.Urls, Is.EqualTo(new List<string> { JwksUri }));
                Assert.That(prop.Urls.Count, Is.EqualTo(0), "HttpClient プロパティは使わない");
                Assert.That(shared.Requests.Count, Is.EqualTo(0), "共有は使わない");

                // GetJwkByDiscoveryAsync：discovery と、そこから引いた jwks_uri の両方が、引数の HttpClient
                RemoteJwkSetStore.Clear();
                arg.Urls.Clear();
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(Discovery, Issuer, kid, argClient).Result, Is.Not.Null);
                Assert.That(arg.Urls, Is.EqualTo(new List<string> { Discovery, JwksUri }));
                Assert.That(prop.Urls.Count, Is.EqualTo(0));
                Assert.That(shared.Requests.Count, Is.EqualTo(0));

                // ResolveJwksUriAsync
                arg.Urls.Clear();
                Assert.That(RemoteJwkSetStore.ResolveJwksUriAsync(Discovery, Issuer, argClient).Result, Is.EqualTo(JwksUri));
                Assert.That(arg.Urls, Is.EqualTo(new List<string> { Discovery }));
                Assert.That(prop.Urls.Count, Is.EqualTo(0));

                // WarmUpAsync
                RemoteJwkSetStore.Clear();
                arg.Urls.Clear();
                Assert.That(RemoteJwkSetStore.WarmUpAsync(JwksUri, argClient).Result, Is.True);
                Assert.That(arg.Urls, Is.EqualTo(new List<string> { JwksUri }));
                Assert.That(prop.Urls.Count, Is.EqualTo(0));

                // RefillInBackground
                RemoteJwkSetStore.Clear();
                arg.Urls.Clear();
                RemoteJwkSetStore.RefillInBackground(JwksUri, argClient);

                for (int i = 0; i < 100 && !RemoteJwkSetStore.HasKeysInCache(JwksUri); i++)
                {
                    Thread.Sleep(50);
                }

                Assert.That(RemoteJwkSetStore.HasKeysInCache(JwksUri), Is.True);
                Assert.That(arg.Urls, Is.EqualTo(new List<string> { JwksUri }));
                Assert.That(prop.Urls.Count, Is.EqualTo(0));

                // 引数が null なら、HttpClient プロパティ
                RemoteJwkSetStore.Clear();
                Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, kid, null).Result, Is.Not.Null);
                Assert.That(prop.Urls, Is.EqualTo(new List<string> { JwksUri }));
                Assert.That(shared.Requests.Count, Is.EqualTo(0));

                // どちらも null なら、共有
                RemoteJwkSetStore.Clear();
                RemoteJwkSetStore.HttpClient = null;
                Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, kid, null).Result, Is.Not.Null);
                Assert.That(shared.Requests.Count, Is.EqualTo(1));
            }
        }

        /// <summary>
        /// 引数の HttpClient が失敗しても、HttpClient プロパティにも共有にも倒さない（絞った行き先を、別の口で迂回しない）。
        /// </summary>
        [Test]
        [Basis("設計：RemoteJwkSetStore.HttpClient の remarks（設定した HttpClient が失敗しても、共有には倒さない。#707）を、引数にも適用")]
        public void HttpClientArgument_FailureDoesNotFallBack()
        {
            string kid = (string)this._jwk["kid"];
            CountingHandler prop = this.OkHandler();

            using (StubHttp shared = this.Server(Issuer))
            {
                RemoteJwkSetStore.HttpClient = new HttpClient(prop, false);

                Assert.That(RemoteJwkSetStore.GetJwkAsync(
                    JwksUri, kid, new HttpClient(this.RejectingHandler(), false)).Result, Is.Null);
                Assert.That(prop.Urls.Count, Is.EqualTo(0), "HttpClient プロパティへ倒さない");
                Assert.That(shared.Requests.Count, Is.EqualTo(0), "共有へ倒さない");
            }
        }

        /// <summary>
        /// 取れた鍵のキャッシュは URL ごとに共有する。ある HttpClient で取れた鍵を、別の HttpClient の呼び出しも引ける
        /// （同じ URL なら同じ中身。取りに行かない）。
        /// </summary>
        [Test]
        [Basis("#718 案 (a)（控え＝取れた鍵は URL ごとに共有する）")]
        public void HttpClientArgument_CacheIsSharedByUrl()
        {
            string kid = (string)this._jwk["kid"];
            CountingHandler a = this.OkHandler();
            CountingHandler b = this.OkHandler();

            Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, kid, new HttpClient(a, false)).Result, Is.Not.Null);
            Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, kid, new HttpClient(b, false)).Result, Is.Not.Null,
                "別の HttpClient の呼び出しも、共有の控えから引ける");
            Assert.That(a.Urls.Count, Is.EqualTo(1));
            Assert.That(b.Urls.Count, Is.EqualTo(0), "控えから引けたので、取りに行かない");
        }

        /// <summary>
        /// 絞った HttpClient の取得が断られても、同じ URL を共有の HttpClient で取りに来た呼び出しを、下限（1 分）で止めない。
        /// 取り直しの下限は、（URL, HttpClient）の組ごとに数える。
        /// </summary>
        [Test]
        [Basis("#718 案 (a)（絞った HttpClient の取得が断られても、共有の HttpClient の呼び出しを巻き込まない）")]
        public void HttpClientArgument_RejectedFetchDoesNotBlockOtherClient()
        {
            string kid = (string)this._jwk["kid"];
            CountingHandler rejected = this.RejectingHandler();
            CountingHandler ok = this.OkHandler();

            HttpClient rejectedClient = new HttpClient(rejected, false);

            // 絞った HttpClient：断られる（取得の時刻は進む）
            Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, kid, rejectedClient).Result, Is.Null);
            Assert.That(rejected.Urls.Count, Is.EqualTo(1));

            // 同じ組は、下限の内側なので取りに行かない（失敗した相手を叩き続けない）
            Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, kid, rejectedClient).Result, Is.Null);
            Assert.That(rejected.Urls.Count, Is.EqualTo(1));

            // 別の HttpClient（共有側）は、下限の内側でも取りに行ける
            Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, kid, new HttpClient(ok, false)).Result, Is.Not.Null,
                "断られた取得に引きずられない");
            Assert.That(ok.Urls, Is.EqualTo(new List<string> { JwksUri }));
        }

        /// <summary>同じ HttpClient の組は、下限の内側なら取りに行かない（従来どおり。kid を偽った要求で叩かせない）。</summary>
        [Test]
        [Basis("設計：RemoteJwkSetStore の remarks（同じ URL の取得は MinRefetchInterval に 1 回まで。組ごとに数える）")]
        public void HttpClientArgument_SamePair_RespectsMinRefetchInterval()
        {
            string kid = (string)this._jwk["kid"];
            CountingHandler ok = this.OkHandler();
            HttpClient client = new HttpClient(ok, false);

            Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, kid, client).Result, Is.Not.Null);
            Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, "unknown", client).Result, Is.Null);
            Assert.That(ok.Urls.Count, Is.EqualTo(1), "同じ組は、下限の内側なので取りに行かない");

            RemoteJwkSetStore.MinRefetchInterval = TimeSpan.Zero;
            Thread.Sleep(20);
            Assert.That(RemoteJwkSetStore.GetJwkAsync(JwksUri, "unknown", client).Result, Is.Null);
            Assert.That(ok.Urls.Count, Is.EqualTo(2), "下限を過ぎれば取り直す");
        }

        /// <summary>
        /// 取得中のまとめは、（URL, HttpClient）の組ごと。ある HttpClient の取得が止まっている間も、
        /// 同じ URL を別の HttpClient で取りに来た呼び出しは、その完了を待たずに取りに行って戻る。
        /// 同じ組の呼び出しは、従来どおり 1 本にまとまる。
        /// </summary>
        [Test]
        [Basis("#718 案 (a)（取得中のまとめは、URL と HttpClient の組ごと。絞った HttpClient の取得が、共有の HttpClient の呼び出しを巻き込まない）")]
        [Basis("#686（同じ組の取得は、同時に 1 本にまとめる）")]
        public void HttpClientArgument_FetchingIsPerPair()
        {
            string kid = (string)this._jwk["kid"];
            GateHandler slow = this.JwksHandler(() => this._jwk);
            GateHandler fast = this.JwksHandler(() => this._jwk);
            HttpClient slowClient = new HttpClient(slow, false);
            HttpClient fastClient = new HttpClient(fast, false);

            slow.Gate = new TaskCompletionSource<bool>();

            // slow の取得を、取得中のまま止める。同じ組の 2 本目は、重ねて取りに行かない（#686）
            Task<JObject> slow1 = Task.Run(() => RemoteJwkSetStore.GetJwkAsync(JwksUri, kid, slowClient));
            WaitUntilRequested(slow, 1);
            Task<JObject> slow2 = Task.Run(() => RemoteJwkSetStore.GetJwkAsync(JwksUri, kid, slowClient));
            Thread.Sleep(300);
            Assert.That(slow.Count, Is.EqualTo(1), "同じ組は、重ねて取りに行かない");

            // 別の HttpClient は、slow の完了を待たずに取りに行って戻る
            Task<JObject> other = Task.Run(() => RemoteJwkSetStore.GetJwkAsync(JwksUri, kid, fastClient));
            Assert.That(other.Wait(TimeSpan.FromSeconds(5)), Is.True, "別の HttpClient の呼び出しが、止まっている取得を待っている");
            Assert.That(other.Result, Is.Not.Null);
            Assert.That(fast.Count, Is.EqualTo(1));

            slow.Gate.SetResult(true);
            Assert.That(Task.WaitAll(new Task[] { slow1, slow2 }, TimeSpan.FromSeconds(10)), Is.True);
            Assert.That(slow1.Result, Is.Not.Null);
            Assert.That(slow2.Result, Is.Not.Null);
            Assert.That(slow.Count, Is.EqualTo(1), "slow の取得は 1 本のまま");
        }

        #endregion
    }
}

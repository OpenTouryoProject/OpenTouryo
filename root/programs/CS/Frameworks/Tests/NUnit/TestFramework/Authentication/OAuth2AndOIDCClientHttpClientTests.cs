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
//* クラス名        ：OAuth2AndOIDCClientHttpClientTests
//* クラス日本語名  ：OAuth2AndOIDCClient・SAML2Metadata の、呼び出しごとの HttpClient（引数）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/11/20  玄人 幸道         新規作成（#719。ph1：OpenIDConnect の 4 口・SendAsync・SAML2Metadata.GetAsync）
//*  2026/11/20  玄人 幸道         ph2 の口（ClientRegistration・Grants・TokenManagement）を Cases に追加（#719）
//*  2026/11/20  玄人 幸道         ph3 の口（AuthZRequest・DPoP・ExtensionGrants）を Cases に追加（#719）
//*  2026/11/20  玄人 幸道         DPoP の nonce の送り直し・Device AuthZ の scope が null のテストを追加（#719 のレビュー）
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
    /// <summary>OAuth2AndOIDCClient（各口）と SAML2Metadata.GetAsync の、httpClient 引数の単体テスト</summary>
    /// <remarks>
    /// HttpClient を受ける口ごとに、同じ 3 つを確かめる。
    /// ・引数の HttpClient で送る（共有の HttpClient は使わない）
    /// ・引数が null なら、共有の HttpClient（OAuth2AndOIDCClient.HttpClient）で送る
    /// ・引数の HttpClient が失敗しても、共有には倒さない
    /// 口は <see cref="Cases"/> に足す（足した口が、3 つの確認に自動で入る）。
    /// </remarks>
    [TestFixture]
    public class OAuth2AndOIDCClientHttpClientTests
    {
        private static readonly Uri Endpoint = new Uri("https://op.example.com/endpoint");

        /// <summary>要求を数えて答える HttpMessageHandler（throwOnSend なら、通信の失敗を起こす）</summary>
        private sealed class CountingHandler : HttpMessageHandler
        {
            private readonly bool _throwOnSend;

            /// <summary>受けた要求の数</summary>
            private int _count = 0;

            public int Count { get { return Volatile.Read(ref this._count); } }

            public CountingHandler(bool throwOnSend)
            {
                this._throwOnSend = throwOnSend;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Interlocked.Increment(ref this._count);

                if (this._throwOnSend)
                {
                    throw new HttpRequestException("rejected");
                }

                return Task.FromResult(StubHttp.Json(HttpStatusCode.OK, "{}"));
            }
        }

        /// <summary>HttpClient を受ける口（名前と、その口を引数の HttpClient で呼ぶ処理）</summary>
        private static IEnumerable<TestCaseData> Cases()
        {
            yield return Case("SendAsync", c => OAuth2AndOIDCClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Get, Endpoint), c));
            yield return Case("GetUserInfoAsync", c => OAuth2AndOIDCClient.GetUserInfoAsync(Endpoint, "token", c));
            yield return Case("GetJwkSetAsync", c => OAuth2AndOIDCClient.GetJwkSetAsync(Endpoint, c));
            yield return Case("GetDiscoveryDocumentAsync", c => OAuth2AndOIDCClient.GetDiscoveryDocumentAsync(Endpoint, c));
            yield return Case("GetRegistrationEndpointAsync", c => OAuth2AndOIDCClient.GetRegistrationEndpointAsync(Endpoint, c));
            yield return Case("SAML2Metadata.GetAsync", c => SAML2Metadata.GetAsync(Endpoint.AbsoluteUri, "https://op.example.com", c));

            // ph2：ClientRegistration（DCR）
            yield return Case("RegisterClientAsync", c => OAuth2AndOIDCClient.RegisterClientAsync(Endpoint, "{}", "iat", c));
            yield return Case("ReadClientRegistrationAsync", c => OAuth2AndOIDCClient.ReadClientRegistrationAsync(Endpoint, "rat", c));
            yield return Case("UpdateClientRegistrationAsync", c => OAuth2AndOIDCClient.UpdateClientRegistrationAsync(Endpoint, "rat", "{}", c));
            yield return Case("DeleteClientRegistrationAsync", c => OAuth2AndOIDCClient.DeleteClientRegistrationAsync(Endpoint, "rat", c));

            // ph2：Grants
            yield return Case("GetAccessTokenByCodeAsync(secret)", c => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                Endpoint, "cid", "sec", "https://rp/cb", "code", Basic, c));
            yield return Case("GetAccessTokenByCodeAsync(PKCE)", c => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                Endpoint, "cid", "sec", "https://rp/cb", "code", "verifier", Post, c));
            yield return Case("GetAccessTokenByCodeAsync(assertion)", c => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                Endpoint, "https://rp/cb", "code", "assertion", PrivateKeyJwt, c));
            yield return Case("GetAccessTokenByCodeWithPkceAsync", c => OAuth2AndOIDCClient.GetAccessTokenByCodeWithPkceAsync(
                Endpoint, "https://rp/cb", "code", "verifier", "assertion", c));
            yield return Case("GetAccessTokenByCodeAsync(any method)", c => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                Endpoint, "cid", "sec", "assertion", "https://rp/cb", "code", "verifier", Post, c));
            yield return Case("ClientCredentialsGrantAsync", c => OAuth2AndOIDCClient.ClientCredentialsGrantAsync(
                Endpoint, "cid", "sec", "scope", c));
            yield return Case("ResourceOwnerPasswordCredentialsGrantAsync", c => OAuth2AndOIDCClient.ResourceOwnerPasswordCredentialsGrantAsync(
                Endpoint, "cid", "sec", "user", "pass", "scope", c));

            // ph2：TokenManagement
            yield return Case("UpdateAccessTokenByRefreshTokenAsync(secret)", c => OAuth2AndOIDCClient.UpdateAccessTokenByRefreshTokenAsync(
                Endpoint, "cid", "sec", "rt", c));
            yield return Case("UpdateAccessTokenByRefreshTokenAsync(any method)", c => OAuth2AndOIDCClient.UpdateAccessTokenByRefreshTokenAsync(
                Endpoint, "cid", "sec", "assertion", "rt", Post, c));
            yield return Case("RevokeTokenAsync(secret)", c => OAuth2AndOIDCClient.RevokeTokenAsync(
                Endpoint, "cid", "sec", "tok", "access_token", Basic, c));
            yield return Case("RevokeTokenAsync(any method)", c => OAuth2AndOIDCClient.RevokeTokenAsync(
                Endpoint, "cid", "sec", "assertion", "tok", "access_token", Post, c));
            yield return Case("IntrospectTokenAsync(secret)", c => OAuth2AndOIDCClient.IntrospectTokenAsync(
                Endpoint, "cid", "sec", "tok", "access_token", Basic, c));
            yield return Case("IntrospectTokenAsync(any method)", c => OAuth2AndOIDCClient.IntrospectTokenAsync(
                Endpoint, "cid", "sec", "assertion", "tok", "access_token", Post, c));

            // ph3：AuthZRequest（Request Object の預け先・PAR）
            yield return Case("RegisterRequestObjectAsync", c => OAuth2AndOIDCClient.RegisterRequestObjectAsync(Endpoint, "jwt", c));
            yield return Case("PushAuthorizationRequestAsync(string, secret)", c => OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                Endpoint, "jwt", "cid", "sec", Basic, c));
            yield return Case("PushAuthorizationRequestAsync(string, assertion)", c => OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                Endpoint, "jwt", "cid", "sec", "assertion", Post, c));
            yield return Case("PushAuthorizationRequestAsync(Dictionary, secret)", c => OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                Endpoint, new Dictionary<string, string> { { "scope", "openid" } }, "cid", "sec", Basic, c));
            yield return Case("PushAuthorizationRequestAsync(Dictionary, assertion)", c => OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                Endpoint, new Dictionary<string, string> { { "scope", "openid" } }, "cid", "sec", "assertion", Post, c));

            // ph3：DPoP
            yield return Case("SendWithDPoPAsync", c => OAuth2AndOIDCClient.SendWithDPoPAsync(
                () => new HttpRequestMessage(HttpMethod.Get, Endpoint), DPoPKey.Signer, DPoPKey.Public, null, c));
            yield return Case("TokenRequestWithDPoPAsync(public client)", c => OAuth2AndOIDCClient.TokenRequestWithDPoPAsync(
                Endpoint, new Dictionary<string, string> { { "grant_type", "authorization_code" } }, DPoPKey.Signer, DPoPKey.Public, c));
            yield return Case("TokenRequestWithDPoPAsync(client authentication)", c => OAuth2AndOIDCClient.TokenRequestWithDPoPAsync(
                Endpoint, new Dictionary<string, string> { { "grant_type", "authorization_code" } },
                "cid", "sec", "assertion", Post, DPoPKey.Signer, DPoPKey.Public, c));
            yield return Case("GetUserInfoAsync(DPoP)", c => OAuth2AndOIDCClient.GetUserInfoAsync(
                Endpoint, "tok", DPoPKey.Signer, DPoPKey.Public, c));

            // ph3：ExtensionGrants（JWT Bearer・Device AuthZ・CIBA）
            yield return Case("JwtBearerTokenFlowAsync", c => OAuth2AndOIDCClient.JwtBearerTokenFlowAsync(Endpoint, "assertion", c));
            yield return Case("DeviceAuthZRequestAsync(scope)", c => OAuth2AndOIDCClient.DeviceAuthZRequestAsync(
                Endpoint, "cid", "scope", c));
            yield return Case("GetAccessTokenByDeviceAuthZAsync", c => OAuth2AndOIDCClient.GetAccessTokenByDeviceAuthZAsync(
                Endpoint, "cid", "device_code", c));
            yield return Case("CibaAuthZRequestAsync(request_uri)", c => OAuth2AndOIDCClient.CibaAuthZRequestAsync(
                Endpoint, "https://rp.example.com/request.jwt", c));
            yield return Case("CibaAuthZRequestAsync(secret)", c => OAuth2AndOIDCClient.CibaAuthZRequestAsync(
                Endpoint, "jwt", "cid", "sec", Basic, c));
            yield return Case("CibaAuthZRequestAsync(any method)", c => OAuth2AndOIDCClient.CibaAuthZRequestAsync(
                Endpoint, "jwt", "cid", "sec", "assertion", Post, c));
            yield return Case("GetAccessTokenByCibaAsync(secret)", c => OAuth2AndOIDCClient.GetAccessTokenByCibaAsync(
                Endpoint, "cid", "sec", "auth_req_id", Basic, c));
            yield return Case("GetAccessTokenByCibaAsync(any method)", c => OAuth2AndOIDCClient.GetAccessTokenByCibaAsync(
                Endpoint, "cid", "sec", "assertion", "auth_req_id", Post, c));
        }

        /// <summary>DPoP の鍵（ES256。署名器と公開鍵。1 回だけ作る）</summary>
        private static class DPoPKey
        {
            public static readonly JWS Signer;
            public static readonly JObject Public;

            static DPoPKey()
            {
                JObject priv;

                using (ECDsa ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256))
                {
                    priv = JObject.Parse(new EccPrivateKeyConverter().ParamToJwk(ecdsa.ExportParameters(true)));
                }

                Public = JwkOperator.ToPublic(priv);
                Signer = JwsFactory.CreateFromPrivateJwk("ES256", priv);
            }
        }

        private const OAuth2AndOIDCEnum.AuthMethods Basic = OAuth2AndOIDCEnum.AuthMethods.client_secret_basic;
        private const OAuth2AndOIDCEnum.AuthMethods Post = OAuth2AndOIDCEnum.AuthMethods.client_secret_post;
        private const OAuth2AndOIDCEnum.AuthMethods PrivateKeyJwt = OAuth2AndOIDCEnum.AuthMethods.private_key_jwt;

        private static TestCaseData Case(string name, Func<HttpClient, Task> call)
        {
            return new TestCaseData(call).SetName("HttpClientArgument_" + name);
        }

        [SetUp]
        public void SetUp()
        {
            SAML2Metadata.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            SAML2Metadata.Clear();
        }

        /// <summary>引数の HttpClient で送り、共有の HttpClient は使わない。</summary>
        [Test, TestCaseSource(nameof(Cases))]
        [Basis("#719（取りに行く・送る HttpClient は、引数 → 共有の OAuth2AndOIDCClient.HttpClient の順に決める）")]
        public void UsesArgument(Func<HttpClient, Task> call)
        {
            CountingHandler arg = new CountingHandler(false);

            using (StubHttp shared = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                call(new HttpClient(arg, false)).Wait();

                Assert.That(arg.Count, Is.EqualTo(1), "引数の HttpClient で送る");
                Assert.That(shared.Requests.Count, Is.EqualTo(0), "共有の HttpClient は使わない");
            }
        }

        /// <summary>引数が null なら、共有の HttpClient（従来どおり）。</summary>
        [Test, TestCaseSource(nameof(Cases))]
        [Basis("#719（引数が null なら、今どおり共有の OAuth2AndOIDCClient.HttpClient）")]
        public void NullUsesShared(Func<HttpClient, Task> call)
        {
            using (StubHttp shared = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                call(null).Wait();

                Assert.That(shared.Requests.Count, Is.EqualTo(1), "null なら共有で送る");
            }
        }

        /// <summary>引数の HttpClient が失敗しても、共有には倒さない（絞った行き先を、共有で迂回しない）。</summary>
        [Test, TestCaseSource(nameof(Cases))]
        [Basis("設計：RemoteJwkSetStore.HttpClient の remarks（設定した HttpClient が失敗しても、共有には倒さない。#707・#718）を、各口の引数にも適用（#719）")]
        public void FailureDoesNotFallBack(Func<HttpClient, Task> call)
        {
            CountingHandler arg = new CountingHandler(true);

            using (StubHttp shared = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                try
                {
                    // 失敗の扱いは口ごとに違う（取得の口は空文字、送信の口は例外）。ここでは、どちらでもよい。
                    call(new HttpClient(arg, false)).Wait();
                }
                catch (AggregateException)
                {
                }

                Assert.That(arg.Count, Is.EqualTo(1), "引数の HttpClient で送ろうとした");
                Assert.That(shared.Requests.Count, Is.EqualTo(0), "共有には倒さない");
            }
        }

        /// <summary>要求の本文を控え、順に決めた応答を返す HttpMessageHandler</summary>
        private sealed class ScriptedHandler : HttpMessageHandler
        {
            private readonly Func<int, HttpResponseMessage> _respond;

            /// <summary>受けた要求の本文</summary>
            public List<string> Bodies = new List<string>();

            public ScriptedHandler(Func<int, HttpResponseMessage> respond)
            {
                this._respond = respond;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                this.Bodies.Add(request.Content == null ? null : await request.Content.ReadAsStringAsync());
                return this._respond(this.Bodies.Count);
            }
        }

        /// <summary>
        /// SendWithDPoPAsync：use_dpop_nonce で断られたときの送り直しも、引数の HttpClient で行う（共有には倒さない）。
        /// 送り直しが共有に倒れる退行を捕まえる。#719 のレビュー
        /// </summary>
        [Test]
        [Basis("RFC 9449 8（use_dpop_nonce で断られたら、受け取った nonce を入れて送り直す）")]
        [Basis("設計：OAuth2AndOIDCClient.SendWithDPoPAsync（HttpClient 引数）の remarks（nonce の送り直しも、同じ HttpClient で行う）")]
        public void SendWithDPoPAsync_NonceRetry_UsesArgument()
        {
            // nonce は、オリジンごとに static に覚えられる。ほかのテストと干渉しないよう、このテストだけのオリジンを使う。
            Uri uri = new Uri("https://dpop-" + Guid.NewGuid().ToString("N") + ".example.com/token");

            ScriptedHandler arg = new ScriptedHandler(n =>
            {
                if (n == 1)
                {
                    HttpResponseMessage first = StubHttp.Json(HttpStatusCode.BadRequest, "{\"error\":\"use_dpop_nonce\"}");
                    first.Headers.TryAddWithoutValidation("DPoP-Nonce", "n1");
                    return first;
                }

                return StubHttp.Json(HttpStatusCode.OK, "{}");
            });

            using (StubHttp shared = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                HttpResponseMessage res = OAuth2AndOIDCClient.SendWithDPoPAsync(
                    () => new HttpRequestMessage(HttpMethod.Post, uri), DPoPKey.Signer, DPoPKey.Public, null,
                    new HttpClient(arg, false)).Result;

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.OK), "送り直した 2 回目の応答");
                Assert.That(arg.Bodies.Count, Is.EqualTo(2), "引数の HttpClient で、断られた分と送り直した分");
                Assert.That(shared.Requests.Count, Is.EqualTo(0), "送り直しも、共有には倒さない");
            }
        }

        /// <summary>
        /// DeviceAuthZRequestAsync（HttpClient 引数）：scope に null を渡すと、scope を受けない版と同じ本文（client_id だけ）になる。
        /// scope を受けない版に HttpClient 版を足していない代わりの確認。#719 のレビュー
        /// </summary>
        [Test]
        [Basis("RFC 8628 3.1（scope は任意）")]
        [Basis("設計：OAuth2AndOIDCClient.DeviceAuthZRequestAsync（HttpClient 引数）の remarks（scope に null を渡せば、scope を送らない）")]
        public void DeviceAuthZRequestAsync_NullScope_SendsClientIdOnly()
        {
            ScriptedHandler arg = new ScriptedHandler(n => StubHttp.Json(HttpStatusCode.OK, "{}"));

            OAuth2AndOIDCClient.DeviceAuthZRequestAsync(Endpoint, "cid", null, new HttpClient(arg, false)).Wait();

            Assert.That(arg.Bodies, Is.EqualTo(new List<string> { "client_id=cid" }));
        }

        /// <summary>SendAsync(HttpRequestMessage, HttpClient)：request が null なら ArgumentNullException（従来の版と同じ）。</summary>
        [Test]
        [Basis("設計：OAuth2AndOIDCClient.SendAsync（request が null なら ArgumentNullException。従来の版と同じ）")]
        public void SendAsync_NullRequest()
        {
            Assert.That(() => OAuth2AndOIDCClient.SendAsync(null, new HttpClient(new CountingHandler(false), false)).GetAwaiter().GetResult(),
                Throws.TypeOf<ArgumentNullException>());
        }
    }
}

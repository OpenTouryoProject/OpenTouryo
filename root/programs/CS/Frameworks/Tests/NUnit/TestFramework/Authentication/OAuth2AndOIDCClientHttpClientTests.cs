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
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Framework.Authentication;

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
        }

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

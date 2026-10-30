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
//* クラス名        ：OAuth2AndOIDCClientTests
//* クラス日本語名  ：OAuth2AndOIDCClient（CIBA の Token 要求・Discovery）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//**********************************************************************************

using System;
using System.Net;
using System.Net.Http;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Framework.Authentication;

namespace TestFramework.Authentication
{
    /// <summary>OAuth2AndOIDCClient（CIBA の Token 要求のクライアント認証・Discovery の取得）の単体テスト</summary>
    /// <remarks>HTTP は StubHttp で差し替える（ネットワークに出ない）。</remarks>
    [TestFixture]
    public class OAuth2AndOIDCClientTests
    {
        private static readonly Uri TokenEndpoint = new Uri("https://as.example.com/token");

        #region CIBA の Token 要求

        /// <summary>CIBA の Token 要求（client_assertion を受ける版）を送り、受けた要求を返す</summary>
        private static Tuple<HttpRequestMessage, string> Ciba(OAuth2AndOIDCEnum.AuthMethods authMethod)
        {
            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{\"access_token\":\"at\"}")))
            {
                string json = OAuth2AndOIDCClient.GetAccessTokenByCibaAsync(
                    TokenEndpoint, "client01", "secret01", "assertion01", "req01", authMethod).Result;

                Assert.That(json, Does.Contain("access_token"));
                return http.Requests[0];
            }
        }

        /// <summary>private_key_jwt：本文に client_assertion_type / client_assertion が載り、Authorization は無い。</summary>
        [Test]
        [Basis("CIBA Core 10.1（grant_type=urn:openid:params:grant-type:ciba と auth_req_id）")]
        [Basis("RFC 7523 2.2（client_assertion_type と client_assertion）")]
        public void Ciba_PrivateKeyJwt()
        {
            Tuple<HttpRequestMessage, string> r = Ciba(OAuth2AndOIDCEnum.AuthMethods.private_key_jwt);

            Assert.That(r.Item1.Headers.Authorization, Is.Null);
            Assert.That(r.Item2, Does.Contain("grant_type=urn%3Aopenid%3Aparams%3Agrant-type%3Aciba"));
            Assert.That(r.Item2, Does.Contain("auth_req_id=req01"));
            Assert.That(r.Item2, Does.Contain("client_assertion_type=urn%3Aietf%3Aparams%3Aoauth%3Aclient-assertion-type%3Ajwt-bearer"));
            Assert.That(r.Item2, Does.Contain("client_assertion=assertion01"));
            Assert.That(r.Item2, Does.Not.Contain("client_secret"));
        }

        /// <summary>tls_client_auth：本文は client_id だけ（証明書は TLS の層）。</summary>
        [Test]
        [Basis("RFC 8705 2（mTLS のクライアント認証では、client_id を送る）")]
        public void Ciba_TlsClientAuth()
        {
            Tuple<HttpRequestMessage, string> r = Ciba(OAuth2AndOIDCEnum.AuthMethods.tls_client_auth);

            Assert.That(r.Item1.Headers.Authorization, Is.Null);
            Assert.That(r.Item2, Does.Contain("client_id=client01"));
            Assert.That(r.Item2, Does.Not.Contain("client_secret"));
            Assert.That(r.Item2, Does.Not.Contain("client_assertion"));
        }

        /// <summary>client_secret_basic：Authorization: Basic。本文に資格情報は載らない。</summary>
        [Test]
        [Basis("RFC 6749 2.3.1（client_secret_basic は HTTP Basic）")]
        public void Ciba_ClientSecretBasic()
        {
            Tuple<HttpRequestMessage, string> r = Ciba(OAuth2AndOIDCEnum.AuthMethods.client_secret_basic);

            Assert.That(r.Item1.Headers.Authorization.Scheme, Is.EqualTo("Basic"));
            Assert.That(r.Item2, Does.Not.Contain("client_id"));
            Assert.That(r.Item2, Does.Not.Contain("client_assertion"));
        }

        /// <summary>既存の版（client_assertion を受けない版）は振る舞いを変えない：private_key_jwt は ArgumentException。</summary>
        [Test]
        public void Ciba_LegacyOverloadUnchanged()
        {
            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                AggregateException ex = Assert.Throws<AggregateException>(() => OAuth2AndOIDCClient.GetAccessTokenByCibaAsync(
                    TokenEndpoint, "client01", "secret01", "req01", OAuth2AndOIDCEnum.AuthMethods.private_key_jwt).Wait());
                Assert.That(ex.InnerException, Is.InstanceOf<ArgumentException>());

                OAuth2AndOIDCClient.GetAccessTokenByCibaAsync(TokenEndpoint, "client01", "secret01", "req01").Wait();
                Assert.That(http.Requests[0].Item1.Headers.Authorization.Scheme, Is.EqualTo("Basic"), "既定は Basic のまま");
            }
        }

        #endregion

        #region Discovery

        /// <summary>
        /// GetDiscoveryDocumentAsync：本文を返す。GetJwkSetAsync も同じ取得の口を通る。
        /// 通信に失敗しても例外にせず、空文字を返す。
        /// </summary>
        [Test]
        public void GetDiscoveryDocumentAsync()
        {
            Uri discovery = new Uri("https://op.example.com/.well-known/openid-configuration");

            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{\"issuer\":\"https://op.example.com\"}")))
            {
                Assert.That(OAuth2AndOIDCClient.GetDiscoveryDocumentAsync(discovery).Result, Does.Contain("issuer"));
                Assert.That(OAuth2AndOIDCClient.GetJwkSetAsync(discovery).Result, Does.Contain("issuer"));
                Assert.That(http.Requests[0].Item1.Method, Is.EqualTo(HttpMethod.Get));
            }

            using (StubHttp http = new StubHttp((req, body) => { throw new HttpRequestException("down"); }))
            {
                Assert.That(OAuth2AndOIDCClient.GetDiscoveryDocumentAsync(discovery).Result, Is.Empty);
                Assert.That(OAuth2AndOIDCClient.GetJwkSetAsync(discovery).Result, Is.Empty);
            }
        }

        #endregion
    }
}

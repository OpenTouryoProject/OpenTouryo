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
//*  2026/11/08  玄人 幸道         クライアント認証の方式を問わない版（認可コード・Refresh・Revoke・Introspect）のテストを追加（#676）
//*  2026/11/08  玄人 幸道         既存の口（PAR・CIBA）の、空の値の検査のテストを追加（#678）
//*  2026/11/09  玄人 幸道         AnyAuth_EmptySecret・ExistingAuth_EmptySecret の根拠に、RFC 6749 2.3.1 を足した（#678 の追補。#680 のレビューの指摘）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

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

        #region クライアント認証の方式を問わない版（認可コード・Refresh・Revoke・Introspect。#676）

        /// <summary>送った本文を、名前と値の組にする（URL デコードする）</summary>
        private static Dictionary<string, string> Form(string body)
        {
            Dictionary<string, string> form = new Dictionary<string, string>();
            foreach (string pair in body.Split('&'))
            {
                string[] kv = pair.Split(new[] { '=' }, 2);
                form[Uri.UnescapeDataString(kv[0].Replace('+', ' '))] = Uri.UnescapeDataString(kv[1].Replace('+', ' '));
            }
            return form;
        }

        /// <summary>送って、受けた要求を返す</summary>
        private static Tuple<HttpRequestMessage, string> Send(Func<Task<string>> call)
        {
            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{\"access_token\":\"at\"}")))
            {
                Assert.That(call().Result, Does.Contain("access_token"));
                Assert.That(http.Requests.Count, Is.EqualTo(1));
                return http.Requests[0];
            }
        }

        /// <summary>4 つの口を、指定した方式で送る（認可コードは code_verifier 付き）</summary>
        private static Tuple<HttpRequestMessage, string>[] SendAll(OAuth2AndOIDCEnum.AuthMethods authMethod)
        {
            return new Tuple<HttpRequestMessage, string>[]
            {
                Send(() => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(TokenEndpoint,
                    "client01", "secret01", "assertion01", "https://rp.example.com/cb", "code01", "verifier01", authMethod)),
                Send(() => OAuth2AndOIDCClient.UpdateAccessTokenByRefreshTokenAsync(TokenEndpoint,
                    "client01", "secret01", "assertion01", "rt01", authMethod)),
                Send(() => OAuth2AndOIDCClient.RevokeTokenAsync(new Uri("https://as.example.com/revoke"),
                    "client01", "secret01", "assertion01", "at01", "access_token", authMethod)),
                Send(() => OAuth2AndOIDCClient.IntrospectTokenAsync(new Uri("https://as.example.com/introspect"),
                    "client01", "secret01", "assertion01", "at01", "access_token", authMethod))
            };
        }

        /// <summary>
        /// private_key_jwt：4 つの口とも、本文に client_assertion_type / client_assertion が載り、
        /// client_id・client_secret・Authorization・従来の assertion は無い。
        /// </summary>
        [Test]
        [Basis("RFC 7523 2.2（client_assertion_type と client_assertion）")]
        [Basis("FAPI 2.0 Security Profile 5.3.2.1 の 6（認可サーバは MTLS か private_key_jwt でクライアントを認証する）・5.3.3.1 の 2（クライアントはどちらか、または両方に対応する）")]
        [Basis("#676")]
        public void AnyAuth_PrivateKeyJwt()
        {
            foreach (Tuple<HttpRequestMessage, string> r in SendAll(OAuth2AndOIDCEnum.AuthMethods.private_key_jwt))
            {
                Dictionary<string, string> form = Form(r.Item2);
                Assert.That(r.Item1.Headers.Authorization, Is.Null, r.Item1.RequestUri.ToString());
                Assert.That(form[OAuth2AndOIDCConst.client_assertion_type], Is.EqualTo(OAuth2AndOIDCConst.JwtBearerClientAssertionType));
                Assert.That(form[OAuth2AndOIDCConst.client_assertion], Is.EqualTo("assertion01"));
                Assert.That(form.ContainsKey(OAuth2AndOIDCConst.client_id), Is.False);
                Assert.That(form.ContainsKey(OAuth2AndOIDCConst.client_secret), Is.False);
                Assert.That(form.ContainsKey(OAuth2AndOIDCConst.assertion), Is.False, "従来の assertion は送らない");
            }
        }

        /// <summary>tls_client_auth：4 つの口とも、本文は client_id だけ（証明書は TLS の層）。秘密もアサーションも載らない。</summary>
        [Test]
        [Basis("RFC 8705 2（mTLS のクライアント認証では、client_id を送る）")]
        [Basis("FAPI 2.0 Security Profile 5.3.2.1 の 6（認可サーバは MTLS か private_key_jwt でクライアントを認証する）・5.3.3.1 の 2（クライアントはどちらか、または両方に対応する）")]
        [Basis("#676")]
        public void AnyAuth_TlsClientAuth()
        {
            foreach (Tuple<HttpRequestMessage, string> r in SendAll(OAuth2AndOIDCEnum.AuthMethods.tls_client_auth))
            {
                Dictionary<string, string> form = Form(r.Item2);
                Assert.That(r.Item1.Headers.Authorization, Is.Null, r.Item1.RequestUri.ToString());
                Assert.That(form[OAuth2AndOIDCConst.client_id], Is.EqualTo("client01"));
                Assert.That(form.ContainsKey(OAuth2AndOIDCConst.client_secret), Is.False);
                Assert.That(form.ContainsKey(OAuth2AndOIDCConst.client_assertion), Is.False);
            }
        }

        /// <summary>client_secret_basic・client_secret_post も、同じ版で送れる（Basic はヘッダ、Post は本文）。</summary>
        [Test]
        [Basis("RFC 6749 2.3.1（client_secret_basic は HTTP Basic）")]
        [Basis("RFC 6749 2.3.1（本文に client_id・client_secret を載せてもよい）")]
        [Basis("#676")]
        public void AnyAuth_ClientSecret()
        {
            foreach (Tuple<HttpRequestMessage, string> r in SendAll(OAuth2AndOIDCEnum.AuthMethods.client_secret_basic))
            {
                Dictionary<string, string> form = Form(r.Item2);
                Assert.That(r.Item1.Headers.Authorization.Scheme, Is.EqualTo("Basic"), r.Item1.RequestUri.ToString());
                Assert.That(form.ContainsKey(OAuth2AndOIDCConst.client_id), Is.False);
                Assert.That(form.ContainsKey(OAuth2AndOIDCConst.client_secret), Is.False);
                Assert.That(form.ContainsKey(OAuth2AndOIDCConst.client_assertion), Is.False);
            }

            foreach (Tuple<HttpRequestMessage, string> r in SendAll(OAuth2AndOIDCEnum.AuthMethods.client_secret_post))
            {
                Dictionary<string, string> form = Form(r.Item2);
                Assert.That(r.Item1.Headers.Authorization, Is.Null, r.Item1.RequestUri.ToString());
                Assert.That(form[OAuth2AndOIDCConst.client_id], Is.EqualTo("client01"));
                Assert.That(form[OAuth2AndOIDCConst.client_secret], Is.EqualTo("secret01"));
                Assert.That(form.ContainsKey(OAuth2AndOIDCConst.client_assertion), Is.False);
            }
        }

        /// <summary>
        /// 各口の本文：認可コードは grant_type・code・redirect_uri（受けた値のまま）と code_verifier（空なら送らない）、
        /// Refresh は grant_type・refresh_token、Revoke・Introspect は token と token_type_hint（空なら送らない）。
        /// </summary>
        [Test]
        [Basis("RFC 6749 4.1.3（grant_type=authorization_code・code・redirect_uri。redirect_uri は認可要求と同じ値）")]
        [Basis("RFC 7636 4.5（Token 要求に code_verifier を送る）")]
        [Basis("RFC 6749 6（grant_type=refresh_token・refresh_token。クライアント認証をする）")]
        [Basis("RFC 7009 2.1（token と token_type_hint。token_type_hint は任意）")]
        [Basis("RFC 7662 2.1（token と token_type_hint。token_type_hint は任意）")]
        [Basis("#676")]
        public void AnyAuth_Body()
        {
            const OAuth2AndOIDCEnum.AuthMethods pkjwt = OAuth2AndOIDCEnum.AuthMethods.private_key_jwt;
            const string redirect = "https://rp.example.com/cb?a=1&b=<x>";

            // 認可コード：redirect_uri は受けた値のまま届く（HTML エンコードしない）
            Dictionary<string, string> code = Form(Send(() => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(TokenEndpoint,
                "client01", "", "assertion01", redirect, "code01", "verifier01", pkjwt)).Item2);
            Assert.That(code[OAuth2AndOIDCConst.grant_type], Is.EqualTo(OAuth2AndOIDCConst.AuthorizationCodeGrantType));
            Assert.That(code[OAuth2AndOIDCConst.code], Is.EqualTo("code01"));
            Assert.That(code[OAuth2AndOIDCConst.redirect_uri], Is.EqualTo(redirect));
            Assert.That(code[OAuth2AndOIDCConst.code_verifier], Is.EqualTo("verifier01"));

            // code_verifier が空（null も）なら送らない
            foreach (string verifier in new string[] { "", null })
            {
                Dictionary<string, string> noPkce = Form(Send(() => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(TokenEndpoint,
                    "client01", "", "assertion01", redirect, "code01", verifier, pkjwt)).Item2);
                Assert.That(noPkce.ContainsKey(OAuth2AndOIDCConst.code_verifier), Is.False);
            }

            // Refresh
            Dictionary<string, string> refresh = Form(Send(() => OAuth2AndOIDCClient.UpdateAccessTokenByRefreshTokenAsync(TokenEndpoint,
                "client01", "", "assertion01", "rt01", pkjwt)).Item2);
            Assert.That(refresh[OAuth2AndOIDCConst.grant_type], Is.EqualTo(OAuth2AndOIDCConst.RefreshTokenGrantType));
            Assert.That(refresh[OAuth2AndOIDCConst.RefreshToken], Is.EqualTo("rt01"));

            // Revoke・Introspect：token と token_type_hint。token_type_hint が空（null も）なら送らない
            Uri revoke = new Uri("https://as.example.com/revoke");
            Uri introspect = new Uri("https://as.example.com/introspect");
            foreach (string hint in new string[] { "refresh_token", "", null })
            {
                foreach (Dictionary<string, string> form in new Dictionary<string, string>[]
                {
                    Form(Send(() => OAuth2AndOIDCClient.RevokeTokenAsync(revoke, "client01", "", "assertion01", "rt01", hint, pkjwt)).Item2),
                    Form(Send(() => OAuth2AndOIDCClient.IntrospectTokenAsync(introspect, "client01", "", "assertion01", "rt01", hint, pkjwt)).Item2)
                })
                {
                    Assert.That(form[OAuth2AndOIDCConst.token], Is.EqualTo("rt01"));
                    if (string.IsNullOrEmpty(hint))
                    {
                        Assert.That(form.ContainsKey(OAuth2AndOIDCConst.token_type_hint), Is.False, "hint = " + (hint ?? "null"));
                    }
                    else
                    {
                        Assert.That(form[OAuth2AndOIDCConst.token_type_hint], Is.EqualTo(hint));
                    }
                }
            }
        }

        /// <summary>知らない方式は ArgumentException で、送らない（4 つの口とも）。</summary>
        [Test]
        [Basis("設計：OAuth2AndOIDCClient.SetClientAuthentication（知らない方式は ArgumentException）")]
        [Basis("#676")]
        public void AnyAuth_UnknownMethod()
        {
            OAuth2AndOIDCEnum.AuthMethods unknown = (OAuth2AndOIDCEnum.AuthMethods)99;
            Uri revoke = new Uri("https://as.example.com/revoke");
            Uri introspect = new Uri("https://as.example.com/introspect");

            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                foreach (Func<Task<string>> call in new Func<Task<string>>[]
                {
                    () => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(TokenEndpoint, "c", "s", "a", "https://rp.example.com/cb", "code", "", unknown),
                    () => OAuth2AndOIDCClient.UpdateAccessTokenByRefreshTokenAsync(TokenEndpoint, "c", "s", "a", "rt", unknown),
                    () => OAuth2AndOIDCClient.RevokeTokenAsync(revoke, "c", "s", "a", "t", "h", unknown),
                    () => OAuth2AndOIDCClient.IntrospectTokenAsync(introspect, "c", "s", "a", "t", "h", unknown)
                })
                {
                    AggregateException ex = Assert.Throws<AggregateException>(() => call().Wait());
                    Assert.That(ex.InnerException, Is.InstanceOf<ArgumentException>());
                }

                Assert.That(http.Requests, Is.Empty, "送らない");
            }
        }

        /// <summary>client_secret が空でも、client_secret_basic・client_secret_post は送る（公開クライアントの使い方を塞がない）。</summary>
        [Test]
        [Basis("RFC 6749 4.1.3（認証しないクライアントは client_id を送る）")]
        [Basis("RFC 6749 2.3.1（client_secret が空文字列なら、省いてよい）")]
        [Basis("設計：OAuth2AndOIDCClient.PostWithClientAuthenticationAsync の remarks（client_secret は検査しない）")]
        [Basis("#676")]
        public void AnyAuth_EmptySecret()
        {
            foreach (OAuth2AndOIDCEnum.AuthMethods authMethod in new[] {
                OAuth2AndOIDCEnum.AuthMethods.client_secret_basic, OAuth2AndOIDCEnum.AuthMethods.client_secret_post })
            {
                foreach (string secret in new string[] { "", null })
                {
                    Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(TokenEndpoint,
                        "client01", secret, null, "https://rp.example.com/cb", "code01", "verifier01", authMethod));
                    Assert.That(Form(r.Item2)[OAuth2AndOIDCConst.code], Is.EqualTo("code01"), authMethod + " / " + (secret ?? "null"));
                }
            }
        }

        /// <summary>空では認証として成り立たない値が空（null も）なら ArgumentNullException で、送らない（4 つの口とも）。</summary>
        [Test]
        [Basis("設計：OAuth2AndOIDCClient.PostWithClientAuthenticationAsync の remarks（方式が使う値が空なら ArgumentNullException）")]
        [Basis("#676")]
        [TestCase(OAuth2AndOIDCEnum.AuthMethods.private_key_jwt, "c", "s", "", "client_assertion")]
        [TestCase(OAuth2AndOIDCEnum.AuthMethods.client_secret_jwt, "c", "s", null, "client_assertion")]
        [TestCase(OAuth2AndOIDCEnum.AuthMethods.client_secret_basic, "", "s", "a", "client_id")]
        [TestCase(OAuth2AndOIDCEnum.AuthMethods.client_secret_post, null, "s", "a", "client_id")]
        [TestCase(OAuth2AndOIDCEnum.AuthMethods.tls_client_auth, "", "s", "a", "client_id")]
        public void AnyAuth_MissingCredential(OAuth2AndOIDCEnum.AuthMethods authMethod,
            string client_id, string client_secret, string client_assertion, string paramName)
        {
            Uri revoke = new Uri("https://as.example.com/revoke");
            Uri introspect = new Uri("https://as.example.com/introspect");

            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                foreach (Func<Task<string>> call in new Func<Task<string>>[]
                {
                    () => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(TokenEndpoint,
                        client_id, client_secret, client_assertion, "https://rp.example.com/cb", "code", "", authMethod),
                    () => OAuth2AndOIDCClient.UpdateAccessTokenByRefreshTokenAsync(TokenEndpoint,
                        client_id, client_secret, client_assertion, "rt", authMethod),
                    () => OAuth2AndOIDCClient.RevokeTokenAsync(revoke,
                        client_id, client_secret, client_assertion, "t", "h", authMethod),
                    () => OAuth2AndOIDCClient.IntrospectTokenAsync(introspect,
                        client_id, client_secret, client_assertion, "t", "h", authMethod)
                })
                {
                    AggregateException ex = Assert.Throws<AggregateException>(() => call().Wait());
                    Assert.That(ex.InnerException, Is.TypeOf<ArgumentNullException>());
                    Assert.That(((ArgumentNullException)ex.InnerException).ParamName, Is.EqualTo(paramName));
                }

                Assert.That(http.Requests, Is.Empty, "送らない");
            }
        }

        #endregion

        #region 既存の口の、空の値の検査（PAR・CIBA の認可リクエスト・CIBA の Token 要求。#678）

        /// <summary>PAR・CIBA の認可リクエスト・CIBA の Token 要求（client_assertion を受ける版）を、同じ資格情報で呼ぶ</summary>
        private static Func<Task<string>>[] ExistingCalls(OAuth2AndOIDCEnum.AuthMethods authMethod,
            string client_id, string client_secret, string client_assertion)
        {
            Uri par = new Uri("https://as.example.com/par");
            Uri ciba = new Uri("https://as.example.com/ciba");
            return new Func<Task<string>>[]
            {
                () => OAuth2AndOIDCClient.PushAuthorizationRequestAsync(par, "ro01", client_id, client_secret, client_assertion, authMethod),
                () => OAuth2AndOIDCClient.CibaAuthZRequestAsync(ciba, "ro01", client_id, client_secret, client_assertion, authMethod),
                () => OAuth2AndOIDCClient.GetAccessTokenByCibaAsync(TokenEndpoint, client_id, client_secret, client_assertion, "req01", authMethod)
            };
        }

        /// <summary>
        /// 既存の口も、空では認証として成り立たない値が空（null も）なら ArgumentNullException で、送らない（3 つの口とも）。
        /// </summary>
        [Test]
        [Basis("設計：OAuth2AndOIDCClient.SetClientAuthentication の remarks（空では認証として成り立たない値は ArgumentNullException）")]
        [Basis("#678")]
        [TestCase(OAuth2AndOIDCEnum.AuthMethods.private_key_jwt, "c", "s", "", "client_assertion")]
        [TestCase(OAuth2AndOIDCEnum.AuthMethods.client_secret_jwt, "c", "s", null, "client_assertion")]
        [TestCase(OAuth2AndOIDCEnum.AuthMethods.client_secret_basic, "", "s", "a", "client_id")]
        [TestCase(OAuth2AndOIDCEnum.AuthMethods.client_secret_post, null, "s", "a", "client_id")]
        [TestCase(OAuth2AndOIDCEnum.AuthMethods.tls_client_auth, "", "s", "a", "client_id")]
        public void ExistingAuth_MissingCredential(OAuth2AndOIDCEnum.AuthMethods authMethod,
            string client_id, string client_secret, string client_assertion, string paramName)
        {
            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                foreach (Func<Task<string>> call in ExistingCalls(authMethod, client_id, client_secret, client_assertion))
                {
                    AggregateException ex = Assert.Throws<AggregateException>(() => call().Wait());
                    Assert.That(ex.InnerException, Is.TypeOf<ArgumentNullException>());
                    Assert.That(((ArgumentNullException)ex.InnerException).ParamName, Is.EqualTo(paramName));
                }

                Assert.That(http.Requests, Is.Empty, "送らない");
            }
        }

        /// <summary>
        /// PAR は、方式によらず client_id が要る（private_key_jwt でも空なら ArgumentNullException）。
        /// CIBA は、private_key_jwt なら client_id が空でも送る（認証方式が要求するもの以外を JWT の外に置かないため、もともと載せない）。
        /// </summary>
        [Test]
        [Basis("RFC 9126 2.1（client_id は認可リクエストの必須パラメタなので、PAR でも必須）")]
        [Basis("CIBA Core 1.0 7.1.1（認証方式が要求するパラメタ以外を、署名付きの認証要求の外に置かない）")]
        [Basis("#678")]
        public void ExistingAuth_ParRequiresClientId()
        {
            foreach (string clientId in new string[] { "", null })
            {
                Func<Task<string>>[] calls = ExistingCalls(OAuth2AndOIDCEnum.AuthMethods.private_key_jwt, clientId, null, "assertion01");

                using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
                {
                    AggregateException ex = Assert.Throws<AggregateException>(() => calls[0]().Wait());
                    Assert.That(ex.InnerException, Is.TypeOf<ArgumentNullException>());
                    Assert.That(((ArgumentNullException)ex.InnerException).ParamName, Is.EqualTo("client_id"));
                    Assert.That(http.Requests, Is.Empty, "PAR は送らない");
                }

                foreach (Func<Task<string>> call in new[] { calls[1], calls[2] })
                {
                    Tuple<HttpRequestMessage, string> r = Send(call);
                    Assert.That(Form(r.Item2)[OAuth2AndOIDCConst.client_assertion], Is.EqualTo("assertion01"));
                }
            }
        }

        /// <summary>既存の口も、client_secret が空（null も）なら、Basic・Post で送る（公開クライアントの使い方を塞がない）。</summary>
        [Test]
        [Basis("RFC 6749 4.1.3（認証しないクライアントは client_id を送る）")]
        [Basis("RFC 6749 2.3.1（client_secret が空文字列なら、省いてよい）")]
        [Basis("設計：OAuth2AndOIDCClient.SetClientAuthentication の remarks（client_secret は検査しない）")]
        [Basis("#678")]
        public void ExistingAuth_EmptySecret()
        {
            foreach (OAuth2AndOIDCEnum.AuthMethods authMethod in new[] {
                OAuth2AndOIDCEnum.AuthMethods.client_secret_basic, OAuth2AndOIDCEnum.AuthMethods.client_secret_post })
            {
                foreach (string secret in new string[] { "", null })
                {
                    foreach (Func<Task<string>> call in ExistingCalls(authMethod, "client01", secret, null))
                    {
                        Assert.That(Send(call).Item1, Is.Not.Null, authMethod + " / " + (secret ?? "null"));
                    }
                }
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

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
//* クラス名        ：OAuth2AndOIDCClientRequestsTests
//* クラス日本語名  ：OAuth2AndOIDCClient の各要求（グラント・トークン管理・拡張グラント・PAR・UserInfo）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（#634。partial に分けたファイルのカバレッジの拡充）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using NUnit.Framework;

using Touryo.Infrastructure.Framework.Authentication;

namespace TestFramework.Authentication
{
    /// <summary>OAuth2AndOIDCClient の各要求（グラント・トークン管理・拡張グラント・PAR・UserInfo）の単体テスト</summary>
    /// <remarks>
    /// HTTP は StubHttp で差し替える（ネットワークに出ない）。
    /// 送った要求（メソッド・URL・Authorization・本文）が、各 RFC の形になっていることを確かめる。
    /// </remarks>
    [TestFixture]
    public class OAuth2AndOIDCClientRequestsTests
    {
        private static readonly Uri TokenEndpoint = new Uri("https://as.example.com/token");
        private const string RedirectUri = "https://rp.example.com/cb";

        #region 補助

        /// <summary>要求を送り、受けた要求（1 件）を返す。応答は {"ok":true}</summary>
        private static Tuple<HttpRequestMessage, string> Send(Func<string> send)
        {
            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{\"ok\":true}")))
            {
                Assert.That(send(), Is.EqualTo("{\"ok\":true}"), "応答の本文をそのまま返す");
                Assert.That(http.Requests.Count, Is.EqualTo(1));
                return http.Requests[0];
            }
        }

        /// <summary>application/x-www-form-urlencoded の本文を読む</summary>
        private static Dictionary<string, string> Form(string body)
        {
            Dictionary<string, string> form = new Dictionary<string, string>();

            foreach (string pair in (body ?? "").Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] kv = pair.Split(new char[] { '=' }, 2);
                form[WebUtility.UrlDecode(kv[0])] = (kv.Length < 2) ? "" : WebUtility.UrlDecode(kv[1]);
            }

            return form;
        }

        /// <summary>Basic 認証で client01:secret01 を送ったこと</summary>
        private static void AssertBasic(HttpRequestMessage req)
        {
            Assert.That(req.Headers.Authorization.Scheme, Is.EqualTo("Basic"));
            Assert.That(req.Headers.Authorization.Parameter,
                Is.EqualTo(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("client01:secret01"))));
        }

        /// <summary>非同期のメソッドが、内側で ArgumentException を投げること</summary>
        private static void AssertArgumentException(Action wait)
        {
            AggregateException ex = Assert.Throws<AggregateException>(() => wait());
            Assert.That(ex.InnerException, Is.InstanceOf<ArgumentException>());
        }

        #endregion

        #region Authorization Code（RFC 6749 4.1）

        /// <summary>認可コード：Basic（既定）は Authorization ヘッダ、本文は grant_type・code・redirect_uri だけ。</summary>
        [Test]
        public void Code_ClientSecretBasic()
        {
            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                TokenEndpoint, "client01", "secret01", RedirectUri, "code01").Result);

            Assert.That(r.Item1.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(r.Item1.RequestUri, Is.EqualTo(TokenEndpoint));
            AssertBasic(r.Item1);

            Dictionary<string, string> form = Form(r.Item2);
            Assert.That(form[OAuth2AndOIDCConst.grant_type], Is.EqualTo(OAuth2AndOIDCConst.AuthorizationCodeGrantType));
            Assert.That(form[OAuth2AndOIDCConst.code], Is.EqualTo("code01"));
            Assert.That(form[OAuth2AndOIDCConst.redirect_uri], Is.EqualTo(RedirectUri));
            Assert.That(form.ContainsKey(OAuth2AndOIDCConst.client_secret), Is.False);
        }

        /// <summary>認可コード：client_secret_post は本文に client_id / client_secret。Authorization は無い。</summary>
        [Test]
        public void Code_ClientSecretPost()
        {
            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                TokenEndpoint, "client01", "secret01", RedirectUri, "code01",
                OAuth2AndOIDCEnum.AuthMethods.client_secret_post).Result);

            Assert.That(r.Item1.Headers.Authorization, Is.Null);
            Dictionary<string, string> form = Form(r.Item2);
            Assert.That(form[OAuth2AndOIDCConst.client_id], Is.EqualTo("client01"));
            Assert.That(form[OAuth2AndOIDCConst.client_secret], Is.EqualTo("secret01"));
        }

        /// <summary>認可コード（PKCE）：本文に client_id と code_verifier。client_secret は送らない。</summary>
        [Test]
        public void Code_Pkce()
        {
            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                TokenEndpoint, "client01", "secret01", RedirectUri, "code01", "verifier01").Result);

            Assert.That(r.Item1.Headers.Authorization, Is.Null);
            Dictionary<string, string> form = Form(r.Item2);
            Assert.That(form[OAuth2AndOIDCConst.grant_type], Is.EqualTo(OAuth2AndOIDCConst.AuthorizationCodeGrantType));
            Assert.That(form[OAuth2AndOIDCConst.client_id], Is.EqualTo("client01"));
            Assert.That(form[OAuth2AndOIDCConst.code_verifier], Is.EqualTo("verifier01"));
            Assert.That(form.ContainsKey(OAuth2AndOIDCConst.client_secret), Is.False);
        }

        /// <summary>
        /// 認可コード（FAPI1 の private_key_jwt）：assertion と、RFC 7523 2.2 の client_assertion_type / client_assertion の両方を送る。
        /// </summary>
        [Test]
        public void Code_PrivateKeyJwt()
        {
            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                TokenEndpoint, RedirectUri, "code01", "assertion01").Result);

            Assert.That(r.Item1.Headers.Authorization, Is.Null);
            Dictionary<string, string> form = Form(r.Item2);
            Assert.That(form[OAuth2AndOIDCConst.assertion], Is.EqualTo("assertion01"));
            Assert.That(form[OAuth2AndOIDCConst.client_assertion_type], Is.EqualTo(OAuth2AndOIDCConst.JwtBearerClientAssertionType));
            Assert.That(form[OAuth2AndOIDCConst.client_assertion], Is.EqualTo("assertion01"));
        }

        /// <summary>認可コード：方式と引数の組み合わせが合わなければ ArgumentException（送らない）。</summary>
        [Test]
        public void Code_InvalidCombination()
        {
            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                // 通常の要求で、秘密を使わない方式
                AssertArgumentException(() => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                    TokenEndpoint, "client01", "secret01", RedirectUri, "code01",
                    OAuth2AndOIDCEnum.AuthMethods.private_key_jwt).Wait());

                // PKCE で client_secret_post 以外
                AssertArgumentException(() => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                    TokenEndpoint, "client01", "secret01", RedirectUri, "code01", "verifier01",
                    OAuth2AndOIDCEnum.AuthMethods.client_secret_basic).Wait());

                // assertion で private_key_jwt 以外
                AssertArgumentException(() => OAuth2AndOIDCClient.GetAccessTokenByCodeAsync(
                    TokenEndpoint, RedirectUri, "code01", "assertion01",
                    OAuth2AndOIDCEnum.AuthMethods.client_secret_basic).Wait());

                Assert.That(http.Requests, Is.Empty, "送らない");
            }
        }

        #endregion

        #region Client Credentials / Password / Refresh

        /// <summary>Client Credentials（RFC 6749 4.4）：Basic と grant_type・scope。</summary>
        [Test]
        public void ClientCredentials()
        {
            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.ClientCredentialsGrantAsync(
                TokenEndpoint, "client01", "secret01", "read write").Result);

            AssertBasic(r.Item1);
            Dictionary<string, string> form = Form(r.Item2);
            Assert.That(form[OAuth2AndOIDCConst.grant_type], Is.EqualTo(OAuth2AndOIDCConst.ClientCredentialsGrantType));
            Assert.That(form[OAuth2AndOIDCConst.scope], Is.EqualTo("read write"));
        }

        /// <summary>Resource Owner Password Credentials（RFC 6749 4.3）：Basic と username・password・scope。</summary>
        [Test]
        public void ResourceOwnerPasswordCredentials()
        {
            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.ResourceOwnerPasswordCredentialsGrantAsync(
                TokenEndpoint, "client01", "secret01", "user01", "p@ss&word", "openid").Result);

            AssertBasic(r.Item1);
            Dictionary<string, string> form = Form(r.Item2);
            Assert.That(form[OAuth2AndOIDCConst.grant_type], Is.EqualTo(OAuth2AndOIDCConst.ResourceOwnerPasswordCredentialsGrantType));
            Assert.That(form["username"], Is.EqualTo("user01"));
            Assert.That(form["password"], Is.EqualTo("p@ss&word"), "記号も URL エンコードされて届く");
            Assert.That(form[OAuth2AndOIDCConst.scope], Is.EqualTo("openid"));
        }

        /// <summary>Refresh Token（RFC 6749 6）：Basic と grant_type・refresh_token。</summary>
        [Test]
        public void RefreshToken()
        {
            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.UpdateAccessTokenByRefreshTokenAsync(
                TokenEndpoint, "client01", "secret01", "rt01").Result);

            AssertBasic(r.Item1);
            Dictionary<string, string> form = Form(r.Item2);
            Assert.That(form[OAuth2AndOIDCConst.grant_type], Is.EqualTo(OAuth2AndOIDCConst.RefreshTokenGrantType));
            Assert.That(form[OAuth2AndOIDCConst.RefreshToken], Is.EqualTo("rt01"));
        }

        #endregion

        #region Revoke（RFC 7009）・Introspect（RFC 7662）

        /// <summary>Revoke / Introspect：Basic（既定）は Authorization ヘッダ、本文は token と token_type_hint。</summary>
        [Test]
        public void RevokeAndIntrospect_ClientSecretBasic()
        {
            Uri revoke = new Uri("https://as.example.com/revoke");
            Uri introspect = new Uri("https://as.example.com/introspect");

            foreach (Tuple<HttpRequestMessage, string> r in new Tuple<HttpRequestMessage, string>[]
            {
                Send(() => OAuth2AndOIDCClient.RevokeTokenAsync(revoke, "client01", "secret01", "at01", "access_token").Result),
                Send(() => OAuth2AndOIDCClient.IntrospectTokenAsync(introspect, "client01", "secret01", "at01", "access_token").Result)
            })
            {
                AssertBasic(r.Item1);
                Dictionary<string, string> form = Form(r.Item2);
                Assert.That(form[OAuth2AndOIDCConst.token], Is.EqualTo("at01"));
                Assert.That(form[OAuth2AndOIDCConst.token_type_hint], Is.EqualTo("access_token"));
                Assert.That(form.ContainsKey(OAuth2AndOIDCConst.client_secret), Is.False);
            }
        }

        /// <summary>Revoke / Introspect：client_secret_post は本文に client_id / client_secret。それ以外の方式は ArgumentException。</summary>
        [Test]
        public void RevokeAndIntrospect_ClientSecretPost()
        {
            Uri revoke = new Uri("https://as.example.com/revoke");
            Uri introspect = new Uri("https://as.example.com/introspect");

            foreach (Tuple<HttpRequestMessage, string> r in new Tuple<HttpRequestMessage, string>[]
            {
                Send(() => OAuth2AndOIDCClient.RevokeTokenAsync(revoke, "client01", "secret01", "rt01", "refresh_token",
                    OAuth2AndOIDCEnum.AuthMethods.client_secret_post).Result),
                Send(() => OAuth2AndOIDCClient.IntrospectTokenAsync(introspect, "client01", "secret01", "rt01", "refresh_token",
                    OAuth2AndOIDCEnum.AuthMethods.client_secret_post).Result)
            })
            {
                Assert.That(r.Item1.Headers.Authorization, Is.Null);
                Dictionary<string, string> form = Form(r.Item2);
                Assert.That(form[OAuth2AndOIDCConst.client_id], Is.EqualTo("client01"));
                Assert.That(form[OAuth2AndOIDCConst.client_secret], Is.EqualTo("secret01"));
                Assert.That(form[OAuth2AndOIDCConst.token], Is.EqualTo("rt01"));
            }

            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                AssertArgumentException(() => OAuth2AndOIDCClient.RevokeTokenAsync(revoke, "client01", "secret01", "t", "h",
                    OAuth2AndOIDCEnum.AuthMethods.private_key_jwt).Wait());
                AssertArgumentException(() => OAuth2AndOIDCClient.IntrospectTokenAsync(introspect, "client01", "secret01", "t", "h",
                    OAuth2AndOIDCEnum.AuthMethods.tls_client_auth).Wait());
                Assert.That(http.Requests, Is.Empty, "送らない");
            }
        }

        #endregion

        #region 拡張グラント（JWT Bearer・Device・CIBA）

        /// <summary>JWT Bearer（RFC 7523 2.1）：grant_type と assertion。クライアント認証は付けない。</summary>
        [Test]
        public void JwtBearer()
        {
            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.JwtBearerTokenFlowAsync(
                TokenEndpoint, "jwt01").Result);

            Assert.That(r.Item1.Headers.Authorization, Is.Null);
            Dictionary<string, string> form = Form(r.Item2);
            Assert.That(form[OAuth2AndOIDCConst.grant_type], Is.EqualTo(OAuth2AndOIDCConst.JwtBearerTokenFlowGrantType));
            Assert.That(form[OAuth2AndOIDCConst.assertion], Is.EqualTo("jwt01"));
        }

        /// <summary>Device（RFC 8628）：認可要求は client_id だけ、Token 要求は grant_type・client_id・device_code。</summary>
        [Test]
        public void DeviceAuthZ()
        {
            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.DeviceAuthZRequestAsync(
                new Uri("https://as.example.com/device"), "client01").Result);

            Assert.That(Form(r.Item2), Is.EqualTo(new Dictionary<string, string> { { OAuth2AndOIDCConst.client_id, "client01" } }));

            r = Send(() => OAuth2AndOIDCClient.GetAccessTokenByDeviceAuthZAsync(TokenEndpoint, "client01", "device01").Result);

            Dictionary<string, string> form = Form(r.Item2);
            Assert.That(form[OAuth2AndOIDCConst.grant_type], Is.EqualTo(OAuth2AndOIDCConst.DeviceAuthZGrantType));
            Assert.That(form[OAuth2AndOIDCConst.client_id], Is.EqualTo("client01"));
            Assert.That(form[OAuth2AndOIDCConst.device_code], Is.EqualTo("device01"));
        }

        /// <summary>
        /// CIBA の認可要求：request_uri を送る版と、Request Object を送る版。
        /// 後者の client_secret_post は本文に client_id / client_secret を載せ、Basic（既定）は client_id を本文に載せない（CIBA Core 7.1.1）。
        /// </summary>
        [Test]
        public void CibaAuthZRequest()
        {
            Uri ciba = new Uri("https://as.example.com/bc-authorize");

            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.CibaAuthZRequestAsync(ciba, "urn:req01").Result);
            Assert.That(Form(r.Item2)[OAuth2AndOIDCConst.request_uri], Is.EqualTo("urn:req01"));

            r = Send(() => OAuth2AndOIDCClient.CibaAuthZRequestAsync(ciba, "ro01", "client01", "secret01").Result);
            AssertBasic(r.Item1);
            Assert.That(Form(r.Item2), Is.EqualTo(new Dictionary<string, string> { { OAuth2AndOIDCConst.request, "ro01" } }));

            r = Send(() => OAuth2AndOIDCClient.CibaAuthZRequestAsync(ciba, "ro01", "client01", "secret01",
                OAuth2AndOIDCEnum.AuthMethods.client_secret_post).Result);
            Assert.That(r.Item1.Headers.Authorization, Is.Null);
            Assert.That(Form(r.Item2)[OAuth2AndOIDCConst.client_secret], Is.EqualTo("secret01"));
        }

        /// <summary>CIBA の Token 要求（client_assertion を受けない版）：client_secret_post は本文に資格情報。</summary>
        [Test]
        public void CibaToken_ClientSecretPost()
        {
            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.GetAccessTokenByCibaAsync(
                TokenEndpoint, "client01", "secret01", "req01", OAuth2AndOIDCEnum.AuthMethods.client_secret_post).Result);

            Assert.That(r.Item1.Headers.Authorization, Is.Null);
            Dictionary<string, string> form = Form(r.Item2);
            Assert.That(form[OAuth2AndOIDCConst.grant_type], Is.EqualTo(OAuth2AndOIDCConst.CibaGrantType));
            Assert.That(form[OAuth2AndOIDCConst.auth_req_id], Is.EqualTo("req01"));
            Assert.That(form[OAuth2AndOIDCConst.client_id], Is.EqualTo("client01"));
            Assert.That(form[OAuth2AndOIDCConst.client_secret], Is.EqualTo("secret01"));
        }

        #endregion

        #region Request Object の預け先・PAR（RFC 9126）

        /// <summary>Request Object の預け先（RFC 9101 5.2.1）：生の本文として POST する。</summary>
        [Test]
        public void RegisterRequestObject()
        {
            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.RegisterRequestObjectAsync(
                new Uri("https://as.example.com/ros"), "eyJ.ro.01").Result);

            Assert.That(r.Item1.Method, Is.EqualTo(HttpMethod.Post));
            Assert.That(r.Item2, Is.EqualTo("eyJ.ro.01"));
        }

        /// <summary>
        /// PAR：Request Object を request に載せ、client_id は方式に依らず本文に載せる（RFC 9126 2.1）。
        /// private_key_jwt は client_assertion、client_secret_post は client_secret。
        /// </summary>
        [Test]
        public void PushAuthorizationRequest()
        {
            Uri par = new Uri("https://as.example.com/par");

            // Request Object・Basic（既定）
            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                par, "ro01", "client01", "secret01").Result);
            AssertBasic(r.Item1);
            Dictionary<string, string> form = Form(r.Item2);
            Assert.That(form[OAuth2AndOIDCConst.request], Is.EqualTo("ro01"));
            Assert.That(form[OAuth2AndOIDCConst.client_id], Is.EqualTo("client01"), "Basic でも client_id を本文に載せる");

            // Request Object・private_key_jwt
            r = Send(() => OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                par, "ro01", "client01", null, "assertion01", OAuth2AndOIDCEnum.AuthMethods.private_key_jwt).Result);
            Assert.That(r.Item1.Headers.Authorization, Is.Null);
            form = Form(r.Item2);
            Assert.That(form[OAuth2AndOIDCConst.client_assertion], Is.EqualTo("assertion01"));
            Assert.That(form[OAuth2AndOIDCConst.client_assertion_type], Is.EqualTo(OAuth2AndOIDCConst.JwtBearerClientAssertionType));

            // パラメタ・client_secret_post（引数の Dictionary は変えない）
            Dictionary<string, string> p = new Dictionary<string, string>
            {
                { OAuth2AndOIDCConst.response_type, "code" }, { OAuth2AndOIDCConst.scope, "openid" }
            };
            r = Send(() => OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                par, p, "client01", "secret01", OAuth2AndOIDCEnum.AuthMethods.client_secret_post).Result);
            form = Form(r.Item2);
            Assert.That(form[OAuth2AndOIDCConst.response_type], Is.EqualTo("code"));
            Assert.That(form[OAuth2AndOIDCConst.client_secret], Is.EqualTo("secret01"));
            Assert.That(p.Count, Is.EqualTo(2), "引数の Dictionary は変えない");

            // 知らない方式は ArgumentException
            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                AssertArgumentException(() => OAuth2AndOIDCClient.PushAuthorizationRequestAsync(
                    par, p, "client01", "secret01", (OAuth2AndOIDCEnum.AuthMethods)(-1)).Wait());
                Assert.That(http.Requests, Is.Empty);
            }
        }

        #endregion

        #region UserInfo・Registration Endpoint・SendAsync

        /// <summary>UserInfo（OIDC Core 5.3）：GET と Bearer。</summary>
        [Test]
        public void GetUserInfo()
        {
            Uri userInfo = new Uri("https://op.example.com/userinfo");
            Tuple<HttpRequestMessage, string> r = Send(() => OAuth2AndOIDCClient.GetUserInfoAsync(userInfo, "at01").Result);

            Assert.That(r.Item1.Method, Is.EqualTo(HttpMethod.Get));
            Assert.That(r.Item1.RequestUri, Is.EqualTo(userInfo));
            Assert.That(r.Item1.Headers.Authorization.Scheme, Is.EqualTo("Bearer"));
            Assert.That(r.Item1.Headers.Authorization.Parameter, Is.EqualTo("at01"));
        }

        /// <summary>GetRegistrationEndpointAsync：discovery の registration_endpoint。無い・文字列でない・JSON でないなら空文字。</summary>
        [TestCase("{\"registration_endpoint\":\"https://op.example.com/register\"}", ExpectedResult = "https://op.example.com/register")]
        [TestCase("{\"issuer\":\"https://op.example.com\"}", ExpectedResult = "")]
        [TestCase("{\"registration_endpoint\":1}", ExpectedResult = "")]
        [TestCase("[]", ExpectedResult = "")]
        [TestCase("not json", ExpectedResult = "")]
        public string GetRegistrationEndpoint(string discovery)
        {
            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, discovery)))
            {
                return OAuth2AndOIDCClient.GetRegistrationEndpointAsync(
                    new Uri("https://op.example.com/.well-known/openid-configuration")).Result;
            }
        }

        /// <summary>SendAsync：同じ HttpClient で送り、応答をそのまま返す。null は ArgumentNullException。</summary>
        [Test]
        public void SendAsync()
        {
            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.Accepted, "{}")))
            {
                HttpResponseMessage res = OAuth2AndOIDCClient.SendAsync(
                    new HttpRequestMessage(HttpMethod.Delete, "https://as.example.com/x")).Result;

                Assert.That(res.StatusCode, Is.EqualTo(HttpStatusCode.Accepted), "状態コードを見られる");
                Assert.That(http.Requests[0].Item1.Method, Is.EqualTo(HttpMethod.Delete));

                AggregateException ex = Assert.Throws<AggregateException>(() => OAuth2AndOIDCClient.SendAsync(null).Wait());
                Assert.That(ex.InnerException, Is.InstanceOf<ArgumentNullException>());
            }
        }

        /// <summary>HttpClient が無いとき、公開情報の取得（GetJwkSetAsync）は例外にせず空文字。</summary>
        [Test]
        public void GetJwkSet_NoHttpClient()
        {
            HttpClient original = OAuth2AndOIDCClient.HttpClient;

            try
            {
                OAuth2AndOIDCClient.HttpClient = null;
                Assert.That(OAuth2AndOIDCClient.GetJwkSetAsync(new Uri("https://op.example.com/jwks")).Result, Is.Empty);
            }
            finally
            {
                OAuth2AndOIDCClient.HttpClient = original;
            }
        }

        #endregion
    }
}

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
//* クラス名        ：JwtTokenTests
//* クラス日本語名  ：CmnJwtToken / AccessToken / IdToken / ResponseObject の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/19  玄人 幸道         新規作成
//*  2026/10/29  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）を広げた（#640）
//*  2026/10/30  玄人 幸道         ResponseObject_Verify：error の応答も iss・aud・exp を照合することにした（#644）
//**********************************************************************************

using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace TestFramework.Authentication
{
    /// <summary>CmnJwtToken / AccessToken / IdToken / ResponseObject の単体テスト</summary>
    /// <remarks>
    /// **検証鍵・iss・aud を引数で受ける版だけを使う**（#601 / #603）。
    /// 設定（JwkSetFilePath・ClientID 等）で判定する版は、配備の設定に依るので対象外（Golden の TestCode が見ている）。
    /// 相手（OP）の鍵は、テストの中で生成する RS256 の鍵。
    /// </remarks>
    [TestFixture]
    public class JwtTokenTests
    {
        private const string Iss = "https://op.example.com/";
        private const string Aud = "client01";

        /// <summary>相手（OP）の署名鍵と、その公開鍵だけの JWS</summary>
        private JWS_RS256_Param _op;
        private JWS _opPublic;
        private JWS _otherPublic;

        [SetUp]
        public void SetUp()
        {
            this._op = new JWS_RS256_Param();
            this._opPublic = new JWS_RS256_Param(this._op.RsaPublicParameters);
            this._otherPublic = new JWS_RS256_Param(new JWS_RS256_Param().RsaPublicParameters);
        }

        /// <summary>ペイロードを作って署名する（exp は 1 時間後、expired なら 1 時間前）</summary>
        private string Sign(JObject payload, bool expired = false)
        {
            payload[OAuth2AndOIDCConst.exp] = DateTimeOffset.Now.AddHours(expired ? -1 : 1).ToUnixTimeSeconds().ToString();
            return this._op.Create(payload.ToString(Formatting.None));
        }

        #region CmnJwtToken

        /// <summary>クレームの値を作る（jti は GUID、iat・exp は UNIX 秒）。exp・nbf の判定。</summary>
        [Test]
        [Basis("RFC 7519 4.1.4（exp 以後は受けない）・4.1.5（nbf より前は受けない）・4.1.6（iat は NumericDate）")]
        public void CmnJwtToken_Claims()
        {
            Assert.That(CmnJwtToken.CreateJitClaim(), Has.Length.EqualTo(32));
            Assert.That(CmnJwtToken.CreateJitClaim(), Is.Not.EqualTo(CmnJwtToken.CreateJitClaim()));

            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            Assert.That(long.Parse(CmnJwtToken.CreateIatClaim()), Is.InRange(now - 5, now + 5));
            Assert.That(long.Parse(CmnJwtToken.CreateExpClaim(TimeSpan.FromHours(1))), Is.InRange(now + 3600 - 5, now + 3600 + 5));

            Assert.That(CmnJwtToken.VerifyExp(CmnJwtToken.CreateExpClaim(TimeSpan.FromMinutes(1))), Is.True);
            Assert.That(CmnJwtToken.VerifyExp(CmnJwtToken.CreateExpClaim(TimeSpan.FromMinutes(-1))), Is.False);
            Assert.That(CmnJwtToken.VerifyNbf(CmnJwtToken.CreateExpClaim(TimeSpan.FromMinutes(-1))), Is.True);
            Assert.That(CmnJwtToken.VerifyNbf(CmnJwtToken.CreateExpClaim(TimeSpan.FromMinutes(1))), Is.False);
        }

        /// <summary>CheckClaims：キーがあれば値を返す。無ければ、nullable なら true、そうでなければ false と server_error。</summary>
        [Test]
        public void CmnJwtToken_CheckClaims()
        {
            JObject json = JObject.Parse("{\"iss\":\"x\"}");
            string value, err, desc;

            Assert.That(CmnJwtToken.CheckClaims(json, "iss", out value, out err, out desc), Is.True);
            Assert.That(value, Is.EqualTo("x"));

            Assert.That(CmnJwtToken.CheckClaims(json, "aud", out value, out err, out desc, true), Is.True);
            Assert.That(err, Is.Empty);

            Assert.That(CmnJwtToken.CheckClaims(json, "aud", out value, out err, out desc), Is.False);
            Assert.That(err, Is.EqualTo("server_error"));
            Assert.That(desc, Does.Contain("aud"));
        }

        /// <summary>Verify（jws を受ける版）：渡した鍵で検証し、ペイロードを返す。別の鍵では通らない。</summary>
        [Test]
        [Basis("RFC 7515 5.2 の手順 8・10（署名を検証し、通らなければ無効）")]
        public void CmnJwtToken_Verify()
        {
            string jwt = this.Sign(new JObject { [OAuth2AndOIDCConst.sub] = "user01" });
            string payload;

            Assert.That(CmnJwtToken.Verify(jwt, this._opPublic, out payload), Is.True);
            Assert.That((string)JObject.Parse(payload)[OAuth2AndOIDCConst.sub], Is.EqualTo("user01"));

            Assert.That(CmnJwtToken.Verify(jwt, this._otherPublic, out payload), Is.False);
            Assert.That(payload, Is.Empty);
        }

        /// <summary>Verify：JWE でない（3 つに分かれる）のに RS256 以外なら NotSupportedException。</summary>
        [Test]
        public void CmnJwtToken_Verify_UnexpectedAlg()
        {
            string hs256 = new JWS_HS256(new byte[32]).Create("{\"sub\":\"x\"}");
            string payload;

            Assert.Throws<NotSupportedException>(() => CmnJwtToken.Verify(hs256, this._opPublic, out payload));
        }

        #endregion

        #region AccessToken

        /// <summary>
        /// AccessToken.Verify（jws / iss / auds）：sub・scopes・roles を返す。aud は文字列でも配列でもよい。
        /// iss・aud・鍵・exp のどれかが違えば通らない。
        /// </summary>
        [Test]
        [Basis("RFC 9068 4（iss が一致し、aud が含まれ、署名が正しく、exp より前であること。typ の照合はこのテストの対象外）")]
        [Basis("RFC 7519 4.1.3（aud は配列でもよい）")]
        public void AccessToken_Verify()
        {
            JObject p = new JObject
            {
                [OAuth2AndOIDCConst.iss] = Iss,
                [OAuth2AndOIDCConst.aud] = Aud,
                [OAuth2AndOIDCConst.sub] = "user01",
                [OAuth2AndOIDCConst.scopes] = new JArray("openid", "profile"),
                [OAuth2AndOIDCConst.Scope_Roles] = new JArray("admin")
            };
            string at = this.Sign(p);

            string sub; List<string> roles, scopes; JObject jobj;
            Assert.That(AccessToken.Verify(at, this._opPublic, Iss, new[] { Aud }, out sub, out roles, out scopes, out jobj), Is.True);
            Assert.That(sub, Is.EqualTo("user01"));
            Assert.That(scopes, Is.EqualTo(new[] { "openid", "profile" }));
            Assert.That(roles, Is.EqualTo(new[] { "admin" }));

            // aud が配列（含まれていればよい）
            p[OAuth2AndOIDCConst.aud] = new JArray("other", Aud);
            Assert.That(AccessToken.Verify(this.Sign(p), this._opPublic, Iss, new[] { Aud }, out sub, out roles, out scopes, out jobj), Is.True);

            Assert.That(AccessToken.Verify(at, this._otherPublic, Iss, new[] { Aud }, out sub, out roles, out scopes, out jobj), Is.False, "別の鍵");
            Assert.That(AccessToken.Verify(at, this._opPublic, "https://other/", new[] { Aud }, out sub, out roles, out scopes, out jobj), Is.False, "iss");
            Assert.That(AccessToken.Verify(at, this._opPublic, Iss, new[] { "client02" }, out sub, out roles, out scopes, out jobj), Is.False, "aud");

            p[OAuth2AndOIDCConst.aud] = Aud;
            Assert.That(AccessToken.Verify(this.Sign(p, true), this._opPublic, Iss, new[] { Aud }, out sub, out roles, out scopes, out jobj), Is.False, "exp");

            p.Remove(OAuth2AndOIDCConst.aud);
            Assert.That(AccessToken.Verify(this.Sign(p), this._opPublic, Iss, new[] { Aud }, out sub, out roles, out scopes, out jobj), Is.False, "aud が無い");
        }

        /// <summary>期待値（iss・auds）を渡さないのは誤用（設定に落とさず、ArgumentNullException）。</summary>
        [Test]
        public void AccessToken_Verify_NoExpectation_Throws()
        {
            string sub; List<string> roles, scopes; JObject jobj;
            Assert.Throws<ArgumentNullException>(() => AccessToken.Verify("x", this._opPublic, "", new[] { Aud }, out sub, out roles, out scopes, out jobj));
            Assert.Throws<ArgumentNullException>(() => AccessToken.Verify("x", this._opPublic, Iss, new string[0], out sub, out roles, out scopes, out jobj));
        }

        #endregion

        #region IdToken

        /// <summary>
        /// IdToken.CreateHash は SHA-256 の左半分を base64url にした値（OpenID Connect Core 1.0 付録 A の例）。
        /// VerifyHash はその値と照合する。
        /// </summary>
        [Test]
        [Basis("OpenID Connect Core 1.0 3.1.3.6（at_hash）・付録 A.3（access_token と at_hash の例）")]
        public void IdToken_Hash()
        {
            Assert.That(IdToken.CreateHash("jHkWEdUXMU1BwAsC4vtUsZwnNvTIxEl0z9K3vx5KF0Y"), Is.EqualTo("77QmUPtjPfzWtF2AnpK9RQ"));
            Assert.That(IdToken.VerifyHash("jHkWEdUXMU1BwAsC4vtUsZwnNvTIxEl0z9K3vx5KF0Y", "77QmUPtjPfzWtF2AnpK9RQ"), Is.True);
            Assert.That(IdToken.VerifyHash("other", "77QmUPtjPfzWtF2AnpK9RQ"), Is.False);
        }

        /// <summary>
        /// IdToken.Verify（jws / iss / aud）：sub・nonce を返す。at_hash・c_hash・s_hash があれば照合する。
        /// </summary>
        [Test]
        [Basis("OIDC Core 3.1.3.7 の 2・3・6・9（iss の一致・aud に client_id・署名・exp）")]
        [Basis("OIDC Core 3.2.2.9（at_hash）・3.3.2.10（c_hash）")]
        public void IdToken_Verify()
        {
            JObject p = new JObject
            {
                [OAuth2AndOIDCConst.iss] = Iss,
                [OAuth2AndOIDCConst.aud] = Aud,
                [OAuth2AndOIDCConst.sub] = "user01",
                [OAuth2AndOIDCConst.nonce] = "nonce01",
                [OAuth2AndOIDCConst.at_hash] = IdToken.CreateHash("at01"),
                [OAuth2AndOIDCConst.c_hash] = IdToken.CreateHash("code01"),
                [OAuth2AndOIDCConst.s_hash] = IdToken.CreateHash("state01")
            };
            string idToken = this.Sign(p);

            string sub, nonce; JObject jobj;
            Assert.That(IdToken.Verify(idToken, "at01", "code01", "state01", this._opPublic, Iss, Aud, out sub, out nonce, out jobj), Is.True);
            Assert.That(sub, Is.EqualTo("user01"));
            Assert.That(nonce, Is.EqualTo("nonce01"));

            // ハッシュの元が無ければ、照合しない
            Assert.That(IdToken.Verify(idToken, "", "", "", this._opPublic, Iss, Aud, out sub, out nonce, out jobj), Is.True);

            Assert.That(IdToken.Verify(idToken, "at02", "code01", "state01", this._opPublic, Iss, Aud, out sub, out nonce, out jobj), Is.False, "at_hash");
            Assert.That(IdToken.Verify(idToken, "at01", "code02", "state01", this._opPublic, Iss, Aud, out sub, out nonce, out jobj), Is.False, "c_hash");
            Assert.That(IdToken.Verify(idToken, "at01", "code01", "state02", this._opPublic, Iss, Aud, out sub, out nonce, out jobj), Is.False, "s_hash");
            Assert.That(IdToken.Verify(idToken, "", "", "", this._otherPublic, Iss, Aud, out sub, out nonce, out jobj), Is.False, "別の鍵");
            Assert.That(IdToken.Verify(idToken, "", "", "", this._opPublic, "https://other/", Aud, out sub, out nonce, out jobj), Is.False, "iss");
            Assert.That(IdToken.Verify(idToken, "", "", "", this._opPublic, Iss, "client02", out sub, out nonce, out jobj), Is.False, "aud");

            // aud が配列
            p[OAuth2AndOIDCConst.aud] = new JArray("other", Aud);
            Assert.That(IdToken.Verify(this.Sign(p), "", "", "", this._opPublic, Iss, Aud, out sub, out nonce, out jobj), Is.True);

            Assert.That(IdToken.Verify(this.Sign(p, true), "", "", "", this._opPublic, Iss, Aud, out sub, out nonce, out jobj), Is.False, "exp");

            Assert.Throws<ArgumentNullException>(() => IdToken.Verify(idToken, "", "", "", this._opPublic, "", Aud, out sub, out nonce, out jobj));
            Assert.Throws<ArgumentNullException>(() => IdToken.Verify(idToken, "", "", "", this._opPublic, Iss, "", out sub, out nonce, out jobj));
        }

        #endregion

        #region ResponseObject

        /// <summary>
        /// ResponseObject.Verify（jws / iss / aud）：iss・aud・exp を照合する。
        /// **error を持つ応答も、iss・aud・exp を照合する**（通れば true で、エラーの内容は jobj で受け取る）。
        /// 2026/10/30 まで、error を持つ応答は、署名が正しければ iss・aud・exp を見ずに true だった（#644）。
        /// false でも、署名が正しければ jobj を返す。exp が無い応答は、例外にせず false。
        /// </summary>
        [Test]
        [Basis("JARM 2.1（JWT は常に iss・aud・exp を持つ）・2.1.1（error の応答の例にも iss・aud・exp がある）")]
        [Basis("JARM 2.4 の 2・3・4（iss・aud・exp が合わなければ、応答を断る）")]
        [Basis("#644")]
        public void ResponseObject_Verify()
        {
            JObject p = new JObject
            {
                [OAuth2AndOIDCConst.iss] = Iss,
                [OAuth2AndOIDCConst.aud] = Aud,
                [OAuth2AndOIDCConst.code] = "code01"
            };
            string response = this.Sign(p);
            JObject jobj;

            Assert.That(ResponseObject.Verify(response, this._opPublic, Iss, Aud, out jobj), Is.True);
            Assert.That((string)jobj[OAuth2AndOIDCConst.code], Is.EqualTo("code01"));

            Assert.That(ResponseObject.Verify(response, this._otherPublic, Iss, Aud, out jobj), Is.False, "別の鍵");
            Assert.That(ResponseObject.Verify(response, this._opPublic, "https://other/", Aud, out jobj), Is.False, "iss");
            Assert.That(ResponseObject.Verify(response, this._opPublic, Iss, "client02", out jobj), Is.False, "aud");
            Assert.That(ResponseObject.Verify(this.Sign(p, true), this._opPublic, Iss, Aud, out jobj), Is.False, "exp");

            // exp が無い応答は、例外にせず false
            JObject noExp = new JObject
            {
                [OAuth2AndOIDCConst.iss] = Iss,
                [OAuth2AndOIDCConst.aud] = Aud,
                [OAuth2AndOIDCConst.code] = "code01"
            };
            Assert.That(ResponseObject.Verify(this._op.Create(noExp.ToString(Formatting.None)), this._opPublic, Iss, Aud, out jobj), Is.False, "exp が無い");

            // error の応答も、iss・aud・exp を照合する（#644）
            JObject error = new JObject
            {
                [OAuth2AndOIDCConst.iss] = Iss,
                [OAuth2AndOIDCConst.aud] = Aud,
                [OAuth2AndOIDCConst.error] = "access_denied"
            };
            Assert.That(ResponseObject.Verify(this.Sign(error), this._opPublic, Iss, Aud, out jobj), Is.True, "error の応答");
            Assert.That((string)jobj[OAuth2AndOIDCConst.error], Is.EqualTo("access_denied"));

            Assert.That(ResponseObject.Verify(this.Sign(error), this._opPublic, Iss, "client02", out jobj), Is.False, "error の応答：aud が違う");
            Assert.That((string)jobj[OAuth2AndOIDCConst.error], Is.EqualTo("access_denied"), "false でも、署名が正しければ jobj を返す");
            Assert.That(ResponseObject.Verify(this.Sign(error), this._opPublic, "https://other/", Aud, out jobj), Is.False, "error の応答：iss が違う");
            Assert.That(ResponseObject.Verify(this.Sign(error, true), this._opPublic, Iss, Aud, out jobj), Is.False, "error の応答：exp");

            JObject bareError = new JObject { [OAuth2AndOIDCConst.error] = "access_denied" };
            Assert.That(ResponseObject.Verify(this.Sign(bareError), this._opPublic, Iss, Aud, out jobj), Is.False, "error の応答：iss・aud が無い");
            Assert.That(ResponseObject.Verify(this.Sign(bareError), this._otherPublic, Iss, Aud, out jobj), Is.False, "error の応答：別の鍵");
            Assert.That(jobj, Is.Null, "署名が通らなければ jobj は null");

            Assert.Throws<ArgumentNullException>(() => ResponseObject.Verify(response, this._opPublic, "", Aud, out jobj));
            Assert.Throws<ArgumentNullException>(() => ResponseObject.Verify(response, this._opPublic, Iss, "", out jobj));
        }

        #endregion
    }
}

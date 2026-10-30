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
//* クラス名        ：IdPSideTests
//* クラス日本語名  ：SP・RP 側と対になる IdP 側の処理の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（#634）
//*  2026/10/22  玄人 幸道         設定（CmnServerParams）を読む版のテストを追加
//*  2026/10/22  玄人 幸道         境界（鍵の並び・空の issuer・aud の配列・JwkSetDocument の設定とファイル無し）のテストを足した
//*  2026/10/29  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）を広げた（#640）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using TestCommon;

#if NETCOREAPP
using Microsoft.Extensions.Configuration;

using Touryo.Infrastructure.Public.Util;
#endif

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace TestFramework.Authentication
{
    /// <summary>SP・RP 側と対になる IdP 側の処理の単体テスト</summary>
    /// <remarks>
    /// 対になる処理同士で往復させて確かめる
    /// （SP が作った要求を IdP が検証する、IdP が作った応答を SP が検証する、など）。
    /// </remarks>
    [TestFixture]
    public class IdPSideTests
    {
        private const string SpEntityId = "https://sp.example.com/";
        private const string IdPEntityId = SAML2BindingsTests.Issuer;
        private const string Acs = SAML2BindingsTests.Recipient;

        /// <summary>SHA256 の DigitalSign（秘密鍵）</summary>
        private static DigitalSign Signer(RSA rsa)
        {
            return new DigitalSignParam(rsa.ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        #region SAML2 : 要求の検証（IdP 側）

        /// <summary>
        /// SP が作った要求（Redirect / POST）を、IdP が要求の Issuer から選んだ鍵で検証できる。
        /// iss と ID（応答の InResponseTo に使う）が返る。
        /// </summary>
        [Test]
        [Basis("設計：SAML2Client.VerifyRequest（鍵を Issuer から返す処理を受ける版）の remarks（要求の Issuer から鍵を引き、複数なら署名が通るまで順に試す）")]
        public void VerifyRequest_RoundTrip()
        {
            using (RSA sp = RSA.Create(2048))
            using (RSA other = RSA.Create(2048))
            {
                Func<string, IEnumerable<RSA>> keysOf = iss => iss == SpEntityId ? new RSA[] { other, sp } : null;

                string id, iss, reqId;
                XmlDocument doc;

                string query = SAML2Client.CreateRedirectRequest(SAML2Enum.RequestOrResponse.Request,
                    SAML2Enum.ProtocolBinding.HttpRedirect, SAML2Enum.NameIDFormat.Unspecified,
                    SpEntityId, Acs, "relay01", Signer(sp), out id);

                Assert.That(SAML2Client.VerifyRequest(query, null, keysOf, out iss, out reqId, out doc), Is.True);
                Assert.That(iss, Is.EqualTo(SpEntityId));
                Assert.That(reqId, Is.EqualTo(id));

                string post = SAML2Client.CreatePostRequest(SAML2Enum.ProtocolBinding.HttpPost, SAML2Enum.NameIDFormat.Unspecified,
                    SpEntityId, Acs, "relay01", sp, out id);

                Assert.That(SAML2Client.VerifyRequest(null, post, keysOf, out iss, out reqId, out doc), Is.True);
                Assert.That(reqId, Is.EqualTo(id));

                // 鍵を固定で渡す版
                Assert.That(SAML2Client.VerifyRequest(null, post, new RSA[] { sp }, out iss, out reqId, out doc), Is.True);
            }
        }

        /// <summary>
        /// 別の鍵・鍵が無い SP・壊れた要求・空の要求は false（例外にしない）。
        /// 鍵を返す処理が null なら ArgumentNullException。
        /// </summary>
        [Test]
        [Basis("設計：SAML2Client.VerifyRequest（鍵を Issuer から返す処理を受ける版）の remarks（読めなくても例外にせず false。false のときの iss は検証されていない値）")]
        public void VerifyRequest_Rejected()
        {
            using (RSA sp = RSA.Create(2048))
            using (RSA other = RSA.Create(2048))
            {
                string id, iss, reqId;
                XmlDocument doc;
                string post = SAML2Client.CreatePostRequest(SAML2Enum.ProtocolBinding.HttpPost, SAML2Enum.NameIDFormat.Unspecified,
                    SpEntityId, Acs, "relay01", sp, out id);

                Assert.That(SAML2Client.VerifyRequest(null, post, new RSA[] { other }, out iss, out reqId, out doc), Is.False, "別の鍵");
                Assert.That(iss, Is.EqualTo(SpEntityId), "切り分けのために iss は返る");
                Assert.That(SAML2Client.VerifyRequest(null, post, x => null, out iss, out reqId, out doc), Is.False, "鍵が無い SP は通さない");
                Assert.That(SAML2Client.VerifyRequest(null, "!!!", new RSA[] { sp }, out iss, out reqId, out doc), Is.False, "壊れた要求");
                Assert.That(SAML2Client.VerifyRequest(null, null, new RSA[] { sp }, out iss, out reqId, out doc), Is.False, "空");

                Assert.Throws<ArgumentNullException>(() => SAML2Client.VerifyRequest(
                    null, post, (Func<string, IEnumerable<RSA>>)null, out iss, out reqId, out doc));
            }
        }

        #endregion

        #region SAML2 : 応答の作成（IdP 側）

        /// <summary>
        /// IdP が作った応答（Redirect / POST）を、SP が検証できる。属性も読める。
        /// </summary>
        [Test]
        public void CreateResponse_RoundTrip()
        {
            List<Tuple<string, string, string, string>> attributes = new List<Tuple<string, string, string, string>>
            {
                Tuple.Create(SAML2Const.UrnOidMail, SAML2Const.FriendlyNameMail, SAML2Const.UrnAttrNameFormatUri, "user01@example.com")
            };

            RSA idp = TestCertificates.RsaPrivateKey();
            RSA[] idpPublic = new RSA[] { TestCertificates.RsaPublicKey() };

            string query = SAML2Client.CreateRedirectResponse(IdPEntityId, Acs, "req01", SAML2Enum.StatusCode.Success,
                "user01", SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.PasswordProtectedTransport,
                3600, attributes, "relay01", Signer(idp));

            SAML2VerifyResult r = SAML2Client.VerifyResponseWithReason(query, null, idpPublic,
                IdPEntityId, Acs, "req01", "relay01", "relay01");
            Assert.That(r.IsValid, Is.True, r.ReasonDescription);
            Assert.That(r.NameId, Is.EqualTo("user01"));
            Assert.That(SAML2Attributes.GetEmail(r.Attributes), Is.EqualTo("user01@example.com"));

            string post = SAML2Client.CreatePostResponse(IdPEntityId, Acs, "req01", SAML2Enum.StatusCode.Success,
                "user01", SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.PasswordProtectedTransport,
                3600, null, idp);

            SAML2VerifyResult r2 = SAML2Client.VerifyResponseWithReason(null, post, idpPublic,
                IdPEntityId, Acs, "req01", null, null);
            Assert.That(r2.IsValid, Is.True, r2.ReasonDescription);
            Assert.That(r2.Attributes, Is.Empty, "属性を渡さなければ出さない");
        }

        /// <summary>エラー応答は Assertion を持たない（SP 側では ErrorStatus）。鍵・必須の値が無ければ例外。</summary>
        [Test]
        [Basis("SAML 2.0 Profiles 4.1.4.2（エラーを返すなら Assertion を含めない）")]
        public void CreateResponse_ErrorAndArguments()
        {
            RSA idp = TestCertificates.RsaPrivateKey();

            string post = SAML2Client.CreatePostResponse(IdPEntityId, Acs, "req01", SAML2Enum.StatusCode.Requester,
                "", SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.Unspecified, 3600, null, idp);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(SAML2Bindings.DecodePost(post));
            Assert.That(doc.GetElementsByTagName(SAML2Const.ElementAssertion, SAML2Const.UrnAssertion).Count, Is.EqualTo(0));

            Assert.That(SAML2Client.VerifyResponseWithReason(null, post, new RSA[] { TestCertificates.RsaPublicKey() },
                IdPEntityId, Acs, "req01", null, null).Reason, Is.EqualTo(SAML2VerifyResult.Reasons.ErrorStatus));

            Assert.Throws<ArgumentNullException>(() => SAML2Client.CreatePostResponse(IdPEntityId, Acs, "req01",
                SAML2Enum.StatusCode.Success, "u", SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.Unspecified,
                3600, null, null));
            Assert.Throws<ArgumentNullException>(() => SAML2Client.CreateRedirectResponse(IdPEntityId, Acs, "req01",
                SAML2Enum.StatusCode.Success, "u", SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.Unspecified,
                3600, null, "", null));
            Assert.Throws<ArgumentNullException>(() => SAML2Client.CreatePostResponse(IdPEntityId, "", "req01",
                SAML2Enum.StatusCode.Success, "u", SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.Unspecified,
                3600, null, idp), "ACS URL が無い");
        }

        #endregion

        #region SAML2 : メタデータに署名用の証明書を書き足す

        /// <summary>書き足した証明書は、いまの鍵の後ろに、渡した順で並び、SP 側で全部読める。空は飛ばす。</summary>
        [Test]
        [Basis("設計：SAML2Bindings.AppendSigningCertificates(metadata, certificates) の remarks（1 本目の後ろに、渡された順に並べる）")]
        public void AppendSigningCertificates()
        {
            XmlDocument metadata = SAML2Bindings.CreateMetadata(IdPEntityId, "CURRENT",
                new SAML2Enum.NameIDFormat[] { SAML2Enum.NameIDFormat.Unspecified },
                "https://idp.example.com/sso", "https://idp.example.com/sso");

            Assert.That(SAML2Bindings.AppendSigningCertificates(metadata, new string[] { "OLD1", "", "OLD2" }), Is.EqualTo(2));

            SAML2Metadata md = SAML2Metadata.Parse(metadata.OuterXml);
            Assert.That(md.SigningCertificates, Is.EqualTo(new List<string> { "CURRENT", "OLD1", "OLD2" }));

            Assert.That(SAML2Bindings.AppendSigningCertificates(metadata, null), Is.EqualTo(0));
            Assert.Throws<ArgumentNullException>(() => SAML2Bindings.AppendSigningCertificates(null, new string[] { "X" }));

            XmlDocument noKey = new XmlDocument();
            noKey.LoadXml("<md:EntityDescriptor xmlns:md=\"" + SAML2Const.UrnMetadata + "\" entityID=\"x\"/>");
            Assert.Throws<ArgumentException>(() => SAML2Bindings.AppendSigningCertificates(noKey, new string[] { "X" }), "署名用の KeyDescriptor が無い");
        }

        #endregion

        #region ResponseObject（JARM）

        /// <summary>Create で作った ResponseObject を、Verify（鍵・iss・aud を受ける版）で検証できる。exp は数値で入る。</summary>
        [Test]
        [Basis("JARM 2.1（iss・aud・exp を持つ）・2.4 の 3・4（aud が client_id と一致し、exp が過ぎていなければ受ける）")]
        public void ResponseObject_RoundTrip()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                JWS signer = new JWS_RS256_Param(rsa.ExportParameters(true));
                JWS verifier = new JWS_RS256_Param(rsa.ExportParameters(false));
                Dictionary<string, string> p = new Dictionary<string, string> { { "code", "c1" }, { "state", "s1" } };

                string response = ResponseObject.Create(p, signer, "https://as.example.com", "client01", DateTimeOffset.UtcNow.AddMinutes(5));

                JObject jobj;
                Assert.That(ResponseObject.Verify(response, verifier, "https://as.example.com", "client01", out jobj), Is.True);
                Assert.That((string)jobj["code"], Is.EqualTo("c1"));
                Assert.That(jobj["exp"].Type, Is.EqualTo(JTokenType.Integer));
                Assert.That(p.ContainsKey("iss"), Is.False, "引数は変えない");

                Assert.That(ResponseObject.Verify(response, verifier, "https://as.example.com", "client02", out jobj), Is.False, "aud が違う");

                string expired = ResponseObject.Create(p, signer, "https://as.example.com", "client01", DateTimeOffset.UtcNow.AddMinutes(-5));
                Assert.That(ResponseObject.Verify(expired, verifier, "https://as.example.com", "client01", out jobj), Is.False, "期限切れ");

                Assert.Throws<ArgumentNullException>(() => ResponseObject.Create(p, null, "i", "a", DateTimeOffset.UtcNow));
                Assert.Throws<ArgumentNullException>(() => ResponseObject.Create(p, signer, "", "a", DateTimeOffset.UtcNow));
                Assert.Throws<ArgumentNullException>(() => ResponseObject.Create(p, signer, "i", "", DateTimeOffset.UtcNow));
            }
        }

        #endregion

        #region PKCE

        /// <summary>S256 はクライアントの計算と一致すれば通る。plain は許したときだけ。未知のメソッド・空は通さない。</summary>
        [Test]
        public void VerifyPkce()
        {
            const string verifier = "dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk";
            string challenge = OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod(verifier);
            Assert.That(challenge, Is.EqualTo("E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM"), "RFC 7636 付録 B の例");

            bool usedS256;
            Assert.That(OAuth2AndOIDCSpec.VerifyPkce("S256", challenge, verifier, false, out usedS256), Is.True);
            Assert.That(usedS256, Is.True);
            Assert.That(OAuth2AndOIDCSpec.VerifyPkce("s256", challenge, verifier, false, out usedS256), Is.True, "大文字小文字は区別しない");
            Assert.That(OAuth2AndOIDCSpec.VerifyPkce("S256", challenge, verifier + "x", false, out usedS256), Is.False);

            Assert.That(OAuth2AndOIDCSpec.VerifyPkce("plain", verifier, verifier, true, out usedS256), Is.True);
            Assert.That(usedS256, Is.False);
            Assert.That(OAuth2AndOIDCSpec.VerifyPkce("plain", verifier, verifier, false, out usedS256), Is.False, "plain を許さない");

            Assert.That(OAuth2AndOIDCSpec.VerifyPkce("S512", challenge, verifier, true, out usedS256), Is.False, "未知のメソッド");
            Assert.That(OAuth2AndOIDCSpec.VerifyPkce("", challenge, verifier, true, out usedS256), Is.False, "PKCE を使っていない");
            Assert.That(OAuth2AndOIDCSpec.VerifyPkce("S256", challenge, "", true, out usedS256), Is.False, "code_verifier が無い");
        }

        #endregion

        #region JwkSetDocument

        /// <summary>
        /// 本文と ETag を返す。同じ中身なら同じ ETag。ファイルが変われば読み直す（鍵の入れ替えに追随する）。
        /// パスが空なら ArgumentNullException。
        /// </summary>
        [Test]
        public void JwkSetDocument_FollowsFileChange()
        {
            string path = Path.Combine(Path.GetTempPath(), "jwkset_" + Guid.NewGuid().ToString("N") + ".json");

            try
            {
                File.WriteAllText(path, "{\"keys\":[]}");

                string etag1, etag2;
                Assert.That(JwkSetDocument.Get(path, out etag1), Is.EqualTo("{\"keys\":[]}"));
                Assert.That(etag1, Does.StartWith("\"").And.EndWith("\""));
                JwkSetDocument.Get(path, out etag2);
                Assert.That(etag2, Is.EqualTo(etag1), "同じ中身なら同じ ETag");

                File.WriteAllText(path, "{\"keys\":[{\"kty\":\"RSA\"}]}");
                File.SetLastWriteTimeUtc(path, DateTime.UtcNow.AddMinutes(1));

                Assert.That(JwkSetDocument.Get(path, out etag2), Does.Contain("RSA"), "変われば読み直す");
                Assert.That(etag2, Is.Not.EqualTo(etag1));

                Assert.Throws<ArgumentNullException>(() => JwkSetDocument.Get("", out etag1));
            }
            finally
            {
                File.Delete(path);
            }
        }

        #endregion

        #region 境界

        /// <summary>
        /// VerifyRequest（鍵を固定で渡す版）：鍵の並びが null なら ArgumentNullException。
        /// 並びの中の null は飛ばし、次の鍵を試す。
        /// </summary>
        [Test]
        public void VerifyRequest_KeyList()
        {
            using (RSA sp = RSA.Create(2048))
            {
                string id, iss, reqId;
                XmlDocument doc;
                string post = SAML2Client.CreatePostRequest(SAML2Enum.ProtocolBinding.HttpPost, SAML2Enum.NameIDFormat.Unspecified,
                    SpEntityId, Acs, "relay01", sp, out id);

                Assert.That(SAML2Client.VerifyRequest(null, post, new RSA[] { null, sp }, out iss, out reqId, out doc), Is.True,
                    "null は飛ばす");
                Assert.That(reqId, Is.EqualTo(id));

                Assert.Throws<ArgumentNullException>(() =>
                    SAML2Client.VerifyRequest(null, post, (IEnumerable<RSA>)null, out iss, out reqId, out doc));
            }
        }

        /// <summary>CreatePostResponse / CreateRedirectResponse：issuer が空なら ArgumentNullException。</summary>
        [Test]
        public void CreateResponse_EmptyIssuer()
        {
            RSA idp = TestCertificates.RsaPrivateKey();

            Assert.Throws<ArgumentNullException>(() => SAML2Client.CreatePostResponse("", Acs, "req01",
                SAML2Enum.StatusCode.Success, "u", SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.Unspecified,
                3600, null, idp));
            Assert.Throws<ArgumentNullException>(() => SAML2Client.CreateRedirectResponse(null, Acs, "req01",
                SAML2Enum.StatusCode.Success, "u", SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.Unspecified,
                3600, null, "", Signer(idp)));
        }

        /// <summary>
        /// ResponseObject：responseParams が null なら ArgumentNullException。
        /// aud が配列の ResponseObject は、配列に期待する aud を含めば通る（RFC 7519 4.1.3）。
        /// </summary>
        [Test]
        [Basis("RFC 7519 4.1.3（aud は配列でもよい。自分を含まなければ拒否する）")]
        public void ResponseObject_Boundaries()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                JWS signer = new JWS_RS256_Param(rsa.ExportParameters(true));
                JWS verifier = new JWS_RS256_Param(rsa.ExportParameters(false));

                Assert.Throws<ArgumentNullException>(() =>
                    ResponseObject.Create(null, signer, "https://as.example.com", "client01", DateTimeOffset.UtcNow));

                string arrayAud = signer.Create(new JObject(
                    new JProperty("code", "c1"),
                    new JProperty("iss", "https://as.example.com"),
                    new JProperty("aud", new JArray("other", "client01")),
                    new JProperty("exp", DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds())).ToString());

                JObject jobj;
                Assert.That(ResponseObject.Verify(arrayAud, verifier, "https://as.example.com", "client01", out jobj), Is.True);
                Assert.That(ResponseObject.Verify(arrayAud, verifier, "https://as.example.com", "client02", out jobj), Is.False);
            }
        }

        /// <summary>JwkSetDocument：ファイルが無ければ例外（設定の誤りを黙って通さない）。</summary>
        [Test]
        public void JwkSetDocument_MissingFile()
        {
            string etag;
            string path = Path.Combine(Path.GetTempPath(), "missing_" + Guid.NewGuid().ToString("N") + ".json");

            Assert.That(() => JwkSetDocument.Get(path, out etag), Throws.Exception);
        }

#if NETCOREAPP
        /// <summary>JwkSetDocument（設定を読む版）：JwkSetFilePath のファイルを返す（net10.0 だけ。構成を差し替える）。</summary>
        [Test]
        public void JwkSetDocument_ReadsConfig()
        {
            string path = Path.Combine(Path.GetTempPath(), "jwkset_" + Guid.NewGuid().ToString("N") + ".json");

            try
            {
                File.WriteAllText(path, "{\"keys\":[]}");

                WithConfig(new Dictionary<string, string> { { "JwkSetFilePath", path } }, () =>
                {
                    string etag;
                    Assert.That(JwkSetDocument.Get(out etag), Is.EqualTo("{\"keys\":[]}"));
                    Assert.That(etag, Is.Not.Empty);
                });
            }
            finally
            {
                File.Delete(path);
            }
        }
#endif

        #endregion

        #region 設定（CmnServerParams）を読む版

        /// <summary>
        /// 設定が空のとき：必須の値（IssuerId 等）が無ければ FrameworkException。
        /// RequirePkceS256 が無ければ plain も受ける。PreviousSigningCerFilePaths が無ければ何も足さない。
        /// </summary>
        [Test]
        [Basis("設計：SAML2Client.CreateRedirectResponse・CreatePostResponse（設定を読む版）の remarks（CmnServerParams の値を使う。どれかが無ければ FrameworkException）")]
        public void ConfigOverloads_EmptyConfig()
        {
            Dictionary<string, string> p = new Dictionary<string, string> { { "code", "c1" } };

            Assert.Throws<FrameworkException>(() => ResponseObject.Create(p, "client01", DateTimeOffset.UtcNow.AddMinutes(5)));
            Assert.Throws<FrameworkException>(() => SAML2Client.CreatePostResponse(Acs, "req01", SAML2Enum.StatusCode.Success,
                "user01", SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.Unspecified, null));
            Assert.Throws<FrameworkException>(() => SAML2Client.CreateRedirectResponse(Acs, "req01", SAML2Enum.StatusCode.Success,
                "user01", SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.Unspecified, null, "relay01"));

            const string verifier = "dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk";
            bool usedS256;
            Assert.That(OAuth2AndOIDCSpec.VerifyPkce("plain", verifier, verifier, out usedS256), Is.True, "既定は plain も受ける");

            XmlDocument metadata = SAML2Bindings.CreateMetadata(IdPEntityId, "CURRENT",
                new SAML2Enum.NameIDFormat[] { SAML2Enum.NameIDFormat.Unspecified },
                "https://idp.example.com/sso", "https://idp.example.com/sso");
            Assert.That(SAML2Bindings.AppendSigningCertificates(metadata), Is.EqualTo(0));
            Assert.Throws<ArgumentNullException>(() => SAML2Bindings.AppendSigningCertificates(null));
        }

#if NETCOREAPP
        /// <summary>構成を差し替えて、テストの後で空に戻す</summary>
        private static void WithConfig(Dictionary<string, string> appSettings, Action action)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> kv in appSettings)
            {
                values.Add("appSettings:" + kv.Key, kv.Value);
            }

            try
            {
                GetConfigParameter.InitConfiguration(new ConfigurationBuilder().AddInMemoryCollection(values).Build());
                action();
            }
            finally
            {
                GetConfigParameter.InitConfiguration(
                    new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()).Build());
            }
        }

        /// <summary>汎用認証サイトと同じキーの、IdP / 認可サーバの設定</summary>
        private static Dictionary<string, string> ServerConfig()
        {
            return new Dictionary<string, string>
            {
                { "IssuerId", IdPEntityId },
                { "RsaPfxFilePath", TestCertificates.RsaPfx },
                { "RsaPfxPassword", TestCertificates.Password },
                { "Saml2AssertionExpireTimeSpanFromMinutes", "10" },
                { "RequirePkceS256", "true" },
                { "PreviousSigningCerFilePaths", TestCertificates.RsaCer + ", " + TestCertificates.PathOf("missing.cer") }
            };
        }

        /// <summary>
        /// 設定を読む版は、IssuerId・RsaPfxFilePath の鍵・有効期間・RequirePkceS256・PreviousSigningCerFilePaths を使う
        /// （net10.0 だけ。構成を差し替える）。
        /// </summary>
        [Test]
        [Basis("設計：SAML2Client.CreateRedirectResponse・CreatePostResponse（設定を読む版）の remarks（CmnServerParams の値を使う。どれかが無ければ FrameworkException）")]
        public void ConfigOverloads_UseServerConfig()
        {
            WithConfig(ServerConfig(), () =>
            {
                RSA[] idpPublic = new RSA[] { TestCertificates.RsaPublicKey() };

                // ResponseObject：iss と鍵（kid 付き）
                string response = ResponseObject.Create(
                    new Dictionary<string, string> { { "code", "c1" } }, "client01", DateTimeOffset.UtcNow.AddMinutes(5));

                JObject jobj;
                Assert.That(ResponseObject.Verify(response, new JWS_RS256_X509(TestCertificates.RsaCer, ""),
                    IdPEntityId, "client01", out jobj), Is.True);

                JObject header = JObject.Parse(Encoding.UTF8.GetString(
                    CustomEncode.FromBase64UrlString(response.Split('.')[0])));
                Assert.That((string)header[JwtConst.kid], Is.Not.Null.And.Not.Empty, "kid で鍵を引かせる");
                Assert.That(header["jku"], Is.Null, "jku は入れない");

                // SAML2 の応答：issuer・鍵・有効期間（10 分）
                string query = SAML2Client.CreateRedirectResponse(Acs, "req01", SAML2Enum.StatusCode.Success,
                    "user01", SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.PasswordProtectedTransport,
                    null, "relay01");
                SAML2VerifyResult r = SAML2Client.VerifyResponseWithReason(query, null, idpPublic,
                    IdPEntityId, Acs, "req01", "relay01", "relay01");
                Assert.That(r.IsValid, Is.True, r.ReasonDescription);
                Assert.That(r.NotOnOrAfter, Is.GreaterThan(DateTime.UtcNow.AddMinutes(9))
                    .And.LessThan(DateTime.UtcNow.AddMinutes(11)), "Saml2AssertionExpireTimeSpanFromMinutes（分）");

                string post = SAML2Client.CreatePostResponse(Acs, "req01", SAML2Enum.StatusCode.Success,
                    "user01", SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.PasswordProtectedTransport, null);
                SAML2VerifyResult r2 = SAML2Client.VerifyResponseWithReason(null, post, idpPublic,
                    IdPEntityId, Acs, "req01", null, null);
                Assert.That(r2.IsValid, Is.True, r2.ReasonDescription);

                // PKCE：RequirePkceS256 = true なら plain を断る
                const string verifier = "dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk";
                bool usedS256;
                Assert.That(OAuth2AndOIDCSpec.VerifyPkce("plain", verifier, verifier, out usedS256), Is.False);
                Assert.That(OAuth2AndOIDCSpec.VerifyPkce("S256",
                    OAuth2AndOIDCClient.PKCE_S256_CodeChallengeMethod(verifier), verifier, out usedS256), Is.True);

                // メタデータ：読めるものだけ足す（読めないファイルは飛ばす）
                XmlDocument metadata = SAML2Bindings.CreateMetadata(IdPEntityId, "CURRENT",
                    new SAML2Enum.NameIDFormat[] { SAML2Enum.NameIDFormat.Unspecified },
                    "https://idp.example.com/sso", "https://idp.example.com/sso");
                Assert.That(SAML2Bindings.AppendSigningCertificates(metadata), Is.EqualTo(1));
            });
        }

        /// <summary>
        /// 有効期間が無い・数値でない、RequirePkceS256 が true / false でなければ FrameworkException
        /// （黙って既定値で動かさない）。
        /// </summary>
        [Test]
        [Basis("設計：SAML2Client.CreateRedirectResponse・CreatePostResponse（設定を読む版）の remarks（CmnServerParams の値を使う。どれかが無ければ FrameworkException）")]
        public void ConfigOverloads_InvalidValues()
        {
            foreach (string minutes in new string[] { null, "abc", "0" })
            {
                Dictionary<string, string> config = ServerConfig();
                config.Remove("Saml2AssertionExpireTimeSpanFromMinutes");
                if (minutes != null)
                {
                    config.Add("Saml2AssertionExpireTimeSpanFromMinutes", minutes);
                }

                WithConfig(config, () =>
                {
                    Assert.Throws<FrameworkException>(() => SAML2Client.CreatePostResponse(Acs, "req01", SAML2Enum.StatusCode.Success,
                        "user01", SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.Unspecified, null),
                        "Saml2AssertionExpireTimeSpanFromMinutes = " + (minutes ?? "(なし)"));
                });
            }

            Dictionary<string, string> pkce = ServerConfig();
            pkce["RequirePkceS256"] = "yes";

            WithConfig(pkce, () =>
            {
                bool usedS256;
                Assert.Throws<FrameworkException>(() => OAuth2AndOIDCSpec.VerifyPkce("plain", "v", "v", out usedS256));
            });
        }
#endif

        #endregion
    }
}

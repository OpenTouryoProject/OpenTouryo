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
//* クラス名        ：SAML2MetadataTests
//* クラス日本語名  ：SAML2Metadata・SAML2Attributes・VerifyResponseWithReason の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（#634）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

using NUnit.Framework;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security;

namespace TestFramework.Authentication
{
    /// <summary>SAML2Metadata（相手のメタデータ）・SAML2Attributes（属性）・VerifyResponseWithReason（理由つきの検証）の単体テスト</summary>
    [TestFixture]
    public class SAML2MetadataTests
    {
        private const string MetadataUrl = "https://idp.example.com/samlmetadata";

        /// <summary>IdP の証明書（base64 の DER）</summary>
        private static string CertBase64()
        {
#if NETCOREAPP
            return Convert.ToBase64String(X509CertificateLoader.LoadCertificateFromFile(TestCertificates.RsaCer).RawData);
#else
            return Convert.ToBase64String(new X509Certificate2(TestCertificates.RsaCer).RawData);
#endif
        }

        /// <summary>IdP のメタデータ（SAML2Bindings.CreateMetadata で作る）</summary>
        private static string Metadata()
        {
            return SAML2Bindings.CreateMetadata(SAML2BindingsTests.Issuer, CertBase64(),
                new SAML2Enum.NameIDFormat[] { SAML2Enum.NameIDFormat.Unspecified },
                "https://idp.example.com/sso-redirect", "https://idp.example.com/sso-post").OuterXml;
        }

        #region SAML2Metadata

        /// <summary>Parse：entityID・SSO の口・署名用の証明書を読み、証明書から検証鍵を作れる。</summary>
        [Test]
        public void Parse()
        {
            SAML2Metadata md = SAML2Metadata.Parse(Metadata());

            Assert.That(md, Is.Not.Null);
            Assert.That(md.EntityId, Is.EqualTo(SAML2BindingsTests.Issuer));
            Assert.That(md.SsoRedirectLocation, Is.EqualTo("https://idp.example.com/sso-redirect"));
            Assert.That(md.SsoPostLocation, Is.EqualTo("https://idp.example.com/sso-post"));
            Assert.That(md.SigningCertificates, Has.Count.EqualTo(1));
            Assert.That(md.GetFirstSigningKey(), Is.Not.Null);
        }

        /// <summary>Parse：XML でない・EntityDescriptor でない・DTD を含むものは null（例外にしない）。</summary>
        [Test]
        public void Parse_Invalid()
        {
            Assert.That(SAML2Metadata.Parse(null), Is.Null);
            Assert.That(SAML2Metadata.Parse("not xml"), Is.Null);
            Assert.That(SAML2Metadata.Parse("<a/>"), Is.Null);
            Assert.That(SAML2Metadata.Parse("<!DOCTYPE x [<!ENTITY e \"v\">]><md:EntityDescriptor xmlns:md=\"urn:oasis:names:tc:SAML:2.0:metadata\" entityID=\"&e;\"/>"),
                Is.Null, "DTD は処理しない");
        }

        /// <summary>GetAsync：期待する entityID と一致すれば返し、キャッシュする。違えば null。</summary>
        [Test]
        public void GetAsync_ChecksEntityId()
        {
            SAML2Metadata.Clear();

            try
            {
                using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, Metadata())))
                {
                    Assert.That(SAML2Metadata.GetAsync(MetadataUrl, SAML2BindingsTests.Issuer).Result, Is.Not.Null);
                    Assert.That(SAML2Metadata.GetAsync(MetadataUrl, SAML2BindingsTests.Issuer).Result, Is.Not.Null);
                    Assert.That(http.Requests.Count, Is.EqualTo(1), "2 回目はキャッシュ");

                    Assert.That(SAML2Metadata.GetAsync(MetadataUrl, "https://other.example.com/").Result, Is.Null);
                }
            }
            finally
            {
                SAML2Metadata.Clear();
            }
        }

        #endregion

        #region SAML2Attributes

        /// <summary>属性を足した応答から、メアド・利用者名・email_verified を読み戻せる（値は DOM で入る）。</summary>
        [Test]
        public void Attributes_RoundTrip()
        {
            XmlDocument res = CreateResponseWithAttributes("a&b@example.com", "user<01>", true);
            Dictionary<string, string> attrs = SAML2Attributes.Read(res, SAML2Bindings.CreateNamespaceManager(res));

            Assert.That(SAML2Attributes.GetEmail(attrs), Is.EqualTo("a&b@example.com"));
            Assert.That(SAML2Attributes.GetUserName(attrs), Is.EqualTo("user<01>"));
            Assert.That(SAML2Attributes.GetEmailVerified(attrs), Is.EqualTo("true"));
            Assert.That(attrs["mail"], Is.EqualTo("a&b@example.com"), "FriendlyName でも引ける");
        }

        /// <summary>別名でも読める。出す属性が無ければ AttributeStatement を足さない。</summary>
        [Test]
        public void Attributes_AliasesAndEmpty()
        {
            Assert.That(SAML2Attributes.GetEmail(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                { { "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", "x@example.com" } }), Is.EqualTo("x@example.com"));
            Assert.That(SAML2Attributes.GetUserName(null), Is.Empty);

            XmlDocument assertion = CreateAssertion();
            Assert.That(SAML2Attributes.AppendStatement(assertion, "", "", false), Is.False);
            Assert.That(assertion.GetElementsByTagName("AttributeStatement", SAML2Const.UrnAssertion).Count, Is.EqualTo(0));
            Assert.Throws<ArgumentNullException>(() => SAML2Attributes.AppendStatement(null, "a", "b", true));
        }

        #endregion

        #region VerifyResponseWithReason

        /// <summary>POST：すべて照合でき、属性も読める。鍵が複数なら、通るまで試す。</summary>
        [Test]
        public void VerifyResponseWithReason_Valid()
        {
            string saml = SignPost(CreateResponseWithAttributes("u@example.com", "user01", false));

            using (RSA other = RSA.Create(2048))
            {
                SAML2VerifyResult r = SAML2Client.VerifyResponseWithReason(null, saml,
                    new RSA[] { other, TestCertificates.RsaPublicKey() },
                    SAML2BindingsTests.Issuer, SAML2BindingsTests.Recipient, SAML2BindingsTests.InResponseTo, "rs", "rs");

                Assert.That(r.IsValid, Is.True, r.ReasonDescription);
                Assert.That(r.Reason, Is.EqualTo(SAML2VerifyResult.Reasons.Valid));
                Assert.That(r.VerifiedKeyIndex, Is.EqualTo(1));
                Assert.That(r.NameId, Is.EqualTo(SAML2BindingsTests.NameID));
                Assert.That(r.AudienceMatched, Is.True);
                Assert.That(r.RecipientMatched, Is.True);
                Assert.That(r.InResponseToMatched, Is.True);
                Assert.That(r.RelayStateMatched, Is.True);
                Assert.That(SAML2Attributes.GetEmail(r.Attributes), Is.EqualTo("u@example.com"));
            }
        }

        /// <summary>Redirect でも検証できる。</summary>
        [Test]
        public void VerifyResponseWithReason_Redirect()
        {
            string saml = SAML2BindingsTests.CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;
            DigitalSignParam ds = new DigitalSignParam(
                TestCertificates.RsaPrivateKey().ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "", ds);

            SAML2VerifyResult r = SAML2Client.VerifyResponseWithReason(query, null,
                new RSA[] { TestCertificates.RsaPublicKey() }, SAML2BindingsTests.Issuer, "", "", null, null);

            Assert.That(r.IsValid, Is.True, r.ReasonDescription);
            Assert.That(r.IsRedirect, Is.True);
            Assert.That(r.SigAlg, Is.EqualTo(SAML2Const.RSAwithSHA256));
            Assert.That(r.AudienceMatched, Is.Null, "期待値が無ければ照合しない");
        }

        /// <summary>落ちた理由を区別して返す。</summary>
        [Test]
        public void VerifyResponseWithReason_Reasons()
        {
            RSA idp = TestCertificates.RsaPublicKey();
            string ok = SignPost(SAML2BindingsTests.CreateResponse(SAML2Enum.StatusCode.Success, true));
            Func<string, string, string, string, string, SAML2VerifyResult.Reasons> reason =
                (saml, iss, acs, irt, rs) => SAML2Client.VerifyResponseWithReason(
                    null, saml, new RSA[] { idp }, iss, acs, irt, "rs", rs).Reason;

            Assert.That(reason(null, SAML2BindingsTests.Issuer, "", "", null), Is.EqualTo(SAML2VerifyResult.Reasons.NoResponse));
            Assert.That(SAML2Client.VerifyResponseWithReason(null, ok, new RSA[0], SAML2BindingsTests.Issuer, "", "", null, null).Reason,
                Is.EqualTo(SAML2VerifyResult.Reasons.NoKey));
            Assert.That(reason("!!!", SAML2BindingsTests.Issuer, "", "", null), Is.EqualTo(SAML2VerifyResult.Reasons.Unreadable));
            Assert.That(reason(SignPost(SAML2BindingsTests.CreateResponse(SAML2Enum.StatusCode.Requester, false)), SAML2BindingsTests.Issuer, "", "", null),
                Is.EqualTo(SAML2VerifyResult.Reasons.ErrorStatus));
            Assert.That(reason(ok, "https://other.example.com/", "", "", null), Is.EqualTo(SAML2VerifyResult.Reasons.IssuerMismatch));
            Assert.That(reason(ok, SAML2BindingsTests.Issuer, "https://other/acs", "", null), Is.EqualTo(SAML2VerifyResult.Reasons.AudienceMismatch));
            Assert.That(reason(ok, SAML2BindingsTests.Issuer, "", "other", null), Is.EqualTo(SAML2VerifyResult.Reasons.InResponseToMismatch));
            Assert.That(reason(ok, SAML2BindingsTests.Issuer, "", "", "other"), Is.EqualTo(SAML2VerifyResult.Reasons.RelayStateMismatch));

            using (RSA other = RSA.Create(2048))
            {
                SAML2VerifyResult r = SAML2Client.VerifyResponseWithReason(null, ok, new RSA[] { other },
                    SAML2BindingsTests.Issuer, "", "", null, null);
                Assert.That(r.Reason, Is.EqualTo(SAML2VerifyResult.Reasons.SignatureInvalid));
                Assert.That(r.IsValid, Is.False);
                Assert.That(r.StatusCode, Is.EqualTo(SAML2Enum.StatusCode.Success), "切り分けのために返る");
                Assert.That(r.Attributes, Is.Empty, "署名を確かめていない応答から属性を読まない");
            }
        }

        #endregion

        #region private

        /// <summary>アサーションだけを作る</summary>
        private static XmlDocument CreateAssertion()
        {
            string id;
            return SAML2Bindings.CreateAssertion(
                SAML2BindingsTests.InResponseTo, SAML2BindingsTests.Issuer, SAML2BindingsTests.NameID,
                SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.PasswordProtectedTransport,
                3600, SAML2BindingsTests.Recipient, out id);
        }

        /// <summary>属性を足したアサーションを持つ応答を作る</summary>
        private static XmlDocument CreateResponseWithAttributes(string email, string userName, bool emailVerified)
        {
            string id;
            XmlDocument response = SAML2Bindings.CreateResponse(SAML2BindingsTests.Issuer, SAML2BindingsTests.Recipient,
                SAML2BindingsTests.InResponseTo, SAML2Enum.StatusCode.Success, out id);

            XmlDocument assertion = CreateAssertion();
            Assert.That(SAML2Attributes.AppendStatement(assertion, email, userName, emailVerified), Is.True);

            response.DocumentElement.AppendChild(response.ImportNode(assertion.DocumentElement, true));
            return response;
        }

        /// <summary>POST の応答に署名する（IdP の鍵）</summary>
        private static string SignPost(XmlDocument res)
        {
            string id = SAML2Bindings.GetIdInResponse(res, SAML2Bindings.CreateNamespaceManager(res));
            return SAML2Bindings.EncodeAndSignPost(res.OuterXml, id, TestCertificates.RsaPrivateKey());
        }

        #endregion
    }
}

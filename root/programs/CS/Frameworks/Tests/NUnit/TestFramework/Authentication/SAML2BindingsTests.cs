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
//* クラス名        ：SAML2BindingsTests
//* クラス日本語名  ：SAML2Bindings（SAML2 の組み立て・符号化・署名・検証）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/19  玄人 幸道         新規作成
//**********************************************************************************

using System.Security.Cryptography;
using System.Xml;

using NUnit.Framework;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security;

namespace TestFramework.Authentication
{
    /// <summary>SAML2Bindings（SAML2 の組み立て・符号化・署名・検証）の単体テスト</summary>
    /// <remarks>
    /// 後退テストは Golden の TestCode（TestSAML2）にもある。ここは、組み立てた要求・応答から値を読み戻せること、
    /// Redirect（クエリ文字列の署名）と POST（XML 署名）の往復、構造の検証（VerifyByXPath）を確かめる。
    /// </remarks>
    [TestFixture]
    public class SAML2BindingsTests
    {
        internal const string Issuer = "https://idp.example.com/";
        internal const string Recipient = "https://sp.example.com/acs";
        internal const string InResponseTo = "s2request";
        internal const string NameID = "user01";

        /// <summary>応答（Assertion 付き・無し）を組み立てる</summary>
        internal static XmlDocument CreateResponse(SAML2Enum.StatusCode statusCode, bool withAssertion)
        {
            string id;
            XmlDocument response = SAML2Bindings.CreateResponse(Issuer, Recipient, InResponseTo, statusCode, out id);

            if (withAssertion)
            {
                XmlDocument assertion = SAML2Bindings.CreateAssertion(
                    InResponseTo, Issuer, NameID,
                    SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.AuthnContextClassRef.PasswordProtectedTransport,
                    3600, Recipient, out id);
                response.DocumentElement.AppendChild(response.ImportNode(assertion.DocumentElement, true));
            }

            return response;
        }

        /// <summary>要求（AuthnRequest）の値を、XPath で読み戻せる。構造の検証も通る。</summary>
        [Test]
        public void CreateRequest_ReadBack()
        {
            string id;
            XmlDocument req = SAML2Bindings.CreateRequest("https://sp.example.com/",
                SAML2Enum.ProtocolBinding.HttpPost, SAML2Enum.NameIDFormat.EmailAddress, Recipient, out id);
            XmlNamespaceManager ns = SAML2Bindings.CreateNamespaceManager(req);

            Assert.That(id, Does.StartWith("s2"));
            Assert.That(SAML2Bindings.GetIdInRequest(req, ns), Is.EqualTo(id));
            Assert.That(SAML2Bindings.GetIssuerInRequest(req, ns), Is.EqualTo("https://sp.example.com/"));
            Assert.That(SAML2Bindings.GetAssertionConsumerServiceURLInRequest(req, ns), Is.EqualTo(Recipient));
            Assert.That(SAML2Bindings.GetProtocolBindingInRequest(req, ns),
                Is.EqualTo(SAML2Enum.EnumToString(SAML2Enum.ProtocolBinding.HttpPost)));
            Assert.That(SAML2Bindings.GetNameIDPolicyFormatInRequest(req, ns),
                Is.EqualTo(SAML2Enum.EnumToString(SAML2Enum.NameIDFormat.EmailAddress)));
            Assert.That(SAML2Bindings.VerifyByXPath(req, SAML2Enum.SamlSchema.Request, ns), Is.True);
        }

        /// <summary>応答（Assertion 付き）の値を、XPath で読み戻せる。構造の検証も通る。</summary>
        [Test]
        public void CreateResponseAndAssertion_ReadBack()
        {
            XmlDocument res = CreateResponse(SAML2Enum.StatusCode.Success, true);
            XmlNamespaceManager ns = SAML2Bindings.CreateNamespaceManager(res);

            Assert.That(SAML2Bindings.GetIdInResponse(res, ns), Does.StartWith("s2"));
            // GetIdInAssertion は Assertion の文書（ルートが Assertion）から読む。Response の中の Assertion は読まない。
            Assert.That(SAML2Bindings.GetIdInAssertion(res, ns), Is.Empty);
            Assert.That(SAML2Bindings.GetIssuerInResponse(res, ns), Is.EqualTo(Issuer));
            Assert.That(SAML2Bindings.GetIssuerInAssertion(res, ns), Is.EqualTo(Issuer));
            Assert.That(SAML2Bindings.GetInResponseToInResponse(res, ns), Is.EqualTo(InResponseTo));
            Assert.That(SAML2Bindings.GetStatusCodeInResponse(res, ns),
                Is.EqualTo(SAML2Enum.EnumToString(SAML2Enum.StatusCode.Success)));

            string format, nameId;
            Assert.That(SAML2Bindings.GetNameIDInAssertion(res, ns, out format, out nameId), Is.Not.Null);
            Assert.That(nameId, Is.EqualTo(NameID));
            Assert.That(format, Is.EqualTo(SAML2Enum.EnumToString(SAML2Enum.NameIDFormat.Unspecified)));

            string inResponseTo, notOnOrAfter, recipient;
            Assert.That(SAML2Bindings.GetSubjectConfirmationDataInAssertion(res, ns,
                out inResponseTo, out notOnOrAfter, out recipient), Is.Not.Null);
            Assert.That(inResponseTo, Is.EqualTo(InResponseTo));
            Assert.That(recipient, Is.EqualTo(Recipient));
            Assert.That(notOnOrAfter, Is.Not.Empty);

            Assert.That(SAML2Bindings.GetSubjectConfirmationMethodInAssertion(res, ns), Is.Not.Empty);
            Assert.That(SAML2Bindings.GetConditionsNotOnOrAfterInAssertion(res, ns), Is.Not.Empty);
            Assert.That(SAML2Bindings.GetAudienceInAssertion(res, ns), Is.EqualTo(Recipient));
            Assert.That(SAML2Bindings.GetAuthnContextClassRefInAssertion(res, ns),
                Is.EqualTo(SAML2Enum.EnumToString(SAML2Enum.AuthnContextClassRef.PasswordProtectedTransport)));

            Assert.That(SAML2Bindings.VerifyByXPath(res, SAML2Enum.SamlSchema.Response, ns), Is.True);
        }

        /// <summary>Assertion の文書：ID を読める。署名すると、Assertion 自身が署名される（同じ鍵で検証できる）。</summary>
        [Test]
        public void CreateAssertion_Standalone()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                string id;
                XmlDocument assertion = SAML2Bindings.CreateAssertion(
                    InResponseTo, Issuer, NameID, SAML2Enum.NameIDFormat.EmailAddress, SAML2Enum.AuthnContextClassRef.X509,
                    3600, Recipient, out id, rsa);

                Assert.That(SAML2Bindings.GetIdInAssertion(assertion, SAML2Bindings.CreateNamespaceManager(assertion)), Is.EqualTo(id));
                Assert.That(SAML2Bindings.VerifyPost(assertion.OuterXml, id, rsa), Is.True);
            }
        }

        /// <summary>
        /// 構造の検証：Success なのに Assertion が無い応答は通らない。
        /// エラー応答（Success 以外）は Assertion が無くても通る（#598）。
        /// </summary>
        [Test]
        public void VerifyByXPath_Response()
        {
            XmlDocument noAssertion = CreateResponse(SAML2Enum.StatusCode.Success, false);
            Assert.That(SAML2Bindings.VerifyByXPath(noAssertion, SAML2Enum.SamlSchema.Response,
                SAML2Bindings.CreateNamespaceManager(noAssertion)), Is.False);

            XmlDocument error = CreateResponse(SAML2Enum.StatusCode.Requester, false);
            Assert.That(SAML2Bindings.VerifyByXPath(error, SAML2Enum.SamlSchema.Response,
                SAML2Bindings.CreateNamespaceManager(error)), Is.True);
        }

        /// <summary>Redirect：署名せずに符号化したものは、そのまま復号できる。</summary>
        [Test]
        public void Redirect_EncodeAndDecode()
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;
            string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "relay01");

            Assert.That(query, Does.StartWith("SAMLResponse=").And.Contain("RelayState=relay01"));
            Assert.That(query, Does.Not.Contain("Signature="), "DigitalSign が null なら署名しない");
            Assert.That(SAML2Bindings.DecodeRedirect(query), Is.EqualTo(saml));
        }

        /// <summary>
        /// Redirect：署名したクエリ文字列は、DigitalSign 版・RSA 版の両方で検証できる。
        /// 別の鍵や、書き換えた値では通らない。
        /// </summary>
        [Test]
        public void Redirect_SignAndVerify()
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;

            using (RSA rsa = RSA.Create(2048))
            using (RSA other = RSA.Create(2048))
            {
                DigitalSignParam ds = new DigitalSignParam(rsa.ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "", ds);
                Assert.That(query, Does.Contain("Signature="));

                Assert.That(SAML2Bindings.VerifyRedirect(query, ds), Is.True);
                Assert.That(SAML2Bindings.VerifyRedirect(query, rsa), Is.True);
                Assert.That(SAML2Bindings.VerifyRedirect(query, other), Is.False, "別の鍵");

                DigitalSignParam otherDs = new DigitalSignParam(other.ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                Assert.That(SAML2Bindings.VerifyRedirect(query, otherDs), Is.False, "別の鍵（DigitalSign 版）");

                // RelayState を足すと、署名した文字列と変わる
                Assert.That(SAML2Bindings.VerifyRedirect(query + "&RelayState=evil", rsa), Is.False, "書き換え");
            }
        }

        /// <summary>POST：XML 署名した応答は、Base64 を戻して、同じ鍵で検証できる。別の鍵では通らない。</summary>
        [Test]
        public void Post_SignAndVerify()
        {
            XmlDocument res = CreateResponse(SAML2Enum.StatusCode.Success, true);
            string id = SAML2Bindings.GetIdInResponse(res, SAML2Bindings.CreateNamespaceManager(res));

            using (RSA rsa = RSA.Create(2048))
            using (RSA other = RSA.Create(2048))
            {
                string encoded = SAML2Bindings.EncodeAndSignPost(res.OuterXml, id, rsa);
                string decoded = SAML2Bindings.DecodePost(encoded);

                Assert.That(decoded, Does.Contain("<Signature"));
                Assert.That(SAML2Bindings.VerifyPost(decoded, id, rsa), Is.True);
                Assert.That(SAML2Bindings.VerifyPost(decoded, id, other), Is.False, "別の鍵");
                Assert.That(SAML2Bindings.VerifyPost(decoded.Replace(NameID, "admin01"), id, rsa), Is.False, "書き換え");
            }
        }

        /// <summary>POST：鍵を渡さなければ署名しない（Base64 にするだけ）。</summary>
        [Test]
        public void Post_WithoutKey_NotSigned()
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;
            string decoded = SAML2Bindings.DecodePost(SAML2Bindings.EncodeAndSignPost(saml));

            Assert.That(decoded, Is.EqualTo(saml));
        }

        /// <summary>メタデータ：渡した値が入る。</summary>
        [Test]
        public void CreateMetadata()
        {
            XmlDocument md = SAML2Bindings.CreateMetadata("https://idp.example.com/", "MIIBpem",
                new[] { SAML2Enum.NameIDFormat.Unspecified, SAML2Enum.NameIDFormat.EmailAddress },
                "https://idp.example.com/redirect", "https://idp.example.com/post");
            string xml = md.OuterXml;

            Assert.That(xml, Does.Contain("https://idp.example.com/redirect"));
            Assert.That(xml, Does.Contain("https://idp.example.com/post"));
            Assert.That(xml, Does.Contain("MIIBpem"));
            Assert.That(xml, Does.Contain(SAML2Enum.EnumToString(SAML2Enum.NameIDFormat.EmailAddress)));
        }
    }
}

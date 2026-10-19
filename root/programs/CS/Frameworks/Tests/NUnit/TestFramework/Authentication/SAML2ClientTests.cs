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
//* クラス名        ：SAML2ClientTests
//* クラス日本語名  ：SAML2Client（要求の作成・応答の検証）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/19  玄人 幸道         新規作成
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Xml;

using NUnit.Framework;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security;

namespace TestFramework.Authentication
{
    /// <summary>SAML2Client（要求の作成・応答の検証）の単体テスト</summary>
    /// <remarks>
    /// **鍵・証明書を引数で受ける版だけを使う**（#600 / #606）。設定（CmnClientParams）を読む版は、配備の設定に依るので対象外。
    /// IdP の鍵は SHA256RSA.pfx、検証は SHA256RSA.cer（TestCertificates）。
    /// </remarks>
    [TestFixture]
    public class SAML2ClientTests
    {
        /// <summary>VerifyResponse の結果</summary>
        private class Result
        {
            public bool Ok;
            public string NameId, Iss, Aud, InResponseTo, Recipient;
            public DateTime? NotOnOrAfter;
            public SAML2Enum.StatusCode? StatusCode;
            public SAML2Enum.NameIDFormat? NameIDFormat;
            public SAML2Enum.AuthnContextClassRef? AuthnContextClassRef;
        }

        /// <summary>VerifyResponse（cerFilePath を受ける版）を呼ぶ</summary>
        private static Result Verify(string queryString, string samlResponse, string cerFilePath)
        {
            Result r = new Result();
            XmlDocument doc;
            r.Ok = SAML2Client.VerifyResponse(queryString, samlResponse, cerFilePath,
                out r.NameId, out r.Iss, out r.Aud, out r.InResponseTo, out r.Recipient, out r.NotOnOrAfter,
                out r.StatusCode, out r.NameIDFormat, out r.AuthnContextClassRef, out doc);
            return r;
        }

        /// <summary>POST の応答を作る（IdP の鍵で署名）</summary>
        private static string PostResponse(SAML2Enum.StatusCode statusCode, bool withAssertion)
        {
            XmlDocument res = SAML2BindingsTests.CreateResponse(statusCode, withAssertion);
            string id = SAML2Bindings.GetIdInResponse(res, SAML2Bindings.CreateNamespaceManager(res));
            return SAML2Bindings.EncodeAndSignPost(res.OuterXml, id, TestCertificates.RsaPrivateKey());
        }

        /// <summary>
        /// POST：IdP の証明書で検証でき、Assertion の値が返る。
        /// </summary>
        [Test]
        public void VerifyResponse_Post()
        {
            Result r = Verify(null, PostResponse(SAML2Enum.StatusCode.Success, true), TestCertificates.RsaCer);

            Assert.That(r.Ok, Is.True);
            Assert.That(r.StatusCode, Is.EqualTo(SAML2Enum.StatusCode.Success));
            Assert.That(r.NameId, Is.EqualTo(SAML2BindingsTests.NameID));
            Assert.That(r.Iss, Is.EqualTo(SAML2BindingsTests.Issuer));
            Assert.That(r.Aud, Is.EqualTo(SAML2BindingsTests.Recipient));
            Assert.That(r.InResponseTo, Is.EqualTo(SAML2BindingsTests.InResponseTo));
            Assert.That(r.Recipient, Is.EqualTo(SAML2BindingsTests.Recipient));
            Assert.That(r.NotOnOrAfter, Is.GreaterThan(DateTime.UtcNow));
            Assert.That(r.NameIDFormat, Is.EqualTo(SAML2Enum.NameIDFormat.Unspecified));
            Assert.That(r.AuthnContextClassRef, Is.EqualTo(SAML2Enum.AuthnContextClassRef.PasswordProtectedTransport));
        }

        /// <summary>Redirect：IdP の証明書で検証できる。</summary>
        [Test]
        public void VerifyResponse_Redirect()
        {
            string saml = SAML2BindingsTests.CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;
            DigitalSignParam ds = new DigitalSignParam(
                TestCertificates.RsaPrivateKey().ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "", ds);

            Result r = Verify(query, null, TestCertificates.RsaCer);
            Assert.That(r.Ok, Is.True);
            Assert.That(r.NameId, Is.EqualTo(SAML2BindingsTests.NameID));
        }

        /// <summary>
        /// 別の鍵で署名した応答は通らない。エラー応答は、署名が正しくても false（Assertion は使えない）。
        /// 応答が無ければ false。statusCode は、検証に失敗しても切り分けのために返る（#598）。
        /// </summary>
        [Test]
        public void VerifyResponse_Rejected()
        {
            using (RSA other = RSA.Create(2048))
            {
                XmlDocument res = SAML2BindingsTests.CreateResponse(SAML2Enum.StatusCode.Success, true);
                string id = SAML2Bindings.GetIdInResponse(res, SAML2Bindings.CreateNamespaceManager(res));
                Result wrongKey = Verify(null, SAML2Bindings.EncodeAndSignPost(res.OuterXml, id, other), TestCertificates.RsaCer);
                Assert.That(wrongKey.Ok, Is.False, "別の鍵");
                Assert.That(wrongKey.StatusCode, Is.EqualTo(SAML2Enum.StatusCode.Success), "statusCode は返る");
            }

            Result error = Verify(null, PostResponse(SAML2Enum.StatusCode.Requester, false), TestCertificates.RsaCer);
            Assert.That(error.Ok, Is.False, "エラー応答");
            Assert.That(error.StatusCode, Is.EqualTo(SAML2Enum.StatusCode.Requester));

            Assert.That(Verify(null, null, TestCertificates.RsaCer).Ok, Is.False, "応答が無い");
        }

        /// <summary>
        /// CreateRedirectRequest（DigitalSign を受ける版）：署名した要求は、同じ鍵で検証できる。要求の id が返る。
        /// </summary>
        [Test]
        public void CreateRedirectRequest_Signed()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                DigitalSignParam ds = new DigitalSignParam(rsa.ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string id;
                string query = SAML2Client.CreateRedirectRequest(
                    SAML2Enum.RequestOrResponse.Request, SAML2Enum.ProtocolBinding.HttpRedirect, SAML2Enum.NameIDFormat.Unspecified,
                    "https://sp.example.com/", SAML2BindingsTests.Recipient, "relay01", ds, out id);

                Assert.That(query, Does.StartWith("SAMLRequest="));
                Assert.That(SAML2Bindings.VerifyRedirect(query, rsa), Is.True);

                XmlDocument req = new XmlDocument();
                req.LoadXml(SAML2Bindings.DecodeRedirect(query));
                Assert.That(SAML2Bindings.GetIdInRequest(req, SAML2Bindings.CreateNamespaceManager(req)), Is.EqualTo(id));
            }
        }

        /// <summary>CreatePostRequest（RSA を受ける版）：XML 署名した要求は、同じ鍵で検証できる。</summary>
        [Test]
        public void CreatePostRequest_Signed()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                string id;
                string encoded = SAML2Client.CreatePostRequest(
                    SAML2Enum.ProtocolBinding.HttpPost, SAML2Enum.NameIDFormat.Unspecified,
                    "https://sp.example.com/", SAML2BindingsTests.Recipient, "relay01", rsa, out id);

                Assert.That(SAML2Bindings.VerifyPost(SAML2Bindings.DecodePost(encoded), id, rsa), Is.True);
            }
        }
    }
}

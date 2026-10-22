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
//*  2026/10/22  玄人 幸道         鍵を受ける版が null・空を受けないこと、鍵を受けない版が設定の鍵を使うことのテストを追加
//*  2026/10/22  玄人 幸道         NotOnOrAfter の比較・期限切れ・InResponseTo の食い違い・エラー応答・Recipient・SigAlg のテストを足した（カバレッジの拡充）
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Xml;

using NUnit.Framework;

#if NETCOREAPP
using Microsoft.Extensions.Configuration;
#endif

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

        #region 食い違いと期限（bool 版と理由つきの版）

        /// <summary>応答を組み立て、Assertion を書き換えてから IdP の鍵で署名する</summary>
        private static string SignedPostWith(Action<XmlDocument, XmlNamespaceManager> edit)
        {
            XmlDocument res = SAML2BindingsTests.CreateResponse(SAML2Enum.StatusCode.Success, true);
            XmlNamespaceManager ns = SAML2Bindings.CreateNamespaceManager(res);
            edit(res, ns);
            string id = SAML2Bindings.GetIdInResponse(res, ns);
            return SAML2Bindings.EncodeAndSignPost(res.OuterXml, id, TestCertificates.RsaPrivateKey());
        }

        /// <summary>理由つきの版で検証する（IdP の証明書）</summary>
        private static SAML2VerifyResult VerifyWithReason(string saml)
        {
            return SAML2Client.VerifyResponseWithReason(null, saml, new RSA[] { TestCertificates.RsaPublicKey() },
                SAML2BindingsTests.Issuer, "", "", null, null);
        }

        /// <summary>
        /// NotOnOrAfter の片方（Conditions）が読めない応答。
        /// 理由つきの版は Expired で断る（読めない方を無視して、もう片方で通さない）。
        /// bool 版は従来どおり例外を投げる（振る舞いを変えていない）。
        /// </summary>
        [Test]
        public void NotOnOrAfter_Unreadable()
        {
            string saml = SignedPostWith((res, ns) =>
                ((XmlElement)res.SelectSingleNode(SAML2Const.XPathConditionsInAssertion, ns)).SetAttribute("NotOnOrAfter", "not-a-date"));

            SAML2VerifyResult r = VerifyWithReason(saml);
            Assert.That(r.IsValid, Is.False);
            Assert.That(r.Reason, Is.EqualTo(SAML2VerifyResult.Reasons.Expired));
            Assert.That(r.ReasonDescription, Does.Contain("could not be read"));

            Assert.That(() => Verify(null, saml, TestCertificates.RsaCer), Throws.Exception, "bool 版は従来どおり例外");
        }

        /// <summary>
        /// Issuer が Response と Assertion で食い違う応答。
        /// bool 版は false（out の iss は空のまま）、理由つきの版は Conflicted。
        /// 片方だけに Issuer が在る応答は、どちらの版もその値を採る。
        /// </summary>
        [Test]
        public void Issuer_Conflicted()
        {
            string conflicted = SignedPostWith((res, ns) =>
                res.SelectSingleNode(SAML2Const.XPathIssuerInAssertion, ns).InnerText = "https://other.example.com/");

            Result r = Verify(null, conflicted, TestCertificates.RsaCer);
            Assert.That(r.Ok, Is.False);
            Assert.That(r.Iss, Is.Empty, "食い違ったら値を入れずに返す（従来どおり）");
            Assert.That(VerifyWithReason(conflicted).Reason, Is.EqualTo(SAML2VerifyResult.Reasons.Conflicted));

            string onlyAssertion = SignedPostWith((res, ns) =>
                res.SelectSingleNode(SAML2Const.XPathIssuerInResponse, ns).InnerText = "");

            Result r2 = Verify(null, onlyAssertion, TestCertificates.RsaCer);
            Assert.That(r2.Ok, Is.True);
            Assert.That(r2.Iss, Is.EqualTo(SAML2BindingsTests.Issuer));
            Assert.That(VerifyWithReason(onlyAssertion).Issuer, Is.EqualTo(SAML2BindingsTests.Issuer));
        }


        /// <summary>W3C 形式の時刻（今から minutes 分後）</summary>
        private static string W3c(int minutes)
        {
            return Touryo.Infrastructure.Public.Str.FormatConverter.ToW3cTimestamp(DateTime.UtcNow.AddMinutes(minutes));
        }

        /// <summary>
        /// NotOnOrAfter が SubjectConfirmationData と Conditions で違えば、短い方を採る（bool 版）。
        /// どちらが短くても、その値が out に返る。
        /// </summary>
        [Test]
        public void NotOnOrAfter_PicksShorter()
        {
            foreach (bool conditionsShorter in new bool[] { true, false })
            {
                string shorter = W3c(5);
                string longer = W3c(30);

                string saml = SignedPostWith((res, ns) =>
                {
                    ((XmlElement)res.SelectSingleNode(SAML2Const.XPathConditionsInAssertion, ns))
                        .SetAttribute("NotOnOrAfter", conditionsShorter ? shorter : longer);
                    ((XmlElement)res.SelectSingleNode(SAML2Const.XPathSubjectConfirmationDataInAssertion, ns))
                        .SetAttribute("NotOnOrAfter", conditionsShorter ? longer : shorter);
                });

                Result r = Verify(null, saml, TestCertificates.RsaCer);
                Assert.That(r.Ok, Is.True, "Conditions が短い : " + conditionsShorter);
                Assert.That(r.NotOnOrAfter,
                    Is.EqualTo(Touryo.Infrastructure.Public.Str.FormatConverter.FromW3cTimestamp(shorter)), "短い方");

                Assert.That(VerifyWithReason(saml).IsValid, Is.True, VerifyWithReason(saml).ReasonDescription);
            }
        }

        /// <summary>期限が切れた Assertion：bool 版は false、理由つきの版は Expired。</summary>
        [Test]
        public void NotOnOrAfter_Expired()
        {
            string expired = W3c(-5);
            string saml = SignedPostWith((res, ns) =>
            {
                ((XmlElement)res.SelectSingleNode(SAML2Const.XPathConditionsInAssertion, ns)).SetAttribute("NotOnOrAfter", expired);
                ((XmlElement)res.SelectSingleNode(SAML2Const.XPathSubjectConfirmationDataInAssertion, ns)).SetAttribute("NotOnOrAfter", expired);
            });

            Assert.That(Verify(null, saml, TestCertificates.RsaCer).Ok, Is.False);

            SAML2VerifyResult r = VerifyWithReason(saml);
            Assert.That(r.Reason, Is.EqualTo(SAML2VerifyResult.Reasons.Expired));
            Assert.That(r.NotExpired, Is.False);
        }

        /// <summary>InResponseTo が SubjectConfirmationData と Response で食い違えば、bool 版は false、理由つきの版は Conflicted。</summary>
        [Test]
        public void InResponseTo_Conflicted()
        {
            string saml = SignedPostWith((res, ns) =>
                ((XmlElement)res.SelectSingleNode(SAML2Const.XPathSubjectConfirmationDataInAssertion, ns)).SetAttribute("InResponseTo", "other"));

            Result r = Verify(null, saml, TestCertificates.RsaCer);
            Assert.That(r.Ok, Is.False);
            Assert.That(r.InResponseTo, Is.Null.Or.Empty, "食い違ったら値を入れずに返す");
            Assert.That(VerifyWithReason(saml).Reason, Is.EqualTo(SAML2VerifyResult.Reasons.Conflicted));
        }

        /// <summary>
        /// 署名もスキーマも正しいエラー応答（Assertion 付き）：理由つきの版は ErrorStatus で、SignatureVerified は false にする
        /// （エラー応答の中身を使わせない）。
        /// </summary>
        [Test]
        public void ErrorStatus_WithAssertion()
        {
            XmlDocument res = SAML2BindingsTests.CreateResponse(SAML2Enum.StatusCode.Requester, true);
            string id = SAML2Bindings.GetIdInResponse(res, SAML2Bindings.CreateNamespaceManager(res));
            string saml = SAML2Bindings.EncodeAndSignPost(res.OuterXml, id, TestCertificates.RsaPrivateKey());

            SAML2VerifyResult r = VerifyWithReason(saml);
            Assert.That(r.Reason, Is.EqualTo(SAML2VerifyResult.Reasons.ErrorStatus));
            Assert.That(r.SignatureVerified, Is.False);
            Assert.That(r.ReasonDescription, Does.Contain("Requester"));
        }

        /// <summary>Recipient だけが ACS URL と違えば RecipientMismatch（Audience は一致）。</summary>
        [Test]
        public void RecipientMismatch()
        {
            string saml = SignedPostWith((res, ns) =>
                ((XmlElement)res.SelectSingleNode(SAML2Const.XPathSubjectConfirmationDataInAssertion, ns))
                    .SetAttribute("Recipient", "https://other.example.com/acs"));

            SAML2VerifyResult r = SAML2Client.VerifyResponseWithReason(null, saml, new RSA[] { TestCertificates.RsaPublicKey() },
                SAML2BindingsTests.Issuer, SAML2BindingsTests.Recipient, "", null, null);

            Assert.That(r.AudienceMatched, Is.True);
            Assert.That(r.RecipientMatched, Is.False);
            Assert.That(r.Reason, Is.EqualTo(SAML2VerifyResult.Reasons.RecipientMismatch));
        }

        /// <summary>Redirect で SigAlg が無ければ NoSigAlg（署名を検証できない）。</summary>
        [Test]
        public void Redirect_NoSigAlg()
        {
            DigitalSign ds = new DigitalSignParam(
                TestCertificates.RsaPrivateKey().ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response,
                SAML2BindingsTests.CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml, "relay01", ds);

            string noSigAlg = System.Text.RegularExpressions.Regex.Replace(query, "&?" + SAML2Const.QueryParamSigAlg + "=[^&]*", "");
            Assert.That(noSigAlg, Does.Not.Contain(SAML2Const.QueryParamSigAlg + "="));

            SAML2VerifyResult r = SAML2Client.VerifyResponseWithReason(noSigAlg, null, new RSA[] { TestCertificates.RsaPublicKey() },
                SAML2BindingsTests.Issuer, "", "", "relay01", "relay01");
            Assert.That(r.Reason, Is.EqualTo(SAML2VerifyResult.Reasons.NoSigAlg));
        }
        #endregion

        #region 鍵を受ける版は設定へ倒さない

        /// <summary>
        /// 鍵・証明書を受ける版は、null・空を ArgumentNullException にする（設定へは倒さない）。
        /// 素通しにすると、SAML2Bindings.EncodeAndSign* が黙って無署名の要求を出すため。
        /// </summary>
        [Test]
        public void KeyOverloads_RejectNull()
        {
            string id;

            Assert.Throws<ArgumentNullException>(() => SAML2Client.CreateRedirectRequest(
                SAML2Enum.RequestOrResponse.Request, SAML2Enum.ProtocolBinding.HttpRedirect, SAML2Enum.NameIDFormat.Unspecified,
                "https://sp.example.com/", SAML2BindingsTests.Recipient, "relay01", (DigitalSign)null, out id));

            Assert.Throws<ArgumentNullException>(() => SAML2Client.CreatePostRequest(
                SAML2Enum.ProtocolBinding.HttpPost, SAML2Enum.NameIDFormat.Unspecified,
                "https://sp.example.com/", SAML2BindingsTests.Recipient, "relay01", (RSA)null, out id));

            string saml = PostResponse(SAML2Enum.StatusCode.Success, true);
            Assert.Throws<ArgumentNullException>(() => Verify(null, saml, null));
            Assert.Throws<ArgumentNullException>(() => Verify(null, saml, ""));
            Assert.Throws<ArgumentNullException>(() => Verify(null, null, null), "応答が無くても、引数の誤りは例外");
        }

#if NETCOREAPP
        /// <summary>
        /// 鍵を受けない版は、設定（SpRp_RsaPfxFilePath / SpRp_RsaPfxPassword / SpRp_RsaCerFilePath）の鍵を使う（net10.0 だけ。構成を差し替える）。
        /// </summary>
        [Test]
        public void ConfigOverloads_UseConfiguredKeys()
        {
            System.Collections.Generic.Dictionary<string, string> values = new System.Collections.Generic.Dictionary<string, string>
            {
                { "appSettings:SpRp_RsaPfxFilePath", TestCertificates.RsaPfx },
                { "appSettings:SpRp_RsaPfxPassword", TestCertificates.Password },
                { "appSettings:SpRp_RsaCerFilePath", TestCertificates.RsaCer }
            };

            try
            {
                Touryo.Infrastructure.Public.Util.GetConfigParameter.InitConfiguration(
                    new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddInMemoryCollection(values).Build());

                string id;
                RSA pub = TestCertificates.RsaPublicKey();

                // Redirect（設定の鍵で署名）
                string query = SAML2Client.CreateRedirectRequest(
                    SAML2Enum.RequestOrResponse.Request, SAML2Enum.ProtocolBinding.HttpRedirect, SAML2Enum.NameIDFormat.Unspecified,
                    "https://sp.example.com/", SAML2BindingsTests.Recipient, "relay01", out id);
                Assert.That(SAML2Bindings.VerifyRedirect(query, pub), Is.True);

                // POST（設定の鍵で署名）
                string encoded = SAML2Client.CreatePostRequest(
                    SAML2Enum.ProtocolBinding.HttpPost, SAML2Enum.NameIDFormat.Unspecified,
                    "https://sp.example.com/", SAML2BindingsTests.Recipient, "relay01", out id);
                Assert.That(SAML2Bindings.VerifyPost(SAML2Bindings.DecodePost(encoded), id, pub), Is.True);

                // 検証（設定の証明書）
                string nameId, iss, aud, inResponseTo, recipient;
                DateTime? notOnOrAfter;
                SAML2Enum.StatusCode? statusCode;
                SAML2Enum.NameIDFormat? nameIDFormat;
                SAML2Enum.AuthnContextClassRef? authnContextClassRef;
                XmlDocument doc;
                Assert.That(SAML2Client.VerifyResponse(null, PostResponse(SAML2Enum.StatusCode.Success, true),
                    out nameId, out iss, out aud, out inResponseTo, out recipient, out notOnOrAfter,
                    out statusCode, out nameIDFormat, out authnContextClassRef, out doc), Is.True);
            }
            finally
            {
                Touryo.Infrastructure.Public.Util.GetConfigParameter.InitConfiguration(
                    new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                        .AddInMemoryCollection(new System.Collections.Generic.Dictionary<string, string>()).Build());
            }
        }
#endif

        #endregion
    }
}

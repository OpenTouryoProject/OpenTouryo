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
//*  2026/10/22  玄人 幸道         SigAlg（ハッシュ別・DSA・PSS）・空の入力・暗号化用の鍵を飛ばすことのテストを足した（#634。カバレッジの拡充）
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//*  2026/10/30  玄人 幸道         Redirect の SigAlg：RSA-PSS・ECDSA を RFC 9231 の URI にし、決められないものは例外にした（#643）
//*  2026/10/31  玄人 幸道         Redirect：ECDSA でハッシュを渡さないときに SHA-256 で署名していることを、SHA-256 を明示した検証で確かめるテストを足した（#643）
//*  2026/11/05  玄人 幸道         Redirect：署名の対象を 3 つのパラメタだけから組み立てることのテストを足した（ほかのパラメタ・並び・重複。#649）
//*  2026/11/05  玄人 幸道         Redirect：署名の検証と SAML の取り出しが同じ値を読むこと、SAMLRequest の側、空の RelayState のテストを足した（#649 のレビュー）
//*  2026/11/05  玄人 幸道         Redirect：空の RelayState を、仕様どおりの照合（Basis）と互換の許容（Observation）に分け、壊れた % の名前のテストを足した（#649 のレビュー）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Str;

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
        [Basis("SAML 2.0 Profiles 4.1.4.2（エラーを返すなら Assertion を含めない。成功なら 1 つ以上含める）")]
        [Basis("#598")]
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
        [Basis("SAML 2.0 Bindings 3.4.4.1（DEFLATE → Base64 → URL エンコードして SAMLResponse に載せる。RelayState は別のパラメタ）")]
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
        [Basis("SAML 2.0 Bindings 3.4.4.1（SAMLResponse・RelayState・SigAlg を連ねた文字列に署名する）")]
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

        /// <summary>
        /// Redirect：署名の対象は SAMLRequest（SAMLResponse）・RelayState・SigAlg の 3 つだけで、受け取ったままの値で組み立てる。
        /// ほかのパラメタ（Web Forms のウィンドウ GUID など）や、パラメタの並びの違い、先頭の "?" があっても検証できる。
        /// 署名に関わる値の書き換え・重複・欠けは通らない（#649）。
        /// </summary>
        [Test]
        [Basis("SAML 2.0 Bindings 3.4.4.1（SAMLRequest / SAMLResponse・RelayState・SigAlg をこの順に連ねた文字列に署名する。検証は受け取った値で行う）")]
        [Basis("#649")]
        public void Redirect_SignedPartOnly()
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;

            using (RSA rsa = RSA.Create(2048))
            {
                DigitalSignParam ds = new DigitalSignParam(rsa.ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "relay01", ds);

                // パラメタを名前で分ける（値は受け取ったまま）
                Dictionary<string, string> p = new Dictionary<string, string>();
                foreach (string pair in query.Split('&'))
                {
                    int i = pair.IndexOf('=');
                    p.Add(pair.Substring(0, i), pair.Substring(i + 1));
                }
                Assert.That(p.Keys, Is.EquivalentTo(new[] { "SAMLResponse", "RelayState", "SigAlg", "Signature" }));

                Func<string, string, string> param = (name, value) => name + "=" + value;
                string res = param("SAMLResponse", p["SAMLResponse"]);
                string relay = param("RelayState", p["RelayState"]);
                string sigAlg = param("SigAlg", p["SigAlg"]);
                string sig = param("Signature", p["Signature"]);

                // 通るもの
                foreach (KeyValuePair<string, string> v in new Dictionary<string, string>
                {
                    { "そのまま", query },
                    { "後ろにほかのパラメタ（BrowserWindowGUID）", query + "&BrowserWindowGUID=e4c6911f-b820-40e1-8946-e8753f22b499" },
                    { "前にほかのパラメタ", "foo=bar&" + query },
                    { "Signature が先頭", string.Join("&", sig, res, relay, sigAlg) },
                    { "並びが違う", string.Join("&", sigAlg, relay, sig, res) },
                    { "先頭に ?", "?" + query },
                })
                {
                    Assert.That(SAML2Bindings.VerifyRedirect(v.Value, ds), Is.True, v.Key + "（DigitalSign 版）");
                    Assert.That(SAML2Bindings.VerifyRedirect(v.Value, rsa), Is.True, v.Key + "（RSA 版）");
                }

                // 断るもの
                foreach (KeyValuePair<string, string> v in new Dictionary<string, string>
                {
                    { "RelayState の書き換え", string.Join("&", res, "RelayState=relay02", sigAlg, sig) },
                    { "RelayState を消す", string.Join("&", res, sigAlg, sig) },
                    { "SAMLResponse の重複", query + "&" + res },
                    { "RelayState の重複", query + "&RelayState=relay02" },
                    { "SAMLRequest と SAMLResponse の両方", query + "&SAMLRequest=" + p["SAMLResponse"] },
                    { "Signature が無い", string.Join("&", res, relay, sigAlg) },
                    { "SigAlg が無い", string.Join("&", res, relay, sig) },
                    { "SAMLResponse が無い", string.Join("&", relay, sigAlg, sig) },
                    { "空", "" },
                })
                {
                    Assert.That(SAML2Bindings.VerifyRedirect(v.Value, ds), Is.False, v.Key + "（DigitalSign 版）");
                    Assert.That(SAML2Bindings.VerifyRedirect(v.Value, rsa), Is.False, v.Key + "（RSA 版）");
                }
            }
        }

        /// <summary>
        /// Redirect：署名を確かめる読み方と、SAML を取り出す読み方（DecodeRedirect）は同じ。
        /// パラメタ名は完全一致で見るので、名前の一部に SAMLResponse を含む別のパラメタは、どちらも読まない。
        /// エンコードされた名前で署名に関わるパラメタを名乗るものは、どちらも読まない（曖昧なため）（#649）。
        /// </summary>
        [Test]
        [Basis("SAML 2.0 Bindings 3.4.4.1（署名の対象は SAMLRequest / SAMLResponse・RelayState・SigAlg）")]
        [Basis("設計：SAML2Bindings.TryReadRedirectParameters の remarks（署名の検証と SAML の取り出しは、同じ読み方で読む）")]
        [Basis("#649")]
        public void Redirect_VerifyAndDecodeReadTheSameValue()
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;
            string other = CreateResponse(SAML2Enum.StatusCode.Requester, false).OuterXml;

            using (RSA rsa = RSA.Create(2048))
            {
                DigitalSignParam ds = new DigitalSignParam(rsa.ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "relay01", ds);

                // 別の SAML を、名前の一部に SAMLResponse を含む別のパラメタに入れる
                string otherValue = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, other, "", null).Split('&')[0].Split('=')[1];

                foreach (string q in new string[] {
                    "xSAMLResponse=" + otherValue + "&" + query,
                    query + "&xSAMLResponse=" + otherValue,
                })
                {
                    Assert.That(SAML2Bindings.VerifyRedirect(q, rsa), Is.True, "署名に関わらないパラメタは無視する : " + q.Substring(0, 16));
                    Assert.That(SAML2Bindings.DecodeRedirect(q), Is.EqualTo(saml), "取り出すのは、署名を確かめた SAML");
                }

                // エンコードされた名前で、署名に関わるパラメタを名乗る
                foreach (string q in new string[] {
                    "SAML%52esponse=" + otherValue + "&" + query,
                    query + "&Relay%53tate=relay02",
                })
                {
                    Assert.That(SAML2Bindings.VerifyRedirect(q, rsa), Is.False, "曖昧な名前は読まない（検証）");
                    Assert.That(SAML2Bindings.VerifyRedirect(q, ds), Is.False, "曖昧な名前は読まない（検証。DigitalSign 版）");
                    Assert.That(SAML2Bindings.DecodeRedirect(q), Is.Empty, "曖昧な名前は読まない（取り出し）");
                }

                // 重複・両方は、取り出しも空
                Assert.That(SAML2Bindings.DecodeRedirect(query + "&SAMLResponse=" + otherValue), Is.Empty, "SAMLResponse の重複");
                Assert.That(SAML2Bindings.DecodeRedirect(query + "&SAMLRequest=" + otherValue), Is.Empty, "SAMLRequest と SAMLResponse の両方");
            }
        }

        /// <summary>
        /// Redirect：SAMLRequest の側も、署名の対象は 3 つのパラメタだけ。ほかのパラメタが付いていても検証でき、取り出せる（#649）。
        /// </summary>
        [Test]
        [Basis("SAML 2.0 Bindings 3.4.4.1（SAMLRequest・RelayState・SigAlg を連ねた文字列に署名する）")]
        [Basis("#649")]
        public void Redirect_Request_SignedPartOnly()
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;

            using (RSA rsa = RSA.Create(2048))
            {
                DigitalSignParam ds = new DigitalSignParam(rsa.ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Request, saml, "relay01", ds);
                Assert.That(query, Does.StartWith("SAMLRequest="));

                string withGuid = query + "&BrowserWindowGUID=e4c6911f-b820-40e1-8946-e8753f22b499";
                Assert.That(SAML2Bindings.VerifyRedirect(withGuid, rsa), Is.True, "ほかのパラメタ（RSA 版）");
                Assert.That(SAML2Bindings.VerifyRedirect(withGuid, ds), Is.True, "ほかのパラメタ（DigitalSign 版）");
                Assert.That(SAML2Bindings.DecodeRedirect(withGuid), Is.EqualTo(saml), "取り出し");
            }
        }

        /// <summary>
        /// Redirect：RelayState の値が無ければ、パラメタ全体を署名の演算から除外する（送る側の EncodeAndSignRedirect も付けない）。
        /// 空の "RelayState=" が付いていても、含めない版で検証できる。
        /// </summary>
        [Test]
        [Basis("SAML 2.0 Bindings 3.4.4.1（RelayState の値が無ければ、パラメタ全体を署名の演算から除外する。空のパラメタとして含めてはならない）")]
        [Basis("#649")]
        public void Redirect_EmptyRelayStateIsNotSigned()
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;

            using (RSA rsa = RSA.Create(2048))
            {
                DigitalSignParam ds = new DigitalSignParam(rsa.ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "", ds);
                Assert.That(query, Does.Not.Contain("RelayState="), "送る側は、空の RelayState を付けない");

                string[] p = query.Split('&');
                string withEmpty = p[0] + "&RelayState=&" + p[1] + "&" + p[2];
                Assert.That(SAML2Bindings.VerifyRedirect(withEmpty, rsa), Is.True, "空の RelayState= は、含めない版で照合する（RSA 版）");
                Assert.That(SAML2Bindings.VerifyRedirect(withEmpty, ds), Is.True, "空の RelayState= は、含めない版で照合する（DigitalSign 版）");
            }
        }

        /// <summary>
        /// Redirect：空の "RelayState=" を含めて署名した相手とも、互換のために照合できる（含める版も候補にする）。
        /// どちらの候補も全体に署名が掛かっているので、SAMLResponse を書き換えれば通らない。
        /// </summary>
        [Test]
        [Observation("空の RelayState= を含めて署名するのは、SAML 2.0 Bindings 3.4.4.1（空のパラメタとして含めてはならない）に反するが、相互運用のために受ける（施主の判断。#649）。2026/11/05 までの版（クエリ文字列の全体を署名の対象にした）も、この形を受けていた")]
        public void Redirect_EmptyRelayStateSignedByPeer()
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;

            using (RSA rsa = RSA.Create(2048))
            {
                DigitalSignParam ds = new DigitalSignParam(rsa.ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string[] p = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "", ds).Split('&');

                // 相手が、空の RelayState= を含めた文字列に署名した
                string signedByPeer = p[0] + "&RelayState=&" + p[1];
                string signature = CustomEncode.UrlEncode(CustomEncode.ToBase64String(rsa.SignData(
                    CustomEncode.StringToByte(signedByPeer, CustomEncode.us_ascii), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)));
                string query = signedByPeer + "&Signature=" + signature;

                Assert.That(SAML2Bindings.VerifyRedirect(query, rsa), Is.True, "含める版で通る（RSA 版）");
                Assert.That(SAML2Bindings.VerifyRedirect(query, ds), Is.True, "含める版で通る（DigitalSign 版）");

                // RelayState を消すと、どちらの候補とも合わない
                Assert.That(SAML2Bindings.VerifyRedirect(p[0] + "&" + p[1] + "&Signature=" + signature, rsa), Is.False,
                    "RelayState= を消すと通らない");

                // SAMLResponse を書き換えると、どちらの候補とも合わない
                string other = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response,
                    CreateResponse(SAML2Enum.StatusCode.Requester, false).OuterXml, "", null);
                Assert.That(SAML2Bindings.VerifyRedirect(other + "&RelayState=&" + p[1] + "&Signature=" + signature, rsa), Is.False,
                    "SAMLResponse を書き換えると通らない");
            }
        }

        /// <summary>
        /// Redirect：名前に壊れた "%" の並びがあっても、例外にしない（署名に関わらない名前として無視する）。
        /// </summary>
        [Test]
        [Basis("設計：SAML2Bindings.TryReadRedirectParameters の remarks（ほかのパラメタは無視する。外から来る値なので例外にしない）")]
        [Basis("#649")]
        public void Redirect_BrokenPercentInName()
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;

            using (RSA rsa = RSA.Create(2048))
            {
                DigitalSignParam ds = new DigitalSignParam(rsa.ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "relay01", ds);

                foreach (string name in new string[] { "%", "%Z1", "a%2", "%%%", "%E3%81" })
                {
                    string q = query + "&" + name + "=x";
                    bool verified = false;
                    string decoded = null;
                    Assert.DoesNotThrow(() => verified = SAML2Bindings.VerifyRedirect(q, rsa), "検証で例外にしない : " + name);
                    Assert.DoesNotThrow(() => decoded = SAML2Bindings.DecodeRedirect(q), "取り出しで例外にしない : " + name);
                    Assert.That(verified, Is.True, "署名に関わらない名前として無視する : " + name);
                    Assert.That(decoded, Is.EqualTo(saml), "取り出しも同じ : " + name);
                }
            }
        }

        /// <summary>POST：XML 署名した応答は、Base64 を戻して、同じ鍵で検証できる。別の鍵では通らない。</summary>
        [Test]
        [Basis("SAML 2.0 Core 3.2.2（署名があれば、受け手は検証し、無効なら内容に頼らない）")]
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
        [Basis("SAML 2.0 Bindings 3.5.4（XML を Base64 にしてフォームに載せる）")]
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

        /// <summary>Redirect の SigAlg を読む（URL デコード済み）</summary>
        private static string SigAlgOf(string query)
        {
            return CustomEncode.UrlDecode(UriOperator.GetParameterFromQueryString(SAML2Const.QueryParamSigAlg, query));
        }

        /// <summary>
        /// Redirect：SigAlg は DigitalSign のハッシュで決まる（SHA1 / 256 / 384 / 512）。
        /// RSA 版の VerifyRedirect は SigAlg に従って検証し、受け入れる一覧に無ければ通さない。
        /// </summary>
        [TestCase("SHA1", SAML2Const.RSAwithSHA1)]
        [TestCase("SHA256", SAML2Const.RSAwithSHA256)]
        [TestCase("SHA384", SAML2Const.RSAwithSHA384)]
        [TestCase("SHA512", SAML2Const.RSAwithSHA512)]
        [Basis("SAML 2.0 Bindings 3.4.4.1（SigAlg は、署名に使ったアルゴリズムを示す URI）")]
        [Basis("設計：SAML2Bindings.VerifyRedirect(queryString, rsa, sigAlgs) の remarks（SigAlg からハッシュを選ぶ。sigAlgs に無ければ検証せずに失敗）")]
        [Basis("#604")]
        public void Redirect_SigAlgByHash(string hash, string expectedSigAlg)
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;

            using (RSA rsa = RSA.Create(2048))
            {
                DigitalSignParam ds = new DigitalSignParam(rsa.ExportParameters(true), new HashAlgorithmName(hash), RSASignaturePadding.Pkcs1);
                string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "", ds);

                Assert.That(SigAlgOf(query), Is.EqualTo(expectedSigAlg));
                Assert.That(SAML2Bindings.VerifyRedirect(query, ds), Is.True, "DigitalSign 版");
                Assert.That(SAML2Bindings.VerifyRedirect(query, rsa, new string[] { expectedSigAlg }), Is.True, "RSA 版（一覧に在る）");

                // 既定の一覧は SHA256 / 384 / 512（SHA1 は受けない）
                Assert.That(SAML2Bindings.VerifyRedirect(query, rsa), Is.EqualTo(hash != "SHA1"));
            }
        }

        /// <summary>
        /// Redirect：RSA 以外の SigAlg は、受け入れる一覧に在っても RSA 版では通さない。
        /// DigitalSign 版は、DigitalSign のアルゴリズムと SigAlg が食い違えば通さない。
        /// </summary>
        [Test]
        [Basis("設計：SAML2Bindings.VerifyRedirect(queryString, ds) の remarks（SigAlg が DigitalSign のアルゴリズムを示していなければ失敗）")]
        [Basis("#604")]
        public void Redirect_SigAlgMismatch()
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;

            using (RSA rsa = RSA.Create(2048))
            {
                DigitalSignParam sha256 = new DigitalSignParam(rsa.ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                DigitalSignParam sha512 = new DigitalSignParam(rsa.ExportParameters(true), HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
                string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "", sha256);

                Assert.That(SAML2Bindings.VerifyRedirect(query, sha512), Is.False, "SigAlg と DigitalSign のハッシュが違う");

                string dsaSigAlg = query.Replace(
                    CustomEncode.UrlEncode(SAML2Const.RSAwithSHA256), CustomEncode.UrlEncode(SAML2Const.DSAwithSHA1));
                Assert.That(SAML2Bindings.VerifyRedirect(dsaSigAlg, rsa, new string[] { SAML2Const.DSAwithSHA1 }), Is.False, "RSA 以外");
            }
        }

        /// <summary>
        /// Redirect：RSA-PSS は、SigAlg を RFC 9231 の sha*-rsa-MGF1 にする。DigitalSign 版で検証でき、
        /// RSA 版は、一覧に並べたときだけ PSS で検証する（**既定の一覧には PSS を入れない**）。
        /// 2026/10/30 まで、SigAlg を RSAwithSHA1 と書いていた（#643）。
        /// </summary>
        [TestCase("SHA256", SAML2Const.RSAPSSwithSHA256)]
        [TestCase("SHA384", SAML2Const.RSAPSSwithSHA384)]
        [TestCase("SHA512", SAML2Const.RSAPSSwithSHA512)]
        [Basis("SAML 2.0 Bindings 3.4.4.1（SigAlg は、署名に使ったアルゴリズムを示す URI）")]
        [Basis("RFC 9231 2.3.10（RSASSA-PSS の URI。ソルト長はハッシュ長）")]
        [Basis("設計：SAML2Bindings.VerifyRedirect(queryString, rsa, sigAlgs) の remarks（RSA-PSS は既定では受け入れない）")]
        [Basis("#643")]
        public void Redirect_SigAlgOfPss(string hash, string expectedSigAlg)
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;

            using (RSA rsa = RSA.Create(2048))
            {
                DigitalSignParam pss = new DigitalSignParam(rsa.ExportParameters(true), new HashAlgorithmName(hash), RSASignaturePadding.Pss);
                string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "", pss);

                Assert.That(SigAlgOf(query), Is.EqualTo(expectedSigAlg));
                Assert.That(SAML2Bindings.VerifyRedirect(query, pss), Is.True, "DigitalSign 版");
                Assert.That(SAML2Bindings.VerifyRedirect(query, rsa, new string[] { expectedSigAlg }), Is.True, "RSA 版（一覧に在る）");
                Assert.That(SAML2Bindings.VerifyRedirect(query, rsa), Is.False, "RSA 版（既定の一覧に PSS は無い）");

                // PKCS#1 v1.5 の SigAlg に書き換えると、署名の対象が変わるので通らない
                string pkcs1 = query.Replace(CustomEncode.UrlEncode(expectedSigAlg), CustomEncode.UrlEncode(SAML2Const.RSAwithSHA256));
                Assert.That(SAML2Bindings.VerifyRedirect(pkcs1, rsa), Is.False, "SigAlg の書き換え");
            }
        }

        /// <summary>
        /// Redirect：ECDSA は、SigAlg を RFC 9231 の ecdsa-sha* にする（Windows の CNG）。
        /// 公開鍵だけの DigitalSign でも検証できる。ハッシュを渡さなければ SHA-256（ECDsaCng の既定）。
        /// 2026/10/30 まで、SigAlg を RSAwithSHA1 と書いていた（#643）。
        /// </summary>
        [TestCase(EnumDigitalSignAlgorithm.ECDsaCng_P256, null, SAML2Const.ECDSAwithSHA256)]
        [TestCase(EnumDigitalSignAlgorithm.ECDsaCng_P256, "SHA256", SAML2Const.ECDSAwithSHA256)]
        [TestCase(EnumDigitalSignAlgorithm.ECDsaCng_P384, "SHA384", SAML2Const.ECDSAwithSHA384)]
        [TestCase(EnumDigitalSignAlgorithm.ECDsaCng_P521, "SHA512", SAML2Const.ECDSAwithSHA512)]
        [Platform("Win")]
        [Basis("SAML 2.0 Bindings 3.4.4.1（SigAlg は、署名に使ったアルゴリズムを示す URI）")]
        [Basis("RFC 9231 2.3.6（ECDSA の URI）")]
        [Basis("#643")]
        public void Redirect_SigAlgOfEcdsa(EnumDigitalSignAlgorithm eaa, string hash, string expectedSigAlg)
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;

            HashAlgorithm ha = hash == null ? null : HashAlgorithmCmnFunc.GetHashAlgorithmFromNameString(hash);
            HashAlgorithm ha2 = hash == null ? null : HashAlgorithmCmnFunc.GetHashAlgorithmFromNameString(hash);

            using (DigitalSignECDsaCng signer = new DigitalSignECDsaCng(eaa, ha))
            using (DigitalSignECDsaCng verifier = new DigitalSignECDsaCng(signer.PublicKey, ha2))
            {
                string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "", signer);

                Assert.That(SigAlgOf(query), Is.EqualTo(expectedSigAlg));
                Assert.That(SAML2Bindings.VerifyRedirect(query, verifier), Is.True, "公開鍵だけの DigitalSign");
            }
        }

        /// <summary>
        /// Redirect：ECDSA でハッシュを渡さないときの署名は、SHA-256 で検証できる（SigAlg の ecdsa-sha256 と合う）。
        /// GetSigAlg は、ハッシュの無い DigitalSignECDsaCng を SHA-256 とみなす。その前提を、SHA-256 を明示した検証で確かめる
        /// （Redirect_SigAlgOfEcdsa は署名と検証で同じ設定を使うので、この前提までは確かめていない。#643）。
        /// </summary>
        [Test]
        [Platform("Win")]
        [Basis("RFC 9231 2.3.6（ecdsa-sha256 は、SHA-256 で計算した ECDSA の署名）")]
        [Basis("#643")]
        public void Redirect_SigAlgOfEcdsa_DefaultHashIsSha256()
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;

            using (DigitalSignECDsaCng signer = new DigitalSignECDsaCng(EnumDigitalSignAlgorithm.ECDsaCng_P256, null))
            using (DigitalSignECDsaCng sha256 = new DigitalSignECDsaCng(
                signer.PublicKey, HashAlgorithmCmnFunc.GetHashAlgorithmFromNameString("SHA256")))
            using (DigitalSignECDsaCng sha384 = new DigitalSignECDsaCng(
                signer.PublicKey, HashAlgorithmCmnFunc.GetHashAlgorithmFromNameString("SHA384")))
            {
                string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "", signer);

                Assert.That(SigAlgOf(query), Is.EqualTo(SAML2Const.ECDSAwithSHA256));
                Assert.That(SAML2Bindings.VerifyRedirect(query, sha256), Is.True, "SHA-256 を明示した検証");

                // SigAlg の照合を通さずに、ハッシュの違いだけで結果が分かれることも見る
                byte[] data = CustomEncode.StringToByte("Redirect_SigAlgOfEcdsa_DefaultHashIsSha256", CustomEncode.UTF_8);
                byte[] sign = signer.Sign(data);
                Assert.That(sha256.Verify(data, sign), Is.True, "SHA-256");
                Assert.That(sha384.Verify(data, sign), Is.False, "SHA-384");
            }
        }

        /// <summary>
        /// Redirect：DSA は DSAwithSHA1。SigAlg を決められないアルゴリズム（RSA-PSS の SHA1 など）は、
        /// EncodeAndSignRedirect が ArgumentException（誤った SigAlg を出さない）、VerifyRedirect は false。
        /// 2026/10/30 まで、決められないものは RSAwithSHA1 と書いていた（#643）。
        /// </summary>
        [Test]
        [Basis("SAML 2.0 Bindings 3.4.4.1（SigAlg は、署名に使ったアルゴリズムを示す URI）")]
        [Basis("RFC 9231 2.3.10（RSASSA-PSS の URI に SHA-1 の版は無い）")]
        [Basis("設計：SAML2Bindings.EncodeAndSignRedirect の remarks（それ以外のアルゴリズムは ArgumentException）")]
        [Basis("#643")]
        public void Redirect_SigAlgOfOtherAlgorithms()
        {
            string saml = CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;

            DigitalSignParam dsa = new DigitalSignParam(EnumDigitalSignAlgorithm.DsaCSP_SHA1);
            string dsaQuery = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "", dsa);
            Assert.That(SigAlgOf(dsaQuery), Is.EqualTo(SAML2Const.DSAwithSHA1));
            Assert.That(SAML2Bindings.VerifyRedirect(dsaQuery, dsa), Is.True);

            using (RSA rsa = RSA.Create(2048))
            {
                DigitalSignParam pssSha1 = new DigitalSignParam(rsa.ExportParameters(true), HashAlgorithmName.SHA1, RSASignaturePadding.Pss);
                Assert.Throws<ArgumentException>(() =>
                    SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "", pssSha1));

                DigitalSignParam sha256 = new DigitalSignParam(rsa.ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                string query = SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, saml, "", sha256);
                Assert.That(SAML2Bindings.VerifyRedirect(query, pssSha1), Is.False, "決められないアルゴリズムでは検証しない");
            }
        }

        /// <summary>
        /// 空の入力：EncodeAndSignRedirect は空文字、DecodeRedirect は SAMLRequest / SAMLResponse が無ければ空文字。
        /// 各 Get*In* は、要素が無ければ空文字。
        /// </summary>
        [Test]
        public void EmptyInputs()
        {
            Assert.That(SAML2Bindings.EncodeAndSignRedirect(SAML2Enum.RequestOrResponse.Response, "", "relay01"), Is.Empty);
            Assert.That(SAML2Bindings.DecodeRedirect("RelayState=relay01"), Is.Empty);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<a/>");
            XmlNamespaceManager ns = SAML2Bindings.CreateNamespaceManager(doc);

            Assert.That(SAML2Bindings.GetIssuerInResponse(doc, ns), Is.Empty);
            Assert.That(SAML2Bindings.GetIssuerInAssertion(doc, ns), Is.Empty);
            Assert.That(SAML2Bindings.GetAudienceInAssertion(doc, ns), Is.Empty);
            Assert.That(SAML2Bindings.GetAuthnContextClassRefInAssertion(doc, ns), Is.Empty);
            Assert.That(SAML2Bindings.GetIssuerInRequest(doc, ns), Is.Empty);
        }

        /// <summary>メタデータ：署名用の証明書は、1 本目の use="signing" の KeyDescriptor の後ろに足す（暗号化用は飛ばす）。</summary>
        [Test]
        [Basis("設計：SAML2Bindings.AppendSigningCertificates(metadata, certificates) の summary・remarks（use=\"signing\" の 1 本目を複製し、その後ろに並べる）")]
        public void AppendSigningCertificates_SkipsEncryptionKey()
        {
            XmlDocument md = SAML2Bindings.CreateMetadata("https://idp.example.com/", "CURRENT",
                new[] { SAML2Enum.NameIDFormat.Unspecified }, "https://idp.example.com/redirect", "https://idp.example.com/post");

            XmlNamespaceManager ns = new XmlNamespaceManager(md.NameTable);
            ns.AddNamespace(SAML2Const.PrefixMetadata, SAML2Const.UrnMetadata);
            ns.AddNamespace(SAML2Const.PrefixDSig, SAML2Const.UrnDSig);

            // 先頭に暗号化用の KeyDescriptor を置く
            XmlNode signing = md.SelectSingleNode(SAML2Const.XPathKeyDescriptorInMetadata, ns);
            XmlElement encryption = (XmlElement)signing.CloneNode(true);
            encryption.SetAttribute(SAML2Const.AttrUse, "encryption");
            encryption.SelectSingleNode(SAML2Const.XPathX509CertificateInKeyDescriptor, ns).InnerText = "ENC";
            signing.ParentNode.InsertBefore(encryption, signing);

            Assert.That(SAML2Bindings.AppendSigningCertificates(md, new string[] { "OLD" }), Is.EqualTo(1));

            XmlNodeList keys = md.SelectNodes(SAML2Const.XPathKeyDescriptorInMetadata, ns);
            Assert.That(keys.Count, Is.EqualTo(3));
            Assert.That(keys[0].SelectSingleNode(SAML2Const.XPathX509CertificateInKeyDescriptor, ns).InnerText, Is.EqualTo("ENC"));
            Assert.That(keys[1].SelectSingleNode(SAML2Const.XPathX509CertificateInKeyDescriptor, ns).InnerText, Is.EqualTo("CURRENT"));
            Assert.That(keys[2].SelectSingleNode(SAML2Const.XPathX509CertificateInKeyDescriptor, ns).InnerText, Is.EqualTo("OLD"));
        }
    }
}

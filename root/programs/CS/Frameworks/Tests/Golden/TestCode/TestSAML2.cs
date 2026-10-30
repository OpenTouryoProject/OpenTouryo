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
//* クラス名        ：TestSAML2
//* クラス日本語名  ：Framework.Authentication.SAML2Bindingsのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/07  玄人 幸道         新規作成（#598）
//*  2026/10/11  玄人 幸道         SAML2Client.VerifyResponseの、証明書を渡す版のテストを追加（#600）
//*  2026/10/12  玄人 幸道         EncodeAndSignRedirectのSigAlgと、VerifyRedirectのSigAlgの扱いのテストを追加（#604）
//*  2026/10/14  玄人 幸道         SAML2Client.CreateRedirectRequest / CreatePostRequestの、鍵を渡す版のテストを追加（#606）
//*  2026/10/22  玄人 幸道         StringExtractor.GetParameterFromQueryString（Obsolete）を UriOperator に置き換えた（#634）
//*  2026/10/30  玄人 幸道         RSA-PSS の SigAlg を、RFC 9231 の sha256-rsa-MGF1 にした（#643）
//**********************************************************************************

using System;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Framework.Authentication.SAML2Bindingsのテスト</summary>
    /// <remarks>
    /// 応答（Response）の構造検証（VerifyByXPath）（#598）と、
    /// 証明書を渡す版の VerifyResponse（#600）と、
    /// Redirect の SigAlg が実際の署名アルゴリズムを示すこと（#604）と、
    /// 鍵を渡す版の CreateRedirectRequest / CreatePostRequest（#606）。
    ///
    /// ＜期待値は、SAML 2.0 Core の定めから置く＞
    ///   成功応答は Assertion を持ち、エラー応答は Status だけで正当である（3.2.2 / 4.1.4.2）。
    ///   2026/10/07 まで、VerifyByXPath は StatusCode を見ずに Assertion を必須にしており、
    ///   CreateResponse が作るエラー応答を、同じライブラリの検証が弾いていた。
    ///
    /// ＜ID・日時は出力しない＞
    ///   CreateResponse / CreateAssertion は GUID と現在時刻を埋めるため、
    ///   XML そのものは実行のたびに変わる。出すのは検証の結果（bool）だけにする。
    ///
    /// ＜鍵は、実行のたびに作る＞
    ///   IdP の鍵と、それとは別の鍵を作り、証明書（*.cer）を一時ファイルに書き出して渡す。
    ///   一時ファイルは、テストの終わりに消す。
    /// </remarks>
    public class TestSAML2
    {
        #region 固定値

        /// <summary>IdP の Issuer</summary>
        private const string Issuer = "https://idp.example.com/";

        /// <summary>SP の Assertion Consumer Service</summary>
        private const string Recipient = "https://sp.example.com/acs";

        /// <summary>要求の ID</summary>
        private const string InResponseTo = "s2request";

        #endregion

        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            TestSAML2.TestVerifyByXPathOfResponse();
            TestSAML2.TestVerifyResponseByCerFilePath();
            TestSAML2.TestSigAlgOfRedirect();
            TestSAML2.TestCreateRequestByKey();
        }

        #endregion

        #region private

        /// <summary>VerifyByXPath（Response）</summary>
        /// <remarks>
        /// Assertion が必須なのは、StatusCode が Success のときだけである。
        /// </remarks>
        private static void TestVerifyByXPathOfResponse()
        {
            MyDebug.OutputDebugAndConsole("SAML2Bindings.VerifyByXPath（Response）");

            // 成功応答
            TestSAML2.OutputVerifyByXPath("Success・Assertion あり",
                TestSAML2.CreateResponse(SAML2Enum.StatusCode.Success, true), true);

            // 成功応答なのに Assertion が無い（認証結果が無いので、使えない）
            TestSAML2.OutputVerifyByXPath("Success・Assertion なし",
                TestSAML2.CreateResponse(SAML2Enum.StatusCode.Success, false), false);

            // エラー応答（#598。CreateResponse の戻り値そのまま）
            TestSAML2.OutputVerifyByXPath("Requester・Assertion なし",
                TestSAML2.CreateResponse(SAML2Enum.StatusCode.Requester, false), true);

            TestSAML2.OutputVerifyByXPath("Responder・Assertion なし",
                TestSAML2.CreateResponse(SAML2Enum.StatusCode.Responder, false), true);

            // エラー応答に Assertion を繋ぐ IdP もある（従来どおり受け付ける）
            TestSAML2.OutputVerifyByXPath("Requester・Assertion あり",
                TestSAML2.CreateResponse(SAML2Enum.StatusCode.Requester, true), true);

            // Assertion が在るなら、StatusCode に依らず構造を見る
            XmlDocument broken = TestSAML2.CreateResponse(SAML2Enum.StatusCode.Requester, true);
            TestSAML2.RemoveNode(broken, SAML2Const.XPathIssuerInAssertion);
            TestSAML2.OutputVerifyByXPath("Requester・Assertion に Issuer なし", broken, false);

            // Status は、エラー応答でも必須
            XmlDocument noStatus = TestSAML2.CreateResponse(SAML2Enum.StatusCode.Requester, false);
            TestSAML2.RemoveNode(noStatus, SAML2Const.XPathResponse + "/samlp:Status");
            TestSAML2.OutputVerifyByXPath("Status なし", noStatus, false);
        }

        /// <summary>VerifyResponse（cerFilePath）</summary>
        /// <remarks>
        /// 渡した証明書で検証されること（#600）。
        /// 2026/10/11 まで、検証の証明書は CmnClientParams.RsaCerFilePath に固定されていた。
        /// </remarks>
        private static void TestVerifyResponseByCerFilePath()
        {
            MyDebug.OutputDebugAndConsole("SAML2Client.VerifyResponse（cerFilePath）");

            string idpCerFilePath = Path.GetTempFileName();
            string otherCerFilePath = Path.GetTempFileName();

            try
            {
                using (RSA idpRsa = RSA.Create(2048))
                using (RSA otherRsa = RSA.Create(2048))
                {
                    File.WriteAllBytes(idpCerFilePath, TestSAML2.CreateCer(idpRsa));
                    File.WriteAllBytes(otherCerFilePath, TestSAML2.CreateCer(otherRsa));

                    XmlDocument response = TestSAML2.CreateResponse(SAML2Enum.StatusCode.Success, true);
                    string id = SAML2Bindings.GetIdInResponse(
                        response, SAML2Bindings.CreateNamespaceManager(response));

                    // POST（XML の署名）
                    string samlResponse = SAML2Bindings.EncodeAndSignPost(response.OuterXml, id, idpRsa);

                    TestSAML2.OutputVerifyResponse("POST・IdP の証明書",
                        null, samlResponse, idpCerFilePath, true);
                    TestSAML2.OutputVerifyResponse("POST・別の証明書",
                        null, samlResponse, otherCerFilePath, false);

                    // Redirect（クエリ文字列の署名）
                    Touryo.Infrastructure.Public.Security.DigitalSignParam dsParam =
                        new Touryo.Infrastructure.Public.Security.DigitalSignParam(
                            idpRsa.ExportParameters(true), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    string queryString = SAML2Bindings.EncodeAndSignRedirect(
                        SAML2Enum.RequestOrResponse.Response, response.OuterXml, "", dsParam);

                    TestSAML2.OutputVerifyResponse("Redirect・IdP の証明書",
                        queryString, null, idpCerFilePath, true);
                    TestSAML2.OutputVerifyResponse("Redirect・別の証明書",
                        queryString, null, otherCerFilePath, false);
                }
            }
            finally
            {
                File.Delete(idpCerFilePath);
                File.Delete(otherCerFilePath);
            }
        }

        /// <summary>EncodeAndSignRedirect の SigAlg と、VerifyRedirect の SigAlg の扱い</summary>
        /// <remarks>
        /// SigAlg は、渡した DigitalSign のアルゴリズムを示すこと（SAML 2.0 Bindings 3.4.4.1）。
        /// 2026/10/12 まで rsa-sha1 固定で、SHA256 の署名にも rsa-sha1 と書いていた（#604 の案 (1)）。
        ///
        /// VerifyRedirect（DigitalSign 版）は、SigAlg が DigitalSign のアルゴリズムと一致しなければ失敗する（案 (2)）。
        /// VerifyRedirect（RSA 版）は、許可リストの中から SigAlg でハッシュを選ぶ。既定で SHA1 は受けない（案 (3)）。
        /// </remarks>
        private static void TestSigAlgOfRedirect()
        {
            MyDebug.OutputDebugAndConsole("SAML2Bindings.EncodeAndSignRedirect（SigAlg） / VerifyRedirect");

            string saml = TestSAML2.CreateResponse(SAML2Enum.StatusCode.Success, true).OuterXml;

            using (RSA rsa = RSA.Create(2048))
            {
                RSAParameters key = rsa.ExportParameters(true);

                TestSAML2.OutputSigAlg("RSA・SHA1", saml, key,
                    HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1, SAML2Const.RSAwithSHA1, false);
                TestSAML2.OutputSigAlg("RSA・SHA256", saml, key,
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1, SAML2Const.RSAwithSHA256, true);
                TestSAML2.OutputSigAlg("RSA・SHA384", saml, key,
                    HashAlgorithmName.SHA384, RSASignaturePadding.Pkcs1, SAML2Const.RSAwithSHA384, true);
                TestSAML2.OutputSigAlg("RSA・SHA512", saml, key,
                    HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1, SAML2Const.RSAwithSHA512, true);

                // PSS は、RFC 9231 2.3.10 の sha256-rsa-MGF1 と書く（2026/10/30 まで rsa-sha1。#643）。
                // RSA 版は、既定の一覧に PSS を入れないので、通らない。
                TestSAML2.OutputSigAlg("RSA-PSS・SHA256", saml, key,
                    HashAlgorithmName.SHA256, RSASignaturePadding.Pss, SAML2Const.RSAPSSwithSHA256, false);

                Touryo.Infrastructure.Public.Security.DigitalSignParam ds1 =
                    new Touryo.Infrastructure.Public.Security.DigitalSignParam(
                        key, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                Touryo.Infrastructure.Public.Security.DigitalSignParam ds256 =
                    new Touryo.Infrastructure.Public.Security.DigitalSignParam(
                        key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                Touryo.Infrastructure.Public.Security.DigitalSignParam ds384 =
                    new Touryo.Infrastructure.Public.Security.DigitalSignParam(
                        key, HashAlgorithmName.SHA384, RSASignaturePadding.Pkcs1);

                // SHA1 も、許可リストに入れれば RSA 版で通る（案 (3)）
                string sha1Query = SAML2Bindings.EncodeAndSignRedirect(
                    SAML2Enum.RequestOrResponse.Response, saml, "", ds1);
                TestSAML2.OutputVerifyRedirect("RSA・SHA1、RSA 版に rsa-sha1 を許可する", true,
                    () => SAML2Bindings.VerifyRedirect(sha1Query, rsa, new string[] { SAML2Const.RSAwithSHA1 }));

                // SigAlg と違うハッシュの DigitalSign で検証する（案 (2)）
                string sha384Query = SAML2Bindings.EncodeAndSignRedirect(
                    SAML2Enum.RequestOrResponse.Response, saml, "", ds384);
                TestSAML2.OutputVerifyRedirect("rsa-sha384 を、SHA256 の DigitalSign で検証する", false,
                    () => SAML2Bindings.VerifyRedirect(sha384Query, ds256));

                // 2026/10/12 より前の Open棟梁 が送ったもの（rsa-sha1 と名乗る SHA256 の署名）
                string oldQuery = TestSAML2.CreateOldRedirect(saml, ds256);
                TestSAML2.OutputVerifyRedirect("旧版（rsa-sha1 と名乗る SHA256）、DigitalSign 版（SHA256）", false,
                    () => SAML2Bindings.VerifyRedirect(oldQuery, ds256));
                TestSAML2.OutputVerifyRedirect("旧版（rsa-sha1 と名乗る SHA256）、RSA 版（既定）", false,
                    () => SAML2Bindings.VerifyRedirect(oldQuery, rsa));
                TestSAML2.OutputVerifyRedirect("旧版（rsa-sha1 と名乗る SHA256）、RSA 版に rsa-sha1 を許可する", false,
                    () => SAML2Bindings.VerifyRedirect(oldQuery, rsa, new string[] { SAML2Const.RSAwithSHA1 }));

                // SigAlg を書き換えると、署名が合わなくなる
                string tampered = SAML2Bindings.EncodeAndSignRedirect(
                    SAML2Enum.RequestOrResponse.Response, saml, "", ds256).Replace(
                    Touryo.Infrastructure.Public.Str.CustomEncode.UrlEncode(SAML2Const.RSAwithSHA256),
                    Touryo.Infrastructure.Public.Str.CustomEncode.UrlEncode(SAML2Const.RSAwithSHA384));
                TestSAML2.OutputVerifyRedirect("SigAlg を rsa-sha384 に書き換える、DigitalSign 版（SHA384）", false,
                    () => SAML2Bindings.VerifyRedirect(tampered, ds384));
                TestSAML2.OutputVerifyRedirect("SigAlg を rsa-sha384 に書き換える、RSA 版", false,
                    () => SAML2Bindings.VerifyRedirect(tampered, rsa));
            }
        }

        /// <summary>CreateRedirectRequest（digitalSign） / CreatePostRequest（rsa）</summary>
        /// <remarks>
        /// 渡した鍵で署名されること（#606）。
        /// 2026/10/14 まで、署名の鍵は CmnClientParams.RsaPfxFilePath に固定されていた。
        ///
        /// Redirect の署名は、SAML2Bindings を通さず、RSA.VerifyData で直に確かめる
        /// （署名の対象は、クエリ文字列の Signature より前。SAML 2.0 Bindings 3.4.4.1）。
        ///
        /// 鍵を渡さない（null）版は、設定の PFX が要るので、ここでは見ない。
        /// </remarks>
        private static void TestCreateRequestByKey()
        {
            MyDebug.OutputDebugAndConsole("SAML2Client.CreateRedirectRequest（digitalSign） / CreatePostRequest（rsa）");

            using (RSA spRsa = RSA.Create(2048))
            using (RSA otherRsa = RSA.Create(2048))
            {
                // Redirect（クエリ文字列の署名）
                TestSAML2.OutputCreateRedirectRequest("Redirect・SHA256",
                    spRsa, otherRsa, HashAlgorithmName.SHA256, SAML2Const.RSAwithSHA256);

                // ハッシュも、渡した DigitalSign のものになる
                TestSAML2.OutputCreateRedirectRequest("Redirect・SHA384",
                    spRsa, otherRsa, HashAlgorithmName.SHA384, SAML2Const.RSAwithSHA384);

                // POST（XML の署名）
                TestSAML2.OutputCreatePostRequest("POST", spRsa, otherRsa);
            }
        }

        #endregion

        #region 組み立てのヘルパ

        /// <summary>応答を作る</summary>
        /// <param name="statusCode">StatusCode</param>
        /// <param name="withAssertion">Assertion を繋ぐか</param>
        /// <returns>応答</returns>
        private static XmlDocument CreateResponse(SAML2Enum.StatusCode statusCode, bool withAssertion)
        {
            string id = "";

            XmlDocument response = SAML2Bindings.CreateResponse(
                TestSAML2.Issuer, TestSAML2.Recipient, TestSAML2.InResponseTo, statusCode, out id);

            if (withAssertion)
            {
                XmlDocument assertion = SAML2Bindings.CreateAssertion(
                    TestSAML2.InResponseTo, TestSAML2.Issuer, "user01",
                    SAML2Enum.NameIDFormat.Unspecified,
                    SAML2Enum.AuthnContextClassRef.Unspecified,
                    3600, TestSAML2.Recipient, out id);

                response.DocumentElement.AppendChild(
                    response.ImportNode(assertion.DocumentElement, true));
            }

            return response;
        }

        /// <summary>自己署名の証明書（*.cer の中身）を作る</summary>
        /// <param name="rsa">鍵</param>
        /// <returns>DER 形式の証明書</returns>
        private static byte[] CreateCer(RSA rsa)
        {
            CertificateRequest request = new CertificateRequest(
                "CN=" + TestSAML2.Issuer, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            using (X509Certificate2 cert = request.CreateSelfSigned(
                DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(1)))
            {
                return cert.Export(X509ContentType.Cert);
            }
        }

        /// <summary>2026/10/12 より前の Open棟梁 と同じ、SigAlg が rsa-sha1 固定の Redirect を作る</summary>
        /// <param name="saml">応答</param>
        /// <param name="ds">署名に使う DigitalSign</param>
        /// <returns>クエリ文字列</returns>
        private static string CreateOldRedirect(string saml,
            Touryo.Infrastructure.Public.Security.DigitalSign ds)
        {
            // 署名しないで組み立て、SigAlg と Signature を足す
            string queryString = SAML2Bindings.EncodeAndSignRedirect(
                SAML2Enum.RequestOrResponse.Response, saml, "")
                + "&SigAlg=" + Touryo.Infrastructure.Public.Str.CustomEncode.UrlEncode(SAML2Const.RSAwithSHA1);

            return queryString + "&Signature=" + Touryo.Infrastructure.Public.Str.CustomEncode.UrlEncode(
                Touryo.Infrastructure.Public.Str.CustomEncode.ToBase64String(ds.Sign(
                    Touryo.Infrastructure.Public.Str.CustomEncode.StringToByte(
                        queryString, Touryo.Infrastructure.Public.Str.CustomEncode.us_ascii))));
        }

        /// <summary>XPath で指定したノードを取り除く</summary>
        /// <param name="saml">応答</param>
        /// <param name="xPath">取り除くノードの XPath</param>
        private static void RemoveNode(XmlDocument saml, string xPath)
        {
            XmlNode node = saml.SelectSingleNode(
                xPath, SAML2Bindings.CreateNamespaceManager(saml));

            node.ParentNode.RemoveChild(node);
        }

        #endregion

        #region 出力のヘルパ

        /// <summary>VerifyByXPath の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="saml">応答</param>
        /// <param name="expected">期待値（SAML 2.0 Core の定めから置いた値）</param>
        private static void OutputVerifyByXPath(string caseName, XmlDocument saml, bool expected)
        {
            try
            {
                bool actual = SAML2Bindings.VerifyByXPath(
                    saml, SAML2Enum.SamlSchema.Response,
                    SAML2Bindings.CreateNamespaceManager(saml));

                MyDebug.OutputDebugAndConsole(
                    caseName + " : " + actual + "（期待値と一致 : " + (actual == expected) + "）");
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole(caseName + " : 例外 " + ex.GetType().FullName);
            }
        }

        /// <summary>VerifyResponse の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="queryString">Redirect の応答</param>
        /// <param name="samlResponse">POST の応答</param>
        /// <param name="cerFilePath">検証に使う証明書</param>
        /// <param name="expected">期待値</param>
        private static void OutputVerifyResponse(string caseName,
            string queryString, string samlResponse, string cerFilePath, bool expected)
        {
            try
            {
                string nameId = "";
                string iss = "";
                string aud = "";
                string inResponseTo = "";
                string recipient = "";
                DateTime? notOnOrAfter = null;
                SAML2Enum.StatusCode? statusCode = null;
                SAML2Enum.NameIDFormat? nameIDFormat = null;
                SAML2Enum.AuthnContextClassRef? authnContextClassRef = null;
                XmlDocument samlResponse2 = null;

                bool actual = SAML2Client.VerifyResponse(
                    queryString, samlResponse, cerFilePath,
                    out nameId, out iss, out aud,
                    out inResponseTo, out recipient, out notOnOrAfter,
                    out statusCode, out nameIDFormat, out authnContextClassRef,
                    out samlResponse2);

                // iss の照合は呼び出し側の責務なので、取れた値も確かめておく。
                MyDebug.OutputDebugAndConsole(
                    caseName + " : " + actual + "（期待値と一致 : " + (actual == expected) + "）"
                    + (actual ? "、iss 一致 : " + (iss == TestSAML2.Issuer) : ""));
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole(caseName + " : 例外 " + ex.GetType().FullName);
            }
        }

        /// <summary>EncodeAndSignRedirect の SigAlg と、VerifyRedirect の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="saml">応答</param>
        /// <param name="key">鍵</param>
        /// <param name="hashAlgorithmName">ハッシュ</param>
        /// <param name="padding">パディング</param>
        /// <param name="expected">期待する SigAlg</param>
        /// <param name="expectedByRsa">RSA 版の VerifyRedirect（既定の許可リスト）の期待値</param>
        private static void OutputSigAlg(string caseName, string saml, RSAParameters key,
            HashAlgorithmName hashAlgorithmName, RSASignaturePadding padding, string expected, bool expectedByRsa)
        {
            try
            {
                Touryo.Infrastructure.Public.Security.DigitalSignParam ds =
                    new Touryo.Infrastructure.Public.Security.DigitalSignParam(key, hashAlgorithmName, padding);

                string queryString = SAML2Bindings.EncodeAndSignRedirect(
                    SAML2Enum.RequestOrResponse.Response, saml, "", ds);

                string sigAlg = Touryo.Infrastructure.Public.Str.CustomEncode.UrlDecode(
                    Touryo.Infrastructure.Public.Str.UriOperator.GetParameterFromQueryString("SigAlg", queryString));

                bool byRsa = false;
                using (RSA rsa = RSA.Create())
                {
                    rsa.ImportParameters(key);
                    byRsa = SAML2Bindings.VerifyRedirect(queryString, rsa);
                }

                MyDebug.OutputDebugAndConsole(
                    caseName + " : " + sigAlg + "（期待値と一致 : " + (sigAlg == expected) + "）"
                    + "、VerifyRedirect（DigitalSign 版） : " + SAML2Bindings.VerifyRedirect(queryString, ds)
                    + "、VerifyRedirect（RSA 版） : " + byRsa + "（期待値と一致 : " + (byRsa == expectedByRsa) + "）");
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole(caseName + " : 例外 " + ex.GetType().FullName);
            }
        }

        /// <summary>CreateRedirectRequest（digitalSign）の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="spRsa">署名に使う鍵</param>
        /// <param name="otherRsa">それとは別の鍵</param>
        /// <param name="hashAlgorithmName">ハッシュ</param>
        /// <param name="expectedSigAlg">期待する SigAlg</param>
        private static void OutputCreateRedirectRequest(string caseName,
            RSA spRsa, RSA otherRsa, HashAlgorithmName hashAlgorithmName, string expectedSigAlg)
        {
            try
            {
                Touryo.Infrastructure.Public.Security.DigitalSignParam ds =
                    new Touryo.Infrastructure.Public.Security.DigitalSignParam(
                        spRsa.ExportParameters(true), hashAlgorithmName, RSASignaturePadding.Pkcs1);

                string id = "";
                string queryString = SAML2Client.CreateRedirectRequest(
                    SAML2Enum.RequestOrResponse.Request,
                    SAML2Enum.ProtocolBinding.HttpRedirect, SAML2Enum.NameIDFormat.Unspecified,
                    TestSAML2.Recipient, TestSAML2.Recipient, "state", ds, out id);

                string sigAlg = Touryo.Infrastructure.Public.Str.CustomEncode.UrlDecode(
                    Touryo.Infrastructure.Public.Str.UriOperator.GetParameterFromQueryString("SigAlg", queryString));

                // 署名の対象（Signature より前）と、署名
                const string signatureKey = "&Signature=";
                int index = queryString.IndexOf(signatureKey, StringComparison.Ordinal);
                byte[] data = System.Text.Encoding.ASCII.GetBytes(queryString.Substring(0, index));
                byte[] signature = Convert.FromBase64String(
                    Touryo.Infrastructure.Public.Str.CustomEncode.UrlDecode(
                        queryString.Substring(index + signatureKey.Length)));

                // out id は、要求（AuthnRequest）の ID
                XmlDocument request = new XmlDocument();
                request.LoadXml(SAML2Bindings.DecodeRedirect(queryString));
                string idInRequest = SAML2Bindings.GetIdInRequest(
                    request, SAML2Bindings.CreateNamespaceManager(request));

                MyDebug.OutputDebugAndConsole(
                    caseName + " : SigAlg " + sigAlg + "（期待値と一致 : " + (sigAlg == expectedSigAlg) + "）"
                    + "、渡した鍵で検証 : " + spRsa.VerifyData(
                        data, signature, hashAlgorithmName, RSASignaturePadding.Pkcs1)
                    + "、別の鍵で検証 : " + otherRsa.VerifyData(
                        data, signature, hashAlgorithmName, RSASignaturePadding.Pkcs1)
                    + "、id 一致 : " + (!string.IsNullOrEmpty(id) && id == idInRequest));
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole(caseName + " : 例外 " + ex.GetType().FullName);
            }
        }

        /// <summary>CreatePostRequest（rsa）の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="spRsa">署名に使う鍵</param>
        /// <param name="otherRsa">それとは別の鍵</param>
        private static void OutputCreatePostRequest(string caseName, RSA spRsa, RSA otherRsa)
        {
            try
            {
                string id = "";
                string samlRequest = SAML2Client.CreatePostRequest(
                    SAML2Enum.ProtocolBinding.HttpPost, SAML2Enum.NameIDFormat.Unspecified,
                    TestSAML2.Recipient, TestSAML2.Recipient, "state", spRsa, out id);

                string decodeSaml = SAML2Bindings.DecodePost(samlRequest);

                // out id は、要求（AuthnRequest）の ID
                XmlDocument request = new XmlDocument();
                request.LoadXml(decodeSaml);
                string idInRequest = SAML2Bindings.GetIdInRequest(
                    request, SAML2Bindings.CreateNamespaceManager(request));

                MyDebug.OutputDebugAndConsole(
                    caseName + " : 渡した鍵で検証 : " + SAML2Bindings.VerifyPost(decodeSaml, id, spRsa)
                    + "、別の鍵で検証 : " + SAML2Bindings.VerifyPost(decodeSaml, id, otherRsa)
                    + "、id 一致 : " + (!string.IsNullOrEmpty(id) && id == idInRequest));
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole(caseName + " : 例外 " + ex.GetType().FullName);
            }
        }

        /// <summary>VerifyRedirect の結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="expected">期待値</param>
        /// <param name="verify">VerifyRedirect の呼び出し</param>
        private static void OutputVerifyRedirect(string caseName, bool expected, Func<bool> verify)
        {
            try
            {
                bool actual = verify();
                MyDebug.OutputDebugAndConsole(
                    caseName + " : " + actual + "（期待値と一致 : " + (actual == expected) + "）");
            }
            catch (Exception ex)
            {
                // メッセージは環境の言語で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole(caseName + " : 例外 " + ex.GetType().FullName);
            }
        }

        #endregion
    }
}

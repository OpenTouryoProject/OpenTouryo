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
//**********************************************************************************

using System;
using System.Xml;

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Framework.Authentication.SAML2Bindingsのテスト</summary>
    /// <remarks>
    /// 応答（Response）の構造検証（VerifyByXPath）（#598）。
    ///
    /// ＜期待値は、SAML 2.0 Core の定めから置く＞
    ///   成功応答は Assertion を持ち、エラー応答は Status だけで正当である（3.2.2 / 4.1.4.2）。
    ///   2026/10/07 まで、VerifyByXPath は StatusCode を見ずに Assertion を必須にしており、
    ///   CreateResponse が作るエラー応答を、同じライブラリの検証が弾いていた。
    ///
    /// ＜ID・日時は出力しない＞
    ///   CreateResponse / CreateAssertion は GUID と現在時刻を埋めるため、
    ///   XML そのものは実行のたびに変わる。出すのは検証の結果（bool）だけにする。
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

        #endregion
    }
}

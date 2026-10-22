//**********************************************************************************
//* Copyright (C) 2017 Hitachi Solutions,Ltd.
//**********************************************************************************

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
//* クラス名        ：SAML2Bindings
//* クラス日本語名  ：SAML2Bindings（ライブラリ）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/05/21  西野 大介         新規作成
//*  2019/05/29  西野 大介         Create XMLでは署名しない。
//*                                 Encode And Sign / Decode, Verifyで署名・検証する。
//*  2019/06/04  西野 大介         スキーマ検証と属性抽出にはXPathを使用。
//*  2026/10/07  玄人 幸道         VerifyByXPathがStatusCodeを見ずにAssertionを必須にしており、
//*                                Assertionを持たない正当なエラー応答を弾いていたのを修正（#598）
//*  2026/10/12  玄人 幸道         EncodeAndSignRedirectのSigAlgが、実際の署名アルゴリズムに依らず
//*                                rsa-sha1固定になっていたのを修正（#604）
//*                                合わせて、SHA1に限らないため、引数名dsRSAwithSHA1をdsに変更
//*  2026/10/12  玄人 幸道         VerifyRedirectで、SigAlgが渡されたDigitalSignのアルゴリズムと
//*                                一致することを確認するようにした。
//*                                また、SigAlgからハッシュを選ぶ、RSAを受けるオーバーロードを追加（#604）
//*  2026/10/22  玄人 幸道         クエリ文字列のパラメタ名と、名前空間の接頭辞・名前の直書きを SAML2Const に寄せた
//*  2026/10/22  玄人 幸道         処理区分（共通・SP 側・IdP 側）ごとに partial クラスへ分割した（SAML2Bindings.*.cs）
//**********************************************************************************

using System;
using System.Xml;

using Touryo.Infrastructure.Public.Xml;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2Bindings（ライブラリ）</summary>
    /// <remarks>
    /// 処理区分ごとに partial クラスのファイルへ分けている（公開 API は 1 つの型のまま）。
    ///
    /// | ファイル | 区分 |
    /// |---|---|
    /// | SAML2Bindings.cs | 共通（NamespaceManager・ID の取得・スキーマ検証） |
    /// | SAML2Bindings.Encoding.cs | 共通（符号化・署名 / 復号・検証） |
    /// | SAML2Bindings.SP.cs | SP 側（要求を作る・応答を読む） |
    /// | SAML2Bindings.IdP.cs | IdP 側（応答・アサーション・メタデータを作る・要求を読む） |
    ///
    /// SAML の署名・検証は SP と IdP の双方が使うため、IdP 側の処理も、例外として同じクラスに置いている。
    /// </remarks>
    public partial class SAML2Bindings
    {
        #region NamespaceManager

        /// <summary>CreateNamespaceManager</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <returns>XmlNamespaceManager</returns>
        public static XmlNamespaceManager CreateNamespaceManager(XmlDocument xmlDoc)
        {
            XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            xmlnsManager.AddNamespace(SAML2Const.PrefixAssertion, SAML2Const.UrnAssertion);
            xmlnsManager.AddNamespace(SAML2Const.PrefixProtocol, SAML2Const.UrnProtocol);

            return xmlnsManager;
        }

        #endregion

        #region ID の取得

        /// <summary>GetIdInRequest</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Id</returns>
        public static string GetIdInRequest(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, @"/samlp:AuthnRequest", "ID", samlNsMgr);
        }

        /// <summary>GetIdInResponse</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Id</returns>
        public static string GetIdInResponse(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, @"/samlp:Response", "ID", samlNsMgr);
        }

        /// <summary>GetIdInAssertion</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Id</returns>
        public static string GetIdInAssertion(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, @"/saml:Assertion", "ID", samlNsMgr);
        }

        #endregion

        #region スキーマ検証

        /// <summary>VerifyByXPath</summary>
        /// <param name="saml">string</param>
        /// <param name="schema">SAML2Enum.SamlSchema</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>bool</returns>
        /// <remarks>
        /// Response の Assertion は、StatusCode が Success のときだけ必須（#598）。
        /// SAML 2.0 Core 3.2.2 / 4.1.4.2 のとおり、エラー応答は Status だけで正当である。
        ///
        /// そのため、Response で true が返っても Assertion があるとは限らない。
        /// Assertion を読む前に、StatusCode が Success であることを確かめること。
        /// </remarks>
        public static bool VerifyByXPath(
            XmlDocument saml, SAML2Enum.SamlSchema schema, XmlNamespaceManager samlNsMgr)
        {
            bool result = false;
            XmlNodeList xmlNodeList = null;

            // Nodeの全体構造を検証（属性はチェックしない）。
            // ※ なお、要素・属性取得時も、XPathで構造を指定している。
            switch (schema)
            {
                case SAML2Enum.SamlSchema.Request:

                    // Request
                    xmlNodeList = saml.SelectNodes(
                        SAML2Const.XPathRequest, samlNsMgr);
                    if (xmlNodeList != null && xmlNodeList.Count == 1)
                    {
                        // Issuer
                        xmlNodeList = saml.SelectNodes(
                            SAML2Const.XPathIssuerInRequest, samlNsMgr);
                        if (xmlNodeList != null && xmlNodeList.Count == 1)
                        {
                            // NameIDPolicy
                            xmlNodeList = saml.SelectNodes(
                                SAML2Const.XPathNameIDPolicyInRequest, samlNsMgr);
                            if (xmlNodeList != null && xmlNodeList.Count == 1)
                            {
                                result = true;
                            }
                        }
                    }

                    break;

                case SAML2Enum.SamlSchema.Response:

                    bool interimReport = false;
                    // Response
                    xmlNodeList = saml.SelectNodes(
                        SAML2Const.XPathResponse, samlNsMgr);
                    if (xmlNodeList != null && xmlNodeList.Count == 1)
                    {
                        // Issuer
                        xmlNodeList = saml.SelectNodes(
                            SAML2Const.XPathIssuerInResponse, samlNsMgr);
                        if (xmlNodeList != null && xmlNodeList.Count == 1)
                        {
                            // Status
                            xmlNodeList = saml.SelectNodes(
                                SAML2Const.XPathStatusCodeInResponse, samlNsMgr);
                            if (xmlNodeList != null && xmlNodeList.Count == 1)
                            {
                                // Assertion
                                xmlNodeList = saml.SelectNodes(
                                    SAML2Const.XPathAssertionInResponse, samlNsMgr);
                                if (xmlNodeList != null && xmlNodeList.Count == 1)
                                {
                                    interimReport = true;
                                }
                                else if (xmlNodeList != null && xmlNodeList.Count == 0)
                                {
                                    // エラー応答（StatusCode が Success 以外）は、
                                    // Assertion を持たないのが正当（#598）。
                                    if (SAML2Bindings.GetStatusCodeInResponse(saml, samlNsMgr)
                                        != SAML2Const.UrnStatusCodeSuccess)
                                    {
                                        result = true;
                                    }
                                }
                            }
                        }
                    }

                    // Assertion
                    if (interimReport)
                    {
                        // Issuer
                        xmlNodeList = saml.SelectNodes(
                            SAML2Const.XPathIssuerInAssertion, samlNsMgr);
                        if (xmlNodeList != null && xmlNodeList.Count == 1)
                        {
                            // NameID
                            xmlNodeList = saml.SelectNodes(
                            SAML2Const.XPathNameIDInAssertion, samlNsMgr);
                            if (xmlNodeList != null && xmlNodeList.Count == 1)
                            {
                                // SubjectConfirmationData
                                xmlNodeList = saml.SelectNodes(
                                    SAML2Const.XPathSubjectConfirmationDataInAssertion, samlNsMgr);
                                if (xmlNodeList != null && xmlNodeList.Count == 1)
                                {
                                    // Audience
                                    xmlNodeList = saml.SelectNodes(
                                        SAML2Const.XPathAudienceInAssertion, samlNsMgr);
                                    if (xmlNodeList != null && xmlNodeList.Count == 1)
                                    {
                                        // AuthnContextClassRef
                                        xmlNodeList = saml.SelectNodes(
                                        SAML2Const.XPathAuthnContextClassRefInAssertion, samlNsMgr);
                                        if (xmlNodeList != null && xmlNodeList.Count == 1)
                                        {
                                            result = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                //case SAML2Enum.SamlSchema.Assertion:
                //    //...
                //    break;
            }

            return result;
        }

        /// <summary>VerifyByXsd</summary>
        /// <param name="saml">string</param>
        /// <param name="schema">SAML2Enum.SamlSchema</param>
        /// <returns>bool</returns>
        /// <remarks>
        /// SAMLのXSDが作成できていないため書きかけ。
        /// - 現時点では、VerifyByXPath メソッドを使って XPath により検証しています。
        /// - XSD検証の方が明らかに良い場合は移行を検討します。
        /// </remarks>
        private static bool VerifyByXsd(string saml, SAML2Enum.SamlSchema schema)
        {
            string embeddedXsdFileName = "";
            string targetNamespace = "";

            // どうも、OASISのXSDではダメで、
            // - https://docs.oasis-open.org/security/saml/v2.0/saml-schema-protocol-2.0.xsd
            // - https://docs.oasis-open.org/security/saml/v2.0/saml-schema-assertion-2.0.xsd
            // 個別にXSDを作成しないとダメっぽい。

            switch (schema)
            {
                case SAML2Enum.SamlSchema.Request:
                    embeddedXsdFileName = "XXXX.xsd"; // RequestのSAML用のXSD
                    targetNamespace = "urn:oasis:names:tc:SAML:2.0:...";
                    break;

                case SAML2Enum.SamlSchema.Response:
                    embeddedXsdFileName = "XXXX.xsd"; // ResponseのSAML用のXSD
                    targetNamespace = "urn:oasis:names:tc:SAML:2.0:...";
                    break;

                //case SAML2Enum.SamlSchema.Assertion:
                    //    embeddedXsdFileName = "XXXX.xsd";
                    //    targetNamespace = "urn:oasis:names:tc:SAML:2.0:...";
                    //    break;
            }

            // 以下の関数は適切に動作するが、XSDに問題があるため動作しない。
            return XmlLib.ValidateByEmbeddedXsd(
                "OpenTouryo.Framework", saml, embeddedXsdFileName, targetNamespace);
        }

        #endregion
    }
}

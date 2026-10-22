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
//* クラス名        ：SAML2Bindings（SP）
//* クラス日本語名  ：SAML2Bindings（ライブラリ）のSP 側（要求を作る・応答を読む）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（SAML2Bindings.cs から処理区分ごとに分割した）
//*  2026/10/22  玄人 幸道         属性名・XPath・要素名・ID の接頭辞の直書きを SAML2Const に寄せた
//**********************************************************************************

using System;
using System.Xml;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Xml;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2Bindings（ライブラリ）のSP 側（要求を作る・応答を読む）</summary>
    public partial class SAML2Bindings
    {
        #region 要求を作る

        /// <summary>CreateRequest</summary>
        /// <param name="issuer">string</param>
        /// <param name="protocolBinding">SAML2Enum.ProtocolBinding</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="assertionConsumerServiceURL">string</param>
        /// <param name="id">string</param>
        /// <returns>SAMLRequest</returns>
        public static XmlDocument CreateRequest(string issuer,
            SAML2Enum.ProtocolBinding protocolBinding,
            SAML2Enum.NameIDFormat nameIDFormat,
            string assertionConsumerServiceURL, out string id)
        {
            // idの先頭は[A-Za-z]のみで、s2とするのが慣例っぽい。
            id = SAML2Const.IdPrefix + Guid.NewGuid().ToString("N");
            string xmlString = SAML2Const.RequestTemplate;

            #region enum 2 string
            string urnNameIDFormatString = SAML2Enum.EnumToString(nameIDFormat);
            string protocolBindingString = SAML2Enum.EnumToString(protocolBinding);
            #endregion

            #region Replace
            // 固定値
            xmlString = xmlString.Replace("{UrnProtocol}", SAML2Const.UrnProtocol);
            xmlString = xmlString.Replace("{UrnAssertion}", SAML2Const.UrnAssertion);

            // 可変値
            // - 共通
            xmlString = xmlString.Replace("{ID}", id);
            xmlString = xmlString.Replace("{Issuer}", issuer);
            xmlString = xmlString.Replace("{IssueInstant}", FormatConverter.ToW3cTimestamp(DateTime.UtcNow));

            // - ...
            xmlString = xmlString.Replace("{UrnNameIDFormat}", urnNameIDFormatString);            

            // XmlDocument化
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;
            xmlDoc.LoadXml(xmlString);
            #endregion

            #region Append
            // 以下はオプション属性
            XmlNode node = xmlDoc.GetElementsByTagName(SAML2Const.PrefixProtocol + ":" + SAML2Const.ElementAuthnRequest)[0];
            XmlAttribute attr = null;
            // - ProtocolBinding属性
            if (!string.IsNullOrEmpty(protocolBindingString))
            {
                attr = xmlDoc.CreateAttribute(SAML2Const.AttrProtocolBinding);
                attr.Value = protocolBindingString;
                node.Attributes.Append(attr);
            }
            // - AssertionConsumerServiceURL属性
            if (!string.IsNullOrEmpty(assertionConsumerServiceURL))
            {
                attr = xmlDoc.CreateAttribute(SAML2Const.AttrAssertionConsumerServiceURL);
                attr.Value = assertionConsumerServiceURL;
                node.Attributes.Append(attr);
            }
            #endregion

            return xmlDoc;
        }

        #endregion

        #region 応答を読む（Response）

        /// <summary>GetInResponseToInResponse</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>InResponseTo</returns>
        public static string GetInResponseToInResponse(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeFromXmlNode(
                xmlDoc.SelectSingleNode(SAML2Const.XPathResponse, samlNsMgr), SAML2Const.AttrInResponseTo);
        }

        /// <summary>GetIssuerInResponse</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Issuer</returns>
        public static string GetIssuerInResponse(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            XmlNode iss = xmlDoc.SelectSingleNode(
                SAML2Const.XPathIssuerInResponse, samlNsMgr);

            if (iss == null)
            {
                return ""; 
            }
            else
            {
                return iss.InnerText.Trim();
            }
        }

        /// <summary>GetStatusCodeInResponse</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>StatusCode</returns>
        public static string GetStatusCodeInResponse(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, SAML2Const.XPathStatusCodeInResponse, SAML2Const.AttrValue, samlNsMgr);
        }

        #endregion

        #region 応答を読む（Assertion）

        /// <summary>GetIssuerInAssertion</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Issuer</returns>
        public static string GetIssuerInAssertion(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            XmlNode iss = xmlDoc.SelectSingleNode(
                SAML2Const.XPathIssuerInAssertion, samlNsMgr);

            if (iss == null)
            {
                return "";
            }
            else
            {
                return iss.InnerText.Trim();
            }
        }

        /// <summary>GetNameIDInAssertion</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <param name="format">out string</param>
        /// <param name="innerText">out string</param>
        /// <returns>NameID</returns>
        public static XmlNode GetNameIDInAssertion(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr,
            out string format, out string innerText)
        {
            format = "";
            innerText = "";

            XmlNode nameID = xmlDoc.SelectSingleNode(
                SAML2Const.XPathNameIDInAssertion, samlNsMgr);

            if (nameID != null)
            {
                format = XmlLib.GetAttributeFromXmlNode(nameID, SAML2Const.AttrFormat);
                innerText = nameID.InnerText;
            }

            return nameID;
        }

        /// <summary>GetSubjectConfirmationMethodInAssertion</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>SubjectConfirmation - Method</returns>
        public static string GetSubjectConfirmationMethodInAssertion(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, SAML2Const.XPathSubjectConfirmationInAssertion, SAML2Const.AttrMethod, samlNsMgr);
        }

        /// <summary>GetSubjectConfirmationDataInAssertion</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <param name="inResponseTo">out string</param>
        /// <param name="notOnOrAfter">out string</param>
        /// <param name="recipient">out string</param>
        /// <returns>SubjectConfirmationData</returns>
        public static XmlNode GetSubjectConfirmationDataInAssertion(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr,
            out string inResponseTo, out string notOnOrAfter, out string recipient)
        {
            XmlNode subjectConfirmationData  = xmlDoc.SelectSingleNode(
                SAML2Const.XPathSubjectConfirmationDataInAssertion, samlNsMgr);

            inResponseTo = "";
            notOnOrAfter = "";
            recipient = "";

            if (subjectConfirmationData.Attributes != null)
            {
                if (subjectConfirmationData.Attributes[SAML2Const.AttrInResponseTo] != null)
                {
                    inResponseTo = subjectConfirmationData.Attributes[SAML2Const.AttrInResponseTo].Value;
                }

                if (subjectConfirmationData.Attributes[SAML2Const.AttrNotOnOrAfter] != null)
                {
                    notOnOrAfter = subjectConfirmationData.Attributes[SAML2Const.AttrNotOnOrAfter].Value;
                }

                if (subjectConfirmationData.Attributes[SAML2Const.AttrRecipient] != null)
                {
                    recipient = subjectConfirmationData.Attributes[SAML2Const.AttrRecipient].Value;
                }
            }

            return subjectConfirmationData;
        }

        /// <summary>GetConditionsNotOnOrAfterInAssertion</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Conditions - NotOnOrAfter</returns>
        public static string GetConditionsNotOnOrAfterInAssertion(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, SAML2Const.XPathConditionsInAssertion, SAML2Const.AttrNotOnOrAfter, samlNsMgr);
        }

        /// <summary>GetAudienceInAssertion</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Audience</returns>
        public static string GetAudienceInAssertion(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            XmlNode aud = xmlDoc.SelectSingleNode(
                SAML2Const.XPathAudienceInAssertion, samlNsMgr);

            if (aud == null)
            {
                return "";
            }
            else
            {
                return aud.InnerText.Trim();
            }
        }

        /// <summary>GetAuthnContextClassRefInAssertion</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>AuthnContextClassRef</returns>
        public static string GetAuthnContextClassRefInAssertion(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            XmlNode accr = xmlDoc.SelectSingleNode(
                SAML2Const.XPathAuthnContextClassRefInAssertion, samlNsMgr);

            if (accr == null)
            {
                return "";
            }
            else
            {
                return accr.InnerText.Trim();
            }
        }

        #endregion
    }
}

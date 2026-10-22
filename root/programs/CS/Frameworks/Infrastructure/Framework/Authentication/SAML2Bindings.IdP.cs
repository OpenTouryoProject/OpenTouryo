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
//* クラス名        ：SAML2Bindings（IdP）
//* クラス日本語名  ：SAML2Bindings（ライブラリ）のIdP 側（応答・アサーション・メタデータを作る・要求を読む）
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
using System.Collections.Generic;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Xml;
using Touryo.Infrastructure.Public.Security.Xml;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2Bindings（ライブラリ）のIdP 側（応答・アサーション・メタデータを作る・要求を読む）</summary>
    /// <remarks>
    /// SAML の署名・検証（SAML2Bindings.Encoding.cs）は SP と IdP の双方が使うため、
    /// IdP 側の処理も、例外として同じクラスに置いている。
    /// </remarks>
    public partial class SAML2Bindings
    {
        #region 応答・アサーション・メタデータを作る

        /// <summary>CreateResponse</summary>
        /// <param name="issuer">string</param>
        /// <param name="destination">string</param>
        /// <param name="inResponseTo">string</param>
        /// <param name="statusCode">SAML2Enum.StatusCode</param>
        /// <param name="id">string</param>
        /// <returns>SAMLResponse</returns>
        public static XmlDocument CreateResponse(
            string issuer, string destination, string inResponseTo,
            SAML2Enum.StatusCode statusCode, out string id)
        {
            // idの先頭は[A-Za-z]のみで、s2とするのが慣例っぽい。
            id = SAML2Const.IdPrefix + Guid.NewGuid().ToString("N");
            string xmlString = SAML2Const.ResponseTemplate;

            #region enum 2 string
            string urnStatusCodeString = SAML2Enum.EnumToString(statusCode);
            #endregion

            #region Replace
            // 固定値
            xmlString = xmlString.Replace("{UrnProtocol}", SAML2Const.UrnProtocol);
            xmlString = xmlString.Replace("{UrnAssertion}", SAML2Const.UrnAssertion);

            // 可変値
            // - 共通
            xmlString = xmlString.Replace("{ID}", id);
            xmlString = xmlString.Replace("{IssueInstant}", FormatConverter.ToW3cTimestamp(DateTime.UtcNow));
            xmlString = xmlString.Replace("{Issuer}", issuer);

            // - Response固有
            xmlString = xmlString.Replace("{Destination}", destination);
            xmlString = xmlString.Replace("{InResponseTo}", inResponseTo);
            xmlString = xmlString.Replace("{UrnStatusCode}", urnStatusCodeString);

            // XmlDocument化
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;
            xmlDoc.LoadXml(xmlString);
            #endregion

            return xmlDoc;
        }

        /// <summary>CreateAssertion</summary>
        /// <param name="inResponseTo">string</param>
        /// <param name="issuer">string</param>
        /// <param name="nameID">string</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="authnContextClassRef">SAML2Enum.AuthnContextClassRef</param>
        /// <param name="expiresFromSecond">double</param>
        /// <param name="recipient">string</param>
        /// <param name="id">string</param>
        /// <param name="rsa">RSA</param>
        /// <returns>SAMLAssertion</returns>
        public static XmlDocument CreateAssertion(
            string inResponseTo, string issuer, string nameID,
            SAML2Enum.NameIDFormat nameIDFormat,
            SAML2Enum.AuthnContextClassRef authnContextClassRef,
            double expiresFromSecond, string recipient,
            out string id, RSA rsa = null)
        {
            // idの先頭は[A-Za-z]のみで、s2とするのが慣例っぽい。
            id = SAML2Const.IdPrefix + Guid.NewGuid().ToString("N");
            string xmlString = SAML2Const.AssertionTemplate;

            #region enum 2 string
            string urnNameIDFormatString = SAML2Enum.EnumToString(nameIDFormat);
            string urnAuthnContextClassRefString = SAML2Enum.EnumToString(authnContextClassRef);
            #endregion

            #region Replace
            // 固定値
            xmlString = xmlString.Replace("{UrnProtocol}", SAML2Const.UrnProtocol);
            xmlString = xmlString.Replace("{UrnAssertion}", SAML2Const.UrnAssertion);
            xmlString = xmlString.Replace("{UrnMethod}", SAML2Const.UrnMethodBearer);

            // 可変値
            // - ID
            xmlString = xmlString.Replace("{ID}", id);
            xmlString = xmlString.Replace("{InResponseTo}", inResponseTo);
            xmlString = xmlString.Replace("{Issuer}", issuer);

            // - 認証関連
            xmlString = xmlString.Replace("{NameID}", nameID);
            xmlString = xmlString.Replace("{UrnNameIDFormat}", urnNameIDFormatString);
            xmlString = xmlString.Replace("{UrnAuthnContextClassRef}", urnAuthnContextClassRefString);

            // - 時間関連
            string utcNow = FormatConverter.ToW3cTimestamp(DateTime.UtcNow);
            xmlString = xmlString.Replace("{IssueInstant}", utcNow);
            xmlString = xmlString.Replace("{AuthnInstant}", utcNow);
            xmlString = xmlString.Replace("{NotBefore}", utcNow);

            string utcExpires = FormatConverter.ToW3cTimestamp(DateTime.UtcNow.AddSeconds(expiresFromSecond));
            xmlString = xmlString.Replace("{NotOnOrAfter}", utcExpires);

            // - SP関連
            xmlString = xmlString.Replace("{Recipient}", recipient);
            xmlString = xmlString.Replace("{Audience}", recipient); // recipientのFQDNまでらしい

            // XmlDocument化
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;
            xmlDoc.LoadXml(xmlString);
            #endregion

            #region Sign
            if (rsa != null)
            {
                SignedXml2 signedXml2 = new SignedXml2(rsa);
                xmlDoc = signedXml2.Create(xmlDoc, id);
            }
            #endregion

            return xmlDoc;
        }

        /// <summary>CreateMetadata</summary>
        /// <param name="entityID">string</param>
        /// <param name="pemString">string</param>
        /// <param name="nameIDFormats">SAML2Enum.NameIDFormat[]</param>
        /// <param name="saml2RequestEndpoint_Redirect">string</param>
        /// <param name="saml2RequestEndpoint_Post">string</param>
        /// <returns>SAMLMetadata</returns>
        public static XmlDocument CreateMetadata(
            string entityID, string pemString,
            SAML2Enum.NameIDFormat[] nameIDFormats,
            string saml2RequestEndpoint_Redirect,
            string saml2RequestEndpoint_Post)
        {
            string xmlString = SAML2Const.MetadataTemplate;

            #region enum 2 string
            List<string> urnNameIDFormatsString = new List<string>();
            foreach (SAML2Enum.NameIDFormat nameIDFormat in nameIDFormats)
            {
                urnNameIDFormatsString.Add(SAML2Enum.EnumToString(nameIDFormat));
            }
            #endregion

            #region Replace
            // 固定値
            xmlString = xmlString.Replace("{UrnMetadata}", SAML2Const.UrnMetadata);
            xmlString = xmlString.Replace("{UrnProtocolSupportEnumeration}", SAML2Const.UrnProtocol);
            xmlString = xmlString.Replace("{SingleSignOnServiceRedirect}", SAML2Const.UrnBindingsRedirect);
            xmlString = xmlString.Replace("{SingleSignOnServicePost}", SAML2Const.UrnBindingsPost);

            // 可変値
            xmlString = xmlString.Replace("{EntityID}", entityID);
            xmlString = xmlString.Replace("{WantAuthnRequestsSigned}", "true");
            xmlString = xmlString.Replace("{X509CertificatePemString}", pemString);
            //xmlString = xmlString.Replace("{UrnNameIDFormat}", urnNameIDFormatString);
            xmlString = xmlString.Replace("{SingleSignOnServiceRedirectLocation}", saml2RequestEndpoint_Redirect);
            xmlString = xmlString.Replace("{SingleSignOnServicePostLocation}", saml2RequestEndpoint_Post);

            // XmlDocument化
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;
            xmlDoc.LoadXml(xmlString);

            #endregion

            #region Append
            // 以下は可変要素
            XmlNode newNode = null;
            XmlNode rootNode = xmlDoc.GetElementsByTagName(SAML2Const.PrefixMetadata + ":" + SAML2Const.ElementIDPSSODescriptor)[0];
            // NameIDFormat
            foreach (string text in urnNameIDFormatsString)
            {
                newNode = xmlDoc.CreateNode(
                    XmlNodeType.Element,
                    SAML2Const.PrefixMetadata, SAML2Const.ElementNameIDFormat,
                    SAML2Const.UrnMetadata);
                // 空のxmlns属性 出力問題の抑止

                newNode.InnerText = text;
                rootNode.AppendChild(newNode);
            }
            #endregion

            return xmlDoc;
        }

        #endregion

        #region 要求を読む

        /// <summary>GetIssuerInRequest</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Issuer</returns>
        public static string GetIssuerInRequest(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            XmlNode iss = xmlDoc.SelectSingleNode(
                SAML2Const.XPathIssuerInRequest, samlNsMgr);

            if (iss == null)
            {
                return "";
            }
            else
            {
                return iss.InnerText.Trim();
            }
        }

        /// <summary>GetNameIDPolicyFormatInRequest</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>NameIDPolicy - Format</returns>
        public static string GetNameIDPolicyFormatInRequest(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, SAML2Const.XPathNameIDPolicyInRequest, SAML2Const.AttrFormat, samlNsMgr);
        }

        /// <summary>GetProtocolBindingInRequest</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>ProtocolBinding</returns>
        public static string GetProtocolBindingInRequest(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, SAML2Const.XPathRequest, SAML2Const.AttrProtocolBinding, samlNsMgr);
        }

        /// <summary>GetAssertionConsumerServiceURLInRequest</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>AssertionConsumerServiceURL</returns>
        public static string GetAssertionConsumerServiceURLInRequest(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, SAML2Const.XPathRequest, SAML2Const.AttrAssertionConsumerServiceURL, samlNsMgr);
        }

        #endregion
    }
}

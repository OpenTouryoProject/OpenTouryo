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
//**********************************************************************************

using System;
using System.Xml;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Security.Xml;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2Bindings（ライブラリ）</summary>
    public class SAML2Bindings
    {
        /// <summary>CreateRequest</summary>
        /// <param name="issuer">string</param>
        /// <param name="protocolBinding">SAML2Enum.ProtocolBinding</param>
        /// <param name="urnNameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="assertionConsumerServiceURL">string</param>
        /// <param name="rsa">RSA</param>
        /// <returns>SAMLRequest</returns>
        public static XmlDocument CreateRequest(string issuer,
            SAML2Enum.ProtocolBinding protocolBinding,
            SAML2Enum.NameIDFormat urnNameIDFormat,
            string assertionConsumerServiceURL, RSA rsa = null)
        {
            // idの先頭は[A-Za-z]のみで、s2とするのが慣例っぽい。
            string id = "s2" + Guid.NewGuid().ToString("N");
            string xmlString = SAML2Const.RequestTemplate;

            #region enum 2 string
            string protocolBindingString = "";
            switch (protocolBinding)
            {
                case SAML2Enum.ProtocolBinding.HttpPost:
                    protocolBindingString = SAML2Const.UrnBindingsPost;
                    break;
                case SAML2Enum.ProtocolBinding.HttpRedirect:
                    protocolBindingString = SAML2Const.UrnBindingsRedirect;
                    break;
            }

            string urnNameIDFormatString = "";
            switch (urnNameIDFormat)
            {
                case SAML2Enum.NameIDFormat.unspecified:
                    urnNameIDFormatString = SAML2Const.UrnNameIDFormatUnspecified;
                    break;
                case SAML2Enum.NameIDFormat.persistent:
                    urnNameIDFormatString = SAML2Const.UrnNameIDFormatPersistent;
                    break;
                case SAML2Enum.NameIDFormat.transient:
                    urnNameIDFormatString = SAML2Const.UrnNameIDFormatTransient;
                    break;
            }
            #endregion

            #region Replace
            // 共通
            xmlString = xmlString.Replace("{ID}", id);
            xmlString = xmlString.Replace("{Issuer}", issuer);
            xmlString = xmlString.Replace("{IssueInstant}", DateTime.UtcNow.ToString("s") + "Z");

            // ...
            xmlString = xmlString.Replace("{UrnNameIDFormat}", urnNameIDFormatString);

            // 固定値
            xmlString = xmlString.Replace("{UrnProtocol}", SAML2Const.UrnProtocol);
            xmlString = xmlString.Replace("{UrnAssertion}", SAML2Const.UrnAssertion);

            // XmlDocument化
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            xmlDoc.PreserveWhitespace = false;
            #endregion

            #region Append
            // 以下はオプション属性
            XmlNode node = xmlDoc.GetElementsByTagName("samlp:AuthnRequest")[0];
            XmlAttribute attr = null;
            // - ProtocolBinding属性
            if (!string.IsNullOrEmpty(protocolBindingString))
            {
                attr = xmlDoc.CreateAttribute("ProtocolBinding");
                attr.Value = protocolBindingString;
                node.Attributes.Append(attr);
            }
            // - AssertionConsumerServiceURL属性
            if (!string.IsNullOrEmpty(assertionConsumerServiceURL))
            {
                attr = xmlDoc.CreateAttribute("AssertionConsumerServiceURL");
                attr.Value = assertionConsumerServiceURL;
                node.Attributes.Append(attr);
            }
            #endregion

            #region Sign
            if (!(rsa == null))
            {
                SignedXml2 signedXml2 = new SignedXml2(rsa);
                xmlDoc = signedXml2.Create(xmlDoc, id);
            }
            #endregion

            return xmlDoc;
        }

        /// <summary>CreateAssertion</summary>
        /// <param name="inResponseTo">string</param>
        /// <param name="issuer">string</param>
        /// <param name="nameID">string</param>
        /// <param name="urnNameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="urnAuthnContextClassRef">SAML2Enum.AuthnContextClassRef</param>
        /// <param name="expiresFromSecond">double</param>
        /// <param name="recipient">string</param>
        /// <param name="rsa">RSA</param>
        /// <returns>SAMLAssertion</returns>
        public static XmlDocument CreateAssertion(
            string inResponseTo, string issuer, string nameID,
            SAML2Enum.NameIDFormat urnNameIDFormat,
            SAML2Enum.AuthnContextClassRef urnAuthnContextClassRef,
            double expiresFromSecond, string recipient, RSA rsa = null)
        {
            // idの先頭は[A-Za-z]のみで、s2とするのが慣例っぽい。
            string id = "s2" + Guid.NewGuid().ToString("N");
            string xmlString = SAML2Const.AssertionTemplate;

            #region enum 2 string
            string urnNameIDFormatString = "";
            switch (urnNameIDFormat)
            {
                case SAML2Enum.NameIDFormat.unspecified:
                    urnNameIDFormatString = SAML2Const.UrnNameIDFormatUnspecified;
                    break;
                case SAML2Enum.NameIDFormat.persistent:
                    urnNameIDFormatString = SAML2Const.UrnNameIDFormatPersistent;
                    break;
                case SAML2Enum.NameIDFormat.transient:
                    urnNameIDFormatString = SAML2Const.UrnNameIDFormatTransient;
                    break;
            }

            string urnAuthnContextClassRefString = "";
            switch (urnAuthnContextClassRef)
            {
                case SAML2Enum.AuthnContextClassRef.unspecified:
                    urnAuthnContextClassRefString = SAML2Const.UrnAuthnContextClassRefUnspecified;
                    break;
                case SAML2Enum.AuthnContextClassRef.Password:
                    urnAuthnContextClassRefString = SAML2Const.UrnAuthnContextClassRefPassword;
                    break;
                case SAML2Enum.AuthnContextClassRef.PasswordProtectedTransport:
                    urnAuthnContextClassRefString = SAML2Const.UrnAuthnContextClassRefPasswordProtectedTransport;
                    break;
                case SAML2Enum.AuthnContextClassRef.X509:
                    urnAuthnContextClassRefString = SAML2Const.UrnAuthnContextClassRefX509;
                    break;
            }
            #endregion
            
            #region Replace
            // ID
            xmlString = xmlString.Replace("{ID}", id);
            xmlString = xmlString.Replace("{InResponseTo}", inResponseTo);
            xmlString = xmlString.Replace("{Issuer}", issuer);

            // 認証関連
            xmlString = xmlString.Replace("{NameID}", nameID);
            xmlString = xmlString.Replace("{UrnNameIDFormat}", urnNameIDFormatString);
            xmlString = xmlString.Replace("{UrnAuthnContextClassRef}", urnAuthnContextClassRefString);

            // 時間関連
            string utcNow = DateTime.UtcNow.ToString("s") + "Z";
            xmlString = xmlString.Replace("{IssueInstant}" , utcNow);
            xmlString = xmlString.Replace("{AuthnInstant}" , utcNow);
            xmlString = xmlString.Replace("{NotBefore}"    , utcNow);

            string utcExpires = DateTime.UtcNow.AddSeconds(expiresFromSecond).ToString("s") + "Z";
            xmlString = xmlString.Replace("{NotOnOrAfter}", utcExpires);

            // SP関連
            xmlString = xmlString.Replace("{Recipient}", recipient);
            xmlString = xmlString.Replace("{Audience}", recipient); // recipientのFQDNまでらしい

            // 固定値
            xmlString = xmlString.Replace("{UrnAssertion}", SAML2Const.UrnAssertion);
            xmlString = xmlString.Replace("{UrnMethod}", SAML2Const.UrnMethodBearer);

            // XmlDocument化
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            xmlDoc.PreserveWhitespace = false;
            #endregion

            #region Sign
            if (!(rsa == null))
            {
                SignedXml2 signedXml2 = new SignedXml2(rsa);
                xmlDoc = signedXml2.Create(xmlDoc, id);
            }
            #endregion

            return xmlDoc;
        }

        /// <summary>CreateResponse</summary>
        /// <param name="issuer">string</param>
        /// <param name="destination">string</param>
        /// <param name="assertion">string</param>
        /// <param name="urnStatusCode">SAML2Enum.StatusCode</param>
        /// <param name="rsa">RSA</param>
        /// <returns>SAMLResponse</returns>
        public static XmlDocument CreateResponse(
            string issuer, string destination,
            string assertion, SAML2Enum.StatusCode urnStatusCode, RSA rsa = null)
        {
            // idの先頭は[A-Za-z]のみで、s2とするのが慣例っぽい。
            string id = "s2" + Guid.NewGuid().ToString("N");
            string xmlString = SAML2Const.ResponseTemplate;

            #region enum 2 string
            string urnStatusCodeString = "";
            switch (urnStatusCode)
            {
                case SAML2Enum.StatusCode.Success:
                    urnStatusCodeString = SAML2Const.UrnStatusCodeSuccess;
                    break;
                case SAML2Enum.StatusCode.Requester:
                    urnStatusCodeString = SAML2Const.UrnStatusCodeRequester;
                    break;
                case SAML2Enum.StatusCode.Responder:
                    urnStatusCodeString = SAML2Const.UrnStatusCodeResponder;
                    break;
                case SAML2Enum.StatusCode.AuthnFailed:
                    urnStatusCodeString = SAML2Const.UrnStatusCodeAuthnFailed;
                    break;
                case SAML2Enum.StatusCode.UnknownPrincipal:
                    urnStatusCodeString = SAML2Const.UrnStatusCodeUnknownPrincipal;
                    break;
                case SAML2Enum.StatusCode.VersionMismatch:
                    urnStatusCodeString = SAML2Const.UrnStatusCodeVersionMismatch;
                    break;
            }
            #endregion

            #region Replace
            // 共通
            xmlString = xmlString.Replace("{ID}", id);
            xmlString = xmlString.Replace("{IssueInstant}", DateTime.UtcNow.ToString("s") + "Z");
            xmlString = xmlString.Replace("{Issuer}", issuer);

            // Response固有
            xmlString = xmlString.Replace("{Destination}", destination);
            xmlString = xmlString.Replace("{UrnStatusCode}", urnStatusCodeString);
            xmlString = xmlString.Replace("{Assertion}", assertion);

            // 固定値
            xmlString = xmlString.Replace("{UrnProtocol}", SAML2Const.UrnProtocol);
            xmlString = xmlString.Replace("{UrnAssertion}", SAML2Const.UrnAssertion);

            // XmlDocument化
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            xmlDoc.PreserveWhitespace = false;
            #endregion

            #region Sign
            if (!(rsa == null))
            {
                SignedXml2 signedXml2 = new SignedXml2(rsa);
                xmlDoc = signedXml2.Create(xmlDoc, id);
            }
            #endregion

            return xmlDoc;
        }
    }
}
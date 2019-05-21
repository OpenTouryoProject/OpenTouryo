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

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2Bindings（ライブラリ）</summary>
    public class SAML2Bindings
    {
        /// <summary>CreateRequest</summary>
        /// <param name="issuer">string</param>
        /// <param name="urnNameIDFormat">string</param>
        /// <param name="protocolBinding">string</param>
        /// <param name="assertionConsumerServiceURL">string</param>
        /// <returns>SAMLRequest</returns>
        public XmlDocument CreateSAMLRequest(
            string issuer, string urnNameIDFormat,
            string protocolBinding, string assertionConsumerServiceURL)
        {
            string xmlString = SAML2Const.RequestTemplate;

            #region Replace
            // 共通
            xmlString = xmlString.Replace("{ID}", Guid.NewGuid().ToString());
            xmlString = xmlString.Replace("{Issuer}", issuer);
            xmlString = xmlString.Replace("{IssueInstant}", DateTime.UtcNow.ToString("s") + "Z");

            // ...
            xmlString = xmlString.Replace("{UrnNameIDFormat}", urnNameIDFormat);

            // 固定値
            xmlString = xmlString.Replace("{UrnProtocol}", SAML2Const.UrnProtocol);
            xmlString = xmlString.Replace("{UrnAssertion}", SAML2Const.UrnAssertion);
            #endregion

            #region Append
            // 以下はオプション属性
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            xmlDoc.PreserveWhitespace = false;
            XmlNode node = xmlDoc.GetElementsByTagName("samlp:AuthnRequest")[0];
            XmlAttribute attr = null;
            // - ProtocolBinding属性
            if (string.IsNullOrEmpty(protocolBinding))
            {
                attr = xmlDoc.CreateAttribute("ProtocolBinding");
                attr.Value = protocolBinding;
                node.Attributes.Append(attr);
            }
            // - AssertionConsumerServiceURL属性
            if (string.IsNullOrEmpty(protocolBinding))
            {
                attr = xmlDoc.CreateAttribute("AssertionConsumerServiceURL");
                attr.Value = assertionConsumerServiceURL;
                node.Attributes.Append(attr);
            }
            #endregion

            return xmlDoc;
        }

        /// <summary>CreateSAMLAssertion</summary>
        /// <param name="inResponseTo">string</param>
        /// <param name="issuer">string</param>
        /// <param name="nameID">string</param>
        /// <param name="urnNameIDFormat">string</param>
        /// <param name="urnAuthnContextClassRef">string</param>
        /// <param name="expiresFromSecond">double</param>
        /// <param name="recipient">string</param>
        /// <returns>SAMLAssertion</returns>
        public XmlDocument CreateSAMLAssertion(
            string inResponseTo, string issuer,
            string nameID, string urnNameIDFormat,
            string urnAuthnContextClassRef,
            double expiresFromSecond, string recipient)
        {
            string xmlString = SAML2Const.AssertionTemplate;

            #region Replace
            // ID
            xmlString = xmlString.Replace("{ID}", Guid.NewGuid().ToString());
            xmlString = xmlString.Replace("{InResponseTo}", inResponseTo);
            xmlString = xmlString.Replace("{Issuer}", issuer);

            // 認証関連
            xmlString = xmlString.Replace("{NameID}", nameID);
            xmlString = xmlString.Replace("{UrnNameIDFormat}", urnNameIDFormat);
            xmlString = xmlString.Replace("{UrnAuthnContextClassRef}", urnAuthnContextClassRef);

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
            #endregion

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            xmlDoc.PreserveWhitespace = false;
            
            return xmlDoc;
        }

        /// <summary>CreateSAMLResponse</summary>
        /// <param name="issuer">string</param>
        /// <param name="destination">string</param>
        /// <param name="urnStatusCode">string</param>
        /// <param name="assertion">string</param>
        /// <returns>SAMLResponse</returns>
        public XmlDocument CreateSAMLResponse(
            string issuer, string destination, string urnStatusCode, string assertion)
        {
            string xmlString = SAML2Const.ResponseTemplate;

            #region Replace
            // 共通
            xmlString = xmlString.Replace("{ID}", Guid.NewGuid().ToString());
            xmlString = xmlString.Replace("{IssueInstant}", DateTime.UtcNow.ToString("s") + "Z");
            xmlString = xmlString.Replace("{Issuer}", issuer);

            // Response固有
            xmlString = xmlString.Replace("{Destination}", destination);
            xmlString = xmlString.Replace("{UrnStatusCode}", urnStatusCode);
            xmlString = xmlString.Replace("{Assertion}", assertion);

            // 固定値
            xmlString = xmlString.Replace("{UrnProtocol}", SAML2Const.UrnProtocol);
            xmlString = xmlString.Replace("{UrnAssertion}", SAML2Const.UrnAssertion);
            #endregion

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            xmlDoc.PreserveWhitespace = false;
            
            return xmlDoc;
        }
    }
}
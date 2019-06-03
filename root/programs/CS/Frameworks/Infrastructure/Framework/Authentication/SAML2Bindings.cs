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
//*  2019/05/29  西野 大介         CreateRequest, Responseでは署名しない。
//*                                CreateAssertion, EncodeRedirect, Postで署名する。
//**********************************************************************************

using System;
using System.Xml;
using System.Text;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Xml;
using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Xml;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2Bindings（ライブラリ）</summary>
    public class SAML2Bindings
    {
        #region Create XML
        /// <summary>CreateRequest</summary>
        /// <param name="issuer">string</param>
        /// <param name="protocolBinding">SAML2Enum.ProtocolBinding</param>
        /// <param name="urnNameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="assertionConsumerServiceURL">string</param>
        /// <param name="id">string</param>
        /// <returns>SAMLRequest</returns>
        public static XmlDocument CreateRequest(string issuer,
            SAML2Enum.ProtocolBinding protocolBinding,
            SAML2Enum.NameIDFormat urnNameIDFormat,
            string assertionConsumerServiceURL, out string id)
        {
            // idの先頭は[A-Za-z]のみで、s2とするのが慣例っぽい。
            id = "s2" + Guid.NewGuid().ToString("N");
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
            xmlDoc.PreserveWhitespace = false;
            xmlDoc.LoadXml(xmlString);
            
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

            return xmlDoc;
        }

        /// <summary>CreateResponse</summary>
        /// <param name="issuer">string</param>
        /// <param name="destination">string</param>
        /// <param name="urnStatusCode">SAML2Enum.StatusCode</param>
        /// <param name="id">string</param>
        /// <returns>SAMLResponse</returns>
        public static XmlDocument CreateResponse(
            string issuer, string destination,
            SAML2Enum.StatusCode urnStatusCode, out string id)
        {
            // idの先頭は[A-Za-z]のみで、s2とするのが慣例っぽい。
            id = "s2" + Guid.NewGuid().ToString("N");
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
            //xmlString = xmlString.Replace("{Assertion}", assertion);// inResponseToのタメ

            // 固定値
            xmlString = xmlString.Replace("{UrnProtocol}", SAML2Const.UrnProtocol);
            xmlString = xmlString.Replace("{UrnAssertion}", SAML2Const.UrnAssertion);

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
        /// <param name="urnNameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="urnAuthnContextClassRef">SAML2Enum.AuthnContextClassRef</param>
        /// <param name="expiresFromSecond">double</param>
        /// <param name="recipient">string</param>
        /// <param name="id">string</param>
        /// <param name="rsa">RSA</param>
        /// <returns>SAMLAssertion</returns>
        public static XmlDocument CreateAssertion(
            string inResponseTo, string issuer, string nameID,
            SAML2Enum.NameIDFormat urnNameIDFormat,
            SAML2Enum.AuthnContextClassRef urnAuthnContextClassRef,
            double expiresFromSecond, string recipient,
            out string id, RSA rsa = null)
        {
            // idの先頭は[A-Za-z]のみで、s2とするのが慣例っぽい。
            id = "s2" + Guid.NewGuid().ToString("N");
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
                case SAML2Enum.AuthnContextClassRef.PreviousSession:
                    urnAuthnContextClassRefString = SAML2Const.UrnAuthnContextClassRefPreviousSession;
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
            xmlString = xmlString.Replace("{IssueInstant}", utcNow);
            xmlString = xmlString.Replace("{AuthnInstant}", utcNow);
            xmlString = xmlString.Replace("{NotBefore}", utcNow);

            string utcExpires = DateTime.UtcNow.AddSeconds(expiresFromSecond).ToString("s") + "Z";
            xmlString = xmlString.Replace("{NotOnOrAfter}", utcExpires);

            // SP関連
            xmlString = xmlString.Replace("{Recipient}", recipient);
            xmlString = xmlString.Replace("{Audience}", recipient); // recipientのFQDNまでらしい

            // 固定値
            xmlString = xmlString.Replace("{UrnProtocol}", SAML2Const.UrnProtocol);
            xmlString = xmlString.Replace("{UrnAssertion}", SAML2Const.UrnAssertion);
            xmlString = xmlString.Replace("{UrnMethod}", SAML2Const.UrnMethodBearer);

            // XmlDocument化
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;
            xmlDoc.LoadXml(xmlString);
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
        #endregion

        #region Encode And Sign / Decode, Verify

        #region Encode And Sign
        /// <summary>EncodeAndSignRedirect</summary>
        /// <param name="type">SAML2Enum.RequestOrResponse</param>
        /// <param name="saml">string</param>
        /// <param name="relayState">string</param>
        /// <param name="dsRSAwithSHA1">DigitalSign</param>
        /// <returns>RedirectBinding用クエリ文字列</returns>
        public static string EncodeAndSignRedirect(
            SAML2Enum.RequestOrResponse type,
            string saml, string relayState,
            DigitalSign dsRSAwithSHA1 = null)
        {
            // --------------------------------------------------
            // - XML → XML宣言のエンコーディング → DEFLATE圧縮
            // -   → Base64エンコード → URLエンコード →  クエリ文字列テンプレへ組込
            // -      → コレをASCIIエンコード → 署名 → Base64エンコード
            // -         → URLエンコード →  Signatureパラメタ追加。
            // --------------------------------------------------
            // ・ヘッダも必要
            //   "<?xml version="1.0" encoding="UTF-8"?>"
            // ・クエリ文字列テンプレート
            //   ・SAMLRequest=value&RelayState=value&SigAlg=value
            //   ・SAMLResponse=value&RelayState=value&SigAlg=value
            // ・クエリ文字列署名
            //   ・クエリ文字列パラメタ値が空文字列の場合は、パラメタ自体を署名の演算から除外する。
            //   ・署名対象は上記テンプレートの文字列で、署名は、SignatureパラメタとしてURLに追加。

            string queryString = "";
            string queryStringTemplate = "";

            #region クエリ文字列テンプレート生成
            if (string.IsNullOrEmpty(saml))
            {
                return "";
            }
            else
            {

                // 第1 QSパラメタ
                switch (type)
                {
                    case SAML2Enum.RequestOrResponse.Request:
                        queryStringTemplate += "SAMLRequest={SAML}";
                        break;
                    case SAML2Enum.RequestOrResponse.Response:
                        queryStringTemplate += "SAMLResponse={SAML}";
                        break;
                }

                // 第2 QSパラメタ
                if (string.IsNullOrEmpty(relayState))
                {
                    // RelayStateパラメタなし
                }
                else
                {
                    queryStringTemplate += "&RelayState={RelayState}";
                }

                // 第3 QSパラメタ
                if (dsRSAwithSHA1 == null)
                {
                    // SigAlg, Signatureパラメタなし
                }
                else
                {
                    // 第3 QSパラメタ
                    queryStringTemplate += "&SigAlg=" + CustomEncode.UrlEncode(SAML2Const.RSAwithSHA1);
                }                
            }
            #endregion

            #region エンコーディング

            // エンコーディング オブジェクトの取得
            Encoding enc = XmlLib.GetEncodingFromXmlDeclaration(saml);

            // XML → XML宣言のエンコーディング → DEFLATE圧縮 → Base64エンコード → URLエンコード
            saml = CustomEncode.UrlEncode(CustomEncode.ToBase64String(
                DeflateCompression.Compress(CustomEncode.StringToByte(saml, enc.CodePage))));
            #endregion

            #region 組込 & 署名

            // 署名対象となるクエリ文字列の生成（クエリ文字列のテンプレートへ組込
            queryString = queryStringTemplate;

            // - SAMLReXXXXXパラメタ
            queryString = queryString.Replace("{SAML}", saml);

            // - RelayStateパラメタ
            if (!string.IsNullOrEmpty(relayState))
            {
                queryString = queryString.Replace("{RelayState}", CustomEncode.UrlEncode(relayState));
            }

            // - Signatureパラメタ
            if (dsRSAwithSHA1 != null)
            {
                // ASCIIエンコード → 署名 → Base64エンコード → URLエンコード →  Signatureパラメタ追加。
                string signature = CustomEncode.UrlEncode(CustomEncode.ToBase64String(
                    dsRSAwithSHA1.Sign(CustomEncode.StringToByte(queryString, CustomEncode.us_ascii))));

                queryString = queryString + "&Signature=" + signature;
            }
            #endregion

            return queryString; 
        }

        /// <summary>EncodeAndSignPost</summary>
        /// <param name="saml">string</param>
        /// <param name="referenceId">string</param>
        /// <param name="rsa">RSA</param>
        /// <returns>RedirectPost用SAML文字列</returns>
        public static string EncodeAndSignPost(string saml, string referenceId = "", RSA rsa = null)
        {
            // エンコーディング オブジェクトの取得
            Encoding enc = XmlLib.GetEncodingFromXmlDeclaration(saml);

            if (rsa == null)
            {
                // 署名しない
            }
            else
            {
                // 署名する
                SignedXml2 signedXml2 = new SignedXml2(rsa);
                saml = signedXml2.Create(saml, referenceId).OuterXml;
            }

            // XML → XML宣言のエンコーディング → Base64エンコード
            return CustomEncode.ToBase64String(CustomEncode.StringToByte(saml, enc.CodePage));
        }
        #endregion

        #region Decode, Verify

        #region Redirect
        /// <summary>DecodeRedirect</summary>
        /// <param name="queryString">string</param>
        /// <returns>デコードされたsaml</returns>
        public static string DecodeRedirect(string queryString)
        {
            // EcodeRedirectの逆
            // --------------------------------------------------
            // Saml → URLデコード → Base64デコード
            //   → DEFLATE解凍 → XML宣言のエンコーディング → XML
            // --------------------------------------------------
            // Samlの抽出
            string saml = "";
            if (queryString.IndexOf("SAMLRequest") != -1)
            {
                saml = StringExtractor.GetParameterFromQueryString("SAMLRequest", queryString);
            }
            else if (queryString.IndexOf("SAMLResponse") != -1)
            {
                saml = StringExtractor.GetParameterFromQueryString("SAMLResponse", queryString);
            }
            else
            {
                return "";
            }

            byte[] tempByte = DeflateCompression.Decompress(
                CustomEncode.FromBase64String(CustomEncode.UrlDecode(saml)));

            // XML宣言部分を取得するために、us_asciiでデコード
            string tempString = CustomEncode.ByteToString(tempByte, CustomEncode.us_ascii);

            // エンコーディング オブジェクトの取得
            Encoding enc = XmlLib.GetEncodingFromXmlDeclaration(tempString);

            return CustomEncode.ByteToString(tempByte, enc.CodePage);
        }

        /// <summary>VerifyRedirect</summary>
        /// <param name="queryString">string</param>
        /// <param name="dsRSAwithSHA1">DigitalSign</param>
        /// <returns>bool</returns>
        public static bool VerifyRedirect(string queryString, DigitalSign dsRSAwithSHA1)
        {
            // EcodeRedirectの逆

            // Signatureの抽出
            string signature = StringExtractor.GetParameterFromQueryString("Signature", queryString);
            // Signatureの削除
            queryString = queryString.Replace("&Signature=" + signature, "");

            // queryString : ASCIIデコード
            // signature   : パラメタ → URLデコード →  Base64デコード
            if (dsRSAwithSHA1.Verify(
                CustomEncode.StringToByte(queryString, CustomEncode.us_ascii),
                CustomEncode.FromBase64String(CustomEncode.UrlDecode(signature))))
            {
                // 署名検証 OK
                return true;
            }
            else
            {
                // 署名検証 NG
                return false;
            }
        }
        #endregion

        #region Post
        /// <summary>DecodePost</summary>
        /// <param name="saml">エンコードされたsaml</param>
        /// <returns>デコードされたsaml</returns>
        public static string DecodePost(string saml)
        {
            // EncodePostの逆

            // Base64エンコード → XML宣言のエンコーディング → XML
            byte[] tempByte = CustomEncode.FromBase64String(saml);

            // XML宣言部分を取得するために、us_asciiでデコード
            string tempString = CustomEncode.ByteToString(tempByte, CustomEncode.us_ascii);

            // エンコーディング オブジェクトの取得
            Encoding enc = XmlLib.GetEncodingFromXmlDeclaration(tempString);

            return CustomEncode.ByteToString(tempByte, enc.CodePage);
        }

        /// <summary>DecodePost</summary>
        /// <param name="saml">デコードされたsaml</param>
        /// <param name="referenceId">string</param>
        /// <param name="rsa">RSA</param>
        /// <returns>bool</returns>
        public static bool VerifyPost(string saml, string referenceId, RSA rsa)
        {
            SignedXml2 signedXml2 = new SignedXml2(rsa);
            return signedXml2.Verify(saml, referenceId);
        }
        #endregion

        #endregion

        #endregion

        #region Verify Schema

        #region VerifyByXPath
        /// <summary>VerifyByXPath</summary>
        /// <param name="saml">string</param>
        /// <param name="schema">SAML2Enum.SamlSchema</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>bool</returns>
        public static bool VerifyByXPath(
            XmlDocument saml, SAML2Enum.SamlSchema schema, XmlNamespaceManager samlNsMgr)
        {
            bool result = false;
            XmlNodeList xmlNodeList = null;

            switch (schema)
            {
                case SAML2Enum.SamlSchema.Request:
                    xmlNodeList = saml.SelectNodes(
                        SAML2Const.XPathRequest, samlNsMgr);
                    if (xmlNodeList != null && xmlNodeList.Count == 1)
                    {
                        xmlNodeList = saml.SelectNodes(
                            SAML2Const.XPathIssuerInRequest, samlNsMgr);
                        if (xmlNodeList != null && xmlNodeList.Count == 1)
                        {
                            result = true;
                        }
                    }

                    break;

                case SAML2Enum.SamlSchema.Response:
                    xmlNodeList = saml.SelectNodes(
                        SAML2Const.XPathResponse, samlNsMgr);
                    if (xmlNodeList != null && xmlNodeList.Count == 1)
                    {
                        xmlNodeList = saml.SelectNodes(
                            SAML2Const.XPathIssuerInResponse, samlNsMgr);
                        if (xmlNodeList != null && xmlNodeList.Count == 1)
                        {
                            xmlNodeList = saml.SelectNodes(
                                SAML2Const.XPathStatusCodeInResponse, samlNsMgr);
                            if (xmlNodeList != null && xmlNodeList.Count == 1)
                            {
                                xmlNodeList = saml.SelectNodes(
                                    SAML2Const.XPathAssertionInResponse, samlNsMgr);
                                if (xmlNodeList != null && xmlNodeList.Count == 1)
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                    break;

                case SAML2Enum.SamlSchema.Assertion:
                    //...

                    break;
            }

            return result;
        }
        #endregion

        #region VerifyByXsd
        /// <summary>VerifyByXsd</summary>
        /// <param name="saml">string</param>
        /// <param name="schema">SAML2Enum.SamlSchema</param>
        /// <returns>bool</returns>
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
                    embeddedXsdFileName = "Touryo.Infrastructure.Framework.Authentication.SamlSchemaProtocol2.xsd";
                    targetNamespace = "urn:oasis:names:tc:SAML:2.0:protocol";
                    break;
                case SAML2Enum.SamlSchema.Assertion:
                    embeddedXsdFileName = "Touryo.Infrastructure.Framework.Authentication.SamlSchemaAssertion2.xsd";
                    targetNamespace = "urn:oasis:names:tc:SAML:2.0:assertion";
                    break;
                case SAML2Enum.SamlSchema.Response:
                    embeddedXsdFileName = "Touryo.Infrastructure.Framework.Authentication.SamlSchemaProtocol2.xsd";
                    targetNamespace = "urn:oasis:names:tc:SAML:2.0:protocol";
                    break;
            }

            // 以下の関数は適切に動作するが、XSDに問題がある。
            return XmlLib.ValidateByEmbeddedXsd(
                "OpenTouryo.Framework", saml, embeddedXsdFileName, targetNamespace);
        }

        #endregion

        #endregion

        #region 各種操作

        #region 値の取得用

        #region Request
        /// <summary>GetIssuerInRequest</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Issuer</returns>
        public static string GetIssuerInRequest(XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            XmlNodeList issuers = xmlDoc.SelectNodes(
                SAML2Const.XPathIssuerInRequest, samlNsMgr);

            if (issuers.Count != 0)
            {
                return issuers[0].InnerText.Trim();
            }

            return "";
        }

        /// <summary>GetNameIDPolicyFormatInRequest</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>NameIDPolicyFormat</returns>
        public static string GetNameIDPolicyFormatInRequest(XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, SAML2Const.XPathNameIDPolicyInRequest, "Format", samlNsMgr);
        }

        /// <summary>GetProtocolBindingInRequest</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>ProtocolBinding</returns>
        public static string GetProtocolBindingInRequest(XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, SAML2Const.XPathRequest, "ProtocolBinding", samlNsMgr);
        }

        /// <summary>GetAssertionConsumerServiceURLInRequest</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>AssertionConsumerServiceURL</returns>
        public static string GetAssertionConsumerServiceURLInRequest(XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, SAML2Const.XPathRequest, "AssertionConsumerServiceURL", samlNsMgr);
        }
        #endregion

        #region SamlAssertion
        /// <summary>GetIssuerInAssertion</summary>
        /// <param name="xmlNode">XmlNode</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Issuer</returns>
        public static string GetIssuerInSamlAssertion(XmlNode xmlNode, XmlNamespaceManager samlNsMgr)
        {
            XmlNodeList issuers = xmlNode.SelectNodes(
                SAML2Const.XPathIssuerInAssertion, samlNsMgr);

            if (issuers.Count != 0)
            {
                return issuers[0].InnerText.Trim();
            }

            return "";
        }

        /// <summary>GetSubjectInAssertion</summary>
        /// <param name="xmlNode">XmlNode</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Subject</returns>
        public static XmlNode GetSubjectInAssertion(XmlNode xmlNode, XmlNamespaceManager samlNsMgr)
        {
            XmlNodeList subject = xmlNode.SelectNodes(
                SAML2Const.XPathSubjectInAssertion, samlNsMgr);

            if (subject.Count != 0)
            {
                return subject[0];
            }

            return null;
        }

        /// <summary>GetConditionsInAssertion</summary>
        /// <param name="xmlNode">XmlNode</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Conditions</returns>
        public static XmlNode GetConditionsInAssertion(XmlNode xmlNode, XmlNamespaceManager samlNsMgr)
        {
            XmlNodeList conditions = xmlNode.SelectNodes(
                SAML2Const.XPathConditionsInSamlAssertion, samlNsMgr);

            if (conditions.Count != 0)
            {
                return conditions[0];
            }

            return null;
        }

        /// <summary>GetAuthnStatementInAssertion</summary>
        /// <param name="xmlNode">XmlNode</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>AuthnStatement</returns>
        public static XmlNode GetAuthnStatementInAssertion(XmlNode xmlNode, XmlNamespaceManager samlNsMgr)
        {
            XmlNodeList authnStatement = xmlNode.SelectNodes(
                SAML2Const.XPathAuthnStatementInAssertion, samlNsMgr);

            if (authnStatement.Count != 0)
            {
                return authnStatement[0];
            }

            return null;
        }
        #endregion

        #region SamlResponse
        /// <summary>GetIssuerInResponse</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Issuer</returns>
        public static string GetIssuerInResponse(XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            XmlNodeList issuers = xmlDoc.SelectNodes(
                SAML2Const.XPathIssuerInResponse, samlNsMgr);

            if (issuers.Count != 0)
            {
                return issuers[0].InnerText.Trim();
            }

            return "";
        }

        /// <summary>GetStatusCodeInResponse</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>StatusCode</returns>
        public static string GetStatusCodeInResponse(XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, SAML2Const.XPathStatusCodeInResponse, "Value", samlNsMgr);
        }

        /// <summary>GetAssertionInResponse</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Assertion</returns>
        public static XmlNode GetAssertionInResponse(XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            XmlNodeList assertion = xmlDoc.SelectNodes(
                SAML2Const.XPathAssertionInResponse, samlNsMgr);

            if (assertion.Count != 0)
            {
                return assertion[0];
            }

            return null;
        }
        #endregion

        #endregion

        #region ID取得用
        /// <summary>GetIdInRequest</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Id</returns>
        public static string GetIdInRequest(XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, @"/samlp:AuthnRequest", "ID", samlNsMgr);
        }

        /// <summary>GetIdInAssertion</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Id</returns>
        public static string GetIdInAssertion(XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, @"/saml:Assertion", "ID", samlNsMgr);
        }

        /// <summary>GetIdInResponse</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Id</returns>
        public static string GetIdInResponse(XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, @"/samlp:Response", "ID", samlNsMgr);
        }
        #endregion

        #region Create NamespaceManager
        /// <summary>CreateNamespaceManager</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <returns>XmlNamespaceManager</returns>
        public static XmlNamespaceManager CreateNamespaceManager(XmlDocument xmlDoc)
        {
            XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            xmlnsManager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            xmlnsManager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

            return xmlnsManager;
        }
        #endregion 

        #endregion
    }
}
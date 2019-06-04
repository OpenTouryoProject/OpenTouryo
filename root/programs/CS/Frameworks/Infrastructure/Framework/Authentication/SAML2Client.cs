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
//* クラス名        ：SAML2Client
//* クラス日本語名  ：SAML2Client（ライブラリ）
//*
//* 作成日時        ：－
//* 作成者          ：－
//* 更新履歴        ：－
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/06/04  西野 大介         新規作成
//**********************************************************************************

using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Touryo.Infrastructure.Public.Security;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2Client（ライブラリ）</summary>
    public class SAML2Client
    {
        #region CreateRequest
        /// <summary>CreateRedirectRequest</summary>
        /// <param name="createRoR">SAML2Enum.RequestOrResponse</param>
        /// <param name="protocolBinding">SAML2Enum.ProtocolBinding</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="iss">string</param>
        /// <param name="assertionConsumerService">string</param>
        /// <param name="relayState">string</param>
        /// <param name="id">out string</param>
        /// <returns>queryString</returns>
        public static string CreateRedirectRequest(
            SAML2Enum.RequestOrResponse createRoR,
            SAML2Enum.ProtocolBinding protocolBinding,
            SAML2Enum.NameIDFormat nameIDFormat,
            string iss, string assertionConsumerService, string relayState, out string id)
        {   
            // DigitalSignX509
            DigitalSignX509 dsX509 = new DigitalSignX509(
                OAuth2AndOIDCParams.RS256Pfx, OAuth2AndOIDCParams.RS256Pwd, HashAlgorithmName.SHA1,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

            // SamlRequestの生成
            
            string samlRequest = SAML2Bindings.CreateRequest(
                iss, protocolBinding, nameIDFormat,
                assertionConsumerService, out id).OuterXml;

            // SamlRequestのエンコと、QueryStringを生成（ + 署名）
            return SAML2Bindings.EncodeAndSignRedirect(
                createRoR, samlRequest, relayState, dsX509);
        }

        /// <summary>CreatePostRequest</summary>
        /// <param name="protocolBinding">SAML2Enum.ProtocolBinding</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="iss">string</param>
        /// <param name="assertionConsumerService">string</param>
        /// <param name="relayState">string</param>
        /// <param name="id">out string</param>
        /// <returns>samlRequest</returns>
        public static string CreatePostRequest(
            SAML2Enum.ProtocolBinding protocolBinding,
            SAML2Enum.NameIDFormat nameIDFormat,
            string iss, string assertionConsumerService, string relayState, out string id)
        {
            // RSA
            X509Certificate2 x509 = new X509Certificate2(
                OAuth2AndOIDCParams.RS256Pfx,
                OAuth2AndOIDCParams.RS256Pwd);

            // SamlRequestの生成
            string samlRequest = SAML2Bindings.CreateRequest(
                iss, protocolBinding, nameIDFormat,
                assertionConsumerService, out id).OuterXml;

            // SamlRequestのエンコと署名
            return SAML2Bindings.EncodeAndSignPost(
                samlRequest, id, x509.GetRSAPrivateKey());
        }
        #endregion

        #region VerifyResponse

        public static bool VerifySamlResponse(
            string queryString, string samlResponse, out string iss)
        {
            bool verified = false;

            string id = "";
            iss = "";

            string decodeSaml = "";
            if (!string.IsNullOrEmpty(queryString))
            {
                decodeSaml = SAML2Bindings.DecodeRedirect(queryString);
            }
            else if (!string.IsNullOrEmpty(samlResponse))
            {
                decodeSaml = SAML2Bindings.DecodePost(samlResponse);
            }
            else
            {
                return false;
            }

            // XmlDocument
            XmlDocument samlResponse2 = new XmlDocument();
            samlResponse2.PreserveWhitespace = false;
            samlResponse2.LoadXml(decodeSaml);

            // XmlNamespaceManager
            XmlNamespaceManager samlNsMgr = SAML2Bindings.CreateNamespaceManager(samlResponse2);

            id = SAML2Bindings.GetIdInResponse(samlResponse2, samlNsMgr);
            iss = SAML2Bindings.GetIssuerInResponse(samlResponse2, samlNsMgr);
            

            // Metadata利用を検討
            DigitalSignX509 dsX509 = new DigitalSignX509(
                    OAuth2AndOIDCParams.RS256Cer, "", HashAlgorithmName.SHA1,
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

            if (!string.IsNullOrEmpty(queryString))
            {
                // VerifyRedirect
                if (SAML2Bindings.VerifyRedirect(queryString, dsX509))
                {
                    // XSDスキーマによる検証
                    // https://developers.onelogin.com/saml/online-tools/validate/xml-against-xsd-schema
                    // The XML is valid.

                    // XPathによる検証
                    verified = SAML2Bindings.VerifyByXPath(
                        samlResponse2, SAML2Enum.SamlSchema.Response, samlNsMgr);

                    // 
                    iss = SAML2Bindings.GetIssuerInAssertion(samlResponse2, samlNsMgr);
                }
            }
            else
            {
                // VerifyPost
                if (SAML2Bindings.VerifyPost(
                    decodeSaml, id, dsX509.X509Certificate.GetRSAPublicKey()))
                {
                    // XSDスキーマによる検証
                    // https://developers.onelogin.com/saml/online-tools/validate/xml-against-xsd-schema
                    // The XML is valid. (ただし、Signature要素は外す。

                    // XPathによる検証
                    verified = SAML2Bindings.VerifyByXPath(
                        samlResponse2, SAML2Enum.SamlSchema.Response, samlNsMgr);

                    // 
                    iss = SAML2Bindings.GetIssuerInAssertion(samlResponse2, samlNsMgr);
                }
            }

            return verified;
        }
        #endregion
    }
}
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

using System;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Touryo.Infrastructure.Public.Str;
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
                CmnClientParams.RsaPfxFilePath, CmnClientParams.RsaPfxPassword,
#if NET45
                HashNameConst.SHA1,
#else
                HashAlgorithmName.SHA1,
#endif
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
                CmnClientParams.RsaPfxFilePath, CmnClientParams.RsaPfxPassword);

            // SamlRequestの生成
            string samlRequest = SAML2Bindings.CreateRequest(
                iss, protocolBinding, nameIDFormat,
                assertionConsumerService, out id).OuterXml;

            // SamlRequestのエンコと署名
            return SAML2Bindings.EncodeAndSignPost(samlRequest, id,
#if NET45
                (RSA)x509.PrivateKey);
#else
                x509.GetRSAPrivateKey());
# endif
        }
        #endregion

        #region VerifyResponse

        /// <summary>VerifyResponse</summary>
        /// <param name="queryString">string</param>
        /// <param name="samlResponse">string</param>
        /// <param name="nameId">out string</param>
        /// <param name="iss">out string</param>
        /// <param name="aud">out string</param>
        /// <param name="inResponseTo">out string</param>
        /// <param name="recipient">out string</param>
        /// <param name="notOnOrAfter">out DateTime</param>
        /// <param name="statusCode">SAML2Enum.StatusCode</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="authnContextClassRef">SAML2Enum.AuthnContextClassRef</param>
        /// <param name="samlResponse2">XmlDocument</param>
        /// <returns>bool</returns>
        public static bool VerifyResponse(
            string queryString, string samlResponse,
            out string nameId, out string iss, out string aud,
            out string inResponseTo, out string recipient, out DateTime? notOnOrAfter,
            out SAML2Enum.StatusCode? statusCode,
            out SAML2Enum.NameIDFormat? nameIDFormat, 
            out SAML2Enum.AuthnContextClassRef? authnContextClassRef,
            out XmlDocument samlResponse2)
        {
            bool verified = false;

            // out string
            nameId = "";
            iss = "";
            aud = "";

            inResponseTo = "";
            recipient = "";

            // out DateTime?
            notOnOrAfter = null;

            // out SAML2Enum
            statusCode = null;
            nameIDFormat = null;
            authnContextClassRef = null;

            // out XmlDocument
            samlResponse2 = null;

#region 準備
            // Decode
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
            samlResponse2 = new XmlDocument();
            samlResponse2.PreserveWhitespace = false;
            samlResponse2.LoadXml(decodeSaml);

            // XmlNamespaceManager
            XmlNamespaceManager samlNsMgr = SAML2Bindings.CreateNamespaceManager(samlResponse2);
#endregion

#region 検証
            // Metadata利用を検討
            DigitalSignX509 dsX509 = new DigitalSignX509(
                CmnClientParams.RsaCerFilePath, "",
#if NET45
                HashNameConst.SHA1,
#else
                HashAlgorithmName.SHA1,
#endif
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

            if (!string.IsNullOrEmpty(queryString))
            {
                // VerifyRedirect
                if (SAML2Bindings.VerifyRedirect(queryString, dsX509))
                {
                    // XPathによる検証
                    verified = SAML2Bindings.VerifyByXPath(
                        samlResponse2, SAML2Enum.SamlSchema.Response, samlNsMgr);
                }
            }
            else
            {
                // VerifyPost
                string id = SAML2Bindings.GetIdInResponse(samlResponse2, samlNsMgr);
                if (SAML2Bindings.VerifyPost(decodeSaml, id,
#if NET45
                (RSA)dsX509.PublicKey))
#else
                dsX509.X509Certificate.GetRSAPublicKey()))
#endif
                {
                    // XPathによる検証
                    verified = SAML2Bindings.VerifyByXPath(
                        samlResponse2, SAML2Enum.SamlSchema.Response, samlNsMgr);
                }
            }
#endregion

            // 値のチェック
            if (verified)
            {
                string temp1 = "";
                string temp2 = "";

                // StatusCode
                SAML2Enum.StringToEnum(
                    SAML2Bindings.GetStatusCodeInResponse(
                        samlResponse2, samlNsMgr), out statusCode);

                if (statusCode == SAML2Enum.StatusCode.Success)
                {
                    // iss
                    temp1 = SAML2Bindings.GetIssuerInResponse(samlResponse2, samlNsMgr);
                    temp2 = SAML2Bindings.GetIssuerInAssertion(samlResponse2, samlNsMgr);

                    if (temp1 == temp2)
                    {
                        iss = temp1;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(temp1))
                        {
                            // temp2のみ値がある
                            if (!string.IsNullOrEmpty(temp2)) iss = temp2;
                        }
                        else if (string.IsNullOrEmpty(temp2))
                        {
                            // temp1のみ値がある
                            if (!string.IsNullOrEmpty(temp1)) iss = temp1;
                        }
                        else
                        {
                            // Assertionは使用できない。
                            return false;
                        } 
                    }

                    // NameID
                    string format = "";
                    SAML2Bindings.GetNameIDInAssertion(
                        samlResponse2, samlNsMgr, out format, out nameId);
                    SAML2Enum.StringToEnum(format, out nameIDFormat);

                    // SubjectConfirmationData
                    string _inResponseTo = "";
                    string _notOnOrAfter = "";
                    XmlNode subjectConfirmationData = 
                        SAML2Bindings.GetSubjectConfirmationDataInAssertion(
                            samlResponse2, samlNsMgr, out _inResponseTo, out _notOnOrAfter, out recipient);

                    temp1 = _inResponseTo;
                    temp2 = SAML2Bindings.GetInResponseToInResponse(samlResponse2, samlNsMgr);

                    if (temp1 == temp2)
                    {
                        inResponseTo = temp1;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(temp1))
                        {
                            // temp2のみ値がある
                            if (!string.IsNullOrEmpty(temp2)) inResponseTo = temp2;
                        }
                        else if (string.IsNullOrEmpty(temp2))
                        {
                            // temp1のみ値がある
                            if (!string.IsNullOrEmpty(temp1)) inResponseTo = temp1;
                        }
                        else
                        {
                            // Assertionは使用できない。
                            return false;
                        }
                    }

                    // Conditions

                    // - aud
                    aud = SAML2Bindings.GetAudienceInAssertion(samlResponse2, samlNsMgr);

                    // - notOnOrAfter
                    temp1 = _notOnOrAfter;
                    temp2 = SAML2Bindings.GetConditionsNotOnOrAfterInAssertion(samlResponse2, samlNsMgr);
                    DateTime time1 = DateTime.MinValue;
                    DateTime time2 = DateTime.MinValue;

                    if (temp1 == temp2)
                    {
                        // 等しい場合
                        notOnOrAfter = FormatConverter.FromSamlTime(temp1);
                    }
                    else
                    {
                        // 等しくない場合
                        time1 = FormatConverter.FromSamlTime(temp1);
                        time2 = FormatConverter.FromSamlTime(temp2);

                        // 短い値を使用
                        if (time1.Ticks <= time2.Ticks)
                        {
                            notOnOrAfter = time1;
                        }
                        else
                        {
                            notOnOrAfter = time2;
                        }
                    }

                    // 現在時刻と比較
                    if (!(DateTime.UtcNow.Ticks <= notOnOrAfter.Value.Ticks))
                    {
                        return false; // Assertionは使用できない。
                    }

                    // AuthnContextClassRef
                    SAML2Enum.StringToEnum(
                        SAML2Bindings.GetAuthnContextClassRefInAssertion(
                        samlResponse2, samlNsMgr), out authnContextClassRef);
                }
                else
                {
                    return false; // Assertionは使用できない。
                }
            }

            return verified;
        }
#endregion
    }
}
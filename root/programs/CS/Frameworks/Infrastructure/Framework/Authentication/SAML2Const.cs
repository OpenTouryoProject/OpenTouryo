//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
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
//* クラス名        ：SAML2Const
//* クラス日本語名  ：SAML2の各種定数
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/05/21  西野 大介         新規作成
//**********************************************************************************

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2の各種定数</summary>
    public class SAML2Const
    {
        #region Template

        // InnerTextに空白が混じる問題があり、
        // 以下の様に要素内の改行は無くした。

        /// <summary>RequestTemplate</summary>
        /// <remarks>
        /// 以下はオプションで追加
        /// ProtocolBinding属性
        /// AssertionConsumerServiceURL属性
        /// </remarks>
        public const string RequestTemplate
            = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
            + "<samlp:AuthnRequest"
            + "  Version=\"2.0\" ID=\"{ID}\""
            + "  IssueInstant=\"{IssueInstant}\""
            + "  xmlns:saml=\"{UrnAssertion}\""
            + "  xmlns:samlp=\"{UrnProtocol}\">"
            + "  <saml:Issuer>{Issuer}</saml:Issuer>"
            + "  <samlp:NameIDPolicy Format=\"{UrnNameIDFormat}\" />"
            + "</samlp:AuthnRequest>";

        /// <summary>ResponseTemplate</summary>
        public const string ResponseTemplate
            = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
            + "<samlp:Response"
            + "  Version=\"2.0\" ID=\"{ID}\""
            + "  IssueInstant=\"{IssueInstant}\""
            + "  InResponseTo=\"{InResponseTo}\""
            + "  Destination=\"{Destination}\""
            + "  xmlns:saml=\"{UrnAssertion}\""
            + "  xmlns:samlp=\"{UrnProtocol}\">"
            + "  <saml:Issuer>{Issuer}</saml:Issuer>"
            + "  <samlp:Status>"
            + "    <samlp:StatusCode Value=\"{UrnStatusCode}\" />"
            + "  </samlp:Status>"
            //+ "  <saml:Assertion>{Assertion}</saml:Assertion>"
            + "</samlp:Response>";

        /// <summary>AssertionTemplate</summary>
        public const string AssertionTemplate
            = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
            + "<saml:Assertion"
            + "  Version=\"2.0\" ID=\"{ID}\""
            + "  IssueInstant=\"{IssueInstant}\""
            + "  xmlns:saml=\"{UrnAssertion}\""
            + "  xmlns:samlp=\"{UrnProtocol}\">"
            + "  <saml:Issuer>{Issuer}</saml:Issuer>"
            + "  <saml:Subject>"
            + "    <saml:NameID Format=\"{UrnNameIDFormat}\">{NameID}</saml:NameID>"
            + "    <saml:SubjectConfirmation Method=\"{UrnMethod}\">"
            + "      <saml:SubjectConfirmationData"
            + "        InResponseTo=\"{InResponseTo}\""
            + "        NotOnOrAfter=\"{NotOnOrAfter}\""
            + "        Recipient=\"{Recipient}\" />"
            + "    </saml:SubjectConfirmation>"
            + "  </saml:Subject>"
            + "  <saml:Conditions"
            + "    NotBefore=\"{NotBefore}\""
            + "    NotOnOrAfter=\"{NotOnOrAfter}\">"
            + "    <saml:AudienceRestriction>"
            + "      <saml:Audience>{Audience}</saml:Audience>"
            + "    </saml:AudienceRestriction>"
            + "  </saml:Conditions>"
            + "  <saml:AuthnStatement"
            + "    AuthnInstant=\"{AuthnInstant}\">"
            + "    <saml:AuthnContext>"
            + "      <saml:AuthnContextClassRef>{UrnAuthnContextClassRef}</saml:AuthnContextClassRef>"
            + "    </saml:AuthnContext>"
            + "  </saml:AuthnStatement>"
            + "</saml:Assertion>";

        /// <summary>MetadataTemplate</summary>
        public const string MetadataTemplate
            = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
            + "<md:EntityDescriptor"
            + "  xmlns:md=\"{UrnMetadata}\""
            + "  entityID=\"{EntityID}\">"
            + "  <md:IDPSSODescriptor"
            + "    WantAuthnRequestsSigned=\"{WantAuthnRequestsSigned}\""
            + "    protocolSupportEnumeration=\"{UrnProtocolSupportEnumeration}\">"
            + "    <md:KeyDescriptor use=\"signing\">"
            + "      <ds:KeyInfo xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\">"
            + "        <ds:X509Data>"
            + "          <ds:X509Certificate>{X509CertificatePemString}</ds:X509Certificate>"
            + "        </ds:X509Data>"
            + "      </ds:KeyInfo>"
            + "    </md:KeyDescriptor>"
            //+ "    <md:NameIDFormat>{NameIDFormat}</md:NameIDFormat>"
            + "    <md:SingleSignOnService"
            + "      Binding=\"{SingleSignOnServiceRedirect}\""
            + "      Location=\"{SingleSignOnServiceRedirectLocation}\">"
            + "    <md:SingleSignOnService"
            + "      Binding=\"{SingleSignOnServicePost}\""
            + "      Location=\"{SingleSignOnServicePostLocation}\">"
            + "  </md:IDPSSODescriptor>"
            + "</md:EntityDescriptor>";
        #endregion

        #region XPath

        #region Request
        /// <summary>XPathRequest</summary>
        public const string XPathRequest = @"/samlp:AuthnRequest";

        /// <summary>XPathIssuerInRequest</summary>
        public const string XPathIssuerInRequest = XPathRequest + @"/saml:Issuer";

        /// <summary>XPathNameIDPolicyInRequest</summary>
        public const string XPathNameIDPolicyInRequest = XPathRequest + @"/samlp:NameIDPolicy";
        #endregion

        #region Response
        /// <summary>XPathResponse</summary>
        public const string XPathResponse = @"/samlp:Response";

        /// <summary>XPathIssuerInResponse</summary>
        public const string XPathIssuerInResponse = XPathResponse + @"/saml:Issuer";

        /// <summary>XPathStatusCodeInResponse</summary>
        public const string XPathStatusCodeInResponse = XPathResponse + @"/samlp:Status/samlp:StatusCode";

        /// <summary>XPathAssertionInResponse</summary>
        public const string XPathAssertionInResponse = XPathResponse + @"/saml:Assertion";
        #endregion

        #region Assertion
        
        /// <summary>XPathIssuerInAssertion</summary>
        public const string XPathIssuerInAssertion = XPathAssertionInResponse + @"/saml:Issuer";

        #region Subject
        /// <summary>XPathSubjectInAssertion</summary>
        public const string XPathSubjectInAssertion = XPathAssertionInResponse + @"/saml:Subject";

        /// <summary>XPathNameIDInAssertion</summary>
        public const string XPathNameIDInAssertion = XPathSubjectInAssertion + @"/saml:NameID";

        /// <summary>XPathSubjectConfirmationInAssertion</summary>
        public const string XPathSubjectConfirmationInAssertion = XPathSubjectInAssertion + @"/saml:SubjectConfirmation";

        /// <summary>XPathSubjectConfirmationDataInAssertion</summary>
        public const string XPathSubjectConfirmationDataInAssertion = XPathSubjectConfirmationInAssertion + @"/saml:SubjectConfirmationData";
        #endregion

        #region Conditions
        /// <summary>XPathConditionsInAssertion</summary>
        public const string XPathConditionsInAssertion = XPathAssertionInResponse + @"/saml:Conditions";

        /// <summary>XPathAudienceInAssertion</summary>
        public const string XPathAudienceInAssertion =
            XPathConditionsInAssertion + @"/saml:AudienceRestriction/saml:Audience";
        #endregion

        #region AuthnStatement
        /// <summary>XPathAuthnContextClassRefInAssertion</summary>
        public const string XPathAuthnContextClassRefInAssertion = 
            XPathAssertionInResponse + @"/saml:AuthnStatement/saml:AuthnContext/saml:AuthnContextClassRef";
        #endregion

        #endregion

        #endregion

        #region urn

        #region header
        /// <summary>SAML1.1系のurnヘッダ</summary>
        public const string UrnHeader11 = "urn:oasis:names:tc:SAML:1.1:";

        /// <summary>SAML2.0系のurnヘッダ</summary>
        public const string UrnHeader20 = "urn:oasis:names:tc:SAML:2.0:";
        #endregion

        #region method
        /// <summary>メソッド：持参人切符</summary>
        public const string UrnMethodBearer = UrnHeader20 + "cm:bearer";

        /// <summary>メソッド：記名式切符</summary>
        public const string UrnMethodPoP = UrnHeader20 + "cm:holder-of-key";
        #endregion

        #region fixed
        /// <summary>プロトコルを意味する名前空間</summary>
        public const string UrnProtocol = UrnHeader20 + "protocol";

        /// <summary>アサーションを意味する名前空間</summary>
        public const string UrnAssertion = UrnHeader20 + "assertion";

        /// <summary>メタデータを意味する名前空間</summary>
        public const string UrnMetadata = UrnHeader20 + "metadata";

        #endregion

        #region bindings
        /// <summary>レスポンスのBindingをPOSTに指定する。</summary>
        public const string UrnBindingsPost = UrnHeader20 + "bindings:HTTP-POST";
        
        /// <summary>レスポンスのBindingをRedirectに指定する。</summary>
        public const string UrnBindingsRedirect = UrnHeader20 + "bindings:HTTP-Redirect";
        #endregion

        #region nameid-format
        /// <summary>NameIDPolicy要素のFormat属性をunspecifiedに指定する。</summary>
        public const string UrnNameIDFormatUnspecified = UrnHeader11 + "nameid-format:unspecified";

        /// <summary>NameIDPolicy要素のFormat属性を永続的仮名に指定する。</summary>
        public const string UrnNameIDFormatPersistent = UrnHeader20 + "nameid-format:persistent";

        /// <summary>NameIDPolicy要素のFormat属性を一時仮名に指定する。</summary>
        public const string UrnNameIDFormatTransient = UrnHeader20 + "nameid-format:transient";
        #endregion

        #region classes
        /// <summary>AuthnContextClassRefをunspecified（不特定の方法で認証）に指定する。</summary>
        public const string UrnAuthnContextClassRefUnspecified = UrnHeader20 + "ac:classes:unspecified";

        /// <summary>AuthnContextClassRefをPassword（HTTP + パスワードを提示して認証）に指定する。</summary>
        public const string UrnAuthnContextClassRefPassword = UrnHeader20 + "ac:classes:Password";
        
        /// <summary>AuthnContextClassRefをPasswordProtectedTransport（HTTPS + パスワードを提示して認証）に指定する。</summary>
        public const string UrnAuthnContextClassRefPasswordProtectedTransport = UrnHeader20 + "ac:classes:PasswordProtectedTransport";

        /// <summary>AuthnContextClassRefをPreviousSession（認証済みセッション）に指定する。</summary>
        public const string UrnAuthnContextClassRefPreviousSession = UrnHeader20 + "ac:classes:PreviousSession";
        
        /// <summary>AuthnContextClassRefをX509（デジタル署名により認証）に指定する。</summary>
        public const string UrnAuthnContextClassRefX509 = UrnHeader20 + "ac:classes:X509";
        #endregion

        #region status
        /// <summary>StatusのStatusCodeをSuccessに指定する。</summary>
        public const string UrnStatusCodeSuccess = UrnHeader20 + "status:Success";

        /// <summary>StatusのStatusCodeをRequesterに指定する。</summary>
        public const string UrnStatusCodeRequester = UrnHeader20 + "status:Requester";

        /// <summary>StatusのStatusCodeをResponderに指定する。</summary>
        public const string UrnStatusCodeResponder = UrnHeader20 + "status:Responder";

        /// <summary>StatusのStatusCodeをAuthnFailedに指定する。</summary>
        public const string UrnStatusCodeAuthnFailed = UrnHeader20 + "status:AuthnFailed";

        /// <summary>StatusのStatusCodeをUnknownPrincipalに指定する。</summary>
        public const string UrnStatusCodeUnknownPrincipal = UrnHeader20 + "status:UnknownPrincipal";

        /// <summary>StatusのStatusCodeをVersionMismatchに指定する。</summary>
        public const string UrnStatusCodeVersionMismatch = UrnHeader20 + "status:VersionMismatch";
        #endregion

        #endregion

        #region url
        /// <summary>RSAwithSHA1</summary>
        public const string RSAwithSHA1 = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

        /// <summary>DSAwithSHA1</summary>
        public const string DSAwithSHA1 = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
        #endregion
    }
}

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
//*  2019/12/25  西野 大介         PPID対応による見直し
//*  2026/10/12  玄人 幸道         RSAwithSHA256 / 384 / 512 を追加（#604）
//*  2026/10/22  玄人 幸道         XML署名の名前空間、AttributeStatementのXPath・属性名を追加（#634）
//*  2026/10/22  玄人 幸道         メタデータのXPath、接頭辞、要素名・属性名、FriendlyName、WS-Federationのクレーム名を追加
//*                                （SAML2Metadata / SAML2Attributes の直書きを寄せた）
//*  2026/10/22  玄人 幸道         並びを整理した（属性の区分を urn の外へ出し、要素名・属性名を小区分に分けた）。
//*                                接頭辞 samlp と、HTTP-Redirect のクエリ文字列のパラメタ名を追加。
//*                                MetadataTemplate の use / xmlns:ds を定数から組み立てる（値は変えていない）
//*  2026/10/22  玄人 幸道         SAML2Bindings の直書きを寄せるため、XPathAssertion、要求・応答・アサーション・
//*                                メタデータの要素名・属性名、ID の接頭辞を追加
//*  2026/10/30  玄人 幸道         RSA-PSS・ECDSA の SigAlg の URI を追加し、RSA の注記を RFC 9231 にした（#643）
//**********************************************************************************

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2の各種定数</summary>
    public class SAML2Const
    {
        #region Template

        // InnerTextに空白が混じる問題があり、
        // 以下の様に要素内の改行は無くした。

        /// <summary>XML宣言(はMetadata以外には付与しないらしい</summary>
        private const string XmlDeclaration = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

        /// <summary>RequestTemplate</summary>
        /// <remarks>
        /// 以下はオプションで追加
        /// ProtocolBinding属性
        /// AssertionConsumerServiceURL属性
        /// </remarks>
        public const string RequestTemplate
            //= SAML2Const.XmlDeclaration
            = "<samlp:AuthnRequest"
            + "  Version=\"2.0\" ID=\"{ID}\""
            + "  IssueInstant=\"{IssueInstant}\""
            + "  xmlns:saml=\"{UrnAssertion}\""
            + "  xmlns:samlp=\"{UrnProtocol}\">"
            + "  <saml:Issuer>{Issuer}</saml:Issuer>"
            + "  <samlp:NameIDPolicy Format=\"{UrnNameIDFormat}\" />"
            + "</samlp:AuthnRequest>";

        /// <summary>ResponseTemplate</summary>
        public const string ResponseTemplate
            //= SAML2Const.XmlDeclaration
            = "<samlp:Response"
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
            //= SAML2Const.XmlDeclaration
            = "<saml:Assertion"
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
            = SAML2Const.XmlDeclaration
            + "<md:EntityDescriptor"
            + "  xmlns:md=\"{UrnMetadata}\""
            + "  entityID=\"{EntityID}\">"
            + "  <md:IDPSSODescriptor"
            + "    WantAuthnRequestsSigned=\"{WantAuthnRequestsSigned}\""
            + "    protocolSupportEnumeration=\"{UrnProtocolSupportEnumeration}\">"
            + "    <md:KeyDescriptor use=\"" + SAML2Const.KeyUseSigning + "\">"
            + "      <ds:KeyInfo xmlns:ds=\"" + SAML2Const.UrnDSig + "\">"
            + "        <ds:X509Data>"
            + "          <ds:X509Certificate>{X509CertificatePemString}</ds:X509Certificate>"
            + "        </ds:X509Data>"
            + "      </ds:KeyInfo>"
            + "    </md:KeyDescriptor>"
            //+ "    <md:NameIDFormat>{NameIDFormat}</md:NameIDFormat>"
            + "    <md:SingleSignOnService"
            + "      Binding=\"{SingleSignOnServiceRedirect}\""
            + "      Location=\"{SingleSignOnServiceRedirectLocation}\" />"
            + "    <md:SingleSignOnService"
            + "      Binding=\"{SingleSignOnServicePost}\""
            + "      Location=\"{SingleSignOnServicePostLocation}\" />"
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

        /// <summary>XPathAssertion（アサーション単体の文書。CreateAssertion の戻りなど）</summary>
        public const string XPathAssertion = @"/saml:Assertion";

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

        #region AttributeStatement
        /// <summary>XPathAttributeInAssertion</summary>
        public const string XPathAttributeInAssertion =
            XPathAssertionInResponse + @"/saml:AttributeStatement/saml:Attribute";

        /// <summary>XPathAttributeValueInAttribute（Attribute からの相対）</summary>
        public const string XPathAttributeValueInAttribute = @"saml:AttributeValue";
        #endregion

        #endregion

        #region Metadata（相手のメタデータを読む。SAML2Metadata）
        /// <summary>XPathEntityDescriptor</summary>
        public const string XPathEntityDescriptor = @"/md:EntityDescriptor";

        /// <summary>XPathIDPSSODescriptorInMetadata</summary>
        public const string XPathIDPSSODescriptorInMetadata = XPathEntityDescriptor + @"/md:IDPSSODescriptor";

        /// <summary>XPathKeyDescriptorInMetadata</summary>
        public const string XPathKeyDescriptorInMetadata = XPathIDPSSODescriptorInMetadata + @"/md:KeyDescriptor";

        /// <summary>XPathSingleSignOnServiceInMetadata</summary>
        public const string XPathSingleSignOnServiceInMetadata = XPathIDPSSODescriptorInMetadata + @"/md:SingleSignOnService";

        /// <summary>XPathX509CertificateInKeyDescriptor（KeyDescriptor からの相対）</summary>
        public const string XPathX509CertificateInKeyDescriptor = @"ds:KeyInfo/ds:X509Data/ds:X509Certificate";
        #endregion

        #endregion

        #region 名前空間の接頭辞（XPath の中で使う）
        /// <summary>アサーション（saml）</summary>
        public const string PrefixAssertion = "saml";

        /// <summary>プロトコル（samlp）</summary>
        public const string PrefixProtocol = "samlp";

        /// <summary>メタデータ（md）</summary>
        public const string PrefixMetadata = "md";

        /// <summary>XML署名（ds）</summary>
        public const string PrefixDSig = "ds";
        #endregion

        #region 要素名・属性名

        #region AttributeStatement
        /// <summary>要素名：Assertion</summary>
        public const string ElementAssertion = "Assertion";

        /// <summary>要素名：AttributeStatement</summary>
        public const string ElementAttributeStatement = "AttributeStatement";

        /// <summary>要素名：Attribute</summary>
        public const string ElementAttribute = "Attribute";

        /// <summary>要素名：AttributeValue</summary>
        public const string ElementAttributeValue = "AttributeValue";

        /// <summary>属性名：Name（Attribute）</summary>
        public const string AttrName = "Name";

        /// <summary>属性名：NameFormat（Attribute）</summary>
        public const string AttrNameFormat = "NameFormat";

        /// <summary>属性名：FriendlyName（Attribute）</summary>
        public const string AttrFriendlyName = "FriendlyName";
        #endregion

        #region Request / Response / Assertion
        /// <summary>要素名：AuthnRequest</summary>
        public const string ElementAuthnRequest = "AuthnRequest";

        /// <summary>属性名：ID（AuthnRequest / Response / Assertion）</summary>
        public const string AttrID = "ID";

        /// <summary>属性名：Format（NameIDPolicy / NameID）</summary>
        public const string AttrFormat = "Format";

        /// <summary>属性名：ProtocolBinding（AuthnRequest）</summary>
        public const string AttrProtocolBinding = "ProtocolBinding";

        /// <summary>属性名：AssertionConsumerServiceURL（AuthnRequest）</summary>
        public const string AttrAssertionConsumerServiceURL = "AssertionConsumerServiceURL";

        /// <summary>属性名：InResponseTo（Response / SubjectConfirmationData）</summary>
        public const string AttrInResponseTo = "InResponseTo";

        /// <summary>属性名：NotOnOrAfter（SubjectConfirmationData / Conditions）</summary>
        public const string AttrNotOnOrAfter = "NotOnOrAfter";

        /// <summary>属性名：Recipient（SubjectConfirmationData）</summary>
        public const string AttrRecipient = "Recipient";

        /// <summary>属性名：Method（SubjectConfirmation）</summary>
        public const string AttrMethod = "Method";

        /// <summary>属性名：Value（StatusCode）</summary>
        public const string AttrValue = "Value";
        #endregion

        #region Metadata
        /// <summary>要素名：IDPSSODescriptor</summary>
        public const string ElementIDPSSODescriptor = "IDPSSODescriptor";

        /// <summary>要素名：NameIDFormat</summary>
        public const string ElementNameIDFormat = "NameIDFormat";

        /// <summary>属性名：entityID（EntityDescriptor）</summary>
        public const string AttrEntityID = "entityID";

        /// <summary>属性名：use（KeyDescriptor）</summary>
        public const string AttrUse = "use";

        /// <summary>属性名：Binding（SingleSignOnService）</summary>
        public const string AttrBinding = "Binding";

        /// <summary>属性名：Location（SingleSignOnService）</summary>
        public const string AttrLocation = "Location";

        /// <summary>KeyDescriptor の use の値：signing（無ければ、どちらにも使える。Metadata 2.4.1.1）</summary>
        public const string KeyUseSigning = "signing";
        #endregion

        #endregion

        #region ID
        /// <summary>生成する ID（AuthnRequest / Response / Assertion の ID 属性）の接頭辞</summary>
        /// <remarks>XML の ID 型は数字で始められない（xs:ID ＝ NCName）ので、GUID の前に付ける。</remarks>
        public const string IdPrefix = "s2";
        #endregion

        #region HTTP-Redirect のクエリ文字列のパラメタ名（Bindings 3.4.4）
        /// <summary>SAMLRequest</summary>
        public const string QueryParamSAMLRequest = "SAMLRequest";

        /// <summary>SAMLResponse</summary>
        public const string QueryParamSAMLResponse = "SAMLResponse";

        /// <summary>RelayState</summary>
        public const string QueryParamRelayState = "RelayState";

        /// <summary>SigAlg</summary>
        public const string QueryParamSigAlg = "SigAlg";

        /// <summary>Signature</summary>
        public const string QueryParamSignature = "Signature";
        #endregion

        #region 属性（Attribute。SAML2Attributes）

        #region attrname-format（SAML Core 8.2）
        /// <summary>属性名の形式：URI（urn:oid:… などに使う）</summary>
        public const string UrnAttrNameFormatUri = UrnHeader20 + "attrname-format:uri";

        /// <summary>属性名の形式：basic</summary>
        public const string UrnAttrNameFormatBasic = UrnHeader20 + "attrname-format:basic";
        #endregion

        #region X.500/LDAP Attribute Profile
        /// <summary>メアド（mail）</summary>
        public const string UrnOidMail = "urn:oid:0.9.2342.19200300.100.1.3";

        /// <summary>利用者名（uid。OIDCのpreferred_usernameに対応）</summary>
        public const string UrnOidUid = "urn:oid:0.9.2342.19200300.100.1.1";

        /// <summary>メアドの FriendlyName（mail）</summary>
        public const string FriendlyNameMail = "mail";

        /// <summary>利用者名の FriendlyName（uid）</summary>
        public const string FriendlyNameUid = "uid";
        #endregion

        #region ADFS / WS-Federation のクレーム（読む側の別名）
        /// <summary>メアド</summary>
        public const string WsFedClaimEmailAddress = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

        /// <summary>名前</summary>
        public const string WsFedClaimName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
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

        /// <summary>XML署名を意味する名前空間</summary>
        /// <remarks>
        /// 値は urn ではなく URL だが、XML の名前空間として UrnProtocol / UrnAssertion / UrnMetadata と並べている。
        /// （署名アルゴリズムの識別子は url の Region に在る）
        /// </remarks>
        public const string UrnDSig = "http://www.w3.org/2000/09/xmldsig#";

        #endregion

        #region bindings
        /// <summary>Binding：HTTP-POST（応答の送り方、メタデータの SingleSignOnService に使う）</summary>
        public const string UrnBindingsPost = UrnHeader20 + "bindings:HTTP-POST";

        /// <summary>Binding：HTTP-Redirect（応答の送り方、メタデータの SingleSignOnService に使う）</summary>
        public const string UrnBindingsRedirect = UrnHeader20 + "bindings:HTTP-Redirect";
        #endregion

        #region nameid-format
        /// <summary>NameIDPolicy要素のFormat属性をunspecifiedに指定する。</summary>
        public const string UrnNameIDFormatUnspecified = UrnHeader11 + "nameid-format:unspecified";

        /// <summary>NameIDPolicy要素のFormat属性をemailAddressに指定する。</summary>
        public const string UrnNameIDFormatEmailAddress = UrnHeader11 + "nameid-format:emailAddress";

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

        /// <summary>RSAwithSHA256（RFC 9231 2.3.2）</summary>
        public const string RSAwithSHA256 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

        /// <summary>RSAwithSHA384（RFC 9231 2.3.3）</summary>
        public const string RSAwithSHA384 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha384";

        /// <summary>RSAwithSHA512（RFC 9231 2.3.4）</summary>
        public const string RSAwithSHA512 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";

        /// <summary>RSA-PSS with SHA256（RFC 9231 2.3.10。ソルト長はハッシュ長）</summary>
        public const string RSAPSSwithSHA256 = "http://www.w3.org/2007/05/xmldsig-more#sha256-rsa-MGF1";

        /// <summary>RSA-PSS with SHA384（RFC 9231 2.3.10。ソルト長はハッシュ長）</summary>
        public const string RSAPSSwithSHA384 = "http://www.w3.org/2007/05/xmldsig-more#sha384-rsa-MGF1";

        /// <summary>RSA-PSS with SHA512（RFC 9231 2.3.10。ソルト長はハッシュ長）</summary>
        public const string RSAPSSwithSHA512 = "http://www.w3.org/2007/05/xmldsig-more#sha512-rsa-MGF1";

        /// <summary>ECDSA with SHA256（RFC 9231 2.3.6）</summary>
        public const string ECDSAwithSHA256 = "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha256";

        /// <summary>ECDSA with SHA384（RFC 9231 2.3.6）</summary>
        public const string ECDSAwithSHA384 = "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha384";

        /// <summary>ECDSA with SHA512（RFC 9231 2.3.6）</summary>
        public const string ECDSAwithSHA512 = "http://www.w3.org/2001/04/xmldsig-more#ecdsa-sha512";
        #endregion
    }
}

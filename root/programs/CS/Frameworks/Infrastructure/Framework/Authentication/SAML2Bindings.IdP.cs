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
//*  2026/10/22  玄人 幸道         メタデータに署名用の証明書を書き足す AppendSigningCertificates を追加（汎用認証サイトから移した。#634）
//*  2026/10/22  玄人 幸道         AppendSigningCertificates に、証明書を設定（CmnServerParams.PreviousSigningCerFilePaths）から読む版を追加
//*  2026/11/14  玄人 幸道         CreateAssertion に、Audience（SP の entityID）を指定する版を追加（受けない版は従来どおり recipient。#693）
//*  2026/11/20  玄人 幸道         CreateResponse・CreateAssertion の値を、文字列の置換ではなく DOM の API で入れるようにした（特殊な文字のエスケープ。inResponseTo が空なら属性ごと省く。#700）
//**********************************************************************************

using System;
using System.Xml;
using System.Collections.Generic;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Xml;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Security;
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
        /// <remarks>
        /// issuer・destination・inResponseTo の値は、文字列の置換ではなく、読み込んだ後に DOM の API で入れる（#700）。
        /// **特殊な文字（引用符・&lt;・&amp;）を含む値も、構造を変えず、エスケープされて入る。**
        /// inResponseTo が null・空なら、InResponseTo 属性ごと省く（SAML 2.0 Core 3.2.2 で任意）。
        /// </remarks>
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

            // - Response固有
            xmlString = xmlString.Replace("{UrnStatusCode}", urnStatusCodeString);

            // 呼び出し側の値（issuer・destination・inResponseTo）は、文字列の置換では入れない（#700）。
            // 特殊な文字（引用符・<・&）で、署名する前の応答の構造が変わるため、読み込んだ後に DOM の API で入れる。
            xmlString = xmlString.Replace("{Issuer}", "");
            xmlString = xmlString.Replace("{Destination}", "");
            xmlString = xmlString.Replace("{InResponseTo}", "");

            // XmlDocument化
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;
            xmlDoc.LoadXml(xmlString);

            // - 呼び出し側の値（DOM の API。特殊な文字もエスケープされる）
            SAML2Bindings.SetElementText(xmlDoc, SAML2Const.ElementIssuer, issuer);
            SAML2Bindings.SetAttributeValue(xmlDoc.DocumentElement, SAML2Const.AttrDestination, destination, false);
            SAML2Bindings.SetAttributeValue(xmlDoc.DocumentElement, SAML2Const.AttrInResponseTo, inResponseTo, true);
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
        /// <remarks>
        /// issuer・nameID・inResponseTo・recipient・Audience の値は、文字列の置換ではなく、読み込んだ後に DOM の API で入れる（#700）。
        /// **特殊な文字（引用符・&lt;・&amp;）を含む値も、構造を変えず、エスケープされて入る。**
        /// inResponseTo が null・空なら、InResponseTo 属性ごと省く（SAML 2.0 Core 3.2.2 で任意）。
        /// </remarks>
        public static XmlDocument CreateAssertion(
            string inResponseTo, string issuer, string nameID,
            SAML2Enum.NameIDFormat nameIDFormat,
            SAML2Enum.AuthnContextClassRef authnContextClassRef,
            double expiresFromSecond, string recipient,
            out string id, RSA rsa = null)
        {
            return SAML2Bindings.CreateAssertionCore(inResponseTo, issuer, nameID, nameIDFormat,
                authnContextClassRef, expiresFromSecond, recipient, null, out id, rsa);
        }

        /// <summary>CreateAssertion（Audience を指定する版）</summary>
        /// <param name="inResponseTo">string</param>
        /// <param name="issuer">string</param>
        /// <param name="nameID">string</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="authnContextClassRef">SAML2Enum.AuthnContextClassRef</param>
        /// <param name="expiresFromSecond">double</param>
        /// <param name="recipient">string（ACS URL。Recipient になる）</param>
        /// <param name="audience">string（Audience に入れる値。SP の entityID。null・空なら、従来どおり recipient）</param>
        /// <param name="id">string</param>
        /// <param name="rsa">RSA</param>
        /// <returns>SAMLAssertion</returns>
        /// <remarks>
        /// 自由に利用できる（#693）。
        /// **Audience には、SP の一意の識別子（entityID）を入れる**（SAML Profiles 4.1.4.2）。
        /// 受けない版は、Audience に recipient（ACS URL）を入れる（従来の振る舞い）。
        /// 値は、XML を組み立てた後に要素の値として入れる（文字列の置換ではないので、特殊な文字もエスケープされる）。
        /// issuer・nameID・inResponseTo・recipient の値も同じ（#700。引用符・&lt;・&amp;を含む値も、構造を変えずに入る）。
        /// inResponseTo が null・空なら、InResponseTo 属性ごと省く（SAML 2.0 Core 3.2.2 で任意）。
        /// </remarks>
        public static XmlDocument CreateAssertion(
            string inResponseTo, string issuer, string nameID,
            SAML2Enum.NameIDFormat nameIDFormat,
            SAML2Enum.AuthnContextClassRef authnContextClassRef,
            double expiresFromSecond, string recipient, string audience,
            out string id, RSA rsa = null)
        {
            return SAML2Bindings.CreateAssertionCore(inResponseTo, issuer, nameID, nameIDFormat,
                authnContextClassRef, expiresFromSecond, recipient, audience, out id, rsa);
        }

        /// <summary>CreateAssertion の本体</summary>
        /// <param name="inResponseTo">string</param>
        /// <param name="issuer">string</param>
        /// <param name="nameID">string</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="authnContextClassRef">SAML2Enum.AuthnContextClassRef</param>
        /// <param name="expiresFromSecond">double</param>
        /// <param name="recipient">string</param>
        /// <param name="audience">string（null・空なら recipient）</param>
        /// <param name="id">string</param>
        /// <param name="rsa">RSA</param>
        /// <returns>SAMLAssertion</returns>
        /// <remarks>
        /// issuer・nameID・inResponseTo・recipient・Audience の値は、文字列の置換ではなく、読み込んだ後に DOM の API で入れる（#700）。
        /// **特殊な文字（引用符・&lt;・&amp;）を含む値も、構造を変えず、エスケープされて入る。**
        /// inResponseTo が null・空なら、InResponseTo 属性ごと省く（SAML 2.0 Core 3.2.2 で任意）。
        /// </remarks>
        private static XmlDocument CreateAssertionCore(
            string inResponseTo, string issuer, string nameID,
            SAML2Enum.NameIDFormat nameIDFormat,
            SAML2Enum.AuthnContextClassRef authnContextClassRef,
            double expiresFromSecond, string recipient, string audience,
            out string id, RSA rsa)
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

            // - 認証関連
            xmlString = xmlString.Replace("{UrnNameIDFormat}", urnNameIDFormatString);
            xmlString = xmlString.Replace("{UrnAuthnContextClassRef}", urnAuthnContextClassRefString);

            // - 時間関連
            string utcNow = FormatConverter.ToW3cTimestamp(DateTime.UtcNow);
            xmlString = xmlString.Replace("{IssueInstant}", utcNow);
            xmlString = xmlString.Replace("{AuthnInstant}", utcNow);
            xmlString = xmlString.Replace("{NotBefore}", utcNow);

            string utcExpires = FormatConverter.ToW3cTimestamp(DateTime.UtcNow.AddSeconds(expiresFromSecond));
            xmlString = xmlString.Replace("{NotOnOrAfter}", utcExpires);

            // 呼び出し側の値（inResponseTo・issuer・nameID・recipient・audience）は、文字列の置換では入れない（#700）。
            // 特殊な文字（引用符・<・&）で、署名する前のアサーションの構造が変わるため、読み込んだ後に DOM の API で入れる。
            xmlString = xmlString.Replace("{InResponseTo}", "");
            xmlString = xmlString.Replace("{Issuer}", "");
            xmlString = xmlString.Replace("{NameID}", "");
            xmlString = xmlString.Replace("{Recipient}", "");
            xmlString = xmlString.Replace("{Audience}", "");

            // XmlDocument化
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = false;
            xmlDoc.LoadXml(xmlString);

            // - 呼び出し側の値（DOM の API。特殊な文字もエスケープされる）。署名の前に入れる
            SAML2Bindings.SetElementText(xmlDoc, SAML2Const.ElementIssuer, issuer);
            SAML2Bindings.SetElementText(xmlDoc, SAML2Const.ElementNameID, nameID);

            XmlNode confirmationData = xmlDoc.GetElementsByTagName(
                SAML2Const.ElementSubjectConfirmationData, SAML2Const.UrnAssertion)[0];
            SAML2Bindings.SetAttributeValue(confirmationData, SAML2Const.AttrInResponseTo, inResponseTo, true);
            SAML2Bindings.SetAttributeValue(confirmationData, SAML2Const.AttrRecipient, recipient, false);

            // - Audience（SP の entityID。SAML Profiles 4.1.4.2。#693）
            //   audience を指定しなければ、従来どおり recipient（recipientのFQDNまでらしい）
            SAML2Bindings.SetElementText(xmlDoc, SAML2Const.ElementAudience,
                string.IsNullOrEmpty(audience) ? recipient : audience);
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

        /// <summary>要素の値として入れる（特殊な文字もエスケープされる。#700）</summary>
        /// <param name="xmlDoc">XmlDocument（SAML の名前空間の要素を 1 つ持つ）</param>
        /// <param name="elementName">string（要素名。saml の名前空間）</param>
        /// <param name="value">string（null は空として扱う）</param>
        private static void SetElementText(XmlDocument xmlDoc, string elementName, string value)
        {
            xmlDoc.GetElementsByTagName(elementName, SAML2Const.UrnAssertion)[0].InnerText = value ?? "";
        }

        /// <summary>属性の値として入れる（特殊な文字もエスケープされる。#700）</summary>
        /// <param name="node">XmlNode（要素）</param>
        /// <param name="attributeName">string</param>
        /// <param name="value">string（null は空として扱う）</param>
        /// <param name="omitIfEmpty">bool（true なら、null・空のとき、属性ごと省く）</param>
        private static void SetAttributeValue(XmlNode node, string attributeName, string value, bool omitIfEmpty)
        {
            XmlElement element = (XmlElement)node;
            if (omitIfEmpty && string.IsNullOrEmpty(value))
            {
                element.RemoveAttribute(attributeName);
            }
            else
            {
                element.SetAttribute(attributeName, value ?? "");
            }
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

        /// <summary>メタデータに、署名用の証明書（KeyDescriptor use="signing"）を書き足す（設定を読む版）</summary>
        /// <param name="metadata">XmlDocument（CreateMetadata の戻り）</param>
        /// <returns>書き足した本数</returns>
        /// <remarks>
        /// 自由に利用できる。CmnServerParams.PreviousSigningCerFilePaths（*.cer。空白・カンマ・タブ区切り）を、書いた順に並べる。
        /// 設定が空なら何もしない（0 を返す）。
        ///
        /// 読めないファイルは飛ばす（汎用認証サイトの従来の扱い。退役させる途中の証明書のために、
        /// メタデータそのものが出せなくなる方が困るため）。並べた本数は戻り値で確かめられる。
        /// </remarks>
        public static int AppendSigningCertificates(XmlDocument metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            string paths = CmnServerParams.PreviousSigningCerFilePaths;

            if (string.IsNullOrEmpty(paths))
            {
                return 0;
            }

            List<string> certificates = new List<string>();

            foreach (string path in paths.Split(
                new char[] { ' ', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    certificates.Add(PrivacyEnhancedMail.GetBase64StringFromPemFilePath(
                        path, PrivacyEnhancedMail.RFC7468Label.Certificate));
                }
                catch (Exception)
                {
                    // 読めないものは飛ばす（remarks）。
                }
            }

            return SAML2Bindings.AppendSigningCertificates(metadata, certificates);
        }

        /// <summary>メタデータに、署名用の証明書（KeyDescriptor use="signing"）を書き足す</summary>
        /// <param name="metadata">XmlDocument（CreateMetadata の戻り）</param>
        /// <param name="certificates">証明書（base64 の DER）の並び。空のものは飛ばす</param>
        /// <returns>書き足した本数</returns>
        /// <remarks>
        /// 自由に利用できる。SAML2Metadata.GetSigningKeys（複数の署名用の証明書を読む）の対である
        /// （汎用認証サイトから移した。#634）。
        ///
        /// CreateMetadata は証明書を 1 本しか受けない。鍵を入れ替える（ロールオーバーする）とき、
        /// 1 本しか広告していないと、メタデータをキャッシュしている SP が、入れ替えた瞬間に検証できなくなる。
        /// そこで、新旧の証明書を並べて広告する。
        ///
        /// 1 本目（CreateMetadata で入れた、いまの鍵）の KeyDescriptor を複製し、証明書だけを差し替えて、
        /// 渡された順に、1 本目の後ろへ並べる。どの証明書を並べるか（ファイルの場所など）は、呼び出し側で決める。
        /// </remarks>
        public static int AppendSigningCertificates(XmlDocument metadata, IEnumerable<string> certificates)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (certificates == null)
            {
                return 0;
            }

            XmlNamespaceManager ns = new XmlNamespaceManager(metadata.NameTable);
            ns.AddNamespace(SAML2Const.PrefixMetadata, SAML2Const.UrnMetadata);
            ns.AddNamespace(SAML2Const.PrefixDSig, SAML2Const.UrnDSig);

            // 1 本目の署名用の KeyDescriptor を複製元にする。
            XmlNode signing = null;

            foreach (XmlNode key in metadata.SelectNodes(SAML2Const.XPathKeyDescriptorInMetadata, ns))
            {
                XmlAttribute use = key.Attributes[SAML2Const.AttrUse];

                if (use != null && use.Value == SAML2Const.KeyUseSigning)
                {
                    signing = key;
                    break;
                }
            }

            if (signing == null || signing.ParentNode == null)
            {
                throw new ArgumentException(PublicExceptionMessage.ARGUMENT_INCORRECT, nameof(metadata));
            }

            int added = 0;
            XmlNode last = signing;

            foreach (string certificate in certificates)
            {
                if (string.IsNullOrEmpty(certificate))
                {
                    continue;
                }

                XmlNode clone = signing.CloneNode(true);
                XmlNode cer = clone.SelectSingleNode(SAML2Const.XPathX509CertificateInKeyDescriptor, ns);
                cer.InnerText = certificate;

                signing.ParentNode.InsertAfter(clone, last);
                last = clone;
                added++;
            }

            return added;
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

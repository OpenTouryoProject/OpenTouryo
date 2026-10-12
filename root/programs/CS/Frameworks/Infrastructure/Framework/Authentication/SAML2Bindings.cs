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
//*  2019/05/29  西野 大介         Create XMLでは署名しない。
//*                                 Encode And Sign / Decode, Verifyで署名・検証する。
//*  2019/06/04  西野 大介         スキーマ検証と属性抽出にはXPathを使用。
//*  2026/10/07  玄人 幸道         VerifyByXPathがStatusCodeを見ずにAssertionを必須にしており、
//*                                Assertionを持たない正当なエラー応答を弾いていたのを修正（#598）
//*  2026/10/12  玄人 幸道         EncodeAndSignRedirectのSigAlgが、実際の署名アルゴリズムに依らず
//*                                rsa-sha1固定になっていたのを修正（#604）
//*                                合わせて、SHA1に限らないため、引数名dsRSAwithSHA1をdsに変更
//*  2026/10/12  玄人 幸道         VerifyRedirectで、SigAlgが渡されたDigitalSignのアルゴリズムと
//*                                一致することを確認するようにした。
//*                                また、SigAlgからハッシュを選ぶ、RSAを受けるオーバーロードを追加（#604）
//**********************************************************************************

using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
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
            id = "s2" + Guid.NewGuid().ToString("N");
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
        /// <param name="inResponseTo">string</param>
        /// <param name="statusCode">SAML2Enum.StatusCode</param>
        /// <param name="id">string</param>
        /// <returns>SAMLResponse</returns>
        public static XmlDocument CreateResponse(
            string issuer, string destination, string inResponseTo,
            SAML2Enum.StatusCode statusCode, out string id)
        {
            // idの先頭は[A-Za-z]のみで、s2とするのが慣例っぽい。
            id = "s2" + Guid.NewGuid().ToString("N");
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
            id = "s2" + Guid.NewGuid().ToString("N");
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
            XmlNode rootNode = xmlDoc.GetElementsByTagName("md:IDPSSODescriptor")[0];
            // NameIDFormat
            foreach (string text in urnNameIDFormatsString)
            {
                newNode = xmlDoc.CreateNode(
                    XmlNodeType.Element,
                    "md", "NameIDFormat",
                    SAML2Const.UrnMetadata);
                // 空のxmlns属性 出力問題の抑止

                newNode.InnerText = text;
                rootNode.AppendChild(newNode);
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
        /// <param name="ds">
        /// 署名に使う DigitalSign（null なら署名しない）。
        /// SigAlg は、この DigitalSign のアルゴリズムから決める（#604）。
        /// </param>
        /// <returns>RedirectBinding用クエリ文字列</returns>
        /// <remarks>
        /// SigAlg（SAML 2.0 Bindings 3.4.4.1）は、実際の署名アルゴリズムを示す。
        /// 2026/10/12 まで rsa-sha1 固定で、既定の SHA256 の署名にも rsa-sha1 と書いていた（#604）。
        /// RSA（PKCS#1 v1.5）の SHA1 / SHA256 / SHA384 / SHA512 と DSA 以外は、従来どおり rsa-sha1 と書く。
        /// </remarks>
        public static string EncodeAndSignRedirect(
            SAML2Enum.RequestOrResponse type,
            string saml, string relayState,
            DigitalSign ds = null)
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
                if (ds == null)
                {
                    // SigAlg, Signatureパラメタなし
                }
                else
                {
                    // 第3 QSパラメタ（実際の署名アルゴリズム。#604）
                    queryStringTemplate += "&SigAlg=" + CustomEncode.UrlEncode(SAML2Bindings.GetSigAlg(ds));
                }                
            }
            #endregion

            #region エンコーディング

            //// エンコーディング オブジェクトの取得
            //Encoding enc = XmlLib.GetEncodingFromXmlDeclaration(saml);

            // XML (→ XML宣言のエンコーディングではなく、asciiエンコーディングに変更)
            // → DEFLATE圧縮 → Base64エンコード → URLエンコード
            saml = CustomEncode.UrlEncode(CustomEncode.ToBase64String(
                DeflateCompression.Compress(CustomEncode.StringToByte(saml, CustomEncode.us_ascii))));
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
            if (ds != null)
            {
                // ASCIIエンコード → 署名 → Base64エンコード → URLエンコード →  Signatureパラメタ追加。
                string signature = CustomEncode.UrlEncode(CustomEncode.ToBase64String(
                    ds.Sign(CustomEncode.StringToByte(
                    	queryString, CustomEncode.us_ascii)))); // enc.CodePage))));

                queryString = queryString + "&Signature=" + signature;
            }
            #endregion

            return queryString; 
        }

        /// <summary>DigitalSign のアルゴリズムから、SigAlg の値を決める（#604）</summary>
        /// <param name="ds">DigitalSign</param>
        /// <returns>SigAlg</returns>
        private static string GetSigAlg(DigitalSign ds)
        {
            if (ds.AsymmetricAlgorithm is RSA && ds.Padding == RSASignaturePadding.Pkcs1)
            {
                string hashName = ds.HashAlgorithmName.Name;

                if (hashName == HashAlgorithmName.SHA1.Name) return SAML2Const.RSAwithSHA1;
                if (hashName == HashAlgorithmName.SHA256.Name) return SAML2Const.RSAwithSHA256;
                if (hashName == HashAlgorithmName.SHA384.Name) return SAML2Const.RSAwithSHA384;
                if (hashName == HashAlgorithmName.SHA512.Name) return SAML2Const.RSAwithSHA512;
            }
            else if (ds.AsymmetricAlgorithm is DSA)
            {
                // DigitalSign.Sign の DSA は SHA1
                return SAML2Const.DSAwithSHA1;
            }

            // 上記以外（PSS・ECDSA など）は、従来どおり。
            // VerifyRedirect（DigitalSign 版）も、この値と照合するので、Open棟梁 同士では通る。
            return SAML2Const.RSAwithSHA1;
        }

        /// <summary>EncodeAndSignPost</summary>
        /// <param name="saml">string</param>
        /// <param name="referenceId">string</param>
        /// <param name="rsa">RSA</param>
        /// <returns>RedirectPost用SAML文字列</returns>
        public static string EncodeAndSignPost(string saml, string referenceId = "", RSA rsa = null)
        {
            //// エンコーディング オブジェクトの取得
            //Encoding enc = XmlLib.GetEncodingFromXmlDeclaration(saml);

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

            // XML (→ XML宣言のエンコーディングではなく、asciiエンコーディングに変更) → Base64エンコード
            return CustomEncode.ToBase64String(CustomEncode.StringToByte(
                saml, CustomEncode.us_ascii)); //enc.CodePage));
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

            //// XML宣言部分を取得するために、us_asciiでデコード
            //string tempString = CustomEncode.ByteToString(tempByte, CustomEncode.us_ascii);

            //// エンコーディング オブジェクトの取得
            //Encoding enc = XmlLib.GetEncodingFromXmlDeclaration(tempString);

            return CustomEncode.ByteToString(tempByte, CustomEncode.us_ascii); // enc.CodePage);
        }

        /// <summary>VerifyRedirect</summary>
        /// <param name="queryString">string</param>
        /// <param name="ds">検証に使う DigitalSign</param>
        /// <returns>bool</returns>
        /// <remarks>
        /// 渡された DigitalSign のアルゴリズムで検証する（アルゴリズムは呼び出し側が決める）。
        /// SigAlg が、そのアルゴリズムを示していなければ失敗にする（#604 の案 (2)）。
        /// 2026/10/12 まで SigAlg を見ておらず、以前の EncodeAndSignRedirect はココで失敗する可能性がある。
        /// SigAlg に従って検証するなら、RSA を受けるオーバーロードを使う（#604 の案 (3)）。
        /// </remarks>
        public static bool VerifyRedirect(string queryString, DigitalSign ds)
        {
            // EcodeRedirectの逆

            // SigAlgの確認（#604）
            string sigAlg = CustomEncode.UrlDecode(
                StringExtractor.GetParameterFromQueryString("SigAlg", queryString));
            if (sigAlg != SAML2Bindings.GetSigAlg(ds))
            {
                return false;
            }

            // Signatureの抽出
            string signature = StringExtractor.GetParameterFromQueryString("Signature", queryString);
            // Signatureの削除
            queryString = queryString.Replace("&Signature=" + signature, "");

            // queryString : ASCIIデコード
            // signature   : パラメタ → URLデコード →  Base64デコード
            if (ds.Verify(
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

        /// <summary>VerifyRedirect</summary>
        /// <param name="queryString">string</param>
        /// <param name="rsa">検証に使う RSA の公開鍵</param>
        /// <param name="sigAlgs">
        /// 受け入れる SigAlg（SAML2Const.RSAwithSHA1 / 256 / 384 / 512 から選ぶ）。
        /// null なら、RSAwithSHA256 / 384 / 512（SHA1 は受け入れない）。
        /// </param>
        /// <returns>bool</returns>
        /// <remarks>
        /// SigAlg からハッシュを選び、RSA（PKCS#1 v1.5）で検証する（#604 の案 (3)）。
        /// SigAlg は署名の検証前に読む値なので、sigAlgs に無い値なら検証せずに失敗にする。
        /// 相手（IdP / SP）ごとにハッシュが違うときに使う。
        /// </remarks>
        public static bool VerifyRedirect(string queryString, RSA rsa, string[] sigAlgs = null)
        {
            if (sigAlgs == null)
            {
                sigAlgs = new string[] {
                    SAML2Const.RSAwithSHA256, SAML2Const.RSAwithSHA384, SAML2Const.RSAwithSHA512 };
            }

            // SigAlgの確認
            string sigAlg = CustomEncode.UrlDecode(
                StringExtractor.GetParameterFromQueryString("SigAlg", queryString));
            if (Array.IndexOf(sigAlgs, sigAlg) == -1)
            {
                return false;
            }

            HashAlgorithmName han;
            if (sigAlg == SAML2Const.RSAwithSHA1) han = HashAlgorithmName.SHA1;
            else if (sigAlg == SAML2Const.RSAwithSHA256) han = HashAlgorithmName.SHA256;
            else if (sigAlg == SAML2Const.RSAwithSHA384) han = HashAlgorithmName.SHA384;
            else if (sigAlg == SAML2Const.RSAwithSHA512) han = HashAlgorithmName.SHA512;
            else return false; // RSA 以外

            // Signatureの抽出
            string signature = StringExtractor.GetParameterFromQueryString("Signature", queryString);
            // Signatureの削除
            queryString = queryString.Replace("&Signature=" + signature, "");

            // queryString : ASCIIデコード
            // signature   : パラメタ → URLデコード →  Base64デコード
            return rsa.VerifyData(
                CustomEncode.StringToByte(queryString, CustomEncode.us_ascii),
                CustomEncode.FromBase64String(CustomEncode.UrlDecode(signature)),
                han, RSASignaturePadding.Pkcs1);
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

            //// XML宣言部分を取得するために、us_asciiでデコード
            //string tempString = CustomEncode.ByteToString(tempByte, CustomEncode.us_ascii);

            //// エンコーディング オブジェクトの取得
            //Encoding enc = XmlLib.GetEncodingFromXmlDeclaration(tempString);

            return CustomEncode.ByteToString(tempByte, CustomEncode.us_ascii); // enc.CodePage);
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
        /// <remarks>
        /// Response の Assertion は、StatusCode が Success のときだけ必須（#598）。
        /// SAML 2.0 Core 3.2.2 / 4.1.4.2 のとおり、エラー応答は Status だけで正当である。
        ///
        /// そのため、Response で true が返っても Assertion があるとは限らない。
        /// Assertion を読む前に、StatusCode が Success であることを確かめること。
        /// </remarks>
        public static bool VerifyByXPath(
            XmlDocument saml, SAML2Enum.SamlSchema schema, XmlNamespaceManager samlNsMgr)
        {
            bool result = false;
            XmlNodeList xmlNodeList = null;

            // Nodeの全体構造を検証（属性はチェックしない）。
            // ※ なお、要素・属性取得時も、XPathで構造を指定している。
            switch (schema)
            {
                case SAML2Enum.SamlSchema.Request:

                    // Request
                    xmlNodeList = saml.SelectNodes(
                        SAML2Const.XPathRequest, samlNsMgr);
                    if (xmlNodeList != null && xmlNodeList.Count == 1)
                    {
                        // Issuer
                        xmlNodeList = saml.SelectNodes(
                            SAML2Const.XPathIssuerInRequest, samlNsMgr);
                        if (xmlNodeList != null && xmlNodeList.Count == 1)
                        {
                            // NameIDPolicy
                            xmlNodeList = saml.SelectNodes(
                                SAML2Const.XPathNameIDPolicyInRequest, samlNsMgr);
                            if (xmlNodeList != null && xmlNodeList.Count == 1)
                            {
                                result = true;
                            }
                        }
                    }

                    break;

                case SAML2Enum.SamlSchema.Response:

                    bool interimReport = false;
                    // Response
                    xmlNodeList = saml.SelectNodes(
                        SAML2Const.XPathResponse, samlNsMgr);
                    if (xmlNodeList != null && xmlNodeList.Count == 1)
                    {
                        // Issuer
                        xmlNodeList = saml.SelectNodes(
                            SAML2Const.XPathIssuerInResponse, samlNsMgr);
                        if (xmlNodeList != null && xmlNodeList.Count == 1)
                        {
                            // Status
                            xmlNodeList = saml.SelectNodes(
                                SAML2Const.XPathStatusCodeInResponse, samlNsMgr);
                            if (xmlNodeList != null && xmlNodeList.Count == 1)
                            {
                                // Assertion
                                xmlNodeList = saml.SelectNodes(
                                    SAML2Const.XPathAssertionInResponse, samlNsMgr);
                                if (xmlNodeList != null && xmlNodeList.Count == 1)
                                {
                                    interimReport = true;
                                }
                                else if (xmlNodeList != null && xmlNodeList.Count == 0)
                                {
                                    // エラー応答（StatusCode が Success 以外）は、
                                    // Assertion を持たないのが正当（#598）。
                                    if (SAML2Bindings.GetStatusCodeInResponse(saml, samlNsMgr)
                                        != SAML2Const.UrnStatusCodeSuccess)
                                    {
                                        result = true;
                                    }
                                }
                            }
                        }
                    }

                    // Assertion
                    if (interimReport)
                    {
                        // Issuer
                        xmlNodeList = saml.SelectNodes(
                            SAML2Const.XPathIssuerInAssertion, samlNsMgr);
                        if (xmlNodeList != null && xmlNodeList.Count == 1)
                        {
                            // NameID
                            xmlNodeList = saml.SelectNodes(
                            SAML2Const.XPathNameIDInAssertion, samlNsMgr);
                            if (xmlNodeList != null && xmlNodeList.Count == 1)
                            {
                                // SubjectConfirmationData
                                xmlNodeList = saml.SelectNodes(
                                    SAML2Const.XPathSubjectConfirmationDataInAssertion, samlNsMgr);
                                if (xmlNodeList != null && xmlNodeList.Count == 1)
                                {
                                    // Audience
                                    xmlNodeList = saml.SelectNodes(
                                        SAML2Const.XPathAudienceInAssertion, samlNsMgr);
                                    if (xmlNodeList != null && xmlNodeList.Count == 1)
                                    {
                                        // AuthnContextClassRef
                                        xmlNodeList = saml.SelectNodes(
                                        SAML2Const.XPathAuthnContextClassRefInAssertion, samlNsMgr);
                                        if (xmlNodeList != null && xmlNodeList.Count == 1)
                                        {
                                            result = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                //case SAML2Enum.SamlSchema.Assertion:
                //    //...
                //    break;
            }

            return result;
        }
        #endregion

        #region VerifyByXsd
        /// <summary>VerifyByXsd</summary>
        /// <param name="saml">string</param>
        /// <param name="schema">SAML2Enum.SamlSchema</param>
        /// <returns>bool</returns>
        /// <remarks>
        /// SAMLのXSDが作成できていないため書きかけ。
        /// - 現時点では、VerifyByXPath メソッドを使って XPath により検証しています。
        /// - XSD検証の方が明らかに良い場合は移行を検討します。
        /// </remarks>
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
                    embeddedXsdFileName = "XXXX.xsd"; // RequestのSAML用のXSD
                    targetNamespace = "urn:oasis:names:tc:SAML:2.0:...";
                    break;

                case SAML2Enum.SamlSchema.Response:
                    embeddedXsdFileName = "XXXX.xsd"; // ResponseのSAML用のXSD
                    targetNamespace = "urn:oasis:names:tc:SAML:2.0:...";
                    break;

                //case SAML2Enum.SamlSchema.Assertion:
                    //    embeddedXsdFileName = "XXXX.xsd";
                    //    targetNamespace = "urn:oasis:names:tc:SAML:2.0:...";
                    //    break;
            }

            // 以下の関数は適切に動作するが、XSDに問題があるため動作しない。
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
                xmlDoc, SAML2Const.XPathNameIDPolicyInRequest, "Format", samlNsMgr);
        }

        /// <summary>GetProtocolBindingInRequest</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>ProtocolBinding</returns>
        public static string GetProtocolBindingInRequest(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, SAML2Const.XPathRequest, "ProtocolBinding", samlNsMgr);
        }

        /// <summary>GetAssertionConsumerServiceURLInRequest</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>AssertionConsumerServiceURL</returns>
        public static string GetAssertionConsumerServiceURLInRequest(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, SAML2Const.XPathRequest, "AssertionConsumerServiceURL", samlNsMgr);
        }
        #endregion

        #region Response
        /// <summary>GetInResponseToInResponse</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>InResponseTo</returns>
        public static string GetInResponseToInResponse(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeFromXmlNode(
                xmlDoc.SelectSingleNode(SAML2Const.XPathResponse, samlNsMgr), "InResponseTo");
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
                xmlDoc, SAML2Const.XPathStatusCodeInResponse, "Value", samlNsMgr);
        }
        #endregion

        #region Assertion
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

        #region Subject
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
                format = XmlLib.GetAttributeFromXmlNode(nameID, "Format");
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
                xmlDoc, SAML2Const.XPathSubjectConfirmationInAssertion, "Method", samlNsMgr);
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
                if (subjectConfirmationData.Attributes["InResponseTo"] != null)
                {
                    inResponseTo = subjectConfirmationData.Attributes["InResponseTo"].Value;
                }

                if (subjectConfirmationData.Attributes["NotOnOrAfter"] != null)
                {
                    notOnOrAfter = subjectConfirmationData.Attributes["NotOnOrAfter"].Value;
                }

                if (subjectConfirmationData.Attributes["Recipient"] != null)
                {
                    recipient = subjectConfirmationData.Attributes["Recipient"].Value;
                }
            }

            return subjectConfirmationData;
        }
        #endregion

        #region Conditions
        /// <summary>GetConditionsNotOnOrAfterInAssertion</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Conditions - NotOnOrAfter</returns>
        public static string GetConditionsNotOnOrAfterInAssertion(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, SAML2Const.XPathConditionsInAssertion, "NotOnOrAfter", samlNsMgr);
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
        #endregion

        #region AuthnStatement
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

        #endregion

        #endregion

        #region ID取得用
        /// <summary>GetIdInRequest</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Id</returns>
        public static string GetIdInRequest(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, @"/samlp:AuthnRequest", "ID", samlNsMgr);
        }

        /// <summary>GetIdInAssertion</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Id</returns>
        public static string GetIdInAssertion(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
        {
            return XmlLib.GetAttributeByXPath(
                xmlDoc, @"/saml:Assertion", "ID", samlNsMgr);
        }

        /// <summary>GetIdInResponse</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="samlNsMgr">XmlNamespaceManager</param>
        /// <returns>Id</returns>
        public static string GetIdInResponse(
            XmlDocument xmlDoc, XmlNamespaceManager samlNsMgr)
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
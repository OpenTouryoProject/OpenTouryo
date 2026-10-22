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
//* クラス名        ：SAML2Attributes
//* クラス日本語名  ：アサーションの AttributeStatement を読み書きする
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した。#634 の C-2）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Xml;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>アサーションの AttributeStatement を読み書きする</summary>
    /// <remarks>
    /// NameID だけでは、メアドや利用者名を運べない（Persistent の NameID はメアドではない）。
    /// 属性で運ぶ（SAML Core 2.7.3）。
    ///
    /// **出す側（IdP）と読む側（SP）を 1 つの型に置く**（名前が片方だけ変わると気付けない）。
    /// 出す値は、OIDC のクレームに 1 対 1 で対応させている。
    ///
    /// | OIDC | Name | FriendlyName | NameFormat |
    /// |---|---|---|---|
    /// | email | urn:oid:0.9.2342.19200300.100.1.3 | mail | uri |
    /// | preferred_username | urn:oid:0.9.2342.19200300.100.1.1 | uid | uri |
    /// | email_verified | email_verified | － | basic |
    ///
    /// urn:oid:… は X.500/LDAP Attribute Profile であり、他の IdP（Shibboleth / ADFS など）とも合う。
    /// email_verified に当たる標準の属性は無いので、OIDC の名前をそのまま basic で出す。
    ///
    /// **読む側は緩い。** 他の IdP は別名で出してくるので、Name と FriendlyName の両方を鍵にし、別名も見る。
    /// </remarks>
    public class SAML2Attributes
    {
        #region 定数

        /// <summary>FriendlyName : mail</summary>
        public const string FriendlyNameEmail = "mail";

        /// <summary>FriendlyName : uid</summary>
        public const string FriendlyNameUserName = "uid";

        /// <summary>メアドの別名（読む側だけが使う。優先順）</summary>
        private static readonly string[] EmailAliases = new string[]
        {
            SAML2Const.UrnOidMail,
            SAML2Attributes.FriendlyNameEmail,
            "email",
            "emailAddress",
            // ADFS / WS-Federation
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
        };

        /// <summary>利用者名の別名（同上）</summary>
        private static readonly string[] UserNameAliases = new string[]
        {
            SAML2Const.UrnOidUid,
            SAML2Attributes.FriendlyNameUserName,
            "preferred_username",
            "username",
            "displayName",
            // ADFS / WS-Federation
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        };

        /// <summary>メアドが検証済みかの別名（同上）</summary>
        private static readonly string[] EmailVerifiedAliases = new string[]
        {
            OAuth2AndOIDCConst.email_verified,
            "emailVerified",
            "mailVerified"
        };

        #endregion

        #region 出す側（IdP）

        /// <summary>アサーションに、メアド・利用者名の AttributeStatement を足す</summary>
        /// <param name="assertion">XmlDocument（SAML2Bindings.CreateAssertion の戻り）</param>
        /// <param name="email">string（空なら出さない）</param>
        /// <param name="userName">string（空なら出さない）</param>
        /// <param name="emailVerified">bool（メアドを出すときだけ、email_verified として出す）</param>
        /// <returns>足したなら true（どちらも空なら何もしない）</returns>
        /// <remarks>自由に利用できる。署名の前に呼ぶこと（後から足すと署名が合わなくなる）。</remarks>
        public static bool AppendStatement(XmlDocument assertion, string email, string userName, bool emailVerified)
        {
            List<Tuple<string, string, string, string>> attributes = new List<Tuple<string, string, string, string>>();

            if (!string.IsNullOrEmpty(email))
            {
                attributes.Add(Tuple.Create(SAML2Const.UrnOidMail,
                    SAML2Attributes.FriendlyNameEmail, SAML2Const.UrnAttrNameFormatUri, email));

                attributes.Add(Tuple.Create(OAuth2AndOIDCConst.email_verified,
                    "", SAML2Const.UrnAttrNameFormatBasic, emailVerified ? "true" : "false"));
            }

            if (!string.IsNullOrEmpty(userName))
            {
                attributes.Add(Tuple.Create(SAML2Const.UrnOidUid,
                    SAML2Attributes.FriendlyNameUserName, SAML2Const.UrnAttrNameFormatUri, userName));
            }

            return SAML2Attributes.AppendStatement(assertion, attributes);
        }

        /// <summary>アサーションに、任意の属性の AttributeStatement を足す</summary>
        /// <param name="assertion">XmlDocument（SAML2Bindings.CreateAssertion の戻り）</param>
        /// <param name="attributes">
        /// 属性の並び（Name, FriendlyName, NameFormat, 値）。
        /// FriendlyName が空なら付けない。値が空の属性は出さない。
        /// </param>
        /// <returns>足したなら true（出す属性が無ければ何もしない）</returns>
        /// <remarks>
        /// 自由に利用できる。
        /// - **署名の前に呼ぶこと。** 署名は応答の全体に対して行う（SAML2Bindings.EncodeAndSign*）
        /// - アサーションの末尾（AuthnStatement の後ろ）に足す。Conditions の後の文は順不同である
        /// - 値は DOM で入れる（&amp; や &lt; を含んでも壊れない）
        /// - 空の AttributeStatement は出さない（SAML Core 2.7.3 は Attribute を 1 つ以上求める）
        /// </remarks>
        public static bool AppendStatement(
            XmlDocument assertion, IEnumerable<Tuple<string, string, string, string>> attributes)
        {
            if (assertion == null)
            {
                throw new ArgumentNullException(nameof(assertion));
            }

            if (attributes == null)
            {
                return false;
            }

            XmlNodeList nodes = assertion.GetElementsByTagName("Assertion", SAML2Const.UrnAssertion);

            if (nodes.Count == 0)
            {
                return false;
            }

            XmlNode root = nodes[0];
            string prefix = string.IsNullOrEmpty(root.Prefix) ? "saml" : root.Prefix;

            XmlElement statement = assertion.CreateElement(prefix, "AttributeStatement", SAML2Const.UrnAssertion);

            foreach (Tuple<string, string, string, string> a in attributes)
            {
                if (string.IsNullOrEmpty(a.Item1) || string.IsNullOrEmpty(a.Item4))
                {
                    continue;
                }

                XmlElement attribute = assertion.CreateElement(prefix, "Attribute", SAML2Const.UrnAssertion);
                attribute.SetAttribute("Name", a.Item1);

                if (!string.IsNullOrEmpty(a.Item3))
                {
                    attribute.SetAttribute("NameFormat", a.Item3);
                }

                if (!string.IsNullOrEmpty(a.Item2))
                {
                    attribute.SetAttribute("FriendlyName", a.Item2);
                }

                XmlElement value = assertion.CreateElement(prefix, "AttributeValue", SAML2Const.UrnAssertion);
                value.InnerText = a.Item4;
                attribute.AppendChild(value);

                statement.AppendChild(attribute);
            }

            if (!statement.HasChildNodes)
            {
                return false;
            }

            root.AppendChild(statement);
            return true;
        }

        #endregion

        #region 読む側（SP）

        /// <summary>応答のアサーションから、属性を読む</summary>
        /// <param name="samlResponse">XmlDocument（応答）</param>
        /// <param name="samlNsMgr">XmlNamespaceManager（SAML2Bindings.CreateNamespaceManager）</param>
        /// <returns>
        /// 属性（Name と FriendlyName の両方を鍵にする。大文字小文字を区別しない。値は最初の AttributeValue）。
        /// 1 つも無ければ空の辞書（null は返さない）。
        /// </returns>
        /// <remarks>
        /// 自由に利用できる。
        /// **署名を検証した応答から読むこと**（SAML2Client.VerifyResponseWithReason は、検証できたときだけ読む）。
        /// 複数の値を持つ属性は、最初の 1 つだけを採る。
        /// </remarks>
        public static Dictionary<string, string> Read(XmlDocument samlResponse, XmlNamespaceManager samlNsMgr)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (samlResponse == null || samlNsMgr == null)
            {
                return ret;
            }

            XmlNodeList attributes = samlResponse.SelectNodes(SAML2Const.XPathAttributeInAssertion, samlNsMgr);

            if (attributes == null)
            {
                return ret;
            }

            foreach (XmlNode attribute in attributes)
            {
                XmlNode value = attribute.SelectSingleNode("saml:AttributeValue", samlNsMgr);

                if (value == null)
                {
                    continue;
                }

                foreach (string key in new string[]
                {
                    SAML2Attributes.Attr(attribute, "Name"),
                    SAML2Attributes.Attr(attribute, "FriendlyName")
                })
                {
                    if (!string.IsNullOrEmpty(key) && !ret.ContainsKey(key))
                    {
                        ret.Add(key, value.InnerText);
                    }
                }
            }

            return ret;
        }

        /// <summary>属性から、メアドを引く</summary>
        /// <param name="attributes">IDictionary（Read の戻り）</param>
        /// <returns>メアド（無ければ空文字）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string GetEmail(IDictionary<string, string> attributes)
        {
            return SAML2Attributes.Pick(attributes, SAML2Attributes.EmailAliases);
        }

        /// <summary>属性から、利用者名を引く</summary>
        /// <param name="attributes">IDictionary（Read の戻り）</param>
        /// <returns>利用者名（無ければ空文字）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string GetUserName(IDictionary<string, string> attributes)
        {
            return SAML2Attributes.Pick(attributes, SAML2Attributes.UserNameAliases);
        }

        /// <summary>属性から、メアドが検証済みかを引く</summary>
        /// <param name="attributes">IDictionary（Read の戻り）</param>
        /// <returns>相手が言った値（"true" / "false" など）。言っていなければ空文字</returns>
        /// <remarks>自由に利用できる。言っていなければ、呼び出し側の従来の判断に倒すこと。</remarks>
        public static string GetEmailVerified(IDictionary<string, string> attributes)
        {
            return SAML2Attributes.Pick(attributes, SAML2Attributes.EmailVerifiedAliases);
        }

        #endregion

        #region private

        /// <summary>別名の並びから、最初に見つかった値を返す</summary>
        /// <param name="attributes">IDictionary</param>
        /// <param name="aliases">string[]（優先順）</param>
        /// <returns>値（無ければ空文字）</returns>
        private static string Pick(IDictionary<string, string> attributes, string[] aliases)
        {
            if (attributes == null)
            {
                return "";
            }

            foreach (string alias in aliases)
            {
                if (attributes.TryGetValue(alias, out string value) && !string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            return "";
        }

        /// <summary>属性値を読む（無ければ空文字）</summary>
        /// <param name="node">XmlNode</param>
        /// <param name="name">string</param>
        /// <returns>値</returns>
        private static string Attr(XmlNode node, string name)
        {
            if (node == null || node.Attributes == null)
            {
                return "";
            }

            XmlNode attr = node.Attributes[name];
            return (attr == null) ? "" : attr.Value;
        }

        #endregion
    }
}

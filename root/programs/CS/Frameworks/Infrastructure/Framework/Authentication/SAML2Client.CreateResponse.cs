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
//* クラス名        ：SAML2Client（CreateResponse）
//* クラス日本語名  ：SAML2Client（ライブラリ）の応答の作成（IdP 側。IdP → SP）
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（汎用認証サイトから移した。#634）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Xml;
using System.Security.Cryptography;

using Touryo.Infrastructure.Public.Security;

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2Client（ライブラリ）の応答の作成（IdP 側。IdP → SP）</summary>
    /// <remarks>
    /// SP 側の要求の作成（CreateRedirectRequest / CreatePostRequest）と対になる処理なので、
    /// IdP 側の処理だが、例外として同じクラスに置いている。
    ///
    /// 配備の決めは、引数で受ける（呼び出し側で持つ）。
    ///
    /// | 決め | 呼び出し側で行うこと |
    /// |---|---|
    /// | 応答を返す先（ACS URL） | **事前登録の値を使う。** 要求に書かれた値は、登録値との照合にだけ使う（SAML Core 3.2.1） |
    /// | NameID | 要求の NameIDPolicy の Format に合わせて決める（PPID など） |
    /// | 属性 | 何を出すか（SAML2Attributes.AppendStatement の形で渡す） |
    /// | Binding | 要求の ProtocolBinding に合わせて、どちらの版を呼ぶかを決める |
    /// | 署名の鍵 | IdP 自身の秘密鍵 |
    /// </remarks>
    public partial class SAML2Client
    {
        #region 引数（鍵）で受ける版

        /// <summary>CreateRedirectResponse（HTTP-Redirect で返す応答を作る）</summary>
        /// <param name="issuer">string（IdP の entityID）</param>
        /// <param name="acsUrl">string（応答を返す先。事前登録の ACS URL。Destination・Recipient・Audience になる）</param>
        /// <param name="inResponseTo">string（要求の ID）</param>
        /// <param name="statusCode">SAML2Enum.StatusCode（Success 以外なら Assertion を組み込まない）</param>
        /// <param name="nameId">string（Assertion の NameID）</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="authnContextClassRef">SAML2Enum.AuthnContextClassRef</param>
        /// <param name="expiresInSeconds">double（Assertion の有効期間（秒））</param>
        /// <param name="attributes">
        /// 属性の並び（Name, FriendlyName, NameFormat, 値。SAML2Attributes.AppendStatement と同じ形）。null なら出さない
        /// </param>
        /// <param name="relayState">string（要求の RelayState をそのまま返す）</param>
        /// <param name="digitalSign">DigitalSign（IdP の秘密鍵。SigAlg もこのアルゴリズムから決まる。null は受けない）</param>
        /// <returns>queryString（SAMLResponse・RelayState・SigAlg・Signature）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string CreateRedirectResponse(
            string issuer, string acsUrl, string inResponseTo, SAML2Enum.StatusCode statusCode,
            string nameId, SAML2Enum.NameIDFormat nameIDFormat, SAML2Enum.AuthnContextClassRef authnContextClassRef,
            double expiresInSeconds, IEnumerable<Tuple<string, string, string, string>> attributes,
            string relayState, DigitalSign digitalSign)
        {
            if (digitalSign == null)
            {
                throw new ArgumentNullException(nameof(digitalSign));
            }

            string id;
            XmlDocument response = SAML2Client.BuildResponse(issuer, acsUrl, inResponseTo, statusCode,
                nameId, nameIDFormat, authnContextClassRef, expiresInSeconds, attributes, out id);

            return SAML2Bindings.EncodeAndSignRedirect(
                SAML2Enum.RequestOrResponse.Response, response.OuterXml, relayState, digitalSign);
        }

        /// <summary>CreatePostResponse（HTTP-POST で返す応答を作る）</summary>
        /// <param name="issuer">string（IdP の entityID）</param>
        /// <param name="acsUrl">string（応答を返す先。事前登録の ACS URL）</param>
        /// <param name="inResponseTo">string（要求の ID）</param>
        /// <param name="statusCode">SAML2Enum.StatusCode（Success 以外なら Assertion を組み込まない）</param>
        /// <param name="nameId">string（Assertion の NameID）</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="authnContextClassRef">SAML2Enum.AuthnContextClassRef</param>
        /// <param name="expiresInSeconds">double（Assertion の有効期間（秒））</param>
        /// <param name="attributes">属性の並び（SAML2Attributes.AppendStatement と同じ形）。null なら出さない</param>
        /// <param name="rsa">RSA（IdP の秘密鍵。null は受けない）</param>
        /// <returns>SAMLResponse（XML 署名して Base64 にしたもの。RelayState は呼び出し側でフォームに載せる）</returns>
        /// <remarks>自由に利用できる。</remarks>
        public static string CreatePostResponse(
            string issuer, string acsUrl, string inResponseTo, SAML2Enum.StatusCode statusCode,
            string nameId, SAML2Enum.NameIDFormat nameIDFormat, SAML2Enum.AuthnContextClassRef authnContextClassRef,
            double expiresInSeconds, IEnumerable<Tuple<string, string, string, string>> attributes,
            RSA rsa)
        {
            if (rsa == null)
            {
                throw new ArgumentNullException(nameof(rsa));
            }

            string id;
            XmlDocument response = SAML2Client.BuildResponse(issuer, acsUrl, inResponseTo, statusCode,
                nameId, nameIDFormat, authnContextClassRef, expiresInSeconds, attributes, out id);

            return SAML2Bindings.EncodeAndSignPost(response.OuterXml, id, rsa);
        }

        #endregion

        #region private

        /// <summary>応答（署名前）を組み立てる</summary>
        /// <param name="issuer">string</param>
        /// <param name="acsUrl">string</param>
        /// <param name="inResponseTo">string</param>
        /// <param name="statusCode">SAML2Enum.StatusCode</param>
        /// <param name="nameId">string</param>
        /// <param name="nameIDFormat">SAML2Enum.NameIDFormat</param>
        /// <param name="authnContextClassRef">SAML2Enum.AuthnContextClassRef</param>
        /// <param name="expiresInSeconds">double</param>
        /// <param name="attributes">IEnumerable</param>
        /// <param name="id">out string（Response の ID）</param>
        /// <returns>XmlDocument（Response）</returns>
        /// <remarks>
        /// Assertion は、成功のときだけ組み込む（エラー応答は Status だけで正当。SAML Core 3.2.2）。
        /// 属性は、署名の前に Assertion へ足す（後から足すと署名が合わなくなる）。
        /// </remarks>
        private static XmlDocument BuildResponse(
            string issuer, string acsUrl, string inResponseTo, SAML2Enum.StatusCode statusCode,
            string nameId, SAML2Enum.NameIDFormat nameIDFormat, SAML2Enum.AuthnContextClassRef authnContextClassRef,
            double expiresInSeconds, IEnumerable<Tuple<string, string, string, string>> attributes, out string id)
        {
            if (string.IsNullOrEmpty(issuer))
            {
                throw new ArgumentNullException(nameof(issuer));
            }

            if (string.IsNullOrEmpty(acsUrl))
            {
                throw new ArgumentNullException(nameof(acsUrl));
            }

            XmlDocument response = SAML2Bindings.CreateResponse(issuer, acsUrl, inResponseTo, statusCode, out id);

            if (statusCode == SAML2Enum.StatusCode.Success)
            {
                string assertionId;
                XmlDocument assertion = SAML2Bindings.CreateAssertion(
                    inResponseTo, issuer, nameId, nameIDFormat, authnContextClassRef,
                    expiresInSeconds, acsUrl, out assertionId);

                if (attributes != null)
                {
                    SAML2Attributes.AppendStatement(assertion, attributes);
                }

                response.DocumentElement.AppendChild(response.ImportNode(assertion.DocumentElement, true));
            }

            return response;
        }

        #endregion
    }
}

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
//* クラス名        ：SAML2Client（VerifyRequest）
//* クラス日本語名  ：SAML2Client（ライブラリ）の要求の検証（IdP 側。SP → IdP）
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

namespace Touryo.Infrastructure.Framework.Authentication
{
    /// <summary>SAML2Client（ライブラリ）の要求の検証（IdP 側。SP → IdP）</summary>
    /// <remarks>
    /// SP 側の応答の検証（VerifyResponse）と対になる処理なので、IdP 側の処理だが、例外として同じクラスに置いている。
    /// どの SP の要求を受けるか、鍵をどこから引くか、鍵が無い SP をどう扱うかは、配備の決めなので呼び出し側で持つ。
    /// </remarks>
    public partial class SAML2Client
    {
        #region 引数（鍵）で受ける版

        /// <summary>VerifyRequest（要求の Issuer から、検証鍵を選ぶ）</summary>
        /// <param name="queryString">string（Redirect Binding のときのクエリ文字列。POST なら null か空文字）</param>
        /// <param name="samlRequest">string（POST Binding のときの SAMLRequest）</param>
        /// <param name="getKeys">
        /// 要求の Issuer（SP の entityID）から、その SP の検証鍵（公開鍵）の並びを返す処理。
        /// null か空を返せば、検証できない（false）。
        /// </param>
        /// <param name="iss">out string（要求の Issuer。読めなければ空文字）</param>
        /// <param name="id">out string（要求の ID。応答の InResponseTo に使う。読めなければ空文字）</param>
        /// <param name="samlRequest2">out XmlDocument（要求。読めなければ null）</param>
        /// <param name="sigAlgs">string[]（Redirect で受け入れる SigAlg。null なら SHA256 / 384 / 512）</param>
        /// <returns>検証できれば true</returns>
        /// <remarks>
        /// 自由に利用できる。
        ///
        /// IdP は、要求の Issuer を読んでから、その SP の鍵で検証する。そのため、鍵を「Issuer から返す処理」で受ける。
        /// 鍵が複数なら、署名が通るまで順に試す（SAML2 の署名には kid に当たるものが無い）。
        ///
        /// 検証するのは、署名と XML の構造（VerifyByXPath）まで。
        /// AssertionConsumerServiceURL が登録値と一致するかなど、応答の返し方の判断は、呼び出し側で行う
        /// （要求に書かれた ACS URL をそのまま使うと、任意の URL へアサーションを飛ばせてしまう）。
        ///
        /// 外から来た値なので、読めなくても例外にせず false を返す。
        /// false のときの iss / id は、検証されていない値である（表示・ログの切り分けにだけ使う）。
        /// </remarks>
        public static bool VerifyRequest(
            string queryString, string samlRequest, Func<string, IEnumerable<RSA>> getKeys,
            out string iss, out string id, out XmlDocument samlRequest2, string[] sigAlgs = null)
        {
            if (getKeys == null)
            {
                throw new ArgumentNullException(nameof(getKeys));
            }

            iss = "";
            id = "";
            samlRequest2 = null;

            bool isRedirect = !string.IsNullOrEmpty(queryString);
            string decodeSaml = "";
            XmlNamespaceManager samlNsMgr = null;

            try
            {
                if (isRedirect)
                {
                    decodeSaml = SAML2Bindings.DecodeRedirect(queryString);
                }
                else if (!string.IsNullOrEmpty(samlRequest))
                {
                    decodeSaml = SAML2Bindings.DecodePost(samlRequest);
                }
                else
                {
                    return false;
                }

                samlRequest2 = new XmlDocument();
                samlRequest2.PreserveWhitespace = false;
                samlRequest2.XmlResolver = null;
                samlRequest2.LoadXml(decodeSaml);

                samlNsMgr = SAML2Bindings.CreateNamespaceManager(samlRequest2);

                iss = SAML2Bindings.GetIssuerInRequest(samlRequest2, samlNsMgr) ?? "";
                id = SAML2Bindings.GetIdInRequest(samlRequest2, samlNsMgr) ?? "";
            }
            catch (Exception)
            {
                // 外から来た値なので、例外にしない。
                return false;
            }

            IEnumerable<RSA> keys = getKeys(iss);

            if (keys == null)
            {
                return false;
            }

            foreach (RSA rsa in keys)
            {
                if (rsa == null)
                {
                    continue;
                }

                try
                {
                    bool signed = isRedirect
                        ? SAML2Bindings.VerifyRedirect(queryString, rsa, sigAlgs)
                        : SAML2Bindings.VerifyPost(decodeSaml, id, rsa);

                    if (signed)
                    {
                        return SAML2Bindings.VerifyByXPath(samlRequest2, SAML2Enum.SamlSchema.Request, samlNsMgr);
                    }
                }
                catch (Exception)
                {
                    // この鍵では検証できなかった。次の鍵を試す。
                }
            }

            return false;
        }

        /// <summary>VerifyRequest（検証鍵を固定で渡す）</summary>
        /// <param name="queryString">string（Redirect Binding のときのクエリ文字列。POST なら null か空文字）</param>
        /// <param name="samlRequest">string（POST Binding のときの SAMLRequest）</param>
        /// <param name="rsaKeys">IEnumerable（検証鍵。SP が 1 つの配備向け）</param>
        /// <param name="iss">out string（要求の Issuer）</param>
        /// <param name="id">out string（要求の ID）</param>
        /// <param name="samlRequest2">out XmlDocument（要求）</param>
        /// <param name="sigAlgs">string[]（Redirect で受け入れる SigAlg。null なら SHA256 / 384 / 512）</param>
        /// <returns>検証できれば true</returns>
        /// <remarks>
        /// 自由に利用できる。Issuer に依らず、同じ鍵で検証する。
        /// Issuer が期待する SP かどうかは、呼び出し側で iss を照合すること。
        /// </remarks>
        public static bool VerifyRequest(
            string queryString, string samlRequest, IEnumerable<RSA> rsaKeys,
            out string iss, out string id, out XmlDocument samlRequest2, string[] sigAlgs = null)
        {
            if (rsaKeys == null)
            {
                throw new ArgumentNullException(nameof(rsaKeys));
            }

            return SAML2Client.VerifyRequest(queryString, samlRequest, x => rsaKeys,
                out iss, out id, out samlRequest2, sigAlgs);
        }

        #endregion
    }
}

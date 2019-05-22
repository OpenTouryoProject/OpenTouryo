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
//* クラス名        ：SignedXml2
//* クラス日本語名  ：SignedXml2クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/05/20  西野 大介         新規作成
//**********************************************************************************

using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

using Touryo.Infrastructure.Public.Str;

namespace Touryo.Infrastructure.Public.Security.Xml
{
    /// <summary>
    /// SignedXml2
    /// System.Security.Cryptography.Xml.SignedXmlのラッパ
    /// ネストしたXMLの署名・検証処理の問題の確認用コード。
    /// https://gist.github.com/daisukenishino2/69074e571cf89c23cce6d1522abc67e5
    /// </summary>
    public class SignedXml2
    {
        /// <summary>RSA</summary>
        private RSA _rsa = null;

        /// <summary></summary>
        /// <param name="rsa">RSA</param>
        public SignedXml2(RSA rsa)
        {
            this._rsa = rsa;
        }

        /// <summary>SignedXml生成メソッド</summary>
        /// <param name="xmlString">Xml文字列</param>
        /// <param name="referenceId">署名対象ノードのID値（「#」は含まない）</param>
        /// <param name="preserveWhitespace">SignedXmlの空白・改行を保持する()・しない(false)</param>
        /// <returns>Signed XmlDocument</returns>
        public XmlDocument Create(string xmlString, string referenceId, bool preserveWhitespace = false)
        {
            // - XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            xmlDoc.PreserveWhitespace = preserveWhitespace;

            return this.Create(xmlDoc, referenceId, preserveWhitespace);
        }

        /// <summary>SignedXml生成メソッド</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="referenceId">署名対象ノードのID値（「#」は含まない）</param>
        /// <param name="preserveWhitespace">SignedXmlの空白・改行を保持する()・しない(false)</param>
        /// <returns>Signed XmlDocument</returns>
        public XmlDocument Create(XmlDocument xmlDoc, string referenceId, bool preserveWhitespace = false)
        {
            // - SignedXml
            SignedXml signedXml = new SignedXml(xmlDoc);
            signedXml.SigningKey = this._rsa;

            // Reference要素
            // - 署名対象ノードをポイント
            Reference reference = new Reference("#" + referenceId);
            // - Add Transform
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            // - Add Transform
            reference.AddTransform(new XmlDsigExcC14NTransform());
            // - Add Reference
            signedXml.AddReference(reference);

            // 署名対象ノードのXML署名の生成
            // - 署名の計算
            signedXml.ComputeSignature();
            // - 署名対象ノードのXML署名を
            XmlNode signatureNode = xmlDoc.ImportNode(signedXml.GetXml(), true);
            // - 署名対象ノード直下に追加
            this.GetTargetXmlNode(xmlDoc, referenceId).AppendChild(signatureNode); 

            // Signed XmlDocumentを返す。
            return xmlDoc;
        }

        /// <summary>SignedXml検証メソッド</summary>
        /// <param name="signedXmlString">SignedXml</param>
        /// <param name="referenceId">署名対象ノードのID値（「#」は含まない）</param>
        /// <param name="preserveWhitespace">SignedXmlの空白・改行を保持する()・しない(false)</param>
        /// <returns>署名の検証結果</returns>
        public bool Verify(string signedXmlString, string referenceId, bool preserveWhitespace = false)
        {
            // 初期処理
            // - XmlDocument
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(signedXmlString);
            xmlDoc.PreserveWhitespace = preserveWhitespace;

            // 子ノード のXML検証
            XmlNode targetNode = this.GetTargetXmlNode(xmlDoc, referenceId);

            // 署名ノードの直下のSignatureを取り出して、signedXml.LoadXmlする。
            SignedXml signedXml = new SignedXml(targetNode.OwnerDocument);
            signedXml.LoadXml(targetNode["Signature"] as XmlElement);

            // XML検証
            return signedXml.CheckSignature(this._rsa);
        }

        /// <summary>GetTargetXmlNode</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="referenceId">string</param>
        /// <remarks>
        /// xmlDoc.GetElementById(referenceId).AppendChild(signatureNode); // DTD問題で使えない。
        /// ...と言う事で、SelectSingleNode(XPath)を使用するが、対象ノードがルートかルート以下でXPathが異なる。
        /// 処理対象を必ず、ルートノードに追加するとすれば問題ないのだが、それは利用者にとって不便。
        /// </remarks>
        public XmlNode GetTargetXmlNode(XmlDocument xmlDoc, string referenceId)
        {
            // ルート以下を検索
            XmlNode targetNode = xmlDoc.SelectSingleNode("//" + referenceId);

            if (targetNode == null)
            {
                // ルートを検索（XML宣言 + ルートノード）
                foreach (XmlNode tempNode in xmlDoc.ChildNodes)
                {
                    if (tempNode.Attributes == null) continue; // XML宣言対策
                    foreach (XmlAttribute tempAttr in tempNode.Attributes)
                    {
                        if (tempAttr.Name.ToLower() == "id")
                        {
                            if (tempAttr.Value == referenceId)
                            {
                                targetNode = tempNode;
                            }
                        }
                    }
                }
            }

            return targetNode;
        }
    }
}

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
//*  2026/10/19  玄人 幸道         Verifyが署名の対象（referenceId）を突き合わせておらず、
//*                                署名の付け替えが通りえたのを修正。
//**********************************************************************************

using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

using Touryo.Infrastructure.Public.Xml;

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
            xmlDoc.PreserveWhitespace = preserveWhitespace;
            xmlDoc.LoadXml(xmlString);

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
            XmlLib.GetXmlNodeById(xmlDoc, referenceId).AppendChild(signatureNode); 

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
            xmlDoc.PreserveWhitespace = preserveWhitespace;
            xmlDoc.LoadXml(signedXmlString);

            try
            {
                // 子ノード のXML検証
                XmlNode targetNode = XmlLib.GetXmlNodeById(xmlDoc, referenceId);
                if (targetNode == null) return false;

                // 署名の付け替え（XML Signature Wrapping）対策。
                // ＜なぜ要るか＞
                //   CheckSignature は「Reference（#id）が指す要素が、その鍵で署名されている」ことしか見ない。
                //   どの要素が署名されたか（referenceId）と、呼び出し側が消費する要素が一致することは、
                //   ここで突き合わせないと保証されない。一致を見ないと、別途正当に署名された要素を
                //   残したまま消費側を差し替えても、検証が通ってしまう。
                //
                // referenceId の id を持つ要素が、文書内でちょうど 1 つで、それが targetNode 自身であること。
                //   同じ id が複数あると、CheckSignature は targetNode とは別の要素を検証しうる。
                XmlElement[] byId = XmlLib.GetElementsById(xmlDoc, referenceId);
                if (byId.Length != 1) return false;
                if (!object.ReferenceEquals(byId[0], targetNode)) return false;

                // 署名ノードの直下のSignatureを取り出して、signedXml.LoadXmlする。
                XmlElement signatureElement = targetNode["Signature"] as XmlElement;
                if (signatureElement == null) return false;

                SignedXml signedXml = new SignedXml(xmlDoc);
                signedXml.LoadXml(signatureElement);

                // 署名が、検証対象ノードちょうど 1 つ（#referenceId）を指していること。
                if (signedXml.SignedInfo.References.Count != 1) return false;
                Reference reference = signedXml.SignedInfo.References[0] as Reference;
                if (reference == null || reference.Uri != "#" + referenceId) return false;

                // XML検証
                return signedXml.CheckSignature(this._rsa);
            }
            catch (CryptographicException)
            {
                // 署名が壊れている・解釈できない場合は、検証失敗として扱う（例外を投げない）。
                return false;
            }
        }
    }
}

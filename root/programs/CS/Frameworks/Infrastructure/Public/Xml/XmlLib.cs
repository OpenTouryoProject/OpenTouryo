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
//* クラス名        ：XmlLib
//* クラス日本語名  ：XmlLib
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/05/29  西野 大介         新規作成（分割
//**********************************************************************************

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Text;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Xml
{
    /// <summary>XmlLib</summary>
    /// <remarks>自由に利用できる。</remarks>
    public static class XmlLib
    {
        #region 検索

        /// <summary>GetAttributeByXPath</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="xPath">string</param>
        /// <param name="attrName">string</param>
        /// <param name="xmlnsManager">XmlNamespaceManager</param>
        /// <param name="index">string</param>
        /// <returns>attr string</returns>
        public static string GetAttributeByXPath(
            XmlDocument xmlDoc, string xPath, string attrName,
            XmlNamespaceManager xmlnsManager = null, int index = 0)
        {
            return XmlLib.GetAttributeFromXmlNode(
                xmlDoc.SelectSingleNode(xPath, xmlnsManager), attrName, index);
        }

        /// <summary>GetAttributeByTagName</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="tagName">string</param>
        /// <param name="attrName">string</param>
        /// <param name="index">string</param>
        /// <returns>attr string</returns>
        public static string GetAttributeByTagName(XmlDocument xmlDoc, string tagName, string attrName, int index = 0)
        {
            XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName(tagName);

            if (xmlNodeList.Count != 0)
            {
                if (xmlNodeList[0].Attributes != null)
                {
                    if (xmlNodeList[0].Attributes[attrName] != null)
                    {
                        return xmlNodeList[0].Attributes[attrName].Value;
                    }
                }
            }

            return "";
        }

        /// <summary>GetXmlNodeById</summary>
        /// <param name="xmlDoc">XmlDocument</param>
        /// <param name="referenceId">string</param>
        /// <remarks>
        /// xmlDoc.GetElementById(referenceId).AppendChild(signatureNode); // DTD問題で使えない。
        /// ...と言う事で、SelectSingleNode(XPath)を使用するが、対象ノードがルートかルート以下でXPathが異なる。
        /// 処理対象を必ず、ルートノードに追加するとすれば問題ないのだが、それは利用者にとって不便。
        /// </remarks>
        public static XmlNode GetXmlNodeById(XmlDocument xmlDoc, string referenceId)
        {
            // ルート以下を検索
            XmlNode targetNode = xmlDoc.SelectSingleNode("//" + referenceId);

            // ルートを検索（XML宣言 + ルートノード）
            if (targetNode == null)
            {   
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
        #endregion

        #region　値取得
        /// <summary>GetAttributeFromXmlNode</summary>
        /// <param name="xmlNode">XmlNode</param>
        /// <param name="attrName">string</param>
        /// <param name="index">int</param>
        /// <returns>attrValue</returns>
        public static string GetAttributeFromXmlNode(
            XmlNode xmlNode, string attrName, int index = 0)
        {
            if (xmlNode != null)
            {
                if (xmlNode.Attributes != null)
                {
                    if (xmlNode.Attributes[attrName] != null)
                    {
                        return xmlNode.Attributes[attrName].Value;
                    }
                }
            }

            return "";
        }

        /// <summary>XML宣言のエンコーディングを返す</summary>
        /// <param name="xmlDeclaration">string</param>
        /// <returns>Encoding</returns>
        public static Encoding GetEncodingFromXmlDeclaration(string xmlDeclaration)
        {
            try
            {
                // エンコーディング オブジェクトに変換
                return Encoding.GetEncoding(
                    StringExtractor.GetAttributeFromXml(xmlDeclaration, "encoding"));
            }
            catch (Exception)
            {
                // ここでエラーとなった場合、
                throw new ArgumentException(String.Format(
                    PublicExceptionMessage.XML_DECLARATION_ERROR, xmlDeclaration));
            }
        }

        #endregion

        #region XmlSchema
        /// <summary>ValidateByEmbeddedXsd</summary>
        /// <param name="xml">string</param>
        /// <param name="assemblyName">string</param>
        /// <param name="embeddedXsdFileName">string</param>
        /// <param name="targetNamespace">string</param>
        /// <returns>bool</returns>
        public static bool ValidateByEmbeddedXsd(string xml, string assemblyName, string embeddedXsdFileName, string targetNamespace)
        {
            #region samlSchemaSet
            Stream stream = EmbeddedResourceLoader.LoadAsStream(
                assemblyName, embeddedXsdFileName);
            stream.Position = 0; // ...コレがいるらしい。
            XmlReader schemaDocument = XmlReader.Create(new StreamReader(stream));
            schemaDocument.Read(); // None対策
            XmlSchemaSet samlSchemaSet = new XmlSchemaSet();

            // 非常に遅くなる（恐らくWeb Access）のでXmlResolverをnullクリアする。
            samlSchemaSet.XmlResolver = null;
            samlSchemaSet.Add(targetNamespace, schemaDocument);
            #endregion

            #region samlDocument
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.ValidationType = ValidationType.Schema;
            xmlReaderSettings.Schemas = samlSchemaSet;

            XmlReader samlDocument = XmlReader.Create(
                new StringReader(xml), xmlReaderSettings); // ココでエラー
            // System.Xml.Schema.XmlSchemaValidationException: 型
            // 'urn:oasis:names:tc:SAML:2.0:assertion:EncryptedElementType' は宣言されていません。

            #endregion

            try
            {
                while (samlDocument.Read())
                {
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}

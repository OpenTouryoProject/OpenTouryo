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
//* クラス名        ：XmlLibTests
//* クラス日本語名  ：XmlLib の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

using System;
using System.Text;
using System.Xml;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Xml;

namespace TestPublic.Xml
{
    /// <summary>XmlLib の単体テスト</summary>
    [TestFixture]
    public class XmlLibTests
    {
        /// <summary>検証用の XML（名前空間なし）</summary>
        private const string Xml =
            "<root id=\"r1\">" +
            "<item id=\"i1\" name=\"first\" />" +
            "<item id=\"i2\" name=\"second\" />" +
            "</root>";

        /// <summary>OpenTouryo.Public に埋め込まれている XSD と、それに沿う XML</summary>
        private const string PublicAssembly = "OpenTouryo.Public";
        private const string EmbeddedXsd = "Touryo.Infrastructure.Public.Xml.TestXsd.xsd";
        private const string BookstoreNs = "urn:bookstore-schema";

        private static XmlDocument Load()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Xml);
            return doc;
        }

        #region ノードと属性

        /// <summary>XPath・タグ名で、index 番目のノードの属性を取る。</summary>
        [Test]
        public void GetAttributeByXPathAndTagName()
        {
            XmlDocument doc = Load();

            Assert.That(XmlLib.GetAttributeByXPath(doc, "/root/item", "name"), Is.EqualTo("first"));
            Assert.That(XmlLib.GetAttributeByXPath(doc, "/root/item", "name", null, 1), Is.EqualTo("second"));
            Assert.That(XmlLib.GetAttributeByTagName(doc, "item", "id", 1), Is.EqualTo("i2"));
        }

        /// <summary>**見つからないときは例外にせず空文字**（範囲外の index、無い属性、無いノード）。</summary>
        [Test]
        public void NotFound_ReturnsEmpty()
        {
            XmlDocument doc = Load();

            Assert.That(XmlLib.GetAttributeByXPath(doc, "/root/item", "name", null, 2), Is.EqualTo(""));
            Assert.That(XmlLib.GetAttributeByXPath(doc, "/root/item", "name", null, -1), Is.EqualTo(""));
            Assert.That(XmlLib.GetAttributeByTagName(doc, "item", "none"), Is.EqualTo(""));
            Assert.That(XmlLib.GetAttributeByTagName(doc, "none", "id"), Is.EqualTo(""));
            Assert.That(XmlLib.GetAttributeFromXmlNode(null, "id"), Is.EqualTo(""));
        }

        /// <summary>名前空間付きの XML は XmlNamespaceManager を渡して引く。</summary>
        [Test]
        public void GetAttributeByXPath_WithNamespace()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<b:bookstore xmlns:b=\"" + BookstoreNs + "\"><b:book genre=\"novel\" /></b:bookstore>");

            XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("x", BookstoreNs);

            Assert.That(XmlLib.GetAttributeByXPath(doc, "/x:bookstore/x:book", "genre", ns), Is.EqualTo("novel"));
        }

        /// <summary>
        /// **GetXmlNodeById の引数は、まず「ノード名」として使われる。**
        /// //＜引数＞ の XPath で当たらなければ、ルート要素の id 属性を見る。
        /// 子要素の id 属性は見ない。
        /// </summary>
        [Test]
        public void GetXmlNodeById()
        {
            XmlDocument doc = Load();

            Assert.That(XmlLib.GetXmlNodeById(doc, "item").Name, Is.EqualTo("item"));
            Assert.That(XmlLib.GetXmlNodeById(doc, "r1").Name, Is.EqualTo("root"));
            Assert.That(XmlLib.GetXmlNodeById(doc, "i1"), Is.Null);
        }

        #endregion

        #region XML 宣言

        /// <summary>XML 宣言の encoding から Encoding を得る。</summary>
        [Test]
        public void GetEncodingFromXmlDeclaration()
        {
            Assert.That(XmlLib.GetEncodingFromXmlDeclaration("<?xml version=\"1.0\" encoding=\"utf-8\"?>").WebName,
                Is.EqualTo("utf-8"));
        }

        /// <summary>encoding が無い・解釈できない宣言は ArgumentException。</summary>
        [TestCase("<?xml version=\"1.0\"?>")]
        [TestCase("<?xml version=\"1.0\" encoding=\"no-such-encoding\"?>")]
        public void GetEncodingFromXmlDeclaration_Invalid(string decl)
        {
            Assert.Throws<ArgumentException>(() => XmlLib.GetEncodingFromXmlDeclaration(decl));
        }

        #endregion

        #region XSD 検証

        /// <summary>埋め込みの XSD で検証する。</summary>
        [Test]
        public void ValidateByEmbeddedXsd()
        {
            string valid =
                "<bookstore xmlns=\"" + BookstoreNs + "\"><book genre=\"novel\">" +
                "<title>T</title><author><first-name>F</first-name><last-name>L</last-name></author>" +
                "<price>10.00</price></book></bookstore>";

            // price が decimal でない
            string invalid = valid.Replace("<price>10.00</price>", "<price>abc</price>");

            Assert.That(XmlLib.ValidateByEmbeddedXsd(valid, PublicAssembly, EmbeddedXsd, BookstoreNs), Is.True);
            Assert.That(XmlLib.ValidateByEmbeddedXsd(invalid, PublicAssembly, EmbeddedXsd, BookstoreNs), Is.False);
        }

        #endregion
    }
}

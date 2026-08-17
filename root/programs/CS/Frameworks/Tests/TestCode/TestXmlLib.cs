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
//* クラス名        ：TestXmlLib
//* クラス日本語名  ：XmlLibのテスト
//*
//* 作成者          ：西野 大介
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/05/30  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Xml;

using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Xml;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    public class TestXmlLib
    {
        #region public
        /// <summary>Root</summary>
        public static void Root()
        {
            // Xmlロード
            string xml = EmbeddedResourceLoader.LoadXMLAsString(
                "OpenTouryo.Public", "Touryo.Infrastructure.Public.Xml.TestXml.xml");

            // Xsdによる検証
            if (XmlLib.ValidateByEmbeddedXsd(
                xml, "OpenTouryo.Public", "Touryo.Infrastructure.Public.Xml.TestXsd.xsd", "urn:bookstore-schema"))
            {
                MyDebug.OutputDebugAndConsole("XmlLib", "is working properly.");
            }
            else
            {
                MyDebug.OutputDebugAndConsole("XmlLib", "is not working properly.");
            }

            MyDebug.OutputDebugAndConsole("--------------------------------------------------");

            TestXmlLib.GetAttributeTest();
            TestXmlLib.GetXmlNodeByIdTest();
            TestXmlLib.GetEncodingTest();
        }
        #endregion

        #region private

        /// <summary>テスト用の XML</summary>
        /// <remarks>
        /// **埋め込みの TestXml.xml は名前空間つき**なので、XPath に
        /// XmlNamespaceManager が要る。属性の取得そのものを見たいので、
        /// ここでは名前空間なしの XML を自前で組む。
        /// </remarks>
        private const string Xml =
            "<root id=\"r1\">"
            + "<item id=\"i1\" name=\"一つ目\" />"
            + "<item id=\"i2\" name=\"二つ目\" />"
            + "<empty />"
            + "</root>";

        /// <summary>属性の取得（GetAttributeByTagName、GetAttributeByXPath、GetAttributeFromXmlNode）</summary>
        /// <remarks>
        /// **見つからないときは空文字列を返す。** 例外にはならないので、
        /// 呼ぶ側は「空」と「属性値が空」を区別できない。
        /// </remarks>
        private static void GetAttributeTest()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(TestXmlLib.Xml);

            // タグ名で引く
            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetAttributeByTagName - item/name: [" + XmlLib.GetAttributeByTagName(doc, "item", "name") + "]");

            // **index で何番目かを選べる。**（#563 で実装された）
            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetAttributeByTagName - item/name(index=1): ["
                + XmlLib.GetAttributeByTagName(doc, "item", "name", 1) + "]");

            // **範囲外は空文字列。** 例外にはならない。
            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetAttributeByTagName - item/name(index=9): ["
                + XmlLib.GetAttributeByTagName(doc, "item", "name", 9) + "]");

            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetAttributeByTagName - item/name(index=-1): ["
                + XmlLib.GetAttributeByTagName(doc, "item", "name", -1) + "]");

            // 属性が無い要素
            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetAttributeByTagName - empty/name: [" + XmlLib.GetAttributeByTagName(doc, "empty", "name") + "]");

            // タグが無い
            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetAttributeByTagName - none/name: [" + XmlLib.GetAttributeByTagName(doc, "none", "name") + "]");

            // 属性名が無い
            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetAttributeByTagName - item/none: [" + XmlLib.GetAttributeByTagName(doc, "item", "none") + "]");

            // XPath で引く
            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetAttributeByXPath - //item[@id='i2']/name: ["
                + XmlLib.GetAttributeByXPath(doc, "//item[@id='i2']", "name") + "]");

            // XPath が当たらない
            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetAttributeByXPath - //none: [" + XmlLib.GetAttributeByXPath(doc, "//none", "name") + "]");

            // **複数当たる XPath で index を効かせる。**（#563 で実装された）
            //   index=0 の結果は、SelectSingleNode を使っていた頃と同じであること。
            for (int i = 0; i <= 2; i++)
            {
                MyDebug.OutputDebugAndConsole(
                    "XmlLib.GetAttributeByXPath - //item(index=" + i.ToString() + "): ["
                    + XmlLib.GetAttributeByXPath(doc, "//item", "name", null, i) + "]");
            }

            // XmlNode から直接
            XmlNode node = doc.SelectSingleNode("//item[@id='i1']");
            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetAttributeFromXmlNode - name: [" + XmlLib.GetAttributeFromXmlNode(node, "name") + "]");

            // **null を渡しても例外にならない。**
            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetAttributeFromXmlNode - null: [" + XmlLib.GetAttributeFromXmlNode(null, "name") + "]");
        }

        /// <summary>id によるノードの取得（GetXmlNodeById）</summary>
        /// <remarks>
        /// **2 段構えになっている。** まず「//＜引数＞」で XPath 検索し、
        /// 当たらなければルート要素の id 属性を見に行く。
        /// 引数はノード名として使われるので、id の値ではない点に注意。
        /// </remarks>
        private static void GetXmlNodeByIdTest()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(TestXmlLib.Xml);

            // 1 段目：ノード名で当たる
            XmlNode n1 = XmlLib.GetXmlNodeById(doc, "item");
            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetXmlNodeById - item: "
                + (n1 == null ? "(null)" : n1.Name + " id=" + XmlLib.GetAttributeFromXmlNode(n1, "id")));

            // 2 段目：ノード名では当たらず、ルートの id 属性で当たる
            XmlNode n2 = XmlLib.GetXmlNodeById(doc, "r1");
            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetXmlNodeById - r1: "
                + (n2 == null ? "(null)" : n2.Name + " id=" + XmlLib.GetAttributeFromXmlNode(n2, "id")));

            // どちらでも当たらない
            XmlNode n3 = XmlLib.GetXmlNodeById(doc, "i1");
            MyDebug.OutputDebugAndConsole(
                "XmlLib.GetXmlNodeById - i1: "
                + (n3 == null ? "(null)" : n3.Name + " id=" + XmlLib.GetAttributeFromXmlNode(n3, "id")));
        }

        /// <summary>XML 宣言からのエンコーディング取得（GetEncodingFromXmlDeclaration）</summary>
        private static void GetEncodingTest()
        {
            string[] declarations = new string[]
            {
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>",
                "<?xml version=\"1.0\" encoding=\"shift_jis\" ?>",
                "<?xml version=\"1.0\" ?>",            // encoding が無い
                "<?xml version=\"1.0\" encoding=\"xxx\" ?>"  // 知らないエンコーディング
            };

            foreach (string d in declarations)
            {
                try
                {
                    MyDebug.OutputDebugAndConsole(
                        "XmlLib.GetEncodingFromXmlDeclaration - " + d + " : "
                        + XmlLib.GetEncodingFromXmlDeclaration(d).WebName);
                }
                catch (Exception ex)
                {
                    // **例外は ArgumentException に包み直される。**
                    //   メッセージは Open棟梁 のリソース由来なので、型名だけを出す。
                    MyDebug.OutputDebugAndConsole(
                        "XmlLib.GetEncodingFromXmlDeclaration - " + d + " : " + ex.GetType().Name);
                }
            }
        }

        #endregion
    }
}
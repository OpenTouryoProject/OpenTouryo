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
//* クラス名        ：ToStringExtensionTests
//* クラス日本語名  ：ToStringExtension の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/16  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System.Xml;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Str
{
    /// <summary>ToStringExtension の単体テスト</summary>
    [TestFixture]
    public class ToStringExtensionTests
    {
        /// <summary>XML 宣言付きで、設定どおりに整形した文字列にする（BOM は残らない）。</summary>
        [Test]
        public void XmlToString_WithDeclarationAndIndent()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root><child a=\"1\">x</child></root>");

            string xml = doc.XmlToString(new XmlWriterSettings() { Indent = true, IndentChars = "  " });

            Assert.That(xml, Does.StartWith("<?xml version=\"1.0\" encoding=\"utf-8\"?>"));
            Assert.That(xml, Does.Contain("  <child a=\"1\">x</child>"));

            // 読み戻せる
            XmlDocument back = new XmlDocument();
            back.LoadXml(xml);
            Assert.That(back.DocumentElement.InnerText, Is.EqualTo("x"));
        }

        /// <summary>宣言を省く設定なら、宣言を出さない。</summary>
        [Test]
        public void XmlToString_OmitDeclaration()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<root/>");

            string xml = doc.XmlToString(new XmlWriterSettings() { OmitXmlDeclaration = true });

            Assert.That(xml, Is.EqualTo("<root />"));
        }
    }
}

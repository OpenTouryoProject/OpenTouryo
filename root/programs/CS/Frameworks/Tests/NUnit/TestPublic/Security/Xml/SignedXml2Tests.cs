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
//* クラス名        ：SignedXml2Tests
//* クラス日本語名  ：SignedXml2（XML 署名）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//*  2026/10/19  玄人 幸道         署名の付け替え（XML Signature Wrapping）を弾くケースを追加
//**********************************************************************************

using System.Security.Cryptography;
using System.Xml;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security.Xml;

namespace TestPublic.Security.Xml
{
    /// <summary>SignedXml2（XML 署名）の単体テスト</summary>
    /// <remarks>
    /// 対象の要素は XmlLib.GetXmlNodeById で探す（ルートは ID 属性、ルート以下は要素名）。
    /// 入れ子の署名の後退テストは Golden の EncAndDecUtilCUI にもある。
    /// </remarks>
    [TestFixture]
    public class SignedXml2Tests
    {
        /// <summary>署名する XML（Golden と同じ形）</summary>
        private const string Xml = "<xml><a ID=\"a\"><b ID=\"b\"><c>value</c></b></a></xml>";

        /// <summary>署名した要素は、同じ鍵で検証できる。文字列でも XmlDocument でも作れる。</summary>
        [Test]
        public void CreateAndVerify()
        {
            using (RSA rsa = RSA.Create())
            {
                SignedXml2 sx = new SignedXml2(rsa);

                string signed = sx.Create(Xml, "b").OuterXml;
                Assert.That(signed, Does.Contain("<Signature"));
                Assert.That(sx.Verify(signed, "b"), Is.True);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Xml);
                Assert.That(sx.Verify(sx.Create(doc, "a").OuterXml, "a"), Is.True);
            }
        }

        /// <summary>入れ子（内側 b を署名した後、外側 a を署名）でも、両方とも検証できる。</summary>
        [Test]
        public void Nested()
        {
            using (RSA rsa = RSA.Create())
            {
                SignedXml2 sx = new SignedXml2(rsa);

                string signed = sx.Create(sx.Create(Xml, "b").OuterXml, "a").OuterXml;

                Assert.That(sx.Verify(signed, "b"), Is.True);
                Assert.That(sx.Verify(signed, "a"), Is.True);
            }
        }

        /// <summary>署名した後で内容を書き換えると、検証に失敗する。違う鍵でも失敗する。</summary>
        [Test]
        public void Tampered_OrOtherKey_False()
        {
            using (RSA rsa = RSA.Create())
            using (RSA other = RSA.Create())
            {
                SignedXml2 sx = new SignedXml2(rsa);
                string signed = sx.Create(Xml, "b").OuterXml;

                Assert.That(sx.Verify(signed.Replace(">value<", ">evil<"), "b"), Is.False);
                Assert.That(new SignedXml2(other).Verify(signed, "b"), Is.False);
            }
        }

        /// <summary>
        /// **署名の付け替え（XML Signature Wrapping）を弾く。**
        /// 正当に署名された断片（Reference は「#b」）を文書内に残したまま、
        /// 消費する要素（ここでは id="z"）を差し替えても、検証は通らない。
        /// Verify は「署名が referenceId を指していること」を突き合わせる。
        /// </summary>
        [Test]
        public void SignatureWrapping_WrongReference_False()
        {
            using (RSA rsa = RSA.Create())
            {
                SignedXml2 sx = new SignedXml2(rsa);

                // b を署名（Reference は「#b」）。この断片は、それ自体は正当に署名されている。
                XmlDocument signedDoc = sx.Create(Xml, "b");
                string signatureXml = signedDoc
                    .SelectSingleNode("//*[local-name()='Signature']").OuterXml;

                // 付け替え：ルート z（消費される側）に、署名が守っていない evil を入れ、
                //           署名が守っている b（署名時のまま）と、b を指す署名を同居させる。
                string wrapped =
                    "<z ID=\"z\"><c>evil</c>"
                    + "<b ID=\"b\"><c>value</c></b>"
                    + signatureXml
                    + "</z>";

                // 署名は「#b」を指すので、z としては通らない（Uri の不一致）。
                Assert.That(sx.Verify(wrapped, "z"), Is.False);
            }
        }

        /// <summary>
        /// **同じ id の要素が複数ある応答を弾く。**
        /// 署名された b のほかに、同じ id="b" の偽ノードを足しても、検証は通らない。
        /// （CheckSignature がどちらを検証するかに依存させない。）
        /// </summary>
        [Test]
        public void SignatureWrapping_DuplicateId_False()
        {
            using (RSA rsa = RSA.Create())
            {
                SignedXml2 sx = new SignedXml2(rsa);

                string signed = sx.Create(Xml, "b").OuterXml;

                // 署名済みの b の後ろに、同じ id="b" の偽ノードを足す。
                string duplicated = signed.Replace(
                    "</b></a>", "</b><b ID=\"b\"><c>evil</c></b></a>");

                Assert.That(sx.Verify(duplicated, "b"), Is.False);
            }
        }

        /// <summary>壊れた署名・対象に Signature が無い場合は、例外ではなく false を返す。</summary>
        [Test]
        public void Malformed_False()
        {
            using (RSA rsa = RSA.Create())
            {
                SignedXml2 sx = new SignedXml2(rsa);

                // Signature を持たない文書
                Assert.That(sx.Verify("<xml><b ID=\"b\"><c>value</c></b></xml>", "b"), Is.False);
            }
        }
    }
}

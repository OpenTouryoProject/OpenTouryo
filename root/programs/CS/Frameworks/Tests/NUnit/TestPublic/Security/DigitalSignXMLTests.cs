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
//* クラス名        ：DigitalSignXMLTests
//* クラス日本語名  ：DigitalSignXML（XML 形式の鍵で署名）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//*  2026/10/18  玄人 幸道         DSA の Sign / Verify のケースを追加（#625）
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//**********************************************************************************

using System;
using System.Text;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Public.Security;

namespace TestPublic.Security
{
    /// <summary>DigitalSignXML（XML 形式の鍵で署名）の単体テスト</summary>
    [TestFixture]
    public class DigitalSignXMLTests
    {
        /// <summary>署名するデータ</summary>
        private static readonly byte[] Data = Encoding.UTF8.GetBytes("署名するデータ");

        /// <summary>改ざんしたデータ</summary>
        private static readonly byte[] Tampered = Encoding.UTF8.GetBytes("署名するデータ!");

        /// <summary>
        /// RSA：秘密鍵の XML で署名し、公開鍵の XML だけを持つ側で検証できる。
        /// 公開鍵の XML から作ったものは、秘密鍵の XML を返さない（空文字）。
        /// </summary>
        [TestCase(EnumDigitalSignAlgorithm.Rsa_SHA256)]
        [TestCase(EnumDigitalSignAlgorithm.Rsa_SHA512)]
        public void Rsa_XmlKeys(EnumDigitalSignAlgorithm eaa)
        {
            using (DigitalSignXML generated = new DigitalSignXML(eaa))
            {
                string privateXml = generated.PrivateKey;
                string publicXml = generated.PublicKey;

                Assert.That(privateXml, Does.Contain("<D>"));
                Assert.That(publicXml, Does.Contain("<Modulus>").And.Not.Contain("<D>"));

                using (DigitalSignXML signer = new DigitalSignXML(privateXml, eaa))
                using (DigitalSignXML verifier = new DigitalSignXML(publicXml, eaa))
                {
                    byte[] sign = signer.Sign(Data);
                    Assert.That(verifier.Verify(Data, sign), Is.True);
                    Assert.That(verifier.Verify(Tampered, sign), Is.False);
                    Assert.That(verifier.PrivateKey, Is.Empty);
                }
            }
        }

        /// <summary>DSA：秘密鍵の XML で署名し（Formatter）、公開鍵の XML で検証できる。</summary>
        [Test]
        [Basis("#625")]
        public void Dsa_XmlKeys()
        {
            using (DigitalSignXML generated = new DigitalSignXML(EnumDigitalSignAlgorithm.DsaCSP_SHA1))
            using (DigitalSignXML signer = new DigitalSignXML(generated.PrivateKey, EnumDigitalSignAlgorithm.DsaCSP_SHA1))
            using (DigitalSignXML verifier = new DigitalSignXML(generated.PublicKey, EnumDigitalSignAlgorithm.DsaCSP_SHA1))
            {
                Assert.That(generated.PublicKey, Does.Contain("<DSAKeyValue>"));

                byte[] sign = signer.SignByFormatter(Data);
                Assert.That(verifier.VerifyByDeformatter(Data, sign), Is.True);
                Assert.That(verifier.VerifyByDeformatter(Tampered, sign), Is.False);

                // データを渡す版でも署名して検証できる（#625）
                byte[] sign2 = signer.Sign(Data);
                Assert.That(verifier.Verify(Data, sign2), Is.True);
                Assert.That(verifier.Verify(Tampered, sign2), Is.False);
            }
        }

        /// <summary>
        /// XML 形式の鍵を持てない方式（ECDSA）は、鍵を渡す版で NotImplementedException。
        /// 鍵を生成する版では、PrivateKey は空文字、PublicKey は null。
        /// </summary>
        [Test]
        [Platform("Win")]
        public void Ecdsa_NotSupported()
        {
            using (DigitalSignXML ecdsa = new DigitalSignXML(EnumDigitalSignAlgorithm.ECDsaCng_P256))
            {
                Assert.That(ecdsa.PrivateKey, Is.Empty);
                Assert.That(ecdsa.PublicKey, Is.Null);
            }

            Assert.Throws<NotImplementedException>(() => new DigitalSignXML("<x/>", EnumDigitalSignAlgorithm.ECDsaCng_P256));
        }
    }
}

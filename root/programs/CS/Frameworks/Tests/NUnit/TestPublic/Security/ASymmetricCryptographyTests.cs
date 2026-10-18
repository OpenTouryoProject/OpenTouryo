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
//* クラス名        ：ASymmetricCryptographyTests
//* クラス日本語名  ：ASymmetricCryptography（公開鍵暗号）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System.Security.Cryptography;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security;

namespace TestPublic.Security
{
    /// <summary>ASymmetricCryptography（公開鍵暗号）の単体テスト</summary>
    /// <remarks>暗号文は乱数を含むので、値ではなく「暗号化して戻せる」を確かめる。</remarks>
    [TestFixture]
    public class ASymmetricCryptographyTests
    {
        /// <summary>平文</summary>
        private const string Plain = "公開鍵暗号 abc 123";

        #region ASymmetricCryptography

        /// <summary>生成した鍵で、文字列もバイト列も暗号化して戻せる。</summary>
        [TestCase(EnumASymmetricAlgorithm.RsaCsp)]
        [TestCase(EnumASymmetricAlgorithm.RsaCng)]
        public void RoundTrip(EnumASymmetricAlgorithm easa)
        {
            ASymmetricCryptography ac = new ASymmetricCryptography(easa);

            Assert.That(ac.AsymmetricAlgorithm, Is.InstanceOf<RSA>());
            Assert.That(ac.DecryptString(ac.EncryptString(Plain)), Is.EqualTo(Plain));

            byte[] bytes = new byte[] { 0, 1, 2, 255 };
            Assert.That(ac.DecryptBytes(ac.EncryptBytes(bytes)), Is.EqualTo(bytes));
        }

        /// <summary>相手の公開鍵（XML）で暗号化し、相手が秘密鍵（XML）で戻せる。</summary>
        [Test]
        public void XmlKeys()
        {
            ASymmetricCryptography receiver = new ASymmetricCryptography(EnumASymmetricAlgorithm.RsaCng);
            ASymmetricCryptography sender = new ASymmetricCryptography(EnumASymmetricAlgorithm.RsaCng);

            Assert.That(receiver.PublicXmlKey, Does.Not.Contain("<D>"));
            Assert.That(receiver.PrivateXmlKey, Does.Contain("<D>"));

            string cipher = sender.EncryptString(Plain, receiver.PublicXmlKey);

            ASymmetricCryptography other = new ASymmetricCryptography(EnumASymmetricAlgorithm.RsaCsp);
            Assert.That(other.DecryptString(cipher, receiver.PrivateXmlKey), Is.EqualTo(Plain));
        }

        /// <summary>パディングを指定できる（OAEP-SHA1）。指定しなければ PKCS#1 v1.5。</summary>
        [Test]
        public void OaepSha1()
        {
            ASymmetricCryptography ac = new ASymmetricCryptography(EnumASymmetricAlgorithm.RsaCsp);

            string cipher = ac.EncryptString(Plain, "", RSAEncryptionPadding.OaepSHA1);

            Assert.That(ac.DecryptString(cipher, "", RSAEncryptionPadding.OaepSHA1), Is.EqualTo(Plain));
            Assert.Throws(Is.InstanceOf<CryptographicException>(), () => ac.DecryptString(cipher));
        }

        /// <summary>証明書：*.cer（公開鍵）で暗号化し、*.pfx（秘密鍵）で戻せる。</summary>
        [Test]
        public void X509()
        {
            ASymmetricCryptography enc = new ASymmetricCryptography(EnumASymmetricAlgorithm.X509, TestCertificates.RsaCer);
            ASymmetricCryptography dec = new ASymmetricCryptography(
                EnumASymmetricAlgorithm.X509, TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags);

            Assert.That(dec.DecryptString(enc.EncryptString(Plain)), Is.EqualTo(Plain));
        }

        #endregion
    }
}

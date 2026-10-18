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
//* クラス名        ：DigitalSignX509Tests
//* クラス日本語名  ：DigitalSignX509 / DigitalSignECDsaX509（証明書で署名）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security;

namespace TestPublic.Security
{
    /// <summary>DigitalSignX509 / DigitalSignECDsaX509（証明書で署名）の単体テスト</summary>
    /// <remarks>
    /// *.pfx（秘密鍵あり）で署名し、*.cer（公開鍵だけ）で検証する。証明書は TestCertificates。
    /// </remarks>
    [TestFixture]
    public class DigitalSignX509Tests
    {
        /// <summary>署名するデータ</summary>
        private static readonly byte[] Data = Encoding.UTF8.GetBytes("署名するデータ");

        /// <summary>改ざんしたデータ</summary>
        private static readonly byte[] Tampered = Encoding.UTF8.GetBytes("署名するデータ!");

        #region RSA（DigitalSignX509）

        /// <summary>RSA：*.pfx で署名し、*.cer で検証できる。ハッシュ名は文字列でも HashAlgorithmName でもよい。</summary>
        [Test]
        public void Rsa_PfxSign_CerVerify()
        {
            using (DigitalSignX509 signer = new DigitalSignX509(TestCertificates.RsaPfx, TestCertificates.Password, "SHA256", TestCertificates.Flags))
            using (DigitalSignX509 verifier = new DigitalSignX509(TestCertificates.RsaCer, "", HashAlgorithmName.SHA256, TestCertificates.Flags))
            {
                Assert.That(signer.X509Certificate.HasPrivateKey, Is.True);
                Assert.That(verifier.X509Certificate.HasPrivateKey, Is.False);
                Assert.That(signer.PrivateKey, Is.InstanceOf<RSA>());
                Assert.That(verifier.PublicKey, Is.InstanceOf<RSA>());
                Assert.That(verifier.PrivateKey, Is.Null);

                byte[] sign = signer.Sign(Data);
                Assert.That(verifier.Verify(Data, sign), Is.True);
                Assert.That(verifier.Verify(Tampered, sign), Is.False);
            }
        }

        /// <summary>RSA：Formatter でも署名して検証できる。</summary>
        [Test]
        public void Rsa_Formatter()
        {
            using (DigitalSignX509 signer = new DigitalSignX509(TestCertificates.RsaPfx, TestCertificates.Password, HashAlgorithmName.SHA256, TestCertificates.Flags))
            using (DigitalSignX509 verifier = new DigitalSignX509(TestCertificates.RsaCer, "", "SHA256", TestCertificates.Flags))
            {
                byte[] sign = signer.SignByFormatter(Data);
                Assert.That(verifier.VerifyByDeformatter(Data, sign), Is.True);
                Assert.That(verifier.VerifyByDeformatter(Tampered, sign), Is.False);
            }
        }

        /// <summary>フラグを省略した版（既定の格納先）でも、署名して検証できる。</summary>
        [Test]
        public void Rsa_DefaultFlags()
        {
            using (DigitalSignX509 signer = new DigitalSignX509(TestCertificates.RsaPfx, TestCertificates.Password, "SHA256"))
            using (DigitalSignX509 verifier = new DigitalSignX509(TestCertificates.RsaCer, "", HashAlgorithmName.SHA256))
            {
                Assert.That(verifier.Verify(Data, signer.Sign(Data)), Is.True);
            }
        }

        #endregion

        #region ECDSA（DigitalSignECDsaX509）

        /// <summary>ECDSA：*.pfx で署名し、*.cer で検証できる（P-256 / P-384 / P-521）。</summary>
        [TestCase("SHA256ECDSA", "SHA256")]
        [TestCase("SHA384ECDSA", "SHA384")]
        [TestCase("SHA521ECDSA", "SHA512")]
        public void Ecdsa_PfxSign_CerVerify(string name, string hashName)
        {
            HashAlgorithmName han = new HashAlgorithmName(hashName);

            using (DigitalSignECDsaX509 signer = new DigitalSignECDsaX509(TestCertificates.PathOf(name + ".pfx"), TestCertificates.Password, han))
            using (DigitalSignECDsaX509 verifier = new DigitalSignECDsaX509(TestCertificates.PathOf(name + ".cer"), "", han, TestCertificates.Flags))
            {
                Assert.That(signer.AsymmetricAlgorithm, Is.InstanceOf<ECDsa>());
                Assert.That(signer.PrivateKey, Is.Not.Null);
                Assert.That(verifier.PublicKey, Is.Not.Null);
                Assert.That(verifier.HashAlgorithmString, Is.EqualTo(hashName));

                byte[] sign = signer.Sign(Data);
                Assert.That(verifier.Verify(Data, sign), Is.True);
                Assert.That(verifier.Verify(Tampered, sign), Is.False);
            }
        }

        /// <summary>ECDSA は Formatter を持たない（NotImplementedException）。</summary>
        [Test]
        public void Ecdsa_Formatter_NotImplemented()
        {
            using (DigitalSignECDsaX509 ds = new DigitalSignECDsaX509(TestCertificates.EcdsaCer256, "", HashAlgorithmName.SHA256))
            {
                Assert.Throws<NotImplementedException>(() => ds.SignByFormatter(Data));
                Assert.Throws<NotImplementedException>(() => ds.VerifyByDeformatter(Data, new byte[64]));
            }
        }

        #endregion
    }
}

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
//* クラス名        ：PrivacyEnhancedMailTests
//* クラス日本語名  ：PrivacyEnhancedMail（PEM の読み込み）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security;

namespace TestPublic.Security
{
    /// <summary>PrivacyEnhancedMail（PEM の読み込み）の単体テスト</summary>
    /// <remarks>PEM は、テストの証明書（*.cer、DER）を Base64 にして作業フォルダに書いたものを使う。</remarks>
    [TestFixture]
    public class PrivacyEnhancedMailTests
    {
        /// <summary>作業フォルダ（%TEMP%\OpenTouryoNUnit\GUID）</summary>
        private string _dir;

        /// <summary>作業フォルダを作る。</summary>
        [SetUp]
        public void SetUp()
        {
            this._dir = Path.Combine(Path.GetTempPath(), "OpenTouryoNUnit", Guid.NewGuid().ToString());
            Directory.CreateDirectory(this._dir);
        }

        /// <summary>作業フォルダを消す。</summary>
        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(this._dir)) { Directory.Delete(this._dir, true); }
        }

        /// <summary>DER を PEM（64 文字で改行）にする。</summary>
        private static string ToPem(byte[] der, string label)
        {
            string b64 = Convert.ToBase64String(der, Base64FormattingOptions.InsertLineBreaks);
            return "-----BEGIN " + label + "-----\n" + b64 + "\n-----END " + label + "-----\n";
        }

        /// <summary>ラベルの列挙値は、RFC 7468 のラベルの文字列になる。</summary>
        [TestCase(PrivacyEnhancedMail.RFC7468Label.Certificate, "CERTIFICATE")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.X509Certificate, "X509 CERTIFICATE")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.X509Certificate2, "X.509 CERTIFICATE")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.TrustedCertificate, "TRUSTED CERTIFICATE")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.NetscapeCertificateSequence, "NETSCAPE CERTIFICATE SEQUENCE")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.AttributeCertificate, "ATTRIBUTE CERTIFICATE")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.PKCS7, "PKCS7")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.CertificateChain, "CERTIFICATE CHAIN")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.PKCS7CertificateChain, "PKCS7 CERTIFICATE CHAIN")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.CMS, "CMS")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.PrivateKey, "PRIVATE KEY")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.PublicKey, "PUBLIC KEY")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.RsaPrivateKey, "RSA PRIVATE KEY")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.RsaPublicKey, "RSA PUBLIC KEY")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.DsaPrivateKey, "DSA PRIVATE KEY")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.DsaPublicKey, "DSA PUBLIC KEY")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.EncryptedPrivateKey, "ENCRYPTED PRIVATE KEY")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.CertificateRequest, "CERTIFICATE REQUEST")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.NewCertificateRequest, "NEW CERTIFICATE REQUEST")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.CRL, "CRL")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.X509CRL, "X509 CRL")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.EcParameters, "EC PARAMETERS")]
        [TestCase(PrivacyEnhancedMail.RFC7468Label.DhParameters, "DH PARAMETERS")]
        [TestCase((PrivacyEnhancedMail.RFC7468Label)999, "")]
        public void EnumToString(PrivacyEnhancedMail.RFC7468Label label, string expected)
        {
            Assert.That(PrivacyEnhancedMail.EnumToString(label), Is.EqualTo(expected));
        }

        /// <summary>PEM の文字列から、ラベルの間の Base64 を取り出す（改行は無視）。ラベルが無ければ null。</summary>
        [Test]
        public void GetBytesFromPemString()
        {
            byte[] der = new byte[] { 1, 2, 3, 4, 5, 250 };
            string pem = "前置き\n" + ToPem(der, "PUBLIC KEY") + "後置き";

            Assert.That(PrivacyEnhancedMail.GetBytesFromPemString(pem, "PUBLIC KEY"), Is.EqualTo(der));
            Assert.That(PrivacyEnhancedMail.GetBytesFromPemString(pem, "PRIVATE KEY"), Is.Null);
            Assert.That(PrivacyEnhancedMail.GetBytesFromPemString("-----BEGIN PUBLIC KEY-----\nAQID", "PUBLIC KEY"), Is.Null);
        }

        /// <summary>PEM ファイルから証明書と、その Base64 を読める。</summary>
        [Test]
        public void FromPemFile()
        {
            byte[] der = File.ReadAllBytes(TestCertificates.RsaCer);
            string path = Path.Combine(this._dir, "cert.pem");
            File.WriteAllText(path, ToPem(der, "CERTIFICATE"));

            using (X509Certificate2 expected = TestCertificates.LoadCer(TestCertificates.RsaCer))
            using (X509Certificate2 cert = PrivacyEnhancedMail.GetX509FromPemFilePath(path, PrivacyEnhancedMail.RFC7468Label.Certificate))
            {
                Assert.That(cert.Thumbprint, Is.EqualTo(expected.Thumbprint));
            }

            Assert.That(PrivacyEnhancedMail.GetBase64StringFromPemFilePath(path, PrivacyEnhancedMail.RFC7468Label.Certificate),
                Is.EqualTo(Convert.ToBase64String(der)));
        }
    }
}

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
//* クラス名        ：AsymmetricAlgorithmCmnFuncTests
//* クラス日本語名  ：AsymmetricAlgorithmCmnFunc（公開鍵のサービス プロバイダの生成）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security;

namespace TestPublic.Security
{
    /// <summary>AsymmetricAlgorithmCmnFunc（公開鍵のサービス プロバイダの生成）の単体テスト</summary>
    [TestFixture]
    public class AsymmetricAlgorithmCmnFuncTests
    {
        /// <summary>証明書から公開鍵・秘密鍵を取り出す。*.cer に秘密鍵は無い（null）。</summary>
        [Test]
        public void GetPublicKey_GetPrivateKey()
        {
            using (X509Certificate2 cer = TestCertificates.LoadCer(TestCertificates.RsaCer))
            using (X509Certificate2 pfx = TestCertificates.LoadPfx(TestCertificates.RsaPfx))
            using (X509Certificate2 ecCer = TestCertificates.LoadCer(TestCertificates.EcdsaCer256))
            using (X509Certificate2 ecPfx = TestCertificates.LoadPfx(TestCertificates.EcdsaPfx256))
            {
                Assert.That(AsymmetricAlgorithmCmnFunc.GetPublicKey(cer), Is.InstanceOf<RSA>());
                Assert.That(AsymmetricAlgorithmCmnFunc.GetPrivateKey(cer), Is.Null);
                Assert.That(AsymmetricAlgorithmCmnFunc.GetPrivateKey(pfx), Is.InstanceOf<RSA>());

                Assert.That(AsymmetricAlgorithmCmnFunc.GetPublicKey(ecCer), Is.InstanceOf<ECDsa>());
                Assert.That(AsymmetricAlgorithmCmnFunc.GetPrivateKey(ecPfx), Is.InstanceOf<ECDsa>());
            }
        }

        /// <summary>
        /// DSA の証明書からも、公開鍵・秘密鍵を取り出せる。
        /// 2026/10/18 まで、net10.0（Windows）では公開鍵の取り出しで例外になった（#626）。
        /// </summary>
        [Test]
        public void GetPublicKey_GetPrivateKey_Dsa()
        {
            using (X509Certificate2 cer = TestCertificates.LoadCer(TestCertificates.DsaCer))
            using (X509Certificate2 pfx = TestCertificates.LoadPfx(TestCertificates.DsaPfx))
            {
                Assert.That(AsymmetricAlgorithmCmnFunc.GetPublicKey(cer), Is.InstanceOf<DSA>());
                Assert.That(AsymmetricAlgorithmCmnFunc.GetPublicKey(pfx), Is.InstanceOf<DSA>());
                Assert.That(AsymmetricAlgorithmCmnFunc.GetPrivateKey(pfx), Is.InstanceOf<DSA>());
                Assert.That(AsymmetricAlgorithmCmnFunc.GetPrivateKey(cer), Is.Null);
            }
        }

        /// <summary>CreateCryptographySP：証明書は、パスワードが無ければ公開鍵、あれば秘密鍵。</summary>
        [Test]
        public void CreateCryptographySP_X509()
        {
            AsymmetricAlgorithm pub = AsymmetricAlgorithmCmnFunc.CreateCryptographySP(
                EnumASymmetricAlgorithm.X509, TestCertificates.RsaCer);
            AsymmetricAlgorithm priv = AsymmetricAlgorithmCmnFunc.CreateCryptographySP(
                EnumASymmetricAlgorithm.X509, TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags);

            Assert.That(pub, Is.InstanceOf<RSA>());
            Assert.That(((RSA)priv).ExportParameters(true).D, Is.Not.Null);
        }

        /// <summary>CreateCryptographySP：RSA の方式は、どれも RSA を返す。</summary>
        [TestCase(EnumASymmetricAlgorithm.RsaCsp)]
        [TestCase(EnumASymmetricAlgorithm.RsaCng)]
        public void CreateCryptographySP_Rsa(EnumASymmetricAlgorithm easa)
        {
            Assert.That(AsymmetricAlgorithmCmnFunc.CreateCryptographySP(easa), Is.InstanceOf<RSA>());
        }

        /// <summary>簡易なファクトリと、同じ鍵長のインスタンスを作る CreateSameKeySizeSP。</summary>
        [Test]
        public void Factories()
        {
            Assert.That(AsymmetricAlgorithmCmnFunc.RsaFactory(), Is.InstanceOf<RSA>());
            Assert.That(AsymmetricAlgorithmCmnFunc.DsaFactory(), Is.InstanceOf<DSA>());

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024))
            {
                AsymmetricAlgorithm same = AsymmetricAlgorithmCmnFunc.CreateSameKeySizeSP(rsa);
                Assert.That(same, Is.InstanceOf<RSACryptoServiceProvider>());
                Assert.That(same.KeySize, Is.EqualTo(1024));
            }
        }

        /// <summary>CreateDigitalSignSP：RSA はハッシュを、DSA は SHA1 を、ECDSA（CNG）はハッシュ無し（null）を返す。</summary>
        [Test]
        [Platform("Win")]
        public void CreateDigitalSignSP()
        {
            AsymmetricAlgorithm aa;
            HashAlgorithm ha;

            AsymmetricAlgorithmCmnFunc.CreateDigitalSignSP(EnumDigitalSignAlgorithm.Rsa_SHA384, out aa, out ha);
            Assert.That(aa, Is.InstanceOf<RSA>());
            Assert.That(ha, Is.InstanceOf<SHA384>());

            AsymmetricAlgorithmCmnFunc.CreateDigitalSignSP(EnumDigitalSignAlgorithm.DsaCSP_SHA1, out aa, out ha);
            Assert.That(aa, Is.InstanceOf<DSA>());
            Assert.That(ha, Is.InstanceOf<SHA1>());

            AsymmetricAlgorithmCmnFunc.CreateDigitalSignSP(EnumDigitalSignAlgorithm.ECDsaCng_P521, out aa, out ha);
            Assert.That(aa, Is.InstanceOf<ECDsa>());
            Assert.That(aa.KeySize, Is.EqualTo(521));
            Assert.That(ha, Is.Null);
        }
    }
}

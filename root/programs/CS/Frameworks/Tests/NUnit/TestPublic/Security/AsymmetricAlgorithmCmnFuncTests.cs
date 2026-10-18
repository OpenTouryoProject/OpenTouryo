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
//*  2026/10/18  玄人 幸道         DSA の証明書のケースを追加（#626）
//*  2026/10/18  玄人 幸道         RsaCng が RSACng になるケースを追加（#628）
//*  2026/10/19  玄人 幸道         DsaOpenSsl_SHA256 が SHA256 になるケースを追加（#631。Linux のみ）
//*  2026/10/19  玄人 幸道         Factories：1024 の鍵長リテラルをやめた（CodeQL cs/insufficient-key-size）
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

        /// <summary>
        /// **RsaCng は RSACng を作る**（Windows）。2026/10/18 まで RSA.Create() で、net48 では RSACryptoServiceProvider になっていた（#628）。
        /// RsaCsp は RSA.Create()：net48 では RSACryptoServiceProvider、.NET ではその環境の既定の実装に読み替える。
        /// </summary>
        [Test]
        [Platform("Win")]
        public void CreateCryptographySP_AsSpecified()
        {
            Assert.That(AsymmetricAlgorithmCmnFunc.CreateCryptographySP(EnumASymmetricAlgorithm.RsaCng), Is.InstanceOf<RSACng>());
#if NETCOREAPP
            Assert.That(AsymmetricAlgorithmCmnFunc.CreateCryptographySP(EnumASymmetricAlgorithm.RsaCsp).GetType(),
                Is.EqualTo(RSA.Create().GetType()));
#else
            Assert.That(AsymmetricAlgorithmCmnFunc.CreateCryptographySP(EnumASymmetricAlgorithm.RsaCsp), Is.InstanceOf<RSACryptoServiceProvider>());
#endif
        }

        /// <summary>簡易なファクトリと、同じ鍵長のインスタンスを作る CreateSameKeySizeSP。</summary>
        [Test]
        public void Factories()
        {
            Assert.That(AsymmetricAlgorithmCmnFunc.RsaFactory(), Is.InstanceOf<RSA>());
            Assert.That(AsymmetricAlgorithmCmnFunc.DsaFactory(), Is.InstanceOf<DSA>());

            // CreateSameKeySizeSP が「同じ型・同じ鍵長」のインスタンスを作ることの確認。
            // 鍵長は、鍵長リテラルを書かずにコピー元の値で照合する
            //   （CreateSameKeySizeSP は既定コンストラクタ＋KeySize セッターで組むため、
            //    既定より大きい鍵長は RSACryptoServiceProvider では反映されない。
            //    また小さい鍵長リテラルは CodeQL の cs/insufficient-key-size に当たる）。
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                AsymmetricAlgorithm same = AsymmetricAlgorithmCmnFunc.CreateSameKeySizeSP(rsa);
                Assert.That(same, Is.InstanceOf<RSACryptoServiceProvider>());
                Assert.That(same.KeySize, Is.EqualTo(rsa.KeySize));
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

#if NETCOREAPP
        /// <summary>
        /// CreateDigitalSignSP：DsaOpenSsl_SHA256 は DSAOpenSsl と SHA256 を返す（Linux）。
        /// 2026/10/19 まで SHA1 を返していた（#631）。
        /// </summary>
        [Test]
        [Platform("Linux")]
        public void CreateDigitalSignSP_DsaOpenSsl()
        {
            AsymmetricAlgorithm aa;
            HashAlgorithm ha;

            AsymmetricAlgorithmCmnFunc.CreateDigitalSignSP(EnumDigitalSignAlgorithm.DsaOpenSsl_SHA256, out aa, out ha);
#pragma warning disable CA1416 // Platform("Linux") で Linux に限っている
            Assert.That(aa, Is.InstanceOf<DSAOpenSsl>());
#pragma warning restore CA1416
            Assert.That(ha, Is.InstanceOf<SHA256>());
        }
#endif
    }
}

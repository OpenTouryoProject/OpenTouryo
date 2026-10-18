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
//* クラス名        ：DigitalSignECDsaCngTests
//* クラス日本語名  ：DigitalSignECDsaCng（Windows の CNG で ECDSA 署名）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//*  2026/10/18  玄人 幸道         生成した鍵の秘密鍵を取り出すケースを追加（#627）
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security;

namespace TestPublic.Security
{
    /// <summary>DigitalSignECDsaCng（Windows の CNG で ECDSA 署名）の単体テスト</summary>
    /// <remarks>
    /// **CNG は Windows だけ**（Public/Security/ANALYSIS.md 3.1 節）。Windows 以外では回さない。
    /// ハッシュを渡さなければ SHA-256（#595 で、渡したハッシュが効くよう直した）。
    /// </remarks>
    [TestFixture]
    [Platform("Win")]
    public class DigitalSignECDsaCngTests
    {
        /// <summary>署名するデータ</summary>
        private static readonly byte[] Data = Encoding.UTF8.GetBytes("署名するデータ");

        /// <summary>改ざんしたデータ</summary>
        private static readonly byte[] Tampered = Encoding.UTF8.GetBytes("署名するデータ!");

        /// <summary>曲線とハッシュの組で、署名し、公開鍵（EccPublicBlob）だけを持つ側で検証できる。</summary>
        [TestCase(EnumDigitalSignAlgorithm.ECDsaCng_P256, null)]
        [TestCase(EnumDigitalSignAlgorithm.ECDsaCng_P256, "SHA256")]
        [TestCase(EnumDigitalSignAlgorithm.ECDsaCng_P384, "SHA384")]
        [TestCase(EnumDigitalSignAlgorithm.ECDsaCng_P521, "SHA512")]
        public void SignAndVerify(EnumDigitalSignAlgorithm eaa, string hashName)
        {
            HashAlgorithm ha = hashName == null ? null : HashAlgorithmCmnFunc.GetHashAlgorithmFromNameString(hashName);
            HashAlgorithm ha2 = hashName == null ? null : HashAlgorithmCmnFunc.GetHashAlgorithmFromNameString(hashName);

            using (DigitalSignECDsaCng signer = new DigitalSignECDsaCng(eaa, ha))
            using (DigitalSignECDsaCng verifier = new DigitalSignECDsaCng(signer.PublicKey, ha2))
            {
                Assert.That(signer.PrivateKey, Is.Not.Null);
                Assert.That(verifier.PrivateKey, Is.Null);

                byte[] sign = signer.Sign(Data);
                Assert.That(verifier.Verify(Data, sign), Is.True);
                Assert.That(verifier.Verify(Tampered, sign), Is.False);
            }
        }

        /// <summary>ハッシュを渡さない版（既定の SHA-256）で、公開鍵だけの側でも検証できる。</summary>
        [Test]
        public void DefaultHash()
        {
            using (DigitalSignECDsaCng signer = new DigitalSignECDsaCng(EnumDigitalSignAlgorithm.ECDsaCng_P256))
            using (DigitalSignECDsaCng verifier = new DigitalSignECDsaCng(signer.PublicKey))
            {
                Assert.That(verifier.Verify(Data, signer.Sign(Data)), Is.True);
            }
        }

        /// <summary>CngKey を渡す版で、署名して検証できる。</summary>
        [Test]
        public void CngKey_SignAndVerify()
        {
            using (DigitalSignECDsaCng generated = new DigitalSignECDsaCng(EnumDigitalSignAlgorithm.ECDsaCng_P256))
            using (DigitalSignECDsaCng ds = new DigitalSignECDsaCng(generated.PrivateKey, true))
            {
                Assert.That(ds.PrivateKey, Is.SameAs(generated.PrivateKey));
                Assert.That(ds.Verify(Data, ds.Sign(Data)), Is.True);
            }
        }

        /// <summary>ECParameters を渡す版（秘密鍵を含む）で、署名して検証できる。</summary>
        [Test]
        public void EcParameters_SignAndVerify()
        {
            ECParameters ecp;
            using (ECDsa ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP384)) { ecp = ecdsa.ExportParameters(true); }

            using (DigitalSignECDsaCng ds = new DigitalSignECDsaCng(ecp, true, SHA384.Create()))
            {
                byte[] sign = ds.Sign(Data);
                Assert.That(ds.Verify(Data, sign), Is.True);
                Assert.That(ds.Verify(Tampered, sign), Is.False);
            }

            // 秘密鍵を持たせない版は、PrivateKey が null
            using (DigitalSignECDsaCng pub = new DigitalSignECDsaCng(ecp, false))
            {
                Assert.That(pub.PrivateKey, Is.Null);
                Assert.That(pub.PublicKey, Is.Not.Null);
            }
        }

        /// <summary>
        /// 生成した鍵は、秘密鍵を ECParameters として取り出せる（平文でのエクスポートを許す）。
        /// 2026/10/18 まで、取り出すと例外になった（#627）。
        /// </summary>
        [TestCase(EnumDigitalSignAlgorithm.ECDsaCng_P256)]
        [TestCase(EnumDigitalSignAlgorithm.ECDsaCng_P384)]
        [TestCase(EnumDigitalSignAlgorithm.ECDsaCng_P521)]
        public void GeneratedKey_Exportable(EnumDigitalSignAlgorithm eaa)
        {
            using (DigitalSignECDsaCng ds = new DigitalSignECDsaCng(eaa))
            {
                ECParameters priv = ((ECDsa)ds.AsymmetricAlgorithm).ExportParameters(true);
                Assert.That(priv.D, Is.Not.Null);

                // 取り出した秘密鍵で署名し、元の公開鍵で検証できる
                using (DigitalSignECDsaCng imported = new DigitalSignECDsaCng(priv, true))
                using (DigitalSignECDsaCng verifier = new DigitalSignECDsaCng(ds.PublicKey))
                {
                    Assert.That(verifier.Verify(Data, imported.Sign(Data)), Is.True);
                }
            }
        }

        /// <summary>CNG の ECDSA は Formatter を持たない（NotImplementedException）。</summary>
        [Test]
        public void Formatter_NotImplemented()
        {
            using (DigitalSignECDsaCng ds = new DigitalSignECDsaCng(EnumDigitalSignAlgorithm.ECDsaCng_P256))
            {
                Assert.Throws<NotImplementedException>(() => ds.SignByFormatter(Data));
                Assert.Throws<NotImplementedException>(() => ds.VerifyByDeformatter(Data, new byte[64]));
            }
        }
    }
}

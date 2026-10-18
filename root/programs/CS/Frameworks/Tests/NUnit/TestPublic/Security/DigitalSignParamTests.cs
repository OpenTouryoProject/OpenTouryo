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
//* クラス名        ：DigitalSignParamTests
//* クラス日本語名  ：DigitalSignParam（パラメタで鍵を持つ署名）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//*  2026/10/18  玄人 幸道         DSA の Sign / Verify のケースを追加（#625）
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security;

namespace TestPublic.Security
{
    /// <summary>DigitalSignParam（パラメタで鍵を持つ署名）の単体テスト</summary>
    /// <remarks>
    /// 署名は鍵ごとに乱数を含むので、値ではなく「署名して検証できる」「改ざんすると検証に失敗する」を確かめる。
    /// 後退テストは Golden の EncAndDecUtilCUI にもある。
    /// </remarks>
    [TestFixture]
    public class DigitalSignParamTests
    {
        /// <summary>署名するデータ</summary>
        private static readonly byte[] Data = Encoding.UTF8.GetBytes("署名するデータ");

        /// <summary>改ざんしたデータ</summary>
        private static readonly byte[] Tampered = Encoding.UTF8.GetBytes("署名するデータ!");

        #region RSA

        /// <summary>RSA は、ハッシュごとに署名して検証できる。改ざんすれば検証に失敗する。</summary>
        [TestCase(EnumDigitalSignAlgorithm.Rsa_MD5, "MD5")]
        [TestCase(EnumDigitalSignAlgorithm.Rsa_SHA1, "SHA1")]
        [TestCase(EnumDigitalSignAlgorithm.Rsa_SHA256, "SHA256")]
        [TestCase(EnumDigitalSignAlgorithm.Rsa_SHA384, "SHA384")]
        [TestCase(EnumDigitalSignAlgorithm.Rsa_SHA512, "SHA512")]
        public void Rsa_SignAndVerify(EnumDigitalSignAlgorithm eaa, string hashName)
        {
            using (DigitalSignParam ds = new DigitalSignParam(eaa))
            {
                Assert.That(ds.AsymmetricAlgorithm, Is.InstanceOf<RSA>());
                Assert.That(ds.HashAlgorithmString, Is.EqualTo(hashName));
                Assert.That(ds.HashAlgorithmName.Name, Is.EqualTo(hashName));

                byte[] sign = ds.Sign(Data);
                Assert.That(ds.Verify(Data, sign), Is.True);
                Assert.That(ds.Verify(Tampered, sign), Is.False);
            }
        }

        /// <summary>Formatter（RSAPKCS1SignatureFormatter）でも署名して検証できる。</summary>
        [TestCase(EnumDigitalSignAlgorithm.Rsa_SHA1)]
        [TestCase(EnumDigitalSignAlgorithm.Rsa_SHA256)]
        [TestCase(EnumDigitalSignAlgorithm.Rsa_SHA512)]
        public void Rsa_Formatter(EnumDigitalSignAlgorithm eaa)
        {
            using (DigitalSignParam ds = new DigitalSignParam(eaa))
            {
                byte[] sign = ds.SignByFormatter(Data);
                Assert.That(ds.VerifyByDeformatter(Data, sign), Is.True);
                Assert.That(ds.VerifyByDeformatter(Tampered, sign), Is.False);

                // Formatter と SignData は同じ RSASSA-PKCS1-v1_5 なので、互いに検証できる
                Assert.That(ds.Verify(Data, sign), Is.True);
            }
        }

        /// <summary>
        /// 公開鍵だけの RSAParameters を渡すと、検証だけができる。
        /// PrivateKey / PublicKey は RSAParameters を返す。
        /// </summary>
        [Test]
        public void Rsa_PublicParameters_VerifyOnly()
        {
            using (DigitalSignParam signer = new DigitalSignParam(EnumDigitalSignAlgorithm.Rsa_SHA256))
            {
                byte[] sign = signer.Sign(Data);

                RSAParameters pub = (RSAParameters)signer.PublicKey;
                Assert.That(pub.D, Is.Null);
                Assert.That(((RSAParameters)signer.PrivateKey).D, Is.Not.Null);

                using (DigitalSignParam verifier = new DigitalSignParam(pub, EnumDigitalSignAlgorithm.Rsa_SHA256))
                {
                    Assert.That(verifier.Verify(Data, sign), Is.True);
                    Assert.That(verifier.Verify(Tampered, sign), Is.False);
                }

                // 秘密鍵を含めて渡せば、署名もできる
                using (DigitalSignParam signer2 = new DigitalSignParam((RSAParameters)signer.PrivateKey, EnumDigitalSignAlgorithm.Rsa_SHA256))
                {
                    Assert.That(signer.Verify(Data, signer2.Sign(Data)), Is.True);
                }
            }
        }

        /// <summary>RSA-PSS（HashAlgorithmName と RSASignaturePadding を渡す版）で署名して検証できる。</summary>
        [Test]
        public void Rsa_Pss()
        {
            using (DigitalSignParam signer = new DigitalSignParam(HashAlgorithmName.SHA256, RSASignaturePadding.Pss))
            {
                Assert.That(signer.Padding, Is.EqualTo(RSASignaturePadding.Pss));

                byte[] sign = signer.Sign(Data);
                Assert.That(signer.Verify(Data, sign), Is.True);

                RSAParameters pub = (RSAParameters)signer.PublicKey;
                using (DigitalSignParam verifier = new DigitalSignParam(pub, HashAlgorithmName.SHA256, RSASignaturePadding.Pss))
                {
                    Assert.That(verifier.Verify(Data, sign), Is.True);
                    Assert.That(verifier.Verify(Tampered, sign), Is.False);
                }
            }
        }

        /// <summary>RSAParameters に DSA の方式を合わせると ArgumentException。</summary>
        [Test]
        public void Rsa_Unmatched_Throws()
        {
            RSAParameters pub;
            using (RSA rsa = RSA.Create()) { pub = rsa.ExportParameters(false); }

            Assert.Throws<ArgumentException>(() => new DigitalSignParam(pub, EnumDigitalSignAlgorithm.DsaCSP_SHA1));
        }

        #endregion

        #region DSA

        /// <summary>DSA は Formatter（DSASignatureFormatter）で署名して検証できる。PublicKey は DSAParameters。</summary>
        [Test]
        public void Dsa_Formatter()
        {
            using (DigitalSignParam signer = new DigitalSignParam(EnumDigitalSignAlgorithm.DsaCSP_SHA1))
            {
                Assert.That(signer.AsymmetricAlgorithm, Is.InstanceOf<DSA>());

                byte[] sign = signer.SignByFormatter(Data);
                Assert.That(signer.VerifyByDeformatter(Data, sign), Is.True);
                Assert.That(signer.VerifyByDeformatter(Tampered, sign), Is.False);

                DSAParameters pub = (DSAParameters)signer.PublicKey;
                Assert.That(pub.X, Is.Null);
                Assert.That(((DSAParameters)signer.PrivateKey).X, Is.Not.Null);

                using (DigitalSignParam verifier = new DigitalSignParam(pub, EnumDigitalSignAlgorithm.DsaCSP_SHA1))
                {
                    Assert.That(verifier.VerifyByDeformatter(Data, sign), Is.True);
                }
            }
        }

        /// <summary>
        /// **DSA も Sign / Verify（データを渡す版）で署名して検証できる。**
        /// Formatter の版と同じ方式なので、互いに検証できる。
        /// 2026/10/18 まで、データをハッシュせずに渡していて例外になった（#625）。
        /// </summary>
        [Test]
        public void Dsa_SignAndVerify()
        {
            using (DigitalSignParam signer = new DigitalSignParam(EnumDigitalSignAlgorithm.DsaCSP_SHA1))
            {
                byte[] sign = signer.Sign(Data);
                Assert.That(signer.Verify(Data, sign), Is.True);
                Assert.That(signer.Verify(Tampered, sign), Is.False);

                Assert.That(signer.VerifyByDeformatter(Data, sign), Is.True);
                Assert.That(signer.Verify(Data, signer.SignByFormatter(Data)), Is.True);

                using (DigitalSignParam verifier = new DigitalSignParam((DSAParameters)signer.PublicKey, EnumDigitalSignAlgorithm.DsaCSP_SHA1))
                {
                    Assert.That(verifier.Verify(Data, sign), Is.True);
                }
            }
        }

        /// <summary>DSAParameters に RSA の方式を合わせると ArgumentException。</summary>
        [Test]
        public void Dsa_Unmatched_Throws()
        {
            DSAParameters pub;
            using (DigitalSignParam dsa = new DigitalSignParam(EnumDigitalSignAlgorithm.DsaCSP_SHA1)) { pub = (DSAParameters)dsa.PublicKey; }

            Assert.Throws<ArgumentException>(() => new DigitalSignParam(pub, EnumDigitalSignAlgorithm.Rsa_SHA256));
        }

        #endregion

        #region その他

        /// <summary>列挙に無い方式は ArgumentException（CreateDigitalSignSP）。</summary>
        [Test]
        public void UnknownAlgorithm_Throws()
        {
            Assert.Throws<ArgumentException>(() => new DigitalSignParam((EnumDigitalSignAlgorithm)999));
        }

        /// <summary>Close / Dispose は何度呼んでもよい。</summary>
        [Test]
        public void Dispose_Twice()
        {
            DigitalSignParam rsa = new DigitalSignParam(EnumDigitalSignAlgorithm.Rsa_SHA256);
            rsa.Close();
            Assert.DoesNotThrow(() => rsa.Dispose());

            DigitalSignParam dsa = new DigitalSignParam(EnumDigitalSignAlgorithm.DsaCSP_SHA1);
            dsa.Dispose();
            Assert.DoesNotThrow(() => dsa.Dispose());
        }

        #endregion
    }
}

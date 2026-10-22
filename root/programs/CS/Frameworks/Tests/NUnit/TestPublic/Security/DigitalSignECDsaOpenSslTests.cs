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
//* クラス名        ：DigitalSignECDsaOpenSslTests
//* クラス日本語名  ：DigitalSignECDsaOpenSsl（ECDSA の OpenSSL 版）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/19  玄人 幸道         新規作成（カバレッジの拡充。Linux のみ）
//**********************************************************************************

#if NETCOREAPP
using System;
using System.Security.Cryptography;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security;

namespace TestPublic.Security
{
    /// <summary>DigitalSignECDsaOpenSsl（ECDSA の OpenSSL 版）の単体テスト</summary>
    /// <remarks>
    /// **ECDsaOpenSsl は Linux でしか作れない**（Windows は CNG。DigitalSignECDsaCngTests）。
    /// Linux の回（2_RunAllTests.ps1 -Linux / -Coverage）でだけ走る。
    /// 署名は乱数を含むので、値ではなく「検証できる」「改ざんすると失敗する」を確かめる。
    /// </remarks>
    [TestFixture]
    [Platform("Linux")]
#pragma warning disable CA1416 // Platform("Linux") で Linux に限っている
    public class DigitalSignECDsaOpenSslTests
    {
        private static readonly byte[] Data = Encoding.UTF8.GetBytes("署名するデータ");
        private static readonly byte[] Tampered = Encoding.UTF8.GetBytes("署名するデータ!");

        /// <summary>
        /// 鍵を生成して署名し、**公開鍵の ECParameters だけを渡した側**で検証できる。
        /// 生成した側は秘密鍵も持ち、公開鍵だけの側は秘密鍵を持たない。
        /// </summary>
        [TestCase(EnumDigitalSignAlgorithm.ECDsaOpenSsl_P256, "SHA256")]
        [TestCase(EnumDigitalSignAlgorithm.ECDsaOpenSsl_P384, "SHA384")]
        [TestCase(EnumDigitalSignAlgorithm.ECDsaOpenSsl_P521, "SHA512")]
        public void SignAndVerify(EnumDigitalSignAlgorithm eaa, string hashName)
        {
            DigitalSignECDsaOpenSsl signer = new DigitalSignECDsaOpenSsl(eaa, CreateHash(hashName));
            Assert.That(signer.AsymmetricAlgorithm, Is.InstanceOf<ECDsaOpenSsl>());
            Assert.That(signer.PrivateKey.HasValue && signer.PrivateKey.Value.D != null, Is.True, "生成した側は秘密鍵を持つ");

            byte[] sign = signer.Sign(Data);
            Assert.That(signer.Verify(Data, sign), Is.True);
            Assert.That(signer.Verify(Tampered, sign), Is.False);

            DigitalSignECDsaOpenSsl verifier = new DigitalSignECDsaOpenSsl(signer.PublicKey.Value, CreateHash(hashName));
            Assert.That(verifier.PrivateKey, Is.Null, "公開鍵だけの側は秘密鍵を持たない");
            Assert.That(verifier.Verify(Data, sign), Is.True);
            Assert.That(verifier.Verify(Tampered, sign), Is.False);
        }

        /// <summary>秘密鍵の ECParameters を渡すと、同じ鍵で署名できる（元の側で検証できる）。</summary>
        [Test]
        public void ImportPrivateKey()
        {
            DigitalSignECDsaOpenSsl generated = new DigitalSignECDsaOpenSsl(EnumDigitalSignAlgorithm.ECDsaOpenSsl_P256, SHA256.Create());
            DigitalSignECDsaOpenSsl imported = new DigitalSignECDsaOpenSsl(generated.PrivateKey.Value, SHA256.Create());

            Assert.That(imported.PrivateKey, Is.Not.Null);
            Assert.That(generated.Verify(Data, imported.Sign(Data)), Is.True);
        }

        /// <summary>Formatter の版は実装していない（NotImplementedException）。</summary>
        [Test]
        public void Formatter_NotImplemented()
        {
            DigitalSignECDsaOpenSsl ds = new DigitalSignECDsaOpenSsl(EnumDigitalSignAlgorithm.ECDsaOpenSsl_P256, SHA256.Create());

            Assert.Throws<NotImplementedException>(() => ds.SignByFormatter(Data));
            Assert.Throws<NotImplementedException>(() => ds.VerifyByDeformatter(Data, new byte[0]));
        }

        /// <summary>ハッシュ名から HashAlgorithm を作る</summary>
        private static HashAlgorithm CreateHash(string name)
        {
            switch (name)
            {
                case "SHA384": return SHA384.Create();
                case "SHA512": return SHA512.Create();
                default: return SHA256.Create();
            }
        }
    }
#pragma warning restore CA1416
}
#endif

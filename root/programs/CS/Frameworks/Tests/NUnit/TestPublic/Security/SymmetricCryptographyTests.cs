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
//* クラス名        ：SymmetricCryptographyTests
//* クラス日本語名  ：SymmetricCryptography（共通鍵暗号）の単体テスト
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
    /// <summary>SymmetricCryptography（共通鍵暗号）の単体テスト</summary>
    /// <remarks>
    /// **鍵と初期化ベクタは、パスワードから PBKDF2-HMAC-SHA1 で、共有キー → 初期化ベクタの順に続けて取り出す。**
    /// これを変えると既存の暗号文を復号できなくなる（Public/Security/ANALYSIS.md 6 節、#608）。
    /// ここでは同じ導出を .NET の API で組み立て、暗号文が一致することを確かめる。
    /// 後退テストは Golden の EncAndDecUtilCUI にもある。
    /// </remarks>
    [TestFixture]
    public class SymmetricCryptographyTests
    {
        /// <summary>平文（日本語を含む）</summary>
        private const string Plain = "こんにちは Open棟梁 123";

        /// <summary>パスワード</summary>
        private const string Password = "p@ssw0rd";

        /// <summary>同じ導出（PBKDF2-HMAC-SHA1、鍵 → IV の順）と、.NET の既定のモード（CBC / PKCS7）で暗号化する。</summary>
        private static byte[] EncryptByBcl(SymmetricAlgorithm sa, byte[] plain, string password, byte[] salt, int stretching)
        {
            using (Rfc2898DeriveBytes db = new Rfc2898DeriveBytes(password, salt, stretching, HashAlgorithmName.SHA1))
            {
                sa.Key = db.GetBytes(sa.KeySize / 8);
                sa.IV = db.GetBytes(sa.BlockSize / 8);
            }

            using (ICryptoTransform t = sa.CreateEncryptor())
            {
                return t.TransformFinalBlock(plain, 0, plain.Length);
            }
        }

        #region 往復

        /// <summary>アルゴリズムごとに、文字列で暗号化して戻せる。</summary>
        [TestCase(EnumSymmetricAlgorithm.AES)]
        [TestCase(EnumSymmetricAlgorithm.TDES)]
        [TestCase(EnumSymmetricAlgorithm.DES)]
        [TestCase(EnumSymmetricAlgorithm.RC2)]
        [TestCase(EnumSymmetricAlgorithm.AES_CNG)]
        [TestCase(EnumSymmetricAlgorithm.TDES_CNG)]
        public void EncryptString_RoundTrip(EnumSymmetricAlgorithm esa)
        {
            SymmetricCryptography sc = new SymmetricCryptography(esa);

            string cipher = sc.EncryptString(Plain, Password);

            Assert.That(cipher, Is.Not.EqualTo(Plain));
            Assert.That(sc.DecryptString(cipher, Password), Is.EqualTo(Plain));
        }

        /// <summary>バイト列でも暗号化して戻せる。</summary>
        [Test]
        public void EncryptBytes_RoundTrip()
        {
            byte[] plain = new byte[] { 0, 1, 2, 3, 250, 251, 252, 253, 254, 255 };
            SymmetricCryptography sc = new SymmetricCryptography();

            Assert.That(sc.DecryptBytes(sc.EncryptBytes(plain, Password), Password), Is.EqualTo(plain));
        }

        /// <summary>モードの指定（CBC / CFB / ECB）とパディングの指定でも、暗号化して戻せる。</summary>
#pragma warning disable CS0618 // CipherMode_ECB は非推奨だが、指定された場合の動作（下位互換）を確かめる。
        [TestCase(EnumSymmetricAlgorithm.AES | EnumSymmetricAlgorithm.CipherMode_CBC | EnumSymmetricAlgorithm.PaddingMode_PKCS7)]
        [TestCase(EnumSymmetricAlgorithm.AES | EnumSymmetricAlgorithm.CipherMode_CFB)]
        [TestCase(EnumSymmetricAlgorithm.AES | EnumSymmetricAlgorithm.CipherMode_ECB)]
        [TestCase(EnumSymmetricAlgorithm.TDES | EnumSymmetricAlgorithm.CipherMode_CFB)]
        [TestCase(EnumSymmetricAlgorithm.AES | EnumSymmetricAlgorithm.PaddingMode_ANSIX923)]
        [TestCase(EnumSymmetricAlgorithm.AES | EnumSymmetricAlgorithm.PaddingMode_ISO10126)]
#pragma warning restore CS0618
        public void ModeAndPadding_RoundTrip(EnumSymmetricAlgorithm esa)
        {
            SymmetricCryptography sc = new SymmetricCryptography(esa);

            Assert.That(sc.DecryptString(sc.EncryptString(Plain, Password), Password), Is.EqualTo(Plain));
        }

        /// <summary>
        /// パディング無し・ゼロ埋めは、**ブロック長の倍数のデータだけ**を戻せる（ゼロ埋めは末尾のゼロが残る）。
        /// パディング無しで倍数でなければ CryptographicException。
        /// </summary>
        [Test]
        public void PaddingNoneAndZeros()
        {
            byte[] block = Encoding.ASCII.GetBytes("0123456789abcdef"); // AES のブロック長（16 バイト）
            SymmetricCryptography none = new SymmetricCryptography(EnumSymmetricAlgorithm.AES | EnumSymmetricAlgorithm.PaddingMode_None);
            SymmetricCryptography zeros = new SymmetricCryptography(EnumSymmetricAlgorithm.AES | EnumSymmetricAlgorithm.PaddingMode_Zeros);

            Assert.That(none.DecryptBytes(none.EncryptBytes(block, Password), Password), Is.EqualTo(block));
            Assert.That(zeros.DecryptBytes(zeros.EncryptBytes(block, Password), Password), Is.EqualTo(block));
            Assert.Throws<CryptographicException>(() => none.EncryptBytes(new byte[] { 1, 2, 3 }, Password));
        }

        /// <summary>.NET が対応していないモード（CTS / OFB）は CryptographicException。</summary>
        [TestCase(EnumSymmetricAlgorithm.AES | EnumSymmetricAlgorithm.CipherMode_CTS)]
        [TestCase(EnumSymmetricAlgorithm.AES | EnumSymmetricAlgorithm.CipherMode_OFB)]
        public void UnsupportedMode_Throws(EnumSymmetricAlgorithm esa)
        {
            SymmetricCryptography sc = new SymmetricCryptography(esa);

            Assert.Throws<CryptographicException>(() => sc.EncryptString(Plain, Password));
        }

        /// <summary>アルゴリズムを 1 つも指定しなければ、暗号化の時点で ArgumentException。</summary>
        [Test]
        public void NoAlgorithm_Throws()
        {
            SymmetricCryptography sc = new SymmetricCryptography((EnumSymmetricAlgorithm)0);

            Assert.Throws<ArgumentException>(() => sc.EncryptString(Plain, Password));
        }

        #endregion

        #region 鍵の導出（#608）

        /// <summary>
        /// **既定（AES、既定のソルトと回数）の暗号文は、同じ導出を .NET の API で組み立てたものと一致する。**
        /// net48 と net10.0 で同じ暗号文になる（#608 の前は net10.0 だけ食い違っていた）。
        /// </summary>
        [Test]
        public void Default_SameAsPbkdf2Sha1()
        {
            byte[] plain = Encoding.UTF8.GetBytes(Plain);
            byte[] expected;
            using (Aes aes = Aes.Create())
            {
                expected = EncryptByBcl(aes, plain, Password,
                    SymmetricCryptography.DefaultSalt, SymmetricCryptography.DefaultStretching);
            }

            Assert.That(new SymmetricCryptography().EncryptString(Plain, Password), Is.EqualTo(Convert.ToBase64String(expected)));
        }

        /// <summary>ソルトと回数を指定した場合も、同じ導出で暗号化する（TripleDES）。</summary>
        [Test]
        public void SaltAndStretching_SameAsPbkdf2Sha1()
        {
            byte[] plain = Encoding.UTF8.GetBytes(Plain);
            byte[] salt = Encoding.UTF8.GetBytes("salt-for-nunit");
            byte[] expected;
            using (TripleDES tdes = TripleDES.Create())
            {
                expected = EncryptByBcl(tdes, plain, Password, salt, 1000);
            }

            SymmetricCryptography sc = new SymmetricCryptography(EnumSymmetricAlgorithm.TDES, salt, 1000);

            Assert.That(sc.EncryptBytes(plain, Password), Is.EqualTo(expected));
            Assert.That(sc.DecryptBytes(expected, Password), Is.EqualTo(plain));
        }

        /// <summary>ソルトか回数が違えば、暗号文も違う。</summary>
        [Test]
        public void DifferentSaltOrStretching_DifferentCipher()
        {
            byte[] salt = Encoding.UTF8.GetBytes("salt-for-nunit");
            string a = new SymmetricCryptography(EnumSymmetricAlgorithm.AES, salt, 10).EncryptString(Plain, Password);
            string b = new SymmetricCryptography(EnumSymmetricAlgorithm.AES, Encoding.UTF8.GetBytes("other-salt-xyz"), 10).EncryptString(Plain, Password);
            string c = new SymmetricCryptography(EnumSymmetricAlgorithm.AES, salt, 11).EncryptString(Plain, Password);

            Assert.That(b, Is.Not.EqualTo(a));
            Assert.That(c, Is.Not.EqualTo(a));
        }

        #endregion
    }
}

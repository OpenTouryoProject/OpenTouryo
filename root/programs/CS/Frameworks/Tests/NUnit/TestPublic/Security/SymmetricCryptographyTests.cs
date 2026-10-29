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
//*  2026/10/28  玄人 幸道         モード・パディングの指定が効いていることを、.NET の API で組み直して確かめるテストを追加
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Text;

using NUnit.Framework;

using TestCommon;

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

        /// <summary>同じ導出（PBKDF2-HMAC-SHA1、鍵 → IV の順）で、sa に鍵と IV を設定する。</summary>
        private static void DeriveByBcl(SymmetricAlgorithm sa, string password, byte[] salt, int stretching)
        {
            using (Rfc2898DeriveBytes db = new Rfc2898DeriveBytes(password, salt, stretching, HashAlgorithmName.SHA1))
            {
                sa.Key = db.GetBytes(sa.KeySize / 8);
                sa.IV = db.GetBytes(sa.BlockSize / 8);
            }
        }

        /// <summary>同じ導出で暗号化する。モードとパディングは sa に設定したもの（既定は CBC / PKCS7）。</summary>
        private static byte[] EncryptByBcl(SymmetricAlgorithm sa, byte[] plain, string password, byte[] salt, int stretching)
        {
            DeriveByBcl(sa, password, salt, stretching);

            using (ICryptoTransform t = sa.CreateEncryptor())
            {
                return t.TransformFinalBlock(plain, 0, plain.Length);
            }
        }

        /// <summary>既定のソルトと回数で導出し、AES（CBC）・パディング無しで復号する（詰めたバイトがそのまま残る）。</summary>
        private static byte[] DecryptByBclWithoutPadding(byte[] cipher)
        {
            using (Aes aes = Aes.Create())
            {
                DeriveByBcl(aes, Password, SymmetricCryptography.DefaultSalt, SymmetricCryptography.DefaultStretching);
                aes.Padding = PaddingMode.None;

                using (ICryptoTransform t = aes.CreateDecryptor())
                {
                    return t.TransformFinalBlock(cipher, 0, cipher.Length);
                }
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
        [Basis("設計：Public/Security/ANALYSIS.md 6 節（PBKDF2-HMAC-SHA1。共有キー → 初期化ベクタの順に、同じ導出から続けて取り出す）")]
        [Basis("別実装：.NET の Rfc2898DeriveBytes（SHA-1）と Aes で組み立てた暗号文と一致")]
        [Basis("#608")]
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
        [Basis("設計：Public/Security/ANALYSIS.md 6 節（PBKDF2-HMAC-SHA1。共有キー → 初期化ベクタの順に、同じ導出から続けて取り出す）")]
        [Basis("別実装：.NET の Rfc2898DeriveBytes（SHA-1）と TripleDES で組み立てた暗号文と一致")]
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

        #region モードとパディング（.NET の API で組み直して比べる）

        // 往復（ModeAndPadding_RoundTrip）だけでは、指定を無視して既定（CBC / PKCS7）で
        // 暗号化・復号しても通ってしまう。指定が暗号文に効いていることは、値で確かめる。

        /// <summary>
        /// **モードの指定は暗号文に効いている。** 同じ導出・同じモードで .NET の API が作る暗号文と一致する。
        /// CFB のフィードバック サイズは、どちらも .NET の既定のまま。
        /// </summary>
#pragma warning disable CS0618 // CipherMode_ECB は非推奨だが、指定された場合の動作（下位互換）を確かめる。
        [TestCase(EnumSymmetricAlgorithm.AES | EnumSymmetricAlgorithm.CipherMode_CBC, CipherMode.CBC)]
        [TestCase(EnumSymmetricAlgorithm.AES | EnumSymmetricAlgorithm.CipherMode_CFB, CipherMode.CFB)]
        [TestCase(EnumSymmetricAlgorithm.AES | EnumSymmetricAlgorithm.CipherMode_ECB, CipherMode.ECB)]
        [TestCase(EnumSymmetricAlgorithm.TDES | EnumSymmetricAlgorithm.CipherMode_CFB, CipherMode.CFB)]
#pragma warning restore CS0618
        [Basis("別実装：.NET の Aes・TripleDES に同じモードを設定して作った暗号文と一致")]
        public void Mode_SameAsBcl(EnumSymmetricAlgorithm esa, CipherMode mode)
        {
            byte[] plain = Encoding.UTF8.GetBytes(Plain);
            byte[] expected;
            using (SymmetricAlgorithm sa = esa.HasFlag(EnumSymmetricAlgorithm.TDES) ? (SymmetricAlgorithm)TripleDES.Create() : Aes.Create())
            {
                sa.Mode = mode;
                expected = EncryptByBcl(sa, plain, Password,
                    SymmetricCryptography.DefaultSalt, SymmetricCryptography.DefaultStretching);
            }

            Assert.That(new SymmetricCryptography(esa).EncryptBytes(plain, Password), Is.EqualTo(expected));
        }

        /// <summary>
        /// **パディングの指定は、末尾の詰め方に効いている。** パディング無しで復号し、詰めたバイトを規格どおりか見る。
        /// 平文 5 バイトを AES（16 バイト）で暗号化するので、11（0x0B）バイト詰める。
        /// PKCS7 は 0x0B を 11 個、ANSI X9.23 は 0x00 を 10 個の後に 0x0B、Zeros は 0x00 を 11 個。
        /// </summary>
        /// <remarks>
        /// 根拠はケースごとに違うので、属性（Basis）にせずここに書く（QUALITY.md 7 節）。
        /// ・PKCS7 … RFC 5652 6.3（k-(lth mod k) 個のバイトを、どれも k-(lth mod k) の値で詰める）
        /// ・ANSI X9.23・Zeros … .NET の PaddingMode の説明による（規格の原典は確かめていない。観測）
        /// </remarks>
        [TestCase(EnumSymmetricAlgorithm.PaddingMode_PKCS7, (byte)0x0B, (byte)0x0B)]
        [TestCase(EnumSymmetricAlgorithm.PaddingMode_ANSIX923, (byte)0x00, (byte)0x0B)]
        [TestCase(EnumSymmetricAlgorithm.PaddingMode_Zeros, (byte)0x00, (byte)0x00)]
        public void Padding_TailAsSpecified(EnumSymmetricAlgorithm padding, byte fill, byte last)
        {
            byte[] plain = new byte[] { 1, 2, 3, 4, 5 };
            byte[] cipher = new SymmetricCryptography(EnumSymmetricAlgorithm.AES | padding).EncryptBytes(plain, Password);

            byte[] expected = new byte[16];
            Array.Copy(plain, expected, plain.Length);
            for (int i = plain.Length; i < 15; i++) { expected[i] = fill; }
            expected[15] = last;

            Assert.That(DecryptByBclWithoutPadding(cipher), Is.EqualTo(expected));
        }

        /// <summary>
        /// ISO 10126 は、最後の 1 バイトが詰めた長さで、残りは乱数。
        /// **乱数なので、同じ鍵・IV・平文でも暗号文が毎回違う**（PKCS7 等なら同じになる）。
        /// </summary>
        [Test]
        public void Padding_Iso10126_LastByteIsLengthAndRestIsRandom()
        {
            byte[] plain = new byte[] { 1, 2, 3, 4, 5 };
            SymmetricCryptography sc = new SymmetricCryptography(EnumSymmetricAlgorithm.AES | EnumSymmetricAlgorithm.PaddingMode_ISO10126);

            byte[] a = sc.EncryptBytes(plain, Password);
            byte[] b = sc.EncryptBytes(plain, Password);
            byte[] raw = DecryptByBclWithoutPadding(a);

            Assert.That(raw.Length, Is.EqualTo(16));
            Assert.That(raw[15], Is.EqualTo(0x0B));
            Assert.That(b, Is.Not.EqualTo(a));
        }

        #endregion
    }
}

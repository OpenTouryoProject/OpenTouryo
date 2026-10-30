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
//* クラス名        ：GetPasswordHashV2Tests
//* クラス日本語名  ：GetPasswordHashV2（ソルト付きのパスワード ハッシュ）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//*  2026/10/28  玄人 幸道         キー無し・キー付きの形式とストレッチを、.NET の API で組み直して確かめるテストを追加
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Text;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Pwd;

namespace TestPublic.Security.Pwd
{
    /// <summary>GetPasswordHashV2（ソルト付きのパスワード ハッシュ）の単体テスト</summary>
    /// <remarks>
    /// ソルトは乱数なので、作るたびに結果が違う。**作ったものを照合して一致すること**を確かめる。
    /// V1 で作ったものは V2 では照合できない（仕様。Public/Security/ANALYSIS.md 5 節）。
    /// 後退テストは Golden の EncAndDecUtilCUI にもある。
    /// </remarks>
    [TestFixture]
    public class GetPasswordHashV2Tests
    {
        /// <summary>パスワード</summary>
        private const string Raw = "p@ssw0rd 棟梁";

        #region ハッシュ（キー無し）

        /// <summary>作ったものは、同じパスワードで一致し、違うパスワードでは一致しない。</summary>
        [TestCase(EnumHashAlgorithm.Default)]
        [TestCase(EnumHashAlgorithm.MD5)]
        [TestCase(EnumHashAlgorithm.SHA1)]
        [TestCase(EnumHashAlgorithm.SHA256)]
        [TestCase(EnumHashAlgorithm.SHA384)]
        [TestCase(EnumHashAlgorithm.SHA512)]
        [TestCase(EnumHashAlgorithm.RIPEMD160_M)]
        public void Hash_RoundTrip(EnumHashAlgorithm eha)
        {
            string salted = GetPasswordHashV2.GetSaltedPassword(Raw, eha, 16);

            Assert.That(GetPasswordHashV2.EqualSaltedPassword(Raw, salted, eha), Is.True);
            Assert.That(GetPasswordHashV2.EqualSaltedPassword(Raw + "x", salted, eha), Is.False);
        }

        /// <summary>ストレッチ回数を指定しても照合できる（回数は結果に含まれる）。0 回でもよい。</summary>
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(100)]
        public void Hash_Stretch_RoundTrip(int stretch)
        {
            string salted = GetPasswordHashV2.GetSaltedPassword(Raw, EnumHashAlgorithm.SHA256, 16, stretch);

            Assert.That(GetPasswordHashV2.EqualSaltedPassword(Raw, salted, EnumHashAlgorithm.SHA256), Is.True);
        }

        /// <summary>ソルトは乱数なので、同じパスワードでも作るたびに違う。照合はどちらも一致する。</summary>
        [Test]
        public void Hash_SaltIsRandom()
        {
            string a = GetPasswordHashV2.GetSaltedPassword(Raw, EnumHashAlgorithm.SHA256, 16);
            string b = GetPasswordHashV2.GetSaltedPassword(Raw, EnumHashAlgorithm.SHA256, 16);

            Assert.That(b, Is.Not.EqualTo(a));
            Assert.That(GetPasswordHashV2.EqualSaltedPassword(Raw, b, EnumHashAlgorithm.SHA256), Is.True);
        }

        /// <summary>作ったときと違うアルゴリズムでは一致しない。</summary>
        [Test]
        public void Hash_DifferentAlgorithm_NotEqual()
        {
            string salted = GetPasswordHashV2.GetSaltedPassword(Raw, EnumHashAlgorithm.SHA256, 16);

            Assert.That(GetPasswordHashV2.EqualSaltedPassword(Raw, salted, EnumHashAlgorithm.SHA512), Is.False);
        }

        /// <summary>
        /// **形式と計算を、.NET の API だけで組み直して一致する。** 往復だけでは、回数を無視しても通ってしまうため。
        /// 形式は「Base64(ソルト).Base64(回数).Base64(ハッシュの Base64 文字列)」（それぞれ UTF-8）。
        /// ハッシュは UTF-8 の「ソルト＋パスワード」に 1 回かけ、さらに回数分かける（GetHash のストレッチと同じ。GetHashTests）。
        /// </summary>
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        [Basis("別実装：.NET の SHA256 で、ソルト＋パスワードに 1 回＋回数分かけて組み直したハッシュと一致")]
        public void Hash_SameAsBcl(int stretch)
        {
            string[] parts = GetPasswordHashV2.GetSaltedPassword(Raw, EnumHashAlgorithm.SHA256, 16, stretch).Split('.');

            Assert.That(parts.Length, Is.EqualTo(3));
            string salt = Encoding.UTF8.GetString(Convert.FromBase64String(parts[0]));
            Assert.That(salt.Length, Is.EqualTo(16));
            Assert.That(Encoding.UTF8.GetString(Convert.FromBase64String(parts[1])), Is.EqualTo(stretch.ToString()));

            byte[] hash;
            using (SHA256 sha = SHA256.Create())
            {
                hash = sha.ComputeHash(Encoding.UTF8.GetBytes(salt + Raw));
                for (int i = 0; i < stretch; i++) { hash = sha.ComputeHash(hash); }
            }
            Assert.That(Encoding.UTF8.GetString(Convert.FromBase64String(parts[2])), Is.EqualTo(Convert.ToBase64String(hash)));
        }

        /// <summary>照合は、保存された回数を使う。回数の欄だけを変えると一致しない。</summary>
        [Test]
        public void Hash_EqualUsesStoredStretch()
        {
            string[] parts = GetPasswordHashV2.GetSaltedPassword(Raw, EnumHashAlgorithm.SHA256, 16, 3).Split('.');
            parts[1] = Convert.ToBase64String(Encoding.UTF8.GetBytes("2"));

            Assert.That(GetPasswordHashV2.EqualSaltedPassword(Raw, string.Join(".", parts), EnumHashAlgorithm.SHA256), Is.False);
        }

        #endregion

        #region ハッシュ（キー付き）

        /// <summary>キー付きでも、作ったものは同じパスワードで一致し、違うパスワードでは一致しない。</summary>
        [TestCase(EnumKeyedHashAlgorithm.Default)]
        [TestCase(EnumKeyedHashAlgorithm.HMACMD5)]
        [TestCase(EnumKeyedHashAlgorithm.HMACSHA1)]
        [TestCase(EnumKeyedHashAlgorithm.HMACSHA256)]
        [TestCase(EnumKeyedHashAlgorithm.HMACSHA384)]
        [TestCase(EnumKeyedHashAlgorithm.HMACSHA512)]
        [TestCase(EnumKeyedHashAlgorithm.HMACRIPEMD160)]
        [TestCase(EnumKeyedHashAlgorithm.MACTripleDES)]
        public void KeyedHash_RoundTrip(EnumKeyedHashAlgorithm ekha)
        {
            string salted = GetPasswordHashV2.GetSaltedPassword(Raw, ekha, "nunit-key", 16, 10);

            Assert.That(GetPasswordHashV2.EqualSaltedPassword(Raw, salted, ekha), Is.True);
            Assert.That(GetPasswordHashV2.EqualSaltedPassword(Raw + "x", salted, ekha), Is.False);
        }

        /// <summary>
        /// 鍵の導出のハッシュ（HashAlgorithmName）は、作るときと照合するときで揃える。既定は SHA256。
        /// 違えば一致しない。
        /// </summary>
        [Test]
        public void KeyedHash_HashAlgorithmName()
        {
            string sha512 = GetPasswordHashV2.GetSaltedPassword(
                Raw, EnumKeyedHashAlgorithm.HMACSHA256, "nunit-key", 16, 10, HashAlgorithmName.SHA512);
            string byDefault = GetPasswordHashV2.GetSaltedPassword(Raw, EnumKeyedHashAlgorithm.HMACSHA256, "nunit-key", 16, 10);

            Assert.That(GetPasswordHashV2.EqualSaltedPassword(Raw, sha512, EnumKeyedHashAlgorithm.HMACSHA256, HashAlgorithmName.SHA512), Is.True);
            Assert.That(GetPasswordHashV2.EqualSaltedPassword(Raw, sha512, EnumKeyedHashAlgorithm.HMACSHA256), Is.False);
            Assert.That(GetPasswordHashV2.EqualSaltedPassword(Raw, byDefault, EnumKeyedHashAlgorithm.HMACSHA256, HashAlgorithmName.SHA256), Is.True);
        }

        /// <summary>
        /// **形式と計算を、.NET の API だけで組み直して一致する。**
        /// 形式は「Base64(キー).Base64(ソルト).Base64(回数).Base64(ハッシュ)」（キー・回数は UTF-8）。
        /// 照合（EqualSaltedPassword）が文字列だけから計算し直せるよう、キーも含めている。
        /// ハッシュは、キーとソルトから PBKDF2（回数・SHA256）で導いた 24 バイトの鍵による、UTF-8 の「ソルト＋パスワード」の HMAC
        /// （ストレッチのかかり方は、GetSaltedPassword の remarks のとおり）。
        /// </summary>
        [Test]
        [Basis("別実装：.NET の Rfc2898DeriveBytes（SHA256）で導いた鍵による HMACSHA256 と一致")]
        [Basis("設計：GetPasswordHashV2.GetSaltedPassword（キー付き）の remarks（ストレッチは、key と salt から PBKDF2 で導く HMAC の鍵にだけかかる）")]
        public void KeyedHash_SameAsBcl()
        {
            string[] parts = GetPasswordHashV2.GetSaltedPassword(
                Raw, EnumKeyedHashAlgorithm.HMACSHA256, "nunit-key", 16, 10).Split('.');

            Assert.That(parts.Length, Is.EqualTo(4));
            Assert.That(Encoding.UTF8.GetString(Convert.FromBase64String(parts[0])), Is.EqualTo("nunit-key"));
            byte[] salt = Convert.FromBase64String(parts[1]);
            Assert.That(Encoding.UTF8.GetString(Convert.FromBase64String(parts[2])), Is.EqualTo("10"));

            byte[] derived;
            using (Rfc2898DeriveBytes db = new Rfc2898DeriveBytes("nunit-key", salt, 10, HashAlgorithmName.SHA256))
            {
                derived = db.GetBytes(24);
            }
            byte[] expected;
            using (HMACSHA256 hmac = new HMACSHA256(derived))
            {
                expected = hmac.ComputeHash(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(salt) + Raw));
            }
            Assert.That(parts[3], Is.EqualTo(Convert.ToBase64String(expected)));
        }

        /// <summary>キー付きも、照合は保存された回数を使う。回数の欄だけを変えると一致しない。</summary>
        [Test]
        public void KeyedHash_EqualUsesStoredStretch()
        {
            string[] parts = GetPasswordHashV2.GetSaltedPassword(
                Raw, EnumKeyedHashAlgorithm.HMACSHA256, "nunit-key", 16, 10).Split('.');
            parts[2] = Convert.ToBase64String(Encoding.UTF8.GetBytes("9"));

            Assert.That(GetPasswordHashV2.EqualSaltedPassword(Raw, string.Join(".", parts), EnumKeyedHashAlgorithm.HMACSHA256), Is.False);
        }

        /// <summary>キー付きは、鍵の導出（PBKDF2）の回数が 1 以上でなければならない。0 なら ArgumentOutOfRangeException。</summary>
        [Test]
        public void KeyedHash_ZeroStretch_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                GetPasswordHashV2.GetSaltedPassword(Raw, EnumKeyedHashAlgorithm.HMACSHA256, "nunit-key", 16, 0));
        }

        #endregion
    }
}

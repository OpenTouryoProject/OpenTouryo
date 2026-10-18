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
//* クラス名        ：GetHashTests
//* クラス日本語名  ：GetHash（ハッシュ（キー無し））の単体テスト
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
    /// <summary>GetHash（ハッシュ（キー無し））の単体テスト</summary>
    /// <remarks>
    /// 期待値は、公表されているテスト ベクタ（"abc"）を使う。
    /// RIPEMD160 は net48 が .NET の RIPEMD160Managed、net10.0 が BouncyCastle で、実装が違う。
    /// 後退テストは Golden の EncAndDecUtilCUI にもある。
    /// </remarks>
    [TestFixture]
    public class GetHashTests
    {
        /// <summary>"abc"</summary>
        private static readonly byte[] Abc = Encoding.UTF8.GetBytes("abc");

        /// <summary>16 進の小文字</summary>
        private static string Hex(byte[] b)
        {
            return BitConverter.ToString(b).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>"abc" のハッシュ（テスト ベクタ）。ストレッチ無し（1 回）。</summary>
        [TestCase(EnumHashAlgorithm.MD5, "900150983cd24fb0d6963f7d28e17f72")]
        [TestCase(EnumHashAlgorithm.SHA1, "a9993e364706816aba3e25717850c26c9cd0d89d")]
        [TestCase(EnumHashAlgorithm.SHA256, "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
        [TestCase(EnumHashAlgorithm.SHA384, "cb00753f45a35e8bb5a03d699ac65007272c32ab0eded1631a8b605a43ff5bed8086072ba1e7cc2358baeca134c825a7")]
        [TestCase(EnumHashAlgorithm.SHA512, "ddaf35a193617abacc417349ae20413112e6fa4e89a97ea20a9eeee64b55d39a2192992a274fc1a836ba3c23a3feebbd454d4423643ce80e2a9ac94fa54ca49f")]
        [TestCase(EnumHashAlgorithm.RIPEMD160_M, "8eb208f7e05d987a9b044a8e98c6b087f15a0bfc")]
        // 既定は SHA256
        [TestCase(EnumHashAlgorithm.Default, "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
        public void GetHashBytes_TestVector(EnumHashAlgorithm eha, string expected)
        {
            Assert.That(Hex(GetHash.GetHashBytes(Abc, eha)), Is.EqualTo(expected));
        }

#if !NETCOREAPP
        /// <summary>CNG 版は net48 だけ（net10.0 では列挙に無い）。CSP 版と同じ値。</summary>
        [TestCase(EnumHashAlgorithm.MD5_CNG, EnumHashAlgorithm.MD5)]
        [TestCase(EnumHashAlgorithm.SHA1_CNG, EnumHashAlgorithm.SHA1)]
        [TestCase(EnumHashAlgorithm.SHA256_CNG, EnumHashAlgorithm.SHA256)]
        [TestCase(EnumHashAlgorithm.SHA384_CNG, EnumHashAlgorithm.SHA384)]
        [TestCase(EnumHashAlgorithm.SHA512_CNG, EnumHashAlgorithm.SHA512)]
        public void GetHashBytes_Cng_SameAsCsp(EnumHashAlgorithm cng, EnumHashAlgorithm csp)
        {
            Assert.That(GetHash.GetHashBytes(Abc, cng), Is.EqualTo(GetHash.GetHashBytes(Abc, csp)));
        }
#endif

        /// <summary>
        /// ストレッチ回数 n は、**最初の 1 回に加えて n 回**かける（合計 n + 1 回）。
        /// GetHashBytes_org はストレッチ 1（合計 2 回）。
        /// </summary>
        [TestCase(EnumHashAlgorithm.SHA256)]
        [TestCase(EnumHashAlgorithm.RIPEMD160_M)]
        public void GetHashBytes_Stretch(EnumHashAlgorithm eha)
        {
            byte[] once = GetHash.GetHashBytes(Abc, eha);
            byte[] twice = GetHash.GetHashBytes(once, eha);
            byte[] thrice = GetHash.GetHashBytes(twice, eha);

            Assert.That(GetHash.GetHashBytes(Abc, eha, 0), Is.EqualTo(once));
            Assert.That(GetHash.GetHashBytes(Abc, eha, 1), Is.EqualTo(twice));
            Assert.That(GetHash.GetHashBytes(Abc, eha, 2), Is.EqualTo(thrice));
            Assert.That(GetHash.GetHashBytes_org(Abc, eha), Is.EqualTo(twice));
        }

        /// <summary>
        /// **GetHashString(s, eha) はストレッチ 1（2 回かける）。** GetHashBytes(b, eha) はストレッチ 0（1 回）で、
        /// 同じ入力でも結果が違う（読み違えやすい）。文字列は UTF-8、結果は Base64。
        /// </summary>
        [Test]
        public void GetHashString_StretchesOnceByDefault()
        {
            byte[] once = GetHash.GetHashBytes(Abc, EnumHashAlgorithm.SHA256);
            byte[] twice = GetHash.GetHashBytes(once, EnumHashAlgorithm.SHA256);

            Assert.That(GetHash.GetHashString("abc", EnumHashAlgorithm.SHA256), Is.EqualTo(Convert.ToBase64String(twice)));
            Assert.That(GetHash.GetHashString("abc", EnumHashAlgorithm.SHA256, 0), Is.EqualTo(Convert.ToBase64String(once)));
        }

        /// <summary>日本語も UTF-8 でハッシュする（.NET の SHA256 と同じ値）。</summary>
        [Test]
        public void GetHashString_Utf8()
        {
            byte[] expected;
            using (SHA256 sha = SHA256.Create()) { expected = sha.ComputeHash(Encoding.UTF8.GetBytes("棟梁")); }

            Assert.That(GetHash.GetHashString("棟梁", EnumHashAlgorithm.SHA256, 0), Is.EqualTo(Convert.ToBase64String(expected)));
        }

#if NETCOREAPP
        /// <summary>GetDigestBytesByBC（net10.0 のみ）は BouncyCastle のダイジェストで計算する。</summary>
        [Test]
        public void GetDigestBytesByBC()
        {
            byte[] bc = GetHash.GetDigestBytesByBC(Abc, new Org.BouncyCastle.Crypto.Digests.Sha256Digest());

            Assert.That(Hex(bc), Is.EqualTo("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad"));
        }
#endif
    }
}

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
//* クラス名        ：GetKeyedHashTests
//* クラス日本語名  ：GetKeyedHash（ハッシュ（キー付き））の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Util;

namespace TestPublic.Security
{
    /// <summary>GetKeyedHash（ハッシュ（キー付き））の単体テスト</summary>
    /// <remarks>
    /// HMAC の期待値は RFC 2202（MD5 / SHA1）・RFC 4231（SHA-2）・RFC 2286（RIPEMD160）の Test Case 2 を使う
    /// （key = "Jefe", data = "what do ya want for nothing?"）。
    /// HMACRIPEMD160 と MACTripleDES は、net48 が .NET、net10.0 が BouncyCastle で、実装が違う。
    /// 後退テストは Golden の EncAndDecUtilCUI にもある。
    /// </remarks>
    [TestFixture]
    public class GetKeyedHashTests
    {
        /// <summary>RFC の Test Case 2 の鍵</summary>
        private static readonly byte[] Jefe = Encoding.UTF8.GetBytes("Jefe");

        /// <summary>RFC の Test Case 2 のデータ</summary>
        private const string Data = "what do ya want for nothing?";

        /// <summary>16 進の小文字</summary>
        private static string Hex(byte[] b)
        {
            return BitConverter.ToString(b).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>HMAC のテスト ベクタ（RFC の Test Case 2）。既定は HMACSHA1。</summary>
        [TestCase(EnumKeyedHashAlgorithm.HMACMD5, "750c783e6ab0b503eaa86e310a5db738")]
        [TestCase(EnumKeyedHashAlgorithm.HMACSHA1, "effcdf6ae5eb2fa2d27416d5f184df9c259a7c79")]
        [TestCase(EnumKeyedHashAlgorithm.HMACSHA256, "5bdcc146bf60754e6a042426089575c75a003f089d2739839dec58b964ec3843")]
        [TestCase(EnumKeyedHashAlgorithm.HMACSHA384, "af45d2e376484031617f78d2b58a6b1b9c7ef464f5a01b47e42ec3736322445e8e2240ca5e69e2c78b3239ecfab21649")]
        [TestCase(EnumKeyedHashAlgorithm.HMACSHA512, "164b7a7bfcf819e2e395fbe73b56e0a387bd64222e831fd610270cd7ea2505549758bf75c05a994a6d034f65f8f0e6fdcaeab1a34d4a6b4b636e070a38bce737")]
        [TestCase(EnumKeyedHashAlgorithm.HMACRIPEMD160, "dda6c0213a485a9e24f4742064a7f033b43c4069")]
        [TestCase(EnumKeyedHashAlgorithm.Default, "effcdf6ae5eb2fa2d27416d5f184df9c259a7c79")]
        public void GetKeyedHashBytes_TestVector(EnumKeyedHashAlgorithm ekha, string expected)
        {
            Assert.That(Hex(GetKeyedHash.GetKeyedHashBytes(Encoding.UTF8.GetBytes(Data), ekha, Jefe)), Is.EqualTo(expected));
        }

        /// <summary>GetKeyedHashString は、データと鍵を UTF-8 にし、結果を Base64 にする。</summary>
        [Test]
        public void GetKeyedHashString_Base64()
        {
            byte[] bytes = GetKeyedHash.GetKeyedHashBytes(Encoding.UTF8.GetBytes(Data), EnumKeyedHashAlgorithm.HMACSHA256, Jefe);

            Assert.That(GetKeyedHash.GetKeyedHashString(Data, EnumKeyedHashAlgorithm.HMACSHA256, "Jefe"),
                Is.EqualTo(Convert.ToBase64String(bytes)));
        }

        /// <summary>
        /// MACTripleDES は 8 バイト。**net48（.NET）と net10.0（BouncyCastle）で同じ値になる。**
        /// 16 バイトの鍵は 2 鍵の 3DES で、先頭 8 バイトを後ろに足した 24 バイトの鍵と同じ値。
        /// </summary>
        [Test]
        public void MacTripleDes_SameOnBothRuntimes()
        {
            byte[] data = Encoding.UTF8.GetBytes(Data);
            byte[] k16 = Encoding.UTF8.GetBytes("0123456789abcdef");
            byte[] k24 = Encoding.UTF8.GetBytes("0123456789abcdef01234567");

            byte[] mac16 = GetKeyedHash.GetKeyedHashBytes(data, EnumKeyedHashAlgorithm.MACTripleDES, k16);
            byte[] mac24 = GetKeyedHash.GetKeyedHashBytes(data, EnumKeyedHashAlgorithm.MACTripleDES, k24);

            // 期待値は net48 と net10.0 の両方で実測して一致したもの（2026/10/18）
            Assert.That(Hex(mac16), Is.EqualTo("ef98f9f0077814a5"));
            Assert.That(mac24, Is.EqualTo(mac16));
        }

        /// <summary>
        /// MACTripleDES の鍵は 16 または 24 バイト。**24 バイトを超えたら ArrayOperator.ShortenByteArray で 24 に畳む**
        /// （切り詰めではない）。16 以上 24 未満は 16 に畳む。
        /// </summary>
        [Test]
        public void MacTripleDes_LongKey_Shortened()
        {
            byte[] data = Encoding.UTF8.GetBytes(Data);
            byte[] k32 = Encoding.UTF8.GetBytes("0123456789abcdef0123456789ABCDEF");
            byte[] k20 = Encoding.UTF8.GetBytes("0123456789abcdefWXYZ");

            Assert.That(GetKeyedHash.GetKeyedHashBytes(data, EnumKeyedHashAlgorithm.MACTripleDES, k32),
                Is.EqualTo(GetKeyedHash.GetKeyedHashBytes(data, EnumKeyedHashAlgorithm.MACTripleDES, ArrayOperator.ShortenByteArray(k32, 24))));
            Assert.That(GetKeyedHash.GetKeyedHashBytes(data, EnumKeyedHashAlgorithm.MACTripleDES, k20),
                Is.EqualTo(GetKeyedHash.GetKeyedHashBytes(data, EnumKeyedHashAlgorithm.MACTripleDES, ArrayOperator.ShortenByteArray(k20, 16))));
        }

        /// <summary>MACTripleDES の鍵が 16 バイト未満なら ArgumentException。</summary>
        [Test]
        public void MacTripleDes_ShortKey_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                GetKeyedHash.GetKeyedHashBytes(Encoding.UTF8.GetBytes(Data), EnumKeyedHashAlgorithm.MACTripleDES, new byte[15]));
        }

#if NETCOREAPP
        /// <summary>GetMacBytesByBC（net10.0 のみ）は BouncyCastle の MAC で計算する。</summary>
        [Test]
        public void GetMacBytesByBC()
        {
            byte[] mac = GetKeyedHash.GetMacBytesByBC(Encoding.UTF8.GetBytes(Data), Jefe,
                new Org.BouncyCastle.Crypto.Macs.HMac(new Org.BouncyCastle.Crypto.Digests.Sha256Digest()));

            Assert.That(Hex(mac), Is.EqualTo("5bdcc146bf60754e6a042426089575c75a003f089d2739839dec58b964ec3843"));
        }
#endif
    }
}

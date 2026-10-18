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
//* クラス名        ：HashAlgorithmCmnFuncTests
//* クラス日本語名  ：HashAlgorithmCmnFunc（ハッシュのサービス プロバイダの生成）の単体テスト
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

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security;

namespace TestPublic.Security
{
    /// <summary>HashAlgorithmCmnFunc（ハッシュのサービス プロバイダの生成）の単体テスト</summary>
    [TestFixture]
    public class HashAlgorithmCmnFuncTests
    {
        #region 名前

        /// <summary>HashAlgorithm から名前（HashNameConst）を引く。</summary>
        [Test]
        public void GetHashAlgorithmName()
        {
            Assert.That(HashAlgorithmCmnFunc.GetHashAlgorithmName(MD5.Create()), Is.EqualTo(HashNameConst.MD5));
            Assert.That(HashAlgorithmCmnFunc.GetHashAlgorithmName(SHA1.Create()), Is.EqualTo(HashNameConst.SHA1));
            Assert.That(HashAlgorithmCmnFunc.GetHashAlgorithmName(SHA256.Create()), Is.EqualTo(HashNameConst.SHA256));
            Assert.That(HashAlgorithmCmnFunc.GetHashAlgorithmName(SHA384.Create()), Is.EqualTo(HashNameConst.SHA384));
            Assert.That(HashAlgorithmCmnFunc.GetHashAlgorithmName(SHA512.Create()), Is.EqualTo(HashNameConst.SHA512));
        }

        /// <summary>MD5 / SHA 系以外（HMAC など）は ArgumentException。</summary>
        [Test]
        public void GetHashAlgorithmName_Other_Throws()
        {
            Assert.Throws<ArgumentException>(() => HashAlgorithmCmnFunc.GetHashAlgorithmName(new HMACSHA256()));
        }

        /// <summary>名前から HashAlgorithm を作る。名前を省くと SHA256。</summary>
        [Test]
        public void GetHashAlgorithmFromNameString()
        {
            Assert.That(HashAlgorithmCmnFunc.GetHashAlgorithmFromNameString(), Is.InstanceOf<SHA256>());
            Assert.That(HashAlgorithmCmnFunc.GetHashAlgorithmFromNameString(HashNameConst.SHA512), Is.InstanceOf<SHA512>());
            Assert.That(HashAlgorithmCmnFunc.GetHashAlgorithmFromNameString(HashNameConst.MD5), Is.InstanceOf<MD5>());
        }

        #endregion

        #region キー無し

        /// <summary>列挙値に対応するハッシュを作る。既定（Default）は SHA256。</summary>
        [Test]
        public void CreateHashAlgorithmSP()
        {
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.Default), Is.InstanceOf<SHA256>());
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.MD5), Is.InstanceOf<MD5>());
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.SHA1), Is.InstanceOf<SHA1>());
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.SHA256), Is.InstanceOf<SHA256>());
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.SHA384), Is.InstanceOf<SHA384>());
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.SHA512), Is.InstanceOf<SHA512>());
        }

        /// <summary>**列挙に無い値は、例外にならずに既定（SHA256）になる。**</summary>
        [Test]
        public void CreateHashAlgorithmSP_Unknown_DefaultsToSha256()
        {
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP((EnumHashAlgorithm)999), Is.InstanceOf<SHA256>());
        }

        /// <summary>
        /// RIPEMD160 は net48 だけ .NET の実装を返す。**net10.0 は null**（GetHash が BouncyCastle で計算する）。
        /// </summary>
        [Test]
        public void CreateHashAlgorithmSP_Ripemd160()
        {
#if NETCOREAPP
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.RIPEMD160_M), Is.Null);
#else
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.RIPEMD160_M), Is.InstanceOf<RIPEMD160>());
#endif
        }

#if !NETCOREAPP
        /// <summary>CNG 版（net48 のみ）</summary>
        [Test]
        public void CreateHashAlgorithmSP_Cng()
        {
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.MD5_CNG), Is.InstanceOf<MD5>());
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.SHA1_CNG), Is.InstanceOf<SHA1>());
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.SHA256_CNG), Is.InstanceOf<SHA256>());
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.SHA384_CNG), Is.InstanceOf<SHA384>());
            Assert.That(HashAlgorithmCmnFunc.CreateHashAlgorithmSP(EnumHashAlgorithm.SHA512_CNG), Is.InstanceOf<SHA512>());
        }
#endif

        #endregion

        #region キー付き

        /// <summary>列挙値に対応する HMAC を作る。既定（Default）は HMACSHA1。鍵はそのまま使う。</summary>
        [Test]
        public void CreateKeyedHashAlgorithmSP()
        {
            byte[] key = new byte[] { 1, 2, 3 };

            Assert.That(HashAlgorithmCmnFunc.CreateKeyedHashAlgorithmSP(EnumKeyedHashAlgorithm.Default, key), Is.InstanceOf<HMACSHA1>());
            Assert.That(HashAlgorithmCmnFunc.CreateKeyedHashAlgorithmSP(EnumKeyedHashAlgorithm.HMACSHA1, key), Is.InstanceOf<HMACSHA1>());
            Assert.That(HashAlgorithmCmnFunc.CreateKeyedHashAlgorithmSP(EnumKeyedHashAlgorithm.HMACMD5, key), Is.InstanceOf<HMACMD5>());
            Assert.That(HashAlgorithmCmnFunc.CreateKeyedHashAlgorithmSP(EnumKeyedHashAlgorithm.HMACSHA256, key), Is.InstanceOf<HMACSHA256>());
            Assert.That(HashAlgorithmCmnFunc.CreateKeyedHashAlgorithmSP(EnumKeyedHashAlgorithm.HMACSHA384, key), Is.InstanceOf<HMACSHA384>());

            KeyedHashAlgorithm sha512 = HashAlgorithmCmnFunc.CreateKeyedHashAlgorithmSP(EnumKeyedHashAlgorithm.HMACSHA512, key);
            Assert.That(sha512, Is.InstanceOf<HMACSHA512>());
            Assert.That(sha512.Key, Is.EqualTo(key));
        }

        /// <summary>
        /// HMACRIPEMD160 と MACTripleDES は、net48 だけ .NET の実装を返す。
        /// **net10.0 は null**（GetKeyedHash が BouncyCastle で計算する）。
        /// </summary>
        [Test]
        public void CreateKeyedHashAlgorithmSP_RipemdAndTripleDes()
        {
            byte[] key = new byte[24];
#if NETCOREAPP
            Assert.That(HashAlgorithmCmnFunc.CreateKeyedHashAlgorithmSP(EnumKeyedHashAlgorithm.HMACRIPEMD160, key), Is.Null);
            Assert.That(HashAlgorithmCmnFunc.CreateKeyedHashAlgorithmSP(EnumKeyedHashAlgorithm.MACTripleDES, key), Is.Null);
#else
            Assert.That(HashAlgorithmCmnFunc.CreateKeyedHashAlgorithmSP(EnumKeyedHashAlgorithm.HMACRIPEMD160, key), Is.InstanceOf<HMACRIPEMD160>());
            Assert.That(HashAlgorithmCmnFunc.CreateKeyedHashAlgorithmSP(EnumKeyedHashAlgorithm.MACTripleDES, key), Is.InstanceOf<MACTripleDES>());
#endif
        }

        /// <summary>キー付きは、列挙に無い値なら ArgumentException（キー無しと違い、既定に倒さない）。</summary>
        [Test]
        public void CreateKeyedHashAlgorithmSP_Unknown_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                HashAlgorithmCmnFunc.CreateKeyedHashAlgorithmSP((EnumKeyedHashAlgorithm)999, new byte[8]));
        }

        #endregion
    }
}

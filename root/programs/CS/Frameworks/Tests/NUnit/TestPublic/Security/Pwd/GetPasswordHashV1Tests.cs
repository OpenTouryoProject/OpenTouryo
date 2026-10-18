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
//* クラス名        ：GetPasswordHashV1Tests
//* クラス日本語名  ：GetPasswordHashV1（旧形式のパスワード ハッシュ。非推奨）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System.Security.Cryptography;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Pwd;

#pragma warning disable CS0618 // GetPasswordHashV1 は非推奨（Obsolete）だが、既存のハッシュの照合に使われるので確かめる。

namespace TestPublic.Security.Pwd
{
    /// <summary>GetPasswordHashV1（旧形式のパスワード ハッシュ。非推奨）の単体テスト</summary>
    /// <remarks>
    /// **V1 は静的なプロパティ（Salt / StretchCount）を持つ。** 変えたら [TearDown] で戻す。
    /// V1 と V2 は同じ入力でも結果が違う（仕様。Public/Security/ANALYSIS.md 5 節）。
    /// </remarks>
    [TestFixture]
    public class GetPasswordHashV1Tests
    {
        /// <summary>パスワード</summary>
        private const string Raw = "p@ssw0rd 棟梁";

        /// <summary>テスト前の Salt</summary>
        private string _salt;

        /// <summary>テスト前の StretchCount</summary>
        private int _stretchCount;

        /// <summary>静的なプロパティを退避する。</summary>
        [SetUp]
        public void SetUp()
        {
            this._salt = GetPasswordHashV1.Salt;
            this._stretchCount = GetPasswordHashV1.StretchCount;
        }

        /// <summary>静的なプロパティを戻す。</summary>
        [TearDown]
        public void TearDown()
        {
            GetPasswordHashV1.Salt = this._salt;
            GetPasswordHashV1.StretchCount = this._stretchCount;
        }

        /// <summary>静的なプロパティは読み書きできる。</summary>
        [Test]
        public void StaticProperties()
        {
            GetPasswordHashV1.Salt = "nunit-salt";
            GetPasswordHashV1.StretchCount = 3;

            Assert.That(GetPasswordHashV1.Salt, Is.EqualTo("nunit-salt"));
            Assert.That(GetPasswordHashV1.StretchCount, Is.EqualTo(3));
        }

        /// <summary>ハッシュ（キー無し）は、作ったものが同じパスワードで一致し、違うパスワードでは一致しない。</summary>
        [TestCase(EnumHashAlgorithm.SHA256, 1)]
        [TestCase(EnumHashAlgorithm.SHA512, 5)]
        [TestCase(EnumHashAlgorithm.MD5, 0)]
        public void Hash_RoundTrip(EnumHashAlgorithm eha, int stretch)
        {
            string salted = GetPasswordHashV1.GetSaltedPassword(Raw, eha, 16, stretch);

            Assert.That(GetPasswordHashV1.EqualSaltedPassword(Raw, salted, eha), Is.True);
            Assert.That(GetPasswordHashV1.EqualSaltedPassword(Raw + "x", salted, eha), Is.False);
            Assert.That(GetPasswordHashV1.EqualSaltedPassword(Raw, GetPasswordHashV1.GetSaltedPassword(Raw, eha, 16), eha), Is.True);
        }

        /// <summary>
        /// ハッシュ（キー付き）は、StretchCount を 1 にし、既定の回数（1）で作ったものが照合できる。
        /// </summary>
        [TestCase(EnumKeyedHashAlgorithm.HMACSHA256)]
        [TestCase(EnumKeyedHashAlgorithm.HMACSHA512)]
        [TestCase(EnumKeyedHashAlgorithm.HMACRIPEMD160)]
        [TestCase(EnumKeyedHashAlgorithm.MACTripleDES)]
        public void KeyedHash_RoundTrip(EnumKeyedHashAlgorithm ekha)
        {
            GetPasswordHashV1.StretchCount = 1;

            string salted = GetPasswordHashV1.GetSaltedPassword(Raw, ekha, "nunit-key", 16);

            Assert.That(GetPasswordHashV1.EqualSaltedPassword(Raw, salted, ekha, HashAlgorithmName.SHA256), Is.True);
            Assert.That(GetPasswordHashV1.EqualSaltedPassword(Raw + "x", salted, ekha, HashAlgorithmName.SHA256), Is.False);
        }
    }
}

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
//* クラス名        ：KeyExchangeTests
//* クラス日本語名  ：KeyExg（鍵交換。Alice と Bob の対）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//*  2026/10/18  玄人 幸道         文字列版の Encrypt / Decrypt のケースを追加（#629）
//*  2026/10/18  玄人 幸道         フラグを省いた版（証明書）の Bob のケースを追加（#630）
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//**********************************************************************************

using System;
using System.Text;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Public.Security.KeyExg;

namespace TestPublic.Security.KeyExg
{
    /// <summary>KeyExg（鍵交換。Alice と Bob の対）の単体テスト</summary>
    /// <remarks>
    /// 流れ：Bob が交換鍵（公開鍵）を出す → Alice が共通鍵を作り、Bob の公開鍵で包んだ交換鍵と IV を出す
    ///       → Bob が交換鍵を開いて共通鍵を得る → 同じ共通鍵で暗号化・復号できる。
    /// バイト列の Encrypt / Decrypt と、文字列版（暗号文は Base64。#629）で確かめる。
    /// ECDH（CNG）は net48 だけ（net10.0 では Compile Remove。Public/Security/ANALYSIS.md 3 節）。
    /// </remarks>
    [TestFixture]
    public class KeyExchangeTests
    {
        /// <summary>平文</summary>
        private static readonly byte[] Plain = Encoding.UTF8.GetBytes("鍵交換のテスト 0123456789 abcdefghij");

        #region RSA

        /// <summary>RSA-OAEP：Bob の公開鍵（RSAParameters）で、Alice と Bob が同じ共通鍵を持てる。</summary>
        [Test]
        public void RsaOaep_Parameters()
        {
            RsaOaepBob bob = new RsaOaepBob();
            RsaOaepAlice alice = new RsaOaepAlice(bob.ExchangeKey2);
            bob.GeneratePrivateKey(alice.ExchangeKey, alice.IV);

            Assert.That(bob.IV, Is.EqualTo(alice.IV));
            Assert.That(bob.Decrypt(alice.Encrypt(Plain)), Is.EqualTo(Plain));
            Assert.That(alice.Decrypt(bob.Encrypt(Plain)), Is.EqualTo(Plain));
        }

        /// <summary>RSA-PKCS#1 v1.5：同じく、同じ共通鍵を持てる。</summary>
        [Test]
        public void RsaPkcs1_Parameters()
        {
            RsaPkcs1Bob bob = new RsaPkcs1Bob();
            RsaPkcs1Alice alice = new RsaPkcs1Alice(bob.ExchangeKey2);
            bob.GeneratePrivateKey(alice.ExchangeKey, alice.IV);

            Assert.That(bob.Decrypt(alice.Encrypt(Plain)), Is.EqualTo(Plain));
        }

        /// <summary>
        /// **文字列版の Encrypt / Decrypt でも暗号化して戻せる。** 暗号文は Base64（バイト列版の暗号文を Base64 にしたもの）。
        /// 2026/10/18 まで、暗号文を UTF-8 の文字列にしていて、戻せなかった（#629）。
        /// </summary>
        [Test]
        [Basis("#629")]
        public void StringOverloads_RoundTrip()
        {
            RsaOaepBob bob = new RsaOaepBob();
            RsaOaepAlice alice = new RsaOaepAlice(bob.ExchangeKey2);
            bob.GeneratePrivateKey(alice.ExchangeKey, alice.IV);

            const string plain = "鍵交換のテスト 0123456789 abcdefghij";
            string cipher = alice.Encrypt(plain);

            Assert.That(Convert.FromBase64String(cipher), Is.EqualTo(alice.Encrypt(Encoding.UTF8.GetBytes(plain))));
            Assert.That(bob.Decrypt(cipher), Is.EqualTo(plain));
            Assert.That(alice.Decrypt(bob.Encrypt(plain)), Is.EqualTo(plain));
        }

        /// <summary>Bob は証明書（*.pfx）の鍵でもよい（鍵をエクスポートできるフラグで読む）。</summary>
        [Test]
        public void RsaOaep_BobFromPfx()
        {
            RsaOaepBob bob = new RsaOaepBob(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags);
            RsaOaepAlice alice = new RsaOaepAlice(bob.ExchangeKey2);
            bob.GeneratePrivateKey(alice.ExchangeKey, alice.IV);

            Assert.That(bob.Decrypt(alice.Encrypt(Plain)), Is.EqualTo(Plain));

            RsaPkcs1Bob bob2 = new RsaPkcs1Bob(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags);
            RsaPkcs1Alice alice2 = new RsaPkcs1Alice(bob2.ExchangeKey2);
            bob2.GeneratePrivateKey(alice2.ExchangeKey, alice2.IV);
            Assert.That(bob2.Decrypt(alice2.Encrypt(Plain)), Is.EqualTo(Plain));
        }

        /// <summary>
        /// **フラグを省いた版（証明書）でも、Bob を作れる。**
        /// 2026/10/18 まで、既定の DefaultKeySet では秘密鍵を写し直せず、必ず例外になった（#630）。
        /// </summary>
        [Test]
        [Basis("#630")]
        public void BobFromPfx_DefaultFlags()
        {
            RsaOaepBob bob = new RsaOaepBob(TestCertificates.RsaPfx, TestCertificates.Password);
            RsaOaepAlice alice = new RsaOaepAlice(bob.ExchangeKey2);
            bob.GeneratePrivateKey(alice.ExchangeKey, alice.IV);
            Assert.That(bob.Decrypt(alice.Encrypt(Plain)), Is.EqualTo(Plain));

            RsaPkcs1Bob bob2 = new RsaPkcs1Bob(TestCertificates.RsaPfx, TestCertificates.Password);
            RsaPkcs1Alice alice2 = new RsaPkcs1Alice(bob2.ExchangeKey2);
            bob2.GeneratePrivateKey(alice2.ExchangeKey, alice2.IV);
            Assert.That(bob2.Decrypt(alice2.Encrypt(Plain)), Is.EqualTo(Plain));

            // フラグを渡す版も、Exportable を含めなくてよい
            RsaOaepBob bob3 = new RsaOaepBob(TestCertificates.RsaPfx, TestCertificates.Password,
                System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.DefaultKeySet);
            RsaOaepAlice alice3 = new RsaOaepAlice(bob3.ExchangeKey2);
            bob3.GeneratePrivateKey(alice3.ExchangeKey, alice3.IV);
            Assert.That(bob3.Decrypt(alice3.Encrypt(Plain)), Is.EqualTo(Plain));
        }

#if !NETCOREAPP
        /// <summary>
        /// CSP のブロブ（ExchangeKey）を渡す版は net48 だけ
        /// （RSA が RSACryptoServiceProvider のときだけブロブを出す。net10.0 では ExchangeKey が null）。
        /// </summary>
        [Test]
        public void Rsa_CspBlob()
        {
            RsaOaepBob bob = new RsaOaepBob();
            RsaOaepAlice alice = new RsaOaepAlice(bob.ExchangeKey);
            bob.GeneratePrivateKey(alice.ExchangeKey, alice.IV);
            Assert.That(bob.Decrypt(alice.Encrypt(Plain)), Is.EqualTo(Plain));

            RsaPkcs1Bob bob2 = new RsaPkcs1Bob();
            RsaPkcs1Alice alice2 = new RsaPkcs1Alice(bob2.ExchangeKey);
            bob2.GeneratePrivateKey(alice2.ExchangeKey, alice2.IV);
            Assert.That(bob2.Decrypt(alice2.Encrypt(Plain)), Is.EqualTo(Plain));
        }
#endif

        #endregion

#if !NETCOREAPP
        #region ECDH（net48 のみ）

        /// <summary>ECDH（CNG）：互いの交換鍵から同じ鍵を導出し、同じ共通鍵で暗号化・復号できる。</summary>
        [Test]
        public void EcdhCng_Hash()
        {
            EcdhCngAlice alice = new EcdhCngAlice();
            EcdhCngBob bob = new EcdhCngBob();

            alice.DeriveKeyMaterial(bob.ExchangeKey);
            bob.DeriveKeyMaterial(alice.ExchangeKey);
            Assert.That(alice.PrivateKey, Is.EqualTo(bob.PrivateKey));
            Assert.That(alice.ECDiffieHellman, Is.Not.Null);

            alice.CreateAesSP();
            bob.CreateAesSP(alice.IV);
            Assert.That(bob.Decrypt(alice.Encrypt(Plain)), Is.EqualTo(Plain));
        }

        /// <summary>ECDH（CNG）の鍵導出を HMAC にしても、同じ鍵を導出できる。</summary>
        [Test]
        public void EcdhCng_Hmac()
        {
            byte[] hmacKey = Encoding.UTF8.GetBytes("hmac-key");
            EcdhCngAlice alice = new EcdhCngAlice(
                System.Security.Cryptography.ECDiffieHellmanKeyDerivationFunction.Hmac, System.Security.Cryptography.CngAlgorithm.Sha256, hmacKey);
            EcdhCngBob bob = new EcdhCngBob(
                System.Security.Cryptography.ECDiffieHellmanKeyDerivationFunction.Hmac, System.Security.Cryptography.CngAlgorithm.Sha256, hmacKey);

            alice.DeriveKeyMaterial(bob.ExchangeKey);
            bob.DeriveKeyMaterial(alice.ExchangeKey);

            Assert.That(alice.PrivateKey, Is.EqualTo(bob.PrivateKey));
        }

        #endregion
#endif
    }
}

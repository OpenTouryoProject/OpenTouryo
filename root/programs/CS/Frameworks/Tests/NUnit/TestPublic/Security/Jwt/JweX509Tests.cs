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
//* クラス名        ：JweX509Tests
//* クラス日本語名  ：JWE の _X509（証明書で暗号化・復号）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/19  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System;
using System.Collections.Generic;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security.Jwt;

namespace TestPublic.Security.Jwt
{
    /// <summary>JWE の _X509（証明書で暗号化・復号）の単体テスト</summary>
    /// <remarks>
    /// **証明書（*.cer）だけを持つ側で暗号化し、秘密鍵入りの証明書（*.pfx）を持つ側で復号する。**
    /// 暗号化は乱数（CEK・IV）を含むので、値ではなく「復号して元に戻る」を確かめる。
    /// typ の扱いは JweTypTests（_Param で確かめている）。
    /// </remarks>
    [TestFixture]
    public class JweX509Tests
    {
        /// <summary>ペイロード</summary>
        private const string Payload = "{\"sub\":\"nunit\"}";

        /// <summary>暗号化する側（*.cer）と、復号する側（*.pfx）</summary>
        public static IEnumerable<TestCaseData> Cases()
        {
            yield return new TestCaseData(
                new Func<JWE>(() => new JWE_RsaOaepAesGcm_X509(TestCertificates.RsaCer, "", TestCertificates.Flags)),
                new Func<JWE>(() => new JWE_RsaOaepAesGcm_X509(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags)))
                .SetName("RSA-OAEP_A256GCM");
            yield return new TestCaseData(
                new Func<JWE>(() => new JWE_Rsa15A128CbcHS256_X509(TestCertificates.RsaCer, "", TestCertificates.Flags)),
                new Func<JWE>(() => new JWE_Rsa15A128CbcHS256_X509(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags)))
                .SetName("RSA1_5_A128CBC-HS256");
        }

        /// <summary>証明書で暗号化したものは、秘密鍵入りの証明書で復号できる。</summary>
        [TestCaseSource(nameof(Cases))]
        public void EncryptByCer_DecryptByPfx(Func<JWE> encrypterFactory, Func<JWE> decrypterFactory)
        {
            string jwt = encrypterFactory().Create(Payload);
            Assert.That(jwt.Split('.').Length, Is.EqualTo(5), "JWE Compact Serialization は 5 つの部分");

            string payload;
            Assert.That(decrypterFactory().Decrypt(jwt, out payload), Is.True);
            Assert.That(payload, Is.EqualTo(Payload));
        }

        /// <summary>証明書のパスとパスワードを保持する。</summary>
        [Test]
        public void Properties()
        {
            JWE_RsaOaepAesGcm_X509 oaep = new JWE_RsaOaepAesGcm_X509(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags);
            Assert.That(oaep.CertificateFilePath, Is.EqualTo(TestCertificates.RsaPfx));
            Assert.That(oaep.CertificatePassword, Is.EqualTo(TestCertificates.Password));

            JWE_Rsa15A128CbcHS256_X509 rsa15 = new JWE_Rsa15A128CbcHS256_X509(TestCertificates.RsaCer, "", TestCertificates.Flags);
            Assert.That(rsa15.CertificateFilePath, Is.EqualTo(TestCertificates.RsaCer));
            Assert.That(rsa15.CertificatePassword, Is.EqualTo(""));
        }
    }
}

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
//* クラス名        ：JwsEcdsaParamTests
//* クラス日本語名  ：JWS_ES256 / ES384 / ES512 の _Param / _X509 の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//*  2026/10/18  玄人 幸道         生成した秘密鍵を取り出すケースを追加（#627）
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Security.Jwt
{
    /// <summary>JWS_ES256 / ES384 / ES512 の _Param / _X509 の単体テスト</summary>
    /// <remarks>
    /// _Param は Windows では CNG、Linux では OpenSSL（net10.0 のみ）で署名する（Public/Security/ANALYSIS.md 3.1 節）。
    /// ここは Windows で回す。署名は乱数を含むので、「作って検証できる」「改ざんすると失敗する」を確かめる。
    /// </remarks>
    [TestFixture]
    [Platform("Win")]
    public class JwsEcdsaParamTests
    {
        /// <summary>ペイロード</summary>
        private const string Payload = "{\"sub\":\"nunit\"}";

        /// <summary>JWT のペイロード部を書き換える。</summary>
        private static string Tamper(string jwt)
        {
            string[] parts = jwt.Split('.');
            parts[1] = CustomEncode.ToBase64UrlString(Encoding.UTF8.GetBytes("{\"sub\":\"evil\"}"));
            return string.Join(".", parts);
        }

        /// <summary>ヘッダの alg</summary>
        private static string AlgOf(string jwt)
        {
            return (string)JObject.Parse(Encoding.UTF8.GetString(CustomEncode.FromBase64UrlString(jwt.Split('.')[0])))["alg"];
        }

        /// <summary>
        /// 鍵を生成した側で作り、**公開鍵の ECParameters だけを渡した側**で検証できる。alg は方式どおり。
        /// </summary>
        [Test]
        public void Es256_Param()
        {
            JWS_ES256_Param signer = new JWS_ES256_Param();
            string jwt = signer.Create(Payload);

            JWS_ES256_Param verifier = new JWS_ES256_Param(signer.ECDsaPublicParameters, false);
            Assert.That(AlgOf(jwt), Is.EqualTo("ES256"));
            Assert.That(verifier.Verify(jwt), Is.True);
            Assert.That(verifier.Verify(Tamper(jwt)), Is.False);
        }

        /// <summary>ES384（#595 で、SHA-384 が効くよう直した）</summary>
        [Test]
        public void Es384_Param()
        {
            JWS_ES384_Param signer = new JWS_ES384_Param();
            string jwt = signer.Create(Payload);

            JWS_ES384_Param verifier = new JWS_ES384_Param(signer.ECDsaPublicParameters, false);
            Assert.That(AlgOf(jwt), Is.EqualTo("ES384"));
            Assert.That(verifier.Verify(jwt), Is.True);
            Assert.That(verifier.Verify(Tamper(jwt)), Is.False);
        }

        /// <summary>ES512（#595 で、SHA-512 が効くよう直した）</summary>
        [Test]
        public void Es512_Param()
        {
            JWS_ES512_Param signer = new JWS_ES512_Param();
            string jwt = signer.Create(Payload);

            JWS_ES512_Param verifier = new JWS_ES512_Param(signer.ECDsaPublicParameters, false);
            Assert.That(AlgOf(jwt), Is.EqualTo("ES512"));
            Assert.That(verifier.Verify(jwt), Is.True);
            Assert.That(verifier.Verify(Tamper(jwt)), Is.False);
        }

        /// <summary>
        /// 外で作った秘密鍵の ECParameters を渡して署名し、公開鍵だけの側で検証できる（ES384 / ES512）。
        /// </summary>
        [Test]
        public void ImportedPrivateKey()
        {
            ECParameters p384, p521;
            using (ECDsa e = ECDsa.Create(ECCurve.NamedCurves.nistP384)) { p384 = e.ExportParameters(true); }
            using (ECDsa e = ECDsa.Create(ECCurve.NamedCurves.nistP521)) { p521 = e.ExportParameters(true); }

            string jwt384 = new JWS_ES384_Param(p384, true).Create(Payload);
            ECParameters pub384 = p384; pub384.D = null;
            Assert.That(new JWS_ES384_Param(pub384, false).Verify(jwt384), Is.True);

            string jwt512 = new JWS_ES512_Param(p521, true).Create(Payload);
            ECParameters pub521 = p521; pub521.D = null;
            Assert.That(new JWS_ES512_Param(pub521, false).Verify(jwt512), Is.True);
        }

        /// <summary>
        /// **生成した鍵の秘密鍵（ECDsaPrivateParameters）を取り出せる。** 取り出した秘密鍵で作った JWT は、元の側で検証できる。
        /// 2026/10/18 まで、Windows（CNG）では例外になった（#627。鍵が平文でのエクスポートを許していなかった）。
        /// </summary>
        [Test]
        public void PrivateParameters_Exportable()
        {
            JWS_ES256_Param es256 = new JWS_ES256_Param();
            ECParameters priv256 = es256.ECDsaPrivateParameters;
            Assert.That(priv256.D, Is.Not.Null);
            Assert.That(es256.Verify(new JWS_ES256_Param(priv256, true).Create(Payload)), Is.True);

            JWS_ES384_Param es384 = new JWS_ES384_Param();
            Assert.That(es384.Verify(new JWS_ES384_Param(es384.ECDsaPrivateParameters, true).Create(Payload)), Is.True);

            JWS_ES512_Param es512 = new JWS_ES512_Param();
            Assert.That(es512.Verify(new JWS_ES512_Param(es512.ECDsaPrivateParameters, true).Create(Payload)), Is.True);
        }

        /// <summary>証明書（_X509）：*.pfx で作り、*.cer で検証できる（ES256 / ES384 / ES512）。</summary>
        [Test]
        public void X509()
        {
            string jwt256 = new JWS_ES256_X509(TestCertificates.EcdsaPfx256, TestCertificates.Password).Create(Payload);
            Assert.That(new JWS_ES256_X509(TestCertificates.EcdsaCer256, "").Verify(jwt256), Is.True);

            string jwt384 = new JWS_ES384_X509(TestCertificates.EcdsaPfx384, TestCertificates.Password).Create(Payload);
            Assert.That(new JWS_ES384_X509(TestCertificates.EcdsaCer384, "").Verify(jwt384), Is.True);

            string jwt512 = new JWS_ES512_X509(TestCertificates.EcdsaPfx521, TestCertificates.Password).Create(Payload);
            Assert.That(new JWS_ES512_X509(TestCertificates.EcdsaCer521, "").Verify(jwt512), Is.True);
            Assert.That(new JWS_ES512_X509(TestCertificates.EcdsaCer521, "").Verify(Tamper(jwt512)), Is.False);
        }

        /// <summary>方式の違う JWT は、署名を確かめる前に false（alg の照合）。</summary>
        [Test]
        public void DifferentAlg_False()
        {
            string jwt256 = new JWS_ES256_Param().Create(Payload);

            Assert.That(new JWS_ES384_Param().Verify(jwt256), Is.False);
        }
    }
}

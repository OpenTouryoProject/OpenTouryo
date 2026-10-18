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
//* クラス名        ：RsaKeyConverterTests
//* クラス日本語名  ：RsaPublicKeyConverter / RsaPrivateKeyConverter（RSA の鍵と JWK の変換）の単体テスト
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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security.Jwt;

namespace TestPublic.Security.Jwt
{
    /// <summary>RsaPublicKeyConverter / RsaPrivateKeyConverter（RSA の鍵と JWK の変換）の単体テスト</summary>
    /// <remarks>
    /// 公開鍵の JWK の kid は、RFC 7638 の JWK Thumbprint と同じ作り方（{"e","kty","n"} の JSON のハッシュ）。
    /// RFC 7638 3.1 節の例で確かめる。
    /// </remarks>
    [TestFixture]
    public class RsaKeyConverterTests
    {
        /// <summary>RFC 7638 3.1 節の例の n</summary>
        private const string Rfc7638N =
            "0vx7agoebGcQSuuPiLJXZptN9nndrQmbXEps2aiAFbWhM78LhWx4cbbfAAtVT86zwu1RK7aPFFxuhDR1L6tSoc_BJECPebWKRXjBZCiFV4n3oknjhMstn64tZ_2W-5JsGY4Hc5n9yBXArwl93lqt7_RN5w6Cf0h4QyQ5v-65YGjQR0_FDW2QvzqY368QQMicAtaSqzs8KJZgnYb9c7d0zgdAZHzu6qMQvRL5hajrn1n91CbOpbISD08qNLyrdkt-bFTWhAI4vMQFh6WeZu0fM4lFd2NcRwr3XPksINHaQ-G_xBniIqbw0Ls1jF44-csFCur-kEgU8awapJzKnqDKgw";

        /// <summary>RFC 7638 3.1 節の例の JWK Thumbprint（SHA-256）</summary>
        private const string Rfc7638Thumbprint = "NzbLsXh8uDCcd-6MNwXF4W_7noWXFZAfHkxZsRGC9Xs";

        /// <summary>生成した RSA の鍵（秘密鍵を含む）</summary>
        private static RSAParameters CreateKey()
        {
            using (RSA rsa = RSA.Create()) { return rsa.ExportParameters(true); }
        }

        #region 公開鍵

        /// <summary>**kid は RFC 7638 の JWK Thumbprint と一致する**（RS256 は SHA-256）。kty / alg / n / e も入る。</summary>
        [Test]
        public void ParamToJwk_KidIsThumbprint()
        {
            RsaPublicKeyConverter conv = new RsaPublicKeyConverter();
            RSAParameters param = conv.JwkToParam("{\"kty\":\"RSA\",\"n\":\"" + Rfc7638N + "\",\"e\":\"AQAB\"}");

            JObject jwk = JObject.Parse(conv.ParamToJwk(param));

            Assert.That((string)jwk["kid"], Is.EqualTo(Rfc7638Thumbprint));
            Assert.That((string)jwk["kty"], Is.EqualTo("RSA"));
            Assert.That((string)jwk["alg"], Is.EqualTo("RS256"));
            Assert.That((string)jwk["n"], Is.EqualTo(Rfc7638N));
            Assert.That((string)jwk["e"], Is.EqualTo("AQAB"));
        }

        /// <summary>alg は方式で決まる（RS256 / RS384 / RS512 / PS256 / PS384 / PS512）。</summary>
        [Test]
        public void ParamToJwk_Alg()
        {
            RSAParameters key = CreateKey();

            Assert.That((string)JObject.Parse(new RsaPublicKeyConverter(JWS_RSA.RS._384).ParamToJwk(key))["alg"], Is.EqualTo("RS384"));
            Assert.That((string)JObject.Parse(new RsaPublicKeyConverter(JWS_RSA.RS._512).ParamToJwk(key))["alg"], Is.EqualTo("RS512"));
            Assert.That((string)JObject.Parse(new RsaPublicKeyConverter(JWS_RSA.PS._256).ParamToJwk(key))["alg"], Is.EqualTo("PS256"));
            Assert.That((string)JObject.Parse(new RsaPublicKeyConverter(JWS_RSA.PS._384).ParamToJwk(key))["alg"], Is.EqualTo("PS384"));
            Assert.That((string)JObject.Parse(new RsaPublicKeyConverter(JWS_RSA.PS._512).ParamToJwk(key))["alg"], Is.EqualTo("PS512"));
        }

        /// <summary>JWK → RSAParameters → JWK で、公開鍵（n / e）が保たれる。JwkToProvider の鍵で署名を検証できる。</summary>
        [Test]
        public void JwkRoundTrip_AndVerify()
        {
            RSAParameters key = CreateKey();
            RsaPublicKeyConverter conv = new RsaPublicKeyConverter();

            string jwk = conv.ParamToJwk(key);
            RSAParameters back = conv.JwkToParam(jwk);
            Assert.That(back.Modulus, Is.EqualTo(key.Modulus));
            Assert.That(back.Exponent, Is.EqualTo(key.Exponent));
            Assert.That(back.D, Is.Null);

            byte[] data = Encoding.UTF8.GetBytes("data");
            byte[] sign;
            using (RSA signer = RSA.Create()) { signer.ImportParameters(key); sign = signer.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1); }

            Assert.That(conv.JwkToProvider(jwk).VerifyData(data, sign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1), Is.True);
            Assert.That(conv.JwkToProvider(JObject.Parse(jwk)).VerifyData(data, sign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1), Is.True);
        }

        /// <summary>XML 形式の鍵 ⇔ RSAParameters ⇔ JWK。XML から作る JWK は公開鍵だけ。</summary>
        [Test]
        public void Xml()
        {
            RSAParameters key = CreateKey();
            RsaPublicKeyConverter conv = new RsaPublicKeyConverter();

            string xml = conv.ParamToXml(key);
            Assert.That(xml, Does.Contain("<Modulus>").And.Not.Contain("<D>"));
            Assert.That(conv.XmlToParam(xml).Modulus, Is.EqualTo(key.Modulus));
            Assert.That(conv.XmlToProvider(key).ExportParameters(false).Modulus, Is.EqualTo(key.Modulus));

            JObject jwk = JObject.Parse(conv.XmlToJwk(xml));
            Assert.That(jwk["d"], Is.Null);
            Assert.That((string)jwk["kid"], Is.EqualTo((string)JObject.Parse(conv.ParamToJwk(key))["kid"]));
        }

        /// <summary>RS384 / RS512 の方式でも、XML の鍵から公開鍵を取り出せる。</summary>
        [Test]
        public void Xml_Rs384AndRs512()
        {
            RSAParameters key = CreateKey();
            string xml = new RsaPublicKeyConverter().ParamToXml(key);

            Assert.That(new RsaPublicKeyConverter(JWS_RSA.RS._384).XmlToParam(xml).Modulus, Is.EqualTo(key.Modulus));
            Assert.That(new RsaPublicKeyConverter(JWS_RSA.RS._512).XmlToParam(xml).Modulus, Is.EqualTo(key.Modulus));
        }

        /// <summary>証明書：*.cer と *.pfx から取り出した公開鍵は同じ（XML でも JWK でも）。</summary>
        [Test]
        public void X509_CerAndPfx_SameKey()
        {
            RsaPublicKeyConverter conv = new RsaPublicKeyConverter();

            Assert.That(conv.X509PfxToJwk(TestCertificates.RsaPfx, TestCertificates.Password, null, TestCertificates.Flags),
                Is.EqualTo(conv.X509CerToJwk(TestCertificates.RsaCer)));
            Assert.That(conv.X509PfxToXml(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags),
                Is.EqualTo(conv.X509CerToXml(TestCertificates.RsaCer)));
            Assert.That(conv.X509PfxToParam(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags).Modulus,
                Is.EqualTo(conv.X509CerToParam(TestCertificates.RsaCer).Modulus));
        }

        /// <summary>kty が RSA でない JWK は ArgumentOutOfRangeException。</summary>
        [Test]
        public void JwkToParam_NotRsa_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RsaPublicKeyConverter().JwkToParam("{\"kty\":\"EC\"}"));
            Assert.Throws<ArgumentOutOfRangeException>(() => new RsaPrivateKeyConverter().JwkToParam("{\"kty\":\"EC\"}"));
        }

        #endregion

        #region 秘密鍵

        /// <summary>秘密鍵の JWK ⇔ RSAParameters で、秘密鍵の要素がすべて保たれる。戻した鍵で署名できる。</summary>
        [Test]
        public void Private_RoundTrip()
        {
            RSAParameters key = CreateKey();
            RsaPrivateKeyConverter conv = new RsaPrivateKeyConverter(JWS_RSA.PS._256);

            string jwk = conv.ParamToJwk(key);
            Assert.That((string)JObject.Parse(jwk)["alg"], Is.EqualTo("PS256"));

            RSAParameters back = conv.JwkToParam(jwk);
            Assert.That(back.D, Is.EqualTo(key.D));
            Assert.That(back.P, Is.EqualTo(key.P));
            Assert.That(back.Q, Is.EqualTo(key.Q));
            Assert.That(back.DP, Is.EqualTo(key.DP));
            Assert.That(back.DQ, Is.EqualTo(key.DQ));
            Assert.That(back.InverseQ, Is.EqualTo(key.InverseQ));

            byte[] data = Encoding.UTF8.GetBytes("data");
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(back);
                // net48 の RSA.Create() は RSACryptoServiceProvider で PSS を持たないので、PKCS#1 v1.5 で確かめる
                byte[] sign = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                Assert.That(new RsaPublicKeyConverter().JwkToProvider(new RsaPublicKeyConverter().ParamToJwk(key))
                    .VerifyData(data, sign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1), Is.True);
            }
        }

        /// <summary>シリアライズの設定を渡せる（インデント付きにしても、読み戻せる）。</summary>
        [Test]
        public void Private_Settings()
        {
            RSAParameters key = CreateKey();
            RsaPrivateKeyConverter conv = new RsaPrivateKeyConverter(JWS_RSA.RS._512);

            string jwk = conv.ParamToJwk(key, new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.Indented });

            Assert.That(jwk, Does.Contain(Environment.NewLine));
            Assert.That(conv.JwkToParam(JObject.Parse(jwk)).D, Is.EqualTo(key.D));
        }

        #endregion
    }
}

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
//* クラス名        ：JwkOperatorTests
//* クラス日本語名  ：JwkOperator（RFC 7638 の指紋・公開鍵だけか）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（#634）
//**********************************************************************************

using System;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace TestPublic.Security.Jwt
{
    /// <summary>JwkOperator（RFC 7638 の指紋・公開鍵だけか）の単体テスト</summary>
    [TestFixture]
    public class JwkOperatorTests
    {
        /// <summary>RFC 7638 3.1 節の例の n</summary>
        private const string Rfc7638N =
            "0vx7agoebGcQSuuPiLJXZptN9nndrQmbXEps2aiAFbWhM78LhWx4cbbfAAtVT86zwu1RK7aPFFxuhDR1L6tSoc_BJECPebWKRXjBZCiFV4n3oknjhMstn64tZ_2W-5JsGY4Hc5n9yBXArwl93lqt7_RN5w6Cf0h4QyQ5v-65YGjQR0_FDW2QvzqY368QQMicAtaSqzs8KJZgnYb9c7d0zgdAZHzu6qMQvRL5hajrn1n91CbOpbISD08qNLyrdkt-bFTWhAI4vMQFh6WeZu0fM4lFd2NcRwr3XPksINHaQ-G_xBniIqbw0Ls1jF44-csFCur-kEgU8awapJzKnqDKgw";

        /// <summary>RFC 7638 3.1 節の例の JWK Thumbprint（SHA-256）</summary>
        private const string Rfc7638Thumbprint = "NzbLsXh8uDCcd-6MNwXF4W_7noWXFZAfHkxZsRGC9Xs";

        /// <summary>RFC 7638 3.1 節の例の JWK（余計な項目と、項目の順の違いを含む）</summary>
        private static JObject Rfc7638Jwk()
        {
            return JObject.Parse("{\"kty\":\"RSA\",\"n\":\"" + Rfc7638N + "\",\"e\":\"AQAB\",\"alg\":\"RS256\",\"kid\":\"2011-04-29\"}");
        }

        /// <summary>RFC 7638 3.1 節の例と一致する（余計な項目・順の違いは影響しない）。</summary>
        [Test]
        public void Thumbprint_Rfc7638Example()
        {
            Assert.That(JwkOperator.Thumbprint(Rfc7638Jwk()), Is.EqualTo(Rfc7638Thumbprint));
        }

        /// <summary>
        /// 指紋は常に SHA-256 で取れる。ハッシュを指定すれば変わる。
        /// RS384 の変換器が作る kid（SHA-384）とは一致しない（jkt に kid を使ってはならない）。
        /// </summary>
        [Test]
        public void Thumbprint_HashIsExplicit()
        {
            JObject jwk = Rfc7638Jwk();
            string sha384 = JwkOperator.Thumbprint(jwk, EnumHashAlgorithm.SHA384);

            Assert.That(sha384, Is.Not.EqualTo(Rfc7638Thumbprint));

            RsaPublicKeyConverter rs384 = new RsaPublicKeyConverter(JWS_RSA.RS._384);
            JObject made = JObject.Parse(rs384.ParamToJwk(new RsaPublicKeyConverter().JwkToParam(jwk)));
            Assert.That((string)made["kid"], Is.EqualTo(sha384), "変換器の kid はハッシュに連動する");
            Assert.That(JwkOperator.Thumbprint(made), Is.EqualTo(Rfc7638Thumbprint));
        }

        /// <summary>EC の指紋は、ES256 の変換器が作る kid と一致する。秘密鍵の JWK でも同じ値になる。</summary>
        [Test]
        public void Thumbprint_Ec()
        {
            using (ECDsa ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256))
            {
                ECParameters param = ecdsa.ExportParameters(true);
                JObject pub = JObject.Parse(new EccPublicKeyConverter(JWS_ECDSA.ES._256).ParamToJwk(param));
                JObject priv = JObject.Parse(new EccPrivateKeyConverter(JWS_ECDSA.ES._256).ParamToJwk(param));

                Assert.That(JwkOperator.Thumbprint(pub), Is.EqualTo((string)pub["kid"]));
                Assert.That(JwkOperator.Thumbprint(priv), Is.EqualTo((string)pub["kid"]));
            }
        }

        /// <summary>oct は k / kty から作る。読めない JWK は空文字（例外にしない）。</summary>
        [Test]
        public void Thumbprint_OctAndInvalid()
        {
            Assert.That(JwkOperator.Thumbprint(JObject.Parse("{\"kty\":\"oct\",\"k\":\"AAEC\"}")), Is.Not.Empty);
            Assert.That(JwkOperator.Thumbprint(null), Is.Empty);
            Assert.That(JwkOperator.Thumbprint(JObject.Parse("{\"kty\":\"RSA\",\"e\":\"AQAB\"}")), Is.Empty, "n が無い");
            Assert.That(JwkOperator.Thumbprint(JObject.Parse("{\"kty\":\"EC\",\"crv\":\"P-256\",\"x\":1,\"y\":\"a\"}")), Is.Empty, "文字列でない");
            Assert.That(JwkOperator.Thumbprint(JObject.Parse("{\"kty\":\"OKP\",\"x\":\"a\"}")), Is.Empty, "扱わない kty");
        }

        /// <summary>IsPublicOnly / ToPublic：秘密鍵の項目を見分け、落とす（引数は変えない）。</summary>
        [Test]
        public void PublicOnly()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                JObject priv = JObject.Parse(new RsaPrivateKeyConverter().ParamToJwk(rsa.ExportParameters(true)));
                Assert.That(JwkOperator.IsPublicOnly(priv), Is.False);

                JObject pub = JwkOperator.ToPublic(priv);
                Assert.That(JwkOperator.IsPublicOnly(pub), Is.True);
                Assert.That(pub["d"], Is.Null);
                Assert.That(priv["d"], Is.Not.Null, "引数は変えない");
                Assert.That(JwkOperator.Thumbprint(pub), Is.EqualTo(JwkOperator.Thumbprint(priv)));
            }

            Assert.That(JwkOperator.IsPublicOnly(JObject.Parse("{\"kty\":\"oct\",\"k\":\"AAEC\"}")), Is.False, "共通鍵は秘密");
            Assert.That(JwkOperator.IsPublicOnly(null), Is.False);
            Assert.Throws<ArgumentNullException>(() => JwkOperator.ToPublic(null));
        }
    }
}

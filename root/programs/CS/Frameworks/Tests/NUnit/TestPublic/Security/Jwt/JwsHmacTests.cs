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
//* クラス名        ：JwsHmacTests
//* クラス日本語名  ：JWS_HS256 / HS384 / HS512（HMAC の JWS）の単体テスト
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

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Security.Jwt
{
    /// <summary>JWS_HS256 / HS384 / HS512（HMAC の JWS）の単体テスト</summary>
    /// <remarks>
    /// HMAC は同じ鍵・同じ入力なら同じ署名になる。RFC 7515 付録 A.1 の例（HS256）で確かめる。
    /// typ の照合は JwsTypTests（#611）。
    /// </remarks>
    [TestFixture]
    public class JwsHmacTests
    {
        /// <summary>鍵（32 バイト）</summary>
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef");

        /// <summary>ペイロード</summary>
        private const string Payload = "{\"sub\":\"nunit\"}";

        /// <summary>方式ごとに作って検証できる。alg は方式どおり。違う鍵では検証に失敗する。</summary>
        [Test]
        public void CreateAndVerify()
        {
            Assert.That(new JWS_HS256(Key).Verify(new JWS_HS256(Key).Create(Payload)), Is.True);
            Assert.That(new JWS_HS384(Key).Verify(new JWS_HS384(Key).Create(Payload)), Is.True);
            Assert.That(new JWS_HS512(Key).Verify(new JWS_HS512(Key).Create(Payload)), Is.True);

            string jwt = new JWS_HS512(Key).Create(Payload);
            Assert.That((string)JObject.Parse(Encoding.UTF8.GetString(CustomEncode.FromBase64UrlString(jwt.Split('.')[0])))["alg"],
                Is.EqualTo("HS512"));
            Assert.That(new JWS_HS512(Encoding.UTF8.GetBytes("other-key-other-key-other-key-00")).Verify(jwt), Is.False);
        }

        /// <summary>方式（alg）の違う JWT は false。</summary>
        [Test]
        public void DifferentAlg_False()
        {
            Assert.That(new JWS_HS384(Key).Verify(new JWS_HS256(Key).Create(Payload)), Is.False);
        }

        /// <summary>
        /// 鍵から作ると、JWK（kty=oct、k は鍵の Base64Url）を持つ。その JWK から作ったものと、互いに検証できる。
        /// </summary>
        [Test]
        public void Jwk_RoundTrip()
        {
            JWS_HS256 fromKey = new JWS_HS256(Key);
            JObject jwk = JObject.Parse(fromKey.JWK);

            Assert.That((string)jwk["kty"], Is.EqualTo("oct"));
            Assert.That((string)jwk["alg"], Is.EqualTo("HS256"));
            Assert.That((string)jwk["k"], Is.EqualTo(CustomEncode.ToBase64UrlString(Key)));

            JWS_HS256 fromJwk = new JWS_HS256(fromKey.JWK);
            Assert.That(fromJwk.Key, Is.EqualTo(Key));
            Assert.That(fromJwk.Verify(fromKey.Create(Payload)), Is.True);

            JWS_HS512 hs512 = new JWS_HS512(Key);
            Assert.That(new JWS_HS512(hs512.JWK).Verify(hs512.Create(Payload)), Is.True);
            JWS_HS384 hs384 = new JWS_HS384(Key);
            Assert.That(new JWS_HS384(hs384.JWK).Verify(hs384.Create(Payload)), Is.True);
        }

        /// <summary>JWK の項目が足りない・値が違う（alg が方式と違う、kty が oct でない）なら ArgumentException。</summary>
        [TestCase("{\"kty\":\"oct\",\"use\":\"sig\",\"alg\":\"HS256\"}")]
        [TestCase("{\"kty\":\"oct\",\"use\":\"sig\",\"alg\":\"HS512\",\"k\":\"AAAA\"}")]
        [TestCase("{\"kty\":\"RSA\",\"use\":\"sig\",\"alg\":\"HS256\",\"k\":\"AAAA\"}")]
        [TestCase("{\"kty\":\"oct\",\"use\":\"sig\",\"alg\":\"HS256\",\"k\":\"\"}")]
        public void Jwk_Invalid_Throws(string jwk)
        {
            Assert.Throws<ArgumentException>(() => new JWS_HS256(jwk));
        }

        /// <summary>RFC 7515 付録 A.1 の例（HS256）の JWS は、例の鍵で検証できる。</summary>
        [Test]
        public void Rfc7515_A1()
        {
            const string k = "AyM1SysPpbyDfgZld3umj1qzKObwVMkoqQ-EstJQLr_T-1qS0gZH75aKtMN3Yj0iPS4hcgUuTwjAzZr1Z9CAow";
            const string jws =
                "eyJ0eXAiOiJKV1QiLA0KICJhbGciOiJIUzI1NiJ9" +
                ".eyJpc3MiOiJqb2UiLA0KICJleHAiOjEzMDA4MTkzODAsDQogImh0dHA6Ly9leGFtcGxlLmNvbS9pc19yb290Ijp0cnVlfQ" +
                ".dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk";

            Assert.That(new JWS_HS256(CustomEncode.FromBase64UrlString(k)).Verify(jws), Is.True);
        }
    }
}

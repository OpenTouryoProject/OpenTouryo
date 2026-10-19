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
//* クラス名        ：AuthenticationHeaderTests
//* クラス日本語名  ：AuthenticationHeader（Basic / Bearer の Authorization ヘッダ）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/19  玄人 幸道         新規作成
//**********************************************************************************

using System;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Framework.Authentication;

namespace TestFramework.Authentication
{
    /// <summary>AuthenticationHeader（Basic / Bearer の Authorization ヘッダ）の単体テスト</summary>
    [TestFixture]
    public class AuthenticationHeaderTests
    {
        /// <summary>"Basic " + Base64(id:secret)</summary>
        private static string Basic(string raw)
        {
            return "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(raw));
        }

        /// <summary>Basic：作ったヘッダから、client_id / client_secret を取り出せる。</summary>
        [Test]
        public void Basic_RoundTrip()
        {
            string header = AuthenticationHeader.CreateBasicAuthenticationHeaderValue("client01", "secret01").ToString();
            Assert.That(header, Does.StartWith("Basic "));

            string id, secret;
            Assert.That(AuthenticationHeader.GetCredentials(header, out id, out secret), Is.True);
            Assert.That(id, Is.EqualTo("client01"));
            Assert.That(secret, Is.EqualTo("secret01"));
        }

        /// <summary>
        /// Basic：作る側は RFC 6749 §2.3.1 のとおり URL 符号化してから Base64 にする。
        /// 復号ありの版は、符号化を戻した値と、ヘッダに載っていたままの値の両方を返す。
        /// </summary>
        [Test]
        public void Basic_UrlEncoded()
        {
            string header = AuthenticationHeader.CreateBasicAuthenticationHeaderValue("client:01", "se cret/+").ToString();

            string id, secret, rawId, rawSecret;
            Assert.That(AuthenticationHeader.GetCredentials(header, out id, out secret, out rawId, out rawSecret), Is.True);
            Assert.That(id, Is.EqualTo("client:01"));
            Assert.That(secret, Is.EqualTo("se cret/+"));
            Assert.That(rawId, Is.EqualTo("client%3A01"));
            Assert.That(rawSecret, Is.EqualTo("se%20cret%2F%2B"));
        }

        /// <summary>Basic：secret の無い形（"id" だけ）も受ける。secret を null で作ると空文字になる。</summary>
        [Test]
        public void Basic_IdOnly()
        {
            string id, secret;
            Assert.That(AuthenticationHeader.GetCredentials(Basic("client01"), out id, out secret), Is.True);
            Assert.That(id, Is.EqualTo("client01"));
            Assert.That(secret, Is.EqualTo(""));

            string header = AuthenticationHeader.CreateBasicAuthenticationHeaderValue("client01", null).ToString();
            Assert.That(AuthenticationHeader.GetCredentials(header, out id, out secret), Is.True);
            Assert.That(secret, Is.EqualTo(""));
        }

        /// <summary>id が空なら、Basic のヘッダは作らない（null）。</summary>
        [Test]
        public void Basic_EmptyId_Null()
        {
            Assert.That(AuthenticationHeader.CreateBasicAuthenticationHeaderValue("", "secret"), Is.Null);
            Assert.That(AuthenticationHeader.CreateBasicAuthenticationHeaderValue(null, "secret"), Is.Null);
        }

        /// <summary>Bearer：作ったヘッダから、トークンを取り出せる。</summary>
        [Test]
        public void Bearer_RoundTrip()
        {
            string header = AuthenticationHeader.CreateBearerAuthenticationHeaderValue("token01").ToString();
            Assert.That(header, Is.EqualTo("Bearer token01"));

            string token;
            Assert.That(AuthenticationHeader.GetCredentials(header, out token), Is.True);
            Assert.That(token, Is.EqualTo("token01"));
        }

        /// <summary>
        /// 方式が違う・値が無い・Base64 でない場合は、例外にせず「資格情報なし」（false、空）を返す。
        /// 呼び出し側は、それを受けて 401 を返せる。
        /// </summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Bearer")]
        [TestCase("Bearer ")]
        [TestCase("Basic !!!not-base64!!!")]
        [TestCase("Digest abc")]
        public void Invalid_NoCredentials(string header)
        {
            string[] credentials;
            Assert.That(AuthenticationHeader.GetCredentials(header, out credentials), Is.EqualTo(""));
            Assert.That(credentials, Is.Empty);

            string id, secret, token;
            Assert.That(AuthenticationHeader.GetCredentials(header, out id, out secret), Is.False);
            Assert.That(AuthenticationHeader.GetCredentials(header, out token), Is.False);
        }

        /// <summary>方式の取り違え：Bearer のヘッダは Basic として、Basic のヘッダは Bearer として読めない。</summary>
        [Test]
        public void WrongScheme_False()
        {
            string id, secret, token;
            Assert.That(AuthenticationHeader.GetCredentials("Bearer token01", out id, out secret), Is.False);
            Assert.That(AuthenticationHeader.GetCredentials(Basic("client01:secret01"), out token), Is.False);
        }

        /// <summary>Basic で ":" が 2 つ以上ある（3 つ以上に分かれる）ものは受けない。</summary>
        [Test]
        public void Basic_TooManyParts_False()
        {
            string id, secret;
            Assert.That(AuthenticationHeader.GetCredentials(Basic("a:b:c"), out id, out secret), Is.False);
        }
    }
}

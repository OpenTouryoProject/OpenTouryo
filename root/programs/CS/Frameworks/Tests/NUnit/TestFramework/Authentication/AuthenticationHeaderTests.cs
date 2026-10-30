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
//*  2026/10/22  玄人 幸道         DPoP・チャレンジ・復号前後の照合のテストを追加（#634）
//*  2026/10/29  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）を広げた（#640）
//*  2026/10/30  玄人 幸道         Basic：最初の ":" で分けることにした（Basic_TooManyParts_False を Basic_ColonInSecret にした。#643）
//*  2026/10/30  玄人 幸道         Basic：復号前の値で通ったかを返す版のテストを足し、Basic_WithVerifier に根拠を付けた（#643）
//**********************************************************************************

using System;
using System.Text;

using NUnit.Framework;

using TestCommon;

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
        [Basis("RFC 7617 2（user-id と password を \":\" で連ねて Base64 にする）")]
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
        [Basis("RFC 6749 2.3.1（client_id・client_secret は application/x-www-form-urlencoded で符号化してから Basic にする）")]
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
        [Observation("RFC 7617 2 では \":\" が要る。\":\" の無い形を id だけとして受けるのは、GetCredentials(Basic) の「Length == 1 の ケースもサポート」による後方互換（今の振る舞いを写した）")]
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
        [Basis("RFC 6750 2.1（Authorization: Bearer の後に b64token）")]
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
        [Basis("設計：AuthenticationHeader.GetCredentials(authHeader, out credentials) の remarks（取り出せない場合は、空の配列と \"\" を返す）")]
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

        /// <summary>
        /// Basic で ":" が 2 つ以上あれば、**最初の ":" で分ける**（2 つ目からの ":" は secret の一部）。
        /// 2026/10/30 まで、3 つ以上に分かれるものは受けなかった（#643）。
        /// </summary>
        [Test]
        [Basis("RFC 7617 2（user-id は \":\" を含まない。最初の \":\" より後ろは、すべて password）")]
        [Basis("#643")]
        public void Basic_ColonInSecret()
        {
            string id, secret;
            Assert.That(AuthenticationHeader.GetCredentials(Basic("a:b:c"), out id, out secret), Is.True);
            Assert.That(id, Is.EqualTo("a"));
            Assert.That(secret, Is.EqualTo("b:c"));

            Assert.That(AuthenticationHeader.GetCredentials(Basic("a::"), out id, out secret), Is.True);
            Assert.That(secret, Is.EqualTo(":"));

            // 照合する版も、この結果を使う（符号化しないクライアントが、":" を含む秘密をそのまま送ってきた）
            Assert.That(AuthenticationHeader.GetCredentials(Basic("client01:se:cret"), (i, s) => s == "se:cret", out id, out secret), Is.True);
            Assert.That(secret, Is.EqualTo("se:cret"));
        }

        #region #634（DPoP・チャレンジ・復号前後の照合）

        /// <summary>
        /// Basic（照合する版）：復号後で通らず、復号前で通るなら、復号前の値を返す（符号化しないクライアント）。
        /// 英数字だけなら照合は 1 回で済む。照合の処理が null なら ArgumentNullException。
        /// 復号前の値での照合は、4.0 で既定ではやめる予定（#643。それまでは設計どおりに受ける）。
        /// </summary>
        [Test]
        [Basis("RFC 6749 2.3.1（client_id・client_secret は application/x-www-form-urlencoded で符号化してから Basic にする）")]
        [Basis("設計：AuthenticationHeader.GetCredentials(authHeader, verifier, …) の remarks（符号化しないクライアントのため、復号前の値でも照合する。#634 の H-3。4.0 でやめる予定）")]
        public void Basic_WithVerifier()
        {
            // 符号化しないクライアントが "a%2Fb" という秘密を、そのまま送ってきた。
            string raw = Basic("client01:a%2Fb");
            string id, secret;
            Assert.That(AuthenticationHeader.GetCredentials(raw, (i, s) => s == "a%2Fb", out id, out secret), Is.True);
            Assert.That(secret, Is.EqualTo("a%2Fb"));

            // 符号化するクライアント（RFC 6749 §2.3.1）。
            string encoded = AuthenticationHeader.CreateBasicAuthenticationHeaderValue("client01", "a/b").ToString();
            Assert.That(AuthenticationHeader.GetCredentials(encoded, (i, s) => s == "a/b", out id, out secret), Is.True);
            Assert.That(secret, Is.EqualTo("a/b"));

            int calls = 0;
            Assert.That(AuthenticationHeader.GetCredentials(Basic("client01:secret01"), (i, s) => { calls++; return false; }, out id, out secret), Is.True);
            Assert.That(calls, Is.EqualTo(0), "英数字だけなら照合しない（どちらも同じ値）");
            Assert.That(secret, Is.EqualTo("secret01"));

            Assert.That(AuthenticationHeader.GetCredentials("Bearer x", (i, s) => true, out id, out secret), Is.False, "Basic でない");
            Assert.Throws<ArgumentNullException>(() => AuthenticationHeader.GetCredentials(raw, null, out id, out secret));
        }

        /// <summary>
        /// Basic（照合し、復号前の値で通ったかを返す版）：符号化しないクライアントが記号を含む値を送ってきたときだけ true。
        /// 振る舞い（返す値）は、usedRawValue を受けない版と同じ。4.0 で復号前の照合をやめる前の、見つけるための版（#643）。
        /// </summary>
        [Test]
        [Basis("設計：AuthenticationHeader.GetCredentials(authHeader, verifier, …, out usedRawValue) の remarks（復号前の値で通ったときだけ true）")]
        [Basis("#643")]
        public void Basic_WithVerifier_UsedRawValue()
        {
            string id, secret;
            bool usedRaw;

            // 符号化しないクライアント（記号を含む値を、そのまま送ってきた）
            Assert.That(AuthenticationHeader.GetCredentials(Basic("client01:a%2Fb"), (i, s) => s == "a%2Fb", out id, out secret, out usedRaw), Is.True);
            Assert.That(secret, Is.EqualTo("a%2Fb"));
            Assert.That(usedRaw, Is.True, "復号前の値で通った");

            // 符号化するクライアント（RFC 6749 §2.3.1）
            string encoded = AuthenticationHeader.CreateBasicAuthenticationHeaderValue("client01", "a/b").ToString();
            Assert.That(AuthenticationHeader.GetCredentials(encoded, (i, s) => s == "a/b", out id, out secret, out usedRaw), Is.True);
            Assert.That(usedRaw, Is.False, "復号後の値で通った");

            // 英数字だけ（どちらも同じ値なので、照合しない）
            Assert.That(AuthenticationHeader.GetCredentials(Basic("client01:secret01"), (i, s) => true, out id, out secret, out usedRaw), Is.True);
            Assert.That(usedRaw, Is.False);

            // どちらでも通らない（返すのは復号後の値）
            Assert.That(AuthenticationHeader.GetCredentials(Basic("client01:a%2Fb"), (i, s) => false, out id, out secret, out usedRaw), Is.True);
            Assert.That(secret, Is.EqualTo("a/b"));
            Assert.That(usedRaw, Is.False);

            Assert.That(AuthenticationHeader.GetCredentials("Bearer x", (i, s) => true, out id, out secret, out usedRaw), Is.False, "Basic でない");
            Assert.That(usedRaw, Is.False);
            Assert.Throws<ArgumentNullException>(() => AuthenticationHeader.GetCredentials(Basic("a:b"), null, out id, out secret, out usedRaw));
        }

        /// <summary>GetAccessToken：Bearer / DPoP を読み、スキームを返す（大文字小文字を区別しない）。</summary>
        [TestCase("Bearer at01", "Bearer", "at01")]
        [TestCase("DPoP at01", "DPoP", "at01")]
        [TestCase("dpop at01", "DPoP", "at01")]
        [TestCase("bearer  at01 ", "Bearer", "at01")]
        [Basis("RFC 7235 2.1（認証のスキームは大文字小文字を区別しない）")]
        [Basis("RFC 9449 7.1（DPoP のスキーム）")]
        [Basis("設計：AuthenticationHeader.GetAccessToken の remarks（スキームを返す。RFC 9449 7.2 で、DPoP で縛ったトークンを Bearer で出されたら断るため）")]
        public void GetAccessToken(string header, string scheme, string token)
        {
            string t, s;
            Assert.That(AuthenticationHeader.GetAccessToken(header, out t, out s), Is.True);
            Assert.That(s, Is.EqualTo(scheme));
            Assert.That(t, Is.EqualTo(token));
        }

        /// <summary>GetAccessToken：値が無い・空白を含む・他の方式は false。</summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase("DPoP")]
        [TestCase("DPoP ")]
        [TestCase("DPoP a b")]
        [TestCase("Basic YTpi")]
        [Basis("RFC 6750 2.1（b64token に空白は含まれない）")]
        [Basis("RFC 9449 7.1（DPoP の資格情報は token68）")]
        public void GetAccessToken_False(string header)
        {
            string t, s;
            Assert.That(AuthenticationHeader.GetAccessToken(header, out t, out s), Is.False);
            Assert.That(s, Is.Empty);
        }

        /// <summary>DPoP スキームのヘッダを作れる。チャレンジのスキームは、要求のスキームに合わせる。</summary>
        [Test]
        [Basis("RFC 9449 7.1（DPoP で縛ったトークンは DPoP のスキームで送る。DPoP で来た要求には DPoP で答える）")]
        [Basis("設計：AuthenticationHeader.GetChallengeScheme の remarks（それ以外は Bearer）")]
        public void DPoP_HeaderAndChallengeScheme()
        {
            Assert.That(AuthenticationHeader.CreateDPoPAuthenticationHeaderValue("at01").ToString(), Is.EqualTo("DPoP at01"));
            Assert.That(AuthenticationHeader.GetChallengeScheme("DPoP at01"), Is.EqualTo("DPoP"));
            Assert.That(AuthenticationHeader.GetChallengeScheme("Bearer at01"), Is.EqualTo("Bearer"));
            Assert.That(AuthenticationHeader.GetChallengeScheme(null), Is.EqualTo("Bearer"));
        }

        /// <summary>
        /// CreateChallengeParameter：トークンが無ければ realm だけ。DPoP なら algs を付ける。
        /// " と \ は逃がす。
        /// </summary>
        [Test]
        [Basis("RFC 6750 3（realm・error・error_description）・3.1（認証情報の無い要求にはエラー情報を付けない）")]
        [Basis("RFC 9449 7.1（algs を付ける）")]
        [Basis("RFC 9110 5.6.4（quoted-string の中では \\ で 1 文字を逃がす）")]
        public void CreateChallengeParameter()
        {
            Assert.That(AuthenticationHeader.CreateChallengeParameter("Bearer", "api", "", "x", null), Is.EqualTo("realm=\"api\""));
            Assert.That(AuthenticationHeader.CreateChallengeParameter("Bearer", "api", "invalid_token", "expired", new string[] { "ES256" }),
                Is.EqualTo("realm=\"api\", error=\"invalid_token\", error_description=\"expired\""));
            Assert.That(AuthenticationHeader.CreateChallengeParameter("DPoP", null, "use_dpop_nonce", null, new string[] { "ES256", "RS256" }),
                Is.EqualTo("error=\"use_dpop_nonce\", algs=\"ES256 RS256\""));
            Assert.That(AuthenticationHeader.CreateChallengeParameter("Bearer", "a\"b\\c", null, null, null),
                Is.EqualTo("realm=\"a\\\"b\\\\c\""));
        }

        #endregion
    }
}

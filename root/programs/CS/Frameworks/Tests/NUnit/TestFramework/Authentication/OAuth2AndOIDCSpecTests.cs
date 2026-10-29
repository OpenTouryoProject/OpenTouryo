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
//* クラス名        ：OAuth2AndOIDCSpecTests
//* クラス日本語名  ：OAuth2AndOIDCSpec（仕様が決めている小さな処理）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成（#634）
//*  2026/10/29  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//**********************************************************************************

using System;
using System.Collections.Generic;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Framework.Authentication;

namespace TestFramework.Authentication
{
    /// <summary>OAuth2AndOIDCSpec（仕様が決めている小さな処理）の単体テスト</summary>
    [TestFixture]
    public class OAuth2AndOIDCSpecTests
    {
        /// <summary>空白区切り：半角・全角の空白・タブ・改行で切り、空を捨てる。カンマでは切らない。</summary>
        [Test]
        [Basis("RFC 6749 3.3（scope は空白区切りの列挙）")]
        [Basis("設計：OAuth2AndOIDCSpec.SplitSpaceDelimited の remarks（全角の空白・タブ・改行でも切る。カンマでは切らない）")]
        public void SplitSpaceDelimited()
        {
            Assert.That(OAuth2AndOIDCSpec.SplitSpaceDelimited(" a\tb\r\nc　d  a "), Is.EqualTo(new string[] { "a", "b", "c", "d", "a" }));
            Assert.That(OAuth2AndOIDCSpec.SplitSpaceDelimited("https://a/?x=1,2"), Is.EqualTo(new string[] { "https://a/?x=1,2" }));
            Assert.That(OAuth2AndOIDCSpec.SplitSpaceDelimited(null), Is.Empty);
            Assert.That(OAuth2AndOIDCSpec.ToDistinctList("b a b"), Is.EqualTo(new List<string> { "b", "a" }));
        }

        /// <summary>response_type：順不同・重複・大文字を正規化し、定数と比べられる。</summary>
        [TestCase("id_token code", OAuth2AndOIDCConst.OidcHybrid2_IdToken_ResponseType)]
        [TestCase("token  id_token code", OAuth2AndOIDCConst.OidcHybrid3_ResponseType)]
        [TestCase("token id_token", OAuth2AndOIDCConst.OidcImplicit2_ResponseType)]
        [TestCase("CODE code", OAuth2AndOIDCConst.AuthorizationCodeResponseType)]
        [TestCase("", "")]
        [TestCase(null, null)]
        [Basis("OAuth 2.0 Multiple Response Type Encoding Practices 1.2（Multiple-Valued Response Types は順不同の空白区切りとして比べる）")]
        [Basis("設計：OAuth2AndOIDCSpec.NormalizeResponseType の remarks（小文字にする。仕様は大文字小文字を区別するが寛容に受ける）")]
        public void NormalizeResponseType(string value, string expected)
        {
            Assert.That(OAuth2AndOIDCSpec.NormalizeResponseType(value), Is.EqualTo(expected));
        }

        /// <summary>prompt：集合として扱う（部分一致しない）。none と他の値の併記は invalid_request。</summary>
        [Test]
        [Basis("OIDC Core 3.1.2.1（prompt は空白区切りの列挙。none と他の値を併記したらエラー）")]
        [Basis("設計：OAuth2AndOIDCSpec.CheckPrompt の remarks（既知でない値だけではエラーにしない）")]
        public void Prompt()
        {
            Assert.That(OAuth2AndOIDCSpec.HasPrompt("login consent", OAuth2AndOIDCConst.PromptConsent), Is.True);
            Assert.That(OAuth2AndOIDCSpec.HasPrompt("nonexistent", OAuth2AndOIDCConst.PromptNone), Is.False);
            Assert.That(OAuth2AndOIDCSpec.HasPrompt("NONE", OAuth2AndOIDCConst.PromptNone), Is.True);

            string err = "", desc = "";
            Assert.That(OAuth2AndOIDCSpec.CheckPrompt("none", ref err, ref desc), Is.True);
            Assert.That(OAuth2AndOIDCSpec.CheckPrompt("login unknown", ref err, ref desc), Is.True, "未知の値だけではエラーにしない");
            Assert.That(OAuth2AndOIDCSpec.CheckPrompt("none login", ref err, ref desc), Is.False);
            Assert.That(err, Is.EqualTo(OAuth2AndOIDCConst.invalid_request));
        }

        /// <summary>max_age：数値でなければ InvalidMaxAge。秒で比べる（max_age=0 で直後は Ok）。</summary>
        [Test]
        [Basis("OIDC Core 3.1.2.1（max_age は秒。経過が超えたら再認証する）")]
        [Basis("設計：OAuth2AndOIDCSpec.CheckAuthTime の remarks（秒で切り捨てて比べる。数値でなければ InvalidMaxAge）")]
        public void CheckAuthTime()
        {
            Assert.That(OAuth2AndOIDCSpec.CheckAuthTime("", null), Is.EqualTo(OAuth2AndOIDCEnum.AuthTimeCheck.Ok));
            Assert.That(OAuth2AndOIDCSpec.CheckAuthTime("abc", DateTime.UtcNow), Is.EqualTo(OAuth2AndOIDCEnum.AuthTimeCheck.InvalidMaxAge));
            Assert.That(OAuth2AndOIDCSpec.CheckAuthTime("-1", DateTime.UtcNow), Is.EqualTo(OAuth2AndOIDCEnum.AuthTimeCheck.InvalidMaxAge));
            Assert.That(OAuth2AndOIDCSpec.CheckAuthTime("60", null), Is.EqualTo(OAuth2AndOIDCEnum.AuthTimeCheck.NeedsReAuthentication));
            Assert.That(OAuth2AndOIDCSpec.CheckAuthTime("0", DateTime.UtcNow.AddMilliseconds(-300)), Is.EqualTo(OAuth2AndOIDCEnum.AuthTimeCheck.Ok), "秒未満は超過としない");
            Assert.That(OAuth2AndOIDCSpec.CheckAuthTime("60", DateTime.UtcNow.AddSeconds(-61)), Is.EqualTo(OAuth2AndOIDCEnum.AuthTimeCheck.NeedsReAuthentication));
            Assert.That(OAuth2AndOIDCSpec.CheckAuthTime("60", DateTime.UtcNow.AddSeconds(-30)), Is.EqualTo(OAuth2AndOIDCEnum.AuthTimeCheck.Ok));
        }

        /// <summary>
        /// BuildRedirectUrl：値は URL エンコード（state の注入を防ぐ）。既存のクエリ・素片に繋ぐ。
        /// iss を付ける（JARM の response のときは付けない）。空の値は付けない。引数は変えない。
        /// </summary>
        [Test]
        [Basis("RFC 9207 2（成功・失敗の応答に iss を付ける）・2.4（JARM なら付けない）")]
        [Basis("設計：OAuth2AndOIDCSpec.BuildRedirectUrl の remarks（値は URL エンコード。既存のクエリ・素片に繋ぐ。引数は変えない）")]
        public void BuildRedirectUrl()
        {
            Dictionary<string, string> p = new Dictionary<string, string>
            {
                { "code", "c1" }, { "state", "s&evil=1" }, { "nonce", "" }
            };

            Assert.That(OAuth2AndOIDCSpec.BuildRedirectUrl("https://rp/cb", p, false, "https://as/"),
                Is.EqualTo("https://rp/cb?code=c1&state=s%26evil%3D1&iss=https%3A%2F%2Fas%2F"));
            Assert.That(OAuth2AndOIDCSpec.BuildRedirectUrl("https://rp/cb?x=1", p, false, null),
                Is.EqualTo("https://rp/cb?x=1&code=c1&state=s%26evil%3D1"));
            Assert.That(OAuth2AndOIDCSpec.BuildRedirectUrl("https://rp/cb", p, true, null),
                Is.EqualTo("https://rp/cb#code=c1&state=s%26evil%3D1"));
            Assert.That(p.ContainsKey("iss"), Is.False, "引数は変えない");

            Dictionary<string, string> jarm = new Dictionary<string, string> { { "response", "jwt" } };
            Assert.That(OAuth2AndOIDCSpec.BuildRedirectUrl("https://rp/cb", jarm, false, "https://as/"),
                Is.EqualTo("https://rp/cb?response=jwt"));

            Assert.That(OAuth2AndOIDCSpec.BuildRedirectUrl("https://rp/cb", null, false, null), Is.EqualTo("https://rp/cb"));
            Assert.Throws<ArgumentNullException>(() => OAuth2AndOIDCSpec.BuildRedirectUrl(null, p, false, null));
        }

        /// <summary>HTTP ステータス：/token とリソースで、use_dpop_nonce の扱いが違う。</summary>
        [Test]
        [Basis("RFC 6749 5.2（エラーは 400。invalid_client は 401）")]
        [Basis("RFC 6750 3.1（invalid_request は 400、invalid_token は 401、insufficient_scope は 403）")]
        [Basis("RFC 9449 8（/token の use_dpop_nonce は 400）・9（リソースの use_dpop_nonce は 401）")]
        public void ErrorStatusCode()
        {
            Assert.That(OAuth2AndOIDCSpec.GetErrorStatusCode(OAuth2AndOIDCConst.invalid_client), Is.EqualTo(401));
            Assert.That(OAuth2AndOIDCSpec.GetErrorStatusCode(OAuth2AndOIDCConst.invalid_token), Is.EqualTo(401));
            Assert.That(OAuth2AndOIDCSpec.GetErrorStatusCode(OAuth2AndOIDCConst.use_dpop_nonce), Is.EqualTo(400));
            Assert.That(OAuth2AndOIDCSpec.GetErrorStatusCode(OAuth2AndOIDCConst.invalid_grant), Is.EqualTo(400));

            Assert.That(OAuth2AndOIDCSpec.GetResourceErrorStatusCode(OAuth2AndOIDCConst.use_dpop_nonce), Is.EqualTo(401));
            Assert.That(OAuth2AndOIDCSpec.GetResourceErrorStatusCode(OAuth2AndOIDCConst.invalid_token), Is.EqualTo(401));
            Assert.That(OAuth2AndOIDCSpec.GetResourceErrorStatusCode(OAuth2AndOIDCConst.insufficient_scope), Is.EqualTo(403));
            Assert.That(OAuth2AndOIDCSpec.GetResourceErrorStatusCode(OAuth2AndOIDCConst.invalid_request), Is.EqualTo(400));
        }

        /// <summary>GetClientAssertion：client_assertion を優先し、型が違えば受けない。無ければ assertion。</summary>
        [Test]
        [Basis("RFC 7523 2.2（client_assertion と client_assertion_type）")]
        [Basis("設計：OAuth2AndOIDCSpec.GetClientAssertion の remarks（無ければ assertion を読む。#634 の H-2）")]
        public void GetClientAssertion()
        {
            string type = OAuth2AndOIDCConst.JwtBearerClientAssertionType;

            Assert.That(OAuth2AndOIDCSpec.GetClientAssertion("ca", type, "a"), Is.EqualTo("ca"));
            Assert.That(OAuth2AndOIDCSpec.GetClientAssertion("ca", "", null), Is.EqualTo("ca"));
            Assert.That(OAuth2AndOIDCSpec.GetClientAssertion("ca", "urn:other", "a"), Is.Empty);
            Assert.That(OAuth2AndOIDCSpec.GetClientAssertion(null, null, "a"), Is.EqualTo("a"));
            Assert.That(OAuth2AndOIDCSpec.GetClientAssertion(null, null, null), Is.Empty);
        }
    }
}

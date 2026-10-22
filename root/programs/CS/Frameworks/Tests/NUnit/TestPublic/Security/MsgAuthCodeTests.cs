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
//* クラス名        ：MsgAuthCodeTests
//* クラス日本語名  ：MsgAuthCode（メッセージ認証コード）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/19  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security;

namespace TestPublic.Security
{
    /// <summary>MsgAuthCode（メッセージ認証コード）の単体テスト</summary>
    /// <remarks>
    /// MAC の計算そのものは GetKeyedHash（GetKeyedHashTests が RFC のテスト ベクタで確かめている）。
    /// ここは、MsgAuthCode の文字列版・バイト配列版と、VerifyMAC の判定を確かめる。
    /// </remarks>
    [TestFixture]
    public class MsgAuthCodeTests
    {
        private const string Msg = "メッセージ認証コードのテスト";
        private const string Key = "secret-key";

        /// <summary>
        /// 文字列版は UTF-8 にして計算し、Base64 で返す。.NET の HMACSHA256 で計算した値と一致する。
        /// バイト配列版も同じ値を返す。
        /// </summary>
        [Test]
        public void GetMAC_MatchesHmacSha256()
        {
            byte[] expected;
            using (HMACSHA256 h = new HMACSHA256(Encoding.UTF8.GetBytes(Key)))
            {
                expected = h.ComputeHash(Encoding.UTF8.GetBytes(Msg));
            }

            Assert.That(MsgAuthCode.GetMAC(Msg, EnumKeyedHashAlgorithm.HMACSHA256, Key),
                Is.EqualTo(Convert.ToBase64String(expected)));
            Assert.That(MsgAuthCode.GetMAC(Encoding.UTF8.GetBytes(Msg), EnumKeyedHashAlgorithm.HMACSHA256, Encoding.UTF8.GetBytes(Key)),
                Is.EqualTo(expected));
        }

        /// <summary>VerifyMAC は、同じメッセージ・キーの MAC なら true、どれかが違えば false。</summary>
        [TestCase(EnumKeyedHashAlgorithm.HMACSHA1)]
        [TestCase(EnumKeyedHashAlgorithm.HMACSHA256)]
        [TestCase(EnumKeyedHashAlgorithm.HMACSHA512)]
        public void VerifyMAC(EnumKeyedHashAlgorithm ekha)
        {
            string mac = MsgAuthCode.GetMAC(Msg, ekha, Key);

            Assert.That(MsgAuthCode.VerifyMAC(Msg, ekha, Key, mac), Is.True);
            Assert.That(MsgAuthCode.VerifyMAC(Msg + "!", ekha, Key, mac), Is.False, "メッセージが違う");
            Assert.That(MsgAuthCode.VerifyMAC(Msg, ekha, Key + "!", mac), Is.False, "キーが違う");

            byte[] macBytes = Convert.FromBase64String(mac);
            Assert.That(MsgAuthCode.VerifyMAC(Encoding.UTF8.GetBytes(Msg), ekha, Encoding.UTF8.GetBytes(Key), macBytes), Is.True);
        }
    }
}

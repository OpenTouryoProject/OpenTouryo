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
//* クラス名        ：GetPasswordTests
//* クラス日本語名  ：GetPassword（パスワード・秘密値の生成）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System;
using System.Linq;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security.Pwd;

namespace TestPublic.Security.Pwd
{
    /// <summary>GetPassword（パスワード・秘密値の生成）の単体テスト</summary>
    /// <remarks>乱数を使うので、値ではなく形（長さ・使う文字・件数）を確かめる。</remarks>
    [TestFixture]
    public class GetPasswordTests
    {
        /// <summary>英数字以外に使う記号</summary>
        private const string Punctuations = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";

        /// <summary>RandomByte は指定した長さ。呼ぶたびに違う値になる。</summary>
        [Test]
        public void RandomByte()
        {
            byte[] a = GetPassword.RandomByte(32);
            byte[] b = GetPassword.RandomByte(32);

            Assert.That(a, Has.Length.EqualTo(32));
            Assert.That(b, Is.Not.EqualTo(a));
        }

        /// <summary>Base64Secret / Base64UrlSecret は、指定したバイト数の乱数を符号化したもの。</summary>
        [Test]
        public void Base64Secrets()
        {
            Assert.That(Convert.FromBase64String(GetPassword.Base64Secret(33)), Has.Length.EqualTo(33));

            // Base64Url は + / = を含まない
            string url = GetPassword.Base64UrlSecret(33);
            Assert.That(url, Does.Not.Contain("+").And.Not.Contain("/").And.Not.Contain("="));
            Assert.That(url, Has.Length.EqualTo(44));
        }

        /// <summary>
        /// Generate は、指定した長さで、**英数字以外をちょうど指定した数だけ**含む。残りは英数字。
        /// </summary>
        [TestCase(10, 0)]
        [TestCase(10, 3)]
        [TestCase(8, 8)]
        [TestCase(128, 20)]
        [TestCase(1, 1)]
        public void Generate(int length, int nonAlnum)
        {
            for (int i = 0; i < 20; i++)
            {
                string pwd = GetPassword.Generate(length, nonAlnum);

                Assert.That(pwd, Has.Length.EqualTo(length));
                Assert.That(pwd.Count(c => Punctuations.IndexOf(c) >= 0), Is.EqualTo(nonAlnum), pwd);
                Assert.That(pwd.Where(c => Punctuations.IndexOf(c) < 0).All(c => c < 128 && char.IsLetterOrDigit(c)), Is.True, pwd);
            }
        }

        /// <summary>長さは 1〜128。英数字以外の数は 0〜長さ。範囲外は ArgumentException。</summary>
        [TestCase(0, 0)]
        [TestCase(129, 0)]
        [TestCase(5, 6)]
        [TestCase(5, -1)]
        public void Generate_OutOfRange_Throws(int length, int nonAlnum)
        {
            Assert.Throws<ArgumentException>(() => GetPassword.Generate(length, nonAlnum));
        }
    }
}

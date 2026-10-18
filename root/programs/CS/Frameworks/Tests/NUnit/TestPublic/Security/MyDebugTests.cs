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
//* クラス名        ：MyDebugTests
//* クラス日本語名  ：Security.MyDebug（JWT・証明書の中身の表示）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security.Jwt;

using SecMyDebug = Touryo.Infrastructure.Public.Security.MyDebug;

namespace TestPublic.Security
{
    /// <summary>Security.MyDebug（JWT・証明書の中身の表示）の単体テスト</summary>
    /// <remarks>
    /// 表示はコンソール（と Debug）に出る。コンソールの出力を受けて、何が出るかを確かめる。
    /// Diagnostics.MyDebug の派生（重複ではない。Public/ANALYSIS.md 8 節）。
    /// </remarks>
    [TestFixture]
    public class MyDebugTests
    {
        /// <summary>コンソールの出力を受ける。</summary>
        private static string Capture(Action action)
        {
            TextWriter original = Console.Out;
            StringWriter sw = new StringWriter();
            Console.SetOut(sw);
            try
            {
                action();
            }
            finally
            {
                Console.SetOut(original);
            }
            return sw.ToString();
        }

        /// <summary>JWS（3 区切り）は、ヘッダとペイロードを JSON に戻して、署名はそのまま表示する。</summary>
        [Test]
        public void InspectJwt_Jws()
        {
            string jwt = new JWS_HS256(Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef")).Create("{\"sub\":\"nunit\"}");

            string output = Capture(() => SecMyDebug.InspectJwt("label", jwt));

            Assert.That(output, Does.Contain("JWS Header: {\"alg\":\"HS256\""));
            Assert.That(output, Does.Contain("JWS Payload: {\"sub\":\"nunit\"}"));
            Assert.That(output, Does.Contain("JWS Signature: " + jwt.Split('.')[2]));
        }

        /// <summary>JWE（5 区切り）は、ヘッダを JSON に戻して、残りの 4 つはそのまま表示する。3 / 5 区切り以外は何も出さない。</summary>
        [Test]
        public void InspectJwt_JweAndOther()
        {
            string jwe = Touryo.Infrastructure.Public.Str.CustomEncode.ToBase64UrlString(Encoding.UTF8.GetBytes("{\"alg\":\"RSA-OAEP\"}"))
                + ".KEY.IV.CIPHER.TAG";

            string output = Capture(() => SecMyDebug.InspectJwt("label", jwe));
            Assert.That(output, Does.Contain("JWE Header: {\"alg\":\"RSA-OAEP\"}"));
            Assert.That(output, Does.Contain("JWE Encrypted Key: KEY"));
            Assert.That(output, Does.Contain("JWE Authentication Tag: TAG"));

            Assert.That(Capture(() => SecMyDebug.InspectJwt("label", "a.b")), Is.Empty);
        }

        /// <summary>証明書の中身を表示する。null なら「is null」とだけ出す。</summary>
        [Test]
        public void InspectX509()
        {
            using (X509Certificate2 pfx = TestCertificates.LoadPfx(TestCertificates.RsaPfx))
            using (X509Certificate2 cer = TestCertificates.LoadCer(TestCertificates.RsaCer))
            {
                string priv = Capture(() => SecMyDebug.InspectPrivateX509Key("label", pfx));
                Assert.That(priv, Does.Contain("X509Certificate2.PrivateKey: is not null"));
                Assert.That(priv, Does.Contain("SignatureAlgorithm.FriendlyName: "));
                Assert.That(priv, Does.Contain("PrivateKey.ToString(): is "));

                string pub = Capture(() => SecMyDebug.InspectPublicX509Key("label", cer));
                Assert.That(pub, Does.Contain("publicX509Key: is not null"));
                Assert.That(pub, Does.Contain("PublicKey.Key.ToString(): is "));
            }

            Assert.That(Capture(() => SecMyDebug.InspectPrivateX509Key("label", null)), Does.Contain("is null"));
            Assert.That(Capture(() => SecMyDebug.InspectPublicX509Key("label", null)), Does.Contain("is null"));
        }
    }
}

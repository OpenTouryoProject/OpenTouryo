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
//* クラス名        ：JwsRsaTests
//* クラス日本語名  ：JWS の RSA 系（RS256/384/512・PS256/384/512 の _Param / _X509 / _XML）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/19  玄人 幸道         新規作成（カバレッジの拡充）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Security.Jwt;
using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Security.Jwt
{
    /// <summary>JWS の RSA 系（RS256/384/512・PS256/384/512 の _Param / _X509 / _XML）の単体テスト</summary>
    /// <remarks>
    /// 鍵の持ち方ごとに、**署名する側と、公開鍵だけを持つ側を別に作り**、作った JWT を検証する。
    ///   _Param … 生成した鍵で署名し、RsaPublicParameters を渡した側で検証する
    ///   _X509  … 秘密鍵入りの証明書（*.pfx）で署名し、証明書（*.cer）で検証する
    ///   _XML   … 生成した鍵で署名し、XMLPublicKey を渡した側で検証する（RS 系だけ。PS 系に _XML は無い）
    /// 署名は乱数を含む（PS）ので、値ではなく「検証できる」「改ざんすると失敗する」を確かめる。
    /// </remarks>
    [TestFixture]
    public class JwsRsaTests
    {
        /// <summary>ペイロード</summary>
        private const string Payload = "{\"sub\":\"nunit\"}";

        /// <summary>署名する側と、検証する側（署名する側から公開鍵を受け取って作る）</summary>
        public static IEnumerable<TestCaseData> Cases()
        {
            // _Param
            yield return Case("RS256", "Param", () => new JWS_RS256_Param(), s => new JWS_RS256_Param(((JWS_RS256_Param)s).RsaPublicParameters));
            yield return Case("RS384", "Param", () => new JWS_RS384_Param(), s => new JWS_RS384_Param(((JWS_RS384_Param)s).RsaPublicParameters));
            yield return Case("RS512", "Param", () => new JWS_RS512_Param(), s => new JWS_RS512_Param(((JWS_RS512_Param)s).RsaPublicParameters));
            yield return Case("PS256", "Param", () => new JWS_PS256_Param(), s => new JWS_PS256_Param(((JWS_PS256_Param)s).RsaPublicParameters));
            yield return Case("PS384", "Param", () => new JWS_PS384_Param(), s => new JWS_PS384_Param(((JWS_PS384_Param)s).RsaPublicParameters));
            yield return Case("PS512", "Param", () => new JWS_PS512_Param(), s => new JWS_PS512_Param(((JWS_PS512_Param)s).RsaPublicParameters));

            // _X509（*.pfx で署名、*.cer で検証）
            yield return Case("RS256", "X509", () => new JWS_RS256_X509(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags),
                _ => new JWS_RS256_X509(TestCertificates.RsaCer, "", TestCertificates.Flags));
            yield return Case("RS384", "X509", () => new JWS_RS384_X509(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags),
                _ => new JWS_RS384_X509(TestCertificates.RsaCer, "", TestCertificates.Flags));
            yield return Case("RS512", "X509", () => new JWS_RS512_X509(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags),
                _ => new JWS_RS512_X509(TestCertificates.RsaCer, "", TestCertificates.Flags));
            yield return Case("PS256", "X509", () => new JWS_PS256_X509(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags),
                _ => new JWS_PS256_X509(TestCertificates.RsaCer, "", TestCertificates.Flags));
            yield return Case("PS384", "X509", () => new JWS_PS384_X509(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags),
                _ => new JWS_PS384_X509(TestCertificates.RsaCer, "", TestCertificates.Flags));
            yield return Case("PS512", "X509", () => new JWS_PS512_X509(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags),
                _ => new JWS_PS512_X509(TestCertificates.RsaCer, "", TestCertificates.Flags));

            // _XML（RS 系だけ）
            yield return Case("RS256", "XML", () => new JWS_RS256_XML(), s => new JWS_RS256_XML(((JWS_RS256_XML)s).XMLPublicKey));
            yield return Case("RS384", "XML", () => new JWS_RS384_XML(), s => new JWS_RS384_XML(((JWS_RS384_XML)s).XMLPublicKey));
            yield return Case("RS512", "XML", () => new JWS_RS512_XML(), s => new JWS_RS512_XML(((JWS_RS512_XML)s).XMLPublicKey));
        }

        /// <summary>TestCaseData を作る（名前は「alg_鍵の持ち方」）</summary>
        private static TestCaseData Case(string alg, string kind, Func<JWS> signer, Func<JWS, JWS> verifier)
        {
            return new TestCaseData(alg, signer, verifier).SetName(alg + "_" + kind);
        }

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
        /// 署名した JWT は、公開鍵だけを持つ側で検証できる。改ざんすると失敗する。alg は方式どおり。
        /// </summary>
        [TestCaseSource(nameof(Cases))]
        public void SignAndVerify(string alg, Func<JWS> signerFactory, Func<JWS, JWS> verifierFactory)
        {
            JWS signer = signerFactory();
            string jwt = signer.Create(Payload);
            JWS verifier = verifierFactory(signer);

            Assert.That(AlgOf(jwt), Is.EqualTo(alg));
            Assert.That(verifier.Verify(jwt), Is.True);
            Assert.That(verifier.Verify(Tamper(jwt)), Is.False);
        }

        /// <summary>
        /// _Param：生成した鍵の秘密鍵（RsaPrivateParameters）を取り出して渡すと、同じ鍵で署名できる。
        /// 取り出した側で作った JWT は、元の側で検証できる。
        /// </summary>
        [Test]
        public void Param_PrivateParameters()
        {
            JWS_RS256_Param rs256 = new JWS_RS256_Param();
            Assert.That(rs256.Verify(new JWS_RS256_Param(rs256.RsaPrivateParameters).Create(Payload)), Is.True);

            JWS_RS384_Param rs384 = new JWS_RS384_Param();
            Assert.That(rs384.Verify(new JWS_RS384_Param(rs384.RsaPrivateParameters).Create(Payload)), Is.True);

            JWS_RS512_Param rs512 = new JWS_RS512_Param();
            Assert.That(rs512.Verify(new JWS_RS512_Param(rs512.RsaPrivateParameters).Create(Payload)), Is.True);

            JWS_PS256_Param ps256 = new JWS_PS256_Param();
            Assert.That(ps256.Verify(new JWS_PS256_Param(ps256.RsaPrivateParameters).Create(Payload)), Is.True);

            JWS_PS384_Param ps384 = new JWS_PS384_Param();
            Assert.That(ps384.Verify(new JWS_PS384_Param(ps384.RsaPrivateParameters).Create(Payload)), Is.True);

            JWS_PS512_Param ps512 = new JWS_PS512_Param();
            Assert.That(ps512.Verify(new JWS_PS512_Param(ps512.RsaPrivateParameters).Create(Payload)), Is.True);
        }

        /// <summary>
        /// _XML：生成した鍵の秘密鍵（XMLPrivateKey）を渡すと、同じ鍵で署名できる。
        /// 公開鍵の XML から作った側は、秘密鍵の XML を持たない。
        /// </summary>
        [Test]
        public void Xml_PrivateKey()
        {
            JWS_RS256_XML generated = new JWS_RS256_XML();
            Assert.That(generated.XMLPrivateKey, Does.Contain("<D>"));
            Assert.That(generated.Verify(new JWS_RS256_XML(generated.XMLPrivateKey).Create(Payload)), Is.True);

            JWS_RS256_XML publicOnly = new JWS_RS256_XML(generated.XMLPublicKey);
            Assert.That(publicOnly.XMLPrivateKey, Does.Not.Contain("<D>"));
        }

        /// <summary>_X509：証明書のパスとパスワードを保持する。</summary>
        [Test]
        public void X509_Properties()
        {
            JWS_RS256_X509 rs = new JWS_RS256_X509(TestCertificates.RsaPfx, TestCertificates.Password, TestCertificates.Flags);
            Assert.That(rs.CertificateFilePath, Is.EqualTo(TestCertificates.RsaPfx));
            Assert.That(rs.CertificatePassword, Is.EqualTo(TestCertificates.Password));
            Assert.That(rs.DigitalSignX509, Is.Not.Null);
        }
    }
}

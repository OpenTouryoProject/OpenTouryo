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
//* クラス名        ：EccKeyConverterTests
//* クラス日本語名  ：EccPublicKeyConverter / EccPrivateKeyConverter（楕円曲線の鍵と JWK の変換）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//*  2026/10/29  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//**********************************************************************************

using System;
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Public.Security.Jwt;

namespace TestPublic.Security.Jwt
{
    /// <summary>EccPublicKeyConverter / EccPrivateKeyConverter（楕円曲線の鍵と JWK の変換）の単体テスト</summary>
    /// <remarks>
    /// 公開鍵の JWK の kid は、RFC 7638 の JWK Thumbprint と同じ作り方（{"crv","kty","x","y"} の JSON のハッシュ）。
    /// ハッシュは方式に合わせる（ES256 は SHA-256、ES384 は SHA-384、ES512 は SHA-512）。
    /// </remarks>
    [TestFixture]
    public class EccKeyConverterTests
    {
        /// <summary>生成した鍵（秘密鍵を含む）</summary>
        private static ECParameters CreateKey(ECCurve curve)
        {
            using (ECDsa ecdsa = ECDsa.Create(curve)) { return ecdsa.ExportParameters(true); }
        }

        /// <summary>方式と曲線の組で、公開鍵の JWK ⇔ ECParameters が往復する。crv / alg も入る。</summary>
        [TestCase(JWS_ECDSA.ES._256, "P-256", "ES256")]
        [TestCase(JWS_ECDSA.ES._384, "P-384", "ES384")]
        [TestCase(JWS_ECDSA.ES._512, "P-521", "ES512")]
        [Basis("RFC 7518 3.4（ES256・ES384・ES512 と曲線 P-256・P-384・P-521 の対応）・6.1（kty は EC）・6.2.1.1（crv の値）")]
        public void Public_RoundTrip(JWS_ECDSA.ES es, string crv, string alg)
        {
            EccPublicKeyConverter conv = new EccPublicKeyConverter(es);
            ECParameters key = CreateKey(conv.GetECCurveFromCrvString(crv));

            JObject jwk = JObject.Parse(conv.ParamToJwk(key));
            Assert.That((string)jwk["kty"], Is.EqualTo("EC"));
            Assert.That((string)jwk["crv"], Is.EqualTo(crv));
            Assert.That((string)jwk["alg"], Is.EqualTo(alg));
            Assert.That(jwk["d"], Is.Null);

            ECParameters back = conv.JwkToParam(jwk.ToString());
            Assert.That(back.Q.X, Is.EqualTo(key.Q.X));
            Assert.That(back.Q.Y, Is.EqualTo(key.Q.Y));
            Assert.That(back.D, Is.Null);
        }

        /// <summary>kid は、{"crv","kty","x","y"} を JSON にしたもののハッシュ（ES256 は SHA-256）。</summary>
        [Test]
        [Basis("RFC 7638 3.2（EC で使うメンバは crv・kty・x・y）・3.3（メンバ名の順に並べる）")]
        public void Kid_IsThumbprint()
        {
            EccPublicKeyConverter conv = new EccPublicKeyConverter();
            JObject jwk = JObject.Parse(conv.ParamToJwk(CreateKey(ECCurve.NamedCurves.nistP256)));

            string canonical = "{\"crv\":\"" + (string)jwk["crv"] + "\",\"kty\":\"EC\",\"x\":\"" + (string)jwk["x"] + "\",\"y\":\"" + (string)jwk["y"] + "\"}";
            byte[] hash;
            using (SHA256 sha = SHA256.Create()) { hash = sha.ComputeHash(Encoding.UTF8.GetBytes(canonical)); }

            Assert.That((string)jwk["kid"], Is.EqualTo(Touryo.Infrastructure.Public.Str.CustomEncode.ToBase64UrlString(hash)));
        }

        /// <summary>秘密鍵の JWK ⇔ ECParameters で d も保たれる。戻した鍵で署名し、公開鍵の JWK の鍵で検証できる。</summary>
        [Test]
        public void Private_RoundTrip_AndVerify()
        {
            ECParameters key = CreateKey(ECCurve.NamedCurves.nistP384);
            EccPrivateKeyConverter priv = new EccPrivateKeyConverter(JWS_ECDSA.ES._384);
            EccPublicKeyConverter pub = new EccPublicKeyConverter(JWS_ECDSA.ES._384);

            string jwk = priv.ParamToJwk(key);
            Assert.That((string)JObject.Parse(jwk)["d"], Is.Not.Null);

            ECParameters back = priv.JwkToParam(jwk);
            Assert.That(back.D, Is.EqualTo(key.D));

            byte[] data = Encoding.UTF8.GetBytes("data");
            byte[] sign;
            using (ECDsa signer = ECDsa.Create(back)) { sign = signer.SignData(data, HashAlgorithmName.SHA384); }
            using (ECDsa verifier = ECDsa.Create(pub.JwkToParam(pub.ParamToJwk(key))))
            {
                Assert.That(verifier.VerifyData(data, sign, HashAlgorithmName.SHA384), Is.True);
            }
        }

        /// <summary>X 座標の長さから曲線を決める（32 / 48 / 66 バイト）。それ以外は ArgumentException。</summary>
        [Test]
        [Basis("RFC 7518 6.2.1.2（x は曲線の座標の大きさそのもの。P-521 なら 66 オクテット）")]
        public void GetCrvStringFromXCoordinate()
        {
            EccPublicKeyConverter conv = new EccPublicKeyConverter();

            Assert.That(conv.GetCrvStringFromXCoordinate(new byte[32]), Is.EqualTo("P-256"));
            Assert.That(conv.GetCrvStringFromXCoordinate(new byte[48]), Is.EqualTo("P-384"));
            Assert.That(conv.GetCrvStringFromXCoordinate(new byte[66]), Is.EqualTo("P-521"));
            Assert.Throws<ArgumentException>(() => conv.GetCrvStringFromXCoordinate(new byte[31]));
        }

        /// <summary>曲線から crv の文字列を引く。扱わない曲線は ArgumentOutOfRangeException。</summary>
        [Test]
        public void GetCrvStringFromECCurve()
        {
            EccPublicKeyConverter conv = new EccPublicKeyConverter();

            Assert.That(conv.GetCrvStringFromECCurve(ECCurve.NamedCurves.nistP521), Is.EqualTo("P-521"));
            Assert.That(conv.GetCrvStringFromECCurve(ECCurve.CreateFromFriendlyName("nistP384")), Is.EqualTo("P-384"));
            Assert.Throws<ArgumentOutOfRangeException>(() => conv.GetCrvStringFromECCurve(ECCurve.NamedCurves.brainpoolP256r1));
        }

        /// <summary>証明書：*.cer と *.pfx から取り出した公開鍵の JWK は同じ。</summary>
        [Test]
        public void X509_CerAndPfx_SameKey()
        {
            EccPublicKeyConverter conv = new EccPublicKeyConverter(JWS_ECDSA.ES._384);

            string fromCer = conv.X509CerToJwk(TestCertificates.EcdsaCer384);
            string fromPfx = conv.X509PfxToJwk(TestCertificates.EcdsaPfx384, TestCertificates.Password, HashAlgorithmName.SHA384);

            Assert.That(fromPfx, Is.EqualTo(fromCer));
            Assert.That((string)JObject.Parse(fromCer)["crv"], Is.EqualTo("P-384"));
            Assert.That(conv.X509CerToECParam(TestCertificates.EcdsaCer384).Q.X,
                Is.EqualTo(conv.X509PfxToParam(TestCertificates.EcdsaPfx384, TestCertificates.Password, HashAlgorithmName.SHA384).Q.X));
        }

        /// <summary>CNG（Windows）：証明書や JWK から CngKey を作れる。</summary>
        [Test]
        [Platform("Win")]
        public void Cng()
        {
            EccPublicKeyConverter conv = new EccPublicKeyConverter();

            Assert.That(conv.X509CerToCngkey(TestCertificates.EcdsaCer256).Algorithm, Is.EqualTo(CngAlgorithm.ECDsaP256));

#pragma warning disable CA1416 // Platform("Win") で Windows に限っている
            CngKey key = conv.JwkToCng(conv.X509CerToJwk(TestCertificates.EcdsaCer256));
#pragma warning restore CA1416
            Assert.That(key.KeySize, Is.EqualTo(256));
        }
    }
}

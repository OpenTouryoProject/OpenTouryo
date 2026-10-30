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
//* クラス名        ：CmnClientParamsTests
//* クラス日本語名  ：相手（OP / IdP）の値を設定から読む版の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/22  玄人 幸道         新規作成
//*  2026/10/22  玄人 幸道         各プロパティが対応するキーを読むことのテストを足した（カバレッジの拡充）
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）を付けた（#640）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Xml;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using TestCommon;

#if NETCOREAPP
using Microsoft.Extensions.Configuration;

using Touryo.Infrastructure.Public.Util;
#endif

using Touryo.Infrastructure.Framework.Authentication;
using Touryo.Infrastructure.Public.Security.Jwt;

namespace TestFramework.Authentication
{
    /// <summary>相手（OP / IdP）の値を設定（CmnClientParams）から読む版の単体テスト</summary>
    /// <remarks>
    /// 設定が空なら、設定を読む版は何もせずに null（検証は NoKey）を返す。通信もしない。
    /// 値を入れた確認は、構成を差し替えられる net10.0 だけで行う（net48 の AppSettings は実行中に書き換えられない）。
    /// </remarks>
    [TestFixture]
    public class CmnClientParamsTests
    {
        /// <summary>設定が空なら、どの版も null / NoKey を返し、通信しない。</summary>
        [Test]
        [Basis("設計：SAML2Client.VerifyResponseWithReason（設定の版）の remarks（SpRp_RsaCerFilePath が空なら NoKey）")]
        [Basis("設計：RemoteJwkSetStore・SAML2Metadata の remarks（引けなければ null を返し、例外にしない）")]
        public void EmptyConfig_ReturnsNothing()
        {
            using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, "{}")))
            {
                Assert.That(RemoteJwkSetStore.GetJwkAsync("kid01").Result, Is.Null);
                Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync("kid01").Result, Is.Null);
                Assert.That(SAML2Metadata.GetAsync().Result, Is.Null);
                Assert.That(http.Requests.Count, Is.EqualTo(0), "通信しない");
            }

            SAML2VerifyResult r = SAML2Client.VerifyResponseWithReason(null, "dummy", "", "", "");
            Assert.That(r.Reason, Is.EqualTo(SAML2VerifyResult.Reasons.NoKey));
            Assert.That(r.IsValid, Is.False);
        }

#if NETCOREAPP
        /// <summary>構成を差し替えて、テストの後で空に戻す</summary>
        private static void WithConfig(Dictionary<string, string> appSettings, Action action)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> kv in appSettings)
            {
                values.Add("appSettings:" + kv.Key, kv.Value);
            }

            try
            {
                GetConfigParameter.InitConfiguration(new ConfigurationBuilder().AddInMemoryCollection(values).Build());
                action();
            }
            finally
            {
                GetConfigParameter.InitConfiguration(
                    new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()).Build());
            }
        }

        /// <summary>RemoteJwkSetStore：JwkSetUri / SpRp_DiscoveryUri ＋ SpRp_Isser を読む。</summary>
        [Test]
        [Basis("設計：RemoteJwkSetStore.GetJwkAsync(kid)・GetJwkByDiscoveryAsync(kid) の remarks（JwkSetUri・SpRp_DiscoveryUri・SpRp_Isser を渡す）")]
        public void RemoteJwkSetStore_ReadsConfig()
        {
            JObject jwk;
            using (RSA rsa = RSA.Create(2048))
            {
                jwk = JObject.Parse(new RsaPublicKeyConverter().ParamToJwk(rsa.ExportParameters(false)));
            }
            string kid = (string)jwk["kid"];
            string jwks = new JObject(new JProperty("keys", new JArray(jwk))).ToString();

            RemoteJwkSetStore.Clear();

            WithConfig(new Dictionary<string, string>
            {
                { "JwkSetUri", "https://op.example.com/jwks" },
                { "SpRp_DiscoveryUri", "https://op.example.com/.well-known/openid-configuration" },
                { "SpRp_Isser", "https://op.example.com" }
            }, () =>
            {
                using (StubHttp http = new StubHttp((req, body) => req.RequestUri.AbsolutePath.EndsWith("/jwks")
                    ? StubHttp.Json(HttpStatusCode.OK, jwks)
                    : StubHttp.Json(HttpStatusCode.OK,
                        "{\"issuer\":\"https://op.example.com\",\"jwks_uri\":\"https://op.example.com/jwks\"}")))
                {
                    Assert.That(RemoteJwkSetStore.GetJwkAsync(kid).Result, Is.Not.Null);
                    Assert.That(RemoteJwkSetStore.GetJwkByDiscoveryAsync(kid).Result, Is.Not.Null);
                }
            });

            RemoteJwkSetStore.Clear();
        }

        /// <summary>
        /// SAML2Metadata.GetAsync()：SpRp_Saml2MetadataUri ＋ SpRp_Saml2EntityId を読む。
        /// VerifyResponseWithReason（設定の版）：SpRp_RsaCerFilePath ＋ SpRp_Saml2EntityId ＋ SpRp_Saml2AcsUri を読む。
        /// </summary>
        [Test]
        [Basis("設計：SAML2Metadata.GetAsync()・SAML2Client.VerifyResponseWithReason（設定の版）の remarks（読む設定の表）")]
        public void Saml2_ReadsConfig()
        {
            SAML2Metadata.Clear();

            string metadata = SAML2Bindings.CreateMetadata(SAML2BindingsTests.Issuer, "MIIB",
                new SAML2Enum.NameIDFormat[] { SAML2Enum.NameIDFormat.Unspecified },
                "https://idp.example.com/sso", "https://idp.example.com/sso").OuterXml;

            XmlDocument res = SAML2BindingsTests.CreateResponse(SAML2Enum.StatusCode.Success, true);
            string id = SAML2Bindings.GetIdInResponse(res, SAML2Bindings.CreateNamespaceManager(res));
            string saml = SAML2Bindings.EncodeAndSignPost(res.OuterXml, id, TestCertificates.RsaPrivateKey());

            Dictionary<string, string> config = new Dictionary<string, string>
            {
                { "SpRp_Saml2MetadataUri", "https://idp.example.com/samlmetadata" },
                { "SpRp_Saml2EntityId", SAML2BindingsTests.Issuer },
                { "SpRp_Saml2AcsUri", SAML2BindingsTests.Recipient },
                { "SpRp_RsaCerFilePath", TestCertificates.RsaCer }
            };

            WithConfig(config, () =>
            {
                using (StubHttp http = new StubHttp((req, body) => StubHttp.Json(HttpStatusCode.OK, metadata)))
                {
                    SAML2Metadata md = SAML2Metadata.GetAsync().Result;
                    Assert.That(md, Is.Not.Null);
                    Assert.That(md.EntityId, Is.EqualTo(SAML2BindingsTests.Issuer));
                }

                SAML2VerifyResult r = SAML2Client.VerifyResponseWithReason(null, saml, SAML2BindingsTests.InResponseTo, "", "");
                Assert.That(r.IsValid, Is.True, r.ReasonDescription);
                Assert.That(r.AudienceMatched, Is.True, "ACS を照合した");
            });

            config["SpRp_Saml2AcsUri"] = "https://other.example.com/acs";
            WithConfig(config, () =>
            {
                Assert.That(SAML2Client.VerifyResponseWithReason(null, saml, "", "", "").Reason,
                    Is.EqualTo(SAML2VerifyResult.Reasons.AudienceMismatch));
            });

            SAML2Metadata.Clear();
        }

        /// <summary>各プロパティは、対応する SpRp_* のキーを読む（キー名の取り違えが無いこと）。</summary>
        [Test]
        public void Properties_ReadTheirKeys()
        {
            Dictionary<string, Func<string>> props = new Dictionary<string, Func<string>>
            {
                { "SpRp_Isser", () => CmnClientParams.Isser },
                { "SpRp_AuthRequestUri", () => CmnClientParams.SpRp_AuthRequestUri },
                { "SpRp_TokenRequestUri", () => CmnClientParams.SpRp_TokenRequestUri },
                { "SpRp_UserInfoUri", () => CmnClientParams.SpRp_UserInfoUri },
                { "SpRp_RedirectUri", () => CmnClientParams.SpRp_RedirectUri },
                { "SpRp_DiscoveryUri", () => CmnClientParams.SpRp_DiscoveryUri },
                { "SpRp_Saml2MetadataUri", () => CmnClientParams.SpRp_Saml2MetadataUri },
                { "SpRp_Saml2EntityId", () => CmnClientParams.SpRp_Saml2EntityId },
                { "SpRp_Saml2AcsUri", () => CmnClientParams.SpRp_Saml2AcsUri },
                { "SpRp_RsaCerFilePath", () => CmnClientParams.RsaCerFilePath },
                { "SpRp_EcdsaCerFilePath", () => CmnClientParams.EcdsaCerFilePath },
                { "SpRp_RsaPfxPassword", () => CmnClientParams.RsaPfxPassword },
                { "SpRp_RsaPfxFilePath", () => CmnClientParams.RsaPfxFilePath },
                { "SpRp_EcdsaPfxPassword", () => CmnClientParams.EcdsaPfxPassword },
                { "SpRp_EcdsaPfxFilePath", () => CmnClientParams.EcdsaPfxFilePath },
                { "SpRp_ClientCertPfxPassword", () => CmnClientParams.ClientCertPfxPassword },
                { "SpRp_ClientCertPfxFilePath", () => CmnClientParams.ClientCertPfxFilePath }
            };

            // 値はキー名そのものにする（違うキーを読んでいれば、別の値になる）。
            Dictionary<string, string> config = new Dictionary<string, string>();
            foreach (string key in props.Keys)
            {
                config.Add(key, "value-of-" + key);
            }

            WithConfig(config, () =>
            {
                foreach (KeyValuePair<string, Func<string>> p in props)
                {
                    Assert.That(p.Value(), Is.EqualTo("value-of-" + p.Key), p.Key);
                }
            });
        }
#endif
    }
}

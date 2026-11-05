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
//*  2026/11/05  玄人 幸道         AccessToken.Verify（設定を読む版）が、設定の client_id で aud を確かめることのテストを足した（#655）
//*  2026/11/05  玄人 幸道         文字列でない aud・空・両方の設定・空白だけの設定のケースを足し、根拠を設計と RFC 7519 4.1.3 にした（#655 のレビュー）
//*  2026/11/05  玄人 幸道         読めない ClientIDs を断ること、IdToken・ResponseObject の aud の判定のテストを足した（#655 の 2 回目のレビュー）
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

        /// <summary>
        /// AccessToken.Verify（設定を読む版）：設定の client_id（OAuth2AndOidcClientID / OAuth2AndOidcClientIDs）が在れば、
        /// aud がそのいずれかを含むものだけを通す（#655）。
        /// </summary>
        [Test]
        [Basis("設計：AccessToken.Verify（設定を読む版）の remarks（設定の client_id で aud を確かめる。IdToken の設定を読む版と同じ判定）")]
        [Basis("RFC 7519 4.1.3（aud は StringOrURI か、その配列。自分を含まなければ断る）")]
        [Basis("#655")]
        public void AccessToken_VerifyAudBySettings()
        {
            const string iss = "https://as.example.com";
            JWS_RS256_Param signer = new JWS_RS256_Param(TestCertificates.RsaPrivateKey().ExportParameters(true));

            Func<JToken, string> create = aud =>
            {
                JObject p = new JObject
                {
                    [OAuth2AndOIDCConst.iss] = iss,
                    [OAuth2AndOIDCConst.sub] = "user01",
                    [OAuth2AndOIDCConst.exp] = DateTimeOffset.Now.ToUnixTimeSeconds() + 300,
                };
                if (aud != null)
                {
                    p[OAuth2AndOIDCConst.aud] = aud;
                }
                return signer.Create(p.ToString(Newtonsoft.Json.Formatting.None));
            };

            Func<string, bool> verify = at =>
            {
                string sub; List<string> roles; List<string> scopes; JObject jobj;
                return AccessToken.Verify(at, out sub, out roles, out scopes, out jobj);
            };

            // 受け付けるクライアントを ClientIDs に並べる（リソース サーバの使い方）
            WithConfig(new Dictionary<string, string>
            {
                { "SpRp_Isser", iss },
                { "SpRp_RsaCerFilePath", TestCertificates.RsaCer },
                { "OAuth2AndOidcClientIDs:0", "client01" },
                { "OAuth2AndOidcClientIDs:1", "client02" },
            }, () =>
            {
                Assert.That(verify(create("client01")), Is.True, "ClientIDs に在る");
                Assert.That(verify(create(new JArray("rs01", "client02"))), Is.True, "配列が ClientIDs の 1 つを含む");
                Assert.That(verify(create("client03")), Is.False, "ClientIDs に無い");
                Assert.That(verify(create(new JArray("rs01", "client03"))), Is.False, "配列が ClientIDs を含まない");
                Assert.That(verify(create(null)), Is.False, "aud が無い");
                Assert.That(verify(create("")), Is.False, "aud が空");
                Assert.That(verify(create(new JArray())), Is.False, "aud が空の配列");

                // 文字列でない aud は、例外にせず false（ContainsAud。#655 のレビュー）
                Assert.That(verify(create(new JValue(1))), Is.False, "aud が数値");
                Assert.That(verify(create(new JObject { ["x"] = "client01" })), Is.False, "aud がオブジェクト");
                JArray nested = new JArray();
                nested.Add(new JArray("client01")); // new JArray(new JArray(...)) は中身を写すので、入れ子にならない
                Assert.That(verify(create(nested)), Is.False, "aud が入れ子の配列");
                Assert.That(verify(create(new JArray("client01", 1))), Is.False, "aud の配列に文字列でない要素");
            });

            // ClientID と ClientIDs の両方（合わせて受ける）
            WithConfig(new Dictionary<string, string>
            {
                { "SpRp_Isser", iss },
                { "SpRp_RsaCerFilePath", TestCertificates.RsaCer },
                { "OAuth2AndOidcClientID", "client01" },
                { "OAuth2AndOidcClientIDs:0", "client02" },
            }, () =>
            {
                Assert.That(verify(create("client01")), Is.True, "ClientID と一致");
                Assert.That(verify(create("client02")), Is.True, "ClientIDs と一致");
                Assert.That(verify(create("client03")), Is.False, "どちらとも違う");
            });

            // ClientIDs が空白・空文字だけなら、ClientID だけで判定する
            WithConfig(new Dictionary<string, string>
            {
                { "SpRp_Isser", iss },
                { "SpRp_RsaCerFilePath", TestCertificates.RsaCer },
                { "OAuth2AndOidcClientID", "client01" },
                { "OAuth2AndOidcClientIDs:0", "" },
                { "OAuth2AndOidcClientIDs:1", "  " },
            }, () =>
            {
                Assert.That(verify(create("client01")), Is.True, "ClientID と一致");
                Assert.That(verify(create("  ")), Is.False, "空白だけの値とは照合しない");
            });

            // ClientID だけ（クライアントの使い方）
            WithConfig(new Dictionary<string, string>
            {
                { "SpRp_Isser", iss },
                { "SpRp_RsaCerFilePath", TestCertificates.RsaCer },
                { "OAuth2AndOidcClientID", "client01" },
            }, () =>
            {
                Assert.That(verify(create("client01")), Is.True, "ClientID と一致");
                Assert.That(verify(create("client02")), Is.False, "ClientID と違う");
            });
        }

        /// <summary>
        /// AccessToken.Verify（設定を読む版）：設定の client_id が無ければ、従来どおり aud を確かめない（#655。4.0 で断る予定）。
        /// </summary>
        [Test]
        [Observation("設定が無いときに aud を確かめないのは、壊さないための従来の振る舞い（Keycloak コンパチ）。4.0 で断る既定に変える予定（#655）なので、変えたら気付くために残す")]
        public void AccessToken_NoClientIDs_SkipsAud()
        {
            const string iss = "https://as.example.com";
            JWS_RS256_Param signer = new JWS_RS256_Param(TestCertificates.RsaPrivateKey().ExportParameters(true));

            WithConfig(new Dictionary<string, string>
            {
                { "SpRp_Isser", iss },
                { "SpRp_RsaCerFilePath", TestCertificates.RsaCer },
            }, () =>
            {
                JObject p = new JObject
                {
                    [OAuth2AndOIDCConst.iss] = iss,
                    [OAuth2AndOIDCConst.sub] = "user01",
                    [OAuth2AndOIDCConst.aud] = "anything",
                    [OAuth2AndOIDCConst.exp] = DateTimeOffset.Now.ToUnixTimeSeconds() + 300,
                };

                string sub; List<string> roles; List<string> scopes; JObject jobj;
                Assert.That(AccessToken.Verify(signer.Create(p.ToString(Newtonsoft.Json.Formatting.None)),
                    out sub, out roles, out scopes, out jobj), Is.True, "設定が無ければ aud を確かめない");
                Assert.That(sub, Is.EqualTo("user01"));
            });

            // 空白・空文字だけの値も、設定が無いのと同じ（環境変数の置き換えで空になった値など。#655 のレビュー）
            WithConfig(new Dictionary<string, string>
            {
                { "SpRp_Isser", iss },
                { "SpRp_RsaCerFilePath", TestCertificates.RsaCer },
                { "OAuth2AndOidcClientID", "  " },
                { "OAuth2AndOidcClientIDs:0", "" },
            }, () =>
            {
                JObject p = new JObject
                {
                    [OAuth2AndOIDCConst.iss] = iss,
                    [OAuth2AndOIDCConst.sub] = "user01",
                    [OAuth2AndOIDCConst.aud] = "anything",
                    [OAuth2AndOIDCConst.exp] = DateTimeOffset.Now.ToUnixTimeSeconds() + 300,
                };

                string sub; List<string> roles; List<string> scopes; JObject jobj;
                Assert.That(AccessToken.Verify(signer.Create(p.ToString(Newtonsoft.Json.Formatting.None)),
                    out sub, out roles, out scopes, out jobj), Is.True, "空白・空文字だけなら、設定が無いのと同じ");
            });
        }

        /// <summary>設定を読む版のテスト用：iss・sub・aud・exp を持つ JWT を、テストの証明書の秘密鍵で作る（kid は付けない）</summary>
        private static string CreateSettingsJwt(string iss, JToken aud)
        {
            JObject p = new JObject
            {
                [OAuth2AndOIDCConst.iss] = iss,
                [OAuth2AndOIDCConst.sub] = "user01",
                [OAuth2AndOIDCConst.exp] = DateTimeOffset.Now.ToUnixTimeSeconds() + 300,
            };
            if (aud != null)
            {
                p[OAuth2AndOIDCConst.aud] = aud;
            }
            return new JWS_RS256_Param(TestCertificates.RsaPrivateKey().ExportParameters(true))
                .Create(p.ToString(Newtonsoft.Json.Formatting.None));
        }

        /// <summary>
        /// AccessToken.Verify（設定を読む版）：OAuth2AndOidcClientIDs が文字列の配列として読めない設定は、
        /// 例外にせず、すべて断る（設定の誤り。#655 の 2 回目のレビュー）。
        /// </summary>
        [Test]
        [Basis("設計：CmnJwtToken.ReadConfiguredClientIDs・AccessToken.Verify（設定を読む版）の remarks（ClientIDs が読めなければ、設定の誤りとして断る）")]
        [Basis("#655")]
        public void AccessToken_BrokenClientIDs_Rejects()
        {
            const string iss = "https://as.example.com";

            // 配列の要素が、文字列でなくオブジェクト（OAuth2AndOidcClientIDs:0:x）
            WithConfig(new Dictionary<string, string>
            {
                { "SpRp_Isser", iss },
                { "SpRp_RsaCerFilePath", TestCertificates.RsaCer },
                { "OAuth2AndOidcClientIDs:0:x", "client01" },
            }, () =>
            {
                string sub; List<string> roles; List<string> scopes; JObject jobj;
                bool actual = false;
                Assert.DoesNotThrow(() => actual = AccessToken.Verify(CmnClientParamsTests.CreateSettingsJwt(iss, "client01"),
                    out sub, out roles, out scopes, out jobj), "例外にしない");
                Assert.That(actual, Is.False, "読めない設定は、すべて断る");
            });

            // 配列でなく、1 つの値（OAuth2AndOidcClientIDs = "client01"）
            WithConfig(new Dictionary<string, string>
            {
                { "SpRp_Isser", iss },
                { "SpRp_RsaCerFilePath", TestCertificates.RsaCer },
                { "OAuth2AndOidcClientIDs", "client01" },
            }, () =>
            {
                string sub; List<string> roles; List<string> scopes; JObject jobj;
                bool actual = true;
                Assert.DoesNotThrow(() => actual = AccessToken.Verify(CmnClientParamsTests.CreateSettingsJwt(iss, "client01"),
                    out sub, out roles, out scopes, out jobj), "例外にしない（配列でない値）");
                Assert.That(actual, Is.False, "配列でない値も、設定の誤りとして断る");
            });
        }

        /// <summary>
        /// IdToken.Verify・ResponseObject.Verify（設定を読む版）も、aud を AccessToken と同じ判定（VerifyAudByClientIDs）で確かめる。
        /// 文字列でない aud は、例外にせず false（2026/11/05 まで、数値は文字列に変えて照合し、オブジェクトは例外だった。#655）。
        /// </summary>
        [Test]
        [Basis("RFC 7519 4.1.3（aud は StringOrURI か、その配列）")]
        [Basis("設計：CmnJwtToken.VerifyAudByClientIDs の remarks（文字列でない aud は false。例外にしない）")]
        [Basis("#655")]
        public void IdTokenAndResponseObject_VerifyAudByClientIDs()
        {
            const string iss = "https://as.example.com";

            WithConfig(new Dictionary<string, string>
            {
                { "SpRp_Isser", iss },
                { "SpRp_RsaCerFilePath", TestCertificates.RsaCer },
                { "OAuth2AndOidcClientIDs:0", "123" },
                { "OAuth2AndOidcClientIDs:1", "client01" },
            }, () =>
            {
                Func<JToken, bool> idToken = aud =>
                {
                    string sub, nonce; JObject jobj;
                    return IdToken.Verify(CmnClientParamsTests.CreateSettingsJwt(iss, aud), "", "", "", out sub, out nonce, out jobj);
                };
                Func<JToken, bool> responseObject = aud =>
                {
                    JObject jobj;
                    return ResponseObject.Verify(CmnClientParamsTests.CreateSettingsJwt(iss, aud), out jobj);
                };

                foreach (KeyValuePair<string, Func<JToken, bool>> v in new Dictionary<string, Func<JToken, bool>>
                {
                    { "IdToken", idToken },
                    { "ResponseObject", responseObject },
                })
                {
                    Assert.That(v.Value("client01"), Is.True, v.Key + "：ClientIDs と一致");
                    Assert.That(v.Value("client02"), Is.False, v.Key + "：ClientIDs と違う");
                    Assert.That(v.Value(new JValue(123)), Is.False, v.Key + "：数値の aud（文字列に変えて照合しない）");

                    bool actual = true;
                    Assert.DoesNotThrow(() => actual = v.Value(new JObject { ["x"] = "client01" }), v.Key + "：オブジェクトの aud で例外にしない");
                    Assert.That(actual, Is.False, v.Key + "：オブジェクトの aud");
                }
            });
        }
#endif
    }
}

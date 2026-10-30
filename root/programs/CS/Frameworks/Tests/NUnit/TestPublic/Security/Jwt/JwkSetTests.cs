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
//* クラス名        ：JwkSetTests
//* クラス日本語名  ：JwkSet（JWK Set の読み書き）の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/18  玄人 幸道         新規作成（カバレッジの拡充）
//*  2026/10/30  玄人 幸道         期待値の根拠（Basis）と観測（Observation）を付けた（#640）
//**********************************************************************************

using System;
using System.IO;

using Newtonsoft.Json.Linq;

using NUnit.Framework;

using TestCommon;

using Touryo.Infrastructure.Public.Security.Jwt;

namespace TestPublic.Security.Jwt
{
    /// <summary>JwkSet（JWK Set の読み書き）の単体テスト</summary>
    [TestFixture]
    public class JwkSetTests
    {
        /// <summary>作業フォルダ（%TEMP%\OpenTouryoNUnit\GUID）</summary>
        private string _dir;

        /// <summary>作業フォルダを作る。</summary>
        [SetUp]
        public void SetUp()
        {
            this._dir = Path.Combine(Path.GetTempPath(), "OpenTouryoNUnit", Guid.NewGuid().ToString());
            Directory.CreateDirectory(this._dir);
        }

        /// <summary>作業フォルダを消す。</summary>
        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(this._dir)) { Directory.Delete(this._dir, true); }
        }

        /// <summary>kid と値を持つ JWK</summary>
        private static JObject Jwk(string kid, string n)
        {
            return JObject.Parse("{\"kty\":\"RSA\",\"kid\":\"" + kid + "\",\"n\":\"" + n + "\",\"e\":\"AQAB\"}");
        }

        /// <summary>**同じ kid の JWK は足さない**（先に入っていたものが残る）。kid で引ける。無ければ null。</summary>
        [Test]
        [Observation("同じ kid を足さずに先のものを残すことは、AddJwkToJwkSet の XML コメントに書かれていない（RFC 7517 4.5 は、JWK Set の中で kid を分けることを SHOULD とするだけ。今の振る舞いを写した）")]
        public void Add_And_Get()
        {
            JwkSet set = new JwkSet();
            JwkSet.AddJwkToJwkSet(set, Jwk("k1", "AAA"));
            JwkSet.AddJwkToJwkSet(set, Jwk("k2", "BBB"));
            JwkSet returned = JwkSet.AddJwkToJwkSet(set, Jwk("k1", "CCC"));

            Assert.That(returned, Is.SameAs(set));
            Assert.That(set.keys, Has.Count.EqualTo(2));
            Assert.That((string)JwkSet.GetJwkObject(set, "k1")["n"], Is.EqualTo("AAA"));
            Assert.That((string)JwkSet.GetJwkObject(set, "k2")["n"], Is.EqualTo("BBB"));
            Assert.That(JwkSet.GetJwkObject(set, "none"), Is.Null);
        }

        /// <summary>ファイルに保存して読み戻せる。保存は keys（小文字）の JSON。</summary>
        [Test]
        [Basis("RFC 7517 5（JWK Set は keys の配列を持つ）")]
        public void Save_And_Load()
        {
            string path = Path.Combine(this._dir, "jwks.json");
            JwkSet set = new JwkSet();
            JwkSet.AddJwkToJwkSet(set, Jwk("k1", "AAA"));
            JwkSet.AddJwkToJwkSet(set, Jwk("k2", "BBB"));

            JwkSet.SaveJwkSet(path, set);

            Assert.That(JObject.Parse(File.ReadAllText(path))["keys"], Is.Not.Null);

            JwkSet loaded = JwkSet.LoadJwkSet(path);
            Assert.That(loaded.keys, Has.Count.EqualTo(2));
            Assert.That((string)JwkSet.GetJwkObject(loaded, "k2")["n"], Is.EqualTo("BBB"));
        }
    }
}

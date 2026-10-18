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
//* クラス名        ：ResourceLoaderTests
//* クラス日本語名  ：ResourceLoader / EmbeddedResourceLoader の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//*  2026/10/19  玄人 幸道         ResolveFilePath_ExpandsEnvironmentVariable：Linux でも成り立つよう、区切りを Path.Combine にした（#632）
//**********************************************************************************

using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Touryo.Infrastructure.Public.IO;

namespace TestPublic.IO
{
    /// <summary>ResourceLoader / EmbeddedResourceLoader の単体テスト</summary>
    [TestFixture]
    public class ResourceLoaderTests
    {
        /// <summary>OpenTouryo.Public に埋め込まれているリソース（XmlLib の検証用）</summary>
        private const string PublicAssembly = "OpenTouryo.Public";
        private const string EmbeddedXml = "Touryo.Infrastructure.Public.Xml.TestXml.xml";

        private const string EnvName = "OT_NUNIT_RESOURCE_DIR";

        /// <summary>テストごとの作業フォルダ</summary>
        private string _work;

        /// <summary>作業フォルダを作る。</summary>
        [SetUp]
        public void SetUp()
        {
            this._work = Path.Combine(Path.GetTempPath(), "OpenTouryoNUnit", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(this._work);
            File.WriteAllText(Path.Combine(this._work, "a.txt"), "あいう", Encoding.UTF8);
        }

        /// <summary>作業フォルダと環境変数を戻す。</summary>
        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable(EnvName, null, EnvironmentVariableTarget.Process);
            if (Directory.Exists(this._work)) { Directory.Delete(this._work, true); }
        }

        #region ResourceLoader

        /// <summary>パスを直接、またはフォルダとファイル名で指定して読む。</summary>
        [Test]
        public void LoadAsString()
        {
            Assert.That(ResourceLoader.LoadAsString(Path.Combine(this._work, "a.txt"), Encoding.UTF8),
                Is.EqualTo("あいう"));
            Assert.That(ResourceLoader.LoadAsString(this._work, "a.txt", Encoding.UTF8),
                Is.EqualTo("あいう"));
        }

        /// <summary>パスの中の %環境変数% を展開してから探す。</summary>
        [Test]
        public void ResolveFilePath_ExpandsEnvironmentVariable()
        {
            Environment.SetEnvironmentVariable(EnvName, this._work, EnvironmentVariableTarget.Process);

            // 区切りは Path.Combine に任せる（#632。"\\" 決め打ちでは Linux で見つからない）
            Assert.That(ResourceLoader.ResolveFilePath(Path.Combine("%" + EnvName + "%", "a.txt")),
                Is.EqualTo(Path.Combine(this._work, "a.txt")));
        }

        /// <summary>
        /// 相対パスで見つからなければ、EXE の配置フォルダ（AppContext.BaseDirectory）基準で探す。
        /// </summary>
        [Test]
        public void ResolveFilePath_RelativeToBaseDirectory()
        {
            // テスト アセンブリは BaseDirectory に置かれている
            string name = Path.GetFileName(typeof(ResourceLoaderTests).Assembly.Location);

            // カレント フォルダで見つかると (1) で返ってしまうので、カレントを外す。
            string current = Environment.CurrentDirectory;
            try
            {
                Environment.CurrentDirectory = this._work;

                Assert.That(ResourceLoader.ResolveFilePath(name),
                    Is.EqualTo(Path.Combine(AppContext.BaseDirectory, name)));
            }
            finally
            {
                Environment.CurrentDirectory = current;
            }
        }

        /// <summary>見つからなければ null。空・null も null。</summary>
        [Test]
        public void ResolveFilePath_NotFound()
        {
            Assert.That(ResourceLoader.ResolveFilePath(Path.Combine(this._work, "none.txt")), Is.Null);
            Assert.That(ResourceLoader.ResolveFilePath(""), Is.Null);
            Assert.That(ResourceLoader.ResolveFilePath(null), Is.Null);
        }

        /// <summary>Exists は、throwException の指定で false を返すか例外にするかを選べる。</summary>
        [Test]
        public void Exists()
        {
            string none = Path.Combine(this._work, "none.txt");

            Assert.That(ResourceLoader.Exists(Path.Combine(this._work, "a.txt"), false), Is.True);
            Assert.That(ResourceLoader.Exists(this._work, "a.txt", false), Is.True);
            Assert.That(ResourceLoader.Exists(none, false), Is.False);
            Assert.Throws<ArgumentException>(() => ResourceLoader.Exists(none, true));
            Assert.Throws<ArgumentException>(() => ResourceLoader.LoadAsString(none, Encoding.UTF8));
        }

        #endregion

        #region EmbeddedResourceLoader

        /// <summary>
        /// アセンブリ名と、埋め込みリソースの論理名（既定の名前空間 ＋ フォルダ ＋ ファイル名）で読む。
        /// </summary>
        [Test]
        public void Embedded_LoadAsString()
        {
            Assert.That(EmbeddedResourceLoader.Exists(PublicAssembly, EmbeddedXml, false), Is.True);

            string xml = EmbeddedResourceLoader.LoadAsString(PublicAssembly, EmbeddedXml, Encoding.UTF8);
            Assert.That(xml, Does.Contain("<title>The 7 Habits</title>"));

            using (Stream s = EmbeddedResourceLoader.LoadAsStream(PublicAssembly, EmbeddedXml))
            {
                Assert.That(s, Is.Not.Null);
                Assert.That(s.Length, Is.GreaterThan(0));
            }
        }

        /// <summary>LoadXMLAsString は XML として読み直した文字列を返す。</summary>
        [Test]
        public void Embedded_LoadXMLAsString()
        {
            string xml = EmbeddedResourceLoader.LoadXMLAsString(PublicAssembly, EmbeddedXml);

            Assert.That(xml, Does.Contain("bookstore"));
            Assert.That(xml, Does.Contain("Keith"));
        }

        /// <summary>
        /// 存在しないリソースは、Exists が false（または例外）、LoadAsString は例外。
        /// **LoadAsStream は例外にならず null を返す**（GetManifestResourceStream の仕様）。
        /// </summary>
        [Test]
        public void Embedded_NotFound()
        {
            const string none = "Touryo.Infrastructure.Public.Xml.None.xml";

            Assert.That(EmbeddedResourceLoader.Exists(PublicAssembly, none, false), Is.False);
            Assert.Throws<ArgumentException>(() => EmbeddedResourceLoader.Exists(PublicAssembly, none, true));
            Assert.Throws<ArgumentException>(() => EmbeddedResourceLoader.LoadAsString(PublicAssembly, none, Encoding.UTF8));
            Assert.That(EmbeddedResourceLoader.LoadAsStream(PublicAssembly, none), Is.Null);
        }

        #endregion
    }
}

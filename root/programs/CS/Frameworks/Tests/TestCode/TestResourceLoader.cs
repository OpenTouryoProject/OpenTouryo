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
//* クラス名        ：TestResourceLoader
//* クラス日本語名  ：ResourceLoader・EmbeddedResourceLoaderのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/06  玄人 幸道         新規作成（#522）
//**********************************************************************************

using System;
using System.IO;
using System.Text;

using Touryo.Infrastructure.Public.Diagnostics;
using Touryo.Infrastructure.Public.IO;

namespace TestCode
{
    /// <summary>ResourceLoader・EmbeddedResourceLoaderのテスト</summary>
    /// <remarks>
    /// SQL 定義ファイルやログ設定など、**リソースの読み込み全般がここに乗る**（#522）。
    ///
    /// ＜なぜここを見るか＞
    ///   実際に踏んだ失敗がある。SimpleBatch が
    ///     resource file [C:\...\ShipperCount.sql] was not found.
    ///   で落ちたのは、この経路で解決できなかったため。
    ///
    /// ＜埋め込みリソース＞
    ///   MyBaseDao.UseEmbeddedResource を立てると、SQL 定義ファイルを
    ///   DLL に埋め込んだ構成に切り替わる（PaaS 向け）。その読み手がこちら。
    ///
    /// ＜出力にパスを出さないこと＞
    ///   一時フォルダも配置フォルダも実行環境で変わる。
    ///   **結果ファイルの比較に載せるのは、真偽と中身だけにする。**
    /// </remarks>
    public class TestResourceLoader
    {
        #region 定数

        /// <summary>埋め込みリソースの論理名</summary>
        /// <remarks>
        /// csproj で LogicalName を明示しているため、net48 と .NET (Core) で同じ名前になる。
        /// **既定のままだと "既定の名前空間 + ファイル名" になり、
        /// プロジェクト名が違う分だけ名前が食い違う。**
        /// </remarks>
        private const string EmbeddedName = "TestCode.TestEmbedded.txt";

        #endregion

        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            TestResourceLoader.TestResolveAndLoad();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestResourceLoader.TestEmbedded();
        }

        #endregion

        #region private

        /// <summary>実ファイルの解決と読み込み</summary>
        private static void TestResolveAndLoad()
        {
            MyDebug.OutputDebugAndConsole("ResourceLoader");

            string dir = Path.Combine(Path.GetTempPath(), "OpenTouryoTestResource");
            string path = Path.Combine(dir, "TestResource.txt");

            Directory.CreateDirectory(dir);
            File.WriteAllText(path, "リソースの内容", new UTF8Encoding(false));

            const string envName = "OPENTOURYO_TEST_RESDIR";
            string orgEnv = Environment.GetEnvironmentVariable(envName, EnvironmentVariableTarget.Process);

            try
            {
                // --- 絶対パス ---
                MyDebug.OutputDebugAndConsole(
                    "絶対パスで解決        : " + (ResourceLoader.ResolveFilePath(path) != null));
                MyDebug.OutputDebugAndConsole(
                    "絶対パスで読み込み    : " + ResourceLoader.LoadAsString(path, Encoding.UTF8));

                // --- 環境変数入りのパス ---
                // ResolveFilePath は先頭で環境変数を展開する。
                Environment.SetEnvironmentVariable(envName, dir, EnvironmentVariableTarget.Process);

                MyDebug.OutputDebugAndConsole(
                    "環境変数入りで解決    : "
                    + (ResourceLoader.ResolveFilePath("%" + envName + "%\\TestResource.txt") != null));

                // --- EXE の配置フォルダ基準の相対パス ---
                // SampleLogConf.xml は出力フォルダにコピーされる（csproj の Content）。
                // カレント ディレクトリに関係なく解決できること。
                MyDebug.OutputDebugAndConsole(
                    "配置フォルダ基準      : "
                    + (ResourceLoader.ResolveFilePath("SampleLogConf.xml") != null));

                // --- 見つからない場合 ---
                MyDebug.OutputDebugAndConsole(
                    "見つからない（解決）  : "
                    + (ResourceLoader.ResolveFilePath("NotExist_OpenTouryo.txt") == null ? "null" : "not null"));

                MyDebug.OutputDebugAndConsole(
                    "見つからない（Exists）: "
                    + ResourceLoader.Exists("NotExist_OpenTouryo.txt", false));

                // throwException = true の場合
                try
                {
                    ResourceLoader.Exists("NotExist_OpenTouryo.txt", true);
                    MyDebug.OutputDebugAndConsole("見つからない（例外）  : 例外にならない");
                }
                catch (Exception ex)
                {
                    // メッセージは環境の言語で変わるため、型名だけを出す。
                    MyDebug.OutputDebugAndConsole("見つからない（例外）  : " + ex.GetType().FullName);
                }

                // --- 空とnull ---
                MyDebug.OutputDebugAndConsole(
                    "空文字（解決）        : "
                    + (ResourceLoader.ResolveFilePath("") == null ? "null" : "not null"));
                MyDebug.OutputDebugAndConsole(
                    "null（解決）          : "
                    + (ResourceLoader.ResolveFilePath(null) == null ? "null" : "not null"));
            }
            finally
            {
                // 後始末。**環境変数は必ず戻す。**
                Environment.SetEnvironmentVariable(envName, orgEnv, EnvironmentVariableTarget.Process);

                try
                {
                    File.Delete(path);
                    Directory.Delete(dir);
                }
                catch
                {
                    // 消せなくてもテストの成否には関係しない。
                }
            }
        }

        /// <summary>埋め込みリソースの読み込み</summary>
        private static void TestEmbedded()
        {
            MyDebug.OutputDebugAndConsole("EmbeddedResourceLoader");

            MyDebug.OutputDebugAndConsole(
                "存在する              : "
                + EmbeddedResourceLoader.Exists(TestResourceLoader.EmbeddedName, false));

            MyDebug.OutputDebugAndConsole(
                "読み込み              : "
                + EmbeddedResourceLoader.LoadAsString(TestResourceLoader.EmbeddedName, Encoding.UTF8).Trim());

            MyDebug.OutputDebugAndConsole(
                "存在しない            : "
                + EmbeddedResourceLoader.Exists("TestCode.NotExist.txt", false));

            // throwException = true の場合
            try
            {
                EmbeddedResourceLoader.Exists("TestCode.NotExist.txt", true);
                MyDebug.OutputDebugAndConsole("存在しない（例外）    : 例外にならない");
            }
            catch (Exception ex)
            {
                MyDebug.OutputDebugAndConsole("存在しない（例外）    : " + ex.GetType().FullName);
            }
        }

        #endregion
    }
}

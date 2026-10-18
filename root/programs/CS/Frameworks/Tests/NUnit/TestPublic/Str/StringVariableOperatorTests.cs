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
//* クラス名        ：StringVariableOperatorTests
//* クラス日本語名  ：StringVariableOperator の単体テスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//*  2026/10/16  玄人 幸道         構文エラーとコマンドライン引数のケースを追加（カバレッジの拡充）
//**********************************************************************************

using System;
using System.Collections.Generic;

using NUnit.Framework;

using Touryo.Infrastructure.Public.Str;

namespace TestPublic.Str
{
    /// <summary>StringVariableOperator の単体テスト</summary>
    [TestFixture]
    public class StringVariableOperatorTests
    {
        #region プロパティ文字列

        /// <summary>
        /// property=value;... を解析する。
        /// **プロパティ名は大文字に揃える。** 値はそのまま。
        /// </summary>
        [Test]
        public void GetPropsFromPropString_Basic()
        {
            Dictionary<string, string> props =
                StringVariableOperator.GetPropsFromPropString("Server=localhost;Database=Northwind;");

            Assert.That(props.Count, Is.EqualTo(2));
            Assert.That(props["SERVER"], Is.EqualTo("localhost"));
            Assert.That(props["DATABASE"], Is.EqualTo("Northwind"));
        }

        /// <summary>末尾の ; は省略できる。値は空でもよい。</summary>
        [Test]
        public void GetPropsFromPropString_NoTrailingSemicolonAndEmptyValue()
        {
            Dictionary<string, string> props = StringVariableOperator.GetPropsFromPropString("a=1;b=");

            Assert.That(props["A"], Is.EqualTo("1"));
            Assert.That(props["B"], Is.EqualTo(""));
        }

        /// <summary>{=} と {;} で、= と ; を名前・値に含められる。</summary>
        [Test]
        public void GetPropsFromPropString_Escape()
        {
            Dictionary<string, string> props = StringVariableOperator.GetPropsFromPropString("a{=}b=x{;}y");

            Assert.That(props["A=B"], Is.EqualTo("x;y"));
        }

        /// <summary>同じ名前は後勝ち（大文字・小文字を区別しない）。</summary>
        [Test]
        public void GetPropsFromPropString_DuplicateOverwrites()
        {
            Dictionary<string, string> props = StringVariableOperator.GetPropsFromPropString("a=1;A=2");

            Assert.That(props.Count, Is.EqualTo(1));
            Assert.That(props["A"], Is.EqualTo("2"));
        }

        /// <summary>構文エラーは ArgumentException。</summary>
        [TestCase("=a")]          // 先頭が区切り
        [TestCase("a==b")]        // 値の中に区切り
        [TestCase("a=1;;")]       // 名前の中に区切り
        [TestCase("=")]           // 名前が空
        [TestCase("a{x}=1")]      // エスケープできない文字
        [TestCase("a}=1")]        // 対応しない閉じ括弧
        [TestCase("a{=x")]        // エスケープが閉じていない
        [TestCase("a=1;=b")]      // 2 つ目の名前が空
        public void GetPropsFromPropString_FormatError(string propString)
        {
            Assert.Throws<ArgumentException>(() => StringVariableOperator.GetPropsFromPropString(propString));
        }

        /// <summary>先頭からエスケープで始めてもよい。</summary>
        [Test]
        public void GetPropsFromPropString_EscapeAtStart()
        {
            Dictionary<string, string> props = StringVariableOperator.GetPropsFromPropString("{=}a=1");

            Assert.That(props["=A"], Is.EqualTo("1"));
        }

        #endregion

        #region コマンドライン引数

        /// <summary>
        /// GetCommandArgs は、**このプロセスの**コマンドライン（Environment.CommandLine）を読む。
        /// 先頭（実行ファイル）を除き、接頭辞で始まるものをキー（大文字）、続くものを値とする。
        /// </summary>
        /// <remarks>
        /// テストでは引数を与えられないので、テスト ホストの起動引数（--port 番号 など）で確かめる。
        /// 期待値は Environment.GetCommandLineArgs から組み立てる。
        /// 引用符・円記号・空白を含む引数は解釈が分かれるので、比べる対象から外す。
        /// </remarks>
        [Test]
        public void GetCommandArgs_ReadsThisProcess()
        {
            Dictionary<string, string> argsDic;
            List<string> valsLst;
            StringVariableOperator.GetCommandArgs('-', out argsDic, out valsLst);

            Assert.That(argsDic, Is.Not.Null);
            Assert.That(valsLst, Is.Not.Null);

            string[] args = Environment.GetCommandLineArgs();
            int compared = 0;
            for (int i = 1; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-") || !IsPlain(args[i])) { continue; }

                if (i + 1 < args.Length && !args[i + 1].StartsWith("-") && IsPlain(args[i + 1]))
                {
                    // キーと値
                    Assert.That(argsDic[args[i].ToUpper()], Is.EqualTo(args[i + 1]), args[i]);
                    compared++;
                }
            }

            // テスト ホストは --port などを必ず渡すので、1 つは比べられるはず。
            Assert.That(compared, Is.GreaterThan(0), string.Join(" ", args));
        }

        /// <summary>解釈が分かれない引数か（引用符・円記号・空白を含まない）</summary>
        private static bool IsPlain(string arg)
        {
            return arg.Length != 0 && arg.IndexOfAny(new char[] { '"', '\\', ' ' }) < 0;
        }

        #endregion

        #region 環境変数の組み込み

        private const string EnvName = "OT_NUNIT_TEST_VAR";

        /// <summary>テスト用の環境変数を設定する（プロセスの範囲）。</summary>
        [SetUp]
        public void SetUp()
        {
            Environment.SetEnvironmentVariable(EnvName, "VALUE", EnvironmentVariableTarget.Process);
        }

        /// <summary>テスト用の環境変数を消す。</summary>
        [TearDown]
        public void TearDown()
        {
            Environment.SetEnvironmentVariable(EnvName, null, EnvironmentVariableTarget.Process);
        }

        /// <summary>%名前% を環境変数の値に置き換える。</summary>
        [Test]
        public void BuiltStringIntoEnvironmentVariable_Replaces()
        {
            Assert.That(StringVariableOperator.BuiltStringIntoEnvironmentVariable(
                @"C:\%" + EnvName + @"%\end"), Is.EqualTo(@"C:\VALUE\end"));
        }

        /// <summary>**未定義の環境変数は空になる**（%名前% は残らない）。</summary>
        [Test]
        public void BuiltStringIntoEnvironmentVariable_UndefinedBecomesEmpty()
        {
            Assert.That(StringVariableOperator.BuiltStringIntoEnvironmentVariable(
                "a%OT_NUNIT_UNDEFINED_VAR%b"), Is.EqualTo("ab"));
        }

        /// <summary>対になっていない % はそのまま残す。null・空・% を含まない文字列は、そのまま返す。</summary>
        [Test]
        public void BuiltStringIntoEnvironmentVariable_Passthrough()
        {
            Assert.That(StringVariableOperator.BuiltStringIntoEnvironmentVariable("100%"), Is.EqualTo("100%"));
            Assert.That(StringVariableOperator.BuiltStringIntoEnvironmentVariable("abc"), Is.EqualTo("abc"));
            Assert.That(StringVariableOperator.BuiltStringIntoEnvironmentVariable(""), Is.EqualTo(""));
            Assert.That(StringVariableOperator.BuiltStringIntoEnvironmentVariable(null), Is.Null);
        }

        #endregion
    }
}

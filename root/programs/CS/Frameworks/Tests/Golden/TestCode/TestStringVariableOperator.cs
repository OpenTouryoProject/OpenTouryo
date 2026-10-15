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
//* クラス名        ：TestStringVariableOperator
//* クラス日本語名  ：StringVariableOperatorのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/06  玄人 幸道         新規作成（#522）
//**********************************************************************************

using System;
using System.Collections.Generic;

using Touryo.Infrastructure.Public.Diagnostics;
using Touryo.Infrastructure.Public.Str;

namespace TestCode
{
    /// <summary>StringVariableOperatorのテスト</summary>
    /// <remarks>
    /// プロパティ文字列の解析と、環境変数の埋め込みを確認する（#522）。
    ///
    /// ＜GetCommandArgs は対象外＞
    ///   **Environment.CommandLine を直接読むため、単体テストでは扱えない。**
    ///   引数を変えて確かめるには、実際にプロセスを起動するしかない。
    ///   このため 3_SmokeTest.ps1 の DaoGen_Tool（CUI）が実質の確認になっている。
    ///
    ///   **なお、あちらには「\ がエスケープ文字」という罠がある**（#508 で踏んだ）。
    ///   パス区切りに \ を使うと消えてしまい、**終了コードは 0 のまま
    ///   別の場所に出力される**。詳細は Tools/DaoGen_Tool/README.md。
    /// </remarks>
    public class TestStringVariableOperator
    {
        #region public

        /// <summary>Root</summary>
        public static void Root()
        {
            TestStringVariableOperator.TestGetPropsFromPropString();

            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            TestStringVariableOperator.TestBuiltStringIntoEnvironmentVariable();
        }

        #endregion

        #region private

        /// <summary>プロパティ文字列の解析</summary>
        /// <remarks>
        /// 書式は [property=value[;property=value]]。
        /// 値に「=」「;」が混じる場合は { } で囲ってエスケープする（{=} / {;}）。
        /// **{ } 自体はエスケープできない**（実装の制約）。
        /// </remarks>
        private static void TestGetPropsFromPropString()
        {
            MyDebug.OutputDebugAndConsole("StringVariableOperator.GetPropsFromPropString");

            // 基本
            TestStringVariableOperator.OutputProps("1 組", "a=1");
            TestStringVariableOperator.OutputProps("2 組", "a=1;b=2");

            // 値が空
            TestStringVariableOperator.OutputProps("値が空", "a=;b=2");

            // エスケープ（値に「=」「;」を含める）
            TestStringVariableOperator.OutputProps("値に = を含む", "a={=};b=2");
            TestStringVariableOperator.OutputProps("値に ; を含む", "a={;};b=2");

            // 名前側のエスケープ
            TestStringVariableOperator.OutputProps("名前に = を含む", "a{=}b=1");

            // 異常系（書式エラーは例外）
            TestStringVariableOperator.OutputProps("= が無い", "a");
            TestStringVariableOperator.OutputProps("空文字", "");
        }

        /// <summary>環境変数の埋め込み</summary>
        /// <remarks>
        /// 「%環境変数名%」を値に置き換える。
        /// **テスト専用の環境変数を自分で設定してから使う。**
        /// 既存の環境変数（TEMP など）に依存すると、実行環境で結果が変わる。
        /// </remarks>
        private static void TestBuiltStringIntoEnvironmentVariable()
        {
            MyDebug.OutputDebugAndConsole("StringVariableOperator.BuiltStringIntoEnvironmentVariable");

            const string name = "OPENTOURYO_TEST_VAR";
            string org = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

            try
            {
                Environment.SetEnvironmentVariable(name, "VALUE", EnvironmentVariableTarget.Process);

                TestStringVariableOperator.OutputEnvVar("そのまま", "abc");
                TestStringVariableOperator.OutputEnvVar("埋め込み", "x%" + name + "%y");
                TestStringVariableOperator.OutputEnvVar("2 回", "%" + name + "%-%" + name + "%");

                // 存在しない環境変数
                TestStringVariableOperator.OutputEnvVar("未定義", "x%OPENTOURYO_NOT_EXIST%y");

                // 「%」が奇数個（閉じていない）
                TestStringVariableOperator.OutputEnvVar("閉じていない", "x%" + name);

                // null と空文字
                TestStringVariableOperator.OutputEnvVar("null", null);
                TestStringVariableOperator.OutputEnvVar("空文字", "");
            }
            finally
            {
                // **必ず戻す。** 後続のテストや同一プロセス内の他の処理に影響するため。
                Environment.SetEnvironmentVariable(name, org, EnvironmentVariableTarget.Process);
            }
        }

        #endregion

        #region 出力のヘルパ

        /// <summary>プロパティ文字列の解析結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="propString">プロパティ文字列</param>
        /// <remarks>
        /// 例外はメッセージではなく**型名だけ**を出す。
        /// メッセージは実行環境の言語で変わり、期待結果と一致しなくなるため。
        /// </remarks>
        private static void OutputProps(string caseName, string propString)
        {
            MyDebug.OutputDebugAndConsole("[" + caseName + "] " + propString);

            try
            {
                Dictionary<string, string> dic =
                    StringVariableOperator.GetPropsFromPropString(propString);

                // 並び順が実装に依存しないよう、キーを並べ替えてから出す。
                List<string> keys = new List<string>(dic.Keys);
                keys.Sort(StringComparer.Ordinal);

                MyDebug.OutputDebugAndConsole("  件数 : " + dic.Count);

                foreach (string key in keys)
                {
                    MyDebug.OutputDebugAndConsole("  " + key + " = " + dic[key]);
                }
            }
            catch (Exception ex)
            {
                MyDebug.OutputDebugAndConsole("  例外 : " + ex.GetType().FullName);
            }
        }

        /// <summary>環境変数の埋め込み結果を出力する</summary>
        /// <param name="caseName">ケース名</param>
        /// <param name="builtString">環境変数名入り文字列</param>
        private static void OutputEnvVar(string caseName, string builtString)
        {
            try
            {
                string result = StringVariableOperator.BuiltStringIntoEnvironmentVariable(builtString);

                MyDebug.OutputDebugAndConsole(
                    "[" + caseName + "] "
                    + (builtString == null ? "(null)" : builtString)
                    + " → "
                    + (result == null ? "(null)" : result));
            }
            catch (Exception ex)
            {
                MyDebug.OutputDebugAndConsole("[" + caseName + "] 例外 : " + ex.GetType().FullName);
            }
        }

        #endregion
    }
}

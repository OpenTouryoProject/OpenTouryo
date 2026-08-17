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
//* クラス名        ：Program
//* クラス日本語名  ：単体テストのエントリ ポイント
//*
//* 作成者          ：西野 大介
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2019/02/06  西野 大介         新規作成
//*  2026/08/18  玄人 幸道         各テストを個別に try で囲むようにした（#564）。
//*                                1 つが例外を投げると、以降が実行されなかった。
//**********************************************************************************

using System;
using System.Text;
using System.Configuration;

using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Diagnostics;

namespace TestCode
{
    /// <summary>Program</summary>
    /// <remarks>
    /// 何をテストしているか、ケースを書き足すときの決まりは README.md を参照。
    /// **結果ファイルとの比較で判定するため、環境で変わる値を出してはならない。**
    /// </remarks>
    public class Program
    {
        /// <summary>Main</summary>
        /// <param name="args">string[]</param>
        public static void Main(string[] args)
        {
            // configの初期化(無くても動くようにせねば。)
#if NETCOREAPP
            GetConfigParameter.InitConfiguration("appsettings.json");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            try
            {
                #region Public
                #region Basic
                Program.Run("TestOutputLog", TestOutputLog.Root);

                Program.Run("TestGetMessageAndProperty", TestGetMessageAndProperty.Root);

                Program.Run("TestStringChecker", TestStringChecker.Root);

                Program.Run("TestFormatChecker", TestFormatChecker.Root);

                Program.Run("TestStringVariableOperator", TestStringVariableOperator.Root);

                Program.Run("TestStringExtractor", TestStringExtractor.Root);

                Program.Run("TestUtil", TestUtil.Root);

                Program.Run("TestStringConverter", TestStringConverter.Root);

                Program.Run("TestFormatConverter", TestFormatConverter.Root);

                Program.Run("TestCustomEncode", TestCustomEncode.Root);

                Program.Run("JISCode", JISCode.Root);
                #endregion
                #region Extension
                Program.Run("TestEnumToStringExtensions", TestEnumToStringExtensions.Root);

                Program.Run("TestXmlLib", TestXmlLib.Root);
                
                Program.Run("TestDeflateCompression", TestDeflateCompression.Root);

                Program.Run("TestResourceLoader", TestResourceLoader.Root);

                Program.Run("TestZipV2", TestZipV2.Root);
                #endregion
                #region Dto
                Program.Run("TestDto", TestDto.Root);
                #endregion
                #region Diagnostics
                Program.Run("TestObjectInspector", TestObjectInspector.Root);
                #endregion
                #region Reflection
                Program.Run("TestLatebind", TestLatebind.Root);

                Program.Run("TestFastReflection", TestFastReflection.Root);
                #endregion
                // Db は TestDataAccess へ移した（#520）。
                // DB に接続するテストと前提が異なるため、プロジェクトを分けている。
                #endregion

                #region Business
                // Touryo.Infrastructure.Business
                // GMTMaster
                // JISX0208_1983Checker
                #endregion

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

                // echoすると例外
                try
                {
                    Console.ReadKey();
                }
                catch { }
            }
            catch (Exception ex)
            {
                MyDebug.OutputDebugAndConsole(ex.ToString());
            }
        }

        /// <summary>1 つのテストを実行する</summary>
        /// <param name="name">テスト名</param>
        /// <param name="test">テストの入口（Root）</param>
        /// <remarks>
        /// **1 つ壊れても、残りは走らせる。**（#564）
        ///
        /// 以前は全テストを 1 つの try で囲んでいたため、
        /// **どれか 1 つが例外を投げると、以降が丸ごと実行されなかった。**
        /// その分の網羅がまとめて失われるうえ、結果ファイルの欠け方から
        /// 「どこで止まったか」を推測するしかなかった。
        ///
        /// ＜スタック トレースを出さない＞
        ///
        ///   結果は Result*.txt と突き合わせるため、**環境で変わる値を出せない。**
        ///   スタック トレースには**ファイル パスと行番号**が入る。
        ///   型名とメッセージだけなら、どのテストで何が起きたかは分かる。
        ///
        ///   詳しく見たいときは、デバッガか、このメソッドを一時的に外して実行する。
        /// </remarks>
        private static void Run(string name, Action test)
        {
            MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");

            try
            {
                test();
            }
            catch (Exception ex)
            {
                // **型名とメッセージだけ。** スタック トレースは環境依存になる。
                MyDebug.OutputDebugAndConsole(
                    "[!] " + name + " で例外 : " + ex.GetType().FullName + " : " + ex.Message);
            }
        }
    }
}

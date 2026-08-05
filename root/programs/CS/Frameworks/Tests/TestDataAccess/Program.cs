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
//* クラス日本語名  ：データ アクセスのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/05  玄人 幸道         新規作成（#520）
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.Text;

using Touryo.Infrastructure.Public.Diagnostics;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace TestDataAccess
{
    /// <summary>Program</summary>
    /// <remarks>
    /// データ アクセスのパターンを、複数の DBMS に対して確認する（#520）。
    ///
    /// ＜なぜ TestCode と分けるか＞
    ///   TestCode は DB に接続しない。こちらは接続するため、前提が大きく異なる。
    ///   混ぜると「DB が無い環境では TestCode ごと動かない」ことになる。
    ///
    /// ＜実行モード＞
    ///   クロス DB は GitHub Actions では実行できない（Docker の各 DB が Linux
    ///   コンテナで、windows-latest は Linux コンテナを動かせないため）。
    ///   このため、対象を切り替えられるようにしてある。
    /// </remarks>
    public class Program
    {
        #region 定数

        /// <summary>SQL Server だけを対象にする（既定）</summary>
        /// <remarks>CI（GitHub Actions）はこちら。</remarks>
        public const string ModeSqlOnly = "SQLONLY";

        /// <summary>ローカルで起動している DBMS をすべて対象にする</summary>
        /// <remarks>LocalServicesOnDocker の各コンテナを起動しておくこと。</remarks>
        public const string ModeLocal = "LOCAL";

        #endregion

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
                string mode = Program.GetMode();
                string[] daps = Program.GetDataProviders(mode);

                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                MyDebug.OutputDebugAndConsole("実行モード     : " + mode);
                MyDebug.OutputDebugAndConsole("対象データプロバイダ : " + string.Join(", ", daps));

                #region DBに接続しないテスト
                // 実行モードによらず常に行う。
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestSQLUtility.Root();
                #endregion

                #region DBに接続するテスト
                TestDataAccessPattern.Root(daps);
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

        #region private

        /// <summary>コマンドラインから実行モードを取得する</summary>
        /// <returns>ModeSqlOnly または ModeLocal</returns>
        /// <remarks>
        /// 指定が無い場合は ModeSqlOnly。
        /// 結果ファイル（Result*.txt）は、この既定で実行したものを期待値としている。
        /// </remarks>
        private static string GetMode()
        {
            List<string> valsLst = null;
            Dictionary<string, string> argsDic = null;

            // コマンドラインをバラす関数がある。
            StringVariableOperator.GetCommandArgs('/', out argsDic, out valsLst);

            if (argsDic != null && argsDic.ContainsKey("/MODE"))
            {
                string mode = argsDic["/MODE"].ToUpper();

                if (mode == Program.ModeLocal)
                {
                    return Program.ModeLocal;
                }
            }

            return Program.ModeSqlOnly;
        }

        /// <summary>実行モードから、対象のデータ プロバイダを決める</summary>
        /// <param name="mode">実行モード</param>
        /// <returns>データ プロバイダの識別子（MyBaseLogic の ActionType 先頭と同じ）</returns>
        /// <remarks>
        /// net48 と .NET (Core) で使えるものが異なる。
        /// PostgreSQL（NPS）は DamPstGrS が .NET (Core) 専用のため、net48 では対象外。
        /// </remarks>
        private static string[] GetDataProviders(string mode)
        {
            if (mode == Program.ModeLocal)
            {
#if NET48
                return new string[] { "SQL", "ODP", "MCN" };
#else
                return new string[] { "SQL", "ODP", "MCN", "NPS" };
#endif
            }

            return new string[] { "SQL" };
        }

        #endregion
    }
}

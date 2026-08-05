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
//* クラス名        ：TestDataAccessPattern
//* クラス日本語名  ：データ アクセスのパターンのテスト
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/05  玄人 幸道         新規作成（#520）
//**********************************************************************************

using System;
using System.Data;
using System.IO;

using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Diagnostics;
using Touryo.Infrastructure.Public.Util;

namespace TestDataAccess
{
    /// <summary>データ アクセスのパターンのテスト</summary>
    /// <remarks>
    /// Dam の実行系メソッドを、同じ問い合わせに対して一通り呼ぶ（#520）。
    ///
    /// ＜Dam を直接使う理由＞
    ///   Ｂ層・Ｄ層を経由すると、引数クラス・戻り値クラス・LayerB / LayerD の一式が要る。
    ///   ここで見たいのは「データ プロバイダごとの実行系の挙動」なので、Dam を直接使う。
    ///   Ｂ層・Ｄ層を通した確認は TestBatch（SimpleBatch）が担う。
    ///
    /// ＜結果の決定性＞
    ///   Shippers は 3 件（初期状態）。件数だけを出力するため、DBMS が違っても同じ結果になる。
    ///   例外は型名だけを出力する。メッセージは OS の表示言語で変わり、
    ///   CI（英語）と開発環境（日本語）で差分になるため。
    /// </remarks>
    public class TestDataAccessPattern
    {
        #region public

        /// <summary>Root</summary>
        /// <param name="daps">対象のデータ プロバイダ</param>
        public static void Root(string[] daps)
        {
            foreach (string dap in daps)
            {
                MyDebug.OutputDebugAndConsole("----------------------------------------------------------------------------------------------------");
                TestDataAccessPattern.TestOneProvider(dap);
            }
        }

        #endregion

        #region private

        /// <summary>データ プロバイダ 1 つ分のテスト</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        private static void TestOneProvider(string dap)
        {
            MyDebug.OutputDebugAndConsole("[" + dap + "]");

            BaseDam dam = TestDataAccessPattern.CreateDam(dap);

            if (dam == null)
            {
                // このターゲット フレームワークでは使えないデータ プロバイダ。
                MyDebug.OutputDebugAndConsole("- 未対応のデータ プロバイダのため、実行しない。");
                return;
            }

            try
            {
                dam.ConnectionOpen(GetConfigParameter.GetConnectionString("ConnectionString_" + dap));

                try
                {
                    TestDataAccessPattern.TestExecPatterns(dam);
                    TestDataAccessPattern.TestStaticSql(dam, dap);
                }
                finally
                {
                    dam.ConnectionClose();
                }
            }
            catch (Exception ex)
            {
                // 接続できない場合もここに来る（LOCAL モードでコンテナが起動していない等）。
                // メッセージは環境で変わるため、型名だけを出す。
                MyDebug.OutputDebugAndConsole("- 例外 : " + ex.GetType().FullName);
            }
        }

        /// <summary>実行系メソッドを一通り呼ぶ</summary>
        /// <param name="dam">BaseDam</param>
        private static void TestExecPatterns(BaseDam dam)
        {
            // 件数を返す（スカラ）
            dam.SetSqlByCommand("SELECT COUNT(*) FROM Shippers");
            MyDebug.OutputDebugAndConsole("- ExecSelectScalar        : " + Convert.ToInt32(dam.ExecSelectScalar()));

            // データテーブルに受ける
            DataTable dt = new DataTable();
            dam.SetSqlByCommand("SELECT * FROM Shippers");
            dam.ExecSelectFill_DT(dt);
            MyDebug.OutputDebugAndConsole("- ExecSelectFill_DT       : " + dt.Rows.Count);

            // データセットに受ける
            DataSet ds = new DataSet();
            dam.SetSqlByCommand("SELECT * FROM Shippers");
            dam.ExecSelectFill_DS(ds);
            MyDebug.OutputDebugAndConsole("- ExecSelectFill_DS       : " + ds.Tables[0].Rows.Count);

            // データリーダで受ける
            dam.SetSqlByCommand("SELECT * FROM Shippers");
            int count = 0;
            IDataReader idr = dam.ExecSelect_DR();
            try
            {
                while (idr.Read())
                {
                    count++;
                }
            }
            finally
            {
                idr.Close();
            }
            MyDebug.OutputDebugAndConsole("- ExecSelect_DR           : " + count);
        }

        /// <summary>静的SQL（ファイル）を読んで実行する</summary>
        /// <param name="dam">BaseDam</param>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <remarks>
        /// SQL は DBMS ごとのフォルダに分かれている。
        /// BaseDam.SetSqlByFile はパスを連結しないため（連結するのは MyBaseDao.SetSqlByFile2）、
        /// ここで組み立てる。
        /// </remarks>
        private static void TestStaticSql(BaseDam dam, string dap)
        {
            string path = Path.Combine(
                GetConfigParameter.GetConfigValue("SqlTextFilePath"),
                TestDataAccessPattern.GetSqlFolder(dap),
                "ShipperCount.sql");

            if (!File.Exists(path))
            {
                MyDebug.OutputDebugAndConsole("- SetSqlByFile            : SQLファイルが無い。");
                return;
            }

            dam.SetSqlByFile(path);
            MyDebug.OutputDebugAndConsole("- SetSqlByFile ＋ Scalar  : " + Convert.ToInt32(dam.ExecSelectScalar()));
        }

        /// <summary>データ プロバイダに対応する Dam を生成する</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <returns>BaseDam（未対応の場合は null）</returns>
        /// <remarks>
        /// 対応関係は MyBaseLogic のデータ プロバイダ選択に合わせている。
        /// PostgreSQL（NPS）は DamPstGrS が .NET (Core) 専用のため、net48 では生成しない。
        /// </remarks>
        private static BaseDam CreateDam(string dap)
        {
            switch (dap)
            {
                case "SQL":
                    return new DamSqlSvr();

                case "ODP":
                    return new DamManagedOdp();

                case "MCN":
                    return new DamMySQL();

#if NET48
#else
                case "NPS":
                    return new DamPstGrS();
#endif

                default:
                    return null;
            }
        }

        /// <summary>データ プロバイダに対応する SQL の格納フォルダ</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <returns>root/files/resource/Sql からの相対フォルダ名</returns>
        private static string GetSqlFolder(string dap)
        {
            switch (dap)
            {
                case "SQL":
                    return "sqlserver";

                case "ODP":
                    return "oracle";

                case "MCN":
                    return "mysql";

                case "NPS":
                    return "pstgrs";

                default:
                    return "";
            }
        }

        #endregion
    }
}

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
//* クラス名        ：DataProvider
//* クラス日本語名  ：データ プロバイダごとの差異の吸収
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/08/06  玄人 幸道         新規作成（#520）
//**********************************************************************************

using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Util;

namespace TestDataAccess
{
    /// <summary>データ プロバイダごとの差異を吸収する</summary>
    /// <remarks>
    /// 識別子（dap）は MyBaseLogic の ActionType 先頭と同じ文字列を使う。
    ///   SQL … SQL Server / ODP … Oracle / MCN … MySQL / NPS … PostgreSQL
    /// テスト クラスから共通に使うため、ここに集約する。
    /// </remarks>
    public class DataProvider
    {
        /// <summary>データ プロバイダに対応する Dam を生成する</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <returns>BaseDam（未対応の場合は null）</returns>
        /// <remarks>
        /// 対応関係は MyBaseLogic のデータ プロバイダ選択に合わせている。
        /// PostgreSQL（NPS）は DamPstGrS が .NET (Core) 専用のため、net48 では生成しない。
        /// </remarks>
        public static BaseDam CreateDam(string dap)
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

        /// <summary>データ プロバイダに対応する接続文字列</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <returns>接続文字列</returns>
        public static string GetConnectionString(string dap)
        {
            return GetConfigParameter.GetConnectionString("ConnectionString_" + dap);
        }

        /// <summary>データ プロバイダに対応する DBMS の種類</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <returns>DBMSType（SQLUtility に渡す）</returns>
        public static DbEnum.DBMSType GetDbmsType(string dap)
        {
            switch (dap)
            {
                case "ODP":
                    return DbEnum.DBMSType.Oracle;

                case "MCN":
                    return DbEnum.DBMSType.MySQL;

                case "NPS":
                    return DbEnum.DBMSType.PstGrS;

                default:
                    return DbEnum.DBMSType.SQLServer;
            }
        }

        /// <summary>識別子を囲う</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <param name="name">識別子</param>
        /// <returns>囲った識別子</returns>
        /// <remarks>
        /// **SQLUtility が生成する囲い文字と必ず一致させること。**
        /// Oracle は囲わないと大文字、PostgreSQL は囲わないと小文字に畳まれるため、
        /// 表を作るときも同じ囲い方をしないと、生成された SQL から列が見えなくなる。
        /// </remarks>
        public static string Quote(string dap, string name)
        {
            switch (dap)
            {
                case "SQL":
                    return "[" + name + "]";

                case "MCN":
                    return "`" + name + "`";

                default:
                    // Oracle・PostgreSQL は二重引用符
                    return "\"" + name + "\"";
            }
        }

        /// <summary>データ プロバイダに対応する SQL の格納フォルダ</summary>
        /// <param name="dap">データ プロバイダの識別子</param>
        /// <returns>root/files/resource/Sql からの相対フォルダ名</returns>
        public static string GetSqlFolder(string dap)
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
    }
}

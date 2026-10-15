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
//* クラス名        ：DbProvider
//* クラス日本語名  ：データ プロバイダごとの差異の吸収
//*
//* 作成者          ：玄人 幸道
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/10/15  玄人 幸道         新規作成（#610）
//**********************************************************************************

using System;

using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.Util;

namespace TestPublicDb
{
    /// <summary>データ プロバイダごとの差異の吸収</summary>
    /// <remarks>
    /// 識別子（dap）は Golden の TestDataAccess（DataProvider.cs）と同じ。
    ///   SQL … SQL Server / ODP … Oracle / MCN … MySQL / NPS … PostgreSQL（.NET のみ）
    /// </remarks>
    public static class DbProvider
    {
        /// <summary>対象のデータ プロバイダ（TestFixtureSource に渡す）</summary>
        public static readonly string[] All =
#if NETCOREAPP
            new[] { "SQL", "ODP", "MCN", "NPS" };
#else
            // DamPstGrS は .NET 専用
            new[] { "SQL", "ODP", "MCN" };
#endif

        /// <summary>**接続できないと失敗にする**データ プロバイダ（CI でも接続できるもの）</summary>
        public const string Required = "SQL";

        /// <summary>Dam を生成する。</summary>
        public static BaseDam CreateDam(string dap)
        {
            switch (dap)
            {
                case "SQL": return new DamSqlSvr();
                case "ODP": return new DamManagedOdp();
                case "MCN": return new DamMySQL();
#if NETCOREAPP
                case "NPS": return new DamPstGrS();
#endif
                default: throw new ArgumentOutOfRangeException(nameof(dap), dap, "Unknown data provider.");
            }
        }

        /// <summary>接続文字列（ConnectionString_＜dap＞）</summary>
        public static string GetConnectionString(string dap)
        {
            return GetConfigParameter.GetConnectionString("ConnectionString_" + dap);
        }

        /// <summary>DBMS の種類（SQLUtility に渡す）</summary>
        public static DbEnum.DBMSType GetDbmsType(string dap)
        {
            switch (dap)
            {
                case "ODP": return DbEnum.DBMSType.Oracle;
                case "MCN": return DbEnum.DBMSType.MySQL;
                case "NPS": return DbEnum.DBMSType.PstGrS;
                default: return DbEnum.DBMSType.SQLServer;
            }
        }

        /// <summary>
        /// 識別子を囲う。**SQLUtility が生成する囲い文字と一致させる。**
        /// Oracle は囲わないと大文字、PostgreSQL は小文字に畳まれる。
        /// </summary>
        public static string Quote(string dap, string name)
        {
            switch (dap)
            {
                case "SQL": return "[" + name + "]";
                case "MCN": return "`" + name + "`";
                default: return "\"" + name + "\"";
            }
        }

        /// <summary>パラメタの先頭記号（Oracle だけ :）</summary>
        public static string Sign(string dap)
        {
            return (dap == "ODP") ? ":" : "@";
        }

        /// <summary>整数型の型名</summary>
        public static string IntType(string dap)
        {
            switch (dap)
            {
                case "ODP": return "NUMBER(10)";
                case "MCN": return "INT";
                case "NPS": return "integer";
                default: return "int";
            }
        }

        /// <summary>文字列型の型名</summary>
        public static string StrType(string dap)
        {
            switch (dap)
            {
                case "ODP": return "VARCHAR2(20)";
                case "MCN": return "VARCHAR(20)";
                case "NPS": return "varchar(20)";
                default: return "nvarchar(20)";
            }
        }
    }
}

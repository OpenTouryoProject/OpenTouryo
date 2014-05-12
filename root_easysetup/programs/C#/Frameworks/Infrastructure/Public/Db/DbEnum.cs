//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
//**********************************************************************************

#region Apache License
//
//  
// 
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
//* クラス名        ：DbEnum
//* クラス日本語名  ：Public.Db名前空間で使用する列挙型クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2008/03/13  西野  大介        NotConnectを追加
//*  2013/01/13  西野  大介        DBMSType、DataProviderTypeを追加
//**********************************************************************************

// System
using System;

namespace Touryo.Infrastructure.Public.Db
{
    /// <summary>Public.Db名前空間で使用する列挙型クラス</summary>
    /// <remarks>特定の箇所で利用できる。</remarks>
    public class DbEnum
    {
        /// <summary>DBMS指定の列挙型</summary>
        /// <remarks>自由に利用できる。</remarks>
        public enum DBMSType : int
        {
            /// <summary>SQLServer</summary>
            SQLServer,

            /// <summary>Oracle</summary>
            Oracle,

            /// <summary>DB2</summary>
            DB2,

            /// <summary>HiRDB</summary>
            HiRDB,

            /// <summary>MySQL</summary>
            MySQL,

            /// <summary>PostgreSQL</summary>
            PstGrS
        }

        /// <summary>DataProvider指定の列挙型</summary>
        /// <remarks>自由に利用できる。</remarks>
        public enum DataProviderType : int
        {
            /// <summary>SQLServer SQLClient</summary>
            SQLClient,

            /// <summary>ODBC</summary>
            ODBC,

            /// <summary>OLEDB</summary>
            OLEDB,

            /// <summary>Oracle ODP.NET</summary>
            OraODP,

            /// <summary>DB2.NET</summary>
            DB2,

            /// <summary>HiRDB.NET</summary>
            HiRDB,

            /// <summary>MySQL Connector/NET</summary>
            MySQLCN,

            /// <summary>PostgreSQL Npgsql</summary>
            Npgsql
        }

        /// <summary>分離レベル指定のための列挙型</summary>
        /// <remarks>自由に利用できる。</remarks>
        public enum IsolationLevelEnum : int
        {
            /// <summary>
            /// NotConnect（コネクションしない）
            /// </summary>
            NotConnect,

            /// <summary>
            /// NoTransaction（トランザクションを開始しない）
            /// </summary>
            NoTransaction,

            /// <summary>
            /// DefaultTransaction（規定の分離レベルでトランザクションを開始）
            /// </summary>
            DefaultTransaction,

            /// <summary>
            /// ReadUncommitted（非コミット読み取りの分離レベルでトランザクションを開始）
            /// </summary>
            ReadUncommitted,

            /// <summary>
            /// ReadCommitted（コミット済み読み取りの分離レベルでトランザクションを開始）
            /// </summary>
            ReadCommitted,

            /// <summary>
            /// RepeatableRead（反復可能読み取りの分離レベルでトランザクションを開始）
            /// </summary>
            RepeatableRead,

            /// <summary>
            /// Serializable（直列化可能の分離レベルでトランザクションを開始）
            /// </summary>
            Serializable,

            /// <summary>
            /// Snapshot（スナップショット分離レベルでトランザクションを開始）
            /// </summary>
            Snapshot,

            /// <summary>
            /// ユーザが、独自のロジックを使用して指定する
            /// </summary>
            User
        }

        /// <summary>設定されているクエリーの状態を示す列挙型</summary>
        /// <remarks>DPQuery_Toolから利用するので、public</remarks>
        public enum QueryStatusEnum : int
        {
            /// <summary>設定されていない状態</summary>
            IsNotSet,

            /// <summary>SPQ（静的パラメタライズド・クエリ）</summary>
            SPQ,

            /// <summary>DPQ（動的パラメタライズド・クエリ）</summary>
            DPQ
        }
    }
}

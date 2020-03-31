//**********************************************************************************
//* Copyright (C) 2007,2016 Hitachi Solutions,Ltd.
//**********************************************************************************

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
//* クラス名        ：Literal
//* クラス日本語名  ：リテラル クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/04/26  西野 大介         新規作成
//*  2009/06/02  西野 大介         sln - IR版からの修正
//*                                ・#x  ： デバッグ用に機能追加。
//*  2010/02/18  西野 大介         データプロバイダ追加（HiRDB、PostgreSQL）
//*  2012/02/09  西野 大介         HiRDBデータプロバイダのコメントアウト（（ソフト）対応せず）
//*  2012/02/09  西野 大介         OLEDB、ODBCのデータプロバイダ対応
//*  2014/02/05  西野 大介         System.Data.OracleClientデータプロバイダ対応
//*  2017/11/29  西野 大介         Resource化したメッセージを削除
//*  2018/10/29  西野 大介         NETCOREAPP対応で、サポートされないDBを「#if」した。
//**********************************************************************************

namespace DPQuery_Tool
{
    /// <summary>リテラル クラス</summary>
    public class Literal
    {
        #region 設定

        /// <summary>設定：コンフィグ ファイルのフッタ</summary>
        public const string CONFIG_FILE_FOOTER = "_config.ini";

        /// <summary>設定：スロット</summary>
        public readonly static string[] SLOT
            = new string[] { "Slot1", "Slot2", "Slot3", "Slot4", "Slot5", "Slot6", "Slot7", "Slot8", "Slot9", "Slot10" };
        
        /// <summary>設定：データプロバイダ</summary>
        public const string CONFIGURATION_DAP = "dap";

        /// <summary>設定：IPアドレス（ホスト名）</summary>
        public const string CONFIGURATION_IP = "ip";

        /// <summary>設定：ユーザID</summary>
        public const string CONFIGURATION_UID = "uid";

        /// <summary>設定：パスワード</summary>
        public const string CONFIGURATION_PWD = "pwd";

        /// <summary>設定：接続文字列</summary>
        public const string CONFIGURATION_CSR = "csr";

        #endregion

        #region 選択

        #region データプロバイダ

        /// <summary>選択：SQL Server用 sqlClient</summary>
        public const string DAP_SQL = "SQL Server - sqlClient";

        /// <summary>選択：ODBC.NET</summary>
        public const string DAP_ODB = "ODBC.NET";

        /// <summary>選択：Oracle用 Oracle Client</summary>
        public const string DAP_ORA = "Oracle - Oracle Client";

        /// <summary>選択：Oracle用 ODP.NET</summary>
        public const string DAP_ODP = "Oracle - ODP.NET";

        /// <summary>選択：MySQL用 MySQL Connector/NET</summary>
        public const string DAP_MySQL = "MySQL - Connector/NET";

        /// <summary>選択：PostgreSQL用 Npgsql </summary>
        public const string DAP_PstgrS = "PostgreSQL - Npgsql";

#if NETCOREAPP
#else
        /// <summary>選択：OLEDB.NET</summary>
        public const string DAP_OLE = "OLEDB.NET";

        /// <summary>選択：DB2用 DB2.NET</summary>
        public const string DAP_DB2 = "DB2 - DB2.NET";

        ///// <summary>選択：HiRDB用 データ プロバイダ</summary>
        //public const string DAP_HiRDB = "HiRDB - HiRDBデータ プロバイダ";
#endif

        #endregion

        #region 実行メソッド

        /// <summary>選択：Fill（DataTable）メソッド</summary>
        public const string METHOD_DATA_TABLE = "DataTable";

        /// <summary>選択：Fill（DataSet）メソッド</summary>
        public const string METHOD_DATA_SET = "DataSet";

        /// <summary>選択：DataReaderメソッド</summary>
        public const string METHOD_DATA_READER = "DataReader";

        /// <summary>選択：Scalarメソッド</summary>
        public const string METHOD_SCALAR = "Scalar";

        /// <summary>選択：NonQueryメソッド</summary>
        public const string METHOD_NON_QUERY = "NonQuery";

        #endregion

        #region 分離レベル

        /// <summary>選択：ノー トランザクション</summary>
        public const string ISO_LEVEL_NO_TRANSACTION = "No Transaction";

        /// <summary>選択：デフォルト トランザクション</summary>
        public const string ISO_LEVEL_DEFAULT_TRANSACTION = "Default Transaction";

        /// <summary>選択：ダーティー リード</summary>
        public const string ISO_LEVEL_READ_UNCOMMITTED = "Read Uncommitted";

        /// <summary>選択：リードコミット</summary>
        public const string ISO_LEVEL_READ_COMMITTED = "Read Committed";

        /// <summary>選択：リピータブル リード</summary>
        public const string ISO_LEVEL_REPEATABLE_READ = "Repeatable Read";

        /// <summary>選択：シリアライザブル</summary>
        public const string ISO_LEVEL_SERIALIZABLE = "Serializable";

        /// <summary>選択：スナップショット</summary>
        public const string ISO_LEVEL_SNAPSHOT = "Snapshot";

        /// <summary>選択：スナップショット</summary>
        public const string ISO_LEVEL_USER = "User(debug)";

        /// <summary>選択：スナップショット</summary>
        public const string ISO_LEVEL_NOT_CONNECT = "Not Connect(debug)";

        #endregion

        #region トランザクション モード

        /// <summary>選択：自動開始～自動コミット</summary>
        public const string COMMIT_MODE_AUTO = "Auto";

        /// <summary>選択：手動開始～手動コミット</summary>
        public const string COMMIT_MODE_MANUAL = "Manual";
        
        #endregion

        #endregion
    }
}

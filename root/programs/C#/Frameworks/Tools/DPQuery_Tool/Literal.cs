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

        ///// <summary>設定：スロット１</summary>
        //public const string SLOT1 = "Slot1";
        ///// <summary>設定：スロット２</summary>
        //public const string SLOT2 = "Slot2";
        ///// <summary>設定：スロット３</summary>
        //public const string SLOT3 = "Slot3";
        ///// <summary>設定：スロット４</summary>
        //public const string SLOT4 = "Slot4";
        ///// <summary>設定：スロット５</summary>
        //public const string SLOT5 = "Slot5";

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

        /// <summary>選択：OLEDB.NET</summary>
        public const string DAP_OLE = "OLEDB.NET";
        
        /// <summary>選択：ODBC.NET</summary>
        public const string DAP_ODB = "ODBC.NET";

        /// <summary>選択：Oracle用 Oracle Client</summary>
        public const string DAP_ORA = "Oracle - Oracle Client";

        /// <summary>選択：Oracle用 ODP.NET</summary>
        public const string DAP_ODP = "Oracle - ODP.NET";

        /// <summary>選択：DB2用 DB2.NET</summary>
        public const string DAP_DB2 = "DB2 - DB2.NET";

        ///// <summary>選択：HiRDB用 データ プロバイダ</summary>
        //public const string DAP_HiRDB = "HiRDB - HiRDBデータ プロバイダ";

        /// <summary>選択：MySQL用 MySQL Connector/NET</summary>
        public const string DAP_MySQL = "MySQL - Connector/NET";

        /// <summary>選択：PostgreSQL用 Npgsql </summary>
        public const string DAP_PstgrS = "PostgreSQL - Npgsql";

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

        #region 状態

        ///// <summary>状態：初期状態</summary>
        //public const string STATUS_INIT = "初期状態";

        ///// <summary>状態：処理をキャンセル</summary>
        //public const string STATUS_PROC_CANCELED = "処理をキャンセルしました。";

        #region DAP生成

        ///// <summary>状態：sqlClientを生成</summary>
        //public const string STATUS_SQL_CREATED = "sqlClientを生成しました。";

        ///// <summary>状態：OLEDB.NETを生成</summary>
        //public const string STATUS_OLE_CREATED = "OLEDB.NETを生成しました。";

        ///// <summary>状態：ODBC.NETを生成</summary>
        //public const string STATUS_ODB_CREATED = "ODBC.NETを生成しました。";
        
        ///// <summary>状態：ODP.NETを生成</summary>
        //public const string STATUS_ODP_CREATED = "ODP.NETを生成しました。";
        
        ///// <summary>状態：DB2.NETを生成</summary>
        //public const string STATUS_DB2_CREATED = "DB2.NETを生成しました。";

        ///// <summary>状態：HiRDBデータ プロバイダを生成</summary>
        //public const string STATUS_HRD_CREATED = "HiRDBデータ プロバイダを生成しました。";

        ///// <summary>状態：MySQL Connector/NETを生成</summary>
        //public const string STATUS_MSL_CREATED = "MySQL Connector/NETを生成しました。";

        ///// <summary>状態：PostgreSQL Npgsqlを生成</summary>
        // public const string STATUS_PGS_CREATED = "PostgreSQL Npgsqlを生成しました。";

        #endregion

        #region 設定

        ///// <summary>状態：新規設定を作成</summary>
        //public const string STATUS_CREATE_CONFIGURATION = "新規設定を作成しました。";

        ///// <summary>状態：設定をセーブ</summary>
        //public const string STATUS_CONFIGURATION_SAVED = "設定をセーブしました。";

        ///// <summary>状態：設定をロード</summary>
        //public const string STATUS_CONFIGURATION_LOADED = "設定をロードしました。";

        #endregion

        #region クエリ ファイル

        ///// <summary>状態：クエリファイルを開いた</summary>
        //public const string STATUS_QUERY_FILE_OPENED = "クエリファイルを開きました。";

        ///// <summary>状態：クエリファイルに上書き保存</summary>
        //public const string STATUS_QUERY_FILE_OVERWRITED = "テキストをクエリファイルに上書き保存しました。";

        ///// <summary>状態：クエリファイルを閉じた</summary>
        //public const string STATUS_QUERY_FILE_CLOSED = "クエリファイルを閉じました。";

        ///// <summary>状態：クエリファイルに保存</summary>
        //public const string STATUS_QUERY_FILE_SAVED = "テキストをクエリファイルに保存しました。";

        #endregion

        #region 接続
        
        ///// <summary>状態：接続をオープン</summary>
        //public const string STATUS_CONNECTION_OPENED = "接続をオープンしました。";

        ///// <summary>状態：接続をクローズ</summary>
        //public const string STATUS_CONNECTION_CLOSED = "接続をクローズしました。";

        #endregion
        
        #region トランザクション

        ///// <summary>状態："トランザクション自動制御</summary>
        //public const string STATUS_AUTO_MODE_WAS_SELECTED = "自動制御（自動コミット トランザクション モードで実行します）";

        ///// <summary>状態："トランザクション手動制御</summary>
        // public const string STATUS_MANUAL_MODE_WAS_SELECTED = "手動制御（ボタンを使用してトランザクションを手動で制御します）";

        ///// <summary>状態："トランザクションを開始</summary>
        //public const string STATUS_TRANSACTION_STARTED = "トランザクションを開始しました。";

        ///// <summary>状態：トランザクションをコミット</summary>
        //public const string STATUS_TRANSACTION_COMMITED = "トランザクションをコミットしました。";

        ///// <summary>状態：トランザクションをロールバック</summary>
        //public const string STATUS_TRANSACTION_ROLLBACKED = "トランザクションをロールバックしました。";

        #endregion

        ///// <summary>状態：クエリを実行</summary>
        //public const string STATUS_QUERY_EXECED = "クエリを実行しました。";

        #endregion

        #region メッセージ

        ///// <summary>メッセージ：DBサーバのipアドレス入力</summary>
        // public const string MSG_INPUT_IP = "DBサーバのipアドレスを入力";

        ///// <summary>メッセージ：DBへのログオンアカウント：ユーザID入力</summary>
        //public const string MSG_INPUT_UID = "DBへのログオンアカウントを入力：ユーザID";

        ///// <summary>メッセージ：DBへのログオンアカウント：パスワード入力</summary>
        //public const string MSG_INPUT_PWD = "DBへのログオンアカウントを入力：パスワード";

        ///// <summary>メッセージ：設定のセーブ ファイルが存在しない。</summary>
        //public const string MSG_CONFIG_FILE_NOT_EXIST = "設定のセーブ ファイルが存在しません。";

        ///// <summary>メッセージ：閉じる際、クエリ ファイルの保存を確認。</summary>
        //public const string MSG_IS_QUERY_FILE_SAVED = "クエリ ファイルをセーブしますか？";

        ///// <summary>メッセージ：通常のパラメタライズド・クエリを実行</summary>
        //public const string MSG_EXEC_SPQ = "通常のパラメタライズド・クエリを実行します。";

        ///// <summary>メッセージ：メッセージ：動的パラメタライズド・クエリを実行</summary>
        //public const string MSG_EXEC_DQP = "動的パラメタライズド・クエリを実行します。";

        /// <summary>メッセージ：～DataReaderの場合～</summary>
        public const string MSG_FOR_DATA_READER = "DataReaderの場合は、処理毎にトランザクションをコミットする必要があります。コミットしますか？";

        /// <summary>メッセージ：トランザクションをコミット</summary>
        public const string MSG_TRANSACTION_COMMITED = "トランザクションをコミットしました。";

        /// <summary>メッセージ：トランザクションをロールバック</summary>
        public const string MSG_TRANSACTION_ROLLBACKED = "トランザクションをロールバックしました。";

        #endregion
    }
}

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
//* クラス名        ：PubLiteral
//* クラス日本語名  ：Public層のリテラルのクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/04/22  西野 大介         新規作成
//*  2009/06/02  西野 大介         sln - IR版からの修正
//*                                ・#x ： CommandTimeOutデフォルト値を設定
//*  2010/09/24  西野 大介         JOINタグのネスト条件緩和
//*  2010/09/24  西野 大介         SELECT-CASE-DEFAULTタグの追加
//*  2010/09/24  西野 大介         DB Lib 別プロジェクト化対応
//*  2011/02/08  西野 大介         COMMENTタグの追加（チェック緩和）
//*  2012/03/21  西野 大介         SQLの型指定（.net型）対応
//*  2012/09/10  西野 大介         CDATAタグの追加（チェック緩和）
//*  2013/12/23  西野 大介         アクセス修飾子をすべてpublicに変更した。
//*  2014/02/03  西野 大介         国際化対応のスイッチ（app.config）を追加した。
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>Public層のリテラルのクラス</summary>
    public class PubLiteral
    {
        #region app.configのキー

        /// <summary>SQLキャシュ機能のON / OFFを設定する。</summary>
        public const string SQL_CACHE_SWITCH = "FxSqlCacheSwitch";

        /// <summary>SQLトレース ログ出力のON / OFFを設定する。</summary>
        public const string SQL_TRACELOG = "FxSqlTraceLog";
        
        /// <summary>SQLのエンコーディングを設定する。</summary>
        public const string SQL_ENCODING = "FxSqlEncoding";

        /// <summary>SQLのCommandTimeout値を設定する。</summary>
        public const string SQL_COMMANDTIMEOUT = "FxSqlCommandTimeout"; // #x-この行

        /// <summary>SQLの型指定方法を設定する。</summary>
        public const string SQL_DOTNET_TYPEINFO = "FxSqlDotnetTypeInfo"; // #x-この行

        /// <summary>log4netのコンフィグファイルを設定する。</summary>
        public const string LOG4NET_CONF_FILE = "FxLog4NetConfFile";

        /// <summary>国際化対応のスイッチ（例外メッセージ）</summary>
        public const string EXCEPTIONMESSAGECULTUER = "FxExceptionMessageCulture";

        #endregion

        #region 設定のリテラル

        /// <summary>ON / OFFのON</summary>
        public const string ON = "ON";

        /// <summary>ON / OFFのOFF</summary>
        public const string OFF = "OFF";

        #endregion

        #region 値文字列

        /// <summary>Boolean：true</summary>
        public const string VALUE_STR_TRUE = "TRUE";

        /// <summary>Boolean：false</summary>
        public const string VALUE_STR_FALSE = "FALSE";

        /// <summary>null</summary>
        public const string VALUE_STR_NULL = "NULL";

        #endregion

        #region ＤＢ部品

        #region 動的パラメタライズドクエリ

        /// <summary>ROOTタグ</summary>
        public const string DPQ_TAG_ROOT = "ROOT";

        /// <summary>VALタグ</summary>
        public const string DPQ_TAG_VAL = "VAL";

        /// <summary>INSCOLタグ</summary>
        public const string DPQ_TAG_INSCOL = "INSCOL";

        /// <summary>IFタグ</summary>
        public const string DPQ_TAG_IF = "IF";

        /// <summary>ELSEタグ</summary>
        public const string DPQ_TAG_ELSE = "ELSE";

        /// <summary>SELECTタグ</summary>
        public const string DPQ_TAG_SELECT = "SELECT";

        /// <summary>CASEタグ</summary>
        public const string DPQ_TAG_CASE = "CASE";

        /// <summary>DEFAULTタグ</summary>
        public const string DPQ_TAG_DEFAULT = "DEFAULT";

        /// <summary>LISTタグ</summary>
        public const string DPQ_TAG_LIST = "LIST";

        /// <summary>JOINタグ</summary>
        public const string DPQ_TAG_JOIN = "JOIN";

        /// <summary>SUBタグ</summary>
        public const string DPQ_TAG_SUB = "SUB";

        /// <summary>WHEREタグ</summary>
        public const string DPQ_TAG_WHERE = "WHERE";
        
        /// <summary>DELCMAタグ</summary>
        public const string DPQ_TAG_DELCMA = "DELCMA";

        /// <summary>PARAMタグ</summary>
        public const string DPQ_TAG_PARAM = "PARAM";

        /// <summary>DIVタグ</summary>
        public const string DPQ_TAG_DIV = "DIV";

        /// <summary>#textタグ</summary>
        public const string DPQ_TAG_TEXT = "#text";

        /// <summary>#commentタグ</summary>
        public const string DPQ_TAG_COMMENT = "#comment";

        /// <summary>#cdata-sectionタグ</summary>
        public const string DPQ_TAG_CDATA = "#cdata-section";
        
        #endregion       

        #region 静的パラメタライズドクエリ

        /// <summary>静的の場合のPARAMタグ（開始）</summary>
        public const string SPQ_PARAM_TAG_START = "/*PARAM*";

        /// <summary>静的の場合のPARAMタグ（終了）</summary>
        public const string SPQ_PARAM_TAG_END = "*PARAM*/";

        #endregion

        #region ログ

        /// <summary>SQLTracelogのコマンドテキストのヘッダ</summary>
        public const string SQLTRACELOG_COMMAND_TEXT_HEADER = "[commandText]:";

        /// <summary>SQLTracelogのコマンドパラメタのヘッダ</summary>
        public const string SQLTRACELOG_COMMAND_PARAM_HEADER = "[commandParameter]:";

        #endregion

        #endregion
    }
}

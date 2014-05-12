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
//* クラス名        ：BaseDam
//* クラス日本語名  ：データアクセス制御クラスのベースクラス
//*                 　データアクセス・プロバイダ毎のデータアクセス制御クラス
//*                 　を作成する場合は、必ず本クラスを継承する。
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2008/10/16  西野  大介        各種、問題点の修正
//*  2008/12/25  西野  大介        新機能（更新系クエリ対応機能）の追加
//*  2008/12/30  西野  大介        上記に対応して、パラメタ終了文字に[,]を追加した。
//*  2009/01/06  西野  大介        VALの置換時の仕様を変更した。
//*  2009/01/07  西野  大介        InnerXML→InnerTextに変更した。
//*  2009/03/19  西野  大介        DRのインターフェイスをobject→IDataReaderへ変更。
//*  2009/04/26  西野  大介        配列バインド対応を兼ね、型指定を可能にした。
//*  2009/04/28  西野  大介        デフォルト値を設けた
//*  2009/06/02  西野  大介        sln - IR版からの修正
//*                                ・#x ： CommandTimeOutデフォルト値を設定
//*  2009/08/12  西野  大介        比較演算子の向きを「<」に統一した。
//*  2009/09/25  西野  大介        内部の性能測定ログ出力処理を挿入した。
//*                                ※ Debug.WriteLineは意外に時間がかかるので注意！
//*                                   特に、繰り返し呼ばれるメソッド内では出力しない。
//*  2010/02/18  西野  大介        HiRDB対応として、サイズ指定を可能に、また、
//*                                合わせてSize, Directionなどのプロパティもサポート
//*  2010/09/24  西野  大介        JOINタグのネスト条件緩和
//*  2010/09/24  西野  大介        SELECT-CASE-DEFAULTタグの追加
//*  2010/09/24  西野  大介        型チェック方式の見直し（ GetType() & typeof() ）
//*  2010/09/24  西野  大介        ジェネリック対応（Dictionary、List、Queue、Stack<T>）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2010/09/24  西野  大介        SQLトレース詳細化用フィールド追加（UserInfo）
//*  2010/11/02  西野  大介        GetParameterメソッドを追加（ｽﾄｱﾄﾞ ﾕｰｻﾞﾋﾞﾘﾃｨ向上）
//*  2010/11/22  西野  大介        XmDPQのXMLを外部公開（パラメタ設定の自動化対応）
//*  2011/02/08  西野  大介        COMMENTタグの追加（チェック緩和）
//*  2012/03/16  西野  大介        ClearTextの仕様変更（文字列中は、空白・タブを詰めない）。
//*  2012/06/14  西野  大介        ResourceLoaderに加え、EmbeddedResourceLoaderに対応
//*  2012/09/10  西野  大介        CDATAタグの追加（チェック緩和）
//*  2013/02/15  加藤  幸紀        GetParamByText(string,char,out int)作成（順番バインドのパラメタ置換処理方式の見直し）
//*  2013/03/18  西野  大介        DeleteFirstLogicalOperatoronWhereClause部分を関数化して外出し
//*  2013/07/07  西野  大介        ExecGenerateSQL（SQL生成）メソッド（実行しない）を追加
//**********************************************************************************

// デバッグ用
//#define PERFORMANCE_LOG_SWITCH
using System.Diagnostics;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;

// 業務フレームワーク（循環参照になるため、参照しない）
// フレームワーク（循環参照になるため、参照しない）

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Db
{
    /// <summary>データアクセス制御クラスのベースクラス</summary>
    /// <remarks>ベース型として利用する。</remarks>
    public abstract class BaseDam
    {
        #region クラス変数

        /// <summary>
        /// SQLをキャッシュする
        /// </summary>
        private static Dictionary<string, string> _sqlHt;

        /// <summary>
        /// 排他のためのクラス変数
        /// </summary>
        private static readonly object _lock = new object();

        #endregion
        
        #region インスタンス変数

        #region SQLトレース詳細化用フィールド追加（ここから）

        /// <summary>任意の領域</summary>
        /// <remarks>自由に利用できる（通常SQLトレース詳細化用フィールドで利用）</remarks>
        private object _obj = null;

        /// <summary>任意の領域</summary>
        /// <remarks>自由に利用できる（通常SQLトレース詳細化用フィールドで利用）</remarks>
        public object Obj
        {
            set
            {
                this._obj = value;
            }
            get
            {
                return this._obj;
            }
        }

        #endregion

        #region 動的SQL（XML）

        /// <summary>DPQをXMLドキュメントとして保持する。</summary>
        private XmlDocument _xml = null;

        /// <summary>DPQのXMLドキュメントを取得する。</summary>
        /// <remarks>自由に利用できる</remarks>
        public XmlDocument Xml
        {
            get { return _xml; }
        }

        /// <summary>SQLが動的パラメタライズド クエリであるかどうかのチェック結果</summary>
        /// <remarks>派生のDamXXXから利用する。</remarks>
        protected DbEnum.QueryStatusEnum _QueryStatus = DbEnum.QueryStatusEnum.IsNotSet;

        /// <summary>クラスの外から参照可能にする。</summary>
        /// <remarks>利用箇所：DPQuery_Toolから利用</remarks>
        public bool IsDPQ
        {
            get
            {
                if (this._QueryStatus == DbEnum.QueryStatusEnum.DPQ)
                {
                    // DPQ
                    return true;
                }
                else
                {
                    // SQL or IsNotSet
                    return false;
                }
            }
        }

        #endregion

        #region パラメタの構成値を保存する。

        /// <summary>
        /// SQLが動的パラメタライズド クエリである場合、
        /// 指定されたパラメータ（の値）を保持するディクショナリ
        /// </summary>
        /// <remarks>派生のDamXXXから利用する。</remarks>
        protected Dictionary<string, object> _parameter = null;

        /// <summary>
        /// SQLが動的パラメタライズド クエリである場合、
        /// 指定されたパラメータ（の型）を保持するディクショナリ
        /// </summary>
        /// <remarks>派生のDamXXXから利用する。</remarks>
        protected Dictionary<string, object> _parameterType = null;

        /// <summary>
        /// SQLが動的パラメタライズド クエリである場合、
        /// 指定されたパラメータ（のサイズ）を保持するディクショナリ
        /// </summary>
        /// <remarks>派生のDamXXXから利用する。</remarks>
        protected Dictionary<string, int> _parameterSize = null;

        /// <summary>
        /// SQLが動的パラメタライズド クエリである場合、
        /// 指定されたパラメータ（の方向）を保持するディクショナリ
        /// </summary>
        /// <remarks>派生のDamXXXから利用する。</remarks>
        protected Dictionary<string, ParameterDirection> _parameterDirection = null;

        /// <summary>
        /// SQLが動的パラメタライズド クエリである場合、
        /// 指定されたユーザ パラメータを保持するディクショナリ
        /// </summary>
        /// <remarks>派生のDamXXXから利用する。</remarks>
        protected Dictionary<string, string> _userParameter = null;

        #endregion

        #endregion

        #region メソッド

        #region データアクセス制御クラスのメイン機能

        #region 初期化メソッド

        /// <summary>
        /// 初期化メソッド（SQLを連続で実行する場合に必要）
        /// SetSqlByCommandメソッドで実行する。
        /// </summary>
        /// <remarks>派生のDamXXXから利用する。</remarks>
        protected void init()
        {
            // メンバ変数を初期化する。

            // XML
            this._xml = null;

            // パラメタ関係
            this._parameter = new Dictionary<string,object>();
            this._parameterType = new Dictionary<string, object>();
            this._parameterSize = new Dictionary<string, int>();
            this._parameterDirection = new Dictionary<string, ParameterDirection>();

            this._userParameter = new Dictionary<string, string>();

            // 静的パラメタライズドクエリ化
            this._QueryStatus = DbEnum.QueryStatusEnum.SPQ;
        }

        // #x-start

        /// <summary>システム共通のCommandTimeout値を設定する。</summary>
        /// <param name="cmd">Commandオブジェクト</param>
        protected void SetCommandTimeout(IDbCommand cmd)
        {
            string sqlCommandTimeout = GetConfigParameter.GetConfigValue(PubLiteral.SQL_COMMANDTIMEOUT);

            if (sqlCommandTimeout == null || sqlCommandTimeout == "")
            {
                // デフォルト：設定しない。
                return;
            }
            else
            {
                int ret =0;

                if(int.TryParse(sqlCommandTimeout, out ret))
                {
                    if (0 < ret) // 2009/08/12-この行
                    {
                        // 指定の値を設定
                        cmd.CommandTimeout = ret;
                        return;
                    }
                    else{}
                }
                else{}
            }

            // CommandTimeout値の指定に誤りがある。
            throw new ArgumentException(PublicExceptionMessage.COMMANDTIMEOUT_ERROR);
        }

        // #x-end

        #endregion

        #region ファイルからクエリ ファイルをロードするメソッド

        /// <summary>SQLファイルから、SQL文を読み込む。</summary>
        /// <param name="sqlFilePath">SQLファイルのフルパス</param>
        /// <returns>SQL文</returns>
        /// <remarks>派生のDamXXXから利用する。</remarks>
        protected string Load(string sqlFilePath)
        {
            // SQL文
            string sql = null;

            // sqlのキャッシュスイッチ
            string sqlCacheSwitch = GetConfigParameter.GetConfigValue(PubLiteral.SQL_CACHE_SWITCH);

            if (sqlCacheSwitch == null)
            {
                // デフォルト：OFF扱い
                sqlCacheSwitch = PubLiteral.OFF;
            }

            // sqlのキャッシュスイッチを確認する
            if (sqlCacheSwitch.ToUpper() == PubLiteral.ON)// キャッシュ：ON
            {
                // SQLをキャッシュするディクショナリへのアクセスをスレッドセーフに実装するため、
                // lockステートメントを使用してこのセクションをクリティカルセクションとする。

                lock (BaseDam._lock)
                {
                    // null対策
                    if (BaseDam._sqlHt == null)
                    {
                        BaseDam._sqlHt = new Dictionary<string,string>();
                    }

                    // キャッシュから取得する。
                    // 2010/10/13 - ContainsKeyによるチェック処理を追加した。
                    if (BaseDam._sqlHt.ContainsKey(sqlFilePath))
                    {
                        sql = (string)BaseDam._sqlHt[sqlFilePath];
                    }

                    // エントリなし
                    if (sql == null)
                    {
                        // SQL文をロードする。
                        sql = this.Load2(sqlFilePath);
                        
                        // SQL文をキャッシュ
                        BaseDam._sqlHt.Add(sqlFilePath, sql);
                    }
                }                
            }
            else if (sqlCacheSwitch.ToUpper() == PubLiteral.OFF)// キャッシュ：OFF
            {
                // SQL文をロードする。
                sql = this.Load2(sqlFilePath);
            }
            else
            {
                // sqlのキャッシュスイッチの指定に誤りがある。
                throw new ArgumentException(String.Format(
                    PublicExceptionMessage.SWITCH_ERROR, PubLiteral.SQL_CACHE_SWITCH));
            }

            // SQL文を返す
            return sql;
        }

        /// <summary>SQLファイルから、SQL文を読み込む。</summary>
        /// <param name="sqlFilePath">SQLファイルのフルパス</param>
        /// <returns>SQL文</returns>
        private string Load2(string sqlFilePath)
        {
            // SQLファイルのEncoding情報の取得
            string sqlEncoding = GetConfigParameter.GetConfigValue(PubLiteral.SQL_ENCODING);

            if (sqlEncoding == null || sqlEncoding == "")
            {
                // デフォルト：UTF-8
                sqlEncoding = "utf-8";
            }

            // SQL文をロードする。
            if (EmbeddedResourceLoader.Exists(sqlFilePath, false))
            {
                // EmbeddedResourceにある場合、
                return EmbeddedResourceLoader.LoadAsString(sqlFilePath, Encoding.GetEncoding(sqlEncoding));
                // sqlEncodingを使用するのでLoadXMLAsStringは不要。
            }
            else
            {
                // EmbeddedResourceにない場合、
                return ResourceLoader.LoadAsString(sqlFilePath, Encoding.GetEncoding(sqlEncoding));
            }
        }

        #endregion

        #region 動的パラメタライズド・クエリのメイン機能

        #region 動的パラメタライズド クエリの確認メソッド

        /// <summary>
        /// 指定されたコマンド テキストが、
        /// 動的パラメタライズド クエリであるか確認する。
        /// </summary>
        /// <param name="commandText">コマンド テキスト</param>
        /// <remarks>派生のDamXXXから利用する。</remarks>
        protected void CheckCommandText(string commandText)
        {

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            PerformanceRecorder perfRec = new PerformanceRecorder();
            perfRec.StartsPerformanceRecord();
            Debug.WriteLine("CheckCommandText（開始）");
#endif

            // ここでDamのメンバ変数の状態を初期化する。
            this.init();

            // 2008/10/16---チェック処理の変更（ここから）

            // XMLかどうかを確認するフラグ。
            bool isXML = false;

            try
            {
                // XLMであるか確認する。
                this._xml = new XmlDocument();
                StringReader sr = new StringReader(commandText);
                this._xml.Load(sr);

                // XMLではある。
                isXML = true;
            }
            catch
            {
                // XMLではない。
                isXML = false;
            }

            if (isXML)
            {
                // デバッグ時は構文チェックする。

                // XMLの場合、DPQであるか確認する。
                foreach (XmlNode xmlNode in this._xml.ChildNodes)
                {
                    // 大文字・小文字は区別する。
                    if (xmlNode.Name == PubLiteral.DPQ_TAG_ROOT)
                    {
                        this.Scan(xmlNode);
                        this._QueryStatus = DbEnum.QueryStatusEnum.DPQ;
                    }
                }
            }            

            // 2008/10/16---チェック処理の変更（ここまで）

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            Debug.WriteLine("CheckCommandText（終了）：" + perfRec.EndsPerformanceRecord());
#endif

        }

        /// <summary>
        /// 動的パラメタライズド クエリであるか確認するメソッド
        /// 構文を大まかに確認する。
        /// </summary>
        /// <param name="xmlNode1">ROOTノード</param>
        private void Scan(XmlNode xmlNode1)
        {
            foreach (XmlNode xmlNode2 in xmlNode1.ChildNodes)
            {
                // 子ノードの名称を取得
                string name = xmlNode2.Name;

                // 2009/09/24---並びの見直し（編集処理順に合わせた）

                // 子ノードの名称をチェック、大文字・小文字は区別する。
                // 2008/12/25---新機能の追加（INSCOL、DELCMAタグ）
                // 2010/09/24---新機能の追加（SELECT-CASE-DEFAULTタグ）
                // 2011/02/08---チェック緩和（COMMENTタグ）
                // 2012/09/10---チェック緩和（CDATAタグ）
                if (name == PubLiteral.DPQ_TAG_VAL ||
                    name == PubLiteral.DPQ_TAG_INSCOL ||
                    name == PubLiteral.DPQ_TAG_IF ||
                    name == PubLiteral.DPQ_TAG_ELSE ||
                    name == PubLiteral.DPQ_TAG_SELECT ||
                    name == PubLiteral.DPQ_TAG_CASE ||
                    name == PubLiteral.DPQ_TAG_DEFAULT ||
                    name == PubLiteral.DPQ_TAG_LIST ||
                    name == PubLiteral.DPQ_TAG_JOIN ||
                    name == PubLiteral.DPQ_TAG_SUB ||
                    name == PubLiteral.DPQ_TAG_WHERE ||
                    name == PubLiteral.DPQ_TAG_DELCMA ||
                    name == PubLiteral.DPQ_TAG_PARAM ||
                    name == PubLiteral.DPQ_TAG_DIV ||
                    name == PubLiteral.DPQ_TAG_TEXT ||
                    name == PubLiteral.DPQ_TAG_COMMENT ||
                    name == PubLiteral.DPQ_TAG_CDATA)
                {
                    #region 個別チェック

                    #region VALタグ

                    if (name == PubLiteral.DPQ_TAG_VAL)
                    {
                        // VALタグに子ノードは存在しない。

                        // 2008/10/16---チェック処理の変更（ここから）

                        if (xmlNode2.ChildNodes.Count == 0)
                        {
                            // 正常
                        }
                        else
                        {
                            throw new ArgumentException(String.Format(
                                PublicExceptionMessage.DPQ_TAG_FORMAT_ERROR, PubLiteral.DPQ_TAG_VAL));
                        }

                        // 2008/10/16---チェック処理の変更（ここまで）
                    }

                    #endregion

                    #region JOINタグ（旧：削除）

                    //if (name == PubLiteral.DPQ_TAG_JOIN)
                    //{
                    //    // JOINタグの中は、「#text」・「VAL」タグのみ。

                    //    // 2008/10/16---チェック処理の変更（ここから）

                    //    foreach (XmlNode xmlNode3 in xmlNode2.ChildNodes)
                    //    {
                    //        if (xmlNode3.Name == PubLiteral.DPQ_TAG_TEXT
                    //            || xmlNode3.Name == PubLiteral.DPQ_TAG_VAL)
                    //        {
                    //            // 正常
                    //        }
                    //        else
                    //        {
                    //            throw new ArgumentException(String.Format(
                    //                PublicExceptionMessage.DPQ_TAG_FORMAT_ERROR, PubLiteral.DPQ_TAG_JOIN));
                    //        }
                    //    }

                    //    // 2008/10/16---チェック処理の変更（ここまで）
                    //}                    

                    #endregion

                    #region INSCOLタグ

                    // 2008/12/25---新機能の追加（ここから）

                    if (name == PubLiteral.DPQ_TAG_INSCOL)
                    {
                        // INSCOLタグの中は、「#text」・「#comment」・「#cdata-section」・「VAL」タグのみ。

                        foreach (XmlNode xmlNode3 in xmlNode2.ChildNodes)
                        {
                            if (xmlNode3.Name == PubLiteral.DPQ_TAG_TEXT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_COMMENT 
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_CDATA
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_VAL)
                            {
                                // 正常
                            }
                            else
                            {
                                throw new ArgumentException(String.Format(
                                    PublicExceptionMessage.DPQ_TAG_FORMAT_ERROR, PubLiteral.DPQ_TAG_INSCOL));
                            }
                        }
                    }

                    // 2008/12/25---新機能の追加（ここまで）

                    #endregion

                    #region IFタグ

                    if (name == PubLiteral.DPQ_TAG_IF)
                    {
                        // IFタグの中は、「#text」・「#comment」・「#cdata-section」・「ELSE」・「VAL」タグのみ。

                        // 2008/10/16---チェック処理の変更（ここから）

                        foreach (XmlNode xmlNode3 in xmlNode2.ChildNodes)
                        {
                            if (xmlNode3.Name == PubLiteral.DPQ_TAG_TEXT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_COMMENT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_CDATA
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_ELSE
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_VAL)
                            {
                                // 正常
                            }
                            else
                            {
                                throw new ArgumentException(String.Format(
                                    PublicExceptionMessage.DPQ_TAG_FORMAT_ERROR, PubLiteral.DPQ_TAG_IF));
                            }
                        }

                        // 2008/10/16---チェック処理の変更（ここまで）
                    }

                    #endregion

                    #region ELSEタグ

                    if (name == PubLiteral.DPQ_TAG_ELSE)
                    {
                        // ELSEタグの中は、「#text」・「#comment」・「#cdata-section」・「VAL」タグのみ。

                        // 2008/10/16---チェック処理の変更（ここから）

                        foreach (XmlNode xmlNode3 in xmlNode2.ChildNodes)
                        {
                            if (xmlNode3.Name == PubLiteral.DPQ_TAG_TEXT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_COMMENT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_CDATA
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_VAL)
                            {
                                // 正常
                            }
                            else
                            {
                                throw new ArgumentException(String.Format(
                                    PublicExceptionMessage.DPQ_TAG_FORMAT_ERROR, PubLiteral.DPQ_TAG_ELSE));
                            }
                        }

                        // 2008/10/16---チェック処理の変更（ここまで）
                    }

                    #endregion

                    #region SELECT-CASE-DEFAULTタグ

                    // 2010/09/24---新機能の追加（ここから）

                    // SELECTタグ
                    if (name == PubLiteral.DPQ_TAG_SELECT)
                    {
                        // SELECTタグの中は、「#comment」・「#cdata-section」・「CASE」・「DEFAULT」タグのみ。

                        foreach (XmlNode xmlNode3 in xmlNode2.ChildNodes)
                        {
                            if (xmlNode3.Name == PubLiteral.DPQ_TAG_COMMENT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_CDATA
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_CASE
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_DEFAULT)
                            {
                                // 正常
                            }
                            else
                            {
                                throw new ArgumentException(String.Format(
                                    PublicExceptionMessage.DPQ_TAG_FORMAT_ERROR, PubLiteral.DPQ_TAG_SELECT));
                            }
                        }
                    }

                    // CASEタグ
                    if (name == PubLiteral.DPQ_TAG_CASE)
                    {
                        // CASEタグの中は、「#text」・「#comment」・「#cdata-section」・「VAL」・「IF-ELSE」タグのみ。

                        foreach (XmlNode xmlNode3 in xmlNode2.ChildNodes)
                        {
                            if (xmlNode3.Name == PubLiteral.DPQ_TAG_TEXT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_COMMENT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_CDATA
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_VAL
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_IF)// ELSEはIFの子だから不要
                            {
                                // 正常
                            }
                            else
                            {
                                throw new ArgumentException(String.Format(
                                    PublicExceptionMessage.DPQ_TAG_FORMAT_ERROR, PubLiteral.DPQ_TAG_CASE));
                            }
                        }
                    }

                    // DEFAULTタグ
                    if (name == PubLiteral.DPQ_TAG_DEFAULT)
                    {
                        // DEFAULTタグの中は、「#text」・「#comment」・「#cdata-section」・「VAL」・「IF-ELSE」タグのみ。

                        foreach (XmlNode xmlNode3 in xmlNode2.ChildNodes)
                        {
                            if (xmlNode3.Name == PubLiteral.DPQ_TAG_TEXT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_COMMENT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_CDATA
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_VAL
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_IF)// ELSEはIFの子だから不要
                            {
                                // 正常
                            }
                            else
                            {
                                throw new ArgumentException(String.Format(
                                    PublicExceptionMessage.DPQ_TAG_FORMAT_ERROR, PubLiteral.DPQ_TAG_DEFAULT));
                            }
                        }
                    }

                    // 2010/09/24---新機能の追加（ここまで）

                    #endregion

                    #region LISTタグ

                    if (name == PubLiteral.DPQ_TAG_LIST)
                    {
                        // LISTタグの中は、「#text」・「#comment」・「#cdata-section」・「VAL」タグのみ。

                        // 2008/10/16---チェック処理の変更（ここから）

                        foreach (XmlNode xmlNode3 in xmlNode2.ChildNodes)
                        {
                            if (xmlNode3.Name == PubLiteral.DPQ_TAG_TEXT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_COMMENT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_CDATA
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_VAL)
                            {
                                // 正常
                            }
                            else
                            {
                                throw new ArgumentException(String.Format(
                                    PublicExceptionMessage.DPQ_TAG_FORMAT_ERROR, PubLiteral.DPQ_TAG_LIST));
                            }
                        }

                        // 2008/10/16---チェック処理の変更（ここまで）
                    }

                    #endregion

                    #region  ネストの自由度が高いので、チェックしない。
                    
                    // JIONタグ

                    // SUBタグ

                    // WHEREタグ

                    // DELCMAタグ

                    #endregion

                    #region PARAMタグ

                    if (name == PubLiteral.DPQ_TAG_PARAM)
                    {
                        // PARAMタグの中は、「#text」・「#comment」・「#cdata-section」・「DIV」タグのみ。

                        // 2008/10/16---チェック処理の変更（ここから）

                        foreach (XmlNode xmlNode3 in xmlNode2.ChildNodes)
                        {
                            if (xmlNode3.Name == PubLiteral.DPQ_TAG_TEXT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_COMMENT
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_CDATA
                                || xmlNode3.Name == PubLiteral.DPQ_TAG_DIV)
                            {
                                // 正常
                            }
                            else
                            {
                                throw new ArgumentException(String.Format(
                                    PublicExceptionMessage.DPQ_TAG_FORMAT_ERROR, PubLiteral.DPQ_TAG_PARAM));
                            }
                        }

                        // 2008/10/16---チェック処理の変更（ここまで）
                    }

                    #endregion

                    #region DIVタグ

                    if (name == PubLiteral.DPQ_TAG_DIV)
                    {
                        // 2008/10/16---チェック処理の変更（ここから）

                        if (xmlNode2.ChildNodes.Count == 0)
                        {
                            // 正常
                        }
                        else
                        {
                            throw new ArgumentException(String.Format(
                                    PublicExceptionMessage.DPQ_TAG_FORMAT_ERROR, PubLiteral.DPQ_TAG_DIV));
                        }

                        // 2008/10/16---チェック処理の変更（ここまで）
                    }

                    #endregion

                    // TEXT、COMMENT、CDATAタグはスルー

                    #endregion

                    // チェック後、問題が無ければ、さらに子ノードを検索する（再帰）。

                    // 2008/10/16---チェック処理の変更（ここから）

                    this.Scan(xmlNode2);

                    // 2008/10/16---チェック処理の変更（ここまで）
                }
                else
                {
                    // チェック後、問題があれば、検索を終了する（再帰せずに戻す）。

                    // 2008/10/16---チェック処理の変更（ここから）

                    throw new ArgumentException(PublicExceptionMessage.THIS_DPQ_TAG_IS_UNKNOWN);

                    // 2008/10/16---チェック処理の変更（ここまで）
                }
            }

            // カレントノードの子ノードのチェック完了
            // ルートの場合は全てのノードのチェック完了

            // 2008/10/16---チェック処理の変更（ここから）
            //return true;
            // 2008/10/16---チェック処理の変更（ここまで）

        }

        #endregion

        #region 動的パラメタライズド クエリの変換メソッド

        #region ルート

        /// <summary>
        /// 動的パラメタライズド クエリを
        /// 通常のパラメタライズド クエリに変換するメソッド
        /// </summary>
        /// <param name="paramSign">パラメタの先頭記号（DBMSによって可変）</param>
        /// <returns>変換後の通常のパラメタライズド クエリ</returns>
        /// <remarks>派生のDamXXXから利用する。</remarks>
        protected string Convert(char paramSign)
        {

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            PerformanceRecorder perfRec = null;
#endif

            #region 変換処理

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            perfRec = new PerformanceRecorder();
            perfRec.StartsPerformanceRecord();
            Debug.WriteLine("ReplaceVALTag（開始）");
#endif

            // VALタグを対応する値で置換する。
            this.ReplaceVALTag();

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            Debug.WriteLine("ReplaceVALTag（終了）：" + perfRec.EndsPerformanceRecord());
#endif

//#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
//            perfRec = new PerformanceRecorder();
//            perfRec.StartsPerformanceRecord();
//            Debug.WriteLine("ProcessJOINTag（開始）");
//#endif

//            // JOINタグを処理する。
//            this.ProcessJOINTag();

//#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
//            Debug.WriteLine("ProcessJOINTag（終了）：" + perfRec.EndsPerformanceRecord());
//#endif

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            perfRec = new PerformanceRecorder();
            perfRec.StartsPerformanceRecord();
            Debug.WriteLine("ProcessINSCOLTag（開始）");
#endif

            // 2008/12/25---新機能の追加（ここから）
            // INSCOLタグを処理する。
            this.ProcessINSCOLTag();
            // 2008/12/25---新機能の追加（ここまで）

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            Debug.WriteLine("ProcessINSCOLTag（終了）：" + perfRec.EndsPerformanceRecord());
#endif

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            perfRec = new PerformanceRecorder();
            perfRec.StartsPerformanceRecord();
            Debug.WriteLine("ProcessIFTag（開始）");
#endif

            // IFタグを処理する。
            this.ProcessIFTag(paramSign);

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            Debug.WriteLine("ProcessIFTag（終了）：" + perfRec.EndsPerformanceRecord());
#endif

#if PERFORMANCE_LOG_SWITCH
            perfRec = new PerformanceRecorder();
            perfRec.StartsPerformanceRecord();
            Debug.WriteLine("ProcessSelectCaseDefaultTag（開始）");
#endif
            // 2010/09/24---新機能の追加（ここから）
            // SELECT-CASE-DEFAULTタグを処理する。
            this.ProcessSelectCaseDefaultTag();
            // 2010/09/24---新機能の追加（ここまで）

#if PERFORMANCE_LOG_SWITCH
            Debug.WriteLine("ProcessSelectCaseDefaultTag（終了）：" + perfRec.EndsPerformanceRecord());
#endif

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            perfRec = new PerformanceRecorder();
            perfRec.StartsPerformanceRecord();
            Debug.WriteLine("ProcessLISTTag（開始）");
#endif

            // LISTタグを処理する。
            this.ProcessLISTTag(paramSign);

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            Debug.WriteLine("ProcessLISTTag（終了）：" + perfRec.EndsPerformanceRecord());
#endif

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            perfRec = new PerformanceRecorder();
            perfRec.StartsPerformanceRecord();
            Debug.WriteLine("ProcessJoinSubWhereTag（開始）");
#endif

            // JOIN、SUB、WHEREタグを処理する。
            this.ProcessJoinSubWhereTag();　// 2010/09/24---JOINタグのネスト条件緩和

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            Debug.WriteLine("ProcessJoinSubWhereTag（終了）：" + perfRec.EndsPerformanceRecord());
#endif

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            perfRec = new PerformanceRecorder();
            perfRec.StartsPerformanceRecord();
            Debug.WriteLine("ProcessDELCMATag（開始）");
#endif

            // 2008/12/24---新機能の追加（ここから）
            // DELCMAタグを処理する。
            this.ProcessDELCMATag();
            // 2008/12/24---新機能の追加（ここまで）

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            Debug.WriteLine("ProcessDELCMATag（終了）：" + perfRec.EndsPerformanceRecord());
#endif

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            perfRec = new PerformanceRecorder();
            perfRec.StartsPerformanceRecord();
            Debug.WriteLine("DeleteParamTag（開始）");
#endif

            // 実行に不要なPARAMタグを消去する。
            this.DeleteParamTag();

#if PERFORMANCE_LOG_SWITCH // 2009/09/25-このプリプロセッサ
            Debug.WriteLine("DeleteParamTag（終了）：" + perfRec.EndsPerformanceRecord());
#endif

            #endregion

            // 戻す
            return this._xml.InnerText;
        } 

        #endregion

        #region 各タグ

        #region VALタグ

        /// <summary>VALタグを対応する値で置換する。</summary>
        private void ReplaceVALTag()
        {
            // すべてのVALタグを取得、大文字・小文字は区別する。
            XmlNodeList xmlNodeList = this._xml.GetElementsByTagName(PubLiteral.DPQ_TAG_VAL);

            // VALタグは処理後に削除されるので常に０番を指定
            XmlNode xmlNodeVal = xmlNodeList[0];

            if (xmlNodeVal != null)
            {
                // VALタグのname属性からユーザパラメタ名を取得。
                // 大文字・小文字は区別する。
                XmlNode xmlNodeParam = xmlNodeVal.Attributes.GetNamedItem("name");

                // エラー処理
                if (xmlNodeParam == null)
                {
                    // VALタグにname属性が設定されていない。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.DPQ_TAG_NAME_ATTR_NOT_EXIST, PubLiteral.DPQ_TAG_VAL));
                }
                else
                {
                    if (xmlNodeParam.Value == "")
                    {
                        // VALタグにname属性が設定されていない。
                        throw new ArgumentException(String.Format(
                            PublicExceptionMessage.DPQ_TAG_NAME_ATTR_VALUE_IS_EMPTY, PubLiteral.DPQ_TAG_VAL));
                    }
                }

                // 2010/10/13 - ContainsKeyによるチェック処理を追加した。
                object val = null;

                if (this._userParameter.ContainsKey(xmlNodeParam.Value))
                {
                    val = this._userParameter[xmlNodeParam.Value];
                }

                // エラー処理
                if (val == null)
                {
                    // 2009/01/06---新機能の追加（ここから）

                    //// VALタグを削除（半角スペースに変換）する。
                    //XmlText xmlText = this._xml.CreateTextNode(" ");

                    // VALタグを削除（空文字列に変換）する。
                    XmlText xmlText = this._xml.CreateTextNode("");

                    // 2009/01/06---新機能の追加（ここまで）

                    xmlNodeVal.ParentNode.ReplaceChild(xmlText, xmlNodeVal);
                }
                else
                {
                    // 2009/01/06---新機能の追加（ここから）

                    //// VALタグを対応する値で置換する。
                    //XmlText xmlText = this._xml.CreateTextNode(" " + val.ToString() + " ");

                    // VALタグを対応する値で置換する（前後に、半角スペースを付けない）。
                    XmlText xmlText = this._xml.CreateTextNode(val.ToString());

                    // 2009/01/06---新機能の追加（ここまで）

                    xmlNodeVal.ParentNode.ReplaceChild(xmlText, xmlNodeVal);
                }
                
                // 次のVALタグを探すため再帰する。
                this.ReplaceVALTag();
            }
            else
            {
                // VALタグの置換が完了した
            }
        }

        #endregion

        #region JOINタグ（旧：削除）

        ///// <summary>JOINタグを処理する。</summary>
        //private void ProcessJOINTag()
        //{
        //    XmlNodeList xmlNodeList = null;
        //    XmlNode xmlNodeJOINTag = null;

        //    // すべてのJOINタグを取得、大文字・小文字は区別する。
        //    xmlNodeList = this._xml.GetElementsByTagName(PubLiteral.DPQ_TAG_JOIN);

        //    // SELECTタグは処理後に削除されるので常に０番を指定
        //    xmlNodeJOINTag = xmlNodeList[0];

        //    if (xmlNodeJOINTag != null)
        //    {

        //        // JOINタグのname属性からユーザパラメタ名を取得。
        //        // 大文字・小文字は区別する。
        //        XmlNode xmlNodeParam = xmlNodeJOINTag.Attributes.GetNamedItem("name");

        //        // エラー処理
        //        if (xmlNodeParam == null)
        //        {
        //            // JOINタグにname属性が設定されていない。
        //            throw new ArgumentException(String.Format(
        //                PublicExceptionMessage.DPQ_TAG_NAME_ATTR_NOT_EXIST, PubLiteral.DPQ_TAG_JOIN));
        //        }
        //        else
        //        {
        //            if (xmlNodeParam.Value == "")
        //            {
        //                // JOINタグにname属性が設定されていない。
        //                throw new ArgumentException(String.Format(
        //                    PublicExceptionMessage.DPQ_TAG_NAME_ATTR_VALUE_IS_EMPTY, PubLiteral.DPQ_TAG_JOIN));
        //            }
        //        }

        //        // パラメタ名は退避しておく（タグが消されることがあるので）
        //        string paramName = xmlNodeParam.Value;

        //        // パラメタを取得
        //        object obj = (object)this._parameter[paramName];

        //        // エラー処理
        //        if (obj == null)
        //        {
        //            // JOINタグに対応するパラメタにデータが
        //            // 設定されていない。 or nullに設定されている。

        //            // JOINタグを削除（半角スペースに変換）する。
        //            XmlText xmlText = this._xml.CreateTextNode(" ");
        //            xmlNodeJOINTag.ParentNode.ReplaceChild(xmlText, xmlNodeJOINTag);
        //        }
        //        else
        //        {
        //            // 型を確認する。
        //            if (obj.GetType().ToString() == typeof(Boolean).ToString())
        //            {
        //                // Boolean型の場合は注意
        //                if (((bool)obj))
        //                {
        //                    // trueの場合、残す。

        //                    // JOINタグを削除（InnerTextで置換する）する。
        //                    XmlText xmlText = this._xml.CreateTextNode(" " + xmlNodeJOINTag.InnerText + " ");
        //                    xmlNodeJOINTag.ParentNode.ReplaceChild(xmlText, xmlNodeJOINTag);
        //                }
        //                else
        //                {
        //                    // falseの場合、（nullでなくても）消す。

        //                    // JOINタグを削除（半角スペースに変換）する。
        //                    XmlText xmlText = this._xml.CreateTextNode(" ");
        //                    xmlNodeJOINTag.ParentNode.ReplaceChild(xmlText, xmlNodeJOINTag);
        //                }
        //            }
        //            else
        //            {
        //                // nullでなく、Boolean型以外の場合はエラー。
        //                throw new ArgumentException(String.Format(
        //                    PublicExceptionMessage.DPQ_SET_ONLY_NULL_OR_BOOL_TO_INNER_PARAM_VALUE,
        //                        PubLiteral.DPQ_TAG_JOIN));
        //            }
        //        }

        //        // パラメタを削除する。
        //        this._parameter.Remove(paramName);

        //        // 次のLISTタグを探すため再帰する。
        //        this.ProcessJOINTag();
        //    }
        //    else
        //    {
        //        // JOINタグの処理が完了した
        //    }
        //}

        #endregion

        #region INSCOLタグ

        // 2008/12/25---新機能の追加（ここから）

        /// <summary>INSCOLタグを処理する。</summary>
        private void ProcessINSCOLTag()
        {
            // すべてのINSCOLタグを取得、大文字・小文字は区別する。
            XmlNodeList xmlNodeList = this._xml.GetElementsByTagName(PubLiteral.DPQ_TAG_INSCOL);

            // INSCOLタグは処理後に削除されるので常に０番を指定
            XmlNode xmlNodeInsCol = xmlNodeList[0];

            if (xmlNodeInsCol != null)
            {
                // INSCOLタグのname属性から対応するパラメタ名を取得。
                // 大文字・小文字は区別する。
                XmlNode xmlNodeParam = xmlNodeInsCol.Attributes.GetNamedItem("name");

                // エラー処理
                if (xmlNodeParam == null)
                {
                    // INSCOLタグにname属性が設定されていない。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.DPQ_TAG_NAME_ATTR_NOT_EXIST, PubLiteral.DPQ_TAG_INSCOL));
                }
                else
                {
                    if (xmlNodeParam.Value == "")
                    {
                        // INSCOLタグにname属性が設定されていない。
                        throw new ArgumentException(String.Format(
                            PublicExceptionMessage.DPQ_TAG_NAME_ATTR_VALUE_IS_EMPTY, PubLiteral.DPQ_TAG_INSCOL));
                    }
                }

                // 2010/10/13 - ContainsKeyによるチェック処理を追加した。
                object val = null;

                if (this._parameter.ContainsKey(xmlNodeParam.Value))
                {
                    val = this._parameter[xmlNodeParam.Value];
                }

                // エラー処理
                if (val == null)
                {
                    // ※ パラメタが設定されていない場合、
                    //    パラメタがNULLに設定されている場合の双方に対応。
                    
                    // INSCOLタグを削除（半角スペースに変換）する。
                    XmlText xmlText = this._xml.CreateTextNode(" ");
                    xmlNodeInsCol.ParentNode.ReplaceChild(xmlText, xmlNodeInsCol);
                }
                else
                {
                    // INSCOLタグを削除（InnerTextで置換する）する。
                    XmlText xmlText = this._xml.CreateTextNode(" " + xmlNodeInsCol.InnerText + " ");
                    xmlNodeInsCol.ParentNode.ReplaceChild(xmlText, xmlNodeInsCol);
                }

                // 次のINSCOLタグを探すため再帰する。
                this.ProcessINSCOLTag();
            }
            else
            {
                // INSCOLタグの置換が完了した
            }
        }

        // 2008/12/25---新機能の追加（ここまで）

        #endregion

        #region IF(ELSE)タグ

        /// <summary>IF(ELSE)タグを処理する。</summary>
        /// <param name="paramSign">パラメタの先頭記号（DBMSによって可変）</param>
        private void ProcessIFTag(char paramSign)
        {
            #region 変数
            
            // IFタグのリスト
            XmlNodeList xmlNodeList = null;

            // IFタグ
            XmlNode xmlNodeIf = null;
            
            // ELSEタグ
            XmlNode xmlNodeElse = null;

            // パラメタ名
            string paramName = "";

            // パラメタライズド・クエリのパラメタが無い場合
            // この場合、最後に、パラメタを消去する。
            bool isNoPRQP = true;

            // 置換処理用
            XmlText xmlText;

            #endregion 

            // すべてのIFタグを取得、大文字・小文字は区別する。
            xmlNodeList = this._xml.GetElementsByTagName(PubLiteral.DPQ_TAG_IF);

            // IFタグは処理後に削除されるので常に０番を指定
            xmlNodeIf = xmlNodeList[0];

            if (xmlNodeIf != null)
            {
                #region パラメタを取得

                // IFタグ内のテキスト要素からパラメタ名を取得する。
                string ifText = "";
                foreach(XmlNode xmlNodeIfChild in xmlNodeIf.ChildNodes)
                {
                    if(xmlNodeIfChild.Name == PubLiteral.DPQ_TAG_TEXT
                        || xmlNodeIfChild.Name == PubLiteral.DPQ_TAG_CDATA)
                    {
                        ifText += " " + xmlNodeIfChild.Value;
                    }
                }

                // パラメタライズド・クエリのパラメタ（テキスト内パラメタ）を取得。
                paramName = this.GetParamByText(ifText, paramSign);

                if (paramName == "")
                {
                    // パラメタライズド・クエリのパラメタ（テキスト内パラメタ）が存在しない。

                    // IFタグのname属性のユーザパラメタ（タグ内パラメタ）を取得。
                    // 大文字・小文字は区別する。
                    XmlNode xmlNodeParam = xmlNodeIf.Attributes.GetNamedItem("name");

                    // エラー処理
                    if (xmlNodeParam == null)
                    {
                        // IFタグのname属性のユーザパラメタ（タグ内パラメタ）が設定されていない。
                        throw new ArgumentException(String.Format(
                            PublicExceptionMessage.DPQ_TAG_NAME_ATTR_NOT_EXIST, PubLiteral.DPQ_TAG_IF));
                    }
                    else
                    {
                        if (xmlNodeParam.Value == "")
                        {
                            // IFタグのname属性のユーザパラメタ（タグ内パラメタ）が設定されていない。
                            throw new ArgumentException(String.Format(
                                PublicExceptionMessage.DPQ_TAG_NAME_ATTR_VALUE_IS_EMPTY, PubLiteral.DPQ_TAG_IF));
                        }
                    }

                    // IFタグのname属性のユーザパラメタ（タグ内パラメタ）値を取得。
                    paramName = xmlNodeParam.Value;

                    // パラメタライズド・クエリのパラメタ（テキスト内パラメタ）が存在しない。
                    isNoPRQP = true;
                }
                else
                {
                    // パラメタライズド・クエリのパラメタ（テキスト内パラメタ）が存在する。
                    isNoPRQP = false;
                }

                // 2010/10/13 - ContainsKeyによるチェック処理を追加した。
                // パラメタを取得
                object obj = null;
                if (this._parameter.ContainsKey(paramName))
                {
                    obj = this._parameter[paramName];
                }

                #endregion

                #region ELSEタグを取得する。

                // IFタグの子ノードにELSEタグがあるか確認する。
                foreach (XmlNode xmlNode in xmlNodeIf.ChildNodes)
                {
                    if (xmlNode.Name == PubLiteral.DPQ_TAG_ELSE)// 大文字・小文字は区別する。
                    {
                        xmlNodeElse = xmlNode;
                    }
                }

                #endregion

                #region IFタグを処理する。

                // パラメタが設定されているか調べる
                if (this._parameter.ContainsKey(paramName)) // Dic化でメソッド名が微変化
                {
                    // パラメタが設定されている場合

                    if (obj == null)
                    {
                        // パラメタがnullに設定されている場合

                        // ELSEを有効にする。

                        // ELSEタグがあるか調べる。
                        if (xmlNodeElse != null)
                        {
                            // ELSEタグがある

                            // 2008/10/16---タグ編集処理の変更（ここから）
                            
                            // ELSEにはパラメタが無い仕様なので、パラメタ消去する。
                            isNoPRQP = true;

                            // 2008/10/16---タグ編集処理の変更（ここまで）

                            // ELSEを有効にする（IFタグをELSEタグのInnerTextで置換する）。
                            xmlText = this._xml.CreateTextNode(" " + xmlNodeElse.InnerText + " ");
                            xmlNodeIf.ParentNode.ReplaceChild(xmlText, xmlNodeIf);
                        }
                        else
                        {
                            // ELSEタグがない

                            // 2008/10/16---タグ編集処理の変更（ここから）

                            // フラグを確認する。
                            if (isNoPRQP)
                            {
                                // タグ内パラメタ → エラー
                                throw new ArgumentException(
                                    PublicExceptionMessage.DPQ_ELSE_TAG_DOESNT_EXIST_WHEN_INNER_PARAM_OF_IF_TAG_IS_NULL);
                            }
                            else
                            {
                                // テキスト内パラメタ → エラー
                                throw new ArgumentException(
                                    PublicExceptionMessage.DPQ_ELSE_TAG_DOESNT_EXIST_WHEN_TEXT_PARAM_OF_IF_TAG_IS_NULL);
                            }

                            // 2008/10/16---タグ編集処理の変更（ここまで）
                        }                        
                    }
                    else
                    {
                        // パラメタがnull以外に設定されている場合

                        // 2008/10/16---タグ編集処理の変更（ここから）

                        // フラグを確認する。
                        if (isNoPRQP)
                        {
                            // パラメタライズド・クエリのパラメタが無い場合
                            // （テキスト内パラメタでなく、タグ内パラメタの場合）

                            if (obj.GetType() == typeof(Boolean))
                            {
                                // Boolean型の場合（タグ内パラメタは通常、Boolean型）

                                if (((bool)obj))
                                {
                                    // trueの場合、IFを有効にする。

                                    // ELSEタグがある場合、ELSEタグを削除
                                    if (xmlNodeElse != null)
                                    {
                                        // ELSEタグを削除する（半角スペースに変換する）。
                                        xmlNodeElse.ParentNode.ReplaceChild(this._xml.CreateTextNode(" "), xmlNodeElse);
                                    }

                                    // IFタグを有効にする（InnerTextで置換する）。
                                    xmlText = this._xml.CreateTextNode(" " + xmlNodeIf.InnerText + " ");
                                    xmlNodeIf.ParentNode.ReplaceChild(xmlText, xmlNodeIf);
                                }
                                else
                                {
                                    // falseの場合、ELSEを有効にする。

                                    // ELSEタグがあるか調べる。
                                    if (xmlNodeElse != null)
                                    {
                                        // ELSEタグがある

                                        // 2008/10/16---タグ編集処理の変更（ここから）

                                        // ELSEにはパラメタが無い仕様なので、パラメタ消去する。
                                        isNoPRQP = true;

                                        // 2008/10/16---タグ編集処理の変更（ここまで）

                                        // ELSEを有効にする（IFタグをELSEタグのInnerTextで置換する）。
                                        xmlText = this._xml.CreateTextNode(" " + xmlNodeElse.InnerText + " ");
                                        xmlNodeIf.ParentNode.ReplaceChild(xmlText, xmlNodeIf);
                                    }
                                    else
                                    {
                                        // ELSEタグがない
                                        throw new ArgumentException(
                                            PublicExceptionMessage.DPQ_ELSE_TAG_DOESNT_EXIST_WHEN_INNER_PARAM_OF_IF_TAG_IS_FALSE);
                                    }
                                }
                            }
                            else
                            {
                                // nullでなく、Boolean型以外の場合はエラー（タグ内パラメタは通常、Boolean型）。
                                throw new ArgumentException(String.Format
                                    (PublicExceptionMessage.DPQ_SET_ONLY_NULL_OR_BOOL_TO_INNER_PARAM_VALUE,
                                        PubLiteral.DPQ_TAG_IF));
                            }                            
                        }
                        else
                        {
                            // 上記以外の場合（テキスト内パラメタに通常のパラメタが設定された場合）はIFを有効にする。

                            // ELSEタグがある場合、ELSEタグを削除
                            if (xmlNodeElse != null)
                            {
                                // ELSEタグを削除する（半角スペースに変換する）。
                                xmlNodeElse.ParentNode.ReplaceChild(this._xml.CreateTextNode(" "), xmlNodeElse);
                            }

                            // IFタグを有効にする（InnerTextで置換する）。
                            xmlText = this._xml.CreateTextNode(" " + xmlNodeIf.InnerText + " ");
                            xmlNodeIf.ParentNode.ReplaceChild(xmlText, xmlNodeIf);
                        }

                        // 2008/10/16---タグ編集処理の変更（ここまで）
                    }
                }
                else
                {
                    // パラメタが設定されていない場合

                    // IFタグを削除（半角スペースに変換）する。
                    xmlText = this._xml.CreateTextNode(" ");
                    xmlNodeIf.ParentNode.ReplaceChild(xmlText, xmlNodeIf);
                }

                // パラメタライズド・クエリのパラメタ（テキスト内パラメタ）が無い（無くなった）場合、
                if (isNoPRQP)
                {
                    // パラメタを削除する。
                    this._parameter.Remove(paramName);
                }                

                #endregion

                // 次のIFタグを探すため再帰する。
                this.ProcessIFTag(paramSign);                
            }
            else
            {
                // IFタグの処理が完了した
            }
        }

        #endregion

        #region SELECT-CASE-DEFAULTタグ

        // 2010/09/24---新機能の追加（ここから）

        /// <summary>SELECT-CASE-DEFAULTタグを処理</summary>
        private void ProcessSelectCaseDefaultTag()
        {
            XmlNodeList xmlNodeList = null;
            XmlNode xmlNodeSelectTag = null;

            // すべてのSELECTタグを取得、大文字・小文字は区別する。
            xmlNodeList = this._xml.GetElementsByTagName(PubLiteral.DPQ_TAG_SELECT);

            // SELECTタグは処理後に削除されるので常に０番を指定
            xmlNodeSelectTag = xmlNodeList[0];

            if (xmlNodeSelectTag != null)
            {
                // SELECTタグのname属性からユーザパラメタ名を取得。
                // 大文字・小文字は区別する。
                XmlNode xmlNodeParam = xmlNodeSelectTag.Attributes.GetNamedItem("name");

                // エラー処理
                if (xmlNodeParam == null)
                {
                    // SELECTタグにname属性が設定されていない。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.DPQ_TAG_NAME_ATTR_NOT_EXIST, PubLiteral.DPQ_TAG_SELECT));
                }
                else
                {
                    if (xmlNodeParam.Value == "")
                    {
                        // SELECTタグにname属性が設定されていない。
                        throw new ArgumentException(String.Format(
                            PublicExceptionMessage.DPQ_TAG_NAME_ATTR_VALUE_IS_EMPTY, PubLiteral.DPQ_TAG_SELECT));
                    }
                }

                // パラメタ名は退避しておく（タグが消されることがあるので）
                string paramName = xmlNodeParam.Value;

                // 2010/10/13 - ContainsKeyによるチェック処理を追加した。
                // パラメタを取得
                object obj = null;
                if (this._parameter.ContainsKey(paramName))
                {
                    obj = this._parameter[paramName];
                }


                // エラー処理
                if (obj == null)
                {
                    // SELECTタグに対応するパラメタにデータが
                    // 設定されていない。 or nullに設定されている。

                    // SELECTタグを削除（半角スペースに変換）する。
                    XmlText xmlText = this._xml.CreateTextNode(" ");
                    xmlNodeSelectTag.ParentNode.ReplaceChild(xmlText, xmlNodeSelectTag);
                }
                else
                {
                    // .ToString()したものが比較値になる。
                    string param = obj.ToString();

                    // 選択されたCASE-DEFAULTタグのInnerText
                    string selectedText = this.ProcessSelectCaseDefaultTag(xmlNodeSelectTag, param);

                    // SELECTタグを削除（取得した文字列で置換する）する。
                    XmlText xmlText = this._xml.CreateTextNode(" " + selectedText + " ");
                    xmlNodeSelectTag.ParentNode.ReplaceChild(xmlText, xmlNodeSelectTag);
                }

                // パラメタを削除する。
                this._parameter.Remove(paramName);

                // 次のSELECTタグを探すため再帰する。
                this.ProcessSelectCaseDefaultTag();
            }
            else
            {
                // SELECTタグの処理が完了した
            }
        }

        /// <summary>CASE-DEFAULTタグを処理</summary>
        /// <param name="xmlNodeSelectTag">SELECTタグ</param>
        /// <param name="param">指定された文字列</param>
        /// <returns>選択されたCASE-DEFAULTタグのInnerText</returns>
        private string ProcessSelectCaseDefaultTag(XmlNode xmlNodeSelectTag, string param)
        {
            List<XmlNode> lstXmlNodeCase = new List<XmlNode>();
            XmlNode xmlNodeDefault = null;

            // SELECTタグの子ノードにCASE-DEFAULTタグがあるか確認する。
            foreach (XmlNode xmlNode in xmlNodeSelectTag.ChildNodes)
            {
                if (xmlNode.Name == PubLiteral.DPQ_TAG_CASE)// 大文字・小文字は区別する。
                {
                    // CASEタグ
                    lstXmlNodeCase.Add(xmlNode);
                }
                else if (xmlNode.Name == PubLiteral.DPQ_TAG_DEFAULT)// 大文字・小文字は区別する。
                {
                    // DEFAULTタグ
                    xmlNodeDefault = xmlNode;
                }
                else
                {
                    // ここは通らない
                    // （チェック済み）
                }
            }

            // CASEタグ
            if (lstXmlNodeCase.Count > 0)
            {
                // CASEタグあり

                // paramに合致するvalue値を持っているか？

                foreach (XmlNode xmlNodeCase in lstXmlNodeCase)
                {
                    // CASEタグのvalue属性から条件値を取得。
                    // 大文字・小文字は区別する。
                    XmlNode xmlNodeParam = xmlNodeCase.Attributes.GetNamedItem("value");

                    // エラー処理
                    if (xmlNodeParam == null)
                    {
                        // CASEタグにvalue属性が設定されていない。
                        throw new ArgumentException(String.Format(
                            PublicExceptionMessage.DPQ_TAG_VALUE_ATTR_NOT_EXIST, PubLiteral.DPQ_TAG_CASE));
                    }
                    else
                    {
                        // 一致していれば、そのCASEタグのInnerTextを戻す。
                        if (xmlNodeParam.Value == param)
                        {
                            return xmlNodeCase.InnerText;
                        }
                        else
                        {
                            // 次のCASEタグを処理する。
                        }
                    }
                }
            }
            else
            {
                // CASEタグなし
            }

            // DEFAULTタグ
            if (xmlNodeDefault != null)
            {
                // DEFAULTタグあり。

                // DEFAULTタグのInnerTextを戻す。
                return xmlNodeDefault.InnerText;
            }
            else
            {
                // DEFAULTタグなし。
            }

            return "";
        }

        // 2010/09/24---新機能の追加（ここまで）

        #endregion

        #region LISTタグ

        /// <summary>LISTタグを処理する。</summary>
        /// <param name="paramSign">パラメタの先頭記号（DBMSによって可変）</param>
        private void ProcessLISTTag(char paramSign)
        {
            XmlNodeList xmlNodeList = null;
            XmlNode xmlNodeListTag = null;

            // すべてのLISTタグを取得、大文字・小文字は区別する。
            xmlNodeList = this._xml.GetElementsByTagName(PubLiteral.DPQ_TAG_LIST);

            // LISTタグは処理後に削除されるので常に０番を指定
            xmlNodeListTag = xmlNodeList[0];

            if (xmlNodeListTag != null)
            {
                // LISTタグ内のテキスト要素からパラメタ名を取得する。
                // パラメタ名
                string paramName = this.GetParamByText(xmlNodeListTag.InnerText, paramSign);

                // 2008/10/16---タグ編集処理の変更（ここから）

                if (paramName == "")
                {
                    // LISTタグにタグ内パラメタが設定されていない。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.DPQ_TAG_INNER_PARAM_NOT_EXIST, "LIST"));
                }

                // 2008/10/16---タグ編集処理の変更（ここまで）

                // 2010/10/13 - ContainsKeyによるチェック処理を追加した。
                // パラメタを取得
                object obj = null;
                if (this._parameter.ContainsKey(paramName))
                {
                    obj = this._parameter[paramName];
                }

                if (obj == null)
                {
                    // LISTタグ内のパラメタに対応するパラメタが、
                    // 設定されていない。 or nullに設定されている。

                    // 2008/10/16---パラメタの削除処理の追加（ここから）
                    
                    // 事前にパラメタの削除をしておく。
                    this._parameter.Remove(paramName);

                    // 2008/10/16---パラメタの削除処理の追加（ここまで）

                    // LISTタグを削除（半角スペースに変換）する。
                    XmlText xmlText = this._xml.CreateTextNode(" ");
                    xmlNodeListTag.ParentNode.ReplaceChild(xmlText, xmlNodeListTag);
                }
                else
                {
                    // LISTタグ内のパラメタに対応するパラメタが設定されている。

                    // パラメタの型は「ArrayList」型であること。
                    if (obj.GetType() == typeof(ArrayList))
                    {
                        // LISTタグ内のパラメタに対応するパラメタが「ArrayList」型である。

                        ArrayList al = (ArrayList)obj;

                        if (al.Count == 0)
                        {
                            // LISTタグ内のパラメタに対応するパラメタの要素数が０。

                            // LISTタグを削除（半角スペースに変換）する。
                            XmlText xmlText = this._xml.CreateTextNode(" ");
                            xmlNodeListTag.ParentNode.ReplaceChild(xmlText, xmlNodeListTag);
                        }
                        else
                        {
                            // LISTタグ内のパラメタに対応するパラメタの要素数が１以上。

                            // LISTタグ内のパラメタを展開する。

                            // 使用する変数
                            string oldString = paramSign + paramName;
                            string newString = "";
                            int counter = 1;

                            foreach (object dum in al)
                            {
                                // 展開パラメタ
                                newString += oldString + "_" + counter.ToString();

                                if (counter == al.Count)
                                {
                                    // 末端（カンマ区切りを付与しない）
                                }
                                else
                                {
                                    // 中間（カンマ区切りを付与する）
                                    newString += ", ";
                                }

                                // カウンタをインクリメント
                                counter++;
                            }

                            // 展開後のパラメタを設定し、LISTタグを削除（InnerTextで置換する）する。
                            // 2008/10/16---タグ編集処理（半角スペースを追加）
                            XmlText xmlText = this._xml.CreateTextNode(" " + xmlNodeListTag.InnerText.Replace(oldString, newString) + " ");
                            xmlNodeListTag.ParentNode.ReplaceChild(xmlText, xmlNodeListTag);
                        }                        
                    }
                    else
                    {
                        // LISTタグ内のパラメタに対応するパラメタが「ArrayList」型でない。

                        // この場合はパラメタ数１と判断し、パラメタを展開しないでLISTタグを削除（InnerTextで置換する）する。
                        // 2008/10/16---タグ編集処理（半角スペースを追加）
                        XmlText xmlText = this._xml.CreateTextNode(" " + xmlNodeListTag.InnerText + " ");
                        xmlNodeListTag.ParentNode.ReplaceChild(xmlText, xmlNodeListTag);
                    }
                }

                // 次のLISTタグを探すため再帰する。
                this.ProcessLISTTag(paramSign);
            }
            else
            {
                // LISTタグの処理が完了した
            }
        }

        #endregion

        #region JOIN、SUB、WHEREタグ

        /// <summary>
        /// JOIN、SUB、WHEREタグを削除する。
        /// </summary>
        private void ProcessJoinSubWhereTag()
        {
            // 2010/09/24---JOINタグのネスト条件緩和

            // ルートとなるJOIN、SUB、WHEREを取得、大文字・小文字は区別する。
            XmlNodeList xmlNodeList;

            // JOINタグを取得
            xmlNodeList = this._xml.GetElementsByTagName(PubLiteral.DPQ_TAG_JOIN);

            if (xmlNodeList.Count == 0)// 無ければ
            {
                // SUBタグを取得
                xmlNodeList = this._xml.GetElementsByTagName(PubLiteral.DPQ_TAG_SUB);
            }

            if (xmlNodeList.Count == 0)// 無ければ
            {
                // WHEREタグを取得
                xmlNodeList = this._xml.GetElementsByTagName(PubLiteral.DPQ_TAG_WHERE);
            }

            if (xmlNodeList.Count == 0)
            {
                // JOIN、SUB、WHEREタグの削除が完了した
            }
            else
            {
                // JOIN、SUB、WHEREタグは処理後に削除されるので常に０番（先頭）を指定
                XmlNode xmlNodeJoinSubWhereTag = xmlNodeList[0];

                // 末端のJOIN、SUB、WHEREタグを処理（削除）する関数を呼ぶ。
                this.ProcessJoinSubWhereTag2(xmlNodeJoinSubWhereTag);

                // JOIN、SUB、WHEREタグが削除されているので一旦仕切りなおして処理を続行する。
                // これは、XMLの構造が変更された後はforeachに入りなおす必要があるため。
                this.ProcessJoinSubWhereTag();
            }
        }

        /// <summary>
        /// JOIN、SUB、WHEREタグを削除する。
        /// 再帰で末端ノードのJOIN、SUB、WHEREタグから処理する。
        /// </summary>
        /// <param name="xmlNodeJoinSubWhereTag">JOIN、SUB、WHEREタグ</param>
        private void ProcessJoinSubWhereTag2(XmlNode xmlNodeJoinSubWhereTag)
        {
            // 2010/09/24---JOINタグのネスト条件緩和

            // 末端ノード検索
            foreach (XmlNode xmlNode in xmlNodeJoinSubWhereTag.ChildNodes)
            {
                // JOIN、SUB、WHEREタグか、大文字・小文字は区別する。
                if (xmlNode.Name == PubLiteral.DPQ_TAG_JOIN
                    || xmlNode.Name == PubLiteral.DPQ_TAG_SUB
                    || xmlNode.Name == PubLiteral.DPQ_TAG_WHERE)
                {
                    // 子ノードがJOIN、SUB、WHEREタグの場合、再帰（下に辿っていく）。
                    this.ProcessJoinSubWhereTag2(xmlNode);

                    // 子ノードが処理された場合は戻る。
                    return;
                }
                else
                {
                    // 子ノードがJOIN、SUB、WHEREタグでない場合、次の子ノードへ。
                }
            }

            // 最初に末端のJOIN、SUB、WHEREタグに到達した段階で、このコードブロックに到達する。

            if (xmlNodeJoinSubWhereTag.Name == PubLiteral.DPQ_TAG_JOIN) // 大文字・小文字は区別する。
            {
                // JOINタグの処理
                this.ProcessJoinTag(xmlNodeJoinSubWhereTag);
            }
            else if (xmlNodeJoinSubWhereTag.Name == PubLiteral.DPQ_TAG_SUB) // 大文字・小文字は区別する。
            {
                // SUBタグの処理
                this.ProcessSubTag(xmlNodeJoinSubWhereTag);
            }
            else if (xmlNodeJoinSubWhereTag.Name == PubLiteral.DPQ_TAG_WHERE) // 大文字・小文字は区別する。
            {
                // WHEREタグの処理
                this.ProcessWhereTag(xmlNodeJoinSubWhereTag);
            }
            else
            {
                // ココは通らない。
            }
        }

        #region JOINタグ

        /// <summary>JOINタグを処理</summary>
        /// <param name="xmlNodeJoinTag">末端のJOINタグ</param>
        private void ProcessJoinTag(XmlNode xmlNodeJoinTag)
        {
            // 2010/09/24---JOINタグのネスト条件緩和

            // JOINタグのname属性からユーザパラメタ名を取得。
            // 大文字・小文字は区別する。
            XmlNode xmlNodeParam = xmlNodeJoinTag.Attributes.GetNamedItem("name");

            // エラー処理
            if (xmlNodeParam == null)
            {
                // JOINタグにname属性が設定されていない。
                throw new ArgumentException(String.Format(
                    PublicExceptionMessage.DPQ_TAG_NAME_ATTR_NOT_EXIST, PubLiteral.DPQ_TAG_JOIN));
            }
            else
            {
                if (xmlNodeParam.Value == "")
                {
                    // JOINタグにname属性が設定されていない。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.DPQ_TAG_NAME_ATTR_VALUE_IS_EMPTY, PubLiteral.DPQ_TAG_JOIN));
                }
            }

            // パラメタ名は退避しておく（タグが消されることがあるので）
            string paramName = xmlNodeParam.Value;

            // 2010/10/13 - ContainsKeyによるチェック処理を追加した。
            // パラメタを取得
            object obj = null;
            if (this._parameter.ContainsKey(paramName))
            {
                obj = this._parameter[paramName];
            }
            
            // エラー処理
            if (obj == null)
            {
                // JOINタグに対応するパラメタにデータが
                // 設定されていない。 or nullに設定されている。

                // JOINタグを削除（半角スペースに変換）する。
                XmlText xmlText = this._xml.CreateTextNode(" ");
                xmlNodeJoinTag.ParentNode.ReplaceChild(xmlText, xmlNodeJoinTag);
            }
            else
            {
                // 型を確認する。
                if (obj.GetType() == typeof(Boolean))
                {
                    // Boolean型の場合は注意
                    if (((bool)obj))
                    {
                        // trueの場合、残す。

                        // JOINタグを削除（InnerTextで置換する）する。
                        XmlText xmlText = this._xml.CreateTextNode(" " + xmlNodeJoinTag.InnerText + " ");
                        xmlNodeJoinTag.ParentNode.ReplaceChild(xmlText, xmlNodeJoinTag);
                    }
                    else
                    {
                        // falseの場合、（nullでなくても）消す。

                        // JOINタグを削除（半角スペースに変換）する。
                        XmlText xmlText = this._xml.CreateTextNode(" ");
                        xmlNodeJoinTag.ParentNode.ReplaceChild(xmlText, xmlNodeJoinTag);
                    }
                }
                else
                {
                    // nullでなく、Boolean型以外の場合はエラー。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.DPQ_SET_ONLY_NULL_OR_BOOL_TO_INNER_PARAM_VALUE,
                            PubLiteral.DPQ_TAG_JOIN));
                }
            }

            // パラメタを削除する。
            this._parameter.Remove(paramName);
        }

        #endregion

        #region SUBタグ

        /// <summary>SUBタグを処理</summary>
        /// <param name="xmlNodeSubTag">末端のSUBタグ</param>
        private void ProcessSubTag(XmlNode xmlNodeSubTag)
        {
            // 2010/09/24---JOINタグのネスト条件緩和

            // SUBタグのname属性からユーザパラメタ名を取得。
            // 大文字・小文字は区別する。
            XmlNode xmlNodeParam = xmlNodeSubTag.Attributes.GetNamedItem("name");

            // エラー処理
            if (xmlNodeParam == null)
            {
                // SUBタグにname属性が設定されていない。
                throw new ArgumentException(String.Format(
                    PublicExceptionMessage.DPQ_TAG_NAME_ATTR_NOT_EXIST, PubLiteral.DPQ_TAG_SUB));
            }
            else
            {
                if (xmlNodeParam.Value == "")
                {
                    // SUBタグにname属性が設定されていない。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.DPQ_TAG_NAME_ATTR_VALUE_IS_EMPTY, PubLiteral.DPQ_TAG_SUB));
                }
            }

            // パラメタ名は退避しておく（タグが消されることがあるので）
            string paramName = xmlNodeParam.Value;

            // 2010/10/13 - ContainsKeyによるチェック処理を追加した。
            // パラメタを取得
            object obj = null;
            if (this._parameter.ContainsKey(paramName))
            {
                obj = this._parameter[paramName];
            }

            // エラー処理
            if (obj == null)
            {
                // SUBタグに対応するパラメタにデータが
                // 設定されていない。 or nullに設定されている。

                // SUBタグを削除（半角スペースに変換）する。
                XmlText xmlText = this._xml.CreateTextNode(" ");
                xmlNodeSubTag.ParentNode.ReplaceChild(xmlText, xmlNodeSubTag);
            }
            else
            {
                // 型を確認する。
                if (obj.GetType() == typeof(Boolean))
                {
                    // Boolean型の場合は注意
                    if (((bool)obj))
                    {
                        // trueの場合、残す。

                        // SUBタグを削除（InnerTextで置換する）する。
                        XmlText xmlText = this._xml.CreateTextNode(" " + xmlNodeSubTag.InnerText + " ");
                        xmlNodeSubTag.ParentNode.ReplaceChild(xmlText, xmlNodeSubTag);
                    }
                    else
                    {
                        // falseの場合、（nullでなくても）消す。

                        // SUBタグを削除（半角スペースに変換）する。
                        XmlText xmlText = this._xml.CreateTextNode(" ");
                        xmlNodeSubTag.ParentNode.ReplaceChild(xmlText, xmlNodeSubTag);
                    }
                }
                else
                {
                    // nullでなく、Boolean型以外の場合はエラー。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.DPQ_SET_ONLY_NULL_OR_BOOL_TO_INNER_PARAM_VALUE,
                            PubLiteral.DPQ_TAG_SUB));
                }
            }

            // パラメタを削除する。
            this._parameter.Remove(paramName);
        }

        #endregion

        #region WHEREタグ

        /// <summary>WHEREタグを処理</summary>
        /// <param name="xmlNodeWhereTag">末端のWHEREタグ</param>
        private void ProcessWhereTag(XmlNode xmlNodeWhereTag)
        {
            // 2010/09/24---JOINタグのネスト条件緩和

            // 大文字・小文字は区別しない。
            if (xmlNodeWhereTag.InnerText.Trim().ToUpper() == PubLiteral.DPQ_TAG_WHERE)
            {
                // WHEREタグ内が、WHEREのみの場合
                // WHEREタグ削除（半角スペースに変換）する。
                XmlText xmlText = this._xml.CreateTextNode(" ");
                xmlNodeWhereTag.ParentNode.ReplaceChild(xmlText, xmlNodeWhereTag);

                // 戻る
                return;
            }
            else
            {
                // WHEREタグ内が、WHEREのみでない場合

                // 関数化した。
                string where = BaseDam.DeleteFirstLogicalOperatoronWhereClause(xmlNodeWhereTag.InnerText);

                // WHEREタグ削除（処理済テキストに変換）する。
                XmlText xmlText = this._xml.CreateTextNode(" " + where + " ");
                xmlNodeWhereTag.ParentNode.ReplaceChild(xmlText, xmlNodeWhereTag);

                // 戻る
                return;
            }
        }

        /// <summary>Where句の最初の論理演算子を削除する</summary>
        /// <param name="where">処理対象のWhere句</param>
        /// <returns>処理後のWhere句</returns>
        public static string DeleteFirstLogicalOperatoronWhereClause(string where)
        {
            // WHERE句のインデックス位置
            int startIndex = 0;

            // AND、OR演算子のインデックス位置
            int andIndex = 0;
            int orIndex = 0;

            // 処理方法を決めるフラグ
            // ・0：処理しない。
            // ・1：ANDを処理する。
            // ・2：ORを処理する。
            int processFlag = 0;

            #region 処理内容の説明

            // IFタグ処理により、
            // ・WHERE AND xxx , WHERE AND(xxx
            // ・WHERE OR xxx , WHERE OR(xxx
            // となった場合は、
            // ・WHERE xxx
            // ・WHERE (xxx
            // に変換する（余剰なAND , OR を消去する）。

            // アルゴリズムとしては、WHEREと
            // AND（OR）の間の文字列を切り出し、
            // この文字列が
            // ・半角スペース
            // ・\r（\r\n）：キャリッジリターン文字
            // ・\n：ラインフィード文字
            // ・\t：タブ文字
            // のみ（Trim()で == ""）の場合、
            // このAND（OR）を削除する。

            #endregion

            // 最初のWHEREを検索
            int start = where.IndexOf(PubLiteral.DPQ_TAG_WHERE, 0, StringComparison.OrdinalIgnoreCase);

            if (start == -1)
            {
                // 予期せぬエラー（スルーする）
            }
            else
            {
                // インデックス位置を調整
                start += 5;

                // ANDが最初に現れる位置
                andIndex = where.IndexOf("AND", startIndex, StringComparison.OrdinalIgnoreCase);
                // ORが最初に現れる位置
                orIndex = where.IndexOf("OR", startIndex, StringComparison.OrdinalIgnoreCase);

                #region 処理対象を決定する

                // 有効なインデックスを取得
                if (andIndex == -1 && orIndex == -1)
                {
                    // ANDもORも見つからない。

                    // 処理しない
                    processFlag = 0;
                }
                else if (andIndex != -1 && orIndex == -1)
                {
                    // ANDのみ見つかる。

                    // 処理方法を決定
                    if (where.Substring(start, andIndex - start).Trim() == "")
                    {
                        // ANDを処理する。
                        processFlag = 1;
                    }
                    else
                    {
                        // 処理しない
                        processFlag = 0;
                    }
                }
                else if (andIndex == -1 && orIndex != -1)
                {
                    // ORのみ見つかる。

                    // 処理方法を決定
                    if (where.Substring(start, orIndex - start).Trim() == "")
                    {
                        // ORを処理する。
                        processFlag = 2;
                    }
                    else
                    {
                        // 処理しない
                        processFlag = 0;
                    }
                }
                else
                {
                    // 両方見つかった場合

                    // インデックスが若い方で処理
                    if (andIndex < orIndex)
                    {
                        // ANDの方が若い

                        // 処理方法を決定
                        if (where.Substring(start, andIndex - start).Trim() == "")
                        {
                            // ANDを処理する。
                            processFlag = 1;
                        }
                        else
                        {
                            // 処理しない
                            processFlag = 0;
                        }
                    }
                    else if (orIndex < andIndex)
                    {
                        // ORの方が若い

                        // 処理方法を決定
                        if (where.Substring(start, orIndex - start).Trim() == "")
                        {
                            // ORを処理する。
                            processFlag = 2;
                        }
                        else
                        {
                            // 処理しない
                            processFlag = 0;
                        }
                    }
                    else
                    {
                        // ありえん
                    }
                }

                #endregion

                #region 対象を処理する

                // 処理用ワーク変数
                string temp = "";

                if (processFlag == 0)
                {
                    // 処理しない
                }
                else if (processFlag == 1)
                {
                    // ANDを消去
                    temp += where.Substring(0, andIndex);// 0 ～ andIndexまで
                    temp += where.Substring(andIndex + 3);// andIndex + 3 ～ 最後まで

                    // 設定
                    where = temp;
                }
                else if (processFlag == 2)
                {
                    // ORを消去
                    temp += where.Substring(0, orIndex);// 0 ～ orIndexまで
                    temp += where.Substring(orIndex + 2);// orIndex + 2 ～ 最後まで

                    // 設定
                    where = temp;
                }
                else
                {
                    // ありえない
                }

                #endregion
            }

            return where;
        }

        #endregion

        #endregion

        #region DELCMAタグ

        // 2008/12/25---新機能の追加（ここから）

        /// <summary>DELCMAタグを処理する。</summary>
        private void ProcessDELCMATag()
        {
            // すべてのDELCMAタグを取得、大文字・小文字は区別する。
            XmlNodeList xmlNodeList = this._xml.GetElementsByTagName(PubLiteral.DPQ_TAG_DELCMA);

            // DELCMAタグは処理後に削除されるので常に０番を指定
            XmlNode xmlNodeDelCma = xmlNodeList[0];

            if (xmlNodeDelCma != null)
            {
                // InnerTextの前後のカンマ（Comma）を削除する。

                // InnerTextを取得する。
                string temp = xmlNodeDelCma.InnerText;

                // インデントを取得する。
                string indent = GetIndent(temp);

                // 処理するためにTrimする。
                temp = temp.Trim(); 

                // 文字列長が、0以外の場合、
                while (temp.Length != 0)
                {
                    // カンマの有無しフラグ
                    bool existComma = false;

                    // 先頭がカンマ
                    if (temp[0] == ',')
                    {
                        // 先頭のカンマを取り除く。
                        temp = temp.Substring(1, temp.Length - 1).Trim();
                        existComma = true;
                    }

                    // 末端がカンマ
                    if (temp[temp.Length - 1] == ',')
                    {
                        // 末端のカンマを取り除く。
                        temp = temp.Substring(0, temp.Length - 1).Trim();
                        existComma = true;
                    }

                    if (existComma)
                    {
                        // カンマを消しました。処理を継続します。
                    }
                    else
                    {
                        // カンマがありませんでした。処理を終了します。
                        break;
                    }
                }

                // DELCMAタグを削除（カンマ削除後のInnerTextで置換する）する。
                XmlText xmlText = this._xml.CreateTextNode(" " + indent + temp + " ");
                xmlNodeDelCma.ParentNode.ReplaceChild(xmlText, xmlNodeDelCma);
                
                // 次のDELCMAタグを探すため再帰する。
                this.ProcessDELCMATag();
            }
            else
            {
                // DELCMAタグの置換が完了した
            }
        }

        // 2008/12/25---新機能の追加（ここまで）

        #endregion

        #region PARAMタグ

        /// <summary>
        /// PARAMタグを削除する。
        /// </summary>
        private void DeleteParamTag()
        {
            // PARAMタグを取得
            // 大文字・小文字は区別する。
            XmlNodeList xmlNodeList = this._xml.GetElementsByTagName(PubLiteral.DPQ_TAG_PARAM);
            XmlNode xmlNodeParam = xmlNodeList[0];

            if (xmlNodeParam != null)
            {
                // IFタグを削除（半角スペースに変換）する。
                XmlText xmlText = this._xml.CreateTextNode(" ");
                xmlNodeParam.ParentNode.ReplaceChild(xmlText, xmlNodeParam);

                // 次のPARAMタグを探すため再帰する。
                this.DeleteParamTag();
            }
            else
            {
                // PARAMタグの削除が完了した
            }
        }

        #endregion

        #endregion

        #region テキスト内のパラメタ名を取得

        /// <summary>
        /// テキスト内のパラメタ名を取得
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="paramSign">パラメタの先頭記号（DBMSによって可変）</param>
        /// <returns>パラメタ名</returns>
        protected string GetParamByText(string text, char paramSign)
        {
            int paramSignIndex;
            return GetParamByText(text,paramSign,out paramSignIndex);
        }



        /// <summary>
        /// テキスト内のパラメタ名を取得<br/>
        /// 本メソッドで返却するパラメタ名は、パラメタの先頭記号を含まない<br/>
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <param name="paramSign">パラメタの先頭記号（DBMSによって可変）</param>
        /// <param name="outParamSignIndex">パラメタの先頭記号のインデックス。検出されない場合、-1を返却。</param>
        /// <returns>パラメタ名。検出されない場合、先頭記号のみのパラメタ("@"のみ)の場合は""を返却する。</returns>
        protected string GetParamByText(string text, char paramSign,out int outParamSignIndex)
        {
            // シングルクォート

            // 内
            bool isInside = false;

            // パラメタ（先頭記号以降）であるかの判定する。
            bool isParam = false;

            // IFタグ内のパラメタ名を記録する。
            StringBuilder sb = new StringBuilder();

            // ２連続のシングルクォートは解析の邪魔になるので他の文字に置換する。
            // （実行されるＳＱＬを変更するのではないので問題ない）
            while (text.IndexOf("''",StringComparison.Ordinal) != -1)
            {
                text = text.Replace("''", "xx");    //2013/02/15---文字数が変わらないように置換文字を"x"→"xx"に仕様変更
            }

            int tmpParamSignIndex = -1; //パラメタの先頭記号の一致箇所

            // 文字列の解析
            for (int charIndex = 0; charIndex < text.Length; charIndex++)
            {
                char currentChar = text[charIndex];

                if (isParam)
                {
                    // 2008/12/30---新機能の追加（ここから）

                    #region パラメタ 区間の説明

                    // この間で
                    // ・半角スペース
                    // ・,
                    // ・)
                    // ・\r（\r\n）：キャリッジリターン文字
                    // ・\n        ：ラインフィード文字
                    // ・\t        ：タブ文字
                    // になるまでパラメタ名を記録する。

                    //// ','はサポートしない（配列指定を利用するので(@P)の')'をサポートする）
                    // → ','をサポートする（Insert、Updateで使用する際に、','が末尾となることがある。）。

                    // ';'はサポートしない（そのような指定方法はサポートしない）

                    #endregion

                    // パラメタの区間に入った。
                    if (currentChar == ' ' || currentChar == ',' || currentChar == ')' ||
                        currentChar == '\r' || currentChar == '\n' || currentChar == '\t')
                    {
                        // パラメタを取得できたので抜ける。
                        break;
                    }
                    else
                    {
                        // パラメタ名を記録。
                        sb.Append(currentChar);
                    }

                    // 2008/12/30---新機能の追加（ここまで）
                }
                else if (currentChar == '\'')
                {
                    // シングルクォート
                    if(isInside == false)
                    {
                        // 先頭
                        isInside = true;
                    }
                    else if (isInside == true)
                    {
                        // 終端
                        isInside = false;
                    }
                }
                else if (currentChar == paramSign)
                {
                    // パラメタの先頭記号を検出した。
                    if (!isInside)
                    {
                        // シングルクォート外。
                        // パラメタの中に入った。(これを無視して次のパラメタの先頭記号を探しに行くことはない)
                        tmpParamSignIndex = charIndex;  //パラメタの先頭記号の検出位置を記憶
                        isParam = true;
                    }
                    else
                    {
                        // シングルクォート内。
                        // シングルクォート内だったので無視。
                    }
                }
                else
                {
                    // 所見無し。
                }
            }

            // パラメタの検出位置
            outParamSignIndex = tmpParamSignIndex;
            // IFタグ内のパラメタ名
            return sb.ToString();
        }

        #endregion

        #region テキストの先頭インデントを取得

        /// <summary>
        /// テキストの先頭インデントを取得する。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>インデント</returns>
        private string GetIndent(string text)
        {
            // インデント
            StringBuilder sb = new StringBuilder();

            // インデント →　先頭から連続する、以下の文字を指す。
            // ・半角スペース
            // ・\r（\r\n）：キャリッジリターン文字
            // ・\n        ：ラインフィード文字
            // ・\t        ：タブ文字
            foreach (char ch in text)
            {
                if (ch == ' ' || ch == '\r' || ch == '\n' || ch == '\t')
                {
                    // インデントとして追加
                    sb.Append(ch);
                }
                else
                {
                    // インデント終了、ループを抜ける。
                    break;
                }
            }

            // インデントを返す。
            return sb.ToString();
        }

        #endregion

        #region 2WAY=SQL（PARAMタグ・PARAMコメントのパラメタ取得）

        #region ルートのメソッド

        /// <summary>
        /// PARAMタグ・PARAMコメントのパラメタ（文字列表現）
        /// を、パラメタの一覧（DataTable）に変換する。
        /// </summary>
        /// <returns>パラメタの一覧（DataTable）</returns>
        /// <remarks>利用箇所：DPQuery_Toolから利用</remarks>
        public DataTable GetParametersFromPARAMTag()
        {
            // コマンドテキストがロードされていること。
            DataTable dt = new DataTable();

            // パラメタの一覧（DataTable）のカラム

            // ★↓のカラム名は他で使っていないのでリテラル化しない。

            // 区分
            dt.Columns.Add("UserParameter", System.Type.GetType(typeof(Boolean).ToString()));
            // パラメタ名
            dt.Columns.Add("ParameterName", System.Type.GetType(typeof(String).ToString()));
            // パラメタ値
            dt.Columns.Add("Object", System.Type.GetType(typeof(Object).ToString()));

            // 2008/10/16---null・DBNull対応（ここから）
            dt.Columns.Add(PubLiteral.VALUE_STR_NULL, System.Type.GetType(typeof(String).ToString()));
            // 2008/10/16---null・DBNull対応（ここまで）

            if (this._QueryStatus == DbEnum.QueryStatusEnum.DPQ)
            {
                #region 動的パラメタライズド・クエリの場合

                XmlNodeList xmlNodeList = this._xml.GetElementsByTagName(PubLiteral.DPQ_TAG_PARAM);
                XmlNode xmlNodeParam = xmlNodeList.Item(0);

                // パラメタタグが無い場合
                if (xmlNodeParam == null)
                {
                    // 空のdtを返す。
                    return dt;
                }

                string param = "";
                foreach (XmlNode xmlChildNode in xmlNodeParam.ChildNodes)
                {
                    // 「#text」・「#cdata-section」は処理対象とする。
                    // ※「#comment」は処理対象としない。
                    if (xmlChildNode.Name == PubLiteral.DPQ_TAG_TEXT
                        || xmlChildNode.Name == PubLiteral.DPQ_TAG_CDATA)
                    {
                        // パラメタの有無をチェック
                        if (xmlChildNode.Value.Trim() == "")
                        {
                            // パラメタが無い場合
                        }
                        else
                        {
                            // パラメタが有る場合
                            param += xmlChildNode.Value;
                        }
                    }
                    else if(xmlChildNode.Name == PubLiteral.DPQ_TAG_COMMENT)
                    {
                        // これはCOMMENTタグ

                        // 無視する。
                    }
                    else
                    {
                        // これはDIVタグ

                        // パラメタを取得する
                        this.StringToParameter(param, dt);
                        // パラメタをクリア
                        param = "";
                    }
                }

                // 最後の要素。
                if (param != "")
                {
                    // パラメタを取得する
                    this.StringToParameter(param, dt);
                    // パラメタをクリア
                    param = "";
                }

                #endregion

                // パラメタの一覧（DataTable）を返す。
                return dt;
            }
            else if (this._QueryStatus == DbEnum.QueryStatusEnum.SPQ)
            {
                #region 通常のパラメタライズド・クエリの場合

                // 使用する変数
                string sql = this.GetCurrentQuery();
                string param = "";

                // Index値を保持
                int startIndex = 0;
                int endIndex = 0;

                while(true)
                {
                    // 「/*PARAM*」を検索
                    startIndex = sql.IndexOf(PubLiteral.SPQ_PARAM_TAG_START, endIndex);
                    // 「*PARAM*/」を検索
                    endIndex = sql.IndexOf(PubLiteral.SPQ_PARAM_TAG_END, endIndex);

                    // 中間のテキストを抽出
                    if (startIndex == -1 || endIndex == -1)
                    {
                        // パラメタ無しと判断
                        break;
                    }
                    else
                    {
                        // パラメタ有りと判断

                        // 中間のテキストを抽出
                        param = sql.Substring(startIndex + 8, endIndex - (startIndex + 8));

                        // インデックスの更新
                        endIndex += 8;

                        // パラメタの有無をチェック
                        if (param.Trim() == "")
                        {
                            // パラメタが無い場合
                        }
                        else
                        {
                            // パラメタが有る場合

                            // パラメタを取得する
                            this.StringToParameter(param, dt);
                        }
                    }
                }

                #endregion

                // パラメタの一覧（DataTable）を返す。
                return dt;
            }
            else
            {
                // 何も設定しないで
                // パラメタの一覧（DataTable）返す。
                return dt;
            }
        }

        #endregion

        #region １パラメタ分の文字列表現をパラメタに変換

        /// <summary>
        /// １パラメタ分の文字列表現をパラメタに変換し、
        /// パラメタの一覧（DataTable）に追加する。
        /// </summary>
        /// <param name="paramString">１パラメタ分の文字列表現</param>
        /// <param name="dt">パラメタの一覧（DataTable）</param>
        private void StringToParameter(string paramString, DataTable dt)
        {
            // 初期処理
            DataRow dr = dt.NewRow();
            string[] aryString = paramString.Split(',');

            try
            {
                if (aryString.Length == 2)
                {
                    // 要素数が２のときは、ユーザパラメタ

                    // パラメタ区分：ユーザパラメタ
                    dr[0] = true;
                    
                    // ユーザパラメタ名
                    dr[1] = aryString[0].Trim();
                    
                    // ユーザパラメタ値（文字列型）
                    dr[2] = aryString[1].Trim();
                    
                    // 2008/10/16---null・DBNull対応（ここから）
                    // null・DBNull判定
                    dr[3] = "";
                    // 2008/10/16---null・DBNull対応（ここまで）
                }
                else if (aryString.Length == 3)
                {
                    // 要素数が３のときは、通常のパラメタ

                    // パラメタ区分：通常のパラメタ
                    dr[0] = false;
                    // パラメタ名
                    dr[1] = aryString[0].Trim();

                    // 2008/10/16---null・DBNull対応（ここから）
                    object tempObj =this.StringToObject(aryString[1].Trim(), aryString[2].Trim());
                    if (tempObj == null)
                    {
                        // パラメタ値
                        dr[2] = tempObj;
                        // null・DBNull判定
                        dr[3] = PubLiteral.VALUE_STR_NULL; 
                    }
                    else
                    {
                        // パラメタ値
                        dr[2] = tempObj;
                        // null・DBNull判定
                        dr[3] = ""; 
                    }                    
                    // 2008/10/16---null・DBNull対応（ここまで）
                }
                else if (3 < aryString.Length)
                {
                    // 要素数が３以上のときは、
                    // ArrayList or 配列パラメタとする。                    

                    // パラメタ区分：通常のパラメタ
                    dr[0] = false;

                    // パラメタ名
                    dr[1] = aryString[0].Trim();

                    ArrayList al = new ArrayList();

                    // ArrayList or 配列
                    if (aryString[1].Trim().IndexOf("[]") == -1)
                    {
                        // ArrayListの場合

                        for (int i = 2; i < aryString.Length; i++)
                        {
                            al.Add(this.StringToObject(aryString[1].Trim(), aryString[i].Trim()));
                        }

                        // パラメタ値（ArrayList）
                        dr[2] = al;
                    }
                    else
                    {
                        // 配列の場合

                        for (int i = 2; i < aryString.Length; i++)
                        {
                            // 配列の[]を消去
                            string temp = aryString[1].Replace("[]", "").Trim();
                            al.Add(this.StringToObject(temp, aryString[i].Trim()));
                        }

                        // パラメタ値（配列）
                        dr[2] = al.ToArray(al[0].GetType());
                    }

                    // 2008/10/16---null・DBNull対応（ここから）
                    // null・DBNull判定
                    dr[3] = "";
                    // 2008/10/16---null・DBNull対応（ここまで）
                }

                // 値を返す
                dt.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                // エラーメッセージを変更
                throw new ArgumentException(String.Format(
                    PublicExceptionMessage.PARAM_TAG_ERROR, paramString, ex.Message));
            }
        }

        #endregion

        #region 文字列（型情報、値情報）を値に変換

        /// <summary>
        /// 文字列（型情報、値情報）を値に変換
        /// </summary>
        /// <param name="typeString">文字列（型情報）</param>
        /// <param name="valString">文字列（値情報）</param>
        /// <returns>値</returns>
        private object StringToObject(string typeString, string valString)
        {
            #region 型の覚書

            ////１バイト
            // System.Boolean

            //// 整数（符号無し）
            // System.Byte
            // System.UInt16
            // System.UInt32
            // System.UInt64

            //// 整数（符号有り）
            // System.SByte
            // System.Int16
            // System.Int32
            // System.Int64

            //// 数値
            // System.Decimal
            // System.Single
            // System.Double

            //// 文字
            // System.Char
            // System.String

            //// 日付
            // System.DateTime

            //// DBのNULL
            // System.DBNull

            #endregion

            #region データ型毎の処理

            // 2008/10/16---比較処理をToUpperに変更（ここから）
            
            // データ型
            if (typeString.ToUpper() == typeof(Boolean).Name.ToUpper())
            {
                // 文字列を「System.Boolean」に変換
                if (valString.ToUpper() == PubLiteral.VALUE_STR_TRUE)
                {
                    return true;
                }
                else if (valString.ToUpper() == PubLiteral.VALUE_STR_FALSE)
                {
                    return false;
                }
                else
                {
                    // エラー
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.PARAM_TAG_VALUE_ERROR, valString));
                }
            }
            else if (typeString.ToUpper() == typeof(Byte).Name.ToUpper())
            {
                // 文字列を「System.Byte」にParse
                return System.Byte.Parse(valString);
            }
            else if (typeString.ToUpper() == typeof(UInt16).Name.ToUpper())
            {
                // 文字列を「System.UInt16」にParse
                return System.UInt16.Parse(valString);
            }
            else if (typeString.ToUpper() == typeof(UInt32).Name.ToUpper())
            {
                // 文字列を「System.UInt32」にParse
                return System.UInt32.Parse(valString);
            }
            else if (typeString.ToUpper() == typeof(UInt64).Name.ToUpper())
            {
                // 文字列を「System.UInt64」にParse
                return System.UInt64.Parse(valString);
            }
            else if (typeString.ToUpper() == typeof(SByte).Name.ToUpper())
            {
                // 文字列を「System.SByte」にParse
                return System.SByte.Parse(valString);
            }
            else if (typeString.ToUpper() == typeof(Int16).Name.ToUpper())
            {
                // 文字列を「System.Int16」にParse
                return System.Int16.Parse(valString);
            }
            else if (typeString.ToUpper() == typeof(Int32).Name.ToUpper())
            {
                // 文字列を「System.Int32」にParse
                return System.Int32.Parse(valString);
            }
            else if (typeString.ToUpper() == typeof(Int64).Name.ToUpper())
            {
                // 文字列を「System.Int64」にParse
                return System.Int64.Parse(valString);
            }
            else if (typeString.ToUpper() == typeof(Decimal).Name.ToUpper())
            {
                // 文字列を「System.Decimal」にParse
                return System.Decimal.Parse(valString);
            }
            else if (typeString.ToUpper() == typeof(Single).Name.ToUpper())
            {
                // 文字列を「System.Single」にParse
                return System.Single.Parse(valString);
            }
            else if (typeString.ToUpper() == typeof(Double).Name.ToUpper())
            {
                // 文字列を「System.Double」にParse
                return System.Double.Parse(valString);
            }
            else if (typeString.ToUpper() == typeof(Char).Name.ToUpper())
            {
                // 文字列を「System.Char」にParse
                return System.Char.Parse(valString);
            }
            else if (typeString.ToUpper() == typeof(String).Name.ToUpper())
            {
                // 文字列は、そのまま返す。
                return valString;
            }
            else if (typeString.ToUpper() == typeof(DateTime).Name.ToUpper())
            {
                // 文字列を「System.DateTime」にParse
                return System.DateTime.Parse(valString);
            }
            else if (typeString.ToUpper() == typeof(DBNull).Name.ToUpper())
            {
                // valStringは不要、任意の文字列
                return System.DBNull.Value;
            }
            else
            {
                // 型名が不一致
                if (valString.ToUpper() == PubLiteral.VALUE_STR_NULL)
                {
                    // 型名が不一致で、値が文字列でnullの時、nullとする。
                    return null;
                }
                else
                {

                    // 型名が不一致なのでエラーとする。
                    throw new ArgumentException(String.Format(
                        PublicExceptionMessage.PARAM_TAG_TYPE_ERROR, typeString));
                }
            }

            // 2008/10/16---比較処理をToUpperに変更（ここまで）

            #endregion
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region データアクセス制御クラスのユーティリティ機能

        #region テキストデータをキレイにする

        /// <summary>
        /// テキストデータをキレイにする。
        /// 
        /// （１）
        /// 以下の文字を半角空白に変換する。
        /// キャリッジリターン文字とラインフィード文字
        /// '\r\n'
        /// キャリッジリターン文字
        /// '\r'
        /// ラインフィード文字
        /// '\n'
        /// 
        /// （２）
        /// ２文字以上連続する
        /// 半角スペース・タブ（\t）は削除する。
        /// （ただし、文字列中は、詰めない。）
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>処理後のテキスト</returns>
        /// <remarks>派生のDamXXXから利用する。</remarks>
        protected string ClearText(string text)
        {
            // StringBuilderを使用して
            // インナーテキストをキレイにする。
            StringBuilder sb = new StringBuilder();

            // キャリッジリターン文字とラインフィード文字
            // '\r\n'
            // キャリッジリターン文字
            // '\r'
            // ラインフィード文字
            // '\n'
            //// タブ文字
            //// '\t'
            // を取り除く
            text = text.Replace("\r\n", " ");
            text = text.Replace('\r', ' ');
            text = text.Replace('\n', ' ');
            //text = text.Replace('\t', ' ');

            // & → &amp;置換
            text = text.Replace("&", "&amp;");
            // エスケープされているシングルクォートを置換
            text = text.Replace("''", "&SingleQuote2;");

            // 連続した空白は、詰める
            bool isConsecutive = false;

            // 文字列中は、詰めない
            bool isString = false;

            foreach (char ch in text)
            {
                if (ch == '\'')
                {
                    // 出たり入ったり（文字列）。
                    isString = !isString;
                }

                if (ch == ' ')
                {
                    if (isConsecutive && !isString)
                    {
                        // 空白（半角スペース）が連続＆文字列外。
                        // → アペンドしない。
                    }
                    else
                    {
                        // 空白（半角スペース）が初回 or 文字列中。
                        // → アペンドする。
                        sb.Append(ch);

                        // 空白（半角スペース）が連続しているフラグを立てる。
                        isConsecutive = true;
                    }
                }
                else if (ch == '\t')
                {
                    if (isConsecutive && !isString)
                    {
                        // 空白（タブ文字）が連続＆文字列外。
                        // → アペンドしない。
                    }
                    else
                    {
                        // 空白（タブ文字）が初回 or 文字列中。
                        // → アペンドする。
                        sb.Append(ch);

                        // 空白（タブ文字）が連続しているフラグを立てる。
                        isConsecutive = true;
                    }
                }
                else
                {
                    // アペンドする。
                    sb.Append(ch);

                    // 連続した空白が途切れたので、フラグを倒す。
                    isConsecutive = false;
                }
            }

            // 戻し（エスケープされているシングルクォートを置換）。
            text = sb.ToString().Replace("&SingleQuote2;", "''");

            // 戻し（& → &amp;置換）
            text = text.Replace("&amp;", "&");

            // 結果を返す
            return text;
        }

        //↓旧メソッド

        //protected string ClearText(string text)
        //{
        //    // StringBuilderを使用して
        //    // インナーテキストをキレイにする。
        //    StringBuilder sb = new StringBuilder();

        //    // キャリッジリターン文字とラインフィード文字
        //    // '\r\n'
        //    // キャリッジリターン文字
        //    // '\r'
        //    // ラインフィード文字
        //    // '\n'
        //    // タブ文字
        //    // '\t'
        //    // を取り除く
        //    text = text.Replace("\r\n", " ");
        //    text = text.Replace('\r', ' ');
        //    text = text.Replace('\n', ' ');
        //    text = text.Replace('\t', ' ');

        //    // 連続した空白は、詰める
        //    bool IsConsecutive = false;

        //    foreach (char ch in text)
        //    {
        //        if (ch == ' ')
        //        {
        //            if (IsConsecutive == true)
        //            {
        //                // 空白が連続しているのでアペンドしない。
        //            }
        //            else
        //            {
        //                // 空白が初回なのでアペンドする。
        //                sb.Append(ch);

        //                // 空白が連続しているフラグを立てる。
        //                IsConsecutive = true;
        //            }
        //        }
        //        else
        //        {
        //            // アペンドする。
        //            sb.Append(ch);

        //            // 連続した空白が途切れたので、フラグを倒す。
        //            IsConsecutive = false;
        //        }
        //    }

        //    // 結果を返す。
        //    return sb.ToString();
        //}

        #endregion

        #endregion

        #endregion

        #region メソッド（インターフェイス相当）

        #region コネクション

        /// <summary>コネクションの確立</summary>
        /// <param name="connstring">接続文字列</param>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract void ConnectionOpen(string connstring);

        /// <summary>コネクションの切断</summary>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract void ConnectionClose();

        #endregion

        #region トランザクション

        /// <summary>トランザクション開始</summary>
        /// <param name="iso">分離レベル</param>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract void BeginTransaction(DbEnum.IsolationLevelEnum iso);

        /// <summary>トランザクションのコミット</summary>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract void CommitTransaction();

        /// <summary>トランザクションのロールバック</summary>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract void RollbackTransaction();
        
        #endregion

        #region SQLの作成

        #region SetSql

        /// <summary>SQL文を記述したファイルへのパスを設定して、Commandオブジェクトを生成。</summary>
        /// <param name="sqlFilePath">SQL文を記述したファイルへのパス</param>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract void SetSqlByFile(string sqlFilePath);

        /// <summary>SQL文を記述したファイルへのパスとCommandTypeを設定して、Commandオブジェクトを生成。</summary>
        /// <param name="sqlFilePath">SQL文を記述したファイルへのパス</param>
        /// <param name="commandType">コマンドの種類</param>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract void SetSqlByFile(string sqlFilePath, CommandType commandType);

        /// <summary>SQL文を設定して、Commandオブジェクトを生成。</summary>
        /// <param name="commandText">実行するSQL文</param>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract void SetSqlByCommand(string commandText);

        /// <summary>SQL文とCommandTypeを設定して、Commandオブジェクトを生成。</summary>
        /// <param name="commandText">実行するSQL文</param>
        /// <param name="commandType">コマンドの種類</param>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract void SetSqlByCommand(string commandText, CommandType commandType);

        #endregion

        #region SetParameter

        /// <summary>パラメタライズドクエリのパラメタを取得する（Out,RetValパラメタ用）。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <returns>Out,RetValパラメタのバリュー</returns>
        /// <remarks>
        /// 派生のDamXXXでオーバーライドする。
        /// 動的SQLの場合はSQL実行後に利用可能
        /// </remarks>
        public abstract object GetParameter(string parameterName);

        /// <summary>パラメタライズドクエリにパラメタを設定する。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <param name="obj">パラメタの値</param>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract void SetParameter(string parameterName, object obj);

        /// <summary>パラメタライズドクエリにパラメタを設定する。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <param name="obj">パラメタの値</param>
        /// <param name="dbTypeInfo">パラメタの型（データプロバイダ固有）</param>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract void SetParameter(string parameterName, object obj, object dbTypeInfo);

        /// <summary>パラメタライズドクエリにパラメタを設定する。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <param name="obj">パラメタの値</param>
        /// <param name="dbTypeInfo">パラメタの型（HiRDBType）</param>
        /// <param name="size">パラメタのサイズ</param>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract void SetParameter(string parameterName, object obj, object dbTypeInfo, int size);

        /// <summary>パラメタライズドクエリにパラメタを設定する。</summary>
        /// <param name="parameterName">パラメタライズドクエリのパラメタ名</param>
        /// <param name="obj">パラメタの値</param>
        /// <param name="dbTypeInfo">パラメタの型（HiRDBType）</param>
        /// <param name="size">パラメタのサイズ</param>
        /// <param name="paramDirection">パラメタの方向</param>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract void SetParameter(string parameterName, object obj,
            object dbTypeInfo, int size, ParameterDirection paramDirection);

        #endregion

        /// <summary>ユーザパラメタを指定の文字列で置換する。</summary>
        /// <param name="userParameterName">置換対象のユーザパラメタ名</param>
        /// <param name="userParameterValue">置換の文字列</param>
        /// <remarks>
        /// SQLインジェクションされる可能性があるユーザ入力は「userParameterValue」に指定しないこと。
        /// 派生のDamXXXでオーバーライドする。
        /// </remarks>
        public abstract void SetUserParameter(string userParameterName, string userParameterValue);

        #endregion

        #region SQLの実行

        /// <summary>Selectクエリを実行し、データテーブルを返す。</summary>
        /// <param name="dt">データテーブル</param>
        /// <remarks>
        /// DataAdapterのFillを実行する。
        /// 派生のDamXXXでオーバーライドする。
        /// </remarks>
        public abstract void ExecSelectFill_DT(DataTable dt);

        /// <summary>Selectクエリを実行し、データセットを返す。</summary>
        /// <param name="ds">データセット</param>
        /// <remarks>
        /// DataAdapterのFillを実行する。
        /// ※ データプロバイダによっては、サーバカーソルをサポートする。
        /// 派生のDamXXXでオーバーライドする。
        /// </remarks>
        public abstract void ExecSelectFill_DS(DataSet ds);

        /// <summary>Selectクエリを実行し、データリーダを返す。</summary>
        /// <returns>データリーダ</returns>
        /// <remarks>
        /// CommandのExecuteReaderを実行する。
        /// 派生のDamXXXでオーバーライドする。
        /// </remarks>
        public abstract IDataReader ExecSelect_DR();

        /// <summary>Selectクエリを実行し、結果セットの最初の行の最初の列を返す。</summary>
        /// <returns>結果セットの最初の行の最初の列（オブジェクト型） </returns>
        /// <remarks>
        /// CommandのExecuteScalarを実行する。
        /// 派生のDamXXXでオーバーライドする。
        /// </remarks>
        public abstract object ExecSelectScalar();

        /// <summary>Insert、Update、Deleteクエリを実行し、影響を受けた行数を返す。</summary>
        /// <returns>影響を受けた行数</returns>
        /// <remarks>
        /// CommandのExecuteNonQueryを実行する。
        /// 派生のDamXXXでオーバーライドする。
        /// </remarks>
        public abstract int ExecInsUpDel_NonQuery();

        /// <summary>静的SQLを生成する</summary>
        /// <param name="sqlUtil">SQLUtility</param>
        /// <returns>SQL文</returns>
        /// <remarks>
        /// Commandでの実行はしない。
        /// 派生のDamXXXでオーバーライドする。
        /// </remarks>
        public abstract string ExecGenerateSQL(SQLUtility sqlUtil);

        #endregion

        #region その他

        #region SQLの取得メソッド

        /// <summary>
        /// 現在コマンドオブジェクトに設定されているSQLを取得する。
        /// </summary>
        /// <returns>現在コマンドオブジェクトに設定されているSQL</returns>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract string GetCurrentQuery();

        /// <summary>
        /// 現在コマンドオブジェクトに設定されているSQLを取得する（ログ出力用）。
        /// </summary>
        /// <returns>現在コマンドオブジェクトに設定されているSQL（ログ出力用）</returns>
        /// <remarks>派生のDamXXXでオーバーライドする。</remarks>
        public abstract string GetCurrentQueryForLog();

        #endregion

        #endregion

        #endregion

        #endregion
    }
}

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
//* クラス名        ：TransactionControl
//*
//* クラス日本語名  ：トランザクション制御クラス
//*
//*                   本来、インナークラスとしたかったが、
//*                   ベースクラスが複数あるため、不可能であった。
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/03/13  西野  大介        新規作成
//*  2009/03/29  西野  大介        定義が無い場合、間違っている場合の処理を追加
//*  2009/04/21  西野  大介        FrameworkExceptionの追加に伴い、実装変更
//*  2009/06/02  西野  大介        sln - IR版からの修正
//*                                ・#14 ： XMLチェック処理追加
//*                                ・#15 ： XML要素のリテラル化
//*                                ・#23 ： ncではconnkeyなし可だがNullReference発生
//*  2010/10/29  西野  大介        RichClientフレームワーク分割によりアクセス修飾子を変更
//*  2011/05/18  西野  大介        埋め込まれたリソース対応（Azure対応）
//**********************************************************************************

// System
using System;
using System.Xml;
using System.Data;
using System.Collections;

// 業務フレームワーク（循環参照になるため、参照しない）

// フレームワーク
using Touryo.Infrastructure.Framework.Business;
using Touryo.Infrastructure.Framework.Common;
using Touryo.Infrastructure.Framework.Dao;
using Touryo.Infrastructure.Framework.Exceptions;
using Touryo.Infrastructure.Framework.Presentation;
using Touryo.Infrastructure.Framework.Util;
using Touryo.Infrastructure.Framework.Transmission;

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Business
{
    /// <summary>トランザクション制御クラス</summary>
    public class TransactionControl
    {
        #region インスタンス変数

        /// <summary>トランザクション定義</summary>
        private XmlDocument XMLTCD = new XmlDocument();

        #endregion

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        public TransactionControl()
        {
            // トランザクション定義をロードする。

            // リソース ローダでチェック（ここで落とすとハンドルされないので落とさない。）
            if (EmbeddedResourceLoader.Exists(
                GetConfigParameter.GetConfigValue(FxLiteral.XML_TC_DEFINITION), false))
            {
                // トランザクション定義（XmlDocument）のロード
                this.XMLTCD.LoadXml(
                    EmbeddedResourceLoader.LoadXMLAsString(
                        GetConfigParameter.GetConfigValue(FxLiteral.XML_TC_DEFINITION)));
            }
            else if (ResourceLoader.Exists(
                GetConfigParameter.GetConfigValue(FxLiteral.XML_TC_DEFINITION), false))
            {
                // トランザクション定義（XmlDocument）のロード
                this.XMLTCD.Load(
                    PubCmnFunction.BuiltStringIntoEnvironmentVariable(
                        GetConfigParameter.GetConfigValue(FxLiteral.XML_TC_DEFINITION)));
            }
            else
            {
                // チェック
                if (GetConfigParameter.GetConfigValue(FxLiteral.XML_TC_DEFINITION) == null
                        || GetConfigParameter.GetConfigValue(FxLiteral.XML_TC_DEFINITION) == "")
                {
                    // 定義が無い（offの扱い）。

                    // トランザクション定義（XmlDocument）を空で初期化
                    this.XMLTCD.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><TCD></TCD>");
                }
                else
                {
                    // 定義が間違っている（エラー）。

                    // エラーをスロー
                    throw new FrameworkException(
                        FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH2[0],
                        String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH2[1],
                                        FxLiteral.XML_TC_DEFINITION));
                }
            }            
        }

        #endregion

        // #14,15,23-start

        #region メソッド

        /// <summary>データアクセス制御クラス（DAM）を初期化する</summary>
        /// <param name="TransactionPatternID">トランザクション パターンID</param>
        /// <param name="dam">データアクセス制御クラス（DAM）インスタンス</param>
        public void InitDam(string TransactionPatternID, BaseDam dam)
        {
            // トランザクション制御情報を取得
            string connectionString;
            DbEnum.IsolationLevelEnum isolevel;

            // 接続文字列, 分離レベルを取得
            this.GetTCInfo(TransactionPatternID, out connectionString, out isolevel);

            if (isolevel == DbEnum.IsolationLevelEnum.NotConnect)
            {
                // コネクションを接続しない。
            }
            else
            {
                // コネクションを初期化する。
                dam.ConnectionOpen(connectionString);

                if (isolevel == DbEnum.IsolationLevelEnum.NoTransaction)
                {
                    // トランザクションを開始しない。
                }
                else
                {
                    // トランザクションを開始する。
                    dam.BeginTransaction(isolevel);
                }
            }
        }

        /// <summary>トランザクション制御情報を取得する。</summary>
        /// <param name="TransactionPatternID">トランザクション パターンID</param>
        /// <param name="connectionString">接続文字列（out）</param>
        /// <param name="isolevel">分離レベル（out）</param>
        private void GetTCInfo(string TransactionPatternID,
            out string connectionString, out DbEnum.IsolationLevelEnum isolevel)
        {
            connectionString = "";
            isolevel = DbEnum.IsolationLevelEnum.NotConnect;

            // 属性チェック用
            XmlNode xmlNode = null;

            // TransactionPatternタグを取得する。 
            XmlElement xmlElement = this.XMLTCD.GetElementById(TransactionPatternID);

            if (xmlElement == null)
            {
                // TransactionPatternタグがない場合

                // 例外を発生させる。
                throw new FrameworkException(
                    FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR[0],
                    String.Format(FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR[1],
                        String.Format(FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR_tp, TransactionPatternID)));
            }
            else
            {
                // TransactionPatternタグがある場合

                // connkey属性
                xmlNode = xmlElement.Attributes[FxLiteral.XML_TX_ATTR_CONNKEY];

                if (xmlNode == null)
                {
                    // connkey属性なしの場合
                }
                else
                {
                    // connkey属性ありの場合
                    connectionString = GetConfigParameter.GetConnectionString(xmlNode.Value);
                }

                // isolevel属性
                xmlNode = xmlElement.Attributes[FxLiteral.XML_TX_ATTR_ISOLEVEL];

                if (xmlNode == null)
                {
                    // isolevel属性なしの場合

                    // 例外を発生させる。
                    throw new FrameworkException(
                    FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR[0],
                    String.Format(FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR[1],
                        String.Format(FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR_iso1, TransactionPatternID)));
                }
                else
                {
                    // isolevel属性ありの場合

                    // 分離level
                    string isolevelString = xmlNode.Value;

                    switch (isolevelString.ToUpper())
                    {
                        case FxLiteral.ISO_LEVEL_NOT_CONNECT:
                            isolevel = DbEnum.IsolationLevelEnum.NotConnect;
                            break;
                        case FxLiteral.ISO_LEVEL_NO_TRANSACTION:
                            isolevel = DbEnum.IsolationLevelEnum.NoTransaction;
                            break;
                        case FxLiteral.ISO_LEVEL_READ_UNCOMMITTED:
                            isolevel = DbEnum.IsolationLevelEnum.ReadUncommitted;
                            break;
                        case FxLiteral.ISO_LEVEL_READ_COMMIT:
                            isolevel = DbEnum.IsolationLevelEnum.ReadCommitted;
                            break;
                        case FxLiteral.ISO_LEVEL_REPEATABLE_READ:
                            isolevel = DbEnum.IsolationLevelEnum.RepeatableRead;
                            break;
                        case FxLiteral.ISO_LEVEL_SERIALIZABLE:
                            isolevel = DbEnum.IsolationLevelEnum.Serializable;
                            break;
                        case FxLiteral.ISO_LEVEL_SNAPSHOT:
                            isolevel = DbEnum.IsolationLevelEnum.Snapshot;
                            break;
                        case FxLiteral.ISO_LEVEL_DEFAULT:
                            isolevel = DbEnum.IsolationLevelEnum.DefaultTransaction;
                            break;
                        default:

                            // 定義（分離level）が間違っている。

                            // 例外を発生させる。
                            throw new FrameworkException(
                                FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR[0],
                                String.Format(FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR[1],
                                    String.Format(FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR_iso2,
                                        isolevelString, TransactionPatternID)));
                    }
                }
            }
        }

        /// <summary>トランザクション グループIDからトランザクション パターンIDを取得する。</summary>
        /// <param name="TransactionGroupID">トランザクション グループID</param>
        /// <param name="TransactionPatternID">トランザクション パターンID（配列）</param>
        public void GetTransactionPatterns(string TransactionGroupID, out string[] TransactionPatternID)
        {
            // 属性チェック用
            XmlNode xmlNode = null;

            // TransactionGroupタグを取得する。
            XmlElement xmlElement = this.XMLTCD.GetElementById(TransactionGroupID);

            if (xmlElement == null)
            {
                // TransactionGroupタグがない場合

                // 定義（トランザクション グループID）が存在しない
                // →　トランザクション制御定義（XML）書式エラー

                // 例外を発生させる。
                throw new FrameworkException(
                    FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR[0],
                    String.Format(FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR[1],
                        String.Format(FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR_tg, TransactionGroupID)));
            }
            else
            {
                // TransactionGroupタグがある場合

                // value属性
                xmlNode = xmlElement.Attributes[FxLiteral.XML_CMN_ATTR_VALUE];

                if (xmlNode == null)
                {
                    // value属性なしの場合

                    // 例外を発生させる。
                    throw new FrameworkException(
                        FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR[0],
                        String.Format(FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR[1],
                            String.Format(FrameworkExceptionMessage.TRANSACTION_CONTROL_XML_FORMAT_ERROR_tgval, TransactionGroupID)));
                }
                else
                {
                    // value属性ありの場合

                    // トランザクション パターンIDの配列
                    TransactionPatternID = xmlNode.Value.Split(',');
                }
            }
        }

        #endregion

        // #14,15,23-end
    }
}

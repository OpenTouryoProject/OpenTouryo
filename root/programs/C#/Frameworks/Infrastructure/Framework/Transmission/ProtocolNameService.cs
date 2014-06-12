//**********************************************************************************
//* Copyright (C) 2007,2014 Hitachi Solutions,Ltd.
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
//* クラス名        ：ProtocolNameService
//* クラス日本語名  ：呼び出しプロトコルの名前解決クラス
//*                   論理名称からＵＲＬ（など）を取得
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/04/02  西野  大介        新規作成
//*  2009/04/15  西野  大介        設計変更により２つに分割
//*  2009/04/20  西野  大介        埋め込まれたリソースXMLファイルのロード メソッドの変更
//*  2009/04/21  西野  大介        FrameworkExceptionの追加に伴い、実装変更
//*  2009/06/02  西野  大介        sln - IR版からの修正
//*                                ・#14 ： XMLチェック処理追加
//*                                ・#15 ： XML要素のリテラル化
//*  2010/09/24  西野  大介        ジェネリック対応（Dictionary、List、Queue、Stack<T>）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2011/01/14  西野  大介        GetPropsFromPropStringをPubCmnFunctionに移動
//*  2011/01/19  西野  大介        環境変数の組み込み処理に対応
//**********************************************************************************

using System.Reflection;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;

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

namespace Touryo.Infrastructure.Framework.Transmission
{
    /// <summary>呼び出しプロトコルの名前解決クラス</summary>
    internal class ProtocolNameService
    {
        /// <summary>呼び出しプロトコルの名前解決定義</summary>
        private XmlDocument XMLTMD_Protocol = new XmlDocument();

        #region コンストラクタ

        /// <summary>コンストラクタ</summary>
        internal ProtocolNameService()
        {
            // 呼び出しプロトコルの名前解決定義をロードする。

            #region  埋め込まれたリソース ローダでチェック（ここで落とすとハンドルされないので落とさない。）

            if (EmbeddedResourceLoader.Exists(
                GetConfigParameter.GetConfigValue(FxLiteral.XML_TM_PROTOCOL_DEFINITION), false))
            {
                // 呼び出しプロトコルの名前解決定義（XmlDocument）を[埋め込まれたリソース]で初期化
                this.XMLTMD_Protocol.LoadXml(EmbeddedResourceLoader.LoadXMLAsString(
                    GetConfigParameter.GetConfigValue(FxLiteral.XML_TM_PROTOCOL_DEFINITION)));

                // 戻す
                return;
            }
            else
            {
                // 何もしない。
            }

            #endregion

            #region リソース ローダでチェック（ここで落とすとハンドルされないので落とさない。）

            if (ResourceLoader.Exists(
                GetConfigParameter.GetConfigValue(FxLiteral.XML_TM_PROTOCOL_DEFINITION), false))
            {
                // 呼び出しプロトコルの名前解決定義（XmlDocument）を[リソース]で初期化
                this.XMLTMD_Protocol.Load(
                    PubCmnFunction.BuiltStringIntoEnvironmentVariable(
                        GetConfigParameter.GetConfigValue(FxLiteral.XML_TM_PROTOCOL_DEFINITION)));

                // 戻す
                return;
            }
            else
            {
                // 何もしない。
            }

            #endregion

            #region チェック（定義の有無や、定義の誤り）

            if (GetConfigParameter.GetConfigValue(FxLiteral.XML_TM_PROTOCOL_DEFINITION) == null
                    || GetConfigParameter.GetConfigValue(FxLiteral.XML_TM_PROTOCOL_DEFINITION) == "")
            {
                // 定義が無い（offの扱い）。

                // 呼び出しプロトコルの名前解決定義（XmlDocument）を空で初期化
                this.XMLTMD_Protocol.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><TMD></TMD>");
            }
            else
            {
                // 定義が間違っている（エラー）。

                // 例外をスロー
                throw new FrameworkException(
                    FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH2[0],
                    String.Format(FrameworkExceptionMessage.ERROR_IN_WRITING_OF_FX_PATH2[1],
                                    FxLiteral.XML_TM_PROTOCOL_DEFINITION));
            }

            #endregion
        }

        #endregion

        // #14,15-start

        #region 名前解決（プロトコル）

        /// <summary>名前解決（プロトコル）</summary>
        /// <param name="name">名前</param>
        /// <param name="protocol">プロトコル名</param>
        internal void NameResolutionProtocolType(string name, out string protocol)
        {
            // 初期化
            protocol = "";

            // 属性チェック用
            XmlNode xmlNode = null;

            // Transmissionタグを取得
            XmlElement xmlTransmission = this.XMLTMD_Protocol.GetElementById(name);

            // チェック
            if (xmlTransmission == null)
            {
                // Transmissionタグがない場合

                // 例外をスロー
                throw new FrameworkException(
                    FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[0],
                    String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[1],
                        String.Format(FrameworkExceptionMessage.NAMESERVICE_XML_FORMAT_ERROR_tm, name)));   
            }
            else
            {
                // Transmissionタグがある場合

                // プロトコル
                xmlNode = xmlTransmission.Attributes[FxLiteral.XML_TM_PROTOCOL_ATTR_PROTOCOL];

                if (xmlNode == null)
                {
                    // protocol属性なしの場合

                    // 例外をスロー
                    throw new FrameworkException(
                        FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[0],
                        System.String.Format(
                            FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[1],
                            System.String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_prt1, name)));
                }
                else
                {
                    // protocol属性ありの場合
                    protocol = xmlNode.Value;
                }                
            }
        }

        #endregion

        #region 名前解決（ＵＲＬ）

        /// <summary>名前解決（ＵＲＬ）</summary>
        /// <param name="name">名前</param>
        /// <param name="url">ＵＲＬ</param>
        /// <param name="timeout">タイムアウト</param>
        /// <param name="props">プロパティ</param>
        internal void NameResolutionProtocolUrl(string name, out string url, out int timeout, out Dictionary<string, string> props)
        {
            // 初期化
            url = "";
            timeout = -1;
            props = new Dictionary<string, string>();

            // 属性チェック用
            XmlNode xmlNode = null;

            // Transmissionタグを取得
            XmlElement xmlTransmission = this.XMLTMD_Protocol.GetElementById(name);

            // チェック
            if (xmlTransmission == null)
            {
                // Transmissionタグがない場合

                // 例外をスロー
                throw new FrameworkException(
                    FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[0],
                    String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[1],
                        String.Format(FrameworkExceptionMessage.NAMESERVICE_XML_FORMAT_ERROR_tm, name)));   
            }
            else
            {
                // Transmissionタグがある場合

                #region ＵＲＬ

                // url属性
                xmlNode = xmlTransmission.Attributes[FxLiteral.XML_TM_PROTOCOL_ATTR_URL];

                if (xmlNode == null)
                {
                    // url属性なしの場合

                    // url_ref属性
                    xmlNode = xmlTransmission.Attributes[FxLiteral.XML_TM_PROTOCOL_ATTR_URL_REF];

                    if (xmlNode == null)
                    {
                        // url_ref属性なしの場合

                        // 例外をスロー
                        throw new FrameworkException(
                            FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[0],
                            System.String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[1],
                                System.String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_url1, name)));
                    }
                    else
                    {
                        // url_ref属性ありの場合
                        string url_ref = xmlNode.Value;

                        if (this.XMLTMD_Protocol.GetElementById(url_ref) == null)
                        {
                            // IDFERからUrlタグを発見できない場合

                            // 例外をスロー
                            throw new FrameworkException(
                            FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[0],
                            System.String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[1],
                                System.String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_url2, url_ref)));
                        }
                        else
                        {
                            // IDFERからUrlタグを発見できた場合

                            // value属性
                            xmlNode = this.XMLTMD_Protocol.GetElementById(url_ref).Attributes[FxLiteral.XML_CMN_ATTR_VALUE];

                            if (xmlNode == null)
                            {
                                // value属性なしの場合

                                // 例外をスロー
                                throw new FrameworkException(
                                    FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[0],
                                    System.String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[1],
                                        System.String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_url3, url_ref)));
                            }
                            else
                            {
                                // value属性ありの場合

                                // urlを取得
                                url = xmlNode.Value;

                                if (url == "")
                                {
                                    // urlが空の場合

                                    // 例外をスロー
                                    throw new FrameworkException(
                                        FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[0],
                                        System.String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[1],
                                            System.String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_url4,
                                                FxLiteral.XML_TM_PROTOCOL_TAG_URL, url_ref)));
                                }
                            }
                        }
                    }
                    
                }
                else
                {
                    // url属性ありの場合

                    // urlを取得
                    url = xmlNode.Value;

                    if (url == "")
                    {
                        // urlが空の場合

                        // 例外をスロー
                        throw new FrameworkException(
                            FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[0],
                            System.String.Format(
                                FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[1],
                                System.String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_url4,
                                    FxLiteral.XML_TM_TAG_TRANSMISSION, name)));
                    }
                }

                #endregion

                #region タイムアウト

                // timeout属性
                xmlNode = xmlTransmission.Attributes[FxLiteral.XML_TM_PROTOCOL_ATTR_TIMEOUT];

                if (xmlNode == null)
                {
                    // timeout属性なしの場合　→　なにもしない。
                }
                else
                {
                    // timeout属性ありの場合
                    string timeoutString = xmlNode.Value;

                    // チェック（数値か）
                    if (int.TryParse(timeoutString, out timeout))
                    {
                        // timeout = int.Parse(timeoutString);
                    }
                    else
                    {
                        // timeout値のフォーマットエラー

                        // 例外をスロー
                        throw new FrameworkException(
                            FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[0],
                            String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[1],
                                String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_to, name)));
                    }
                }

                #endregion

                #region プロパティ

                // prop_ref属性
                xmlNode = xmlTransmission.Attributes[FxLiteral.XML_TM_PROTOCOL_ATTR_PROP_REF];

                if (xmlNode == null)
                {
                    // prop_ref属性なしの場合　→　なにもしない。
                }
                else
                {
                    // prop_ref属性ありの場合
                    string prop_ref = xmlNode.Value;

                    if (this.XMLTMD_Protocol.GetElementById(prop_ref) == null)
                    {
                        // IDFERからPropタグを発見できない場合

                        // 例外をスロー
                        throw new FrameworkException(
                            FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[0],
                            String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[1],
                                String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_prop1, prop_ref)));
                    }
                    else
                    {
                        // IDFERからPropタグを発見できた場合

                        // value属性
                        xmlNode = this.XMLTMD_Protocol.GetElementById(prop_ref).Attributes[FxLiteral.XML_CMN_ATTR_VALUE];

                        // プロパティ文字列を取得
                        if (xmlNode == null)
                        {
                            // value属性なしの場合

                            // 例外をスロー
                            throw new FrameworkException(
                            FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[0],
                            String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR[1],
                                String.Format(FrameworkExceptionMessage.PRT_NAMESERVICE_XML_FORMAT_ERROR_prop2, prop_ref)));
                        }
                        else
                        {
                            // value属性ありの場合

                            // プロパティ文字列をディクショナリに変換
                            string propStr = xmlNode.Value;
                            props = PubCmnFunction.GetPropsFromPropString(propStr);
                        }
                    }
                }

                #endregion
            }
        }

        #endregion

        // #14,15-end
    }
}

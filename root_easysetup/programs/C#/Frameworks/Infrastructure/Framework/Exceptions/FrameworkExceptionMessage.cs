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
//* クラス名        ：FrameworkExceptionMessage
//* クラス日本語名  ：フレームワーク層の例外のメッセージＩＤ、メッセージに使用する
//*                   文字列定数を定義する定数クラス（フレームワーク用）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2009/04/21  西野  大介        FrameworkExceptionの追加に伴い、名称変更
//*  2009/06/02  西野  大介        sln - IR版からの修正
//*                                ・#7, 8, 9 ： 「エラーメッセージ：」
//*                                ・#14 ： XMLチェック処理追加
//*                                ・#15 ： XML要素のリテラル化
//*  2009/07/21  西野  大介        コントロール取得処理の仕様変更
//*  2009/07/21  西野  大介        マスタ ページのネストに対応
//*  2009/07/31  西野  大介        セッション情報の自動削除機能を追加
//*  2009/07/31  西野  大介        不正操作の検出機能を追加
//*  2010/09/24  西野  大介        ジェネリック対応（XMLのDictionary化）
//*                                nullチェック方法、Contains → ContainsKeyなどに注意
//*  2010/10/29  西野  大介        RichClientフレームワークの分割によりアクセス修飾子を変更
//*  2010/11/15  西野  大介        RichClientフレームワーク（非同期呼び出し）のメッセージを追加
//*  2010/12/21  西野  大介        RichClientフレームワーク（非同期イベント）のメッセージを追加
//*  2011/01/14  西野  大介        GetPropsFromPropStringをPubCmnFunctionに移動
//*  2011/10/09  西野  大介        国際化対応
//*  2013/12/23  西野  大介        アクセス修飾子をすべてpublicに変更した。
//*  2014/01/18  Rituparna.Biswas  国際化対応の見直し。
//*  2014/01/22  Rituparna.Biswas  Changes from ConfigurationManager.AppSettings to GetConfigParameter.GetConfigValue in CmnFunc
//*  2014/02/03  西野  大介        取り込み：リソースファイル名とスイッチ名の変更、#pragma warning disableの追加。
//**********************************************************************************

// System
using System;
using System.Threading;

// フレームワーク
using Touryo.Infrastructure.Framework.Util;
using System.Resources;
using System.Globalization;
using System.Configuration;
using Touryo.Infrastructure.Framework.Resources;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Framework.Exceptions
{
    /// <summary>
    /// フレームワーク層の例外のメッセージＩＤ、メッセージに使用する
    /// 文字列定数を定義する定数クラス（フレームワーク用）
    /// </summary>
    public class FrameworkExceptionMessage
    {
        #region 定義不正

        #region メッセージ定義ファイルの不正

        /// <summary>メッセージ定義ファイルの不正</summary>
        public static string[] MESSAGE_XML_FORMAT_ERROR
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                //Returns the specified string resource for the specified culture or current UI culture.
                return new string[] { "MessageXMLFormatError", temp };
            }
        }

        /// <summary>メッセージ定義ファイルの不正（メッセージ補足）</summary>
        public static string MESSAGE_XML_FORMAT_ERROR_ATTR
        {
            get
            {

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return FrameworkExceptionMessage.CmnFunc(key);
            }
        }

        #endregion

        #region 共有情報定義ファイルの不正

        /// <summary>共有情報定義ファイルの不正</summary>
        public static string[] SHAREDPROPERTY_XML_FORMAT_ERROR
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource for the specified culture or current UI culture.
                return new string[] { "SharedPropertyXMLFormatError", temp };
            }
        }

        /// <summary>共有情報定義ファイルの不正（メッセージ補足）</summary>
        public static string SHAREDPROPERTY_XML_FORMAT_ERROR_ATTR
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return FrameworkExceptionMessage.CmnFunc(key);
            }
        }

        #endregion

        #region 画面遷移定義ファイルの不正

        /// <summary>画面遷移定義ファイルの不正</summary>
        public static string[] SCREEN_CONTROL_XML_FORMAT_ERROR
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource for the specified culture or current UI culture.
                return new string[] { "ScreenControlXMLFormatError", temp };
            }
        }

        /// <summary>画面遷移定義ファイルの不正（メッセージ補足）</summary>
        public static string SCREEN_CONTROL_XML_FORMAT_ERROR_value
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_CMN_ATTR_VALUE in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_CMN_ATTR_VALUE", FxLiteral.XML_CMN_ATTR_VALUE);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>画面遷移定義ファイルの不正（メッセージ補足）</summary>
        public static string SCREEN_CONTROL_XML_FORMAT_ERROR_label
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_SC_ATTR_LABEL in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_SC_ATTR_LABEL", FxLiteral.XML_SC_ATTR_LABEL);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>画面遷移定義ファイルの不正（メッセージ補足）</summary>
        public static string SCREEN_CONTROL_XML_FORMAT_ERROR_dl1
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_SC_TAG_SCREEN in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_SC_TAG_SCREEN", FxLiteral.XML_SC_TAG_SCREEN);

                //Replacing FxLiteral.XML_SC_ATTR_DIRECTLINK in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_SC_ATTR_DIRECTLINK", FxLiteral.XML_SC_ATTR_DIRECTLINK);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>画面遷移定義ファイルの不正（メッセージ補足）</summary>
        public static string SCREEN_CONTROL_XML_FORMAT_ERROR_dl2
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_SC_ATTR_DIRECTLINK in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_SC_ATTR_DIRECTLINK", FxLiteral.XML_SC_ATTR_DIRECTLINK);

                //Replacing FxLiteral.XML_SC_TAG_SCREEN in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_SC_TAG_SCREEN", FxLiteral.XML_SC_TAG_SCREEN);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>画面遷移定義ファイルの不正（メッセージ補足）</summary>
        public static string SCREEN_CONTROL_XML_FORMAT_ERROR_mode
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_SC_ATTR_MODE in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_SC_ATTR_MODE", FxLiteral.XML_SC_ATTR_MODE);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        #endregion

        #region トランザクション制御定義ファイルの不正

        /// <summary>トランザクション制御定義ファイルの不正</summary>
        public static string[] TRANSACTION_CONTROL_XML_FORMAT_ERROR
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource for the specified culture or current UI culture.
                return new string[] { "TransactionControlXMLFormatError", temp };
            }
        }

        /// <summary>トランザクション制御定義ファイルの不正（メッセージ補足）</summary>
        public static string TRANSACTION_CONTROL_XML_FORMAT_ERROR_tp
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TX_TAG_TRANSACTION_PATTERN in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TX_TAG_TRANSACTION_PATTERN", FxLiteral.XML_TX_TAG_TRANSACTION_PATTERN);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>トランザクション制御定義ファイルの不正（メッセージ補足）</summary>
        public static string TRANSACTION_CONTROL_XML_FORMAT_ERROR_iso1
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TX_ATTR_ISOLEVEL in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TX_ATTR_ISOLEVEL", FxLiteral.XML_TX_ATTR_ISOLEVEL);

                //Replacing FxLiteral.XML_TX_TAG_TRANSACTION_PATTERN in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TX_TAG_TRANSACTION_PATTERN", FxLiteral.XML_TX_TAG_TRANSACTION_PATTERN);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>トランザクション制御定義ファイルの不正（メッセージ補足）</summary>
        public static string TRANSACTION_CONTROL_XML_FORMAT_ERROR_iso2
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TX_ATTR_ISOLEVEL in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TX_ATTR_ISOLEVEL", FxLiteral.XML_TX_ATTR_ISOLEVEL);

                //Replacing FxLiteral.XML_TX_TAG_TRANSACTION_PATTERN in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TX_TAG_TRANSACTION_PATTERN", FxLiteral.XML_TX_TAG_TRANSACTION_PATTERN);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>トランザクション制御定義ファイルの不正（メッセージ補足）</summary>
        public static string TRANSACTION_CONTROL_XML_FORMAT_ERROR_tg
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TX_TAG_TRANSACTION_GROUP in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TX_TAG_TRANSACTION_GROUP", FxLiteral.XML_TX_TAG_TRANSACTION_GROUP);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>トランザクション制御定義ファイルの不正（メッセージ補足）</summary>
        public static string TRANSACTION_CONTROL_XML_FORMAT_ERROR_tgval
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_CMN_ATTR_VALUE in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_CMN_ATTR_VALUE", FxLiteral.XML_CMN_ATTR_VALUE);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                temp = temp.Replace("FxLiteral.XML_TX_TAG_TRANSACTION_GROUP", FxLiteral.XML_TX_TAG_TRANSACTION_GROUP);
                return temp;
            }
        }

        #endregion

        #region 名前解決定義の不正

        #region インプロセス呼び出し

        /// <summary>インプロセス呼び出しの名前解決定義ファイルの不正</summary>
        public static string[] IPR_NAMESERVICE_XML_FORMAT_ERROR
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource for the specified culture or current UI culture.
                return new string[] { "InProcessNameServiceXMLFormatError", temp };
            }
        }

        /// <summary>インプロセス呼び出しの名前解決定義ファイルの不正（メッセージ補足）</summary>
        public static string IPR_NAMESERVICE_XML_FORMAT_ERROR_asmName1
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TM_INPROCESS_ATTR_ASSEMBLYNAMEE in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_INPROCESS_ATTR_ASSEMBLYNAME", FxLiteral.XML_TM_INPROCESS_ATTR_ASSEMBLYNAME);

                //Replacing FxLiteral.XML_TM_TAG_TRANSMISSION in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_TAG_TRANSMISSION", FxLiteral.XML_TM_TAG_TRANSMISSION);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>インプロセス呼び出しの名前解決定義ファイルの不正（メッセージ補足）</summary>
        public static string IPR_NAMESERVICE_XML_FORMAT_ERROR_asmName2
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TM_INPROCESS_ATTR_ASSEMBLYNAME in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_INPROCESS_ATTR_ASSEMBLYNAME", FxLiteral.XML_TM_INPROCESS_ATTR_ASSEMBLYNAME);

                //Replacing FxLiteral.XML_TM_TAG_TRANSMISSION in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_TAG_TRANSMISSION", FxLiteral.XML_TM_TAG_TRANSMISSION);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>インプロセス呼び出しの名前解決定義ファイルの不正（メッセージ補足）</summary>
        public static string IPR_NAMESERVICE_XML_FORMAT_ERROR_clsName1
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TM_INPROCESS_ATTR_CLASSNAME in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_INPROCESS_ATTR_CLASSNAME", FxLiteral.XML_TM_INPROCESS_ATTR_CLASSNAME);

                //Replacing FxLiteral.XML_TM_TAG_TRANSMISSION in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_TAG_TRANSMISSION", FxLiteral.XML_TM_TAG_TRANSMISSION);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>インプロセス呼び出しの名前解決定義ファイルの不正（メッセージ補足）</summary>
        public static string IPR_NAMESERVICE_XML_FORMAT_ERROR_clsName2
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TM_INPROCESS_ATTR_CLASSNAME in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_INPROCESS_ATTR_CLASSNAME", FxLiteral.XML_TM_INPROCESS_ATTR_CLASSNAME);

                //Replacing FxLiteral.XML_TM_TAG_TRANSMISSION in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_TAG_TRANSMISSION", FxLiteral.XML_TM_TAG_TRANSMISSION);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        #endregion

        #region 呼び出しプロトコル

        /// <summary>呼び出しプロトコルの名前解決定義ファイルの不正</summary>
        public static string[] PRT_NAMESERVICE_XML_FORMAT_ERROR
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource for the specified culture or current UI culture.
                return new string[] { "ProtocolNameServiceXMLFormatError", temp };
            }
        }

        /// <summary>呼び出しプロトコルの名前解決定義ファイルの不正（メッセージ補足）</summary>
        public static string PRT_NAMESERVICE_XML_FORMAT_ERROR_prt1
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TM_PROTOCOL_ATTR_PROTOCOL in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_PROTOCOL_ATTR_PROTOCOL", FxLiteral.XML_TM_PROTOCOL_ATTR_PROTOCOL);

                //Replacing FxLiteral.XML_TM_TAG_TRANSMISSION in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_TAG_TRANSMISSION", FxLiteral.XML_TM_TAG_TRANSMISSION);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>呼び出しプロトコルの名前解決定義ファイルの不正（メッセージ補足）</summary>
        public static string PRT_NAMESERVICE_XML_FORMAT_ERROR_prt2
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TM_PROTOCOL_ATTR_PROTOCOL in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_PROTOCOL_ATTR_PROTOCOL", FxLiteral.XML_TM_PROTOCOL_ATTR_PROTOCOL);

                //Replacing FxLiteral.XML_TM_TAG_TRANSMISSION in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_TAG_TRANSMISSION", FxLiteral.XML_TM_TAG_TRANSMISSION);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>呼び出しプロトコルの名前解決定義ファイルの不正（メッセージ補足）</summary>
        public static string PRT_NAMESERVICE_XML_FORMAT_ERROR_url1
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TM_PROTOCOL_ATTR_URL_REF in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_PROTOCOL_ATTR_URL_REF", FxLiteral.XML_TM_PROTOCOL_ATTR_URL_REF);

                //Replacing FxLiteral.XML_TM_PROTOCOL_ATTR_URL in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_PROTOCOL_ATTR_URL", FxLiteral.XML_TM_PROTOCOL_ATTR_URL);

                //Replacing FxLiteral.XML_TM_TAG_TRANSMISSION in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_TAG_TRANSMISSION", FxLiteral.XML_TM_TAG_TRANSMISSION);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>呼び出しプロトコルの名前解決定義ファイルの不正（メッセージ補足）</summary>
        public static string PRT_NAMESERVICE_XML_FORMAT_ERROR_url2
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TM_PROTOCOL_TAG_URL in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_PROTOCOL_TAG_URL", FxLiteral.XML_TM_PROTOCOL_TAG_URL);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>呼び出しプロトコルの名前解決定義ファイルの不正（メッセージ補足）</summary>
        public static string PRT_NAMESERVICE_XML_FORMAT_ERROR_url3
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);


                //Replacing FxLiteral.XML_CMN_ATTR_VALUE in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_CMN_ATTR_VALUE", FxLiteral.XML_CMN_ATTR_VALUE);

                //Replacing FxLiteral.XML_TM_PROTOCOL_TAG_URL in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_PROTOCOL_TAG_URL", FxLiteral.XML_TM_PROTOCOL_TAG_URL);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>呼び出しプロトコルの名前解決定義ファイルの不正（メッセージ補足）</summary>
        public static string PRT_NAMESERVICE_XML_FORMAT_ERROR_url4
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return FrameworkExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>呼び出しプロトコルの名前解決定義ファイルの不正（メッセージ補足）</summary>
        public static string PRT_NAMESERVICE_XML_FORMAT_ERROR_to
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TM_PROTOCOL_ATTR_TIMEOUT in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_PROTOCOL_ATTR_TIMEOUT", FxLiteral.XML_TM_PROTOCOL_ATTR_TIMEOUT);

                //Replacing FxLiteral.XML_TM_TAG_TRANSMISSION in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_TAG_TRANSMISSION", FxLiteral.XML_TM_TAG_TRANSMISSION);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>呼び出しプロトコルの名前解決定義ファイルの不正（メッセージ補足）</summary>
        public static string PRT_NAMESERVICE_XML_FORMAT_ERROR_prop1
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TM_PROTOCOL_TAG_PROP in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_PROTOCOL_TAG_PROP", FxLiteral.XML_TM_PROTOCOL_TAG_PROP);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        /// <summary>呼び出しプロトコルの名前解決定義ファイルの不正（メッセージ補足）</summary>
        public static string PRT_NAMESERVICE_XML_FORMAT_ERROR_prop2
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_CMN_ATTR_VALUE in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_CMN_ATTR_VALUE", FxLiteral.XML_CMN_ATTR_VALUE);

                //Replacing FxLiteral.XML_TM_PROTOCOL_TAG_PROP in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_PROTOCOL_TAG_PROP", FxLiteral.XML_TM_PROTOCOL_TAG_PROP);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        #endregion

        /// <summary>名前解決定義の不正（メッセージ補足）</summary>
        public static string NAMESERVICE_XML_FORMAT_ERROR_tm
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                //Replacing FxLiteral.XML_TM_TAG_TRANSMISSION in Resource File with Original value
                temp = temp.Replace("FxLiteral.XML_TM_TAG_TRANSMISSION", FxLiteral.XML_TM_TAG_TRANSMISSION);

                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return temp;
            }
        }

        #endregion

        #endregion

        #region 共通

        #region 初回起動時チェックエラー

        /// <summary>フレームワークで必要なHIDDENタグが実装されていない場合。</summary>
        public static string[] NO_FX_HIDDEN
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource for the specified culture or current UI culture.
                return new string[] { "NoFxHidden", temp };
            }
        }

        /// <summary>フレームワークの数値情報が正しく設定されていない場合。</summary>/*34/
        public static string[] ERROR_IN_WRITING_OF_FX_NUMVAL
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource for the specified culture or current UI culture.
                return new string[] { "ErrorInWritingOfFxNumVal", temp };
            }
        }

        /// <summary>フレームワークのパス情報が設定されていない場合。</summary>
        public static string[] ERROR_IN_WRITING_OF_FX_PATH1
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                return new string[] { "ErrorInWritingOfFxPath1", temp };
            }
        }

        /// <summary>フレームワークのパス情報が間違っている場合。</summary>
        public static string[] ERROR_IN_WRITING_OF_FX_PATH2
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource for the specified culture or current UI culture.
                return new string[] { "ErrorInWritingOfFxPath2", temp };
            }
        }

        /// <summary>フレームワークのスイッチが正しく設定されていない場合（on or off）。</summary>
        public static string[] ERROR_IN_WRITING_OF_FX_SWITCH1
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource for the specified culture or current UI culture.
                return new string[] { "ErrorInWritingOfFxSwitch1", temp };
            }
        }

        /// <summary>フレームワークのスイッチが正しく設定されていない場合（true or false）。</summary>
        public static string[] ERROR_IN_WRITING_OF_FX_SWITCH2
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource for the specified culture or current UI culture.
                return new string[] { "ErrorInWritingOfFxSwitch2", temp };
            }
        }

        #endregion

        #region パラメタのチェックエラー

        #region 汎用

        /// <summary>パラメタのチェックエラー</summary>
        public static string[] PARAMETER_CHECK_ERROR
        {
            get
            {
                string temp = "";
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource for the specified culture or current UI culture.
                return new string[] { "ParameterCheckError", temp };
            }
        }

        /// <summary>パラメタのチェックエラー（メッセージ補足１）</summary>
        public static string PARAMETER_CHECK_ERROR_null
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return FrameworkExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>パラメタのチェックエラー（メッセージ補足２）</summary>
        public static string PARAMETER_CHECK_ERROR_empty
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return FrameworkExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>パラメタのチェックエラー（メッセージ補足３）</summary>
        public static string PARAMETER_CHECK_ERROR_length
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return FrameworkExceptionMessage.CmnFunc(key);
            }
        }

        #endregion

        #region Rich

        /// <summary>非同期呼び出しフレームワークの引数エラー</summary>
        public static string[] ASYNC_FUNC_CHECK_ERROR
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource for the specified culture or current UI culture.
                return new string[] { "AsyncFunc", temp };
            }
        }

        /// <summary>非同期呼び出しフレームワークの利用API不整合</summary>
        public static string[] ASYNC_MSGBOX_ERROR
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                // Returns the specified string resource with corrected value for the specified culture or current UI culture.
                return new string[] { "AsyncMsgBox", temp };
            }
        }

        /// <summary>非同期イベント フレームワークの引数エラー（Control）</summary>
        public static string[] ASYNC_EVENT_ENTRY_CHECK_ERROR
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "AsyncEventEntry", temp };
            }
        }

        /// <summary>非同期イベント フレームワークの引数エラー（Control）</summary>
        public static string[] ASYNC_EVENT_ENTRY_CONTROL_CHECK_ERROR
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "AsyncEventEntry_Control", temp };
            }
        }

        /// <summary>非同期イベント フレームワークの引数エラー（Callback）</summary>
        public static string[] ASYNC_EVENT_ENTRY_CALLBACK_CHECK_ERROR
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "AsyncEventEntry_Callback", temp };
            }
        }

        #endregion

        #endregion

        #endregion

        #region Ｐ層

        #region 機能

        /// <summary>コントロール取得処理（型不一致）</summary>
        public static string[] CONTROL_TYPE_ERROR
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "ControlTypeError", temp };
            }
        }

        /// <summary>コントロール取得処理（重複 - Web）</summary>
        public static string[] CONTROL_REPETITION_ERROR1
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "ControlRepetitionError", temp };
            }
        }

        /// <summary>コントロール取得処理（重複 - Windows）</summary>
        public static string[] CONTROL_REPETITION_ERROR2
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "ControlRepetitionError", temp };
            }
        }

        #region Web

        /// <summary>コントロール取得処理</summary>
        public static string[] NO_MASTER_PAGE
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "NoMasterPage", temp };
            }
        }

        /// <summary>コントロール取得処理</summary>
        public static string[] MASTER_PAGE_TYPE_ERROR
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "MasterPageTypeError", temp };
            }
        }

        /// <summary>セッションタイムアウト</summary>
        public static string[] SESSION_TIMEOUT
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "SessionTimeOut", temp };
            }
        }

        /// <summary>不正操作チェック</summary>
        public static string[] ILLEGAL_OPERATION_CHECK_ERROR
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "IllegalOperationCheckError", temp };
            }
        }

        /// <summary>画面遷移チェック</summary>
        public static string[] SCREEN_CONTROL_CHECK_ERROR
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "ScreenControlCheckError", temp };
            }
        }

        /// <summary>不正な状態（メッセージ補足１）</summary>
        public static string SCREEN_CONTROL_CHECK_ERROR_get
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return FrameworkExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>不正な状態（メッセージ補足２）</summary>
        public static string SCREEN_CONTROL_CHECK_ERROR_naked
        {
            get
            {

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return FrameworkExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>不正な状態（メッセージ補足３）</summary>
        public static string SCREEN_CONTROL_CHECK_ERROR_nolbl
        {
            get
            {

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return FrameworkExceptionMessage.CmnFunc(key);
            }
        }

        #endregion

        #region Rich

        #endregion

        #endregion

        #region 不正な状態

        /// <summary>フレームワーク処理で不正な状態を検出した場合。</summary>
        public static string[] FX_PROCESSING_STATUS_ERROR
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);
                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "ProcessingStatusError", temp };
            }
        }

        /// <summary>不正な状態（諸事情によりボタン履歴を存在しない）</summary>
        public static string FX_PROCESSING_STATUS_ERROR_NO_BH_QUEUE
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return FrameworkExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>ダイアログ表示後の後処理で不正な状態を検出した場合。</summary>
        public static string[] DIALOG_AFTER_PROCESSING_STATUS_ERROR
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "DialogAfterProcessingStatusError", temp };
            }
        }

        /// <summary>ダイアログを閉じる処理で不正な状態を検出した場合。</summary>
        public static string[] DIALOG_CLOSING_STATUS_ERROR
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "DialogClosingStatusError", temp };
            }
        }

        /// <summary>不正な状態（メッセージ補足１）</summary>
        public static string DIALOG_CLOSING_STATUS_ERROR1
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return FrameworkExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>不正な状態（メッセージ補足２）</summary>
        public static string DIALOG_CLOSING_STATUS_ERROR2
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return FrameworkExceptionMessage.CmnFunc(key);
            }
        }

        #endregion

        #endregion

        #region Ｂ層

        /// <summary>Ｂ層呼び出しチェック</summary>
        public static string[] LB_ILLEGAL_CALL_CHECK_ERROR
        {
            get
            {
                string temp = "";

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Stores the specified string resource for the specified culture or current UI culture.
                temp = FrameworkExceptionMessage.CmnFunc(key);

                // Returns the specified string resource  for the specified culture or current UI culture.
                return new string[] { "LayerBIllegalCallCheckError", temp };
            }
        }

        #endregion

        #region CmnFunc
        /// <summary>Returns the specified string resource for the specified culture or current UI culture. </summary> 
        /// <param name="key">resource key</param> 
        /// <returns>resource string</returns>
        private static string CmnFunc(string key)
        {
            // We acquire ResourceManager.
            ResourceManager rm = FrameworkExceptionMessageResource.ResourceManager;

            // We acquire a value from App.Config.
            string FxUICulture = GetConfigParameter.GetConfigValue(PubLiteral.EXCEPTIONMESSAGECULTUER); 

            if (string.IsNullOrEmpty(FxUICulture))
            {
                // When the key is not set to App.Config, we use a default culture. 
                return rm.GetString(key);
            }
            else
            {
                // When the key is set to App.Config, we use the specified culture.
                try
                {
                    CultureInfo culture = new CultureInfo(FxUICulture);
                    return rm.GetString(key, culture);
                }
#pragma warning disable
                catch (Exception ex) // There is not CultureNotFoundException in .NET3.5.
                {
#pragma warning restore
                    // When the specified culture is not an effective name, we use a default culture.
                    return rm.GetString(key);
                }
            }
        }
        #endregion

    }
}

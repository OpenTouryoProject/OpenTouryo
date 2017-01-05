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
//* クラス名        ：PublicExceptionMessage
//* クラス日本語名  ：Public層の例外メッセージに使用する文字列定数を定義する定数クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2009/04/21  西野  大介        新規作成
//*  2009/06/02  西野  大介        sln - IR版からの修正
//*                                ・#11 ： レイトバインド（assemblyが存在しない場合）
//*                                ・#x  ： CommandTimeOutデフォルト値を設定
//*  2010/09/24  西野  大介        SELECT-CASE-DEFAULTタグの追加
//*  2010/09/24  西野  大介        DB Lib 別プロジェクト化対応
//*  2010/11/20  西野  大介        オーバーロード メソッド対応
//*  2010/12/03  西野  大介        log4netの埋め込まれたリソース対応（RichClient）
//*  2011/01/14  西野  大介        GetPropsFromPropStringをPubCmnFunctionに移動
//*  2011/10/09  西野  大介        国際化対応
//*  2013/02/15  加藤  幸紀        順番バインド置き換え時のパラメタ設定不足用のメッセージ追加
//*                                （順番バインドのパラメタ置換処理方式の見直し）
//*  2013/07/03  西野  大介        NotImplementedExceptionの場合のメッセージを追加
//*  2013/12/23  西野  大介        アクセス修飾子をすべてpublicに変更した。
//*  2014/01/17  Rituparna.Biswas  国際化対応の見直し。
//*  2014/01/22  Rituparna.Biswas  Changes from ConfigurationManager.AppSettings to GetConfigParameter.GetConfigValue in CmnFunc
//*  2014/02/03  西野  大介        取り込み：リソースファイル名とスイッチ名の変更、#pragma warning disableの追加。
//*  2016/05/30  Supragyan        Added a message in the case of NotSupportedException
//**********************************************************************************

// System
using System;
using System.Threading;
using System.Resources;
using Touryo.Infrastructure.Public.Resources;
using System.Configuration;
using System.Globalization;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>Public層の例外メッセージに使用する文字列定数を定義する定数クラス</summary>
    public class PublicExceptionMessage
    {
        #region app.configエラー

        /// <summary>コンフィグに指定がありません。</summary>
        public static string NO_CONFIG
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>スイッチの指定が不正です。</summary>
        public static string SWITCH_ERROR
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>CommandTimeoutの指定が不正です。</summary>
        public static string COMMANDTIMEOUT_ERROR
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the concatination of specified string resource for the specified culture or current UI culture and PubLiteral.SQL_COMMANDTIMEOUT. 
                return PublicExceptionMessage.CmnFunc(key) + PubLiteral.SQL_COMMANDTIMEOUT;
            }
        }

        #endregion

        #region リソース ファイル

        /// <summary>リソース ファイルがみつからない</summary>
        public static string RESOURCE_FILE_NOT_FOUND
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>XML宣言が不正です</summary>
        public static string XML_DECLARATION_ERROR
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>XMLエレメントが不正です</summary>
        public static string XML_ELEMENT_ERROR
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>XMLエレメントが不正です（log4net）</summary>
        public static string XML_ELEMENT_ERROR_LOG4NET
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        #endregion

        #region 圧縮解凍

        /// <summary>解凍パスワードの指定なし</summary>
        public static string ZIP_PASSWORD
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        #endregion

        #region レイトバインド

        /// <summary>レイトバインドでエラー（０）</summary>
        public static string LATEBIND_ERROR0
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>レイトバインドでエラー（１）</summary>
        public static string LATEBIND_ERROR1
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>レイトバインドでエラー（２）</summary>
        public static string LATEBIND_ERROR2
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>レイトバインドでエラー（３）</summary>
        public static string LATEBIND_ERROR3
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        #endregion

        #region ＤＢ部品

        #region 分離レベル

        /// <summary>無効な分離レベル[ReadUncommitted]</summary>
        public static string DB_ISO_LEVEL_PARAM_ERROR_UC
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>無効な分離レベル[RepeatableRead]</summary>
        public static string DB_ISO_LEVEL_PARAM_ERROR_RR
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>無効な分離レベル[Snapshot]</summary>
        public static string DB_ISO_LEVEL_PARAM_ERROR_SS
        {
            get
            {

                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>無効なパラメタ[User]</summary>
        public static string DB_ISO_LEVEL_PARAM_ERROR_USR
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>無効なパラメタ[NotConnect]</summary>
        public static string DB_ISO_LEVEL_PARAM_ERROR_NC
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        #endregion

        #region タグ

        /// <summary>不正なタグのエラー</summary>
        public static string THIS_DPQ_TAG_IS_UNKNOWN
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        #region タグのフォーマットエラー

        /// <summary>タグのフォーマットエラー</summary>
        public static string DPQ_TAG_FORMAT_ERROR
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>タグのフォーマットエラー（name属性が設定されていません（null））</summary>
        public static string DPQ_TAG_NAME_ATTR_NOT_EXIST
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>タグのフォーマットエラー（name属性が設定されていません（空文字列））</summary>
        public static string DPQ_TAG_NAME_ATTR_VALUE_IS_EMPTY
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>タグのフォーマットエラー（value属性が設定されていません（null））</summary>
        public static string DPQ_TAG_VALUE_ATTR_NOT_EXIST
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>タグのフォーマットエラー（タグ内パラメタが設定されていません）</summary>
        public static string DPQ_TAG_INNER_PARAM_NOT_EXIST
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>PARAMタグの型変換エラー</summary>
        public static string PARAM_TAG_ERROR
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>PARAMタグの型変換エラー（型不正）</summary>
        public static string PARAM_TAG_TYPE_ERROR
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>PARAMタグの型変換エラー（値不正）</summary>
        public static string PARAM_TAG_VALUE_ERROR
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        #endregion

        #region タグのパラメタ設定エラー

        /// <summary>ｘｘタグのフォーマットエラー（ｘｘタグのタグ内パラメタはnull値 or Boolean値のみ）</summary>
        public static string DPQ_SET_ONLY_NULL_OR_BOOL_TO_INNER_PARAM_VALUE
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>IFタグのフォーマットエラー（テキストパラメタがnullであるのに、ELSEタグが見つからない）</summary>
        public static string DPQ_ELSE_TAG_DOESNT_EXIST_WHEN_TEXT_PARAM_OF_IF_TAG_IS_NULL
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>IFタグのフォーマットエラー（タグ内パラメタがnullであるのに、ELSEタグが見つからない）</summary>
        public static string DPQ_ELSE_TAG_DOESNT_EXIST_WHEN_INNER_PARAM_OF_IF_TAG_IS_NULL
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>IFタグのフォーマットエラー（タグ内パラメタがfalseであるのに、ELSEタグが見つからない）</summary>
        public static string DPQ_ELSE_TAG_DOESNT_EXIST_WHEN_INNER_PARAM_OF_IF_TAG_IS_FALSE
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        #endregion

        #endregion

        #region 順番バインド

        /// <summary>順番バインド置き換え時のパラメタ設定不足</summary>
        public static string ORDER_BIND_ERROR_PARAMETER_NOT_FOUND
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        #endregion //順番バインド

        #endregion

        #region DTO

        // SLのほうから参照できないのでSL側を英語化

        #endregion

        #region プロパティ文字列

        /// <summary>プロパティ文字列のフォーマット不正</summary>
        public static string PROP_STRING_FORMAT_ERROR
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>プロパティ文字列のフォーマット不正（開始文字不正）</summary>
        public static string PROP_STRING_FORMAT_ERROR_START_CHARACTER
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>プロパティ文字列のフォーマット不正（エスケープ文字不正）</summary>
        public static string PROP_STRING_FORMAT_ERROR_ESCAPE_CHARACTER
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>プロパティ文字列のフォーマット不正（中括弧不正）</summary>
        public static string PROP_STRING_FORMAT_ERROR_CURLY_BRACE
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>プロパティ文字列のフォーマット不正（プロパティ名が空）</summary>
        public static string PROP_STRING_FORMAT_ERROR_PROPERTY_NAME_IS_EMPTY
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>プロパティ文字列のフォーマット不正（プロパティ値が空）</summary>
        public static string PROP_STRING_FORMAT_ERROR_PROPERTY_VALUE_IS_EMPTY
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>プロパティ文字列のフォーマット不正（プロパティ名の区切り文字不正）</summary>
        public static string PROP_STRING_FORMAT_ERROR_DELIMITER_OF_PROPERTY_NAME
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>プロパティ文字列のフォーマット不正（プロパティ値の区切り文字不正）</summary>
        public static string PROP_STRING_FORMAT_ERROR_DELIMITER_OF_PROPERTY_VALUE
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        #endregion

        #region 汎用メッセージ

        /// <summary>引数不正</summary>
        public static string ARGUMENT_INJUSTICE
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>パラメタがnullの場合</summary>
        public static string PARAM_IS_NULL
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>NotImplementedExceptionの場合</summary>
        public static string NOT_IMPLEMENTED
        {
            get
            {
                // Get current property name.
                //string key = PubCmnFunction.GetCurrentMethodName();
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>NotSupportedExceptionの場合</summary>
        public static string NOT_SUPPORTED
        {
            get
            {
                // Get current property name.
                string key = PubCmnFunction.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return PublicExceptionMessage.CmnFunc(key);
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
            ResourceManager rm = PublicExceptionMessageResource.ResourceManager;

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

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
//* クラス名        ：MyBusinessSystemExceptionMessage
//* クラス日本語名  ：システム例外のメッセージＩＤ、メッセージに使用する
//*                   文字列定数を定義する定数クラス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ             新規作成（テンプレート）
//*  2011/10/09  西野 大介             国際化対応
//*  2013/12/23  西野 大介             アクセス修飾子をすべてpublicに変更した。
//*  2014/01/17  Pradeepa.Shanmugham   Code for Internalization
//*  2014/01/22  Pradeepa.Shanmugham   Changes from ConfigurationManager.AppSettings to GetConfigParameter.GetConfigValue in CmnFunc
//*  2014/02/03  西野 大介             取り込み：リソースファイル名とスイッチ名の変更、#pragma warning disableの追加。
//**********************************************************************************

using System;
using System.Resources;
using System.Globalization;

using Touryo.Infrastructure.Business.Resources;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Diagnostics;

namespace Touryo.Infrastructure.Business.Exceptions
{
    /// <summary>
    /// Business層の
    /// システム例外のメッセージＩＤ、メッセージに
    /// 使用する文字列定数を定義する定数クラス
    /// </summary>
    public class MyBusinessSystemExceptionMessage
    {
        #region SAMPLE_ERROR method

        /// <summary>システム例外のメッセージＩＤ、メッセージに使用する文字列定数（例）</summary>
        public static string[] SAMPLE_ERROR
        {
            get
            {
                string temp = "";
                // Get current property name.
                string key = StackFrameOperator.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                temp = MyBusinessSystemExceptionMessage.CmnFunc(key);
                return new string[] { "MessageID_SampleError", temp };
            }
        }

        #endregion

        #region CMN_DAO_ERROR method

        /// <summary>共通Daoのエラー</summary>
        public static string[] CMN_DAO_ERROR
        {
            get
            {
                string temp = "";
                // Get current property name.
                string key = StackFrameOperator.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                temp = MyBusinessSystemExceptionMessage.CmnFunc(key);
                return new string[] { "CommonDaoError", temp };
            }
        }

        #region CMN_DAO_ERROR_SQL method

        /// <summary>メッセージ定義ファイルの不正（メッセージ補足）</summary>
        public static string CMN_DAO_ERROR_SQL
        {
            get
            {
                // Get current property name.
                string key = StackFrameOperator.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return MyBusinessSystemExceptionMessage.CmnFunc(key);
            }
        }

        #endregion

        #endregion

        #region WORKFLOW_ERROR method

        /// <summary>Workflowのエラー</summary>
        public static string[] WORKFLOW_ERROR
        {
            get
            {
                string temp = "";
                // Get current property name.
                string key = StackFrameOperator.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                temp = MyBusinessSystemExceptionMessage.CmnFunc(key);
                return new string[] { "WorkflowError", temp };
            }
        }

        #region WORKFLOW_ERROR_CHECK_ method

        /// <summary>メッセージ定義ファイルの不正（メッセージ補足）</summary>
        public static string WORKFLOW_ERROR_CHECK_EMPTY
        {
            get
            {
                // Get current property name.
                string key = StackFrameOperator.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return MyBusinessSystemExceptionMessage.CmnFunc(key);
            }
        }

        /// <summary>メッセージ定義ファイルの不正（メッセージ補足）</summary>
        public static string WORKFLOW_ERROR_CHECK_FIELD_ISNT_CONTAINED
        {
            get
            {
                // Get current property name.
                string key = StackFrameOperator.GetCurrentPropertyName();

                // Returns the specified string resource for the specified culture or current UI culture.
                return MyBusinessSystemExceptionMessage.CmnFunc(key);
            }
        }
        
        #endregion

        #endregion

        #region CmnFunc method
        /// <summary>Returns the specified string resource for the specified culture or current UI culture. </summary>
        /// <param name="key">resource key</param>
        /// <returns>resource string</returns>
        private static string CmnFunc(string key)
        {
            // We acquire ResourceManager.
            ResourceManager rm = MyBusinessSystemExceptionMessageResource.ResourceManager;

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

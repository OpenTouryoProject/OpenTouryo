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
//* クラス名        ：LogIF
//* クラス日本語名  ：ログ出力を行うクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野 大介         新規作成
//*  2008/09/19  西野 大介         WebLogManagerクラスの設計不良に対応したIF変更に対応
//*  2009/01/28  西野 大介         クラス名の変更（WebLog → LogIF）
//*  2012/03/16  西野 大介         ログ・レベル情報取得インターフェイスの追加
//*  2025/06/15  西野 大介         Log_log4net、Log_nlogを選択的に使用する実装に変更
//*  2025/06/15  西野 大介         GetLoggerLogLevel、GetRootLoggerLogLevelの削除
//**********************************************************************************

using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.Log
{
    /// <summary>ログ出力を行うクラス</summary>
    /// <remarks>自由に利用できる。</remarks>
    public class LogIF
    {
        /// <summary>ログ</summary>
        private static BaseLog baseLog;

        /// <summary>静的コンストラクタ</summary>
        static LogIF()
        {
            // LogLib
            string logLib = GetConfigParameter.GetConfigValue("LogLib");

            if (string.IsNullOrEmpty(logLib))
            {
                baseLog = new Log_log4net();
            }
            else
            {
                baseLog = new Log_log4net();

                if (logLib.ToLower() == "nlog")
                {
                    baseLog = new Log_nlog();
                }
            }   
        }

        #region ログ出力

        /// <summary>DEBUGログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>自由に利用できる。</remarks>
        public static void DebugLog(string loggerName, string message)
        {
            LogIF.baseLog.DebugLog(loggerName, message);
        }

        /// <summary>INFORMATIONログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>自由に利用できる。</remarks>
        public static void InfoLog(string loggerName, string message)
        {
            LogIF.baseLog.InfoLog(loggerName, message);
        }

        /// <summary>WARNINGログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>自由に利用できる。</remarks>
        public static void WarnLog(string loggerName, string message)
        {
            LogIF.baseLog.WarnLog(loggerName, message);
        }

        /// <summary>ERRORログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>自由に利用できる。</remarks>
        public static void ErrorLog(string loggerName, string message)
        {
            LogIF.baseLog.ErrorLog(loggerName, message);
        }

        /// <summary>FATALログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>自由に利用できる。</remarks>
        public static void FatalLog(string loggerName, string message)
        {
            LogIF.baseLog.FatalLog(loggerName, message);
        }

        #endregion

        #region ログ レベル情報取得インターフェイス
        
        /// <summary>ロガーのIsDebugEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsDebugEnabled</returns>
        public static bool IsDebugEnabled(string loggerName)
        {
            return LogIF.baseLog.IsDebugEnabled(loggerName);
        }

        /// <summary>ロガーのIsInfoEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsInfoEnabled</returns>
        public static bool IsInfoEnabled(string loggerName)
        {
            return LogIF.baseLog.IsInfoEnabled(loggerName);
        }

        /// <summary>ロガーのIsWarnEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsWarnEnabled</returns>
        public static bool IsWarnEnabled(string loggerName)
        {
            return LogIF.baseLog.IsWarnEnabled(loggerName);
        }

        /// <summary>ロガーのIsErrorEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsErrorEnabled</returns>
        public static bool IsErrorEnabled(string loggerName)
        {
            return LogIF.baseLog.IsErrorEnabled(loggerName);
        }

        /// <summary>ロガーのIsFatalEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsFatalEnabled</returns>
        public static bool IsFatalEnabled(string loggerName)
        {
            return LogIF.baseLog.IsFatalEnabled(loggerName);
        }

        #endregion
    }
}

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
//* クラス日本語名  ：log4netログ出力を行うクラス
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
//**********************************************************************************

using log4net.Repository.Hierarchy;

namespace Touryo.Infrastructure.Public.Log
{
    // log4net-sdk-net-4.0
    // http://logging.apache.org/log4net/release/sdk/

    /// <summary>log4netログ出力を行うクラス</summary>
    /// <remarks>自由に利用できる。</remarks>
    public class LogIF
    {
        #region ログ出力

        /// <summary>log4netのDEBUGログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>自由に利用できる。</remarks>
        public static void DebugLog(string loggerName, string message)
        {
            LogManager.GetLog4netIf(loggerName).Debug(message);
        }

        /// <summary>log4netのINFORMATIONログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>自由に利用できる。</remarks>
        public static void InfoLog(string loggerName, string message)
        {
            LogManager.GetLog4netIf(loggerName).Info(message);
        }

        /// <summary>log4netのWARNINGログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>自由に利用できる。</remarks>
        public static void WarnLog(string loggerName, string message)
        {
            LogManager.GetLog4netIf(loggerName).Warn(message);
        }

        /// <summary>log4netのERRORログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>自由に利用できる。</remarks>
        public static void ErrorLog(string loggerName, string message)
        {
            LogManager.GetLog4netIf(loggerName).Error(message);
        }

        /// <summary>log4netのFATALログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>自由に利用できる。</remarks>
        public static void FatalLog(string loggerName, string message)
        {
            LogManager.GetLog4netIf(loggerName).Fatal(message);
        }

        #endregion

        #region ログ レベル情報取得インターフェイス
        
        /// <summary>ロガーのログ レベルを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>ロガーのログ レベル（DisplayName）</returns>
        public static string GetLoggerLogLevel(string loggerName)
        {
            return ((Logger)LogManager.GetLog4netIf(loggerName).Logger).EffectiveLevel.DisplayName;
        }

        /// <summary>ルート ロガーのログ レベルを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>ルート ロガーのログ レベル（DisplayName）</returns>
        public static string GetRootLoggerLogLevel(string loggerName)
        {
            return ((Logger)LogManager.GetLog4netIf(loggerName).Logger).Level.DisplayName;
        }

        // ---

        /// <summary>ロガーのIsDebugEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsDebugEnabled</returns>
        public static bool IsDebugEnabled(string loggerName)
        {
            return LogManager.GetLog4netIf(loggerName).IsDebugEnabled;
        }

        /// <summary>ロガーのIsInfoEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsInfoEnabled</returns>
        public static bool IsInfoEnabled(string loggerName)
        {
            return LogManager.GetLog4netIf(loggerName).IsInfoEnabled;
        }

        /// <summary>ロガーのIsWarnEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsWarnEnabled</returns>
        public static bool IsWarnEnabled(string loggerName)
        {
            return LogManager.GetLog4netIf(loggerName).IsWarnEnabled;
        }

        /// <summary>ロガーのIsErrorEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsErrorEnabled</returns>
        public static bool IsErrorEnabled(string loggerName)
        {
            return LogManager.GetLog4netIf(loggerName).IsErrorEnabled;
        }

        /// <summary>ロガーのIsFatalEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsFatalEnabled</returns>
        public static bool IsFatalEnabled(string loggerName)
        {
            return LogManager.GetLog4netIf(loggerName).IsFatalEnabled;
        }

        #endregion
    }
}

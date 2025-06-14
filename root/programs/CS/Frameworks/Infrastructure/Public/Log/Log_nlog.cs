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
//* クラス名        ：Log_nlog
//* クラス日本語名  ：NLogログ出力を行うクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2025/06/15  西野 大介         新規作成
//**********************************************************************************

using log4net.Repository.Hierarchy;

namespace Touryo.Infrastructure.Public.Log
{
    /// <summary>NLogログ出力を行うクラス</summary>
    /// <remarks>自由に利用できる。</remarks>
    internal class Log_nlog : BaseLog
    {
        #region ログ出力

        /// <summary>NLogのDEBUGログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>internalクラス</remarks>
        public override void DebugLog(string loggerName, string message)
        {
            LogManager_nlog.GetNLogIf(loggerName).Debug(message);
        }

        /// <summary>NLogのINFORMATIONログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>internalクラス</remarks>
        public override void InfoLog(string loggerName, string message)
        {
            LogManager_nlog.GetNLogIf(loggerName).Info(message);
        }

        /// <summary>NLogのWARNINGログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>internalクラス</remarks>
        public override void WarnLog(string loggerName, string message)
        {
            LogManager_nlog.GetNLogIf(loggerName).Warn(message);
        }

        /// <summary>NLogのERRORログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>internalクラス</remarks>
        public override void ErrorLog(string loggerName, string message)
        {
            LogManager_nlog.GetNLogIf(loggerName).Error(message);
        }

        /// <summary>NLogのFATALログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>internalクラス</remarks>
        public override void FatalLog(string loggerName, string message)
        {
            LogManager_nlog.GetNLogIf(loggerName).Fatal(message);
        }

        #endregion

        #region ログ レベル情報取得インターフェイス

        /// <summary>NLogのロガーのIsDebugEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsDebugEnabled</returns>
        /// <remarks>internalクラス</remarks>
        public override bool IsDebugEnabled(string loggerName)
        {
            return LogManager_nlog.GetNLogIf(loggerName).IsDebugEnabled;
        }

        /// <summary>NLogのロガーのIsInfoEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsInfoEnabled</returns>
        /// <remarks>internalクラス</remarks>
        public override bool IsInfoEnabled(string loggerName)
        {
            return LogManager_nlog.GetNLogIf(loggerName).IsInfoEnabled;
        }

        /// <summary>NLogのロガーのIsWarnEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsWarnEnabled</returns>
        /// <remarks>internalクラス</remarks>
        public override bool IsWarnEnabled(string loggerName)
        {
            return LogManager_nlog.GetNLogIf(loggerName).IsWarnEnabled;
        }

        /// <summary>NLogのロガーのIsErrorEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsErrorEnabled</returns>
        /// <remarks>internalクラス</remarks>
        public override bool IsErrorEnabled(string loggerName)
        {
            return LogManager_nlog.GetNLogIf(loggerName).IsErrorEnabled;
        }

        /// <summary>NLogのロガーのIsFatalEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsFatalEnabled</returns>
        /// <remarks>internalクラス</remarks>
        public override bool IsFatalEnabled(string loggerName)
        {
            return LogManager_nlog.GetNLogIf(loggerName).IsFatalEnabled;
        }

        #endregion
    }
}

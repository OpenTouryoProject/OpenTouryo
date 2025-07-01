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
//* クラス名        ：BaseLog
//* クラス日本語名  ：ログ出力インターフェイス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2025/06/15  西野 大介         新規作成
//**********************************************************************************

namespace Touryo.Infrastructure.Public.Log
{
    /// <summary>ログ出力インターフェイス</summary>
    internal abstract class BaseLog
    {
        #region ログ出力

        /// <summary>DEBUGログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>internalなベース型</remarks>
        public abstract void DebugLog(string loggerName, string message);

        /// <summary>INFORMATIONログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>internalなベース型</remarks>
        public abstract void InfoLog(string loggerName, string message);

        /// <summary>WARNINGログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>internalなベース型</remarks>
        public abstract void WarnLog(string loggerName, string message);

        /// <summary>ERRORログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>internalなベース型</remarks>
        public abstract void ErrorLog(string loggerName, string message);

        /// <summary>FATALログを出力する。</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <param name="message">メッセージ内容</param>
        /// <remarks>internalなベース型</remarks>
        public abstract void FatalLog(string loggerName, string message);

        #endregion

        #region ログ レベル情報取得インターフェイス

        /// <summary>ロガーのIsDebugEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsDebugEnabled</returns>
        /// <remarks>internalなベース型</remarks>
        public abstract bool IsDebugEnabled(string loggerName);

        /// <summary>ロガーのIsInfoEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsInfoEnabled</returns>
        /// <remarks>internalなベース型</remarks>
        public abstract bool IsInfoEnabled(string loggerName);

        /// <summary>ロガーのIsWarnEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsWarnEnabled</returns>
        /// <remarks>internalなベース型</remarks>
        public abstract bool IsWarnEnabled(string loggerName);

        /// <summary>ロガーのIsErrorEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsErrorEnabled</returns>
        /// <remarks>internalなベース型</remarks>
        public abstract bool IsErrorEnabled(string loggerName);

        /// <summary>ロガーのIsFatalEnabledを取得</summary>
        /// <param name="loggerName">ロガー名</param>
        /// <returns>IsFatalEnabled</returns>
        /// <remarks>internalなベース型</remarks>
        public abstract bool IsFatalEnabled(string loggerName);

        #endregion
    }
}

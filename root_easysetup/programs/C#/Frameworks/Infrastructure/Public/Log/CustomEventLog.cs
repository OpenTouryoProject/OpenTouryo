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
//* クラス名        ：CustomEventLog
//* クラス日本語名  ：カスタム イベント ログ出力クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/09/21  西野  大介        新規作成
//**********************************************************************************

using System;
using System.Diagnostics;

namespace Touryo.Infrastructure.Public.Log
{
    /// <summary>カスタム イベント ログ出力</summary>
    public class CustomEventLog
    {
        /// <summary>ソース（内部）</summary>
        private string _source;
        /// <summary>ソース（内部）…読み取り専用</summary>
        public string Source
        {
            get { return this._source; }
        }

        /// <summary>ログ名（表示）</summary>
        private string _logName;
        /// <summary>ログ名（表示）…読み取り専用</summary>
        public string LogName
        {
            get { return this._logName; }
        }

        /// <summary>コンストラクタ</summary>
        /// <param name="source">ソース（内部）</param>
        /// <param name="logName">ログ名（表示）</param>
        public CustomEventLog(string source, string logName)
        {
            // メンバ変数
            this._source = source;
            this._logName = logName;

            // ソースが存在していない時は、作成する
            if (!EventLog.SourceExists(source))
            {
                // ログ名を空白にすると、"Application"となる
                EventLog.CreateEventSource(source, logName);
            }
        }

        /// <summary>メッセージを出力</summary>
        /// <param name="message">メッセージ</param>
        /// <param name="type">EventLogEntryType</param>
        /// <param name="eventID">eventID</param>
        /// <param name="category">カテゴリ</param>
        /// <param name="data">データ</param>
        public void Write(string message, EventLogEntryType type, int eventID, short category, byte[] data)
        {
            // イベントログにエントリを書き込む
            System.Diagnostics.EventLog.WriteEntry(
                this._source, message, type, eventID, category, data);
        }

        /// <summary>Informationエントリとしてメッセージを出力</summary>
        /// <param name="message">メッセージ</param>
        public void WriteInfo(string message)
        {
            // Informationエントリとして
            // イベントログにエントリを書き込む
            System.Diagnostics.EventLog.WriteEntry(this._source, message);
        }

        /// <summary>イベント・ソースを消去</summary>
        /// <param name="source">ソース（内部）</param>
        /// <returns>
        /// true：消去
        /// false：イベント・ソースなし
        /// </returns>
        public static bool Delete(string source)
        {
            // イベント・ソースがあるか調べる
            if (EventLog.SourceExists(source))
            {
                // イベント・ソースあり

                // 2番目のパラメータのコンピュータ名を"."とすることで、ローカルコンピュータとなる
                string logName =
                    System.Diagnostics.EventLog.LogNameFromSourceName(source, ".");

                //ソースを削除
                System.Diagnostics.EventLog.DeleteEventSource(source);
                //イベントログを削除
                System.Diagnostics.EventLog.Delete(logName);

                return true;
            }
            else
            {
                // イベント・ソースなし
                return false;
            }
        }
    }
}

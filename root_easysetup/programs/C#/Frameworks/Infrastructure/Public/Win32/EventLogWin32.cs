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
//* クラス名        ：EventLogWin32
//* クラス日本語名  ：イベントログ関連Win32 API宣言クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/09/21  西野  大介        新規作成
//*  2013/02/18  西野  大介        SetLastError対応
//**********************************************************************************

using System;
using System.Runtime.InteropServices;

namespace Touryo.Infrastructure.Public.Win32
{
    class EventLogWin32
    {
        /// <summary>イベントログの登録済みハンドルを返します。</summary>
        /// <param name="lpUNCServerName">
        /// 操作を実行するサーバの名前
        /// （NULLを指定でローカルコンピュータ）
        /// </param>
        /// <param name="lpSourceName">
        /// 登録済みハンドルを取得するイベントソースの名前
        /// HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Eventlog\Application
        /// </param>
        /// <returns>
        /// イベントログの登録済みハンドル
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern IntPtr RegisterEventSource(string lpUNCServerName, string lpSourceName);

        /// <summary>指定したイベントログのハンドルを閉じます。</summary>
        /// <param name="hEventLog">
        /// イベントログの登録済みハンドル
        /// （RegisterEventSource 関数が返すハンドル）
        /// </param>
        /// <returns>
        /// true：関数が成功
        /// false：関数が失敗
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DeregisterEventSource(IntPtr hEventLog);

        /// <summary>指定したイベントログの最後にエントリを書き込みます。</summary>
        /// <param name="hEventLog">イベントログのハンドル</param>
        /// <param name="wType">ログに書き込むイベントの種類</param>
        /// <param name="wCategory">イベントの分類</param>
        /// <param name="dwEventID">イベント識別子</param>
        /// <param name="lpUserSid">ユーザーセキュリティ識別子</param>
        /// <param name="wNumStrings">メッセージにマージする文字列の数</param>
        /// <param name="dwDataSize">バイナリデータのサイズ（バイト数）</param>
        /// <param name="lpStrings">メッセージにマージする文字列の配列</param>
        /// <param name="lpRawData">バイナリデータのアドレス</param>
        /// <returns>
        /// true：関数が成功
        /// false：関数が失敗
        /// </returns>
        [DllImport("advapi32.dll", EntryPoint = "ReportEventW", CharSet = CharSet.Unicode, SetLastError=true)]
        public static extern bool ReportEvent(
            IntPtr hEventLog,
            ushort wType,
            ushort wCategory,
            int dwEventID,
            IntPtr lpUserSid,
            ushort wNumStrings,
            int dwDataSize,
            string[] lpStrings,
            IntPtr lpRawData);
    }
}

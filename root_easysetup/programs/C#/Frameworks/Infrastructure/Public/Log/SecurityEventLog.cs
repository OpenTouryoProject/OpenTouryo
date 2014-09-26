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
//* クラス名        ：SecurityEventLog
//* クラス日本語名  ：セキュリティ イベント ログ出力クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/09/21  西野  大介        新規作成（作成途中）
//**********************************************************************************

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Touryo.Infrastructure.Public.Win32;

namespace Touryo.Infrastructure.Public.Log
{
    /// <summary>セキュリティ イベント ログ出力クラス</summary>
    public class SecurityEventLog
    {
        /// <summary>
        /// ログ出力には以下に同名のキー・値が必要
        /// HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Eventlog\Security\Touryo
        /// EventMessageFile = C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\EventLogMessages.dll
        /// TypesSupported = 00000007(REG_DWORD)
        /// </summary>
        private const string APP_NAME = "Touryo";

        /// <summary>TOKEN_QUERY アクセス権</summary>
        private const int TOKEN_QUERY = 0x00000008;
        /// <summary>TOKEN_ADJUST_PRIVILEGES アクセス権</summary>
        private const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;

        /// <summary>特権名：監査とセキュリティ ログの管理のアクセス許可</summary>
        private const string SE_SECURITY_NAME = "SeSecurityPrivilege";
        /// <summary>特権名：セキュリティ ログに書き込むアクセス許可</summary>
        private const string SE_AUDIT_NAME = "SeAuditPrivilege";
        /// <summary>特権のLUIDの属性（特権を有効にする）</summary>
        private const int SE_PRIVILEGE_ENABLED = 0x00000002;

        /// <summary>イベントログのタイプ</summary>
        private const ushort EVENTLOG_INFORMATION_TYPE = 0x0004;

        /// <summary>コンストラクタ</summary>
        public SecurityEventLog()
        {
            bool ret = false;
            IntPtr hToken = IntPtr.Zero;

            // アクセス権を希望
            ret = SecurityWin32.OpenProcessToken(
                Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref hToken);
            // ２つの特権を有効にする（特権名を指定して）
            EnablePrivilege(hToken, SE_SECURITY_NAME);
            EnablePrivilege(hToken, SE_AUDIT_NAME);
        }

        /// <summary>特権を有効にする。</summary>
        /// <param name="hToken">トークン</param>
        /// <param name="name"></param>
        private void EnablePrivilege(IntPtr hToken, string name)
        {
            long LUID = 0;
            bool ret = false;

            // 特権のLUIDを取得
            ret = SecurityWin32.LookupPrivilegeValue(null, name, ref LUID);

            // 特権情報構造体
            SecurityWin32.TOKEN_PRIVILEGES tp = new SecurityWin32.TOKEN_PRIVILEGES();
            // 特権のLUID属性を設定
            tp.Privileges = new SecurityWin32.LUID_AND_ATTRIBUTES();
            tp.Privileges.Luid = LUID;
            tp.Privileges.Attributes = SE_PRIVILEGE_ENABLED;
            
            // 特権を有効にする。
            ret = SecurityWin32.AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>Informationエントリとしてメッセージを出力</summary>
        /// <param name="message">メッセージ</param>
        /// <param name="category">カテゴリ</param>
        /// <param name="eventID">eventID</param>
        public void Write(string message, ushort category, int eventID)
        {
            bool ret = false;
            // イベント・ソースの登録済みハンドルを開く
            IntPtr hEventLog = EventLogWin32.RegisterEventSource(null, APP_NAME);

            // ここでエラー（ERROR_ACCESS_DENIED ）になる。
            // Writing in Security log on WinXP - Sysinternals Forums
            // http://forum.sysinternals.com/writing-in-security-log-on-winxp_topic2804.html
            CmnWin32.ErrorCodes ec = CmnWin32.GetLastError();

            // セキュリティ・ログに書き込み
            ret = EventLogWin32.ReportEvent(
                hEventLog, EVENTLOG_INFORMATION_TYPE, category, eventID, IntPtr.Zero, 1, 0, new string[] { message }, IntPtr.Zero);

            // イベント・ソースの登録済みハンドルを閉じる
            ret = EventLogWin32.DeregisterEventSource(hEventLog);
        }
    }
}

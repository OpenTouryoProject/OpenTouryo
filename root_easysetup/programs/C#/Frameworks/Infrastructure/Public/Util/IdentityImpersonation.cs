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
//* クラス名        ：IdentityImpersonation
//* クラス日本語名  ：偽装クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/06/14  西野  大介        新規作成
//*  2013/03/13  西野  大介        偽装ユーザでEXE起動するメソッドを追加。
//*  2013/03/13  西野  大介        クラス名の誤りを修正した。
//*  2013/03/13  西野  大介        CreateProcessAsImpersonationUser追加対応
//**********************************************************************************

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;

using System.Web.Security;
using System.Security.Principal;
using System.Runtime.InteropServices;

// 業務フレームワーク（循環参照になるため、参照しない）
// フレームワーク（循環参照になるため、参照しない）

// 部品
using Touryo.Infrastructure.Public.Db;
using Touryo.Infrastructure.Public.IO;
using Touryo.Infrastructure.Public.Log;
using Touryo.Infrastructure.Public.Str;
using Touryo.Infrastructure.Public.Util;
using Touryo.Infrastructure.Public.Win32;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>偽装クラス</summary>
    /// <remarks>
    /// 自由に利用できる。
    /// 
    /// ASP.NET アプリケーションに偽装を実装する方法
    /// http://support.microsoft.com/kb/306158/ja
    /// 
    /// if (impersonateValidUser("username", "domain", "password"))
    /// {
    ///   // Insert your code that runs under the security context of a specific user here.
    ///   undoImpersonation();
    /// }
    /// else
    /// {
    ///   // Your impersonation failed. Therefore, include a fail-safe mechanism here.
    /// }
    /// </remarks>
    public class IdentityImpersonation
    {
        /// <summary>dwLogonType：対話ユーザ（LOGON32_LOGON_INTERACTIVE）</summary>
        private const int LOGON32_LOGON_INTERACTIVE = 2;
        /// <summary>dwLogonProvider：システム標準（LOGON32_PROVIDER_DEFAULT）</summary>
        private const int LOGON32_PROVIDER_DEFAULT = 0;

        /// <summary>偽装前のWindowsユーザを表す。</summary>
        /// <remarks>DisposeでUndoされるものと考える</remarks>
        private WindowsImpersonationContext impersonationContext;

        /// <summary>
        /// WindowsIdentityで偽装する。
        /// </summary>
        /// <param name="winId"></param>
        /// <returns>
        /// ・true：成功
        /// ・false：失敗
        /// </returns>
        public bool ImpersonateWinIdUser(WindowsIdentity winId)
        {
            try
            {
                this.impersonationContext = winId.Impersonate();

                // 正常終了
                return true;
            }
            catch
            {
                // 異常終了
                return false;
            }
        }

        /// <summary>
        /// ユーザ名・ドメイン・パスワードで偽装する。
        /// </summary>
        /// <param name="userName">ユーザ名</param>
        /// <param name="domain">ドメイン</param>
        /// <param name="password">パスワード</param>
        /// <returns>
        /// ・true：成功
        /// ・false：失敗
        /// </returns>
        public bool ImpersonateValidUser(string userName, string domain, string password)
        {
            string temp;

            // 既定は、SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation
            return this.ImpersonateValidUser(userName, domain, password,
                SecurityWin32.SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, out temp);
        }

        /// <summary>
        /// ユーザ名・ドメイン・パスワードで偽装する。
        /// </summary>
        /// <param name="userName">ユーザ名</param>
        /// <param name="domain">ドメイン</param>
        /// <param name="password">パスワード</param>
        /// <param name="impersonationLevel">偽装レベル</param>
        /// <param name="errorInfo">エラー情報</param>
        /// <returns>
        /// ・true：成功
        /// ・false：失敗
        /// </returns>
        public bool ImpersonateValidUser(
            string userName, string domain, string password,
            SecurityWin32.SECURITY_IMPERSONATION_LEVEL impersonationLevel, out string errorInfo)
        {
            // エラー情報の初期化
            errorInfo = "";

            // ワーク
            WindowsIdentity tempWindowsIdentity;

            // トークンのハンドラ
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            try
            {
                // クライアントアプリケーションによる偽装を終了。
                if (SecurityWin32.RevertToSelf())
                {
                    // RevertToSelf成功

                    // 偽装する。

                    // トークンハンドルを取得
                    if (SecurityWin32.LogonUserA(userName, domain, password,
                        LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                    {
                        // LogonUserA成功

                        // 偽装アクセストークンハンドルを作成
                        if (SecurityWin32.DuplicateToken(token, impersonationLevel, ref tokenDuplicate) != 0)
                        {
                            // DuplicateToken成功

                            // 偽装アクセストークンを使用して偽装する。
                            tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                            this.impersonationContext = tempWindowsIdentity.Impersonate();
                            if (this.impersonationContext != null)
                            {
                                // Impersonate成功
                                // 正常終了
                                return true;
                            }
                            else
                            {
                                // Impersonate失敗
                                errorInfo = "Impersonate failed";
                            }
                        }
                        else
                        {
                            // DuplicateToken失敗
                            errorInfo = "DuplicateToken failed with " + Marshal.GetLastWin32Error();
                        }
                    }
                    else
                    {
                        // LogonUserA失敗
                        errorInfo = "LogonUserA failed with " + Marshal.GetLastWin32Error();
                    }
                }
                else
                {
                    // RevertToSelf失敗
                    errorInfo = "RevertToSelf failed with " + Marshal.GetLastWin32Error();
                }
            }
            finally
            {
                // 失敗（例外発生）時など。

                // トークンハンドル、
                // 偽装アクセストークンハンドルをクローズ
                if (token != IntPtr.Zero)
                {
                    CmnWin32.CloseHandle(token);
                }
                if (tokenDuplicate != IntPtr.Zero)
                {
                    CmnWin32.CloseHandle(tokenDuplicate);
                }
            }

            // 異常終了
            return false;
        }

        /// <summary>コンテキストを偽装状態から元に戻す。</summary>
        /// <returns>
        /// ・true：成功
        /// ・false：失敗
        /// </returns>
        public bool UndoImpersonation()
        {
            if (this.impersonationContext != null)
            {
                // 偽装前に戻す。
                this.impersonationContext.Undo();
                this.impersonationContext = null;
                return true; // 成功
            }
            else
            {
                return false; // 失敗
            }
        }

        /// <summary>
        /// ASP.NET で偽装ユーザーのコンテキストで実行されるプロセスを生成する
        /// http://support.microsoft.com/kb/889251/ja
        /// </summary>
        /// <param name="commandLinePath">コマンドライン</param>
        /// <param name="currentDirectory">カレント・ディレクトリ</param>
        /// <returns>
        /// ・true：成功
        /// ・false：失敗
        /// </returns>
        public static bool CreateProcessAsImpersonationUser(string commandLinePath, string currentDirectory)
        {
            string temp;

            // 既定は、SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation
            return CreateProcessAsImpersonationUser(
                commandLinePath, currentDirectory,
                SecurityWin32.SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, out temp);
        }

        /// <summary>
        /// ASP.NET で偽装ユーザーのコンテキストで実行されるプロセスを生成する
        /// http://support.microsoft.com/kb/889251/ja
        /// </summary>
        /// <param name="commandLinePath">コマンドライン</param>
        /// <param name="currentDirectory">カレント・ディレクトリ</param>
        /// <param name="impersonationLevel">偽装レベル</param>
        /// <param name="errorInfo">エラー情報</param>
        /// <returns>
        /// ・true：成功
        /// ・false：失敗
        /// </returns>
        public static bool CreateProcessAsImpersonationUser(
            string commandLinePath, string currentDirectory,
            SecurityWin32.SECURITY_IMPERSONATION_LEVEL impersonationLevel, out string errorInfo)
        {
            // エラー情報の初期化
            errorInfo = "";

            // 失敗するので初期化
            if (string.IsNullOrEmpty(currentDirectory))
            {   
                currentDirectory = 
                    Environment.GetEnvironmentVariable(
                        "SystemRoot", EnvironmentVariableTarget.Process);
            }

            // トークン
            IntPtr token = IntPtr.Zero;
            // 継承可能にする。
            IntPtr tokenDuplicate = IntPtr.Zero;

            // 戻り値
            bool ret;

            // 偽装ユーザのアカウント・トークン
            token = WindowsIdentity.GetCurrent().Token;

            // SECURITY_ATTRIBUTES構造体
            SecurityWin32.SECURITY_ATTRIBUTES sa 
                = new SecurityWin32.SECURITY_ATTRIBUTES();

            // Security Descriptor
            sa.lpSecurityDescriptor = IntPtr.Zero; // = (IntPtr)0;
            // Security Descriptorのハンドルは継承不可能
            sa.bInheritHandle = false;
            // サイズ
            sa.Length = Marshal.SizeOf(sa);

            try
            {
                // 偽装アクセストークンハンドルは、
                // CreateProcessAsUserに指定できないため、 
                // DuplicateTokenExでプライマリ・トークンに変換する
                ret = SecurityWin32.DuplicateTokenEx(
                    token, (uint)SecurityWin32.ACCESS_MASK.GENERIC_ALL, ref sa,
                    (int)impersonationLevel, (int)SecurityWin32.TOKEN_TYPE.TokenPrimary, ref tokenDuplicate);

                // 戻り値判定
                if (ret)
                {
                    // true（成功）

                    // STARTUPINFO構造体
                    ProcessWin32.STARTUPINFO si = new ProcessWin32.STARTUPINFO();
                    // デスクトップ名
                    si.lpDesktop = "";
                    // サイズ
                    si.cb = Marshal.SizeOf(si);

                    // PROCESS_INFORMATION構造体
                    ProcessWin32.PROCESS_INFORMATION pi = new ProcessWin32.PROCESS_INFORMATION();

                    // 偽装可能にしたトークンを指定してプロセス起動
                    ret = ProcessWin32.CreateProcessAsUser(
                        tokenDuplicate, null, commandLinePath, ref sa, ref sa, false,
                        0, IntPtr.Zero, currentDirectory, ref si, out pi);

                    // 戻り値判定
                    if (ret)
                    {
                        // true（成功）

                        CmnWin32.CloseHandle(pi.hProcess);
                        CmnWin32.CloseHandle(pi.hThread);

                        // 偽装可能にしたトークンのハンドラを閉じる
                        ret = CmnWin32.CloseHandle(tokenDuplicate);
                    }
                    else
                    {
                        // asp.net - Running cscript.exe from C# .ashx does not execute code in vbscript file - Stack Overflow
                        // http://stackoverflow.com/questions/3842020/running-cscript-exe-from-c-sharp-ashx-does-not-execute-code-in-vbscript-file

                        // false（失敗）
                        errorInfo = "CreateProcessAsUser failed with " + Marshal.GetLastWin32Error()
                            + ": if 1314, make sure user is a member 'Replace a process level token' Control Panel -> Administrative Tools -> Local Security Settings.";
                    }
                }
                else
                {
                    // false（失敗）
                    errorInfo = "DuplicateTokenEx failed with " + Marshal.GetLastWin32Error();
                }
            }
            finally
            {
                // 失敗（例外発生）時など。

                // トークンハンドル、
                // 偽装アクセストークンハンドルをクローズ
                if (token != IntPtr.Zero)
                {
                    CmnWin32.CloseHandle(token);
                }
                if (tokenDuplicate != IntPtr.Zero)
                {
                    CmnWin32.CloseHandle(tokenDuplicate);
                }
            }

            // false（失敗）
            return false;
        }
    }
}

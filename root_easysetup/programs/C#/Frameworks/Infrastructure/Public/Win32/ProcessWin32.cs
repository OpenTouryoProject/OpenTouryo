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
//* クラス名        ：ProcessWin32
//* クラス日本語名  ：プロセス関連Win32 API宣言クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2013/03/13  西野  大介        新規作成
//**********************************************************************************

using System;
using System.Runtime.InteropServices;

namespace Touryo.Infrastructure.Public.Win32
{
    /// <summary>プロセス関連Win32 API宣言クラス</summary>
    public class ProcessWin32
    {
        #region 構造体・列挙型

        /// <summary>
        /// STARTUPINFO構造体
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms686331.aspx
        /// Specifies the window station, desktop, standard handles,
        /// and appearance of the main window for a process at creation time.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            /// <summary>
            /// The size of the structure, in bytes.</summary>
            public int cb;

            /// <summary>
            /// Reserved; must be NULL.</summary>
            public String lpReserved;

            /// <summary>
            /// The name of the desktop, or the name of both the desktop and window station for this process.
            /// </summary>
            public String lpDesktop;

            /// <summary>
            /// For console processes, this is the title displayed in the title bar if a new console window is created.
            /// </summary>
            public String lpTitle;

            /// <summary>
            /// If dwFlags specifies STARTF_USEPOSITION
            /// </summary>
            public uint dwX;

            /// <summary>
            /// If dwFlags specifies STARTF_USEPOSITION
            /// </summary>
            public uint dwY;

            /// <summary>
            /// If dwFlags specifies STARTF_USESIZE
            /// </summary>
            public uint dwXSize;

            /// <summary>
            /// If dwFlags specifies STARTF_USESIZE
            /// </summary>
            public uint dwYSize;

            /// <summary>
            /// If dwFlags specifies STARTF_USECOUNTCHARS
            /// </summary>
            public uint dwXCountChars;

            /// <summary>
            /// If dwFlags specifies STARTF_USECOUNTCHARS
            /// </summary>
            public uint dwYCountChars;

            /// <summary>
            /// If dwFlags specifies STARTF_USEFILLATTRIBUTE
            /// </summary>
            public uint dwFillAttribute;

            /// <summary>
            /// A bitfield that determines whether certain STARTUPINFO
            /// members are used when the process creates a window.
            /// </summary>
            public uint dwFlags;

            /// <summary>
            /// If dwFlags specifies STARTF_USESHOWWINDOW
            /// </summary>
            public short wShowWindow;

            /// <summary>
            /// Reserved for use by the C Run-time; must be zero.
            /// </summary>
            public short cbReserved2;

            /// <summary>
            /// Reserved for use by the C Run-time; must be NULL.
            /// </summary>
            public IntPtr lpReserved2;

            /// <summary>
            /// If dwFlags specifies STARTF_USESTDHANDLES, this member is the standard input handle for the process.
            /// </summary>
            public IntPtr hStdInput;

            /// <summary>
            /// If dwFlags specifies STARTF_USESTDHANDLES, this member is the standard output handle for the process.
            /// </summary>
            public IntPtr hStdOutput;

            /// <summary>
            /// If dwFlags specifies STARTF_USESTDHANDLES, this member is the standard error handle for the process.
            /// </summary>
            public IntPtr hStdError;
        }

        /// <summary>
        /// PROCESS_INFORMATION構造体
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms684873.aspx
        /// Contains information about a newly created process and its primary thread. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            /// <summary>
            /// A handle to the newly created process.
            /// </summary>
            public IntPtr hProcess;

            /// <summary>
            /// A handle to the primary thread of the newly created process.
            /// </summary>
            public IntPtr hThread;

            /// <summary>
            /// A value that can be used to identify a process.
            /// </summary> 
            public uint dwProcessId;

            /// <summary>
            /// A value that can be used to identify a thread.
            /// </summary>
            public uint dwThreadId;
        }

        /// <summary>
        /// STARTUPINFO構造体で利用可能なオプション
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms686331.aspx
        /// </summary>
        [Flags]
        public enum STARTFFlags
        {
            /// <summary>
            /// Indicates that the cursor is in feedback mode for two seconds after CreateProcess is called. 
            /// </summary>
            STARTF_FORCEONFEEDBACK = 0x00000040,

            /// <summary>
            /// Indicates that the feedback cursor is forced off while the process is starting.
            /// </summary>
            STARTF_FORCEOFFFEEDBACK = 0x00000080,

            /// <summary>
            /// Indicates that any windows created by the process cannot be pinned on the taskbar.
            /// </summary>
            STARTF_PREVENTPINNING = 0x00002000,

            /// <summary>
            /// Indicates that the process should be run in full-screen mode, rather than in windowed mode.
            /// </summary>
            STARTF_RUNFULLSCREEN = 0x00000020,

            /// <summary>
            /// The lpTitle member contains an AppUserModelID. 
            /// </summary>
            STARTF_TITLEISAPPID = 0x00001000,

            /// <summary>
            /// The lpTitle member contains the path of the shortcut file (.lnk) that the user invoked to start this process.
            /// </summary>
            STARTF_TITLEISLINKNAME = 0x00000800,

            /// <summary>
            /// The dwXCountChars and dwYCountChars members contain additional information.
            /// </summary>
            STARTF_USECOUNTCHARS = 0x00000008,

            /// <summary>
            /// The dwFillAttribute member contains additional information.
            /// </summary>
            STARTF_USEFILLATTRIBUTE = 0x00000010,

            /// <summary>
            /// The hStdInput member contains additional information.
            /// </summary>
            STARTF_USEHOTKEY = 0x00000200,

            /// <summary>
            /// The dwX and dwY members contain additional information.
            /// </summary>
            STARTF_USEPOSITION = 0x00000004,

            /// <summary>
            /// The wShowWindow member contains additional information.
            /// </summary>
            STARTF_USESHOWWINDOW = 0x00000001,

            /// <summary>
            /// The dwXSize and dwYSize members contain additional information.
            /// </summary>
            STARTF_USESIZE = 0x00000002,

            /// <summary>
            /// The hStdInput, hStdOutput, and hStdError members contain additional information.
            /// </summary>
            STARTF_USESTDHANDLES = 0x00000100
        }

        /// <summary>
        /// CreateProcessで利用可能なオプション
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms684863.aspx
        /// The following process creation flags are used by the
        /// ・CreateProcess,
        /// ・CreateProcessAsUser,
        /// ・CreateProcessWithLogonW,
        /// ・CreateProcessWithTokenW. 
        /// </summary>
        [Flags]
        public enum ProcessCreationFlags
        {
            /// <summary>
            /// The child processes of a process associated with a job are not associated with the job.
            /// </summary>
            CREATE_BREAKAWAY_FROM_JOB = 0x01000000,

            /// <summary>
            /// The new process does not inherit the error mode of the calling process.
            /// </summary>
            CREATE_DEFAULT_ERROR_MODE = 0x04000000,

            /// <summary>
            /// The new process has a new console, instead of inheriting its parent's console (the default).
            /// </summary>
            CREATE_NEW_CONSOLE = 0x00000010,

            /// <summary>
            /// The new process is the root process of a new process group.
            /// </summary>
            CREATE_NEW_PROCESS_GROUP = 0x00000200,

            /// <summary>
            /// The process is a console application that is being run without a console window.
            /// </summary>
            CREATE_NO_WINDOW = 0x08000000,

            /// <summary>
            /// The process is to be run as a protected process.
            /// </summary>
            CREATE_PROTECTED_PROCESS = 0x00040000,

            /// <summary>
            /// Allows the caller to execute a child process that ・・・
            /// </summary>
            CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,

            /// <summary>
            /// This flag is valid only when starting a 16-bit Windows-based application.
            /// </summary>
            CREATE_SEPARATE_WOW_VDM = 0x00000800,

            /// <summary>
            /// The flag is valid only when starting a 16-bit Windows-based application.
            /// </summary>
            CREATE_SHARED_WOW_VDM = 0x00001000,

            /// <summary>
            /// The primary thread of the new process is created in a suspended state,
            /// and does not run until the ResumeThread function is called.
            /// </summary>
            CREATE_SUSPENDED = 0x00000004,

            /// <summary>
            /// the environment block pointed to by lpEnvironment uses Unicode characters.
            /// </summary>
            CREATE_UNICODE_ENVIRONMENT = 0x00000400,

            /// <summary>
            /// The calling thread starts and debugs the new process.
            /// </summary>
            DEBUG_ONLY_THIS_PROCESS = 0x00000002,

            /// <summary>
            /// The calling thread starts and debugs the new process
            /// and all child processes created by the new process.
            /// </summary>
            DEBUG_PROCESS = 0x00000001,

            /// <summary>
            /// For console processes, the new process does not inherit its parent's console (the default).
            /// </summary>
            DETACHED_PROCESS = 0x00000008,

            /// <summary>
            /// The process is created with extended startup information
            /// the lpStartupInfo parameter specifies a STARTUPINFOEX structure.
            /// </summary>
            EXTENDED_STARTUPINFO_PRESENT = 0x00080000,

            /// <summary>
            /// The process inherits its parent's affinity.
            /// </summary>
            INHERIT_PARENT_AFFINITY = 0x00010000
        }

        #endregion

        #region API

        /// <summary>
        /// CreateProcessAsUser関数
        /// http://msdn.microsoft.com/ja-jp/library/cc429069.aspx
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms682429.aspx
        /// 新しいプロセスとそのプライマリスレッドを返します。
        /// hToken によって表されるユーザのセキュリティコンテキストを使う
        /// </summary>
        /// <param name="hToken">
        /// ログオンしているユーザーを表すトークンを識別するハンドル
        /// </param>
        /// <param name="lpApplicationName">
        /// 実行可能モジュールの名前へのポインタ
        /// </param>
        /// <param name="lpCommandLine">
        /// コマンドライン文字列へのポインタ
        /// </param>
        /// <param name="lpProcessAttributes">
        /// プロセスのセキュリティ属性へのポインタ
        /// </param>
        /// <param name="lpThreadAttributes">
        /// スレッドのセキュリティ属性へのポインタ
        /// </param>
        /// <param name="bInheritHandle">
        /// 新しいプロセスがハンドルを識別するかどうか
        /// </param>
        /// <param name="dwCreationFlags">
        /// プロセスの作成方法を制御するフラグ 
        /// </param>
        /// <param name="lpEnvironment">
        /// 新しい環境ブロックへのポインタ 
        /// </param>
        /// <param name="lpCurrentDirectory">
        /// 現在のディレクトリ名へのポインタ 
        /// </param>
        /// <param name="lpStartupInfo">
        /// STARTUPINFO 構造体へのポインタ
        /// </param>
        /// <param name="lpProcessInformation">
        /// PROCESS_INFORMATION 構造体へのポインタ
        /// </param>
        /// <remarks>
        /// CreateProcessAsUser  すなのかたまり
        /// http://msmania.wordpress.com/tag/createprocessasuser/
        /// [Win32] [C++] LogonUser と CreateProcessAsUser  すなのかたまり
        /// http://msmania.wordpress.com/2011/02/06/win32-c-logonuser-%e3%81%a8-createprocessasuser/
        /// [Win32] [C++] CreateProcessAsUser  #1 特権編  すなのかたまり
        /// http://msmania.wordpress.com/2011/12/31/win32-c-createprocessasuser-1-%E7%89%B9%E6%A8%A9%E7%B7%A8/
        /// [Win32] [C++] CreateProcessAsUser  #2 トークン編  すなのかたまり
        /// http://msmania.wordpress.com/2011/12/31/win32-c-createprocessasuser-2-%E3%83%88%E3%83%BC%E3%82%AF%E3%83%B3%E7%B7%A8/
        /// [Win32] [C++] CreateProcessAsUser  #3 ソース  すなのかたまり
        /// http://msmania.wordpress.com/2011/12/31/win32-c-createprocessasuser-3-%E3%82%BD%E3%83%BC%E3%82%B9/
        /// [Win32] [C++] CreateProcessAsUser  #4 セキュリティ記述子  すなのかたまり
        /// http://msmania.wordpress.com/2012/01/01/win32-c-createprocessasuser-4-%E3%82%BB%E3%82%AD%E3%83%A5%E3%83%AA%E3%83%86%E3%82%A3%E8%A8%98%E8%BF%B0%E5%AD%90/             
        /// </remarks>
        /// <returns>
        /// ・0以外：成功
        /// ・0：失敗
        /// 拡張エラー情報を取得するには、
        /// GetLastError 関数を使用する。
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateProcessAsUser(
            IntPtr hToken,
            String lpApplicationName,
            String lpCommandLine,
            ref SecurityWin32.SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SecurityWin32.SECURITY_ATTRIBUTES lpThreadAttributes,
            bool bInheritHandle,
            ProcessWin32.ProcessCreationFlags dwCreationFlags,
            IntPtr lpEnvironment,
            String lpCurrentDirectory,
            ref ProcessWin32.STARTUPINFO lpStartupInfo,
            out ProcessWin32.PROCESS_INFORMATION lpProcessInformation);

        #endregion
    }
}

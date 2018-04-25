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
//* クラス名        ：SecurityWin32
//* クラス日本語名  ：セキュリティ関連Win32 API宣言クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2012/06/14  西野 大介         新規作成
//*  2012/09/21  西野 大介         セキュリティ・ログで使用
//*  2013/02/18  西野 大介         SetLastError対応
//*  2013/03/13  西野 大介         CreateProcessAsImpersonationUser追加対応
//**********************************************************************************

using System;
using System.Runtime.InteropServices;

namespace Touryo.Infrastructure.Public.Win32
{
    /// <summary>
    /// セキュリティ関連Win32 API宣言クラス
    /// </summary>
    public class SecurityWin32
    {
        #region 構造体

        /// <summary>
        /// SECURITY_ATTRIBUTES構造体
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa379560.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            /// <summary>
            /// The size, in bytes, of this structure.
            /// </summary>
            public int Length;

            /// <summary>
            /// A pointer to a SECURITY_DESCRIPTOR structure that controls access to the object.
            /// </summary>
            public IntPtr lpSecurityDescriptor;

            /// <summary>
            /// If this member is TRUE, the new process inherits the handle.
            /// </summary>
            public bool bInheritHandle;
        }

        /// <summary>
        /// SECURITY_DESCRIPTOR構造体
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa379561.aspx
        /// 
        /// pinvoke.net SECURITY_DESCRIPTOR (Structures)
        /// http://www.pinvoke.net/default.aspx/Structures/SECURITY_DESCRIPTOR.html
        /// Summary
        /// The SECURITY_DESCRIPTOR structure contains the security information associated with an object.
        /// Applications use this structure to set and query an object's security status.
        /// 
        /// セキュリティディスクリプタとは？ - Web-DB プログラミング徹底解説
        /// http://keicode.com/windows/security-descriptor.php
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct SECURITY_DESCRIPTOR
        {
            /// <summary>
            /// revision
            /// </summary>
            public byte revision;

            /// <summary>
            /// size
            /// </summary>
            public byte size;

            /// <summary>
            /// control
            /// </summary>
            public short control;

            /// <summary>
            /// An owner security identifier (SID)
            /// </summary>
            public IntPtr owner;

            /// <summary>
            /// A primary group security identifier (SID)
            /// </summary>
            public IntPtr group;

            /// <summary>
            /// SACL (System Access Control List)
            /// </summary>
            public IntPtr sacl;

            /// <summary>
            /// DACL (Discretionary Access Control List)
            /// </summary>
            public IntPtr dacl;
        }

        /// <summary>
        /// 特権情報
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa379630.aspx
        /// ANYSIZE_ARRAYは、メンバが配列を表すための指標であり、実際には1として定義されています。
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TOKEN_PRIVILEGES
        {
            /// <summary>特権の数</summary>
            private const int PRIVILEGECOUNT = 1;
            /// <summary>特権構造体</summary>
            public LUID_AND_ATTRIBUTES Privileges;
        }

        /// <summary>
        /// 特権のLUID
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa379263.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct LUID_AND_ATTRIBUTES
        {
            /// <summary>特権のLUID</summary>
            public long Luid;
            /// <summary>LUIDの属性</summary>
            public int Attributes;
        }

        /// <summary>
        /// SECURITY_IMPERSONATION_LEVEL enumeration (Windows)
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa379572.aspx
        /// pinvoke.net SECURITY_IMPERSONATION_LEVEL (Enums)
        /// http://www.pinvoke.net/default.aspx/Enums/SECURITY_IMPERSONATION_LEVEL.html
        /// Summary
        /// The SECURITY_IMPERSONATION_LEVEL enumeration type contains values that specify security impersonation levels.
        /// Security impersonation levels govern the degree to which a server process can act on behalf of a client process.
        /// </summary>
        public enum SECURITY_IMPERSONATION_LEVEL
        {
            /// <summary>
            /// 呼び出し元の資格情報を非表示にする。 
            /// クライアントを偽装できない。
            /// </summary>
            SecurityAnonymous,

            /// <summary>
            /// 呼び出し元の資格情報を照会できる（偽装はできない）。
            /// </summary>
            SecurityIdentification,

            /// <summary>
            /// 呼び出し元の資格情報を照会、偽装で使用できる。
            /// </summary>
            SecurityImpersonation,

            /// <summary>
            /// 呼び出し元の呼び出し元の資格情報を照会、偽装で使用できる。
            /// </summary>
            SecurityDelegation
        }

        /// <summary>
        /// トークン型（プライマリか偽装）
        /// 
        /// TOKEN_TYPE enumeration (Windows)
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa379633.aspx
        /// pinvoke.net TOKEN_TYPE (Enums)
        /// http://www.pinvoke.net/default.aspx/Enums/TOKEN_TYPE.html
        /// Summary
        /// The TOKEN_TYPE enumeration type contains values
        /// that differentiate between a primary token and an impersonation token.
        /// </summary>
        public enum TOKEN_TYPE
        {
            /// <summary>プライマリ・トークン</summary>
            TokenPrimary = 1,
            /// <summary>偽装トークン</summary>
            TokenImpersonation
        }

        /// <summary>
        /// ACCESS_MASK (Windows)
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/aa374892.aspx
        /// pinvoke.net ACCESS_MASK (Enums)
        /// http://www.pinvoke.net/default.aspx/Enums/ACCESS_MASK.html
        /// Summary
        /// The ACCESS_MASK data type is a double word value that defines standard, specific, and generic rights.
        /// These rights are used in access control entries (ACEs) and are the primary means of specifying the requested or granted access to an object.
        /// 
        /// Desktop Security and Access Rights (Windows)
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms682575.aspx
        /// Window Station Security and Access Rights (Windows)
        /// http://msdn.microsoft.com/ja-jp/library/windows/desktop/ms687391.aspx
        /// </summary>
        /// <remarks>
        /// FileSystemRightsとの関連
        /// FileSystemRights 列挙体 (System.Security.AccessControl)
        /// http://msdn.microsoft.com/ja-jp/library/system.security.accesscontrol.filesystemrights.aspx
        /// ↓定義
        /// DACL Access Rules.
        /// http://social.msdn.microsoft.com/forums/en-US/clr/thread/b2137ad3-3db8-47aa-8d83-af154a37ee7e
        /// ↓24-31ビット
        /// Access Mask Format (Windows)
        /// http://msdn.microsoft.com/ja-jp/library/aa374896.aspx
        /// アクセス権
        /// http://eternalwindows.jp/security/accesscontrol/accesscontrol03.html
        /// </remarks>
        /// <example>
        /// （例）
        /// FileSystemRights.FullControl = 0x1f01ff = 00000000000111110000000111111111
        /// ・Specific rights. 0-8
        /// ・Standard rights. 16-20
        /// </example>
        [Flags]
        public enum ACCESS_MASK : uint
        {
            #region Specific rights.

            /// <summary>
            /// (0-15)  Specific rights. Contains the access mask specific to the object type associated with the mask.
            /// </summary>
            SPECIFIC_RIGHTS_ALL = 0x0000ffff,

            #region Desktop Security

            /// <summary>
            /// (00)    DESKTOP_READOBJECTS
            /// </summary>
            DESKTOP_READOBJECTS = 0x00000001,

            /// <summary>
            /// (01)    DESKTOP_CREATEWINDOW
            /// </summary>
            DESKTOP_CREATEWINDOW = 0x00000002,

            /// <summary>
            /// (02)    DESKTOP_CREATEMENU
            /// </summary>
            DESKTOP_CREATEMENU = 0x00000004,

            /// <summary>
            /// (03)    DESKTOP_HOOKCONTROL
            /// </summary>
            DESKTOP_HOOKCONTROL = 0x00000008,

            /// <summary>
            /// (04)    DESKTOP_JOURNALRECORD
            /// </summary>
            DESKTOP_JOURNALRECORD = 0x00000010,

            /// <summary>
            /// (05)    DESKTOP_JOURNALPLAYBACK
            /// </summary>
            DESKTOP_JOURNALPLAYBACK = 0x00000020,

            /// <summary>
            /// (06)    DESKTOP_ENUMERATE
            /// </summary>
            DESKTOP_ENUMERATE = 0x00000040,

            /// <summary>
            /// (07)    DESKTOP_WRITEOBJECTS
            /// </summary>
            DESKTOP_WRITEOBJECTS = 0x00000080,

            /// <summary>
            /// (08)    DESKTOP_SWITCHDESKTOP
            /// </summary>
            DESKTOP_SWITCHDESKTOP = 0x00000100,

            #endregion

            #region Window Station Security

            /// <summary>
            /// (00)    WINSTA_ENUMDESKTOPS
            /// </summary>
            WINSTA_ENUMDESKTOPS = 0x00000001,
            
            /// <summary>
            /// (01)    WINSTA_READATTRIBUTES
            /// </summary>
            WINSTA_READATTRIBUTES = 0x00000002,
            
            /// <summary>
            /// (02)    WINSTA_ACCESSCLIPBOARD
            /// </summary>
            WINSTA_ACCESSCLIPBOARD = 0x00000004,
            
            /// <summary>
            /// (03)    WINSTA_CREATEDESKTOP
            /// </summary>
            WINSTA_CREATEDESKTOP = 0x00000008,
            
            /// <summary>
            /// (04)    WINSTA_WRITEATTRIBUTES
            /// </summary>
            WINSTA_WRITEATTRIBUTES = 0x00000010,
            
            /// <summary>
            /// (05)    WINSTA_ACCESSGLOBALATOMS
            /// </summary>
            WINSTA_ACCESSGLOBALATOMS = 0x00000020,
            
            /// <summary>
            /// (06)    WINSTA_EXITWINDOWS
            /// </summary>
            WINSTA_EXITWINDOWS = 0x00000040,
            
            /// <summary>
            /// (08)    WINSTA_ENUMERATE
            /// </summary>
            WINSTA_ENUMERATE = 0x00000100,
            
            /// <summary>
            /// (09)    WINSTA_READSCREEN
            /// </summary>
            WINSTA_READSCREEN = 0x00000200,
            
            /// <summary>
            /// WINSTA_ALL_ACCESS(上記07を除くすべて)
            /// </summary>
            WINSTA_ALL_ACCESS = 0x0000037f,

            #endregion

            #endregion

            #region Standard rights.

            #region 基本

            /// <summary>
            /// (16)    DELETE
            /// </summary>
            DELETE                    = 0x00010000,

            /// <summary>
            /// (17)    READ_CONTROL
            /// </summary>
            READ_CONTROL              = 0x00020000,

            /// <summary>
            /// (18)    WRITE_DAC
            /// </summary>
            WRITE_DAC                 = 0x00040000,

            /// <summary>
            /// (19)    WRITE_OWNER
            /// </summary>
            WRITE_OWNER               = 0x00080000,

            /// <summary>
            /// (20)    SYNCHRONIZE
            /// </summary>
            SYNCHRONIZE               = 0x00100000,

            //  (21-23) 定義されていない（復合権限）

            #endregion

            #region 別名

            /// <summary>
            /// READ_CONTROLの別名
            /// </summary>
            STANDARD_RIGHTS_READ = ACCESS_MASK.READ_CONTROL,
            /// <summary>
            /// READ_CONTROLの別名
            /// </summary>
            STANDARD_RIGHTS_WRITE = ACCESS_MASK.READ_CONTROL,
            /// <summary>
            /// READ_CONTROLの別名
            /// </summary>
            STANDARD_RIGHTS_EXECUTE = ACCESS_MASK.READ_CONTROL,

            #endregion

            #region 組み合わせ
            /// <summary>
            /// 16-23 Standard rights. Contains the object's standard access rights.
            /// 上記16-19の権限を合わせたもの。
            /// </summary>
            STANDARD_RIGHTS_REQUIRED =
                ACCESS_MASK.DELETE | ACCESS_MASK.READ_CONTROL | ACCESS_MASK.WRITE_DAC | ACCESS_MASK.WRITE_OWNER,

            /// <summary>
            /// 16-23 Standard rights. Contains the object's standard access rights.
            /// 上記16-20の権限を合わせたもの、21-22 定義されていない（復合権限）
            /// </summary>
            STANDARD_RIGHTS_ALL =
                ACCESS_MASK.DELETE | ACCESS_MASK.READ_CONTROL | ACCESS_MASK.WRITE_DAC | ACCESS_MASK.WRITE_OWNER | ACCESS_MASK.SYNCHRONIZE,

            #endregion

            #endregion

            #region その他

            /// <summary>
            /// (24)    ACCESS_SYSTEM_SECURITY
            /// </summary>
            ACCESS_SYSTEM_SECURITY = 0x01000000,

            /// <summary>
            /// (25)    MAXIMUM_ALLOWED
            /// </summary>
            MAXIMUM_ALLOWED = 0x02000000,

            //  (26-27) 定義されていない（予約）

            #endregion

            #region Generic

            /// <summary>
            /// (28)    GENERIC_READ
            /// </summary>
            GENERIC_READ = 0x80000000,

            /// <summary>
            /// (29)    GENERIC_WRITE
            /// </summary>
            GENERIC_WRITE = 0x40000000,

            /// <summary>
            /// (30)    GENERIC_EXECUTE
            /// </summary>
            GENERIC_EXECUTE = 0x20000000,

            /// <summary>
            /// (31)    GENERIC_ALL
            /// </summary>
            GENERIC_ALL = 0x10000000

            #endregion
        }


        #endregion

        #region Win32 API

        #region セキュリティ・ログで使用

        /// <summary>プロセスに関連付けられているアクセストークンを開きます。</summary>
        /// <param name="processHandle">プロセスのハンドル</param>
        /// <param name="desiredAccess">プロセスに対して希望するアクセス権</param>
        /// <param name="tokenHandle">開かれたアクセストークンのハンドルへのポインタ</param>
        /// <returns>
        /// true：関数が成功
        /// false：関数が失敗
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(
            IntPtr processHandle, int desiredAccess, ref IntPtr tokenHandle);

        /// <summary>ローカル一意識別子（LUID）を取得し、指定された特権名をローカルで表現します。</summary>
        /// <param name="lpSystemName">システムを指定する文字列のアドレス</param>
        /// <param name="lpName">特権を指定する文字列のアドレス</param>
        /// <param name="lpLuid">ローカル一意識別子のアドレス</param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LookupPrivilegeValue(
            string lpSystemName, string lpName, ref long lpLuid);

        /// <summary>
        /// 指定したアクセストークン内の特権を有効または無効にします。
        /// TOKEN_ADJUST_PRIVILEGES アクセス権が必要です。
        /// </summary>
        /// <param name="TokenHandle">// 特権を保持するトークンのハンドル</param>
        /// <param name="DisableAllPrivileges">すべての特権を無効にするためのフラグ</param>
        /// <param name="NewState">新しい特権情報へのポインタ</param>
        /// <param name="BufferLength">PreviousState バッファのバイト単位のサイズ</param>
        /// <param name="PreviousState">変更を加えられた特権の元の状態を受け取る</param>
        /// <param name="ReturnLength">PreviousState バッファが必要とするサイズを受け取る</param>
        /// <returns>
        /// true：関数が成功
        /// false：関数が失敗
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(
            IntPtr TokenHandle,
            bool DisableAllPrivileges,
            ref TOKEN_PRIVILEGES NewState,
            int BufferLength,
            IntPtr PreviousState,
            IntPtr ReturnLength);

        #endregion

        #region 偽装で使用

        /// <summary>
        /// ユーザを表すトークンのハンドルを取得。
        /// このトークンハンドルを使って、指定したユーザーを偽装するか、
        /// 指定したユーザのコンテキスト内で動作するプロセスを作成できる。
        /// </summary>
        /// <param name="lpszUserName">ユーザ名</param>
        /// <param name="lpszDomain">ドメイン名</param>
        /// <param name="lpszPassword">パスワード</param>
        /// <param name="dwLogonType">ログオン動作のタイプ</param>
        /// <param name="dwLogonProvider">ログオンプロバイダ</param>
        /// <param name="phToken">トークンハンドル</param>
        /// <returns>
        /// ・0以外：成功
        /// ・0：失敗
        /// 拡張エラー情報を取得するには、
        /// GetLastError 関数を使用する。
        /// </returns>
        /// <remarks>
        /// LogonUser 関数
        /// http://msdn.microsoft.com/ja-jp/library/cc447468.aspx
        /// 
        /// ＜ログオン動作のタイプ＞
        /// ・LOGON32_LOGON_INTERACTIVE（2）：対話ユーザ 
        /// ・LOGON32_LOGON_NETWORK（3）：平文パスワード認証する高性能サーバ（偽装）
        /// ・LOGON32_LOGON_BATCH（4）：平文パスワード認証する高性能サーバ（バッチ）
        /// ・LOGON32_LOGON_SERVICE（5）：サービス（サービス特権が必要） 
        /// 
        /// ＜ログオン・プロバイダ＞
        /// ・LOGON32_PROVIDER_DEFAULT（0）：システム標準
        /// ・LOGON32_PROVIDER_WINNT35（1）：Windows NT 3.5
        /// ・LOGON32_PROVIDER_WINNT40（2）：Windows NT 4.0
        /// ・LOGON32_PROVIDER_WINNT50（3）：Windows 2000
        /// </remarks>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LogonUserA(
            string lpszUserName, string lpszDomain, string lpszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        /// <summary>
        /// 既存のアクセストークンを複製して、
        /// SetThreadToken、ImpersonateLoggedOnUser関数で
        /// 利用できる新しい偽装アクセストークンを作成する。
        /// </summary>
        /// <param name="hToken">複製するトークンのハンドル</param>
        /// <param name="impersonationLevel">偽装レベル</param>
        /// <param name="hNewToken">複製されたトークンのハンドル</param>
        /// <returns>
        /// ・0以外：成功
        /// ・0：失敗
        /// 拡張エラー情報を取得するには、
        /// GetLastError 関数を使用する。
        /// </returns>
        /// <remarks>
        /// DuplicateToken 関数
        /// http://msdn.microsoft.com/ja-jp/library/cc402013.aspx
        /// </remarks>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int DuplicateToken(
            IntPtr hToken,
            SECURITY_IMPERSONATION_LEVEL impersonationLevel,
            ref IntPtr hNewToken);

        /// <summary>
        /// 既存のアクセストークンを複製して、新しいアクセストークンを作成する。
        /// プライマリトークンと偽装トークンのどちらかを作成できる。
        /// </summary>
        /// <param name="ExistingTokenHandle">複製するトークンのハンドル</param>
        /// <param name="dwDesiredAccess">新しいトークンのアクセス権利</param>
        /// <param name="lpThreadAttributes">新しいトークンのセキュリティ属性</param>
        /// <param name="TokenType">トークン型（プライマリか偽装）</param>
        /// <param name="ImpersonationLevel">新しいトークンの偽装レベル</param>
        /// <param name="DuplicateTokenHandle">複製によって作成された新しいトークンのハンドル</param>
        /// <returns>
        /// ・0以外：成功
        /// ・0：失敗
        /// 拡張エラー情報を取得するには、
        /// GetLastError 関数を使用する。
        /// </returns>
        /// <remarks>
        /// DuplicateTokenEx 関数
        /// http://msdn.microsoft.com/ja-jp/library/cc402016.aspx
        /// </remarks>
        [DllImport("advapi32.dll", SetLastError = true)]
        public extern static bool DuplicateTokenEx(
            IntPtr ExistingTokenHandle,
            uint dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpThreadAttributes,
            int TokenType,
            int ImpersonationLevel,
            ref IntPtr DuplicateTokenHandle);

        /// <summary>
        /// クライアントアプリケーションによる偽装を終了。
        /// </summary>
        /// <returns>
        /// ・true：成功
        /// ・false：失敗
        /// 拡張エラー情報を取得するには、
        /// GetLastError 関数を使用する。
        /// </returns>
        /// <remarks>
        /// RevertToSelf 関数
        /// http://msdn.microsoft.com/ja-jp/library/cc447534.aspx
        /// 
        /// ・DdeImpersonateClient、
        /// ・ImpersonateDdeClientWindow、
        /// ・ImpersonateLoggedOnUser、
        /// ・ImpersonateNamedPipeClient、
        /// ・ImpersonateSelf、
        /// ・SetThreadToken
        /// のいずれかの関数を使って偽装を開始した後、
        /// RevertToSelf 関数を呼び出して偽装を終了します。
        /// </remarks>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        #endregion

        #endregion
    }
}

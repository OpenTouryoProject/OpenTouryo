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
//* クラス名        ：CmnWin32
//* クラス日本語名  ：Win32 API宣言クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2010/10/26  西野  大介        新規作成
//*  2013/02/18  西野  大介        SetLastError対応
//**********************************************************************************

using System;
using System.Runtime.InteropServices;

namespace Touryo.Infrastructure.Public.Win32
{
    #region 共通構造体

    /// <summary>
    /// POINT構造体
    /// http://msdn.microsoft.com/ja-jp/library/vstudio/8kk2sy33.aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary>
        /// X座標
        /// </summary>
        public int x;
        /// <summary>
        /// Y座標
        /// </summary>
        public int y;
    }

    /// <summary>
    /// SYSTEMTIME構造体
    /// http://msdn.microsoft.com/ja-jp/library/vstudio/tc6fd5zs.aspx
    /// </summary>
    /// <remarks>
    /// C言語構造体と同様に、メンバが宣言された順にメモリに配置するには、
    /// StructLayoutにLayoutKind.Sequentialという値を指定する。
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEMTIME
    {
        // UnmanagedType 列挙体で
        // アンマネージ コードにマーシャリングする方法を指定する。
        // http://msdn.microsoft.com/ja-jp/library/system.runtime.interopservices.unmanagedtype.aspx

        /// <summary>年</summary>
        [MarshalAs(UnmanagedType.U2)]
        public short Year;

        /// <summary>月</summary>
        [MarshalAs(UnmanagedType.U2)]
        public short Month;

        /// <summary>曜日</summary>
        [MarshalAs(UnmanagedType.U2)]
        public short DayOfWeek;

        /// <summary>日</summary>
        [MarshalAs(UnmanagedType.U2)]
        public short Day;

        /// <summary>時</summary>
        [MarshalAs(UnmanagedType.U2)]
        public short Hour;

        /// <summary>分</summary>
        [MarshalAs(UnmanagedType.U2)]
        public short Minute;

        /// <summary>秒</summary>
        [MarshalAs(UnmanagedType.U2)]
        public short Second;

        /// <summary>ミリ秒</summary>
        [MarshalAs(UnmanagedType.U2)]
        public short Milliseconds;

        /// <summary>コンストラクタ</summary>
        public SYSTEMTIME(DateTime dt)
        {
            dt = dt.ToUniversalTime();  // SetSystemTime expects the SYSTEMTIME in UTC

            Year = (short)dt.Year;
            Month = (short)dt.Month;
            DayOfWeek = (short)dt.DayOfWeek;
            Day = (short)dt.Day;
            Hour = (short)dt.Hour;
            Minute = (short)dt.Minute;
            Second = (short)dt.Second;
            Milliseconds = (short)dt.Millisecond;
        }
    }

    #endregion

    #region 共通 Win32 API

    /// <summary>
    /// Win32 API宣言クラス
    /// </summary>
    public class CmnWin32
    {
        #region GetXXXX関数

        /// <summary>Win32 API関数（GetCurrentThread）のプロトタイプ宣言</summary>
        /// <returns>カレントスレッドのハンドラ</returns>
        /// <remarks>Win32 API関数（GetThreadTimes）の引数に必要な、カレントスレッドのハンドラを取得する。</remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentThread();

        /// <summary>現在のシステム日時（UTC）を取得</summary>
        /// <param name="st">現在のシステム日時（UTC）</param>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void GetSystemTime(out SYSTEMTIME st);

        /// <summary>現在のローカル日時を取得</summary>
        /// <param name="st">現在のローカル日時</param>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void GetLocalTime(out SYSTEMTIME st);

        #endregion

        #region CloseHandle関数

        /// <summary>
        /// 開いているオブジェクトハンドルを閉じる。
        /// </summary>
        /// <param name="hObject">
        /// 開いているオブジェクトハンドルを指定する。
        /// 閉じることに成功した場合はカウントが 1 減る。
        /// </param>
        /// <returns>
        /// ● 関数が成功すると、true が返る。
        /// ● 関数が失敗すると、false が返る。
        /// ● 拡張エラー情報を取得するには、GetLastError 関数を使う。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        #endregion

        #region GetLastError関数

        // …と同等のP/Invoke用メソッド
        // DWORD GetLastError(VOID);

        /// <summary>
        /// 最終アンマネージ関数によって返されるエラー コードを返す。
        /// Marshal.GetLastWin32Errorメソッド → Win32 GetLastError APIメソッド
        /// </summary>
        /// <remarks>
        /// Win32 GetLastError APIメソッドを直接使用できない理由も含め、
        /// 詳細は http://msdn.microsoft.com/ja-jp/library/system.runtime.interopservices.marshal.getlastwin32error(v=VS.90).aspx を参照
        /// </remarks>
        /// <returns>
        /// Win32 GetLastError APIメソッドへの呼び出しで最後に設定されたエラー コード。
        /// </returns>
        public static ErrorCodes GetLastError()
        {
            return (ErrorCodes)Marshal.GetLastWin32Error();
        }

        #region GetLastError のエラーコード

        // 2009/12/15 04:13:14付の以下ヘッダファイルより
        //   C:\Program Files\Microsoft SDKs\Windows\v7.0A\Include\WinError.h

        /// <summary>エラーコード</summary>
        public enum ErrorCodes : int
        {
            /// <summary>#define ERROR_SUCCESS 0L</summary>
            ERROR_SUCCESS = 0,
            /// <summary>#define NO_ERROR 0L</summary>
            NO_ERROR = 0,
            /// <summary>#define SEC_E_OK 0x00000000L</summary>
            SEC_E_OK = 0,
            /// <summary>#define ERROR_INVALID_FUNCTION 1L</summary>
            ERROR_INVALID_FUNCTION = 1,
            /// <summary>#define ERROR_FILE_NOT_FOUND 2L</summary>
            ERROR_FILE_NOT_FOUND = 2,
            /// <summary>#define ERROR_PATH_NOT_FOUND 3L</summary>
            ERROR_PATH_NOT_FOUND = 3,
            /// <summary>#define ERROR_TOO_MANY_OPEN_FILES 4L</summary>
            ERROR_TOO_MANY_OPEN_FILES = 4,
            /// <summary>#define ERROR_ACCESS_DENIED 5L</summary>
            ERROR_ACCESS_DENIED = 5,
            /// <summary>#define ERROR_INVALID_HANDLE 6L</summary>
            ERROR_INVALID_HANDLE = 6,
            /// <summary>#define ERROR_ARENA_TRASHED 7L</summary>
            ERROR_ARENA_TRASHED = 7,
            /// <summary>#define ERROR_NOT_ENOUGH_MEMORY 8L</summary>
            ERROR_NOT_ENOUGH_MEMORY = 8,
            /// <summary>#define ERROR_INVALID_BLOCK 9L</summary>
            ERROR_INVALID_BLOCK = 9,
            /// <summary>#define ERROR_BAD_ENVIRONMENT 10L</summary>
            ERROR_BAD_ENVIRONMENT = 10, // 
            /// <summary>#define ERROR_BAD_FORMAT 11L</summary>
            ERROR_BAD_FORMAT = 11, // 
            /// <summary>#define ERROR_INVALID_ACCESS 12L</summary>
            ERROR_INVALID_ACCESS = 12,
            /// <summary>#define ERROR_INVALID_DATA 13L</summary>
            ERROR_INVALID_DATA = 13,
            /// <summary>#define ERROR_OUTOFMEMORY 14L</summary>
            ERROR_OUTOFMEMORY = 14,
            /// <summary>#define ERROR_INVALID_DRIVE 15L</summary>
            ERROR_INVALID_DRIVE = 15,
            /// <summary>#define ERROR_CURRENT_DIRECTORY 16L</summary>
            ERROR_CURRENT_DIRECTORY = 16,
            /// <summary>#define ERROR_NOT_SAME_DEVICE 17L</summary>
            ERROR_NOT_SAME_DEVICE = 17,
            /// <summary>#define ERROR_NO_MORE_FILES 18L</summary>
            ERROR_NO_MORE_FILES = 18,
            /// <summary>#define ERROR_WRITE_PROTECT 19L</summary>
            ERROR_WRITE_PROTECT = 19,
            /// <summary>#define ERROR_BAD_UNIT 20L</summary>
            ERROR_BAD_UNIT = 20,
            /// <summary>#define ERROR_NOT_READY 21L</summary>
            ERROR_NOT_READY = 21,
            /// <summary>#define ERROR_BAD_COMMAND 22L</summary>
            ERROR_BAD_COMMAND = 22,
            /// <summary>#define ERROR_CRC 23L</summary>
            ERROR_CRC = 23,
            /// <summary>#define ERROR_BAD_LENGTH 24L</summary>
            ERROR_BAD_LENGTH = 24,
            /// <summary>#define ERROR_SEEK 25L</summary>
            ERROR_SEEK = 25,
            /// <summary>#define ERROR_NOT_DOS_DISK 26L</summary>
            ERROR_NOT_DOS_DISK = 26,
            /// <summary>#define ERROR_SECTOR_NOT_FOUND 27L</summary>
            ERROR_SECTOR_NOT_FOUND = 27,
            /// <summary>#define ERROR_OUT_OF_PAPER 28L</summary>
            ERROR_OUT_OF_PAPER = 28,
            /// <summary>#define ERROR_WRITE_FAULT 29L</summary>
            ERROR_WRITE_FAULT = 29,
            /// <summary>#define ERROR_READ_FAULT 30L</summary>
            ERROR_READ_FAULT = 30,
            /// <summary>#define ERROR_GEN_FAILURE 31L</summary>
            ERROR_GEN_FAILURE = 31,
            /// <summary>#define ERROR_SHARING_VIOLATION 32L</summary>
            ERROR_SHARING_VIOLATION = 32,
            /// <summary>#define ERROR_LOCK_VIOLATION 33L</summary>
            ERROR_LOCK_VIOLATION = 33,
            /// <summary>#define ERROR_WRONG_DISK 34L</summary>
            ERROR_WRONG_DISK = 34,
            /// <summary>#define ERROR_SHARING_BUFFER_EXCEEDED 36L</summary>
            ERROR_SHARING_BUFFER_EXCEEDED = 36,
            /// <summary>#define ERROR_HANDLE_EOF 38L</summary>
            ERROR_HANDLE_EOF = 38,
            /// <summary>#define ERROR_HANDLE_DISK_FULL 39L</summary>
            ERROR_HANDLE_DISK_FULL = 39,
            /// <summary>#define ERROR_NOT_SUPPORTED 50L</summary>
            ERROR_NOT_SUPPORTED = 50,
            /// <summary>#define ERROR_REM_NOT_LIST 51L</summary>
            ERROR_REM_NOT_LIST = 51,
            /// <summary>#define ERROR_DUP_NAME 52L</summary>
            ERROR_DUP_NAME = 52,
            /// <summary>#define ERROR_BAD_NETPATH 53L</summary>
            ERROR_BAD_NETPATH = 53,
            /// <summary>#define ERROR_NETWORK_BUSY 54L</summary>
            ERROR_NETWORK_BUSY = 54,
            /// <summary>#define ERROR_DEV_NOT_EXIST 55L</summary>
            ERROR_DEV_NOT_EXIST = 55,
            /// <summary>#define ERROR_TOO_MANY_CMDS 56L</summary>
            ERROR_TOO_MANY_CMDS = 56,
            /// <summary>#define ERROR_ADAP_HDW_ERR 57L</summary>
            ERROR_ADAP_HDW_ERR = 57,
            /// <summary>#define ERROR_BAD_NET_RESP 58L</summary>
            ERROR_BAD_NET_RESP = 58,
            /// <summary>#define ERROR_UNEXP_NET_ERR 59L</summary>
            ERROR_UNEXP_NET_ERR = 59,
            /// <summary>#define ERROR_BAD_REM_ADAP 60L</summary>
            ERROR_BAD_REM_ADAP = 60,
            /// <summary>#define ERROR_PRINTQ_FULL 61L</summary>
            ERROR_PRINTQ_FULL = 61,
            /// <summary>#define ERROR_NO_SPOOL_SPACE 62L</summary>
            ERROR_NO_SPOOL_SPACE = 62,
            /// <summary>#define ERROR_PRINT_CANCELLED 63L</summary>
            ERROR_PRINT_CANCELLED = 63,
            /// <summary>#define ERROR_NETNAME_DELETED 64L</summary>
            ERROR_NETNAME_DELETED = 64,
            /// <summary>#define ERROR_NETWORK_ACCESS_DENIED 65L</summary>
            ERROR_NETWORK_ACCESS_DENIED = 65,
            /// <summary>#define ERROR_BAD_DEV_TYPE 66L</summary>
            ERROR_BAD_DEV_TYPE = 66,
            /// <summary>#define ERROR_BAD_NET_NAME 67L</summary>
            ERROR_BAD_NET_NAME = 67,
            /// <summary>#define ERROR_TOO_MANY_NAMES 68L</summary>
            ERROR_TOO_MANY_NAMES = 68,
            /// <summary>#define ERROR_TOO_MANY_SESS 69L</summary>
            ERROR_TOO_MANY_SESS = 69,
            /// <summary>#define ERROR_SHARING_PAUSED 70L</summary>
            ERROR_SHARING_PAUSED = 70,
            /// <summary>#define ERROR_REQ_NOT_ACCEP 71L</summary>
            ERROR_REQ_NOT_ACCEP = 71,
            /// <summary>#define ERROR_REDIR_PAUSED 72L</summary>
            ERROR_REDIR_PAUSED = 72,
            /// <summary>#define ERROR_FILE_EXISTS 80L</summary>
            ERROR_FILE_EXISTS = 80,
            /// <summary>#define ERROR_CANNOT_MAKE 82L</summary>
            ERROR_CANNOT_MAKE = 82, //
            /// <summary>#define ERROR_FAIL_I24 83L</summary>
            ERROR_FAIL_I24 = 83,
            /// <summary>#define ERROR_OUT_OF_STRUCTURES 84L</summary>
            ERROR_OUT_OF_STRUCTURES = 84,
            /// <summary>#define ERROR_ALREADY_ASSIGNED 85L</summary>
            ERROR_ALREADY_ASSIGNED = 85,
            /// <summary>#define ERROR_INVALID_PASSWORD 86L</summary>
            ERROR_INVALID_PASSWORD = 86,
            /// <summary>#define ERROR_INVALID_PARAMETER 87L</summary>
            ERROR_INVALID_PARAMETER = 87,
            /// <summary>#define ERROR_NET_WRITE_FAULT 88L</summary>
            ERROR_NET_WRITE_FAULT = 88,
            /// <summary>#define ERROR_NO_PROC_SLOTS 89L</summary>
            ERROR_NO_PROC_SLOTS = 89,
            /// <summary>#define ERROR_TOO_MANY_SEMAPHORES 100L</summary>
            ERROR_TOO_MANY_SEMAPHORES = 100,
            /// <summary>#define ERROR_EXCL_SEM_ALREADY_OWNED 101L</summary>
            ERROR_EXCL_SEM_ALREADY_OWNED = 101,
            /// <summary>#define ERROR_SEM_IS_SET 102L</summary>
            ERROR_SEM_IS_SET = 102,
            /// <summary>#define ERROR_TOO_MANY_SEM_REQUESTS 103L</summary>
            ERROR_TOO_MANY_SEM_REQUESTS = 103,
            /// <summary>#define ERROR_INVALID_AT_INTERRUPT_TIME 104L</summary>
            ERROR_INVALID_AT_INTERRUPT_TIME = 104,
            /// <summary>#define ERROR_SEM_OWNER_DIED 105L</summary>
            ERROR_SEM_OWNER_DIED = 105,
            /// <summary>#define ERROR_SEM_USER_LIMIT 106L</summary>
            ERROR_SEM_USER_LIMIT = 106,
            /// <summary>#define ERROR_DISK_CHANGE 107L</summary>
            ERROR_DISK_CHANGE = 107,
            /// <summary>#define ERROR_DRIVE_LOCKED 108L</summary>
            ERROR_DRIVE_LOCKED = 108,
            /// <summary>#define ERROR_BROKEN_PIPE 109L</summary>
            ERROR_BROKEN_PIPE = 109,
            /// <summary>#define ERROR_OPEN_FAILED 110L</summary>
            ERROR_OPEN_FAILED = 110,
            /// <summary>#define ERROR_BUFFER_OVERFLOW 111L</summary>
            ERROR_BUFFER_OVERFLOW = 111,
            /// <summary>#define ERROR_DISK_FULL 112L</summary>
            ERROR_DISK_FULL = 112,
            /// <summary>// #define ERROR_NO_MORE_SEARCH_HANDLES 113L</summary>
            ERROR_NO_MORE_SEARCH_HANDLES = 113,
            /// <summary>#define ERROR_INVALID_TARGET_HANDLE 114L</summary>
            ERROR_INVALID_TARGET_HANDLE = 114,
            /// <summary>#define ERROR_INVALID_CATEGORY 117L</summary>
            ERROR_INVALID_CATEGORY = 117,
            /// <summary>#define ERROR_INVALID_VERIFY_SWITCH 118L</summary>
            ERROR_INVALID_VERIFY_SWITCH = 118,
            /// <summary>#define ERROR_BAD_DRIVER_LEVEL 119L</summary>
            ERROR_BAD_DRIVER_LEVEL = 119,
            /// <summary>#define ERROR_CALL_NOT_IMPLEMENTED 120L</summary>
            ERROR_CALL_NOT_IMPLEMENTED = 120,
            /// <summary>#define ERROR_SEM_TIMEOUT 121L</summary>
            ERROR_SEM_TIMEOUT = 121,
            /// <summary>#define ERROR_INSUFFICIENT_BUFFER 122L</summary>
            ERROR_INSUFFICIENT_BUFFER = 122,
            /// <summary>#define ERROR_INVALID_NAME 123L</summary>
            ERROR_INVALID_NAME = 123,
            /// <summary>#define ERROR_INVALID_LEVEL 124L</summary>
            ERROR_INVALID_LEVEL = 124,
            /// <summary>#define ERROR_NO_VOLUME_LABEL 125L</summary>
            ERROR_NO_VOLUME_LABEL = 125,
            /// <summary>#define ERROR_MOD_NOT_FOUND 126L</summary>
            ERROR_MOD_NOT_FOUND = 126,
            /// <summary>#define ERROR_PROC_NOT_FOUND 127L</summary>
            ERROR_PROC_NOT_FOUND = 127,
            /// <summary>#define ERROR_WAIT_NO_CHILDREN 128L</summary>
            ERROR_WAIT_NO_CHILDREN = 128,
            /// <summary>#define ERROR_CHILD_NOT_COMPLETE 129L</summary>
            ERROR_CHILD_NOT_COMPLETE = 129,
            /// <summary>#define ERROR_DIRECT_ACCESS_HANDLE 130L</summary>
            ERROR_DIRECT_ACCESS_HANDLE = 130,
            /// <summary>#define ERROR_NEGATIVE_SEEK 131L</summary>
            ERROR_NEGATIVE_SEEK = 131,
            /// <summary>#define ERROR_SEEK_ON_DEVICE 132L</summary>
            ERROR_SEEK_ON_DEVICE = 132,
            /// <summary>#define ERROR_IS_JOIN_TARGET 133L</summary>
            ERROR_IS_JOIN_TARGET = 133,
            /// <summary>#define ERROR_IS_JOINED 134L</summary>
            ERROR_IS_JOINED = 134,
            /// <summary>#define ERROR_IS_SUBSTED 135L</summary>
            ERROR_IS_SUBSTED = 135,
            /// <summary>#define ERROR_NOT_JOINED 136L</summary>
            ERROR_NOT_JOINED = 136,
            /// <summary>#define ERROR_NOT_SUBSTED 137L</summary>
            ERROR_NOT_SUBSTED = 137,
            /// <summary>#define ERROR_JOIN_TO_JOIN 138L</summary>
            ERROR_JOIN_TO_JOIN = 138,
            /// <summary>#define ERROR_SUBST_TO_SUBST 139L</summary>
            ERROR_SUBST_TO_SUBST = 139,
            /// <summary>#define ERROR_JOIN_TO_SUBST 140L</summary>
            ERROR_JOIN_TO_SUBST = 140,
            /// <summary>#define ERROR_SUBST_TO_JOIN 141L</summary>
            ERROR_SUBST_TO_JOIN = 141,
            /// <summary>#define ERROR_BUSY_DRIVE 142L</summary>
            ERROR_BUSY_DRIVE = 142,
            /// <summary>#define ERROR_SAME_DRIVE 143L</summary>
            ERROR_SAME_DRIVE = 143,
            /// <summary>#define ERROR_DIR_NOT_ROOT 144L</summary>
            ERROR_DIR_NOT_ROOT = 144,
            /// <summary>#define ERROR_DIR_NOT_EMPTY 145L</summary>
            ERROR_DIR_NOT_EMPTY = 145,
            /// <summary>#define ERROR_IS_SUBST_PATH 146L</summary>
            ERROR_IS_SUBST_PATH = 146,
            /// <summary>#define ERROR_IS_JOIN_PATH 147L</summary>
            ERROR_IS_JOIN_PATH = 147,
            /// <summary>#define ERROR_PATH_BUSY 148L</summary>
            ERROR_PATH_BUSY = 148,
            /// <summary>#define ERROR_IS_SUBST_TARGET 149L</summary>
            ERROR_IS_SUBST_TARGET = 149,
            /// <summary>#define ERROR_SYSTEM_TRACE 150L</summary>
            ERROR_SYSTEM_TRACE = 150,
            /// <summary>#define ERROR_INVALID_EVENT_COUNT 151L</summary>
            ERROR_INVALID_EVENT_COUNT = 151,
            /// <summary>#define ERROR_TOO_MANY_MUXWAITERS 152L</summary>
            ERROR_TOO_MANY_MUXWAITERS = 152,
            /// <summary>#define ERROR_INVALID_LIST_FORMAT 153L</summary>
            ERROR_INVALID_LIST_FORMAT = 153,
            /// <summary>#define ERROR_LABEL_TOO_LONG 154L</summary>
            ERROR_LABEL_TOO_LONG = 154,
            /// <summary>#define ERROR_TOO_MANY_TCBS 155L</summary>
            ERROR_TOO_MANY_TCBS = 155,
            /// <summary>#define ERROR_SIGNAL_REFUSED 156L</summary>
            ERROR_SIGNAL_REFUSED = 156,
            /// <summary>#define ERROR_DISCARDED 157L</summary>
            ERROR_DISCARDED = 157,
            /// <summary>#define ERROR_NOT_LOCKED 158L</summary>
            ERROR_NOT_LOCKED = 158,
            /// <summary>#define ERROR_BAD_THREADID_ADDR 159L</summary>
            ERROR_BAD_THREADID_ADDR = 159,
            /// <summary>#define ERROR_BAD_ARGUMENTS 160L</summary>
            ERROR_BAD_ARGUMENTS = 160,
            /// <summary>#define ERROR_BAD_PATHNAME 161L</summary>
            ERROR_BAD_PATHNAME = 161,
            /// <summary>#define ERROR_SIGNAL_PENDING 162L</summary>
            ERROR_SIGNAL_PENDING = 162,
            /// <summary>// #define ERROR_MAX_THRDS_REACHED 164L</summary>
            ERROR_MAX_THRDS_REACHED = 164,
            /// <summary>#define ERROR_LOCK_FAILED 167L</summary>
            ERROR_LOCK_FAILED = 167,
            /// <summary>#define ERROR_BUSY 170L</summary>
            ERROR_BUSY = 170,
            /// <summary>#define ERROR_CANCEL_VIOLATION 173L</summary>
            ERROR_CANCEL_VIOLATION = 173,
            /// <summary>#define ERROR_ATOMIC_LOCKS_NOT_SUPPORTED 174L</summary>
            ERROR_ATOMIC_LOCKS_NOT_SUPPORTED = 174,
            /// <summary>#define ERROR_INVALID_SEGMENT_NUMBER 180L</summary>
            ERROR_INVALID_SEGMENT_NUMBER = 180,
            /// <summary>#define ERROR_INVALID_ORDINAL 182L</summary>
            ERROR_INVALID_ORDINAL = 182,
            /// <summary>#define ERROR_ALREADY_EXISTS 183L</summary>
            ERROR_ALREADY_EXISTS = 183,
            /// <summary>#define ERROR_INVALID_FLAG_NUMBER 186L</summary>
            ERROR_INVALID_FLAG_NUMBER = 186,
            /// <summary>#define ERROR_SEM_NOT_FOUND 187L</summary>
            ERROR_SEM_NOT_FOUND = 187,
            /// <summary>#define ERROR_INVALID_STARTING_CODESEG 188L</summary>
            ERROR_INVALID_STARTING_CODESEG = 188,
            /// <summary>#define ERROR_INVALID_STACKSEG 189L</summary>
            ERROR_INVALID_STACKSEG = 189,
            /// <summary>#define ERROR_INVALID_MODULETYPE 190L</summary>
            ERROR_INVALID_MODULETYPE = 190,
            /// <summary>#define ERROR_INVALID_EXE_SIGNATURE 191L</summary>
            ERROR_INVALID_EXE_SIGNATURE = 191,
            /// <summary>#define ERROR_EXE_MARKED_INVALID 192L</summary>
            ERROR_EXE_MARKED_INVALID = 192,
            /// <summary>#define ERROR_BAD_EXE_FORMAT 193L</summary>
            ERROR_BAD_EXE_FORMAT = 193,
            /// <summary>#define ERROR_ITERATED_DATA_EXCEEDS_64k 194L</summary>
            ERROR_ITERATED_DATA_EXCEEDS_64k = 194,
            /// <summary>#define ERROR_INVALID_MINALLOCSIZE 195L</summary>
            ERROR_INVALID_MINALLOCSIZE = 195,
            /// <summary>#define ERROR_DYNLINK_FROM_INVALID_RING 196L</summary>
            ERROR_DYNLINK_FROM_INVALID_RING = 196,
            /// <summary>#define ERROR_IOPL_NOT_ENABLED 197L</summary>
            ERROR_IOPL_NOT_ENABLED = 197,
            /// <summary>#define ERROR_INVALID_SEGDPL 198L</summary>
            ERROR_INVALID_SEGDPL = 198,
            /// <summary>#define ERROR_AUTODATASEG_EXCEEDS_64k 199L</summary>
            ERROR_AUTODATASEG_EXCEEDS_64k = 199,
            /// <summary>#define ERROR_RING2SEG_MUST_BE_MOVABLE 200L</summary>
            ERROR_RING2SEG_MUST_BE_MOVABLE = 200,
            /// <summary>#define ERROR_RELOC_CHAIN_XEEDS_SEGLIM 201L</summary>
            ERROR_RELOC_CHAIN_XEEDS_SEGLIM = 201,
            /// <summary>#define ERROR_INFLOOP_IN_RELOC_CHAIN 202L</summary>
            ERROR_INFLOOP_IN_RELOC_CHAIN = 202,
            /// <summary>#define ERROR_ENVVAR_NOT_FOUND 203L</summary>
            ERROR_ENVVAR_NOT_FOUND = 203,
            /// <summary>#define ERROR_NO_SIGNAL_SENT 205L</summary>
            ERROR_NO_SIGNAL_SENT = 205,
            /// <summary>#define ERROR_FILENAME_EXCED_RANGE 206L</summary>
            ERROR_FILENAME_EXCED_RANGE = 206,
            /// <summary>#define ERROR_RING2_STACK_IN_USE 207L</summary>
            ERROR_RING2_STACK_IN_USE = 207,
            /// <summary>#define ERROR_META_EXPANSION_TOO_LONG 208L</summary>
            ERROR_META_EXPANSION_TOO_LONG = 208,
            /// <summary>#define ERROR_INVALID_SIGNAL_NUMBER 209L</summary>
            ERROR_INVALID_SIGNAL_NUMBER = 209,
            /// <summary>#define ERROR_THREAD_1_INACTIVE 210L</summary>
            ERROR_THREAD_1_INACTIVE = 210,
            /// <summary>#define ERROR_LOCKED 212L</summary>
            ERROR_LOCKED = 212,
            /// <summary>#define ERROR_TOO_MANY_MODULES 214L</summary>
            ERROR_TOO_MANY_MODULES = 214,
            /// <summary>#define ERROR_NESTING_NOT_ALLOWED 215L</summary>
            ERROR_NESTING_NOT_ALLOWED = 215,
            /// <summary>#define ERROR_EXE_MACHINE_TYPE_MISMATCH 216L</summary>
            ERROR_EXE_MACHINE_TYPE_MISMATCH = 216,
            /// <summary>#define ERROR_EXE_CANNOT_MODIFY_SIGNED_BINARY 217L</summary>
            ERROR_EXE_CANNOT_MODIFY_SIGNED_BINARY = 217,
            /// <summary>#define ERROR_EXE_CANNOT_MODIFY_STRONG_SIGNED_BINARY 218L</summary>
            ERROR_EXE_CANNOT_MODIFY_STRONG_SIGNED_BINARY = 218,
            /// <summary>#define ERROR_FILE_CHECKED_OUT 220L</summary>
            ERROR_FILE_CHECKED_OUT = 220,
            /// <summary>#define ERROR_CHECKOUT_REQUIRED 221L</summary>
            ERROR_CHECKOUT_REQUIRED = 221,
            /// <summary>#define ERROR_BAD_FILE_TYPE 222L</summary>
            ERROR_BAD_FILE_TYPE = 222,
            /// <summary>#define ERROR_FILE_TOO_LARGE 223L</summary>
            ERROR_FILE_TOO_LARGE = 223,
            /// <summary>#define ERROR_FORMS_AUTH_REQUIRED 224L</summary>
            ERROR_FORMS_AUTH_REQUIRED = 224,
            /// <summary>#define ERROR_VIRUS_INFECTED 225L</summary>
            ERROR_VIRUS_INFECTED = 225,
            /// <summary>#define ERROR_VIRUS_DELETED 226L</summary>
            ERROR_VIRUS_DELETED = 226,
            /// <summary>#define ERROR_PIPE_LOCAL 229L</summary>
            ERROR_PIPE_LOCAL = 229,
            /// <summary>#define ERROR_BAD_PIPE 230L</summary>
            ERROR_BAD_PIPE = 230,
            /// <summary>#define ERROR_PIPE_BUSY 231L</summary>
            ERROR_PIPE_BUSY = 231,
            /// <summary>#define ERROR_NO_DATA 232L</summary>
            ERROR_NO_DATA = 232,
            /// <summary>#define ERROR_PIPE_NOT_CONNECTED 233L</summary>
            ERROR_PIPE_NOT_CONNECTED = 233,
            /// <summary>#define ERROR_MORE_DATA 234L</summary>
            ERROR_MORE_DATA = 234,
            /// <summary>#define ERROR_VC_DISCONNECTED 240L</summary>
            ERROR_VC_DISCONNECTED = 240,
            /// <summary>#define ERROR_INVALID_EA_NAME 254L</summary>
            ERROR_INVALID_EA_NAME = 254,
            /// <summary>#define ERROR_EA_LIST_INCONSISTENT 255L</summary>
            ERROR_EA_LIST_INCONSISTENT = 255,
            /// <summary>#define WAIT_TIMEOUT 258L</summary>
            WAIT_TIMEOUT = 258,
            /// <summary>#define ERROR_NO_MORE_ITEMS 259L</summary>
            ERROR_NO_MORE_ITEMS = 259,
            /// <summary>#define ERROR_CANNOT_COPY 266L</summary>
            ERROR_CANNOT_COPY = 266,
            /// <summary>#define ERROR_DIRECTORY 267L</summary>
            ERROR_DIRECTORY = 267,
            /// <summary>#define ERROR_EAS_DIDNT_FIT 275L</summary>
            ERROR_EAS_DIDNT_FIT = 275,
            /// <summary>#define ERROR_EA_FILE_CORRUPT 276L</summary>
            ERROR_EA_FILE_CORRUPT = 276,
            /// <summary>#define ERROR_EA_TABLE_FULL 277L</summary>
            ERROR_EA_TABLE_FULL = 277,
            /// <summary>#define ERROR_INVALID_EA_HANDLE 278L</summary>
            ERROR_INVALID_EA_HANDLE = 278,
            /// <summary>#define ERROR_EAS_NOT_SUPPORTED 282L</summary>
            ERROR_EAS_NOT_SUPPORTED = 282,
            /// <summary>#define ERROR_NOT_OWNER 288L</summary>
            ERROR_NOT_OWNER = 288,
            /// <summary>#define ERROR_TOO_MANY_POSTS 298L</summary>
            ERROR_TOO_MANY_POSTS = 298,
            /// <summary>#define ERROR_PARTIAL_COPY 299L</summary>
            ERROR_PARTIAL_COPY = 299,
            /// <summary>#define ERROR_OPLOCK_NOT_GRANTED 300L</summary>
            ERROR_OPLOCK_NOT_GRANTED = 300,
            /// <summary>#define ERROR_INVALID_OPLOCK_PROTOCOL 301L</summary>
            ERROR_INVALID_OPLOCK_PROTOCOL = 301,
            /// <summary>#define ERROR_DISK_TOO_FRAGMENTED 302L</summary>
            ERROR_DISK_TOO_FRAGMENTED = 302,
            /// <summary>#define ERROR_DELETE_PENDING 303L</summary>
            ERROR_DELETE_PENDING = 303,
            /// <summary>#define ERROR_INCOMPATIBLE_WITH_GLOBAL_SHORT_NAME_REGISTRY_SETTING 304L</summary>
            ERROR_INCOMPATIBLE_WITH_GLOBAL_SHORT_NAME_REGISTRY_SETTING = 304,
            /// <summary>#define ERROR_SHORT_NAMES_NOT_ENABLED_ON_VOLUME 305L</summary>
            ERROR_SHORT_NAMES_NOT_ENABLED_ON_VOLUME = 305,
            /// <summary>#define ERROR_SECURITY_STREAM_IS_INCONSISTENT 306L</summary>
            ERROR_SECURITY_STREAM_IS_INCONSISTENT = 306,
            /// <summary>#define ERROR_INVALID_LOCK_RANGE 307L</summary>
            ERROR_INVALID_LOCK_RANGE = 307,
            /// <summary>#define ERROR_IMAGE_SUBSYSTEM_NOT_PRESENT 308L</summary>
            ERROR_IMAGE_SUBSYSTEM_NOT_PRESENT = 308,
            /// <summary>#define ERROR_NOTIFICATION_GUID_ALREADY_DEFINED 309L</summary>
            ERROR_NOTIFICATION_GUID_ALREADY_DEFINED = 309,
            /// <summary>#define ERROR_MR_MID_NOT_FOUND 317L</summary>
            ERROR_MR_MID_NOT_FOUND = 317,
            /// <summary>#define ERROR_SCOPE_NOT_FOUND 318L</summary>
            ERROR_SCOPE_NOT_FOUND = 318,
            /// <summary>#define ERROR_FAIL_NOACTION_REBOOT 350L</summary>
            ERROR_FAIL_NOACTION_REBOOT = 350,
            /// <summary>#define ERROR_FAIL_SHUTDOWN 351L</summary>
            ERROR_FAIL_SHUTDOWN = 351,
            /// <summary>#define ERROR_FAIL_RESTART 352L</summary>
            ERROR_FAIL_RESTART = 352,
            /// <summary>#define ERROR_MAX_SESSIONS_REACHED 353L</summary>
            ERROR_MAX_SESSIONS_REACHED = 353,
            /// <summary>#define ERROR_THREAD_MODE_ALREADY_BACKGROUND 400L</summary>
            ERROR_THREAD_MODE_ALREADY_BACKGROUND = 400,
            /// <summary>#define ERROR_THREAD_MODE_NOT_BACKGROUND 401L</summary>
            ERROR_THREAD_MODE_NOT_BACKGROUND = 401,
            /// <summary>#define ERROR_PROCESS_MODE_ALREADY_BACKGROUND 402L</summary>
            ERROR_PROCESS_MODE_ALREADY_BACKGROUND = 402,
            /// <summary>#define ERROR_PROCESS_MODE_NOT_BACKGROUND 403L</summary>
            ERROR_PROCESS_MODE_NOT_BACKGROUND = 403,
            /// <summary>#define ERROR_INVALID_ADDRESS 487L</summary>
            ERROR_INVALID_ADDRESS = 487,
            /// <summary>#define ERROR_USER_PROFILE_LOAD 500L</summary>
            ERROR_USER_PROFILE_LOAD = 500,
            /// <summary>#define ERROR_ARITHMETIC_OVERFLOW 534L</summary>
            ERROR_ARITHMETIC_OVERFLOW = 534,
            /// <summary>#define ERROR_PIPE_CONNECTED 535L</summary>
            ERROR_PIPE_CONNECTED = 535,
            /// <summary>#define ERROR_PIPE_LISTENING 536L</summary>
            ERROR_PIPE_LISTENING = 536,
            /// <summary>#define ERROR_VERIFIER_STOP 537L</summary>
            ERROR_VERIFIER_STOP = 537,
            /// <summary>#define ERROR_ABIOS_ERROR 538L</summary>
            ERROR_ABIOS_ERROR = 538,
            /// <summary>#define ERROR_WX86_WARNING 539L</summary>
            ERROR_WX86_WARNING = 539,
            /// <summary>#define ERROR_WX86_ERROR 540L</summary>
            ERROR_WX86_ERROR = 540,
            /// <summary>#define ERROR_TIMER_NOT_CANCELED 541L</summary>
            ERROR_TIMER_NOT_CANCELED = 541,
            /// <summary>#define ERROR_UNWIND 542L</summary>
            ERROR_UNWIND = 542,
            /// <summary>#define ERROR_BAD_STACK 543L</summary>
            ERROR_BAD_STACK = 543,
            /// <summary>#define ERROR_INVALID_UNWIND_TARGET 544L</summary>
            ERROR_INVALID_UNWIND_TARGET = 544,
            /// <summary>#define ERROR_INVALID_PORT_ATTRIBUTES 545L</summary>
            ERROR_INVALID_PORT_ATTRIBUTES = 545,
            /// <summary>#define ERROR_PORT_MESSAGE_TOO_LONG 546L</summary>
            ERROR_PORT_MESSAGE_TOO_LONG = 546,
            /// <summary>#define ERROR_INVALID_QUOTA_LOWER 547L</summary>
            ERROR_INVALID_QUOTA_LOWER = 547,
            /// <summary>#define ERROR_DEVICE_ALREADY_ATTACHED 548L</summary>
            ERROR_DEVICE_ALREADY_ATTACHED = 548,
            /// <summary>#define ERROR_INSTRUCTION_MISALIGNMENT 549L</summary>
            ERROR_INSTRUCTION_MISALIGNMENT = 549,
            /// <summary>#define ERROR_PROFILING_NOT_STARTED 550L</summary>
            ERROR_PROFILING_NOT_STARTED = 550,
            /// <summary>#define ERROR_PROFILING_NOT_STOPPED 551L</summary>
            ERROR_PROFILING_NOT_STOPPED = 551,
            /// <summary>#define ERROR_COULD_NOT_INTERPRET 552L</summary>
            ERROR_COULD_NOT_INTERPRET = 552,
            /// <summary>#define ERROR_PROFILING_AT_LIMIT 553L</summary>
            ERROR_PROFILING_AT_LIMIT = 553,
            /// <summary>#define ERROR_CANT_WAIT 554L</summary>
            ERROR_CANT_WAIT = 554,
            /// <summary>#define ERROR_CANT_TERMINATE_SELF 555L</summary>
            ERROR_CANT_TERMINATE_SELF = 555,
            /// <summary>#define ERROR_UNEXPECTED_MM_CREATE_ERR 556L</summary>
            ERROR_UNEXPECTED_MM_CREATE_ERR = 556,
            /// <summary>#define ERROR_UNEXPECTED_MM_MAP_ERROR 557L</summary>
            ERROR_UNEXPECTED_MM_MAP_ERROR = 557,
            /// <summary>#define ERROR_UNEXPECTED_MM_EXTEND_ERR 558L</summary>
            ERROR_UNEXPECTED_MM_EXTEND_ERR = 558,
            /// <summary>#define ERROR_BAD_FUNCTION_TABLE 559L</summary>
            ERROR_BAD_FUNCTION_TABLE = 559,
            /// <summary>#define ERROR_NO_GUID_TRANSLATION 560L</summary>
            ERROR_NO_GUID_TRANSLATION = 560,
            /// <summary>#define ERROR_INVALID_LDT_SIZE 561L</summary>
            ERROR_INVALID_LDT_SIZE = 561,
            /// <summary>#define ERROR_INVALID_LDT_OFFSET 563L</summary>
            ERROR_INVALID_LDT_OFFSET = 563,
            /// <summary>#define ERROR_INVALID_LDT_DESCRIPTOR 564L</summary>
            ERROR_INVALID_LDT_DESCRIPTOR = 564,
            /// <summary>#define ERROR_TOO_MANY_THREADS 565L</summary>
            ERROR_TOO_MANY_THREADS = 565,
            /// <summary>#define ERROR_THREAD_NOT_IN_PROCESS 566L</summary>
            ERROR_THREAD_NOT_IN_PROCESS = 566,
            /// <summary>#define ERROR_PAGEFILE_QUOTA_EXCEEDED 567L</summary>
            ERROR_PAGEFILE_QUOTA_EXCEEDED = 567,
            /// <summary>#define ERROR_LOGON_SERVER_CONFLICT 568L</summary>
            ERROR_LOGON_SERVER_CONFLICT = 568,
            /// <summary>#define ERROR_SYNCHRONIZATION_REQUIRED 569L</summary>
            ERROR_SYNCHRONIZATION_REQUIRED = 569,
            /// <summary>#define ERROR_NET_OPEN_FAILED 570L</summary>
            ERROR_NET_OPEN_FAILED = 570,
            /// <summary>#define ERROR_IO_PRIVILEGE_FAILED 571L</summary>
            ERROR_IO_PRIVILEGE_FAILED = 571,
            /// <summary>#define ERROR_CONTROL_C_EXIT 572L</summary>
            ERROR_CONTROL_C_EXIT = 572,
            /// <summary>#define ERROR_MISSING_SYSTEMFILE 573L</summary>
            ERROR_MISSING_SYSTEMFILE = 573,
            /// <summary>#define ERROR_UNHANDLED_EXCEPTION 574L</summary>
            ERROR_UNHANDLED_EXCEPTION = 574,
            /// <summary>#define ERROR_APP_INIT_FAILURE 575L</summary>
            ERROR_APP_INIT_FAILURE = 575,
            /// <summary>#define ERROR_PAGEFILE_CREATE_FAILED 576L</summary>
            ERROR_PAGEFILE_CREATE_FAILED = 576,
            /// <summary>#define ERROR_INVALID_IMAGE_HASH 577L</summary>
            ERROR_INVALID_IMAGE_HASH = 577,
            /// <summary>#define ERROR_NO_PAGEFILE 578L</summary>
            ERROR_NO_PAGEFILE = 578,
            /// <summary>#define ERROR_ILLEGAL_FLOAT_CONTEXT 579L</summary>
            ERROR_ILLEGAL_FLOAT_CONTEXT = 579,
            /// <summary>#define ERROR_NO_EVENT_PAIR 580L</summary>
            ERROR_NO_EVENT_PAIR = 580,
            /// <summary>#define ERROR_DOMAIN_CTRLR_CONFIG_ERROR 581L</summary>
            ERROR_DOMAIN_CTRLR_CONFIG_ERROR = 581,
            /// <summary>#define ERROR_ILLEGAL_CHARACTER 582L</summary>
            ERROR_ILLEGAL_CHARACTER = 582,
            /// <summary>#define ERROR_UNDEFINED_CHARACTER 583L</summary>
            ERROR_UNDEFINED_CHARACTER = 583,
            /// <summary>#define ERROR_FLOPPY_VOLUME 584L</summary>
            ERROR_FLOPPY_VOLUME = 584,
            /// <summary>#define ERROR_BIOS_FAILED_TO_CONNECT_INTERRUPT 585L</summary>
            ERROR_BIOS_FAILED_TO_CONNECT_INTERRUPT = 585,
            /// <summary>#define ERROR_BACKUP_CONTROLLER 586L</summary>
            ERROR_BACKUP_CONTROLLER = 586,
            /// <summary>#define ERROR_MUTANT_LIMIT_EXCEEDED 587L</summary>
            ERROR_MUTANT_LIMIT_EXCEEDED = 587,
            /// <summary>#define ERROR_FS_DRIVER_REQUIRED 588L</summary>
            ERROR_FS_DRIVER_REQUIRED = 588,
            /// <summary>#define ERROR_CANNOT_LOAD_REGISTRY_FILE 589L</summary>
            ERROR_CANNOT_LOAD_REGISTRY_FILE = 589,
            /// <summary>#define ERROR_DEBUG_ATTACH_FAILED 590L</summary>
            ERROR_DEBUG_ATTACH_FAILED = 590,
            /// <summary>#define ERROR_SYSTEM_PROCESS_TERMINATED 591L</summary>
            ERROR_SYSTEM_PROCESS_TERMINATED = 591,
            /// <summary>#define ERROR_DATA_NOT_ACCEPTED 592L</summary>
            ERROR_DATA_NOT_ACCEPTED = 592,
            /// <summary>#define ERROR_VDM_HARD_ERROR 593L</summary>
            ERROR_VDM_HARD_ERROR = 593,
            /// <summary>#define ERROR_DRIVER_CANCEL_TIMEOUT 594L</summary>
            ERROR_DRIVER_CANCEL_TIMEOUT = 594,
            /// <summary>#define ERROR_REPLY_MESSAGE_MISMATCH 595L</summary>
            ERROR_REPLY_MESSAGE_MISMATCH = 595,
            /// <summary>#define ERROR_LOST_WRITEBEHIND_DATA 596L</summary>
            ERROR_LOST_WRITEBEHIND_DATA = 596,
            /// <summary>#define ERROR_CLIENT_SERVER_PARAMETERS_INVALID 597L</summary>
            ERROR_CLIENT_SERVER_PARAMETERS_INVALID = 597,
            /// <summary>#define ERROR_NOT_TINY_STREAM 598L</summary>
            ERROR_NOT_TINY_STREAM = 598,
            /// <summary>#define ERROR_STACK_OVERFLOW_READ 599L</summary>
            ERROR_STACK_OVERFLOW_READ = 599,
            /// <summary>#define ERROR_CONVERT_TO_LARGE 600L</summary>
            ERROR_CONVERT_TO_LARGE = 600,
            /// <summary>#define ERROR_FOUND_OUT_OF_SCOPE 601L</summary>
            ERROR_FOUND_OUT_OF_SCOPE = 601,
            /// <summary>#define ERROR_ALLOCATE_BUCKET 602L</summary>
            ERROR_ALLOCATE_BUCKET = 602,
            /// <summary>#define ERROR_MARSHALL_OVERFLOW 603L</summary>
            ERROR_MARSHALL_OVERFLOW = 603,
            /// <summary>#define ERROR_INVALID_VARIANT 604L</summary>
            ERROR_INVALID_VARIANT = 604,
            /// <summary>#define ERROR_BAD_COMPRESSION_BUFFER 605L</summary>
            ERROR_BAD_COMPRESSION_BUFFER = 605,
            /// <summary>#define ERROR_AUDIT_FAILED 606L</summary>
            ERROR_AUDIT_FAILED = 606,
            /// <summary>#define ERROR_TIMER_RESOLUTION_NOT_SET 607L</summary>
            ERROR_TIMER_RESOLUTION_NOT_SET = 607,
            /// <summary>#define ERROR_INSUFFICIENT_LOGON_INFO 608L</summary>
            ERROR_INSUFFICIENT_LOGON_INFO = 608,
            /// <summary>#define ERROR_BAD_DLL_ENTRYPOINT 609L</summary>
            ERROR_BAD_DLL_ENTRYPOINT = 609,
            /// <summary>#define ERROR_BAD_SERVICE_ENTRYPOINT 610L</summary>
            ERROR_BAD_SERVICE_ENTRYPOINT = 610,
            /// <summary>#define ERROR_IP_ADDRESS_CONFLICT1 611L</summary>
            ERROR_IP_ADDRESS_CONFLICT1 = 611,
            /// <summary>#define ERROR_IP_ADDRESS_CONFLICT2 612L</summary>
            ERROR_IP_ADDRESS_CONFLICT2 = 612,
            /// <summary>#define ERROR_REGISTRY_QUOTA_LIMIT 613L</summary>
            ERROR_REGISTRY_QUOTA_LIMIT = 613,
            /// <summary>#define ERROR_NO_CALLBACK_ACTIVE 614L</summary>
            ERROR_NO_CALLBACK_ACTIVE = 614,
            /// <summary>#define ERROR_PWD_TOO_SHORT 615L</summary>
            ERROR_PWD_TOO_SHORT = 615,
            /// <summary>#define ERROR_PWD_TOO_RECENT 616L</summary>
            ERROR_PWD_TOO_RECENT = 616,
            /// <summary>#define ERROR_PWD_HISTORY_CONFLICT 617L</summary>
            ERROR_PWD_HISTORY_CONFLICT = 617,
            /// <summary>#define ERROR_UNSUPPORTED_COMPRESSION 618L</summary>
            ERROR_UNSUPPORTED_COMPRESSION = 618,
            /// <summary>#define ERROR_INVALID_HW_PROFILE 619L</summary>
            ERROR_INVALID_HW_PROFILE = 619,
            /// <summary>#define ERROR_INVALID_PLUGPLAY_DEVICE_PATH 620L</summary>
            ERROR_INVALID_PLUGPLAY_DEVICE_PATH = 620,
            /// <summary>#define ERROR_QUOTA_LIST_INCONSISTENT 621L</summary>
            ERROR_QUOTA_LIST_INCONSISTENT = 621,
            /// <summary>#define ERROR_EVALUATION_EXPIRATION 622L</summary>
            ERROR_EVALUATION_EXPIRATION = 622,
            /// <summary>#define ERROR_ILLEGAL_DLL_RELOCATION 623L</summary>
            ERROR_ILLEGAL_DLL_RELOCATION = 623,
            /// <summary>#define ERROR_DLL_INIT_FAILED_LOGOFF 624L</summary>
            ERROR_DLL_INIT_FAILED_LOGOFF = 624,
            /// <summary>#define ERROR_VALIDATE_CONTINUE 625L</summary>
            ERROR_VALIDATE_CONTINUE = 625,
            /// <summary>#define ERROR_NO_MORE_MATCHES 626L</summary>
            ERROR_NO_MORE_MATCHES = 626,
            /// <summary>#define ERROR_RANGE_LIST_CONFLICT 627L</summary>
            ERROR_RANGE_LIST_CONFLICT = 627,
            /// <summary>#define ERROR_SERVER_SID_MISMATCH 628L</summary>
            ERROR_SERVER_SID_MISMATCH = 628,
            /// <summary>#define ERROR_CANT_ENABLE_DENY_ONLY 629L</summary>
            ERROR_CANT_ENABLE_DENY_ONLY = 629,
            /// <summary>#define ERROR_FLOAT_MULTIPLE_FAULTS 630L</summary>
            ERROR_FLOAT_MULTIPLE_FAULTS = 630,
            /// <summary>#define ERROR_FLOAT_MULTIPLE_TRAPS 631L</summary>
            ERROR_FLOAT_MULTIPLE_TRAPS = 631,
            /// <summary>#define ERROR_NOINTERFACE 632L</summary>
            ERROR_NOINTERFACE = 632,
            /// <summary>#define ERROR_DRIVER_FAILED_SLEEP 633L</summary>
            ERROR_DRIVER_FAILED_SLEEP = 633,
            /// <summary>#define ERROR_CORRUPT_SYSTEM_FILE 634L</summary>
            ERROR_CORRUPT_SYSTEM_FILE = 634,
            /// <summary>#define ERROR_COMMITMENT_MINIMUM 635L</summary>
            ERROR_COMMITMENT_MINIMUM = 635,
            /// <summary>#define ERROR_PNP_RESTART_ENUMERATION 636L</summary>
            ERROR_PNP_RESTART_ENUMERATION = 636,
            /// <summary>#define ERROR_SYSTEM_IMAGE_BAD_SIGNATURE 637L</summary>
            ERROR_SYSTEM_IMAGE_BAD_SIGNATURE = 637,
            /// <summary>#define ERROR_PNP_REBOOT_REQUIRED 638L</summary>
            ERROR_PNP_REBOOT_REQUIRED = 638,
            /// <summary>#define ERROR_INSUFFICIENT_POWER 639L</summary>
            ERROR_INSUFFICIENT_POWER = 639,
            /// <summary>#define ERROR_MULTIPLE_FAULT_VIOLATION 640L</summary>
            ERROR_MULTIPLE_FAULT_VIOLATION = 640,
            /// <summary>#define ERROR_SYSTEM_SHUTDOWN 641L</summary>
            ERROR_SYSTEM_SHUTDOWN = 641,
            /// <summary>#define ERROR_PORT_NOT_SET 642L</summary>
            ERROR_PORT_NOT_SET = 642,
            /// <summary>#define ERROR_DS_VERSION_CHECK_FAILURE 643L</summary>
            ERROR_DS_VERSION_CHECK_FAILURE = 643,
            /// <summary>#define ERROR_RANGE_NOT_FOUND 644L</summary>
            ERROR_RANGE_NOT_FOUND = 644,
            /// <summary>#define ERROR_NOT_SAFE_MODE_DRIVER 646L</summary>
            ERROR_NOT_SAFE_MODE_DRIVER = 646,
            /// <summary>#define ERROR_FAILED_DRIVER_ENTRY 647L</summary>
            ERROR_FAILED_DRIVER_ENTRY = 647,
            /// <summary>#define ERROR_DEVICE_ENUMERATION_ERROR 648L</summary>
            ERROR_DEVICE_ENUMERATION_ERROR = 648,
            /// <summary>#define ERROR_MOUNT_POINT_NOT_RESOLVED 649L</summary>
            ERROR_MOUNT_POINT_NOT_RESOLVED = 649,
            /// <summary>#define ERROR_INVALID_DEVICE_OBJECT_PARAMETER 650L</summary>
            ERROR_INVALID_DEVICE_OBJECT_PARAMETER = 650,
            /// <summary>#define ERROR_MCA_OCCURED 651L</summary>
            ERROR_MCA_OCCURED = 651,
            /// <summary>#define ERROR_DRIVER_DATABASE_ERROR 652L</summary>
            ERROR_DRIVER_DATABASE_ERROR = 652,
            /// <summary>#define ERROR_SYSTEM_HIVE_TOO_LARGE 653L</summary>
            ERROR_SYSTEM_HIVE_TOO_LARGE = 653,
            /// <summary>#define ERROR_DRIVER_FAILED_PRIOR_UNLOAD 654L</summary>
            ERROR_DRIVER_FAILED_PRIOR_UNLOAD = 654,
            /// <summary>#define ERROR_VOLSNAP_PREPARE_HIBERNATE 655L</summary>
            ERROR_VOLSNAP_PREPARE_HIBERNATE = 655,
            /// <summary>#define ERROR_HIBERNATION_FAILURE 656L</summary>
            ERROR_HIBERNATION_FAILURE = 656,
            /// <summary>#define ERROR_FILE_SYSTEM_LIMITATION 665L</summary>
            ERROR_FILE_SYSTEM_LIMITATION = 665,
            /// <summary>#define ERROR_ASSERTION_FAILURE 668L</summary>
            ERROR_ASSERTION_FAILURE = 668,
            /// <summary>#define ERROR_ACPI_ERROR 669L</summary>
            ERROR_ACPI_ERROR = 669,
            /// <summary>#define ERROR_WOW_ASSERTION 670L</summary>
            ERROR_WOW_ASSERTION = 670,
            /// <summary>#define ERROR_PNP_BAD_MPS_TABLE 671L</summary>
            ERROR_PNP_BAD_MPS_TABLE = 671,
            /// <summary>#define ERROR_PNP_TRANSLATION_FAILED 672L</summary>
            ERROR_PNP_TRANSLATION_FAILED = 672,
            /// <summary>#define ERROR_PNP_IRQ_TRANSLATION_FAILED 673L</summary>
            ERROR_PNP_IRQ_TRANSLATION_FAILED = 673,
            /// <summary>#define ERROR_PNP_INVALID_ID 674L</summary>
            ERROR_PNP_INVALID_ID = 674,
            /// <summary>#define ERROR_WAKE_SYSTEM_DEBUGGER 675L</summary>
            ERROR_WAKE_SYSTEM_DEBUGGER = 675,
            /// <summary>#define ERROR_HANDLES_CLOSED 676L</summary>
            ERROR_HANDLES_CLOSED = 676,
            /// <summary>#define ERROR_EXTRANEOUS_INFORMATION 677L</summary>
            ERROR_EXTRANEOUS_INFORMATION = 677,
            /// <summary>#define ERROR_RXACT_COMMIT_NECESSARY 678L</summary>
            ERROR_RXACT_COMMIT_NECESSARY = 678,
            /// <summary>#define ERROR_MEDIA_CHECK 679L</summary>
            ERROR_MEDIA_CHECK = 679,
            /// <summary>#define ERROR_GUID_SUBSTITUTION_MADE 680L</summary>
            ERROR_GUID_SUBSTITUTION_MADE = 680,
            /// <summary>#define ERROR_STOPPED_ON_SYMLINK 681L</summary>
            ERROR_STOPPED_ON_SYMLINK = 681,
            /// <summary>#define ERROR_LONGJUMP 682L</summary>
            ERROR_LONGJUMP = 682,
            /// <summary>#define ERROR_PLUGPLAY_QUERY_VETOED 683L</summary>
            ERROR_PLUGPLAY_QUERY_VETOED = 683,
            /// <summary>#define ERROR_UNWIND_CONSOLIDATE 684L</summary>
            ERROR_UNWIND_CONSOLIDATE = 684,
            /// <summary>#define ERROR_REGISTRY_HIVE_RECOVERED 685L</summary>
            ERROR_REGISTRY_HIVE_RECOVERED = 685,
            /// <summary>#define ERROR_DLL_MIGHT_BE_INSECURE 686L</summary>
            ERROR_DLL_MIGHT_BE_INSECURE = 686,
            /// <summary>#define ERROR_DLL_MIGHT_BE_INCOMPATIBLE 687L</summary>
            ERROR_DLL_MIGHT_BE_INCOMPATIBLE = 687,
            /// <summary>#define ERROR_DBG_EXCEPTION_NOT_HANDLED 688L</summary>
            ERROR_DBG_EXCEPTION_NOT_HANDLED = 688,
            /// <summary>#define ERROR_DBG_REPLY_LATER 689L</summary>
            ERROR_DBG_REPLY_LATER = 689,
            /// <summary>#define ERROR_DBG_UNABLE_TO_PROVIDE_HANDLE 690L</summary>
            ERROR_DBG_UNABLE_TO_PROVIDE_HANDLE = 690,
            /// <summary>#define ERROR_DBG_TERMINATE_THREAD 691L</summary>
            ERROR_DBG_TERMINATE_THREAD = 691,
            /// <summary>#define ERROR_DBG_TERMINATE_PROCESS 692L</summary>
            ERROR_DBG_TERMINATE_PROCESS = 692,
            /// <summary>#define ERROR_DBG_CONTROL_C 693L</summary>
            ERROR_DBG_CONTROL_C = 693,
            /// <summary>#define ERROR_DBG_PRINTEXCEPTION_C 694L</summary>
            ERROR_DBG_PRINTEXCEPTION_C = 694,
            /// <summary>#define ERROR_DBG_RIPEXCEPTION 695L</summary>
            ERROR_DBG_RIPEXCEPTION = 695,
            /// <summary>#define ERROR_DBG_CONTROL_BREAK 696L</summary>
            ERROR_DBG_CONTROL_BREAK = 696,
            /// <summary>#define ERROR_DBG_COMMAND_EXCEPTION 697L</summary>
            ERROR_DBG_COMMAND_EXCEPTION = 697,
            /// <summary>#define ERROR_OBJECT_NAME_EXISTS 698L</summary>
            ERROR_OBJECT_NAME_EXISTS = 698,
            /// <summary>#define ERROR_THREAD_WAS_SUSPENDED 699L</summary>
            ERROR_THREAD_WAS_SUSPENDED = 699,
            /// <summary>#define ERROR_IMAGE_NOT_AT_BASE 700L</summary>
            ERROR_IMAGE_NOT_AT_BASE = 700,
            /// <summary>#define ERROR_RXACT_STATE_CREATED 701L</summary>
            ERROR_RXACT_STATE_CREATED = 701,
            /// <summary>#define ERROR_SEGMENT_NOTIFICATION 702L</summary>
            ERROR_SEGMENT_NOTIFICATION = 702,
            /// <summary>#define ERROR_BAD_CURRENT_DIRECTORY 703L</summary>
            ERROR_BAD_CURRENT_DIRECTORY = 703,
            /// <summary>#define ERROR_FT_READ_RECOVERY_FROM_BACKUP 704L</summary>
            ERROR_FT_READ_RECOVERY_FROM_BACKUP = 704,
            /// <summary>#define ERROR_FT_WRITE_RECOVERY 705L</summary>
            ERROR_FT_WRITE_RECOVERY = 705,
            /// <summary>#define ERROR_IMAGE_MACHINE_TYPE_MISMATCH 706L</summary>
            ERROR_IMAGE_MACHINE_TYPE_MISMATCH = 706,
            /// <summary>#define ERROR_RECEIVE_PARTIAL 707L</summary>
            ERROR_RECEIVE_PARTIAL = 707,
            /// <summary>#define ERROR_RECEIVE_EXPEDITED 708L</summary>
            ERROR_RECEIVE_EXPEDITED = 708,
            /// <summary>#define ERROR_RECEIVE_PARTIAL_EXPEDITED 709L</summary>
            ERROR_RECEIVE_PARTIAL_EXPEDITED = 709,
            /// <summary>#define ERROR_EVENT_DONE 710L</summary>
            ERROR_EVENT_DONE = 710,
            /// <summary>#define ERROR_EVENT_PENDING 711L</summary>
            ERROR_EVENT_PENDING = 711,
            /// <summary>#define ERROR_CHECKING_FILE_SYSTEM 712L</summary>
            ERROR_CHECKING_FILE_SYSTEM = 712,
            /// <summary>#define ERROR_FATAL_APP_EXIT 713L</summary>
            ERROR_FATAL_APP_EXIT = 713,
            /// <summary>#define ERROR_PREDEFINED_HANDLE 714L</summary>
            ERROR_PREDEFINED_HANDLE = 714,
            /// <summary>#define ERROR_WAS_UNLOCKED 715L</summary>
            ERROR_WAS_UNLOCKED = 715,
            /// <summary>#define ERROR_SERVICE_NOTIFICATION 716L</summary>
            ERROR_SERVICE_NOTIFICATION = 716,
            /// <summary>#define ERROR_WAS_LOCKED 717L</summary>
            ERROR_WAS_LOCKED = 717,
            /// <summary>#define ERROR_LOG_HARD_ERROR 718L</summary>
            ERROR_LOG_HARD_ERROR = 718,
            /// <summary>#define ERROR_ALREADY_WIN32 719L</summary>
            ERROR_ALREADY_WIN32 = 719,
            /// <summary>#define ERROR_IMAGE_MACHINE_TYPE_MISMATCH_EXE 720L</summary>
            ERROR_IMAGE_MACHINE_TYPE_MISMATCH_EXE = 720,
            /// <summary>#define ERROR_NO_YIELD_PERFORMED 721L</summary>
            ERROR_NO_YIELD_PERFORMED = 721,
            /// <summary>#define ERROR_TIMER_RESUME_IGNORED 722L</summary>
            ERROR_TIMER_RESUME_IGNORED = 722,
            /// <summary>#define ERROR_ARBITRATION_UNHANDLED 723L</summary>
            ERROR_ARBITRATION_UNHANDLED = 723,
            /// <summary>#define ERROR_CARDBUS_NOT_SUPPORTED 724L</summary>
            ERROR_CARDBUS_NOT_SUPPORTED = 724,
            /// <summary>#define ERROR_MP_PROCESSOR_MISMATCH 725L</summary>
            ERROR_MP_PROCESSOR_MISMATCH = 725,
            /// <summary>#define ERROR_HIBERNATED 726L </summary>
            ERROR_HIBERNATED = 726,
            /// <summary>#define ERROR_RESUME_HIBERNATION 727L </summary>
            ERROR_RESUME_HIBERNATION = 727,
            /// <summary>#define ERROR_FIRMWARE_UPDATED 728L</summary>
            ERROR_FIRMWARE_UPDATED = 728,
            /// <summary>#define ERROR_DRIVERS_LEAKING_LOCKED_PAGES 729L</summary>
            ERROR_DRIVERS_LEAKING_LOCKED_PAGES = 729,
            /// <summary>#define ERROR_WAKE_SYSTEM 730L</summary>
            ERROR_WAKE_SYSTEM = 730,
            /// <summary>#define ERROR_WAIT_1 731L</summary>
            ERROR_WAIT_1 = 731,
            /// <summary>#define ERROR_WAIT_2 732L</summary>
            ERROR_WAIT_2 = 732,
            /// <summary>#define ERROR_WAIT_3 733L</summary>
            ERROR_WAIT_3 = 733,
            /// <summary>#define ERROR_WAIT_63 734L</summary>
            ERROR_WAIT_63 = 734,
            /// <summary>#define ERROR_ABANDONED_WAIT_0 735L</summary>
            ERROR_ABANDONED_WAIT_0 = 735,
            /// <summary>#define ERROR_ABANDONED_WAIT_63 736L</summary>
            ERROR_ABANDONED_WAIT_63 = 736,
            /// <summary>#define ERROR_USER_APC 737L</summary>
            ERROR_USER_APC = 737,
            /// <summary>#define ERROR_KERNEL_APC 738L</summary>
            ERROR_KERNEL_APC = 738,
            /// <summary>#define ERROR_ALERTED 739L</summary>
            ERROR_ALERTED = 739,
            /// <summary>#define ERROR_ELEVATION_REQUIRED 740L</summary>
            ERROR_ELEVATION_REQUIRED = 740,
            /// <summary>#define ERROR_REPARSE 741L</summary>
            ERROR_REPARSE = 741,
            /// <summary>#define ERROR_OPLOCK_BREAK_IN_PROGRESS 742L</summary>
            ERROR_OPLOCK_BREAK_IN_PROGRESS = 742,
            /// <summary>#define ERROR_VOLUME_MOUNTED 743L</summary>
            ERROR_VOLUME_MOUNTED = 743,
            /// <summary>#define ERROR_RXACT_COMMITTED 744L</summary>
            ERROR_RXACT_COMMITTED = 744,
            /// <summary>#define ERROR_NOTIFY_CLEANUP 745L</summary>
            ERROR_NOTIFY_CLEANUP = 745,
            /// <summary>#define ERROR_PRIMARY_TRANSPORT_CONNECT_FAILED 746L</summary>
            ERROR_PRIMARY_TRANSPORT_CONNECT_FAILED = 746,
            /// <summary>#define ERROR_PAGE_FAULT_TRANSITION 747L</summary>
            ERROR_PAGE_FAULT_TRANSITION = 747,
            /// <summary>#define ERROR_PAGE_FAULT_DEMAND_ZERO 748L</summary>
            ERROR_PAGE_FAULT_DEMAND_ZERO = 748,
            /// <summary>#define ERROR_PAGE_FAULT_COPY_ON_WRITE 749L</summary>
            ERROR_PAGE_FAULT_COPY_ON_WRITE = 749,
            /// <summary>#define ERROR_PAGE_FAULT_GUARD_PAGE 750L</summary>
            ERROR_PAGE_FAULT_GUARD_PAGE = 750,
            /// <summary>#define ERROR_PAGE_FAULT_PAGING_FILE 751L</summary>
            ERROR_PAGE_FAULT_PAGING_FILE = 751,
            /// <summary>#define ERROR_CACHE_PAGE_LOCKED 752L</summary>
            ERROR_CACHE_PAGE_LOCKED = 752,
            /// <summary>#define ERROR_CRASH_DUMP 753L</summary>
            ERROR_CRASH_DUMP = 753,
            /// <summary>#define ERROR_BUFFER_ALL_ZEROS 754L</summary>
            ERROR_BUFFER_ALL_ZEROS = 754,
            /// <summary>#define ERROR_REPARSE_OBJECT 755L</summary>
            ERROR_REPARSE_OBJECT = 755,
            /// <summary>#define ERROR_RESOURCE_REQUIREMENTS_CHANGED 756L</summary>
            ERROR_RESOURCE_REQUIREMENTS_CHANGED = 756,
            /// <summary>#define ERROR_TRANSLATION_COMPLETE 757L</summary>
            ERROR_TRANSLATION_COMPLETE = 757,
            /// <summary>#define ERROR_NOTHING_TO_TERMINATE 758L</summary>
            ERROR_NOTHING_TO_TERMINATE = 758,
            /// <summary>#define ERROR_PROCESS_NOT_IN_JOB 759L</summary>
            ERROR_PROCESS_NOT_IN_JOB = 759,
            /// <summary>#define ERROR_PROCESS_IN_JOB 760L</summary>
            ERROR_PROCESS_IN_JOB = 760,
            /// <summary>#define ERROR_VOLSNAP_HIBERNATE_READY 761L</summary>
            ERROR_VOLSNAP_HIBERNATE_READY = 761,
            /// <summary>#define ERROR_FSFILTER_OP_COMPLETED_SUCCESSFULLY 762L</summary>
            ERROR_FSFILTER_OP_COMPLETED_SUCCESSFULLY = 762,
            /// <summary>#define ERROR_INTERRUPT_VECTOR_ALREADY_CONNECTED 763L</summary>
            ERROR_INTERRUPT_VECTOR_ALREADY_CONNECTED = 763,
            /// <summary>#define ERROR_INTERRUPT_STILL_CONNECTED 764L</summary>
            ERROR_INTERRUPT_STILL_CONNECTED = 764,
            /// <summary>#define ERROR_WAIT_FOR_OPLOCK 765L</summary>
            ERROR_WAIT_FOR_OPLOCK = 765,
            /// <summary>#define ERROR_DBG_EXCEPTION_HANDLED 766L</summary>
            ERROR_DBG_EXCEPTION_HANDLED = 766,
            /// <summary>#define ERROR_DBG_CONTINUE 767L</summary>
            ERROR_DBG_CONTINUE = 767,
            /// <summary>#define ERROR_CALLBACK_POP_STACK 768L</summary>
            ERROR_CALLBACK_POP_STACK = 768,
            /// <summary>#define ERROR_COMPRESSION_DISABLED 769L</summary>
            ERROR_COMPRESSION_DISABLED = 769,
            /// <summary>#define ERROR_CANTFETCHBACKWARDS 770L</summary>
            ERROR_CANTFETCHBACKWARDS = 770,
            /// <summary>#define ERROR_CANTSCROLLBACKWARDS 771L</summary>
            ERROR_CANTSCROLLBACKWARDS = 771,
            /// <summary>#define ERROR_ROWSNOTRELEASED 772L</summary>
            ERROR_ROWSNOTRELEASED = 772,
            /// <summary>#define ERROR_BAD_ACCESSOR_FLAGS 773L</summary>
            ERROR_BAD_ACCESSOR_FLAGS = 773,
            /// <summary>#define ERROR_ERRORS_ENCOUNTERED 774L</summary>
            ERROR_ERRORS_ENCOUNTERED = 774,
            /// <summary>#define ERROR_NOT_CAPABLE 775L</summary>
            ERROR_NOT_CAPABLE = 775,
            /// <summary>#define ERROR_REQUEST_OUT_OF_SEQUENCE 776L</summary>
            ERROR_REQUEST_OUT_OF_SEQUENCE = 776,
            /// <summary>#define ERROR_VERSION_PARSE_ERROR 777L</summary>
            ERROR_VERSION_PARSE_ERROR = 777,
            /// <summary>#define ERROR_BADSTARTPOSITION 778L</summary>
            ERROR_BADSTARTPOSITION = 778,
            /// <summary>#define ERROR_MEMORY_HARDWARE 779L</summary>
            ERROR_MEMORY_HARDWARE = 779,
            /// <summary>#define ERROR_DISK_REPAIR_DISABLED 780L</summary>
            ERROR_DISK_REPAIR_DISABLED = 780,
            /// <summary>#define ERROR_INSUFFICIENT_RESOURCE_FOR_SPECIFIED_SHARED_SECTION_SIZE 781L</summary>
            ERROR_INSUFFICIENT_RESOURCE_FOR_SPECIFIED_SHARED_SECTION_SIZE = 781,
            /// <summary>#define ERROR_SYSTEM_POWERSTATE_TRANSITION 782L</summary>
            ERROR_SYSTEM_POWERSTATE_TRANSITION = 782,
            /// <summary>#define ERROR_SYSTEM_POWERSTATE_COMPLEX_TRANSITION 783L</summary>
            ERROR_SYSTEM_POWERSTATE_COMPLEX_TRANSITION = 783,
            /// <summary>#define ERROR_MCA_EXCEPTION 784L</summary>
            ERROR_MCA_EXCEPTION = 784,
            /// <summary>#define ERROR_ACCESS_AUDIT_BY_POLICY 785L</summary>
            ERROR_ACCESS_AUDIT_BY_POLICY = 785,
            /// <summary>#define ERROR_ACCESS_DISABLED_NO_SAFER_UI_BY_POLICY 786L</summary>
            ERROR_ACCESS_DISABLED_NO_SAFER_UI_BY_POLICY = 786,
            /// <summary>#define ERROR_ABANDON_HIBERFILE 787L</summary>
            ERROR_ABANDON_HIBERFILE = 787,
            /// <summary>#define ERROR_LOST_WRITEBEHIND_DATA_NETWORK_DISCONNECTED 788L</summary>
            ERROR_LOST_WRITEBEHIND_DATA_NETWORK_DISCONNECTED = 788,
            /// <summary>#define ERROR_LOST_WRITEBEHIND_DATA_NETWORK_SERVER_ERROR 789L</summary>
            ERROR_LOST_WRITEBEHIND_DATA_NETWORK_SERVER_ERROR = 789,
            /// <summary>#define ERROR_LOST_WRITEBEHIND_DATA_LOCAL_DISK_ERROR 790L</summary>
            ERROR_LOST_WRITEBEHIND_DATA_LOCAL_DISK_ERROR = 790,
            /// <summary>#define ERROR_BAD_MCFG_TABLE 791L</summary>
            ERROR_BAD_MCFG_TABLE = 791,
            /// <summary>#define ERROR_OPLOCK_SWITCHED_TO_NEW_HANDLE 800L</summary>
            ERROR_OPLOCK_SWITCHED_TO_NEW_HANDLE = 800,
            /// <summary>#define ERROR_CANNOT_GRANT_REQUESTED_OPLOCK 801L</summary>
            ERROR_CANNOT_GRANT_REQUESTED_OPLOCK = 801,
            /// <summary>#define ERROR_CANNOT_BREAK_OPLOCK 802L</summary>
            ERROR_CANNOT_BREAK_OPLOCK = 802,
            /// <summary>#define ERROR_OPLOCK_HANDLE_CLOSED 803L</summary>
            ERROR_OPLOCK_HANDLE_CLOSED = 803,
            /// <summary>#define ERROR_NO_ACE_CONDITION 804L</summary>
            ERROR_NO_ACE_CONDITION = 804,
            /// <summary>#define ERROR_INVALID_ACE_CONDITION 805L</summary>
            ERROR_INVALID_ACE_CONDITION = 805,
            /// <summary>#define ERROR_EA_ACCESS_DENIED 994L</summary>
            ERROR_EA_ACCESS_DENIED = 994,
            /// <summary>#define ERROR_OPERATION_ABORTED 995L</summary>
            ERROR_OPERATION_ABORTED = 995,
            /// <summary>#define ERROR_IO_INCOMPLETE 996L</summary>
            ERROR_IO_INCOMPLETE = 996,
            /// <summary>#define ERROR_IO_PENDING 997L</summary>
            ERROR_IO_PENDING = 997,
            /// <summary>#define ERROR_NOACCESS 998L</summary>
            ERROR_NOACCESS = 998,
            /// <summary>#define ERROR_SWAPERROR 999L</summary>
            ERROR_SWAPERROR = 999,
            /// <summary>#define ERROR_STACK_OVERFLOW 1001L</summary>
            ERROR_STACK_OVERFLOW = 1001, //
            /// <summary>#define ERROR_INVALID_MESSAGE 1002L</summary>
            ERROR_INVALID_MESSAGE = 1002,
            /// <summary>#define ERROR_CAN_NOT_COMPLETE 1003L</summary>
            ERROR_CAN_NOT_COMPLETE = 1003,
            /// <summary>#define ERROR_INVALID_FLAGS 1004L</summary>
            ERROR_INVALID_FLAGS = 1004,
            /// <summary>#define ERROR_UNRECOGNIZED_VOLUME 1005L</summary>
            ERROR_UNRECOGNIZED_VOLUME = 1005,
            /// <summary>#define ERROR_FILE_INVALID 1006L</summary>
            ERROR_FILE_INVALID = 1006,
            /// <summary>#define ERROR_FULLSCREEN_MODE 1007L</summary>
            ERROR_FULLSCREEN_MODE = 1007,
            /// <summary>#define ERROR_NO_TOKEN 1008L</summary>
            ERROR_NO_TOKEN = 1008,
            /// <summary>#define ERROR_BADDB 1009L</summary>
            ERROR_BADDB = 1009,
            /// <summary>#define ERROR_BADKEY 1010L</summary>
            ERROR_BADKEY = 1010,
            /// <summary>#define ERROR_CANTOPEN 1011L</summary>
            ERROR_CANTOPEN = 1011,
            /// <summary>#define ERROR_CANTREAD 1012L</summary>
            ERROR_CANTREAD = 1012,
            /// <summary>#define ERROR_CANTWRITE 1013L</summary>
            ERROR_CANTWRITE = 1013,
            /// <summary>#define ERROR_REGISTRY_RECOVERED 1014L</summary>
            ERROR_REGISTRY_RECOVERED = 1014,
            /// <summary>#define ERROR_REGISTRY_CORRUPT 1015L</summary>
            ERROR_REGISTRY_CORRUPT = 1015,
            /// <summary>#define ERROR_REGISTRY_IO_FAILED 1016L</summary>
            ERROR_REGISTRY_IO_FAILED = 1016,
            /// <summary>#define ERROR_NOT_REGISTRY_FILE 1017L</summary>
            ERROR_NOT_REGISTRY_FILE = 1017,
            /// <summary>#define ERROR_KEY_DELETED 1018L</summary>
            ERROR_KEY_DELETED = 1018,
            /// <summary>#define ERROR_NO_LOG_SPACE 1019L</summary>
            ERROR_NO_LOG_SPACE = 1019,
            /// <summary>#define ERROR_KEY_HAS_CHILDREN 1020L</summary>
            ERROR_KEY_HAS_CHILDREN = 1020,
            /// <summary>#define ERROR_CHILD_MUST_BE_VOLATILE 1021L</summary>
            ERROR_CHILD_MUST_BE_VOLATILE = 1021,
            /// <summary>#define ERROR_NOTIFY_ENUM_DIR 1022L</summary>
            ERROR_NOTIFY_ENUM_DIR = 1022,
            /// <summary>#define ERROR_DEPENDENT_SERVICES_RUNNING 1051L</summary>
            ERROR_DEPENDENT_SERVICES_RUNNING = 1051,
            /// <summary>#define ERROR_INVALID_SERVICE_CONTROL 1052L</summary>
            ERROR_INVALID_SERVICE_CONTROL = 1052,
            /// <summary>#define ERROR_SERVICE_REQUEST_TIMEOUT 1053L</summary>
            ERROR_SERVICE_REQUEST_TIMEOUT = 1053,
            /// <summary>#define ERROR_SERVICE_NO_THREAD 1054L</summary>
            ERROR_SERVICE_NO_THREAD = 1054,
            /// <summary>#define ERROR_SERVICE_DATABASE_LOCKED 1055L</summary>
            ERROR_SERVICE_DATABASE_LOCKED = 1055,
            /// <summary>#define ERROR_SERVICE_ALREADY_RUNNING 1056L</summary>
            ERROR_SERVICE_ALREADY_RUNNING = 1056,
            /// <summary>#define ERROR_INVALID_SERVICE_ACCOUNT 1057L</summary>
            ERROR_INVALID_SERVICE_ACCOUNT = 1057,
            /// <summary>#define ERROR_SERVICE_DISABLED 1058L</summary>
            ERROR_SERVICE_DISABLED = 1058,
            /// <summary>#define ERROR_CIRCULAR_DEPENDENCY 1059L</summary>
            ERROR_CIRCULAR_DEPENDENCY = 1059,
            /// <summary>#define ERROR_SERVICE_DOES_NOT_EXIST 1060L</summary>
            ERROR_SERVICE_DOES_NOT_EXIST = 1060,
            /// <summary>#define ERROR_SERVICE_CANNOT_ACCEPT_CTRL 1061L</summary>
            ERROR_SERVICE_CANNOT_ACCEPT_CTRL = 1061,
            /// <summary>#define ERROR_SERVICE_NOT_ACTIVE 1062L</summary>
            ERROR_SERVICE_NOT_ACTIVE = 1062,
            /// <summary>#define ERROR_FAILED_SERVICE_CONTROLLER_CONNECT 1063L</summary>
            ERROR_FAILED_SERVICE_CONTROLLER_CONNECT = 1063,
            /// <summary>#define ERROR_EXCEPTION_IN_SERVICE 1064L</summary>
            ERROR_EXCEPTION_IN_SERVICE = 1064,
            /// <summary>#define ERROR_DATABASE_DOES_NOT_EXIST 1065L</summary>
            ERROR_DATABASE_DOES_NOT_EXIST = 1065,
            /// <summary>#define ERROR_SERVICE_SPECIFIC_ERROR 1066L</summary>
            ERROR_SERVICE_SPECIFIC_ERROR = 1066,
            /// <summary>#define ERROR_PROCESS_ABORTED 1067L</summary>
            ERROR_PROCESS_ABORTED = 1067,
            /// <summary>#define ERROR_SERVICE_DEPENDENCY_FAIL 1068L</summary>
            ERROR_SERVICE_DEPENDENCY_FAIL = 1068,
            /// <summary>#define ERROR_SERVICE_LOGON_FAILED 1069L</summary>
            ERROR_SERVICE_LOGON_FAILED = 1069,
            /// <summary>#define ERROR_SERVICE_START_HANG 1070L</summary>
            ERROR_SERVICE_START_HANG = 1070,
            /// <summary>#define ERROR_INVALID_SERVICE_LOCK 1071L</summary>
            ERROR_INVALID_SERVICE_LOCK = 1071,
            /// <summary>#define ERROR_SERVICE_MARKED_FOR_DELETE 1072L</summary>
            ERROR_SERVICE_MARKED_FOR_DELETE = 1072,
            /// <summary>#define ERROR_SERVICE_EXISTS 1073L</summary>
            ERROR_SERVICE_EXISTS = 1073,
            /// <summary>#define ERROR_ALREADY_RUNNING_LKG 1074L</summary>
            ERROR_ALREADY_RUNNING_LKG = 1074,
            /// <summary>#define ERROR_SERVICE_DEPENDENCY_DELETED 1075L</summary>
            ERROR_SERVICE_DEPENDENCY_DELETED = 1075,
            /// <summary>#define ERROR_BOOT_ALREADY_ACCEPTED 1076L</summary>
            ERROR_BOOT_ALREADY_ACCEPTED = 1076,
            /// <summary>#define ERROR_SERVICE_NEVER_STARTED 1077L</summary>
            ERROR_SERVICE_NEVER_STARTED = 1077,
            /// <summary>#define ERROR_DUPLICATE_SERVICE_NAME 1078L</summary>
            ERROR_DUPLICATE_SERVICE_NAME = 1078,
            /// <summary>#define ERROR_DIFFERENT_SERVICE_ACCOUNT 1079L</summary>
            ERROR_DIFFERENT_SERVICE_ACCOUNT = 1079,
            /// <summary>#define ERROR_CANNOT_DETECT_DRIVER_FAILURE 1080L</summary>
            ERROR_CANNOT_DETECT_DRIVER_FAILURE = 1080,
            /// <summary>#define ERROR_CANNOT_DETECT_PROCESS_ABORT 1081L</summary>
            ERROR_CANNOT_DETECT_PROCESS_ABORT = 1081,
            /// <summary>#define ERROR_NO_RECOVERY_PROGRAM 1082L</summary>
            ERROR_NO_RECOVERY_PROGRAM = 1082,
            /// <summary>#define ERROR_SERVICE_NOT_IN_EXE 1083L</summary>
            ERROR_SERVICE_NOT_IN_EXE = 1083,
            /// <summary>#define ERROR_NOT_SAFEBOOT_SERVICE 1084L</summary>
            ERROR_NOT_SAFEBOOT_SERVICE = 1084,
            /// <summary>#define ERROR_END_OF_MEDIA 1100L</summary>
            ERROR_END_OF_MEDIA = 1100,
            /// <summary>#define ERROR_FILEMARK_DETECTED 1101L</summary>
            ERROR_FILEMARK_DETECTED = 1101,
            /// <summary>#define ERROR_BEGINNING_OF_MEDIA 1102L</summary>
            ERROR_BEGINNING_OF_MEDIA = 1102,
            /// <summary>#define ERROR_SETMARK_DETECTED 1103L</summary>
            ERROR_SETMARK_DETECTED = 1103,
            /// <summary>#define ERROR_NO_DATA_DETECTED 1104L</summary>
            ERROR_NO_DATA_DETECTED = 1104,
            /// <summary>#define ERROR_PARTITION_FAILURE 1105L</summary>
            ERROR_PARTITION_FAILURE = 1105,
            /// <summary>#define ERROR_INVALID_BLOCK_LENGTH 1106L</summary>
            ERROR_INVALID_BLOCK_LENGTH = 1106,
            /// <summary>#define ERROR_DEVICE_NOT_PARTITIONED 1107L</summary>
            ERROR_DEVICE_NOT_PARTITIONED = 1107,
            /// <summary>#define ERROR_UNABLE_TO_LOCK_MEDIA 1108L</summary>
            ERROR_UNABLE_TO_LOCK_MEDIA = 1108,
            /// <summary>#define ERROR_UNABLE_TO_UNLOAD_MEDIA 1109L</summary>
            ERROR_UNABLE_TO_UNLOAD_MEDIA = 1109,
            /// <summary>#define ERROR_MEDIA_CHANGED 1110L</summary>
            ERROR_MEDIA_CHANGED = 1110,
            /// <summary>#define ERROR_BUS_RESET 1111L</summary>
            ERROR_BUS_RESET = 1111,
            /// <summary>#define ERROR_NO_MEDIA_IN_DRIVE 1112L</summary>
            ERROR_NO_MEDIA_IN_DRIVE = 1112,
            /// <summary>#define ERROR_NO_UNICODE_TRANSLATION 1113L</summary>
            ERROR_NO_UNICODE_TRANSLATION = 1113,
            /// <summary>#define ERROR_DLL_INIT_FAILED 1114L</summary>
            ERROR_DLL_INIT_FAILED = 1114,
            /// <summary>#define ERROR_SHUTDOWN_IN_PROGRESS 1115L</summary>
            ERROR_SHUTDOWN_IN_PROGRESS = 1115,
            /// <summary>#define ERROR_NO_SHUTDOWN_IN_PROGRESS 1116L</summary>
            ERROR_NO_SHUTDOWN_IN_PROGRESS = 1116,
            /// <summary>#define ERROR_IO_DEVICE 1117L</summary>
            ERROR_IO_DEVICE = 1117,
            /// <summary>#define ERROR_SERIAL_NO_DEVICE 1118L</summary>
            ERROR_SERIAL_NO_DEVICE = 1118,
            /// <summary>#define ERROR_IRQ_BUSY 1119L</summary>
            ERROR_IRQ_BUSY = 1119,
            /// <summary>#define ERROR_MORE_WRITES 1120L</summary>
            ERROR_MORE_WRITES = 1120,
            /// <summary>#define ERROR_COUNTER_TIMEOUT 1121L</summary>
            ERROR_COUNTER_TIMEOUT = 1121,
            /// <summary>#define ERROR_FLOPPY_ID_MARK_NOT_FOUND 1122L</summary>
            ERROR_FLOPPY_ID_MARK_NOT_FOUND = 1122,
            /// <summary>#define ERROR_FLOPPY_WRONG_CYLINDER 1123L</summary>
            ERROR_FLOPPY_WRONG_CYLINDER = 1123,
            /// <summary>#define ERROR_FLOPPY_UNKNOWN_ERROR 1124L</summary>
            ERROR_FLOPPY_UNKNOWN_ERROR = 1124,
            /// <summary>#define ERROR_FLOPPY_BAD_REGISTERS 1125L</summary>
            ERROR_FLOPPY_BAD_REGISTERS = 1125,
            /// <summary>#define ERROR_DISK_RECALIBRATE_FAILED 1126L</summary>
            ERROR_DISK_RECALIBRATE_FAILED = 1126,
            /// <summary>#define ERROR_DISK_OPERATION_FAILED 1127L</summary>
            ERROR_DISK_OPERATION_FAILED = 1127,
            /// <summary>#define ERROR_DISK_RESET_FAILED 1128L</summary>
            ERROR_DISK_RESET_FAILED = 1128,
            /// <summary>#define ERROR_EOM_OVERFLOW 1129L</summary>
            ERROR_EOM_OVERFLOW = 1129,
            /// <summary>#define ERROR_NOT_ENOUGH_SERVER_MEMORY 1130L</summary>
            ERROR_NOT_ENOUGH_SERVER_MEMORY = 1130,
            /// <summary>#define ERROR_POSSIBLE_DEADLOCK 1131L</summary>
            ERROR_POSSIBLE_DEADLOCK = 1131,
            /// <summary>#define ERROR_MAPPED_ALIGNMENT 1132L</summary>
            ERROR_MAPPED_ALIGNMENT = 1132,
            /// <summary>#define ERROR_SET_POWER_STATE_VETOED 1140L</summary>
            ERROR_SET_POWER_STATE_VETOED = 1140,
            /// <summary>#define ERROR_SET_POWER_STATE_FAILED 1141L</summary>
            ERROR_SET_POWER_STATE_FAILED = 1141,
            /// <summary>#define ERROR_TOO_MANY_LINKS 1142L</summary>
            ERROR_TOO_MANY_LINKS = 1142,
            /// <summary>#define ERROR_OLD_WIN_VERSION 1150L</summary>
            ERROR_OLD_WIN_VERSION = 1150,
            /// <summary>#define ERROR_APP_WRONG_OS 1151L</summary>
            ERROR_APP_WRONG_OS = 1151,
            /// <summary>#define ERROR_SINGLE_INSTANCE_APP 1152L</summary>
            ERROR_SINGLE_INSTANCE_APP = 1152,
            /// <summary>#define ERROR_RMODE_APP 1153L</summary>
            ERROR_RMODE_APP = 1153,
            /// <summary>#define ERROR_INVALID_DLL 1154L</summary>
            ERROR_INVALID_DLL = 1154,
            /// <summary>#define ERROR_NO_ASSOCIATION 1155L</summary>
            ERROR_NO_ASSOCIATION = 1155,
            /// <summary>#define ERROR_DDE_FAIL 1156L</summary>
            ERROR_DDE_FAIL = 1156,
            /// <summary>#define ERROR_DLL_NOT_FOUND 1157L</summary>
            ERROR_DLL_NOT_FOUND = 1157,
            /// <summary>#define ERROR_NO_MORE_USER_HANDLES 1158L</summary>
            ERROR_NO_MORE_USER_HANDLES = 1158,
            /// <summary>#define ERROR_MESSAGE_SYNC_ONLY 1159L</summary>
            ERROR_MESSAGE_SYNC_ONLY = 1159,
            /// <summary>#define ERROR_SOURCE_ELEMENT_EMPTY 1160L</summary>
            ERROR_SOURCE_ELEMENT_EMPTY = 1160,
            /// <summary>#define ERROR_DESTINATION_ELEMENT_FULL 1161L</summary>
            ERROR_DESTINATION_ELEMENT_FULL = 1161,
            /// <summary>#define ERROR_ILLEGAL_ELEMENT_ADDRESS 1162L</summary>
            ERROR_ILLEGAL_ELEMENT_ADDRESS = 1162,
            /// <summary>#define ERROR_MAGAZINE_NOT_PRESENT 1163L</summary>
            ERROR_MAGAZINE_NOT_PRESENT = 1163,
            /// <summary>#define ERROR_DEVICE_REINITIALIZATION_NEEDED 1164L</summary>
            ERROR_DEVICE_REINITIALIZATION_NEEDED = 1164,
            /// <summary>#define ERROR_DEVICE_REQUIRES_CLEANING 1165L</summary>
            ERROR_DEVICE_REQUIRES_CLEANING = 1165,
            /// <summary>#define ERROR_DEVICE_DOOR_OPEN 1166L</summary>
            ERROR_DEVICE_DOOR_OPEN = 1166,
            /// <summary>#define ERROR_DEVICE_NOT_CONNECTED 1167L</summary>
            ERROR_DEVICE_NOT_CONNECTED = 1167,
            /// <summary>#define ERROR_NOT_FOUND 1168L</summary>
            ERROR_NOT_FOUND = 1168,
            /// <summary>#define ERROR_NO_MATCH 1169L</summary>
            ERROR_NO_MATCH = 1169,
            /// <summary>#define ERROR_SET_NOT_FOUND 1170L</summary>
            ERROR_SET_NOT_FOUND = 1170,
            /// <summary>#define ERROR_POINT_NOT_FOUND 1171L</summary>
            ERROR_POINT_NOT_FOUND = 1171,
            /// <summary>#define ERROR_NO_TRACKING_SERVICE 1172L</summary>
            ERROR_NO_TRACKING_SERVICE = 1172,
            /// <summary>#define ERROR_NO_VOLUME_ID 1173L</summary>
            ERROR_NO_VOLUME_ID = 1173,
            /// <summary>#define ERROR_UNABLE_TO_REMOVE_REPLACED 1175L</summary>
            ERROR_UNABLE_TO_REMOVE_REPLACED = 1175,
            /// <summary>#define ERROR_UNABLE_TO_MOVE_REPLACEMENT 1176L</summary>
            ERROR_UNABLE_TO_MOVE_REPLACEMENT = 1176,
            /// <summary>#define ERROR_UNABLE_TO_MOVE_REPLACEMENT_2 1177L</summary>
            ERROR_UNABLE_TO_MOVE_REPLACEMENT_2 = 1177,
            /// <summary>#define ERROR_JOURNAL_DELETE_IN_PROGRESS 1178L</summary>
            ERROR_JOURNAL_DELETE_IN_PROGRESS = 1178,
            /// <summary>#define ERROR_JOURNAL_NOT_ACTIVE 1179L</summary>
            ERROR_JOURNAL_NOT_ACTIVE = 1179,
            /// <summary>#define ERROR_POTENTIAL_FILE_FOUND 1180L</summary>
            ERROR_POTENTIAL_FILE_FOUND = 1180,
            /// <summary>#define ERROR_JOURNAL_ENTRY_DELETED 1181L</summary>
            ERROR_JOURNAL_ENTRY_DELETED = 1181,
            /// <summary>#define ERROR_SHUTDOWN_IS_SCHEDULED 1190L</summary>
            ERROR_SHUTDOWN_IS_SCHEDULED = 1190,
            /// <summary>#define ERROR_SHUTDOWN_USERS_LOGGED_ON 1191L</summary>
            ERROR_SHUTDOWN_USERS_LOGGED_ON = 1191,
            /// <summary>#define ERROR_BAD_DEVICE 1200L</summary>
            ERROR_BAD_DEVICE = 1200,
            /// <summary>#define ERROR_CONNECTION_UNAVAIL 1201L</summary>
            ERROR_CONNECTION_UNAVAIL = 1201,
            /// <summary>#define ERROR_DEVICE_ALREADY_REMEMBERED 1202L</summary>
            ERROR_DEVICE_ALREADY_REMEMBERED = 1202,
            /// <summary>#define ERROR_NO_NET_OR_BAD_PATH 1203L</summary>
            ERROR_NO_NET_OR_BAD_PATH = 1203,
            /// <summary>#define ERROR_BAD_PROVIDER 1204L</summary>
            ERROR_BAD_PROVIDER = 1204,
            /// <summary>#define ERROR_CANNOT_OPEN_PROFILE 1205L</summary>
            ERROR_CANNOT_OPEN_PROFILE = 1205,
            /// <summary>#define ERROR_BAD_PROFILE 1206L</summary>
            ERROR_BAD_PROFILE = 1206,
            /// <summary>#define ERROR_NOT_CONTAINER 1207L</summary>
            ERROR_NOT_CONTAINER = 1207,
            /// <summary>#define ERROR_EXTENDED_ERROR 1208L</summary>
            ERROR_EXTENDED_ERROR = 1208,
            /// <summary>#define ERROR_INVALID_GROUPNAME 1209L</summary>
            ERROR_INVALID_GROUPNAME = 1209,
            /// <summary>#define ERROR_INVALID_COMPUTERNAME 1210L</summary>
            ERROR_INVALID_COMPUTERNAME = 1210,
            /// <summary>#define ERROR_INVALID_EVENTNAME 1211L</summary>
            ERROR_INVALID_EVENTNAME = 1211,
            /// <summary>#define ERROR_INVALID_DOMAINNAME 1212L</summary>
            ERROR_INVALID_DOMAINNAME = 1212,
            /// <summary>#define ERROR_INVALID_SERVICENAME 1213L</summary>
            ERROR_INVALID_SERVICENAME = 1213,
            /// <summary>#define ERROR_INVALID_NETNAME 1214L</summary>
            ERROR_INVALID_NETNAME = 1214,
            /// <summary>#define ERROR_INVALID_SHARENAME 1215L</summary>
            ERROR_INVALID_SHARENAME = 1215,
            /// <summary>#define ERROR_INVALID_PASSWORDNAME 1216L</summary>
            ERROR_INVALID_PASSWORDNAME = 1216,
            /// <summary>#define ERROR_INVALID_MESSAGENAME 1217L</summary>
            ERROR_INVALID_MESSAGENAME = 1217,
            /// <summary>#define ERROR_INVALID_MESSAGEDEST 1218L</summary>
            ERROR_INVALID_MESSAGEDEST = 1218,
            /// <summary>#define ERROR_SESSION_CREDENTIAL_CONFLICT 1219L</summary>
            ERROR_SESSION_CREDENTIAL_CONFLICT = 1219,
            /// <summary>#define ERROR_REMOTE_SESSION_LIMIT_EXCEEDED 1220L</summary>
            ERROR_REMOTE_SESSION_LIMIT_EXCEEDED = 1220,
            /// <summary>#define ERROR_DUP_DOMAINNAME 1221L</summary>
            ERROR_DUP_DOMAINNAME = 1221,
            /// <summary>#define ERROR_NO_NETWORK 1222L</summary>
            ERROR_NO_NETWORK = 1222,
            /// <summary>#define ERROR_CANCELLED 1223L</summary>
            ERROR_CANCELLED = 1223,
            /// <summary>#define ERROR_USER_MAPPED_FILE 1224L</summary>
            ERROR_USER_MAPPED_FILE = 1224,
            /// <summary>#define ERROR_CONNECTION_REFUSED 1225L</summary>
            ERROR_CONNECTION_REFUSED = 1225,
            /// <summary>#define ERROR_GRACEFUL_DISCONNECT 1226L</summary>
            ERROR_GRACEFUL_DISCONNECT = 1226,
            /// <summary>#define ERROR_ADDRESS_ALREADY_ASSOCIATED 1227L</summary>
            ERROR_ADDRESS_ALREADY_ASSOCIATED = 1227,
            /// <summary>#define ERROR_ADDRESS_NOT_ASSOCIATED 1228L</summary>
            ERROR_ADDRESS_NOT_ASSOCIATED = 1228,
            /// <summary>#define ERROR_CONNECTION_INVALID 1229L</summary>
            ERROR_CONNECTION_INVALID = 1229,
            /// <summary>#define ERROR_CONNECTION_ACTIVE 1230L</summary>
            ERROR_CONNECTION_ACTIVE = 1230,
            /// <summary>#define ERROR_NETWORK_UNREACHABLE 1231L</summary>
            ERROR_NETWORK_UNREACHABLE = 1231,
            /// <summary>#define ERROR_HOST_UNREACHABLE 1232L</summary>
            ERROR_HOST_UNREACHABLE = 1232,
            /// <summary>#define ERROR_PROTOCOL_UNREACHABLE 1233L</summary>
            ERROR_PROTOCOL_UNREACHABLE = 1233,
            /// <summary>#define ERROR_PORT_UNREACHABLE 1234L</summary>
            ERROR_PORT_UNREACHABLE = 1234,
            /// <summary>#define ERROR_REQUEST_ABORTED 1235L</summary>
            ERROR_REQUEST_ABORTED = 1235,
            /// <summary>#define ERROR_CONNECTION_ABORTED 1236L</summary>
            ERROR_CONNECTION_ABORTED = 1236,
            /// <summary>#define ERROR_RETRY 1237L</summary>
            ERROR_RETRY = 1237,
            /// <summary>#define ERROR_CONNECTION_COUNT_LIMIT 1238L</summary>
            ERROR_CONNECTION_COUNT_LIMIT = 1238,
            /// <summary>#define ERROR_LOGIN_TIME_RESTRICTION 1239L</summary>
            ERROR_LOGIN_TIME_RESTRICTION = 1239,
            /// <summary>#define ERROR_LOGIN_WKSTA_RESTRICTION 1240L</summary>
            ERROR_LOGIN_WKSTA_RESTRICTION = 1240,
            /// <summary>#define ERROR_INCORRECT_ADDRESS 1241L</summary>
            ERROR_INCORRECT_ADDRESS = 1241,
            /// <summary>#define ERROR_ALREADY_REGISTERED 1242L</summary>
            ERROR_ALREADY_REGISTERED = 1242,
            /// <summary>#define ERROR_SERVICE_NOT_FOUND 1243L</summary>
            ERROR_SERVICE_NOT_FOUND = 1243,
            /// <summary>#define ERROR_NOT_AUTHENTICATED 1244L</summary>
            ERROR_NOT_AUTHENTICATED = 1244,
            /// <summary>#define ERROR_NOT_LOGGED_ON 1245L</summary>
            ERROR_NOT_LOGGED_ON = 1245,
            /// <summary>#define ERROR_CONTINUE 1246L</summary>
            ERROR_CONTINUE = 1246,
            /// <summary>#define ERROR_ALREADY_INITIALIZED 1247L</summary>
            ERROR_ALREADY_INITIALIZED = 1247,
            /// <summary>#define ERROR_NO_MORE_DEVICES 1248L</summary>
            ERROR_NO_MORE_DEVICES = 1248,
            /// <summary>#define ERROR_NO_SUCH_SITE 1249L</summary>
            ERROR_NO_SUCH_SITE = 1249,
            /// <summary>#define ERROR_DOMAIN_CONTROLLER_EXISTS 1250L</summary>
            ERROR_DOMAIN_CONTROLLER_EXISTS = 1250,
            /// <summary>#define ERROR_ONLY_IF_CONNECTED 1251L</summary>
            ERROR_ONLY_IF_CONNECTED = 1251,
            /// <summary>#define ERROR_OVERRIDE_NOCHANGES 1252L</summary>
            ERROR_OVERRIDE_NOCHANGES = 1252,
            /// <summary>#define ERROR_BAD_USER_PROFILE 1253L</summary>
            ERROR_BAD_USER_PROFILE = 1253,
            /// <summary>#define ERROR_NOT_SUPPORTED_ON_SBS 1254L</summary>
            ERROR_NOT_SUPPORTED_ON_SBS = 1254,
            /// <summary>#define ERROR_SERVER_SHUTDOWN_IN_PROGRESS 1255L</summary>
            ERROR_SERVER_SHUTDOWN_IN_PROGRESS = 1255,
            /// <summary>#define ERROR_HOST_DOWN 1256L</summary>
            ERROR_HOST_DOWN = 1256,
            /// <summary>#define ERROR_NON_ACCOUNT_SID 1257L</summary>
            ERROR_NON_ACCOUNT_SID = 1257,
            /// <summary>#define ERROR_NON_DOMAIN_SID 1258L</summary>
            ERROR_NON_DOMAIN_SID = 1258,
            /// <summary>#define ERROR_APPHELP_BLOCK 1259L</summary>
            ERROR_APPHELP_BLOCK = 1259,
            /// <summary>#define ERROR_ACCESS_DISABLED_BY_POLICY 1260L</summary>
            ERROR_ACCESS_DISABLED_BY_POLICY = 1260,
            /// <summary>#define ERROR_REG_NAT_CONSUMPTION 1261L</summary>
            ERROR_REG_NAT_CONSUMPTION = 1261,
            /// <summary>#define ERROR_CSCSHARE_OFFLINE 1262L</summary>
            ERROR_CSCSHARE_OFFLINE = 1262,
            /// <summary>#define ERROR_PKINIT_FAILURE 1263L</summary>
            ERROR_PKINIT_FAILURE = 1263,
            /// <summary>#define ERROR_SMARTCARD_SUBSYSTEM_FAILURE 1264L</summary>
            ERROR_SMARTCARD_SUBSYSTEM_FAILURE = 1264,
            /// <summary>#define ERROR_DOWNGRADE_DETECTED 1265L</summary>
            ERROR_DOWNGRADE_DETECTED = 1265,
            /// <summary>#define ERROR_MACHINE_LOCKED 1271L</summary>
            ERROR_MACHINE_LOCKED = 1271,
            /// <summary>#define ERROR_CALLBACK_SUPPLIED_INVALID_DATA 1273L</summary>
            ERROR_CALLBACK_SUPPLIED_INVALID_DATA = 1273,
            /// <summary>#define ERROR_SYNC_FOREGROUND_REFRESH_REQUIRED 1274L</summary>
            ERROR_SYNC_FOREGROUND_REFRESH_REQUIRED = 1274,
            /// <summary>#define ERROR_DRIVER_BLOCKED 1275L</summary>
            ERROR_DRIVER_BLOCKED = 1275,
            /// <summary>#define ERROR_INVALID_IMPORT_OF_NON_DLL 1276L</summary>
            ERROR_INVALID_IMPORT_OF_NON_DLL = 1276,
            /// <summary>#define ERROR_ACCESS_DISABLED_WEBBLADE 1277L</summary>
            ERROR_ACCESS_DISABLED_WEBBLADE = 1277,
            /// <summary>#define ERROR_ACCESS_DISABLED_WEBBLADE_TAMPER 1278L</summary>
            ERROR_ACCESS_DISABLED_WEBBLADE_TAMPER = 1278,
            /// <summary>#define ERROR_RECOVERY_FAILURE 1279L</summary>
            ERROR_RECOVERY_FAILURE = 1279,
            /// <summary>#define ERROR_ALREADY_FIBER 1280L</summary>
            ERROR_ALREADY_FIBER = 1280,
            /// <summary>#define ERROR_ALREADY_THREAD 1281L</summary>
            ERROR_ALREADY_THREAD = 1281,
            /// <summary>#define ERROR_STACK_BUFFER_OVERRUN 1282L</summary>
            ERROR_STACK_BUFFER_OVERRUN = 1282,
            /// <summary>#define ERROR_PARAMETER_QUOTA_EXCEEDED 1283L</summary>
            ERROR_PARAMETER_QUOTA_EXCEEDED = 1283,
            /// <summary>#define ERROR_DEBUGGER_INACTIVE 1284L</summary>
            ERROR_DEBUGGER_INACTIVE = 1284,
            /// <summary>#define ERROR_DELAY_LOAD_FAILED 1285L</summary>
            ERROR_DELAY_LOAD_FAILED = 1285,
            /// <summary>#define ERROR_VDM_DISALLOWED 1286L</summary>
            ERROR_VDM_DISALLOWED = 1286,
            /// <summary>#define ERROR_UNIDENTIFIED_ERROR 1287L</summary>
            ERROR_UNIDENTIFIED_ERROR = 1287,
            /// <summary>#define ERROR_INVALID_CRUNTIME_PARAMETER 1288L</summary>
            ERROR_INVALID_CRUNTIME_PARAMETER = 1288,
            /// <summary>#define ERROR_BEYOND_VDL 1289L</summary>
            ERROR_BEYOND_VDL = 1289,
            /// <summary>#define ERROR_INCOMPATIBLE_SERVICE_SID_TYPE 1290L</summary>
            ERROR_INCOMPATIBLE_SERVICE_SID_TYPE = 1290,
            /// <summary>#define ERROR_DRIVER_PROCESS_TERMINATED 1291L</summary>
            ERROR_DRIVER_PROCESS_TERMINATED = 1291,
            /// <summary>#define ERROR_IMPLEMENTATION_LIMIT 1292L</summary>
            ERROR_IMPLEMENTATION_LIMIT = 1292,
            /// <summary>#define ERROR_PROCESS_IS_PROTECTED 1293L</summary>
            ERROR_PROCESS_IS_PROTECTED = 1293,
            /// <summary>#define ERROR_SERVICE_NOTIFY_CLIENT_LAGGING 1294L</summary>
            ERROR_SERVICE_NOTIFY_CLIENT_LAGGING = 1294,
            /// <summary>#define ERROR_DISK_QUOTA_EXCEEDED 1295L</summary>
            ERROR_DISK_QUOTA_EXCEEDED = 1295,
            /// <summary>#define ERROR_CONTENT_BLOCKED 1296L</summary>
            ERROR_CONTENT_BLOCKED = 1296,
            /// <summary>#define ERROR_INCOMPATIBLE_SERVICE_PRIVILEGE 1297L</summary>
            ERROR_INCOMPATIBLE_SERVICE_PRIVILEGE = 1297,
            /// <summary>#define ERROR_APP_HANG 1298L</summary>
            ERROR_APP_HANG = 1298,
        }

        #endregion

        #endregion
    }

    #endregion
}

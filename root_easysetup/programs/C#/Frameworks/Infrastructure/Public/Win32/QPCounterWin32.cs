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
//* クラス名        ：QPCounterWin32
//* クラス日本語名  ：高分解能パフォーマンスカウンタ 関連Win32 API宣言クラス
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
    /// <summary>
    /// 高分解能パフォーマンスカウンタ 関連Win32 API宣言クラス
    /// </summary>
    public class QPCounterWin32
    {
        /// <summary>
        /// Win32 API関数（QueryPerformanceCounter）のプロトタイプ宣言
        /// </summary>
        /// <returns>ハードウェアが高分解能パフォーマンスカウンタをサポートしている場合、0 以外の値が返る。</returns>
        /// <remarks>
        /// 高分解能パフォーマンスカウンタの現在の値が格納される。
        /// ハードウェアが高分解能パフォーマンスカウンタをサポートしていない場合、0 が格納されることがある。
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int16 QueryPerformanceCounter(ref long x);

        /// <summary>Win32 API関数（QueryPerformanceFrequency）のプロトタイプ宣言</summary>
        /// <returns>ハードウェアが高分解能パフォーマンスカウンタをサポートしている場合、0 以外の値が返る。</returns>
        /// <remarks>
        /// 高分解能パフォーマンスカウンタが存在する場合、カウンタの周波数（更新頻度）を取得する。
        /// 周波数は、1 秒あたりのカウント数として表現される。
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int16 QueryPerformanceFrequency(ref long x);

        /// <summary>
        /// Win32 API関数（GetThreadTimes）のプロトタイプ宣言
        /// </summary>
        /// <param name="ipr">カレントスレッドのハンドラ</param>
        /// <param name="ftCreateTime">スレッドの作成時刻（FILETIME構造体へのポインタ）</param>
        /// <param name="ftDELETETime">スレッドの終了時刻（FILETIME構造体へのポインタ）</param>
        /// <param name="ftKernelTime">カーネル時間（FILETIME構造体へのポインタ）</param>
        /// <param name="ftUserTime">ユーザ時間（FILETIME構造体へのポインタ）</param>
        /// <remarks>
        /// このAPIを使用して、CPUの
        /// ・カーネル時間
        /// ・ユーザ時間
        /// を取得する。
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void GetThreadTimes(
            IntPtr ipr,
            ref System.Runtime.InteropServices.ComTypes.FILETIME ftCreateTime,
            ref System.Runtime.InteropServices.ComTypes.FILETIME ftDELETETime,
            ref System.Runtime.InteropServices.ComTypes.FILETIME ftKernelTime,
            ref System.Runtime.InteropServices.ComTypes.FILETIME ftUserTime);
    }
}

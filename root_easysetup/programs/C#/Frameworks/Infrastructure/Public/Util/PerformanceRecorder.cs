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
//* クラス名        ：PerformanceRecorder
//* クラス日本語名  ：パフォーマンス測定クラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2007/xx/xx  西野  大介        新規作成
//*  2010/10/26  西野  大介        場所移動（Win32置き場新設）
//**********************************************************************************

// 性能測定に必要
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

// System
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections;

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
    /// <summary>パフォーマンス測定クラス</summary>
    /// <remarks>
    /// 自由に利用できる。
    /// 
    /// 以下の測定項目を記録する。
    /// ●実行時間
    /// ●CPU時間
    /// 　・CPU（カーネル）時間
    /// 　・CPU（ユーザ）時間
    /// 
    /// 記録方式に、以下の方式を利用できる。
    /// ●ファイル出力
    /// ●カウンタ出力
    /// 
    /// 精度について
    /// ●実行時間  ⇒ 高分解能パフォーマンスカウンタカウンタの周波数（更新頻度）にあわせた精度で取得する。
    /// ●CPU時間   ⇒ 0001年1月1日午前12:00から経過した100ns間隔のタイマ刻み数だが、10msec前後の誤差あり。
    /// </remarks>
	public class PerformanceRecorder
    {
        #region 測定結果の保存用メンバ変数

        /// <summary>
        /// 実行時間
        /// </summary>
        private string _ExecTime = "";

        /// <summary>
        /// CPU時間
        /// </summary>
        private string _CpuTime = "";

        /// <summary>
        /// CPUカーネル モード時間
        /// </summary>
        private string _CpuKernelTime = "";

        /// <summary>
        /// CPUユーザ モード時間
        /// </summary>
        private string _CpuUserTime = "";

        #endregion

        #region 測定値計算用定数宣言 セクション

        /// <summary>
        /// FILETIME構造体のdwHighDateTime処理用定数
        /// 2~32乗+1をdwHighDateTimeに×て、64bitの10進数に戻す。
        /// </summary>
        private const UInt64 U64HIBIT = 4294967296;

        #endregion

        #region 測定値計算用変数宣言 セクション

        /// <summary>
        /// 実行時間の開始時間を保持する。
        /// ● ハードウェアが高分解能パフォーマンスカウンタをサポートしている場合、
        ///    APIからは、カウンタの周波数（更新頻度）にあわせた精度で取得する。
        /// 
        /// ● ハードウェアが高分解能パフォーマンスカウンタをサポートしていない場合、
        ///    APIからは、0001年1月1日午前12:00から経過した
        ///    100ns間隔のタイマ刻み数で取得する ⇒ ただし精度は10msecと悪い。
        /// </summary>
        private UInt64 U64ExTS;

        /// <summary>
        /// QueryPerformanceCounter用のワーク変数
        /// </summary>
        private long LngExTS;

        /// <summary>
        /// CPU（カーネル）時間の開始時間を保持する。
        /// APIからは、0001年1月1日午前12:00から経過した100ns間隔のタイマ刻み数で取得する⇒ただし精度は10msecと悪い。</summary>
        private UInt64 U64KTS;


        /// <summary>
        /// CPU（ユーザ）時間の開始時間を保持する。
        /// APIからは、0001年1月1日午前12:00から経過した100ns間隔のタイマ刻み数で取得する⇒ただし精度は10msecと悪い。</summary>
        private UInt64 U64UTS;

        #endregion

        #region 測定開始メソッド

        /// <summary>測定開始メソッド</summary>
        /// <returns>処理が成功した場合：True、失敗した場合：False</returns>
        /// <remarks>自由に利用できる。</remarks>
        public bool StartsPerformanceRecord()
        {

            #region メンバ変数を初期化する

            // 実行時間
            this.U64ExTS = 0;
            this.LngExTS = 0; // QueryPerformanceCounter用

            // CPU時間
            this.U64KTS = 0;
            this.U64UTS = 0;

            // 測定結果の保存用メンバ変数の初期化
            this._ExecTime = "";
            this._CpuTime = "";
            this._CpuKernelTime = "";
            this._CpuUserTime = "";

            #endregion

            try
            {
                #region 実行時間取得処理セクション

                // システム時刻(Start)

                if (QPCounterWin32.QueryPerformanceCounter(ref LngExTS) != 0)	// 時間の計測を開始します。
                {
                }
                else// 高分解能のカウンタはサポートされません。
                {
                    // 100ns間隔（精度は低い）
                    U64ExTS = Convert.ToUInt64(DateTime.Now.Ticks);
                }

                #endregion

                #region CPU時間取得処理セクション

                // カレントスレッドのハンドルを返す（IDではないので注意）。
                IntPtr iptHdr;
                iptHdr = CmnWin32.GetCurrentThread();

                // スレッドの作成時刻
                System.Runtime.InteropServices.ComTypes.FILETIME ftCreateTime;
                // スレッドの終了時刻
                System.Runtime.InteropServices.ComTypes.FILETIME ftDELETETime;
                // カーネル時間
                System.Runtime.InteropServices.ComTypes.FILETIME ftKernelTime;
                // ユーザ時間
                System.Runtime.InteropServices.ComTypes.FILETIME ftUserTime;

                // 初期化が必要？
                ftCreateTime.dwHighDateTime = 0;
                ftCreateTime.dwLowDateTime = 0;
                ftDELETETime.dwHighDateTime = 0;
                ftDELETETime.dwLowDateTime = 0;
                ftKernelTime.dwHighDateTime = 0;
                ftKernelTime.dwLowDateTime = 0;
                ftUserTime.dwHighDateTime = 0;
                ftUserTime.dwLowDateTime = 0;

                // Win32 API関数（GetCurrentThread）
                QPCounterWin32.GetThreadTimes(iptHdr, ref ftCreateTime, ref ftDELETETime,
                    ref ftKernelTime, ref ftUserTime);

                // 計算用の領域
                UInt32 u32KTH;
                UInt32 u32KTL;
                UInt32 u32UTH;
                UInt32 u32UTL;

                // 変換（int32 ⇒　uint32）
                u32KTH = Convert.ToUInt32(ftKernelTime.dwHighDateTime);
                u32KTL = Convert.ToUInt32(ftKernelTime.dwLowDateTime);
                u32UTH = Convert.ToUInt32(ftUserTime.dwHighDateTime);
                u32UTL = Convert.ToUInt32(ftUserTime.dwLowDateTime);

                // CPU時間：(uint64 * uint32) + uint64 ⇒　uint64（オーバーフローはしない）

                // カーネル時間(Start)
                U64KTS = Convert.ToUInt64((u32KTH * U64HIBIT) + u32KTL);

                // ユーザ時間(Start)
                U64UTS = Convert.ToUInt64((u32UTH * U64HIBIT) + u32UTL);

                #endregion
            }
            catch
            {
                // ランタイムエラー。
                return false;
            }
            finally
            {
            }

            return true;

        }

        #endregion

        #region 測定終了メソッド

        /// <summary>測定終了メソッド</summary>
        /// <returns>処理が成功した場合：結果文字列、失敗した場合：エラーメッセージ</returns>
        /// <remarks>自由に利用できる。</remarks>
        public string EndsPerformanceRecord()
        {
            try
            {
                #region 実行時間取得処理セクション

                // システム時刻(End)
                UInt64 u64ExTE = 0;

                long lngExTE = 0;

                if (QPCounterWin32.QueryPerformanceCounter(ref lngExTE) != 0)	// 時間の計測を開始します。
                {
                    // 周波数（更新頻度）を取得
                    long lngFreq = 0;
                    QPCounterWin32.QueryPerformanceFrequency(ref lngFreq);

                    // 秒単位
                    double dblTemp = ((lngExTE - LngExTS) * 1.0 / lngFreq);
                    // 1s → 100(ns)に合わせる。
                    dblTemp = dblTemp * 1000 * 1000 * 10;
                    // 整数値に変更
                    u64ExTE = (UInt64)Math.Round(dblTemp);
                }
                else// 高分解能のカウンタはサポートされません。
                {
                    // 100ns間隔（精度は低い）
                    u64ExTE = Convert.ToUInt64(DateTime.Now.Ticks);
                }
                
                #endregion

                #region CPU時間取得処理セクション

                // カレントスレッドのハンドルを返す（IDではないので注意）。
                IntPtr iptHdr;
                iptHdr = CmnWin32.GetCurrentThread();

                // スレッドの作成時刻
                System.Runtime.InteropServices.ComTypes.FILETIME ftCreateTime;
                // スレッドの終了時刻
                System.Runtime.InteropServices.ComTypes.FILETIME ftDELETETime;
                // カーネル時間
                System.Runtime.InteropServices.ComTypes.FILETIME ftKernelTime;
                // ユーザ時間
                System.Runtime.InteropServices.ComTypes.FILETIME ftUserTime;

                // 初期化が必要？
                ftCreateTime.dwHighDateTime = 0;
                ftCreateTime.dwLowDateTime = 0;
                ftDELETETime.dwHighDateTime = 0;
                ftDELETETime.dwLowDateTime = 0;
                ftKernelTime.dwHighDateTime = 0;
                ftKernelTime.dwLowDateTime = 0;
                ftUserTime.dwHighDateTime = 0;
                ftUserTime.dwLowDateTime = 0;

                // Win32 API関数（GetCurrentThread）
                QPCounterWin32.GetThreadTimes(iptHdr, ref ftCreateTime, ref ftDELETETime,
                    ref ftKernelTime, ref ftUserTime);

                // 計算用の領域
                UInt32 u32KTH;
                UInt32 u32KTL;
                UInt32 u32UTH;
                UInt32 u32UTL;

                // 変換（int32 ⇒　uint32）
                u32KTH = Convert.ToUInt32(ftKernelTime.dwHighDateTime);
                u32KTL = Convert.ToUInt32(ftKernelTime.dwLowDateTime);
                u32UTH = Convert.ToUInt32(ftUserTime.dwHighDateTime);
                u32UTL = Convert.ToUInt32(ftUserTime.dwLowDateTime);

                // CPU時間：(uint64 * uint32) + uint64 ⇒　uint64（オーバーフローはしない）
                // カーネル時間(End)
                UInt64 u64KTE;
                u64KTE = Convert.ToUInt64((u32KTH * U64HIBIT) + u32KTL);
                // ユーザ時間(End)
                UInt64 u64UTE;
                u64UTE = Convert.ToUInt64((u32UTH * U64HIBIT) + u32UTL);

                #endregion

                #region 出力文字列作成セクション

                // 当該処理の実行時間を算出
                UInt64 u64ExT;
                u64ExT = u64ExTE - U64ExTS;

                // 当該処理のCPU（カーネル）時間を算出
                UInt64 u64KT;
                u64KT = u64KTE - U64KTS;

                // 当該処理のCPU（ユーザ）時間を算出
                UInt64 u64UT;
                u64UT = u64UTE - U64UTS;

                // 当該処理のCPU時間を算出
                // ※ オーバーフローは無いはず．．．
                UInt64 u64CT;
                u64CT = u64KT + u64UT;

                // 初期化
                U64ExTS = 0;
                U64KTS = 0;
                U64UTS = 0;

                // 測定結果を文字列で返す
                double temp;
                
                // 四捨五入（msecの整数）
                temp = Math.Floor((u64ExT * 0.1 * 0.001) + 0.5);
                this._ExecTime = temp.ToString();

                temp = Math.Floor((u64CT * 0.1 * 0.001) + 0.5);
                this._CpuTime = temp.ToString();

                temp = Math.Floor((u64KT * 0.1 * 0.001) + 0.5);
                this._CpuKernelTime = temp.ToString();

                temp = Math.Floor((u64UT * 0.1 * 0.001) + 0.5);
                this._CpuUserTime = temp.ToString();

                return
                        "ExT:" + this._ExecTime + "[msec]" +
                        ", CT:" + this._CpuTime + "[msec]" +
                        ", KT:" + this._CpuKernelTime + "[msec]" +
                        ", UT:" + this._CpuUserTime + "[msec]";

                #endregion                
            }

            catch (Exception ex)
            {
                // ランタイムエラー。
                return ex.Message;
            }
        }

        #endregion

        #region プロパティ

        /// <summary>実行時間（ミリ秒）</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string ExecTime
        {
            get
            {
                return this._ExecTime;
            }
        }

        /// <summary>CPU時間（ミリ秒）</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string CpuTime
        {
            get
            {
                return this._CpuTime;
            }
        }

        /// <summary>CPUカーネル モード時間（ミリ秒）</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string CpuKernelTime
        {
            get
            {
                return this._CpuKernelTime;
            }
        }

        /// <summary>CPUユーザ モード時間（ミリ秒）</summary>
        /// <remarks>自由に利用できる。</remarks>
        public string CpuUserTime
        {
            get
            {
                return this._CpuUserTime;
            }
        }

        #endregion
    }
}

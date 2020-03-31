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
//* クラス名        ：ExponentialBackoff
//* クラス日本語名  ：ExponentialBackoffクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2020/03/16  西野 大介         新規作成
//**********************************************************************************

using System;
using System.Threading;
using Touryo.Infrastructure.Public.Util;

namespace Touryo.Infrastructure.Public.IO
{
    /// <summary>
    /// Exponential Backoff（指数バックオフ） - マイクロソフト系技術情報 Wiki
    /// https://techinfoofmicrosofttech.osscons.jp/index.php?Exponential%20Backoff%EF%BC%88%E6%8C%87%E6%95%B0%E3%83%90%E3%83%83%E3%82%AF%E3%82%AA%E3%83%95%EF%BC%89
    /// </summary>
    public class ExponentialBackoff
    {
        /// <summary>現在のリトライ間隔</summary>
        private int _current_retry_interval_msec = -1;

        /// <summary>現在のリトライ回数</summary>
        private int _current_retry_count = 0;
                
        #region 最大

        /// <summary>最大リトライ回数</summary>
        private int _maximum_retry_count = 0;

        /// <summary>
        /// リトライ間隔の最大値
        /// ・通常、32 秒または 64 秒
        /// ・適切な値はユースケースによって異なる。
        /// </summary>
        private int _maximum_backoff_msec = 0;
        
        #endregion

        /// <summary>constructor</summary>
        /// <param name="maximum_retry_count">int</param>
        public ExponentialBackoff(int maximum_retry_count) : this(maximum_retry_count, 32) { }

        /// <summary>constructor</summary>
        /// <param name="maximum_retry_count">int</param>
        /// <param name="maximum_backoff_seconds">int</param>
        public ExponentialBackoff(int maximum_retry_count, int maximum_backoff_seconds)
        {
            // maximum_backoff_secondsの既定値は32

            this._maximum_retry_count = maximum_retry_count;
            this._maximum_backoff_msec = maximum_backoff_seconds * 1000;
        }

        /// <summary>Sleep</summary>
        /// <returns>
        /// - true  : ループを継続する。
        /// - false : ループを終了する。
        /// </returns>
        public bool Sleep()
        {
            int temp = this.CalculateSleepIntervalMSec();

            if (temp == -1)
            {
                return false;
            }
            else
            {
                Thread.Sleep(temp);
                return true;
            }
        }

        /// <summary>GetSleepIntervalMSec</summary>
        /// <returns>int</returns>
        public int GetSleepIntervalMSec()
        {
            return this._current_retry_interval_msec;
        }

        /// <summary>CalculateSleepIntervalMSec</summary>
        /// <returns>int</returns>
        public int CalculateSleepIntervalMSec()
        {
            //Thread.Sleep(int millisecondsTimeout) // 引数はミリ秒

            // 基底のミリ秒
            int temp = (int)(Math.Pow(2, this._current_retry_count) * 1000);

            // 1,000 ミリ秒以下の乱数を足し込む。
            // 再試行リクエストの後に毎回再計算
            temp += (int)(RandomValueGenerator.GenerateRandomUint() % 1000);

            this._current_retry_count++;

            if (this._maximum_retry_count <= this._current_retry_count)
            {
                // 最大リトライ回数超過 → ループ終了
                this._current_retry_interval_msec = -1;
            }
            else
            {
                if (this._maximum_backoff_msec <= temp)
                {
                    // 最大リトライ間隔超過 → 最大値を返す。
                    this._current_retry_interval_msec = this._maximum_backoff_msec;
                }
                else
                {
                    // 計算値を返す。
                    this._current_retry_interval_msec = temp;
                }
            }

            return this._current_retry_interval_msec;
        }
    }
}

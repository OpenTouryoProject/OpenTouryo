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
//* クラス名        ：RandomValueGenerator
//* クラス日本語名  ：RandomValueGeneratorクラス
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//*
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2018/11/29  西野 大介         新規作成（非同期処理サービスから抜いた。
//**********************************************************************************

using System;
using System.Security.Cryptography;

namespace Touryo.Infrastructure.Public.Util
{
    /// <summary>RandomValueGeneratorクラス</summary>
    public class RandomValueGenerator
    {
        /// <summary>_lastProgressRate</summary>
        private uint _lastProgressRate = 0;
        /// <summary>LastProgressRate</summary>
        public uint LastProgressRate
        {
            get { return this._lastProgressRate; }
            private set { this._lastProgressRate = value; }
        }

        /// <summary>
        /// Generates new progress rate for the task based on last progress rate in increasing.
        /// </summary>
        /// <param name="maxProgressRate">Maximum progress rate.</param>
        /// <param name="upperLimitOfProgressRate">Upper limit of progress rate.</param>
        /// <returns>Incremented progress rate.</returns>
        public uint IncrementProgressRate(uint maxProgressRate, uint upperLimitOfProgressRate = 100)
        {
            // 乱数の、maxProgressRateの剰余を足し込む。
            this._lastProgressRate += (RandomValueGenerator.GenerateRandomUint() % maxProgressRate);

            if (upperLimitOfProgressRate <= this._lastProgressRate)
            {
                // 100%以上の場合、100%
                return upperLimitOfProgressRate;
            }
            else
            {
                // 100%未満の場合、その値
                return this._lastProgressRate;
            }
        }

        /// <summary>真偽の占い</summary>
        /// <returns>真・偽</returns>
        public static bool TrueOrFalse()
        {
            return ((RandomValueGenerator.GenerateRandomUint() % 2) == 0);
        }

        /// <summary>GenerateRandomUint</summary>
        /// <returns>Random uint</returns>
        public static uint GenerateRandomUint()
        {
            byte[] bs = new byte[sizeof(uint)];

            RandomNumberGenerator.Fill(bs);

            return BitConverter.ToUInt32(bs, 0);
        }
    }
}

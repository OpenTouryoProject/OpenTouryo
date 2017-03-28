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
//* クラス名        ：IGetValue
//* クラス日本語名  ：値取得のインターフェイス（テンプレート）
//*
//* 作成者          ：生技 西野
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  20xx/xx/xx  ＸＸ ＸＸ         新規作成（テンプレート）
//**********************************************************************************

using System;
using System.Diagnostics;
using System.Globalization;

namespace Touryo.Infrastructure.CustomControl
{
    /// <summary>値取得のインターフェイス（テンプレート）</summary>
    public interface IGetValue
    {
        /// <summary>
        /// Text値をDateTime型にキャストして返す。
        /// </summary>
        /// <returns>DateTime値</returns>
        [DebuggerStepThrough]
        DateTime GetDateTime();

        /// <summary>
        /// Text値をDateTime型にキャストして返す。
        /// </summary>
        /// <param name="provider">書式</param>
        /// <returns>DateTime値</returns>
        [DebuggerStepThrough]
        DateTime GetDateTime(IFormatProvider provider);

        /// <summary>
        /// Text値をDateTime型にキャストして返す。
        /// </summary>
        /// <param name="provider">書式</param>
        /// <param name="styles">スタイル</param>
        /// <returns>DateTime値</returns>
        [DebuggerStepThrough]
        DateTime GetDateTime(IFormatProvider provider, DateTimeStyles styles);

        /// <summary>
        /// Text値をDecimal型にキャストして返す。
        /// </summary>
        /// <returns>Decimal値</returns>
        [DebuggerStepThrough]
        decimal GetDecimal();

        /// <summary>
        /// Text値をDouble型にキャストして返す。
        /// </summary>
        /// <returns>Double値</returns>
        [DebuggerStepThrough]
        double GetDouble();

        /// <summary>
        /// Text値をFloat型にキャストして返す。
        /// </summary>
        /// <returns>Float値</returns>
        [DebuggerStepThrough]
        float GetFloat();

        /// <summary>
        /// Text値をInt16型にキャストして返す。
        /// </summary>
        /// <returns>Int16値</returns>
        [DebuggerStepThrough]
        short GetInt16();

        /// <summary>
        /// Text値をInt32型にキャストして返す。
        /// </summary>
        /// <returns>Int32値</returns>
        [DebuggerStepThrough]
        int GetInt32();

        /// <summary>
        /// Text値をInt64型にキャストして返す。
        /// </summary>
        /// <returns>Int64値</returns>
        [DebuggerStepThrough]
        long GetInt64();
    }
}
